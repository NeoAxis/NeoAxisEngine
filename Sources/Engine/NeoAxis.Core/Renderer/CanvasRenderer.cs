// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Internal.SharpBgfx;

namespace NeoAxis
{
	//!!!!рисовать графиком количество создаваемого и освобождаемого. количество Lock и т.д. будет наглядно видно где-то что-то не так
	//!!!!!3d gui texture filtering

	//!!!!!double

	/// <summary>
	/// Specifies the horizontal alignment.
	/// </summary>
	public enum EHorizontalAlignment
	{
		/// <summary>The element is aligned to the left side.</summary>
		Left,
		/// <summary>The element is centered horizontally.</summary>
		Center,
		/// <summary>The element is aligned to the right side.</summary>
		Right,
		//!!!!new
		Stretch
	}

	/// <summary>
	/// Specifies the vertical alignment.
	/// </summary>
	public enum EVerticalAlignment
	{
		/// <summary>The element is aligned to the top.</summary>
		Top,
		/// <summary>The element is centered vertically.</summary>
		Center,
		/// <summary>The element is aligned to the bottom.</summary>
		Bottom,
		//!!!!new
		Stretch
	}

	/////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies a class that allows to draw 2D graphic elements.
	/// </summary>
	public abstract class CanvasRenderer
	{
		//public static float DefaultFontSize = .02f;

		////////////////////////////////////////

		/// <summary>
		/// Enumerates possible blending methods.
		/// </summary>
		public enum BlendingType
		{
			AlphaBlend,
			AlphaAdd,//!!!!?
			Opaque,
			Add
		}

		////////////////////////////////////////

		/// <summary>
		/// Enumerates possible filtering modes on texture samplers.
		/// </summary>
		public enum TextureFilteringMode
		{
			Point,
			Linear,
		}

		///////////////////////////////////////////

		/// <summary>
		/// Represents triangle vertex data for <see cref="CanvasRenderer"/>.
		/// </summary>
		[StructLayout( LayoutKind.Sequential )]
		public struct TriangleVertex
		{
			public Vector2F position;
			public ColorValue color;
			public Vector2F texCoord;

			//!!!!можно его сделать развитым. в юнити normal, tangent, uv0, uv1
			//!!!!!еще про смешивание. хотя вероятно это надо обуздать что максимум можно извлечь из шейдеров. + еще всякие размытия

			//

			public TriangleVertex( Vector2F position, ColorValue color, Vector2F texCoord )
			{
				this.position = position;
				this.color = color;
				this.texCoord = texCoord;
			}

			public TriangleVertex( Vector2F position, ColorValue color )
			{
				this.position = position;
				this.color = color;
				this.texCoord = Vector2F.Zero;
			}
		}

		////////////////////////////////////////

		/// <summary>
		/// Represents line data for <see cref="CanvasRenderer"/>.
		/// </summary>
		[StructLayout( LayoutKind.Sequential )]
		public struct LineItem
		{
			public Vector2F start;
			public Vector2F end;
			public ColorValue color;

			public LineItem( Vector2F start, Vector2F end, ColorValue color )
			{
				this.start = start;
				this.end = end;
				this.color = color;
			}
		}

		///////////////////////////////////////////

		/// <summary>
		/// Represents the data for a canvas visualization customization with its defines and parameters.
		/// </summary>
		public class ShaderItem
		{
			public string VertexProgramFileName = @"Base\Shaders\CanvasRenderer_vs.sc";
			public string FragmentProgramFileName = @"Base\Shaders\CanvasRenderer_fs.sc";

			public List<DefineItem> defines;
			//public DefineItem[] Defines;
			public ParameterContainer Parameters = new ParameterContainer();

			public GpuProgram CompiledVertexProgram;
			public GpuProgram CompiledFragmentProgram;
			public List<ParameterContainer> additionalParameterContainers;
			//public List<ParameterContainer> AdditionalParameterContainers = new List<ParameterContainer>();

			////////////

			/// <summary>
			/// Represents a compilation definition for <see cref="ShaderItem"/>.
			/// </summary>
			public struct DefineItem
			{
				public string Name;
				public string Value;

				//

