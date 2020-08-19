@echo off
set PDPPath=..\Project\Binaries\NeoAxis.Internal\Tools\PlatformTools\ProjectDiffPatcher\bin\Debug\ProjectDiffPatcher.exe

%PDPPath% "Engine\NeoAxis.Core\NeoAxis.Core.UWP.csproj" "Engine\NeoAxis.Core\NeoAxis.Core.csproj" UWP
%PDPPath% "..\Project\Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.UWP.csproj" "..\Project\Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.csproj" UWP
%PDPPath% "..\Project\Project.UWP.csproj" "..\Project\Project.csproj" UWP

pause
