# RegionServer
RegionServers are used for dispatching UDP traffic from the gameserver. This involves fanning out packets and performing encryption, which can be time consuming on the gameserver itself. The purpose of the regionserer is to decrease the fan-out & encryption load on the gameserver, thus lowering the GameServer cpu by ~30% when dealing with more than 3500 clients.

The region server is a dedicated gRPC service using a bidirectional raw, binary TCP relay socket for communicating with the gameservers. Clients talk to the RegionServer for all UDP requests.
