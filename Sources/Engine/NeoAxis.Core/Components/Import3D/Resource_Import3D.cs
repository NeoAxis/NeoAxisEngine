// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	public class Resource_Import3D : Resource
	{
		/// <summary>
		/// Represents an instance for <see cref="Resource_Import3D"/>.
		/// </summary>
		public class InstanceImport : Instance
		{
			public InstanceImport( Resource owner, InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
				: base( owner, instanceType, componentCreateHierarchyController, componentSetEnabled )
			{
			}

			protected override void Load()
			{
				if( Owner.LoadedBlock == null )
				{
					var settingsFileName = Owner.Name + ".settings";
					if( VirtualFile.Exists( settingsFileName ) )
					{
						var block = TextBlockUtility.LoadFromVirtualFile( settingsFileName );
						Owner.LoadedBlock = block;
					}
					else
					{
						var block = new TextBlock();
						var child = block.AddChild( ".component", MetadataManager.GetTypeOfNetType( typeof( Component_Import3D ) ).Name );

						Owner.LoadedBlock = block;
					}
				}

				base.Load();
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
