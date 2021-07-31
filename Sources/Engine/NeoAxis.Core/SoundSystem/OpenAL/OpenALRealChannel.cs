// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NeoAxis;
using NeoAxis.OggVorbisTheora;
using Tao.OpenAl;

namespace OpenALSoundSystem
{
	sealed class OpenALRealChannel : SoundRealChannel
	{
		internal int alSource;

		internal OpenALSound currentSound;

		//stream (file stream, data stream)
		unsafe byte* streamBuffer;
		int streamBufferSize;
		bool streamDataAvailable;
		int[] streamAlDataBuffers;

		//file stream
		internal VorbisFile.File fileStreamVorbisFile;
		internal VorbisFileReader fileStreamVorbisFileReader;

		byte[] tempDataStreamReadArray = new byte[ 0 ];

		//

		public OpenALRealChannel()
		{
		}

		unsafe internal protected override void PostAttachVirtualChannel()
		{
			OpenALSoundWorld.criticalSection.Enter();

			currentSound = (OpenALSound)CurrentVirtualChannel.Sound;

			OpenALSampleSound sampleSound = currentSound as OpenALSampleSound;
			OpenALDataBufferSound streamSound = null;
			OpenALFileStreamSound fileStreamSound = null;
			OpenALDataStreamSound dataStreamSound = null;
			if( sampleSound == null )
			{
				streamSound = currentSound as OpenALDataBufferSound;
				fileStreamSound = currentSound as OpenALFileStreamSound;
				dataStreamSound = currentSound as OpenALDataStreamSound;
			}

			//create streamBuffer
			if( fileStreamSound != null )
			{
				var stream = OpenALSoundWorld.CreateFileStream2( currentSound.Name );
				if( stream == null )
				{
					//Log.Warning( string.Format( "Creating sound \"{0}\" failed.", currentSound.Name ) );
					PreDetachVirtualChannel();
					OpenALSoundWorld.criticalSection.Leave();
					return;
				}
				fileStreamVorbisFile = new VorbisFile.File();
				fileStreamVorbisFileReader = new VorbisFileReader( stream, true );
				if( !fileStreamVorbisFileReader.OpenVorbisFile( fileStreamVorbisFile ) )
				{
					//Log.Warning( string.Format( "Creating sound \"{0}\" failed.", currentSound.Name ) );
					PreDetachVirtualChannel();
					OpenALSoundWorld.criticalSection.Leave();
					return;
				}

				int numSamples = (int)fileStreamVorbisFile.pcm_total( -1 );
				fileStreamVorbisFile.get_info( -1, out var channels, out var frequency );

				//int numSamples = (int)fileStreamSound.vorbisFile.pcm_total( -1 );
				//fileStreamSound.vorbisFile.get_info( -1, out var channels, out var rate );

				if( fileStreamSound.needConvertToMono )
					channels = 1;

				int sizeInBytes = numSamples * channels * 2;

				int bufferSize = sizeInBytes / 2;
				if( bufferSize > 65536 * 4 )
					bufferSize = 65536 * 4;

				streamBufferSize = bufferSize;
				streamBuffer = (byte*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.SoundAndVideo, streamBufferSize );
			}

			if( dataStreamSound != null )
			{
				streamBufferSize = dataStreamSound.bufferSize;
				streamBuffer = (byte*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.SoundAndVideo, streamBufferSize );
			}

			//create al data buffers
			if( fileStreamSound != null || dataStreamSound != null )
			{
				if( streamAlDataBuffers == null )
				{
					streamAlDataBuffers = new int[ 2 ];
					fixed ( int* pAlDataBuffers = streamAlDataBuffers )
						Al.alGenBuffers( streamAlDataBuffers.Length, pAlDataBuffers );
					if( OpenALSoundWorld.CheckError() )
					{
						streamAlDataBuffers = null;
						PreDetachVirtualChannel();
						OpenALSoundWorld.criticalSection.Leave();
						return;
					}
				}
			}

			//init source

			bool mode3d = ( currentSound.Mode & SoundModes.Mode3D ) != 0;
			bool loop = ( currentSound.Mode & SoundModes.Loop ) != 0;

			if( alSource == 0 )
			{
				Al.alGenSources( 1, out alSource );
				if( OpenALSoundWorld.CheckError() )
				{
					PreDetachVirtualChannel();
					OpenALSoundWorld.criticalSection.Leave();
					return;
				}
			}


			if( sampleSound != null )
			{
				//no stream sound
				Al.alSourcei( alSource, Al.AL_BUFFER, sampleSound.alBuffer );
				if( OpenALSoundWorld.CheckError() )
				{
					PreDetachVirtualChannel();
					OpenALSoundWorld.criticalSection.Leave();
					return;
				}
			}

			if( fileStreamSound != null )
				FileStreamStartPlay();

			if( dataStreamSound != null )
				DataStreamStartPlay();

			Al.alSourcei( alSource, Al.AL_SOURCE_RELATIVE, mode3d ? Al.AL_FALSE : Al.AL_TRUE );

			//update parameters
			if( mode3d )
			{
				UpdatePosition2();
				UpdateVelocity2();
			}
			else
				UpdatePan2();
			UpdatePitch2();
			UpdateVolume2();

			if( sampleSound != null )
				Al.alSourcei( alSource, Al.AL_LOOPING, loop ? Al.AL_TRUE : Al.AL_FALSE );
			else
				Al.alSourcei( alSource, Al.AL_LOOPING, Al.AL_FALSE );
			if( OpenALSoundWorld.CheckError() )
			{
				PreDetachVirtualChannel();
				OpenALSoundWorld.criticalSection.Leave();
				return;
			}

			UpdateTime2();

			//unpause
			Al.alSourcePlay( alSource );
			OpenALSoundWorld.CheckError();

			//add to fileStreamChannels
			if( fileStreamSound != null )
				OpenALSoundWorld.fileStreamRealChannels.Add( this );

			OpenALSoundWorld.criticalSection.Leave();
		}

