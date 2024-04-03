#if CLOUD
#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using NeoAxis.Networking;

namespace NeoAxis.Editor
{
	public class LauncherEditRepositoryContentBrowserItem : ContentBrowser.Item
	{
		LauncherEditRepositoryForm launcherEditRepositoryForm;

		string fullPath;
		string text;
		bool isDirectory;

		EDictionary<string, ContentBrowser.Item> fileChildren = new EDictionary<string, ContentBrowser.Item>();
		//EDictionary<Component, ContentBrowser.Item> componentChildren = new EDictionary<Component, ContentBrowser.Item>();
		//EDictionary<Metadata.Member, ContentBrowser.Item> memberChildren = new EDictionary<Metadata.Member, ContentBrowser.Item>();

		//

		public LauncherEditRepositoryContentBrowserItem( LauncherEditRepositoryForm launcherEditRepositoryForm, ContentBrowser owner, ContentBrowser.Item parent, string fullPath, bool isDirectory )
			: base( owner, parent )
		{
			this.launcherEditRepositoryForm = launcherEditRepositoryForm;
			this.fullPath = fullPath;

			text = Path.GetFileName( fullPath );
			if( string.IsNullOrEmpty( text ) )
				text = fullPath;
			this.isDirectory = isDirectory;

			//UpdateShowDisabledFlag( true );
		}

		public override void Dispose()
		{
			foreach( var item in fileChildren.Values )
				item.Dispose();
			fileChildren.Clear();

			//foreach( var item in componentChildren.Values )
			//	item.Dispose();
			//componentChildren.Clear();

			//foreach( var item in memberChildren.Values )
			//	item.Dispose();
			//memberChildren.Clear();
		}

		public string FullPath
		{
			get { return fullPath; }
		}

		public override string Text
		{
			get { return text; }
		}

		public void SetText( string text )
		{
			this.text = text;
		}

		public bool IsDirectory
		{
			get { return isDirectory; }
		}

		public static string GetFileChildrenKey( string path )
		{
			return VirtualPathUtility.NormalizePath( path );
		}

		//void GetDirectorySettings( out ESet<string> itemsToHide )
		//{
		//	itemsToHide = new ESet<string>();

		//	itemsToHide.Add( ".Directory.settings".ToLower() );

		//	//!!!!!errors, etc

		//	string filePath = Path.Combine( fullPath, ".Directory.settings" );
		//	if( File.Exists( filePath ) )
		//	{
		//		var rootBlock = TextBlockUtility.LoadFromRealFile( filePath );
		//		if( rootBlock != null )
		//		{
		//			foreach( var block in rootBlock.Children )
		//			{
		//				if( block.Name == "Item" )
		//				{
		//					string itemName = block.Data;
		//					bool hide = bool.Parse( block.GetAttribute( "Hide", "False" ) );

		//					if( hide )
		//						itemsToHide.Add( itemName.ToLower() );
		//				}
		//			}
		//		}
		//	}
		//}

		public EDictionary<string, ContentBrowser.Item> FileChildren
		{
			get { return fileChildren; }
		}

		//public EDictionary<Component, ContentBrowser.Item> ComponentChildren
		//{
		//	get { return componentChildren; }
		//}

		void SortDirectories( List<(string, LauncherEditRepositoryContentBrowserItem)> list )
		{
			CollectionUtility.MergeSort( list, delegate ( (string, LauncherEditRepositoryContentBrowserItem) tuple1, (string, LauncherEditRepositoryContentBrowserItem) tuple2 )
			{
				var item1 = tuple1.Item2;
				var item2 = tuple2.Item2;

				var fileName1 = Path.GetFileName( item1.FullPath );
				var fileName2 = Path.GetFileName( item2.FullPath );
				var result = string.Compare( fileName1, fileName2 );
				if( !Owner.Options.SortFilesByAscending )
					result *= -1;

				return result;
			} );
		}

