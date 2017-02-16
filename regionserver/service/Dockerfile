FROM debian:wheezy

ENV GRPC_RELEASE_TAG v1.1.2
RUN apt-get update && apt-get install -y dh-autoreconf build-essential linux-headers-amd64 git libncurses5-dev libtbb-dev pkg-config gdb
RUN git clone -b ${GRPC_RELEASE_TAG} https://github.com/grpc/grpc /var/local/git/grpc

# install grpc
WORKDIR /var/local/git/grpc
RUN git submodule update --init && sed -i s/-Werror//g Makefile && make && make install && make clean

#install protoc
WORKDIR /var/local/git/grpc/third_party/protobuf 
RUN make && make install && make clean

RUN mkdir -p /usr/src
WORKDIR /usr/src
COPY . .
RUN make all

ENTRYPOINT ["./regionserver"]
