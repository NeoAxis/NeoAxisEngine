// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using Internal.SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// This class represents a RenderTarget that renders to a <see cref="GpuTexture"/>.
	/// </summary>
	public class RenderTexture : RenderTarget
	{
		GpuTexture creator;

		//

		internal RenderTexture( FrameBuffer frameBuffer, Vector2I size, GpuTexture creator )
			: base( frameBuffer, true, size )
		{
			this.creator = creator;
		}

		public GpuTexture Creator
		{
			get { return creator; }
		}
	}
}