		void SortFiles( List<(string, LauncherEditRepositoryContentBrowserItem)> list )
		{
			CollectionUtility.MergeSort( list, delegate ( (string, LauncherEditRepositoryContentBrowserItem) tuple1, (string, LauncherEditRepositoryContentBrowserItem) tuple2 )
			{
				var item1 = tuple1.Item2;
				var item2 = tuple2.Item2;

				switch( Owner.Options.SortFilesBy )
				{
				case ContentBrowser.SortByItems.Date:
					{
						DateTime GetDate( LauncherEditRepositoryContentBrowserItem item )
						{
							try
							{
								return File.GetLastWriteTime( item.fullPath );
							}
							catch
							{
								return new DateTime();
							}
						}

						var date1 = GetDate( item1 );
						var date2 = GetDate( item2 );
						int result = 0;
						if( date1 < date2 )
							result = -1;
						else if( date1 > date2 )
							result = 1;
						if( !Owner.Options.SortFilesByAscending )
							result *= -1;
						if( result != 0 )
							return result;
					}
					break;

				case ContentBrowser.SortByItems.Type:
					{
						var extension1 = Path.GetExtension( item1.FullPath );
						var extension2 = Path.GetExtension( item2.FullPath );
						var result = string.Compare( extension1, extension2 );
						if( !Owner.Options.SortFilesByAscending )
							result *= -1;
						if( result != 0 )
							return result;
					}
					break;

				case ContentBrowser.SortByItems.Size:
					{
						long GetSize( LauncherEditRepositoryContentBrowserItem item )
						{
							try
							{
								return new FileInfo( item.fullPath ).Length;
							}
							catch
							{
								return 0;
							}
						}

						long size1 = GetSize( item1 );
						long size2 = GetSize( item2 );
						int result = 0;
						if( size1 < size2 )
							result = -1;
						else if( size1 > size2 )
							result = 1;
						if( !Owner.Options.SortFilesByAscending )
							result *= -1;
						if( result != 0 )
							return result;
					}
					break;
				}

				//by name if same
				{
					var fileName1 = Path.GetFileName( item1.FullPath );
					var fileName2 = Path.GetFileName( item2.FullPath );
					var result = string.Compare( fileName1, fileName2 );
					if( !Owner.Options.SortFilesByAscending )
						result *= -1;
					return result;
				}
			} );
		}

