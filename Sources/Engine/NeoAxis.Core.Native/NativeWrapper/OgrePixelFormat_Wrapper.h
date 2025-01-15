// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once
using namespace Ogre;

///////////////////////////////////////////////////////////////////////////////////////////////////

//EXPORT int OgrePixelUtil_getMemorySize(int width, int height, int depth, PixelFormat format)
//{
//	return PixelUtil::getMemorySize(width, height, depth, format);
//}

EXPORT int OgrePixelUtil_getNumElemBytes( PixelFormat format )
{
	return PixelUtil::getNumElemBytes(format);
}

EXPORT bool OgrePixelUtil_hasAlpha( PixelFormat format )
{
	return PixelUtil::hasAlpha(format);
}

EXPORT bool OgrePixelUtil_isFloatingPoint( PixelFormat format )
{
	return PixelUtil::isFloatingPoint(format);
}

EXPORT bool OgrePixelUtil_isCompressed( PixelFormat format )
{
	return PixelUtil::isCompressed(format);
}

EXPORT bool OgrePixelUtil_isDepth( PixelFormat format )
{
	return PixelUtil::isDepth(format);
}

//EXPORT void OgrePixelUtil_unpackColour(float *r, float *g, float *b, float *a, PixelFormat pf, 
//	const void* src)
//{
//	PixelUtil::unpackColour(r, g, b, a, pf, src);
//}

///////////////////////////////////////////////////////////////////////////////////////////////////
