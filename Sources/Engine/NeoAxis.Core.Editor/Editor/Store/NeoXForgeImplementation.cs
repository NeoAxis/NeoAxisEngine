// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using NeoAxis.Networking;
using NeoAxis.CloudServer;

namespace NeoAxis.Editor
{
	public class NeoXForgeImplementation : StoreManager.StoreImplementation
	{
		static NeoXForgeImplementation instance;

		public const string StorageDirectory = "Forge";
		const long projectID = 3082;

		ThreadItem currentThread;
		CloudFunctionsClient cloudClient;

		//static ESet<string> featuredStoreItems = new ESet<string>();

		///////////////////////////////////////////////

		class ThreadItem
		{
			public Thread thread;
			public bool needStop;
			public string search;
			public StoreManager.FilterSettingsClass filterSettings;
		}

		///////////////////////////////////////////////

		public NeoXForgeImplementation()
		{
			instance = this;
		}

		public static NeoXForgeImplementation Instance
		{
			get { return instance; }
		}

		//public static ESet<string> FeaturedStoreItems
		//{
		//	get { return featuredStoreItems; }
		//}

		//static string ProcessDescription( string text )
		//{
		//	try
		//	{
		//		// Define a regex pattern to match links in the specified format
		//		string pattern = @"\[(.*?)\]\(.*?\)";

		//		// Replace the matches with just the link text
		//		string result = Regex.Replace( text, pattern, "$1" );

		//		return result;
		//	}
		//	catch
		//	{
		//		return text;
		//	}
		//}

		async Task<bool> ConnectAsync( CancellationToken cancellationToken = default )
		{
			//disconnect previous client
			cloudClient?.Destroy();
			cloudClient = null;

			//connection settings
			var settings = BasicServiceClient.ConnectionSettingsClass.CreateCloud( CloudUserRole.Player, projectID, true );

			//create client and connect
			var createResult = await CloudFunctionsClient.CreateAsync( settings, true, cancellationToken );
			if( !string.IsNullOrEmpty( createResult.Error ) )
			{
				throw new Exception( "Error of connecting to the service. " + createResult.Error );
				//Log( "Error: " + createResult.Error );
				//return false;
			}

			//now connected
			cloudClient = createResult.Client;

			//!!!!need?
			//register types from additional dlls for call methods
			cloudClient.ConnectionNode.CloudFunctions.RegisterAssemblyForCloudMethodTypes( typeof( Chats.Chat ).Assembly );

			////register to receive messages from the server
			//cloudClient.ConnectionNode.Messages.ReceiveMessageString += Messages_ReceiveMessageString;

			return true;
		}

		long GetThisUserID()
		{
			return cloudClient.ConnectionNode?.Users.ThisUser?.UserID ?? 0;
		}

		async Task GetThumbnainsAsync( List<PackageManager.PackageInfo> packages )
		{
			//get all with one request

			var storageFileNames = new string[ packages.Count ];
			for( int n = 0; n < packages.Count; n++ )
			{
				var info = packages[ n ];
				var storageDirectoryName = $"{info.Author}/{StorageDirectory}/{info.Identifier}";
				storageFileNames[ n ] = $"{storageDirectoryName}/OutputPreview.jpg";
			}

			var cts3 = new CancellationTokenSource( new TimeSpan( 0, 0, 15 * packages.Count ) );
			var getContentUrlsResult = await CloudServiceFunctions.StorageGetContentUrlsAsync( storageFileNames, false, false, "", cts3.Token );

			if( string.IsNullOrEmpty( getContentUrlsResult.Error ) )
			{
				for( int n = 0; n < packages.Count; n++ )
				{
					var info = packages[ n ];
					info.Thumbnail = getContentUrlsResult.Urls[ n ];
				}
			}
			else
			{
				//per item because of possible errors. if error, just no thumbnail

				for( int n = 0; n < packages.Count; n++ )
				{
					var info = packages[ n ];

					var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					var getContentUrlResult = await CloudServiceFunctions.StorageGetContentUrlAsync( storageFileNames[ n ], false, false, "", cts.Token );

					if( !string.IsNullOrEmpty( getContentUrlResult.Url ) )
						info.Thumbnail = getContentUrlResult.Url;
				}
			}
		}

