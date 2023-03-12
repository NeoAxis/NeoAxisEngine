// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Internal;

namespace NeoAxis
{
	/// <summary>
	/// Class for processing cubemaps.
	/// </summary>
	public static class CubemapProcessing
	{
		static string GetDestFileNameBase( string sourceFileName )
		{
			return sourceFileName + "_Gen";
		}

		static void GetDestFileNames( string destFileNameBase, out string infoFileName, out string envFileName, out string irrFileName )
		{
			infoFileName = destFileNameBase + ".info";
			envFileName = destFileNameBase + "Env.dds";
			irrFileName = destFileNameBase + "Irr.dds";
		}

		static void ReadCachedInfoFile( string destFileNameBase, out string sourceFileHash, out int sourceFileSize, out Vector4F[] irradiance )//, out PixelFormat sourceFileFormat )
		{
			sourceFileHash = "";
			sourceFileSize = 0;
			irradiance = null;
			//sourceFileFormat = PixelFormat.Unknown;

			GetDestFileNames( destFileNameBase, out var infoFileName, out var envFileName, out var irrFileName );

			if( VirtualFile.Exists( envFileName ) && VirtualFile.Exists( irrFileName ) )
			{
				var block = TextBlockUtility.LoadFromVirtualFile( infoFileName, out _ );
				if( block != null )
				{
					try
					{
						sourceFileHash = block.GetAttribute( "SourceFileHash" );
						sourceFileSize = int.Parse( block.GetAttribute( "SourceFileSize" ) );
						//sourceFileFormat = (PixelFormat)Enum.Parse( typeof( PixelFormat ), block.GetAttribute( "SourceFileFormat" ) );

						irradiance = new Vector4F[ 9 ];
						for( int n = 0; n < irradiance.Length; n++ )
							irradiance[ n ] = new Vector4F( Vector3F.Parse( block.GetAttribute( "Irradiance" + n.ToString() ) ), 0 );
					}
					catch { }
				}
			}
		}

		static string GetTemporaryDirectory()
		{
			string tempDirectory = Path.Combine( Path.GetTempPath(), Path.GetRandomFileName() );
			Directory.CreateDirectory( tempDirectory );
			return tempDirectory;
		}

