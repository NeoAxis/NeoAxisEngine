// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NeoAxis
{
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
