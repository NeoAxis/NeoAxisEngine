// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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
