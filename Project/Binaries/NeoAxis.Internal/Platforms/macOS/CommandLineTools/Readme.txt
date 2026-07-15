How run a build server on macOS:
	./CommandLineTools -platformServer {password}
How enable file synchronization functionality:
	-synchronizeFilesEnable True

Example: ./CommandLineTools -platformServer 12345 -synchronizeFilesEnable True

Where the folder:
	"NeoAxisPlatformServer" folder in the Documents folder of the user.

----- Additional -----
	
Allow when the app is blocking by the firewall:
	sudo /usr/libexec/ApplicationFirewall/socketfilterfw --add {path to app}/CommandLineTools
	sudo /usr/libexec/ApplicationFirewall/socketfilterfw --unblockapp {path to app}/CommandLineTools

Allow when the app is in quarantine:
	sudo xattr -rd com.apple.quarantine {path to app}/CommandLineTools
