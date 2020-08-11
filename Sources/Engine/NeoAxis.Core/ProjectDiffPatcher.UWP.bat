@echo off
set PDPPath=..\..\..\Project\Binaries\NeoAxis.Internal\Tools\PlatformTools\ProjectDiffPatcher\bin\Debug\netcoreapp3.1
%PDPPath%\ProjectDiffPatcher.exe NeoAxis.Core.UWP.csproj NeoAxis.Core.csproj ProjectDiffPatcher.UWP.json
pause
