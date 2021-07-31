// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "OgreNativeWrapperGeneral.h"
#include "OgreNativeWrapper.h"
#include "OgreImage.h"

//headers without .cpp
#include "OgreStringInterface_Wrapper.h"
#include "OgreImageManager_Wrapper.h"
#include "OgreLogManager_Wrapper.h"
#include "OgreResourceGroupManager_Wrapper.h"
#include "OgreArchiveManager_Wrapper.h"
#include "OgreNameValuePairList_Wrapper.h"
#include "OgreRoot_Wrapper.h"
#include "OgrePixelFormat_Wrapper.h"
#include "YUVToRGBConverter_Wrapper.h"
#include "AdditionalMathFunctions.h"
#include "OgreRoot.h"
#include "MyOgreSceneManager.h"


#if defined(_UNICODE) && OGRE_PLATFORM != OGRE_PLATFORM_WINRT
	#error need multibyte 
#endif

using namespace Ogre;

EXPORT void OgreNativeWrapper_CheckNativeBridge( int parameterTypeTextureCubeValue )
{
	if (parameterTypeTextureCubeValue != ParameterType_TextureCube)
		Fatal("OgreNativeWrapper: parameterTypeTextureCubeValue != ParameterType_TextureCube");


	if(sizeof(PolygonMode) != 4)
		Fatal("OgreNativeWrapper: sizeof(PolygonMode) != 4");

	if(sizeof(ShadowTechnique) != 4)
		Fatal("OgreNativeWrapper: sizeof(ShadowTechnique) != 4");

	if(sizeof(FogMode) != 4)
		Fatal("OgreNativeWrapper: sizeof(FogMode) != 4");

	if(sizeof(TextureFilterOptions) != 4)
		Fatal("OgreNativeWrapper: sizeof(TextureFilterOptions) != 4");

	if(sizeof(FilterType) != 4)
		Fatal("OgreNativeWrapper: sizeof(FilterType) != 4");

	if(sizeof(FilterOptions) != 4)
		Fatal("OgreNativeWrapper: sizeof(FilterOptions) != 4");
	
	if(sizeof(FrameBufferType) != 4)
		Fatal("OgreNativeWrapper: sizeof(FrameBufferType) != 4");

	if(sizeof(SceneBlendFactor) != 4)
		Fatal("OgreNativeWrapper: sizeof(SceneBlendFactor) != 4");

	if(sizeof(CullingMode) != 4)
		Fatal("OgreNativeWrapper: sizeof(CullingMode) != 4");

	if(sizeof(CompareFunction) != 4)
		Fatal("OgreNativeWrapper: sizeof(CompareFunction) != 4");

	if(sizeof(PixelFormat) != 4)
		Fatal("OgreNativeWrapper: sizeof(PixelFormat) != 4");

	if(sizeof(LogMessageLevel) != 4)
		Fatal("OgreNativeWrapper: sizeof(LogMessageLevel) != 4");

	if(sizeof(Capabilities) != 4)
		Fatal("OgreNativeWrapper: sizeof(Capabilities) != 4");
}

wchar16* CreateOutString(const Ogre::WString& str)
{
#ifdef _WIN32
	wchar16* result = new wchar_t[str.length() + 1];
	wcscpy(result, str.c_str());
	return result;
#else
	int len = str.length();
	wchar16* result = new Ogre::wchar16[len + 1];
	for(int n = 0; n < len; n++)
		result[n] = (Ogre::wchar16)str[n];
	result[len] = 0;
	return result;
#endif
}

wchar16* CreateOutString(const Ogre::String& str)
{
	return CreateOutString(StringUtil::toUTFWide(str));
}

EXPORT void OgreNativeWrapper_FreeOutString(wchar16* pointer)
{
	delete[] pointer;
}
