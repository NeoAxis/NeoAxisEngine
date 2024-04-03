// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Base class of all rendering effects.
	/// </summary>
	public class RenderingEffect : ResultCompile<RenderingEffect.CompiledData>// Component
	{
		/////////////////////////////////////////

		/// <summary>
		/// Attribute to configure default order of rendering effects.
		/// </summary>
		[AttributeUsage( AttributeTargets.Class )]
		public class DefaultOrderOfEffectAttribute : Attribute
		{
			double value;

			public DefaultOrderOfEffectAttribute( double value )
			{
				this.value = value;
			}

			public double Value
			{
				get { return value; }
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents a precalculated data of <see cref="RenderingEffect"/>.
		/// </summary>
		public class CompiledData : IDisposable
		{
			public virtual void Dispose()
			{
			}
		}

		/////////////////////////////////////////

		protected virtual void OnRender( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
		}

		public delegate void RenderEventDelegate( RenderingEffect sender, ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture );
		public event RenderEventDelegate RenderEvent;

		public void Render( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			OnRender( context, frameData, ref actualTexture );
			RenderEvent?.Invoke( this, context, frameData, ref actualTexture );
		}

		[Browsable( false )]
		public virtual bool LimitedDevicesSupport
		{
			get { return false; }
		}

		[Browsable( false )]
		public virtual bool IsSupported
		{
			get
			{
				if( SystemSettings.LimitedDevice && !LimitedDevicesSupport )
					return false;
				return true;
			}
		}
	}
}
