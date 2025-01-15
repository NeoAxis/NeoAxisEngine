// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using NeoAxis;
using Tao.OpenAl;
using System.Runtime.CompilerServices;

namespace OpenALSoundSystem
{
	sealed class OpenALSoundWorld : SoundWorld
	{
		static IntPtr alDevice;
		static IntPtr alContext;

		static internal List<OpenALRealChannel> realChannels;
		static internal List<OpenALRealChannel> fileStreamRealChannels;

		static bool useThread;
		static Thread thread;
		static volatile bool needAbortThread;

		static IntPtr hWnd;

		static internal string captureDeviceName = "";

		static OpenALCaptureSoundData recordingSound;

		public static CriticalSection criticalSection;

		static float[] tempFloatArray6 = new float[ 6 ];

		//

		internal static bool CheckError()
		{
			int error = Al.alGetError();

			if( error == Al.AL_NO_ERROR )
				return false;

			string text;

			switch( error )
			{
			case Al.AL_INVALID_ENUM: text = "Invalid enum"; break;
			case Al.AL_INVALID_VALUE: text = "Invalid value"; break;
			case Al.AL_INVALID_OPERATION: text = "Invalid operation"; break;
			case Al.AL_OUT_OF_MEMORY: text = "Out of memory"; break;
			case Al.AL_INVALID_NAME: text = "Invalid name"; break;
			default: text = string.Format( "Unknown error ({0})", error ); break;
			}

			Log.Warning( "OpenALSoundSystem: Internal error: {0}.", text );

			return true;
		}

		protected override bool InitLibrary( IntPtr mainWindowHandle, int maxReal2DChannels, int maxReal3DChannels )
		{
			//NativeLibraryManager.PreloadLibrary( "libogg" );
			//NativeLibraryManager.PreloadLibrary( "libvorbis" );
			//NativeLibraryManager.PreloadLibrary( "libvorbisfile" );

			////preload dlls
			//{
			//	var fileNames = new List<string>();
			//	if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
			//		fileNames.Add( "OpenAL32.dll" );
			//	else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
			//	{
			//		fileNames.Add( "SDL2.dll" );
			//		fileNames.Add( "OpenAL32.dll" );
			//	}
			//	else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
			//		fileNames.Add( "OpenAL32.dylib" );
			//	else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
			//	{
			//	}
			//	else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS )
			//	{
			//	}
			//	else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
			//	{
			//	}
			//	else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Linux )
			//	{
			//	}
			//	else
			//	{
			//		Log.Fatal( "OpenALSoundWorld: InitLibrary: Unknown platform." );
			//		return false;
			//	}

			//	foreach( var fileName in fileNames )
			//	{
			//		var path = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, fileName );
			//		if( File.Exists( path ) )
			//			NativeUtility.PreloadLibrary( fileName );
			//	}
			//}

			criticalSection = CriticalSection.Create();

			//if( PlatformInfo.Platform == PlatformInfo.Platforms.Android )
			//{
			//   Alc.alcSetJNIEnvironmentAndJavaVM(
			//      EngineApp.Instance._CallCustomPlatformSpecificMethod( "GetJNIEnvironment", IntPtr.Zero ),
			//      EngineApp.Instance._CallCustomPlatformSpecificMethod( "GetJavaVM", IntPtr.Zero ) );
			//}

			//string[] devices = Alc.alcGetStringv( IntPtr.Zero, Alc.ALC_DEVICE_SPECIFIER );

			try
			{
				alDevice = Alc.alcOpenDevice( null );
			}
			catch( DllNotFoundException )
			{
				Log.InvisibleInfo( "OpenALSoundSystem: OpenAL not found." );
				return false;
			}
			catch( Exception e )
			{
				Log.InvisibleInfo( "OpenALSoundSystem: Open device failed. " + e.Message );
				return false;
			}
			if( alDevice == IntPtr.Zero )
			{
				Log.InvisibleInfo( "OpenALSoundSystem: No sound driver." );
				return false;
			}

			alContext = Alc.alcCreateContext( alDevice, IntPtr.Zero );
			if( alContext == IntPtr.Zero )
			{
				Log.Error( "OpenALSoundSystem: Create context failed." );
				return false;
			}

			try
			{
				//!!!!new. not work on windows
				//var result = Alc.alcMakeContextCurrent( alContext );
				//if( result != 1 )
				//	return false;

				Alc.alcMakeContextCurrent( alContext );
			}
			catch( Exception e )
			{
				Log.InvisibleInfo( "OpenALSoundSystem: alcMakeContextCurrent failed. " + e.Message );
				return false;
			}

			//!!!!new. no warnings
			Al.alGetError();
			//if( CheckError() )
			//	return false;

			//get captureDeviceName
			try
			{
				captureDeviceName = Alc.alcGetString( alDevice, Alc.ALC_CAPTURE_DEFAULT_DEVICE_SPECIFIER );
			}
			catch { }

			//Channels
			realChannels = new List<OpenALRealChannel>();
			for( int n = 0; n < maxReal2DChannels; n++ )
			{
				OpenALRealChannel realChannel = new OpenALRealChannel();
				AddRealChannel( realChannel, false );
				realChannels.Add( realChannel );
			}
			for( int n = 0; n < maxReal3DChannels; n++ )
			{
				OpenALRealChannel realChannel = new OpenALRealChannel();
				AddRealChannel( realChannel, true );
				realChannels.Add( realChannel );
			}

			fileStreamRealChannels = new List<OpenALRealChannel>();


			useThread = true;
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
				useThread = false;

			Log.InvisibleInfo( "OpenALSoundSystem: Threads: " + useThread.ToString() );

			if( useThread )
			{
				thread = new Thread( new ThreadStart( ThreadFunction ) );
				try
				{
					if( SystemSettings.CurrentPlatform != SystemSettings.Platform.UWP )
						thread.CurrentCulture = new System.Globalization.CultureInfo( "en-US" );
				}
				catch { }
				thread.IsBackground = true;
				thread.Start();
			}

			hWnd = mainWindowHandle;

			Al.alDistanceModel( Al.AL_NONE );

			return true;
		}

