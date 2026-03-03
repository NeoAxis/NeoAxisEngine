// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Item3DTypePreviewImage : PreviewImageGenerator
	{
		public Item3DTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var item = ObjectOfPreview as Item3DType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Item3D>( enabled: false );
				objectInSpace.ItemType = item;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 1.5 );
		}
	}
}
#endif