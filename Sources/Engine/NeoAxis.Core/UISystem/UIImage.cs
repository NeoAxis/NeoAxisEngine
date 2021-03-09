// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace NeoAxis
{
	/// <summary>
	/// Standard control that shows an image.
	/// </summary>
	//[Metadata.UserFriendlyNameForInstanceOfType( "Image" )]
	public class UIImage : UIControl
	{
		//!!!!больше возможностей

		/// <summary>
		/// The source texture of the image.
		/// </summary>
		[Serialize]
		[Category( "Image" )]
		public Reference<Component_Image> SourceImage
		{
			get { if( _sourceImage.BeginGet() ) SourceImage = _sourceImage.Get( this ); return _sourceImage.value; }
			set { if( _sourceImage.BeginSet( ref value ) ) { try { SourceImageChanged?.Invoke( this ); } finally { _sourceImage.EndSet(); } } }
		}
		public event Action<UIImage> SourceImageChanged;
		ReferenceField<Component_Image> _sourceImage;

		/// <summary>
		/// The UV coordinates of the image texture.
		/// </summary>
		[DefaultValue( "0 0 1 1" )]
		[Serialize]
		[Category( "Image" )]
		public Reference<Rectangle> TextureCoordinates
		{
			get { if( _textureCoordinates.BeginGet() ) TextureCoordinates = _textureCoordinates.Get( this ); return _textureCoordinates.value; }
			set { if( _textureCoordinates.BeginSet( ref value ) ) { try { TextureCoordinatesChanged?.Invoke( this ); } finally { _textureCoordinates.EndSet(); } } }
		}
		public event Action<UIImage> TextureCoordinatesChanged;
		ReferenceField<Rectangle> _textureCoordinates = new Rectangle( 0, 0, 1, 1 );

		/// <summary>
		/// Specifies the clamp texture address mode.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Image" )]
		public Reference<bool> Clamp
		{
			get { if( _clamp.BeginGet() ) Clamp = _clamp.Get( this ); return _clamp.value; }
			set { if( _clamp.BeginSet( ref value ) ) { try { ClampChanged?.Invoke( this ); } finally { _clamp.EndSet(); } } }
		}
		public event Action<UIImage> ClampChanged;
		ReferenceField<bool> _clamp = true;

		/// <summary>
		/// The filtering mode of the image texture.
		/// </summary>
		[DefaultValue( CanvasRenderer.TextureFilteringMode.Linear )]
		[Serialize]
		[Category( "Image" )]
		public Reference<CanvasRenderer.TextureFilteringMode> Filtering
		{
			get { if( _filtering.BeginGet() ) Filtering = _filtering.Get( this ); return _filtering.value; }
			set { if( _filtering.BeginSet( ref value ) ) { try { FilteringChanged?.Invoke( this ); } finally { _filtering.EndSet(); } } }
		}
		public event Action<UIImage> FilteringChanged;
		ReferenceField<CanvasRenderer.TextureFilteringMode> _filtering = CanvasRenderer.TextureFilteringMode.Linear;

		/////////////////////////////////////////

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );

			//bool backColorZero = backColor == new ColorValue( 0, 0, 0, 0 );

			//!!!!неправильной текстурой рисовать, чтобы видно было что ошибка? везде так
			//!!!!!заранее при загрузке?

			Component_Image texture = SourceImage;

			////!!!!?
			//GpuTexture gpuTexture = ResourceUtils.GetTextureCompiledData( texture );
			//if( gpuTexture == null )
			//	gpuTexture = ResourceUtils.GetTextureCompiledData( ResourceUtils.WhiteTexture2D );

			if( texture != null )
			{
				//Rect texCoord = backTextureCoord;

				//if( backTextureTile && texture != null )
				//{
				//	double baseHeight = UIControlsWorld.ScaleByResolutionBaseHeight;
				//	//!!!!!!slow
				//	Vec2 tileCount = new Vec2( baseHeight * renderer.AspectRatio, baseHeight ) / gpuTexture.SourceSize.ToVec2F() * GetScreenSize();
				//	texCoord = new Rect( -tileCount * .5f, tileCount * .5f ) + new Vec2( .5f, .5f );
				//}

				//var color = GetTotalColorMultiplier();
				//if( color.Alpha > 0 )
				//{
				//	color.Saturate();

				GetScreenRectangle( out var rect );

				var filtering = Filtering.Value;
				//!!!!или всегда выставлять. или добавить пункт Default.
				if( filtering == CanvasRenderer.TextureFilteringMode.Point )
					renderer.PushTextureFilteringMode( CanvasRenderer.TextureFilteringMode.Point );

				renderer.AddQuad( rect, TextureCoordinates.Value, texture, ColorValue.One, Clamp );
				//renderer.AddQuad( rect, texCoord, texture, color, backTextureTile ? false : true );

				if( filtering == CanvasRenderer.TextureFilteringMode.Point )
					renderer.PopTextureFilteringMode();
				//}
			}
		}
	}
}
