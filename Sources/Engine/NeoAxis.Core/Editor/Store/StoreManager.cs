#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using Internal.ComponentFactory.Krypton.Toolkit;
using System.Net;
using System.Threading;
using System.Xml;
using System.Linq;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public static class StoreManager
	{
		static List<StoreItem> stores = new List<StoreItem>();

		static ImageManagerClass imageManager = new ImageManagerClass();
		//static FullPackageInfoClass fullPackageInfo = new FullPackageInfoClass();

		static List<Dictionary<string, PackageManager.PackageInfo>> downloadedListOfPackages = new List<Dictionary<string, PackageManager.PackageInfo>>();
		//static Dictionary<string, PackageManager.PackageInfo> downloadedListOfPackages = new Dictionary<string, PackageManager.PackageInfo>();
		public static event Action DownloadedListOfPackagesUpdated;

		public static ESet<StoreItem> needGetNextItemsForStores = new ESet<StoreItem>();

		///////////////////////////////////////////////

		public class StoreItem
		{
			public string Name;
			public Image Icon16;
			public Image Icon32;
			public string Website;
			public StoreImplementation Implementation;

			internal Image IconScaled;
		}

		///////////////////////////////////////////////

		public abstract class StoreImplementation
		{
			public StoreItem store;

			public abstract void StartRefreshPackageList( string search, FilterSettingsClass filterSettings );
			public abstract void StopCurrentTask();

			public void Shutdown()
			{
				StopCurrentTask();
			}

			public abstract void ThreadDownloadBody( StoresWindow.ThreadDownloadData data );
			//public abstract void ThreadDownloadBody( PackageManager.PackageInfo package, ref bool cancelled );//, ref Exception error );
		}

		///////////////////////////////////////////////

		public class ImageManagerClass
		{
			Dictionary<string, Item> items = new Dictionary<string, Item>();

			/////////////////////

			class Item
			{
				public string url;

				public volatile HttpWebRequest webRequest;
				public volatile HttpWebResponse webResponse;

				public volatile Image sourceImage;
				public volatile Image squareImage;

				public volatile bool disposed;

				public double lastUsedTime;

				//

				public void Dispose()
				{
					disposed = true;

					try
					{
						webResponse?.Close();
						webRequest?.Abort();
					}
					catch { }

					try
					{
						sourceImage?.Dispose();
						squareImage?.Dispose();
					}
					catch { }
				}
			}

			/////////////////////

			Item GetItem( string url )
			{
				if( !items.TryGetValue( url, out var item ) )
				{
					item = new Item();
					item.url = url;
					items[ url ] = item;

					var thread1 = new Thread( ThreadGetItemImage );
					thread1.IsBackground = true;
					thread1.Start( item );
				}

				return item;
			}

			public Image GetSourceImage( string url, double currentTime )
			{
				var item = GetItem( url );
				if( item.sourceImage != null && item.sourceImage.PixelFormat != System.Drawing.Imaging.PixelFormat.DontCare )
				{
					item.lastUsedTime = currentTime;
					return item.sourceImage;
				}
				return null;
			}

			public Image GetSquareImage( string url, double currentTime )
			{
				var item = GetItem( url );
				if( item.squareImage != null && item.squareImage.PixelFormat != System.Drawing.Imaging.PixelFormat.DontCare )
				{
					item.lastUsedTime = currentTime;
					return item.squareImage;
				}
				return null;
			}

			void ThreadGetItemImage( object tag )
			{
				var item = (Item)tag;

				try
				{
					var request = (HttpWebRequest)WebRequest.Create( item.url );
					if( item.disposed )
						return;
					item.webRequest = request;

					using( var response = (HttpWebResponse)request.GetResponse() )
					{
						if( item.disposed )
							return;
						item.webResponse = response;

						using( var stream = response.GetResponseStream() )
						{
							var sourceImage = Image.FromStream( stream, true, true );

							//!!!!
							var maxSize = 512;

							var scale = 1.0;
							if( sourceImage.Height > maxSize )
								scale = (double)maxSize / (double)sourceImage.Height;

							var size = (int)( scale * (double)sourceImage.Height );

							//var size = Math.Min( sourceImage.Width, sourceImage.Height );
							//if( size > 512 )
							//	size = 512;

							var squareImage = new Bitmap( size, size, sourceImage.PixelFormat );
							using( var g = Graphics.FromImage( squareImage ) )
							{
								g.DrawImage( sourceImage, new System.Drawing.Rectangle( 0, 0, size, size ), new System.Drawing.Rectangle( ( sourceImage.Width - sourceImage.Height ) / 2, 0, sourceImage.Height, sourceImage.Height ), GraphicsUnit.Pixel );
							}

							if( item.disposed )
								return;

							item.sourceImage = sourceImage;
							item.squareImage = squareImage;
						}
					}
				}
				catch
				{
				}
				finally
				{
					item.webRequest = null;
					item.webResponse = null;
				}
			}

			public void Clear()
			{
				foreach( var item in items.Values )
					item.Dispose();
				items.Clear();
			}

			public void DeleteItemsNotUsedForLongTime()
			{
				var time = EngineApp.GetSystemTime();

				var toDelete = new List<Item>();
				foreach( var item in items.Values )
				{
					if( item.lastUsedTime != 0 && item.lastUsedTime + 30 < time )
						toDelete.Add( item );
				}

				foreach( var item in toDelete )
				{
					items.Remove( item.url );
					item.Dispose();
				}
			}
		}

		///////////////////////////////////////////////

		public class FilterSettingsClass
		{
			//!!!!
			//Date
			//Sound

			//!!!!
			//MostLiked, MostViewed
			//Popularity, Average Rating

			//!!!!

			//public enum CategoriesEnum
			//{
			//	SelectSubCategoriesUsingBreadcrumb
			//}

			//[Category( "Categories" )]
			////[Description( "" )]
			//[DefaultValue( CategoriesEnum.SelectSubCategoriesUsingBreadcrumb )]
			//public CategoriesEnum Category { get; } = CategoriesEnum.SelectSubCategoriesUsingBreadcrumb;

			//////////////////////////////////////////////

			[Category( "Licenses" )]
			[Description( "Permissive free software license." )]
			[DefaultValue( true )]
			[DisplayName( "MIT" )]
			[EngineConfig( "StoresWindowFilterSettings" )]
			public bool LicenseMIT { get; set; } = true;

			[Category( "Licenses" )]
			[Description( "Attribution alone. Licensees may copy, distribute, display and perform the work and make derivative works and remixes based on it only if they give the author the credits." )]
			[DefaultValue( true )]
			[DisplayName( "CC Attribution" )]
			public bool CCAttribution { get; set; } = true;

			[Category( "Licenses" )]
			[Description( "Attribution, share-alike. Licensees may distribute derivative works only under a license identical to the license that governs the original work." )]
			[DefaultValue( true )]
			[DisplayName( "CC Attribution BY-SA" )]
			public bool CCAttributionBYSA { get; set; } = true;

			[Category( "Licenses" )]
			[Description( "Attribution, no derivative works. Licensees may copy, distribute, display and perform only verbatim copies of the work, not derivative works and remixes based on it." )]
			[DefaultValue( true )]
			[DisplayName( "CC Attribution BY-ND" )]
			public bool CCAttributionBYND { get; set; } = true;

			[Category( "Licenses" )]
			[Description( "Attribution, non-commercial. Licensees may copy, distribute, display, and perform the work and make derivative works and remixes based on it only for non-commercial purposes." )]
			[DefaultValue( false )]
			[DisplayName( "CC Attribution BY-NC" )]
			public bool CCAttributionBYNC { get; set; } = false;

			[Category( "Licenses" )]
			[Description( "Attribution, non-commercial, share-alike." )]
			[DefaultValue( false )]
			[DisplayName( "CC Attribution BY-NC-SA" )]
			public bool CCAttributionBYNCSA { get; set; } = false;

			[Category( "Licenses" )]
			[Description( "Attribution, non-commercial, no derivative works." )]
			[DefaultValue( false )]
			[DisplayName( "CC Attribution BY-NC-ND" )]
			public bool CCAttributionBYNCND { get; set; } = false;

			[Category( "Licenses" )]
			[Description( "Freeing content globally without restrictions." )]
			[DefaultValue( true )]
			[DisplayName( "CC0" )]
			public bool CC0 { get; set; } = true;

			//!!!!
			//[Category( "Licenses" )]
			//[Description( "Free to use license." )]
			//[DefaultValue( true )]
			//[DisplayName( "Free To Use" )]
			//public bool FreeToUse { get; set; } = true;

			[Category( "Licenses" )]
			[Description( "Free to use, can only be used in NeoAxis products." )]
			[DefaultValue( true )]
			[DisplayName( "Free To Use With NeoAxis" )]
			public bool FreeToUseWithNeoAxis { get; set; } = true;

			[Category( "Licenses" )]
			[Description( "Paid license for individual person or per seat in an organization." )]
			[DefaultValue( true )]
			public bool PaidPerSeat { get; set; } = true;

			//[Category( "Licenses" )]
			//[Description( "Paid product from a third-party service, not from NeoAxis." )]
			//[DefaultValue( false )]
			//public bool PaidThirdParty { get; set; } = false;

			//////////////////////////////////////////////

			public enum SortByEnum
			{
				Relevance,
				Latest,
			}

			[Category( "Sort" )]
			//[Description( "" )]
			[DefaultValue( SortByEnum.Relevance )]
			public SortByEnum SortBy { get; set; } = SortByEnum.Relevance;

			//[Category( "Sort" )]
			////[Description( "" )]
			//[DefaultValue( true )]
			//[DisplayName( "Ascending" )]
			//public bool SortByAscending { get; set; } = true;

			//////////////////////////////////////////////

			[Category( "Models" )]
			[Description( "Minimum number of triangles in the model." )]
			[DefaultValue( 1 )]
			[Range( 1, 10000000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
			public int MinTriangleCount { get; set; } = 1;

			[Category( "Models" )]
			[Description( "Maximum number of triangles in the model." )]
			[DefaultValue( 10000000 )]
			[Range( 1, 10000000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
			public int MaxTriangleCount { get; set; } = 10000000;

			[Category( "Models" )]
			[Description( "Selected as awesome content by staff." )]
			[DefaultValue( false )]
			public bool StaffPicks { get; set; } = false;

			[Category( "Models" )]
			[Description( "The content has animations." )]
			[DefaultValue( false )]
			public bool Animated { get; set; } = false;

			[Category( "Models" )]
			[Description( "Content that contain partial or full nudity, that depict drug use, extreme or realistically violent content are filtered." )]
			[DefaultValue( false )]
			public bool ShowRestricted { get; set; } = false;

			//////////////////////////////////////////////

			public FilterSettingsClass Clone()
			{
				var clone = new FilterSettingsClass();

				var type = MetadataManager.GetTypeOfNetType( typeof( FilterSettingsClass ) );
				foreach( var member in type.MetadataGetMembers() )
				{
					var p = member as Metadata.Property;
					if( p != null && !p.ReadOnly )
					{
						var v = p.GetValue( this, null );
						p.SetValue( clone, v, null );
					}
				}

				return clone;
			}

			public void Load( TextBlock block )
			{
				try
				{
					var type = MetadataManager.GetTypeOfNetType( typeof( FilterSettingsClass ) );
					foreach( var member in type.MetadataGetMembers() )
					{
						var p = member as Metadata.Property;
						if( p != null && !p.ReadOnly )
						{
							if( block.AttributeExists( p.Name ) )
							{
								var v = SimpleTypes.ParseValue( p.Type.GetNetType(), block.GetAttribute( p.Name ) );
								p.SetValue( this, v, null );
							}
						}
					}
				}
				catch { }
			}

			public void Save( TextBlock block )
			{
				var type = MetadataManager.GetTypeOfNetType( typeof( FilterSettingsClass ) );
				foreach( var member in type.MetadataGetMembers() )
				{
					var p = member as Metadata.Property;
					if( p != null && !p.ReadOnly )
						block.SetAttribute( p.Name, p.GetValue( this, null ).ToString() );
				}
			}

			public bool Equals( FilterSettingsClass other )
			{
				var type = MetadataManager.GetTypeOfNetType( typeof( FilterSettingsClass ) );
				foreach( var member in type.MetadataGetMembers() )
				{
					var p = member as Metadata.Property;
					if( p != null && !p.ReadOnly )
					{
						var v1 = p.GetValue( this, null );
						var v2 = p.GetValue( other, null );
						if( !v1.Equals( v2 ) )
							return false;
					}
				}
				return true;
			}
		}

		///////////////////////////////////////////////

		//public class FullPackageInfoClass
		//{
		//	zzzzzzz;

		//	Dictionary<string, Item> packages = new Dictionary<string, Item>();

		//	/////////////////////

		//	class Item
		//	{
		//		public string packageId;

		//		public volatile HttpWebRequest webRequest;
		//		public volatile HttpWebResponse webResponse;

		//		zzzzzzz;

		//			//PackageManager.PackageInfo

		//		public volatile Image sourceImage;
		//		public volatile Image squareImage;

		//		public volatile bool disposed;

		//		public double lastUsedTime;

		//		//

		//		public void Dispose()
		//		{
		//			disposed = true;

		//			try
		//			{
		//				webResponse?.Close();
		//				webRequest?.Abort();
		//			}
		//			catch { }

		//			try
		//			{
		//				sourceImage?.Dispose();
		//				squareImage?.Dispose();
		//			}
		//			catch { }
		//		}
		//	}

		//	/////////////////////

		//	Item GetItem( string url )
		//	{
		//		if( !items.TryGetValue( url, out var item ) )
		//		{
		//			item = new Item();
		//			item.url = url;
		//			items[ url ] = item;

		//			var thread1 = new Thread( ThreadGetItemImage );
		//			commandThread.IsBackground = true;
		//			thread1.Start( item );
		//		}

		//		return item;
		//	}

		//	public Image GetSourceImage( string url, double currentTime )
		//	{
		//		var item = GetItem( url );
		//		if( item.sourceImage != null && item.sourceImage.PixelFormat != System.Drawing.Imaging.PixelFormat.DontCare )
		//		{
		//			item.lastUsedTime = currentTime;
		//			return item.sourceImage;
		//		}
		//		return null;
		//	}

		//	public Image GetSquareImage( string url, double currentTime )
		//	{
		//		var item = GetItem( url );
		//		if( item.squareImage != null && item.squareImage.PixelFormat != System.Drawing.Imaging.PixelFormat.DontCare )
		//		{
		//			item.lastUsedTime = currentTime;
		//			return item.squareImage;
		//		}
		//		return null;
		//	}

		//	void ThreadGetItemImage( object tag )
		//	{
		//		var item = (Item)tag;

		//		try
		//		{
		//			var request = (HttpWebRequest)WebRequest.Create( item.url );
		//			if( item.disposed )
		//				return;
		//			item.webRequest = request;

		//			using( var response = (HttpWebResponse)request.GetResponse() )
		//			{
		//				if( item.disposed )
		//					return;
		//				item.webResponse = response;

		//				using( var stream = response.GetResponseStream() )
		//				{
		//					var sourceImage = Image.FromStream( stream, true, true );

		//					var size = Math.Min( sourceImage.Width, sourceImage.Height );
		//					//!!!!
		//					if( size > 512 )
		//						size = 512;

		//					var squareImage = new Bitmap( size, size, sourceImage.PixelFormat );
		//					using( var g = Graphics.FromImage( squareImage ) )
		//					{
		//						g.DrawImage( sourceImage, new System.Drawing.Rectangle( 0, 0, size, size ), new System.Drawing.Rectangle( ( sourceImage.Width - size ) / 2, ( sourceImage.Height - size ) / 2, size, size ), GraphicsUnit.Pixel );
		//					}

		//					if( item.disposed )
		//						return;

		//					item.sourceImage = sourceImage;
		//					item.squareImage = squareImage;
		//				}
		//			}
		//		}
		//		catch
		//		{
		//		}
		//		finally
		//		{
		//			item.webRequest = null;
		//			item.webResponse = null;
		//		}
		//	}

		//	public void Clear()
		//	{
		//		foreach( var item in items.Values )
		//			item.Dispose();
		//		items.Clear();
		//	}

		//	public void DeleteItemsNotUsedForLongTime()
		//	{
		//		var time = EngineApp.GetSystemTime();

		//		var toDelete = new List<Item>();
		//		foreach( var item in items.Values )
		//		{
		//			if( item.lastUsedTime != 0 && item.lastUsedTime + 30 < time )
		//				toDelete.Add( item );
		//		}

		//		foreach( var item in toDelete )
		//		{
		//			items.Remove( item.url );
		//			item.Dispose();
		//		}
		//	}
		//}


		///////////////////////////////////////////////

		public static List<StoreItem> Stores
		{
			get { return stores; }
		}

		public static ImageManagerClass ImageManager
		{
			get { return imageManager; }
		}

		//public static FullPackageInfoClass FullPackageInfo
		//{
		//	get { return fullPackageInfo; }
		//}

		public static void RegisterStore( StoreItem storeItem )
		{
			storeItem.IconScaled = RenderStandard.GetImageForDispalyScale( storeItem.Icon16, storeItem.Icon32 );
			storeItem.Implementation.store = storeItem;

			stores.Add( storeItem );
		}

		internal static void Init()
		{
			RegisterStore( new StoreItem() { Name = "NeoAxis Store", Website = EngineInfo.StoreAddress + "/", Icon16 = Properties.Resources.NeoAxis_16, Icon32 = Properties.Resources.NeoAxis_32, Implementation = new NeoAxisStoreImplementation() } );
		}

		internal static void Shutdown()
		{
			foreach( var store in stores )
				store.Implementation.Shutdown();
		}

		public static void SetDownloadedListOfPackages( StoreItem store, List<PackageManager.PackageInfo> list )
		{
			var storeIndex = Stores.IndexOf( store );

			lock( downloadedListOfPackages )
			{
				while( storeIndex >= downloadedListOfPackages.Count )
					downloadedListOfPackages.Add( new Dictionary<string, PackageManager.PackageInfo>() );

				var storeList = downloadedListOfPackages[ storeIndex ];

				storeList.Clear();
				foreach( var package in list )
					storeList[ package.Identifier ] = package;
			}

			//lock( downloadedListOfPackages )
			//{
			//	downloadedListOfPackages.Clear();
			//	foreach( var package in list )
			//		downloadedListOfPackages[ package.Identifier ] = package;
			//}

			//called from thread
			DownloadedListOfPackagesUpdated?.Invoke();
		}

		public static void ClearDownloadedListOfPackages()
		{
			lock( downloadedListOfPackages )
				downloadedListOfPackages.Clear();

			DownloadedListOfPackagesUpdated?.Invoke();
		}

		public static string[] GetPackages( StoreItem specifiedStore = null )
		{
			var result = new ESet<string>();

			lock( downloadedListOfPackages )
			{
				foreach( var packages in downloadedListOfPackages )
				{
					foreach( var package in packages.Values )
					{
						if( specifiedStore == null || specifiedStore == package.Store )
							result.AddWithCheckAlreadyContained( package.Identifier );
					}

					//result.AddRangeWithCheckAlreadyContained( packages.Keys );
				}
			}

			return result.ToArray();

			//lock( downloadedListOfPackages )
			//	return downloadedListOfPackages.Keys.ToArray();
		}

		public static PackageManager.PackageInfo GetPackageInfo( string packageId, bool needDetailedInfo )
		{
			//!!!!needDetailedInfo

			lock( downloadedListOfPackages )
			{
				foreach( var packages in downloadedListOfPackages )
				{
					if( packages.TryGetValue( packageId, out var package ) )
						return package;
				}
			}

			return null;

			//lock( downloadedListOfPackages )
			//{
			//	if( downloadedListOfPackages.TryGetValue( packageId, out var package ) )
			//		return package;
			//	else
			//		return null;
			//}
		}

		public static StoreItem GetPackageStore( string packageId )
		{
			lock( downloadedListOfPackages )
			{
				for( int n = 0; n < downloadedListOfPackages.Count; n++ )
				{
					var packages = downloadedListOfPackages[ n ];
					if( packages.ContainsKey( packageId ) )
						return stores[ n ];
				}
			}
			return null;
		}

		public static StoreItem DefaultStore
		{
			get { return stores[ 0 ]; }
		}

		public static StoreItem GetStore( string name )
		{
			return Stores.FirstOrDefault( s => s.Name == name );
		}
	}
}

#endif