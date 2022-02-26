// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using RoslynPad.Roslyn;

namespace NeoAxis
{
	class RoslynPadNeoAxisCoreExchangeImpl : NeoAxisCoreExchange
	{
		public static void Init()
		{
			if( Instance == null )
				Instance = new RoslynPadNeoAxisCoreExchangeImpl();
		}

		/////////////////////////////////////////

		public override bool GetProjectData( out List<string> csFiles, out List<string> references )
		{
			csFiles = new List<string>( CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ) );
			references = CSharpProjectFileUtility.GetProjectFileReferences( false );
			return true;
		}

		public override List<string> CSharpScriptReferenceAssemblies
		{
			get { return ScriptingCSharpEngine.CSharpScriptReferenceAssemblies; }
		}

		public override List<string> CSharpScriptUsingNamespaces
		{
			get { return ScriptingCSharpEngine.CSharpScriptUsingNamespaces; }
		}

		public override string ResolveAssemblyName( string name )
		{
			return ScriptingCSharpEngine.scriptAssemblyNameResolver.Resolve( name );
		}
	}
}
