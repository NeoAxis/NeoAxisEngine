@echo off
set ExeFile=..\Project\Binaries\NeoAxis.Editor.exe

if exist "Engine\NeoAxis.Core\NeoAxis.Core.csproj" %ExeFile% -platformProjectPatch "Engine\NeoAxis.Core\NeoAxis.Core.UWP.csproj" -baseProject "Engine\NeoAxis.Core\NeoAxis.Core.csproj"

if exist "..\Project\Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.csproj" %ExeFile% -platformProjectPatch "..\Project\Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.UWP.csproj" -baseProject "..\Project\Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.csproj"

if exist "..\Project\Project.csproj" %ExeFile% -platformProjectPatch "..\Project\Project.UWP.csproj" -baseProject "..\Project\Project.csproj"

if exist "..\Project\Sources\NeoAxis.Extended\NeoAxis.Extended.csproj" %ExeFile% -platformProjectPatch "..\Project\Sources\NeoAxis.Extended\NeoAxis.Extended.UWP.csproj" -baseProject "..\Project\Sources\NeoAxis.Extended\NeoAxis.Extended.csproj"

pause
