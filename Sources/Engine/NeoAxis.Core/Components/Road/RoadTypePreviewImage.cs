// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class RoadTypePreviewImage : PreviewImageGenerator
	{
		public RoadTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var Road = ObjectOfPreview as RoadType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Road>( enabled: false );
				objectInSpace.RoadType = Road;

				{
					var point = objectInSpace.CreateComponent<RoadPoint>();
					point.Transform = new Transform( new Vector3( 50, 0, 0 ) );
				}

				objectInSpace.Enabled = true;

				objectInSpace.GetVisualData();
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 1.5 );
		}
	}
}
#endif