// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis.Editor;
using Newtonsoft.Json;

namespace NeoAxis
{
	public class SketchfabStoreImplementation : StoreManager.StoreImplementation
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

		public class GetAccessTokenJson
		{
			public string access_token { get; set; }
			public int expires_in { get; set; }
			public string token_type { get; set; }
			public string scope { get; set; }
			public string refresh_token { get; set; }

			public string error { get; set; }
			public string error_description { get; set; }
		}

		///////////////////////////////////////////////

		public class DownloadLinkJson
		{
			public class Gltf
			{
				public string url { get; set; }
				public int size { get; set; }
				public int expires { get; set; }
			}

			public class Usdz
			{
				public string url { get; set; }
				public int size { get; set; }
				public int expires { get; set; }
			}

			public class Root
			{
				public Gltf gltf { get; set; }
				public Usdz usdz { get; set; }
			}
		}

		///////////////////////////////////////////////

		void ThreadGetStoreItems( object threadItem2 )
		{
			ThreadItem threadItem = (ThreadItem)threadItem2;
			var filter = threadItem.filterSettings;

			try
			{
				var packages = new List<PackageManager.PackageInfo>();

				var url = "https://api.sketchfab.com/v3/search?type=models&downloadable=true";


				//!!!!category


				var licenses = new List<string>();

				if( filter.CCAttribution )
					licenses.Add( "by" );
				if( filter.CCAttributionBYSA )
					licenses.Add( "by-sa" );
				if( filter.CCAttributionBYND )
					licenses.Add( "by-nd" );
				if( filter.CCAttributionBYNC )
					licenses.Add( "by-nc" );
				if( filter.CCAttributionBYNCSA )
					licenses.Add( "by-nc-sa" );
				if( filter.CCAttributionBYNCND )
					licenses.Add( "by-nc-nd" );
				if( filter.CC0 )
					licenses.Add( "cc0" );
				//!!!!
				//if( filter.PaidThirdParty )
				//{
				//licenses.Add( "783b685da9bf457d81e829fa283f3567" );
				//licenses.Add( "5b54cf13b1a4422ca439696eb152070d" );
				//}

				foreach( var license in licenses )
					url += "&licenses=" + license;


				if( filter.SortBy == StoreManager.FilterSettingsClass.SortByEnum.Latest )
					url += "&sort_by=-publishedAt";
				//if( filter.SortBy == StoreManager.FilterSettingsClass.SortByEnum.Relevance )
				//	url += "&sort_by=-relevance";

				if( !string.IsNullOrEmpty( threadItem.search ) )
					url += "&q=" + threadItem.search.Replace( ' ', '+' );

				url += "&min_face_count=" + filter.MinTriangleCount.ToString();
				if( filter.MaxTriangleCount > 0 )
					url += "&max_face_count=" + filter.MaxTriangleCount.ToString();

				if( filter.StaffPicks )
					url += "&staffpicked=true";
				if( filter.Animated )
					url += "&animated=true";
				if( filter.ShowRestricted )
					url += "&restricted=true";


				goNext:;

				var jsonString = "";

				var request = (HttpWebRequest)WebRequest.Create( url );

				using( var response = (HttpWebResponse)request.GetResponse() )
				using( var stream = response.GetResponseStream() )
				using( var reader = new StreamReader( stream ) )
					jsonString = reader.ReadToEnd();

				if( threadItem.needStop || EditorAPI.ClosingApplication )
					return;

				var root = JsonConvert.DeserializeObject<SearchJson.Root>( jsonString );

				foreach( var item in root.results )
				{
					var info = new PackageManager.PackageInfo();
					info.Store = store;

					var id = item.name;
					foreach( char c in new string( Path.GetInvalidFileNameChars() ) + new string( Path.GetInvalidPathChars() ) )
						id = id.Replace( c.ToString(), "_" );
					id = id.Replace( " ", "_" ).Replace( "-", "_" );
					if( id.Length > 30 )
						id = id.Substring( 0, 30 );
					id += "_" + item.uid;
					info.Identifier = id;

					info.Title = item.name;
					info.Version = "1.0.0.0";
					info.Author = item.user != null ? item.user.username : "Unknown";

					var thumbnails = item.thumbnails?.images;
					if( thumbnails != null && thumbnails.Count > 0 )
						info.Thumbnail = thumbnails[ 0 ].url;

					info.Permalink = item.viewerUrl;
					info.Triangles = item.faceCount;
					info.Vertices = item.vertexCount;
					info.Animations = item.animationCount;
					//!!!!remove links
					info.ShortDescription = item.description.Trim( new char[] { '\n', '\r', ' ', '\t' } );

					//!!!!
					info.Categories = "Uncategorized Models";
					//info.Categories = "Models";

					if( item.isDownloadable )
					{
						var archive = item?.archives?.gltf;
						if( archive != null )
						{
							info.Size = archive.size;
							info.FreeDownload = item.uri;
						}
					}


					//configure Files to enable autoloading when Drag & Drop
					{
						var virtualDestinationFolder = GetVirtualDestinationFolder( info );
						var sceneFileName = Path.Combine( virtualDestinationFolder, "scene.gltf" );
						info.Files = Path.Combine( "Assets", sceneFileName );
					}


					//!!!!
					//info.License = Component_StoreProduct.LicenseEnum.None;

					//!!!!
					//Date

					//!!!!
					//Tags

					//!!!!
					//public string viewerUrl { get; set; }
					//public List<Tag> tags { get; set; }
					//public User user { get; set; }
					//public DateTime createdAt { get; set; }


					//!!!!
					//foreach( XmlNode child in itemNode.ChildNodes )
					//{
					//	else if( child.Name == "secure_download" && !string.IsNullOrEmpty( child.InnerText ) )
					//		info.SecureDownload = (bool)SimpleTypes.ParseValue( typeof( bool ), child.InnerText );
					//	else if( child.Name == "cost" )
					//		info.Cost = child.InnerText;
					//	else if( child.Name == "date" )
					//		info.Date = child.InnerText;
					//	else if( child.Name == "files" )
					//		info.Files = child.InnerText;
					//	else if( child.Name == "rigged" && !string.IsNullOrEmpty( child.InnerText ) )
					//		info.Rigged = (bool)SimpleTypes.ParseValue( typeof( bool ), child.InnerText );
					//}

					packages.Add( info );
				}

				////sort by date
				//CollectionUtility.MergeSort( packages, delegate ( PackageManager.PackageInfo p1, PackageManager.PackageInfo p2 )
				//{
				//	if( p1.Date != p2.Date )
				//		return -string.Compare( p1.Date, p2.Date );
				//	return 0;
				//} );

				StoreManager.SetDownloadedListOfPackages( store, packages );
				if( threadItem.needStop )
					return;

				Thread.Sleep( 1000 );
				if( threadItem.needStop || EditorAPI.ClosingApplication )
					return;


				//next

				checkNext:;

				bool needGetNext;
				lock( StoreManager.needGetNextItemsForStores )
					needGetNext = StoreManager.needGetNextItemsForStores.Remove( store );

				//sleep
				if( !needGetNext )
				{
					Thread.Sleep( 1000 );
					if( threadItem.needStop || EditorAPI.ClosingApplication )
						return;
					goto checkNext;
				}

				//check to go next
				if( !string.IsNullOrEmpty( root.next ) )
				{
					url = root.next;
					goto goNext;
				}

			}
			catch { }
		}

