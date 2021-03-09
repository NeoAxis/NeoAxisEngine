// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Component_Surface_PreviewImage : PreviewImageGenerator
	{
		public Component_Surface_PreviewImage()
		{
		}

		protected override void Update()
		{
			var surface = ObjectOfPreview as Component_Surface;

			var scene = CreateScene( false );
			Component_SurfaceUtility.CreatePreviewObjects( scene, surface );
			scene.Enabled = true;

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 1.6 );
		}
	}
}
