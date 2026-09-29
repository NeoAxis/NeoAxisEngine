:: Synchronization files with macOS system to test builds
:: How to install the server: Project\Binaries\NeoAxis.Internal\Platforms\macOS\CommandLineTools\Readme.txt

Project\Binaries\NeoAxis.Internal\Platforms\Windows\CommandLineTools\CommandLineTools.exe -synchronizeFiles "..\..\..\..\..\..\Project" -targetRemoteDirectory macOS -server 192.168.1.142 -password 12345
