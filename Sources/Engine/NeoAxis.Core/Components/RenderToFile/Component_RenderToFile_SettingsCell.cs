// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace NeoAxis.Editor
{
	public class Component_RenderToFile_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonRender;

		//

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

			var destRealFileName = renderToFile.OutputFileName;
			if( string.IsNullOrEmpty( destRealFileName ) )
			{
				var dialog = new SaveFileDialog();
				//dialog.InitialDirectory = Path.GetDirectoryName( RealFileName );
				dialog.FileName = "Render.png";
				//dialog.FileName = RealFileName;
				dialog.Filter = "PNG files (*.png)|*.png";//dialog.Filter = "All files (*.*)|*.*";
				dialog.RestoreDirectory = true;
				if( dialog.ShowDialog() != DialogResult.OK )
					return;

				destRealFileName = dialog.FileName;
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
				texture.CreateSize = resolution;// new Vector2I( size, size );
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

		private void Scene_GetDisplayDevelopmentDataInThisApplicationOverride( Component_Scene sender, ref bool display )
		{
			display = RenderToFile.DisplayDevelopmentData;
		}
	}
}
