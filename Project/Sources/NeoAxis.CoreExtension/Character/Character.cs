// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// A basic class for characters.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character", -8999 )]
#if !DEPLOY
	[Editor.SettingsCell( typeof( Editor.CharacterSettingsCell ), true )]
#endif
	public class Character : MeshInSpace, IProcessDamage, InteractiveObjectInterface, MeshInSpaceAnimationController.IParentAnimationTriggerProcess
	{
		static FastRandom staticRandom = new FastRandom( 0 );

		CharacterType typeCached = new CharacterType();
		int typeCachedVersion;

		Scene.PhysicsWorldClass.Body groundBody1;
		float groundBody1RemainingTime;
		Vector3F groundBody1Velocity;
		//networking
		bool clientIsOnGround;
		bool sentToClientsIsOnGround;
		Vector3F sentToClientsGroundBody1Velocity;

		//Scene.PhysicsWorldClass.Body groundBody2;
		//float groundBody2RemainingTime;

		//float forceIsOnGroundRemainingTime;
		//float disableGravityRemainingTime;
		bool isOnGroundHasValue;
		bool isOnGroundValue;
		float onGroundTime;
		float elapsedTimeSinceLastGroundContact;

		//moveVector
		int moveVectorTimer;//is disabled when equal 0
		Vector2F moveVector;
		bool moveVectorRun;
		//Vector2F lastTickForceVector;

		//jumping state
		float jumpInactiveTime;
		float jumpDisableRemainingTime;

		Vector3? lastTransformPosition;
		Vector3F lastLinearVelocity;
		bool lastActive;

		Vector3F[] groundRelativeVelocitySmoothArray;
		Vector3F groundRelativeVelocitySmooth;

		//crouching is not implemented
		//crouching
		//const float crouchingVisualSwitchTime = .3f;
		bool crouching;
		//float crouchingSwitchRemainingTime;
		//float crouchingVisualFactor;

		//wiggle camera when walking
		float wiggleWhenWalkingSpeedFactor;

		//smooth camera
		float smoothCameraOffsetZ;
		float sentToClientsSmoothCameraOffsetZ;

		//play one animation
		Animation playOneAnimation;
		double playOneAnimationSpeed = 1;
		bool playOneAnimationFreezeOnEnd;
		double playOneAnimationRemainingTime;

		//play one animation additional
		MeshInSpaceAnimationController.AnimationStateClass.AnimationItem playOneAnimationAdditional;
		double playOneAnimationAdditionalRemainingTime;

		//optimization
		MeshInSpaceAnimationController animationControllerCached;

		bool flyEndSoundOnGround = true;

		float disableControlRemainingTime;

		//client cached data
		//double sentToClientsSmoothCameraOffsetZ;

		//float allowToSleepTime;

		//Vector3 linearVelocityForSerialization;

		////damageFastChangeSpeed
		//Vector3 damageFastChangeSpeedLastVelocity = new Vector3( float.NaN, float.NaN, float.NaN );

		/////////////////////////////////////////

		const string characterTypeDefault = @"Content\Characters\NeoAxis\Bryce\Bryce.charactertype";
		//const string characterTypeDefault = @"Content\Characters\Default\Default.charactertype";

		[DefaultValueReference( characterTypeDefault )]
		public Reference<CharacterType> CharacterType
		{
			get { if( _characterType.BeginGet() ) CharacterType = _characterType.Get( this ); return _characterType.value; }
			set
			{
				if( _characterType.BeginSet( this, ref value ) )
				{
					try
					{
						CharacterTypeChanged?.Invoke( this );

						//update cached type
						typeCached = _characterType.value;
						if( typeCached == null )
							typeCached = new CharacterType();
						typeCachedVersion = typeCached.Version;

						//update mesh and body
						if( EnabledInHierarchyAndIsInstance )
						{
							UpdateMesh();
							RecreateBody();
						}
					}
					finally { _characterType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CharacterType"/> property value changes.</summary>
		public event Action<Character> CharacterTypeChanged;
		ReferenceField<CharacterType> _characterType = new Reference<CharacterType>( null, characterTypeDefault );


		//left hand

		/// <summary>
		/// Left hand control ratio.
		/// </summary>
		[Category( "Left Hand" )]
		[Range( 0, 1 )]
		[DefaultValue( 0.0 )]
		public Reference<double> LeftHandFactor
		{
			get { if( _leftHandFactor.BeginGet() ) LeftHandFactor = _leftHandFactor.Get( this ); return _leftHandFactor.value; }
			set { if( _leftHandFactor.BeginSet( this, ref value ) ) { try { LeftHandFactorChanged?.Invoke( this ); } finally { _leftHandFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandFactor"/> property value changes.</summary>
		public event Action<Character> LeftHandFactorChanged;
		ReferenceField<double> _leftHandFactor = 0.0;

		//transform is ok? maybe add bool LeftHandApplyRotation

		/// <summary>
		/// Left hand target transform in the world coordinates. X - forward, -Z - palm.
		/// </summary>
		[Category( "Left Hand" )]
		[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		public Reference<Transform> LeftHandTransform
		{
			get { if( _leftHandTransform.BeginGet() ) LeftHandTransform = _leftHandTransform.Get( this ); return _leftHandTransform.value; }
			set { if( _leftHandTransform.BeginSet( this, ref value ) ) { try { LeftHandTransformChanged?.Invoke( this ); } finally { _leftHandTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandTransform"/> property value changes.</summary>
		public event Action<Character> LeftHandTransformChanged;
		ReferenceField<Transform> _leftHandTransform = NeoAxis.Transform.Identity;

		[Category( "Left Hand" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> LeftHandThumbFingerFlexionFactor
		{
			get { if( _leftHandThumbFingerFlexionFactor.BeginGet() ) LeftHandThumbFingerFlexionFactor = _leftHandThumbFingerFlexionFactor.Get( this ); return _leftHandThumbFingerFlexionFactor.value; }
			set { if( _leftHandThumbFingerFlexionFactor.BeginSet( this, ref value ) ) { try { LeftHandThumbFingerFlexionFactorChanged?.Invoke( this ); } finally { _leftHandThumbFingerFlexionFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandThumbFingerFlexionFactor"/> property value changes.</summary>
		public event Action<Character> LeftHandThumbFingerFlexionFactorChanged;
		ReferenceField<double> _leftHandThumbFingerFlexionFactor = 0.0;

		[Category( "Left Hand" )]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> LeftHandThumbFingerFlexionValue
		{
			get { if( _leftHandThumbFingerFlexionValue.BeginGet() ) LeftHandThumbFingerFlexionValue = _leftHandThumbFingerFlexionValue.Get( this ); return _leftHandThumbFingerFlexionValue.value; }
			set { if( _leftHandThumbFingerFlexionValue.BeginSet( this, ref value ) ) { try { LeftHandThumbFingerFlexionValueChanged?.Invoke( this ); } finally { _leftHandThumbFingerFlexionValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandThumbFingerFlexionValue"/> property value changes.</summary>
		public event Action<Character> LeftHandThumbFingerFlexionValueChanged;
		ReferenceField<double> _leftHandThumbFingerFlexionValue = 0.5;

		[Category( "Left Hand" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> LeftHandIndexFingerFlexionFactor
		{
			get { if( _leftHandIndexFingerFlexionFactor.BeginGet() ) LeftHandIndexFingerFlexionFactor = _leftHandIndexFingerFlexionFactor.Get( this ); return _leftHandIndexFingerFlexionFactor.value; }
			set { if( _leftHandIndexFingerFlexionFactor.BeginSet( this, ref value ) ) { try { LeftHandIndexFingerFlexionFactorChanged?.Invoke( this ); } finally { _leftHandIndexFingerFlexionFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandIndexFingerFlexionFactor"/> property value changes.</summary>
		public event Action<Character> LeftHandIndexFingerFlexionFactorChanged;
		ReferenceField<double> _leftHandIndexFingerFlexionFactor = 0.0;

		[Category( "Left Hand" )]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> LeftHandIndexFingerFlexionValue
		{
			get { if( _leftHandIndexFingerFlexionValue.BeginGet() ) LeftHandIndexFingerFlexionValue = _leftHandIndexFingerFlexionValue.Get( this ); return _leftHandIndexFingerFlexionValue.value; }
			set { if( _leftHandIndexFingerFlexionValue.BeginSet( this, ref value ) ) { try { LeftHandIndexFingerFlexionValueChanged?.Invoke( this ); } finally { _leftHandIndexFingerFlexionValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandIndexFingerFlexionValue"/> property value changes.</summary>
		public event Action<Character> LeftHandIndexFingerFlexionValueChanged;
		ReferenceField<double> _leftHandIndexFingerFlexionValue = 0.5;

		[Category( "Left Hand" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> LeftHandMiddleFingerFlexionFactor
		{
			get { if( _leftHandMiddleFingerFlexionFactor.BeginGet() ) LeftHandMiddleFingerFlexionFactor = _leftHandMiddleFingerFlexionFactor.Get( this ); return _leftHandMiddleFingerFlexionFactor.value; }
			set { if( _leftHandMiddleFingerFlexionFactor.BeginSet( this, ref value ) ) { try { LeftHandMiddleFingerFlexionFactorChanged?.Invoke( this ); } finally { _leftHandMiddleFingerFlexionFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandMiddleFingerFlexionFactor"/> property value changes.</summary>
		public event Action<Character> LeftHandMiddleFingerFlexionFactorChanged;
		ReferenceField<double> _leftHandMiddleFingerFlexionFactor = 0.0;

		[Category( "Left Hand" )]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> LeftHandMiddleFingerFlexionValue
		{
			get { if( _leftHandMiddleFingerFlexionValue.BeginGet() ) LeftHandMiddleFingerFlexionValue = _leftHandMiddleFingerFlexionValue.Get( this ); return _leftHandMiddleFingerFlexionValue.value; }
			set { if( _leftHandMiddleFingerFlexionValue.BeginSet( this, ref value ) ) { try { LeftHandMiddleFingerFlexionValueChanged?.Invoke( this ); } finally { _leftHandMiddleFingerFlexionValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandMiddleFingerFlexionValue"/> property value changes.</summary>
		public event Action<Character> LeftHandMiddleFingerFlexionValueChanged;
		ReferenceField<double> _leftHandMiddleFingerFlexionValue = 0.5;

		[Category( "Left Hand" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> LeftHandRingFingerFlexionFactor
		{
			get { if( _leftHandRingFingerFlexionFactor.BeginGet() ) LeftHandRingFingerFlexionFactor = _leftHandRingFingerFlexionFactor.Get( this ); return _leftHandRingFingerFlexionFactor.value; }
			set { if( _leftHandRingFingerFlexionFactor.BeginSet( this, ref value ) ) { try { LeftHandRingFingerFlexionFactorChanged?.Invoke( this ); } finally { _leftHandRingFingerFlexionFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandRingFingerFlexionFactor"/> property value changes.</summary>
		public event Action<Character> LeftHandRingFingerFlexionFactorChanged;
		ReferenceField<double> _leftHandRingFingerFlexionFactor = 0.0;

		[Category( "Left Hand" )]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> LeftHandRingFingerFlexionValue
		{
			get { if( _leftHandRingFingerFlexionValue.BeginGet() ) LeftHandRingFingerFlexionValue = _leftHandRingFingerFlexionValue.Get( this ); return _leftHandRingFingerFlexionValue.value; }
			set { if( _leftHandRingFingerFlexionValue.BeginSet( this, ref value ) ) { try { LeftHandRingFingerFlexionValueChanged?.Invoke( this ); } finally { _leftHandRingFingerFlexionValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandRingFingerFlexionValue"/> property value changes.</summary>
		public event Action<Character> LeftHandRingFingerFlexionValueChanged;
		ReferenceField<double> _leftHandRingFingerFlexionValue = 0.5;

		[Category( "Left Hand" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> LeftHandLittleFingerFlexionFactor
		{
			get { if( _leftHandLittleFingerFlexionFactor.BeginGet() ) LeftHandLittleFingerFlexionFactor = _leftHandLittleFingerFlexionFactor.Get( this ); return _leftHandLittleFingerFlexionFactor.value; }
			set { if( _leftHandLittleFingerFlexionFactor.BeginSet( this, ref value ) ) { try { LeftHandLittleFingerFlexionFactorChanged?.Invoke( this ); } finally { _leftHandLittleFingerFlexionFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandLittleFingerFlexionFactor"/> property value changes.</summary>
		public event Action<Character> LeftHandLittleFingerFlexionFactorChanged;
		ReferenceField<double> _leftHandLittleFingerFlexionFactor = 0.0;

		[Category( "Left Hand" )]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> LeftHandLittleFingerFlexionValue
		{
			get { if( _leftHandLittleFingerFlexionValue.BeginGet() ) LeftHandLittleFingerFlexionValue = _leftHandLittleFingerFlexionValue.Get( this ); return _leftHandLittleFingerFlexionValue.value; }
			set { if( _leftHandLittleFingerFlexionValue.BeginSet( this, ref value ) ) { try { LeftHandLittleFingerFlexionValueChanged?.Invoke( this ); } finally { _leftHandLittleFingerFlexionValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandLittleFingerFlexionValue"/> property value changes.</summary>
		public event Action<Character> LeftHandLittleFingerFlexionValueChanged;
		ReferenceField<double> _leftHandLittleFingerFlexionValue = 0.5;

		//right hand

		/// <summary>
		/// Right hand control ratio.
		/// </summary>
		[Category( "Right Hand" )]
		[Range( 0, 1 )]
		[DefaultValue( 0.0 )]
		public Reference<double> RightHandFactor
		{
			get { if( _rightHandFactor.BeginGet() ) RightHandFactor = _rightHandFactor.Get( this ); return _rightHandFactor.value; }
			set { if( _rightHandFactor.BeginSet( this, ref value ) ) { try { RightHandFactorChanged?.Invoke( this ); } finally { _rightHandFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandFactor"/> property value changes.</summary>
		public event Action<Character> RightHandFactorChanged;
		ReferenceField<double> _rightHandFactor = 0.0;

		/// <summary>
		/// Right hand target transform in the world coordinates. X - forward, -Z - palm.
		/// </summary>
		[Category( "Right Hand" )]
		[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		public Reference<Transform> RightHandTransform
		{
			get { if( _rightHandTransform.BeginGet() ) RightHandTransform = _rightHandTransform.Get( this ); return _rightHandTransform.value; }
			set { if( _rightHandTransform.BeginSet( this, ref value ) ) { try { RightHandTransformChanged?.Invoke( this ); } finally { _rightHandTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandTransform"/> property value changes.</summary>
		public event Action<Character> RightHandTransformChanged;
		ReferenceField<Transform> _rightHandTransform = NeoAxis.Transform.Identity;

		[Category( "Right Hand" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> RightHandThumbFingerFlexionFactor
		{
			get { if( _rightHandThumbFingerFlexionFactor.BeginGet() ) RightHandThumbFingerFlexionFactor = _rightHandThumbFingerFlexionFactor.Get( this ); return _rightHandThumbFingerFlexionFactor.value; }
			set { if( _rightHandThumbFingerFlexionFactor.BeginSet( this, ref value ) ) { try { RightHandThumbFingerFlexionFactorChanged?.Invoke( this ); } finally { _rightHandThumbFingerFlexionFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandThumbFingerFlexionFactor"/> property value changes.</summary>
		public event Action<Character> RightHandThumbFingerFlexionFactorChanged;
		ReferenceField<double> _rightHandThumbFingerFlexionFactor = 0.0;

		[Category( "Right Hand" )]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> RightHandThumbFingerFlexionValue
		{
			get { if( _rightHandThumbFingerFlexionValue.BeginGet() ) RightHandThumbFingerFlexionValue = _rightHandThumbFingerFlexionValue.Get( this ); return _rightHandThumbFingerFlexionValue.value; }
			set { if( _rightHandThumbFingerFlexionValue.BeginSet( this, ref value ) ) { try { RightHandThumbFingerFlexionValueChanged?.Invoke( this ); } finally { _rightHandThumbFingerFlexionValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandThumbFingerFlexionValue"/> property value changes.</summary>
		public event Action<Character> RightHandThumbFingerFlexionValueChanged;
		ReferenceField<double> _rightHandThumbFingerFlexionValue = 0.5;

		[Category( "Right Hand" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> RightHandIndexFingerFlexionFactor
		{
			get { if( _rightHandIndexFingerFlexionFactor.BeginGet() ) RightHandIndexFingerFlexionFactor = _rightHandIndexFingerFlexionFactor.Get( this ); return _rightHandIndexFingerFlexionFactor.value; }
			set { if( _rightHandIndexFingerFlexionFactor.BeginSet( this, ref value ) ) { try { RightHandIndexFingerFlexionFactorChanged?.Invoke( this ); } finally { _rightHandIndexFingerFlexionFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandIndexFingerFlexionFactor"/> property value changes.</summary>
		public event Action<Character> RightHandIndexFingerFlexionFactorChanged;
		ReferenceField<double> _rightHandIndexFingerFlexionFactor = 0.0;

		[Category( "Right Hand" )]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> RightHandIndexFingerFlexionValue
		{
			get { if( _rightHandIndexFingerFlexionValue.BeginGet() ) RightHandIndexFingerFlexionValue = _rightHandIndexFingerFlexionValue.Get( this ); return _rightHandIndexFingerFlexionValue.value; }
			set { if( _rightHandIndexFingerFlexionValue.BeginSet( this, ref value ) ) { try { RightHandIndexFingerFlexionValueChanged?.Invoke( this ); } finally { _rightHandIndexFingerFlexionValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandIndexFingerFlexionValue"/> property value changes.</summary>
		public event Action<Character> RightHandIndexFingerFlexionValueChanged;
		ReferenceField<double> _rightHandIndexFingerFlexionValue = 0.5;

		[Category( "Right Hand" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> RightHandMiddleFingerFlexionFactor
		{
			get { if( _rightHandMiddleFingerFlexionFactor.BeginGet() ) RightHandMiddleFingerFlexionFactor = _rightHandMiddleFingerFlexionFactor.Get( this ); return _rightHandMiddleFingerFlexionFactor.value; }
			set { if( _rightHandMiddleFingerFlexionFactor.BeginSet( this, ref value ) ) { try { RightHandMiddleFingerFlexionFactorChanged?.Invoke( this ); } finally { _rightHandMiddleFingerFlexionFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandMiddleFingerFlexionFactor"/> property value changes.</summary>
		public event Action<Character> RightHandMiddleFingerFlexionFactorChanged;
		ReferenceField<double> _rightHandMiddleFingerFlexionFactor = 0.0;

		[Category( "Right Hand" )]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> RightHandMiddleFingerFlexionValue
		{
			get { if( _rightHandMiddleFingerFlexionValue.BeginGet() ) RightHandMiddleFingerFlexionValue = _rightHandMiddleFingerFlexionValue.Get( this ); return _rightHandMiddleFingerFlexionValue.value; }
			set { if( _rightHandMiddleFingerFlexionValue.BeginSet( this, ref value ) ) { try { RightHandMiddleFingerFlexionValueChanged?.Invoke( this ); } finally { _rightHandMiddleFingerFlexionValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandMiddleFingerFlexionValue"/> property value changes.</summary>
		public event Action<Character> RightHandMiddleFingerFlexionValueChanged;
		ReferenceField<double> _rightHandMiddleFingerFlexionValue = 0.5;

		[Category( "Right Hand" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> RightHandRingFingerFlexionFactor
		{
			get { if( _rightHandRingFingerFlexionFactor.BeginGet() ) RightHandRingFingerFlexionFactor = _rightHandRingFingerFlexionFactor.Get( this ); return _rightHandRingFingerFlexionFactor.value; }
			set { if( _rightHandRingFingerFlexionFactor.BeginSet( this, ref value ) ) { try { RightHandRingFingerFlexionFactorChanged?.Invoke( this ); } finally { _rightHandRingFingerFlexionFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandRingFingerFlexionFactor"/> property value changes.</summary>
		public event Action<Character> RightHandRingFingerFlexionFactorChanged;
		ReferenceField<double> _rightHandRingFingerFlexionFactor = 0.0;

		[Category( "Right Hand" )]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> RightHandRingFingerFlexionValue
		{
			get { if( _rightHandRingFingerFlexionValue.BeginGet() ) RightHandRingFingerFlexionValue = _rightHandRingFingerFlexionValue.Get( this ); return _rightHandRingFingerFlexionValue.value; }
			set { if( _rightHandRingFingerFlexionValue.BeginSet( this, ref value ) ) { try { RightHandRingFingerFlexionValueChanged?.Invoke( this ); } finally { _rightHandRingFingerFlexionValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandRingFingerFlexionValue"/> property value changes.</summary>
		public event Action<Character> RightHandRingFingerFlexionValueChanged;
		ReferenceField<double> _rightHandRingFingerFlexionValue = 0.5;

		[Category( "Right Hand" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> RightHandLittleFingerFlexionFactor
		{
			get { if( _rightHandLittleFingerFlexionFactor.BeginGet() ) RightHandLittleFingerFlexionFactor = _rightHandLittleFingerFlexionFactor.Get( this ); return _rightHandLittleFingerFlexionFactor.value; }
			set { if( _rightHandLittleFingerFlexionFactor.BeginSet( this, ref value ) ) { try { RightHandLittleFingerFlexionFactorChanged?.Invoke( this ); } finally { _rightHandLittleFingerFlexionFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandLittleFingerFlexionFactor"/> property value changes.</summary>
		public event Action<Character> RightHandLittleFingerFlexionFactorChanged;
		ReferenceField<double> _rightHandLittleFingerFlexionFactor = 0.0;

		[Category( "Right Hand" )]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> RightHandLittleFingerFlexionValue
		{
			get { if( _rightHandLittleFingerFlexionValue.BeginGet() ) RightHandLittleFingerFlexionValue = _rightHandLittleFingerFlexionValue.Get( this ); return _rightHandLittleFingerFlexionValue.value; }
			set { if( _rightHandLittleFingerFlexionValue.BeginSet( this, ref value ) ) { try { RightHandLittleFingerFlexionValueChanged?.Invoke( this ); } finally { _rightHandLittleFingerFlexionValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandLittleFingerFlexionValue"/> property value changes.</summary>
		public event Action<Character> RightHandLittleFingerFlexionValueChanged;
		ReferenceField<double> _rightHandLittleFingerFlexionValue = 0.5;

		//head

		/// <summary>
		/// Head control ratio.
		/// </summary>
		[Category( "Head" )]
		[Range( 0, 1 )]
		[DefaultValue( 0.0 )]
		public Reference<double> HeadFactor
		{
			get { if( _headFactor.BeginGet() ) HeadFactor = _headFactor.Get( this ); return _headFactor.value; }
			set { if( _headFactor.BeginSet( this, ref value ) ) { try { HeadFactorChanged?.Invoke( this ); } finally { _headFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadFactor"/> property value changes.</summary>
		public event Action<Character> HeadFactorChanged;
		ReferenceField<double> _headFactor = 0.0;

		/// <summary>
		/// Target position of the head.
		/// </summary>
		[Category( "Head" )]
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> HeadLookAt
		{
			get { if( _headLookAt.BeginGet() ) HeadLookAt = _headLookAt.Get( this ); return _headLookAt.value; }
			set { if( _headLookAt.BeginSet( this, ref value ) ) { try { HeadLookAtChanged?.Invoke( this ); } finally { _headLookAt.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadLookAt"/> property value changes.</summary>
		public event Action<Character> HeadLookAtChanged;
		ReferenceField<Vector3> _headLookAt = new Vector3( 0, 0, 0 );




		//!!!!foots

		//!!!!
		//EyesLookAtFactor
		//EyesLookAtValue

		//!!!!
		//[Category( "Skeleton State" )]
		//[DefaultValue( null )]
		//public Reference<ObjectInSpace> EyesLookAt
		//{
		//	get { if( _eyesLookAt.BeginGet() ) EyesLookAt = _eyesLookAt.Get( this ); return _eyesLookAt.value; }
		//	set { if( _eyesLookAt.BeginSet( this, ref value ) ) { try { EyesLookAtChanged?.Invoke( this ); } finally { _eyesLookAt.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="EyesLookAt"/> property value changes.</summary>
		//public event Action<Character> EyesLookAtChanged;
		//ReferenceField<ObjectInSpace> _eyesLookAt = null;

		/////////////////////////////////////////
		//Crawl

		////damageFastChangeSpeed

		//const float damageFastChangeSpeedMinimalSpeedDefault = 10;
		//[FieldSerialize]
		//float damageFastChangeSpeedMinimalSpeed = damageFastChangeSpeedMinimalSpeedDefault;

		//const float damageFastChangeSpeedFactorDefault = 40;
		//[FieldSerialize]
		//float damageFastChangeSpeedFactor = damageFastChangeSpeedFactorDefault;

		/////////////////////////////////////////

		public enum LifeStatusEnum
		{
			Normal,
			Dead,
		}

		/// <summary>
		/// Dead or alive.
		/// </summary>
		[Category( "Game Framework" )]
		[DefaultValue( LifeStatusEnum.Normal )]
		public Reference<LifeStatusEnum> LifeStatus
		{
			get { if( _lifeStatus.BeginGet() ) LifeStatus = _lifeStatus.Get( this ); return _lifeStatus.value; }
			set
			{
				if( _lifeStatus.BeginSet( this, ref value ) )
				{
					try
					{
						LifeStatusChanged?.Invoke( this );
						CurrentLifeStatusTime = 0;

						if( EngineApp.IsSimulation && _lifeStatus.value.Value == LifeStatusEnum.Dead )
						{
							RequiredTurnToDirection = null;
							RequiredLookToPosition = null;
							CurrentLookToPosition = null;

							foreach( var item in GetAllItems() )
								ItemDeactivate( null, item );
						}
					}
					finally { _lifeStatus.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LifeStatus"/> property value changes.</summary>
		public event Action<Character> LifeStatusChanged;
		ReferenceField<LifeStatusEnum> _lifeStatus = LifeStatusEnum.Normal;

		/// <summary>
		/// The health of the character.
		/// </summary>
		[Category( "Game Framework" )]
		[DefaultValue( 0.0 )]
		public Reference<float> Health
		{
			get { if( _health.BeginGet() ) Health = _health.Get( this ); return _health.value; }
			set { if( _health.BeginSet( this, ref value ) ) { try { HealthChanged?.Invoke( this ); } finally { _health.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Health"/> property value changes.</summary>
		public event Action<Character> HealthChanged;
		ReferenceField<float> _health = 0.0f;

		[Category( "Game Framework" )]
		[DefaultValue( 10.0 )]
		public Reference<double> DeletionTimeAfterDeath
		{
			get { if( _deletionTimeAfterDeath.BeginGet() ) DeletionTimeAfterDeath = _deletionTimeAfterDeath.Get( this ); return _deletionTimeAfterDeath.value; }
			set { if( _deletionTimeAfterDeath.BeginSet( this, ref value ) ) { try { DeletionTimeAfterDeathChanged?.Invoke( this ); } finally { _deletionTimeAfterDeath.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DeletionTimeAfterDeath"/> property value changes.</summary>
		public event Action<Character> DeletionTimeAfterDeathChanged;
		ReferenceField<double> _deletionTimeAfterDeath = 10.0;

		[Category( "Game Framework" )]
		[DefaultValue( 0.0 )]
		[Browsable( false )]
		[NetworkSynchronize( false )] //don't syncronize via network, but increment on clients too
		public Reference<double> CurrentLifeStatusTime
		{
			get { if( _currentLifeStatusTime.BeginGet() ) CurrentLifeStatusTime = _currentLifeStatusTime.Get( this ); return _currentLifeStatusTime.value; }
			set { if( _currentLifeStatusTime.BeginSet( this, ref value ) ) { try { CurrentLifeStatusTimeChanged?.Invoke( this ); } finally { _currentLifeStatusTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurrentLifeStatusTime"/> property value changes.</summary>
		public event Action<Character> CurrentLifeStatusTimeChanged;
		ReferenceField<double> _currentLifeStatusTime = 0.0;

		/// <summary>
		/// The team index of the object.
		/// </summary>
		[Category( "Game Framework" )]
		[DefaultValue( 0 )]
		public Reference<int> Team
		{
			get { if( _team.BeginGet() ) Team = _team.Get( this ); return _team.value; }
			set { if( _team.BeginSet( this, ref value ) ) { try { TeamChanged?.Invoke( this ); } finally { _team.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Team"/> property value changes.</summary>
		public event Action<Character> TeamChanged;
		ReferenceField<int> _team = 0;

		/// <summary>
		/// Whether to display the RequiredLookToPosition, RequiredTurnToDirection, hands position and sitting info.
		/// </summary>
		[Category( "Debug" )]
		[DefaultValue( false )]
		public Reference<bool> DebugVisualization
		{
			get { if( _debugVisualization.BeginGet() ) DebugVisualization = _debugVisualization.Get( this ); return _debugVisualization.value; }
			set { if( _debugVisualization.BeginSet( this, ref value ) ) { try { DebugVisualizationChanged?.Invoke( this ); } finally { _debugVisualization.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugVisualization"/> property value changes.</summary>
		public event Action<Character> DebugVisualizationChanged;
		ReferenceField<bool> _debugVisualization = false;

		/////////////////////////////////////////
		//sitting

		[Browsable( false )]
		public bool Sitting
		{
			get { return sitting; }
			set
			{
				if( sitting == value )
					return;
				sitting = value;

				ResetAnimation();
				lastTransformPosition = null;
				lastLinearVelocity = Vector3F.Zero;
				PhysicalBodyLinearVelocity = Vector3.Zero;
				PhysicalBodyAngularVelocity = Vector3.Zero;
				TickAnimate( 0.001f );

				if( NetworkIsServer )
				{
					var writer = BeginNetworkMessageToEveryone( "Sitting" );
					writer.Write( Sitting );
					EndNetworkMessage();
				}
			}
		}
		bool sitting;

		[Browsable( false )]
		public Degree SittingSpineAngle { get; set; }

		[Browsable( false )]
		public Degree SittingLegsAngle { get; set; }

		/////////////////////////////////////////

		public Character()
		{
			Collision = true;
		}

		[Browsable( false )]
		public CharacterType TypeCached
		{
			get { return typeCached; }
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//these properties are under control by the class
				case nameof( Mesh ):
				case nameof( Collision ):
					skip = true;
					break;
				}
			}
		}

		void ResetAnimation()
		{
			var controller = GetAnimationController();
			if( controller != null )
			{
				controller.PlayAnimation = null;
				controller.Speed = 1;
				controller.SetAnimationState( null, false );
			}
			StartPlayOneAnimation( null );
			StartPlayOneAnimationAdditional( null );
		}

		void UpdateMesh()
		{
			//!!!!new
			Mesh = TypeCached.Mesh.Value;
			//Mesh = TypeCached.Mesh;
			ResetAnimation();
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			animationControllerCached = null;
			CharacterType.Touch();

			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				//optimize networking
				{
					var controller = GetAnimationController();
					if( controller != null )
					{
						controller.NetworkDisablePropertySynchronization( "PlayAnimation" );
						controller.NetworkDisablePropertySynchronization( "Speed" );
						controller.NetworkDisablePropertySynchronization( "AutoRewind" );
						controller.NetworkDisablePropertySynchronization( "FreezeOnEnd" );
					}
				}

				UpdateMesh();

				//update currentTurnToDirection
				CalculateCurrentTurnToDirectionByTransform();

				//if( mainBody != null )
				//	mainBody.LinearVelocity = linearVelocityForSerialization;

				if( ParentScene != null )
					ParentScene.PhysicsSimulationStepAfter += ParentScene_PhysicsSimulationStepAfter;

				TickAnimate( 0.001f );
				SpaceBoundsUpdate();

				if( NetworkIsClient )
					clientIsOnGround = true;
			}
			else
			{
				if( ParentScene != null )
					ParentScene.PhysicsSimulationStepAfter -= ParentScene_PhysicsSimulationStepAfter;
			}
		}

		//!!!!need? maybe slowly to unsubscribe from event. event for body?
		private void ParentScene_PhysicsSimulationStepAfter( Scene obj )
		{
			if( PhysicalBody != null && PhysicalBody.Active )
				UpdateRotation();
		}

		protected override RigidBody OnGetCollisionShapeData( Mesh.CompiledData meshResult )
		{
			//it must support physics shape caching

			var body = new RigidBody();
			body.CharacterMode = true;
			body.MotionType = PhysicsMotionType.Kinematic;
			body.Mass = TypeCached.Mass;

			TypeCached.GetBodyFormInfo( crouching, out var height, out _, out _ );
			var length = height - TypeCached.Radius * 2;
			if( length < 0.01 )
				length = 0.01;

			//!!!!apply scale. where else

			//!!!!new disabled
			//body.CenterOfMassOffset = new Vector3( 0, 0, height * 0.5 );

			var capsuleShape = body.CreateComponent<CollisionShape_Capsule>();
			capsuleShape.Height = length;
			capsuleShape.Radius = TypeCached.Radius;
			capsuleShape.LocalTransform = new Transform( new Vector3( 0, 0, height * 0.5 ), Quaternion.Identity );

			return body;
		}

		protected override void OnCollisionBodyCreated()
		{
			base.OnCollisionBodyCreated();

			if( PhysicalBody != null )
			{
				TypeCached.GetBodyFormInfo( crouching, out _, out var walkUpHeight, out var walkDownHeight );

				PhysicalBody.CharacterModeMaxStrength = (float)TypeCached.MaxStrength;
				PhysicalBody.CharacterModeWalkUpHeight = (float)walkUpHeight;
				PhysicalBody.CharacterModeWalkDownHeight = (float)walkDownHeight;
				PhysicalBody.CharacterModeMaxSlopeAngle = (float)TypeCached.MaxSlopeAngle.Value.InRadians();

				//!!!!smaller area works bad. maybe need set another ExtendedUpdateSettings
				PhysicalBody.CharacterModeSetSupportingVolume = (float)TypeCached.Radius * 0.8f;
				//PhysicalBody.CharacterModeSetSupportingVolume = (float)TypeCached.Radius * 0.5f;

				//PhysicalBody.CharacterModePredictiveContactDistance = 0.1f;
				//what else
			}
		}

		public double GetScaleFactor()
		{
			var result = TransformV.Scale.MaxComponent();
			if( result < 0.0001 )
				result = 0.0001;
			return result;
		}

		///////////////////////////////////////////

		public void SetMoveVector( Vector2F direction, bool run )
		{
			if( NetworkIsServer )
			{
				//one second
				moveVectorTimer = (int)( 1.0 / Time.SimulationDelta );
			}
			else
				moveVectorTimer = 2;

			moveVector = direction;
			moveVectorRun = run;
		}

		[Browsable( false )]
		public Vector2F MoveVector
		{
			get { return moveVector; }
		}

		[Browsable( false )]
		public bool MoveVectorRun
		{
			get { return moveVectorRun; }
		}

		//network: no sense to update via network, it is calculated from Transform
		SphericalDirectionF currentTurnToDirection;//use SphericalDirectionF instead float because vertical direction is used for first person camera

		//network: no sense to send to clients
		[Browsable( false )]
		public SphericalDirectionF? RequiredTurnToDirection { get; set; }

		[Browsable( false )]
		public Degree? TurningSpeedOverride { get; set; }

		//network: no sense to send to clients
		[Browsable( false )]
		public Vector3? RequiredLookToPosition { get; set; }

		//network: sychronized via network
		[Browsable( false )]
		public Vector3? CurrentLookToPosition
		{
			get { return currentLookToPosition; }
			set
			{
				if( currentLookToPosition == value )
					return;
				currentLookToPosition = value;

				if( NetworkIsServer )
				{
					//!!!!pack

					var writer = BeginNetworkMessageToEveryone( "CurrentLookToPosition" );
					writer.Write( currentLookToPosition.HasValue );
					if( currentLookToPosition.HasValue )
						writer.Write( currentLookToPosition.Value );
					EndNetworkMessage();
				}
			}
		}
		Vector3? currentLookToPosition;

		//network: no sense to send to clients
		[Browsable( false )]
		public bool AllowLookToBackWhenNoActiveItem { get; set; }

		[Browsable( false )]
		public SphericalDirectionF CurrentTurnToDirection
		{
			get { return currentTurnToDirection; }
		}

		public void TurnToDirection( SphericalDirectionF? value, bool turnInstantly )
		{
			RequiredTurnToDirection = value;

			if( turnInstantly && RequiredTurnToDirection.HasValue )
			{
				currentTurnToDirection = RequiredTurnToDirection.Value;
				UpdateRotation();
				RequiredTurnToDirection = null;
			}
		}

		public void TurnToDirection( Vector2F direction, bool turnInstantly )
		{
			TurnToDirection( new SphericalDirectionF( (float)MathEx.Atan2( direction.Y, direction.X ), 0 ), turnInstantly );
		}

		void CalculateCurrentTurnToDirectionByTransform()
		{
			var forward = TransformV.Rotation.GetForward();
			currentTurnToDirection = new SphericalDirectionF( (float)MathEx.Atan2( forward.Y, forward.X ), 0 );
		}

		public void LookToPosition( Vector3? value, bool turnInstantly )
		{
			RequiredLookToPosition = value;
			if( turnInstantly )
			{
				CurrentLookToPosition = RequiredLookToPosition;
				RequiredLookToPosition = null;
			}
		}

		public void SetTransformAndTurnToDirectionInstantly( Transform value )
		{
			Transform = value;
			TurnToDirection( TransformV.Rotation.GetForward().ToVector2F(), true );
		}

		protected override void OnTransformUpdating( ref Reference<Transform> value )
		{
			base.OnTransformUpdating( ref value );

			//when it is client and turning instantly, then don't update rotation from server. restore current value from CurrentTurnToDirection
			if( NetworkIsClient && IsControlledByPlayerAndFirstPersonCameraEnabled() && !Sitting )
			{
				var halfAngle = currentTurnToDirection.Horizontal * 0.5;
				var rot = new Quaternion( new Vector3( 0, 0, Math.Sin( halfAngle ) ), Math.Cos( halfAngle ) );

				var tr = value.Value;
				if( tr != null )
				{
					tr = tr.UpdateRotation( rot );
					value = new Reference<Transform>( tr, value.GetByReference );
				}
			}
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			//update currentTurnToDirection
			if( EngineApp.IsEditor )
				CalculateCurrentTurnToDirectionByTransform();

			//update currentTurnToDirection
			if( NetworkIsClient && ( !IsControlledByPlayerAndFirstPersonCameraEnabled() || Sitting ) )
				CalculateCurrentTurnToDirectionByTransform();

			isOnGroundHasValue = false;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void UpdateRotation()
		{
			if( Collision )
			{
				var halfAngle = currentTurnToDirection.Horizontal * 0.5;
				var rot = new Quaternion( new Vector3( 0, 0, Math.Sin( halfAngle ) ), Math.Cos( halfAngle ) );

				const float epsilon = 0.0001f;

				var tr = TransformV;

				//update Rotation
				if( !tr.Rotation.Equals( rot, epsilon ) )
					Transform = tr.UpdateRotation( rot );
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool CalculateIsOnGround()
		{
			if( jumpInactiveTime != 0 )
				return false;
			//if( forceIsOnGroundRemainingTime > 0 )
			//	return true;
			if( Sitting )
				return true;

			return groundBody1 != null || EngineApp.IsEditor || clientIsOnGround;// || groundBody2 != null;

			//const float maxThreshold = 0.1f;
			//return mainBodyGroundDistanceNoScale < maxThreshold && groundBody1 != null;

			////double distanceFromPositionToFloor = 0.0;//crouching ? typeCached.CrouchingPositionToGroundHeight : typeCached.PositionToGroundHeight;
			////const double maxThreshold = 0.2;
			////return mainBodyGroundDistanceNoScale - maxThreshold < distanceFromPositionToFloor && groundBody != null;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IsOnGround()
		{
			if( !isOnGroundHasValue )
			{
				isOnGroundHasValue = true;
				isOnGroundValue = CalculateIsOnGround();
			}
			return isOnGroundValue;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IsOnGroundWithLatency()
		{
			//return IsOnGround();
			return elapsedTimeSinceLastGroundContact < 0.25f || IsOnGround();
		}

		public float GetElapsedTimeSinceLastGroundContact()
		{
			return elapsedTimeSinceLastGroundContact;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void GroundBodiesSimulationStepAndClearGroundBodiesWhenDisposed()
		{
			if( PhysicalBody != null && PhysicalBody.Active )
			{
				if( groundBody1 != null )
				{
					groundBody1RemainingTime -= Time.SimulationDelta;
					if( groundBody1RemainingTime <= 0 )
					{
						groundBody1 = null;
						groundBody1Velocity = Vector3F.Zero;
					}
				}
				//if( groundBody2 != null )
				//{
				//	groundBody2RemainingTime -= Time.SimulationDelta;
				//	if( groundBody2RemainingTime <= 0 )
				//		groundBody2 = null;
				//}
			}

			if( groundBody1 != null && groundBody1.Disposed )
			{
				groundBody1 = null;
				groundBody1Velocity = Vector3F.Zero;
			}
			//if( groundBody2 != null && groundBody2.Disposed )
			//	groundBody2 = null;

			//if( groundBody1 == null && groundBody2 != null )
			//{
			//	groundBody1 = groundBody2;
			//	groundBody1RemainingTime = groundBody2RemainingTime;
			//	groundBody2 = null;
			//}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//!!!!slowly? что-то может реже вызывать

			isOnGroundHasValue = false;

			GroundBodiesSimulationStepAndClearGroundBodiesWhenDisposed();

			if( PhysicalBody != null && ParentScene != null )
			{
				CalculateMainBodyGroundDistanceAndGroundBody();
				TickPhysicsForce();
			}

			TickTurnToDirection();
			TickLookToPosition();

			if( TypeCached.Jump )
				TickJump( false );

			if( IsOnGround() )
				onGroundTime += Time.SimulationDelta;
			else
				onGroundTime = 0;
			if( !IsOnGround() )
				elapsedTimeSinceLastGroundContact += Time.SimulationDelta;
			else
				elapsedTimeSinceLastGroundContact = 0;
			CalculateGroundRelativeVelocity();

			TickWiggleWhenWalkingSpeedFactor();
			TickSmoothCameraOffset();

			if( moveVectorTimer != 0 )
				moveVectorTimer--;

			//if( CrouchingSupport )
			//	TickCrouching();

			//if( DamageFastChangeSpeedFactor != 0 )
			//	DamageFastChangeSpeedTick();

			if( PhysicalBody != null && !PhysicalBody.Active && lastActive )
				SpaceBoundsUpdate();

			var trPosition = TransformV.Position;
			if( lastTransformPosition.HasValue )
				lastLinearVelocity = ( ( trPosition - lastTransformPosition.Value ) / Time.SimulationDelta ).ToVector3F();
			else
				lastLinearVelocity = Vector3F.Zero;
			lastTransformPosition = trPosition;
			lastActive = PhysicalBody != null && PhysicalBody.Active;

			//if( forceIsOnGroundRemainingTime > 0 )
			//{
			//	forceIsOnGroundRemainingTime -= Time.SimulationDelta;
			//	if( forceIsOnGroundRemainingTime < 0 )
			//	{
			//		forceIsOnGroundRemainingTime = 0;
			//		isOnGroundHasValue = false;
			//	}
			//}

			//if( disableGravityRemainingTime > 0 )
			//{
			//	disableGravityRemainingTime -= Time.SimulationDelta;
			//	if( disableGravityRemainingTime < 0 )
			//		disableGravityRemainingTime = 0;
			//}

			TickFlyEndSound();

			if( disableControlRemainingTime > 0 )
			{
				disableControlRemainingTime -= Time.SimulationDelta;
				if( disableControlRemainingTime < 0 )
					disableControlRemainingTime = 0;
			}

			//update life time
			CurrentLifeStatusTime += Time.SimulationDelta;
			if( LifeStatus.Value == LifeStatusEnum.Dead && CurrentLifeStatusTime.Value >= DeletionTimeAfterDeath.Value )
				RemoveFromParent( true );

			//send on ground info to clients, smoothCameraOffsetZ
			if( NetworkIsServer )
			{
				if( sentToClientsIsOnGround != IsOnGround() || sentToClientsGroundBody1Velocity != groundBody1Velocity )
				{
					sentToClientsIsOnGround = IsOnGround();
					sentToClientsGroundBody1Velocity = groundBody1Velocity;

					var writer = BeginNetworkMessageToEveryone( "OnGroundState" );
					if( writer != null )
					{
						writer.Write( sentToClientsIsOnGround );
						writer.Write( sentToClientsGroundBody1Velocity );
						EndNetworkMessage();
					}
				}

				if( sentToClientsSmoothCameraOffsetZ != smoothCameraOffsetZ )
				{
					sentToClientsSmoothCameraOffsetZ = smoothCameraOffsetZ;

					var writer = BeginNetworkMessageToEveryone( "SmoothCameraOffsetZ" );
					if( writer != null )
					{
						writer.Write( sentToClientsSmoothCameraOffsetZ );
						EndNetworkMessage();
					}
				}
			}
		}

		protected override void OnSimulationStepClient()
		{
			base.OnSimulationStepClient();

			isOnGroundHasValue = false;

			//GroundBodiesSimulationStepAndClearGroundBodiesWhenDisposed();

			//if( PhysicalBody != null )
			//	CalculateMainBodyGroundDistanceAndGroundBody();

			if( TypeCached.Jump )
				TickJumpClient( false );

			if( IsOnGround() )
				onGroundTime += Time.SimulationDelta;
			else
				onGroundTime = 0;
			if( !IsOnGround() )
				elapsedTimeSinceLastGroundContact += Time.SimulationDelta;
			else
				elapsedTimeSinceLastGroundContact = 0;
			CalculateGroundRelativeVelocity();

			TickWiggleWhenWalkingSpeedFactor();
			//TickSmoothCameraOffset();

			////if( CrouchingSupport )
			////	TickCrouching();

			////if( DamageFastChangeSpeedFactor != 0 )
			////	DamageFastChangeSpeedTick();

			var trPosition = TransformV.Position;
			if( lastTransformPosition.HasValue )
				lastLinearVelocity = ( ( trPosition - lastTransformPosition.Value ) / Time.SimulationDelta ).ToVector3F();
			else
				lastLinearVelocity = Vector3F.Zero;
			lastTransformPosition = trPosition;

			//if( forceIsOnGroundRemainingTime > 0 )
			//{
			//	forceIsOnGroundRemainingTime -= Time.SimulationDelta;
			//	if( forceIsOnGroundRemainingTime < 0 )
			//	{
			//		forceIsOnGroundRemainingTime = 0;
			//		isOnGroundHasValue = false;
			//	}
			//}

			TickFlyEndSound();

			CurrentLifeStatusTime += Time.SimulationDelta;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.IsEditor )
				isOnGroundHasValue = false;

			//FixTurnToDirectionWhenItInvalidForAnimation();
			TickAnimate( delta );
			UpdateActiveItemTransformOffset();

			//check for CharacterType changes
			//!!!!also update Mesh when it is null. to fix on client in networking mode
			if( TypeCached.Version != typeCachedVersion || Mesh.Value == null )
			{
				typeCachedVersion = TypeCached.Version;

				//update mesh and body
				if( EnabledInHierarchyAndIsInstance )
				{
					UpdateMesh();
					RecreateBody();
				}
			}
		}

		public bool IsNeedRun()
		{
			//use specified force move vector
			if( moveVectorTimer != 0 )
				return moveVectorRun;

			return false;
		}

		Vector2F GetMoveVector()
		{
			//use specified move vector
			if( moveVectorTimer != 0 )
				return moveVector;

			return Vector2F.Zero;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void TickPhysicsForce()
		{
			var desiredVelocity = Vector2F.Zero;

			var forceVec = GetMoveVector();
			if( forceVec != Vector2.Zero && disableControlRemainingTime == 0 )
			{
				float speedMultiplier = 1;
				//if( FastMoveInfluence != null )
				//	speedCoefficient = FastMoveInfluence.Type.Coefficient;

				double maxSpeed = 0;
				//double force = 0;

				if( IsOnGround() )
				{
					//calcualate maxSpeed and force on ground

					var localVelocity = ( new Vector3F( forceVec.X, forceVec.Y, 0 ) * TransformV.Rotation.GetInverse().ToQuaternionF() ).ToVector2();

					var absSum = Math.Abs( localVelocity.X ) + Math.Abs( localVelocity.Y );
					if( absSum > 1 )
						localVelocity /= absSum;

					maxSpeed = 0;
					//force = 0;

					if( !Crouching )
					{
						bool running = IsNeedRun();

						if( Math.Abs( localVelocity.X ) >= .001f )
						{
							//forward and backward
							double speedX;
							if( localVelocity.X > 0 )
								speedX = running ? TypeCached.RunForwardMaxSpeed : TypeCached.WalkForwardMaxSpeed;
							else
								speedX = running ? TypeCached.RunBackwardMaxSpeed : TypeCached.WalkBackwardMaxSpeed;
							maxSpeed += speedX * Math.Abs( localVelocity.X );
							//force += ( running ? TypeCached.RunForce : TypeCached.WalkForce ) * Math.Abs( localVec.X );
						}

						if( Math.Abs( localVelocity.Y ) >= .001f )
						{
							//left and right
							maxSpeed += ( running ? TypeCached.RunSideMaxSpeed : TypeCached.WalkSideMaxSpeed ) * Math.Abs( localVelocity.Y );
							//force += ( running ? TypeCached.RunForce : TypeCached.WalkForce ) * Math.Abs( localVec.Y );
						}
					}
					else
					{
						maxSpeed = TypeCached.CrouchingMaxSpeed;
						//force = TypeCached.CrouchingForce;
					}
				}
				else
				{
					//calcualate maxSpeed and force when flying.
					if( TypeCached.FlyControl )
					{
						maxSpeed = TypeCached.FlyControlMaxSpeed;
						//force = TypeCached.FlyControlForce;
					}
				}

				if( maxSpeed > 0 )//if( force > 0 )
				{
					//speedCoefficient
					maxSpeed *= speedMultiplier;
					//force *= speedMultiplier;

					var scaleFactor = GetScaleFactor();
					maxSpeed *= scaleFactor;
					//force *= scaleFactor;

					//if( velocity.ToVector2().LengthSquared() < maxSpeed * maxSpeed )
					////if( GetLinearVelocity().ToVector2().LengthSquared() < maxSpeed * maxSpeed )
					//{

					desiredVelocity = new Vector2F( forceVec.X, forceVec.Y ).GetNormalize() * (float)maxSpeed;

					//}
				}
			}

			PhysicalBody.CharacterModeDesiredVelocity = desiredVelocity;
		}

		////!!!!
		//List<Ray> debugRays;// = new List<Ray>();
		//List<Vector3> debugPoints = new List<Vector3>();

		[MethodImpl( (MethodImplOptions)512 )]
		void CalculateMainBodyGroundDistanceAndGroundBody()
		{
			PhysicalBody.GetCharacterModeData( out var groundState, out var groundBodyID, out var groundBodySubShapeID, out var groundPosition, out var groundNormal, out var groundVelocity, out var walkUpDownLastChange );

			if( groundState == Scene.PhysicsWorldClass.Body.CharacterDataGroundState.OnGround )
			{
				groundBody1 = ParentScene.PhysicsWorld.GetBodyById( groundBodyID );
				groundBody1RemainingTime = 0.1f;//0.25f;
				groundBody1Velocity = groundVelocity;
			}

			if( walkUpDownLastChange != 0 )
				smoothCameraOffsetZ -= walkUpDownLastChange;

			isOnGroundHasValue = false;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void GetVolumeCapsule( out Capsule capsule )
		{
			//!!!!scaling тут не учтен

			var tr = TransformV;
			var height = TypeCached.Height.Value;
			var radius = TypeCached.Radius.Value;
			capsule = new Capsule( tr.Position + new Vector3( 0, 0, radius ), tr.Position + new Vector3( 0, 0, height - radius ), radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void CapsuleAddOffset( ref Capsule capsule, ref Vector3 offset )
		{
			capsule.Point1 += offset;
			capsule.Point2 += offset;
		}

		protected virtual void OnJump()
		{
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void TickJump( bool ignoreTicks )
		{
			if( !ignoreTicks )
			{
				if( jumpDisableRemainingTime != 0 )
				{
					jumpDisableRemainingTime -= Time.SimulationDelta;
					if( jumpDisableRemainingTime < 0 )
						jumpDisableRemainingTime = 0;
				}

				if( jumpInactiveTime != 0 )
				{
					jumpInactiveTime -= Time.SimulationDelta;
					if( jumpInactiveTime < 0 )
					{
						jumpInactiveTime = 0;
						isOnGroundHasValue = false;
					}
				}
			}

			if( IsOnGround() && onGroundTime > Time.SimulationDelta && jumpInactiveTime == 0 && jumpDisableRemainingTime != 0 && PhysicalBody != null && disableControlRemainingTime == 0 )
			{
				var velocity = PhysicalBody.LinearVelocity;
				velocity.Z = (float)( TypeCached.JumpSpeed * GetScaleFactor() );
				PhysicalBody.LinearVelocity = velocity;

				var tr = TransformV;
				Transform = tr.UpdatePosition( tr.Position + new Vector3( 0, 0, 0.05 ) );
				//SetTransform( tr.UpdatePosition( tr.Position + new Vector3( 0, 0, 0.05 ) ), false );

				jumpInactiveTime = 0.2f;
				jumpDisableRemainingTime = 0;
				isOnGroundHasValue = false;

				//UpdateMainBodyDamping();

				//PhysicalBody.Active = true;

				SoundPlay( TypeCached.JumpSound );
				StartPlayOneAnimation( TypeCached.JumpAnimation );

				if( NetworkIsServer && ( TypeCached.JumpSound.ReferenceOrValueSpecified || TypeCached.JumpAnimation.ReferenceOrValueSpecified ) )
				{
					BeginNetworkMessageToEveryone( "Jump" );
					EndNetworkMessage();
				}

				OnJump();
			}
		}

		void TickJumpClient( bool ignoreTicks )
		{
			if( !ignoreTicks )
			{
				if( jumpDisableRemainingTime != 0 )
				{
					jumpDisableRemainingTime -= Time.SimulationDelta;
					if( jumpDisableRemainingTime < 0 )
						jumpDisableRemainingTime = 0;
				}

				if( jumpInactiveTime != 0 )
				{
					jumpInactiveTime -= Time.SimulationDelta;
					if( jumpInactiveTime < 0 )
					{
						jumpInactiveTime = 0;
						isOnGroundHasValue = false;
					}
				}
			}
		}

		public void Jump()
		{
			if( !TypeCached.Jump )
				return;
			if( Crouching )
				return;

			jumpDisableRemainingTime = 0.4f;
			TickJump( true );
		}

		public void JumpClient()
		{
			if( !TypeCached.Jump )
				return;
			if( Crouching )
				return;

			BeginNetworkMessageToServer( "Jump" );
			EndNetworkMessage();
		}

		//[Browsable( false )]
		//public Vector2 LastTickForceVector
		//{
		//	get { return lastTickForceVector; }
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			var meshResult = Mesh.Value?.Result;
			if( meshResult != null )
			{
				//make bounds from bounds sphere (to expand bounds for animation)
				var meshBoundsLocal = meshResult.SpaceBounds;
				meshBoundsLocal.BoundingSphere.ToBounds( out var b );
				var meshBoundsTransformed = TransformV * b;
				newBounds = new SpaceBounds( ref meshBoundsTransformed );

				//!!!!predict when inside vehicle

				//!!!!check
				//bounds prediction to skip small updates in future steps
				if( PhysicalBody != null && PhysicalBody.Active )
				{
					var realBounds = newBounds.BoundingBox;

					if( !SpaceBoundsOctreeOverride.HasValue || !SpaceBoundsOctreeOverride.Value.Contains( ref realBounds ) )
					{
						//calculated extended bounds. predict for 2-3 seconds

						var trPosition = TransformV.Position;

						var bTotal = realBounds;
						var b2 = new Bounds( trPosition );
						b2.Add( trPosition + PhysicalBody.LinearVelocity * ( 2.0f + staticRandom.NextFloat() ) );
						b2.Expand( newBounds.BoundingSphere.Radius * 1.1 );
						bTotal.Add( ref b2 );

						SpaceBoundsOctreeOverride = bTotal;
					}
				}
				else
					SpaceBoundsOctreeOverride = null;
			}
			else
				base.OnSpaceBoundsUpdate( ref newBounds );


			//if( PhysicalBody != null )
			//{
			//	GetBox( out var box );
			//	box.ToBounds( out var bounds );
			//	newBounds = SpaceBounds.Merge( newBounds, new SpaceBounds( ref bounds ) );
			//}
		}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			context.thisObjectWasChecked = true;
			GetBox( out var box );
			if( box.Intersects( context.ray, out var scale1, out var scale2 ) )
				context.thisObjectResultRayScale = Math.Min( scale1, scale2 );

			//if( SpaceBounds.BoundingBox.Intersects( context.ray, out var scale ) )
			//	context.thisObjectResultRayScale = scale;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void CalculateGroundRelativeVelocity()
		{
			if( PhysicalBody != null && PhysicalBody.Active )
			{
				//groundRelativeVelocitySmooth
				if( groundRelativeVelocitySmoothArray == null )
				{
					var seconds = 0.2f;
					var count = ( seconds / Time.SimulationDelta ) + 0.999f;
					groundRelativeVelocitySmoothArray = new Vector3F[ (int)count ];
				}
				for( int n = 0; n < groundRelativeVelocitySmoothArray.Length - 1; n++ )
					groundRelativeVelocitySmoothArray[ n ] = groundRelativeVelocitySmoothArray[ n + 1 ];
				groundRelativeVelocitySmoothArray[ groundRelativeVelocitySmoothArray.Length - 1 ] = GetGroundRelativeVelocity();//groundRelativeVelocity;
				groundRelativeVelocitySmooth = Vector3F.Zero;
				for( int n = 0; n < groundRelativeVelocitySmoothArray.Length; n++ )
					groundRelativeVelocitySmooth += groundRelativeVelocitySmoothArray[ n ];
				groundRelativeVelocitySmooth /= groundRelativeVelocitySmoothArray.Length;
			}
			else
			{
				if( groundRelativeVelocitySmoothArray != null )
					groundRelativeVelocitySmoothArray = null;
				if( groundRelativeVelocitySmooth != Vector3F.Zero )
					groundRelativeVelocitySmooth = Vector3F.Zero;
			}
		}

		public Vector3F GetGroundRelativeVelocity()
		{
			return lastLinearVelocity - groundBody1Velocity;
		}

		[Browsable( false )]
		public Vector3 GroundRelativeVelocitySmooth
		{
			get { return groundRelativeVelocitySmooth; }
		}

		public Vector3F GetLinearVelocity()
		{
			return lastLinearVelocity;
		}

		//public void DamageFastChangeSpeedResetLastVelocity()
		//{
		//	damageFastChangeSpeedLastVelocity = new Vector3( float.NaN, float.NaN, float.NaN );
		//}

		//void DamageFastChangeSpeedTick()
		//{
		//	if( MainBody == null )
		//		return;
		//	Vector3 velocity = MainBody.LinearVelocity;

		//	if( float.IsNaN( damageFastChangeSpeedLastVelocity.X ) )
		//		damageFastChangeSpeedLastVelocity = velocity;

		//	Vector3 diff = velocity - damageFastChangeSpeedLastVelocity;
		//	if( diff.Z > 0 )
		//	{
		//		float v = diff.Z - Type.DamageFastChangeSpeedMinimalSpeed;
		//		if( v > 0 )
		//		{
		//			float damage = v * Type.DamageFastChangeSpeedFactor;
		//			if( damage > 0 )
		//				DoDamage( null, Position, null, damage, true );
		//		}
		//	}

		//	damageFastChangeSpeedLastVelocity = velocity;
		//}

		[Browsable( false )]
		public bool Crouching
		{
			get { return crouching; }
		}

		//void TickCrouching()
		//{
		//	if( crouchingSwitchRemainingTime > 0 )
		//	{
		//		crouchingSwitchRemainingTime -= TickDelta;
		//		if( crouchingSwitchRemainingTime < 0 )
		//			crouchingSwitchRemainingTime = 0;
		//	}

		//	if( Intellect != null && crouchingSwitchRemainingTime == 0 )
		//	{
		//		bool needCrouching = Intellect.IsControlKeyPressed( GameControlKeys.Crouching );

		//		if( crouching != needCrouching )
		//		{
		//			Vector3 newPosition;
		//			{
		//				float diff = Type.HeightFromPositionToGround - Type.CrouchingHeightFromPositionToGround;
		//				if( needCrouching )
		//					newPosition = Position + new Vector3( 0, 0, -diff );
		//				else
		//					newPosition = Position + new Vector3( 0, 0, diff );
		//			}

		//			bool freePlace = true;
		//			{
		//				Capsule capsule;
		//				{
		//					float radius = Type.Radius - .01f;

		//					float length;
		//					if( needCrouching )
		//						length = Type.CrouchingHeight - radius * 2 - Type.CrouchingWalkUpHeight;
		//					else
		//						length = Type.Height - radius * 2 - Type.WalkUpHeight;

		//					capsule = new Capsule(
		//						newPosition + new Vector3( 0, 0, -length / 2 ),
		//						newPosition + new Vector3( 0, 0, length / 2 ), radius );
		//				}

		//				Body[] bodies = PhysicsWorld.Instance.VolumeCast( capsule, (int)ContactGroup.CastOnlyContact );
		//				foreach( Body body in bodies )
		//				{
		//					if( body == mainBody )
		//						continue;

		//					freePlace = false;
		//					break;
		//				}
		//			}

		//			if( freePlace )
		//			{
		//				crouching = needCrouching;
		//				crouchingSwitchRemainingTime = .3f;

		//				ReCreateMainBody();

		//				Position = newPosition;
		//				OldPosition = Position;

		//				Vector3 addForceOnBigSlope;
		//				CalculateMainBodyGroundDistanceAndGroundBody( out addForceOnBigSlope );
		//			}
		//		}
		//	}
		//}

		public void GetBox( out Box box )
		{
			GetVolumeCapsule( out var capsule );
			var extents = new Vector3( capsule.Radius, capsule.Radius, ( capsule.Point2 - capsule.Point1 ).Length() * 0.5 + capsule.Radius );
			box = new Box( capsule.GetCenter(), extents, TransformV.Rotation.ToMatrix3() );
		}

		void DebugDraw( Viewport viewport )
		{
			var renderer = viewport.Simple3DRenderer;
			GetBox( out var box );
			var points = box.ToPoints();

			renderer.AddArrow( points[ 0 ], points[ 1 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 1 ], points[ 2 ], -1 );
			renderer.AddArrow( points[ 3 ], points[ 2 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 3 ], points[ 0 ], -1 );

			renderer.AddArrow( points[ 4 ], points[ 5 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 5 ], points[ 6 ], -1 );
			renderer.AddArrow( points[ 7 ], points[ 6 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 7 ], points[ 4 ], -1 );

			renderer.AddLine( points[ 0 ], points[ 4 ], -1 );
			renderer.AddLine( points[ 1 ], points[ 5 ], -1 );
			renderer.AddLine( points[ 2 ], points[ 6 ], -1 );
			renderer.AddLine( points[ 3 ], points[ 7 ], -1 );
		}

		void UpdateTransformVisualOverride()
		{
			var rotateMeshDependingGroundEnabled = TypeCached.RotateMeshDependingGround && IsOnGround();// && groundBody1 != null;

			if( rotateMeshDependingGroundEnabled || smoothCameraOffsetZ != 0 )
			{
				var tr = TransformV;

				var rotateMeshDependingGround = Quaternion.Identity;
				var rotateMeshDependingGroundSpecified = false;

				//rotate mesh depending ground
				if( rotateMeshDependingGroundEnabled )
				{
					var scene = ParentScene;
					if( scene != null )
					{
						//!!!!GC

						var r = TypeCached.Radius;

						var positionPlusHalfRadius = tr.Position;
						positionPlusHalfRadius.Z += r * 0.5;
						var vectorForward = tr.Rotation * new Vector3( r * 0.5, 0, 0 );

						var rayForward = new Ray( positionPlusHalfRadius + vectorForward, new Vector3( 0, 0, -r ) );
						var itemForward = new PhysicsRayTestItem( rayForward, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );

						var rayBackward = new Ray( positionPlusHalfRadius - vectorForward, new Vector3( 0, 0, -r ) );
						var itemBackward = new PhysicsRayTestItem( rayBackward, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );

						scene.PhysicsRayTest( new PhysicsRayTestItem[] { itemForward, itemBackward }, true );

						double? forwardZ = null;
						double? backwardZ = null;
						foreach( var item in itemForward.Result )
						{
							if( item.Body != PhysicalBody )
							{
								forwardZ = item.Position.Z;
								break;
							}
						}
						foreach( var item in itemBackward.Result )
						{
							if( item.Body != PhysicalBody )
							{
								backwardZ = item.Position.Z;
								break;
							}
						}

						if( forwardZ.HasValue && backwardZ.HasValue )
						{
							var frontAngle = (float)Math.Atan2( forwardZ.Value - backwardZ.Value, r );
							rotateMeshDependingGround = QuaternionF.FromRotateByY( frontAngle );
							rotateMeshDependingGroundSpecified = true;
						}
					}
				}

				if( rotateMeshDependingGroundSpecified || smoothCameraOffsetZ != 0 )
				{
					var pos = tr.Position;
					pos.Z += smoothCameraOffsetZ;

					var rot = tr.Rotation;
					if( rotateMeshDependingGroundSpecified )
						rot *= rotateMeshDependingGround;

					TransformVisualOverride = new Transform( pos, rot, tr.Scale );
				}
				else
					TransformVisualOverride = null;
			}
			else
				TransformVisualOverride = null;
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			EditorAllowRenderSelection = false;

			UpdateTransformVisualOverride();

			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;
				var scene = context.Owner.AttachedScene;

				if( scene != null && context.SceneDisplayDevelopmentDataInThisApplication && scene.DisplayPhysicalObjects )
				{
					TypeCached.GetBodyFormInfo( crouching, out var height, out var walkUpHeight, out var walkDownHeight );

					var renderer = context.Owner.Simple3DRenderer;
					var tr = TransformV;
					var scaleFactor = tr.Scale.MaxComponent();

					renderer.SetColor( new ColorValue( 1, 0, 0, 1 ) );

					//object position
					renderer.AddSphere( new Sphere( tr.Position, 0.05 ), 16 );

					//walk up height
					{
						Vector3 pos = tr.Position + new Vector3( 0, 0, walkUpHeight * scaleFactor );
						renderer.AddLine( pos + new Vector3( 0.05, 0, 0 ), pos - new Vector3( 0.05, 0, 0 ) );
						renderer.AddLine( pos + new Vector3( 0, 0.05, 0 ), pos - new Vector3( 0, 0.05, 0 ) );
					}

					//walk down height
					{
						Vector3 pos = tr.Position - new Vector3( 0, 0, walkDownHeight * scaleFactor );
						renderer.AddLine( pos + new Vector3( 0.05, 0, 0 ), pos - new Vector3( 0.05, 0, 0 ) );
						renderer.AddLine( pos + new Vector3( 0, 0.05, 0 ), pos - new Vector3( 0, 0.05, 0 ) );
					}

					//eye position
					renderer.SetColor( new ColorValue( 0, 1, 0, 1 ) );
					var eyePosition = TransformV * TypeCached.EyePosition.Value;
					renderer.AddArrow( eyePosition, eyePosition + tr.Rotation * new Vector3( TypeCached.Height.Value * 0.05, 0, 0 ) );
					//renderer.AddSphere( new Sphere( TransformV * TypeCached.EyePosition.Value, .05f ), 16 );
				}

				var showLabels = /*show &&*/ PhysicalBody == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;

#if !DEPLOY
				//draw selection
				if( EngineApp.IsEditor && PhysicalBody != null )
				{
					if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
					{
						ColorValue color;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.SelectedColor;
						else //if( context2.canSelectObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.CanSelectColor;
						//else
						//	color = ProjectSettings.Get.SceneShowPhysicsDynamicActiveColor;

						var viewport = context.Owner;

						viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
						DebugDraw( viewport );
					}
				}
#endif

				//if( debugRays != null && debugRays.Count != 0 )
				//{
				//	foreach( var ray in debugRays )
				//	{
				//		var renderer = context.Owner.Simple3DRenderer;
				//		renderer.SetColor( new ColorValue( 1, 0, 0 ) );
				//		renderer.AddArrow( ray.Origin, ray.GetEndPoint(), 0, 0, true, 0 );
				//	}
				//}

				//if( debugPoints != null && debugPoints.Count != 0 )
				//{
				//	foreach( var point in debugPoints )
				//	{
				//		var renderer = context.Owner.Simple3DRenderer;
				//		renderer.SetColor( new ColorValue( 1, 0, 0 ) );
				//		renderer.AddSphere( point, 0.01, 32 );
				//	}
				//}

				if( DebugVisualization )
				{
					var renderer = context.Owner.Simple3DRenderer;

					if( RequiredLookToPosition.HasValue )
					{
						renderer.SetColor( new ColorValue( 1, 0, 0 ) );
						renderer.AddSphere( new Sphere( RequiredLookToPosition.Value, .05 ) );
					}

					if( RequiredTurnToDirection.HasValue )
					{
						renderer.SetColor( new ColorValue( 1, 0, 0 ) );
						renderer.AddArrow( TransformV.Position, TransformV.Position + TransformV.Rotation.GetForward() );
					}

					//hand positions
					var weapon = GetActiveWeapon();
					if( weapon != null )
					{
						var controller = GetAnimationController();
						if( controller != null )
						{
							var globalBoneTransforms = controller.GetBoneGlobalTransforms();
							if( globalBoneTransforms != null )
							{
								if( weapon.TypeCached.WayToUse.Value == WeaponType.WayToUseEnum.Rifle )
								{
									var boneLeftHandIndex = controller.GetBoneIndex( TypeCached.LeftHandBone );
									var boneLeftHandMiddle1Index = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Left", "Middle", 1 ) );

									if( boneLeftHandIndex != -1 && boneLeftHandMiddle1Index != -1 )
									{
										var boneLeftHand = globalBoneTransforms[ boneLeftHandIndex ].Position;
										var boneLeftHandMiddle1 = globalBoneTransforms[ boneLeftHandMiddle1Index ].Position;

										var localCenter = Vector3F.Lerp( boneLeftHand, boneLeftHandMiddle1, 0.75f );
										var center = TransformV * localCenter;

										renderer.SetColor( new ColorValue( 1, 0, 0 ) );
										renderer.AddSphere( new Sphere( center, .02 ) );
									}
								}

								{
									var boneRightHandIndex = controller.GetBoneIndex( TypeCached.RightHandBone );
									var boneRightHandMiddle1Index = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Right", "Middle", 1 ) );

									if( boneRightHandIndex != -1 && boneRightHandMiddle1Index != -1 )
									{
										var boneRightHand = globalBoneTransforms[ boneRightHandIndex ].Position;
										var boneRightHandMiddle1 = globalBoneTransforms[ boneRightHandMiddle1Index ].Position;

										var localCenter = Vector3F.Lerp( boneRightHand, boneRightHandMiddle1, 0.75f );
										var center = TransformV * localCenter;

										renderer.SetColor( new ColorValue( 1, 0, 0 ) );
										renderer.AddSphere( new Sphere( center, .02 ) );
									}
								}

							}
						}
					}

					if( Sitting )
					{
						renderer.SetColor( new ColorValue( 1, 0, 0 ) );
						renderer.AddSphere( new Sphere( TransformV.Position + new Vector3( 0, 0, TypeCached.SitButtHeight ), .02 ) );
					}
				}


				//!!!!
				//if( _tempDebug.Count != 0 )
				//{
				//	foreach( var p in _tempDebug )
				//	{
				//		var viewport = context.Owner;
				//		viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );

				//		var pp = TransformV * p;

				//		viewport.Simple3DRenderer.AddSphere( new Sphere( pp, 0.01 ) );
				//	}
				//}
			}

			//protected override void OnRenderFrame()
			//
			//if( ( crouching && crouchingVisualFactor < 1 ) || ( !crouching && crouchingVisualFactor > 0 ) )
			//{
			//	float delta = RendererWorld.Instance.FrameRenderTimeStep / crouchingVisualSwitchTime;
			//	if( crouching )
			//	{
			//		crouchingVisualFactor += delta;
			//		if( crouchingVisualFactor > 1 )
			//			crouchingVisualFactor = 1;
			//	}
			//	else
			//	{
			//		crouchingVisualFactor -= delta;
			//		if( crouchingVisualFactor < 0 )
			//			crouchingVisualFactor = 0;
			//	}
			//}

			//{
			//	var renderer = context.viewport.Simple3DRenderer;
			//	renderer.SetColor( new ColorValue( 0, 1, 0, 1 ) );
			//	foreach( var c in GetVolumeCapsules() )
			//	{
			//		renderer.AddCapsule( c );
			//	}
			//}

		}

		void TickWiggleWhenWalkingSpeedFactor()
		{
			//!!!!slowly?

			float destinationFactor;
			if( IsOnGround() )
			{
				destinationFactor = (float)GroundRelativeVelocitySmooth.Length() * 0.3f;
				if( destinationFactor < 0.1f ) //0.5f
					destinationFactor = 0;
				if( destinationFactor > 1 )
					destinationFactor = 1;
			}
			else
				destinationFactor = 0;

			float step = Time.SimulationDelta/* RendererWorld.Instance.FrameRenderTimeStep*/ * 5;
			if( wiggleWhenWalkingSpeedFactor < destinationFactor )
			{
				wiggleWhenWalkingSpeedFactor += step;
				if( wiggleWhenWalkingSpeedFactor > destinationFactor )
					wiggleWhenWalkingSpeedFactor = destinationFactor;
			}
			else
			{
				wiggleWhenWalkingSpeedFactor -= step;
				if( wiggleWhenWalkingSpeedFactor < destinationFactor )
					wiggleWhenWalkingSpeedFactor = destinationFactor;
			}
		}

		void TickSmoothCameraOffset()
		{
			if( smoothCameraOffsetZ != 0 )
			{
				var speed = (float)TypeCached.Height.Value * 1.2f;// 0.75;

				if( smoothCameraOffsetZ > 0 )
				{
					smoothCameraOffsetZ -= Time.SimulationDelta * speed;
					if( smoothCameraOffsetZ < 0 )
						smoothCameraOffsetZ = 0;
				}
				else
				{
					smoothCameraOffsetZ += Time.SimulationDelta * speed;
					if( smoothCameraOffsetZ > 0 )
						smoothCameraOffsetZ = 0;
				}
			}
		}

		Vector3F GetSmoothCameraOffset()
		{
			//if need X, Y then need change code outside. checks for smoothCameraOffsetZ

			return new Vector3F( 0, 0, smoothCameraOffsetZ );
		}

		public bool GetEyesPosition( Transform tr, out Vector3 position )
		{
			var controller = GetAnimationController();
			if( controller != null && controller.Bones != null )
			{
				//already calculated where this method called
				//UpdateTransformVisualOverride();
				//var tr = TransformVisualOverride ?? TransformV;

				////check for bones for eyes. Default: mixamorig:LeftEye, mixamorig:RightEye
				//{
				//	var leftEyeIndex = controller.GetBoneIndex( TypeCached.LeftEyeBone );
				//	var rightEyeIndex = controller.GetBoneIndex( TypeCached.RightEyeBone );
				//	if( leftEyeIndex != -1 && rightEyeIndex != -1 )
				//	{
				//		//get bones position in object space
				//		controller.GetBoneGlobalTransform( leftEyeIndex, out var globalItem1 );
				//		controller.GetBoneGlobalTransform( rightEyeIndex, out var globalItem2 );

				//		//calculate position in object space
				//		var resultPos = ( globalItem1.Position + globalItem2.Position ) / 2;

				//		//calculate world position
				//		position = tr.ToMatrix4() * resultPos;

				//		return true;
				//	}
				//}

				//get eye position by head bones. Default: mixamorig:HeadTop_End, mixamorig:Head
				{
					var headTopEndIndex = controller.GetBoneIndex( TypeCached.HeadTopBone );
					var headIndex = controller.GetBoneIndex( TypeCached.HeadBone );
					if( headTopEndIndex != -1 && headIndex != -1 )
					{
						//get bones position in object space
						controller.GetBoneGlobalTransform( headTopEndIndex, out var globalItem1 );
						controller.GetBoneGlobalTransform( headIndex, out var globalItem2 );

						//calculate base position in object space
						var basePos = ( globalItem1.Position + globalItem2.Position ) / 2;

						//add offset
						var length = ( globalItem1.Position - globalItem2.Position ).Length();
						var offset = globalItem1.Rotation * new Vector3F( 0, 0, 1 ) * length / 2;

						//calculate result position in object space
						var resultPos = basePos + offset;

						//calculate world position
						position = tr.ToMatrix4() * resultPos;

						return true;
					}
				}


				//var bones = new List<SkeletonBone>();
				//foreach( var bone in controller.Bones )
				//{
				//	if( bone.Name.Contains( "eye", StringComparison.OrdinalIgnoreCase ) )
				//		bones.Add( bone );

				//	//if( bone.Name.Contains( "head", StringComparison.OrdinalIgnoreCase ) )
				//	//	bones.Add( bone );
				//}

				//if( bones.Count != 0 )
				//{
				//	position = Vector3.Zero;

				//	//already calculated where this method called
				//	//UpdateTransformVisualOverride();
				//	//var tr = TransformVisualOverride ?? TransformV;

				//	foreach( var bone in bones )
				//	{
				//		var boneIndex = controller.GetBoneIndex( bone );

				//		controller.GetBoneGlobalTransform( boneIndex, out var globalItem );
				//		globalItem.ToMatrix( out var globalMatrix );
				//		//controller.GetBoneGlobalTransform( boneIndex, out var globalMatrix );

				//		//!!!!что с RotateRootBone

				//		var m = tr.ToMatrix4() * globalMatrix;

				//		position += m.GetTranslation();
				//	}

				//	position /= bones.Count;

				//	return true;
				//}

			}

			position = Vector3.Zero;
			return false;
		}

		public virtual void GetFirstPersonCameraPosition( bool useEyesPositionOfModel, out Vector3 position, out Vector3 forward, out Vector3 up )
		{
			position = Vector3.Zero;

			UpdateTransformVisualOverride();
			var tr = TransformVisualOverride ?? TransformV;

			var positionCalculated = false;

			//get eyes position from skeleton
			if( useEyesPositionOfModel )
			{
				if( GetEyesPosition( tr, out position ) )
					positionCalculated = true;
			}

			//calculate position
			if( !positionCalculated )
			{
				position = tr * TypeCached.EyePosition.Value + GetSmoothCameraOffset();

				//if( CrouchingSupport )
				//{
				//	if( ( crouching && crouchingVisualFactor != 1 ) || ( !crouching && crouchingVisualFactor != 0 ) )
				//	{
				//		float diff = Type.HeightFromPositionToGround - Type.CrouchingHeightFromPositionToGround;
				//		if( !crouching )
				//			position -= new Vector3( 0, 0, diff * crouchingVisualFactor );
				//		else
				//			position += new Vector3( 0, 0, diff * ( 1.0f - crouchingVisualFactor ) );
				//	}
				//}

				//wiggle camera when walking
				var angle = Time.Current * 10;
				var radius = wiggleWhenWalkingSpeedFactor * .04f;
				Vector3 localPosition = new Vector3( 0, Math.Cos( angle ) * radius, Math.Abs( Math.Sin( angle ) * radius ) );
				position += localPosition * tr.ToMatrix4( false, true, true );//GetInterpolatedRotation();
			}

			//calculate up vector. wiggle camera when walking
			{
				var angle = Time.Current * 20;
				var radius = wiggleWhenWalkingSpeedFactor * .003f;
				Vector3 localUp = new Vector3( Math.Cos( angle ) * radius, Math.Sin( angle ) * radius, 1 );
				localUp.Normalize();
				up = localUp * tr.Rotation;//GetInterpolatedRotation();
			}

			//calculate forward vector
			forward = CurrentTurnToDirection.GetVector();
		}

		//public Vector3 GetSmoothPosition()
		//{
		//	return TransformV.Position + GetSmoothCameraOffset();
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform GetCenteredTransform()
		{
			var tr = TransformV;
			var offset = new Vector3( 0, 0, TypeCached.Height * 0.5 );
			return new Transform( tr.Position + tr.Rotation * offset, tr.Rotation, tr.Scale );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 GetCenteredPosition()
		{
			return TransformV.Position + new Vector3( 0, 0, TypeCached.Height * 0.5 );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 GetCenteredSmoothPosition()
		{
			var result = GetCenteredPosition();
			result.Z += smoothCameraOffsetZ;
			return result;
			//return TransformV.Position + new Vector3( 0, 0, TypeCached.Height * 0.5 ) + GetSmoothCameraOffset();
		}

		public MeshInSpaceAnimationController GetAnimationController()
		{
			if( animationControllerCached == null )
			{
				//get or create animation controller
				animationControllerCached = GetComponent<MeshInSpaceAnimationController>();// onlyEnabledInHierarchy: true );
				if( animationControllerCached == null && !NetworkIsClient )//&& EngineApp.IsSimulation && createInSimulationIfNotExists )
				{
					animationControllerCached = CreateComponent<MeshInSpaceAnimationController>();
					animationControllerCached.Name = "Animation Controller";
				}
			}

			return animationControllerCached;
		}

		//!!!!temp
		//List<Vector3> _tempDebug = new List<Vector3>();


		[MethodImpl( (MethodImplOptions)512 )]
		void TickAnimate( float delta )
		{
			//play one animation
			if( playOneAnimation != null )
			{
				playOneAnimationRemainingTime -= delta;
				if( playOneAnimationRemainingTime <= 0 )
					StartPlayOneAnimation( null );
			}

			//play one animation additional
			if( playOneAnimationAdditional != null )
			{
				playOneAnimationAdditionalRemainingTime -= delta;
				if( playOneAnimationAdditionalRemainingTime <= 0 )
					StartPlayOneAnimationAdditional( null );
			}

			//if( TypeCached.Animate )
			{
				var controller = GetAnimationController();
				if( controller != null )
				{
					var activeItem = GetActiveItem();

					Weapon activeWeapon;
					bool rifleAnimations;
					bool oneHandedMeleeWeaponAnimations;
					bool basicItemAnimations;
					if( activeItem != null )
					{
						activeWeapon = activeItem as Weapon;
						rifleAnimations = activeWeapon != null && activeWeapon.TypeCached.WayToUse.Value == WeaponType.WayToUseEnum.Rifle;
						oneHandedMeleeWeaponAnimations = activeWeapon != null && activeWeapon.TypeCached.WayToUse.Value == WeaponType.WayToUseEnum.OneHandedMelee;
						basicItemAnimations = activeItem as Item != null;
					}
					else
					{
						activeWeapon = null;
						rifleAnimations = false;
						oneHandedMeleeWeaponAnimations = false;
						basicItemAnimations = false;
					}

					Animation animation = null;
					double speed = 1;
					bool autoRewind = true;
					bool freezeOnEnd = false;

					if( PhysicalBody != null )
					{
						if( IsOnGround() )// IsOnGroundWithLatency() )
						{
							var localVelocityNoScale = TransformV.Rotation.GetInverse() * GetGroundRelativeVelocity() / GetScaleFactor();
							//var localVelocityNoScale = TransformV.Rotation.GetInverse() * GetLinearVelocity() / GetScaleFactor();

							var localSpeedNoScale = localVelocityNoScale.X;
							//var localSpeedNoScale = localVelocityNoScale.ToVector2().Length();
							//var localMovementAngle = Math.Atan2( localVelocityNoScale.Y, localVelocityNoScale.X );

							//Run
							if( TypeCached.Run )
							{
								var running = Math.Abs( localSpeedNoScale ) > TypeCached.RunForwardMaxSpeed * 0.6;
								//var running = localSpeedNoScale > TypeCached.RunForwardMaxSpeed * 0.6;
								if( running )
								{
									animation = TypeCached.RunAnimation;
									if( animation != null )
										speed = TypeCached.RunAnimationSpeed * localSpeedNoScale;
								}
							}

							//Walk
							if( animation == null )
							{
								var walking = Math.Abs( localSpeedNoScale ) > TypeCached.WalkForwardMaxSpeed * 0.2 || Math.Abs( localVelocityNoScale.Y ) > TypeCached.WalkForwardMaxSpeed * 0.2;
								//var walking = localSpeedNoScale > TypeCached.WalkForwardMaxSpeed * 0.2;
								if( walking )
								{
									if( rifleAnimations )
									{
										animation = TypeCached.RifleAimingWalkAnimation;
										if( animation != null )
											speed = TypeCached.RifleAimingWalkAnimationSpeed * localSpeedNoScale;
									}
									if( oneHandedMeleeWeaponAnimations )
									{
										animation = TypeCached.OneHandedMeleeWeaponWalkAnimation;
										if( animation != null )
											speed = TypeCached.OneHandedMeleeWeaponWalkAnimationSpeed * localSpeedNoScale;
									}
									if( animation == null )
									{
										animation = TypeCached.WalkAnimation;
										if( animation != null )
											speed = TypeCached.WalkAnimationSpeed * localSpeedNoScale;
									}
								}
							}

							//Left Turn, Right Turn
							if( animation == null )
							{
								if( RequiredTurnToDirection.HasValue && IsOnGround() )
								{
									if( CurrentTurnToDirection.Horizontal != RequiredTurnToDirection.Value.Horizontal )
									{
										var angle = RequiredTurnToDirection.Value.Horizontal - CurrentTurnToDirection.Horizontal;
										var leftTurn = Math.Sin( angle ) > 0;

										animation = leftTurn ? TypeCached.LeftTurnAnimation : TypeCached.RightTurnAnimation;
										if( animation != null )
											speed = TypeCached.TurnAnimationSpeed;
									}
								}
							}
						}
						else
						{
							if( !IsOnGroundWithLatency() )
								animation = TypeCached.FlyAnimation;
						}
					}

					if( Sitting )
					{
						if( animation == null )
							animation = TypeCached.SitAnimation;
					}

					//Idle
					if( animation == null )
					{
						if( rifleAnimations )
							animation = TypeCached.RifleAimingIdleAnimation;
						if( oneHandedMeleeWeaponAnimations )
							animation = TypeCached.OneHandedMeleeWeaponIdleAnimation;
						if( animation == null )
							animation = TypeCached.IdleAnimation;
					}

					//play one animation
					if( playOneAnimation != null )
					{
						animation = playOneAnimation;
						speed = playOneAnimationSpeed;
						autoRewind = false;
						freezeOnEnd = playOneAnimationFreezeOnEnd;
					}

					//!!!!GC
					var state = new MeshInSpaceAnimationController.AnimationStateClass();
					state.Animations.Add( new MeshInSpaceAnimationController.AnimationStateClass.AnimationItem() { Animation = animation, Speed = speed, AutoRewind = autoRewind, FreezeOnEnd = freezeOnEnd } );


					//additional animations

					if( playOneAnimationAdditional != null )
						state.Animations.Add( playOneAnimationAdditional );

					if( rifleAnimations )
					{
						var rifleIdle = TypeCached.RifleAimingIdleAnimation.Value;
						var rifleIdleMinus45 = TypeCached.RifleAimingIdleMinus45Animation.Value;
						var rifleIdlePlus45 = TypeCached.RifleAimingIdlePlus45Animation.Value;

						if( rifleIdle != null && ( rifleIdleMinus45 != null || rifleIdlePlus45 != null ) )
						{
							//!!!!good to take angle from transform offset?
							var transformOffset = activeWeapon.GetComponent<TransformOffset>();
							if( transformOffset != null )
							{
								var spherical = SphericalDirection.FromVector( transformOffset.RotationOffset.Value.GetForward() );
								var vertical = (float)spherical.Vertical;

								if( vertical <= 0 && rifleIdleMinus45 != null )
								{
									var factor = -vertical / ( MathEx.PI / 4 );
									if( factor > 1 )
										factor = 1;

									state.Animations.Add( new MeshInSpaceAnimationController.AnimationStateClass.AnimationItem() { Animation = rifleIdle, Animation2 = rifleIdleMinus45, Animation2Factor = factor, AnimationItemTag = 2, AffectBonesWithChildren = new string[] { TypeCached.UpperPartBone }, ReplaceMode = true } );
								}
								else if( vertical > 0 && rifleIdlePlus45 != null )
								{
									var factor = vertical / ( MathEx.PI / 4 );
									if( factor > 1 )
										factor = 1;

									state.Animations.Add( new MeshInSpaceAnimationController.AnimationStateClass.AnimationItem() { Animation = rifleIdle, Animation2 = rifleIdlePlus45, Animation2Factor = factor, AnimationItemTag = 2, AffectBonesWithChildren = new string[] { TypeCached.UpperPartBone }, ReplaceMode = true } );
								}
							}
						}
					}

					if( basicItemAnimations )
					{
						var idle = TypeCached.ItemHoldingIdleAnimation.Value;
						if( idle == null )
							idle = TypeCached.IdleAnimation.Value;

						if( idle != null )
						{
							//affect only right shoulder
							state.Animations.Add( new MeshInSpaceAnimationController.AnimationStateClass.AnimationItem() { Animation = idle, AnimationItemTag = 4, AffectBonesWithChildren = new string[] { TypeCached.RightShoulderBone }, ReplaceMode = true } );
						}
					}

					state.AdditionalBoneTransformsUpdate = AdditionalBoneTransformsUpdate;

					//update controller
					controller.SetAnimationState( state, true );
				}
			}
		}

		class InverseTransformValue
		{
			public Matrix4 Value;
		}

		//!!!!static, multithreading
		[MethodImpl( (MethodImplOptions)512 )]
		protected virtual void AdditionalBoneTransformsUpdate( MeshInSpaceAnimationController controller, MeshInSpaceAnimationController.AnimationStateClass animationState, Skeleton skeleton, SkeletonAnimationTrack.CalculateBoneTransformsItem[] outputBoneTransforms )
		{
			//this code is quite specialized

			//!!!!
			//_tempDebug.Clear();


			//get inverse transform with caching
			InverseTransformValue inverseTransform = null;
			InverseTransformValue GetInverseTransform()
			{
				if( inverseTransform == null )
				{
					inverseTransform = new InverseTransformValue();
					( TransformVisualOverride ?? TransformV ).ToMatrix4().GetInverse( out inverseTransform.Value );
				}
				return inverseTransform;
			}


			var globalBoneTransforms = controller.GetBoneGlobalTransforms();

			var activeItem = GetActiveItem();
			Weapon weapon = null;
			Item basicItem = null;
			if( activeItem != null )
			{
				weapon = activeItem as Weapon;
				basicItem = activeItem as Item;
			}

			var firstPersonCamera = IsControlledByPlayerAndFirstPersonCameraEnabled();

			//rotate look to position for rifle weapon
			if( weapon != null && weapon.TypeCached.WayToUse.Value == WeaponType.WayToUseEnum.Rifle && !Sitting )
			{
				var transformOffset = weapon.GetComponent<TransformOffset>();
				if( transformOffset != null )
				{
					//get factor from animation state
					var morphFactor = 0.0f;
					var animationItem = animationState.FindItemByAnimationItemTag( 2 );
					if( animationItem != null && animationItem.CurrentFactor.HasValue )
						morphFactor = animationItem.CurrentFactor.Value;

					var leftHandBoneIndex = controller.GetBoneIndex( TypeCached.LeftHandBone );
					var rightHandBoneIndex = controller.GetBoneIndex( TypeCached.RightHandBone );
					if( leftHandBoneIndex != -1 && rightHandBoneIndex != -1 )
					{
						var spineBoneIndexes = new List<int>();
						{
							var boneIndex = controller.GetBoneIndex( TypeCached.SpineBone1 );
							if( boneIndex != -1 )
								spineBoneIndexes.Add( boneIndex );
						}
						{
							var boneIndex = controller.GetBoneIndex( TypeCached.SpineBone2 );
							if( boneIndex != -1 )
								spineBoneIndexes.Add( boneIndex );
						}
						{
							var boneIndex = controller.GetBoneIndex( TypeCached.SpineBone3 );
							if( boneIndex != -1 )
								spineBoneIndexes.Add( boneIndex );
						}

						if( spineBoneIndexes.Count != 0 )
						{
							controller.CalculateGlobalBoneTransforms();


							Vector3 requiredArmsCenterPosition;
							if( transformOffset.AnyData != null && transformOffset.AnyData is Vector3 )
							{
								requiredArmsCenterPosition = (Vector3)transformOffset.AnyData - GetSmoothCameraOffset() + transformOffset.RotationOffset.Value * weapon.TypeCached.GetArmsCenter();
							}
							else
							{
								requiredArmsCenterPosition = transformOffset.PositionOffset.Value - GetSmoothCameraOffset() + transformOffset.RotationOffset.Value * weapon.TypeCached.GetArmsCenter();
							}


							for( int boneNumber = 0; boneNumber < spineBoneIndexes.Count; boneNumber++ )
							{
								var spineBoneIndex = spineBoneIndexes[ boneNumber ];
								var spineBonePosition = globalBoneTransforms[ spineBoneIndex ].Position;
								var spineBoneRotation = globalBoneTransforms[ spineBoneIndex ].Rotation;
								ref var spineBoneItem = ref outputBoneTransforms[ spineBoneIndex ];

								var currentArmsCenterPosition = ( globalBoneTransforms[ leftHandBoneIndex ].Position + globalBoneTransforms[ rightHandBoneIndex ].Position ) * 0.5f;

								var toCurrentDirection = SphericalDirectionF.FromVector( currentArmsCenterPosition - spineBonePosition );
								var toRequiredDirection = SphericalDirectionF.FromVector( requiredArmsCenterPosition.ToVector3F() - spineBonePosition );
								var diffDirection = toRequiredDirection - toCurrentDirection;

								var spineBoneRotationNew = QuaternionF.FromRotateByZ( -diffDirection.Horizontal ) * QuaternionF.FromRotateByY( diffDirection.Vertical ) * spineBoneRotation;

								float factor = 0.3333f + boneNumber * 0.3333f;
								if( factor > 0.99f )
									factor = 1;

								spineBoneItem.Rotation = QuaternionF.Slerp( spineBoneRotation, spineBoneRotationNew, factor * morphFactor );
								spineBoneItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
								globalBoneTransforms[ spineBoneIndex ].NeedUpdate = true;
							}
						}

						//calibrate hands position

						{
							controller.CalculateGlobalBoneTransforms();

							{
								var boneLeftShoulderIndex = controller.GetBoneIndex( TypeCached.LeftShoulderBone );
								var boneLeftArmIndex = controller.GetBoneIndex( TypeCached.LeftArmBone );
								//var boneLeftForeArmIndex = controller.GetBoneIndex( "mixamorig:LeftForeArm" );
								var boneLeftHandIndex = controller.GetBoneIndex( TypeCached.LeftHandBone );
								var boneLeftHandMiddle1Index = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Left", "Middle", 1 ) );

								if( boneLeftShoulderIndex != -1 && boneLeftArmIndex != -1 && boneLeftHandIndex != -1 && boneLeftHandMiddle1Index != -1 )
								{
									ref var boneLeftShoulderItem = ref outputBoneTransforms[ boneLeftShoulderIndex ];
									ref var boneLeftArmItem = ref outputBoneTransforms[ boneLeftArmIndex ];

									var boneLeftHand = globalBoneTransforms[ boneLeftHandIndex ].Position;
									var boneLeftHandMiddle1 = globalBoneTransforms[ boneLeftHandMiddle1Index ].Position;
									var currentPosition = Vector3F.Lerp( boneLeftHand, boneLeftHandMiddle1, 0.75f );

									var requiredPosition = transformOffset.PositionOffset.Value - GetSmoothCameraOffset() + transformOffset.RotationOffset.Value * weapon.TypeCached.GetHandguardCenter() - new Vector3( 0, 0, 0.01 );

									var sourceBonePosition = globalBoneTransforms[ boneLeftShoulderIndex ].Position;
									var sourceBoneRotation = globalBoneTransforms[ boneLeftShoulderIndex ].Rotation;

									var rotation1 = QuaternionF.FromDirectionZAxisUp( ( requiredPosition - sourceBonePosition ).ToVector3F() );
									var rotation2 = QuaternionF.FromDirectionZAxisUp( currentPosition - sourceBonePosition );

									var sourceBoneRotationNew = ( rotation1 * rotation2.GetInverse() ) * sourceBoneRotation;

									//!!!!
									var factor = 1.0f;

									boneLeftShoulderItem.Rotation = QuaternionF.Slerp( sourceBoneRotation, sourceBoneRotationNew, factor * morphFactor );
									boneLeftShoulderItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
									globalBoneTransforms[ boneLeftShoulderIndex ].NeedUpdate = true;

									//don't change next bone rotation
									boneLeftArmItem.Rotation = globalBoneTransforms[ boneLeftArmIndex ].Rotation;
									boneLeftArmItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
									globalBoneTransforms[ boneLeftArmIndex ].NeedUpdate = true;
								}
							}


							{
								var boneRightShoulderIndex = controller.GetBoneIndex( TypeCached.RightShoulderBone );
								var boneRightArmIndex = controller.GetBoneIndex( TypeCached.RightArmBone );
								//var boneRightForeArmIndex = controller.GetBoneIndex( "mixamorig:RightForeArm" );
								var boneRightHandIndex = controller.GetBoneIndex( TypeCached.RightHandBone );
								var boneRightHandMiddle1Index = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Right", "Middle", 1 ) );

								if( boneRightShoulderIndex != -1 && boneRightArmIndex != -1 && boneRightHandIndex != -1 && boneRightHandMiddle1Index != -1 )
								{
									ref var boneRightShoulderItem = ref outputBoneTransforms[ boneRightShoulderIndex ];
									ref var boneRightArmItem = ref outputBoneTransforms[ boneRightArmIndex ];

									var boneRightHand = globalBoneTransforms[ boneRightHandIndex ].Position;
									var boneRightHandMiddle1 = globalBoneTransforms[ boneRightHandMiddle1Index ].Position;
									//var currentPosition = boneRightHandMiddle1;
									var currentPosition = Vector3F.Lerp( boneRightHand, boneRightHandMiddle1, 0.75f );

									var requiredPosition = transformOffset.PositionOffset.Value - GetSmoothCameraOffset() + transformOffset.RotationOffset.Value * weapon.TypeCached.GetPistolGripCenter();

									var sourceBonePosition = globalBoneTransforms[ boneRightShoulderIndex ].Position;
									var sourceBoneRotation = globalBoneTransforms[ boneRightShoulderIndex ].Rotation;

									var rotation1 = QuaternionF.FromDirectionZAxisUp( ( requiredPosition - sourceBonePosition ).ToVector3F() );
									var rotation2 = QuaternionF.FromDirectionZAxisUp( currentPosition - sourceBonePosition );

									var sourceBoneRotationNew = ( rotation1 * rotation2.GetInverse() ) * sourceBoneRotation;
									//var sourceBoneRotationNew = sourceBoneRotation * ( rotation1 * rotation2.GetInverse() );

									//	sourceBoneRotationNew = ( rotation2.GetInverse() * rotation1 ) * sourceBoneRotation;

									//!!!!
									var factor = 1.0f;

									boneRightShoulderItem.Rotation = QuaternionF.Slerp( sourceBoneRotation, sourceBoneRotationNew, factor * morphFactor );
									boneRightShoulderItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
									globalBoneTransforms[ boneRightShoulderIndex ].NeedUpdate = true;

									//don't change next bone rotation
									boneRightArmItem.Rotation = globalBoneTransforms[ boneRightArmIndex ].Rotation;
									boneRightArmItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
									globalBoneTransforms[ boneRightArmIndex ].NeedUpdate = true;
								}
							}
						}

					}
				}
			}

			//rotate look to position for basic item
			if( basicItem != null && CurrentLookToPosition.HasValue && !Sitting )
			{
				var transformOffset = basicItem.GetComponent<TransformOffset>();
				if( transformOffset != null )
				{
					//get factor from animation state
					var morphFactor = 0.0f;
					var animationItem = animationState.FindItemByAnimationItemTag( 4 );
					if( animationItem != null && animationItem.CurrentFactor.HasValue )
						morphFactor = animationItem.CurrentFactor.Value;

					//rotate upper body part
					{
						var spineBoneIndexes = new List<int>();
						{
							var boneIndex = controller.GetBoneIndex( TypeCached.SpineBone1 );
							if( boneIndex != -1 )
								spineBoneIndexes.Add( boneIndex );
						}
						{
							var boneIndex = controller.GetBoneIndex( TypeCached.SpineBone2 );
							if( boneIndex != -1 )
								spineBoneIndexes.Add( boneIndex );
						}
						{
							var boneIndex = controller.GetBoneIndex( TypeCached.SpineBone3 );
							if( boneIndex != -1 )
								spineBoneIndexes.Add( boneIndex );
						}

						if( spineBoneIndexes.Count != 0 )
						{
							controller.CalculateGlobalBoneTransforms();

							var tr = TransformV;
							var current = QuaternionF.FromDirectionZAxisUp( tr.Rotation.GetForward().ToVector3F() );
							var diff = CurrentLookToPosition.Value - tr.Position;
							var required = QuaternionF.LookAt( new Vector3F( (float)diff.X, (float)diff.Y, 0 ), Vector3F.ZAxis );
							var diffRotation = required * current.GetInverse();

							for( int boneNumber = 0; boneNumber < spineBoneIndexes.Count; boneNumber++ )
							{
								var spineBoneIndex = spineBoneIndexes[ boneNumber ];
								ref var spineBoneRotation = ref globalBoneTransforms[ spineBoneIndex ].Rotation;
								ref var spineBoneItem = ref outputBoneTransforms[ spineBoneIndex ];

								var spineBoneRotationNew = diffRotation * spineBoneRotation;

								float factor = 0.3333f + boneNumber * 0.3333f;
								if( factor > 0.99f )
									factor = 1;

								spineBoneItem.Rotation = QuaternionF.Slerp( spineBoneRotation, spineBoneRotationNew, factor * morphFactor );
								spineBoneItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
								globalBoneTransforms[ spineBoneIndex ].NeedUpdate = true;
							}
						}
					}


					controller.CalculateGlobalBoneTransforms();


					//calibrate hands position
					{
						QuaternionF CalculateLookAt( Vector3F diff )
						{
							var angle = MathEx.Atan2( diff.Z, diff.X );
							angle -= MathEx.PI / 2;
							return QuaternionF.LookAt( diff, new SphericalDirectionF( 0, angle ).GetVector() );
						}

						var handBoneIndex = controller.GetBoneIndex( TypeCached.RightHandBone );
						if( handBoneIndex != -1 )
						{
							var handBoneComponent = controller.Bones[ handBoneIndex ];

							var foreArmBoneComponent = handBoneComponent.Parent as SkeletonBone;
							if( foreArmBoneComponent != null )
							{
								var foreArmBoneIndex = controller.GetBoneIndex( foreArmBoneComponent.Name );
								if( foreArmBoneIndex != -1 )
								{
									var armBoneComponent = foreArmBoneComponent.Parent as SkeletonBone;
									if( armBoneComponent != null )
									{
										var armBoneIndex = controller.GetBoneIndex( armBoneComponent.Name );
										if( armBoneIndex != -1 )
										{
											var requiredHandPosition = ( GetInverseTransform().Value * basicItem.TransformV.Position ).ToVector3F();

											//custom
											requiredHandPosition += transformOffset.RotationOffset.Value.ToQuaternionF() * new Vector3F( -0.13f, 0, 0 );
											//requiredHandPosition += transformOffset.RotationOffset.Value.ToQuaternionF() * new Vector3F( -0.07f, 0.03f, 0.03f );

											//calculate arm bone rotation
											{
												ref var handBonePosition = ref globalBoneTransforms[ handBoneIndex ].Position;
												ref var foreArmBonePosition = ref globalBoneTransforms[ foreArmBoneIndex ].Position;
												ref var armBonePosition = ref globalBoneTransforms[ armBoneIndex ].Position;
												ref var armBoneRotation = ref globalBoneTransforms[ armBoneIndex ].Rotation;

												Vector3F requiredForeArmPosition;
												{
													var bone1Length = ( foreArmBonePosition - armBonePosition ).Length();
													var bone2Length = ( handBonePosition - foreArmBonePosition ).Length();
													var totalLength = bone1Length + bone2Length;
													var requiredLength = ( requiredHandPosition - armBonePosition ).Length();

													if( requiredLength >= totalLength )
													{
														//flat
														requiredForeArmPosition = ( armBonePosition + requiredHandPosition ) * 0.5f;
													}
													else
													{
														//bend

														var a = requiredLength;
														var b = bone1Length;
														var c = bone2Length;
														var p = 0.5 * ( a + b + c );
														var h = ( 2.0 * Math.Sqrt( p * ( p - a ) * ( p - b ) * ( p - c ) ) ) / a;

														var yAngle = (float)Math.Asin( h / b );//h = b * sin(yAngle)

														var dir = ( requiredHandPosition - armBonePosition ).GetNormalize();
														var sphericalDir = SphericalDirectionF.FromVector( dir );

														sphericalDir.Vertical -= yAngle;

														requiredForeArmPosition = armBonePosition + sphericalDir.GetVector().GetNormalize() * bone1Length;

														//add horizontal offset
														requiredForeArmPosition.Y -= 0.1f;

														//!!!!
														////apply twist
														//{
														//	var twistAngle = new DegreeF( 20 ).InRadians();
														//	var rot = QuaternionF.FromDirectionZAxisUp( requiredHandPosition - armBonePosition ) * QuaternionF.FromRotateByX( twistAngle );
														//	//var rot = QuaternionF.FromRotateByX( twistAngle ) * QuaternionF.FromDirectionZAxisUp( requiredHandPosition - armBonePosition );

														//	var diff = requiredForeArmPosition - armBonePosition;
														//	diff = rot * diff;
														//	requiredForeArmPosition = armBonePosition + diff;
														//}
													}
												}

												ref var armBoneItem = ref outputBoneTransforms[ armBoneIndex ];

												var current = CalculateLookAt( foreArmBonePosition - armBonePosition );
												var required = CalculateLookAt( requiredForeArmPosition - armBonePosition );
												var diffRotation = required * current.GetInverse();

												var armBoneRotationNew = diffRotation * armBoneRotation;

												armBoneItem.Rotation = QuaternionF.Slerp( armBoneRotation, armBoneRotationNew, morphFactor );
												armBoneItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
												globalBoneTransforms[ armBoneIndex ].NeedUpdate = true;
											}


											controller.CalculateGlobalBoneTransforms();


											//calculate fore arm bone rotation
											{
												ref var handBonePosition = ref globalBoneTransforms[ handBoneIndex ].Position;
												ref var foreArmBonePosition = ref globalBoneTransforms[ foreArmBoneIndex ].Position;
												ref var foreArmBoneRotation = ref globalBoneTransforms[ foreArmBoneIndex ].Rotation;

												ref var foreArmBoneItem = ref outputBoneTransforms[ foreArmBoneIndex ];

												var current = CalculateLookAt( handBonePosition - foreArmBonePosition );
												var required = CalculateLookAt( requiredHandPosition - foreArmBonePosition );
												var diffRotation = required * current.GetInverse();

												var foreArmBoneRotationNew = diffRotation * foreArmBoneRotation;

												foreArmBoneItem.Rotation = QuaternionF.Slerp( foreArmBoneRotation, foreArmBoneRotationNew, morphFactor );
												foreArmBoneItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
												globalBoneTransforms[ foreArmBoneIndex ].NeedUpdate = true;
											}


											controller.CalculateGlobalBoneTransforms();


											//calculate hand rotation
											{
												ref var handBoneRotation = ref globalBoneTransforms[ handBoneIndex ].Rotation;
												ref var handBoneItem = ref outputBoneTransforms[ handBoneIndex ];

												//custom
												var handBoneRotationNew = transformOffset.RotationOffset.Value.ToQuaternionF();
												handBoneRotationNew *= QuaternionF.FromRotateByZ( new DegreeF( 90 ).InRadians() );
												handBoneRotationNew *= QuaternionF.FromRotateByY( new DegreeF( 180 ).InRadians() );

												//var handBoneRotationNew = transformOffset.RotationOffset.Value.ToQuaternionF() * QuaternionF.FromRotateByZ( new DegreeF( -65 ).InRadians() ) * new QuaternionF( 0.5f, 0.5f, 0.5f, 0.5f );

												handBoneItem.Rotation = QuaternionF.Slerp( handBoneRotation, handBoneRotationNew, morphFactor );
												handBoneItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
												globalBoneTransforms[ handBoneIndex ].NeedUpdate = true;
											}
										}
									}
								}
							}

						}
					}
				}
			}

			//sitting
			if( Sitting )
			{
				controller.CalculateGlobalBoneTransforms();

				//!!!!maybe rotate the root and the beginning of the legs in the other direction to get better visual result

				//!!!!control hands


				{
					var boneIndex = controller.GetBoneIndex( TypeCached.SpineBone1 );
					if( boneIndex != -1 )
					{
						ref var boneItem = ref outputBoneTransforms[ boneIndex ];
						var boneRotation = globalBoneTransforms[ boneIndex ].Rotation;

						boneItem.Rotation = QuaternionF.FromRotateByY( SittingSpineAngle.ToDegreeF().InRadians() ) * boneRotation;
						boneItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
						globalBoneTransforms[ boneIndex ].NeedUpdate = true;
					}
				}

				var legsRotation = QuaternionF.FromRotateByY( SittingLegsAngle.ToDegreeF().InRadians() );

				{
					var boneIndex = controller.GetBoneIndex( typeCached.LeftLegBone );
					if( boneIndex != -1 )
					{
						ref var boneItem = ref outputBoneTransforms[ boneIndex ];
						var boneRotation = globalBoneTransforms[ boneIndex ].Rotation;

						boneItem.Rotation = legsRotation * boneRotation;
						boneItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
						globalBoneTransforms[ boneIndex ].NeedUpdate = true;
					}
				}

				{
					var boneIndex = controller.GetBoneIndex( typeCached.RightLegBone );
					if( boneIndex != -1 )
					{
						ref var boneItem = ref outputBoneTransforms[ boneIndex ];
						var boneRotation = globalBoneTransforms[ boneIndex ].Rotation;

						boneItem.Rotation = legsRotation * boneRotation;
						boneItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
						globalBoneTransforms[ boneIndex ].NeedUpdate = true;
					}
				}
			}

			//hands
			for( var hand = HandEnum.Left; hand <= HandEnum.Right; hand++ )
			{
				var morphFactor = (float)( hand == HandEnum.Left ? LeftHandFactor.Value : RightHandFactor.Value );
				if( morphFactor > 0 )
				{
					var requiredWorldHandTransform = hand == HandEnum.Left ? LeftHandTransform.Value : RightHandTransform.Value;
					var requiredHandPosition = ( GetInverseTransform().Value * requiredWorldHandTransform.Position ).ToVector3F();
					//!!!!impl
					//var requiredHandRotation = Quaternion.LookAt( inverseTransform * worldHandTransform.Rotation.GetForward(), inverseTransform * worldHandTransform.Rotation.GetUp() );

					//!!!!
					//_tempDebug.Add( localHandPosition );

					QuaternionF CalculateLookAt( Vector3F diff )
					{
						var angle = MathEx.Atan2( diff.Z, diff.X );
						angle -= MathEx.PI / 2;
						return QuaternionF.LookAt( diff, new SphericalDirectionF( 0, angle ).GetVector() );
					}

					var handBoneIndex = controller.GetBoneIndex( hand == HandEnum.Left ? TypeCached.LeftHandBone : TypeCached.RightHandBone );
					if( handBoneIndex != -1 )
					{
						var handBoneComponent = controller.Bones[ handBoneIndex ];

						var foreArmBoneComponent = handBoneComponent.Parent as SkeletonBone;
						if( foreArmBoneComponent != null )
						{
							var foreArmBoneIndex = controller.GetBoneIndex( foreArmBoneComponent.Name );
							if( foreArmBoneIndex != -1 )
							{
								var armBoneComponent = foreArmBoneComponent.Parent as SkeletonBone;
								if( armBoneComponent != null )
								{
									var armBoneIndex = controller.GetBoneIndex( armBoneComponent.Name );
									if( armBoneIndex != -1 )
									{

										controller.CalculateGlobalBoneTransforms();


										//calculate arm bone rotation
										{
											ref var handBonePosition = ref globalBoneTransforms[ handBoneIndex ].Position;
											ref var foreArmBonePosition = ref globalBoneTransforms[ foreArmBoneIndex ].Position;
											ref var armBonePosition = ref globalBoneTransforms[ armBoneIndex ].Position;
											ref var armBoneRotation = ref globalBoneTransforms[ armBoneIndex ].Rotation;

											Vector3F requiredForeArmPosition;
											{
												var bone1Length = ( foreArmBonePosition - armBonePosition ).Length();
												var bone2Length = ( handBonePosition - foreArmBonePosition ).Length();
												var totalLength = bone1Length + bone2Length;
												var requiredLength = ( requiredHandPosition - armBonePosition ).Length();

												if( requiredLength >= totalLength )
												{
													//flat
													requiredForeArmPosition = ( armBonePosition + requiredHandPosition ) * 0.5f;
												}
												else
												{
													//bend

													var a = requiredLength;
													var b = bone1Length;
													var c = bone2Length;
													var p = 0.5 * ( a + b + c );
													var h = ( 2.0 * Math.Sqrt( p * ( p - a ) * ( p - b ) * ( p - c ) ) ) / a;

													var yAngle = (float)Math.Asin( h / b );//h = b * sin(yAngle)

													var dir = ( requiredHandPosition - armBonePosition ).GetNormalize();
													var sphericalDir = SphericalDirectionF.FromVector( dir );

													sphericalDir.Vertical -= yAngle;

													requiredForeArmPosition = armBonePosition + sphericalDir.GetVector().GetNormalize() * bone1Length;
												}
											}

											ref var armBoneItem = ref outputBoneTransforms[ armBoneIndex ];

											var current = CalculateLookAt( foreArmBonePosition - armBonePosition );
											var required = CalculateLookAt( requiredForeArmPosition - armBonePosition );
											var diffRotation = required * current.GetInverse();

											var armBoneRotationNew = diffRotation * armBoneRotation;

											armBoneItem.Rotation = QuaternionF.Slerp( armBoneRotation, armBoneRotationNew, morphFactor );
											armBoneItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
											globalBoneTransforms[ armBoneIndex ].NeedUpdate = true;
										}


										controller.CalculateGlobalBoneTransforms();


										//calculate fore arm bone rotation
										{
											ref var handBonePosition = ref globalBoneTransforms[ handBoneIndex ].Position;
											ref var foreArmBonePosition = ref globalBoneTransforms[ foreArmBoneIndex ].Position;
											ref var foreArmBoneRotation = ref globalBoneTransforms[ foreArmBoneIndex ].Rotation;

											ref var foreArmBoneItem = ref outputBoneTransforms[ foreArmBoneIndex ];

											var current = CalculateLookAt( handBonePosition - foreArmBonePosition );
											var required = CalculateLookAt( requiredHandPosition - foreArmBonePosition );
											var diffRotation = required * current.GetInverse();

											var foreArmBoneRotationNew = diffRotation * foreArmBoneRotation;

											foreArmBoneItem.Rotation = QuaternionF.Slerp( foreArmBoneRotation, foreArmBoneRotationNew, morphFactor );
											foreArmBoneItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
											globalBoneTransforms[ foreArmBoneIndex ].NeedUpdate = true;
										}


										//controller.CalculateGlobalBoneTransforms();


										////calculate hand rotation
										//{
										//	ref var handBoneRotation = ref globalBoneTransforms[ handBoneIndex ].Rotation;
										//	ref var handBoneItem = ref outputBoneTransforms[ handBoneIndex ];

										//	//custom
										//	var handBoneRotationNew = transformOffset.RotationOffset.Value.ToQuaternionF() * QuaternionF.FromRotateByZ( new DegreeF( -65 ).InRadians() ) * new QuaternionF( 0.5f, 0.5f, 0.5f, 0.5f );

										//	handBoneItem.Rotation = QuaternionF.Slerp( handBoneRotation, handBoneRotationNew, morphFactor );
										//	handBoneItem.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
										//	globalBoneTransforms[ handBoneIndex ].NeedUpdate = true;
										//}

									}
								}
							}
						}
					}
				}
			}

			//head
			{
				var headFactor = HeadFactor.Value;

				var factor = headFactor;
				Vector3F? localLookAt = null;

				if( headFactor == 0 )
				{
					//!!!!
					//rotate head for first person camera when no item activated
					if( headFactor == 0 /*&& activeItem == null*/ && firstPersonCamera && CurrentLookToPosition.HasValue )
					{
						factor = 1;
						localLookAt = ( GetInverseTransform().Value * CurrentLookToPosition.Value ).ToVector3F();
					}

					//Sitting specific
					if( Sitting )
					{
						factor = 1;
						localLookAt = new Vector3F( 1000, 0, 0 );
					}
				}

				if( factor > 0 )
				{
					//default behaviour
					if( !localLookAt.HasValue )
						localLookAt = ( GetInverseTransform().Value * HeadLookAt.Value ).ToVector3F();

					var headBoneIndex = controller.GetBoneIndex( TypeCached.HeadBone );
					if( headBoneIndex != -1 )
					{
						ref var headBone = ref outputBoneTransforms[ headBoneIndex ];

						controller.CalculateGlobalBoneTransforms();

						var headBonePosition = globalBoneTransforms[ headBoneIndex ].Position;
						var headBoneRotation = globalBoneTransforms[ headBoneIndex ].Rotation;

						var dir = ( localLookAt.Value - headBonePosition ).GetNormalize();
						var rotation = QuaternionF.LookAt( dir, Vector3F.ZAxis );

						//!!!!maybe better to multiply to initial bone transform? controller.Bones[ headBoneIndex ].Transform.Value.Rotation;
						var destRotation = rotation * new QuaternionF( 0.5f, 0.5f, 0.5f, 0.5f );
						headBone.Rotation = QuaternionF.Slerp( headBoneRotation, destRotation, (float)factor );
						headBone.Flags |= SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation;
						globalBoneTransforms[ headBoneIndex ].NeedUpdate = true;
					}
				}
			}


			//!!!!can skip fingers update on far distances

			//left hand fingers
			{
				if( LeftHandThumbFingerFlexionFactor > 0 )
				{
					var value = (float)LeftHandThumbFingerFlexionValue.Value;

					var rotation =
						-QuaternionF.FromRotateByY( MathEx.PI / 2 / -1.0f * value / 4 ) *
						-QuaternionF.FromRotateByX( MathEx.PI / 2 / -1.0f * value / 4 ) *
						-QuaternionF.FromRotateByZ( MathEx.PI / 2 / 1.0f * value / 4 );

					for( int n = 1; n <= 3; n++ )
					{
						var boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Left", "Thumb", n ) );
						if( boneIndex != -1 )
						{
							ref var item = ref outputBoneTransforms[ boneIndex ];
							QuaternionF.Slerp( ref item.Rotation, ref rotation, (float)LeftHandThumbFingerFlexionFactor, out item.Rotation );
							globalBoneTransforms[ boneIndex ].NeedUpdate = true;
						}
					}
				}

				if( LeftHandIndexFingerFlexionFactor > 0 )
				{
					var rotation = QuaternionF.FromRotateByX( MathEx.PI / 2 * -1.0f * (float)LeftHandIndexFingerFlexionValue );
					for( int n = 1; n <= 3; n++ )
					{
						var boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Left", "Index", n ) );
						if( boneIndex != -1 )
						{
							ref var item = ref outputBoneTransforms[ boneIndex ];
							QuaternionF.Slerp( ref item.Rotation, ref rotation, (float)LeftHandIndexFingerFlexionFactor, out item.Rotation );
							globalBoneTransforms[ boneIndex ].NeedUpdate = true;
						}
					}
				}

				if( LeftHandMiddleFingerFlexionFactor > 0 )
				{
					var rotation = QuaternionF.FromRotateByX( MathEx.PI / 2 * -1.0f * (float)LeftHandMiddleFingerFlexionValue );
					for( int n = 1; n <= 3; n++ )
					{
						var boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Left", "Middle", n ) );
						if( boneIndex != -1 )
						{
							ref var item = ref outputBoneTransforms[ boneIndex ];
							QuaternionF.Slerp( ref item.Rotation, ref rotation, (float)LeftHandMiddleFingerFlexionFactor, out item.Rotation );
							globalBoneTransforms[ boneIndex ].NeedUpdate = true;
						}
					}
				}

				if( LeftHandRingFingerFlexionFactor > 0 )
				{
					var rotation = QuaternionF.FromRotateByX( MathEx.PI / 2 * -1.0f * (float)LeftHandRingFingerFlexionValue );
					for( int n = 1; n <= 3; n++ )
					{
						var boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Left", "Ring", n ) );
						if( boneIndex != -1 )
						{
							ref var item = ref outputBoneTransforms[ boneIndex ];
							QuaternionF.Slerp( ref item.Rotation, ref rotation, (float)LeftHandRingFingerFlexionFactor, out item.Rotation );
							globalBoneTransforms[ boneIndex ].NeedUpdate = true;
						}
					}
				}

				if( LeftHandLittleFingerFlexionFactor > 0 )
				{
					var rotation = QuaternionF.FromRotateByX( MathEx.PI / 2 * -1.0f * (float)LeftHandLittleFingerFlexionValue );
					for( int n = 1; n <= 3; n++ )
					{
						var boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Left", "Pinky", n ) );
						if( boneIndex == -1 )
							boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Left", "Little", n ) );
						if( boneIndex != -1 )
						{
							ref var item = ref outputBoneTransforms[ boneIndex ];
							QuaternionF.Slerp( ref item.Rotation, ref rotation, (float)LeftHandLittleFingerFlexionFactor, out item.Rotation );
							globalBoneTransforms[ boneIndex ].NeedUpdate = true;
						}
					}
				}
			}

			//right hand fingers
			{
				var basicItemInHand = basicItem != null;

				//thumb
				{
					var factor = (float)RightHandThumbFingerFlexionFactor.Value;
					var value = (float)RightHandThumbFingerFlexionValue.Value;
					if( factor == 0 && basicItemInHand )
					{
						factor = 1;
						value = 0.5f;
					}

					if( factor > 0 )
					{
						var rotation =
							QuaternionF.FromRotateByY( MathEx.PI / 2 / 1.0f * value / 4 ) *
							QuaternionF.FromRotateByX( MathEx.PI / 2 / -1.0f * value / 4 ) *
							QuaternionF.FromRotateByZ( MathEx.PI / 2 / -1.0f * value / 4 );

						for( int n = 1; n <= 3; n++ )
						{
							var boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Right", "Thumb", n ) );
							if( boneIndex != -1 )
							{
								ref var item = ref outputBoneTransforms[ boneIndex ];
								QuaternionF.Slerp( ref item.Rotation, ref rotation, factor, out item.Rotation );
								globalBoneTransforms[ boneIndex ].NeedUpdate = true;
							}
						}
					}
				}

				//index
				{
					var factor = (float)RightHandIndexFingerFlexionFactor.Value;
					var value = (float)RightHandIndexFingerFlexionValue.Value;
					if( factor == 0 && basicItemInHand )
					{
						factor = 1;
						value = 0.5f;
					}

					if( factor > 0 )
					{
						var rotation = QuaternionF.FromRotateByX( MathEx.PI / 2 * -value );
						for( int n = 1; n <= 3; n++ )
						{
							var boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Right", "Index", n ) );
							if( boneIndex != -1 )
							{
								ref var item = ref outputBoneTransforms[ boneIndex ];
								QuaternionF.Slerp( ref item.Rotation, ref rotation, factor, out item.Rotation );
								globalBoneTransforms[ boneIndex ].NeedUpdate = true;
							}
						}
					}
				}

				//middle
				{
					var factor = (float)RightHandMiddleFingerFlexionFactor.Value;
					var value = (float)RightHandMiddleFingerFlexionValue.Value;
					if( factor == 0 && basicItemInHand )
					{
						factor = 1;
						value = 0.6f;
					}

					if( factor > 0 )
					{
						var rotation = QuaternionF.FromRotateByX( MathEx.PI / 2 * -value );
						for( int n = 1; n <= 3; n++ )
						{
							var boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Right", "Middle", n ) );
							if( boneIndex != -1 )
							{
								ref var item = ref outputBoneTransforms[ boneIndex ];
								QuaternionF.Slerp( ref item.Rotation, ref rotation, factor, out item.Rotation );
								globalBoneTransforms[ boneIndex ].NeedUpdate = true;
							}
						}
					}
				}

				//ring
				{
					var factor = (float)RightHandRingFingerFlexionFactor.Value;
					var value = (float)RightHandRingFingerFlexionValue.Value;
					if( factor == 0 && basicItemInHand )
					{
						factor = 1;
						value = 0.7f;
					}

					if( factor > 0 )
					{
						var rotation = QuaternionF.FromRotateByX( MathEx.PI / 2 * -value );
						for( int n = 1; n <= 3; n++ )
						{
							var boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Right", "Ring", n ) );
							if( boneIndex != -1 )
							{
								ref var item = ref outputBoneTransforms[ boneIndex ];
								QuaternionF.Slerp( ref item.Rotation, ref rotation, factor, out item.Rotation );
								globalBoneTransforms[ boneIndex ].NeedUpdate = true;
							}
						}
					}
				}

				//little
				{
					var factor = (float)RightHandLittleFingerFlexionFactor.Value;
					var value = (float)RightHandLittleFingerFlexionValue.Value;
					if( factor == 0 && basicItemInHand )
					{
						factor = 1;
						value = 0.8f;
					}

					if( factor > 0 )
					{
						var rotation = QuaternionF.FromRotateByX( MathEx.PI / 2 * -value );
						for( int n = 1; n <= 3; n++ )
						{
							var boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Right", "Pinky", n ) );
							if( boneIndex == -1 )
								boneIndex = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Right", "Little", n ) );
							if( boneIndex != -1 )
							{
								ref var item = ref outputBoneTransforms[ boneIndex ];
								QuaternionF.Slerp( ref item.Rotation, ref rotation, factor, out item.Rotation );
								globalBoneTransforms[ boneIndex ].NeedUpdate = true;
							}
						}
					}
				}
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public bool IsFreePositionToMove( Vector3 position )
		{
			//!!!!check where used

			if( PhysicalBody != null )
			{
				GetVolumeCapsule( out var capsule );
				//make radius smaller
				capsule.Radius *= 0.99;

				var offset = position - TransformV.Position;//capsule.GetCenter();
				var checkCapsule = capsule;
				CapsuleAddOffset( ref checkCapsule, ref offset );

				var scene = ParentScene;
				if( scene != null )
				{
					var contactTestItem = new PhysicsVolumeTestItem( checkCapsule, Vector3.Zero, PhysicsVolumeTestItem.ModeEnum.OneForEach );
					ParentScene.PhysicsVolumeTest( contactTestItem );

					foreach( var item in contactTestItem.Result )
					{
						if( item.Body == PhysicalBody )
							continue;

						return false;
					}
				}
			}

			return true;
		}

		/////////////////////////////////////////

		public ItemInterface[] GetAllItems()
		{
			return GetComponents<ItemInterface>();
		}

		public ItemInterface GetItemByType( ItemTypeInterface type )
		{
			if( type != null )
			{
				foreach( var c in GetComponents<ItemInterface>() )
				{
					var item = c as Item;
					if( item != null )
					{
						if( item.ItemType.Value == type )
							return c;
					}

					var weapon = c as Weapon;
					if( weapon != null )
					{
						if( weapon.WeaponType.Value == type )
							return c;
					}
				}
			}
			return null;
		}

		public ItemInterface GetItemByResourceName( string resourceName )
		{
			foreach( var c in GetComponents<ItemInterface>() )
			{
				var item = c as Item;
				if( item != null )
				{
					var itemType = item.ItemType.Value;
					if( itemType != null )
					{
						var resource = ComponentUtility.GetResourceInstanceByComponent( itemType );
						if( resource != null && resource.Owner.Name == resourceName )
							return c;
					}
				}

				var weapon = c as Weapon;
				if( weapon != null )
				{
					var itemType = weapon.WeaponType.Value;
					if( itemType != null )
					{
						var resource = ComponentUtility.GetResourceInstanceByComponent( itemType );
						if( resource != null && resource.Owner.Name == resourceName )
							return c;
					}
				}

			}
			return null;
		}

		public ItemInterface GetActiveItem()
		{
			//optimized to remove memory allocations

			var node = Components.GetLinkedListReadOnly()?.First;
			while( node != null )
			{
				var component = node.Value;
				if( component.Enabled )
				{
					var item = component as ItemInterface;
					if( item != null )
						return item;
				}
				node = node.Next;
			}

			//foreach( var item in GetAllItems() )
			//{
			//	if( item.Enabled )
			//		return item;
			//}

			return null;
		}

		public Weapon GetActiveWeapon()
		{
			return GetActiveItem() as Weapon;
		}

		public bool ItemCanTake( GameMode gameMode, ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			//allow manage inventory
			if( !TypeCached.AllowManageInventory && EngineApp.IsSimulation )
				return false;

			//check already taken
			if( item2.Parent == this )
				return false;

			//disable taking from another owner
			if( item2.Parent as ObjectInSpace != null )
				return false;

			//InventoryCharacterCanHaveSeveralWeapons, InventoryCharacterCanHaveSeveralWeaponsOfSameType
			var weapon = item2 as Weapon;
			if( weapon != null )
			{
				if( !gameMode.InventoryCharacterCanHaveSeveralWeapons && GetComponent<Weapon>() != null )
					return false;
				if( !gameMode.InventoryCharacterCanHaveSeveralWeaponsOfSameType && GetItemByType( weapon.WeaponType.Value ) != null )
					return false;
			}

			//!!!!need check by distance and do other checks. can be done in GameMode.ItemTakeEvent

			var allowAction = true;
			gameMode.PerformItemCanTakeEvent( this, item, ref allowAction );
			if( !allowAction )
				return false;

			return true;
		}

		/// <summary>
		/// Takes the item. The item will moved to the character and will disabled.
		/// </summary>
		/// <param name="item"></param>
		public bool ItemTake( GameMode gameMode, ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			if( gameMode != null )
			{
				if( !ItemCanTake( gameMode, item ) )
					return false;
			}

			//disable
			item2.Enabled = false;

			//detach
			ObjectInSpaceUtility.Detach( item2 );
			item2.RemoveFromParent( false );

			//Item: combine into one item
			var basicItem = item as Item;
			if( basicItem != null )
			{
				if( basicItem.ItemType.Value.CanCombineIntoOneItem )
				{
					var existsItem = GetItemByType( basicItem.ItemType.Value );
					if( existsItem != null )
					{
						existsItem.ItemCount += basicItem.ItemCount;
						return true;
					}
				}
			}

			var originalScale = item2.TransformV.Scale;

			//attach
			AddComponent( item2 );
			item2.Transform = GetCenteredTransform();//Transform.Value;
			var transformOffset = ObjectInSpaceUtility.Attach( this, item2, TransformOffset.ModeEnum.Elements );
			if( transformOffset != null )
				transformOffset.ScaleOffset = originalScale / GetScaleFactor();

			return true;
		}

		/// <summary>
		/// Drops the item. The item will moved to the scene and will enabled.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="newTransform"></param>
		public bool ItemDrop( GameMode gameMode, ItemInterface item/*, bool calculateTransform, Transform setTransform*/, double amount )
		{
			var item2 = (ObjectInSpace)item;
			var amount2 = amount;

			if( !TypeCached.AllowManageInventory && EngineApp.IsSimulation )
				return false;

			//check can drop
			if( item2.Parent != this )
				return false;

			var allowAction = true;
			gameMode.PerformItemCanDropEvent( this, item, ref allowAction, ref amount2 );
			if( !allowAction )
				return false;

			//Item: combined into one item
			var itemSplit = false;
			{
				var basicItem = item2 as Item;
				if( basicItem != null )
				{
					if( basicItem.ItemCount - amount2 > 0 )
					{
						basicItem.ItemCount -= amount2;
						itemSplit = true;
					}
				}
			}

			var existsCollision = false;
			{
				var meshInSpace = item2 as MeshInSpace;
				if( meshInSpace != null )
				{
					var mesh = meshInSpace.Mesh.Value;
					if( mesh != null && mesh.GetComponent<RigidBody>( "Collision Definition" ) != null )
						existsCollision = true;
				}
			}

			if( !itemSplit )
			{
				//disable
				item2.Enabled = false;

				//detach
				ObjectInSpaceUtility.Detach( item2 );
				item2.RemoveFromParent( false );
			}
			else
			{
				item2 = (ObjectInSpace)item2.Clone();
				( (Item)item2 ).ItemCount = amount2;
			}

			//add to the scene
			ParentScene.AddComponent( item2 );

			//calculate transform
			{
				var scaleFactor = GetScaleFactor();

				//!!!!need calculate better position

				var tr = TransformVisualOverride ?? TransformV;

				if( existsCollision )
				{
					var pos = tr.Position + new Vector3( 0, 0, TypeCached.Height / 2 ) + tr.Rotation * new Vector3( 1.3, 0, 0 );
					item2.Transform = new Transform( pos, tr.Rotation, item2.TransformV.Scale );
				}
				else
				{
					item2.Transform = new Transform( tr.Position + new Vector3( 0, 0, 0.1 * scaleFactor /*- TypeCached.PositionToGroundHeight * scaleFactor*/ ), tr.Rotation, item2.TransformV.Scale );
				}
			}
			//else if( setTransform != null )
			//	item2.Transform = setTransform;

			//enable
			item2.Enabled = true;

			return true;
		}

		public void ItemDropClient( ItemInterface item, double amount )
		{
			var component = item as Component;
			if( component != null )
			{
				var writer = BeginNetworkMessageToServer( "ItemDrop" );
				if( writer != null )
				{
					writer.WriteVariableUInt64( (ulong)component.NetworkID );
					writer.Write( amount );//writer.WriteVariableInt32( amount );
					EndNetworkMessage();
				}
			}
		}

		/// <summary>
		/// Activates the item. The item will enabled.
		/// </summary>
		/// <param name="item"></param>
		public bool ItemActivate( GameMode gameMode, ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			if( !TypeCached.AllowManageInventory && EngineApp.IsSimulation )
				return false;

			//Item
			var basicItem = item as Item;
			if( basicItem != null && !basicItem.ItemType.Value.CanActivate )
				return false;

			if( gameMode != null )
			{
				var allowAction = true;
				gameMode.PerformItemCanActivateEvent( this, item, ref allowAction );
				if( !allowAction )
					return false;
			}

			//deactivate other before activate new
			{
				foreach( var item3 in GetAllItems() )
					ItemDeactivate( gameMode, item3 );

				//if can't deactivate other, then can't activate new
				if( GetActiveItem() != null )
					return false;
			}

			item2.Enabled = true;

			return true;
		}

		/// <summary>
		/// Deactivates the item. The item will disabled.
		/// </summary>
		/// <param name="item"></param>
		public bool ItemDeactivate( GameMode gameMode, ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			if( !TypeCached.AllowManageInventory && EngineApp.IsSimulation )
				return false;

			if( !item2.Enabled )
				return true;

			if( gameMode != null )
			{
				var allowAction = true;
				gameMode.PerformItemCanDeactivateEvent( this, item, ref allowAction );
				if( !allowAction )
					return false;
			}

			item2.Enabled = false;

			return true;
		}

		public void DeactivateAllItems( GameMode gameMode )
		{
			foreach( var item in GetAllItems() )
				ItemDeactivate( gameMode, item );
		}

		public void ItemTakeAndActivateClient( ItemInterface item, bool activate )
		{
			var item2 = (ObjectInSpace)item;

			var writer = BeginNetworkMessageToServer( "ItemTakeAndActivate" );
			if( writer != null )
			{
				writer.WriteVariableUInt64( (ulong)item2.NetworkID );
				writer.Write( activate );
				EndNetworkMessage();
			}
		}

		public void ItemActivateClient( ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			var writer = BeginNetworkMessageToServer( "ItemActivate" );
			if( writer != null )
			{
				writer.WriteVariableUInt64( (ulong)item2.NetworkID );
				EndNetworkMessage();
			}
		}

		public void ItemDeactivateClient( ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			var writer = BeginNetworkMessageToServer( "ItemDeactivate" );
			if( writer != null )
			{
				writer.WriteVariableUInt64( (ulong)item2.NetworkID );
				EndNetworkMessage();
			}
		}

		/// <summary>
		/// Returns first item of the character.
		/// </summary>
		/// <returns></returns>
		public ItemInterface ItemGetFirst()
		{
			foreach( var c in Components )
				if( c is ItemInterface item )
					return item;
			return null;
		}

		void RotateWeaponIfCollided( Weapon weapon, TransformOffset offset, Degree rotationOffsetVertical )
		{
			var mesh = weapon.Mesh.Value;
			if( mesh == null || mesh.Result == null )
				return;

			////mesh length + offset
			var meshLength = mesh.Result.SpaceBounds.BoundingBox.Maximum.X;// * 1.5;// + 0.1;
			if( meshLength <= 0 )
				return;

			var scene = ParentScene;//FindParent<Scene>();
			if( scene == null )
				return;

			//!!!!check volume
			var weaponTransform = weapon.TransformV;
			var weaponDirection = weaponTransform.Rotation.GetForward();
			var ray = new Ray( weaponTransform.Position - weaponDirection * meshLength * 0.5, weaponDirection * meshLength * 1.5 );
			//var ray = new Ray( weaponTransform.Position, weaponTransform.Rotation.GetForward() * meshLength );

			//!!!!volume cast sweep

			var item = new PhysicsRayTestItem( ray, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );
			scene.PhysicsRayTest( item );


			Degree? newAngle = null;

			for( int n = 0; n < item.Result.Length; n++ )//foreach( var resultItem in item.Result )
			{
				ref var resultItem = ref item.Result[ n ];

				if( PhysicalBody != null && resultItem.Body == PhysicalBody )
					continue;
				//what else to skip

				var length = ray.Direction.Length() * resultItem.DistanceScale;

				var factor = length / ( meshLength * 1.5 );
				if( factor < 1 )
				{
					var angle = 90 * ( 1.0 - factor );

					//!!!!

					newAngle = rotationOffsetVertical - angle;
					if( newAngle.Value < -90 )
						newAngle = -90;

					//if( rotationOffsetVertical > 0 )
					//{
					//}
					//else
					//{
					//}

					break;
				}
			}

			if( newAngle != null )
				offset.RotationOffset = Quaternion.FromRotateByY( newAngle.Value.InRadians() );
		}

		void UpdateActiveItemTransformOffset()
		{
			var item = GetActiveItem() as MeshInSpace;
			if( item != null )
			{
				var firstPersonCamera = IsControlledByPlayerAndFirstPersonCameraEnabled();

				var transformOffset = item.GetComponent<TransformOffset>();
				if( transformOffset != null )
				{
					//disable properties synchronization
					if( NetworkIsServer )
					{
						if( !transformOffset.NetworkIsDisabledPropertySynchronization( "PositionOffset" ) )
						{
							transformOffset.NetworkDisablePropertySynchronization( "PositionOffset" );
							transformOffset.NetworkDisablePropertySynchronization( "RotationOffset" );

							//!!!!motion blur factor?

							//!!!!что еще? weapon Transform?
						}
					}

					//!!!!у пушки надо уменьшать scale, если персонаж увеличен. где еще учитывать

					var weapon = item as Weapon;

					if( weapon != null && weapon.TypeCached.WayToUse.Value == WeaponType.WayToUseEnum.OneHandedMelee )
					{
						var controller = GetAnimationController();
						if( controller != null )
						{
							var globalBoneTransforms = controller.GetBoneGlobalTransforms();
							if( globalBoneTransforms != null )
							{
								var boneRightHandIndex = controller.GetBoneIndex( TypeCached.RightHandBone );
								var boneRightHandMiddle1Index = controller.GetBoneIndex( TypeCached.GetFingerBoneName( "Right", "Middle", 1 ) );

								if( boneRightHandIndex != -1 && boneRightHandMiddle1Index != -1 )
								{
									var boneRightHand = globalBoneTransforms[ boneRightHandIndex ];//.Position;
									var boneRightHandMiddle1 = globalBoneTransforms[ boneRightHandMiddle1Index ];//.Position;

									var localCenter = Vector3F.Lerp( boneRightHand.Position, boneRightHandMiddle1.Position, 0.75f );
									//var center = TransformV * localCenter;

									//!!!!offset тоже

									transformOffset.PositionOffset = localCenter.ToVector3();// boneItem.Position.ToVector3();

									//!!!!
									transformOffset.RotationOffset = boneRightHand.Rotation.ToQuaternion() * new Angles( -90, 0, 0 ).ToQuaternion();

									//transformOffset.RotationOffset = boneRightHand.Rotation.ToQuaternion() * new Angles( -90, 0, -90 ).ToQuaternion();


									////transformOffset.RotationOffset = new Angles( -90, 0, -90 ).ToQuaternion() * boneRightHand.Rotation.ToQuaternion();
									////transformOffset.RotationOffset = Quaternion.FromRotateByX( -Math.PI / 2 ) * boneRightHand.Rotation.ToQuaternion();

									////transformOffset.PositionOffset = boneItem.Position.ToVector3();
									////transformOffset.RotationOffset = Quaternion.FromRotateByY( -Math.PI / 2 ) * boneItem.Rotation.ToQuaternion();

									////transformOffset.PositionOffset = boneItem.Position.ToVector3();
									////transformOffset.RotationOffset = boneItem.Rotation.ToQuaternion();

								}
							}
						}
					}
					else
					{
						var typeArmsCenter = Vector3.Zero;
						if( weapon != null )
							typeArmsCenter = weapon.TypeCached.GetArmsCenter();

						Vector3 aimingArmsCenter;
						if( weapon != null )
						{
							if( firstPersonCamera )
								aimingArmsCenter = TypeCached.RifleAimingArmsCenterFirstPerson.Value;
							else
								aimingArmsCenter = TypeCached.RifleAimingArmsCenter.Value;
						}
						else
							aimingArmsCenter = TypeCached.ItemHoldingPosition;

						var tr = TransformV;
						var trInverse = tr.ToMatrix4().GetInverse();

						var worldVectorFrom = tr.ToMatrix4() * new Vector3( 0, 0, aimingArmsCenter.Z );

						Vector3 worldVectorTo;
						if( firstPersonCamera )
						{
							//in first person mode use not real direction to have constant rotation
							worldVectorTo = worldVectorFrom + Quaternion.FromDirectionZAxisUp( CurrentTurnToDirection.GetVector() ) * new Vector3( 1000, 0, 0 );
						}
						else if( CurrentLookToPosition.HasValue )
							worldVectorTo = CurrentLookToPosition.Value;
						else
							worldVectorTo = worldVectorFrom + tr.Rotation * new Vector3( 1000, 0, 0 );

						var worldRotationFromTo = Quaternion.FromDirectionZAxisUp( worldVectorTo - worldVectorFrom );

						//calculate breath offset
						var amplitude = 0.003;
						if( weapon == null )
							amplitude += 0.02 * GetLinearVelocity().ToVector2().Length();
						var breathOffset = new Vector3( Math.Sin( Time.Current * 2 ) * amplitude * 1.7, 0, Math.Cos( Time.Current * 2 ) * amplitude );

						//recoil offset
						var addRecoilOffset = Vector3.Zero;
						if( weapon != null )
						{
							var maxRecoil = 0.0;

							for( int mode = 1; mode <= 2; mode++ )
							{
								if( weapon.IsFiring( mode ) )
								{
									var currentTime = weapon.GetFiringCurrentTime( mode );

									var firingTotalTime = mode == 1 ? weapon.TypeCached.Mode1FiringTotalTime : weapon.TypeCached.Mode2FiringTotalTime;
									var recoilOffset = mode == 1 ? weapon.TypeCached.Mode1MaxRecoilOffset : weapon.TypeCached.Mode2MaxRecoilOffset;
									var maxRecoilOffsetTime = mode == 1 ? weapon.TypeCached.Mode1MaxRecoilOffsetTime : weapon.TypeCached.Mode2MaxRecoilOffsetTime;

									var endTime = Math.Min( firingTotalTime, maxRecoilOffsetTime * 10 );

									var curve = new CurveCubicSpline1();
									curve.AddPoint( 0, 0 );
									curve.AddPoint( maxRecoilOffsetTime, recoilOffset );
									curve.AddPoint( endTime, 0 );

									var recoil = curve.CalculateValueByTime( currentTime );
									if( recoil > maxRecoil )
										maxRecoil = recoil;
								}
							}

							addRecoilOffset.X -= maxRecoil;
						}

						var worldItemPosition = worldVectorFrom + worldRotationFromTo * ( new Vector3( aimingArmsCenter.X, aimingArmsCenter.Y, 0 ) - typeArmsCenter + breathOffset + addRecoilOffset ) + GetSmoothCameraOffset();

						transformOffset.PositionOffset = trInverse * worldItemPosition;
						transformOffset.RotationOffset = tr.Rotation.GetInverse() * Quaternion.FromDirectionZAxisUp( worldVectorTo - worldItemPosition );

						//save position offset to AnyData. it is used to calculate upper body rotation
						transformOffset.AnyData = trInverse * worldItemPosition;

						////save position offset without recoil and breath offset to AnyData. it is used to calculate upper body rotation
						//transformOffset.AnyData = trInverse * ( worldVectorFrom + worldRotationFromTo * ( new Vector3( aimingArmsCenter.X, aimingArmsCenter.Y, 0 ) - typeArmsCenter ) + GetSmoothCameraOffset() );


						//rotate weapon in special situations
						if( weapon != null )
							RotateWeaponIfCollided( weapon, transformOffset, new Radian( CurrentTurnToDirection.Vertical ).InDegrees() );
					}


					//!!!!by idea it must be always enabled
					//reset motion blur settings
					if( firstPersonCamera )
						item.MotionBlurFactor = 0;
					else
						item.MotionBlurFactor = 1;
				}
			}
		}


		////var item = GetActiveItem() as MeshInSpace;
		////if( item != null )
		////{
		////	var transformOffset = item.GetComponent<TransformOffset>();
		////	if( transformOffset != null )
		////	{
		////		var weapon = item as Weapon;


		////		//!!!!у пушки надо уменьшать scale, если персонаж увеличен. где еще учитывать



		////		//var armsCenterWhenAimingRifle = TypeCached.ArmsCenterWhenAimingRifle.Value;
		////		//var localTargetFromPosition = new Vector3( 0, armsCenterWhenAimingRifle.Y, armsCenterWhenAimingRifle.Z );

		////		//Vector3 localTargetToPosition;
		////		//if( CurrentLookToPosition.HasValue )
		////		//	localTargetToPosition = tr.ToMatrix4().GetInverse() * CurrentLookToPosition.Value;
		////		//else
		////		//	localTargetToPosition = new Vector3( 0, armsCenterWhenAimingRifle.Y, 0 ) + new Vector3( 100, 0, 0 );

		////		//var localDirection = ( localTargetToPosition - localTargetFromPosition ).GetNormalize();
		////		//transformOffset.RotationOffset = Quaternion.FromDirectionZAxisUp( localDirection );

		////		//var sphericalDir = SphericalDirection.FromVector( localDirection );

		////		////!!!!
		////		//var breathOffset = new Vector3( Math.Sin( Time.Current * 1.5 ) * 0.005, 0, Math.Cos( Time.Current * 1.5 ) * 0.003 );

		////		//transformOffset.PositionOffset = localTargetFromPosition + Quaternion.FromRotateByY( sphericalDir.Vertical ) * ( new Vector3( armsCenterWhenAimingRifle.X, 0, 0 ) - weaponTypeArmsCenter + breathOffset ) + GetSmoothCameraOffset();




		////		//var worldTargetFromPosition = tr * localTargetFromPosition;

		////		//Vector3 worldTargetToPosition;
		////		//if( WeaponTargetToPosition.HasValue )
		////		//	worldTargetToPosition = WeaponTargetToPosition.Value;
		////		//else
		////		//{
		////		//	worldTargetToPosition = worldTargetFromPosition + tr.Rotation * new Vector3( 100, 0, 0 );
		////		//	//worldTargetToPosition = GetCenteredPosition() + TransformV.Rotation * new Vector3( 100, 0, 0 );
		////		//}

		////		//var worldDirection = ( worldTargetToPosition - worldTargetFromPosition ).Normalize();



		////		//zzzz;//мы его как раз считаем
		////		//var weaponPosition = weapon.TransformV.Position;
		////		//var lookDirection = worldTargetToPosition/*WeaponTargetToPosition.Value*/ - weaponPosition;

		////		//var rotationOffset = Quaternion.FromDirectionZAxisUp( lookDirection );
		////		////var centeredOffset = new Vector3( 0, 0, TypeCached.Height * 0.5 );


		////		////!!!!
		////		//var breathOffset = new Vector3( Math.Sin( Time.Current * 1.5 ) * 0.005, 0, Math.Cos( Time.Current * 1.5 ) * 0.003 );


		////		////!!!!
		////		//transformOffset.PositionOffset = armsCenterWhenAimingRifle - weaponTypeArmsCenter + breathOffset;

		////		////offset.PositionOffset = TypeCached.RiflePosition.Value;
		////		////offset.PositionOffset = centeredOffset + weapon.TypeCached.PositionOffsetWhenAttached;

		////		//transformOffset.RotationOffset = TransformV.Rotation.GetInverse() * rotationOffset;




		////		//!!!!need?
		////		//RotateWeaponIfCollided( weapon, offset, rotationOffset.InDegrees() );

		////		//reset motion blur settings
		////		item.MotionBlurFactor = 1;


		////		//var rotationOffet = CurrentTurnToDirection.Vertical + weapon.TypeCached.RotationOffsetWhenAttached.Value.InRadians();

		////		////!!!!
		////		//var centeredOffset = new Vector3( 0, 0, TypeCached.Height * 0.5 );

		////		//offset.PositionOffset = centeredOffset + weapon.TypeCached.PositionOffsetWhenAttached;
		////		//offset.RotationOffset = Quaternion.FromRotateByY( rotationOffet );

		////		//RotateWeaponIfCollided( weapon, offset, rotationOffet.InDegrees() );

		////		////reset motion blur settings
		////		//weapon.MotionBlurFactor = 1;
		////	}
		////}
		//}

		//}

		public void UpdateDataForFirstOrThirdPersonCamera( bool firstPersonCamera )
		{
			UpdateActiveItemTransformOffset();
		}

		//public void UpdateDataForFirstPersonCamera( GameMode gameMode, Viewport viewport )
		//{

		//var item = GetActiveItem() as MeshInSpace;
		//if( item != null )
		//{
		//	var transformOffset = item.GetComponent<TransformOffset>();
		//	if( transformOffset != null )
		//	{
		//		var weapon = item as Weapon;

		//		var typeArmsCenter = Vector3.Zero;
		//		if( weapon != null )
		//			typeArmsCenter = weapon.TypeCached.GetArmsCenter();

		//		Vector3 aimingArmsCenter;
		//		if( weapon != null )
		//			aimingArmsCenter = TypeCached.RifleAimingArmsCenter.Value;
		//		else
		//			aimingArmsCenter = TypeCached.ItemHoldingPosition;


		//		var tr = TransformV;
		//		var trInverse = tr.ToMatrix4().GetInverse();

		//		var worldVectorFrom = tr.ToMatrix4() * new Vector3( 0, 0, aimingArmsCenter.Z );

		//		Vector3 worldVectorTo;
		//		if( CurrentLookToPosition.HasValue )
		//			worldVectorTo = CurrentLookToPosition.Value;
		//		else
		//			worldVectorTo = worldVectorFrom + tr.Rotation * new Vector3( 100, 0, 0 );

		//		var worldRotationFromTo = Quaternion.FromDirectionZAxisUp( worldVectorTo - worldVectorFrom );

		//		//!!!!
		//		var breathOffset = new Vector3( Math.Sin( Time.Current * 1.5 ) * 0.005, 0, Math.Cos( Time.Current * 1.5 ) * 0.003 );

		//		var worldItemPosition = worldVectorFrom + worldRotationFromTo * ( new Vector3( aimingArmsCenter.X, aimingArmsCenter.Y, 0 ) - typeArmsCenter + breathOffset ) + GetSmoothCameraOffset();

		//		transformOffset.PositionOffset = trInverse * worldItemPosition;
		//		transformOffset.RotationOffset = tr.Rotation.GetInverse() * Quaternion.FromDirectionZAxisUp( worldVectorTo - worldItemPosition );



		//		//var positionOffset = weapon.TypeCached.PositionOffsetWhenAttachedFirstPerson.Value;
		//		////!!!!было
		//		////var rotationOffset = weapon.TypeCached.RotationOffsetWhenAttachedFirstPerson.Value;

		//		////idle, walking
		//		//{
		//		//	var maxOffset = 0.005;

		//		//	//!!!!impl when moving
		//		//	//if( IsOnGround() )
		//		//	//{
		//		//	//	var maxOffset2 = 0.01;
		//		//	//	var factor = MathEx.Saturate( GetLinearVelocity().Length() * 0.5 );
		//		//	//	maxOffset = MathEx.Lerp( maxOffset, maxOffset2, factor );
		//		//	//}

		//		//	positionOffset.Y += Math.Sin( Time.Current * 0.25 ) * maxOffset;
		//		//	positionOffset.Z += Math.Sin( Time.Current * 0.43 ) * maxOffset;
		//		//}

		//		////add recoil offset
		//		//{
		//		//	var maxRecoil = 0.0;

		//		//	for( int mode = 1; mode <= 2; mode++ )
		//		//	{
		//		//		if( weapon.IsFiring( mode ) )
		//		//		{
		//		//			var currentTime = weapon.GetFiringCurrentTime( mode );

		//		//			var firingTotalTime = mode == 1 ? weapon.TypeCached.Mode1FiringTotalTime : weapon.TypeCached.Mode2FiringTotalTime;
		//		//			var recoilOffset = mode == 1 ? weapon.TypeCached.Mode1MaxRecoilOffset : weapon.TypeCached.Mode2MaxRecoilOffset;
		//		//			var maxRecoilOffsetTime = mode == 1 ? weapon.TypeCached.Mode1MaxRecoilOffsetTime : weapon.TypeCached.Mode2MaxRecoilOffsetTime;

		//		//			var endTime = Math.Min( firingTotalTime, maxRecoilOffsetTime * 10 );

		//		//			var curve = new CurveCubicSpline1();//new CurveLine1();
		//		//			curve.AddPoint( 0, 0 );
		//		//			curve.AddPoint( maxRecoilOffsetTime, recoilOffset );
		//		//			curve.AddPoint( endTime, 0 );

		//		//			var recoil = curve.CalculateValueByTime( currentTime );
		//		//			if( recoil > maxRecoil )
		//		//				maxRecoil = recoil;
		//		//		}
		//		//	}

		//		//	positionOffset.X -= maxRecoil;
		//		//}

		//		//var cameraSettings = viewport.CameraSettings;
		//		//var worldTransform = new Transform( cameraSettings.Position + cameraSettings.Rotation * positionOffset, cameraSettings.Rotation );

		//		//var localToCameraTransform = TransformV.ToMatrix4().GetInverse() * worldTransform.ToMatrix4();
		//		//localToCameraTransform.Decompose( out var position, out Quaternion rotation, out var scale );

		//		//offset.PositionOffset = position;

		//		////{
		//		////	var weaponPosition = weapon.TransformV.Position;
		//		////	var lookDirection = WeaponTargetToPosition.Value - weaponPosition;
		//		////	var rotationOffset = Quaternion.FromDirectionZAxisUp( lookDirection );
		//		////	//var centeredOffset = new Vector3( 0, 0, TypeCached.Height * 0.5 );
		//		////	//offset.PositionOffset = centeredOffset + weapon.TypeCached.PositionOffsetWhenAttached;

		//		////	offset.RotationOffset = TransformV.Rotation.GetInverse() * rotationOffset;
		//		////}
		//		//offset.RotationOffset = rotation;




		//		//var positionOffset = weapon.TypeCached.PositionOffsetWhenAttachedFirstPerson.Value;
		//		////!!!!было
		//		////var rotationOffset = weapon.TypeCached.RotationOffsetWhenAttachedFirstPerson.Value;

		//		////idle, walking
		//		//{
		//		//	var maxOffset = 0.005;

		//		//	//!!!!impl when moving
		//		//	//if( IsOnGround() )
		//		//	//{
		//		//	//	var maxOffset2 = 0.01;
		//		//	//	var factor = MathEx.Saturate( GetLinearVelocity().Length() * 0.5 );
		//		//	//	maxOffset = MathEx.Lerp( maxOffset, maxOffset2, factor );
		//		//	//}

		//		//	positionOffset.Y += Math.Sin( Time.Current * 0.25 ) * maxOffset;
		//		//	positionOffset.Z += Math.Sin( Time.Current * 0.43 ) * maxOffset;
		//		//}

		//		////add recoil offset
		//		//{
		//		//	var maxRecoil = 0.0;

		//		//	for( int mode = 1; mode <= 2; mode++ )
		//		//	{
		//		//		if( weapon.IsFiring( mode ) )
		//		//		{
		//		//			var currentTime = weapon.GetFiringCurrentTime( mode );

		//		//			var firingTotalTime = mode == 1 ? weapon.TypeCached.Mode1FiringTotalTime : weapon.TypeCached.Mode2FiringTotalTime;
		//		//			var recoilOffset = mode == 1 ? weapon.TypeCached.Mode1MaxRecoilOffset : weapon.TypeCached.Mode2MaxRecoilOffset;
		//		//			var maxRecoilOffsetTime = mode == 1 ? weapon.TypeCached.Mode1MaxRecoilOffsetTime : weapon.TypeCached.Mode2MaxRecoilOffsetTime;

		//		//			var endTime = Math.Min( firingTotalTime, maxRecoilOffsetTime * 10 );

		//		//			var curve = new CurveCubicSpline1();//new CurveLine1();
		//		//			curve.AddPoint( 0, 0 );
		//		//			curve.AddPoint( maxRecoilOffsetTime, recoilOffset );
		//		//			curve.AddPoint( endTime, 0 );

		//		//			var recoil = curve.CalculateValueByTime( currentTime );
		//		//			if( recoil > maxRecoil )
		//		//				maxRecoil = recoil;
		//		//		}
		//		//	}

		//		//	positionOffset.X -= maxRecoil;
		//		//}

		//		//var cameraSettings = viewport.CameraSettings;
		//		//var worldTransform = new Transform( cameraSettings.Position + cameraSettings.Rotation * positionOffset, cameraSettings.Rotation );

		//		//var localToCameraTransform = TransformV.ToMatrix4().GetInverse() * worldTransform.ToMatrix4();
		//		//localToCameraTransform.Decompose( out var position, out Quaternion rotation, out var scale );

		//		//offset.PositionOffset = position;

		//		////{
		//		////	var weaponPosition = weapon.TransformV.Position;
		//		////	var lookDirection = WeaponTargetToPosition.Value - weaponPosition;
		//		////	var rotationOffset = Quaternion.FromDirectionZAxisUp( lookDirection );
		//		////	//var centeredOffset = new Vector3( 0, 0, TypeCached.Height * 0.5 );
		//		////	//offset.PositionOffset = centeredOffset + weapon.TypeCached.PositionOffsetWhenAttached;

		//		////	offset.RotationOffset = TransformV.Rotation.GetInverse() * rotationOffset;
		//		////}
		//		//offset.RotationOffset = rotation;








		//		//!!!!надо
		//		//!!!!надо
		//		//!!!!надо
		//		//RotateWeaponIfCollided( weapon, offset, new Radian( CurrentTurnToDirection.Vertical ).InDegrees() );



		//		////RotateWeaponIfCollided( weapon, offset, new Radian( CurrentTurnToDirection.Vertical ).InDegrees() + rotationOffset );

		//		//update bounds
		//		item.SpaceBoundsUpdate();

		//		//disable motion blur
		//		item.MotionBlurFactor = 0;

		//		//inform the character about first person update to disable default behaviour which doing in UpdateEnabledItemTransform()
		//		disableUpdateAttachedWeaponRemainingTime = 0.25;
		//	}
		//}

		//}


		public delegate void SoundPlayBeforeDelegate( Character sender, ref Sound sound, ref bool handled );
		public event SoundPlayBeforeDelegate SoundPlayBefore;

		public virtual void SoundPlay( Sound sound, double priority = 0.5 )
		{
			var handled = false;
			SoundPlayBefore?.Invoke( this, ref sound, ref handled );
			if( handled )
				return;

			ParentScene?.SoundPlay( sound, ( TransformVisualOverride ?? TransformV ).Position, priority: priority );
		}

		public delegate void StartPlayOneAnimationBeforeDelegate( Character sender, ref Animation animation, ref double speed, ref bool freezeOnEnd, ref bool handled );
		public event StartPlayOneAnimationBeforeDelegate StartPlayOneAnimationBefore;

		public virtual void StartPlayOneAnimation( Animation animation, double speed = 1.0, bool freezeOnEnd = false )
		{
			var handled = false;
			StartPlayOneAnimationBefore?.Invoke( this, ref animation, ref speed, ref freezeOnEnd, ref handled );
			if( handled )
				return;

			playOneAnimation = animation;
			playOneAnimationSpeed = speed;
			if( playOneAnimation != null && playOneAnimationSpeed == 0 )
				playOneAnimationSpeed = 0.000001;
			playOneAnimationFreezeOnEnd = freezeOnEnd;

			if( playOneAnimation != null )
			{
				if( freezeOnEnd )
					playOneAnimationRemainingTime = 1000000000.0;
				else
					playOneAnimationRemainingTime = playOneAnimation.Length / playOneAnimationSpeed;

				var controller = GetAnimationController();
				if( controller != null && playOneAnimationRemainingTime > controller.InterpolationTime.Value )
					playOneAnimationRemainingTime -= controller.InterpolationTime.Value;
			}
			else
				playOneAnimationRemainingTime = 0;
		}

		[Browsable( false )]
		public Animation PlayOneAnimation
		{
			get { return playOneAnimation; }
		}

		[Browsable( false )]
		public double PlayOneAnimationSpeed
		{
			get { return playOneAnimationSpeed; }
		}

		[Browsable( false )]
		public bool PlayOneAnimationFreezeOnEnd
		{
			get { return playOneAnimationFreezeOnEnd; }
		}

		[Browsable( false )]
		public double PlayOneAnimationRemainingTime
		{
			get { return playOneAnimationRemainingTime; }
		}

		public void StartPlayOneAnimationAdditional( MeshInSpaceAnimationController.AnimationStateClass.AnimationItem animationItem )
		{
			playOneAnimationAdditional = animationItem;
			if( playOneAnimationAdditional != null && playOneAnimationAdditional.Speed == 0 )
				playOneAnimationAdditional.Speed = 0.000001;

			if( playOneAnimationAdditional != null )
			{
				if( playOneAnimationAdditional.FreezeOnEnd )
					playOneAnimationAdditionalRemainingTime = 1000000000.0;
				else
					playOneAnimationAdditionalRemainingTime = playOneAnimationAdditional.Animation.Length / playOneAnimationAdditional.Speed;

				var controller = GetAnimationController();
				if( controller != null && playOneAnimationAdditionalRemainingTime > controller.InterpolationTime.Value )
					playOneAnimationAdditionalRemainingTime -= controller.InterpolationTime.Value;
			}
			else
				playOneAnimationAdditionalRemainingTime = 0;
		}

		[Browsable( false )]
		public MeshInSpaceAnimationController.AnimationStateClass.AnimationItem PlayOneAnimationAdditional
		{
			get { return playOneAnimationAdditional; }
		}

		[Browsable( false )]
		public double PlayOneAnimationAdditionalRemainingTime
		{
			get { return playOneAnimationAdditionalRemainingTime; }
		}

		public void WeaponFiringBegin( Weapon weapon, int mode )
		{
			//One-handed melee weapon. play character attack animation
			if( weapon.TypeCached.WayToUse.Value == WeaponType.WayToUseEnum.OneHandedMelee )
			{
				var stopMovementWhenFiring = mode == 1 ? weapon.TypeCached.Mode1StopMovementWhenFiring : weapon.TypeCached.Mode2StopMovementWhenFiring;

				var playIdleAnimation = false;

				if( IsOnGround() )// IsOnGroundWithLatency() )
				{
					var localVelocityNoScale = TransformV.Rotation.GetInverse() * GetLinearVelocity() / GetScaleFactor();
					var localSpeedNoScale = localVelocityNoScale.X;

					var walking = Math.Abs( localSpeedNoScale ) > TypeCached.WalkForwardMaxSpeed * 0.2 || Math.Abs( localVelocityNoScale.Y ) > TypeCached.WalkForwardMaxSpeed * 0.2;

					if( !walking )
						playIdleAnimation = true;
				}

				MeshInSpaceAnimationController.AnimationStateClass.AnimationItem animationItem;

				if( playIdleAnimation )
				{
					//play full body
					animationItem = new MeshInSpaceAnimationController.AnimationStateClass.AnimationItem() { Animation = TypeCached.OneHandedMeleeWeaponAttackAnimation, Speed = TypeCached.OneHandedMeleeWeaponAttackAnimationSpeed, AnimationItemTag = 3, ReplaceMode = true };
				}
				else
				{
					//play only upper part body
					animationItem = new MeshInSpaceAnimationController.AnimationStateClass.AnimationItem() { Animation = TypeCached.OneHandedMeleeWeaponAttackAnimation, Speed = TypeCached.OneHandedMeleeWeaponAttackAnimationSpeed, AnimationItemTag = 3, AffectBonesWithChildren = new string[] { TypeCached.UpperPartBone }, ReplaceMode = true };
				}

				if( animationItem.Animation != null )
				{
					//disable control
					if( playIdleAnimation || stopMovementWhenFiring )
					{
						var time = animationItem.Animation.Length / animationItem.Speed;
						disableControlRemainingTime = Math.Max( disableControlRemainingTime, (float)time );
					}

					StartPlayOneAnimationAdditional( animationItem );
				}
			}
		}


		//!!!!резко обновляет модель
		//[MethodImpl( (MethodImplOptions)512 )]
		//void FixTurnToDirectionWhenItInvalidForAnimation()
		//{
		//	var allowLookToBack = AllowLookToBackWhenNoActiveItem && GetActiveItem() == null;

		//	//turn direction when it have too big angle between look to
		//	if( CurrentLookToPosition.HasValue && !allowLookToBack )
		//	{
		//		var thresholdAngle = new DegreeF( 130.0f ).InRadians();

		//		var tr = TransformV;

		//		var currentLookToDirection = ( CurrentLookToPosition.Value - tr.Position ).ToVector2F();
		//		if( currentLookToDirection != Vector2F.Zero )
		//		{
		//			currentLookToDirection.Normalize();

		//			var currentDirection = tr.Rotation.GetForward().ToVector2F();

		//			var angle = MathAlgorithms.GetVectorsAngle( ref currentLookToDirection, ref currentDirection );
		//			if( angle > thresholdAngle )
		//			{
		//				var lookToAngle = MathEx.Atan2( currentLookToDirection.Y, currentLookToDirection.X );
		//				var currentAngle = MathEx.Atan2( currentDirection.Y, currentDirection.X );

		//				var leftTurn = Math.Sin( lookToAngle - currentAngle ) > 0;

		//				if( leftTurn )
		//					currentTurnToDirection.Horizontal = currentAngle - thresholdAngle;
		//				else
		//					currentTurnToDirection.Horizontal = currentAngle + thresholdAngle;
		//				UpdateRotation();
		//			}
		//		}
		//	}
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		void TickTurnToDirection()
		{
			var allowLookToBack = AllowLookToBackWhenNoActiveItem && GetActiveItem() == null;

			//turn direction when it have too big angle between look to
			if( !RequiredTurnToDirection.HasValue && RequiredLookToPosition.HasValue && !allowLookToBack )
			{
				var thresholdAngle = new DegreeF( 91.0f ).InRadians();

				var tr = TransformV;

				var requiredDiff = ( RequiredLookToPosition.Value - tr.Position ).ToVector2F();
				if( requiredDiff != Vector2F.Zero )
				{
					requiredDiff.Normalize();

					var currentDirection = tr.Rotation.GetForward().ToVector2F();

					var angle = MathAlgorithms.GetVectorsAngle( ref requiredDiff, ref currentDirection );
					if( angle > thresholdAngle )
					{
						var lookToAngle = MathEx.Atan2( requiredDiff.Y, requiredDiff.X );
						var currentAngle = MathEx.Atan2( currentDirection.Y, currentDirection.X );

						var leftTurn = Math.Sin( lookToAngle - currentAngle ) > 0;

						if( leftTurn )
							RequiredTurnToDirection = new SphericalDirectionF( currentAngle - thresholdAngle, 0 );
						else
							RequiredTurnToDirection = new SphericalDirectionF( currentAngle + thresholdAngle, 0 );
					}
				}
			}

			//simulate RequiredTurnToDirection
			if( RequiredTurnToDirection.HasValue && disableControlRemainingTime == 0 )// && IsOnGround() )
			{
				var requiredTurnToDirection = RequiredTurnToDirection.Value.Horizontal;

				if( currentTurnToDirection.Horizontal != requiredTurnToDirection )
				{
					if( RequiredLookToPosition.HasValue && !allowLookToBack )
					{
						//change rotation when look to is enabled. character can't turn around in back to look to direction

						var diff = RequiredLookToPosition.Value - TransformV.Position;
						var lookToDirection = Math.Atan2( diff.Y, diff.X );

						var requiredAngleDiff = MathEx.RadianNormalize180( requiredTurnToDirection - lookToDirection );
						var currentAngleDiff = MathEx.RadianNormalize180( currentTurnToDirection.Horizontal - lookToDirection );
						var leftTurn = requiredAngleDiff > currentAngleDiff;

						var step = (float)(double)( TurningSpeedOverride ?? TypeCached.TurningSpeed.Value ).InRadians() * Time.SimulationDelta;

						var current = currentTurnToDirection.Horizontal;
						current += leftTurn ? step : -step;

						var newCurrentAngleDiff = MathEx.RadianNormalize180( current - lookToDirection );
						var newLeftTurn = requiredAngleDiff > newCurrentAngleDiff;

						if( newLeftTurn != leftTurn )
						{
							current = requiredTurnToDirection;

							//reset
							RequiredTurnToDirection = null;
						}

						currentTurnToDirection.Horizontal = current;

						UpdateRotation();
					}
					else
					{
						//change rotation when look to is disabled

						var angle = requiredTurnToDirection - currentTurnToDirection.Horizontal;
						var leftTurn = Math.Sin( angle ) > 0;

						var step = (float)(double)( TurningSpeedOverride ?? TypeCached.TurningSpeed.Value ).InRadians() * Time.SimulationDelta;

						var current = currentTurnToDirection.Horizontal;
						current += leftTurn ? step : -step;

						var newAngle = requiredTurnToDirection - current;
						var newLeftTurn = Math.Sin( newAngle ) > 0;

						if( newLeftTurn != leftTurn )
						{
							current = requiredTurnToDirection;

							//reset
							RequiredTurnToDirection = null;
						}

						currentTurnToDirection.Horizontal = current;

						UpdateRotation();
					}
				}
				else
				{
					//reset
					RequiredTurnToDirection = null;
				}
			}
		}

		void TickLookToPosition()
		{
			if( RequiredLookToPosition.HasValue && disableControlRemainingTime == 0 )
			{
				var tr = TransformVisualOverride ?? TransformV;

				Vector3 eyeDirection;

				//when weapon is active, eye position is calculated from weapon position
				var weapon = GetActiveWeapon();
				if( weapon != null )
				{
					var weaponTransform = weapon.TransformV;
					var bulletTransform = weapon.TypeCached.Mode1BulletTransform.Value;
					eyeDirection = weaponTransform.Position + weaponTransform.Rotation * new Vector3( 0, 0, bulletTransform.Position.Z );
				}
				else
				{
					eyeDirection = tr.Position + new Vector3( 0, 0, TypeCached.EyePosition.Value.Z );
					//eyeDirection = tr.Position + new Vector3( 0, 0, TypeCached.Height * 0.8 );
				}

				if( !CurrentLookToPosition.HasValue )
					CurrentLookToPosition = eyeDirection + tr.Rotation.GetForward() * 1000;

				var required = SphericalDirection.FromVector( RequiredLookToPosition.Value - eyeDirection );
				var current = SphericalDirection.FromVector( CurrentLookToPosition.Value - eyeDirection );

				//!!!!change together axes, not separated

				var step = (float)(double)TypeCached.TurningSpeedOfLooking.Value.InRadians() * Time.SimulationDelta;

				//update horizontal
				if( Math.Abs( required.Horizontal - current.Horizontal ) > 0.0001 )
				{
					var angle = required.Horizontal - current.Horizontal;
					var leftTurn = Math.Sin( angle ) > 0;

					current.Horizontal += leftTurn ? step : -step;

					var newAngle = required.Horizontal - current.Horizontal;
					var newLeftTurn = Math.Sin( newAngle ) > 0;

					if( newLeftTurn != leftTurn )
						current.Horizontal = required.Horizontal;
				}

				//update vertical
				if( Math.Abs( required.Vertical - current.Vertical ) > 0.0001 )
				{
					if( current.Vertical < required.Vertical )
					{
						current.Vertical += step;
						if( current.Vertical > required.Vertical )
							current.Vertical = required.Vertical;
					}
					else
					{
						current.Vertical -= step;
						if( current.Vertical < required.Vertical )
							current.Vertical = required.Vertical;
					}
				}

				CurrentLookToPosition = eyeDirection + current.GetVector() * 1000;

				//reset
				if( required == current )
					RequiredLookToPosition = null;
			}
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "Jump" )
			{
				SoundPlay( TypeCached.JumpSound );
				StartPlayOneAnimation( TypeCached.JumpAnimation );
			}
			else if( message == "CurrentLookToPosition" )
			{
				Vector3? value = null;
				if( reader.ReadBoolean() )
					value = reader.ReadVector3();
				if( !reader.Complete() )
					return false;

				if( !IsControlledByPlayerAndFirstPersonCameraEnabled() )
					CurrentLookToPosition = value;
			}
			else if( message == "Sitting" )
			{
				var value = reader.ReadBoolean();
				if( !reader.Complete() )
					return false;
				Sitting = value;
			}
			else if( message == "PlayDieAnimation" )
			{
				if( TypeCached.DieAnimation.ReferenceOrValueSpecified )
					StartPlayOneAnimation( TypeCached.DieAnimation, freezeOnEnd: true );
			}
			else if( message == "OnGroundState" )
			{
				clientIsOnGround = reader.ReadBoolean();
				groundBody1Velocity = reader.ReadVector3F();
				if( !reader.Complete() )
					return false;
			}
			else if( message == "SmoothCameraOffsetZ" )
			{
				smoothCameraOffsetZ = reader.ReadSingle();
				if( !reader.Complete() )
					return false;
			}

			return true;
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			//security check the object is controlled by the player
			var networkLogic = NetworkLogicUtility.GetNetworkLogic( this );
			if( networkLogic != null && networkLogic.ServerGetObjectControlledByUser( client.User, true ) == this )
			{
				if( message == "Jump" )
					Jump();
				else if( message == "ItemTakeAndActivate" )
				{
					var itemNetworkID = (long)reader.ReadVariableUInt64();
					var activate = reader.ReadBoolean();
					if( !reader.Complete() )
						return false;

					var item = ParentRoot.HierarchyController.GetComponentByNetworkID( itemNetworkID );
					if( item != null )
					{
						var item2 = item as ItemInterface;
						if( item2 != null )
						{
							var gameMode = (GameMode)ParentScene?.GetGameMode();
							if( gameMode != null )
							{
								if( ItemTake( gameMode, item2 ) )
								{
									if( activate )
										ItemActivate( gameMode, item2 );
								}
							}
						}
					}
				}
				else if( message == "ItemDrop" )
				{
					var itemNetworkID = (long)reader.ReadVariableUInt64();
					var amount = reader.ReadDouble();//ReadVariableInt32();
					if( !reader.Complete() )
						return false;

					var item = ParentRoot.HierarchyController.GetComponentByNetworkID( itemNetworkID );
					if( item != null )
					{
						var gameMode = (GameMode)ParentScene?.GetGameMode();
						if( gameMode != null )
						{
							var item2 = item as ItemInterface;
							if( item2 != null )
								ItemDrop( gameMode, item2, amount );
						}
					}
				}
				else if( message == "ItemActivate" )
				{
					var itemNetworkID = (long)reader.ReadVariableUInt64();
					if( !reader.Complete() )
						return false;

					var item = ParentRoot.HierarchyController.GetComponentByNetworkID( itemNetworkID );
					if( item != null && item.Parent == this )
					{
						var gameMode = (GameMode)ParentScene?.GetGameMode();
						if( gameMode != null )
						{
							var item2 = item as ItemInterface;
							if( item2 != null )
								ItemActivate( gameMode, item2 );
						}
					}
				}
				else if( message == "ItemDeactivate" )
				{
					var itemNetworkID = (long)reader.ReadVariableUInt64();
					if( !reader.Complete() )
						return false;

					var item = ParentRoot.HierarchyController.GetComponentByNetworkID( itemNetworkID );
					if( item != null && item.Parent == this )
					{
						var gameMode = (GameMode)ParentScene?.GetGameMode();
						if( gameMode != null )
						{
							var item2 = item as ItemInterface;
							if( item2 != null )
								ItemDeactivate( gameMode, item2 );
						}
					}
				}
			}

			return true;
		}

		public delegate void ProcessDamageBeforeDelegate( Character sender, long whoFired, ref float damage, ref object anyData, ref bool handled );
		public event ProcessDamageBeforeDelegate ProcessDamageBefore;
		public static event ProcessDamageBeforeDelegate ProcessDamageBeforeAll;

		public delegate void ProcessDamageAfterDelegate( Character sender, long whoFired, float damage, object anyData, double oldHealth );
		public event ProcessDamageAfterDelegate ProcessDamageAfter;
		public static event ProcessDamageAfterDelegate ProcessDamageAfterAll;

		public void ProcessDamage( long whoFired, float damage, object anyData )
		{
			var oldHealth = Health.Value;

			var damage2 = damage;
			var anyData2 = anyData;
			var handled = false;
			ProcessDamageBefore?.Invoke( this, whoFired, ref damage2, ref anyData2, ref handled );
			ProcessDamageBeforeAll?.Invoke( this, whoFired, ref damage2, ref anyData2, ref handled );

			if( !handled )
			{
				if( LifeStatus.Value == LifeStatusEnum.Normal )
				{
					var health = Health.Value;
					if( health > 0 )
					{
						Health = health - damage;

						if( Health.Value <= 0 )
						{
							LifeStatus = LifeStatusEnum.Dead;

							//!!!!manage Collision not just change here and from vehicle
							Collision = false;

							if( TypeCached.DieAnimation.ReferenceOrValueSpecified )
								StartPlayOneAnimation( TypeCached.DieAnimation, freezeOnEnd: true );

							if( NetworkIsServer )
							{
								BeginNetworkMessageToEveryone( "PlayDieAnimation" );
								EndNetworkMessage();
							}

							//RemoveFromParent( true );
						}
					}
				}
			}

			ProcessDamageAfter?.Invoke( this, whoFired, damage2, anyData2, oldHealth );
			ProcessDamageAfterAll?.Invoke( this, whoFired, damage2, anyData2, oldHealth );
		}

		public delegate void ObjectInteractionGetInfoEventDelegate( Character sender, GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info );
		public event ObjectInteractionGetInfoEventDelegate InteractionGetInfoEvent;

		public virtual void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info )
		{
			GetComponent<AI>()?.InteractionGetInfo( gameMode, initiator, ref info );
			InteractionGetInfoEvent?.Invoke( this, gameMode, initiator, ref info );
		}

		public virtual bool InteractionInputMessage( GameMode gameMode, Component initiator, InputMessage message )
		{
			var ai = GetComponent<AI>();
			if( ai != null && ai.InteractionInputMessage( gameMode, initiator, message ) )
				return true;
			return false;
		}

		public virtual void InteractionEnter( ObjectInteractionContext context )
		{
			GetComponent<AI>()?.InteractionEnter( context );
		}

		public virtual void InteractionExit( ObjectInteractionContext context )
		{
			GetComponent<AI>()?.InteractionExit( context );
		}

		public virtual void InteractionUpdate( ObjectInteractionContext context )
		{
			GetComponent<AI>()?.InteractionUpdate( context );
		}

		protected override void OnComponentRemoved( Component component )
		{
			base.OnComponentRemoved( component );

			if( animationControllerCached == component )
				animationControllerCached = null;
		}

		public delegate void FootstepSoundBeforeDelegate( Character sender, MeshInSpaceAnimationController.AnimationStateClass animationState, MeshInSpaceAnimationController.AnimationStateClass.AnimationItem animationItem, AnimationTrigger trigger, ref bool handled );
		public event FootstepSoundBeforeDelegate FootstepSoundBefore;

		void MeshInSpaceAnimationController.IParentAnimationTriggerProcess.AnimationTriggerFired( MeshInSpaceAnimationController sender, MeshInSpaceAnimationController.AnimationStateClass animationState, MeshInSpaceAnimationController.AnimationStateClass.AnimationItem animationItem, AnimationTrigger trigger )
		{
			if( trigger.AnyData == "Footstep" && animationItem.Factor > 0.5 )
			{
				var handled = false;
				FootstepSoundBefore?.Invoke( this, animationState, animationItem, trigger, ref handled );
				if( !handled )
					TypeCached.PerformFootstepSoundBefore( this, animationState, animationItem, trigger, ref handled );
				if( !handled )
					SoundPlay( TypeCached.FootstepSound, 0.25 );
			}
		}

		public delegate void FlyEndSoundBeforeDelegate( Character sender, ref bool handled );
		public event FlyEndSoundBeforeDelegate FlyEndSoundBefore;

		void TickFlyEndSound()
		{
			var newValue = IsOnGround();//IsOnGroundWithLatency();

			if( newValue && !flyEndSoundOnGround )
			{
				var handled = false;
				FlyEndSoundBefore?.Invoke( this, ref handled );
				if( !handled )
					TypeCached.PerformFlyEndSoundBefore( this, ref handled );
				if( !handled )
					SoundPlay( TypeCached.FlyEndSound );
			}

			flyEndSoundOnGround = newValue;
		}

		public bool IsControlledByPlayerAndFirstPersonCameraEnabled()
		{
			var gameMode = (GameMode)ParentScene?.GetGameMode();
			if( gameMode != null )
			{
				if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson && !gameMode.FreeCamera )
				{
					var character = gameMode.ObjectControlledByPlayer.Value as Character;
					if( character != null && character == this )
						return true;
				}
			}
			return false;
		}
	}
}
