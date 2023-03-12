//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;
//using System.Linq;
//using NeoAxis.Editor;

//!!!!name 'Plant'

//namespace NeoAxis
//{
//	/// <summary>
//	/// A procedural generated mesh by specific plant type.
//	/// </summary>
//	[AddToResourcesWindow( @"Addons\Vegetation\Plant Mesh", 10020 )]
//	public class PlantMesh : Mesh
//	{
//		//[Category( "Generator: General" )]
//		[DefaultValue( null )]
//		public Reference<PlantType> PlantType
//		{
//			get { if( _plantType.BeginGet() ) PlantType = _plantType.Get( this ); return _plantType.value; }
//			set { if( _plantType.BeginSet( ref value ) ) { try { PlantTypeChanged?.Invoke( this ); ShouldRecompile = true; } finally { _plantType.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="PlantType"/> property value changes.</summary>
//		public event Action<PlantMesh> PlantTypeChanged;
//		ReferenceField<PlantType> _plantType = null;

//		//!!!!null support, null by default?
//		//[Category( "Generator: General" )]
//		[DefaultValue( 0 )]
//		[Range( 0, 100 )]
//		public Reference<int> Seed
//		{
//			get { if( _seed.BeginGet() ) Seed = _seed.Get( this ); return _seed.value; }
//			set { if( _seed.BeginSet( ref value ) ) { try { SeedChanged?.Invoke( this ); ShouldRecompile = true; } finally { _seed.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Seed"/> property value changes.</summary>
//		public event Action<PlantMesh> SeedChanged;
//		ReferenceField<int> _seed = 0;

//		//!!!!
//		///// <summary>
//		///// Specifies the quality for procedural generation of 3D model. A value of 0.5 is optimal for real-time graphics. The higher values are more applicable for non real-time rendering.
//		///// </summary>
//		//[Category( "Output" )]
//		////[Category( "Generator: General" )]
//		//[DefaultValue( 0.5 )]
//		//[Range( 0, 1 )]
//		//public Reference<double> Quality
//		//{
//		//	get { if( _quality.BeginGet() ) Quality = _quality.Get( this ); return _quality.value; }
//		//	set { if( _quality.BeginSet( ref value ) ) { try { QualityChanged?.Invoke( this ); ShouldRecompile = true; } finally { _quality.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="Quality"/> property value changes.</summary>
//		//public event Action<PlantMesh> QualityChanged;
//		//ReferenceField<double> _quality = 0.5;

//		/// <summary>
//		/// Whether to generate levels of detail.
//		/// </summary>
//		[Category( "Output" )]
//		[DisplayName( "LODs" )]
//		[DefaultValue( true )]
//		public Reference<bool> LODs
//		{
//			get { if( _lODs.BeginGet() ) LODs = _lODs.Get( this ); return _lODs.value; }
//			set { if( _lODs.BeginSet( ref value ) ) { try { LODsChanged?.Invoke( this ); ShouldRecompile = true; } finally { _lODs.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="LODs"/> property value changes.</summary>
//		public event Action<PlantMesh> LODsChanged;
//		ReferenceField<bool> _lODs = true;

//		//!!!!может дополнительно калибровать
//		///// <summary>
//		///// The distance from the previous to the next level of detail.
//		///// </summary>
//		//[Category( "Output" )]
//		//[DisplayName( "LOD Distance" )]
//		//[DefaultValue( 10.0 )]
//		//[Range( 1, 50, RangeAttribute.ConvenientDistributionEnum.Exponential )]
//		//public Reference<double> LODDistance
//		//{
//		//	get { if( _lODDistance.BeginGet() ) LODDistance = _lODDistance.Get( this ); return _lODDistance.value; }
//		//	set { if( _lODDistance.BeginSet( ref value ) ) { try { LODDistanceChanged?.Invoke( this ); ShouldRecompile = true; } finally { _lODDistance.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="LODDistance"/> property value changes.</summary>
//		//public event Action<PlantMesh> LODDistanceChanged;
//		//ReferenceField<double> _lODDistance = 10.0;


//		//!!!!null parameters support?

//		//MatureAge when 0
//		//[Category( "Generator: General" )]
//		[DefaultValue( 0.0 )]
//		[Range( 0.0, 100.0, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
//		public Reference<double> Age
//		{
//			get { if( _age.BeginGet() ) Age = _age.Get( this ); return _age.value; }
//			set { if( _age.BeginSet( ref value ) ) { try { AgeChanged?.Invoke( this ); ShouldRecompile = true; } finally { _age.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Age"/> property value changes.</summary>
//		public event Action<PlantMesh> AgeChanged;
//		ReferenceField<double> _age = 0.0;

