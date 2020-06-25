// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// The attribute for tagging access to members of properties for use in visual adjustment of materials and effects.
	/// </summary>
	[AttributeUsage( AttributeTargets.Property )]
	public class ShaderGenerationPropertyPostfixAttribute : Attribute
	{
		string postfix;

		public ShaderGenerationPropertyPostfixAttribute( string postfix )
		{
			this.postfix = postfix;
		}

		public string Postfix
		{
			get { return postfix; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The attribute for tagging functions for use in visual adjustment of materials and effects.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Field )]
	public class ShaderGenerationFunctionAttribute : Attribute
	{
		string template;

		public ShaderGenerationFunctionAttribute( string template )
		{
			this.template = template;
		}

		public string Template
		{
			get { return template; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The attribute for tagging members for use in visual adjustment of materials and effects.
	/// </summary>
	[AttributeUsage( AttributeTargets.Property )]
	public class ShaderGenerationAutoConstantAttributeAttribute : Attribute
	{
		Type type;
		string name;

		public ShaderGenerationAutoConstantAttributeAttribute( Type type, string name )
		{
			this.type = type;
			this.name = name;
		}

		public Type Type
		{
			get { return type; }
		}

		public string Name
		{
			get { return name; }
		}
	}
}
