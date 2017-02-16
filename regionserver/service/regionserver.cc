#include <iostream>
#include <atomic>
#include <memory>
#include <string>
#include <unistd.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <thread>
#include <grpc++/grpc++.h>
#include <tbb/concurrent_queue.h>
#include "regionservice.grpc.pb.h"

#define DEFAULT_IP "127.0.0.1"
#define DEFAULT_PORT 10400
#define UDP_WORKERS 50
#define UDP_QUEUES (UDP_WORKERS / 5)
#define QUEUE_SLEEP_MICROS (500 /* .5ms, only used when idle */)
//#define LOG_EVERYTHING

#define UDP_BUF_LEN 2048
#define UDP_IN_HDR_SIZE 12
#define UDP_OUT_HDR_SIZE 5
#define TCP_BUF_LEN (UDP_BUF_LEN + 1024)
#define SBOX_LEN 256

#define METRIC_UDP_RECV 0
#define METRIC_TCP_RECV 2
#define METRIC_TCP_SEND 3
#define METRIC_SECS 4
#define METRIC_TCP_CLIENTS 5
#define METRIC_UDP_ERROR 6
#define METRIC_MISSING_CLIENT 7
#define METRIC_DEDUPE 8
#define METRIC_ADD_CLIENT 9
#define METRIC_QUEUE_IN 11
#define METRIC_QUEUE_OUT 12
#define METRIC_MISSING_ENDPOINT 13

#define LEN_CLIENTS 65536
#define LEN_METRICS 14
#define GRPC_MAX_MESSAGE_SIZE 65536 /* needs to match on client */

#define STATS_REFRESH_SECS 1

using grpc::Server;
using grpc::ServerBuilder;
using grpc::ServerContext;
using grpc::ServerWriter;
using grpc::WriteOptions;
using grpc::Status;

int udpSocket, tcpSocket;
std::string our_ip = DEFAULT_IP;
uint16_t our_port = DEFAULT_PORT;
std::thread tcp_threads[100];
int tcp_thread_count = 0;
int sockSize = 1024 * 65536;

struct Client {
	uint16_t recvSequenceId; 
	uint16_t sendSequenceId; 
	uint8_t sbox[SBOX_LEN];
	uint32_t uniqueId;
	bool hasEndpoint, endpointVerified, known;
	struct sockaddr_in endpoint;
} clients[LEN_CLIENTS] = { 0 };

struct Metric {
	int64_t value;
	int64_t lastValue;
};

struct Metric metrics[LEN_METRICS] = { 0 };
struct Metric metrics_udp_in[256] = { 0 };
struct Metric metrics_udp_out[256] = { 0 };
std::atomic<int> queue_size;
std::atomic<uint64_t> udp_out;
uint64_t udp_out_last;

struct UdpBroadcast {
	uint16_t pakLen;
	uint8_t* pakBuf;
	uint16_t numSessions;
	uint16_t* sessions;
};
struct UdpFromClient {
	uint16_t pakLen;
	uint8_t* pakBuf;
};

// Debug helpers
#ifdef LOG_EVERYTHING
#define dbg(...) { printf("dbg: "); printf(__VA_ARGS__); printf("\n"); }
#else
#define dbg(...) {}
#endif

// -----------------------------------------------------------
// gRPC
// -----------------------------------------------------------
int generateSboxFromCryptKey(uint8_t *cryptKey, int cryptKeyLen, uint8_t *sbox);