//		////MatureAge when null 
//		//[Category( "Generator: General" )]
//		//[DefaultValue( null )]
//		//[Range( 0.1, 100.0, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
//		//public Reference<double?> Age
//		//{
//		//	get { if( _age.BeginGet() ) Age = _age.Get( this ); return _age.value; }
//		//	set { if( _age.BeginSet( ref value ) ) { try { AgeChanged?.Invoke( this ); ShouldRecompile = true; } finally { _age.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="Age"/> property value changes.</summary>
//		//public event Action<PlantMesh> AgeChanged;
//		//ReferenceField<double?> _age = null;

//		//[Category( "Generator: General" )]
//		[DefaultValue( 0.0 )]
//		[Range( 0, 1 )]
//		public Reference<double> Dead
//		{
//			get { if( _dead.BeginGet() ) Dead = _dead.Get( this ); return _dead.value; }
//			set { if( _dead.BeginSet( ref value ) ) { try { DeadChanged?.Invoke( this ); ShouldRecompile = true; } finally { _dead.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Dead"/> property value changes.</summary>
//		public event Action<PlantMesh> DeadChanged;
//		ReferenceField<double> _dead = 0.0;

//		//[Category( "Generator: General" )]
//		[DefaultValue( 0.0 )]
//		[Range( 0, 1 )]
//		public Reference<double> Fired
//		{
//			get { if( _fired.BeginGet() ) Fired = _fired.Get( this ); return _fired.value; }
//			set { if( _fired.BeginSet( ref value ) ) { try { FiredChanged?.Invoke( this ); ShouldRecompile = true; } finally { _fired.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Fired"/> property value changes.</summary>
//		public event Action<PlantMesh> FiredChanged;
//		ReferenceField<double> _fired = 0.0;


//		//

//		//!!!!

//		//[Category( "Generator: Environment" )]
//		//[DefaultValue( null )]
//		//[Range( 0, 1 )]
//		//public Reference<double?> MineralsQuality
//		//{
//		//	get { if( _mineralsQuality.BeginGet() ) MineralsQuality = _mineralsQuality.Get( this ); return _mineralsQuality.value; }
//		//	set { if( _mineralsQuality.BeginSet( ref value ) ) { try { MineralsQualityChanged?.Invoke( this ); ShouldRecompile = true; } finally { _mineralsQuality.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="MineralsQuality"/> property value changes.</summary>
//		//public event Action<PlantMesh> MineralsQualityChanged;
//		//ReferenceField<double?> _mineralsQuality = null;

//		//[Category( "Generator: Environment" )]
//		//[DefaultValue( null )]
//		//[Range( 0, 1 )]
//		//public Reference<double?> AverageSolarEnergy
//		//{
//		//	get { if( _averageSolarEnergy.BeginGet() ) AverageSolarEnergy = _averageSolarEnergy.Get( this ); return _averageSolarEnergy.value; }
//		//	set { if( _averageSolarEnergy.BeginSet( ref value ) ) { try { AverageSolarEnergyChanged?.Invoke( this ); ShouldRecompile = true; } finally { _averageSolarEnergy.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="AverageSolarEnergy"/> property value changes.</summary>
//		//public event Action<PlantMesh> AverageSolarEnergyChanged;
//		//ReferenceField<double?> _averageSolarEnergy = null;

//		//[Category( "Generator: Environment" )]
//		//[DefaultValue( null )]
//		//[Range( 0, 1 )]
//		//public Reference<double?> AverageHumidity
//		//{
//		//	get { if( _averageAverageHumidity.BeginGet() ) AverageHumidity = _averageAverageHumidity.Get( this ); return _averageAverageHumidity.value; }
//		//	set { if( _averageAverageHumidity.BeginSet( ref value ) ) { try { AverageHumidityChanged?.Invoke( this ); ShouldRecompile = true; } finally { _averageAverageHumidity.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="AverageHumidity"/> property value changes.</summary>
//		//public event Action<PlantMesh> AverageHumidityChanged;
//		//ReferenceField<double?> _averageAverageHumidity = null;


//		//!!!!null parameters support?

