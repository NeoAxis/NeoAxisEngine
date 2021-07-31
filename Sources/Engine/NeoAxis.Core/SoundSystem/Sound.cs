// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents a sound data of the sound system.
	/// </summary>
	public abstract class Sound : IDisposable
	{
		string name;
		SoundModes mode;
		object userData;

		double length;
		int channels;
		int frequency;

		//internal int playingCount;

		//only for small fix sorting jerking problem at the DataBuffer and Streams
		static int createIndexCounter;
		internal int createIndex;

		//

		protected void Init( string name, SoundModes mode, double length, int channels, int frequency )
		{
			this.mode = mode;

			if( name != null )
			{
				this.name = name.ToLower();
				string nameWithMode = this.name + " " + ( (int)mode & ~(int)SoundModes.Software ).ToString();
				SoundWorld.sounds.Add( nameWithMode, this );
			}

			this.length = length;
			this.channels = channels;
			this.frequency = frequency;

			createIndex = createIndexCounter;
			unchecked
			{
				createIndexCounter++;
			}
		}

		protected virtual void OnDispose() { }

		public void Dispose()
		{
			if( SoundWorld._Instance == null )
				return;

			//stop playing channels
			again:
			var activeChannels = ( ( mode & SoundModes.Mode3D ) != 0 ) ? SoundWorld.activeVirtual3DChannels : SoundWorld.activeVirtual2DChannels;
			foreach( SoundVirtualChannel virtualChannel in activeChannels )
			{
				if( virtualChannel.Sound == this )
				{
					virtualChannel.Stop();
					goto again;
				}
			}

			OnDispose();

			if( name != null )
			{
				string nameWithMode = name + " " + ( (int)mode & ~(int)SoundModes.Software ).ToString();
				SoundWorld.sounds.Remove( nameWithMode );

				//name = null;
			}

			GC.SuppressFinalize( this );
		}

		public string Name
		{
			get { return name; }
		}

		public SoundModes Mode
		{
			get { return mode; }
		}

		public double Length
		{
			get { return length; }
		}

		public int Channels
		{
			get { return channels; }
		}

		public int Frequency
		{
			get { return frequency; }
		}

		public object UserData
		{
			get { return userData; }
			set { userData = value; }
		}

		public override string ToString()
		{
			if( name != null )
			{
				string text = name;
				if( mode != 0 )
					text += " " + mode.ToString();
				return text;
			}
			else
				return "DataBuffer";
		}

		//public int PlayingCount
		//{
		//	get { return playingCount; }
		//}

		public virtual int RecordRead( byte[] buffer, int length )
		{
			return 0;
		}

		public virtual object CallCustomMethod( string message, object param ) { return null; }
	}
}
