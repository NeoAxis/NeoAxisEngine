// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Represents mesh skeleton animation. Defines when and which animation track will be used by skeleton.
	/// </summary>
	public class Component_SkeletonAnimation : Component_Animation
	{
		//[DefaultValue( null )]
		/// <summary>
		/// Reference to the track with animation data.
		/// </summary>
		[Serialize]
		public Reference<Component_SkeletonAnimationTrack> Track
		{
			get { if( _track.BeginGet() ) Track = _track.Get( this ); return _track.value; }
			set { if( _track.BeginSet( ref value ) ) { try { TrackChanged?.Invoke( this ); } finally { _track.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Track"/> property value changes.</summary>
		public event Action<Component_SkeletonAnimation> TrackChanged;
		ReferenceField<Component_SkeletonAnimationTrack> _track;

		//[DefaultValue( 0.0 )]
		/// <summary>
		/// Start time of a track.
		/// </summary>
		[Serialize]
		public Reference<double> TrackStartTime
		{
			get { if( _trackStartTime.BeginGet() ) TrackStartTime = _trackStartTime.Get( this ); return _trackStartTime.value; }
			set { if( _trackStartTime.BeginSet( ref value ) ) { try { TrackStartTimeChanged?.Invoke( this ); } finally { _trackStartTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrackStartTime"/> property value changes.</summary>
		public event Action<Component_SkeletonAnimation> TrackStartTimeChanged;
		ReferenceField<double> _trackStartTime;
	}
}
