// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Weapon2DTypePreview : CanvasBasedPreview
	{
		Weapon2D objectInSpace;

		//

		public Weapon2DType WeaponType
		{
			get { return ObjectOfPreview as Weapon2DType; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Weapon2D>( enabled: false );
			objectInSpace.WeaponType = WeaponType;
			objectInSpace.Enabled = true;
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( WeaponType != null )
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), mode2D: true );
			}
		}
	}
}
#endif