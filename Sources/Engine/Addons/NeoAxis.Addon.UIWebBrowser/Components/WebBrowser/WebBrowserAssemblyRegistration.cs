// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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
