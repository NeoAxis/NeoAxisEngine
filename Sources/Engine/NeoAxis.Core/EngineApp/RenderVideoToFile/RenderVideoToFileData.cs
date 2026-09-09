// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace NeoAxis
{
	public class RenderVideoToFileData
	{
		public string OutputFileName = "";
		public Vector2I Resolution;
		public int FramesPerSecond;
		public double Length;
		public string Camera = "";
		public string RenderingPipeline = "";
		public string Format = "";
		public string Quality = "";

		//

		int skipFrames = 2;
		long remainingFrames;
		Process ffmpegProcess;
		Stream processInputStream;
		bool started;
		internal ImageComponent textureRead;

		//

		public static void Init()
		{
			if( !SystemSettings.CommandLineParameters.TryGetValue( "-renderVideoToFile", out var outputFileName ) )
				return;

			if( EngineApp.RenderVideoToFileData == null )
				EngineApp.RenderVideoToFileData = new RenderVideoToFileData();

			var data = EngineApp.RenderVideoToFileData;
			data.OutputFileName = outputFileName;

			SystemSettings.CommandLineParameters.TryGetValue( "-framesPerSecond", out var framesPerSecond );
			int.TryParse( framesPerSecond, out data.FramesPerSecond );
			if( data.FramesPerSecond == 0 )
				data.FramesPerSecond = 60;

			SystemSettings.CommandLineParameters.TryGetValue( "-length", out var length );
			double.TryParse( length, out data.Length );

			SystemSettings.CommandLineParameters.TryGetValue( "-camera", out data.Camera );
			SystemSettings.CommandLineParameters.TryGetValue( "-renderingPipeline", out data.RenderingPipeline );

			//if( SystemSettings.CommandLineParameters.TryGetValue( "-resolution", out var resolution ) )
			//{
			//	try
			//	{
			//		data.Resolution = Vector2I.Parse( resolution );
			//	}
			//	catch { }
			//}
			//if( data.Resolution == Vector2I.Zero )
			//	data.Resolution = EngineApp.platform.GetScreenSize();

			SystemSettings.CommandLineParameters.TryGetValue( "-format", out data.Format );
			if( string.IsNullOrEmpty( data.Format ) )
				data.Format = "NoCompression";

			SystemSettings.CommandLineParameters.TryGetValue( "-quality", out data.Quality );
			if( string.IsNullOrEmpty( data.Quality ) )
				data.Quality = "High";
		}

		void Start( ImageUtility.Image2D imageRGBA )
		{
			if( started )
				return;
			started = true;

			remainingFrames = (long)( Length * FramesPerSecond );

			try
			{
				if( File.Exists( OutputFileName ) )
					File.Delete( OutputFileName );

				var ffmpegPath = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, "ffmpeg.exe" );
				if( !File.Exists( ffmpegPath ) )
					throw new Exception( $"FFmpeg executable not found at path: {ffmpegPath}" );

				var outputFormat = "";
				switch( Format.ToLower() )
				{
				case "nocompression":
					outputFormat = "rawvideo -pix_fmt bgr24";
					break;
				case "h264":
					outputFormat = "libx264 -pix_fmt yuv420p -movflags +faststart";
					break;
				case "h265":
					outputFormat = "libx265 -pix_fmt yuv420p -movflags +faststart";
					break;
				case "vp9":
					outputFormat = "libvpx-vp9 -pix_fmt yuv420p -movflags +faststart";
					break;
				}

				var qualityString = "";
				switch( Quality.ToLower() )
				{
				case "low":
					qualityString = "-crf 28";
					break;
				case "medium":
					qualityString = "-crf 23";
					break;
				case "high":
					qualityString = "-crf 18";
					break;
				case "ultra":
					qualityString = "-crf 14";
					break;
				}

				if( Resolution == Vector2I.Zero )
					Resolution = imageRGBA.Size;

				var resolution = Resolution;

				var startInfo = new ProcessStartInfo
				{
					FileName = ffmpegPath,
					Arguments = $"-y -f rawvideo -pix_fmt bgr24 -video_size {resolution.X}x{resolution.Y} -framerate {FramesPerSecond} -i - -c:v {outputFormat} {qualityString} -vf \"scale=iw:-2\" \"{OutputFileName}\"",
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				};

				ffmpegProcess = new Process { StartInfo = startInfo };

				ffmpegProcess.ErrorDataReceived += ( _, e ) =>
				{
					if( !string.IsNullOrWhiteSpace( e.Data ) )
						Log.InvisibleInfo( "FFmpeg: " + e.Data );
				};

				ffmpegProcess.Start();
				ffmpegProcess.BeginErrorReadLine();
				processInputStream = ffmpegProcess.StandardInput.BaseStream; 
			}
			catch( Exception e )
			{
				Log.Fatal( "RenderVideoToFileData: Start: Rendering video to file failed. " + e.Message );
			}
		}

		public virtual void AddFrame( ImageUtility.Image2D imageRGBA )
		{
			//skip first frames
			if( skipFrames > 0 )
			{
				skipFrames--;
				return;
			}

			try
			{
				Start( imageRGBA );

				if( remainingFrames > 0 && ffmpegProcess != null )
				{
					var imageRGB = new ImageUtility.Image2D( PixelFormat.R8G8B8, imageRGBA.Size );
					for( int y = 0; y < imageRGBA.Size.Y; y++ )
					{
						for( int x = 0; x < imageRGBA.Size.X; x++ )
						{
							var color = imageRGBA.GetPixelByte( new Vector2I( x, y ) );
							imageRGB.SetPixel( new Vector2I( x, y ), new ColorByte( color.Red, color.Green, color.Blue ) );
						}
					}

					if( imageRGBA.Size != Resolution )
						throw new Exception( $"Image size does not match the specified resolution. Expected: {Resolution.X}x{Resolution.Y}, Actual: {imageRGBA.Size.X}x{imageRGBA.Size.Y}" );

					var frame = imageRGB.Data;
					processInputStream.Write( frame, 0, frame.Length );
					processInputStream.Flush();

					remainingFrames--;
					if( remainingFrames <= 0 )
						EngineApp.NeedExit = true;
				}
			}
			catch( Exception e )
			{
				Log.Fatal( "RenderVideoToFileData: AddFrame: Rendering video to file failed. " + e.Message );
			}
		}

		public virtual void Close()
		{
			try
			{
				if( ffmpegProcess != null )
				{
					processInputStream?.Dispose();
					processInputStream = null;

					ffmpegProcess.WaitForExit();
					var exitCode = ffmpegProcess.ExitCode;

					ffmpegProcess.Dispose();
					ffmpegProcess = null;

					if( exitCode != 0 )
						throw new InvalidOperationException( $"ffmpeg exited with code {exitCode}." );
				}
			}
			catch( Exception e )
			{
				Log.Fatal( "RenderVideoToFileData: Close: Rendering video to file failed. " + e.Message );
			}
		}
	}
}
