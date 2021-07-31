// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NeoAxis;
using NeoAxis.OggVorbisTheora;

namespace OggDecoder
{
	sealed class OggFile : IDisposable
	{
		internal VirtualFileStream stream;
		VideoDriver videoDriver;
		AudioDriver audioDriver;

		ogg.SyncState oggSyncState;
		ogg.Page oggPage;

		bool audioStarted;

		internal double currentTime;

		bool endOfFile;

		bool pause;

		internal CriticalSection criticalSection;

		bool sound3D;
		Vector3 soundPosition;

		//internal bool decodeOnlyAudio;
		//internal bool decodeOnlyVideo;

		double lastUpdateTime;

		double volume = 1;

		//

		public OggFile()
		{
			criticalSection = CriticalSection.Create();
		}

		public void Init( VirtualFileStream stream, VideoDriver videoDriver, AudioDriver audioDriver,
			bool sound3D )
		{
			this.stream = stream;
			this.audioDriver = audioDriver;
			this.videoDriver = videoDriver;
			this.sound3D = sound3D;

			if( videoDriver != null )
				videoDriver.oggFile = this;
			if( audioDriver != null )
				audioDriver.oggFile = this;

			oggSyncState = new ogg.SyncState();
			oggPage = new ogg.Page();

			ParsePrimaryHeaders();
			ParseSecondaryHeaders();

			if( this.audioDriver != null )
				this.audioDriver.InitDSP();
			if( this.videoDriver != null )
				this.videoDriver.InitTheora();

			//Update first frame
			Update( 0 );
		}

		public void Dispose()
		{
			try
			{
				criticalSection?.Enter();

				Pause = true;

				if( videoDriver != null )
				{
					videoDriver.Dispose();
					videoDriver = null;
				}

				if( audioDriver != null )
				{
					audioDriver.Dispose();
					audioDriver = null;
				}

				if( oggSyncState != null )
				{
					oggSyncState.Dispose();
					oggSyncState = null;
				}

				if( oggPage != null )
				{
					oggPage.Dispose();
					oggPage = null;
				}

				if( stream != null )
				{
					stream.Dispose();
					stream = null;
				}

			}
			catch( Exception ex )
			{
				Log.Error( "OggFile: Exception: " + ex.ToString() );
			}
			finally
			{
				criticalSection?.Leave();
			}

			criticalSection?.Dispose();
			criticalSection = null;

			GC.SuppressFinalize( this );
		}

		public AudioDriver AudioDriver
		{
			get { return audioDriver; }
		}

		public VideoDriver VideoDriver
		{
			get { return videoDriver; }
		}

		public bool EndOfFile
		{
			get { return endOfFile; }
		}

		int BufferData()
		{
			IntPtr buffer = oggSyncState.buffer( 4096 );
			int bytes = stream.ReadUnmanaged( buffer, 4096 );
			oggSyncState.wrote( bytes );
			return bytes;
		}

		/// <summary>
		/// internal method for parsing the main stream headers
		/// </summary>
		void ParsePrimaryHeaders()
		{
			ogg.Packet tempOggPacket = new ogg.Packet();
			bool notDone = true;
			int vorbis_streams = 0;
			int theora_streams = 0;

			while( notDone )
			{
				if( BufferData() == 0 )
				{
					tempOggPacket.Dispose();
					throw new Exception( "Invalid ogg file" );
				}

				//Check for page data
				while( oggSyncState.pageout( oggPage ) > 0 )
				{
					//is this an initial header? If not, stop
					if( oggPage.bos() == 0 )
					{
						//This is done blindly, because stream only accept them selfs
						if( videoDriver != null && theora_streams > 0 )
							videoDriver.PageIn( oggPage );
						if( audioDriver != null && vorbis_streams > 0 )
							audioDriver.PageIn( oggPage );
						notDone = false;
						break;
					}

					ogg.StreamState oggStateTest = new ogg.StreamState( oggPage.serialno() );
					//oggStateTest.init( oggPage.serialno() );
					oggStateTest.pagein( oggPage );
					oggStateTest.packetout( tempOggPacket );

					//identify the codec
					if( videoDriver != null && videoDriver.DecodePrimaryHeader( tempOggPacket, oggStateTest ) )
						theora_streams = 1;
					else if( audioDriver != null && audioDriver.DecodePrimaryHeader( tempOggPacket, oggStateTest ) )
						vorbis_streams = 1;
					else
						oggStateTest.Dispose();

				} //end while ogg_sync_pageout
			} //end while notdone

			//if pri headers not found, then remove driver
			if( theora_streams == 0 && videoDriver != null )
			{
				videoDriver.Dispose();
				videoDriver = null;
			}
			if( vorbis_streams == 0 && audioDriver != null )
			{
				audioDriver.Dispose();
				audioDriver = null;
			}

			tempOggPacket.Dispose();
		}