		public override void StartRefreshPackageList( string search, StoreManager.FilterSettingsClass filterSettings )
		{
			StopCurrentTask();

			var thread = new Thread( ThreadGetStoreItems );
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

		async Task<string> GetAccessTokenResponseAsync()
		{
			var CLIENT_ID = "iCfsSDtpdY2nNQDsv2pyWEBgKrKwlK58XWRZhKmE";

			if( !SketchfabLogin.LoadFromRegistry( out var username, out var password ) || string.IsNullOrEmpty( username ) || string.IsNullOrEmpty( password ) )
			{
				StoresWindow.needOpenOptions = true;

				throw new Exception( "Sketchfab: Need to enter login information in the options of the Stores window." );
			}

			var client = new HttpClient();
			client.BaseAddress = new Uri( "https://sketchfab.com" );

			var request = new HttpRequestMessage( HttpMethod.Post, "/oauth2/token/?grant_type=password&client_id=" + CLIENT_ID );

			var keyValues = new List<KeyValuePair<string, string>>();
			keyValues.Add( new KeyValuePair<string, string>( "username", username ) );
			keyValues.Add( new KeyValuePair<string, string>( "password", password ) );

			request.Content = new FormUrlEncodedContent( keyValues );

			var response = await client.SendAsync( request );
			var result = await response.Content.ReadAsStringAsync();

			return result;
		}

		string GetAccessToken()
		{
			var task = GetAccessTokenResponseAsync();
			task.Wait();
			var responseString = task.Result;

			var data = JsonConvert.DeserializeObject<GetAccessTokenJson>( responseString );

			if( !string.IsNullOrEmpty( data.error ) )
				throw new Exception( "Sketchfab: " + ( !string.IsNullOrEmpty( data.error_description ) ? data.error_description : data.error ) );

			return data.access_token;
		}

		async Task<string> RequestDownloadLinkAsync( PackageManager.PackageInfo package, string accessToken )
		{
			var id = package.Identifier;

			var sketchfabUID = id;
			{
				var index = id.LastIndexOf( '_' );
				if( index != -1 )
					sketchfabUID = id.Substring( index + 1 );
			}

			var client = new HttpClient();
			client.BaseAddress = new Uri( "https://api.sketchfab.com" );

			var request = new HttpRequestMessage( HttpMethod.Get, $"/v3/models/{sketchfabUID}/download" );
			request.Headers.Add( "Authorization", "Bearer " + accessToken );

			var response = await client.SendAsync( request );
			var result = await response.Content.ReadAsStringAsync();

			return result;
		}

		string RequestDownloadLink( PackageManager.PackageInfo package, string accessToken )
		{
			var task = RequestDownloadLinkAsync( package, accessToken );
			task.Wait();
			var responseString = task.Result;

			var data = JsonConvert.DeserializeObject<DownloadLinkJson.Root>( responseString );

			if( data == null || data.gltf == null )
				throw new Exception( "No link GLTF file." );

			return data.gltf.url;
		}

		string GetVirtualDestinationFolder( PackageManager.PackageInfo package )
		{
			var namePath = package.Identifier.Replace( '_', ' ' );
			foreach( char c in new string( Path.GetInvalidFileNameChars() ) + new string( Path.GetInvalidPathChars() ) )
				namePath = namePath.Replace( c.ToString(), "_" );
			return @$"Content\Models\Authors\{package.Author}\{namePath}";
		}

		public override void ThreadDownloadBody( StoresWindow.ThreadDownloadData data )
		{
			var package = data.Package;
			var state = data.State;


			//get download link
			var accessToken = GetAccessToken();
			var downloadLink = RequestDownloadLink( package, accessToken );


			var tempDownloadedFileName = Path.GetTempFileName();

			try
			{
				//download
				{
					using( WebClient client = new WebClient() )
					{
						state.downloadingClient = client;

						client.DownloadProgressChanged += delegate ( object sender, DownloadProgressChangedEventArgs e )
						{
							//check already ended
							if( data.Cancelled )
								return;

							if( e.TotalBytesToReceive != 0 )
								state.downloadProgress = MathEx.Saturate( (float)e.BytesReceived / (float)e.TotalBytesToReceive );
						};

						client.DownloadFileCompleted += delegate ( object sender, AsyncCompletedEventArgs e )
						{
							//check already ended
							if( !data.Cancelled )
							{
								//releases blocked thread
								lock( e.UserState )
									Monitor.Pulse( e.UserState );

								data.Cancelled = e.Cancelled;
								data.Error = e.Error;
							}
						};

						using( var task = client.DownloadFileTaskAsync( new Uri( downloadLink ), tempDownloadedFileName ) )
						{
							while( !string.IsNullOrEmpty( state.downloadingAddress ) && !task.Wait( 10 ) )
							{
							}
						}

						state.downloadingClient = null;
					}

					if( data.Cancelled || data.Error != null )
						return;
				}


				//process downloaded file

				var virtualDestinationFolder = GetVirtualDestinationFolder( package );

				using( var archive = ZipFile.Open( state.downloadingDestinationPath, ZipArchiveMode.Create ) )
				{
					//write Package.info
					{
						var block = new TextBlock();

						block.SetAttribute( "Identifier", package.Identifier );
						block.SetAttribute( "Store", store.Name );
						block.SetAttribute( "Title", package.Title );
						block.SetAttribute( "Version", package.Version );
						block.SetAttribute( "Author", package.Author );

						if( !string.IsNullOrEmpty( package.ShortDescription ) )
							block.SetAttribute( "Description", package.ShortDescription );

						if( !string.IsNullOrEmpty( package.FullDescription ) )
							block.SetAttribute( "FullDescription", package.FullDescription );

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

						if( !string.IsNullOrEmpty( package.Cost ) )
							block.SetAttribute( "Cost", package.Cost );

						if( package.License != StoreProductLicense.None )
							block.SetAttribute( "License", EnumUtility.GetValueDisplayName( package.License ) );

						//categories
						block.SetAttribute( "Categories", package.Categories );
						//{
						//var s = "";
						//foreach( CategoryEnum flag in GetFlags( package.Categories.Value ) )
						//{
						//	if( flag != 0 )
						//	{
						//		if( s.Length != 0 )
						//			s += ", ";
						//		s += TypeUtility.DisplayNameAddSpaces( flag.ToString() );
						//	}
						//}
						//block.SetAttribute( "Categories", s );
						//}

						if( !string.IsNullOrEmpty( package.Tags ) )
							block.SetAttribute( "Tags", package.Tags );

						var openAfterInstall = Path.Combine( virtualDestinationFolder, "scene.gltf" );
						//if( !string.IsNullOrEmpty( specifiedFile ) )
						//	openAfterInstall = Path.GetDirectoryName( specifiedFile );
						//else
						//	openAfterInstall = sourceDirectory.Substring( VirtualFileSystem.Directories.Assets.Length + 1 );
						block.SetAttribute( "OpenAfterInstall", openAfterInstall );


						//write to zip
						var entry = archive.CreateEntry( "Package.info" );
						using( var entryStream = entry.Open() )
						using( var streamWriter = new StreamWriter( entryStream ) )
							streamWriter.Write( block.DumpToString() );
					}

					//copy files
					using( var sketchfabArchive = ZipFile.Open( tempDownloadedFileName, ZipArchiveMode.Read ) )
					{
						foreach( var sketchfabEntry in sketchfabArchive.Entries )
						{
							//read
							var bytes = new byte[ sketchfabEntry.Length ];
							using( var sketchfabStream = sketchfabEntry.Open() )
								sketchfabStream.Read( bytes );

							var destPath = Path.Combine( "Assets", virtualDestinationFolder, sketchfabEntry.FullName );

							//write
							var entry = archive.CreateEntry( destPath );
							using( var entryStream = entry.Open() )
								entryStream.Write( bytes );
						}

					}

				}

			}
			finally
			{
				try
				{
					if( File.Exists( tempDownloadedFileName ) )
						File.Delete( tempDownloadedFileName );
				}
				catch { }
			}
		}
	}
}
