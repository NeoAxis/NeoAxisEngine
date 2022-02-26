// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// The class to handle image file as a resource.
	/// </summary>
	class ImageResource : Resource
	{
		/// <summary>
		/// Represents an instance of <see cref="ImageResource"/>.
		/// </summary>
		public class InstanceImage : Instance
		{
			public InstanceImage( Resource owner, InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
				: base( owner, instanceType, componentCreateHierarchyController, componentSetEnabled )
			{
			}

			protected override void Load( ref TextBlock loadedBlock )
			{
				if( loadedBlock/*Owner.LoadedBlock*/ == null )
				{
					if( Path.GetExtension( Owner.Name ).ToLower() != ".image" )
					{
						var block = new TextBlock();
						var b = block.AddChild( ".component", MetadataManager.GetTypeOfNetType( typeof( ImageComponent ) ).Name );
						var propertyBlock = b.AddChild( "LoadFile" );
						propertyBlock.SetAttribute( "GetByReference", Owner.Name );
						//b.SetAttribute( "LoadFile", Owner.Name );

						loadedBlock/*Owner.LoadedBlock*/ = block;
					}
				}

				base.Load( ref loadedBlock );
			}
		}

		public override Instance CreateInstanceClassObject( InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
		{
			return new InstanceImage( this, instanceType, componentCreateHierarchyController, componentSetEnabled );
		}
	}
}
