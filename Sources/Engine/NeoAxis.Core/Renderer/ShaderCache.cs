// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using SharpBgfx;
using LiteDB;

namespace NeoAxis
{
	static class ShaderCache
	{
		static bool triedToInit;
		static LiteDatabase database;
		static Dictionary<string, string> shaderFileHashes = new Dictionary<string, string>();
		static Dictionary<string, string[]> shaderFileIncludedFiles = new Dictionary<string, string[]>();

		/////////////////////////////////////////

		public class DatabaseItem
		{
			public int Id { get; set; }

			public int KeyIndex { get; set; }
			public string Key { get; set; }
			public byte[] Data { get; set; }
		}

		/////////////////////////////////////////

		static string GetCacheFileName()
		{
			if( Bgfx.GetCurrentBackend() == RendererBackend.Noop )
				return "";

			string folder = PathUtility.Combine( VirtualFileSystem.Directories.Project, @"Caches\ShaderCache" );

			string name = "";
			if( Bgfx.GetCurrentBackend() == RendererBackend.Direct3D11 || Bgfx.GetCurrentBackend() == RendererBackend.Direct3D12 )
				name = "Direct3D11";
			else if( Bgfx.GetCurrentBackend() == RendererBackend.OpenGLES )
				name = "OpenGLES";
			else if( Bgfx.GetCurrentBackend() == RendererBackend.Vulkan )
				name = "Vulkan";
			else
				Log.Fatal( "GpuProgramManager: Shader model is not specified. Bgfx.GetCurrentBackend() == {0}.", Bgfx.GetCurrentBackend() );

			return Path.Combine( folder, name + ".cache" );

			//return Path.Combine( folder, Bgfx.GetCurrentBackend().ToString() + ".cache" );
		}

		static void Init()
		{
			if( !triedToInit )
			{
				triedToInit = true;

				var fileName = GetCacheFileName();
				if( !string.IsNullOrEmpty( fileName ) )
				{
					var folder = Path.GetDirectoryName( fileName );

					try
					{
						//!!!!Android readonly?
						bool readOnly = SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP || SystemSettings.CurrentPlatform == SystemSettings.Platform.Android;

						bool skip = false;
						if( readOnly && !File.Exists( fileName ) )
							skip = true;

						if( !skip )
						{
							if( !Directory.Exists( folder ) )
								Directory.CreateDirectory( folder );

							var options = new LiteDB.FileOptions();
							//in UWP we do not have write access to the application folder
							if( readOnly )
								options.FileMode = LiteDB.FileMode.ReadOnly;

							database = new LiteDatabase( new FileDiskService( fileName, options ) );

							//!!!!
							//in UWP we do not have write access to the application folder
							//even if we set FileMode = LiteDB.FileMode.ReadOnly, EnsureIndex() method writes to disk. it is a LiteDB issue?
							if( !readOnly )
							{
								var collection = database.GetCollection<DatabaseItem>( "items" );
								collection.EnsureIndex( "KeyIndex" );
							}
						}
					}
					catch( Exception e )
					{
						Log.Warning( e.Message );
						return;
					}
				}
			}
		}

		public static void Shutdown()
		{
			if( database != null )
			{
				//!!!!вызывать, если менялось что-то
				//database.Shrink();

				database.Dispose();
				database = null;
			}
		}

		//!!!!
		//public static void Clear()
		//{
		//	ClearShaderFileHashesAndIncludesFilesCache();

		//	//!!!!
		//}

		static string GetKey( ShaderCompiler.ShaderModel shaderModel, ShaderCompiler.ShaderType shaderType, string shaderFile, string varyingFile, ICollection<(string, string)> defines )
		{
			var b = new StringBuilder( 4096 );

			b.Append( shaderModel.ToString() );
			b.Append( '_' );
			b.Append( shaderType.ToString() );
			b.Append( '_' );
			b.Append( shaderFile );
			b.Append( '_' );
			b.Append( varyingFile );
			b.Append( '_' );

			if( defines != null )
			{
				foreach( var defineItem in defines )
				{
					b.Append( '{' );
					b.Append( defineItem.Item1 );
					b.Append( '=' );
					if( defineItem.Item2 != null )
						b.Append( defineItem.Item2 );
					b.Append( '}' );
				}
			}

			//file content hashes
			{
				b.Append( '_' );
				b.Append( GetShaderFileHash( shaderFile ) );
				b.Append( '_' );
				b.Append( GetShaderFileHash( varyingFile ) );

				var included = GetAllIncludedFiles( shaderFile );
				for( int n = 0; n < included.Length; n++ )
				{
					b.Append( '_' );
					b.Append( n.ToString() );
					b.Append( '_' );
					b.Append( GetShaderFileHash( included[ n ] ) );
				}
			}

			//Log.Info( b.ToString() );

			return b.ToString();
		}

		static byte[] Zip( byte[] data )
		{
			using( var memoryStream = new MemoryStream() )
			{
				using( var zipArchive = new ZipArchive( memoryStream, ZipArchiveMode.Create, true ) )
				{
					var file = zipArchive.CreateEntry( "file" );
					using( var entryStream = file.Open() )
						entryStream.Write( data, 0, data.Length );
				}
				return memoryStream.ToArray();
			}
		}

