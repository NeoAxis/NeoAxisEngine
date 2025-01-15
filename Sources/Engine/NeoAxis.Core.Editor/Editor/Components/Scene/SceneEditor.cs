// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using Internal.ComponentFactory.Krypton.Toolkit;
using System.Drawing;

namespace NeoAxis.Editor
{
	public partial class SceneEditor : DocumentWindowWithViewport, ISceneEditor
	{
		[EngineConfig( "Scene Editor", "CreateObjectsMode" )]
		public static CreateObjectsModeEnum CreateObjectsMode = CreateObjectsModeEnum.Drop;
		[EngineConfig( "Scene Editor", "CreateObjectsBrushRadius" )]
		public static double CreateObjectsBrushRadius = 1;//5;
		[EngineConfig( "Scene Editor", "CreateObjectsBrushStrength" )]
		public static double CreateObjectsBrushStrength = 0.5;
		//!!!!с масками не нужно
		[EngineConfig( "Scene Editor", "CreateObjectsBrushHardness" )]
		public static double CreateObjectsBrushHardness = 0.5;

		[EngineConfig( "Terrain Editor", "TerrainToolShape" )]
		public static TerrainEditingMode.TerrainEditorToolShape TerrainToolShape = TerrainEditingMode.TerrainEditorToolShape.Circle;
		[EngineConfig( "Terrain Editor", "TerrainToolRadius" )]
		public static double TerrainToolRadius = 3;//5;
		[EngineConfig( "Terrain Editor", "TerrainToolStrength" )]
		public static double TerrainToolStrength = 0.5;
		[EngineConfig( "Terrain Editor", "TerrainToolHardness" )]
		public static double TerrainToolHardness = 0.5;


		//Select by rectangle
		bool selectByRectangle_Enabled;
		bool selectByRectangle_Activated;
		Vector2 selectByRectangle_StartPosition;
		Vector2 selectByRectangle_LastMousePosition;

		//Transform tool
		TransformToolClass transformTool;
		bool transformToolModifyCloned;
		bool transformToolNeedCallOnMouseMove;
		Vector2 transformToolNeedCallOnMouseMovePosition;
		internal TransformToolMode transformToolModeRestore = TransformToolMode.PositionRotation;

		//createObjectsDestination
		List<(CreateObjectsDestinationModeEnum Mode, Component Obj, string Text)> createObjectsDestinationCachedList = new List<(CreateObjectsDestinationModeEnum, Component, string)>();
		double createObjectsDestinationLastUpdateTime;
		(CreateObjectsDestinationModeEnum Mode, Component Obj) createObjectsDestinationSelected;

		//terrainPaintLayers
		List<(PaintLayer Obj, string Text)> terrainPaintLayersCachedList = new List<(PaintLayer, string)>();
		double terrainPaintLayersLastUpdateTime;
		PaintLayer terrainPaintLayersSelected;

		//create drag and drop or by click
		bool createByDropEntered;
		bool createByClickEntered;
		bool createByBrushEntered;
		Metadata.TypeInfo objectTypeToCreate;
		Component objectToCreate;
		object createSetPropertyObject;
		Metadata.Property createSetProperty;
		object[] createSetPropertyIndexes;
		IReference createSetPropertyOldValue;
		Component createSetPropertyVisualizeCanSelectObject;

		bool createByBrushPaint;
		bool createByBrushPaintDeleting;
		float createByBrushPaintTimer;
		GroupOfObjects createByBrushGroupOfObjects;
		List<int> createByBrushGroupOfObjectsObjectsCreated = new List<int>();
		List<GroupOfObjects.Object> createByBrushGroupOfObjectsObjectsDeleted = new List<GroupOfObjects.Object>();
		List<Component> createByBrushComponentsCreated = new List<Component>();
		List<(Component obj, bool wasEnabled)> createByBrushComponentsToDelete = new List<(Component obj, bool wasEnabled)>();

		bool skipSelectionByButtonInMouseUp;

		EngineToolTip screenLabelToolTip = new EngineToolTip();
		Component screenLabelToolTipComponent;
		string screenLabelToolTipText = "";

		public static List<Type> CanvasWidgetsToCreate { get; } = new List<Type>();
		public List<CanvasWidget> CanvasWidgets { get; } = new List<CanvasWidget>();

		/////////////////////////////////////

		public enum CreateObjectsModeEnum
		{
			Drop,
			Click,
			Brush
		}

		/////////////////////////////////////////

		enum CreateObjectsDestinationModeEnum
		{
			Auto,

			SeparateObjectsToRoot,
			SeparateObjectsToLayer,
			ToGroupOfObjects,
			//!!!!ToPaintLayer,
		}

		/////////////////////////////////////////

		//public abstract class SceneEditorWorkareaModeClass : DocumentWindowWithViewportWorkareaModeClass
		//{
		//	protected SceneEditorWorkareaModeClass( SceneEditor documentWindow )
		//		: base( documentWindow )
		//	{
		//	}

		//	public new SceneEditor DocumentWindow
		//	{
		//		get { return (SceneEditor)base.DocumentWindow; }
		//	}

		//	protected virtual bool OnGetObjectsToSelectByRectangle( Rectangle rectangle, ref List<object> objectsToSelect ) { return false; }
		//	public delegate void GetObjectsToSelectByRectangleDelegate( DocumentWindowWithViewportWorkareaModeClass sender, Rectangle rectangle, ref bool handled, ref List<object> objectsToSelect );
		//	public event GetObjectsToSelectByRectangleDelegate GetObjectsToSelectByRectangle;
		//	internal bool PerformGetObjectsToSelectByRectangle( Rectangle rectangle, ref List<object> objectsToSelect )
		//	{
		//		var handled = OnGetObjectsToSelectByRectangle( rectangle, ref objectsToSelect );
		//		if( !handled )
		//			GetObjectsToSelectByRectangle?.Invoke( this, rectangle, ref handled, ref objectsToSelect );
		//		return handled;
		//	}

		//	protected virtual bool OnGetMouseOverObjectToSelectByClick( GetMouseOverObjectToSelectByClickContext context ) { return false; }
		//	public delegate void GetMouseOverObjectToSelectByClickDelegate( DocumentWindowWithViewportWorkareaModeClass sender, GetMouseOverObjectToSelectByClickContext context );
		//	public event GetMouseOverObjectToSelectByClickDelegate GetMouseOverObjectToSelectByClick;
		//	internal bool PerformGetMouseOverObjectToSelectByClick( GetMouseOverObjectToSelectByClickContext context )
		//	{
		//		var handled = OnGetMouseOverObjectToSelectByClick( context );
		//		if( !handled )
		//			GetMouseOverObjectToSelectByClick?.Invoke( this, context );
		//		return handled;
		//	}

		//	protected virtual bool OnTransformToolCreateObject( object forObject, ref TransformToolObject transformToolObject ) { return false; }
		//	public delegate void TransformToolCreateObjectDelegate( DocumentWindowWithViewportWorkareaModeClass sender, object forObject, ref bool handled, ref TransformToolObject transformToolObject );
		//	public event TransformToolCreateObjectDelegate TransformToolCreateObject;
		//	internal bool PerformTransformToolCreateObject( object forObject, ref TransformToolObject transformToolObject )
		//	{
		//		var handled = OnTransformToolCreateObject( forObject, ref transformToolObject );
		//		if( !handled )
		//			TransformToolCreateObject?.Invoke( this, forObject, ref handled, ref transformToolObject );
		//		return handled;
		//	}

		//	protected virtual bool OnTransformToolModifyBegin() { return false; }
		//	public delegate void TransformToolModifyBeginDelegate( DocumentWindowWithViewportWorkareaModeClass sender, ref bool handled );
		//	public event TransformToolModifyBeginDelegate TransformToolModifyBegin;
		//	internal bool PerformTransformToolModifyBegin()
		//	{
		//		var handled = OnTransformToolModifyBegin();
		//		if( !handled )
		//			TransformToolModifyBegin?.Invoke( this, ref handled );
		//		return handled;
		//	}

		//	protected virtual bool OnTransformToolModifyCommit() { return false; }
		//	public delegate void TransformToolModifyCommitDelegate( DocumentWindowWithViewportWorkareaModeClass sender, ref bool handled );
		//	public event TransformToolModifyCommitDelegate TransformToolModifyCommit;
		//	internal bool PerformTransformToolModifyCommit()
		//	{
		//		var handled = OnTransformToolModifyCommit();
		//		if( !handled )
		//			TransformToolModifyCommit?.Invoke( this, ref handled );
		//		return handled;
		//	}

		//	protected virtual bool OnTransformToolModifyCancel() { return false; }
		//	public delegate void TransformToolModifyCancelDelegate( DocumentWindowWithViewportWorkareaModeClass sender, ref bool handled );
		//	public event TransformToolModifyCancelDelegate TransformToolModifyCancel;
		//	internal bool PerformTransformToolModifyCancel()
		//	{
		//		var handled = OnTransformToolModifyCancel();
		//		if( !handled )
		//			TransformToolModifyCancel?.Invoke( this, ref handled );
		//		return handled;
		//	}

		//	protected virtual bool OnTransformToolCloneAndSelectObjects() { return false; }
		//	public delegate void TransformToolCloneAndSelectObjectsDelegate( DocumentWindowWithViewportWorkareaModeClass sender, ref bool handled );
		//	public event TransformToolCloneAndSelectObjectsDelegate TransformToolCloneAndSelectObjects;
		//	internal bool PerformTransformToolCloneAndSelectObjects()
		//	{
		//		var handled = OnTransformToolCloneAndSelectObjects();
		//		if( !handled )
		//			TransformToolCloneAndSelectObjects?.Invoke( this, ref handled );
		//		return handled;
		//	}
		//}

		/////////////////////////////////////////

		public abstract class CanvasWidget
		{
			SceneEditor window;

			//

			protected CanvasWidget( SceneEditor window )
			{
				this.window = window;
			}

			public SceneEditor Window
			{
				get { return window; }
			}

			public abstract void OnUpdate( SceneEditor window, ref double screenPositionY );
		}

		/////////////////////////////////////////

		static SceneEditor()
		{
			EngineConfig.RegisterClassParameters( typeof( SceneEditor ) );
		}

		public SceneEditor()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			var scene = ObjectOfWindow as Scene;
			if( scene != null )
			{
				Scene = scene;
				SceneNeedDispose = false;

				scene.ViewportUpdateGetCameraSettings += Scene_ViewportUpdateGetCameraSettings;
			}
			else
				Log.Fatal( "scene == null" );

			transformTool = new TransformToolClass( ViewportControl2 );
			transformTool.Mode = TransformToolMode.PositionRotation;

			transformTool.ModifyBegin += TransformToolModifyBegin;
			transformTool.ModifyCommit += TransformToolModifyCommit;
			transformTool.ModifyCancel += TransformToolModifyCancel;
			transformTool.CloneAndSelectObjects += TransformToolCloneAndSelectObjects;

			ViewportControl2.MouseEnter += ViewportControl_MouseEnter;
			ViewportControl2.MouseLeave += ViewportControl_MouseLeave;

			if( ObjectOfWindow != null )
				SelectObjects( new object[] { ObjectOfWindow } );

			Scene.RenderEvent += Scene_RenderEvent;
		}

		protected override void OnDestroy()
		{
			if( Scene != null )
				Scene.RenderEvent -= Scene_RenderEvent;

			if( transformTool != null )
			{
				transformTool.ModifyBegin -= TransformToolModifyBegin;
				transformTool.ModifyCommit -= TransformToolModifyCommit;
				transformTool.ModifyCancel -= TransformToolModifyCancel;
				transformTool.CloneAndSelectObjects -= TransformToolCloneAndSelectObjects;
			}

			screenLabelToolTip?.Dispose();

			base.OnDestroy();
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );

			////connect scene to viewport
			//ViewportControl.Viewport.AttachedScene = Scene;

