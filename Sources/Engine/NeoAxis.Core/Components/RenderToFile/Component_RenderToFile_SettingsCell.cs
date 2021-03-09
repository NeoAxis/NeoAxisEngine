// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace NeoAxis.Editor
{
	public class Component_RenderToFile_SettingsCell : SettingsCellProcedureUI
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
				return result;
			}

			public NewMaterialData Clone()
			{
				var result = new NewMaterialData();

				result.BaseColorTexture = BaseColorTexture;
				result.MetallicTexture = MetallicTexture;
				result.RoughnessTexture = RoughnessTexture;
				result.NormalTexture = NormalTexture;
				result.DisplacementTexture = DisplacementTexture;
				result.AmbientOcclusionTexture = AmbientOcclusionTexture;
				result.EmissiveTexture = EmissiveTexture;
				result.OpacityTexture = OpacityTexture;

				return result;
			}
		}

		///////////////////////////////////////////////

		enum MaterialChannel
		{
			Opacity,
			BaseColor,
			Metallic,
			Roughness,
			Normal,
		}

		///////////////////////////////////////////////

		protected override void OnInit()
		{
			buttonRender = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Render" ) );
			buttonRender.Click += ButtonCalculate_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonRender } );
		}

		private void ButtonCalculate_Click( ProcedureUI.Button sender )
		{
			RenderByEngineRender();
		}

		public Component_RenderToFile RenderToFile
		{
			get { return GetFirstObject<Component_RenderToFile>(); }
		}

		void RenderByEngineRender()
		{
			var renderToFile = RenderToFile;
			var scene = renderToFile.ParentRoot as Component_Scene;

			//get camera
			var camera = renderToFile.Camera.Value;
			if( camera == null )
				camera = scene.Mode.Value == Component_Scene.ModeEnum._2D ? scene.CameraEditor2D : scene.CameraEditor;
			if( camera == null )
			{
				EditorMessageBox.ShowWarning( "Camera is not specified." );
				return;
			}

			var destRealFileName = renderToFile.OutputFileName.Value.Trim();
			if( !Path.IsPathRooted( destRealFileName ) && !string.IsNullOrEmpty( destRealFileName ) )
				destRealFileName = VirtualPathUtility.GetRealPathByVirtual( destRealFileName );

			if( string.IsNullOrEmpty( destRealFileName ) )
			{
				var dialog = new SaveFileDialog();
				//dialog.InitialDirectory = Path.GetDirectoryName( RealFileName );
				if( renderToFile.Mode.Value == Component_RenderToFile.ModeEnum.Material )
				{
					dialog.FileName = "Output.material";
					dialog.Filter = "Material files (*.material)|*.material";
				}
				else
				{
					dialog.FileName = "Output.png";
					dialog.Filter = "PNG files (*.png)|*.png";
				}
				dialog.RestoreDirectory = true;
				if( dialog.ShowDialog() != DialogResult.OK )
					return;

				destRealFileName = dialog.FileName;
			}
			else
			{

				//!!!!material mode

				//file already exists
				if( File.Exists( destRealFileName ) )
				{
					var text = $"The file with name \'{destRealFileName}\' is already exists. Overwrite?";
					if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
						return;
				}
			}

			var item = ScreenNotifications.ShowSticky( "Processing..." );
			try
			{
				if( renderToFile.Mode.Value == Component_RenderToFile.ModeEnum.Material )
					RenderMaterial( camera, destRealFileName );
				else
					RenderScreenshot( camera, destRealFileName );
			}
			finally
			{
				item.Close();
			}

		}

		private void Scene_GetDisplayDevelopmentDataInThisApplicationOverride( Component_Scene sender, ref bool display )
		{
			display = RenderToFile.DisplayDevelopmentData;
		}

		void RenderScreenshot( Component_Camera camera, string destRealFileName )
		{
			var renderToFile = RenderToFile;
			var scene = renderToFile.ParentRoot as Component_Scene;

			Component_Image texture = null;
			Component_Image textureRead = null;

			try
			{
				//create
				var resolution = renderToFile.Resolution.Value;

				//!!!!impl
				var hdr = false;//HDR.Value;
				PixelFormat format = hdr ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8;
				//PixelFormat format = hdr ? PixelFormat.Float32RGBA : PixelFormat.A8R8G8B8;

				texture = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
				texture.CreateType = Component_Image.TypeEnum._2D;
				texture.CreateSize = resolution;
				texture.CreateMipmaps = false;
				texture.CreateFormat = format;
				texture.CreateUsage = Component_Image.Usages.RenderTarget;
				texture.CreateFSAA = 0;
				texture.Enabled = true;

				var renderTexture = texture.Result.GetRenderTarget( 0, 0 );
				var viewport = renderTexture.AddViewport( true, true );
				viewport.AttachedScene = scene;

				textureRead = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
				textureRead.CreateType = Component_Image.TypeEnum._2D;
				textureRead.CreateSize = resolution;
				textureRead.CreateMipmaps = false;
				textureRead.CreateFormat = format;
				textureRead.CreateUsage = Component_Image.Usages.ReadBack | Component_Image.Usages.BlitDestination;
				textureRead.CreateFSAA = 0;
				textureRead.Enabled = true;
				//!!!!
				textureRead.Result.PrepareNativeObject();

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

					var cameraSettings = new Viewport.CameraSettingsClass( viewport, camera );

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

				texture.Result.GetRealObject( true ).BlitTo( viewport.RenderingContext.CurrentViewNumber, textureRead.Result.GetRealObject( true ), 0, 0 );


				//!!!!pitch

				//get data
				var totalBytes = PixelFormatUtility.GetNumElemBytes( format ) * resolution.X * resolution.Y;
				var data = new byte[ totalBytes ];
				unsafe
				{
					fixed( byte* pBytes = data )
					{
						var demandedFrame = textureRead.Result.GetRealObject( true ).Read( (IntPtr)pBytes, 0 );
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

			ScreenNotifications.Show( "Rendering completed successfully." );
		}

		void MaterialNewObjectCreateShaderGraph( Component_Material material, NewMaterialData data, out Component_FlowGraph graph )
		{
			graph = material.CreateComponent<Component_FlowGraph>();
			graph.Name = "Shader graph";
			graph.Specialization = ReferenceUtility.MakeReference(
				MetadataManager.GetTypeOfNetType( typeof( Component_FlowGraphSpecialization_Shader ) ).Name + "|Instance" );

			{
				var node = graph.CreateComponent<Component_FlowGraphNode>();
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
					var node = graph.CreateComponent<Component_FlowGraphNode>();
					node.Name = "Node Texture Sample " + "BaseColor";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<Component_ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<Component_Image>( null, data.BaseColorTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					material.BaseColor = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
				}
				//else if( data.BaseColor.HasValue )
				//	BaseColor = data.BaseColor.Value;

				//Metallic
				if( !string.IsNullOrEmpty( data.MetallicTexture ) )
				{
					var node = graph.CreateComponent<Component_FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Metallic";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<Component_ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<Component_Image>( null, data.MetallicTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					material.Metallic = ReferenceUtility.MakeThisReference( material, sample, "R" );
				}

				//Roughness
				if( !string.IsNullOrEmpty( data.RoughnessTexture ) )
				{
					var node = graph.CreateComponent<Component_FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Roughness";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<Component_ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<Component_Image>( null, data.RoughnessTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					material.Roughness = ReferenceUtility.MakeThisReference( material, sample, "R" );
				}

				//Normal
				if( !string.IsNullOrEmpty( data.NormalTexture ) )
				{
					var node = graph.CreateComponent<Component_FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Normal";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<Component_ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<Component_Image>( null, data.NormalTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					material.Normal = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
				}

				////Displacement
				//if( !string.IsNullOrEmpty( data.DisplacementTexture ) )
				//{
				//	var node = graph.CreateComponent<Component_FlowGraphNode>();
				//	node.Name = "Node Texture Sample " + "Displacement";
				//	node.Position = position;
				//	position.Y += step;

				//	var sample = node.CreateComponent<Component_ShaderTextureSample>();
				//	sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				//	sample.Texture = new Reference<Component_Image>( null, data.DisplacementTexture );

				//	node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				//	Displacement = ReferenceUtility.MakeThisReference( this, sample, "R" );
				//}

				////AmbientOcclusion
				//if( !string.IsNullOrEmpty( data.AmbientOcclusionTexture ) )
				//{
				//	var node = graph.CreateComponent<Component_FlowGraphNode>();
				//	node.Name = "Node Texture Sample " + "AmbientOcclusion";
				//	node.Position = position;
				//	position.Y += step;

				//	var sample = node.CreateComponent<Component_ShaderTextureSample>();
				//	sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				//	sample.Texture = new Reference<Component_Image>( null, data.AmbientOcclusionTexture );

				//	node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				//	AmbientOcclusion = ReferenceUtility.MakeThisReference( this, sample, "R" );
				//}

				////Emissive
				//if( !string.IsNullOrEmpty( data.EmissiveTexture ) )
				//{
				//	var node = graph.CreateComponent<Component_FlowGraphNode>();
				//	node.Name = "Node Texture Sample " + "Emissive";
				//	node.Position = position;
				//	position.Y += step;

				//	var sample = node.CreateComponent<Component_ShaderTextureSample>();
				//	sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				//	sample.Texture = new Reference<Component_Image>( null, data.EmissiveTexture );

				//	node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				//	Emissive = ReferenceUtility.MakeThisReference( this, sample, "RGBA" );
				//}

				//Opacity
				if( !string.IsNullOrEmpty( data.OpacityTexture ) )
				{
					var node = graph.CreateComponent<Component_FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Opacity";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<Component_ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<Component_Image>( null, data.OpacityTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					material.Opacity = ReferenceUtility.MakeThisReference( material, sample, "R" );

					material.BlendMode = Component_Material.BlendModeEnum.Masked;
				}
			}
		}

		///////////////////////////////////////////////

		struct DistanceMapItem
		{
			public float distance;
			public Vector2I offset;
		}
		static DistanceMapItem[] distanceMap;
		static Vector2I distanceMapForSize;

		DistanceMapItem[] GetDistanceMap( ImageUtility.Image2D image )
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

				distanceMap = map.ToArray();
				distanceMapForSize = image.Size;

				CollectionUtility.MergeSort( distanceMap, delegate ( DistanceMapItem item1, DistanceMapItem item2 )
				{
					if( item1.distance < item2.distance )
						return -1;
					else if( item1.distance > item2.distance )
						return 1;
					return 0;
				}, true );
			}

			return distanceMap;
		}

		///////////////////////////////////////////////

		void FillTransparentPixelsByNearPixels2( ref ImageUtility.Image2D image, Vector2I[,] opacityImageNearestCellTable )
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

		void RenderMaterial( Component_Camera camera, string destRealFileName )
		{
			var renderToFile = RenderToFile;
			var scene = renderToFile.ParentRoot as Component_Scene;

			var textureFileNames = new string[ 5 ];
			//ImageUtility.Image2D opacityImage = null;
			Vector2I[,] opacityImageNearestCellTable = null;

			//write textures
			for( int nChannel = 0; nChannel < 5; nChannel++ )
			{
				var channel = (MaterialChannel)nChannel;

				Component_Image texture = null;
				Component_Image textureRead = null;

				try
				{
					//!!!!все каналы
					//!!!!какие еще параметры?

					var prefix = Path.GetFileNameWithoutExtension( destRealFileName ) + "_";

					string fileName = "";
					switch( nChannel )
					{
					case 0: fileName = prefix + "Opacity.png"; break;
					case 1: fileName = prefix + "BaseColor.png"; break;
					case 2: fileName = prefix + "Metallic.png"; break;
					case 3: fileName = prefix + "Roughness.png"; break;
					case 4: fileName = prefix + "Normal.png"; break;
					}

					var fullPath = Path.Combine( Path.GetDirectoryName( destRealFileName ), fileName );

					//create
					var resolution = renderToFile.Resolution.Value;

					PixelFormat format = PixelFormat.A8R8G8B8;

					texture = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
					texture.CreateType = Component_Image.TypeEnum._2D;
					texture.CreateSize = resolution;
					texture.CreateMipmaps = false;
					texture.CreateFormat = format;
					texture.CreateUsage = Component_Image.Usages.RenderTarget;
					texture.CreateFSAA = 0;
					texture.Enabled = true;

					var renderTexture = texture.Result.GetRenderTarget( 0, 0 );
					var viewport = renderTexture.AddViewport( true, true );
					viewport.AttachedScene = scene;

					textureRead = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
					textureRead.CreateType = Component_Image.TypeEnum._2D;
					textureRead.CreateSize = resolution;
					textureRead.CreateMipmaps = false;
					textureRead.CreateFormat = format;
					textureRead.CreateUsage = Component_Image.Usages.ReadBack | Component_Image.Usages.BlitDestination;
					textureRead.CreateFSAA = 0;
					textureRead.Enabled = true;
					//!!!!
					textureRead.Result.PrepareNativeObject();


					var restorePipeline = scene.RenderingPipeline;

					var pipeline = ComponentUtility.CreateComponent<Component_RenderingPipeline_Default>( null, true, true );
					switch( nChannel )
					{
					case 0: pipeline.DebugMode = Component_RenderingPipeline_Basic.DebugModeEnum.Normal; break;
					case 1: pipeline.DebugMode = Component_RenderingPipeline_Basic.DebugModeEnum.BaseColor; break;
					case 2: pipeline.DebugMode = Component_RenderingPipeline_Basic.DebugModeEnum.Metallic; break;
					case 3: pipeline.DebugMode = Component_RenderingPipeline_Basic.DebugModeEnum.Roughness; break;
					case 4: pipeline.DebugMode = Component_RenderingPipeline_Basic.DebugModeEnum.Normal; break;
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

					texture.Result.GetRealObject( true ).BlitTo( viewport.RenderingContext.CurrentViewNumber, textureRead.Result.GetRealObject( true ), 0, 0 );


					//!!!!pitch

					//get data
					var totalBytes = PixelFormatUtility.GetNumElemBytes( format ) * resolution.X * resolution.Y;
					var data = new byte[ totalBytes ];
					unsafe
					{
						fixed( byte* pBytes = data )
						{
							var demandedFrame = textureRead.Result.GetRealObject( true ).Read( (IntPtr)pBytes, 0 );
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
							var boolOpacityImage = new bool[ image.Size.X, image.Size.Y ];
							for( int y = 0; y < image.Size.Y; y++ )
							{
								for( int x = 0; x < image.Size.X; x++ )
								{
									var c = image.GetPixelByte( new Vector2I( x, y ) );
									boolOpacityImage[ x, y ] = c.Red == 0;
								}
							}

							var distanceMap = GetDistanceMap( image );

							opacityImageNearestCellTable = new Vector2I[ image.Size.X, image.Size.Y ];
							for( int y = 0; y < image.Size.Y; y++ )
								for( int x = 0; x < image.Size.X; x++ )
									opacityImageNearestCellTable[ x, y ] = new Vector2I( x, y );

							var table = opacityImageNearestCellTable;

							//!!!!slowly

							Parallel.For( 0, image.Size.Y, delegate ( int y )//for( int y = 0; y < image.Size.Y; y++ )
							{
								for( int x = 0; x < image.Size.X; x++ )
								{
									var transparent = boolOpacityImage[ x, y ];
									if( transparent )
									{
										for( int n = 0; n < distanceMap.Length; n++ )//foreach( var indexItem in distanceMap )
										{
											ref var indexItem = ref distanceMap[ n ];

											var takeFrom = new Vector2I( x, y ) + indexItem.offset;
											if( takeFrom.X >= 0 && takeFrom.X < image.Size.X && takeFrom.Y >= 0 && takeFrom.Y < image.Size.Y )
											{
												var transparent2 = boolOpacityImage[ takeFrom.X, takeFrom.Y ];
												if( !transparent2 )
												{
													table[ x, y ] = takeFrom;
													break;
												}
											}
										}
									}
								}
							} );
						}

					}

					if( channel == MaterialChannel.BaseColor || channel == MaterialChannel.Metallic || channel == MaterialChannel.Roughness || channel == MaterialChannel.Normal )
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

					//check for zero metallic
					var skip = false;
					if( channel == MaterialChannel.Metallic )
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
						if( channel == MaterialChannel.BaseColor || channel == MaterialChannel.Metallic || channel == MaterialChannel.Roughness || channel == MaterialChannel.Normal )
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
				var material = ComponentUtility.CreateComponent<Component_Material>( null, true, false );

				//!!!!
				material.OpacityMaskThreshold = 0.75;

				//use Subsurface material parameters if available
				foreach( var obj in scene.GetComponents<Component_MeshInSpace>( onlyEnabledInHierarchy: true ) )
				{
					var m = obj.ReplaceMaterial.Value;
					if( m != null )
					{
						if( m.ShadingModel.Value == Component_Material.ShadingModelEnum.Subsurface )
						{
							material.ShadingModel = Component_Material.ShadingModelEnum.Subsurface;
							material.SubsurfacePower = m.SubsurfacePower;
							material.SubsurfaceColor = m.SubsurfaceColor;

							break;
						}
					}
				}

				var data = new NewMaterialData();
				data.OpacityTexture = textureFileNames[ 0 ];
				data.BaseColorTexture = textureFileNames[ 1 ];
				data.MetallicTexture = textureFileNames[ 2 ];
				data.RoughnessTexture = textureFileNames[ 3 ];
				data.NormalTexture = textureFileNames[ 4 ];

				MaterialNewObjectCreateShaderGraph( material, data, out var graph );

				var toSelect = new Component[] { material, graph };
				material.EditorDocumentConfiguration = KryptonConfigGenerator.CreateEditorDocumentXmlConfiguration( toSelect, graph );

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

			ScreenNotifications.Show( "Rendering completed successfully." );
		}
	}
}
