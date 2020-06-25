// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Widget;

namespace NeoAxis.Editor
{
	public partial class Component_ParticleSystem_DocumentWindow : DocumentWindowWithViewport
	{
		bool firstCameraUpdate = true;

		bool needResultCompile;

		int particleCount;
		double particleCountLastUpdateTime;

		//

		public Component_ParticleSystem_DocumentWindow()
		{
			InitializeComponent();
		}

		public Component_ParticleSystem ParticleSystem
		{
			get { return (Component_ParticleSystem)ObjectOfWindow; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			var scene = CreateScene( false );
			if( ParticleSystem != null )
			{
				var objInSpace = scene.CreateComponent<Component_ParticleSystemInSpace>();
				objInSpace.ParticleSystem = ParticleSystem;
			}
			scene.Enabled = true;

			if( Document != null )
				Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;

			if( ObjectOfWindow != null )
				SelectObjects( new object[] { ObjectOfWindow } );
		}

		protected override void OnDestroy()
		{
			if( Document != null )
				Document.UndoSystem.ListOfActionsChanged -= UndoSystem_ListOfActionsChanged;

			base.OnDestroy();
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );

			//!!!!
			ViewportControl.Viewport.AllowRenderScreenLabels = false;
		}

		protected override void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
		{
			base.Scene_ViewportUpdateGetCameraSettings( scene, viewport, ref processed );

			if( firstCameraUpdate && scene.CameraEditor.Value != null )
			{
				InitCamera();
				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, scene.CameraEditor );
			}

			firstCameraUpdate = false;
		}

		void InitCamera()
		{
			//copy from Mesh document window code

			var camera = Scene.CameraEditor.Value;
			//!!!!
			var bounds = new Bounds( -5, -5, -5, 5, 5, 5 );
			//var bounds = Scene.CalculateTotalBoundsOfObjectsInSpace();
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

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			if( ParticleSystem != null && needResultCompile )
			{
				ParticleSystem.ResultCompile();
				needResultCompile = false;
			}

			//highlight emitter shapes
			if( ParticleSystem != null && ParticleSystem.Result != null )
			{
				foreach( var emitter in ParticleSystem.GetComponents<Component_ParticleEmitter>() )
				{
					if( emitter.Enabled )
					{
						bool emitterSelected = SelectedObjectsSet.Contains( emitter );

						var tr = Transform.Identity;
						int verticesRendered = 0;

						foreach( var shape in emitter.GetComponents<Component_ParticleEmitterShape>( false, false, false ) )
						{
							if( shape.Enabled )
							{
								ColorValue color;
								if( emitterSelected || SelectedObjectsSet.Contains( shape ) )
									color = ProjectSettings.Get.SelectedColor;
								else
								{
									//!!!!add editor options
									color = new ColorValue( 0, 0, 0.8 );
								}
								viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
								shape.PerformRender( viewport, tr, false, ref verticesRendered );
							}
						}
					}
				}

			}
		}

		static string Translate( string text )
		{
			return EditorLocalization.Translate( "ParticleSystemDocumentWindow", text );
		}

		protected override void GetTextInfoLeftTopCorner( List<string> lines )
		{
			base.GetTextInfoLeftTopCorner( lines );

			if( ParticleSystem != null && Scene != null )
			{
				var obj = Scene.GetComponent<Component_ParticleSystemInSpace>();
				if( obj != null )
				{
					////!!!!fallback, reason
					//lines.Add( "Batching: " + ( ParticleSystem.Batching.Value ? "Enabled" : "Disabled" ) );

					if( Time.Current > particleCountLastUpdateTime + 0.25 )
					{
						particleCountLastUpdateTime = Time.Current;
						particleCount = obj.ObjectsGetCount();
					}
					lines.Add( Translate( "Particles" ) + ": " + particleCount.ToString() );
				}
			}
		}

		/////////////////////////////////////////

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			needResultCompile = true;
		}
	}
}