//		/// <summary>
//		/// Season of a year. 0 - summer, 1 - fall, 2 - winter, 3 - spring.
//		/// </summary>
//		[DefaultValue( null )]
//		[Range( 0, 4 )]
//		public Reference<double> Season
//		{
//			get { if( _season.BeginGet() ) Season = _season.Get( this ); return _season.value; }
//			set { if( _season.BeginSet( ref value ) ) { try { SeasonChanged?.Invoke( this ); ShouldRecompile = true; } finally { _season.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Season"/> property value changes.</summary>
//		public event Action<PlantMesh> SeasonChanged;
//		ReferenceField<double> _season = 0.0;

//		///// <summary>
//		///// Season of a year. 0 - summer, 1 - fall, 2 - winter, 3 - spring.
//		///// </summary>
//		//[Category( "Generator: Environment" )]
//		//[DefaultValue( null )]
//		//[Range( 0, 4 )]
//		//public Reference<double?> Season
//		//{
//		//	get { if( _season.BeginGet() ) Season = _season.Get( this ); return _season.value; }
//		//	set { if( _season.BeginSet( ref value ) ) { try { SeasonChanged?.Invoke( this ); ShouldRecompile = true; } finally { _season.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="Season"/> property value changes.</summary>
//		//public event Action<PlantMesh> SeasonChanged;
//		//ReferenceField<double?> _season = null;

//		//!!!!
//		[DefaultValue( 0.0 )]
//		[Range( 0, 1 )]
//		public Reference<double> Lichen
//		{
//			get { if( _lichen.BeginGet() ) Lichen = _lichen.Get( this ); return _lichen.value; }
//			set { if( _lichen.BeginSet( ref value ) ) { try { LichenChanged?.Invoke( this ); } finally { _lichen.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Lichen"/> property value changes.</summary>
//		public event Action<PlantMesh> LichenChanged;
//		ReferenceField<double> _lichen = 0.0;

//		//!!!!

//		//[Category( "Generator: Environment" )]
//		//[DefaultValue( null )]
//		//[Range( -40, 60 )]
//		//[DisplayName( "Temperature °C" )]
//		//public Reference<double?> Temperature
//		//{
//		//	get { if( _temperature.BeginGet() ) Temperature = _temperature.Get( this ); return _temperature.value; }
//		//	set { if( _temperature.BeginSet( ref value ) ) { try { TemperatureChanged?.Invoke( this ); } finally { _temperature.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="Temperature"/> property value changes.</summary>
//		//public event Action<PlantMesh> TemperatureChanged;
//		//ReferenceField<double?> _temperature = null;

//		//[Category( "Generator: Environment" )]
//		//[DefaultValue( null )]
//		//[Range( 0, 1 )]
//		//public Reference<double?> Humidity
//		//{
//		//	get { if( _humidity.BeginGet() ) Humidity = _humidity.Get( this ); return _humidity.value; }
//		//	set { if( _humidity.BeginSet( ref value ) ) { try { HumidityChanged?.Invoke( this ); } finally { _humidity.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="Humidity"/> property value changes.</summary>
//		//public event Action<PlantMesh> HumidityChanged;
//		//ReferenceField<double?> _humidity = null;

//		//[Category( "Generator: Environment" )]
//		//[DefaultValue( null )]
//		//[Range( 0, 1 )]
//		//public Reference<double?> Precipitation
//		//{
//		//	get { if( _precipitation.BeginGet() ) Precipitation = _precipitation.Get( this ); return _precipitation.value; }
//		//	set { if( _precipitation.BeginSet( ref value ) ) { try { PrecipitationChanged?.Invoke( this ); } finally { _precipitation.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="Precipitation"/> property value changes.</summary>
//		//public event Action<PlantMesh> PrecipitationChanged;
//		//ReferenceField<double?> _precipitation = null;


//		//!!!!

//		///// <summary>
//		///// The range of levels of detail to generate.
//		///// </summary>
//		//[Category( "Generator" )]
//		//[DefaultValue( null )]//[DefaultValue( "0 3" )]
//		//[Range( 0, 3 )]
//		//[DisplayName( "LOD Range" )]
//		//public Reference<RangeI?> LODRange
//		//{
//		//	get { if( _lODRange.BeginGet() ) LODRange = _lODRange.Get( this ); return _lODRange.value; }
//		//	set { if( _lODRange.BeginSet( ref value ) ) { try { LODRangeChanged?.Invoke( this ); ShouldRecompile = true; } finally { _lODRange.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="LODRange"/> property value changes.</summary>
//		//public event Action<Plant> LODRangeChanged;
//		//ReferenceField<RangeI?> _lODRange = null;//new RangeI( 0, 3 );

//		//

//		///////////////////////////////////////////////

//		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
//		{
//			base.OnMetadataGetMembersFilter( context, member, ref skip );

