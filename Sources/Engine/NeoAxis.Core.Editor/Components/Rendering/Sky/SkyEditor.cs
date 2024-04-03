// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis.Editor
{
	public partial class SkyEditor : DocumentWindowWithViewport
	{
		double lastUpdateTime;
		//bool needRecreateInstance;
		Sky instanceInScene;

		//

		public SkyEditor()
		{
			InitializeComponent();
		}

		public Sky Sky
		{
			get { return (Sky)ObjectOfWindow; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( Sky != null )
			{
				var scene = CreateScene( false );

				instanceInScene = (Sky)Sky.Clone();
				scene.AddComponent( instanceInScene );

				scene.Enabled = true;

				//if( Document != null )
				//	Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;

				if( ObjectOfWindow != null )
					SelectObjects( new object[] { ObjectOfWindow } );
			}
		}

		//protected override void OnDestroy()
		//{
		//	if( Document != null )
		//		Document.UndoSystem.ListOfActionsChanged -= UndoSystem_ListOfActionsChanged;

		//	base.OnDestroy();
		//}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );

			ViewportControl2.Viewport.AllowRenderScreenLabels = false;
		}

		protected override void Viewport_Tick( Viewport viewport, float delta )
		{
			base.Viewport_Tick( viewport, delta );

			//update
			if( instanceInScene != null && Time.Current > lastUpdateTime + 0.05 )
			//if( instanceInScene != null && needRecreateInstance )// Time.Current > lastUpdateTime + 0.05 )
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
				//needRecreateInstance = false;
			}
		}

		//protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		//{
		//	base.Viewport_UpdateBeforeOutput( viewport );

		//	if( Sky != null && needRecreateInstance )
		//	{
		//		instanceInScene?.Dispose();

		//		instanceInScene = (Sky)Skybox.Clone();
		//		Scene.AddComponent( instanceInScene );

		//		needRecreateInstance = false;
		//	}
		//}

		//private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		//{
		//	needRecreateInstance = true;
		//}
	}
}