		static void DeleteDirectory( string directoryName )
		{
			DirectoryInfo info = new DirectoryInfo( directoryName );
			foreach( FileInfo file in info.GetFiles() )
				file.Delete();
			foreach( DirectoryInfo directory in info.GetDirectories() )
				directory.Delete( true );
		}

#if !DEPLOY
		static bool GenerateFile( string sourceRealFileName, bool generateIrradiance, int destSize, string destRealFileName, List<Vector3> outIrradianceValues, out string error )
		{
			var tempDirectory = GetTemporaryDirectory();

			string arguments;
			if( generateIrradiance )
				arguments = $"--format=hdr --size={destSize} --type=cubemap --ibl-irradiance=\"{tempDirectory}\" \"{sourceRealFileName}\"";
			else
				arguments = $"--format=hdr --size={destSize} --type=cubemap -x \"{tempDirectory}\" \"{sourceRealFileName}\"";

			var process = new Process();
			process.StartInfo.FileName = Path.Combine( VirtualFileSystem.Directories.EngineInternal, @"Tools\Filament\cmgen.exe" );
			process.StartInfo.Arguments = arguments;
			process.Start();
			process.WaitForExit();

			var exitCode = process.ExitCode;
			if( exitCode != 0 )
			{
				error = $"cmgen.exe exit code = {exitCode}.";
				return false;
			}

			var folder = Path.Combine( tempDirectory, Path.GetFileNameWithoutExtension( sourceRealFileName ) );

			int size;
			bool need16bit = false;
			{
				var file = Path.Combine( folder, generateIrradiance ? "i_nx.hdr" : "m0_nx.hdr" );
				var bytes = File.ReadAllBytes( file );
				if( !ImageUtility.LoadFromBuffer( bytes, "hdr", out var data, out var size2, out _, out var format, out _, out _, out error ) )
					return false;
				size = size2.X;
				if( format != PixelFormat.Float32RGB )
				{
					error = "format != PixelFormat.Float32RGB";
					return false;
				}

				var image2D = new ImageUtility.Image2D( format, size2, data );
				for( int y = 0; y < image2D.Size.Y; y++ )
				{
					for( int x = 0; x < image2D.Size.X; x++ )
					{
						var v = image2D.GetPixel( new Vector2I( x, y ) );
						if( v.X > 1.001f || v.Y >= 1.001f || v.Z >= 1.001f )
						{
							need16bit = true;
							goto end16bit;
						}
					}
				}
				end16bit:;
			}

			var surfaces = new List<DDSTextureTools.DDSImage.Surface>();

			for( int face = 0; face < 6; face++ )
			{
				int counter = 0;
				int currentSize = size;
				while( currentSize > 0 )
				{
					var postfixes = new string[] { "px", "nx", "py", "ny", "pz", "nz" };

					string file;
					if( generateIrradiance )
						file = Path.Combine( folder, $"i_{postfixes[ face ]}.hdr" );
					else
						file = Path.Combine( folder, $"m{counter}_{postfixes[ face ]}.hdr" );

					var bytes = File.ReadAllBytes( file );
					if( !ImageUtility.LoadFromBuffer( bytes, "hdr", out var data, out var size2, out _, out var format, out _, out _, out error ) )
						return false;
					if( format != PixelFormat.Float32RGB )
					{
						error = "format != PixelFormat.Float32RGB";
						return false;
					}
					if( size2.X != currentSize )
					{
						error = "size2.X != currentSize";
						return false;
					}

					if( need16bit )
					{
						byte[] newData = new byte[ currentSize * currentSize * 4 * 2 ];

						unsafe
						{
							fixed( byte* pData = data )
							{
								float* pData2 = (float*)pData;

								fixed( byte* pNewData = newData )
								{
									HalfType* pNewData2 = (HalfType*)pNewData;

									for( int n = 0; n < currentSize * currentSize; n++ )
									{
										pNewData2[ n * 4 + 0 ] = new HalfType( pData2[ n * 3 + 0 ] );
										pNewData2[ n * 4 + 1 ] = new HalfType( pData2[ n * 3 + 1 ] );
										pNewData2[ n * 4 + 2 ] = new HalfType( pData2[ n * 3 + 2 ] );
										pNewData2[ n * 4 + 3 ] = new HalfType( 1.0f );
									}
								}
							}
						}

						surfaces.Add( new DDSTextureTools.DDSImage.Surface( new Vector2I( currentSize, currentSize ), newData ) );
					}
					else
					{
						byte[] newData = new byte[ currentSize * currentSize * 4 ];

						unsafe
						{
							fixed( byte* pData = data )
							{
								float* pData2 = (float*)pData;

								for( int n = 0; n < currentSize * currentSize; n++ )
								{
									newData[ n * 4 + 3 ] = 255;
									newData[ n * 4 + 2 ] = (byte)( MathEx.Saturate( pData2[ n * 3 + 0 ] ) * 255.0 );
									newData[ n * 4 + 1 ] = (byte)( MathEx.Saturate( pData2[ n * 3 + 1 ] ) * 255.0 );
									newData[ n * 4 + 0 ] = (byte)( MathEx.Saturate( pData2[ n * 3 + 2 ] ) * 255.0 );
								}
							}
						}

						surfaces.Add( new DDSTextureTools.DDSImage.Surface( new Vector2I( currentSize, currentSize ), newData ) );
					}

					counter++;
					currentSize /= 2;

					if( generateIrradiance )
						break;
				}
			}

			var image = new DDSTextureTools.DDSImage( need16bit ? DDSTextureTools.DDSImage.FormatEnum.R16G16B16A16 : DDSTextureTools.DDSImage.FormatEnum.X8R8G8B8, surfaces.ToArray(), true );
			if( !DDSTextureTools.WriteToFile( destRealFileName, image, out error ) )
				return false;

			if( outIrradianceValues != null )
			{
				var shFile = Path.Combine( folder, "sh.txt" );
				var lines = File.ReadAllLines( shFile );
				foreach( var line in lines )
				{
					var index1 = line.IndexOf( '(' );
					var index2 = line.IndexOf( ')' );
					if( index1 != -1 && index2 != -1 )
					{
						var str = line.Substring( index1 + 1, index2 - index1 - 1 ).Trim();

						var strs = str.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
						if( strs.Length != 3 )
						{
							error = "Unable to parse \"sh.txt\".";
							return false;
						}

						var x = double.Parse( strs[ 0 ].Trim() );
						var y = double.Parse( strs[ 1 ].Trim() );
						var z = double.Parse( strs[ 2 ].Trim() );

						outIrradianceValues.Add( new Vector3( x, y, z ) );
					}
				}
			}

			DeleteDirectory( tempDirectory );

			error = "";
			return true;
		}

