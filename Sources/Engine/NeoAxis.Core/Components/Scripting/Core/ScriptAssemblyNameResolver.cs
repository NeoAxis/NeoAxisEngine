// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;

namespace NeoAxis
{
	//TODO: add GAC search and assembly strong name support. see Mono.Cecil.BaseAssemblyResolver
	//TODO: move to Utils folder ?
	//TODO: use AssemblyName as return value ?
	class ScriptAssemblyNameResolver
	{
		static readonly bool onMono = Type.GetType( "Mono.Runtime" ) != null;
		readonly List<string> directories;

		internal ScriptAssemblyNameResolver()
		{
			directories = new List<string>() /*{ ".", "bin" }*/;
		}

		public void AddSearchDirectory( string directory )
		{
			directories.Add( directory );
		}

		public void RemoveSearchDirectory( string directory )
		{
			directories.Remove( directory );
		}

		public string[] GetSearchDirectories()
		{
			return directories.ToArray();
		}

		public string Resolve( string name )
		{
			var assembly = SearchDirectory( name, directories );
			if( assembly != null )
				return assembly;

			var frameworkDir = Path.GetDirectoryName( typeof( object ).Module.FullyQualifiedName );
			var frameworkDirs = onMono
				? new[] { frameworkDir, Path.Combine( frameworkDir, "Facades" ) }
				: new[] { frameworkDir };

			assembly = SearchDirectory( name, frameworkDirs );
			if( assembly != null )
				return assembly;

			throw new FileNotFoundException( string.Format( "Failed to resolve assembly: '{0}'", name ) );
		}

		string SearchDirectory( string name, IEnumerable<string> directories )
		{
			var extensions =  new[] { ".exe", ".dll" };
			foreach( var directory in directories )
			{
				foreach( var extension in extensions )
				{
					string file = Path.Combine( directory, name + extension );
					if( File.Exists( file ) )
						return file;
				}
			}

			return null;
		}
	}
}
