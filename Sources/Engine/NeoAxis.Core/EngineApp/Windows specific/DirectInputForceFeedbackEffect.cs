// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if WINDOWS || UWP
using System;
using System.Collections.Generic;
using System.Text;
using NeoAxis;
using NeoAxis.Input;

namespace DirectInput
{
	internal class DirectInputForceFeedbackEffectUtils
	{
		//public static uint SecondsToMicroSeconds( float seconds )
		//{
		//   x;
		//   if( seconds == ForceFeedbackEffect.infiniteTime )
		//      return DInput.INFINITE;

		//   return (uint)( seconds * DInput.DI_SECONDS );
		//}

		//public static unsafe int SetParameters( ForceFeedbackEffect forceFeedbackEffect,
		//    void* typeSpecificParams, uint specificParamsSize, IDirectInputEffect* directInputEffect )
		//{
		//DIENVELOPE effectEnvelope = new DIENVELOPE();

		//if( forceFeedbackEffect.Envelope.Active )
		//{
		//   effectEnvelope.dwSize = (uint)sizeof( DIENVELOPE );
		//   effectEnvelope.dwAttackLevel =
		//       (uint)( forceFeedbackEffect.Envelope.AttackLevel * DInput.DI_FFNOMINALMAX );
		//   effectEnvelope.dwAttackTime = SecondsToMicroSeconds( forceFeedbackEffect.Envelope.AttackTime );
		//   effectEnvelope.dwFadeLevel =
		//       (uint)( forceFeedbackEffect.Envelope.FadeLevel * DInput.DI_FFNOMINALMAX );
		//   effectEnvelope.dwFadeTime = SecondsToMicroSeconds( forceFeedbackEffect.Envelope.FadeTime );
		//}

		//uint dwDuration = SecondsToMicroSeconds( forceFeedbackEffect.Duration );
		////uint dwTriggerRepeatInterval = SecondsToMicroSeconds( forceFeedbackEffect.TriggerRepeatInterval );
		//uint dwSamplePeriod = SecondsToMicroSeconds( forceFeedbackEffect.SamplePeriod );

		////uint dwTriggerButton = DInput.DIEB_NOTRIGGER;
		////if( forceFeedbackEffect.TriggerButtonEnabled )
		////   dwTriggerButton = (uint)GetOffsetByButtonType( forceFeedbackEffect.TriggerButton );

		//uint dwFlags = DInput.DIEP_TYPESPECIFICPARAMS | DInput.DIEP_DURATION | DInput.DIEP_ENVELOPE |
		//   DInput.DIEP_GAIN | DInput.DIEP_SAMPLEPERIOD;
		////uint dwFlags = DInput.DIEP_TYPESPECIFICPARAMS | DInput.DIEP_DURATION | DInput.DIEP_ENVELOPE |
		////   DInput.DIEP_GAIN | DInput.DIEP_TRIGGERBUTTON | DInput.DIEP_TRIGGERREPEATINTERVAL |
		////   DInput.DIEP_STARTDELAY | DInput.DIEP_SAMPLEPERIOD;

		//DIEFFECT diEffect = new DIEFFECT();
		//diEffect.dwSize = (uint)sizeof( DIEFFECT );
		//diEffect.dwFlags = DInput.DIEFF_CARTESIAN | DInput.DIEFF_OBJECTOFFSETS;
		//diEffect.lpEnvelope = ( forceFeedbackEffect.Envelope.Active ) ? &effectEnvelope : null;
		//diEffect.cbTypeSpecificParams = specificParamsSize;
		//diEffect.lpvTypeSpecificParams = typeSpecificParams;
		//diEffect.dwDuration = dwDuration;
		//diEffect.dwGain = forceFeedbackEffect.Gain;
		//diEffect.dwTriggerButton = DInput.DIEB_NOTRIGGER;
		////effectParams.dwTriggerButton = dwTriggerButton;
		//diEffect.dwTriggerRepeatInterval = 0;
		////effectParams.dwTriggerRepeatInterval = dwTriggerRepeatInterval;
		////diEffect.dwStartDelay = (uint)( forceFeedbackEffect.StartDelay * DInput.DI_SECONDS );
		//diEffect.dwSamplePeriod = dwSamplePeriod;