		void UpdateChildren()
		{
			//update what need
			if( isDirectory )
			{
				////read .Directory.settings
				//GetDirectorySettings( out ESet<string> itemsToHide );

				var newChildrenDirectories = new List<(string, LauncherEditRepositoryContentBrowserItem)>();
				var newChildrenFiles = new List<(string, LauncherEditRepositoryContentBrowserItem)>();

				//add (with new order)
				{
					//directories
					string[] dirs;
					try
					{
						dirs = Directory.GetDirectories( fullPath );
					}
					catch
					{
						dirs = new string[ 0 ];
					}

					foreach( var fullPath in dirs )
					{
						string name = Path.GetFileName( fullPath );
						//if( !itemsToHide.Contains( name.ToLower() ) )
						{
							string key = GetFileChildrenKey( fullPath );

							fileChildren.TryGetValue( key, out ContentBrowser.Item item );

							if( item == null )
							{
								item = new LauncherEditRepositoryContentBrowserItem( launcherEditRepositoryForm, Owner, this, fullPath, true );
								item.imageKey = "Folder";
							}

							newChildrenDirectories.Add( (key, (LauncherEditRepositoryContentBrowserItem)item) );
						}
					}

					//files
					string[] files;
					try
					{
						var searchPatterns = Owner.FilteringMode?.FileSearchPatterns;
						if( searchPatterns != null )
						{
							var list = new List<string>( 256 );
							foreach( var pattern in searchPatterns )
								list.AddRange( Directory.GetFiles( fullPath, pattern ) );
							files = list.ToArray();
						}
						else
							files = Directory.GetFiles( fullPath );
					}
					catch
					{
						files = new string[ 0 ];
					}

					foreach( var fullPath in files )
					{
						string name = Path.GetFileName( fullPath );
						//if( !itemsToHide.Contains( name.ToLower() ) )
						{
							string key = GetFileChildrenKey( fullPath );

							fileChildren.TryGetValue( key, out ContentBrowser.Item item );

							if( item == null )
							{
								item = new LauncherEditRepositoryContentBrowserItem( launcherEditRepositoryForm, Owner, this, fullPath, false );
								item.imageKey = ResourceManager.GetResourceImageKey( fullPath );

								//!!!!так?
								if( Owner.Mode == ContentBrowser.ModeEnum.Resources /*&&
									Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.None*/ )
									item.chooseByDoubleClickAndReturnKey = true;
							}

							newChildrenFiles.Add( (key, (LauncherEditRepositoryContentBrowserItem)item) );
						}
					}
				}

				//sort
				SortDirectories( newChildrenDirectories );
				SortFiles( newChildrenFiles );

				//get new dictionary
				var newChildren = new EDictionary<string, ContentBrowser.Item>();
				foreach( var pair in newChildrenDirectories )
					newChildren[ pair.Item1 ] = pair.Item2;
				foreach( var pair in newChildrenFiles )
					newChildren[ pair.Item1 ] = pair.Item2;

				//remove old
				{
					ESet<ContentBrowser.Item> s = new ESet<ContentBrowser.Item>( newChildren.Values );
					foreach( var item in fileChildren.Values )
					{
						if( !s.Contains( item ) )
							item.Dispose();
					}
				}

				fileChildren = newChildren;
			}
			else
			{
				//file

				//if( Owner.Mode == ContentBrowser.ModeEnum.Resources || Owner.Mode == ContentBrowser.ModeEnum.SetReference )
				//{
				//	bool gotData = false;

				//	var res = ResourceManager.GetByName( VirtualPathUtility.GetVirtualPathByReal( fullPath ) );
				//	if( res != null && res.PrimaryInstance != null )
				//	{
				//		var component = res.PrimaryInstance.ResultComponent;
				//		if( component != null )
				//		{
				//			gotData = true;

				//			//components
				//			{
				//				var newChildren = new EDictionary<Component, ContentBrowser.Item>();

				//				//add (with new order)
				//				foreach( var childComponent in component.Components )
				//				{
				//					if( childComponent.DisplayInEditor && childComponent.TypeSettingsIsPublic() && EditorUtility.PerformComponentDisplayInEditorFilter( childComponent ) )
				//					{
				//						componentChildren.TryGetValue( childComponent, out ContentBrowser.Item item );

				//						if( item == null )
				//							item = new ContentBrowserItem_Component( Owner, this, childComponent );

				//						newChildren.Add( childComponent, item );
				//					}
				//				}

				//				//remove old
				//				{
				//					ESet<ContentBrowser.Item> s = new ESet<ContentBrowser.Item>( newChildren.Values );
				//					foreach( var item in componentChildren.Values )
				//					{
				//						if( !s.Contains( item ) )
				//							item.Dispose();
				//					}
				//				}

				//				componentChildren = newChildren;
				//			}

				//			//members
				//			if( Owner.Mode == ContentBrowser.ModeEnum.Resources || Owner.Mode == ContentBrowser.ModeEnum.SetReference )
				//			{
				//				//get members
				//				var members = new List<Metadata.Member>( 256 );
				//				{
				//					foreach( var member in component.MetadataGetMembers() )
				//					{
				//						bool allow;
				//						if( Owner.Mode == ContentBrowser.ModeEnum.SetReference )
				//						{
				//							if( Owner.SetReferenceModeData.newObjectWindow || Owner.SetReferenceModeData.selectTypeWindow )
				//								allow = false;
				//							else
				//							{
				//								var type = Owner.SetReferenceModeData.DemandedType;
				//								allow = ContentBrowserUtility.ContentBrowserSetReferenceModeCheckAllowAddMember( type, member, false );
				//							}

				//							//if( member is Metadata.Property )
				//							//{
				//							//	//property
				//							//	var property = (Metadata.Property)member;
				//							//	if( !member.Static && !property.HasIndexers )
				//							//		allow = true;
				//							//}
				//							//else if( member is Metadata.Method )
				//							//{
				//							//	//method
				//							//	var method = (Metadata.Method)member;
				//							//	if( !member.Static && method.Parameters.Length == 1 && method.GetReturnParameters().Length == 1 )
				//							//		allow = true;
				//							//}
				//						}
				//						else
				//						{
				//							if( member.Owner == component )
				//								allow = true;
				//							else
				//								allow = false;
				//							//allow = true;

				//							////property
				//							//var property = member as Metadata.Property;
				//							////!!!!как везде унифицировать получаемый список нужных мемберов. в гриде тоже подобно ведь. где-то еще
				//							//if( property != null && property.Browsable && !property.HasIndexers && !property.Static )
				//							//{
				//							//	memberChildren.TryGetValue( property, out ContentBrowser.Item item );

				//							//	if( item == null )
				//							//	{
				//							//		//!!!!дальше получаем либо тип, либо объект типа
				//							//		//!!!!!!объект включает в себя мемберов типа

				//							//		item = new ContentBrowserItem_Member( Owner, this, member );
				//							//		//!!!!статичные еще
				//							//		item.imageKey = "Property";

				//							//		//item = new ContentBrowserItem_NoSpecialization( Owner, this, "property: " + p.Name, null );
				//							//	}

				//							//	newChildren.Add( property, item );
				//							//}

				//						}

				//						if( allow )
				//							members.Add( member );
				//					}

				//					//sort
				//					ContentBrowserUtility.SortMemberItems( members );
				//				}

				//				//get new list of items
				//				var newChildren = new EDictionary<Metadata.Member, ContentBrowser.Item>( 256 );
				//				foreach( var member in members )
				//				{
				//					memberChildren.TryGetValue( member, out ContentBrowser.Item item );
				//					if( item == null )
				//						item = new ContentBrowserItem_Member( Owner, this, member );

				//					if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
				//						newChildren.Add( member, item );
				//					else
				//						item.Dispose();
				//				}

				//				//remove old
				//				{
				//					ESet<ContentBrowser.Item> s = new ESet<ContentBrowser.Item>( newChildren.Values );
				//					foreach( var item in memberChildren.Values )
				//					{
				//						if( !s.Contains( item ) )
				//							item.Dispose();
				//					}
				//				}

				//				memberChildren = newChildren;
				//			}
				//		}
				//	}

				//	if( !gotData )
				//	{
				//		//remove old

				//		foreach( var item in componentChildren.Values )
				//			item.Dispose();
				//		componentChildren = new EDictionary<Component, ContentBrowser.Item>();

				//		foreach( var item in memberChildren.Values )
				//			item.Dispose();
				//		memberChildren = new EDictionary<Metadata.Member, ContentBrowser.Item>();
				//	}
				//}
			}
		}

