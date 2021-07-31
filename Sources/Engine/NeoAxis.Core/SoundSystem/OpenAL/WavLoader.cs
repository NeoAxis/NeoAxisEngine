// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpenALSoundSystem
{
	class WavLoader
	{
		static bool StreamReadUint( Stream stream, out uint value )
		{
			//!!!!!slowly
			int byte0 = stream.ReadByte();
			int byte1 = stream.ReadByte();
			int byte2 = stream.ReadByte();
			int byte3 = stream.ReadByte();
			int v = byte3 * ( 1 << 24 ) + byte2 * ( 1 << 16 ) + byte1 * ( 1 << 8 ) + byte0;
			value = (uint)v;
			return byte3 != -1;
		}

		static bool StreamReadUshort( Stream stream, out ushort value )
		{
			//!!!!!slowly
			int byte0 = stream.ReadByte();
			int byte1 = stream.ReadByte();
			int v = byte1 * ( 1 << 8 ) + byte0;
			value = (ushort)v;
			return byte1 != -1;
		}

		enum WavTag
		{
			RIFF = 0x46464952,
			WAVE = 0x45564157,
			FMT = 0x20746D66,
			DATA = 0x61746164,
		};

		public static bool Load( Stream stream, out int channels,
			out int frequency, out byte[] samples, out int sizeInBytes,
			out string error )
		{
			channels = 0;
			frequency = 0;
			samples = null;
			sizeInBytes = 0;
			error = null;

			uint magic;
			uint length;

			StreamReadUint( stream, out magic );
			StreamReadUint( stream, out length );
			if( magic != (uint)WavTag.RIFF )
			{
				error = "Invalid format";
				return false;
			}

			StreamReadUint( stream, out magic );
			if( magic != (uint)WavTag.WAVE )
			{
				error = "Unknown file type";
				return false;
			}

			ushort wavEncoding = 0;
			ushort wavChannels = 0;
			uint wavFrequency = 0;
			uint wavByteRate = 0;
			ushort wavBlockAlign = 0;
			ushort wavBitsPerSample = 0;

			while( true )
			{
				if( !StreamReadUint( stream, out magic ) )
					break;
				if( !StreamReadUint( stream, out length ) )
					break;

				if( magic == (uint)WavTag.FMT )
				{
					StreamReadUshort( stream, out wavEncoding );
					StreamReadUshort( stream, out wavChannels );
					StreamReadUint( stream, out wavFrequency );
					StreamReadUint( stream, out wavByteRate );
					StreamReadUshort( stream, out wavBlockAlign );
					StreamReadUshort( stream, out wavBitsPerSample );

					if( wavEncoding != 1 )
					{
						error = "Cannot load compressed wave file";
						return false;
					}

					if( wavBitsPerSample != 16 )
					{
						error = string.Format( "Can`t open {0} bit per sample format", wavBitsPerSample );
						return false;
					}
				}
				else if( magic == (uint)WavTag.DATA )
				{
					sizeInBytes = (int)length;
					samples = new byte[ sizeInBytes ];

					if( stream.Read( samples, 0, sizeInBytes ) != sizeInBytes )
					{
						error = "Invalid format";
						return false;
					}
				}
				else
				{
					stream.Seek( length, SeekOrigin.Current );
				}
			}

			if( samples == null )
			{
				error = "Invalid format";
				return false;
			}

			channels = wavChannels;
			frequency = (int)wavFrequency;

			return true;
		}
	}
}