		//int hr = IDirectInputEffect.SetParameters( directInputEffect, ref diEffect, dwFlags );

		//return hr;
		//}

		//public static int GetOffsetByButtonType( JoystickButtons buttonName )
		//{
		//   int offset = (int)buttonName + DInput.DIJOFS_BUTTON0;
		//   return offset;
		//}

		static int GetOffsetByAxisType( JoystickAxes axisName )
		{
			switch( axisName )
			{
			case JoystickAxes.X:
				return DInput.DIJOFS_X;
			case JoystickAxes.Y:
				return DInput.DIJOFS_Y;
			case JoystickAxes.Z:
				return DInput.DIJOFS_Z;
			case JoystickAxes.Rx:
				return DInput.DIJOFS_RX;
			case JoystickAxes.Ry:
				return DInput.DIJOFS_RY;
			case JoystickAxes.Rz:
				return DInput.DIJOFS_RZ;

			default:
				Log.Fatal( "DirectInputForceFeedbackController: GetOffsetByAxisType " +
					"axisType \"{0}\" incorrect.", axisName );
				return -1;
			}
		}

		public unsafe static IDirectInputEffect* CreateEffect( DirectInputJoystickInputDevice device,
			ForceFeedbackEffectTypes effectType, IList<JoystickAxes> axes )
		{
			uint* pAxes = stackalloc uint[ axes.Count ];
			int* pDirections = stackalloc int[ axes.Count ];
			for( int n = 0; n < axes.Count; n++ )
			{
				pAxes[ n ] = (uint)GetOffsetByAxisType( axes[ n ] );
				pDirections[ n ] = 0;
			}

			//

			DICONSTANTFORCE diConstantForce = new DICONSTANTFORCE();
			//DICUSTOMFORCE diCustomForce = new DICUSTOMFORCE();
			DICONDITION diCondition = new DICONDITION();
			DIPERIODIC diPeriodic = new DIPERIODIC();
			DIRAMPFORCE diRamp = new DIRAMPFORCE();

			GUID effectTypeGuid = new GUID();
			DIEFFECT diEffect = new DIEFFECT();

			switch( effectType )
			{
			case ForceFeedbackEffectTypes.Spring:
				effectTypeGuid = DInput.GUID_Spring;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DICONDITION );
				diEffect.lpvTypeSpecificParams = &diCondition;
				break;

			case ForceFeedbackEffectTypes.Damper:
				effectTypeGuid = DInput.GUID_Damper;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DICONDITION );
				diEffect.lpvTypeSpecificParams = &diCondition;
				break;

			case ForceFeedbackEffectTypes.Friction:
				effectTypeGuid = DInput.GUID_Friction;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DICONDITION );
				diEffect.lpvTypeSpecificParams = &diCondition;
				break;

