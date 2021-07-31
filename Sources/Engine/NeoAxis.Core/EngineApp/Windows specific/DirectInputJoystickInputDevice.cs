// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if WINDOWS || UWP
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using NeoAxis;
using NeoAxis.Input;

namespace DirectInput
{
	internal class DirectInputJoystickInputDevice : JoystickInputDevice
	{
		const float MaxRange = 32768.0f;
		const int BufferSize = 124;

		//

		uint temporarySliderCount;
		List<JoystickInputDevice.Axis> temporaryAxisList;

		IntPtr deviceDataBuffer;

		GUID deviceGuid;
		internal unsafe IDirectInputDevice8* directInputDevice;

		static DirectInputJoystickInputDevice tempDeviceForEnumerate;

		//

		internal DirectInputJoystickInputDevice( string name, GUID deviceGuid )
			: base( name )
		{
			this.deviceGuid = deviceGuid;
		}

		internal unsafe bool Init()
		{
			GUID devGuid = deviceGuid;

			void*/*IDirectInputDevice8*/ directInputDeviceTemp = null;

			int hr = IDirectInput.CreateDevice(
				WindowsInputDeviceManager.Instance.DirectInput,
				ref devGuid, out directInputDeviceTemp, null );

			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputJoystickDevice: Cannot create device \"{0}\" ({1}).",
					Name, DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
				return false;
			}

			directInputDevice = (IDirectInputDevice8*)directInputDeviceTemp;

			// get capabilities

			DIDEVCAPS caps = new DIDEVCAPS();
			caps.dwSize = (uint)sizeof( DIDEVCAPS );

