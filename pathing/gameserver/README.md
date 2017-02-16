# GameServer Pathing Integration

Install instructions:

1.  Add NuGet dependencies to gRPC and Protobuf 
1.  Copy in core/ and world/ files into the GameServer project
1.  Manual adjustments needed to replace GameNPC.WalkTo with GameNPC.PathTo() (see snippets in file)
1.  GameServer.Start() needs to call PathingMgr.Init()

