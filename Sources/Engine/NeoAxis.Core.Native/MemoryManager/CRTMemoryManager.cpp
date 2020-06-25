// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "MemoryManagerInternal_precompiled.h"
#pragma hdrstop
#include "MemoryManagerInternal.h"

#undef malloc
#undef free
#undef realloc
#undef _msize

///////////////////////////////////////////////////////////////////////////////////////////////////

class CRTMemoryManager : public MemoryManager
{
	int64_t* crtAllocatedMemory;
	//int crtAllocationCount;

	//////////////////////////////////////////////

public:

	CRTMemoryManager()
	{
#ifdef ANDROID
		int size = 8;
		int align = 16;
		int newSize = size + align + sizeof(void*);
		unsigned char* mem = (unsigned char*)malloc(newSize);
		unsigned char* v = mem + (int)(align + sizeof(void*));
		void** ptr = (void**)((int64_t)(v) & ~(align - 1));
		ptr[-1] = mem;
		crtAllocatedMemory = (int64_t*)ptr;
#else
		crtAllocatedMemory = (int64_t*)_aligned_malloc(8, 16);
#endif
		*crtAllocatedMemory = 0;
		//crtAllocationCount = 0;
	}

	~CRTMemoryManager()
	{
	}

	void GetStatistics(MemoryAllocationType allocationType, int64_t* allocatedMemory, int* allocationCount)
	{
		if(allocationType == MemoryAllocationType_Count)
		{
			*allocatedMemory = *crtAllocatedMemory;
			*allocationCount = 0;// crtAllocationCount;
		}
		else
		{
			*allocatedMemory = 0;
			*allocationCount = 0;
		}
	}

	void GetCRTStatistics(int64_t* allocatedMemory, int* allocationCount)
	{
		*allocatedMemory = *crtAllocatedMemory;
		*allocationCount = 0;// crtAllocationCount;
	}

	void* Alloc( MemoryAllocationType allocationType, int size, const char* fileName, const int lineNumber )
	{
#ifdef PLATFORM_WINDOWS
		InterlockedExchangeAdd64(crtAllocatedMemory, size);
#endif
		//crtAllocationCount++;

		return malloc(size);
	}

	void* Realloc( MemoryAllocationType allocationType, void* pointer, int newSize, const char* fileName, const int lineNumber )
	{
#ifdef PLATFORM_WINDOWS
		InterlockedExchangeAdd64(crtAllocatedMemory, newSize - (int)_msize(pointer));
#endif

		return realloc(pointer, newSize);
	}

	void Free( void* pointer )
	{
#ifdef PLATFORM_WINDOWS
		InterlockedExchangeAdd64(crtAllocatedMemory, -(int)_msize(pointer));
#endif
		//crtAllocationCount--;

		free(pointer);
	}

	void GetAllocationInformation( MemoryManager_GetAllocationInformationDelegate* callback )
	{
	}

};

///////////////////////////////////////////////////////////////////////////////////////////////////

MemoryManager* CreateCRTMemoryManager()
{
	return new CRTMemoryManager();
}

///////////////////////////////////////////////////////////////////////////////////////////////////
