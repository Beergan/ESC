@echo off
SET publishDir=D:\DEV_CUONG\PUBLISH
SET projectPath=ESC.CONCOST.WebHost\ESC.CONCOST.WebHost.csproj

echo ================================
echo Cleaning old publish folder...
echo ================================

IF EXIST "%publishDir%" (
    rmdir /S /Q "%publishDir%"
)

mkdir "%publishDir%"

echo ================================
echo Publishing project...
echo ================================

dotnet publish "%projectPath%" -c Release -r win-x64 --self-contained false -o "%publishDir%"

echo ================================
echo Publish completed!
echo Output: %publishDir%
echo ================================

pause