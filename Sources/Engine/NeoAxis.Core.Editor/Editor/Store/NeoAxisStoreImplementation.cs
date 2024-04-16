#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Xml;
using System.Linq;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Downloader;

namespace NeoAxis.Editor
{
	public class NeoAxisStoreImplementation : StoreManager.StoreImplementation
	{
		ThreadItem currentThread;

		///////////////////////////////////////////////

		class ThreadItem
		{
			public Thread thread;
			public bool needStop;
			public string search;
			public StoreManager.FilterSettingsClass filterSettings;
		}

		///////////////////////////////////////////////

		void ThreadGetStoreItems( object threadItem2 )
		{
			ThreadItem threadItem = (ThreadItem)threadItem2;
			var filter = threadItem.filterSettings;

			try
			{
				string xml = "";

				string url = EngineInfo.StoreAddress + @"/api/get_store_items2/";

				//first parameter must start from '?'
				url += "?min_triangle_count=" + filter.MinTriangleCount.ToString();
				if( filter.MaxTriangleCount > 0 )
					url += "&max_triangle_count=" + filter.MaxTriangleCount.ToString();

				if( !string.IsNullOrEmpty( threadItem.search ) )
					url += "&search=" + threadItem.search.Replace( ' ', '+' );

				if( filter.LicenseMIT )
					url += "&license_mit=true";
				if( filter.CCAttribution )
					url += "&license_cc_attribution=true";
				//if( filter.CCAttributionBYSA )
				//	licenses.Add( "by-sa" );
				//if( filter.CCAttributionBYND )
				//	licenses.Add( "by-nd" );
				//if( filter.CCAttributionBYNC )
				//	licenses.Add( "by-nc" );
				//if( filter.CCAttributionBYNCSA )
				//	licenses.Add( "by-nc-sa" );
				//if( filter.CCAttributionBYNCND )
				//	licenses.Add( "by-nc-nd" );
				//if( filter.CC0 )
				//	licenses.Add( "cc0" );

				//!!!!FreeToUse

				if( filter.FreeToUseWithNeoAxis )
					url += "&license_free_to_use_with_neoaxis=true";
				if( filter.PaidPerSeat )
					url += "&license_paid_per_seat=true";

				if( filter.SortBy == StoreManager.FilterSettingsClass.SortByEnum.Latest )
					url += "&sort_by=latest";
				////if( filter.SortBy == FilterSettingsClass.SortByEnum.Relevance )
				////	url += "&sort_by=-relevance";



				var request = (HttpWebRequest)WebRequest.Create( url );

				using( var response = (HttpWebResponse)request.GetResponse() )
				using( var stream = response.GetResponseStream() )
				using( var reader = new StreamReader( stream ) )
					xml = reader.ReadToEnd();

				if( threadItem.needStop || EditorAPI.ClosingApplication )
					return;

				XmlDocument xDoc = new XmlDocument();
				xDoc.LoadXml( xml );

				if( threadItem.needStop || EditorAPI.ClosingApplication )
					return;

				var packages = new List<PackageManager.PackageInfo>();
				//!!!!
				//var featuredItems = "";

				foreach( XmlNode itemNode in xDoc.GetElementsByTagName( "item" ) )
				{
					var info = new PackageManager.PackageInfo();
					info.Store = store;

					//info.Name = "None";
					//info.Version = "1.0.0.0";

					foreach( XmlNode child in itemNode.ChildNodes )
					{
						if( child.Name == "identifier" )
							info.Identifier = child.InnerText;
						else if( child.Name == "title" )
							info.Title = child.InnerText;
						else if( child.Name == "author" )
							info.Author = child.InnerText;
						else if( child.Name == "version" )
							info.Version = child.InnerText;
						else if( child.Name == "size" )
						{
							double.TryParse( child.InnerText, out var value );
							info.Size = (long)value;
						}
						else if( child.Name == "free_download" )
							info.FreeDownload = child.InnerText;
						else if( child.Name == "secure_download" && !string.IsNullOrEmpty( child.InnerText ) )
							info.SecureDownload = (bool)SimpleTypes.ParseValue( typeof( bool ), child.InnerText );
						else if( child.Name == "short_description" )
						{
							info.ShortDescription = child.InnerText.Trim( new char[] { '\n', '\r', ' ', '\t' } );
							//remove tags
							try
							{
								info.ShortDescription = Regex.Replace( info.ShortDescription, "<.*?>", string.Empty );
							}
							catch { }
						}
						else if( child.Name == "full_description" )
							info.FullDescription = child.InnerText;
						else if( child.Name == "permalink" )
							info.Permalink = child.InnerText;
						else if( child.Name == "cost" )
							info.Cost = child.InnerText;
						else if( child.Name == "date" )
							info.Date = child.InnerText;
						else if( child.Name == "files" )
							info.Files = child.InnerText;
						else if( child.Name == "categories" )
							info.Categories = child.InnerText;
						else if( child.Name == "tags" )
							info.Tags = child.InnerText;
						else if( child.Name == "thumbnail" )
							info.Thumbnail = child.InnerText;
						else if( child.Name == "triangles" )
						{
							int.TryParse( child.InnerText, out var value );
							info.Triangles = value;
						}
						else if( child.Name == "vertices" )
						{
							int.TryParse( child.InnerText, out var value );
							info.Vertices = value;
						}
						else if( child.Name == "rigged" && !string.IsNullOrEmpty( child.InnerText ) )
							info.Rigged = (bool)SimpleTypes.ParseValue( typeof( bool ), child.InnerText );
						else if( child.Name == "animations" )
						{
							int.TryParse( child.InnerText, out var value );
							info.Animations = value;
						}
						else if( child.Name == "license" )
						{
							var text = child.InnerText.Replace( " ", "" ).Replace( "-", "" );
							Enum.TryParse<StoreProductLicense>( text, out var value );
							info.License = value;
						}
					}

					////skip for cloud project mode
					//if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient )
					//{
					//	if( info.Identifier == "Platform_Tools" )
					//		continue;
					//}

					packages.Add( info );
				}

				if( threadItem.needStop || EditorAPI.ClosingApplication )
					return;

				////sort by date
				//CollectionUtility.MergeSort( packages, delegate ( PackageManager.PackageInfo p1, PackageManager.PackageInfo p2 )
				//{
				//	if( p1.Date != p2.Date )
				//		return -string.Compare( p1.Date, p2.Date );
				//	return 0;
				//} );

				StoreManager.SetDownloadedListOfPackages( store, packages );
			}
			catch { }
		}

