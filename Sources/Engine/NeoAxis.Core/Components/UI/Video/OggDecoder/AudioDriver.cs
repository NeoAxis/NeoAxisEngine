// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using NeoAxis;


using NeoAxis.OggVorbisTheora;

namespace OggDecoder
{
	abstract class AudioDriver : IDisposable
	{
		internal OggFile oggFile;

		internal vorbis.Info vorbisInfo;
		vorbis.Comment vorbisComment;

		vorbis.DspState vorbisDSPState;
		vorbis.Block vorbisBlock;

		ogg.StreamState vorbisStreamState;

		protected const int soundBufferSize = 16384 * 2;
		internal int bufferSize;
		byte[] samplesBuffer = new byte[ 65536 * 2 ];

		ogg.Packet oggPacket;

		int headerCount;

		//

		public AudioDriver()
		{
			vorbisInfo = new vorbis.Info();
			vorbisComment = new vorbis.Comment();
			oggPacket = new ogg.Packet();
		}

		public abstract void StartAudioStream();

		public virtual void Dispose()
		{
			if( oggPacket != null )
			{
				oggPacket.Dispose();
				oggPacket = null;
			}

			if( vorbisDSPState != null )
			{
				vorbisDSPState.Dispose();
				vorbisDSPState = null;
			}

			if( vorbisBlock != null )
			{
				vorbisBlock.Dispose();
				vorbisBlock = null;
			}

			if( vorbisInfo != null )
			{
				vorbisInfo.Dispose();
				vorbisInfo = null;
			}

			if( vorbisComment != null )
			{
				vorbisComment.Dispose();
				vorbisComment = null;
			}

			if( vorbisStreamState != null )
			{
				vorbisStreamState.Dispose();
				vorbisStreamState = null;
			}
		}

		internal void InitDSP()
		{
			vorbisDSPState = new vorbis.DspState( vorbisInfo );
			//vorbisDSPState.synthesis_init( vorbisInfo );

			vorbisBlock = new vorbis.Block( vorbisDSPState );
			//vorbisBlock.init( vorbisDSPState );
		}

		internal void PageIn( ogg.Page page )
		{
			vorbisStreamState.pagein( page );
		}

		internal bool DecodePrimaryHeader( ogg.Packet p, ogg.StreamState s )
		{
			if( vorbisInfo.synthesis_headerin( vorbisComment, p ) >= 0 )
			{
				//This is vorbis stream, so store it
				vorbisStreamState = s;
				headerCount = 1;
				return true;
			}
			return false;
		}

		internal bool CheckSecondHeader( ogg.Packet p, ref bool needMoreData )
		{
			needMoreData = false;
			//We have all headers already, so just return
			if( headerCount == 3 )
				return false;

			int retVal = vorbisStreamState.packetout( p );
			if( retVal == 1 )
			{
				if( vorbisInfo.synthesis_headerin( vorbisComment, p ) != 0 )
					throw new Exception( "Invalid stream" );

				headerCount++;
				return true;
			}
			else if( retVal < 0 ) //error
				throw new Exception( "Invalid stream" );

			//Need more data
			needMoreData = true;
			return true;
		}

		unsafe internal bool Decode()
		{
			bool realSoundPlay = IsSoundRealPlaying();

			if( !realSoundPlay )
				bufferSize = 0;

			//get some audio data
			while( !IsBufferReady() )
			{
				float** pcm;
				int ret = vorbisDSPState.synthesis_pcmout( out pcm );

				//if there's pending, decoded audio, grab it
				if( ret > 0 && bufferSize < samplesBuffer.Length )
				{
					int j;

					if( realSoundPlay )
					{
						j = 0;

						int maxSamples = samplesBuffer.Length - bufferSize + vorbisInfo.channels * 2;

						for( j = 0; j < ret && j < maxSamples; j++ )
						{
							for( int i = 0; i < vorbisInfo.channels; i++ )
							{
								int val = (int)( pcm[ i ][ j ] * 32767.0f );
								if( val < -32768 )
									val = -32768;
								if( val > 32767 )
									val = 32767;

								if( val < 0 )
									val = val | 0x8000;

								samplesBuffer[ bufferSize ] = (byte)val;
								bufferSize++;
								samplesBuffer[ bufferSize ] = (byte)( val >> 8 );
								bufferSize++;
							}
						}
					}
					else
						j = ret;

					//tell libvorbis how many samples we actually consumed
					vorbisDSPState.synthesis_read( j );
				}
				else
				{
					//no pending audio; is there a pending packet to decode?
					if( vorbisStreamState.packetout( oggPacket ) > 0 )
					{
						//test for success!
						if( vorbisBlock.synthesis( oggPacket ) == 0 )
							vorbisDSPState.synthesis_blockin( vorbisBlock );
					}
					else    //we need more data; break out to suck in another page
						return false;

				}

				if( !realSoundPlay )
					break;
			}

			return true;
		}

