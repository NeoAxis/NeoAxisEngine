// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NeoAxis
{
	public class ForceFeedbackController
	{
		JoystickInputDevice device;
		bool[] supportedEffects;
		List<ForceFeedbackEffect> effects = new List<ForceFeedbackEffect>();
		ReadOnlyCollection<ForceFeedbackEffect> effectsAsReadOnly;

		//

		public ForceFeedbackController( JoystickInputDevice device )
		{
			this.device = device;
			effectsAsReadOnly = new ReadOnlyCollection<ForceFeedbackEffect>( effects );

			int forceFeedbackEffectTypesCount = Enum.GetValues( typeof( ForceFeedbackEffectTypes ) ).Length;
			supportedEffects = new bool[ forceFeedbackEffectTypesCount ];
		}

		public JoystickInputDevice Device
		{
			get { return device; }
		}

		protected void SetEffectSupported( ForceFeedbackEffectTypes effectType )
		{
			supportedEffects[ (int)effectType ] = true;
		}

		public bool IsEffectSupported( ForceFeedbackEffectTypes effectType )
		{
			return supportedEffects[ (int)effectType ];
		}

		protected virtual ForceFeedbackEffect OnCreateEffect( ForceFeedbackEffectTypes effectType,
			JoystickAxes[] axes )
		{
			return null;
		}

		public ForceFeedbackEffect CreateEffect( ForceFeedbackEffectTypes effectType, JoystickAxes[] axes )
		{
			ForceFeedbackEffect effect = OnCreateEffect( effectType, axes );
			if( effect == null )
				return null;

			effects.Add( effect );

			if( !device.IsDeviceLost() )
			{
				if( !effect.CallOnCreateRealEffect() )
				{
					effect.Destroy();
					return null;
				}
			}

			return effect;
		}

		public virtual bool SetEnableAutocenter( bool enable )
		{
			return false;
		}

		public virtual bool HaveRumble
		{
			get { return false; }
		}

		public virtual bool SetRumbleSpeed( float leftMotor, float rightMotor )
		{
			return false;
		}

		internal void OnUpdateState()
		{
			for( int n = 0; n < effects.Count; n++ )
			{
				ForceFeedbackEffect effect = effects[ n ];

				effect.OnUpdateState();
				if( effect.Destroyed )
					n--;
			}
		}

		public IList<ForceFeedbackEffect> Effects
		{
			get { return effectsAsReadOnly; }
		}

		internal void OnDeviceLost()
		{
			List<ForceFeedbackEffect> list = new List<ForceFeedbackEffect>( effects );
			foreach( ForceFeedbackEffect effect in list )
			{
				if( !effect.Destroyed )
					effect.OnDeviceLost();
			}
		}

		internal void OnDeviceRestore()
		{
			List<ForceFeedbackEffect> list = new List<ForceFeedbackEffect>( effects );
			foreach( ForceFeedbackEffect effect in list )
			{
				if( !effect.Destroyed )
					effect.OnDeviceRestore();
			}
		}

		internal void RemoveCreatedEffect( ForceFeedbackEffect effect )
		{
			effects.Remove( effect );
		}
	}
}
