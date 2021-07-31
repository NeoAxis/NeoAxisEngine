// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "MemoryManagerInternal_precompiled.h"
#pragma hdrstop
#include "MemoryManagerInternal.h"
#include "MiniDump.h"
#include <exception>
#include <iostream>

///////////////////////////////////////////////////////////////////////////////////////////////////

#undef malloc
#undef free
#undef realloc

///////////////////////////////////////////////////////////////////////////////////////////////////

void Fatal(const char* text)
{
#ifdef PLATFORM_WINDOWS
	MessageBox(NULL, text, "Fatal", MB_OK | MB_ICONEXCLAMATION);
#elif defined(PLATFORM_WINRT)
	OutputDebugStringA("Fatal: ");
	OutputDebugStringA(text);
	OutputDebugStringA("\n");
	std::cerr << "Fatal: " << text << std::endl;
	//throw std::exception(text);
#elif defined(PLATFORM_MACOS)
	CFStringRef textRef = CFStringCreateWithCString(NULL, text, kCFStringEncodingASCII);
	CFUserNotificationDisplayAlert(0, kCFUserNotificationStopAlertLevel, NULL, NULL, NULL, 
		CFSTR("Fatal"), textRef, CFSTR("OK"), NULL, NULL, NULL);
	CFRelease(textRef);
#elif defined(PLATFORM_ANDROID)
//!!!!!!dr
	char tempBuffer[4096];
	sprintf(tempBuffer, "NativeMemoryManager: Fatal: %s\n", text);
	__android_log_write(ANDROID_LOG_ERROR,"NeoAxis Engine", tempBuffer);
#endif
	exit(0);
}

///////////////////////////////////////////////////////////////////////////////////////////////////

//#ifndef NATIVE_MEMORY_MANAGER_ENABLE
//#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
//CRITICAL_SECTION criticalSection;
//#else
//pthread_mutex_t mutex;
//#endif
//#endif //NATIVE_MEMORY_MANAGER_ENABLE

MemoryManager* memoryManager = NULL;

///////////////////////////////////////////////////////////////////////////////////////////////////

INLINE void Init()
{
	if(!memoryManager)
	{
#ifdef NATIVE_MEMORY_MANAGER_ENABLE
		memoryManager = CreateTCMallocMemoryManager();
		//memoryManager = CreatePagedMemoryManager();
		//memoryManager = CreateSimpleMemoryManager();
#else
		memoryManager = CreateCRTMemoryManager();
#endif
		
//#ifndef NATIVE_MEMORY_MANAGER_ENABLE
//#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
//	InitializeCriticalSection(&criticalSection);
//#else
//	pthread_mutex_init(&mutex, NULL);
//#endif
//#endif //NATIVE_MEMORY_MANAGER_ENABLE

	}
}

//void EnterMutex()
//{
//#ifndef NATIVE_MEMORY_MANAGER_ENABLE
//#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
//	EnterCriticalSection(&criticalSection);
//#else
//	pthread_mutex_lock(&mutex);
//#endif
//#endif //NATIVE_MEMORY_MANAGER_ENABLE
//}
//
//void LeaveMutex()
//{
//#ifndef NATIVE_MEMORY_MANAGER_ENABLE
//#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
//	LeaveCriticalSection(&criticalSection);
//#else
//	pthread_mutex_unlock(&mutex);
//#endif
//#endif //NATIVE_MEMORY_MANAGER_ENABLE
//}

void MemoryManager_GetStatistics(MemoryAllocationType allocationType, int64_t* allocatedMemory, int* allocationCount)
{
	Init();

	//EnterMutex();
	memoryManager->GetStatistics(allocationType, allocatedMemory, allocationCount);
	//LeaveMutex();
}

void MemoryManager_GetCRTStatistics(int64_t* allocatedMemory, int* allocationCount )
{
	Init();

	//EnterMutex();
	memoryManager->GetCRTStatistics(allocatedMemory, allocationCount);
	//LeaveMutex();
}

void* Memory_Alloc( MemoryAllocationType allocationType, int size, 
	const char* fileName, const int lineNumber )
{
	Init();

	//EnterMutex();
	void* result = memoryManager->Alloc(allocationType, size, fileName, lineNumber);
	//LeaveMutex();

	return result;
}

void* Memory_Realloc( MemoryAllocationType allocationType, void* pointer, 
	int newSize, const char* fileName, const int lineNumber )
{
	Init();

	//EnterMutex();

	void* result;
	if(pointer)
		result = memoryManager->Realloc(allocationType, pointer, newSize, fileName, lineNumber);
	else
		result = memoryManager->Alloc(allocationType, newSize, fileName, lineNumber);

	//LeaveMutex();

	return result;
}

void Memory_Free( void* pointer )
{
	Init();

	if(!pointer)
		return;

	//EnterMutex();
	memoryManager->Free(pointer);
	//LeaveMutex();
}

void* Memory_AllocAligned( MemoryAllocationType allocationType, int size, int align, const char* fileName, const int lineNumber )
{
	int newSize = size + align + sizeof( void* );
	unsigned char* mem = (unsigned char*)Memory_Alloc( allocationType, newSize, fileName, lineNumber );
	unsigned char* v = mem + (int)( align + sizeof( void* ) );
	void** ptr = (void**)( (int64_t)( v ) & ~( align - 1 ) );
	ptr[ -1 ] = mem;
	return ptr;
}

void Memory_FreeAligned( void* pointer )
{
	if(!pointer)
		return;
	void* p = ((void**)pointer)[ -1 ];
	Memory_Free(p);
}

void MemoryManager_GetAllocationInformation( MemoryManager_GetAllocationInformationDelegate* callback)
{
	Init();

	//EnterMutex();
	memoryManager->GetAllocationInformation(callback);
	//LeaveMutex();
}

char* Memory_StrDup( const char* strSource )
{
	size_t size = strlen(strSource) + 1;
	char* str = (char*)malloc(size);

#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
	strcpy_s(str, size, strSource);
#else
	strcpy(str, strSource);
#endif

	return str;
}

const char* GetCorrectFileNamePointer(const char* fileName)
{
	if(!fileName)
		return NULL;

	//check up for bad pointer
	{
#ifdef PLATFORM_WINDOWS

		char* pointer = (char*)fileName;
		for(int n = 0; n < MAX_PATH; n++)
		{
			if(IsBadReadPtr(pointer, 1))
				return "(Unloaded dll or corrupt memory)";

			char c = *pointer;
			if(c == 0)
				goto end;
			if(c < 32 || c > 127)
				return "(Unloaded dll or corrupt memory)";

			pointer++;
		}
		return "(Unloaded dll or corrupt memory)";
		end:;

#else

	return "(Unknown file)";

#endif
	}

	//make name without path
	{
		int length = (int)strlen(fileName);

		char* pointer = (char*)fileName + (length - 1);
		while(true)
		{
			char c = *pointer;
			if(c == '\\' || c == '/')
			{
				fileName = pointer + 1;
				break;
			}

			pointer--;
			if(pointer == fileName)
				break;
		}
	}

	return fileName;
}
