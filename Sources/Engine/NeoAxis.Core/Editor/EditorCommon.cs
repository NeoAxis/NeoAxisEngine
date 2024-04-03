// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class EditorControlAttribute : Attribute
	{
		Type documentClass;
		string documentClassName;
		bool onlyWhenRootComponent;

		public EditorControlAttribute( Type documentClass, bool onlyWhenRootComponent = false )
		{
			this.documentClass = documentClass;
			this.onlyWhenRootComponent = onlyWhenRootComponent;
		}

		public EditorControlAttribute( string documentClassName, bool onlyWhenRootComponent = false )
		{
			this.documentClassName = documentClassName;
			this.onlyWhenRootComponent = onlyWhenRootComponent;
		}

		public Type DocumentClass
		{
			get { return documentClass; }
		}

		public string DocumentClassName
		{
			get { return documentClassName; }
		}

		public bool OnlyWhenRootComponent
		{
			get { return onlyWhenRootComponent; }
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	[AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]
	public class SettingsCellAttribute : Attribute
	{
		Type settingsCellClass;
		string settingsCellClassName;
		bool multiselectionSupport;

		public SettingsCellAttribute( Type settingsCellClass, bool multiselectionSupport = false )
		{
			this.settingsCellClass = settingsCellClass;
			this.multiselectionSupport = multiselectionSupport;
		}

		public SettingsCellAttribute( string settingsCellClassName, bool multiselectionSupport = false )
		{
			this.settingsCellClassName = settingsCellClassName;
			this.multiselectionSupport = multiselectionSupport;
		}

		public Type SettingsCellClass
		{
			get { return settingsCellClass; }
		}

		public string SettingsCellClassName
		{
			get { return settingsCellClassName; }
		}

		public bool MultiselectionSupport
		{
			get { return multiselectionSupport; }
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class PreviewAttribute : Attribute
	{
		Type previewClass;
		string previewClassName;

		public PreviewAttribute( Type previewClass )
		{
			this.previewClass = previewClass;
		}

		public PreviewAttribute( string previewClassName )
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

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class PreviewImageAttribute : Attribute
	{
		Type previewClass;
		string previewClassName;

		public PreviewImageAttribute( Type previewClass )
		{
			this.previewClass = previewClass;
		}

		public PreviewImageAttribute( string previewClassName )
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

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class NewObjectSettingsAttribute : Attribute
	{
		Type settingsClass;
		string settingsClassName;

		public NewObjectSettingsAttribute( Type settingsClass )
		{
			this.settingsClass = settingsClass;
		}

		public NewObjectSettingsAttribute( string settingsClassName )
		{
			this.settingsClassName = settingsClassName;
		}

		public Type SettingsClass
		{
			get { return settingsClass; }
		}

		public string SettingsClassName
		{
			get { return settingsClassName; }
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class NewObjectCellAttribute : Attribute
	{
		Type cellClass;

		public NewObjectCellAttribute( Type cellClass )
		{
			this.cellClass = cellClass;
		}

		public Type CellClass
		{
			get { return cellClass; }
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum RenderSelectionState
	{
		None,
		CanSelect,
		Selected,
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Auxiliary class to work with clipboard of the system.
	/// </summary>
	public abstract class EditorExtensions
	{
		public virtual void OnRegister() { }
		public virtual void OnUnregister() { }
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An interface provides the ability to inform the change document to objects.
	/// </summary>
	public interface IEditorUpdateWhenDocumentModified
	{
		void EditorUpdateWhenDocumentModified();
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An attribute to mark components to show warning when component creating if another component with same type already exists.
	/// </summary>
	public class WhenCreatingShowWarningIfItAlreadyExistsAttribute : Attribute
	{
		public WhenCreatingShowWarningIfItAlreadyExistsAttribute()
		{
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class RestoreDockWindowAfterEditorReloadAttribute : Attribute
	{
		public RestoreDockWindowAfterEditorReloadAttribute()
		{
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum StoreProductLicense
	{
		None,
		MIT,
		[DisplayNameEnum( "CC Attribution" )]
		CCAttribution,
		[DisplayNameEnum( "CC Attribution BY-SA" )]
		CCAttributionBYSA,
		[DisplayNameEnum( "CC Attribution BY-ND" )]
		CCAttributionBYND,
		[DisplayNameEnum( "CC Attribution BY-NC" )]
		CCAttributionBYNC,
		[DisplayNameEnum( "CC Attribution BY-NC-SA" )]
		CCAttributionBYNCSA,
		[DisplayNameEnum( "CC Attribution BY-NC-ND" )]
		CCAttributionBYNCND,
		FreeToUse,
		[DisplayNameEnum( "Free To Use With NeoAxis" )]
		FreeToUseWithNeoAxis,
		PaidPerSeat,
		//PaidThirdParty,
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface IDocumentInstance
	{
		string Name { get; }
		string RealFileName { get; }
		string SpecialMode { get; }
		Component ResultComponent { get; }
		object ResultObject { get; }
		bool Modified { get; set; }
		UndoSystem UndoSystem { get; }
		bool AllowUndoRedo { get; set; }
		bool Destroyed { get; }

		void CommitUndoAction( UndoSystem.Action action, bool setModified = true );
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Represents a collection of focuced objects in the document window.
	/// </summary>
	public class ObjectsInFocus
	{
		public IDocumentWindow DocumentWindow;
		public object[] Objects;

		public ObjectsInFocus( IDocumentWindow documentWindow, object[] objects )
		{
			DocumentWindow = documentWindow;
			Objects = objects;
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum EditorActionHolder
	{
		RibbonQAT,
		ContextMenu,//!!!!потом может еще быть какое именно контекстное меню
		ShortcutKey,
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class EditorActionGetStateContext
	{
		EditorActionHolder holder;
		ObjectsInFocus objectsInFocus;
		IEditorAction action;

		//

		internal EditorActionGetStateContext( EditorActionHolder holder, ObjectsInFocus objectsInFocus, IEditorAction action )
		{
			this.holder = holder;
			this.objectsInFocus = objectsInFocus;
			this.action = action;
		}

		public EditorActionHolder Holder
		{
			get { return holder; }
		}

		public ObjectsInFocus ObjectsInFocus
		{
			get { return objectsInFocus; }
		}

		public IEditorAction Action
		{
			get { return action; }
		}

		public bool Enabled { get; set; }
		public bool Checked { get; set; }
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class EditorActionClickContext
	{
		EditorActionHolder holder;
		ObjectsInFocus objectsInFocus;
		IEditorAction action;

		//

		internal EditorActionClickContext( EditorActionHolder holder, ObjectsInFocus objectsInFocus, IEditorAction action )
		{
			this.holder = holder;
			this.objectsInFocus = objectsInFocus;
			this.action = action;
		}

		public EditorActionHolder Holder
		{
			get { return holder; }
		}

		public ObjectsInFocus ObjectsInFocus
		{
			get { return objectsInFocus; }
		}

		public IEditorAction Action
		{
			get { return action; }
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//#if !DEPLOY
	public interface IEditorAction
	{
		string Name { get; }
		ProjectSettingsPage_Shortcuts.Keys2[] ShortcutKeys { get; }

		bool QatSupport { get; }
		bool QatAddByDefault { get; }

		string ContextMenuText { get; }
		object UserData { get; }
		bool CompletelyDisabled { get; }
	}
	//#endif

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public delegate bool EditorRibbonDefaultConfigurationTabVisibleConditionDelegate();

	public interface IEditorRibbonDefaultConfigurationTab
	{
		string Name { get; }
		string Type { get; }

		Metadata.TypeInfo VisibleOnlyForType { get; }

		EditorRibbonDefaultConfigurationTabVisibleConditionDelegate VisibleCondition { get; }

		IEditorRibbonDefaultConfigurationGroup[] Groups { get; }
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface IEditorRibbonDefaultConfigurationGroup
	{
		string Name { get; }

		List<object> Children { get; }
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface IDragDropSetReferenceData
	{
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface IDockWindow
	{
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface IEngineViewportControl
	{
		Viewport Viewport { get; }
		Viewport.CameraSettingsClass OverrideCameraSettings { get; set; }
		bool AllowCreateRenderWindow { get; set; }
		bool Visible { get; set; }
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public delegate void DocumentWindowSelectedObjectsChangedDelegate( IDocumentWindow sender, object[] oldSelectedObjects );

	/// <summary>
	/// Provides access to the window of the document.
	/// </summary>
	public interface IDocumentWindow : IDockWindow
	{
		IDocumentInstance Document { get; }
		object[] SelectedObjects { get; }
		ESet<object> SelectedObjectsSet { get; }
		object ObjectOfWindow { get; }
		bool OpenAsSettings { get; }
		Dictionary<string, object> WindowTypeSpecificOptions { get; }
		bool IsWindowInWorkspace { get; set; }

		event DocumentWindowSelectedObjectsChangedDelegate SelectedObjectsChanged;

		bool SaveDocument();
		bool IsObjectSelected( object obj );
		bool IsDocumentSaved();
		bool Focus();
		void SelectObjects( ICollection<object> objects, bool updateForeachDocumentWindowContainers = true, bool updateSettingsWindowSelectObjects = true, bool forceUpdate = false );
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Provides access to the window of the document (with viewport).
	/// </summary>
	public interface IDocumentWindowWithViewport : IDocumentWindow
	{
		IEngineViewportControl ViewportControl { get; }
		Viewport Viewport { get; }

		Scene Scene { get; set; }
		bool SceneNeedDispose { get; set; }
		bool CameraRotating { get; }
		string WorkareaModeName { get; }
		bool AllowCameraControl { get; }
		bool AllowSelectObjects { get; }
		bool DisplaySelectedObjects { get; }

		Scene CreateScene( bool enable );
		void DestroyScene();

		void WorkareaModeSet( string name, DocumentWindowWithViewportWorkareaMode instance = null );

		void AddScreenMessage( string text, ColorValue color );
		void AddScreenMessage( string text );

		double GetFontSize();
		void AddTextWithShadow( FontComponent font, double fontSize, string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
		void AddTextWithShadow( string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
		void AddTextLinesWithShadow( FontComponent font, double fontSize, IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
		void AddTextLinesWithShadow( IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
		int AddTextWordWrapWithShadow( FontComponent font, double fontSize, string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
		int AddTextWordWrapWithShadow( string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Provides access to the preview control of the document.
	/// </summary>
	public interface IPreviewControl
	{
		object ObjectOfPreview { get; }
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Provides access to the preview control of the document (with viewport).
	/// </summary>
	public interface IPreviewControlWithViewport : IPreviewControl
	{
		IEngineViewportControl ViewportControl { get; }
		Viewport Viewport { get; }

		Scene Scene { get; set; }
		bool SceneNeedDispose { get; set; }
		Vector3 CameraLookTo { get; set; }
		double CameraInitialDistance { get; set; }
		bool CameraRotationMode { get; set; }
		SphericalDirection CameraDirection { get; set; }

		Scene CreateScene( bool enable );
		void DestroyScene();
		void SetCameraByBounds( Bounds bounds, double distanceScale, bool mode2D );
		double GetFontSize();
		void AddTextWithShadow( FontComponent font, double fontSize, string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
		void AddTextWithShadow( string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
		void AddTextLinesWithShadow( FontComponent font, double fontSize, IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
		void AddTextLinesWithShadow( IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
		int AddTextWordWrapWithShadow( FontComponent font, double fontSize, string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
		int AddTextWordWrapWithShadow( string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color );
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum FormSizeType //same as winforms SizeType
	{
		AutoSize,
		Absolute,
		Percent
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface ISettingsCell
	{
		ISettingsProvider Provider { get; }
		float CellsSortingPriority { get; set; }
		FormSizeType SizeType { get; set; }

		T[] GetObjects<T>() where T : class;
		T GetFirstObject<T>() where T : class;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface ISettingsProvider
	{
		IDocumentWindow DocumentWindow { get; }
		object[] SelectedObjects { get; }
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface ISceneEditor : IDocumentWindowWithViewport
	{
		ITransformTool TransformTool { get; }

		void ResetWorkareaMode();

		void GetMouseOverObjectToSelectByClick( SceneEditorGetMouseOverObjectToSelectByClickContext context );
		void GetMouseOverObjectInSpaceToSelectByClick( SceneEditorGetMouseOverObjectToSelectByClickContext context );
		object GetMouseOverObjectToSelectByClick( out SceneEditorGetMouseOverObjectToSelectByClickContext context );
		object GetMouseOverObjectToSelectByClick();
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class SelectTypeWindowInitData
	{
		public IDocumentWindow DocumentWindow;
		public Metadata.TypeInfo[] DemandedTypes;
		public bool CanSelectNull;
		public bool CanSelectAbstractClass;

		public delegate void WasSelectedDelegate( Metadata.TypeInfo selectedType, ref bool cancel );
		public WasSelectedDelegate WasSelected;
	}

}