class RegionServiceImpl final : public RegionService::Service {
	Status ServerInfo(ServerContext* context, const ServerInfoRequest* request, ServerInfoResponse* reply) override {
		dbg("rpc --> ServerInfo() start");
		reply->set_publicip4(our_ip);
		reply->set_udpport(our_port);
		dbg("rpc --> ServerInfo() end");
		return Status::OK;
	}
	Status AddClient(ServerContext* context, const AddClientRequest* request, AddClientResponse* reply) override {
		dbg("rpc --> AddClient() start");
		// TODO(mlinder): Check if already added
		Client oldClient = clients[request->sessionid()];		

		Client clt;
		memset(&clt, 0, sizeof(clt));

		clt.known = true;
		clt.uniqueId = request->uniqueid();
		clt.recvSequenceId = 0;
		clt.sendSequenceId = 0;

		clt.hasEndpoint = oldClient.hasEndpoint;
		clt.endpoint = oldClient.endpoint;
		clt.endpointVerified = false;

		uint16_t port;
		char host[256];
		if (request->endpoint().length() > 0 && sscanf(request->endpoint().c_str(), "%[^:]:%u", host, &port) == 2) {
			sockaddr_in ep;
			inet_pton(AF_INET, host, &ep);
			ep.sin_port = htons(port);
			clt.endpoint = ep;
		}

		generateSboxFromCryptKey((uint8_t*) request->encryptionkey().c_str(), request->encryptionkey().length(), &clt.sbox[0]);

		clients[request->sessionid()] = clt;
		metrics[METRIC_ADD_CLIENT].value++;
		dbg("rpc --> AddClient() end");
		return Status::OK;
	}
	Status RemoveClient(ServerContext* context, const RemoveClientRequest* request, RemoveClientResponse* reply) override {
		dbg("rpc --> RemoveClient() start");
		memset(&clients[request->sessionid()], 0, sizeof(Client));
		dbg("rpc --> RemoveClient() end");
		return Status::OK;
	}
	Status ClearAllClients(ServerContext* context, const ClearAllClientsRequest* request, ClearAllClientsResponse* reply) override {
		dbg("rpc --> ClearAllClients() start");
		memset(&clients, 0, sizeof(clients));
		dbg("rpc --> ClearAllClients() end");
		return Status::OK;
	}
	Status ListClients(ServerContext* context, const ListClientsRequest* request, ServerWriter<ClientInfo>* writer) override {
		dbg("rpc --> ListClients() start");
		WriteOptions wo;
		wo.set_buffer_hint();
		for (int i = 0; i < LEN_CLIENTS; i++) {
			Client clt = clients[i];
			
			ClientInfo info;
			info.set_sessionid(i);
			info.set_isknown(clt.known);
			info.set_uniqueid(clt.uniqueId);
			if (clt.hasEndpoint && clt.endpointVerified) {
				char ip[INET_ADDRSTRLEN];
				inet_ntop(AF_INET, &clt.endpoint.sin_addr, ip, INET_ADDRSTRLEN);
				uint16_t port = ntohs(clt.endpoint.sin_port);
				char str[256];
				sprintf(str, "%s:%u", ip, port);
				info.set_endpoint(str);
  			}
			writer->Write(info, wo);
		}
		dbg("rpc --> ListClients() end");
		return Status::OK;
	}
};

// -----------------------------------------------------------
// Helpers
// -----------------------------------------------------------
void die(std::string s)
{
	perror(s.c_str());
	exit(1);
}

uint16_t calculateUdpChecksum(uint8_t *buf, int offset, int len) {
	dbg("udp -> calculateUdpChecksum() start");
	uint8_t val1 = 0x7E, val2 = 0x7E;
	for (int i = offset; i < len; i++) {
		val1 += buf[i];
		val2 += val1;
	}
	dbg("udp -> calculateUdpChecksum() end");
	return val2 - ((val1 + val2) << 8);
}

int generateSboxFromCryptKey(uint8_t *cryptKey, int cryptKeyLen, uint8_t *sbox) {
	dbg("udp -> generateSboxFromCryptKey() start");
	for (int i = 0; i < SBOX_LEN; i++) {
		sbox[i] = (uint8_t) i;
	}
	uint8_t y = 0;
	for (int x = 0; x < SBOX_LEN; x++) {
		y += sbox[x] + cryptKey[x % cryptKeyLen];
		uint8_t tmp = sbox[x];
		sbox[x] = sbox[y];
		sbox[y] = tmp;
	}
	dbg("udp -> generateSboxFromCryptKey() end");
}

