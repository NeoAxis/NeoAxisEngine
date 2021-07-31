// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Win32;
using NeoAxis.Editor;

namespace NeoAxis
{
	public class SketchfabAssemblyRegistration : AssemblyUtility.AssemblyRegistration
	{
		public override void OnRegister()
		{
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
			{
				//register the store
				StoreManager.RegisterStore( new StoreManager.StoreItem() { Name = "Sketchfab", Website = "https://sketchfab.com/", Icon16 = Addon.Sketchfab.Properties.Resources.Sketchfab_16, Icon32 = Addon.Sketchfab.Properties.Resources.Sketchfab_32, Implementation = new SketchfabStoreImplementation() } );

				//init login functionality
				SketchfabLogin.Init();
			}
		}
	}
}
