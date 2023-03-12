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
	/// Mesh geometry in the form of a sphere.
	/// </summary>
	public class MeshGeometry_Sphere : MeshGeometry_Procedural
	{
		public enum SphereTypeEnum
		{
			GeoSphere,
			IcoSphere
		}

		/// <summary>
		/// The type of the sphere.
		/// </summary>
		[DefaultValue( SphereTypeEnum.GeoSphere )]
		[Serialize]
		public Reference<SphereTypeEnum> SphereType
		{
			get { if( _sphereType.BeginGet() ) SphereType = _sphereType.Get( this ); return _sphereType.value; }
			set
			{
				if( _sphereType.BeginSet( ref value ) )
				{
					try
					{
						SphereTypeChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _sphereType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SphereType"/> property value changes.</summary>
		public event Action<MeshGeometry_Sphere> SphereTypeChanged;
		ReferenceField<SphereTypeEnum> _sphereType = SphereTypeEnum.GeoSphere;

		/// <summary>
		/// The radius of the sphere.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Serialize]
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
		public event Action<MeshGeometry_Sphere> RadiusChanged;
		ReferenceField<double> _radius = 0.5;

		//!!!!defaults. везде так
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
		public event Action<MeshGeometry_Sphere> SegmentsHorizontalChanged;
		ReferenceField<int> _segmentsHorizontal = 32;

		/// <summary>
		/// The number of vertical segments used.
		/// </summary>
		[DefaultValue( 32 )]
		[Serialize]
		[Range( 2, 64 )]
		public Reference<int> SegmentsVertical
		{
			get { if( _segmentsVertical.BeginGet() ) SegmentsVertical = _segmentsVertical.Get( this ); return _segmentsVertical.value; }
			set
			{
				if( value < 2 )
					value = new Reference<int>( 2, value.GetByReference );
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
		public event Action<MeshGeometry_Sphere> SegmentsVerticalChanged;
		ReferenceField<int> _segmentsVertical = 32;

		/// <summary>
		/// The subdivision level of the sphere.
		/// </summary>
		[DefaultValue( 3 )]
		[Serialize]
		[Range( 0, 5 )]
		public Reference<int> Subdivisions
		{
			get { if( _subdivisions.BeginGet() ) Subdivisions = _subdivisions.Get( this ); return _subdivisions.value; }
			set
			{
				if( _subdivisions.BeginSet( ref value ) )
				{
					try
					{
						SubdivisionsChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _subdivisions.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Subdivisions"/> property value changes.</summary>
		public event Action<MeshGeometry_Sphere> SubdivisionsChanged;
		ReferenceField<int> _subdivisions = 3;

		/// <summary>
		/// Whether the sphere is flipped.
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
		public event Action<MeshGeometry_Sphere> InsideOutChanged;
		ReferenceField<bool> _insideOut = false;

		/////////////////////////////////////////

		public override void GetProceduralGeneratedData( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Material material, ref byte[] voxelData, ref byte[] clusterData, ref Mesh.StructureClass structure )
		{
			//!!!!!можно было бы не обновлять если такие же параметры

			vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );
			unsafe
			{
				if( vertexSize != sizeof( StandardVertex.StaticOneTexCoord ) )
					Log.Fatal( "vertexSize != sizeof( StandardVertexF )" );
			}

			Vector3F[] positions;
			Vector3F[] normals;
			Vector4F[] tangents;
			Vector2F[] texCoords;
			SimpleMeshGenerator.Face[] faces;

			if( SphereType.Value == SphereTypeEnum.GeoSphere )
			{
				SimpleMeshGenerator.GenerateSphere( Radius, SegmentsHorizontal, SegmentsVertical, InsideOut, out positions, out normals, out tangents, out texCoords, out indices, out faces );
			}
			else
			{
				SimpleMeshGenerator.GenerateIcoSphere( Radius, Subdivisions.Value, InsideOut, out positions, out normals, out tangents, out texCoords, out indices, out faces );
			}

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

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( SegmentsHorizontal ):
				case nameof( SegmentsVertical ):
					if( SphereType.Value != SphereTypeEnum.GeoSphere )
						skip = true;
					break;
				case nameof( Subdivisions ):
					if( SphereType.Value != SphereTypeEnum.IcoSphere )
						skip = true;
					break;
				}
			}
		}
	}
}
