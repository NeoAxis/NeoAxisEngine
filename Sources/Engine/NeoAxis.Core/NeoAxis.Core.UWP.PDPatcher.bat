@echo off
set PDPPath=..\Tools\ProjectDiffPatcher\ProjectDiffPatcher\bin\Debug
%PDPPath%\pdpatcher NeoAxis.Core.UWP.csproj NeoAxis.Core.csproj NeoAxis.Core.UWP.PDPatcher.json
pause