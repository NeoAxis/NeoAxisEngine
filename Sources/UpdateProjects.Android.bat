@echo off
set PDPPath=..\Project\Binaries\NeoAxis.Internal\Tools\PlatformTools\ProjectDiffPatcher\bin\Debug\ProjectDiffPatcher.exe

%PDPPath% "Engine\NeoAxis.Core\NeoAxis.Core.Android.csproj" "Engine\NeoAxis.Core\NeoAxis.Core.csproj" Android
%PDPPath% "..\Project\Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.Android.csproj" "..\Project\Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.csproj" Android
%PDPPath% "..\Project\Project.Android.csproj" "..\Project\Project.csproj" Android

pause
