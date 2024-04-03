// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class ButtonTypePreview : CanvasBasedPreview
	{
		Button objectInSpace;

		//

		public ButtonType ButtonType
		{
			get { return ObjectOfPreview as ButtonType; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Button>( enabled: false );
			objectInSpace.ButtonType = ButtonType;
			objectInSpace.Enabled = true;
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( ButtonType != null )
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