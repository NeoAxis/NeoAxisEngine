// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Mesh geometry in the form of a plane.
	/// </summary>
	public class MeshGeometry_Plane : MeshGeometry_Procedural
	{
		/// <summary>
		/// The axis of the plane.
		/// </summary>
		[Serialize]
		[DefaultValue( 2 )]
		[Range( 0, 2 )]
		public Reference<int> Axis
		{
			get { if( _axis.BeginGet() ) Axis = _axis.Get( this ); return _axis.value; }
			set
			{
				if( value < 0 )
					value = new Reference<int>( 0, value.GetByReference );
				if( value > 2 )
					value = new Reference<int>( 2, value.GetByReference );
				if( _axis.BeginSet( this, ref value ) )
				{
					try
					{
						AxisChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _axis.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Axis"/> property value changes.</summary>
		public event Action<MeshGeometry_Plane> AxisChanged;
		ReferenceField<int> _axis = 2;

		/// <summary>
		/// The size of the plane. 
		/// </summary>
		[Serialize]
		[DefaultValue( "1 1" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<Vector2> Dimensions
		{
			get { if( _dimensions.BeginGet() ) Dimensions = _dimensions.Get( this ); return _dimensions.value; }
			set
			{
				var v = value.Value;
				if( v.X < 0 || v.Y < 0 )
				{
					if( v.X < 0 ) v.X = 0;
					if( v.Y < 0 ) v.Y = 0;
					value = new Reference<Vector2>( v, value.GetByReference );
				}
				if( _dimensions.BeginSet( this, ref value ) )
				{
					try
					{
						DimensionsChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _dimensions.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Dimensions"/> property value changes.</summary>
		public event Action<MeshGeometry_Plane> DimensionsChanged;
		ReferenceField<Vector2> _dimensions = Vector2.One;

		/// <summary>
		/// The number of segments used.
		/// </summary>
		[Serialize]
		[DefaultValue( "1 1" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<Vector2I> Segments
		{
			get { if( _segments.BeginGet() ) Segments = _segments.Get( this ); return _segments.value; }
			set
			{
				var v = value.Value;
				if( v.X < 1 || v.Y < 1 )
				{
					if( v.X < 1 ) v.X = 1;
					if( v.Y < 1 ) v.Y = 1;
					value = new Reference<Vector2I>( v, value.GetByReference );
				}
				if( _segments.BeginSet( this, ref value ) )
				{
					try
					{
						SegmentsChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _segments.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Segments"/> property value changes.</summary>
		public event Action<MeshGeometry_Plane> SegmentsChanged;
		ReferenceField<Vector2I> _segments = new Vector2I( 1, 1 );

		/// <summary>
		/// The number of UV tiles per unit of the world.
		/// </summary>
		[DefaultValue( "1 1" )]
		[Serialize]
		[DisplayName( "UV Tiles Per Unit" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<Vector2> UVTilesPerUnit
		{
			get { if( _uvTilesPerUnit.BeginGet() ) UVTilesPerUnit = _uvTilesPerUnit.Get( this ); return _uvTilesPerUnit.value; }
			set
			{
				if( _uvTilesPerUnit.BeginSet( this, ref value ) )
				{
					try
					{
						UVTilesPerUnitChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _uvTilesPerUnit.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="UVTilesPerUnit"/> property value changes.</summary>
		public event Action<MeshGeometry_Plane> UVTilesPerUnitChanged;
		ReferenceField<Vector2> _uvTilesPerUnit = Vector2.One;

		/// <summary>
		/// The total number of uv tiles.
		/// </summary>
		[DefaultValue( "0 0" )]
		[Serialize]
		[DisplayName( "UV Tiles In Total" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<Vector2> UVTilesInTotal
		{
			get { if( _uvTilesInTotal.BeginGet() ) UVTilesInTotal = _uvTilesInTotal.Get( this ); return _uvTilesInTotal.value; }
			set
			{
				if( _uvTilesInTotal.BeginSet( this, ref value ) )
				{
					try
					{
						UVTilesInTotalChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _uvTilesInTotal.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="UVTilesInTotal"/> property value changes.</summary>
		public event Action<MeshGeometry_Plane> UVTilesInTotalChanged;
		ReferenceField<Vector2> _uvTilesInTotal = Vector2.Zero;

		/// <summary>
		/// The offset of UV coordinates.
		/// </summary>
		[DefaultValue( "0 0" )]
		[DisplayName( "UV Offset" )]
		[Range( 0, 1 )]
		public Reference<Vector2> UVOffset
		{
			get { if( _uVOffset.BeginGet() ) UVOffset = _uVOffset.Get( this ); return _uVOffset.value; }
			set { if( _uVOffset.BeginSet( this, ref value ) ) { try { UVOffsetChanged?.Invoke( this ); ShouldRecompileMesh(); } finally { _uVOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVOffset"/> property value changes.</summary>
		public event Action<MeshGeometry_Plane> UVOffsetChanged;
		ReferenceField<Vector2> _uVOffset = Vector2.Zero;


		////InsideOut
		//ReferenceField<bool> _insideOut = false;
		//[DefaultValue( false )]
		//[Serialize]
		//public Reference<bool> InsideOut
		//{
		//	get
		//	{
		//		if( _insideOut.BeginGet() )
		//			InsideOut = _insideOut.Get( this );
		//		return _insideOut.value;
		//	}
		//	set
		//	{
		//		if( _insideOut.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				InsideOutChanged?.Invoke( this );
		//				ShouldRecompileMesh();
		//			}
		//			finally { _insideOut.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<MeshGeometry_Plane> InsideOutChanged;

		//!!!!!by tesselation modifier?

		////SegmentsHorizontal
		//Reference<int> segmentsHorizontal = 32;
		//[Serialize]
		//public virtual Reference<int> SegmentsHorizontal
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( segmentsHorizontal.GetByReference ) )
		//			SegmentsHorizontal = segmentsHorizontal.GetValue( this );
		//		return segmentsHorizontal;
		//	}
		//	set
		//	{
		//		if( segmentsHorizontal == value ) return;
		//		segmentsHorizontal = value;
		//		SegmentsHorizontalChanged?.Invoke( this );
		//	}
		//}
		//public event Action<MeshData_Parallelepiped> SegmentsHorizontalChanged;


		////SegmentsVertical
		//Reference<int> segmentsVertical = 32;
		//[Serialize]
		//public virtual Reference<int> SegmentsVertical
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( segmentsVertical.GetByReference ) )
		//			SegmentsVertical = segmentsVertical.GetValue( this );
		//		return segmentsVertical;
		//	}
		//	set
		//	{
		//		if( segmentsVertical == value ) return;
		//		segmentsVertical = value;
		//		SegmentsVerticalChanged?.Invoke( this );
		//	}
		//}
		//public event Action<MeshData_Parallelepiped> SegmentsVerticalChanged;

		/////////////////////////////////////////

		public override void GetProceduralGeneratedData( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Material material, ref byte[] voxelData, ref byte[] clusterData, ref Mesh.StructureClass structure )
		{
			vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );
			unsafe
			{
				if( vertexSize != sizeof( StandardVertex.StaticOneTexCoord ) )
					Log.Fatal( "vertexSize != sizeof( StandardVertexF )" );
			}

			var dimensions = Dimensions.Value;
			//if( InsideOut )
			//	dimensions = -dimensions;

			SimpleMeshGenerator.GenerateSegmentedPlane( Axis, dimensions.ToVector2F(), Segments.Value, UVTilesPerUnit.Value.ToVector2F(), UVTilesInTotal.Value.ToVector2F(), UVOffset.Value.ToVector2F(), out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out indices, out var faces );
			//SimpleMeshGenerator.GeneratePlane( dimensions, UVTilesPerUnit.Value, UVTilesInTotal.Value, out Vector3F[] positions, out var normals, out var tangents, out var texCoords, out indicesV, out var faces );

			if( faces != null )
				structure = SimpleMeshGenerator.CreateMeshStructure( faces );

			vertices = new byte[ vertexSize * positions.Length ];
			unsafe
			{
				fixed( byte* pVertices = vertices )
				{
					StandardVertex.StaticOneTexCoord* pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

					for( int n = 0; n < positions.Length; n++ )
					{
						pVertex->Position = positions[ n ];
						pVertex->Normal = normals[ n ];
						pVertex->Tangent = tangents[ n ];
						pVertex->Color = new ColorValue( 1, 1, 1, 1 );
						pVertex->TexCoord0 = texCoords[ n ];

						pVertex++;
					}
				}
			}
		}
	}
}
