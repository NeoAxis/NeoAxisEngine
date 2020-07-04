@echo off
set PDPPath=..\..\..\Project\Binaries\NeoAxis.Internal\Tools\PlatformTools\ProjectDiffPatcher\bin\Release
%PDPPath%\ProjectDiffPatcher.exe NeoAxis.Core.Android.csproj NeoAxis.Core.csproj ProjectDiffPatcher.Android.json
pause
