// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Component_Skybox_PreviewImage : PreviewImageGenerator
	{
		public Component_Skybox_PreviewImage()
		{
		}

		public override bool ClampImage
		{
			get { return false; }
		}

		public Component_Skybox Skybox
		{
			get { return ObjectOfPreview as Component_Skybox; }
		}

		protected override void Update()
		{
			var scene = CreateScene( false );

			if( Skybox != null )
			{
				var instanceInScene = (Component_Skybox)Skybox.Clone();
				Scene.AddComponent( instanceInScene );

				CameraDirection = new SphericalDirection( 0, 0 );
			}

			scene.Enabled = true;
		}
	}
}
