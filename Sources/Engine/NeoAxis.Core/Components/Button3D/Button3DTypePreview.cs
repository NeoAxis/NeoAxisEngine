// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Button3DTypePreview : CanvasBasedPreview
	{
		Button3D objectInSpace;

		//

		public Button3DType Button3DType
		{
			get { return ObjectOfPreview as Button3DType; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Button3D>( enabled: false );
			objectInSpace.Button3DType = Button3DType;
			objectInSpace.Enabled = true;
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( Button3DType != null )
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), 0.5 );
			}
		}
	}
}