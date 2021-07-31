// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;

namespace NeoAxis
{
	/// <summary>
	/// Class for calculation CRC32 checksums.
	/// </summary>
	public class CRC32 : HashAlgorithm
	{
		const uint AllOnes = 0xffffffff;
		[ThreadStatic]
		static Hashtable cachedCRC32Tables;
		[ThreadStatic]
		static bool autoCache;

		uint[] crc32Table;
		uint m_crc;

		/// <summary>
		/// Returns the default polynomial
		/// </summary>
		public static uint DefaultPolynomial
		{
			get { return 0x04C11DB7; }
		}

		/// <summary>
		/// Gets or sets the auto-cache setting of this class.
		/// </summary>
		public static bool AutoCache
		{
			get { return autoCache; }
			set { autoCache = value; }
		}

		/// <summary>
		/// Initialize the cache
		/// </summary>
		static CRC32()
		{
			cachedCRC32Tables = Hashtable.Synchronized( new Hashtable() );
			autoCache = true;
		}

		public static void ClearCache()
		{
			cachedCRC32Tables.Clear();
		}


		/// <summary>
		/// Builds a crc32 table given a polynomial
		/// </summary>
		/// <param name="polynomial"></param>
		/// <returns></returns>
		protected static uint[] BuildCRC32Table( uint polynomial )
		{
			//changes (for standard crc32 compability)

			// 256 entries, one for each possible byte value
			const int length = 256;
			uint[] table = new uint[ length ];
			for( int i = 0; i < length; i++ )
			{
				uint r = Reflect( (uint)i, 8 ) << 24;

				for( int j = 8; j > 0; j-- )
				{
					const uint topbit = (uint)1 << 31;
					if( ( r & topbit ) != 0 )
						r = ( r << 1 ) ^ polynomial;
					else
						r <<= 1;
				}

				table[ i ] = Reflect( r, 32 );
			}
			return table;
		}

		//changes (for standard crc32 compability)
		protected static uint Reflect( uint value, int count )
		{
			uint t = value;
			for( int i = 0; i < count; i++ )
			{
				if( ( t & 1 ) != 0 )
					value |= (uint)( 1 << ( ( count - 1 ) - i ) );
				else
					value &= (uint)( ~( 1 << ( ( count - 1 ) - i ) ) );
				t >>= 1;
			}
			return value;
		}


		/// <summary>
		/// Creates a CRC32 object using the DefaultPolynomial
		/// </summary>
		public CRC32()
			: this( DefaultPolynomial )
		{
		}

		/// <summary>
		/// Creates a CRC32 object using the specified Creates a CRC32 object 
		/// </summary>
		public CRC32( uint polynomial )
			: this( polynomial, CRC32.AutoCache )
		{
		}

		/// <summary>
		/// Construct the 
		/// </summary>
		public CRC32( uint polynomial, bool cacheTable )
		{
			this.HashSizeValue = 32;

			crc32Table = (uint[])cachedCRC32Tables[ polynomial ];
			if( crc32Table == null )
			{
				crc32Table = CRC32.BuildCRC32Table( polynomial );
				if( cacheTable )
					cachedCRC32Tables.Add( polynomial, crc32Table );
			}
			Initialize();
		}

		/// <summary>
		/// Initializes an implementation of HashAlgorithm.
		/// </summary>
		public override void Initialize()
		{
			m_crc = AllOnes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		protected override void HashCore( byte[] buffer, int offset, int count )
		{
			// Save the text in the buffer. 
			for( int i = offset; i < count; i++ )
			{
				ulong tabPtr = ( m_crc & 0xFF ) ^ buffer[ i ];
				m_crc >>= 8;
				m_crc ^= crc32Table[ tabPtr ];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override byte[] HashFinal()
		{
			byte[] finalHash = new byte[ 4 ];
			ulong finalCRC = m_crc ^ AllOnes;

			finalHash[ 0 ] = (byte)( ( finalCRC >> 24 ) & 0xFF );
			finalHash[ 1 ] = (byte)( ( finalCRC >> 16 ) & 0xFF );
			finalHash[ 2 ] = (byte)( ( finalCRC >> 8 ) & 0xFF );
			finalHash[ 3 ] = (byte)( ( finalCRC >> 0 ) & 0xFF );

			return finalHash;
		}

		/// <summary>
		/// Computes the hash value for the specified Stream.
		/// </summary>
		new public byte[] ComputeHash( Stream inputStream )
		{
			byte[] buffer = new byte[ 4096 ];
			int bytesRead;
			while( ( bytesRead = inputStream.Read( buffer, 0, 4096 ) ) > 0 )
			{
				HashCore( buffer, 0, bytesRead );
			}
			return HashFinal();
		}


		/// <summary>
		/// Overloaded. Computes the hash value for the input data.
		/// </summary>
		new public byte[] ComputeHash( byte[] buffer )
		{
			return ComputeHash( buffer, 0, buffer.Length );
		}

		/// <summary>
		/// Overloaded. Computes the hash value for the input data.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		new public byte[] ComputeHash( byte[] buffer, int offset, int count )
		{
			HashCore( buffer, offset, count );
			return HashFinal();
		}
	}
}
