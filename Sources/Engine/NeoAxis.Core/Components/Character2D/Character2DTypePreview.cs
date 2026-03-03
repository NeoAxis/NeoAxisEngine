// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Character2DTypePreview : CanvasBasedPreview
	{
		Character2D objectInSpace;

		//

		public Character2DType Character2DType
		{
			get { return ObjectOfPreview as Character2DType; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Character2D>( enabled: false );
			objectInSpace.NewObjectSetDefaultConfiguration();
			objectInSpace.CharacterType = Character2DType;
			objectInSpace.Enabled = true;
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( Character2DType != null )
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