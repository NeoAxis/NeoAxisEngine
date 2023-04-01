// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class PipeTypePreview : CanvasBasedPreview
	{
		Pipe objectInSpace;

		//

		public PipeType PipeType
		{
			get { return ObjectOfPreview as PipeType; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Pipe>( enabled: false );
			objectInSpace.PipeType = PipeType;

			var panelLength = PipeType.OutsideDiameter.Value * 20;

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
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( PipeType != null )
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