			foreach( var type in CanvasWidgetsToCreate )
			{
				try
				{
					var widget = (CanvasWidget)type.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { this } );
					CanvasWidgets.Add( widget );
				}
				catch { }
			}
		}

		protected override void Viewport_KeyDown( Viewport viewport, KeyEvent e, ref bool handled )
		{
			base.Viewport_KeyDown( viewport, e, ref handled );
			if( handled )
				return;

			//Inside only Escape key is processed.
			transformTool.PerformKeyDown( e, ref handled );
			if( handled )
				return;

			//create by brush cancel by Escape
			if( e.Key == EKeys.Escape )
			{
				if( createByBrushPaint )
				{
					CreateByBrushPaintEnd( true );
					handled = true;
					return;
				}
			}

			//disable workarea mode by Space or Escape
			if( ( e.Key == EKeys.Space || e.Key == EKeys.Escape ) )//&& !toolModify )
			{
				if( ( CreateObjectsMode == CreateObjectsModeEnum.Click && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick ) ) || ( CreateObjectsMode == CreateObjectsModeEnum.Brush && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByBrush ) ) )
				{
					if( CreateObjectsMode == CreateObjectsModeEnum.Click )
						CreateByClickCancel();
					if( CreateObjectsMode == CreateObjectsModeEnum.Brush )
						CreateByBrushCancel();
					EditorAPI2.ResetSelectedObjectToCreate();
					handled = true;
					return;
				}

				//ChangeCreateObjectsMode( CreateObjectsModeEnum.Drop );
				//handled = true;
				//return;
			}
		}

		protected override void Viewport_KeyPress( Viewport viewport, KeyPressEvent e, ref bool handled )
		{
			base.Viewport_KeyPress( viewport, e, ref handled );
			if( handled )
				return;
		}

		protected override void Viewport_KeyUp( Viewport viewport, KeyEvent e, ref bool handled )
		{
			base.Viewport_KeyUp( viewport, e, ref handled );

			transformTool.PerformKeyUp( e, ref handled );
		}

		static Type GetCreationModeType( ObjectCreationModeAttribute attrib )
		{
			if( !string.IsNullOrEmpty( attrib.CreationModeClassName ) )
			{
				var type = EditorUtility.GetTypeByName( attrib.CreationModeClassName );
				if( type == null )
					Log.Warning( $"SceneEditor: GetCreationModeType: Class with name \"{attrib.CreationModeClassName}\" is not found." );
				return type;
			}
			else
				return attrib.CreationModeClass;
		}

		public bool StartObjectCreationMode( Metadata.TypeInfo type, Component obj )
		{
			var attributes = type/*objectTypeToCreate*/.GetNetType()/* objectToCreate.GetType()*/.GetCustomAttributes<ObjectCreationModeAttribute>().ToArray();
			if( attributes.Length != 0 )
			{
				//begin creation mode

				var type2 = GetCreationModeType( attributes[ 0 ] );
				var mode = (ObjectCreationMode)type2.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { this, obj/*objectToCreate*/ } );

				ObjectCreationModeSet( mode );
				objectTypeToCreate = null;
				objectToCreate = null;
				createByClickEntered = false;
				return true;
			}

			return false;
		}

		public delegate void ViewportMouseDownAfterTransformToolDelegate( SceneEditor sender, Viewport viewport, EMouseButtons button, ref bool handled );
		public static event ViewportMouseDownAfterTransformToolDelegate Viewport_MouseDown_AfterTransformTool;

		protected override void Viewport_MouseDown( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			//code from Viewport.PerformMouseDown
			if( viewport.UIContainer != null && viewport.UIContainer.PerformMouseDown( button ) )
			{
				handled = true;
				return;
			}


			base.Viewport_MouseDown( viewport, button, ref handled );
			if( handled )
				return;

			//transform tool
			transformTool.PerformMouseDown( button, ref handled );
			if( handled )
				return;

			//Viewport_MouseDown_AfterTransformTool event
			{
				bool handled2 = false;
				Viewport_MouseDown_AfterTransformTool?.Invoke( this, viewport, button, ref handled2 );
				if( handled2 )
				{
					handled = true;
					return;
				}
			}

			//create by click
			if( CreateObjectsMode == CreateObjectsModeEnum.Click && button == EMouseButtons.Left && createByClickEntered && objectToCreate != null )
			{
				////creation mode
				//if( StartObjectCreationMode() )
				//{
				//	handled = true;
				//	return;
				//}
				{
					var attributes = objectTypeToCreate.GetNetType()/* objectToCreate.GetType()*/.GetCustomAttributes<ObjectCreationModeAttribute>().ToArray();
					if( attributes.Length != 0 )
					{
						//begin creation mode
						var type2 = GetCreationModeType( attributes[ 0 ] );
						var mode = (ObjectCreationMode)type2.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { this, objectToCreate } );
						ObjectCreationModeSet( mode );
						objectTypeToCreate = null;
						objectToCreate = null;
						createByClickEntered = false;
						handled = true;
						return;
					}
				}

				//commit creation
				if( CreateSetProperty_Commit() )
					CreateObject_Destroy();
				else
					CreateObject_Commit();
				createByClickEntered = false;

				//create new
				if( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick )
				{
					if( CreateByClickCreate() )
						createByClickEntered = true;
				}

				handled = true;
				return;
			}

			//creating objects paint
			if( button == EMouseButtons.Left )
			{
				if( CreateByBrushPaintBegin( viewport ) )
				{
					handled = true;
					return;
				}
			}

			//selection by rectangle
			if( button == EMouseButtons.Left && AllowSelectObjects )
			{
				selectByRectangle_Enabled = true;
				selectByRectangle_Activated = false;
				selectByRectangle_StartPosition = viewport.MousePosition;
				selectByRectangle_LastMousePosition = selectByRectangle_StartPosition;

				handled = true;
				return;
			}

			//always set handled to disable calling uiContainer.PerformMouseDown. it is already called in the beginning of this method
			handled = true;
		}

		public delegate void Viewport_MouseUp_AfterTransformToolDelegate( SceneEditor sender, Viewport viewport, EMouseButtons button, ref bool handled, ref bool skipSelectionByButtonInMouseUp );
		public static event Viewport_MouseUp_AfterTransformToolDelegate Viewport_MouseUp_AfterTransformTool;

		protected override void Viewport_MouseUp( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			//inside: cameraRotation
			base.Viewport_MouseUp( viewport, button, ref handled );
			//!!!!new. for workarea mode
			if( handled )
				return;

			//below:
			//transform tool
			//select by rectangle
			//context menu

			//transform tool
			transformTool.PerformMouseUp( button, ref handled );

			Viewport_MouseUp_AfterTransformTool?.Invoke( this, viewport, button, ref handled, ref skipSelectionByButtonInMouseUp );

			//!!!!что еще проверять?
			if( AllowSelectObjects && objectToCreate == null && !createByDropEntered && !createByClickEntered && !createByBrushEntered && !skipSelectionByButtonInMouseUp && !handled )
			{
				var selectedObjects = new ESet<object>( SelectedObjectsSet );

				//select by rectangle
				if( button == EMouseButtons.Left )
				{
					//reset selected objects even mode is not activated.
					bool shiftPressed = ( Form.ModifierKeys & Keys.Shift ) != 0;
					if( !shiftPressed )
						selectedObjects.Clear();

					if( selectByRectangle_Enabled && selectByRectangle_Activated )
					{
						foreach( var obj in SelectByRectangle_GetObjectsInSpace() )
							selectedObjects.AddWithCheckAlreadyContained( obj );
						handled = true;
					}

					//теперь ниже
					//selectByRectangle_Enabled = false;
					//selectByRectangle_Activated = false;
				}

				//select by click
				if( button == EMouseButtons.Left && !handled && !CameraRotating )
				{
					var obj = GetMouseOverObjectToSelectByClick();
					if( obj != null )
					{
						selectedObjects.AddWithCheckAlreadyContained( obj );
						handled = true;
					}
				}

				//update selected objects
				SelectObjects( selectedObjects );
			}

			if( button == EMouseButtons.Left && selectByRectangle_Enabled )
			{
				selectByRectangle_Enabled = false;
				selectByRectangle_Activated = false;
			}

			//!!!!тут?
			//creating objects paint
			if( button == EMouseButtons.Left )
			{
				if( CreateByBrushPaintEnd( false ) )
					handled = true;
			}

			//context menu
			if( !handled && button == EMouseButtons.Right )
				ShowContextMenu();

			if( button == EMouseButtons.Left )
				skipSelectionByButtonInMouseUp = false;
		}

		protected override void Viewport_MouseDoubleClick( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			base.Viewport_MouseDoubleClick( viewport, button, ref handled );
			if( handled )
				return;

			transformTool.PerformMouseDoubleClick( button, ref handled );
			if( handled )
				return;

			//select by double click
			//!!!!что еще проверять?
			if( button == EMouseButtons.Left && AllowSelectObjects && objectToCreate == null && !createByDropEntered && !createByClickEntered && !createByBrushEntered )
			{
				var selectedObjects = new ESet<object>( SelectedObjectsSet );

				var obj = GetMouseOverObjectToSelectByClick();
				if( obj != null && IsObjectSelected( obj ) )
				{
					var addObjects = SelectByDoubleClick( obj );
					if( addObjects.Length != 0 )
					{
						if( !ModifierKeys.HasFlag( Keys.Shift ) )
						{
							selectedObjects.Clear();
							selectedObjects.Add( obj );
						}
						selectedObjects.AddRangeWithCheckAlreadyContained( addObjects );

						//update selected objects
						SelectObjects( selectedObjects );

						handled = true;
						skipSelectionByButtonInMouseUp = true;
					}
				}
			}
		}

		protected override void Viewport_MouseMove( Viewport viewport, Vector2 mouse )
		{
			base.Viewport_MouseMove( viewport, mouse );

			transformToolNeedCallOnMouseMove = true;
			transformToolNeedCallOnMouseMovePosition = mouse;

			//update select by rectangle
			if( selectByRectangle_Enabled && AllowSelectObjects )
			{
				Vector2 diffPixels = ( viewport.MousePosition - selectByRectangle_StartPosition ) * viewport.SizeInPixels.ToVector2();
				if( Math.Abs( diffPixels.X ) >= 3 || Math.Abs( diffPixels.Y ) >= 3 )
					selectByRectangle_Activated = true;

				selectByRectangle_LastMousePosition = viewport.MousePosition;
			}

			//update create by click
			if( CreateObjectsMode == CreateObjectsModeEnum.Click && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick ) && createByClickEntered )
			{
				//для IMeshInSpaceChild потому как он создает при наведении
				//if( objectToCreate == null )
				//	DragDropCreateObject_Create( e );

				CreateObject_Update();
				//if( objectToCreate != null )
				//	e.Effect = DragDropEffects.Link;

				CreateSetProperty_Update( null );
				//if( createSetProperty != null )
				//	e.Effect = DragDropEffects.Link;

			}
		}

		protected override void Viewport_Tick( Viewport viewport, float delta )
		{
			base.Viewport_Tick( viewport, delta );

			UpdateCreateObjectsDestinationCachedList();
			UpdateTerrainPaintLayersCachedList();
			UpdateTerrainPaintLayersSelected();

			UpdateTransformToolObjects();
			UpdateTransformToolActiveState();

			if( transformToolNeedCallOnMouseMove )
			{
				transformTool.PerformMouseMove( transformToolNeedCallOnMouseMovePosition );
				transformToolNeedCallOnMouseMove = false;
			}
			transformTool.PerformTick( delta );

			CreateByBrushPaintTick( viewport, delta );

			UpdateScreenLabelTooltip();
		}

		public void GetMouseOverObjectInSpaceToSelectByClick( SceneEditorGetMouseOverObjectToSelectByClickContext context )
		{
			var viewport = ViewportControl2.Viewport;
			var mouse = viewport.MousePosition;

			if( !new Rectangle( 0, 0, 1, 1 ).Contains( mouse ) )
				return;
			if( viewport.MouseRelativeMode )
				return;

			//get by screen label
			foreach( var item in viewport.LastFrameScreenLabels.GetReverse() )
			{
				var obj = item.Object;
				var objectInSpace = obj as ObjectInSpace;

				if( objectInSpace != null && ( !context.CheckOnlyObjectsWithEnabledSelectionByCursorFlag || objectInSpace.EnabledSelectionByCursor ) || objectInSpace == null || item.AlwaysVisible )
				{
					bool found = false;

					switch( item.Shape )
					{
					case Viewport.LastFrameScreenLabelItem.ShapeEnum.Ellipse:
						found = MathAlgorithms.CheckPointInsideEllipse( item.ScreenRectangle, mouse );
						break;
					case Viewport.LastFrameScreenLabelItem.ShapeEnum.Rectangle:
						found = item.ScreenRectangle.Contains( mouse );
						break;
					}

					if( found )
					{
						context.ResultObject = obj;
						context.ScreenLabelItem = item;
						return;
					}

				}
			}

			//get by ray
			ObjectInSpace.CheckSelectionByRayContext rayContext = new ObjectInSpace.CheckSelectionByRayContext();
			rayContext.viewport = viewport;
			rayContext.screenPosition = mouse;
			rayContext.ray = viewport.CameraSettings.GetRayByScreenCoordinates( rayContext.screenPosition );

			var request = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, rayContext.ray );
			Scene.GetObjectsInSpace( request );
			var list = request.Result;

			ObjectInSpace foundObject = null;
			double foundScale = 0;

			//double scaleEpsilon = rayContext.ray.Direction.Length() / 100;

			foreach( var item in list )
			{
				var obj = item.Object;
				//var scale = item.DistanceScale;

				if( !context.CheckOnlyObjectsWithEnabledSelectionByCursorFlag || obj.EnabledSelectionByCursor )
				//if( obj.CanBeSelected )
				{
					//fast exit
					//!!!!slowly. можно всё не проверять
					{
					}

					//reset this object data
					rayContext.thisObjectWasChecked = false;
					rayContext.thisObjectResultRayScale = -10000000;

					obj.CheckSelectionByRay( rayContext );

					if( rayContext.thisObjectWasChecked && rayContext.thisObjectResultRayScale >= 0 )
					{
						var scale = rayContext.thisObjectResultRayScale;

						//if( context.thisObjectWasChecked )
						//{
						//	if( context.thisObjectResultRayScale >= 0 )
						//		scale = context.thisObjectResultRayScale;
						//	else
						//	{
						//		//skip this object
						//		continue;
						//	}
						//}

						bool foundNew;
						if( foundObject == null )
							foundNew = true;
						else
						{
							//!!!!
							if( Math.Abs( foundScale - scale ) < 0.0001 )
							//if( Math.Abs( foundScale - scale ) < scaleEpsilon )
							{
								var first = foundObject;
								var second = obj;
								bool secondIsParentOfFirst = first.GetAllParents().Contains( second );
								foundNew = secondIsParentOfFirst;
							}
							else
								foundNew = scale < foundScale;
						}

						if( foundNew )//if( foundObject == null || scale < foundScale )
						{
							foundObject = obj;
							foundScale = scale;
						}
					}
				}
			}

			context.ResultObject = foundObject;
			context.ResultPosition = rayContext.ray.GetPointOnRay( foundScale );
		}

		public virtual void GetMouseOverObjectToSelectByClick( SceneEditorGetMouseOverObjectToSelectByClickContext context )
		{
			var viewport = ViewportControl2.Viewport;
			var mouse = viewport.MousePosition;

			if( !new Rectangle( 0, 0, 1, 1 ).Contains( mouse ) )
				return;
			if( viewport.MouseRelativeMode )
				return;

			if( WorkareaMode != null && WorkareaMode.PerformGetMouseOverObjectToSelectByClick( context ) )
				return;
			GetMouseOverObjectInSpaceToSelectByClick( context );
		}

		public object GetMouseOverObjectToSelectByClick( out SceneEditorGetMouseOverObjectToSelectByClickContext context )
		{
			context = new SceneEditorGetMouseOverObjectToSelectByClickContext();
			GetMouseOverObjectToSelectByClick( context );
			return context.ResultObject;
		}

		public object GetMouseOverObjectToSelectByClick()
		{
			GetMouseOverObjectToSelectByClick( out var context );
			return context.ResultObject;
		}

		void AddScreenLabels( Viewport viewport, ref double screenPositionY )
		{
			var context = viewport.RenderingContext;
			var context2 = context.ObjectInSpaceRenderingContext;

			var settings = ProjectSettings.Get;
			var maxSize = settings.SceneEditor.ScreenLabelMaxSize.Value;
			Vector2 sizeInPixels = new Vector2( maxSize, maxSize );
			Vector2 screenSize = sizeInPixels / viewport.SizeInPixels.ToVector2();


			var list = new List<Component>();

			{
				Camera cameraEditor = Scene.Mode.Value == Scene.ModeEnum._3D ? Scene.CameraEditor : Scene.CameraEditor2D;

				//!!!!slowly
				foreach( var obj in Scene.GetComponents( checkChildren: true, depthFirstSearch: true ) )
				{
					var display = obj.ScreenLabel.Value;
					if( display != ScreenLabelEnum.NeverDisplay )
					{
						var info = obj.GetScreenLabelInfo();

						//display current camera always in the corner
						if( cameraEditor != null && obj == cameraEditor )
							info.DisplayInCorner = true;

						if( display == ScreenLabelEnum.AlwaysDisplay )
							info.DisplayInCorner = true;

						if( info.DisplayInCorner )
							list.Add( obj );
					}
				}
			}

			//double pos = 1.0 - screenSize.X * 0.25;

			for( int n = list.Count - 1; n >= 0; n-- )
			{
				var obj = list[ n ];

				//var rect = new Rectangle( pos - screenSize.X, screenSize.Y * 0.25, pos, screenSize.Y * 1.25 ).ToRectangleF();

				//pos -= screenSize.X * 1.25;

				ColorValue color;
				if( context2.selectedObjects.Contains( obj ) )
					color = ProjectSettings.Get.Colors.SelectedColor;
				else if( context2.canSelectObjects.Contains( obj ) )
					color = ProjectSettings.Get.Colors.CanSelectColor;
				else
					color = ProjectSettings.Get.SceneEditor.ScreenLabelColor;

				var item = new Viewport.LastFrameScreenLabelItem();
				item.Object = obj;
				item.DistanceToCamera = -1;
				//item.ScreenRectangle = rect;
				item.Color = color;
				if( !obj.EnabledInHierarchy )
					item.Color.Alpha *= 0.5f;

				item.AlwaysVisible = true;
				viewport.LastFrameScreenLabels.AddLast( item );
				viewport.LastFrameScreenLabelByObjectInSpace[ obj ] = item;

				//screenPositionY = item.ScreenRectangle.Bottom;
			}

			if( list.Count != 0 )
				screenPositionY = screenSize.Y * 1.25;
		}

		private void Scene_RenderEvent( Scene sender, Viewport viewport )
		{
			double screenPositionY = 0;

			//screen labels
			if( Scene.GetDisplayDevelopmentDataInThisApplication() && Scene.DisplayLabels && viewport.AllowRenderScreenLabels && viewport.CanvasRenderer != null )
				AddScreenLabels( viewport, ref screenPositionY );

			foreach( var widget in CanvasWidgets )
				widget.OnUpdate( this, ref screenPositionY );
		}

		protected override void Viewport_UpdateGetObjectInSceneRenderingContext( Viewport viewport, ref ObjectInSpace.RenderingContext context )
		{
			base.Viewport_UpdateGetObjectInSceneRenderingContext( viewport, ref context );

			//prepare context

			context = new ObjectInSpace.RenderingContext( viewport );

			if( !createByDropEntered && !createByClickEntered && !createByBrushEntered && ObjectCreationMode == null )
			{
				//hightlight selected objects
				if( DisplaySelectedObjects )
				{
					foreach( var obj in SelectedObjectsSet )
						context.selectedObjects.AddWithCheckAlreadyContained( obj );
				}

				//select by rectangle
				if( AllowSelectObjects )
				{
					foreach( var obj in SelectByRectangle_GetObjectsInSpace() )
					{
						if( !context.selectedObjects.Contains( obj ) )
							context.canSelectObjects.AddWithCheckAlreadyContained( obj );
					}
				}

				//!!!!screen labels еще 
				//!!!!уменьшить Boundng box для источников. еще чего

				//select by click
				{
					bool skip = false;
					if( selectByRectangle_Activated )
						skip = true;
					if( transformTool != null && transformTool.IsMouseOverAxisToActivation() )
						skip = true;
					if( CameraRotating )
						skip = true;
					if( createByDropEntered || createByClickEntered || createByBrushEntered )
						skip = true;
					if( !AllowSelectObjects )
						skip = true;

					//!!!!выключить когда меню. когда еще

					if( !skip )
					{
						var obj = GetMouseOverObjectToSelectByClick();
						if( obj != null )
						{
							if( !context.selectedObjects.Contains( obj ) )
								context.canSelectObjects.AddWithCheckAlreadyContained( obj );
						}
					}
				}
			}
			else
			{
				//drag drop entered

				if( createSetPropertyVisualizeCanSelectObject != null )
					context.canSelectObjects.Add( createSetPropertyVisualizeCanSelectObject );

				if( ObjectCreationMode != null )
					context.objectToCreate = ObjectCreationMode.CreatingObject;
				else
					context.objectToCreate = objectToCreate;
			}

			//drop LensFlares to Light
			if( createByDropEntered && objectToCreate != null && objectToCreate is LensFlares )
			{
				var overObject = GetMouseOverObjectToSelectByClick() as Component;

				if( overObject != null && overObject is Light )
					context.canSelectObjects.AddWithCheckAlreadyContained( overObject );
			}

		}

		public delegate void Viewport_UpdateBeforeOutputEventDelegate( SceneEditor sender, Viewport viewport );
		public static event Viewport_UpdateBeforeOutputEventDelegate Viewport_UpdateBeforeOutputEvent;

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			var renderer = viewport.CanvasRenderer;

			//render UI controls
			viewport.UIContainer.PerformRenderUI( renderer );

			//draw selection rectangle
			if( selectByRectangle_Enabled && selectByRectangle_Activated && AllowSelectObjects )
			{
				Rectangle rect = new Rectangle( selectByRectangle_StartPosition );
				rect.Add( viewport.MousePosition );

				Vector2I windowSize = viewport.SizeInPixels;
				Vector2 thickness = new Vector2( 1.0f / (float)windowSize.X, 1.0f / (float)windowSize.Y );

				renderer.AddRectangle( rect + thickness, new ColorValue( 0, 0, 0, .5f ) );
				renderer.AddRectangle( rect, new ColorValue( 0, 1, 0, 1 ) );

				////draw amount of objects
				//{
				//   int count = 0;
				//   Map.Instance.GetObjectsByScreenRectangle( rect, delegate( MapObject obj )
				//   {
				//      if( obj.EditorSelectable && obj.Visible && obj.EditorLayer.AllowSelect &&
				//         !MapEditorEngineApp.Instance.IsNeedHideMapObjectForMapEditorRequirements( obj ) )
				//      {
				//         count++;
				//      }
				//   } );
				//   if( count != 0 )
				//   {
				//      AddTextWithShadow( renderer, count.ToString(), rect.LeftBottom, HorizontalAlign.Left, VerticalAlign.Top,
				//         new ColorValue( 0, 1, 0 ) );
				//   }
				//}
			}

			//Gizmo.Instance.IsMouseOverAxisToActivation()

			if( !selectByRectangle_Activated )
				transformTool.PerformRender();

			CreateByBrushRender( viewport );

			Viewport_UpdateBeforeOutputEvent?.Invoke( this, viewport );
		}

		protected override void Viewport_UpdateBeforeOutput2( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput2( viewport );

			if( !selectByRectangle_Activated )
				transformTool.PerformOnRenderUI();
		}

		public Rectangle SelectByRectangle_GetRectangle()
		{
			Rectangle rect = new Rectangle( selectByRectangle_StartPosition );
			rect.Add( selectByRectangle_LastMousePosition );
			return rect;
		}

		ESet<object> SelectByRectangle_GetObjectsInSpace()
		{
			var result = new ESet<object>();
			if( selectByRectangle_Activated )
			{
				foreach( var obj in GetObjectsToSelectByRectangle( SelectByRectangle_GetRectangle() ) )//, true ) )
					result.Add( obj );
			}
			return result;
		}

		public List<object> GetObjectsInSpaceToSelectByRectangle( Rectangle rectangle )//, bool skipChildrenOfResultObjects )
		{
			var allSet = new ESet<Component>();

			//get by screen label
			foreach( var item in ViewportControl2.Viewport.LastFrameScreenLabels.GetReverse() )
			{
				var obj = item.Object;
				var objectInSpace = item.Object as ObjectInSpace;
				if( objectInSpace != null && objectInSpace.EnabledSelectionByCursor || objectInSpace == null || item.AlwaysVisible )
				{
					if( rectangle.Contains( item.ScreenRectangle.GetCenter() ) )
						allSet.AddWithCheckAlreadyContained( obj );
				}
			}

			foreach( var c in Scene.GetComponents( false, true, true ) )
			{
				var obj = c as ObjectInSpace;
				if( obj != null && obj.EnabledSelectionByCursor )//obj.EnabledInHierarchy && obj.VisibleInHierarchy && obj.CanBeSelected )
				{
					var transform = obj.Transform.Value;
					var pos = transform.Position;

					if( ViewportControl2.Viewport.CameraSettings.ProjectToScreenCoordinates( pos, out Vector2 screenPosition ) )
					{
						if( rectangle.Contains( screenPosition ) )
							allSet.AddWithCheckAlreadyContained( obj );
					}
				}
			}

			//if( skipChildrenOfResultObjects )
			//{
			//ESet<ObjectInSpace> allSet = new ESet<ObjectInSpace>( all );

			var result = new List<object>();
			foreach( var obj in allSet )
			{
				bool skip = false;
				foreach( var parent in obj.GetAllParents( true ) )
				{
					if( !IsObjectSelected( parent ) )
					{
						var parent2 = parent as ObjectInSpace;
						if( parent2 != null && allSet.Contains( parent2 ) )
						{
							skip = true;
							break;
						}
					}
				}

				if( !skip )
					result.Add( obj );
			}

			return result;

			//}
			//else
			//	return all;
		}

		protected virtual List<object> GetObjectsToSelectByRectangle( Rectangle rectangle )
		{
			var result = new List<object>();
			if( WorkareaMode != null && WorkareaMode.PerformGetObjectsToSelectByRectangle( rectangle, ref result ) )
				return result;
			return GetObjectsInSpaceToSelectByRectangle( rectangle );
		}

		TransformToolObject GetTransformToolObjectForObject( object controlledObject )
		{
			foreach( var toolObject in transformTool.Objects )
				if( ReferenceEquals( toolObject.ControlledObject, controlledObject ) )
					return toolObject;
			return null;
		}

		void UpdateTransformToolObjects()
		{
			var currentObjects = new ESet<object>();
			foreach( var toolObject in transformTool.Objects )
				currentObjects.AddWithCheckAlreadyContained( toolObject.ControlledObject );

			//remove old
			foreach( var currentObject in currentObjects )
			{
				bool deleted = false;

				//!!!!object in space specific
				//!!!!так? какие еще?
				var objectInSpace = currentObject as ObjectInSpace;
				if( objectInSpace != null && ( objectInSpace.Parent == null || objectInSpace.Disposed ) )
					deleted = true;

				//remove. is not selected or deleted
				if( !SelectedObjectsSet.Contains( currentObject ) || deleted )
				{
					var obj = GetTransformToolObjectForObject( currentObject );
					if( obj != null )
						transformTool.Objects.Remove( obj );
				}
			}

			//add new
			foreach( var obj in SelectedObjectsSet )
			{
				var toolObject = GetTransformToolObjectForObject( obj );
				if( toolObject == null )
				{
					toolObject = TransformToolCreateObject( obj );
					if( toolObject != null )
						transformTool.Objects.Add( toolObject );
				}
			}
		}

		protected virtual void TransformToolModifyBegin( TransformToolClass sender )
		{
			if( WorkareaMode != null && WorkareaMode.PerformTransformToolModifyBegin() )
				return;
		}

		protected virtual void TransformToolModifyCommit( TransformToolClass sender )
		{
			if( WorkareaMode != null && WorkareaMode.PerformTransformToolModifyCommit() )
				return;

			if( !transformToolModifyCloned )
			{
				//add changes to undo list

				var undoItems = new List<UndoActionPropertiesChange.Item>();

				foreach( var toolObject in transformTool.Objects )
				{
					var toolObject2 = toolObject as TransformToolObjectObjectInSpace;
					if( toolObject2 != null )
					{
						var obj = toolObject2.ObjectToTransform;
						if( obj != null )
						{
							var p = obj.MetadataGetMemberBySignature( "property:Transform" ) as Metadata.Property;
							if( p != null )
							{
								var undoItem = new UndoActionPropertiesChange.Item( obj, p, toolObject2.BeforeModifyTransform, null );
								undoItems.Add( undoItem );
							}
						}
					}
				}

				//undo
				if( undoItems.Count != 0 )
				{
					var action = new UndoActionPropertiesChange( undoItems.ToArray() );
					Document2.UndoSystem.CommitAction( action );
					Document2.Modified = true;
				}
			}
			else
			{
				var newObjects = new List<Component>();
				foreach( var toolObject in transformTool.Objects )
				{
					var toolObject2 = toolObject as TransformToolObjectObjectInSpace;
					if( toolObject2 != null )
						newObjects.Add( toolObject2.SelectedObject );
				}

				//add to undo with deletion
				var action = new UndoActionComponentCreateDelete( Document2, newObjects, true );
				Document2.UndoSystem.CommitAction( action );
				Document2.Modified = true;

				//update selected objects to update Settings Window
				SelectObjects( SelectedObjects, forceUpdate: true );
			}

			transformToolModifyCloned = false;
		}

		protected virtual void TransformToolModifyCancel( TransformToolClass sender )
		{
			if( WorkareaMode != null && WorkareaMode.PerformTransformToolModifyCancel() )
				return;

			if( transformToolModifyCloned )
			{
				ContentBrowserUtility.AllContentBrowsers_SuspendChildrenChangedEvent();

				//delete objects

				var toDelete = new List<Component>();
				foreach( var toolObject in transformTool.Objects )
				{
					var toolObject2 = toolObject as TransformToolObjectObjectInSpace;
					if( toolObject2 != null )
						toDelete.Add( toolObject2.SelectedObject );
				}

				//clear selected objects
				SelectObjects( null );

				//delete
				foreach( var obj in toDelete )
					obj.RemoveFromParent( true );

				Scene.HierarchyController?.ProcessDelayedOperations();
				ContentBrowserUtility.AllContentBrowsers_ResumeChildrenChangedEvent();
			}

			transformToolModifyCloned = false;
		}

		protected virtual void TransformToolCloneAndSelectObjects()
		{
			if( WorkareaMode != null && WorkareaMode.PerformTransformToolCloneAndSelectObjects() )
				return;

			ContentBrowserUtility.AllContentBrowsers_SuspendChildrenChangedEvent();

			var objectsToClone = new List<Component>();
			foreach( var toolObject in transformTool.Objects )
			{
				var toolObject2 = toolObject as TransformToolObjectObjectInSpace;
				if( toolObject2 != null )
					objectsToClone.Add( toolObject2.SelectedObject );
			}

			//remove children which inside selected parents
			objectsToClone = ComponentUtility.GetComponentsWithoutChildren( objectsToClone );

			var newObjects = new List<Component>();
			foreach( var obj in objectsToClone )
			{
				var newObject = EditorUtility.CloneComponent( obj );
				newObjects.Add( newObject );
				AddClonedSelectableChildrenToList( newObjects, newObject );
			}

			Scene.HierarchyController?.ProcessDelayedOperations();
			ContentBrowserUtility.AllContentBrowsers_ResumeChildrenChangedEvent();

			//select objects
			SelectObjects( newObjects.Cast<object>().ToArray(), updateSettingsWindowSelectObjects: false );

			//add screen message
			EditorUtility.ShowScreenNotificationObjectsCloned( newObjects.Count );

			transformToolModifyCloned = true;

			UpdateTransformToolObjects();
			transformTool.PerformUpdateInitialObjectsTransform();
		}

		public override void EditorActionGetState( EditorActionGetStateContext context )
		{
			base.EditorActionGetState( context );

			switch( context.Action.Name )
			{

			case "Select":
				context.Enabled = true;
				context.Checked = transformTool != null && transformTool.Mode == TransformToolMode.None;
				break;

			case "Move & Rotate":
				context.Enabled = true;
				context.Checked = transformTool != null && transformTool.Mode == TransformToolMode.PositionRotation;
				break;

			case "Move":
				context.Enabled = true;
				context.Checked = transformTool != null && transformTool.Mode == TransformToolMode.Position;
				break;

			case "Rotate":
				context.Enabled = true;
				context.Checked = transformTool != null && transformTool.Mode == TransformToolMode.Rotation;
				break;

			case "Scale":
				context.Enabled = true;
				context.Checked = transformTool != null && transformTool.Mode == TransformToolMode.Scale;
				break;

			case "Transform Using Local Coordinates":
				context.Enabled = true;
				context.Checked = transformTool != null && transformTool.CoordinateSystemMode == TransformToolCoordinateSystemMode.Local;
				break;

			//!!!!New object?

			case "Snap All Axes":
			case "Snap X":
			case "Snap Y":
			case "Snap Z":
				if( CanSnap( out _ ) )
					context.Enabled = true;
				break;

			case "Focus Camera On Selected Object":
				if( CanFocusCameraOnSelectedObject( out _ ) )
					context.Enabled = true;
				break;

			case "Create Objects By Drag & Drop":
				context.Enabled = true;
				context.Checked = CreateObjectsMode == CreateObjectsModeEnum.Drop;
				break;

			case "Create Objects By Click":
				context.Enabled = true;
				context.Checked = CreateObjectsMode == CreateObjectsModeEnum.Click;
				break;

			case "Create Objects By Brush":
				context.Enabled = true;
				context.Checked = CreateObjectsMode == CreateObjectsModeEnum.Brush;
				break;

			case "Create Objects Brush Radius":
			case "Create Objects Brush Strength":
			case "Create Objects Brush Hardness":
				context.Enabled = CreateObjectsMode == CreateObjectsModeEnum.Brush;
				break;

			case "Create Objects Destination":
				{
					if( createObjectsDestinationCachedList.Count != 0 )
					{
						//if( CreateObjectsMode == CreateObjectsModeEnum.Brush )
						context.Enabled = true;

						//update items
						var items = new List<(string, Image)>();
						foreach( var item in createObjectsDestinationCachedList )
							items.Add( (item.Text, null) );
						( (EditorAction)context.Action ).ListBox.Items = items;

						//update selected item
						var selectIndex = createObjectsDestinationCachedList.FindIndex( a => a.Mode == createObjectsDestinationSelected.Mode && a.Obj == createObjectsDestinationSelected.Obj );
						if( selectIndex == -1 )
							selectIndex = 0;
						( (EditorAction)context.Action ).ListBox.SelectIndex = selectIndex;
					}
				}
				break;

			case "Terrain Geometry Raise":
			case "Terrain Geometry Lower":
			case "Terrain Geometry Smooth":
			case "Terrain Geometry Flatten":
				context.Enabled = true;
				context.Checked = WorkareaModeName == context.Action.Name;
				break;

			case "Terrain Shape Circle":
				context.Enabled = true;
				context.Checked = TerrainToolShape == TerrainEditingMode.TerrainEditorToolShape.Circle;
				break;

			case "Terrain Shape Square":
				context.Enabled = true;
				context.Checked = TerrainToolShape == TerrainEditingMode.TerrainEditorToolShape.Square;
				break;

			case "Terrain Tool Radius":
			case "Terrain Tool Strength":
			case "Terrain Tool Hardness":
				context.Enabled = true;
				break;

			case "Terrain Paint Paint":
			case "Terrain Paint Clear":
			case "Terrain Paint Smooth":
			case "Terrain Paint Flatten":
				context.Enabled = true;
				context.Checked = WorkareaModeName == context.Action.Name;
				break;

			case "Terrain Paint Layers":
				{
					if( terrainPaintLayersCachedList.Count != 0 )
						context.Enabled = true;

					//update items
					var items = new List<(string, Image)>();
					foreach( var item in terrainPaintLayersCachedList )
						items.Add( (item.Text, PreviewImagesManager.GetImageForPaintLayer( item.Obj )) );
					( (EditorAction)context.Action ).ListBox.Items = items;

					//update selected item
					var selectIndex = terrainPaintLayersCachedList.FindIndex( a => a.Obj == terrainPaintLayersSelected );
					if( selectIndex == -1 )
						selectIndex = 0;
					( (EditorAction)context.Action ).ListBox.SelectIndex = selectIndex;
				}
				break;

			case "Terrain Paint Add Layer":
				context.Enabled = true;
				break;
			}
		}

		public override void EditorActionClick( EditorActionClickContext context )
		{
			base.EditorActionClick( context );

			switch( context.Action.Name )
			{
			case "Select":
				if( transformTool != null )
				{
					if( transformTool.Mode == TransformToolMode.Undefined )
						WorkareaModeSet( "" );
					transformTool.Mode = TransformToolMode.None;
				}
				break;

			case "Move & Rotate":
				if( transformTool != null )
				{
					if( transformTool.Mode == TransformToolMode.Undefined )
						WorkareaModeSet( "" );
					transformTool.Mode = TransformToolMode.PositionRotation;
				}
				break;

			case "Move":
				if( transformTool != null )
				{
					if( transformTool.Mode == TransformToolMode.Undefined )
						WorkareaModeSet( "" );
					transformTool.Mode = TransformToolMode.Position;
				}
				break;

			case "Rotate":
				if( transformTool != null )
				{
					if( transformTool.Mode == TransformToolMode.Undefined )
						WorkareaModeSet( "" );
					transformTool.Mode = TransformToolMode.Rotation;
				}
				break;

			case "Scale":
				if( transformTool != null )
				{
					if( transformTool.Mode == TransformToolMode.Undefined )
						WorkareaModeSet( "" );
					transformTool.Mode = TransformToolMode.Scale;
				}
				break;

			case "Transform Using Local Coordinates":
				if( transformTool != null )
					transformTool.CoordinateSystemMode = transformTool.CoordinateSystemMode == TransformToolCoordinateSystemMode.Local ? TransformToolCoordinateSystemMode.World : TransformToolCoordinateSystemMode.Local;
				break;

			case "Snap All Axes":
			case "Snap X":
			case "Snap Y":
			case "Snap Z":
				Snap( (EditorAction)context.Action );
				break;

			case "Focus Camera On Selected Object":
				if( CanFocusCameraOnSelectedObject( out var objects ) )
					FocusCameraOnSelectedObject( objects );
				break;

			case "Create Objects By Drag & Drop":
				ChangeCreateObjectsMode( CreateObjectsModeEnum.Drop );
				break;

			case "Create Objects By Click":
				ChangeCreateObjectsMode( CreateObjectsModeEnum.Click );
				break;

			case "Create Objects By Brush":
				ChangeCreateObjectsMode( CreateObjectsModeEnum.Brush );
				break;

			case "Create Objects Brush Radius":
				CreateObjectsBrushRadius = ( (EditorAction)context.Action ).Slider.Value;
				break;

			case "Create Objects Brush Strength":
				CreateObjectsBrushStrength = ( (EditorAction)context.Action ).Slider.Value;
				break;

			case "Create Objects Brush Hardness":
				CreateObjectsBrushHardness = ( (EditorAction)context.Action ).Slider.Value;
				break;

			case "Create Objects Destination":
				if( ( (EditorAction)context.Action ).ListBox.LastSelectedIndexChangedByUser )
				{
					var newIndex = ( (EditorAction)context.Action ).ListBox.SelectedIndex;
					if( newIndex >= 0 && newIndex < createObjectsDestinationCachedList.Count )
					{
						var item = createObjectsDestinationCachedList[ newIndex ];
						createObjectsDestinationSelected = (item.Mode, item.Obj);
					}
					else
						createObjectsDestinationSelected = (CreateObjectsDestinationModeEnum.Auto, null);
				}
				break;

			case "Terrain Geometry Raise":
			case "Terrain Geometry Lower":
			case "Terrain Geometry Smooth":
			case "Terrain Geometry Flatten":
				if( WorkareaModeName != context.Action.Name )
				{
					//reset create objects mode
					ChangeCreateObjectsMode( CreateObjectsModeEnum.Drop );

					var modeStr = context.Action.Name.Substring( "Terrain".Length ).Replace( " ", "" );
					var mode = (TerrainEditingMode.TerrainEditorMode)Enum.Parse( typeof( TerrainEditingMode.TerrainEditorMode ), modeStr );

					WorkareaModeSet( context.Action.Name, new TerrainEditingMode( this, mode ) );
					//var editingModeType = MetadataManager.GetType( "NeoAxis.Editor.TerrainEditingMode" );
					//if( editingModeType != null )
					//	WorkareaModeSet( context.Action.Name, (WorkareaModeClass)editingModeType.InvokeInstance( new object[] { this, mode, null } ) );

					if( transformTool.Mode != TransformToolMode.Undefined )
						transformToolModeRestore = transformTool.Mode;
					transformTool.Mode = TransformToolMode.Undefined;
				}
				else
					ResetWorkareaMode();
				break;

			case "Terrain Shape Circle":
				TerrainToolShape = TerrainEditingMode.TerrainEditorToolShape.Circle;
				break;

			case "Terrain Shape Square":
				TerrainToolShape = TerrainEditingMode.TerrainEditorToolShape.Square;
				break;

			case "Terrain Tool Radius":
				TerrainToolRadius = ( (EditorAction)context.Action ).Slider.Value;
				break;

			case "Terrain Tool Strength":
				TerrainToolStrength = ( (EditorAction)context.Action ).Slider.Value;
				break;

			case "Terrain Tool Hardness":
				TerrainToolHardness = ( (EditorAction)context.Action ).Slider.Value;
				break;

			case "Terrain Paint Paint":
			case "Terrain Paint Clear":
			case "Terrain Paint Smooth":
			case "Terrain Paint Flatten":
				if( WorkareaModeName != context.Action.Name )
				{
					//reset create objects mode
					ChangeCreateObjectsMode( CreateObjectsModeEnum.Drop );

					var modeStr = context.Action.Name.Substring( "Terrain".Length ).Replace( " ", "" );
					var mode = (TerrainEditingMode.TerrainEditorMode)Enum.Parse( typeof( TerrainEditingMode.TerrainEditorMode ), modeStr );

					WorkareaModeSet( context.Action.Name, new TerrainEditingMode( this, mode ) );
					//var editingModeType = MetadataManager.GetType( "NeoAxis.Editor.TerrainEditingMode" );
					//if( editingModeType != null )
					//	WorkareaModeSet( context.Action.Name, (WorkareaModeClass)editingModeType.InvokeInstance( new object[] { this, mode, null } ) );

					if( transformTool.Mode != TransformToolMode.Undefined )
						transformToolModeRestore = transformTool.Mode;
					transformTool.Mode = TransformToolMode.Undefined;
				}
				else
					ResetWorkareaMode();
				break;

			case "Terrain Paint Layers":
				//after selection changed. better after click
				if( ( (EditorAction)context.Action ).ListBox.LastSelectedIndexChangedByUser )
				{
					var newIndex = ( (EditorAction)context.Action ).ListBox.SelectedIndex;
					if( newIndex >= 0 && newIndex < terrainPaintLayersCachedList.Count )
					{
						var layer = terrainPaintLayersCachedList[ newIndex ].Obj;
						SelectObjects( new object[] { layer } );
					}

					//var newIndex = context.Action.ListBox.SelectedIndex;
					//if( newIndex >= 0 && newIndex < terrainPaintLayersCachedList.Count )
					//	terrainPaintLayersSelected = terrainPaintLayersCachedList[ newIndex ].Obj;
					//else
					//	terrainPaintLayersSelected = null;
				}
				break;

			case "Terrain Paint Add Layer":
				{
					var rootComponent = EditorForm.Instance.WorkspaceController.SelectedDocumentWindow?.ObjectOfWindow as Scene;

					var terrain = (Component)rootComponent?.GetComponent<Terrain>( true );

					//var terrains = SelectedObjects.OfType<Terrain>().ToArray();
					//if( terrains.Length != 0 )
					if( terrain != null )
					{
						var newObjects = new List<Component>();

						//foreach( var terrain in terrains )
						{
							var layer = terrain.CreateComponent<PaintLayer>( enabled: false );
							layer.Name = EditorUtility.GetUniqueFriendlyName( layer );
							layer.Enabled = true;

							newObjects.Add( layer );
						}

						Focus();

						//undo
						var action = new UndoActionComponentCreateDelete( Document2, newObjects, true );
						Document2.CommitUndoAction( action );
						SelectObjects( newObjects.ToArray() );
					}
				}
				break;
			}
		}

		public override void EditorActionClick2( EditorActionClickContext context )
		{
			base.EditorActionClick2( context );

			switch( context.Action.Name )
			{
			case "Terrain Paint Layers":
				//if( context.Action.ListBox.LastSelectedIndexChangedByUser )
				{
					var newIndex = ( (EditorAction)context.Action ).ListBox.SelectedIndex;
					if( newIndex >= 0 && newIndex < terrainPaintLayersCachedList.Count )
					{
						var layer = terrainPaintLayersCachedList[ newIndex ].Obj;
						SelectObjects( new object[] { layer } );
						ShowContextMenu();
					}
				}
				break;
			}
		}

		//!!!!connect
		private void renderTargetUserControl1_KeyUp( object sender, KeyEventArgs e )
		{
			//!!!!
			//if( e.KeyCode == Keys.Apps )
			//   ShowContextMenu( new Point( 0, 0 ) );
		}

		string TranslateContextMenu( string text )
		{
			return EditorContextMenuWinForms.Translate( text );
		}

		void ShowContextMenu()
		{
			var items = new List<KryptonContextMenuItemBase>();

			Component oneSelectedComponent = null;
			{
				if( SelectedObjects.Length == 1 )
					oneSelectedComponent = SelectedObjects[ 0 ] as Component;
			}

			//Transform Tool
			if( transformTool != null )
			{
				EditorContextMenuWinForms.AddTransformToolToMenu( items, transformTool );
				items.Add( new KryptonContextMenuSeparator() );
			}

			Vector2 mouse = new Vector2( .5, .5 );
			if( ViewportControl2 != null && ViewportControl2.Viewport != null )
				mouse = ViewportControl2.Viewport.MousePosition;

			//Editor
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Editor" ), EditorResourcesCache.Edit, delegate ( object s, EventArgs e2 )
				 {
					 EditorAPI2.OpenDocumentWindowForObject( Document2, oneSelectedComponent );
				 } );
				item.Enabled = oneSelectedComponent != null && EditorAPI2.IsDocumentObjectSupport( oneSelectedComponent );
				items.Add( item );
			}

			//Settings
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Settings" ), EditorResourcesCache.Settings, delegate ( object s, EventArgs e2 )
				{
					EditorAPI2.SelectDockWindow( EditorAPI2.FindWindow<SettingsWindow>() );
				} );
				items.Add( item );
			}

			//Separate Settings
			if( EditorUtility.AllowSeparateSettings )
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Separate Settings" ), EditorResourcesCache.Settings, delegate ( object s, EventArgs e2 )
				{
					var obj = oneSelectedComponent ?? ObjectOfWindow;
					bool canUseAlreadyOpened = !ModifierKeys.HasFlag( Keys.Shift );
					EditorAPI2.ShowObjectSettingsWindow( Document2, obj, canUseAlreadyOpened );
				} );
				item.Enabled = oneSelectedComponent != null || SelectedObjects.Length == 0;
				items.Add( item );
			}

			items.Add( new KryptonContextMenuSeparator() );

			//New object
			{
				EditorContextMenuWinForms.AddNewObjectItem( items, CanNewObject( out _ ), delegate ( Metadata.TypeInfo type, bool assetsFolderOnly )
				{
					TryNewObject( mouse, type );
				} );

				//newObjectItem.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
				//items.Add( newObjectItem );

				//KryptonContextMenuItem item = new KryptonContextMenuItem( Translate( "New object" ), Properties.Resources.New_16, delegate ( object s, EventArgs e2 )
				//{
				//	TryNewObject( mouse, null );
				//} );
				//item.Enabled = CanNewObject( out _ );
				//items.Add( item );
			}

			//separator
			items.Add( new KryptonContextMenuSeparator() );

			//Cut
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Cut" ), EditorResourcesCache.Cut, delegate ( object s, EventArgs e2 )
				{
					EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, "Cut" );
				} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Cut" );
				item.Enabled = EditorAPI2.EditorActionGetState( EditorActionHolder.ContextMenu, "Cut" ).Enabled;
				items.Add( item );
			}

			//Copy
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Copy" ), EditorResourcesCache.Copy, delegate ( object s, EventArgs e2 )
				{
					EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, "Copy" );
				} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Copy" );
				item.Enabled = EditorAPI2.EditorActionGetState( EditorActionHolder.ContextMenu, "Copy" ).Enabled;
				items.Add( item );
			}

			//Paste
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Paste" ), EditorResourcesCache.Paste, delegate ( object s, EventArgs e2 )
				{
					EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, "Paste" );
				} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Paste" );
				item.Enabled = EditorAPI2.EditorActionGetState( EditorActionHolder.ContextMenu, "Paste" ).Enabled;
				items.Add( item );
			}

			//Clone
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Duplicate" ), EditorResourcesCache.Clone, delegate ( object s, EventArgs e2 )
				{
					EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, "Duplicate" );
				} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Duplicate" );
				item.Enabled = EditorAPI2.EditorActionGetState( EditorActionHolder.ContextMenu, "Duplicate" ).Enabled;
				items.Add( item );
			}

			//Export to File
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Export to File" ), null, delegate ( object s, EventArgs e2 )
				{
					EditorUtility2.ExportComponentToFile( oneSelectedComponent, IntPtr.Zero );
				} );
				item.Enabled = oneSelectedComponent != null;
				items.Add( item );
			}

			//separator
			items.Add( new KryptonContextMenuSeparator() );

			//Delete
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Delete" ), EditorResourcesCache.Delete, delegate ( object s, EventArgs e2 )
				{
					EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, "Delete" );
				} );
				item.Enabled = EditorAPI2.EditorActionGetState( EditorActionHolder.ContextMenu, "Delete" ).Enabled;
				items.Add( item );
			}

			//Rename
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Rename" ), null, delegate ( object s, EventArgs e2 )
				{
					EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, "Rename" );
				} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rename" );
				item.Enabled = EditorAPI2.EditorActionGetState( EditorActionHolder.ContextMenu, "Rename" ).Enabled;
				items.Add( item );
			}

			EditorContextMenuWinForms.AddActionsToMenu( EditorActionContextMenuType.Document, items );

			EditorContextMenuWinForms.Show( items, this );
		}

		public bool CanNewObject( out List<Component> parentsForNewObjects )
		{
			parentsForNewObjects = new List<Component>();

			foreach( var obj in SelectedObjects )
			{
				var component = obj as Component;
				if( component != null )
					parentsForNewObjects.Add( component );
			}

			//can create without selected objects
			if( parentsForNewObjects.Count == 0 )
				parentsForNewObjects.Add( Scene );
			return true;
		}

		public void TryNewObject( Vector2 mouse, Metadata.TypeInfo lockType )
		{
			if( !CanNewObject( out var parentsForNewObjects ) )
				return;

			//var viewport = ViewportControl.Viewport;
			//var ray = viewport.CameraSettings.GetRayByScreenCoordinates( mouse );

			//Vector3 pos = ray.Origin + ray.Direction.GetNormalize() * 20;

			var data = new NewObjectWindow.CreationDataClass();
			data.initDocumentWindow = this;
			data.initParentObjects = new List<object>();
			data.initParentObjects.AddRange( parentsForNewObjects );

			//type specific
			data.beforeCreateObjectsFunction = delegate ( NewObjectWindow window, Metadata.TypeInfo selectedType )
			{
				if( window.creationData.initParentObjects.Count == 1 && window.creationData.initParentObjects[ 0 ] is Scene )
				{
					Component createTo = GetObjectCreationSettings().destinationObject as Layer;
					if( createTo == null )
						createTo = Scene;

					//MeshGeometry_Procedural
					if( MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( selectedType ) )
					{
						var meshInSpace = createTo.CreateComponent<MeshInSpace>( -1, false );

						//Name
						meshInSpace.Name = EditorUtility.GetUniqueFriendlyName( meshInSpace, selectedType.GetUserFriendlyNameForInstance() );
						//var defaultName = meshInSpace.BaseType.GetUserFriendlyNameForInstance();
						//if( meshInSpace.Parent.GetComponent( defaultName ) == null )
						//	meshInSpace.Name = defaultName;
						//else
						//	meshInSpace.Name = meshInSpace.Parent.Components.GetUniqueName( defaultName, false, 2 );

						window.creationData.createdObjects = new List<object>();
						window.creationData.createdObjects.Add( meshInSpace );
						window.creationData.createdComponentsOnTopLevel.Add( meshInSpace );

						var mesh = meshInSpace.CreateComponent<Mesh>();
						window.creationData.createdObjects.Add( mesh );
						mesh.Name = "Mesh";

						var geometry = mesh.CreateComponent( selectedType );
						window.creationData.createdObjects.Add( geometry );
						geometry.Name = "Mesh Geometry";

						meshInSpace.Mesh = ReferenceUtility.MakeThisReference( meshInSpace, mesh );

						return true;
					}

					//CollisionShape
					if( MetadataManager.GetTypeOfNetType( typeof( CollisionShape ) ).IsAssignableFrom( selectedType )/* &&
						selectedType != MetadataManager.GetTypeOfNetType( typeof( CollisionShape_Mesh ) )*/ )
					{
						var rigidBody = createTo.CreateComponent<RigidBody>( -1, false );

						//Name
						var defaultName = rigidBody.BaseType.GetUserFriendlyNameForInstance();
						if( rigidBody.Parent.GetComponent( defaultName ) == null )
							rigidBody.Name = defaultName;
						else
							rigidBody.Name = rigidBody.Parent.Components.GetUniqueName( defaultName, false, 2 );

						window.creationData.createdObjects = new List<object>();
						window.creationData.createdObjects.Add( rigidBody );
						window.creationData.createdComponentsOnTopLevel.Add( rigidBody );

						var shape = rigidBody.CreateComponent( selectedType );
						window.creationData.createdObjects.Add( shape );
						shape.Name = "Collision Shape";

						return true;
					}

					//RenderingEffect
					if( MetadataManager.GetTypeOfNetType( typeof( RenderingEffect ) ).IsAssignableFrom( selectedType ) )
					{
						var pipeline = Scene.RenderingPipeline.Value;
						if( pipeline != null )//&& pipeline.GetComponent( selectedType, true ) == null )
						{
							var group = pipeline.GetComponent( "Scene Effects" );
							if( group != null )
							{
								var insertIndex = EditorUtility.GetNewObjectInsertIndex( group, selectedType );
								var effect = group.CreateComponent( selectedType, insertIndex, false );

								//Name
								var defaultName = effect.BaseType.GetUserFriendlyNameForInstance();
								if( group.GetComponent( defaultName ) == null )
									effect.Name = defaultName;
								else
									effect.Name = group.Components.GetUniqueName( defaultName, false, 2 );

								window.creationData.createdObjects = new List<object>();
								window.creationData.createdObjects.Add( effect );
								window.creationData.createdComponentsOnTopLevel.Add( effect );

								return true;
							}
						}
					}
				}

				return true;
			};

			//set Transform
			data.additionActionAfterEnabled = delegate ( NewObjectWindow window )
			//data.additionActionBeforeEnabled = delegate ( NewObjectWindow window )
			{
				foreach( var obj in data.createdComponentsOnTopLevel )
				{
					if( obj is ObjectInSpace objectInSpace )
					{
						var objectToTransform = ObjectInSpaceUtility.CalculateObjectToTransform( objectInSpace );
						if( objectToTransform == null )
							objectToTransform = objectInSpace;

						//!!!!так? выставляется при создании в NewObjectSettings
						if( objectToTransform.Transform.Value.Position == Vector3.Zero )
						{
							CalculateCreateObjectPosition( objectInSpace, objectToTransform, mouse );
							//objectInSpace.Transform = new Transform( pos, Quaternion.Identity );
						}
					}
				}
			};

			data.initLockType = lockType;
			data.initSupportAutoCreateAndClose = lockType != null;

			EditorAPI2.OpenNewObjectWindow( data );
		}

		public override bool TryDeleteObjects()
		{
			if( !base.TryDeleteObjects() )
				return false;

			UpdateTransformToolObjects();

			return true;
		}

		private void Scene_DocumentWindow_DragEnter( object sender, DragEventArgs e )
		{
			if( /*CreateObjectsMode == CreateObjectsModeEnum.Drop &&*/ ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByDrop ) )
			{
				createByDropEntered = true;
				DragDropCreateObject_Create( e );
			}
		}

		private void Scene_DocumentWindow_DragOver( object sender, DragEventArgs e )
		{
			e.Effect = DragDropEffects.None;

			if( /*CreateObjectsMode == CreateObjectsModeEnum.Drop &&*/ ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByDrop ) && createByDropEntered )
			{
				ViewportControl2?.PerformMouseMove();

				//для IMeshInSpaceChild потому как он создает при наведении
				//if( objectToCreate == null )
				//	DragDropCreateObject_Create( e );

				CreateObject_Update();
				if( objectToCreate != null )
					e.Effect = DragDropEffects.Link;

				CreateSetProperty_Update( e );
				if( createSetProperty != null )
					e.Effect = DragDropEffects.Link;

				ViewportControl2.TryRender();
			}
		}

		private void Scene_DocumentWindow_DragLeave( object sender, EventArgs e )
		{
			if( /*CreateObjectsMode == CreateObjectsModeEnum.Drop &&*/ createByDropEntered )
			{
				CreateObject_Destroy();
				CreateSetProperty_Cancel();
				createByDropEntered = false;

				//!!!!не всегда работает
				ViewportControl2.TryRender();
			}
		}

		private void Scene_DocumentWindow_DragDrop( object sender, DragEventArgs e )
		{
			if( /*CreateObjectsMode == CreateObjectsModeEnum.Drop &&*/ createByDropEntered )
			{
				//creation mode
				if( objectToCreate != null )
				{
					var attributes = objectTypeToCreate.GetNetType()/* objectToCreate.GetType()*/.GetCustomAttributes<ObjectCreationModeAttribute>().ToArray();
					if( attributes.Length != 0 )
					{
						//begin creation mode
						var type2 = GetCreationModeType( attributes[ 0 ] );
						var mode = (ObjectCreationMode)type2.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { this, objectToCreate } );
						ObjectCreationModeSet( mode );
						objectTypeToCreate = null;
						objectToCreate = null;
						createByDropEntered = false;
						EditorAPI2.SelectDockWindow( this );
						return;
					}
				}

				if( CreateSetProperty_Commit() )
					CreateObject_Destroy();
				else
					CreateObject_Commit();
				createByDropEntered = false;
			}
		}

		//public delegate void CreateObjectWhatTypeWillCreatedEventDelegate( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type );
		//public static event CreateObjectWhatTypeWillCreatedEventDelegate CreateObjectWhatTypeWillCreatedEvent;

		Metadata.TypeInfo CreateObjectWhatTypeWillCreated( Metadata.TypeInfo objectType, string referenceToObject )
		{
			//add-ons support
			{
				Metadata.TypeInfo type = null;
				SceneEditorUtility.PerformCreateObjectWhatTypeWillCreatedEvent( objectType, referenceToObject, ref type );
				//CreateObjectWhatTypeWillCreatedEvent?.Invoke( objectType, referenceToObject, ref type );
				if( type != null )
					return type;
			}

			//MeshGeometry_Procedural
			if( MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
				return MetadataManager.GetTypeOfNetType( typeof( MeshInSpace ) );

			//Mesh
			if( MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( objectType ) )
				return MetadataManager.GetTypeOfNetType( typeof( MeshInSpace ) );

			//ParticleSystem
			if( MetadataManager.GetTypeOfNetType( typeof( ParticleSystem ) ).IsAssignableFrom( objectType ) )
				return MetadataManager.GetTypeOfNetType( typeof( ParticleSystemInSpace ) );

			//CollisionShape
			if( MetadataManager.GetTypeOfNetType( typeof( CollisionShape ) ).IsAssignableFrom( objectType ) /*&&
				objectType != MetadataManager.GetTypeOfNetType( typeof( CollisionShape_Mesh ) )*/ )
				return MetadataManager.GetTypeOfNetType( typeof( RigidBody ) );

			//CollisionShape2D
			if( MetadataManager.GetTypeOfNetType( typeof( CollisionShape2D ) ).IsAssignableFrom( objectType ) )
				return MetadataManager.GetTypeOfNetType( typeof( RigidBody2D ) );

			//Import3D
			if( typeof( Import3D ).IsAssignableFrom( objectType.GetNetType() ) )
			{
				var componentType = objectType as Metadata.ComponentTypeInfo;
				if( componentType != null && componentType.BasedOnObject != null )
				{
					//!!!!если один вложенный ObjectInSpace, то можно корневой не создавать. но тогда сбрасывается трансформ у вложенного.

					var mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Mesh;
					if( mesh != null )
						return MetadataManager.GetTypeOfNetType( typeof( MeshInSpace ) );
					else
					{
						//Scene mode
						var sceneObjects = componentType.BasedOnObject.GetComponent( "Scene Objects" ) as ObjectInSpace;
						if( sceneObjects != null && sceneObjects.GetProvidedType() != null )
							return sceneObjects.GetProvidedType();
					}
				}
			}

			//RenderingEffect
			if( MetadataManager.GetTypeOfNetType( typeof( RenderingEffect ) ).IsAssignableFrom( objectType ) )
			{
				var pipeline = Scene.RenderingPipeline.Value;
				if( pipeline != null )//&& pipeline.GetComponent( objectType, true ) == null )
				{
					var group = pipeline.GetComponent( "Scene Effects" );
					if( group != null )
						return objectType;
				}
			}

			//Sound
			if( MetadataManager.GetTypeOfNetType( typeof( Sound ) ).IsAssignableFrom( objectType ) )
				return MetadataManager.GetTypeOfNetType( typeof( SoundSource ) );

			//Component by default
			if( MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( objectType ) )
				return objectType;

			return null;
		}

		//public delegate void CreateObjectByCreationDataEventDelegate( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject );
		//public static event CreateObjectByCreationDataEventDelegate CreateObjectByCreationDataEvent;

		Component CreateObjectByCreationData( Metadata.TypeInfo objectType, string referenceToObject, object anyData, string objectName )
		{
			Component createTo = GetObjectCreationSettings().destinationObject as Layer;
			if( createTo == null )
				createTo = Scene;

			//var overObject = GetMouseOverObjectForSelection();

			Component newObject = null;

			//!!!!hardcoded? может это как-то унифицировать?

			////ICanDropToScene
			////ObjectInSpace, _Sky, _Fog
			//if( newObject == null && typeof( ICanDropToScene ).IsAssignableFrom( objectType.GetNetType() ) )
			//{
			//	var objectInSpace = Scene.CreateComponent( objectType, -1, false );
			//	newObject = objectInSpace;
			//}
			////ObjectInSpace
			//if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( objectType ) )
			//{
			//	var objectInSpace = Scene.CreateComponent( objectType, -1, false );
			//	newObject = objectInSpace;
			//}

			//add-ons support
			SceneEditorUtility.PerformCreateObjectByCreationDataEvent( objectType, referenceToObject, anyData, createTo, ref newObject );
			//CreateObjectByCreationDataEvent?.Invoke( objectType, referenceToObject, anyData, createTo, ref newObject );

			//MeshGeometry_Procedural
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
			{
				var meshInSpace = createTo.CreateComponent<MeshInSpace>( -1, false );
				newObject = meshInSpace;
				newObject.Name = EditorUtility.GetUniqueFriendlyName( newObject, objectType.GetUserFriendlyNameForInstance() );

				var mesh = meshInSpace.CreateComponent<Mesh>();
				mesh.Name = "Mesh";
				var geometry = mesh.CreateComponent( objectType );
				geometry.Name = "Mesh Geometry";
				//geometry.Name = geometry.BaseType.GetUserFriendlyNameForInstance().Replace( '_', ' ' );
				meshInSpace.Mesh = new Reference<Mesh>( null, ReferenceUtility.CalculateThisReference( meshInSpace, mesh ) );
			}

			//Mesh
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( objectType ) )
			{
				var meshInSpace = createTo.CreateComponent<MeshInSpace>( -1, false );
				newObject = meshInSpace;

				//!!!!ссылка такая для всех?
				meshInSpace.Mesh = new Reference<Mesh>( null, referenceToObject );
			}

			//ParticleSystem
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( ParticleSystem ) ).IsAssignableFrom( objectType ) )
			{
				var particleInSpace = createTo.CreateComponent<ParticleSystemInSpace>( -1, false );
				newObject = particleInSpace;

				if( MetadataManager.GetTypeOfNetType( typeof( ParticleSystem ) ) == objectType && string.IsNullOrEmpty( referenceToObject ) )
				{
					var particleSystem = particleInSpace.CreateComponent<ParticleSystem>();
					particleSystem.Name = "Particle System";
					particleSystem.NewObjectSetDefaultConfiguration();
					particleInSpace.ParticleSystem = ReferenceUtility.MakeThisReference( particleInSpace, particleSystem );
				}
				else
					particleInSpace.ParticleSystem = new Reference<ParticleSystem>( null, referenceToObject );
			}

			//CollisionShape
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( CollisionShape ) ).IsAssignableFrom( objectType )/* &&
				objectType != MetadataManager.GetTypeOfNetType( typeof( CollisionShape_Mesh ) )*/ )
			{
				var rigidBody = createTo.CreateComponent<RigidBody>( -1, false );
				newObject = rigidBody;

				var shape = rigidBody.CreateComponent( objectType );
				shape.Name = "Collision Shape";
			}

			//CollisionShape2D
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( CollisionShape2D ) ).IsAssignableFrom( objectType ) )
			{
				var rigidBody = createTo.CreateComponent<RigidBody2D>( -1, false );
				newObject = rigidBody;

				var shape = rigidBody.CreateComponent( objectType );
				shape.Name = "Collision Shape";
			}

			//Import3D
			if( newObject == null && typeof( Import3D ).IsAssignableFrom( objectType.GetNetType() ) )
			{
				var componentType = objectType as Metadata.ComponentTypeInfo;
				if( componentType != null && componentType.BasedOnObject != null )
				{
					//!!!!если один вложенный ObjectInSpace, то можно корневой не создавать. но тогда сбрасывается трансформ у вложенного.

					var mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Mesh;
					if( mesh != null )
					{
						//OneMesh mode
						var meshInSpace = createTo.CreateComponent<MeshInSpace>( -1, false );
						newObject = meshInSpace;
						newObject.Name = EditorUtility.GetUniqueFriendlyName( newObject, objectType.GetUserFriendlyNameForInstance() );
						meshInSpace.Mesh = new Reference<Mesh>( null, ReferenceUtility.CalculateResourceReference( mesh ) );
					}
					else
					{
						//Scene mode
						var sceneObjects = componentType.BasedOnObject.GetComponent( "Scene Objects" ) as ObjectInSpace;
						if( sceneObjects != null && sceneObjects.GetProvidedType() != null )
						{
							var objectInSpace = createTo.CreateComponent( sceneObjects.GetProvidedType(), -1, false );
							newObject = objectInSpace;
						}
					}

					//var sceneObjects = componentType.BasedOnObject.GetComponentByName( "Scene Objects" ) as ObjectInSpace;
					//if( sceneObjects != null )
					//{
					//	Component objectToCreate = null;

					//	if( sceneObjects.Components.Count == 1 && sceneObjects.Transform.Value.IsIdentity )
					//	{
					//		var components = sceneObjects.GetComponents();
					//		if( components.Length == 1 )
					//			objectToCreate = components[ 0 ];
					//	}
					//	else
					//		objectToCreate = sceneObjects;

					//	if( objectToCreate != null && objectToCreate.GetProvidedType() != null )
					//	{
					//		var objectInSpace = Scene.CreateComponent( objectToCreate.GetProvidedType(), -1, false );
					//		newObject = objectInSpace;
					//	}
					//}
				}
			}

			//RenderingEffect
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( RenderingEffect ) ).IsAssignableFrom( objectType ) )
			{
				var pipeline = Scene.RenderingPipeline.Value;
				if( pipeline != null )//&& pipeline.GetComponent( objectType, true ) == null )
				{
					var group = pipeline.GetComponent( "Scene Effects" );
					if( group != null )
					{
						var insertIndex = EditorUtility.GetNewObjectInsertIndex( group, objectType );
						var effect = group.CreateComponent( objectType, insertIndex, false );
						newObject = effect;
					}
				}
			}

			////!!!!удаляется в _Update. может можно всё делать в _Update
			////MeshInSpace.IMeshInSpaceChild as child of MeshInSpace
			//if( newObject == null && typeof( MeshInSpace.IMeshInSpaceChild ).IsAssignableFrom( objectType.GetNetType() ) )
			//{
			//	var overMeshInSpace = overObject as MeshInSpace;
			//	if( overMeshInSpace != null )
			//	{
			//		var obj = overMeshInSpace.CreateComponent( objectType, -1, false );
			//		newObject = obj;
			//	}
			//}

			//Sound
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( Sound ) ).IsAssignableFrom( objectType ) )
			{
				var soundSource = createTo.CreateComponent<SoundSource>( -1, false );
				newObject = soundSource;

				//!!!!ссылка такая для всех?
				soundSource.Sound = new Reference<Sound>( null, referenceToObject );
			}

			//Component by default
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( objectType ) )
			{
				var objectInSpace = createTo.CreateComponent( objectType, -1, false );
				newObject = objectInSpace;
			}

			if( newObject != null )
			{
				//set name
				if( string.IsNullOrEmpty( newObject.Name ) )
				{
					if( !string.IsNullOrEmpty( objectName ) && newObject.Parent != null )
					{
						if( newObject.Parent.GetComponent( objectName ) == null )
							newObject.Name = objectName;
						else
							newObject.Name = newObject.Parent.Components.GetUniqueName( objectName, true, 2 );
					}
					else
						newObject.Name = ComponentUtility.GetNewObjectUniqueName( newObject );
				}

				//set default configuration
				newObject.NewObjectSetDefaultConfiguration();

				//finish object creation
				newObject.Enabled = true;
			}

			return newObject;
		}

		bool CreateObjectFilterEqual( Metadata.TypeInfo objectType, string referenceToObject, ObjectInSpace obj )
		{
			//MeshGeometry_Procedural
			if( MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
			{
				var meshInSpace = obj as MeshInSpace;
				if( meshInSpace != null )
				{
					var mesh = meshInSpace.GetComponent( "Mesh" ) as Mesh;
					if( mesh != null )
					{
						var meshGeometry = mesh.GetComponent( objectType );
						if( meshGeometry != null && meshGeometry.Name == "Mesh Geometry" )
							return true;
					}
				}
			}

			//Mesh
			if( MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( objectType ) )
			{
				var meshInSpace = obj as MeshInSpace;
				if( meshInSpace != null && meshInSpace.Mesh.GetByReference == referenceToObject )
					return true;
			}

			//ParticleSystem
			if( MetadataManager.GetTypeOfNetType( typeof( ParticleSystem ) ).IsAssignableFrom( objectType ) )
			{
				var particleInSpace = obj as ParticleSystemInSpace;
				if( particleInSpace != null && particleInSpace.ParticleSystem.GetByReference == referenceToObject )
					return true;
			}

			//CollisionShape
			if( MetadataManager.GetTypeOfNetType( typeof( CollisionShape ) ).IsAssignableFrom( objectType ) /* &&
				objectType != MetadataManager.GetTypeOfNetType( typeof( CollisionShape_Mesh ) )*/ )
			{
				var rigidBody = obj as RigidBody;
				if( rigidBody != null )
				{
					var shape = rigidBody.GetComponent( objectType );
					if( shape != null && shape.Name == "Collision Shape" )
						return true;
				}
			}

			//!!!!CollisionShape2D

			//Import3D
			if( typeof( Import3D ).IsAssignableFrom( objectType.GetNetType() ) )
			{
				var meshInSpace = obj as MeshInSpace;
				if( meshInSpace != null )
				{
					var componentType = objectType as Metadata.ComponentTypeInfo;
					if( componentType != null && componentType.BasedOnObject != null )
					{
						var mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Mesh;
						if( mesh != null )
						{
							if( meshInSpace.Mesh.GetByReference == ReferenceUtility.CalculateResourceReference( mesh ) )
								return true;
						}
						else
						{
							//!!!!

							////Scene mode
							//var sceneObjects = componentType.BasedOnObject.GetComponent( "Scene Objects" ) as ObjectInSpace;
							//if( sceneObjects != null && sceneObjects.GetProvidedType() != null )
							//{
							//	var objectInSpace = Scene.CreateComponent( sceneObjects.GetProvidedType(), -1, false );
							//	newObject = objectInSpace;
							//}
						}
					}
				}
			}

			//Sound
			if( MetadataManager.GetTypeOfNetType( typeof( Sound ) ).IsAssignableFrom( objectType ) )
			{
				var soundSource = obj as SoundSource;
				if( soundSource != null && soundSource.Sound.GetByReference == referenceToObject )
					return true;
			}

			//by class filtering
			if( string.IsNullOrEmpty( referenceToObject ) && objectType.IsAssignableFrom( obj.BaseType ) )
				return true;

			return false;
		}

		void DragDropCreateObject_Create( DragEventArgs e )
		{
			(var objectType, var referenceToObject, var anyData, var objectName) = EditorAPI2.GetObjectToCreateByDropData( e );
			if( objectType != null )//|| memberFullSignature != "" || createNodeWithComponent != null )
			{
				var newObject = CreateObjectByCreationData( objectType, referenceToObject, anyData, objectName );
				if( newObject != null )
				{
					objectTypeToCreate = objectType;
					objectToCreate = newObject;
					CreateObject_Update();
				}
			}
		}

		void CreateObject_Destroy()
		{
			if( objectToCreate != null )
			{
				objectToCreate.Dispose();
				objectTypeToCreate = null;
				objectToCreate = null;
			}
		}

		public (bool found, Vector3 position, Vector3F normal, ObjectInSpace collidedWith) CalculateCreateObjectPositionUnderCursor( Viewport viewport, ObjectInSpace objectInSpace = null, Vector2? overrideMouse = null, Ray? overrideRay = null, bool allowSnap = true )
		{
			Vector2 mouse;
			if( overrideMouse.HasValue )
				mouse = overrideMouse.Value;
			else
			{
				mouse = viewport.MousePosition;
				if( !new Rectangle( 0, 0, 1, 1 ).Contains( mouse ) )
					mouse = new Vector2( 0.5, 0.5 );
			}

			Ray ray;
			if( overrideRay.HasValue )
				ray = overrideRay.Value;
			else
				ray = viewport.CameraSettings.GetRayByScreenCoordinates( mouse );
			//!!!!? clamp max distance
			//ray.Direction = ray.Direction.GetNormalize() * 100;

			var allowSnap2 = allowSnap && ModifierKeys.HasFlag( Keys.Control );

			return SceneUtility.CalculateCreateObjectPositionByRay( Scene, objectInSpace, ray, allowSnap2 );
		}

		public void CalculateCreateObjectPosition( ObjectInSpace objectInSpace, ObjectInSpace objectToTransform, Vector2? mouse = null )
		{
			var result = CalculateCreateObjectPositionUnderCursor( Viewport, objectInSpace, mouse );
			objectToTransform.Transform = new Transform( result.position, objectToTransform.Transform.Value.Rotation, objectToTransform.Transform.Value.Scale );
		}

		void CreateObject_Update()
		{
			if( objectToCreate != null && objectToCreate is ObjectInSpace objectInSpace )
			{
				var objectToTransform = ObjectInSpaceUtility.CalculateObjectToTransform( objectInSpace );
				if( objectToTransform == null )
					objectToTransform = objectInSpace;

				CalculateCreateObjectPosition( objectInSpace, objectToTransform );

				//!!!!тут? только тут?
				objectInSpace.NewObjectSetDefaultConfigurationUpdate();

				////set Transform
				//data.additionActionBeforeEnabled = delegate ( NewObjectWindow window )
				//{
				//	foreach( var obj in data.createdComponentsOnTopLevel )
				//	{
				//		var objectInSpace = obj as ObjectInSpace;
				//		if( objectInSpace != null )
				//		{
				//			//!!!!не всегда так. если чилд другого ObjectInSpace, то ссылку настраивать?

				//			//!!!!так? выставляется при создании в NewObjectSettings
				//			if( objectInSpace.Transform.Value.Position == Vec3.Zero )
				//			{
				//				//!!!!temp
				//				objectInSpace.Transform = new Transform( pos, Quat.Identity );
				//			}
				//		}
				//	}
				//};
			}

			//drop LensFlares to Light
			if( objectToCreate != null && objectToCreate is LensFlares )
			{
				var overObject = GetMouseOverObjectToSelectByClick() as Component;

				Component needParent = Scene;
				if( overObject != null && overObject is Light )
					needParent = overObject;

				if( objectToCreate.Parent != needParent )
				{
					objectToCreate.RemoveFromParent( false );
					needParent.AddComponent( objectToCreate );
				}
			}

			////!!!!тут удаляется. создается в _Create. может можно в одном месте это всё
			////MeshInSpace.IMeshInSpaceChild as child of MeshInSpace
			//if( dragDropCreateObject != null && dragDropCreateObject is MeshInSpace.IMeshInSpaceChild meshInSpaceChild )
			//{
			//	var overObject = GetMouseOverObjectForSelection();
			//	if( overObject != ( (Component)meshInSpaceChild ).Parent )
			//		DragDropCreateObject_Destroy();
			//}
		}

		void CreateObject_Commit()
		{
			if( objectToCreate != null )
			{
				var obj = objectToCreate;

				//add to undo with deletion
				var newObjects = new List<Component>();
				newObjects.Add( objectToCreate );
				var action = new UndoActionComponentCreateDelete( Document2, newObjects, true );
				Document2.UndoSystem.CommitAction( action );
				Document2.Modified = true;

				objectTypeToCreate = null;
				objectToCreate = null;

				//select created object
				if( CreateObjectsMode != CreateObjectsModeEnum.Click )
					//if( CreateObjectsMode == CreateObjectsModeEnum.Drop )
					EditorAPI2.SelectComponentsInMainObjectsWindow( this, new Component[] { obj } );

				EditorAPI2.SelectDockWindow( this );
			}
		}

		void CreateSetProperty_Update( DragEventArgs dragEventArgs )
		{
			object setPropertyObject = null;
			//Component setPropertyObject = null;
			//ObjectInSpace setPropertyObject = null;
			Metadata.Property setProperty = null;
			object[] setPropertyIndexes = null;
			IReference setPropertyValue = null;
			Component setPropertyVisualizeCanSelectObject = null;

			ContentBrowser.Item contentBrowserItem = null;
			{
				if( /*CreateObjectsMode == CreateObjectsModeEnum.Drop &&*/ dragEventArgs != null )
				{
					var dragDropData = ContentBrowser.GetDroppingItemData( dragEventArgs.Data );
					if( dragDropData != null )
						contentBrowserItem = dragDropData.Item;
				}
				if( CreateObjectsMode == CreateObjectsModeEnum.Click )
					contentBrowserItem = EditorAPI2.CreateObjectGetSelectedContentBrowserItem();
			}

			//var dragDropData = ContentBrowser.GetDroppingItemData( dragEventArgs.Data );
			//if( dragDropData != null )
			if( contentBrowserItem != null )
			{
				//var contentBrowserItem = dragDropData.Item;

				//!!!!про новый WorkareaMode
				var overObject = GetMouseOverObjectToSelectByClick() as Component;
				//var overObject = GetMouseOverObjectForSelection();

				if( overObject != null )
				{
					Metadata.TypeInfo dragDropType = null;
					string referenceValue = "";
					char referenceValueAddSeparator = '\\';

					//_File
					var fileItem = contentBrowserItem as ContentBrowserItem_File;
					if( fileItem != null && !fileItem.IsDirectory )
					{
						//!!!!не делать предпросмотр для карты, т.к. долго. что еще?
						var ext = Path.GetExtension( fileItem.FullPath );
						if( ResourceManager.GetTypeByFileExtension( ext ) != null )
						{
							var res = ResourceManager.GetByName( VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath ) );

							var type = res?.PrimaryInstance?.ResultComponent?.GetProvidedType();
							if( type != null )
							{
								dragDropType = type;
								referenceValue = res.Name;
								referenceValueAddSeparator = '|';
							}
						}
					}

					//_Component
					var componentItem = contentBrowserItem as ContentBrowserItem_Component;
					if( componentItem != null )
					{
						var component = componentItem.Component;

						if( Scene.ParentRoot == component.ParentRoot )
						{
							dragDropType = MetadataManager.MetadataGetType( component );
							if( dragDropType != null )
							{
								ReferenceUtility.CalculateThisReference( overObject, component, "", out referenceValue, out referenceValueAddSeparator );
								//referenceValue = ReferenceUtility.CalculateThisReference( overObject, component );
							}
						}
						else
						{
							var resourceInstance = component.ParentRoot?.HierarchyController.CreatedByResource;
							if( resourceInstance != null )
							{
								dragDropType = component.GetProvidedType();
								if( dragDropType != null )
								{
									referenceValue = dragDropType.Name;
									referenceValueAddSeparator = '|';
								}
							}
						}

						//if( component.ParentRoot.HierarchyController != null &&
						//	component.ParentRoot.HierarchyController.CreatedByResource.InstanceType == Resource.InstanceType.Resource )
						//{
						//	dragDropType = component.GetProvidedType();
						//	if( dragDropType != null )
						//		referenceValue = dragDropType.Name;
						//}
						//else
						//{
						//	if( Scene.ParentRoot == component.ParentRoot )
						//	{
						//		dragDropType = MetadataManager.MetadataGetType( component );
						//		if( dragDropType != null )
						//			referenceValue = ReferenceUtils.CalculateThisReference( overObject, component );
						//	}
						//}
					}

					//_Member
					var memberItem = contentBrowserItem as ContentBrowserItem_Member;
					if( memberItem != null )
					{
						var member = memberItem.Member;

						//свойство еще неизвестно
						ContentBrowserUtility.CalculateReferenceValueForMemberItem( overObject, null, memberItem, out referenceValue, out referenceValueAddSeparator );

						if( member is Metadata.Property )
						{
							//property
							var property2 = (Metadata.Property)member;

							dragDropType = property2.TypeUnreferenced;
							//if( ReferenceUtils.CanMakeReferenceToObjectWithType( property, property2.TypeUnreferenced ) )
							//	canSet = true;
						}
						else if( member is Metadata.Method )
						{
							//method
							var method = (Metadata.Method)member;

							var returnParameters = method.GetReturnParameters();
							if( method.Parameters.Length == 1 && returnParameters.Length == 1 )
							{
								dragDropType = returnParameters[ 0 ].Type;
								//if( ReferenceUtils.CanMakeReferenceToObjectWithType( property, returnParameters[ 0 ].Type ) )
								//	canSet = true;
							}
						}
					}

					//_StoreItem
					var storeItem = contentBrowserItem as StoresWindow.ContentBrowserItem_StoreItem;
					if( storeItem != null )
					{
						var (type, reference) = storeItem.GetFileToDrop( CreateObjectsMode == CreateObjectsModeEnum.Drop );

						switch( type )
						{
						case PackageManager.PackageInfo.FileTypeToDrop.Material:
							dragDropType = MetadataManager.GetTypeOfNetType( typeof( Material ) );
							referenceValue = reference;
							referenceValueAddSeparator = '|';
							break;
						}
					}


					if( dragDropType != null )
					{
						//drop Material to
						if( MetadataManager.GetTypeOfNetType( typeof( Material ) ).IsAssignableFrom( dragDropType ) )
						{
							//drop Material to MeshInSpace.ReplaceMaterial
							var overMeshInSpace = overObject as MeshInSpace;
							if( overMeshInSpace != null )
							{
								setPropertyObject = overMeshInSpace;
								setProperty = (Metadata.Property)overMeshInSpace.MetadataGetMemberBySignature( "property:" + nameof( MeshInSpace.ReplaceMaterial ) );
								setPropertyValue = ReferenceUtility.MakeReference( typeof( Material ), null, referenceValue );

								setPropertyVisualizeCanSelectObject = null;
							}

							//drop Material to Billboard.Material
							var overBillboard = overObject as Billboard;
							if( overBillboard != null )
							{
								setPropertyObject = overBillboard;
								setProperty = (Metadata.Property)overBillboard.MetadataGetMemberBySignature( "property:" + nameof( Billboard.Material ) );
								setPropertyValue = ReferenceUtility.MakeReference( typeof( Material ), null, referenceValue );

								setPropertyVisualizeCanSelectObject = null;
							}

							//drop Material to Decal.Material
							var overDecal = overObject as Decal;
							if( overDecal != null )
							{
								setPropertyObject = overDecal;
								setProperty = (Metadata.Property)overDecal.MetadataGetMemberBySignature( "property:" + nameof( Decal.Material ) );
								setPropertyValue = ReferenceUtility.MakeReference( typeof( Material ), null, referenceValue );

								setPropertyVisualizeCanSelectObject = null;
							}
						}

						//drop Material from Import3D to
						if( typeof( Import3D ).IsAssignableFrom( dragDropType.GetNetType() ) )
						{
							//check Material component exists
							bool materialExists = false;
							{
								var componentType = dragDropType as Metadata.ComponentTypeInfo;
								if( componentType != null && componentType.BasedOnObject != null )
								{
									var material = componentType.BasedOnObject.GetComponent( "Material" ) as Material;
									if( material != null )
										materialExists = true;
								}

							}

							if( materialExists )
							{
								var referenceValue2 = referenceValue + referenceValueAddSeparator.ToString() + "$Material";

								//drop Material to MeshInSpace.ReplaceMaterial
								var overMeshInSpace = overObject as MeshInSpace;
								if( overMeshInSpace != null )
								{
									setPropertyObject = overMeshInSpace;
									setProperty = (Metadata.Property)overMeshInSpace.MetadataGetMemberBySignature( "property:" + nameof( MeshInSpace.ReplaceMaterial ) );
									setPropertyValue = ReferenceUtility.MakeReference( typeof( Material ), null, referenceValue2 );

									setPropertyVisualizeCanSelectObject = null;
								}

								//drop Material to Billboard.Material
								var overBillboard = overObject as Billboard;
								if( overBillboard != null )
								{
									setPropertyObject = overBillboard;
									setProperty = (Metadata.Property)overBillboard.MetadataGetMemberBySignature( "property:" + nameof( Billboard.Material ) );
									setPropertyValue = ReferenceUtility.MakeReference( typeof( Material ), null, referenceValue2 );

									setPropertyVisualizeCanSelectObject = null;
								}

								//drop Material to Decal.Material
								var overDecal = overObject as Decal;
								if( overDecal != null )
								{
									setPropertyObject = overDecal;
									setProperty = (Metadata.Property)overDecal.MetadataGetMemberBySignature( "property:" + nameof( Decal.Material ) );
									setPropertyValue = ReferenceUtility.MakeReference( typeof( Material ), null, referenceValue2 );

									setPropertyVisualizeCanSelectObject = null;
								}
							}
						}
					}
				}
			}

			if( dragEventArgs != null )
			{
				var dragDropDataSetReferenceData = (DragDropSetReferenceData)dragEventArgs.Data.GetData( typeof( DragDropSetReferenceData ) );
				if( dragDropDataSetReferenceData != null )
				{
					//!!!!не обязательно этот же документ
					if( dragDropDataSetReferenceData.document == Document2 )
					{
						var propertyTypeUnref = dragDropDataSetReferenceData.property.TypeUnreferenced;

						if( MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( propertyTypeUnref ) )
						//if( MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( propertyTypeUnref ) )
						{
							//!!!!про новый WorkareaMode
							var overObject = GetMouseOverObjectToSelectByClick() as Component;
							//var overObject = GetMouseOverObjectForSelection();

							if( overObject != null && propertyTypeUnref.IsAssignableFrom( MetadataManager.MetadataGetType( overObject ) ) )
							{
								//!!!!multiselection

								var referenceValue = ReferenceUtility.CalculateThisReference( dragDropDataSetReferenceData.controlledComponents[ 0 ], overObject );

								setPropertyObject = dragDropDataSetReferenceData.propertyOwners[ 0 ];
								setProperty = dragDropDataSetReferenceData.property;
								setPropertyIndexes = dragDropDataSetReferenceData.indexers;
								setPropertyValue = ReferenceUtility.MakeReference( propertyTypeUnref.GetNetType(), null, referenceValue );

								setPropertyVisualizeCanSelectObject = overObject;
							}
						}

						//if( MetadataManager.GetTypeOfNetType( typeof( RigidBody ) ).IsAssignableFrom( propertyTypeUnref ) )
						//{
						//	multiselection

						//	var overRigidBody = GetMouseOverObjectForSelection() as RigidBody;
						//	if( overRigidBody != null )
						//	{
						//		var referenceValue = ReferenceUtils.CalculateThisReference(
						//			dragDropDataSetReferenceData.controlledObjects[ 0 ], overRigidBody );

						//		setPropertyObject = dragDropDataSetReferenceData.controlledObjects[ 0 ];
						//		setProperty = dragDropDataSetReferenceData.property;
						//		setPropertyValue = ReferenceUtils.CreateReference( propertyTypeUnref.GetNetType(), null, referenceValue );

						//		setPropertyVisualizeCanSelectObject = overRigidBody;
						//	}
						//}
					}
				}
			}

			//change
			if( createSetPropertyObject != setPropertyObject ||
				createSetProperty != setProperty ||
				createSetPropertyVisualizeCanSelectObject != setPropertyVisualizeCanSelectObject )
			{
				CreateSetProperty_Cancel();

				if( setPropertyObject != null )
				{
					try
					{
						createSetPropertyObject = setPropertyObject;
						createSetProperty = setProperty;
						createSetPropertyIndexes = setPropertyIndexes;
						createSetPropertyOldValue = (IReference)createSetProperty.GetValue( createSetPropertyObject, createSetPropertyIndexes );

						createSetProperty.SetValue( createSetPropertyObject, setPropertyValue, setPropertyIndexes );

						createSetPropertyVisualizeCanSelectObject = setPropertyVisualizeCanSelectObject;

					}
					catch( Exception e )
					{
						createSetPropertyObject = null;
						createSetProperty = null;
						createSetPropertyIndexes = null;
						createSetPropertyOldValue = null;
						createSetPropertyVisualizeCanSelectObject = null;

						Log.Warning( e.Message );
					}
				}
			}
		}

		void CreateSetProperty_Cancel()
		{
			if( createSetPropertyObject != null )
			{
				try
				{
					createSetProperty.SetValue( createSetPropertyObject, createSetPropertyOldValue, createSetPropertyIndexes );
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
				}

				createSetPropertyObject = null;
				createSetProperty = null;
				createSetPropertyIndexes = null;
				createSetPropertyOldValue = null;
				createSetPropertyVisualizeCanSelectObject = null;
			}
		}

		bool CreateSetProperty_Commit()
		{
			if( createSetPropertyObject != null )
			{
				try
				{
					var undoItems = new List<UndoActionPropertiesChange.Item>();

					var obj = createSetPropertyObject;
					var oldValue = createSetPropertyOldValue;

					var value = createSetProperty.GetValue( obj, createSetPropertyIndexes );
					if( !value.Equals( oldValue ) )
						undoItems.Add( new UndoActionPropertiesChange.Item( obj, createSetProperty, oldValue, createSetPropertyIndexes ) );

					if( undoItems.Count != 0 )
					{
						var action = new UndoActionPropertiesChange( undoItems.ToArray() );
						Document2.UndoSystem.CommitAction( action );
						Document2.Modified = true;
					}
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
				}

				createSetPropertyObject = null;
				createSetProperty = null;
				createSetPropertyIndexes = null;
				createSetPropertyOldValue = null;
				createSetPropertyVisualizeCanSelectObject = null;

				EditorAPI2.SelectDockWindow( this );

				return true;
			}

			return false;
		}

		void UpdateTransformToolActiveState()
		{
			transformTool.Active = !createByDropEntered && !createByClickEntered && !createByBrushEntered && ObjectCreationMode == null;
		}

		public bool CanSnap( out List<ObjectInSpace> resultObjects )
		{
			resultObjects = new List<ObjectInSpace>();
			foreach( var toolObject in transformTool.Objects )
			{
				var toolObject2 = toolObject as TransformToolObjectObjectInSpace;
				if( toolObject2 != null )
				{
					var objectToTransform = ObjectInSpaceUtility.CalculateObjectToTransform( toolObject2.SelectedObject );
					if( objectToTransform != null )
						resultObjects.Add( objectToTransform );
				}
			}
			return resultObjects.Count != 0;
		}

		public void Snap( EditorAction action )
		{
			if( !CanSnap( out var objects ) )
				return;

			var property = (Metadata.Property)MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).MetadataGetMemberBySignature( "property:Transform" );
			var snapValue = ProjectSettings.Get.SceneEditor.SceneEditorStepMovement.Value;

			var undoItems = new List<UndoActionPropertiesChange.Item>();
			foreach( var obj in objects )
			{
				var oldValue = obj.Transform;

				var position = obj.Transform.Value.Position;
				if( snapValue != 0 )
				{
					if( action.Name == "Snap All Axes" || action.Name == "Snap X" )
						position.X = ( (long)( position.X / snapValue + ( position.X > 0 ? 0.5 : -0.5 ) ) ) * snapValue;
					if( action.Name == "Snap All Axes" || action.Name == "Snap Y" )
						position.Y = ( (long)( position.Y / snapValue + ( position.Y > 0 ? 0.5 : -0.5 ) ) ) * snapValue;
					if( action.Name == "Snap All Axes" || action.Name == "Snap Z" )
						position.Z = ( (long)( position.Z / snapValue + ( position.Z > 0 ? 0.5 : -0.5 ) ) ) * snapValue;
				}
				obj.Transform = obj.Transform.Value.UpdatePosition( position );

				undoItems.Add( new UndoActionPropertiesChange.Item( obj, property, oldValue, null ) );
			}

			if( undoItems.Count != 0 )
			{
				var undoAction = new UndoActionPropertiesChange( undoItems.ToArray() );
				Document2.UndoSystem.CommitAction( undoAction );
				Document2.Modified = true;
			}
		}

		static string Translate( string text )
		{
			return EditorLocalization2.Translate( "SceneDocumentWindow", text );
		}

		protected override void GetTextInfoLeftTopCorner( List<string> lines )
		{
			base.GetTextInfoLeftTopCorner( lines );

			//statistics
			if( Scene != null && Scene.GetDisplayDevelopmentDataInThisApplication() && Scene.DisplayTextInfo )
			{
				var statistics = ViewportControl2?.Viewport?.RenderingContext?.UpdateStatisticsPrevious;
				if( statistics != null )
				{
					lines.Add( Translate( "FPS" ) + ": " + statistics.FPS.ToString( "F1" ) );
					lines.Add( Translate( "Triangles" ) + ": " + statistics.Triangles.ToString( "N0" ) );
					lines.Add( Translate( "Lines" ) + ": " + statistics.Lines.ToString( "N0" ) );
					lines.Add( Translate( "Compute dispatches" ) + ": " + statistics.ComputeDispatches.ToString( "N0" ) );
					lines.Add( Translate( "Draw calls total" ) + ": " + statistics.DrawCalls.ToString( "N0" ) );
					lines.Add( Translate( "Draw calls shadows" ) + ": " + statistics.DrawCallsShadows.ToString( "N0" ) );
					lines.Add( Translate( "Draw calls deferred" ) + ": " + statistics.DrawCallsDeferred.ToString( "N0" ) );
					lines.Add( Translate( "Draw calls forward" ) + ": " + statistics.DrawCallsForward.ToString( "N0" ) );
					lines.Add( Translate( "Instances" ) + ": " + statistics.Instances.ToString( "N0" ) );
					//lines.Add( Translate( "Compute dispatches" ) + ": " + statistics.ComputeDispatches.ToString( "N0" ) );
					lines.Add( Translate( "Render targets" ) + ": " + statistics.RenderTargets.ToString( "N0" ) );
					lines.Add( Translate( "Dynamic textures" ) + ": " + statistics.DynamicTextures.ToString( "N0" ) );
					lines.Add( Translate( "Compute write images" ) + ": " + statistics.ComputeWriteImages.ToString( "N0" ) );
					lines.Add( Translate( "Lights" ) + ": " + statistics.Lights.ToString( "N0" ) );
					lines.Add( Translate( "Reflection probes" ) + ": " + statistics.ReflectionProbes.ToString( "N0" ) );
					lines.Add( Translate( "Occlusion culling buffers" ) + ": " + statistics.OcclusionCullingBuffers.ToString( "N0" ) );
				}
			}
		}

		static Type GetTypeWhereDefinedWhenCreatingShowWarningIfItAlreadyExistsAttribute( Component obj )
		{
			var type = obj.GetType();
			if( type.GetCustomAttribute( typeof( WhenCreatingShowWarningIfItAlreadyExistsAttribute ), true ) != null )
			{
				var types = new Stack<Type>();
				{
					var t = type;
					while( t != null )
					{
						types.Push( t );
						t = t.BaseType;
					}
				}

				while( types.Count != 0 )
				{
					var t = types.Pop();
					if( t.GetCustomAttribute( typeof( WhenCreatingShowWarningIfItAlreadyExistsAttribute ), true ) != null )
						return t;
				}
			}

			return null;
		}

		protected override void GetTextInfoCenterBottomCorner( List<string> lines )
		{
			base.GetTextInfoCenterBottomCorner( lines );

			if( createByClickEntered )
				lines.Add( Translate( "Creating objects by mouse clicking." ) );
			if( createByBrushEntered )
			{
				//!!!!если painting
				lines.Add( Translate( "Creating objects by brush." ) );
			}

			//warning component already exists
			if( createByDropEntered && objectToCreate != null )
			{
				try
				{
					var type = GetTypeWhereDefinedWhenCreatingShowWarningIfItAlreadyExistsAttribute( objectToCreate );
					if( type != null && Scene.GetComponents( type, checkChildren: true ).Length > 1 )
					{
						var name = TypeUtility.GetUserFriendlyNameForInstanceOfType( type );

						if( lines.Count != 0 )
							lines.Add( "" );
						lines.Add( $"Another {name} component already exists." );
					}
				}
				catch { }
			}

			AddScreenMessagesFromTerrain( lines );

			//if( ( CreateObjectsMode == CreateObjectsModeEnum.Click || CreateObjectsMode == CreateObjectsModeEnum.Brush ) && ( WorkareaMode == null || WorkareaMode.AllowDragAndDropOrCreateByClickObjects ) )
			//{
			//	(var objectType, var referenceToObject) = EditorAPI.GetSelectedObjectToCreate();
			//	if( objectType != null )
			//	{
			//		string text;
			//		if( CreateObjectsMode == CreateObjectsModeEnum.Click )
			//			text = "Creating an object by click";
			//		else
			//			text = "Creating an object by brush";
			//		lines.Add( text );
			//	}
			//}
		}

		//bool CanPaste( out Component destinationParent )
		//{
		//	if( ClipboardManager.CheckAvailableInClipboard<ObjectCutCopyPasteData>() )
		//	{
		//		if( SelectedObjects.Length == 0 )
		//		{
		//			destinationParent = Scene;
		//			return true;
		//		}
		//		else if( SelectedObjects.Length == 1 )
		//		{
		//			var c = SelectedObjects[ 0 ] as Component;
		//			if( c != null )
		//			{
		//				destinationParent = c;
		//				return true;
		//			}
		//		}
		//	}

		//	destinationParent = null;
		//	return false;
		//}

		public override bool Paste()
		{
			if( !CanPaste( out var destinationParent ) )
				return false;

			var data = ClipboardManager.GetFromClipboard<ObjectCutCopyPasteData>();
			if( data != null )
			{
				var components = new List<Component>();
				foreach( var obj in data.objects )
				{
					var c = obj as Component;
					if( c != null )
						components.Add( c );
				}

				//create new objects

				var newObjects = new List<Component>();
				Vector3 addToPosition = Vector3.Zero;

				for( int n = 0; n < components.Count; n++ )
				{
					var c = components[ n ];

					var cloned = c.Clone();
					if( destinationParent.GetComponent( c.Name ) == null )
						cloned.Name = c.Name;
					else
						cloned.Name = destinationParent.Components.GetUniqueName( c.Name, true, 2 );
					destinationParent.AddComponent( cloned );

					//Transform
					if( destinationParent == Scene )
					{
						var objectInSpace = cloned as ObjectInSpace;
						if( objectInSpace != null )
						{
							var objectToTransform = ObjectInSpaceUtility.CalculateObjectToTransform( objectInSpace );
							if( objectToTransform == null )
								objectToTransform = objectInSpace;

							if( n == 0 )
							{
								CalculateCreateObjectPosition( objectInSpace, objectToTransform );
								addToPosition = objectToTransform.Transform.Value.Position - ( (ObjectInSpace)c ).Transform.Value.Position;
							}
							else
							{
								var old = objectInSpace.Transform.Value;
								objectToTransform.Transform = new Transform( old.Position + addToPosition, old.Rotation, old.Scale );
							}
						}
					}

					newObjects.Add( cloned );
				}

				if( data.cut )
				{
					//cut
					if( data.documentWindow.Document2 != Document2 )
					{
						//another document
						{
							var action = new UndoActionComponentCreateDelete( data.documentWindow.Document2, components, false );
							data.documentWindow.Document2.UndoSystem.CommitAction( action );
							data.documentWindow.Document2.Modified = true;
						}
						{
							var action = new UndoActionComponentCreateDelete( Document2, newObjects, true );
							Document2.UndoSystem.CommitAction( action );
							Document2.Modified = true;
						}
					}
					else
					{
						//same document
						var multiAction = new UndoMultiAction();
						multiAction.AddAction( new UndoActionComponentCreateDelete( Document2, components, false ) );
						multiAction.AddAction( new UndoActionComponentCreateDelete( Document2, newObjects, true ) );
						Document2.UndoSystem.CommitAction( multiAction );
						Document2.Modified = true;
					}
				}
				else
				{
					//copy
					var action = new UndoActionComponentCreateDelete( Document2, newObjects, true );
					Document2.UndoSystem.CommitAction( action );
					Document2.Modified = true;
				}
			}

			return true;
		}

		public bool CanFocusCameraOnSelectedObject( out object[] objects )
		{
			if( !AllowCameraControl )
			{
				objects = null;
				return false;
			}

			var result = new List<object>();

#if !DEPLOY
			foreach( var obj in SelectedObjects )
			{
				if( obj is ObjectInSpace || obj is BuilderWorkareaMode.Vertex || obj is BuilderWorkareaMode.Edge || obj is BuilderWorkareaMode.Face )
					result.Add( obj );
			}
#endif

			objects = result.ToArray();
			//objects = SelectedObjects.OfType<ObjectInSpace>().ToArray();
			return objects.Length != 0;
		}

		public void FocusCameraOnSelectedObject( object[] objects )
		{
#if !DEPLOY
			Bounds bounds = NeoAxis.Bounds.Cleared;
			foreach( var obj in objects )
			{
				if( obj is Light light )
				{
					var b = new Bounds( light.Transform.Value.Position );
					b.Expand( 0.1 );
					bounds.Add( b );
				}
				else if( obj is ObjectInSpace objectInSpace )
					bounds.Add( objectInSpace.SpaceBounds.BoundingBox );
				else if( obj is BuilderWorkareaMode.Vertex vertex )
				{
					var b = new Bounds( vertex.Position );
					b.Expand( 0.1 );
					bounds.Add( b );
				}
				else if( obj is BuilderWorkareaMode.Edge edge )
				{
					var b = new Bounds( edge.Position );
					b.Expand( 0.5 );
					bounds.Add( b );
				}
				else if( obj is BuilderWorkareaMode.Face face )
				{
					var b = new Bounds( face.Position );
					b.Expand( 0.5 );
					bounds.Add( b );
				}
			}

			Camera camera = Scene.Mode.Value == Scene.ModeEnum._3D ? Scene.CameraEditor : Scene.CameraEditor2D;
			if( !bounds.IsCleared() && camera != null )
			{
				var needRectangle = new Rectangle( .4f, .3f, .6f, .7f );

				var lookTo = bounds.GetCenter();
				var points = bounds.ToPoints();

				if( Scene.Mode.Value == Scene.ModeEnum._3D )
				{
					double distance = 1000;
					while( distance > 0.3 )
					{
						camera.SetPosition( lookTo - camera.TransformV.Rotation.GetForward() * distance );

						var viewport = ViewportControl2.Viewport;

						//!!!!good?
						bool processed = false;
						Scene_ViewportUpdateGetCameraSettings( Scene, viewport, ref processed );

						foreach( var point in points )
						{
							viewport.CameraSettings.ProjectToScreenCoordinates( point, out var screenPos );
							if( !needRectangle.Contains( screenPos ) )
								goto end;
						}

						distance /= 1.03f;
					}
end:;
				}
				else
				{
					camera.SetPosition( new Vector3( lookTo.ToVector2(), camera.TransformV.Position.Z ) );
				}
			}
#endif
		}

		[Browsable( false )]
		public ITransformTool TransformTool
		{
			get { return transformTool; }
		}

		[Browsable( false )]
		public TransformToolClass TransformTool2
		{
			get { return transformTool; }
		}

		virtual protected TransformToolObject TransformToolCreateObject( object forObject )
		{
			TransformToolObject transformToolObject = null;

			var handled = false;
			if( WorkareaMode != null && WorkareaMode.PerformTransformToolCreateObject( forObject, ref transformToolObject ) )
				handled = true;

			if( !handled )
			{
				//ObjectInSpace
				if( transformToolObject == null )
				{
					//!!!!для каких объектов не создавать?

					var objectInSpace = forObject as ObjectInSpace;
					if( objectInSpace != null )
						return new TransformToolObjectObjectInSpace( objectInSpace );
				}
			}

			return transformToolObject;
		}

		[Browsable( false )]
		public new SceneEditorWorkareaMode WorkareaMode
		{
			get { return (SceneEditorWorkareaMode)base.WorkareaMode; }
		}

		public void ResetWorkareaMode()
		{
			WorkareaModeSet( "" );
			transformTool.Mode = transformToolModeRestore;
		}

		public void ChangeCreateObjectsMode( CreateObjectsModeEnum mode )
		{
			//reset terrain workarea mode
			if( WorkareaMode as TerrainEditingMode != null )
				ResetWorkareaMode();

			if( CreateObjectsMode == mode )
				return;

			CreateObjectsMode = mode;

			//!!!!что-нибудь делать после изменения?
		}

		bool CreateByClickCreate()
		{
			(var objectType, var referenceToObject, var anyData, var objectName) = EditorAPI2.GetSelectedObjectToCreate();
			if( objectType != null )//|| memberFullSignature != "" || createNodeWithComponent != null )
			{
				var typeWillCreated = CreateObjectWhatTypeWillCreated( objectType, referenceToObject );
				if( typeWillCreated != null && MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( typeWillCreated ) )
				{
					var newObject = CreateObjectByCreationData( objectType, referenceToObject, anyData, objectName );
					if( newObject != null )
					{
						objectTypeToCreate = objectType;
						objectToCreate = newObject;
						CreateObject_Update();
						return true;
					}
				}
			}

			return false;
		}

		private void ViewportControl_MouseEnter( object sender, EventArgs e )
		{
			if( CreateObjectsMode == CreateObjectsModeEnum.Click && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick ) && ObjectCreationMode == null )
			{
				if( CreateByClickCreate() )
					createByClickEntered = true;
			}
			if( CreateObjectsMode == CreateObjectsModeEnum.Brush && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByBrush ) && ObjectCreationMode == null )
			{
				if( CreateByBrushCreate() )
					createByBrushEntered = true;
			}
		}

		void CreateByClickCancel()
		{
			CreateObject_Destroy();
			CreateSetProperty_Cancel();
			createByClickEntered = false;
		}

		private void ViewportControl_MouseLeave( object sender, EventArgs e )
		{
			if( CreateObjectsMode == CreateObjectsModeEnum.Click )
				CreateByClickCancel();
			if( CreateObjectsMode == CreateObjectsModeEnum.Brush )
				CreateByBrushCancel();
		}

		public override void WorkareaModeSet( string name, DocumentWindowWithViewportWorkareaMode instance = null )
		{
			base.WorkareaModeSet( name, instance );

			CreateByClickCancel();
			CreateByBrushCancel();

			if( new Rectangle( 0, 0, 1, 1 ).Contains( Viewport.MousePosition ) )
			{
				if( CreateObjectsMode == CreateObjectsModeEnum.Click && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick ) && ObjectCreationMode == null )
				{
					if( CreateByClickCreate() )
						createByClickEntered = true;
				}
				if( CreateObjectsMode == CreateObjectsModeEnum.Brush && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByBrush ) && ObjectCreationMode == null )
				{
					if( CreateByBrushCreate() )
						createByBrushEntered = true;
				}
			}
		}

		public override void ObjectCreationModeSet( ObjectCreationMode mode )
		{
			if( mode == null )
			{
				//continue modes
				if( new Rectangle( 0, 0, 1, 1 ).Contains( Viewport.MousePosition ) )
				{
					if( CreateObjectsMode == CreateObjectsModeEnum.Click && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick ) )
					{
						if( CreateByClickCreate() )
							createByClickEntered = true;
					}
					if( CreateObjectsMode == CreateObjectsModeEnum.Brush && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByBrush ) )
					{
						if( CreateByBrushCreate() )
							createByBrushEntered = true;
					}
				}
			}

			base.ObjectCreationModeSet( mode );
		}

		bool CreateByBrushCreate()
		{
			var toGroupOfObjects = GetObjectCreationSettings().destinationObject as GroupOfObjects;
			if( toGroupOfObjects != null )
			{
				//when destination != null

				(var objectType, var referenceToObject, var anyData, var objectName) = GetSelectedObjectToCreateWithCheckSelectedElementOfGroupOfObjects();
				if( objectType != null )
				{
					//mesh
					{
						Mesh mesh = null;
						Metadata.TypeInfo meshGeometryProcedural = null;
						{
							var componentType = objectType as Metadata.ComponentTypeInfo;
							if( componentType != null && componentType.BasedOnObject != null )
							{
								//Mesh
								mesh = componentType.BasedOnObject as Mesh;

								//Import3D
								if( componentType.BasedOnObject is Import3D )
									mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Mesh;
							}

							//for Stores Window. the window provides reference to mesh because it can still not downloaded
							if( mesh == null && MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( objectType ) )
							{
								var componentTypeInfo = MetadataManager.GetType( referenceToObject ) as Metadata.ComponentTypeInfo;
								if( componentTypeInfo != null )
									mesh = componentTypeInfo.BasedOnObject as Mesh;
							}

							//MeshGeometry_Procedural
							if( MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
								meshGeometryProcedural = objectType;
						}

						if( mesh != null || meshGeometryProcedural != null )
							return true;
					}

					//surface
					{
						Surface surface = null;
						{
							var componentType = objectType as Metadata.ComponentTypeInfo;
							if( componentType != null && componentType.BasedOnObject != null )
								surface = componentType.BasedOnObject as Surface;
						}

						if( surface != null )
							return true;
					}
				}
			}
			else
			{
				//when toGroupOfObjects == null

				(var objectType, var referenceToObject, var anyData, var objectName) = GetSelectedObjectToCreateWithCheckSelectedElementOfGroupOfObjects();
				if( objectType != null )
				{
					if( MetadataManager.GetTypeOfNetType( typeof( Surface ) ).IsAssignableFrom( objectType ) )
						return true;

					var typeWillCreated = CreateObjectWhatTypeWillCreated( objectType, referenceToObject );
					if( typeWillCreated != null && MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( typeWillCreated ) )
						return true;
				}
			}

			return false;
		}

		void CreateByBrushCancel()
		{
			CreateByBrushPaintEnd( false );
			createByBrushEntered = false;
		}

		bool CreateByBrushPaintBegin( Viewport viewport )
		{
			if( CreateObjectsMode == CreateObjectsModeEnum.Brush && !viewport.MouseRelativeMode && createByBrushEntered )
			{
				//var toGroupOfObjects = CreateObjectGetDestinationSelected() as GroupOfObjects;
				//if( CreateByBrushGetCenter( viewport, toGroupOfObjects, out _ ) )
				if( CreateByBrushGetToolPosition( out _, out _, out var affectsComponents ) )
				{

					var toGroupOfObjects = GetObjectCreationSettings().destinationObject as GroupOfObjects;
					if( toGroupOfObjects != null )
					{
						var destinationCachedBaseObjects = toGroupOfObjects.GetBaseObjects();
						if( !affectsComponents.Any( c => destinationCachedBaseObjects.Contains( c ) ) )
							return false;
					}

					createByBrushPaint = true;
					createByBrushPaintDeleting = ( ModifierKeys & Keys.Shift ) != 0;

					//CreateObjectsTick() вызвать?

					//!!!!?
					return true;
				}

				//!!!!?
				//return true;
			}

			return false;
		}

		bool CreateByBrushPaintEnd( bool cancel )
		{
			//!!!!

			if( createByBrushPaint )
			{
				if( cancel )
				{
					//cancel created or deleted

					if( createByBrushGroupOfObjectsObjectsCreated.Count != 0 )
						createByBrushGroupOfObjects.ObjectsRemove( createByBrushGroupOfObjectsObjectsCreated.ToArray() );

					if( createByBrushGroupOfObjectsObjectsDeleted.Count != 0 )
						createByBrushGroupOfObjects.ObjectsAdd( createByBrushGroupOfObjectsObjectsDeleted.ToArray() );

					foreach( var component in createByBrushComponentsCreated )
						component.Dispose();

					foreach( var item in createByBrushComponentsToDelete )
						if( item.wasEnabled )
							item.obj.Enabled = true;
				}
				else
				{
					//add to undo

					var undoMultiAction = new UndoMultiAction();

					if( createByBrushGroupOfObjectsObjectsCreated.Count != 0 )
					{
						undoMultiAction.AddAction( new GroupOfObjectsUndo.UndoActionCreateDelete( createByBrushGroupOfObjects, createByBrushGroupOfObjectsObjectsCreated.ToArray(), true, false ) );
					}

					if( createByBrushGroupOfObjectsObjectsDeleted.Count != 0 )
					{
						undoMultiAction.AddAction( new GroupOfObjectsUndo.UndoActionCreateDelete( createByBrushGroupOfObjects, createByBrushGroupOfObjectsObjectsDeleted.ToArray(), false, false ) );
					}

					if( createByBrushComponentsCreated.Count != 0 )
						undoMultiAction.AddAction( new UndoActionComponentCreateDelete( Document2, createByBrushComponentsCreated, true ) );

					if( createByBrushComponentsToDelete.Count != 0 )
					{
						var objects = new List<Component>( createByBrushComponentsToDelete.Count );
						foreach( var item in createByBrushComponentsToDelete )
						{
							if( item.wasEnabled )
								item.obj.Enabled = true;
							objects.Add( item.obj );
						}
						undoMultiAction.AddAction( new UndoActionComponentCreateDelete( Document2, objects, false ) );
					}

					if( undoMultiAction.Actions.Count != 0 )
						Document2.CommitUndoAction( undoMultiAction );
				}

				createByBrushPaint = false;
				createByBrushPaintDeleting = false;
				createByBrushGroupOfObjects = null;
				createByBrushGroupOfObjectsObjectsCreated.Clear();
				createByBrushGroupOfObjectsObjectsDeleted.Clear();
				createByBrushComponentsCreated.Clear();
				createByBrushComponentsToDelete.Clear();
				return true;
			}

			return false;
		}

		void CreateByBrushPaintTickStep( Viewport viewport )
		{
			//!!!!

			(var createObjectsMode, var destinationMode, var destinationObject) = GetObjectCreationSettings();

			//!!!!
			//if( destinationMode == CreateObjectsDestinationModeEnum.ToPaintLayer )
			//{

			//	//!!!!
			//	bool deleting;
			//	if( createByBrushPaint )
			//		deleting = createByBrushPaintDeleting;
			//	else
			//		deleting = ( ModifierKeys & Keys.Shift ) != 0;

			//	//!!!!

			//	//!!!!
			//	var overObject = GetMouseOverObjectToSelectByClick( out var context );
			//	if( overObject != null )//&& context.ScreenLabelItem != null )
			//	{

			//		(var objectType, var referenceToObject, var anyData, var objectName) = GetSelectedObjectToCreateWithCheckSelectedElementOfGroupOfObjects();
			//		if( objectType != null )
			//		{

			//			if( CreateByBrushGetToolPosition( out var toolPosition, out var toolDirection, out var affectsComponents ) )
			//			{
			//				var toolRadius = CreateObjectsBrushRadius;
			//				var toolStrength = CreateObjectsBrushStrength;

			//				//!!!!другие объекты охватывать


			//				//!!!!нет террейна
			//				var component = context.ResultObject as Component;
			//				//var component = GetMouseOverObjectToSelectByClick();//var obj = CreateObjectGetDestinationSelected();

			//				if( component != null && context.ResultPosition.HasValue )
			//				{
			//					var objectInSpace = component as ObjectInSpace;
			//					if( objectInSpace != null )
			//					{
			//						var data = new SceneUtility.PaintToLayerData();
			//						data.AffectObjects = new Component[] { component };

			//						data.Position = toolPosition;
			//						data.Rotation = Quaternion.FromDirectionZAxisUp( toolDirection );
			//						data.Radius = toolRadius;
			//						data.Height = toolRadius * 2; //data.Height = toolRadius;
			//						data.Strength = toolStrength;

			//						data.CanCreateLayer = true;
			//						data.Removing = deleting;

			//						//!!!!так?

			//						//Material
			//						if( MetadataManager.GetTypeOfNetType( typeof( Material ) ).IsAssignableFrom( objectType ) )
			//						{
			//							var componentType = objectType as Metadata.ComponentTypeInfo;
			//							if( componentType != null && componentType.BasedOnObject != null )
			//								data.Material = ReferenceUtility.MakeReference( objectType.Name );// componentType.BasedOnObject as Material;
			//						}

			//						//Surface
			//						if( MetadataManager.GetTypeOfNetType( typeof( Surface ) ).IsAssignableFrom( objectType ) )
			//						{
			//							var componentType = objectType as Metadata.ComponentTypeInfo;
			//							if( componentType != null && componentType.BasedOnObject != null )
			//								data.Surface = ReferenceUtility.MakeReference( objectType.Name ); //componentType.BasedOnObject as Surface;
			//						}

			//						SceneUtility.PaintToLayer( data );
			//					}
			//				}
			//			}
			//		}
			//	}

			//	//!!!!
			//	return;

			//}


			//!!!!

			{

				var toGroupOfObjects = GetObjectCreationSettings().destinationObject as GroupOfObjects;
				var toolRadius = CreateObjectsBrushRadius;
				var toolStrength = CreateObjectsBrushStrength;
				var toolHardness = CreateObjectsBrushHardness;
				var random = new FastRandom();

				double GetHardnessFactor( double length )
				{
					if( length == 0 || length <= toolHardness * toolRadius )
						return 1;
					else
					{
						double c;
						if( toolRadius - toolRadius * toolHardness != 0 )
							c = ( length - toolRadius * toolHardness ) / ( toolRadius - toolRadius * toolHardness );
						else
							c = 0;
						return (float)Math.Cos( Math.PI / 2 * c );
					}
				}

				if( CreateByBrushGetToolPosition( out var toolPosition, out var toolDirection, out var affectsComponents ) )
				//if( CreateByBrushGetCenter( viewport, toGroupOfObjects, out var center ) )
				{
					if( toGroupOfObjects != null )
					{
						//when destination != null

						var destinationCachedBaseObjects = toGroupOfObjects.GetBaseObjects();

						////!!!!trtt
						//{
						//	var terrain = Scene.GetComponent<Terrain>();
						//	if( terrain != null )
						//	{
						//		if( !destinationCachedBaseObjects.Contains( terrain ) )
						//		{
						//			destinationCachedBaseObjects.Add( terrain );
						//		}
						//	}
						//}



						if( !createByBrushPaintDeleting )
						{
							//creating

							(var objectType, var referenceToObject, var anyData, var objectName) = GetSelectedObjectToCreateWithCheckSelectedElementOfGroupOfObjects();
							if( objectType != null )
							{
								//mesh
								{
									Mesh mesh = null;
									ReferenceNoValue referenceToMesh = new ReferenceNoValue();
									Metadata.TypeInfo meshGeometryProcedural = null;
									{
										var componentType = objectType as Metadata.ComponentTypeInfo;
										if( componentType != null && componentType.BasedOnObject != null )
										{
											//!!!!пока только ресурсные ссылки поддерживаются

											//Mesh
											mesh = componentType.BasedOnObject as Mesh;
											if( mesh != null )
												referenceToMesh = ReferenceUtility.MakeResourceReference( mesh );

											//Import3D
											if( componentType.BasedOnObject is Import3D )
											{
												mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Mesh;
												if( mesh != null )
													referenceToMesh = ReferenceUtility.MakeResourceReference( mesh );
											}
										}

										//for Stores Window. the window provides reference to mesh because it can still not downloaded
										if( mesh == null && MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( objectType ) )
										{
											var componentTypeInfo = MetadataManager.GetType( referenceToObject ) as Metadata.ComponentTypeInfo;
											if( componentTypeInfo != null )
											{
												mesh = componentTypeInfo.BasedOnObject as Mesh;
												if( mesh != null )
													referenceToMesh = new ReferenceNoValue( referenceToObject );
											}
										}

										//MeshGeometry_Procedural
										if( MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
										{
											meshGeometryProcedural = objectType;
											referenceToMesh = new ReferenceNoValue( "this:$Mesh" );
										}
									}

									if( mesh != null || meshGeometryProcedural != null )
									{
										double minDistanceBetweenObjects = 1;
										if( mesh != null )
											minDistanceBetweenObjects = mesh.Result.SpaceBounds.BoundingSphere.Radius;// * 2;

										//calculate object count
										int count;
										{
											var toolSquare = Math.PI * toolRadius * toolRadius;

											double radius = minDistanceBetweenObjects / 2;
											double objectSquare = Math.PI * radius * radius;
											if( objectSquare < 0.1 )
												objectSquare = 0.1;

											double maxCount = toolSquare / objectSquare;
											maxCount /= 20;

											count = (int)( toolStrength * (double)maxCount );
											count = Math.Max( count, 1 );
										}

										var data = new List<GroupOfObjects.Object>( count );

										//find element
										GroupOfObjectsElement_Mesh element = null;
										{
											var elements = toGroupOfObjects.GetComponents<GroupOfObjectsElement_Mesh>();

											if( mesh != null )
												element = elements.FirstOrDefault( e => e.Mesh.Value == mesh && e.Enabled );

											if( meshGeometryProcedural != null )
											{
												foreach( var e in elements )
												{
													if( e.Enabled )
													{
														var mesh2 = e.GetComponent( "Mesh" ) as Mesh;
														if( mesh2 != null )
														{
															var meshGeometry = mesh2.GetComponent( "Mesh Geometry" );
															if( meshGeometry != null && meshGeometryProcedural.IsAssignableFrom( meshGeometry.BaseType ) )
															{
																element = e;
																break;
															}
														}
													}
												}
											}
										}

										//create element
										if( element == null )
										{
											var elementIndex = toGroupOfObjects.GetFreeElementIndex();
											var elementMesh = toGroupOfObjects.CreateComponent<GroupOfObjectsElement_Mesh>( enabled: false );

											//set name
											string defaultName = "";
											{
												string fileName = "";
												if( mesh != null )
													fileName = ComponentUtility.GetOwnedFileNameOfComponent( mesh );
												if( !string.IsNullOrEmpty( fileName ) )
													defaultName = "Mesh " + Path.GetFileNameWithoutExtension( fileName );
												if( string.IsNullOrEmpty( defaultName ) )
													defaultName = "Mesh";
											}
											if( elementMesh.Parent.GetComponent( defaultName ) == null )
												elementMesh.Name = defaultName;
											else
												elementMesh.Name = elementMesh.Parent.Components.GetUniqueName( defaultName, false, 2 );

											elementMesh.Index = elementIndex;
											elementMesh.Mesh = referenceToMesh;
											//пока только ресурсные ссылки поддерживаются
											//elementMesh.Mesh = ReferenceUtility.MakeResourceReference( mesh );
											toGroupOfObjects.ElementTypesCacheNeedUpdate();

											if( meshGeometryProcedural != null )
											{
												var mesh2 = elementMesh.CreateComponent<Mesh>();
												mesh2.Name = "Mesh";
												var geometry = mesh2.CreateComponent( meshGeometryProcedural );
												geometry.Name = "Mesh Geometry";
											}

											elementMesh.Enabled = true;

											element = elementMesh;
											//!!!!какой порядок в undo
											createByBrushComponentsCreated.Add( element );
										}

										//create point container to check by MinDistanceBetweenObjects
										PointContainer3D pointContainerFindFreePlace;
										{
											var bounds = new Bounds( toolPosition );
											bounds.Expand( toolRadius + minDistanceBetweenObjects );
											pointContainerFindFreePlace = new PointContainer3D( bounds, 100 );

											var item = new GroupOfObjects.GetObjectsItem( GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, true, bounds );
											toGroupOfObjects.GetObjects( item );
											foreach( var resultItem in item.Result )
											{
												ref var obj = ref toGroupOfObjects.ObjectGetData( resultItem.ObjectIndex );
												if( obj.Element == element.Index )
													pointContainerFindFreePlace.Add( ref obj.Position );
											}
										}

										for( int n = 0; n < count; n++ )
										{
											Vector3? position = null;

											int counter = 0;
											while( counter < 10 )
											{
												var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

												//check by radius and by hardness
												var length = offset.Length();
												if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
												{
													var position2 = toolPosition.ToVector2() + offset;

													var result = SceneUtility.CalculateObjectPositionZ( Scene, toGroupOfObjects, toolPosition.Z, position2, destinationCachedBaseObjects );
													if( result.found )
													{
														var p = new Vector3( position2, result.positionZ );
														//move up to align
														{
															var mesh2 = element.Mesh.Value;
															if( mesh2 != null )
																p.Z += -mesh2.Result.SpaceBounds.BoundingBox.Minimum.Z;
														}

														//check by MinDistanceBetweenObjects
														if( !pointContainerFindFreePlace.Contains( new Sphere( p, minDistanceBetweenObjects ) ) )
														{
															//found place to create
															position = p;
															break;
														}
													}
												}

												counter++;
											}

											if( position != null )
											{
												var obj = new GroupOfObjects.Object();
												obj.Element = (ushort)element.Index.Value;
												obj.Flags = GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible;
												obj.Position = position.Value;
												obj.Rotation = QuaternionF.Identity;
												obj.Scale = Vector3F.One;
												obj.Color = ColorValue.One;
												data.Add( obj );

												//add to point container
												pointContainerFindFreePlace.Add( ref obj.Position );
											}
										}

										createByBrushGroupOfObjects = toGroupOfObjects;

										var newIndexes = toGroupOfObjects.ObjectsAdd( data.ToArray() );
										createByBrushGroupOfObjectsObjectsCreated.AddRange( newIndexes );
									}
								}

								//surface
								{
									Surface surface = null;
									{
										var componentType = objectType as Metadata.ComponentTypeInfo;
										if( componentType != null && componentType.BasedOnObject != null )
											surface = componentType.BasedOnObject as Surface;
									}

									if( surface != null )
									{
										double maxOccupiedAreaRadius;
										double averageOccupiedAreaRadius;
										{
											var groups = surface.GetComponents<SurfaceGroupOfElements>();
											if( groups.Length != 0 )
											{
												maxOccupiedAreaRadius = 0;
												averageOccupiedAreaRadius = 0;
												foreach( var group in groups )
												{
													if( group.OccupiedAreaRadius > maxOccupiedAreaRadius )
														maxOccupiedAreaRadius = group.OccupiedAreaRadius;
													averageOccupiedAreaRadius += group.OccupiedAreaRadius;
												}
												averageOccupiedAreaRadius /= groups.Length;
											}
											else
											{
												maxOccupiedAreaRadius = 1;
												averageOccupiedAreaRadius = 1;
											}
										}

										//calculate object count
										int count;
										{
											var toolSquare = Math.PI * toolRadius * toolRadius;

											double radius = averageOccupiedAreaRadius;//maxOccupiedAreaRadius;
											double objectSquare = Math.PI * radius * radius;
											if( objectSquare < 0.1 )
												objectSquare = 0.1;

											double maxCount = toolSquare / objectSquare;
											maxCount /= 20;

											count = (int)( toolStrength * (double)maxCount );
											count = Math.Max( count, 1 );
										}

										var data = new List<GroupOfObjects.Object>( count );

										//find element
										var element = toGroupOfObjects.GetComponents<GroupOfObjectsElement_Surface>().FirstOrDefault( e => e.Surface.Value == surface );

										//create element
										if( element == null )
										{
											var elementIndex = toGroupOfObjects.GetFreeElementIndex();

											var elementSurface = toGroupOfObjects.CreateComponent<GroupOfObjectsElement_Surface>();

											//set name
											string defaultName = "";
											{
												var fileName = ComponentUtility.GetOwnedFileNameOfComponent( surface );
												if( !string.IsNullOrEmpty( fileName ) )
													defaultName = "Surface " + Path.GetFileNameWithoutExtension( fileName );
												if( string.IsNullOrEmpty( defaultName ) )
													defaultName = "Surface";
											}
											if( elementSurface.Parent.GetComponent( defaultName ) == null )
												elementSurface.Name = defaultName;
											else
												elementSurface.Name = elementSurface.Parent.Components.GetUniqueName( defaultName, false, 2 );

											elementSurface.Index = elementIndex;
											//!!!!пока только ресурсные ссылки поддерживаются
											elementSurface.Surface = ReferenceUtility.MakeResourceReference( surface );
											toGroupOfObjects.ElementTypesCacheNeedUpdate();

											element = elementSurface;

											//!!!!какой порядок в undo
											createByBrushComponentsCreated.Add( element );
										}

										var totalBounds = new Bounds( toolPosition );
										totalBounds.Expand( toolRadius + maxOccupiedAreaRadius * 4.01 );

										var initSettings = new OctreeContainer.InitSettings();
										initSettings.InitialOctreeBounds = totalBounds;
										initSettings.OctreeBoundsRebuildExpand = Vector3.Zero;
										initSettings.MinNodeSize = totalBounds.GetSize() / 40;
										var octree = new OctreeContainer( initSettings );

										var octreeOccupiedAreas = new List<Sphere>( 256 );

										{
											var item = new GroupOfObjects.GetObjectsItem( GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, true, totalBounds );
											toGroupOfObjects.GetObjects( item );
											foreach( var resultItem in item.Result )
											{
												ref var obj = ref toGroupOfObjects.ObjectGetData( resultItem.ObjectIndex );
												if( obj.Element == element.Index )
												{
													var surfaceGroup = surface.GetGroup( obj.VariationGroup );
													if( surfaceGroup != null )
													{
														octreeOccupiedAreas.Add( new Sphere( obj.Position, surfaceGroup.OccupiedAreaRadius ) );

														var b = new Bounds( obj.Position );
														b.Expand( surfaceGroup.OccupiedAreaRadius * 4 );
														octree.AddObject( ref b, 1 );
													}
												}
											}
										}

										for( int n = 0; n < count; n++ )
										{
											surface.GetRandomVariation( new Surface.GetRandomVariationOptions(), random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );
											var surfaceGroup = surface.GetGroup( groupIndex );

											Vector3? position = null;
											Vector3F? normal = null;

											for( var nRadiusMultiplier = 0; nRadiusMultiplier < 3; nRadiusMultiplier++ )
											{
												var radiusMultiplier = 1.0;
												switch( nRadiusMultiplier )
												{
												case 0: radiusMultiplier = 4; break;
												case 1: radiusMultiplier = 2; break;
												case 2: radiusMultiplier = 1; break;
												}

												int counter = 0;
												while( counter < 10 )
												{
													var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

													//check by radius and by hardness
													var length = offset.Length();
													if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
													{
														var position2 = toolPosition.ToVector2() + offset;

														var regularAlignment = surfaceGroup.RegularAlignment;
														if( regularAlignment != 0 )
														{
															position2 /= regularAlignment;
															position2 = new Vector2( (int)position2.X, (int)position2.Y );
															position2 *= regularAlignment;
														}

														var result = SceneUtility.CalculateObjectPositionZ( Scene, toGroupOfObjects, toolPosition.Z, position2, destinationCachedBaseObjects );
														if( result.found )
														{
															var p = new Vector3( position2, result.positionZ );

															var objSphere = new Sphere( p, surfaceGroup.OccupiedAreaRadius );
															objSphere.ToBounds( out var objBounds );

															var occupied = false;

															foreach( var index in octree.GetObjects( objBounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.All ) )
															{
																var sphere = octreeOccupiedAreas[ index ];
																sphere.Radius *= 0.25;//back to original
																sphere.Radius *= radiusMultiplier;//multiply

																if( ( p - sphere.Center ).LengthSquared() < ( sphere.Radius + objSphere.Radius ) * ( sphere.Radius + objSphere.Radius ) )
																{
																	occupied = true;
																	break;
																}
															}

															if( !occupied )
															{
																//found place to create
																position = p;
																normal = result.normal;
																goto end;
															}
														}
													}

													counter++;
												}
											}

end:;

											if( position != null )
											{
												//!!!!good?
												//calculate rotation again with normal
												surface.GetRandomVariation( new Surface.GetRandomVariationOptions( groupIndex, normal ), random, out _, out _, out _, out rotation, out _ );

												//add object
												var obj = new GroupOfObjects.Object();
												obj.Element = (ushort)element.Index.Value;
												obj.VariationGroup = groupIndex;
												obj.VariationElement = elementIndex;
												obj.Flags = GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible;
												obj.Position = position.Value + new Vector3( 0, 0, positionZ );
												obj.Rotation = rotation;
												obj.Scale = scale;
												obj.Color = ColorValue.One;
												data.Add( obj );

												//add to the octree

												octreeOccupiedAreas.Add( new Sphere( position.Value, surfaceGroup.OccupiedAreaRadius ) );

												var b = new Bounds( position.Value );
												b.Expand( surfaceGroup.OccupiedAreaRadius * 4 );
												octree.AddObject( ref b, 1 );
											}
										}

										octree.Dispose();

										createByBrushGroupOfObjects = toGroupOfObjects;

										var newIndexes = toGroupOfObjects.ObjectsAdd( data.ToArray() );
										createByBrushGroupOfObjectsObjectsCreated.AddRange( newIndexes );
									}
								}

							}

						}
						else
						{
							//deleting

							(var objectType, var referenceToObject, var anyData, var objectName) = GetSelectedObjectToCreateWithCheckSelectedElementOfGroupOfObjects();
							if( objectType != null )
							{
								int elementIndex = -1;
								{
									//mesh
									{
										Mesh mesh = null;
										Metadata.TypeInfo meshGeometryProcedural = null;
										{
											var componentType = objectType as Metadata.ComponentTypeInfo;
											if( componentType != null && componentType.BasedOnObject != null )
											{
												//Mesh
												mesh = componentType.BasedOnObject as Mesh;

												//Import3D
												if( componentType.BasedOnObject is Import3D )
													mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Mesh;
											}

											//for Stores Window. the window provides reference to mesh because it can still not downloaded
											if( mesh == null && MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( objectType ) )
											{
												var componentTypeInfo = MetadataManager.GetType( referenceToObject ) as Metadata.ComponentTypeInfo;
												if( componentTypeInfo != null )
													mesh = componentTypeInfo.BasedOnObject as Mesh;
											}

											//MeshGeometry_Procedural
											if( MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
												meshGeometryProcedural = objectType;
										}

										var elements = toGroupOfObjects.GetComponents<GroupOfObjectsElement_Mesh>();

										if( mesh != null )
										{
											var element = elements.FirstOrDefault( e => e.Mesh.Value == mesh && e.Enabled );
											if( element != null )
												elementIndex = element.Index;
										}

										if( meshGeometryProcedural != null )
										{
											foreach( var e in elements )
											{
												if( e.Enabled )
												{
													var mesh2 = e.GetComponent( "Mesh" ) as Mesh;
													if( mesh2 != null )
													{
														var meshGeometry = mesh2.GetComponent( "Mesh Geometry" );
														if( meshGeometry != null && meshGeometryProcedural.IsAssignableFrom( meshGeometry.BaseType ) )
														{
															elementIndex = e.Index;
															break;
														}
													}
												}
											}
										}
									}

									//surface
									{
										Surface surface = null;
										{
											var componentType = objectType as Metadata.ComponentTypeInfo;
											if( componentType != null && componentType.BasedOnObject != null )
												surface = componentType.BasedOnObject as Surface;
										}

										if( surface != null )
										{
											var element = toGroupOfObjects.GetComponents<GroupOfObjectsElement_Surface>().FirstOrDefault( e => e.Surface.Value == surface );
											if( element != null )
												elementIndex = element.Index;
										}
									}
								}

								if( elementIndex != -1 )
								{
									var bounds = new Bounds( toolPosition );
									bounds.Expand( toolRadius );

									var resultItem = new GroupOfObjects.GetObjectsItem( GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, false, bounds );
									toGroupOfObjects.GetObjects( resultItem );

									var indexesToDelete = new List<int>( resultItem.Result.Length );

									foreach( var item in resultItem.Result )
									{
										ref var obj = ref toGroupOfObjects.ObjectGetData( item.ObjectIndex );
										var length = ( toolPosition.ToVector2() - obj.Position.ToVector2() ).Length();

										if( obj.Element == elementIndex && length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) && random.NextDouble() <= toolStrength + 0.05 )
										{
											createByBrushGroupOfObjects = toGroupOfObjects;

											indexesToDelete.Add( item.ObjectIndex );
											createByBrushGroupOfObjectsObjectsDeleted.Add( obj );
										}
									}

									if( indexesToDelete.Count != 0 )
										toGroupOfObjects.ObjectsRemove( indexesToDelete.ToArray() );
								}
							}
						}

					}
					else
					{
						//when toGroupOfObjects == null

						if( !createByBrushPaintDeleting )
						{
							//creating

							(var objectType, var referenceToObject, var anyData, var objectName) = GetSelectedObjectToCreateWithCheckSelectedElementOfGroupOfObjects();
							if( objectType != null )
							{
								Component createTo = GetObjectCreationSettings().destinationObject as Layer;
								if( createTo == null )
									createTo = Scene;

								if( MetadataManager.GetTypeOfNetType( typeof( Surface ) ).IsAssignableFrom( objectType ) )
								{
									//surface

									Surface surface = null;
									{
										var componentType = objectType as Metadata.ComponentTypeInfo;
										if( componentType != null && componentType.BasedOnObject != null )
											surface = componentType.BasedOnObject as Surface;
									}

									if( surface?.Result != null )
									{
										var surfaceAllMeshesSet = new ESet<Mesh>();
										surfaceAllMeshesSet.AddRangeWithCheckAlreadyContained( surface.Result.GetAllMeshes() );

										double maxOccupiedAreaRadius;
										double averageOccupiedAreaRadius;
										{
											var groups = surface.GetComponents<SurfaceGroupOfElements>();
											if( groups.Length != 0 )
											{
												maxOccupiedAreaRadius = 0;
												averageOccupiedAreaRadius = 0;
												foreach( var group in groups )
												{
													if( group.OccupiedAreaRadius > maxOccupiedAreaRadius )
														maxOccupiedAreaRadius = group.OccupiedAreaRadius;
													averageOccupiedAreaRadius += group.OccupiedAreaRadius;
												}
												averageOccupiedAreaRadius /= groups.Length;
											}
											else
											{
												maxOccupiedAreaRadius = 1;
												averageOccupiedAreaRadius = 1;
											}
										}

										//calculate object count
										int count;
										{
											var toolSquare = Math.PI * toolRadius * toolRadius;

											double radius = averageOccupiedAreaRadius;//maxOccupiedAreaRadius;
											double objectSquare = Math.PI * radius * radius;
											if( objectSquare < 0.1 )
												objectSquare = 0.1;

											double maxCount = toolSquare / objectSquare;
											maxCount /= 20;

											count = (int)( toolStrength * (double)maxCount );
											count = Math.Max( count, 1 );
										}

										for( int n = 0; n < count; n++ )
										{
											surface.GetRandomVariation( new Surface.GetRandomVariationOptions(), random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );
											var surfaceGroup = surface.GetGroup( groupIndex );

											Vector3? position = null;
											Vector3F? normal = null;

											int counter = 0;
											while( counter < 10 )
											{
												var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

												//check by radius and by hardness
												var length = offset.Length();
												if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
												{
													var position2 = toolPosition.ToVector2() + offset;

													var regularAlignment = surfaceGroup.RegularAlignment;
													if( regularAlignment != 0 )
													{
														position2 /= regularAlignment;
														position2 = new Vector2( (int)position2.X, (int)position2.Y );
														position2 *= regularAlignment;
													}

													//!!!!
													var result = SceneUtility.CalculateObjectPositionZ( Scene, null, toolPosition.Z, position2 );
													if( result.found )
													{
														var p = new Vector3( position2, result.positionZ );

														//check by OccupiedAreaRadius
														bool free = true;
														{
															var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, new Sphere( p, surfaceGroup.OccupiedAreaRadius ) );
															Scene.GetObjectsInSpace( getObjectsItem );
															foreach( var item in getObjectsItem.Result )
															{
																var obj = item.Object;

																if( ( toolPosition.ToVector2() - obj.TransformV.Position.ToVector2() ).Length() <= surfaceGroup.OccupiedAreaRadius * surfaceGroup.OccupiedAreaRadius )
																{
																	var meshInSpace = obj as MeshInSpace;
																	if( meshInSpace != null && meshInSpace.Mesh.Value != null && surfaceAllMeshesSet.Contains( meshInSpace.Mesh ) )
																	{
																		free = false;
																		break;
																	}
																}
															}
														}

														if( free )
														{
															//found place to create
															position = p;
															normal = result.normal;
															break;
														}
													}
												}

												counter++;
											}

											if( position != null )
											{
												ObjectInSpace newObject = null;
												{
													var elements = surfaceGroup.GetComponents();
													if( elementIndex < elements.Length )
													{
														var element = elements[ elementIndex ];

														//mesh element
														var elementMesh = element as SurfaceElement_Mesh;
														if( elementMesh != null )
														{
															var meshInSpace = createTo.CreateComponent<MeshInSpace>( enabled: false );
															if( meshInSpace != null )
															{
																//!!!!если в свойствах ссылки настроены тогда как? конвертировать в Resource reference?
																//где еще такое

																var mesh = elementMesh.Mesh.Value;
																if( mesh != null )
																	meshInSpace.Mesh = new ReferenceNoValue( ReferenceUtility.CalculateResourceReference( mesh ) );
																//meshInSpace.Mesh = elementMesh.Mesh;

																var replaceMaterial = elementMesh.ReplaceMaterial.Value;
																if( replaceMaterial != null )
																	meshInSpace.ReplaceMaterial = new ReferenceNoValue( ReferenceUtility.CalculateResourceReference( replaceMaterial ) );

																meshInSpace.VisibilityDistanceFactor = elementMesh.VisibilityDistanceFactor;
																meshInSpace.CastShadows = elementMesh.CastShadows;
																meshInSpace.ReceiveDecals = elementMesh.ReceiveDecals;
																meshInSpace.MotionBlurFactor = elementMesh.MotionBlurFactor;

																newObject = meshInSpace;
															}
														}

													}
												}

												if( newObject != null )
												{
													//!!!!good?
													//calculate rotation again with normal
													surface.GetRandomVariation( new Surface.GetRandomVariationOptions( groupIndex, normal ), random, out _, out _, out _, out rotation, out _ );

													//set transform
													var objectToTransform = ObjectInSpaceUtility.CalculateObjectToTransform( newObject );
													if( objectToTransform == null )
														objectToTransform = newObject;
													objectToTransform.Transform = new Transform( position.Value + new Vector3( 0, 0, positionZ ), rotation, scale );

													//set name
													newObject.Name = ComponentUtility.GetNewObjectUniqueName( newObject );
													//set default configuration
													newObject.NewObjectSetDefaultConfiguration();
													//finish object creation
													newObject.Enabled = true;

													createByBrushComponentsCreated.Add( newObject );
												}
											}
										}

									}
								}
								else
								{
									//object in space

									double minDistanceBetweenObjects = 1;
									{
										var componentType = objectType as Metadata.ComponentTypeInfo;
										if( componentType != null && componentType.BasedOnObject != null )
										{
											//Mesh
											var mesh = componentType.BasedOnObject as Mesh;
											if( mesh != null )
												minDistanceBetweenObjects = mesh.Result.SpaceBounds.BoundingSphere.Radius;// * 2;

											//Import3D
											if( componentType.BasedOnObject is Import3D )
											{
												mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Mesh;
												if( mesh != null )
													minDistanceBetweenObjects = mesh.Result.SpaceBounds.BoundingSphere.Radius;// * 2;
											}
										}

										//for Stores Window. the window provides reference to mesh because it can still not downloaded
										if( MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( objectType ) )
										{
											var componentTypeInfo = MetadataManager.GetType( referenceToObject ) as Metadata.ComponentTypeInfo;
											if( componentTypeInfo != null )
											{
												var mesh = componentTypeInfo.BasedOnObject as Mesh;
												if( mesh != null )
													minDistanceBetweenObjects = mesh.Result.SpaceBounds.BoundingSphere.Radius;// * 2;
											}
										}
									}

									//calculate object count
									int count;
									{
										var toolSquare = Math.PI * toolRadius * toolRadius;

										double radius = minDistanceBetweenObjects / 2;
										double objectSquare = Math.PI * radius * radius;
										if( objectSquare < 0.1 )
											objectSquare = 0.1;

										double maxCount = toolSquare / objectSquare;
										maxCount /= 20;

										count = (int)( toolStrength * (double)maxCount );
										count = Math.Max( count, 1 );
									}


									for( int n = 0; n < count; n++ )
									{
										Vector3? position = null;

										int counter = 0;
										while( counter < 10 )
										{
											var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

											//check by radius and by hardness
											var length = offset.Length();
											if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
											{
												var position2 = toolPosition.ToVector2() + offset;

												var result = SceneUtility.CalculateObjectPositionZ( Scene, null, toolPosition.Z, position2 );
												if( result.found )
												{
													var p = new Vector3( position2, result.positionZ );

													//check by MinDistanceBetweenObjects
													bool free = true;
													{
														var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, new Sphere( p, minDistanceBetweenObjects ) );
														Scene.GetObjectsInSpace( getObjectsItem );
														foreach( var item in getObjectsItem.Result )
														{
															var obj = item.Object;
															if( CreateObjectFilterEqual( objectType, referenceToObject, obj ) && ( toolPosition.ToVector2() - obj.TransformV.Position.ToVector2() ).Length() <= minDistanceBetweenObjects * minDistanceBetweenObjects )
															{
																free = false;
																break;
															}
														}
													}

													if( free )
													{
														//found place to create
														position = p;
														break;
													}
												}
											}

											counter++;
										}

										if( position != null )
										{
											var newObject = CreateObjectByCreationData( objectType, referenceToObject, anyData, objectName );
											if( newObject != null )
											{
												var objectInSpace = newObject as ObjectInSpace;
												if( objectInSpace != null )
												{
													var objectToTransform = ObjectInSpaceUtility.CalculateObjectToTransform( objectInSpace );
													if( objectToTransform == null )
														objectToTransform = objectInSpace;

													var p = position.Value;
													//move up to align
													p.Z += objectToTransform.TransformV.Position.Z - objectToTransform.SpaceBounds.BoundingBox.Minimum.Z;

													objectToTransform.Transform = new Transform( p, objectToTransform.Transform.Value.Rotation, objectToTransform.Transform.Value.Scale );
												}

												createByBrushComponentsCreated.Add( newObject );
											}
										}
									}
								}
							}
						}
						else
						{
							//deleting

							(var objectType, var referenceToObject, var anyData, var objectName) = GetSelectedObjectToCreateWithCheckSelectedElementOfGroupOfObjects();
							if( objectType != null )
							{
								if( MetadataManager.GetTypeOfNetType( typeof( Surface ) ).IsAssignableFrom( objectType ) )
								{
									//surface

									Surface surface = null;
									{
										var componentType = objectType as Metadata.ComponentTypeInfo;
										if( componentType != null && componentType.BasedOnObject != null )
											surface = componentType.BasedOnObject as Surface;
									}

									if( surface?.Result != null )
									{
										var surfaceAllMeshesSet = new ESet<Mesh>();
										surfaceAllMeshesSet.AddRangeWithCheckAlreadyContained( surface.Result.GetAllMeshes() );

										var bounds = new Bounds( toolPosition );
										bounds.Expand( toolRadius );

										var resultItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, bounds );
										Scene.GetObjectsInSpace( resultItem );

										var objectsToDelete = new List<Component>( resultItem.Result.Length );
										foreach( var item in resultItem.Result )
										{
											var obj = item.Object;
											var length = ( toolPosition.ToVector2() - obj.TransformV.Position.ToVector2() ).Length();

											if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) && random.NextDouble() <= toolStrength + 0.05 )
											{
												var meshInSpace = obj as MeshInSpace;
												if( meshInSpace != null && meshInSpace.Mesh.Value != null && surfaceAllMeshesSet.Contains( meshInSpace.Mesh ) )
													objectsToDelete.Add( obj );
											}
										}

										foreach( var obj in objectsToDelete )
										{
											createByBrushComponentsToDelete.Add( (obj, obj.Enabled) );
											obj.Enabled = false;
										}

									}
								}
								else
								{
									//object in space

									var typeWillCreated = CreateObjectWhatTypeWillCreated( objectType, referenceToObject );
									if( typeWillCreated != null && MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( typeWillCreated ) )
									{
										var bounds = new Bounds( toolPosition );
										bounds.Expand( toolRadius );

										var resultItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, typeWillCreated, true, bounds );
										Scene.GetObjectsInSpace( resultItem );

										var objectsToDelete = new List<Component>( resultItem.Result.Length );
										foreach( var item in resultItem.Result )
										{
											var obj = item.Object;
											var length = ( toolPosition.ToVector2() - obj.TransformV.Position.ToVector2() ).Length();

											if( CreateObjectFilterEqual( objectType, referenceToObject, obj ) && length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) && random.NextDouble() <= toolStrength + 0.05 )
											{
												objectsToDelete.Add( obj );
											}
										}

										foreach( var obj in objectsToDelete )
										{
											createByBrushComponentsToDelete.Add( (obj, obj.Enabled) );
											obj.Enabled = false;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		void CreateByBrushPaintTick( Viewport viewport, float delta )
		{
			if( CreateObjectsMode == CreateObjectsModeEnum.Brush && createByBrushPaint && !viewport.MouseRelativeMode )
			{
				//!!!!может соединить чтобы быстрее?
				createByBrushPaintTimer -= delta;
				while( createByBrushPaintTimer < 0 )
				{
					CreateByBrushPaintTickStep( viewport );
					createByBrushPaintTimer += .05f;
				}
			}
		}

		bool CreateByBrushGetToolPosition( out Vector3 toolPosition, out Vector3 toolDirection, out Component[] affectsComponents )
		{
			(var createObjectsMode, var destinationMode, var destinationObject) = GetObjectCreationSettings();

			if( createObjectsMode == CreateObjectsModeEnum.Brush )
			{

				//!!!!что полезно из CreateByBrushGetCenter?

				var context = new SceneEditorGetMouseOverObjectToSelectByClickContext();
				context.CheckOnlyObjectsWithEnabledSelectionByCursorFlag = false;
				GetMouseOverObjectToSelectByClick( context );

				if( context.ResultObject != null && context.ResultPosition.HasValue )
				{
					var meshInSpace = context.ResultObject as MeshInSpace;
					Terrain terrain = null;
					if( meshInSpace != null )
					{
						terrain = Terrain.GetTerrainByMeshInSpace( meshInSpace );
						if( terrain != null )
							meshInSpace = null;
					}
					if( context.CheckOnlyObjectsWithEnabledSelectionByCursorFlag && meshInSpace != null && !meshInSpace.EnabledSelectionByCursor )
						meshInSpace = null;


					if( meshInSpace != null || terrain != null )
					{
						toolPosition = context.ResultPosition.Value;
						var ray = Viewport.CameraSettings.GetRayByScreenCoordinates( Viewport.MousePosition );
						toolDirection = ray.Direction;

						if( meshInSpace != null )
							affectsComponents = new Component[] { meshInSpace };
						else
							affectsComponents = new Component[] { terrain };

						return true;
					}

				}
			}

			toolPosition = Vector3.Zero;
			toolDirection = Vector3.Zero;
			affectsComponents = null;
			return false;
		}

		//bool CreateByBrushGetCenter( Viewport viewport, GroupOfObjects destination, out Vector3 center )
		//{
		//	var ray = viewport.CameraSettings.GetRayByScreenCoordinates( viewport.MousePosition );

		//	if( destination != null )
		//	{
		//		//when destination != null

		//		double? minLength = null;
		//		Vector3 minPosition = Vector3.Zero;

		//		var baseObjects = destination.GetBaseObjects();
		//		foreach( var baseObject in baseObjects )
		//		{
		//			//terrain
		//			var terrain = baseObject as Terrain;
		//			if( terrain != null )
		//			{
		//				if( terrain.GetPositionByRay( ray, false, out var position ) )
		//				{
		//					var length = ( position - ray.Origin ).Length();
		//					if( minLength == null || length < minLength.Value )
		//					{
		//						minLength = length;
		//						minPosition = position;
		//					}
		//				}
		//			}

		//			//mesh in space
		//			var meshInSpace = baseObject as MeshInSpace;
		//			if( meshInSpace != null )
		//			{
		//				if( meshInSpace.RayCast( ray, Mesh.CompiledData.RayCastMode.Auto, out var scale, out _ ) )
		//				{
		//					var position = ray.GetPointOnRay( scale );
		//					var length = ( position - ray.Origin ).Length();
		//					if( minLength == null || length < minLength.Value )
		//					{
		//						minLength = length;
		//						minPosition = position;
		//					}
		//				}
		//			}

		//			////group of objects
		//			//var groupOfObjects = baseObject as GroupOfObjects;
		//			//if( groupOfObjects != null )
		//			//{
		//			//}

		//		}

		//		if( minLength != null )
		//		{
		//			center = minPosition;
		//			return true;
		//		}


		//		////!!!!trtt

		//		////when toGroupOfObjects == null
		//		//var result = CalculateCreateObjectPositionUnderCursor( Viewport, allowSnap: false );
		//		//if( result.found )
		//		//{
		//		//	center = result.position;
		//		//	return true;
		//		//}


		//	}
		//	else
		//	{
		//		//when toGroupOfObjects == null
		//		var result = CalculateCreateObjectPositionUnderCursor( Viewport, allowSnap: false );
		//		if( result.found )
		//		{
		//			center = result.position;
		//			return true;
		//		}
		//	}

		//	center = Vector3.Zero;
		//	return false;
		//}

		void CreateByBrushRender( Viewport viewport )
		{
			if( CreateObjectsMode == CreateObjectsModeEnum.Brush && createByBrushEntered && !viewport.MouseRelativeMode )
			{
				bool deleting;
				if( createByBrushPaint )
					deleting = createByBrushPaintDeleting;
				else
					deleting = ( ModifierKeys & Keys.Shift ) != 0;

				if( CreateByBrushGetToolPosition( out var toolPosition, out var toolDirection, out var affectsComponents ) )
				{
					var toolRadius = CreateObjectsBrushRadius;


					var toGroupOfObjects = GetObjectCreationSettings().destinationObject as GroupOfObjects;
					if( toGroupOfObjects != null )
					{
						//when destination != null

						var destinationCachedBaseObjects = toGroupOfObjects.GetBaseObjects();
						affectsComponents = affectsComponents.Where( c => destinationCachedBaseObjects.Contains( c ) ).ToArray();
					}


					//(var createObjectsMode, var destinationMode, var destinationObject) = GetObjectCreationSettings();

					//!!!!
					//if( destinationMode == CreateObjectsDestinationModeEnum.ToPaintLayer )
					//{

					//!!!!переключаться на box, cylinder

					//var toolBox = new Box( toolPosition, xxxx, xxxxxx );
					//var toolBounds = toolBox.ToBounds();




					//!!!!пока так
					toolDirection = new Vector3( 0, 0, -1 );




					var toolCylinder = new Cylinder( toolPosition - toolDirection * toolRadius, toolPosition + toolDirection * toolRadius, toolRadius );
					//var toolCylinder = new Cylinder( toolPosition - toolDirection * toolRadius / 2, toolPosition + toolDirection * toolRadius / 2, toolRadius );
					var toolBounds = toolCylinder.ToBounds();

					//var toolSphere = new Sphere( toolPosition, toolRadius );
					//var toolBounds = toolSphere.ToBounds();

					var renderer = viewport.Simple3DRenderer;

					//!!!!цвета в настройки редактора
					var color = !deleting ? new ColorValue( 1, 1, 0, 0.3 ) : new ColorValue( 1, 0, 0, 0.3 );
					renderer.SetColor( color );

					var cutVolume = new RenderingPipeline.RenderSceneData.CutVolumeItem( toolCylinder, CutVolumeFlags.Invert | CutVolumeFlags.CutSimple3DRenderer );
					//var cutVolume = new RenderingPipeline.RenderSceneData.CutVolumeItem( toolCylinder, true, false, false, true );
					//var cutVolume = new RenderingPipeline.RenderSceneData.CutVolumeItem( toolBox, true, false, false, true );

					renderer.SetCutVolumes( new RenderingPipeline.RenderSceneData.CutVolumeItem[] { cutVolume } );
					//var cutVolume = new RenderingPipeline.RenderSceneData.CutVolumeItem( toolSphere, true, false, false, true );

					foreach( var affectComponent in affectsComponents )
					{
						//MeshInSpace
						var meshInSpace = affectComponent as MeshInSpace;
						if( meshInSpace != null )
						{
							var mesh = meshInSpace.MeshOutput;
							if( mesh != null )
								renderer.AddMesh( mesh.Result, meshInSpace.Transform.Value.ToMatrix4(), false, false );
						}

						//Terrain
						var terrain = affectComponent as Terrain;
						if( terrain != null )
						{
							terrain.GetGeometryFromTiles( delegate ( SpaceBounds tileBounds, Vector3[] tileVertices, int[] tileIndices )
							{
								if( tileBounds.BoundingBox.Intersects( ref toolBounds ) )
									renderer.AddTriangles( tileVertices, tileIndices, Matrix4.Identity, false, false );
							} );
						}
					}

					renderer.ResetCutVolumes();




					//!!!!
					//}
					//else
					//{
					//	//!!!!так рисовать?
					//	//!!!!для каких режимов?

					//	//!!!!в настройки редактора
					//	var color = !deleting ? new ColorValue( 1, 1, 0 ) : new ColorValue( 1, 0, 0 );
					//	viewport.Simple3DRenderer.SetColor( color );

					//	const double step = Math.PI / 32;
					//	Vector3 lastPos = Vector3.Zero;
					//	for( double angle = 0; angle <= Math.PI * 2 + step / 2; angle += step )
					//	{
					//		var position = new Vector3( toolPosition.X + Math.Cos( angle ) * toolRadius, toolPosition.Y + Math.Sin( angle ) * toolRadius, 0 );

					//		//!!!!GroupOfObjects
					//		var toGroupOfObjects = destinationObject as GroupOfObjects;
					//		position.Z = SceneUtility.CalculateObjectPositionZ( Scene, toGroupOfObjects, toolPosition.Z, position.ToVector2() ).positionZ;

					//		if( angle != 0 )
					//		{
					//			const float zOffset = 0.2f;// .3f;
					//			viewport.Simple3DRenderer.AddLine( lastPos + new Vector3( 0, 0, zOffset ), position + new Vector3( 0, 0, zOffset ) );
					//		}

					//		lastPos = position;
					//	}

					//}
				}

				//	////!!!!нет террейна
				//	//var obj = GetMouseOverObjectToSelectByClick();
				//	////var obj = CreateObjectGetDestinationSelected();
				//	//if( obj != null )
				//	//{
				//	//	var objectInSpace = obj as ObjectInSpace;
				//	//	if( objectInSpace != null )
				//	//	{
				//	//		viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
				//	//		viewport.Simple3DRenderer.AddSphere( new Sphere( objectInSpace.Transform.Value.Position, 1 ) );
				//	//	}
				//	//}
				//}

				//var toGroupOfObjects = CreateObjectGetDestinationSelected() as GroupOfObjects;

				//if( CreateByBrushGetCenter( viewport, toGroupOfObjects, out var center ) )
				//{
				//	//!!!!в настройки редактора
				//	var color = !deleting ? new ColorValue( 1, 1, 0 ) : new ColorValue( 1, 0, 0 );
				//	viewport.Simple3DRenderer.SetColor( color );

				//	double radius = CreateObjectsBrushRadius;

				//	const double step = Math.PI / 32;
				//	Vector3 lastPos = Vector3.Zero;
				//	for( double angle = 0; angle <= Math.PI * 2 + step / 2; angle += step )
				//	{
				//		var position = new Vector3( center.X + Math.Cos( angle ) * radius, center.Y + Math.Sin( angle ) * radius, 0 );
				//		position.Z = SceneUtility.CalculateObjectPositionZ( Scene, toGroupOfObjects, center.Z, position.ToVector2() ).positionZ;

				//		if( angle != 0 )
				//		{
				//			const float zOffset = 0.2f;// .3f;
				//			viewport.Simple3DRenderer.AddLine( lastPos + new Vector3( 0, 0, zOffset ), position + new Vector3( 0, 0, zOffset ) );
				//		}

				//		lastPos = position;
				//	}



				//	//viewport.Simple3DRenderer.AddSphere( new Sphere( center, 0.1 ) );

				//	//{
				//	//	var destination = CreateObjectGetDestination();
				//	//	if( destination != null )
				//	//	{
				//	//		var bounds = new Bounds( center );
				//	//		bounds.Expand( radius );

				//	//		var resultItem = new GroupOfObjects.GetObjectsItem( GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, false, bounds );
				//	//		destination.GetObjects( resultItem );

				//	//		foreach( var item in resultItem.Result )
				//	//		{
				//	//			var index = item.Object;

				//	//			ref var objectMesh = ref destination.ObjectsMeshGetData( index );
				//	//			viewport.Simple3DRenderer.AddSphere( new Sphere( objectMesh.Position, 0.1 ) );
				//	//		}

				//	//	}
				//	//}
				//}
			}
		}

		void UpdateCreateObjectsDestinationCachedList()
		{
			if( Time.Current > createObjectsDestinationLastUpdateTime + 1.0 )
			{
				createObjectsDestinationLastUpdateTime = Time.Current;

				createObjectsDestinationCachedList.Clear();

				if( CreateObjectsMode == CreateObjectsModeEnum.Drop || CreateObjectsMode == CreateObjectsModeEnum.Click )
				{
					//Drop, Click

					createObjectsDestinationCachedList.Add( (CreateObjectsDestinationModeEnum.SeparateObjectsToRoot, null, EditorLocalization2.Translate( "General", "to Root" )) );

					foreach( var obj in Scene.GetComponents<Layer>( checkChildren: true, onlyEnabledInHierarchy: true ) )
					{
						if( obj.DisplayInEditor )
							createObjectsDestinationCachedList.Add( (CreateObjectsDestinationModeEnum.SeparateObjectsToLayer, obj, "to " + obj.Name) );
					}
				}
				else
				{
					//Brush

					//auto mode
					createObjectsDestinationCachedList.Add( (CreateObjectsDestinationModeEnum.Auto, null, EditorLocalization2.Translate( "General", "Auto" )) );

					//!!!!
					////paint layer
					//createObjectsDestinationCachedList.Add( (CreateObjectsDestinationModeEnum.ToPaintLayer, null, EditorLocalization.Translate( "General", "to Paint layer" )) );

					//Group Of Objects
					foreach( var obj in Scene.GetComponents<GroupOfObjects>( checkChildren: true, onlyEnabledInHierarchy: true ) )
					{
						if( obj.DisplayInEditor && obj.EditorAllowUsePaintBrush )
							createObjectsDestinationCachedList.Add( (CreateObjectsDestinationModeEnum.ToGroupOfObjects, obj, "to " + obj.Name) );
					}

					//separate objects. Root, layers
					createObjectsDestinationCachedList.Add( (CreateObjectsDestinationModeEnum.SeparateObjectsToRoot, null, EditorLocalization2.Translate( "General", "Separate objects to Root" )) );
					foreach( var obj in Scene.GetComponents<Layer>( checkChildren: true, onlyEnabledInHierarchy: true ) )
					{
						if( obj.DisplayInEditor )
							createObjectsDestinationCachedList.Add( (CreateObjectsDestinationModeEnum.SeparateObjectsToLayer, obj, "Separate objects to " + obj.Name) );
					}
				}
			}
		}

		void UpdateTerrainPaintLayersCachedList()
		{
			if( Time.Current > terrainPaintLayersLastUpdateTime + 1.0 )
			{
				terrainPaintLayersLastUpdateTime = Time.Current;

				var layers = new ESet<PaintLayer>();
				{
					var terrains = Scene.GetComponents<Terrain>( checkChildren: true );
					foreach( var terrain in terrains )
					{
						foreach( var layer in terrain.GetComponents<PaintLayer>() )
						{
							if( layer.Enabled && layer.MaskImage.Value == null )
								layers.Add( layer );
						}
					}
				}

				terrainPaintLayersCachedList.Clear();
				//terrainPaintLayersCachedList.Add( (null, "Base") );

				foreach( var layer in layers )
				{
					var name = "";

					Component obj = layer.Material.Value;
					if( obj == null )
						obj = layer.Surface.Value;

					if( obj != null )
					{
						if( obj.Parent == null )
						{
							var fileName = ComponentUtility.GetOwnedFileNameOfComponent( obj );
							if( !string.IsNullOrEmpty( fileName ) )
								name = Path.GetFileNameWithoutExtension( fileName );
						}
						else
							name = obj.Name;
					}

					if( string.IsNullOrEmpty( name ) )
						name = layer.ToString();

					terrainPaintLayersCachedList.Add( (layer, name) );
				}
			}
		}

		void UpdateTerrainPaintLayersSelected()
		{
			var action = EditorActions.GetByName( "Terrain Paint Layers" );
			if( action != null && !action.CompletelyDisabled )
			{
				var newIndex = action.ListBox.SelectedIndex;
				if( newIndex >= 0 && newIndex < terrainPaintLayersCachedList.Count )
					terrainPaintLayersSelected = terrainPaintLayersCachedList[ newIndex ].Obj;
				else
					terrainPaintLayersSelected = null;
			}
		}

		public PaintLayer TerrainPaintLayersGetSelected()
		{
			if( terrainPaintLayersSelected != null )
			{
				if( terrainPaintLayersSelected.Disposed || terrainPaintLayersSelected.ParentRoot != Scene )
					terrainPaintLayersSelected = null;
			}
			return terrainPaintLayersSelected;
		}

		protected virtual object[] SelectByDoubleClick( object overObject )
		{
			var result = new ESet<object>();

			var radius = ProjectSettings.Get.SceneEditor.SceneEditorSelectByDoubleClickRadius.Value;

			var overObject2 = overObject as ObjectInSpace;
			if( overObject2 != null && radius > 0 )
			{
				var sphere = new Sphere( overObject2.TransformV.Position, radius );
				var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );
				Scene.GetObjectsInSpace( getObjectsItem );

				foreach( var item in getObjectsItem.Result )
				{
					var obj = item.Object;
					if( obj.VisibleInHierarchy && obj.CanBeSelectedInHierarchy && ( obj.TransformV.Position - overObject2.TransformV.Position ).Length() <= radius )
					{
						if( obj.BaseType == overObject2.BaseType )
						{
							//MeshInSpace specific
							var meshInSpace1 = obj as MeshInSpace;
							var meshInSpace2 = overObject2 as MeshInSpace;
							if( meshInSpace1 != null && meshInSpace2 != null )
							{
								if( meshInSpace1.Mesh.Value != meshInSpace2.Mesh.Value )
									continue;
							}

							result.AddWithCheckAlreadyContained( obj );
						}
					}
				}
			}

			return result.ToArray();
		}

		void AddScreenMessagesFromTerrain( List<string> lines )
		{
			Component terrain = null;
			//Terrain terrain = null;

			foreach( var obj in SelectedObjects )
			{
				var terrain2 = obj as Terrain as Component;
				//var terrain2 = obj as Terrain;
				if( terrain2 != null )
					terrain = terrain2;

				var layer = obj as PaintLayer;
				if( layer != null )
				{
					var terrain3 = layer.Parent as Terrain as Component;
					//var terrain3 = layer.Parent as Terrain;
					if( terrain3 != null )
						terrain = terrain3;
				}
			}

			if( terrain != null )
			{
				var list = new List<(PaintLayer, int)>();

				foreach( var layer in terrain.GetComponents<PaintLayer>() )
				{
					if( layer.Enabled && layer.IsDataAvailable() )
					{
						var materialData = layer.Material.Value?.Result;
						if( materialData == null )
							materialData = ResourceUtility.MaterialNull.Result;

						int materialType;
						if( materialData.deferredShadingSupport )
							materialType = 0;
						else if( materialData.Transparent )
							materialType = 2;
						else
							materialType = 1;

						list.Add( (layer, materialType) );
					}
				}

				var list2 = new List<(PaintLayer, int)>( list );

				CollectionUtility.InsertionSort( list2, delegate ( (PaintLayer, int) v1, (PaintLayer, int) v2 )
				{
					if( v1.Item2 < v2.Item2 )
						return -1;
					if( v1.Item2 > v2.Item2 )
						return 1;
					return 0;
				} );

				var displayMessage = !list.SequenceEqual( list2 );
				if( displayMessage )
				{
					if( lines.Count != 0 )
						lines.Add( "" );
					lines.Add( "The layers of this terrain are displayed in a different sequence than they are defined" );
					lines.Add( "in the list because the materials of the layers drawn at different stages of rendering" );
					lines.Add( "(deferred, opacity forward, transparent)." );
					//lines.Add( "The layers of this terrain are displayed in a different sequence than they are defined in the list because the materials of the layers drawn at different stages of rendering (deferred, opacity forward, transparent)." );
				}
			}
		}

		void UpdateScreenLabelTooltip()
		{
			Component component = null;
			string text = "";

			var overObject = GetMouseOverObjectToSelectByClick( out var context );
			if( overObject != null && context.ScreenLabelItem != null )
			{
				component = context.ResultObject as Component;
				if( component != null )
					text = !string.IsNullOrEmpty( component.Name ) ? component.Name : component.ToString();
			}

			if( screenLabelToolTipComponent != component || screenLabelToolTipText != text )
			{
				screenLabelToolTipComponent = component;
				screenLabelToolTipText = text;

				screenLabelToolTip.Hide( ViewportControl2 );
				screenLabelToolTip.SetToolTip( ViewportControl2, screenLabelToolTipText );
			}
		}

		(CreateObjectsModeEnum createObjectsMode, CreateObjectsDestinationModeEnum destinationMode, Component destinationObject) GetObjectCreationSettings()
		{
			//disable disposed selected object
			if( createObjectsDestinationSelected.Obj != null )
			{
				if( createObjectsDestinationSelected.Obj.Disposed || createObjectsDestinationSelected.Obj.ParentRoot != Scene )
				{
					if( CreateObjectsMode == CreateObjectsModeEnum.Drop || CreateObjectsMode == CreateObjectsModeEnum.Click )
						createObjectsDestinationSelected = (CreateObjectsDestinationModeEnum.SeparateObjectsToRoot, null);
					else
						createObjectsDestinationSelected = (CreateObjectsDestinationModeEnum.Auto, null);
				}
			}


			(CreateObjectsModeEnum createObjectsMode, CreateObjectsDestinationModeEnum destinationMode, Component destinationObject) result = (CreateObjectsMode, createObjectsDestinationSelected.Mode, createObjectsDestinationSelected.Obj);

			//set destinationObject to Scene
			if( result.createObjectsMode == CreateObjectsModeEnum.Drop || result.createObjectsMode == CreateObjectsModeEnum.Click )
			{
				if( result.destinationMode == CreateObjectsDestinationModeEnum.SeparateObjectsToRoot )
					result.destinationObject = Scene;
			}

			//get actual mode for Auto mode
			if( result.createObjectsMode == CreateObjectsModeEnum.Brush && result.destinationMode == CreateObjectsDestinationModeEnum.Auto )
			{
				//!!!!
				////PaintLayer mode
				//{
				//	(var objectType, var referenceToObject, var anyData, var objectName) = GetSelectedObjectToCreateWithCheckSelectedElementOfGroupOfObjects();// EditorAPI.GetSelectedObjectToCreate();
				//	if( objectType != null )
				//	{
				//		var componentType = objectType as Metadata.ComponentTypeInfo;
				//		if( componentType != null && componentType.BasedOnObject != null )
				//		{
				//			//Material
				//			var material = componentType.BasedOnObject as Material;
				//			if( material != null )
				//				return (CreateObjectsModeEnum.Brush, CreateObjectsDestinationModeEnum.ToPaintLayer, material);

				//			//Surface
				//			var surface = componentType.BasedOnObject as Surface;
				//			//!!!!если нет материала?
				//			if( surface != null && surface.Material.ReferenceOrValueSpecified )
				//				return (CreateObjectsModeEnum.Brush, CreateObjectsDestinationModeEnum.ToPaintLayer, surface);
				//		}
				//	}
				//}


				//create to group of objects mode

				GroupOfObjects groupOfObjects = null;
				{
					foreach( var obj in Scene.GetComponents( checkChildren: true, onlyEnabledInHierarchy: true ) )
					{
						if( obj is GroupOfObjects obj2 && obj2.EditorAllowUsePaintBrush && obj.DisplayInEditor )
						{
							groupOfObjects = obj2;
							break;
						}
					}
				}

				if( groupOfObjects != null )
				{
					(var objectType, var referenceToObject, var anyData, string objectName) = GetSelectedObjectToCreateWithCheckSelectedElementOfGroupOfObjects();
					if( objectType != null )
					{
						//mesh
						{
							Mesh mesh = null;
							Metadata.TypeInfo meshGeometryProcedural = null;
							{
								var componentType = objectType as Metadata.ComponentTypeInfo;
								if( componentType != null && componentType.BasedOnObject != null )
								{
									//Mesh
									mesh = componentType.BasedOnObject as Mesh;

									//Import3D
									if( componentType.BasedOnObject is Import3D )
										mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Mesh;
								}

								//for Stores Window. the window provides reference to mesh because it can still not downloaded
								if( mesh == null && MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( objectType ) )
								{
									var componentTypeInfo = MetadataManager.GetType( referenceToObject ) as Metadata.ComponentTypeInfo;
									if( componentTypeInfo != null )
										mesh = componentTypeInfo.BasedOnObject as Mesh;
								}

								//MeshGeometry_Procedural
								if( MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
									meshGeometryProcedural = objectType;
							}

							if( mesh != null || meshGeometryProcedural != null )
								return (CreateObjectsModeEnum.Brush, CreateObjectsDestinationModeEnum.ToGroupOfObjects, groupOfObjects);
						}

						//surface
						{
							Surface surface = null;
							{
								var componentType = objectType as Metadata.ComponentTypeInfo;
								if( componentType != null && componentType.BasedOnObject != null )
									surface = componentType.BasedOnObject as Surface;
							}

							if( surface != null )
								return (CreateObjectsModeEnum.Brush, CreateObjectsDestinationModeEnum.ToGroupOfObjects, groupOfObjects);
						}
					}
				}

				return (CreateObjectsModeEnum.Brush, CreateObjectsDestinationModeEnum.SeparateObjectsToRoot, Scene);
			}

			return result;
		}

		(Metadata.TypeInfo objectType, string referenceToObject, object anyData, string objectName) GetSelectedObjectToCreateWithCheckSelectedElementOfGroupOfObjects()
		{
			(Metadata.TypeInfo objectType, string referenceToObject, object anyData, string objectName) result = EditorAPI2.GetSelectedObjectToCreate();

			Component oneSelectedComponent = null;
			if( SelectedObjects.Length == 1 )
				oneSelectedComponent = SelectedObjects[ 0 ] as Component;

			//group of objects elements
			if( oneSelectedComponent != null )
			{
				//mesh element
				{
					var element = oneSelectedComponent as GroupOfObjectsElement_Mesh;
					if( element != null )
					{
						var mesh = element.Mesh.Value;
						if( mesh != null )
						{
							var type = mesh.GetProvidedType();
							if( type != null )
								result = (type, element.Mesh.GetByReference, null, "");
						}
					}
				}

				//surface element
				{
					var element = oneSelectedComponent as GroupOfObjectsElement_Surface;
					if( element != null )
					{
						var surface = element.Surface.Value;
						if( surface != null )
						{
							var type = surface.GetProvidedType();
							if( type != null )
								result = (type, element.Surface.GetByReference, null, "");
						}
					}
				}
			}

			return result;
		}

	}
}