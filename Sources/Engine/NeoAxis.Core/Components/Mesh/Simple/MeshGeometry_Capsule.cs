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
	/// Mesh geometry in the form of a capsule.
	/// </summary>
	public class MeshGeometry_Capsule : MeshGeometry_Procedural
	{
		/// <summary>
		/// The axis of the capsule.
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
		public event Action<MeshGeometry_Capsule> AxisChanged;
		ReferenceField<int> _axis = 2;

		/// <summary>
		/// The radius of the capsule.
		/// </summary>
		[Serialize]
		[DefaultValue( "0.5" )]
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
		public event Action<MeshGeometry_Capsule> RadiusChanged;
		ReferenceField<double> _radius = 0.5;

		/// <summary>
		/// The height of the capsule.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _height.BeginSet( ref value ) )
				{
					try
					{
						HeightChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _height.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<MeshGeometry_Capsule> HeightChanged;
		ReferenceField<double> _height = 1;

		/// <summary>
		/// The number of horizontal segments used.
		/// </summary>
		[DefaultValue( 32 )]
		[Serialize]
		[Range( 3, 64 )]
		public Reference<int> SegmentsHorizontal
		{
			get { if( _segmentsHorizontal.BeginGet() ) SegmentsHorizontal = _segmentsHorizontal.Get( this ); return _segmentsHorizontal.value; }
			set
			{
				if( value < 3 )
					value = new Reference<int>( 3, value.GetByReference );
				//!!!!очень большое число

				if( _segmentsHorizontal.BeginSet( ref value ) )
				{
					try
					{
						SegmentsHorizontalChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _segmentsHorizontal.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SegmentsHorizontal"/> property value changes.</summary>
		public event Action<MeshGeometry_Capsule> SegmentsHorizontalChanged;
		ReferenceField<int> _segmentsHorizontal = 32;

		/// <summary>
		/// The number of vertical segments used.
		/// </summary>
		[DefaultValue( 31 )]
		[Serialize]
		[Range( 3, 64 )]
		public Reference<int> SegmentsVertical
		{
			get { if( _segmentsVertical.BeginGet() ) SegmentsVertical = _segmentsVertical.Get( this ); return _segmentsVertical.value; }
			set
			{
				if( value < 3 )
					value = new Reference<int>( 3, value.GetByReference );
				//!!!!очень большое число

				if( _segmentsVertical.BeginSet( ref value ) )
				{
					try
					{
						SegmentsVerticalChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _segmentsVertical.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SegmentsVertical"/> property value changes.</summary>
		public event Action<MeshGeometry_Capsule> SegmentsVerticalChanged;
		ReferenceField<int> _segmentsVertical = 31;

		/// <summary>
		/// Whether the capsule is flipped.
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
		public event Action<MeshGeometry_Capsule> InsideOutChanged;
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

			int vert = SegmentsVertical;
			if( vert % 2 == 0 )
				vert++;
			var radius = Radius.Value;
			var height = Height.Value;
			if( InsideOut )
			{
				radius = -radius;
				height = -height;
			}
			SimpleMeshGenerator.GenerateCapsule( Axis, (float)radius, (float)height, SegmentsHorizontal, vert, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out indices, out var faces );

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

		//!!!!скрывать свойства Vertices, INdices, etc
		//!!!!!!!а материал?
	}
}
