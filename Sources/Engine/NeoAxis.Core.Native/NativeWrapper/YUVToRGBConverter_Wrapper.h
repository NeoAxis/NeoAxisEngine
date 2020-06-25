// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once
using namespace Ogre;

#include "YUVToRGBConverter.h"

EXPORT void YUVToRGBConverter_Convert(int yWidth, int yHeight, int yStride, int uvWidth, int uvHeight, 
	int uvStride, uint8* ySrc, uint8* uSrc, uint8* vSrc, int destBufferSizeX, uint8* destBuffer,
	bool isABGR)
{
	YUVToRGBConverter::instance->Convert(yWidth, yHeight, yStride, uvWidth, uvHeight, 
		uvStride, ySrc, uSrc, vSrc, destBufferSizeX, destBuffer, isABGR);
}
