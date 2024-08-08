// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Internal.SharpBgfx;

namespace NeoAxis
{
	class CanvasRendererImpl : CanvasRenderer
	{
		const int quadBufferSize = 6;
		const int textBufferSize = 6 * 50;//50 chars
		const int lineBufferSize = 2;
		//const int trianglesBufferSize = 3 * 64;//64 triangles

		static Uniform? u_canvasClipRectangleUniform;
		static RectangleF canvasClipRectangleBinded = new RectangleF( -1000, -1000, 1000, 1000 );

		static Uniform? u_bc5UNormLUniform;
		static Vector4F bc5UNormLBinded = new Vector4F( -100, -100, -100, -100 );

		static Uniform? u_canvasColorMultiplier;
		static ColorValue canvasColorMultiplierBinded = new ColorValue( -100, -100, -100, -100 );

		static Uniform? u_canvasOcclusionDepthCheck;
		static Vector4F canvasOcclusionDepthCheckBinded = new Vector4F( -100, -100, -100, -100 );

		//

		float aspectRatio = 1;
		internal float aspectRatioInv = 1;

		internal bool isScreen;
		Viewport viewportForScreenCanvasRenderer;
		//!!!!!
		//GuiSceneObject parentGuiSceneObjectFor3DRendering;

		FontComponent defaultFont;

		ShaderItem defaultShader = new ShaderItem();

		//Push/Pop settings
		Stack<BlendingType> blendingTypesStack = new Stack<BlendingType>();
		Stack<ShaderItem> shadersStack = new Stack<ShaderItem>();
		Stack<RectangleF> clipRectanglesStack = new Stack<RectangleF>();
		Stack<TextureFilteringMode> textureFilteringModeStack = new Stack<TextureFilteringMode>();
		Stack<ColorValue> colorMultiplierStack = new Stack<ColorValue>();
		Stack<Vector4F> occlusionDepthCheckStack = new Stack<Vector4F>();

		bool disposed;

		////!!!!такое как Push/Pop надо делать
		////!!!!еще вращение
		//bool outGeometryTransformEnabled;
		//Vec2F outGeometryTransformScale;
		//Vec2F outGeometryTransformOffset;

		//List<TriangleVertex> tempVertices1 = new List<TriangleVertex>();
		//List<TriangleVertex> tempVertices2 = new List<TriangleVertex>();

		Dictionary<QuadItemKey, Item> quadItemCache = new Dictionary<QuadItemKey, Item>( new QuadItemKeyComparer() );
		Dictionary<TextItemKey, Item> textItemCache = new Dictionary<TextItemKey, Item>( new TextItemKeyComparer() );
		Dictionary<LineItemKey, Item> lineItemCache = new Dictionary<LineItemKey, Item>( new LineItemKeyComparer() );
		Dictionary<TrianglesItemKey, Item> trianglesItemCache = new Dictionary<TrianglesItemKey, Item>( new TrianglesItemKeyComparer() );
		Dictionary<LinesItemKey, Item> linesItemCache = new Dictionary<LinesItemKey, Item>( new LinesItemKeyComparer() );

		List<Item> outItems = new List<Item>();

		List<FreeRenderableItemGroup> freeRenderableItems = new List<FreeRenderableItemGroup>();

		bool enableCache = true;

		double updateTime;
		double updateTimePrevious;

		//static double? logoInitialTime;
		//static bool logoInitialTimeFinished;
		//static Image watermarkTexture;

		static ImageComponent circleForRoundedCached;
		static ImageComponent circleForRoundedFadingCached;

		////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		struct BufferVertex
		{
			//Vec2 position; - old intel cards are not support Position as float2
			public Vector3F position;
			public ColorValue color;//!!!!new public uint color;
			public Vector2F texCoord;
		}

		///////////////////////////////////////////

		class Item
		{
			public ItemKey itemKey;
			public RenderableItem[] renderableItems;
			public ColorValue colorMultiplier;
			public Vector4F occlusionDepthCheck;
		}

		///////////////////////////////////////////

		abstract class ItemKey
		{
			public RectangleF clipRectangle;
			//public string sourceFileName;
			//public int textureCount;
			public ShaderItem shader;

			public float aspectRatio;
			//public bool outGeometryTransformEnabled;
			//public Vec2F outGeometryTransformScale;
			//public Vec2F outGeometryTransformOffset;

			public int hashCode;

			//

			protected bool IsEqualGeneral( ItemKey obj )
			{
				if( clipRectangle != obj.clipRectangle )
					return false;
				//!!!!так сравнивать?
				if( shader != obj.shader )
					return false;
				//if( sourceFileName != obj.sourceFileName )
				//	return false;
				//if( textureCount != obj.textureCount )
				//	return false;

				//if( customShaderModeTextures != null && obj.customShaderModeTextures == null )
				//	return false;
				//if( customShaderModeTextures == null && obj.customShaderModeTextures != null )
				//	return false;
				//if( customShaderModeTextures != null )
				//{
				//	if( customShaderModeTextures.Length != obj.customShaderModeTextures.Length )
				//		return false;
				//	for( int n = 0; n < customShaderModeTextures.Length; n++ )
				//	{
				//		if( customShaderModeTextures[ n ].TextureName != obj.customShaderModeTextures[ n ].TextureName )
				//			return false;
				//		if( customShaderModeTextures[ n ].Clamp != obj.customShaderModeTextures[ n ].Clamp )
				//			return false;
				//	}
				//}

				if( aspectRatio != obj.aspectRatio )
					return false;
				//if( outGeometryTransformEnabled != obj.outGeometryTransformEnabled )
				//	return false;
				//if( outGeometryTransformScale != obj.outGeometryTransformScale )
				//	return false;
				//if( outGeometryTransformOffset != obj.outGeometryTransformOffset )
				//	return false;

				return true;
			}

			public abstract void CalculateHashCode();
		}

		///////////////////////////////////////////

		class QuadItemKey : ItemKey
		{
			public RectangleF rectangle;
			public RectangleF textureCoordRectangle;
			public ImageComponent texture;
			public ColorValue color;
			public bool clamp;

			//

			public bool IsEqual( QuadItemKey obj )
			{
				if( rectangle != obj.rectangle )
					return false;
				if( textureCoordRectangle != obj.textureCoordRectangle )
					return false;
				if( texture != obj.texture )
					return false;
				if( color != obj.color )
					return false;
				if( clamp != obj.clamp )
					return false;

				if( !IsEqualGeneral( obj ) )
					return false;

				return true;
			}

			public override void CalculateHashCode()
			{
				hashCode = rectangle.GetHashCode();
				if( texture != null )
					hashCode ^= texture.GetHashCode();
				hashCode ^= shader.GetHashCode();
				hashCode ^= textureCoordRectangle.GetHashCode();
			}
		}

		///////////////////////////////////////////

		class TextItemKey : ItemKey
		{
			public FontComponent font;
			public double fontSize;
			public string text;
			public Vector2F startPosition;
			public ColorValue color;

			//

			public bool IsEqual( TextItemKey obj )
			{
				if( font != obj.font )
					return false;
				if( fontSize != obj.fontSize )
					return false;
				if( text != obj.text )
					return false;
				if( startPosition != obj.startPosition )
					return false;
				if( color != obj.color )
					return false;

				if( !IsEqualGeneral( obj ) )
					return false;

				return true;
			}

			public override void CalculateHashCode()
			{
				hashCode = fontSize.GetHashCode() ^ text.GetHashCode() ^ startPosition.GetHashCode();
				hashCode ^= shader.GetHashCode();
			}
		}

		///////////////////////////////////////////

		class LineItemKey : ItemKey
		{
			public Vector2F correctedStart;
			public Vector2F correctedEnd;
			public ColorValue color;

			//

			public bool IsEqual( LineItemKey obj )
			{
				if( correctedStart != obj.correctedStart )
					return false;
				if( correctedEnd != obj.correctedEnd )
					return false;
				if( color != obj.color )
					return false;

				if( !IsEqualGeneral( obj ) )
					return false;

				return true;
			}

			public override void CalculateHashCode()
			{
				hashCode = correctedStart.GetHashCode() ^ correctedEnd.GetHashCode();
				hashCode ^= shader.GetHashCode();
			}
		}

		///////////////////////////////////////////

		class TrianglesItemKey : ItemKey
		{
			public TriangleVertex[] vertices;
			public ImageComponent texture;
			public bool clamp;

			//

			public unsafe bool IsEqual( TrianglesItemKey obj )
			{
				if( vertices.Length != obj.vertices.Length )
					return false;

				if( texture != obj.texture )
					return false;
				if( clamp != obj.clamp )
					return false;

				if( !IsEqualGeneral( obj ) )
					return false;

				if( !ReferenceEquals( vertices, obj.vertices ) )
				{
					fixed( TriangleVertex* v1 = vertices, v2 = obj.vertices )
					{
						if( NativeUtility.CompareMemory( v1, v2, vertices.Length * sizeof( TriangleVertex ) ) != 0 )
							return false;
					}
				}
				//else
				//{
				//	//!!!!
				//	Log.Info( "same references 2" );
				//}

				////!!!!
				//Log.Info( "equal 2" );

				return true;
			}

			public override void CalculateHashCode()
			{
				hashCode = vertices[ 0 ].position.GetHashCode();
				hashCode ^= shader.GetHashCode();
			}
		}

		///////////////////////////////////////////

		class LinesItemKey : ItemKey
		{
			public TriangleVertex[] vertices;

			//

			public unsafe bool IsEqual( LinesItemKey obj )
			{
				if( vertices.Length != obj.vertices.Length )
					return false;
				if( !IsEqualGeneral( obj ) )
					return false;

				if( !ReferenceEquals( vertices, obj.vertices ) )
				{
					fixed( TriangleVertex* v1 = vertices, v2 = obj.vertices )
					{
						if( NativeUtility.CompareMemory( v1, v2, vertices.Length * sizeof( TriangleVertex ) ) != 0 )
							return false;
					}
				}
				//else
				//{
				//	//!!!!
				//	Log.Info( "same references" );
				//}

				////!!!!
				//Log.Info( "equal" );

				return true;
			}

			public override void CalculateHashCode()
			{
				hashCode = vertices[ 0 ].position.GetHashCode();
				hashCode ^= shader.GetHashCode();
			}
		}

		///////////////////////////////////////////

		class QuadItemKeyComparer : IEqualityComparer<QuadItemKey>
		{
			public bool Equals( QuadItemKey x, QuadItemKey y )
			{
				return x.IsEqual( y );
			}

			public int GetHashCode( QuadItemKey obj )
			{
				return obj.hashCode;
			}
		}

		///////////////////////////////////////////

		class TextItemKeyComparer : IEqualityComparer<TextItemKey>
		{
			public bool Equals( TextItemKey x, TextItemKey y )
			{
				return x.IsEqual( y );
			}

			public int GetHashCode( TextItemKey obj )
			{
				return obj.hashCode;
			}
		}

		///////////////////////////////////////////

		class LineItemKeyComparer : IEqualityComparer<LineItemKey>
		{
			public bool Equals( LineItemKey x, LineItemKey y )
			{
				return x.IsEqual( y );
			}

			public int GetHashCode( LineItemKey obj )
			{
				return obj.hashCode;
			}
		}

		///////////////////////////////////////////

		class TrianglesItemKeyComparer : IEqualityComparer<TrianglesItemKey>
		{
			public bool Equals( TrianglesItemKey x, TrianglesItemKey y )
			{
				return x.IsEqual( y );
			}

			public int GetHashCode( TrianglesItemKey obj )
			{
				return obj.hashCode;
			}
		}

		///////////////////////////////////////////

		class LinesItemKeyComparer : IEqualityComparer<LinesItemKey>
		{
			public bool Equals( LinesItemKey x, LinesItemKey y )
			{
				return x.IsEqual( y );
			}

			public int GetHashCode( LinesItemKey obj )
			{
				return obj.hashCode;
			}
		}

		///////////////////////////////////////////

		class RenderableItem
		{
			//public GpuVertexDeclaration vertexDeclaration;
			public GpuVertexBuffer vertexBuffer;
			public int vertexCount;
			//!!!!!можно ли меньше материалов сделать?
			public MaterialData material;

			public double lastReleaseRenderTime;

			public RenderOperationType renderOperation;
			public ImageComponent texture;
			public bool textureClamp;
			public TextureFilteringMode textureFiltering;
		}

		///////////////////////////////////////////

		class FreeRenderableItemGroup
		{
			public ShaderItem shader;
			//public string sourceFileName;
			//public int textureCount;
			public int bufferVertexCount;

			public List<RenderableItem> list = new List<RenderableItem>();
		}

		///////////////////////////////////////////

		class MaterialData
		{
			bool canvasRendererIsScreen;
			ShaderItem shader;
			GpuMaterialPass materialPass;

			//

			public MaterialData( bool canvasRendererIsScreen, ShaderItem shader )
			{
				this.canvasRendererIsScreen = canvasRendererIsScreen;
				this.shader = shader;
			}

			public bool CanvasRendererIsScreen
			{
				get { return canvasRendererIsScreen; }
			}

			public ShaderItem Shader
			{
				get { return shader; }
			}

			public GpuMaterialPass MaterialPass
			{
				get
				{
					if( materialPass == null )
					{
						//generate compile arguments
						var defines = new List<(string, string)>( 16 );
						if( shader.CompiledVertexProgram == null || shader.CompiledFragmentProgram == null )
						{
							if( shader.DefinesExists )
							{
								foreach( ShaderItem.DefineItem item in shader.Defines )
									defines.Add( (item.Name, item.Value) );
							}

							//if( !canvasRendererIsScreen )
							//{
							////Fog
							//FogModes fogMode = SceneManager.Instance.GetFogMode();
							//if( !canvasRendererIsScreen )//&& fogMode != FogModes.None )
							//{
							//arguments.Append( " -DFOG_ENABLED" );
							//arguments.Append( " -DFOG_" + fogMode.ToString().ToUpper() );
							//}
							//}
						}

						string error;

						//vertex program
						GpuProgram vertexProgram;
						if( shader.CompiledVertexProgram != null )
							vertexProgram = shader.CompiledVertexProgram;
						else
						{
							vertexProgram = GpuProgramManager.GetProgram( "CanvasRenderer_Vertex_", GpuProgramType.Vertex,
								shader.VertexProgramFileName, defines, true, out error );
							if( !string.IsNullOrEmpty( error ) )
							{
								Log.Warning( error );
								return null;
							}
						}

						//fragment program
						GpuProgram fragmentProgram;
						if( shader.CompiledFragmentProgram != null )
							fragmentProgram = shader.CompiledFragmentProgram;
						else
						{
							fragmentProgram = GpuProgramManager.GetProgram( "CanvasRenderer_Fragment_", GpuProgramType.Fragment,
								shader.FragmentProgramFileName, defines, true, out error );
							if( !string.IsNullOrEmpty( error ) )
							{
								Log.Warning( error );
								return null;
							}
						}

						materialPass = new GpuMaterialPass( null, vertexProgram, fragmentProgram );

						//pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
						//pass.DestBlendFactor = SceneBlendFactor.OneMinusSourceAlpha;
						materialPass.DepthWrite = false;
						materialPass.CullingMode = CullingMode.None;
						//pass.Lighting = false;
						materialPass.DepthCheck = !canvasRendererIsScreen;

					}
					return materialPass;
				}
			}

			//protected override void OnResultCompile()
			//{
			//	var result = new CompiledData();

			//	//!!!!good?
			//	EngineThreading.ExecuteFromMainThreadWait( delegate ()
			//	{
			//		//generate compile arguments
			//		var defines = new List<Tuple<string, string>>();
			//		if( shader.CompiledVertexProgram == null || shader.CompiledFragmentProgram == null )
			//		{
			//			if( shader.Defines != null )
			//			{
			//				foreach( ShaderItem.DefineItem item in shader.Defines )
			//				{
			//					defines.Add( Tuple.Create( item.Name, item.Value ) );

