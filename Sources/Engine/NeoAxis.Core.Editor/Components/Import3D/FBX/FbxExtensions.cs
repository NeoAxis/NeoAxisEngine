// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using Internal.Fbx;

namespace NeoAxis.Import.FBX
{
	static class FbxExtensions
	{
		public static Matrix4 ToMatrix4( this FbxAMatrix value )
		{
			// value.Get( rowIndex, columnIndex )
			return new Matrix4(
				value.Get( 0, 0 ), value.Get( 0, 1 ), value.Get( 0, 2 ), value.Get( 0, 3 ),
				value.Get( 1, 0 ), value.Get( 1, 1 ), value.Get( 1, 2 ), value.Get( 1, 3 ),
				value.Get( 2, 0 ), value.Get( 2, 1 ), value.Get( 2, 2 ), value.Get( 2, 3 ),
				value.Get( 3, 0 ), value.Get( 3, 1 ), value.Get( 3, 2 ), value.Get( 3, 3 )
				);
			//return new Mat4(
			//	value.A1, value.B1, value.C1, value.D1,
			//	value.A2, value.B2, value.C2, value.D2,
			//	value.A3, value.B3, value.C3, value.D3,
			//	value.A4, value.B4, value.C4, value.D4 );
		}

		public static Matrix4 ToMatrix4( this FbxMatrix value )
		{
			// value.Get( rowIndex, columnIndex )
			return new Matrix4(
				value.Get( 0, 0 ), value.Get( 0, 1 ), value.Get( 0, 2 ), value.Get( 0, 3 ),
				value.Get( 1, 0 ), value.Get( 1, 1 ), value.Get( 1, 2 ), value.Get( 1, 3 ),
				value.Get( 2, 0 ), value.Get( 2, 1 ), value.Get( 2, 2 ), value.Get( 2, 3 ),
				value.Get( 3, 0 ), value.Get( 3, 1 ), value.Get( 3, 2 ), value.Get( 3, 3 )
			);
		}

		public static Matrix4F ToMatrix4F( this FbxAMatrix value )
		{
			// value.Get( rowIndex, columnIndex )
			return new Matrix4F(
				(float)value.Get( 0, 0 ), (float)value.Get( 0, 1 ), (float)value.Get( 0, 2 ), (float)value.Get( 0, 3 ),
				(float)value.Get( 1, 0 ), (float)value.Get( 1, 1 ), (float)value.Get( 1, 2 ), (float)value.Get( 1, 3 ),
				(float)value.Get( 2, 0 ), (float)value.Get( 2, 1 ), (float)value.Get( 2, 2 ), (float)value.Get( 2, 3 ),
				(float)value.Get( 3, 0 ), (float)value.Get( 3, 1 ), (float)value.Get( 3, 2 ), (float)value.Get( 3, 3 )
			);
		}

		public static Matrix4F ToMatrix4F( this FbxMatrix value )
		{
			// value.Get( rowIndex, columnIndex )
			return new Matrix4F(
				(float)value.Get( 0, 0 ), (float)value.Get( 0, 1 ), (float)value.Get( 0, 2 ), (float)value.Get( 0, 3 ),
				(float)value.Get( 1, 0 ), (float)value.Get( 1, 1 ), (float)value.Get( 1, 2 ), (float)value.Get( 1, 3 ),
				(float)value.Get( 2, 0 ), (float)value.Get( 2, 1 ), (float)value.Get( 2, 2 ), (float)value.Get( 2, 3 ),
				(float)value.Get( 3, 0 ), (float)value.Get( 3, 1 ), (float)value.Get( 3, 2 ), (float)value.Get( 3, 3 )
			);
		}

		public static ColorValue ToColorValue( this FbxColor color )
		{
			return new ColorValue( color.mRed, color.mGreen, color.mBlue, color.mAlpha );
		}

		//!!!!slowly
		public static Vector3 ToVector3( this FbxVector4 value )
		{
			SWIGTYPE_p_double data = value.mData;
			DoubleArray d = DoubleArray.frompointer( data );
			return new Vector3( d.getitem( 0 ), d.getitem( 1 ), d.getitem( 2 ) );
		}

		//!!!!slowly
		public static Vector3F ToVector3F( this FbxVector4 value )
		{
			SWIGTYPE_p_double data = value.mData;
			DoubleArray d = DoubleArray.frompointer( data );
			return new Vector3F( (float)d.getitem( 0 ), (float)d.getitem( 1 ), (float)d.getitem( 2 ) );
		}

		//!!!!slowly
		public static Vector4 ToVector4( this FbxVector4 value )
		{
			SWIGTYPE_p_double data = value.mData;
			DoubleArray d = DoubleArray.frompointer( data );
			return new Vector4( d.getitem( 0 ), d.getitem( 1 ), d.getitem( 2 ), d.getitem( 3 ) );
		}

		public static FbxTime ToFbxTime( long value )
		{
			// Этот тип в C++: typedef signed __int64		FbxInt64;  В Wrapper как SWIGTYPE_p_signed___int64.
			//В Swig была добавлена функция:  FbxTime.ToFbxLongLong, FbxTime.ToInt64 :  static FbxLongLong ToFbxLongLong(__int64 value) {return FbxLongLong( value );}
			// добавлено из-за того что SWIG правильно не распознает FbxLongLong хотя он определен через typedef как __int64
			// Иначе не получается передавать FbxLongLong :

			return new FbxTime( FbxTime.ToFbxLongLong( value ) );
		}