		unsafe internal protected override void PreDetachVirtualChannel()
		{
			OpenALSoundWorld.criticalSection.Enter();

			Al.alSourceStop( alSource );
			OpenALSoundWorld.CheckError();

			//never delete buffer because cannot delete. maybe need some time to free buffer internally
			////delete al data buffers
			//if( streamAlDataBuffers != null )
			//{
			//	fixed ( int* pAlDataBuffers = streamAlDataBuffers )
			//		Al.alDeleteBuffers( streamAlDataBuffers.Length, pAlDataBuffers );
			//	OpenALSoundWorld.CheckError();
			//	streamAlDataBuffers = null;
			//}

			if( currentSound is OpenALDataBufferSound )
			{
				if( currentSound is OpenALFileStreamSound )
					OpenALSoundWorld.fileStreamRealChannels.Remove( this );

				if( streamBuffer != null )
				{
					NativeUtility.Free( (IntPtr)streamBuffer );
					streamBuffer = null;
					streamBufferSize = 0;
				}
			}

			fileStreamVorbisFile?.Dispose();
			fileStreamVorbisFile = null;
			fileStreamVorbisFileReader?.Dispose();
			fileStreamVorbisFileReader = null;

			Al.alSourcei( alSource, Al.AL_BUFFER, 0 );
			OpenALSoundWorld.CheckError();

			currentSound = null;

			OpenALSoundWorld.criticalSection.Leave();
		}

		void UpdatePosition2()
		{
			Vector3 value = CurrentVirtualChannel.Position;

			//!!!!!double
			Al.alSource3f( alSource, Al.AL_POSITION, (float)value.X, (float)value.Y, (float)value.Z );
			OpenALSoundWorld.CheckError();
		}

		internal protected override void UpdatePosition()
		{
			OpenALSoundWorld.criticalSection.Enter();
			UpdatePosition2();
			OpenALSoundWorld.criticalSection.Leave();
		}

		void UpdateVelocity2()
		{
			Vector3 value = CurrentVirtualChannel.Velocity;

			//!!!!double
			Al.alSource3f( alSource, Al.AL_VELOCITY, (float)value.X, (float)value.Y, (float)value.Z );
			OpenALSoundWorld.CheckError();
		}

		internal protected override void UpdateVelocity()
		{
			OpenALSoundWorld.criticalSection.Enter();
			UpdateVelocity2();
			OpenALSoundWorld.criticalSection.Leave();
		}

		void UpdateVolume2()
		{
			double value = CurrentVirtualChannel.GetTotalVolume() * CurrentVirtualChannel.GetRolloffFactor();
			Al.alSourcef( alSource, Al.AL_GAIN, (float)value );
			OpenALSoundWorld.CheckError();
		}

		internal protected override void UpdateVolume()
		{
			OpenALSoundWorld.criticalSection.Enter();
			UpdateVolume2();
			OpenALSoundWorld.criticalSection.Leave();
		}

		void UpdatePitch2()
		{
			Al.alSourcef( alSource, Al.AL_PITCH, (float)CurrentVirtualChannel.GetTotalPitch() );
			OpenALSoundWorld.CheckError();
		}

