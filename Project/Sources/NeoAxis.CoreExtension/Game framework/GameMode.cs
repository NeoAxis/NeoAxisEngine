// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The object to interact Player app with the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Game Mode", -9999 )]
	public class GameMode : Component
	{
		static UIControl playScreen;

		//input
		bool inputEnabled;
		bool[] keys;
		bool[] mouseButtons = new bool[ 5 ];
		Vector2 mousePosition = new Vector2( -10000, -10000 );
		bool mouseRelativeMode;

		//free camera
		Vector3 freeCameraPosition;
		SphericalDirection freeCameraDirection;
		bool freeCameraMouseRotating;
		bool freeCameraMouseRotatingActivated;
		Vector2 freeCameraRotatingStartPos;

		/////////////////////////////////////////

		/// <summary>
		/// The scene object under player control.
		/// </summary>
		[Category( "Input" )]
		[DefaultValue( null )]
		[NetworkSynchronize( false )]
		public Reference<Component> ObjectControlledByPlayer
		{
			get { if( _objectControlledByPlayer.BeginGet() ) ObjectControlledByPlayer = _objectControlledByPlayer.Get( this ); return _objectControlledByPlayer.value; }
			set
			{
				var oldObject = _objectControlledByPlayer.value.Value;

				if( _objectControlledByPlayer.BeginSet( ref value ) )
				{
					try
					{
						ObjectControlledByPlayerChanged?.Invoke( this );

						//process changing object controlled by the player in simulation
						if( EngineApp.IsSimulation && playScreen != null )
						{
							if( oldObject != null && !oldObject.Disposed )
							{
								var input = GetInputProcessing( oldObject );
								if( input != null )
								{
									input.PerformMessage( this, new InputMessageInputEnabledChanged( false ) );
									input.PerformMessage( this, new InputMessageMouseRelativeModeChanged( false ) );
									foreach( var key in Viewport.AllKeys )
										input.PerformMessage( this, new InputMessageKeyUp( key ) );
									foreach( EMouseButtons button in Enum.GetValues( typeof( EMouseButtons ) ) )
										input.PerformMessage( this, new InputMessageMouseButtonUp( button ) );
								}

								UpdateControlledObjectVisibilitySetToDefault( oldObject );
							}
						}
					}
					finally { _objectControlledByPlayer.EndSet(); }
				}
			}
		}
		public event Action<GameMode> ObjectControlledByPlayerChanged;
		ReferenceField<Component> _objectControlledByPlayer = null;

		public enum BuiltInCameraEnum
		{
			None,
			FirstPerson,
			ThirdPerson,
		}

		/// <summary>
		/// Whether to use one of built-in camera management methods.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( BuiltInCameraEnum.None )]
		public Reference<BuiltInCameraEnum> UseBuiltInCamera
		{
			get { if( _useBuiltInCamera.BeginGet() ) UseBuiltInCamera = _useBuiltInCamera.Get( this ); return _useBuiltInCamera.value; }
			set { if( _useBuiltInCamera.BeginSet( ref value ) ) { try { UseBuiltInCameraChanged?.Invoke( this ); } finally { _useBuiltInCamera.EndSet(); } } }
		}
		public event Action<GameMode> UseBuiltInCameraChanged;
		ReferenceField<BuiltInCameraEnum> _useBuiltInCamera = BuiltInCameraEnum.None;

		/// <summary>
		/// Whether to show a character or another unit which controlled by the player.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( true )]
		public Reference<bool> FirstPersonCameraShowControlledObject
		{
			get { if( _firstPersonCameraShowControlledObject.BeginGet() ) FirstPersonCameraShowControlledObject = _firstPersonCameraShowControlledObject.Get( this ); return _firstPersonCameraShowControlledObject.value; }
			set { if( _firstPersonCameraShowControlledObject.BeginSet( ref value ) ) { try { FirstPersonCameraShowControlledObjectChanged?.Invoke( this ); } finally { _firstPersonCameraShowControlledObject.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FirstPersonCameraShowControlledObject"/> property value changes.</summary>
		public event Action<GameMode> FirstPersonCameraShowControlledObjectChanged;
		ReferenceField<bool> _firstPersonCameraShowControlledObject = true;

		/// <summary>
		/// The radius of cut geometry from the camera to prevent drawing the internal part of the controlled object 3D model.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 0.2 )]
		[Range( 0, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> FirstPersonCameraCutVolumeRadius
		{
			get { if( _firstPersonCameraCutVolumeRadius.BeginGet() ) FirstPersonCameraCutVolumeRadius = _firstPersonCameraCutVolumeRadius.Get( this ); return _firstPersonCameraCutVolumeRadius.value; }
			set { if( _firstPersonCameraCutVolumeRadius.BeginSet( ref value ) ) { try { FirstPersonCameraCutVolumeRadiusChanged?.Invoke( this ); } finally { _firstPersonCameraCutVolumeRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FirstPersonCameraCutVolumeRadius"/> property value changes.</summary>
		public event Action<GameMode> FirstPersonCameraCutVolumeRadiusChanged;
		ReferenceField<double> _firstPersonCameraCutVolumeRadius = 0.2;

		/// <summary>
		/// Whether to attach the camera in the first person mode to the eyes of the controlled object.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( true )]
		public Reference<bool> FirstPersonCameraAttachToEyes
		{
			get { if( _firstPersonCameraAttachToEyes.BeginGet() ) FirstPersonCameraAttachToEyes = _firstPersonCameraAttachToEyes.Get( this ); return _firstPersonCameraAttachToEyes.value; }
			set { if( _firstPersonCameraAttachToEyes.BeginSet( ref value ) ) { try { FirstPersonCameraAttachToEyesChanged?.Invoke( this ); } finally { _firstPersonCameraAttachToEyes.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FirstPersonCameraAttachToEyes"/> property value changes.</summary>
		public event Action<GameMode> FirstPersonCameraAttachToEyesChanged;
		ReferenceField<bool> _firstPersonCameraAttachToEyes = true;

		/// <summary>
		/// Whether to change the position of the third person camera depending on the direction of the object.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( false )]
		public Reference<bool> ThirdPersonCameraFollowDirection
		{
			get { if( _thirdPersonCameraFollowDirection.BeginGet() ) ThirdPersonCameraFollowDirection = _thirdPersonCameraFollowDirection.Get( this ); return _thirdPersonCameraFollowDirection.value; }
			set { if( _thirdPersonCameraFollowDirection.BeginSet( ref value ) ) { try { ThirdPersonCameraFollowDirectionChanged?.Invoke( this ); } finally { _thirdPersonCameraFollowDirection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraFollowDirection"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraFollowDirectionChanged;
		ReferenceField<bool> _thirdPersonCameraFollowDirection = false;

		/// <summary>
		/// Third-person camera rotation speed. Degree per second.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 180.0 )]
		[Range( 0, 360 )]
		public Reference<Degree> ThirdPersonCameraFollowDirectionSpeed
		{
			get { if( _thirdPersonCameraFollowDirectionSpeed.BeginGet() ) ThirdPersonCameraFollowDirectionSpeed = _thirdPersonCameraFollowDirectionSpeed.Get( this ); return _thirdPersonCameraFollowDirectionSpeed.value; }
			set { if( _thirdPersonCameraFollowDirectionSpeed.BeginSet( ref value ) ) { try { ThirdPersonCameraFollowDirectionSpeedChanged?.Invoke( this ); } finally { _thirdPersonCameraFollowDirectionSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraFollowDirectionSpeed"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraFollowDirectionSpeedChanged;
		ReferenceField<Degree> _thirdPersonCameraFollowDirectionSpeed = new Degree( 180 );

		/// <summary>
		/// The horizontal angle of the camera in third-person mode.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 60 )]
		[Range( 0, 360 )]
		public Reference<Degree> ThirdPersonCameraHorizontalAngle
		{
			get { if( _thirdPersonCameraHorizontalAngle.BeginGet() ) ThirdPersonCameraHorizontalAngle = _thirdPersonCameraHorizontalAngle.Get( this ); return _thirdPersonCameraHorizontalAngle.value; }
			set { if( _thirdPersonCameraHorizontalAngle.BeginSet( ref value ) ) { try { ThirdPersonCameraHorizontalAngleChanged?.Invoke( this ); } finally { _thirdPersonCameraHorizontalAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraHorizontalAngle"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraHorizontalAngleChanged;
		ReferenceField<Degree> _thirdPersonCameraHorizontalAngle = new Degree( 60 );

		/// <summary>
		/// The vertical angle of the camera in third-person mode.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 30 )]
		[Range( -89.99, 89.99 )]
		//[Range( -180, 180 )]
		public Reference<Degree> ThirdPersonCameraVerticalAngle
		{
			get { if( _thirdPersonCameraVerticalAngle.BeginGet() ) ThirdPersonCameraVerticalAngle = _thirdPersonCameraVerticalAngle.Get( this ); return _thirdPersonCameraVerticalAngle.value; }
			set { if( _thirdPersonCameraVerticalAngle.BeginSet( ref value ) ) { try { ThirdPersonCameraVerticalAngleChanged?.Invoke( this ); } finally { _thirdPersonCameraVerticalAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraVerticalAngle"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraVerticalAngleChanged;
		ReferenceField<Degree> _thirdPersonCameraVerticalAngle = new Degree( 30 );

		/// <summary>
		/// The distance of the camera in third-person mode.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 5.0 )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> ThirdPersonCameraDistance
		{
			get { if( _thirdPersonCameraDistance.BeginGet() ) ThirdPersonCameraDistance = _thirdPersonCameraDistance.Get( this ); return _thirdPersonCameraDistance.value; }
			set { if( _thirdPersonCameraDistance.BeginSet( ref value ) ) { try { ThirdPersonCameraDistanceChanged?.Invoke( this ); } finally { _thirdPersonCameraDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraDistance"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraDistanceChanged;
		ReferenceField<double> _thirdPersonCameraDistance = 5;

		/// <summary>
		/// The height of the camera in third-person mode.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 1.0 )]
		[Range( -20, 20 )]
		public Reference<double> ThirdPersonCameraHeight
		{
			get { if( _thirdPersonCameraHeight.BeginGet() ) ThirdPersonCameraHeight = _thirdPersonCameraHeight.Get( this ); return _thirdPersonCameraHeight.value; }
			set { if( _thirdPersonCameraHeight.BeginSet( ref value ) ) { try { ThirdPersonCameraHeightChanged?.Invoke( this ); } finally { _thirdPersonCameraHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraHeight"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraHeightChanged;
		ReferenceField<double> _thirdPersonCameraHeight = 1.0;

		/// <summary>
		/// Whether is a free camera enabled.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( false )]
		public Reference<bool> FreeCamera
		{
			get { if( _freeCamera.BeginGet() ) FreeCamera = _freeCamera.Get( this ); return _freeCamera.value; }
			set { if( _freeCamera.BeginSet( ref value ) ) { try { FreeCameraChanged?.Invoke( this ); } finally { _freeCamera.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FreeCamera"/> property value changes.</summary>
		public event Action<GameMode> FreeCameraChanged;
		ReferenceField<bool> _freeCamera = false;

		/// <summary>
		/// Whether to use hotkey to enable a free camera.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( true )]
		public Reference<bool> FreeCameraHotKey
		{
			get { if( _freeCameraHotKey.BeginGet() ) FreeCameraHotKey = _freeCameraHotKey.Get( this ); return _freeCameraHotKey.value; }
			set { if( _freeCameraHotKey.BeginSet( ref value ) ) { try { FreeCameraHotKeyChanged?.Invoke( this ); } finally { _freeCameraHotKey.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FreeCameraHotKey"/> property value changes.</summary>
		public event Action<GameMode> FreeCameraHotKeyChanged;
		ReferenceField<bool> _freeCameraHotKey = true;

		/// <summary>
		/// Hotkey to enable a free camera.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( EKeys.F7 )]
		public Reference<EKeys> FreeCameraHotKeyValue
		{
			get { if( _freeCameraHotKeyValue.BeginGet() ) FreeCameraHotKeyValue = _freeCameraHotKeyValue.Get( this ); return _freeCameraHotKeyValue.value; }
			set { if( _freeCameraHotKeyValue.BeginSet( ref value ) ) { try { FreeCameraHotKeyValueChanged?.Invoke( this ); } finally { _freeCameraHotKeyValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FreeCameraHotKeyValue"/> property value changes.</summary>
		public event Action<GameMode> FreeCameraHotKeyValueChanged;
		ReferenceField<EKeys> _freeCameraHotKeyValue = EKeys.F7;

		/// <summary>
		/// The normal speed of a free camera.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 3 )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> FreeCameraSpeedNormal
		{
			get { if( _freeCameraSpeedNormal.BeginGet() ) FreeCameraSpeedNormal = _freeCameraSpeedNormal.Get( this ); return _freeCameraSpeedNormal.value; }
			set { if( _freeCameraSpeedNormal.BeginSet( ref value ) ) { try { FreeCameraSpeedNormalChanged?.Invoke( this ); } finally { _freeCameraSpeedNormal.EndSet(); } } }
		}
		public event Action<GameMode> FreeCameraSpeedNormalChanged;
		ReferenceField<double> _freeCameraSpeedNormal = 3;

		/// <summary>
		/// The fast speed of a free camera (when Shift key is pressed).
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 20 )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> FreeCameraSpeedFast
		{
			get { if( _freeCameraSpeedFast.BeginGet() ) FreeCameraSpeedFast = _freeCameraSpeedFast.Get( this ); return _freeCameraSpeedFast.value; }
			set { if( _freeCameraSpeedFast.BeginSet( ref value ) ) { try { FreeCameraSpeedFastChanged?.Invoke( this ); } finally { _freeCameraSpeedFast.EndSet(); } } }
		}
		public event Action<GameMode> FreeCameraSpeedFastChanged;
		ReferenceField<double> _freeCameraSpeedFast = 20;

		/// <summary>
		/// Whether to display a target in the center of the screen in the first or third person camera.
		/// </summary>
		[Category( "UI" )]
		[DefaultValue( AutoTrueFalse.Auto )]
		public Reference<AutoTrueFalse> DisplayTarget
		{
			get { if( _displayTarget.BeginGet() ) DisplayTarget = _displayTarget.Get( this ); return _displayTarget.value; }
			set { if( _displayTarget.BeginSet( ref value ) ) { try { DisplayTargetChanged?.Invoke( this ); } finally { _displayTarget.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayTarget"/> property value changes.</summary>
		public event Action<GameMode> DisplayTargetChanged;
		ReferenceField<AutoTrueFalse> _displayTarget = AutoTrueFalse.Auto;

		/// <summary>
		/// The vertical size of the display target.
		/// </summary>
		[Category( "UI" )]
		[DefaultValue( 0.04 )]
		public Reference<double> DisplayTargetSize
		{
			get { if( _displayTargetSize.BeginGet() ) DisplayTargetSize = _displayTargetSize.Get( this ); return _displayTargetSize.value; }
			set { if( _displayTargetSize.BeginSet( ref value ) ) { try { DisplayTargetSizeChanged?.Invoke( this ); } finally { _displayTargetSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayTargetSize"/> property value changes.</summary>
		public event Action<GameMode> DisplayTargetSizeChanged;
		ReferenceField<double> _displayTargetSize = 0.04;

		/// <summary>
		/// The image of the display target.
		/// </summary>
		[Category( "UI" )]
		[DefaultValueReference( @"Base\UI\Cursors\Target.png" )]
		public Reference<ImageComponent> DisplayTargetImage
		{
			get { if( _displayTargetImage.BeginGet() ) DisplayTargetImage = _displayTargetImage.Get( this ); return _displayTargetImage.value; }
			set { if( _displayTargetImage.BeginSet( ref value ) ) { try { DisplayTargetImageChanged?.Invoke( this ); } finally { _displayTargetImage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayTargetImage"/> property value changes.</summary>
		public event Action<GameMode> DisplayTargetImageChanged;
		ReferenceField<ImageComponent> _displayTargetImage = new Reference<ImageComponent>( null, @"Base\UI\Cursors\Target.png" );

		/// <summary>
		/// The color and alpha multiplier of the display target.
		/// </summary>
		[Category( "UI" )]
		[DefaultValue( "1 1 1 0.5" )]
		public Reference<ColorValue> DisplayTargetColor
		{
			get { if( _displayTargetColor.BeginGet() ) DisplayTargetColor = _displayTargetColor.Get( this ); return _displayTargetColor.value; }
			set { if( _displayTargetColor.BeginSet( ref value ) ) { try { DisplayTargetColorChanged?.Invoke( this ); } finally { _displayTargetColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayTargetColor"/> property value changes.</summary>
		public event Action<GameMode> DisplayTargetColorChanged;
		ReferenceField<ColorValue> _displayTargetColor = new ColorValue( 1, 1, 1, 0.5 );

		//[DefaultValue( true )]
		//public Reference<bool> DisplayHelpText
		//{
		//	get { if( _displayHelpText.BeginGet() ) DisplayHelpText = _displayHelpText.Get( this ); return _displayHelpText.value; }
		//	set { if( _displayHelpText.BeginSet( ref value ) ) { try { DisplayHelpTextChanged?.Invoke( this ); } finally { _displayHelpText.EndSet(); } } }
		//}
		//public event Action<GameMode> DisplayHelpTextChanged;
		//ReferenceField<bool> _displayHelpText = true;

		[Browsable( false )]
		public ObjectInteractionContext ObjectInteractionContext { get; set; }

		//cutscene
		[Browsable( false )]
		[Serialize]
		public bool CutsceneStarted { get; set; }
		[Browsable( false )]
		[Serialize]
		public double CutsceneGuiFadingFactor { get; set; }
		[Browsable( false )]
		[Serialize]
		public double CutsceneGuiFadingSpeed { get; set; }
		[Browsable( false )]
		[Serialize]
		public string CutsceneText { get; set; } = "";

		//screen fading
		[Browsable( false )]
		[Serialize]
		public ColorValue ScreenFadingCurrentColor { get; set; }
		[Browsable( false )]
		[Serialize]
		public ColorValue ScreenFadingTargetColor { get; set; }
		[Browsable( false )]
		[Serialize]
		public double ScreenFadingSpeed { get; set; }

		//replace camera
		[Browsable( false )]
		[Serialize]
		[DefaultValue( null )]
		public Reference<Camera> ReplaceCamera
		{
			get { if( _replaceCamera.BeginGet() ) ReplaceCamera = _replaceCamera.Get( this ); return _replaceCamera.value; }
			set { if( _replaceCamera.BeginSet( ref value ) ) { try { ReplaceCameraChanged?.Invoke( this ); } finally { _replaceCamera.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReplaceCamera"/> property value changes.</summary>
		public event Action<GameMode> ReplaceCameraChanged;
		ReferenceField<Camera> _replaceCamera = null;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( FreeCameraHotKeyValue ):
					if( !FreeCameraHotKey )
						skip = true;
					break;

				case nameof( FirstPersonCameraShowControlledObject ):
				case nameof( FirstPersonCameraAttachToEyes ):
				case nameof( FirstPersonCameraCutVolumeRadius ):
					if( UseBuiltInCamera.Value != BuiltInCameraEnum.FirstPerson )
						skip = true;
					break;

				case nameof( ThirdPersonCameraFollowDirection ):
				case nameof( ThirdPersonCameraVerticalAngle ):
				case nameof( ThirdPersonCameraDistance ):
				case nameof( ThirdPersonCameraHeight ):
					if( UseBuiltInCamera.Value != BuiltInCameraEnum.ThirdPerson )
						skip = true;
					break;

				case nameof( ThirdPersonCameraHorizontalAngle ):
					if( UseBuiltInCamera.Value != BuiltInCameraEnum.ThirdPerson || ThirdPersonCameraFollowDirection.Value )
						skip = true;
					break;

				case nameof( ThirdPersonCameraFollowDirectionSpeed ):
					if( UseBuiltInCamera.Value != BuiltInCameraEnum.ThirdPerson || !ThirdPersonCameraFollowDirection.Value )
						skip = true;
					break;

				case nameof( DisplayTargetSize ):
				case nameof( DisplayTargetImage ):
				case nameof( DisplayTargetColor ):
					if( DisplayTarget.Value == AutoTrueFalse.False )
						skip = true;
					break;
				}
			}
		}

		public delegate void GetCameraSettingsEventDelegate( GameMode sender, Viewport viewport, Camera cameraDefault, ref Viewport.CameraSettingsClass cameraSettings );
		public event GetCameraSettingsEventDelegate GetCameraSettingsEvent;

		public virtual Viewport.CameraSettingsClass GetCameraSettings( Viewport viewport, Camera cameraDefault )
		{
			Viewport.CameraSettingsClass result = null;

			//replace camera
			if( result == null )
			{
				var replaceCamera = ReplaceCamera.Value;
				if( replaceCamera != null )
					result = new Viewport.CameraSettingsClass( viewport, replaceCamera );
			}
			//if( result == null && ReplaceCamera != null )
			//	result = new Viewport.CameraSettingsClass( viewport, ReplaceCamera );

			if( result == null )
			{
				if( FreeCamera )
				{
					//free camera

					result = new Viewport.CameraSettingsClass( viewport, cameraDefault.AspectRatio, cameraDefault.FieldOfView, cameraDefault.NearClipPlane, cameraDefault.FarClipPlane, freeCameraPosition, freeCameraDirection.GetVector(), Vector3.ZAxis, ProjectionType.Perspective, 1, cameraDefault.Exposure, cameraDefault.EmissiveFactor );
				}
				else if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson )
				{
					//first person camera

					var objectInSpace = ObjectControlledByPlayer.Value as ObjectInSpace;
					if( objectInSpace != null )
					{
						var tr = objectInSpace.TransformV;

						var position = tr.Position;
						var forward = tr.Rotation.GetForward();
						var up = tr.Rotation.GetUp();

						//Character specific
						var character = ObjectControlledByPlayer.Value as Character;
						if( character != null )
							character.GetFirstPersonCameraPosition( FirstPersonCameraAttachToEyes, out position, out forward, out up );

						//Vehicle specific
						var vehicle = ObjectControlledByPlayer.Value as Vehicle;
						if( vehicle != null )
							vehicle.GetFirstPersonCameraPosition( FirstPersonCameraAttachToEyes, out position, out forward, out up );

						result = new Viewport.CameraSettingsClass( viewport, cameraDefault.AspectRatio, cameraDefault.FieldOfView, cameraDefault.NearClipPlane, cameraDefault.FarClipPlane, position, forward, up, ProjectionType.Perspective, 1, cameraDefault.Exposure, cameraDefault.EmissiveFactor );
					}
				}
				else if( UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
				{
					//third person camera

					//3D
					if( Scene.Mode.Value == Scene.ModeEnum._3D )
					{
						var obj = ObjectControlledByPlayer.Value as ObjectInSpace;
						//var character = ObjectControlledByPlayer.Value as Character;
						if( obj != null )
						{
							var lookAt = obj.TransformV.Position;
							//Character specific
							var character = ObjectControlledByPlayer.Value as Character;
							if( character != null )
								lookAt = character.GetCenteredSmoothPosition();
							lookAt.Z += ThirdPersonCameraHeight;

							var d = new SphericalDirection( MathEx.DegreeToRadian( ThirdPersonCameraHorizontalAngle.Value ), MathEx.DegreeToRadian( ThirdPersonCameraVerticalAngle.Value ) );
							var direction = -d.GetVector();

							var from = lookAt - direction * ThirdPersonCameraDistance.Value;

							//!!!!impl
							////fix by physics
							//{
							//	var item = new PhysicsVolumeTestItem( new Sphere( lookAt, 0.1 ), direction, PhysicsVolumeTestItem.ModeEnum.All );
							//	Scene.PhysicsVolumeTest( item );

							//	if( item.Result.Length != 0 )
							//	{
							//		var body = item.Result[ 0 ].Body;

							//		//!!!!need DistanceScale
							//	}

							//	//var ray = new Ray( lookAt, from - lookAt );
							//	//var item = new PhysicsRayTestItem( ray, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );
							//	//Scene.PhysicsRayTest( item );

							//	//if( item.Result.Length != 0 )//for( int n = 0; n < item.Result.Length; n++ )
							//	//{
							//	//	ref var resultItem = ref item.Result[ 0 ];//ref var resultItem = ref item.Result[ n ];

							//	//	var newFrom = ray.GetPointOnRay( resultItem.DistanceScale );
							//	//	from = newFrom - ray.Direction.GetNormalize() * 0.1;
							//	//}
							//}

							result = new Viewport.CameraSettingsClass( viewport, cameraDefault.AspectRatio, cameraDefault.FieldOfView, cameraDefault.NearClipPlane, cameraDefault.FarClipPlane, from, direction, Vector3.ZAxis, ProjectionType.Perspective, 1, cameraDefault.Exposure, cameraDefault.EmissiveFactor );
						}
					}

					//2D
					if( Scene.Mode.Value == Scene.ModeEnum._2D )
					{
						var obj = ObjectControlledByPlayer.Value as ObjectInSpace;
						//var character = ObjectControlledByPlayer.Value as Character2D;
						if( obj != null )
						{
							var lookAt = obj.TransformV.Position;
							var from = lookAt + new Vector3( 0, 0, 10 );

							result = new Viewport.CameraSettingsClass( viewport, cameraDefault.AspectRatio, cameraDefault.FieldOfView, cameraDefault.NearClipPlane, cameraDefault.FarClipPlane, from, -Vector3.ZAxis, Vector3.YAxis, ProjectionType.Orthographic, cameraDefault.Height, cameraDefault.Exposure, cameraDefault.EmissiveFactor );
						}
					}

				}
			}

			//CameraManagement
			if( result == null )
			{
				var m = GetCameraManagementOfCurrentObject();
				if( m != null )
					result = m.GetCameraSettings( this, viewport, cameraDefault );
			}

			GetCameraSettingsEvent?.Invoke( this, viewport, cameraDefault, ref result );

			return result;
		}

		public delegate void InputMessageEventDelegate( GameMode sender, InputMessage message/*, ref bool handled*/ );
		public event InputMessageEventDelegate InputMessageEvent;

		static int GetEKeysMaxIndex()
		{
			int maxIndex = 0;
			foreach( EKeys eKey in Enum.GetValues( typeof( EKeys ) ) )
			{
				int index = (int)eKey;
				if( index > maxIndex )
					maxIndex = index;
			}
			return maxIndex;
		}

		bool ProcessInputMessageBefore( InputMessage message )
		{
			//input enabled changed
			{
				var m = message as InputMessageInputEnabledChanged;
				if( m != null )
					inputEnabled = m.Value;
			}

			//key down
			{
				var m = message as InputMessageKeyDown;
				if( m != null )
				{
					if( keys == null )
						keys = new bool[ GetEKeysMaxIndex() + 1 ];
					keys[ (int)m.Key ] = true;
				}
			}

			//key up
			{
				var m = message as InputMessageKeyUp;
				if( m != null )
				{
					if( keys == null )
						keys = new bool[ GetEKeysMaxIndex() + 1 ];
					keys[ (int)m.Key ] = false;
				}
			}

			//mouse button down
			{
				var m = message as InputMessageMouseButtonDown;
				if( m != null )
					mouseButtons[ (int)m.Button ] = true;
			}

			//mouse button up
			{
				var m = message as InputMessageMouseButtonUp;
				if( m != null )
					mouseButtons[ (int)m.Button ] = false;
			}

			//mouse move
			{
				var m = message as InputMessageMouseMove;
				if( m != null )
					mousePosition = m.Position;
			}

			//mouse relative mode
			{
				var m = message as InputMessageMouseRelativeModeChanged;
				if( m != null )
					mouseRelativeMode = m.Value;
			}

			var keyDown = message as InputMessageKeyDown;
			if( keyDown != null && FreeCameraHotKey && FreeCameraHotKeyValue.Value == keyDown.Key )
			{
				var viewport = playScreen.ParentContainer.Viewport;
				viewport.KeysAndMouseButtonUpAll();
				viewport.NotifyInstantCameraMovement();

				//change free camera
				FreeCamera = !FreeCamera;

				//show screen message
				if( FreeCamera )
					ScreenMessages.Add( "Free camera is activated." );
				else
					ScreenMessages.Add( "Free camera is deactivated." );

				return true;
			}

			var mouseDown = message as InputMessageMouseButtonDown;
			if( mouseDown != null )
			{
				//free camera rotating
				if( mouseDown.Button == EMouseButtons.Right && FreeCamera )
				{
					freeCameraMouseRotating = true;
					freeCameraMouseRotatingActivated = false;
					freeCameraRotatingStartPos = MousePosition;
				}
			}

			var mouseUp = message as InputMessageMouseButtonUp;
			if( mouseUp != null )
			{
				//free camera rotating
				if( mouseUp.Button == EMouseButtons.Right && freeCameraMouseRotating )
				{
					//var viewport = playScreen.ParentContainer.Viewport;
					//viewport.MouseRelativeMode = false;
					freeCameraMouseRotating = false;
					freeCameraMouseRotatingActivated = false;
				}
			}

			var mouseMove = message as InputMessageMouseMove;
			if( mouseMove != null )
			{
				//third person camera update direction controlled by mouse
				if( EngineApp.IsSimulation && UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
				{
					if( !ThirdPersonCameraFollowDirection )
					{
						var viewport = playScreen.ParentContainer.Viewport;
						if( viewport.MouseRelativeMode )
						{
							var sensitivity = 2.0;

							var h = ThirdPersonCameraHorizontalAngle.Value - new Radian( MousePosition.X ).InDegrees() * sensitivity;
							if( h < 0 ) h += 360;
							if( h > 360 ) h -= 360;

							var v = ThirdPersonCameraVerticalAngle.Value + new Radian( MousePosition.Y ).InDegrees() * sensitivity;
							v = MathEx.Clamp( (double)v, -89.99, 89.99 );

							ThirdPersonCameraHorizontalAngle = h;
							ThirdPersonCameraVerticalAngle = v;
						}
					}
				}

				//free camera rotating
				if( FreeCamera && freeCameraMouseRotating )
				{
					var viewport = playScreen.ParentContainer.Viewport;

					if( !viewport.MouseRelativeMode )
					{
						var diffPixels = ( MousePosition - freeCameraRotatingStartPos ) * viewport.SizeInPixels.ToVector2();
						if( Math.Abs( diffPixels.X ) >= 3 || Math.Abs( diffPixels.Y ) >= 3 )
						{
							freeCameraMouseRotatingActivated = true;
							//viewport.MouseRelativeMode = true;
						}
					}
					else
					{
						var dir = freeCameraDirection;
						dir.Horizontal -= MousePosition.X;// * cameraRotateSensitivity;
						dir.Vertical -= MousePosition.Y;// * cameraRotateSensitivity;

						dir.Horizontal = MathEx.RadianNormalize360( dir.Horizontal );

						const double vlimit = Math.PI / 2 - .01f;
						if( dir.Vertical > vlimit ) dir.Vertical = vlimit;
						if( dir.Vertical < -vlimit ) dir.Vertical = -vlimit;

						freeCameraDirection = dir;
					}
				}
			}

			if( FreeCamera && !EngineConsole.Active && InputEnabled )
			{
				//key down
				{
					var m = message as InputMessageKeyDown;
					if( m != null )
					{
						if( m.Key == EKeys.W || m.Key == EKeys.Up || m.Key == EKeys.S || m.Key == EKeys.Down || m.Key == EKeys.A || m.Key == EKeys.Left || m.Key == EKeys.D || m.Key == EKeys.Right || m.Key == EKeys.E || m.Key == EKeys.Q )
							return true;
					}
				}

				//up down
				{
					var m = message as InputMessageKeyUp;
					if( m != null )
					{
						if( m.Key == EKeys.W || m.Key == EKeys.Up || m.Key == EKeys.S || m.Key == EKeys.Down || m.Key == EKeys.A || m.Key == EKeys.Left || m.Key == EKeys.D || m.Key == EKeys.Right || m.Key == EKeys.E || m.Key == EKeys.Q )
							return true;
					}
				}
			}

			return false;
		}

		public virtual bool ProcessInputMessage( InputMessage message )
		{
			if( ProcessInputMessageBefore( message ) )
				return true;

			InputMessageEvent?.Invoke( this, message );
			if( message.Handled )
				return true;

			//Object interaction
			if( ObjectInteractionContext != null && ObjectInteractionContext.Obj.ObjectInteractionInputMessage( this, message ) )
				return true;

			//InputProcessing
			var inputProcessing = GetInputProcessingOfCurrentObject();
			if( inputProcessing != null && inputProcessing.PerformMessage( this, message ) )
				return true;

			return false;
		}

		public InputProcessing GetInputProcessing( Component objectControlledByPlayer )
		{
			var input = objectControlledByPlayer as InputProcessing;
			if( input != null )
				return input;

			input = objectControlledByPlayer.GetComponent<InputProcessing>( onlyEnabledInHierarchy: true );
			if( input != null )
				return input;

			//create input processing if not exists
			if( EngineApp.IsSimulation )
			{
				var character = objectControlledByPlayer as Character;
				if( character != null )
				{
					var input2 = character.CreateComponent<CharacterInputProcessing>();
					return input2;
				}
				var vehicle = objectControlledByPlayer as Vehicle;
				if( vehicle != null )
				{
					var input2 = vehicle.CreateComponent<VehicleInputProcessing>();
					return input2;
				}
			}

			return null;
		}

		public InputProcessing GetInputProcessingOfCurrentObject()
		{
			var objectControlledByPlayer = ObjectControlledByPlayer.Value;
			if( objectControlledByPlayer != null )
			{
				var input = GetInputProcessing( objectControlledByPlayer );
				if( input != null )
					return input;
			}

			return null;
		}

		public CameraManagement GetCameraManagement( Component objectControlledByPlayer )
		{
			var m = objectControlledByPlayer as CameraManagement;
			if( m != null )
				return m;

			m = objectControlledByPlayer.GetComponent<CameraManagement>( onlyEnabledInHierarchy: true );
			if( m != null )
				return m;

			return null;
		}

		public CameraManagement GetCameraManagementOfCurrentObject()
		{
			var objectControlledByPlayer = ObjectControlledByPlayer.Value;
			if( objectControlledByPlayer != null )
			{
				var m = GetCameraManagement( objectControlledByPlayer );
				if( m != null )
					return m;
			}

			return null;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				//get free camera initial settings
				if( EngineApp.IsSimulation )
				{
					var scene = Parent as Scene;
					if( scene != null )
					{
						Camera camera = scene.CameraDefault;
						if( camera == null )
							camera = scene.Mode.Value == Scene.ModeEnum._2D ? scene.CameraEditor2D : scene.CameraEditor;

						if( camera != null )
						{
							var tr = camera.TransformV;
							freeCameraPosition = tr.Position;
							freeCameraDirection = SphericalDirection.FromVector( tr.Rotation.GetForward() );
						}
					}
				}

				//get third camera initial settings
				{
					var obj = ObjectControlledByPlayer.Value as ObjectInSpace;
					if( obj != null )
					{
						var direction = -obj.TransformV.Rotation.GetForward().ToVector2();
						ThirdPersonCameraHorizontalAngle = new Radian( Math.Atan2( direction.Y, direction.X ) ).InDegrees();
					}
				}

				//subscribe to scene Render event
				{
					var scene = FindParent<Scene>();
					if( scene != null )
						scene.RenderEvent += Scene_RenderEvent;
				}
			}
			else
			{
				//unsubscribe from scene Render event
				{
					var scene = FindParent<Scene>();
					if( scene != null )
						scene.RenderEvent -= Scene_RenderEvent;
				}

				if( EngineApp.IsSimulation )
					DeleteSceneSpecificScreenData();
			}
		}

		private void Scene_RenderEvent( Scene sender, Viewport viewport )
		{
			PerformRender( viewport );
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//cutscene fading update
			CutsceneGuiFadingFactor += CutsceneGuiFadingSpeed * Time.SimulationDelta * ( CutsceneStarted ? 1.0 : -1.0 );
			CutsceneGuiFadingFactor = MathEx.Saturate( CutsceneGuiFadingFactor );
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.IsSimulation && UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
			{
				//third person camera update horizontal direction
				if( ThirdPersonCameraFollowDirection )
				{
					var step = (double)ThirdPersonCameraFollowDirectionSpeed.Value.InRadians() * delta;
					if( step != 0 )
					{
						var obj = ObjectControlledByPlayer.Value as ObjectInSpace;
						if( obj != null )
						{
							var direction = obj.TransformV.Rotation.GetForward().ToVector2();
							if( direction != Vector2.Zero )
							{
								var demandedAngle = Math.Atan2( direction.Y, direction.X );
								var currentAngle = ThirdPersonCameraHorizontalAngle.Value.InRadians();

								var angle = demandedAngle - currentAngle;
								var d = new Vector2( Math.Cos( angle ), Math.Sin( angle ) );
								double factor = -d.Y;

								ThirdPersonCameraHorizontalAngle += new Radian( step * factor ).InDegrees();
							}
						}
					}
				}

				//update third person camera distance
				{
					var speed = 2.0;

					var distance = ThirdPersonCameraDistance.Value;

					if( IsKeyPressed( EKeys.PageUp ) )
						distance -= speed * delta;
					if( IsKeyPressed( EKeys.PageDown ) )
						distance += speed * delta;

					if( distance < 1 )
						distance = 1;

					ThirdPersonCameraDistance = distance;
				}

				//update third person camera height
				{
					var speed = 1.0;

					var height = ThirdPersonCameraHeight.Value;

					if( IsKeyPressed( EKeys.Home ) )
						height += speed * delta;
					if( IsKeyPressed( EKeys.End ) )
						height -= speed * delta;

					ThirdPersonCameraHeight = height;
				}
			}

			//moving free camera by keys
			if( FreeCamera && !EngineConsole.Active && InputEnabled )
			{
				double cameraVelocity = IsKeyPressed( EKeys.Shift ) ? FreeCameraSpeedFast : FreeCameraSpeedNormal;
				var step = cameraVelocity * delta;

				var pos = freeCameraPosition;
				var dir = freeCameraDirection;

				if( IsKeyPressed( EKeys.W ) || IsKeyPressed( EKeys.Up ) || IsKeyPressed( EKeys.NumPad8 ) )
					pos += dir.GetVector() * step;
				if( IsKeyPressed( EKeys.S ) || IsKeyPressed( EKeys.Down ) || IsKeyPressed( EKeys.NumPad2 ) )
					pos -= dir.GetVector() * step;
				if( IsKeyPressed( EKeys.A ) || IsKeyPressed( EKeys.Left ) || IsKeyPressed( EKeys.NumPad4 ) )
					pos += new SphericalDirection( dir.Horizontal + Math.PI / 2, 0 ).GetVector() * step;
				if( IsKeyPressed( EKeys.D ) || IsKeyPressed( EKeys.Right ) || IsKeyPressed( EKeys.NumPad6 ) )
					pos += new SphericalDirection( dir.Horizontal - Math.PI / 2, 0 ).GetVector() * step;
				if( IsKeyPressed( EKeys.E ) )
					pos += new SphericalDirection( dir.Horizontal, dir.Vertical + Math.PI / 2 ).GetVector() * step;
				if( IsKeyPressed( EKeys.Q ) )
					pos += new SphericalDirection( dir.Horizontal, dir.Vertical - Math.PI / 2 ).GetVector() * step;

				freeCameraPosition = pos;
			}

			if( freeCameraMouseRotating && !FreeCamera )
			{
				freeCameraMouseRotating = false;
				freeCameraMouseRotatingActivated = false;
			}
		}

		[Browsable( false )]
		public bool InputEnabled
		{
			get { return inputEnabled; }
		}

		public bool IsKeyPressed( EKeys key )
		{
			if( keys != null )
				return keys[ (int)key ];
			else
				return false;
		}

		public bool IsMouseButtonPressed( EMouseButtons button )
		{
			return mouseButtons[ (int)button ];
		}

		[Browsable( false )]
		public Vector2 MousePosition
		{
			get { return mousePosition; }
		}

		[Browsable( false )]
		public bool MouseRelativeMode
		{
			get { return mouseRelativeMode; }
		}

		//public delegate void IsNeedMouseRelativeModeEventDelegate( GameMode sender, ref bool needMouseRelativeMode );
		//public event IsNeedMouseRelativeModeEventDelegate IsNeedMouseRelativeModeEvent;

		public bool IsNeedMouseRelativeMode()
		{
			bool result = false;

			if( FreeCamera )
			{
				//free camera
				if( freeCameraMouseRotating && freeCameraMouseRotatingActivated )
					result = true;
			}
			else
			{
				//built-in camera
				if( ObjectControlledByPlayer.Value != null )
				{
					if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
						result = true;
				}
			}

			//CameraManagement
			var m = GetCameraManagementOfCurrentObject();
			if( m != null )
				result = m.IsNeedMouseRelativeMode( this );

			//IsNeedMouseRelativeModeEvent?.Invoke( this, ref result );

			return result;
		}

		public delegate void RenderDelegate( GameMode sender, Viewport viewport );
		public event RenderDelegate Render;

		public virtual void PerformRender( Viewport viewport )
		{
			Render?.Invoke( this, viewport );

			if( Scene != null && EngineApp.IsSimulation )
			{
				UpdateObjectInteraction( viewport );
				UpdateSceneSpecificScreenData( viewport );
			}
		}

		public delegate void RenderUIDelegate( GameMode sender, CanvasRenderer renderer );
		public event RenderUIDelegate RenderUI;

		public virtual void PerformRenderUI( CanvasRenderer renderer )
		{
			RenderUI?.Invoke( this, renderer );

			if( Scene != null && EngineApp.IsSimulation )
			{
				RenderObjectInteraction( renderer );
				RenderTargetImage( renderer );
			}

			//if( DisplayHelpText )
			//{
			//	var lines = new List<string>();
			//	lines.Add( "Game Mode" );
			//	if( CanChangeCameraTypeByKey )
			//		lines.Add( "Press F7 to change camera type." );

			//	var fontSize = renderer.DefaultFontSize;
			//	var offset = new Vector2( fontSize * renderer.AspectRatioInv * 0.8, fontSize * 0.6 );

			//	CanvasRendererUtility.AddTextLinesWithShadow( renderer.ViewportForScreenCanvasRenderer, null, fontSize, lines, new Rectangle( offset.X, offset.Y, 1.0 - offset.X, 1.0 - offset.Y ), EHorizontalAlignment.Right, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
			//}
		}

		[Browsable( false )]
		public Scene Scene
		{
			get { return Parent as Scene; }
		}

		public delegate void GetInteractiveObjectInfoEventDelegate( GameMode sender, InteractiveObject obj, ref InteractiveObjectObjectInfo result );
		public event GetInteractiveObjectInfoEventDelegate GetInteractiveObjectInfoEvent;

		public virtual InteractiveObjectObjectInfo GetInteractiveObjectInfo( InteractiveObject obj )
		{
			InteractiveObjectObjectInfo result = null;
			obj.ObjectInteractionGetInfo( this, ref result );
			GetInteractiveObjectInfoEvent?.Invoke( this, obj, ref result );
			if( result == null )
				result = new InteractiveObjectObjectInfo();
			return result;
		}

		public delegate void PickInteractiveObjectEventDelegate( GameMode gameMode, Viewport viewport, ref InteractiveObject result );
		public event PickInteractiveObjectEventDelegate PickInteractiveObjectEvent;

		public virtual InteractiveObject PickInteractiveObject( Viewport viewport )
		{
			InteractiveObject result = null;
			//PickInteractiveObjectEvent?.Invoke( this, viewport, ref result );

			if( !FreeCamera && !CutsceneStarted )
			{
				//pick for 3D
				if( Scene.Mode.Value == Scene.ModeEnum._3D )
				{
					if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson )//|| UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
					{
						Ray ray;
						{
							double rayDistance = 2.5;
							//scaling
							{
								var obj = ObjectControlledByPlayer.Value as ObjectInSpace;
								if( obj != null )
									rayDistance *= obj.TransformV.Scale.MaxComponent();
							}

							ray = viewport.CameraSettings.GetRayByScreenCoordinates( new Vector2( 0.5, 0.5 ) );
							ray.Direction = ray.Direction.GetNormalize() * rayDistance;
						}

						var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, ray );
						Scene.GetObjectsInSpace( getObjectsItem );

						foreach( var item in getObjectsItem.Result )
						{
							if( item.Object is Camera || item.Object is Sensor || item.Object is Sensor2D )
								continue;

							if( item.Object.SpaceBounds.BoundingSphere.Intersects( ray ) )
							{
								var obj = item.Object.FindThisOrParent<InteractiveObject>();
								if( obj != null )
								{
									if( GetInteractiveObjectInfo( obj ).AllowInteract )
										result = obj;
								}
							}
						}
					}

					if( UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
					{
						var controlledObject = ObjectControlledByPlayer.Value as ObjectInSpace;
						if( controlledObject != null )
						{
							var distance = 1.5;

							var bounds = controlledObject.SpaceBounds.BoundingBox;
							var offsetsRange = new Range( bounds.Minimum.Z - controlledObject.TransformV.Position.Z, bounds.Maximum.Z - controlledObject.TransformV.Position.Z );
							//var offsetsRange = new Range( -0.25, 0.5 );

							for( var factor = 0.0; factor <= 1.001; factor += 0.1 )
							{
								var offset = MathEx.Lerp( offsetsRange.Minimum, offsetsRange.Maximum, factor );
								Ray ray = new Ray( controlledObject.TransformV.Position + new Vector3( 0, 0, offset ), controlledObject.TransformV.Rotation.GetForward() * distance );

								var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, ray );
								Scene.GetObjectsInSpace( getObjectsItem );

								foreach( var item in getObjectsItem.Result )
								{
									if( item.Object is Camera || item.Object is Sensor || item.Object is Sensor2D )
										continue;

									if( item.Object.SpaceBounds.BoundingSphere.Intersects( ray, out var scale1, out var scale2 ) )
									{
										var scale = Math.Min( scale1, scale2 );
										if( ( ray.Direction * scale ).Length() < distance )
										{
											var obj = item.Object.FindThisOrParent<InteractiveObject>();
											if( obj != null )
											{
												if( GetInteractiveObjectInfo( obj ).AllowInteract )
													result = obj;
											}
										}
									}
								}
							}
						}
					}
				}

				//pick for 2D
				if( Scene.Mode.Value == Scene.ModeEnum._2D )
				{
					if( UseBuiltInCamera.Value == BuiltInCameraEnum.None || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
					{
						var character = ObjectControlledByPlayer.Value as Character2D;
						if( character != null )
						{
							double maxDistance = 2.0;
							//scaling
							{
								var obj = ObjectControlledByPlayer.Value as ObjectInSpace;
								if( obj != null )
									maxDistance *= obj.TransformV.Scale.MaxComponent();
							}

							var bounds = new Bounds( character.TransformV.Position );
							bounds.Expand( new Vector3( maxDistance, maxDistance, 10000 ) );

							var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, bounds );
							Scene.GetObjectsInSpace( getObjectsItem );

							foreach( var item in getObjectsItem.Result )
							{
								var obj = item.Object.FindThisOrParent<InteractiveObject>();
								if( obj != null )
								{
									var objectInSpace = obj as ObjectInSpace;
									if( objectInSpace != null )
									{
										var distance = ( objectInSpace.TransformV.Position - character.TransformV.Position ).Length();
										if( distance <= maxDistance )
										{
											if( GetInteractiveObjectInfo( obj ).AllowInteract )
												result = obj;
										}
									}
								}
							}
						}
					}
				}
			}

			//CameraManagement
			var m = GetCameraManagementOfCurrentObject();
			if( m != null )
				result = m.PickInteractiveObject( this, viewport );

			PickInteractiveObjectEvent?.Invoke( this, viewport, ref result );

			return result;
		}

		protected virtual void UpdateObjectInteraction( Viewport viewport )
		{
			//find interactive object on the screen
			var overObject = PickInteractiveObject( viewport );

			//check to update
			if( ObjectInteractionContext == null || ObjectInteractionContext.Obj != overObject )
			{
				//end old context
				if( ObjectInteractionContext != null )
				{
					ObjectInteractionContext.Obj.ObjectInteractionExit( ObjectInteractionContext );
					ObjectInteractionContext.Dispose();
					ObjectInteractionContext = null;
				}

				//create new
				if( overObject != null )
				{
					ObjectInteractionContext = new ObjectInteractionContext( overObject, this, viewport );
					ObjectInteractionContext.Obj.ObjectInteractionEnter( ObjectInteractionContext );
				}
			}

			//update context
			if( ObjectInteractionContext != null )
			{
				ObjectInteractionContext.Viewport = viewport;
				ObjectInteractionContext.Obj.ObjectInteractionUpdate( ObjectInteractionContext );
			}
		}

		protected virtual void RenderObjectInteraction( CanvasRenderer renderer )
		{
			if( !FreeCamera && ObjectInteractionContext != null && !CutsceneStarted )
			{
				bool render = false;
				if( Scene.Mode.Value == Scene.ModeEnum._3D )
					if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
						render = true;
				if( Scene.Mode.Value == Scene.ModeEnum._2D )
					if( UseBuiltInCamera.Value == BuiltInCameraEnum.None || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
						render = true;
				var m = GetCameraManagementOfCurrentObject();
				if( m != null )
					render = m.IsNeedRenderObjectInteraction( this, renderer );

				if( render )
				{
					var obj = ObjectInteractionContext.Obj as ObjectInSpace;
					if( obj != null )
					{
						var info = GetInteractiveObjectInfo( ObjectInteractionContext.Obj );

						if( info.AllowInteract && info.DisplaySelectionRectangle )
						{
							//calculate screen rectangle
							var rectangle = Rectangle.Cleared;
							foreach( var point in obj.SpaceBounds.BoundingBox.ToPoints() )
							{
								if( renderer.ViewportForScreenCanvasRenderer.CameraSettings.ProjectToScreenCoordinates( point, out var screenPosition ) )
									rectangle.Add( screenPosition );
							}

							if( !rectangle.IsCleared() )
							{
								//expand rectangle
								{
									var multiplier = 1.3;
									var center = rectangle.GetCenter();
									var size = rectangle.GetSize();
									rectangle = new Rectangle( center );
									rectangle.Expand( size / 2 * multiplier );
								}

								var color = new ColorValue( 1, 1, 0, 0.5 );

								//add rectangle
								var thickness = new Vector2( 0.004, 0.004 * renderer.AspectRatio );
								var inner = rectangle;
								inner.Expand( -thickness );
								renderer.AddThickRectangle( rectangle, inner, color );

								//add text
								renderer.AddTextLines( info.SelectionTextInfo, new Vector2( rectangle.GetCenter().X, rectangle.Bottom ), EHorizontalAlignment.Center, EVerticalAlignment.Top, 0, color );
							}
						}
					}
				}
			}
		}

		public delegate void RenderTargetImageBeforeDelegate( GameMode sender, ref bool show );
		public event RenderTargetImageBeforeDelegate RenderTargetImageBefore;

		protected virtual void RenderTargetImage( CanvasRenderer renderer )
		{
			var display = DisplayTarget.Value;
			if( display == AutoTrueFalse.Auto )
				display = UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson ? AutoTrueFalse.True : AutoTrueFalse.False;

			if( display == AutoTrueFalse.True && ( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson || GetCameraManagementOfCurrentObject() != null ) && !FreeCamera && Scene.Mode.Value == Scene.ModeEnum._3D && !CutsceneStarted )
			{
				var show = true;
				RenderTargetImageBefore?.Invoke( this, ref show );
				if( show )
				{
					//if( weapon != null || currentAttachedGuiObject != null || currentSwitch != null )
					//{
					var size = DisplayTargetSize.Value / 2;
					var rectangle = new Rectangle(
						0.5 - size, 0.5 - size * renderer.AspectRatio,
						0.5 + size, 0.5 + size * renderer.AspectRatio );
					renderer.AddQuad( rectangle, new Rectangle( 0, 0, 1, 1 ), DisplayTargetImage, DisplayTargetColor );
					//}
				}
			}
		}

		[Browsable( false )]
		public Vector3 FreeCameraPosition
		{
			get { return freeCameraPosition; }
		}

		[Browsable( false )]
		public SphericalDirection FreeCameraDirection
		{
			get { return freeCameraDirection; }
		}

		public void CutsceneStart( double guiFadingSpeed = 1.0 )
		{
			CutsceneStarted = true;
			CutsceneGuiFadingSpeed = guiFadingSpeed;

			CutsceneText = "";
		}

		public void CutsceneEnd( double guiFadingSpeed = 1.0 )
		{
			CutsceneStarted = false;
			CutsceneGuiFadingSpeed = guiFadingSpeed;
		}

		public void DoScreenFading( ColorValue color, double speed = 1.0 )
		{
			ScreenFadingTargetColor = color;
			ScreenFadingSpeed = speed;
		}

		public void SetReplaceCamera( Camera camera )
		{
			ReplaceCamera = camera;
		}

		public void SetReplaceCamera( Reference<Camera> camera )
		{
			ReplaceCamera = camera;
		}

		public void ResetReplaceCamera()
		{
			ReplaceCamera = null;
		}

		public delegate void ShowControlledObjectDelegate( GameMode sender, Viewport viewport, ref bool show );
		public event ShowControlledObjectDelegate ShowControlledObject;

		void UpdateControlledObjectVisibility( Viewport viewport )
		{
			var obj = ObjectControlledByPlayer.Value as ObjectInSpace;
			if( obj != null )
			{
				var meshInSpace = obj as MeshInSpace;
				if( meshInSpace == null )
					meshInSpace = obj.GetComponent<MeshInSpace>();

				if( meshInSpace != null )
				{
					var firstPersonCamera = UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson && !FreeCamera;

					var show = true;
					if( firstPersonCamera && !FirstPersonCameraShowControlledObject )
						show = false;
					ShowControlledObject?.Invoke( this, viewport, ref show );

					meshInSpace.Visible = show;

					if( firstPersonCamera && show && FirstPersonCameraCutVolumeRadius.Value > 0 && viewport != null )
					{
						var cutVolume = new RenderingPipeline.RenderSceneData.CutVolumeItem();
						cutVolume.Shape = CutVolumeShape.Sphere;
						cutVolume.Flags = CutVolumeFlags.CutScene;
						//cutVolume.CutScene = true;
						//cutVolume.CutShadows = false;

						var scl = FirstPersonCameraCutVolumeRadius.Value * 2;
						cutVolume.Transform = new Transform( viewport.CameraSettings.Position, Quaternion.Identity, new Vector3( scl, scl, scl ) );

						meshInSpace.CutVolumes = new RenderingPipeline.RenderSceneData.CutVolumeItem[] { cutVolume };
					}
					else
						meshInSpace.CutVolumes = null;
				}
			}
		}

		static void UpdateControlledObjectVisibilitySetToDefault( Component objectControlledByPlayer )
		{
			var obj = objectControlledByPlayer as ObjectInSpace;
			if( obj != null )
			{
				var meshInSpace = obj as MeshInSpace;
				if( meshInSpace == null )
					meshInSpace = obj.GetComponent<MeshInSpace>();

				if( meshInSpace != null )
				{
					meshInSpace.Visible = true;
					meshInSpace.CutVolumes = null;
				}
			}
		}

		void UpdateSceneSpecificScreenData( Viewport viewport )
		{
			//update controlled object
			UpdateControlledObjectVisibility( viewport );

			//update character data for first person camera before frame rendering
			if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson && !FreeCamera )
			{
				var character = ObjectControlledByPlayer.Value as Character;
				if( character != null )
					character.UpdateDataForFirstPersonCamera( this, viewport );
			}

			//update character data for third person camera before frame rendering
			if( UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson && !FreeCamera )
			{
				var character = ObjectControlledByPlayer.Value as Character;
				if( character != null )
				{
					if( !ThirdPersonCameraFollowDirection )
					{
						//!!!!here?

						var d = new SphericalDirection( MathEx.DegreeToRadian( ThirdPersonCameraHorizontalAngle.Value + 180 ), MathEx.DegreeToRadian( ThirdPersonCameraVerticalAngle.Value ) );

						character.TurnToDirection( d.ToSphericalDirectionF(), true );
					}
				}
			}
		}

		void DeleteSceneSpecificScreenData()
		{
		}

		public static UIControl PlayScreen
		{
			get { return playScreen; }
		}

		public static void UpdatePlayScreen( UIControl newValue )
		{
			playScreen = newValue;
		}
	}
}
