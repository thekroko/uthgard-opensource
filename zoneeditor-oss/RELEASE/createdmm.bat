@echo off
call createcsv.bat
rm testzone.dmm
"tools\CreateDMM.exe" "testzone.dmm" "My first zone" "Mr. Awesome" "This is my awesome new zone!" "pre\noimage.jpg" "src"
sleep 1