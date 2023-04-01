// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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

		/// <summary>
		/// The curvature frequency of the terrain.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[Category( "Geometry" )]
		public Reference<double> Curvature
		{
			get { if( _curvature.BeginGet() ) Curvature = _curvature.Get( this ); return _curvature.value; }
			set { if( _curvature.BeginSet( ref value ) ) { try { CurvatureChanged?.Invoke( this ); } finally { _curvature.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Curvature"/> property value changes.</summary>
		public event Action<WorldGenerator> CurvatureChanged;
		ReferenceField<double> _curvature = 1.0;

		public enum TemplateEnum
		{
			OakForest,
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


		const string baseMaterialDefault = @"Content\Materials\Basic Library\Ground\Grass 01.material";
		/// <summary>
		/// The material of the terrain.
		/// </summary>
		[DefaultValueReference( baseMaterialDefault )]
		[Category( "Template" )]
		public Reference<Material> BaseMaterial
		{
			get { if( _baseMaterial.BeginGet() ) BaseMaterial = _baseMaterial.Get( this ); return _baseMaterial.value; }
			set { if( _baseMaterial.BeginSet( ref value ) ) { try { BaseMaterialChanged?.Invoke( this ); } finally { _baseMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseMaterial"/> property value changes.</summary>
		public event Action<WorldGenerator> BaseMaterialChanged;
		ReferenceField<Material> _baseMaterial = new Reference<Material>( null, baseMaterialDefault );

		/// <summary>
		/// The color multiplier of the terrain.
		/// </summary>
		[Category( "Template" )]
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> BaseMaterialColor
		{
			get { if( _baseMaterialColor.BeginGet() ) BaseMaterialColor = _baseMaterialColor.Get( this ); return _baseMaterialColor.value; }
			set { if( _baseMaterialColor.BeginSet( ref value ) ) { try { BaseMaterialColorChanged?.Invoke( this ); } finally { _baseMaterialColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseMaterialColor"/> property value changes.</summary>
		public event Action<WorldGenerator> BaseMaterialColorChanged;
		ReferenceField<ColorValue> _baseMaterialColor = ColorValue.One;


		const string baseSurfaceDefault = @"Content\Vegetation\Models\Flowering plant\Poa annua\Poa annua.surface";
		//const string baseSurfaceDefault = "";
		/// <summary>
		/// The third surface.
		/// </summary>
		[DefaultValueReference( baseSurfaceDefault )]
		[Category( "Template" )]
		public Reference<Surface> BaseSurface
		{
			get { if( _baseSurface.BeginGet() ) BaseSurface = _baseSurface.Get( this ); return _baseSurface.value; }
			set { if( _baseSurface.BeginSet( ref value ) ) { try { BaseSurfaceChanged?.Invoke( this ); } finally { _baseSurface.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseSurface"/> property value changes.</summary>
		public event Action<WorldGenerator> BaseSurfaceChanged;
		ReferenceField<Surface> _baseSurface = new Reference<Surface>( null, baseSurfaceDefault );

		/// <summary>
		/// The strength of painting for the third surface.
		/// </summary>
		[DefaultValue( 0.75 )]
		[Range( 0, 1 )]
		[Category( "Template" )]
		public Reference<double> BaseSurfacePaintFactor
		{
			get { if( _baseSurfacePaintFactor.BeginGet() ) BaseSurfacePaintFactor = _baseSurfacePaintFactor.Get( this ); return _baseSurfacePaintFactor.value; }
			set { if( _baseSurfacePaintFactor.BeginSet( ref value ) ) { try { BaseSurfacePaintFactorChanged?.Invoke( this ); } finally { _baseSurfacePaintFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseSurfacePaintFactor"/> property value changes.</summary>
		public event Action<WorldGenerator> BaseSurfacePaintFactorChanged;
		ReferenceField<double> _baseSurfacePaintFactor = 0.75;

		/// <summary>
		/// The color of the first surface.
		/// </summary>
		[DefaultValue( "0.57 0.75 0.49" )]
		[Category( "Template" )]
		public Reference<ColorValue> BaseSurfaceColor
		{
			get { if( _baseSurfaceColor.BeginGet() ) BaseSurfaceColor = _baseSurfaceColor.Get( this ); return _baseSurfaceColor.value; }
			set { if( _baseSurfaceColor.BeginSet( ref value ) ) { try { BaseSurfaceColorChanged?.Invoke( this ); } finally { _baseSurfaceColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseSurfaceColor"/> property value changes.</summary>
		public event Action<WorldGenerator> BaseSurfaceColorChanged;
		ReferenceField<ColorValue> _baseSurfaceColor = new ColorValue( 0.57, 0.75, 0.49 );

		/// <summary>
		/// Whether to enable the collision for the base surface.
		/// </summary>
		[Category( "Template" )]
		[DefaultValue( false )]
		public Reference<bool> BaseSurfaceCollision
		{
			get { if( _baseSurfaceCollision.BeginGet() ) BaseSurfaceCollision = _baseSurfaceCollision.Get( this ); return _baseSurfaceCollision.value; }
			set { if( _baseSurfaceCollision.BeginSet( ref value ) ) { try { BaseSurfaceCollisionChanged?.Invoke( this ); } finally { _baseSurfaceCollision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseSurfaceCollision"/> property value changes.</summary>
		public event Action<WorldGenerator> BaseSurfaceCollisionChanged;
		ReferenceField<bool> _baseSurfaceCollision = false;


		const string surface1Default = @"Content\Vegetation\Models\Woody plant\Quercus robur\Quercus robur.surface";
		/// <summary>
		/// The first surface.
		/// </summary>
		[DefaultValueReference( surface1Default )]
		[DisplayName( "Surface 1" )]
		[Category( "Surface 1" )]
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
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[DisplayName( "Surface 1 Paint Factor" )]
		[Category( "Surface 1" )]
		public Reference<double> Surface1PaintFactor
		{
			get { if( _surface1PaintFactor.BeginGet() ) Surface1PaintFactor = _surface1PaintFactor.Get( this ); return _surface1PaintFactor.value; }
			set { if( _surface1PaintFactor.BeginSet( ref value ) ) { try { Surface1PaintFactorChanged?.Invoke( this ); } finally { _surface1PaintFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface1PaintFactor"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface1PaintFactorChanged;
		ReferenceField<double> _surface1PaintFactor = 0.5;

		[DisplayName( "Surface 1 Distribution" )]
		[Category( "Surface 1" )]
		[DefaultValue( 1.0 )]
		public Reference<double> Surface1Distribution
		{
			get { if( _surface1Distribution.BeginGet() ) Surface1Distribution = _surface1Distribution.Get( this ); return _surface1Distribution.value; }
			set { if( _surface1Distribution.BeginSet( ref value ) ) { try { Surface1DistributionChanged?.Invoke( this ); } finally { _surface1Distribution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface1Distribution"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface1DistributionChanged;
		ReferenceField<double> _surface1Distribution = 1.0;

		/// <summary>
		/// The color of the first surface.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[DisplayName( "Surface 1 Color" )]
		[Category( "Surface 1" )]
		public Reference<ColorValue> Surface1Color
		{
			get { if( _surface1Color.BeginGet() ) Surface1Color = _surface1Color.Get( this ); return _surface1Color.value; }
			set { if( _surface1Color.BeginSet( ref value ) ) { try { Surface1ColorChanged?.Invoke( this ); } finally { _surface1Color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface1Color"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface1ColorChanged;
		ReferenceField<ColorValue> _surface1Color = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// Whether to enable the collision for the first surface.
		/// </summary>
		[Category( "Surface 1" )]
		[DefaultValue( true )]
		public Reference<bool> Surface1Collision
		{
			get { if( _surface1Collision.BeginGet() ) Surface1Collision = _surface1Collision.Get( this ); return _surface1Collision.value; }
			set { if( _surface1Collision.BeginSet( ref value ) ) { try { Surface1CollisionChanged?.Invoke( this ); } finally { _surface1Collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface1Collision"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface1CollisionChanged;
		ReferenceField<bool> _surface1Collision = true;


		const string surface2Default = @"Content\Models\Rocks\Rocks (4 pieces)\Rocks.surface";
		/// <summary>
		/// The second surface.
		/// </summary>
		[DefaultValueReference( surface2Default )]
		[DisplayName( "Surface 2" )]
		[Category( "Surface 2" )]
		public Reference<Surface> Surface2
		{
			get { if( _surface2.BeginGet() ) Surface2 = _surface2.Get( this ); return _surface2.value; }
			set { if( _surface2.BeginSet( ref value ) ) { try { Surface2Changed?.Invoke( this ); } finally { _surface2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface2"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface2Changed;
		ReferenceField<Surface> _surface2 = new Reference<Surface>( null, surface2Default );

		/// <summary>
		/// The color of the second surface.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[DisplayName( "Surface 2 Color" )]
		[Category( "Surface 2" )]
		public Reference<ColorValue> Surface2Color
		{
			get { if( _surface2Color.BeginGet() ) Surface2Color = _surface2Color.Get( this ); return _surface2Color.value; }
			set { if( _surface2Color.BeginSet( ref value ) ) { try { Surface2ColorChanged?.Invoke( this ); } finally { _surface2Color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface2Color"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface2ColorChanged;
		ReferenceField<ColorValue> _surface2Color = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// The strength of painting for the second surface.
		/// </summary>
		[DefaultValue( 0.02 )]
		[Range( 0, 1 )]
		[DisplayName( "Surface 2 Paint Factor" )]
		[Category( "Surface 2" )]
		public Reference<double> Surface2PaintFactor
		{
			get { if( _surface2PaintFactor.BeginGet() ) Surface2PaintFactor = _surface2PaintFactor.Get( this ); return _surface2PaintFactor.value; }
			set { if( _surface2PaintFactor.BeginSet( ref value ) ) { try { Surface2PaintFactorChanged?.Invoke( this ); } finally { _surface2PaintFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface2PaintFactor"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface2PaintFactorChanged;
		ReferenceField<double> _surface2PaintFactor = 0.02;

		[DisplayName( "Surface 2 Distribution" )]
		[Category( "Surface 2" )]
		[DefaultValue( 1.0 )]
		public Reference<double> Surface2Distribution
		{
			get { if( _surface2Distribution.BeginGet() ) Surface2Distribution = _surface2Distribution.Get( this ); return _surface2Distribution.value; }
			set { if( _surface2Distribution.BeginSet( ref value ) ) { try { Surface2DistributionChanged?.Invoke( this ); } finally { _surface2Distribution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface2Distribution"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface2DistributionChanged;
		ReferenceField<double> _surface2Distribution = 1.0;

		/// <summary>
		/// Whether to enable the collision for the second surface.
		/// </summary>
		[Category( "Surface 2" )]
		[DisplayName( "Surface 2 Collision" )]
		[DefaultValue( true )]
		public Reference<bool> Surface2Collision
		{
			get { if( _surface2Collision.BeginGet() ) Surface2Collision = _surface2Collision.Get( this ); return _surface2Collision.value; }
			set { if( _surface2Collision.BeginSet( ref value ) ) { try { Surface2CollisionChanged?.Invoke( this ); } finally { _surface2Collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface2Collision"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface2CollisionChanged;
		ReferenceField<bool> _surface2Collision = true;


		const string surface3Default = @"Content\Vegetation\Models\Flowering plant\Matricaria chamomilla\Matricaria chamomilla.surface";
		/// <summary>
		/// The third surface.
		/// </summary>
		[DefaultValueReference( surface3Default )]
		[DisplayName( "Surface 3" )]
		[Category( "Surface 3" )]
		public Reference<Surface> Surface3
		{
			get { if( _surface3.BeginGet() ) Surface3 = _surface3.Get( this ); return _surface3.value; }
			set { if( _surface3.BeginSet( ref value ) ) { try { Surface3Changed?.Invoke( this ); } finally { _surface3.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface3"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface3Changed;
		ReferenceField<Surface> _surface3 = new Reference<Surface>( null, surface3Default );

		/// <summary>
		/// The color of the third surface.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[DisplayName( "Surface 3 Color" )]
		[Category( "Surface 3" )]
		public Reference<ColorValue> Surface3Color
		{
			get { if( _surface3Color.BeginGet() ) Surface3Color = _surface3Color.Get( this ); return _surface3Color.value; }
			set { if( _surface3Color.BeginSet( ref value ) ) { try { Surface3ColorChanged?.Invoke( this ); } finally { _surface3Color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface3Color"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface3ColorChanged;
		ReferenceField<ColorValue> _surface3Color = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// The strength of painting for the third surface.
		/// </summary>
		[DefaultValue( 0.15 )]
		[Range( 0, 1 )]
		[DisplayName( "Surface 3 Paint Factor" )]
		[Category( "Surface 3" )]
		public Reference<double> Surface3PaintFactor
		{
			get { if( _surface3PaintFactor.BeginGet() ) Surface3PaintFactor = _surface3PaintFactor.Get( this ); return _surface3PaintFactor.value; }
			set { if( _surface3PaintFactor.BeginSet( ref value ) ) { try { Surface3PaintFactorChanged?.Invoke( this ); } finally { _surface3PaintFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface3PaintFactor"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface3PaintFactorChanged;
		ReferenceField<double> _surface3PaintFactor = 0.15;

		[DisplayName( "Surface 3 Distribution" )]
		[Category( "Surface 3" )]
		[DefaultValue( 1.0 )]
		public Reference<double> Surface3Distribution
		{
			get { if( _surface3Distribution.BeginGet() ) Surface3Distribution = _surface3Distribution.Get( this ); return _surface3Distribution.value; }
			set { if( _surface3Distribution.BeginSet( ref value ) ) { try { Surface3DistributionChanged?.Invoke( this ); } finally { _surface3Distribution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface3Distribution"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface3DistributionChanged;
		ReferenceField<double> _surface3Distribution = 1.0;

		/// <summary>
		/// Whether to enable the collision for the third surface.
		/// </summary>
		[DisplayName( "Surface 3 Collision" )]
		[Category( "Surface 3" )]
		[DefaultValue( false )]
		public Reference<bool> Surface3Collision
		{
			get { if( _surface3Collision.BeginGet() ) Surface3Collision = _surface3Collision.Get( this ); return _surface3Collision.value; }
			set { if( _surface3Collision.BeginSet( ref value ) ) { try { Surface3CollisionChanged?.Invoke( this ); } finally { _surface3Collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface3Collision"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface3CollisionChanged;
		ReferenceField<bool> _surface3Collision = false;


		const string surface4Default = @"Content\Vegetation\Models\Flowering plant\Doronicum grandiflorum\Doronicum grandiflorum.surface";
		/// <summary>
		/// The fourth surface.
		/// </summary>
		[DefaultValueReference( surface4Default )]
		[DisplayName( "Surface 4" )]
		[Category( "Surface 4" )]
		public Reference<Surface> Surface4
		{
			get { if( _surface4.BeginGet() ) Surface4 = _surface4.Get( this ); return _surface4.value; }
			set { if( _surface4.BeginSet( ref value ) ) { try { Surface4Changed?.Invoke( this ); } finally { _surface4.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface4"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface4Changed;
		ReferenceField<Surface> _surface4 = new Reference<Surface>( null, surface4Default );

		/// <summary>
		/// The color of the fourth surface.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[DisplayName( "Surface 4 Color" )]
		[Category( "Surface 4" )]
		public Reference<ColorValue> Surface4Color
		{
			get { if( _surface4Color.BeginGet() ) Surface4Color = _surface4Color.Get( this ); return _surface4Color.value; }
			set { if( _surface4Color.BeginSet( ref value ) ) { try { Surface4ColorChanged?.Invoke( this ); } finally { _surface4Color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface4Color"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface4ColorChanged;
		ReferenceField<ColorValue> _surface4Color = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// The strength of painting for the fourth surface.
		/// </summary>
		[DefaultValue( 0.15 )]
		[Range( 0, 1 )]
		[DisplayName( "Surface 4 Paint Factor" )]
		[Category( "Surface 4" )]
		public Reference<double> Surface4PaintFactor
		{
			get { if( _surface4PaintFactor.BeginGet() ) Surface4PaintFactor = _surface4PaintFactor.Get( this ); return _surface4PaintFactor.value; }
			set { if( _surface4PaintFactor.BeginSet( ref value ) ) { try { Surface4PaintFactorChanged?.Invoke( this ); } finally { _surface4PaintFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface4PaintFactor"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface4PaintFactorChanged;
		ReferenceField<double> _surface4PaintFactor = 0.15;

		[DisplayName( "Surface 4 Distribution" )]
		[Category( "Surface 4" )]
		[DefaultValue( 1.1 )]
		public Reference<double> Surface4Distribution
		{
			get { if( _surface4Distribution.BeginGet() ) Surface4Distribution = _surface4Distribution.Get( this ); return _surface4Distribution.value; }
			set { if( _surface4Distribution.BeginSet( ref value ) ) { try { Surface4DistributionChanged?.Invoke( this ); } finally { _surface4Distribution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface4Distribution"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface4DistributionChanged;
		ReferenceField<double> _surface4Distribution = 1.1;

		/// <summary>
		/// Whether to enable the collision for the fourth surface.
		/// </summary>
		[DisplayName( "Surface 4 Collision" )]
		[Category( "Surface 4" )]
		[DefaultValue( false )]
		public Reference<bool> Surface4Collision
		{
			get { if( _surface4Collision.BeginGet() ) Surface4Collision = _surface4Collision.Get( this ); return _surface4Collision.value; }
			set { if( _surface4Collision.BeginSet( ref value ) ) { try { Surface4CollisionChanged?.Invoke( this ); } finally { _surface4Collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface4Collision"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface4CollisionChanged;
		ReferenceField<bool> _surface4Collision = false;


		const string surface5Default = "";//@"Content\Vegetation\Models\Flowering plant\Tanacetum coccineum\Tanacetum coccineum.surface";
		/// <summary>
		/// The fifth surface.
		/// </summary>
		[DefaultValueReference( surface5Default )]
		[DisplayName( "Surface 5" )]
		[Category( "Surface 5" )]
		public Reference<Surface> Surface5
		{
			get { if( _surface5.BeginGet() ) Surface5 = _surface5.Get( this ); return _surface5.value; }
			set { if( _surface5.BeginSet( ref value ) ) { try { Surface5Changed?.Invoke( this ); } finally { _surface5.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface5"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface5Changed;
		ReferenceField<Surface> _surface5 = new Reference<Surface>( null, surface5Default );

		/// <summary>
		/// The color of the fifth surface.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[DisplayName( "Surface 5 Color" )]
		[Category( "Surface 5" )]
		public Reference<ColorValue> Surface5Color
		{
			get { if( _surface5Color.BeginGet() ) Surface5Color = _surface5Color.Get( this ); return _surface5Color.value; }
			set { if( _surface5Color.BeginSet( ref value ) ) { try { Surface5ColorChanged?.Invoke( this ); } finally { _surface5Color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface5Color"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface5ColorChanged;
		ReferenceField<ColorValue> _surface5Color = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// The strength of painting for the fifth surface.
		/// </summary>
		[DefaultValue( 0.15 )]
		[Range( 0, 1 )]
		[DisplayName( "Surface 5 Paint Factor" )]
		[Category( "Surface 5" )]
		public Reference<double> Surface5PaintFactor
		{
			get { if( _surface5PaintFactor.BeginGet() ) Surface5PaintFactor = _surface5PaintFactor.Get( this ); return _surface5PaintFactor.value; }
			set { if( _surface5PaintFactor.BeginSet( ref value ) ) { try { Surface5PaintFactorChanged?.Invoke( this ); } finally { _surface5PaintFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface5PaintFactor"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface5PaintFactorChanged;
		ReferenceField<double> _surface5PaintFactor = 0.15;

		[DisplayName( "Surface 5 Distribution" )]
		[Category( "Surface 5" )]
		[DefaultValue( 0.9 )]
		public Reference<double> Surface5Distribution
		{
			get { if( _surface5Distribution.BeginGet() ) Surface5Distribution = _surface5Distribution.Get( this ); return _surface5Distribution.value; }
			set { if( _surface5Distribution.BeginSet( ref value ) ) { try { Surface5DistributionChanged?.Invoke( this ); } finally { _surface5Distribution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface5Distribution"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface5DistributionChanged;
		ReferenceField<double> _surface5Distribution = 0.9;

		/// <summary>
		/// Whether to enable the collision for the fifth surface.
		/// </summary>
		[DisplayName( "Surface 5 Collision" )]
		[Category( "Surface 5" )]
		[DefaultValue( false )]
		public Reference<bool> Surface5Collision
		{
			get { if( _surface5Collision.BeginGet() ) Surface5Collision = _surface5Collision.Get( this ); return _surface5Collision.value; }
			set { if( _surface5Collision.BeginSet( ref value ) ) { try { Surface5CollisionChanged?.Invoke( this ); } finally { _surface5Collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface5Collision"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface5CollisionChanged;
		ReferenceField<bool> _surface5Collision = false;


		const string surface6Default = "";
		/// <summary>
		/// The sixth surface.
		/// </summary>
		[DefaultValueReference( surface6Default )]
		[DisplayName( "Surface 6" )]
		[Category( "Surface 6" )]
		public Reference<Surface> Surface6
		{
			get { if( _surface6.BeginGet() ) Surface6 = _surface6.Get( this ); return _surface6.value; }
			set { if( _surface6.BeginSet( ref value ) ) { try { Surface6Changed?.Invoke( this ); } finally { _surface6.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface6"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface6Changed;
		ReferenceField<Surface> _surface6 = new Reference<Surface>( null, surface6Default );

		/// <summary>
		/// The color of the sixth surface.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[DisplayName( "Surface 6 Color" )]
		[Category( "Surface 6" )]
		public Reference<ColorValue> Surface6Color
		{
			get { if( _surface6Color.BeginGet() ) Surface6Color = _surface6Color.Get( this ); return _surface6Color.value; }
			set { if( _surface6Color.BeginSet( ref value ) ) { try { Surface6ColorChanged?.Invoke( this ); } finally { _surface6Color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface6Color"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface6ColorChanged;
		ReferenceField<ColorValue> _surface6Color = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// The strength of painting for the sixth surface.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Range( 0, 1 )]
		[DisplayName( "Surface 6 Paint Factor" )]
		[Category( "Surface 6" )]
		public Reference<double> Surface6PaintFactor
		{
			get { if( _surface6PaintFactor.BeginGet() ) Surface6PaintFactor = _surface6PaintFactor.Get( this ); return _surface6PaintFactor.value; }
			set { if( _surface6PaintFactor.BeginSet( ref value ) ) { try { Surface6PaintFactorChanged?.Invoke( this ); } finally { _surface6PaintFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface6PaintFactor"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface6PaintFactorChanged;
		ReferenceField<double> _surface6PaintFactor = 0.1;

		[DisplayName( "Surface 6 Distribution" )]
		[Category( "Surface 6" )]
		[DefaultValue( 1.0 )]
		public Reference<double> Surface6Distribution
		{
			get { if( _surface6Distribution.BeginGet() ) Surface6Distribution = _surface6Distribution.Get( this ); return _surface6Distribution.value; }
			set { if( _surface6Distribution.BeginSet( ref value ) ) { try { Surface6DistributionChanged?.Invoke( this ); } finally { _surface6Distribution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface6Distribution"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface6DistributionChanged;
		ReferenceField<double> _surface6Distribution = 1.0;

		/// <summary>
		/// Whether to enable the collision for the sixth surface.
		/// </summary>
		[DisplayName( "Surface 6 Collision" )]
		[Category( "Surface 6" )]
		[DefaultValue( false )]
		public Reference<bool> Surface6Collision
		{
			get { if( _surface6Collision.BeginGet() ) Surface6Collision = _surface6Collision.Get( this ); return _surface6Collision.value; }
			set { if( _surface6Collision.BeginSet( ref value ) ) { try { Surface6CollisionChanged?.Invoke( this ); } finally { _surface6Collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface6Collision"/> property value changes.</summary>
		public event Action<WorldGenerator> Surface6CollisionChanged;
		ReferenceField<bool> _surface6Collision = false;


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

		///////////////////////////////////////////////

		public class TemplateData
		{
			public Reference<Material> BaseMaterial;
			public ColorValue BaseMaterialColor = ColorValue.One;
			public Reference<Surface> BaseSurface;
			public double BaseSurfacePaintFactor = 1;
			public ColorValue BaseSurfaceColor = ColorValue.One;
			public bool BaseSurfaceCollision;

			public List<SurfaceItem> Surfaces = new List<SurfaceItem>();

			//

			public class SurfaceItem
			{
				public Reference<Surface> Surface;
				public double PaintFactor;
				public double Distribution;
				public ColorValue Color;
				public bool Collision;

				public SurfaceItem()
				{
				}

				public SurfaceItem( Reference<Surface> surface, double paintFactor, double distribution, ColorValue color, bool collision )
				{
					Surface = surface;
					PaintFactor = paintFactor;
					Distribution = distribution;
					Color = color;
					Collision = collision;
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
				case nameof( BaseMaterial ):
				case nameof( BaseMaterialColor ):
				case nameof( BaseSurface ):
				case nameof( BaseSurfacePaintFactor ):
				case nameof( BaseSurfaceColor ):
				case nameof( BaseSurfaceCollision ):
				case nameof( Surface1 ):
				case nameof( Surface2 ):
				case nameof( Surface3 ):
				case nameof( Surface4 ):
				case nameof( Surface5 ):
				case nameof( Surface6 ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					break;

				case nameof( Surface1PaintFactor ):
				case nameof( Surface1Distribution ):
				case nameof( Surface1Color ):
				case nameof( Surface1Collision ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface1.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Surface2PaintFactor ):
				case nameof( Surface2Distribution ):
				case nameof( Surface2Color ):
				case nameof( Surface2Collision ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface2.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Surface3PaintFactor ):
				case nameof( Surface3Distribution ):
				case nameof( Surface3Color ):
				case nameof( Surface3Collision ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface3.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Surface4PaintFactor ):
				case nameof( Surface4Distribution ):
				case nameof( Surface4Color ):
				case nameof( Surface4Collision ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface4.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Surface5PaintFactor ):
				case nameof( Surface5Distribution ):
				case nameof( Surface5Color ):
				case nameof( Surface5Collision ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface5.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Surface6PaintFactor ):
				case nameof( Surface6Distribution ):
				case nameof( Surface6Color ):
				case nameof( Surface6Collision ):
					if( Template.Value != TemplateEnum.Custom )
						skip = true;
					if( !Surface6.ReferenceSpecified )
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
					data.BaseMaterial = new Reference<Material>( null, baseMaterialDefault );
					data.BaseMaterialColor = ColorValue.One;
					data.BaseSurface = new Reference<Surface>( null, baseSurfaceDefault );
					data.BaseSurfacePaintFactor = 1;
					data.BaseSurfaceColor = new ColorValue( 0.57, 0.75, 0.49 );
					data.BaseSurfaceCollision = false;

					var surface1 = new Reference<Surface>( null, surface1Default );
					data.Surfaces.Add( new TemplateData.SurfaceItem( surface1, 0.5, 1, ColorValue.One, true ) );

					var surface2 = new Reference<Surface>( null, surface2Default );
					data.Surfaces.Add( new TemplateData.SurfaceItem( surface2, 0.02, 1, ColorValue.One, true ) );

					var surface3 = new Reference<Surface>( null, surface3Default );
					data.Surfaces.Add( new TemplateData.SurfaceItem( surface3, 0.15, 1.0, ColorValue.One, false ) );

					var surface4 = new Reference<Surface>( null, surface4Default );
					data.Surfaces.Add( new TemplateData.SurfaceItem( surface4, 0.15, 1.1, ColorValue.One, false ) );

					var surface5 = new Reference<Surface>( null, surface5Default );
					data.Surfaces.Add( new TemplateData.SurfaceItem( surface5, 0.15, 0.9, ColorValue.One, false ) );
				}
				break;

			default:

				data.BaseMaterial = BaseMaterial;
				data.BaseMaterialColor = BaseMaterialColor;
				data.BaseSurface = BaseSurface;
				data.BaseSurfacePaintFactor = BaseSurfacePaintFactor;
				data.BaseSurfaceColor = BaseSurfaceColor;
				data.BaseSurfaceCollision = BaseSurfaceCollision;

				for( int nSurface = 0; nSurface < 6; nSurface++ )
				{
					Reference<Surface> surface = null;
					var paintFactor = 0.1;//var paintSteps = 1;
					var distribution = 1.0;
					var color = ColorValue.One;
					var collision = false;
					switch( nSurface )
					{
					case 0: surface = Surface1; paintFactor = Surface1PaintFactor; distribution = Surface1Distribution; color = Surface1Color; collision = Surface1Collision; break;
					case 1: surface = Surface2; paintFactor = Surface2PaintFactor; distribution = Surface2Distribution; color = Surface2Color; collision = Surface2Collision; break;
					case 2: surface = Surface3; paintFactor = Surface3PaintFactor; distribution = Surface3Distribution; color = Surface3Color; collision = Surface3Collision; break;
					case 3: surface = Surface4; paintFactor = Surface4PaintFactor; distribution = Surface4Distribution; color = Surface4Color; collision = Surface4Collision; break;
					case 4: surface = Surface5; paintFactor = Surface5PaintFactor; distribution = Surface5Distribution; color = Surface5Color; collision = Surface5Collision; break;
					case 5: surface = Surface6; paintFactor = Surface6PaintFactor; distribution = Surface6Distribution; color = Surface6Color; collision = Surface6Collision; break;
					}

					if( surface.ReferenceSpecified )
						data.Surfaces.Add( new TemplateData.SurfaceItem( surface, paintFactor, distribution, color, collision ) );
				}
				break;
			}

			return data;
		}

		public delegate void GenerateOverrideDelegate( WorldGenerator sender, Editor.DocumentInstance document, ref bool handled );
		public event GenerateOverrideDelegate GenerateOverride;

		public virtual void Generate( Editor.DocumentInstance document )
		{
			var handled = false;
			GenerateOverride?.Invoke( this, document, ref handled );
			if( handled )
				return;

			var scene = FindParent<Scene>();
			if( scene == null )
				return;

			TemplateData templateData = null;
			GetTemplateData?.Invoke( this, ref templateData );
			if( templateData == null )
				templateData = GetTemplateDataDefault();

			var randomizer = new FastRandom( Seed );

			var terrain = scene.GetComponent<Terrain>( true );
			var maskSize = 1;
			if( terrain != null )
				maskSize = terrain.GetPaintMaskSizeInteger();
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
			terrain.Material = templateData.BaseMaterial;
			terrain.MaterialColor = templateData.BaseMaterialColor;
			if( templateData.BaseSurfacePaintFactor > 0 )
			{
				terrain.Surface = templateData.BaseSurface;
				terrain.SurfaceObjectsColor = templateData.BaseSurfaceColor;
				terrain.SurfaceObjectsDistribution = 1.0 / templateData.BaseSurfacePaintFactor;
				terrain.SurfaceObjectsCollision = templateData.BaseSurfaceCollision;
			}

			var heightRange = HeightRange.Value;
			var heightmapSize = terrain.GetHeightmapSizeInteger();

			//update terrain heights
			{
				var globalScale = Curvature.Value;
				//var globalScale = 3.0;

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

			//delete old layers
			if( terrain != null )
			{
				foreach( var c in terrain.GetComponents<PaintLayer>() )
					c.RemoveFromParent( false );
			}

			////add material layers
			//{
			//	var layer = terrain.CreateComponent<PaintLayer>();
			//	//layer.Material = ;

			//	//layer.fill

			//	var name = "";
			//	{
			//		var surfaceV = layer.Surface.Value;
			//		if( surfaceV != null )
			//		{
			//			if( surfaceV.Parent == null )
			//			{
			//				var fileName = ComponentUtility.GetOwnedFileNameOfComponent( surfaceV );
			//				if( !string.IsNullOrEmpty( fileName ) )
			//					name = Path.GetFileNameWithoutExtension( fileName );
			//			}
			//			else
			//				name = surfaceV.Name;
			//		}
			//		if( string.IsNullOrEmpty( name ) )
			//			name = ( nSurface + 1 ).ToString();
			//	}
			//	layer.Name = "Paint Layer " + name;


			//	layer.Mask = new byte[ terrain.GetPaintMaskSizeInteger() * terrain.GetPaintMaskSizeInteger() ];

			//	{
			//		var min = new Vector2I( -100000, -100000 );
			//		var max = new Vector2I( 100000, 100000 );
			//		MathEx.Clamp( ref min.X, 0, maskSize - 1 );
			//		MathEx.Clamp( ref min.Y, 0, maskSize - 1 );
			//		MathEx.Clamp( ref max.X, 0, maskSize - 1 );
			//		MathEx.Clamp( ref max.Y, 0, maskSize - 1 );

			//		for( int y = min.Y; y <= max.Y; y++ )
			//		{
			//			for( int x = min.X; x <= max.X; x++ )
			//			{
			//				var maskIndex = new Vector2I( x, y );
			//				//var pos = terrain.GetPositionXYByMaskIndex( maskIndex );

			//				var factor = surfaceItem.PaintFactor;

			//				//var factor = leveledAreaRectangle.Contains( pos ) ? 1.0 : 0.0;
			//				//var distance = areaRectangle.GetPointDistance( pos );
			//				//var factor = 1.0 - MathEx.Saturate( distance / borderSize );

			//				//if( factor > 0 )
			//				{
			//					layer.SetMaskValue( maskIndex, (float)factor );

			//					//var source = terrain.GetHeightWithoutPosition( maskIndex, false );
			//					//var dest = LeveledAreaHeight.Value;

			//					//var newValue = MathEx.Lerp( source, dest, factor );
			//					//terrain.SetHeightWithoutPosition( maskIndex, (float)newValue );
			//				}
			//			}
			//		}
			//	}

			//}

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

			//Rectangle leveledAreaRectangle = Rectangle.Zero;

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
						layer.SurfaceObjectsColor = surfaceItem.Color;
						layer.SurfaceObjectsCollision = surfaceItem.Collision;
						layer.BlendMode = PaintLayer.BlendModeEnum.NoBlend;
						layer.SurfaceObjectsDistribution = surfaceItem.Distribution;

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
				var leveledAreaRectangle = new Rectangle( -LeveledAreaSize.Value / 2, LeveledAreaSize.Value / 2 );
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


					layer.Mask = new byte[ terrain.GetPaintMaskSizeInteger() * terrain.GetPaintMaskSizeInteger() ];

					{
						//var borderSize = areaRectangle.Size.MaxComponent() / 10.0;
						//if( borderSize < 0.001 )
						//	borderSize = 0.001;

						var min = terrain.GetMaskIndexByPosition( leveledAreaRectangle.Minimum );// - new Vector2( borderSize, borderSize ) ) - new Vector2I( 1, 1 );
						var max = terrain.GetMaskIndexByPosition( leveledAreaRectangle.Maximum );// + new Vector2( borderSize, borderSize ) ) + new Vector2I( 2, 2 );

						min -= new Vector2I( 10, 10 );
						max += new Vector2I( 10, 10 );

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

								var factor = MathEx.Saturate( 1.0 - GetPointDistance( pos ) );
								//var factor = GetPointDistance( pos ) == 0 ? 1.0 : 0.0;

								if( factor > 0 )
									layer.SetMaskValue( maskIndex, (float)factor );
							}
						}
					}
				}

			}

			//enable components after update
			if( terrain != null )
				terrain.Enabled = true;
			if( groupOfObjects != null )
				groupOfObjects.Enabled = true;
		}

#endif

	}
}
