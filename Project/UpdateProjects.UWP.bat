@echo off
set ExeFile=Binaries\NeoAxis.Editor.exe

if exist "Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.csproj" %ExeFile% -platformProjectPatch "Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.UWP.csproj" -baseProject "Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.csproj"

if exist "Project.csproj" %ExeFile% -platformProjectPatch "Project.UWP.csproj" -baseProject "Project.csproj"

pause
