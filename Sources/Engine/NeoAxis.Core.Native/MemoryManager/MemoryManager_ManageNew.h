// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

//#ifdef NATIVE_MEMORY_MANAGER_ENABLE
//
//#include "MemoryManager_SimpleNew.h"
//
//inline void* NATIVEMEMORYMANAGER_CALLING_CONVENTION operator new(size_t nSize, int nType, const char* lpszFileName, int nLine)
//{
//	return Memory_Alloc( _DefinedMemoryAllocationType, (int)nSize, lpszFileName, nLine );
//}
//
//inline void NATIVEMEMORYMANAGER_CALLING_CONVENTION operator delete(void* p, int nType, const char*, int)
//{
//	Memory_Free( p );
//}
//
//inline void* NATIVEMEMORYMANAGER_CALLING_CONVENTION operator new[](size_t nSize, int nType, const char* lpszFileName, int nLine)
//{
//	return ::operator new(nSize, nType, lpszFileName, nLine);
//}
//
//inline void NATIVEMEMORYMANAGER_CALLING_CONVENTION operator delete[](void* p, int nType, const char* lpszFileName, int nLine)
//{
//	::operator delete(p, nType, lpszFileName, nLine);
//}
//
//inline void* NATIVEMEMORYMANAGER_CALLING_CONVENTION operator new(size_t nSize, const char* lpszFileName, int nLine)
//{
//	return ::operator new(nSize, 1/*_NORMAL_BLOCK*/, lpszFileName, nLine);
//}
//
//inline void* NATIVEMEMORYMANAGER_CALLING_CONVENTION operator new[](size_t nSize, const char* lpszFileName, int nLine)
//{
//	return ::operator new[](nSize, 1/*_NORMAL_BLOCK*/, lpszFileName, nLine);
//}
//
//inline void NATIVEMEMORYMANAGER_CALLING_CONVENTION operator delete(void* pData, const char*, int)
//{
//	::operator delete(pData);
//}
//
//inline void NATIVEMEMORYMANAGER_CALLING_CONVENTION operator delete[](void* pData, const char*, int)
//{
//	::operator delete(pData);
//}
//
//#define new new(__FILE__, __LINE__)
//
//#endif //NATIVE_MEMORY_MANAGER_ENABLE