		public override void StartRefreshPackageList( string search, StoreManager.FilterSettingsClass filterSettings )
		{
			StopCurrentTask();

			var thread = new Thread( ThreadGetStoreItems );
			thread.IsBackground = true;
			var threadItem = new ThreadItem() { thread = thread, search = search, filterSettings = filterSettings };
			currentThread = threadItem;

			thread.Start( threadItem );
		}

		public override void StopCurrentTask()
		{
			var item = currentThread;
			if( item != null )
				item.needStop = true;
			currentThread = null;
		}

		public override void ThreadDownloadBody( StoresWindow.ThreadDownloadData data )
		{
			var state = data.State;

			var downloaderOptions = new DownloadConfiguration();
			long chunkSize = 30 * 1024 * 1024;
			if( data.Package.Size > chunkSize )
				downloaderOptions.ChunkCount = (int)Math.Max( data.Package.Size / chunkSize + 1, 1 );

			using( var downloader = new DownloadService( downloaderOptions ) )
			{
				state.downloadingDownloader = downloader;

				var tempFileName = Path.Combine( Path.GetTempPath(), "Temp5" + Path.GetRandomFileName() );
				//var tempFileName = Path.GetTempFileName();

				downloader.DownloadProgressChanged += delegate ( object sender, Downloader.DownloadProgressChangedEventArgs e )
				{
					//check already ended
					if( data.Cancelled )
						return;

					if( e.TotalBytesToReceive != 0 )
						state.downloadProgress = MathEx.Saturate( (float)e.ReceivedBytesSize / (float)e.TotalBytesToReceive );
				};

				downloader.DownloadFileCompleted += delegate ( object sender, AsyncCompletedEventArgs e )
				{
					//check already ended
					if( !data.Cancelled )
					{
						//releases blocked thread
						lock( e.UserState )
							Monitor.Pulse( e.UserState );

						data.Cancelled = e.Cancelled;
						data.Error = e.Error;

						//copy to destination path
						if( !data.Cancelled && data.Error == null )
							File.Copy( tempFileName, state.downloadingDestinationPath, true );
					}

					try
					{
						if( File.Exists( tempFileName ) )
							File.Delete( tempFileName );
					}
					catch { }
				};

				using( var task = downloader.DownloadFileTaskAsync( state.downloadingAddress, tempFileName ) )
				{
					while( !string.IsNullOrEmpty( state.downloadingAddress ) && !task.Wait( 10 ) )
					{
					}
				}

				state.downloadingDownloader = null;
			}

			if( data.Cancelled || data.Error != null )
				return;



			//using( WebClient client = new WebClient() )
			//{
			//	state.downloadingClient = client;

			//	var tempFileName = Path.Combine( Path.GetTempPath(), "Temp5" + Path.GetRandomFileName() );
			//	//var tempFileName = Path.GetTempFileName();

			//	client.DownloadProgressChanged += delegate ( object sender, DownloadProgressChangedEventArgs e )
			//	{
			//		//check already ended
			//		if( data.Cancelled )
			//			return;

			//		if( e.TotalBytesToReceive != 0 )
			//			state.downloadProgress = MathEx.Saturate( (float)e.BytesReceived / (float)e.TotalBytesToReceive );
			//	};

			//	client.DownloadFileCompleted += delegate ( object sender, AsyncCompletedEventArgs e )
			//	{
			//		//check already ended
			//		if( !data.Cancelled )
			//		{
			//			//releases blocked thread
			//			lock( e.UserState )
			//				Monitor.Pulse( e.UserState );

			//			data.Cancelled = e.Cancelled;
			//			data.Error = e.Error;

			//			//copy to destination path
			//			if( !data.Cancelled && data.Error == null )
			//				File.Copy( tempFileName, state.downloadingDestinationPath, true );
			//		}

			//		try
			//		{
			//			if( File.Exists( tempFileName ) )
			//				File.Delete( tempFileName );
			//		}
			//		catch { }
			//	};

			//	using( var task = client.DownloadFileTaskAsync( new Uri( state.downloadingAddress ), tempFileName ) )
			//	{
			//		while( !string.IsNullOrEmpty( state.downloadingAddress ) && !task.Wait( 10 ) )
			//		{
			//		}
			//	}

			//	state.downloadingClient = null;
			//}

			//if( data.Cancelled || data.Error != null )
			//	return;


			//update Package.info in the the archive
			using( var archive = ZipFile.Open( state.downloadingDestinationPath, ZipArchiveMode.Update ) )
			{
				var entry = archive.GetEntry( "Package.info" );
				if( entry != null )
				{
					//read
					string sourceString;
					using( var stream = entry.Open() )
					using( var reader = new StreamReader( stream ) )
						sourceString = reader.ReadToEnd();

					//parse
					var block = TextBlock.Parse( sourceString, out var error );
					if( !string.IsNullOrEmpty( error ) )
						throw new Exception( error );

					//update

					var package = data.Package;

					block.SetAttribute( "Store", store.Name );
					if( !string.IsNullOrEmpty( package.Permalink ) )
						block.SetAttribute( "Permalink", package.Permalink );
					if( !string.IsNullOrEmpty( package.Thumbnail ) )
						block.SetAttribute( "Thumbnail", package.Thumbnail );
					if( package.Triangles != 0 )
						block.SetAttribute( "Triangles", package.Triangles.ToString() );
					if( package.Vertices != 0 )
						block.SetAttribute( "Vertices", package.Vertices.ToString() );
					if( package.Rigged )
						block.SetAttribute( "Rigged", package.Rigged.ToString() );
					if( package.Animations != 0 )
						block.SetAttribute( "Animations", package.Animations.ToString() );

					//public string Cost;
					//public string FreeDownload;
					//public bool SecureDownload;

					//write
					entry.Delete();
					entry = archive.CreateEntry( "Package.info" );
					using( var stream = entry.Open() )
					using( var writer = new StreamWriter( stream ) )
						writer.Write( block.DumpToString() );
				}
			}

		}
	}
}

#endif