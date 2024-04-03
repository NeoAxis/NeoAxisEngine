// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.SharpBgfx;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Screen effect that shows internal render targets.
	/// </summary>
	[DefaultOrderOfEffect( 15 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_ShowRenderTarget : RenderingEffect
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
			set { if( _intensity.BeginSet( this, ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<RenderingEffect_ShowRenderTarget> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		//Texture
		public enum TextureType
		{
			Normal,
			Depth,
			MotionVector,
			ShadowDirectionalLightSplit1,
			ShadowDirectionalLightSplit2,
			ShadowDirectionalLightSplit3,
			ShadowDirectionalLightSplit4,
			ShadowSpotlight,
			ShadowPointLight,
			LightGrid,

			//!!!!
			//GlobalIlluminationGridAccelerated,
			//GlobalIlluminationGrid,
			//GlobalIlluminationNormal,

			//ObjectId,
		}

		/// <summary>
		/// The texture type used.
		/// </summary>
		[Serialize]
		[DefaultValue( TextureType.Normal )]
		public Reference<TextureType> Texture
		{
			get { if( _texture.BeginGet() ) Texture = _texture.Get( this ); return _texture.value; }
			set { if( _texture.BeginSet( this, ref value ) ) { try { TextureChanged?.Invoke( this ); } finally { _texture.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Texture"/> property value changes.</summary>
		public event Action<RenderingEffect_ShowRenderTarget> TextureChanged;
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
				if( _depthMultiplier.BeginSet( this, ref value ) ) { try { DepthMultiplierChanged?.Invoke( this ); } finally { _depthMultiplier.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="DepthMultiplier"/> property value changes.</summary>
		public event Action<RenderingEffect_ShowRenderTarget> DepthMultiplierChanged;
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
				if( _motionMultiplier.BeginSet( this, ref value ) ) { try { MotionMultiplierChanged?.Invoke( this ); } finally { _motionMultiplier.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="MotionMultiplier"/> property value changes.</summary>
		public event Action<RenderingEffect_ShowRenderTarget> MotionMultiplierChanged;
		ReferenceField<double> _motionMultiplier = 1000;

		/// <summary>
		/// The shadow multiplier applied.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> ShadowMultiplier
		{
			get { if( _shadowMultiplier.BeginGet() ) ShadowMultiplier = _shadowMultiplier.Get( this ); return _shadowMultiplier.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _shadowMultiplier.BeginSet( this, ref value ) ) { try { ShadowMultiplierChanged?.Invoke( this ); } finally { _shadowMultiplier.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="ShadowMultiplier"/> property value changes.</summary>
		public event Action<RenderingEffect_ShowRenderTarget> ShadowMultiplierChanged;
		ReferenceField<double> _shadowMultiplier = 1;

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
				if( p.Name == nameof( ShadowMultiplier ) && !Texture.Value.ToString().Contains( "Shadow" ) )
				{
					skip = true;
					return;
				}
			}
		}

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			if( Intensity <= 0 )
				return;

			//create final
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, PixelFormat.A8R8G8B8 );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\ShowRenderTarget_fs.sc";

				string textureName = "";
				ImageComponent texture = null;
				double multiplier = 1.0;

				switch( Texture.Value )
				{
				case TextureType.Normal:
					textureName = "normalTexture";
					break;

				case TextureType.Depth:
					textureName = "depthTexture";
					multiplier = DepthMultiplier;
					break;

				case TextureType.MotionVector:
					textureName = "motionAndObjectIdTexture";
					multiplier = MotionMultiplier;
					break;

				case TextureType.ShadowDirectionalLightSplit1:
				case TextureType.ShadowDirectionalLightSplit2:
				case TextureType.ShadowDirectionalLightSplit3:
				case TextureType.ShadowDirectionalLightSplit4:
					texture = frameData.ShadowTextureArrayDirectional;
					multiplier = ShadowMultiplier;
					break;

				case TextureType.ShadowSpotlight:
					texture = frameData.ShadowTextureArraySpot;
					multiplier = ShadowMultiplier;
					break;

				case TextureType.ShadowPointLight:
					texture = frameData.ShadowTextureArrayPoint;
					multiplier = ShadowMultiplier;
					break;

				case TextureType.LightGrid:
					if( frameData.LightGrid != null && frameData.LightsTexture != null )
						texture = frameData.LightGrid;
					break;

				//!!!!
				//case TextureType.GlobalIlluminationGridAccelerated:
				//case TextureType.GlobalIlluminationGrid:
				//case TextureType.GlobalIlluminationNormal:
				//	if( frameData.GIData != null )
				//		texture = frameData.GIData.Grid1Texture;
				//	break;

					//case TextureType.ObjectId:
					//	textureName = "motionAndObjectIdTexture";
					//	break;
				}

				ImageComponent showTexture;
				if( !string.IsNullOrEmpty( textureName ) )
					context.ObjectsDuringUpdate.namedTextures.TryGetValue( textureName, out showTexture );
				else
					showTexture = texture;

				if( showTexture == null )
				{
					context.DynamicTexture_Free( finalTexture );
					return;
				}

				//context.ObjectsDuringUpdate.namedTextures.TryGetValue( textureName, out var showTexture );
				//if( showTexture == null )
				//{
				//	context.DynamicTexture_Free( finalTexture );
				//	return;
				//}

				if( Texture.Value.ToString().Contains( "ShadowDirectionalLight" ) )
					shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "SHADOW_DIRECTIONAL_LIGHT" ) );
				if( Texture.Value.ToString().Contains( "ShadowPointLight" ) )
					shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "SHADOW_POINT_LIGHT" ) );
				if( Texture.Value.ToString().Contains( "ShadowSpotlight" ) )
					shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "SHADOW_SPOT_LIGHT" ) );
				if( Texture.Value == TextureType.LightGrid )
					shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "LIGHT_GRID" ) );
				//!!!!
				//if( Texture.Value == TextureType.GlobalIlluminationGridAccelerated || Texture.Value == TextureType.GlobalIlluminationGrid || Texture.Value == TextureType.GlobalIlluminationNormal )
				//	shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "GI_GRID" ) );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"showTexture"*/, showTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				if( Texture.Value == TextureType.LightGrid )
				{
					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2/* "s_lightsTexture"*/, frameData.LightsTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
				}

				//!!!!
				//if( Texture.Value == TextureType.GlobalIlluminationGridAccelerated || Texture.Value == TextureType.GlobalIlluminationGrid || Texture.Value == TextureType.GlobalIlluminationNormal )
				//{
				//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2/* "s_giGrid2"*/, frameData.GIData.Grid2Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				//	//var smallGrid = context.ObjectsDuringUpdate.namedTextures[ "giSmallGrid" ];
				//	//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3/* "s_giGrid2"*/, smallGrid, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
				//}

				shader.Parameters.Set( "intensity", (float)Intensity );
				shader.Parameters.Set( "nearClipDistance", (float)context.Owner.CameraSettings.NearClipDistance );
				shader.Parameters.Set( "farClipDistance", (float)context.Owner.CameraSettings.FarClipDistance );
				shader.Parameters.Set( "mode", (float)Texture.Value );
				shader.Parameters.Set( "multiplier", (float)multiplier );

				//!!!!
				//if( Texture.Value == TextureType.GlobalIlluminationGridAccelerated || Texture.Value == TextureType.GlobalIlluminationGrid || Texture.Value == TextureType.GlobalIlluminationNormal )
				//{
				//	var giData = frameData.GIData;
				//	var accelerated = Texture.Value == TextureType.GlobalIlluminationGridAccelerated;

				//	RenderingPipeline_Basic.GISetRayCastInfoUniform( context/*, shader*/, accelerated );


				//	//var array = new Vector4F[ 5 ];
				//	//array[ 0 ] = new Vector4F( giData.Cascades.Length, giData.GridSize, accelerated ? 1 : 0, 0 );

				//	//for( int cascade = 0; cascade < giData.Cascades.Length; cascade++ )
				//	//{
				//	//	var cascadeItem = giData.Cascades[ cascade ];
				//	//	var gridPosition = cascadeItem.BoundsRelative.Minimum;
				//	//	var cellSize = cascadeItem.CellSize;

				//	//	array[ 1 + cascade ] = new Vector4F( gridPosition, cellSize );
				//	//}

				//	//shader.Parameters.Set( "showRenderTargetGIData", array, ParameterType.Vector4, 5 );



				//	////unsafe
				//	////{
				//	////	var giData = frameData.GIData;

				//	////	var array = stackalloc Vector4F[ 5 ];
				//	////	array[ 0 ] = new Vector4F( giData.Cascades.Length, giData.GridSize, 0, 0 );

				//	////	for( int cascade = 0; cascade < giData.Cascades.Length; cascade++ )
				//	////	{
				//	////		var cascadeItem = giData.Cascades[ cascade ];
				//	////		var gridPosition = cascadeItem.BoundsRelative.Minimum;
				//	////		var cellSize = cascadeItem.CellSize;

				//	////		array[ 1 + cascade ] = new Vector4F( gridPosition, cellSize );
				//	////	}

				//	////	//shader.Parameters.Set( "showRenderTargetGIData", array, ParameterType.Vector4, 5 );

				//	////	var uniform = GpuProgramManager.RegisterUniform( "showRenderTargetGIData", UniformType.Vector4, 5 );
				//	////	Bgfx.SetUniform( uniform, array, 2 );
				//	////}



				//	//var cascade = 0;

				//	//////!!!!temp
				//	////if( EngineApp._DebugCapsLock && giData.Cascades.Length > 1 )
				//	////	cascade = 1;

				//	////!!!!cascades

				//	//var cascadeItem = giData.Cascades[ cascade ];

				//	//var gridPosition = cascadeItem.BoundsRelative.Minimum;
				//	//var gridSize = giData.GridSize;

				//	//var p0 = new Vector4F( gridPosition, gridSize );
				//	//var p1 = new Vector4F( cascadeItem.CellSize, cascade, 0, 0 );

				//	//shader.Parameters.Set( "showRenderTargetGI0", p0 );
				//	//shader.Parameters.Set( "showRenderTargetGI1", p1 );
				//}

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old textures
			context.DynamicTexture_Free( actualTexture );

			//update actual texture
			actualTexture = finalTexture;
		}

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}
	}
}
