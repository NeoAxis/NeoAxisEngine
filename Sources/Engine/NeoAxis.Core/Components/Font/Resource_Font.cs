// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using NeoAxis;

namespace NeoAxis
{
	/// <summary>
	/// Represents a resource of a font.
	/// </summary>
	public class Resource_Font : Resource
	{
		/// <summary>
		/// Represents an instance of <see cref="Resource_Font"/>.
		/// </summary>
		public class InstanceFont : Instance
		{
			public InstanceFont( Resource owner, InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
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
						//!!!!check errors
						var block = TextBlockUtility.LoadFromVirtualFile( settingsFileName );

						Owner.LoadedBlock = block;
					}
					else
					{
						//!!!!так?

						var block = new TextBlock();
						var child = block.AddChild( ".component", MetadataManager.GetTypeOfNetType( typeof( Component_Font ) ).Name );

						//default configuration
						var b1 = child.AddChild( "CharacterRanges" );
						var b2 = b1.AddChild( "Value" );
						b2.SetAttribute( "Count", "2" );
						b2.SetAttribute( "0", "32 127" );
						b2.SetAttribute( "1", "160 191" );

						Owner.LoadedBlock = block;
					}
				}

				base.Load();
			}
		}

		public override Instance CreateInstanceClassObject( InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
		{
			return new InstanceFont( this, instanceType, componentCreateHierarchyController, componentSetEnabled );
		}

		public override string GetSaveAddFileExtension()
		{
			return ".settings";
		}
	}
}
