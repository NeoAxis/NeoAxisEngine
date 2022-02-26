// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace RoslynPad.Roslyn
{
	public abstract class NeoAxisCoreExchange
	{
		public static NeoAxisCoreExchange Instance;

		public abstract bool GetProjectData( out List<string> csFiles, out List<string> references );

		public abstract List<string> CSharpScriptReferenceAssemblies { get; }
		public abstract List<string> CSharpScriptUsingNamespaces { get; }

		public abstract string ResolveAssemblyName( string name );
	}
}