			//					//if( defines.Length != 0 )
			//					//	defines.Append( " " );
			//					//defines.Append( "-D" );
			//					//defines.Append( item.Name );
			//					//if( !string.IsNullOrEmpty( item.Value ) )
			//					//{
			//					//	defines.Append( "=" );
			//					//	defines.Append( item.Value );
			//					//}
			//				}
			//			}

			//			//if( !canvasRendererIsScreen )
			//			//{
			//			////Fog
			//			//FogModes fogMode = SceneManager.Instance.GetFogMode();
			//			//if( !canvasRendererIsScreen )//&& fogMode != FogModes.None )
			//			//{
			//			//arguments.Append( " -DFOG_ENABLED" );
			//			//arguments.Append( " -DFOG_" + fogMode.ToString().ToUpper() );
			//			//}
			//			//}
			//		}

			//		string error;

			//		//vertex program
			//		GpuProgram vertexProgram;
			//		if( shader.CompiledVertexProgram != null )
			//			vertexProgram = shader.CompiledVertexProgram;
			//		else
			//		{
			//			vertexProgram = GpuProgramManager.GetProgram( "CanvasRenderer_Vertex_", GpuProgramType.Vertex,
			//				shader.VertexProgramFileName, defines, out error );
			//			if( !string.IsNullOrEmpty( error ) )
			//				Log.Fatal( error );
			//		}

			//		//fragment program
			//		GpuProgram fragmentProgram;
			//		if( shader.CompiledFragmentProgram != null )
			//			fragmentProgram = shader.CompiledFragmentProgram;
			//		else
			//		{
			//			fragmentProgram = GpuProgramManager.GetProgram( "CanvasRenderer_Fragment_", GpuProgramType.Fragment,
			//				shader.FragmentProgramFileName, defines, out error );
			//			if( !string.IsNullOrEmpty( error ) )
			//				Log.Fatal( error );
			//		}

			//		var pass = new GpuMaterialPass( vertexProgram, fragmentProgram );
			//		result.AllPasses.Add( pass );

			//		//pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
			//		//pass.DestBlendFactor = SceneBlendFactor.OneMinusSourceAlpha;
			//		pass.DepthWrite = false;
			//		pass.CullingMode = CullingMode.None;
			//		//pass.Lighting = false;
			//		pass.DepthCheck = !canvasRendererIsScreen;
			//	} );

			//	Result = result;
			//}
		}

		///////////////////////////////////////////

		public CanvasRendererImpl( Viewport viewportForScreenCanvasRenderer )
		{
			this.isScreen = true;// isScreen;
			this.viewportForScreenCanvasRenderer = viewportForScreenCanvasRenderer;

			InitInternal();

			lock( RenderingSystem.canvasRenderers )
				RenderingSystem.canvasRenderers.Add( this );
		}

		//!!!!!
		//public GuiRenderer( GuiSceneObject guiMovableObjectFor3dInGameGuiRenderer )
		//{
		//	isScreen = false;
		//	this.parentGuiSceneObjectFor3DRendering = guiMovableObjectFor3dInGameGuiRenderer;

		//	InitInternal();

		//lock( RendererWorld.guiRenderers )
		//	RendererWorld.guiRenderers.Add( this );
		//}

		public void ChangeViewport( Viewport newViewport )
		{
			//!!!!!переименовать метод? как лучше?
			//что-то обновл€ть?
			//может пересоздавать лучше?
			Log.Fatal( "impl" );

			if( !isScreen )
				Log.Fatal( "CanvasRenderer: ChangeViewport: Unable to change viewport for 3D gui." );

			viewportForScreenCanvasRenderer = newViewport;
		}

		public override float AspectRatio
		{
			get { return aspectRatio; }
		}
		public override float AspectRatioInv
		{
			get { return aspectRatioInv; }
		}

		public void SetAspectRatio( float value )
		{
			aspectRatio = value;
			if( aspectRatio != 0 )
				aspectRatioInv = 1.0f / aspectRatio;
		}

		/// <summary>Releases the resources that are used by the object.</summary>
		public void Dispose()
		{
			if( !disposed )
			{
				//after shutdown check
				if( RenderingSystem.Disposed )
					Log.Fatal( "CanvasRendererImpl: Dispose after shutdown." );

				DestroyAllRenderableItems();

				lock( RenderingSystem.canvasRenderers )
					RenderingSystem.canvasRenderers.Remove( this );

				ShutdownInternal();

				disposed = true;
			}
		}

		public override void AddQuad( RectangleF rectangle, RectangleF textureCoordRectangle, ImageComponent texture, ColorValue color, bool clamp )
		{
			//!!!!везде закомментил
			//if( RenderSystem.Instance.IsDeviceLost() )
			//	return;
			if( RenderingSystem.BackendNull )
				return;

			GetCurrentClipRectangle( out var clipRectangle );

			//fully culled
			if( !clipRectangle.IsCleared() && !clipRectangle.Intersects( ref rectangle ) )
				return;

			RectangleF newRectangle = rectangle;
			RectangleF newTextureCoordRectangle = textureCoordRectangle;

			//remove fullscreen quad artefacts on screen edges when FSAA enabled.
			//!!!!!было
			//if( IsScreen && !RendererWorld.isEditor )
			//{
			//	Rect clip;
			//	if( clipRectangle.IsCleared() )
			//		clip = new Rect( 0, 0, 1, 1 );
			//	else
			//		clip = clipRectangle;

			//	if( rectangle.Size.X != 0 )
			//	{
			//		bool fixLeft = rectangle.Left == 0 && clip.Left <= 0;
			//		bool fixRight = rectangle.Right == 1 && clip.Right >= 1;

			//		if( fixLeft || fixRight )
			//		{
			//			if( fixLeft )
			//				newRectangle.Left = -.02f;
			//			if( fixRight )
			//				newRectangle.Right = 1.02f;

			//			//if( texture != null )
			//			{
			//				//y = kx + b
			//				float k = ( textureCoordRectangle.Right - textureCoordRectangle.Left ) / ( rectangle.Right - rectangle.Left );
			//				float b = textureCoordRectangle.Left - k * rectangle.Left;

			//				if( fixLeft )
			//					newTextureCoordRectangle.Left = k * newRectangle.Left + b;
			//				if( fixRight )
			//					newTextureCoordRectangle.Right = k * newRectangle.Right + b;
			//			}
			//		}
			//	}

			//	if( rectangle.Size.Y != 0 )
			//	{
			//		bool fixTop = rectangle.Top == 0 && clip.Top <= 0;
			//		bool fixBottom = rectangle.Bottom == 1 && clip.Bottom >= 1;

			//		if( fixTop || fixBottom )
			//		{
			//			if( fixTop )
			//				newRectangle.Top = -.02f;
			//			if( fixBottom )
			//				newRectangle.Bottom = 1.02f;

			//			//if( texture != null )
			//			{
			//				//y = kx + b
			//				float k = ( textureCoordRectangle.Bottom - textureCoordRectangle.Top ) / ( rectangle.Bottom - rectangle.Top );
			//				float b = textureCoordRectangle.Top - k * rectangle.Top;

			//				if( fixTop )
			//					newTextureCoordRectangle.Top = k * newRectangle.Top + b;
			//				if( fixBottom )
			//					newTextureCoordRectangle.Bottom = k * newRectangle.Bottom + b;
			//			}
			//		}
			//	}
			//}

			//uint uintColor = RenderSystem.Instance.ConvertColorValue( ref color );

			Item item = null;

			QuadItemKey key = new QuadItemKey();
			{
				key.rectangle = newRectangle;
				key.textureCoordRectangle = newTextureCoordRectangle;
				key.texture = texture;
				key.color = color;
				key.clamp = clamp;
				key.clipRectangle = clipRectangle;
				key.shader = GetCurrentShader();
				//key.sourceFileName = sourceFileName;
				//key.textureCount = textureCount;

				key.aspectRatio = aspectRatio;
				//key.outGeometryTransformEnabled = outGeometryTransformEnabled;
				//key.outGeometryTransformScale = outGeometryTransformScale;
				//key.outGeometryTransformOffset = outGeometryTransformOffset;

				key.CalculateHashCode();
			}

			//trying to get item from cache
			if( enableCache )
			{
				if( quadItemCache.TryGetValue( key, out item ) )
				{
					if( !quadItemCache.Remove( key ) )
						Log.Fatal( "CanvasRenderer: AddQuad: Internal error." );
				}
			}

			int vertexBufferSize = quadBufferSize;

			if( item == null )
			{
				//create renderable item
				RenderableItem renderableItem = GetFreeRenderableItem( GetCurrentShader(), vertexBufferSize );
				SetRenderableItemDynamicData( renderableItem, RenderOperationType.TriangleList, texture, clamp );

				//fill to buffer
				unsafe
				{
					RectangleF correctedRectangle = newRectangle;
					RectangleF correctedTextureCoordRectangle = new RectangleF();

					correctedTextureCoordRectangle = newTextureCoordRectangle;
					//if( !clipRectangle.IsCleared() && !clipRectangle.Contains( correctedRectangle ) )
					//{
					//	Vec2F oldPosition = correctedRectangle.LeftTop;
					//	Vec2F oldSize = correctedRectangle.Size;

					//	correctedRectangle = correctedRectangle.Intersection( clipRectangle );

					//	if( /*texture != null && */oldSize.X != 0 && oldSize.Y != 0 )
					//	{
					//		Vec2F invOldSize = 1.0f / oldSize;

					//		Vec2F scaleStart = ( correctedRectangle.LeftTop - oldPosition ) * invOldSize;
					//		Vec2F scaleEnd = ( correctedRectangle.RightBottom - oldPosition ) * invOldSize;

					//		correctedTextureCoordRectangle.Left = newTextureCoordRectangle.Left +
					//			scaleStart.X * newTextureCoordRectangle.Size.X;
					//		correctedTextureCoordRectangle.Right = newTextureCoordRectangle.Left +
					//			scaleEnd.X * newTextureCoordRectangle.Size.X;
					//		correctedTextureCoordRectangle.Top = newTextureCoordRectangle.Top +
					//			scaleStart.Y * newTextureCoordRectangle.Size.Y;
					//		correctedTextureCoordRectangle.Bottom = newTextureCoordRectangle.Top +
					//			scaleEnd.Y * newTextureCoordRectangle.Size.Y;
					//	}
					//}
					//else
					//{
					//	//if( texture != null )
					//	correctedTextureCoordRectangle = newTextureCoordRectangle;
					//}

					RectangleF destRect = FixResultPosition( correctedRectangle );

					//copy data to buffer
					{
						GpuVertexBuffer vertexBuffer = renderableItem.vertexBuffer;

						var vertices = vertexBuffer.WriteBegin();
						fixed( byte* pVertices = vertices )
							AddQuadToBuffer( (BufferVertex*)pVertices, ref destRect, ref correctedTextureCoordRectangle, /*texture, */ref color );
						vertexBuffer.WriteEnd();

						//byte[] vertices = new byte[ vertexBuffer.Vertices.Length ];
						//fixed ( byte* pVertices = vertices )
						//{
						//	AddQuadToBuffer( (BufferVertex*)pVertices, ref destRect, ref correctedTextureCoordRectangle, /*texture, */ref color );
						//}
						//vertexBuffer.SetData( vertices );

						renderableItem.vertexCount = vertexBufferSize;
					}
				}

				//create item
				item = new Item();
				item.itemKey = key;
				item.renderableItems = new RenderableItem[] { renderableItem };
			}

			//set dynamic data
			SetItemDynamicData( item );

			//add to out list
			outItems.Add( item );
		}

		unsafe void CopyDataToRenderableItemVertexBuffer( RenderableItem renderableItem, BufferVertex* buffer, int vertexCount )
		{
			GpuVertexBuffer vertexBuffer = renderableItem.vertexBuffer;

			vertexBuffer.Write( (IntPtr)buffer, vertexCount );
			//byte[] vertices = new byte[ vertexBuffer.Vertices.Length ];
			//Marshal.Copy( (IntPtr)buffer, vertices, 0, vertices.Length );
			//vertexBuffer.SetData( vertices );

			renderableItem.vertexCount = vertexCount;
		}

		int GetTextLineCount( string text )
		{
			int result = 1;
			foreach( char c in text )
			{
				if( c == '\n' )
					result++;
			}
			return result;
		}

