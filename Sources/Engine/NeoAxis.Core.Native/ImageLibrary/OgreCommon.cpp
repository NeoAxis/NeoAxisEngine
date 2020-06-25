/*
-----------------------------------------------------------------------------
This source file is part of OGRE
    (Object-oriented Graphics Rendering Engine)
For the latest info, see http://www.ogre3d.org/

Copyright (c) 2000-2009 Torus Knot Software Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
#include "OgreStableHeaders.h"
#include "OgrePrerequisites.h"
#include "OgreCommon.h"
#include "OgreString.h"
#include "OgreLogManager.h"

namespace Ogre 
{

//	/** General hash function, derived from here
//	http://www.azillionmonkeys.com/qed/hash.html
//	Original by Paul Hsieh 
//	*/
//#if OGRE_ENDIAN == OGRE_ENDIAN_LITTLE
//#  define OGRE_GET16BITS(d) (*((const uint16 *) (d)))
//#else
//	// Cast to uint16 in little endian means first byte is least significant
//	// replicate that here
//#  define OGRE_GET16BITS(d) (*((const uint8 *) (d)) + (*((const uint8 *) (d+1))<<8))
//#endif
//	uint32 _OgreExport FastHash (const char * data, int len, uint32 hashSoFar)
//	{
//		uint32 hash;
//		uint32 tmp;
//		int rem;
//
//		if (hashSoFar)
//			hash = hashSoFar;
//		else
//			hash = len;
//
//		if (len <= 0 || data == NULL) return 0;
//
//		rem = len & 3;
//		len >>= 2;
//
//		/* Main loop */
//		for (;len > 0; len--) {
//			hash  += OGRE_GET16BITS (data);
//			tmp    = (OGRE_GET16BITS (data+2) << 11) ^ hash;
//			hash   = (hash << 16) ^ tmp;
//			data  += 2*sizeof (uint16);
//			hash  += hash >> 11;
//		}
//
//		/* Handle end cases */
//		switch (rem) {
//		case 3: hash += OGRE_GET16BITS (data);
//			hash ^= hash << 16;
//			hash ^= data[sizeof (uint16)] << 18;
//			hash += hash >> 11;
//			break;
//		case 2: hash += OGRE_GET16BITS (data);
//			hash ^= hash << 11;
//			hash += hash >> 17;
//			break;
//		case 1: hash += *data;
//			hash ^= hash << 10;
//			hash += hash >> 1;
//		}
//
//		/* Force "avalanching" of final 127 bits */
//		hash ^= hash << 3;
//		hash += hash >> 5;
//		hash ^= hash << 4;
//		hash += hash >> 17;
//		hash ^= hash << 25;
//		hash += hash >> 6;
//
//		return hash;
//	}

	_OgreExport String GetParameterTypeString(ParameterType type)
	{
		switch (type)
		{
		case Ogre::ParameterType_Unknown:return "Unknown";
		case Ogre::ParameterType_Object:return "Object";
		case Ogre::ParameterType_String:return "String";
		case Ogre::ParameterType_Boolean:return "Boolean";
		case Ogre::ParameterType_Byte:return "Byte";
		case Ogre::ParameterType_Integer:return "Integer";
		case Ogre::ParameterType_Vector2I:return "Vector2I";
		case Ogre::ParameterType_RectangleI:return "RectangleI";
		case Ogre::ParameterType_RangeI:return "RangeI";
		case Ogre::ParameterType_Vector3I:return "Vector3I";
		case Ogre::ParameterType_Vector4I:return "Vector4I";
		case Ogre::ParameterType_Float:return "Float";
		case Ogre::ParameterType_Radian:return "Radian";
		case Ogre::ParameterType_Degree:return "Degree";
		case Ogre::ParameterType_Vector2:return "Vector2";
		case Ogre::ParameterType_Range:return "Range";
		case Ogre::ParameterType_SphereDirection:return "SphereDirection";
		case Ogre::ParameterType_Vector3:return "Vector3";
		case Ogre::ParameterType_Angles:return "Angles";
		case Ogre::ParameterType_Vector4:return "Vector4";
		case Ogre::ParameterType_ColorValue:return "ColorValue";
		case Ogre::ParameterType_Rectangle:return "Rectangle";
		case Ogre::ParameterType_Quaternion:return "Quaternion";
		case Ogre::ParameterType_Bounds:return "Bounds";
		case Ogre::ParameterType_Box:return "Box";
		case Ogre::ParameterType_Capsule:return "Capsule";
		case Ogre::ParameterType_Cone:return "Cone";
		case Ogre::ParameterType_Line:return "Line";
		case Ogre::ParameterType_Plane:return "Plane";
		case Ogre::ParameterType_Ray:return "Ray";
		case Ogre::ParameterType_Matrix2x2:return "Matrix2x2";
		//case Ogre::ParameterType_Matrix2x3:return "Matrix2x3";
		//case Ogre::ParameterType_Matrix2x4:return "Matrix2x4";
		//case Ogre::ParameterType_Matrix3x2:return "Matrix3x2";
		case Ogre::ParameterType_Matrix3x3:return "Matrix3x3";
		//case Ogre::ParameterType_Matrix3x4:return "Matrix3x4";
		//case Ogre::ParameterType_Matrix4x2:return "Matrix4x2";
		//case Ogre::ParameterType_Matrix4x3:return "Matrix4x3";
		case Ogre::ParameterType_Matrix4x4:return "Matrix4x4";
		//case Ogre::ParameterType_Double:return "Double";
		//case Ogre::ParameterType_RadianD:return "RadianD";
		//case Ogre::ParameterType_DegreeD:return "DegreeD";
		//case Ogre::ParameterType_Vector2D:return "Vector2D";
		//case Ogre::ParameterType_RangeD:return "RangeD";
		//case Ogre::ParameterType_SphereDirectionD:return "SphereDirectionD";
		//case Ogre::ParameterType_Vector3D:return "Vector3D";
		//case Ogre::ParameterType_AnglesD:return "AnglesD";
		//case Ogre::ParameterType_Vector4D:return "Vector4D";
		//case Ogre::ParameterType_RectangleD:return "RectangleD";
		//case Ogre::ParameterType_QuaternionD:return "QuaternionD";
		//case Ogre::ParameterType_BoundsD:return "BoundsD";
		//case Ogre::ParameterType_BoxD:return "BoxD";
		//case Ogre::ParameterType_CapsuleD:return "CapsuleD";
		//case Ogre::ParameterType_ConeD:return "ConeD";
		//case Ogre::ParameterType_LineD:return "LineD";
		//case Ogre::ParameterType_PlaneD:return "PlaneD";
		//case Ogre::ParameterType_RayD:return "RayD";
		//case Ogre::ParameterType_Matrix2x2D:return "Matrix2x2D";
		//case Ogre::ParameterType_Matrix2x3D:return "Matrix2x3D";
		//case Ogre::ParameterType_Matrix2x4D:return "Matrix2x4D";
		//case Ogre::ParameterType_Matrix3x2D:return "Matrix3x2D";
		//case Ogre::ParameterType_Matrix3x3D:return "Matrix3x3D";
		//case Ogre::ParameterType_Matrix3x4D:return "Matrix3x4D";
		//case Ogre::ParameterType_Matrix4x2D:return "Matrix4x2D";
		//case Ogre::ParameterType_Matrix4x3D:return "Matrix4x3D";
		//case Ogre::ParameterType_Matrix4x4D:return "Matrix4x4D";
		//case Ogre::ParameterType_Texture1D:return "Texture1D";
		case Ogre::ParameterType_Texture2D:return "Texture2D";
		case Ogre::ParameterType_Texture3D:return "Texture3D";
		case Ogre::ParameterType_TextureCube:return "TextureCube";
		default:
			Fatal("GetParameterTypeString: No implementation.");
			return "";
		}
	}
}
