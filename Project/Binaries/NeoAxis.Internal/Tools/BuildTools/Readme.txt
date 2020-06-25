NeoAxis build tools notes

Installation
	Build Tools from Visual Studio 2017  from official site.
	https://visualstudio.microsoft.com/ru/thank-you-downloading-visual-studio/?sku=BuildTools&rel=15

version 17.8 ?
	
BIG PACK (600mb):
	
	Workloads selected in installer: 
		1) .Net Desktop 
		2) UWP build tools.
		3) Individual comp: NuGet targets and build tasks

		remove all optional components.

	To reduce tools size delete:
		BuildTools\VC
		BuildTools\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation

SMALL PACK (70mb):

	Workloads selected in installer: 
		1) .Net Desktop 
		3) Individual comp: NuGet targets and build tasks
		
	Add folders from UWP build tools workloads
		BuildTools\MSBuild\Microsoft\VisualStudio
		BuildTools\MSBuild\Microsoft\WindowsXaml
		
		
		
full msbuild sdk issue:
https://github.com/dotnet/sdk/issues/300