		internal protected override void UpdatePitch()
		{
			OpenALSoundWorld.criticalSection.Enter();
			UpdatePitch2();
			OpenALSoundWorld.criticalSection.Leave();
		}

		void UpdatePan2()
		{
			float value = (float)CurrentVirtualChannel.Pan;
			MathEx.Clamp( ref value, -1, 1 );
			Al.alSource3f( alSource, Al.AL_POSITION, value * .1f, 0, 0 );
			OpenALSoundWorld.CheckError();
		}

		internal protected override void UpdatePan()
		{
			OpenALSoundWorld.criticalSection.Enter();
			UpdatePan2();
			OpenALSoundWorld.criticalSection.Leave();
		}

		void UpdateTime2()
		{
			Al.alSourcef( alSource, Al.AL_SEC_OFFSET, (float)CurrentVirtualChannel.Time );
			OpenALSoundWorld.CheckError();
		}

		internal protected override void UpdateTime()
		{
			OpenALSoundWorld.criticalSection.Enter();
			UpdateTime2();
			OpenALSoundWorld.criticalSection.Leave();
		}

		public void Update()
		{
			if( currentSound == null )
				return;

			if( ( currentSound.Mode & SoundModes.Mode3D ) != 0 )
				UpdateVolume2();

			if( currentSound is OpenALSampleSound )
			{
				UpdateSample();
				return;
			}

			if( currentSound is OpenALDataStreamSound )
				UpdateDataStream();
		}

		void UpdateSample()
		{
			int state;
			Al.alGetSourcei( alSource, Al.AL_SOURCE_STATE, out state );
			OpenALSoundWorld.CheckError();
			if( state == Al.AL_STOPPED )
				CurrentVirtualChannel.Stop();
		}

		int ReadDataFromDataStream( IntPtr buffer, int needRead )
		{
			OpenALDataStreamSound currentDataStreamSound = (OpenALDataStreamSound)currentSound;

			if( tempDataStreamReadArray.Length < needRead )
				tempDataStreamReadArray = new byte[ needRead ];

			int readed = currentDataStreamSound.dataReadCallback( tempDataStreamReadArray, 0, needRead );
			if( readed != 0 )
				Marshal.Copy( tempDataStreamReadArray, 0, buffer, readed );

			if( readed < 16 )
			{
				readed = Math.Min( needRead, 16 );
				NativeUtility.ZeroMemory( buffer, readed );
			}

			return readed;
		}

		unsafe void DataStreamStartPlay()
		{
			for( int n = 0; n < streamAlDataBuffers.Length; n++ )
			{
				int readed = ReadDataFromDataStream( (IntPtr)streamBuffer, streamBufferSize );

				int alFormat = ( currentSound.channels == 1 ) ? Al.AL_FORMAT_MONO16 : Al.AL_FORMAT_STEREO16;
				Al.alBufferData( streamAlDataBuffers[ n ], alFormat, streamBuffer, readed, currentSound.frequency );
			}

			fixed ( int* pAlDataBuffers = streamAlDataBuffers )
				Al.alSourceQueueBuffers( alSource, streamAlDataBuffers.Length, pAlDataBuffers );
		}

		unsafe void UpdateDataStream()
		{
			OpenALDataStreamSound dataStreamSound = (OpenALDataStreamSound)currentSound;

			int alFormat = ( currentSound.channels == 1 ) ? Al.AL_FORMAT_MONO16 : Al.AL_FORMAT_STEREO16;

			int processed;

			Al.alGetSourcei( alSource, Al.AL_BUFFERS_PROCESSED, out processed );
			OpenALSoundWorld.CheckError();

			while( processed != 0 )
			{
				int alBuffer = 0;

				int readed = ReadDataFromDataStream( (IntPtr)streamBuffer, streamBufferSize );

				Al.alSourceUnqueueBuffers( alSource, 1, ref alBuffer );
				OpenALSoundWorld.CheckError();

				Al.alBufferData( alBuffer, alFormat, streamBuffer, readed, currentSound.frequency );
				OpenALSoundWorld.CheckError();

				Al.alSourceQueueBuffers( alSource, 1, ref alBuffer );
				OpenALSoundWorld.CheckError();

				processed--;
			}

			int state;
			Al.alGetSourcei( alSource, Al.AL_SOURCE_STATE, out state );
			OpenALSoundWorld.CheckError();

			if( state != Al.AL_PLAYING )
			{
				Al.alGetSourcei( alSource, Al.AL_BUFFERS_PROCESSED, out processed );
				Al.alSourcePlay( alSource );
			}
		}

