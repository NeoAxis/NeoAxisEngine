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
	/// Screen effect that shows internal render targets.
	/// </summary>
	[DefaultOrderOfEffect( 14 )]
	public class Component_RenderingEffect_ShowRenderTarget : Component_RenderingEffect
	{
		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Effect" )]
		public Reference<double> Intensity
		{
			get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
			set { if( _intensity.BeginSet( ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ShowRenderTarget> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		//Texture
		public enum TextureType
		{
			Normal,
			Depth,
			MotionVector,
		}

		/// <summary>
		/// The texture type used.
		/// </summary>
		[Serialize]
		[DefaultValue( TextureType.Normal )]
		public Reference<TextureType> Texture
		{
			get { if( _texture.BeginGet() ) Texture = _texture.Get( this ); return _texture.value; }
			set { if( _texture.BeginSet( ref value ) ) { try { TextureChanged?.Invoke( this ); } finally { _texture.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Texture"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ShowRenderTarget> TextureChanged;
		ReferenceField<TextureType> _texture = TextureType.Normal;

		/// <summary>
		/// The depth multiplier applied.
		/// </summary>
		[Serialize]
		[DefaultValue( 10.0 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> DepthMultiplier
		{
			get { if( _depthMultiplier.BeginGet() ) DepthMultiplier = _depthMultiplier.Get( this ); return _depthMultiplier.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _depthMultiplier.BeginSet( ref value ) ) { try { DepthMultiplierChanged?.Invoke( this ); } finally { _depthMultiplier.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="DepthMultiplier"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ShowRenderTarget> DepthMultiplierChanged;
		ReferenceField<double> _depthMultiplier = 10;

		/// <summary>
		/// The motion vector multiplier applied.
		/// </summary>
		[Serialize]
		[DefaultValue( 1000.0 )]
		[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> MotionMultiplier
		{
			get { if( _motionMultiplier.BeginGet() ) MotionMultiplier = _motionMultiplier.Get( this ); return _motionMultiplier.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _motionMultiplier.BeginSet( ref value ) ) { try { MotionMultiplierChanged?.Invoke( this ); } finally { _motionMultiplier.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="MotionMultiplier"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ShowRenderTarget> MotionMultiplierChanged;
		ReferenceField<double> _motionMultiplier = 1000;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				if( p.Name == nameof( DepthMultiplier ) && Texture.Value != TextureType.Depth )
				{
					skip = true;
					return;
				}
				if( p.Name == nameof( MotionMultiplier ) && Texture.Value != TextureType.MotionVector )
				{
					skip = true;
					return;
				}
			}
		}

		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			//create final
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, PixelFormat.A8R8G8B8 );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\ShowRenderTarget_fs.sc";

				string textureName = "";
				switch( Texture.Value )
				{
				case TextureType.Normal: textureName = "normalTexture"; break;
				case TextureType.Depth: textureName = "depthTexture"; break;
				case TextureType.MotionVector: textureName = "motionTexture"; break;
				}
				context.objectsDuringUpdate.namedTextures.TryGetValue( textureName, out var showTexture );
				if( showTexture == null )
				{
					context.DynamicTexture_Free( finalTexture );
					return;
				}

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"showTexture"*/, showTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				shader.Parameters.Set( "intensity", (float)Intensity );
				shader.Parameters.Set( "nearClipDistance", (float)context.Owner.CameraSettings.NearClipDistance );
				shader.Parameters.Set( "farClipDistance", (float)context.Owner.CameraSettings.FarClipDistance );
				shader.Parameters.Set( "mode", (float)Texture.Value );
				shader.Parameters.Set( "depthMultiplier", (float)DepthMultiplier );
				shader.Parameters.Set( "motionMultiplier", (float)MotionMultiplier );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old textures
			context.DynamicTexture_Free( actualTexture );

			//update actual texture
			actualTexture = finalTexture;
		}
	}
}
