@ECHO OFF
    powershell -NoProfile -Command "gci -Recurse -Directory -Include bin,obj -Force | rm -Recurse -Force; Write-Host 'Deleted bin and obj folders successfully' -ForegroundColor Green"
    SET "name=%~1"
    IF "%name%"=="build" dotnet build
PAUSE