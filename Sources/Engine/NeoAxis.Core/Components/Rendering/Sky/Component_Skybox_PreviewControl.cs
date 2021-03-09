// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Component_Skybox_PreviewControl : PreviewControlWithViewport
	{
		double lastUpdateTime;
		Component_Skybox instanceInScene;

		//

		public Component_Skybox_PreviewControl()
		{
			InitializeComponent();
		}

		public Component_Skybox Skybox
		{
			get { return ObjectOfPreview as Component_Skybox; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( Skybox != null )
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Enabled = true;

				CameraDirection = new SphericalDirection( 0, 0 );
			}
		}

		protected override void Viewport_Tick( Viewport sender, float delta )
		{
			base.Viewport_Tick( sender, delta );

			//update
			if( instanceInScene != null && Time.Current > lastUpdateTime + 0.05 )
			{
				instanceInScene.Cubemap = Skybox.Cubemap;
				instanceInScene.Rotation = Skybox.Rotation;
				instanceInScene.Multiplier = Skybox.Multiplier;
				instanceInScene.LightingMultiplier = Skybox.LightingMultiplier;
				instanceInScene.AlwaysUseProcessedCubemap = Skybox.AlwaysUseProcessedCubemap;
				instanceInScene.AllowProcessEnvironmentCubemap = Skybox.AllowProcessEnvironmentCubemap;

				//CreateObject();

				lastUpdateTime = Time.Current;
			}
		}

		//protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		//{
		//	base.Viewport_UpdateBeforeOutput( viewport );

		//	//update
		//	if( instanceInScene != null && Time.Current > lastUpdateTime + 0.1 )
		//	{
		//		CreateObject();
		//		lastUpdateTime = Time.Current;
		//	}
		//}

		void CreateObject()
		{
			instanceInScene?.Dispose();

			instanceInScene = (Component_Skybox)Skybox.Clone();
			Scene.AddComponent( instanceInScene );
		}
	}
}
