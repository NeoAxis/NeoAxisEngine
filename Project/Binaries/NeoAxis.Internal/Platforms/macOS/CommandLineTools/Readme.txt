How run a build server on macOS:
	./CommandLineTools -platformServer {password}

Allow when blocking by the firewall:
	sudo /usr/libexec/ApplicationFirewall/socketfilterfw --add {path to app}/CommandLineTools
	sudo /usr/libexec/ApplicationFirewall/socketfilterfw --unblockapp {path to app}/CommandLineTools

Allow when in quarantine:
	sudo xattr -rd com.apple.quarantine {path to app}/CommandLineTools