				public DefineItem( string name, string value = null )
				{
					this.Name = name;
					this.Value = value;
				}

				//public string Name
				//{
				//	get { return name; }
				//	set { name = value; }
				//}

				//public string Value
				//{
				//	get { return this.value; }
				//	set { this.value = value; }
				//}
			}

			////////////

			public ShaderItem()
			{
			}

			//public string VertexProgramFileName
			//{
			//	get { return vertexProgramFileName; }
			//	set { vertexProgramFileName = value; }
			//}

			//public string FragmentProgramFileName
			//{
			//	get { return fragmentProgramFileName; }
			//	set { fragmentProgramFileName = value; }
			//}

			//public DefineItem[] Defines
			//{
			//	get { return defines; }
			//	set { defines = value; }
			//}

			//public ParameterContainer Parameters
			//{
			//	get { return parameters; }
			//	set { parameters = value; }
			//}

			//public GpuProgram CompiledVertexProgram
			//{
			//	get { return compiledVertexProgram; }
			//	set { compiledVertexProgram = value; }
			//}

			//public GpuProgram CompiledFragmentProgram
			//{
			//	get { return compiledFragmentProgram; }
			//	set { compiledFragmentProgram = value; }
			//}

			//public List<ParameterContainer> AdditionalParameterContainers
			//{
			//	get { return additionalParameterContainers; }
			//	set { additionalParameterContainers = value; }
			//}

			//public bool EnableDefaultBindingBehaviorOfFirstTexture
			//{
			//	get { return enableDefaultBindingBehaviorOfFirstTexture; }
			//	set { enableDefaultBindingBehaviorOfFirstTexture = value; }
			//}

			public List<DefineItem> Defines
			{
				get
				{
					if( defines == null )
						defines = new List<DefineItem>();
					return defines;
				}
			}

			internal bool DefinesExists
			{
				get { return defines != null; }
			}

			public List<ParameterContainer> AdditionalParameterContainers
			{
				get
				{
					if( additionalParameterContainers == null )
						additionalParameterContainers = new List<ParameterContainer>();
					return additionalParameterContainers;
				}
			}

			internal bool AdditionalParameterContainersExists
			{
				get { return additionalParameterContainers != null; }
			}
		}

		///////////////////////////////////////////

		public abstract float AspectRatio
		{
			get;
		}
		public abstract float AspectRatioInv
		{
			get;
		}

		/// <summary>
		/// Gets or sets the default font.
		/// </summary>
		public abstract FontComponent DefaultFont
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the default font.
		/// </summary>
		public abstract double DefaultFontSize
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets value which indicates what is it renderer is screen renderer.
		/// </summary>
		public abstract bool IsScreen
		{
			get;
		}

		public abstract Viewport ViewportForScreenCanvasRenderer
		{
			get;
		}

		/////////////////////////////////////////

		////!!!!такое как Push/Pop надо делать
		////!!!!еще вращение

		//public abstract bool _OutGeometryTransformEnabled
		//{
		//	get;
		//	set;
		//}

		//public abstract Vec2F _OutGeometryTransformScale
		//{
		//	get;
		//	set;
		//}

		//public abstract Vec2F _OutGeometryTransformOffset
		//{
		//	get;
		//	set;
		//}

		/////////////////////////////////////////

		public abstract void PushClipRectangle( RectangleF clipRectangle );
		public void PushClipRectangle( Rectangle clipRectangle )
		{
			PushClipRectangle( clipRectangle.ToRectangleF() );
		}
		public abstract void PopClipRectangle();

		public abstract void PushShader( ShaderItem shader );
		public abstract void PopShader();

		public abstract void PushTextureFilteringMode( TextureFilteringMode mode );
		public abstract void PopTextureFilteringMode();

		public abstract void PushBlendingType( BlendingType blendingType );
		public abstract void PopBlendingType();

		public abstract void PushColorMultiplier( ColorValue color );
		public abstract void PopColorMultiplier();

		/////////////////////////////////////////

		public abstract void GetCurrentClipRectangle( out RectangleF result );
		public abstract ShaderItem GetCurrentShader();
		public abstract TextureFilteringMode GetCurrentTextureFilteringMode();
		public abstract BlendingType GetCurrentBlendingType();
		public abstract ColorValue GetCurrentColorMultiplier();