		public static FbxMatrix ToFbxMatrix( this FbxAMatrix value )
		{
			return new FbxMatrix( value );
		}

		public static int GetAnimLayerCount( FbxAnimStack animStack )
		{
			return animStack.GetMemberCount( FbxCriteria.ObjectType( FbxAnimLayer.ClassId ) );  //pAnimStack.GetMemberCount<FbxAnimLayer>(); 

		}

		public static FbxAnimLayer GetAnimLayer( FbxAnimStack animStack, int index )
		{
			return FbxAnimLayer.Cast( animStack.GetSrcObject( FbxCriteria.ObjectType( FbxAnimLayer.ClassId ), index ) ); //pAnimStack.GetMember<FbxAnimLayer>( index );
		}

		public static FbxAnimStack GetAnimStack( FbxScene scene, int trackIndex )
		{
			return FbxAnimStack.Cast( scene.GetSrcObject( FbxCriteria.ObjectType( FbxAnimStack.ClassId ), trackIndex ) ); //scene.GetSrcObject<FbxAnimStack>( trackIndex );
		}

		public static int GetAnimStackCount( FbxScene scene )
		{
			return scene.GetSrcObjectCount( FbxCriteria.ObjectType( FbxAnimStack.ClassId ) ); //scene.GetSrcObject<FbxAnimStack>( trackIndex );
		}


		//public static double[] ToDouble3Array( FbxVector4 value )
		//{
		//	SWIGTYPE_p_double data = value.mData;
		//	DoubleArray d = DoubleArray.frompointer( data );
		//	return new double[]{ d.getitem( 0 ), d.getitem( 1 ), d.getitem( 2 ) };
		//}

		public static Vector3 ToVector3( this FbxDouble3 value )
		{
			SWIGTYPE_p_double data = value.mData;
			DoubleArray d = DoubleArray.frompointer( data );
			return new Vector3( d.getitem( 0 ), d.getitem( 1 ), d.getitem( 2 ) );
		}

		//////The fourth component of this object is assigned 1.
		//public static Vec4 ToVec4( this FbxDouble3 value, double w )
		//{
		//	SWIGTYPE_p_double data = value.mData;
		//	DoubleArray d = DoubleArray.frompointer( data );
		//	return new Vec4( d.getitem( 0 ), d.getitem( 1 ), d.getitem( 2 ), w );// 1 );
		//}

		public static FbxLayerElementUV[] GetElementUVs( FbxMesh fbxMesh )
		{
			var ret = new List<FbxLayerElementUV>();
			FbxStringList lst = new FbxStringList();
			fbxMesh.GetUVSetNames( lst );
			for( int i = 0; i < lst.GetCount(); i++ )
			{
				string uvSetName = lst.GetItemAt( i ).mString.Buffer();
				var uv = fbxMesh.GetElementUV( uvSetName );
				if( uv != null )
					ret.Add( uv );
			}
			return ret.ToArray();
		}

		public static Vector2 ToVector2( this FbxVector2 fbxVector2 )
		{
			//FbxVector имеет метод at - доступ по индексу, но он все равно возвращает указатель SWIGTYPE_p_double, 
			//который пришлось бы разименовывать специалной функцией. Поэтому берется mData.
			return fbxVector2.mData.ToVector2();
		}

		public static Vector2 ToVector2( this SWIGTYPE_p_double ptr )
		{
			return new Vector2( FbxWrapperNative.DoubleArrayFunc_getitem( ptr, 0 ), FbxWrapperNative.DoubleArrayFunc_getitem( ptr, 1 ) );
		}

		public static FbxNodeAttribute[] GetAttributes( FbxNode node )
		{
			//не null только когда node.GetNodeAttributeCount()==1. Иначе использовать GetNodeAttributeByIndex
			var attribute = node.GetNodeAttribute();
			if( attribute != null )
				return new[] { attribute };
			var ret = new FbxNodeAttribute[ node.GetNodeAttributeCount() ];
			for( int i = 0; i < ret.Length; i++ )
				ret[ i ] = node.GetNodeAttributeByIndex( i );
			return ret;
		}

		public static int[] GetPolygonVertices( FbxMesh mesh )
		{
			int count = mesh.GetPolygonVertexCount();
			var ar = IntArray.frompointer( mesh.GetPolygonVertices() );
			var ret = new int[ count ];
			for( int i = 0; i < count; i++ )
				ret[ i ] = ar.getitem( i );
			return ret;
		}

		public static bool GetNodeMesh( FbxNode pNode, out FbxMesh pMesh )
		{
			pMesh = null;
			for( int i = 0; i < pNode.GetNodeAttributeCount(); i++ )
			{
				if( pNode.GetNodeAttributeByIndex( i ).GetAttributeType() == FbxNodeAttribute.EType.eMesh )
				{
					pMesh = FbxMesh.Cast( pNode.GetNodeAttributeByIndex( i ) );
					return true;
				}
			}
			return false;
		}
	}
}
