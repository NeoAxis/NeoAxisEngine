// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// A tool for procedural scene generation.
	/// </summary>
	[EditorSettingsCell( typeof( Component_WorldGenerator_SettingsCell ) )]
	public class Component_WorldGenerator : Component
	{
		/// <summary>
		/// The size of the generated area.
		/// </summary>
		[DefaultValue( 2000.0 )]
		[Range( 100, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Geometry" )]
		public Reference<double> Size
		{
			get { if( _size.BeginGet() ) Size = _size.Get( this ); return _size.value; }
			set { if( _size.BeginSet( ref value ) ) { try { SizeChanged?.Invoke( this ); } finally { _size.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Size"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> SizeChanged;
		ReferenceField<double> _size = 2000.0;

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
		public event Action<Component_WorldGenerator> HeightRangeChanged;
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
		public event Action<Component_WorldGenerator> TemplateChanged;
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
		//public event Action<Component_WorldGenerator> AddDetailsChanged;
		//ReferenceField<bool> _addDetails = true;
		
		const string terrainMaterialDefault = @"Content\Materials\Basic Library\Ground\Forest Ground 01.material";
		/// <summary>
		/// The material of the terrain.
		/// </summary>
		[DefaultValueReference( terrainMaterialDefault )]
		[Category( "Template" )]
		public Reference<Component_Material> TerrainMaterial
		{
			get { if( _terrainMaterial.BeginGet() ) TerrainMaterial = _terrainMaterial.Get( this ); return _terrainMaterial.value; }
			set { if( _terrainMaterial.BeginSet( ref value ) ) { try { TerrainMaterialChanged?.Invoke( this ); } finally { _terrainMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TerrainMaterial"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> TerrainMaterialChanged;
		ReferenceField<Component_Material> _terrainMaterial = new Reference<Component_Material>( null, terrainMaterialDefault );

		const string surface1Default = @"Content\Vegetation\Models\Woody plant\Common oak\Common oak.surface";
		/// <summary>
		/// The first surface.
		/// </summary>
		[DefaultValueReference( surface1Default )]
		[DisplayName( "Surface 1" )]
		[Category( "Template" )]
		public Reference<Component_Surface> Surface1
		{
			get { if( _surface1.BeginGet() ) Surface1 = _surface1.Get( this ); return _surface1.value; }
			set { if( _surface1.BeginSet( ref value ) ) { try { Surface1Changed?.Invoke( this ); } finally { _surface1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface1"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> Surface1Changed;
		ReferenceField<Component_Surface> _surface1 = new Reference<Component_Surface>( null, surface1Default );

		const int paintStepsDefault = 5;

		/// <summary>
		/// The number of painting iterations for the first surface.
		/// </summary>
		[DefaultValue( paintStepsDefault )]
		[Range( 1, 20 )]
		[DisplayName( "Surface 1 Paint Steps" )]
		[Category( "Template" )]
		public Reference<int> Surface1PaintSteps
		{
			get { if( _surface1PaintSteps.BeginGet() ) Surface1PaintSteps = _surface1PaintSteps.Get( this ); return _surface1PaintSteps.value; }
			set { if( _surface1PaintSteps.BeginSet( ref value ) ) { try { Surface1PaintStepsChanged?.Invoke( this ); } finally { _surface1PaintSteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface1PaintSteps"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> Surface1PaintStepsChanged;
		ReferenceField<int> _surface1PaintSteps = paintStepsDefault;

		const string surface2Default = @"Content\Models\Rocks\Rocks (4 pieces)\Rocks.surface";
		/// <summary>
		/// The second surface.
		/// </summary>
		[DefaultValue( surface2Default )]
		[DisplayName( "Surface 2" )]
		[Category( "Template" )]
		public Reference<Component_Surface> Surface2
		{
			get { if( _surface2.BeginGet() ) Surface2 = _surface2.Get( this ); return _surface2.value; }
			set { if( _surface2.BeginSet( ref value ) ) { try { Surface2Changed?.Invoke( this ); } finally { _surface2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface2"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> Surface2Changed;
		ReferenceField<Component_Surface> _surface2 = new Reference<Component_Surface>( null, surface2Default );

		/// <summary>
		/// The number of painting iterations for the second surface.
		/// </summary>
		[DefaultValue( paintStepsDefault )]
		[Range( 1, 20 )]
		[DisplayName( "Surface 2 Paint Steps" )]
		[Category( "Template" )]
		public Reference<int> Surface2PaintSteps
		{
			get { if( _surface2PaintSteps.BeginGet() ) Surface2PaintSteps = _surface2PaintSteps.Get( this ); return _surface2PaintSteps.value; }
			set { if( _surface2PaintSteps.BeginSet( ref value ) ) { try { Surface2PaintStepsChanged?.Invoke( this ); } finally { _surface2PaintSteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface2PaintSteps"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> Surface2PaintStepsChanged;
		ReferenceField<int> _surface2PaintSteps = paintStepsDefault;

		const string surface3Default = "";
		/// <summary>
		/// The third surface.
		/// </summary>
		[DefaultValueReference( null )]
		[DisplayName( "Surface 3" )]
		[Category( "Template" )]
		public Reference<Component_Surface> Surface3
		{
			get { if( _surface3.BeginGet() ) Surface3 = _surface3.Get( this ); return _surface3.value; }
			set { if( _surface3.BeginSet( ref value ) ) { try { Surface3Changed?.Invoke( this ); } finally { _surface3.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface3"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> Surface3Changed;
		ReferenceField<Component_Surface> _surface3 = new Reference<Component_Surface>( null, surface3Default );

		/// <summary>
		/// The number of painting iterations for the third surface.
		/// </summary>
		[DefaultValue( paintStepsDefault )]
		[Range( 1, 20 )]
		[DisplayName( "Surface 3 Paint Steps" )]
		[Category( "Template" )]
		public Reference<int> Surface3PaintSteps
		{
			get { if( _surface3PaintSteps.BeginGet() ) Surface3PaintSteps = _surface3PaintSteps.Get( this ); return _surface3PaintSteps.value; }
			set { if( _surface3PaintSteps.BeginSet( ref value ) ) { try { Surface3PaintStepsChanged?.Invoke( this ); } finally { _surface3PaintSteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface3PaintSteps"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> Surface3PaintStepsChanged;
		ReferenceField<int> _surface3PaintSteps = paintStepsDefault;

		const string surface4Default = "";
		/// <summary>
		/// The fourth surface.
		/// </summary>
		[DefaultValueReference( surface4Default )]
		[DisplayName( "Surface 4" )]
		[Category( "Template" )]
		public Reference<Component_Surface> Surface4
		{
			get { if( _surface4.BeginGet() ) Surface4 = _surface4.Get( this ); return _surface4.value; }
			set { if( _surface4.BeginSet( ref value ) ) { try { Surface4Changed?.Invoke( this ); } finally { _surface4.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface4"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> Surface4Changed;
		ReferenceField<Component_Surface> _surface4 = new Reference<Component_Surface>( null, surface4Default );

		/// <summary>
		/// The number of painting iterations for the fourth surface.
		/// </summary>
		[DefaultValue( paintStepsDefault )]
		[Range( 1, 20 )]
		[DisplayName( "Surface 4 Paint Steps" )]
		[Category( "Template" )]
		public Reference<int> Surface4PaintSteps
		{
			get { if( _surface4PaintSteps.BeginGet() ) Surface4PaintSteps = _surface4PaintSteps.Get( this ); return _surface4PaintSteps.value; }
			set { if( _surface4PaintSteps.BeginSet( ref value ) ) { try { Surface4PaintStepsChanged?.Invoke( this ); } finally { _surface4PaintSteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface4PaintSteps"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> Surface4PaintStepsChanged;
		ReferenceField<int> _surface4PaintSteps = paintStepsDefault;


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
		public event Action<Component_WorldGenerator> LeveledAreaChanged;
		ReferenceField<bool> _leveledArea = true;

		/// <summary>
		/// The size of leveled area.
		/// </summary>
		[DefaultValue( "200 200" )]
		[Category( "Leveled Area" )]
		[Range( 10, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<Vector2> LeveledAreaSize
		{
			get { if( _leveledAreaSize.BeginGet() ) LeveledAreaSize = _leveledAreaSize.Get( this ); return _leveledAreaSize.value; }
			set { if( _leveledAreaSize.BeginSet( ref value ) ) { try { LeveledAreaSizeChanged?.Invoke( this ); } finally { _leveledAreaSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeveledAreaSize"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> LeveledAreaSizeChanged;
		ReferenceField<Vector2> _leveledAreaSize = new Vector2( 200, 200 );

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
		public event Action<Component_WorldGenerator> LeveledAreaHeightChanged;
		ReferenceField<double> _leveledAreaHeight = leveledAreaHeightDefault;

		const string leveledAreaMaterialDefault = "Content\\Materials\\Basic Library\\Ground\\Forest Ground 03.material";
		/// <summary>
		/// The material of the leveled area.
		/// </summary>
		[DefaultValueReference( leveledAreaMaterialDefault )]
		[Category( "Leveled Area" )]
		public Reference<Component_Material> LeveledAreaMaterial
		{
			get { if( _leveledAreaMaterial.BeginGet() ) LeveledAreaMaterial = _leveledAreaMaterial.Get( this ); return _leveledAreaMaterial.value; }
			set { if( _leveledAreaMaterial.BeginSet( ref value ) ) { try { LeveledAreaMaterialChanged?.Invoke( this ); } finally { _leveledAreaMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeveledAreaMaterial"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> LeveledAreaMaterialChanged;
		ReferenceField<Component_Material> _leveledAreaMaterial = new Reference<Component_Material>( null, leveledAreaMaterialDefault );

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
		public event Action<Component_WorldGenerator> SeedChanged;
		ReferenceField<int> _seed = 0;

		/// <summary>
		/// Whether to generate objects by the surfaces.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Generator" )]
		public Reference<bool> AddSurfaces
		{
			get { if( _addSurfaces.BeginGet() ) AddSurfaces = _addSurfaces.Get( this ); return _addSurfaces.value; }
			set { if( _addSurfaces.BeginSet( ref value ) ) { try { AddSurfacesChanged?.Invoke( this ); } finally { _addSurfaces.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AddSurfaces"/> property value changes.</summary>
		public event Action<Component_WorldGenerator> AddSurfacesChanged;
		ReferenceField<bool> _addSurfaces = true;

		///////////////////////////////////////////////

		public class TemplateData
		{
			public Reference<Component_Material> TerrainMaterial;
			public List<SurfaceItem> Surfaces = new List<SurfaceItem>();

			//

			public class SurfaceItem
			{
				public Reference<Component_Surface> Surface;
				public int PaintSteps;

				public SurfaceItem()
				{
				}

				public SurfaceItem( Reference<Component_Surface> surface, int paintSteps )
				{
					Surface = surface;
					PaintSteps = paintSteps;
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

				case nameof( Surface1PaintSteps ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface1.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Surface2PaintSteps ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface2.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Surface3PaintSteps ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface3.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Surface4PaintSteps ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface4.ReferenceSpecified )
						skip = true;
					break;

				case nameof( LeveledAreaSize ):
				case nameof( LeveledAreaHeight ):
				case nameof( LeveledAreaMaterial ):
					if( !LeveledArea )
						skip = true;
					break;

				}
			}
		}

		//TemplateData

		public delegate void GetTemplateDataDelegate( Component_WorldGenerator sender, ref TemplateData data );
		public event GetTemplateDataDelegate GetTemplateData;

		TemplateData GetTemplateDataDefault()
		{
			var data = new TemplateData();

			switch( Template.Value )
			{
			case TemplateEnum.OakForest:
				{
					data.TerrainMaterial = new Reference<Component_Material>( null, terrainMaterialDefault );

					var surface1 = new Reference<Component_Surface>( null, surface1Default );
					data.Surfaces.Add( new TemplateData.SurfaceItem( surface1, paintStepsDefault ) );

					var surface2 = new Reference<Component_Surface>( null, surface2Default );
					data.Surfaces.Add( new TemplateData.SurfaceItem( surface2, paintStepsDefault ) );
				}
				break;

			default:

				data.TerrainMaterial = TerrainMaterial;

				for( int nSurface = 0; nSurface < 4; nSurface++ )
				{
					Reference<Component_Surface> surface = null;
					var paintSteps = 1;
					switch( nSurface )
					{
					case 0: surface = Surface1; paintSteps = Surface1PaintSteps; break;
					case 1: surface = Surface2; paintSteps = Surface2PaintSteps; break;
					case 2: surface = Surface3; paintSteps = Surface3PaintSteps; break;
					case 3: surface = Surface4; paintSteps = Surface4PaintSteps; break;
					}

					if( surface.ReferenceSpecified )
						data.Surfaces.Add( new TemplateData.SurfaceItem( surface, paintSteps ) );
				}
				break;
			}

			return data;
		}

		public virtual void Generate( DocumentInstance document )
		{
			var scene = FindParent<Component_Scene>();
			if( scene == null )
				return;

			TemplateData templateData = null;
			GetTemplateData?.Invoke( this, ref templateData );
			if( templateData == null )
				templateData = GetTemplateDataDefault();

			var randomizer = new Random( Seed );

			var terrain = scene.GetComponent<Component_Terrain>( true );
			var groupOfObjects = scene.GetComponent<Component_GroupOfObjects>( true );

			//disable components before update
			if( terrain != null )
				terrain.Enabled = false;
			if( groupOfObjects != null )
				groupOfObjects.Enabled = false;

			//update terrain

			if( terrain == null )
			{
				terrain = scene.CreateComponent<Component_Terrain>( enabled: false );
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
				groupOfObjects = scene.CreateComponent<Component_GroupOfObjects>( enabled: false );
				groupOfObjects.Name = "Group Of Objects";
			}


			var undoMultiAction = new UndoMultiAction();

			//clear
			var indexes = groupOfObjects.ObjectsGetAll();
			if( indexes.Count != 0 )
			{
				var action = new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true );
				undoMultiAction.AddAction( action );
			}
			var components = groupOfObjects.GetComponents();
			undoMultiAction.AddAction( new UndoActionComponentCreateDelete( document, components, false ) );


			if( undoMultiAction.Actions.Count != 0 )
				document.CommitUndoAction( undoMultiAction );

			//update base objects
			if( terrain != null && !groupOfObjects.GetBaseObjects().Contains( terrain ) )
				groupOfObjects.BaseObjects.Add( ReferenceUtility.MakeRootReference( terrain ) );

			Rectangle leveledAreaRectangle = Rectangle.Zero;

			//delete old layers
			if( terrain != null )
			{
				foreach( var c in terrain.GetComponents<Component_PaintLayer>() )
					c.RemoveFromParent( false );
			}

			if( LeveledArea )
			{
				leveledAreaRectangle = new Rectangle( -LeveledAreaSize.Value / 2, LeveledAreaSize.Value / 2 );

				//change terrain height
				{
					var borderSize = leveledAreaRectangle.Size.MaxComponent() / 10.0;
					if( borderSize < 0.001 )
						borderSize = 0.001;

					var min = terrain.GetCellIndexByPosition( leveledAreaRectangle.Minimum - new Vector2( borderSize, borderSize ) ) - new Vector2I( 1, 1 );
					var max = terrain.GetCellIndexByPosition( leveledAreaRectangle.Maximum + new Vector2( borderSize, borderSize ) ) + new Vector2I( 2, 2 );
					MathEx.Clamp( min.X, 0, heightmapSize - 1 );
					MathEx.Clamp( min.Y, 0, heightmapSize - 1 );
					MathEx.Clamp( max.X, 0, heightmapSize - 1 );
					MathEx.Clamp( max.Y, 0, heightmapSize - 1 );

					for( int y = min.Y; y <= max.Y; y++ )
					{
						for( int x = min.X; x <= max.X; x++ )
						{
							var cellIndex = new Vector2I( x, y );
							var pos = terrain.GetPositionXY( cellIndex );

							var distance = leveledAreaRectangle.GetPointDistance( pos );

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
					var layer = terrain.CreateComponent<Component_PaintLayer>();
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

								var factor = leveledAreaRectangle.Contains( pos ) ? 1.0 : 0.0;
								//var distance = areaRectangle.GetPointDistance( pos );
								//var factor = 1.0 - MathEx.Saturate( distance / borderSize );

								if( factor > 0 )
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

			//surfaces
			if( AddSurfaces )
			{
				for( int nSurface = 0; nSurface < templateData.Surfaces.Count; nSurface++ )
				{
					var surfaceItem = templateData.Surfaces[ nSurface ];

					var surface = surfaceItem.Surface;
					var surfaceV = surface.GetValue( this ).Value;
					if( surfaceV != null )
					{
						var element = groupOfObjects.CreateComponent<Component_GroupOfObjectsElement_Surface>();
						element.Name = "Element " + ( nSurface + 1 ).ToString();
						element.Surface = surface;
						element.Index = nSurface;

						for( int n = 0; n < surfaceItem.PaintSteps; n++ )
							FillGroupOfObjects( scene, terrain, groupOfObjects, randomizer, surfaceV, element.Index );
					}
				}
			}

			if( LeveledArea )
			{
				//clear objects from the group of objects
				{
					var borderSize = leveledAreaRectangle.Size.MaxComponent() / 20.0;
					if( borderSize < 0.001 )
						borderSize = 0.001;

					var rectangle = leveledAreaRectangle;
					rectangle.Expand( borderSize );

					var objectsToRemove = new List<int>();

					foreach( var objectIndex in groupOfObjects.ObjectsGetAll() )
					{
						ref var obj = ref groupOfObjects.ObjectGetData( objectIndex );

						if( rectangle.Contains( obj.Position.ToVector2() ) )
							objectsToRemove.Add( objectIndex );
					}

					groupOfObjects.ObjectsRemove( objectsToRemove.ToArray() );


					//works only when enabled:

					//var bounds = new Bounds( rectangle.Minimum.X, rectangle.Minimum.Y, double.MinValue, rectangle.Maximum.X, rectangle.Maximum.Y, double.MaxValue );
					//var getObjectsItem = new Component_GroupOfObjects.GetObjectsItem( Component_GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, false, bounds );

					//groupOfObjects.GetObjects( getObjectsItem );

					//foreach( var item in getObjectsItem.Result )
				}
			}

			//enable components after update
			if( terrain != null )
				terrain.Enabled = true;
			if( groupOfObjects != null )
				groupOfObjects.Enabled = true;
		}

		void FillGroupOfObjects( Component_Scene scene, Component_Terrain terrain, Component_GroupOfObjects toGroupOfObjects, Random random, Component_Surface surface, int elementIndex )
		{
			var terrainBounds = terrain.GetBounds2();
			var center = new Vector3( terrainBounds.GetCenter(), terrain.Position.Value.Z );

			var toolRadius = ( terrainBounds.Minimum - terrainBounds.GetCenter() ).Length();
			var toolStrength = 1.0;


			//when destination != null

			var destinationCachedBaseObjects = toGroupOfObjects.GetBaseObjects();

			//creating

			//calculate object count
			int count;
			{
				var toolSquare = Math.PI * toolRadius * toolRadius;

				//!!!!average from all groups
				double minDistanceBetweenObjects;
				{
					var groups = surface.GetComponents<Component_SurfaceGroupOfElements>();
					if( groups.Length != 0 )
					{
						minDistanceBetweenObjects = 0;
						foreach( var group in groups )
							minDistanceBetweenObjects += group.MinDistanceBetweenObjects;
						minDistanceBetweenObjects /= groups.Length;
					}
					else
						minDistanceBetweenObjects = 1;
				}

				double radius = minDistanceBetweenObjects / 2;
				double objectSquare = Math.PI * radius * radius;
				if( objectSquare < 0.1 )
					objectSquare = 0.1;

				double maxCount = toolSquare / objectSquare;
				maxCount /= 10;

				count = (int)( toolStrength * (double)maxCount );
				count = Math.Max( count, 1 );
			}

			var data = new List<Component_GroupOfObjects.Object>( count );

			////find element
			//var element = toGroupOfObjects.GetComponents<Component_GroupOfObjectsElement_Surface>().FirstOrDefault( e => e.Surface.Value == surface );
			//if( element == null )
			//	return;

			//create point container to check by MinDistanceBetweenObjects
			PointContainer3D pointContainerFindFreePlace;
			{
				double minDistanceBetweenObjectsMax = 0;
				foreach( var group in surface.GetComponents<Component_SurfaceGroupOfElements>() )
					minDistanceBetweenObjectsMax = Math.Max( minDistanceBetweenObjectsMax, group.MinDistanceBetweenObjects );

				var bounds = new Bounds( center );
				bounds.Expand( toolRadius + minDistanceBetweenObjectsMax );
				pointContainerFindFreePlace = new PointContainer3D( bounds, 100 );

				var item = new Component_GroupOfObjects.GetObjectsItem( Component_GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, true, bounds );
				toGroupOfObjects.GetObjects( item );
				foreach( var resultItem in item.Result )
				{
					ref var obj = ref toGroupOfObjects.ObjectGetData( resultItem.Object );
					if( obj.Element == elementIndex )
						pointContainerFindFreePlace.Add( ref obj.Position );
				}
			}

			for( int n = 0; n < count; n++ )
			{
				surface.GetRandomVariation( new Component_Surface.GetRandomVariationOptions(), random, out var groupIndex, out var variationIndex, out var positionZ, out var rotation, out var scale );
				var surfaceGroup = surface.GetGroup( groupIndex );

				Vector3? position = null;

				int counter = 0;
				while( counter < 20 )
				{
					var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

					////check by radius and by hardness
					//var length = offset.Length();
					//if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
					//{
					var position2 = center.ToVector2() + offset;
					if( terrainBounds.Contains( position2 ) )
					{
						var result = Component_Scene_Utility.CalculateObjectPositionZ( scene, toGroupOfObjects, center.Z, position2, destinationCachedBaseObjects );
						if( result.found )
						{
							var p = new Vector3( position2, result.positionZ );

							//check by MinDistanceBetweenObjects
							if( surfaceGroup == null || !pointContainerFindFreePlace.Contains( new Sphere( p, surfaceGroup.MinDistanceBetweenObjects ) ) )
							{
								//found place to create
								position = p;
								break;
							}
						}
					}
					//}

					counter++;
				}

				if( position != null )
				{
					var obj = new Component_GroupOfObjects.Object();
					obj.Element = (ushort)elementIndex;
					obj.VariationGroup = groupIndex;
					obj.VariationElement = variationIndex;
					obj.Flags = Component_GroupOfObjects.Object.FlagsEnum.Enabled | Component_GroupOfObjects.Object.FlagsEnum.Visible;
					obj.Position = position.Value + new Vector3( 0, 0, positionZ );
					obj.Rotation = rotation;
					obj.Scale = scale;
					obj.Color = ColorValue.One;
					data.Add( obj );

					//add to point container
					pointContainerFindFreePlace.Add( ref obj.Position );
				}
			}

			//createByBrushGroupOfObjects = toGroupOfObjects;

			var newIndexes = toGroupOfObjects.ObjectsAdd( data.ToArray() );
			//createByBrushGroupOfObjectsObjectsCreated.AddRange( newIndexes );
		}
	}
}
