// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents a virtual sound channel of the sound system.
	/// </summary>
	public sealed class SoundVirtualChannel
	{
		internal Scene attachedToScene;
		internal SoundData sound;
		internal SoundChannelGroup group;
		internal double startTime;

		internal Vector3 position;
		internal Vector3 velocity;
		internal double pitch = 1;
		internal double pan;
		internal double volume = 1;
		internal Curve1 rolloffGraph;//internal RolloffGraphItem[] rolloffGraph;
		internal double time;
		internal double priority = .5f;

		object userData;

		bool pause = true;
		bool stopped;
		internal SoundRealChannel currentRealChannel;

		//for SoundWorld.UpdateChannels()
		internal double tempPriority;

		///////////////////////////////////////////

		///// <summary>
		///// Represents an item for <see cref="RolloffGraph"/> method.
		///// </summary>
		//public struct RolloffGraphItem
		//{
		//	double distance;
		//	double gain;

		//	public RolloffGraphItem( double distance, double gain )
		//	{
		//		this.distance = distance;
		//		this.gain = gain;
		//	}

		//	public double Distance
		//	{
		//		get { return distance; }
		//		set { distance = value; }
		//	}

		//	public double Gain
		//	{
		//		get { return gain; }
		//		set { gain = value; }
		//	}

		//	public override string ToString()
		//	{
		//		return string.Format( "Distance {0} - {1}", distance.ToString( "F3" ), gain.ToString( "F3" ) );
		//	}
		//}

		///////////////////////////////////////////

		internal SoundVirtualChannel() { }

		internal void SoundPlayInit( Scene attachedToScene, SoundData sound, SoundChannelGroup group )
		{
			this.attachedToScene = attachedToScene;
			this.sound = sound;
			this.group = group;
			startTime = EngineApp.EngineTime;

			if( attachedToScene != null && ( sound.Mode & SoundModes.Mode3D ) != 0 )
			{
				if( attachedToScene.soundDefaultRolloffGraph == null )
				{
					var minDistance = attachedToScene.SoundAttenuationNear.Value;
					var maxDistance = attachedToScene.SoundAttenuationFar.Value;
					var rolloffFactor = attachedToScene.SoundRolloffFactor.Value;

					if( maxDistance < minDistance )
						maxDistance = minDistance;

					int count = 10;

					attachedToScene.soundDefaultRolloffGraph = new CurveCubicSpline1( count );
					for( int n = 0; n < count; n++ )
						attachedToScene.soundDefaultRolloffGraph.AddPoint( 0, 0 );

					attachedToScene.soundDefaultRolloffGraph.Points[ 0 ] = new Curve1.Point( minDistance, 1 );
					attachedToScene.soundDefaultRolloffGraph.Points[ count - 1 ] = new Curve1.Point( maxDistance, 0 );

					var distanceOffset = ( maxDistance - minDistance ) * 0.5;
					for( int index = count - 2; index >= 1; index-- )
					{
						var distance = minDistance + distanceOffset;
						var divisor = minDistance + rolloffFactor * distanceOffset;
						double gain;
						if( divisor != 0 )
							gain = minDistance / divisor;
						else
							gain = 1;
						MathEx.Saturate( ref gain );
						attachedToScene.soundDefaultRolloffGraph.Points[ index ] = new Curve1.Point( distance, gain );

						distanceOffset *= 0.5;
					}


					//int count = 10;
					//if( attachedToScene.soundDefaultRolloffGraph == null || attachedToScene.soundDefaultRolloffGraph.Length != count )
					//	attachedToScene.soundDefaultRolloffGraph = new RolloffGraphItem[ count ];
					//attachedToScene.soundDefaultRolloffGraph[ 0 ] = new RolloffGraphItem( minDistance, 1 );
					//attachedToScene.soundDefaultRolloffGraph[ count - 1 ] = new RolloffGraphItem( maxDistance, 0 );

					//double distanceOffset = ( maxDistance - minDistance ) * 0.5;
					//for( int index = count - 2; index >= 1; index-- )
					//{
					//	double distance = minDistance + distanceOffset;
					//	double divisor = minDistance + rolloffFactor * distanceOffset;
					//	double gain;
					//	if( divisor != 0 )
					//		gain = minDistance / divisor;
					//	else
					//		gain = 1;
					//	MathEx.Saturate( ref gain );
					//	attachedToScene.soundDefaultRolloffGraph[ index ] = new RolloffGraphItem( distance, gain );

					//	distanceOffset *= 0.5;
					//}
				}

				RolloffGraph = attachedToScene.soundDefaultRolloffGraph;
			}

			//!!!!было
			//sound.playingCount++;
			//if( ( sound.Mode & SoundModes.Stream ) != 0 )
			//{
			//	if( sound.playingCount != 1 )
			//		Log.Fatal( "SoundVirtualChannel: SoundPlayInit: sound.playingCount != 1." );
			//}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Stop()
		{
			if( Stopped )
				return;

			Pause = true;
			if( currentRealChannel != null )
				Log.Fatal( "SoundVirtualChannel: Stop: currentRealChannel != null." );

			//!!!!было
			//data.currentSound.playingCount--;
			//if( data.currentSound.playingCount < 0 )
			//	Log.Fatal( "SoundVirtualChannel: Stop: data.currentSound.playingCount < 0." );

			if( ( sound.Mode & SoundModes.Mode3D ) != 0 )
			{
				bool removed = SoundWorld.activeVirtual3DChannels.Remove( this );
				if( !removed )
					Log.Fatal( "SoundVirtualChannel: Stop: !removed." );
			}
			else
			{
				bool removed = SoundWorld.activeVirtual2DChannels.Remove( this );
				if( !removed )
					Log.Fatal( "SoundVirtualChannel: Stop: !removed." );
			}

			stopped = true;
		}

		public Scene AttachedToScene
		{
			get { return attachedToScene; }
		}

		public SoundData Sound
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
				SoundWorld.Internal_Instance.OnVirtualChannelUpdatePause( this );
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IsTotalPaused()
		{
			return pause || group.IsTotalPaused();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double GetTotalVolume()
		{
			return volume * group.GetTotalVolume();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

		public Curve1 RolloffGraph
		{
			get { return rolloffGraph; }
			set { rolloffGraph = value; }
		}

		//public RolloffGraphItem[] RolloffGraph
		//{
		//	get { return rolloffGraph; }
		//	set { rolloffGraph = value; }
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

		[MethodImpl( (MethodImplOptions)512 )]
		public void SetLogarithmicRolloff( double minDistance, double maxDistance, double rolloffFactor )
		{
			if( maxDistance < minDistance )
				maxDistance = minDistance;

			int count = 10;

			rolloffGraph = new CurveCubicSpline1( count );
			for( int n = 0; n < count; n++ )
				rolloffGraph.AddPoint( 0, 0 );

			rolloffGraph.Points[ 0 ] = new Curve1.Point( minDistance, 1 );
			rolloffGraph.Points[ count - 1 ] = new Curve1.Point( maxDistance, 0 );

			var distanceOffset = ( maxDistance - minDistance ) * 0.5;
			for( int index = count - 2; index >= 1; index-- )
			{
				var distance = minDistance + distanceOffset;
				var divisor = minDistance + rolloffFactor * distanceOffset;
				double gain;
				if( divisor != 0 )
					gain = minDistance / divisor;
				else
					gain = 1;
				MathEx.Saturate( ref gain );
				rolloffGraph.Points[ index ] = new Curve1.Point( distance, gain );

				distanceOffset *= 0.5;
			}

			//int count = 10;
			//if( rolloffGraph == null || rolloffGraph.Length != count )
			//	rolloffGraph = new RolloffGraphItem[ count ];
			//rolloffGraph[ 0 ] = new RolloffGraphItem( minDistance, 1 );
			//rolloffGraph[ count - 1 ] = new RolloffGraphItem( maxDistance, 0 );

			//double distanceOffset = ( maxDistance - minDistance ) * 0.5;
			//for( int index = count - 2; index >= 1; index-- )
			//{
			//	double distance = minDistance + distanceOffset;
			//	double divisor = minDistance + rolloffFactor * distanceOffset;
			//	double gain;
			//	if( divisor != 0 )
			//		gain = minDistance / divisor;
			//	else
			//		gain = 1;
			//	MathEx.Saturate( ref gain );
			//	rolloffGraph[ index ] = new RolloffGraphItem( distance, gain );

			//	distanceOffset *= 0.5;
			//}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void SetLinearRolloff( double minDistance, double maxDistance )
		{
			if( maxDistance < minDistance )
				maxDistance = minDistance;

			rolloffGraph = new CurveLine1();
			rolloffGraph.AddPoint( minDistance, 1 );
			rolloffGraph.AddPoint( maxDistance, 0 );

			//if( rolloffGraph == null || rolloffGraph.Length != 2 )
			//	rolloffGraph = new RolloffGraphItem[ 2 ];
			//rolloffGraph[ 0 ] = new RolloffGraphItem( minDistance, 1 );
			//rolloffGraph[ 1 ] = new RolloffGraphItem( maxDistance, 0 );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double GetRolloffFactor()
		{
			if( ( sound.Mode & SoundModes.Mode3D ) != 0 )
			{
				var graph = rolloffGraph;
				if( graph != null )
				{
					var points = graph.Points;
					if( points.Count > 0 )
					{
						var distance = ( position - SoundWorld.ListenerPosition ).Length();

						if( distance <= points[ 0 ].Time )
							return points[ 0 ].Value;
						if( distance >= points[ points.Count - 1 ].Time )
							return points[ points.Count - 1 ].Value;

						return MathEx.Saturate( graph.CalculateValueByTime( distance ) );
					}
				}


				//if( graph != null && graph.Length > 0 )
				//{
				//	var distance = ( position - SoundWorld.ListenerPosition ).Length();

				//	if( distance <= graph[ 0 ].Distance )
				//		return graph[ 0 ].Gain;
				//	if( distance >= graph[ graph.Length - 1 ].Distance )
				//		return graph[ graph.Length - 1 ].Gain;

				//	if( graph.Length == 1 )
				//	{
				//		return graph[ 0 ].Gain;
				//	}
				//	else if( graph.Length == 2 )
				//	{
				//		//linear rolloff
				//		if( graph[ 0 ].Distance >= graph[ 1 ].Distance )
				//			return graph[ 0 ].Gain;
				//		var factor = ( distance - graph[ 0 ].Distance ) / ( graph[ 1 ].Distance - graph[ 0 ].Distance );
				//		var gain = graph[ 0 ].Gain * ( 1.0f - factor ) + graph[ 1 ].Gain * factor;
				//		return MathEx.Saturate( gain );
				//	}
				//	else
				//	{
				//		//spline rolloff

				//		//!!!!!slowly

				//		var curve = new CurveCubicSpline1();

				//		for( int n = 0; n < graph.Length; n++ )
				//		{
				//			ref var item = ref graph[ n ];
				//			curve.Points.Add( new Curve1.Point( item.Distance, item.Gain ) );
				//		}

				//		var gain = curve.CalculateValueByTime( distance );
				//		return MathEx.Saturate( gain );



				//		//CubicSpline spline = new CubicSpline();

				//		//float[] distances = new float[ graph.Length ];
				//		//float[] gains = new float[ graph.Length ];
				//		//for( int n = 0; n < graph.Length; n++ )
				//		//{
				//		//	distances[ n ] = (float)graph[ n ].Distance;
				//		//	gains[ n ] = (float)graph[ n ].Gain;
				//		//}

				//		//double gain = spline.FitAndEval( distances, gains, (float)distance );

				//		////Log.Info( "GET: {0} - {1}", distance, gain );

				//		//return MathEx.Saturate( gain );
				//	}
				//}
			}

			return 1;
		}
	}
}
