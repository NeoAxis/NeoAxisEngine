// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
