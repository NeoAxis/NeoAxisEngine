// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Represents a type of the plant.
	/// </summary>
	[ResourceFileExtension( "planttype" )]
	[AddToResourcesWindow( @"Addons\Plant\Plant Type", 10000 )]
	[SettingsCell( typeof( PlantSettingsCell ) )]
	[EditorControl( typeof( PlantEditor ) )]
	public class PlantType : Component
	{
		public delegate void GenerateStageDelegate( PlantType sender, PlantGenerator generator, PlantGenerator.ElementTypeEnum stage );
		public event GenerateStageDelegate GenerateStage;

		internal void PerformGenerateStage( PlantGenerator generator, PlantGenerator.ElementTypeEnum stage )
		{
			GenerateStage?.Invoke( this, generator, stage );
		}

		///////////////////////////////////////////////

		/// <summary>
		/// The age at which active growth ceases.
		/// </summary>
		//[Category( "General" )]
		[DefaultValue( 1.0 )]
		public Reference<double> MatureAge
		{
			get { if( _matureAge.BeginGet() ) MatureAge = _matureAge.Get( this ); return _matureAge.value; }
			set { if( _matureAge.BeginSet( ref value ) ) { try { MatureAgeChanged?.Invoke( this ); } finally { _matureAge.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MatureAge"/> property value changes.</summary>
		public event Action<PlantType> MatureAgeChanged;
		ReferenceField<double> _matureAge = 1.0;

		/// <summary>
		/// The height range at mature age.
		/// </summary>
		[DefaultValue( "1 1" )]
		[Range( 0.1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<Range> MatureHeight
		{
			get { if( _matureHeight.BeginGet() ) MatureHeight = _matureHeight.Get( this ); return _matureHeight.value; }
			set { if( _matureHeight.BeginSet( ref value ) ) { try { MatureHeightChanged?.Invoke( this ); } finally { _matureHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MatureHeight"/> property value changes.</summary>
		public event Action<PlantType> MatureHeightChanged;
		ReferenceField<Range> _matureHeight = new Range( 1, 1 );

		public enum PredefinedConfigurationEnum
		{
			None,
			//QuercusRobur,
			//MatricariaChamomilla,//Chamomile,
			//DoronicumGrandiflorum,
			//AlopecurusPratensis,
			//PoaAnnua,
			//TanacetumCoccineum,
			//CyperusEsculentus,
		}

		/// <summary>
		/// The predefined configuration.
		/// </summary>
		[DefaultValue( PredefinedConfigurationEnum.None )]
		public Reference<PredefinedConfigurationEnum> PredefinedConfiguration
		{
			get { if( _predefinedConfiguration.BeginGet() ) PredefinedConfiguration = _predefinedConfiguration.Get( this ); return _predefinedConfiguration.value; }
			set { if( _predefinedConfiguration.BeginSet( ref value ) ) { try { PredefinedConfigurationChanged?.Invoke( this ); } finally { _predefinedConfiguration.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PredefinedConfiguration"/> property value changes.</summary>
		public event Action<PlantType> PredefinedConfigurationChanged;
		ReferenceField<PredefinedConfigurationEnum> _predefinedConfiguration = PredefinedConfigurationEnum.None;

		/// <summary>
		/// Whether to generate individual meshes.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( true )]
		public Reference<bool> GenerateIndividuals
		{
			get { if( _generateIndividuals.BeginGet() ) GenerateIndividuals = _generateIndividuals.Get( this ); return _generateIndividuals.value; }
			set { if( _generateIndividuals.BeginSet( ref value ) ) { try { GenerateIndividualsChanged?.Invoke( this ); } finally { _generateIndividuals.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GenerateIndividuals"/> property value changes.</summary>
		public event Action<PlantType> GenerateIndividualsChanged;
		ReferenceField<bool> _generateIndividuals = true;

		/// <summary>
		/// Whether to generate groups of meshes.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( false )]
		public Reference<bool> GenerateGroups
		{
			get { if( _generateGroups.BeginGet() ) GenerateGroups = _generateGroups.Get( this ); return _generateGroups.value; }
			set { if( _generateGroups.BeginSet( ref value ) ) { try { GenerateGroupsChanged?.Invoke( this ); } finally { _generateGroups.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GenerateGroups"/> property value changes.</summary>
		public event Action<PlantType> GenerateGroupsChanged;
		ReferenceField<bool> _generateGroups = false;

		/// <summary>
		/// The size of the group of meshes.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( 5 )]
		public Reference<int> GroupSize
		{
			get { if( _groupSize.BeginGet() ) GroupSize = _groupSize.Get( this ); return _groupSize.value; }
			set { if( _groupSize.BeginSet( ref value ) ) { try { GroupSizeChanged?.Invoke( this ); } finally { _groupSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GroupSize"/> property value changes.</summary>
		public event Action<PlantType> GroupSizeChanged;
		ReferenceField<int> _groupSize = 5;

		/// <summary>
		/// The radius of the group of meshes.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( 1.0 )]
		public Reference<double> GroupRadius
		{
			get { if( _groupRadius.BeginGet() ) GroupRadius = _groupRadius.Get( this ); return _groupRadius.value; }
			set { if( _groupRadius.BeginSet( ref value ) ) { try { GroupRadiusChanged?.Invoke( this ); } finally { _groupRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GroupRadius"/> property value changes.</summary>
		public event Action<PlantType> GroupRadiusChanged;
		ReferenceField<double> _groupRadius = 1.0;

		/// <summary>
		/// The maximal incline in the group.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( 0.0 )]
		public Reference<Degree> GroupMaxIncline
		{
			get { if( _groupMaxIncline.BeginGet() ) GroupMaxIncline = _groupMaxIncline.Get( this ); return _groupMaxIncline.value; }
			set { if( _groupMaxIncline.BeginSet( ref value ) ) { try { GroupMaxInclineChanged?.Invoke( this ); } finally { _groupMaxIncline.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GroupMaxIncline"/> property value changes.</summary>
		public event Action<PlantType> GroupMaxInclineChanged;
		ReferenceField<Degree> _groupMaxIncline = Degree.Zero;

		/// <summary>
		/// The maximal incline from the center in the group.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( 0.0 )]
		public Reference<Degree> GroupMaxInclineFromCenter
		{
			get { if( _groupMaxInclineFromCenter.BeginGet() ) GroupMaxInclineFromCenter = _groupMaxInclineFromCenter.Get( this ); return _groupMaxInclineFromCenter.value; }
			set { if( _groupMaxInclineFromCenter.BeginSet( ref value ) ) { try { GroupMaxInclineFromCenterChanged?.Invoke( this ); } finally { _groupMaxInclineFromCenter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GroupMaxInclineFromCenter"/> property value changes.</summary>
		public event Action<PlantType> GroupMaxInclineFromCenterChanged;
		ReferenceField<Degree> _groupMaxInclineFromCenter = Degree.Zero;

		/// <summary>
		/// The number of variations to generate.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( 2 )]
		public Reference<int> Variations
		{
			get { if( _variations.BeginGet() ) Variations = _variations.Get( this ); return _variations.value; }
			set { if( _variations.BeginSet( ref value ) ) { try { VariationsChanged?.Invoke( this ); } finally { _variations.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Variations"/> property value changes.</summary>
		public event Action<PlantType> VariationsChanged;
		ReferenceField<int> _variations = 2;

		[Flags]
		public enum AgesEnum
		{
			VeryYoung = 1,
			Young = 2,
			Mature = 4,
			Old = 8,
			All = VeryYoung | Young | Mature | Old,
		}

		/// <summary>
		/// The possible ages of the plant.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( AgesEnum.All )]
		public Reference<AgesEnum> Ages
		{
			get { if( _ages.BeginGet() ) Ages = _ages.Get( this ); return _ages.value; }
			set { if( _ages.BeginSet( ref value ) ) { try { AgesChanged?.Invoke( this ); } finally { _ages.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Ages"/> property value changes.</summary>
		public event Action<PlantType> AgesChanged;
		ReferenceField<AgesEnum> _ages = AgesEnum.All;

		/// <summary>
		/// The amount of segments by length.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( 2.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> SegmentsByMeter
		{
			get { if( _segmentsByMeter.BeginGet() ) SegmentsByMeter = _segmentsByMeter.Get( this ); return _segmentsByMeter.value; }
			set { if( _segmentsByMeter.BeginSet( ref value ) ) { try { SegmentsByMeterChanged?.Invoke( this ); } finally { _segmentsByMeter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SegmentsByMeter"/> property value changes.</summary>
		public event Action<PlantType> SegmentsByMeterChanged;
		ReferenceField<double> _segmentsByMeter = 2.0;

		/// <summary>
		/// The amount of segments by circle.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( 16 )]
		public Reference<int> SegmentsByCircle
		{
			get { if( _segmentsByCircle.BeginGet() ) SegmentsByCircle = _segmentsByCircle.Get( this ); return _segmentsByCircle.value; }
			set { if( _segmentsByCircle.BeginSet( ref value ) ) { try { SegmentsByCircleChanged?.Invoke( this ); } finally { _segmentsByCircle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SegmentsByCircle"/> property value changes.</summary>
		public event Action<PlantType> SegmentsByCircleChanged;
		ReferenceField<int> _segmentsByCircle = 16;

		/// <summary>
		/// The amount of levels of detail to generate.
		/// </summary>
		[DefaultValue( 4 )]
		[Range( 1, 6 )]
		[DisplayName( "LOD Levels" )]
		[Category( "Output" )]
		public Reference<int> LODLevels
		{
			get { if( _lODLevels.BeginGet() ) LODLevels = _lODLevels.Get( this ); return _lODLevels.value; }
			set { if( _lODLevels.BeginSet( ref value ) ) { try { LODLevelsChanged?.Invoke( this ); } finally { _lODLevels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODLevels"/> property value changes.</summary>
		[DisplayName( "LOD Levels Changed" )]
		public event Action<PlantType> LODLevelsChanged;
		ReferenceField<int> _lODLevels = 4;

		/// <summary>
		/// The distance from the previous to the next level of detail. The result value depends mesh bounds, MatureHeight.
		/// </summary>
		[Category( "Output" )]
		[DisplayName( "LOD Distance" )]
		[DefaultValue( 10.0 )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> LODDistance
		{
			get { if( _lODDistance.BeginGet() ) LODDistance = _lODDistance.Get( this ); return _lODDistance.value; }
			set { if( _lODDistance.BeginSet( ref value ) ) { try { LODDistanceChanged?.Invoke( this ); } finally { _lODDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODDistance"/> property value changes.</summary>
		[DisplayName( "LOD Distance Changed" )]
		public event Action<PlantType> LODDistanceChanged;
		ReferenceField<double> _lODDistance = 10.0;

		/// <summary>
		/// Whether to generate a voxel grid for a last LOD.
		/// </summary>
		[DefaultValue( true )]
		[DisplayName( "LOD Voxels" )]
		public Reference<bool> LODVoxels
		{
			get { if( _lODVoxels.BeginGet() ) LODVoxels = _lODVoxels.Get( this ); return _lODVoxels.value; }
			set { if( _lODVoxels.BeginSet( ref value ) ) { try { LODVoxelsChanged?.Invoke( this ); } finally { _lODVoxels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODVoxels"/> property value changes.</summary>
		[DisplayName( "LOD Voxels Changed" )]
		public event Action<PlantType> LODVoxelsChanged;
		ReferenceField<bool> _lODVoxels = true;

		/// <summary>
		/// The size of a voxel grid of a last LOD.
		/// </summary>
		[DefaultValue( VoxelGridSizeEnum._64 )]
		[DisplayName( "LOD Voxel Grid" )]
		public Reference<VoxelGridSizeEnum> LODVoxelGrid
		{
			get { if( _lODVoxelGrid.BeginGet() ) LODVoxelGrid = _lODVoxelGrid.Get( this ); return _lODVoxelGrid.value; }
			set { if( _lODVoxelGrid.BeginSet( ref value ) ) { try { LODVoxelGridChanged?.Invoke( this ); } finally { _lODVoxelGrid.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODVoxelGrid"/> property value changes.</summary>
		[DisplayName( "LOD Voxel Grid Changed" )]
		public event Action<PlantType> LODVoxelGridChanged;
		ReferenceField<VoxelGridSizeEnum> _lODVoxelGrid = VoxelGridSizeEnum._64;

		//!!!!default
		/// <summary>
		/// The factor to changing the visibility of thin objects. It is also useful when your model has holes in its shape, the algorithm thinks your model is empty inside.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[DisplayName( "LOD Voxel Thin Factor" )]
		public Reference<double> LODVoxelThinFactor
		{
			get { if( _lODVoxelThinFactor.BeginGet() ) LODVoxelThinFactor = _lODVoxelThinFactor.Get( this ); return _lODVoxelThinFactor.value; }
			set { if( _lODVoxelThinFactor.BeginSet( ref value ) ) { try { LODVoxelThinFactorChanged?.Invoke( this ); } finally { _lODVoxelThinFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODVoxelThinFactor"/> property value changes.</summary>
		[DisplayName( "LOD Voxel Thin Factor Changed" )]
		public event Action<PlantType> LODVoxelThinFactorChanged;
		ReferenceField<double> _lODVoxelThinFactor = 0.5;

		/// <summary>
		/// The maximal distance to fill holes, which happens when ray matching can't find the result because reach max steps limitations.
		/// </summary>
		[DisplayName( "LOD Voxel Fill Holes Distance" )]
		[Category( "Geometry" )]
		[DefaultValue( 1.1 )]
		[Range( 0, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 5 )]
		public Reference<double> LODVoxelFillHolesDistance
		{
			get { if( _lODVoxelFillHolesDistance.BeginGet() ) LODVoxelFillHolesDistance = _lODVoxelFillHolesDistance.Get( this ); return _lODVoxelFillHolesDistance.value; }
			set { if( _lODVoxelFillHolesDistance.BeginSet( ref value ) ) { try { LODVoxelFillHolesDistanceChanged?.Invoke( this ); } finally { _lODVoxelFillHolesDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODVoxelFillHolesDistance"/> property value changes.</summary>
		[DisplayName( "LOD Voxel Fill Holes Distance Changed" )]
		public event Action<PlantType> LODVoxelFillHolesDistanceChanged;
		ReferenceField<double> _lODVoxelFillHolesDistance = 1.1;

		/// <summary>
		/// Whether to generate collision meshes.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( false )]
		public Reference<bool> Collision
		{
			get { if( _collision.BeginGet() ) Collision = _collision.Get( this ); return _collision.value; }
			set { if( _collision.BeginSet( ref value ) ) { try { CollisionChanged?.Invoke( this ); } finally { _collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Collision"/> property value changes.</summary>
		public event Action<PlantType> CollisionChanged;
		ReferenceField<bool> _collision = false;

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( GroupSize ):
				case nameof( GroupRadius ):
				case nameof( GroupMaxIncline ):
				case nameof( GroupMaxInclineFromCenter ):
					if( !GenerateGroups )
						skip = true;
					break;

				case nameof( LODVoxelGrid ):
				case nameof( LODVoxelThinFactor ):
				case nameof( LODVoxelFillHolesDistance ):
					if( !LODVoxels )
						skip = true;
					break;
				}
			}
		}

		//internal double CalculateVisibilityDistance( Bounds bounds )
		//{
		//	var boundsSize = bounds.GetSize().MaxComponent();
		//	var factor = MathEx.Saturate( boundsSize / MatureHeight.Value.GetCenter() );

		//	var result = factor * VisibilityDistance;
		//	//var result = factor * factor * VisibilityDistance;

		//	//Log.Info( factor.ToString() + " " + result.ToString() );

		//	return result;
		//}

		internal double CalculateLODDistance( Bounds bounds, int lod )
		{
			var boundsSize = bounds.GetSize().MaxComponent();
			var factor = MathEx.Saturate( boundsSize / MatureHeight.Value.GetCenter() );

			return LODDistance.Value * lod * factor;

			//levelOfDetail.Distance = m.VisibilityDistance.Value * ( (double)lod / 4.0 );
		}

		public bool ExportToMeshes( string writeToFolder, bool getFileNamesMode, List<string> fileNames, out string error )
		{
			error = "";

			var typeFileName = ComponentUtility.GetOwnedFileNameOfComponent( this );

			for( int individualOrGroup = 0; individualOrGroup < 2; individualOrGroup++ )
			{
				switch( individualOrGroup )
				{
				case 0:
					if( !GenerateIndividuals )
						continue;
					break;

				case 1:
					if( !GenerateGroups )
						continue;
					break;
				}

				var groupSize = individualOrGroup == 1 ? GroupSize.Value : 1;

				for( int variation = 0; variation < Variations.Value; variation++ )
				{
					//for( int live = 0; live < 3; live++ )
					//{
					//	var dead = live == 1 ? 1.0 : 0.0;
					//	var fired = live == 2 ? 1.0 : 0.0;

					//var liveStrings = new[] { "Alive", "Dead", "Fired" };
					//string liveString = liveStrings[ live ];

					//for( int season = 0; season < 4; season++ )
					//{
					//var seasonStrings = new[] { "Summer", "Fall", "Winter", "Spring" };
					//string seasonString = seasonStrings[ season ];

					var ages = Ages.Value;

					for( int ageNumber = 0; ageNumber < 4; ageNumber++ )
					{
						switch( ageNumber )
						{
						case 0:
							if( !ages.HasFlag( PlantType.AgesEnum.VeryYoung ) )
								continue;
							break;
						case 1:
							if( !ages.HasFlag( PlantType.AgesEnum.Young ) )
								continue;
							break;
						case 2:
							if( !ages.HasFlag( PlantType.AgesEnum.Mature ) )
								continue;
							break;
						case 3:
							if( !ages.HasFlag( PlantType.AgesEnum.Old ) )
								continue;
							break;
						}


						var ageStrings = new[] { "Very young", "Young", "Mature", "Old" };
						string ageString = ageStrings[ ageNumber ];

						var age = MatureAge.Value;
						switch( ageNumber )
						{
						case 0: age *= 0.33; break;
						case 1: age *= 0.66; break;
						case 3: age *= 1.5; break;
						}

						//Common oak 1 Individual 1 Very young
						var individualOrGroupString = individualOrGroup == 1 ? "Group" : "Individual";
						var postFix = $"{variation + 1} {individualOrGroupString} {ageNumber + 1} {ageString}";

						var realFileName = Path.Combine( writeToFolder, Path.GetFileNameWithoutExtension( typeFileName ) + " " + postFix + ".mesh" );

						if( getFileNamesMode )
						{
							//only get file names
							fileNames.Add( realFileName );
						}
						else
						{
							//world transforms
							var worldTransforms = new Transform[ groupSize ];
							if( groupSize > 1 )
							{
								var random = new FastRandom( variation, true );

								for( int n = 0; n < groupSize; n++ )
								{
									//!!!!так больше в центре будет. может по квадрату искать
									var angle = random.Next( MathEx.PI * 2 );
									var distance = random.Next( GroupRadius ) + 0.0001;

									var position = new Vector3( Math.Cos( angle ), Math.Sin( angle ), 0 ) * distance;

									var rotation = Quaternion.Identity;

									if( GroupMaxInclineFromCenter.Value != 0 )
									{
										//!!!!strange
										var angle2 = angle - Math.PI / 2;

										//rotate around its axis
										var rot = Quaternion.FromDirectionZAxisUp( new Vector3( Math.Cos( angle2 ), Math.Sin( angle2 ), 0 ) );
										//var rot = Quaternion.FromRotateByZ( -angle );

										var incline = random.Next( GroupMaxInclineFromCenter.Value.InRadians() );
										rot *= QuaternionF.FromRotateByX( (float)incline );

										rotation *= rot;
									}

									//rotate around its axis
									rotation *= Quaternion.FromRotateByZ( random.Next( 0, MathEx.PI * 2 ) );

									if( GroupMaxIncline.Value != 0 )
									{
										var incline = random.Next( GroupMaxIncline.Value.InRadians() );
										rotation *= QuaternionF.FromRotateByX( (float)incline );
									}

									worldTransforms[ n ] = new Transform( position, rotation );
								}
							}
							else
								worldTransforms[ 0 ] = Transform.Identity;


							var voxelLodIndex = -1;
							if( LODVoxels )
								voxelLodIndex = LODLevels - 1;

							var m = ComponentUtility.CreateComponent<Mesh>( null, true, false );
							Bounds bounds = Bounds.Cleared;

							var meshes = new List<Mesh>();

							var lod0Generators = new List<PlantGenerator>();

							{
								var lod = 0;

								for( var groupElement = 0; groupElement < groupSize; groupElement++ )
								{
									var worldTransform = worldTransforms[ groupElement ];
									var seed = variation + 10 + ( variation + 1 ) * ( groupElement + 1 );

									var generator = new PlantGenerator( this, seed, age/*, dead, fired, season*/, lod == voxelLodIndex ? 0 : lod, worldTransform, null, m );
									generator.Generate();

									bounds.Add( generator.Bounds );
									//generator.Bounds
									//if( groupElement == 0 )
									//{
									//	generator0 = generator;
									//	m.VisibilityDistance = generator.CalculateVisibilityDistance();
									//}

									lod0Generators.Add( generator );
								}

								//!!!!
								//VisibilityDistanceFactor
								//m.VisibilityDistance = plantType.CalculateVisibilityDistance( bounds );

								m.MergeGeometriesWithEqualVertexStructureAndMaterial();

								meshes.Add( m );
							}


							for( int lodIndex = 1; lodIndex < LODLevels; lodIndex++ )
							{
								var lod = m.CreateComponent<MeshLevelOfDetail>();
								lod.Name = "LOD " + lodIndex.ToString();

								var lodMesh = lod.CreateComponent<Mesh>();
								lodMesh.Name = "Mesh";

								for( var groupElement = 0; groupElement < groupSize; groupElement++ )
								{
									var worldTransform = worldTransforms[ groupElement ];
									var seed = variation + 10 + ( variation + 1 ) * ( groupElement + 1 );

									var generator2 = new PlantGenerator( this, seed, age/*, dead, fired, season*/, lodIndex == voxelLodIndex ? 0 : lodIndex, worldTransform, null, lodMesh );
									generator2.MeshRealFileName = realFileName;
									generator2.Generate();
								}

								lodMesh.MergeGeometriesWithEqualVertexStructureAndMaterial();

								lod.Mesh = ReferenceUtility.MakeThisReference( lod, lodMesh );
								lod.Distance = CalculateLODDistance( bounds, lodIndex );
								//levelOfDetail.Distance = generator0.CalculateLODDistance( lod );

								//if( voxelLodIndex == lodIndex )
								//	lod.Distance = 100000.0;

								meshes.Add( lodMesh );
							}

							//convert last lod mesh to voxel
							if( voxelLodIndex != -1 )
							{
								var mesh = meshes[ voxelLodIndex ];

								var imageSize = int.Parse( LODVoxelGrid.Value.ToString().Replace( "_", "" ) );

								//smaller image for very young and individual
								if( individualOrGroup == 0 )
								{
									if( ageNumber == 0 )
									{
										imageSize /= 2;
										if( imageSize < 8 )
											imageSize = 8;
									}
								}

								mesh.ConvertToVoxel( imageSize, LODVoxelThinFactor, true, false, LODVoxelFillHolesDistance );
							}

							//collision
							if( Collision && lod0Generators.Count != 0 )
							{
								m.EditorDisplayCollision = true;

								var meshGeometry = m.CreateComponent<RigidBody>();
								meshGeometry.Name = "Collision Definition";

								foreach( var generator in lod0Generators )
								{
									var shape = meshGeometry.CreateComponent<CollisionShape_Cone>();
									shape.Name = "Shape";

									if( generator.Trunks.Count != 0 )
									{
										var trunk = generator.Trunks[ 0 ];

										shape.Radius = trunk.Width / 2;
										shape.Height = trunk.Length;
										shape.LocalTransform = generator.WorldTransform * new Transform( new Vector3( 0, 0, trunk.Length / 2 + trunk.DeepOffset ) );
									}
								}
							}

							m.Enabled = true;

							try
							{
								if( !ComponentUtility.SaveComponentToFile( m, realFileName, null, out error ) )
									return false;
							}
							finally
							{
								m.Dispose();
							}
						}
					}
					//}
					//}
				}
			}

			//var realFileName = Path.Combine( realDirectoryPath, Path.GetFileNameWithoutExtension( typeFileName ) + ".fbx" );
			//Mesh m = GenerateMesh( plantType, 0, 0, 0, 0, 0 );
			//try
			//{
			//	if( !m.ExportToFBX( realFileName, out var error ) )
			//	{
			//		Log.Warning( error );
			//		return;
			//	}
			//}
			//finally
			//{
			//	m.Dispose();
			//}

			return true;
		}

	}
}
#endif