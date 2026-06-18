// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class SkyPreviewImage : PreviewImageGenerator
	{
		public SkyPreviewImage()
		{
		}

		public override bool ClampImage
		{
			get { return false; }
		}

		public Sky Sky
		{
			get { return ObjectOfPreview as Sky; }
		}

		protected override void OnUpdate()
		{
			var scene = CreateScene( false );

			if( Sky != null )
			{
				var instanceInScene = (Sky)Sky.Clone();
				Scene.AddComponent( instanceInScene );

				CameraDirection = new SphericalDirection( 0, 0 );
			}

			scene.Enabled = true;
		}
	}
}