		static byte[] Unzip( byte[] data )
		{
			using( var zippedStream = new MemoryStream( data ) )
			{
				using( var archive = new ZipArchive( zippedStream ) )
				{
					var entry = archive.Entries.FirstOrDefault();
					using( var stream = entry.Open() )
					{
						using( var memoryStream = new MemoryStream() )
						{
							stream.CopyTo( memoryStream );
							return memoryStream.ToArray();
						}
					}
				}
			}
		}

		public static bool GetFromCache( ShaderCompiler.ShaderModel shaderModel, ShaderCompiler.ShaderType shaderType, string shaderFile, string varyingFile, ICollection<(string, string)> defines, out byte[] compiledData )
		{
			Init();
			if( database == null )
			{
				compiledData = null;
				return false;
			}

			var collection = database.GetCollection<DatabaseItem>( "items" );

			var key = GetKey( shaderModel, shaderType, shaderFile, varyingFile, defines );
			var keyIndex = key.GetStableHashCode();

			var items = collection.Find( Query.EQ( "KeyIndex", keyIndex ) );
			foreach( var item in items )
			{
				if( item.Key == key )
				{
					compiledData = Unzip( item.Data );
					return true;
				}
			}

			compiledData = null;
			return false;
		}

		public static void AddToCache( ShaderCompiler.ShaderModel shaderModel, ShaderCompiler.ShaderType shaderType, string shaderFile, string varyingFile, ICollection<(string, string)> defines, byte[] compiledData )
		{
			Init();
			if( database == null )
				return;

			var item = new DatabaseItem();
			item.Key = GetKey( shaderModel, shaderType, shaderFile, varyingFile, defines );
			item.KeyIndex = item.Key.GetStableHashCode();
			item.Data = Zip( compiledData );

			var collection = database.GetCollection<DatabaseItem>( "items" );

			try
			{
				collection.Insert( item );
			}
			catch
			{
			}
		}

		static string GetShaderFileHash( string virtualFileName )
		{
			if( !shaderFileHashes.TryGetValue( virtualFileName, out var hash ) )
			{
				try
				{
					if( VirtualFile.Exists( virtualFileName ) )
					{
						var data = VirtualFile.ReadAllBytes( virtualFileName );

						using( var sha = new SHA256Managed() )
						{
							byte[] checksum = sha.ComputeHash( data );
							var hash2 = BitConverter.ToString( checksum ).Replace( "-", String.Empty );

							hash = data.Length.ToString() + "*" + hash2;
						}
					}
				}
				catch
				{
					hash = "";
				}

				//Log.Info( virtualFileName + " --- " + hash );

				shaderFileHashes[ virtualFileName ] = hash;
			}
			return hash;
		}

		public static void ClearShaderFileHashesAndIncludesFilesCache()
		{
			shaderFileHashes.Clear();
			shaderFileIncludedFiles.Clear();
		}

		static List<string> GetIncludedFilesOnlyInThisFile( string virtualFileName )
		{
			//Log.Info( "--------------------------------" );
			//Log.Info( "FILE: " + virtualFileName );

			var realDirectoryName = VirtualPathUtility.GetRealPathByVirtual( Path.GetDirectoryName( virtualFileName ) );

			var result = new List<string>();

			try
			{
				var lines = VirtualFile.ReadAllText( virtualFileName ).Split( new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries );

				foreach( string line in lines )
				{
					if( !line.Contains( "#include" ) )
						continue;

					int firstQuota = line.IndexOf( '\"' );
					if( firstQuota == -1 )
						firstQuota = line.IndexOf( '<' );
					if( firstQuota == -1 )
						continue;

					int secondQuota = line.IndexOf( '\"', firstQuota + 1 );
					if( secondQuota == -1 )
						secondQuota = line.IndexOf( '>', firstQuota + 1 );
					if( secondQuota == -1 )
						continue;

					var includedFileName = line.Substring( firstQuota + 1, secondQuota - firstQuota - 1 ).Replace( '/', '\\' );

					try
					{
						var path = VirtualPathUtility.GetVirtualPathByReal( Path.GetFullPath( Path.Combine( realDirectoryName, includedFileName ) ) );

						if( VirtualFile.Exists( path ) )
							result.Add( path );

						//Log.Info( path );
					}
					catch
					{
					}
				}
			}
			catch
			{
			}

			return result;
		}

		static string[] GetAllIncludedFiles( string virtualFileName )
		{
			if( !shaderFileIncludedFiles.TryGetValue( virtualFileName, out var includedFiles ) )
			{
				var all = new ESet<string>();
				foreach( var file in GetIncludedFilesOnlyInThisFile( virtualFileName ) )
				{
					all.AddWithCheckAlreadyContained( file );
					all.AddRangeWithCheckAlreadyContained( GetAllIncludedFiles( file ) );
				}
				includedFiles = all.ToArray();

				//Log.Info( "--------------------------------" );
				//Log.Info( "FILE: " + virtualFileName );
				//foreach( var i in includedFiles )
				//	Log.Info( i );

				shaderFileIncludedFiles[ virtualFileName ] = includedFiles;
			}

			return includedFiles;
		}
	}
}