// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using NeoAxis;
using NeoAxis.OggVorbisTheora;
using Tao.OpenAl;

namespace OpenALSoundSystem
{
	abstract class OpenALSoundData : SoundData
	{
		internal int channels;
		internal int frequency;

		protected static SoundType GetSoundTypeByName( string name )
		{
			//get soundType from name
			string extension = Path.GetExtension( name );

			if( string.Compare( extension, ".ogg", true ) == 0 )
				return SoundType.OGG;
			else if( string.Compare( extension, ".wav", true ) == 0 )
				return SoundType.WAV;
			return SoundType.Unknown;
		}

		protected static SoundType GetSoundTypeByStream( VirtualFileStream stream )
		{
			byte[] buffer = new byte[ 4 ];

			if( stream.Read( buffer, 0, 4 ) != 4 )
				return SoundType.Unknown;

			stream.Seek( -4, SeekOrigin.Current );

			if( ( buffer[ 0 ] == 'o' || buffer[ 0 ] == 'O' ) &&
				( buffer[ 1 ] == 'g' || buffer[ 1 ] == 'g' ) &&
				( buffer[ 2 ] == 'g' || buffer[ 2 ] == 'g' ) )
			{
				return SoundType.OGG;
			}

			if( ( buffer[ 0 ] == 'r' || buffer[ 0 ] == 'R' ) &&
				( buffer[ 1 ] == 'i' || buffer[ 1 ] == 'I' ) &&
				( buffer[ 2 ] == 'f' || buffer[ 2 ] == 'F' ) &&
				( buffer[ 3 ] == 'f' || buffer[ 3 ] == 'F' ) )
			{
				return SoundType.WAV;
			}

			return SoundType.Unknown;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	sealed class OpenALSampleSoundData : OpenALSoundData
	{
		internal int alBuffer;

		unsafe public OpenALSampleSoundData( VirtualFileStream stream, SoundType soundType, string name, SoundModes mode, out bool initialized )
		{
			initialized = false;

			byte[] samples;
			int sizeInBytes;
			float timeLength;

			if( string.Compare( Path.GetExtension( name ), ".ogg", true ) == 0 )
			{
				//ogg

				VorbisFileReader vorbisFileReader = new VorbisFileReader( stream, false );
				VorbisFile.File vorbisFile = new VorbisFile.File();

				if( !vorbisFileReader.OpenVorbisFile( vorbisFile ) )
				{
					vorbisFile.Dispose();
					vorbisFileReader.Dispose();

					Log.Warning( "OpenALSoundSystem: Creating sound failed \"{0}\" (Reading failed).", name );
					return;
				}

				int numSamples = (int)vorbisFile.pcm_total( -1 );

				vorbisFile.get_info( -1, out channels, out frequency );
				timeLength = (float)vorbisFile.time_total( -1 );

				sizeInBytes = numSamples * channels * 2;
				samples = new byte[ sizeInBytes ];

				fixed ( byte* pSamples = samples )
				{
					int samplePos = 0;
					while( samplePos < sizeInBytes )
					{
						int readBytes = vorbisFile.read( (IntPtr)( pSamples + samplePos ), sizeInBytes - samplePos, 0, 2, 1, IntPtr.Zero );
						if( readBytes <= 0 )
							break;
						samplePos += readBytes;
					}
				}

				vorbisFile.Dispose();
				vorbisFileReader.Dispose();
			}
			else if( string.Compare( Path.GetExtension( name ), ".wav", true ) == 0 )
			{
				//wav

				string error;
				if( !WavLoader.Load( stream, out channels, out frequency, out samples, out sizeInBytes, out error ) )
				{
					Log.Warning( "OpenALSoundSystem: Creating sound failed \"{0}\" ({1}).", name, error );
					return;
				}

				timeLength = (float)( samples.Length / channels / 2 ) / (float)frequency;
			}
			else
			{
				Log.Warning( "OpenALSoundSystem: Creating sound failed \"{0}\" (Unknown file type).", name );
				return;
			}

			//create buffer

			Al.alGenBuffers( 1, out alBuffer );

			int alFormat = ( channels == 1 ) ? Al.AL_FORMAT_MONO16 : Al.AL_FORMAT_STEREO16;

			//bug fix: half volume mono 2D sounds
			//convert to stereo
			if( ( mode & SoundModes.Mode3D ) == 0 && alFormat == Al.AL_FORMAT_MONO16 )
			{
				byte[] stereoSamples = new byte[ sizeInBytes * 2 ];
				for( int n = 0; n < sizeInBytes; n += 2 )
				{
					stereoSamples[ n * 2 + 0 ] = samples[ n ];
					stereoSamples[ n * 2 + 1 ] = samples[ n + 1 ];
					stereoSamples[ n * 2 + 2 ] = samples[ n ];
					stereoSamples[ n * 2 + 3 ] = samples[ n + 1 ];
				}
				samples = stereoSamples;
				alFormat = Al.AL_FORMAT_STEREO16;
				sizeInBytes *= 2;
			}

			//convert to mono for 3D
			if( ( mode & SoundModes.Mode3D ) != 0 && channels == 2 )
			{
				byte[] oldSamples = samples;
				samples = new byte[ oldSamples.Length / 2 ];
				for( int n = 0; n < samples.Length; n += 2 )
				{
					samples[ n + 0 ] = oldSamples[ n * 2 + 0 ];
					samples[ n + 1 ] = oldSamples[ n * 2 + 1 ];
				}
				alFormat = Al.AL_FORMAT_MONO16;
				sizeInBytes /= 2;
			}

			fixed ( byte* pSamples = samples )
				Al.alBufferData( alBuffer, alFormat, pSamples, sizeInBytes, frequency );

			if( OpenALSoundWorld.CheckError() )
			{
				Log.Warning( "OpenALSoundSystem: Creating sound failed \"{0}\".", name );
				return;
			}

			Init( name, mode, timeLength, channels, frequency );
			initialized = true;
		}

		protected override void OnDispose()
		{
			OpenALSoundWorld.criticalSection.Enter();

			if( alBuffer != 0 )
			{
				Al.alDeleteBuffers( 1, ref alBuffer );
				OpenALSoundWorld.CheckError();
				alBuffer = 0;
			}

			OpenALSoundWorld.criticalSection.Leave();

			base.OnDispose();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	class OpenALDataBufferSoundData : OpenALSoundData
	{
		//internal int[] alDataBuffers;

		//

		//unsafe protected bool GenerateBuffers( int count )
		//{
		//	alDataBuffers = new int[ count ];
		//	fixed ( int* pAlDataBuffers = alDataBuffers )
		//		Al.alGenBuffers( alDataBuffers.Length, pAlDataBuffers );
		//	if( OpenALSoundWorld.CheckError() )
		//		return false;
		//	return true;
		//}

		//unsafe protected override void OnDispose()
		//{
		//	OpenALSoundWorld.criticalSection.Enter();

		//	if( alDataBuffers != null )
		//	{
		//		fixed ( int* pAlDataBuffers = alDataBuffers )
		//			Al.alDeleteBuffers( alDataBuffers.Length, pAlDataBuffers );
		//		OpenALSoundWorld.CheckError();
		//		alDataBuffers = null;
		//	}

		//	OpenALSoundWorld.criticalSection.Leave();

		//	base.OnDispose();
		//}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	sealed class OpenALFileStreamSoundData : OpenALDataBufferSoundData
	{
		//internal VorbisFile.File vorbisFile;
		//internal VorbisFileReader vorbisFileReader;
		internal bool needConvertToMono;

		//

		//public OpenALFileStreamSound( VirtualFileStream stream, bool closeStreamAfterReading, SoundType soundType, string name, SoundModes mode, out bool initialized )
		public OpenALFileStreamSoundData( VirtualFileStream stream, SoundType soundType, string name, SoundModes mode, out bool initialized )
		{
			initialized = false;

			if( soundType == SoundType.Unknown )
			{
				if( name != null )
					soundType = GetSoundTypeByName( name );
				else
					soundType = GetSoundTypeByStream( stream );
			}

			if( soundType != SoundType.OGG )
			{
				Log.Warning( string.Format( "Streaming is not supported for \"{0}\" files ({1}).", soundType, name ) );
				return;
			}

			//get sound properties
			var vorbisFile = new VorbisFile.File();
			var vorbisFileReader = new VorbisFileReader( stream, true );// closeStreamAfterReading );
			if( !vorbisFileReader.OpenVorbisFile( vorbisFile ) )
			{
				vorbisFileReader.Dispose();
				Log.Warning( string.Format( "Creating sound \"{0}\" failed.", name ) );
				return;
			}
			long numSamples = vorbisFile.pcm_total( -1 );
			vorbisFile.get_info( -1, out channels, out frequency );
			vorbisFile?.Dispose();
			vorbisFileReader?.Dispose();

			//vorbisFile = new VorbisFile.File();

			//vorbisFileReader = new VorbisFileReader( stream, closeStreamAfterReading );

			//if( !vorbisFileReader.OpenVorbisFile( vorbisFile ) )
			//{
			//	vorbisFileReader.Dispose();
			//	Log.Warning( string.Format( "Creating sound \"{0}\" failed.", name ) );
			//	return;
			//}

			//long numSamples = vorbisFile.pcm_total( -1 );
			//vorbisFile.get_info( -1, out channels, out frequency );

			//convert to mono for 3D
			if( ( mode & SoundModes.Mode3D ) != 0 && channels == 2 )
			{
				needConvertToMono = true;
				channels = 1;
			}

			//if( !GenerateBuffers( 2 ) )
			//{
			//	Log.Warning( "OpenALSoundSystem: Creating sound failed \"{0}\".", name );
			//	return;
			//}

			double length = (double)numSamples / (double)frequency;

			Init( name, mode, (float)length, channels, frequency );
			initialized = true;
		}

		protected override void OnDispose()
		{
			//OpenALSoundWorld.criticalSection.Enter();

			//if( vorbisFile != null )
			//{
			//	vorbisFile.Dispose();
			//	vorbisFile = null;
			//}

			//if( vorbisFileReader != null )
			//{
			//	vorbisFileReader.Dispose();
			//	vorbisFileReader = null;
			//}

			//OpenALSoundWorld.criticalSection.Leave();

			base.OnDispose();
		}

		//public void Rewind()
		//{
		//	if( vorbisFile != null )
		//	{
		//		vorbisFile.Dispose();
		//		vorbisFile = null;
		//	}

		//	vorbisFileReader.RewindStreamToBegin();

		//	vorbisFile = new VorbisFile.File();
		//	if( !vorbisFileReader.OpenVorbisFile( vorbisFile ) )
		//	{
		//		Log.Warning( "OpenALSoundSystem: Creating sound failed \"{0}\".", Name );
		//		return;
		//	}
		//}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	sealed class OpenALDataStreamSoundData : OpenALDataBufferSoundData
	{
		internal SoundWorld.DataReadDelegate dataReadCallback;
		internal int bufferSize;

		//

		public OpenALDataStreamSoundData( SoundModes mode, int channels, int frequency, int bufferSize, SoundWorld.DataReadDelegate dataReadCallback )
		{
			this.channels = channels;
			this.frequency = frequency;
			this.dataReadCallback = dataReadCallback;
			this.bufferSize = bufferSize;

			//if( !GenerateBuffers( 2 ) )
			//{
			//	Log.Warning( "OpenALSoundSystem: Creating data stream sound failed." );
			//	return;
			//}

			Init( null, mode, 100000.0f, channels, frequency );
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	sealed class OpenALCaptureSoundData : OpenALSoundData
	{
		internal IntPtr alCaptureDevice;

		//

		public OpenALCaptureSoundData( SoundModes mode, int channels, int frequency, int bufferSize )
		{
			mode |= SoundModes.Loop | SoundModes.Software;

			int alFormat = channels == 2 ? Al.AL_FORMAT_STEREO16 : Al.AL_FORMAT_MONO16;

			alCaptureDevice = Alc.alcCaptureOpenDevice( OpenALSoundWorld.captureDeviceName, frequency, alFormat, bufferSize );
			if( alCaptureDevice == IntPtr.Zero )
				return;

			this.channels = channels;
			this.frequency = frequency;

			Init( null, mode, 100000.0f, channels, frequency );
		}

		protected override void OnDispose()
		{
			OpenALSoundWorld.criticalSection.Enter();

			if( alCaptureDevice != IntPtr.Zero )
			{
				Alc.alcCaptureCloseDevice( alCaptureDevice );
				alCaptureDevice = IntPtr.Zero;
			}

			OpenALSoundWorld.criticalSection.Leave();

			base.OnDispose();
		}

		public override int RecordRead( byte[] buffer, int length )
		{
			OpenALSoundWorld.criticalSection.Enter();

			int samplesAvailable;
			Alc.alcGetIntegerv( alCaptureDevice, Alc.ALC_CAPTURE_SAMPLES, 1, out samplesAvailable );
			int bytesAvailable = channels * 2 * samplesAvailable;

			int needLength = Math.Min( length, bytesAvailable );

			unsafe
			{
				fixed ( byte* pBuffer = buffer )
					Alc.alcCaptureSamples( alCaptureDevice, (IntPtr)pBuffer, needLength / ( channels * 2 ) );
			}

			OpenALSoundWorld.criticalSection.Leave();

			return needLength;
		}
	}
}