		public override void AddText( FontComponent font, double fontSize, string text, Vector2F position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			//if( RenderSystem.Instance.IsDeviceLost() )
			//	return;
			if( RenderingSystem.BackendNull )
				return;
			if( string.IsNullOrEmpty( text ) )
				return;

			if( font == null )//|| font.Disposed )
				font = DefaultFont;
			if( font == null || font.Disposed )
				return;
			if( fontSize < 0 )
				fontSize = DefaultFontSize;

			//only for screen gui renderer
			Vector2F viewportSize = Vector2F.Zero;
			if( ViewportForScreenCanvasRenderer != null )
				viewportSize = ViewportForScreenCanvasRenderer.SizeInPixels.ToVector2F();
			Vector2F viewportSizeInv = new Vector2F( 1, 1 );
			if( viewportSize.X != 0 && viewportSize.Y != 0 )
				viewportSizeInv = 1.0f / viewportSize;

			GetCurrentClipRectangle( out var clipRectangle );

			//check culled
			//!!!!можно проверить что весь за clipRectangle

			//!!!!!float. везде так
			Vector2F startPos = position;

			if( horizontalAlign == EHorizontalAlignment.Center )
				startPos.X -= font.GetTextLength( fontSize, this, text ) * .5f;
			else if( horizontalAlign == EHorizontalAlignment.Right )
				startPos.X -= font.GetTextLength( fontSize, this, text );

			if( verticalAlign == EVerticalAlignment.Center )
				startPos.Y -= (float)fontSize * .5f * (float)GetTextLineCount( text );
			else if( verticalAlign == EVerticalAlignment.Bottom )
				startPos.Y -= (float)fontSize * (float)GetTextLineCount( text );

			//uint uintColor = RenderSystem.Instance.ConvertColorValue( ref color );

			Item item = null;

			TextItemKey key = new TextItemKey();
			{
				key.font = font;
				key.fontSize = fontSize;
				key.text = text;
				key.startPosition = startPos;
				key.color = color;
				key.clipRectangle = clipRectangle;
				key.shader = GetCurrentShader();
				//key.sourceFileName = sourceFileName;
				//key.textureCount = textureCount;

				key.aspectRatio = aspectRatio;
				//key.outGeometryTransformEnabled = outGeometryTransformEnabled;
				//key.outGeometryTransformScale = outGeometryTransformScale;
				//key.outGeometryTransformOffset = outGeometryTransformOffset;

				key.CalculateHashCode();
			}

			//trying to get item from cache
			if( enableCache )
			{
				if( textItemCache.TryGetValue( key, out item ) )
				{
					if( !textItemCache.Remove( key ) )
						Log.Fatal( "CanvasRenderer: AddText: Internal error." );
				}
			}

			int vertexBufferSize = textBufferSize;

			//!!!!было
			////update Variant.trueTypeLastUsedTime
			//if( item != null )
			//	font.GetTextures( fontSize, this );

			if( item == null )
			{
				var compiledData = font.GetCompiledData( fontSize, this );
				if( compiledData == null )
					return;

				//create renderable items

				List<RenderableItem> renderableItems = new List<RenderableItem>();

				var textures = compiledData.Textures;
				//Image[] textures = font.GetTextures( fontSize, this );
				if( textures == null )
					return;

				//get list of used texture
				bool[] textureIndices = new bool[ textures.Length ];
				{
					if( textures.Length != 1 )
					{
						foreach( char c in text )
						{
							if( font.IsCharacterInitialized( c ) )
								textureIndices[ compiledData.GetCharacterTextureIndex( c ) ] = true;
						}
					}
					else
						textureIndices[ 0 ] = true;
				}

				unsafe
				{
					BufferVertex* buffer = (BufferVertex*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility,
						sizeof( BufferVertex ) * vertexBufferSize );

					//enumerate textures
					for( int fontTextureIndex = 0; fontTextureIndex < textureIndices.Length; fontTextureIndex++ )
					{
						if( !textureIndices[ fontTextureIndex ] )
							continue;

						ImageComponent texture = textures[ fontTextureIndex ];
						if( texture == null )
							continue;

						Vector2F pos = startPos;

						int vertexCountInBuffer = 0;

						//render each character with specified texture index
						foreach( char c in text )
						{
							if( c == '\n' )
							{
								pos.X = startPos.X;
								pos.Y += (float)fontSize;
								continue;
							}

							if( c < 32 )
								continue;

							float characterWidth;
							int textureIndex;
							Vector2F drawOffset;
							Vector2F drawSize;
							RectangleF textureCoordRectangle;

							compiledData.GetCharacterInfo( fontSize, this, c, out characterWidth, out textureIndex, out drawOffset, out drawSize, out textureCoordRectangle );

							if( textureIndex == fontTextureIndex )
							{
								RectangleF rectangle = new RectangleF( pos + drawOffset, pos + drawOffset + drawSize );

								//add quad
								if( clipRectangle.IsCleared() || clipRectangle.Intersects( ref rectangle ) )
								{
									//check for full buffer
									if( vertexCountInBuffer + 6 > vertexBufferSize )
									{
										//make renderable item
										RenderableItem renderableItem = GetFreeRenderableItem( GetCurrentShader(), vertexBufferSize );
										SetRenderableItemDynamicData( renderableItem, RenderOperationType.TriangleList, texture, true );

										//copy data to buffer
										CopyDataToRenderableItemVertexBuffer( renderableItem, buffer, vertexCountInBuffer );
										renderableItems.Add( renderableItem );

										vertexCountInBuffer = 0;
									}

									//add character to buffer

									RectangleF correctedRectangle = rectangle;
									RectangleF correctedTextureCoordRectangle = new RectangleF();

									correctedTextureCoordRectangle = textureCoordRectangle;
									//if( !clipRectangle.IsCleared() && !clipRectangle.Contains( ref correctedRectangle ) )
									//{
									//	Vec2F oldPosition = correctedRectangle.LeftTop;
									//	Vec2F oldSize = correctedRectangle.Size;

									//	correctedRectangle = correctedRectangle.Intersection( clipRectangle );

									//	if( oldSize != new Vec2F( 0, 0 ) )
									//	{
									//		Vec2F invOldSize = 1.0f / oldSize;

									//		Vec2F scaleStart = ( correctedRectangle.LeftTop - oldPosition ) * invOldSize;
									//		Vec2F scaleEnd = ( correctedRectangle.RightBottom - oldPosition ) * invOldSize;

									//		correctedTextureCoordRectangle.Left = textureCoordRectangle.Left +
									//			scaleStart.X * textureCoordRectangle.Size.X;
									//		correctedTextureCoordRectangle.Right = textureCoordRectangle.Left +
									//			scaleEnd.X * textureCoordRectangle.Size.X;
									//		correctedTextureCoordRectangle.Top = textureCoordRectangle.Top +
									//			scaleStart.Y * textureCoordRectangle.Size.Y;
									//		correctedTextureCoordRectangle.Bottom = textureCoordRectangle.Top +
									//			scaleEnd.Y * textureCoordRectangle.Size.Y;
									//	}
									//}
									//else
									//	correctedTextureCoordRectangle = textureCoordRectangle;

									//align rectangle coordinates to screen pixels. for true type fonts.
									//!!!!было font.Definition.Type == EngineFontDefinition.Types.TrueType

									if( IsScreen && !compiledData.trueTypeFontSizeYInPixelsScaled && ( ( options & AddTextOptions.PixelAlign ) != 0 ) )//&& font.Definition.Type == EngineFontDefinition.Types.TrueType /*&& !outGeometryTransformEnabled*/ )
									{
										//Vec2 viewportSize = ViewportForScreenGuiRenderer.DimensionsInPixels.
										//   Size.ToVec2();

										Vector2F leftTop = correctedRectangle.LeftTop;
										Vector2F rightBottom = correctedRectangle.RightBottom;

										leftTop *= viewportSize;
										leftTop = new Vector2F( (int)( leftTop.X + .9999f ), (int)( leftTop.Y + .9999f ) );
										//leftTop = new Vec2( (int)leftTop.X, (int)leftTop.Y );
										//texel offset
										//!!!!!
										//if( RenderSystem.Instance.IsDirect3D() )
										//	leftTop -= new Vec2F( .5f, .5f );
										//if( RenderSystem.Instance.IsDirect3D() )
										//   leftTop += new Vec2( .5f, .5f );
										leftTop *= viewportSizeInv;// leftTop /= viewportSize;

										rightBottom *= viewportSize;
										rightBottom = new Vector2F( (int)( rightBottom.X + .9999f ), (int)( rightBottom.Y + .9999f ) );
										//rightBottom = new Vec2( (int)rightBottom.X, (int)rightBottom.Y );
										//texel offset
										//!!!!
										//if( RenderSystem.Instance.IsDirect3D() )
										//	rightBottom -= new Vec2F( .5f, .5f );
										//if( RenderSystem.Instance.IsDirect3D() )
										//   rightBottom += new Vec2( .5f, .5f );
										rightBottom *= viewportSizeInv;// rightBottom  /= viewportSize;

										correctedRectangle = new RectangleF( leftTop, rightBottom );
									}

									RectangleF destRect = FixResultPosition( correctedRectangle );

									AddQuadToBuffer( buffer + vertexCountInBuffer, ref destRect,
										ref correctedTextureCoordRectangle, /*texture, */ref color );

									vertexCountInBuffer += 6;
								}
							}

							pos.X += font.GetCharacterWidth( fontSize, this, c );
						}

						if( vertexCountInBuffer != 0 )
						{
							//make renderable item
							RenderableItem renderableItem = GetFreeRenderableItem( GetCurrentShader(), vertexBufferSize );
							SetRenderableItemDynamicData( renderableItem, RenderOperationType.TriangleList, texture, true );

							//copy data to buffer
							CopyDataToRenderableItemVertexBuffer( renderableItem, buffer, vertexCountInBuffer );
							renderableItems.Add( renderableItem );

							vertexCountInBuffer = 0;
						}
					}

					NativeUtility.Free( (IntPtr)buffer );
				}

				//create item
				item = new Item();
				item.itemKey = key;
				item.renderableItems = renderableItems.ToArray();
			}

			//set dynamic data
			SetItemDynamicData( item );

			//add to out list
			outItems.Add( item );
		}

		public override void AddTextLines( FontComponent font, double fontSize, IList<string> lines, Vector2F pos, EHorizontalAlignment horizontalAlign,
			EVerticalAlignment verticalAlign, float textVerticalIndention, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			//if( RenderSystem.Instance.IsDeviceLost() )
			//	return;
			if( RenderingSystem.BackendNull )
				return;
			if( lines.Count == 0 )
				return;

			if( font == null )//|| font.Disposed )
				font = DefaultFont;
			if( font == null || font.Disposed )
				return;
			if( fontSize < 0 )
				fontSize = DefaultFontSize;

			double startY = pos.Y;
			if( verticalAlign == EVerticalAlignment.Center )
			{
				double height = fontSize * (float)lines.Count + textVerticalIndention * ( (float)lines.Count - 1 );
				startY -= height / 2;
			}
			else if( verticalAlign == EVerticalAlignment.Bottom )
			{
				double height = fontSize * (float)lines.Count + textVerticalIndention * ( (float)lines.Count - 1 );
				startY -= height;
			}

			double stepY = fontSize + textVerticalIndention;

			double positionY = startY;
			foreach( string line in lines )
			{
				AddText( font, fontSize, line, new Vector2( pos.X, positionY ), horizontalAlign, EVerticalAlignment.Top, color, options );
				positionY += stepY;
			}
		}

		struct TextLineItem
		{
			public string text;
			public bool alignByWidth;
			public TextLineItem( string text, bool alignByWidth )
			{
				this.text = text;
				this.alignByWidth = alignByWidth;
			}
		}

		public override int AddTextWordWrap( FontComponent font, double fontSize, string text, RectangleF rect, EHorizontalAlignment horizontalAlign,
			bool alignByWidth, EVerticalAlignment verticalAlign, float textVerticalIndention, ColorValue color, AddTextOptions options = AddTextOptions.PixelAlign )
		{
			//if( RenderSystem.Instance.IsDeviceLost() )
			//	return 0;
			if( RenderingSystem.BackendNull )
				return 0;
			if( string.IsNullOrEmpty( text ) )
				return 0;

			if( font == null )//|| font.Disposed )
				font = DefaultFont;
			if( font == null || font.Disposed )
				return 0;
			if( fontSize < 0 )
				fontSize = DefaultFontSize;

			Vector2 size = rect.GetSize();

			List<TextLineItem> lines = new List<TextLineItem>();
			{
				double posY = 0;
				int count = 0;

				StringBuilder word = new StringBuilder( text.Length );
				float wordLength = 0;

				StringBuilder toDraw = new StringBuilder( text.Length );
				float toDrawLength = 0;

				float spaceCharacterLength = font.GetCharacterWidth( fontSize, this, ' ' );

				foreach( char c in text )
				{
					if( c == ' ' )
					{
						if( word.Length != 0 )
						{
							toDraw.Append( ' ' );
							toDraw.Append( word );
							toDrawLength += spaceCharacterLength + wordLength;
							word.Length = 0;
							wordLength = 0;
						}
						else
						{
							toDraw.Append( ' ' );
							toDrawLength += spaceCharacterLength;
						}
						continue;
					}

					if( c == '\n' )
					{
						toDraw.Append( ' ' );
						toDraw.Append( word );
						toDrawLength += wordLength;
						count++;
						lines.Add( new TextLineItem( toDraw.ToString().Trim(), false ) );

						posY += fontSize + textVerticalIndention;

						if( posY >= size.Y )
							break;

						toDraw.Length = 0;
						toDrawLength = 0;
						word.Length = 0;
						wordLength = 0;
					}

					//slowly?
					float characterWidth = font.GetCharacterWidth( fontSize, this, c );

					if( toDrawLength + wordLength + characterWidth >= size.X )
					{
						if( toDraw.Length == 0 )
						{
							toDraw.Length = 0;
							toDraw.Append( word.ToString() );
							toDrawLength = wordLength;
							word.Length = 0;
							wordLength = 0;
						}
						count++;
						lines.Add( new TextLineItem( toDraw.ToString().Trim(), alignByWidth ) );

						posY += fontSize + textVerticalIndention;
						if( posY >= size.Y )
							break;
						toDraw.Length = 0;
						toDrawLength = 0;
					}

					word.Append( c );
					wordLength += characterWidth;
				}

				string s = string.Format( "{0} {1}", toDraw, word );
				s = s.Trim();
				if( s.Length != 0 )
				{
					if( posY < size.Y )
						lines.Add( new TextLineItem( s, false ) );
					count++;
				}
			}

			if( lines.Count == 0 )
				return 0;

			double startY = 0;
			switch( verticalAlign )
			{
			case EVerticalAlignment.Top:
				startY = rect.Top;
				break;
			case EVerticalAlignment.Center:
				{
					double height = fontSize * (float)lines.Count + textVerticalIndention * ( (float)lines.Count - 1 );
					startY = rect.Top + ( rect.GetSize().Y - height ) / 2;
				}
				break;
			case EVerticalAlignment.Bottom:
				{
					double height = fontSize * (float)lines.Count + textVerticalIndention * ( (float)lines.Count - 1 );
					startY = rect.Bottom - height;
				}
				break;
			}

			double stepY = fontSize + textVerticalIndention;

			double positionY = startY;
			foreach( TextLineItem line in lines )
			{
				if( line.alignByWidth )
				{
					string[] words = line.text.Split( new char[] { ' ' } );
					float[] lengths = new float[ words.Length ];
					float totalLength = 0;
					for( int n = 0; n < lengths.Length; n++ )
					{
						float length = font.GetTextLength( fontSize, this, words[ n ] );
						lengths[ n ] = length;
						totalLength += length;
					}

					double space = 0;
					if( words.Length > 1 )
						space = ( size.X - totalLength ) / ( words.Length - 1 );

					double posX = rect.Left;
					for( int n = 0; n < words.Length; n++ )
					{
						AddText( font, fontSize, words[ n ], new Vector2( posX, positionY ), EHorizontalAlignment.Left, EVerticalAlignment.Top, color, options );
						posX += lengths[ n ] + space;
					}
				}
				else
				{
					double positionX = 0;
					switch( horizontalAlign )
					{
					case EHorizontalAlignment.Left:
						positionX = rect.Left;
						break;
					case EHorizontalAlignment.Center:
						positionX = rect.Left + ( rect.GetSize().X - font.GetTextLength( fontSize, this, line.text ) ) / 2;
						break;
					case EHorizontalAlignment.Right:
						positionX = rect.Right - font.GetTextLength( fontSize, this, line.text );
						break;
					}

					AddText( font, fontSize, line.text, new Vector2( positionX, positionY ), EHorizontalAlignment.Left, EVerticalAlignment.Top, color, options );
				}

				positionY += stepY;
			}

			return lines.Count;
		}

		public unsafe override void AddLines( IList<TriangleVertex> vertices )
		{
			GetCurrentClipRectangle( out var clipRectangle );

			////check culled
			//if( !clipRectangle.IsCleared() )
			//{
			//	RectF bounds = RectF.Cleared;
			//	foreach( var line in lines )
			//	{
			//		bounds.Add( line.start );
			//		bounds.Add( line.end );
			//	}
			//	if( !clipRectangle.Intersects( bounds ) )
			//		return;
			//}

			Item item = null;

			LinesItemKey key = new LinesItemKey();
			{
				if( vertices is TriangleVertex[] )
					key.vertices = (TriangleVertex[])vertices;
				else
					key.vertices = vertices.ToArray();

				key.clipRectangle = clipRectangle;
				key.shader = GetCurrentShader();
				//key.sourceFileName = sourceFileName;
				//key.textureCount = textureCount;

				key.aspectRatio = aspectRatio;
				//key.outGeometryTransformEnabled = outGeometryTransformEnabled;
				//key.outGeometryTransformScale = outGeometryTransformScale;
				//key.outGeometryTransformOffset = outGeometryTransformOffset;

				key.CalculateHashCode();
			}

			//trying to get item from cache
			if( enableCache )
			{
				if( linesItemCache.TryGetValue( key, out item ) )
				{
					if( !linesItemCache.Remove( key ) )
						Log.Fatal( "CanvasRenderer: AddLines: Internal error." );
				}
			}

			int vertexBufferSize = vertices.Count;

			if( item == null )
			{
				//create renderable item
				RenderableItem renderableItem = GetFreeRenderableItem( GetCurrentShader(), vertexBufferSize );
				SetRenderableItemDynamicData( renderableItem, RenderOperationType.LineList, null, false );

				GpuVertexBuffer vertexBuffer = renderableItem.vertexBuffer;

				byte[] verticesArray = vertexBuffer.WriteBegin();
				fixed( byte* pVertices = verticesArray )
				{
					//IntPtr buffer = vertexBuffer.Lock( HardwareBuffer.LockOptions.Discard, 0, vertexBufferSize * sizeof( BufferVertex ) );
					BufferVertex* bufferPointer = (BufferVertex*)pVertices;

					foreach( var v in vertices )
					{
						var v2 = v;
						//correct destination coordinates
						v2.position = FixResultPosition( v2.position );
						//copy to buffer
						AddTriangleVertexToBuffer( bufferPointer, ref v2 );
						bufferPointer++;
					}

					//vertexBuffer.Unlock();
				}
				vertexBuffer.WriteEnd();

				renderableItem.vertexCount = vertexBufferSize;

				//create item
				item = new Item();
				item.itemKey = key;
				item.renderableItems = new RenderableItem[] { renderableItem };
			}

			//set dynamic data
			SetItemDynamicData( item );

			//add to out list
			outItems.Add( item );
		}

