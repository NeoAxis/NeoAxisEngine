// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class VehicleTypePreviewImage : PreviewImageGenerator
	{
		public VehicleTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var Vehicle = ObjectOfPreview as VehicleType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Vehicle>( enabled: false );
				objectInSpace.VehicleType = Vehicle;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 1.5 );
		}
	}
}
#endif