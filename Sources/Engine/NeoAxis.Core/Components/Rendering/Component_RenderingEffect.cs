// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Base class of all rendering effects.
	/// </summary>
	public class Component_RenderingEffect : Component_ResultCompile<Component_RenderingEffect.CompiledData>// Component
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
		/// Represents a precalculated data of <see cref="Component_RenderingEffect"/>.
		/// </summary>
		public class CompiledData : IDisposable
		{
			public virtual void Dispose()
			{
			}
		}

		/////////////////////////////////////////

		protected virtual void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
		}

		public delegate void RenderEventDelegate( Component_RenderingEffect sender, ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture );
		public event RenderEventDelegate RenderEvent;

		public void Render( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			OnRender( context, frameData, ref actualTexture );
			RenderEvent?.Invoke( this, context, frameData, ref actualTexture );
		}

		//!!!!
		[Browsable( false )]
		public virtual bool LimitedDevicesSupport
		{
			get { return false; }
		}

		//!!!!
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