		protected int ReadData( byte[] buffer, int bufferOffset, int length )
		{
			if( oggFile.EndOfFile )
				return 0;

			oggFile.criticalSection?.Enter();

			try
			{
				if( !IsSoundRealPlaying() )
					return 0;

				int sourceLength = length;
				if( oggFile.Sound3D && vorbisInfo.channels == 2 )
					sourceLength *= 2;

				//suspended
				//!!!!!норм EngineTime юзать?
				double time = EngineApp.EngineTime;
				if( time - oggFile.LastUpdateTime > .6f )
					return 0;

				//read from ogg
				while( bufferSize < sourceLength )
				{
					oggFile.Decode( true, false );
					if( oggFile.EndOfFile )
						break;
				}

				if( bufferSize == 0 )
					return 0;

				if( sourceLength > bufferSize )
					sourceLength = bufferSize;

				int returnLength = sourceLength;

				//copy to channel buffer
				if( oggFile.Sound3D && vorbisInfo.channels == 2 )
				{
					unsafe
					{
						fixed( byte* source = samplesBuffer, destination = buffer )
						{
							ushort* s = (ushort*)source;
							ushort* d = (ushort*)( destination + bufferOffset );
							for( int n = 0; n < sourceLength; n += 4 )
							{
								*d = *s;
								d++;
								s += 2;
							}
						}
					}

					returnLength = sourceLength / 2;
				}
				else
				{
					unsafe
					{
						fixed( byte* pSamplesBuffer = samplesBuffer, pBuffer = buffer )
							NativeUtility.CopyMemory( (IntPtr)( pBuffer + bufferOffset ), (IntPtr)pSamplesBuffer, sourceLength );
					}
					//Array.Copy( samplesBuffer, 0, buffer, bufferOffset, sourceLength );
				}

				//remove data from samplesBuffer
				{
					unsafe
					{
						fixed( byte* pSamplesBuffer = samplesBuffer )
							NativeUtility.CopyMemory( (IntPtr)pSamplesBuffer, (IntPtr)( pSamplesBuffer + sourceLength ), bufferSize - sourceLength );
					}
					//Array.Copy( samplesBuffer, sourceLength, samplesBuffer, 0, bufferSize - sourceLength );

					bufferSize -= sourceLength;
				}

				return returnLength;
			}
			catch( Exception ex )
			{
				Log.Error( "OggFile: Exception: " + ex.ToString() );
				return 0;
			}
			finally
			{
				oggFile.criticalSection?.Leave();
			}
		}

		public bool IsBufferReady()
		{
			return bufferSize >= soundBufferSize;
		}

		public virtual void OnSetPause( bool pause ) { }

		protected internal virtual void OnUpdateSoundPosition() { }

		public abstract bool IsSoundRealPlaying();

		internal float GetCurrentStateTime()
		{
			return (float)vorbisDSPState.granule_time( vorbisDSPState.granulepos );
		}

		internal virtual void OnSetVolume() { }

		public int GetRate()
		{
			if( vorbisInfo == null )
				return 0;
			return vorbisInfo.rate;
		}

		public int GetChannels()
		{
			if( vorbisInfo == null )
				return 0;
			return vorbisInfo.channels;
		}

	}
}
