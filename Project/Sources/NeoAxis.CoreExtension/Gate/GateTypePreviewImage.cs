// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class GateTypePreviewImage : PreviewImageGenerator
	{
		public GateTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var type = ObjectOfPreview as GateType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Gate>( enabled: false );
				objectInSpace.GateType = type;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 2.5 );
		}
	}
}
#endif