		public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
		{
			if( !onlyAlreadyCreated )
				UpdateChildren();

			//prepare result
			if( isDirectory )
			{
				var hideDirectoriesWithoutItems = Owner.FilteringMode != null && Owner.FilteringMode.HideDirectoriesWithoutItems;
				if( hideDirectoriesWithoutItems )
				{
					List<ContentBrowser.Item> items = new List<ContentBrowser.Item>( fileChildren.Values.Count );
					foreach( var item in fileChildren.Values )
					{
						bool skip = false;

						//it is different than _File. with this code empty folders are not updated by file watcher
						//var fileItem = item as LauncherEditRepositoryContentBrowserItem;
						//if( fileItem != null && fileItem.IsDirectory && fileItem.GetChildrenFilter( false ).Count == 0 )
						//	skip = true;

						if( !skip )
							items.Add( item );
					}
					return items;
				}
				else
					return fileChildren.Values.ToArray();
			}
			else
			{
				return new List<ContentBrowser.Item>();
				//List<ContentBrowser.Item> result = new List<ContentBrowser.Item>( componentChildren.Values.Count + memberChildren.Count );
				//foreach( var item in componentChildren.Values )
				//	result.Add( item );
				//foreach( var item in memberChildren.Values )
				//	result.Add( item );
				//return result;
			}
		}

		public override object ContainedObject
		{
			get
			{
				return null;
				//var res = ResourceManager.GetByName( VirtualPathUtility.GetVirtualPathByReal( fullPath ) );
				//return res?.PrimaryInstance?.ResultComponent;
			}
		}

		//void UpdateShowDisabledFlag( bool fullUpdate )
		//{
		//	//!!!!врядли тут будет
		//	if( fullUpdate )
		//	{
		//		if( !isDirectory )
		//		{
		//			var extension = Path.GetExtension( fullPath ).ToLower();

