// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Gate in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Gate\Gate", 630 )]
	[NewObjectDefaultName( "Gate" )]
#if !DEPLOY
	[Editor.SettingsCell( typeof( Editor.GateSettingsCell ) )]
#endif
	public class Gate : MeshInSpace, InteractiveObjectInterface
	{
		DynamicData dynamicData;
		bool needRecreateDynamicData;

		/////////////////////////////////////////

		const string defaultGateType = @"Content\Gates\Default\Default.gatetype";

		/// <summary>
		/// The type of the gate.
		/// </summary>
		[DefaultValueReference( defaultGateType )]
		public Reference<GateType> GateType
		{
			get { if( _gateType.BeginGet() ) GateType = _gateType.Get( this ); return _gateType.value; }
			set { if( _gateType.BeginSet( this, ref value ) ) { try { GateTypeChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _gateType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GateType"/> property value changes.</summary>
		public event Action<Gate> GateTypeChanged;
		ReferenceField<GateType> _gateType = new Reference<GateType>( null, defaultGateType );


		//!!!!for angular doors it can be from -1 to 1

		/// <summary>
		/// The target state of the door. 0 - closed, 1 - opened.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> DesiredState
		{
			get { if( _desiredState.BeginGet() ) DesiredState = _desiredState.Get( this ); return _desiredState.value; }
			set
			{
				if( _desiredState.BeginSet( this, ref value ) )
				{
					try
					{
						DesiredStateChanged?.Invoke( this );

						if( EngineApp.IsEditor )
							UpdateBodiesToStateImmediately( _desiredState.value );
					}
					finally { _desiredState.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DesiredState"/> property value changes.</summary>
		public event Action<Gate> DesiredStateChanged;
		ReferenceField<double> _desiredState = 0.0;

		/// <summary>
		/// Whether to allow user interaction with the object.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> AllowInteract
		{
			get { if( _allowInteract.BeginGet() ) AllowInteract = _allowInteract.Get( this ); return _allowInteract.value; }
			set { if( _allowInteract.BeginSet( this, ref value ) ) { try { AllowInteractChanged?.Invoke( this ); } finally { _allowInteract.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AllowInteract"/> property value changes.</summary>
		public event Action<Gate> AllowInteractChanged;
		ReferenceField<bool> _allowInteract = false;

		/////////////////////////////////////////

		public class DynamicData
		{
			public GateType GateType;
			public Mesh BaseMesh;

			public MeshInSpace Door1;
			public MeshInSpace Door2;
			public Constraint_SixDOF Door1Constraint;
			public Constraint_SixDOF Door2Constraint;

			//

			public bool IsDynamicBodies()
			{
				if( Door1?.PhysicalBody != null )
					return Door1.PhysicalBody.MotionType == PhysicsMotionType.Dynamic;
				return false;
			}
		}

		/////////////////////////////////////////

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				CreateDynamicData();
				if( EngineApp.IsEditor )
					UpdateBodiesToStateImmediately( DesiredState );
			}
			else
				DestroyDynamicData();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( needRecreateDynamicData )
			{
				CreateDynamicData();
				if( EngineApp.IsEditor )
					UpdateBodiesToStateImmediately( DesiredState );
			}
		}

		public void NeedRecreateDynamicData()
		{
			needRecreateDynamicData = true;
		}

		protected virtual DynamicData OnNewDynamicData()
		{
			return new DynamicData();
		}

		public void CreateDynamicData()
		{
			DestroyDynamicData();

			if( !EnabledInHierarchyAndIsInstance )
				return;

			var scene = ParentScene;
			if( scene == null )
				return;

			var type = GateType.Value;
			if( type == null )
				return;

			dynamicData = OnNewDynamicData();
			dynamicData.GateType = type;
			dynamicData.BaseMesh = type.BaseMesh;

			Mesh = dynamicData.BaseMesh;

			//create doors, constraints (only on server)
			if( !NetworkIsClient )
			{
				//door 1
				if( type.Door1Mesh.ReferenceSpecified )
				{
					//mesh in space
					MeshInSpace meshInSpace;
					{
						//need set DisplayInEditor = false before AddComponent
						meshInSpace = ComponentUtility.CreateComponent<MeshInSpace>( null, false, false );
						meshInSpace.DisplayInEditor = false;
						AddComponent( meshInSpace, -1 );
						//meshInSpace = CreateComponent<MeshInSpace>( enabled: false );

						dynamicData.Door1 = meshInSpace;
						meshInSpace.Name = "Door 1";
						meshInSpace.SaveSupport = false;
						meshInSpace.CanBeSelected = false;
						meshInSpace.CloneSupport = false;
						//meshInSpace.NetworkMode = NetworkModeEnum.False;
						meshInSpace.Mesh = type.Door1Mesh; //ReferenceUtility.MakeThisReference( meshInSpace, mesh );
						meshInSpace.Collision = true;

						meshInSpace.Transform = TransformV * new Transform( type.Door1Position );

						meshInSpace.Enabled = true;
					}

					//constraint
					Constraint_SixDOF constraint;
					if( dynamicData.IsDynamicBodies() )
					{
						//need set DisplayInEditor = false before AddComponent
						constraint = ComponentUtility.CreateComponent<Constraint_SixDOF>( null, false, false );
						constraint.DisplayInEditor = false;
						AddComponent( constraint, -1 );
						//constraint = CreateComponent<Constraint_SixDOF>( enabled: false );

						dynamicData.Door1Constraint = constraint;
						constraint.Name = "Door 1 Constraint";
						constraint.SaveSupport = false;
						constraint.CanBeSelected = false;
						constraint.CloneSupport = false;
						//constraint.NetworkMode = NetworkModeEnum.False;
						constraint.BodyA = ReferenceUtility.MakeThisReference( constraint, this );
						constraint.BodyB = ReferenceUtility.MakeThisReference( constraint, meshInSpace );
						constraint.CollisionsBetweenLinkedBodies = false;

						if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderHorizontal )
						{
							constraint.Transform = Transform;

							constraint.LinearXAxis = PhysicsAxisMode.Limited;
							constraint.LinearXAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.Position;

							if( type.DoorOpenOffset > 0 )
								constraint.LinearXAxisLimit = new Range( type.DoorLinearLimit.Value.Minimum, type.DoorLinearLimit.Value.Maximum );
							else
								constraint.LinearXAxisLimit = new Range( -type.DoorLinearLimit.Value.Maximum, -type.DoorLinearLimit.Value.Minimum );

							//if( type.DoorOpenOffset > 0 )
							//	constraint.LinearXAxisLimit = new Range( 0, type.DoorOpenOffset );
							//else
							//	constraint.LinearXAxisLimit = new Range( type.DoorOpenOffset, 0 );

							constraint.LinearXAxisMotorFrequency = type.DoorMotorFrequency;
							constraint.LinearXAxisMotorDamping = type.DoorMotorDamping;
							//constraint.LinearXAxisMotorLimit
						}
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderVertical )
						{
							constraint.Transform = Transform;

							constraint.LinearZAxis = PhysicsAxisMode.Limited;
							constraint.LinearZAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.Position;

							if( type.DoorOpenOffset > 0 )
								constraint.LinearZAxisLimit = new Range( type.DoorLinearLimit.Value.Minimum, type.DoorLinearLimit.Value.Maximum );
							else
								constraint.LinearZAxisLimit = new Range( -type.DoorLinearLimit.Value.Maximum, -type.DoorLinearLimit.Value.Minimum );

							//if( type.DoorOpenOffset > 0 )
							//	constraint.LinearZAxisLimit = new Range( 0, type.DoorOpenOffset );
							//else
							//	constraint.LinearZAxisLimit = new Range( type.DoorOpenOffset, 0 );

							constraint.LinearZAxisMotorFrequency = type.DoorMotorFrequency;
							constraint.LinearZAxisMotorDamping = type.DoorMotorDamping;
							//constraint.LinearXAxisMotorLimit
						}
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeSide )
						{
							var tr = TransformV;

							var forward = tr.Rotation * new Vector3( 0, 0, -1 );
							var up = tr.Rotation * new Vector3( 1, 0, 0 );

							var pos = ( tr * new Transform( type.Door1ConstraintPosition ) ).Position;
							var rot = Quaternion.LookAt( forward, up );
							constraint.Transform = new Transform( pos, rot, tr.Scale );

							constraint.AngularXAxis = PhysicsAxisMode.Limited;
							constraint.AngularXAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.Position;
							constraint.AngularXAxisLimit = type.DoorAngularLimit;
							constraint.AngularXAxisMotorFrequency = type.DoorMotorFrequency;
							constraint.AngularXAxisMotorDamping = type.DoorMotorDamping;
							//constraint.LinearXAxisMotorLimit


							//can't use AngularXAxis because 
							//RotationX, ///< When limited: MinLimit needs to be [-PI, 0], MaxLimit needs to be [0, PI]
							//RotationY, ///< When limited: MaxLimit between [0, PI]. MinLimit = -MaxLimit. Forms a cone shaped limit with Z.
							//RotationZ, ///< When limited: MaxLimit between [0, PI]. MinLimit = -MaxLimit. Forms a cone shaped limit with Y.

							//constraint.Transform = TransformV * new Transform( type.Door1ConstraintPosition );

							//constraint.AngularZAxis = PhysicsAxisMode.Limited;
							//constraint.AngularZAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.Position;
							//constraint.AngularZAxisLimit = type.DoorAngularLimit;
							//constraint.AngularZAxisMotorFrequency = type.DoorMotorFrequency;
							//constraint.AngularZAxisMotorDamping = type.DoorMotorDamping;
							////constraint.LinearZAxisMotorLimit
						}
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeUp )
						{
							var tr = TransformV;

							var forward = tr.Rotation * new Vector3( 1, 0, 0 );
							var up = tr.Rotation * new Vector3( 0, 0, -1 );

							var pos = ( tr * new Transform( type.Door1ConstraintPosition ) ).Position;
							var rot = Quaternion.LookAt( forward, up );
							constraint.Transform = new Transform( pos, rot, tr.Scale );

							constraint.AngularXAxis = PhysicsAxisMode.Limited;
							constraint.AngularXAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.Position;
							constraint.AngularXAxisLimit = type.DoorAngularLimit;
							constraint.AngularXAxisMotorFrequency = type.DoorMotorFrequency;
							constraint.AngularXAxisMotorDamping = type.DoorMotorDamping;
							//constraint.LinearXAxisMotorLimit
						}

						constraint.Enabled = true;
					}
				}

				//door 2
				if( type.Door2Mesh.ReferenceSpecified )
				{
					//mesh in space
					MeshInSpace meshInSpace;
					{
						//need set DisplayInEditor = false before AddComponent
						meshInSpace = ComponentUtility.CreateComponent<MeshInSpace>( null, false, false );
						meshInSpace.DisplayInEditor = false;
						AddComponent( meshInSpace, -1 );
						//meshInSpace = CreateComponent<MeshInSpace>( enabled: false );

						dynamicData.Door2 = meshInSpace;
						meshInSpace.Name = "Door 2";
						meshInSpace.SaveSupport = false;
						meshInSpace.CanBeSelected = false;
						meshInSpace.CloneSupport = false;
						//meshInSpace.NetworkMode = NetworkModeEnum.False;
						meshInSpace.Mesh = type.Door2Mesh; //ReferenceUtility.MakeThisReference( meshInSpace, mesh );
						meshInSpace.Collision = true;

						meshInSpace.Transform = TransformV * new Transform( type.Door2Position );

						meshInSpace.Enabled = true;
					}

					//constraint
					Constraint_SixDOF constraint;
					if( dynamicData.IsDynamicBodies() )
					{
						//need set DisplayInEditor = false before AddComponent
						constraint = ComponentUtility.CreateComponent<Constraint_SixDOF>( null, false, false );
						constraint.DisplayInEditor = false;
						AddComponent( constraint, -1 );
						//constraint = CreateComponent<Constraint_SixDOF>( enabled: false );

						dynamicData.Door2Constraint = constraint;
						constraint.Name = "Door 2 Constraint";
						constraint.SaveSupport = false;
						constraint.CanBeSelected = false;
						constraint.CloneSupport = false;
						//constraint.NetworkMode = NetworkModeEnum.False;
						constraint.BodyA = ReferenceUtility.MakeThisReference( constraint, this );
						constraint.BodyB = ReferenceUtility.MakeThisReference( constraint, meshInSpace );
						constraint.CollisionsBetweenLinkedBodies = false;

						if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderHorizontal )
						{
							constraint.Transform = Transform;

							constraint.LinearXAxis = PhysicsAxisMode.Limited;
							constraint.LinearXAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.Position;

							if( type.DoorOpenOffset < 0 )
								constraint.LinearXAxisLimit = new Range( type.DoorLinearLimit.Value.Minimum, type.DoorLinearLimit.Value.Maximum );
							else
								constraint.LinearXAxisLimit = new Range( -type.DoorLinearLimit.Value.Maximum, -type.DoorLinearLimit.Value.Minimum );

							//var invOffset = -type.DoorOpenOffset.Value;
							//if( invOffset > 0 )
							//	constraint.LinearXAxisLimit = new Range( 0, invOffset );
							//else
							//	constraint.LinearXAxisLimit = new Range( invOffset, 0 );

							constraint.LinearXAxisMotorFrequency = type.DoorMotorFrequency;
							constraint.LinearXAxisMotorDamping = type.DoorMotorDamping;
							//constraint.LinearXAxisMotorLimit
						}
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderVertical )
						{
							constraint.Transform = Transform;

							constraint.LinearZAxis = PhysicsAxisMode.Limited;
							constraint.LinearZAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.Position;

							if( type.DoorOpenOffset > 0 )
								constraint.LinearZAxisLimit = new Range( type.DoorLinearLimit.Value.Minimum, type.DoorLinearLimit.Value.Maximum );
							else
								constraint.LinearZAxisLimit = new Range( -type.DoorLinearLimit.Value.Maximum, -type.DoorLinearLimit.Value.Minimum );

							//var invOffset = -type.DoorOpenOffset.Value;
							//if( invOffset > 0 )
							//	constraint.LinearZAxisLimit = new Range( 0, invOffset );
							//else
							//	constraint.LinearZAxisLimit = new Range( invOffset, 0 );

							constraint.LinearZAxisMotorFrequency = type.DoorMotorFrequency;
							constraint.LinearZAxisMotorDamping = type.DoorMotorDamping;
							//constraint.LinearXAxisMotorLimit
						}
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeSide )
						{
							var tr = TransformV;

							var forward = tr.Rotation * new Vector3( 0, 0, 1 );
							var up = tr.Rotation * new Vector3( -1, 0, 0 );

							var pos = ( tr * new Transform( type.Door2ConstraintPosition ) ).Position;
							var rot = Quaternion.LookAt( forward, up );
							constraint.Transform = new Transform( pos, rot, tr.Scale );

							constraint.AngularXAxis = PhysicsAxisMode.Limited;
							constraint.AngularXAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.Position;
							constraint.AngularXAxisLimit = type.DoorAngularLimit;
							constraint.AngularXAxisMotorFrequency = type.DoorMotorFrequency;
							constraint.AngularXAxisMotorDamping = type.DoorMotorDamping;
							//constraint.LinearXAxisMotorLimit
						}
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeUp )
						{
							var tr = TransformV;

							var forward = tr.Rotation * new Vector3( 1, 0, 0 );
							var up = tr.Rotation * new Vector3( 0, 0, -1 );

							var pos = ( tr * new Transform( type.Door2ConstraintPosition ) ).Position;
							var rot = Quaternion.LookAt( forward, up );
							constraint.Transform = new Transform( pos, rot, tr.Scale );

							constraint.AngularXAxis = PhysicsAxisMode.Limited;
							constraint.AngularXAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.Position;
							constraint.AngularXAxisLimit = type.DoorAngularLimit;
							constraint.AngularXAxisMotorFrequency = type.DoorMotorFrequency;
							constraint.AngularXAxisMotorDamping = type.DoorMotorDamping;
							//constraint.LinearXAxisMotorLimit
						}

						constraint.Enabled = true;
					}
				}
			}

			Collision = true;
			SpaceBoundsUpdate();
			needRecreateDynamicData = false;
		}

		public void DestroyDynamicData()
		{
			if( dynamicData != null )
			{
				dynamicData.Door1Constraint?.RemoveFromParent( false );
				dynamicData.Door2Constraint?.RemoveFromParent( false );
				dynamicData.Door1?.RemoveFromParent( false );
				dynamicData.Door2?.RemoveFromParent( false );

				dynamicData = null;
			}
		}

		[Browsable( false )]
		public double CurrentState
		{
			get
			{
				if( dynamicData != null )
				{
					var type = dynamicData.GateType;

					if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderHorizontal || type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderVertical )
					{
						var stateDoor1 = 0.0;
						var stateDoor2 = 0.0;

						if( dynamicData.Door1 != null && type.DoorOpenOffset != 0 )
						{
							var p1 = ( TransformV * new Transform( type.Door1Position ) ).Position;
							var p2 = dynamicData.Door1.TransformV.Position;

							var diff = p1 - p2;
							if( diff == Vector3.Zero )
								diff.X += 0.001;
							var length = diff.Length();

							stateDoor1 = MathEx.Saturate( length / Math.Abs( type.DoorOpenOffset ) );
							if( dynamicData.IsDynamicBodies() )
							{
								if( stateDoor1 < 0.01 )
									stateDoor1 = 0;
								if( stateDoor1 > 0.99 )
									stateDoor1 = 1;
							}
						}

						if( dynamicData.Door2 != null && type.DoorOpenOffset != 0 )
						{
							var p1 = ( TransformV * new Transform( type.Door2Position ) ).Position;
							var p2 = dynamicData.Door2.TransformV.Position;

							var diff = p1 - p2;
							if( diff == Vector3.Zero )
								diff.X += 0.001;
							var length = diff.Length();

							stateDoor2 = MathEx.Saturate( length / Math.Abs( type.DoorOpenOffset ) );
							if( dynamicData.IsDynamicBodies() )
							{
								if( stateDoor2 < 0.01 )
									stateDoor2 = 0;
								if( stateDoor2 > 0.99 )
									stateDoor2 = 1;
							}
						}

						if( dynamicData.Door2 != null && type.DoorOpenOffset != 0 )
							return ( stateDoor1 + stateDoor2 ) / 2;
						else
							return stateDoor1;
					}
					else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeSide || type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeUp )
					{
						var stateDoor1 = 0.0;
						var stateDoor2 = 0.0;

						if( dynamicData.Door1 != null && type.DoorOpenAngle.Value != 0 )
						{
							var localRotation = dynamicData.Door1.TransformV.Rotation * TransformV.Rotation.GetInverse();

							if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeSide )
								stateDoor1 = localRotation.ToAngles().Yaw / type.DoorOpenAngle.Value;
							else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeUp )
								stateDoor1 = -localRotation.ToAngles().Roll / type.DoorOpenAngle.Value;

							//if( Name == "Hinge Up Dynamic Gate" )
							//{
							//	Log.Info( "qq :" + localRotation.ToAngles().ToString() );
							//	Log.Info( "vv :" + stateDoor1.ToString() );
							//}

							if( dynamicData.IsDynamicBodies() )
							{
								if( stateDoor1 < 0.01 )
									stateDoor1 = 0;
								if( stateDoor1 > 0.99 )
									stateDoor1 = 1;
							}
						}

						if( dynamicData.Door2 != null && type.DoorOpenAngle.Value != 0 )
						{
							var localRotation = dynamicData.Door2.TransformV.Rotation * TransformV.Rotation.GetInverse();

							if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeSide )
								stateDoor2 = -localRotation.ToAngles().Yaw / type.DoorOpenAngle.Value;
							else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeUp )
								stateDoor2 = localRotation.ToAngles().Roll / type.DoorOpenAngle.Value;

							if( dynamicData.IsDynamicBodies() )
							{
								if( stateDoor2 < 0.01 )
									stateDoor2 = 0;
								if( stateDoor2 > 0.99 )
									stateDoor2 = 1;
							}
						}

						if( dynamicData.Door2 != null && type.DoorOpenAngle.Value != 0 )
							return ( stateDoor1 + stateDoor2 ) / 2;
						else
							return stateDoor1;
					}
				}

				return 0;
			}
		}

		//public
		void Open()
		{
			if( CurrentState != 1 && DesiredState != 1 )
			{
				if( dynamicData != null )
				{
					var type = dynamicData.GateType;
					ParentScene?.SoundPlay( type.OpenSound, TransformV.Position );
					if( NetworkIsServer && type.OpenSound.ReferenceOrValueSpecified )
					{
						var m = BeginNetworkMessageToEveryone( "OpenSound" );
						if( m != null )
							m.End();
					}
				}
			}

			DesiredState = 1;
		}

		//public
		void Close()
		{
			if( CurrentState != 0 && DesiredState != 0 )
			{
				if( dynamicData != null )
				{
					var type = dynamicData.GateType;
					ParentScene?.SoundPlay( type.CloseSound, TransformV.Position );
					if( NetworkIsServer && type.CloseSound.ReferenceOrValueSpecified )
					{
						var m = BeginNetworkMessageToEveryone( "CloseSound" );
						if( m != null )
							m.End();
					}
				}
			}

			DesiredState = 0;
		}

		[Browsable( false )]
		public bool IsOpen
		{
			get { return CurrentState == 1; }
		}

		[Browsable( false )]
		public bool IsClosed
		{
			get { return CurrentState == 0; }
		}

		protected virtual void OnCanSwitch( Component initiator, ref bool canSwitch ) { }

		public delegate void CanSwitchDelegate( Gate sender, Component initiator, ref bool canSwitch );
		public event CanSwitchDelegate CanSwitch;

		public void TrySwitch( Component initiator )
		{
			//CanSwitch
			var canSwitch = true;
			OnCanSwitch( initiator, ref canSwitch );
			CanSwitch?.Invoke( this, initiator, ref canSwitch );
			if( !canSwitch )
				return;

			//start switching
			if( NetworkIsClient )
			{
				var m = BeginNetworkMessageToServer( "Switch" );
				if( m != null )
				{
					m.Writer.WriteVariableUInt64( initiator != null ? (ulong)initiator.NetworkID : 0 );
					m.End();
				}
			}
			else
			{
				if( CurrentState != 1 && DesiredState != 1 ) //if( !IsOpen )
					Open();
				else
					Close();
			}
		}


		//!!!!also add TrySetState( float value )


		void UpdateBodiesToStateImmediately( double state )
		{
			if( dynamicData != null )
			{
				var type = dynamicData.GateType;

				if( dynamicData.Door1 != null )
				{
					if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderHorizontal )
					{
						var offset = type.Door1Position.Value;
						offset.X += type.DoorOpenOffset * state;
						dynamicData.Door1.Transform = TransformV * new Transform( offset );
					}
					else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderVertical )
					{
						var offset = type.Door1Position.Value;
						offset.Z += type.DoorOpenOffset * state;
						dynamicData.Door1.Transform = TransformV * new Transform( offset );
					}
					else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeSide )
					{
						var rot = Quaternion.FromRotateByZ( MathEx.DegreeToRadian( MathEx.Lerp( 0, type.DoorOpenAngle.Value, state ) ) );
						var typeConstraintPos = type.Door1ConstraintPosition.Value;

						var localTransform = new Transform( type.Door1Position.Value - new Vector3( typeConstraintPos.X, typeConstraintPos.Y, 0 ) );
						localTransform = new Transform( Vector3.Zero, rot ) * localTransform;
						localTransform = new Transform( new Vector3( typeConstraintPos.X, typeConstraintPos.Y, 0 ) ) * localTransform;

						dynamicData.Door1.Transform = TransformV * localTransform;


						//var offset = type.Door1Position.Value;
						//offset.X += type.DoorOpenOffset * state;

						//var rot = Quaternion.FromRotateByZ( MathEx.DegreeToRadian( MathEx.Lerp( 0, type.DoorOpenAngle.Value, DesiredState ) ) );
						////var rot = Quaternion.FromRotateByX( MathEx.DegreeToRadian( MathEx.Lerp( 0, type.DoorOpenAngle.Value, DesiredState ) ) );

						//dynamicData.Door1.Transform = TransformV * new Transform( offset, rot );
					}
					else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeUp )
					{
						var rot = Quaternion.FromRotateByX( -MathEx.DegreeToRadian( MathEx.Lerp( 0, type.DoorOpenAngle.Value, state ) ) );
						var typeConstraintPos = type.Door1ConstraintPosition.Value;

						var localTransform = new Transform( type.Door1Position.Value - new Vector3( 0, 0, typeConstraintPos.Z ) );
						localTransform = new Transform( Vector3.Zero, rot ) * localTransform;
						localTransform = new Transform( new Vector3( 0, 0, typeConstraintPos.Z ) ) * localTransform;

						dynamicData.Door1.Transform = TransformV * localTransform;
					}


					//else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeSide )
					//{
					//	var tr = TransformV;

					//	var forward = tr.Rotation * new Vector3( 0, 0, 1 );
					//	var up = tr.Rotation * new Vector3( -1, 0, 0 );

					//	var pos = ( tr * new Transform( type.Door2ConstraintPosition ) ).Position;
					//	var rot = Quaternion.LookAt( forward, up );
					//	constraint.Transform = new Transform( pos, rot, tr.Scale );

					//	constraint.AngularXAxis = PhysicsAxisMode.Limited;
					//	constraint.AngularXAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.Position;
					//	constraint.AngularXAxisLimit = type.DoorAngularLimit;
					//	constraint.AngularXAxisMotorFrequency = type.DoorMotorFrequency;
					//	constraint.AngularXAxisMotorDamping = type.DoorMotorDamping;
					//	//constraint.LinearXAxisMotorLimit
					//}
					//else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeUp )
					//{
					//	var tr = TransformV;

					//	var forward = tr.Rotation * new Vector3( 1, 0, 0 );
					//	var up = tr.Rotation * new Vector3( 0, 0, -1 );

					//	var pos = ( tr * new Transform( type.Door2ConstraintPosition ) ).Position;
					//	var rot = Quaternion.LookAt( forward, up );
					//	constraint.Transform = new Transform( pos, rot, tr.Scale );

					//	constraint.AngularXAxis = PhysicsAxisMode.Limited;
					//	constraint.AngularXAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.Position;
					//	constraint.AngularXAxisLimit = type.DoorAngularLimit;
					//	constraint.AngularXAxisMotorFrequency = type.DoorMotorFrequency;
					//	constraint.AngularXAxisMotorDamping = type.DoorMotorDamping;
					//	//constraint.LinearXAxisMotorLimit
					//}


					//else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeSide )
					//	dynamicData.Door1Constraint.AngularXAxisMotorTarget = MathEx.Lerp( 0, type.DoorOpenAngle.Value, DesiredState );
					//else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeUp )
					//	dynamicData.Door1Constraint.AngularXAxisMotorTarget = MathEx.Lerp( 0, type.DoorOpenAngle.Value, DesiredState );

				}

				if( dynamicData.Door2 != null )
				{
					if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderHorizontal )
					{
						var offset = type.Door2Position.Value;
						offset.X -= type.DoorOpenOffset * state;
						dynamicData.Door2.Transform = TransformV * new Transform( offset );
					}
					else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderVertical )
					{
						var offset = type.Door2Position.Value;
						offset.Z -= type.DoorOpenOffset * state;
						dynamicData.Door2.Transform = TransformV * new Transform( offset );
					}
					else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeSide )
					{
						var rot = Quaternion.FromRotateByZ( -MathEx.DegreeToRadian( MathEx.Lerp( 0, type.DoorOpenAngle.Value, state ) ) );
						var typeConstraintPos = type.Door2ConstraintPosition.Value;

						var localTransform = new Transform( type.Door2Position.Value - new Vector3( typeConstraintPos.X, typeConstraintPos.Y, 0 ) );
						localTransform = new Transform( Vector3.Zero, rot ) * localTransform;
						localTransform = new Transform( new Vector3( typeConstraintPos.X, typeConstraintPos.Y, 0 ) ) * localTransform;

						dynamicData.Door2.Transform = TransformV * localTransform;
					}
					else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeUp )
					{
						var rot = Quaternion.FromRotateByX( -MathEx.DegreeToRadian( MathEx.Lerp( 0, type.DoorOpenAngle.Value, state ) ) );
						var typeConstraintPos = type.Door1ConstraintPosition.Value;

						var localTransform = new Transform( type.Door1Position.Value - new Vector3( 0, 0, typeConstraintPos.Z ) );
						localTransform = new Transform( Vector3.Zero, rot ) * localTransform;
						localTransform = new Transform( new Vector3( 0, 0, typeConstraintPos.Z ) ) * localTransform;

						dynamicData.Door1.Transform = TransformV * localTransform;
					}
				}
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//Log.Info( "current: " + CurrentState.ToString() );

			if( CurrentState != DesiredState )
			{
				if( dynamicData != null )
				{
					var type = dynamicData.GateType;

					//update the constraint of the door 1
					if( dynamicData.Door1Constraint != null )
					{
						if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderHorizontal )
							dynamicData.Door1Constraint.LinearXAxisMotorTarget = MathEx.Lerp( 0, type.DoorOpenOffset, DesiredState );
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderVertical )
							dynamicData.Door1Constraint.LinearZAxisMotorTarget = MathEx.Lerp( 0, type.DoorOpenOffset, DesiredState );
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeSide )
							dynamicData.Door1Constraint.AngularXAxisMotorTarget = MathEx.Lerp( 0, type.DoorOpenAngle.Value, DesiredState );
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeUp )
							dynamicData.Door1Constraint.AngularXAxisMotorTarget = MathEx.Lerp( 0, type.DoorOpenAngle.Value, DesiredState );

						//!!!!wake up. maybe it can be automatic
						if( dynamicData.Door1?.PhysicalBody != null )
							dynamicData.Door1.PhysicalBody.Active = true;
					}

					//update the constraint of the door 2
					if( dynamicData.Door2Constraint != null )
					{
						if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderHorizontal )
							dynamicData.Door2Constraint.LinearXAxisMotorTarget = MathEx.Lerp( 0, -type.DoorOpenOffset, DesiredState );
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.SliderVertical )
							dynamicData.Door2Constraint.LinearZAxisMotorTarget = MathEx.Lerp( 0, -type.DoorOpenOffset, DesiredState );
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeSide )
							dynamicData.Door2Constraint.AngularXAxisMotorTarget = MathEx.Lerp( 0, type.DoorOpenAngle.Value, DesiredState );
						else if( type.OpeningType.Value == NeoAxis.GateType.OpeningTypeEnum.HingeUp )
							dynamicData.Door2Constraint.AngularXAxisMotorTarget = MathEx.Lerp( 0, type.DoorOpenAngle.Value, DesiredState );

						//!!!!wake up. maybe it can be automatic
						if( dynamicData.Door2?.PhysicalBody != null )
							dynamicData.Door2.PhysicalBody.Active = true;
					}

					//update when the dynamic body is not dynamic
					if( !dynamicData.IsDynamicBodies() )
					{
						var openingTime = type.DoorOpeningTime.Value;
						if( openingTime == 0 )
							openingTime = 0.000001;

						var newState = CurrentState;
						if( newState < DesiredState )
						{
							newState += Time.SimulationDelta / openingTime;
							if( newState > DesiredState )
								newState = DesiredState;
						}
						else
						{
							newState -= Time.SimulationDelta / openingTime;
							if( newState < DesiredState )
								newState = DesiredState;
						}

						UpdateBodiesToStateImmediately( newState );
					}
				}
			}
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "OpenSound" )
			{
				if( dynamicData != null )
					ParentScene?.SoundPlay( dynamicData.GateType.OpenSound, TransformV.Position );
			}
			else if( message == "CloseSound" )
			{
				if( dynamicData != null )
					ParentScene?.SoundPlay( dynamicData.GateType.CloseSound, TransformV.Position );
			}

			return true;
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			NeedRecreateDynamicData();
		}

		public delegate void InteractionGetInfoEventDelegate( Gate sender, GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info );
		public event InteractionGetInfoEventDelegate InteractionGetInfoEvent;

		public virtual void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info )
		{
			info = new InteractiveObjectObjectInfo();
			info.AllowInteract = AllowInteract;
			//info.Text.Add( Name );
			InteractionGetInfoEvent?.Invoke( this, gameMode, initiator, ref info );
		}

		public virtual bool InteractionInputMessage( GameMode gameMode, Component initiator, InputMessage message )
		{
			var mouseDown = message as InputMessageMouseButtonDown;
			if( mouseDown != null )
			{
				if( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right )
				{
					//var initiator = gameMode.ObjectControlledByPlayer.Value;
					TrySwitch( initiator );
					return true;
				}
			}

			return false;
		}

		public virtual void InteractionEnter( ObjectInteractionContext context )
		{
		}

		public virtual void InteractionExit( ObjectInteractionContext context )
		{
		}

		public virtual void InteractionUpdate( ObjectInteractionContext context )
		{
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			if( message == "Switch" )
			{
				var initiatorNetworkID = (long)reader.ReadVariableUInt64();
				if( !reader.Complete() )
					return false;
				var initiator = ParentRoot.HierarchyController.GetComponentByNetworkID( initiatorNetworkID );
				TrySwitch( initiator );
			}

			return true;
		}

		//protected override bool OnSpaceBoundsUpdateIncludeChildren()
		//{
		//	return true;
		//}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			context.thisObjectWasChecked = true;
			var bounds = SpaceBounds.BoundingBox;
			if( bounds.Intersects( context.ray, out var scale ) )
				context.thisObjectResultRayScale = scale;
		}
	}
}