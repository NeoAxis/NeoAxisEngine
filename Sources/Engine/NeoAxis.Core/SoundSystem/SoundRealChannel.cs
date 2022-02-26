// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents a real sound channel of the sound system.
	/// </summary>
	public abstract class SoundRealChannel
	{
		internal int index;
		internal bool is3D;

		internal SoundVirtualChannel currentVirtualChannel;

		//

		public int Index
		{
			get { return index; }
		}

		public bool Is3D
		{
			get { return is3D; }
		}

		public SoundVirtualChannel CurrentVirtualChannel
		{
			get { return currentVirtualChannel; }
		}

		protected internal abstract void PostAttachVirtualChannel();
		protected internal abstract void PreDetachVirtualChannel();

		protected internal abstract void UpdatePosition();
		protected internal abstract void UpdateVelocity();
		protected internal abstract void UpdateVolume();
		//protected internal abstract void UpdateMinDistance();
		protected internal abstract void UpdatePitch();
		protected internal abstract void UpdatePan();
		protected internal abstract void UpdateTime();

		public override string ToString()
		{
			string s = Index.ToString() + ( Is3D ? "3D" : "2D" ) + ". ";

			if( CurrentVirtualChannel != null )
				s += CurrentVirtualChannel.ToString();
			else
				s += "Free";

			return s;
		}

		public virtual object CallCustomMethod( string message, object param ) { return null; }
	}
}
