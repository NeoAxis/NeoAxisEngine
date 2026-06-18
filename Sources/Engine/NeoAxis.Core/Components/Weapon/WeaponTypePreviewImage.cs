// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class WeaponTypePreviewImage : PreviewImageGenerator
	{
		public WeaponTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var weapon = ObjectOfPreview as WeaponType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Weapon>( enabled: false );
				objectInSpace.WeaponType = weapon;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 1.5 );
		}
	}
}