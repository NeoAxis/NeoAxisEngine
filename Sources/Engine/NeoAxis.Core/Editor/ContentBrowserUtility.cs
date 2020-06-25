// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Text;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Auxiliary class for <see cref="ContentBrowser"/>.
	/// </summary>
	static class ContentBrowserUtility
	{
		static ContainsComponentClassesData containsComponentClassesData = new ContainsComponentClassesData();

		//static string _CalculateReferenceValue( bool allowThisFormat, Metadata.Property property, ContentBrowser.Item item )
		////, out ContentBrowserItem_Component.ReferenceSelectionModeEnum topItemMode )
		//{
		//	//!!!!multiselection

		//	List<string> result = new List<string>();
		//	var topItemMode = ContentBrowserItem_Component.ReferenceSelectionModeEnum.None;
		//	ContentBrowser.Item firstTypeOrFileItem = null;
		//	ContentBrowser.Item topItem = null;

		//	ContentBrowser.Item current = item;

		//	do
		//	{
		//		var componentItem = current as ContentBrowserItem_Component;
		//		if( componentItem != null )
		//		{
		//			//_Component

		//			if( componentItem.Parent != null )
		//			{
		//				if( componentItem.ReferenceSelectionMode == ContentBrowserItem_Component.ReferenceSelectionModeEnum.This )
		//					result.Add( ".." );
		//				else
		//					result.Add( componentItem.Component.GetNameWithIndexFromParent() );
		//			}

		//			if( componentItem.Parent == null )
		//				topItemMode = componentItem.ReferenceSelectionMode;
		//		}
		//		else
		//		{
		//			//_Member
		//			var memberItem = current as ContentBrowserItem_Member;
		//			if( memberItem != null )
		//			{
		//				var member = memberItem.Member;
		//				if( member != null )
		//				{
		//					xx xx;

		//					//!!!!так?
		//					//!!!!new
		//					if( item.Owner.Mode == ContentBrowser.ModeEnum.SetReference && MetadataManager.GetTypeOfNetType( typeof( ReferenceValueType_Member ) ).IsAssignableFrom( item.Owner.SetReferenceModeData.property.TypeUnreferenced ) )
		//					{
		//						result.Add( member.Signature );
		//					}
		//					else if( member is Metadata.Property || member is Metadata.Method )
		//					{
		//						result.Add( member.Name );
		//					}
		//					else
		//					{
		//						//!!!!
		//					}
		//				}
		//			}
		//		}

		//		if( firstTypeOrFileItem == null )
		//			firstTypeOrFileItem = current as ContentBrowserItem_Type;
		//		if( firstTypeOrFileItem == null )
		//			firstTypeOrFileItem = current as ContentBrowserItem_File;

		//		topItem = current;

		//		current = current.Parent;
		//	}
		//	while( current != null );


		//	StringBuilder builder = new StringBuilder();

		//	switch( topItemMode )
		//	{
		//	case ContentBrowserItem_Component.ReferenceSelectionModeEnum.None:
		//		{
		//			if( firstTypeOrFileItem != null )
		//			{
		//				var fileItem = firstTypeOrFileItem as ContentBrowserItem_File;
		//				if( fileItem != null )
		//					builder.Append( VirtualPathUtils.GetVirtualPathByReal( fileItem.FullPath ) + "|" );

		//				var typeItem = firstTypeOrFileItem as ContentBrowserItem_Type;
		//				if( typeItem != null )
		//					builder.Append( typeItem.Type.Name + "|" );
		//			}
		//			else
		//			{
		//				var componentTopItem = topItem as ContentBrowserItem_Component;
		//				if( componentTopItem != null )
		//				{
		//					builder.Append( "root:" );
		//					var path = componentTopItem.Component.GetNamePathToAccessFromRoot();
		//					if( path != "" )
		//						result.Add( path );
		//				}
		//			}
		//		}
		//		break;

		//	case ContentBrowserItem_Component.ReferenceSelectionModeEnum.Root:
		//		builder.Append( "root:" );
		//		break;

		//	case ContentBrowserItem_Component.ReferenceSelectionModeEnum.This:
		//		builder.Append( "this:" );
		//		break;
		//	}

		//	bool first = true;
		//	for( int n = result.Count - 1; n >= 0; n-- )
		//	{
		//		if( !first )
		//			builder.Append( '\\' );
		//		builder.Append( result[ n ] );

		//		first = false;
		//	}

		//	return builder.ToString();
		//}

		static ContentBrowserItem_Component.ReferenceSelectionModeEnum GetReferenceSelectionModeInHierarchy( ContentBrowserItem_Component item )
		{
			var current = item;
			do
			{
				if( current.ReferenceSelectionMode != ContentBrowserItem_Component.ReferenceSelectionModeEnum.None )
					return current.ReferenceSelectionMode;
				current = current.Parent as ContentBrowserItem_Component;
			} while( current != null );

			return ContentBrowserItem_Component.ReferenceSelectionModeEnum.None;
		}

		public static void CalculateReferenceValueForComponentItem( Component from, ContentBrowserItem_Component item, out string referenceValue, out char addSeparator )
		{
			Component to = item.Component;

			//different hierarchy
			if( from == null || from.ParentRoot != to.ParentRoot )
			{
				//if( canMakeRelativeFilePath && from != null )
				//	ReferenceUtility.CalculateCurrentFolderReference( from, to, "", out referenceValue, out addSeparator );
				//else
				ReferenceUtility.CalculateResourceReference( to, "", out referenceValue, out addSeparator );
				return;
			}

			//root:, this:
			{
				var referenceSelectionMode = GetReferenceSelectionModeInHierarchy( item );

				if( referenceSelectionMode == ContentBrowserItem_Component.ReferenceSelectionModeEnum.This )
				{
					ReferenceUtility.CalculateThisReference( from, to, "", out referenceValue, out addSeparator );
					return;
				}
				else if( referenceSelectionMode == ContentBrowserItem_Component.ReferenceSelectionModeEnum.Root )
				{
					ReferenceUtility.CalculateRootReference( to, "", out referenceValue, out addSeparator );
					return;
				}
			}

			//default
			{
				//_File
				//_Type
				//_Component

				var componentItemsStack = new List<ContentBrowserItem_Component>();
				ContentBrowser.Item firstTypeOrFileItem = null;
				{
					ContentBrowser.Item current = item;
					do
					{
						if( current is ContentBrowserItem_Component )
						{
							if( current.Parent != null )
								componentItemsStack.Add( (ContentBrowserItem_Component)current );
						}
						else if( current is ContentBrowserItem_Type || current is ContentBrowserItem_File )
						{
							firstTypeOrFileItem = current;
							break;
						}
						else
							Log.Fatal( "ContentBrowserUtils: CalculateReferenceValueForComponentItem: Internal error." );

						current = current.Parent;
					} while( current != null );

					componentItemsStack.Reverse();
				}

				var result = new StringBuilder();

				//start item
				if( firstTypeOrFileItem != null )
				{
					//_File, _Type

					var fileItem = firstTypeOrFileItem as ContentBrowserItem_File;
					if( fileItem != null )
						result.Append( VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath ) );

					var typeItem = firstTypeOrFileItem as ContentBrowserItem_Type;
					if( typeItem != null )
						result.Append( typeItem.Type.Name );

					addSeparator = '|';
				}
				else
				{
					//_Component
					result.Append( "root:" );
					addSeparator = '\0';
				}

				//add path of components
				foreach( var componentItem in componentItemsStack )
				{
					if( addSeparator != '\0' )
						result.Append( addSeparator );
					result.Append( componentItem.Component.GetPathFromParent() );
					addSeparator = '\\';
				}

				referenceValue = result.ToString();
			}
		}

		public static string CalculateReferenceValueForComponentItem( Component from, ContentBrowserItem_Component item )
		{
			CalculateReferenceValueForComponentItem( from, item, out string referenceValue, out char addSeparator );
			return referenceValue;

			//Component to = item.Component;

			//if( from.ParentRoot == to.ParentRoot )
			//{
			//	var referenceSelectionMode = GetReferenceSelectionModeInHierarchy( item );

			//	if( referenceSelectionMode == ContentBrowserItem_Component.ReferenceSelectionModeEnum.This )
			//		return ReferenceUtils.CalculateThisReference( from, to );
			//	else if( referenceSelectionMode == ContentBrowserItem_Component.ReferenceSelectionModeEnum.Root )
			//		return ReferenceUtils.CalculateRootReference( to );
			//}

			//CalculateReferenceValueForComponentItem( item, out string referenceValue, out char addSeparator );
			//return referenceValue;
		}

		public static void CalculateReferenceValueForMemberItem( Component from, Metadata.TypeInfo expectedType, ContentBrowserItem_Member item, out string referenceValue, out char addSeparator )
		{
			var memberItemsStack = new List<ContentBrowserItem_Member>();
			ContentBrowser.Item firstTypeOrFileOrComponentItem = null;
			{
				ContentBrowser.Item current = item;
				do
				{
					if( current is ContentBrowserItem_Member )
					{
						memberItemsStack.Add( (ContentBrowserItem_Member)current );
					}
					else if( current is ContentBrowserItem_Type || current is ContentBrowserItem_File || current is ContentBrowserItem_Component )
					{
						firstTypeOrFileOrComponentItem = current;
						break;
					}
					else
						Log.Fatal( "ContentBrowserUtils: CalculateReferenceValueForMemberItem: Internal error." );

					current = current.Parent;
				} while( current != null );

				memberItemsStack.Reverse();
			}

			var result = new StringBuilder();
			//char addSeparator;

			if( firstTypeOrFileOrComponentItem is ContentBrowserItem_Component )
			{
				//_Component
				var componentItem = (ContentBrowserItem_Component)firstTypeOrFileOrComponentItem;
				CalculateReferenceValueForComponentItem( from, componentItem, out string referenceValue2, out addSeparator );
				result.Append( referenceValue2 );
			}
			else
			{
				//_File, _Type

				var fileItem = firstTypeOrFileOrComponentItem as ContentBrowserItem_File;
				if( fileItem != null )
					result.Append( VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath ) );

				var typeItem = firstTypeOrFileOrComponentItem as ContentBrowserItem_Type;
				if( typeItem != null )
					result.Append( typeItem.Type.Name );

				addSeparator = '|';
			}

			//add members path
			foreach( var memberItem in memberItemsStack )
			{
				if( addSeparator != '\0' )
					result.Append( addSeparator );

				var member = memberItem.Member;
				if( expectedType != null && MetadataManager.GetTypeOfNetType( typeof( ReferenceValueType_Member ) ).IsAssignableFrom( expectedType ) )
					result.Append( member.Signature );
				else
					result.Append( member.Name );

				addSeparator = '\\';
			}

			referenceValue = result.ToString();
			//return result.ToString();
		}

		public static string CalculateReferenceValueForMemberItem( Component from, Metadata.TypeInfo expectedType, ContentBrowserItem_Member item )
		{
			CalculateReferenceValueForMemberItem( from, expectedType, item, out string referenceValue, out _ );
			return referenceValue;
		}

		static int GetMemberTypeSortIndex( Metadata.Member member )
		{
			//!!!!другие типы тоже

			if( member is Metadata.Property )
				return 0;
			if( member is Metadata.Method )
			{
				if( ( (Metadata.Method)member ).Constructor )
					return 1;
				else
					return 2;
			}
			if( member is Metadata.Event )
				return 3;

			return 100;
		}

		public static void SortMemberItems( List<Metadata.Member> members )
		{
			CollectionUtility.MergeSort( members, delegate ( Metadata.Member m1, Metadata.Member m2 )
			{
				int typeIndex1 = GetMemberTypeSortIndex( m1 );
				int typeIndex2 = GetMemberTypeSortIndex( m2 );
				if( typeIndex1 < typeIndex2 )
					return -1;
				if( typeIndex1 > typeIndex2 )
					return 1;

				if( m1.Static && !m2.Static )
					return -1;
				if( !m1.Static && m2.Static )
					return 1;

				return string.Compare( m1.Name + "z", m2.Name + "z" );
			} );
		}

		public static bool ContentBrowserSetReferenceModeCheckAllowAddMember( Metadata.TypeInfo propertyTypeUnreferenced, Metadata.Member member,
			bool staticForDefaultBehaviour )
		{
			var expectedType = propertyTypeUnreferenced;

			//if( MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ).IsAssignableFrom( expectedType ) )
			//{
			//	//specialization for Metadata.TypeInfo
			//	return false;
			//}
			//else 

			//specialization for ReferenceValueType_Member
			if( MetadataManager.GetTypeOfNetType( typeof( ReferenceValueType_Member ) ).IsAssignableFrom( expectedType ) )
			{
				if( MetadataManager.GetTypeOfNetType( typeof( ReferenceValueType_Method ) ).IsAssignableFrom( expectedType ) )
				{
					if( member is Metadata.Method )
						return true;
				}
				else if( MetadataManager.GetTypeOfNetType( typeof( ReferenceValueType_Property ) ).IsAssignableFrom( expectedType ) )
				{
					if( member is Metadata.Property )
						return true;
				}
				else if( MetadataManager.GetTypeOfNetType( typeof( ReferenceValueType_Event ) ).IsAssignableFrom( expectedType ) )
				{
					if( member is Metadata.Event )
						return true;
				}
				else
					return true;
			}

			//default behaviour
			if( staticForDefaultBehaviour == member.Static )
			{
				if( member is Metadata.Property )
				{
					//property
					var property = (Metadata.Property)member;
					if( !property.HasIndexers )
						return true;
				}
				else if( member is Metadata.Method )
				{
					//method
					var method = (Metadata.Method)member;
					if( method.Parameters.Length == 1 && method.GetReturnParameters().Length == 1 )
						return true;
				}
			}

			return false;
		}

		class ContainsComponentClassesData
		{
			public Dictionary<Type, bool> classes = new Dictionary<Type, bool>();
		}

		public static bool ContainsComponentClasses( Type type )
		{
			lock( containsComponentClassesData )
			{
				var data = containsComponentClassesData;

				bool contains;
				if( !data.classes.TryGetValue( type, out contains ) )
				{
					contains = typeof( Component ).IsAssignableFrom( type );
					if( !contains )
					{
						foreach( var nestedType in type.GetNestedTypes( BindingFlags.Public ) )
						{
							if( ContainsComponentClasses( nestedType ) )
							{
								contains = true;
								break;
							}
						}
					}

					data.classes[ type ] = contains;
				}

				return contains;
			}
		}

		public static bool ContainsType( Type seekingType, Type type )
		{
			//не кешируется как компоненты

			var contains = seekingType.IsAssignableFrom( type );
			if( !contains )
			{
				foreach( var nestedType in type.GetNestedTypes( BindingFlags.Public ) )
				{
					if( ContainsType( seekingType, nestedType ) )
					{
						contains = true;
						break;
					}
				}
			}
			return contains;
		}

		static ContentBrowserItem_File FindCreatedItemByRealPath( ContentBrowser browser, string realPath )
		{
			foreach( var item in browser.GetAllItems() )
			{
				var fileItem = item as ContentBrowserItem_File;
				if( fileItem != null && string.Compare( fileItem.FullPath, realPath, true ) == 0 )
					return fileItem;
			}
			return null;
		}

		public static ContentBrowserItem_File GetItemByRealFilePath_WithCreationNotCreatedItems( ContentBrowser browser, string realPath )//, bool canExpandToCreateNotCreatedItems )
		{
			if( browser.DataItem == null )
				return null;
			if( !VirtualPathUtility.GetVirtualPathByReal( realPath, out var virtualPath ) )
				return null;
			if( string.IsNullOrEmpty( virtualPath ) )
				return browser.DataItem;

			//!!!!slowly

			browser.SelectItems( new ContentBrowser.Item[] { browser.DataItem }, true );

			var strings = virtualPath.Split( new char[] { '\\', '/' } );
			var currentRealPath = VirtualFileSystem.Directories.Assets;
			for( int n = 0; n < strings.Length; n++ )
			{
				var str = strings[ n ];
				bool last = n == strings.Length - 1;

				currentRealPath = Path.Combine( currentRealPath, str );

				var item = FindCreatedItemByRealPath( browser, currentRealPath );
				if( item == null )
					return null;

				if( !last )
					browser.SelectItems( new ContentBrowser.Item[] { item }, true );
				else
					return item;
			}
			return null;
		}

		public static bool SelectFileItems( ContentBrowser browser, string[] realPaths, bool expandNodes )
		{
			var items = new List<ContentBrowserItem_File>();
			foreach( var realPath in realPaths )
			{
				var item = GetItemByRealFilePath_WithCreationNotCreatedItems( browser, realPath );
				if( item == null )
					return false;
				items.Add( item );
			}
			browser.SelectItems( items.ToArray(), expandNodes, true );
			return true;
		}

		public static ContentBrowserItem_Component GetItemByComponent_WithCreationNotCreatedItems( ContentBrowser browser, Component component )
		{
			var stackToExpand = new Stack<Component>();
			{
				var current = component.Parent;
				while( current != null )
				{
					stackToExpand.Push( current );

					var item = browser.FindItemByContainedObject( current ) as ContentBrowserItem_Component;
					if( item != null )
						break;

					current = current.Parent;
				}
			}

			//expand from stack
			while( stackToExpand.Count != 0 )
			{
				var c = stackToExpand.Pop();
				var item = browser.FindItemByContainedObject( c ) as ContentBrowserItem_Component;
				if( item != null )
					browser.SelectItems( new ContentBrowser.Item[] { item }, true );
			}

			return browser.FindItemByContainedObject( component ) as ContentBrowserItem_Component;
		}

		public static void SelectComponentItems( ContentBrowser browser, Component[] components )
		{
			var items = new List<ContentBrowserItem_Component>();
			foreach( var component in components )
			{
				var item = GetItemByComponent_WithCreationNotCreatedItems( browser, component );
				if( item != null )
					items.Add( item );
			}
			if( items.Count != 0 )
				browser.SelectItems( items.ToArray(), false, true );
		}

		/////////////////////////////////////////

		public static bool allContentBrowsers_SuspendChildrenChangedEvent;
		public static ESet<(ContentBrowser, ContentBrowser.Item)> allContentBrowsers_SuspendChildrenChangedEvent_Items = new ESet<(ContentBrowser, ContentBrowser.Item)>();

		public static void AllContentBrowsers_SuspendChildrenChangedEvent()
		{
			allContentBrowsers_SuspendChildrenChangedEvent = true;
		}

		public static void AllContentBrowsers_ResumeChildrenChangedEvent()
		{
			allContentBrowsers_SuspendChildrenChangedEvent = false;

			var copy = allContentBrowsers_SuspendChildrenChangedEvent_Items.ToArray();
			allContentBrowsers_SuspendChildrenChangedEvent_Items.Clear();
			foreach( var tuple in copy )
			{
				var browser = tuple.Item1;
				var item = tuple.Item2;

				browser.Item_ChildrenChanged( item );
			}
		}
	}
}
