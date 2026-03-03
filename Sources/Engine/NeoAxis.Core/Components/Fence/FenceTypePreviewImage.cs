// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class FenceTypePreviewImage : PreviewImageGenerator
	{
		public FenceTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var Fence = ObjectOfPreview as FenceType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Fence>( enabled: false );
				objectInSpace.FenceType = Fence;

				var panelLength = Fence.PanelLength.Value;

				{
					var point = objectInSpace.CreateComponent<FencePoint>();
					point.Transform = new Transform( new Vector3( panelLength * 2, 0, 0 ) );
				}

				{
					var point = objectInSpace.CreateComponent<FencePoint>();
					point.Transform = new Transform( new Vector3( panelLength * 2, panelLength * 2, 0 ) );
				}

				{
					var point = objectInSpace.CreateComponent<FencePoint>();
					point.Transform = new Transform( new Vector3( panelLength * 4, panelLength * 2, 0 ) );
				}

				{
					var point = objectInSpace.CreateComponent<FencePoint>();
					point.Transform = new Transform( new Vector3( panelLength * 6, panelLength * 2, 0 ) );
				}

				objectInSpace.Enabled = true;

				objectInSpace.GetVisualData();
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 1.5 );
		}
	}
}
#endif