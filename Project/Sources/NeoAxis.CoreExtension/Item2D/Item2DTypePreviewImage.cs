// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Item2DTypePreviewImage : PreviewImageGenerator
	{
		public Item2DTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var item = ObjectOfPreview as Item2DType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Item2D>( enabled: false );
				objectInSpace.ItemType = item;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), mode2D: true );
		}
	}
}
#endif