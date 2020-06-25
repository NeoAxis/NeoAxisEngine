// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The component intended to manage and display render target in the scene.
	/// </summary>
	public class Component_RenderTargetInSpace : Component_MeshInSpace, IComponent_InteractiveObject
	{
		Component_Image createdImage;
		Vector2I createdForSize;
		bool createdForHDR;
		bool createdForMipmaps;

		Viewport createdViewport;
		UIControl createdControl;

		bool viewportDuringTick;
		bool viewportDuringUpdate;

		bool uiControlUpdating;

		Component_Scene createdScene;
		Component_Scene createdSceneSource;

		double lastVisibleTime;

		/////////////////////////////////////////

		/// <summary>
		/// The height of a 2D canvas in the pixels. The width is calculated by height and aspect ratio.
		/// </summary>
		[DefaultValue( 1024 )]
		public Reference<int> SizeInPixels
		{
			get { if( _sizeInPixels.BeginGet() ) SizeInPixels = _sizeInPixels.Get( this ); return _sizeInPixels.value; }
			set { if( _sizeInPixels.BeginSet( ref value ) ) { try { SizeInPixelsChanged?.Invoke( this ); } finally { _sizeInPixels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SizeInPixels"/> property value changes.</summary>
		public event Action<Component_RenderTargetInSpace> SizeInPixelsChanged;
		ReferenceField<int> _sizeInPixels = 1024;

		/// <summary>
		/// The aspect ratio of the camera.
		/// </summary>
		[DefaultValue( 1.3333 )]
		public Reference<double> AspectRatio
		{
			get { if( _aspectRatio.BeginGet() ) AspectRatio = _aspectRatio.Get( this ); return _aspectRatio.value; }
			set { if( _aspectRatio.BeginSet( ref value ) ) { try { AspectRatioChanged?.Invoke( this ); } finally { _aspectRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AspectRatio"/> property value changes.</summary>
		public event Action<Component_RenderTargetInSpace> AspectRatioChanged;
		ReferenceField<double> _aspectRatio = 1.3333;

		/// <summary>
		/// Whether the high dynamic range is enabled.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> HDR
		{
			get { if( _hdr.BeginGet() ) HDR = _hdr.Get( this ); return _hdr.value; }
			set { if( _hdr.BeginSet( ref value ) ) { try { HDRChanged?.Invoke( this ); } finally { _hdr.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HDR"/> property value changes.</summary>
		public event Action<Component_RenderTargetInSpace> HDRChanged;
		ReferenceField<bool> _hdr = true;

		///// <summary>
		///// Whether to create mip levels for the render target.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> Mipmaps
		//{
		//	get { if( _mipmaps.BeginGet() ) Mipmaps = _mipmaps.Get( this ); return _mipmaps.value; }
		//	set { if( _mipmaps.BeginSet( ref value ) ) { try { MipmapsChanged?.Invoke( this ); } finally { _mipmaps.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Mipmaps"/> property value changes.</summary>
		//public event Action<Component_RenderTargetInSpace> MipmapsChanged;
		//ReferenceField<bool> _mipmaps = false;

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
		public event Action<Component_RenderTargetInSpace> AutoUpdateChanged;
		ReferenceField<bool> _autoUpdate = true;

		/// <summary>
		/// The background color of the control.
		/// </summary>
		[DefaultValue( "0.4 0.4 0.4" )]
		public Reference<ColorValue> BackgroundColor
		{
			get { if( _backgroundColor.BeginGet() ) BackgroundColor = _backgroundColor.Get( this ); return _backgroundColor.value; }
			set { if( _backgroundColor.BeginSet( ref value ) ) { try { BackgroundColorChanged?.Invoke( this ); } finally { _backgroundColor.EndSet(); } } }
		}
		public event Action<Component_RenderTargetInSpace> BackgroundColorChanged;
		ReferenceField<ColorValue> _backgroundColor = new ColorValue( 0.4, 0.4, 0.4 );

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
		public event Action<Component_RenderTargetInSpace> DisplaySceneChanged;
		ReferenceField<bool> _displayScene = true;

		/// <summary>
		/// The scene to display. Set 'null' to use current scene.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Scene" )]
		public Reference<Component_Scene> Scene
		{
			get { if( _scene.BeginGet() ) Scene = _scene.Get( this ); return _scene.value; }
			set { if( _scene.BeginSet( ref value ) ) { try { SceneChanged?.Invoke( this ); UpdateAttachedScene(); } finally { _scene.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Scene"/> property value changes.</summary>
		public event Action<Component_RenderTargetInSpace> SceneChanged;
		ReferenceField<Component_Scene> _scene = null;

		/// <summary>
		/// The camera to use for displaying scene.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Scene" )]
		public Reference<Component_Camera> Camera
		{
			get { if( _camera.BeginGet() ) Camera = _camera.Get( this ); return _camera.value; }
			set { if( _camera.BeginSet( ref value ) ) { try { CameraChanged?.Invoke( this ); } finally { _camera.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Camera"/> property value changes.</summary>
		public event Action<Component_RenderTargetInSpace> CameraChanged;
		ReferenceField<Component_Camera> _camera = null;

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
		public event Action<Component_RenderTargetInSpace> CameraByNameChanged;
		ReferenceField<string> _cameraByName = "";

		/// <summary>
		/// The UI control to use.
		/// </summary>
		[DefaultValue( null )]
		[Category( "GUI" )]
		[DisplayName( "UI Screen" )]
		public Reference<UIControl> UIControl
		{
			get { if( _uiControl.BeginGet() ) UIControl = _uiControl.Get( this ); return _uiControl.value; }
			set { if( _uiControl.BeginSet( ref value ) ) { try { UIControlChanged?.Invoke( this ); UIControlUpdate(); } finally { _uiControl.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIControl"/> property value changes.</summary>
		public event Action<Component_RenderTargetInSpace> UIControlChanged;
		ReferenceField<UIControl> _uiControl = null;

		/// <summary>
		/// Whether to allow user interaction with the object.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AllowInteract
		{
			get { if( _allowInteract.BeginGet() ) AllowInteract = _allowInteract.Get( this ); return _allowInteract.value; }
			set { if( _allowInteract.BeginSet( ref value ) ) { try { AllowInteractChanged?.Invoke( this ); } finally { _allowInteract.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AllowInteract"/> property value changes.</summary>
		public event Action<Component_RenderTargetInSpace> AllowInteractChanged;
		ReferenceField<bool> _allowInteract = true;

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

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			//Mesh

			var mesh = CreateComponent<Component_Mesh>();
			mesh.Name = "Mesh";
			Mesh = ReferenceUtility.MakeThisReference( this, mesh );

			var geometry = mesh.CreateComponent<Component_MeshGeometry_Plane>();
			geometry.Name = "Mesh Geometry";
			geometry.Axis = 0;
			geometry.Dimensions = new Vector2( 1.333, 1 );
			geometry.UVTilesInTotal = new Vector2( 1, 1 );

			//Material

			var material = CreateComponent<Component_Material>();
			material.Name = "Material";
			material.BaseColor = ReferenceUtility.MakeReference( "this:$Shader graph\\$Node Shader Texture Sample 1\\$Shader Texture Sample\\RGBA" );
			material.Emissive = ReferenceUtility.MakeReference( "this:$Shader graph\\$Node Invoke Member 1\\$Invoke Member\\__parameter_ReturnValue" );

			var flowGraph = material.CreateComponent<Component_FlowGraph>();
			flowGraph.Name = "Shader graph";
			flowGraph.Specialization = ReferenceUtility.MakeReference( "NeoAxis.Component_FlowGraphSpecialization_Shader|Instance" );

			{
				var flowGraphNode = flowGraph.CreateComponent<Component_FlowGraphNode>();
				flowGraphNode.Name = "Node Material";
				flowGraphNode.Position = new Vector2I( 10, -7 );
				flowGraphNode.ControlledObject = ReferenceUtility.MakeReference( "this:..\\.." );
			}

			{
				var flowGraphNode = flowGraph.CreateComponent<Component_FlowGraphNode>();
				flowGraphNode.Name = "Node Shader Texture Sample 1";
				flowGraphNode.Position = new Vector2I( -20, -9 );
				flowGraphNode.ControlledObject = ReferenceUtility.MakeReference( "this:$Shader Texture Sample" );

				var textureSample = flowGraphNode.CreateComponent<Component_ShaderTextureSample>();
				textureSample.Name = "Shader Texture Sample";
				textureSample.Texture = ReferenceUtility.MakeReference( "this:..\\..\\..\\..\\CreatedImage" );
			}

			{
				var flowGraphNode = flowGraph.CreateComponent<Component_FlowGraphNode>();
				flowGraphNode.Name = "Node Invoke Member 1";
				flowGraphNode.Position = new Vector2I( -5, -2 );
				flowGraphNode.ControlledObject = ReferenceUtility.MakeReference( "this:$Invoke Member" );

				var invokeMember = flowGraphNode.CreateComponent<Component_InvokeMember>();
				invokeMember.Name = "Invoke Member";
				invokeMember.Member = ReferenceUtility.MakeReference( "NeoAxis.ColorValue|method:op_Multiply(NeoAxis.ColorValue,System.Single)" );
				invokeMember.SetPropertyValue( "property:__parameter_S", new Reference<float>( 0.4f ) );
				invokeMember.SetPropertyValue( "property:__parameter_V", ReferenceUtility.MakeReference( "this:..\\..\\$Node Shader Texture Sample 1\\$Shader Texture Sample\\RGBA" ) );

			}

			geometry.Material = ReferenceUtility.MakeThisReference( geometry, material );

			//Camera

			var camera = CreateComponent<Component_Camera>();
			camera.Name = "Camera";
			camera.Transform = ReferenceUtility.MakeReference( "this:$Attach Transform Offset\\Result" );

			var transformOffet = camera.CreateComponent<Component_TransformOffset>();
			transformOffet.Name = "Attach Transform Offset";
			transformOffet.PositionOffset = new Vector3( 0, 0, 0.55 );
			transformOffet.Source = ReferenceUtility.MakeReference( "this:..\\..\\Transform" );

			Camera = ReferenceUtility.MakeReference( "this:$Camera" );
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( ParentScene != null )
			{
				if( EnabledInHierarchy )
					ParentScene.ViewportUpdateBefore += ParentScene_ViewportUpdateBefore;
				else
					ParentScene.ViewportUpdateBefore -= ParentScene_ViewportUpdateBefore;
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

			//UpdateInteraction( delta );
			ViewportPerformTick( delta );
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( createdScene != null )
				ComponentsHidePublic.PerformSimulationStep( createdScene );
		}

		/////////////////////////////////////////

		protected virtual Component_Camera OnGetCamera()
		{
			//default behavior
			if( createdViewport != null && createdViewport.AttachedScene != null )
			{
				//Camera property
				{
					var camera = Camera.Value;
					if( camera != null )
						return camera;
				}

				//CameraByName property
				if( !string.IsNullOrEmpty( CameraByName ) )
				{
					var camera = createdViewport.AttachedScene.GetComponent( CameraByName, onlyEnabledInHierarchy: true ) as Component_Camera;
					if( camera != null )
						return camera;
				}
			}

			return null;
		}

		public delegate void GetCameraEventDelegate( Component_RenderTargetInSpace sender, ref Component_Camera camera );
		public event GetCameraEventDelegate GetCameraEvent;

		public Component_Camera GetCamera()
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
				var aspectRatio = AspectRatio.Value;
				if( aspectRatio == 0 )
					aspectRatio = camera.AspectRatio.Value;
				var tr = camera.TransformV;

				return new Viewport.CameraSettingsClass( createdViewport, aspectRatio, camera.FieldOfView, camera.NearClipPlane, camera.FarClipPlane, tr.Position, tr.Rotation.GetForward(), camera.FixedUp, camera.Projection, camera.Height, camera.Exposure, camera.EmissiveFactor, renderingPipelineOverride: camera.RenderingPipelineOverride );
			}

			return null;
		}

		public delegate void GetCameraSettingsEventDelegate( Component_RenderTargetInSpace sender, ref Viewport.CameraSettingsClass cameraSettings );
		public event GetCameraSettingsEventDelegate GetCameraSettingsEvent;

		public Viewport.CameraSettingsClass GetCameraSettings()
		{
			var result = OnGetCameraSettings();
			GetCameraSettingsEvent?.Invoke( this, ref result );
			return result;
		}

		/////////////////////////////////////////

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

		private void ParentScene_ViewportUpdateBefore( Component_Scene scene, Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		{
			if( EnabledInHierarchyAndIsNotResource && AutoUpdate && ( viewport.LastUpdateTime == lastVisibleTime || viewport.PreviousUpdateTime == lastVisibleTime ) )
				RenderTargetUpdate();
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			base.OnGetRenderSceneData( context, mode );

			if( mode == GetRenderSceneDataMode.InsideFrustum && AutoUpdate )
				lastVisibleTime = Time.Current;
		}

		[Browsable( false )]
		public Component_Image CreatedImage
		{
			get { return createdImage; }
		}

		[Browsable( false )]
		public Viewport CreatedViewport
		{
			get { return createdViewport; }
		}

		[Browsable( false )]
		public UIControl CreatedControl
		{
			get { return createdControl; }
		}

		protected virtual Vector2I GetDemandedSize()
		{
			if( AspectRatio != 0 )
				return new Vector2I( (int)( (double)SizeInPixels * AspectRatio ), SizeInPixels );
			else
				return new Vector2I( SizeInPixels, SizeInPixels );
		}

		void CreateRenderTarget()
		{
			DestroyRenderTarget();

			var size = GetDemandedSize();
			var hdr = HDR.Value;
			//!!!!is not works
			//var mipmaps = true;
			var mipmaps = false;//Mipmaps.Value;

			//!!!!
			//mipmaps = true;

			createdImage = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
			createdImage.CreateType = Component_Image.TypeEnum._2D;
			createdImage.CreateSize = size;
			createdImage.CreateMipmaps = mipmaps;
			createdImage.CreateFormat = hdr ? PixelFormat.Float32RGBA : PixelFormat.A8R8G8B8;
			var usage = Component_Image.Usages.RenderTarget;
			if( mipmaps )
				usage |= Component_Image.Usages.AutoMipmaps;
			createdImage.CreateUsage = usage;
			createdImage.CreateFSAA = 0;
			createdImage.Enabled = true;

			var renderTexture = createdImage.Result.GetRenderTarget();
			createdViewport = renderTexture.AddViewport( true, true );
			createdViewport.UIContainer.Transform3D = TransformV;

			createdViewport.UpdateBeforeOutput += CreatedViewport_UpdateBeforeOutput;

			createdForSize = size;
			createdForHDR = hdr;
			createdForMipmaps = mipmaps;

			UpdateAttachedScene();
			UIControlUpdate();
		}

		void RecreateRenderTargetIfNeeded()
		{
			if( createdImage == null || createdForSize != GetDemandedSize() || createdForHDR != HDR.Value )//!!!! || createdForMipmaps != Mipmaps.Value )
				CreateRenderTarget();
		}

		void DestroyRenderTarget()
		{
			if( createdImage != null )
			{
				createdViewport.UpdateBeforeOutput -= CreatedViewport_UpdateBeforeOutput;

				UIControlDestroy();
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
				Component_Scene scene = null;
				if( DisplayScene )
					scene = Scene.ReferenceSpecified ? Scene.Value : ParentScene;

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

		void UIControlUpdate()
		{
			if( uiControlUpdating )
				return;

			UIControlDestroy();

			if( createdImage != null )
			{
				try
				{
					uiControlUpdating = true;

					var sourceControl = UIControl.Value;
					if( sourceControl != null )
					{
						createdControl = (UIControl)sourceControl.Clone();
						createdViewport.UIContainer.AddComponent( createdControl );

						ViewportPerformTick( 0.0001f );
					}
				}
				finally
				{
					uiControlUpdating = false;
				}
			}
		}

		void UIControlDestroy()
		{
			createdControl?.Dispose();
			createdControl = null;
		}

		[Browsable( false )]
		public Component_Scene CreatedScene
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

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			if( createdViewport != null )
				createdViewport.UIContainer.Transform3D = TransformV;
		}

		/////////////////////////////////////////

		public delegate void SceneCreatedDelegate( Component_RenderTargetInSpace sender );
		public event SceneCreatedDelegate SceneCreated;

		public delegate void SceneDestroyedDelegate( Component_RenderTargetInSpace sender, Component_Scene destroyedScene );
		public event SceneDestroyedDelegate SceneDestroyed;

		void SceneCreate( Component_Scene source )
		{
			SceneDestroy();

			var fileName = ComponentUtility.GetOwnedFileNameOfComponent( source );
			if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
				createdScene = ResourceManager.LoadSeparateInstance<Component>( fileName, true, null ) as Component_Scene;

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

		void SceneRecreateIfNeeded( Component_Scene source )
		{
			if( source != null )
			{
				if( createdSceneSource != source )
					SceneCreate( source );
			}
			else
				SceneDestroy();
		}

		/////////////////////////////////////////

		public bool GetScreenPositionByRay( Ray ray, out Vector2 screenPosition )
		{
			if( createdViewport != null && createdViewport.UIContainer.Transform3D != null )
			{
				var matrix = createdViewport.UIContainer.Transform3D.ToMatrix4();
				var inv = matrix.GetInverse();

				var origin = inv * ray.Origin;
				var end = inv * ray.GetEndPoint();
				var rayLocal = new Ray( origin, end - origin );

				var plane = Plane.FromPointAndNormal( Vector3.Zero, Vector3.XAxis );
				if( plane.Intersects( rayLocal, out Vector3 intersection ) )
				{
					var aspect = 1.0;
					if( AspectRatio != 0 )
						aspect = AspectRatio;
					screenPosition = new Vector2( intersection.Y / aspect + 0.5, 0.5 - intersection.Z );
					return true;
				}
			}

			screenPosition = Vector2.Zero;
			return false;
		}

		public bool ObjectInteractionInputMessage( UIControl playScreen, Component_GameMode gameMode, InputMessage message )
		{
			if( createdViewport != null )
			{
				var mouseDown = message as InputMessageMouseButtonDown;
				if( mouseDown != null && createdViewport.UIContainer.PerformMouseDown( mouseDown.Button ) )
					return true;

				var mouseDoubleClick = message as InputMessageMouseDoubleClick;
				if( mouseDoubleClick != null && createdViewport.UIContainer.PerformMouseDoubleClick( mouseDoubleClick.Button ) )
					return true;

				var mouseUp = message as InputMessageMouseButtonUp;
				if( mouseUp != null && createdViewport.UIContainer.PerformMouseUp( mouseUp.Button ) )
					return true;

				var keyDown = message as InputMessageKeyDown;
				if( keyDown != null && createdViewport.UIContainer.PerformKeyDown( new KeyEvent( keyDown.Key ) ) )
					return true;

				var keyPress = message as InputMessageKeyPress;
				if( keyPress != null && createdViewport.UIContainer.PerformKeyPress( new KeyPressEvent( keyPress.KeyChar ) ) )
					return true;

				var keyUp = message as InputMessageKeyUp;
				if( keyUp != null && createdViewport.UIContainer.PerformKeyUp( new KeyEvent( keyUp.Key ) ) )
					return true;

				var mouseWheel = message as InputMessageMouseWheel;
				if( mouseWheel != null && createdViewport.UIContainer.PerformMouseWheel( mouseWheel.Delta ) )
					return true;

				//PerformJoystickEvent
				//PerformSpecialInputDeviceEvents
			}

			return false;
		}

		public void ObjectInteractionGetInfo( UIControl playScreen, Component_GameMode gameMode, ref IComponent_InteractiveObject_ObjectInfo info )
		{
			info = new IComponent_InteractiveObject_ObjectInfo();
			info.AllowInteract = AllowInteract;
			info.SelectionTextInfo.Add( Name );
			info.DisplaySelectionRectangle = false;
		}

		public void ObjectInteractionEnter( Component_GameMode.ObjectInteractionContextClass context )
		{
		}

		public void ObjectInteractionExit( Component_GameMode.ObjectInteractionContextClass context )
		{
		}

		public void ObjectInteractionUpdate( Component_GameMode.ObjectInteractionContextClass context )
		{
			if( createdViewport != null )
			{
				var cameraSettings = context.Viewport.CameraSettings;
				var ray = new Ray( cameraSettings.Position, cameraSettings.Rotation.GetForward() * cameraSettings.FarClipDistance );

				//update mouse position
				if( GetScreenPositionByRay( ray, out var screenPosition ) )
					createdViewport.UIContainer.PerformMouseMove( screenPosition );
			}
		}
	}
}