			case ForceFeedbackEffectTypes.Inertia:
				effectTypeGuid = DInput.GUID_Inertia;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DICONDITION );
				diEffect.lpvTypeSpecificParams = &diCondition;
				break;

			case ForceFeedbackEffectTypes.ConstantForce:
				effectTypeGuid = DInput.GUID_ConstantForce;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DICONSTANTFORCE );
				diEffect.lpvTypeSpecificParams = &diConstantForce;
				break;

			//case ForceFeedbackEffectTypes.CustomForce:
			//   effectTypeGuid = DInput.GUID_CustomForce;
			//   diEffect.cbTypeSpecificParams = (uint)sizeof( DICUSTOMFORCE );
			//   diEffect.lpvTypeSpecificParams = &diCustomForce;
			//   break;

			case ForceFeedbackEffectTypes.SawtoothDown:
				effectTypeGuid = DInput.GUID_SawtoothDown;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DIPERIODIC );
				diEffect.lpvTypeSpecificParams = &diPeriodic;
				break;

			case ForceFeedbackEffectTypes.SawtoothUp:
				effectTypeGuid = DInput.GUID_SawtoothUp;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DIPERIODIC );
				diEffect.lpvTypeSpecificParams = &diPeriodic;
				break;

			case ForceFeedbackEffectTypes.Sine:
				effectTypeGuid = DInput.GUID_Sine;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DIPERIODIC );
				diEffect.lpvTypeSpecificParams = &diPeriodic;
				break;

			case ForceFeedbackEffectTypes.Square:
				effectTypeGuid = DInput.GUID_Square;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DIPERIODIC );
				diEffect.lpvTypeSpecificParams = &diPeriodic;
				break;

			case ForceFeedbackEffectTypes.Triangle:
				effectTypeGuid = DInput.GUID_Triangle;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DIPERIODIC );
				diEffect.lpvTypeSpecificParams = &diPeriodic;
				break;

			case ForceFeedbackEffectTypes.Ramp:
				effectTypeGuid = DInput.GUID_RampForce;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DIRAMPFORCE );
				diEffect.lpvTypeSpecificParams = &diRamp;
				break;
			}

			diEffect.dwSize = (uint)sizeof( DIEFFECT );
			diEffect.dwFlags = DInput.DIEFF_CARTESIAN | DInput.DIEFF_OBJECTOFFSETS;
			diEffect.dwDuration = DInput.INFINITE;
			diEffect.dwSamplePeriod = 0;
			diEffect.dwGain = DInput.DI_FFNOMINALMAX;
			diEffect.dwTriggerButton = DInput.DIEB_NOTRIGGER;
			diEffect.dwTriggerRepeatInterval = 0;
			diEffect.cAxes = (uint)axes.Count;
			diEffect.rgdwAxes = pAxes;
			diEffect.rglDirection = pDirections;
			diEffect.lpEnvelope = null;
			diEffect.dwStartDelay = 0;
			diEffect.dwSamplePeriod = 0;

			//

			void*/*IDirectInputEffect*/ directInputEffect = null;

			int hr = IDirectInputDevice8.CreateEffect( device.directInputDevice,
				ref effectTypeGuid, ref diEffect, out directInputEffect, null );

			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputForceFeedbackController: " +
					"Cannot create ForceFeedbackEffect for \"{0}\" ({1}).", device.Name,
					DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
				return null;
			}

			return (IDirectInputEffect*)directInputEffect;
		}

		public static unsafe void UpdateEffectDirection( ForceFeedbackEffect effect,
			IDirectInputEffect* directInputEffect )
		{
			if( effect.Direction != null )
			{
				int* pDirections = stackalloc int[ effect.Direction.Count ];
				for( int n = 0; n < effect.Direction.Count; n++ )
					pDirections[ n ] = (int)( effect.Direction[ n ] * DInput.DI_FFNOMINALMAX );

				uint dwFlags = DInput.DIEP_DIRECTION;

				DIEFFECT diEffect = new DIEFFECT();
				diEffect.dwSize = (uint)sizeof( DIEFFECT );
				diEffect.dwFlags = DInput.DIEFF_CARTESIAN | DInput.DIEFF_OBJECTOFFSETS;
				diEffect.cAxes = (uint)effect.Axes.Count;
				diEffect.rglDirection = pDirections;

				int hr = IDirectInputEffect.SetParameters( directInputEffect, ref diEffect, dwFlags );

				if( Wrapper.FAILED( hr ) )
				{
					Log.Warning( "DirectInputForceFeedbackEffect: " +
						"Cannot update direction ({0}).",
						DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
					return;
				}
			}
		}

	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////

	internal class DirectInputForceFeedbackConstantForceEffect : ForceFeedbackConstantForceEffect
	{
		unsafe IDirectInputEffect* directInputEffect;

		//

		public unsafe DirectInputForceFeedbackConstantForceEffect( ForceFeedbackController owner,
			 JoystickAxes[] axes )
			: base( owner, axes )
		{
		}

		protected override unsafe bool OnCreateRealEffect()
		{
			base.OnCreateRealEffect();

			DirectInputJoystickInputDevice device = (DirectInputJoystickInputDevice)Owner.Device;
			directInputEffect = DirectInputForceFeedbackEffectUtils.CreateEffect( device, EffectType, Axes );
			return directInputEffect != null;
		}

		protected override unsafe void OnDestroyRealEffect()
		{
			if( directInputEffect != null )
			{
				IDirectInputEffect.Unload( directInputEffect );
				IDirectInputEffect.Release( directInputEffect );
				directInputEffect = null;
			}

			base.OnDestroyRealEffect();
		}

		protected override unsafe void OnStart()
		{
			base.OnStart();

			OnUpdateDirection();
			OnUpdateEffectTypeSpecificParameters();

			int hr = IDirectInputEffect.Start( directInputEffect, 1, 0 );
			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputForceFeedbackConstantForceEffect: " +
					"Cannot start Constant Force force feedback effect ({0}).",
					DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
				return;
			}
		}

		protected override unsafe void OnUpdateDirection()
		{
			base.OnUpdateDirection();

			if( directInputEffect != null )
				DirectInputForceFeedbackEffectUtils.UpdateEffectDirection( this, directInputEffect );
		}

		protected override unsafe void OnUpdateEffectTypeSpecificParameters()
		{
			base.OnUpdateEffectTypeSpecificParameters();

			if( directInputEffect != null )
			{
				DICONSTANTFORCE diConstantForce = new DICONSTANTFORCE();
				diConstantForce.lMagnitude = (int)( Magnitude * DInput.DI_FFNOMINALMAX );

				DIEFFECT diEffect = new DIEFFECT();
				diEffect.dwSize = (uint)sizeof( DIEFFECT );
				diEffect.dwFlags = DInput.DIEFF_CARTESIAN | DInput.DIEFF_OBJECTOFFSETS;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DICONSTANTFORCE );
				diEffect.lpvTypeSpecificParams = &diConstantForce;

				int hr = IDirectInputEffect.SetParameters( directInputEffect, ref diEffect,
					DInput.DIEP_TYPESPECIFICPARAMS );
				if( Wrapper.FAILED( hr ) )
				{
					Log.Warning( "DirectInputForceFeedbackConstantForceEffect: " +
						"Cannot update Constant Force effect parameters ({0}).",
						DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
					return;
				}
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////

	internal class DirectInputForceFeedbackConditionEffect : ForceFeedbackConditionEffect
	{
		unsafe IDirectInputEffect* directInputEffect;

		//

		public unsafe DirectInputForceFeedbackConditionEffect( ForceFeedbackController owner,
			 ForceFeedbackEffectTypes effectType, JoystickAxes[] axes )
			: base( owner, effectType, axes )
		{
		}

		protected override unsafe bool OnCreateRealEffect()
		{
			base.OnCreateRealEffect();

			DirectInputJoystickInputDevice device = (DirectInputJoystickInputDevice)Owner.Device;
			directInputEffect = DirectInputForceFeedbackEffectUtils.CreateEffect( device, EffectType, Axes );
			return directInputEffect != null;
		}

		protected override unsafe void OnDestroyRealEffect()
		{
			if( directInputEffect != null )
			{
				IDirectInputEffect.Unload( directInputEffect );
				IDirectInputEffect.Release( directInputEffect );
				directInputEffect = null;
			}

			base.OnDestroyRealEffect();
		}

		protected override unsafe void OnStart()
		{
			base.OnStart();

			OnUpdateDirection();
			OnUpdateEffectTypeSpecificParameters();

			int hr = IDirectInputEffect.Start( directInputEffect, 1, 0 );
			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputForceFeedbackConditionEffect: " +
					"Cannot start Condition force feedback effect ({0}).",
					DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
				return;
			}
		}

		protected override unsafe void OnUpdateDirection()
		{
			base.OnUpdateDirection();

			if( directInputEffect != null )
				DirectInputForceFeedbackEffectUtils.UpdateEffectDirection( this, directInputEffect );
		}

		protected override unsafe void OnUpdateEffectTypeSpecificParameters()
		{
			base.OnUpdateEffectTypeSpecificParameters();

			if( directInputEffect != null )
			{
				DICONDITION diCondition = new DICONDITION();
				diCondition.lOffset = (int)( Offset * DInput.DI_FFNOMINALMAX );
				diCondition.lNegativeCoefficient = (int)( NegativeCoefficient * DInput.DI_FFNOMINALMAX );
				diCondition.lPositiveCoefficient = (int)( PositiveCoefficient * DInput.DI_FFNOMINALMAX );
				diCondition.dwNegativeSaturation = (uint)( NegativeSaturation * DInput.DI_FFNOMINALMAX );
				diCondition.dwPositiveSaturation = (uint)( PositiveSaturation * DInput.DI_FFNOMINALMAX );
				diCondition.lDeadBand = (int)( DeadBand * DInput.DI_FFNOMINALMAX );

				DIEFFECT diEffect = new DIEFFECT();
				diEffect.dwSize = (uint)sizeof( DIEFFECT );
				diEffect.dwFlags = DInput.DIEFF_CARTESIAN | DInput.DIEFF_OBJECTOFFSETS;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DICONDITION );
				diEffect.lpvTypeSpecificParams = &diCondition;

				int hr = IDirectInputEffect.SetParameters( directInputEffect, ref diEffect,
					DInput.DIEP_TYPESPECIFICPARAMS );
				if( Wrapper.FAILED( hr ) )
				{
					Log.Warning( "DirectInputForceFeedbackConditionEffect: " +
						"Cannot update Condition effect parameters ({0}).",
						DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
					return;
				}
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////

	internal class DirectInputForceFeedbackPeriodicEffect : ForceFeedbackPeriodicEffect
	{
		unsafe IDirectInputEffect* directInputEffect;

		//

		public unsafe DirectInputForceFeedbackPeriodicEffect( ForceFeedbackController owner,
			 ForceFeedbackEffectTypes effectType, JoystickAxes[] axes )
			: base( owner, effectType, axes )
		{
		}

		protected override unsafe bool OnCreateRealEffect()
		{
			base.OnCreateRealEffect();

			DirectInputJoystickInputDevice device = (DirectInputJoystickInputDevice)Owner.Device;
			directInputEffect = DirectInputForceFeedbackEffectUtils.CreateEffect( device, EffectType, Axes );
			return directInputEffect != null;
		}

		protected override unsafe void OnDestroyRealEffect()
		{
			if( directInputEffect != null )
			{
				IDirectInputEffect.Unload( directInputEffect );
				IDirectInputEffect.Release( directInputEffect );
				directInputEffect = null;
			}

			base.OnDestroyRealEffect();
		}

		protected override unsafe void OnStart()
		{
			base.OnStart();

			OnUpdateDirection();
			OnUpdateEffectTypeSpecificParameters();

			int hr = IDirectInputEffect.Start( directInputEffect, 1, 0 );
			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputForceFeedbackPeriodicEffect: " +
					"Cannot start Periodic force feedback effect ({0}).",
					DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
				return;
			}
		}

		protected override unsafe void OnUpdateDirection()
		{
			base.OnUpdateDirection();

			if( directInputEffect != null )
				DirectInputForceFeedbackEffectUtils.UpdateEffectDirection( this, directInputEffect );
		}

		protected override unsafe void OnUpdateEffectTypeSpecificParameters()
		{
			base.OnUpdateEffectTypeSpecificParameters();

			if( directInputEffect != null )
			{
				DIPERIODIC diPeriodic = new DIPERIODIC();
				diPeriodic.dwMagnitude = (uint)( 10000.0f * Magnitude );
				diPeriodic.lOffset = (int)( (float)diPeriodic.dwMagnitude * Offset );
				diPeriodic.dwPhase = (uint)( 36000.0f * Phase );
				diPeriodic.dwPeriod = (uint)( Period * DInput.DI_SECONDS );

				DIEFFECT diEffect = new DIEFFECT();
				diEffect.dwSize = (uint)sizeof( DIEFFECT );
				diEffect.dwFlags = DInput.DIEFF_CARTESIAN | DInput.DIEFF_OBJECTOFFSETS;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DIPERIODIC );
				diEffect.lpvTypeSpecificParams = &diPeriodic;

				int hr = IDirectInputEffect.SetParameters( directInputEffect, ref diEffect,
					DInput.DIEP_TYPESPECIFICPARAMS );
				if( Wrapper.FAILED( hr ) )
				{
					Log.Warning( "DirectInputForceFeedbackPeriodicEffect: " +
						"Cannot update Periodic effect parameters ({0}).",
						DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
					return;
				}
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////

	internal class DirectInputForceFeedbackRampEffect : ForceFeedbackRampEffect
	{
		unsafe IDirectInputEffect* directInputEffect;

		//

		public unsafe DirectInputForceFeedbackRampEffect( ForceFeedbackController owner,
			 JoystickAxes[] axes )
			: base( owner, axes )
		{
		}

		protected override unsafe bool OnCreateRealEffect()
		{
			base.OnCreateRealEffect();

			DirectInputJoystickInputDevice device = (DirectInputJoystickInputDevice)Owner.Device;
			directInputEffect = DirectInputForceFeedbackEffectUtils.CreateEffect( device, EffectType, Axes );
			return directInputEffect != null;
		}

		protected override unsafe void OnDestroyRealEffect()
		{
			if( directInputEffect != null )
			{
				IDirectInputEffect.Unload( directInputEffect );
				IDirectInputEffect.Release( directInputEffect );
				directInputEffect = null;
			}

			base.OnDestroyRealEffect();
		}

		protected override unsafe void OnStart()
		{
			base.OnStart();

			OnUpdateDirection();
			OnUpdateEffectTypeSpecificParameters();

			int hr = IDirectInputEffect.Start( directInputEffect, 1, 0 );
			if( Wrapper.FAILED( hr ) )
			{
				Log.Warning( "DirectInputForceFeedbackRampEffect: " +
					"Cannot start Ramp force feedback effect ({0}).",
					DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
				return;
			}
		}

		protected override unsafe void OnUpdateDirection()
		{
			base.OnUpdateDirection();

			if( directInputEffect != null )
				DirectInputForceFeedbackEffectUtils.UpdateEffectDirection( this, directInputEffect );
		}

		protected override unsafe void OnUpdateEffectTypeSpecificParameters()
		{
			base.OnUpdateEffectTypeSpecificParameters();

			if( directInputEffect != null )
			{
				DIRAMPFORCE diRamp = new DIRAMPFORCE();
				diRamp.lStart = (int)( StartForce * DInput.DI_FFNOMINALMAX );
				diRamp.lEnd = (int)( EndForce * DInput.DI_FFNOMINALMAX );

				DIEFFECT diEffect = new DIEFFECT();
				diEffect.dwSize = (uint)sizeof( DIEFFECT );
				diEffect.dwFlags = DInput.DIEFF_CARTESIAN | DInput.DIEFF_OBJECTOFFSETS;
				diEffect.cbTypeSpecificParams = (uint)sizeof( DIRAMPFORCE );
				diEffect.lpvTypeSpecificParams = &diRamp;

				int hr = IDirectInputEffect.SetParameters( directInputEffect, ref diEffect,
					DInput.DIEP_TYPESPECIFICPARAMS );
				if( Wrapper.FAILED( hr ) )
				{
					Log.Warning( "DirectInputForceFeedbackRampEffect: " +
						"Cannot update Ramp effect parameters ({0}).",
						DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
					return;
				}
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////

	//internal class DirectInputForceFeedbackCustomForceEffect : ForceFeedbackCustomForceEffect
	//{
	//   unsafe IDirectInputEffect* directInputEffect;

	//   //

	//   public unsafe DirectInputForceFeedbackCustomForceEffect( ForceFeedbackController owner,
	//      JoystickAxes[] axes )
	//      : base( owner, axes )
	//   {
	//   }

	//   protected override unsafe bool OnCreateRealEffect()
	//   {
	//      base.OnCreateRealEffect();

	//      DirectInputJoystickInputDevice device = (DirectInputJoystickInputDevice)Owner.Device;
	//      directInputEffect = DirectInputForceFeedbackEffectUtils.CreateEffect( device, EffectType, Axes );
	//      return directInputEffect != null;
	//   }

	//   protected override unsafe void OnDestroyRealEffect()
	//   {
	//      if( directInputEffect != null )
	//      {
	//         IDirectInputEffect.Unload( directInputEffect );
	//         IDirectInputEffect.Release( directInputEffect );
	//         directInputEffect = null;
	//      }

	//      base.OnDestroyRealEffect();
	//   }

	//   protected override unsafe void OnStart()
	//   {
	//      base.OnStart();

	//      OnUpdateAxesDirections();
	//      OnUpdateEffectTypeSpecificParameters();

	//      int hr = IDirectInputEffect.Start( directInputEffect, 1, 0 );
	//      if( Wrapper.FAILED( hr ) )
	//      {
	//         Log.Warning( "DirectInputForceFeedbackCustomForceEffect: " +
	//            "Cannot start Custom force feedback effect ({0}).",
	//            DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
	//         return;
	//      }
	//   }

	//   protected override unsafe void OnUpdateAxesDirections()
	//   {
	//      base.OnUpdateAxesDirections();

	//      if( directInputEffect != null )
	//         DirectInputForceFeedbackEffectUtils.UpdateEffectAxisDirections( this, directInputEffect );
	//   }

	//   protected override unsafe void OnUpdateEffectTypeSpecificParameters()
	//   {
	//      base.OnUpdateEffectTypeSpecificParameters();

	//      if( directInputEffect != null )
	//      {
	//         x;
	//      }
	//   }

	//   //protected unsafe bool ApplyParams()
	//   //{
	//   //   IntPtr rglSampleData = NativeUtils.Alloc( (int)( sizeof( int ) * samples ) );
	//   //   int* pSampleData = (int*)rglSampleData;

	//   //   for( int i = 0; i < samples; i++ )
	//   //      pSampleData[ i ] = (int)( DInput.DI_FFNOMINALMAX * sampleData[ i ] );

	//   //   uint dwSamplePeriod = DirectInputForceFeedbackEffectUtils.SecondsToMicroSeconds( SamplePeriod );

	//   //   DICUSTOMFORCE customForceParams = new DICUSTOMFORCE();
	//   //   customForceParams.cChannels = (uint)channels;
	//   //   customForceParams.cSamples = (uint)samples;
	//   //   customForceParams.dwSamplePeriod = dwSamplePeriod;
	//   //   customForceParams.rglForceData = pSampleData;

	//   //   int hr = DirectInputForceFeedbackEffectUtils.SetParameters( this, &customForceParams,
	//   //       (uint)sizeof( DICUSTOMFORCE ), directInputEffect );

	//   //   NativeUtils.Free( rglSampleData );

	//   //   if( Wrapper.FAILED( hr ) )
	//   //   {
	//   //      x;
	//   //      Log.Warning( "DirectInputForceFeedbackCustomForceEffect: " +
	//   //         "Cannot set IDirectInputEffect params ({0}).",
	//   //         DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
	//   //      return false;
	//   //   }

	//   //   needUpdateParameters = false;

	//   //   return true;
	//   //}
	//}

}
#endif