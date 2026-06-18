// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using RoslynPad.Roslyn;
using Internal;

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
			get { return ScriptCompiler.CSharpScriptReferenceAssemblies; }
		}

		public override List<string> CSharpScriptUsingNamespaces
		{
			get { return ScriptCompiler.CSharpScriptUsingNamespaces; }
		}

		public override string ResolveAssemblyName( string name )
		{
			return ScriptCompiler.ScriptAssemblyNameResolver.Resolve( name );
		}
	}
}