		//			if( extension == ".settings" )
		//			{
		//				try
		//				{
		//					if( File.Exists( fullPath.Substring( 0, fullPath.Length - ".settings".Length ) ) )
		//						ShowDisabled = true;
		//				}
		//				catch { }
		//			}
		//			else if( extension == ".meta" )
		//			{
		//				try
		//				{
		//					var path = fullPath.Substring( 0, fullPath.Length - ".meta".Length );
		//					if( File.Exists( path ) || Directory.Exists( path ) )
		//						ShowDisabled = true;
		//				}
		//				catch { }
		//			}
		//			else if( extension == ".dds" )
		//			{
		//				if( fullPath.Length > 11 )
		//				{
		//					var s = fullPath.Substring( fullPath.Length - 11 );
		//					if( s == "_GenEnv.dds" || s == "_GenIrr.dds" )
		//					{
		//						var path = fullPath.Substring( 0, fullPath.Length - 11 );
		//						try
		//						{
		//							if( File.Exists( path ) )
		//								ShowDisabled = true;
		//						}
		//						catch { }
		//					}
		//				}
		//			}
		//			else if( extension == ".info" )
		//			{
		//				if( fullPath.Length > 9 )
		//				{
		//					var s = fullPath.Substring( fullPath.Length - 9 );
		//					if( s == "_Gen.info" )
		//					{
		//						var path = fullPath.Substring( 0, fullPath.Length - 9 );
		//						try
		//						{
		//							if( File.Exists( path ) )
		//								ShowDisabled = true;
		//						}
		//						catch { }
		//					}
		//				}
		//			}
		//		}
		//	}

		//	//!!!!врядли тут будет
		//	if( !isDirectory && Path.GetExtension( fullPath ).ToLower() == ".cs" )
		//	{
		//		var newValue = false;
		//		if( !CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fullPath ) )
		//			newValue = true;

		//		if( ShowDisabled != newValue )
		//		{
		//			ShowDisabled = newValue;
		//			Owner?.Invalidate( true );
		//		}
		//	}
		//}

		public override void LightweightUpdate()
		{
			base.LightweightUpdate();

			//UpdateShowDisabledFlag( false );
		}

		public override bool CanDoDragDrop() { return true; }

