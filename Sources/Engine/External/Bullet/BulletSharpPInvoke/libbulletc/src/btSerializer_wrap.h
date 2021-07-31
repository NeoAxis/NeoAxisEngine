#include "main.h"

#ifndef BT_SERIALIZER_H
#define p_btSerializer_allocate void*
#define p_btSerializer_finalizeChunk void*
#define p_btSerializer_findNameForPointer void*
#define p_btSerializer_findPointer void*
#define p_btSerializer_finishSerialization void*
#define p_btSerializer_getBufferPointer void*
#define p_btSerializer_getChunk void*
#define p_btSerializer_getCurrentBufferSize void*
#define p_btSerializer_getNumChunks void*
#define p_btSerializer_getSerializationFlags void*
#define p_btSerializer_getUniquePointer void*
#define p_btSerializer_registerNameForPointer void*
#define p_btSerializer_serializeName void*
#define p_btSerializer_setSerializationFlags void*
#define p_btSerializer_startSerialization void*
#define btSerializerWrapper void
#else
typedef btChunk* (*p_btSerializer_allocate)(size_t size, int numElements);
typedef void (*p_btSerializer_finalizeChunk)(btChunk* chunk, const char* structType,
	int chunkCode, void* oldPtr);
typedef const char* (*p_btSerializer_findNameForPointer)(const void* ptr);
typedef void* (*p_btSerializer_findPointer)(void* oldPtr);
typedef void (*p_btSerializer_finishSerialization)();
typedef const unsigned char* (*p_btSerializer_getBufferPointer)();
typedef btChunk* (*p_btSerializer_getChunk)(int chunkIndex);
typedef int (*p_btSerializer_getCurrentBufferSize)();
typedef int (*p_btSerializer_getNumChunks)();
typedef int (*p_btSerializer_getSerializationFlags)();
typedef void* (*p_btSerializer_getUniquePointer)(void* oldPtr);
typedef void (*p_btSerializer_registerNameForPointer)(const void* ptr, const char* name);
typedef void (*p_btSerializer_serializeName)(const char* ptr);
typedef void (*p_btSerializer_setSerializationFlags)(int flags);
typedef void (*p_btSerializer_startSerialization)();

class btSerializerWrapper : public btSerializer
{
private:
	p_btSerializer_allocate _allocateCallback;
	p_btSerializer_finalizeChunk _finalizeChunkCallback;
	p_btSerializer_findNameForPointer _findNameForPointerCallback;
	p_btSerializer_findPointer _findPointerCallback;
	p_btSerializer_finishSerialization _finishSerializationCallback;
	p_btSerializer_getBufferPointer _getBufferPointerCallback;
	p_btSerializer_getChunk _getChunkCallback;
	p_btSerializer_getCurrentBufferSize _getCurrentBufferSizeCallback;
	p_btSerializer_getNumChunks _getNumChunksCallback;
	p_btSerializer_getSerializationFlags _getSerializationFlagsCallback;
	p_btSerializer_getUniquePointer _getUniquePointerCallback;
	p_btSerializer_registerNameForPointer _registerNameForPointerCallback;
	p_btSerializer_serializeName _serializeNameCallback;
	p_btSerializer_setSerializationFlags _setSerializationFlagsCallback;
	p_btSerializer_startSerialization _startSerializationCallback;

public:
	btSerializerWrapper(p_btSerializer_allocate allocateCallback, p_btSerializer_finalizeChunk finalizeChunkCallback,
		p_btSerializer_findNameForPointer findNameForPointerCallback, p_btSerializer_findPointer findPointerCallback,
		p_btSerializer_finishSerialization finishSerializationCallback, p_btSerializer_getBufferPointer getBufferPointerCallback,
		p_btSerializer_getChunk getChunkCallback, p_btSerializer_getCurrentBufferSize getCurrentBufferSizeCallback,
		p_btSerializer_getNumChunks getNumChunksCallback, p_btSerializer_getSerializationFlags getSerializationFlagsCallback,
		p_btSerializer_getUniquePointer getUniquePointerCallback, p_btSerializer_registerNameForPointer registerNameForPointerCallback,
		p_btSerializer_serializeName serializeNameCallback, p_btSerializer_setSerializationFlags setSerializationFlagsCallback,
		p_btSerializer_startSerialization startSerializationCallback);

	virtual btChunk* allocate(size_t size, int numElements);
	virtual void finalizeChunk(btChunk* chunk, const char* structType, int chunkCode,
		void* oldPtr);
	virtual const char* findNameForPointer(const void* ptr) const;
	virtual void* findPointer(void* oldPtr);
	virtual void finishSerialization();
	virtual const unsigned char* getBufferPointer() const;
	virtual btChunk* getChunk(int chunkIndex) const;
	virtual int getCurrentBufferSize() const;
	virtual int getNumChunks() const;
	virtual int getSerializationFlags() const;
	virtual void* getUniquePointer(void* oldPtr);
	virtual void registerNameForPointer(const void* ptr, const char* name);
	virtual void serializeName(const char* ptr);
	virtual void setSerializationFlags(int flags);
	virtual void startSerialization();
};
#endif

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btChunk* btChunk_new();
	EXPORT int btChunk_getChunkCode(btChunk* obj);
	EXPORT int btChunk_getDna_nr(btChunk* obj);
	EXPORT int btChunk_getLength(btChunk* obj);
	EXPORT int btChunk_getNumber(btChunk* obj);
	EXPORT void* btChunk_getOldPtr(btChunk* obj);
	EXPORT void btChunk_setChunkCode(btChunk* obj, int value);
	EXPORT void btChunk_setDna_nr(btChunk* obj, int value);
	EXPORT void btChunk_setLength(btChunk* obj, int value);
	EXPORT void btChunk_setNumber(btChunk* obj, int value);
	EXPORT void btChunk_setOldPtr(btChunk* obj, void* value);
	EXPORT void btChunk_delete(btChunk* obj);

	EXPORT btSerializerWrapper* btSerializerWrapper_new(p_btSerializer_allocate allocateCallback,
		p_btSerializer_finalizeChunk finalizeChunkCallback, p_btSerializer_findNameForPointer findNameForPointerCallback,
		p_btSerializer_findPointer findPointerCallback, p_btSerializer_finishSerialization finishSerializationCallback,
		p_btSerializer_getBufferPointer getBufferPointerCallback, p_btSerializer_getChunk getChunkCallback,
		p_btSerializer_getCurrentBufferSize getCurrentBufferSizeCallback, p_btSerializer_getNumChunks getNumChunksCallback,
		p_btSerializer_getSerializationFlags getSerializationFlagsCallback, p_btSerializer_getUniquePointer getUniquePointerCallback,
		p_btSerializer_registerNameForPointer registerNameForPointerCallback, p_btSerializer_serializeName serializeNameCallback,
		p_btSerializer_setSerializationFlags setSerializationFlagsCallback, p_btSerializer_startSerialization startSerializationCallback);

	EXPORT void btSerializer_delete(btSerializer* obj);

	EXPORT btDefaultSerializer* btDefaultSerializer_new();
	EXPORT btDefaultSerializer* btDefaultSerializer_new2(int totalSize);
	EXPORT unsigned char* btDefaultSerializer_internalAlloc(btDefaultSerializer* obj, size_t size);
	EXPORT void btDefaultSerializer_writeHeader(btDefaultSerializer* obj, unsigned char* buffer);

	EXPORT char* getBulletDNAstr();
	EXPORT int getBulletDNAlen();
	EXPORT char* getBulletDNAstr64();
	EXPORT int getBulletDNAlen64();
#ifdef __cplusplus
}
#endif
