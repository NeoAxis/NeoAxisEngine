// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;
using Internal.SharpBgfx;
using System.Linq;

namespace NeoAxis
{
	//!!!!а они не шарятся? если несколько контекстов есть

	/// <summary>
	/// Represents a manager for rendering to viewport.
	/// </summary>
	public class ViewportRenderingContext
	{
		//static
		internal static ViewportRenderingContext current;

		//constants
		internal Viewport owner;
		CanvasRendererImpl renderer;

		//!!!!!какие-то параметры. они сохраняются при изменении размера виевпорта. например: адаптация глаза

		//!!!!можно еще расшаривать между другими контекстами. например, текстуры теней те же самые. ну и если размер виевпорта такой же.
		List<DynamicTextureItem> dynamicTexturesAllocated = new List<DynamicTextureItem>();
		ESet<DynamicTextureItem> dynamicTexturesFree = new ESet<DynamicTextureItem>();

		List<MultiRenderTarget> multiRenderTargets = new List<MultiRenderTarget>();
		//List<MultiRenderTargetItem> multiRenderTargets = new List<MultiRenderTargetItem>();
		//List<MultiRenderTargetItem> multiRenderTargetsAllocated = new List<MultiRenderTargetItem>();
		//ESet<MultiRenderTargetItem> multiRenderTargetsFree = new ESet<MultiRenderTargetItem>();

		List<OcclusionCullingBufferItem> occlusionCullingBuffersAllocated = new List<OcclusionCullingBufferItem>();
		ESet<OcclusionCullingBufferItem> occlusionCullingBuffersFree = new ESet<OcclusionCullingBufferItem>();

		//update each frame
		//!!!!public
		internal object uniquePerFrameObjectToDetectNewFrame;
		//!!!!в FrameData перенести может
		public ObjectsDuringUpdateClass ObjectsDuringUpdate;
		internal StatisticsClass updateStatisticsCurrent = new StatisticsClass();
		internal StatisticsClass updateStatisticsPrevious = new StatisticsClass();
		internal RenderingPipeline renderingPipeline;
		public RenderingPipeline.IFrameData FrameData;
		public ObjectInSpace.RenderingContext ObjectInSpaceRenderingContext;

		//update many times during frame rendering
		Viewport currentViewport;
		internal int currentViewNumber = -1;

		//custom cached data
		public Dictionary<string, object> AnyData = new Dictionary<string, object>();
		public Dictionary<string, ImageComponent> AnyImageAutoDispose = new Dictionary<string, ImageComponent>();

		internal RenderingPipeline.RenderSceneData.CutVolumeItem[] CurrentCutVolumes;

		internal Vector2I SizeInPixelsLowResolutionBeforeUpscale;

