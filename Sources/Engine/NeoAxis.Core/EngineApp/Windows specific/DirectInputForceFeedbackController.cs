// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if WINDOWS || UWP
using System;
using System.Collections.Generic;
using System.Text;
using NeoAxis;

namespace DirectInput
{
	internal class DirectInputForceFeedbackController : ForceFeedbackController
	{
		public unsafe DirectInputForceFeedbackController( IDirectInputDevice8* directInputDevice,
			JoystickInputDevice joystick )
			: base( joystick )
		{
			int hr = IDirectInputDevice8.EnumEffects( directInputDevice, EnumEffectHandler, null,
				 DInput.DIEFT_ALL );
			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputForceFeedbackController: " +
					"Cannot enum ForceFeedbackEffects for \"{0}\" ({1}).", Device.Name,
					DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
			}
		}

		unsafe bool EnumEffectHandler( IntPtr /*LPCDIEFFECTINFO*/ lpdei, void* pvRef )
		{
			DIEFFECTINFO* effectInfo = (DIEFFECTINFO*)lpdei.ToPointer();

			GUID effectGUID = effectInfo->guid;

			if( effectGUID == DInput.GUID_ConstantForce )
			{
				SetEffectSupported( ForceFeedbackEffectTypes.ConstantForce );
			}
			if( effectGUID == DInput.GUID_RampForce )
			{
				SetEffectSupported( ForceFeedbackEffectTypes.Ramp );
			}
			if( effectGUID == DInput.GUID_Square )
			{
				SetEffectSupported( ForceFeedbackEffectTypes.Square );
			}
			if( effectGUID == DInput.GUID_Sine )
			{
				SetEffectSupported( ForceFeedbackEffectTypes.Sine );
			}
			if( effectGUID == DInput.GUID_Triangle )
			{
				SetEffectSupported( ForceFeedbackEffectTypes.Triangle );
			}
			if( effectGUID == DInput.GUID_SawtoothUp )
			{
				SetEffectSupported( ForceFeedbackEffectTypes.SawtoothUp );
			}
			if( effectGUID == DInput.GUID_SawtoothDown )
			{
				SetEffectSupported( ForceFeedbackEffectTypes.SawtoothDown );
			}
			if( effectGUID == DInput.GUID_Spring )
			{
				SetEffectSupported( ForceFeedbackEffectTypes.Spring );
			}
			if( effectGUID == DInput.GUID_Damper )
			{
				SetEffectSupported( ForceFeedbackEffectTypes.Damper );
			}
			if( effectGUID == DInput.GUID_Inertia )
			{
				SetEffectSupported( ForceFeedbackEffectTypes.Inertia );
			}
			if( effectGUID == DInput.GUID_Friction )
			{
				SetEffectSupported( ForceFeedbackEffectTypes.Friction );
			}
			//if( effectGUID == DInput.GUID_CustomForce )
			//{
			//   SetEffectSupported( ForceFeedbackEffectTypes.CustomForce );
			//}

			return true; //continue
		}

		protected override ForceFeedbackEffect OnCreateEffect( ForceFeedbackEffectTypes effectType,
			JoystickAxes[] axes )
		{
			ForceFeedbackEffect effect = null;

			switch( effectType )
			{
			case ForceFeedbackEffectTypes.Spring:
				effect = new DirectInputForceFeedbackConditionEffect(
					this, ForceFeedbackEffectTypes.Spring, axes );
				break;

			case ForceFeedbackEffectTypes.Damper:
				effect = new DirectInputForceFeedbackConditionEffect(
					this, ForceFeedbackEffectTypes.Damper, axes );
				break;

			case ForceFeedbackEffectTypes.Friction:
				effect = new DirectInputForceFeedbackConditionEffect(
					this, ForceFeedbackEffectTypes.Friction, axes );
				break;

			case ForceFeedbackEffectTypes.Inertia:
				effect = new DirectInputForceFeedbackConditionEffect(
					this, ForceFeedbackEffectTypes.Inertia, axes );
				break;

			case ForceFeedbackEffectTypes.ConstantForce:
				effect = new DirectInputForceFeedbackConstantForceEffect( this, axes );
				break;

			//case ForceFeedbackEffectTypes.CustomForce:
			//   effect = new DirectInputForceFeedbackCustomForceEffect( this, axes );
			//   break;

			case ForceFeedbackEffectTypes.SawtoothDown:
				effect = new DirectInputForceFeedbackPeriodicEffect(
					this, ForceFeedbackEffectTypes.SawtoothDown, axes );
				break;

			case ForceFeedbackEffectTypes.SawtoothUp:
				effect = new DirectInputForceFeedbackPeriodicEffect(
					this, ForceFeedbackEffectTypes.SawtoothUp, axes );
				break;

			case ForceFeedbackEffectTypes.Sine:
				effect = new DirectInputForceFeedbackPeriodicEffect(
					this, ForceFeedbackEffectTypes.Sine, axes );
				break;

			case ForceFeedbackEffectTypes.Square:
				effect = new DirectInputForceFeedbackPeriodicEffect(
					this, ForceFeedbackEffectTypes.Square, axes );
				break;

			case ForceFeedbackEffectTypes.Triangle:
				effect = new DirectInputForceFeedbackPeriodicEffect(
					this, ForceFeedbackEffectTypes.Triangle, axes );
				break;

			case ForceFeedbackEffectTypes.Ramp:
				effect = new DirectInputForceFeedbackRampEffect( this, axes );
				break;

			default:
				Log.Fatal( "DirectInputForceFeedbackController: OnCreateEffect: not implemented." );
				break;
			}

			return effect;
		}

		public unsafe override bool SetEnableAutocenter( bool enable )
		{
			IDirectInputDevice8* directInputDevice = 
				( (DirectInputJoystickInputDevice)Device ).directInputDevice;

			DIPROPDWORD dipdw = new DIPROPDWORD();
			dipdw.diph.dwSize = (uint)sizeof( DIPROPDWORD );
			dipdw.diph.dwHeaderSize = (uint)sizeof( DIPROPHEADER );
			dipdw.diph.dwObj = 0;
			dipdw.diph.dwHow = DInput.DIPH_DEVICE;
			dipdw.dwData = (uint)( ( enable ) ? 1 : 0 );

			GUID* centerProp = (GUID*)DInput.getDIPROP_AUTOCENTER();
			int hr = IDirectInputDevice8.SetProperty( directInputDevice, centerProp, ref dipdw.diph );
			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputForceFeedbackController: " +
					"Cannot change autocenter property for \"{0}\".", Device.Name );
				return false;
			}

			return true;
		}
	}
}
#endif