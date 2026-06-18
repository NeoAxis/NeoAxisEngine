// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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
using System.Globalization;
using Microsoft.Win32;
using System.Threading.Tasks;
using NeoAxis.Networking;

namespace NeoAxis.Editor
{
	public class NeoAxisStoreImplementation : StoreManager.StoreImplementation
	{
		const string registryPath = @"SOFTWARE\NeoAxis";

		ThreadItem currentThread;

		static ESet<string> featuredStoreItems = new ESet<string>();

		///////////////////////////////////////////////

		class ThreadItem
		{
			public Thread thread;
			public bool needStop;
			public string search;
			public StoreManager.FilterSettingsClass filterSettings;
		}

		///////////////////////////////////////////////

		public NeoAxisStoreImplementation()
		{
			//Add login properties to the Store window options
			ContentBrowserOptions.Configure += ContentBrowserOptions_Configure;
		}

		public static ESet<string> FeaturedStoreItems
		{
			get { return featuredStoreItems; }
		}

		static string ProcessDescription( string text )
		{
			try
			{
				// Define a regex pattern to match links in the specified format
				string pattern = @"\[(.*?)\]\(.*?\)";

				// Replace the matches with just the link text
				string result = Regex.Replace( text, pattern, "$1" );

				return result;
			}
			catch
			{
				return text;
			}
		}

