// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#pragma once
using namespace Native;

//EXPORT void AdditionalMathFunctions_Vec3ArrayMultiplyMat4( unsigned char* vertices, int vertexCount, int strideInBytes, 
//	const Native::Matrix4& transform )
//{
//	unsigned char* pointer = vertices;
//	for( int n = 0; n < vertexCount; n++ )
//	{
//		Native::Vector3* pVertex = (Native::Vector3*)pointer;
//		*pVertex = ( transform * ( *pVertex ) );
//		pointer += strideInBytes;
//	}
//}
//
//EXPORT void AdditionalMathFunctions_Vec3ArrayMultiplyMat3( unsigned char* vertices, int vertexCount, int strideInBytes, 
//	const Native::Matrix3& transform )
//{
//	unsigned char* pointer = vertices;
//	for( int n = 0; n < vertexCount; n++ )
//	{
//		Native::Vector3* pVertex = (Native::Vector3*)pointer;
//		*pVertex = ( transform * ( *pVertex ) );
//		pointer += strideInBytes;
//	}
//}
