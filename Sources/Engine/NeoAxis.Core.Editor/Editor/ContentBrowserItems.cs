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
	public class ContentBrowserItem_File : ContentBrowser.Item
	{
		string fullPath;
		string text;
		bool isDirectory;

		EDictionary<string, ContentBrowser.Item> fileChildren = new EDictionary<string, ContentBrowser.Item>();
		EDictionary<Component, ContentBrowser.Item> componentChildren = new EDictionary<Component, ContentBrowser.Item>();
		EDictionary<Metadata.Member, ContentBrowser.Item> memberChildren = new EDictionary<Metadata.Member, ContentBrowser.Item>();

		//public bool canExpandDeeper;

		//

		public ContentBrowserItem_File( ContentBrowser owner, ContentBrowser.Item parent, string fullPath, bool isDirectory )//, bool canExpandDeeper )
			: base( owner, parent )
		{
			this.fullPath = fullPath;

			//!!!!!rename Data -> Files

			text = Path.GetFileName( fullPath );
			if( string.IsNullOrEmpty( text ) )
				text = fullPath;
			this.isDirectory = isDirectory;

			//!!!!!
			//this.canExpandDeeper = canExpandDeeper;

			UpdateShowDisabledFlag( true );
		}

		public override void Dispose()
		{
			foreach( var item in fileChildren.Values )
				item.Dispose();
			fileChildren.Clear();

			foreach( var item in componentChildren.Values )
				item.Dispose();
			componentChildren.Clear();

			foreach( var item in memberChildren.Values )
				item.Dispose();
			memberChildren.Clear();
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
			//!!!!new
			return VirtualPathUtility.NormalizePath( path );
			//return VirtualPathUtility.NormalizePath( path.ToLower() );
		}

		void GetDirectorySettings( out ESet<string> itemsToHide )
		{
			itemsToHide = new ESet<string>();

			itemsToHide.Add( ".Directory.settings".ToLower() );

			//!!!!!errors, etc

			string filePath = Path.Combine( fullPath, ".Directory.settings" );
			if( File.Exists( filePath ) )
			{
				var rootBlock = TextBlockUtility.LoadFromRealFile( filePath );
				if( rootBlock != null )
				{
					foreach( var block in rootBlock.Children )
					{
						if( block.Name == "Item" )
						{
							string itemName = block.Data;
							bool hide = bool.Parse( block.GetAttribute( "Hide", "False" ) );

							if( hide )
								itemsToHide.Add( itemName.ToLower() );
						}
					}
				}
			}
		}

		//!!!!!

		public EDictionary<string, ContentBrowser.Item> FileChildren
		{
			get { return fileChildren; }
		}

		public EDictionary<Component, ContentBrowser.Item> ComponentChildren
		{
			get { return componentChildren; }
		}

		void SortDirectories( List<(string, ContentBrowserItem_File)> list )
		{
			CollectionUtility.MergeSort( list, delegate ( (string, ContentBrowserItem_File) tuple1, (string, ContentBrowserItem_File) tuple2 )
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

		void SortFiles( List<(string, ContentBrowserItem_File)> list )
		{
			CollectionUtility.MergeSort( list, delegate ( (string, ContentBrowserItem_File) tuple1, (string, ContentBrowserItem_File) tuple2 )
			{
				var item1 = tuple1.Item2;
				var item2 = tuple2.Item2;

				switch( Owner.Options.SortFilesBy )
				{
				case ContentBrowser.SortByItems.Date:
					{
						DateTime GetDate( ContentBrowserItem_File item )
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
						long GetSize( ContentBrowserItem_File item )
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
				//read .Directory.settings
				GetDirectorySettings( out ESet<string> itemsToHide );

				//EDictionary<string, ContentBrowser.Item> newChildren = new EDictionary<string, ContentBrowser.Item>();
				var newChildrenDirectories = new List<(string, ContentBrowserItem_File)>();
				var newChildrenFiles = new List<(string, ContentBrowserItem_File)>();

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
						if( !itemsToHide.Contains( name.ToLower() ) )
						{
							string key = GetFileChildrenKey( fullPath );

							fileChildren.TryGetValue( key, out ContentBrowser.Item item );

							if( item == null )
							{
								item = new ContentBrowserItem_File( Owner, this, fullPath, true );
								item.imageKey = "Folder";
							}

							newChildrenDirectories.Add( (key, (ContentBrowserItem_File)item) );
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
						if( !itemsToHide.Contains( name.ToLower() ) )
						{
							string key = GetFileChildrenKey( fullPath );

							fileChildren.TryGetValue( key, out ContentBrowser.Item item );

							if( item == null )
							{
								item = new ContentBrowserItem_File( Owner, this, fullPath, false );
								item.imageKey = ResourceManager.GetResourceImageKey( fullPath );

								//!!!!так?
								if( Owner.Mode == ContentBrowser.ModeEnum.Resources /*&&
									Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.None*/ )
									item.chooseByDoubleClickAndReturnKey = true;
							}

							newChildrenFiles.Add( (key, (ContentBrowserItem_File)item) );
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

				//string[] dirs = Directory.GetDirectories( fullPath );
				//foreach( var dir in dirs )
				//{
				//	string name = Path.GetFileName( dir );
				//	if( !itemsToHide.Contains( name.ToLower() ) )
				//	{
				//		var child = new ContentBrowser_FileItem( Owner, this, dir, true );
				//		newChildren.Add( dir, child );
				//	}
				//}

				//string[] files = Directory.GetFiles( fullPath );
				//foreach( var file in files )
				//{
				//	string name = Path.GetFileName( file );
				//	if( !itemsToHide.Contains( name.ToLower() ) )
				//	{
				//		var child = new ContentBrowser_FileItem( Owner, this, file, false );
				//		newChildren.Add( file, child );
				//	}
				//}

				fileChildren = newChildren;


				////add (with new order)
				//foreach( var childComponent in component.Components )
				//{
				//	children.TryGetValue( childComponent, out ContentBrowser.Item item );

				//	if( item == null )
				//		item = new ContentBrowser_ComponentItem( Owner, this, childComponent );

				//	newChildren.Add( childComponent, item );
				//}

				////remove old
				//{
				//	ESet<ContentBrowser.Item> s = new ESet<ContentBrowser.Item>( newChildren.Values );
				//	foreach( var item in children.Values )
				//	{
				//		if( !s.Contains( item ) )
				//			item.Dispose();
				//	}
				//}
			}
			else
			{
				//file

				if( Owner.Mode == ContentBrowser.ModeEnum.Resources /*&&
					( Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.None ||
					Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.Type ||
					Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.Member )*/ ||
					Owner.Mode == ContentBrowser.ModeEnum.SetReference )
				{
					bool gotData = false;

					var res = ResourceManager.GetByName( VirtualPathUtility.GetVirtualPathByReal( fullPath ) );
					if( res != null && res.PrimaryInstance != null )
					{
						var component = res.PrimaryInstance.ResultComponent;
						if( component != null )
						{
							gotData = true;

							//components
							{
								var newChildren = new EDictionary<Component, ContentBrowser.Item>();

								//add (with new order)
								foreach( var childComponent in component.Components )
								{
									if( childComponent.DisplayInEditor && childComponent.TypeSettingsIsPublic() && EditorUtility.PerformComponentDisplayInEditorFilter( childComponent ) )
									{
										//bool skip = false;

										////Type Settings filter
										//var baseComponentType = component.BaseType as Metadata.ComponentTypeInfo;
										//if( baseComponentType != null && ComponentUtility.TypeSettingsCheckHideObject( baseComponentType.BasedOnObject, true, childComponent ) )
										//	skip = true;
										////if( baseComponentType != null && ComponentUtils.TypeSettingsCheckHideObject( component, true, childComponent ) )
										////	skip = true;

										//if( !skip )
										//{
										componentChildren.TryGetValue( childComponent, out ContentBrowser.Item item );

										if( item == null )
										{
											item = new ContentBrowserItem_Component( Owner, this, childComponent );

											//if( string.IsNullOrEmpty( item.imageKey ) )
											//	item.imageKey = "Class";
										}

										newChildren.Add( childComponent, item );
										//}
									}
								}

								//remove old
								{
									ESet<ContentBrowser.Item> s = new ESet<ContentBrowser.Item>( newChildren.Values );
									foreach( var item in componentChildren.Values )
									{
										if( !s.Contains( item ) )
											item.Dispose();
									}
								}

								componentChildren = newChildren;
							}

							//members
							if( Owner.Mode == ContentBrowser.ModeEnum.Resources /*&&
								( Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.None ||
								Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.Member )*/ ||
								Owner.Mode == ContentBrowser.ModeEnum.SetReference )
							{
								//get members
								var members = new List<Metadata.Member>( 256 );
								{
									foreach( var member in component.MetadataGetMembers() )
									{
										bool allow;
										if( Owner.Mode == ContentBrowser.ModeEnum.SetReference )
										{
											if( Owner.SetReferenceModeData.newObjectWindow || Owner.SetReferenceModeData.selectTypeWindow )
												allow = false;
											else
											{
												var type = Owner.SetReferenceModeData.DemandedType;
												allow = ContentBrowserUtility.ContentBrowserSetReferenceModeCheckAllowAddMember( type, member, false );
											}

											//if( member is Metadata.Property )
											//{
											//	//property
											//	var property = (Metadata.Property)member;
											//	if( !member.Static && !property.HasIndexers )
											//		allow = true;
											//}
											//else if( member is Metadata.Method )
											//{
											//	//method
											//	var method = (Metadata.Method)member;
											//	if( !member.Static && method.Parameters.Length == 1 && method.GetReturnParameters().Length == 1 )
											//		allow = true;
											//}
										}
										else
										{
											if( member.Owner == component )
												allow = true;
											else
												allow = false;
											//allow = true;

											////property
											//var property = member as Metadata.Property;
											////!!!!как везде унифицировать получаемый список нужных мемберов. в гриде тоже подобно ведь. где-то еще
											//if( property != null && property.Browsable && !property.HasIndexers && !property.Static )
											//{
											//	memberChildren.TryGetValue( property, out ContentBrowser.Item item );

											//	if( item == null )
											//	{
											//		//!!!!дальше получаем либо тип, либо объект типа
											//		//!!!!!!объект включает в себя мемберов типа

											//		item = new ContentBrowserItem_Member( Owner, this, member );
											//		//!!!!статичные еще
											//		item.imageKey = "Property";

											//		//item = new ContentBrowserItem_NoSpecialization( Owner, this, "property: " + p.Name, null );
											//	}

											//	newChildren.Add( property, item );
											//}

										}

										if( allow )
											members.Add( member );
									}

									//sort
									ContentBrowserUtility.SortMemberItems( members );
								}

								//get new list of items
								var newChildren = new EDictionary<Metadata.Member, ContentBrowser.Item>( 256 );
								foreach( var member in members )
								{
									memberChildren.TryGetValue( member, out ContentBrowser.Item item );
									if( item == null )
										item = new ContentBrowserItem_Member( Owner, this, member );

									if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
										newChildren.Add( member, item );
									else
										item.Dispose();
								}

								//remove old
								{
									ESet<ContentBrowser.Item> s = new ESet<ContentBrowser.Item>( newChildren.Values );
									foreach( var item in memberChildren.Values )
									{
										if( !s.Contains( item ) )
											item.Dispose();
									}
								}

								memberChildren = newChildren;
							}
						}
					}

					if( !gotData )
					{
						//remove old

						foreach( var item in componentChildren.Values )
							item.Dispose();
						componentChildren = new EDictionary<Component, ContentBrowser.Item>();

						foreach( var item in memberChildren.Values )
							item.Dispose();
						memberChildren = new EDictionary<Metadata.Member, ContentBrowser.Item>();
					}
				}
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

						var fileItem = item as ContentBrowserItem_File;
						if( fileItem != null && fileItem.IsDirectory && fileItem.GetChildrenFilter( false ).Count == 0 )
							skip = true;

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
				List<ContentBrowser.Item> result = new List<ContentBrowser.Item>( componentChildren.Values.Count + memberChildren.Count );
				foreach( var item in componentChildren.Values )
					result.Add( item );
				foreach( var item in memberChildren.Values )
					result.Add( item );
				return result;
			}
		}

		public override void CalculateReferenceValue( Component from, Metadata.TypeInfo expectedType, out string referenceValue, out bool canSet )
		{
			referenceValue = "";
			canSet = false;

			if( !IsDirectory )
			{
				//file

				referenceValue = VirtualPathUtility.GetVirtualPathByReal( FullPath );

				//specialization for ReferenceValueType_Resource
				if( MetadataManager.GetTypeOfNetType( typeof( ReferenceValueType_Resource ) ).IsAssignableFrom( expectedType ) )
				{
					canSet = true;
				}
				else
				{
					//default behaviour

					//!!!!должен быть загружен

					//!!!!не делать предпросмотр для карты, т.к. долго. что еще?
					var ext = Path.GetExtension( FullPath );
					if( ResourceManager.GetTypeByFileExtension( ext ) != null )//!!!!надо?
					{
						var res = ResourceManager.GetByName( referenceValue );
						if( res != null && res.PrimaryInstance != null )
						{
							var obj = res.PrimaryInstance.ResultComponent;
							if( obj != null )
							{
								//!!!!

								//specialization for Metadata.TypeInfo
								if( MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ).IsAssignableFrom( expectedType ) )
								{
									canSet = true;
								}
								else
								{
									//specialization for Import3D
									if( obj is Import3D && expectedType == MetadataManager.GetTypeOfNetType( typeof( Mesh ) ) )
									{
										var mesh = obj.GetComponent( "Mesh" ) as Mesh;
										if( mesh != null )
										{
											canSet = true;
											referenceValue += "|$Mesh";
										}
									}

									//default behaviour
									if( ReferenceUtility.CanMakeReferenceToObjectWithType( expectedType, MetadataManager.MetadataGetType( obj ) ) )
										canSet = true;
								}
							}
						}
					}
				}
			}
			else
			{
				//directory. need return value to gradual open nodes selection.
				referenceValue = VirtualPathUtility.GetVirtualPathByReal( FullPath );
			}
		}

		public override object ContainedObject
		{
			get
			{
				var res = ResourceManager.GetByName( VirtualPathUtility.GetVirtualPathByReal( fullPath ) );
				return res?.PrimaryInstance?.ResultComponent;
			}
		}

		void UpdateShowDisabledFlag( bool fullUpdate )
		{
			//!!!!врядли тут будет
			if( fullUpdate )
			{
				if( !isDirectory )
				{
					var extension = Path.GetExtension( fullPath ).ToLower();

					if( extension == ".settings" )
					{
						try
						{
							if( File.Exists( fullPath.Substring( 0, fullPath.Length - ".settings".Length ) ) )
								ShowDisabled = true;
						}
						catch { }
					}
					else if( extension == ".meta" )
					{
						try
						{
							var path = fullPath.Substring( 0, fullPath.Length - ".meta".Length );
							if( File.Exists( path ) || Directory.Exists( path ) )
								ShowDisabled = true;
						}
						catch { }
					}
					else if( extension == ".dds" )
					{
						if( fullPath.Length > 11 )
						{
							var s = fullPath.Substring( fullPath.Length - 11 );
							if( s == "_GenEnv.dds" || s == "_GenIrr.dds" )
							{
								var path = fullPath.Substring( 0, fullPath.Length - 11 );
								try
								{
									if( File.Exists( path ) )
										ShowDisabled = true;
								}
								catch { }
							}
						}
					}
					else if( extension == ".info" )
					{
						if( fullPath.Length > 9 )
						{
							var s = fullPath.Substring( fullPath.Length - 9 );
							if( s == "_Gen.info" )
							{
								var path = fullPath.Substring( 0, fullPath.Length - 9 );
								try
								{
									if( File.Exists( path ) )
										ShowDisabled = true;
								}
								catch { }
							}
						}
					}
					else if( extension == ".bin" )
					{
						ShowDisabled = true;
					}
				}
			}

			//!!!!врядли тут будет
			if( !isDirectory && Path.GetExtension( fullPath ).ToLower() == ".cs" )
			{
				var newValue = false;
				if( !CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fullPath ) )
					newValue = true;

				if( ShowDisabled != newValue )
				{
					ShowDisabled = newValue;
					Owner?.Invalidate( true );
				}
			}
		}

		public override void LightweightUpdate()
		{
			base.LightweightUpdate();

			UpdateShowDisabledFlag( false );
		}

		public override bool CanDoDragDrop() { return true; }

		public override Image[] GetCornerImages()
		{
#if CLOUD
			if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient && Owner.Mode == ContentBrowser.ModeEnum.Resources )
			{
				var projectFileName = VirtualPathUtility.GetAllFilesPathByReal( fullPath );

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


					//if( serverStateItem != null || localItemExists )
					//{
					//!!!!может _16 тоже

					//var images = new Image[ 2 ];

					//if( localStatus.HasValue && localStatus.Value == RepositoryLocal.Status.Added )
					//	images[ 0 ] = EditorResourcesCache.AddCircle_32;
					//else if( localStatus.HasValue && localStatus.Value == RepositoryLocal.Status.Deleted )
					//	images[ 0 ] = EditorResourcesCache.DeleteCircle_32;
					//else
					//{
					//	var modified = false;
					//	if( serverStateItem != null )
					//	{
					//		var iconsItem = RepositoryIconCache.GetFileItem( projectFileName );
					//		var equal = iconsItem != null && serverStateItem.Length == iconsItem.Length && serverStateItem.Hash == iconsItem.Hash;
					//		if( !equal )
					//			modified = true;
					//	}
					//	images[ 0 ] = modified ? EditorResourcesCache.Warning_32 : EditorResourcesCache.CheckedCircle_32;
					//}

					//var syncMode = RepositorySyncMode.Synchronize;
					//if( localSyncMode.HasValue )
					//	syncMode = localSyncMode.Value;
					//else if( serverStateItem != null )
					//	syncMode = serverStateItem.SyncMode;

					//switch( syncMode )
					//{
					////case RepositorySyncMode.StorageOnly: images[ 1 ] = EditorResourcesCache.StorageOnly_32; break;
					//case RepositorySyncMode.ServerOnly: images[ 1 ] = EditorResourcesCache.ServerOnly_32; break;
					//}

					//return images;

					//}

				}

				return images;
			}
#endif

			return base.GetCornerImages();
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ContentBrowserItem_Virtual : ContentBrowser.Item
	{
		//!!!!public. везде так
		public string text;
		public List<ContentBrowser.Item> children = new List<ContentBrowser.Item>();

		public string Description { get; set; } = "";

		//

		public ContentBrowserItem_Virtual( ContentBrowser owner, ContentBrowser.Item parent, string text )
			: base( owner, parent )
		{
			this.text = text;
		}

		public override void Dispose()
		{
			if( children != null )
			{
				foreach( var item in children )
					item.Dispose();
			}
		}

		public void DeleteChildren()
		{
			foreach( var item in children )
				item.Dispose();
			children.Clear();
		}

		public void DeleteChild( ContentBrowser.Item item )
		{
			item.Dispose();
			children.Remove( item );
		}

		public override string Text
		{
			get { return text; }
		}

		public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
		{
			return children;
		}

		public override string GetDescription()
		{
			return Description;
		}

		public void SetText( string value )
		{
			if( value != text )
			{
				text = value;
				PerformTextChanged();
			}
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ContentBrowserItem_Type : ContentBrowser.Item
	{
		public Metadata.TypeInfo type;
		public string text;
		public List<ContentBrowser.Item> nestedTypeChildren = new List<ContentBrowser.Item>();
		public List<ContentBrowserItem_Member> memberChildren = new List<ContentBrowserItem_Member>();

		//!!!!new
		public bool memberCreationDisable;

		//

		public ContentBrowserItem_Type( ContentBrowser owner, ContentBrowser.Item parent, Metadata.TypeInfo type, string text )
			: base( owner, parent )
		{
			this.type = type;
			this.text = text;
		}

		public override void Dispose()
		{
			foreach( var item in nestedTypeChildren )
				item.Dispose();
			nestedTypeChildren.Clear();
			foreach( var item in memberChildren )
				item.Dispose();
			memberChildren.Clear();
		}

		public void DeleteNestedTypeChild( ContentBrowser.Item item )
		{
			item.Dispose();
			nestedTypeChildren.Remove( item );
		}

		public void DeleteMemberChild( ContentBrowserItem_Member item )
		{
			item.Dispose();
			memberChildren.Remove( item );
		}

		public Metadata.TypeInfo Type
		{
			get { return type; }
		}

		public override string Text
		{
			get { return text; }
		}

		void UpdateMemberChildren()
		{
			//get members
			var members = new List<Metadata.Member>( 256 );
			{
				foreach( var member in type.MetadataGetMembers() )
				{
					bool allow;
					if( Owner.Mode == ContentBrowser.ModeEnum.SetReference )
					{
						if( Owner.SetReferenceModeData.newObjectWindow || Owner.SetReferenceModeData.selectTypeWindow )
							allow = false;
						else
						{
							var demandedType = Owner.SetReferenceModeData.DemandedType;
							allow = ContentBrowserUtility.ContentBrowserSetReferenceModeCheckAllowAddMember( demandedType, member, true );
						}
					}
					else
					{
						if( member.Owner == type )
							allow = true;
						else
							allow = false;
						//allow = true;
					}

					if( allow )
						members.Add( member );
				}

				//sort
				ContentBrowserUtility.SortMemberItems( members );
			}

			//create member items
			memberChildren.Clear();
			foreach( var member in members )
			{
				var item = new ContentBrowserItem_Member( Owner, this, member );

				if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
					memberChildren.Add( item );
				else
					item.Dispose();
			}
		}

		//public override void SetWasExpanded()
		//{
		//	base.SetWasExpanded();

		//	//!!!!не тут для поддержки рекомпиляции?

		//	//if( Owner.Mode == ContentBrowser.ModeEnum.Resources &&
		//	//	( Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.None ||
		//	//	Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.Member ) ||
		//	//	Owner.Mode == ContentBrowser.ModeEnum.SetReference )
		//	//{
		//	//	UpdateMemberChildren();
		//	//}

		//	//PerformChildrenChanged();
		//}

		public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
		{
			if( !onlyAlreadyCreated )
			{
				if( Owner.Mode == ContentBrowser.ModeEnum.Resources /*&&
					( Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.None ||
					Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.Member )*/ ||
					Owner.Mode == ContentBrowser.ModeEnum.SetReference )
				{
					if( !memberCreationDisable )
						UpdateMemberChildren();
				}
			}

			//prepare result
			List<ContentBrowser.Item> result = new List<ContentBrowser.Item>( 1 + nestedTypeChildren.Count + memberChildren.Count );
			foreach( var item in nestedTypeChildren )
				result.Add( item );
			foreach( var item in memberChildren )
				result.Add( item );
			return result;
			//return children;
		}

		public override void CalculateReferenceValue( Component from, Metadata.TypeInfo expectedType, out string referenceValue, out bool canSet )
		{
			referenceValue = Type.Name;
			canSet = false;

			//specialization for Metadata.TypeInfo
			if( MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ).IsAssignableFrom( expectedType ) )
				canSet = true;
		}

		public override string GetDescription()
		{
			var id = XmlDocumentationFiles.GetTypeId( type );
			if( !string.IsNullOrEmpty( id ) )
			{
				var v = XmlDocumentationFiles.GetMemberSummary( id );

				////remove last dot for one dot strings
				//if( v != null && v.Count( c => c == '.' ) == 1 && v[ v.Length - 1 ] == '.' )
				//	v = v.Substring( 0, v.Length - 1 );

				return v;
			}
			return "";
		}

		public override bool CanDoDragDrop() { return true; }
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ContentBrowserItem_Component : ContentBrowser.Item
	{
		Component component;
		EDictionary<Component, ContentBrowser.Item> componentChildren = new EDictionary<Component, ContentBrowser.Item>();
		EDictionary<Metadata.Member, ContentBrowser.Item> memberChildren = new EDictionary<Metadata.Member, ContentBrowser.Item>();

		string specialTextPrefix = "";
		ReferenceSelectionModeEnum referenceSelectionMode;
		ContentBrowserItem_Component thisItem;

		/////////////////////////////////////////

		public enum ReferenceSelectionModeEnum
		{
			None,
			Root,
			This,
		}

		/////////////////////////////////////////

		public ContentBrowserItem_Component( ContentBrowser owner, ContentBrowser.Item parent, Component component )
			: base( owner, parent )
		{
			this.component = component;

			component.NameChanged += NameChanged;
			component.EnabledInHierarchyChanged += EnabledInHierarchyChanged;
			component.ComponentsChanged += ComponentsChanged;

			if( !component.EnabledInHierarchy )
				ShowDisabled = true;

			ApplyImageKey();
		}

		public override void Dispose()
		{
			component.NameChanged -= NameChanged;
			component.EnabledInHierarchyChanged -= EnabledInHierarchyChanged;
			component.ComponentsChanged -= ComponentsChanged;

			foreach( var item in componentChildren.Values )
				item.Dispose();
			componentChildren.Clear();

			thisItem?.Dispose();

			foreach( var item in memberChildren.Values )
				item.Dispose();
			memberChildren.Clear();
		}

		public Component Component
		{
			get { return component; }
		}

		public override object ContainedObject
		{
			get { return component; }
		}

		public string SpecialTextPrefix
		{
			get { return specialTextPrefix; }
			set { specialTextPrefix = value; }
		}

		public ReferenceSelectionModeEnum ReferenceSelectionMode
		{
			get { return referenceSelectionMode; }
			set { referenceSelectionMode = value; }
		}

		public override string Text
		{
			get
			{
				//!!!!
				//получать цвета, жирность и т.д.

				var nameText = component.Name;

				if( string.IsNullOrEmpty( SpecialTextPrefix ) )
				{
					if( nameText == "" && component.Parent == null )
						nameText = "\'" + EditorLocalization2.Translate( "General", "Root" ) + "\'";
					if( nameText == "" )
						nameText = "\'" + EditorLocalization2.Translate( "General", "No name" ) + "\'";
				}

				var typeName = component.BaseType.ToString();
				//!!!!а если это имя файла?
				{
					string prefix = "NeoAxis.";
					if( typeName.Length > prefix.Length && typeName.Substring( 0, prefix.Length ) == prefix )
						typeName = typeName.Substring( prefix.Length );
				}
				//!!!!
				//{
				//	string prefix = "";
				//	if( typeName.Length > prefix.Length && typeName.Substring( 0, prefix.Length ) == prefix )
				//		typeName = typeName.Substring( prefix.Length );
				//}

				if( Owner.ReadOnlyHierarchy )
				{
					//Project settings specific
					return nameText;
				}
				else
				{
					string result = "";
					if( !string.IsNullOrEmpty( SpecialTextPrefix ) )
						result += SpecialTextPrefix;

					if( !string.IsNullOrEmpty( nameText ) )
						result += nameText + " - ";
					result += typeName;

					return result;
				}

				//return string.Format( "{0} - {1}", nameText, typeName );
			}
		}

		bool NeedShowMembers()
		{
			if( Owner.ShowMembers )
			{
				if( Owner.Mode == ContentBrowser.ModeEnum.Resources /*&&
					( Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.None ||
					Owner.ResourcesModeData.selectionMode == ResourceSelectionMode.Member )*/ ||
					Owner.Mode == ContentBrowser.ModeEnum.SetReference ||
					Owner.Mode == ContentBrowser.ModeEnum.Objects )
				{
					return true;
				}
			}
			return false;
		}

		void UpdateMemberChildren()
		{
			//get members
			var members = new List<Metadata.Member>( 256 );
			{
				foreach( var member in component.MetadataGetMembers() )
				{
					bool allow;
					if( Owner.Mode == ContentBrowser.ModeEnum.SetReference )
					{
						if( Owner.SetReferenceModeData.newObjectWindow || Owner.SetReferenceModeData.selectTypeWindow )
							allow = false;
						else
						{
							var type = Owner.SetReferenceModeData.DemandedType;
							allow = ContentBrowserUtility.ContentBrowserSetReferenceModeCheckAllowAddMember( type, member, false );
						}
					}
					else
					{
						if( component.ParentRoot.hierarchyController != null &&
							component.ParentRoot.hierarchyController.CreatedByResource.InstanceType == Resource.InstanceType.Resource )
						{
							if( member.Owner == component )
								allow = true;
							else
								allow = false;
						}
						else
							allow = true;

						//allow = true;

						////property
						//var property = member as Metadata.Property;
						////!!!!как везде унифицировать получаемый список нужных мемберов. в гриде тоже подобно ведь. где-то еще
						//if( property != null && property.Browsable && !property.HasIndexers && !property.Static )
						//{
						//	memberChildren.TryGetValue( property, out ContentBrowser.Item item );

						//	if( item == null )
						//	{
						//		//!!!!дальше получаем либо тип, либо объект типа
						//		//!!!!!!объект включает в себя мемберов типа

						//		item = new ContentBrowserItem_Member( Owner, this, member );
						//		//!!!!статичные еще
						//		item.imageKey = "Property";

						//		//item = new ContentBrowserItem_NoSpecialization( Owner, this, "property: " + p.Name, null );
						//	}

						//	newChildren.Add( property, item );
						//}

					}

					if( allow )
						members.Add( member );
				}

				//sort
				ContentBrowserUtility.SortMemberItems( members );
			}

			//get new list of items
			var newChildren = new EDictionary<Metadata.Member, ContentBrowser.Item>( 256 );
			foreach( var member in members )
			{
				memberChildren.TryGetValue( member, out ContentBrowser.Item item );
				if( item == null )
					item = new ContentBrowserItem_Member( Owner, this, member );

				if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
					newChildren.Add( member, item );
				else
					item.Dispose();
			}

			//remove old
			{
				ESet<ContentBrowser.Item> s = new ESet<ContentBrowser.Item>( newChildren.Values );
				foreach( var item in memberChildren.Values )
				{
					if( !s.Contains( item ) )
						item.Dispose();
				}
			}

			memberChildren = newChildren;
		}

		//public override void SetWasExpanded()
		//{
		//	base.SetWasExpanded();

		//	////!!!!как динамически обновлять?
		//	//if( NeedShowMembers() )
		//	//	UpdateMemberChildren();

		//	////!!!!new
		//	////for member refresh
		//	//PerformChildrenChanged();
		//}

		public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
		{
			if( !onlyAlreadyCreated )
			{
				//update what need
				{
					EDictionary<Component, ContentBrowser.Item> newChildren = new EDictionary<Component, ContentBrowser.Item>();

					//add (with new order)
					foreach( var childComponent in component.Components )
					{
						if( childComponent.DisplayInEditor && childComponent.TypeSettingsIsPublic() && EditorUtility.PerformComponentDisplayInEditorFilter( childComponent ) )
						{
							//bool skip = false;

							////Type Settings filter
							//var baseComponentType = component.BaseType as Metadata.ComponentTypeInfo;
							//if( baseComponentType != null && ComponentUtility.TypeSettingsCheckHideObject( baseComponentType.BasedOnObject, true, childComponent ) )
							//	skip = true;
							////if( baseComponentType != null && ComponentUtils.TypeSettingsCheckHideObject( component, true, childComponent ) )
							////	skip = true;

							//if( !skip )
							//{
							componentChildren.TryGetValue( childComponent, out ContentBrowser.Item item );

							if( item == null )
							{
								item = new ContentBrowserItem_Component( Owner, this, childComponent );

								//for Resources
								if( /*!string.IsNullOrEmpty( imageKey ) && */ imageKey != "GoUpper" )
								{
									if( !string.IsNullOrEmpty( imageKey ) && string.IsNullOrEmpty( item.imageKey ) )
										item.imageKey = imageKey;
								}
							}

							newChildren.Add( childComponent, item );
							//}
						}
					}

					//remove old
					{
						ESet<ContentBrowser.Item> s = new ESet<ContentBrowser.Item>( newChildren.Values );
						foreach( var item in componentChildren.Values )
						{
							if( !s.Contains( item ) )
								item.Dispose();
						}
					}

					componentChildren = newChildren;
				}

				if( ReferenceSelectionMode == ReferenceSelectionModeEnum.This && component.Parent != null )
				{
					if( thisItem == null )
					{
						thisItem = new ContentBrowserItem_Component( Owner, this, component.Parent );
						thisItem.imageKey = "GoUpper";

						//!!!!может не точки
						thisItem.SpecialTextPrefix = ".. ";
						thisItem.ReferenceSelectionMode = ReferenceSelectionModeEnum.This;

						//thisItem = new ContentBrowserItem_Virtual( Owner, this, ".." );

						//var item = new ContentBrowserItem_Component( Owner, thisItem, component.Parent );
						//item.ReferenceSelectionMode = ReferenceSelectionModeEnum.This;
						//thisItem.children.Add( item );
					}
				}

				//members
				if( NeedShowMembers() )
				{
					UpdateMemberChildren();
				}
			}

			//prepare result
			List<ContentBrowser.Item> result = new List<ContentBrowser.Item>( componentChildren.Values.Count + 1 + memberChildren.Count );
			if( thisItem != null )
				result.Add( thisItem );
			foreach( var item in componentChildren.Values )
				result.Add( item );
			foreach( var item in memberChildren.Values )
				result.Add( item );
			return result;
		}

		private void NameChanged( Component obj )
		{
			if( EditorAPI.ClosingApplication )
				return;
			if( obj.RemoveFromParentQueued || obj.Disposed )
				return;
			if( obj.ParentRoot?.HierarchyController != null && !obj.ParentRoot.HierarchyController.HierarchyEnabled )
				return;

			PerformTextChanged();
		}

		private void EnabledInHierarchyChanged( Component obj )
		{
			if( EditorAPI.ClosingApplication )
				return;
			if( obj.RemoveFromParentQueued || obj.Disposed )
				return;
			if( obj.ParentRoot?.HierarchyController != null && !obj.ParentRoot.HierarchyController.HierarchyEnabled )
				return;

			//!!!!slowly? update later?

			ShowDisabled = !component.EnabledInHierarchy;
			PerformTextColorChanged();
		}

		private void ComponentsChanged( Component obj )
		{
			if( EditorAPI.ClosingApplication )
				return;
			if( obj.RemoveFromParentQueued || obj.Disposed )
				return;
			if( obj.ParentRoot?.HierarchyController != null && !obj.ParentRoot.HierarchyController.HierarchyEnabled )
				return;

			var document = Owner?.DocumentWindow?.Document2;
			if( document != null && document.Destroyed )
				return;

			PerformChildrenChanged();
		}

		public override void CalculateReferenceValue( Component from, Metadata.TypeInfo expectedType, out string referenceValue, out bool canSet )
		{
			referenceValue = ContentBrowserUtility.CalculateReferenceValueForComponentItem( from, this );
			canSet = false;

			//specialization for Metadata.TypeInfo
			if( MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ).IsAssignableFrom( expectedType ) )
			{
				//reference to component of type
				var type = component.GetProvidedType();
				if( type != null )
					canSet = true;
			}
			else
			{
				//default behaviour

				//check type
				if( ReferenceUtility.CanMakeReferenceToObjectWithType( expectedType, MetadataManager.MetadataGetType( component ) ) )
				{
					if( from != null && from.ParentRoot == component.ParentRoot )
					{
						//reference inside same resource
						canSet = true;
					}
					else
					{
						//reference to another resource
						var resourceInstance = component.ParentRoot?.HierarchyController.CreatedByResource;
						if( resourceInstance != null )
						{
							//reference to component of type
							var type = component.GetProvidedType();
							if( type != null )
								canSet = true;
						}
					}
				}
			}
		}

		public override bool CanDoDragDrop() { return true; }

		void ApplyImageKey()
		{
			//imageKey = "Class";//"Default";

			try
			{
				var type = MetadataManager.GetTypeOfNetType( component.GetType() );
				imageKey = EditorImageHelperComponentTypes.GetImageKey( type );
			}
			catch { }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ContentBrowserItem_Member : ContentBrowser.Item
	{
		Metadata.Member member;
		//ContentBrowser.Item[] children;
		//!!!!не только свойства. отдельным словарем?
		EDictionary<Metadata.Member, ContentBrowser.Item> memberChildren = new EDictionary<Metadata.Member, ContentBrowser.Item>();

		/////////////////////////////////////////

		//public delegate void AllObjects_ChildrenChangedDelegate( object obj );
		//public static event AllObjects_ChildrenChangedDelegate AllObjects_ChildrenChanged;

		//public static void AllObjects_PerformChildrenChanged( object obj )
		//{
		//	AllObjects_ChildrenChanged?.Invoke( obj );
		//}

		/////////////////////////////////////////

		public ContentBrowserItem_Member( ContentBrowser owner, ContentBrowser.Item parent, Metadata.Member member )
			: base( owner, parent )
		{
			this.member = member;

			if( member is Metadata.Property )
				imageKey = member.Static ? "StaticProperty" : "Property";
			else if( member is Metadata.Method )
			{
				var method = (Metadata.Method)member;
				if( method.Constructor )
					imageKey = "Constructor";
				else if( method.Operator )
					imageKey = "Operator";
				else
					imageKey = member.Static ? "StaticMethod" : "Method";
			}
			else if( member is Metadata.Event )
				imageKey = member.Static ? "StaticEvent" : "Event";

			//AllObjects_ChildrenChanged += ContentBrowserItem_AllObjects_ChildrenChanged;
		}

		public override void Dispose()
		{
			//!!!!что еще удалять. где еще забылось

			foreach( var item in memberChildren.Values )
				item.Dispose();
			memberChildren.Clear();

			//AllObjects_ChildrenChanged -= ContentBrowserItem_AllObjects_ChildrenChanged;
		}

		//void ContentBrowserItem_AllObjects_ChildrenChanged( object obj )
		//{
		//	if( obj == this.sourceObject )
		//		PerformChildrenChanged();
		//}

		public Metadata.Member Member
		{
			get { return member; }
		}

		public override string Text
		{
			get { return member.ToString(); }
		}

		public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
		{
			if( !onlyAlreadyCreated )
			{
				if( Owner.Mode == ContentBrowser.ModeEnum.SetReference )
				{
					//!!!!юзать объект или тип? сейчас юзается тип

					//var expectedType = Owner.SetReferenceModeData.DemandedType;

					//if( !MetadataManager.GetTypeOfNetType( typeof( ReferenceValueType_Member ) ).IsAssignableFrom( expectedType ) )
					//{

					var property = member as Metadata.Property;
					var method = member as Metadata.Method;
					if( property != null || method != null )
					{
						//get type
						Metadata.TypeInfo type = null;
						if( property != null )
							type = property.Type;
						else
						{
							var parameters = method.GetReturnParameters();
							if( parameters.Length == 1 )
								type = parameters[ 0 ].Type;
						}

						if( type != null )
						{
							//get members
							var members = new List<Metadata.Member>( 256 );
							{
								//the property with Reference<> type. get members of unreferenced types.
								//минус этого в том, что нельзя обратиться к свойства Reference<>, например, к свойству ReferenceSpecified.
								if( property != null && ReferenceUtility.IsReferenceType( property.Type.GetNetType() ) )
								{
									foreach( var m in property.TypeUnreferenced.MetadataGetMembers() )
									{
										bool allow = false;
										if( !m.Static )
										{
											//allow = ContentBrowserUtils.ContentBrowserSetReferenceModeCheckAllowAddMember( Owner, m );
											var property2 = m as Metadata.Property;
											if( property2 != null && !property2.HasIndexers )
												allow = true;
											var method2 = m as Metadata.Method;
											if( method2 != null && method2.Parameters.Length == 1 && method2.GetReturnParameters().Length == 1 )
												allow = true;
										}

										if( allow )
											members.Add( m );
									}
								}
								else
								{
									//default non Reference<> members
									foreach( var m in type.MetadataGetMembers() )
									{
										bool allow = false;
										if( !m.Static )
										{
											//allow = ContentBrowserUtils.ContentBrowserSetReferenceModeCheckAllowAddMember( Owner, m );
											var property2 = m as Metadata.Property;
											if( property2 != null && !property2.HasIndexers )
												allow = true;
											var method2 = m as Metadata.Method;
											if( method2 != null && method2.Parameters.Length == 1 && method2.GetReturnParameters().Length == 1 )
												allow = true;
										}

										if( allow )
											members.Add( m );
									}
								}

								//sort
								ContentBrowserUtility.SortMemberItems( members );
							}

							//get new list of items
							var newChildren = new EDictionary<Metadata.Member, ContentBrowser.Item>();
							foreach( var m in members )
							{
								memberChildren.TryGetValue( m, out ContentBrowser.Item item );
								if( item == null )
									item = new ContentBrowserItem_Member( Owner, this, m );
								newChildren.Add( m, item );
							}

							//remove old
							{
								ESet<ContentBrowser.Item> s = new ESet<ContentBrowser.Item>( newChildren.Values );
								foreach( var item in memberChildren.Values )
								{
									if( !s.Contains( item ) )
										item.Dispose();
								}
							}

							memberChildren = newChildren;
						}
					}
					//}
				}
			}

			//prepare result
			List<ContentBrowser.Item> result = new List<ContentBrowser.Item>( memberChildren.Count );
			foreach( var item in memberChildren.Values )
				result.Add( item );
			return result;
		}

		public override void CalculateReferenceValue( Component from, Metadata.TypeInfo expectedType, out string referenceValue, out bool canSet )
		{
			referenceValue = ContentBrowserUtility.CalculateReferenceValueForMemberItem( from, expectedType, this );
			canSet = false;

			//!!!!тогда можно было бы указать тип, которые выдают в мембере по ссылке
			////specialization for Metadata.TypeInfo
			//if( MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ).IsAssignableFrom( expectedType ) )
			//{
			//	//if( ReferenceUtils.CanMakeReferenceToObjectWithType( expectedType, property2.TypeUnreferenced ) )
			//	//{
			//		canSet = true;
			//	//}
			//}

			//specialization for ReferenceValueType_Member
			if( MetadataManager.GetTypeOfNetType( typeof( ReferenceValueType_Member ) ).IsAssignableFrom( expectedType ) )
			{
				if( ContentBrowserUtility.ContentBrowserSetReferenceModeCheckAllowAddMember( expectedType, member, false ) )
					canSet = true;
				//!!!!?
				//return;
			}
			//if( Owner.Mode == ContentBrowser.ModeEnum.SetReference )
			//{
			//	var expectedType = Owner.SetReferenceModeData.property.TypeUnreferenced;
			//	if( MetadataManager.GetTypeOfNetType( typeof( ReferenceValueType_Member ) ).IsAssignableFrom( expectedType ) )
			//	{
			//		if( ContentBrowserUtils.ContentBrowserSetReferenceModeCheckAllowAddMember( Owner, member, false ) )
			//			canSet = true;

			//		return;
			//	}
			//}

			//default behaviour
			if( member is Metadata.Property )
			{
				//property
				var property2 = (Metadata.Property)member;

				if( ReferenceUtility.CanMakeReferenceToObjectWithType( expectedType, property2.TypeUnreferenced ) )
					canSet = true;
			}
			else if( member is Metadata.Method )
			{
				//method
				var method = (Metadata.Method)member;

				var returnParameters = method.GetReturnParameters();
				if( method.Parameters.Length == 1 && returnParameters.Length == 1 )
				{
					if( ReferenceUtility.CanMakeReferenceToObjectWithType( expectedType, returnParameters[ 0 ].Type ) )
						canSet = true;
				}
			}
		}

		public override string GetDescription()
		{
			var id = XmlDocumentationFiles.GetMemberId( member );
			if( !string.IsNullOrEmpty( id ) )
			{
				var v = XmlDocumentationFiles.GetMemberSummary( id );

				////remove last dot for one dot strings
				//if( v != null && v.Count( c => c == '.' ) == 1 && v[ v.Length - 1 ] == '.' )
				//	v = v.Substring( 0, v.Length - 1 );

				return v;
			}
			return "";
		}

		public override bool CanDoDragDrop() { return true; }
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ContentBrowserItem_Null : ContentBrowser.Item
	{
		//!!!!public. везде так
		public string text;

		//

		public ContentBrowserItem_Null( ContentBrowser owner, ContentBrowser.Item parent, string text )
			: base( owner, parent )
		{
			this.text = text;
		}

		public override void Dispose()
		{
		}

		public override string Text
		{
			get { return text; }
		}

		public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
		{
			return new ContentBrowser.Item[ 0 ];
		}

		public override void CalculateReferenceValue( Component propertyOwner, Metadata.TypeInfo expectedType, out string referenceValue, out bool canSet )
		{
			referenceValue = "";
			canSet = true;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ContentBrowserItem_Favorites : ContentBrowser.Item
	{
		string text;
		EDictionary<Metadata.TypeInfo, ContentBrowserItem_Type> typeChildren = new EDictionary<Metadata.TypeInfo, ContentBrowserItem_Type>();
		EDictionary<string, ContentBrowserItem_File> fileChildren = new EDictionary<string, ContentBrowserItem_File>();

		//

		public ContentBrowserItem_Favorites( ContentBrowser owner, ContentBrowser.Item parent, string text )
			: base( owner, parent )
		{
			this.text = text;
		}

		public override void Dispose()
		{
			foreach( var item in typeChildren.Values )
				item.Dispose();
			typeChildren.Clear();

			foreach( var item in fileChildren.Values )
				item.Dispose();
			fileChildren.Clear();
		}

		public override string Text
		{
			get { return text; }
		}

		void UpdateChildren()
		{
			//get data

			var types = new List<Metadata.TypeInfo>( 32 );
			var files = new List<string>( 32 );
			foreach( var name in EditorFavorites.Favorites.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries ) )
			{
				var type = MetadataManager.GetTypeOfNetType( name );
				if( type != null )
					types.Add( type );
				else if( VirtualFile.Exists( name ) )
					files.Add( name );
			}

			CollectionUtility.MergeSort( types, delegate ( Metadata.TypeInfo t1, Metadata.TypeInfo t2 )
			{
				return string.Compare( t1.Name, t2.Name );
			} );

			CollectionUtility.MergeSort( files, delegate ( string name1, string name2 )
			{
				var n1 = Path.GetFileName( name1 );
				var n2 = Path.GetFileName( name2 );
				return string.Compare( n1, n2 );
			} );


			//types
			{
				//get new list of items
				var newChildren = new EDictionary<Metadata.TypeInfo, ContentBrowserItem_Type>( 256 );
				foreach( var type in types )
				{
					typeChildren.TryGetValue( type, out var typeItem );
					if( typeItem == null )
					{
						var text = type.DisplayName;
						{
							var item = ResourcesWindowItems.GetItemByType( type.GetNetType() );
							if( item != null )
							{
								try
								{
									text = Path.GetFileName( item.Path );
								}
								catch { }
							}
						}

						typeItem = new ContentBrowserItem_Type( Owner, this, type, text );
						typeItem.imageKey = ContentBrowser.GetTypeImageKey( type );
						typeItem.memberCreationDisable = true;
					}

					if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( typeItem ) )
						newChildren.Add( type, typeItem );
					else
						typeItem.Dispose();
				}

				//remove old
				{
					var s = new ESet<ContentBrowserItem_Type>( newChildren.Values );
					foreach( var item in typeChildren.Values )
					{
						if( !s.Contains( item ) )
							item.Dispose();
					}
				}

				typeChildren = newChildren;
			}

			//files
			{
				//get new list of items
				var newChildren = new EDictionary<string, ContentBrowserItem_File>( 256 );
				foreach( var file in files )
				{
					fileChildren.TryGetValue( file, out var fileItem );
					if( fileItem == null )
					{
						var fullPath = VirtualPathUtility.GetRealPathByVirtual( file );

						fileItem = new ContentBrowserItem_File( Owner, this, fullPath, false );
						fileItem.imageKey = ResourceManager.GetResourceImageKey( fullPath );
					}

					if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( fileItem ) )
						newChildren.Add( file, fileItem );
					else
						fileItem.Dispose();
				}

				//remove old
				{
					var s = new ESet<ContentBrowserItem_File>( newChildren.Values );
					foreach( var item in fileChildren.Values )
					{
						if( !s.Contains( item ) )
							item.Dispose();
					}
				}

				fileChildren = newChildren;
			}
		}

		public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
		{
			if( !onlyAlreadyCreated )
				UpdateChildren();

			//prepare result
			List<ContentBrowser.Item> result = new List<ContentBrowser.Item>( typeChildren.Values.Count + fileChildren.Count );
			foreach( var item in typeChildren.Values )
				result.Add( item );
			foreach( var item in fileChildren.Values )
				result.Add( item );
			return result;
		}

		public override void CalculateReferenceValue( Component propertyOwner, Metadata.TypeInfo expectedType, out string referenceValue, out bool canSet )
		{
			referenceValue = "";
			canSet = true;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//public class ContentBrowserItem_List : ContentBrowser.Item
	//{
	//	IList container;
	//	ContentBrowser.Item[] children;

	//	//

	//	public ContentBrowserItem_List( ContentBrowser owner, ContentBrowser.Item parent, IList container )
	//		: base( owner, parent )
	//	{
	//		this.container = container;

	//		//if( displayObject == null )
	//		//{
	//		//	if( IsList( sourceObject ) )
	//		//	{
	//		//		//!!!!

	//		//		ListProvider provider = new ListProvider();

	//		//		//!!!!можно списком вывести итемы, если они простые

	//		//		//Metadata.TypeInfo type = MetadataManager.GetTypeOfNetType( GetListElementType( list ) );
	//		//		//ListItemProperty property = new ListItemProperty( valueProvider, "Value", "", false, type, null, false );
	//		//		//property.owner = this;
	//		//		//property.listIndex = n;

	//		//		//provider.Property = property;

	//		//		displayObject = provider;

	//		//	}

	//		//	//!!!!
	//		//	//if(IsDictionary( sourceObject))

	//		//	if( displayObject == null )
	//		//		displayObject = sourceObject;
	//		//}

	//		//this.displayObject = displayObject;

	//		//AllObjects_ChildrenChanged += ContentBrowserItem_AllObjects_ChildrenChanged;
	//	}

	//	public override void Dispose()
	//	{
	//		if( children != null )
	//		{
	//			foreach( var item in children )
	//				item.Dispose();
	//		}

	//		//AllObjects_ChildrenChanged -= ContentBrowserItem_AllObjects_ChildrenChanged;
	//	}

	//	xx xx;
	//	void ContentBrowserItem_AllObjects_ChildrenChanged( object obj )
	//	{
	//		//!!!!
	//		if( obj == this.sourceObject )
	//			PerformChildrenChanged();
	//	}

	//	public IList Container
	//	{
	//		get { return container; }
	//	}

	//	//public object DisplayObject
	//	//{
	//	//	get { return displayObject; }
	//	//}

	//	public override object ContainedObject
	//	{
	//		get { return container; }
	//	}

	//	public static bool EqualsValues( object value1, object value2 )
	//	{
	//		if( value1 == null && value2 != null )
	//			return false;
	//		if( value1 != null && value2 == null )
	//			return false;
	//		if( value1 == null && value2 == null )
	//			return true;

	//		if( value1.GetType().IsValueType )
	//			return Equals( value1, value2 );
	//		else
	//			return ReferenceEquals( value1, value2 );
	//	}

	//	public override string Text
	//	{
	//		get { return MetadataManager.GetTypeOfNetType( Container.GetType() ).DisplayName; }
	//	}

	//	//!!!!
	//	//public static bool IsArray( object obj )
	//	//{
	//	//}

	//	public static bool IsList( object obj )
	//	{
	//		return obj is IList &&
	//			obj.GetType().IsGenericType &&
	//			obj.GetType().GetGenericTypeDefinition().IsAssignableFrom( typeof( List<> ) );
	//	}

	//	public static Type GetListElementType( object list )
	//	{
	//		return list.GetType().GetGenericArguments()[ 0 ];
	//	}

	//	public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
	//	{
	//		var list = Container;

	//		var usedItems = new ESet<ContentBrowser.Item>();

	//		var newChildren = new ContentBrowser.Item[ list.Count ];
	//		for( int index = 0; index < newChildren.Length; index++ )
	//		{
	//			var value = list[ index ];

	//			ContentBrowser.Item item = null;

	//			//use old item if same
	//			if( children != null && index < children.Length )
	//			{
	//				var oldItem = children[ index ];
	//				if( oldItem != null )
	//				{
	//					var oldNoSpec = oldItem as ContentBrowserItem_NoSpecialization;
	//					if( oldNoSpec != null && oldNoSpec.listIndex == index )
	//					{
	//						var oldValue = oldNoSpec.SourceObject;
	//						if( EqualsValues( value, oldValue ) )
	//							item = oldItem;
	//					}

	//					//object oldValue = null;
	//					//var oldNoSpec = oldItem as ContentBrowserItem_NoSpecialization;
	//					//if( oldNoSpec != null )
	//					//	oldValue = oldNoSpec.SourceObject;

	//					//if( EqualsValues( value, oldValue ) )
	//					//	item = oldItem;
	//				}
	//			}

	//			if( item == null )
	//			{
	//				xx xx;

	//				var valueProvider = new ListItemProvider();

	//				//!!!!structures
	//				//!!!!enums
	//				//!!!!если null


	//				object sourceObject = null;
	//				object displayObject = null;

	//				var elementType = GetListElementType( list );
	//				if( SimpleTypesUtils.IsSimpleType( elementType ) )
	//				{
	//					//simple type. one property "Value"

	//					Metadata.TypeInfo type = MetadataManager.GetTypeOfNetType( GetListElementType( list ) );

	//					//!!!!так?
	//					var typeUnreferenced = type;

	//					var property = new ListItemProperty( valueProvider, "Value", false, type, typeUnreferenced, null, false );
	//					property.Description = "";
	//					property.sourceObject = list;
	//					//property.owner = this;
	//					property.listIndex = index;

	//					valueProvider.Property = property;

	//					sourceObject = value;//valueProvider
	//					displayObject = valueProvider;
	//				}
	//				else
	//				{
	//					//class/structure
	//					sourceObject = value;
	//					displayObject = value;
	//				}

	//				var noSpecItem = new ContentBrowserItem_NoSpecialization( Owner, this, sourceObject, displayObject );
	//				noSpecItem.listIndex = index;
	//				item = noSpecItem;
	//			}

	//			newChildren[ index ] = item;

	//			usedItems.Add( item );
	//		}

	//		//remove old
	//		if( children != null )
	//		{
	//			foreach( var item in children )
	//			{
	//				if( !usedItems.Contains( item ) )
	//					item.Dispose();
	//			}
	//		}

	//		children = newChildren;

	//		return children;
	//	}

	//	//public bool CanDelete()
	//	//{
	//	//	//!!!!

	//	//	if( Parent != null )
	//	//	{
	//	//		var noSpecParent = Parent as ContentBrowserItem_NoSpecialization;
	//	//		if( noSpecParent != null )
	//	//		{
	//	//			if( IsList( noSpecParent.SourceObject ) )
	//	//				return true;
	//	//		}

	//	//		return true;
	//	//	}

	//	//	return false;
	//	//}
	//}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////!!!!name
	//public class ContentBrowserItem_NoSpecialization : ContentBrowser.Item
	//{
	//	xx xx;

	//	object sourceObject;
	//	object displayObject;
	//	ContentBrowser.Item[] children;

	//	//!!!!new
	//	public int listIndex;

	//	/////////////////////////////////////////

	//	//!!!!!

	//	xx xx;
	//	public delegate void AllObjects_ChildrenChangedDelegate( object obj );
	//	public static event AllObjects_ChildrenChangedDelegate AllObjects_ChildrenChanged;

	//	public static void AllObjects_PerformChildrenChanged( object obj )
	//	{
	//		AllObjects_ChildrenChanged?.Invoke( obj );
	//	}

	//	/////////////////////////////////////////

	//	//!!!!
	//	//public class ListProperty : Metadata.Property
	//	//{
	//	//	//!!!!public
	//	//	public ContentBrowserItem_NoSpecialization owner;
	//	//	//!!!!
	//	//	public int listIndex;
	//	//	//public object value;

	//	//	//

	//	//	public ListProperty( object creator, string name, string description, bool isStatic, Metadata.TypeInfo type,
	//	//		Metadata.Parameter[] indexers, bool readOnly )
	//	//		: base( creator, name, description, isStatic, type, indexers, readOnly )
	//	//	{
	//	//	}

	//	//	public override object[] GetCustomAttributes( Type attributeType, bool inherit )
	//	//	{
	//	//		//!!!!какие еще атрибуты

	//	//		List<object> attribs = new List<object>();
	//	//		if( attributeType.IsAssignableFrom( typeof( CategoryAttribute ) ) )
	//	//			attribs.Add( new CategoryAttribute( "Item" ) );

	//	//		return attribs.ToArray();
	//	//		//return new object[ 0 ];
	//	//	}

	//	//	public override object GetValue( object obj, object[] index )
	//	//	{
	//	//		var list = (IList)owner.sourceObject;
	//	//		return list[ listIndex ];
	//	//	}

	//	//	public override void SetValue( object obj, object value, object[] index )
	//	//	{
	//	//		var list = (IList)owner.sourceObject;
	//	//		list[ listIndex ] = value;
	//	//	}
	//	//}

	//	/////////////////////////////////////////

	//	////!!!!!
	//	//public class ListProvider : Metadata.IMetadataProvider
	//	//{
	//	//	//ListItemProperty property;

	//	//	//

	//	//	public ListProvider()
	//	//	{
	//	//	}

	//	//	//public ListItemProperty Property
	//	//	//{
	//	//	//	get { return property; }
	//	//	//	set { property = value; }
	//	//	//}

	//	//	public Metadata.TypeInfo BaseType
	//	//	{
	//	//		get { return null; }
	//	//	}

	//	//	public IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context )
	//	//	{
	//	//		//!!!!
	//	//		Metadata.Member property = null;
	//	//		if( property != null )
	//	//			yield return property;
	//	//	}

	//	//	public Metadata.Member MetadataGetMemberBySignature( string signature, Metadata.GetMembersContext context )
	//	//	{
	//	//		//if( property != null && property.Signature == signature )
	//	//		//	return property;
	//	//		return null;
	//	//	}

	//	//	public override string ToString()
	//	//	{
	//	//		//if( property != null )
	//	//		//{
	//	//		//	var v = property.GetValue( property.owner.sourceObject, null );
	//	//		//	if( v != null )
	//	//		//		return v.ToString();
	//	//		//	else
	//	//		//		return "Null";
	//	//		//}

	//	//		return base.ToString();
	//	//	}
	//	//}

	//	/////////////////////////////////////////

	//	//!!!!

	//	public class ListItemProperty : Metadata.Property
	//	{
	//		//!!!!public
	//		public object sourceObject;
	//		//public ContentBrowserItem_NoSpecialization owner;
	//		public int listIndex;

	//		//

	//		public ListItemProperty( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced,
	//			Metadata.Parameter[] indexers, bool readOnly )
	//			: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
	//		{
	//		}

	//		protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
	//		{
	//			//!!!!какие еще атрибуты

	//			List<object> attribs = new List<object>();

	//			if( attributeType.IsAssignableFrom( typeof( CategoryAttribute ) ) )
	//				attribs.Add( new CategoryAttribute( "Item" ) );

	//			return attribs.ToArray();
	//		}

	//		public override object GetValue( object obj, object[] index )
	//		{
	//			var list = (IList)sourceObject;
	//			//var list = (IList)owner.sourceObject;

	//			//anti crash
	//			if( listIndex >= list.Count )
	//				return null;

	//			return list[ listIndex ];
	//		}

	//		public override void SetValue( object obj, object value, object[] index )
	//		{
	//			var list = (IList)sourceObject;
	//			//var list = (IList)owner.sourceObject;

	//			if( !EqualsValues( list[ listIndex ], value ) )
	//			{
	//				list[ listIndex ] = value;
	//				//owner.PerformChildrenChanged();
	//			}
	//		}
	//	}

	//	/////////////////////////////////////////

	//	//!!!!!
	//	public class ListItemProvider : Metadata.IMetadataProvider
	//	{
	//		ListItemProperty property;

	//		//

	//		public ListItemProvider()
	//		{
	//		}

	//		public ListItemProperty Property
	//		{
	//			get { return property; }
	//			set { property = value; }
	//		}

	//		public Metadata.TypeInfo BaseType
	//		{
	//			get { return null; }
	//		}

	//		public IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context )
	//		{
	//			if( property != null )
	//				yield return property;
	//		}

	//		public Metadata.Member MetadataGetMemberBySignature( string signature, Metadata.GetMembersContext context )
	//		{
	//			if( property != null && property.Signature == signature )
	//				return property;
	//			return null;
	//		}

	//		public override string ToString()
	//		{
	//			if( property != null )
	//			{
	//				var v = property.GetValue( property.sourceObject, null );
	//				//var v = property.GetValue( property.owner.sourceObject, null );
	//				if( v != null )
	//					return v.ToString();
	//				else
	//					return "Null";
	//			}

	//			return base.ToString();
	//		}
	//	}

	//	/////////////////////////////////////////

	//	public ContentBrowserItem_NoSpecialization( ContentBrowser owner, ContentBrowser.Item parent, object sourceObject, object displayObject )
	//		: base( owner, parent )
	//	{
	//		this.sourceObject = sourceObject;

	//		if( displayObject == null )
	//		{
	//			if( IsList( sourceObject ) )
	//			{
	//				//!!!!

	//				ListProvider provider = new ListProvider();

	//				//!!!!можно списком вывести итемы, если они простые

	//				//Metadata.TypeInfo type = MetadataManager.GetTypeOfNetType( GetListElementType( list ) );
	//				//ListItemProperty property = new ListItemProperty( valueProvider, "Value", "", false, type, null, false );
	//				//property.owner = this;
	//				//property.listIndex = n;

	//				//provider.Property = property;

	//				displayObject = provider;

	//			}

	//			//!!!!
	//			//if(IsDictionary( sourceObject))

	//			if( displayObject == null )
	//				displayObject = sourceObject;
	//		}

	//		this.displayObject = displayObject;

	//		//xx xx;
	//		AllObjects_ChildrenChanged += ContentBrowserItem_AllObjects_ChildrenChanged;
	//	}

	//	public override void Dispose()
	//	{
	//		if( children != null )
	//		{
	//			foreach( var item in children )
	//				item.Dispose();
	//		}

	//		AllObjects_ChildrenChanged -= ContentBrowserItem_AllObjects_ChildrenChanged;
	//	}

	//	void ContentBrowserItem_AllObjects_ChildrenChanged( object obj )
	//	{
	//		//!!!!
	//		if( obj == this.sourceObject )
	//			PerformChildrenChanged();
	//	}

	//	public object SourceObject
	//	{
	//		get { return sourceObject; }
	//	}

	//	public object DisplayObject
	//	{
	//		get { return displayObject; }
	//	}

	//	public override object ContainedObject
	//	{
	//		get { return displayObject; }
	//	}

	//	public static bool EqualsValues( object value1, object value2 )
	//	{
	//		if( value1 == null && value2 != null )
	//			return false;
	//		if( value1 != null && value2 == null )
	//			return false;
	//		if( value1 == null && value2 == null )
	//			return true;

	//		if( value1.GetType().IsValueType )
	//			return Equals( value1, value2 );
	//		else
	//			return ReferenceEquals( value1, value2 );
	//	}

	//	public override string Text
	//	{
	//		get
	//		{
	//			//!!!!
	//			if( sourceObject != null )
	//			{
	//				if( IsList( sourceObject ) )
	//					return MetadataManager.GetTypeOfNetType( sourceObject.GetType() ).DisplayName;
	//				else
	//					return sourceObject.ToString();
	//			}
	//			else
	//				return "";
	//		}
	//	}

	//	//!!!!
	//	//public static bool IsArray( object obj )
	//	//{
	//	//}

	//	public static bool IsList( object obj )
	//	{
	//		return obj is IList &&
	//			obj.GetType().IsGenericType &&
	//			obj.GetType().GetGenericTypeDefinition().IsAssignableFrom( typeof( List<> ) );
	//	}

	//	public static Type GetListElementType( object obj )
	//	{
	//		return obj.GetType().GetGenericArguments()[ 0 ];
	//	}

	//	public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
	//	{

	//		//!!!!bool onlyAlreadyCreated



	//		if( IsList( SourceObject ) )
	//		{
	//			var list = (IList)SourceObject;

	//			var usedItems = new ESet<ContentBrowser.Item>();

	//			var newChildren = new ContentBrowser.Item[ list.Count ];
	//			for( int index = 0; index < newChildren.Length; index++ )
	//			{
	//				var value = list[ index ];

	//				ContentBrowser.Item item = null;

	//				//use old item if same
	//				if( children != null && index < children.Length )
	//				{
	//					var oldItem = children[ index ];
	//					if( oldItem != null )
	//					{
	//						var oldNoSpec = oldItem as ContentBrowserItem_NoSpecialization;
	//						if( oldNoSpec != null && oldNoSpec.listIndex == index )
	//						{
	//							var oldValue = oldNoSpec.SourceObject;
	//							if( EqualsValues( value, oldValue ) )
	//								item = oldItem;
	//						}

	//						//object oldValue = null;
	//						//var oldNoSpec = oldItem as ContentBrowserItem_NoSpecialization;
	//						//if( oldNoSpec != null )
	//						//	oldValue = oldNoSpec.SourceObject;

	//						//if( EqualsValues( value, oldValue ) )
	//						//	item = oldItem;
	//					}
	//				}

	//				if( item == null )
	//				{
	//					var valueProvider = new ListItemProvider();

	//					//!!!!structures
	//					//!!!!enums
	//					//!!!!если null


	//					object sourceObject = null;
	//					object displayObject = null;

	//					var elementType = GetListElementType( list );
	//					if( SimpleTypesUtils.IsSimpleType( elementType ) )
	//					{
	//						//simple type. one property "Value"

	//						Metadata.TypeInfo type = MetadataManager.GetTypeOfNetType( GetListElementType( list ) );

	//						//!!!!так?
	//						var typeUnreferenced = type;

	//						var property = new ListItemProperty( valueProvider, "Value", false, type, typeUnreferenced, null, false );
	//						property.Description = "";
	//						property.sourceObject = list;
	//						//property.owner = this;
	//						property.listIndex = index;

	//						valueProvider.Property = property;

	//						sourceObject = value;//valueProvider
	//						displayObject = valueProvider;
	//					}
	//					else
	//					{
	//						//class/structure
	//						sourceObject = value;
	//						displayObject = value;
	//					}

	//					var noSpecItem = new ContentBrowserItem_NoSpecialization( Owner, this, sourceObject, displayObject );
	//					noSpecItem.listIndex = index;
	//					item = noSpecItem;
	//				}

	//				newChildren[ index ] = item;

	//				usedItems.Add( item );
	//			}

	//			//remove old
	//			if( children != null )
	//			{
	//				foreach( var item in children )
	//				{
	//					if( !usedItems.Contains( item ) )
	//						item.Dispose();
	//				}
	//			}

	//			children = newChildren;
	//		}
	//		else
	//		{
	//			children = new ContentBrowser.Item[ 0 ];
	//		}

	//		return children;
	//	}

	//	//!!!!
	//	public bool CanDelete()
	//	{
	//		//!!!!

	//		if( Parent != null )
	//		{
	//			var noSpecParent = Parent as ContentBrowserItem_NoSpecialization;
	//			if( noSpecParent != null )
	//			{
	//				if( IsList( noSpecParent.SourceObject ) )
	//					return true;
	//			}

	//			return true;
	//		}

	//		return false;
	//	}
	//}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!

	//Array

	//RegisterListSet( typeof( List<> ) );
	//RegisterListSet( typeof( ESet<> ) );
	//RegisterListSet( typeof( HashSet<> ) );
	//RegisterListSet( typeof( SortedSet<> ) );
	//RegisterListSet( typeof( Stack<> ) );
	//RegisterListSet( typeof( Queue<> ) );

	//RegisterDictionary( typeof( Dictionary<,> ) );
	//RegisterDictionary( typeof( EDictionary<,> ) );
	//RegisterDictionary( typeof( SortedList<,> ) );


	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////!!!!так ли
	//public class ContentBrowserItem_Dictionary : ContentBrowser.Item
	//{
	//	object obj;
	//	ContentBrowser.Item[] children;

	//	/////////////////////////////////////////

	//	//!!!!!

	//	xx xx;

	//	public delegate void AllLists_ChildrenChangedDelegate( object list );
	//	public static event AllLists_ChildrenChangedDelegate AllLists_ChildrenChanged;

	//	public static void AllLists_PerformChildrenChanged( object list )
	//	{
	//		AllLists_ChildrenChanged?.Invoke( list );
	//	}

	//	/////////////////////////////////////////

	//	public ContentBrowserItem_Dictionary( ContentBrowser owner, ContentBrowser.Item parent, object obj )
	//		: base( owner, parent )
	//	{
	//		this.obj = obj;

	//		xx xx;
	//		AllLists_ChildrenChanged += ContentBrowserItem_List_AllLists_ChildrenChanged;
	//	}

	//	public override void Dispose()
	//	{
	//		if( children != null )
	//		{
	//			foreach( var item in children )
	//				item.Dispose();
	//		}

	//		AllLists_ChildrenChanged -= ContentBrowserItem_List_AllLists_ChildrenChanged;
	//	}

	//	void ContentBrowserItem_List_AllLists_ChildrenChanged( object list )
	//	{
	//		xx xx;

	//		if( list == obj )
	//			PerformChildrenChanged();
	//	}

	//	public override object ContainedObject
	//	{
	//		get { return obj; }
	//	}

	//	public override string Text
	//	{
	//		get
	//		{
	//			//!!!!
	//			return MetadataManager.GetTypeOfNetType( obj.GetType() ).DisplayName;
	//			//return obj.ToString();
	//		}
	//	}

	//	public override IList<ContentBrowser.Item> GetChildren()//!!!!!bool forceUpdate )
	//	{
	//		xx xx;
	//		//EDictionary<int, int> cc;

	//		//!!!!могут повторяться

	//		xx xx;
	//		var container = (IList)ContainedObject;

	//		ESet<ContentBrowser.Item> usedItems = new ESet<ContentBrowser.Item>();

	//		ContentBrowser.Item[] newChildren = new ContentBrowser.Item[ container.Count ];
	//		for( int n = 0; n < newChildren.Length; n++ )
	//		{
	//			ContentBrowser.Item oldItem = null;
	//			if( children != null && n < children.Length )
	//				oldItem = children[ n ];

	//			var value = container[ n ];

	//			ContentBrowser.Item item = null;
	//			if( oldItem != null )
	//			{
	//				if( value != null )
	//				{
	//					if( value.GetType().IsValueType )
	//					{
	//						if( object.Equals( oldItem.ContainedObject, value ) )
	//							item = oldItem;
	//					}
	//					else
	//					{
	//						//!!!!!!?
	//						//if( oldItem.ContainedObject == value )

	//						if( object.ReferenceEquals( oldItem.ContainedObject, value ) )
	//							item = oldItem;
	//					}
	//				}
	//				else
	//				{
	//					if( oldItem.ContainedObject == null )
	//						item = oldItem;
	//				}
	//			}

	//			xx xx;
	//			if( item == null )
	//				item = new ContentBrowserItem_DictionaryItem( Owner, this, value );

	//			newChildren[ n ] = item;

	//			usedItems.Add( item );
	//		}

	//		//remove old
	//		if( children != null )
	//		{
	//			foreach( var item in children )
	//			{
	//				if( !usedItems.Contains( item ) )
	//					item.Dispose();
	//			}
	//		}

	//		children = newChildren;

	//		return children;
	//	}
	//}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////!!!!так ли
	//public class ContentBrowserItem_DictionaryItem : ContentBrowser.Item
	//{
	//	xx xx;

	//	//!!!!
	//	ValueProvider valueProvider;
	//	//object obj;

	//	List<ContentBrowser.Item> children;

	//	/////////////////////////////////////////

	//	//!!!!

	//	public class ValueProperty : Metadata.Property
	//	{
	//		//!!!!!public
	//		//public object value;

	//		//

	//		public ValueProperty( object creator, string name, string description, bool isStatic, Metadata.TypeInfo type,
	//			Metadata.Parameter[] indexers, bool readOnly, XXMemberInfo netMember )
	//			: base( creator, name, description, isStatic, type, indexers, readOnly )
	//		{
	//			xx xx;
	//			this.netMember = netMember;
	//		}

	//		public override object[] GetCustomAttributes( Type attributeType, bool inherit )
	//		{
	//			return new object[ 0 ];
	//		}

	//		public override object GetValue( object obj, object[] index )
	//		{
	//			return value;
	//		}

	//		public override void SetValue( object obj, object value, object[] index )
	//		{
	//			this.value = value;
	//		}
	//	}

	//	/////////////////////////////////////////

	//	public class ValueProvider : Metadata.IMetadataProvider
	//	{
	//		//!!!!
	//		public ValueProperty property;

	//		//

	//		//public ValueProvider( ValueProperty property )
	//		//{
	//		//	this.property = property;
	//		//}

	//		public Metadata.TypeInfo BaseType
	//		{
	//			get { return null; }
	//		}

	//		public IEnumerable<Metadata.Member> MetadataGetMembers( bool includeBaseTypes )
	//		{
	//			if( property != null )
	//				yield return property;
	//		}

	//		public Metadata.Member MetadataGetMemberBySignature( string signature, bool includeBaseTypes )
	//		{
	//			if( property != null && property.Signature == signature )
	//				return property;
	//			return null;
	//		}
	//	}

	//	/////////////////////////////////////////

	//	public ContentBrowserItem_DictionaryItem( ContentBrowser owner, ContentBrowser.Item parent, object obj )
	//		: base( owner, parent )
	//	{
	//		this.obj = obj;
	//	}

	//	public override void Dispose()
	//	{
	//		if( children != null )
	//		{
	//			foreach( var item in children )
	//				item.Dispose();
	//		}
	//	}

	//	public override object ContainedObject
	//	{
	//		get { return obj; }
	//	}

	//	public override string Text
	//	{
	//		get
	//		{
	//			//!!!!
	//			return obj.ToString();
	//		}
	//	}

	//	public override IList<ContentBrowser.Item> GetChildren()//!!!!!bool forceUpdate )
	//	{
	//		if( children == null )//!!!!!|| forceUpdate )
	//		{
	//			children = new List<ContentBrowser.Item>();
	//		}

	//		return children;
	//	}
	//}

}

#endif