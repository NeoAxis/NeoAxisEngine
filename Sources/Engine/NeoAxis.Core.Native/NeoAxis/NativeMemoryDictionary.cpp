//// Copyright 2006–2026 Ivan Efimov. All rights reserved.
//#include "NativeStableHeaders.h"
////#include "NeoAxisProprietary.h"
//#include "NativeMemoryDictionary.h"
//#include "NativeNativeWrapperGeneral.h"
//
//using namespace Native;
//
//EXPORT void* NativeMemoryDictionary_New()
//{
//	return new NativeMemoryDictionary();
//}
//
//EXPORT void NativeMemoryDictionary_Delete(NativeMemoryDictionary* obj)
//{
//	delete obj;
//}
//
//EXPORT void* NativeMemoryDictionary_AllocArray(NativeMemoryDictionary* obj, wchar16* name, int sizeInBytes)
//{
//	return obj->AllocArray(TO_WCHAR_T(name), sizeInBytes);
//}
//
//EXPORT void NativeMemoryDictionary_GetArray(NativeMemoryDictionary* obj, wchar16* name, void** pointer, int* sizeInBytes)
//{
//	obj->GetArray(TO_WCHAR_T(name), pointer, sizeInBytes);
//}
//
//EXPORT void NativeMemoryDictionary_SetString(NativeMemoryDictionary* obj, wchar16* name, wchar16* value)
//{
//	obj->SetString(TO_WCHAR_T(name), TO_WCHAR_T(value));
//}
//
//EXPORT void* NativeMemoryDictionary_GetString(NativeMemoryDictionary* obj, wchar16* name)
//{
//	WString value;
//	if (obj->GetString(TO_WCHAR_T(name), value))
//		return CreateOutString(value);
//	else
//		return nullptr;
//}
