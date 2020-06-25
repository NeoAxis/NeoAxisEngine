// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// The class to handle dynamic link library (DLL) as a resource.
	/// </summary>
	public class Resource_Assembly : Resource
	{
		//!!!!threading
		//!!!!или тут Assembly assembly;

		////////////////

		/// <summary>
		/// Represents an instance of <see cref="Resource_Assembly"/>.
		/// </summary>
		public class InstanceAssembly : Instance
		{
			Assembly assembly;
			EDictionary<string, Metadata.TypeInfo> typeByPathInside;

			////////////////

			public InstanceAssembly( Resource owner, InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
				: base( owner, instanceType, componentCreateHierarchyController, componentSetEnabled )
			{
			}

			public Assembly Assembly
			{
				get { return assembly; }
			}

			protected override void OnDispose()
			{
				//!!!!!возможность перезагружать

				base.OnDispose();
			}

			protected override void Load()
			{
				var realFileName = VirtualPathUtility.GetRealPathByVirtual( Owner.Name );
				if( File.Exists( realFileName ) )
				{
					//!!!!!экскепшены?
					//!!!!virtual name
					//!!!!reloading

					assembly = AssemblyUtility.LoadAssemblyByRealFileName( realFileName, false, true, Owner.Name );
					//if( assembly == null )
					//{
					//	//!!!!
					//	Log.Fatal( "impl" );
					//}

					ResultObject = assembly;
				}
			}

			public override IEnumerable<Metadata.TypeInfo> MetadataGetTypes()
			{
				foreach( var t in base.MetadataGetTypes() )
					yield return t;

				if( assembly != null )
				{
					foreach( var type in assembly.GetExportedTypes() )
						yield return MetadataManager.GetTypeOfNetType( type );//, false );
				}
			}

			public override Metadata.TypeInfo MetadataGetType( string pathInside )
			{
				if( assembly != null )
				{
					if( typeByPathInside == null )
					{
						typeByPathInside = new EDictionary<string, Metadata.TypeInfo>();

						foreach( var netType in assembly.GetExportedTypes() )
						{
							var type = MetadataManager.GetTypeOfNetType( netType );//, false );

							//!!!!так?
							string pathInside2;
							{
								int index = type.Name.IndexOf( '|' );
								if( index != -1 )
									pathInside2 = type.Name.Substring( index + 1 );
								else
									pathInside2 = "";
							}

							typeByPathInside[ pathInside2 ] = type;
						}
					}

					{
						Metadata.TypeInfo type;
						typeByPathInside.TryGetValue( pathInside, out type );
						return type;
					}
				}

				return base.MetadataGetType( pathInside );
			}
		}

		////////////////

		public Resource_Assembly()
		{
		}

		protected override void OnDispose()
		{
			//!!!!!возможность перезагружать

			base.OnDispose();
		}

		public override Instance CreateInstanceClassObject( InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
		{
			return new InstanceAssembly( this, instanceType, componentCreateHierarchyController, componentSetEnabled );
		}
	}
}
