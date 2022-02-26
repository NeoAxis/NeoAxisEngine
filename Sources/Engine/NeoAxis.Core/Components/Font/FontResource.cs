// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	class FontResource : Resource
	{
		/// <summary>
		/// Represents an instance of <see cref="FontResource"/>.
		/// </summary>
		public class InstanceFont : Instance
		{
			public InstanceFont( Resource owner, InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
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
						//!!!!check errors
						var block = TextBlockUtility.LoadFromVirtualFile( settingsFileName );

						loadedBlock/*Owner.LoadedBlock*/ = block;
					}
					else
					{
						//!!!!так?

						var block = new TextBlock();
						var child = block.AddChild( ".component", MetadataManager.GetTypeOfNetType( typeof( FontComponent ) ).Name );

						//default configuration
						var b1 = child.AddChild( "CharacterRanges" );
						var b2 = b1.AddChild( "Value" );
						b2.SetAttribute( "Count", "2" );
						b2.SetAttribute( "0", "32 127" );
						b2.SetAttribute( "1", "160 191" );

						loadedBlock/*Owner.LoadedBlock*/ = block;
					}
				}

				base.Load( ref loadedBlock );
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