		public override Image[] GetCornerImages()
		{
			//if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient && Owner.Mode == ContentBrowser.ModeEnum.Resources )
			{
				var projectFileName = RepositoryUtility.GetAllFilesPathByReal( launcherEditRepositoryForm.GetProjectFolder(), fullPath );
				//var projectFileName = VirtualPathUtility.GetAllFilesPathByReal( fullPath );

				var images = new Image[ 1 ];

				if( IsDirectory )
				{
					if( RepositoryLocal.FolderContainsFileItems( projectFileName ) || RepositoryIconCache.FolderModified( projectFileName ) )
					{
						images[ 0 ] = EditorResourcesCache.Warning_32;
					}
					else
					{
						var serverStateItem = RepositoryServerState.GetFolderItem( projectFileName );
						if( serverStateItem != null )
						{
							if( serverStateItem.ContainsSyncModeServerOnly && !serverStateItem.ContainsSyncModeSynchronize )
								images[ 0 ] = EditorResourcesCache.ServerOnly_32;
							else
								images[ 0 ] = EditorResourcesCache.CheckedCircle_32;
						}
						else
							images[ 0 ] = EditorResourcesCache.NotAdded_32;
					}
				}
				else
				{
					var serverStateItem = RepositoryServerState.GetFileItem( projectFileName );
					var localItemExists = RepositoryLocal.GetFileItemData( projectFileName, out var localStatus, out var localSyncMode );

					//!!!!может _16 тоже

					if( serverStateItem != null || localItemExists )
					{
						if( localStatus.HasValue && localStatus.Value == RepositoryLocal.Status.Added )
							images[ 0 ] = EditorResourcesCache.AddCircle_32;
						else if( localStatus.HasValue && localStatus.Value == RepositoryLocal.Status.Deleted )
							images[ 0 ] = EditorResourcesCache.DeleteCircle_32;
						else
						{
							var modified = false;
							if( serverStateItem != null )
							{
								//check for modified data of the file
								var iconsItem = RepositoryIconCache.GetFileItem( projectFileName );
								var equal = iconsItem != null && serverStateItem.Length == iconsItem.Length && serverStateItem.Hash == iconsItem.Hash;
								if( !equal )
									modified = true;

								//changed sync mode is also considered as modified
								if( localSyncMode.HasValue && serverStateItem.SyncMode != localSyncMode.Value )
									modified = true;
							}

							if( modified )
								images[ 0 ] = EditorResourcesCache.Warning_32;
							else
							{
								var syncMode = RepositorySyncMode.Synchronize;
								if( localSyncMode.HasValue )
									syncMode = localSyncMode.Value;
								else if( serverStateItem != null )
									syncMode = serverStateItem.SyncMode;

								if( syncMode == RepositorySyncMode.ServerOnly )
									images[ 0 ] = EditorResourcesCache.ServerOnly_32;
								else
									images[ 0 ] = EditorResourcesCache.CheckedCircle_32;
							}
						}
					}
					else
						images[ 0 ] = EditorResourcesCache.NotAdded_32;
				}

				return images;
			}


			//if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient && Owner.Mode == ContentBrowser.ModeEnum.Resources )
			//{
			//	var projectFileName = VirtualPathUtility.GetAllFilesPathByReal( fullPath );

			//	var images = new Image[ 1 ];

			//	if( IsDirectory )
			//	{
			//		if( RepositoryLocal.FolderContainsFileItems( projectFileName ) || RepositoryIconCache.FolderModified( projectFileName ) )
			//		{
			//			images[ 0 ] = EditorResourcesCache.Warning_32;
			//		}
			//		else
			//		{
			//			var serverStateItem = RepositoryServerState.GetFolderItem( projectFileName );
			//			if( serverStateItem != null )
			//			{
			//				if( serverStateItem.ContainsSyncModeServerOnly && !serverStateItem.ContainsSyncModeSynchronize )
			//					images[ 0 ] = EditorResourcesCache.ServerOnly_32;
			//				else
			//					images[ 0 ] = EditorResourcesCache.CheckedCircle_32;
			//			}
			//			else
			//				images[ 0 ] = EditorResourcesCache.NotAdded_32;
			//		}
			//	}
			//	else
			//	{
			//		var serverStateItem = RepositoryServerState.GetFileItem( projectFileName );
			//		var localItemExists = RepositoryLocal.GetFileItemData( projectFileName, out var localStatus, out var localSyncMode );

			//		//!!!!может _16 тоже

			//		if( serverStateItem != null || localItemExists )
			//		{
			//			if( localStatus.HasValue && localStatus.Value == RepositoryLocal.Status.Added )
			//				images[ 0 ] = EditorResourcesCache.AddCircle_32;
			//			else if( localStatus.HasValue && localStatus.Value == RepositoryLocal.Status.Deleted )
			//				images[ 0 ] = EditorResourcesCache.DeleteCircle_32;
			//			else
			//			{
			//				var modified = false;
			//				if( serverStateItem != null )
			//				{
			//					//check for modified data of the file
			//					var iconsItem = RepositoryIconCache.GetFileItem( projectFileName );
			//					var equal = iconsItem != null && serverStateItem.Length == iconsItem.Length && serverStateItem.Hash == iconsItem.Hash;
			//					if( !equal )
			//						modified = true;

			//					//changed sync mode is also considered as modified
			//					if( localSyncMode.HasValue && serverStateItem.SyncMode != localSyncMode.Value )
			//						modified = true;
			//				}

			//				if( modified )
			//					images[ 0 ] = EditorResourcesCache.Warning_32;
			//				else
			//				{
			//					var syncMode = RepositorySyncMode.Synchronize;
			//					if( localSyncMode.HasValue )
			//						syncMode = localSyncMode.Value;
			//					else if( serverStateItem != null )
			//						syncMode = serverStateItem.SyncMode;

			//					if( syncMode == RepositorySyncMode.ServerOnly )
			//						images[ 0 ] = EditorResourcesCache.ServerOnly_32;
			//					else
			//						images[ 0 ] = EditorResourcesCache.CheckedCircle_32;
			//				}
			//			}
			//		}
			//		else
			//			images[ 0 ] = EditorResourcesCache.NotAdded_32;
			//	}

			//	return images;
			//}

			//return base.GetCornerImages();
		}
	}
}
#endif
#endif