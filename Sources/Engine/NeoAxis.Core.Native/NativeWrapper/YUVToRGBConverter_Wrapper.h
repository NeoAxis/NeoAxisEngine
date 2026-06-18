// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#pragma once
using namespace Native;

#include "YUVToRGBConverter.h"

EXPORT void YUVToRGBConverter_Convert(int yWidth, int yHeight, int yStride, int uvWidth, int uvHeight, 
	int uvStride, uint8* ySrc, uint8* uSrc, uint8* vSrc, int destBufferSizeX, uint8* destBuffer,
	bool isABGR)
{
	YUVToRGBConverter::instance->Convert(yWidth, yHeight, yStride, uvWidth, uvHeight, 
		uvStride, ySrc, uSrc, vSrc, destBufferSizeX, destBuffer, isABGR);
}
