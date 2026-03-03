// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class RoadTypePreview : CanvasBasedPreview
	{
		Road objectInSpace;

		//

		public RoadType Road
		{
			get { return ObjectOfPreview as RoadType; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Road>( enabled: false );
			objectInSpace.RoadType = Road;

			{
				var point = objectInSpace.CreateComponent<RoadPoint>();
				point.Transform = new Transform( new Vector3( 50, 0, 0 ) );
			}

			objectInSpace.Enabled = true;
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( Road != null )
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