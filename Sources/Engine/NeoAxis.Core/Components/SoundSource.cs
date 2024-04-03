// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Ambient sound in the scene.
	/// </summary>
	public class SoundSource : ObjectInSpace
	{
		SoundData sound;
		SoundVirtualChannel channel;
		double replayIntervalRemainingTime;
		double lastChannelTime;

		//!!!!так?
		double lastUpdateEngineTime;

		bool playCalling;

		//

		/// <summary>
		/// Sound file to be played by the source.
		/// </summary>
		[Serialize]
		//[Category( "General" )]
		public Reference<Sound> Sound
		{
			get { if( _sound.BeginGet() ) Sound = _sound.Get( this ); return _sound.value; }
			set
			{
				if( _sound.BeginSet( this, ref value ) )
				{
					try
					{
						SoundChanged?.Invoke( this );
						Play();
					}
					finally { _sound.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Sound"/> property value changes.</summary>
		public event Action<SoundSource> SoundChanged;
		ReferenceField<Sound> _sound;

		/// <summary>
		/// Sound volume.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Serialize]
		[Range( 0, 1 )]
		//[Category( "General" )]
		public Reference<double> Volume
		{
			get { if( _volume.BeginGet() ) Volume = _volume.Get( this ); return _volume.value; }
			set
			{
				if( _volume.BeginSet( this, ref value ) )
				{
					try
					{
						VolumeChanged?.Invoke( this );
						if( channel != null )
							channel.Volume = Volume;
					}
					finally { _volume.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Volume"/> property value changes.</summary>
		public event Action<SoundSource> VolumeChanged;
		ReferenceField<double> _volume = 1.0;

		/// <summary>
		/// The playback speed of sound.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Serialize]
		[Range( 0.25, 4.0 )]
		//[Category( "General" )]
		public Reference<double> Pitch
		{
			get { if( _pitch.BeginGet() ) Pitch = _pitch.Get( this ); return _pitch.value; }
			set
			{
				if( _pitch.BeginSet( this, ref value ) )
				{
					try
					{
						PitchChanged?.Invoke( this );
						if( channel != null )
							channel.Pitch = Pitch;
					}
					finally { _pitch.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Pitch"/> property value changes.</summary>
		public event Action<SoundSource> PitchChanged;
		ReferenceField<double> _pitch = 1.0;

		/// <summary>
		/// Whether to play in 3D mode.
		/// </summary>
		[DefaultValue( true )]
		[DisplayName( "Mode 3D" )]
		public Reference<bool> Mode3D
		{
			get { if( _mode3D.BeginGet() ) Mode3D = _mode3D.Get( this ); return _mode3D.value; }
			set { if( _mode3D.BeginSet( this, ref value ) ) { try { Mode3DChanged?.Invoke( this ); Play(); } finally { _mode3D.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode3D"/> property value changes.</summary>
		public event Action<SoundSource> Mode3DChanged;
		ReferenceField<bool> _mode3D = true;

		public enum RolloffModeEnum
		{
			Logarithmic,
			Linear,
			Manual,
		}

		/// <summary>
		/// Calculation type of the damping volume depending on the distance from the camera.
		/// </summary>
		/// <remarks>
		/// There are three types: Logarithmic, Linear, manually. By using the type Manually programmer can specify attenuation formula as any spline.
		/// </remarks>
		[DefaultValue( RolloffModeEnum.Logarithmic )]
		[Serialize]
		public Reference<RolloffModeEnum> RolloffMode
		{
			get { if( _rolloffMode.BeginGet() ) RolloffMode = _rolloffMode.Get( this ); return _rolloffMode.value; }
			set
			{
				if( _rolloffMode.BeginSet( this, ref value ) )
				{
					try
					{
						RolloffModeChanged?.Invoke( this );
						UpdateChannelRolloffGraph();
					}
					finally { _rolloffMode.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="RolloffMode"/> property value changes.</summary>
		public event Action<SoundSource> RolloffModeChanged;
		ReferenceField<RolloffModeEnum> _rolloffMode = RolloffModeEnum.Logarithmic;

		/// <summary>
		/// The minimum distance from the listener, after which the sound begins to weaken.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Serialize]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> AttenuationNear
		{
			get { if( _attenuationNear.BeginGet() ) AttenuationNear = _attenuationNear.Get( this ); return _attenuationNear.value; }
			set
			{
				if( _attenuationNear.BeginSet( this, ref value ) )
				{
					try
					{
						AttenuationNearChanged?.Invoke( this );
						UpdateChannelRolloffGraph();
					}
					finally { _attenuationNear.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AttenuationNear"/> property value changes.</summary>
		public event Action<SoundSource> AttenuationNearChanged;
		ReferenceField<double> _attenuationNear = 5.0;

		/// <summary>
		/// The maximum distance from the listener, after which no sound is heard.
		/// </summary>
		[DefaultValue( 100.0 )]
		[Serialize]
		[Range( 0, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> AttenuationFar
		{
			get { if( _attenuationFar.BeginGet() ) AttenuationFar = _attenuationFar.Get( this ); return _attenuationFar.value; }
			set
			{
				if( _attenuationFar.BeginSet( this, ref value ) )
				{
					try
					{
						AttenuationFarChanged?.Invoke( this );
						UpdateChannelRolloffGraph();
					}
					finally { _attenuationFar.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AttenuationFar"/> property value changes.</summary>
		public event Action<SoundSource> AttenuationFarChanged;
		ReferenceField<double> _attenuationFar = 100.0;

		/// <summary>
		/// Damping factor for the logarithmic rolloff mode.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Serialize]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> RolloffLogarithmicFactor
		{
			get { if( _rolloffLogarithmicFactor.BeginGet() ) RolloffLogarithmicFactor = _rolloffLogarithmicFactor.Get( this ); return _rolloffLogarithmicFactor.value; }
			set
			{
				if( _rolloffLogarithmicFactor.BeginSet( this, ref value ) )
				{
					try
					{
						RolloffLogarithmicFactorChanged?.Invoke( this );
						UpdateChannelRolloffGraph();
					}
					finally { _rolloffLogarithmicFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="RolloffLogarithmicFactor"/> property value changes.</summary>
		public event Action<SoundSource> RolloffLogarithmicFactorChanged;
		ReferenceField<double> _rolloffLogarithmicFactor = 1.0;

		/// <summary>
		/// The delay between the completion of playing sound and playing it again.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Serialize]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> ReplayDelay
		{
			get { if( _replayDelay.BeginGet() ) ReplayDelay = _replayDelay.Get( this ); return _replayDelay.value; }
			set
			{
				var oldValue = _replayDelay.value;

				if( _replayDelay.BeginSet( this, ref value ) )
				{
					try
					{
						ReplayDelayChanged?.Invoke( this );

						var newValue = _replayDelay.value;

						if( replayIntervalRemainingTime > newValue )
							replayIntervalRemainingTime = newValue;

						if( EnabledInHierarchyAndIsInstance )
						{
							bool oldNonZeroDelay = oldValue != 0;
							bool nonZeroDelay = newValue != 0;
							if( oldNonZeroDelay != nonZeroDelay )
								Play();
						}
					}
					finally { _replayDelay.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReplayDelay"/> property value changes.</summary>
		public event Action<SoundSource> ReplayDelayChanged;
		ReferenceField<double> _replayDelay = 0.0;

		public enum StreamingEnum
		{
			Auto,
			Disable,
			Enable,
		}

		/// <summary>
		/// This parameter enables streaming reading from the file while playing.
		/// </summary>
		[DefaultValue( StreamingEnum.Auto )]
		[Serialize]
		public Reference<StreamingEnum> Streaming
		{
			get { if( _streaming.BeginGet() ) Streaming = _streaming.Get( this ); return _streaming.value; }
			set
			{
				if( _streaming.BeginSet( this, ref value ) )
				{
					try
					{
						StreamingChanged?.Invoke( this );
						Play();
					}
					finally { _streaming.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Streaming"/> property value changes.</summary>
		public event Action<SoundSource> StreamingChanged;
		ReferenceField<StreamingEnum> _streaming = StreamingEnum.Auto;

		/// <summary>
		/// This parameter allows you to set the priority when playing sounds.
		/// </summary>
		/// <remarks>
		/// If a lot of sounds are played at the same time, the sounds of the lowest priority will be turned off because it is impossible to play all sounds because of the limitations of sound cards.
		/// </remarks>
		[DefaultValue( 0.5 )]
		[Serialize]
		[Range( 0, 1 )]
		public Reference<double> Priority
		{
			get { if( _priority.BeginGet() ) Priority = _priority.Get( this ); return _priority.value; }
			set
			{
				if( _priority.BeginSet( this, ref value ) )
				{
					try
					{
						PriorityChanged?.Invoke( this );
						if( channel != null )
							channel.Priority = Priority;
					}
					finally { _priority.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Priority"/> property value changes.</summary>
		public event Action<SoundSource> PriorityChanged;
		ReferenceField<double> _priority = 0.5;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( RolloffMode ):
				case nameof( AttenuationNear ):
				case nameof( AttenuationFar ):
					if( !Mode3D.Value )
						skip = true;
					break;

				case nameof( RolloffLogarithmicFactor ):
					if( !Mode3D.Value || RolloffMode.Value != RolloffModeEnum.Logarithmic )
						skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
				Play();
			else
				Stop();
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			if( channel != null )
				channel.Position = Transform.Value.Position;
		}

		void Update()//float delta )
		{
			//!!!!так?
			var time = EngineApp.EngineTime;
			var delta = time - lastUpdateEngineTime;
			lastUpdateEngineTime = time;

			if( ReplayDelay != 0 )
			{
				//delay processing
				if( replayIntervalRemainingTime > 0 )
				{
					replayIntervalRemainingTime -= delta;
					if( replayIntervalRemainingTime <= 0 )
						replayIntervalRemainingTime = 0;
				}

				if( replayIntervalRemainingTime == 0 )
				{
					if( channel != null && ( channel.Stopped || ( sound.Mode & SoundModes.Stream ) != 0 && lastChannelTime > channel.Time ) )
					{
						Stop();
						replayIntervalRemainingTime = ReplayDelay;
					}
					else if( channel == null )
						Play();
				}

				////delay processing
				//if( replayIntervalRemainingTime > 0 )
				//{
				//   replayIntervalRemainingTime -= TickDelta;
				//   if( replayIntervalRemainingTime <= 0 )
				//   {
				//      replayIntervalRemainingTime = 0;
				//      PlaySound();
				//   }
				//}

				////go to delay
				//if( channel != null && channel.IsStopped() )
				//{
				//   StopSound();
				//   replayIntervalRemainingTime = replayDelay;
				//}

				////switch from looped mode to delayed mode.
				//if( channel != null && !channel.IsStopped() &&
				//   ( channel.CurrentSound.Mode & SoundMode.Loop ) != 0 )
				//{
				//   if( ( channel.Time >= channel.CurrentSound.Length - TickDelta * 0.5f ) || editor )
				//   {
				//      PlaySound();
				//   }
				//}
			}
			else
			{
				if( channel != null && channel.Stopped )
					Play();
			}

			if( channel != null )
				lastChannelTime = channel.Time;
			else
				lastChannelTime = 0;
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( EnabledInHierarchyAndIsInstance )
				Update();
			//if( EnabledInHierarchyAndIsInstance )
			//	Update( TickDelta );
		}

		void DebugDrawBorder( Viewport viewport, double distance, bool drawBorderLines )
		{
			var trans = Transform.Value;
			var pos = trans.Position;
			var rot = trans.Rotation;
			var rotMat = rot.ToMatrix3();

			var t = new Matrix4( rotMat, pos );
			double r = distance;

			const double angleStep = Math.PI / 32;
			for( double angle = 0; angle < Math.PI * 2 - angleStep * .5; angle += angleStep )
			{
				double p1sin = Math.Sin( angle );
				double p1cos = Math.Cos( angle );
				double p2sin = Math.Sin( angle + angleStep );
				double p2cos = Math.Cos( angle + angleStep );

				//может больше линий рисовать. еще под 45 градусов

				viewport.Simple3DRenderer.AddLine( t * ( new Vector3( p1cos, p1sin, 0 ) * r ), t * ( new Vector3( p2cos, p2sin, 0 ) * r ) );
				viewport.Simple3DRenderer.AddLine( t * ( new Vector3( 0, p1cos, p1sin ) * r ), t * ( new Vector3( 0, p2cos, p2sin ) * r ) );
				viewport.Simple3DRenderer.AddLine( t * ( new Vector3( p1cos, 0, p1sin ) * r ), t * ( new Vector3( p2cos, 0, p2sin ) * r ) );
			}
		}

		public void DebugDraw( Viewport viewport )
		{
			if( viewport.Simple3DRenderer != null )
			{
				var trans = Transform.Value;
				var pos = trans.Position;
				var rot = trans.Rotation;

				var scale = trans.Scale.MaxComponent();
				double near = AttenuationNear * scale;
				double far = AttenuationFar * scale;

				//!!!!так?
				//!!!!.3? может не больше какого-то числа?
				//!!!!билбордом?
				//viewport.DebugGeometry.AddSphere( Transform.Value.Position, .3 );

				{
					////AttenuationNear
					//if( near != 0 && near < far - .01f )
					//	DebugDrawBorder( viewport, near, false );

					//AttenuationFar
					if( far != 0 )
					{
						DebugDrawBorder( viewport, far, true );

						var renderer = viewport.Simple3DRenderer;
						RendererUtility.AddLineSegmented( renderer, pos - rot * new Vector3( far, 0, 0 ), pos + rot * new Vector3( far, 0, 0 ) );
						RendererUtility.AddLineSegmented( renderer, pos - rot * new Vector3( 0, far, 0 ), pos + rot * new Vector3( 0, far, 0 ) );
						RendererUtility.AddLineSegmented( renderer, pos - rot * new Vector3( 0, 0, far ), pos + rot * new Vector3( 0, 0, far ) );
					}
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EnabledInHierarchyAndIsInstance )
				Update();
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var scale = TransformV.Scale.MaxComponent();
			double far = AttenuationFar * scale;
			newBounds = new SpaceBounds( new Sphere( TransformV.Position, far ) );
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				bool show = ( context.SceneDisplayDevelopmentDataInThisApplication && ParentScene.DisplaySoundSources ) ||
					context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
				if( show )
				{
					if( context2.displaySoundSourcesCounter < context2.displaySoundSourcesMax )
					{
						context2.displaySoundSourcesCounter++;

						ColorValue color;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.SelectedColor;
						else if( context2.canSelectObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.CanSelectColor;
						else
							color = ProjectSettings.Get.Colors.SceneShowSoundSourceColor;

						var viewport = context.Owner;
						viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
						DebugDraw( viewport );
					}
				}
			}
		}

		void UpdateChannelRolloffGraph()
		{
			if( channel != null )
			{
				var scale = Transform.Value.Scale.MaxComponent();

				switch( RolloffMode.Value )
				{
				case RolloffModeEnum.Logarithmic:
					channel.SetLogarithmicRolloff( AttenuationNear * scale, AttenuationFar * scale, RolloffLogarithmicFactor );
					break;
				case RolloffModeEnum.Linear:
					channel.SetLinearRolloff( AttenuationNear * scale, AttenuationFar * scale );
					break;
				}
			}
		}

		[Browsable( false )]
		public SoundVirtualChannel Channel
		{
			get { return channel; }
		}

		protected virtual void OnBeforePlay()
		{
		}

		public delegate void BeforePlayEventDelegate( SoundSource sender );
		public event BeforePlayEventDelegate BeforePlayEvent;

		void Play()
		{
			if( SoundWorld.BackendNull )
				return;

			if( playCalling )
				return;
			playCalling = true;

			Stop();

			if( EnabledInHierarchyAndIsInstance && ParentScene != null )
			{
				var res = ParentRoot?.HierarchyController?.CreatedByResource;
				if( res != null && res.InstanceType == Resource.InstanceType.SeparateInstance )
				{
					var soundComponent = Sound.Value;
					if( soundComponent != null && soundComponent.Result != null )
					{
						bool streaming = false;

						switch( Streaming.Value )
						{
						case StreamingEnum.Auto:
							{
								string fileName = soundComponent.LoadFile.Value.ResourceName;
								if( !string.IsNullOrEmpty( fileName ) && Path.GetExtension( fileName ).ToLower() == ".ogg" )
								{
									long length = 0;
									try
									{
										length = VirtualFile.GetLength( fileName );
									}
									catch { }
									if( length > 400000 )
										streaming = true;
								}
							}
							break;

						case StreamingEnum.Enable:
							streaming = true;
							break;
						}

						SoundModes mode = 0;
						if( Mode3D )
							mode |= SoundModes.Mode3D;
						if( ReplayDelay == 0 || streaming )//if( replayDelay == 0 )
							mode |= SoundModes.Loop;
						if( streaming )
							mode |= SoundModes.Stream;

						sound = soundComponent.Result.LoadSoundByMode( mode );
						if( sound != null )
						{
							channel = SoundWorld.SoundPlay( ParentScene, sound, EngineApp.DefaultSoundChannelGroup, Priority, Volume, true );
							if( channel != null )
							{
								channel.Position = Transform.Value.Position;
								UpdateChannelRolloffGraph();
								channel.Pitch = Pitch;

								OnBeforePlay();
								BeforePlayEvent?.Invoke( this );

								channel.Pause = false;

								lastUpdateEngineTime = EngineApp.EngineTime;
							}
						}
					}
				}
			}

			playCalling = false;
		}

		void Stop()
		{
			channel?.Stop();
			channel = null;
			sound = null;
			replayIntervalRemainingTime = 0;
			lastChannelTime = 0;
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "SoundSource" );
		}
	}
}
