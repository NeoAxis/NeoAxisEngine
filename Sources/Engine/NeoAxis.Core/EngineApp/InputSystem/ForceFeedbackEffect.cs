// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Runtime.InteropServices;

namespace NeoAxis.Input
{
	public enum ForceFeedbackEffectTypes
	{
		/// <summary>
		/// The force increases in proportion to the distance of the axis from a defined neutral point. This is Condition force.
		/// </summary>
		Spring,

		/// <summary>
		/// The force increases in proportion to the velocity with which the user moves the axis. This is Condition force.
		/// </summary>
		Damper,

		/// <summary>
		/// The force increases in proportion to the acceleration of the axis. This is Condition force.
		/// </summary>
		Inertia,

		/// <summary>
		/// The force is applied when the axis is moved and depends on the defined friction coefficient. This is Condition force.
		/// </summary>
		Friction,

		/// <summary>
		/// A constant force is a force with a defined magnitude and duration.
		/// </summary> 
		ConstantForce,

		///// <summary>
		///// Custom force
		///// </summary> 
		//CustomForce,

		/// <summary>
		/// This is Periodic force.
		/// </summary> 
		Square,

		/// <summary>
		/// This is Periodic force.
		/// </summary>
		Sine,

		/// <summary>
		/// This is Periodic force.
		/// </summary>
		Triangle,

		/// <summary>
		/// The waveform drops vertically after it reaches maximum positive force. This is Periodic force.
		/// </summary>
		SawtoothUp,

		/// <summary>
		/// The waveform rises vertically after it reaches maximum negative force. This is Periodic force.
		/// </summary>
		SawtoothDown,

