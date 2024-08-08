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
	public class GameMode : Component, IGameMode
	{
		static UIControl playScreen;
		public static double MouseSensitivity = 1;

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

				if( _objectControlledByPlayer.BeginSet( this, ref value ) )
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

								if( !NetworkIsClient )
									UpdateControlledObjectVisibilitySetToDefault( oldObject );

								//reset motion blur factor for first person camera
								var meshInSpace = oldObject as MeshInSpace;
								if( meshInSpace != null )
									meshInSpace.MotionBlurFactor = 1;
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
			set { if( _useBuiltInCamera.BeginSet( this, ref value ) ) { try { UseBuiltInCameraChanged?.Invoke( this ); } finally { _useBuiltInCamera.EndSet(); } } }
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
			set { if( _firstPersonCameraShowControlledObject.BeginSet( this, ref value ) ) { try { FirstPersonCameraShowControlledObjectChanged?.Invoke( this ); } finally { _firstPersonCameraShowControlledObject.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FirstPersonCameraShowControlledObject"/> property value changes.</summary>
		public event Action<GameMode> FirstPersonCameraShowControlledObjectChanged;
		ReferenceField<bool> _firstPersonCameraShowControlledObject = true;

		/// <summary>
		/// The radius of cut geometry from the camera to prevent drawing the internal part of the controlled object 3D model.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 0.15 )]//0.2 )]
		[Range( 0, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> FirstPersonCameraCutVolumeRadius
		{
			get { if( _firstPersonCameraCutVolumeRadius.BeginGet() ) FirstPersonCameraCutVolumeRadius = _firstPersonCameraCutVolumeRadius.Get( this ); return _firstPersonCameraCutVolumeRadius.value; }
			set { if( _firstPersonCameraCutVolumeRadius.BeginSet( this, ref value ) ) { try { FirstPersonCameraCutVolumeRadiusChanged?.Invoke( this ); } finally { _firstPersonCameraCutVolumeRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FirstPersonCameraCutVolumeRadius"/> property value changes.</summary>
		public event Action<GameMode> FirstPersonCameraCutVolumeRadiusChanged;
		ReferenceField<double> _firstPersonCameraCutVolumeRadius = 0.15;//0.2;

		/// <summary>
		/// Whether to attach the camera in the first person mode to the eyes of the controlled object.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( true )]
		public Reference<bool> FirstPersonCameraAttachToEyes
		{
			get { if( _firstPersonCameraAttachToEyes.BeginGet() ) FirstPersonCameraAttachToEyes = _firstPersonCameraAttachToEyes.Get( this ); return _firstPersonCameraAttachToEyes.value; }
			set { if( _firstPersonCameraAttachToEyes.BeginSet( this, ref value ) ) { try { FirstPersonCameraAttachToEyesChanged?.Invoke( this ); } finally { _firstPersonCameraAttachToEyes.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FirstPersonCameraAttachToEyes"/> property value changes.</summary>
		public event Action<GameMode> FirstPersonCameraAttachToEyesChanged;
		ReferenceField<bool> _firstPersonCameraAttachToEyes = true;

		/// <summary>
		/// The distance of object picking for first person camera.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 2.0 )]
		public Reference<double> FirstPersonCameraPickInteractiveObjectDistance
		{
			get { if( _firstPersonCameraPickInteractiveObjectDistance.BeginGet() ) FirstPersonCameraPickInteractiveObjectDistance = _firstPersonCameraPickInteractiveObjectDistance.Get( this ); return _firstPersonCameraPickInteractiveObjectDistance.value; }
			set { if( _firstPersonCameraPickInteractiveObjectDistance.BeginSet( this, ref value ) ) { try { FirstPersonCameraPickInteractiveObjectDistanceChanged?.Invoke( this ); } finally { _firstPersonCameraPickInteractiveObjectDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FirstPersonCameraPickInteractiveObjectDistance"/> property value changes.</summary>
		public event Action<GameMode> FirstPersonCameraPickInteractiveObjectDistanceChanged;
		ReferenceField<double> _firstPersonCameraPickInteractiveObjectDistance = 2.0;

		///// <summary>
		///// Whether to change the position of the third person camera depending on the direction of the object.
		///// </summary>
		//[Category( "Camera" )]
		//[DefaultValue( false )]
		//public Reference<bool> ThirdPersonCameraFollowDirection
		//{
		//	get { if( _thirdPersonCameraFollowDirection.BeginGet() ) ThirdPersonCameraFollowDirection = _thirdPersonCameraFollowDirection.Get( this ); return _thirdPersonCameraFollowDirection.value; }
		//	set { if( _thirdPersonCameraFollowDirection.BeginSet( this, ref value ) ) { try { ThirdPersonCameraFollowDirectionChanged?.Invoke( this ); } finally { _thirdPersonCameraFollowDirection.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ThirdPersonCameraFollowDirection"/> property value changes.</summary>
		//public event Action<GameMode> ThirdPersonCameraFollowDirectionChanged;
		//ReferenceField<bool> _thirdPersonCameraFollowDirection = false;

		///// <summary>
		///// Third-person camera rotation speed. Degree per second.
		///// </summary>
		//[Category( "Camera" )]
		//[DefaultValue( 180.0 )]
		//[Range( 0, 360 )]
		//public Reference<Degree> ThirdPersonCameraFollowDirectionSpeed
		//{
		//	get { if( _thirdPersonCameraFollowDirectionSpeed.BeginGet() ) ThirdPersonCameraFollowDirectionSpeed = _thirdPersonCameraFollowDirectionSpeed.Get( this ); return _thirdPersonCameraFollowDirectionSpeed.value; }
		//	set { if( _thirdPersonCameraFollowDirectionSpeed.BeginSet( this, ref value ) ) { try { ThirdPersonCameraFollowDirectionSpeedChanged?.Invoke( this ); } finally { _thirdPersonCameraFollowDirectionSpeed.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ThirdPersonCameraFollowDirectionSpeed"/> property value changes.</summary>
		//public event Action<GameMode> ThirdPersonCameraFollowDirectionSpeedChanged;
		//ReferenceField<Degree> _thirdPersonCameraFollowDirectionSpeed = new Degree( 180 );

		/// <summary>
		/// The horizontal angle of the camera in third-person mode.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 60 )]
		[Range( 0, 360 )]
		public Reference<Degree> ThirdPersonCameraHorizontalAngle
		{
			get { if( _thirdPersonCameraHorizontalAngle.BeginGet() ) ThirdPersonCameraHorizontalAngle = _thirdPersonCameraHorizontalAngle.Get( this ); return _thirdPersonCameraHorizontalAngle.value; }
			set { if( _thirdPersonCameraHorizontalAngle.BeginSet( this, ref value ) ) { try { ThirdPersonCameraHorizontalAngleChanged?.Invoke( this ); } finally { _thirdPersonCameraHorizontalAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraHorizontalAngle"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraHorizontalAngleChanged;
		ReferenceField<Degree> _thirdPersonCameraHorizontalAngle = new Degree( 60 );

		/// <summary>
		/// The vertical angle of the camera in third-person mode.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( -30 )]
		[Range( -80.0, 80.0 )]//[Range( -89.99, 89.99 )]
		public Reference<Degree> ThirdPersonCameraVerticalAngle
		{
			get { if( _thirdPersonCameraVerticalAngle.BeginGet() ) ThirdPersonCameraVerticalAngle = _thirdPersonCameraVerticalAngle.Get( this ); return _thirdPersonCameraVerticalAngle.value; }
			set { if( _thirdPersonCameraVerticalAngle.BeginSet( this, ref value ) ) { try { ThirdPersonCameraVerticalAngleChanged?.Invoke( this ); } finally { _thirdPersonCameraVerticalAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraVerticalAngle"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraVerticalAngleChanged;
		ReferenceField<Degree> _thirdPersonCameraVerticalAngle = new Degree( -30 );

		/// <summary>
		/// The distance of the camera in third-person mode.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 5.0 )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> ThirdPersonCameraDistance
		{
			get { if( _thirdPersonCameraDistance.BeginGet() ) ThirdPersonCameraDistance = _thirdPersonCameraDistance.Get( this ); return _thirdPersonCameraDistance.value; }
			set { if( _thirdPersonCameraDistance.BeginSet( this, ref value ) ) { try { ThirdPersonCameraDistanceChanged?.Invoke( this ); } finally { _thirdPersonCameraDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraDistance"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraDistanceChanged;
		ReferenceField<double> _thirdPersonCameraDistance = 5;

		/// <summary>
		/// The height of the camera in third-person mode.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 1.5 )]
		[Range( -20, 20 )]
		public Reference<double> ThirdPersonCameraHeight
		{
			get { if( _thirdPersonCameraHeight.BeginGet() ) ThirdPersonCameraHeight = _thirdPersonCameraHeight.Get( this ); return _thirdPersonCameraHeight.value; }
			set { if( _thirdPersonCameraHeight.BeginSet( this, ref value ) ) { try { ThirdPersonCameraHeightChanged?.Invoke( this ); } finally { _thirdPersonCameraHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraHeight"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraHeightChanged;
		ReferenceField<double> _thirdPersonCameraHeight = 1.5;

		/// <summary>
		/// The horizontal offset of the camera in third-person mode.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 0.0 )]
		[Range( -5, 5 )]
		public Reference<double> ThirdPersonCameraLeft
		{
			get { if( _thirdPersonCameraLeft.BeginGet() ) ThirdPersonCameraLeft = _thirdPersonCameraLeft.Get( this ); return _thirdPersonCameraLeft.value; }
			set { if( _thirdPersonCameraLeft.BeginSet( this, ref value ) ) { try { ThirdPersonCameraLeftChanged?.Invoke( this ); } finally { _thirdPersonCameraLeft.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraLeft"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraLeftChanged;
		ReferenceField<double> _thirdPersonCameraLeft = 0.0;

		/// <summary>
		/// The distance of object picking for third person camera.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 2.0 )]
		public Reference<double> ThirdPersonCameraPickInteractiveObjectDistance
		{
			get { if( _thirdPersonCameraPickInteractiveObjectDistance.BeginGet() ) ThirdPersonCameraPickInteractiveObjectDistance = _thirdPersonCameraPickInteractiveObjectDistance.Get( this ); return _thirdPersonCameraPickInteractiveObjectDistance.value; }
			set { if( _thirdPersonCameraPickInteractiveObjectDistance.BeginSet( this, ref value ) ) { try { ThirdPersonCameraPickInteractiveObjectDistanceChanged?.Invoke( this ); } finally { _thirdPersonCameraPickInteractiveObjectDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraPickInteractiveObjectDistance"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraPickInteractiveObjectDistanceChanged;
		ReferenceField<double> _thirdPersonCameraPickInteractiveObjectDistance = 2.0;

		[Category( "Camera" )]
		[DefaultValue( true )]
		public Reference<bool> ThirdPersonCameraAllowLookToBackWhenNoActiveItem
		{
			get { if( _thirdPersonCameraAllowLookToBackWhenNoActiveItem.BeginGet() ) ThirdPersonCameraAllowLookToBackWhenNoActiveItem = _thirdPersonCameraAllowLookToBackWhenNoActiveItem.Get( this ); return _thirdPersonCameraAllowLookToBackWhenNoActiveItem.value; }
			set { if( _thirdPersonCameraAllowLookToBackWhenNoActiveItem.BeginSet( this, ref value ) ) { try { ThirdPersonCameraAllowLookToBackWhenNoActiveItemChanged?.Invoke( this ); } finally { _thirdPersonCameraAllowLookToBackWhenNoActiveItem.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraAllowLookToBackWhenNoActiveItem"/> property value changes.</summary>
		public event Action<GameMode> ThirdPersonCameraAllowLookToBackWhenNoActiveItemChanged;
		ReferenceField<bool> _thirdPersonCameraAllowLookToBackWhenNoActiveItem = true;

		/// <summary>
		/// The distance of object picking for 2D camera.
		/// </summary>
		[Category( "Camera" )]
		[DisplayName( "Camera 2D Pick Interactive Object Distance" )]
		[DefaultValue( 1.5 )]
		public Reference<double> Camera2DPickInteractiveObjectDistance
		{
			get { if( _camera2DPickInteractiveObjectDistance.BeginGet() ) Camera2DPickInteractiveObjectDistance = _camera2DPickInteractiveObjectDistance.Get( this ); return _camera2DPickInteractiveObjectDistance.value; }
			set { if( _camera2DPickInteractiveObjectDistance.BeginSet( this, ref value ) ) { try { Camera2DPickInteractiveObjectDistanceChanged?.Invoke( this ); } finally { _camera2DPickInteractiveObjectDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Camera2DPickInteractiveObjectDistance"/> property value changes.</summary>
		public event Action<GameMode> Camera2DPickInteractiveObjectDistanceChanged;
		ReferenceField<double> _camera2DPickInteractiveObjectDistance = 1.5;

		/// <summary>
		/// Whether is a free camera enabled.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( false )]
		public Reference<bool> FreeCamera
		{
			get { if( _freeCamera.BeginGet() ) FreeCamera = _freeCamera.Get( this ); return _freeCamera.value; }
			set { if( _freeCamera.BeginSet( this, ref value ) ) { try { FreeCameraChanged?.Invoke( this ); } finally { _freeCamera.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FreeCamera"/> property value changes.</summary>
		public event Action<GameMode> FreeCameraChanged;
		ReferenceField<bool> _freeCamera = false;

		/// <summary>
		/// The key code to enable a free camera.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( EKeys.F7 )]
		public Reference<EKeys> FreeCameraKey
		{
			get { if( _freeCameraKey.BeginGet() ) FreeCameraKey = _freeCameraKey.Get( this ); return _freeCameraKey.value; }
			set { if( _freeCameraKey.BeginSet( this, ref value ) ) { try { FreeCameraKeyChanged?.Invoke( this ); } finally { _freeCameraKey.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FreeCameraKey"/> property value changes.</summary>
		public event Action<GameMode> FreeCameraKeyChanged;
		ReferenceField<EKeys> _freeCameraKey = EKeys.F7;

		/// <summary>
		/// The normal speed of a free camera.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 3 )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> FreeCameraSpeedNormal
		{
			get { if( _freeCameraSpeedNormal.BeginGet() ) FreeCameraSpeedNormal = _freeCameraSpeedNormal.Get( this ); return _freeCameraSpeedNormal.value; }
			set { if( _freeCameraSpeedNormal.BeginSet( this, ref value ) ) { try { FreeCameraSpeedNormalChanged?.Invoke( this ); } finally { _freeCameraSpeedNormal.EndSet(); } } }
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
			set { if( _freeCameraSpeedFast.BeginSet( this, ref value ) ) { try { FreeCameraSpeedFastChanged?.Invoke( this ); } finally { _freeCameraSpeedFast.EndSet(); } } }
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
			set { if( _displayTarget.BeginSet( this, ref value ) ) { try { DisplayTargetChanged?.Invoke( this ); } finally { _displayTarget.EndSet(); } } }
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
			set { if( _displayTargetSize.BeginSet( this, ref value ) ) { try { DisplayTargetSizeChanged?.Invoke( this ); } finally { _displayTargetSize.EndSet(); } } }
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
			set { if( _displayTargetImage.BeginSet( this, ref value ) ) { try { DisplayTargetImageChanged?.Invoke( this ); } finally { _displayTargetImage.EndSet(); } } }
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
			set { if( _displayTargetColor.BeginSet( this, ref value ) ) { try { DisplayTargetColorChanged?.Invoke( this ); } finally { _displayTargetColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayTargetColor"/> property value changes.</summary>
		public event Action<GameMode> DisplayTargetColorChanged;
		ReferenceField<ColorValue> _displayTargetColor = new ColorValue( 1, 1, 1, 0.5 );

		/// <summary>
		/// Whether to display a UI control with the player object's inventory.
		/// </summary>
		[Category( "Inventory" )]
		[DefaultValue( false )]
		public Reference<bool> InventoryWidget
		{
			get { if( _inventoryWidget.BeginGet() ) InventoryWidget = _inventoryWidget.Get( this ); return _inventoryWidget.value; }
			set { if( _inventoryWidget.BeginSet( this, ref value ) ) { try { InventoryWidgetChanged?.Invoke( this ); } finally { _inventoryWidget.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InventoryWidget"/> property value changes.</summary>
		public event Action<GameMode> InventoryWidgetChanged;
		ReferenceField<bool> _inventoryWidget = false;

		/// <summary>
		/// Whether the ability to have several weapons in the character's inventory.
		/// </summary>
		[Category( "Inventory" )]
		[DefaultValue( true )]
		public Reference<bool> InventoryCharacterCanHaveSeveralWeapons
		{
			get { if( _inventoryCharacterCanHaveSeveralWeapons.BeginGet() ) InventoryCharacterCanHaveSeveralWeapons = _inventoryCharacterCanHaveSeveralWeapons.Get( this ); return _inventoryCharacterCanHaveSeveralWeapons.value; }
			set { if( _inventoryCharacterCanHaveSeveralWeapons.BeginSet( this, ref value ) ) { try { InventoryCharacterCanHaveSeveralWeaponsChanged?.Invoke( this ); } finally { _inventoryCharacterCanHaveSeveralWeapons.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InventoryCharacterCanHaveSeveralWeapons"/> property value changes.</summary>
		public event Action<GameMode> InventoryCharacterCanHaveSeveralWeaponsChanged;
		ReferenceField<bool> _inventoryCharacterCanHaveSeveralWeapons = true;

		/// <summary>
		/// Whether the ability to have several weapons of same type in the character's inventory.
		/// </summary>
		[Category( "Inventory" )]
		[DefaultValue( false )]
		public Reference<bool> InventoryCharacterCanHaveSeveralWeaponsOfSameType
		{
			get { if( _inventoryCharacterCanHaveSeveralWeaponsOfSameType.BeginGet() ) InventoryCharacterCanHaveSeveralWeaponsOfSameType = _inventoryCharacterCanHaveSeveralWeaponsOfSameType.Get( this ); return _inventoryCharacterCanHaveSeveralWeaponsOfSameType.value; }
			set { if( _inventoryCharacterCanHaveSeveralWeaponsOfSameType.BeginSet( this, ref value ) ) { try { InventoryCharacterCanHaveSeveralWeaponsOfSameTypeChanged?.Invoke( this ); } finally { _inventoryCharacterCanHaveSeveralWeaponsOfSameType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InventoryCharacterCanHaveSeveralWeaponsOfSameType"/> property value changes.</summary>
		public event Action<GameMode> InventoryCharacterCanHaveSeveralWeaponsOfSameTypeChanged;
		ReferenceField<bool> _inventoryCharacterCanHaveSeveralWeaponsOfSameType = false;

		//!!!!impl
		///// <summary>
		///// The ability to use the inventory window.
		///// </summary>
		//[Category( "Inventory" )]
		//[DefaultValue( true )]
		//public Reference<bool> InventoryWindow
		//{
		//	get { if( _inventoryWindow.BeginGet() ) InventoryWindow = _inventoryWindow.Get( this ); return _inventoryWindow.value; }
		//	set { if( _inventoryWindow.BeginSet( this, ref value ) ) { try { InventoryWindowChanged?.Invoke( this ); } finally { _inventoryWindow.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="InventoryWindow"/> property value changes.</summary>
		//public event Action<GameMode> InventoryWindowChanged;
		//ReferenceField<bool> _inventoryWindow = true;

		/// <summary>
		/// The first key code to change camera type. Between first person and third-person.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.F6 )]
		[DisplayName( "Key Camera 1" )]
		public Reference<EKeys> KeyCamera1
		{
			get { if( _keyCamera1.BeginGet() ) KeyCamera1 = _keyCamera1.Get( this ); return _keyCamera1.value; }
			set { if( _keyCamera1.BeginSet( this, ref value ) ) { try { KeyCamera1Changed?.Invoke( this ); } finally { _keyCamera1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyCamera1"/> property value changes.</summary>
		public event Action<GameMode> KeyCamera1Changed;
		ReferenceField<EKeys> _keyCamera1 = EKeys.F6;

		/// <summary>
		/// The second key code to change camera type. Between first person and third-person.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.None )]
		[DisplayName( "Key Camera 2" )]
		public Reference<EKeys> KeyCamera2
		{
			get { if( _keyCamera2.BeginGet() ) KeyCamera2 = _keyCamera2.Get( this ); return _keyCamera2.value; }
			set { if( _keyCamera2.BeginSet( this, ref value ) ) { try { KeyCamera2Changed?.Invoke( this ); } finally { _keyCamera2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyCamera2"/> property value changes.</summary>
		public event Action<GameMode> KeyCamera2Changed;
		ReferenceField<EKeys> _keyCamera2 = EKeys.None;

		[Category( "Control Keys" )]
		[DefaultValue( EKeys.W )]
		[DisplayName( "Key Forward 1" )]
		public Reference<EKeys> KeyForward1
		{
			get { if( _keyForward1.BeginGet() ) KeyForward1 = _keyForward1.Get( this ); return _keyForward1.value; }
			set { if( _keyForward1.BeginSet( this, ref value ) ) { try { KeyForward1Changed?.Invoke( this ); } finally { _keyForward1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyForward1"/> property value changes.</summary>
		public event Action<GameMode> KeyForward1Changed;
		ReferenceField<EKeys> _keyForward1 = EKeys.W;

		[Category( "Control Keys" )]
		[DefaultValue( EKeys.Up )]
		[DisplayName( "Key Forward 2" )]
		public Reference<EKeys> KeyForward2
		{
			get { if( _keyForward2.BeginGet() ) KeyForward2 = _keyForward2.Get( this ); return _keyForward2.value; }
			set { if( _keyForward2.BeginSet( this, ref value ) ) { try { KeyForward2Changed?.Invoke( this ); } finally { _keyForward2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyForward2"/> property value changes.</summary>
		public event Action<GameMode> KeyForward2Changed;
		ReferenceField<EKeys> _keyForward2 = EKeys.Up;

		[Category( "Control Keys" )]
		[DefaultValue( EKeys.S )]
		[DisplayName( "Key Backward 1" )]
		public Reference<EKeys> KeyBackward1
		{
			get { if( _keyBackward1.BeginGet() ) KeyBackward1 = _keyBackward1.Get( this ); return _keyBackward1.value; }
			set { if( _keyBackward1.BeginSet( this, ref value ) ) { try { KeyBackward1Changed?.Invoke( this ); } finally { _keyBackward1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyBackward1"/> property value changes.</summary>
		public event Action<GameMode> KeyBackward1Changed;
		ReferenceField<EKeys> _keyBackward1 = EKeys.S;

		[Category( "Control Keys" )]
		[DefaultValue( EKeys.Down )]
		[DisplayName( "Key Backward 2" )]
		public Reference<EKeys> KeyBackward2
		{
			get { if( _keyBackward2.BeginGet() ) KeyBackward2 = _keyBackward2.Get( this ); return _keyBackward2.value; }
			set { if( _keyBackward2.BeginSet( this, ref value ) ) { try { KeyBackward2Changed?.Invoke( this ); } finally { _keyBackward2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyBackward2"/> property value changes.</summary>
		public event Action<GameMode> KeyBackward2Changed;
		ReferenceField<EKeys> _keyBackward2 = EKeys.Down;

		[Category( "Control Keys" )]
		[DefaultValue( EKeys.A )]
		[DisplayName( "Key Left 1" )]
		public Reference<EKeys> KeyLeft1
		{
			get { if( _keyLeft1.BeginGet() ) KeyLeft1 = _keyLeft1.Get( this ); return _keyLeft1.value; }
			set { if( _keyLeft1.BeginSet( this, ref value ) ) { try { KeyLeft1Changed?.Invoke( this ); } finally { _keyLeft1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyLeft1"/> property value changes.</summary>
		public event Action<GameMode> KeyLeft1Changed;
		ReferenceField<EKeys> _keyLeft1 = EKeys.A;

		[Category( "Control Keys" )]
		[DefaultValue( EKeys.Left )]
		[DisplayName( "Key Left 2" )]
		public Reference<EKeys> KeyLeft2
		{
			get { if( _keyLeft2.BeginGet() ) KeyLeft2 = _keyLeft2.Get( this ); return _keyLeft2.value; }
			set { if( _keyLeft2.BeginSet( this, ref value ) ) { try { KeyLeft2Changed?.Invoke( this ); } finally { _keyLeft2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyLeft2"/> property value changes.</summary>
		public event Action<GameMode> KeyLeft2Changed;
		ReferenceField<EKeys> _keyLeft2 = EKeys.Left;

		[Category( "Control Keys" )]
		[DefaultValue( EKeys.D )]
		[DisplayName( "Key Right 1" )]
		public Reference<EKeys> KeyRight1
		{
			get { if( _keyRight1.BeginGet() ) KeyRight1 = _keyRight1.Get( this ); return _keyRight1.value; }
			set { if( _keyRight1.BeginSet( this, ref value ) ) { try { KeyRight1Changed?.Invoke( this ); } finally { _keyRight1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyRight1"/> property value changes.</summary>
		public event Action<GameMode> KeyRight1Changed;
		ReferenceField<EKeys> _keyRight1 = EKeys.D;

		[Category( "Control Keys" )]
		[DefaultValue( EKeys.Right )]
		[DisplayName( "Key Right 2" )]
		public Reference<EKeys> KeyRight2
		{
			get { if( _keyRight2.BeginGet() ) KeyRight2 = _keyRight2.Get( this ); return _keyRight2.value; }
			set { if( _keyRight2.BeginSet( this, ref value ) ) { try { KeyRight2Changed?.Invoke( this ); } finally { _keyRight2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyRight2"/> property value changes.</summary>
		public event Action<GameMode> KeyRight2Changed;
		ReferenceField<EKeys> _keyRight2 = EKeys.Right;

		/// <summary>
		/// The first key code to run.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.Shift )]
		[DisplayName( "Key Run 1" )]
		public Reference<EKeys> KeyRun1
		{
			get { if( _keyRun1.BeginGet() ) KeyRun1 = _keyRun1.Get( this ); return _keyRun1.value; }
			set { if( _keyRun1.BeginSet( this, ref value ) ) { try { KeyRun1Changed?.Invoke( this ); } finally { _keyRun1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyRun1"/> property value changes.</summary>
		public event Action<GameMode> KeyRun1Changed;
		ReferenceField<EKeys> _keyRun1 = EKeys.Shift;

		/// <summary>
		/// The second key code to run.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.None )]
		[DisplayName( "Key Run 2" )]
		public Reference<EKeys> KeyRun2
		{
			get { if( _keyRun2.BeginGet() ) KeyRun2 = _keyRun2.Get( this ); return _keyRun2.value; }
			set { if( _keyRun2.BeginSet( this, ref value ) ) { try { KeyRun2Changed?.Invoke( this ); } finally { _keyRun2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyRun2"/> property value changes.</summary>
		public event Action<GameMode> KeyRun2Changed;
		ReferenceField<EKeys> _keyRun2 = EKeys.None;

		/// <summary>
		/// The first key code to jump.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.Space )]
		[DisplayName( "Key Jump 1" )]
		public Reference<EKeys> KeyJump1
		{
			get { if( _keyJump1.BeginGet() ) KeyJump1 = _keyJump1.Get( this ); return _keyJump1.value; }
			set { if( _keyJump1.BeginSet( this, ref value ) ) { try { KeyJump1Changed?.Invoke( this ); } finally { _keyJump1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyJump1"/> property value changes.</summary>
		public event Action<GameMode> KeyJump1Changed;
		ReferenceField<EKeys> _keyJump1 = EKeys.Space;

		/// <summary>
		/// The second key code to jump.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.None )]
		[DisplayName( "Key Jump 2" )]
		public Reference<EKeys> KeyJump2
		{
			get { if( _keyJump2.BeginGet() ) KeyJump2 = _keyJump2.Get( this ); return _keyJump2.value; }
			set { if( _keyJump2.BeginSet( this, ref value ) ) { try { KeyJump2Changed?.Invoke( this ); } finally { _keyJump2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyJump2"/> property value changes.</summary>
		public event Action<GameMode> KeyJump2Changed;
		ReferenceField<EKeys> _keyJump2 = EKeys.None;

		/// <summary>
		/// The first key code to interact with objects. Take item, sit to vehicle.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.E )]
		[DisplayName( "Key Interact 1" )]
		public Reference<EKeys> KeyInteract1
		{
			get { if( _keyInteract1.BeginGet() ) KeyInteract1 = _keyInteract1.Get( this ); return _keyInteract1.value; }
			set { if( _keyInteract1.BeginSet( this, ref value ) ) { try { KeyInteract1Changed?.Invoke( this ); } finally { _keyInteract1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyInteract1"/> property value changes.</summary>
		public event Action<GameMode> KeyInteract1Changed;
		ReferenceField<EKeys> _keyInteract1 = EKeys.E;

		/// <summary>
		/// The second key code to interact with objects. Take item, sit to vehicle.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.None )]
		[DisplayName( "Key Interact 2" )]
		public Reference<EKeys> KeyInteract2
		{
			get { if( _keyInteract2.BeginGet() ) KeyInteract2 = _keyInteract2.Get( this ); return _keyInteract2.value; }
			set { if( _keyInteract2.BeginSet( this, ref value ) ) { try { KeyInteract2Changed?.Invoke( this ); } finally { _keyInteract2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyInteract2"/> property value changes.</summary>
		public event Action<GameMode> KeyInteract2Changed;
		ReferenceField<EKeys> _keyInteract2 = EKeys.None;

		/// <summary>
		/// The first key code to drop item.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.Control )]//T )]
		[DisplayName( "Key Drop 1" )]
		public Reference<EKeys> KeyDrop1
		{
			get { if( _keyDrop1.BeginGet() ) KeyDrop1 = _keyDrop1.Get( this ); return _keyDrop1.value; }
			set { if( _keyDrop1.BeginSet( this, ref value ) ) { try { KeyDrop1Changed?.Invoke( this ); } finally { _keyDrop1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyDrop1"/> property value changes.</summary>
		public event Action<GameMode> KeyDrop1Changed;
		ReferenceField<EKeys> _keyDrop1 = EKeys.Control;//.T;

		/// <summary>
		/// The second key code to drop item.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.None )]
		[DisplayName( "Key Drop 2" )]
		public Reference<EKeys> KeyDrop2
		{
			get { if( _keyDrop2.BeginGet() ) KeyDrop2 = _keyDrop2.Get( this ); return _keyDrop2.value; }
			set { if( _keyDrop2.BeginSet( this, ref value ) ) { try { KeyDrop2Changed?.Invoke( this ); } finally { _keyDrop2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyDrop2"/> property value changes.</summary>
		public event Action<GameMode> KeyDrop2Changed;
		ReferenceField<EKeys> _keyDrop2 = EKeys.None;

		/// <summary>
		/// The first key code to brake.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.Space )]
		[DisplayName( "Key Brake 1" )]
		public Reference<EKeys> KeyBrake1
		{
			get { if( _keyBrake1.BeginGet() ) KeyBrake1 = _keyBrake1.Get( this ); return _keyBrake1.value; }
			set { if( _keyBrake1.BeginSet( this, ref value ) ) { try { KeyBrake1Changed?.Invoke( this ); } finally { _keyBrake1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyBrake1"/> property value changes.</summary>
		public event Action<GameMode> KeyBrake1Changed;
		ReferenceField<EKeys> _keyBrake1 = EKeys.Space;

		/// <summary>
		/// The second key code to brake.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.None )]
		[DisplayName( "Key Brake 2" )]
		public Reference<EKeys> KeyBrake2
		{
			get { if( _keyBrake2.BeginGet() ) KeyBrake2 = _keyBrake2.Get( this ); return _keyBrake2.value; }
			set { if( _keyBrake2.BeginSet( this, ref value ) ) { try { KeyBrake2Changed?.Invoke( this ); } finally { _keyBrake2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyBrake2"/> property value changes.</summary>
		public event Action<GameMode> KeyBrake2Changed;
		ReferenceField<EKeys> _keyBrake2 = EKeys.None;

		/// <summary>
		/// The first key code to headlights low.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.L )]
		[DisplayName( "Key Headlights Low 1" )]
		public Reference<EKeys> KeyHeadlightsLow1
		{
			get { if( _keyHeadlightsLow1.BeginGet() ) KeyHeadlightsLow1 = _keyHeadlightsLow1.Get( this ); return _keyHeadlightsLow1.value; }
			set { if( _keyHeadlightsLow1.BeginSet( this, ref value ) ) { try { KeyHeadlightsLow1Changed?.Invoke( this ); } finally { _keyHeadlightsLow1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyHeadlightsLow1"/> property value changes.</summary>
		public event Action<GameMode> KeyHeadlightsLow1Changed;
		ReferenceField<EKeys> _keyHeadlightsLow1 = EKeys.L;

		/// <summary>
		/// The second key code to headlights low.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.None )]
		[DisplayName( "Key Headlights Low 2" )]
		public Reference<EKeys> KeyHeadlightsLow2
		{
			get { if( _keyHeadlightsLow2.BeginGet() ) KeyHeadlightsLow2 = _keyHeadlightsLow2.Get( this ); return _keyHeadlightsLow2.value; }
			set { if( _keyHeadlightsLow2.BeginSet( this, ref value ) ) { try { KeyHeadlightsLow2Changed?.Invoke( this ); } finally { _keyHeadlightsLow2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyHeadlightsLow2"/> property value changes.</summary>
		public event Action<GameMode> KeyHeadlightsLow2Changed;
		ReferenceField<EKeys> _keyHeadlightsLow2 = EKeys.None;

		/// <summary>
		/// The first key code to headlights high.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.H )]
		[DisplayName( "Key Headlights High 1" )]
		public Reference<EKeys> KeyHeadlightsHigh1
		{
			get { if( _keyHeadlightsHigh1.BeginGet() ) KeyHeadlightsHigh1 = _keyHeadlightsHigh1.Get( this ); return _keyHeadlightsHigh1.value; }
			set { if( _keyHeadlightsHigh1.BeginSet( this, ref value ) ) { try { KeyHeadlightsHigh1Changed?.Invoke( this ); } finally { _keyHeadlightsHigh1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyHeadlightsHigh1"/> property value changes.</summary>
		public event Action<GameMode> KeyHeadlightsHigh1Changed;
		ReferenceField<EKeys> _keyHeadlightsHigh1 = EKeys.H;

		/// <summary>
		/// The second key code to headlights high.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.None )]
		[DisplayName( "Key Headlights High 2" )]
		public Reference<EKeys> KeyHeadlightsHigh2
		{
			get { if( _keyHeadlightsHigh2.BeginGet() ) KeyHeadlightsHigh2 = _keyHeadlightsHigh2.Get( this ); return _keyHeadlightsHigh2.value; }
			set { if( _keyHeadlightsHigh2.BeginSet( this, ref value ) ) { try { KeyHeadlightsHigh2Changed?.Invoke( this ); } finally { _keyHeadlightsHigh2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyHeadlightsHigh2"/> property value changes.</summary>
		public event Action<GameMode> KeyHeadlightsHigh2Changed;
		ReferenceField<EKeys> _keyHeadlightsHigh2 = EKeys.None;

		/// <summary>
		/// The first key code to left turn signal.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.O )]
		[DisplayName( "Key Left Turn Signal 1" )]
		public Reference<EKeys> KeyLeftTurnSignal1
		{
			get { if( _keyLeftTurnSignal1.BeginGet() ) KeyLeftTurnSignal1 = _keyLeftTurnSignal1.Get( this ); return _keyLeftTurnSignal1.value; }
			set { if( _keyLeftTurnSignal1.BeginSet( this, ref value ) ) { try { KeyLeftTurnSignal1Changed?.Invoke( this ); } finally { _keyLeftTurnSignal1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyLeftTurnSignal1"/> property value changes.</summary>
		public event Action<GameMode> KeyLeftTurnSignal1Changed;
		ReferenceField<EKeys> _keyLeftTurnSignal1 = EKeys.O;

		/// <summary>
		/// The second key code to left turn signal.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.None )]
		[DisplayName( "Key Left Turn Signal 2" )]
		public Reference<EKeys> KeyLeftTurnSignal2
		{
			get { if( _keyLeftTurnSignal2.BeginGet() ) KeyLeftTurnSignal2 = _keyLeftTurnSignal2.Get( this ); return _keyLeftTurnSignal2.value; }
			set { if( _keyLeftTurnSignal2.BeginSet( this, ref value ) ) { try { KeyLeftTurnSignal2Changed?.Invoke( this ); } finally { _keyLeftTurnSignal2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyLeftTurnSignal2"/> property value changes.</summary>
		public event Action<GameMode> KeyLeftTurnSignal2Changed;
		ReferenceField<EKeys> _keyLeftTurnSignal2 = EKeys.None;

		/// <summary>
		/// The first key code to right turn signal.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.P )]
		[DisplayName( "Key Right Turn Signal 1" )]
		public Reference<EKeys> KeyRightTurnSignal1
		{
			get { if( _keyRightTurnSignal1.BeginGet() ) KeyRightTurnSignal1 = _keyRightTurnSignal1.Get( this ); return _keyRightTurnSignal1.value; }
			set { if( _keyRightTurnSignal1.BeginSet( this, ref value ) ) { try { KeyRightTurnSignal1Changed?.Invoke( this ); } finally { _keyRightTurnSignal1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyRightTurnSignal1"/> property value changes.</summary>
		public event Action<GameMode> KeyRightTurnSignal1Changed;
		ReferenceField<EKeys> _keyRightTurnSignal1 = EKeys.P;

		/// <summary>
		/// The second key code to right turn signal.
		/// </summary>
		[Category( "Control Keys" )]
		[DefaultValue( EKeys.None )]
		[DisplayName( "Key Right Turn Signal 2" )]
		public Reference<EKeys> KeyRightTurnSignal2
		{
			get { if( _keyRightTurnSignal2.BeginGet() ) KeyRightTurnSignal2 = _keyRightTurnSignal2.Get( this ); return _keyRightTurnSignal2.value; }
			set { if( _keyRightTurnSignal2.BeginSet( this, ref value ) ) { try { KeyRightTurnSignal2Changed?.Invoke( this ); } finally { _keyRightTurnSignal2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyRightTurnSignal2"/> property value changes.</summary>
		public event Action<GameMode> KeyRightTurnSignal2Changed;
		ReferenceField<EKeys> _keyRightTurnSignal2 = EKeys.None;

		///////////////////////////////////////////////

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
			set { if( _replaceCamera.BeginSet( this, ref value ) ) { try { ReplaceCameraChanged?.Invoke( this ); } finally { _replaceCamera.EndSet(); } } }
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
				//case nameof( FirstPersonCameraShowControlledObject ):
				//case nameof( FirstPersonCameraAttachToEyes ):
				//case nameof( FirstPersonCameraCutVolumeRadius ):
				//	if( UseBuiltInCamera.Value != BuiltInCameraEnum.FirstPerson )
				//		skip = true;
				//	break;

				////case nameof( ThirdPersonCameraFollowDirection ):
				//case nameof( ThirdPersonCameraVerticalAngle ):
				//case nameof( ThirdPersonCameraDistance ):
				//case nameof( ThirdPersonCameraHeight ):
				//	if( UseBuiltInCamera.Value != BuiltInCameraEnum.ThirdPerson )
				//		skip = true;
				//	break;

				//case nameof( ThirdPersonCameraHorizontalAngle ):
				//	if( UseBuiltInCamera.Value != BuiltInCameraEnum.ThirdPerson )//|| ThirdPersonCameraFollowDirection.Value )
				//		skip = true;
				//	break;

				//case nameof( ThirdPersonCameraFollowDirectionSpeed ):
				//	if( UseBuiltInCamera.Value != BuiltInCameraEnum.ThirdPerson )//|| !ThirdPersonCameraFollowDirection.Value )
				//		skip = true;
				//	break;

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
							var direction = d.GetVector();

							var vector = direction * ThirdPersonCameraDistance.Value;
							var from = lookAt - vector;

							if( ThirdPersonCameraLeft.Value != 0 )
							{
								var d2 = new SphericalDirection( MathEx.DegreeToRadian( ThirdPersonCameraHorizontalAngle.Value + 90 ), 0 );
								var direction2 = d2.GetVector();
								from += direction2 * ThirdPersonCameraLeft;
							}

							//update when not visible
							{
								var volumeTestItem = new PhysicsVolumeTestItem( new Sphere( lookAt, 0.1 ), -vector, PhysicsVolumeTestItem.ModeEnum.OneClosestForEach );
								Scene.PhysicsVolumeTest( volumeTestItem );

								foreach( var item in volumeTestItem.Result )
								{
									var c = item.Body.Owner as Component;
									if( c != null && ( c == obj || c.GetAllParents().Contains( obj ) ) )
										continue;

									from += vector * ( 1.0 - item.DistanceScale );
									break;
								}
							}

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

		public bool ChangeCameraType()
		{
			var demoMode = ParentRoot.GetComponent<DemoMode>( onlyEnabledInHierarchy: true );
			if( demoMode != null )
			{
				//with DemoMode

				if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson || UseBuiltInCamera.Value == BuiltInCameraEnum.None )
				{
					UseBuiltInCamera = BuiltInCameraEnum.ThirdPerson;

					var viewport = playScreen.ParentContainer.Viewport;
					viewport.KeysAndMouseButtonUpAll();
					viewport.NotifyInstantCameraMovement();

					demoMode.UpdateWalkMode();

					return true;
				}
				else if( UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
				{
					UseBuiltInCamera = BuiltInCameraEnum.FirstPerson;

					var viewport = playScreen.ParentContainer.Viewport;
					viewport.KeysAndMouseButtonUpAll();
					viewport.NotifyInstantCameraMovement();

					demoMode.UpdateWalkMode();

					return true;
				}

				return true;
			}
			else
			{
				//without DemoMode

				if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson )
				{
					UseBuiltInCamera = BuiltInCameraEnum.ThirdPerson;

					var viewport = playScreen.ParentContainer.Viewport;
					viewport.KeysAndMouseButtonUpAll();
					viewport.NotifyInstantCameraMovement();

					return true;
				}
				else if( UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
				{
					UseBuiltInCamera = BuiltInCameraEnum.FirstPerson;

					var viewport = playScreen.ParentContainer.Viewport;
					viewport.KeysAndMouseButtonUpAll();
					viewport.NotifyInstantCameraMovement();

					return true;
				}
			}

			return false;
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
			if( keyDown != null )
			{
				if( keyDown.Key == KeyCamera1.Value || keyDown.Key == KeyCamera2.Value )
				{
					if( ChangeCameraType() )
						return true;
				}

				if( keyDown.Key == FreeCameraKey.Value )
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
					//if( !ThirdPersonCameraFollowDirection )
					//{
					var viewport = playScreen.ParentContainer.Viewport;
					if( viewport.MouseRelativeMode )
					{
						var sensitivity = MouseSensitivity * 2;

						var h = ThirdPersonCameraHorizontalAngle.Value - new Radian( MousePosition.X ).InDegrees() * sensitivity;
						if( h < 0 ) h += 360;
						if( h > 360 ) h -= 360;

						var v = ThirdPersonCameraVerticalAngle.Value - new Radian( MousePosition.Y ).InDegrees() * sensitivity;
						v = MathEx.Clamp( (double)v, -80, 80 );

						ThirdPersonCameraHorizontalAngle = h;
						ThirdPersonCameraVerticalAngle = v;
					}
					//}
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
			var initiator = ObjectControlledByPlayer.Value;
			if( ObjectInteractionContext != null && ObjectInteractionContext.Obj.InteractionInputMessage( this, initiator, message ) )
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
				var character2D = objectControlledByPlayer as Character2D;
				if( character2D != null )
				{
					var input2 = character2D.CreateComponent<Character2DInputProcessing>();
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
						var direction = obj.TransformV.Rotation.GetForward().ToVector2();
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
				////third person camera update horizontal direction
				//if( ThirdPersonCameraFollowDirection )
				//{
				//	var step = (double)ThirdPersonCameraFollowDirectionSpeed.Value.InRadians() * delta;
				//	if( step != 0 )
				//	{
				//		var obj = ObjectControlledByPlayer.Value as ObjectInSpace;
				//		if( obj != null )
				//		{
				//			var direction = obj.TransformV.Rotation.GetForward().ToVector2();
				//			if( direction != Vector2.Zero )
				//			{
				//				var demandedAngle = Math.Atan2( direction.Y, direction.X );
				//				var currentAngle = ThirdPersonCameraHorizontalAngle.Value.InRadians();

				//				var angle = demandedAngle - currentAngle;
				//				var d = new Vector2( Math.Cos( angle ), Math.Sin( angle ) );
				//				double factor = -d.Y;

				//				ThirdPersonCameraHorizontalAngle += new Radian( step * factor ).InDegrees();
				//			}
				//		}
				//	}
				//}

				//update third person camera distance
				{
					var speed = 2.0;

					var values = ThirdPersonCameraDistance.Value;

					if( IsKeyPressed( EKeys.PageUp ) )
						values -= speed * delta;
					if( IsKeyPressed( EKeys.PageDown ) )
						values += speed * delta;

					if( values < 1 )
						values = 1;

					ThirdPersonCameraDistance = values;
				}

				//update third person camera height
				{
					var speed = 1.0;

					var value = ThirdPersonCameraHeight.Value;

					if( IsKeyPressed( EKeys.Home ) )
						value += speed * delta;
					if( IsKeyPressed( EKeys.End ) )
						value -= speed * delta;

					ThirdPersonCameraHeight = value;
				}

				//update third person camera left
				{
					var speed = 1.0;

					var value = ThirdPersonCameraLeft.Value;

					if( IsKeyPressed( EKeys.Oemplus ) )
						value += speed * delta;
					if( IsKeyPressed( EKeys.OemMinus ) )
						value -= speed * delta;

					ThirdPersonCameraLeft = value;
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

		public delegate void GetInteractiveObjectInfoEventDelegate( GameMode sender, InteractiveObjectInterface obj, ref InteractiveObjectObjectInfo result );
		public event GetInteractiveObjectInfoEventDelegate GetInteractiveObjectInfoEvent;

		public virtual InteractiveObjectObjectInfo GetInteractiveObjectInfo( InteractiveObjectInterface obj )
		{
			InteractiveObjectObjectInfo result = null;
			var initiator = ObjectControlledByPlayer.Value;
			obj.InteractionGetInfo( this, initiator, ref result );
			GetInteractiveObjectInfoEvent?.Invoke( this, obj, ref result );
			if( result == null )
				result = new InteractiveObjectObjectInfo();
			return result;
		}

		public delegate void PickInteractiveObjectEventDelegate( GameMode gameMode, Viewport viewport, ref InteractiveObjectInterface result );
		public event PickInteractiveObjectEventDelegate PickInteractiveObjectEvent;

		public virtual Sphere GetInteractionBoundingSphere( ObjectInSpace obj )
		{
			var boundingSphere = obj.SpaceBounds.BoundingSphere;

			//Character specific
			if( obj is Character character )
				boundingSphere.Radius = character.TypeCached.Height * character.TransformV.Scale.MaxComponent() * 0.6;

			//minimal radius
			if( boundingSphere.Radius < 0.2 )
				boundingSphere.Radius = 0.2;

			return boundingSphere;
		}

		public virtual Vector3[] GetInteractionBoundingPoints( ObjectInSpace obj )
		{
			//Vehicle specific
			var vehicle = obj as Vehicle;
			if( vehicle != null )
				return vehicle.GetBox().ToPoints();

			////Door specific
			//var door = obj as Door;
			//if( door != null )
			//{
			//	var box = door.GetBox();
			//	box.Extents *= 1.05;
			//	return box.ToPoints();
			//	//return door.GetBox().ToPoints();
			//}

			var sphere = GetInteractionBoundingSphere( obj );
			var result = new List<Vector3>( 64 );
			for( var v = -MathEx.PI; v <= MathEx.PI + 0.01; v += MathEx.PI / 6 )
				for( var h = 0.0; h <= MathEx.PI * 2 + 0.01; h += MathEx.PI / 6 )
					result.Add( sphere.Center + new SphericalDirection( h, v ).GetVector() * sphere.Radius );
			return result.ToArray();
		}

		public virtual InteractiveObjectInterface PickInteractiveObject( Viewport viewport )
		{
			InteractiveObjectInterface result = null;
			//PickInteractiveObjectEvent?.Invoke( this, viewport, ref result );

			var controlledObject = ObjectControlledByPlayer.Value as ObjectInSpace;

			if( !FreeCamera && !CutsceneStarted && controlledObject != null )
			{
				//pick for 3D
				if( Scene.Mode.Value == Scene.ModeEnum._3D )
				{
					if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson )//|| UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
					{
						double maxDistance = FirstPersonCameraPickInteractiveObjectDistance.Value;
						//scaling
						maxDistance *= controlledObject.TransformV.Scale.MaxComponent();

						Ray ray;
						{
							ray = viewport.CameraSettings.GetRayByScreenCoordinates( new Vector2( 0.5, 0.5 ) );
							ray.Direction = ray.Direction.GetNormalize() * maxDistance;
						}

						var bounds = new Bounds( ray.Origin );
						bounds.Add( ray.Origin + ray.Direction );
						bounds.Expand( 1 );

						var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, bounds );
						Scene.GetObjectsInSpace( getObjectsItem );

						InteractiveObjectInterface minObject = null;
						var minScale = double.MaxValue;

						foreach( var item in getObjectsItem.Result )
						{
							if( item.Object == controlledObject )
								continue;
							if( item.Object is Camera || item.Object is Light || item.Object is Sensor || item.Object is Sensor2D )
								continue;

							var boundingSphere = GetInteractionBoundingSphere( item.Object );
							if( boundingSphere.Intersects( ray, out var scale1, out var scale2 ) )
							{
								//check direction
								if( ( ray.Origin + ray.Direction.GetNormalize() * 1000 - boundingSphere.Center ).Length() > ( ray.Origin - ray.Direction.GetNormalize() * 1000 - boundingSphere.Center ).Length() )
									continue;

								var scale = Math.Min( scale1, scale2 );
								if( scale <= 1 && scale < minScale )
								{
									var obj = item.Object.FindThisOrParent<InteractiveObjectInterface>();
									if( obj == null )
										obj = item.Object.GetComponent<InteractiveObject>();

									if( obj != null && GetInteractiveObjectInfo( obj ).AllowInteract )
									{
										minObject = obj;
										minScale = scale;
									}
								}
							}
						}

						//check accesibility by physics
						if( minObject != null )
						{
							var ray2 = new Ray( ray.Origin, ray.Direction * minScale );
							var rayTestItem = new PhysicsRayTestItem( ray2, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );
							Scene.PhysicsRayTest( rayTestItem );

							foreach( var resultItem in rayTestItem.Result )
							{
								var owner = resultItem.Body.Owner as Component;
								if( owner != null )
								{
									if( controlledObject != null )
										if( owner == controlledObject || owner.Parent == controlledObject )
											continue;
									if( owner == minObject || owner.Parent == minObject )
										continue;
								}

								//found blocking object
								minObject = null;
							}
						}

						if( minObject != null )
							result = minObject;


						//var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, ray );
						//Scene.GetObjectsInSpace( getObjectsItem );

						//foreach( var item in getObjectsItem.Result )
						//{
						//	if( item.Object is Camera || item.Object is Sensor || item.Object is Sensor2D )
						//		continue;

						//	var boundingSphere = GetInteractionBoundingSphere( item.Object );
						//	if( boundingSphere.Intersects( ray ) )
						//	{
						//		var obj = item.Object.FindThisOrParent<InteractiveObject>();
						//		if( obj != null )
						//		{
						//			if( GetInteractiveObjectInfo( obj ).AllowInteract )
						//				result = obj;
						//		}
						//	}
						//}
					}

					if( UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
					{
						var maxDistance = ThirdPersonCameraPickInteractiveObjectDistance.Value;
						//scaling
						maxDistance *= controlledObject.TransformV.Scale.MaxComponent();

						var bounds = controlledObject.SpaceBounds.BoundingBox;
						var offsetsRange = new Range( bounds.Minimum.Z - controlledObject.TransformV.Position.Z, bounds.Maximum.Z - controlledObject.TransformV.Position.Z );
						//var offsetsRange = new Range( -0.25, 0.5 );

						for( var factor = 0.0; factor <= 1.001; factor += 0.1 )
						{
							var offset = MathEx.Lerp( offsetsRange.Minimum, offsetsRange.Maximum, factor );

							var cameraDirection2D = viewport.CameraSettings.Direction.ToVector2();
							if( cameraDirection2D == Vector2.Zero )
								cameraDirection2D.X += 0.0001;
							cameraDirection2D.Normalize();

							Ray ray = new Ray( controlledObject.TransformV.Position + new Vector3( 0, 0, offset ), new Vector3( cameraDirection2D, 0 ) * maxDistance );
							//Ray ray = new Ray( controlledObject.TransformV.Position + new Vector3( 0, 0, offset ), controlledObject.TransformV.Rotation.GetForward() * maxDistance );

							var bounds2 = new Bounds( ray.Origin );
							bounds2.Add( ray.Origin + ray.Direction );
							bounds2.Expand( 1 );

							var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, bounds2 );
							Scene.GetObjectsInSpace( getObjectsItem );

							InteractiveObjectInterface minObject = null;
							var minScale = double.MaxValue;

							foreach( var item in getObjectsItem.Result )
							{
								if( item.Object == controlledObject )
									continue;
								if( item.Object is Camera || item.Object is Light || item.Object is Sensor || item.Object is Sensor2D )
									continue;

								var boundingSphere = GetInteractionBoundingSphere( item.Object );
								if( boundingSphere.Intersects( ray, out var scale1, out var scale2 ) )
								{
									//check direction
									if( ( ray.Origin + ray.Direction.GetNormalize() * 1000 - boundingSphere.Center ).Length() > ( ray.Origin - ray.Direction.GetNormalize() * 1000 - boundingSphere.Center ).Length() )
										continue;

									var scale = Math.Min( scale1, scale2 );
									if( scale <= 1 && scale < minScale )
									{
										var obj = item.Object.FindThisOrParent<InteractiveObjectInterface>();
										if( obj == null )
											obj = item.Object.GetComponent<InteractiveObject>();

										if( obj != null && GetInteractiveObjectInfo( obj ).AllowInteract )
										{
											minObject = obj;
											minScale = scale;
										}
									}
								}
							}

							//check accesibility by physics
							if( minObject != null )
							{
								var ray2 = new Ray( ray.Origin, ray.Direction * minScale );
								var rayTestItem = new PhysicsRayTestItem( ray2, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );
								Scene.PhysicsRayTest( rayTestItem );

								foreach( var resultItem in rayTestItem.Result )
								{
									var owner = resultItem.Body.Owner as Component;
									if( owner != null )
									{
										if( controlledObject != null )
											if( owner == controlledObject || owner.Parent == controlledObject )
												continue;
										if( owner == minObject || owner.Parent == minObject )
											continue;
									}

									//found blocking object
									minObject = null;
								}
							}

							if( minObject != null )
								result = minObject;


							//var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, ray );
							//Scene.GetObjectsInSpace( getObjectsItem );

							//foreach( var item in getObjectsItem.Result )
							//{
							//	if( item.Object is Camera || item.Object is Sensor || item.Object is Sensor2D )
							//		continue;

							//	var boundingSphere = GetInteractionBoundingSphere( item.Object );
							//	if( boundingSphere.Intersects( ray, out var scale1, out var scale2 ) )
							//	{
							//		var scale = Math.Min( scale1, scale2 );
							//		if( ( ray.Direction * scale ).Length() < distance )
							//		{
							//			var obj = item.Object.FindThisOrParent<InteractiveObject>();
							//			if( obj != null )
							//			{
							//				if( GetInteractiveObjectInfo( obj ).AllowInteract )
							//					result = obj;
							//			}
							//		}
							//	}
							//}
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
							double maxDistance = Camera2DPickInteractiveObjectDistance.Value;
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
								var obj = item.Object.FindThisOrParent<InteractiveObjectInterface>();
								if( obj == null )
									obj = item.Object.GetComponent<InteractiveObject>();

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
					ObjectInteractionContext.Obj.InteractionExit( ObjectInteractionContext );
					//ObjectInteractionContext.Dispose();
					ObjectInteractionContext = null;
				}

				//create new
				if( overObject != null )
				{
					var initiator = ObjectControlledByPlayer.Value;
					ObjectInteractionContext = new ObjectInteractionContext( overObject, initiator, this, viewport );
					ObjectInteractionContext.Obj.InteractionEnter( ObjectInteractionContext );
				}
			}

			//update context
			if( ObjectInteractionContext != null )
			{
				ObjectInteractionContext.Viewport = viewport;
				ObjectInteractionContext.Obj.InteractionUpdate( ObjectInteractionContext );
			}
		}

		public delegate void RenderObjectInteractionEventDelegate( GameMode sender, CanvasRenderer renderer, ObjectInSpace obj, InteractiveObjectObjectInfo info, ref bool handled );
		public event RenderObjectInteractionEventDelegate RenderObjectInteractionEvent;

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
					if( obj == null )
					{
						var interactiveObject = ObjectInteractionContext.Obj as InteractiveObject;
						if( interactiveObject != null )
							obj = interactiveObject.Parent as ObjectInSpace;
					}

					if( obj != null )
					{
						var info = GetInteractiveObjectInfo( ObjectInteractionContext.Obj );
						if( info.AllowInteract )
						{
							//override render
							var handled = false;
							RenderObjectInteractionEvent?.Invoke( this, renderer, obj, info, ref handled );
							if( !handled && info.OverrideRender != null )
								info.OverrideRender( this, info, renderer, ref handled );

							//default implementation
							if( !handled && info.DefaultRender )
							{
								//calculate screen rectangle
								var rectangle = Rectangle.Cleared;
								foreach( var point in GetInteractionBoundingPoints( obj ) )
								{
									if( renderer.ViewportForScreenCanvasRenderer.CameraSettings.ProjectToScreenCoordinates( point, out var screenPosition ) )
										rectangle.Add( screenPosition );
								}

								if( !rectangle.IsCleared() )
								{
									//optionally can use Outline effect

									//expand rectangle
									{
										var multiplier = 1.05;//1.1;
										var center = rectangle.GetCenter();
										var size = rectangle.GetSize();
										rectangle = new Rectangle( center );
										rectangle.Expand( size / 2 * multiplier );
									}

									var color = new ColorValue( 1, 1, 0, 0.8 );
									if( info.OverrideColor.HasValue )
										color = info.OverrideColor.Value;

									//add rectangle
									for( int step = 0; step < 3; step++ )
									{
										var rectangle2 = rectangle;

										var color2 = color;
										color2.Alpha /= 3;

										var thickness = new Vector2( 0.004, 0.004 * renderer.AspectRatio );
										thickness *= renderer.ViewportForScreenCanvasRenderer.SizeInPixels.ToVector2();
										thickness = thickness.ToVector2I().ToVector2();
										thickness /= renderer.ViewportForScreenCanvasRenderer.SizeInPixels.ToVector2();

										var innerRectangle = rectangle2;
										innerRectangle.Expand( -thickness );

										switch( step )
										{
										case 0:
											rectangle2.Expand( thickness / 8 );
											innerRectangle.Expand( -thickness / 8 );
											break;

										case 1:
											rectangle2.Expand( thickness / 10 );
											innerRectangle.Expand( -thickness / 10 );
											break;
										}

										//rounded corners

										var roundedSize = new Vector2( 0.003, 0.003 * renderer.AspectRatio );

										renderer.PushClipRectangle( new Rectangle( -10000, -10000, 10000, rectangle2.Top + thickness.Y ) );
										renderer.AddRoundedQuad( rectangle2, roundedSize.Y, CanvasRenderer.AddRoundedQuadMode.Antialiasing, color2 );
										renderer.PopClipRectangle();

										renderer.PushClipRectangle( new Rectangle( -10000, rectangle2.Bottom - thickness.Y, 10000, 10000 ) );
										renderer.AddRoundedQuad( rectangle2, roundedSize.Y, CanvasRenderer.AddRoundedQuadMode.Antialiasing, color2 );
										renderer.PopClipRectangle();

										renderer.PushClipRectangle( new Rectangle( -10000, rectangle2.Top + thickness.Y, rectangle2.Left + thickness.X, rectangle2.Bottom - thickness.Y ) );
										renderer.AddRoundedQuad( rectangle2, roundedSize.Y, CanvasRenderer.AddRoundedQuadMode.Antialiasing, color2 );
										renderer.PopClipRectangle();

										renderer.PushClipRectangle( new Rectangle( rectangle2.Right - thickness.X, rectangle2.Top + thickness.Y, 10000, rectangle2.Bottom - thickness.Y ) );
										renderer.AddRoundedQuad( rectangle2, roundedSize.Y, CanvasRenderer.AddRoundedQuadMode.Antialiasing, color2 );
										renderer.PopClipRectangle();

										////sharp corners
										//renderer.AddThickRectangle( rectangle, innerRectangle, color );
									}

									//add text

									var lines = info.Text.Split( new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );
									var bottonCenter = new Vector2( rectangle.GetCenter().X, rectangle.Bottom + 0.002 );

									CanvasRendererUtility.AddTextLinesWithShadow( renderer.ViewportForScreenCanvasRenderer, renderer.DefaultFont, renderer.DefaultFontSize * 1.3, lines, new Rectangle( bottonCenter, bottonCenter ), EHorizontalAlignment.Center, EVerticalAlignment.Top, color );

									//renderer.AddTextLines( info.TextInfo, new Vector2( rectangle.GetCenter().X, rectangle.Bottom ), EHorizontalAlignment.Center, EVerticalAlignment.Top, 0, color );
								}
							}
						}
					}
				}
			}
		}

		public delegate void RenderTargetImageBeforeDelegate( GameMode sender, CanvasRenderer renderer, ref bool show );
		public event RenderTargetImageBeforeDelegate RenderTargetImageBefore;

		protected virtual void RenderTargetImage( CanvasRenderer renderer )
		{
			var obj = ObjectControlledByPlayer.Value;
			if( obj != null && !FreeCamera && Scene.Mode.Value == Scene.ModeEnum._3D && !CutsceneStarted )
			{
				var display = DisplayTarget.Value;

				if( display == AutoTrueFalse.Auto )
				{
					//Character
					var character = obj as Character;
					if( character != null && character.GetActiveWeapon() != null )
					{
						if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson || GetCameraManagementOfCurrentObject() != null )
						{
							display = AutoTrueFalse.True;
						}
					}

					//Vehicle
					var vehicle = obj as Vehicle;
					if( vehicle != null && vehicle.DynamicData?.Turrets != null )
					{
						if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson || GetCameraManagementOfCurrentObject() != null )
						{
							display = AutoTrueFalse.True;
						}
					}
				}

				if( display == AutoTrueFalse.True )
				{
					var show = true;
					RenderTargetImageBefore?.Invoke( this, renderer, ref show );
					if( show )
					{
						var size = DisplayTargetSize.Value / 2;
						var rectangle = new Rectangle( 0.5 - size, 0.5 - size * renderer.AspectRatio, 0.5 + size, 0.5 + size * renderer.AspectRatio );
						renderer.AddQuad( rectangle, new Rectangle( 0, 0, 1, 1 ), DisplayTargetImage, DisplayTargetColor );
					}
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

			//update character data for first or third person camera before frame rendering
			if( ( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson ) && !FreeCamera )
			{
				var character = ObjectControlledByPlayer.Value as Character;
				if( character != null )
				{
					var inputProcessing = character.GetComponent<CharacterInputProcessing>();
					if( inputProcessing != null )
						inputProcessing.UpdateTurnToDirectionAndLookToToPosition( this, null );

					character.UpdateDataForFirstOrThirdPersonCamera( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson );
				}
			}

			////update character data for first person camera before frame rendering
			//if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson && !FreeCamera )
			//{
			//	var character = ObjectControlledByPlayer.Value as Character;
			//	if( character != null )
			//		character.UpdateDataForFirstPersonCamera( this, viewport );
			//}

			////update character data for third person camera before frame rendering
			//if( UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson && !FreeCamera )
			//{
			//	var character = ObjectControlledByPlayer.Value as Character;
			//	if( character != null )
			//	{
			//		var inputProcessing = character.GetComponent<CharacterInputProcessing>();
			//		if( inputProcessing != null )
			//			inputProcessing.UpdateTurnToDirectionAndLookToToPosition( this, null );

			//		character.UpdateDataForThirdPersonCamera( this, viewport );
			//	}
			//}

			//!!!!need? maybe need improve blur effect
			//update controlled object motion blur factor
			{
				var meshInSpace = ObjectControlledByPlayer.Value as MeshInSpace;
				if( meshInSpace != null )
				{
					if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson && !FreeCamera )
						meshInSpace.MotionBlurFactor = 0;
					else
						meshInSpace.MotionBlurFactor = 1;
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

		///////////////////////////////////////////////

		public delegate void ItemCanTakeDelegate( GameMode sender, Component taker, ItemInterface item, ref bool allowAction );
		public event ItemCanTakeDelegate ItemCanTake;
		internal void PerformItemCanTakeEvent( Component taker, ItemInterface item, ref bool allowAction )
		{
			ItemCanTake?.Invoke( this, taker, item, ref allowAction );
		}

		public delegate void ItemCanDropDelegate( GameMode sender, Component taker, ItemInterface item, ref bool allowAction, ref double amount );
		public event ItemCanDropDelegate ItemCanDrop;
		internal void PerformItemCanDropEvent( Component taker, ItemInterface item, ref bool allowAction, ref double amount )
		{
			ItemCanDrop?.Invoke( this, taker, item, ref allowAction, ref amount );
		}

		public delegate void ItemCanActivateDelegate( GameMode sender, Component holder, ItemInterface item, ref bool allowAction );
		public event ItemCanActivateDelegate ItemCanActivate;
		internal void PerformItemCanActivateEvent( Component holder, ItemInterface item, ref bool allowAction )
		{
			ItemCanActivate?.Invoke( this, holder, item, ref allowAction );
		}

		public delegate void ItemCanDeactivateDelegate( GameMode sender, Component holder, ItemInterface item, ref bool allowAction );
		public event ItemCanDeactivateDelegate ItemCanDeactivate;
		internal void PerformItemCanDeactivateEvent( Component holder, ItemInterface item, ref bool allowAction )
		{
			ItemCanDeactivate?.Invoke( this, holder, item, ref allowAction );
		}

		public virtual Vector3 GetBestGlobalReflectionProbePosition( Viewport viewport )
		{
			var cameraSettings = viewport.CameraSettings;

			//TPS camera specific. set probe to character position
			if( UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson && !FreeCamera )
			{
				var obj = ObjectControlledByPlayer.Value as ObjectInSpace;
				if( obj != null )
				{
					var sphere = obj.SpaceBounds.BoundingSphere;
					return sphere.Center + new Vector3( 0, 0, sphere.Radius );

					//return obj.TransformV.Position;
				}
			}

			//set probe to camera position
			return cameraSettings.Position;
		}
	}
}
