// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Specifies the engine settings.
	/// </summary>
	public static class EngineSettings
	{
		//!!!!!всё заюзано?
		//!!!!конфиги работают для всего?
		//!!!!!всё норм?

		//!!!!!gamma? куда её?

		/// <summary>
		/// Represents initialization parameters for <see cref="EngineSettings"/>.
		/// </summary>
		public static class Init
		{
			public static RendererBackend RendererBackend = RendererBackend.Default;
			public static bool RendererReportDebugToLog;
			public static bool SimulationVSync = true;
			public static bool SimulationTripleBuffering;
			public static bool UseShaderCache = true;
			public static bool AnisotropicFiltering = true;

			public static string SoundSystemDLL = "";
			public static int SoundMaxReal2DChannels = 32;
			public static int SoundMaxReal3DChannels = 50;

			public static bool ScriptingCompileProjectSolutionAtStartup = true;
		}

		////!!!!DebugRendering?
		//public static class Debug
		//{
		//	//!!!!!общий флаг для выключения какого-либо отладочного рисования
		//	static bool enabled = true;

		//	//!!!!!drawDebugGeometry?

		//	//!!!!!bool enableDeferredRendering;

		//	//visibility
		//	[EngineConfig( "Debug", "drawFullScreenEffects" )]
		//	static bool drawFullScreenEffects = true;
		//	[EngineConfig( "Debug", "drawSky" )]
		//	static bool drawSky = true;
		//	[EngineConfig( "Debug", "drawModels" )]
		//	static bool drawModels = true;
		//	[EngineConfig( "Debug", "drawEffects" )]
		//	static bool drawEffects = true;

		//	//!!!!new
		//	[EngineConfig( "Debug", "drawInGame3DGUI" )]
		//	static bool drawInGame3DGUI = true;
		//	[EngineConfig( "Debug", "drawFog" )]
		//	static bool drawFog = true;
		//	[EngineConfig( "Debug", "drawShadows" )]
		//	static bool drawShadows = true;
		//	[EngineConfig( "Debug", "drawAmbientLighting" )]
		//	static bool drawAmbientLighting = true;
		//	[EngineConfig( "Debug", "drawDynamicLighting" )]
		//	static bool drawDynamicLighting = true;
		//	[EngineConfig( "Debug", "drawScreenGUI" )]
		//	static bool drawScreenGUI = true;

		//	//debug geometry
		//	[EngineConfig( "Debug", "drawStaticPhysics" )]
		//	static bool drawStaticPhysics;
		//	[EngineConfig( "Debug", "drawDynamicPhysics" )]
		//	static bool drawDynamicPhysics;
		//	[EngineConfig( "Debug", "drawPhysicsRayVolumeChecks" )]
		//	static bool drawPhysicsRayVolumeChecks;
		//	[EngineConfig( "Debug", "drawSceneGraphOfSceneNodes" )]
		//	static bool drawSceneGraphOfSceneNodes;
		//	[EngineConfig( "Debug", "drawSceneGraphOfLights" )]
		//	static bool drawSceneGraphOfLights;
		//	[EngineConfig( "Debug", "drawSceneGraphOfMapObjects" )]
		//	static bool drawSceneGraphOfMapObjects;
		//	[EngineConfig( "Debug", "drawRegions" )]
		//	static bool drawRegions;
		//	[EngineConfig( "Debug", "drawMapObjectBounds" )]
		//	static bool drawMapObjectBounds;
		//	[EngineConfig( "Debug", "drawSceneNodeBounds" )]
		//	static bool drawSceneNodeBounds;
		//	[EngineConfig( "Debug", "drawZonesPortalsOccluders" )]
		//	static bool drawZonesPortalsOccluders;
		//	[EngineConfig( "Debug", "drawLightVolumes" )]
		//	static bool drawLightVolumes;
		//	//!!!!!need?
		//	[EngineConfig( "Debug", "drawGameSpecificDebugGeometry" )]
		//	static bool drawGameSpecificDebugGeometry;

		//	//special options
		//	[EngineConfig( "Debug", "drawInWireframeMode" )]
		//	static bool drawInWireframeMode;
		//	[EngineConfig( "Debug", "frustumCullingTest" )]
		//	static bool frustumCullingTest;
		//	[EngineConfig( "Debug", "allowHardwareInstancing" )]
		//	static bool allowHardwareInstancing;//!!!!! = true;
		//	[EngineConfig( "Debug", "drawShadowDebugging" )]
		//	static bool drawShadowDebugging;
		//	[EngineConfig( "Debug", "allowPortalSystem" )]
		//	static bool allowPortalSystem = true;

		//	//

		//	[DefaultValue( true )]
		//	public static bool Enabled
		//	{
		//		get { return enabled; }
		//		set { enabled = value; }
		//	}

		//	[DefaultValue( true )]
		//	public static bool DrawFullScreenEffects
		//	{
		//		get { return drawFullScreenEffects; }
		//		set
		//		{
		//			if( drawFullScreenEffects == value )
		//				return;
		//			drawFullScreenEffects = value;

		//			//!!!!!было
		//			//if( RendererWorld.Instance != null )
		//			//{
		//			//   foreach( Viewport viewport in RendererWorld.Viewports )
		//			//      viewport.SetNeedRecreateCompositors();
		//			//}
		//		}
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw sky.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw sky." )]
		//	[DefaultValue( true )]
		//	public static bool DrawSky
		//	{
		//		get { return drawSky; }
		//		set { drawSky = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw models.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw models." )]
		//	[DefaultValue( true )]
		//	public static bool DrawModels
		//	{
		//		get { return drawModels; }
		//		set { drawModels = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw effects.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw effects." )]
		//	[DefaultValue( true )]
		//	public static bool DrawEffects
		//	{
		//		get { return drawEffects; }
		//		set { drawEffects = value; }
		//	}

		//	[DefaultValue( true )]
		//	public static bool DrawInGame3DGUI
		//	{
		//		get { return drawInGame3DGUI; }
		//		set { drawInGame3DGUI = value; }
		//	}

		//	[DefaultValue( true )]
		//	public static bool DrawFog
		//	{
		//		get { return drawFog; }
		//		set { drawFog = value; }
		//	}

		//	[DefaultValue( true )]
		//	public static bool DrawShadows
		//	{
		//		get { return drawShadows; }
		//		set { drawShadows = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw ambient lighting.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw ambient lighting." )]
		//	[DefaultValue( true )]
		//	public static bool DrawAmbientLighting
		//	{
		//		get { return drawAmbientLighting; }
		//		set { drawAmbientLighting = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw dynamic lighting.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw dynamic lighting." )]
		//	[DefaultValue( true )]
		//	public static bool DrawDynamicLighting
		//	{
		//		get { return drawDynamicLighting; }
		//		set { drawDynamicLighting = value; }
		//	}

		//	[DefaultValue( true )]
		//	public static bool DrawScreenGUI
		//	{
		//		get { return drawScreenGUI; }
		//		set { drawScreenGUI = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw static physics.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw static physics." )]
		//	[DefaultValue( false )]
		//	public static bool DrawStaticPhysics
		//	{
		//		get { return drawStaticPhysics; }
		//		set { drawStaticPhysics = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw dynamic physics.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw dynamic physics." )]
		//	[DefaultValue( false )]
		//	public static bool DrawDynamicPhysics
		//	{
		//		get { return drawDynamicPhysics; }
		//		set { drawDynamicPhysics = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to visualize ray casts, volume casts which happens during updating this frame.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to visualize ray casts, volume casts which happens during updating this frame." )]
		//	[DefaultValue( false )]
		//	public static bool DrawPhysicsRayVolumeChecks
		//	{
		//		get { return drawPhysicsRayVolumeChecks; }
		//		set { drawPhysicsRayVolumeChecks = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw scene graph of scene nodes.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw scene graph of scene nodes." )]
		//	[DefaultValue( false )]
		//	public static bool DrawSceneGraphOfSceneNodes
		//	{
		//		get { return drawSceneGraphOfSceneNodes; }
		//		set { drawSceneGraphOfSceneNodes = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw scene graph of lights.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw scene graph of lights." )]
		//	[DefaultValue( false )]
		//	public static bool DrawSceneGraphOfLights
		//	{
		//		get { return drawSceneGraphOfLights; }
		//		set { drawSceneGraphOfLights = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw scene graph of MapObjects.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw scene graph of MapObjects." )]
		//	[DefaultValue( false )]
		//	public static bool DrawSceneGraphOfMapObjects
		//	{
		//		get { return drawSceneGraphOfMapObjects; }
		//		set { drawSceneGraphOfMapObjects = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw regions.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw regions." )]
		//	[DefaultValue( false )]
		//	public static bool DrawRegions
		//	{
		//		get { return drawRegions; }
		//		set { drawRegions = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw map object bounds.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw map object bounds." )]
		//	[DefaultValue( false )]
		//	public static bool DrawMapObjectBounds
		//	{
		//		get { return drawMapObjectBounds; }
		//		set { drawMapObjectBounds = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw scene node bounds.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw scene node bounds." )]
		//	[DefaultValue( false )]
		//	public static bool DrawSceneNodeBounds
		//	{
		//		get { return drawSceneNodeBounds; }
		//		set { drawSceneNodeBounds = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw zones, portals and occluders.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw zones, portals and occluders." )]
		//	[Browsable( false )]
		//	[DefaultValue( false )]
		//	public static bool DrawZonesPortalsOccluders
		//	{
		//		get { return drawZonesPortalsOccluders; }
		//		set { drawZonesPortalsOccluders = value; }
		//	}

		//	[DefaultValue( false )]
		//	public static bool DrawLightVolumes
		//	{
		//		get { return drawLightVolumes; }
		//		set { drawLightVolumes = value; }
		//	}

		//	/// <summary>
		//	/// Gets or sets a value indicating whether showing it is necessary to draw game specific debug geometry.
		//	/// </summary>
		//	[Description( "A value indicating whether showing it is necessary to draw game specific debug geometry." )]
		//	[DefaultValue( false )]
		//	public static bool DrawGameSpecificDebugGeometry
		//	{
		//		get { return drawGameSpecificDebugGeometry; }
		//		set { drawGameSpecificDebugGeometry = value; }
		//	}

		//	[DefaultValue( false )]
		//	public static bool DrawInWireframeMode
		//	{
		//		get { return drawInWireframeMode; }
		//		set { drawInWireframeMode = value; }
		//	}

		//	[DefaultValue( false )]
		//	public static bool FrustumCullingTest
		//	{
		//		get { return frustumCullingTest; }
		//		set { frustumCullingTest = value; }
		//	}

		//	[DefaultValue( true )]
		//	public static bool AllowHardwareInstancing
		//	{
		//		get { return allowHardwareInstancing; }
		//		set { allowHardwareInstancing = value; }
		//	}

		//	[DefaultValue( false )]
		//	public static bool DrawShadowDebugging
		//	{
		//		get { return drawShadowDebugging; }
		//		set { drawShadowDebugging = value; }
		//	}

		//	[DefaultValue( true )]
		//	public static bool AllowPortalSystem
		//	{
		//		get { return allowPortalSystem; }
		//		set { allowPortalSystem = value; }
		//	}
		//}
	}
}