int encryptRC4(uint8_t* inBuf, int len, uint8_t* outBuf, Client* client) { // UNOPTIMIZED
	dbg("udp -> encryptRC4() start");
	// Create a copy of the sbox
	uint8_t sbox[SBOX_LEN];
	std::memcpy(sbox, &client->sbox, sizeof(sbox));

	uint8_t x = 0, y = 0, tmp = 0;

	// Skip header (TODO: Double check)
	int offset = 2;
	len -= offset;
	outBuf[0] = inBuf[0];
	outBuf[1] = inBuf[1];

	// Add sequence num (TODO: might require lock?)
	uint16_t seqNum = (++client->sendSequenceId);
	inBuf[2] = seqNum >> 8;
	inBuf[3] = seqNum;

	// Crypt data of the packet
	int midpoint = len / 2;
	int pos;
	for (pos = midpoint; pos < len; pos++) {
		x++;
		tmp = sbox[x];
		y += sbox[x];
		sbox[x] = sbox[y];
		sbox[y] = tmp;
		tmp = sbox[x] + sbox[y];
		y += inBuf[pos + offset];
		outBuf[pos + offset] = inBuf[pos + offset] ^ sbox[tmp];
	}
	for (pos = 0; pos < midpoint; pos++) {
		x++;
		y += sbox[x];
		tmp = sbox[x];
		sbox[x] = sbox[y];
		sbox[y] = tmp;
		tmp = sbox[x] + sbox[y];
		y += inBuf[pos + offset];
		outBuf[pos + offset] = inBuf[pos + offset] ^ sbox[tmp];
	}
	dbg("udp -> encryptRC4() end");
}

// -----------------------------------------------------------
// Queuing
// -----------------------------------------------------------

tbb::concurrent_queue<UdpFromClient> udp2tcpQueue; 
tbb::concurrent_queue<UdpBroadcast> tcp2udpQueue[UDP_QUEUES]; 

void enqueueReceivedUdp(uint8_t *buf, int len) {
	UdpFromClient udp;
	udp.pakBuf = (uint8_t*)malloc(len);
	udp.pakLen = len;
	std::memcpy(udp.pakBuf, buf, len);
	udp2tcpQueue.push(udp);
}

UdpFromClient dequeueReceivedUdp() {
	UdpFromClient udp;
	while (!udp2tcpQueue.try_pop(udp)) {
		usleep(QUEUE_SLEEP_MICROS);
	}
	return udp;
}

void broadcastUdp(UdpBroadcast broadcast, int i) {
	metrics[METRIC_QUEUE_IN].value++;
        queue_size++;
	tcp2udpQueue[i % UDP_QUEUES].push(broadcast);
}

UdpBroadcast dequeueUdpBroadcast(int threadNum) {
	UdpBroadcast udp;
	while (!tcp2udpQueue[threadNum % UDP_QUEUES].try_pop(udp)) {
		usleep(QUEUE_SLEEP_MICROS);
	}

	queue_size--;
	metrics[METRIC_QUEUE_OUT].value++;
	return udp;
}

// -----------------------------------------------------------
// Worker threads
// -----------------------------------------------------------

int tcpRecv(int socket, uint8_t * buf, int len) {
	int received = 0;
	while (received < len) {
		int remaining = len - received;
		dbg("tcpRecv(received=%d, len=%d, remaining=%d)", received, len, remaining);
		int read = recv(socket, buf + received, remaining, 0);
		if (read <= 0) { 
			dbg("tcpRecv(error=%d)", read);
			return false;
		}
		received += read;
	}
	dbg("tcpRecv(success, received=%d, len=%d)", received, len);
	return true;
}

