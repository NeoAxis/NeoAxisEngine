// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	/// The class to handle sound file as a resource.
	/// </summary>
	public class Resource_Sound : Resource
	{
		/// <summary>
		/// Represents an instance of <see cref="Resource_Sound"/>.
		/// </summary>
		public class InstanceSound : Instance
		{
			public InstanceSound( Resource owner, InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
				: base( owner, instanceType, componentCreateHierarchyController, componentSetEnabled )
			{
			}

			protected override void Load()
			{
				if( Owner.LoadedBlock == null )
				{
					if( Path.GetExtension( Owner.Name ).ToLower() != ".sound" )
					{
						var block = new TextBlock();
						var b = block.AddChild( ".component", "NeoAxis.Component_Sound" );
						var propertyBlock = b.AddChild( "LoadFile" );
						propertyBlock.SetAttribute( "GetByReference", Owner.Name );
						//b.SetAttribute( "LoadFile", Owner.Name );

						Owner.LoadedBlock = block;
					}
				}

				base.Load();
			}
		}

		public override Instance CreateInstanceClassObject( InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
		{
			return new InstanceSound( this, instanceType, componentCreateHierarchyController, componentSetEnabled );
		}
	}
}
