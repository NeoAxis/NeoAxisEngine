#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class SkeletonEditor : CanvasBasedEditor
	{
		bool firstCameraUpdate = true;

		//

		public Skeleton Skeleton
		{
			get { return (Skeleton)ObjectOfEditor; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			var scene = CreateScene( false );
			scene.Enabled = true;

			if( ObjectOfEditor != null )
				SelectObjects( new object[] { ObjectOfEditor } );
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		protected override void OnViewportCreated()
		{
			base.OnViewportCreated();

			ViewportControl.Viewport.AllowRenderScreenLabels = false;
		}

		protected override void OnSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			base.OnSceneViewportUpdateGetCameraSettings( ref processed );

			if( firstCameraUpdate && Scene.CameraEditor.Value != null )
			{
				InitCamera();
				Viewport.CameraSettings = new Viewport.CameraSettingsClass( Viewport, Scene.CameraEditor );
			}

			//if( Mesh != null && Scene.CameraEditor.Value != null )
			//	Mesh.EditorCameraTransform = Scene.CameraEditor.Value.Transform;

			firstCameraUpdate = false;
		}

		void InitCamera()
		{
			var camera = Scene.CameraEditor.Value;
			var bounds = Scene.CalculateTotalBoundsOfObjectsInSpace();

			var skeleton = Skeleton;
			if( skeleton != null )
			{
				foreach( var bone in skeleton.GetBones() )
				{
					var tr = bone.Transform.Value;
					bounds.Add( tr.Position );
				}
			}

			var cameraLookTo = bounds.GetCenter();

			double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
			double distance = maxGararite * 2;
			if( distance < 2 )
				distance = 2;

			double cameraZoomFactor = 1;
			SphericalDirection cameraDirection = new SphericalDirection( -3.83, -.47 );

			var cameraPosition = cameraLookTo - cameraDirection.GetVector() * distance * cameraZoomFactor;
			var center = cameraLookTo;// GetSceneCenter();

			Vector3 from = cameraPosition;//center + cameraDirection.GetVector() * cameraDistance;
			Vector3 to = center;
			Degree fov = 65;// 75;

			//camera.AspectRatio = (double)ViewportControl.Viewport.SizeInPixels.X / (double)ViewportControl.Viewport.SizeInPixels.Y;
			camera.FieldOfView = fov;
			camera.NearClipPlane = Math.Max( distance / 10000, 0.01 );//.1;
			camera.FarClipPlane = Math.Max( 1000, distance * 2 );

			//if( Mesh != null && Mesh.EditorCameraTransform != null )
			//	camera.Transform = Mesh.EditorCameraTransform;
			//else
			camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );

			camera.FixedUp = Vector3.ZAxis;
		}

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			var skeleton = Skeleton;
			if( skeleton != null )
			{
				//center axes
				//if( Mesh.EditorDisplayPivot )
				{
					var sizeInPixels = 35 * EditorAPI2.DPIScale;
					var size = Viewport.Simple3DRenderer.GetThicknessByPixelSize( Vector3.Zero, sizeInPixels );

					var thickness = size / 20;
					var headHeight = size / 3;
					var headRadius = headHeight / 3;
					//!!!!рисовать с прозрачность но в режиме чтобы не перекрывалось
					var alpha = 1.0;// 0.5;

					Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0, alpha ), false );//, true );
					Viewport.Simple3DRenderer.AddArrow( Vector3.Zero, Vector3.XAxis * size, headHeight, headRadius, true, thickness );
					Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0, alpha ), false );//, true );
					Viewport.Simple3DRenderer.AddArrow( Vector3.Zero, Vector3.YAxis * size, headHeight, headRadius, true, thickness );
					Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1, alpha ), false );//, true );
					Viewport.Simple3DRenderer.AddArrow( Vector3.Zero, Vector3.ZAxis * size, headHeight, headRadius, true, thickness );
				}

				//skeleton
				foreach( var bone in skeleton.GetBones() )
				{
					var tr = bone.Transform.Value;

					var pos = tr.Position;
					var parent = bone.Parent as SkeletonBone;
					if( parent != null )
					{
						var from = parent.Transform.Value.Position;

						var isSelected = SelectedObjectsSet.Contains( bone );

						ColorValue color;
						if( isSelected )
							color = new ColorValue( 0, 1, 0 );
						else
							color = new ColorValue( 0, 0.5, 1, 0.7 );
						Viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

						Viewport.Simple3DRenderer.AddArrow( from, pos );

						if( isSelected )
						{
							var length = ( pos - from ).Length() / 3;

							Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ), new ColorValue( 1, 0, 0 ) * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							Viewport.Simple3DRenderer.AddArrow( pos, pos + tr.Rotation * new Vector3( length, 0, 0 ) );

							Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0 ), new ColorValue( 0, 1, 0 ) * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							Viewport.Simple3DRenderer.AddArrow( pos, pos + tr.Rotation * new Vector3( 0, length, 0 ) );

							Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1 ), new ColorValue( 0, 0, 1 ) * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							Viewport.Simple3DRenderer.AddArrow( pos, pos + tr.Rotation * new Vector3( 0, 0, length ) );
						}
					}
				}
			}
		}

		protected override void OnViewportUpdateBeforeOutput2()
		{
			base.OnViewportUpdateBeforeOutput2();
		}

		static string Translate( string text )
		{
			return EditorLocalization2.Translate( "SkeletonEditor", text );
		}

		protected override void OnGetTextInfoLeftTopCorner( List<string> lines )
		{
			base.OnGetTextInfoLeftTopCorner( lines );

			var skeleton = Skeleton;
			if( skeleton != null )
			{
				var bones = skeleton.GetComponents<SkeletonBone>( checkChildren: true );
				lines.Add( Translate( "Bones" ) + ": " + bones.Length.ToString( "N0" ) );
			}
		}
	}
}
#endif