void tcpReceiveWorker(int clientSocket) {
	printf("Starting tcpReceiveWorker(%d)\n", clientSocket);
	metrics[METRIC_TCP_CLIENTS].value++;
	uint8_t buf[TCP_BUF_LEN];
	int i = 0;

	while (1) {
		// Receive header
		if (!tcpRecv(clientSocket, buf, sizeof(short)*2)) break;
		uint16_t sessionsLen = (buf[0] << 8) | buf[1];
		uint16_t pakLen = (buf[2] << 8) | buf[3];
		dbg("tcp -> recv() DONE sessionsLen=%hu pakLen=%hu", sessionsLen, pakLen);

		// Receive data
		if (!tcpRecv(clientSocket, buf, sessionsLen + pakLen)) break;
		metrics[METRIC_TCP_RECV].value++;

		// Create udp broadcast
		UdpBroadcast broadcast;		
		broadcast.numSessions = sessionsLen / sizeof(uint16_t);
		broadcast.pakLen = pakLen;
		broadcast.sessions = (uint16_t*)std::malloc(sessionsLen);
		broadcast.pakBuf = (uint8_t*)std::malloc(pakLen);
		std::memcpy(broadcast.sessions, buf, sessionsLen);
		std::memcpy(broadcast.pakBuf, buf + sessionsLen, pakLen);
		dbg("tcp -> recv3() -> broadcastUdp()");
		broadcastUdp(broadcast, i++);
	}
	close(clientSocket);
	metrics[METRIC_TCP_CLIENTS].value--;
	printf("Ending tcpReceiveWorker(%d)\n", clientSocket);
}

void tcpSendWorker(int clientSocket) {
	printf("Starting tcpSendWorker(%d)\n", clientSocket);
	while (1) {
		UdpFromClient udp = dequeueReceivedUdp();
		int sent = 0;
		dbg("tcp -> send()");
		while (sent < udp.pakLen) {
			int written = send(clientSocket, udp.pakBuf + sent, udp.pakLen - sent, 0);
			if (written <= 0) break; // sock error
			sent += written;
		}
		metrics[METRIC_TCP_SEND].value++;
		free(udp.pakBuf);
	}
	printf("Ending tcpSendWorker(%d)\n", clientSocket);
}

void tcpAcceptWorker() {
	std::cout << "Starting tcpReceiveWorker()" << std::endl;
	while (1) {
		// Accept client
		struct sockaddr_in tcpClientAddr;
		socklen_t tcpClientAddrLen = sizeof(tcpClientAddr);
		int tcpClientSocket = accept(tcpSocket, (struct sockaddr *) &tcpClientAddr, &tcpClientAddrLen);
		if (tcpClientSocket < 0)
			die("tcp accept() failed");
		setsockopt(tcpClientSocket, SOL_SOCKET, SO_RCVBUF, &sockSize, sizeof(sockSize));
		setsockopt(tcpClientSocket, SOL_SOCKET, SO_SNDBUF, &sockSize, sizeof(sockSize));
	
		dbg("tcp -> accept()");
		printf("Accepted TCP client from %s\n", inet_ntoa(tcpClientAddr.sin_addr));
		tcp_threads[tcp_thread_count++] = std::thread(tcpReceiveWorker, tcpClientSocket);
		tcp_threads[tcp_thread_count++] = std::thread(tcpSendWorker, tcpClientSocket);
	}
	std::cout << "Ending tcpAcceptWorker()" << std::endl;
}