		public unsafe override void AddLines( IList<LineItem> lines )
		{
			//!!!!чтобы друг с другом не блендились, если альфа < 1

			if( lines.Count == 0 )
				return;
			//if( RenderSystem.Instance.IsDeviceLost() )
			//	return;
			if( RenderingSystem.BackendNull )
				return;

			var vertices = new TriangleVertex[ lines.Count * 2 ];
			for( int n = 0; n < lines.Count; n++ )
			{
				var line = lines[ n ];
				vertices[ n * 2 + 0 ] = new TriangleVertex( line.start, line.color, Vector2F.Zero );
				vertices[ n * 2 + 1 ] = new TriangleVertex( line.end, line.color, Vector2F.Zero );
			}

			AddLines( vertices );
		}

		/// <summary>
		/// Adds line to rendering queue.
		/// </summary>
		/// <param name="start">The line start position.</param>
		/// <param name="end">The line end position.</param>
		/// <param name="color">The text color.</param>
		public override void AddLine( Vector2F start, Vector2F end, ColorValue color )
		{
			//if( RenderSystem.Instance.IsDeviceLost() )
			//	return;
			if( RenderingSystem.BackendNull )
				return;

			GetCurrentClipRectangle( out var clipRectangle );

			//check culled
			if( !clipRectangle.IsCleared() )
			{
				if( Math.Max( start.X, end.X ) < clipRectangle.Minimum.X )
					return;
				if( Math.Min( start.X, end.X ) > clipRectangle.Maximum.X )
					return;
				if( Math.Max( start.Y, end.Y ) < clipRectangle.Minimum.Y )
					return;
				if( Math.Min( start.Y, end.Y ) > clipRectangle.Maximum.Y )
					return;
			}

			Vector2F correctedStart = start;
			Vector2F correctedEnd = end;

			////clipRectangle
			//if( !clipRectangle.IsCleared() )
			//{
			//	Vec2F newStart;
			//	Vec2F newEnd;

			//	int count = MathAlgorithms.IntersectRectangleLine( clipRectangle, correctedStart,
			//		correctedEnd, out newStart, out newEnd );

			//	if( count == 0 && !clipRectangle.Contains( correctedStart ) )
			//		return;

			//	if( count == 2 )
			//	{
			//		correctedStart = newStart;
			//		correctedEnd = newEnd;
			//	}

			//	if( count == 1 )
			//	{
			//		if( !clipRectangle.Contains( correctedStart ) )
			//			correctedStart = newStart;
			//		if( !clipRectangle.Contains( correctedEnd ) )
			//			correctedEnd = newStart;
			//	}
			//}
			//else
			//{
			//	//if( IsScreen && !RendererWorld.isEditor )
			//	//{
			//	//   if( !new Rect( 0, 0, 1, 1 ).IsContainsPoint( start ) &&
			//	//      !new Rect( 0, 0, 1, 1 ).IsContainsPoint( end ) )
			//	//   {
			//	//      Vec2 newStart;
			//	//      Vec2 newEnd;

			//	//      slow
			//	//      int count = MathAlgorithms.IntersectRectangleLine( new Rect( 0, 0, 1, 1 ), start, end, 
			//	//         out newStart, out newEnd );

			//	//      if( count == 0 )
			//	//         return;
			//	//   }
			//	//}
			//}

			//uint uintColor = RenderSystem.Instance.ConvertColorValue( ref color );

			Item item = null;

			LineItemKey key = new LineItemKey();
			{
				key.correctedStart = correctedStart;
				key.correctedEnd = correctedEnd;
				key.color = color;
				key.clipRectangle = clipRectangle;
				key.shader = GetCurrentShader();
				//key.sourceFileName = sourceFileName;
				//key.textureCount = textureCount;

				key.aspectRatio = aspectRatio;
				//key.outGeometryTransformEnabled = outGeometryTransformEnabled;
				//key.outGeometryTransformScale = outGeometryTransformScale;
				//key.outGeometryTransformOffset = outGeometryTransformOffset;

				key.CalculateHashCode();
			}

			//trying to get item from cache
			if( enableCache )
			{
				if( lineItemCache.TryGetValue( key, out item ) )
				{
					if( !lineItemCache.Remove( key ) )
						Log.Fatal( "CanvasRenderer: AddLine: Internal error." );
				}
			}

			int vertexBufferSize = lineBufferSize;

			if( item == null )
			{
				//create renderable item
				RenderableItem renderableItem = GetFreeRenderableItem( GetCurrentShader(), vertexBufferSize );
				SetRenderableItemDynamicData( renderableItem, RenderOperationType.LineList, null, false );

				//fill to buffer
				unsafe
				{
					Vector2F destRectMinimum = FixResultPosition( correctedStart );
					Vector2F destRectMaximum = FixResultPosition( correctedEnd );

					//copy data to buffer
					{
						GpuVertexBuffer vertexBuffer = renderableItem.vertexBuffer;

						byte[] vertices = vertexBuffer.WriteBegin();
						fixed( byte* pVertices = vertices )
							AddLineToBuffer( (BufferVertex*)pVertices, ref destRectMinimum, ref destRectMaximum, ref color );
						vertexBuffer.WriteEnd();

						renderableItem.vertexCount = vertexBufferSize;
					}
				}

				//create item
				item = new Item();
				item.itemKey = key;
				item.renderableItems = new RenderableItem[] { renderableItem };
			}

			//set dynamic data
			SetItemDynamicData( item );

			//add to out list
			outItems.Add( item );
		}

		public override void AddRectangle( RectangleF rectangle, ColorValue color )
		{
			AddLines( new LineItem[]
			{
				new LineItem( rectangle.LeftTop, rectangle.RightTop, color ),
				new LineItem( rectangle.RightTop, rectangle.RightBottom, color ),
				new LineItem( rectangle.RightBottom, rectangle.LeftBottom, color ),
				new LineItem( rectangle.LeftBottom, rectangle.LeftTop, color ),
			} );
		}

		unsafe void AddTrianglesWithoutClipRectangles( IList<TriangleVertex> vertices, ImageComponent texture, bool clamp )
		{
			if( vertices.Count == 0 )
				return;

			Item item = null;

			GetCurrentClipRectangle( out var clipRectangle );

			TrianglesItemKey key = new TrianglesItemKey();
			{
				if( vertices is TriangleVertex[] )
					key.vertices = (TriangleVertex[])vertices;
				else
					key.vertices = vertices.ToArray();
				//key.vertices = new TriangleVertex[ vertices.Count ];
				//for( int n = 0; n < vertices.Count; n++ )
				//	key.vertices[ n ] = vertices[ n ];

				key.texture = texture;
				key.clamp = clamp;
				key.clipRectangle = clipRectangle;
				key.shader = GetCurrentShader();
				//key.sourceFileName = sourceFileName;
				//key.textureCount = textureCount;

				key.aspectRatio = aspectRatio;
				//key.outGeometryTransformEnabled = outGeometryTransformEnabled;
				//key.outGeometryTransformScale = outGeometryTransformScale;
				//key.outGeometryTransformOffset = outGeometryTransformOffset;

				key.CalculateHashCode();
			}

			//trying to get item from cache
			if( enableCache )
			{
				if( trianglesItemCache.TryGetValue( key, out item ) )
				{
					if( !trianglesItemCache.Remove( key ) )
						Log.Fatal( "CanvasRenderer: AddTriangles: Internal error." );
				}
			}

			int vertexBufferSize = vertices.Count;

			if( item == null )
			{
				//create renderable item
				RenderableItem renderableItem = GetFreeRenderableItem( GetCurrentShader(), vertexBufferSize );
				SetRenderableItemDynamicData( renderableItem, RenderOperationType.TriangleList, texture, clamp );

				GpuVertexBuffer vertexBuffer = renderableItem.vertexBuffer;

				byte[] verticesArray = vertexBuffer.WriteBegin();
				fixed( byte* pVertices = verticesArray )
				{
					//IntPtr buffer = vertexBuffer.Lock( HardwareBuffer.LockOptions.Discard, 0, vertexBufferSize * sizeof( BufferVertex ) );
					BufferVertex* bufferPointer = (BufferVertex*)pVertices;

					foreach( var v in vertices )
					{
						var v2 = v;
						//correct destination coordinates
						v2.position = FixResultPosition( v2.position );
						//copy to buffer
						AddTriangleVertexToBuffer( bufferPointer, ref v2 );
						bufferPointer++;
					}
					//int triangleCount = vertices.Count / 3;
					//for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
					//{
					//	TriangleVertex vertex0 = vertices[ nTriangle * 3 + 0 ];
					//	TriangleVertex vertex1 = vertices[ nTriangle * 3 + 1 ];
					//	TriangleVertex vertex2 = vertices[ nTriangle * 3 + 2 ];

					//	//correct destination coordinates
					//	vertex0.position = FixResultPosition( vertex0.position );
					//	vertex1.position = FixResultPosition( vertex1.position );
					//	vertex2.position = FixResultPosition( vertex2.position );

					//	//copy to buffer
					//	AddTriangleVertexToBuffer( bufferPointer, ref vertex0 );
					//	bufferPointer++;
					//	AddTriangleVertexToBuffer( bufferPointer, ref vertex1 );
					//	bufferPointer++;
					//	AddTriangleVertexToBuffer( bufferPointer, ref vertex2 );
					//	bufferPointer++;
					//}

					//vertexBuffer.Unlock();
				}
				vertexBuffer.WriteEnd();

				renderableItem.vertexCount = vertexBufferSize;

				//create item
				item = new Item();
				item.itemKey = key;
				item.renderableItems = new RenderableItem[] { renderableItem };
			}

			//set dynamic data
			SetItemDynamicData( item );

			//add to out list
			outItems.Add( item );
		}

		public override void AddFillEllipse( RectangleF rectangle, int segments, ColorValue color, ImageComponent texture, RectangleF textureCoordRectangle, bool textureClamp )
		{
			if( segments < 3 )
				Log.Fatal( "CanvasRenderer: AddFillEllipse: segments < 3." );
			//if( RenderSystem.Instance.IsDeviceLost() )
			//	return;
			if( RenderingSystem.BackendNull )
				return;

			Vector2 rectangleSize = rectangle.GetSize();
			Vector2 texCoordSize = textureCoordRectangle.GetSize();

			Vector2 center = rectangle.GetCenter();

			Vector2 centerTexCoord = Vector2.Zero;
			if( rectangleSize.X != 0 )
				centerTexCoord.X = textureCoordRectangle.Left + texCoordSize.X * ( ( center.X - rectangle.Left ) / rectangleSize.X );
			if( rectangleSize.Y != 0 )
				centerTexCoord.Y = textureCoordRectangle.Top + texCoordSize.Y * ( ( center.Y - rectangle.Top ) / rectangleSize.Y );

			Vector2 halfSize = rectangle.GetSize() * .5f;
			float step = MathEx.PI * 2 / (float)segments;
			int steps = segments + 1;

			TriangleVertex[] vertices = new TriangleVertex[ steps * 3 ];

			float angle = 0;
			Vector2 lastPoint = Vector2.Zero;
			Vector2 lastPointTexCoord = Vector2.Zero;

			for( int n = 0; n < steps; n++ )
			{
				angle += step;

				Vector2 point = center + new Vector2( MathEx.Cos( angle ), MathEx.Sin( angle ) ) * halfSize;

				Vector2 pointTexCoord = Vector2.Zero;
				if( texture != null )
				{
					if( rectangleSize.X != 0 )
						pointTexCoord.X = textureCoordRectangle.Left + texCoordSize.X * ( ( point.X - rectangle.Left ) / rectangleSize.X );
					if( rectangleSize.Y != 0 )
						pointTexCoord.Y = textureCoordRectangle.Top + texCoordSize.Y * ( ( point.Y - rectangle.Top ) / rectangleSize.Y );
				}

				if( n != 0 )
				{
					vertices[ n * 3 + 0 ] = new TriangleVertex( center.ToVector2F(), color, centerTexCoord.ToVector2F() );
					vertices[ n * 3 + 1 ] = new TriangleVertex( lastPoint.ToVector2F(), color, lastPointTexCoord.ToVector2F() );
					vertices[ n * 3 + 2 ] = new TriangleVertex( point.ToVector2F(), color, pointTexCoord.ToVector2F() );
				}

				lastPoint = point;
				lastPointTexCoord = pointTexCoord;
			}

			AddTriangles( vertices, texture, textureClamp );
		}

		public override void AddEllipse( RectangleF rectangle, int segments, ColorValue color )
		{
			if( segments < 3 )
				Log.Fatal( "CanvasRenderer: AddEllipse: segments < 3." );
			//if( RenderSystem.Instance.IsDeviceLost() )
			//	return;
			if( RenderingSystem.BackendNull )
				return;

			var center = rectangle.GetCenter();
			var halfSize = rectangle.Size * 0.5f;

			var lines = new LineItem[ segments ];

			float step = MathEx.PI * 2 / (float)segments;
			int steps = segments + 1;

			float angle = 0;
			Vector2F lastPoint = Vector2F.Zero;

			for( int n = 0; n < steps; n++ )
			{
				var point = center + new Vector2F( MathEx.Cos( angle ), MathEx.Sin( angle ) ) * halfSize;

				if( n != 0 )
					lines[ n - 1 ] = new LineItem( lastPoint, point, color );

				angle += step;
				lastPoint = point;
			}

			AddLines( lines );
		}

		//static bool IsInside( Vec2F position, ref RectF clipRectangle, int clipSide )
		//{
		//	switch( clipSide )
		//	{
		//	case 0: return position.X >= clipRectangle.Left;
		//	case 1: return position.Y >= clipRectangle.Top;
		//	case 2: return position.X <= clipRectangle.Right;
		//	case 3: return position.Y <= clipRectangle.Bottom;
		//	}
		//	return false;
		//}

		//static float LineIntersection( Vec2F position1, Vec2F position2, ref RectF clipRectangle, int clipSide )
		//{
		//	float scale = 0;

		//	MathAlgorithms.IntersectLineLine( position1, position2,
		//		new Vec2F( clipRectangle.Left, -1000 ), new Vec2F( clipRectangle.Left, 1000 ), out scale );

		//	switch( clipSide )
		//	{
		//	case 0:
		//		MathAlgorithms.IntersectLineLine( position1, position2,
		//			new Vec2F( clipRectangle.Left, -1000 ), new Vec2F( clipRectangle.Left, 1000 ),
		//			out scale );
		//		break;

		//	case 1:
		//		MathAlgorithms.IntersectLineLine( position1, position2,
		//			new Vec2F( -1000, clipRectangle.Top ), new Vec2F( 1000, clipRectangle.Top ),
		//			out scale );
		//		break;

		//	case 2:
		//		MathAlgorithms.IntersectLineLine( position1, position2,
		//			new Vec2F( clipRectangle.Right, -1000 ), new Vec2F( clipRectangle.Right, 1000 ),
		//			out scale );
		//		break;

		//	case 3:
		//		MathAlgorithms.IntersectLineLine( position1, position2,
		//			new Vec2F( -1000, clipRectangle.Bottom ), new Vec2F( 1000, clipRectangle.Bottom ),
		//			out scale );
		//		break;
		//	}

		//	return scale;
		//}

		//static TriangleVertex CreateVertex( ref TriangleVertex vertex1, ref TriangleVertex vertex2, float scale )
		//{
		//	TriangleVertex result = new TriangleVertex();
		//	result.position = vertex1.position + ( vertex2.position - vertex1.position ) * scale;
		//	result.color = vertex1.color + ( vertex2.color - vertex1.color ) * scale;
		//	result.texCoord = vertex1.texCoord + ( vertex2.texCoord - vertex1.texCoord ) * scale;
		//	return result;
		//}

