# Build Navmeshes

Tool for generating Recast navmeshes from DAoC zones.

## Usage instructions

1. Build project using msbuild
1. Run `buildnav.exe --daoc=<gamedir> --obj=false --all=true` to build navmeshes (this will take a long time ..)
1. Copy `*.nav` into the PathingServers `/opt/navmeshes/` directory
