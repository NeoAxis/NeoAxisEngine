// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Provides data to implement drag and drop functionaly for references.
	/// </summary>
	public class DragDropSetReferenceData : IDragDropSetReferenceData
	{
		public DocumentInstance document;
		public Component[] controlledComponents;
		public object[] propertyOwners;
		//public Component[] controlledObjects;
		public Metadata.Property property;
		public object[] indexers;

		public void SetProperty( string[] referenceValues )
		{
			EditorUtility.SetPropertyReference( document, propertyOwners/*controlledObjects*/, property, indexers, referenceValues );
		}

		////!!!!TypeInfo?
		//public Type GetDemandedType()
		//{
		//	var netType = ReferenceUtils.GetUnreferencedType( property.Type.GetNetType() );
		//	//if( ReferenceUtils.IsReferenceType( netType ) )
		//	//	return ReferenceUtils.GetUnderlyingType( netType );
		//	return netType;
		//}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//public class DragDropSetResourceData
	//{
	//	public DocumentInstance document;
	//	public object[] controlledObjects;
	//	public Metadata.Property property;
	//	public object[] indexers;

	//	ResourceSelectionMode? selectionMode;
	//	public ResourceSelectionMode SelectionMode
	//	{
	//		get
	//		{
	//			if( selectionMode == null )
	//				selectionMode = ResourceUtils.GetSelectionModeByPropertyAttributes( property );
	//			return selectionMode.Value;
	//		}
	//	}

	//	public void SetProperty( string resourceName )
	//	{
	//		EditorUtils.SetPropertyResourceName( document, controlledObjects, property, indexers, resourceName );
	//	}

	//	////!!!!TypeInfo?
	//	//public Type GetDemandedType()
	//	//{
	//	//	var netType = ReferenceUtils.GetUnreferencedType( property.Type.GetNetType() );
	//	//	//if( ReferenceUtils.IsReferenceType( netType ) )
	//	//	//	return ReferenceUtils.GetUnderlyingType( netType );
	//	//	return netType;
	//	//}
	//}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Auxiliary class to work with clipboard of the system.
	/// </summary>
	public static class ClipboardManager
	{
		static object currentObject;

		public static void CopyToClipboard<T>( T objectToCopy ) where T : class
		{
			var format = DataFormats.GetFormat( typeof( T ).FullName );

			currentObject = objectToCopy;

			var dataObject = new DataObject();
			dataObject.SetData( format.Name, false, "NeoAxis.ClipboardManager" );
			Clipboard.SetDataObject( dataObject, false );
		}

		public static bool CheckAvailableInClipboard<T>() where T : class
		{
			if( currentObject != null && currentObject is T )
			{
				var dataObject = Clipboard.GetDataObject();
				var format = DataFormats.GetFormat( typeof( T ).FullName );

				if( dataObject.GetDataPresent( format.Name ) )
					return true;
			}
			return false;
		}

		public static T GetFromClipboard<T>() where T : class
		{
			if( currentObject != null && currentObject is T )
			{
				var dataObject = Clipboard.GetDataObject();
				var format = DataFormats.GetFormat( typeof( T ).FullName );

				if( dataObject.GetDataPresent( format.Name ) && dataObject.GetData( format.Name ) as string == "NeoAxis.ClipboardManager" )
				{
					var result = currentObject;
					currentObject = null;
					return (T)result;
				}
			}
			return null;
		}

		public static void Clear()
		{
			Clipboard.Clear();
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Data for storing in clipboard of the system to implement cut/copy/paste functionality.
	/// </summary>
	public class ObjectCutCopyPasteData
	{
		public DocumentWindow documentWindow;
		//public DocumentInstance document;
		public bool cut;
		public object[] objects;

		public ObjectCutCopyPasteData( DocumentWindow documentWindow, bool cut, object[] objects )
		{
			this.documentWindow = documentWindow;
			this.cut = cut;
			this.objects = objects;
		}
	}
}
