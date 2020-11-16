// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	//!!!!names

	public class EditorDocumentWindowAttribute : Attribute
	{
		Type documentClass;
		string documentClassName;

		public EditorDocumentWindowAttribute( Type documentClass )
		{
			this.documentClass = documentClass;
		}

		public EditorDocumentWindowAttribute( string documentClassName )
		{
			this.documentClassName = documentClassName;
		}

		public Type DocumentClass
		{
			get { return documentClass; }
		}

		public string DocumentClassName
		{
			get { return documentClassName; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	[AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]
	public class EditorSettingsCellAttribute : Attribute
	{
		Type settingsCellClass;
		bool multiselectionSupport;

		public EditorSettingsCellAttribute( Type settingsCellClass, bool multiselectionSupport = false )
		{
			this.settingsCellClass = settingsCellClass;
			this.multiselectionSupport = multiselectionSupport;
		}

		public Type SettingsCellClass
		{
			get { return settingsCellClass; }
		}

		public bool MultiselectionSupport
		{
			get { return multiselectionSupport; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class EditorPreviewControlAttribute : Attribute
	{
		Type previewClass;
		string previewClassName;

		public EditorPreviewControlAttribute( Type previewClass )
		{
			this.previewClass = previewClass;
		}

		public EditorPreviewControlAttribute( string previewClassName )
		{
			this.previewClassName = previewClassName;
		}

		public Type PreviewClass
		{
			get { return previewClass; }
		}

		public string PreviewClassName
		{
			get { return previewClassName; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class EditorPreviewImageAttribute : Attribute
	{
		Type previewClass;
		string previewClassName;

		public EditorPreviewImageAttribute( Type previewClass )
		{
			this.previewClass = previewClass;
		}

		public EditorPreviewImageAttribute( string previewClassName )
		{
			this.previewClassName = previewClassName;
		}

		public Type PreviewClass
		{
			get { return previewClass; }
		}

		public string PreviewClassName
		{
			get { return previewClassName; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class EditorNewObjectSettingsAttribute : Attribute
	{
		Type settingsClass;

		public EditorNewObjectSettingsAttribute( Type settingsClass )
		{
			this.settingsClass = settingsClass;
		}

		public Type SettingsClass
		{
			get { return settingsClass; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class EditorNewObjectCellAttribute : Attribute
	{
		Type cellClass;

		public EditorNewObjectCellAttribute( Type cellClass )
		{
			this.cellClass = cellClass;
		}

		public Type CellClass
		{
			get { return cellClass; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!new
	public enum EditorRenderSelectionState
	{
		None,
		CanSelect,
		Selected,
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Provides data to implement drag and drop functionaly for references.
	/// </summary>
	public class DragDropSetReferenceData
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
	public abstract class EditorExtensions
	{
		public abstract void Register();
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Represents a collection of focuced objects in the document window.
	/// </summary>
	public class ObjectsInFocus
	{
		//public DockWindow dockWindow;
		public DocumentWindow DocumentWindow;
		public object[] Objects;

		public ObjectsInFocus( /*DockWindow dockWindow,*/ DocumentWindow documentWindow, object[] objects )
		{
			//this.dockWindow = dockWindow;
			this.DocumentWindow = documentWindow;
			this.Objects = objects;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	[AttributeUsage( AttributeTargets.Class )]
	public class AddToResourcesWindowAttribute : Attribute
	{
		string path;
		double sortOrder;
		bool disabled;

		public AddToResourcesWindowAttribute( string path, double sortOrder = 0, bool disabled = false )
		{
			this.path = path;
			this.sortOrder = sortOrder;
			this.disabled = disabled;
		}

		public string Path
		{
			get { return path; }
		}

		public double SortOrder
		{
			get { return sortOrder; }
		}

		public bool Disabled
		{
			get { return disabled; }
		}

		//string group;
		//string displayNameOptional;

		//public AddToResourcesWindowAttribute( string group, string displayNameOptional = "" )
		//{
		//	this.group = group;
		//	this.displayNameOptional = displayNameOptional;
		//}

		//public string Group
		//{
		//	get { return group; }
		//}

		//public string DisplayNameOptional
		//{
		//	get { return displayNameOptional; }
		//}
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
	/// An interface provides the ability to inform the change document to objects.
	/// </summary>
	public interface IComponent_EditorUpdateWhenDocumentModified
	{
		void EditorUpdateWhenDocumentModified();
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An attribute to mark dependent property from another. Used when working with the list of properties in the Settings Window.
	/// </summary>
	public class UndoDependentPropertyAttribute : Attribute
	{
		string propertyName;

		public UndoDependentPropertyAttribute( string propertyName )
		{
			this.propertyName = propertyName;
		}

		public string PropertyName
		{
			get { return propertyName; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An attribute to mark components to show warning when component creating if another component with same type already exists.
	/// </summary>
	public class WhenCreatingShowWarningIfItAlreadyExistsAttribute : Attribute
	{
		public WhenCreatingShowWarningIfItAlreadyExistsAttribute()
		{
		}
	}

}
