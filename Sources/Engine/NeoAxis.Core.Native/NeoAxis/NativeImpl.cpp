// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "NeoAxisCoreNative.h"
#include "OgreNativeWrapperGeneral.h"
#include "NativeImpl.h"

using namespace Ogre;


EXPORT void* NativeImpl_Example(void* obj, Vector3* arrayVec3F, wchar16* stringParameter, bool boolParameter, int& outputParameter)
{
	outputParameter = 0;

	WString stringParameter2 = TO_WCHAR_T(stringParameter);
	if (stringParameter2 == L"String Value")
		outputParameter = 22;

	return NULL;
}
