// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Mesh geometry in the form of a torus.
	/// </summary>
	public class MeshGeometry_Torus : MeshGeometry_Procedural
	{
		/// <summary>
		/// The axis of the torus.
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
		public event Action<MeshGeometry_Torus> AxisChanged;
		ReferenceField<int> _axis = 2;

		/// <summary>
		/// The radius of the entire torus.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _radius.BeginSet( this, ref value ) )
				{
					try
					{
						RadiusChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _radius.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<MeshGeometry_Torus> RadiusChanged;
		ReferenceField<double> _radius = 1.0;

		/// <summary>
		/// The number of torus segments used.
		/// </summary>
		[Serialize]
		[DefaultValue( 32 )]
		[Range( 3, 64 )]
		public Reference<int> Segments
		{
			get { if( _segments.BeginGet() ) Segments = _segments.Get( this ); return _segments.value; }
			set
			{
				if( value < 3 )
					value = new Reference<int>( 3, value.GetByReference );
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
		public event Action<MeshGeometry_Torus> SegmentsChanged;
		ReferenceField<int> _segments = 32;

		/// <summary>
		/// The degree of the torus circumference.
		/// </summary>
		[Serialize]
		[DefaultValue( 360 )]
		[Range( 0.01, 360 )]
		public Reference<Degree> Circumference
		{
			get { if( _circumference.BeginGet() ) Circumference = _circumference.Get( this ); return _circumference.value; }
			set
			{
				if( value.Value < 0.01 )
					value = new Reference<Degree>( 0.01, value.GetByReference );
				if( value.Value > 360 )
					value = new Reference<Degree>( 360, value.GetByReference );
				if( _circumference.BeginSet( this, ref value ) )
				{
					try
					{
						CircumferenceChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _circumference.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Circumference"/> property value changes.</summary>
		public event Action<MeshGeometry_Torus> CircumferenceChanged;
		ReferenceField<Degree> _circumference = new Degree( 360 );

		/// <summary>
		/// The radius of the torus tube.
		/// </summary>
		[Serialize]
		[DefaultValue( 0.2 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> TubeRadius
		{
			get { if( _tubeRadius.BeginGet() ) TubeRadius = _tubeRadius.Get( this ); return _tubeRadius.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _tubeRadius.BeginSet( this, ref value ) )
				{
					try
					{
						TubeRadiusChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _tubeRadius.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="TubeRadius"/> property value changes.</summary>
		public event Action<MeshGeometry_Torus> TubeRadiusChanged;
		ReferenceField<double> _tubeRadius = 0.2;

		/// <summary>
		/// The number of tube segments used.
		/// </summary>
		[Serialize]
		[DefaultValue( 16 )]
		[Range( 3, 32 )]
		public Reference<int> TubeSegments
		{
			get { if( _tubeSegments.BeginGet() ) TubeSegments = _tubeSegments.Get( this ); return _tubeSegments.value; }
			set
			{
				if( value < 3 )
					value = new Reference<int>( 3, value.GetByReference );
				if( _tubeSegments.BeginSet( this, ref value ) )
				{
					try
					{
						TubeSegmentsChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _tubeSegments.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="TubeSegments"/> property value changes.</summary>
		public event Action<MeshGeometry_Torus> TubeSegmentsChanged;
		ReferenceField<int> _tubeSegments = 16;

		/// <summary>
		/// The degree of the tube circumference.
		/// </summary>
		[Serialize]
		[DefaultValue( 360 )]
		[Range( 0.01, 360 )]
		public Reference<Degree> TubeCircumference
		{
			get { if( _tubeCircumference.BeginGet() ) TubeCircumference = _tubeCircumference.Get( this ); return _tubeCircumference.value; }
			set
			{
				if( value.Value < 0.01 )
					value = new Reference<Degree>( 0.01, value.GetByReference );
				if( value.Value > 360 )
					value = new Reference<Degree>( 360, value.GetByReference );
				if( _tubeCircumference.BeginSet( this, ref value ) )
				{
					try
					{
						TubeCircumferenceChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _tubeCircumference.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="TubeCircumference"/> property value changes.</summary>
		public event Action<MeshGeometry_Torus> TubeCircumferenceChanged;
		ReferenceField<Degree> _tubeCircumference = new Degree( 360 );

		////!!!!другим фигурам Smooth
		///// <summary>
		///// Whether to smooth the edges of the polygons.
		///// </summary>
		//[Serialize]
		//[DefaultValue( true )]
		//public Reference<bool> Smooth
		//{
		//	get { if( _smooth.BeginGet() ) Smooth = _smooth.Get( this ); return _smooth.value; }
		//	set
		//	{
		//		if( _smooth.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				SmoothChanged?.Invoke( this );
		//				ShouldRecompileMesh();
		//			}
		//			finally { _smooth.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="Smooth"/> property value changes.</summary>
		//public event Action<MeshGeometry_Torus> SmoothChanged;
		//ReferenceField<bool> _smooth = true;

		/// <summary>
		/// Whether the torus is flipped.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		public Reference<bool> InsideOut
		{
			get { if( _insideOut.BeginGet() ) InsideOut = _insideOut.Get( this ); return _insideOut.value; }
			set
			{
				if( _insideOut.BeginSet( this, ref value ) )
				{
					try
					{
						InsideOutChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _insideOut.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="InsideOut"/> property value changes.</summary>
		public event Action<MeshGeometry_Torus> InsideOutChanged;
		ReferenceField<bool> _insideOut = false;

		/////////////////////////////////////////

		public override void GetProceduralGeneratedData( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Material material, ref byte[] voxelData, ref byte[] clusterData, ref Mesh.StructureClass structure )
		{
			vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );
			unsafe
			{
				if( vertexSize != sizeof( StandardVertex.StaticOneTexCoord ) )
					Log.Fatal( "vertexSize != sizeof( StandardVertexF )" );
			}

			SimpleMeshGenerator.GenerateTorus( Axis, Radius, Segments, Circumference, TubeRadius, TubeSegments, TubeCircumference, /*Smooth, */InsideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out indices, out var faces );

			if( faces != null )
				structure = SimpleMeshGenerator.CreateMeshStructure( faces );

			vertices = new byte[ vertexSize * positions.Length ];
			unsafe
			{
				fixed ( byte* pVertices = vertices )
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
