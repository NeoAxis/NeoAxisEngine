// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents a virtual sound channel of the sound system.
	/// </summary>
	public sealed class SoundVirtualChannel
	{
		internal Component_Scene attachedToScene;
		internal Sound sound;
		internal SoundChannelGroup group;
		internal Vector3 position;
		internal Vector3 velocity;
		internal double pitch = 1;
		internal double pan;
		internal double volume = 1;
		internal RolloffGraphItem[] rolloffGraph;
		internal double time;
		internal double priority = .5f;

		object userData;

		bool pause = true;
		bool stopped;
		internal SoundRealChannel currentRealChannel;

		//for SoundWorld.UpdateChannels()
		internal double tempPriority;

		///////////////////////////////////////////

		/// <summary>
		/// Represents an item for <see cref="RolloffGraph"/> method.
		/// </summary>
		public struct RolloffGraphItem
		{
			double distance;
			double gain;

			public RolloffGraphItem( double distance, double gain )
			{
				this.distance = distance;
				this.gain = gain;
			}

			public double Distance
			{
				get { return distance; }
				set { distance = value; }
			}

			public double Gain
			{
				get { return gain; }
				set { gain = value; }
			}

			public override string ToString()
			{
				return string.Format( "Distance {0} - {1}", distance.ToString( "F3" ), gain.ToString( "F3" ) );
			}
		}

		///////////////////////////////////////////

		internal SoundVirtualChannel() { }

		internal void SoundPlayInit( Component_Scene attachedToScene, Sound sound, SoundChannelGroup group )
		{
			this.attachedToScene = attachedToScene;
			this.sound = sound;
			this.group = group;

			//!!!!было
			//sound.playingCount++;
			//if( ( sound.Mode & SoundModes.Stream ) != 0 )
			//{
			//	if( sound.playingCount != 1 )
			//		Log.Fatal( "VirtualChannel: SoundPlayInit: sound.playingCount != 1." );
			//}
		}

		public void Stop()
		{
			if( Stopped )
				return;

			Pause = true;
			if( currentRealChannel != null )
				Log.Fatal( "VirtualChannel: Stop: currentRealChannel != null." );

			//!!!!было
			//data.currentSound.playingCount--;
			//if( data.currentSound.playingCount < 0 )
			//	Log.Fatal( "VirtualChannel: Stop: data.currentSound.playingCount < 0." );

			if( ( sound.Mode & SoundModes.Mode3D ) != 0 )
			{
				bool removed = SoundWorld.activeVirtual3DChannels.Remove( this );
				if( !removed )
					Log.Fatal( "VirtualChannel: Stop: !removed." );
			}
			else
			{
				bool removed = SoundWorld.activeVirtual2DChannels.Remove( this );
				if( !removed )
					Log.Fatal( "VirtualChannel: Stop: !removed." );
			}

			stopped = true;
		}

		public Component_Scene AttachedToScene
		{
			get { return attachedToScene; }
		}

		public Sound Sound
		{
			get { return sound; }
		}

		public SoundChannelGroup Group
		{
			get { return group; }
		}

		public bool Stopped
		{
			get { return stopped; }
		}

		public bool Pause
		{
			get { return pause; }
			set
			{
				if( pause == value )
					return;
				pause = value;
				SoundWorld._Instance.OnVirtualChannelUpdatePause( this );
			}
		}

		public Vector3 Position
		{
			get { return position; }
			set
			{
				if( position == value )
					return;
				position = value;
				currentRealChannel?.UpdatePosition();
			}
		}

		public Vector3 Velocity
		{
			get { return velocity; }
			set
			{
				if( velocity == value )
					return;
				velocity = value;
				currentRealChannel?.UpdateVelocity();
			}
		}

		public double Volume
		{
			get { return volume; }
			set
			{
				if( volume == value )
					return;
				volume = value;
				currentRealChannel?.UpdateVolume();
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
				currentRealChannel?.UpdatePitch();
			}
		}

		public double Pan
		{
			get { return pan; }
			set
			{
				if( pan == value )
					return;
				pan = value;
				currentRealChannel?.UpdatePan();
			}
		}

		public object UserData
		{
			get { return userData; }
			set { userData = value; }
		}

		public bool IsTotalPaused()
		{
			return pause || group.IsTotalPaused();
		}

		public double GetTotalVolume()
		{
			return volume * group.GetTotalVolume();
		}

		public double GetTotalPitch()
		{
			return pitch * group.GetTotalPitch();
		}

		public double Time
		{
			get { return time; }
			set
			{
				if( time == value )
					return;
				time = value;
				currentRealChannel?.UpdateTime();
			}
		}

		public SoundRealChannel CurrentRealChannel
		{
			get { return currentRealChannel; }
		}

		public override string ToString()
		{
			return string.Format( "{0}; {1}, Volume {2}",
				Sound.ToString(),
				Stopped ? "Stopped" : ( IsTotalPaused() ? "Pause" : "Play" ),
				GetTotalVolume().ToString( "F3" ) );
		}

		public double Priority
		{
			get { return priority; }
			set { priority = value; }
		}

		public RolloffGraphItem[] RolloffGraph
		{
			get { return rolloffGraph; }
			set { rolloffGraph = value; }
		}

		internal bool IsUsingGroup( SoundChannelGroup group )
		{
			SoundChannelGroup g = this.Group;
			while( g != null )
			{
				if( g == group )
					return true;
				g = g.parent;
			}
			return false;
		}

		public void SetLogarithmicRolloff( double minDistance, double maxDistance, double rolloffFactor )
		{
			//!!!!slowly

			if( maxDistance < minDistance )
				maxDistance = minDistance;

			int count = 10;
			if( rolloffGraph == null || rolloffGraph.Length != count )
				rolloffGraph = new RolloffGraphItem[ count ];
			rolloffGraph[ 0 ] = new RolloffGraphItem( minDistance, 1 );
			rolloffGraph[ count - 1 ] = new RolloffGraphItem( maxDistance, 0 );

			double distanceOffset = ( maxDistance - minDistance ) * .5f;
			for( int index = count - 2; index >= 1; index-- )
			{
				double distance = minDistance + distanceOffset;
				double divisor = minDistance + rolloffFactor * distanceOffset;
				double gain;
				if( divisor != 0 )
					gain = minDistance / divisor;
				else
					gain = 1;
				MathEx.Saturate( ref gain );
				rolloffGraph[ index ] = new RolloffGraphItem( distance, gain );

				distanceOffset *= .5f;
			}
		}

		public void SetLinearRolloff( double minDistance, double maxDistance )
		{
			if( maxDistance < minDistance )
				maxDistance = minDistance;

			if( rolloffGraph == null || rolloffGraph.Length != 2 )
				rolloffGraph = new RolloffGraphItem[ 2 ];
			rolloffGraph[ 0 ] = new RolloffGraphItem( minDistance, 1 );
			rolloffGraph[ 1 ] = new RolloffGraphItem( maxDistance, 0 );
		}

		public double GetRolloffFactor()
		{
			if( ( sound.Mode & SoundModes.Mode3D ) != 0 )
			{
				var graph = rolloffGraph;
				if( graph != null && graph.Length > 0 )
				{
					double distance = ( position - SoundWorld.ListenerPosition ).Length();

					if( distance <= graph[ 0 ].Distance )
						return graph[ 0 ].Gain;
					if( distance >= graph[ graph.Length - 1 ].Distance )
						return graph[ graph.Length - 1 ].Gain;

					if( graph.Length == 1 )
					{
						return graph[ 0 ].Gain;
					}
					else if( graph.Length == 2 )
					{
						//linear rolloff
						if( graph[ 0 ].Distance >= graph[ 1 ].Distance )
							return graph[ 0 ].Gain;
						double factor = ( distance - graph[ 0 ].Distance ) / ( graph[ 1 ].Distance - graph[ 0 ].Distance );
						double gain = graph[ 0 ].Gain * ( 1.0f - factor ) + graph[ 1 ].Gain * factor;
						return MathEx.Saturate( gain );
					}
					else
					{
						//spline rolloff

						//!!!!!slowly

						CubicSpline spline = new CubicSpline();

						//!!!!double

						float[] distances = new float[ graph.Length ];
						float[] gains = new float[ graph.Length ];
						for( int n = 0; n < graph.Length; n++ )
						{
							distances[ n ] = (float)graph[ n ].Distance;
							gains[ n ] = (float)graph[ n ].Gain;
						}

						double gain = spline.FitAndEval( distances, gains, (float)distance );

						//Log.Info( "GET: {0} - {1}", distance, gain );

						return MathEx.Saturate( gain );
					}
				}
			}
			return 1;
		}
	}
}