		//static void ClipTriangles( IList<TriangleVertex> input, ref RectF clipRectangle, int clipSide, List<TriangleVertex> output )
		//{
		//	int triangleCount = input.Count / 3;
		//	for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
		//	{
		//		TriangleVertex vertex0 = input[ nTriangle * 3 + 0 ];
		//		TriangleVertex vertex1 = input[ nTriangle * 3 + 1 ];
		//		TriangleVertex vertex2 = input[ nTriangle * 3 + 2 ];

		//		Vec2F position0 = vertex0.position;
		//		Vec2F position1 = vertex1.position;
		//		Vec2F position2 = vertex2.position;

		//		bool inside0 = IsInside( position0, ref clipRectangle, clipSide );
		//		bool inside1 = IsInside( position1, ref clipRectangle, clipSide );
		//		bool inside2 = IsInside( position2, ref clipRectangle, clipSide );

		//		if( inside0 && inside1 && inside2 )
		//		{
		//			output.Add( vertex0 ); output.Add( vertex1 ); output.Add( vertex2 );
		//		}
		//		else if( !inside0 && !inside1 && !inside2 )
		//		{
		//		}
		//		else if( inside0 && inside1 && !inside2 )
		//		{
		//			float scale;
		//			scale = LineIntersection( position0, position2, ref clipRectangle, clipSide );
		//			TriangleVertex split1 = CreateVertex( ref vertex0, ref vertex2, scale );
		//			scale = LineIntersection( position1, position2, ref clipRectangle, clipSide );
		//			TriangleVertex split2 = CreateVertex( ref vertex1, ref vertex2, scale );

		//			output.Add( vertex0 ); output.Add( vertex1 ); output.Add( split2 );
		//			output.Add( split2 ); output.Add( split1 ); output.Add( vertex0 );
		//		}
		//		else if( !inside0 && !inside1 && inside2 )
		//		{
		//			float scale;
		//			scale = LineIntersection( position0, position2, ref clipRectangle, clipSide );
		//			TriangleVertex split1 = CreateVertex( ref vertex0, ref vertex2, scale );
		//			scale = LineIntersection( position1, position2, ref clipRectangle, clipSide );
		//			TriangleVertex split2 = CreateVertex( ref vertex1, ref vertex2, scale );

		//			output.Add( vertex2 ); output.Add( split1 ); output.Add( split2 );
		//		}
		//		else if( inside0 && inside2 && !inside1 )
		//		{
		//			float scale;
		//			scale = LineIntersection( position0, position1, ref clipRectangle, clipSide );
		//			TriangleVertex split1 = CreateVertex( ref vertex0, ref vertex1, scale );
		//			scale = LineIntersection( position1, position2, ref clipRectangle, clipSide );
		//			TriangleVertex split2 = CreateVertex( ref vertex1, ref vertex2, scale );

		//			output.Add( vertex0 ); output.Add( split1 ); output.Add( split2 );
		//			output.Add( split2 ); output.Add( vertex2 ); output.Add( vertex0 );
		//		}
		//		else if( !inside0 && !inside2 && inside1 )
		//		{
		//			float scale;
		//			scale = LineIntersection( position0, position1, ref clipRectangle, clipSide );
		//			TriangleVertex split1 = CreateVertex( ref vertex0, ref vertex1, scale );
		//			scale = LineIntersection( position1, position2, ref clipRectangle, clipSide );
		//			TriangleVertex split2 = CreateVertex( ref vertex1, ref vertex2, scale );

		//			output.Add( vertex1 ); output.Add( split2 ); output.Add( split1 );
		//		}
		//		else if( inside1 && inside2 && !inside0 )
		//		{
		//			float scale;
		//			scale = LineIntersection( position0, position1, ref clipRectangle, clipSide );
		//			TriangleVertex split1 = CreateVertex( ref vertex0, ref vertex1, scale );
		//			scale = LineIntersection( position0, position2, ref clipRectangle, clipSide );
		//			TriangleVertex split2 = CreateVertex( ref vertex0, ref vertex2, scale );

		//			output.Add( vertex1 ); output.Add( vertex2 ); output.Add( split2 );
		//			output.Add( split2 ); output.Add( split1 ); output.Add( vertex1 );
		//		}
		//		else if( !inside1 && !inside2 && inside0 )
		//		{
		//			float scale;
		//			scale = LineIntersection( position0, position1, ref clipRectangle, clipSide );
		//			TriangleVertex split1 = CreateVertex( ref vertex0, ref vertex1, scale );
		//			scale = LineIntersection( position0, position2, ref clipRectangle, clipSide );
		//			TriangleVertex split2 = CreateVertex( ref vertex0, ref vertex2, scale );

		//			output.Add( vertex0 ); output.Add( split1 ); output.Add( split2 );
		//		}
		//	}
		//}

		public override void AddTriangles( IList<TriangleVertex> vertices, ImageComponent texture, bool clamp )
		{
			if( vertices.Count == 0 )
				return;
			if( vertices.Count % 3 != 0 )
				Log.Fatal( "CanvasRenderer: AddTriangles: Invalid vertex count." );
			//if( RenderSystem.Instance.IsDeviceLost() )
			//	return;
			if( RenderingSystem.BackendNull )
				return;

			//!!!!можно провер€ть не отсечено ли полностью

			AddTrianglesWithoutClipRectangles( vertices, texture, clamp );
			//if( !clipRectangle.IsCleared() )
			//{
			//	bool wholeInside = true;
			//	for( int n = 0; n < vertices.Count; n++ )
			//	{
			//		if( !clipRectangle.Contains( vertices[ n ].position ) )
			//		{
			//			wholeInside = false;
			//			break;
			//		}
			//	}

			//	if( !wholeInside )
			//	{
			//		tempVertices1.Clear();
			//		ClipTriangles( vertices, ref clipRectangle, 0, tempVertices1 );

			//		tempVertices2.Clear();
			//		ClipTriangles( tempVertices1, ref clipRectangle, 1, tempVertices2 );

			//		tempVertices1.Clear();
			//		ClipTriangles( tempVertices2, ref clipRectangle, 2, tempVertices1 );

			//		tempVertices2.Clear();
			//		ClipTriangles( tempVertices1, ref clipRectangle, 3, tempVertices2 );

			//		AddTrianglesWithoutClipRectangles( tempVertices2, texture, clamp );

			//		tempVertices1.Clear();
			//		tempVertices2.Clear();
			//	}
			//	else
			//		AddTrianglesWithoutClipRectangles( vertices, texture, clamp );
			//}
			//	else
			//		AddTrianglesWithoutClipRectangles( vertices, texture, clamp );
		}

		public override void AddTriangles( IList<TriangleVertex> vertices, IList<int> indices, ImageComponent texture = null, bool clamp = false )
		{
			if( indices != null )
			{
				if( indices.Count % 3 != 0 )
					Log.Fatal( "CanvasRenderer: AddTriangles: Invalid index count." );

				//!!!!может не разворачивать индексы
				var vertices2 = new TriangleVertex[ indices.Count ];
				for( int n = 0; n < vertices2.Length; n++ )
					vertices2[ n ] = vertices[ indices[ n ] ];
				AddTriangles( vertices2, texture, clamp );
			}
			else
				AddTriangles( vertices, texture, clamp );
		}

		/// <summary>
		/// Gets or sets the default font.
		/// </summary>
		public override FontComponent DefaultFont
		{
			get { return defaultFont; }
			set { defaultFont = value; }
		}

		/// <summary>
		/// Gets or sets the default font size.
		/// </summary>
		public override double DefaultFontSize { get; set; } = 0.02;

		/// <summary>
		/// Gets or sets value which indicates what is it renderer is screen renderer.
		/// </summary>
		public override bool IsScreen
		{
			get { return isScreen; }
		}

		public override Viewport ViewportForScreenCanvasRenderer
		{
			get { return viewportForScreenCanvasRenderer; }
		}

		//!!!!
		//public GuiSceneObject ParentGuiSceneObjectFor3DRendering
		//{
		//	get { return parentGuiSceneObjectFor3DRendering; }
		//}

		void InitInternal()
		{
			//!!!!threading
			//defaultFont
			defaultFont = ResourceManager.LoadResource<FontComponent>( @"Base\Fonts\Default.ttf" );
			//defaultFont = EngineFontManager.Instance.LoadFont( "Default", DefaultFontSize );
			//if( defaultFont == null )
			//	defaultFont = EngineFontManager.Instance.LoadFont( "Default", "English", DefaultFontSize );

			//add RenderSystem listener event for "DeviceRestored"
			RenderingSystem.RenderSystemEvent += RenderSystem_RenderSystemEvent;
		}

		void ShutdownInternal()
		{
			//remove RenderSystem listener event for "DeviceRestored"
			RenderingSystem.RenderSystemEvent -= RenderSystem_RenderSystemEvent;
		}

		//		void RenderEngineLogo( ViewportRenderingContext context )
		//		{
		//			if( !logoInitialTimeFinished && EngineApp.CreatedInsideEngineWindow != null && EngineApp.IsSimulation )
		//			{
		//				var viewport = context.CurrentViewport;
		//				if( viewport == RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ] )
		//				{
		//					//create
		//					if( watermarkTexture == null )
		//					{
		//						Image texture;

		//						if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP || SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
		//						{
		//							//load watermark from file for UWP

		//							string name = viewport.SizeInPixels.X >= 1600 ? "Watermark.png" : "Watermark256.png";
		//							texture = ResourceManager.LoadResource<Image>( Path.Combine( @"Base\UI\Images\", name ) );
		//							if( texture.Result == null )
		//							{
		//								//warn ? logo file not found / not loaded
		//								logoInitialTimeFinished = true;
		//								return;
		//							}
		//							//texture.CreateFormat = PixelFormat.A8R8G8B8;
		//						}
		//						else
		//						{
		//							//default behaviour

		//#if !ANDROID
		//							var image = viewport.SizeInPixels.X >= 1600 ? Properties.Resources.Watermark : Properties.Resources.Watermark256;

		//							var size = new Vector2I( image.Size.Width, image.Size.Height );

		//							byte[] data = new byte[ size.X * size.Y * 4 ];
		//							unsafe
		//							{
		//								fixed ( byte* pData = data )
		//								{
		//									int* p = (int*)pData;

		//									for( int y = 0; y < size.Y; y++ )
		//									{
		//										for( int x = 0; x < size.X; x++ )
		//										{
		//											var c = image.GetPixel( x, y );

		//											*p = c.ToArgb();
		//											p++;
		//										}
		//									}
		//								}
		//							}

		//							texture = ComponentUtility.CreateComponent<Image>( null, true, false );

		//							texture.CreateType = Image.TypeEnum._2D;
		//							texture.CreateSize = size;
		//							texture.CreateMipmaps = false;
		//							texture.CreateFormat = PixelFormat.A8R8G8B8;
		//							texture.CreateUsage = Image.Usages.WriteOnly;
		//							texture.Enabled = true;

		//							texture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( 0, 0, data ) } );
		//#else //ANDROID
		//							texture = null;
		//#endif //!ANDROID
		//						}

		//						watermarkTexture = texture;
		//					}

		//					//render
		//					var viewportAspect = (double)viewport.SizeInPixels.X / (double)viewport.SizeInPixels.Y;
		//					var imageAspect = (double)watermarkTexture.Result.ResultSize.X / (double)watermarkTexture.Result.ResultSize.Y;
		//					var s = new Vector2( 0.12, 0.12 * viewportAspect / imageAspect );
		//					AddQuad( new Rectangle( 1.0 - s.X, 1.0 - s.Y, 1, 1 ), new Rectangle( 0, 0, 1, 1 ), watermarkTexture, new ColorValue( 1, 1, 1 ), true );

		//					//update time, state
		//					double timeInSeconds = 5;
		//					if( logoInitialTime == null )
		//						logoInitialTime = EngineApp.GetSystemTime();
		//					if( EngineApp.GetSystemTime() >= logoInitialTime.Value + timeInSeconds )
		//						logoInitialTimeFinished = true;
		//				}
		//			}
		//		}

		[MethodImpl( (MethodImplOptions)512 )]
		public override unsafe void ViewportRendering_RenderToCurrentViewport( ViewportRenderingContext context, bool clearData, double time, bool registerUniformsAndSetIdentityMatrix )
		{
			if( updateTime != time )
				updateTimePrevious = updateTime;
			updateTime = time;

			////engine logo
			//RenderEngineLogo( context );

			CheckForAllPushedParameters();

			//3d gui
			//Matrix4 worldTransform;
			//_this->getWorldTransform( worldTransform );
			//RenderingSystem.CallCustomMethod( "_setWorldMatrix", &identity );

			if( registerUniformsAndSetIdentityMatrix )
			{
				if( !u_canvasClipRectangleUniform.HasValue )
					u_canvasClipRectangleUniform = GpuProgramManager.RegisterUniform( "u_canvasClipRectangle", UniformType.Vector4, 1 );
				if( !u_bc5UNormLUniform.HasValue )
					u_bc5UNormLUniform = GpuProgramManager.RegisterUniform( "u_bc5UNorm_L", UniformType.Vector4, 1 );
				if( !u_canvasColorMultiplier.HasValue )
					u_canvasColorMultiplier = GpuProgramManager.RegisterUniform( "u_canvasColorMultiplier", UniformType.Vector4, 1 );
				if( !u_canvasOcclusionDepthCheck.HasValue )
					u_canvasOcclusionDepthCheck = GpuProgramManager.RegisterUniform( "u_canvasOcclusionDepthCheck", UniformType.Vector4, 1 );

				Matrix4F identity = Matrix4F.Identity;
				Bgfx.SetTransform( (float*)&identity );
			}

			context.ObjectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out ImageComponent depthTexture );

