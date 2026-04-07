// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Internal
{
	// see similar RoslynPad ScriptRunner class

	public abstract class ScriptCompiler
	{
		public static ScriptCompiler Instance { get; set; }

		public abstract void SettingsAddReferences( List<string> references );
		public abstract Assembly CompileCode( string scriptText, string writeToDllOptional );

		//

		public static List<string> CSharpScriptReferenceAssemblies { get; } = new List<string>();
		public static List<string> CSharpScriptUsingNamespaces { get; } = new List<string>();
		public static ScriptAssemblyNameResolver ScriptAssemblyNameResolver;

		public abstract string ScriptCodeGenerator_GenerateWrappedScript( IEnumerable<string> methods, IEnumerable<string> usingNamespaces, string inheritFrom );
		public abstract void ScriptCodeGenerator_CheckForSyntaxErrors( string code );

		public abstract object/*Document*/ ScriptCodeGenerator_AddMethodToClass( object/*Document*/ document, object/*MethodDeclarationSyntax*/ method );
		public abstract object/*MethodDeclarationSyntax*/ ScriptCodeGenerator_GenerateMethodFromReflection( string methodName, ParameterInfo[] parameters );
	}
}
