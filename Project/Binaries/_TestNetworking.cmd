start NeoAxis.Player.exe -windowed 1 -windowState Normal -windowPosition "10 10" -windowSize "954 754" -server 1 -networkMode Direct -serverPort 52000 -play "Samples\Shooter\Scenes\Free for all.scene" -rendererBackend Noop -soundSystem null
rem start NeoAxis.Player.exe -windowed 1 -windowState Normal -windowPosition "10 10" -windowSize "954 754" -server 1 -networkMode Direct -serverPort 52000 -play "Samples\Shooter\Scenes\Free for all.scene"

timeout 10

start NeoAxis.Player.exe -windowed 1 -windowState Maximized -client 1 -networkMode Direct -serverPort 52000 -serverAddress "localhost"
rem start NeoAxis.Player.exe -windowed 1 -windowState Normal -windowPosition "960 10" -windowSize "954 754" -client 1 -networkMode Direct -serverPort 52000 -serverAddress "localhost"
