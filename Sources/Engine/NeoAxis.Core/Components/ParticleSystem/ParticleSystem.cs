// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Defines a particle system.
	/// </summary>
	[ResourceFileExtension( "particle" )]
	[EditorControl( typeof( ParticleSystemEditor ) )]
	[Preview( typeof( ParticleSystemPreview ) )]
	[SettingsCell( typeof( ParticleSystemSettingsCell ) )]
	public class ParticleSystem : ResultCompile<ParticleSystem.CompiledData>
	{
		int mustRecreateInstancesCounter;

		//

		/// <summary>
		/// Whether the particle system will be repeated after completion.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> Looping
		{
			get { if( _looping.BeginGet() ) Looping = _looping.Get( this ); return _looping.value; }
			set { if( _looping.BeginSet( ref value ) ) { try { LoopingChanged?.Invoke( this ); ShouldRecompile = true; } finally { _looping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Looping"/> property value changes.</summary>
		public event Action<ParticleSystem> LoopingChanged;
		ReferenceField<bool> _looping = true;

		public enum SimulationSpaceEnum
		{
			World,
			Local,
			//Custom
		}

		/// <summary>
		/// Space in which the particles will be displayed.
		/// </summary>
		[DefaultValue( SimulationSpaceEnum.World )]
		public Reference<SimulationSpaceEnum> SimulationSpace
		{
			get { if( _simulationSpace.BeginGet() ) SimulationSpace = _simulationSpace.Get( this ); return _simulationSpace.value; }
			set { if( _simulationSpace.BeginSet( ref value ) ) { try { SimulationSpaceChanged?.Invoke( this ); ShouldRecompile = true; } finally { _simulationSpace.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SimulationSpace"/> property value changes.</summary>
		public event Action<ParticleSystem> SimulationSpaceChanged;
		ReferenceField<SimulationSpaceEnum> _simulationSpace = SimulationSpaceEnum.World;

		///// <summary>
		///// Whether particles will be drawn in a group for purpose of faster rendering.
		///// </summary>
		//[DefaultValue( true )]
		//public Reference<bool> Batching
		//{
		//	get { if( _batching.BeginGet() ) Batching = _batching.Get( this ); return _batching.value; }
		//	set { if( _batching.BeginSet( ref value ) ) { try { BatchingChanged?.Invoke( this ); } finally { _batching.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Batching"/> property value changes.</summary>
		//public event Action<ParticleSystem> BatchingChanged;
		//ReferenceField<bool> _batching = true;

		/// <summary>
		/// The maximum number of particles that can exist at one time.
		/// </summary>
		[DefaultValue( 1000 )]
		public Reference<int> MaxParticles
		{
			get { if( _maxParticles.BeginGet() ) MaxParticles = _maxParticles.Get( this ); return _maxParticles.value; }
			set { if( _maxParticles.BeginSet( ref value ) ) { try { MaxParticlesChanged?.Invoke( this ); } finally { _maxParticles.EndSet(); } } }
		}
		public event Action<ParticleSystem> MaxParticlesChanged;
		ReferenceField<int> _maxParticles = 1000;

		/// <summary>
		/// Multiplier for simulation run time.
		/// </summary>
		[DefaultValue( 1.0f )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<float> SimulationSpeed
		{
			get { if( _simulationSpeed.BeginGet() ) SimulationSpeed = _simulationSpeed.Get( this ); return _simulationSpeed.value; }
			set { if( _simulationSpeed.BeginSet( ref value ) ) { try { SimulationSpeedChanged?.Invoke( this ); } finally { _simulationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SimulationSpeed"/> property value changes.</summary>
		public event Action<ParticleSystem> SimulationSpeedChanged;
		ReferenceField<float> _simulationSpeed = 1.0f;

		/// <summary>
		/// The factor of maximum visibility distance. The maximum distance is calculated based on the size of the object on the screen.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> VisibilityDistanceFactor
		{
			get { if( _visibilityDistanceFactor.BeginGet() ) VisibilityDistanceFactor = _visibilityDistanceFactor.Get( this ); return _visibilityDistanceFactor.value; }
			set { if( _visibilityDistanceFactor.BeginSet( ref value ) ) { try { VisibilityDistanceFactorChanged?.Invoke( this ); } finally { _visibilityDistanceFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<ParticleSystem> VisibilityDistanceFactorChanged;
		ReferenceField<double> _visibilityDistanceFactor = 1.0;

		///// <summary>
		///// Maximum visibility range of the object.
		///// </summary>
		//[DefaultValue( 10000.0 )]
		//[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		////[Category( "Rendering" )]
		//public Reference<double> VisibilityDistance
		//{
		//	get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
		//	set { if( _visibilityDistance.BeginSet( ref value ) ) { try { VisibilityDistanceChanged?.Invoke( this ); } finally { _visibilityDistance.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		//public event Action<ParticleSystem> VisibilityDistanceChanged;
		//ReferenceField<double> _visibilityDistance = 10000.0;

		/////////////////////////////////////////

		[HCExpandable]
		public struct CurvePoint : ICanParseFromAndConvertToString
		{
			//[DefaultValue( 0.0 )]
			[Serialize]
			public float Point { get; set; }

			//[DefaultValue( 0.0 )]
			[Serialize]
			public float Value { get; set; }

			//

			public CurvePoint( float point, float value )
			{
				Point = point;
				Value = value;
			}

			public override string ToString()
			{
				return string.Format( "{0} {1}", Point, Value );
				//return string.Format( "{0} - {1}", Time, Value );
			}

			public static CurvePoint Parse( string text )
			{
				if( string.IsNullOrEmpty( text ) )
					throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

				string[] values = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

				if( values.Length != 2 )
					throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form \'Point Value\'.", text ) );

				try
				{
					return new CurvePoint( float.Parse( values[ 0 ] ), float.Parse( values[ 1 ] ) );
				}
				catch( Exception )
				{
					throw new FormatException( "The parts of the value must be decimal numbers." );
				}
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents precalculated data of <see cref="ParticleSystem"/>.
		/// </summary>
		public class CompiledData : IDisposable
		{
			public ParticleSystem Owner { get; }
			public object AnyData { get; set; }

			public bool Looping { get; }
			//public int MaxParticles { get; }
			public SimulationSpaceEnum SimulationSpace { get; }
			//public float SimulationSpeed { get; }
			//public double VisibilityDistance { get; }
			public Emitter[] Emitters { get; }

			////////////

			public class Emitter
			{
				public ParticleEmitter Owner;
				public object AnyData;

				//public double VisibilityDistance;
				public bool CastShadows;
				public bool ReceiveDecals;
				public float MotionBlurFactor;

				//properties

				public SingleProperty StartTime;
				public SingleProperty Duration;

				//public SingleProperty Probability;
				public SingleProperty SpawnRate;
				public IntegerProperty SpawnCount;
				public ParticleEmitter.DirectionEnum Direction;
				public SingleProperty DispersionAngle;
				public SingleProperty Speed;
				public bool RotateAlongMovement;
				public AnglesProperty Rotation;
				public AnglesProperty AngularVelocity;

				public SingleProperty Lifetime;
				public SingleProperty Size;
				public SingleProperty GravityMultiplier;

				public Mesh Mesh;
				public Material Material;
				public ColorValueProperty Color;

				//shapes
				public ParticleEmitterShape[] Shapes;

				//linear, angular acceleration by time
				public struct AccelerationByTime
				{
					public ParticleAccelerationByTime.TypeEnum Type;
					public Vector3F Constant;
					public RangeVector3F Range;
					public CurveCubicSpline1F CurveX;
					public CurveCubicSpline1F CurveY;
					public CurveCubicSpline1F CurveZ;
					//public ParticleAccelerationByTime.LockedAxesEnum LockedAxes;

					public void Calculate( float lifetime, float time, out Vector3F result )
					{
						switch( Type )
						{
						case ParticleAccelerationByTime.TypeEnum.Constant:
							result = Constant;
							break;

						case ParticleAccelerationByTime.TypeEnum.Range:
							Vector3F.Lerp( ref Range.Minimum, ref Range.Maximum, MathEx.Saturate( time / lifetime ), out result );
							break;

						case ParticleAccelerationByTime.TypeEnum.Curve:
							result.X = CurveX != null ? CurveX.CalculateValueByTime( time ) : 0;
							result.Y = CurveY != null ? CurveY.CalculateValueByTime( time ) : 0;
							result.Z = CurveZ != null ? CurveZ.CalculateValueByTime( time ) : 0;
							break;

						default:
							result = Vector3F.Zero;
							break;
						}
					}
				}
				public AccelerationByTime[] LinearAccelerationByTimes;
				public AccelerationByTime[] AngularAccelerationByTimes;

				//colorMultiplierByTimes
				public struct ColorMultiplierByTime
				{
					public ParticleColorMultiplierByTime.ChannelsEnum Channels;
					public ParticleColorMultiplierByTime.TypeEnum Type;
					public RangeF Range;
					public CurveCubicSpline1F Curve;

					public void Calculate( float lifetime, float time, out ColorValue result )
					{
						float value = 1;

						switch( Type )
						{
						case ParticleColorMultiplierByTime.TypeEnum.Range:
							value = MathEx.Lerp( Range.Minimum, Range.Maximum, MathEx.Saturate( time / lifetime ) );
							break;

						case ParticleColorMultiplierByTime.TypeEnum.Curve:
							value = Curve != null ? Curve.CalculateValueByTime( time ) : 0;
							break;
						}

						result = ColorValue.One;
						if( Channels.HasFlag( ParticleColorMultiplierByTime.ChannelsEnum.Red ) )
							result.Red *= value;
						if( Channels.HasFlag( ParticleColorMultiplierByTime.ChannelsEnum.Green ) )
							result.Green *= value;
						if( Channels.HasFlag( ParticleColorMultiplierByTime.ChannelsEnum.Blue ) )
							result.Blue *= value;
						if( Channels.HasFlag( ParticleColorMultiplierByTime.ChannelsEnum.Alpha ) )
							result.Alpha *= value;
					}
				}
				public ColorMultiplierByTime[] ColorMultiplierByTimes;

				//sizeMultiplierByTimes
				public struct SizeMultiplierByTime
				{
					public ParticleSizeMultiplierByTime.AxesEnum Axes;
					public ParticleSizeMultiplierByTime.TypeEnum Type;
					public RangeF Range;
					public CurveCubicSpline1F Curve;

					public float Calculate( float lifetime, float time )
					{
						switch( Type )
						{
						case ParticleSizeMultiplierByTime.TypeEnum.Range:
							return MathEx.Lerp( Range.Minimum, Range.Maximum, MathEx.Saturate( time / lifetime ) );

						case ParticleSizeMultiplierByTime.TypeEnum.Curve:
							return Curve != null ? Curve.CalculateValueByTime( time ) : 1;
						}

						return 1;
					}
				}
				public SizeMultiplierByTime[] SizeMultiplierByTimes;

				//linear, angular speedMultiplierByTimes
				public struct SpeedMultiplierByTime
				{
					public ParticleSpeedMultiplierByTime.TypeEnum Type;
					public float Constant;
					public RangeF Range;
					public CurveCubicSpline1F Curve;

					public float Calculate( float lifetime, float time )
					{
						switch( Type )
						{
						case ParticleSpeedMultiplierByTime.TypeEnum.Constant:
							return Constant;

						case ParticleSpeedMultiplierByTime.TypeEnum.Range:
							return MathEx.Lerp( Range.Minimum, Range.Maximum, MathEx.Saturate( time / lifetime ) );

						case ParticleSpeedMultiplierByTime.TypeEnum.Curve:
							return Curve != null ? Curve.CalculateValueByTime( time ) : 1;
						}

						return 1;
					}
				}
				public SpeedMultiplierByTime[] LinearSpeedMultiplierByTimes;
				public SpeedMultiplierByTime[] AngularSpeedMultiplierByTimes;

				//linear, angular VelocityByTimes
				public struct VelocityByTime
				{
					public ParticleVelocityByTime.TypeEnum Type;
					public Vector3F Constant;
					public RangeVector3F Range;
					public CurveCubicSpline1F CurveX;
					public CurveCubicSpline1F CurveY;
					public CurveCubicSpline1F CurveZ;

					public void Calculate( float lifetime, float time, out Vector3F result )
					{
						switch( Type )
						{
						case ParticleVelocityByTime.TypeEnum.Constant:
							result = Constant;
							break;

						case ParticleVelocityByTime.TypeEnum.Range:
							Vector3F.Lerp( ref Range.Minimum, ref Range.Maximum, MathEx.Saturate( time / lifetime ), out result );
							break;

						case ParticleVelocityByTime.TypeEnum.Curve:
							result.X = CurveX != null ? CurveX.CalculateValueByTime( time ) : 0;
							result.Y = CurveY != null ? CurveY.CalculateValueByTime( time ) : 0;
							result.Z = CurveZ != null ? CurveZ.CalculateValueByTime( time ) : 0;
							break;

						default:
							result = Vector3F.Zero;
							break;
						}
					}
				}
				public VelocityByTime[] LinearVelocityByTimes;
				public VelocityByTime[] AngularVelocityByTimes;

				//custom modules
				public ParticleModuleCustom[] CustomModules;

				//

				//public bool Equals( Emitter obj )
				//{

				//	if( !Duration.Equals( ref obj.Duration ) )
				//		return false;

				//	return true;
				//}
			}

			////////////

			public struct SingleProperty
			{
				public ParticleEmitter.SingleProperty.TypeEnum Type;
				public float Constant;
				public RangeF Range;
				public CurveCubicSpline1F Curve;

				public float GenerateValue( FastRandom random )
				{
					switch( Type )
					{
					case ParticleEmitter.SingleProperty.TypeEnum.Constant:
						return Constant;

					case ParticleEmitter.SingleProperty.TypeEnum.Range:
						return MathEx.Lerp( Range.Minimum, Range.Maximum, random.NextFloat() );

					case ParticleEmitter.SingleProperty.TypeEnum.Curve:
						if( Curve != null )
						{
							var maxValue = Curve.Points[ Curve.Points.Count - 1 ].Time;
							return Curve.CalculateValueByTime( random.Next( maxValue ) );
						}
						break;
					}

					return 0;
				}

				//public bool Equals( ref SingleProperty obj )
				//{
				//	return true;
				//}
			}

			////////////

			public struct IntegerProperty
			{
				public ParticleEmitter.IntegerProperty.TypeEnum Type;
				public int Constant;
				public RangeI Range;
				//!!!!Integer?
				public CurveCubicSpline1F Curve;

				public int GenerateValue( FastRandom random )
				{
					switch( Type )
					{
					case ParticleEmitter.IntegerProperty.TypeEnum.Constant:
						return Constant;

					case ParticleEmitter.IntegerProperty.TypeEnum.Range:
						return (int)MathEx.Lerp( Range.Minimum, Range.Maximum, random.NextFloat() );

					case ParticleEmitter.IntegerProperty.TypeEnum.Curve:
						if( Curve != null )
						{
							var maxValue = Curve.Points[ Curve.Points.Count - 1 ].Time;
							return (int)Curve.CalculateValueByTime( random.Next( maxValue ) );
						}
						break;
					}

					return 0;
				}
			}

			////////////

			public struct AnglesProperty
			{
				public ParticleEmitter.AnglesProperty.TypeEnum Type;
				public Vector3F Constant;
				public RangeVector3F Range;
				public CurveCubicSpline1F CurveX;
				public CurveCubicSpline1F CurveY;
				public CurveCubicSpline1F CurveZ;

				public Vector3F GenerateValue( FastRandom random )
				{
					switch( Type )
					{
					case ParticleEmitter.AnglesProperty.TypeEnum.Constant:
						return Constant;

					case ParticleEmitter.AnglesProperty.TypeEnum.Range:
						return Vector3F.Lerp( Range.Minimum, Range.Maximum, random.NextFloat() );

					case ParticleEmitter.AnglesProperty.TypeEnum.Curve:
						{
							Vector3F result = Vector3F.Zero;
							if( CurveX != null )
							{
								var maxValue = CurveX.Points[ CurveX.Points.Count - 1 ].Time;
								result.X = CurveX.CalculateValueByTime( random.Next( maxValue ) );
							}
							if( CurveY != null )
							{
								var maxValue = CurveY.Points[ CurveY.Points.Count - 1 ].Time;
								result.Y = CurveY.CalculateValueByTime( random.Next( maxValue ) );
							}
							if( CurveZ != null )
							{
								var maxValue = CurveZ.Points[ CurveZ.Points.Count - 1 ].Time;
								result.Z = CurveZ.CalculateValueByTime( random.Next( maxValue ) );
							}
							return result;
						}
					}

					return Vector3F.Zero;
				}
			}

			////////////

			public struct ColorValueProperty
			{
				public ParticleEmitter.ColorValueProperty.TypeEnum Type;
				public ColorValue Constant;
				public RangeColorValue Range;
				//public CurveCubicSpline1F curve;

				public void GenerateValue( FastRandom random, out ColorValue result )
				{
					switch( Type )
					{
					case ParticleEmitter.ColorValueProperty.TypeEnum.Constant:
						result = Constant;
						break;

					case ParticleEmitter.ColorValueProperty.TypeEnum.Range:
						ColorValue.Lerp( ref Range.Minimum, ref Range.Maximum, random.NextFloat(), out result );
						break;

					//case ParticleEmitter.ColorValuePropertys.TypeEnum.Curve:
					//	if( Curve.Points.Count != 0 )
					//	{
					//		var maxValue = Curve.Points[ Curve.Points.Count - 1 ].Time;
					//		return (float)Curve.CalculateValueByTime( random.Next( maxValue ) );
					//	}
					//	break;

					default:
						result = ColorValue.Zero;
						break;
					}
				}
			}

			////////////

			public CompiledData( ParticleSystem owner )
			{
				Owner = owner;

				Looping = owner.Looping;
				//MaxParticles = owner.MaxParticles;
				SimulationSpace = owner.SimulationSpace;
				//SimulationSpeed = owner.SimulationSpeed;
				//VisibilityDistance = owner.VisibilityDistance;

				var emitters = new List<Emitter>();
				foreach( var emitter in owner.GetComponents<ParticleEmitter>() )
				{
					if( emitter.Enabled )
					{
						var compiledEmitter = new Emitter();
						compiledEmitter.Owner = emitter;
						//compiledEmitter.Owner = emitter;

						//compiledEmitter.VisibilityDistance = emitter.VisibilityDistance;
						compiledEmitter.CastShadows = emitter.CastShadows;
						compiledEmitter.ReceiveDecals = emitter.ReceiveDecals;
						compiledEmitter.MotionBlurFactor = (float)emitter.MotionBlurFactor;

						//properties

						compiledEmitter.StartTime = CreateSingleProperty( emitter.StartTime );
						compiledEmitter.Duration = CreateSingleProperty( emitter.Duration );
						//compiledEmitter.Looping = emitter.Looping;

						//compiledEmitter.Probability = CreateSingleProperty( emitter.Probability );
						compiledEmitter.SpawnRate = CreateSingleProperty( emitter.SpawnRate );
						compiledEmitter.SpawnCount = CreateIntegerProperty( emitter.SpawnCount );
						compiledEmitter.Direction = emitter.Direction;
						compiledEmitter.DispersionAngle = CreateSingleProperty( emitter.DispersionAngle );
						compiledEmitter.Speed = CreateSingleProperty( emitter.Speed );
						compiledEmitter.RotateAlongMovement = emitter.RotateAlongMovement;
						compiledEmitter.Rotation = CreateAnglesProperty( emitter.Rotation );
						compiledEmitter.AngularVelocity = CreateAnglesProperty( emitter.AngularVelocity );

						compiledEmitter.Lifetime = CreateSingleProperty( emitter.Lifetime );
						compiledEmitter.Size = CreateSingleProperty( emitter.Size );
						compiledEmitter.GravityMultiplier = CreateSingleProperty( emitter.GravityMultiplier );

						compiledEmitter.Mesh = emitter.Mesh;
						compiledEmitter.Material = emitter.Material;
						compiledEmitter.Color = CreateColorValueProperty( emitter.Color );

						//components

						var shapes = new List<ParticleEmitterShape>();
						var linearAccelerationByTimes = new List<Emitter.AccelerationByTime>();
						var angularAccelerationByTimes = new List<Emitter.AccelerationByTime>();
						var colorMultiplierByTimes = new List<Emitter.ColorMultiplierByTime>();
						var sizeMultiplierByTimes = new List<Emitter.SizeMultiplierByTime>();
						var linearSpeedMultiplierByTimes = new List<Emitter.SpeedMultiplierByTime>();
						var angularSpeedMultiplierByTimes = new List<Emitter.SpeedMultiplierByTime>();
						var linearVelocityByTimes = new List<Emitter.VelocityByTime>();
						var angularVelocityByTimes = new List<Emitter.VelocityByTime>();
						var customModules = new List<ParticleModuleCustom>();

						foreach( var child in emitter.GetComponents() )
						{
							if( child.Enabled )
							{
								//shape
								var shape = child as ParticleEmitterShape;
								if( shape != null )
								{
									shapes.Add( shape );

									//var compiledShape = new Emitter.Shape();
									//compiledShape.Component = shape;
									//shapes.Add( compiledShape );

									//var point = shape as ParticleEmitterShape_Point;
									//if( point != null )
									//{
									//	var compiledShape = new Emitter.Shape();
									//	compiledShape.Type = Emitter.Shape.TypeEnum.Point;
									//	compiledShape.Transform = shape.Transform;
									//	compiledShape.Probability = shape.Probability;
									//	shapes.Add( compiledShape );
									//}

									//var sphere = shape as ParticleEmitterShape_Sphere;
									//if( sphere != null )
									//{
									//	var compiledShape = new Emitter.Shape();
									//	compiledShape.Type = Emitter.Shape.TypeEnum.Sphere;
									//	compiledShape.Transform = shape.Transform;
									//	compiledShape.Probability = shape.Probability;
									//	compiledShape.Radius = sphere.Radius;
									//	compiledShape.Thickness = sphere.Thickness;
									//	shapes.Add( compiledShape );
									//}
								}

								//linear acceleration by time
								{
									var source = child as ParticleLinearAccelerationByTime;
									if( source != null )
									{
										var to = new Emitter.AccelerationByTime();
										to.Type = source.Type;
										to.Constant = source.Constant;
										to.Range = source.Range;
										to.CurveX = CreateCurve( source.CurveX );
										to.CurveY = CreateCurve( source.CurveY );
										to.CurveZ = CreateCurve( source.CurveZ );
										//to.LockedAxes = source.LockedAxes;
										linearAccelerationByTimes.Add( to );
									}
								}

								//angular acceleration by time
								{
									var source = child as ParticleAngularAccelerationByTime;
									if( source != null )
									{
										var to = new Emitter.AccelerationByTime();
										to.Type = source.Type;
										to.Constant = source.Constant;
										to.Range = source.Range;
										to.CurveX = CreateCurve( source.CurveX );
										to.CurveY = CreateCurve( source.CurveY );
										to.CurveZ = CreateCurve( source.CurveZ );
										//to.LockedAxes = source.LockedAxes;
										angularAccelerationByTimes.Add( to );
									}
								}

								//color multiplier by time
								{
									var source = child as ParticleColorMultiplierByTime;
									if( source != null )
									{
										var to = new Emitter.ColorMultiplierByTime();
										to.Channels = source.Channels;
										to.Type = source.Type;
										to.Range = source.Range;
										to.Curve = CreateCurve( source.Curve );
										colorMultiplierByTimes.Add( to );
									}
								}

								//size multiplier by time
								{
									var source = child as ParticleSizeMultiplierByTime;
									if( source != null )
									{
										var to = new Emitter.SizeMultiplierByTime();
										to.Axes = source.Axes;
										to.Type = source.Type;
										to.Range = source.Range;
										to.Curve = CreateCurve( source.Curve );
										sizeMultiplierByTimes.Add( to );
									}
								}

								//linear speed multiplier by time
								{
									var source = child as ParticleLinearSpeedMultiplierByTime;
									if( source != null )
									{
										var to = new Emitter.SpeedMultiplierByTime();
										to.Type = source.Type;
										to.Constant = source.Constant;
										to.Range = source.Range;
										to.Curve = CreateCurve( source.Curve );
										linearSpeedMultiplierByTimes.Add( to );
									}
								}

								//angular speed multiplier by time
								{
									var source = child as ParticleAngularSpeedMultiplierByTime;
									if( source != null )
									{
										var to = new Emitter.SpeedMultiplierByTime();
										to.Type = source.Type;
										to.Constant = source.Constant;
										to.Range = source.Range;
										to.Curve = CreateCurve( source.Curve );
										angularSpeedMultiplierByTimes.Add( to );
									}
								}

								//linear velocity by time
								{
									var source = child as ParticleLinearVelocityByTime;
									if( source != null )
									{
										var to = new Emitter.VelocityByTime();
										to.Type = source.Type;
										to.Constant = source.Constant;
										to.Range = source.Range;
										to.CurveX = CreateCurve( source.CurveX );
										to.CurveY = CreateCurve( source.CurveY );
										to.CurveZ = CreateCurve( source.CurveZ );
										linearVelocityByTimes.Add( to );
									}
								}

								//angular velocity by time
								{
									var source = child as ParticleAngularVelocityByTime;
									if( source != null )
									{
										var to = new Emitter.VelocityByTime();
										to.Type = source.Type;
										to.Constant = source.Constant;
										to.Range = source.Range;
										to.CurveX = CreateCurve( source.CurveX );
										to.CurveY = CreateCurve( source.CurveY );
										to.CurveZ = CreateCurve( source.CurveZ );
										angularVelocityByTimes.Add( to );
									}
								}

								//custom module
								var customModule = child as ParticleModuleCustom;
								if( customModule != null )
									customModules.Add( customModule );
							}
						}

						compiledEmitter.Shapes = shapes.ToArray();

						compiledEmitter.LinearAccelerationByTimes = linearAccelerationByTimes.Count != 0 ? linearAccelerationByTimes.ToArray() : Array.Empty<Emitter.AccelerationByTime>();
						compiledEmitter.AngularAccelerationByTimes = angularAccelerationByTimes.Count != 0 ? angularAccelerationByTimes.ToArray() : Array.Empty<Emitter.AccelerationByTime>();

						compiledEmitter.ColorMultiplierByTimes = colorMultiplierByTimes.Count != 0 ? colorMultiplierByTimes.ToArray() : Array.Empty<Emitter.ColorMultiplierByTime>();
						compiledEmitter.SizeMultiplierByTimes = sizeMultiplierByTimes.Count != 0 ? sizeMultiplierByTimes.ToArray() : Array.Empty<Emitter.SizeMultiplierByTime>();

						compiledEmitter.LinearSpeedMultiplierByTimes = linearSpeedMultiplierByTimes.Count != 0 ? linearSpeedMultiplierByTimes.ToArray() : Array.Empty<Emitter.SpeedMultiplierByTime>();
						compiledEmitter.AngularSpeedMultiplierByTimes = angularSpeedMultiplierByTimes.Count != 0 ? angularSpeedMultiplierByTimes.ToArray() : Array.Empty<Emitter.SpeedMultiplierByTime>();

						compiledEmitter.LinearVelocityByTimes = linearVelocityByTimes.Count != 0 ? linearVelocityByTimes.ToArray() : Array.Empty<Emitter.VelocityByTime>();
						compiledEmitter.AngularVelocityByTimes = angularVelocityByTimes.Count != 0 ? angularVelocityByTimes.ToArray() : Array.Empty<Emitter.VelocityByTime>();

						compiledEmitter.CustomModules = customModules.Count != 0 ? customModules.ToArray() : Array.Empty<ParticleModuleCustom>();

						emitters.Add( compiledEmitter );
					}
				}
				Emitters = emitters.ToArray();

			}

			public virtual void Dispose()
			{
			}

			//static float CurveCalculateValueByTime( Curve1F curve, float time )
			//{
			//var time2 = time;
			//if( time2 < curve.points[ 0 ].Time )
			//	time2 = curve.points[ 0 ].Time;
			//else if( time2 > curve.points[ curve.points.Count - 1 ].Time )
			//	time2 = curve.points[ curve.points.Count - 1 ].Time;
			//	return curve.CalculateValueByTime( time );
			//}
		}

		/////////////////////////////////////////

		//static CurveCubicSpline1F CreateCurve( List<CurvePoint> curve )
		//{
		//	if( curve != null && curve.Count != 0 )
		//	{
		//		var result = new CurveCubicSpline1F();
		//		foreach( var point in curve )
		//		{
		//			if( result.GetPointIndexByTime( point.Point ) == -1 )
		//				result.AddPoint( point.Point, point.Value );
		//		}
		//		return result;
		//	}
		//	else
		//		return null;
		//}

		static CurveCubicSpline1F CreateCurve( ReferenceList<CurvePoint> curve )
		{
			if( curve != null && curve.Count != 0 )
			{
				var result = new CurveCubicSpline1F();
				foreach( var point in curve )
				{
					if( result.GetPointIndexByTime( point.Value.Point ) == -1 )
						result.AddPoint( point.Value.Point, point.Value.Value );
				}
				return result;
			}
			else
				return null;
		}

		static CompiledData.SingleProperty CreateSingleProperty( ParticleEmitter.SingleProperty value )
		{
			var result = new CompiledData.SingleProperty();
			result.Type = value.Type;

			switch( value.Type )
			{
			case ParticleEmitter.SingleProperty.TypeEnum.Constant:
				result.Constant = value.Constant;
				break;

			case ParticleEmitter.SingleProperty.TypeEnum.Range:
				result.Range = value.Range;
				break;

			case ParticleEmitter.SingleProperty.TypeEnum.Curve:
				result.Curve = CreateCurve( value.Curve );
				break;
			}

			return result;
		}

		static CompiledData.IntegerProperty CreateIntegerProperty( ParticleEmitter.IntegerProperty value )
		{
			var result = new CompiledData.IntegerProperty();
			result.Type = value.Type;

			switch( value.Type )
			{
			case ParticleEmitter.IntegerProperty.TypeEnum.Constant:
				result.Constant = value.Constant;
				break;

			case ParticleEmitter.IntegerProperty.TypeEnum.Range:
				result.Range = value.Range;
				break;

			case ParticleEmitter.IntegerProperty.TypeEnum.Curve:
				result.Curve = CreateCurve( value.Curve );
				break;
			}

			return result;
		}

		static CompiledData.AnglesProperty CreateAnglesProperty( ParticleEmitter.AnglesProperty value )
		{
			var result = new CompiledData.AnglesProperty();
			result.Type = value.Type;

			switch( value.Type )
			{
			case ParticleEmitter.AnglesProperty.TypeEnum.Constant:
				result.Constant = value.Constant;
				break;

			case ParticleEmitter.AnglesProperty.TypeEnum.Range:
				result.Range = value.Range;
				break;

			case ParticleEmitter.AnglesProperty.TypeEnum.Curve:
				result.CurveX = CreateCurve( value.CurveX );
				result.CurveY = CreateCurve( value.CurveY );
				result.CurveZ = CreateCurve( value.CurveZ );
				break;
			}

			return result;
		}

		static CompiledData.ColorValueProperty CreateColorValueProperty( ParticleEmitter.ColorValueProperty value )
		{
			var result = new CompiledData.ColorValueProperty();
			result.Type = value.Type;

			switch( value.Type )
			{
			case ParticleEmitter.ColorValueProperty.TypeEnum.Constant:
				result.Constant = value.Constant;
				break;

			case ParticleEmitter.ColorValueProperty.TypeEnum.Range:
				result.Range = value.Range;
				break;

				//case ParticleEmitter.IntegerProperty.TypeEnum.Curve:
				//	result.curve = new CurveCubicSpline1F();
				//	foreach( var point in value.Curve )
				//		result.curve.AddPoint( new Curve.Point( point.Point, new Vector3( point.Value, 0, 0 ) ) );
				//	break;
			}

			return result;
		}

		protected override void OnResultCompile()
		{
			if( Result == null )
			{
				var compiledData = new CompiledData( this );
				Result = compiledData;
			}
		}

		[Browsable( false )]
		public int MustRecreateInstancesCounter
		{
			get { return mustRecreateInstancesCounter; }
		}

		public void MustRecreateInstances()
		{
			unchecked
			{
				mustRecreateInstancesCounter++;
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			var emitter = CreateComponent<ParticleEmitter>();
			emitter.Name = "Emitter";
			emitter.Material = ReferenceUtility.MakeReference( @"Base\Components\Particle system default.material" );
			emitter.DispersionAngle = new ParticleEmitter.SingleProperty( 10 );

			var shape = emitter.CreateComponent<ParticleEmitterShape_Point>();
			shape.Name = "Point Shape";
			shape.Transform = new Transform( Vector3.Zero, new Angles( 0, 90, 0 ).ToQuaternion() );

			var colorMultiplier = emitter.CreateComponent<ParticleColorMultiplierByTime>();
			colorMultiplier.Name = "Color Multiplier By Time";
			colorMultiplier.Channels = ParticleColorMultiplierByTime.ChannelsEnum.Alpha;
			colorMultiplier.Type = ParticleColorMultiplierByTime.TypeEnum.Curve;
			colorMultiplier.Curve.Add( new CurvePoint( 0, 0 ) );
			colorMultiplier.Curve.Add( new CurvePoint( 1, 1 ) );
			colorMultiplier.Curve.Add( new CurvePoint( 2, 1 ) );
			colorMultiplier.Curve.Add( new CurvePoint( 3, 0 ) );
		}

	}
}
