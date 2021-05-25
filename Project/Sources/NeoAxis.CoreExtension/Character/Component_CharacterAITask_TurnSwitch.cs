// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The task of the character to turn a valve.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character AI Turn Switch", -8991 )]
	[NewObjectDefaultName( "Turn Switch" )]
	public class Component_CharacterAITask_TurnSwitch : Component_AITask
	{
		const double totalTimeToGetWorkingState = 0.5;

		///////////////////////////////////////////////

		public enum StateEnum
		{
			None,
			Preparing,
			Working,
			Finishing,
		}
		[Browsable( false )]
		[Serialize]
		[DefaultValue( StateEnum.None )]
		public StateEnum currentState;

		[Browsable( false )]
		[Serialize]
		[DefaultValue( 0 )]
		public double currentTimeToGetWorkingState;

		/// <summary>
		/// The target object.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_RegulatorSwitchInSpace> Target
		{
			get { if( _target.BeginGet() ) Target = _target.Get( this ); return _target.value; }
			set { if( _target.BeginSet( ref value ) ) { try { TargetChanged?.Invoke( this ); } finally { _target.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Target"/> property value changes.</summary>
		public event Action<Component_CharacterAITask_TurnSwitch> TargetChanged;
		ReferenceField<Component_RegulatorSwitchInSpace> _target = null;

		/// <summary>
		/// Required value to be set by the character.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> RequiredValue
		{
			get { if( _requiredValue.BeginGet() ) RequiredValue = _requiredValue.Get( this ); return _requiredValue.value; }
			set { if( _requiredValue.BeginSet( ref value ) ) { try { RequiredValueChanged?.Invoke( this ); } finally { _requiredValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RequiredValue"/> property value changes.</summary>
		public event Action<Component_CharacterAITask_TurnSwitch> RequiredValueChanged;
		ReferenceField<double> _requiredValue = 0.0;

		/// <summary>
		/// Whether to use two hands
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> TwoHands
		{
			get { if( _twoHands.BeginGet() ) TwoHands = _twoHands.Get( this ); return _twoHands.value; }
			set { if( _twoHands.BeginSet( ref value ) ) { try { TwoHandsChanged?.Invoke( this ); } finally { _twoHands.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TwoHands"/> property value changes.</summary>
		public event Action<Component_CharacterAITask_TurnSwitch> TwoHandsChanged;
		ReferenceField<bool> _twoHands = false;

		///////////////////////////////////////////////

		void GetHandPositions( Component_RegulatorSwitchInSpace target, bool forward, out double handsFactor, out Transform leftHandTransform, out Transform rightHandTransform, out bool idling )
		{
			handsFactor = 0;
			leftHandTransform = Transform.Identity;
			rightHandTransform = Transform.Identity;
			idling = false;

			if( currentState != StateEnum.None )
			{
				handsFactor = MathEx.Saturate( currentTimeToGetWorkingState / totalTimeToGetWorkingState );

				var tr = target.TransformV;
				var center = tr.Position + tr.Rotation * new Vector3( target.ValveOffset, 0, 0 );

				double time;
				{
					var contoller = ParentRoot.HierarchyController;
					if( contoller != null )
						time = contoller.SimulationTime;
					else
						time = EngineApp.EngineTime;
				}

				var timePeriod = 2.0;
				var t = ( time % timePeriod ) / timePeriod;

				bool animationForward;
				double angleFactor;
				if( t < 0.5 )
				{
					animationForward = true;
					angleFactor = t * 2.0;
				}
				else
				{
					animationForward = false;
					angleFactor = 1.0 - ( t - 0.5 ) * 2.0;
				}

				var angleRange = new Range( new Degree( 60 ).InRadians(), new Degree( -60 ).InRadians() );
				var angle = MathEx.Lerp( angleRange.Minimum, angleRange.Maximum, angleFactor );

				var offset = tr.Rotation * new Vector3( 0, Math.Cos( angle ), Math.Sin( angle ) ) * target.ValveRadius;

				var leftHandPosition = center - offset;
				var rightHandPosition = center + offset;

				leftHandTransform = new Transform( leftHandPosition, Quaternion.LookAt( -target.TransformV.Rotation.GetForward(), ( leftHandPosition - center ).GetNormalize() ) );
				rightHandTransform = new Transform( rightHandPosition, Quaternion.LookAt( -target.TransformV.Rotation.GetForward(), ( rightHandPosition - center ).GetNormalize() ) );
				idling = forward != animationForward;
			}
		}

		protected override void OnPerformTaskSimulationStep()
		{
			base.OnPerformTaskSimulationStep();

			var ai = FindParent<Component_CharacterAI>();
			if( ai != null )
			{

				//!!!!check if far away to press. walk or stop

				var target = Target.Value;
				if( target == null || !target.EnabledInHierarchy )
				{
					//no target
					currentState = StateEnum.None;
					currentTimeToGetWorkingState = 0;
					ResetCharacterState( ai );
					if( DeleteTaskWhenReach )
						Dispose();
				}
				else
				{
					GetHandPositions( target, RequiredValue > target.Value, out var handsFactor, out var leftHandTransform, out var rightHandTransform, out var idling );

					switch( currentState )
					{
					case StateEnum.None:
						currentState = StateEnum.Preparing;
						currentTimeToGetWorkingState = 0;
						break;

					case StateEnum.Preparing:
						currentTimeToGetWorkingState += Time.SimulationDelta;
						if( currentTimeToGetWorkingState >= totalTimeToGetWorkingState )
						{
							currentTimeToGetWorkingState = totalTimeToGetWorkingState;
							currentState = StateEnum.Working;
						}
						break;

					case StateEnum.Working:
						if( !idling )
							target.SumulateRequiredValue( RequiredValue, Time.SimulationDelta );
						if( target.Value == RequiredValue )
							currentState = StateEnum.Finishing;
						break;

					case StateEnum.Finishing:
						currentTimeToGetWorkingState -= Time.SimulationDelta;
						if( currentTimeToGetWorkingState <= 0 )
						{
							currentTimeToGetWorkingState = 0;
							currentState = StateEnum.None;
						}
						break;
					}

					//update character state
					var character = ai.Character;
					if( character != null )
					{
						if( TwoHands )
						{
							character.LeftHandFactor = handsFactor;
							character.LeftHandTransform = leftHandTransform;
						}
						character.RightHandFactor = handsFactor;
						character.RightHandTransform = rightHandTransform;
					}

					if( currentState == StateEnum.None )
					{
						//task is done
						currentState = StateEnum.None;
						currentTimeToGetWorkingState = 0;
						ResetCharacterState( ai );
						if( DeleteTaskWhenReach )
							Dispose();
					}
				}
			}
		}

		void ResetCharacterState( Component_CharacterAI ai )
		{
			var character = ai.Character;
			if( character != null )
			{
				if( TwoHands )
				{
					character.LeftHandFactor = 0;
					character.LeftHandTransform = Transform.Identity;
				}
				character.RightHandFactor = 0;
				character.RightHandTransform = Transform.Identity;
			}
		}
	}
}
