#include <LinearMath/btSerializer.h>

#include "btSerializer_wrap.h"

btSerializerWrapper::btSerializerWrapper(p_btSerializer_allocate allocateCallback,
	p_btSerializer_finalizeChunk finalizeChunkCallback, p_btSerializer_findNameForPointer findNameForPointerCallback,
	p_btSerializer_findPointer findPointerCallback, p_btSerializer_finishSerialization finishSerializationCallback,
	p_btSerializer_getBufferPointer getBufferPointerCallback, p_btSerializer_getChunk getChunkCallback,
	p_btSerializer_getCurrentBufferSize getCurrentBufferSizeCallback, p_btSerializer_getNumChunks getNumChunksCallback,
	p_btSerializer_getSerializationFlags getSerializationFlagsCallback, p_btSerializer_getUniquePointer getUniquePointerCallback,
	p_btSerializer_registerNameForPointer registerNameForPointerCallback, p_btSerializer_serializeName serializeNameCallback,
	p_btSerializer_setSerializationFlags setSerializationFlagsCallback, p_btSerializer_startSerialization startSerializationCallback)
{
	_allocateCallback = allocateCallback;
	_finalizeChunkCallback = finalizeChunkCallback;
	_findNameForPointerCallback = findNameForPointerCallback;
	_findPointerCallback = findPointerCallback;
	_finishSerializationCallback = finishSerializationCallback;
	_getBufferPointerCallback = getBufferPointerCallback;
	_getChunkCallback = getChunkCallback;
	_getCurrentBufferSizeCallback = getCurrentBufferSizeCallback;
	_getNumChunksCallback = getNumChunksCallback;
	_getSerializationFlagsCallback = getSerializationFlagsCallback;
	_getUniquePointerCallback = getUniquePointerCallback;
	_registerNameForPointerCallback = registerNameForPointerCallback;
	_serializeNameCallback = serializeNameCallback;
	_setSerializationFlagsCallback = setSerializationFlagsCallback;
	_startSerializationCallback = startSerializationCallback;
}

btChunk* btSerializerWrapper::allocate(size_t size, int numElements)
{
	return _allocateCallback(size, numElements);
}

void btSerializerWrapper::finalizeChunk(btChunk* chunk, const char* structType, int chunkCode,
	void* oldPtr)
{
	_finalizeChunkCallback(chunk, structType, chunkCode, oldPtr);
}

const char* btSerializerWrapper::findNameForPointer(const void* ptr) const
{
	return _findNameForPointerCallback(ptr);
}

void* btSerializerWrapper::findPointer(void* oldPtr)
{
	return _findPointerCallback(oldPtr);
}

void btSerializerWrapper::finishSerialization()
{
	_finishSerializationCallback();
}

const unsigned char* btSerializerWrapper::getBufferPointer()  const
{
	return _getBufferPointerCallback();
}

btChunk* btSerializerWrapper::getChunk(int chunkIndex) const
{
	return _getChunkCallback(chunkIndex);
}

int btSerializerWrapper::getCurrentBufferSize() const
{
	return _getCurrentBufferSizeCallback();
}

int btSerializerWrapper::getNumChunks() const
{
	return _getNumChunksCallback();
}

int btSerializerWrapper::getSerializationFlags() const
{
	return _getSerializationFlagsCallback();
}

void* btSerializerWrapper::getUniquePointer(void* oldPtr)
{
	return _getUniquePointerCallback(oldPtr);
}

void btSerializerWrapper::registerNameForPointer(const void* ptr, const char* name)
{
	_registerNameForPointerCallback(ptr, name);
}

void btSerializerWrapper::serializeName(const char* ptr)
{
	_serializeNameCallback(ptr);
}

void btSerializerWrapper::setSerializationFlags(int flags)
{
	_setSerializationFlagsCallback(flags);
}

void btSerializerWrapper::startSerialization()
{
	_startSerializationCallback();
}


btChunk* btChunk_new()
{
	return new btChunk();
}

int btChunk_getChunkCode(btChunk* obj)
{
	return obj->m_chunkCode;
}

int btChunk_getDna_nr(btChunk* obj)
{
	return obj->m_dna_nr;
}

int btChunk_getLength(btChunk* obj)
{
	return obj->m_length;
}

int btChunk_getNumber(btChunk* obj)
{
	return obj->m_number;
}

void* btChunk_getOldPtr(btChunk* obj)
{
	return obj->m_oldPtr;
}

void btChunk_setChunkCode(btChunk* obj, int value)
{
	obj->m_chunkCode = value;
}

void btChunk_setDna_nr(btChunk* obj, int value)
{
	obj->m_dna_nr = value;
}

void btChunk_setLength(btChunk* obj, int value)
{
	obj->m_length = value;
}

