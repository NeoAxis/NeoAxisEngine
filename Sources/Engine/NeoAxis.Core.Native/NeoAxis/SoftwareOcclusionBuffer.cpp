// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "NeoAxisCoreNative.h"
#include "SoftwareOcclusionBuffer.h"
//
//using namespace Ogre;
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//class SoftwareOcclusionBuffer
//{
//public:
//	//!!!!или квадратный?
//	Vector2I bufferSize;
//
//	/////////////////////////////////////////////
//
//	class QueryVisibilityData
//	{
//	public:
//		int minDepth;
//		int maxDepth;
//
//		bool notVisible;
//	};
//
//	/////////////////////////////////////////////
//
//	//!!!!!можно все данные расположить локально в одном месте в памяти
//	//!!!!!!!!делая запросы тоже учитывать расположение в памяти
//	class Node
//	{
//	public:
//		Node* children[ 4 ];
//		int/*bool*/ endNode;
//		int minDepth;
//		int maxDepth;
//
//		//
//
//		bool CheckVisibility( QueryVisibilityData& queryData )
//		{
//			if( !endNode )
//			{
//				if( queryData.maxDepth <= minDepth )
//					return true;
//				if( queryData.minDepth > maxDepth )
//					return false;
//
//				for( int n = 0; n < 4; n++ )
//				{
//					Node* childNode = children[ n ];
//					if( childNode )//!!!!всегда проверять?
//					{
//						xx xx;
//						//http://sol.gfxile.net/tri/index.html
//
//						if( childNode->CheckVisibility( queryData ) )
//							return true;
//					}
//				}
//				return false;
//			}
//			else
//				return queryData.maxDepth <= minDepth;
//		}
//	};
//
//	/////////////////////////////////////////////
//
//	Node* rootNode;
//
//	/////////////////////////////////////////////
//
//	SoftwareOcclusionBuffer()
//	{
//		this->bufferSize = xx;
//	}
//
//	~SoftwareOcclusionBuffer()
//	{
//	}
//
//	//!!!!!!
//	void Configure( float maxDepth, xx viewMatricesEtc )
//	{
//	}
//
//	void Clear()
//	{
//	}
//
//	void Write( Vector3* vertices, int vertexCount, int* indices, int indexCount, const Matrix4& transform )
//	{
//	}
//
//	//!!!!!!also by sphere, box, convex mesh?
//	bool CheckVisibility( const Bounds& bounds )
//	{
//		//!!!! convert to 2d space
//
//		QueryVisibilityData queryData;
//
//		return rootNode->CheckVisibility( queryData );
//	}
//};
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//EXPORT SoftwareOcclusionBuffer* SoftwareOcclusionBuffer_New()
//{
//	return new SoftwareOcclusionBuffer();
//}
//
//EXPORT void SoftwareOcclusionBuffer_Delete( SoftwareOcclusionBuffer* buffer )
//{
//	delete buffer;
//}