			for( int nItem = 0; nItem < outItems.Count; nItem++ )
			{
				var item = outItems[ nItem ];

				var shader = item.itemKey.shader;

				//get parameters from shader
				//!!!!can optimize
				var itemContainer = shader.Parameters;

				foreach( RenderableItem renderableItem in item.renderableItems )
				{
					if( renderableItem.vertexBuffer.Disposed )
						continue;

					//!!!!!может все пассы. хот€ по дефолту всегда один

					var pass = renderableItem.material.MaterialPass;// Result.AllPasses[ 0 ];
					if( pass != null )
					{
						//bind texture
						{
							//!!!!а если не загружена еще? то ниже подставл€ть другую? какую?
							var texture = renderableItem.texture;
							if( texture == null )
								texture = ResourceUtility.WhiteTexture2D;

							var minMag = renderableItem.textureFiltering == TextureFilteringMode.Linear ? FilterOption.Linear : FilterOption.Point;

							context.BindTexture( 0, texture, renderableItem.textureClamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap, minMag, minMag, FilterOption.None, 0, false );

							//GpuMaterialPass.TextureParameterValue textureValue = new GpuMaterialPass.TextureParameterValue( texture,
							//	renderableItem.textureClamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap,
							//	minMag, minMag, FilterOption.None );

							//context.BindTexture( 0, textureValue );
							//generalContainer.Set( "0", textureValue, ParameterType.Texture2D );//"baseTexture"
						}

						//!!!!impl mobile
						//bind depth texture
						if( !SystemSettings.MobileDevice )
						{
							context.BindTexture( 1/* "depthTexture"*/, depthTexture ?? ResourceUtility.WhiteTexture2D, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
						}


						var scissorEnabled = false;

						//doing scissor and clip rectangle both because just scissor can not clip one pixel line

						if( IsScreen )
						{
							ref var clip = ref item.itemKey.clipRectangle;
							if( !clip.IsCleared() && ( clip.Left > 0 || clip.Top > 0 || clip.Right < 1 || clip.Bottom < 1 ) )
							{
								var sizeInPixels = context.CurrentViewport.SizeInPixels;

								var size = clip.Size;

								var x = (int)( clip.Left * sizeInPixels.X );
								var y = (int)( clip.Top * sizeInPixels.Y );
								var width = (int)Math.Ceiling( size.X * sizeInPixels.X );
								var height = (int)Math.Ceiling( size.Y * sizeInPixels.Y );

								Bgfx.SetScissor( x, y, width, height );
								scissorEnabled = true;
							}
						}
						else
						{
						}

						//bind clip rectangle
						{
							var clipRectangle = item.itemKey.clipRectangle;
							if( ( IsScreen && clipRectangle == new RectangleF( 0, 0, 1, 1 ) ) || clipRectangle.IsCleared() )
								clipRectangle = new RectangleF( -10000, -10000, 20000, 20000 );
							if( clipRectangle != canvasClipRectangleBinded )
							{
								canvasClipRectangleBinded = clipRectangle;
								Bgfx.SetUniform( u_canvasClipRectangleUniform.Value, &clipRectangle, 1 );
							}
						}

						//bind u_bc5UNorm_L8
						{
							bool bc5UNorm = false;
							bool luminance = false;
							var result = renderableItem.texture?.Result;
							if( result != null )
							{
								bc5UNorm = result.ResultFormat == PixelFormat.BC5_UNorm;
								luminance = result.ResultFormat == PixelFormat.L8 || result.ResultFormat == PixelFormat.L16;
							}
							var value = new Vector4F( bc5UNorm ? 1 : 0, luminance ? 1 : 0, 0, 0 );
							if( value != bc5UNormLBinded )
							{
								bc5UNormLBinded = value;
								Bgfx.SetUniform( u_bc5UNormLUniform.Value, &value, 1 );
							}
						}

						//bind u_canvasColorMultiplier
						if( canvasColorMultiplierBinded != item.colorMultiplier )
						{
							canvasColorMultiplierBinded = item.colorMultiplier;
							var v = item.colorMultiplier;
							Bgfx.SetUniform( u_canvasColorMultiplier.Value, &v, 1 );
						}

						//bind u_canvasOcclusionDepthCheck
						if( canvasOcclusionDepthCheckBinded != item.occlusionDepthCheck )
						{
							canvasOcclusionDepthCheckBinded = item.occlusionDepthCheck;
							var v = item.occlusionDepthCheck;
							Bgfx.SetUniform( u_canvasOcclusionDepthCheck.Value, &v, 1 );
						}


						List<ParameterContainer> containers = null;
						if( itemContainer != null || shader.AdditionalParameterContainersExists )
						{
							//!!!!GC
							containers = new List<ParameterContainer>();

							if( itemContainer != null )
								containers.Add( itemContainer );
							if( shader.AdditionalParameterContainersExists )
								containers.AddRange( shader.AdditionalParameterContainers );
						}


						//!!!!!!gpu parameters
						//надо обновить gpu параметры дл€ выставленныъ программ
						//а также выставить текстуры

						//!!!!!!
						//virtual void _setTextureLayerAnisotropy( size_t unit, unsigned int maxAnisotropy ) = 0;
						//virtual void _setTextureMipmapBias( size_t unit, float bias ) = 0;



						//!!!!!нужное из RenderSystem
						//Stencil
						//virtual void setStencilCheckEnabled( bool enabled ) = 0;
						//virtual void setStencilBufferParams( CompareFunction func = CMPF_ALWAYS_PASS,
						//	uint32 refValue = 0, uint32 mask = 0xFFFFFFFF,
						//	StencilOperation stencilFailOp = SOP_KEEP,
						//	StencilOperation depthFailOp = SOP_KEEP,
						//	StencilOperation passOp = SOP_KEEP,
						//	bool twoSidedOperation = false ) = 0;

						//!!!!!нужное из RenderSystem
						//Clip planes
						//virtual void setClipPlanes(const PlaneList& clipPlanes);
						//virtual void resetClipPlanes();


						context.SetVertexBuffer( 0, renderableItem.vertexBuffer, 0, renderableItem.vertexCount );
						context.SetPassAndSubmit( pass, renderableItem.renderOperation, containers, null, false, true );

						if( scissorEnabled )
							Bgfx.SetScissor( -1 );

						if( renderableItem.renderOperation == RenderOperationType.TriangleList )
							context.UpdateStatisticsCurrent.Triangles += renderableItem.vertexCount / 3;
						else if( renderableItem.renderOperation == RenderOperationType.LineList )
							context.UpdateStatisticsCurrent.Lines += renderableItem.vertexCount / 2;
						context.UpdateStatisticsCurrent.Instances++;
					}
				}
			}

			Bgfx.Discard( DiscardFlags.All );

			if( clearData )
				ViewportRendering_Clear( updateTime );
		}

		void DestroyItem( Item item )
		{
			foreach( RenderableItem renderableItem in item.renderableItems )
				FreeRenderableItem( renderableItem );
		}

		void DestroyRenderableItemsInCaches( double time )
		{
			if( quadItemCache.Count != 0 )
			{
				foreach( Item item in quadItemCache.Values )
					DestroyItem( item );
				quadItemCache.Clear();
			}

			if( textItemCache.Count != 0 )
			{
				foreach( Item item in textItemCache.Values )
					DestroyItem( item );
				textItemCache.Clear();
			}

			if( lineItemCache.Count != 0 )
			{
				foreach( Item item in lineItemCache.Values )
					DestroyItem( item );
				lineItemCache.Clear();
			}

			if( trianglesItemCache.Count != 0 )
			{
				foreach( Item item in trianglesItemCache.Values )
					DestroyItem( item );
				trianglesItemCache.Clear();
			}

			if( linesItemCache.Count != 0 )
			{
				foreach( Item item in linesItemCache.Values )
					DestroyItem( item );
				linesItemCache.Clear();
			}
		}

		void DestroyLongTimeNotUsedFreeRenderableItems( double time )
		{
			for( int nGroup = 0; nGroup < freeRenderableItems.Count; nGroup++ )
			{
				FreeRenderableItemGroup group = freeRenderableItems[ nGroup ];

				bool existsNeedForDeletion = false;
				foreach( RenderableItem renderableItem in group.list )
				{
					bool needDelete =
						renderableItem.lastReleaseRenderTime != updateTime &&
						renderableItem.lastReleaseRenderTime != updateTimePrevious;

					if( needDelete )
					{
						existsNeedForDeletion = true;
						break;
					}
				}

				if( existsNeedForDeletion )
				{
					List<RenderableItem> newList = new List<RenderableItem>( group.list.Count );

					foreach( RenderableItem renderableItem in group.list )
					{
						bool needDelete =
							renderableItem.lastReleaseRenderTime != updateTime &&
							renderableItem.lastReleaseRenderTime != updateTimePrevious;

						if( needDelete )
							DestroyRenderableItem( renderableItem );
						else
							newList.Add( renderableItem );
					}

					group.list = newList;
				}

				//delete empty group
				if( group.list.Count == 0 )
				{
					freeRenderableItems.RemoveAt( nGroup );
					nGroup--;
				}
			}
		}

		//!!!!public?
		internal void ViewportRendering_Clear( double time )
		{
			if( updateTime != time )
				updateTimePrevious = updateTime;
			updateTime = time;

			//!!!!!вызывать

			if( !Log.FatalActivated )
				CheckForAllPushedParameters();

			DestroyRenderableItemsInCaches( time );

			//move used renderable items to freeRenderableItems.
			for( int nItem = 0; nItem < outItems.Count; nItem++ )
			{
				var item = outItems[ nItem ];

				//!!!!slowly?

				{
					var itemKey = item.itemKey as QuadItemKey;
					if( itemKey != null )
					{
						if( !quadItemCache.ContainsKey( itemKey ) )
							quadItemCache.Add( itemKey, item );
						else
							DestroyItem( item );
						continue;
					}
				}

				{
					var itemKey = item.itemKey as TextItemKey;
					if( itemKey != null )
					{
						if( !textItemCache.ContainsKey( itemKey ) )
							textItemCache.Add( itemKey, item );
						else
							DestroyItem( item );
						continue;
					}
				}

				{
					var itemKey = item.itemKey as LineItemKey;
					if( itemKey != null )
					{
						if( !lineItemCache.ContainsKey( itemKey ) )
							lineItemCache.Add( itemKey, item );
						else
							DestroyItem( item );
						continue;
					}
				}

				{
					var itemKey = item.itemKey as TrianglesItemKey;
					if( itemKey != null )
					{
						if( !trianglesItemCache.ContainsKey( itemKey ) )
							trianglesItemCache.Add( itemKey, item );
						else
							DestroyItem( item );
						continue;
					}
				}

				{
					var itemKey = item.itemKey as LinesItemKey;
					if( itemKey != null )
					{
						if( !linesItemCache.ContainsKey( itemKey ) )
							linesItemCache.Add( itemKey, item );
						else
							DestroyItem( item );
						continue;
					}
				}
			}

			outItems.Clear();

			//check for delete free renderable items
			DestroyLongTimeNotUsedFreeRenderableItems( time );
		}

		internal void DestroyAllRenderableItems()
		{
			ViewportRendering_Clear( 0 );
			DestroyRenderableItemsInCaches( 0 );

			//destroy free renderable items
			{
				foreach( FreeRenderableItemGroup group in freeRenderableItems )
				{
					foreach( RenderableItem renderableItem in group.list )
						DestroyRenderableItem( renderableItem );
					group.list.Clear();
				}
				freeRenderableItems.Clear();
			}
		}

		unsafe void AddQuadToBuffer( BufferVertex* buffer, ref RectangleF rectangle, ref RectangleF texCoordRectangle, /*Texture texture, */ ref ColorValue color )
		{
			buffer[ 0 ].color = color;
			buffer[ 1 ].color = color;
			buffer[ 2 ].color = color;
			buffer[ 4 ].color = color;

			//if( texture != null )
			//{
			buffer[ 0 ].texCoord = new Vector2F( texCoordRectangle.Left, texCoordRectangle.Top );
			buffer[ 1 ].texCoord = new Vector2F( texCoordRectangle.Left, texCoordRectangle.Bottom );
			buffer[ 2 ].texCoord = new Vector2F( texCoordRectangle.Right, texCoordRectangle.Bottom );
			buffer[ 4 ].texCoord = new Vector2F( texCoordRectangle.Right, texCoordRectangle.Top );
			//}
			//else
			//{
			//	buffer[ 0 ].texCoord = Vec2.Zero;
			//	buffer[ 1 ].texCoord = Vec2.Zero;
			//	buffer[ 2 ].texCoord = Vec2.Zero;
			//	buffer[ 4 ].texCoord = Vec2.Zero;
			//}

			buffer[ 0 ].position = new Vector3F( rectangle.Minimum.X, rectangle.Minimum.Y, 0 );
			buffer[ 1 ].position = new Vector3F( rectangle.Minimum.X, rectangle.Maximum.Y, 0 );
			buffer[ 2 ].position = new Vector3F( rectangle.Maximum.X, rectangle.Maximum.Y, 0 );
			buffer[ 3 ] = buffer[ 2 ];
			buffer[ 4 ].position = new Vector3F( rectangle.Maximum.X, rectangle.Minimum.Y, 0 );
			buffer[ 5 ] = buffer[ 0 ];

			//if( outGeometryTransformEnabled )
			//{
			//	for( int n = 0; n < 6; n++ )
			//	{
			//		Vec2F v = buffer[ n ].position.ToVec2() * outGeometryTransformScale + outGeometryTransformOffset;
			//		buffer[ n ].position = new Vec3F( v, 0 );
			//	}
			//}
		}

		unsafe void AddLineToBuffer( BufferVertex* buffer, ref Vector2F start, ref Vector2F end, ref ColorValue color )
		{
			buffer[ 0 ].color = color;
			buffer[ 1 ].color = color;
			buffer[ 0 ].texCoord = Vector2F.Zero;
			buffer[ 1 ].texCoord = Vector2F.Zero;
			buffer[ 0 ].position = new Vector3F( start.X, start.Y, 0 );
			buffer[ 1 ].position = new Vector3F( end.X, end.Y, 0 );

			//if( outGeometryTransformEnabled )
			//{
			//	for( int n = 0; n < 2; n++ )
			//	{
			//		Vec2F v = buffer[ n ].position.ToVec2() * outGeometryTransformScale + outGeometryTransformOffset;
			//		buffer[ n ].position = new Vec3F( v, 0 );
			//	}
			//}
		}

		unsafe void AddTriangleVertexToBuffer( BufferVertex* buffer, ref TriangleVertex vertex )
		{
			buffer->position = new Vector3F( vertex.position.X, vertex.position.Y, 0 );
			buffer->texCoord = vertex.texCoord;
			buffer->color = vertex.color;// RenderSystem.Instance.ConvertColorValue( vertex.Color );
										 //if( outGeometryTransformEnabled )
										 //{
										 //	Vec2F v = buffer->position.ToVec2() * outGeometryTransformScale + outGeometryTransformOffset;
										 //	buffer->position = new Vec3F( v, 0 );
										 //}
		}

		void RenderSystem_RenderSystemEvent( RenderSystemEvent name )
		{
			//!!!!!threading. везде разрулить

			if( name == RenderSystemEvent.DeviceLost || name == RenderSystemEvent.DeviceRestored )
				DestroyAllRenderableItems();
		}

		//!!!!
		//internal void RemoveCachedTexture( Image texture )
		//{
		//	//!!!!!!где юзаетс€. медленно ведь

		//	replyQuad:
		//	foreach( KeyValuePair<QuadItemKey, Item> pair in quadItemCache )
		//	{
		//		QuadItemKey itemKey = pair.Key;
		//		Item item = pair.Value;

		//		if( itemKey.texture == texture )
		//		{
		//			DestroyItem( item );
		//			quadItemCache.Remove( itemKey );
		//			goto replyQuad;
		//		}
		//	}

		//	replyText:
		//	foreach( KeyValuePair<TextItemKey, Item> pair in textItemCache )
		//	{
		//		TextItemKey itemKey = pair.Key;
		//		Item item = pair.Value;

		//		if( itemKey.font != null && itemKey.font.IsContainsTexture( texture ) )
		//		{
		//			DestroyItem( item );
		//			textItemCache.Remove( itemKey );
		//			goto replyText;
		//		}
		//	}

		//	replyTriangles:
		//	foreach( KeyValuePair<TrianglesItemKey, Item> pair in trianglesItemCache )
		//	{
		//		TrianglesItemKey itemKey = pair.Key;
		//		Item item = pair.Value;

		//		if( itemKey.texture == texture )
		//		{
		//			DestroyItem( item );
		//			trianglesItemCache.Remove( itemKey );
		//			goto replyTriangles;
		//		}
		//	}
		//}


		////!!!!temp

		//public override bool _OutGeometryTransformEnabled
		//{
		//	get { return outGeometryTransformEnabled; }
		//	set { outGeometryTransformEnabled = value; }
		//}

		//public override Vec2F _OutGeometryTransformScale
		//{
		//	get { return outGeometryTransformScale; }
		//	set { outGeometryTransformScale = value; }
		//}

		//public override Vec2F _OutGeometryTransformOffset
		//{
		//	get { return outGeometryTransformOffset; }
		//	set { outGeometryTransformOffset = value; }
		//}


		public override void PushShader( ShaderItem shader )
		{
			shadersStack.Push( shader );
		}

		public override void PopShader()
		{
			if( shadersStack.Count == 0 )
				Log.Fatal( "CanvasRenderer: PopShader: shadersStack.Count == 0." );
			shadersStack.Pop();
		}