void btChunk_setNumber(btChunk* obj, int value)
{
	obj->m_number = value;
}

void btChunk_setOldPtr(btChunk* obj, void* value)
{
	obj->m_oldPtr = value;
}

void btChunk_delete(btChunk* obj)
{
	delete obj;
}


btSerializerWrapper* btSerializerWrapper_new(p_btSerializer_allocate allocateCallback,
	p_btSerializer_finalizeChunk finalizeChunkCallback, p_btSerializer_findNameForPointer findNameForPointerCallback,
	p_btSerializer_findPointer findPointerCallback, p_btSerializer_finishSerialization finishSerializationCallback,
	p_btSerializer_getBufferPointer getBufferPointerCallback, p_btSerializer_getChunk getChunkCallback,
	p_btSerializer_getCurrentBufferSize getCurrentBufferSizeCallback, p_btSerializer_getNumChunks getNumChunksCallback,
	p_btSerializer_getSerializationFlags getSerializationFlagsCallback, p_btSerializer_getUniquePointer getUniquePointerCallback,
	p_btSerializer_registerNameForPointer registerNameForPointerCallback, p_btSerializer_serializeName serializeNameCallback,
	p_btSerializer_setSerializationFlags setSerializationFlagsCallback, p_btSerializer_startSerialization startSerializationCallback)
{
	return new btSerializerWrapper(allocateCallback, finalizeChunkCallback, findNameForPointerCallback,
		findPointerCallback, finishSerializationCallback, getBufferPointerCallback,
		getChunkCallback, getCurrentBufferSizeCallback, getNumChunksCallback, getSerializationFlagsCallback,
		getUniquePointerCallback, registerNameForPointerCallback, serializeNameCallback,
		setSerializationFlagsCallback, startSerializationCallback);
}


btChunk* btSerializer_allocate(btSerializer* obj, int size, int numElements)
{
	return obj->allocate(size, numElements);
}

void btSerializer_finalizeChunk(btSerializer* obj, btChunk* chunk, const char* structType,
	int chunkCode, void* oldPtr)
{
	obj->finalizeChunk(chunk, structType, chunkCode, oldPtr);
}

const char* btSerializer_findNameForPointer(btSerializer* obj, const void* ptr)
{
	return obj->findNameForPointer(ptr);
}

void* btSerializer_findPointer(btSerializer* obj, void* oldPtr)
{
	return obj->findPointer(oldPtr);
}

void btSerializer_finishSerialization(btSerializer* obj)
{
	obj->finishSerialization();
}

const unsigned char* btSerializer_getBufferPointer(btSerializer* obj)
{
	return obj->getBufferPointer();
}

const btChunk* btSerializer_getChunk(btSerializer* obj, int chunkIndex)
{
	return obj->getChunk(chunkIndex);
}

int btSerializer_getCurrentBufferSize(btSerializer* obj)
{
	return obj->getCurrentBufferSize();
}

int btSerializer_getNumChunks(btSerializer* obj)
{
	return obj->getNumChunks();
}

int btSerializer_getSerializationFlags(btSerializer* obj)
{
	return obj->getSerializationFlags();
}

void* btSerializer_getUniquePointer(btSerializer* obj, void* oldPtr)
{
	return obj->getUniquePointer(oldPtr);
}

void btSerializer_registerNameForPointer(btSerializer* obj, const void* ptr, const char* name)
{
	obj->registerNameForPointer(ptr, name);
}

void btSerializer_serializeName(btSerializer* obj, const char* ptr)
{
	obj->serializeName(ptr);
}

void btSerializer_setSerializationFlags(btSerializer* obj, int flags)
{
	obj->setSerializationFlags(flags);
}

void btSerializer_startSerialization(btSerializer* obj)
{
	obj->startSerialization();
}

void btSerializer_delete(btSerializer* obj)
{
	delete obj;
}


btDefaultSerializer* btDefaultSerializer_new()
{
	return new btDefaultSerializer();
}

btDefaultSerializer* btDefaultSerializer_new2(int totalSize)
{
	return new btDefaultSerializer(totalSize);
}

unsigned char* btDefaultSerializer_internalAlloc(btDefaultSerializer* obj, size_t size)
{
	return obj->internalAlloc(size);
}

void btDefaultSerializer_writeHeader(btDefaultSerializer* obj, unsigned char* buffer)
{
	obj->writeHeader(buffer);
}


char* getBulletDNAstr()
{
	return sBulletDNAstr;
}

int getBulletDNAlen()
{
	return sBulletDNAlen;
}

char* getBulletDNAstr64()
{
	return sBulletDNAstr64;
}

int getBulletDNAlen64()
{
	return sBulletDNAlen64;
}
