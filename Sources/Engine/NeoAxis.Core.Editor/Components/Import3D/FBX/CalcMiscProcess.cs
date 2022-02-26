// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Internal.Fbx;

namespace NeoAxis.Import.FBX
{
	static class CalcMiscProcess
	{
		//public static void FlipUVs()
		//{
		//	//ToDo : сделать FlipUv для всех mesh, и во всех Materials развернуть UVTransform
		//}

		public static void FlipUVs( VertexInfo[] vertices, StandardVertex.Components components )
		{
			if( components.HasFlag( StandardVertex.Components.TexCoord0 ) )
			{
				for( int i = 0; i < vertices.Length; i++ )
					vertices[ i ].Vertex.TexCoord0.Y = 1.0f - vertices[ i ].Vertex.TexCoord0.Y;
			}
			if( components.HasFlag( StandardVertex.Components.TexCoord1 ) )
			{
				for( int i = 0; i < vertices.Length; i++ )
					vertices[ i ].Vertex.TexCoord1.Y = 1.0f - vertices[ i ].Vertex.TexCoord1.Y;
			}
			if( components.HasFlag( StandardVertex.Components.TexCoord2 ) )
			{
				for( int i = 0; i < vertices.Length; i++ )
					vertices[ i ].Vertex.TexCoord2.Y = 1.0f - vertices[ i ].Vertex.TexCoord2.Y;
			}
			if( components.HasFlag( StandardVertex.Components.TexCoord3 ) )
			{
				for( int i = 0; i < vertices.Length; i++ )
					vertices[ i ].Vertex.TexCoord3.Y = 1.0f - vertices[ i ].Vertex.TexCoord3.Y;
			}
		}

		//Перебор минимальных циклов на yield 5-10 наносекунд на итерацию, не тормозит.
		public static IEnumerable<RangeI> EnumeratePolygons( VertexInfo[] vertices )
		{
			int startIndex = 0;
			int curPolygonIndex = vertices[ 0 ].PolygonIndex;
			for( int i = 1; i < vertices.Length; i++ )
			{
				if( vertices[ i ].PolygonIndex != curPolygonIndex )
				{
					yield return new RangeI( startIndex, i );
					startIndex = i;
					curPolygonIndex = vertices[ i ].PolygonIndex;
				}
			}
			yield return new RangeI( startIndex, vertices.Length );
		}

		//public static void FlipUVForMaterial( FbxSurfaceMaterial material )
		//{
		//	int propCount = material.GetSrcPropertyCount();
		//	for( int a = 0; a < propCount; ++a )
		//	{
		//		FbxProperty prop = material.GetSrcProperty( a );
		//		if( prop == null )
		//		{
		//			FbxImportLog.LogMessage( "Property is null" );
		//			continue;
		//		}
		//........
		//	}
		//}

		//ToDo :  ... Вызывалось в FlipUvs
		//// Converts a single material
		//ToDo  : Читается assimp(собственное а не из FBX) Property ("$tex.uvtrafo"), которое записываются в: (файл FbxConverter) Converter::TrySetTextureProperties( aiMaterial* out_mat, const TextureMap& textures, const std::string& propName, aiTextureType target, const MeshGeometry* const mesh )
		//ToDo :  .... значения этого property берется из tex->UVScaling(); tex->UVTranslation();
		//void FlipUVsProcess::ProcessMaterial( aiMaterial* _mat )
		//{
		//	aiMaterial* mat = (aiMaterial*)_mat;
		//	for( unsigned int a = 0; a < mat->mNumProperties; ++a )
		//	{
		//		aiMaterialProperty* prop = mat->mProperties[a];
		//		if( !prop )
		//		{
		//			ASSIMP_LOG_DEBUG( "Property is null" );
		//			continue;
		//		}

		//		// UV transformation key?
		//		if( !::strcmp( prop->mKey.data, "$tex.uvtrafo" ) )
		//		{
		//			ai_assert( prop->mDataLength >= sizeof( aiUVTransform ) );  /* something is wrong with the validation if we end up here */
		//			aiUVTransform* uv = (aiUVTransform*)prop->mData;

		//			// just flip it, that's everything
		//			uv->mTranslation.y *= -1.f;
		//			uv->mRotation *= -1.f;
		//		}
		//	}
		//}

	}
}
