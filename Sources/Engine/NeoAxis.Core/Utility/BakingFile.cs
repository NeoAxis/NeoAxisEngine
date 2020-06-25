// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace NeoAxis
{
	/// <summary>
	/// Implements creating NeoAxis Baking files.
	/// </summary>
	static class BakingFile
	{
		public static bool Create( IList<string> paths, bool compressArchive, string destinationFileName, out string error )
		{
			error = "";

			try
			{
				var destinationFolder = Path.GetDirectoryName( destinationFileName );
				var compressionLevel = compressArchive ? CompressionLevel.Optimal : CompressionLevel.NoCompression;

				using( var archive = ZipFile.Open( destinationFileName, ZipArchiveMode.Create ) )
				{
					foreach( var path in paths )
					{
						if( Directory.Exists( path ) )
						{
							foreach( var file in Directory.GetFiles( path, "*.*", SearchOption.AllDirectories ) )
							{
								//read
								var bytes = File.ReadAllBytes( file );

								//encode
								for( int n = 0; n < bytes.Length; n++ )
									bytes[ n ] = (byte)~bytes[ n ];

								//write
								var fileName = file.Substring( destinationFolder.Length + 1 );
								var entry = archive.CreateEntry( fileName, compressionLevel );
								using( var stream = entry.Open() )
									stream.Write( bytes, 0, bytes.Length );
							}
						}
						else if( File.Exists( path ) )
						{
							//read
							var bytes = File.ReadAllBytes( path );

							//encode
							for( int n = 0; n < bytes.Length; n++ )
								bytes[ n ] = (byte)~bytes[ n ];

							//write
							var fileName = path.Substring( destinationFolder.Length + 1 );
							var entry = archive.CreateEntry( fileName, compressionLevel );
							using( var stream = entry.Open() )
								stream.Write( bytes, 0, bytes.Length );
						}
					}
				}
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
		}
	}
}
