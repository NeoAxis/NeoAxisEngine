// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NeoAxis.Editor;

namespace NeoAxis
{
	public class PipeAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Pipe Type", new string[] { "pipetype" }, typeof( Resource ) );
		}
	}
}
