// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Character2DTypePreviewImage : PreviewImageGenerator
	{
		public Character2DTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var Character2D = ObjectOfPreview as Character2DType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Character2D>( enabled: false );
				objectInSpace.NewObjectSetDefaultConfiguration();
				objectInSpace.CharacterType = Character2D;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), mode2D: true );
		}
	}
}
