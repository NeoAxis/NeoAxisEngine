// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Scripting context variables container.
	/// </summary>
	public class CSharpScriptContext
	{
		public Component_CSharpScript Owner { get; set; }
	}

	/// <summary>
	/// Internal attribute of the engine for compiling C# scripts.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class )]
	public class CSharpScriptGeneratedAttribute : Attribute
	{
		internal string Key { get; set; }

		public CSharpScriptGeneratedAttribute( string key )
		{
			Key = key;
		}
	}
}
