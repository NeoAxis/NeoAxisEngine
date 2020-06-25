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
	/// Mesh geometry in the form of a box.
	/// </summary>
	public class Component_MeshGeometry_Box : Component_MeshGeometry_Procedural
	{
		/// <summary>
		/// The size of the box.
		/// </summary>
		[Serialize]
		[DefaultValue( "1 1 1" )]
		//!!!!applicable range. сделать поддержку крутилок в чилдовых итемах.
		public Reference<Vector3> Dimensions
		{
			get { if( _dimensions.BeginGet() ) Dimensions = _dimensions.Get( this ); return _dimensions.value; }
			set
			{
				var v = value.Value;
				if( v.X < 0 || v.Y < 0 || v.Z < 0 )
				{
					if( v.X < 0 ) v.X = 0;
					if( v.Y < 0 ) v.Y = 0;
					if( v.Z < 0 ) v.Z = 0;
					value = new Reference<Vector3>( v, value.GetByReference );
				}
				if( _dimensions.BeginSet( ref value ) )
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
		public event Action<Component_MeshGeometry_Box> DimensionsChanged;
		ReferenceField<Vector3> _dimensions = Vector3.One;

		/// <summary>
		/// Whether the box is flipped.
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
		public event Action<Component_MeshGeometry_Box> InsideOutChanged;
		ReferenceField<bool> _insideOut = false;

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
		//public event Action<Component_MeshData_Parallelepiped> SegmentsHorizontalChanged;


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
		//public event Action<Component_MeshData_Parallelepiped> SegmentsVerticalChanged;


		//!!!!полусферной
		//!!!!закрытость сферы как Shape volume


		/////////////////////////////////////////

		//[StructLayout( LayoutKind.Sequential )]
		//public struct Vertex
		//{
		//	public Vec3F position;
		//	public Vec3F normal;
		//	public Vec2F texCoord;
		//}

		/////////////////////////////////////////

		public override void GetProceduralGeneratedData( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Component_Material material, ref Component_Mesh.StructureClass structure )
		{
			vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );
			unsafe
			{
				if( vertexSize != sizeof( StandardVertex.StaticOneTexCoord ) )
					Log.Fatal( "vertexSize != sizeof( StandardVertexF )" );
			}

			SimpleMeshGenerator.GenerateBox( Dimensions.Value.ToVector3F(), InsideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out indices, out var faces );

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

			//vertexStructureV = new VertexElement[ 3 ];
			//vertexStructureV[ 0 ] = new VertexElement( 0, 0, VertexElementType.Float3, VertexElementSemantic.Position );
			//vertexStructureV[ 1 ] = new VertexElement( 0, 12, VertexElementType.Float3, VertexElementSemantic.Normal );
			//vertexStructureV[ 2 ] = new VertexElement( 0, 24, VertexElementType.Float2, VertexElementSemantic.TextureCoordinates );
			////!!!!!tangents
			//int vertexSize = 32;

			//GeometryGenerator.GenerateBox( Size, out Vec3[] positions, out Vec3[] normals, out indicesV );

			//verticesV = new byte[ vertexSize * positions.Length ];

			//unsafe
			//{
			//	fixed ( byte* pVertices = verticesV )
			//	{
			//		Vertex* pVertex = (Vertex*)pVertices;

			//		for( int n = 0; n < positions.Length; n++ )
			//		{
			//			pVertex->position = positions[ n ].ToVec3F();
			//			pVertex->normal = normals[ n ].ToVec3F();

			//			//!!!!temp. как-то указывать способ нат€гивани€?
			//			pVertex->texCoord = pVertex->position.ToVec2() * 2;

			//			pVertex++;
			//		}
			//	}
			//}
		}
	}
}