		public void UpdateFileStreamFromThread()
		{
			if( currentSound == null )
				return;

			//update buffers
			int processed;
			Al.alGetSourcei( alSource, Al.AL_BUFFERS_PROCESSED, out processed );
			OpenALSoundWorld.CheckError();
			while( processed != 0 )
			{
				int alStreamBuffer = 0;
				Al.alSourceUnqueueBuffers( alSource, 1, ref alStreamBuffer );

				OpenALSoundWorld.CheckError();
				FileStream( alStreamBuffer );
				if( streamDataAvailable )
				{
					Al.alSourceQueueBuffers( alSource, 1, ref alStreamBuffer );
					OpenALSoundWorld.CheckError();
				}

				processed--;
			}

			//play if buffer stopped (from behind internal buffers processed)
			int state;
			Al.alGetSourcei( alSource, Al.AL_SOURCE_STATE, out state );
			OpenALSoundWorld.CheckError();
			bool stoppedNoQueued = false;
			if( state == Al.AL_STOPPED )
			{
				int queued;
				Al.alGetSourcei( alSource, Al.AL_BUFFERS_QUEUED, out queued );
				if( queued != 0 )
					Al.alSourcePlay( alSource );
				else
					stoppedNoQueued = true;
			}

			//file stream played

			OpenALFileStreamSound fileStreamSound = (OpenALFileStreamSound)currentSound;

			if( !streamDataAvailable && stoppedNoQueued )
			{
				if( ( currentSound.Mode & SoundModes.Loop ) != 0 )
				{
					//loop play. we need recreate vorbis file

					//stop and unqueues sources
					Al.alSourceStop( alSource );
					Al.alGetSourcei( alSource, Al.AL_BUFFERS_PROCESSED, out processed );
					OpenALSoundWorld.CheckError();
					while( processed != 0 )
					{
						int alStreamBuffer = 0;
						Al.alSourceUnqueueBuffers( alSource, 1, ref alStreamBuffer );
						OpenALSoundWorld.CheckError();
						processed--;
					}

					//recreate vorbis file
					{
						fileStreamVorbisFile?.Dispose();
						fileStreamVorbisFile = null;

						fileStreamVorbisFileReader.RewindStreamToBegin();

						fileStreamVorbisFile = new VorbisFile.File();
						if( !fileStreamVorbisFileReader.OpenVorbisFile( fileStreamVorbisFile ) )
						{
							Log.Warning( "OpenALSoundSystem: Creating sound failed \"{0}\".", currentSound.Name );
							return;
						}
					}
					//fileStreamSound.Rewind();

					FileStreamStartPlay();

					//Pause = false;
					Al.alSourcePlay( alSource );
					OpenALSoundWorld.CheckError();
				}
				else
					CurrentVirtualChannel.Stop();
			}
		}

		void FileStreamStartPlay()
		{
			for( int n = 0; n < streamAlDataBuffers.Length; n++ )
			{
				if( !FileStream( streamAlDataBuffers[ n ] ) )
					return;

				Al.alSourceQueueBuffers( alSource, 1, ref streamAlDataBuffers[ n ] );
				OpenALSoundWorld.CheckError();
			}
		}

		unsafe bool FileStream( int alStreamBuffer )
		{
			OpenALFileStreamSound fileStreamSound = (OpenALFileStreamSound)currentSound;

			int size = 0;

			streamDataAvailable = true;

			while( size < streamBufferSize )
			{
				byte* pointer = streamBuffer + size;

				int readBytes = fileStreamVorbisFile.read( (IntPtr)pointer, streamBufferSize - size, 0, 2, 1, IntPtr.Zero );

				//convert to mono for 3D
				if( readBytes > 0 && fileStreamSound.needConvertToMono )
				{
					readBytes /= 2;
					for( int n = 0; n < readBytes; n += 2 )
					{
						pointer[ n + 0 ] = pointer[ n * 2 + 0 ];
						pointer[ n + 1 ] = pointer[ n * 2 + 1 ];
					}
				}

				if( readBytes > 0 )
					size += readBytes;
				else
					break;
			}

			if( size == 0 )
			{
				streamDataAvailable = false;
				return false;
			}

			int alFormat = ( currentSound.channels == 1 ) ? Al.AL_FORMAT_MONO16 : Al.AL_FORMAT_STEREO16;
			Al.alBufferData( alStreamBuffer, alFormat, streamBuffer, size, currentSound.frequency );
			OpenALSoundWorld.CheckError();

			return true;
		}
	}
}
