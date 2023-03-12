// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class FenceTypePreview : CanvasBasedPreview
	{
		Fence objectInSpace;

		//

		public FenceType FenceType
		{
			get { return ObjectOfPreview as FenceType; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Fence>( enabled: false );
			objectInSpace.FenceType = FenceType;

			var panelLength = FenceType.PanelLength.Value;

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
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( FenceType != null )
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