		/////////////////////////////////////////

		public abstract void ViewportRendering_RenderToCurrentViewport( ViewportRenderingContext context, bool clearData, double time );

		/////////////////////////////////////////

		/// <summary>
		/// Adds quad to rendering queue.
		/// </summary>
		/// <param name="rectangle">The quad rectangle.</param>
		/// <param name="textureCoordRectangle">The texture coordinates.</param>
		/// <param name="texture">The quad texture or <b>null</b>.</param>
		/// <param name="color">The quad color.</param>
		/// <param name="clamp">The texture clamp.</param>
		public abstract void AddQuad( RectangleF rectangle, RectangleF textureCoordRectangle, ImageComponent texture, ColorValue color, bool clamp );
		/// <summary>
		/// Adds quad to rendering queue.
		/// </summary>
		/// <param name="rectangle">The quad rectangle.</param>
		/// <param name="textureCoordRectangle">The texture coordinates.</param>
		/// <param name="texture">The quad texture or <b>null</b>.</param>
		/// <param name="color">The quad color.</param>
		/// <param name="clamp">The texture clamp.</param>
		public void AddQuad( Rectangle rectangle, Rectangle textureCoordRectangle, ImageComponent texture, ColorValue color, bool clamp )
		{
			AddQuad( rectangle.ToRectangleF(), textureCoordRectangle.ToRectangleF(), texture, color, clamp );
		}

		/// <summary>
		/// Adds quad to rendering queue.
		/// </summary>
		/// <param name="rectangle">The quad rectangle.</param>
		/// <param name="textureCoordRectangle">The texture coordinates.</param>
		/// <param name="texture">The quad texture or <b>null</b>.</param>
		/// <param name="color">The quad color.</param>
		public void AddQuad( RectangleF rectangle, RectangleF textureCoordRectangle, ImageComponent texture, ColorValue color )
		{
			AddQuad( rectangle, textureCoordRectangle, texture, color, true );
		}
		/// <summary>
		/// Adds quad to rendering queue.
		/// </summary>
		/// <param name="rectangle">The quad rectangle.</param>
		/// <param name="textureCoordRectangle">The texture coordinates.</param>
		/// <param name="texture">The quad texture or <b>null</b>.</param>
		/// <param name="color">The quad color.</param>
		public void AddQuad( Rectangle rectangle, Rectangle textureCoordRectangle, ImageComponent texture, ColorValue color )
		{
			AddQuad( rectangle.ToRectangleF(), textureCoordRectangle.ToRectangleF(), texture, color, true );
		}

		/// <summary>
		/// Adds quad to rendering queue.
		/// </summary>
		/// <param name="rectangle">The quad rectangle.</param>
		/// <param name="textureCoordRectangle">The texture coordinates.</param>
		/// <param name="texture">The quad texture or <b>null</b>.</param>
		public void AddQuad( RectangleF rectangle, RectangleF textureCoordRectangle, ImageComponent texture )
		{
			AddQuad( rectangle, textureCoordRectangle, texture, new ColorValue( 1, 1, 1, 1 ), true );
		}
		/// <summary>
		/// Adds quad to rendering queue.
		/// </summary>
		/// <param name="rectangle">The quad rectangle.</param>
		/// <param name="textureCoordRectangle">The texture coordinates.</param>
		/// <param name="texture">The quad texture or <b>null</b>.</param>
		public void AddQuad( Rectangle rectangle, Rectangle textureCoordRectangle, ImageComponent texture )
		{
			AddQuad( rectangle.ToRectangleF(), textureCoordRectangle.ToRectangleF(), texture, new ColorValue( 1, 1, 1, 1 ), true );
		}

		/// <summary>
		/// Adds quad to rendering queue.
		/// </summary>
		/// <param name="rectangle">The quad rectangle.</param>
		/// <param name="color">The quad color.</param>
		public void AddQuad( RectangleF rectangle, ColorValue color )
		{
			AddQuad( rectangle, new RectangleF( 0, 0, 0, 0 ), null, color, false );
		}
		/// <summary>
		/// Adds quad to rendering queue.
		/// </summary>
		/// <param name="rectangle">The quad rectangle.</param>
		/// <param name="color">The quad color.</param>
		public void AddQuad( Rectangle rectangle, ColorValue color )
		{
			AddQuad( rectangle.ToRectangleF(), new RectangleF( 0, 0, 0, 0 ), null, color, false );
		}