//			if( member is Metadata.Property )
//			{
//				switch( member.Name )
//				{
//				case nameof( Skeleton ):
//				case nameof( Billboard ):
//					skip = true;
//					break;

//					//case nameof( LODDistance ):
//					//	if( !LODs )
//					//		skip = true;
//					//	break;
//				}
//			}
//		}

//		protected override void OnMeshCompile( CompiledData compiledData )
//		{
//			base.OnMeshCompile( compiledData );

//			if( PlantType.Value != null )
//			{
//				int lodLevels = 1;
//				if( LODs )
//					lodLevels = 3;//No billboard LOD // 4;

//				if( lodLevels != 1 )
//					compiledData.MeshData.LODs = new RenderingPipeline.RenderSceneData.IMeshDataLODLevel[ lodLevels - 1 ];

//				for( int lod = 0; lod < lodLevels; lod++ )
//				{
//					if( lod == 0 )
//					{

//						//!!!!billboard

//						var generator = new PlantGenerator( PlantType, Seed, Age/*, Dead, Fired, Season*/, lod/*, false*/, Transform.Identity, compiledData, null );
//						generator.Generate();
//					}
//					else
//					{
//						var mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );

//						//!!!!billboard

//						var generator = new PlantGenerator( PlantType, Seed, Age/*, Dead, Fired, Season*/, lod/*, lod == 3*/, Transform.Identity, null, mesh );
//						generator.Generate();

//						//!!!!
//						//mesh.Billboard = generator.LODBillboard;
//						//if( mesh.Billboard )
//						//	mesh.BillboardShadowOffset = 0;

//						mesh.Enabled = true;

//						var lodData = new RenderingPipeline.RenderSceneData.IMeshDataLODLevel();
//						lodData.Mesh = mesh;
//						lodData.Distance = (float)PlantType.Value.CalculateLODDistance( generator.Bounds, lod );
//						//lodData.DistanceSquared = (float)Math.Pow( generator.CalculateLODDistance( lod ), 2 );
//						compiledData.MeshData.LODs[ lod - 1 ] = lodData;

//						//dispose the mesh when the compiled data disposed
//						compiledData.ObjectsToDispose.Add( mesh );
//					}
//				}
//			}
//		}
//	}
//}



////public enum ParameterEnum
////{
////	//MatureAge,

////	TrunksTransforms,
////	//Trunks,
////	//TrunksSeedingRadius,

////	//!!!!
////	//GetElementData,//параметры

////	//!!!!Trunk максимальный наклон. числом для всех или выдавать каждому
////}

////public virtual object OnGetParameter( Generator generator, ParameterEnum param )
////{
////	return null;
////}

////public delegate void GetParameterEventDelegate( PlantMesh sender, Generator generator, ParameterEnum param, ref object value );
////public event GetParameterEventDelegate GetParameterEvent;

////public object GetParameter( Generator generator, ParameterEnum param )
////{
////	object value = null;
////	GetParameterEvent?.Invoke( this, generator, param, ref value );
////	if( value != null )
////		return value;

////	value = OnGetParameter( generator, param );
////	if( value != null )
////		return value;

////	//default
////	//switch( param )
////	//{
////	//!!!!
////	//case ParameterEnum.Trunks: return 1;

////	//case ParameterEnum.TrunksTransforms:
////	//	break;

////	//case ParameterEnum.TrunksSeedingRadius:
////	//	break;
////	//}

////	return null;
////}

////public T GetParameter<T>( Generator generator, ParameterEnum param )
////{
////	var value = GetParameter( generator, param );

////	if( value != null )
////	{
////		if( typeof( T ).IsAssignableFrom( value.GetType() ) )
////			return (T)value;

////		if( typeof( T ) == typeof( int ) )
////		{
////			if( value.GetType() == typeof( double ) )
////				return (T)(object)(int)(double)value;
////			if( value.GetType() == typeof( float ) )
////				return (T)(object)(int)(float)value;
////		}
////		else if( typeof( T ) == typeof( float ) )
////		{
////			if( value.GetType() == typeof( double ) )
////				return (T)(object)(float)(double)value;
////			if( value.GetType() == typeof( int ) )
////				return (T)(object)(float)(int)value;
////		}
////		else if( typeof( T ) == typeof( double ) )
////		{
////			if( value.GetType() == typeof( float ) )
////				return (T)(object)(double)(float)value;
////			if( value.GetType() == typeof( int ) )
////				return (T)(object)(double)(int)value;
////		}
////	}

////	return default;
////}