		unsafe RenderableItem CreateRenderableItem( ShaderItem shader, int bufferVertexCount )
		{
			RenderableItem renderableItem = new RenderableItem();

			//create vertex data
			{
				var declaration = new VertexLayout().Begin()
					.Add( VertexAttributeUsage.Position, 3, VertexAttributeType.Float )
					.Add( VertexAttributeUsage.Color0, 4, VertexAttributeType.Float )
					.Add( VertexAttributeUsage.TexCoord0, 2, VertexAttributeType.Float )
					.End();
				//VertexElement[] vertexElements = new VertexElement[]
				//{
				//	new VertexElement( 0, 0, VertexElementType.Float3, VertexElementSemantic.Position ),
				//	new VertexElement( 0, 12, VertexElementType.Float4, VertexElementSemantic.Diffuse ),
				//	new VertexElement( 0, 28, VertexElementType.Float2, VertexElementSemantic.TextureCoordinates ),
				//};
				//renderableItem.vertexDeclaration = GpuBufferManager.CreateVertexDeclaration( vertexElements );

				if( sizeof( BufferVertex ) != 36 )
					Log.Fatal( "CanvasRenderer: CreateRenderableItem: sizeof(BufferVertex) != 36." );
				if( declaration.Stride != 36 )
					Log.Fatal( "CanvasRenderer: CreateRenderableItem: declaration.Stride != 36." );

				byte[] vertices = new byte[ declaration.Stride * bufferVertexCount ];
				renderableItem.vertexBuffer = GpuBufferManager.CreateVertexBuffer( vertices, declaration, GpuBufferFlags.Dynamic );
				//renderableItem.vertexBuffer = GpuBufferManager.CreateVertexBuffer(
				//	renderableItem.vertexDeclaration.VertexSize, bufferVertexCount, null, true );
				if( renderableItem.vertexBuffer == null )
					return null;
			}

			//create material
			renderableItem.material = new MaterialData( isScreen, shader );
			//renderableItem.material = ComponentUtility.CreateComponent<CanvasRendererMaterial>( new object[] { isScreen, shader }, true, true );

			//create renderable
			//{
			//renderableItem.renderable = (GuiRendererItemRenderable*)GuiRendererItemRenderable.New(
			//	RendererWorld.realRoot,
			//	//( parentGuiSceneObjectFor3DRendering != null ? parentGuiSceneObjectFor3DRendering.realObject : null ),
			//	renderableItem.vertexData.realObject,
			//	renderableItem.material.BaseMaterial.realObjectPtr,
			//	true parentGuiSceneObjectFor3DRendering == null
			//	);
			//}

			return renderableItem;
		}

		unsafe void DestroyRenderableItem( RenderableItem renderableItem )
		{
			//if( renderableItem.renderable != null )
			//{
			//	GuiRendererItemRenderable.Delete( renderableItem.renderable );
			//	renderableItem.renderable = null;
			//}

			if( renderableItem.vertexBuffer != null )
			{
				renderableItem.vertexBuffer.Dispose();
				renderableItem.vertexBuffer = null;
			}

			if( renderableItem.material != null )
			{
				//renderableItem.material.Dispose();
				renderableItem.material = null;
			}
		}

		public override void PushBlendingType( BlendingType blendingType )
		{
			blendingTypesStack.Push( blendingType );
		}

		public override void PopBlendingType()
		{
			if( blendingTypesStack.Count == 0 )
				Log.Fatal( "CanvasRenderer: PopBlendingType: blendingTypes.Count == 0." );
			blendingTypesStack.Pop();
		}

		public override void PushTextureFilteringMode( TextureFilteringMode mode )
		{
			textureFilteringModeStack.Push( mode );
		}

		public override void PopTextureFilteringMode()
		{
			if( textureFilteringModeStack.Count == 0 )
				Log.Fatal( "CanvasRenderer: PopTextureFilteringMode: textureFilteringModeStack.Count == 0." );
			textureFilteringModeStack.Pop();
		}

		public override void PushColorMultiplier( ColorValue color )
		{
			GetCurrentColorMultiplier( out var current );
			colorMultiplierStack.Push( current * color );
		}

		public override void PopColorMultiplier()
		{
			if( colorMultiplierStack.Count == 0 )
				Log.Fatal( "CanvasRenderer: PopTextureFilteringMode: colorMultiplierStack.Count == 0." );
			colorMultiplierStack.Pop();
		}

		public override void PushOcclusionDepthCheck( Vector2F screenPosition, float screenSize, float compareDepth )
		{
			occlusionDepthCheckStack.Push( new Vector4F( screenPosition.X, screenPosition.Y, screenSize, compareDepth ) );
		}

		public override void PopOcclusionDepthCheck()
		{
			if( occlusionDepthCheckStack.Count == 0 )
				Log.Fatal( "CanvasRenderer: PopOcclusionDepthCheck: occlusionDepthCheckStack.Count == 0." );
			occlusionDepthCheckStack.Pop();
		}

		void CheckForAllPushedParameters()
		{
			if( blendingTypesStack.Count != 0 )
				Log.Fatal( "CanvasRenderer: Not all blending type values are removed from stack." );
			if( shadersStack.Count != 0 )
				Log.Fatal( "CanvasRenderer: Not all custom shader mode values are removed from stack." );
			if( clipRectanglesStack.Count != 0 )
				Log.Fatal( "CanvasRenderer: Not all clip rectangle values are removed from stack." );
			if( textureFilteringModeStack.Count != 0 )
				Log.Fatal( "CanvasRenderer: Not all texture filtering mode values are removed from stack." );
			if( colorMultiplierStack.Count != 0 )
				Log.Fatal( "CanvasRenderer: Not all color multiplier values are removed from stack." );
			if( occlusionDepthCheckStack.Count != 0 )
				Log.Fatal( "CanvasRenderer: Not all occlusion depth check values are removed from stack." );
		}

		public override void PushClipRectangle( RectangleF clipRectangle )
		{
			GetCurrentClipRectangle( out var currentRectangle );

			RectangleF rectangle;

			if( !clipRectangle.IsCleared() )
			{
				var clipRectangle2 = clipRectangle;

				if( viewportForScreenCanvasRenderer != null )
				{
					clipRectangle2.Top *= viewportForScreenCanvasRenderer.SizeInPixels.Y;
					clipRectangle2.Top = (int)clipRectangle2.Top;
					clipRectangle2.Top /= viewportForScreenCanvasRenderer.SizeInPixels.Y;

					clipRectangle2.Bottom *= viewportForScreenCanvasRenderer.SizeInPixels.Y;
					clipRectangle2.Bottom = (int)clipRectangle2.Bottom;
					clipRectangle2.Bottom /= viewportForScreenCanvasRenderer.SizeInPixels.Y;

					clipRectangle2.Left *= viewportForScreenCanvasRenderer.SizeInPixels.X;
					clipRectangle2.Left = (int)clipRectangle2.Left;
					clipRectangle2.Left /= viewportForScreenCanvasRenderer.SizeInPixels.X;

					clipRectangle2.Right *= viewportForScreenCanvasRenderer.SizeInPixels.X;
					clipRectangle2.Right = (int)clipRectangle2.Right;
					clipRectangle2.Right /= viewportForScreenCanvasRenderer.SizeInPixels.X;
				}

				if( clipRectangle2.Right <= 0 )
				{
					clipRectangle2.Left = 0;
					clipRectangle2.Right = 0;
				}
				if( clipRectangle2.Left >= 1 )
				{
					clipRectangle2.Left = 1;
					clipRectangle2.Right = 1;
				}
				if( clipRectangle2.Bottom <= 0 )
				{
					clipRectangle2.Top = 0;
					clipRectangle2.Bottom = 0;
				}
				if( clipRectangle2.Top >= 1 )
				{
					clipRectangle2.Top = 1;
					clipRectangle2.Bottom = 1;
				}

				if( !currentRectangle.IsCleared() )
					rectangle = currentRectangle.Intersection( clipRectangle2 );
				else
					rectangle = clipRectangle2;
			}
			else
				rectangle = currentRectangle;

			//if( !clipRectangle.IsCleared() )
			//{
			//	if( !currentRectangle.IsCleared() )
			//		rectangle = currentRectangle.Intersection( clipRectangle );
			//	else
			//		rectangle = clipRectangle;
			//}
			//else
			//	rectangle = currentRectangle;

			clipRectanglesStack.Push( rectangle );
		}

		public override void PopClipRectangle()
		{
			if( clipRectanglesStack.Count == 0 )
				Log.Fatal( "CanvasRenderer: PopClipRectangle: clipRectanglesStack.Count == 0." );
			clipRectanglesStack.Pop();
		}

		void SetItemDynamicData( Item item )
		{
			//set blending
			foreach( RenderableItem renderableItem in item.renderableItems )
			{
				//!!!!!может все пассы. хот€ по дефолту всегда один

				GpuMaterialPass pass = renderableItem.material.MaterialPass;// Result.AllPasses[ 0 ];
				if( pass != null )
				{
					//blendingType
					switch( GetCurrentBlendingType() )
					{
					case BlendingType.AlphaBlend:
						pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha; //BGFX_STATE_BLEND_SRC_ALPHA
						pass.DestinationBlendFactor = SceneBlendFactor.OneMinusSourceAlpha; //BGFX_STATE_BLEND_INV_SRC_ALPHA

						//pass.ComposeOIT = false;
						break;

					case BlendingType.AlphaAdd:
						pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
						pass.DestinationBlendFactor = SceneBlendFactor.One;
						//pass.ComposeOIT = false;
						break;

					case BlendingType.Opaque:
						pass.SourceBlendFactor = SceneBlendFactor.One;
						pass.DestinationBlendFactor = SceneBlendFactor.Zero;
						//pass.ComposeOIT = false;
						break;

					case BlendingType.Add:
						pass.SourceBlendFactor = SceneBlendFactor.One;
						pass.DestinationBlendFactor = SceneBlendFactor.One;
						//pass.ComposeOIT = false;
						break;

						//case BlendingType.ComposeOIT:
						//	//pass.ComposeOIT = true;

						//	//pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
						//	//pass.DestinationBlendFactor = SceneBlendFactor.OneMinusSourceAlpha;

						//	pass.SourceBlendFactor = SceneBlendFactor.OneMinusSourceAlpha;
						//	pass.DestinationBlendFactor = SceneBlendFactor.SourceAlpha;

						//	//bgfx::setState( 0
						//	//	| BGFX_STATE_WRITE_RGB
						//	//	| BGFX_STATE_BLEND_FUNC( BGFX_STATE_BLEND_INV_SRC_ALPHA, BGFX_STATE_BLEND_SRC_ALPHA )
						//	//	);

						//	//pass.SourceBlendFactor = SceneBlendFactor.Zero;
						//	//pass.DestinationBlendFactor = SceneBlendFactor.OneMinusSourceAlpha;

						//	break;
					}
				}
			}

			GetCurrentColorMultiplier( out item.colorMultiplier );
			GetCurrentOcclusionDepthCheck( out item.occlusionDepthCheck );
		}

		void ClearTexturesForRenderableItem( RenderableItem renderableItem )
		{
			//!!!!?

			//LowLevelMaterial.Pass pass = renderableItem.material.BaseMaterial.AllPasses[ 0 ];
			//for( int n = 0; n < pass.VertexProgramTextures.Count; n++ )
			//	pass.VertexProgramTextures[ n ].texture = null;
			//for( int n = 0; n < pass.FragmentProgramTextures.Count; n++ )
			//	pass.FragmentProgramTextures[ n ].texture = null;
		}

		FreeRenderableItemGroup GetFreeRenderableItemGroup( ShaderItem shader, int bufferVertexCount )
		{
			//try to get already exists
			for( int n = 0; n < freeRenderableItems.Count; n++ )
			{
				FreeRenderableItemGroup group = freeRenderableItems[ n ];
				if( group.shader == shader &&
					group.bufferVertexCount == bufferVertexCount )
				//	group.sourceFileName == sourceFileName &&
				//	group.textureCount == textureCount )
				{
					return group;
				}
			}

			return null;
		}

		RenderableItem GetFreeRenderableItem( ShaderItem shader, int bufferVertexCount )
		{
			FreeRenderableItemGroup group = GetFreeRenderableItemGroup( shader, bufferVertexCount );

			if( group == null )
			{
				//create new group
				group = new FreeRenderableItemGroup();
				group.shader = shader;
				//group.sourceFileName = sourceFileName;
				//group.textureCount = textureCount;
				group.bufferVertexCount = bufferVertexCount;
				freeRenderableItems.Add( group );
			}

			RenderableItem renderableItem;

			if( group.list.Count != 0 && enableCache )
			{
				renderableItem = group.list[ group.list.Count - 1 ];
				group.list.RemoveAt( group.list.Count - 1 );
			}
			else
			{
				renderableItem = CreateRenderableItem( shader, bufferVertexCount );
			}

			return renderableItem;
		}

		void FreeRenderableItem( RenderableItem renderableItem )
		{
			//!!!!!всегда ведь не null?
			//!!!!!на выходе получилось null
			//!!!!!
			var m = renderableItem.material;
			if( m == null )
				return;
			var shader = m.Shader;
			//ShaderItem shader = ( (GuiRendererMaterial)renderableItem.material.ResultObject ).Shader;

			//string sourceFileName = renderableItem.material.SourceFileName;
			//int textureCount = renderableItem.material.TextureCount;

			int bufferVertexCount = renderableItem.vertexBuffer.VertexCount;

			ClearTexturesForRenderableItem( renderableItem );

			var group = GetFreeRenderableItemGroup( shader, bufferVertexCount );

			if( group != null && enableCache )
			{
				renderableItem.lastReleaseRenderTime = updateTime;
				group.list.Add( renderableItem );
			}
			else
			{
				DestroyRenderableItem( renderableItem );
			}
		}

		//!!!!!public?
		internal bool IsExistsOutItems()
		{
			return outItems.Count != 0;
		}

		public bool EnableCache
		{
			get { return enableCache; }
			set { enableCache = value; }
		}

		//!!!!
		//public bool IsTextureCurrentlyIsUse( Image texture )
		//{
		//	if( texture == null )
		//		return false;

		//	foreach( Item item in outItems )
		//	{
		//		{
		//			QuadItemKey itemKey = item.itemKey as QuadItemKey;
		//			if( itemKey != null )
		//			{
		//				if( itemKey.texture == texture )
		//					return true;
		//			}
		//		}

		//		{
		//			TextItemKey itemKey = item.itemKey as TextItemKey;
		//			if( itemKey != null )
		//			{
		//				if( itemKey.font != null && itemKey.font.IsContainsTexture( texture ) )
		//					return true;
		//			}
		//		}

		//		{
		//			TrianglesItemKey itemKey = item.itemKey as TrianglesItemKey;
		//			if( itemKey != null )
		//			{
		//				if( itemKey.texture == texture )
		//					return true;
		//			}
		//		}
		//	}

		//	return false;
		//}

		public override void GetCurrentClipRectangle( out RectangleF result )
		{
			if( clipRectanglesStack.Count != 0 )
				result = clipRectanglesStack.Peek();
			else
				result = RectangleF.Cleared;

			if( IsScreen && result.IsCleared() )
				result = new RectangleF( 0, 0, 1, 1 );
		}

		public override ShaderItem GetCurrentShader()
		{
			if( shadersStack.Count != 0 )
				return shadersStack.Peek();
			else
				return defaultShader;
		}

		public override TextureFilteringMode GetCurrentTextureFilteringMode()
		{
			if( textureFilteringModeStack.Count != 0 )
				return textureFilteringModeStack.Peek();
			else
				return TextureFilteringMode.Linear;
		}

		public override BlendingType GetCurrentBlendingType()
		{
			if( blendingTypesStack.Count != 0 )
				return blendingTypesStack.Peek();
			else
				return BlendingType.AlphaBlend;
		}

		public override void GetCurrentColorMultiplier( out ColorValue result )
		{
			if( colorMultiplierStack.Count != 0 )
				result = colorMultiplierStack.Peek();
			else
				result = new ColorValue( 1, 1, 1 );
		}

		public override void GetCurrentOcclusionDepthCheck( out Vector4F result )
		{
			if( occlusionDepthCheckStack.Count != 0 )
				result = occlusionDepthCheckStack.Peek();
			else
				result = Vector4F.Zero;
		}

