// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Representation of a player's point of view.
	/// </summary>
	[EditorPreviewControl( typeof( Component_Camera_PreviewControl ) )]
	public class Component_Camera : Component_ObjectInSpace
	{
		//!!!!
		//bool reflectionEnabled;
		//Plane reflectionPlane = new Plane( Vec3.XAxis, 0 );

		//!!!!
		//double lodBias = 1;

		////!!!!!в viewport?
		////!!!!!настроить всем
		//bool allowFrustumCullingTestMode;
		////bool allowZonesAndPortalsSceneManagement = true;

		/// <summary>
		/// The projection mode of the camera.
		/// </summary>
		[DefaultValue( ProjectionType.Perspective )]
		//[Category( "General" )]
		public Reference<ProjectionType> Projection
		{
			get { if( _projection.BeginGet() ) Projection = _projection.Get( this ); return _projection.value; }
			set { if( _projection.BeginSet( ref value ) ) { try { ProjectionChanged?.Invoke( this ); SpaceBoundsUpdate(); } finally { _projection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Projection"/> property value changes.</summary>
		public event Action<Component_Camera> ProjectionChanged;
		ReferenceField<ProjectionType> _projection = ProjectionType.Perspective;

		/// <summary>
		/// The field of view of the perspective camera in degrees.
		/// </summary>
		[DefaultValue( "75" )]
		[Range( 1, 179 )]
		//[Category( "General" )]
		public Reference<Degree> FieldOfView
		{
			get { if( _fieldOfView.BeginGet() ) FieldOfView = _fieldOfView.Get( this ); return _fieldOfView.value; }
			set { if( _fieldOfView.BeginSet( ref value ) ) { try { FieldOfViewChanged?.Invoke( this ); SpaceBoundsUpdate(); } finally { _fieldOfView.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FieldOfView"/> property value changes.</summary>
		public event Action<Component_Camera> FieldOfViewChanged;
		ReferenceField<Degree> _fieldOfView = new Degree( 75 );

		/// <summary>
		/// The height of the orthographic camera.
		/// </summary>
		[DefaultValue( 100.0 )]
		[Range( 1, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//[Category( "General" )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set { if( _height.BeginSet( ref value ) ) { try { HeightChanged?.Invoke( this ); } finally { _height.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<Component_Camera> HeightChanged;
		ReferenceField<double> _height = 100;

		/// <summary>
		/// Up direction, determines camera orientation.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		//[Category( "General" )]
		public Reference<Vector3> FixedUp
		{
			get { if( _fixedUp.BeginGet() ) FixedUp = _fixedUp.Get( this ); return _fixedUp.value; }
			set { if( _fixedUp.BeginSet( ref value ) ) { try { FixedUpChanged?.Invoke( this ); } finally { _fixedUp.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FixedUp"/> property value changes.</summary>
		public event Action<Component_Camera> FixedUpChanged;
		ReferenceField<Vector3> _fixedUp = Vector3.ZAxis;

		/// <summary>
		/// The distance of near clipping plane.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Range( 0.01, 1 )]
		//[Category( "View Distance" )]
		public Reference<double> NearClipPlane
		{
			get { if( _nearClipPlane.BeginGet() ) NearClipPlane = _nearClipPlane.Get( this ); return _nearClipPlane.value; }
			set { if( _nearClipPlane.BeginSet( ref value ) ) { try { NearClipPlaneChanged?.Invoke( this ); } finally { _nearClipPlane.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="NearClipPlane"/> property value changes.</summary>
		public event Action<Component_Camera> NearClipPlaneChanged;
		ReferenceField<double> _nearClipPlane = 0.1;

		/// <summary>
		/// The distance of far clipping plane.
		/// </summary>
		[DefaultValue( 1000.0 )]
		[Range( 100, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//[Category( "View Distance" )]
		public Reference<double> FarClipPlane
		{
			get { if( _farClipPlane.BeginGet() ) FarClipPlane = _farClipPlane.Get( this ); return _farClipPlane.value; }
			set { if( _farClipPlane.BeginSet( ref value ) ) { try { FarClipPlaneChanged?.Invoke( this ); } finally { _farClipPlane.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FarClipPlane"/> property value changes.</summary>
		public event Action<Component_Camera> FarClipPlaneChanged;
		ReferenceField<double> _farClipPlane = 1000;

		/// <summary>
		/// The aspect ratio of the camera.
		/// </summary>
		[DefaultValue( 0.0 )]
		//[Category( "General" )]
		public Reference<double> AspectRatio
		{
			get { if( _aspectRatio.BeginGet() ) AspectRatio = _aspectRatio.Get( this ); return _aspectRatio.value; }
			set { if( _aspectRatio.BeginSet( ref value ) ) { try { AspectRatioChanged?.Invoke( this ); } finally { _aspectRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AspectRatio"/> property value changes.</summary>
		public event Action<Component_Camera> AspectRatioChanged;
		ReferenceField<double> _aspectRatio = 0.0;

		/// <summary>
		/// Aperture controls the brightness of the image that passes through the lens and falls on the image sensor.
		/// </summary>
		[DefaultValue( 16.0 )]
		[Range( 0.5, 64, RangeAttribute.ConvenientDistributionEnum.Exponential, 2 )]
		[Category( "Exposure" )]
		public Reference<double> Aperture
		{
			get { if( _aperture.BeginGet() ) Aperture = _aperture.Get( this ); return _aperture.value; }
			set { if( _aperture.BeginSet( ref value ) ) { try { ApertureChanged?.Invoke( this ); } finally { _aperture.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Aperture"/> property value changes.</summary>
		public event Action<Component_Camera> ApertureChanged;
		ReferenceField<double> _aperture = 16.0;

		/// <summary>
		/// Shutter speed or exposure time is the length of time when the film or digital sensor inside the camera is exposed to light.
		/// </summary>
		[DefaultValue( 0.003 )]//[DefaultValue( 0.008 )]
		[Range( 0.00004, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 8 )]
		//[Range( 0.00004, 60, RangeAttribute.ConvenientDistributionEnum.Exponential, 6 )]
		[Category( "Exposure" )]
		public Reference<double> ShutterSpeed
		{
			get { if( _shutterSpeed.BeginGet() ) ShutterSpeed = _shutterSpeed.Get( this ); return _shutterSpeed.value; }
			set { if( _shutterSpeed.BeginSet( ref value ) ) { try { ShutterSpeedChanged?.Invoke( this ); } finally { _shutterSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShutterSpeed"/> property value changes.</summary>
		public event Action<Component_Camera> ShutterSpeedChanged;
		ReferenceField<double> _shutterSpeed = 0.003;//0.008;

		/// <summary>
		/// Sensitivity refers to a film or digital camera sensor's sensitivity to light.
		/// </summary>
		[DefaultValue( 100.0 )]
		[Range( 10, 204800, RangeAttribute.ConvenientDistributionEnum.Exponential, 6 )]
		[Category( "Exposure" )]
		public Reference<double> Sensitivity
		{
			get { if( _sensitivity.BeginGet() ) Sensitivity = _sensitivity.Get( this ); return _sensitivity.value; }
			set { if( _sensitivity.BeginSet( ref value ) ) { try { SensitivityChanged?.Invoke( this ); } finally { _sensitivity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Sensitivity"/> property value changes.</summary>
		public event Action<Component_Camera> SensitivityChanged;
		ReferenceField<double> _sensitivity = 100.0;

		/// <summary>
		/// The rendering pipeline of the camera. When 'null' the rendering pipeline of the scene is used.
		/// </summary>
		[Category( "Rendering" )]
		public Reference<Component_RenderingPipeline> RenderingPipelineOverride
		{
			get { if( _renderingPipelineOverride.BeginGet() ) RenderingPipelineOverride = _renderingPipelineOverride.Get( this ); return _renderingPipelineOverride.value; }
			set { if( _renderingPipelineOverride.BeginSet( ref value ) ) { try { RenderingPipelineOverrideChanged?.Invoke( this ); } finally { _renderingPipelineOverride.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RenderingPipelineOverride"/> property value changes.</summary>
		public event Action<Component_Camera> RenderingPipelineOverrideChanged;
		ReferenceField<Component_RenderingPipeline> _renderingPipelineOverride;

		////!!!!
		//[Category( "Exposure" )]
		//[DefaultValue( 1.0 )]
		//[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> EmissiveMaterialsFactor
		//{
		//	get { if( _emissiveMaterialsFactor.BeginGet() ) EmissiveMaterialsFactor = _emissiveMaterialsFactor.Get( this ); return _emissiveMaterialsFactor.value; }
		//	set { if( _emissiveMaterialsFactor.BeginSet( ref value ) ) { try { EmissiveMaterialsFactorChanged?.Invoke( this ); } finally { _emissiveMaterialsFactor.EndSet(); } } }
		//}
		//public event Action<Component_Camera> EmissiveMaterialsFactorChanged;
		//ReferenceField<double> _emissiveMaterialsFactor = 1.0;

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//!!!!slowly?
			//!!!!так?
			var p = member as Metadata.Property;
			if( p != null )
			{
				if( p.Name == "FieldOfView" && Projection.Value != ProjectionType.Perspective )
				{
					skip = true;
					return;
				}
				if( p.Name == "Height" && Projection.Value != ProjectionType.Orthographic )
				{
					skip = true;
					return;
				}
			}
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() || !ParentScene.DisplayLabels )
				return false;
			//if( !ParentScene.GetShowDevelopmentDataInThisApplication() || !ParentScene.ShowCameras )
			//	return false;
			return base.OnEnabledSelectionByCursor();
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			newBounds = new SpaceBounds( GetVisualBounds() );
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			base.OnGetRenderSceneData( context, mode );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				//context.

				bool show = ( ParentScene.GetDisplayDevelopmentDataInThisApplication() && ParentScene.DisplayCameras ) ||
					context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
				if( show )
				{
					var skip = false;

					var cameraSettings = context.Owner.CameraSettings;
					//!!!!
					if( ( cameraSettings.Position - TransformV.Position ).Length() < 0.5 )
					{
						skip = true;
					}

					if( !skip && context2.displayCamerasCounter < context2.displayCamerasMax )
					{
						context2.displayCamerasCounter++;

						ColorValue color;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.SelectedColor;
						else if( context2.canSelectObjects.Contains( this ) )
							color = ProjectSettings.Get.CanSelectColor;
						else
							color = ProjectSettings.Get.SceneShowLightColor;

						var viewport = context.Owner;
						viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
						DebugDraw( viewport );
					}
				}
				//if( !show )
				//	context.disableShowingLabelForThisObject = true;
			}
		}

		//!!!!
		double GetVisualLength()
		{
			return 5;
		}

		public void DebugDraw( Viewport viewport )
		{
			var transform = Transform.Value;
			var pos = transform.Position;
			var rot = transform.Rotation;

			//direction
			{
				var toolSize = viewport.Simple3DRenderer.GetThicknessByPixelSize( pos, ProjectSettings.Get.TransformToolSizeScaled );
				double thickness = viewport.Simple3DRenderer.GetThicknessByPixelSize( pos, ProjectSettings.Get.TransformToolLineThicknessScaled );
				var length = toolSize / 1.5;
				var headHeight = length / 4;
				viewport.Simple3DRenderer.AddArrow( pos, pos + rot * new Vector3( length, 0, 0 ), headHeight, 0, true, thickness );
			}

			double aspectRatio = AspectRatio;
			if( aspectRatio == 0 )
				aspectRatio = 1;

			double halfWidth;
			double halfHeight;
			if( Projection.Value == ProjectionType.Perspective )
			{
				double tan = Math.Tan( FieldOfView.Value.InRadians() / 2 );
				halfWidth = tan * FarClipPlane.Value * aspectRatio;
				halfHeight = tan * FarClipPlane.Value;
			}
			else
			{
				halfWidth = Height * .5 * aspectRatio;
				halfHeight = Height * .5;
			}
			var frustum = new Frustum( true, Projection.Value, pos, rot, NearClipPlane.Value, FarClipPlane.Value, halfWidth, halfHeight );
			var frustumPoints = frustum.Points;

			switch( Projection.Value )
			{
			case ProjectionType.Perspective:
				{
					var points = new Vector3[ 8 ];
					{
						for( int n = 4; n < 8; n++ )
						{
							double distance = GetVisualLength();
							var dir = ( frustumPoints[ n ] - pos ).GetNormalize() * distance;
							var p = pos + dir * distance;
							points[ n ] = p;
						}
					}

					for( int n = 4; n < 8; n++ )
						viewport.Simple3DRenderer.AddLine( pos, points[ n ] );
					viewport.Simple3DRenderer.AddLine( points[ 4 ], points[ 5 ] );
					viewport.Simple3DRenderer.AddLine( points[ 5 ], points[ 6 ] );
					viewport.Simple3DRenderer.AddLine( points[ 6 ], points[ 7 ] );
					viewport.Simple3DRenderer.AddLine( points[ 7 ], points[ 4 ] );
				}
				break;

			case ProjectionType.Orthographic:
				{
					//!!!!
				}
				break;
			}
		}

		public Bounds GetVisualBounds()
		{
			var transform = Transform.Value;
			var pos = transform.Position;
			var rot = transform.Rotation;

			var bounds = new Bounds( pos );

			double aspectRatio = AspectRatio;
			if( aspectRatio == 0 )
				aspectRatio = 1;

			double halfWidth;
			double halfHeight;
			if( Projection.Value == ProjectionType.Perspective )
			{
				double tan = Math.Tan( FieldOfView.Value.InRadians() / 2 );
				halfWidth = tan * FarClipPlane.Value * aspectRatio;
				halfHeight = tan * FarClipPlane.Value;
			}
			else
			{
				halfWidth = Height * .5 * aspectRatio;
				halfHeight = Height * .5;
			}
			var frustum = new Frustum( true, Projection.Value, pos, rot, NearClipPlane.Value, FarClipPlane.Value, halfWidth, halfHeight );
			var frustumPoints = frustum.Points;

			switch( Projection.Value )
			{
			case ProjectionType.Perspective:
				{
					var points = new Vector3[ 8 ];
					{
						for( int n = 4; n < 8; n++ )
						{
							double distance = GetVisualLength();
							var dir = ( frustumPoints[ n ] - pos ).GetNormalize() * distance;
							var p = pos + dir * distance;
							points[ n ] = p;
						}
					}

					for( int n = 4; n < 8; n++ )
						bounds.Add( points[ n ] );
				}
				break;

			case ProjectionType.Orthographic:
				{
					//!!!!
				}
				break;
			}

			return bounds;
		}

		[Category( "Exposure" )]
		public double Ev100
		{
			get
			{
				if( ShutterSpeed.Value == 0 || Sensitivity.Value == 0 )
					return 0;
				return Math.Log( ( Aperture.Value * Aperture.Value ) / ShutterSpeed.Value * 100.0f / Sensitivity.Value, 2 );
			}
		}

		[Category( "Exposure" )]
		public double Exposure
		{
			get { return 1.0 / ( 1.2 * Math.Pow( 2.0, Ev100 ) ); }
		}

		[Category( "Exposure" )]
		public double EmissiveFactor
		{
			get
			{
				var result = Exposure * 103000;
				//var result = Exposure * 140000 * EmissiveMaterialsFactor;
				//var result = Math.Pow( 2.0, Ev100 - 2.0 ) * Exposure;
				return result;
			}
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Camera" );
		}
	}
}