		static bool Generate( string sourceFileName, string sourceFileHash, int sourceFileSize, int specifiedSize, out string error )
		{
			var text = $"Generating cubemap \"{Path.GetFileName( sourceFileName )}\"...";
			var notification = Editor.ScreenNotifications.ShowSticky( text );

			var destFileNameBase = GetDestFileNameBase( sourceFileName );
			GetDestFileNames( destFileNameBase, out var infoFileName, out var envFileName, out var irrFileName );

			string temporaryGeneratedFile = "";

			try
			{
				var sourceRealFileName = VirtualPathUtility.GetRealPathByVirtual( sourceFileName );
				{
					//convert .image to 6 face HDR image

					if( Path.GetExtension( sourceRealFileName ) == ".image" )
					{
						var image = ResourceManager.LoadResource<ImageComponent>( sourceFileName, out error );
						if( image == null )
							return false;

						string[] loadCubeMap6Files = null;
						string loadFromOneFile = null;
						{
							if( image.AnyCubemapSideIsSpecified() )
							{
								if( image.AllCubemapSidesAreaSpecified() )
								{
									loadCubeMap6Files = new string[ 6 ];
									for( int n = 0; n < 6; n++ )
									{
										var v = image.GetLoadCubeSide( n );
										if( v != null )
											loadCubeMap6Files[ n ] = v.ResourceName;

										if( string.IsNullOrEmpty( loadCubeMap6Files[ n ] ) || !VirtualFile.Exists( loadCubeMap6Files[ n ] ) )
										{
											loadCubeMap6Files = null;
											break;
										}
									}
								}
							}
							else
							{
								var v = image.LoadFile.Value;
								loadFromOneFile = v != null ? v.ResourceName : "";
								if( string.IsNullOrEmpty( loadFromOneFile ) || !VirtualFile.Exists( loadFromOneFile ) )
									loadFromOneFile = null;
							}
						}

						if( loadFromOneFile != null )
							sourceRealFileName = VirtualPathUtility.GetRealPathByVirtual( loadFromOneFile );
						else if( loadCubeMap6Files != null )
						{
							temporaryGeneratedFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".hdr";
							sourceRealFileName = temporaryGeneratedFile;

							ImageUtility.Image2D image2D = null;

							for( int face = 0; face < 6; face++ )
							{
								if( !ImageUtility.LoadFromVirtualFile( loadCubeMap6Files[ face ], out var data, out var size, out _, out var format, out _, out _, out error ) )
									return false;

								if( image2D == null )
									image2D = new ImageUtility.Image2D( PixelFormat.Float32RGB, size * new Vector2I( 4, 3 ) );

								Vector2I index = Vector2I.Zero;
								switch( face )
								{
								case 0: index = new Vector2I( 2, 1 ); break;
								case 1: index = new Vector2I( 0, 1 ); break;
								case 2: index = new Vector2I( 1, 0 ); break;
								case 3: index = new Vector2I( 1, 2 ); break;
								case 4: index = new Vector2I( 1, 1 ); break;
								case 5: index = new Vector2I( 3, 1 ); break;
								}

								var faceImage = new ImageUtility.Image2D( format, size, data );
								image2D.Blit( index * size, faceImage );
							}

							if( !ImageUtility.Save( temporaryGeneratedFile, image2D.Data, image2D.Size, 1, image2D.Format, 1, 0, out error ) )
								return false;
						}
						else
						{
							error = "The image wrong configured.";
							return false;
						}
					}
				}

				int sizeEnv = 32;
				if( specifiedSize == 0 )
				{
					//detect size

					if( !ImageUtility.LoadFromRealFile( sourceRealFileName, out _, out var sourceSize, out _, out _, out _, out _, out error ) )
						return false;

					//!!!!если кубемапа

					if( Math.Abs( (double)sourceSize.X / (double)sourceSize.Y - 1.333333 ) <= 0.001 )
					{
						sizeEnv = sourceSize.X / 4;
					}
					else
					{
						//!!!!
						sizeEnv = MathEx.NextPowerOfTwo( sourceSize.X / 3 );
					}

					if( sizeEnv > 512 )
						sizeEnv = 512;

					if( sizeEnv == 0 )
						sizeEnv = 1;
				}
				else
					sizeEnv = specifiedSize;

				int sizeIrr = 32;

				var destRealFileNameBase = VirtualPathUtility.GetRealPathByVirtual( destFileNameBase );
				var infoRealFileName = VirtualPathUtility.GetRealPathByVirtual( infoFileName );
				var envRealFileName = VirtualPathUtility.GetRealPathByVirtual( envFileName );
				var irrRealFileName = VirtualPathUtility.GetRealPathByVirtual( irrFileName );

				//delete old files
				if( File.Exists( infoRealFileName ) )
					File.Delete( infoRealFileName );
				if( File.Exists( envRealFileName ) )
					File.Delete( envRealFileName );
				if( File.Exists( irrRealFileName ) )
					File.Delete( irrRealFileName );

				var destDirectory = Path.GetDirectoryName( destRealFileNameBase );
				if( !Directory.Exists( destDirectory ) )
					Directory.CreateDirectory( destDirectory );

				var irradianceValues = new List<Vector3>();

				if( !GenerateFile( sourceRealFileName, false, sizeEnv, envRealFileName, irradianceValues, out error ) )
					return false;
				if( !GenerateFile( sourceRealFileName, true, sizeIrr, irrRealFileName, null, out error ) )
					return false;

				//make .info file
				{
					var block = new TextBlock();
					block.SetAttribute( "SourceFileHash", sourceFileHash );
					block.SetAttribute( "SourceFileSize", sourceFileSize.ToString() );
					//block.SetAttribute( "SourceFileFormat", sourceFileFormat.ToString() );

					for( int n = 0; n < irradianceValues.Count; n++ )
						block.SetAttribute( "Irradiance" + n.ToString(), irradianceValues[ n ].ToString() );

					if( !TextBlockUtility.SaveToRealFile( block, infoRealFileName, out error ) )
						return false;
				}
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}
			finally
			{
				notification.Close();

				try
				{
					if( File.Exists( temporaryGeneratedFile ) )
						File.Delete( temporaryGeneratedFile );
				}
				catch { }
			}

			error = "";
			return true;
		}
#endif

