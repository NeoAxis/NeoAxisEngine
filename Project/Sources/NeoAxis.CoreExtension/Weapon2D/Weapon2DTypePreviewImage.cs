// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Weapon2DTypePreviewImage : PreviewImageGenerator
	{
		public Weapon2DTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var weapon = ObjectOfPreview as Weapon2DType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Weapon2D>( enabled: false );
				objectInSpace.WeaponType = weapon;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), mode2D: true );
		}
	}
}
#endif