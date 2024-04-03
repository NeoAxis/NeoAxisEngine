// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class SkyPreview : PreviewControlWithViewport
	{
		double lastUpdateTime;
		Sky instanceInScene;

		//

		public SkyPreview()
		{
			InitializeComponent();
		}

		public Sky Sky
		{
			get { return ObjectOfPreview as Sky; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( Sky != null )
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
				instanceInScene.Mode = Sky.Mode;
				instanceInScene.MixedModeProceduralFactor = Sky.MixedModeProceduralFactor;
				//instanceInScene.ProceduralIntensity = Sky.ProceduralIntensity;
				instanceInScene.ProceduralLuminance = Sky.ProceduralLuminance;
				instanceInScene.ProceduralApplySunPower = Sky.ProceduralApplySunPower;
				instanceInScene.ProceduralTurbidity = Sky.ProceduralTurbidity;
				instanceInScene.ProceduralSunSize = Sky.ProceduralSunSize;
				instanceInScene.ProceduralSunBloom = Sky.ProceduralSunBloom;
				instanceInScene.ProceduralExposition = Sky.ProceduralExposition;
				instanceInScene.ProceduralPreventBanding = Sky.ProceduralPreventBanding;
				instanceInScene.ProceduralResolution = Sky.ProceduralResolution;
				instanceInScene.Cubemap = Sky.Cubemap;
				instanceInScene.CubemapRotation = Sky.CubemapRotation;
				instanceInScene.CubemapStretch = Sky.CubemapStretch;
				instanceInScene.CubemapMultiplier = Sky.CubemapMultiplier;
				instanceInScene.AffectLighting = Sky.AffectLighting;
				instanceInScene.LightingCubemap = Sky.LightingCubemap;
				instanceInScene.LightingCubemapRotation = Sky.LightingCubemapRotation;
				instanceInScene.AlwaysUseProcessedCubemap = Sky.AlwaysUseProcessedCubemap;
				instanceInScene.AllowProcessEnvironmentCubemap = Sky.AllowProcessEnvironmentCubemap;

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

			instanceInScene = (Sky)Sky.Clone();
			Scene.AddComponent( instanceInScene );
		}
	}
}