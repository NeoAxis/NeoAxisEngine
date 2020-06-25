// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "NeoAxisCoreNative.h"
#include "UtilsNativeWrapper.h"

using namespace Ogre;

#ifdef USE_NATIVE_MEMORY_MANAGER

EXPORT void* NativeUtils_Alloc(MemoryAllocationType allocationType, int size)
{
	//return malloc(size);

	return Memory_Alloc(allocationType, size, NULL, 0);
}

EXPORT void NativeUtils_Free(void* pointer)
{
	//return free(pointer);

	Memory_Free(pointer);
}

#else

//!!!!

EXPORT void* NativeUtils_Alloc(int allocationType, int size)
{
	return malloc(size);
}

EXPORT void NativeUtils_Free(void* pointer)
{
	return free(pointer);
}

#endif

//

EXPORT void NativeUtils_CopyMemory( void* destination, void* source, int length )
{
#ifdef PLATFORM_WINDOWS
	CopyMemory( destination, source, length );
#else
	memcpy( destination, source, length );
#endif
}

EXPORT void NativeUtils_MoveMemory( void* destination, void* source, int length )
{
#ifdef PLATFORM_WINDOWS
	MoveMemory( destination, source, length );
#else
	memmove( destination, source, length );
#endif
}

EXPORT int NativeUtils_CompareMemory( void* buffer1, void* buffer2, int length )
{
	return memcmp( buffer1, buffer2, length );
}

EXPORT void NativeUtils_ZeroMemory( void* buffer, int length )
{
#ifdef PLATFORM_WINDOWS
	ZeroMemory( buffer, length );
#else
	memset( buffer, 0, length );
#endif
}

EXPORT void NativeUtils_FillMemory( void* buffer, int length, uint8 value )
{
#ifdef PLATFORM_WINDOWS
	FillMemory( buffer, length, value );
#else
	memset( buffer, value, length );
#endif
}

EXPORT int NativeUtils_CalculateHash( void* buffer, int length )
{
	uint8* pointer = (uint8*)buffer;

	const int p = 16777619;
	int hash = (int)2166136261;
	for( int n = 0; n < length; n++ )
	{
		hash = ( hash ^ (*pointer) ) * p;
		pointer++;
	}
	hash += hash << 13;
	hash ^= hash >> 7;
	hash += hash << 3;
	hash ^= hash >> 17;
	hash += hash << 5;
	return hash;
}

#if defined(PLATFORM_MACOS) || defined(PLATFORM_ANDROID)

EXPORT void* UtilsNativeWrapper_pthread_mutex_init()
{
	pthread_mutexattr_t attr;
	pthread_mutexattr_init(&attr);
	pthread_mutexattr_settype(&attr, PTHREAD_MUTEX_RECURSIVE);

	//!!!!
#ifndef ANDROID
	pthread_mutex_t* mutex = (pthread_mutex_t*)NativeUtils_Alloc(MemoryAllocationType_Utility, sizeof(pthread_mutex_t));
#else
	pthread_mutex_t* mutex = (pthread_mutex_t*)malloc(sizeof(pthread_mutex_t));
#endif

	int result = pthread_mutex_init(mutex, &attr);
	if(result != 0)
	{
		NativeUtils_Free(mutex);
		return NULL;
	}

	return mutex;
}

EXPORT void UtilsNativeWrapper_pthread_mutex_destroy(void* mutex)
{
	pthread_mutex_destroy((pthread_mutex_t*)mutex);
	NativeUtils_Free(mutex);
}

EXPORT bool UtilsNativeWrapper_pthread_mutex_lock(void* mutex)
{
	int result = pthread_mutex_lock((pthread_mutex_t*)mutex);
	if(result != 0)
		return false;
	return true;
}

EXPORT bool UtilsNativeWrapper_pthread_mutex_unlock(void* mutex)
{
	int result = pthread_mutex_unlock((pthread_mutex_t*)mutex);
	if(result != 0)
		return false;
	return true;
}

#endif

EXPORT int FloatingPointModel_GetValue()
{
#ifdef PLATFORM_WINDOWS
	unsigned int precision;
	_controlfp_s(&precision, 0, 0);
	if(precision & _PC_64)
		return 2;
	if(precision & _PC_53)
		return 1;
	if(precision & _PC_24)
		return 0;
#endif

	return 2;
}

EXPORT void FloatingPointModel_SetValue(int value)
{
#ifdef PLATFORM_WINDOWS
	switch(value)
	{
	case 0:_controlfp_s(NULL, _PC_24, MCW_PC);break;
	case 1:_controlfp_s(NULL, _PC_53, MCW_PC);break;
	case 2:_controlfp_s(NULL, _PC_64, MCW_PC);break;
	}
#endif
}
