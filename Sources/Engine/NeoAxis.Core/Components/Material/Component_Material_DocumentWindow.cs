// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NeoAxis.Widget;

namespace NeoAxis.Editor
{
	public partial class Component_Material_DocumentWindow : DocumentWindowWithViewport
	{
		bool needCameraUpdate = true;

		//bool needUpdate = true;
		//double? needUpdateTime;

		Component_Mesh previewMeshUsed;
		Component_Mesh defaultMesh;

		Reference<Component_Image> previewEnvironmentUsed;
		ColorValuePowered previewEnvironmentMultiplierUsed;
		double previewEnvironmentAffectLightingUsed = -1;
		Component_Skybox skybox;

		//

		public Component_Material_DocumentWindow()
		{
			InitializeComponent();
		}

		public Component_Material Material
		{
			get { return (Component_Material)ObjectOfWindow; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			//create scene
			{
				var scene = CreateScene( false );

				var meshInSpace = scene.CreateComponent<Component_MeshInSpace>();
				meshInSpace.Name = "Mesh In Space";
				meshInSpace.ReplaceMaterial = Material;

				UpdatePreviewMesh();
				UpdatePreviewEnvironment();

				scene.Enabled = true;
			}

			if( ObjectOfWindow != null )
				SelectObjects( new object[] { ObjectOfWindow } );

			//if( Document != null )
			//	Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;

			timer1.Start();
		}

		protected override void OnDestroy()
		{
			//if( Document != null )
			//	Document.UndoSystem.ListOfActionsChanged -= UndoSystem_ListOfActionsChanged;

			base.OnDestroy();
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );

			//!!!!?
			ViewportControl.Viewport.AllowRenderScreenLabels = false;
		}

		protected override void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
		{
			base.Scene_ViewportUpdateGetCameraSettings( scene, viewport, ref processed );

			//check update preview mesh
			{
				var mesh = ProjectSettings.Get.MaterialPreviewMesh.Value;
				if( previewMeshUsed != mesh )
				{
					UpdatePreviewMesh();
					needCameraUpdate = true;
				}
			}

			UpdatePreviewEnvironment();

			if( needCameraUpdate && scene.CameraEditor.Value != null )
			{
				InitCamera();
				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, scene.CameraEditor );

				needCameraUpdate = false;
			}
		}

		void InitCamera()
		{
			var camera = Scene.CameraEditor.Value;
			var bounds = Scene.CalculateTotalBoundsOfObjectsInSpace();
			var cameraLookTo = bounds.GetCenter();

			//!!!!

			double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
			double distance = maxGararite * 2;// 2.3;
			if( distance < 2 )
				distance = 2;

			double cameraZoomFactor = 1;
			SphericalDirection cameraDirection = new SphericalDirection( -3.83, -.47 );

			var cameraPosition = cameraLookTo - cameraDirection.GetVector() * distance * cameraZoomFactor;
			var center = cameraLookTo;// GetSceneCenter();

			Vector3 from = cameraPosition;//center + cameraDirection.GetVector() * cameraDistance;
			Vector3 to = center;
			Degree fov = 65;// 75;

			//!!!!
			//camera.AspectRatio = (double)ViewportControl.Viewport.SizeInPixels.X / (double)ViewportControl.Viewport.SizeInPixels.Y;
			camera.FieldOfView = fov;
			camera.NearClipPlane = Math.Max( distance / 10000, 0.01 );//.1;
			camera.FarClipPlane = Math.Max( 1000, distance * 2 );

			camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );
			camera.FixedUp = Vector3.ZAxis;
		}

		static string Translate( string text )
		{
			return EditorLocalization.Translate( "MaterialDocumentWindow", text );
		}

		protected override void GetTextInfoLeftTopCorner( List<string> lines )
		{
			base.GetTextInfoLeftTopCorner( lines );

			if( Material != null )
			{
				var data = Material.Result;
				if( data != null )
				{
					var standardData = data as Component_Material.CompiledMaterialData;
					if( standardData != null )
					{
						if( standardData.deferredShadingSupport )
							lines.Add( Translate( "Deferred shading is supported." ) );
						else
							lines.Add( string.Format( Translate( "Deferred shading is not supported because {0}." ), standardData.deferredShadingSupportReason ) );

						if( standardData.receiveDecalsSupport )
							lines.Add( Translate( "Receiving decals is supported." ) );
						else
							lines.Add( string.Format( Translate( "Receiving decals is not supported because {0}." ), standardData.receiveDecalsSupportReason ) );

						if( standardData.decalSupport )
							lines.Add( Translate( "Decal rendering is supported." ) );
						else
							lines.Add( string.Format( Translate( "Decal rendering is not supported because {0}." ), standardData.decalSupportReason ) );
					}
				}
			}
		}

		//private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		//{
		//	//needUpdate = true;
		//	needUpdateTime = EngineApp.GetSystemTime() + 0.1;

		//	////!!!!
		//	//Log.Warning( "update" );
		//}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			if( Material != null )
			{
				//!!!!

				//if( Material.EditorAutoUpdate )//!!!! && needUpdate )
				//{
				//	Material.ResultCompile();

				//	//Material.ShouldRecompile = true;
				//	//needUpdate = false;
				//}

				//if( Material.EditorAutoUpdate && needUpdateTime.HasValue )
				//{
				//	var time = EngineApp.GetSystemTime();
				//	if( time > needUpdateTime.Value )
				//	{
				//		Material.ResultCompile();
				//		//Material.ShouldRecompile = true;
				//		needUpdateTime = null;
				//	}
				//}
			}
		}

		void UpdatePreviewMesh()
		{
			var mesh = ProjectSettings.Get.MaterialPreviewMesh.Value;

			previewMeshUsed = mesh;

			if( mesh == null )
			{
				if( defaultMesh == null )
				{
					//sphere if null
					defaultMesh = Scene.CreateComponent<Component_Mesh>( enabled: false );
					var sphere = defaultMesh.CreateComponent<Component_MeshGeometry_Sphere>();
					sphere.SegmentsHorizontal = 64;
					sphere.SegmentsVertical = 64;
					defaultMesh.Enabled = true;
				}
				mesh = defaultMesh;
			}

			var meshInSpace = (Component_MeshInSpace)Scene.GetComponent( "Mesh In Space" );
			meshInSpace.Mesh = mesh;
		}

		void UpdatePreviewEnvironment()
		{
			var env = ProjectSettings.Get.GetMaterialPreviewEnvironment();
			var multiplier = ProjectSettings.Get.MaterialPreviewEnvironmentMultiplier.Value;
			var affect = ProjectSettings.Get.MaterialPreviewEnvironmentAffectLighting.Value;

			if( !previewEnvironmentUsed.Equals( env ) || previewEnvironmentMultiplierUsed != multiplier || previewEnvironmentAffectLightingUsed != affect )
			{
				previewEnvironmentUsed = env;
				previewEnvironmentMultiplierUsed = multiplier;
				previewEnvironmentAffectLightingUsed = affect;

				if( env.Value != null )
				{
					if( skybox == null )
						skybox = Scene.CreateComponent<Component_Skybox>();
					skybox.Cubemap = env;
					skybox.Multiplier = multiplier;
					skybox.AffectLighting = affect;
				}
				else
				{
					skybox?.Dispose();
					skybox = null;
				}
			}
		}
	}
}
