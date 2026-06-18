// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "NativeStableHeaders.h"
#include "NeoAxisCoreNative.h"
#include "NativeNativeWrapperGeneral.h"
#include "NativeImpl.h"

using namespace Native;


EXPORT void* NativeImpl_Example(void* obj, Vector3* arrayVec3F, wchar16* stringParameter, bool boolParameter, int& outputParameter)
{
	outputParameter = 0;

	WString stringParameter2 = TO_WCHAR_T(stringParameter);
	if (stringParameter2 == L"String Value")
		outputParameter = 22;

	return NULL;
}
