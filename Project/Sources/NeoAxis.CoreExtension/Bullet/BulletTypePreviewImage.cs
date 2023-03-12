// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class BulletTypePreviewImage : PreviewImageGenerator
	{
		public BulletTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var Bullet = ObjectOfPreview as BulletType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Bullet>( enabled: false );
				objectInSpace.BulletType = Bullet;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 1.5 );
		}
	}
}
#endif