		//optimization
		internal Viewport.CameraSettingsClass OwnerCameraSettings;
		internal Vector3 OwnerCameraSettingsPosition;
		internal double ShadowObjectVisibilityDistanceFactor = 1;
		Curve1F getVisibilityDistanceByObjectSize;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum DynamicTextureType
		{
			RenderTarget,
			DynamicTexture,
			ComputeWrite,
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		class DynamicTextureItem
		{
			//creation data
			public DynamicTextureType type;
			public ImageComponent.TypeEnum imageType;
			public Vector2I size;
			public PixelFormat format;
			public int fsaaLevel;
			public bool mipmaps;
			public int arrayLayers;
			public bool createSimple3DRenderer;
			public bool createCanvasRenderer;

			public ImageComponent image;

			public bool usedLastUpdate;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		class OcclusionCullingBufferItem
		{
			public OcclusionCullingBuffer buffer;
			public bool usedLastUpdate;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The data provided during the context update process.
		/// </summary>
		public class ObjectsDuringUpdateClass
		{
			public Dictionary<string, ImageComponent> namedTextures = new Dictionary<string, ImageComponent>();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The data with the statistics of context's working.
		/// </summary>
		public class StatisticsClass
		{
			public double FPS;
			internal double[] previousTimeStep = new double[ 30 ];
			internal int updateFPSCounter;

			public int Triangles;
			public int Lines;
			public int DrawCalls;
			public int RenderTargets;
			public int DynamicTextures;
			public int ComputeWriteImages;
			public int Lights;
			public int ReflectionProbes;
			public int OcclusionCullingBuffers;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public class InputTextureItem
		//{
		//	GpuTexture texture;
		//	FilterOption minFilter;
		//	FilterOption magFilter;
		//	FilterOption mipFilter;


		//	public GpuTexture Texture
		//	{
		//		get { return texture; }
		//		set { texture = value; }
		//	}

		//	public FilterOption MinFilter
		//	{
		//		get { return minFilter; }
		//		set { minFilter = value; }
		//	}

		//	public FilterOption MagFilter
		//	{
		//		get { return magFilter; }
		//		set { magFilter = value; }
		//	}

		//	public FilterOption MipFilter
		//	{
		//		get { return mipFilter; }
		//		set { mipFilter = value; }
		//	}

		//	public InputTextureItem()
		//	{
		//	}

		//	public InputTextureItem( GpuTexture texture, FilterOption minFilter, FilterOption magFilter, FilterOption mipFilter )
		//	{
		//		this.texture = texture;
		//		this.minFilter = minFilter;
		//		this.magFilter = magFilter;
		//		this.mipFilter = mipFilter;
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a texture binding settings.
		/// </summary>
		public struct BindTextureData
		{
			public int TextureUnit;

			//!!!!какие-то параметры могут быть тоже составными
			public ImageComponent Texture;
			public TextureAddressingMode AddressingMode;
			public FilterOption FilteringMin;
			public FilterOption FilteringMag;
			public FilterOption FilteringMip;
			//public ColorValue BorderColor;
			public TextureFlags AdditionalFlags;

			//!!!!
			//int numMipmaps;
			//bool isAlpha;
			//virtual void _setTextureLayerAnisotropy( size_t unit, unsigned int maxAnisotropy ) = 0;
			//virtual void _setTextureMipmapBias( size_t unit, float bias ) = 0;
			//public ContentTypes contentType;
			//bool fetch4;

			//public TextureParameterValue()
			//{
			//}

			//public TextureParameterValue( Image texture, TextureAddressingMode addressingMode, FilterOption filteringMin, FilterOption filteringMag, FilterOption filteringMip, ColorValue borderColor )
			//{
			//	this.texture = texture;
			//	this.addressingMode = addressingMode;
			//	this.filteringMin = filteringMin;
			//	this.filteringMag = filteringMag;
			//	this.filteringMip = filteringMip;
			//	this.borderColor = borderColor;
			//}

			public BindTextureData( int textureUnit, ImageComponent texture, TextureAddressingMode addressingMode, FilterOption filteringMin, FilterOption filteringMag, FilterOption filteringMip, TextureFlags additionFlags = 0 )
			{
				this.TextureUnit = textureUnit;

				this.Texture = texture;
				this.AddressingMode = addressingMode;
				this.FilteringMin = filteringMin;
				this.FilteringMag = filteringMag;
				this.FilteringMip = filteringMip;

				//this.BorderColor = new ColorValue( 0, 0, 0, 0 );
				this.AdditionalFlags = additionFlags;
			}

			//public Texture Texture
			//{
			//	get { return texture; }
			//}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public ViewportRenderingContext( Viewport owner )
		{
			this.owner = owner;

			renderer = new CanvasRendererImpl( owner );
			//!!!!!resize? или еще что-то?
		}

		public static ViewportRenderingContext Current
		{
			get { return current; }
		}

		/// <summary>
		/// Occurs before object is disposed.
		/// </summary>
		public event Action<ViewportRenderingContext> Disposing;

		public virtual void Dispose()
		{
			Disposing?.Invoke( this );

			MultiRenderTarget_DestroyAll();
			DynamicTexture_DestroyAll();
			OcclusionCullingBuffer_DestroyAll();

			foreach( var image in AnyImageAutoDispose.Values )
				image.Dispose();
			AnyImageAutoDispose.Clear();

			if( renderer != null )
			{
				renderer.Dispose();
				renderer = null;
			}
		}

		public Viewport Owner
		{
			get { return owner; }
		}

		/////////////////////////////////////////

		ImageComponent DynamicTexture_GetFree( DynamicTextureType type, ImageComponent.TypeEnum imageType, Vector2I size, PixelFormat format, int fsaaLevel, bool mipmaps, int arrayLayers, bool createSimple3DRenderer, bool createCanvasRenderer )
		{
			//var item = renderTargetsFree.Find( i => i.type == type && i.size == size && i.format == format && i.fsaaLevel == fsaaLevel );
			DynamicTextureItem item = null;
			foreach( var i in dynamicTexturesFree )
			{
				if( i.type == type && i.imageType == imageType && i.size == size && i.format == format && i.fsaaLevel == fsaaLevel && i.mipmaps == mipmaps && i.arrayLayers == arrayLayers && i.createSimple3DRenderer == createSimple3DRenderer && i.createCanvasRenderer == createCanvasRenderer )
				{
					item = i;
					break;
				}
			}

			if( item != null )
			{
				dynamicTexturesFree.Remove( item );
				item.usedLastUpdate = true;
				return item.image;
			}
			return null;
		}

		public ImageComponent DynamicTexture_Alloc( DynamicTextureType type, ImageComponent.TypeEnum imageType, Vector2I size, PixelFormat format, int fsaaLevel, bool mipmaps, int arrayLayers = 1, bool createSimple3DRenderer = false, bool createCanvasRenderer = false )
		{
			if( type == DynamicTextureType.RenderTarget )
				UpdateStatisticsCurrent.RenderTargets++;
			else if( type == DynamicTextureType.DynamicTexture )
				UpdateStatisticsCurrent.DynamicTextures++;
			else
				UpdateStatisticsCurrent.ComputeWriteImages++;

			//find free
			{
				var target = DynamicTexture_GetFree( type, imageType, size, format, fsaaLevel, mipmaps, arrayLayers, createSimple3DRenderer, createCanvasRenderer );
				if( target != null )
					return target;
			}

			//!!!!need hierarchy controller?
			ImageComponent texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
			texture.CreateType = imageType;
			texture.CreateSize = size;
			texture.CreateMipmaps = mipmaps;
			texture.CreateArrayLayers = arrayLayers;
			texture.CreateFormat = format;
			if( type == DynamicTextureType.DynamicTexture )
				texture.CreateUsage = ImageComponent.Usages.Dynamic | ImageComponent.Usages.WriteOnly;
			else if( type == DynamicTextureType.ComputeWrite )
				texture.CreateUsage = ImageComponent.Usages.ComputeWrite | ImageComponent.Usages.WriteOnly;
			else
				texture.CreateUsage = ImageComponent.Usages.RenderTarget;
			texture.CreateFSAA = fsaaLevel;
			texture.Enabled = true;

			//!!!!
			if( type == DynamicTextureType.ComputeWrite )
				texture.Result.PrepareNativeObject();

			//!!!!!как проверять ошибки создания текстур? везде так
			//if( texture == null )
			//{
			//	//!!!!!
			//	Log.Fatal( "ViewportRenderingPipeline: RenderTarget_Alloc: Unable to create texture." );
			//	return null;
			//}

			int faces = imageType == ImageComponent.TypeEnum.Cube ? 6 : arrayLayers;

			int numMips;
			if( mipmaps )
			{
				float kInvLogNat2 = 1.4426950408889634073599246810019f;
				numMips = 1 + (int)( Math.Log( size.MaxComponent() ) * kInvLogNat2 );
			}
			else
				numMips = 1;

			if( type == DynamicTextureType.RenderTarget )
			{
				for( int face = 0; face < faces; face++ )
				{
					for( int mip = 0; mip < numMips; mip++ )
					{
						RenderTexture renderTexture = texture.Result.GetRenderTarget( mip, face );
						var viewport = renderTexture.AddViewport( createSimple3DRenderer, createCanvasRenderer );

						viewport.RenderingPipelineCreate();
						viewport.RenderingPipelineCreated.UseRenderTargets = false;
					}
					//!!!!что-то еще?
				}
			}

			//Log.Info( "alloc: " + type.ToString() + " " + size.ToString() );

			//add to list of allocated
			var item = new DynamicTextureItem();
			item.type = type;
			item.imageType = imageType;
			item.size = size;
			item.format = format;
			item.fsaaLevel = fsaaLevel;
			item.mipmaps = mipmaps;
			//item.num_mipmaps = num_mipmaps;
			item.arrayLayers = arrayLayers;
			item.createSimple3DRenderer = createSimple3DRenderer;
			item.createCanvasRenderer = createCanvasRenderer;
			item.image = texture;
			dynamicTexturesAllocated.Add( item );

			item.usedLastUpdate = true;
			return texture;
		}

		public ImageComponent RenderTarget2D_Alloc( Vector2I size, PixelFormat format, int fsaaLevel = 0, bool mipmaps = false, int arrayLayers = 1, bool createSimple3DRenderer = false, bool createCanvasRenderer = false )
		{
			return DynamicTexture_Alloc( DynamicTextureType.RenderTarget, ImageComponent.TypeEnum._2D, size, format, fsaaLevel, mipmaps, arrayLayers, createSimple3DRenderer, createCanvasRenderer );
		}

		//!!!!check arrayLayers
		public ImageComponent RenderTargetCube_Alloc( Vector2I size, PixelFormat format, int fsaaLevel = 0, bool mipmaps = false, int arrayLayers = 1, bool createSimple3DRenderer = false, bool createCanvasRenderer = false )
		{
			return DynamicTexture_Alloc( DynamicTextureType.RenderTarget, ImageComponent.TypeEnum.Cube, size, format, fsaaLevel, mipmaps, arrayLayers, createSimple3DRenderer, createCanvasRenderer );
		}

		public void DynamicTexture_Free( ImageComponent texture )
		{
			var item = dynamicTexturesAllocated.Find( i => i.image == texture );
			//RenderTargetItem item = null;
			//foreach( var i in renderTargetsAllocated )
			//{
			//	if( i.target == renderTarget )
			//	{
			//		item = i;
			//		break;
			//	}
			//}

			////can be created for MRT. such render targets can't free.
			//if( item == null )
			//	return;
			if( item == null )
				Log.Fatal( "ViewportRenderingContext: DynamicTexture_Free: Dynamic texture is not allocated." );
			if( dynamicTexturesFree.Contains( item ) )
				Log.Fatal( "ViewportRenderingContext: DynamicTexture_Free: Dynamic texture is already free." );

			dynamicTexturesFree.Add( item );
		}

		public void DynamicTexture_FreeAllEndUpdate()
		{
			//free all
			foreach( var item in dynamicTexturesAllocated )
			{
				if( !dynamicTexturesFree.Contains( item ) )
					DynamicTexture_Free( item.image );
			}

			//!!!!как вариант можно удалять только когда создается новый, которого не оказалось в списке свободных.
			//!!!!!!т.е. тогда получится, что буду висеть старые, реиспользоваться.

			//destroy not used at this frame
			List<DynamicTextureItem> copy = new List<DynamicTextureItem>( dynamicTexturesAllocated );
			foreach( var item in copy )
			{
				if( !item.usedLastUpdate )
					DynamicTexture_Destroy( item );
			}

			//clear usedLastUpdate
			foreach( var item in dynamicTexturesAllocated )
				item.usedLastUpdate = false;

			////bug fix for Intel
			////mark target is not cleared
			//foreach( var item in dynamicTexturesAllocated )
			//{
			//	if( item.image.Result != null )
			//		item.image.Result._renderTargetCleared = false;
			//}
		}

		void DynamicTexture_Destroy( DynamicTextureItem item )
		{
			dynamicTexturesFree.Remove( item );

			item.image.Dispose();
			dynamicTexturesAllocated.Remove( item );
		}

		public void DynamicTexture_DestroyAll()
		{
			List<DynamicTextureItem> copy = new List<DynamicTextureItem>( dynamicTexturesAllocated );
			foreach( var item in copy )
				DynamicTexture_Destroy( item );
		}

		/////////////////////////////////////////

		public MultiRenderTarget MultiRenderTarget_Create( MultiRenderTarget.Item[] items )
		{
			var mrt = RenderingSystem.CreateMultiRenderTarget( items );
			mrt.AddViewport( false, false );
			//var item = new MultiRenderTargetItem( targets, mrt );
			multiRenderTargets.Add( mrt );
			return mrt;
		}

		public void MultiRenderTarget_DestroyAll()
		{
			foreach( var mrt in multiRenderTargets )
				mrt.Dispose();
			multiRenderTargets.Clear();
		}

		//MultiRenderTargetItem MultiRenderTarget_GetFree( Vec2I size, PixelFormat[] formats )
		//{
		//	MultiRenderTargetItem item = null;
		//	foreach( var i in multiRenderTargetsFree )
		//	{
		//		if( i.size == size && i.formats.SequenceEqual( formats ) )
		//		{
		//			item = i;
		//			break;
		//		}
		//	}

		//	if( item != null )
		//	{
		//		multiRenderTargetsFree.Remove( item );
		//		item.usedLastUpdate = true;
		//		return item;
		//	}
		//	return null;
		//}

		//public MultiRenderTargetItem MultiRenderTarget_Alloc( Vec2I size, PixelFormat[] formats )
		//{
		//	//find free
		//	{
		//		var item2 = MultiRenderTarget_GetFree( size, formats );
		//		if( item2 != null )
		//			return item2;
		//	}

		//	//!!!!показывать в профайлере сколько создается и удаляется за кадр. еще что-то показывать?

		//	var item = new MultiRenderTargetItem();
		//	item.size = size;
		//	item.formats = formats;
		//	item.targets = new Texture[ formats.Length ];

		//	for( int n = 0; n < item.targets.Length; n++ )
		//	{
		//		//!!!!need hierarchy controller?
		//		Texture texture = ComponentUtils.CreateComponent<Texture>( null, true, false );
		//		texture.CreateType = Texture.TypeEnum._2D;
		//		texture.CreateSize = size;
		//		texture.CreateMipmaps = false;//0;
		//		texture.CreateFormat = formats[ n ];
		//		texture.CreateUsage = Texture.Usages.RenderTarget;
		//		texture.CreateFSAA = 0;// fsaaLevel;
		//		texture.Enabled = true;

		//		//!!!!!как проверять ошибки создания текстур? везде так
		//		//if( texture == null )
		//		//{
		//		//	//!!!!!
		//		//	Log.Fatal( "ViewportRenderingPipeline: RenderTarget_Alloc: Unable to create texture." );
		//		//	return null;
		//		//}

		//		RenderTexture renderTexture = texture.Result.GetRenderTarget();
		//		renderTexture.AddViewport( false, false );

		//		item.targets[ n ] = texture;
		//	}

		//	multiRenderTargetsAllocated.Add( item );
		//	item.usedLastUpdate = true;
		//	return item;
		//}

		//public void MultiRenderTarget_Free( MultiRenderTargetItem mrt )
		//{
		//	var item = multiRenderTargetsAllocated.Find( i => i == mrt );

		//	if( item == null )
		//		Log.Fatal( "ViewportRenderingContext: MultiRenderTarget_Free: Render target is not allocated." );
		//	if( multiRenderTargetsFree.Contains( item ) )
		//		Log.Fatal( "ViewportRenderingContext: MultiRenderTarget_Free: Render target is already free." );
		//	multiRenderTargetsFree.Add( item );
		//}

		//public void MultiRenderTarget_FreeAllEndUpdate()
		//{
		//	//free all
		//	foreach( var item in multiRenderTargetsAllocated )
		//	{
		//		if( !multiRenderTargetsFree.Contains( item ) )
		//			MultiRenderTarget_Free( item );
		//	}

		//	//!!!!как вариант можно удалять только когда создается новый, которого не оказалось в списке свободных.
		//	//!!!!!!т.е. тогда получится, что буду висеть старые, реиспользоваться.

		//	//destroy not used at this frame
		//	List<MultiRenderTargetItem> copy = new List<MultiRenderTargetItem>( multiRenderTargetsAllocated );
		//	foreach( var item in copy )
		//	{
		//		if( !item.usedLastUpdate )
		//			MultiRenderTarget_Destroy( item );
		//	}

		//	//clear usedLastUpdate
		//	foreach( var item in multiRenderTargetsAllocated )
		//		item.usedLastUpdate = false;
		//}

		//void MultiRenderTarget_Destroy( MultiRenderTargetItem item )
		//{
		//	multiRenderTargetsFree.Remove( item );

		//	foreach( var target in item.targets )
		//		target.Dispose();

		//	multiRenderTargetsAllocated.Remove( item );
		//}

		//public void MultiRenderTarget_DestroyAll()
		//{
		//	List<MultiRenderTargetItem> copy = new List<MultiRenderTargetItem>( multiRenderTargetsAllocated );
		//	foreach( var item in copy )
		//		MultiRenderTarget_Destroy( item );
		//}

		/////////////////////////////////////////

		OcclusionCullingBuffer OcclusionCullingBuffer_GetFree( Vector2I size )
		{
			OcclusionCullingBufferItem item = null;
			foreach( var i in occlusionCullingBuffersFree )
			{
				if( i.buffer.Size == size )
				{
					item = i;
					break;
				}
			}

			if( item != null )
			{
				occlusionCullingBuffersFree.Remove( item );
				item.usedLastUpdate = true;
				return item.buffer;
			}
			return null;
		}

		public OcclusionCullingBuffer OcclusionCullingBuffer_Alloc( Vector2I size )
		{
			UpdateStatisticsCurrent.OcclusionCullingBuffers++;

			//find free
			{
				var buffer2 = OcclusionCullingBuffer_GetFree( size );
				if( buffer2 != null )
					return buffer2;
			}

			var buffer = OcclusionCullingBuffer.Create();
			buffer.SetResolution( size );

			//add to list of allocated
			var item = new OcclusionCullingBufferItem();
			item.buffer = buffer;
			occlusionCullingBuffersAllocated.Add( item );

			item.usedLastUpdate = true;
			return buffer;
		}

		public void OcclusionCullingBuffer_Free( OcclusionCullingBuffer buffer )
		{
			var item = occlusionCullingBuffersAllocated.Find( i => i.buffer == buffer );

			if( item == null )
				Log.Fatal( "ViewportRenderingContext: OcclusionCullingBuffer_Free: The buffer is not allocated." );
			if( occlusionCullingBuffersFree.Contains( item ) )
				Log.Fatal( "ViewportRenderingContext: OcclusionCullingBuffer_Free: The buffer is already free." );

			occlusionCullingBuffersFree.Add( item );
		}

		public void OcclusionCullingBuffer_FreeAllEndUpdate()
		{
			//free all
			foreach( var item in occlusionCullingBuffersAllocated )
			{
				if( !occlusionCullingBuffersFree.Contains( item ) )
					OcclusionCullingBuffer_Free( item.buffer );
			}

			//destroy not used at this frame
			var copy = new List<OcclusionCullingBufferItem>( occlusionCullingBuffersAllocated );
			foreach( var item in copy )
			{
				if( !item.usedLastUpdate )
					OcclusionCullingBuffer_Destroy( item );
			}

			//clear usedLastUpdate
			foreach( var item in occlusionCullingBuffersAllocated )
				item.usedLastUpdate = false;
		}

		void OcclusionCullingBuffer_Destroy( OcclusionCullingBufferItem item )
		{
			occlusionCullingBuffersFree.Remove( item );

			item.buffer.Dispose();
			occlusionCullingBuffersAllocated.Remove( item );
		}

		public void OcclusionCullingBuffer_DestroyAll()
		{
			var copy = new List<OcclusionCullingBufferItem>( occlusionCullingBuffersAllocated );
			foreach( var item in copy )
				OcclusionCullingBuffer_Destroy( item );
		}

		/////////////////////////////////////////

		public Viewport CurrentViewport
		{
			get { return currentViewport; }
			//set
			//{
			//	currentViewport = value;

			//	currentViewNumber++;

			//	Bgfx.ResetView( CurrentViewNumber );

			//	bool skip = false;
			//	var renderWindow = currentViewport.parent as RenderWindow;
			//	if( renderWindow != null && renderWindow.ThisIsApplicationWindow )
			//		skip = true;
			//	if( !skip )
			//		Bgfx.SetViewFrameBuffer( CurrentViewNumber, currentViewport.parent.FrameBuffer );

			//	Bgfx.SetViewMode( CurrentViewNumber, ViewMode.Sequential );
			//	Bgfx.SetViewRect( CurrentViewNumber, 0, 0, CurrentViewport.SizeInPixels.X, CurrentViewport.SizeInPixels.Y );

			//	//!!!!new
			//	Mat4F identity = Mat4F.Identity;
			//	unsafe
			//	{
			//		Bgfx.SetViewTransform( CurrentViewNumber, (float*)&identity, (float*)&identity );
			//	}
			//}
		}

		public ushort CurrentViewNumber
		{
			get { return (ushort)currentViewNumber; }
		}

		public void ResetViews()
		{
			//!!!!
			for( int n = 0; n < currentViewNumber; n++ )
				Bgfx.ResetView( (ushort)n );
			currentViewNumber = -1;
		}

		static uint ToRGBA( ColorValue color )
		{
			var alpha = (uint)MathEx.Clamp( color.Alpha * 255, 0, 255 );
			var red = (uint)MathEx.Clamp( color.Red * 255, 0, 255 );
			var green = (uint)MathEx.Clamp( color.Green * 255, 0, 255 );
			var blue = (uint)MathEx.Clamp( color.Blue * 255, 0, 255 );
			return ( red << 24 | green << 16 | blue << 8 | alpha );
		}

		[StructLayout( LayoutKind.Sequential )]
		struct ViewportSettingsUniform
		{
			public Vector2F size;
			public Vector2F sizeInv;
		}

		public void SetViewport( Viewport viewport, Matrix4F viewMatrix, Matrix4F projectionMatrix, FrameBufferTypes clearBuffers, ColorValue clearBackgroundColor, float clearDepthValue = 1, byte clearStencilValue = 0 )
		{
			currentViewport = viewport;
			currentViewNumber++;

			//init bgfx view

			Bgfx.ResetView( CurrentViewNumber );

			bool skip = false;
			var renderWindow = currentViewport.parent as RenderWindow;
			if( renderWindow != null && renderWindow.ThisIsApplicationWindow )
				skip = true;
			if( !skip )
				Bgfx.SetViewFrameBuffer( CurrentViewNumber, currentViewport.parent.FrameBuffer );

			Bgfx.SetViewMode( CurrentViewNumber, ViewMode.Sequential );
			Bgfx.SetViewRect( CurrentViewNumber, 0, 0, CurrentViewport.SizeInPixels.X, CurrentViewport.SizeInPixels.Y );
			unsafe
			{
				Bgfx.SetViewTransform( CurrentViewNumber, (float*)&viewMatrix, (float*)&projectionMatrix );
			}

			////!!!!
			//if( clearBuffers == 0 )
			//{
			//	var target = viewport.Parent as RenderTexture;
			//	if( target != null )
			//	{
			//		if( !target.Creator._renderTargetCleared )
			//		{
			//			clearBuffers = FrameBufferTypes.Color;
			//			//clearBuffers = FrameBufferTypes.All;
			//		}
			//	}
			//}

			//clear
			if( clearBuffers != 0 )
			{
				ClearTargets targets = 0;
				if( ( clearBuffers & FrameBufferTypes.Color ) != 0 )
					targets |= ClearTargets.Color;
				if( ( clearBuffers & FrameBufferTypes.Depth ) != 0 )
					targets |= ClearTargets.Depth;
				if( ( clearBuffers & FrameBufferTypes.Stencil ) != 0 )
					targets |= ClearTargets.Stencil;
				Bgfx.SetViewClear( CurrentViewNumber, targets, ToRGBA( clearBackgroundColor ), clearDepthValue, clearStencilValue );

				Bgfx.Touch( CurrentViewNumber );
			}

			////!!!!
			//if( clearBuffers != 0 )//if( clearBuffers == FrameBufferTypes.All )
			//{
			//	var target = viewport.Parent as RenderTexture;
			//	if( target != null )
			//		target.Creator._renderTargetCleared = true;
			//}

			//u_viewportSettings
			unsafe
			{
				var data = new ViewportSettingsUniform();
				data.size = viewport.SizeInPixels.ToVector2F();
				data.sizeInv = new Vector2F( 1, 1 ) / data.size;

				int vec4Count = sizeof( ViewportSettingsUniform ) / sizeof( Vector4F );
				if( vec4Count != 1 )
					Log.Fatal( "RenderingPipeline: Render: vec4Count != 1." );
				SetUniform( "u_viewportSettings", ParameterType.Vector4, vec4Count, &data );
			}
		}

		public void SetViewport( Viewport viewport, Matrix4F viewMatrix, Matrix4F projectionMatrix )
		{
			SetViewport( viewport, viewMatrix, projectionMatrix, 0, ColorValue.Zero );
		}

		public void SetViewport( Viewport viewport )
		{
			SetViewport( viewport, Matrix4F.Identity, Matrix4F.Identity );
		}

		public void SetComputeView()
		{
			currentViewport = null;
			currentViewNumber++;

			Bgfx.ResetView( CurrentViewNumber );
		}

		//public void ClearCurrentViewport( FrameBufferTypes buffers, ColorValue backgroundColor, float depth = 1, uint stencil = 0 )
		//{
		//	if( currentViewport == null )
		//		Log.Fatal( "ViewportRenderingContext: ClearCurrentViewport: CurrentViewport == null." );

		//	//!!!!mrt уметь чистить. хотя не обязательно

		//	ClearTargets targets = 0;
		//	if( (buffers & FrameBufferTypes.Color) != 0 )
		//		targets |= ClearTargets.Color;
		//	if( (buffers & FrameBufferTypes.Depth) != 0 )
		//		targets |= ClearTargets.Depth;
		//	if( (buffers & FrameBufferTypes.Stencil) != 0 )
		//		targets |= ClearTargets.Stencil;
		//	Bgfx.SetViewClear( CurrentViewNumber, targets, ToRGBA( backgroundColor ), xx, xx );

		//	Bgfx.Touch( CurrentViewNumber );
		//}

		//!!!!!как тут это всё с Viewport и multirendertarget?
		//!!!!!для RenderQuad добавить "RectI? scissorRectangle"?
		public void RenderQuadToCurrentViewport( CanvasRenderer.ShaderItem shader, CanvasRenderer.BlendingType blending = CanvasRenderer.BlendingType.Opaque, bool flipY = false )
		{
			if( currentViewport == null )
				Log.Fatal( "ViewportRenderingContext: RenderQuadToCurrentViewport: CurrentViewport == null." );

			renderer.PushShader( shader );
			renderer.PushBlendingType( blending );

			if( flipY )
				renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new Rectangle( 0, 1, 1, 0 ), null, new ColorValue( 1, 1, 1 ), false );
			else
				renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new Rectangle( 0, 0, 1, 1 ), null, new ColorValue( 1, 1, 1 ), false );

			renderer.PopBlendingType();
			renderer.PopShader();

			renderer.ViewportRendering_RenderToCurrentViewport( this, true, Owner.LastUpdateTime );
		}

		public void RenderTrianglesToCurrentViewport( CanvasRenderer.ShaderItem shader, IList<CanvasRenderer.TriangleVertex> vertices, CanvasRenderer.BlendingType blending = CanvasRenderer.BlendingType.Opaque )
		{
			if( currentViewport == null )
				Log.Fatal( "ViewportRenderingContext: RenderTrianglesToCurrentViewport: CurrentViewport == null." );

			renderer.PushShader( shader );
			renderer.PushBlendingType( blending );// GuiRenderer.BlendingType.Opaque );
			renderer.AddTriangles( vertices, null, false );
			renderer.PopBlendingType();
			renderer.PopShader();

			renderer.ViewportRendering_RenderToCurrentViewport( this, true, Owner.LastUpdateTime );
		}

		public void SetVertexBuffer( int stream, GpuVertexBuffer buffer, int startVertex = 0, int count = -1 )
		{
			if( buffer.Flags.HasFlag( GpuBufferFlags.Dynamic ) || buffer.Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
				Bgfx.SetVertexBuffer( stream, (DynamicVertexBuffer)buffer.GetNativeObject(), startVertex, count );
			else
				Bgfx.SetVertexBuffer( stream, (VertexBuffer)buffer.GetNativeObject(), startVertex, count );
		}

		public void SetInstanceDataBuffer( GpuVertexBuffer buffer, int startVertex /*= 0*/, int count /*= -1*/ )
		{
			if( buffer.Flags.HasFlag( GpuBufferFlags.Dynamic ) || buffer.Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
				Bgfx.SetInstanceDataBuffer( (DynamicVertexBuffer)buffer.GetNativeObject(), startVertex, count );
			else
				Bgfx.SetInstanceDataBuffer( (VertexBuffer)buffer.GetNativeObject(), startVertex, count );
		}

		public void SetIndexBuffer( GpuIndexBuffer buffer, int startIndex = 0, int count = -1 )
		{
			if( buffer.Flags.HasFlag( GpuBufferFlags.Dynamic ) || buffer.Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
				Bgfx.SetIndexBuffer( (DynamicIndexBuffer)buffer.GetNativeObject(), startIndex, count );
			else
				Bgfx.SetIndexBuffer( (IndexBuffer)buffer.GetNativeObject(), startIndex, count );
		}

		public static UniformType GetUniformTypeByParameterType( ParameterType type )
		{
			//!!!!может какие-то как массив интов?

			switch( type )
			{
			case ParameterType.Boolean:
			case ParameterType.Byte:
			case ParameterType.Integer:
				return UniformType.Sampler;

			case ParameterType.Matrix3x3:
				return UniformType.Matrix3x3;

			case ParameterType.Matrix4x4:
				return UniformType.Matrix4x4;
			}

			return UniformType.Vector4;
		}

		public unsafe void SetUniform( Uniform uniform, ParameterType type, int arraySize, IntPtr valueData )
		{
			//int1, vec4, mat3, mat4
			var uniformType = GetUniformTypeByParameterType( type );
			//var uniform = GpuProgramManager.RegisterUniform( name, uniformType, arraySize );

			switch( uniformType )
			{
			//case UniformType.Sampler:
			//Log.Fatal( "ViewportRenderingContext: SetUniform: UniformType.Int1 impl." );
			//break;

			case UniformType.Vector4:
				{
					int sizeInBytes = ParameterTypeUtility.GetElementSizeInBytes( type );
					if( sizeInBytes == 16 )
					{
						Bgfx.SetUniform( uniform, valueData, arraySize );
					}
					else if( sizeInBytes < 16 )
					{
						if( arraySize != 1 )
							Log.Fatal( "ViewportRenderingContext: SetUniform: Arrays with type \'{0}\' are not supported. Only Vec4, Mat3, Mat4 arrays are supported.", type );

						Vector4F value = Vector4F.Zero;
						Buffer.MemoryCopy( (void*)valueData, &value, sizeInBytes, sizeInBytes );
						Bgfx.SetUniform( uniform, &value, 1 );
					}
					else
						Log.Fatal( "ViewportRenderingContext: SetUniform: The type \'{0}\' is not supported.", type );
				}
				break;

			case UniformType.Matrix3x3:
			case UniformType.Matrix4x4:
				Bgfx.SetUniform( uniform, valueData, arraySize );
				break;
			}
		}

		public unsafe void SetUniform( Uniform uniform, ParameterType type, int arraySize, void* valueData )
		{
			SetUniform( uniform, type, arraySize, (IntPtr)valueData );
		}

		public unsafe void SetUniform( Uniform uniform, ParameterType type, int arraySize, ArraySegment<byte> valueData )
		{
			fixed( byte* pData = valueData.Array )
				SetUniform( uniform, type, arraySize, (IntPtr)( pData + valueData.Offset ) );
		}

		public unsafe void SetUniform( string name, ParameterType type, int arraySize, IntPtr valueData )
		{
			//int1, vec4, mat3, mat4
			var uniformType = GetUniformTypeByParameterType( type );
			var uniform = GpuProgramManager.RegisterUniform( name, uniformType, arraySize );

			SetUniform( uniform, type, arraySize, valueData );
		}

		public unsafe void SetUniform( string name, ParameterType type, int arraySize, void* valueData )
		{
			SetUniform( name, type, arraySize, (IntPtr)valueData );
		}

		public unsafe void SetUniform( string name, ParameterType type, int arraySize, ArraySegment<byte> valueData )
		{
			fixed( byte* pData = valueData.Array )
				SetUniform( name, type, arraySize, (IntPtr)( pData + valueData.Offset ) );
		}

		//public unsafe void SetUniform( string name, Vec4F value )
		//{
		//	var uniform = GpuProgramManager.Instance.RegisterUniform( name, value.GetType(), 1 );
		//	Bgfx.SetUniform( uniform, &value );

		//	//var u = new Uniform( "u_test", UniformType.Vector4 );
		//	//Log.Info( u.ToString() );

		//	//Vec4F r = new Vec4F( 1, 0, 0, 1 );
		//	//Bgfx.SetUniform( u, &r );
		//	//Bgfx.SetUniform( GpuProgramManager.r, &r );
		//}

		//public unsafe void SetUniform( string name, Vec4F[] value )
		//{
		//}

		//static ParameterContainer.ParameterItem GetValueFromContainers( List<ParameterContainer> allContainers, string name )
		//{
		//	for( int n = allContainers.Count - 1; n >= 0; n-- )
		//	{
		//		ParameterContainer c = allContainers[ n ];
		//		ParameterContainer.ParameterItem item = c.GetByName( name );
		//		if( item != null )
		//			return item;
		//	}
		//	return null;
		//}

		internal unsafe void BindTexture( int textureUnit, ImageComponent texture, TextureAddressingMode addressingMode, FilterOption filteringMin, FilterOption filteringMag, FilterOption filteringMip, TextureFlags additionalFlags, ref uint* writeToPointer, ref int writeToPointerCount )
		{
			if( texture == null )
				Log.Fatal( "ViewportRenderingContext: BindTexture: texture == null." );

			//sampler settings
			TextureFlags flags = TextureFlags.None;

			//adressing mode
			if( addressingMode != TextureAddressingMode.Wrap )
			{
				if( ( addressingMode & TextureAddressingMode.Mirror ) != 0 )
				{
					if( ( addressingMode & TextureAddressingMode.MirrorU ) != 0 )
						flags |= TextureFlags.MirrorU;
					if( ( addressingMode & TextureAddressingMode.MirrorV ) != 0 )
						flags |= TextureFlags.MirrorV;
					if( ( addressingMode & TextureAddressingMode.MirrorW ) != 0 )
						flags |= TextureFlags.MirrorW;
				}
				if( ( addressingMode & TextureAddressingMode.Clamp ) != 0 )
				{
					if( ( addressingMode & TextureAddressingMode.ClampU ) != 0 )
						flags |= TextureFlags.ClampU;
					if( ( addressingMode & TextureAddressingMode.ClampV ) != 0 )
						flags |= TextureFlags.ClampV;
					if( ( addressingMode & TextureAddressingMode.ClampW ) != 0 )
						flags |= TextureFlags.ClampW;
				}
				if( ( addressingMode & TextureAddressingMode.Border ) != 0 )
				{
					if( ( addressingMode & TextureAddressingMode.BorderU ) != 0 )
						flags |= TextureFlags.BorderU;
					if( ( addressingMode & TextureAddressingMode.BorderV ) != 0 )
						flags |= TextureFlags.BorderV;
					if( ( addressingMode & TextureAddressingMode.BorderW ) != 0 )
						flags |= TextureFlags.BorderW;
				}
			}

			//filtering

			if( filteringMin == FilterOption.Anisotropic )
				flags |= TextureFlags.MinFilterAnisotropic;
			else if( filteringMin == FilterOption.Point )
				flags |= TextureFlags.MinFilterPoint;

			if( filteringMag == FilterOption.Anisotropic )
				flags |= TextureFlags.MagFilterAnisotropic;
			else if( filteringMag == FilterOption.Point )
				flags |= TextureFlags.MagFilterPoint;

			if( filteringMip == FilterOption.Point )
				flags |= TextureFlags.MipFilterPoint;
			//else if( value.filteringMip == FilterOption.Anisotropic )
			//	flags |= TextureFlags.MipFilterAnisotropic;

			flags |= additionalFlags;


			//if( value.texture?.Result?.SourceFormat == PixelFormat.Float32R &&
			//	value.texture?.Result?.ResultSize.X == value.texture?.Result?.ResultSize.Y )
			//	flags |= TextureFlags.BGFX_SAMPLER_COMPARE_LEQUAL;
			//if( value.texture?.Result?.ResultFormat == PixelFormat.Float32R &&
			//	value.texture?.Result?.ResultSize.X == value.texture?.Result?.ResultSize.Y ) // shadow textures need cmp flag for cmp samplers
			//	flags |= TextureFlags.BGFX_SAMPLER_COMPARE_LEQUAL;

			//!!!!impl border color
			//!!!!еще есть настройки MSAA

			var realObject = texture?.Result?.GetNativeObject( true );
			if( realObject != null )
			{
				if( writeToPointer != null )
				{
					*writeToPointer++ = (uint)textureUnit;
					*writeToPointer++ = Uniform.Invalid.handle;
					*writeToPointer++ = realObject.handle;
					*writeToPointer++ = (uint)flags;
					writeToPointerCount++;
				}
				else
					Bgfx.SetTexture( (byte)textureUnit, Uniform.Invalid, realObject, flags );
			}
		}

		internal unsafe void BindTexture( ref BindTextureData value, ref uint* writeToPointer, ref int writeToPointerCount )
		{
			BindTexture( value.TextureUnit, value.Texture, value.AddressingMode, value.FilteringMin, value.FilteringMag, value.FilteringMip, value.AdditionalFlags, ref writeToPointer, ref writeToPointerCount );
		}

		public void BindTexture( int textureUnit, ImageComponent texture, TextureAddressingMode addressingMode, FilterOption filteringMin, FilterOption filteringMag, FilterOption filteringMip, TextureFlags additionalFlags = 0 )
		{
			unsafe
			{
				uint* dummy1 = null;
				int dummy2 = 0;
				BindTexture( textureUnit, texture, addressingMode, filteringMin, filteringMag, filteringMip, additionalFlags, ref dummy1, ref dummy2 );
			}
		}

		public void BindTexture( ref BindTextureData value )
		{
			BindTexture( value.TextureUnit, value.Texture, value.AddressingMode, value.FilteringMin, value.FilteringMag, value.FilteringMip, value.AdditionalFlags );
		}

		//public void BindTexture( BindTextureData value )
		//{
		//	BindTexture( ref value );
		//}

		public void BindParameterContainer( ParameterContainer container )
		{
			if( container.NamedParameters != null )
			{
				foreach( var tuple in container.NamedParameters )
				{
					var name = tuple.Key;
					var p = tuple.Value;

					if( ParameterTypeUtility.CanConvertToByteArray( p.Type ) )
					{
						//uniforms
						SetUniform( name, p.Type, p.ElementCount, p.GetValue( container ) );
					}
					else
					{
						//Log.Fatal( "ViewportRenderingContext: BindParameterContainer: Is not supported named parameter." );
					}
					//else if( ParameterTypeUtility.IsTexture( p.Type ) )
					//{
					//	//textures

					//	//!!!!текстуры в вершинах

					//	var value = (GpuMaterialPass.TextureParameterValue)p.Value;
					//	var index = int.Parse( p.Name );
					//	BindTexture( index, value );
					//}
				}
			}

			if( container.UniformParameters != null )
			{
				for( int n = 0; n < container.UniformParameters.Count; n++ )
				{
					ref var p = ref container.UniformParameters.Data[ n ];
					SetUniform( p.Uniform, p.Type, p.ElementCount, p.GetValue( container ) );
				}
			}

			if( container.TextureParameters != null )
			{
				for( int n = 0; n < container.TextureParameters.Count; n++ )
					BindTexture( ref container.TextureParameters.Data[ n ] );
			}

			//foreach( var p in container.AllParameters )
			//{
			//	if( ParameterTypeUtility.CanConvertToByteArray( p.Type ) )
			//	{
			//		//uniforms

			//		var segment = p.GetValue();
			//		SetUniform( p.Name, p.Type, p.elementCount, segment );
			//	}
			//	else if( ParameterTypeUtility.IsTexture( p.Type ) )
			//	{
			//		//textures

			//		var value = (GpuMaterialPass.TextureParameterValue)p.Value;
			//		var index = int.Parse( p.Name );
			//		BindTexture( index, value );
			//	}
			//}
		}

		//!!!!parameterContainers пока так
		public void SetPassAndSubmit( GpuMaterialPass pass, RenderOperationType renderOperation, List<ParameterContainer> parameterContainers, OcclusionQuery? occlusionQuery = null )
		{
			////set parameters
			//unsafe
			//{
			//	var allContainers = new List<ParameterContainer>();
			//	if( pass.ConstantParameterValues != null )
			//		allContainers.Add( pass.ConstantParameterValues );
			//	allContainers.AddRange( parameterContainers );

			//	foreach( var program in pass.LinkedProgram.Programs )
			//	{
			//		foreach( var uniform in program.RealObject.Uniforms )
			//		{
			//			//skip samplers
			//			if( uniform == Uniform.Invalid )
			//				continue;

			//			var containerItem = GetValueFromContainers( allContainers, uniform.Name );
			//			if( containerItem == null )
			//			{
			//				//!!!!
			//				Log.Fatal( "ViewportRenderingContext: SetPassAndSubmit: Parameter value with name \'{0}\' is not exists in containers.", uniform.Name );
			//			}

			//			byte[] data = new byte[ containerItem.GetTotalSizeInBytes() ];
			//			containerItem.GetValue( data, 0 );

			//			bool success = false;

			//			switch( uniform.Type )
			//			{
			//			case UniformType.Int1:
			//				if( containerItem.Type == ParameterType.Integer )
			//				{
			//					fixed ( byte* pData = data )
			//						Bgfx.SetUniform( uniform, pData, containerItem.ElementCount );
			//					success = true;
			//				}
			//				break;

			//			case UniformType.Vector4:
			//				if( containerItem.Type == ParameterType.Vector4 )
			//				{
			//					fixed ( byte* pData = data )
			//						Bgfx.SetUniform( uniform, pData, containerItem.ElementCount );
			//					success = true;
			//				}
			//				else if( containerItem.Type == ParameterType.Vector3 && containerItem.ElementCount == 1 )
			//				{
			//					fixed ( byte* pData = data )
			//					{
			//						var value = new Vec4F( *(Vec3F*)pData, 0 );
			//						Bgfx.SetUniform( uniform, &value, containerItem.ElementCount );
			//					}
			//					success = true;
			//				}
			//				else if( containerItem.Type == ParameterType.Vector2 && containerItem.ElementCount == 1 )
			//				{
			//					fixed ( byte* pData = data )
			//					{
			//						var value2 = *(Vec2F*)pData;
			//						var value = new Vec4F( value2.X, value2.Y, 0, 0 );
			//						Bgfx.SetUniform( uniform, &value, containerItem.ElementCount );
			//					}
			//					success = true;
			//				}
			//				break;

			//			case UniformType.Matrix3x3:
			//				if( containerItem.Type == ParameterType.Matrix3x3 )
			//				{
			//					fixed ( byte* pData = data )
			//						Bgfx.SetUniform( uniform, pData, containerItem.ElementCount );
			//					success = true;
			//				}
			//				break;

			//			case UniformType.Matrix4x4:
			//				if( containerItem.Type == ParameterType.Matrix4x4 )
			//				{
			//					fixed ( byte* pData = data )
			//						Bgfx.SetUniform( uniform, pData, containerItem.ElementCount );
			//					success = true;
			//				}
			//				break;
			//			}

			//			if( !success )
			//			{
			//				//!!!!
			//				Log.Fatal( "ViewportRenderingContext: SetPassAndSubmit: Unable to convert parameter value with name \'{0}\'. Source type: {1}. Element count: {2}.", uniform.Name, containerItem.Type, containerItem.ElementCount );
			//			}
			//		}
			//	}
			//}

			//!!!!
			//bind parameter containers
			{
				//if( pass.ConstantParameterValues != null )
				//	BindParameterContainer( pass.ConstantParameterValues );

				if( parameterContainers != null )
				{
					for( int n = 0; n < parameterContainers.Count; n++ )
						BindParameterContainer( parameterContainers[ n ] );
				}

				//var containers = new List<ParameterContainer>();
				//if( pass.ConstantParameterValues != null && !pass.ConstantParameterValues.IsEmpty() )
				//	containers.Add( pass.ConstantParameterValues );
				//if( parameterContainers != null )
				//	containers.AddRange( parameterContainers );

				//foreach( var container in containers )
				//	BindParameterContainer( container );
			}

			pass.RenderingProcess_SetRenderState( renderOperation, occlusionQuery == null );

			var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

			if( occlusionQuery != null )
				Bgfx.Submit( CurrentViewNumber, pass.LinkedProgram.RealObject, occlusionQuery.Value, 0, discardFlags );// DiscardFlags.All );
			else
				Bgfx.Submit( CurrentViewNumber, pass.LinkedProgram.RealObject, 0, discardFlags );// DiscardFlags.All );

			//Bgfx.Submit( CurrentViewNumber, pass.LinkedProgram.RealObject, preserveState: true );

			UpdateStatisticsCurrent.DrawCalls++;
		}

		public StatisticsClass UpdateStatisticsCurrent
		{
			get { return updateStatisticsCurrent; }
		}

		public StatisticsClass UpdateStatisticsPrevious
		{
			get { return updateStatisticsPrevious; }
		}

		public RenderingPipeline RenderingPipeline
		{
			get { return renderingPipeline; }
		}

		public ColorValue GetBackgroundColor()
		{
			if( renderingPipeline != null && renderingPipeline.BackgroundColorOverride.HasValue )
				return renderingPipeline.BackgroundColorOverride.Value;
			if( owner.AttachedScene != null )
				return owner.AttachedScene.BackgroundColor;
			return owner.BackgroundColorDefault;
		}

		public float GetBackgroundColorAffectLighting()
		{
			if( owner.AttachedScene != null )
				return (float)owner.AttachedScene.BackgroundColorAffectLighting.Value;
			return 0;
		}

		public bool DynamicTexturesAreExists()
		{
			return dynamicTexturesAllocated.Count != 0 || multiRenderTargets.Count != 0;
		}

		public void UpdateGetVisibilityDistanceByObjectSize()
		{
			Curve1F function = null;

			if( OwnerCameraSettings.Projection != ProjectionType.Orthographic )
			{
				function = new CurveLine1F();
				//function = new CurveCubicSpline1F();

				var cameraSettings = OwnerCameraSettings;

				var offset = RenderingPipeline.MinimumVisibleSizeOfObjects.Value / (double)cameraSettings.Viewport.SizeInPixels.Y * 0.5;
				var ray1 = cameraSettings.GetRayByScreenCoordinates( new Vector2( 0.5, 0.5 - offset ) );
				var ray2 = cameraSettings.GetRayByScreenCoordinates( new Vector2( 0.5, 0.5 + offset ) );


				var distance = Owner.CameraSettings.FarClipDistance;

				for( int n = 0; n < 15; n++ )
				{
					var pos = cameraSettings.Position + cameraSettings.Rotation * ( Vector3.XAxis * distance );

					var plane = Plane.FromPointAndNormal( pos, cameraSettings.Rotation * Vector3.XAxis );

					plane.Intersects( ref ray1, out Vector3 intersect1 );
					plane.Intersects( ref ray2, out Vector3 intersect2 );

					var objectSize = ( intersect1 - intersect2 ).Length();

					function.AddPoint( (float)objectSize, (float)distance );


					distance *= 0.5;



					//var pos = cameraSettings.Position + cameraSettings.Rotation * ( Vector3.XAxis * distance );
					//var offset = cameraSettings.Frustum.Up * minimumVisibleSizeOfObjects * 0.5;
					//var point1 = pos - offset;
					//var point2 = pos + offset;

					//var points = new Vector3[ 2 ];
					//points[ 0 ] = point1;
					//points[ 1 ] = point2;

					////var cameraDistanceMinSquared2 = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, ref group.BoundingBox );
					////var cameraDistanceMin = Math.Sqrt( cameraDistanceMinSquared2 );

					////var points = SpaceBounds.CalculatedBoundingBox.ToPoints();

					//var min = 10000.0;
					//var max = -10000.0;

					//foreach( var p in points )
					//{
					//	if( cameraSettings.ProjectToScreenCoordinates( p, out var screenP ) )
					//	{
					//		if( screenP.Y < min )
					//			min = screenP.Y;
					//		if( screenP.Y > max )
					//			max = screenP.Y;
					//	}
					//}

					//var heightInPixels = ( max - min ) * cameraSettings.Viewport.SizeInPixels.Y;

					//var objectSize = zzzzz;

					//function.AddPoint( (float)objectSize, (float)distance );


					//var pos = cameraSettings.Position + cameraSettings.Rotation * ( Vector3.XAxis * distance );
					//zzzzz;
					//var offset = cameraSettings.Frustum.Up * minimumVisibleSizeOfObjects * 0.5;
					//var point1 = pos - offset;
					//var point2 = pos + offset;

					//var points = new Vector3[ 2 ];
					//points[ 0 ] = point1;
					//points[ 1 ] = point2;

					////var cameraDistanceMinSquared2 = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, ref group.BoundingBox );
					////var cameraDistanceMin = Math.Sqrt( cameraDistanceMinSquared2 );

					////var points = SpaceBounds.CalculatedBoundingBox.ToPoints();

					//var min = 10000.0;
					//var max = -10000.0;

					//foreach( var p in points )
					//{
					//	if( cameraSettings.ProjectToScreenCoordinates( p, out var screenP ) )
					//	{
					//		if( screenP.Y < min )
					//			min = screenP.Y;
					//		if( screenP.Y > max )
					//			max = screenP.Y;
					//	}
					//}

					//var heightInPixels = ( max - min ) * cameraSettings.Viewport.SizeInPixels.Y;

					//var objectSize = zzzzz;

					//function.AddPoint( (float)objectSize, (float)distance );
					////function.AddPoint( (float)heightInPixels, (float)distance );
					////function.AddPoint( (float)distance, ZZZZZZZ );

					//distance *= 0.5;
				}
			}

			getVisibilityDistanceByObjectSize = function;
		}

		public double GetVisibilityDistanceByObjectSize( double objectSize )
		{
			if( getVisibilityDistanceByObjectSize != null )
				return getVisibilityDistanceByObjectSize.CalculateValueByTime( (float)objectSize );
			else
				return OwnerCameraSettings.FarClipDistance * 1.1;
		}

		public double GetShadowVisibilityDistance( double visibilityDistance )
		{
			return visibilityDistance * ShadowObjectVisibilityDistanceFactor;
		}

		public double GetShadowVisibilityDistanceSquared( double visibilityDistance )
		{
			var v = visibilityDistance * ShadowObjectVisibilityDistanceFactor;
			return v * v;
		}
	}
}