		async void ThreadGetStoreItems( object threadItem2 )
		{
			ThreadItem threadItem = (ThreadItem)threadItem2;
			var filter = threadItem.filterSettings;

			try
			{
				//connect
				{
					var client2 = cloudClient;
					if( client2 == null || client2.Status == NetworkStatus.Disconnected )
						await ConnectAsync();
				}

				//!!!!impl chain when many items

				//get items
				{
					var client2 = cloudClient;
					var userID = GetThisUserID();
					if( client2 != null && userID != 0 )
					{
						var packages = new List<PackageManager.PackageInfo>();


						//!!!!more content types


						var cts2 = new CancellationTokenSource( new TimeSpan( 0, 2, 0 ) );
						var getItemsResult = await client2.CallMethodAsync<string>( "Implementation", "GetItemsTextBlock", cts2.Token, null, new[] { userID }, null, new[] { "Finished" }, new[] { "3D Model" } );
						if( !string.IsNullOrEmpty( getItemsResult.Error ) )
							throw new Exception( getItemsResult.Error );

						if( threadItem.needStop || EditorAPI.ClosingApplication )
							return;

						var rootBlock = TextBlock.Parse( getItemsResult.Value, out var error );
						if( !string.IsNullOrEmpty( error ) )
							throw new Exception( "Error of parsing the response data. " + error );

						foreach( var block in rootBlock.Children )
						{
							if( block.Name == "Item" )
							{
								var info = new PackageManager.PackageInfo();
								info.Store = store;

								var itemID = block.GetAttribute( "Id" );
								info.Identifier = itemID;
								info.Version = "1.0.0.0";

								info.Author = block.GetAttribute( "UserID" );

								info.Title = block.GetAttribute( "Name" );
								if( info.Title.Length > 40 )
									info.Title = info.Title.Substring( 0, 40 ) + "...";

								if( string.IsNullOrEmpty( info.Title ) )
									info.Title = "No name";

								long.TryParse( block.GetAttribute( "CreationTime" ), out var creationTimeTicks );
								var creationTime = new DateTime( creationTimeTicks, DateTimeKind.Utc );

								var contentType = block.GetAttribute( "ContentType" );

								int.TryParse( block.GetAttribute( "Amount", "0" ), out var amount );

								info.Time = creationTime;


								//!!!!

								//public long Size;
								//public string FreeDownload;
								//public string Files;
								//public string Tags;


								//!!!!

								//descriptions
								info.FullDescription = "";
								info.ShortDescription = "";

								//info.FullDescription = block.GetAttribute( "Description" );
								//info.ShortDescription = info.FullDescription;
								//if( info.ShortDescription.Length > 80 )
								//	info.ShortDescription = info.ShortDescription.Substring( 0, 80 ) + "...";


								//no permalink because private content
								//info.Permalink = EngineInfo.WebsiteFullAddress + "/neox/catalog/" + block.GetAttribute( "Slug" );


								//license
								info.License = CloudProductLicense.None;
								//if( Enum.TryParse<CloudProductLicense>( block.GetAttribute( "License" ), out var license ) )
								//	info.License = license;


								//!!!!

								//if( cost == 0 )
								//{
								//	info.FreeDownload = block.GetAttribute( "BuildContent" );
								//}
								//else
								//{
								//	//!!!!impl paid
								//}


								//!!!!

								//info.Size = long.Parse( block.GetAttribute( "BuildContentSize", "0" ) );


								//files are not provided from the service. files are read from zip archive
								//////files
								////var buildContentFiles = block.GetAttribute( "BuildContentFiles" );
								////if( !string.IsNullOrEmpty( buildContentFiles ) )
								////{
								////	var files = buildContentFiles.Split( ';', StringSplitOptions.RemoveEmptyEntries );
								////	info.Files = string.Join( '\n', files );
								////}


								//category
								if( contentType == "3D Model" )
									info.Categories = "3D Models";
								else
								{
									//!!!!
								}


								//!!!!

								//else if( child.Name == "tags" )
								//	info.Tags = child.InnerText;



								//!!!!impl. 3d model details

								//else if( child.Name == "triangles" )
								//{
								//	int.TryParse( child.InnerText, out var value );
								//	info.Triangles = value;
								//}
								//else if( child.Name == "vertices" )
								//{
								//	int.TryParse( child.InnerText, out var value );
								//	info.Vertices = value;
								//}
								//else if( child.Name == "rigged" && !string.IsNullOrEmpty( child.InnerText ) )
								//	info.Rigged = (bool)SimpleTypes.ParseValue( typeof( bool ), child.InnerText );
								//else if( child.Name == "animations" )
								//{
								//	int.TryParse( child.InnerText, out var value );
								//	info.Animations = value;
								//}



								//!!!!

								//screenshots

								//public string Author;
								//public bool SecureDownload;
								//public string Files;
								//public string Categories;
								//public string Tags;

								//public int Triangles;
								//public int Vertices;

								//public bool Rigged;
								//public int Animations;

								//public StoreProductLicense License;



								//!!!!

								//info.Triangles = item.faceCount;
								//info.Vertices = item.vertexCount;
								//public bool Rigged;
								//info.Animations = item.animationCount;


								packages.Add( info );
							}
						}

						//get thumbnails
						await GetThumbnainsAsync( packages );

						//sort by date
						CollectionUtility.MergeSort( packages, delegate ( PackageManager.PackageInfo p1, PackageManager.PackageInfo p2 )
						{
							if( p1.Time != p2.Time )
								return -DateTime.Compare( p1.Time.Value, p2.Time.Value );
							return 0;
						} );

						StoreManager.SetDownloadedListOfPackages( store, packages );

						//refresh content urls
						while( true )
						{
							for( int n = 0; n < 45; n++ )
							{
								await Task.Delay( new TimeSpan( 0, 1, 0 ) );
								if( threadItem.needStop || EditorAPI.ClosingApplication )
									return;
							}

							//get thumbnails
							await GetThumbnainsAsync( packages );
						}
					}
				}


#if ___


				//login from registry or access token specicied
				var authenticated = false;
				LoadFromRegistry( out var accessToken );
				if( !string.IsNullOrEmpty( accessToken ) )
					authenticated = true;
				else
				{
					if( LoginUtility.GetCurrentLicense( out string email, out string hash ) )
						if( LoginUtility.GetRequestedInfo( out var userID, out _ ) )
							authenticated = true;
				}

				TextBlock block2 = null;
				if( authenticated )
				{
					using var cts = new CancellationTokenSource( new TimeSpan( 0, 2, 0 ) );
					var getResult = await CloudServiceFunctions.ProductGetUserSpecificProductsAsync( accessToken, cancellationToken: cts.Token );
					if( !string.IsNullOrEmpty( getResult.Error ) )
						throw new Exception( getResult.Error );

					block2 = getResult.Data;
				}



				//!!!!filters


				//url += "?min_triangle_count=" + filter.MinTriangleCount.ToString();
				//if( filter.MaxTriangleCount > 0 )
				//	url += "&max_triangle_count=" + filter.MaxTriangleCount.ToString();


				//!!!!search
				//if( !string.IsNullOrEmpty( threadItem.search ) )
				//	url += "&search=" + threadItem.search.Replace( ' ', '+' );


				//!!!!
				//if( filter.LicenseMIT )
				//	url += "&license_mit=true";
				//if( filter.CCAttribution )
				//	url += "&license_cc_attribution=true";
				//if( filter.CCAttributionBYSA )
				//	url += "&license_cc_attribution_by_sa=true";
				//if( filter.CCAttributionBYND )
				//	url += "&license_cc_attribution_by_nd=true";
				//if( filter.CCAttributionBYNC )
				//	url += "&license_cc_attribution_by_nc=true";
				//if( filter.CCAttributionBYNCSA )
				//	url += "&license_cc_attribution_by_nc_sa=true";
				//if( filter.CCAttributionBYNCND )
				//	url += "&license_cc_attribution_by_nc_nd=true";
				//if( filter.CC0 )
				//	url += "&license_cc0=true";
				//if( filter.FreeToUseWithNeoAxis )
				//	url += "&license_free_to_use_with_neoaxis=true";
				//if( filter.PaidPerSeat )
				//	url += "&license_paid_per_seat=true";


				//!!!
				//if( filter.SortBy == StoreManager.FilterSettingsClass.SortByEnum.Latest )
				//	url += "&sort_by=latest";


				//!!!!zip
				////read data transfered in zip archive
				//using( var response = (HttpWebResponse)request.GetResponse() )
				//using( var stream = response.GetResponseStream() )
				//{
				//	byte[] bytes;
				//	using( var memoryStream = new MemoryStream() )
				//	{
				//		stream.CopyTo( memoryStream );
				//		bytes = memoryStream.ToArray();
				//	}
				//	var data = IOUtility.Unzip( bytes );
				//	xml = Encoding.ASCII.GetString( data );
				//}


#endif

			}
			catch( Exception e )
			{
				Log.Info( "NeoXForge: Error of loading the store items. " + e.ToString() );
			}
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

		public class DownloadBuildResult
		{
			public string Error;
		}

		public async override Task DownloadBodyAsync( StoresWindow.TaskDownloadData data )
		{

			//!!!!

#if ___

			var package = data.Package;
			var state = data.State;

			//login from registry or access token specicied
			var authenticated = false;
			LoadFromRegistry( out var accessToken );
			if( !string.IsNullOrEmpty( accessToken ) )
				authenticated = true;
			else
			{
				if( LoginUtility.GetCurrentLicense( out string email, out string hash ) )
					if( LoginUtility.GetRequestedInfo( out var userID, out _ ) )
						authenticated = true;
			}
			if( !authenticated )
			{
				data.Error = new Exception( "You must be logged in to download this package. Use Project Menu to login." );
				return;
			}

			var tempDownloadedFileName = Path.Combine( Path.GetTempPath(), "Temp" + Path.GetRandomFileName() );

			var fileNameFromBuild = package.Identifier + "-" + Path.GetFileName( PathUtility.NormalizePath( package.FreeDownload ?? "" ) );
			var downloadFullPath = Path.Combine( PackageManager.PackagesFolder, fileNameFromBuild );

			try
			{
				//download
				{
					var storageFileName = package.FreeDownload;

					//check storage file name
					if( string.IsNullOrEmpty( storageFileName ) )
					{
						data.Error = new Exception( "The content build is not available." );
						return;
					}

					//get content url
					using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					var getContentUrlResult = await CloudServiceFunctions.StorageGetContentUrlAsync( storageFileName, false, false, accessToken, cts.Token );
					if( !string.IsNullOrEmpty( getContentUrlResult.Error ) )
					{
						data.Error = new Exception( getContentUrlResult.Error );
						return;
					}

					//destinationFullPath = PathUtility.NormalizePath( destinationFullPath );

					//var targetFullPathDirectory = Path.GetDirectoryName( tempDownloadedFileName );
					//Directory.CreateDirectory( targetFullPathDirectory );

					//download file
					using var cts2 = new CancellationTokenSource( new TimeSpan( 100, 0, 0 ) );

					void Progress( int downloadedIncrement, long totalDownloaded, long totalSize )
					{
						if( data.Cancelled )
							cts2.Cancel();

						state.downloadProgress = (float)MathEx.Saturate( (double)totalDownloaded / Math.Max( totalSize, 1 ) );
					}

					var downloadResult = await NetworkUtility.DownloadFileByUrlAsync( getContentUrlResult.Url, tempDownloadedFileName, Progress, cts2.Token );
					if( !string.IsNullOrEmpty( downloadResult.Error ) )
					{
						data.Error = new Exception( downloadResult.Error );
						return;
					}

					if( data.Cancelled )
						return;
				}

				//move temp file to the final destination
				try
				{
					File.Move( tempDownloadedFileName, downloadFullPath );// state.downloadingDestinationPath );
				}
				catch( Exception e )
				{
					data.Error = e;
					return;
				}

				//process downloaded file

				using( var archive = ZipFile.Open( downloadFullPath/*state.downloadingDestinationPath*/, ZipArchiveMode.Update ) )
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

						if( package.License != CloudProductLicense.None )
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


						//!!!!open after install depending category and files inside

						//var openAfterInstall = Path.Combine( virtualDestinationFolder, "scene.gltf" );
						////if( !string.IsNullOrEmpty( specifiedFile ) )
						////	openAfterInstall = Path.GetDirectoryName( specifiedFile );
						////else
						////	openAfterInstall = sourceDirectory.Substring( VirtualFileSystem.Directories.Assets.Length + 1 );
						//block.SetAttribute( "OpenAfterInstall", openAfterInstall );

						//files loaded from zip
						////Files
						//{						
						//}

						//write to zip
						var entry = archive.CreateEntry( "Package.info" );
						using( var entryStream = entry.Open() )
						using( var streamWriter = new StreamWriter( entryStream ) )
							streamWriter.Write( block.DumpToString() );
					}

					//var licenseTxtText = "";

					////copy files
					//using( var zipArchive = ZipFile.Open( tempDownloadedFileName, ZipArchiveMode.Read ) )
					//{
					//	foreach( var zipEntry in zipArchive.Entries )
					//	{
					//		//read
					//		var bytes = new byte[ zipEntry.Length ];
					//		using( var stream = zipEntry.Open() )
					//			stream.Read( bytes );

					//		var destPath = Path.Combine( "Assets", virtualDestinationFolder, zipEntry.FullName );

					//		//write
					//		var entry = archive.CreateEntry( destPath );
					//		using( var entryStream = entry.Open() )
					//			entryStream.Write( bytes );
					//	}

					//	////license.txt
					//	//{
					//	//	var licenseTxtEntry = sketchfabArchive.GetEntry( "license.txt" );
					//	//	if( licenseTxtText != null )
					//	//	{
					//	//		using( var sketchfabStream = licenseTxtEntry.Open() )
					//	//		{
					//	//			using( var reader = new StreamReader( sketchfabStream ) )
					//	//				licenseTxtText = reader.ReadToEnd();
					//	//		}
					//	//	}
					//	//}
					//}
				}

				//set downloaded path to process after download
				state.downloadingDestinationPath = downloadFullPath;
				//state.downloadingInstallAfterDownload = true;

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
#endif
		}

		public async Task<CloudFunctionsClient> GetOrConnectClientAsync( CancellationToken cancellationToken )
		{
			var client2 = cloudClient;
			if( client2 == null || client2.Status == NetworkStatus.Disconnected )
				await ConnectAsync( cancellationToken );
			return client2;
		}
	}
}