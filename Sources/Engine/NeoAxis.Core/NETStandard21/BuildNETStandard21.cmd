del NeoAxis.Core.NETStandard21.dll
del NeoAxis.Core.NETStandard21.deps.json
del NeoAxis.Core.NETStandard21.pdb
del NeoAxis.Core.xml
copy ..\NeoAxis.Core.csproj ..\NeoAxis.Core.NETStandard21.csproj
powershell -Command "(Get-Content '..\NeoAxis.Core.NETStandard21.csproj') -replace 'net8.0', 'netstandard2.1' | Set-Content '..\NeoAxis.Core.NETStandard21.csproj'"
rd /s /q "..\obj"
dotnet build "..\NeoAxis.Core.NETStandard21.csproj" -c Release -p:TargetFramework=netstandard2.1 -p:OutputPath=".\NETStandard21" -p:AppendTargetFrameworkToOutputPath=false --no-incremental
rd /s /q "..\obj"
del ..\NeoAxis.Core.NETStandard21.csproj
del NeoAxis.Core.xml