		/// <summary>
		/// A ramp force is a force with defined starting and ending magnitudes and a finite duration. A ramp force can continue in a single direction, or it can start as a strong push in one direction, weaken, stop, and then strengthen in the opposite direction.
		/// </summary> 
		Ramp
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class ForceFeedbackEffect
	{
		//internal const float infiniteTime = 0xFFFFFFFF;
		//internal const uint nominalMaxGain = 10000;
		//internal const uint emptyInvolvedAxesNumber = uint.MaxValue;

		ForceFeedbackController owner;
		ForceFeedbackEffectTypes effectType;
		JoystickAxes[] axes;
		ReadOnlyCollection<JoystickAxes> axesAsReadOnly;
		float[] direction;
		ReadOnlyCollection<float> directionAsReadOnly;

		float duration;
		double startTime;
		bool destroyed;
		bool needUpdateEffectTypeSpecificParameters;
		object userData;

		//ForceFeedbackEnvelope envelope;
		//uint gain = nominalMaxGain;
		//float samplePeriod;

		//

		internal ForceFeedbackEffect( ForceFeedbackController owner, ForceFeedbackEffectTypes effectType,
			JoystickAxes[] axes )
		{
			this.owner = owner;
			this.effectType = effectType;

			this.axes = new JoystickAxes[ axes.Length ];
			for( int n = 0; n < axes.Length; n++ )
				this.axes[ n ] = axes[ n ];
			this.axesAsReadOnly = new ReadOnlyCollection<JoystickAxes>( this.axes );

			//if( axes.Length > 1 )
			//{
			//   direction = new float[ this.axes.Length ];
			//   directionAsReadOnly = new ReadOnlyCollection<float>( direction );
			//}

			//envelope = new ForceFeedbackEnvelope( this );
		}

		public ForceFeedbackController Owner
		{
			get { return owner; }
		}

		//public ForceFeedbackEnvelope Envelope
		//{
		//   get { return envelope; }
		//}

		public IList<JoystickAxes> Axes
		{
			get { return axesAsReadOnly; }
		}

		/// <summary>
		/// Normalized vector conformable to the axes. For single-axis must be null.
		/// </summary>
		public IList<float> Direction
		{
			get { return directionAsReadOnly; }
		}

		/// <summary>
		/// Normalized vector conformable to the axes. For single-axis may be null.
		/// </summary>
		public void SetDirection( float[] value )
		{
			//if( axes.Length == 1 && value != null )
			//   Log.Fatal( "ForceFeedbackEffect: set SetDirection: axes.Length == 1 && value != null." );
			//if( axes.Length > 1 && value == null )
			//   Log.Fatal( "ForceFeedbackEffect: set SetDirection: axes.Length > 1 && value == null." );

			if( value != null )
			{
				if( value.Length != axes.Length )
					Log.Fatal( "ForceFeedbackEffect: set AxisDirections: Invalid list item count." );

				if( direction == null )
				{
					direction = new float[ axes.Length ];
					directionAsReadOnly = new ReadOnlyCollection<float>( direction );
				}

				bool updated = false;

				for( int n = 0; n < axes.Length; n++ )
				{
					if( direction[ n ] != value[ n ] )
					{
						direction[ n ] = value[ n ];
						updated = true;
					}
				}

				if( !updated )
					return;

				OnUpdateDirection();
			}
		}

		public bool Destroyed
		{
			get { return destroyed; }
		}

		public double StartTime
		{
			get { return startTime; }
		}

		public ForceFeedbackEffectTypes EffectType
		{
			get { return effectType; }
		}

		/// <summary>
		/// The total duration of the effect, in seconds.
		/// </summary>
		public float Duration
		{
			get { return duration; }
			set
			{
				if( duration == value )
					return;
				duration = value;
			}
		}

		public object UserData
		{
			get { return userData; }
			set { userData = value; }
		}

		protected virtual bool OnCreateRealEffect() { return false; }
		protected virtual void OnDestroyRealEffect() { }
		protected virtual void OnStart() { }
		protected virtual void OnUpdateDirection() { }
		protected virtual void OnUpdateEffectTypeSpecificParameters() { }

		///// <summary>
		///// The gain to be applied to the effect, in the range from 0 through 10,000. 
		///// The gain is a scaling factor applied to all magnitudes of the effect and its envelope.
		///// </summary>
		//public uint Gain
		//{
		//   get { return gain; }
		//   set
		//   {
		//      if( gain == value )
		//         return;
		//      gain = value;

		//x;
		//   }
		//}

		///// <summary>
		///// The period at which the device should play back the effect, in seconds. 
		///// A value of 0 indicates that the default playback sample rate should be used.
		///// If the device is not capable of playing back the effect at the specified rate, 
		///// it chooses the supported rate that is closest to the requested value.
		///// Setting a custom SamplePeriod can be used for special effects. 
		///// For example, playing a sine wave at an artificially large sample period results in a rougher texture.
		///// </summary>
		//public float SamplePeriod
		//{
		//   get { return samplePeriod; }
		//   set
		//   {
		//      if( samplePeriod == value )
		//         return;
		//      samplePeriod = value;

		//x;
		//   }
		//}

		public void Start()
		{
			if( destroyed )
				return;

			if( startTime != 0 )
				Log.Fatal( "ForceFeedbackEffect: Already started." );

			startTime = EngineApp.EngineTime;

			if( !Owner.Device.IsDeviceLost() )
			{
				OnStart();
			}
		}

		public void Destroy()
		{
			OnDestroyRealEffect();

			if( Owner != null )
				Owner.RemoveCreatedEffect( this );

			destroyed = true;
		}

		protected void SetNeedUpdateEffectTypeSpecificParameters()
		{
			needUpdateEffectTypeSpecificParameters = true;
		}

		internal void OnUpdateState()
		{
			if( Duration != 0 )
			{
				if( EngineApp.EngineTime - startTime > Duration )
				{
					Destroy();
					return;
				}
			}

			if( needUpdateEffectTypeSpecificParameters )
			{
				OnUpdateEffectTypeSpecificParameters();
				needUpdateEffectTypeSpecificParameters = false;
			}
		}

		internal bool CallOnCreateRealEffect()
		{
			return OnCreateRealEffect();
		}

		internal void CallOnDestroyRealEffect()
		{
			OnDestroyRealEffect();
		}

		public override string ToString()
		{
			string result = EffectType.ToString();
			if( Axes.Count > 0 )
			{
				result += ", Axes: ";
				for( int n = 0; n < Axes.Count; n++ )
				{
					JoystickAxes axis = Axes[ n ];
					if( n > 0 )
						result += ", ";
					result += axis.ToString();
				}
			}
			return result;
		}

		internal void OnDeviceLost()
		{
			OnDestroyRealEffect();
		}

		internal void OnDeviceRestore()
		{
			if( !OnCreateRealEffect() )
			{
				Destroy();
				return;
			}
			OnStart();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class ForceFeedbackConstantForceEffect : ForceFeedbackEffect
	{
		float magnitude;

		internal ForceFeedbackConstantForceEffect( ForceFeedbackController owner, JoystickAxes[] axes )
			: base( owner, ForceFeedbackEffectTypes.ConstantForce, axes )
		{
		}

		/// <summary>
		/// The magnitude of the effect, in the range from -1 through 1.
		/// </summary>
		/// <remarks>
		/// If an envelope is applied to this effect, the value represents the magnitude of the sustain. If no envelope is applied, the value represents the amplitude of the entire effect.
		/// </remarks>
		public float Magnitude
		{
			get { return magnitude; }
			set
			{
				if( magnitude == value )
					return;
				magnitude = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class ForceFeedbackConditionEffect : ForceFeedbackEffect
	{
		float offset;
		float positiveCoefficient;
		float negativeCoefficient;
		float positiveSaturation;
		float negativeSaturation;
		float deadBand;

		//

		internal ForceFeedbackConditionEffect( ForceFeedbackController owner,
			ForceFeedbackEffectTypes effectType, JoystickAxes[] axes )
			: base( owner, effectType, axes )
		{
		}

		/// <summary>
		/// Offset for the condition, in the range from -1 through 1.
		/// </summary>
		public float Offset
		{
			get { return offset; }
			set
			{
				if( offset == value )
					return;
				offset = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}

		/// <summary>
		/// Coefficient constant on the positive side of the offset, in the range from -1 through 1.
		/// </summary>
		public float PositiveCoefficient
		{
			get { return positiveCoefficient; }
			set
			{
				if( positiveCoefficient == value )
					return;
				positiveCoefficient = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}

		/// <summary>
		/// Coefficient constant on the negative side of the offset, in the range from -1 through 1. If the device does not support separate positive and negative coefficients, the value of NegativeCoefficient is ignored, and the value of PositiveCoefficient is used as both the positive and negative coefficients.
		/// </summary>
		public float NegativeCoefficient
		{
			get { return negativeCoefficient; }
			set
			{
				if( negativeCoefficient == value )
					return;
				negativeCoefficient = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}

		/// <summary>
		/// Maximum force output on the positive side of the offset, in the range from 0 through 1. If the device does not support force saturation, the value of this member is ignored.
		/// </summary>
		public float PositiveSaturation
		{
			get { return positiveSaturation; }
			set
			{
				if( positiveSaturation == value )
					return;
				positiveSaturation = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}

		/// <summary>
		/// Maximum force output on the negative side of the offset, in the range from 0 through 1. If the device does not support force saturation, the value of this member is ignored. 
		/// </summary>
		/// <remarks>
		/// If the device does not support separate positive and negative saturation, the value of NegativeSaturation is ignored, and the value of PositiveSaturation is used as both the positive and negative saturation.
		/// </remarks>
		public float NegativeSaturation
		{
			get { return negativeSaturation; }
			set
			{
				if( negativeSaturation == value )
					return;
				negativeSaturation = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}

		/// <summary>
		/// Region around Offset in which the condition is not active, in the range from 0 through 1. In other words, the condition is not active between Offset minus DeadBand and Offset plus DeadBand. 
		/// </summary>
		public float DeadBand
		{
			get { return deadBand; }
			set
			{
				if( deadBand == value )
					return;
				deadBand = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class ForceFeedbackPeriodicEffect : ForceFeedbackEffect
	{
		float magnitude;
		float offset;
		float phase;
		float period;

		internal ForceFeedbackPeriodicEffect( ForceFeedbackController owner,
			ForceFeedbackEffectTypes effectType, JoystickAxes[] axes )
			: base( owner, effectType, axes )
		{
		}

		/// <summary>
		/// Magnitude of the effect, in the range from 0 through 1. If an envelope is applied to this effect, the value represents the magnitude of the sustain. If no envelope is applied, the value represents the amplitude of the entire effect.
		/// </summary>
		public float Magnitude
		{
			get { return magnitude; }
			set
			{
				if( magnitude == value )
					return;
				magnitude = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}

		/// <summary>
		/// Offset of the effect. The range of forces generated by the effect is <see cref="Offset"/> minus <see cref="Magnitude"/> to Offset plus <see cref="Magnitude"/>. The value of the <see cref="Offset"/> member is also the baseline for any envelope that is applied to the effect.
		/// </summary>
		public float Offset
		{
			get { return offset; }
			set
			{
				if( offset == value )
					return;
				offset = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}

		/// <summary>
		/// Position in the cycle of the periodic effect at which playback begins, in the range from 0 through 1.
		/// </summary>
		/// <remarks>
		/// A device driver cannot provide support for all values in the <see cref="Phase"/> member. In this case, the value is rounded off to the nearest supported value.
		/// </remarks>
		public float Phase
		{
			get { return phase; }
			set
			{
				if( phase == value )
					return;
				phase = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}

		/// <summary>
		/// Period of the effect, in seconds. 
		/// </summary>
		public float Period
		{
			get { return period; }
			set
			{
				if( period == value )
					return;
				period = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class ForceFeedbackRampEffect : ForceFeedbackEffect
	{
		float startForce;
		float endForce;

		internal ForceFeedbackRampEffect( ForceFeedbackController owner, JoystickAxes[] axes )
			: base( owner, ForceFeedbackEffectTypes.Ramp, axes )
		{
		}

		/// <summary>
		/// Magnitude at the start of the effect, in the range from -1 through 1.
		/// </summary>
		public float StartForce
		{
			get { return startForce; }
			set
			{
				if( startForce == value )
					return;
				startForce = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}

		/// <summary>
		/// Magnitude at the end of the effect, in the range from -1 through 1.
		/// </summary>
		public float EndForce
		{
			get { return endForce; }
			set
			{
				if( endForce == value )
					return;
				endForce = value;
				SetNeedUpdateEffectTypeSpecificParameters();
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	//public abstract class ForceFeedbackCustomForceEffect : ForceFeedbackEffect
	//{
	//   xx internal;
	//   internal int channels;
	//   internal int samples;
	//   internal float[] sampleData;

	//   internal ForceFeedbackCustomForceEffect( ForceFeedbackController owner, JoystickAxes[] axes )
	//      : base( owner, ForceFeedbackEffectTypes.CustomForce, axes )
	//   {
	//   }

	//   /// <summary>
	//   /// Number of channels (axes) affected by this force. 
	//   /// </summary>
	//   public int Channels
	//   {
	//      get { return channels; }
	//      set
	//      {
	//         channels = value;

	//         x;
	//         //SetNeedUpdateParameters();
	//      }
	//   }

	//   /// <summary>
	//   /// Total number of samples in the RglForceData. It must be an integral multiple of the Channels.
	//   /// </summary>
	//   public int Samples
	//   {
	//      get { return samples; }
	//      set
	//      {
	//         samples = value;

	//         x;
	//         //SetNeedUpdateParameters();
	//      }
	//   }

	//   x плохо что только set
	//   /// <summary>
	//   /// Only to set new array.
	//   /// To get value use GetSampleDataValue.
	//   /// To change value use SetSampleDataValue.
	//   /// Value must be in -1..1.
	//   /// </summary>
	//   public float[] SampleData
	//   {
	//      set
	//      {
	//         sampleData = value;

	//         x;
	//         //SetNeedUpdateParameters();
	//      }
	//   }

	//   x;
	//   /// <summary>
	//   /// Set force value of sample.
	//   /// </summary>
	//   /// <param name="n">Index of sample.</param>
	//   /// <param name="value">-1..1</param>
	//   public void SetSampleDataValue( int n, float value )
	//   {
	//      sampleData[ n ] = value;

	//      x;
	//      //SetNeedUpdateParameters();
	//   }

	//   x;
	//   /// <summary>
	//   /// Get force value of sample.
	//   /// </summary>
	//   /// <param name="n">Index of sample.</param>
	//   /// <returns>Return value of force.</returns>
	//   public float GetSampleDataValue( int n )
	//   {
	//      return sampleData[ n ];
	//   }
	//}

}