void udpReceiveWorker() {
	std::cout << "Starting udpReceiveWorker()" << std::endl;
	while (1) {
		uint8_t buf[UDP_BUF_LEN];
		struct sockaddr_in udpRemoteAddr;
		socklen_t slen = sizeof(udpRemoteAddr);
		int recv_len;

		if ((recv_len = recvfrom(udpSocket, buf, UDP_BUF_LEN, 0, (struct sockaddr *) &udpRemoteAddr, &slen)) == -1) die("recvfrom() failed");
		metrics[METRIC_UDP_RECV].value++;
		dbg("udp -> recvfrom()");

		if (recv_len < 2) { // too small
			metrics[METRIC_UDP_ERROR].value++;
			continue;
		}

		// Validate checksum
		uint16_t pakCheck = (buf[recv_len - 2] << 8) | buf[recv_len - 1];
		uint16_t calcCheck = calculateUdpChecksum(buf, 0, recv_len - 2);
		if (calcCheck != pakCheck) {
			metrics[METRIC_UDP_ERROR].value++;
			continue;
		}

		// Validate size
		uint16_t contentSize = (buf[0] << 8) | buf[1];
		if (contentSize + UDP_IN_HDR_SIZE != recv_len) {
			metrics[METRIC_UDP_ERROR].value++;
			continue;
		}

		// Parse packet
		uint16_t sequenceId = (buf[2] << 8) | buf[3];
		uint16_t sessionId = (buf[4] << 8) | buf[5];
		uint16_t packetType = (buf[8] << 8) | buf[9];

		metrics_udp_in[(uint8_t)packetType].value++;

		Client* client = &clients[sessionId];
		if (!client->endpointVerified) {
			// we add the endpoint even if the client is currently unknown (just in case there's a race condition on the server)
			client->endpoint = udpRemoteAddr;
			client->hasEndpoint = true;
			client->endpointVerified = true;
		}
		if (!client->known) {
			metrics[METRIC_MISSING_CLIENT].value++;
			continue;
		}
		if (sequenceId == client->recvSequenceId) {
			metrics[METRIC_DEDUPE].value++; // TODO(mlinder): Consider preserving packet order and doing an ordered de-dupe
			continue;
		}
		client->recvSequenceId = sequenceId;

		enqueueReceivedUdp(buf, recv_len);
	}
	std::cout << "Ending udpReceiveWorker()" << std::endl;
}

void udpSendWorker(int threadNum) {
	printf("Starting udpSendWorker(%d)\n", threadNum);
	uint8_t sendBuf[UDP_BUF_LEN];
	socklen_t epLen = sizeof(sockaddr_in);
	while (1) {
		UdpBroadcast bc = dequeueUdpBroadcast(threadNum);
		dbg("udp -> sendBroadcast(pakLen=%hu, numSessions=%hu)", bc.pakLen, bc.numSessions);

		// Fix content len
		uint16_t contentLen = bc.pakLen - UDP_OUT_HDR_SIZE;
		bc.pakBuf[0] = (contentLen >> 8);
		bc.pakBuf[1] = contentLen;

		// Send packet to clients ..
		for (int i = 0; i < bc.numSessions; i++) {
			dbg("udp -> send() clt");
			uint16_t sessionId = bc.sessions[i];
			Client* clt = &clients[sessionId];
			if (!clt->known) { // we cannot respond to unknown clients because we're missing the encryption key
				dbg("udp -> send() missing client=%hu", sessionId);
				metrics[METRIC_MISSING_CLIENT].value++;
				continue;
			}
			if (!clt->hasEndpoint) {
				dbg("udp -> send() missing ep=%hu", sessionId);
				metrics[METRIC_MISSING_ENDPOINT].value++;
				continue;
			}
			dbg("udp -> send() to i=%d client=%hu", i, sessionId);

			// Encryption + sequence id
			encryptRC4(bc.pakBuf, bc.pakLen, sendBuf, clt);

			// Send pak
			int res = sendto(udpSocket, sendBuf, bc.pakLen, 0, (struct sockaddr *)&clt->endpoint, epLen);	
			dbg("udp -> send() sendto sent=%d", res);
			udp_out++;
			metrics_udp_out[bc.pakBuf[4]].value++;

			// debug
#ifdef LOG_EVERYTHING
			printf("bc.pakBuf[%hu] =: ", bc.pakLen);
			for (int b = 0; b < bc.pakLen; b++) printf(" %02X", bc.pakBuf[b]);
			printf("\n");

			printf("sendBuf[%hu] =: ", bc.pakLen);
			for (int b = 0; b < bc.pakLen; b++) printf(" %02X", sendBuf[b]);
			printf("\n");
#endif
		}		

		free(bc.pakBuf);
		free(bc.sessions);
	}
	printf("Ending udpSendWorker(%d)\n", threadNum);
}

