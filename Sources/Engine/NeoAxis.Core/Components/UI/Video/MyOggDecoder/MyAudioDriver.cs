// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NeoAxis;

using OggDecoder;

namespace MyOggDecoder
{
	class MyAudioDriver : AudioDriver
	{
		Sound sound;
		SoundVirtualChannel channel;

		//

		public MyAudioDriver()
		{
		}

		public override void Dispose()
		{
			if( channel != null )
			{
				channel.Stop();
				channel = null;
			}

			if( sound != null )
			{
				sound.Dispose();
				sound = null;
			}

			base.Dispose();
		}

		public override void StartAudioStream()
		{
			if( sound != null )
				return;

			SoundModes mode = SoundModes.Loop | SoundModes.Stream;
			int channels = vorbisInfo.channels;
			int bufferSize = soundBufferSize;
			if( oggFile.Sound3D )
			{
				mode |= SoundModes.Mode3D;
				if( vorbisInfo.channels == 2 )
				{
					channels = 1;
					bufferSize /= 2;
				}
			}

			sound = SoundWorld.SoundCreateDataBuffer( mode, channels, vorbisInfo.rate, bufferSize, ReadData );

			if( sound == null )
				return;

			//!!!!attachedToScene
			channel = SoundWorld.SoundPlay( null, sound, EngineApp.DefaultSoundChannelGroup, 1, true );

			if( channel != null )
			{
				if( oggFile.Sound3D )
					channel.Position = oggFile.SoundPosition;
				channel.Volume = oggFile.Volume;
				channel.Pause = oggFile.Pause;
			}
		}

		public override void OnSetPause( bool pause )
		{
			base.OnSetPause( pause );

			if( channel != null )
				channel.Pause = pause;
		}

		protected internal override void OnUpdateSoundPosition()
		{
			base.OnUpdateSoundPosition();

			if( channel != null && oggFile.Sound3D )
				channel.Position = oggFile.SoundPosition;
		}

		public override bool IsSoundRealPlaying()
		{
			if( channel == null )
				return false;
			return channel.CurrentRealChannel != null;
		}

		internal override void OnSetVolume()
		{
			base.OnSetVolume();

			if( channel != null )
				channel.Volume = oggFile.Volume;
		}

	}
}