		/////////////////////////////////////////

		[Flags]
		public enum AddTextOptions
		{
			PixelAlign = 1,
		}

		/// <summary>
		/// Adds text to rendering queue.
		/// </summary>
		/// <param name="font">The text font.</param>
		/// <param name="text">The text.</param>
		/// <param name="position">The text position.</param>
		/// <param name="horizontalAlign">The text horizontal align.</param>
		/// <param name="verticalAlign">The text vertical align.</param>
		/// <param name="color">The text color.</param>
		public abstract void AddText( FontComponent font, double fontSize, string text, Vector2F position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign );
		/// <summary>
		/// Adds text to rendering queue.
		/// </summary>
		/// <param name="font">The text font.</param>
		/// <param name="text">The text.</param>
		/// <param name="position">The text position.</param>
		/// <param name="horizontalAlign">The text horizontal align.</param>
		/// <param name="verticalAlign">The text vertical align.</param>
		/// <param name="color">The text color.</param>
		public void AddText( FontComponent font, double fontSize, string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			AddText( font, fontSize, text, position.ToVector2F(), horizontalAlign, verticalAlign, color, options );
		}

		/// <summary>
		/// Adds text to rendering queue.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="position">The text position.</param>
		/// <param name="horizontalAlign">The text horizontal align.</param>
		/// <param name="verticalAlign">The text vertical align.</param>
		/// <param name="color">The text color.</param>
		public void AddText( string text, Vector2F position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			AddText( null, DefaultFontSize, text, position, horizontalAlign, verticalAlign, color, options );
		}
		/// <summary>
		/// Adds text to rendering queue.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="position">The text position.</param>
		/// <param name="horizontalAlign">The text horizontal align.</param>
		/// <param name="verticalAlign">The text vertical align.</param>
		/// <param name="color">The text color.</param>
		public void AddText( string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			AddText( null, DefaultFontSize, text, position.ToVector2F(), horizontalAlign, verticalAlign, color, options );
		}

		/// <summary>
		/// Adds text to rendering queue.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="position">The text position.</param>
		/// <param name="horizontalAlign">The text horizontal align.</param>
		/// <param name="verticalAlign">The text vertical align.</param>
		public void AddText( string text, Vector2F position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign )
		{
			AddText( null, DefaultFontSize, text, position, horizontalAlign, verticalAlign, new ColorValue( 1, 1, 1 ) );
		}
		/// <summary>
		/// Adds text to rendering queue.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="position">The text position.</param>
		/// <param name="horizontalAlign">The text horizontal align.</param>
		/// <param name="verticalAlign">The text vertical align.</param>
		public void AddText( string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign )
		{
			AddText( null, DefaultFontSize, text, position.ToVector2F(), horizontalAlign, verticalAlign, new ColorValue( 1, 1, 1 ) );
		}

