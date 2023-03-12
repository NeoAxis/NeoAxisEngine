// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents a type of the road.
	/// </summary>
	[ResourceFileExtension( "roadtype" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Road\Road Type", 10500 )]
	[EditorControl( typeof( RoadTypeEditor ) )]
	[Preview( typeof( RoadTypePreview ) )]
	[PreviewImage( typeof( RoadTypePreviewImage ) )]
#endif
	public class RoadType : Component
	{
		int version;

		//
		
		/// <summary>
		/// The width of lanes.
		/// </summary>
		[DefaultValue( 3.5 )]
		[Category( "Surface" )]
		public Reference<double> LaneWidth
		{
			get { if( _laneWidth.BeginGet() ) LaneWidth = _laneWidth.Get( this ); return _laneWidth.value; }
			set { if( _laneWidth.BeginSet( ref value ) ) { try { LaneWidthChanged?.Invoke( this ); DataWasChanged(); } finally { _laneWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LaneWidth"/> property value changes.</summary>
		public event Action<RoadType> LaneWidthChanged;
		ReferenceField<double> _laneWidth = 3.5;

		/// <summary>
		/// The width of roadside edges.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Category( "Surface" )]
		public Reference<double> RoadsideEdgeWidth
		{
			get { if( _roadsideEdgeWidth.BeginGet() ) RoadsideEdgeWidth = _roadsideEdgeWidth.Get( this ); return _roadsideEdgeWidth.value; }
			set { if( _roadsideEdgeWidth.BeginSet( ref value ) ) { try { RoadsideEdgeWidthChanged?.Invoke( this ); DataWasChanged(); } finally { _roadsideEdgeWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RoadsideEdgeWidth"/> property value changes.</summary>
		public event Action<RoadType> RoadsideEdgeWidthChanged;
		ReferenceField<double> _roadsideEdgeWidth = 0.5;

		const string surfaceMaterialDefault = @"Content\Constructors\Roads\Default Road\Asphalt 15 dark.material";

		/// <summary>
		/// The material of the surface.
		/// </summary>
		[DefaultValueReference( surfaceMaterialDefault )]
		[Category( "Surface" )]
		public Reference<Material> SurfaceMaterial
		{
			get { if( _surfaceMaterial.BeginGet() ) SurfaceMaterial = _surfaceMaterial.Get( this ); return _surfaceMaterial.value; }
			set { if( _surfaceMaterial.BeginSet( ref value ) ) { try { SurfaceMaterialChanged?.Invoke( this ); DataWasChanged(); } finally { _surfaceMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SurfaceMaterial"/> property value changes.</summary>
		public event Action<RoadType> SurfaceMaterialChanged;
		ReferenceField<Material> _surfaceMaterial = new Reference<Material>( null, surfaceMaterialDefault );

		/// <summary>
		/// The length of UV tiles.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0.1, 20, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[Category( "Surface" )]
		[DisplayName( "UV Tiles Length" )]
		public Reference<double> UVTilesLength
		{
			get { if( _uVTilesLength.BeginGet() ) UVTilesLength = _uVTilesLength.Get( this ); return _uVTilesLength.value; }
			set { if( _uVTilesLength.BeginSet( ref value ) ) { try { UVTilesLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _uVTilesLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVTilesLength"/> property value changes.</summary>
		public event Action<RoadType> UVTilesLengthChanged;
		ReferenceField<double> _uVTilesLength = 0.5;

		/// <summary>
		/// The surface of age imperfections.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Surface" )]
		public Reference<Surface> AgeImperfection
		{
			get { if( _ageImperfection.BeginGet() ) AgeImperfection = _ageImperfection.Get( this ); return _ageImperfection.value; }
			set { if( _ageImperfection.BeginSet( ref value ) ) { try { AgeImperfectionChanged?.Invoke( this ); } finally { _ageImperfection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AgeImperfection"/> property value changes.</summary>
		public event Action<RoadType> AgeImperfectionChanged;
		ReferenceField<Surface> _ageImperfection = null;

		//[DefaultValue( 1.0 )]
		//[Range( 1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> UVTilesCircle
		//{
		//	get { if( _uVTilesCircle.BeginGet() ) UVTilesCircle = _uVTilesCircle.Get( this ); return _uVTilesCircle.value; }
		//	set { if( _uVTilesCircle.BeginSet( ref value ) ) { try { UVTilesCircleChanged?.Invoke( this ); DataWasChanged(); } finally { _uVTilesCircle.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="UVTilesCircle"/> property value changes.</summary>
		//public event Action<Road> UVTilesCircleChanged;
		//ReferenceField<double> _uVTilesCircle = 1.0;

		//!!!!impl
		//[DefaultValue( 0.0 )]
		//[Category( "Median Strip" )]
		//public Reference<double> MedianStripWidth
		//{
		//	get { if( _medianStripWidth.BeginGet() ) MedianStripWidth = _medianStripWidth.Get( this ); return _medianStripWidth.value; }
		//	set { if( _medianStripWidth.BeginSet( ref value ) ) { try { MedianStripWidthChanged?.Invoke( this ); } finally { _medianStripWidth.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MedianStripWidth"/> property value changes.</summary>
		//public event Action<RoadType> MedianStripWidthChanged;
		//ReferenceField<double> _medianStripWidth = 0.0;

		//!!!!impl
		//[DefaultValue( null )]
		//[Category( "Median Strip" )]
		//public Reference<FenceType> MedianStripFence
		//{
		//	get { if( _medianStripFence.BeginGet() ) MedianStripFence = _medianStripFence.Get( this ); return _medianStripFence.value; }
		//	set { if( _medianStripFence.BeginSet( ref value ) ) { try { MedianStripFenceChanged?.Invoke( this ); } finally { _medianStripFence.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MedianStripFence"/> property value changes.</summary>
		//public event Action<RoadType> MedianStripFenceChanged;
		//ReferenceField<FenceType> _medianStripFence = null;

		const string markupMaterialDefault = @"Content\Constructors\Roads\Default Road\Markup.material";

		/// <summary>
		/// The material of the markup.
		/// </summary>
		[DefaultValueReference( markupMaterialDefault )]
		[Category( "Markup" )]
		public Reference<Material> MarkupMaterial
		{
			get { if( _markupMaterial.BeginGet() ) MarkupMaterial = _markupMaterial.Get( this ); return _markupMaterial.value; }
			set { if( _markupMaterial.BeginSet( ref value ) ) { try { MarkupMaterialChanged?.Invoke( this ); DataWasChanged(); } finally { _markupMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MarkupMaterial"/> property value changes.</summary>
		public event Action<RoadType> MarkupMaterialChanged;
		ReferenceField<Material> _markupMaterial = new Reference<Material>( null, markupMaterialDefault );

		const string markupDottedMaterialDefault = @"Content\Constructors\Roads\Default Road\Markup dotted.material";

		/// <summary>
		/// The material of dotted markups.
		/// </summary>
		[DefaultValueReference( markupDottedMaterialDefault )]
		[Category( "Markup" )]
		public Reference<Material> MarkupDottedMaterial
		{
			get { if( _markupDottedMaterial.BeginGet() ) MarkupDottedMaterial = _markupDottedMaterial.Get( this ); return _markupDottedMaterial.value; }
			set { if( _markupDottedMaterial.BeginSet( ref value ) ) { try { MarkupDottedMaterialChanged?.Invoke( this ); DataWasChanged(); } finally { _markupDottedMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MarkupDottedMaterial"/> property value changes.</summary>
		public event Action<RoadType> MarkupDottedMaterialChanged;
		ReferenceField<Material> _markupDottedMaterial = new Reference<Material>( null, markupDottedMaterialDefault );

		/// <summary>
		/// The width of markup dividing lanes.
		/// </summary>
		[DefaultValue( 0.12 )]
		[Range( 0.05, 0.5, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Markup" )]
		public Reference<double> MarkupDividingLaneWidth
		{
			get { if( _markupDividingLaneWidth.BeginGet() ) MarkupDividingLaneWidth = _markupDividingLaneWidth.Get( this ); return _markupDividingLaneWidth.value; }
			set { if( _markupDividingLaneWidth.BeginSet( ref value ) ) { try { MarkupDividingLaneWidthChanged?.Invoke( this ); DataWasChanged(); } finally { _markupDividingLaneWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MarkupDividingLaneWidth"/> property value changes.</summary>
		public event Action<RoadType> MarkupDividingLaneWidthChanged;
		ReferenceField<double> _markupDividingLaneWidth = 0.12;

		/// <summary>
		/// The width of roadside markups.
		/// </summary>
		[DefaultValue( 0.15 )]
		[Range( 0.05, 0.5, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Markup" )]
		public Reference<double> MarkupRoadsideWidth
		{
			get { if( _markupRoadsideWidth.BeginGet() ) MarkupRoadsideWidth = _markupRoadsideWidth.Get( this ); return _markupRoadsideWidth.value; }
			set { if( _markupRoadsideWidth.BeginSet( ref value ) ) { try { MarkupRoadsideWidthChanged?.Invoke( this ); DataWasChanged(); } finally { _markupRoadsideWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MarkupRoadsideWidth"/> property value changes.</summary>
		public event Action<RoadType> MarkupRoadsideWidthChanged;
		ReferenceField<double> _markupRoadsideWidth = 0.15;

		//////////////////////

		/// <summary>
		/// The width of shoulders.
		/// </summary>
		[DefaultValue( 2.0 )]
		[Category( "Shoulder" )]
		public Reference<double> ShoulderWidth
		{
			get { if( _shoulderWidth.BeginGet() ) ShoulderWidth = _shoulderWidth.Get( this ); return _shoulderWidth.value; }
			set { if( _shoulderWidth.BeginSet( ref value ) ) { try { ShoulderWidthChanged?.Invoke( this ); DataWasChanged(); } finally { _shoulderWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShoulderWidth"/> property value changes.</summary>
		public event Action<RoadType> ShoulderWidthChanged;
		ReferenceField<double> _shoulderWidth = 2.0;

		const string shoulderMaterialDefault = @"Content\Materials\Basic Library\Ground\Rock Ground.material";

		/// <summary>
		/// The material of shoulders.
		/// </summary>
		[DefaultValueReference( shoulderMaterialDefault )]
		[Category( "Shoulder" )]
		public Reference<Material> ShoulderMaterial
		{
			get { if( _shoulderMaterial.BeginGet() ) ShoulderMaterial = _shoulderMaterial.Get( this ); return _shoulderMaterial.value; }
			set { if( _shoulderMaterial.BeginSet( ref value ) ) { try { ShoulderMaterialChanged?.Invoke( this ); DataWasChanged(); } finally { _shoulderMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShoulderMaterial"/> property value changes.</summary>
		public event Action<RoadType> ShoulderMaterialChanged;
		ReferenceField<Material> _shoulderMaterial = new Reference<Material>( null, shoulderMaterialDefault );

		/// <summary>
		/// The UV tiles length of shoulders.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 20, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[Category( "Shoulder" )]
		[DisplayName( "Shoulder UV Tiles Length" )]
		public Reference<double> ShoulderUVTilesLength
		{
			get { if( _shoulderUVTilesLength.BeginGet() ) ShoulderUVTilesLength = _shoulderUVTilesLength.Get( this ); return _shoulderUVTilesLength.value; }
			set { if( _shoulderUVTilesLength.BeginSet( ref value ) ) { try { ShoulderUVTilesLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _shoulderUVTilesLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShoulderUVTilesLength"/> property value changes.</summary>
		public event Action<RoadType> ShoulderUVTilesLengthChanged;
		ReferenceField<double> _shoulderUVTilesLength = 1.0;

		//////////////////////

		/// <summary>
		/// The width of sidewalks.
		/// </summary>
		[DefaultValue( 1.5 )]
		[Category( "Sidewalk" )]
		public Reference<double> SidewalkWidth
		{
			get { if( _sidewalkWidth.BeginGet() ) SidewalkWidth = _sidewalkWidth.Get( this ); return _sidewalkWidth.value; }
			set { if( _sidewalkWidth.BeginSet( ref value ) ) { try { SidewalkWidthChanged?.Invoke( this ); DataWasChanged(); } finally { _sidewalkWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SidewalkWidth"/> property value changes.</summary>
		public event Action<RoadType> SidewalkWidthChanged;
		ReferenceField<double> _sidewalkWidth = 1.5;

		/// <summary>
		/// The height of sidewalks.
		/// </summary>
		[DefaultValue( 0.2 )]
		[Category( "Sidewalk" )]
		public Reference<double> SidewalkHeight
		{
			get { if( _sidewalkHeight.BeginGet() ) SidewalkHeight = _sidewalkHeight.Get( this ); return _sidewalkHeight.value; }
			set { if( _sidewalkHeight.BeginSet( ref value ) ) { try { SidewalkHeightChanged?.Invoke( this ); DataWasChanged(); } finally { _sidewalkHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SidewalkHeight"/> property value changes.</summary>
		public event Action<RoadType> SidewalkHeightChanged;
		ReferenceField<double> _sidewalkHeight = 0.2;

		const string sidewalkMaterialDefault = @"Content\Materials\Basic Library\Floor\Paving Stones 081.material";

		/// <summary>
		/// The material of sidewalks.
		/// </summary>
		[DefaultValueReference( sidewalkMaterialDefault )]
		[Category( "Sidewalk" )]
		public Reference<Material> SidewalkMaterial
		{
			get { if( _sidewalkMaterial.BeginGet() ) SidewalkMaterial = _sidewalkMaterial.Get( this ); return _sidewalkMaterial.value; }
			set { if( _sidewalkMaterial.BeginSet( ref value ) ) { try { SidewalkMaterialChanged?.Invoke( this ); DataWasChanged(); } finally { _sidewalkMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SidewalkMaterial"/> property value changes.</summary>
		public event Action<RoadType> SidewalkMaterialChanged;
		ReferenceField<Material> _sidewalkMaterial = new Reference<Material>( null, sidewalkMaterialDefault );

		/// <summary>
		/// The UV tiles length of sidewalks.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 20, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[Category( "Sidewalk" )]
		[DisplayName( "Sidewalk UV Tiles Length" )]
		public Reference<double> SidewalkUVTilesLength
		{
			get { if( _sidewalkUVTilesLength.BeginGet() ) SidewalkUVTilesLength = _sidewalkUVTilesLength.Get( this ); return _sidewalkUVTilesLength.value; }
			set { if( _sidewalkUVTilesLength.BeginSet( ref value ) ) { try { SidewalkUVTilesLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _sidewalkUVTilesLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SidewalkUVTilesLength"/> property value changes.</summary>
		public event Action<RoadType> SidewalkUVTilesLengthChanged;
		ReferenceField<double> _sidewalkUVTilesLength = 1.0;

		//////////////////////

		/// <summary>
		/// The width of sidewalk borders.
		/// </summary>
		[DefaultValue( 0.05 )]
		[Category( "Sidewalk Border" )]
		public Reference<double> SidewalkBorderWidth
		{
			get { if( _sidewalkBorderWidth.BeginGet() ) SidewalkBorderWidth = _sidewalkBorderWidth.Get( this ); return _sidewalkBorderWidth.value; }
			set { if( _sidewalkBorderWidth.BeginSet( ref value ) ) { try { SidewalkBorderWidthChanged?.Invoke( this ); DataWasChanged(); } finally { _sidewalkBorderWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SidewalkBorderWidth"/> property value changes.</summary>
		public event Action<RoadType> SidewalkBorderWidthChanged;
		ReferenceField<double> _sidewalkBorderWidth = 0.05;

		const string sidewalkBorderMaterialDefault = @"Content\Materials\Basic Library\Asphalt\Asphalt 010.material";

		/// <summary>
		/// The material of sidewalk borders.
		/// </summary>
		[DefaultValueReference( sidewalkBorderMaterialDefault )]
		[Category( "Sidewalk Border" )]
		public Reference<Material> SidewalkBorderMaterial
		{
			get { if( _sidewalkBorderMaterial.BeginGet() ) SidewalkBorderMaterial = _sidewalkBorderMaterial.Get( this ); return _sidewalkBorderMaterial.value; }
			set { if( _sidewalkBorderMaterial.BeginSet( ref value ) ) { try { SidewalkBorderMaterialChanged?.Invoke( this ); DataWasChanged(); } finally { _sidewalkBorderMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SidewalkBorderMaterial"/> property value changes.</summary>
		public event Action<RoadType> SidewalkBorderMaterialChanged;
		ReferenceField<Material> _sidewalkBorderMaterial = new Reference<Material>( null, sidewalkBorderMaterialDefault );

		/// <summary>
		/// The UV tiles length of sidewalk borders.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 20, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[Category( "Sidewalk Border" )]
		[DisplayName( "Sidewalk Border UV Tiles Length" )]
		public Reference<double> SidewalkBorderUVTilesLength
		{
			get { if( _sidewalkBorderUVTilesLength.BeginGet() ) SidewalkBorderUVTilesLength = _sidewalkBorderUVTilesLength.Get( this ); return _sidewalkBorderUVTilesLength.value; }
			set { if( _sidewalkBorderUVTilesLength.BeginSet( ref value ) ) { try { SidewalkBorderUVTilesLengthChanged?.Invoke( this ); } finally { _sidewalkBorderUVTilesLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SidewalkBorderUVTilesLength"/> property value changes.</summary>
		public event Action<RoadType> SidewalkBorderUVTilesLengthChanged;
		ReferenceField<double> _sidewalkBorderUVTilesLength = 1.0;

		//////////////////////

		//[DefaultValue( null )]
		//[Category( "Sidewalk" )]
		//public Reference<FenceType> SidewalkFence
		//{
		//	get { if( _sidewalkFence.BeginGet() ) SidewalkFence = _sidewalkFence.Get( this ); return _sidewalkFence.value; }
		//	set { if( _sidewalkFence.BeginSet( ref value ) ) { try { SidewalkFenceChanged?.Invoke( this ); } finally { _sidewalkFence.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SidewalkFence"/> property value changes.</summary>
		//public event Action<RoadType> SidewalkFenceChanged;
		//ReferenceField<FenceType> _sidewalkFence = null;

		//////////////////////

		//!!!!

		//[DefaultValue( 0.5 )]
		//[Category( "Overpass" )]
		//public Reference<double> OverpassWidth
		//{
		//	get { if( _overpassWidth.BeginGet() ) OverpassWidth = _overpassWidth.Get( this ); return _overpassWidth.value; }
		//	set { if( _overpassWidth.BeginSet( ref value ) ) { try { OverpassWidthChanged?.Invoke( this ); } finally { _overpassWidth.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassWidth"/> property value changes.</summary>
		//public event Action<RoadType> OverpassWidthChanged;
		//ReferenceField<double> _overpassWidth = 0.5;

		//[DefaultValue( 1.0 )]
		//[Category( "Overpass" )]
		//public Reference<double> OverpassHeight
		//{
		//	get { if( _overpassHeight.BeginGet() ) OverpassHeight = _overpassHeight.Get( this ); return _overpassHeight.value; }
		//	set { if( _overpassHeight.BeginSet( ref value ) ) { try { OverpassHeightChanged?.Invoke( this ); } finally { _overpassHeight.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassHeight"/> property value changes.</summary>
		//public event Action<RoadType> OverpassHeightChanged;
		//ReferenceField<double> _overpassHeight = 1.0;

		//[DefaultValue( null )]
		//[Category( "Overpass" )]
		//public Reference<Material> OverpassMaterial
		//{
		//	get { if( _overpassMaterial.BeginGet() ) OverpassMaterial = _overpassMaterial.Get( this ); return _overpassMaterial.value; }
		//	set { if( _overpassMaterial.BeginSet( ref value ) ) { try { OverpassMaterialChanged?.Invoke( this ); } finally { _overpassMaterial.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassMaterial"/> property value changes.</summary>
		//public event Action<RoadType> OverpassMaterialChanged;
		//ReferenceField<Material> _overpassMaterial = null;

		//[DefaultValue( 1.0 )]
		//[Range( 0.1, 20, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//[Category( "Overpass" )]
		//[DisplayName( "Overpass UV Tiles Length" )]
		//public Reference<double> OverpassUVTilesLength
		//{
		//	get { if( _overpassUVTilesLength.BeginGet() ) OverpassUVTilesLength = _overpassUVTilesLength.Get( this ); return _overpassUVTilesLength.value; }
		//	set { if( _overpassUVTilesLength.BeginSet( ref value ) ) { try { OverpassUVTilesLengthChanged?.Invoke( this ); } finally { _overpassUVTilesLength.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassUVTilesLength"/> property value changes.</summary>
		//public event Action<RoadType> OverpassUVTilesLengthChanged;
		//ReferenceField<double> _overpassUVTilesLength = 1.0;

		////[DefaultValue( null )]
		////[Category( "Overpass" )]
		////public Reference<FenceType> OverpassFence
		////{
		////	get { if( _overpassFence.BeginGet() ) OverpassFence = _overpassFence.Get( this ); return _overpassFence.value; }
		////	set { if( _overpassFence.BeginSet( ref value ) ) { try { OverpassFenceChanged?.Invoke( this ); } finally { _overpassFence.EndSet(); } } }
		////}
		/////// <summary>Occurs when the <see cref="OverpassFence"/> property value changes.</summary>
		////public event Action<RoadType> OverpassFenceChanged;
		////ReferenceField<FenceType> _overpassFence = null;

		//[DefaultValue( null )]
		//[Category( "Overpass Support" )]
		//public Reference<Mesh> OverpassSupportTopMesh
		//{
		//	get { if( _overpassSupportTopMesh.BeginGet() ) OverpassSupportTopMesh = _overpassSupportTopMesh.Get( this ); return _overpassSupportTopMesh.value; }
		//	set { if( _overpassSupportTopMesh.BeginSet( ref value ) ) { try { OverpassSupportTopMeshChanged?.Invoke( this ); } finally { _overpassSupportTopMesh.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassSupportTopMesh"/> property value changes.</summary>
		//public event Action<RoadType> OverpassSupportTopMeshChanged;
		//ReferenceField<Mesh> _overpassSupportTopMesh = null;

		//[DefaultValue( null )]
		//[Category( "Overpass Support" )]
		//public Reference<Mesh> OverpassSupportBottomMesh
		//{
		//	get { if( _overpassSupportBottomMesh.BeginGet() ) OverpassSupportBottomMesh = _overpassSupportBottomMesh.Get( this ); return _overpassSupportBottomMesh.value; }
		//	set { if( _overpassSupportBottomMesh.BeginSet( ref value ) ) { try { OverpassSupportBottomMeshChanged?.Invoke( this ); } finally { _overpassSupportBottomMesh.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassSupportBottomMesh"/> property value changes.</summary>
		//public event Action<RoadType> OverpassSupportBottomMeshChanged;
		//ReferenceField<Mesh> _overpassSupportBottomMesh = null;

		//[DefaultValue( 1.0 )]
		//[Category( "Overpass Support" )]
		//public Reference<double> OverpassSupportColumnRadius
		//{
		//	get { if( _overpassSupportColumnRadius.BeginGet() ) OverpassSupportColumnRadius = _overpassSupportColumnRadius.Get( this ); return _overpassSupportColumnRadius.value; }
		//	set { if( _overpassSupportColumnRadius.BeginSet( ref value ) ) { try { OverpassSupportColumnRadiusChanged?.Invoke( this ); } finally { _overpassSupportColumnRadius.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassSupportColumnRadius"/> property value changes.</summary>
		//public event Action<RoadType> OverpassSupportColumnRadiusChanged;
		//ReferenceField<double> _overpassSupportColumnRadius = 1.0;

		//[DefaultValue( null )]
		//[Category( "Overpass Support" )]
		//public Reference<Material> OverpassSupportColumnMaterial
		//{
		//	get { if( _overpassSupportColumnMaterial.BeginGet() ) OverpassSupportColumnMaterial = _overpassSupportColumnMaterial.Get( this ); return _overpassSupportColumnMaterial.value; }
		//	set { if( _overpassSupportColumnMaterial.BeginSet( ref value ) ) { try { OverpassSupportColumnMaterialChanged?.Invoke( this ); } finally { _overpassSupportColumnMaterial.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassSupportColumnMaterial"/> property value changes.</summary>
		//public event Action<RoadType> OverpassSupportColumnMaterialChanged;
		//ReferenceField<Material> _overpassSupportColumnMaterial = null;

		//[DefaultValue( 1.0 )]
		//[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//[Category( "Overpass Support" )]
		//[DisplayName( "Overpass Support Column UV Tiles Length" )]
		//public Reference<double> OverpassSupportColumnUVTilesLength
		//{
		//	get { if( _overpassSupportColumnUVTilesLength.BeginGet() ) OverpassSupportColumnUVTilesLength = _overpassSupportColumnUVTilesLength.Get( this ); return _overpassSupportColumnUVTilesLength.value; }
		//	set { if( _overpassSupportColumnUVTilesLength.BeginSet( ref value ) ) { try { OverpassSupportColumnUVTilesLengthChanged?.Invoke( this ); } finally { _overpassSupportColumnUVTilesLength.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassSupportColumnUVTilesLength"/> property value changes.</summary>
		//public event Action<RoadType> OverpassSupportColumnUVTilesLengthChanged;
		//ReferenceField<double> _overpassSupportColumnUVTilesLength = 1.0;

		//[DefaultValue( 1.0 )]
		//[Range( 1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//[DisplayName( "Overpass Support Column UV Tiles Circle" )]
		//[Category( "Overpass Support" )]
		//public Reference<double> OverpassSupportColumnUVTilesCircle
		//{
		//	get { if( _overpassSupportColumnUVTilesCircle.BeginGet() ) OverpassSupportColumnUVTilesCircle = _overpassSupportColumnUVTilesCircle.Get( this ); return _overpassSupportColumnUVTilesCircle.value; }
		//	set { if( _overpassSupportColumnUVTilesCircle.BeginSet( ref value ) ) { try { OverpassSupportColumnUVTilesCircleChanged?.Invoke( this ); } finally { _overpassSupportColumnUVTilesCircle.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassSupportColumnUVTilesCircle"/> property value changes.</summary>
		//public event Action<RoadType> OverpassSupportColumnUVTilesCircleChanged;
		//ReferenceField<double> _overpassSupportColumnUVTilesCircle = 1.0;


		/// <summary>
		/// The length of segments.
		/// </summary>
		//!!!!default value
		[DefaultValue( 0.5 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[Category( "Rendering" )]
		public Reference<double> SegmentsLength
		{
			get { if( _segmentsLength.BeginGet() ) SegmentsLength = _segmentsLength.Get( this ); return _segmentsLength.value; }
			set { if( _segmentsLength.BeginSet( ref value ) ) { try { SegmentsLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _segmentsLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SegmentsLength"/> property value changes.</summary>
		public event Action<RoadType> SegmentsLengthChanged;
		ReferenceField<double> _segmentsLength = 0.5;



		//[DefaultValue( 16 )]
		//[Range( 4, 64, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<int> SegmentsCircle
		//{
		//	get { if( _segmentsCircle.BeginGet() ) SegmentsCircle = _segmentsCircle.Get( this ); return _segmentsCircle.value; }
		//	set { if( _segmentsCircle.BeginSet( ref value ) ) { try { SegmentsCircleChanged?.Invoke( this ); DataWasChanged(); } finally { _segmentsCircle.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SegmentsCircle"/> property value changes.</summary>
		//public event Action<Road> SegmentsCircleChanged;
		//ReferenceField<int> _segmentsCircle = 16;

		////!!!!
		////[DefaultValue( false )]
		////public Reference<bool> SharpEdges
		////{
		////	get { if( _sharpEdges.BeginGet() ) SharpEdges = _sharpEdges.Get( this ); return _sharpEdges.value; }
		////	set { if( _sharpEdges.BeginSet( ref value ) ) { try { SharpEdgesChanged?.Invoke( this ); DataWasChanged(); } finally { _sharpEdges.EndSet(); } } }
		////}
		/////// <summary>Occurs when the <see cref="SharpEdges"/> property value changes.</summary>
		////public event Action<Road> SharpEdgesChanged;
		////ReferenceField<bool> _sharpEdges = false;

		/// <summary>
		/// The factor of maximum visibility distance. The maximum distance is calculated based on the size of the object on the screen.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Rendering" )]
		public Reference<double> VisibilityDistanceFactor
		{
			get { if( _visibilityDistanceFactor.BeginGet() ) VisibilityDistanceFactor = _visibilityDistanceFactor.Get( this ); return _visibilityDistanceFactor.value; }
			set { if( _visibilityDistanceFactor.BeginSet( ref value ) ) { try { VisibilityDistanceFactorChanged?.Invoke( this ); } finally { _visibilityDistanceFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<RoadType> VisibilityDistanceFactorChanged;
		ReferenceField<double> _visibilityDistanceFactor = 1.0;


		/// <summary>
		/// The physical material used by the rigid body.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Physics" )]
		public Reference<PhysicalMaterial> SurfaceCollisionMaterial
		{
			get { if( _surfaceCollisionMaterial.BeginGet() ) SurfaceCollisionMaterial = _surfaceCollisionMaterial.Get( this ); return _surfaceCollisionMaterial.value; }
			set { if( _surfaceCollisionMaterial.BeginSet( ref value ) ) { try { SurfaceCollisionMaterialChanged?.Invoke( this ); DataWasChanged(); } finally { _surfaceCollisionMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SurfaceCollisionMaterial"/> property value changes.</summary>
		public event Action<RoadType> SurfaceCollisionMaterialChanged;
		ReferenceField<PhysicalMaterial> _surfaceCollisionMaterial;

		//!!!!
		///// <summary>
		///// The type of friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( PhysicalMaterial.FrictionModeEnum.Simple )]
		//[Category( "Physics" )]
		//public Reference<PhysicalMaterial.FrictionModeEnum> SurfaceCollisionFrictionMode
		//{
		//	get { if( _surfaceCollisionFrictionMode.BeginGet() ) SurfaceCollisionFrictionMode = _surfaceCollisionFrictionMode.Get( this ); return _surfaceCollisionFrictionMode.value; }
		//	set { if( _surfaceCollisionFrictionMode.BeginSet( ref value ) ) { try { SurfaceCollisionFrictionModeChanged?.Invoke( this ); DataWasChanged(); } finally { _surfaceCollisionFrictionMode.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SurfaceCollisionFrictionMode"/> property value changes.</summary>
		//public event Action<RoadType> SurfaceCollisionFrictionModeChanged;
		//ReferenceField<PhysicalMaterial.FrictionModeEnum> _surfaceCollisionFrictionMode = PhysicalMaterial.FrictionModeEnum.Simple;

		/// <summary>
		/// The amount of friction applied on the rigid body.
		/// </summary>
		[DefaultValue( 1.0 )]//0.5 )]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> SurfaceCollisionFriction
		{
			get { if( _surfaceCollisionFriction.BeginGet() ) SurfaceCollisionFriction = _surfaceCollisionFriction.Get( this ); return _surfaceCollisionFriction.value; }
			set { if( _surfaceCollisionFriction.BeginSet( ref value ) ) { try { SurfaceCollisionFrictionChanged?.Invoke( this ); DataWasChanged(); } finally { _surfaceCollisionFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SurfaceCollisionFriction"/> property value changes.</summary>
		public event Action<RoadType> SurfaceCollisionFrictionChanged;
		ReferenceField<double> _surfaceCollisionFriction = 1.0;//0.5;

		//!!!!
		///// <summary>
		///// The amount of directional friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( "1 1 1" )]
		//[Category( "Physics" )]
		//public Reference<Vector3> SurfaceCollisionAnisotropicFriction
		//{
		//	get { if( _surfaceCollisionAnisotropicFriction.BeginGet() ) SurfaceCollisionAnisotropicFriction = _surfaceCollisionAnisotropicFriction.Get( this ); return _surfaceCollisionAnisotropicFriction.value; }
		//	set { if( _surfaceCollisionAnisotropicFriction.BeginSet( ref value ) ) { try { SurfaceCollisionAnisotropicFrictionChanged?.Invoke( this ); DataWasChanged(); } finally { _surfaceCollisionAnisotropicFriction.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SurfaceCollisionAnisotropicFriction"/> property value changes.</summary>
		//public event Action<RoadType> SurfaceCollisionAnisotropicFrictionChanged;
		//ReferenceField<Vector3> _surfaceCollisionAnisotropicFriction = Vector3.One;

		//!!!!?
		///// <summary>
		///// The amount of friction applied when rigid body is spinning.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//[Category( "Physics" )]
		//public Reference<double> SurfaceCollisionSpinningFriction
		//{
		//	get { if( _surfaceCollisionSpinningFriction.BeginGet() ) SurfaceCollisionSpinningFriction = _surfaceCollisionSpinningFriction.Get( this ); return _surfaceCollisionSpinningFriction.value; }
		//	set { if( _surfaceCollisionSpinningFriction.BeginSet( ref value ) ) { try { SurfaceCollisionSpinningFrictionChanged?.Invoke( this ); DataWasChanged(); } finally { _surfaceCollisionSpinningFriction.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SurfaceCollisionSpinningFriction"/> property value changes.</summary>
		//public event Action<RoadType> SurfaceCollisionSpinningFrictionChanged;
		//ReferenceField<double> _surfaceCollisionSpinningFriction = 0.5;

		//!!!!
		///// <summary>
		///// The amount of friction applied when rigid body is rolling.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//[Category( "Physics" )]
		//public Reference<double> SurfaceCollisionRollingFriction
		//{
		//	get { if( _surfaceCollisionRollingFriction.BeginGet() ) SurfaceCollisionRollingFriction = _surfaceCollisionRollingFriction.Get( this ); return _surfaceCollisionRollingFriction.value; }
		//	set { if( _surfaceCollisionRollingFriction.BeginSet( ref value ) ) { try { SurfaceCollisionRollingFrictionChanged?.Invoke( this ); DataWasChanged(); } finally { _surfaceCollisionRollingFriction.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SurfaceCollisionRollingFriction"/> property value changes.</summary>
		//public event Action<RoadType> SurfaceCollisionRollingFrictionChanged;
		//ReferenceField<double> _surfaceCollisionRollingFriction = 0.5;

		/// <summary>
		/// The ratio of the final relative velocity to initial relative velocity of the rigid body after collision.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> SurfaceCollisionRestitution
		{
			get { if( _surfaceCollisionRestitution.BeginGet() ) SurfaceCollisionRestitution = _surfaceCollisionRestitution.Get( this ); return _surfaceCollisionRestitution.value; }
			set { if( _surfaceCollisionRestitution.BeginSet( ref value ) ) { try { SurfaceCollisionRestitutionChanged?.Invoke( this ); DataWasChanged(); } finally { _surfaceCollisionRestitution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SurfaceCollisionRestitution"/> property value changes.</summary>
		public event Action<RoadType> SurfaceCollisionRestitutionChanged;
		ReferenceField<double> _surfaceCollisionRestitution;

		//

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//!!!!
				//case nameof( SurfaceCollisionFrictionMode ):
				//	if( SurfaceCollisionMaterial.Value != null )
				//		skip = true;
				//	break;

				case nameof( SurfaceCollisionFriction ):
					if( SurfaceCollisionMaterial.Value != null )
						skip = true;
					break;

				//!!!!
				//case nameof( SurfaceCollisionRollingFriction ):
				//case nameof( SurfaceCollisionSpinningFriction ):
				//case nameof( SurfaceCollisionAnisotropicFriction ):
				//	if( SurfaceCollisionFrictionMode.Value == PhysicalMaterial.FrictionModeEnum.Simple || SurfaceCollisionMaterial.Value != null )
				//		skip = true;
				//	break;

				case nameof( SurfaceCollisionRestitution ):
					if( SurfaceCollisionMaterial.Value != null )
						skip = true;
					break;
				}
			}
		}

		[Browsable( false )]
		public int Version
		{
			get { return version; }
		}

		public void DataWasChanged()
		{
			unchecked
			{
				version++;
			}
		}
	}
}
