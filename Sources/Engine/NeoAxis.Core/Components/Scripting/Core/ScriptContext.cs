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
	/// <remarks>only fields and properties supported</remarks>
	public class CSharpScriptContext
	{
		public Component_CSharpScript Owner { get; set; }
	}

	internal static class ScriptContextHelper
	{
		//!!!!?

		internal static IEnumerable<(string Name, string Type)> GetContextVars( Type contextType )
		{
			if( contextType == null )
				yield break;

			foreach( var member in GetContextMembers( contextType ) )
				yield return (member.Name, Type: member.GetUnderlyingType().FullName);
		}

		internal static IEnumerable<MemberInfo> GetContextMembers( Type contextType )
		{
			if( contextType == null )
				yield break;

			var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
			var fieldsAndProps = contextType.GetFields( bindingFlags ).Cast<MemberInfo>().Concat( contextType.GetProperties( bindingFlags ) );
			foreach( var member in fieldsAndProps )
				yield return member;
		}
	}
}
