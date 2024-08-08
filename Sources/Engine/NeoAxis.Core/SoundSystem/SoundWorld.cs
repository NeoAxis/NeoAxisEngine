// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//!!!!!threading всей либе?

	/// <summary>
	/// Represents a sound system of the engine.
	/// </summary>
	public abstract class SoundWorld
	{
		static SoundWorld instance;
		static bool instanceIsNullSoundWorld;

		//system
		static bool suspendWorkingWhenApplicationIsNotActive = true;

		//channel groups
		static internal List<SoundChannelGroup> channelGroups = new List<SoundChannelGroup>();
		static SoundChannelGroup masterChannelGroup;

		//real channel
		static List<SoundRealChannel> real2DChannels = new List<SoundRealChannel>();
		static List<SoundRealChannel> real3DChannels = new List<SoundRealChannel>();

		//virtual channels
		static internal ESet<SoundVirtualChannel> activeVirtual2DChannels = new ESet<SoundVirtualChannel>();
		static internal ESet<SoundVirtualChannel> activeVirtual3DChannels = new ESet<SoundVirtualChannel>();
		//static internal List<SoundVirtualChannel> activeVirtual2DChannels = new List<SoundVirtualChannel>();
		//static internal List<SoundVirtualChannel> activeVirtual3DChannels = new List<SoundVirtualChannel>();

		//sounds
		static internal Dictionary<string, SoundData> sounds = new Dictionary<string, SoundData>( 64 );

		//listener
		static Scene listenerCurrentScene;
		static Vector3 listenerPosition;
		static Vector3 listenerVelocity;
		static Quaternion listenerRotation;

		//double distanceFactor;
		static double dopplerScale;

		static double lastUpdateTime;

		static bool channelsUpdating;

		//!!!!
		////update after system pause. it need only for some bugged sound drivers
		//static double startTimeAfterEnginePauseUpdate;

		//

		protected SoundWorld()
		{
		}

		internal static void Init( IntPtr mainWindowHandle )
		{
			instanceIsNullSoundWorld = false;

			if( instance != null )
				Log.Fatal( "SoundWorld: Init: The sound system is already initialized." );

			CreateSoundWorldInstance();

			var maxReal2DChannels = EngineApp.InitSettings.SoundMaxReal2DChannels;
			var maxReal3DChannels = EngineApp.InitSettings.SoundMaxReal3DChannels;

			if( instance == null || !instance.InitLibrary( mainWindowHandle, maxReal2DChannels, maxReal3DChannels ) )
			{
				Shutdown();

				instance = new NoneSoundWorld();
				instance.InitLibrary( mainWindowHandle, maxReal2DChannels, maxReal3DChannels );
			}

			if( instance is NoneSoundWorld )
			{
				maxReal2DChannels = 0;
				maxReal3DChannels = 0;
				instanceIsNullSoundWorld = true;
			}

			instance.InitInternal( maxReal2DChannels, maxReal3DChannels );

			if( Internal_Instance != null )
				Log.InvisibleInfo( string.Format( "Sound system is initialized (Driver: {0})", Internal_Instance.Internal_DriverName ) );
		}

		static void CreateSoundWorldInstance()
		{
			if( string.Compare( EngineApp.InitSettings.SoundSystem, "null", true ) != 0 )
			{
				//default sound system
				instance = new OpenALSoundSystem.OpenALSoundWorld();

				//string fullPath = "";
				//if( !string.IsNullOrEmpty( EngineApp.InitSettings.SoundSystem ) )
				//{
				//	string fullPath2 = Path.Combine( VirtualFileSystem.Directories.Binaries, EngineApp.InitSettings.SoundSystem );
				//	if( File.Exists( fullPath2 ) )
				//		fullPath = fullPath2;
				//}

				//if( fullPath != "" )
				//{
				//	try
				//	{
				//		Assembly assembly = Internal.AssemblyUtility.LoadAssemblyByRealFileName( fullPath, false );

				//		Type foundType = null;
				//		foreach( Type type in assembly.GetTypes() )
				//		{
				//			if( !type.IsAbstract && typeof( SoundWorld ).IsAssignableFrom( type ) )
				//			{
				//				foundType = type;
				//				break;
				//			}
				//		}

				//		if( foundType == null )
				//		{
				//			Log.Fatal( "SoundWorld: CreateSoundWorldInstance: SoundWorld based class is not available in the assembly \'{0}\'.", assembly.FullName );
				//		}

				//		ConstructorInfo constructor = foundType.GetConstructor( new Type[ 0 ] { } );
				//		instance = (SoundWorld)constructor.Invoke( null );
				//	}
				//	catch( Exception e )
				//	{
				//		Log.Fatal( "SoundWorld: CreateSoundWorldInstance: Loading assembly failed \'{0}\' ({1}).", fullPath, e.Message );
				//	}
				//}
				//else
				//{
				//	//if( SystemSettings.CurrentPlatform != SystemSettings.Platform.UWP )
				//	//{

				//	//default sound system
				//	instance = new OpenALSoundSystem.OpenALSoundWorld();

				//	//}
				//}
			}
		}

		internal static void Shutdown()
		{
			if( instance != null )
			{
				instance.ShutdownInternal();
				instance.ShutdownLibrary();
				instance = null;
			}
		}

		public static SoundWorld Internal_Instance
		{
			get { return instance; }
		}

		protected abstract bool InitLibrary( IntPtr mainWindowHandle, int maxReal2DChannels, int maxReal3DChannels );
		protected abstract void ShutdownLibrary();

		protected void InitInternal( int maxReal2DChannels, int maxReal3DChannels )
		{
			masterChannelGroup = CreateChannelGroup( "Master" );

			SetDopplerEffectScale( 1 );
			SetListener( null, Vector3.Zero, Vector3.Zero, Quaternion.FromDirectionZAxisUp( Vector3.XAxis ) );
		}

		protected void ShutdownInternal()
		{
			StopAllChannels();
			DisposeAllSounds();
		}

		//public abstract string[] PlaybackDrivers
		//{
		//   get;
		//}

		protected abstract string[] Internal_RecordDrivers
		{
			get;
		}
		public static string[] RecordDrivers
		{
			get { return Internal_Instance.Internal_RecordDrivers; }
		}

		//public abstract int PlaybackDriver
		//{
		//   get;
		//   set;
		//}

		protected abstract int Internal_RecordDriver
		{
			get;
			set;
		}
		public static int RecordDriver
		{
			get { return instance.Internal_RecordDriver; }
			set { instance.Internal_RecordDriver = value; }
		}

		public static void DisposeAllSounds()
		{
			while( sounds.Count != 0 )
			{
				var enumerator = sounds.GetEnumerator();
				enumerator.MoveNext();
				enumerator.Current.Value.Dispose();
			}
		}

		public static void StopAllChannels()
		{
			foreach( var channel in activeVirtual2DChannels.ToArray() )
				channel.Stop();
			foreach( var channel in activeVirtual3DChannels.ToArray() )
				channel.Stop();

			//while( activeVirtual2DChannels.Count != 0 )
			//	activeVirtual2DChannels[ activeVirtual2DChannels.Count - 1 ].Stop();
			//while( activeVirtual3DChannels.Count != 0 )
			//	activeVirtual3DChannels[ activeVirtual3DChannels.Count - 1 ].Stop();
		}

		protected abstract void OnUpdateLibrary();

		[MethodImpl( (MethodImplOptions)512 )]
		internal static void UpdateInternal( /*double time,*/ bool forceUpdateChannels = false )
		{
			//check deleted scene
			if( listenerCurrentScene != null && listenerCurrentScene.Disposed )
				SetListener( null, ListenerPosition, ListenerVelocity, ListenerRotation );

			double time = EngineApp.EngineTime;

			instance.OnUpdateLibrary();

			double delta = time - lastUpdateTime;
			if( delta > 0.001 || forceUpdateChannels )//if( delta > 0.05 || forceUpdateChannels )
				UpdateChannels( time );

			//!!!!
			////update after engine pause. it need only for some bugged sound drivers.
			//if( startTimeAfterEnginePauseUpdate != 0 )
			//{
			//	if( startTimeAfterEnginePauseUpdate < time - 1 )
			//		startTimeAfterEnginePauseUpdate = 0;

			//	if( MasterChannelGroup != null )
			//	{
			//		double v = MasterChannelGroup.Volume;
			//		MasterChannelGroup.Volume = .00001f;
			//		MasterChannelGroup.Volume = v;
			//	}
			//}
		}

		//!!!!
		//internal static void _UpdateAfterEnginePause( double time )
		//{
		//	//update after system pause. it need only for some bugged sound drivers
		//	startTimeAfterEnginePauseUpdate = time;
		//}

		protected virtual SoundData Internal_SoundCreate( string fileName, SoundModes mode )
		{
			SoundData sound;
			string nameWithMode = fileName.ToLower() + " " + ( (int)mode & ~(int)SoundModes.Software ).ToString();
			if( sounds.TryGetValue( nameWithMode, out sound ) )
				return sound;
			return null;
		}
		public static SoundData SoundCreate( string fileName, SoundModes mode )
		{
			return instance.Internal_SoundCreate( fileName, mode );
		}

		//!!!!было. file stream менялся, пока отключено
		//protected abstract Sound _SoundCreate( VirtualFileStream stream, bool closeStreamAfterReading, SoundType soundType, SoundModes mode );
		//public Sound SoundCreate( VirtualFileStream stream, bool closeStreamAfterReading, SoundType soundType, SoundModes mode )
		//{
		//	return instance._SoundCreate( stream, closeStreamAfterReading, soundType, mode );
		//}

		public delegate int DataReadDelegate( byte[] buffer, int bufferOffset, int length );
		protected abstract SoundData Internal_SoundCreateDataBuffer( SoundModes mode, int channels, int frequency, int bufferSize, DataReadDelegate dataReadCallback );

		public static SoundData SoundCreateDataBuffer( SoundModes mode, int channels, int frequency, int bufferSize, DataReadDelegate dataReadCallback )
		{
			return instance.Internal_SoundCreateDataBuffer( mode, channels, frequency, bufferSize, dataReadCallback );
		}

		public static SoundChannelGroup CreateChannelGroup( string name )
		{
			SoundChannelGroup group = new SoundChannelGroup( name );
			channelGroups.Add( group );
			return group;
		}

		protected abstract void OnSetDopplerEffectScale( double dopplerScale );

		public static void SetDopplerEffectScale( double dopplerScale )
		{
			if( SoundWorld.dopplerScale != dopplerScale )
			{
				SoundWorld.dopplerScale = dopplerScale;
				instance.OnSetDopplerEffectScale( dopplerScale );
			}
		}

		protected abstract void OnSetListener( Vector3 position, Vector3 velocity, Quaternion rotation );

		public static void SetListener( Scene currentScene, Vector3 position, Vector3 velocity, Quaternion rotation )
		{
			bool newScene = !ReferenceEquals( listenerCurrentScene, currentScene );
			double saveVolume = 0;

			if( newScene )
			{
				listenerCurrentScene = currentScene;

				saveVolume = MasterChannelGroup.Volume;
				MasterChannelGroup.Volume = 0;
			}

			if( listenerPosition != position || listenerVelocity != velocity || listenerRotation != rotation )
			{
				listenerPosition = position;
				listenerVelocity = velocity;
				listenerRotation = rotation;
				instance.OnSetListener( listenerPosition, listenerVelocity, listenerRotation );
			}

			if( newScene )
			{
				MasterChannelGroup.Volume = saveVolume;
				UpdateInternal( true );
			}
		}

		public static void SetListenerReset()
		{
			SetListener( null, new Vector3( 1000000, 1000000, 1000000 ), Vector3.Zero, Quaternion.FromDirectionZAxisUp( Vector3.XAxis ) );
		}

		public static SoundChannelGroup MasterChannelGroup
		{
			get { return masterChannelGroup; }
		}

		protected abstract string Internal_DriverName
		{
			get;
		}
		public static string DriverName
		{
			get { return instance.Internal_DriverName; }
		}

		protected static VirtualFileStream CreateFileStream( string fileName )
		{
			try
			{
				return VirtualFile.Open( fileName );
			}
			catch( FileNotFoundException )
			{
				return null;
			}
			catch( Exception e )
			{
				Log.Warning( "SoundWorld: File stream creation failed: {0}", e.Message );
				return null;
			}
		}

		protected static void AddRealChannel( SoundRealChannel realChannel, bool is3D )
		{
			realChannel.index = real2DChannels.Count + real3DChannels.Count;
			realChannel.is3D = is3D;
			if( is3D )
				real3DChannels.Add( realChannel );
			else
				real2DChannels.Add( realChannel );
		}

		public static ReadOnlyCollection<SoundRealChannel> Real2DChannels
		{
			get { return real2DChannels.AsReadOnly(); }
		}

		public static ReadOnlyCollection<SoundRealChannel> Real3DChannels
		{
			get { return real3DChannels.AsReadOnly(); }
		}

		public static ICollection<SoundVirtualChannel> ActiveVirtual2DChannels
		{
			get { return activeVirtual2DChannels.AsReadOnly(); }
		}

		public static ICollection<SoundVirtualChannel> ActiveVirtual3DChannels
		{
			get { return activeVirtual3DChannels.AsReadOnly(); }
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public static SoundVirtualChannel SoundPlay( Scene attachedToScene, SoundData sound, SoundChannelGroup group, double priority, double volume, bool paused = false )
		{
			if( sound == null || BackendNull )
				return null;

			//!!!!

			////!!!!так?
			//if( ( sound.Mode & SoundModes.Loop ) == 0 && ( sound.Mode & SoundModes.Stream ) == 0 )
			//{
			//	if( attachedToScene != null && listenerCurrentScene != attachedToScene )
			//		return null;
			//}

			//!!!!

			//!!!!было
			////!!!!так?
			//if( ( sound.Mode & SoundModes.Loop ) == 0 && ( sound.Mode & SoundModes.Stream ) == 0 )
			//{
			//	List<RealChannel> realChannels = ( sound.Mode & SoundModes.Mode3D ) != 0 ? real3DChannels : real2DChannels;

			//	bool allowPlay = false;
			//	for( int n = 0; n < realChannels.Count; n++ )
			//	{
			//		VirtualChannel c = realChannels[ n ].CurrentVirtualChannel;
			//		if( c == null || c.Priority < priority )
			//		{
			//			allowPlay = true;
			//			break;
			//		}
			//	}
			//	if( !allowPlay )
			//		return null;
			//}

			//priority = MathEx.Saturate( priority );


			//!!!!

			//!!!!было
			//if( ( sound.Mode & SoundModes.Stream ) != 0 )
			//{
			//	if( sound.playingCount == 1 )
			//	{
			//		Log.Warning( "SoundSystem: It is impossible to play simultaneously more one channel stream sound." );
			//		return null;
			//	}
			//}

			if( group == null )
				group = MasterChannelGroup;

			var virtualChannel = new SoundVirtualChannel();

			if( ( sound.Mode & SoundModes.Mode3D ) != 0 )
				activeVirtual3DChannels.Add( virtualChannel );
			else
				activeVirtual2DChannels.Add( virtualChannel );

			virtualChannel.SoundPlayInit( attachedToScene, sound, group );
			virtualChannel.Priority = priority;
			virtualChannel.Volume = volume;

			if( !paused )
				virtualChannel.Pause = false;

			return virtualChannel;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void OnVirtualChannelUpdatePause( SoundVirtualChannel virtualChannel )
		{
			if( !virtualChannel.IsTotalPaused() )
			{
				lastUpdateTime = 0;
				//slowly
				//if( lastUpdateTime != 0 )
				//	UpdateChannels( lastUpdateTime );
			}
			else
			{
				//!!!!slowly?

				if( virtualChannel.currentRealChannel != null )
				{
					virtualChannel.currentRealChannel.PreDetachVirtualChannel();
					virtualChannel.currentRealChannel.currentVirtualChannel = null;
					virtualChannel.currentRealChannel = null;
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static void UpdateChannels( double time )
		{
			if( channelsUpdating )
				return;

			if( lastUpdateTime == 0 )
				lastUpdateTime = time;

			double delta = time - lastUpdateTime;
			lastUpdateTime = time;

			channelsUpdating = true;
			UpdateChannels( delta, false );
			UpdateChannels( delta, true );
			channelsUpdating = false;
		}

		public static Dictionary<string, SoundData>.ValueCollection Sounds
		{
			get { return sounds.Values; }
		}

		protected abstract bool Internal_RecordStart( SoundData sound );
		public static bool RecordStart( SoundData sound ) { return instance.Internal_RecordStart( sound ); }

		protected abstract void Internal_RecordStop();
		public static void RecordStop() { instance.Internal_RecordStop(); }

		protected abstract bool Internal_IsRecording();
		public static bool IsRecording() { return instance.Internal_IsRecording(); }

		public static bool Internal_SuspendWorkingWhenApplicationIsNotActive
		{
			get { return suspendWorkingWhenApplicationIsNotActive; }
			set { suspendWorkingWhenApplicationIsNotActive = value; }
		}

		public virtual object CallCustomMethod( string message, object param ) { return null; }

		public static Scene ListenerCurrentScene
		{
			get { return listenerCurrentScene; }
		}

		public static Vector3 ListenerPosition
		{
			get { return listenerPosition; }
		}

		public static Vector3 ListenerVelocity
		{
			get { return listenerVelocity; }
		}

		public static Quaternion ListenerRotation
		{
			get { return listenerRotation; }
		}

		public static double DopplerScale
		{
			get { return dopplerScale; }
		}

		static Comparison<SoundVirtualChannel> virtualChannelsTempPriorityComparer = new Comparison<SoundVirtualChannel>( VirtualChannelsTempPriorityComparer );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static int VirtualChannelsTempPriorityComparer( SoundVirtualChannel c1, SoundVirtualChannel c2 )
		{
			return c2.tempPriority.CompareTo( c1.tempPriority );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		static void UpdateChannels( double delta, bool update3DChannels )
		{
			//update virtual channels time
			if( delta != 0 )
			{
				List<SoundVirtualChannel> toStop = null;

				var list = update3DChannels ? activeVirtual3DChannels : activeVirtual2DChannels;
				foreach( var virtualChannel in list )
				{
					if( !virtualChannel.IsTotalPaused() )
					{
						virtualChannel.time += delta * virtualChannel.pitch;

						var sound = virtualChannel.sound;
						if( virtualChannel.time >= sound.Length )
						{
							if( ( sound.Mode & SoundModes.Loop ) != 0 )
								virtualChannel.time -= sound.Length;
							else
							{
								if( virtualChannel.currentRealChannel == null || virtualChannel.time >= sound.Length + .1f )
								{
									if( toStop == null )
										toStop = new List<SoundVirtualChannel>();
									toStop.Add( virtualChannel );
								}
							}
						}

						//if( ( sound.Mode & SoundModes.Loop ) == 0 )
						//{
						//	if( virtualChannel.currentRealChannel == null )
						//	{
						//		//!!!!
						//		if( ( EngineApp.EngineTime - virtualChannel.startTime ) > virtualChannel.sound.Length * 2 )
						//		{
						//			if( toStop == null )
						//				toStop = new List<SoundVirtualChannel>();
						//			toStop.Add( virtualChannel );
						//		}
						//	}
						//}
					}
				}

				if( toStop != null )
				{
					foreach( var virtualChannel in toStop )
						virtualChannel.Stop();
				}
			}

			SoundVirtualChannel[] activeVirtualChannels;
			List<SoundRealChannel> realChannels;
			if( update3DChannels )
			{
				activeVirtualChannels = activeVirtual3DChannels.ToArray();
				realChannels = real3DChannels;
			}
			else
			{
				activeVirtualChannels = activeVirtual2DChannels.ToArray();
				realChannels = real2DChannels;
			}

			//calculate priority

			for( int n = 0; n < activeVirtualChannels.Length; n++ )
			{
				var virtualChannel = activeVirtualChannels[ n ];
				var sound = virtualChannel.sound;

				double priority = 0;

				if( virtualChannel.AttachedToScene == null || virtualChannel.AttachedToScene == listenerCurrentScene )
				{
					if( virtualChannel.GetTotalVolume() != 0 && !virtualChannel.IsTotalPaused() )
					{
						if( update3DChannels )
						{
							double distanceSqr = ( virtualChannel.position - listenerPosition ).LengthSquared();
							priority = ( 100000000.0 - distanceSqr ) * ( virtualChannel.Priority + .00001 );
							//priority += 1.0f / distanceSqr + virtualChannel.Priority * 10000;
						}
						else
						{
							//if( ( sound.Mode & SoundModes.Stream ) != 0 )
							//	priority += 10;
							priority = ( virtualChannel.Priority + 0.00001 ) * 10000000000;

							//fix sorting jerking problem
							priority += (double)sound.createIndex;
						}

						//if( ( sound.Mode & SoundModes.Mode3D ) == 0 )
						//{
						//	priority += 100000000;
						//	if( ( sound.Mode & SoundModes.Stream ) != 0 )
						//		priority += 100000000;
						//}
						//else
						//{
						//	double distanceSqr = ( virtualChannel.position - listenerPosition ).LengthSquared();

						//	priority += ( 1000000.0 - distanceSqr ) + virtualChannel.priority * 10000;
						//	//priority += 1.0f / distanceSqr + virtualChannel.Priority * 10000;
						//}

						////only for small fix sorting jerking problem at the DataBuffer and Streams
						//if( sound.Name == null || ( sound.Mode & SoundModes.Stream ) != 0 )
						//	priority += (double)sound.createIndex;
					}
				}

				virtualChannel.tempPriority = priority;
			}

			//sort by priority
			CollectionUtility.MergeSort( activeVirtualChannels, virtualChannelsTempPriorityComparer, true );

			//remove virtual channels from real channels
			for( int n = 0; n < activeVirtualChannels.Length; n++ )
			{
				var virtualChannel = activeVirtualChannels[ n ];
				var realChannel = virtualChannel.currentRealChannel;

				if( realChannel != null )
				{
					if( n >= realChannels.Count || virtualChannel.tempPriority == 0 )
					{
						realChannel.PreDetachVirtualChannel();
						realChannel.currentVirtualChannel = null;
						virtualChannel.currentRealChannel = null;
					}
				}
			}

			//bind virtual channels to real channels
			var freeRealChannels = new Queue<SoundRealChannel>( realChannels.Count );
			foreach( var c in realChannels )
			{
				if( c.CurrentVirtualChannel == null )
					freeRealChannels.Enqueue( c );
			}

			for( int n = 0; n < activeVirtualChannels.Length; n++ )
			{
				//no free real channels
				if( freeRealChannels.Count == 0 )
					break;

				var virtualChannel = activeVirtualChannels[ n ];
				if( virtualChannel.currentRealChannel == null && virtualChannel.tempPriority != 0 )
				{
					var realChannel = freeRealChannels.Dequeue();

					realChannel.currentVirtualChannel = virtualChannel;
					virtualChannel.currentRealChannel = realChannel;
					realChannel.PostAttachVirtualChannel();
				}
			}

			////add virtual channels to real channels
			//int currentRealChannelIndex = 0;

			//for( int n = 0; n < activeVirtualChannels.Count; n++ )
			//{
			//	VirtualChannel virtualChannel = activeVirtualChannels[ n ];

			//	if( currentRealChannelIndex >= realChannels.Count )
			//		break;
			//	if( virtualChannel.currentRealChannel != null )
			//		continue;
			//	if( virtualChannel.tempPriority == 0 )
			//		continue;

			//	if( virtualChannel.Time >= virtualChannel.Sound.Length )
			//	{
			//		if( ( virtualChannel.Sound.Mode & SoundModes.Loop ) == 0 )
			//			continue;
			//		else
			//			virtualChannel.time -= virtualChannel.Sound.Length;
			//	}

			//	RealChannel realChannel = null;
			//	{
			//		while( realChannels[ currentRealChannelIndex ].CurrentVirtualChannel != null )
			//		{
			//			currentRealChannelIndex++;
			//			if( currentRealChannelIndex >= realChannels.Count )
			//				break;
			//		}

			//		if( currentRealChannelIndex < realChannels.Count )
			//		{
			//			realChannel = realChannels[ currentRealChannelIndex ];
			//			currentRealChannelIndex++;
			//		}
			//	}

			//	if( realChannel == null )
			//		break;

			//	if( realChannel.currentVirtualChannel != null )
			//		Log.Fatal( "SoundWorld: UpdateChannels: realChannel.currentVirtualChannel != null." );

			//	realChannel.currentVirtualChannel = virtualChannel;
			//	virtualChannel.currentRealChannel = realChannel;
			//	realChannel.PostAttachVirtualChannel();
			//}

			//update gain for 3d sound (calculate attenuation)
			if( update3DChannels )
			{
				for( int n = 0; n < real3DChannels.Count; n++ )
				{
					var realChannel = real3DChannels[ n ];
					if( realChannel.currentVirtualChannel != null )
					{
						//!!!!slowly enter to critical section for each real channel?
						realChannel.UpdateVolume();
					}
				}
			}
		}


		//[MethodImpl( (MethodImplOptions)512 )]
		//static void UpdateChannels( double delta, bool update3DChannels )
		//{
		//	SoundVirtualChannel[] activeVirtualChannels;
		//	List<SoundRealChannel> realChannels;
		//	if( update3DChannels )
		//	{
		//		activeVirtualChannels = activeVirtual3DChannels.ToArray();
		//		realChannels = real3DChannels;
		//	}
		//	else
		//	{
		//		activeVirtualChannels = activeVirtual2DChannels.ToArray();
		//		realChannels = real2DChannels;
		//	}

		//	//update virtual channels time
		//	if( delta != 0 )
		//	{
		//		for( int n = 0; n < activeVirtualChannels.Length; n++ )
		//		{
		//			var virtualChannel = activeVirtualChannels[ n ];
		//			if( !virtualChannel.IsTotalPaused() )
		//			{
		//				virtualChannel.time += delta * virtualChannel.pitch;

		//				var sound = virtualChannel.sound;
		//				if( virtualChannel.time >= sound.Length )
		//				{
		//					if( ( sound.Mode & SoundModes.Loop ) != 0 )
		//						virtualChannel.time -= sound.Length;
		//					else
		//					{
		//						if( virtualChannel.currentRealChannel == null || virtualChannel.time >= sound.Length + .1f )
		//						{
		//							virtualChannel.Stop();
		//							//!!!!new
		//							//n--;
		//							continue;
		//						}
		//					}
		//				}

		//				//!!!!
		//				if( ( sound.Mode & SoundModes.Loop ) == 0 )
		//				{
		//					if( virtualChannel.currentRealChannel == null )
		//					{

		//						//!!!!

		//						if( ( EngineApp.EngineTime - virtualChannel.startTime ) > virtualChannel.sound.Length * 2 )
		//						{
		//							virtualChannel.Stop();
		//							continue;
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}


		//	//calculate priority

		//	//!!!!slowly. часто обновляется может

		//	for( int n = 0; n < activeVirtualChannels.Length; n++ )
		//	{
		//		var virtualChannel = activeVirtualChannels[ n ];
		//		var sound = virtualChannel.sound;

		//		double priority = 0;

		//		if( virtualChannel.AttachedToScene == null || virtualChannel.AttachedToScene == listenerCurrentScene )
		//		{
		//			if( virtualChannel.GetTotalVolume() != 0 && !virtualChannel.IsTotalPaused() )
		//			{
		//				if( update3DChannels )
		//				{
		//					double distanceSqr = ( virtualChannel.position - listenerPosition ).LengthSquared();
		//					priority = ( 100000000.0 - distanceSqr ) * ( virtualChannel.Priority + .00001 );
		//					//priority += 1.0f / distanceSqr + virtualChannel.Priority * 10000;
		//				}
		//				else
		//				{
		//					//if( ( sound.Mode & SoundModes.Stream ) != 0 )
		//					//	priority += 10;
		//					priority = ( virtualChannel.Priority + .00001 ) * 10000000000;

		//					//fix sorting jerking problem
		//					priority += (double)sound.createIndex;
		//				}

		//				//if( ( sound.Mode & SoundModes.Mode3D ) == 0 )
		//				//{
		//				//	priority += 100000000;
		//				//	if( ( sound.Mode & SoundModes.Stream ) != 0 )
		//				//		priority += 100000000;
		//				//}
		//				//else
		//				//{
		//				//	double distanceSqr = ( virtualChannel.position - listenerPosition ).LengthSquared();

		//				//	priority += ( 1000000.0 - distanceSqr ) + virtualChannel.priority * 10000;
		//				//	//priority += 1.0f / distanceSqr + virtualChannel.Priority * 10000;
		//				//}

		//				////only for small fix sorting jerking problem at the DataBuffer and Streams
		//				//if( sound.Name == null || ( sound.Mode & SoundModes.Stream ) != 0 )
		//				//	priority += (double)sound.createIndex;
		//			}
		//		}

		//		virtualChannel.tempPriority = priority;
		//	}

		//	//sort by priority
		//	CollectionUtility.MergeSort( activeVirtualChannels, virtualChannelsTempPriorityComparer, true );
		//	//CollectionUtility.SelectionSort( activeVirtualChannels, virtualChannelsTempPriorityComparer );

		//	//remove virtual channels from real channels
		//	for( int n = 0; n < activeVirtualChannels.Length; n++ )
		//	{
		//		var virtualChannel = activeVirtualChannels[ n ];
		//		var realChannel = virtualChannel.currentRealChannel;

		//		if( realChannel != null )
		//		{
		//			if( n >= realChannels.Count || virtualChannel.tempPriority == 0 )
		//			{
		//				realChannel.PreDetachVirtualChannel();
		//				realChannel.currentVirtualChannel = null;
		//				virtualChannel.currentRealChannel = null;
		//			}
		//		}
		//	}

		//	//bind virtual channels to real channels
		//	var freeRealChannels = new Queue<SoundRealChannel>( realChannels.Count );
		//	foreach( var c in realChannels )
		//	{
		//		if( c.CurrentVirtualChannel == null )
		//			freeRealChannels.Enqueue( c );
		//	}

		//	for( int n = 0; n < activeVirtualChannels.Length; n++ )
		//	{
		//		//no free real channels
		//		if( freeRealChannels.Count == 0 )
		//			break;

		//		var virtualChannel = activeVirtualChannels[ n ];
		//		if( virtualChannel.currentRealChannel == null && virtualChannel.tempPriority != 0 )
		//		{
		//			var realChannel = freeRealChannels.Dequeue();

		//			realChannel.currentVirtualChannel = virtualChannel;
		//			virtualChannel.currentRealChannel = realChannel;
		//			realChannel.PostAttachVirtualChannel();
		//		}
		//	}

		//	////add virtual channels to real channels
		//	//int currentRealChannelIndex = 0;

		//	//for( int n = 0; n < activeVirtualChannels.Count; n++ )
		//	//{
		//	//	VirtualChannel virtualChannel = activeVirtualChannels[ n ];

		//	//	if( currentRealChannelIndex >= realChannels.Count )
		//	//		break;
		//	//	if( virtualChannel.currentRealChannel != null )
		//	//		continue;
		//	//	if( virtualChannel.tempPriority == 0 )
		//	//		continue;

		//	//	if( virtualChannel.Time >= virtualChannel.Sound.Length )
		//	//	{
		//	//		if( ( virtualChannel.Sound.Mode & SoundModes.Loop ) == 0 )
		//	//			continue;
		//	//		else
		//	//			virtualChannel.time -= virtualChannel.Sound.Length;
		//	//	}

		//	//	RealChannel realChannel = null;
		//	//	{
		//	//		while( realChannels[ currentRealChannelIndex ].CurrentVirtualChannel != null )
		//	//		{
		//	//			currentRealChannelIndex++;
		//	//			if( currentRealChannelIndex >= realChannels.Count )
		//	//				break;
		//	//		}

		//	//		if( currentRealChannelIndex < realChannels.Count )
		//	//		{
		//	//			realChannel = realChannels[ currentRealChannelIndex ];
		//	//			currentRealChannelIndex++;
		//	//		}
		//	//	}

		//	//	if( realChannel == null )
		//	//		break;

		//	//	if( realChannel.currentVirtualChannel != null )
		//	//		Log.Fatal( "SoundWorld: UpdateChannels: realChannel.currentVirtualChannel != null." );

		//	//	realChannel.currentVirtualChannel = virtualChannel;
		//	//	virtualChannel.currentRealChannel = realChannel;
		//	//	realChannel.PostAttachVirtualChannel();
		//	//}

		//	//update gain for 3d sound (calculate attenuation)
		//	if( update3DChannels )
		//	{
		//		for( int n = 0; n < real3DChannels.Count; n++ )
		//		{
		//			SoundRealChannel realChannel = real3DChannels[ n ];
		//			if( realChannel.currentVirtualChannel != null )
		//			{
		//				//!!!!slowly enter to critical section for each real channel?
		//				realChannel.UpdateVolume();
		//			}
		//		}
		//	}
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static SoundChannelGroup GetChannelGroup( string name )
		{
			for( int n = 0; n < channelGroups.Count; n++ )
				if( channelGroups[ n ].Name == name )
					return channelGroups[ n ];
			return null;
		}

		public static bool BackendNull
		{
			get { return instanceIsNullSoundWorld; }
		}
	}
}
