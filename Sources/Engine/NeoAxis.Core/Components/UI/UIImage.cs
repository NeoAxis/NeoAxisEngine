// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

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
		public Reference<ImageComponent> SourceImage
		{
			get { if( _sourceImage.BeginGet() ) SourceImage = _sourceImage.Get( this ); return _sourceImage.value; }
			set { if( _sourceImage.BeginSet( this, ref value ) ) { try { SourceImageChanged?.Invoke( this ); } finally { _sourceImage.EndSet(); } } }
		}
		public event Action<UIImage> SourceImageChanged;
		ReferenceField<ImageComponent> _sourceImage;

		/// <summary>
		/// The UV coordinates of the image texture.
		/// </summary>
		[DefaultValue( "0 0 1 1" )]
		[Serialize]
		[Category( "Image" )]
		public Reference<Rectangle> TextureCoordinates
		{
			get { if( _textureCoordinates.BeginGet() ) TextureCoordinates = _textureCoordinates.Get( this ); return _textureCoordinates.value; }
			set { if( _textureCoordinates.BeginSet( this, ref value ) ) { try { TextureCoordinatesChanged?.Invoke( this ); } finally { _textureCoordinates.EndSet(); } } }
		}
		public event Action<UIImage> TextureCoordinatesChanged;
		ReferenceField<Rectangle> _textureCoordinates = new Rectangle( 0, 0, 1, 1 );

		/// <summary>
		/// The rotation of UV coordinates around the center.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 360 )]
		public Reference<Degree> TextureCoordinatesRotation
		{
			get { if( _textureCoordinatesRotation.BeginGet() ) TextureCoordinatesRotation = _textureCoordinatesRotation.Get( this ); return _textureCoordinatesRotation.value; }
			set { if( _textureCoordinatesRotation.BeginSet( this, ref value ) ) { try { TextureCoordinatesRotationChanged?.Invoke( this ); } finally { _textureCoordinatesRotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextureCoordinatesRotation"/> property value changes.</summary>
		public event Action<UIImage> TextureCoordinatesRotationChanged;
		ReferenceField<Degree> _textureCoordinatesRotation = new Degree( 0 );

		/// <summary>
		/// Specifies the clamp texture address mode.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Image" )]
		public Reference<bool> Clamp
		{
			get { if( _clamp.BeginGet() ) Clamp = _clamp.Get( this ); return _clamp.value; }
			set { if( _clamp.BeginSet( this, ref value ) ) { try { ClampChanged?.Invoke( this ); } finally { _clamp.EndSet(); } } }
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
			set { if( _filtering.BeginSet( this, ref value ) ) { try { FilteringChanged?.Invoke( this ); } finally { _filtering.EndSet(); } } }
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

			ImageComponent texture = SourceImage;

			////!!!!?
			//GpuTexture gpuTexture = ResourceUtils.GetTextureCompiledData( texture );
			//if( gpuTexture == null )
			//	gpuTexture = ResourceUtils.GetTextureCompiledData( ResourceUtils.WhiteTexture2D );

			if( texture != null && !texture.Disposed )
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

				var texCoords = TextureCoordinates.Value;
				var rotation = TextureCoordinatesRotation.Value;

				if( rotation != 0 )
				{
					var rectF = rect.ToRectangleF();
					var texCoordsF = texCoords.ToRectangleF();

					var vertices = new CanvasRenderer.TriangleVertex[ 4 ];

					ref var v0 = ref vertices[ 0 ];
					v0.position = rectF.LeftTop;
					v0.texCoord = texCoordsF.LeftTop;
					ref var v1 = ref vertices[ 1 ];
					v1.position = rectF.RightTop;
					v1.texCoord = texCoordsF.RightTop;
					ref var v2 = ref vertices[ 2 ];
					v2.position = rectF.RightBottom;
					v2.texCoord = texCoordsF.RightBottom;
					ref var v3 = ref vertices[ 3 ];
					v3.position = rectF.LeftBottom;
					v3.texCoord = texCoordsF.LeftBottom;

					var texCoordsCenter = texCoordsF.GetCenter();
					var matrix = Matrix2F.FromRotate( rotation.InRadians().ToRadianF() );

					for( int n = 0; n < 4; n++ )
					{
						ref var v = ref vertices[ n ];

						v.color = ColorValue.One;

						v.texCoord -= texCoordsCenter;
						v.texCoord = matrix * v.texCoord;
						v.texCoord += texCoordsCenter;
					}

					var indices = new int[] { 0, 1, 2, 2, 3, 0 };
					renderer.AddTriangles( vertices, indices, texture, Clamp );
				}
				else
					renderer.AddQuad( rect, texCoords, texture, ColorValue.One, Clamp );

				//renderer.AddQuad( rect, texCoord, texture, color, backTextureTile ? false : true );

				if( filtering == CanvasRenderer.TextureFilteringMode.Point )
					renderer.PopTextureFilteringMode();
				//}
			}
		}
	}
}