		/// <summary>
		/// Adds text to rendering queue.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="position">The text position.</param>
		public void AddText( string text, Vector2F position )
		{
			AddText( null, DefaultFontSize, text, position, EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
		}
		/// <summary>
		/// Adds text to rendering queue.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="position">The text position.</param>
		public void AddText( string text, Vector2 position )
		{
			AddText( null, DefaultFontSize, text, position.ToVector2F(), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
		}

		/////////////////////////////////////////

		/// <summary>
		/// Adds text lines to rendering queue.
		/// </summary>
		/// <param name="font">The font.</param>
		/// <param name="lines">The text lines.</param>
		/// <param name="pos">The text position.</param>
		/// <param name="horizontalAlign">The text horizontal align.</param>
		/// <param name="verticalAlign">The text vertical align.</param>
		/// <param name="textVerticalIndention">The vertical intention between lines.</param>
		/// <param name="color">The text color.</param>
		public abstract void AddTextLines( FontComponent font, double fontSize, IList<string> lines, Vector2F pos, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, float textVerticalIndention, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign );
		/// <summary>
		/// Adds text lines to rendering queue.
		/// </summary>
		/// <param name="font">The font.</param>
		/// <param name="lines">The text lines.</param>
		/// <param name="pos">The text position.</param>
		/// <param name="horizontalAlign">The text horizontal align.</param>
		/// <param name="verticalAlign">The text vertical align.</param>
		/// <param name="textVerticalIndention">The vertical intention between lines.</param>
		/// <param name="color">The text color.</param>
		public void AddTextLines( FontComponent font, double fontSize, IList<string> lines, Vector2 pos, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, double textVerticalIndention, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			AddTextLines( font, fontSize, lines, pos.ToVector2F(), horizontalAlign, verticalAlign, (float)textVerticalIndention, color, options );
		}

		/// <summary>
		/// Adds text lines to rendering queue.
		/// </summary>
		/// <param name="lines">The text lines.</param>
		/// <param name="pos">The text position.</param>
		/// <param name="textVerticalIndention">The vertical intention between lines.</param>
		/// <param name="horizontalAlign">The text horizontal align.</param>
		/// <param name="verticalAlign">The text vertical align.</param>
		/// <param name="color">The text color.</param>
		public void AddTextLines( IList<string> lines, Vector2F pos, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, float textVerticalIndention, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			AddTextLines( null, DefaultFontSize, lines, pos, horizontalAlign, verticalAlign, textVerticalIndention, color, options );
		}
		/// <summary>
		/// Adds text lines to rendering queue.
		/// </summary>
		/// <param name="lines">The text lines.</param>
		/// <param name="pos">The text position.</param>
		/// <param name="textVerticalIndention">The vertical intention between lines.</param>
		/// <param name="horizontalAlign">The text horizontal align.</param>
		/// <param name="verticalAlign">The text vertical align.</param>
		/// <param name="color">The text color.</param>
		public void AddTextLines( IList<string> lines, Vector2 pos, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, double textVerticalIndention, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			AddTextLines( null, DefaultFontSize, lines, pos.ToVector2F(), horizontalAlign, verticalAlign, (float)textVerticalIndention, color, options );
		}

		/////////////////////////////////////////

		public abstract int AddTextWordWrap( FontComponent font, double fontSize, string text, RectangleF rect, EHorizontalAlignment horizontalAlign, bool alignByWidth, EVerticalAlignment verticalAlign, float textVerticalIndention, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign );
		public int AddTextWordWrap( FontComponent font, double fontSize, string text, Rectangle rect, EHorizontalAlignment horizontalAlign, bool alignByWidth, EVerticalAlignment verticalAlign, double textVerticalIndention, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			return AddTextWordWrap( font, fontSize, text, rect.ToRectangleF(), horizontalAlign, alignByWidth, verticalAlign, (float)textVerticalIndention, color, options );
		}

		public int AddTextWordWrap( string text, RectangleF rect, EHorizontalAlignment horizontalAlign, bool alignByWidth, EVerticalAlignment verticalAlign, float textVerticalIndention, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			return AddTextWordWrap( null, DefaultFontSize, text, rect, horizontalAlign, alignByWidth, verticalAlign, textVerticalIndention, color, options );
		}
		public int AddTextWordWrap( string text, Rectangle rect, EHorizontalAlignment horizontalAlign, bool alignByWidth, EVerticalAlignment verticalAlign, double textVerticalIndention, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			return AddTextWordWrap( null, DefaultFontSize, text, rect.ToRectangleF(), horizontalAlign, alignByWidth, verticalAlign, (float)textVerticalIndention, color, options );
		}

		/////////////////////////////////////////

		public abstract void AddLines( IList<TriangleVertex> vertices );

		public abstract void AddLines( IList<LineItem> lines );

		/////////////////////////////////////////

		/// <summary>
		/// Adds line to rendering queue.
		/// </summary>
		/// <param name="start">The line start position.</param>
		/// <param name="end">The line end position.</param>
		/// <param name="color">The text color.</param>
		public abstract void AddLine( Vector2F start, Vector2F end, ColorValue color );
		/// <summary>
		/// Adds line to rendering queue.
		/// </summary>
		/// <param name="start">The line start position.</param>
		/// <param name="end">The line end position.</param>
		/// <param name="color">The text color.</param>
		public void AddLine( Vector2 start, Vector2 end, ColorValue color )
		{
			AddLine( start.ToVector2F(), end.ToVector2F(), color );
		}

		/////////////////////////////////////////

		/// <summary>
		/// Adds rectangle to rendering queue.
		/// </summary>
		/// <param name="rectangle">The rectangle.</param>
		/// <param name="color">The text color.</param>
		public abstract void AddRectangle( RectangleF rectangle, ColorValue color );
		/// <summary>
		/// Adds rectangle to rendering queue.
		/// </summary>
		/// <param name="rectangle">The rectangle.</param>
		/// <param name="color">The text color.</param>
		public void AddRectangle( Rectangle rectangle, ColorValue color )
		{
			AddRectangle( rectangle.ToRectangleF(), color );
		}

		public void AddThickRectangle( RectangleF outer, RectangleF inner, ColorValue color )
		{
			var vertices = new TriangleVertex[ 8 ]
			{
				new TriangleVertex( outer.LeftTop, color ),
				new TriangleVertex( inner.LeftTop, color ),
				new TriangleVertex( outer.RightTop, color ),
				new TriangleVertex( inner.RightTop, color ),
				new TriangleVertex( outer.RightBottom, color ),
				new TriangleVertex( inner.RightBottom, color ),
				new TriangleVertex( outer.LeftBottom, color ),
				new TriangleVertex( inner.LeftBottom, color )
			};

			var indices = new int[ 8 * 3 ]
			{
				0,1,2,2,1,3,
				2,3,4,4,3,5,
				4,5,6,6,5,7,
				6,7,0,0,7,1
			};

			AddTriangles( vertices, indices );
		}
		public void AddThickRectangle( Rectangle outer, Rectangle inner, ColorValue color )
		{
			AddThickRectangle( outer.ToRectangleF(), inner.ToRectangleF(), color );
		}

		/////////////////////////////////////////

		public abstract void AddTriangles( IList<TriangleVertex> vertices, ImageComponent texture = null, bool clamp = false );

		public abstract void AddTriangles( IList<TriangleVertex> vertices, IList<int> indices, ImageComponent texture = null, bool clamp = false );

		/////////////////////////////////////////

		public abstract void AddFillEllipse( RectangleF rectangle, int segments, ColorValue color, ImageComponent texture, RectangleF textureCoordRectangle, bool textureClamp );
		public void AddFillEllipse( Rectangle rectangle, int segments, ColorValue color, ImageComponent texture, Rectangle textureCoordRectangle, bool textureClamp )
		{
			AddFillEllipse( rectangle.ToRectangleF(), segments, color, texture, textureCoordRectangle.ToRectangleF(), textureClamp );
		}

		public void AddFillEllipse( RectangleF rectangle, int segments, ColorValue color )
		{
			AddFillEllipse( rectangle, segments, color, null, RectangleF.Zero, false );
		}
		public void AddFillEllipse( Rectangle rectangle, int segments, ColorValue color )
		{
			AddFillEllipse( rectangle.ToRectangleF(), segments, color, null, RectangleF.Zero, false );
		}

		/////////////////////////////////////////

		/// <summary>
		/// Adds an ellipse to rendering queue.
		/// </summary>
		/// <param name="rectangle">The rectangle.</param>
		/// <param name="color">The text color.</param>
		public abstract void AddEllipse( RectangleF rectangle, int segments, ColorValue color );
		/// <summary>
		/// Adds an ellipse to rendering queue.
		/// </summary>
		/// <param name="rectangle">The rectangle.</param>
		/// <param name="color">The text color.</param>
		public void AddEllipse( Rectangle rectangle, int segments, ColorValue color )
		{
			AddEllipse( rectangle.ToRectangleF(), segments, color );
		}

		//public void AddCircle( CircleF circle, int segments, ColorValue color )
		//{
		//	AddEllipse( circle.ToBounds(), segments, color );
		//}

		//public void AddCircle( Circle circle, int segments, ColorValue color )
		//{
		//	AddEllipse( circle.ToBounds(), segments, color );
		//}
	}
}
