// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Internal
{
	public class CompileScriptsAssembly : AssemblyRegistration
	{
		public override void OnRegister()
		{
			new ScriptCompilerImpl();
		}
	}
}
