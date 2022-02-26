// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// The task of the character to press button.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character AI Press Button", -8992 )]
	[NewObjectDefaultName( "Press Button" )]
	public class CharacterAITask_PressButton : AITask
	{
		const double totalTime = 2;
		const double clickTime = 1;
		const double clickPressingMoveTime = 0.4;
		const double clickPressingOffset = 0.3;
		const double buttonPressOffset = 0.05;
		const double handBoneOffset = 0.2;

		///////////////////////////////////////////////

		[Browsable( false )]
		[Serialize]
		[DefaultValue( 0 )]
		public double currentTime;

		/// <summary>
		/// The target object.
		/// </summary>
		[DefaultValue( null )]
		public Reference<ButtonInSpace> Target
		{
			get { if( _target.BeginGet() ) Target = _target.Get( this ); return _target.value; }
			set { if( _target.BeginSet( ref value ) ) { try { TargetChanged?.Invoke( this ); } finally { _target.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Target"/> property value changes.</summary>
		public event Action<CharacterAITask_PressButton> TargetChanged;
		ReferenceField<ButtonInSpace> _target = null;

		///////////////////////////////////////////////

		void GetPressingFactorByTime( double time, out double pressingFactor, out double skinningFactor )
		{
			var curve = new CurveLine();
			curve.AddPoint( 0, new Vector3( 0, 0, 0 ) );
			curve.AddPoint( clickTime - clickPressingMoveTime, new Vector3( 0, 1, 0 ) );
			curve.AddPoint( clickTime, new Vector3( 1, 1, 0 ) );
			curve.AddPoint( clickTime + clickPressingMoveTime, new Vector3( 0, 1, 0 ) );
			curve.AddPoint( totalTime, new Vector3( 0, 0, 0 ) );

			var value = curve.CalculateValueByTime( time );
			pressingFactor = value.X;
			skinningFactor = value.Y;
		}

		protected override void OnPerformTaskSimulationStep()
		{
			base.OnPerformTaskSimulationStep();

			var ai = FindParent<CharacterAI>();
			if( ai != null )
			{

				//!!!!check if far away to press. walk or stop

				var target = Target.Value;
				if( target == null || !target.EnabledInHierarchy )
				{
					//no target
					ResetCharacterState( ai );
					if( DeleteTaskWhenReach )
						Dispose();
				}
				else
				{
					var previousTime = currentTime;
					currentTime += Time.SimulationDelta;

					//update character state
					var character = ai.Character;
					if( character != null )
					{
						GetPressingFactorByTime( currentTime, out var pressingFactor, out var skinningFactor );

						var tr = target.TransformV;
						var clickPoint = tr.Position + tr.Rotation * new Vector3( buttonPressOffset + handBoneOffset, 0, 0 );
						var clickFrom = clickPoint + tr.Rotation * new Vector3( clickPressingOffset, 0, 0 );

						character.RightHandFactor = skinningFactor;
						character.RightHandTransform = new Transform( Vector3.Lerp( clickFrom, clickPoint, pressingFactor ), character.TransformV.Rotation );
					}

					if( currentTime >= totalTime )
					{
						//task is done
						ResetCharacterState( ai );
						if( DeleteTaskWhenReach )
							Dispose();
					}
					else
					{
						//click the button
						if( currentTime >= clickTime && previousTime < clickTime )
							target.ClickingBegin();
					}
				}
			}
		}

		void ResetCharacterState( CharacterAI ai )
		{
			var character = ai.Character;
			if( character != null )
			{
				character.RightHandFactor = 0;
				character.RightHandTransform = Transform.Identity;
			}
		}
	}
}
