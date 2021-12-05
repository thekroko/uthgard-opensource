call settings.cmd
"tools\ImageSplitter.exe" "pre\tex.bmp" "src\zones\zone%ZONE_ID%\lod%ZONE_ID%.mpk\lod#1-#2.dds" true
"tools\ImageSplitter.exe" "pre\tex.bmp" "src\zones\zone%ZONE_ID%\tex%ZONE_ID%.mpk\tex#1-#2.bmp" true
del "src\zones\zone%ZONE_ID%\ter%ZONE_ID%.mpk\*.dds"
echo Textures splitted.
pause