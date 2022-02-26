// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A basic class for making doors.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Door", -3000 )]
	[NewObjectDefaultName( "Door" )]
	public class Door : MeshInSpace
	{
		/// <summary>
		/// The offset for a 'Door 1' component when the door is open.
		/// </summary>
		[DefaultValue( "0 1.5 1.1" )]
		[DisplayName( "Door 1 Open Offset" )]
		public Reference<Vector3> Door1OpenOffset
		{
			get { if( _door1OpenOffset.BeginGet() ) Door1OpenOffset = _door1OpenOffset.Get( this ); return _door1OpenOffset.value; }
			set
			{
				if( _door1OpenOffset.BeginSet( ref value ) )
				{
					try
					{
						Door1OpenOffsetChanged?.Invoke( this );

						if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
							UpdateBodiesToState( DesiredState );
					}
					finally { _door1OpenOffset.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Door1OpenOffset"/> property value changes.</summary>
		public event Action<Door> Door1OpenOffsetChanged;
		ReferenceField<Vector3> _door1OpenOffset = new Vector3( 0, 1.5, 1.1 );

		/// <summary>
		/// The offset for a 'Door 1' component when the door is closed.
		/// </summary>
		[DefaultValue( "0 0 1.1" )]
		[DisplayName( "Door 1 Closed Offset" )]
		public Reference<Vector3> Door1ClosedOffset
		{
			get { if( _door1ClosedOffset.BeginGet() ) Door1ClosedOffset = _door1ClosedOffset.Get( this ); return _door1ClosedOffset.value; }
			set
			{
				if( _door1ClosedOffset.BeginSet( ref value ) )
				{
					try
					{
						Door1ClosedOffsetChanged?.Invoke( this );

						if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
							UpdateBodiesToState( DesiredState );
					}
					finally { _door1ClosedOffset.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Door1ClosedOffset"/> property value changes.</summary>
		public event Action<Door> Door1ClosedOffsetChanged;
		ReferenceField<Vector3> _door1ClosedOffset = new Vector3( 0, 0, 1.1 );

		/// <summary>
		/// The offset for a 'Door 2' component when the door is open.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[DisplayName( "Door 2 Open Offset" )]
		public Reference<Vector3> Door2OpenOffset
		{
			get { if( _door2OpenOffset.BeginGet() ) Door2OpenOffset = _door2OpenOffset.Get( this ); return _door2OpenOffset.value; }
			set
			{
				if( _door2OpenOffset.BeginSet( ref value ) )
				{
					try
					{
						Door2OpenOffsetChanged?.Invoke( this );

						if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
							UpdateBodiesToState( DesiredState );
					}
					finally { _door2OpenOffset.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Door2OpenOffset"/> property value changes.</summary>
		public event Action<Door> Door2OpenOffsetChanged;
		ReferenceField<Vector3> _door2OpenOffset = new Vector3( 0, 0, 0 );

		/// <summary>
		/// The offset for a 'Door 2' component when the door is closed.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[DisplayName( "Door 2 Closed Offset" )]
		public Reference<Vector3> Door2ClosedOffset
		{
			get { if( _door2ClosedOffset.BeginGet() ) Door2ClosedOffset = _door2ClosedOffset.Get( this ); return _door2ClosedOffset.value; }
			set
			{
				if( _door2ClosedOffset.BeginSet( ref value ) )
				{
					try
					{
						Door2ClosedOffsetChanged?.Invoke( this );

						if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
							UpdateBodiesToState( DesiredState );
					}
					finally { _door2ClosedOffset.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Door2ClosedOffset"/> property value changes.</summary>
		public event Action<Door> Door2ClosedOffsetChanged;
		ReferenceField<Vector3> _door2ClosedOffset = new Vector3( 0, 0, 0 );

		/// <summary>
		/// The sound played when the door begins opening.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> OpenSound
		{
			get { if( _openSound.BeginGet() ) OpenSound = _openSound.Get( this ); return _openSound.value; }
			set { if( _openSound.BeginSet( ref value ) ) { try { OpenSoundChanged?.Invoke( this ); } finally { _openSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OpenSound"/> property value changes.</summary>
		public event Action<Door> OpenSoundChanged;
		ReferenceField<Sound> _openSound = null;

		/// <summary>
		/// The sound played when the door begins closing.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> CloseSound
		{
			get { if( _closeSound.BeginGet() ) CloseSound = _closeSound.Get( this ); return _closeSound.value; }
			set { if( _closeSound.BeginSet( ref value ) ) { try { CloseSoundChanged?.Invoke( this ); } finally { _closeSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CloseSound"/> property value changes.</summary>
		public event Action<Door> CloseSoundChanged;
		ReferenceField<Sound> _closeSound = null;

		/// <summary>
		/// Door opening time.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 5, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> OpeningTime
		{
			get { if( _openingTime.BeginGet() ) OpeningTime = _openingTime.Get( this ); return _openingTime.value; }
			set { if( _openingTime.BeginSet( ref value ) ) { try { OpeningTimeChanged?.Invoke( this ); } finally { _openingTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OpeningTime"/> property value changes.</summary>
		public event Action<Door> OpeningTimeChanged;
		ReferenceField<double> _openingTime = 1.0;

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
				if( _desiredState.BeginSet( ref value ) )
				{
					try
					{
						DesiredStateChanged?.Invoke( this );

						if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
							UpdateBodiesToState( _desiredState.value );
					}
					finally { _desiredState.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DesiredState"/> property value changes.</summary>
		public event Action<Door> DesiredStateChanged;
		ReferenceField<double> _desiredState = 0.0;

		///////////////////////////////////////////////

		//[Browsable( false )]
		//public double CurrentStateDoor1
		//{
		//	get
		//	{
		//		return 0;
		//	}
		//}

		//[Browsable( false )]
		//public double CurrentStateDoor2
		//{
		//	get
		//	{
		//		return 0;
		//	}
		//}

		[Browsable( false )]
		public double CurrentState
		{
			get
			{
				var transformOffset = GetTransformOffset( 1 );
				if( transformOffset != null )
				{
					var totalLength = ( Door1OpenOffset.Value - Door1ClosedOffset.Value ).Length();
					if( totalLength != 0 )
					{
						var length = ( transformOffset.PositionOffset.Value - Door1ClosedOffset.Value ).Length();

						var factor = length / totalLength;
						if( factor < 0.001 )
							factor = 0;
						if( factor > 0.999f )
							factor = 1;
						return factor;
					}
				}

				return 0;

				//return ( CurrentStateDoor1 + CurrentStateDoor2 ) / 2; 
			}
		}

		public void Open()
		{
			if( CurrentState != 1 && DesiredState != 1 )
				ParentScene?.SoundPlay( OpenSound, TransformV.Position );

			DesiredState = 1;
		}

		public void Close()
		{
			if( CurrentState != 0 && DesiredState != 0 )
				ParentScene?.SoundPlay( CloseSound, TransformV.Position );

			DesiredState = 0;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy && EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
				UpdateBodiesToState( _desiredState.value );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			{
				//Mesh
				Mesh mesh;
				{
					mesh = CreateComponent<Mesh>();
					mesh.Name = "Mesh";
					Mesh = ReferenceUtility.MakeThisReference( this, mesh );

					var geometry = mesh.CreateComponent<MeshGeometry_Door>();
					geometry.Name = "Mesh Geometry";
					geometry.Width = 2;
					geometry.Height = 2.4;
					geometry.DoorWidth = 1.5;
					geometry.DoorHeight = 2.2;
					geometry.Material = ReferenceUtility.MakeReference( @"Base\Materials\Green.material" );
				}

				//Collision Body
				RigidBody body;
				{
					body = CreateComponent<RigidBody>();
					body.Name = "Collision Body";

					var shape = body.CreateComponent<CollisionShape_Mesh>();
					shape.Mesh = ReferenceUtility.MakeThisReference( shape, mesh );
				}

				Transform = ReferenceUtility.MakeThisReference( this, body, "Transform" );
			}

			//Door 1
			{
				//Mesh in space
				MeshInSpace meshInSpace;
				{
					meshInSpace = CreateComponent<MeshInSpace>();
					meshInSpace.Name = "Door 1";
					meshInSpace.CanBeSelected = false;

					//Mesh
					Mesh mesh;
					{
						mesh = meshInSpace.CreateComponent<Mesh>();
						mesh.Name = "Mesh";

						var geometry = mesh.CreateComponent<MeshGeometry_Box>();
						geometry.Name = "Mesh Geometry";
						geometry.Dimensions = new Vector3( 0.1, 1.55, 2.25 );
						geometry.Material = ReferenceUtility.MakeReference( @"Base\Materials\Yellow.material" );
					}

					meshInSpace.Mesh = ReferenceUtility.MakeThisReference( meshInSpace, mesh );
				}

				//Collision Body
				RigidBody body;
				{
					body = meshInSpace.CreateComponent<RigidBody>();
					body.Name = "Collision Body";
					body.MotionType = RigidBody.MotionTypeEnum.Kinematic;

					var shape = body.CreateComponent<CollisionShape_Box>();
					shape.Dimensions = new Vector3( 0.1, 1.55, 2.25 );
					shape.Name = "Collision Shape";
				}

				//attach the mesh in space to the body
				meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, body, "Transform" );

				//attach the body to this component
				var transformOffset = body.CreateComponent<TransformOffset>();
				transformOffset.Name = "Attach Transform Offset";
				transformOffset.PositionOffset = Door1ClosedOffset;
				transformOffset.Source = ReferenceUtility.MakeThisReference( transformOffset, this, "Transform" );
				body.Transform = ReferenceUtility.MakeThisReference( body, transformOffset, "Result" );
			}
		}

		TransformOffset GetTransformOffset( int door )
		{
			return Components.GetByPath( $@"Door {door}\Collision Body\Attach Transform Offset" ) as TransformOffset;
		}

		void UpdateBodiesToState( double state )
		{
			for( int door = 1; door <= 2; door++ )
			{
				var transformOffset = GetTransformOffset( door );
				if( transformOffset != null )
				{
					var offsetOpen = Vector3.Zero;
					var offsetClosed = Vector3.Zero;
					switch( door )
					{
					case 1:
						offsetOpen = Door1OpenOffset;
						offsetClosed = Door1ClosedOffset;
						break;
					case 2:
						offsetOpen = Door2OpenOffset;
						offsetClosed = Door2ClosedOffset;
						break;
					}

					transformOffset.PositionOffset = Vector3.Lerp( offsetClosed, offsetOpen, state );
				}
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( CurrentState != DesiredState )
			{
				////play sounds
				//if( CurrentState == 0 )
				//	ParentScene?.SoundPlay( OpenSound, TransformV.Position );
				//if( CurrentState == 1 )
				//	ParentScene?.SoundPlay( CloseSound, TransformV.Position );

				//update doors position

				var openingTime = OpeningTime.Value;
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

				UpdateBodiesToState( newState );
			}
		}
	}
}
