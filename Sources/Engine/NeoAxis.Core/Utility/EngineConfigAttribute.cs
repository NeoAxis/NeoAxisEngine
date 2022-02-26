// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Attribute to save the property or field in the application config.
	/// </summary>
	/// <remarks>
	/// To register a class with marked members, you must call the EngineConfig.RegisterClassParameters method.
	/// </remarks>
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class EngineConfigAttribute : Attribute
	{
		string groupPath;
		string name;

		public EngineConfigAttribute( string groupPath = "", string name = "" )
		{
			this.groupPath = groupPath;
			if( string.IsNullOrEmpty( this.groupPath ) )
				this.groupPath = "General";

			this.name = name;
		}

		public string GroupPath
		{
			get { return groupPath; }
		}

		public string Name
		{
			get { return name; }
		}

		public string GetName( MemberInfo member )
		{
			var result = Name;
			if( string.IsNullOrEmpty( result ) )
				result = member.Name;
			return result;
		}
	}
}
