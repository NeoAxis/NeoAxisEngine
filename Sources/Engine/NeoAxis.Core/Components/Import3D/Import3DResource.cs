// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents a resource of source 3D data for import.
	/// </summary>
	class Import3DResource : Resource
	{
		/// <summary>
		/// Represents an instance for <see cref="Import3DResource"/>.
		/// </summary>
		public class InstanceImport : Instance
		{
			public InstanceImport( Resource owner, InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
				: base( owner, instanceType, componentCreateHierarchyController, componentSetEnabled )
			{
			}

			protected override void Load( ref TextBlock loadedBlock )
			{
				if( loadedBlock/*Owner.LoadedBlock*/ == null )
				{
					var settingsFileName = Owner.Name + ".settings";
					if( VirtualFile.Exists( settingsFileName ) )
					{
						var block = TextBlockUtility.LoadFromVirtualFile( settingsFileName );
						loadedBlock/*Owner.LoadedBlock*/ = block;
					}
					else
					{
						var block = new TextBlock();
						var child = block.AddChild( ".component", MetadataManager.GetTypeOfNetType( typeof( Import3D ) ).Name );

						loadedBlock/*Owner.LoadedBlock*/ = block;
					}
				}

				base.Load( ref loadedBlock );
			}
		}

		public override Instance CreateInstanceClassObject( InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
		{
			return new InstanceImport( this, instanceType, componentCreateHierarchyController, componentSetEnabled );
		}

		public override string GetSaveAddFileExtension()
		{
			return ".settings";
		}
	}
}
