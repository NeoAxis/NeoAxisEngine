// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "NativeStableHeaders.h"
#include "NativeNativeWrapperGeneral.h"
#include "NativeNativeWrapper.h"
#include "NativeImage.h"

//headers without .cpp
#include "NativeStringInterface_Wrapper.h"
#include "NativeImageManager_Wrapper.h"
#include "NativeLogManager_Wrapper.h"
#include "NativeResourceGroupManager_Wrapper.h"
#include "NativeArchiveManager_Wrapper.h"
#include "NativeNameValuePairList_Wrapper.h"
#include "NativeRoot_Wrapper.h"
#include "NativePixelFormat_Wrapper.h"
#include "YUVToRGBConverter_Wrapper.h"
#include "AdditionalMathFunctions.h"
#include "NativeRoot.h"
#include "MyNativeSceneManager.h"
#include "NativePlatformInformation.h"


#if defined(_UNICODE) && OGRE_PLATFORM != OGRE_PLATFORM_WINRT
	#error need multibyte 
#endif

using namespace Native;

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

wchar16* CreateOutString(const Native::WString& str)
{
#ifdef _WIN32
	wchar16* result = new wchar_t[str.length() + 1];
	wcscpy(result, str.c_str());
	return result;
#else
	int len = str.length();
	wchar16* result = new Native::wchar16[len + 1];
	for(int n = 0; n < len; n++)
		result[n] = (Native::wchar16)str[n];
	result[len] = 0;
	return result;
#endif
}

wchar16* CreateOutString(const Native::String& str)
{
	return CreateOutString(StringUtil::toUTFWide(str));
}

EXPORT void OgreNativeWrapper_FreeOutString(wchar16* pointer)
{
	delete[] pointer;
}

EXPORT wchar16* OgreNativeWrapper_GetGlobalParameter(const char* name)//, void* parameter1, void* parameter2, void* parameter3, void* parameter4)
{
	Native::String result;

	if (strcmp(name, "CPU_ID") == 0)
	{
		result = Native::PlatformInformation::getCpuIdentifier();
	}

	return CreateOutString(result);
}