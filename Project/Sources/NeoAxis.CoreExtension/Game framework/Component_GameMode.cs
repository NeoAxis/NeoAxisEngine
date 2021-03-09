// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The object to interact Player app with the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Game Mode", -9999 )]
	public class Component_GameMode : Component
	{
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
		public Reference<Component> ObjectControlledByPlayer
		{
			get { if( _objectControlledByPlayer.BeginGet() ) ObjectControlledByPlayer = _objectControlledByPlayer.Get( this ); return _objectControlledByPlayer.value; }
			set { if( _objectControlledByPlayer.BeginSet( ref value ) ) { try { ObjectControlledByPlayerChanged?.Invoke( this ); } finally { _objectControlledByPlayer.EndSet(); } } }
		}
		public event Action<Component_GameMode> ObjectControlledByPlayerChanged;
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
		public event Action<Component_GameMode> UseBuiltInCameraChanged;
		ReferenceField<BuiltInCameraEnum> _useBuiltInCamera = BuiltInCameraEnum.None;

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
		public event Action<Component_GameMode> ThirdPersonCameraHorizontalAngleChanged;
		ReferenceField<Degree> _thirdPersonCameraHorizontalAngle = new Degree( 60 );

		/// <summary>
		/// The vertical angle of the camera in third-person mode.
		/// </summary>
		[Category( "Camera" )]
		[DefaultValue( 30 )]
		[Range( -180, 180 )]
		public Reference<Degree> ThirdPersonCameraVerticalAngle
		{
			get { if( _thirdPersonCameraVerticalAngle.BeginGet() ) ThirdPersonCameraVerticalAngle = _thirdPersonCameraVerticalAngle.Get( this ); return _thirdPersonCameraVerticalAngle.value; }
			set { if( _thirdPersonCameraVerticalAngle.BeginSet( ref value ) ) { try { ThirdPersonCameraVerticalAngleChanged?.Invoke( this ); } finally { _thirdPersonCameraVerticalAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThirdPersonCameraVerticalAngle"/> property value changes.</summary>
		public event Action<Component_GameMode> ThirdPersonCameraVerticalAngleChanged;
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
		public event Action<Component_GameMode> ThirdPersonCameraDistanceChanged;
		ReferenceField<double> _thirdPersonCameraDistance = 5;

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
		public event Action<Component_GameMode> FreeCameraChanged;
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
		public event Action<Component_GameMode> FreeCameraHotKeyChanged;
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
		public event Action<Component_GameMode> FreeCameraHotKeyValueChanged;
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
		public event Action<Component_GameMode> FreeCameraSpeedNormalChanged;
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
		public event Action<Component_GameMode> FreeCameraSpeedFastChanged;
		ReferenceField<double> _freeCameraSpeedFast = 20;

		/// <summary>
		/// Whether to display a target in the center of the screen in the first or third person camera.
		/// </summary>
		[Category( "UI" )]
		[DefaultValue( true )]
		public Reference<bool> DisplayTarget
		{
			get { if( _displayTarget.BeginGet() ) DisplayTarget = _displayTarget.Get( this ); return _displayTarget.value; }
			set { if( _displayTarget.BeginSet( ref value ) ) { try { DisplayTargetChanged?.Invoke( this ); } finally { _displayTarget.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayTarget"/> property value changes.</summary>
		public event Action<Component_GameMode> DisplayTargetChanged;
		ReferenceField<bool> _displayTarget = true;

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
		public event Action<Component_GameMode> DisplayTargetSizeChanged;
		ReferenceField<double> _displayTargetSize = 0.04;

		/// <summary>
		/// The image of the display target.
		/// </summary>
		[Category( "UI" )]
		[DefaultValueReference( @"Base\UI\Cursors\Target.png" )]
		public Reference<Component_Image> DisplayTargetImage
		{
			get { if( _displayTargetImage.BeginGet() ) DisplayTargetImage = _displayTargetImage.Get( this ); return _displayTargetImage.value; }
			set { if( _displayTargetImage.BeginSet( ref value ) ) { try { DisplayTargetImageChanged?.Invoke( this ); } finally { _displayTargetImage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayTargetImage"/> property value changes.</summary>
		public event Action<Component_GameMode> DisplayTargetImageChanged;
		ReferenceField<Component_Image> _displayTargetImage = new Reference<Component_Image>( null, @"Base\UI\Cursors\Target.png" );

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
		public event Action<Component_GameMode> DisplayTargetColorChanged;
		ReferenceField<ColorValue> _displayTargetColor = new ColorValue( 1, 1, 1, 0.5 );

		//[DefaultValue( true )]
		//public Reference<bool> DisplayHelpText
		//{
		//	get { if( _displayHelpText.BeginGet() ) DisplayHelpText = _displayHelpText.Get( this ); return _displayHelpText.value; }
		//	set { if( _displayHelpText.BeginSet( ref value ) ) { try { DisplayHelpTextChanged?.Invoke( this ); } finally { _displayHelpText.EndSet(); } } }
		//}
		//public event Action<Component_GameMode> DisplayHelpTextChanged;
		//ReferenceField<bool> _displayHelpText = true;

		[Browsable( false )]
		public ObjectInteractionContextClass ObjectInteractionContext { get; set; }

		/////////////////////////////////////////

		public class ObjectInteractionContextClass
		{
			public IComponent_InteractiveObject Obj;
			public object AnyData;
			public UIControl PlayScreen;
			public Component_GameMode GameMode;
			public Viewport Viewport;

			public ObjectInteractionContextClass( IComponent_InteractiveObject obj, UIControl playScreen, Component_GameMode gameMode, Viewport viewport )
			{
				Obj = obj;
				PlayScreen = playScreen;
				GameMode = gameMode;
				Viewport = viewport;
			}

			public virtual void Dispose() { }
		}

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

				case nameof( ThirdPersonCameraHorizontalAngle ):
				case nameof( ThirdPersonCameraVerticalAngle ):
				case nameof( ThirdPersonCameraDistance ):
					if( UseBuiltInCamera.Value != BuiltInCameraEnum.ThirdPerson )
						skip = true;
					break;

				case nameof( DisplayTargetSize ):
				case nameof( DisplayTargetImage ):
				case nameof( DisplayTargetColor ):
					if( !DisplayTarget )
						skip = true;
					break;
				}
			}
		}

		public delegate void GetCameraSettingsEventDelegate( Component_GameMode sender, Viewport viewport, Component_Camera cameraDefault, ref Viewport.CameraSettingsClass cameraSettings );
		public event GetCameraSettingsEventDelegate GetCameraSettingsEvent;

		public virtual Viewport.CameraSettingsClass GetCameraSettings( Viewport viewport, Component_Camera cameraDefault )
		{
			Viewport.CameraSettingsClass result = null;
			GetCameraSettingsEvent?.Invoke( this, viewport, cameraDefault, ref result );

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

					//Character
					var character = ObjectControlledByPlayer.Value as Component_Character;
					if( character != null )
					{
						character.GetFirstPersonCameraPosition( out var position, out var forward, out var up );

						result = new Viewport.CameraSettingsClass( viewport, cameraDefault.AspectRatio, cameraDefault.FieldOfView, cameraDefault.NearClipPlane, cameraDefault.FarClipPlane, position, forward, up, ProjectionType.Perspective, 1, cameraDefault.Exposure, cameraDefault.EmissiveFactor );
					}

				}
				else if( UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
				{
					//third person camera

					//Character
					if( Scene.Mode.Value == Component_Scene.ModeEnum._3D )
					{
						var character = ObjectControlledByPlayer.Value as Component_Character;
						if( character != null )
						{
							var lookAt = character.TransformV.Position;

							var d = new SphericalDirection( MathEx.DegreeToRadian( ThirdPersonCameraHorizontalAngle.Value ), MathEx.DegreeToRadian( ThirdPersonCameraVerticalAngle.Value ) );
							var direction = -d.GetVector();

							var from = lookAt - direction * ThirdPersonCameraDistance.Value;

							result = new Viewport.CameraSettingsClass( viewport, cameraDefault.AspectRatio, cameraDefault.FieldOfView, cameraDefault.NearClipPlane, cameraDefault.FarClipPlane, from, direction, Vector3.ZAxis, ProjectionType.Perspective, 1, cameraDefault.Exposure, cameraDefault.EmissiveFactor );
						}
					}

					//Character2D
					if( Scene.Mode.Value == Component_Scene.ModeEnum._2D )
					{
						var character = ObjectControlledByPlayer.Value as Component_Character2D;
						if( character != null )
						{
							var lookAt = character.TransformV.Position;
							var from = lookAt + new Vector3( 0, 0, 10 );

							result = new Viewport.CameraSettingsClass( viewport, cameraDefault.AspectRatio, cameraDefault.FieldOfView, cameraDefault.NearClipPlane, cameraDefault.FarClipPlane, from, -Vector3.ZAxis, Vector3.YAxis, ProjectionType.Orthographic, cameraDefault.Height, cameraDefault.Exposure, cameraDefault.EmissiveFactor );
						}
					}

				}
			}

			//Component_CameraManagement
			if( result == null )
			{
				var m = GetCurrentCameraManagement();
				if( m != null )
					result = m.GetCameraSettings( this, viewport, cameraDefault );
			}

			return result;
		}

		public delegate void InputMessageEventDelegate( Component_GameMode sender, UIControl playScreen, InputMessage message/*, ref bool handled*/ );
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

		bool ProcessInputMessageBefore( UIControl playScreen, InputMessage message )
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
					//lock( lockerKeysMouse )
					//{
					if( keys == null )
						keys = new bool[ GetEKeysMaxIndex() + 1 ];
					keys[ (int)m.Key ] = true;
					//}
				}
			}

			//key up
			{
				var m = message as InputMessageKeyUp;
				if( m != null )
				{
					//lock( lockerKeysMouse )
					//{
					if( keys == null )
						keys = new bool[ GetEKeysMaxIndex() + 1 ];

					if( keys[ (int)m.Key ] )
					{
						keys[ (int)m.Key ] = false;
					}
					//}
				}
			}

			//mouse button down
			{
				var m = message as InputMessageMouseButtonDown;
				if( m != null )
				{
					//lock( lockerKeysMouse )
					//{
					mouseButtons[ (int)m.Button ] = true;
					//}
				}
			}

			//mouse button up
			{
				var m = message as InputMessageMouseButtonUp;
				if( m != null )
				{
					//lock( lockerKeysMouse )
					//{
					mouseButtons[ (int)m.Button ] = false;
					//}
				}
			}

			//mouse move
			{
				var m = message as InputMessageMouseMove;
				if( m != null )
				{
					//lock( lockerKeysMouse )
					//{
					mousePosition = m.Position;
					//}
				}
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
				//all keys and mouse buttons up
				var viewport = playScreen.ParentContainer.Viewport;
				viewport.KeysAndMouseButtonUpAll();

				//change free camera
				FreeCamera = !FreeCamera;

				//show screen message
				if( FreeCamera )
					ScreenMessages.Add( $"Free camera is activated." );
				else
					ScreenMessages.Add( $"Free camera is deactivated." );

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
					var viewport = playScreen.ParentContainer.Viewport;
					//viewport.MouseRelativeMode = false;
					freeCameraMouseRotating = false;
					freeCameraMouseRotatingActivated = false;
				}
			}

			var mouseMove = message as InputMessageMouseMove;
			if( mouseMove != null )
			{
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

		public virtual bool ProcessInputMessage( UIControl playScreen, InputMessage message )
		{
			if( ProcessInputMessageBefore( playScreen, message ) )
				return true;

			InputMessageEvent?.Invoke( this, playScreen, message );
			if( message.Handled )
				return true;

			//Object interaction
			if( ObjectInteractionContext != null && ObjectInteractionContext.Obj.ObjectInteractionInputMessage( playScreen, this, message ) )
				return true;

			//InputProcessing
			var inputProcessing = GetCurrentInputProcessing();
			if( inputProcessing != null && inputProcessing.PerformMessage( playScreen, this, message ) )
				return true;

			return false;
		}

		public Component_InputProcessing GetCurrentInputProcessing()
		{
			var userControlledObject = ObjectControlledByPlayer.Value;
			if( userControlledObject != null )
			{
				var input = userControlledObject as Component_InputProcessing;
				if( input != null )
					return input;

				input = userControlledObject.GetComponent<Component_InputProcessing>( onlyEnabledInHierarchy: true );
				if( input != null )
					return input;
			}

			return null;
		}

		public Component_CameraManagement GetCurrentCameraManagement()
		{
			var userControlledObject = ObjectControlledByPlayer.Value;
			if( userControlledObject != null )
			{
				var m = userControlledObject as Component_CameraManagement;
				if( m != null )
					return m;

				m = userControlledObject.GetComponent<Component_CameraManagement>( onlyEnabledInHierarchy: true );
				if( m != null )
					return m;
			}

			return null;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				//get free camera initial settings
				if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
				{
					var scene = Parent as Component_Scene;
					if( scene != null )
					{
						Component_Camera camera = scene.CameraDefault;
						if( camera == null )
							camera = scene.Mode.Value == Component_Scene.ModeEnum._2D ? scene.CameraEditor2D : scene.CameraEditor;

						if( camera != null )
						{
							var tr = camera.TransformV;
							freeCameraPosition = tr.Position;
							freeCameraDirection = SphericalDirection.FromVector( tr.Rotation.GetForward() );
						}
					}
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

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
			//lock( lockerKeysMouse )
			//{
			if( keys != null )
				return keys[ (int)key ];
			else
				return false;
			//}
		}

		public bool IsMouseButtonPressed( EMouseButtons button )
		{
			//lock( lockerKeysMouse )
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

		public delegate void IsNeedMouseRelativeModeEventDelegate( Component_GameMode sender, ref bool needMouseRelativeMode );
		public event IsNeedMouseRelativeModeEventDelegate IsNeedMouseRelativeModeEvent;

		public bool IsNeedMouseRelativeMode()
		{
			bool result = false;
			IsNeedMouseRelativeModeEvent?.Invoke( this, ref result );

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

			//Component_CameraManagement
			var m = GetCurrentCameraManagement();
			if( m != null )
				result = m.IsNeedMouseRelativeMode( this );

			return result;
		}

		public virtual void PerformRender( UIControl playScreen, Viewport viewport )
		{
			if( Scene != null )
				UpdateObjectInteraction( playScreen, viewport );
		}

		public virtual void PerformRenderUI( UIControl playScreen, CanvasRenderer renderer )
		{
			if( Scene != null )
			{
				RenderObjectInteraction( playScreen, renderer );
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
		public Component_Scene Scene
		{
			get { return Parent as Component_Scene; }
		}

		public virtual IComponent_InteractiveObject_ObjectInfo GetInteractiveObjectInfo( UIControl playScreen, IComponent_InteractiveObject obj )
		{
			IComponent_InteractiveObject_ObjectInfo result = null;
			obj.ObjectInteractionGetInfo( playScreen, this, ref result );
			if( result == null )
				result = new IComponent_InteractiveObject_ObjectInfo();
			return result;
		}

		public delegate void PickInteractiveObjectEventDelegate( Component_GameMode gameMode, UIControl playScreen, Viewport viewport, ref IComponent_InteractiveObject result );
		public event PickInteractiveObjectEventDelegate PickInteractiveObjectEvent;

		public virtual IComponent_InteractiveObject PickInteractiveObject( UIControl playScreen, Viewport viewport )
		{
			IComponent_InteractiveObject result = null;
			PickInteractiveObjectEvent?.Invoke( this, playScreen, viewport, ref result );

			if( !FreeCamera )
			{
				//pick for 3D
				if( Scene.Mode.Value == Component_Scene.ModeEnum._3D )
				{
					if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
					{
						Ray ray;
						{
							double rayDistance = UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson ? 2.5 : 10;
							//scaling
							{
								var obj = ObjectControlledByPlayer.Value as Component_ObjectInSpace;
								if( obj != null )
									rayDistance *= obj.TransformV.Scale.MaxComponent();
							}

							ray = viewport.CameraSettings.GetRayByScreenCoordinates( new Vector2( 0.5, 0.5 ) );
							ray.Direction = ray.Direction.GetNormalize() * rayDistance;
						}

						var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, ray );
						Scene.GetObjectsInSpace( getObjectsItem );

						foreach( var item in getObjectsItem.Result )
						{
							var obj = item.Object.FindThisOrParent<IComponent_InteractiveObject>();
							if( obj != null )
							{
								if( GetInteractiveObjectInfo( playScreen, obj ).AllowInteract )
									result = obj;
							}
						}
					}
				}

				//pick for 2D
				if( Scene.Mode.Value == Component_Scene.ModeEnum._2D )
				{
					if( UseBuiltInCamera.Value == BuiltInCameraEnum.None || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
					{
						var character = ObjectControlledByPlayer.Value as Component_Character2D;
						if( character != null )
						{
							double maxDistance = 2.0;
							//scaling
							{
								var obj = ObjectControlledByPlayer.Value as Component_ObjectInSpace;
								if( obj != null )
									maxDistance *= obj.TransformV.Scale.MaxComponent();
							}

							var bounds = new Bounds( character.TransformV.Position );
							bounds.Expand( new Vector3( maxDistance, maxDistance, 10000 ) );

							var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, bounds );
							Scene.GetObjectsInSpace( getObjectsItem );

							foreach( var item in getObjectsItem.Result )
							{
								var obj = item.Object.FindThisOrParent<IComponent_InteractiveObject>();
								if( obj != null )
								{
									var objectInSpace = obj as Component_ObjectInSpace;
									if( objectInSpace != null )
									{
										var distance = ( objectInSpace.TransformV.Position - character.TransformV.Position ).Length();
										if( distance <= maxDistance )
										{
											if( GetInteractiveObjectInfo( playScreen, obj ).AllowInteract )
												result = obj;
										}
									}
								}
							}
						}
					}
				}
			}

			//Component_CameraManagement
			var m = GetCurrentCameraManagement();
			if( m != null )
				result = m.PickInteractiveObject( this, playScreen, viewport );

			return result;
		}

		protected virtual void UpdateObjectInteraction( UIControl playScreen, Viewport viewport )
		{
			//find interactive object on the screen
			var overObject = PickInteractiveObject( playScreen, viewport );

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
					ObjectInteractionContext = new ObjectInteractionContextClass( overObject, playScreen, this, viewport );
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

		protected virtual void RenderObjectInteraction( UIControl playScreen, CanvasRenderer renderer )
		{
			if( !FreeCamera && ObjectInteractionContext != null )
			{
				bool render = false;
				if( Scene.Mode.Value == Component_Scene.ModeEnum._3D )
					if( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
						render = true;
				if( Scene.Mode.Value == Component_Scene.ModeEnum._2D )
					if( UseBuiltInCamera.Value == BuiltInCameraEnum.None || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson )
						render = true;
				var m = GetCurrentCameraManagement();
				if( m != null )
					render = m.IsNeedRenderObjectInteraction( this, playScreen, renderer );

				if( render )
				{
					var obj = ObjectInteractionContext.Obj as Component_ObjectInSpace;
					if( obj != null )
					{
						var info = GetInteractiveObjectInfo( playScreen, ObjectInteractionContext.Obj );

						if( info.AllowInteract && info.DisplaySelectionRectangle )
						{
							//calculate screen rectangle
							var rectangle = Rectangle.Cleared;
							foreach( var point in obj.SpaceBounds.CalculatedBoundingBox.ToPoints() )
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

		protected virtual void RenderTargetImage( CanvasRenderer renderer )
		{
			//visualize target image
			if( DisplayTarget && ( UseBuiltInCamera.Value == BuiltInCameraEnum.FirstPerson || UseBuiltInCamera.Value == BuiltInCameraEnum.ThirdPerson || GetCurrentCameraManagement() != null ) && !FreeCamera && Scene.Mode.Value == Component_Scene.ModeEnum._3D )
			{
				//!!!!?
				//if( weapon != null || currentAttachedGuiObject != null || currentSwitch != null )
				{
					var size = DisplayTargetSize.Value / 2;
					var rectangle = new Rectangle(
						0.5 - size, 0.5 - size * renderer.AspectRatio,
						0.5 + size, 0.5 + size * renderer.AspectRatio );
					renderer.AddQuad( rectangle, new Rectangle( 0, 0, 1, 1 ), DisplayTargetImage, DisplayTargetColor );
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
	}
}
