// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class VehicleTypePreview : CanvasBasedPreview
	{
		Vehicle objectInSpace;

		//

		public VehicleType VehicleType
		{
			get { return ObjectOfPreview as VehicleType; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Vehicle>( enabled: false );
			objectInSpace.VehicleType = VehicleType;
			objectInSpace.Enabled = true;
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( VehicleType != null )
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), 0.5 );
			}
		}
	}
}
#endif