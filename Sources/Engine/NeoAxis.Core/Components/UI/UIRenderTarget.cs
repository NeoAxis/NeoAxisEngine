// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// UI control with a render target.
	/// </summary>
	public class UIRenderTarget : UIControl
	{
		ImageComponent createdImage;
		Vector2I createdForSize;
		bool createdForHDR;

		Viewport createdViewport;
		//UIControl createdControl;

		bool viewportDuringTick;
		bool viewportDuringUpdate;

		//bool uiControlUpdating;

		Scene createdScene;
		Scene createdSceneSource;

		//double lastVisibleTime;

		/////////////////////////////////////////

		///// <summary>
		///// The size of a render target.
		///// </summary>
		//[DefaultValue( "512 512" )]
		//public Reference<Vector2I> RenderTargetSize
		//{
		//	get { if( _renderTargetSize.BeginGet() ) RenderTargetSize = _renderTargetSize.Get( this ); return _renderTargetSize.value; }
		//	set { if( _renderTargetSize.BeginSet( ref value ) ) { try { RenderTargetSizeChanged?.Invoke( this ); } finally { _renderTargetSize.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RenderTargetSize"/> property value changes.</summary>
		//public event Action<UIRenderTarget> RenderTargetSizeChanged;
		//ReferenceField<Vector2I> _renderTargetSize = new Vector2I( 512, 512 );

		/// <summary>
		/// Whether the high dynamic range is enabled. For Auto mode HDR is disabled on limited devices (mobile).
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		public Reference<AutoTrueFalse> HDR
		{
			get { if( _hdr.BeginGet() ) HDR = _hdr.Get( this ); return _hdr.value; }
			set { if( _hdr.BeginSet( ref value ) ) { try { HDRChanged?.Invoke( this ); } finally { _hdr.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HDR"/> property value changes.</summary>
		public event Action<UIRenderTarget> HDRChanged;
		ReferenceField<AutoTrueFalse> _hdr = AutoTrueFalse.Auto;

		///// <summary>
		///// The aspect ratio of the camera.
		///// </summary>
		//[DefaultValue( 1.3333 )]
		//public Reference<double> AspectRatio
		//{
		//	get { if( _aspectRatio.BeginGet() ) AspectRatio = _aspectRatio.Get( this ); return _aspectRatio.value; }
		//	set { if( _aspectRatio.BeginSet( ref value ) ) { try { AspectRatioChanged?.Invoke( this ); } finally { _aspectRatio.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AspectRatio"/> property value changes.</summary>
		//public event Action<UIRenderTarget> AspectRatioChanged;
		//ReferenceField<double> _aspectRatio = 1.3333;

		/// <summary>
		/// Whether to enable auto update. For manual update RenderTargetUpdate method is used.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AutoUpdate
		{
			get { if( _autoUpdate.BeginGet() ) AutoUpdate = _autoUpdate.Get( this ); return _autoUpdate.value; }
			set { if( _autoUpdate.BeginSet( ref value ) ) { try { AutoUpdateChanged?.Invoke( this ); } finally { _autoUpdate.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AutoUpdate"/> property value changes.</summary>
		public event Action<UIRenderTarget> AutoUpdateChanged;
		ReferenceField<bool> _autoUpdate = true;

		///// <summary>
		///// The background color of the control.
		///// </summary>
		//[DefaultValue( "0.4 0.4 0.4" )]
		//public Reference<ColorValue> BackgroundColor
		//{
		//	get { if( _backgroundColor.BeginGet() ) BackgroundColor = _backgroundColor.Get( this ); return _backgroundColor.value; }
		//	set { if( _backgroundColor.BeginSet( ref value ) ) { try { BackgroundColorChanged?.Invoke( this ); } finally { _backgroundColor.EndSet(); } } }
		//}
		//public event Action<UIRenderTarget> BackgroundColorChanged;
		//ReferenceField<ColorValue> _backgroundColor = new ColorValue( 0.4, 0.4, 0.4 );

		/// <summary>
		/// Whether to attach a scene to the render target.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Scene" )]
		public Reference<bool> DisplayScene
		{
			get { if( _displayScene.BeginGet() ) DisplayScene = _displayScene.Get( this ); return _displayScene.value; }
			set { if( _displayScene.BeginSet( ref value ) ) { try { DisplaySceneChanged?.Invoke( this ); UpdateAttachedScene(); } finally { _displayScene.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayScene"/> property value changes.</summary>
		public event Action<UIRenderTarget> DisplaySceneChanged;
		ReferenceField<bool> _displayScene = true;

		/// <summary>
		/// The scene to display. Set 'null' to use current scene.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Scene" )]
		public Reference<Scene> Scene
		{
			get { if( _scene.BeginGet() ) Scene = _scene.Get( this ); return _scene.value; }
			set { if( _scene.BeginSet( ref value ) ) { try { SceneChanged?.Invoke( this ); UpdateAttachedScene(); } finally { _scene.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Scene"/> property value changes.</summary>
		public event Action<UIRenderTarget> SceneChanged;
		ReferenceField<Scene> _scene = null;

		/// <summary>
		/// The camera to use for displaying scene.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Scene" )]
		public Reference<Camera> Camera
		{
			get { if( _camera.BeginGet() ) Camera = _camera.Get( this ); return _camera.value; }
			set { if( _camera.BeginSet( ref value ) ) { try { CameraChanged?.Invoke( this ); } finally { _camera.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Camera"/> property value changes.</summary>
		public event Action<UIRenderTarget> CameraChanged;
		ReferenceField<Camera> _camera = null;

		/// <summary>
		/// The camera to use for displaying scene, specified by name.
		/// </summary>
		[DefaultValue( "" )]
		[Category( "Scene" )]
		public Reference<string> CameraByName
		{
			get { if( _cameraByName.BeginGet() ) CameraByName = _cameraByName.Get( this ); return _cameraByName.value; }
			set { if( _cameraByName.BeginSet( ref value ) ) { try { CameraByNameChanged?.Invoke( this ); } finally { _cameraByName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraByName"/> property value changes.</summary>
		public event Action<UIRenderTarget> CameraByNameChanged;
		ReferenceField<string> _cameraByName = "";

		///// <summary>
		///// The UI control to use.
		///// </summary>
		//[DefaultValue( null )]
		//[Category( "GUI" )]
		//[DisplayName( "UI Screen" )]
		//public Reference<UIControl> UIControl
		//{
		//	get { if( _uiControl.BeginGet() ) UIControl = _uiControl.Get( this ); return _uiControl.value; }
		//	set { if( _uiControl.BeginSet( ref value ) ) { try { UIControlChanged?.Invoke( this ); UIControlUpdate(); } finally { _uiControl.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="UIControl"/> property value changes.</summary>
		//public event Action<UIRenderTarget> UIControlChanged;
		//ReferenceField<UIControl> _uiControl = null;

		///// <summary>
		///// Whether the object is read-only.
		///// </summary>
		//[DefaultValue( false )]
		//[Category( "GUI" )]
		//public Reference<bool> ReadOnly
		//{
		//	get { if( _readOnly.BeginGet() ) ReadOnly = _readOnly.Get( this ); return _readOnly.value; }
		//	set { if( _readOnly.BeginSet( ref value ) ) { try { ReadOnlyChanged?.Invoke( this ); } finally { _readOnly.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ReadOnly"/> property value changes.</summary>
		//public event Action<UIRenderTarget> ReadOnlyChanged;
		//ReferenceField<bool> _readOnly = false;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Scene ):
				case nameof( Camera ):
				case nameof( CameraByName ):
					if( !DisplayScene )
						skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( ParentContainer != null && ParentContainer.Viewport != null )
			{
				if( EnabledInHierarchy )
					ParentContainer.Viewport.UpdateBegin += Viewport_UpdateBegin;
				else
					ParentContainer.Viewport.UpdateBegin -= Viewport_UpdateBegin;
			}

			if( !EnabledInHierarchy )
			{
				DestroyRenderTarget();
				SceneDestroy();
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			ViewportPerformTick( delta );
		}

		/////////////////////////////////////////

		protected virtual Camera OnGetCamera()
		{
			//default behavior
			if( createdViewport != null && createdViewport.AttachedScene != null )
			{
				var scene = createdViewport.AttachedScene;

				//Camera property
				{
					var camera = Camera.Value;
					if( camera != null )
						return camera;
				}

				//CameraByName property
				if( !string.IsNullOrEmpty( CameraByName ) )
				{
					var camera = scene.GetComponent( CameraByName, onlyEnabledInHierarchy: true ) as Camera;
					if( camera != null )
						return camera;
				}

				//use default camera of the scene
				{
					var camera = scene.CameraDefault.Value;
					if( camera == null )
						camera = scene.Mode.Value == NeoAxis.Scene.ModeEnum._2D ? scene.CameraEditor2D : scene.CameraEditor;
					if( camera != null )
						return camera;
				}
			}

			return null;
		}

		public delegate void GetCameraEventDelegate( UIRenderTarget sender, ref Camera camera );
		public event GetCameraEventDelegate GetCameraEvent;

		public Camera GetCamera()
		{
			var result = OnGetCamera();
			GetCameraEvent?.Invoke( this, ref result );
			return result;
		}

		/////////////////////////////////////////

		protected virtual Viewport.CameraSettingsClass OnGetCameraSettings()
		{
			var camera = GetCamera();
			if( createdViewport != null && camera != null )
			{
				var aspectRatio = 0.0;//AspectRatio.Value;
				if( aspectRatio == 0 )
					aspectRatio = camera.AspectRatio.Value;
				var tr = camera.TransformV;

				return new Viewport.CameraSettingsClass( createdViewport, aspectRatio, camera.FieldOfView, camera.NearClipPlane, camera.FarClipPlane, tr.Position, tr.Rotation.GetForward(), camera.FixedUp, camera.Projection, camera.Height, camera.Exposure, camera.EmissiveFactor, renderingPipelineOverride: camera.RenderingPipelineOverride );
			}

			return null;
		}

		public delegate void GetCameraSettingsEventDelegate( UIRenderTarget sender, ref Viewport.CameraSettingsClass cameraSettings );
		public event GetCameraSettingsEventDelegate GetCameraSettingsEvent;

		public Viewport.CameraSettingsClass GetCameraSettings()
		{
			var result = OnGetCameraSettings();
			GetCameraSettingsEvent?.Invoke( this, ref result );
			return result;
		}

		/////////////////////////////////////////

		protected virtual Vector2I GetDemandedSize()
		{
			Vector2I result;

			Vector2I viewportSize = ParentContainer.Viewport.SizeInPixels;
			result = ( viewportSize.ToVector2F() * GetScreenSize() ).ToVector2I();

			if( result.X < 1 )
				result.X = 1;
			if( result.Y < 1 )
				result.Y = 1;

			//fix max texture size
			if( result.X > RenderingSystem.Capabilities.MaxTextureSize || result.Y > RenderingSystem.Capabilities.MaxTextureSize )
			{
				double divideX = (double)result.X / (double)RenderingSystem.Capabilities.MaxTextureSize;
				double divideY = (double)result.Y / (double)RenderingSystem.Capabilities.MaxTextureSize;
				double divide = Math.Max( Math.Max( divideX, divideY ), 1 );
				if( divide != 1 )
				{
					result = ( result.ToVector2() / divide ).ToVector2I();
					if( result.X > RenderingSystem.Capabilities.MaxTextureSize )
						result.X = RenderingSystem.Capabilities.MaxTextureSize;
					if( result.Y > RenderingSystem.Capabilities.MaxTextureSize )
						result.Y = RenderingSystem.Capabilities.MaxTextureSize;
				}
			}

			return result;
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( createdScene != null )
				ComponentsHidePublic.PerformSimulationStep( createdScene );
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );

			//draw texture
			{
				var color = new ColorValue( 1, 1, 1 );

				GetScreenRectangle( out var rect );

				var tex = createdImage;
				if( tex == null )
					tex = ResourceUtility.WhiteTexture2D;

				if( renderer.IsScreen )//&& !renderer._OutGeometryTransformEnabled )
				{
					//screen per pixel accuracy

					Vector2 viewportSize = renderer.ViewportForScreenCanvasRenderer.SizeInPixels.ToVector2F();
					var v = createdForSize.ToVector2() / viewportSize;
					Rectangle fixedRect = new Rectangle( rect.LeftTop, rect.LeftTop + v );

					renderer.PushTextureFilteringMode( CanvasRenderer.TextureFilteringMode.Point );
					renderer.AddQuad( fixedRect, new Rectangle( 0, 0, 1, 1 ), tex, color, true );
					renderer.PopTextureFilteringMode();
				}
				else
					renderer.AddQuad( rect, new Rectangle( 0, 0, 1, 1 ), tex, color, true );
				//}
			}

		}

		public void RenderTargetUpdate()
		{
			if( !viewportDuringUpdate )
			{
				RecreateRenderTargetIfNeeded();
				UpdateAttachedScene();

				if( createdViewport != null )
				{
					try
					{
						viewportDuringUpdate = true;

						createdViewport.BackgroundColorDefault = BackgroundColor;
						createdViewport.Update( true, GetCameraSettings() );
					}
					finally
					{
						viewportDuringUpdate = false;
					}
				}
			}
		}

		private void Viewport_UpdateBegin( Viewport viewport )
		{
			if( EnabledInHierarchyAndIsNotResource && AutoUpdate && VisibleInHierarchy )
				RenderTargetUpdate();
		}

		[Browsable( false )]
		public ImageComponent CreatedImage
		{
			get { return createdImage; }
		}

		[Browsable( false )]
		public Viewport CreatedViewport
		{
			get { return createdViewport; }
		}

		//[Browsable( false )]
		//public UIControl CreatedControl
		//{
		//	get { return createdControl; }
		//}

		public bool GetHDR()
		{
			var hdr = HDR.Value;
			if( hdr == AutoTrueFalse.Auto )
				hdr = SystemSettings.LimitedDevice ? AutoTrueFalse.False : AutoTrueFalse.True;
			return hdr == AutoTrueFalse.True;
		}

		void CreateRenderTarget()
		{
			DestroyRenderTarget();

			var size = GetDemandedSize();
			var hdr = GetHDR();

			var mipmaps = false;

			createdImage = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
			createdImage.CreateType = ImageComponent.TypeEnum._2D;
			createdImage.CreateSize = size;
			createdImage.CreateMipmaps = mipmaps;
			createdImage.CreateFormat = hdr ? PixelFormat.Float32RGBA : PixelFormat.A8R8G8B8;
			var usage = ImageComponent.Usages.RenderTarget;
			if( mipmaps )
				usage |= ImageComponent.Usages.AutoMipmaps;
			createdImage.CreateUsage = usage;
			createdImage.CreateFSAA = 0;
			createdImage.Enabled = true;

			var renderTexture = createdImage.Result.GetRenderTarget();
			createdViewport = renderTexture.AddViewport( true, true );

			createdViewport.UpdateBeforeOutput += CreatedViewport_UpdateBeforeOutput;

			createdForSize = size;
			createdForHDR = hdr;

			UpdateAttachedScene();
		}

		void RecreateRenderTargetIfNeeded()
		{
			if( createdImage == null || createdForSize != GetDemandedSize() || createdForHDR != GetHDR() )
				CreateRenderTarget();
		}

		void DestroyRenderTarget()
		{
			if( createdImage != null )
			{
				createdViewport.UpdateBeforeOutput -= CreatedViewport_UpdateBeforeOutput;

				createdImage.Dispose();
				createdImage = null;
				createdViewport = null;
			}
		}

		void UpdateAttachedScene()
		{
			bool needSceneCreate = false;

			if( createdViewport != null )
			{
				Scene scene = null;
				if( DisplayScene )
					scene = Scene.ReferenceSpecified ? Scene.Value : null;//ParentScene;

				if( scene != null )
				{
					var ins = ComponentUtility.GetResourceInstanceByComponent( scene );
					var isResource = ins != null && ins.InstanceType == Resource.InstanceType.Resource;

					if( isResource )
					{
						SceneRecreateIfNeeded( scene );
						scene = createdScene;
						needSceneCreate = true;
					}
				}

				createdViewport.AttachedScene = scene;
			}

			if( !needSceneCreate )
				SceneDestroy();
		}

		[Browsable( false )]
		public Scene CreatedScene
		{
			get { return createdScene; }
		}

		void ViewportPerformTick( float delta )
		{
			if( !viewportDuringTick )
			{
				try
				{
					viewportDuringTick = true;
					createdViewport?.PerformTick( delta );
				}
				finally
				{
					viewportDuringTick = false;
				}
			}
		}

		private void CreatedViewport_UpdateBeforeOutput( Viewport viewport )
		{
			createdViewport.UIContainer.PerformRenderUI( viewport.CanvasRenderer );
		}

		/////////////////////////////////////////

		public delegate void SceneCreatedDelegate( UIRenderTarget sender );
		public event SceneCreatedDelegate SceneCreated;

		public delegate void SceneDestroyedDelegate( UIRenderTarget sender, Scene destroyedScene );
		public event SceneDestroyedDelegate SceneDestroyed;

		void SceneCreate( Scene source )
		{
			SceneDestroy();

			var fileName = ComponentUtility.GetOwnedFileNameOfComponent( source );
			if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
				createdScene = ResourceManager.LoadSeparateInstance<Component>( fileName, true, null ) as Scene;

			if( createdScene != null )
				SceneCreated?.Invoke( this );

			createdSceneSource = source;
		}

		void SceneDestroy()
		{
			if( createdScene != null )
			{
				var destroyedScene = createdScene;

				createdScene.Dispose();
				createdScene = null;
				createdSceneSource = null;

				SceneDestroyed?.Invoke( this, destroyedScene );
			}
		}

		void SceneRecreateIfNeeded( Scene source )
		{
			if( source != null )
			{
				if( createdSceneSource != source )
					SceneCreate( source );
			}
			else
				SceneDestroy();
		}

	}
}