		void SetRenderableItemDynamicData( RenderableItem renderableItem, RenderOperationType renderOperation, ImageComponent texture, bool clamp )
		{
			renderableItem.renderOperation = renderOperation;
			renderableItem.texture = texture;
			renderableItem.textureClamp = clamp;
			renderableItem.textureFiltering = GetCurrentTextureFilteringMode();
		}

		static Vector2F FixResultPosition( Vector2F pos )
		{
			//return pos;
			return new Vector2F( pos.X * 2 - 1, ( pos.Y * 2 - 1 ) * -1 );
		}

		static RectangleF FixResultPosition( RectangleF pos )
		{
			//return pos;
			return new RectangleF(
				pos.Minimum.X * 2 - 1, ( pos.Minimum.Y * 2 - 1 ) * -1,
				pos.Maximum.X * 2 - 1, ( pos.Maximum.Y * 2 - 1 ) * -1 );
		}

		public override void AddRoundedQuad( Rectangle rectangle, Vector2 roundingSize, AddRoundedQuadMode mode/*bool antialiasing, bool fading*/, ColorValue color )
		{
			var rect2 = rectangle.ToRectangleF();
			var roundSize = roundingSize.ToVector2F();

			if( mode == AddRoundedQuadMode.Antialiasing || mode == AddRoundedQuadMode.Fading )
			{
				ImageComponent texture;
				if( mode == AddRoundedQuadMode.Fading )
				{
					if( circleForRoundedFadingCached == null )
						circleForRoundedFadingCached = ResourceManager.LoadResource<ImageComponent>( @"Base\UI\Images\CircleForRoundedFading.png" );
					texture = circleForRoundedFadingCached;
				}
				else
				{
					if( circleForRoundedCached == null )
						circleForRoundedCached = ResourceManager.LoadResource<ImageComponent>( @"Base\UI\Images\CircleForRounded.png" );
					texture = circleForRoundedCached;
				}

				var vertices = new List<TriangleVertex>( 16 );
				var indices = new List<int>( 9 * 3 * 2 );

				vertices.Add( new TriangleVertex( rect2.LeftTop, color, new Vector2F( 0, 0 ) ) );
				vertices.Add( new TriangleVertex( rect2.LeftTop + new Vector2F( roundSize.X, 0 ), color, new Vector2F( 0.5f, 0 ) ) );
				vertices.Add( new TriangleVertex( rect2.RightTop + new Vector2F( -roundSize.X, 0 ), color, new Vector2F( 0.5f, 0 ) ) );
				vertices.Add( new TriangleVertex( rect2.RightTop, color, new Vector2F( 1, 0 ) ) );

				vertices.Add( new TriangleVertex( rect2.LeftTop + new Vector2F( 0, roundSize.Y ), color, new Vector2F( 0, 0.5f ) ) );
				vertices.Add( new TriangleVertex( rect2.LeftTop + new Vector2F( roundSize.X, roundSize.Y ), color, new Vector2F( 0.5f, 0.5f ) ) );
				vertices.Add( new TriangleVertex( rect2.RightTop + new Vector2F( -roundSize.X, roundSize.Y ), color, new Vector2F( 0.5f, 0.5f ) ) );
				vertices.Add( new TriangleVertex( rect2.RightTop + new Vector2F( 0, roundSize.Y ), color, new Vector2F( 1, 0.5f ) ) );

				vertices.Add( new TriangleVertex( rect2.LeftBottom + new Vector2F( 0, -roundSize.Y ), color, new Vector2F( 0, 0.5f ) ) );
				vertices.Add( new TriangleVertex( rect2.LeftBottom + new Vector2F( roundSize.X, -roundSize.Y ), color, new Vector2F( 0.5f, 0.5f ) ) );
				vertices.Add( new TriangleVertex( rect2.RightBottom + new Vector2F( -roundSize.X, -roundSize.Y ), color, new Vector2F( 0.5f, 0.5f ) ) );
				vertices.Add( new TriangleVertex( rect2.RightBottom + new Vector2F( 0, -roundSize.Y ), color, new Vector2F( 1, 0.5f ) ) );

				vertices.Add( new TriangleVertex( rect2.LeftBottom, color, new Vector2F( 0, 1 ) ) );
				vertices.Add( new TriangleVertex( rect2.LeftBottom + new Vector2F( roundSize.X, 0 ), color, new Vector2F( 0.5f, 1 ) ) );
				vertices.Add( new TriangleVertex( rect2.RightBottom + new Vector2F( -roundSize.X, 0 ), color, new Vector2F( 0.5f, 1 ) ) );
				vertices.Add( new TriangleVertex( rect2.RightBottom, color, new Vector2F( 1, 1 ) ) );

				indices.Add( 0 ); indices.Add( 1 ); indices.Add( 1 + 4 );
				indices.Add( 1 + 4 ); indices.Add( 0 + 4 ); indices.Add( 0 );

				indices.Add( 1 ); indices.Add( 2 ); indices.Add( 2 + 4 );
				indices.Add( 2 + 4 ); indices.Add( 1 + 4 ); indices.Add( 1 );

				indices.Add( 2 ); indices.Add( 3 ); indices.Add( 3 + 4 );
				indices.Add( 3 + 4 ); indices.Add( 2 + 4 ); indices.Add( 2 );

				indices.Add( 4 ); indices.Add( 5 ); indices.Add( 5 + 4 );
				indices.Add( 5 + 4 ); indices.Add( 4 + 4 ); indices.Add( 4 );

				indices.Add( 5 ); indices.Add( 6 ); indices.Add( 6 + 4 );
				indices.Add( 6 + 4 ); indices.Add( 5 + 4 ); indices.Add( 5 );

				indices.Add( 6 ); indices.Add( 7 ); indices.Add( 7 + 4 );
				indices.Add( 7 + 4 ); indices.Add( 6 + 4 ); indices.Add( 6 );

				indices.Add( 8 ); indices.Add( 9 ); indices.Add( 9 + 4 );
				indices.Add( 9 + 4 ); indices.Add( 8 + 4 ); indices.Add( 8 );

				indices.Add( 9 ); indices.Add( 10 ); indices.Add( 10 + 4 );
				indices.Add( 10 + 4 ); indices.Add( 9 + 4 ); indices.Add( 9 );

				indices.Add( 10 ); indices.Add( 11 ); indices.Add( 11 + 4 );
				indices.Add( 11 + 4 ); indices.Add( 10 + 4 ); indices.Add( 10 );

				AddTriangles( vertices, indices, texture, true );
			}
			else
			{
				int steps = 16;

				var list = new List<Vector2F>( steps * 4 );

				for( int n = 0; n < steps; n++ )
				{
					var v = (float)n / (float)( steps - 1 );
					var angle = v * MathEx.PI / 2;
					list.Add( rect2.LeftTop + new Vector2F( 1.0f - MathEx.Cos( angle ), 1.0f - MathEx.Sin( angle ) ) * roundSize );
				}

				for( int n = steps - 1; n >= 0; n-- )
				{
					var v = (float)n / (float)( steps - 1 );
					var angle = v * MathEx.PI / 2;
					list.Add( rect2.RightTop + new Vector2F( MathEx.Cos( angle ) - 1.0f, 1.0f - MathEx.Sin( angle ) ) * roundSize );
				}

				for( int n = 0; n < steps; n++ )
				{
					var v = (float)n / (float)( steps - 1 );
					var angle = v * MathEx.PI / 2;
					list.Add( rect2.RightBottom + new Vector2F( MathEx.Cos( angle ) - 1.0f, MathEx.Sin( angle ) - 1.0f ) * roundSize );
				}

				for( int n = steps - 1; n >= 0; n-- )
				{
					var v = (float)n / (float)( steps - 1 );
					var angle = v * MathEx.PI / 2;
					list.Add( rect2.LeftBottom + new Vector2F( 1.0f - MathEx.Cos( angle ), MathEx.Sin( angle ) - 1.0f ) * roundSize );
				}

				var vertices = new List<TriangleVertex>( 1 + list.Count );
				var indices = new List<int>( list.Count * 3 );

				vertices.Add( new TriangleVertex( rect2.GetCenter(), color ) );
				foreach( var v in list )
					vertices.Add( new TriangleVertex( v, color ) );

				for( int n = 0; n < list.Count; n++ )
				{
					indices.Add( 0 );
					indices.Add( 1 + n );
					indices.Add( 1 + ( n + 1 ) % list.Count );
				}

				AddTriangles( vertices, indices );
			}
		}

		//public override void AddRoundedQuad( Rectangle rectangle, Vector2 roundingSize, bool antialiasing, ColorValue color )
		//{
		//	var rect2 = rectangle.ToRectangleF();
		//	var roundSize = roundingSize.ToVector2F();

		//	if( antialiasing )
		//	{
		//		if( circleForRoundedCached == null )
		//			circleForRoundedCached = ResourceManager.LoadResource<ImageComponent>( @"Base\UI\Images\CircleForRounded.png" );
		//		var texture = circleForRoundedCached;

		//		var vertices = new List<TriangleVertex>( 16 );
		//		var indices = new List<int>( 9 * 3 * 2 );

		//		vertices.Add( new TriangleVertex( rect2.LeftTop, color, new Vector2F( 0, 0 ) ) );
		//		vertices.Add( new TriangleVertex( rect2.LeftTop + new Vector2F( roundSize.X, 0 ), color, new Vector2F( 0.5f, 0 ) ) );
		//		vertices.Add( new TriangleVertex( rect2.RightTop + new Vector2F( -roundSize.X, 0 ), color, new Vector2F( 0.5f, 0 ) ) );
		//		vertices.Add( new TriangleVertex( rect2.RightTop, color, new Vector2F( 1, 0 ) ) );

		//		vertices.Add( new TriangleVertex( rect2.LeftTop + new Vector2F( 0, roundSize.Y ), color, new Vector2F( 0, 0.5f ) ) );
		//		vertices.Add( new TriangleVertex( rect2.LeftTop + new Vector2F( roundSize.X, roundSize.Y ), color, new Vector2F( 0.5f, 0.5f ) ) );
		//		vertices.Add( new TriangleVertex( rect2.RightTop + new Vector2F( -roundSize.X, roundSize.Y ), color, new Vector2F( 0.5f, 0.5f ) ) );
		//		vertices.Add( new TriangleVertex( rect2.RightTop + new Vector2F( 0, roundSize.Y ), color, new Vector2F( 1, 0.5f ) ) );

		//		vertices.Add( new TriangleVertex( rect2.LeftBottom + new Vector2F( 0, -roundSize.Y ), color, new Vector2F( 0, 0.5f ) ) );
		//		vertices.Add( new TriangleVertex( rect2.LeftBottom + new Vector2F( roundSize.X, -roundSize.Y ), color, new Vector2F( 0.5f, 0.5f ) ) );
		//		vertices.Add( new TriangleVertex( rect2.RightBottom + new Vector2F( -roundSize.X, -roundSize.Y ), color, new Vector2F( 0.5f, 0.5f ) ) );
		//		vertices.Add( new TriangleVertex( rect2.RightBottom + new Vector2F( 0, -roundSize.Y ), color, new Vector2F( 1, 0.5f ) ) );

		//		vertices.Add( new TriangleVertex( rect2.LeftBottom, color, new Vector2F( 0, 1 ) ) );
		//		vertices.Add( new TriangleVertex( rect2.LeftBottom + new Vector2F( roundSize.X, 0 ), color, new Vector2F( 0.5f, 1 ) ) );
		//		vertices.Add( new TriangleVertex( rect2.RightBottom + new Vector2F( -roundSize.X, 0 ), color, new Vector2F( 0.5f, 1 ) ) );
		//		vertices.Add( new TriangleVertex( rect2.RightBottom, color, new Vector2F( 1, 1 ) ) );

		//		indices.Add( 0 ); indices.Add( 1 ); indices.Add( 1 + 4 );
		//		indices.Add( 1 + 4 ); indices.Add( 0 + 4 ); indices.Add( 0 );

		//		indices.Add( 1 ); indices.Add( 2 ); indices.Add( 2 + 4 );
		//		indices.Add( 2 + 4 ); indices.Add( 1 + 4 ); indices.Add( 1 );

		//		indices.Add( 2 ); indices.Add( 3 ); indices.Add( 3 + 4 );
		//		indices.Add( 3 + 4 ); indices.Add( 2 + 4 ); indices.Add( 2 );

		//		indices.Add( 4 ); indices.Add( 5 ); indices.Add( 5 + 4 );
		//		indices.Add( 5 + 4 ); indices.Add( 4 + 4 ); indices.Add( 4 );

		//		indices.Add( 5 ); indices.Add( 6 ); indices.Add( 6 + 4 );
		//		indices.Add( 6 + 4 ); indices.Add( 5 + 4 ); indices.Add( 5 );

		//		indices.Add( 6 ); indices.Add( 7 ); indices.Add( 7 + 4 );
		//		indices.Add( 7 + 4 ); indices.Add( 6 + 4 ); indices.Add( 6 );

		//		indices.Add( 8 ); indices.Add( 9 ); indices.Add( 9 + 4 );
		//		indices.Add( 9 + 4 ); indices.Add( 8 + 4 ); indices.Add( 8 );

		//		indices.Add( 9 ); indices.Add( 10 ); indices.Add( 10 + 4 );
		//		indices.Add( 10 + 4 ); indices.Add( 9 + 4 ); indices.Add( 9 );

		//		indices.Add( 10 ); indices.Add( 11 ); indices.Add( 11 + 4 );
		//		indices.Add( 11 + 4 ); indices.Add( 10 + 4 ); indices.Add( 10 );

		//		AddTriangles( vertices, indices, texture, true );
		//	}
		//	else
		//	{
		//		int steps = 16;

		//		var list = new List<Vector2F>( steps * 4 );

		//		for( int n = 0; n < steps; n++ )
		//		{
		//			var v = (float)n / (float)( steps - 1 );
		//			var angle = v * MathEx.PI / 2;
		//			list.Add( rect2.LeftTop + new Vector2F( 1.0f - MathEx.Cos( angle ), 1.0f - MathEx.Sin( angle ) ) * roundSize );
		//		}

		//		for( int n = steps - 1; n >= 0; n-- )
		//		{
		//			var v = (float)n / (float)( steps - 1 );
		//			var angle = v * MathEx.PI / 2;
		//			list.Add( rect2.RightTop + new Vector2F( MathEx.Cos( angle ) - 1.0f, 1.0f - MathEx.Sin( angle ) ) * roundSize );
		//		}

		//		for( int n = 0; n < steps; n++ )
		//		{
		//			var v = (float)n / (float)( steps - 1 );
		//			var angle = v * MathEx.PI / 2;
		//			list.Add( rect2.RightBottom + new Vector2F( MathEx.Cos( angle ) - 1.0f, MathEx.Sin( angle ) - 1.0f ) * roundSize );
		//		}

		//		for( int n = steps - 1; n >= 0; n-- )
		//		{
		//			var v = (float)n / (float)( steps - 1 );
		//			var angle = v * MathEx.PI / 2;
		//			list.Add( rect2.LeftBottom + new Vector2F( 1.0f - MathEx.Cos( angle ), MathEx.Sin( angle ) - 1.0f ) * roundSize );
		//		}

		//		var vertices = new List<TriangleVertex>( 1 + list.Count );
		//		var indices = new List<int>( list.Count * 3 );

		//		vertices.Add( new TriangleVertex( rect2.GetCenter(), color ) );
		//		foreach( var v in list )
		//			vertices.Add( new TriangleVertex( v, color ) );

		//		for( int n = 0; n < list.Count; n++ )
		//		{
		//			indices.Add( 0 );
		//			indices.Add( 1 + n );
		//			indices.Add( 1 + ( n + 1 ) % list.Count );
		//		}

		//		AddTriangles( vertices, indices );
		//	}
		//}

	}
}
