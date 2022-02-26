// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The camera that captures surroundings in the scene and passes information to the reflective surfaces.
	/// </summary>
	[SettingsCell( typeof( ReflectionProbeSettingsCell ) )]
	public class ReflectionProbe : ObjectInSpace
	{
		//Image captureTexture;
		//bool captureTextureNeedUpdate = true;

		bool processedCubemapNeedUpdate = true;
		ImageComponent processedEnvironmentCubemap;
		Vector4F[] processedIrradianceHarmonics;
		//ImageComponent processedIrradianceCubemap;

		/////////////////////////////////////////

		public enum ModeEnum
		{
			Resource,
			Capture,
		}

		/// <summary>
		/// The mode of the reflection probe.
		/// </summary>
		[DefaultValue( ModeEnum.Resource )]//Capture )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set
			{
				if( _mode.BeginSet( ref value ) )
				{
					try
					{
						ModeChanged?.Invoke( this );
						//if( Mode.Value != ModeEnum.Capture )
						//	CaptureTextureDispose();
						processedCubemapNeedUpdate = true;
					}
					finally { _mode.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<ReflectionProbe> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.Resource;// Capture;

		//!!!!может ColorMultiplier?
		///// <summary>
		///// The brightness of the reflection probe.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Range( 0, 2 )]
		//public Reference<double> Brightness
		//{
		//	get { if( _brightness.BeginGet() ) Brightness = _brightness.Get( this ); return _brightness.value; }
		//	set { if( _brightness.BeginSet( ref value ) ) { try { BrightnessChanged?.Invoke( this ); } finally { _brightness.EndSet(); } } }
		//}
		//public event Action<ReflectionProbe> BrightnessChanged;
		//ReferenceField<double> _brightness = 1;

		/// <summary>
		/// The cubemap texture of reflection data used by the probe.
		/// </summary>
		[Category( "Resource" )]
		[DefaultValueReference( @"Content\Environments\Base\Forest.image" )]
		public Reference<ImageComponent> Cubemap
		{
			get { if( _cubemap.BeginGet() ) Cubemap = _cubemap.Get( this ); return _cubemap.value; }
			set
			{
				if( _cubemap.BeginSet( ref value ) )
				{
					try
					{
						CubemapChanged?.Invoke( this );
						processedCubemapNeedUpdate = true;
					}
					finally { _cubemap.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Cubemap"/> property value changes.</summary>
		public event Action<ReflectionProbe> CubemapChanged;
		ReferenceField<ImageComponent> _cubemap = new Reference<ImageComponent>( null, @"Content\Environments\Base\Forest.image" );

		/// <summary>
		/// The horizontal rotation of the cubemap.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 360 )]
		public Reference<Degree> Rotation
		{
			get { if( _rotation.BeginGet() ) Rotation = _rotation.Get( this ); return _rotation.value; }
			set { if( _rotation.BeginSet( ref value ) ) { try { RotationChanged?.Invoke( this ); } finally { _rotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Rotation"/> property value changes.</summary>
		public event Action<ReflectionProbe> RotationChanged;
		ReferenceField<Degree> _rotation;

		/// <summary>
		/// A cubemap color multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[ApplicableRangeColorValuePower( 0, 4 )]
		[ColorValueNoAlpha]
		public Reference<ColorValuePowered> Multiplier
		{
			get { if( _multiplier.BeginGet() ) Multiplier = _multiplier.Get( this ); return _multiplier.value; }
			set { if( _multiplier.BeginSet( ref value ) ) { try { MultiplierChanged?.Invoke( this ); } finally { _multiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Multiplier"/> property value changes.</summary>
		public event Action<ReflectionProbe> MultiplierChanged;
		ReferenceField<ColorValuePowered> _multiplier = new ColorValuePowered( 1, 1, 1 );

		///// <summary>
		///// The cubemap texture of irradiance data used by the probe.
		///// </summary>
		//[Category( "Resource" )]
		//public Reference<Image> CubemapIrradiance
		//{
		//	get { if( _cubemapIrradiance.BeginGet() ) CubemapIrradiance = _cubemapIrradiance.Get( this ); return _cubemapIrradiance.value; }
		//	set { if( _cubemapIrradiance.BeginSet( ref value ) ) { try { CubemapIrradianceChanged?.Invoke( this ); } finally { _cubemapIrradiance.EndSet(); } } }
		//}
		//public event Action<ReflectionProbe> CubemapIrradianceChanged;
		//ReferenceField<Image> _cubemapIrradiance;

		//!!!!remove
		///// <summary>
		///// Whether to capture and pass the information in real-time.
		///// </summary>
		//[DefaultValue( false )]
		//[Category( "Capture" )]
		//public Reference<bool> Realtime
		//{
		//	get { if( _realtime.BeginGet() ) Realtime = _realtime.Get( this ); return _realtime.value; }
		//	set { if( _realtime.BeginSet( ref value ) ) { try { RealtimeChanged?.Invoke( this ); } finally { _realtime.EndSet(); } } }
		//}
		//public event Action<ReflectionProbe> RealtimeChanged;
		//ReferenceField<bool> _realtime = false;

		public enum ResolutionEnum
		{
			_1,
			_2,
			_4,
			_8,
			_16,
			_32,
			_64,
			_128,
			_256,
			_512,
			_1024,
			_2048,
			//_4096,
			//_8192,
			//_16384,//!!!!как проверять хватит ли памяти
		}

		/// <summary>
		/// The resolution of the capture.
		/// </summary>
		[DefaultValue( ResolutionEnum._256 )]
		[Category( "Capture" )]
		public Reference<ResolutionEnum> Resolution
		{
			get { if( _resolution.BeginGet() ) Resolution = _resolution.Get( this ); return _resolution.value; }
			set
			{
				if( _resolution.BeginSet( ref value ) )
				{
					try
					{
						ResolutionChanged?.Invoke( this );
						//CaptureTextureDispose();
					}
					finally { _resolution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Resolution"/> property value changes.</summary>
		public event Action<ReflectionProbe> ResolutionChanged;
		ReferenceField<ResolutionEnum> _resolution = ResolutionEnum._256;

		/// <summary>
		/// Whether the high dynamic range is enabled. For Auto mode HDR is disabled on limited devices (mobile).
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "Capture" )]
		public Reference<AutoTrueFalse> HDR
		{
			get { if( _hdr.BeginGet() ) HDR = _hdr.Get( this ); return _hdr.value; }
			set
			{
				if( _hdr.BeginSet( ref value ) )
				{
					try
					{
						HDRChanged?.Invoke( this );
						//CaptureTextureDispose();
					}
					finally { _hdr.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="HDR"/> property value changes.</summary>
		public event Action<ReflectionProbe> HDRChanged;
		ReferenceField<AutoTrueFalse> _hdr = AutoTrueFalse.Auto;

		//!!!!remove
		///// <summary>
		///// Whether to generate a mip levels.
		///// </summary>
		//[DefaultValue( false )]
		//[Category( "Capture" )]
		//public Reference<bool> Mipmaps
		//{
		//	get { if( _mipmaps.BeginGet() ) Mipmaps = _mipmaps.Get( this ); return _mipmaps.value; }
		//	set { if( _mipmaps.BeginSet( ref value ) ) { try { MipmapsChanged?.Invoke( this ); } finally { _mipmaps.EndSet(); } } }
		//}
		//public event Action<ReflectionProbe> MipmapsChanged;
		//ReferenceField<bool> _mipmaps = false;

		/// <summary>
		/// The minimum distance of the reflection probe.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Category( "Capture" )]
		[Range( 0.01, 1 )]
		public Reference<double> NearClipPlane
		{
			get { if( _nearClipPlane.BeginGet() ) NearClipPlane = _nearClipPlane.Get( this ); return _nearClipPlane.value; }
			set { if( _nearClipPlane.BeginSet( ref value ) ) { try { NearClipPlaneChanged?.Invoke( this ); } finally { _nearClipPlane.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="NearClipPlane"/> property value changes.</summary>
		public event Action<ReflectionProbe> NearClipPlaneChanged;
		ReferenceField<double> _nearClipPlane = 0.1;

		/// <summary>
		/// The maximum distance of the reflection probe.
		/// </summary>
		[DefaultValue( 100.0 )]
		[Category( "Capture" )]
		[Range( 10, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> FarClipPlane
		{
			get { if( _farClipPlane.BeginGet() ) FarClipPlane = _farClipPlane.Get( this ); return _farClipPlane.value; }
			set { if( _farClipPlane.BeginSet( ref value ) ) { try { FarClipPlaneChanged?.Invoke( this ); } finally { _farClipPlane.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FarClipPlane"/> property value changes.</summary>
		public event Action<ReflectionProbe> FarClipPlaneChanged;
		ReferenceField<double> _farClipPlane = 100;

		/// <summary>
		/// Whether to render sky of the scene.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Capture" )]
		public Reference<bool> RenderSky
		{
			get { if( _renderSky.BeginGet() ) RenderSky = _renderSky.Get( this ); return _renderSky.value; }
			set { if( _renderSky.BeginSet( ref value ) ) { try { RenderSkyChanged?.Invoke( this ); } finally { _renderSky.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RenderSky"/> property value changes.</summary>
		public event Action<ReflectionProbe> RenderSkyChanged;
		ReferenceField<bool> _renderSky = true;

		//!!!!
		//capture settings:
		//Resolution - в анрыле это общая настройка двига. можно по ссылке указать
		//Shadow Distance
		//Background Color
		//Use Occlusion Culling
		//из CubemapZone что там?

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Cubemap ):
					//case nameof( CubemapIrradiance ):
					if( Mode.Value != ModeEnum.Resource )
						skip = true;
					break;

				case nameof( Resolution ):
				case nameof( HDR ):
				//case nameof( Mipmaps ):
				//case nameof( Realtime ):
				case nameof( NearClipPlane ):
				case nameof( FarClipPlane ):
				case nameof( RenderSky ):
					if( Mode.Value != ModeEnum.Capture )
						skip = true;
					break;
				}
			}
		}

		protected override void OnEnabled()
		{
			base.OnEnabled();

			processedCubemapNeedUpdate = true;
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var tr = Transform.Value;
			var attenuationFar = tr.Scale.MaxComponent();
			newBounds = new SpaceBounds( new Sphere( tr.Position, attenuationFar ) );
		}

		//protected override void OnTransformChanged()
		//{
		//	base.OnTransformChanged();

		//	captureTextureNeedUpdate = true;
		//}

		[Browsable( false )]
		public Sphere Sphere
		{
			get { return SpaceBounds.CalculatedBoundingSphere; }
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() || !ParentScene.DisplayLabels )
				return false;
			//if( !ParentScene.GetShowDevelopmentDataInThisApplication() || !ParentScene.ShowReflectionProbes )
			//	return false;
			return base.OnEnabledSelectionByCursor();
		}

		void DebugDrawBorder( Viewport viewport )
		{
			var sphere = Sphere;
			var pos = sphere.Center;
			var r = sphere.Radius;

			const double angleStep = Math.PI / 32;
			for( double angle = 0; angle < Math.PI * 2 - angleStep * .5; angle += angleStep )
			{
				double p1sin = Math.Sin( angle );
				double p1cos = Math.Cos( angle );
				double p2sin = Math.Sin( angle + angleStep );
				double p2cos = Math.Cos( angle + angleStep );

				//может больше линий рисовать. еще под 45 градусов

				viewport.Simple3DRenderer.AddLine( pos + ( new Vector3( p1cos, p1sin, 0 ) * r ), pos + ( new Vector3( p2cos, p2sin, 0 ) * r ) );
				viewport.Simple3DRenderer.AddLine( pos + ( new Vector3( 0, p1cos, p1sin ) * r ), pos + ( new Vector3( 0, p2cos, p2sin ) * r ) );
				viewport.Simple3DRenderer.AddLine( pos + ( new Vector3( p1cos, 0, p1sin ) * r ), pos + ( new Vector3( p2cos, 0, p2sin ) * r ) );
			}
		}

		public void DebugDraw( Viewport viewport )
		{
			var sphere = Sphere;
			var pos = sphere.Center;
			var r = sphere.Radius;

			DebugDrawBorder( viewport );

			viewport.Simple3DRenderer.AddLine( pos - new Vector3( r, 0, 0 ), pos + new Vector3( r, 0, 0 ) );
			viewport.Simple3DRenderer.AddLine( pos - new Vector3( 0, r, 0 ), pos + new Vector3( 0, r, 0 ) );
			viewport.Simple3DRenderer.AddLine( pos - new Vector3( 0, 0, r ), pos + new Vector3( 0, 0, r ) );
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			UpdateProcessedCubemaps();
		}

		public void GetRotationQuaternion( out QuaternionF result )
		{
			QuaternionF.FromRotateByZ( Rotation.Value.InRadians().ToRadianF(), out result );
		}

		//public void GetRotationMatrix( out Matrix3F result )
		//{
		//	Matrix3.FromRotateByZ( Rotation.Value.InRadians() ).ToMatrix3F( out result );
		//}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				//UpdateProcessedCubemaps();

				var sphere = Sphere;
				if( sphere.Radius > 0 )
				{
					var item = new RenderingPipeline.RenderSceneData.ReflectionProbeItem();
					item.Creator = this;
					item.BoundingBox = SpaceBounds.CalculatedBoundingBox;
					//item.BoundingSphere = SpaceBounds.CalculatedBoundingSphere;
					item.Sphere = sphere;

					//if( Mode.Value == ModeEnum.Resource )
					//{
					if( processedEnvironmentCubemap != null )
						item.CubemapEnvironment = processedEnvironmentCubemap;
					else
						item.CubemapEnvironment = Cubemap;

					item.HarmonicsIrradiance = processedIrradianceHarmonics;
					//item.CubemapIrradiance = processedIrradianceCubemap;

					GetRotationQuaternion( out item.Rotation );
					item.Multiplier = Multiplier.Value.ToVector3F();

					//item.CubemapEnvironment = Cubemap;
					//item.CubemapIrradiance = CubemapIrradiance;
					//}
					//else
					//{
					//	//!!!!IBL
					//	item.CubemapEnvironment = CaptureTexture;
					//}

					context.FrameData.RenderSceneData.ReflectionProbes.Add( item );
				}

				{
					var context2 = context.ObjectInSpaceRenderingContext;

					bool show = ( ParentScene.GetDisplayDevelopmentDataInThisApplication() && ParentScene.DisplayReflectionProbes ) || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
					if( show )
					{
						if( context2.displayReflectionProbesCounter < context2.displayReflectionProbesMax )
						{
							context2.displayReflectionProbesCounter++;

							ColorValue color;
							if( context2.selectedObjects.Contains( this ) )
								color = ProjectSettings.Get.General.SelectedColor;
							else if( context2.canSelectObjects.Contains( this ) )
								color = ProjectSettings.Get.General.CanSelectColor;
							else
								color = ProjectSettings.Get.General.SceneShowReflectionProbeColor;

							var viewport = context.Owner;
							if( viewport.Simple3DRenderer != null )
							{
								viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.General.HiddenByOtherObjectsColorMultiplier );
								DebugDraw( viewport );
								//viewport.Simple3DRenderer.AddSphere( Transform.Value.ToMat4(), 0.5, 32 );
							}
						}
					}
					//if( !show )
					//	context.disableShowingLabelForThisObject = true;
				}
			}
		}

		//public bool Contains( Vec3 point )
		//{
		//	var scale = Transform.Value.Scale;
		//	var radius = Math.Max( scale.X, Math.Max( scale.Y, scale.Z ) ) * 0.5;
		//	return new Sphere( Transform.Value.Position, radius ).Contains( point );
		//}

		//public virtual void GetOutputTextures( out Texture texture, out Texture textureIBL )
		//{
		//	texture = null;
		//	textureIBL = null;

		//	//!!!!пока так

		//	if( SourceType.Value == SourceTypeEnum.SpecifiedCubemap )
		//	{
		//		texture = Cubemap;
		//		textureIBL = CubemapIBL;
		//	}
		//}

		//protected override void OnDisabled()
		//{
		//	CaptureTextureDispose();
		//	base.OnDisabled();
		//}

		//void CaptureTextureUpdate()
		//{
		//	if( Mode.Value == ModeEnum.Capture )
		//	{
		//		var resolution = Resolution.Value;
		//		var hdr = HDR.Value;
		//		var mipmaps = Mipmaps.Value;

		//		if( captureTexture != null && captureTexture.Disposed )
		//			captureTexture = null;
		//		if( captureTexture == null )
		//		{
		//			var size = int.Parse( resolution.ToString().Replace( "_", "" ) );
		//			PixelFormat format = hdr ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8;

		//			var texture = ComponentUtility.CreateComponent<Image>( null, true, false );
		//			captureTexture = texture;
		//			texture.CreateType = Image.TypeEnum.Cube;
		//			texture.CreateSize = new Vector2I( size, size );
		//			texture.CreateMipmaps = mipmaps;
		//			//texture.CreateMipmaps = true;
		//			//texture.CreateArrayLayers = arrayLayers;
		//			texture.CreateFormat = format;
		//			texture.CreateUsage = Image.Usages.RenderTarget;
		//			texture.CreateFSAA = 0;// fsaaLevel;
		//			texture.Enabled = true;

		//			//!!!!

		//			//int faces = type == Image.TypeEnum.Cube ? 6 : arrayLayers;

		//			//int numMips;
		//			//if( mipmaps )
		//			//{
		//			//	int max = size.MaxComponent();
		//			//	float kInvLogNat2 = 1.4426950408889634073599246810019f;
		//			//	numMips = 1 + (int)( Math.Log( size.MaxComponent() ) * kInvLogNat2 );
		//			//}
		//			//else
		//			//	numMips = 1;

		//			for( int face = 0; face < 6; face++ )
		//			{
		//				//!!!!
		//				var mip = 0;
		//				//for( int mip = 0; mip < numMips; mip++ )
		//				//{

		//				var renderTexture = CaptureTexture.Result.GetRenderTarget( mip, face );
		//				//!!!!
		//				var viewport = renderTexture.AddViewport( true, false );
		//				//var viewport = renderTexture.AddViewport( false, false );
		//				viewport.Mode = Viewport.ModeEnum.ReflectionProbeCubemap;
		//				viewport.AttachedScene = ParentScene;

		//				//!!!!надо ли чистить что-то? лишние таржеты чтобы не висели

		//				//}
		//				//!!!!что-то еще?
		//			}

		//			//captureTextureNeedUpdate = true;
		//		}
		//	}
		//	else
		//		CaptureTextureDispose();

		//	//!!!!!как проверять ошибки создания текстур? везде так
		//	//if( texture == null )
		//	//{
		//	//	//!!!!!
		//	//	Log.Fatal( "ViewportRenderingPipeline: RenderTarget_Alloc: Unable to create texture." );
		//	//	return null;
		//	//}

		//	//int faces = type == Image.TypeEnum.Cube ? 6 : arrayLayers;

		//	//int numMips;
		//	//if( mipmaps )
		//	//{
		//	//	int max = size.MaxComponent();
		//	//	float kInvLogNat2 = 1.4426950408889634073599246810019f;
		//	//	numMips = 1 + (int)( Math.Log( size.MaxComponent() ) * kInvLogNat2 );
		//	//}
		//	//else
		//	//	numMips = 1;

		//	//for( int face = 0; face < faces; face++ )
		//	//{
		//	//	for( int mip = 0; mip < numMips; mip++ )
		//	//	{
		//	//		RenderTexture renderTexture = texture.Result.GetRenderTarget( mip, face );
		//	//		var viewport = renderTexture.AddViewport( false, false );

		//	//		viewport.RenderingPipelineCreate();
		//	//		viewport.RenderingPipelineCreated.UseRenderTargets = false;
		//	//	}
		//	//	//!!!!что-то еще?
		//	//}
		//}

		//void CaptureTextureDispose()
		//{
		//	captureTexture?.Dispose();
		//	captureTexture = null;
		//}

		//[Browsable( false )]
		//public Image CaptureTexture
		//{
		//	get { return captureTexture; }
		//}

		string GetDestVirtualFileName()
		{
			string name = GetPathFromRoot();
			foreach( char c in new string( Path.GetInvalidFileNameChars() ) + new string( Path.GetInvalidPathChars() ) )
				name = name.Replace( c.ToString(), "_" );
			name = name.Replace( " ", "_" );
			return PathUtility.Combine( ComponentUtility.GetOwnedFileNameOfComponent( this ) + "_Files", name + ".hdr" );
		}

		public bool GetHDR()
		{
			var hdr = HDR.Value;
			if( hdr == AutoTrueFalse.Auto )
				hdr = SystemSettings.LimitedDevice ? AutoTrueFalse.False : AutoTrueFalse.True;
			return hdr == AutoTrueFalse.True;
		}

		public void UpdateCaptureCubemap()
		{
			if( Mode.Value != ModeEnum.Capture )
				return;

			ImageComponent texture = null;
			ImageComponent textureRead = null;

			try
			{
				//create

				var resolution = Resolution.Value;
				var hdr = GetHDR();

				var size = int.Parse( resolution.ToString().Replace( "_", "" ) );
				//!!!!16 бит достаточно, тогда нужно поддержку для Image2D
				PixelFormat format = hdr ? PixelFormat.Float32RGBA : PixelFormat.A8R8G8B8;
				//PixelFormat format = hdr ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8;

				texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
				texture.CreateType = ImageComponent.TypeEnum._2D;
				texture.CreateSize = new Vector2I( size, size );
				texture.CreateMipmaps = false;
				texture.CreateFormat = format;
				texture.CreateUsage = ImageComponent.Usages.RenderTarget;
				texture.CreateFSAA = 0;
				texture.Enabled = true;

				var renderTexture = texture.Result.GetRenderTarget( 0, 0 );
				//!!!!
				var viewport = renderTexture.AddViewport( true, false );
				//var viewport = renderTexture.AddViewport( false, false );
				viewport.Mode = Viewport.ModeEnum.ReflectionProbeCubemap;
				viewport.AttachedScene = ParentScene;

				textureRead = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
				textureRead.CreateType = ImageComponent.TypeEnum._2D;
				textureRead.CreateSize = new Vector2I( size, size );
				textureRead.CreateMipmaps = false;
				textureRead.CreateFormat = format;
				textureRead.CreateUsage = ImageComponent.Usages.ReadBack | ImageComponent.Usages.BlitDestination;
				textureRead.CreateFSAA = 0;
				textureRead.Enabled = true;

				//render
				var image2D = new ImageUtility.Image2D( PixelFormat.Float32RGB, new Vector2I( size * 4, size * 3 ) );

				var position = Transform.Value.Position;

				for( int face = 0; face < 6; face++ )
				{
					Vector3 dir = Vector3.Zero;
					Vector3 up = Vector3.Zero;

					//flipped
					switch( face )
					{
					case 0: dir = -Vector3.YAxis; up = Vector3.ZAxis; break;
					case 1: dir = Vector3.YAxis; up = Vector3.ZAxis; break;
					case 2: dir = Vector3.ZAxis; up = -Vector3.XAxis; break;
					case 3: dir = -Vector3.ZAxis; up = Vector3.XAxis; break;
					case 4: dir = Vector3.XAxis; up = Vector3.ZAxis; break;
					case 5: dir = -Vector3.XAxis; up = Vector3.ZAxis; break;
					}

					//!!!!renderingPipelineOverride

					var scene = ParentScene;
					var cameraEditor = scene.Mode.Value == Scene.ModeEnum._2D ? scene.CameraEditor2D.Value : scene.CameraEditor.Value;
					if( cameraEditor == null )
						cameraEditor = new Camera();

					var cameraSettings = new Viewport.CameraSettingsClass( viewport, 1, 90, NearClipPlane.Value, FarClipPlane.Value, position, dir, up, ProjectionType.Perspective, 1, cameraEditor.Exposure, cameraEditor.EmissiveFactor, renderSky: RenderSky );

					viewport.Update( true, cameraSettings );

					//clear temp data
					viewport.RenderingContext.MultiRenderTarget_DestroyAll();
					viewport.RenderingContext.DynamicTexture_DestroyAll();

					texture.Result.GetNativeObject( true ).BlitTo( viewport.RenderingContext.CurrentViewNumber, textureRead.Result.GetNativeObject( true ), 0, 0 );

					//get data
					var totalBytes = PixelFormatUtility.GetNumElemBytes( format ) * size * size;
					var data = new byte[ totalBytes ];
					unsafe
					{
						fixed( byte* pBytes = data )
						{
							var demandedFrame = textureRead.Result.GetNativeObject( true ).Read( (IntPtr)pBytes, 0 );

							while( RenderingSystem.CallBgfxFrame() < demandedFrame )
							{
							}
						}
					}

					Vector2I index = Vector2I.Zero;
					switch( face )
					{
					case 1: index = new Vector2I( 2, 1 ); break;
					case 0: index = new Vector2I( 0, 1 ); break;
					case 2: index = new Vector2I( 1, 0 ); break;
					case 3: index = new Vector2I( 1, 2 ); break;
					case 4: index = new Vector2I( 1, 1 ); break;
					case 5: index = new Vector2I( 3, 1 ); break;
					}
					//switch( face )
					//{
					//case 0: index = new Vector2I( 2, 1 ); break;
					//case 1: index = new Vector2I( 0, 1 ); break;
					//case 2: index = new Vector2I( 1, 0 ); break;
					//case 3: index = new Vector2I( 1, 2 ); break;
					//case 4: index = new Vector2I( 1, 1 ); break;
					//case 5: index = new Vector2I( 3, 1 ); break;
					//}

					var faceImage = new ImageUtility.Image2D( format, new Vector2I( size, size ), data );

					//flip by X
					var faceImageFlip = new ImageUtility.Image2D( format, new Vector2I( size, size ) );
					for( int y = 0; y < size; y++ )
					{
						for( int x = 0; x < size; x++ )
						{
							var pixel = faceImage.GetPixel( new Vector2I( x, y ) );
							faceImageFlip.SetPixel( new Vector2I( size - 1 - x, y ), pixel );
						}
					}

					image2D.Blit( index * size, faceImageFlip );
				}

				//reset alpha channel
				for( int y = 0; y < image2D.Size.Y; y++ )
				{
					for( int x = 0; x < image2D.Size.X; x++ )
					{
						var pixel = image2D.GetPixel( new Vector2I( x, y ) );
						pixel.W = 1.0f;
						image2D.SetPixel( new Vector2I( x, y ), pixel );
					}
				}

				var destRealFileName = VirtualPathUtility.GetRealPathByVirtual( GetDestVirtualFileName() );

				if( !Directory.Exists( Path.GetDirectoryName( destRealFileName ) ) )
					Directory.CreateDirectory( Path.GetDirectoryName( destRealFileName ) );

				if( !ImageUtility.Save( destRealFileName, image2D.Data, image2D.Size, 1, image2D.Format, 1, 0, out var error ) )
					throw new Exception( error );

				//delete Gen files
				var names = new string[] { "_Gen.info", "_GenEnv.dds", "_GenIrr.dds" };
				foreach( var name in names )
				{
					var fileName2 = VirtualPathUtility.GetRealPathByVirtual( destRealFileName + name );
					if( File.Exists( fileName2 ) )
						File.Delete( fileName2 );
				}

			}
			catch( Exception e )
			{
				Log.Error( e.Message );
			}
			finally
			{
				texture?.Dispose();
				textureRead?.Dispose();
			}

			processedCubemapNeedUpdate = true;
		}

		//public void Update( bool forceUpdate )
		//{
		//	CaptureTextureUpdate();

		//	if( CaptureTexture != null && ( Realtime || captureTextureNeedUpdate || forceUpdate ) )
		//	{
		//		captureTextureNeedUpdate = false;

		//		var position = Transform.Value.Position;

		//		for( int face = 0; face < 6; face++ )
		//		{
		//			int mip = 0;
		//			var renderTexture = CaptureTexture.Result.GetRenderTarget( mip, face );
		//			if( renderTexture != null )
		//			{
		//				var viewport = renderTexture.Viewports[ 0 ];

		//				Vector3 dir = Vector3.Zero;
		//				Vector3 up = Vector3.Zero;

		//				//flipped
		//				switch( face )
		//				{
		//				case 0: dir = -Vector3.YAxis; up = Vector3.ZAxis; break;
		//				case 1: dir = Vector3.YAxis; up = Vector3.ZAxis; break;
		//				case 2: dir = Vector3.ZAxis; up = -Vector3.XAxis; break;
		//				case 3: dir = -Vector3.ZAxis; up = Vector3.XAxis; break;
		//				case 4: dir = Vector3.XAxis; up = Vector3.ZAxis; break;
		//				case 5: dir = -Vector3.XAxis; up = Vector3.ZAxis; break;
		//				}

		//				//!!!!
		//				var cameraSettings = new Viewport.CameraSettingsClass( viewport, 1, 90, NearClipPlane.Value, FarClipPlane.Value, position, dir, up, ProjectionType.Perspective, 1, 1, 1 );

		//				viewport.Update( true, cameraSettings );

		//				//clear temp data
		//				if( !Realtime.Value )
		//				{
		//					viewport.RenderingContext.MultiRenderTarget_DestroyAll();
		//					viewport.RenderingContext.RenderTarget_DestroyAll();
		//				}
		//			}
		//		}
		//	}
		//}

		void UpdateProcessedCubemaps()
		{
			if( processedEnvironmentCubemap != null && processedEnvironmentCubemap.Disposed )
			{
				processedEnvironmentCubemap = null;
				processedCubemapNeedUpdate = true;
			}
			//if( processedIrradianceCubemap != null && processedIrradianceCubemap.Disposed )
			//{
			//	processedIrradianceCubemap = null;
			//	processedCubemapNeedUpdate = true;
			//}

			if( processedCubemapNeedUpdate )//&& AllowProcessEnvironmentCubemap )
			{
				processedCubemapNeedUpdate = false;
				ProcessCubemaps();
			}
		}

		void ProcessCubemaps()
		{
			processedEnvironmentCubemap = null;
			processedIrradianceHarmonics = null;
			//processedIrradianceCubemap = null;

			string sourceFileName;
			if( Mode.Value == ModeEnum.Resource )
			{
				sourceFileName = Cubemap.Value?.LoadFile.Value?.ResourceName;
				if( string.IsNullOrEmpty( sourceFileName ) )
				{
					var getByReference = Cubemap.GetByReference;
					if( !string.IsNullOrEmpty( getByReference ) )
					{
						try
						{
							if( Path.GetExtension( getByReference ) == ".image" )
								sourceFileName = getByReference;
						}
						catch { }
					}
				}
			}
			else
			{
				sourceFileName = GetDestVirtualFileName();
			}

			if( !string.IsNullOrEmpty( sourceFileName ) && VirtualFile.Exists( sourceFileName ) )
			{
				bool skip = false;
				if( sourceFileName.Length > 11 )
				{
					var s = sourceFileName.Substring( sourceFileName.Length - 11 );
					if( s == "_GenEnv.dds" || s == "_GenIrr.dds" )
						skip = true;
				}

				if( !skip )
				{
					if( !CubemapProcessing.GetOrGenerate( sourceFileName, false, 0, out var envVirtualFileName, out var irradiance/*irrVirtualFileName*/, out var error ) )
					{
						Log.Error( error );
						return;
					}

					if( VirtualFile.Exists( envVirtualFileName ) )
						processedEnvironmentCubemap = ResourceManager.LoadResource<ImageComponent>( envVirtualFileName );
					processedIrradianceHarmonics = irradiance;
					//if( VirtualFile.Exists( irrVirtualFileName ) )
					//	processedIrradianceCubemap = ResourceManager.LoadResource<ImageComponent>( irrVirtualFileName );
				}
			}
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "ReflectionProbe" );
		}
	}
}
