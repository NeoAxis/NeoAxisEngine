// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

#include <map>

#include "MemoryManager.h"

///////////////////////////////////////////////////////////////////////////////////////////////////

#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
	#define INLINE __forceinline
#else
	#define INLINE inline
#endif

typedef unsigned char uint8;
#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
	typedef unsigned char uint16;
#endif

#ifdef PLATFORM_ANDROID
	typedef unsigned char uint16;
#endif

void Fatal(const char* text);

///////////////////////////////////////////////////////////////////////////////////////////////////

class MemoryManager
{
protected:

	MemoryManager()
	{
	}

public:

	virtual void* Alloc( MemoryAllocationType allocationType, int size, const char* fileName, const int lineNumber ) = 0;
	virtual void* Realloc( MemoryAllocationType allocationType, void* pointer, int newSize, const char* fileName, const int lineNumber ) = 0;
	virtual void Free( void* pointer ) = 0;

	virtual void GetStatistics(MemoryAllocationType allocationType, int64_t* allocatedMemory, int* allocationCount) = 0;
	virtual void GetCRTStatistics(int64_t* allocatedMemory, int* allocationCount) = 0;

	virtual void GetAllocationInformation( MemoryManager_GetAllocationInformationDelegate* callback ) = 0;
};

///////////////////////////////////////////////////////////////////////////////////////////////////

extern const char* GetCorrectFileNamePointer(const char* fileName);

///////////////////////////////////////////////////////////////////////////////////////////////////

extern MemoryManager* CreateCRTMemoryManager();
//extern MemoryManager* CreateTCMallocMemoryManager();
//extern MemoryManager* CreateSimpleMemoryManager();
//extern MemoryManager* CreatePagedMemoryManager();

///////////////////////////////////////////////////////////////////////////////////////////////////
