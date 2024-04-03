// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class ButtonTypePreviewImage : PreviewImageGenerator
	{
		public ButtonTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var Button = ObjectOfPreview as ButtonType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Button>( enabled: false );
				objectInSpace.ButtonType = Button;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 2.5 );
		}
	}
}
#endif