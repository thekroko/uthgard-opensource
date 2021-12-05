call settings.cmd
copy src\zones\zone%ZONE_ID%\dat%ZONE_ID%.mpk\*.csv src\zones\zone%ZONE_ID%\csv%ZONE_ID%.mpk\*.csv
copy src\zones\zone%ZONE_ID%\dat%ZONE_ID%.mpk\SECTOR.DAT src\zones\zone%ZONE_ID%\csv%ZONE_ID%.mpk\SECTOR.DAT
echo .csv Files copied.