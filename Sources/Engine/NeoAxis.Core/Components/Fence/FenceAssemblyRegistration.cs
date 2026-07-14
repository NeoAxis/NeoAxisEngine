// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NeoAxis.Editor;

namespace NeoAxis
{
	public class FenceAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Fence Type", new string[] { "fencetype" }, typeof( Resource ) );
		}
	}
}
