#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace NeoAxis.Editor
{
	public class RenderToFileSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonRender;

		///////////////////////////////////////////////

		class NewMaterialData
		{
			//public int Index;
			//public string Name;
			//public ShadingModelEnum ShadingModel = ShadingModelEnum.Lit;
			//public bool TwoSided;

			//public ColorValue? BaseColor;
			public string BaseColorTexture;
			public string MetallicTexture;
			public string RoughnessTexture;
			public string NormalTexture;
			public string DisplacementTexture;
			public string AmbientOcclusionTexture;
			public string EmissiveTexture;
			public string OpacityTexture;
			public string SubsurfaceColorTexture;

			public string GetTextureValueByName( string name )
			{
				switch( name )
				{
				case "BaseColor": return BaseColorTexture;
				case "Metallic": return MetallicTexture;
				case "Roughness": return RoughnessTexture;
				case "Normal": return NormalTexture;
				case "Displacement": return DisplacementTexture;
				case "AmbientOcclusion": return AmbientOcclusionTexture;
				case "Emissive": return EmissiveTexture;
				case "Opacity": return OpacityTexture;
				case "SubsurfaceColor": return SubsurfaceColorTexture;
				}
				return "";
			}

			public void SetTextureValueByName( string name, string value )
			{
				switch( name )
				{
				case "BaseColor": BaseColorTexture = value; break;
				case "Metallic": MetallicTexture = value; break;
				case "Roughness": RoughnessTexture = value; break;
				case "Normal": NormalTexture = value; break;
				case "Displacement": DisplacementTexture = value; break;
				case "AmbientOcclusion": AmbientOcclusionTexture = value; break;
				case "Emissive": EmissiveTexture = value; break;
				case "Opacity": OpacityTexture = value; break;
				case "SubsurfaceColor": SubsurfaceColorTexture = value; break;
				}
			}

			public int GetTextureCount()
			{
				int result = 0;
				if( !string.IsNullOrEmpty( BaseColorTexture ) )
					result++;
				if( !string.IsNullOrEmpty( MetallicTexture ) )
					result++;
				if( !string.IsNullOrEmpty( RoughnessTexture ) )
					result++;
				if( !string.IsNullOrEmpty( NormalTexture ) )
					result++;
				if( !string.IsNullOrEmpty( DisplacementTexture ) )
					result++;
				if( !string.IsNullOrEmpty( AmbientOcclusionTexture ) )
					result++;
				if( !string.IsNullOrEmpty( EmissiveTexture ) )
					result++;
				if( !string.IsNullOrEmpty( OpacityTexture ) )
					result++;
				if( !string.IsNullOrEmpty( SubsurfaceColorTexture ) )
					result++;
				return result;
			}

			//public NewMaterialData Clone()
			//{
			//	var result = new NewMaterialData();

			//	result.BaseColorTexture = BaseColorTexture;
			//	result.MetallicTexture = MetallicTexture;
			//	result.RoughnessTexture = RoughnessTexture;
			//	result.NormalTexture = NormalTexture;
			//	result.DisplacementTexture = DisplacementTexture;
			//	result.AmbientOcclusionTexture = AmbientOcclusionTexture;
			//	result.EmissiveTexture = EmissiveTexture;
			//	result.OpacityTexture = OpacityTexture;
			//  ..

			//	return result;
			//}
		}

		///////////////////////////////////////////////

		enum MaterialChannel
		{
			Opacity,
			BaseColor,
			Metallic,
			Roughness,
			Normal,
			SubsurfaceColor,
		}

		///////////////////////////////////////////////

		protected override void OnInit()
		{
			buttonRender = ProcedureForm.CreateButton( EditorLocalization2.Translate( "General", "Render" ) );
			buttonRender.Click += ButtonCalculate_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonRender } );
		}

		private void ButtonCalculate_Click( ProcedureUI.Button sender )
		{
			RenderByEngineRender();
		}

		public RenderToFile RenderToFile
		{
			get { return GetFirstObject<RenderToFile>(); }
		}

		void RenderByEngineRender()
		{
			var renderToFile = RenderToFile;
			var scene = renderToFile.ParentRoot as Scene;

			//get camera
			var camera = renderToFile.Camera.Value;
			if( renderToFile.Mode.Value == RenderToFile.ModeEnum.Screenshot || renderToFile.Mode.Value == RenderToFile.ModeEnum.Material )
			{
				if( camera == null )
					camera = scene.Mode.Value == Scene.ModeEnum._2D ? scene.CameraEditor2D : scene.CameraEditor;
				if( camera == null )
				{
					EditorMessageBox.ShowWarning( "Camera is not specified." );
					return;
				}
			}

			//get rendering pipeline
			var renderingPipeline = renderToFile.RenderingPipeline.Value;
			if( renderingPipeline == null )
				renderingPipeline = scene.RenderingPipeline.Value;

			var destRealFileName = renderToFile.OutputFileName.Value.Trim();
			if( !Path.IsPathRooted( destRealFileName ) && !string.IsNullOrEmpty( destRealFileName ) )
				destRealFileName = VirtualPathUtility.GetRealPathByVirtual( destRealFileName );

			if( string.IsNullOrEmpty( destRealFileName ) )
			{
				switch( renderToFile.Mode.Value )
				{
				case RenderToFile.ModeEnum.Screenshot:
					if( !EditorUtility2.ShowSaveFileDialog( "", "Output.png", "PNG files (*.png)|*.png", out destRealFileName ) )
						return;
					break;

				case RenderToFile.ModeEnum.Video:
					if( !EditorUtility2.ShowSaveFileDialog( "", "Output.avi", "AVI files (*.avi)|*.avi", out destRealFileName ) )
						return;
					break;

				case RenderToFile.ModeEnum.Material:
					if( !EditorUtility2.ShowSaveFileDialog( "", "Output.material", "Material files (*.material)|*.material", out destRealFileName ) )
						return;
					break;
				}
			}
			else
			{
				//file already exists
				if( File.Exists( destRealFileName ) )
				{
					var text = $"The file with name \'{destRealFileName}\' is already exists. Overwrite?";
					if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
						return;
				}
			}

			var item = ScreenNotifications2.ShowSticky( "Processing..." );
			try
			{
				switch( renderToFile.Mode.Value )
				{
				case RenderToFile.ModeEnum.Screenshot:
					RenderScreenshot( camera, renderingPipeline, destRealFileName );
					break;

				case RenderToFile.ModeEnum.Video:
					RenderVideo( camera, renderingPipeline, destRealFileName );
					break;

				case RenderToFile.ModeEnum.Material:
					RenderMaterial( camera, destRealFileName );
					break;
				}
			}
			finally
			{
				item.Close();
			}

		}

		private void Scene_GetDisplayDevelopmentDataInThisApplicationOverride( Scene sender, ref bool display )
		{
			display = RenderToFile.DisplayDevelopmentData;
		}

		void RenderScreenshot( Camera camera, RenderingPipeline renderingPipeline, string destRealFileName )
		{
			var renderToFile = RenderToFile;
			var scene = renderToFile.ParentRoot as Scene;

			ImageComponent texture = null;
			ImageComponent textureRead = null;

			try
			{
				//create
				var resolution = renderToFile.Resolution.Value;

				//!!!!impl
				var hdr = false;//HDR.Value;
				PixelFormat format = hdr ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8;
				//PixelFormat format = hdr ? PixelFormat.Float32RGBA : PixelFormat.A8R8G8B8;

				texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
				texture.CreateType = ImageComponent.TypeEnum._2D;
				texture.CreateSize = resolution;
				texture.CreateMipmaps = false;
				texture.CreateFormat = format;
				texture.CreateUsage = ImageComponent.Usages.RenderTarget;
				texture.CreateFSAA = 0;
				texture.Enabled = true;

				var renderTexture = texture.Result.GetRenderTarget( 0, 0 );
				var viewport = renderTexture.AddViewport( true, true );
				viewport.AttachedScene = scene;

				textureRead = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
				textureRead.CreateType = ImageComponent.TypeEnum._2D;
				textureRead.CreateSize = resolution;
				textureRead.CreateMipmaps = false;
				textureRead.CreateFormat = format;
				textureRead.CreateUsage = ImageComponent.Usages.ReadBack | ImageComponent.Usages.BlitDestination;
				textureRead.CreateFSAA = 0;
				textureRead.Enabled = true;

				//render
				//var image2D = new ImageUtility.Image2D( PixelFormat.Float32RGB, new Vector2I( size * 4, size * 3 ) );

				//var position = Transform.Value.Position;

				//for( int face = 0; face < 6; face++ )
				//{

				//Vector3 dir = Vector3.Zero;
				//Vector3 up = Vector3.Zero;

				////flipped
				//switch( face )
				//{
				//case 0: dir = -Vector3.YAxis; up = Vector3.ZAxis; break;
				//case 1: dir = Vector3.YAxis; up = Vector3.ZAxis; break;
				//case 2: dir = Vector3.ZAxis; up = -Vector3.XAxis; break;
				//case 3: dir = -Vector3.ZAxis; up = Vector3.XAxis; break;
				//case 4: dir = Vector3.XAxis; up = Vector3.ZAxis; break;
				//case 5: dir = -Vector3.XAxis; up = Vector3.ZAxis; break;
				//}

				try
				{
					scene.GetDisplayDevelopmentDataInThisApplicationOverride += Scene_GetDisplayDevelopmentDataInThisApplicationOverride;

					var cameraSettings = new Viewport.CameraSettingsClass( viewport, camera, renderingPipeline );

					//var cameraSettings = new Viewport.CameraSettingsClass( viewport, 1, 90, NearClipPlane.Value, FarClipPlane.Value, position, dir, up, ProjectionType.Perspective, 1, 1, 1 );

					viewport.Update( true, cameraSettings );

					//clear temp data
					viewport.RenderingContext.MultiRenderTarget_DestroyAll();
					viewport.RenderingContext.DynamicTexture_DestroyAll();
				}
				finally
				{
					scene.GetDisplayDevelopmentDataInThisApplicationOverride -= Scene_GetDisplayDevelopmentDataInThisApplicationOverride;
				}

				texture.Result.GetNativeObject( true ).BlitTo( (ushort)viewport.RenderingContext.CurrentViewNumber, textureRead.Result.GetNativeObject( true ), 0, 0 );


				//get data
				var totalBytes = PixelFormatUtility.GetNumElemBytes( format ) * resolution.X * resolution.Y;
				var data = new byte[ totalBytes ];
				unsafe
				{
					fixed( byte* pBytes = data )
					{
						var demandedFrame = textureRead.Result.GetNativeObject( true ).Read( (IntPtr)pBytes, 0 );
						while( RenderingSystem.CallBgfxFrame() < demandedFrame ) { }
					}
				}

				var image = new ImageUtility.Image2D( format, resolution, data );

				//reset alpha channel
				for( int y = 0; y < image.Size.Y; y++ )
				{
					for( int x = 0; x < image.Size.X; x++ )
					{
						var pixel = image.GetPixel( new Vector2I( x, y ) );
						pixel.W = 1.0f;
						image.SetPixel( new Vector2I( x, y ), pixel );
					}
				}

				//image.Data
				//image2D.Blit( index * size, faceImage );

				//Vector2I index = Vector2I.Zero;
				//switch( face )
				//{
				//case 0: index = new Vector2I( 2, 1 ); break;
				//case 1: index = new Vector2I( 0, 1 ); break;
				//case 2: index = new Vector2I( 1, 0 ); break;
				//case 3: index = new Vector2I( 1, 2 ); break;
				//case 4: index = new Vector2I( 1, 1 ); break;
				//case 5: index = new Vector2I( 3, 1 ); break;
				//}

				//var faceImage = new ImageUtility.Image2D( format, new Vector2I( size, size ), data );
				//image2D.Blit( index * size, faceImage );
				//}

				if( !Directory.Exists( Path.GetDirectoryName( destRealFileName ) ) )
					Directory.CreateDirectory( Path.GetDirectoryName( destRealFileName ) );

				if( !ImageUtility.Save( destRealFileName, image.Data, image.Size, 1, image.Format, 1, 0, out var error ) )
					throw new Exception( error );
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return;
			}
			finally
			{
				texture?.Dispose();
				textureRead?.Dispose();
			}

			ScreenNotifications2.Show( "Rendering completed successfully." );
		}

		void MaterialNewObjectCreateShaderGraph( Material material, NewMaterialData data, out FlowGraph graph )
		{
			graph = material.CreateComponent<FlowGraph>();
			graph.Name = "Shader graph";
			graph.Specialization = ReferenceUtility.MakeReference(
				MetadataManager.GetTypeOfNetType( typeof( FlowGraphSpecialization_Shader ) ).Name + "|Instance" );

			{
				var node = graph.CreateComponent<FlowGraphNode>();
				node.Name = "Node " + "Material";//Name;
				node.Position = new Vector2I( 10, -7 );
				node.ControlledObject = ReferenceUtility.MakeThisReference( node, material );
			}

			//configure
			{
				const int step = 9;
				Vector2I position = new Vector2I( -20, -data.GetTextureCount() * step / 2 );

				//BaseColor
				if( !string.IsNullOrEmpty( data.BaseColorTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "BaseColor";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.BaseColorTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					material.BaseColor = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
				}
				//else if( data.BaseColor.HasValue )
				//	BaseColor = data.BaseColor.Value;

				//Metallic
				if( !string.IsNullOrEmpty( data.MetallicTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Metallic";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.MetallicTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					material.Metallic = ReferenceUtility.MakeThisReference( material, sample, "R" );
				}

				//Roughness
				if( !string.IsNullOrEmpty( data.RoughnessTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Roughness";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.RoughnessTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					material.Roughness = ReferenceUtility.MakeThisReference( material, sample, "R" );
				}

				//Normal
				if( !string.IsNullOrEmpty( data.NormalTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Normal";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.NormalTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					material.Normal = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
				}

				////Displacement
				//if( !string.IsNullOrEmpty( data.DisplacementTexture ) )
				//{
				//	var node = graph.CreateComponent<FlowGraphNode>();
				//	node.Name = "Node Texture Sample " + "Displacement";
				//	node.Position = position;
				//	position.Y += step;

				//	var sample = node.CreateComponent<ShaderTextureSample>();
				//	sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				//	sample.Texture = new Reference<Image>( null, data.DisplacementTexture );

				//	node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				//	Displacement = ReferenceUtility.MakeThisReference( this, sample, "R" );
				//}

				////AmbientOcclusion
				//if( !string.IsNullOrEmpty( data.AmbientOcclusionTexture ) )
				//{
				//	var node = graph.CreateComponent<FlowGraphNode>();
				//	node.Name = "Node Texture Sample " + "AmbientOcclusion";
				//	node.Position = position;
				//	position.Y += step;

				//	var sample = node.CreateComponent<ShaderTextureSample>();
				//	sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				//	sample.Texture = new Reference<Image>( null, data.AmbientOcclusionTexture );

				//	node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				//	AmbientOcclusion = ReferenceUtility.MakeThisReference( this, sample, "R" );
				//}

				////Emissive
				//if( !string.IsNullOrEmpty( data.EmissiveTexture ) )
				//{
				//	var node = graph.CreateComponent<FlowGraphNode>();
				//	node.Name = "Node Texture Sample " + "Emissive";
				//	node.Position = position;
				//	position.Y += step;

				//	var sample = node.CreateComponent<ShaderTextureSample>();
				//	sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				//	sample.Texture = new Reference<Image>( null, data.EmissiveTexture );

				//	node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				//	Emissive = ReferenceUtility.MakeThisReference( this, sample, "RGBA" );
				//}

				//Opacity
				if( !string.IsNullOrEmpty( data.OpacityTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Opacity";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.OpacityTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					material.Opacity = ReferenceUtility.MakeThisReference( material, sample, "R" );

					material.BlendMode = Material.BlendModeEnum.Masked;
				}

				//SubsurfaceColor
				if( !string.IsNullOrEmpty( data.SubsurfaceColorTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "SubsurfaceColor";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.SubsurfaceColorTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					material.SubsurfaceColor = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
				}
			}
		}

		///////////////////////////////////////////////

		struct DistanceMapItem
		{
			public float distance;
			public Vector2I offset;
		}
		static Vector2I[] distanceMap;
		static Vector2I distanceMapForSize;

		static Vector2I[] GetDistanceMap( ImageUtility.Image2D image )
		{
			if( distanceMap == null || distanceMapForSize != image.Size )
			{
				var map = new List<DistanceMapItem>( image.Size.X * 5 );

				for( int y = -image.Size.Y; y <= image.Size.Y; y++ )
				{
					for( int x = -image.Size.X; x <= image.Size.X; x++ )
					{
						if( x == 0 && y == 0 )
							continue;

						var item = new DistanceMapItem();
						item.offset = new Vector2I( x, y );
						item.distance = MathEx.Sqrt( x * x + y * y );
						map.Add( item );
					}
				}

				//distanceMap = map.ToArray();
				distanceMapForSize = image.Size;

				CollectionUtility.MergeSort( map, delegate ( DistanceMapItem item1, DistanceMapItem item2 )
				{
					if( item1.distance < item2.distance )
						return -1;
					else if( item1.distance > item2.distance )
						return 1;
					return 0;
				}, true );

				distanceMap = new Vector2I[ map.Count ];
				for( var n = 0; n < distanceMap.Length; n++ )
					distanceMap[ n ] = map[ n ].offset;
			}

			return distanceMap;
		}

		///////////////////////////////////////////////

		static void FillTransparentPixelsByNearPixels2( ref ImageUtility.Image2D image, Vector2I[,] opacityImageNearestCellTable )
		{
			for( int y = 0; y < image.Size.Y; y++ )
			{
				for( int x = 0; x < image.Size.X; x++ )
				{
					var takeFrom = opacityImageNearestCellTable[ x, y ];
					var pixel = image.GetPixelByte( new Vector2I( takeFrom.X, takeFrom.Y ) );

					image.SetPixel( new Vector2I( x, y ), pixel );


					//var transparent = opacityImage.GetPixel( new Vector2I( x, y ) ).ToVector3F() == Vector3F.Zero;

					//ColorByte pixel;
					//if( transparent )
					//{
					//	var takeFrom = opacityImageNearestCellTable[ x, y ];
					//	pixel = image.GetPixelByte( new Vector2I( takeFrom.X, takeFrom.Y ) );
					//}
					//else
					//	pixel = image.GetPixelByte( new Vector2I( x, y ) );

					//image.SetPixel( new Vector2I( x, y ), pixel );
				}
			}
		}

		void RenderMaterial( Camera camera, string destRealFileName )
		{
			var renderToFile = RenderToFile;
			var scene = renderToFile.ParentRoot as Scene;

			var textureFileNames = new string[ 6 ];
			//ImageUtility.Image2D opacityImage = null;
			Vector2I[,] opacityImageNearestCellTable = null;

			//write textures
			for( int nChannel = 0; nChannel < 6; nChannel++ )
			{
				var channel = (MaterialChannel)nChannel;

				ImageComponent texture = null;
				ImageComponent textureRead = null;

				try
				{
					var prefix = Path.GetFileNameWithoutExtension( destRealFileName ) + "_";

					string fileName = "";
					switch( nChannel )
					{
					case 0: fileName = prefix + "Opacity.png"; break;
					case 1: fileName = prefix + "BaseColor.png"; break;
					case 2: fileName = prefix + "Metallic.png"; break;
					case 3: fileName = prefix + "Roughness.png"; break;
					case 4: fileName = prefix + "Normal.png"; break;
					case 5: fileName = prefix + "SubsurfaceColor.png"; break;
					}

					var fullPath = Path.Combine( Path.GetDirectoryName( destRealFileName ), fileName );

					//create
					var resolution = renderToFile.Resolution.Value;

					PixelFormat format = PixelFormat.A8R8G8B8;

					texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
					texture.CreateType = ImageComponent.TypeEnum._2D;
					texture.CreateSize = resolution;
					texture.CreateMipmaps = false;
					texture.CreateFormat = format;
					texture.CreateUsage = ImageComponent.Usages.RenderTarget;
					texture.CreateFSAA = 0;
					texture.Enabled = true;

					var renderTexture = texture.Result.GetRenderTarget( 0, 0 );
					var viewport = renderTexture.AddViewport( true, true );
					viewport.AttachedScene = scene;

					textureRead = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
					textureRead.CreateType = ImageComponent.TypeEnum._2D;
					textureRead.CreateSize = resolution;
					textureRead.CreateMipmaps = false;
					textureRead.CreateFormat = format;
					textureRead.CreateUsage = ImageComponent.Usages.ReadBack | ImageComponent.Usages.BlitDestination;
					textureRead.CreateFSAA = 0;
					textureRead.Enabled = true;


					var restorePipeline = scene.RenderingPipeline;

					var pipeline = ComponentUtility.CreateComponent<RenderingPipeline_Basic>( null, true, true );
					switch( nChannel )
					{
					case 0: pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.Normal; break;
					case 1: pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.BaseColor; break;
					case 2: pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.Metallic; break;
					case 3: pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.Roughness; break;
					case 4: pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.Normal; break;
					case 5: pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.SubsurfaceColor; break;
					}

					try
					{
						scene.RenderingPipeline = pipeline;
						scene.GetDisplayDevelopmentDataInThisApplicationOverride += Scene_GetDisplayDevelopmentDataInThisApplicationOverride;

						var cameraSettings = new Viewport.CameraSettingsClass( viewport, camera );

						viewport.Update( true, cameraSettings );

						//clear temp data
						viewport.RenderingContext.MultiRenderTarget_DestroyAll();
						viewport.RenderingContext.DynamicTexture_DestroyAll();
					}
					finally
					{
						scene.RenderingPipeline = restorePipeline;
						scene.GetDisplayDevelopmentDataInThisApplicationOverride -= Scene_GetDisplayDevelopmentDataInThisApplicationOverride;
					}

					texture.Result.GetNativeObject( true ).BlitTo( (ushort)viewport.RenderingContext.CurrentViewNumber, textureRead.Result.GetNativeObject( true ), 0, 0 );

					//get data
					var totalBytes = PixelFormatUtility.GetNumElemBytes( format ) * resolution.X * resolution.Y;
					var data = new byte[ totalBytes ];
					unsafe
					{
						fixed( byte* pBytes = data )
						{
							var demandedFrame = textureRead.Result.GetNativeObject( true ).Read( (IntPtr)pBytes, 0 );
							while( RenderingSystem.CallBgfxFrame() < demandedFrame ) { }
						}
					}

					var image = new ImageUtility.Image2D( format, resolution, data );

					if( channel == MaterialChannel.Opacity )
					{
						//convert pixels
						for( int y = 0; y < image.Size.Y; y++ )
						{
							for( int x = 0; x < image.Size.X; x++ )
							{
								var pixel = image.GetPixel( new Vector2I( x, y ) );

								if( pixel.ToVector3F() != Vector3F.Zero )
									pixel = Vector4F.One;
								else
									pixel = Vector4F.Zero;

								image.SetPixel( new Vector2I( x, y ), pixel );
							}
						}

						//opacityImageNearestCellTable
						if( renderToFile.FillTransparentPixelsByNearPixels )
						{
							var boolOpacityImage = new int[ image.Size.X, image.Size.Y ];
							for( int y = 0; y < image.Size.Y; y++ )
							{
								for( int x = 0; x < image.Size.X; x++ )
								{
									var c = image.GetPixelByte( new Vector2I( x, y ) );
									boolOpacityImage[ x, y ] = c.Red == 0 ? 1 : 0;
									//boolOpacityImage[ x, y ] = c.Red == 0;
								}
							}

							var distanceMap = GetDistanceMap( image );

							opacityImageNearestCellTable = new Vector2I[ image.Size.X, image.Size.Y ];
							for( int y = 0; y < image.Size.Y; y++ )
								for( int x = 0; x < image.Size.X; x++ )
									opacityImageNearestCellTable[ x, y ] = new Vector2I( x, y );

							//var table = opacityImageNearestCellTable;


							//var time = EngineApp.GetSystemTime();

							var imageSizeX = image.Size.X;
							var imageSizeY = image.Size.Y;

							Parallel.For( 0, image.Size.X * image.Size.Y, delegate ( int xy )
							{
								var y = xy / imageSizeX;
								var x = xy % imageSizeX;

								var transparent = boolOpacityImage[ x, y ];
								if( transparent != 0 )//if( transparent )
								{
									for( int n = 0; n < distanceMap.Length; n++ )
									{
										ref var indexItem = ref distanceMap[ n ];

										var takeFromX = x + indexItem.X;
										var takeFromY = y + indexItem.Y;
										if( takeFromX >= 0 && takeFromX < imageSizeX && takeFromY >= 0 && takeFromY < imageSizeY )
										{
											var transparent2 = boolOpacityImage[ takeFromX, takeFromY ];
											if( transparent2 == 0 )//if( !transparent2 )
											{
												opacityImageNearestCellTable[ x, y ] = new Vector2I( takeFromX, takeFromY );
												break;
											}
										}
									}
								}
							} );

							//Parallel.For( 0, image.Size.Y, delegate ( int y )//for( int y = 0; y < image.Size.Y; y++ )
							//{
							//	for( int x = 0; x < image.Size.X; x++ )
							//	{
							//		var transparent = boolOpacityImage[ x, y ];
							//		if( transparent )
							//		{
							//			for( int n = 0; n < distanceMap.Length; n++ )//foreach( var indexItem in distanceMap )
							//			{
							//				ref var indexItem = ref distanceMap[ n ];

							//				var takeFrom = new Vector2I( x, y ) + indexItem.offset;
							//				if( takeFrom.X >= 0 && takeFrom.X < image.Size.X && takeFrom.Y >= 0 && takeFrom.Y < image.Size.Y )
							//				{
							//					var transparent2 = boolOpacityImage[ takeFrom.X, takeFrom.Y ];
							//					if( !transparent2 )
							//					{
							//						table[ x, y ] = takeFrom;
							//						break;
							//					}
							//				}
							//			}
							//		}
							//	}
							//} );

							//var time2 = EngineApp.GetSystemTime() - time;
							//Log.Info( time2.ToString() );

						}

					}

					if( channel == MaterialChannel.BaseColor || channel == MaterialChannel.Metallic || channel == MaterialChannel.Roughness || channel == MaterialChannel.Normal || channel == MaterialChannel.SubsurfaceColor )
					{
						//reset alpha channel
						for( int y = 0; y < image.Size.Y; y++ )
						{
							for( int x = 0; x < image.Size.X; x++ )
							{
								var pixel = image.GetPixel( new Vector2I( x, y ) );
								pixel.W = 1.0f;
								image.SetPixel( new Vector2I( x, y ), pixel );
							}
						}
					}

					//check for zero metallic and for subsurface color
					var skip = false;
					if( channel == MaterialChannel.Metallic || channel == MaterialChannel.SubsurfaceColor )
					{
						var allZero = true;

						for( int y = 0; y < image.Size.Y; y++ )
						{
							for( int x = 0; x < image.Size.X; x++ )
							{
								var pixel = image.GetPixel( new Vector2I( x, y ) );
								if( pixel != new Vector4F( 0, 0, 0, 1 ) )
								{
									allZero = false;
									break;
								}
							}
						}

						if( allZero )
							skip = true;
					}

					if( !skip )
					{
						if( channel == MaterialChannel.BaseColor || channel == MaterialChannel.Metallic || channel == MaterialChannel.Roughness || channel == MaterialChannel.Normal || channel == MaterialChannel.SubsurfaceColor )
						{
							//fill transparent pixels by near pixels
							if( opacityImageNearestCellTable != null )
								FillTransparentPixelsByNearPixels2( ref image, opacityImageNearestCellTable );
						}

						if( !Directory.Exists( Path.GetDirectoryName( fullPath ) ) )
							Directory.CreateDirectory( Path.GetDirectoryName( fullPath ) );

						if( !ImageUtility.Save( fullPath, image.Data, image.Size, 1, image.Format, 1, 0, out var error ) )
							throw new Exception( error );

						textureFileNames[ nChannel ] = VirtualPathUtility.GetVirtualPathByReal( fullPath );
					}

				}
				catch( Exception e )
				{
					EditorMessageBox.ShowWarning( e.Message );
					return;
				}
				finally
				{
					texture?.Dispose();
					textureRead?.Dispose();
				}
			}

			//write material
			try
			{
				//var materialsInScene = new List<Material>();
				//{
				//	foreach( var obj in scene.GetComponents<MeshInSpace>( onlyEnabledInHierarchy: true ) )
				//	{
				//		var m = obj.ReplaceMaterial.Value;
				//		if( m != null )
				//			materialsInScene.Add( m );
				//	}

				//	foreach( var obj in scene.GetComponents<CurveInSpace>( onlyEnabledInHierarchy: true ) )
				//	{
				//		var m = obj.Material.Value;
				//		if( m != null )
				//			materialsInScene.Add( m );
				//	}
				//}


				//Metadata.TypeInfo materialType = null;
				//{
				//	foreach( var m in materialsInScene )
				//	{
				//		if( MetadataManager.GetTypeOfNetType( typeof( Material ) ) != m.BaseType )
				//		{
				//			if( materialType == null )
				//				materialType = m.BaseType;
				//			break;
				//		}
				//	}

				//	if( materialType == null )
				//		materialType = MetadataManager.GetTypeOfNetType( typeof( Material ) );
				//}

				Material material = null;
				var template = renderToFile.Template.Value;
				if( template != null )
					material = (Material)template.Clone();
				if( material == null )
					material = ComponentUtility.CreateComponent<Material>( null, true, false );

				//var material = (Material)ComponentUtility.CreateComponent( materialType, null, true, false );
				//material.TwoSided = RenderToFile.TwoSided;
				//material.TwoSidedFlipNormals = RenderToFile.TwoSidedFlipNormals;

				////use Subsurface, Masked material parameters if available
				//foreach( var m in materialsInScene )
				//{
				//	//!!!!how also to get used materials?
				//	//!!!!what to do when material have different parameters?

				//	if( m.ShadingModel.Value == Material.ShadingModelEnum.Subsurface )
				//	{
				//		material.ShadingModel = Material.ShadingModelEnum.Subsurface;
				//		if( !m.SubsurfacePower.ReferenceSpecified )
				//			material.SubsurfacePower = m.SubsurfacePower.Value;
				//		if( !m.SubsurfaceColor.ReferenceSpecified )
				//			material.SubsurfaceColor = m.SubsurfaceColor.Value;
				//		if( !m.Thickness.ReferenceSpecified )
				//			material.Thickness = m.Thickness.Value;
				//	}

				//	if( m.BlendMode.Value == Material.BlendModeEnum.Masked )
				//	{
				//		if( m.OpacityMaskThreshold.Value != 0.5 && !m.OpacityMaskThreshold.ReferenceSpecified )
				//			material.OpacityMaskThreshold = m.OpacityMaskThreshold.Value;
				//	}
				//}

				var data = new NewMaterialData();
				data.OpacityTexture = textureFileNames[ 0 ];
				data.BaseColorTexture = textureFileNames[ 1 ];
				data.MetallicTexture = textureFileNames[ 2 ];
				data.RoughnessTexture = textureFileNames[ 3 ];
				data.NormalTexture = textureFileNames[ 4 ];
				data.SubsurfaceColorTexture = textureFileNames[ 5 ];

				MaterialNewObjectCreateShaderGraph( material, data, out var graph );

				var toSelect = new Component[] { material, graph };
				material.EditorDocumentConfiguration = EditorAPI.CreateEditorDocumentXmlConfiguration( toSelect, graph );

				material.Enabled = true;


				if( !ComponentUtility.SaveComponentToFile( material, destRealFileName, null, out var error ) )
					throw new Exception( error );

				material.Dispose();

			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return;
			}

			ScreenNotifications2.Show( "Rendering completed successfully." );
		}

		void RenderVideo( Camera camera, RenderingPipeline renderingPipeline, string destRealFileName )
		{
			var component = RenderToFile.ParentRoot;
			//var component = LoadedResource?.ResultComponent;

			if( !RunSimulation.CheckTypeSupportedByPlayer( component.BaseType ) )
				return;
			if( !EditorAPI2.SaveDocuments() )
				return;
			//if( Modified )
			//{
			//	if( !Save( null ) )
			//		return;
			//}

			var document = EditorAPI2.GetDocumentByObject( component );
			if( document == null )
				return;

			var format = "";
			switch( RenderToFile.Format.Value )
			{
			case RenderToFile.FormatEnum.NoCompression: format = "DIB "; break;
			case RenderToFile.FormatEnum.LagarithLosslessLAGS: format = "LAGS"; break;
			case RenderToFile.FormatEnum.Other: format = RenderToFile.FormatFourCC.Value; break;
			}

			RunSimulation.RunRenderVideoToFile( document.RealFileName, destRealFileName, RunSimulation.RunMethod.Player, RenderToFile.FramesPerSecond, RenderToFile.Length, camera != null ? camera.GetPathFromRoot() : "", renderingPipeline != null ? renderingPipeline.GetPathFromRoot() : "", RenderToFile.Resolution.Value, format );
		}
	}
}

#endif
#endif