// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Mesh geometry in the form of an arch.
	/// </summary>
	public class MeshGeometry_Arch : MeshGeometry_Procedural
	{
		/// <summary>
		/// The axis of the arch.
		/// </summary>
		[Serialize]
		[DefaultValue( 0 )]
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
				if( _axis.BeginSet( ref value ) )
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
		public event Action<MeshGeometry_Arch> AxisChanged;
		ReferenceField<int> _axis = 0;

		/// <summary>
		/// The radius of the arch.
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
				if( _radius.BeginSet( ref value ) )
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
		public event Action<MeshGeometry_Arch> RadiusChanged;
		ReferenceField<double> _radius = 1.0;

		/// <summary>
		/// The thickness of the arch.
		/// </summary>
		[Serialize]
		[DefaultValue( 0.2 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Thickness
		{
			get { if( _thickness.BeginGet() ) Thickness = _thickness.Get( this ); return _thickness.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _thickness.BeginSet( ref value ) )
				{
					try
					{
						ThicknessChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _thickness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Thickness"/> property value changes.</summary>
		public event Action<MeshGeometry_Arch> ThicknessChanged;
		ReferenceField<double> _thickness = 0.2;

		/// <summary>
		/// The depth of the arch.
		/// </summary>
		[Serialize]
		[DefaultValue( 0.5 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Depth
		{
			get { if( _depth.BeginGet() ) Depth = _depth.Get( this ); return _depth.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _depth.BeginSet( ref value ) )
				{
					try
					{
						DepthChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _depth.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Depth"/> property value changes.</summary>
		public event Action<MeshGeometry_Arch> DepthChanged;
		ReferenceField<double> _depth = 0.5;

		/// <summary>
		/// The number of sides for the arch.
		/// </summary>
		[Serialize]
		[DefaultValue( 32 )]
		[Range( 2, 64 )]
		public Reference<int> Segments
		{
			get { if( _segments.BeginGet() ) Segments = _segments.Get( this ); return _segments.value; }
			set
			{
				if( value < 2 )
					value = new Reference<int>( 2, value.GetByReference );
				if( _segments.BeginSet( ref value ) )
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
		public event Action<MeshGeometry_Arch> SegmentsChanged;
		ReferenceField<int> _segments = 32;

		/// <summary>
		/// The number of depth segments for the arch.
		/// </summary>
		[Serialize]
		[DefaultValue( 1 )]
		[Range( 1, 32 )]
		public Reference<int> SegmentsDepth
		{
			get { if( _SegmentsDepth.BeginGet() ) SegmentsDepth = _SegmentsDepth.Get( this ); return _SegmentsDepth.value; }
			set
			{
				if( value < 1 )
					value = new Reference<int>( 1, value.GetByReference );
				if( _SegmentsDepth.BeginSet( ref value ) )
				{
					try
					{
						SegmentsDepthChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _SegmentsDepth.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SegmentsDepth"/> property value changes.</summary>
		public event Action<MeshGeometry_Arch> SegmentsDepthChanged;
		ReferenceField<int> _SegmentsDepth = 1;

		/// <summary>
		/// The degree of the arch circumference.
		/// </summary>
		[Serialize]
		[DefaultValue( "180" )]
		[Range( 0, 360 )]
		public Reference<Degree> Circumference
		{
			get { if( _circumference.BeginGet() ) Circumference = _circumference.Get( this ); return _circumference.value; }
			set
			{
				if( value < new Degree( 0 ) )
					value = new Reference<Degree>( 0, value.GetByReference );
				if( value > new Degree( 360 ) )
					value = new Reference<Degree>( 360, value.GetByReference );
				if( _circumference.BeginSet( ref value ) )
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
		public event Action<MeshGeometry_Arch> CircumferenceChanged;
		ReferenceField<Degree> _circumference = new Degree( 180 );

		/// <summary>
		/// Whether to create faces for the ends of the arch.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		public Reference<bool> EndCapes
		{
			get { if( _endCapes.BeginGet() ) EndCapes = _endCapes.Get( this ); return _endCapes.value; }
			set
			{
				if( _endCapes.BeginSet( ref value ) )
				{
					try
					{
						EndCapesChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _endCapes.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="EndCapes"/> property value changes.</summary>
		public event Action<MeshGeometry_Arch> EndCapesChanged;
		ReferenceField<bool> _endCapes = true;

		/// <summary>
		/// Whether the arch is flipped.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		public Reference<bool> InsideOut
		{
			get { if( _insideOut.BeginGet() ) InsideOut = _insideOut.Get( this ); return _insideOut.value; }
			set
			{
				if( _insideOut.BeginSet( ref value ) )
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
		public event Action<MeshGeometry_Arch> InsideOutChanged;
		ReferenceField<bool> _insideOut = false;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( EndCapes ):
					if( Circumference.Value == 360 )
						skip = true;
					break;
				}
			}
		}

		public override void GetProceduralGeneratedData( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Material material, ref byte[] billboardData, ref Mesh.StructureClass structure )
		{
			vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );
			unsafe
			{
				if( vertexSize != sizeof( StandardVertex.StaticOneTexCoord ) )
					Log.Fatal( "vertexSize != sizeof( StandardVertexF )" );
			}

			SimpleMeshGenerator.GenerateArch( Axis, Radius, Thickness, Depth, Segments, SegmentsDepth, Circumference, EndCapes, InsideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out indices, out var faces );

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