			hr = IDirectInputDevice8.GetCapabilities( directInputDevice, ref caps );
			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputJoystickDevice: Cannot get device capabilities \"{0}\".", Name );
				return false;
			}

			//buttons
			Button[] buttons = new Button[ caps.dwButtons ];
			for( int n = 0; n < buttons.Length; n++ )
				buttons[ n ] = new Button( (JoystickButtons)n, n );

			//povs
			POV[] povs = new POV[ caps.dwPOVs ];
			for( int n = 0; n < povs.Length; n++ )
				povs[ n ] = new JoystickInputDevice.POV( (JoystickPOVs)n );

			// setup

			hr = IDirectInputDevice8.SetDataFormat( directInputDevice, DInput.Get_c_dfDIJoystick2() );

			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputJoystickDevice: Cannot set device data format \"{0}\".", Name );
				return false;
			}

			hr = IDirectInputDevice8.SetCooperativeLevel( directInputDevice,
				WindowsInputDeviceManager.Instance.WindowHandle,
				DInput.DISCL_EXCLUSIVE | DInput.DISCL_FOREGROUND );

			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputJoystickDevice: Cannot set device " +
					"cooperative level \"{0}\".", Name );
				return false;
			}

			//-------------------------------------------------------------------
			// setup size for buffered input

			DIPROPDWORD dibuf = new DIPROPDWORD();
			dibuf.diph.dwSize = (uint)sizeof( DIPROPDWORD );
			dibuf.diph.dwHeaderSize = (uint)sizeof( DIPROPHEADER );
			dibuf.diph.dwHow = DInput.DIPH_DEVICE;
			dibuf.diph.dwObj = 0;
			dibuf.dwData = BufferSize;

			GUID* bufferSizeGuid = (GUID*)DInput.getDIPROP_BUFFERSIZE();

			hr = IDirectInputDevice8.SetProperty( directInputDevice, bufferSizeGuid, ref dibuf.diph );
			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputJoystickDevice: Cannot set device buffer size \"{0}\".",
					Name );
				return false;
			}

			deviceDataBuffer = NativeUtility.Alloc(NativeUtility.MemoryAllocationType.Utility,
				sizeof( DIDEVICEOBJECTDATA ) * BufferSize );

			//--------------------------------------------------------------------

			temporarySliderCount = 0;

			temporaryAxisList = new List<JoystickInputDevice.Axis>();

			tempDeviceForEnumerate = this;
			hr = IDirectInputDevice8.EnumObjects( directInputDevice, EnumDeviceObjectsHandler,
				null, DInput.DIDFT_ALL );
			tempDeviceForEnumerate = null;

			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputJoystickDevice: Cannot enumerate device objects \"{0}\".",
					Name );
				return false;
			}

			//axes
			Axis[] axes = temporaryAxisList.ToArray();
			temporaryAxisList = null;

			//sliders
			Slider[] sliders = new Slider[ temporarySliderCount ];
			for( int n = 0; n < sliders.Length; n++ )
				sliders[ n ] = new JoystickInputDevice.Slider( (JoystickSliders)n );

			//forceFeedbackController
			ForceFeedbackController forceFeedbackController = null;
			if( ( caps.dwFlags & DInput.DIDC_FORCEFEEDBACK ) != 0 )
				forceFeedbackController = new DirectInputForceFeedbackController( directInputDevice, this );

			//initialize data
			InitDeviceData( buttons, axes, povs, sliders, forceFeedbackController );

			return true;
		}

		protected unsafe override void OnShutdown()
		{
			if( directInputDevice != null )
			{
				IDirectInputDevice8.Unacquire( directInputDevice );
				IDirectInputDevice8.Release( directInputDevice );
				directInputDevice = null;
			}

			if( deviceDataBuffer != IntPtr.Zero )
			{
				NativeUtility.Free( deviceDataBuffer );
				deviceDataBuffer = IntPtr.Zero;
			}
		}

		static JoystickAxes GetJoystickAxisNameByGUID( GUID guid )
		{
			if( guid == DInput.GUID_XAxis )
				return JoystickAxes.X;
			if( guid == DInput.GUID_YAxis )
				return JoystickAxes.Y;
			if( guid == DInput.GUID_ZAxis )
				return JoystickAxes.Z;
			if( guid == DInput.GUID_RxAxis )
				return JoystickAxes.Rx;
			if( guid == DInput.GUID_RyAxis )
				return JoystickAxes.Ry;
			if( guid == DInput.GUID_RzAxis )
				return JoystickAxes.Rz;

			Log.Fatal( "DirectInputJoystickInputDevice: GetJoystickAxisNameByGUID: " +
				"Unknown axis type." );
			return JoystickAxes.X;
		}

		unsafe static bool EnumDeviceObjectsHandler( IntPtr /*DIDEVICEOBJECTINSTANCE*/ lpddoi, void* pvRef )
		{
			DirectInputJoystickInputDevice device = tempDeviceForEnumerate;

			DIDEVICEOBJECTINSTANCE* deviceObjectInstance = (DIDEVICEOBJECTINSTANCE*)lpddoi;

			if( ( deviceObjectInstance->dwType & DInput.DIDFT_AXIS ) != 0 )
			{
				int hr;

				// set range

				DIPROPRANGE diPropRange = new DIPROPRANGE();
				diPropRange.diph.dwSize = (uint)sizeof( DIPROPRANGE );
				diPropRange.diph.dwHeaderSize = (uint)sizeof( DIPROPHEADER );
				diPropRange.diph.dwHow = DInput.DIPH_BYID;
				diPropRange.diph.dwObj = deviceObjectInstance->dwType;
				diPropRange.lMin = -(int)MaxRange;
				diPropRange.lMax = +(int)MaxRange;

				GUID* propRangeGuid = (GUID*)DInput.getDIPROP_RANGE();

				hr = IDirectInputDevice8.SetProperty_DIPROPRANGE( device.directInputDevice,
					propRangeGuid, ref diPropRange );
				//hr = IDirectInputDevice8.SetProperty( device.directInputDevice,
				//   propRangeGuid, ref diPropRange.diph );

				if( Wrapper.FAILED( hr ) )
				{
					Log.Warning( "DirectInputJoystickInputDevice: Cannot set axis range for \"{0}\" ({1}).",
						device.Name, DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
				}

				// set axis type

				//uint userData = 0xFFFFFFFF;

				if( deviceObjectInstance->guidType == DInput.GUID_Slider )
				{
					device.temporarySliderCount++;
				}

				if( deviceObjectInstance->guidType == DInput.GUID_XAxis ||
					deviceObjectInstance->guidType == DInput.GUID_YAxis ||
					deviceObjectInstance->guidType == DInput.GUID_ZAxis ||
					deviceObjectInstance->guidType == DInput.GUID_RxAxis ||
					deviceObjectInstance->guidType == DInput.GUID_RyAxis ||
					deviceObjectInstance->guidType == DInput.GUID_RzAxis )
				{
					// set dead zone

					//DIPROPDWORD deadZone = new DIPROPDWORD();
					//deadZone.diph.dwSize = (uint)sizeof( DIPROPDWORD );
					//deadZone.diph.dwHeaderSize = (uint)sizeof( DIPROPHEADER );
					//deadZone.diph.dwHow = DInput.DIPH_BYID;
					//deadZone.diph.dwObj = deviceObjectInstance->dwType; // Specify the enumerated axis
					//deadZone.dwData = 500;	// dead zone of 5%

					//GUID* propDeadZone = (GUID*)DInput.getDIPROP_DEADZONE();

					//hr = IDirectInputDevice8.SetProperty( joystickDevice,
					//   propDeadZone, ref deadZone.diph );
					//if( Wrapper.FAILED( hr ) )
					//{
					//   Log.Error( "Cannot set axis dead zone for '" + Name + "'" );
					//}

					// type settings

					//userData = 0x80000000 | (uint)device.temporaryAxisList.Count;

					JoystickAxes axisName = GetJoystickAxisNameByGUID( deviceObjectInstance->guidType );

					RangeF range = new RangeF( -1, 1 );

					bool forceFeedbackSupported =
						( deviceObjectInstance->dwFlags & DInput.DIDOI_FFACTUATOR ) != 0;

					JoystickInputDevice.Axis axis = new JoystickInputDevice.Axis(
						axisName, range, forceFeedbackSupported );

					device.temporaryAxisList.Add( axis );
				}

				//// set user data

				//DIPROPPOINTER diptr = new DIPROPPOINTER();
				//diptr.diph.dwSize = (uint)sizeof( DIPROPPOINTER );
				//diptr.diph.dwHeaderSize = (uint)sizeof( DIPROPHEADER );
				//diptr.diph.dwHow = DInput.DIPH_BYID;
				//diptr.diph.dwObj = deviceObjectInstance->dwType;
				////if( IntPtr.Size == 8 )
				////{
				////   UInt64 v64 = userData;
				////   NativeUtils.CopyMemory( (IntPtr)( &diptr.uData ), (IntPtr)( &v64 ), IntPtr.Size );
				////}
				////else
				////{
				////   NativeUtils.CopyMemory( (IntPtr)( &diptr.uData ), (IntPtr)( &userData ), IntPtr.Size );
				////}
				//diptr.uData = IntPtr.Zero;
				////diptr.uData = (IntPtr)userData;

				//GUID* appDataGuid = (GUID*)DInput.getDIPROP_APPDATA();

				//hr = IDirectInputDevice8.SetProperty( device.directInputDevice,
				//   appDataGuid, ref diptr.diph );
				//if( Wrapper.FAILED( hr ) )
				//{
				//   Log.InvisibleInfo( "DirectInputJoystickDevice: Cannot set appData for \"{0}\".",
				//      device.Name );
				//   //Log.Warning( "DirectInputJoystickDevice: Cannot set appData for \"{0}\".",
				//   //   device.Name );
				//}

			}
			return true; // continue
		}

		unsafe protected override void OnUpdateState()
		{
			int hr;

			// gain access to device
			hr = IDirectInputDevice8.Poll( directInputDevice );
			if( Wrapper.FAILED( hr ) )
			{
				if( hr == DInput.GetHRESULT_DIERR_INPUTLOST() )
				{
					if( !IsDeviceLost() )
					{
						DeviceLost();
						return;
					}
				}

				hr = IDirectInputDevice8.Acquire( directInputDevice );
				while( hr == DInput.GetHRESULT_DIERR_INPUTLOST() )
					hr = IDirectInputDevice8.Acquire( directInputDevice );
				IDirectInputDevice8.Poll( directInputDevice );
			}
			else
			{
				if( IsDeviceLost() )
					DeviceRestore();
			}

			// get data	

			uint entries = BufferSize;
			DIDEVICEOBJECTDATA* entryPtr = (DIDEVICEOBJECTDATA*)deviceDataBuffer;

			hr = IDirectInputDevice8.GetDeviceData( directInputDevice,
				(uint)sizeof( DIDEVICEOBJECTDATA ), entryPtr, ref entries, 0 );
			if( Wrapper.FAILED( hr ) )
			{
				//Log.Info( "Cannot get device data for '" + Name + "' error = " +
				//   DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
				return;
			}

			// process data

			for( int k = 0; k < entries; k++ )
			{
				switch( entryPtr->dwOfs )
				{
				case DInput.DIJOFS_X:
				case DInput.DIJOFS_Y:
				case DInput.DIJOFS_Z:
				case DInput.DIJOFS_RX:
				case DInput.DIJOFS_RY:
				case DInput.DIJOFS_RZ:
					{
						JoystickAxes axisName = JoystickAxes.X;
						switch( entryPtr->dwOfs )
						{
						case DInput.DIJOFS_X: axisName = JoystickAxes.X; break;
						case DInput.DIJOFS_Y: axisName = JoystickAxes.Y; break;
						case DInput.DIJOFS_Z: axisName = JoystickAxes.Z; break;
						case DInput.DIJOFS_RX: axisName = JoystickAxes.Rx; break;
						case DInput.DIJOFS_RY: axisName = JoystickAxes.Ry; break;
						case DInput.DIJOFS_RZ: axisName = JoystickAxes.Rz; break;
						}

						Axis axis = GetAxisByName( axisName );

						float value = (float)( (int)entryPtr->dwData ) / MaxRange;

						//invert value for specific axes
						if( axis.Name == JoystickAxes.Y || axis.Name == JoystickAxes.Ry ||
							axis.Name == JoystickAxes.Rz )
						{
							value = -value;
						}

						axis.Value = value;

						InputDeviceManager.Instance.SendEvent( new JoystickAxisChangedEvent( this, axis ) );
					}
					break;

				case DInput.DIJOFS_SLIDER00:
					{
						Vector2F value = Sliders[ 0 ].Value;
						value.X = -(float)( (int)entryPtr->dwData ) / MaxRange;
						Sliders[ 0 ].Value = value;
						InputDeviceManager.Instance.SendEvent(
							new JoystickSliderChangedEvent( this, Sliders[ 0 ],JoystickSliderAxes.X ) );
					}
					break;

				case DInput.DIJOFS_SLIDER01:
					{
						Vector2F value = Sliders[ 0 ].Value;
						value.Y = -(float)( (int)entryPtr->dwData ) / MaxRange;
						Sliders[ 0 ].Value = value;
						InputDeviceManager.Instance.SendEvent(
                            new JoystickSliderChangedEvent(this, Sliders[0], JoystickSliderAxes.Y ) );
					}
					break;

				case DInput.DIJOFS_SLIDER10:
					{
						Vector2F value = Sliders[ 1 ].Value;
						value.X = -(float)( (int)entryPtr->dwData ) / MaxRange;
						Sliders[ 1 ].Value = value;
						InputDeviceManager.Instance.SendEvent(
                            new JoystickSliderChangedEvent(this, Sliders[1], JoystickSliderAxes.X ) );
					}
					break;

				case DInput.DIJOFS_SLIDER11:
					{
						Vector2F value = Sliders[ 1 ].Value;
						value.Y = -(float)( (int)entryPtr->dwData ) / MaxRange;
						Sliders[ 1 ].Value = value;
						InputDeviceManager.Instance.SendEvent(
                            new JoystickSliderChangedEvent(this, Sliders[1], JoystickSliderAxes.Y ) );
					}
					break;

				case DInput.DIJOFS_SLIDER20:
					{
						Vector2F value = Sliders[ 2 ].Value;
						value.X = -(float)( (int)entryPtr->dwData ) / MaxRange;
						Sliders[ 2 ].Value = value;
						InputDeviceManager.Instance.SendEvent(
                            new JoystickSliderChangedEvent(this, Sliders[2], JoystickSliderAxes.X) );
					}
					break;

				case DInput.DIJOFS_SLIDER21:
					{
						Vector2F value = Sliders[ 2 ].Value;
						value.Y = -(float)( (int)entryPtr->dwData ) / MaxRange;
						Sliders[ 2 ].Value = value;
						InputDeviceManager.Instance.SendEvent(
                            new JoystickSliderChangedEvent(this, Sliders[2], JoystickSliderAxes.Y ) );
					}
					break;

				case DInput.DIJOFS_SLIDER30:
					{
						Vector2F value = Sliders[ 3 ].Value;
						value.X = -(float)( (int)entryPtr->dwData ) / MaxRange;
						Sliders[ 3 ].Value = value;
						InputDeviceManager.Instance.SendEvent(
                            new JoystickSliderChangedEvent(this, Sliders[3], JoystickSliderAxes.X ) );
					}
					break;

				case DInput.DIJOFS_SLIDER31:
					{
						Vector2F value = Sliders[ 3 ].Value;
						value.Y = -(float)( (int)entryPtr->dwData ) / MaxRange;
						Sliders[ 3 ].Value = value;
						InputDeviceManager.Instance.SendEvent(
                            new JoystickSliderChangedEvent(this, Sliders[3], JoystickSliderAxes.Y ) );
					}
					break;

				case DInput.DIJOFS_POV0:
					UpdatePOV( 0, entryPtr->dwData );
					break;

				case DInput.DIJOFS_POV1:
					UpdatePOV( 1, entryPtr->dwData );
					break;

				case DInput.DIJOFS_POV2:
					UpdatePOV( 2, entryPtr->dwData );
					break;

				case DInput.DIJOFS_POV3:
					UpdatePOV( 3, entryPtr->dwData );
					break;

				default:
					if( entryPtr->dwOfs >= DInput.DIJOFS_BUTTON0 && entryPtr->dwOfs < DInput.DIJOFS_BUTTON128 )
					{
						int buttonIndex = (int)( entryPtr->dwOfs - DInput.DIJOFS_BUTTON0 );
						bool pressed = ( ( entryPtr->dwData & 0x80 ) != 0 );

						Button button = Buttons[ buttonIndex ];
						if( button.Pressed != pressed )
						{
							button.Pressed = pressed;

							if( pressed )
							{
								InputDeviceManager.Instance.SendEvent(
									new JoystickButtonDownEvent( this, button ) );
							}
							else
							{
								InputDeviceManager.Instance.SendEvent(
									new JoystickButtonUpEvent( this, button ) );
							}
						}
					}
					break;
				}

				entryPtr++;
			}

			//update states for effects. effects can be destroyed inside OnUpdateState().
			if( ForceFeedbackController != null )
				ForceFeedbackController.OnUpdateState();
		}

		void UpdatePOV( int povIndex, uint povData )
		{
			JoystickPOVDirections direction;

			if( ( povData & 0xFFFF ) == 0xFFFF )
			{
				direction = JoystickPOVDirections.Centered;
			}
			else
			{
				switch( povData )
				{
				case 0:
				case 36000: direction = JoystickPOVDirections.North; break;
				case 4500: direction = JoystickPOVDirections.North | JoystickPOVDirections.East; break;
				case 9000: direction = JoystickPOVDirections.East; break;
				case 13500: direction = JoystickPOVDirections.South | JoystickPOVDirections.East; break;
				case 18000: direction = JoystickPOVDirections.South; break;
				case 22500: direction = JoystickPOVDirections.South | JoystickPOVDirections.West; break;
				case 27000: direction = JoystickPOVDirections.West; break;
				case 31500: direction = JoystickPOVDirections.North | JoystickPOVDirections.West; break;
				default: direction = JoystickPOVDirections.Centered; break;
				}
			}

			POVs[ povIndex ].Value = direction;

			InputDeviceManager.Instance.SendEvent( new JoystickPOVChangedEvent( this, POVs[ povIndex ] ) );
		}
	}
}
#endif