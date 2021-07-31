// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace NeoAxis
{
	public partial class Component_RenderingPipeline_Basic
	{
		void DisplayObjectInSpaceBounds( ViewportRenderingContext context, FrameData frameData )
		{
			var context2 = context.objectInSpaceRenderingContext;
			var viewport = context.Owner;
			var scene = viewport.AttachedScene;

			var objects = new List<(Component_ObjectInSpace, float)>( frameData.ObjectInSpaces.Count );
			for( int n = 0; n < frameData.ObjectInSpaces.Count; n++ )
			{
				ref var data = ref frameData.ObjectInSpaces.Data[ n ];

				if( data.InsideFrustum )
				{
					var obj = data.ObjectInSpace;
					var center = obj.SpaceBounds.CalculatedBoundingBox.GetCenter();
					var distanceSquared = ( center - viewport.CameraSettings.Position ).LengthSquared();
					//var distanceSquared = ( obj.TransformV.Position - viewport.CameraSettings.Position ).LengthSquared();
					objects.Add( (obj, (float)distanceSquared) );
				}
			}

			CollectionUtility.MergeSort( objects, delegate ( (Component_ObjectInSpace, float) item1, (Component_ObjectInSpace, float) item2 )
			{
				var distanceSquared1 = item1.Item2;
				var distanceSquared2 = item2.Item2;
				if( distanceSquared1 < distanceSquared2 )
					return -1;
				if( distanceSquared1 > distanceSquared2 )
					return 1;
				return 0;
			}, true );

			int counter = 0;

			foreach( var item in objects )
			{
				var obj = item.Item1;

				ColorValue color = ProjectSettings.Get.SceneShowObjectInSpaceBoundsColor;
				viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

				var bounds = obj.SpaceBounds.CalculatedBoundingBox;

				double lineThickness = 0;
				//precalculate line thickness
				if( bounds.GetSize().MaxComponent() < 10 )
					lineThickness = viewport.Simple3DRenderer.GetThicknessByPixelSize( bounds.GetCenter(), ProjectSettings.Get.LineThickness );

				viewport.Simple3DRenderer.AddBounds( bounds, false, lineThickness );

				counter++;
				if( counter >= context2.displayObjectInSpaceBoundsMax )
					break;
			}
		}

		void DisplayPhysicalObjects( ViewportRenderingContext context, FrameData frameData )
		{
			var scene = context.Owner.AttachedScene;
			var viewport = context.Owner;
			var context2 = context.objectInSpaceRenderingContext;

			if( ( scene.GetDisplayDevelopmentDataInThisApplication() && scene.DisplayPhysicalObjects ) ||
				context2.selectedObjects.Count != 0 || context2.canSelectObjects.Count != 0 || context2.objectToCreate != null )
			{
				var objects = new List<(Component_IPhysicalObject, float)>( frameData.ObjectInSpaces.Count );
				for( int n = 0; n < frameData.ObjectInSpaces.Count; n++ )
				{
					ref var data = ref frameData.ObjectInSpaces.Data[ n ];

					if( data.InsideFrustum )
					{
						var obj = data.ObjectInSpace;
						if( obj is Component_IPhysicalObject physicalObject )
						{
							var center = obj.SpaceBounds.CalculatedBoundingBox.GetCenter();
							var distanceSquared = ( center - viewport.CameraSettings.Position ).LengthSquared();
							//var distanceSquared = ( obj.TransformV.Position - viewport.CameraSettings.Position ).LengthSquared();
							objects.Add( (physicalObject, (float)distanceSquared) );
						}
					}
				}

				CollectionUtility.MergeSort( objects, delegate ( (Component_IPhysicalObject, float) item1, (Component_IPhysicalObject, float) item2 )
				{
					var distanceSquared1 = item1.Item2;
					var distanceSquared2 = item2.Item2;
					if( distanceSquared1 < distanceSquared2 )
						return -1;
					if( distanceSquared1 > distanceSquared2 )
						return 1;
					return 0;
				}, true );

				int counterCount = 0;
				int counterVertices = 0;

				foreach( var item in objects )
				{
					var obj = item.Item1;

					bool show = ( scene.GetDisplayDevelopmentDataInThisApplication() && scene.DisplayPhysicalObjects ) ||
						context2.selectedObjects.Contains( obj ) || context2.canSelectObjects.Contains( obj ) || context2.objectToCreate == obj;
					if( show )
					{
						obj.Render( context, out var verticesRendered );

						counterCount++;
						if( counterCount >= context2.displayPhysicalObjectsMaxCount )
							break;
						counterVertices += verticesRendered;
						if( counterVertices >= context2.displayPhysicalObjectsMaxVertices )
							break;
					}
				}
			}
		}

		void SortObjectInSpaceLabels( ViewportRenderingContext context )
		{
			var array = context.Owner.LastFrameScreenLabels.ToArray();

			CollectionUtility.MergeSort( array, delegate ( Viewport.LastFrameScreenLabelItem item1, Viewport.LastFrameScreenLabelItem item2 )
			{
				if( item1.DistanceToCamera > item2.DistanceToCamera )
					return -1;
				if( item1.DistanceToCamera < item2.DistanceToCamera )
					return 1;
				return 0;
			}, true );

			context.Owner.LastFrameScreenLabels.Clear();
			foreach( var item in array )
				context.Owner.LastFrameScreenLabels.AddLast( item );
		}

		void DisplayObjectInSpaceLabels( ViewportRenderingContext context )
		{
			var viewport = context.Owner;
			//var context2 = context.objectInSpaceRenderingContext;

			var currentImage = "Default";
			var triangles = new List<CanvasRenderer.TriangleVertex>( context.Owner.LastFrameScreenLabels.Count * 6 );

			var maxSize = ProjectSettings.Get.ScreenLabelMaxSize.Value;
			var add = "";
			if( maxSize <= 24 )
				add = @"24\";
			else if( maxSize <= 32 )
				add = @"32\";

			//calculate screen rectangle for display in corner item
			{
				var list = new List<Viewport.LastFrameScreenLabelItem>();
				foreach( var item in viewport.LastFrameScreenLabels )
				{
					if( item.DistanceToCamera < 0 )
						list.Add( item );
				}

				Vector2 sizeInPixels = new Vector2( maxSize, maxSize );
				Vector2 screenSize = sizeInPixels / viewport.SizeInPixels.ToVector2();

				double pos = 1.0 - screenSize.X * 0.25;

				for( int n = list.Count - 1; n >= 0; n-- )
				{
					var item = list[ n ];
					//var obj = item.Object;

					var rect = new Rectangle( pos - screenSize.X, screenSize.Y * 0.25, pos, screenSize.Y * 1.25 ).ToRectangleF();

					pos -= screenSize.X * 1.25;

					//ColorValue color;
					//if( context2.selectedObjects.Contains( obj ) )
					//	color = ProjectSettings.Get.SelectedColor;
					//else if( context2.canSelectObjects.Contains( obj ) )
					//	color = ProjectSettings.Get.CanSelectColor;
					//else
					//	color = ProjectSettings.Get.ScreenLabelColor;

					//var item = new Viewport.LastFrameScreenLabelItem();
					//item.Object = obj;
					//zzzz;
					//item.DistanceToCamera = -1;
					item.ScreenRectangle = rect;
					//item.Color = color;
					//if( !obj.EnabledInHierarchy )
					//	item.Color.Alpha *= 0.5f;

					//item.AlwaysVisible = true;
					//viewport.LastFrameScreenLabels.Add( item );
					//viewport.LastFrameScreenLabelByObjectInSpace[ obj ] = item;


					//screenPositionY = item.ScreenRectangle.Bottom;
				}
			}

			foreach( var label in viewport.LastFrameScreenLabels )
			{
				if( label.Color.Alpha > 0 )
				{
					string imageName = "Default";
					if( ProjectSettings.Get.ScreenLabelDisplayIcons && label.Object != null )
					{
						var name = label.Object.GetScreenLabelInfo().LabelName;
						if( !string.IsNullOrEmpty( name ) )
							imageName = name;
					}

					if( imageName != currentImage && triangles.Count != 0 )
					{
						var texture = ResourceManager.LoadResource<Component_Image>( @"Base\UI\Images\Labels\" + add + currentImage + ".png" );
						viewport.CanvasRenderer.AddTriangles( triangles, texture, true );

						triangles.Clear();
					}

					currentImage = imageName;

					var rect = label.ScreenRectangle.ToRectangleF();

					var v0 = new CanvasRenderer.TriangleVertex( rect.LeftTop, label.Color, new Vector2F( 0, 0 ) );
					var v1 = new CanvasRenderer.TriangleVertex( rect.RightTop, label.Color, new Vector2F( 1, 0 ) );
					var v2 = new CanvasRenderer.TriangleVertex( rect.RightBottom, label.Color, new Vector2F( 1, 1 ) );
					var v3 = new CanvasRenderer.TriangleVertex( rect.LeftBottom, label.Color, new Vector2F( 0, 1 ) );

					triangles.Add( v0 );
					triangles.Add( v1 );
					triangles.Add( v2 );
					triangles.Add( v2 );
					triangles.Add( v3 );
					triangles.Add( v0 );
				}
			}

			if( triangles.Count != 0 )
			{
				var texture = ResourceManager.LoadResource<Component_Image>( @"Base\UI\Images\Labels\" + add + currentImage + ".png" );
				viewport.CanvasRenderer.AddTriangles( triangles, texture, true );
			}
		}

		//!!!!

		////!!!!чистить при переключении RenderingMethod. корректно чистить
		////!!!!чистить при изменениях размера. что еще
		////!!!!чистить при удалении пайплайна

		//class PhysicallyCorrectRenderingData
		//{
		//	public Component_Image cameraDistancesTexture;
		//	public Component_Image cameraDistancesTextureRead;
		//	public IntPtr depthData;
		//	public int depthDataDemandedFrame;
		//	public bool depthDataDone;

		//	//!!!!temp deferred?
		//	public Component_Image deferredLightingTexture;

		//	//!!!!

		//	public void Clear()
		//	{
		//		//!!!!

		//		cameraDistancesTexture?.Dispose();
		//		cameraDistancesTexture = null;
		//		if( depthData != IntPtr.Zero )
		//		{
		//			NativeUtility.Free( depthData );
		//			depthData = IntPtr.Zero;
		//		}
		//		depthDataDemandedFrame = 0;
		//		depthDataDone = false;

		//		deferredLightingTexture?.Dispose();
		//		deferredLightingTexture = null;
		//	}
		//}
		//PhysicallyCorrectRenderingData physicallyCorrectRenderingData;

		//public override void PhysicallyCorrectRendering_ResetFrame()
		//{
		//	physicallyCorrectRenderingData?.Clear();
		//}

		//void PhysicallyCorrectRendering_CallSaveDepthBufferToTexture( ViewportRenderingContext context )
		//{
		//	if( RenderingMethod.Value != RenderingMethodEnum.PhysicallyCorrect )
		//		return;

		//	if( physicallyCorrectRenderingData == null )
		//		physicallyCorrectRenderingData = new PhysicallyCorrectRenderingData();

		//	var owner = context.Owner;

		//	//!!!!double
		//	Matrix4F viewMatrix = owner.CameraSettings.ViewMatrix.ToMatrix4F();
		//	Matrix4F projectionMatrix = owner.CameraSettings.ProjectionMatrix.ToMatrix4F();

		//	context.objectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
		//	if( depthTexture == null )
		//	{
		//		//!!!!
		//		return;
		//	}

		//	if( physicallyCorrectRenderingData.cameraDistancesTexture == null )
		//	{
		//		var resolution = context.owner.SizeInPixels;
		//		var format = PixelFormat.Float32R;

		//		var texture = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
		//		physicallyCorrectRenderingData.cameraDistancesTexture = texture;
		//		texture.CreateType = Component_Image.TypeEnum._2D;
		//		texture.CreateSize = resolution;
		//		texture.CreateMipmaps = false;
		//		texture.CreateFormat = format;
		//		texture.CreateUsage = Component_Image.Usages.RenderTarget;
		//		texture.CreateFSAA = 0;
		//		texture.Enabled = true;

		//		var renderTexture = texture.Result.GetRenderTarget();
		//		var viewport = renderTexture.AddViewport( false, true );
		//		//!!!!
		//		viewport.RenderingPipelineCreate();
		//		viewport.RenderingPipelineCreated.UseRenderTargets = false;

		//		var textureRead = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
		//		physicallyCorrectRenderingData.cameraDistancesTextureRead = texture;
		//		textureRead.CreateType = Component_Image.TypeEnum._2D;
		//		textureRead.CreateSize = resolution;
		//		textureRead.CreateMipmaps = false;
		//		textureRead.CreateFormat = format;
		//		textureRead.CreateUsage = Component_Image.Usages.ReadBack | Component_Image.Usages.BlitDestination;
		//		textureRead.CreateFSAA = 0;
		//		textureRead.Enabled = true;
		//		//!!!!
		//		textureRead.Result.PrepareNativeObject();


		//		//render
		//		{
		//			context.SetViewport( viewport, viewMatrix, projectionMatrix );

		//			CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
		//			shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
		//			shader.FragmentProgramFileName = @"Base\Shaders\PhysicallyCorrect_CameraDistances_fs.sc";

		//			shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"depthTexture"*/,
		//				depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

		//			//shader.Parameters.Set( "affectBackground", new Vector4F( (float)frameData.Fog.AffectBackground.Value, 0, 0, 0 ) );

		//			context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Opaque );

		//			//viewport.Update( true, cameraSettings );

		//			//!!!!
		//			//clear temp data
		//			viewport.RenderingContext?.MultiRenderTarget_DestroyAll();
		//			viewport.RenderingContext?.DynamicTexture_DestroyAll();
		//		}

		//		//blit to textureRead
		//		//!!!!
		//		texture.Result.RealObject.BlitTo( context.CurrentViewNumber, textureRead.Result.RealObject, 0, 0 );
		//		//texture.Result.RealObject.BlitTo( viewport.RenderingContext.CurrentViewNumber, textureRead.Result.RealObject, 0, 0 );

		//		//begin reading
		//		var totalBytes = PixelFormatUtility.GetNumElemBytes( format ) * resolution.X * resolution.Y;
		//		physicallyCorrectRenderingData.depthData = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, totalBytes );
		//		physicallyCorrectRenderingData.depthDataDemandedFrame = textureRead.Result.RealObject.Read( physicallyCorrectRenderingData.depthData, 0 );
		//	}

		//	//check reading is done
		//	if( RenderingSystem.LastFrameNumber >= physicallyCorrectRenderingData.depthDataDemandedFrame )
		//		physicallyCorrectRenderingData.depthDataDone = true;


		//	//!!!!

		//}

		//void PhysicallyCorrectRendering_RenderLightsDeferred( ViewportRenderingContext context, out bool handled )
		//{
		//	handled = false;

		//	if( RenderingMethod.Value != RenderingMethodEnum.PhysicallyCorrect )
		//		return;
		//	if( physicallyCorrectRenderingData.cameraDistancesTexture == null )
		//		return;
		//	if( !physicallyCorrectRenderingData.depthDataDone )
		//		return;

		//	handled = true;

		//	var resolution = context.owner.SizeInPixels;

		//	//deferredLightingTexture = zzzzzzz;

		//	//create texture
		//	if( physicallyCorrectRenderingData.deferredLightingTexture == null )
		//	{
		//		var mipmaps = false;

		//		var usage = Component_Image.Usages.WriteOnly;
		//		//if( mipmaps )
		//		//	usage |= Component_Image.Usages.AutoMipmaps;

		//		var texture2 = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
		//		physicallyCorrectRenderingData.deferredLightingTexture = texture2;
		//		texture2.CreateType = Component_Image.TypeEnum._2D;
		//		texture2.CreateSize = resolution;
		//		texture2.CreateMipmaps = mipmaps;

		//		//!!!!temp
		//		texture2.CreateFormat = PixelFormat.Float32RGBA;//A8R8G8B8

		//		texture2.CreateUsage = usage;
		//		texture2.Enabled = true;
		//	}

		//	//!!!!stride. где еще

		//	var texture = physicallyCorrectRenderingData.deferredLightingTexture;
		//	var format = texture.Result.ResultFormat;

		//	//update texture
		//	var data = new byte[ PixelFormatUtility.GetNumElemBytes( format ) * resolution.X * resolution.Y ];

		//	unsafe
		//	{
		//		fixed( byte* pData = data )
		//		{
		//			Vector4F* data2 = (Vector4F*)pData;

		//			for( int y = 0; y < resolution.Y; y++ )
		//			{
		//				for( int x = 0; x < resolution.X; x++ )
		//				{
		//					data2[ y * resolution.X + x ] = new Vector4F( (float)x / 1000.0f, 0, 0, 0 );
		//				}
		//			}
		//		}
		//	}

		//	texture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( 0, 0, data ) } );

		//	////!!!!
		//	//var sourceTexture = physicallyCorrectRenderingData.cameraDistancesTexture;

		//	//generate compile arguments
		//	var vertexDefines = new List<(string, string)>();
		//	var fragmentDefines = new List<(string, string)>();

		//	string error2;

		//	//vertex program
		//	var vertexProgram = GpuProgramManager.GetProgram( "PhysicallyCorrect_DeferredLight_",
		//		GpuProgramType.Vertex, @"Base\Shaders\PhysicallyCorrect_DeferredLight_vs.sc", vertexDefines, out error2 );
		//	if( !string.IsNullOrEmpty( error2 ) )
		//	{
		//		Log.Fatal( error2 );
		//		return;
		//	}

		//	var fragmentProgram = GpuProgramManager.GetProgram( "PhysicallyCorrect_DeferredLight_",
		//		GpuProgramType.Fragment, @"Base\Shaders\PhysicallyCorrect_DeferredLight_fs.sc", fragmentDefines, out error2 );
		//	if( !string.IsNullOrEmpty( error2 ) )
		//	{
		//		Log.Fatal( error2 );
		//		return;
		//	}


		//	{
		//		var generalContainer = new ParameterContainer();

		//		{
		//			//bind textures

		//			generalContainer.Set( new ViewportRenderingContext.BindTextureData( 0/* "sceneTexture"*/, texture,
		//				TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//			//generalContainer.Set( new ViewportRenderingContext.BindTextureData( 0/* "sceneTexture"*/, sceneTexture,
		//			//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//			//generalContainer.Set( new ViewportRenderingContext.BindTextureData( 1/* "normalTexture"*/, normalTexture,
		//			//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//			//generalContainer.Set( new ViewportRenderingContext.BindTextureData( 2/* "gBuffer2Texture"*/, gBuffer2Texture,
		//			//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//			//generalContainer.Set( new ViewportRenderingContext.BindTextureData( 8/*"s_brdfLUT"*/, BrdfLUT,
		//			//	TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

		//			//generalContainer.Set( new ViewportRenderingContext.BindTextureData( 9/* "gBuffer2Texture"*/, gBuffer3Texture,
		//			//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//			//generalContainer.Set( new ViewportRenderingContext.BindTextureData( 3/* "depthTexture"*/, depthTexture,
		//			//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//			//generalContainer.Set( new ViewportRenderingContext.BindTextureData( 10/* "gBuffer4Texture"*/, gBuffer4Texture,
		//			//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
		//		}

		//		{
		//			var shader = new CanvasRenderer.ShaderItem();
		//			shader.CompiledVertexProgram = vertexProgram;
		//			shader.CompiledFragmentProgram = fragmentProgram;
		//			shader.AdditionalParameterContainers.Add( generalContainer );

		//			context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
		//		}
		//	}

		//}

	}
}
