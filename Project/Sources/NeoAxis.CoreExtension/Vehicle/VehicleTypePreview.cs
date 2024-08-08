// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class VehicleTypePreview : CanvasBasedPreview
	{
		bool needRecreateInstance;
		Vehicle objectInSpace;
		int createdVersionOfType;

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

			createdVersionOfType = VehicleType.Version;
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

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			if( createdVersionOfType != VehicleType.Version )
				needRecreateInstance = true;

			if( needRecreateInstance )
			{
				CreateObject();
				needRecreateInstance = false;
			}
		}

	}
}
#endif