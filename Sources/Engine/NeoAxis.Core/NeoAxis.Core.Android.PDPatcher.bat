@echo off
set PDPPath=..\Tools\ProjectDiffPatcher\ProjectDiffPatcher\bin\Debug
%PDPPath%\pdpatcher NeoAxis.Core.Android.csproj NeoAxis.Core.csproj NeoAxis.Core.Android.PDPatcher.json
pause