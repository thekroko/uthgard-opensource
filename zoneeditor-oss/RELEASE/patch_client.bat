call settings.cmd
call createcsv.bat
%PORTAL%\ModManager.exe --auto-install "%CLIENT%" testzone.dmm %CD%\testzone.dmm nocache
echo Client patched.