// -------------------------------------------------------------------
// GUI
// -------------------------------------------------------------------

int drawMetric(const char* name, Metric *metric) {
        int64_t value = (int64_t) metric->value;
	int64_t lastValue = metric->lastValue;
	metric->lastValue = value;
	double qps = (value - lastValue) * 1.0 / STATS_REFRESH_SECS;
	printf("- %s \t %.1f qps \t (%ld total)\n", name, qps, value);
}
int renderGui() {
	// count clients
	uint32_t known = 0, withVerifiedEp = 0, withGuessedEp = 0;
	for (int i = 0; i < LEN_CLIENTS; i++) { 
		Client clt = clients[i];
		if (!clt.known) continue;
		known++;
		if (clt.hasEndpoint) {
			if (clt.endpointVerified) withVerifiedEp++;
			else withGuessedEp++;
		}
	}

	std::cout << "\033[2J\033[1;1H"; // clear screen 
	std::cout << "regionserver2" << "\t" << our_ip << ":" << our_port << std::endl;
	
	std::cout << std::endl;
	std::cout << "clients: \tknown=" << known	<< " \tverifiedEp=" << withVerifiedEp << " \tguessedEp=" << withGuessedEp << " \tnoEp=" << (known - withVerifiedEp - withGuessedEp) << std::endl;
	
	std::cout << std::endl;
	drawMetric("elapsedSecs", &metrics[METRIC_SECS]);
	drawMetric("tcpClients", &metrics[METRIC_TCP_CLIENTS]);
	drawMetric("addClient", &metrics[METRIC_ADD_CLIENT]);

	std::cout << std::endl;
	drawMetric("tcpRecv", &metrics[METRIC_TCP_RECV]);
	drawMetric("tcpSend", &metrics[METRIC_TCP_SEND]);

	std::cout << std::endl;
	drawMetric("udpRecv", &metrics[METRIC_UDP_RECV]);
	uint64_t val = (uint64_t) udp_out;
	printf("- udpSend \t %.1f qps \n", (val - udp_out_last) * 1.0 / STATS_REFRESH_SECS);
	udp_out_last = val;

	std::cout << std::endl;
	drawMetric("queueIn", &metrics[METRIC_QUEUE_IN]);
	drawMetric("queueOut", &metrics[METRIC_QUEUE_OUT]);
	printf("- queueSize \t %d\n", (int) queue_size);

	std::cout << std::endl;
	drawMetric("udpError", &metrics[METRIC_UDP_ERROR]);
	drawMetric("missClient", &metrics[METRIC_MISSING_CLIENT]);
	drawMetric("missEp", &metrics[METRIC_MISSING_ENDPOINT]);
	drawMetric("dedupe", &metrics[METRIC_DEDUPE]);

	std::cout << std::endl;
	std::cout << "udp in:" << std::endl;
	char idStr[5];
	for (int i = 0; i < 256; i++) {
		if (metrics_udp_in[i].value > 0) { 
			std::cout << "<<";
			sprintf(idStr, "%02X", (uint8_t) i);
			drawMetric(idStr, &metrics_udp_in[i]);
		}
	}

	std::cout << std::endl;
	std::cout << "udp out:" << std::endl;
	for (int i = 0; i < 256; i++) {
		if (metrics_udp_out[i].value > 0) { 
			std::cout << ">>";
			sprintf(idStr, "%02X", (uint8_t) i);
			drawMetric(idStr, &metrics_udp_out[i]);
		}
	}

}

