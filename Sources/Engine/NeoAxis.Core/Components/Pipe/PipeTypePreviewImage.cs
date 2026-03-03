// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class PipeTypePreviewImage : PreviewImageGenerator
	{
		public PipeTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var pipeType = ObjectOfPreview as PipeType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Pipe>( enabled: false );
				objectInSpace.PipeType = pipeType;

				var panelLength = pipeType.OutsideDiameter.Value * 20;

				{
					var point = objectInSpace.CreateComponent<PipePoint>();
					point.Transform = new Transform( new Vector3( panelLength, 0, 0 ) );
				}

				{
					var point = objectInSpace.CreateComponent<PipePoint>();
					point.Transform = new Transform( new Vector3( panelLength, panelLength, 0 ) );
				}

				{
					var point = objectInSpace.CreateComponent<PipePoint>();
					point.Transform = new Transform( new Vector3( panelLength * 2, panelLength, 0 ) );
					point.Specialty = PipePoint.SpecialtyEnum.Socket;
				}

				{
					var point = objectInSpace.CreateComponent<PipePoint>();
					point.Transform = new Transform( new Vector3( panelLength * 3, panelLength, 0 ) );
				}

				objectInSpace.Enabled = true;

				objectInSpace.GetVisualData();
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 1.5 );
		}
	}
}
#endif