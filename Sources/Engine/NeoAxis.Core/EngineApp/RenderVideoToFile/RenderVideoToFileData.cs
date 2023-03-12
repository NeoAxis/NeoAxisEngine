// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

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

		//

#if !DEPLOY

		int skipFrames = 6;
		long remainingFrames;
		AVIWriter writer;
		bool started;

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

			if( SystemSettings.CommandLineParameters.TryGetValue( "-resolution", out var resolution ) )
			{
				try
				{
					data.Resolution = Vector2I.Parse( resolution );
				}
				catch { }
			}
			if( data.Resolution == Vector2I.Zero )
				data.Resolution = EngineApp.platform.GetScreenSize();

			SystemSettings.CommandLineParameters.TryGetValue( "-format", out data.Format );
		}

		void Start()
		{
			if( started )
				return;
			started = true;

			remainingFrames = (long)( (double)Length * FramesPerSecond );

			try
			{
				if( File.Exists( OutputFileName ) )
					File.Delete( OutputFileName );

				writer = new AVIWriter();// "wmv3" );
				writer.FrameRate = FramesPerSecond;
				writer.Codec = Format;

				writer.Open( OutputFileName, Resolution.X, Resolution.Y );
			}
			catch( Exception e )
			{
				Log.Fatal( "RenderVideoToFileData: Start: Rendering video to file failed. " + e.Message );
			}
		}

		public virtual void AddFrame()// Bitmap bitmap )
		{
			//skip first frames
			if( skipFrames > 0 )
			{
				skipFrames--;
				return;
			}

			try
			{
				Start();

				if( remainingFrames > 0 )
				{
					if( writer != null )
					{
						using( var bitmap = new Bitmap( Resolution.X, Resolution.Y, System.Drawing.Imaging.PixelFormat.Format24bppRgb ) )
						{
							using( var g = Graphics.FromImage( bitmap ) )
								g.CopyFromScreen( 0, 0, 0, 0, new Size( Resolution.X, Resolution.Y ) );
							writer.AddFrame( bitmap );
						}
					}

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
				writer?.Close();
				writer = null;
			}
			catch( Exception e )
			{
				Log.Fatal( "RenderVideoToFileData: Close: Rendering video to file failed. " + e.Message );
			}
		}
#else

		public static void Init()
		{
		}

		public virtual void AddFrame()// Bitmap bitmap )
		{
		}

		public virtual void Close()
		{
		}

#endif
	}
}
