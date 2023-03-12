// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once
using namespace Ogre;

///////////////////////////////////////////////////////////////////////////////////////////////////

EXPORT bool OgreImageManager_loadFromFile( Root* root, wchar16* fileName, uint8** data, int* dataSize, 
	int* width, int* height, int* depth, PixelFormat* format, int* numFaces, int* numMipmaps,
	wchar16** error )
{
	*data = NULL;
	*width = 0;
	*height = 0;
	*depth = 0;
	*format = PF_UNKNOWN;
	*numFaces = 0;
	*numMipmaps = 0;
	*error = NULL;

	Image image;
	try
	{
		image.load(TO_WCHAR_T(fileName), ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
	}
	catch(Exception& ex)
	{
		*error = CreateOutString(ex.getFullDescription());
		return false;
	}
	catch(...)
	{
		*error = CreateOutString("Unknown exception");
		return false;
	}

	if( !image.getData() )
	{
		*error = CreateOutString("Unknown exception");
		return false;
	}

	*data = new uint8[ image.getSize() ];
	memcpy(*data, image.getData(), image.getSize());
	*dataSize = image.getSize();
	*width = image.getWidth();
	*height = image.getHeight();
	*depth = image.getDepth();
	*format = image.getFormat();
	*numFaces = image.getNumFaces();
	*numMipmaps = image.getNumMipmaps();

	return true;
}

EXPORT bool OgreImageManager_loadFromBuffer( Root* root, uint8* sourceBuffer, int sourceBufferSize, 
	char* fileType, uint8** data, int* dataSize, int* width, int* height, int* depth, 
	PixelFormat* format, int* numFaces, int* numMipmaps, wchar16** error )
{
	*data = NULL;
	*width = 0;
	*height = 0;
	*depth = 0;
	*format = PF_UNKNOWN;
	*numFaces = 0;
	*numMipmaps = 0;
	*error = NULL;

	Image image;
	try
	{
		DataStreamPtr stream(new MemoryDataStream(sourceBuffer, sourceBufferSize, false));
		image.load(stream, fileType);
	}
	catch(Exception& ex)
	{
		*error = CreateOutString(ex.getFullDescription());
		return false;
	}
	catch(...)
	{
		*error = CreateOutString("Unknown exception");
		return false;
	}

	if( !image.getData() )
	{
		*error = CreateOutString("Unknown exception");
		return false;
	}

	*data = new uint8[ image.getSize() ];
	memcpy(*data, image.getData(), image.getSize());
	*dataSize = image.getSize();
	*width = image.getWidth();
	*height = image.getHeight();
	*depth = image.getDepth();
	*format = image.getFormat();
	*numFaces = image.getNumFaces();
	*numMipmaps = image.getNumMipmaps();

	return true;
}

EXPORT void OgreImageManager_freeData( Root* root, uint8* data )
{
	delete[] data;
}

EXPORT bool OgreImageManager_save( Root* root, wchar16* fileName, uint8* data, int width,
	int height, int depth, PixelFormat format, int numFaces, int numMipmaps,
	wchar16** error )
{
	*error = NULL;

	Image image;
	try
	{
		image.loadDynamicImage( data, width, height, depth, format, false, numFaces, numMipmaps);
		image.save(TO_WCHAR_T(fileName));
	}
	catch(Exception& ex)
	{
		*error = CreateOutString(ex.getFullDescription());
		return false;
	}
	catch(...)
	{
		*error = CreateOutString("Unknown exception");
		return false;
	}

	return true;
}

EXPORT void OgreImageManager_scale( Root* root, uint8* data, int width, int height, PixelFormat format, 
	int newWidth, int newHeight, Image::Filter filter, uint8* newData )
{
	PixelBox src(width, height, 1, format, data);
	PixelBox dest(newWidth, newHeight, 1, format, newData);

	Image::scale(src, dest, filter);
}

EXPORT bool OgreImageManager_getImageFlags(Root* root, wchar16* fileName, uint* flags, wchar16** error)
{
	*error = NULL;

	try
	{
		*flags = Image::getFlagsFromHeader(TO_WCHAR_T(fileName), ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
	}
	catch (Exception& ex)
	{
		*error = CreateOutString(ex.getFullDescription());
		return false;
	}
	catch (...)
	{
		*error = CreateOutString("Unknown exception");
		return false;
	}

	return true;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
