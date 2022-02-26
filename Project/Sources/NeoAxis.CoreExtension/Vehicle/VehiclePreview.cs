// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class VehiclePreview : ObjectInSpacePreview
	{
		public VehiclePreview()
		{
		}

		protected override void OnCreate()
		{
			//base.OnCreate();

			if( EnableViewportControl )
			{
				var objectInSpace = ObjectOfPreview as ObjectInSpace;
				if( objectInSpace != null && objectInSpace.ParentScene == null )//show only if no scene
				{
					var scene = CreateScene( false );

					var type = objectInSpace.GetProvidedType();
					if( type != null )
					{
						var obj = scene.CreateComponent( type ) as Vehicle;
						if( obj != null )
							obj.SetTransform( Transform.Identity );

						//var obj = (ObjectInSpace)scene.CreateComponent( type );
						//obj.Transform = Transform.Identity;
					}

					scene.Enabled = true;
					SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace() );
				}
			}
			else
			{
				ViewportControl.AllowCreateRenderWindow = false;
				ViewportControl.Visible = false;
			}
		}
	}
}
#endif