// -------------------------------------------------------------------
// Main (setup)
// -------------------------------------------------------------------

int main(int argc, char* argv[]) {
	if (argc > 1) {
		our_ip = argv[1];
	}
	if (argc > 2) {
		our_port = (uint16_t) std::stoi(argv[2], NULL, 10);
	}
	char grpc_addr[30];
	sprintf(grpc_addr, "0.0.0.0:%hu", our_port + 10000);
	uint16_t udp_port = our_port;
	uint16_t tcp_port = our_port;

	// Run gRPC
	RegionServiceImpl service;
	ServerBuilder builder;
	builder.AddListeningPort(grpc_addr, grpc::InsecureServerCredentials());
	builder.SetMaxMessageSize(GRPC_MAX_MESSAGE_SIZE);
	builder.RegisterService(&service);
	std::unique_ptr<Server> server(builder.BuildAndStart());
	std::cout << "gRPC server listening on " << grpc_addr << std::endl;

	// Run TCP
	struct sockaddr_in tcpBindAddr;
	memset((char *) &tcpBindAddr, 0, sizeof(tcpBindAddr));
	tcpBindAddr.sin_family = AF_INET;
	tcpBindAddr.sin_port = htons(tcp_port);
	tcpBindAddr.sin_addr.s_addr = htonl(INADDR_ANY);

	if ((tcpSocket=socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)) == -1) die("failed to create tcp socket");
	setsockopt(tcpSocket, SOL_SOCKET, SO_RCVBUF, &sockSize, sizeof(sockSize));
	setsockopt(tcpSocket, SOL_SOCKET, SO_SNDBUF, &sockSize, sizeof(sockSize));
	if (bind(tcpSocket , (struct sockaddr*)&tcpBindAddr, sizeof(tcpBindAddr)) == -1) die("tcp bind failed");
	listen(tcpSocket, 1);
	std::cout << "TCP server listening on " << tcp_port << std::endl;

	// Run UDP
	struct sockaddr_in udpBindAddr;
	memset((char *) &udpBindAddr, 0, sizeof(udpBindAddr));
	udpBindAddr.sin_family = AF_INET;
	udpBindAddr.sin_port = htons(udp_port);
	udpBindAddr.sin_addr.s_addr = htonl(INADDR_ANY);

	if ((udpSocket=socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) == -1) die("failed to create udp socket");
	setsockopt(udpSocket, SOL_SOCKET, SO_RCVBUF, &sockSize, sizeof(sockSize));
	setsockopt(udpSocket, SOL_SOCKET, SO_SNDBUF, &sockSize, sizeof(sockSize));
	
	if (bind(udpSocket , (struct sockaddr*)&udpBindAddr, sizeof(udpBindAddr)) == -1) die("udp bind failed");
	std::cout << "UDP server listening on " << udp_port << std::endl;

	// Spawn workers
	std::cout << "Spawning worker threads" << std::endl;
	std::thread threads[2+UDP_WORKERS];
	threads[0] = std::thread(tcpAcceptWorker);
	threads[1] = std::thread(udpReceiveWorker);
	for (int i = 0; i < UDP_WORKERS; i++) threads[i+2] = std::thread(udpSendWorker, i);

	// Wait for server shutdown
	std::cout << "*** All running ***" << std::endl;
	while (1) {
		metrics[METRIC_SECS].value += STATS_REFRESH_SECS;
		sleep(STATS_REFRESH_SECS);
		renderGui();
	}
	for (int i = 0; i < UDP_WORKERS + 2; i++) threads[i].join();
	for (int i = 0; i < tcp_thread_count; i++) tcp_threads[i].join();

	// Kill everything
	std::cout << "Shutting down sockets..." << std::endl;
	close(tcpSocket);
	close(udpSocket);
	std::cout << "exit()" << std::endl;
	return 0;
}
