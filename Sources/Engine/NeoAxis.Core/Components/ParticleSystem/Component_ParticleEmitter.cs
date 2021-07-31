// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A source of new particles of a <see cref="Component_ParticleSystem">particle system</see>.
	/// </summary>
	[NewObjectDefaultName( "Emitter" )]
	public class Component_ParticleEmitter : Component
	{
		/// <summary>
		/// The start time of the emitter.
		/// </summary>
		[DefaultValue( "0" )]
		[Category( "Time" )]
		public Reference<SingleProperty> StartTime
		{
			get { if( _startTime.BeginGet() ) StartTime = _startTime.Get( this ); return _startTime.value; }
			set
			{
				if( _startTime.BeginSet( ref value ) )
				{
					//!!!!is not so good
					if( !_startTime.value.ReferenceSpecified && _startTime.value.Value != null )
						_startTime.value.Value.Owner = this;

					try { StartTimeChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _startTime.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="StartTime"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> StartTimeChanged;
		ReferenceField<SingleProperty> _startTime = new SingleProperty( 0 );

		/// <summary>
		/// Emitter operating time.
		/// </summary>
		[DefaultValue( "3" )]
		[Category( "Time" )]
		public Reference<SingleProperty> Duration
		{
			get { if( _duration.BeginGet() ) Duration = _duration.Get( this ); return _duration.value; }
			set
			{
				if( _duration.BeginSet( ref value ) )
				{
					if( !_duration.value.ReferenceSpecified && _duration.value.Value != null )
						_duration.value.Value.Owner = this;

					try { DurationChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _duration.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Duration"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> DurationChanged;
		ReferenceField<SingleProperty> _duration = new SingleProperty( 3.0f );

		///// <summary>
		///// The probability of choosing this emitter from others when emit particle.
		///// </summary>
		//[DefaultValue( "1" )]
		////[Range( 0.0, 10.0, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		//[Category( "Emission" )]
		//public Reference<SingleProperty> Probability
		//{
		//	get { if( _probability.BeginGet() ) Probability = _probability.Get( this ); return _probability.value; }
		//	set { if( _probability.BeginSet( ref value ) ) { try { ProbabilityChanged?.Invoke( this ); ShouldRecompile(); } finally { _probability.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Probability"/> property value changes.</summary>
		//public event Action<Component_ParticleEmitter> ProbabilityChanged;
		//ReferenceField<SingleProperty> _probability = new SingleProperty( 1.0f );

		/// <summary>
		/// Particle creation frequency.
		/// </summary>
		[DefaultValue( "10" )]
		[Category( "Emission" )]
		public Reference<SingleProperty> SpawnRate
		{
			get { if( _spawnRate.BeginGet() ) SpawnRate = _spawnRate.Get( this ); return _spawnRate.value; }
			set
			{
				if( _spawnRate.BeginSet( ref value ) )
				{
					if( !_spawnRate.value.ReferenceSpecified && _spawnRate.value.Value != null )
						_spawnRate.value.Value.Owner = this;

					try { SpawnRateChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _spawnRate.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SpawnRate"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> SpawnRateChanged;
		ReferenceField<SingleProperty> _spawnRate = new SingleProperty( 10 );

		/// <summary>
		/// The multiplier of created particles number.
		/// </summary>
		[DefaultValue( "1" )]
		[Category( "Emission" )]
		public Reference<IntegerProperty> SpawnCount
		{
			get { if( _spawnCount.BeginGet() ) SpawnCount = _spawnCount.Get( this ); return _spawnCount.value; }
			set
			{
				if( _spawnCount.BeginSet( ref value ) )
				{
					if( !_spawnCount.value.ReferenceSpecified && _spawnCount.value.Value != null )
						_spawnCount.value.Value.Owner = this;

					try { SpawnCountChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _spawnCount.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SpawnCount"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> SpawnCountChanged;
		ReferenceField<IntegerProperty> _spawnCount = new IntegerProperty( 1 );

		public enum DirectionEnum
		{
			EmitterDirection,
			FromCenterOfEmitter,
		}

		/// <summary>
		/// The initial direction of the particles.
		/// </summary>
		[DefaultValue( DirectionEnum.EmitterDirection )]
		[Category( "Emission" )]
		public Reference<DirectionEnum> Direction
		{
			get { if( _direction.BeginGet() ) Direction = _direction.Get( this ); return _direction.value; }
			set { if( _direction.BeginSet( ref value ) ) { try { DirectionChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _direction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Direction"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> DirectionChanged;
		ReferenceField<DirectionEnum> _direction = DirectionEnum.EmitterDirection;

		/// <summary>
		/// The dispersion angle from the initial movement direction.
		/// </summary>
		[DefaultValue( "0" )]
		[Category( "Emission" )]
		public Reference<SingleProperty> DispersionAngle
		{
			get { if( _dispersionAngle.BeginGet() ) DispersionAngle = _dispersionAngle.Get( this ); return _dispersionAngle.value; }
			set
			{
				if( _dispersionAngle.BeginSet( ref value ) )
				{
					if( !_dispersionAngle.value.ReferenceSpecified && _dispersionAngle.value.Value != null )
						_dispersionAngle.value.Value.Owner = this;

					try { DispersionAngleChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _dispersionAngle.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DispersionAngle"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> DispersionAngleChanged;
		ReferenceField<SingleProperty> _dispersionAngle = new SingleProperty( 0.0f );

		/// <summary>
		/// The initial speed of the particles.
		/// </summary>
		[DefaultValue( "1" )]
		[Category( "Emission" )]
		public Reference<SingleProperty> Speed
		{
			get { if( _speed.BeginGet() ) Speed = _speed.Get( this ); return _speed.value; }
			set
			{
				if( _speed.BeginSet( ref value ) )
				{
					if( !_speed.value.ReferenceSpecified && _speed.value.Value != null )
						_speed.value.Value.Owner = this;

					try { SpeedChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _speed.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Speed"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> SpeedChanged;
		ReferenceField<SingleProperty> _speed = new SingleProperty( 1.0f );

		/// <summary>
		/// Whether to set the initial rotation in the direction of motion velocity.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Emission" )]
		public Reference<bool> RotateAlongMovement
		{
			get { if( _rotateAlongMovement.BeginGet() ) RotateAlongMovement = _rotateAlongMovement.Get( this ); return _rotateAlongMovement.value; }
			set { if( _rotateAlongMovement.BeginSet( ref value ) ) { try { RotateAlongMovementChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _rotateAlongMovement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotateAlongMovement"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> RotateAlongMovementChanged;
		ReferenceField<bool> _rotateAlongMovement = false;

		/// <summary>
		/// The initial rotation of the particles.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Emission" )]
		public Reference<AnglesProperty> Rotation
		{
			get { if( _rotation.BeginGet() ) Rotation = _rotation.Get( this ); return _rotation.value; }
			set
			{
				if( _rotation.BeginSet( ref value ) )
				{
					if( !_rotation.value.ReferenceSpecified && _rotation.value.Value != null )
						_rotation.value.Value.Owner = this;

					try { RotationChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _rotation.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Rotation"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> RotationChanged;
		ReferenceField<AnglesProperty> _rotation = new AnglesProperty( Vector3F.Zero );

		/// <summary>
		/// The initial angular velocity of the particles.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Emission" )]
		public Reference<AnglesProperty> AngularVelocity
		{
			get { if( _angularVelocity.BeginGet() ) AngularVelocity = _angularVelocity.Get( this ); return _angularVelocity.value; }
			set
			{
				if( _angularVelocity.BeginSet( ref value ) )
				{
					if( !_angularVelocity.value.ReferenceSpecified && _angularVelocity.value.Value != null )
						_angularVelocity.value.Value.Owner = this;

					try { AngularVelocityChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _angularVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularVelocity"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> AngularVelocityChanged;
		ReferenceField<AnglesProperty> _angularVelocity = new AnglesProperty( Vector3F.Zero );

		/// <summary>
		/// Life time of the particles.
		/// </summary>
		[DefaultValue( "3" )]
		[Category( "Particle" )]
		public Reference<SingleProperty> Lifetime
		{
			get { if( _lifetime.BeginGet() ) Lifetime = _lifetime.Get( this ); return _lifetime.value; }
			set
			{
				if( _lifetime.BeginSet( ref value ) )
				{
					if( !_lifetime.value.ReferenceSpecified && _lifetime.value.Value != null )
						_lifetime.value.Value.Owner = this;

					try { LifetimeChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _lifetime.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Lifetime"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> LifetimeChanged;
		ReferenceField<SingleProperty> _lifetime = new SingleProperty( 3.0f );

		/// <summary>
		/// The initial size of the particles.
		/// </summary>
		[DefaultValue( "1" )]
		[Category( "Particle" )]
		public Reference<SingleProperty> Size
		{
			get { if( _size.BeginGet() ) Size = _size.Get( this ); return _size.value; }
			set
			{
				if( _size.BeginSet( ref value ) )
				{
					if( !_size.value.ReferenceSpecified && _size.value.Value != null )
						_size.value.Value.Owner = this;

					try { SizeChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _size.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Size"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> SizeChanged;
		ReferenceField<SingleProperty> _size = new SingleProperty( 1.0f );

		/// <summary>
		/// Multiplier that affects the gravity of the scene.
		/// </summary>
		[DefaultValue( "0" )]
		[Category( "Particle" )]
		public Reference<SingleProperty> GravityMultiplier
		{
			get { if( _gravityMultiplier.BeginGet() ) GravityMultiplier = _gravityMultiplier.Get( this ); return _gravityMultiplier.value; }
			set
			{
				if( _gravityMultiplier.BeginSet( ref value ) )
				{
					if( !_gravityMultiplier.value.ReferenceSpecified && _gravityMultiplier.value.Value != null )
						_gravityMultiplier.value.Value.Owner = this;

					try { GravityMultiplierChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _gravityMultiplier.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="GravityMultiplier"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> GravityMultiplierChanged;
		ReferenceField<SingleProperty> _gravityMultiplier = new SingleProperty( 0.0f );

		public enum RenderingModeEnum
		{
			Billboard,
			Mesh,
			//Trail,
		}

		/// <summary>
		/// The rendering mode of the particles.
		/// </summary>
		[DefaultValue( RenderingModeEnum.Billboard )]
		[Category( "Rendering" )]
		public Reference<RenderingModeEnum> RenderingMode
		{
			get { if( _renderingMode.BeginGet() ) RenderingMode = _renderingMode.Get( this ); return _renderingMode.value; }
			set { if( _renderingMode.BeginSet( ref value ) ) { try { RenderingModeChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _renderingMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RenderingMode"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> RenderingModeChanged;
		ReferenceField<RenderingModeEnum> _renderingMode = RenderingModeEnum.Billboard;

		/// <summary>
		/// The mesh of the particles when Mesh rendering mode is used.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Rendering" )]
		public Reference<Component_Mesh> Mesh
		{
			get
			{
				//!!!!get optimized?

				if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value;
			}
			set { if( _mesh.BeginSet( ref value ) ) { try { MeshChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> MeshChanged;
		ReferenceField<Component_Mesh> _mesh = null;

		/// <summary>
		/// The material of the particles.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Rendering" )]
		public Reference<Component_Material> Material
		{
			get
			{
				//!!!!get optimized?

				if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value;
			}
			set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> MaterialChanged;
		ReferenceField<Component_Material> _material = null;

		/// <summary>
		/// The initial color and opacity of the particles.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Rendering" )]
		public Reference<ColorValueProperty> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set
			{
				if( _color.BeginSet( ref value ) )
				{
					if( !_color.value.ReferenceSpecified && _color.value.Value != null )
						_color.value.Value.Owner = this;

					try { ColorChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _color.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> ColorChanged;
		ReferenceField<ColorValueProperty> _color = new ColorValueProperty( ColorValue.One );

		/// <summary>
		/// Whether to cast shadows on the other surfaces.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Rendering" )]
		public Reference<bool> CastShadows
		{
			get { if( _castShadows.BeginGet() ) CastShadows = _castShadows.Get( this ); return _castShadows.value; }
			set { if( _castShadows.BeginSet( ref value ) ) { try { CastShadowsChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _castShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// Whether decals can be applied to the object.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Rendering" )]
		public Reference<bool> ReceiveDecals
		{
			get { if( _receiveDecals.BeginGet() ) ReceiveDecals = _receiveDecals.Get( this ); return _receiveDecals.value; }
			set { if( _receiveDecals.BeginSet( ref value ) ) { try { ReceiveDecalsChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _receiveDecals.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<Component_ParticleEmitter> ReceiveDecalsChanged;
		ReferenceField<bool> _receiveDecals = true;

		/////////////////////////////////////////

		[HCExpandable]
		public sealed class SingleProperty : Metadata.IMetadataProvider, ICanParseFromAndConvertToString
		{
			//!!!!логичнее было бы использовать продолжение ссылок в таких классах

			[Browsable( false )]
			public Component_ParticleEmitter Owner { get; set; }

			public enum TypeEnum
			{
				Constant,
				Range,
				Curve,
			}

			[DefaultValue( TypeEnum.Constant )]
			public TypeEnum Type
			{
				get { return type; }
				set { if( type != value ) { type = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			TypeEnum type = TypeEnum.Constant;

			public float Constant
			{
				get { return constant; }
				set { if( constant != value ) { constant = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			float constant = 1;

			public RangeF Range
			{
				get { return range; }
				set { if( range != value ) { range = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			RangeF range = new RangeF( 0, 1 );

			//!!!!не ReferenceList
			[Serialize]
			[Cloneable( CloneType.Deep )]
			public ReferenceList<Component_ParticleSystem.CurvePoint> Curve
			{
				get { return _curve; }
			}
			ReferenceList<Component_ParticleSystem.CurvePoint> _curve;

			//[DefaultValue( TypeEnum.Constant )]
			//public TypeEnum Type { get; set; } = TypeEnum.Constant;

			//public float Constant { get; set; } = 1;

			//public RangeF Range { get; set; } = new RangeF( 0, 1 );

			//[Cloneable( CloneType.Deep )]
			//public List<Component_ParticleSystem.CurvePoint> Curve { get; set; } = new List<Component_ParticleSystem.CurvePoint>();

			//

			public SingleProperty()
			{
				_curve = new ReferenceList<Component_ParticleSystem.CurvePoint>( null, delegate () { Owner?.ShouldRecompileParticleSystem(); } );
			}

			public SingleProperty( float contant ) : this()
			{
				Type = TypeEnum.Constant;
				Constant = contant;
			}

			public SingleProperty( RangeF range ) : this()
			{
				Type = TypeEnum.Range;
				Range = range;
			}

			[Browsable( false )]
			public Metadata.TypeInfo BaseType
			{
				get { return MetadataManager.GetTypeOfNetType( GetType() ); }
			}

			public IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context = null )
			{
				foreach( var m in BaseType.MetadataGetMembers( EditorUtility.getMemberContextNoFilter ) )
				{
					bool skip = false;
					if( context == null || context.filter )
						MetadataGetMembersFilter( context, m, ref skip );
					if( !skip )
						yield return m;
				}
			}

			public Metadata.Member MetadataGetMemberBySignature( string signature, Metadata.GetMembersContext context = null )
			{
				var m = BaseType.MetadataGetMemberBySignature( signature, EditorUtility.getMemberContextNoFilter );
				if( m != null )
					return m;
				return null;
			}

			void MetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
			{
				var p = member as Metadata.Property;
				if( p != null )
				{
					switch( p.Name )
					{
					case nameof( Constant ):
						if( Type != TypeEnum.Constant )
							skip = true;
						break;

					case nameof( Range ):
						if( Type != TypeEnum.Range )
							skip = true;
						break;

					case nameof( Curve ):
						if( Type != TypeEnum.Curve )
							skip = true;
						break;
					}
				}
			}

			public override string ToString()
			{
				switch( Type )
				{
				case TypeEnum.Constant: return Constant.ToString();
				case TypeEnum.Range: return Range.ToString();
				case TypeEnum.Curve: return "Curve";
				}
				return base.ToString();
			}

			public static SingleProperty Parse( string text )
			{
				if( string.IsNullOrEmpty( text ) )
					throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

				string[] values = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

				if( values.Length != 1 && values.Length != 2 )
					throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 1 or 2 parts separated by spaces in the form (x) or (x y).", text ) );

				try
				{
					if( values.Length == 1 )
						return new SingleProperty( float.Parse( values[ 0 ] ) );
					if( values.Length == 2 )
						return new SingleProperty( new RangeF( float.Parse( values[ 0 ] ), float.Parse( values[ 1 ] ) ) );
					return new SingleProperty();
				}
				catch( Exception )
				{
					throw new FormatException( "The parts of the vectors must be decimal numbers." );
				}
			}
		}

		/////////////////////////////////////////

		[HCExpandable]
		public sealed class IntegerProperty : Metadata.IMetadataProvider, ICanParseFromAndConvertToString
		{
			[Browsable( false )]
			public Component_ParticleEmitter Owner { get; set; }

			public enum TypeEnum
			{
				Constant,
				Range,
				Curve,
			}

			[DefaultValue( TypeEnum.Constant )]
			public TypeEnum Type
			{
				get { return type; }
				set { if( type != value ) { type = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			TypeEnum type = TypeEnum.Constant;

			public int Constant
			{
				get { return constant; }
				set { if( constant != value ) { constant = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			int constant = 1;

			public RangeI Range
			{
				get { return range; }
				set { if( range != value ) { range = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			RangeI range = new RangeI( 1, 1 );

			[Serialize]
			[Cloneable( CloneType.Deep )]
			public ReferenceList<Component_ParticleSystem.CurvePoint> Curve
			{
				get { return _curve; }
			}
			ReferenceList<Component_ParticleSystem.CurvePoint> _curve;

			////[DefaultValue( 1 )]
			//public int Constant { get; set; } = 1;

			////[DefaultValue( "1 1" )]
			//public RangeI Range { get; set; } = new RangeI( 1, 1 );

			//[Cloneable( CloneType.Deep )]
			//public List<Component_ParticleSystem.CurvePoint> Curve { get; set; } = new List<Component_ParticleSystem.CurvePoint>();

			//

			public IntegerProperty()
			{
				_curve = new ReferenceList<Component_ParticleSystem.CurvePoint>( null, delegate () { Owner?.ShouldRecompileParticleSystem(); } );
			}

			public IntegerProperty( int contant ) : this()
			{
				Type = TypeEnum.Constant;
				Constant = contant;
			}

			public IntegerProperty( RangeI range ) : this()
			{
				Type = TypeEnum.Range;
				Range = range;
			}

			[Browsable( false )]
			public Metadata.TypeInfo BaseType
			{
				get { return MetadataManager.GetTypeOfNetType( GetType() ); }
			}

			public IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context = null )
			{
				foreach( var m in BaseType.MetadataGetMembers( EditorUtility.getMemberContextNoFilter ) )
				{
					bool skip = false;
					if( context == null || context.filter )
						MetadataGetMembersFilter( context, m, ref skip );
					if( !skip )
						yield return m;
				}
			}

			public Metadata.Member MetadataGetMemberBySignature( string signature, Metadata.GetMembersContext context = null )
			{
				var m = BaseType.MetadataGetMemberBySignature( signature, EditorUtility.getMemberContextNoFilter );
				if( m != null )
					return m;
				return null;
			}

			void MetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
			{
				var p = member as Metadata.Property;
				if( p != null )
				{
					switch( p.Name )
					{
					case nameof( Constant ):
						if( Type != TypeEnum.Constant )
							skip = true;
						break;

					case nameof( Range ):
						if( Type != TypeEnum.Range )
							skip = true;
						break;

					case nameof( Curve ):
						if( Type != TypeEnum.Curve )
							skip = true;
						break;
					}
				}
			}

			public override string ToString()
			{
				switch( Type )
				{
				case TypeEnum.Constant: return Constant.ToString();
				case TypeEnum.Range: return Range.ToString();
				case TypeEnum.Curve: return "Curve";
				}
				return base.ToString();
			}

			public static IntegerProperty Parse( string text )
			{
				if( string.IsNullOrEmpty( text ) )
					throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

				string[] values = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

				if( values.Length != 1 && values.Length != 2 )
					throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 1 or 2 parts separated by spaces in the form (x) or (x y).", text ) );

				try
				{
					if( values.Length == 1 )
						return new IntegerProperty( int.Parse( values[ 0 ] ) );
					if( values.Length == 2 )
						return new IntegerProperty( new RangeI( int.Parse( values[ 0 ] ), int.Parse( values[ 1 ] ) ) );
					return new IntegerProperty();
				}
				catch( Exception )
				{
					throw new FormatException( "The parts of the vectors must be decimal numbers." );
				}
			}
		}

		/////////////////////////////////////////

		[HCExpandable]
		public sealed class AnglesProperty : Metadata.IMetadataProvider, ICanParseFromAndConvertToString
		{
			[Browsable( false )]
			public Component_ParticleEmitter Owner { get; set; }

			public enum TypeEnum
			{
				Constant,
				Range,
				Curve,
			}

			[DefaultValue( TypeEnum.Constant )]
			public TypeEnum Type
			{
				get { return type; }
				set { if( type != value ) { type = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			TypeEnum type = TypeEnum.Constant;

			public Vector3F Constant
			{
				get { return constant; }
				set { if( constant != value ) { constant = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			Vector3F constant = Vector3F.Zero;

			public RangeVector3F Range
			{
				get { return range; }
				set { if( range != value ) { range = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			RangeVector3F range = new RangeVector3F( Vector3F.Zero, Vector3F.Zero );

			[Serialize]
			[Cloneable( CloneType.Deep )]
			public ReferenceList<Component_ParticleSystem.CurvePoint> CurveX
			{
				get { return _curveX; }
			}
			ReferenceList<Component_ParticleSystem.CurvePoint> _curveX;

			[Serialize]
			[Cloneable( CloneType.Deep )]
			public ReferenceList<Component_ParticleSystem.CurvePoint> CurveY
			{
				get { return _curveY; }
			}
			ReferenceList<Component_ParticleSystem.CurvePoint> _curveY;

			[Serialize]
			[Cloneable( CloneType.Deep )]
			public ReferenceList<Component_ParticleSystem.CurvePoint> CurveZ
			{
				get { return _curveZ; }
			}
			ReferenceList<Component_ParticleSystem.CurvePoint> _curveZ;

			//

			public AnglesProperty()
			{
				_curveX = new ReferenceList<Component_ParticleSystem.CurvePoint>( null, delegate () { Owner?.ShouldRecompileParticleSystem(); } );
				_curveY = new ReferenceList<Component_ParticleSystem.CurvePoint>( null, delegate () { Owner?.ShouldRecompileParticleSystem(); } );
				_curveZ = new ReferenceList<Component_ParticleSystem.CurvePoint>( null, delegate () { Owner?.ShouldRecompileParticleSystem(); } );
			}

			public AnglesProperty( Vector3F contant ) : this()
			{
				Type = TypeEnum.Constant;
				Constant = contant;
			}

			public AnglesProperty( RangeVector3F range ) : this()
			{
				Type = TypeEnum.Range;
				Range = range;
			}

			[Browsable( false )]
			public Metadata.TypeInfo BaseType
			{
				get { return MetadataManager.GetTypeOfNetType( GetType() ); }
			}

			public IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context = null )
			{
				foreach( var m in BaseType.MetadataGetMembers( EditorUtility.getMemberContextNoFilter ) )
				{
					bool skip = false;
					if( context == null || context.filter )
						MetadataGetMembersFilter( context, m, ref skip );
					if( !skip )
						yield return m;
				}
			}

			public Metadata.Member MetadataGetMemberBySignature( string signature, Metadata.GetMembersContext context = null )
			{
				var m = BaseType.MetadataGetMemberBySignature( signature, EditorUtility.getMemberContextNoFilter );
				if( m != null )
					return m;
				return null;
			}

			void MetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
			{
				var p = member as Metadata.Property;
				if( p != null )
				{
					switch( p.Name )
					{
					case nameof( Constant ):
						if( Type != TypeEnum.Constant )
							skip = true;
						break;

					case nameof( Range ):
						if( Type != TypeEnum.Range )
							skip = true;
						break;

					case nameof( Curve ):
						if( Type != TypeEnum.Curve )
							skip = true;
						break;
					}
				}
			}

			public override string ToString()
			{
				switch( Type )
				{
				case TypeEnum.Constant: return Constant.ToString();
				case TypeEnum.Range: return Range.ToString();
				case TypeEnum.Curve: return "Curve";
				}
				return base.ToString();
			}

			public static AnglesProperty Parse( string text )
			{
				if( string.IsNullOrEmpty( text ) )
					throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

				string[] values = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

				if( values.Length != 3 && values.Length != 6 )
					throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 3, 6 parts separated by spaces in the form (X Y Z), or (X Y Z X Y Z).", text ) );

				try
				{
					if( values.Length == 3 )
						return new AnglesProperty( new Vector3F( float.Parse( values[ 0 ] ), float.Parse( values[ 1 ] ), float.Parse( values[ 2 ] ) ) );
					if( values.Length == 6 )
						return new AnglesProperty( new RangeVector3F( new Vector3F( float.Parse( values[ 0 ] ), float.Parse( values[ 1 ] ), float.Parse( values[ 2 ] ) ), new Vector3F( float.Parse( values[ 3 ] ), float.Parse( values[ 4 ] ), float.Parse( values[ 5 ] ) ) ) );
					return new AnglesProperty();
				}
				catch( Exception )
				{
					throw new FormatException( "The parts of the vectors must be decimal numbers." );
				}
			}
		}

		/////////////////////////////////////////

		[HCExpandable]
		public sealed class ColorValueProperty : Metadata.IMetadataProvider, ICanParseFromAndConvertToString
		{
			[Browsable( false )]
			public Component_ParticleEmitter Owner { get; set; }

			public enum TypeEnum
			{
				Constant,
				Range,
				//Curve,
			}

			[DefaultValue( TypeEnum.Constant )]
			public TypeEnum Type
			{
				get { return type; }
				set { if( type != value ) { type = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			TypeEnum type = TypeEnum.Constant;

			public ColorValue Constant
			{
				get { return constant; }
				set { if( constant != value ) { constant = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			ColorValue constant = ColorValue.One;

			public RangeColorValue Range
			{
				get { return range; }
				set { if( range != value ) { range = value; Owner?.ShouldRecompileParticleSystem(); } }
			}
			RangeColorValue range = new RangeColorValue( ColorValue.One, ColorValue.One );

			//[DefaultValue( TypeEnum.Constant )]
			//public TypeEnum Type { get; set; } = TypeEnum.Constant;

			////[DefaultValue( "1 1 1" )]
			//public ColorValue Constant { get; set; } = ColorValue.One;

			////[DefaultValue( "0 0 0 0 1 1 1 1" )]
			//public RangeColorValue Range { get; set; } = new RangeColorValue( ColorValue.Zero, ColorValue.One );

			////[Cloneable( CloneType.Deep )]
			////public List<Component_ParticleSystem.CurvePoint> Curve { get; set; } = new List<Component_ParticleSystem.CurvePoint>();

			//

			public ColorValueProperty()
			{
				//curve
			}

			public ColorValueProperty( ColorValue contant ) : this()
			{
				Type = TypeEnum.Constant;
				Constant = contant;
			}

			public ColorValueProperty( RangeColorValue range ) : this()
			{
				Type = TypeEnum.Range;
				Range = range;
			}

			[Browsable( false )]
			public Metadata.TypeInfo BaseType
			{
				get { return MetadataManager.GetTypeOfNetType( GetType() ); }
			}

			public IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context = null )
			{
				foreach( var m in BaseType.MetadataGetMembers( EditorUtility.getMemberContextNoFilter ) )
				{
					bool skip = false;
					if( context == null || context.filter )
						MetadataGetMembersFilter( context, m, ref skip );
					if( !skip )
						yield return m;
				}
			}

			public Metadata.Member MetadataGetMemberBySignature( string signature, Metadata.GetMembersContext context = null )
			{
				var m = BaseType.MetadataGetMemberBySignature( signature, EditorUtility.getMemberContextNoFilter );
				if( m != null )
					return m;
				return null;
			}

			void MetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
			{
				var p = member as Metadata.Property;
				if( p != null )
				{
					switch( p.Name )
					{
					case nameof( Constant ):
						if( Type != TypeEnum.Constant )
							skip = true;
						break;

					case nameof( Range ):
						if( Type != TypeEnum.Range )
							skip = true;
						break;

						//case nameof( Curve ):
						//	if( Type != TypeEnum.Curve )
						//		skip = true;
						//	break;
					}
				}
			}

			public override string ToString()
			{
				switch( Type )
				{
				case TypeEnum.Constant: return Constant.ToString();
				case TypeEnum.Range: return Range.ToString();
					//case TypeEnum.Curve: return "Curve";
				}
				return base.ToString();
			}

			public static ColorValueProperty Parse( string text )
			{
				if( string.IsNullOrEmpty( text ) )
					throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

				string[] values = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

				if( values.Length != 3 && values.Length != 4 && values.Length != 8 )
					throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 3, 4 or 8 parts separated by spaces in the form (Red Green Blue), (Red Green Blue Alpha) or (Red Green Blue Alpha Red Green Blue Alpha).", text ) );

				try
				{
					if( values.Length == 3 )
						return new ColorValueProperty( new ColorValue( float.Parse( values[ 0 ] ), float.Parse( values[ 1 ] ), float.Parse( values[ 2 ] ) ) );
					if( values.Length == 4 )
						return new ColorValueProperty( new ColorValue( float.Parse( values[ 0 ] ), float.Parse( values[ 1 ] ), float.Parse( values[ 2 ] ), float.Parse( values[ 3 ] ) ) );
					if( values.Length == 8 )
						return new ColorValueProperty( new RangeColorValue( new ColorValue( float.Parse( values[ 0 ] ), float.Parse( values[ 1 ] ), float.Parse( values[ 2 ] ), float.Parse( values[ 3 ] ) ), new ColorValue( float.Parse( values[ 4 ] ), float.Parse( values[ 5 ] ), float.Parse( values[ 6 ] ), float.Parse( values[ 7 ] ) ) ) );
					return new ColorValueProperty();
				}
				catch( Exception )
				{
					throw new FormatException( "The parts of the vectors must be decimal numbers." );
				}
			}
		}

		/////////////////////////////////////////

		//!!!!

		//public enum AttachmentEnum
		//{
		//	Free,
		//	Emitter,
		//}

		//[DefaultValue( AttachmentEnum.Free )]
		//[Category( "Particle" )]
		//public Reference<AttachmentEnum> Attachment
		//{
		//	get { if( _attachment.BeginGet() ) Attachment = _attachment.Get( this ); return _attachment.value; }
		//	set { if( _attachment.BeginSet( ref value ) ) { try { AttachmentChanged?.Invoke( this ); } finally { _attachment.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Attachment"/> property value changes.</summary>
		//public event Action<Component_ParticleEmitter> AttachmentChanged;
		//ReferenceField<AttachmentEnum> _attachment = AttachmentEnum.Free;

		/////////////////////////////////////////

		public Component_ParticleEmitter()
		{
			_startTime.value.Value.Owner = this;
			_duration.value.Value.Owner = this;
			_spawnRate.value.Value.Owner = this;
			_spawnCount.value.Value.Owner = this;
			_dispersionAngle.value.Value.Owner = this;
			_speed.value.Value.Owner = this;
			_lifetime.value.Value.Owner = this;
			_size.value.Value.Owner = this;
			_gravityMultiplier.value.Value.Owner = this;
			_color.value.Value.Owner = this;
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Mesh ):
					if( RenderingMode.Value != RenderingModeEnum.Mesh )
						skip = true;
					break;
				}
			}
		}

		public void ShouldRecompileParticleSystem()
		{
			var particleSystem = Parent as Component_ParticleSystem;
			if( particleSystem != null )
				particleSystem.ShouldRecompile = true;
		}

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			ShouldRecompileParticleSystem();
		}

		protected override void OnAddedToParent()
		{
			base.OnAddedToParent();

			ShouldRecompileParticleSystem();
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			base.OnRemovedFromParent( oldParent );

			var system = oldParent.FindThisOrParent<Component_ParticleSystem>();
			if( system != null )
				system.ShouldRecompile = true;
		}
	}
}
