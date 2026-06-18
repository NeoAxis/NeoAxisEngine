// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Button3DTypePreviewImage : PreviewImageGenerator
	{
		public Button3DTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var type = ObjectOfPreview as Button3DType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Button3D>( enabled: false );
				objectInSpace.Button3DType = type;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 2.5 );
		}
	}
}