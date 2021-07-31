// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

class YUVToRGBConverter
{
public:
	static YUVToRGBConverter* instance;

	int YTable[256];
	int BUTable[256];
	int GUTable[256];
	int GVTable[256];
	int RVTable[256];

	//

	static void Init();
	static void Shutdown();

	void InitInternal();
	void ShutdownInternal();

	void Convert(int yWidth, int yHeight, int yStride, int uvWidth, int uvHeight, int uvStride,
		Ogre::uint8* ySrc, Ogre::uint8* uSrc, Ogre::uint8* vSrc, int destBufferSizeX, Ogre::uint8* destBuffer,
		bool isABGR) const;
};