		static bool GetInfoFromSourceFile( string sourceFileName, out string hash, out int sourceFileSize )
		{
			try
			{
				var data = VirtualFile.ReadAllBytes( sourceFileName );
				//!!!!optimization: faster method maybe
				//!!!!optimization: может один раз создавать для всего?
				using( var sha = new SHA256Managed() )
				{
					byte[] checksum = sha.ComputeHash( data );
					hash = BitConverter.ToString( checksum ).Replace( "-", string.Empty );
				}

				sourceFileSize = (int)VirtualFile.GetLength( sourceFileName );
				return true;
			}
			catch
			{
				hash = "";
				sourceFileSize = 0;
				return false;
			}
		}

		public static bool GetOrGenerate( string sourceFileName, bool forceUpdate, int specifiedSize, out string envFileName, out Vector4F[] irradiance /*, out string irrFileName*/, out string error )
		{
			EngineThreading.CheckMainThread();

			var destFileNameBase = GetDestFileNameBase( sourceFileName );
			GetDestFileNames( destFileNameBase, out _, out envFileName, out _ );
			//GetDestFileNames( destFileNameBase, out var infoFileName, out envFileName, out irrFileName );

			if( !GetInfoFromSourceFile( sourceFileName, out var sourceFileHash, out var sourceFileSize ) )
			{
				irradiance = null;
				error = "Unable to get hash from source file.";
				return false;
			}

			ReadCachedInfoFile( destFileNameBase, out var cacheSourceFileHash, out var cacheSourceFileSize, out irradiance );//, out var cacheSourceFileFormat );

#if !DEPLOY
			bool needUpdate = sourceFileHash != cacheSourceFileHash || sourceFileSize != cacheSourceFileSize;
			if( needUpdate || forceUpdate )
				return Generate( sourceFileName, sourceFileHash, sourceFileSize, specifiedSize, out error );
			error = "";
			return true;
#else
			error = "The current platform is not support cubemap processing.";
			return false;
#endif
		}
	}
}