		async void ThreadGetStoreItems( object threadItem2 )
		{
			ThreadItem threadItem = (ThreadItem)threadItem2;
			var filter = threadItem.filterSettings;

			try
			{

				//!!!!temp. impl chain paging later

				//!!!!
				var cloudServiceCommonStorage = "https://neox.nl-ams-1.linodeobjects.com";


				//not authorized Published projects
				TextBlock block1 = null;
				{

					//!!!!не качать если не было обновлений. может время в отдельном файле сохранять. где еще

					//!!!!pages


					//!!!!в сервисе этот адрес должен быть. получать оттуда в get service info

					var url = cloudServiceCommonStorage + "/Common%2FGetProjectsForApp.txt";

					var request = (HttpWebRequest)WebRequest.Create( url );
					request.Timeout = 120000;

					var blockString = "";
					using( var response = (HttpWebResponse)request.GetResponse() )
					using( var stream = response.GetResponseStream() )
					using( var reader = new StreamReader( stream ) )
						blockString = reader.ReadToEnd();

					if( threadItem.needStop || EditorAPI.ClosingApplication )
						return;

					block1 = TextBlock.Parse( blockString, out var error );
					if( !string.IsNullOrEmpty( error ) )
						throw new Exception( "Error of parsing the response data. " + error );
				}

				if( threadItem.needStop || EditorAPI.ClosingApplication )
					return;


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

					if( threadItem.needStop || EditorAPI.ClosingApplication )
						return;

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


				var packages = new List<PackageManager.PackageInfo>();


				//parse projects
				for( int nBlock = 0; nBlock < 2; nBlock++ )
				{
					var rootBlock = nBlock == 0 ? block1 : block2;
					if( rootBlock != null )
					{
						foreach( var block in rootBlock.Children )
						{
							if( block.Name == "Project" && block.GetAttribute( "ProductType" ) == "Content" )
							{
								var info = new PackageManager.PackageInfo();
								info.Store = store;

								var projectID = block.GetAttribute( "ID" );

								info.Identifier = projectID;
								info.Version = "1.0.0.0";

								info.Author = block.GetAttribute( "UserID" ); //item.UserID = long.Parse( block.GetAttribute( "UserID" ) );

								//Basic
								info.Title = block.GetAttribute( "Name" );

								//last update time
								if( DateTime.TryParseExact( block.GetAttribute( "PublicationStatusUpdateTime" ), "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var publicationStatusUpdateTime ) )
								{
									info.Time = publicationStatusUpdateTime;
								}
								else
								{
									//?
									//item.FirstPublishedTime = block.GetAttribute( "FirstPublishedTime" );
								}

								info.FullDescription = block.GetAttribute( "Description" );

								//!!!!need?
								info.FullDescription = ProcessDescription( info.FullDescription );

								info.ShortDescription = info.FullDescription;
								if( info.ShortDescription.Length > 80 )
									info.ShortDescription = info.ShortDescription.Substring( 0, 80 ) + "...";


								info.Permalink = EngineInfo.WebsiteFullAddress + "/neox/catalog/" + block.GetAttribute( "Slug" );


								////Type
								//item.Genre = block.GetAttribute( "Genre" );
								//item.AgeRestriction = int.Parse( block.GetAttribute( "AgeRestriction", "0" ) );


								//Publish: Monetization

								if( Enum.TryParse<CloudProductLicense>( block.GetAttribute( "License" ), out var license ) )
									info.License = license;

								if( float.TryParse( block.GetAttribute( "EntryFee", "0" ), out var cost ) )
									info.Cost = cost.ToString();

								if( cost == 0 )
								{
									info.FreeDownload = block.GetAttribute( "BuildContent" );
								}
								else
								{
									//!!!!impl paid
								}


								//!!!!
								//item.Currency = block.GetAttribute( "Currency" );

								info.Size = long.Parse( block.GetAttribute( "BuildContentSize", "0" ) );

								//files are not provided from the service. files are read from zip archive
								//////files
								////var buildContentFiles = block.GetAttribute( "BuildContentFiles" );
								////if( !string.IsNullOrEmpty( buildContentFiles ) )
								////{
								////	var files = buildContentFiles.Split( ';', StringSplitOptions.RemoveEmptyEntries );
								////	info.Files = string.Join( '\n', files );
								////}

								info.Thumbnail = $"{cloudServiceCommonStorage}/Projects/{projectID}_Logo_Preview.jpg";
								info.ThumbnailBig = $"{cloudServiceCommonStorage}/Projects/{projectID}_Logo.png";


								//!!!!возможно не движковые не добавлять


								//category
								var category = block.GetAttribute( "Category" );
								if( category.Contains( "NeoAxis Engine: " ) )
								{
									var category2 = category.Substring( "NeoAxis Engine: ".Length );
									info.Categories = category2;
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



								//!!!!from sketchfab:

								//info.Triangles = item.faceCount;
								//info.Vertices = item.vertexCount;
								//info.Animations = item.animationCount;


								//featured products
								if( info.Author == "3845" )
								{
									//!!!!
									if( info.Title == "Basic Materials 2K" || info.Title == "Basic Environments 4K" || info.Title == "Sci-fi Demo" )
									//if( info.Title == "Basic Materials 2K" || info.Title == "Basic Environments 4K" || info.Title == "City Demo" || info.Title == "Nature Demo" )
									{
										featuredStoreItems.AddWithCheckAlreadyContained( info.Identifier );
									}
								}

								////featured
								//foreach( XmlNode child in itemNode.ChildNodes )
								//{
								//	if( child.Name == "featured" )
								//		featuredStoreItems.AddWithCheckAlreadyContained( info.Identifier );
								//}


								packages.Add( info );
							}
						}
					}
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

		////void ThreadGetStoreItems( object threadItem2 )
		////{
		////	ThreadItem threadItem = (ThreadItem)threadItem2;
		////	var filter = threadItem.filterSettings;

		////	try
		////	{
		////		string xml = "";

		////		string url = EngineInfo.StoreAddress + @"/api/get_store_items2/";

		////		//first parameter must start from '?'
		////		url += "?min_triangle_count=" + filter.MinTriangleCount.ToString();
		////		if( filter.MaxTriangleCount > 0 )
		////			url += "&max_triangle_count=" + filter.MaxTriangleCount.ToString();

		////		if( !string.IsNullOrEmpty( threadItem.search ) )
		////			url += "&search=" + threadItem.search.Replace( ' ', '+' );

		////		if( filter.LicenseMIT )
		////			url += "&license_mit=true";
		////		if( filter.CCAttribution )
		////			url += "&license_cc_attribution=true";
		////		if( filter.CCAttributionBYSA )
		////			url += "&license_cc_attribution_by_sa=true";
		////		if( filter.CCAttributionBYND )
		////			url += "&license_cc_attribution_by_nd=true";
		////		if( filter.CCAttributionBYNC )
		////			url += "&license_cc_attribution_by_nc=true";
		////		if( filter.CCAttributionBYNCSA )
		////			url += "&license_cc_attribution_by_nc_sa=true";
		////		if( filter.CCAttributionBYNCND )
		////			url += "&license_cc_attribution_by_nc_nd=true";
		////		if( filter.CC0 )
		////			url += "&license_cc0=true";
		////		if( filter.FreeToUseWithNeoAxis )
		////			url += "&license_free_to_use_with_neoaxis=true";
		////		if( filter.PaidPerSeat )
		////			url += "&license_paid_per_seat=true";

		////		if( filter.SortBy == StoreManager.FilterSettingsClass.SortByEnum.Latest )
		////			url += "&sort_by=latest";
		////		////if( filter.SortBy == FilterSettingsClass.SortByEnum.Relevance )
		////		////	url += "&sort_by=-relevance";

		////		//!!!!
		////		//staff picked
		////		//animated
		////		//show restricted

		////		var useZip = true;

		////		if( useZip )
		////			url += "&zip=true";

		////		if( StoreManager.ModeratorMode && LoginUtility.GetCurrentLicense( out var email, out var hash ) )
		////		{
		////			var email64 = StringUtility.EncodeToBase64URL( email );
		////			var hash64 = StringUtility.EncodeToBase64URL( hash );
		////			url += $"&moderator_mode=true&email={email64}&hash={hash64}";
		////		}


		////		var request = (HttpWebRequest)WebRequest.Create( url );


		////		if( useZip )
		////		{
		////			//read data transfered in zip archive
		////			using( var response = (HttpWebResponse)request.GetResponse() )
		////			using( var stream = response.GetResponseStream() )
		////			{
		////				byte[] bytes;
		////				using( var memoryStream = new MemoryStream() )
		////				{
		////					stream.CopyTo( memoryStream );
		////					bytes = memoryStream.ToArray();
		////				}
		////				var data = IOUtility.Unzip( bytes );
		////				xml = Encoding.ASCII.GetString( data );
		////			}
		////		}
		////		else
		////		{
		////			using( var response = (HttpWebResponse)request.GetResponse() )
		////			using( var stream = response.GetResponseStream() )
		////			using( var reader = new StreamReader( stream ) )
		////				xml = reader.ReadToEnd();
		////		}

		////		if( threadItem.needStop || EditorAPI.ClosingApplication )
		////			return;

		////		XmlDocument xDoc = new XmlDocument();
		////		xDoc.LoadXml( xml );

		////		if( threadItem.needStop || EditorAPI.ClosingApplication )
		////			return;

		////		var packages = new List<PackageManager.PackageInfo>();

		////		foreach( XmlNode itemNode in xDoc.GetElementsByTagName( "item" ) )
		////		{
		////			var info = new PackageManager.PackageInfo();
		////			info.Store = store;

		////			//info.Name = "None";
		////			//info.Version = "1.0.0.0";

		////			foreach( XmlNode child in itemNode.ChildNodes )
		////			{
		////				if( child.Name == "identifier" )
		////					info.Identifier = child.InnerText;
		////				else if( child.Name == "title" )
		////					info.Title = child.InnerText;
		////				else if( child.Name == "author" )
		////					info.Author = child.InnerText;
		////				else if( child.Name == "version" )
		////					info.Version = child.InnerText;
		////				else if( child.Name == "size" )
		////				{
		////					double.TryParse( child.InnerText, out var value );
		////					info.Size = (long)value;
		////				}
		////				else if( child.Name == "free_download" )
		////					info.FreeDownload = child.InnerText;
		////				else if( child.Name == "secure_download" && !string.IsNullOrEmpty( child.InnerText ) )
		////					info.SecureDownload = (bool)SimpleTypes.ParseValue( typeof( bool ), child.InnerText );
		////				else if( child.Name == "short_description" )
		////				{
		////					info.ShortDescription = child.InnerText.Trim( new char[] { '\n', '\r', ' ', '\t' } );
		////					//remove tags
		////					try
		////					{
		////						info.ShortDescription = Regex.Replace( info.ShortDescription, "<.*?>", string.Empty );
		////					}
		////					catch { }
		////				}
		////				else if( child.Name == "full_description" )
		////					info.FullDescription = child.InnerText;
		////				else if( child.Name == "permalink" )
		////					info.Permalink = child.InnerText;
		////				else if( child.Name == "cost" )
		////					info.Cost = child.InnerText;
		////				else if( child.Name == "date" )
		////					info.Date = child.InnerText;
		////				else if( child.Name == "files" )
		////					info.Files = child.InnerText;
		////				else if( child.Name == "categories" )
		////					info.Categories = child.InnerText;
		////				else if( child.Name == "tags" )
		////					info.Tags = child.InnerText;
		////				else if( child.Name == "thumbnail" )
		////					info.Thumbnail = child.InnerText;
		////				else if( child.Name == "triangles" )
		////				{
		////					int.TryParse( child.InnerText, out var value );
		////					info.Triangles = value;
		////				}
		////				else if( child.Name == "vertices" )
		////				{
		////					int.TryParse( child.InnerText, out var value );
		////					info.Vertices = value;
		////				}
		////				else if( child.Name == "rigged" && !string.IsNullOrEmpty( child.InnerText ) )
		////					info.Rigged = (bool)SimpleTypes.ParseValue( typeof( bool ), child.InnerText );
		////				else if( child.Name == "animations" )
		////				{
		////					int.TryParse( child.InnerText, out var value );
		////					info.Animations = value;
		////				}
		////				else if( child.Name == "license" )
		////				{
		////					var text = child.InnerText.Replace( " ", "" ).Replace( "-", "" );
		////					Enum.TryParse<StoreProductLicense>( text, out var value );
		////					info.License = value;
		////				}
		////			}

		////			//featured
		////			foreach( XmlNode child in itemNode.ChildNodes )
		////			{
		////				if( child.Name == "featured" )
		////					featuredStoreItems.AddWithCheckAlreadyContained( info.Identifier );
		////			}

		////			//calculate short description
		////			if( string.IsNullOrEmpty( info.ShortDescription ) && !string.IsNullOrEmpty( info.FullDescription ) )
		////			{
		////				try
		////				{
		////					var text = info.FullDescription;

		////					var index = info.FullDescription.IndexOf( "<img" );
		////					if( index != -1 )
		////						text = text.Substring( 0, index ).Trim();

		////					text = Regex.Replace( text, "<.*?>", string.Empty );

		////					text = Regex.Replace( text, @"(?<=[.?!])(?=[^\s])", " " );

		////					if( text.Length > 80 )
		////						text = text.Substring( 0, 80 ) + "...";

		////					info.ShortDescription = text;
		////				}
		////				catch { }
		////			}

		////			////skip for cloud project mode
		////			//if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient )
		////			//{
		////			//	if( info.Identifier == "Platform_Tools" )
		////			//		continue;
		////			//}

		////			packages.Add( info );
		////		}

		////		if( threadItem.needStop || EditorAPI.ClosingApplication )
		////			return;

		////		////sort by date
		////		//CollectionUtility.MergeSort( packages, delegate ( PackageManager.PackageInfo p1, PackageManager.PackageInfo p2 )
		////		//{
		////		//	if( p1.Date != p2.Date )
		////		//		return -string.Compare( p1.Date, p2.Date );
		////		//	return 0;
		////		//} );

		////		StoreManager.SetDownloadedListOfPackages( store, packages );
		////	}
		////	catch { }
		////}

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
					File.Move( tempDownloadedFileName, downloadFullPath, true );// state.downloadingDestinationPath );
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
		}

		private void ContentBrowserOptions_Configure( ContentBrowserOptions sender )
		{
			if( sender is StoresWindow.StoresContentBrowserOptions )
			{
				LoadFromRegistry( out var accessToken );

				{
					var attributes = new List<Attribute>();
					attributes.Add( new DisplayNameAttribute( "NeoX Access Token" ) );
					attributes.Add( new DescriptionAttribute( "Log in to the NeoX app to enable the store, or specify the access token." ) );

					var property = new ContentBrowserOptions.PropertyImpl( sender, "NeoXAccessToken", MetadataManager.GetTypeOfNetType( typeof( string ) ), attributes, "NeoX", "" );
					property.DefaultValueSpecified = true;
					property.DefaultValue = "";

					property.Value = accessToken;

					property.ValueChanged += delegate ( ContentBrowserOptions.PropertyImpl sender )
					{
						Save( (ContentBrowserOptions)sender.Owner );
					};

					sender.AddProperty( property );
				}
			}
		}

		public static bool LoadFromRegistry( out string accesssToken )
		{
			try
			{
				//opening the subkey  
				RegistryKey key = Registry.CurrentUser.OpenSubKey( registryPath );

				//if it does exist, retrieve the stored values
				if( key != null )
				{
					accesssToken = EncryptDecrypt( ( key.GetValue( "NeoXAccessToken" ) ?? "" ).ToString() );

					key.Close();
					return true;
				}
			}
			catch { }

			accesssToken = "";
			return false;
		}

		static string EncryptDecrypt( string input )
		{
			char[] key = { 'K', 'C', 'Q' }; //Any chars will work, in an array of any size
			char[] output = new char[ input.Length ];

			for( int i = 0; i < input.Length; i++ )
				output[ i ] = (char)( input[ i ] ^ key[ i % key.Length ] );

			return new string( output );
		}

		public static void SaveToRegistry( string accessToken )
		{
			try
			{
				var key = Registry.CurrentUser.CreateSubKey( registryPath );
				key.SetValue( "NeoXAccessToken", EncryptDecrypt( accessToken ) );
				key.Close();
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return;
			}
		}

		static void Save( ContentBrowserOptions options )
		{
			var accessToken = (string)( (ContentBrowserOptions.PropertyImpl)options.MetadataGetMemberBySignature( "property:NeoXAccessToken" ) ).Value;

			SaveToRegistry( accessToken );
		}
	}
}