// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class SkyboxPreviewImage : PreviewImageGenerator
	{
		public SkyboxPreviewImage()
		{
		}

		public override bool ClampImage
		{
			get { return false; }
		}

		public Skybox Skybox
		{
			get { return ObjectOfPreview as Skybox; }
		}

		protected override void OnUpdate()
		{
			var scene = CreateScene( false );

			if( Skybox != null )
			{
				var instanceInScene = (Skybox)Skybox.Clone();
				Scene.AddComponent( instanceInScene );

				CameraDirection = new SphericalDirection( 0, 0 );
			}

			scene.Enabled = true;
		}
	}
}