		protected override void ShutdownLibrary()
		{
			if( thread != null )
			{
				needAbortThread = true;
				Thread.Sleep( 100 );
				//Thread.Sleep( 50 );
				//thread.Abort();
			}

			if( realChannels != null )
			{
				foreach( OpenALRealChannel realChannel in realChannels )
				{
					if( realChannel.alSource != 0 )
					{
						Al.alDeleteSources( 1, ref realChannel.alSource );
						realChannel.alSource = 0;
					}
				}
			}

			try
			{
				Alc.alcMakeContextCurrent( IntPtr.Zero );
				Alc.alcDestroyContext( alContext );
				Alc.alcCloseDevice( alDevice );
			}
			catch { }

			if( realChannels != null )
			{
				realChannels.Clear();
				realChannels = null;
			}

			if( criticalSection != null )
			{
				criticalSection.Dispose();
				criticalSection = null;
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnUpdateLibrary()
		{
			if( IsActive() )
			{
				if( !useThread )
					UpdateNoThreads();

				criticalSection.Enter();

				for( int n = 0; n < realChannels.Count; n++ )
					realChannels[ n ].Update();

				criticalSection.Leave();
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override SoundData Internal_SoundCreate( string name, SoundModes mode )
		{
			criticalSection.Enter();

			OpenALSoundData sound;

			sound = (OpenALSoundData)base.Internal_SoundCreate( name, mode );
			if( sound != null )
			{
				criticalSection.Leave();
				return sound;
			}

			VirtualFileStream stream = CreateFileStream( name );
			if( stream == null )
			{
				criticalSection.Leave();
				Log.Warning( string.Format( "Creating sound \"{0}\" failed.", name ) );
				return null;
			}

			try
			{
				bool initialized;


				//!!!!temp
				//but maybe is good for mobile devices
				var allowStreaming = true;
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
					allowStreaming = false;


				if( ( mode & SoundModes.Stream ) == 0 || !allowStreaming )
				{
					//read whole file to a memory stream
					var bytes = new byte[ stream.Length ];
					IOUtility.ReadGuaranteed( stream, bytes );
					//stream.Read( bytes, 0, bytes.Length );
					var memoryStream = new MemoryVirtualFileStream( bytes );

					sound = new OpenALSampleSoundData( memoryStream, SoundType.Unknown, name, mode, out initialized );
					stream.Dispose();
				}
				else
					sound = new OpenALFileStreamSoundData( stream, /*true, */SoundType.Unknown, name, mode, out initialized );

				if( !initialized )
				{
					sound.Dispose();
					sound = null;
				}
			}
			catch( Exception ex )
			{
				criticalSection.Leave();
				Log.Warning( string.Format( "Creating sound \"{0}\" failed. {1}", name, ex.Message ) );
				return null;
			}

			criticalSection.Leave();

			return sound;
		}

		//!!!!было. file stream менялся, пока отключено
		//protected override Sound _SoundCreate( VirtualFileStream stream, bool closeStreamAfterReading, SoundType soundType, SoundModes mode )
		//{
		//	criticalSection.Enter();

		//	OpenALSound sound;
		//	bool initialized;

		//	if( (int)( mode & SoundModes.Stream ) == 0 )
		//	{
		//		sound = new OpenALSampleSound( stream, soundType, null, mode, out initialized );
		//		if( closeStreamAfterReading )
		//			stream.Dispose();
		//	}
		//	else
		//		sound = new OpenALFileStreamSound( stream, closeStreamAfterReading, soundType, null, mode, out initialized );

		//	if( !initialized )
		//	{
		//		sound.Dispose();
		//		sound = null;
		//	}

		//	criticalSection.Leave();

		//	return sound;
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override SoundData Internal_SoundCreateDataBuffer( SoundModes mode, int channels, int frequency, int bufferSize, DataReadDelegate dataReadCallback )
		{
			criticalSection.Enter();

			SoundData sound;

			if( ( mode & SoundModes.Record ) != 0 )
			{
				OpenALCaptureSoundData captureSound = new OpenALCaptureSoundData( mode, channels, frequency, bufferSize );
				if( captureSound.alCaptureDevice == IntPtr.Zero )
				{
					criticalSection.Leave();
					return null;
				}
				sound = captureSound;
			}
			else
				sound = new OpenALDataStreamSoundData( mode, channels, frequency, bufferSize, dataReadCallback );

			criticalSection.Leave();

			return sound;
		}

		protected override void OnSetDopplerEffectScale( double dopplerScale )
		{
			criticalSection.Enter();

			Al.alDopplerFactor( (float)dopplerScale );
			CheckError();
			Al.alDopplerVelocity( 1.0f );
			CheckError();

			criticalSection.Leave();
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnSetListener( Vector3 position, Vector3 velocity, Quaternion rotation )
		{
			criticalSection.Enter();

			//!!!!double
			Al.alListener3f( Al.AL_POSITION, (float)position.X, (float)position.Y, (float)position.Z );
			Al.alListener3f( Al.AL_VELOCITY, (float)velocity.X, (float)velocity.Y, (float)velocity.Z );

			var forward = rotation.GetForward();
			var up = rotation.GetUp();

			unsafe
			{
				fixed( float* orientation = tempFloatArray6 )
				{
					orientation[ 0 ] = (float)forward.X;
					orientation[ 1 ] = (float)forward.Y;
					orientation[ 2 ] = (float)forward.Z;
					orientation[ 3 ] = (float)up.X;
					orientation[ 4 ] = (float)up.Y;
					orientation[ 5 ] = (float)up.Z;
					Al.alListenerfv( Al.AL_ORIENTATION, orientation );
				}
			}

			CheckError();

			criticalSection.Leave();
		}

		protected override string Internal_DriverName
		{
			get
			{
				criticalSection.Enter();

				string version = "UNKNOWN";
				string device = "UNKNOWN";
				try
				{
					version = Al.alGetString( Al.AL_VERSION );
					device = Alc.alcGetString( alDevice, Alc.ALC_DEVICE_SPECIFIER );
				}
				catch { }

				string value = string.Format( "OpenAL {0}, Device: {1}", version, device );

				criticalSection.Leave();

				return value;
			}
		}

		protected override string[] Internal_RecordDrivers
		{
			get
			{
				criticalSection.Enter();

				string[] value = null;
				try
				{
					value = Alc.alcGetStringv( IntPtr.Zero, Alc.ALC_CAPTURE_DEVICE_SPECIFIER );
				}
				catch { }
				if( value == null )
					value = new string[ 0 ];

				criticalSection.Leave();

				return value;
			}
		}

		protected override int Internal_RecordDriver
		{
			get
			{
				return Array.IndexOf<string>( Internal_RecordDrivers, captureDeviceName );
			}
			set
			{
				captureDeviceName = Internal_RecordDrivers[ value ];
			}
		}

		protected override bool Internal_RecordStart( SoundData sound )
		{
			criticalSection.Enter();

			OpenALCaptureSoundData captureSound = sound as OpenALCaptureSoundData;
			if( captureSound == null )
			{
				criticalSection.Leave();
				Log.Warning( "OpenALSoundSystem: Recording failed. Is sound a not for recording." );
				return false;
			}

			Alc.alcCaptureStart( captureSound.alCaptureDevice );

			recordingSound = captureSound;

			criticalSection.Leave();
			return true;
		}

		protected override void Internal_RecordStop()
		{
			criticalSection.Enter();

			if( recordingSound != null )
			{
				Alc.alcCaptureStop( recordingSound.alCaptureDevice );
				recordingSound = null;
			}

			criticalSection.Leave();
		}

		protected override bool Internal_IsRecording()
		{
			return recordingSound != null;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void ThreadFunction()
		{
			while( true )
			{
				criticalSection.Enter();

				for( int n = 0; n < fileStreamRealChannels.Count; n++ )
					fileStreamRealChannels[ n ].UpdateFileStreamFromThread();

				criticalSection.Leave();

				while( !IsActive() )
				{
					Thread.Sleep( 100 );
					if( needAbortThread )
						break;
				}

				Thread.Sleep( 10 );

				if( needAbortThread )
					break;
			}
		}

		void UpdateNoThreads()
		{
			for( int n = 0; n < fileStreamRealChannels.Count; n++ )
				fileStreamRealChannels[ n ].UpdateFileStreamFromThread();
		}

		//[DllImport( "user32.dll" )]
		//static extern int IsWindow( IntPtr hWnd );
		//[DllImport( "user32.dll" )]
		//static extern int IsWindowVisible( IntPtr hWnd );
		//[DllImport( "user32.dll" )]
		//static extern int IsIconic( IntPtr hWnd );
		//[DllImport( "user32.dll" )]
		//static extern IntPtr GetParent( IntPtr hWnd );

		//[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
		//private static extern IntPtr GetForegroundWindow();
		//[DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
		//private static extern int GetWindowThreadProcessId( IntPtr handle, out int processId );

		//static bool ApplicationIsActivated()
		//{
		//	var activatedHandle = GetForegroundWindow();
		//	if( activatedHandle == IntPtr.Zero )
		//		return false;       // No window is currently activated

		//	var procId = Process.GetCurrentProcess().Id;
		//	int activeProcId;
		//	GetWindowThreadProcessId( activatedHandle, out activeProcId );

		//	return activeProcId == procId;
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		bool IsActive()
		{
			SoundChannelGroup masterGroup = MasterChannelGroup;
			bool masterGroupPaused = masterGroup == null || masterGroup.Pause;

			bool active = !masterGroupPaused;

			if( active && Internal_SuspendWorkingWhenApplicationIsNotActive )
			{
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
				{
					active = EngineApp.platform.ApplicationIsActivated();

					//IntPtr h = hWnd;
					//while( h != IntPtr.Zero )
					//{
					//	active = IsWindow( h ) != 0 && IsWindowVisible( h ) != 0 && IsIconic( h ) == 0;
					//	if( !active )
					//		break;

					//	h = GetParent( h );
					//}
				}
				else
				{
					if( EngineApp.EnginePaused )
						active = false;
				}
			}

			return active;
		}

		public static VirtualFileStream CreateFileStream2( string fileName )
		{
			return CreateFileStream( fileName );
		}
	}
}
