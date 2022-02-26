// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

//#ifdef NATIVE_MEMORY_MANAGER_ENABLE
//
//#if (defined(_WIN32) || defined(__WIN32__))
//	#define MEMORYMANAGER_NEWDELETE_INLINE inline
//#elif defined(__APPLE_CC__)
//	#define MEMORYMANAGER_NEWDELETE_INLINE __private_extern__ inline __attribute__((always_inline))
//#else
//	#define MEMORYMANAGER_NEWDELETE_INLINE inline __attribute__((always_inline))
//#endif
//
//MEMORYMANAGER_NEWDELETE_INLINE void* NATIVEMEMORYMANAGER_CALLING_CONVENTION operator new(size_t nSize)
//{
//	return Memory_Alloc( _DefinedMemoryAllocationType, (int)nSize, NULL, 0 );
//}
//
//MEMORYMANAGER_NEWDELETE_INLINE void NATIVEMEMORYMANAGER_CALLING_CONVENTION operator delete(void* p)
//{
//	Memory_Free( p );
//}
//
//MEMORYMANAGER_NEWDELETE_INLINE void* NATIVEMEMORYMANAGER_CALLING_CONVENTION operator new[](size_t nSize)
//{
//	return Memory_Alloc( _DefinedMemoryAllocationType, (int)nSize, NULL, 0 );
//}
//
//MEMORYMANAGER_NEWDELETE_INLINE void NATIVEMEMORYMANAGER_CALLING_CONVENTION operator delete[](void* p)
//{
//	Memory_Free( p );
//}
//
//#endif //NATIVE_MEMORY_MANAGER_ENABLE
