Use publish to make builds. Output folders:
	Project\Binaries\NeoAxis.Internal\Platforms\Windows\CommandLineTools
	Project\Binaries\NeoAxis.Internal\Platforms\macOS\CommandLineTools

Debug command line parameters:
	-compile "..\..\..\..\..\..\..\..\Sources\Engine\NeoAxis.Core.Native\NeoAxisCoreNative.Web.compile"
	-compileRemote "..\..\..\..\..\..\..\..\Sources\Engine\NeoAxis.Core.Native\NeoAxisCoreNative.macOS.compile" -server 192.168.1.235 -password 12345
	-compileRemote "..\..\..\..\..\..\..\..\Sources\Engine\NeoAxis.Core.Native\NeoAxisCoreNative.iOS.compile" -server 192.168.1.235 -password 12345
	-synchronizeFiles "..\..\..\..\..\..\..\..\Project" -targetRemoteDirectory macOS -server 192.168.1.235 -password 12345
