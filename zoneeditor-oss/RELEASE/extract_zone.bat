call settings.cmd
cd ..\Utils\
set TARGET_DIR="..\RELEASE\src\zones\zone%ZONE_ID%"
if not exist "%TARGET_DIR" ( 
  mkdir "%TARGET_DIR%" 
) 
BatchMPAKExtract.exe "%CLIENT%\zones\zone%ZONE_ID%" "%TARGET_DIR%"
cd ..\RELEASE\