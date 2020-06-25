//!!!!!!

//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Diagnostics;
//using System.IO;
//using ICSharpCode.SharpZipLib.Zip;

//namespace NeoAxis
//{
//	class ZipPackage : Package
//	{
//		ZipFile zipFile;

//		//

//		public ZipPackage( string realFileName, bool loadInfoOnly, out string error )
//			: base( realFileName, loadInfoOnly, out error )
//		{
//			try
//			{
//				//Mono runtime specific
//				if( SystemSettings.NetRuntime == SystemSettings.NetRuntimeType.Mono )
//					ZipConstants.DefaultCodePage = 0;
//				zipFile = new ZipFile( realFileName );
//				error = "";
//			}
//			catch( Exception e )
//			{
//				error = string.Format( "ZipPackage: Loading zip file \'{0}\' failed. Error: {1}.", realFileName, e.Message );
//				return;
//			}

//			//!!!!!!config file
//			Info.LoadAtStartup = true;
//			Info.LoadingPriority = .5f;
//		}

//		public override void Dispose()
//		{
//			if( zipFile != null )
//			{
//				zipFile.Close();
//				zipFile = null;
//			}

//			base.Dispose();
//		}

//		protected internal override void OnGetDirectoryAndFileList( out string[] directories, out FileInfo[] files )
//		{
//			List<string> directoryNames = new List<string>();
//			List<FileInfo> fileInfos = new List<FileInfo>();

//			foreach( ZipEntry entry in zipFile )
//			{
//				if( entry.IsDirectory )
//					directoryNames.Add( entry.Name );
//				else if( entry.IsFile )
//					fileInfos.Add( new FileInfo( entry.Name, entry.Size ) );
//			}

//			directories = directoryNames.ToArray();
//			files = fileInfos.ToArray();
//		}

//		protected internal override VirtualFileStream OnFileOpen( string inArchiveFileName )
//		{
//			lock ( zipFile )
//			{
//				ZipEntry entry = zipFile.GetEntry( inArchiveFileName );
//				Stream zipStream = zipFile.GetInputStream( entry );

//				byte[] buffer = new byte[ entry.Size ];
//				int readed = zipStream.Read( buffer, 0, (int)entry.Size );
//				if( readed != buffer.Length )
//					throw new Exception( "ZipArchive: Reading stream failed." );

//				return new MemoryVirtualFileStream( buffer );
//			}
//		}
//	}
//}
