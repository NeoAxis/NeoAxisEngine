// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	public static class ClientUtility
	{
		public static Scene GetScene()
		{
			return PlayScreen.Instance?.Scene;
		}

		public static GameMode GetGameMode()
		{
			var scene = GetScene();
			if( scene != null )
				return (GameMode)scene.GetGameMode();
			return null;
		}

		public static NetworkLogic GetNetworkLogic()
		{
			var scene = GetScene();
			if( scene != null )
				return NetworkLogicUtility.GetNetworkLogic( scene ) as NetworkLogic;
			return null;
		}
	}
}
