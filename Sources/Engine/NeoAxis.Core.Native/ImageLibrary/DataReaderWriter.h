// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

#include "OgrePrerequisites.h"

namespace Ogre
{
	class DataWriter
	{
		uint8* data;
		int dataSize;
		int dataPosition;

		//

		void ExpandBuffer( int demandSize )
		{
			if( dataSize < demandSize )
			{
				int newSize = dataSize;
				while( newSize < demandSize )
					newSize *= 2;
				data = (uint8*)Memory_Realloc(MemoryAllocationType_Renderer, data, newSize, NULL, 0);
			}
		}

	public:

		DataWriter(int initialCapacity)
		{
			data = (uint8*)Memory_Alloc(MemoryAllocationType_Renderer, initialCapacity, NULL, 0);
			dataSize = initialCapacity;
			dataPosition = 0;
		}

		~DataWriter()
		{
			if(data)
			{
				Memory_Free(data);
				data = NULL;
			}
		}

		uint8* GetData()
		{
			return data;
		}

		int GetDataPosition()
		{
			return dataPosition;
		}

		void WriteBuffer( const void* source, int size )
		{
			int newSize = dataPosition + size;
			ExpandBuffer( newSize );
			memcpy(data + dataPosition, source, size);
			dataPosition = newSize;
		}

		void WriteByte(uint8 source )
		{
			WriteBuffer(&source, 1);
		}

		void WriteUInt32( uint source )
		{
			WriteBuffer(&source, 4);
		}

		int WriteVariableUInt32( uint source )
		{
			int retval = 1;
			uint num1 = (uint)source;
			while( num1 >= 0x80 )
			{
				WriteByte( (uint8)( num1 | 0x80 ) );
				num1 = num1 >> 7;
				retval++;
			}
			WriteByte( (uint8)num1 );
			return retval;
		}

		void WriteString( const char* source )
		{
			int length = strlen(source);
			WriteVariableUInt32(length + 1);
			WriteBuffer(source, length + 1);
		}
	};

	///////////////////////////////////////////////////////////////////////////////////////////////////

	class DataReader
	{
		uint8* data;
		int dataSize;
		int dataPosition;
		bool overflow;

	public:

		//

		DataReader(uint8* data, int dataSize )
		{
			this->data = data;
			this->dataSize = dataSize;
			this->dataPosition = 0;
			overflow = false;
		}

		uint8* GetData()
		{
			return data;
		}

		int GetDataSize()
		{
			return dataSize;
		}

		int GetDataPosition()
		{
			return dataPosition;
		}

		bool IsOverflow()
		{
			return overflow;
		}

		uint8* ReadBuffer( int length )
		{
			int newPosition = dataPosition + length;
			if( overflow || newPosition > dataSize )
			{
				overflow = true;
				return NULL;
			}
			uint8* result = data + dataPosition;
			dataPosition = newPosition;
			return result;
		}

		uint8 ReadByte()
		{
			uint8 result = *((uint8*)ReadBuffer(1));
			return result;
		}

		uint ReadUInt32()
		{
			uint result = *((uint*)ReadBuffer(4));
			return result;
		}

		uint ReadVariableUInt32()
		{
			if( overflow )
				return 0;

			int num1 = 0;
			int num2 = 0;
			while( true )
			{
				if( num2 == 0x23 )
				{
					overflow = true;
					return 0;
				}

				uint8 num3 = ReadByte();
				if( overflow )
					return 0;

				num1 |= ( num3 & 0x7f ) << ( num2 & 0x1f );
				num2 += 7;
				if( ( num3 & 0x80 ) == 0 )
					return (uint)num1;
			}
		}

		const char* ReadString()
		{
			int bufferSize = (int)ReadVariableUInt32();

			int newPosition = dataPosition + bufferSize;
			if( overflow || newPosition > dataSize )
			{
				overflow = true;
				return "";
			}

			const char* result = (const char*)(data + dataPosition);
			dataPosition = newPosition;
			return result;
		}
	};

}