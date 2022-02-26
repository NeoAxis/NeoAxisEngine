// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// A tool for procedural scene generation.
	/// </summary>
	[AddToResourcesWindow( @"Base\Scene objects\Additional\World Generator", 0 )]
#if !DEPLOY
	[Editor.SettingsCell( typeof( Editor.WorldGeneratorSettingsCell ) )]
#endif
	public class WorldGenerator : Component
	{
		/// <summary>
		/// The size of the generated area.
		/// </summary>
		[DefaultValue( 1000.0 )]
		[Range( 100, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Geometry" )]
		public Reference<double> Size
		{
			get { if( _size.BeginGet() ) Size = _size.Get( this ); return _size.value; }
			set { if( _size.BeginSet( ref value ) ) { try { SizeChanged?.Invoke( this ); } finally { _size.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Size"/> property value changes.</summary>
		public event Action<WorldGenerator> SizeChanged;
		ReferenceField<double> _size = 1000.0;

		/// <summary>
		/// The height range of the generated terrain.
		/// </summary>
		[DefaultValue( "-5 5" )]
		[Range( -50, 50 )]
		[Category( "Geometry" )]
		public Reference<Range> HeightRange
		{
			get { if( _heightRange.BeginGet() ) HeightRange = _heightRange.Get( this ); return _heightRange.value; }
			set { if( _heightRange.BeginSet( ref value ) ) { try { HeightRangeChanged?.Invoke( this ); } finally { _heightRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeightRange"/> property value changes.</summary>
		public event Action<WorldGenerator> HeightRangeChanged;
		ReferenceField<Range> _heightRange = new Range( -5, 5 );

		public enum TemplateEnum
		{
			OakForest,
			//!!!!impl
			//Plains,
			//FlowerMeadow,
			//Hills,
			//Rocks,
			Custom,
		}

		/// <summary>
		/// The predefined template of materials and surfaces.
		/// </summary>
		[DefaultValue( TemplateEnum.OakForest )]
		[Category( "Template" )]
		public Reference<TemplateEnum> Template
		{
			get { if( _template.BeginGet() ) Template = _template.Get( this ); return _template.value; }
			set { if( _template.BeginSet( ref value ) ) { try { TemplateChanged?.Invoke( this ); } finally { _template.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Template"/> property value changes.</summary>
		public event Action<WorldGenerator> TemplateChanged;
		ReferenceField<TemplateEnum> _template = TemplateEnum.OakForest;

		//!!!!impl
		//[DefaultValue( true )]
		//[Category( "Template" )]
		//public Reference<bool> AddDetails
		//{
		//	get { if( _addDetails.BeginGet() ) AddDetails = _addDetails.Get( this ); return _addDetails.value; }
		//	set { if( _addDetails.BeginSet( ref value ) ) { try { AddDetailsChanged?.Invoke( this ); } finally { _addDetails.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AddDetails"/> property value changes.</summary>
		//public event Action<WorldGenerator> AddDetailsChanged;
		//ReferenceField<bool> _addDetails = true;

		const string terrainMaterialDefault = @"Content\Materials\Basic Library\Ground\Forest Ground 01.material";
		/// <summary>
		/// The material of the terrain.
		/// </summary>
		[DefaultValueReference( terrainMaterialDefault )]
		[Category( "Template" )]
		public Reference<Material> TerrainMaterial
		{
			get { if( _terrainMaterial.BeginGet() ) TerrainMaterial = _terrainMaterial.Get( this ); return _terrainMaterial.value; }
			set { if( _terrainMaterial.BeginSet( ref value ) ) { try { TerrainMaterialChanged?.Invoke( this ); } finally { _terrainMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TerrainMaterial"/> property value changes.</summary>
		public event Action<WorldGenerator> TerrainMaterialChanged;
		ReferenceField<Material> _terrainMaterial = new Reference<Material>( null, terrainMaterialDefault );

		const string surface1Default = @"Content\Vegetation\Models\Woody plant\Quercus robur\Quercus robur.surface";
		/// <summary>
		/// The first surface.
		/// </summary>
		[DefaultValueReference( surface1Default )]
		[DisplayName( "Surface 1" )]
		[Category( "Template" )]
		public Reference<Surface> Surface1
		{
			get { if( _surface1.BeginGet() ) Surface1 = _surface1.Get( this ); return _surface1.value; }
			set { if( _surface1.BeginSet( ref value ) ) { try { Surface1Changed?.Invoke( this ); } finally { _surface1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface1"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface1Changed;
		ReferenceField<Surface> _surface1 = new Reference<Surface>( null, surface1Default );

		/// <summary>
		/// The strength of painting for the first surface.
		/// </summary>
		[DefaultValue( 0.3 )]
		[Range( 0, 1 )]
		[DisplayName( "Surface 1 Paint Factor" )]
		[Category( "Template" )]
		public Reference<double> Surface1PaintFactor
		{
			get { if( _surface1PaintFactor.BeginGet() ) Surface1PaintFactor = _surface1PaintFactor.Get( this ); return _surface1PaintFactor.value; }
			set { if( _surface1PaintFactor.BeginSet( ref value ) ) { try { Surface1PaintFactorChanged?.Invoke( this ); } finally { _surface1PaintFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface1PaintFactor"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface1PaintFactorChanged;
		ReferenceField<double> _surface1PaintFactor = 0.3;

		///// <summary>
		///// The number of painting iterations for the first surface.
		///// </summary>
		//[DefaultValue( 5 )]
		//[Range( 1, 20 )]
		//[DisplayName( "Surface 1 Paint Steps" )]
		//[Category( "Template" )]
		//public Reference<int> Surface1PaintSteps
		//{
		//	get { if( _surface1PaintSteps.BeginGet() ) Surface1PaintSteps = _surface1PaintSteps.Get( this ); return _surface1PaintSteps.value; }
		//	set { if( _surface1PaintSteps.BeginSet( ref value ) ) { try { Surface1PaintStepsChanged?.Invoke( this ); } finally { _surface1PaintSteps.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Surface1PaintSteps"/> property value changes.</summary>
		//public event Action<WorldGenerator> Surface1PaintStepsChanged;
		//ReferenceField<int> _surface1PaintSteps = 5;

		const string surface2Default = @"Content\Models\Rocks\Rocks (4 pieces)\Rocks.surface";
		/// <summary>
		/// The second surface.
		/// </summary>
		[DefaultValue( surface2Default )]
		[DisplayName( "Surface 2" )]
		[Category( "Template" )]
		public Reference<Surface> Surface2
		{
			get { if( _surface2.BeginGet() ) Surface2 = _surface2.Get( this ); return _surface2.value; }
			set { if( _surface2.BeginSet( ref value ) ) { try { Surface2Changed?.Invoke( this ); } finally { _surface2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface2"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface2Changed;
		ReferenceField<Surface> _surface2 = new Reference<Surface>( null, surface2Default );

		/// <summary>
		/// The strength of painting for the second surface.
		/// </summary>
		[DefaultValue( 0.04 )]
		[Range( 0, 1 )]
		[DisplayName( "Surface 2 Paint Factor" )]
		[Category( "Template" )]
		public Reference<double> Surface2PaintFactor
		{
			get { if( _surface2PaintFactor.BeginGet() ) Surface2PaintFactor = _surface2PaintFactor.Get( this ); return _surface2PaintFactor.value; }
			set { if( _surface2PaintFactor.BeginSet( ref value ) ) { try { Surface2PaintFactorChanged?.Invoke( this ); } finally { _surface2PaintFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface2PaintFactor"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface2PaintFactorChanged;
		ReferenceField<double> _surface2PaintFactor = 0.04;

		///// <summary>
		///// The number of painting iterations for the second surface.
		///// </summary>
		//[DefaultValue( 1 )]
		//[Range( 1, 20 )]
		//[DisplayName( "Surface 2 Paint Steps" )]
		//[Category( "Template" )]
		//public Reference<int> Surface2PaintSteps
		//{
		//	get { if( _surface2PaintSteps.BeginGet() ) Surface2PaintSteps = _surface2PaintSteps.Get( this ); return _surface2PaintSteps.value; }
		//	set { if( _surface2PaintSteps.BeginSet( ref value ) ) { try { Surface2PaintStepsChanged?.Invoke( this ); } finally { _surface2PaintSteps.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Surface2PaintSteps"/> property value changes.</summary>
		//public event Action<WorldGenerator> Surface2PaintStepsChanged;
		//ReferenceField<int> _surface2PaintSteps = 1;

		const string surface3Default = "";
		/// <summary>
		/// The third surface.
		/// </summary>
		[DefaultValueReference( surface3Default )]
		[DisplayName( "Surface 3" )]
		[Category( "Template" )]
		public Reference<Surface> Surface3
		{
			get { if( _surface3.BeginGet() ) Surface3 = _surface3.Get( this ); return _surface3.value; }
			set { if( _surface3.BeginSet( ref value ) ) { try { Surface3Changed?.Invoke( this ); } finally { _surface3.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface3"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface3Changed;
		ReferenceField<Surface> _surface3 = new Reference<Surface>( null, surface3Default );

		/// <summary>
		/// The strength of painting for the third surface.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Range( 0, 1 )]
		[DisplayName( "Surface 3 Paint Factor" )]
		[Category( "Template" )]
		public Reference<double> Surface3PaintFactor
		{
			get { if( _surface3PaintFactor.BeginGet() ) Surface3PaintFactor = _surface3PaintFactor.Get( this ); return _surface3PaintFactor.value; }
			set { if( _surface3PaintFactor.BeginSet( ref value ) ) { try { Surface3PaintFactorChanged?.Invoke( this ); } finally { _surface3PaintFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface3PaintFactor"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface3PaintFactorChanged;
		ReferenceField<double> _surface3PaintFactor = 0.1;

		///// <summary>
		///// The number of painting iterations for the third surface.
		///// </summary>
		//[DefaultValue( 1 )]
		//[Range( 1, 20 )]
		//[DisplayName( "Surface 3 Paint Steps" )]
		//[Category( "Template" )]
		//public Reference<int> Surface3PaintSteps
		//{
		//	get { if( _surface3PaintSteps.BeginGet() ) Surface3PaintSteps = _surface3PaintSteps.Get( this ); return _surface3PaintSteps.value; }
		//	set { if( _surface3PaintSteps.BeginSet( ref value ) ) { try { Surface3PaintStepsChanged?.Invoke( this ); } finally { _surface3PaintSteps.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Surface3PaintSteps"/> property value changes.</summary>
		//public event Action<WorldGenerator> Surface3PaintStepsChanged;
		//ReferenceField<int> _surface3PaintSteps = 1;

		const string surface4Default = "";
		/// <summary>
		/// The fourth surface.
		/// </summary>
		[DefaultValueReference( surface4Default )]
		[DisplayName( "Surface 4" )]
		[Category( "Template" )]
		public Reference<Surface> Surface4
		{
			get { if( _surface4.BeginGet() ) Surface4 = _surface4.Get( this ); return _surface4.value; }
			set { if( _surface4.BeginSet( ref value ) ) { try { Surface4Changed?.Invoke( this ); } finally { _surface4.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface4"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface4Changed;
		ReferenceField<Surface> _surface4 = new Reference<Surface>( null, surface4Default );

		/// <summary>
		/// The strength of painting for the fourth surface.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Range( 0, 1 )]
		[DisplayName( "Surface 4 Paint Factor" )]
		[Category( "Template" )]
		public Reference<double> Surface4PaintFactor
		{
			get { if( _surface4PaintFactor.BeginGet() ) Surface4PaintFactor = _surface4PaintFactor.Get( this ); return _surface4PaintFactor.value; }
			set { if( _surface4PaintFactor.BeginSet( ref value ) ) { try { Surface4PaintFactorChanged?.Invoke( this ); } finally { _surface4PaintFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface4PaintFactor"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface4PaintFactorChanged;
		ReferenceField<double> _surface4PaintFactor = 0.1;

		///// <summary>
		///// The number of painting iterations for the fourth surface.
		///// </summary>
		//[DefaultValue( 1 )]
		//[Range( 1, 20 )]
		//[DisplayName( "Surface 4 Paint Steps" )]
		//[Category( "Template" )]
		//public Reference<int> Surface4PaintSteps
		//{
		//	get { if( _surface4PaintSteps.BeginGet() ) Surface4PaintSteps = _surface4PaintSteps.Get( this ); return _surface4PaintSteps.value; }
		//	set { if( _surface4PaintSteps.BeginSet( ref value ) ) { try { Surface4PaintStepsChanged?.Invoke( this ); } finally { _surface4PaintSteps.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Surface4PaintSteps"/> property value changes.</summary>
		//public event Action<WorldGenerator> Surface4PaintStepsChanged;
		//ReferenceField<int> _surface4PaintSteps = 1;


		//LeveledArea

		/// <summary>
		/// Whether to add leveled, cleared area.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Leveled Area" )]
		public Reference<bool> LeveledArea
		{
			get { if( _leveledArea.BeginGet() ) LeveledArea = _leveledArea.Get( this ); return _leveledArea.value; }
			set { if( _leveledArea.BeginSet( ref value ) ) { try { LeveledAreaChanged?.Invoke( this ); } finally { _leveledArea.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeveledArea"/> property value changes.</summary>
		public event Action<WorldGenerator> LeveledAreaChanged;
		ReferenceField<bool> _leveledArea = true;

		/// <summary>
		/// The size of the leveled area.
		/// </summary>
		[DefaultValue( "160 160" )]
		[Category( "Leveled Area" )]
		[Range( 10, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<Vector2> LeveledAreaSize
		{
			get { if( _leveledAreaSize.BeginGet() ) LeveledAreaSize = _leveledAreaSize.Get( this ); return _leveledAreaSize.value; }
			set { if( _leveledAreaSize.BeginSet( ref value ) ) { try { LeveledAreaSizeChanged?.Invoke( this ); } finally { _leveledAreaSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeveledAreaSize"/> property value changes.</summary>
		public event Action<WorldGenerator> LeveledAreaSizeChanged;
		ReferenceField<Vector2> _leveledAreaSize = new Vector2( 160, 160 );

		public enum LeveledAreaShapeEnum
		{
			Rectangle,
			Ellipse,
		}

		/// <summary>
		/// The shape of the leveled area.
		/// </summary>
		[Category( "Leveled Area" )]
		[DefaultValue( LeveledAreaShapeEnum.Rectangle )]
		public Reference<LeveledAreaShapeEnum> LeveledAreaShape
		{
			get { if( _leveledAreaShape.BeginGet() ) LeveledAreaShape = _leveledAreaShape.Get( this ); return _leveledAreaShape.value; }
			set { if( _leveledAreaShape.BeginSet( ref value ) ) { try { LeveledAreaShapeChanged?.Invoke( this ); } finally { _leveledAreaShape.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeveledAreaShape"/> property value changes.</summary>
		public event Action<WorldGenerator> LeveledAreaShapeChanged;
		ReferenceField<LeveledAreaShapeEnum> _leveledAreaShape = LeveledAreaShapeEnum.Rectangle;

		const double leveledAreaHeightDefault = 0.0;
		/// <summary>
		/// The height of the leveled area.
		/// </summary>
		[DefaultValue( leveledAreaHeightDefault )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Leveled Area" )]
		public Reference<double> LeveledAreaHeight
		{
			get { if( _leveledAreaHeight.BeginGet() ) LeveledAreaHeight = _leveledAreaHeight.Get( this ); return _leveledAreaHeight.value; }
			set { if( _leveledAreaHeight.BeginSet( ref value ) ) { try { LeveledAreaHeightChanged?.Invoke( this ); } finally { _leveledAreaHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeveledAreaHeight"/> property value changes.</summary>
		public event Action<WorldGenerator> LeveledAreaHeightChanged;
		ReferenceField<double> _leveledAreaHeight = leveledAreaHeightDefault;

		const string leveledAreaMaterialDefault = "Content\\Materials\\Basic Library\\Ground\\Forest Ground 03.material";
		/// <summary>
		/// The material of the leveled area.
		/// </summary>
		[DefaultValueReference( leveledAreaMaterialDefault )]
		[Category( "Leveled Area" )]
		public Reference<Material> LeveledAreaMaterial
		{
			get { if( _leveledAreaMaterial.BeginGet() ) LeveledAreaMaterial = _leveledAreaMaterial.Get( this ); return _leveledAreaMaterial.value; }
			set { if( _leveledAreaMaterial.BeginSet( ref value ) ) { try { LeveledAreaMaterialChanged?.Invoke( this ); } finally { _leveledAreaMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeveledAreaMaterial"/> property value changes.</summary>
		public event Action<WorldGenerator> LeveledAreaMaterialChanged;
		ReferenceField<Material> _leveledAreaMaterial = new Reference<Material>( null, leveledAreaMaterialDefault );

		//

		/// <summary>
		/// The seed number for random number generator.
		/// </summary>
		[DefaultValue( 0 )]
		[Range( 0, 100 )]
		[Category( "Generator" )]
		public Reference<int> Seed
		{
			get { if( _seed.BeginGet() ) Seed = _seed.Get( this ); return _seed.value; }
			set { if( _seed.BeginSet( ref value ) ) { try { SeedChanged?.Invoke( this ); } finally { _seed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Seed"/> property value changes.</summary>
		public event Action<WorldGenerator> SeedChanged;
		ReferenceField<int> _seed = 0;

		/// <summary>
		/// Whether to add paint layers with objects.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Generator" )]
		public Reference<bool> AddLayersWithObjects
		{
			get { if( _addLayersWithObjects.BeginGet() ) AddLayersWithObjects = _addLayersWithObjects.Get( this ); return _addLayersWithObjects.value; }
			set { if( _addLayersWithObjects.BeginSet( ref value ) ) { try { AddLayersWithObjectsChanged?.Invoke( this ); } finally { _addLayersWithObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AddLayersWithObjects"/> property value changes.</summary>
		public event Action<WorldGenerator> AddLayersWithObjectsChanged;
		ReferenceField<bool> _addLayersWithObjects = true;

		///// <summary>
		///// Whether to generate objects by the surfaces.
		///// </summary>
		//[DefaultValue( true )]
		//[Category( "Generator" )]
		//public Reference<bool> AddSurfaces
		//{
		//	get { if( _addSurfaces.BeginGet() ) AddSurfaces = _addSurfaces.Get( this ); return _addSurfaces.value; }
		//	set { if( _addSurfaces.BeginSet( ref value ) ) { try { AddSurfacesChanged?.Invoke( this ); } finally { _addSurfaces.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AddSurfaces"/> property value changes.</summary>
		//public event Action<WorldGenerator> AddSurfacesChanged;
		//ReferenceField<bool> _addSurfaces = true;

		///////////////////////////////////////////////

		public class TemplateData
		{
			public Reference<Material> TerrainMaterial;
			public List<SurfaceItem> Surfaces = new List<SurfaceItem>();

			//

			public class SurfaceItem
			{
				public Reference<Surface> Surface;
				public double PaintFactor;
				//public int PaintSteps;

				public SurfaceItem()
				{
				}

				public SurfaceItem( Reference<Surface> surface, double paintFactor )// int paintSteps )
				{
					Surface = surface;
					PaintFactor = paintFactor;
					//PaintSteps = paintSteps;
				}
			}
		}

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( TerrainMaterial ):
				case nameof( Surface1 ):
				case nameof( Surface2 ):
				case nameof( Surface3 ):
				case nameof( Surface4 ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					break;

				case nameof( Surface1PaintFactor ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface1.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Surface2PaintFactor ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface2.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Surface3PaintFactor ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface3.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Surface4PaintFactor ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface4.ReferenceSpecified )
						skip = true;
					break;

				case nameof( LeveledAreaSize ):
				case nameof( LeveledAreaShape ):
				case nameof( LeveledAreaHeight ):
				case nameof( LeveledAreaMaterial ):
					if( !LeveledArea )
						skip = true;
					break;

				}
			}
		}

#if !DEPLOY

		//TemplateData

		public delegate void GetTemplateDataDelegate( WorldGenerator sender, ref TemplateData data );
		public event GetTemplateDataDelegate GetTemplateData;

		TemplateData GetTemplateDataDefault()
		{
			var data = new TemplateData();

			switch( Template.Value )
			{
			case TemplateEnum.OakForest:
				{
					data.TerrainMaterial = new Reference<Material>( null, terrainMaterialDefault );

					var surface1 = new Reference<Surface>( null, surface1Default );
					data.Surfaces.Add( new TemplateData.SurfaceItem( surface1, 0.3 ) );

					var surface2 = new Reference<Surface>( null, surface2Default );
					data.Surfaces.Add( new TemplateData.SurfaceItem( surface2, 0.04 ) );
				}
				break;

			default:

				data.TerrainMaterial = TerrainMaterial;

				for( int nSurface = 0; nSurface < 4; nSurface++ )
				{
					Reference<Surface> surface = null;
					var paintFactor = 0.1;//var paintSteps = 1;
					switch( nSurface )
					{
					case 0: surface = Surface1; paintFactor = Surface1PaintFactor; break;
					case 1: surface = Surface2; paintFactor = Surface2PaintFactor; break;
					case 2: surface = Surface3; paintFactor = Surface3PaintFactor; break;
					case 3: surface = Surface4; paintFactor = Surface4PaintFactor; break;
					}

					if( surface.ReferenceSpecified )
						data.Surfaces.Add( new TemplateData.SurfaceItem( surface, paintFactor ) );
				}
				break;
			}

			return data;
		}

		public virtual void Generate( Editor.DocumentInstance document )
		{
			var scene = FindParent<Scene>();
			if( scene == null )
				return;

			TemplateData templateData = null;
			GetTemplateData?.Invoke( this, ref templateData );
			if( templateData == null )
				templateData = GetTemplateDataDefault();

			var randomizer = new FastRandom( Seed );

			var terrain = scene.GetComponent<Terrain>( true );
			var groupOfObjects = scene.GetComponent<GroupOfObjects>( true );

			//disable components before update
			if( terrain != null )
				terrain.Enabled = false;
			if( groupOfObjects != null )
				groupOfObjects.Enabled = false;

			//update terrain

			if( terrain == null )
			{
				terrain = scene.CreateComponent<Terrain>( enabled: false );
				terrain.Name = "Terrain";
			}

			terrain.HorizontalSize = Size;
			//terrain.Position = new Vector3( -terrain.HorizontalSize.Value / 2, -terrain.HorizontalSize.Value / 2, 0 );
			terrain.Material = templateData.TerrainMaterial;

			var heightRange = HeightRange.Value;
			var heightmapSize = terrain.GetHeightmapSizeInteger();

			//update terrain heights
			{
				var globalScale = 3.0;

				var functions = new (Radian angle, double scale)[]{
					(randomizer.Next( Math.PI * 2 ), randomizer.Next( 7 * globalScale, 10 * globalScale )) ,
					(randomizer.Next( Math.PI * 2 ), randomizer.Next( 5 * globalScale, 8 * globalScale )) ,
					(randomizer.Next( Math.PI * 2 ), randomizer.Next( 9 * globalScale, 15 * globalScale )) };

				var heights = new double[ heightmapSize, heightmapSize ];
				double min = double.MaxValue;
				double max = double.MinValue;

				for( int y = 0; y < heightmapSize; y++ )
				{
					for( int x = 0; x < heightmapSize; x++ )
					{
						var pos = terrain.GetPositionXY( new Vector2I( x, y ) );
						var localPos = pos - new Vector2( -Size.Value / 2, -Size.Value / 2 );

						var height = 0.0;

						foreach( var f in functions )
						{
							var mat = Matrix2.FromRotate( f.angle );
							var p = mat * localPos / f.scale;

							height += Math.Cos( p.X );
						}

						heights[ x, y ] = height;

						if( height < min )
							min = height;
						if( height > max )
							max = height;
					}
				}

				for( int y = 0; y < heightmapSize; y++ )
				{
					for( int x = 0; x < heightmapSize; x++ )
					{
						if( min < max )
						{
							var coef = MathEx.Saturate( ( heights[ x, y ] - min ) / ( max - min ) );
							var v = heightRange.Minimum + coef * heightRange.Size;
							terrain.SetHeightWithoutPosition( new Vector2I( x, y ), (float)v );
						}
						else
							terrain.SetHeightWithoutPosition( new Vector2I( x, y ), 0.0f );
					}

				}
			}

			//update group of objects

			if( groupOfObjects == null )
			{
				groupOfObjects = scene.CreateComponent<GroupOfObjects>( enabled: false );
				groupOfObjects.Name = "Group Of Objects";
			}


			var undoMultiAction = new Editor.UndoMultiAction();

			//clear
			var indexes = groupOfObjects.ObjectsGetAll();
			if( indexes.Count != 0 )
			{
				var action = new Editor.GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true );
				undoMultiAction.AddAction( action );
			}
			var components = groupOfObjects.GetComponents();
			undoMultiAction.AddAction( new Editor.UndoActionComponentCreateDelete( document, components, false ) );


			if( undoMultiAction.Actions.Count != 0 )
				document.CommitUndoAction( undoMultiAction );

			//update base objects
			if( terrain != null && !groupOfObjects.GetBaseObjects().Contains( terrain ) )
				groupOfObjects.BaseObjects.Add( ReferenceUtility.MakeRootReference( terrain ) );

			Rectangle leveledAreaRectangle = Rectangle.Zero;

			//delete old layers
			if( terrain != null )
			{
				foreach( var c in terrain.GetComponents<PaintLayer>() )
					c.RemoveFromParent( false );
			}

			//create internal data of the terrain
			if( terrain != null )
			{
				terrain.Enabled = true;
				terrain.Enabled = false;
			}

			//add layers with objects
			if( AddLayersWithObjects )
			{
				for( int nSurface = 0; nSurface < templateData.Surfaces.Count; nSurface++ )
				{
					var surfaceItem = templateData.Surfaces[ nSurface ];

					var surface = surfaceItem.Surface;
					if( surface.ReferenceSpecified && surfaceItem.PaintFactor > 0 )
					{
						var layer = terrain.CreateComponent<PaintLayer>();
						layer.Surface = surface;
						layer.BlendMode = PaintLayer.BlendModeEnum.NoBlend;

						var name = "";
						{
							var surfaceV = layer.Surface.Value;
							if( surfaceV != null )
							{
								if( surfaceV.Parent == null )
								{
									var fileName = ComponentUtility.GetOwnedFileNameOfComponent( surfaceV );
									if( !string.IsNullOrEmpty( fileName ) )
										name = Path.GetFileNameWithoutExtension( fileName );
								}
								else
									name = surfaceV.Name;
							}
							if( string.IsNullOrEmpty( name ) )
								name = ( nSurface + 1 ).ToString();
						}
						layer.Name = "Paint Layer " + name;


						var maskSize = terrain.GetPaintMaskSizeInteger();

						layer.Mask = new byte[ terrain.GetPaintMaskSizeInteger() * terrain.GetPaintMaskSizeInteger() ];

						{
							var min = new Vector2I( -100000, -100000 );
							var max = new Vector2I( 100000, 100000 );
							MathEx.Clamp( ref min.X, 0, maskSize - 1 );
							MathEx.Clamp( ref min.Y, 0, maskSize - 1 );
							MathEx.Clamp( ref max.X, 0, maskSize - 1 );
							MathEx.Clamp( ref max.Y, 0, maskSize - 1 );

							for( int y = min.Y; y <= max.Y; y++ )
							{
								for( int x = min.X; x <= max.X; x++ )
								{
									var maskIndex = new Vector2I( x, y );
									//var pos = terrain.GetPositionXYByMaskIndex( maskIndex );

									var factor = surfaceItem.PaintFactor;

									//var factor = leveledAreaRectangle.Contains( pos ) ? 1.0 : 0.0;
									//var distance = areaRectangle.GetPointDistance( pos );
									//var factor = 1.0 - MathEx.Saturate( distance / borderSize );

									//if( factor > 0 )
									{
										layer.SetMaskValue( maskIndex, (float)factor );

										//var source = terrain.GetHeightWithoutPosition( maskIndex, false );
										//var dest = LeveledAreaHeight.Value;

										//var newValue = MathEx.Lerp( source, dest, factor );
										//terrain.SetHeightWithoutPosition( maskIndex, (float)newValue );
									}
								}
							}
						}
					}
				}
			}

			if( LeveledArea && LeveledAreaSize.Value.X > 0 && LeveledAreaSize.Value.Y > 0 )
			{
				leveledAreaRectangle = new Rectangle( -LeveledAreaSize.Value / 2, LeveledAreaSize.Value / 2 );
				//var leveledAreaCenter = leveledAreaRectangle.GetCenter();
				var shape = LeveledAreaShape.Value;

				double GetPointDistance( Vector2 position )
				{
					if( shape == LeveledAreaShapeEnum.Ellipse )
					{
						var aspect = leveledAreaRectangle.Size.X / leveledAreaRectangle.Size.Y;
						if( aspect > 1.001 )
						{
							var pX = position.X / aspect;
							var c = new Circle( Vector2.Zero, leveledAreaRectangle.Maximum.Y );
							return c.GetPointDistance( new Vector2( pX, position.Y ) );
						}
						if( aspect < 0.999 )
						{
							var pY = position.Y * aspect;
							var c = new Circle( Vector2.Zero, leveledAreaRectangle.Maximum.X );
							return c.GetPointDistance( new Vector2( position.X, pY ) );
						}
						else
						{
							var c = new Circle( Vector2.Zero, leveledAreaRectangle.Maximum.X );
							return c.GetPointDistance( position );
						}

						//var c = new Circle( leveledAreaCenter, leveledAreaRectangle.Maximum.X - leveledAreaCenter.X );
						//return c.GetPointDistance( position );
					}
					else
						return leveledAreaRectangle.GetPointDistance( position );
				}

				//change terrain height
				{
					var borderSize = leveledAreaRectangle.Size.MaxComponent() / 10.0;
					if( borderSize < 0.001 )
						borderSize = 0.001;

					var min = terrain.GetCellIndexByPosition( leveledAreaRectangle.Minimum - new Vector2( borderSize, borderSize ) ) - new Vector2I( 1, 1 );
					var max = terrain.GetCellIndexByPosition( leveledAreaRectangle.Maximum + new Vector2( borderSize, borderSize ) ) + new Vector2I( 2, 2 );
					MathEx.Clamp( ref min.X, 0, heightmapSize - 1 );
					MathEx.Clamp( ref min.Y, 0, heightmapSize - 1 );
					MathEx.Clamp( ref max.X, 0, heightmapSize - 1 );
					MathEx.Clamp( ref max.Y, 0, heightmapSize - 1 );

					for( int y = min.Y; y <= max.Y; y++ )
					{
						for( int x = min.X; x <= max.X; x++ )
						{
							var cellIndex = new Vector2I( x, y );
							var pos = terrain.GetPositionXY( cellIndex );

							var distance = GetPointDistance( pos );

							var factor = 1.0 - MathEx.Saturate( distance / borderSize );
							if( factor > 0 )
							{
								var source = terrain.GetHeightWithoutPosition( cellIndex, false );
								var dest = LeveledAreaHeight.Value;

								var newValue = MathEx.Lerp( source, dest, factor );
								terrain.SetHeightWithoutPosition( cellIndex, (float)newValue );
							}
						}
					}
				}

				//add layer
				{
					var layer = terrain.CreateComponent<PaintLayer>();
					layer.Material = LeveledAreaMaterial;

					var material = layer.Material.Value;

					var name = "";
					{
						if( material != null )
						{
							if( material.Parent == null )
							{
								var fileName = ComponentUtility.GetOwnedFileNameOfComponent( material );
								if( !string.IsNullOrEmpty( fileName ) )
									name = Path.GetFileNameWithoutExtension( fileName );
							}
							else
								name = material.Name;
						}
						if( string.IsNullOrEmpty( name ) )
							name = layer.ToString();
					}
					layer.Name = "Paint Layer " + name;


					var maskSize = terrain.GetPaintMaskSizeInteger();

					layer.Mask = new byte[ terrain.GetPaintMaskSizeInteger() * terrain.GetPaintMaskSizeInteger() ];

					{
						//var borderSize = areaRectangle.Size.MaxComponent() / 10.0;
						//if( borderSize < 0.001 )
						//	borderSize = 0.001;

						var min = terrain.GetMaskIndexByPosition( leveledAreaRectangle.Minimum );// - new Vector2( borderSize, borderSize ) ) - new Vector2I( 1, 1 );
						var max = terrain.GetMaskIndexByPosition( leveledAreaRectangle.Maximum );// + new Vector2( borderSize, borderSize ) ) + new Vector2I( 2, 2 );
						MathEx.Clamp( min.X, 0, maskSize - 1 );
						MathEx.Clamp( min.Y, 0, maskSize - 1 );
						MathEx.Clamp( max.X, 0, maskSize - 1 );
						MathEx.Clamp( max.Y, 0, maskSize - 1 );

						for( int y = min.Y; y <= max.Y; y++ )
						{
							for( int x = min.X; x <= max.X; x++ )
							{
								var maskIndex = new Vector2I( x, y );
								var pos = terrain.GetPositionXYByMaskIndex( maskIndex );

								var factor = GetPointDistance( pos ) == 0 ? 1.0 : 0.0;
								//var factor = leveledAreaRectangle.Contains( pos ) ? 1.0 : 0.0;

								//var distance = areaRectangle.GetPointDistance( pos );
								//var factor = 1.0 - MathEx.Saturate( distance / borderSize );

								if( factor > 0 )
									layer.SetMaskValue( maskIndex, (float)factor );
							}
						}
					}
				}

			}

			////surfaces
			//if( AddSurfaces )
			//{
			//	for( int nSurface = 0; nSurface < templateData.Surfaces.Count; nSurface++ )
			//	{
			//		var surfaceItem = templateData.Surfaces[ nSurface ];

			//		var surface = surfaceItem.Surface;
			//		var surfaceV = surface.GetValue( this ).Value;
			//		if( surfaceV != null )
			//		{
			//			var element = groupOfObjects.CreateComponent<GroupOfObjectsElement_Surface>();
			//			element.Name = "Element " + ( nSurface + 1 ).ToString();
			//			element.Surface = surface;
			//			element.Index = nSurface;

			//			for( int n = 0; n < surfaceItem.PaintSteps; n++ )
			//				FillGroupOfObjects( scene, terrain, groupOfObjects, randomizer, surfaceV, element.Index );
			//		}
			//	}
			//}

			//if( LeveledArea )
			//{
			//	//clear objects from the group of objects
			//	{
			//		var borderSize = leveledAreaRectangle.Size.MaxComponent() / 20.0;
			//		if( borderSize < 0.001 )
			//			borderSize = 0.001;

			//		var rectangle = leveledAreaRectangle;
			//		rectangle.Expand( borderSize );

			//		var objectsToRemove = new List<int>();

			//		foreach( var objectIndex in groupOfObjects.ObjectsGetAll() )
			//		{
			//			ref var obj = ref groupOfObjects.ObjectGetData( objectIndex );

			//			if( rectangle.Contains( obj.Position.ToVector2() ) )
			//				objectsToRemove.Add( objectIndex );
			//		}

			//		groupOfObjects.ObjectsRemove( objectsToRemove.ToArray() );


			//		//works only when enabled:

			//		//var bounds = new Bounds( rectangle.Minimum.X, rectangle.Minimum.Y, double.MinValue, rectangle.Maximum.X, rectangle.Maximum.Y, double.MaxValue );
			//		//var getObjectsItem = new GroupOfObjects.GetObjectsItem( GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, false, bounds );

			//		//groupOfObjects.GetObjects( getObjectsItem );

			//		//foreach( var item in getObjectsItem.Result )
			//	}
			//}

			//enable components after update
			if( terrain != null )
				terrain.Enabled = true;
			if( groupOfObjects != null )
				groupOfObjects.Enabled = true;
		}

		//void FillGroupOfObjects( Scene scene, Terrain terrain, GroupOfObjects toGroupOfObjects, FastRandom random, Surface surface, int elementIndex )
		//{
		//	var terrainBounds = terrain.GetBounds2();
		//	var center = new Vector3( terrainBounds.GetCenter(), terrain.Position.Value.Z );

		//	var toolRadius = ( terrainBounds.Minimum - terrainBounds.GetCenter() ).Length();
		//	var toolStrength = 1.0;


		//	//when destination != null

		//	var destinationCachedBaseObjects = toGroupOfObjects.GetBaseObjects();

		//	//creating

		//	double maxOccupiedAreaRadius;
		//	double averageOccupiedAreaRadius;
		//	{
		//		var groups = surface.GetComponents<SurfaceGroupOfElements>();
		//		if( groups.Length != 0 )
		//		{
		//			maxOccupiedAreaRadius = 0;
		//			averageOccupiedAreaRadius = 0;
		//			foreach( var group in groups )
		//			{
		//				if( group.OccupiedAreaRadius > maxOccupiedAreaRadius )
		//					maxOccupiedAreaRadius = group.OccupiedAreaRadius;
		//				averageOccupiedAreaRadius += group.OccupiedAreaRadius;
		//			}
		//			averageOccupiedAreaRadius /= groups.Length;
		//		}
		//		else
		//		{
		//			maxOccupiedAreaRadius = 1;
		//			averageOccupiedAreaRadius = 1;
		//		}
		//	}

		//	//calculate object count
		//	int count;
		//	{
		//		var toolSquare = Math.PI * toolRadius * toolRadius;

		//		double radius = averageOccupiedAreaRadius;// minDistanceBetweenObjects / 2;
		//		double objectSquare = Math.PI * radius * radius;
		//		if( objectSquare < 0.1 )
		//			objectSquare = 0.1;

		//		double maxCount = toolSquare / objectSquare;
		//		maxCount /= 20;

		//		count = (int)( toolStrength * (double)maxCount );
		//		count = Math.Max( count, 1 );
		//	}

		//	var data = new List<GroupOfObjects.Object>( count );

		//	////find element
		//	//var element = toGroupOfObjects.GetComponents<GroupOfObjectsElement_Surface>().FirstOrDefault( e => e.Surface.Value == surface );
		//	//if( element == null )
		//	//	return;

		//	var totalBounds = new Bounds( center );
		//	totalBounds.Expand( toolRadius + maxOccupiedAreaRadius * 4.01 );

		//	var initSettings = new OctreeContainer.InitSettings();
		//	initSettings.InitialOctreeBounds = totalBounds;
		//	initSettings.OctreeBoundsRebuildExpand = Vector3.Zero;
		//	initSettings.MinNodeSize = totalBounds.GetSize() / 100;
		//	var octree = new OctreeContainer( initSettings );

		//	var octreeOccupiedAreas = new List<Sphere>( 256 );

		//	{
		//		var item = new GroupOfObjects.GetObjectsItem( GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, true, totalBounds );
		//		toGroupOfObjects.GetObjects( item );
		//		foreach( var resultItem in item.Result )
		//		{
		//			ref var obj = ref toGroupOfObjects.ObjectGetData( resultItem.Object );
		//			if( obj.Element == elementIndex )
		//			{
		//				var surfaceGroup = surface.GetGroup( obj.VariationGroup );
		//				if( surfaceGroup != null )
		//				{
		//					octreeOccupiedAreas.Add( new Sphere( obj.Position, surfaceGroup.OccupiedAreaRadius ) );

		//					var b = new Bounds( obj.Position );
		//					b.Expand( surfaceGroup.OccupiedAreaRadius * 4 );
		//					octree.AddObject( b, 1 );
		//				}
		//			}
		//		}
		//	}

		//	////create point container to check by MinDistanceBetweenObjects
		//	//PointContainer3D pointContainerFindFreePlace;
		//	//{
		//	//	double minDistanceBetweenObjectsMax = 0;
		//	//	foreach( var group in surface.GetComponents<SurfaceGroupOfElements>() )
		//	//		minDistanceBetweenObjectsMax = Math.Max( minDistanceBetweenObjectsMax, group.MinDistanceBetweenObjects );

		//	//	var bounds = new Bounds( center );
		//	//	bounds.Expand( toolRadius + minDistanceBetweenObjectsMax );
		//	//	pointContainerFindFreePlace = new PointContainer3D( bounds, 100 );

		//	//	var item = new GroupOfObjects.GetObjectsItem( GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, true, bounds );
		//	//	toGroupOfObjects.GetObjects( item );
		//	//	foreach( var resultItem in item.Result )
		//	//	{
		//	//		ref var obj = ref toGroupOfObjects.ObjectGetData( resultItem.Object );
		//	//		if( obj.Element == elementIndex )
		//	//			pointContainerFindFreePlace.Add( ref obj.Position );
		//	//	}
		//	//}

		//	for( int n = 0; n < count; n++ )
		//	{
		//		surface.GetRandomVariation( new Surface.GetRandomVariationOptions(), random, out var groupIndex, out var variationIndex, out var positionZ, out var rotation, out var scale );
		//		var surfaceGroup = surface.GetGroup( groupIndex );

		//		Vector3? position = null;

		//		for( var nRadiusMultiplier = 0; nRadiusMultiplier < 3; nRadiusMultiplier++ )
		//		{
		//			var radiusMultiplier = 1.0;
		//			switch( nRadiusMultiplier )
		//			{
		//			case 0: radiusMultiplier = 4; break;
		//			case 1: radiusMultiplier = 2; break;
		//			case 2: radiusMultiplier = 1; break;
		//			}

		//			int counter = 0;
		//			while( counter < 10 )
		//			{
		//				var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

		//				////check by radius and by hardness
		//				//var length = offset.Length();
		//				//if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
		//				//{

		//				var position2 = center.ToVector2() + offset;
		//				if( terrainBounds.Contains( position2 ) )
		//				{
		//					var result = SceneUtility.CalculateObjectPositionZ( scene, toGroupOfObjects, center.Z, position2, destinationCachedBaseObjects );
		//					if( result.found )
		//					{
		//						var p = new Vector3( position2, result.positionZ );

		//						var objSphere = new Sphere( p, surfaceGroup.OccupiedAreaRadius );
		//						objSphere.ToBounds( out var objBounds );

		//						var occupied = false;

		//						foreach( var index in octree.GetObjects( objBounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.All ) )
		//						{
		//							var sphere = octreeOccupiedAreas[ index ];
		//							sphere.Radius *= 0.25;//back to original
		//							sphere.Radius *= radiusMultiplier;//multiply

		//							if( ( p - sphere.Center ).LengthSquared() < ( sphere.Radius + objSphere.Radius ) * ( sphere.Radius + objSphere.Radius ) )
		//							{
		//								occupied = true;
		//								break;
		//							}
		//						}

		//						if( !occupied )
		//						{
		//							//found place to create
		//							position = p;
		//							goto end;
		//						}

		//						////check by MinDistanceBetweenObjects
		//						//if( surfaceGroup == null || !pointContainerFindFreePlace.Contains( new Sphere( p, surfaceGroup.MinDistanceBetweenObjects ) ) )
		//						//{
		//						//	//found place to create
		//						//	position = p;
		//						//	break;
		//						//}
		//					}
		//				}

		//				//}

		//				counter++;
		//			}
		//		}

		//		end:;

		//		if( position != null )
		//		{
		//			var obj = new GroupOfObjects.Object();
		//			obj.Element = (ushort)elementIndex;
		//			obj.VariationGroup = groupIndex;
		//			obj.VariationElement = variationIndex;
		//			obj.Flags = GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible;
		//			obj.Position = position.Value + new Vector3( 0, 0, positionZ );
		//			obj.Rotation = rotation;
		//			obj.Scale = scale;
		//			obj.Color = ColorValue.One;
		//			data.Add( obj );

		//			//add to the octree

		//			octreeOccupiedAreas.Add( new Sphere( position.Value, surfaceGroup.OccupiedAreaRadius ) );

		//			var b = new Bounds( position.Value );
		//			b.Expand( surfaceGroup.OccupiedAreaRadius * 4 );
		//			octree.AddObject( b, 1 );

		//			////add to point container
		//			//pointContainerFindFreePlace.Add( ref obj.Position );
		//		}
		//	}

		//	octree.Dispose();

		//	//createByBrushGroupOfObjects = toGroupOfObjects;

		//	var newIndexes = toGroupOfObjects.ObjectsAdd( data.ToArray() );
		//	//createByBrushGroupOfObjectsObjectsCreated.AddRange( newIndexes );
		//}

#endif

	}
}
