// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The sensor is intended to destroy detected objects. Can be used to remove objects that have gone beyond the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\Scene objects\Sensors\Destroying Sensor", 20 )]
	public class DestroyingSensor : Sensor
	{
		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//hide properties
			if( member is Metadata.Property )
			{
				var names = new string[] { "IgnoreSensors", "WhenUpdate", "DisplayObjects", "DisplayObjectsColor" };

				foreach( var name in names )
				{
					var signature = "property:" + name;
					if( member.Signature == signature )
					{
						skip = true;
						break;
					}
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			//update sensor
			WhenUpdate = WhenUpdateEnum.SimulationStep;

			base.OnEnabledInHierarchyChanged();
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//!!!!the ability to filter objects

			//get objects to destroy
			List<ObjectInSpace> toDelete = null;

			foreach( var obj in Objects )
			{
				var obj2 = obj.Value;
				//find top object in space
				while( obj2.Parent as ObjectInSpace != null )
					obj2 = (ObjectInSpace)obj2.Parent;

				//add to the list for deletion
				if( toDelete == null )
					toDelete = new List<ObjectInSpace>();
				toDelete.Add( obj2 );
			}

			//destroy
			if( toDelete != null )
			{
				foreach( var obj in toDelete )
					obj.RemoveFromParent( true );
			}
		}
	}
}
