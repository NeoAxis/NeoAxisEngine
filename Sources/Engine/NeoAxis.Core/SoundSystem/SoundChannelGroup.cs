// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents a group of sound channels of the sound system.
	/// </summary>
	public class SoundChannelGroup
	{
		string name;
		internal SoundChannelGroup parent;
		List<SoundChannelGroup> children = new List<SoundChannelGroup>();

		double volume = 1;
		double pitch = 1;
		bool pause;

		object userData;

		//

		internal SoundChannelGroup( string name )
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public void AddGroup( SoundChannelGroup group )
		{
			if( group.parent != null )
				Log.Fatal( "SoundChannelGroup: AddGroup: Trying to add already attached group." );

			group.parent = this;
			children.Add( group );
		}

		public double Volume
		{
			get { return volume; }
			set
			{
				if( volume == value )
					return;

				volume = value;

				{
					SoundVirtualChannel[] channels;

					channels = SoundWorld.activeVirtual2DChannels.ToArray();
					foreach( SoundVirtualChannel virtualChannel in channels )
					{
						SoundRealChannel realChannel = virtualChannel.CurrentRealChannel;
						if( realChannel != null && virtualChannel.IsUsingGroup( this ) )
							realChannel.UpdateVolume();
					}

					channels = SoundWorld.activeVirtual3DChannels.ToArray();
					foreach( SoundVirtualChannel virtualChannel in channels )
					{
						SoundRealChannel realChannel = virtualChannel.CurrentRealChannel;
						if( realChannel != null && virtualChannel.IsUsingGroup( this ) )
							realChannel.UpdateVolume();
					}
				}
			}
		}

		public double Pitch
		{
			get { return pitch; }
			set
			{
				if( pitch == value )
					return;
				pitch = value;

				{
					SoundVirtualChannel[] channels;

					channels = SoundWorld.activeVirtual2DChannels.ToArray();
					foreach( SoundVirtualChannel virtualChannel in channels )
					{
						SoundRealChannel realChannel = virtualChannel.CurrentRealChannel;
						if( realChannel != null && virtualChannel.IsUsingGroup( this ) )
							realChannel.UpdatePitch();
					}

					channels = SoundWorld.activeVirtual3DChannels.ToArray();
					foreach( SoundVirtualChannel virtualChannel in channels )
					{
						SoundRealChannel realChannel = virtualChannel.CurrentRealChannel;
						if( realChannel != null && virtualChannel.IsUsingGroup( this ) )
							realChannel.UpdatePitch();
					}
				}
			}
		}

		public bool Pause
		{
			get { return pause; }
			set
			{
				if( pause == value )
					return;

				pause = value;

				{
					SoundVirtualChannel[] channels;

					channels = SoundWorld.activeVirtual2DChannels.ToArray();
					foreach( SoundVirtualChannel virtualChannel in channels )
					{
						if( virtualChannel.IsUsingGroup( this ) )
							SoundWorld.Internal_Instance.OnVirtualChannelUpdatePause( virtualChannel );
					}

					channels = SoundWorld.activeVirtual3DChannels.ToArray();
					foreach( SoundVirtualChannel virtualChannel in channels )
					{
						if( virtualChannel.IsUsingGroup( this ) )
							SoundWorld.Internal_Instance.OnVirtualChannelUpdatePause( virtualChannel );
					}
				}
			}
		}

		public object UserData
		{
			get { return userData; }
			set { userData = value; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double GetTotalVolume()
		{
			double v = Volume;
			if( parent != null )
				v *= parent.GetTotalVolume();
			return v;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double GetTotalPitch()
		{
			double p = Pitch;
			if( parent != null )
				p *= parent.GetTotalPitch();
			return p;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IsTotalPaused()
		{
			if( parent != null && parent.IsTotalPaused() )
				return true;
			return Pause;
		}
	}
}
