# Pathing for Dawn of Light

This is a distributed pathing implementation based upon Recast. The pathing server runs as a dedicated process (can be run under linux using Docker), and GameServer talks to pathing server via gRPC. Pathing Server can be restarted on the fly (gameserver will fallback to normal walking).

## Folder structure

*  `service/` contains the Pathing Server (docker image, runs on win64 and linux x64)
*  `gameserver/` contains code snippets suggesting alterations that should be made to the game server
*  `buildnav/` contains the Tools necessary for generating the navmeshes

## Usage Instructions

*  See seperate READMEs for each subfolder for a full installation.
