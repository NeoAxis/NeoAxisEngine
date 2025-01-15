// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	public class WebBrowserAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			EngineApp.AppDestroy += EngineApp_AppDestroy;
		}

		private void EngineApp_AppDestroy()
		{
			UIWebBrowser.ShutdownCefRuntime();
		}
	}
}
