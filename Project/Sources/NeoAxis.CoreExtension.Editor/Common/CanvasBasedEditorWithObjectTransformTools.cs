// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
//!!!!WinForms
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	//Includes SeatItem, Vehicle specific for transform tool

	public class CanvasBasedEditorWithObjectTransformTools : CanvasBasedEditor
	{
		//[EngineConfig( "CanvasBasedEditorWithObjectTransformTools", "CreateObjectsMode" )]
		//public static CreateObjectsModeEnum CreateObjectsMode = CreateObjectsModeEnum.Drop;

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

		//create drag and drop or by click
		bool createByDropEntered;
		//bool createByClickEntered;
		//bool createByBrushEntered;
		Metadata.TypeInfo objectTypeToCreate;
		Component objectToCreate;
		object createSetPropertyObject;
		Metadata.Property createSetProperty;
		object[] createSetPropertyIndexes;
		IReference createSetPropertyOldValue;
		Component createSetPropertyVisualizeCanSelectObject;

		bool skipSelectionByButtonInMouseUp;

		bool sceneRenderSubscribed;

		EngineToolTip screenLabelToolTip = new EngineToolTip();
		Component screenLabelToolTipComponent;
		string screenLabelToolTipText = "";

		/////////////////////////////////////

		//public enum CreateObjectsModeEnum
		//{
		//	Drop,
		//	Click,
		//}

		/////////////////////////////////////////

		static CanvasBasedEditorWithObjectTransformTools()
		{
			EngineConfig.RegisterClassParameters( typeof( CanvasBasedEditorWithObjectTransformTools ) );
		}

		public Component ComponentOfEditor
		{
			get { return (Component)ObjectOfEditor; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			transformTool = new TransformToolClass( (EngineViewportControl)ViewportControl );
			transformTool.Mode = TransformToolMode.PositionRotation;

			transformTool.ModifyBegin += TransformToolModifyBegin;
			transformTool.ModifyCommit += TransformToolModifyCommit;
			transformTool.ModifyCancel += TransformToolModifyCancel;
			transformTool.CloneAndSelectObjects += TransformToolCloneAndSelectObjects;

			//ViewportControl2.MouseEnter += ViewportControl_MouseEnter;
			//ViewportControl2.MouseLeave += ViewportControl_MouseLeave;

			var owner = (Control)Owner;
			owner.DragDrop += new System.Windows.Forms.DragEventHandler( this.Scene_DocumentWindow_DragDrop );
			owner.DragEnter += new System.Windows.Forms.DragEventHandler( this.Scene_DocumentWindow_DragEnter );
			owner.DragOver += new System.Windows.Forms.DragEventHandler( this.Scene_DocumentWindow_DragOver );
			owner.DragLeave += new System.EventHandler( this.Scene_DocumentWindow_DragLeave );

			if( ObjectOfEditor != null )
				SelectObjects( new object[] { ObjectOfEditor } );
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

		protected override void OnKeyDown( KeyEvent e, ref bool handled )
		{
			base.OnKeyDown( e, ref handled );
			if( handled )
				return;

			//Inside only Escape key is processed.
			transformTool.PerformKeyDown( e, ref handled );
			if( handled )
				return;

			////disable workarea mode by Space or Escape
			//if( ( e.Key == EKeys.Space || e.Key == EKeys.Escape ) )//&& !toolModify )
			//{
			//	if( CreateObjectsMode == CreateObjectsModeEnum.Click && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick ) )
			//	{
			//		if( CreateObjectsMode == CreateObjectsModeEnum.Click )
			//			CreateByClickCancel();
			//		EditorAPI2.ResetSelectedObjectToCreate();
			//		handled = true;
			//		return;
			//	}
			//}
		}

		protected override void OnKeyPress( KeyPressEvent e, ref bool handled )
		{
			base.OnKeyPress( e, ref handled );
			if( handled )
				return;
		}

		protected override void OnKeyUp( KeyEvent e, ref bool handled )
		{
			base.OnKeyUp( e, ref handled );

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

		//public bool StartObjectCreationMode( Metadata.TypeInfo type, Component obj )
		//{
		//	var attributes = type/*objectTypeToCreate*/.GetNetType()/* objectToCreate.GetType()*/.GetCustomAttributes<ObjectCreationModeAttribute>().ToArray();
		//	if( attributes.Length != 0 )
		//	{
		//		//begin creation mode

		//		var type2 = GetCreationModeType( attributes[ 0 ] );
		//		var mode = (ObjectCreationMode)type2.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { this, obj/*objectToCreate*/ } );

		//		//ObjectCreationModeSet( mode );
		//		objectTypeToCreate = null;
		//		objectToCreate = null;
		//		//createByClickEntered = false;
		//		return true;
		//	}

		//	return false;
		//}

		public delegate void ViewportMouseDownAfterTransformToolDelegate( CanvasBasedEditorWithObjectTransformTools sender, /*Viewport viewport,*/ EMouseButtons button, ref bool handled );
		public static event ViewportMouseDownAfterTransformToolDelegate Viewport_MouseDown_AfterTransformTool;

		protected override void OnMouseDown( EMouseButtons button, ref bool handled )
		{
			//code from Viewport.PerformMouseDown
			if( Viewport.UIContainer != null && Viewport.UIContainer.PerformMouseDown( button ) )
			{
				handled = true;
				return;
			}

			base.OnMouseDown( button, ref handled );
			if( handled )
				return;

			//transform tool
			transformTool.PerformMouseDown( button, ref handled );
			if( handled )
				return;

			//Viewport_MouseDown_AfterTransformTool event
			{
				bool handled2 = false;
				Viewport_MouseDown_AfterTransformTool?.Invoke( this, /*Viewport, */button, ref handled2 );
				if( handled2 )
				{
					handled = true;
					return;
				}
			}

			////create by click
			//if( CreateObjectsMode == CreateObjectsModeEnum.Click && button == EMouseButtons.Left && createByClickEntered && objectToCreate != null )
			//{
			//	////creation mode
			//	//if( StartObjectCreationMode() )
			//	//{
			//	//	handled = true;
			//	//	return;
			//	//}
			//	{
			//		var attributes = objectTypeToCreate.GetNetType()/* objectToCreate.GetType()*/.GetCustomAttributes<ObjectCreationModeAttribute>().ToArray();
			//		if( attributes.Length != 0 )
			//		{
			//			//begin creation mode
			//			var type2 = GetCreationModeType( attributes[ 0 ] );
			//			var mode = (ObjectCreationMode)type2.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { this, objectToCreate } );
			//			ObjectCreationModeSet( mode );
			//			objectTypeToCreate = null;
			//			objectToCreate = null;
			//			createByClickEntered = false;
			//			handled = true;
			//			return;
			//		}
			//	}

			//	//commit creation
			//	if( CreateSetProperty_Commit() )
			//		CreateObject_Destroy();
			//	else
			//		CreateObject_Commit();
			//	createByClickEntered = false;

			//	//create new
			//	if( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick )
			//	{
			//		if( CreateByClickCreate() )
			//			createByClickEntered = true;
			//	}

			//	handled = true;
			//	return;
			//}

			//selection by rectangle
			if( button == EMouseButtons.Left && AllowSelectObjects )
			{
				selectByRectangle_Enabled = true;
				selectByRectangle_Activated = false;
				selectByRectangle_StartPosition = Viewport.MousePosition;
				selectByRectangle_LastMousePosition = selectByRectangle_StartPosition;

				handled = true;
				return;
			}

			//always set handled to disable calling uiContainer.PerformMouseDown. it is already called in the beginning of this method
			handled = true;
		}

		public delegate void Viewport_MouseUp_AfterTransformToolDelegate( CanvasBasedEditorWithObjectTransformTools sender, /*Viewport viewport,*/ EMouseButtons button, ref bool handled, ref bool skipSelectionByButtonInMouseUp );
		public static event Viewport_MouseUp_AfterTransformToolDelegate Viewport_MouseUp_AfterTransformTool;

		protected override void OnMouseUp( EMouseButtons button, ref bool handled )
		{
			//inside: cameraRotation
			base.OnMouseUp( button, ref handled );
			if( handled )
				return;

			//below:
			//transform tool
			//select by rectangle
			//context menu

			//transform tool
			transformTool.PerformMouseUp( button, ref handled );

			Viewport_MouseUp_AfterTransformTool?.Invoke( this, /*Viewport, */button, ref handled, ref skipSelectionByButtonInMouseUp );

			if( AllowSelectObjects && objectToCreate == null && !createByDropEntered && /*!createByClickEntered && !createByBrushEntered &&*/ !skipSelectionByButtonInMouseUp && !handled )
			{
				var selectedObjects = new ESet<object>( SelectedObjectsSet );

				//select by rectangle
				if( button == EMouseButtons.Left )
				{
					//reset selected objects even mode is not activated.

					bool shiftPressed = EditorAPI2.IsModifierKeyPressed( EKeys.Shift );
					if( !shiftPressed )
						selectedObjects.Clear();

					if( selectByRectangle_Enabled && selectByRectangle_Activated )
					{
						foreach( var obj in SelectByRectangle_GetObjectsInSpace() )
							selectedObjects.AddWithCheckAlreadyContained( obj );
						handled = true;
					}
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

			//context menu
			if( !handled && button == EMouseButtons.Right )
				ShowContextMenu();

			if( button == EMouseButtons.Left )
				skipSelectionByButtonInMouseUp = false;
		}

		protected override void OnMouseDoubleClick( EMouseButtons button, ref bool handled )
		{
			base.OnMouseDoubleClick( button, ref handled );
			if( handled )
				return;

			transformTool.PerformMouseDoubleClick( button, ref handled );
			if( handled )
				return;

			////select by double click
			//if( button == EMouseButtons.Left && AllowSelectObjects && objectToCreate == null && !createByDropEntered )//&& !createByClickEntered && !createByBrushEntered )
			//{
			//	var selectedObjects = new ESet<object>( SelectedObjectsSet );

			//	var obj = GetMouseOverObjectToSelectByClick();
			//	if( obj != null && IsObjectSelected( obj ) )
			//	{
			//		var addObjects = SelectByDoubleClick( obj );
			//		if( addObjects.Length != 0 )
			//		{
			//			if( !EditorAPI2.IsModifierKeyPressed( EKeys.Shift ) )
			//			{
			//				selectedObjects.Clear();
			//				selectedObjects.Add( obj );
			//			}
			//			selectedObjects.AddRangeWithCheckAlreadyContained( addObjects );

			//			//update selected objects
			//			SelectObjects( selectedObjects );

			//			handled = true;
			//			skipSelectionByButtonInMouseUp = true;
			//		}
			//	}
			//}
		}

		protected override void OnMouseMove( Vector2 mouse )
		{
			base.OnMouseMove( mouse );

			transformToolNeedCallOnMouseMove = true;
			transformToolNeedCallOnMouseMovePosition = mouse;

			//update select by rectangle
			if( selectByRectangle_Enabled && AllowSelectObjects )
			{
				Vector2 diffPixels = ( Viewport.MousePosition - selectByRectangle_StartPosition ) * Viewport.SizeInPixels.ToVector2();
				if( Math.Abs( diffPixels.X ) >= 3 || Math.Abs( diffPixels.Y ) >= 3 )
					selectByRectangle_Activated = true;

				selectByRectangle_LastMousePosition = Viewport.MousePosition;
			}

			////update create by click
			//if( CreateObjectsMode == CreateObjectsModeEnum.Click && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick ) && createByClickEntered )
			//{
			//	//для IMeshInSpaceChild потому как он создает при наведении
			//	//if( objectToCreate == null )
			//	//	DragDropCreateObject_Create( e );

			//	CreateObject_Update();
			//	//if( objectToCreate != null )
			//	//	e.Effect = DragDropEffects.Link;

			//	CreateSetProperty_Update( null );
			//	//if( createSetProperty != null )
			//	//	e.Effect = DragDropEffects.Link;

			//}
		}

		protected override void OnTick( float delta )
		{
			base.OnTick( delta );

			UpdateTransformToolObjects();
			UpdateTransformToolActiveState();

			if( transformToolNeedCallOnMouseMove )
			{
				transformTool.PerformMouseMove( transformToolNeedCallOnMouseMovePosition );
				transformToolNeedCallOnMouseMove = false;
			}
			transformTool.PerformTick( delta );

			UpdateScreenLabelTooltip();

			if( !sceneRenderSubscribed && Scene != null )
			{
				Scene.RenderEvent += Scene_RenderEvent;
				sceneRenderSubscribed = true;
			}
		}

		//this is not scene editor: SceneEditorGetMouseOverObjectToSelectByClickContext
		public void GetMouseOverObjectToSelectByClick2( SceneEditorGetMouseOverObjectToSelectByClickContext context )
		{
			var viewport = ViewportControl.Viewport;
			var mouse = viewport.MousePosition;

			if( !new Rectangle( 0, 0, 1, 1 ).Contains( mouse ) )
				return;
			if( viewport.MouseRelativeMode )
				return;

			//get by screen label
			foreach( var item in viewport.LastFrameScreenLabels.GetReverse() )
			{
				var obj = item.Object;
				//var objectInSpace = obj as ObjectInSpace;

				//if( objectInSpace != null && ( !context.CheckOnlyObjectsWithEnabledSelectionByCursorFlag || objectInSpace.EnabledSelectionByCursor ) || objectInSpace == null || item.AlwaysVisible )
				//{

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

				//}
			}


			////get by ray

			//var list = ComponentOfEditor.GetComponents<ObjectInSpace>( checkChildren: true, onlyEnabledInHierarchy: true );

			//ObjectInSpace.CheckSelectionByRayContext rayContext = new ObjectInSpace.CheckSelectionByRayContext();
			//rayContext.viewport = viewport;
			//rayContext.screenPosition = mouse;
			//rayContext.ray = viewport.CameraSettings.GetRayByScreenCoordinates( rayContext.screenPosition );

			////var request = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, rayContext.ray );
			////Scene.GetObjectsInSpace( request );
			////var list = request.Result;

			//ObjectInSpace foundObject = null;
			//double foundScale = 0;

			////foreach( var item in list )
			//foreach( var obj in list )
			//{
			//	//var obj = item.Object;

			//	if( !context.CheckOnlyObjectsWithEnabledSelectionByCursorFlag || obj.EnabledSelectionByCursor )
			//	{
			//		//reset this object data
			//		rayContext.thisObjectWasChecked = false;
			//		rayContext.thisObjectResultRayScale = -10000000;

			//		obj.CheckSelectionByRay( rayContext );

			//		if( rayContext.thisObjectWasChecked && rayContext.thisObjectResultRayScale >= 0 )
			//		{
			//			var scale = rayContext.thisObjectResultRayScale;

			//			bool foundNew;
			//			if( foundObject == null )
			//				foundNew = true;
			//			else
			//			{
			//				if( Math.Abs( foundScale - scale ) < 0.0001 )
			//				{
			//					var first = foundObject;
			//					var second = obj;
			//					bool secondIsParentOfFirst = first.GetAllParents().Contains( second );
			//					foundNew = secondIsParentOfFirst;
			//				}
			//				else
			//					foundNew = scale < foundScale;
			//			}

			//			if( foundNew )
			//			{
			//				foundObject = obj;
			//				foundScale = scale;
			//			}
			//		}
			//	}
			//}

			//context.ResultObject = foundObject;
			//context.ResultPosition = rayContext.ray.GetPointOnRay( foundScale );
		}

		public virtual void GetMouseOverObjectToSelectByClick( SceneEditorGetMouseOverObjectToSelectByClickContext context )
		{
			var viewport = ViewportControl.Viewport;
			var mouse = viewport.MousePosition;

			if( !new Rectangle( 0, 0, 1, 1 ).Contains( mouse ) )
				return;
			if( viewport.MouseRelativeMode )
				return;

			//if( WorkareaMode != null && WorkareaMode.PerformGetMouseOverObjectToSelectByClick( context ) )
			//	return;
			GetMouseOverObjectToSelectByClick2( context );
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

		protected virtual void AddObjectScreenLabel( ViewportRenderingContext context, Component obj, Vector3 objectPosition )
		{
			var context2 = context.ObjectInSpaceRenderingContext;
			var viewport = context2.viewport;
			//var t = transformValue;
			var pos = objectPosition;// t.Position;

			if( viewport.CameraSettings.ProjectToScreenCoordinates( pos, out Vector2 screenPosition ) )
			{
				if( new Rectangle( 0, 0, 1, 1 ).Contains( ref screenPosition ) )
				{
					var settings = ProjectSettings.Get;
					var maxSize = settings.SceneEditor.ScreenLabelMaxSize.Value;
					var minSize = settings.SceneEditor.ScreenLabelMinSizeFactor.Value * maxSize;
					var maxDistance = settings.SceneEditor.ScreenLabelMaxDistance.Value;

					double distance = ( pos - viewport.CameraSettings.Position ).Length();
					if( distance < maxDistance )
					{
						Vector2 sizeInPixels = Vector2.Lerp( new Vector2( maxSize, maxSize ), new Vector2( minSize, minSize ), distance / maxDistance );
						Vector2 screenSize = sizeInPixels / viewport.SizeInPixels.ToVector2();

						var rect = new Rectangle( screenPosition - screenSize * .5, screenPosition + screenSize * .5 ).ToRectangleF();

						ColorValue color;
						if( context2.selectedObjects.Contains( obj ) )
							color = ProjectSettings.Get.Colors.SelectedColor;
						else if( context2.canSelectObjects.Contains( obj ) )
							color = ProjectSettings.Get.Colors.CanSelectColor;
						else
							color = ProjectSettings.Get.SceneEditor.ScreenLabelColor;

						context2.displayLabelsCounter++;

						var item = new Viewport.LastFrameScreenLabelItem();
						item.Object = obj;
						item.DistanceToCamera = (float)( objectPosition/*transformValue.Position*/ - context.Owner.CameraSettings.Position ).Length();
						item.ScreenRectangle = rect;
						item.Color = color;

						////remove display in corner
						//if( viewport.LastFrameScreenLabelByObjectInSpace.ContainsKey( obj ) )
						//{
						//	foreach( var item2 in viewport.LastFrameScreenLabels )
						//	{
						//		if( item2.Object == obj )
						//		{
						//			viewport.LastFrameScreenLabels.Remove( item2 );
						//			break;
						//		}
						//	}
						//}

						viewport.LastFrameScreenLabels.AddLast( item );
						viewport.LastFrameScreenLabelByObjectInSpace[ obj ] = item;
					}
				}
			}
		}

		void AddScreenLabels( Viewport viewport )//, ref double screenPositionY )
		{
			var context = viewport.RenderingContext;
			//var context2 = context.ObjectInSpaceRenderingContext;

			//var settings = ProjectSettings.Get;
			//var maxSize = settings.SceneEditor.ScreenLabelMaxSize.Value;
			//Vector2 sizeInPixels = new Vector2( maxSize, maxSize );
			//Vector2 screenSize = sizeInPixels / viewport.SizeInPixels.ToVector2();

			//var list = new List<Component>();

			{
				//Camera cameraEditor = Scene.Mode.Value == Scene.ModeEnum._3D ? Scene.CameraEditor : Scene.CameraEditor2D;

				//foreach( var obj in Scene.GetComponents( checkChildren: true, depthFirstSearch: true ) )
				foreach( var obj in ComponentOfEditor.GetComponents( checkChildren: true, depthFirstSearch: true ) )
				{
					if( obj.Enabled && obj.ScreenLabel.Value != ScreenLabelEnum.NeverDisplay )
					{

						//ObjectInSpace
						{
							var objectInSpace = obj as ObjectInSpace;
							if( objectInSpace != null )
								AddObjectScreenLabel( context, objectInSpace, objectInSpace.TransformV.Position );
						}

						//SeatItem specific
						{
							var seatItem = obj as SeatItem;
							if( seatItem != null )
								AddObjectScreenLabel( context, seatItem, seatItem.Transform.Value.Position );
						}

						//Vehicle specific
						{
							var vehicleTypeWheel = obj as VehicleTypeWheel;
							if( vehicleTypeWheel != null )
								AddObjectScreenLabel( context, vehicleTypeWheel, vehicleTypeWheel.Position );
						}


						//var light = obj as Light;
						//if( light != null )
						//{
						//	AddObjectScreenLabel( context, light, light.Transform );

						//	//var display = obj.ScreenLabel.Value;
						//	//if( display != ScreenLabelEnum.NeverDisplay )
						//	//{
						//	//	var info = obj.GetScreenLabelInfo();

						//	//	//display current camera always in the corner
						//	//	if( cameraEditor != null && obj == cameraEditor )
						//	//		info.DisplayInCorner = true;

						//	//	if( display == ScreenLabelEnum.AlwaysDisplay )
						//	//		info.DisplayInCorner = true;

						//	//	if( info.DisplayInCorner )
						//	//		list.Add( obj );
						//	//}

						//}

					}
				}
			}

			//for( int n = list.Count - 1; n >= 0; n-- )
			//{
			//	var obj = list[ n ];

			//	ColorValue color;
			//	if( context2.selectedObjects.Contains( obj ) )
			//		color = ProjectSettings.Get.Colors.SelectedColor;
			//	else if( context2.canSelectObjects.Contains( obj ) )
			//		color = ProjectSettings.Get.Colors.CanSelectColor;
			//	else
			//		color = ProjectSettings.Get.SceneEditor.ScreenLabelColor;

			//	var item = new Viewport.LastFrameScreenLabelItem();
			//	item.Object = obj;
			//	item.DistanceToCamera = -1;
			//	item.Color = color;
			//	if( !obj.EnabledInHierarchy )
			//		item.Color.Alpha *= 0.5f;

			//	item.AlwaysVisible = true;
			//	viewport.LastFrameScreenLabels.AddLast( item );
			//	viewport.LastFrameScreenLabelByObjectInSpace[ obj ] = item;
			//}

			//if( list.Count != 0 )
			//	screenPositionY = screenSize.Y * 1.25;
		}

		private void Scene_RenderEvent( Scene sender, Viewport viewport )
		{
			//double screenPositionY = 0;

			//screen labels
			if( viewport.CanvasRenderer != null )
				AddScreenLabels( viewport );//, ref screenPositionY );

			//foreach( var widget in CanvasWidgets )
			//	widget.OnUpdate( this, ref screenPositionY );
		}

		protected override void OnViewportUpdateGetObjectInSceneRenderingContext( ref ObjectInSpace.RenderingContext context )
		{
			base.OnViewportUpdateGetObjectInSceneRenderingContext( ref context );

			//prepare context

			context = new ObjectInSpace.RenderingContext( Viewport );

			if( !createByDropEntered )//&& !createByClickEntered )&& !createByBrushEntered && ObjectCreationMode == null )
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

				//select by click
				{
					bool skip = false;
					if( selectByRectangle_Activated )
						skip = true;
					if( transformTool != null && transformTool.IsMouseOverAxisToActivation() )
						skip = true;
					if( CameraRotating )
						skip = true;
					if( createByDropEntered )//|| createByClickEntered )|| createByBrushEntered )
						skip = true;
					if( !AllowSelectObjects )
						skip = true;

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

				//if( ObjectCreationMode != null )
				//	context.objectToCreate = ObjectCreationMode.CreatingObject;
				//else
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

		public delegate void Viewport_UpdateBeforeOutputEventDelegate( CanvasBasedEditorWithObjectTransformTools sender/*, Viewport viewport*/ );
		public static event Viewport_UpdateBeforeOutputEventDelegate Viewport_UpdateBeforeOutputEvent;

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			var renderer = Viewport.CanvasRenderer;

			//render UI controls
			Viewport.UIContainer.PerformRenderUI( renderer );

			//draw selection rectangle
			if( selectByRectangle_Enabled && selectByRectangle_Activated && AllowSelectObjects )
			{
				Rectangle rect = new Rectangle( selectByRectangle_StartPosition );
				rect.Add( Viewport.MousePosition );

				Vector2I windowSize = Viewport.SizeInPixels;
				Vector2 thickness = new Vector2( 1.0f / (float)windowSize.X, 1.0f / (float)windowSize.Y );

				renderer.AddRectangle( rect + thickness, new ColorValue( 0, 0, 0, .5f ) );
				renderer.AddRectangle( rect, new ColorValue( 0, 1, 0, 1 ) );
			}

			if( !selectByRectangle_Activated )
				transformTool.PerformRender();

			Viewport_UpdateBeforeOutputEvent?.Invoke( this );//, viewport );
		}

		protected override void OnViewportUpdateBeforeOutput2()
		{
			base.OnViewportUpdateBeforeOutput2();

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
			foreach( var item in ViewportControl.Viewport.LastFrameScreenLabels.GetReverse() )
			{
				var obj = item.Object;

				//var objectInSpace = item.Object as ObjectInSpace;
				//if( objectInSpace != null && objectInSpace.EnabledSelectionByCursor || objectInSpace == null || item.AlwaysVisible )
				//{

				if( rectangle.Contains( item.ScreenRectangle.GetCenter() ) )
					allSet.AddWithCheckAlreadyContained( obj );

				//}
			}

			//foreach( var c in ComponentOfEditor.GetComponents( false, true, true ) )
			//{
			//	var obj = c as ObjectInSpace;
			//	if( obj != null && obj.EnabledSelectionByCursor )
			//	{
			//		var transform = obj.Transform.Value;
			//		var pos = transform.Position;

			//		if( ViewportControl.Viewport.CameraSettings.ProjectToScreenCoordinates( pos, out Vector2 screenPosition ) )
			//		{
			//			if( rectangle.Contains( screenPosition ) )
			//				allSet.AddWithCheckAlreadyContained( obj );
			//		}
			//	}
			//}

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
		}

		protected virtual List<object> GetObjectsToSelectByRectangle( Rectangle rectangle )
		{
			//var result = new List<object>();
			//if( WorkareaMode != null && WorkareaMode.PerformGetObjectsToSelectByRectangle( rectangle, ref result ) )
			//	return result;
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

				var component = currentObject as Component;
				if( component != null && ( component.Parent == null || component.Disposed ) )
					deleted = true;
				//var objectInSpace = currentObject as ObjectInSpace;
				//if( objectInSpace != null && ( objectInSpace.Parent == null || objectInSpace.Disposed ) )
				//	deleted = true;

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
			//if( WorkareaMode != null && WorkareaMode.PerformTransformToolModifyBegin() )
			//	return;
		}

		protected virtual void TransformToolModifyCommit( TransformToolClass sender )
		{
			//if( WorkareaMode != null && WorkareaMode.PerformTransformToolModifyCommit() )
			//	return;

			if( !transformToolModifyCloned )
			{
				//add changes to undo list

				var undoItems = new List<UndoActionPropertiesChange.Item>();

				foreach( var toolObject in transformTool.Objects )
				{
					//ObjectInSpace
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

					//SeatItem specific
					{
						var toolObject2 = toolObject as TransformToolObjectSeatItem;
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

					//Vehicle specific
					{
						var toolObject2 = toolObject as TransformToolObjectVehicleTypeWheel;
						if( toolObject2 != null )
						{
							var obj = toolObject2.ObjectToTransform;
							if( obj != null )
							{
								var p = obj.MetadataGetMemberBySignature( "property:Position" ) as Metadata.Property;
								if( p != null )
								{
									var undoItem = new UndoActionPropertiesChange.Item( obj, p, toolObject2.BeforeModifyTransform, null );
									undoItems.Add( undoItem );
								}
							}
						}
					}

				}

				//undo
				if( undoItems.Count != 0 )
				{
					var action = new UndoActionPropertiesChange( undoItems.ToArray() );
					Document.UndoSystem.CommitAction( action );
					Document.Modified = true;
				}
			}
			else
			{
				var newObjects = new List<Component>();
				foreach( var toolObject in transformTool.Objects )
				{
					//ObjectInSpace
					{
						var toolObject2 = toolObject as TransformToolObjectObjectInSpace;
						if( toolObject2 != null )
							newObjects.Add( toolObject2.SelectedObject );
					}

					//SeatItem specific
					{
						var toolObject2 = toolObject as TransformToolObjectSeatItem;
						if( toolObject2 != null )
							newObjects.Add( toolObject2.SelectedObject );
					}

					//Vehicle specific
					{
						var toolObject2 = toolObject as TransformToolObjectVehicleTypeWheel;
						if( toolObject2 != null )
							newObjects.Add( toolObject2.SelectedObject );
					}

				}

				//add to undo with deletion
				var action = new UndoActionComponentCreateDelete( Document, newObjects, true );
				Document.UndoSystem.CommitAction( action );
				Document.Modified = true;

				//update selected objects to update Settings Window
				SelectObjects( SelectedObjects, forceUpdate: true );
			}

			transformToolModifyCloned = false;
		}

		protected virtual void TransformToolModifyCancel( TransformToolClass sender )
		{
			//if( WorkareaMode != null && WorkareaMode.PerformTransformToolModifyCancel() )
			//	return;

			if( transformToolModifyCloned )
			{
				ContentBrowserUtility.AllContentBrowsers_SuspendChildrenChangedEvent();

				//delete objects

				var toDelete = new List<Component>();
				foreach( var toolObject in transformTool.Objects )
				{
					//ObjectInSpace
					{
						var toolObject2 = toolObject as TransformToolObjectObjectInSpace;
						if( toolObject2 != null )
							toDelete.Add( toolObject2.SelectedObject );
					}

					//SeatItem specific
					{
						var toolObject2 = toolObject as TransformToolObjectSeatItem;
						if( toolObject2 != null )
							toDelete.Add( toolObject2.SelectedObject );
					}

					//Vehicle specific
					{
						var toolObject2 = toolObject as TransformToolObjectVehicleTypeWheel;
						if( toolObject2 != null )
							toDelete.Add( toolObject2.SelectedObject );
					}

				}

				//clear selected objects
				SelectObjects( null );

				//delete
				foreach( var obj in toDelete )
					obj.RemoveFromParent( true );

				ComponentOfEditor/*Scene*/.HierarchyController?.ProcessDelayedOperations();
				ContentBrowserUtility.AllContentBrowsers_ResumeChildrenChangedEvent();
			}

			transformToolModifyCloned = false;
		}

		protected virtual void TransformToolCloneAndSelectObjects()
		{
			//if( WorkareaMode != null && WorkareaMode.PerformTransformToolCloneAndSelectObjects() )
			//	return;

			ContentBrowserUtility.AllContentBrowsers_SuspendChildrenChangedEvent();

			var objectsToClone = new List<Component>();
			foreach( var toolObject in transformTool.Objects )
			{
				//ObjectInSpace
				{
					var toolObject2 = toolObject as TransformToolObjectObjectInSpace;
					if( toolObject2 != null )
						objectsToClone.Add( toolObject2.SelectedObject );
				}

				//SeatItem specific
				{
					var toolObject2 = toolObject as TransformToolObjectSeatItem;
					if( toolObject2 != null )
						objectsToClone.Add( toolObject2.SelectedObject );
				}

				//Vehicle specific
				{
					var toolObject2 = toolObject as TransformToolObjectVehicleTypeWheel;
					if( toolObject2 != null )
						objectsToClone.Add( toolObject2.SelectedObject );
				}

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

			ComponentOfEditor/*Scene*/.HierarchyController?.ProcessDelayedOperations();
			ContentBrowserUtility.AllContentBrowsers_ResumeChildrenChangedEvent();

			//select objects
			SelectObjects( newObjects.Cast<object>().ToArray(), updateSettingsWindowSelectObjects: false );

			//add screen message
			EditorUtility.ShowScreenNotificationObjectsCloned( newObjects.Count );

			transformToolModifyCloned = true;

			UpdateTransformToolObjects();
			transformTool.PerformUpdateInitialObjectsTransform();
		}

		protected override void OnEditorActionGetState( EditorActionGetStateContext context )
		{
			base.OnEditorActionGetState( context );

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

			case "Snap All Axes":
			case "Snap X":
			case "Snap Y":
			case "Snap Z":
				if( CanSnap( out _ ) )
					context.Enabled = true;
				break;

				//case "Focus Camera On Selected Object":
				//	if( CanFocusCameraOnSelectedObject( out _ ) )
				//		context.Enabled = true;
				//	break;

				//case "Create Objects By Drag & Drop":
				//	context.Enabled = true;
				//	context.Checked = CreateObjectsMode == CreateObjectsModeEnum.Drop;
				//	break;

				//case "Create Objects By Click":
				//	context.Enabled = true;
				//	context.Checked = CreateObjectsMode == CreateObjectsModeEnum.Click;
				//	break;

			}
		}

		protected override void OnEditorActionClick( EditorActionClickContext context )
		{
			base.OnEditorActionClick( context );

			switch( context.Action.Name )
			{
			case "Select":
				if( transformTool != null )
				{
					//if( transformTool.Mode == TransformToolMode.Undefined )
					//	WorkareaModeSet( "" );
					transformTool.Mode = TransformToolMode.None;
				}
				break;

			case "Move & Rotate":
				if( transformTool != null )
				{
					//if( transformTool.Mode == TransformToolMode.Undefined )
					//	WorkareaModeSet( "" );
					transformTool.Mode = TransformToolMode.PositionRotation;
				}
				break;

			case "Move":
				if( transformTool != null )
				{
					//if( transformTool.Mode == TransformToolMode.Undefined )
					//	WorkareaModeSet( "" );
					transformTool.Mode = TransformToolMode.Position;
				}
				break;

			case "Rotate":
				if( transformTool != null )
				{
					//if( transformTool.Mode == TransformToolMode.Undefined )
					//	WorkareaModeSet( "" );
					transformTool.Mode = TransformToolMode.Rotation;
				}
				break;

			case "Scale":
				if( transformTool != null )
				{
					//if( transformTool.Mode == TransformToolMode.Undefined )
					//	WorkareaModeSet( "" );
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

				//case "Focus Camera On Selected Object":
				//	if( CanFocusCameraOnSelectedObject( out var objects ) )
				//		FocusCameraOnSelectedObject( objects );
				//	break;

				//case "Create Objects By Drag & Drop":
				//	ChangeCreateObjectsMode( CreateObjectsModeEnum.Drop );
				//	break;

				//case "Create Objects By Click":
				//	ChangeCreateObjectsMode( CreateObjectsModeEnum.Click );
				//	break;


			}
		}

		string TranslateContextMenu( string text )
		{
			return EditorContextMenu.Translate( text );
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
			if( ViewportControl != null && ViewportControl.Viewport != null )
				mouse = ViewportControl.Viewport.MousePosition;

			//Editor
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Editor" ), EditorResourcesCache.Edit, delegate ( object s, EventArgs e2 )
				{
					EditorAPI2.OpenDocumentWindowForObject( (DocumentInstance)Document, oneSelectedComponent );
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
					var obj = oneSelectedComponent ?? ObjectOfEditor;
					bool canUseAlreadyOpened = !EditorAPI2.IsModifierKeyPressed( EKeys.Shift );// !ModifierKeys.HasFlag( Keys.Shift );
					EditorAPI2.ShowObjectSettingsWindow( (DocumentInstance)Document, obj, canUseAlreadyOpened );
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

			EditorContextMenuWinForms.Show( items, (Control)Owner );
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
				parentsForNewObjects.Add( ComponentOfEditor );// Scene );
			return true;
		}

		public void TryNewObject( Vector2 mouse, Metadata.TypeInfo lockType )
		{
			if( !CanNewObject( out var parentsForNewObjects ) )
				return;

			var data = new NewObjectWindow.CreationDataClass();
			data.initDocumentWindow = (DocumentWindow)Owner;
			data.initParentObjects = new List<object>();
			data.initParentObjects.AddRange( parentsForNewObjects );

			//type specific
			data.beforeCreateObjectsFunction = delegate ( NewObjectWindow window, Metadata.TypeInfo selectedType )
			{
				if( window.creationData.initParentObjects.Count == 1 && window.creationData.initParentObjects[ 0 ] is Scene )
				{
					Component createTo = ComponentOfEditor;
					//Component createTo = GetObjectCreationSettings().destinationObject as Layer;
					//if( createTo == null )
					//	createTo = Scene;

					//MeshGeometry_Procedural
					if( MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( selectedType ) )
					{
						var meshInSpace = createTo.CreateComponent<MeshInSpace>( -1, false );

						//Name
						meshInSpace.Name = EditorUtility.GetUniqueFriendlyName( meshInSpace, selectedType.GetUserFriendlyNameForInstance() );

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
					if( MetadataManager.GetTypeOfNetType( typeof( CollisionShape ) ).IsAssignableFrom( selectedType ) )
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

					////RenderingEffect
					//if( MetadataManager.GetTypeOfNetType( typeof( RenderingEffect ) ).IsAssignableFrom( selectedType ) )
					//{
					//	var pipeline = Scene.RenderingPipeline.Value;
					//	if( pipeline != null )
					//	{
					//		var group = pipeline.GetComponent( "Scene Effects" );
					//		if( group != null )
					//		{
					//			var insertIndex = EditorUtility.GetNewObjectInsertIndex( group, selectedType );
					//			var effect = group.CreateComponent( selectedType, insertIndex, false );

					//			//Name
					//			var defaultName = effect.BaseType.GetUserFriendlyNameForInstance();
					//			if( group.GetComponent( defaultName ) == null )
					//				effect.Name = defaultName;
					//			else
					//				effect.Name = group.Components.GetUniqueName( defaultName, false, 2 );

					//			window.creationData.createdObjects = new List<object>();
					//			window.creationData.createdObjects.Add( effect );
					//			window.creationData.createdComponentsOnTopLevel.Add( effect );

					//			return true;
					//		}
					//	}
					//}
				}

				return true;
			};

			//set Transform
			data.additionActionAfterEnabled = delegate ( NewObjectWindow window )
			{
				foreach( var obj in data.createdComponentsOnTopLevel )
				{
					if( obj is ObjectInSpace objectInSpace )
					{
						var objectToTransform = ObjectInSpaceUtility.CalculateObjectToTransform( objectInSpace );
						if( objectToTransform == null )
							objectToTransform = objectInSpace;

						if( objectToTransform.Transform.Value.Position == Vector3.Zero )
							CalculateCreateObjectPosition( objectInSpace, objectToTransform, mouse );
					}
				}
			};

			data.initLockType = lockType;
			data.initSupportAutoCreateAndClose = lockType != null;

			EditorAPI2.OpenNewObjectWindow( data );
		}

		//public override bool TryDeleteObjects()
		//{
		//	if( !base.TryDeleteObjects() )
		//		return false;

		//	UpdateTransformToolObjects();

		//	return true;
		//}

		private void Scene_DocumentWindow_DragEnter( object sender, DragEventArgs e )
		{
			//if( /*CreateObjectsMode == CreateObjectsModeEnum.Drop &&*/ ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByDrop ) )
			{
				createByDropEntered = true;
				DragDropCreateObject_Create( e );
			}
		}

		private void Scene_DocumentWindow_DragOver( object sender, DragEventArgs e )
		{
			e.Effect = DragDropEffects.None;

			if( /*CreateObjectsMode == CreateObjectsModeEnum.Drop &&*/ /*( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByDrop ) && */ createByDropEntered )
			{
				( (EngineViewportControl)ViewportControl )?.PerformMouseMove();

				//для IMeshInSpaceChild потому как он создает при наведении
				//if( objectToCreate == null )
				//	DragDropCreateObject_Create( e );

				CreateObject_Update();
				if( objectToCreate != null )
					e.Effect = DragDropEffects.Link;

				CreateSetProperty_Update( e );
				if( createSetProperty != null )
					e.Effect = DragDropEffects.Link;

				( (EngineViewportControl)ViewportControl ).TryRender();
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
				( (EngineViewportControl)ViewportControl ).TryRender();
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
						//ObjectCreationModeSet( mode );
						objectTypeToCreate = null;
						objectToCreate = null;
						createByDropEntered = false;
						EditorAPI2.SelectDockWindow( Owner );
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

		//Metadata.TypeInfo CreateObjectWhatTypeWillCreated( Metadata.TypeInfo objectType, string referenceToObject )
		//{
		//	////add-ons support
		//	//{
		//	//	Metadata.TypeInfo type = null;
		//	//	SceneEditorUtility.PerformCreateObjectWhatTypeWillCreatedEvent( objectType, referenceToObject, ref type );
		//	//	//CreateObjectWhatTypeWillCreatedEvent?.Invoke( objectType, referenceToObject, ref type );
		//	//	if( type != null )
		//	//		return type;
		//	//}

		//	//MeshGeometry_Procedural
		//	if( MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
		//		return MetadataManager.GetTypeOfNetType( typeof( MeshInSpace ) );

		//	//Mesh
		//	if( MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( objectType ) )
		//		return MetadataManager.GetTypeOfNetType( typeof( MeshInSpace ) );

		//	//ParticleSystem
		//	if( MetadataManager.GetTypeOfNetType( typeof( ParticleSystem ) ).IsAssignableFrom( objectType ) )
		//		return MetadataManager.GetTypeOfNetType( typeof( ParticleSystemInSpace ) );

		//	//CollisionShape
		//	if( MetadataManager.GetTypeOfNetType( typeof( CollisionShape ) ).IsAssignableFrom( objectType ) /*&&
		//		objectType != MetadataManager.GetTypeOfNetType( typeof( CollisionShape_Mesh ) )*/ )
		//		return MetadataManager.GetTypeOfNetType( typeof( RigidBody ) );

		//	//CollisionShape2D
		//	if( MetadataManager.GetTypeOfNetType( typeof( CollisionShape2D ) ).IsAssignableFrom( objectType ) )
		//		return MetadataManager.GetTypeOfNetType( typeof( RigidBody2D ) );

		//	//Import3D
		//	if( typeof( Import3D ).IsAssignableFrom( objectType.GetNetType() ) )
		//	{
		//		var componentType = objectType as Metadata.ComponentTypeInfo;
		//		if( componentType != null && componentType.BasedOnObject != null )
		//		{
		//			//если один вложенный ObjectInSpace, то можно корневой не создавать. но тогда сбрасывается трансформ у вложенного.

		//			var mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Mesh;
		//			if( mesh != null )
		//				return MetadataManager.GetTypeOfNetType( typeof( MeshInSpace ) );
		//			else
		//			{
		//				//Scene mode
		//				var sceneObjects = componentType.BasedOnObject.GetComponent( "Scene Objects" ) as ObjectInSpace;
		//				if( sceneObjects != null && sceneObjects.GetProvidedType() != null )
		//					return sceneObjects.GetProvidedType();
		//			}
		//		}
		//	}

		//	//RenderingEffect
		//	if( MetadataManager.GetTypeOfNetType( typeof( RenderingEffect ) ).IsAssignableFrom( objectType ) )
		//	{
		//		var pipeline = Scene.RenderingPipeline.Value;
		//		if( pipeline != null )//&& pipeline.GetComponent( objectType, true ) == null )
		//		{
		//			var group = pipeline.GetComponent( "Scene Effects" );
		//			if( group != null )
		//				return objectType;
		//		}
		//	}

		//	//Sound
		//	if( MetadataManager.GetTypeOfNetType( typeof( Sound ) ).IsAssignableFrom( objectType ) )
		//		return MetadataManager.GetTypeOfNetType( typeof( SoundSource ) );

		//	//Component by default
		//	if( MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( objectType ) )
		//		return objectType;

		//	return null;
		//}

		//public delegate void CreateObjectByCreationDataEventDelegate( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject );
		//public static event CreateObjectByCreationDataEventDelegate CreateObjectByCreationDataEvent;

		Component CreateObjectByCreationData( Metadata.TypeInfo objectType, string referenceToObject, object anyData, string objectName )
		{
			Component createTo = ComponentOfEditor;
			//Component createTo = GetObjectCreationSettings().destinationObject as Layer;
			//if( createTo == null )
			//	createTo = Scene;

			//var overObject = GetMouseOverObjectForSelection();

			Component newObject = null;

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

			//!!!!
			////add-ons support
			//SceneEditorUtility.PerformCreateObjectByCreationDataEvent( objectType, referenceToObject, anyData, createTo, ref newObject );

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

			////RenderingEffect
			//if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( RenderingEffect ) ).IsAssignableFrom( objectType ) )
			//{
			//	var pipeline = Scene.RenderingPipeline.Value;
			//	if( pipeline != null )//&& pipeline.GetComponent( objectType, true ) == null )
			//	{
			//		var group = pipeline.GetComponent( "Scene Effects" );
			//		if( group != null )
			//		{
			//			var insertIndex = EditorUtility.GetNewObjectInsertIndex( group, objectType );
			//			var effect = group.CreateComponent( objectType, insertIndex, false );
			//			newObject = effect;
			//		}
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

		//bool CreateObjectFilterEqual( Metadata.TypeInfo objectType, string referenceToObject, ObjectInSpace obj )
		//{
		//	//MeshGeometry_Procedural
		//	if( MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
		//	{
		//		var meshInSpace = obj as MeshInSpace;
		//		if( meshInSpace != null )
		//		{
		//			var mesh = meshInSpace.GetComponent( "Mesh" ) as Mesh;
		//			if( mesh != null )
		//			{
		//				var meshGeometry = mesh.GetComponent( objectType );
		//				if( meshGeometry != null && meshGeometry.Name == "Mesh Geometry" )
		//					return true;
		//			}
		//		}
		//	}

		//	//Mesh
		//	if( MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( objectType ) )
		//	{
		//		var meshInSpace = obj as MeshInSpace;
		//		if( meshInSpace != null && meshInSpace.Mesh.GetByReference == referenceToObject )
		//			return true;
		//	}

		//	//ParticleSystem
		//	if( MetadataManager.GetTypeOfNetType( typeof( ParticleSystem ) ).IsAssignableFrom( objectType ) )
		//	{
		//		var particleInSpace = obj as ParticleSystemInSpace;
		//		if( particleInSpace != null && particleInSpace.ParticleSystem.GetByReference == referenceToObject )
		//			return true;
		//	}

		//	//CollisionShape
		//	if( MetadataManager.GetTypeOfNetType( typeof( CollisionShape ) ).IsAssignableFrom( objectType ) /* &&
		//		objectType != MetadataManager.GetTypeOfNetType( typeof( CollisionShape_Mesh ) )*/ )
		//	{
		//		var rigidBody = obj as RigidBody;
		//		if( rigidBody != null )
		//		{
		//			var shape = rigidBody.GetComponent( objectType );
		//			if( shape != null && shape.Name == "Collision Shape" )
		//				return true;
		//		}
		//	}

		//	//Import3D
		//	if( typeof( Import3D ).IsAssignableFrom( objectType.GetNetType() ) )
		//	{
		//		var meshInSpace = obj as MeshInSpace;
		//		if( meshInSpace != null )
		//		{
		//			var componentType = objectType as Metadata.ComponentTypeInfo;
		//			if( componentType != null && componentType.BasedOnObject != null )
		//			{
		//				var mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Mesh;
		//				if( mesh != null )
		//				{
		//					if( meshInSpace.Mesh.GetByReference == ReferenceUtility.CalculateResourceReference( mesh ) )
		//						return true;
		//				}
		//				else
		//				{
		//					////Scene mode
		//					//var sceneObjects = componentType.BasedOnObject.GetComponent( "Scene Objects" ) as ObjectInSpace;
		//					//if( sceneObjects != null && sceneObjects.GetProvidedType() != null )
		//					//{
		//					//	var objectInSpace = Scene.CreateComponent( sceneObjects.GetProvidedType(), -1, false );
		//					//	newObject = objectInSpace;
		//					//}
		//				}
		//			}
		//		}
		//	}

		//	//Sound
		//	if( MetadataManager.GetTypeOfNetType( typeof( Sound ) ).IsAssignableFrom( objectType ) )
		//	{
		//		var soundSource = obj as SoundSource;
		//		if( soundSource != null && soundSource.Sound.GetByReference == referenceToObject )
		//			return true;
		//	}

		//	//by class filtering
		//	if( string.IsNullOrEmpty( referenceToObject ) && objectType.IsAssignableFrom( obj.BaseType ) )
		//		return true;

		//	return false;
		//}

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

			var allowSnap2 = allowSnap && EditorAPI2.IsModifierKeyPressed( EKeys.Control );// ModifierKeys.HasFlag( Keys.Control );

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

				objectInSpace.NewObjectSetDefaultConfigurationUpdate();
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
		}

		void CreateObject_Commit()
		{
			if( objectToCreate != null )
			{
				var obj = objectToCreate;

				//add to undo with deletion
				var newObjects = new List<Component>();
				newObjects.Add( objectToCreate );
				var action = new UndoActionComponentCreateDelete( Document, newObjects, true );
				Document.UndoSystem.CommitAction( action );
				Document.Modified = true;

				objectTypeToCreate = null;
				objectToCreate = null;

				//select created object
				//if( CreateObjectsMode != CreateObjectsModeEnum.Click )
				EditorAPI2.SelectComponentsInMainObjectsWindow( Owner, new Component[] { obj } );

				EditorAPI2.SelectDockWindow( Owner );
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
				//if( CreateObjectsMode == CreateObjectsModeEnum.Click )
				//	contentBrowserItem = EditorAPI2.CreateObjectGetSelectedContentBrowserItem();
			}

			//var dragDropData = ContentBrowser.GetDroppingItemData( dragEventArgs.Data );
			//if( dragDropData != null )
			if( contentBrowserItem != null )
			{
				//var contentBrowserItem = dragDropData.Item;

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

						if( ComponentOfEditor/*Scene*/.ParentRoot == component.ParentRoot ) //if( Scene.ParentRoot == component.ParentRoot )
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
						var (type, reference) = storeItem.GetFileToDrop( true );// CreateObjectsMode == CreateObjectsModeEnum.Drop );

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
					if( dragDropDataSetReferenceData.document == Document )
					{
						var propertyTypeUnref = dragDropDataSetReferenceData.property.TypeUnreferenced;

						if( MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( propertyTypeUnref ) )
						//if( MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( propertyTypeUnref ) )
						{
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
						Document.UndoSystem.CommitAction( action );
						Document.Modified = true;
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

				EditorAPI2.SelectDockWindow( Owner );

				return true;
			}

			return false;
		}

		void UpdateTransformToolActiveState()
		{
			transformTool.Active = !createByDropEntered;// && !createByClickEntered && !createByBrushEntered && ObjectCreationMode == null;
		}

		public bool CanSnap( out List<ObjectInSpace> resultObjects )
		{
			resultObjects = new List<ObjectInSpace>();
			foreach( var toolObject in transformTool.Objects )
			{
				{
					var toolObject2 = toolObject as TransformToolObjectObjectInSpace;
					if( toolObject2 != null )
					{
						var objectToTransform = ObjectInSpaceUtility.CalculateObjectToTransform( toolObject2.SelectedObject );
						if( objectToTransform != null )
							resultObjects.Add( objectToTransform );
					}
				}

				//!!!!
				//{
				//	var toolObject2 = toolObject as TransformToolObjectSeatItem;
				//	if( toolObject2 != null )
				//	{
				//		var objectToTransform = toolObject2.SelectedObject;// ObjectInSpaceUtility.CalculateObjectToTransform( toolObject2.SelectedObject );
				//		if( objectToTransform != null )
				//			resultObjects.Add( objectToTransform );
				//	}
				//}
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
				Document.UndoSystem.CommitAction( undoAction );
				Document.Modified = true;
			}
		}

		static string Translate( string text )
		{
			return EditorLocalization.Translate( "CanvasBasedEditorWithObjectTransformTools", text );
		}

		protected override void OnGetTextInfoLeftTopCorner( List<string> lines )
		{
			base.OnGetTextInfoLeftTopCorner( lines );

			////statistics
			//if( Scene != null && Scene.GetDisplayDevelopmentDataInThisApplication() && Scene.DisplayTextInfo )
			//{
			//	var statistics = ViewportControl2?.Viewport?.RenderingContext?.UpdateStatisticsPrevious;
			//	if( statistics != null )
			//	{
			//		lines.Add( Translate( "FPS" ) + ": " + statistics.FPS.ToString( "F1" ) );
			//		lines.Add( Translate( "Triangles" ) + ": " + statistics.Triangles.ToString( "N0" ) );
			//	}
			//}
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

		protected override void OnGetTextInfoCenterBottomCorner( List<string> lines )
		{
			base.OnGetTextInfoCenterBottomCorner( lines );

			//if( createByClickEntered )
			//	lines.Add( Translate( "Creating objects by mouse clicking." ) );

			//warning component already exists
			if( createByDropEntered && objectToCreate != null )
			{
				try
				{
					var type = GetTypeWhereDefinedWhenCreatingShowWarningIfItAlreadyExistsAttribute( objectToCreate );
					if( type != null && ComponentOfEditor.GetComponents( type, checkChildren: true ).Length > 1 )
					{
						var name = TypeUtility.GetUserFriendlyNameForInstanceOfType( type );

						if( lines.Count != 0 )
							lines.Add( "" );
						lines.Add( $"Another {name} component already exists." );
					}
				}
				catch { }
			}
		}

		//public override bool Paste()
		//{
		//	if( !CanPaste( out var destinationParent ) )
		//		return false;

		//	var data = ClipboardManager.GetFromClipboard<ObjectCutCopyPasteData>();
		//	if( data != null )
		//	{
		//		var components = new List<Component>();
		//		foreach( var obj in data.objects )
		//		{
		//			var c = obj as Component;
		//			if( c != null )
		//				components.Add( c );
		//		}

		//		//create new objects

		//		var newObjects = new List<Component>();
		//		Vector3 addToPosition = Vector3.Zero;

		//		for( int n = 0; n < components.Count; n++ )
		//		{
		//			var c = components[ n ];

		//			var cloned = c.Clone();
		//			if( destinationParent.GetComponent( c.Name ) == null )
		//				cloned.Name = c.Name;
		//			else
		//				cloned.Name = destinationParent.Components.GetUniqueName( c.Name, true, 2 );
		//			destinationParent.AddComponent( cloned );

		//			//Transform
		//			if( destinationParent == Scene )
		//			{
		//				var objectInSpace = cloned as ObjectInSpace;
		//				if( objectInSpace != null )
		//				{
		//					var objectToTransform = ObjectInSpaceUtility.CalculateObjectToTransform( objectInSpace );
		//					if( objectToTransform == null )
		//						objectToTransform = objectInSpace;

		//					if( n == 0 )
		//					{
		//						CalculateCreateObjectPosition( objectInSpace, objectToTransform );
		//						addToPosition = objectToTransform.Transform.Value.Position - ( (ObjectInSpace)c ).Transform.Value.Position;
		//					}
		//					else
		//					{
		//						var old = objectInSpace.Transform.Value;
		//						objectToTransform.Transform = new Transform( old.Position + addToPosition, old.Rotation, old.Scale );
		//					}
		//				}
		//			}

		//			newObjects.Add( cloned );
		//		}

		//		if( data.cut )
		//		{
		//			//cut
		//			if( data.documentWindow.Document2 != Document )
		//			{
		//				//another document
		//				{
		//					var action = new UndoActionComponentCreateDelete( data.documentWindow.Document, components, false );
		//					data.documentWindow.Document2.UndoSystem.CommitAction( action );
		//					data.documentWindow.Document2.Modified = true;
		//				}
		//				{
		//					var action = new UndoActionComponentCreateDelete( Document, newObjects, true );
		//					Document.UndoSystem.CommitAction( action );
		//					Document.Modified = true;
		//				}
		//			}
		//			else
		//			{
		//				//same document
		//				var multiAction = new UndoMultiAction();
		//				multiAction.AddAction( new UndoActionComponentCreateDelete( Document, components, false ) );
		//				multiAction.AddAction( new UndoActionComponentCreateDelete( Document, newObjects, true ) );
		//				Document.UndoSystem.CommitAction( multiAction );
		//				Document.Modified = true;
		//			}
		//		}
		//		else
		//		{
		//			//copy
		//			var action = new UndoActionComponentCreateDelete( Document, newObjects, true );
		//			Document.UndoSystem.CommitAction( action );
		//			Document.Modified = true;
		//		}
		//	}

		//	return true;
		//}

		//public bool CanFocusCameraOnSelectedObject( out object[] objects )
		//{
		//	if( !AllowCameraControl )
		//	{
		//		objects = null;
		//		return false;
		//	}

		//	var result = new List<object>();

		//	foreach( var obj in SelectedObjects )
		//	{
		//		if( obj is ObjectInSpace )//|| obj is BuilderWorkareaMode.Vertex || obj is BuilderWorkareaMode.Edge || obj is BuilderWorkareaMode.Face )
		//			result.Add( obj );
		//	}

		//	objects = result.ToArray();
		//	//objects = SelectedObjects.OfType<ObjectInSpace>().ToArray();
		//	return objects.Length != 0;
		//}

		//		public void FocusCameraOnSelectedObject( object[] objects )
		//		{
		//#if !DEPLOY
		//			Bounds bounds = NeoAxis.Bounds.Cleared;
		//			foreach( var obj in objects )
		//			{
		//				if( obj is Light light )
		//				{
		//					var b = new Bounds( light.Transform.Value.Position );
		//					b.Expand( 0.1 );
		//					bounds.Add( b );
		//				}
		//				else if( obj is ObjectInSpace objectInSpace )
		//					bounds.Add( objectInSpace.SpaceBounds.BoundingBox );
		//				//else if( obj is BuilderWorkareaMode.Vertex vertex )
		//				//{
		//				//	var b = new Bounds( vertex.Position );
		//				//	b.Expand( 0.1 );
		//				//	bounds.Add( b );
		//				//}
		//				//else if( obj is BuilderWorkareaMode.Edge edge )
		//				//{
		//				//	var b = new Bounds( edge.Position );
		//				//	b.Expand( 0.5 );
		//				//	bounds.Add( b );
		//				//}
		//				//else if( obj is BuilderWorkareaMode.Face face )
		//				//{
		//				//	var b = new Bounds( face.Position );
		//				//	b.Expand( 0.5 );
		//				//	bounds.Add( b );
		//				//}
		//			}

		//			Camera camera = Scene.Mode.Value == Scene.ModeEnum._3D ? Scene.CameraEditor : Scene.CameraEditor2D;
		//			if( !bounds.IsCleared() && camera != null )
		//			{
		//				var needRectangle = new Rectangle( .4f, .3f, .6f, .7f );

		//				var lookTo = bounds.GetCenter();
		//				var points = bounds.ToPoints();

		//				if( Scene.Mode.Value == Scene.ModeEnum._3D )
		//				{
		//					double distance = 1000;
		//					while( distance > 0.3 )
		//					{
		//						camera.SetPosition( lookTo - camera.TransformV.Rotation.GetForward() * distance );

		//						var viewport = ViewportControl.Viewport;

		//						bool processed = false;
		//						Scene_ViewportUpdateGetCameraSettings( Scene, viewport, ref processed );

		//						foreach( var point in points )
		//						{
		//							viewport.CameraSettings.ProjectToScreenCoordinates( point, out var screenPos );
		//							if( !needRectangle.Contains( screenPos ) )
		//								goto end;
		//						}

		//						distance /= 1.03f;
		//					}
		//end:;
		//				}
		//				else
		//				{
		//					camera.SetPosition( new Vector3( lookTo.ToVector2(), camera.TransformV.Position.Z ) );
		//				}
		//			}
		//#endif
		//		}

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
			//if( WorkareaMode != null && WorkareaMode.PerformTransformToolCreateObject( forObject, ref transformToolObject ) )
			//	handled = true;

			if( !handled )
			{
				if( transformToolObject == null && ComponentOfEditor != forObject )
				{
					//ObjectInSpace
					{
						var objectInSpace = forObject as ObjectInSpace;
						if( objectInSpace != null )
							return new TransformToolObjectObjectInSpace( objectInSpace );
					}

					//SeatItem specific
					{
						var seatItem = forObject as SeatItem;
						if( seatItem != null )
							return new TransformToolObjectSeatItem( seatItem );
					}

					//Vehicle specific
					{
						var vehicleTypeWheel = forObject as VehicleTypeWheel;
						if( vehicleTypeWheel != null )
							return new TransformToolObjectVehicleTypeWheel( vehicleTypeWheel );
					}
				}
			}

			return transformToolObject;
		}

		//[Browsable( false )]
		//public new SceneEditorWorkareaMode WorkareaMode
		//{
		//	get { return (SceneEditorWorkareaMode)base.WorkareaMode; }
		//}

		//public void ResetWorkareaMode()
		//{
		//	WorkareaModeSet( "" );
		//	transformTool.Mode = transformToolModeRestore;
		//}

		//public void ChangeCreateObjectsMode( CreateObjectsModeEnum mode )
		//{
		//	//reset terrain workarea mode
		//	if( WorkareaMode as TerrainEditingMode != null )
		//		ResetWorkareaMode();

		//	if( CreateObjectsMode == mode )
		//		return;

		//	CreateObjectsMode = mode;

		//}

		//bool CreateByClickCreate()
		//{
		//	(var objectType, var referenceToObject, var anyData, var objectName) = EditorAPI2.GetSelectedObjectToCreate();
		//	if( objectType != null )//|| memberFullSignature != "" || createNodeWithComponent != null )
		//	{
		//		var typeWillCreated = CreateObjectWhatTypeWillCreated( objectType, referenceToObject );
		//		if( typeWillCreated != null && MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( typeWillCreated ) )
		//		{
		//			var newObject = CreateObjectByCreationData( objectType, referenceToObject, anyData, objectName );
		//			if( newObject != null )
		//			{
		//				objectTypeToCreate = objectType;
		//				objectToCreate = newObject;
		//				CreateObject_Update();
		//				return true;
		//			}
		//		}
		//	}

		//	return false;
		//}

		//private void ViewportControl_MouseEnter( object sender, EventArgs e )
		//{
		//	if( CreateObjectsMode == CreateObjectsModeEnum.Click && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick ) && ObjectCreationMode == null )
		//	{
		//		if( CreateByClickCreate() )
		//			createByClickEntered = true;
		//	}
		//}

		//void CreateByClickCancel()
		//{
		//	CreateObject_Destroy();
		//	CreateSetProperty_Cancel();
		//	createByClickEntered = false;
		//}

		//private void ViewportControl_MouseLeave( object sender, EventArgs e )
		//{
		//	if( CreateObjectsMode == CreateObjectsModeEnum.Click )
		//		CreateByClickCancel();
		//}

		//public override void WorkareaModeSet( string name, DocumentWindowWithViewportWorkareaMode instance = null )
		//{
		//	base.WorkareaModeSet( name, instance );

		//	CreateByClickCancel();

		//	if( new Rectangle( 0, 0, 1, 1 ).Contains( Viewport.MousePosition ) )
		//	{
		//		if( CreateObjectsMode == CreateObjectsModeEnum.Click && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick ) && ObjectCreationMode == null )
		//		{
		//			if( CreateByClickCreate() )
		//				createByClickEntered = true;
		//		}
		//	}
		//}

		//public override void ObjectCreationModeSet( ObjectCreationMode mode )
		//{
		//	if( mode == null )
		//	{
		//		//continue modes
		//		if( new Rectangle( 0, 0, 1, 1 ).Contains( Viewport.MousePosition ) )
		//		{
		//			if( CreateObjectsMode == CreateObjectsModeEnum.Click && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick ) )
		//			{
		//				if( CreateByClickCreate() )
		//					createByClickEntered = true;
		//			}
		//			if( CreateObjectsMode == CreateObjectsModeEnum.Brush && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByBrush ) )
		//			{
		//				if( CreateByBrushCreate() )
		//					createByBrushEntered = true;
		//			}
		//		}
		//	}

		//	base.ObjectCreationModeSet( mode );
		//}

		//protected virtual object[] SelectByDoubleClick( object overObject )
		//{
		//	var result = new ESet<object>();

		//	var radius = ProjectSettings.Get.SceneEditor.SceneEditorSelectByDoubleClickRadius.Value;

		//	var overObject2 = overObject as ObjectInSpace;
		//	if( overObject2 != null && radius > 0 )
		//	{
		//		var sphere = new Sphere( overObject2.TransformV.Position, radius );
		//		var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );
		//		Scene.GetObjectsInSpace( getObjectsItem );

		//		foreach( var item in getObjectsItem.Result )
		//		{
		//			var obj = item.Object;
		//			if( obj.VisibleInHierarchy && obj.CanBeSelectedInHierarchy && ( obj.TransformV.Position - overObject2.TransformV.Position ).Length() <= radius )
		//			{
		//				if( obj.BaseType == overObject2.BaseType )
		//				{
		//					//MeshInSpace specific
		//					var meshInSpace1 = obj as MeshInSpace;
		//					var meshInSpace2 = overObject2 as MeshInSpace;
		//					if( meshInSpace1 != null && meshInSpace2 != null )
		//					{
		//						if( meshInSpace1.Mesh.Value != meshInSpace2.Mesh.Value )
		//							continue;
		//					}

		//					result.AddWithCheckAlreadyContained( obj );
		//				}
		//			}
		//		}
		//	}

		//	return result.ToArray();
		//}

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

				//!!!!WinForms
				screenLabelToolTip.Hide( (Control)ViewportControl );
				screenLabelToolTip.SetToolTip( (Control)ViewportControl, screenLabelToolTipText );
			}
		}

		//(CreateObjectsModeEnum createObjectsMode, CreateObjectsDestinationModeEnum destinationMode, Component destinationObject) GetObjectCreationSettings()
		//{
		//	//disable disposed selected object
		//	if( createObjectsDestinationSelected.Obj != null )
		//	{
		//		if( createObjectsDestinationSelected.Obj.Disposed || createObjectsDestinationSelected.Obj.ParentRoot != Scene )
		//		{
		//			if( CreateObjectsMode == CreateObjectsModeEnum.Drop || CreateObjectsMode == CreateObjectsModeEnum.Click )
		//				createObjectsDestinationSelected = (CreateObjectsDestinationModeEnum.SeparateObjectsToRoot, null);
		//			else
		//				createObjectsDestinationSelected = (CreateObjectsDestinationModeEnum.Auto, null);
		//		}
		//	}


		//	(CreateObjectsModeEnum createObjectsMode, CreateObjectsDestinationModeEnum destinationMode, Component destinationObject) result = (CreateObjectsMode, createObjectsDestinationSelected.Mode, createObjectsDestinationSelected.Obj);

		//	//set destinationObject to Scene
		//	if( result.createObjectsMode == CreateObjectsModeEnum.Drop || result.createObjectsMode == CreateObjectsModeEnum.Click )
		//	{
		//		if( result.destinationMode == CreateObjectsDestinationModeEnum.SeparateObjectsToRoot )
		//			result.destinationObject = Scene;
		//	}

		//	//get actual mode for Auto mode
		//	if( result.createObjectsMode == CreateObjectsModeEnum.Brush && result.destinationMode == CreateObjectsDestinationModeEnum.Auto )
		//	{
		//		//create to group of objects mode

		//		GroupOfObjects groupOfObjects = null;
		//		{
		//			foreach( var obj in Scene.GetComponents( checkChildren: true, onlyEnabledInHierarchy: true ) )
		//			{
		//				if( obj is GroupOfObjects obj2 && obj2.EditorAllowUsePaintBrush && obj.DisplayInEditor )
		//				{
		//					groupOfObjects = obj2;
		//					break;
		//				}
		//			}
		//		}

		//		if( groupOfObjects != null )
		//		{
		//			(var objectType, var referenceToObject, var anyData, string objectName) = GetSelectedObjectToCreateWithCheckSelectedElementOfGroupOfObjects();
		//			if( objectType != null )
		//			{
		//				//mesh
		//				{
		//					Mesh mesh = null;
		//					Metadata.TypeInfo meshGeometryProcedural = null;
		//					{
		//						var componentType = objectType as Metadata.ComponentTypeInfo;
		//						if( componentType != null && componentType.BasedOnObject != null )
		//						{
		//							//Mesh
		//							mesh = componentType.BasedOnObject as Mesh;

		//							//Import3D
		//							if( componentType.BasedOnObject is Import3D )
		//								mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Mesh;
		//						}

		//						//for Stores Window. the window provides reference to mesh because it can still not downloaded
		//						if( mesh == null && MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( objectType ) )
		//						{
		//							var componentTypeInfo = MetadataManager.GetType( referenceToObject ) as Metadata.ComponentTypeInfo;
		//							if( componentTypeInfo != null )
		//								mesh = componentTypeInfo.BasedOnObject as Mesh;
		//						}

		//						//MeshGeometry_Procedural
		//						if( MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
		//							meshGeometryProcedural = objectType;
		//					}

		//					if( mesh != null || meshGeometryProcedural != null )
		//						return (CreateObjectsModeEnum.Brush, CreateObjectsDestinationModeEnum.ToGroupOfObjects, groupOfObjects);
		//				}

		//				//surface
		//				{
		//					Surface surface = null;
		//					{
		//						var componentType = objectType as Metadata.ComponentTypeInfo;
		//						if( componentType != null && componentType.BasedOnObject != null )
		//							surface = componentType.BasedOnObject as Surface;
		//					}

		//					if( surface != null )
		//						return (CreateObjectsModeEnum.Brush, CreateObjectsDestinationModeEnum.ToGroupOfObjects, groupOfObjects);
		//				}
		//			}
		//		}

		//		return (CreateObjectsModeEnum.Brush, CreateObjectsDestinationModeEnum.SeparateObjectsToRoot, Scene);
		//	}

		//	return result;
		//}

	}
}