		/// <summary>
		/// internal method to parse all secondary headers
		/// </summary>
		void ParseSecondaryHeaders()
		{
			ogg.Packet tempOggPacket = new ogg.Packet();
			bool theoraNotDone = true;
			bool vorbisNotDone = true;

			//Loop until all secondary headers of theora and/or vorbis are parsed
			while( ( videoDriver != null && theoraNotDone ) ||
				  ( audioDriver != null && vorbisNotDone ) )
			{
				bool needMoreData = false;

				while( videoDriver != null && theoraNotDone && !needMoreData )
					theoraNotDone = videoDriver.CheckSecondHeader( tempOggPacket, ref needMoreData );

				while( audioDriver != null && vorbisNotDone && !needMoreData )
					vorbisNotDone = audioDriver.CheckSecondHeader( tempOggPacket, ref needMoreData );

				//Buffer some more data
				if( oggSyncState.pageout( oggPage ) > 0 )
				{
					//This is done blindly, because stream only accept them selfs
					if( videoDriver != null )
						videoDriver.PageIn( oggPage );
					if( audioDriver != null )
						audioDriver.PageIn( oggPage );
				}
				else
				{
					if( BufferData() == 0 )
					{
						tempOggPacket.Dispose();
						throw new Exception( "Invalid ogg file" );
					}
				}
			}

			tempOggPacket.Dispose();
		}

		internal void Decode( bool decodeAudio, bool decodeVideo )
		{
			bool audioReady = true;
			bool videoReady = true;

			if( decodeAudio && audioDriver != null )
				audioReady = audioDriver.Decode();

			if( decodeVideo && videoDriver != null )
				videoReady = videoDriver.Decode();

			if( EndOfFile )
				return;

			if( !audioReady || !videoReady )
			{
				//read from stream
				int bytesRead = BufferData();

				while( oggSyncState.pageout( oggPage ) > 0 )
				{
					if( videoDriver != null )
						videoDriver.PageIn( oggPage );
					if( audioDriver != null )
						audioDriver.PageIn( oggPage );
				}

				if( bytesRead == 0 )
					endOfFile = true;
			}
		}

		public void Update( double delta )
		{
			try
			{
				criticalSection?.Enter();

				if( videoDriver == null || EndOfFile || Pause )
					return;

				lastUpdateTime = EngineApp.EngineTime;

				if( delta > 1 )
					return;

				if( audioDriver != null )
				{
					if( !audioStarted && delta != 0 )
					{
						audioDriver.StartAudioStream();
						audioStarted = true;
					}
				}

				again:;

				currentTime += delta;

				bool soundRealPlay = false;
				if( audioDriver != null )
					soundRealPlay = audioDriver.IsSoundRealPlaying();

				while( currentTime > videoDriver.GetCurrentStateTime() && !EndOfFile )
					Decode( !soundRealPlay, true );

				if( audioDriver != null && soundRealPlay )
				{
					if( currentTime > audioDriver.GetCurrentStateTime() )
						currentTime = audioDriver.GetCurrentStateTime();

					if( currentTime + .4f < audioDriver.GetCurrentStateTime() )
						goto again;
				}

				videoDriver.Blit();
			}
			catch( Exception ex )
			{
				Log.Error( "OggFile: Exception: " + ex.ToString() );
			}
			finally
			{
				criticalSection?.Leave();
			}
		}

		public bool Pause
		{
			get { return pause; }
			set
			{
				try
				{
					criticalSection?.Enter();

					pause = value;

					if( audioDriver != null )
						audioDriver.OnSetPause( pause );
				}
				catch( Exception ex )
				{
					Log.Error( "OggFile: Exception: " + ex.ToString() );
				}
				finally
				{
					criticalSection?.Leave();
				}
			}
		}

		public bool Sound3D
		{
			get { return sound3D; }
		}

		public Vector3 SoundPosition
		{
			get { return soundPosition; }
			set
			{
				try
				{
					criticalSection?.Enter();

					if( soundPosition == value )
						return;

					soundPosition = value;

					if( audioDriver != null )
						audioDriver.OnUpdateSoundPosition();
				}
				catch( Exception ex )
				{
					Log.Error( "OggFile: Exception: " + ex.ToString() );
				}
				finally
				{
					criticalSection?.Leave();
				}
			}
		}

		public double LastUpdateTime
		{
			get { return lastUpdateTime; }
		}

		public double Volume
		{
			get { return volume; }
			set
			{
				volume = value;

				if( audioDriver != null )
					audioDriver.OnSetVolume();
			}
		}

	}
}
