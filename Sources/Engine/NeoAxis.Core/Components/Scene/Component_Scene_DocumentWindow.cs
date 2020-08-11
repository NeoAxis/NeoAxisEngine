// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Drawing.Design;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.Linq;
using ComponentFactory.Krypton.Toolkit;
using NeoAxis.Widget;
using System.Drawing;

namespace NeoAxis.Editor
{
	public partial class Component_Scene_DocumentWindow : DocumentWindowWithViewport
	{
		[EngineConfig( "Scene Editor", "CreateObjectsMode" )]
		static CreateObjectsModeEnum CreateObjectsMode = CreateObjectsModeEnum.Drop;
		[EngineConfig( "Scene Editor", "CreateObjectsBrushRadius" )]
		public static double CreateObjectsBrushRadius = 1;//5;
		[EngineConfig( "Scene Editor", "CreateObjectsBrushStrength" )]
		public static double CreateObjectsBrushStrength = 0.5;
		[EngineConfig( "Scene Editor", "CreateObjectsBrushHardness" )]
		public static double CreateObjectsBrushHardness = 0.5;

		[EngineConfig( "Terrain Editor", "TerrainToolShape" )]
		internal static Component_Terrain_EditingMode.ToolShape TerrainToolShape = Component_Terrain_EditingMode.ToolShape.Circle;
		[EngineConfig( "Terrain Editor", "TerrainToolRadius" )]
		internal static double TerrainToolRadius = 3;//5;
		[EngineConfig( "Terrain Editor", "TerrainToolStrength" )]
		internal static double TerrainToolStrength = 0.5;
		[EngineConfig( "Terrain Editor", "TerrainToolHardness" )]
		internal static double TerrainToolHardness = 0.5;


		//Select by rectangle
		bool selectByRectangle_Enabled;
		bool selectByRectangle_Activated;
		Vector2 selectByRectangle_StartPosition;
		Vector2 selectByRectangle_LastMousePosition;

		//Transform tool
		TransformTool transformTool;
		bool transformToolModifyCloned;
		bool transformToolNeedCallOnMouseMove;
		Vector2 transformToolNeedCallOnMouseMovePosition;
		TransformTool.ModeEnum transformToolModeRestore = TransformTool.ModeEnum.Position;

		//createObjectsDestination
		List<(Component Obj, string Text)> createObjectsDestinationCachedList = new List<(Component, string)>();
		double createObjectsDestinationLastUpdateTime;
		Component createObjectsDestinationSelected;

		//terrainPaintLayers
		List<(Component_PaintLayer Obj, string Text)> terrainPaintLayersCachedList = new List<(Component_PaintLayer, string)>();
		double terrainPaintLayersLastUpdateTime;
		Component_PaintLayer terrainPaintLayersSelected;

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

		bool createByBrushPainting;
		bool createByBrushPaintingDeleting;
		float createByBrushPaintingTimer;
		Component_GroupOfObjects createByBrushGroupOfObjects;
		List<int> createByBrushGroupOfObjectsObjectsCreated = new List<int>();
		List<Component_GroupOfObjects.Object> createByBrushGroupOfObjectsObjectsDeleted = new List<Component_GroupOfObjects.Object>();
		List<Component> createByBrushComponentsCreated = new List<Component>();
		List<(Component obj, bool wasEnabled)> createByBrushComponentsToDelete = new List<(Component obj, bool wasEnabled)>();

		bool skipSelectionByButtonInMouseUp;

		/////////////////////////////////////

		public enum CreateObjectsModeEnum
		{
			Drop,
			Click,
			Brush
		}

		/////////////////////////////////////////

		public abstract class WorkareaModeClass_Scene : WorkareaModeClass
		{
			protected WorkareaModeClass_Scene( Component_Scene_DocumentWindow documentWindow )
				: base( documentWindow )
			{
			}

			public new Component_Scene_DocumentWindow DocumentWindow
			{
				get { return (Component_Scene_DocumentWindow)base.DocumentWindow; }
			}

			protected virtual bool OnGetObjectsToSelectByRectangle( Rectangle rectangle, ref List<object> objectsToSelect ) { return false; }
			public delegate void GetObjectsToSelectByRectangleDelegate( WorkareaModeClass sender, Rectangle rectangle, ref bool handled, ref List<object> objectsToSelect );
			public event GetObjectsToSelectByRectangleDelegate GetObjectsToSelectByRectangle;
			internal bool PerformGetObjectsToSelectByRectangle( Rectangle rectangle, ref List<object> objectsToSelect )
			{
				var handled = OnGetObjectsToSelectByRectangle( rectangle, ref objectsToSelect );
				if( !handled )
					GetObjectsToSelectByRectangle?.Invoke( this, rectangle, ref handled, ref objectsToSelect );
				return handled;
			}

			protected virtual bool OnGetMouseOverObjectToSelectByClick( GetMouseOverObjectToSelectByClickContext context ) { return false; }
			public delegate void GetMouseOverObjectToSelectByClickDelegate( WorkareaModeClass sender, GetMouseOverObjectToSelectByClickContext context );
			public event GetMouseOverObjectToSelectByClickDelegate GetMouseOverObjectToSelectByClick;
			internal bool PerformGetMouseOverObjectToSelectByClick( GetMouseOverObjectToSelectByClickContext context )
			{
				var handled = OnGetMouseOverObjectToSelectByClick( context );
				if( !handled )
					GetMouseOverObjectToSelectByClick?.Invoke( this, context );
				return handled;
			}

			protected virtual bool OnTransformToolCreateObject( object forObject, ref TransformToolObject transformToolObject ) { return false; }
			public delegate void TransformToolCreateObjectDelegate( WorkareaModeClass sender, object forObject, ref bool handled, ref TransformToolObject transformToolObject );
			public event TransformToolCreateObjectDelegate TransformToolCreateObject;
			internal bool PerformTransformToolCreateObject( object forObject, ref TransformToolObject transformToolObject )
			{
				var handled = OnTransformToolCreateObject( forObject, ref transformToolObject );
				if( !handled )
					TransformToolCreateObject?.Invoke( this, forObject, ref handled, ref transformToolObject );
				return handled;
			}

			protected virtual bool OnTransformToolModifyBegin() { return false; }
			public delegate void TransformToolModifyBeginDelegate( WorkareaModeClass sender, ref bool handled );
			public event TransformToolModifyBeginDelegate TransformToolModifyBegin;
			internal bool PerformTransformToolModifyBegin()
			{
				var handled = OnTransformToolModifyBegin();
				if( !handled )
					TransformToolModifyBegin?.Invoke( this, ref handled );
				return handled;
			}

			protected virtual bool OnTransformToolModifyCommit() { return false; }
			public delegate void TransformToolModifyCommitDelegate( WorkareaModeClass sender, ref bool handled );
			public event TransformToolModifyCommitDelegate TransformToolModifyCommit;
			internal bool PerformTransformToolModifyCommit()
			{
				var handled = OnTransformToolModifyCommit();
				if( !handled )
					TransformToolModifyCommit?.Invoke( this, ref handled );
				return handled;
			}

			protected virtual bool OnTransformToolModifyCancel() { return false; }
			public delegate void TransformToolModifyCancelDelegate( WorkareaModeClass sender, ref bool handled );
			public event TransformToolModifyCancelDelegate TransformToolModifyCancel;
			internal bool PerformTransformToolModifyCancel()
			{
				var handled = OnTransformToolModifyCancel();
				if( !handled )
					TransformToolModifyCancel?.Invoke( this, ref handled );
				return handled;
			}

			protected virtual bool OnTransformToolCloneAndSelectObjects() { return false; }
			public delegate void TransformToolCloneAndSelectObjectsDelegate( WorkareaModeClass sender, ref bool handled );
			public event TransformToolCloneAndSelectObjectsDelegate TransformToolCloneAndSelectObjects;
			internal bool PerformTransformToolCloneAndSelectObjects()
			{
				var handled = OnTransformToolCloneAndSelectObjects();
				if( !handled )
					TransformToolCloneAndSelectObjects?.Invoke( this, ref handled );
				return handled;
			}
		}

		/////////////////////////////////////////

		public class GetMouseOverObjectToSelectByClickContext
		{
			public bool CheckOnlyObjectsWithEnabledSelectionByCursorFlag = true;

			public object ResultObject;//public Component_ObjectInSpace ResultObject;
			public Vector3? ResultPosition;
		}

		/////////////////////////////////////////

		static Component_Scene_DocumentWindow()
		{
			EngineConfig.RegisterClassParameters( typeof( Component_Scene_DocumentWindow ) );
		}

		public Component_Scene_DocumentWindow()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			var scene = ObjectOfWindow as Component_Scene;
			if( scene != null )
			{
				Scene = scene;
				SceneNeedDispose = false;

				scene.ViewportUpdateGetCameraSettings += Scene_ViewportUpdateGetCameraSettings;
			}
			else
				Log.Fatal( "scene == null" );

			transformTool = new TransformTool( ViewportControl );
			transformTool.Mode = TransformTool.ModeEnum.Position;

			transformTool.ModifyBegin += TransformToolModifyBegin;
			transformTool.ModifyCommit += TransformToolModifyCommit;
			transformTool.ModifyCancel += TransformToolModifyCancel;
			transformTool.CloneAndSelectObjects += TransformToolCloneAndSelectObjects;

			ViewportControl.MouseEnter += ViewportControl_MouseEnter;
			ViewportControl.MouseLeave += ViewportControl_MouseLeave;

			if( ObjectOfWindow != null )
				SelectObjects( new object[] { ObjectOfWindow } );
		}

		protected override void OnDestroy()
		{
			if( transformTool != null )
			{
				transformTool.ModifyBegin -= TransformToolModifyBegin;
				transformTool.ModifyCommit -= TransformToolModifyCommit;
				transformTool.ModifyCancel -= TransformToolModifyCancel;
				transformTool.CloneAndSelectObjects -= TransformToolCloneAndSelectObjects;
			}

			base.OnDestroy();
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );

			////connect scene to viewport
			//ViewportControl.Viewport.AttachedScene = Scene;
		}

		protected override void Viewport_KeyDown( Viewport viewport, KeyEvent e, ref bool handled )
		{
			base.Viewport_KeyDown( viewport, e, ref handled );
			if( handled )
				return;

			//Inside only Espace key is processed.
			transformTool.PerformKeyDown( e, ref handled );
			if( handled )
				return;

			//create by brush cancel by Escape
			if( e.Key == EKeys.Escape )
			{
				if( createByBrushPainting )
				{
					CreateByBrushPaintingEnd( true );
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
						CreateByClick_Cancel();
					if( CreateObjectsMode == CreateObjectsModeEnum.Brush )
						CreateByBrush_Cancel();
					EditorAPI.ResetSelectedObjectToCreate();
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

		public bool StartObjectCreationMode( Metadata.TypeInfo type, Component obj )
		{
			var attributes = type/*objectTypeToCreate*/.GetNetType()/* objectToCreate.GetType()*/.GetCustomAttributes<ObjectCreationModeAttribute>().ToArray();
			if( attributes.Length != 0 )
			{
				//begin creation mode
				var mode = (ObjectCreationMode)attributes[ 0 ].CreationModeClass.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { this, obj/*objectToCreate*/ } );
				ObjectCreationModeSet( mode );
				objectTypeToCreate = null;
				objectToCreate = null;
				createByClickEntered = false;
				return true;
			}

			return false;
		}

		protected override void Viewport_MouseDown( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			base.Viewport_MouseDown( viewport, button, ref handled );
			if( handled )
				return;

			//transform tool
			transformTool.PerformMouseDown( button, ref handled );
			if( handled )
				return;

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
						var mode = (ObjectCreationMode)attributes[ 0 ].CreationModeClass.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { this, objectToCreate } );
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
					if( CreateByClick_Create() )
						createByClickEntered = true;
				}

				handled = true;
				return;
			}

			//creating objects painting
			if( button == EMouseButtons.Left )
			{
				if( CreateByBrushPaintingBegin( viewport ) )
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
		}

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

			//!!!!что еще проверять?
			if( AllowSelectObjects && objectToCreate == null && !createByDropEntered && !createByClickEntered && !createByBrushEntered && !skipSelectionByButtonInMouseUp )
			{
				var selectedObjects = new ESet<object>( SelectedObjectsSet );

				//select by rectangle
				if( button == EMouseButtons.Left && selectByRectangle_Enabled )
				{
					//reset selected objects even mode is not activated.
					bool shiftPressed = ( Form.ModifierKeys & Keys.Shift ) != 0;
					if( !shiftPressed )
						selectedObjects.Clear();

					if( selectByRectangle_Activated )
					{
						foreach( var obj in SelectByRectangle_GetObjectsInSpace() )
							selectedObjects.AddWithCheckAlreadyContained( obj );
					}

					if( selectByRectangle_Activated )
						handled = true;

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
			//creating objects painting
			if( button == EMouseButtons.Left )
			{
				if( CreateByBrushPaintingEnd( false ) )
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

			CreateByBrushPaintingTick( viewport, delta );

			//if( firstTick )
			//{
			//	firstTick = false;
			//	//!!!!!temp
			//	//{
			//	//	var selectedObjects = new ESet<object>();
			//	//	foreach( var node in GetObjectInSpaceByRectangle( SelectByRectangle_GetRectangle() ) )
			//	//	{
			//	//		if( selectedObjects.Count != 0 )
			//	//			break;
			//	//		selectedObjects.AddWithCheckAlreadyContained( node );
			//	//	}
			//	//	SettingsWindow.Instance.SelectObjects( this, selectedObjects.ToArray() );
			//	//}

			//}
		}

		public void GetMouseOverObjectInSpaceToSelectByClick( GetMouseOverObjectToSelectByClickContext context )
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
				var obj = item.ObjectInSpace;
				if( !context.CheckOnlyObjectsWithEnabledSelectionByCursorFlag || obj.EnabledSelectionByCursor )// obj.EnabledInHierarchy && obj.VisibleInHierarchy && obj.CanBeSelected )
				{
					if( MathAlgorithms.CheckPointInsideEllipse( item.ScreenRectangle, mouse ) )
					{
						context.ResultObject = obj;
						return;
					}
				}
			}

			//get by ray
			Component_ObjectInSpace.CheckSelectionByRayContext rayContext = new Component_ObjectInSpace.CheckSelectionByRayContext();
			rayContext.viewport = viewport;
			rayContext.screenPosition = mouse;
			rayContext.ray = viewport.CameraSettings.GetRayByScreenCoordinates( rayContext.screenPosition );

			var request = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, rayContext.ray );
			Scene.GetObjectsInSpace( request );
			var list = request.Result;

			Component_ObjectInSpace foundObject = null;
			double foundScale = 0;

			//!!!!
			double scaleEpsilon = rayContext.ray.Direction.Length() / 100;

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
							if( Math.Abs( foundScale - scale ) < scaleEpsilon )
							{
								var first = foundObject;
								var second = obj;
								bool secondIsParentOfFirst = first.GetAllParents( false ).Contains( second );
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

		public virtual void GetMouseOverObjectToSelectByClick( GetMouseOverObjectToSelectByClickContext context )
		{
			var viewport = ViewportControl.Viewport;
			var mouse = viewport.MousePosition;

			if( !new Rectangle( 0, 0, 1, 1 ).Contains( mouse ) )
				return;
			if( viewport.MouseRelativeMode )
				return;

			if( WorkareaMode != null && WorkareaMode.PerformGetMouseOverObjectToSelectByClick( context ) )
				return;
			GetMouseOverObjectInSpaceToSelectByClick( context );
		}

		public object GetMouseOverObjectToSelectByClick()
		{
			var context = new GetMouseOverObjectToSelectByClickContext();
			GetMouseOverObjectToSelectByClick( context );
			return context.ResultObject;
		}

		protected override void Viewport_UpdateGetObjectInSceneRenderingContext( Viewport viewport, ref Component_ObjectInSpace.RenderingContext context )
		{
			base.Viewport_UpdateGetObjectInSceneRenderingContext( viewport, ref context );

			//prepare context

			context = new Component_ObjectInSpace.RenderingContext( viewport );

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
							context.canSelectObjects.Add( obj );
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
								context.canSelectObjects.Add( obj );
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
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			var renderer = viewport.CanvasRenderer;

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

			transformTool.PerformRender();

			CreateByBrushRender( viewport );
		}

		protected override void Viewport_UpdateBeforeOutput2( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput2( viewport );

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
			var allSet = new ESet<Component_ObjectInSpace>();

			//get by screen label
			foreach( var item in ViewportControl.Viewport.LastFrameScreenLabels.GetReverse() )
			{
				var obj = item.ObjectInSpace;
				//if( obj.EnabledInHierarchy && obj.VisibleInHierarchy && obj.CanBeSelected )
				if( obj.EnabledSelectionByCursor )
				{
					if( rectangle.Contains( item.ScreenRectangle.GetCenter() ) )
						allSet.AddWithCheckAlreadyContained( obj );
				}
			}

			foreach( var c in Scene.GetComponents( false, true, true ) )
			{
				var obj = c as Component_ObjectInSpace;
				if( obj != null && obj.EnabledSelectionByCursor )//obj.EnabledInHierarchy && obj.VisibleInHierarchy && obj.CanBeSelected )
				{
					var transform = obj.Transform.Value;
					var pos = transform.Position;

					if( ViewportControl.Viewport.CameraSettings.ProjectToScreenCoordinates( pos, out Vector2 screenPosition ) )
					{
						if( rectangle.Contains( screenPosition ) )
							allSet.AddWithCheckAlreadyContained( obj );
					}
				}
			}

			//if( skipChildrenOfResultObjects )
			//{
			//ESet<Component_ObjectInSpace> allSet = new ESet<Component_ObjectInSpace>( all );

			var result = new List<object>();
			foreach( var obj in allSet )
			{
				bool skip = false;
				foreach( var parent in obj.GetAllParents( true ) )
				{
					var parent2 = parent as Component_ObjectInSpace;
					if( parent2 != null && allSet.Contains( parent2 ) )
					{
						skip = true;
						break;
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
				var objectInSpace = currentObject as Component_ObjectInSpace;
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

		protected virtual void TransformToolModifyBegin()
		{
			if( WorkareaMode != null && WorkareaMode.PerformTransformToolModifyBegin() )
				return;
		}

		protected virtual void TransformToolModifyCommit()
		{
			if( WorkareaMode != null && WorkareaMode.PerformTransformToolModifyCommit() )
				return;

			if( !transformToolModifyCloned )
			{
				//add changes to undo list

				var undoItems = new List<UndoActionPropertiesChange.Item>();

				foreach( var toolObject in transformTool.Objects )
				{
					var toolObject2 = toolObject as TransformToolObject_ObjectInSpace;
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
					Document.UndoSystem.CommitAction( action );
					Document.Modified = true;
				}
			}
			else
			{
				var newObjects = new List<Component>();
				foreach( var toolObject in transformTool.Objects )
				{
					var toolObject2 = toolObject as TransformToolObject_ObjectInSpace;
					if( toolObject2 != null )
						newObjects.Add( toolObject2.SelectedObject );
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

		protected virtual void TransformToolModifyCancel()
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
					var toolObject2 = toolObject as TransformToolObject_ObjectInSpace;
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

			//!!!!что с вложенными или еще какими-то особыми случаями

			ContentBrowserUtility.AllContentBrowsers_SuspendChildrenChangedEvent();

			var newObjects = new List<object>();
			foreach( var toolObject in transformTool.Objects )
			{
				var toolObject2 = toolObject as TransformToolObject_ObjectInSpace;
				if( toolObject2 != null )
				{
					var obj = toolObject2.SelectedObject;

					var newObject = EditorUtility.CloneComponent( obj );
					newObjects.Add( newObject );

					//newObject.Editor_BeginTransformModifying();
				}
			}

			Scene.HierarchyController?.ProcessDelayedOperations();
			ContentBrowserUtility.AllContentBrowsers_ResumeChildrenChangedEvent();

			//select objects
			SelectObjects( newObjects, updateSettingsWindowSelectObjects: false );

			//add screen message
			EditorUtility.ShowScreenNotificationObjectsCloned( newObjects.Count );

			transformToolModifyCloned = true;

			UpdateTransformToolObjects();
		}

		public override void EditorActionGetState( EditorAction.GetStateContext context )
		{
			base.EditorActionGetState( context );

			switch( context.Action.Name )
			{

			case "Select":
				context.Enabled = true;
				context.Checked = transformTool != null && transformTool.Mode == TransformTool.ModeEnum.None;
				break;

			case "Move":
				context.Enabled = true;
				context.Checked = transformTool != null && transformTool.Mode == TransformTool.ModeEnum.Position;
				break;

			case "Rotate":
				context.Enabled = true;
				context.Checked = transformTool != null && transformTool.Mode == TransformTool.ModeEnum.Rotation;
				break;

			case "Scale":
				context.Enabled = true;
				context.Checked = transformTool != null && transformTool.Mode == TransformTool.ModeEnum.Scale;
				break;

			case "Transform Using Local Coordinates":
				context.Enabled = true;
				context.Checked = transformTool != null && transformTool.CoordinateSystemMode == TransformTool.CoordinateSystemModeEnum.Local;
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
						context.Action.ListBox.Items = items;

						//update selected item
						var selectIndex = createObjectsDestinationCachedList.FindIndex( a => a.Obj == createObjectsDestinationSelected );
						if( selectIndex == -1 )
							selectIndex = 0;
						context.Action.ListBox.SelectIndex = selectIndex;
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
				context.Checked = TerrainToolShape == Component_Terrain_EditingMode.ToolShape.Circle;
				break;

			case "Terrain Shape Square":
				context.Enabled = true;
				context.Checked = TerrainToolShape == Component_Terrain_EditingMode.ToolShape.Square;
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
						items.Add( (item.Text, PreviewIconsManager.GetIcon( item.Obj )) );
					context.Action.ListBox.Items = items;

					//update selected item
					var selectIndex = terrainPaintLayersCachedList.FindIndex( a => a.Obj == terrainPaintLayersSelected );
					if( selectIndex == -1 )
						selectIndex = 0;
					context.Action.ListBox.SelectIndex = selectIndex;
				}
				break;

			}
		}

		public override void EditorActionClick( EditorAction.ClickContext context )
		{
			base.EditorActionClick( context );

			switch( context.Action.Name )
			{
			case "Select":
				if( transformTool != null )
				{
					if( transformTool.Mode == TransformTool.ModeEnum.Undefined )
						WorkareaModeSet( "" );
					transformTool.Mode = TransformTool.ModeEnum.None;
				}
				break;

			case "Move":
				if( transformTool != null )
				{
					if( transformTool.Mode == TransformTool.ModeEnum.Undefined )
						WorkareaModeSet( "" );
					transformTool.Mode = TransformTool.ModeEnum.Position;
				}
				break;

			case "Rotate":
				if( transformTool != null )
				{
					if( transformTool.Mode == TransformTool.ModeEnum.Undefined )
						WorkareaModeSet( "" );
					transformTool.Mode = TransformTool.ModeEnum.Rotation;
				}
				break;

			case "Scale":
				if( transformTool != null )
				{
					if( transformTool.Mode == TransformTool.ModeEnum.Undefined )
						WorkareaModeSet( "" );
					transformTool.Mode = TransformTool.ModeEnum.Scale;
				}
				break;

			case "Transform Using Local Coordinates":
				if( transformTool != null )
					transformTool.CoordinateSystemMode = transformTool.CoordinateSystemMode == TransformTool.CoordinateSystemModeEnum.Local ? TransformTool.CoordinateSystemModeEnum.World : TransformTool.CoordinateSystemModeEnum.Local;
				break;

			case "Snap All Axes":
			case "Snap X":
			case "Snap Y":
			case "Snap Z":
				Snap( context.Action );
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
				CreateObjectsBrushRadius = context.Action.Slider.Value;
				break;

			case "Create Objects Brush Strength":
				CreateObjectsBrushStrength = context.Action.Slider.Value;
				break;

			case "Create Objects Brush Hardness":
				CreateObjectsBrushHardness = context.Action.Slider.Value;
				break;

			case "Create Objects Destination":
				if( context.Action.ListBox.LastSelectedIndexChangedByUser )
				{
					var newIndex = context.Action.ListBox.SelectedIndex;
					if( newIndex >= 0 && newIndex < createObjectsDestinationCachedList.Count )
						createObjectsDestinationSelected = createObjectsDestinationCachedList[ newIndex ].Obj;
					else
						createObjectsDestinationSelected = null;
				}
				break;

			case "Terrain Geometry Raise":
			case "Terrain Geometry Lower":
			case "Terrain Geometry Smooth":
			case "Terrain Geometry Flatten":
				if( WorkareaModeName != context.Action.Name )
				{
					var modeStr = context.Action.Name.Substring( "Terrain".Length ).Replace( " ", "" );
					var mode = (Component_Terrain_EditingMode.ModeEnum)Enum.Parse( typeof( Component_Terrain_EditingMode.ModeEnum ), modeStr );
					WorkareaModeSet( context.Action.Name, new Component_Terrain_EditingMode( this, mode ) );

					if( transformTool.Mode != TransformTool.ModeEnum.Undefined )
						transformToolModeRestore = transformTool.Mode;
					transformTool.Mode = TransformTool.ModeEnum.Undefined;
				}
				else
				{
					WorkareaModeSet( "" );
					transformTool.Mode = transformToolModeRestore;
					//ResetWorkareaMode();
				}
				break;

			case "Terrain Shape Circle":
				TerrainToolShape = Component_Terrain_EditingMode.ToolShape.Circle;
				break;

			case "Terrain Shape Square":
				TerrainToolShape = Component_Terrain_EditingMode.ToolShape.Square;
				break;

			case "Terrain Tool Radius":
				TerrainToolRadius = context.Action.Slider.Value;
				break;

			case "Terrain Tool Strength":
				TerrainToolStrength = context.Action.Slider.Value;
				break;

			case "Terrain Tool Hardness":
				TerrainToolHardness = context.Action.Slider.Value;
				break;

			case "Terrain Paint Paint":
			case "Terrain Paint Clear":
			case "Terrain Paint Smooth":
			case "Terrain Paint Flatten":
				if( WorkareaModeName != context.Action.Name )
				{
					var modeStr = context.Action.Name.Substring( "Terrain".Length ).Replace( " ", "" );
					var mode = (Component_Terrain_EditingMode.ModeEnum)Enum.Parse( typeof( Component_Terrain_EditingMode.ModeEnum ), modeStr );
					WorkareaModeSet( context.Action.Name, new Component_Terrain_EditingMode( this, mode ) );

					if( transformTool.Mode != TransformTool.ModeEnum.Undefined )
						transformToolModeRestore = transformTool.Mode;
					transformTool.Mode = TransformTool.ModeEnum.Undefined;
				}
				else
				{
					WorkareaModeSet( "" );
					transformTool.Mode = transformToolModeRestore;
					//ResetWorkareaMode();
				}
				break;

				//case "Terrain Paint Layers":
				//if( context.Action.ListBox.LastSelectedIndexChangedByUser )
				//{
				//	var newIndex = context.Action.ListBox.SelectedIndex;
				//	if( newIndex >= 0 && newIndex < terrainPaintLayersCachedList.Count )
				//		terrainPaintLayersSelected = terrainPaintLayersCachedList[ newIndex ].Obj;
				//	else
				//		terrainPaintLayersSelected = null;
				//}
				//break;

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
			if( ViewportControl != null && ViewportControl.Viewport != null )
				mouse = ViewportControl.Viewport.MousePosition;

			//Editor
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Editor" ), EditorResourcesCache.Edit, delegate ( object s, EventArgs e2 )
				 {
					 EditorAPI.OpenDocumentWindowForObject( Document, oneSelectedComponent );
				 } );
				item.Enabled = oneSelectedComponent != null && EditorAPI.IsDocumentObjectSupport( oneSelectedComponent );
				items.Add( item );
			}

			//Settings
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Settings" ), EditorResourcesCache.Settings, delegate ( object s, EventArgs e2 )
				{
					bool canUseAlreadyOpened = !ModifierKeys.HasFlag( Keys.Shift );
					EditorAPI.ShowObjectSettingsWindow( Document, oneSelectedComponent, canUseAlreadyOpened );
				} );
				item.Enabled = oneSelectedComponent != null;
				items.Add( item );
			}

			items.Add( new KryptonContextMenuSeparator() );

			//New object
			{
				EditorContextMenuWinForms.AddNewObjectItem( items, CanNewObject( out _ ), delegate ( Metadata.TypeInfo type )
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
					EditorAPI.EditorActionClick( EditorAction.HolderEnum.ContextMenu, "Cut" );
				} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Cut" );
				item.Enabled = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.ContextMenu, "Cut" ).Enabled;
				items.Add( item );
			}

			//Copy
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Copy" ), EditorResourcesCache.Copy, delegate ( object s, EventArgs e2 )
				{
					EditorAPI.EditorActionClick( EditorAction.HolderEnum.ContextMenu, "Copy" );
				} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Copy" );
				item.Enabled = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.ContextMenu, "Copy" ).Enabled;
				items.Add( item );
			}

			//Paste
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Paste" ), EditorResourcesCache.Paste, delegate ( object s, EventArgs e2 )
				{
					EditorAPI.EditorActionClick( EditorAction.HolderEnum.ContextMenu, "Paste" );
				} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Paste" );
				item.Enabled = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.ContextMenu, "Paste" ).Enabled;
				items.Add( item );
			}

			//Clone
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Duplicate" ), EditorResourcesCache.Clone, delegate ( object s, EventArgs e2 )
				{
					EditorAPI.EditorActionClick( EditorAction.HolderEnum.ContextMenu, "Duplicate" );
				} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Duplicate" );
				item.Enabled = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.ContextMenu, "Duplicate" ).Enabled;
				items.Add( item );
			}

			//separator
			items.Add( new KryptonContextMenuSeparator() );

			//Delete
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Delete" ), EditorResourcesCache.Delete, delegate ( object s, EventArgs e2 )
				{
					EditorAPI.EditorActionClick( EditorAction.HolderEnum.ContextMenu, "Delete" );
				} );
				item.Enabled = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.ContextMenu, "Delete" ).Enabled;
				items.Add( item );
			}

			//Rename
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Rename" ), null, delegate ( object s, EventArgs e2 )
				{
					EditorAPI.EditorActionClick( EditorAction.HolderEnum.ContextMenu, "Rename" );
				} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rename" );
				item.Enabled = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.ContextMenu, "Rename" ).Enabled;
				items.Add( item );
			}

			EditorContextMenuWinForms.AddActionsToMenu( EditorContextMenuWinForms.MenuTypeEnum.Document, items );

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
				if( window.creationData.initParentObjects.Count == 1 && window.creationData.initParentObjects[ 0 ] is Component_Scene )
				{
					Component createTo = CreateObjectGetDestinationSelected() as Component_Layer;
					if( createTo == null )
						createTo = Scene;

					//Component_MeshGeometry_Procedural
					if( MetadataManager.GetTypeOfNetType( typeof( Component_MeshGeometry_Procedural ) ).IsAssignableFrom( selectedType ) )
					{
						var meshInSpace = createTo.CreateComponent<Component_MeshInSpace>( -1, false );

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

						var mesh = meshInSpace.CreateComponent<Component_Mesh>();
						window.creationData.createdObjects.Add( mesh );
						mesh.Name = "Mesh";

						var geometry = mesh.CreateComponent( selectedType );
						window.creationData.createdObjects.Add( geometry );
						geometry.Name = "Mesh Geometry";

						meshInSpace.Mesh = ReferenceUtility.MakeThisReference( meshInSpace, mesh );

						return true;
					}

					//Component_CollisionShape
					if( MetadataManager.GetTypeOfNetType( typeof( Component_CollisionShape ) ).IsAssignableFrom( selectedType )/* &&
						selectedType != MetadataManager.GetTypeOfNetType( typeof( Component_CollisionShape_Mesh ) )*/ )
					{
						var rigidBody = createTo.CreateComponent<Component_RigidBody>( -1, false );

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

					//!!!!hardcoded
					//Component_RenderingEffect
					if( MetadataManager.GetTypeOfNetType( typeof( Component_RenderingEffect ) ).IsAssignableFrom( selectedType ) )
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
					if( obj is Component_ObjectInSpace objectInSpace )
					{
						var objectToTransform = Component_ObjectInSpace_Utility.CalculateObjectToTransform( objectInSpace );
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

			EditorAPI.OpenNewObjectWindow( data );
		}

		public override bool TryDeleteObjects()
		{
			if( !base.TryDeleteObjects() )
				return false;

			UpdateTransformToolObjects();

			return true;
		}

		private void Component_Scene_DocumentWindow_DragEnter( object sender, DragEventArgs e )
		{
			//createByDropEntered = true;

			if( /*CreateObjectsMode == CreateObjectsModeEnum.Drop &&*/ ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByDrop ) )
			{
				createByDropEntered = true;
				DragDropCreateObject_Create( e );
			}
		}

		private void Component_Scene_DocumentWindow_DragOver( object sender, DragEventArgs e )
		{
			e.Effect = DragDropEffects.None;

			if( /*CreateObjectsMode == CreateObjectsModeEnum.Drop &&*/ ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByDrop ) && createByDropEntered )
			{
				ViewportControl?.PerformMouseMove();

				//для IMeshInSpaceChild потому как он создает при наведении
				//if( objectToCreate == null )
				//	DragDropCreateObject_Create( e );

				CreateObject_Update();
				if( objectToCreate != null )
					e.Effect = DragDropEffects.Link;

				CreateSetProperty_Update( e );
				if( createSetProperty != null )
					e.Effect = DragDropEffects.Link;

				ViewportControl.TryRender();
			}
		}

		private void Component_Scene_DocumentWindow_DragLeave( object sender, EventArgs e )
		{
			if( /*CreateObjectsMode == CreateObjectsModeEnum.Drop &&*/ createByDropEntered )
			{
				CreateObject_Destroy();
				CreateSetProperty_Cancel();
				createByDropEntered = false;

				//!!!!не всегда работает
				ViewportControl.TryRender();
			}
		}

		private void Component_Scene_DocumentWindow_DragDrop( object sender, DragEventArgs e )
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
						var mode = (ObjectCreationMode)attributes[ 0 ].CreationModeClass.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { this, objectToCreate } );
						ObjectCreationModeSet( mode );
						objectTypeToCreate = null;
						objectToCreate = null;
						createByDropEntered = false;
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

		static ContentBrowser.Item CreateObjectGetSelectedContentBrowserItem()
		{
			//Resources Window
			{
				var window = EditorAPI.FindWindow<ResourcesWindow>();
				var selectedObjects = window.GetObjectsInFocus().Objects;
				if( selectedObjects.Length == 1 )
				{
					object selectedObject = selectedObjects[ 0 ];

					var item = selectedObject as ContentBrowser.Item;
					if( item != null )
						return item;
				}
			}

			//!!!!

			return null;
		}

		Metadata.TypeInfo CreateObjectWhatTypeWillCreated( Metadata.TypeInfo objectType, string referenceToObject )
		{
			//Component_MeshGeometry_Procedural
			if( MetadataManager.GetTypeOfNetType( typeof( Component_MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
				return MetadataManager.GetTypeOfNetType( typeof( Component_MeshInSpace ) );

			//Component_Mesh
			if( MetadataManager.GetTypeOfNetType( typeof( Component_Mesh ) ).IsAssignableFrom( objectType ) )
				return MetadataManager.GetTypeOfNetType( typeof( Component_MeshInSpace ) );

			//Component_ParticleSystem
			if( MetadataManager.GetTypeOfNetType( typeof( Component_ParticleSystem ) ).IsAssignableFrom( objectType ) )
				return MetadataManager.GetTypeOfNetType( typeof( Component_ParticleSystemInSpace ) );

			//Component_CollisionShape
			if( MetadataManager.GetTypeOfNetType( typeof( Component_CollisionShape ) ).IsAssignableFrom( objectType ) /*&&
				objectType != MetadataManager.GetTypeOfNetType( typeof( Component_CollisionShape_Mesh ) )*/ )
				return MetadataManager.GetTypeOfNetType( typeof( Component_RigidBody ) );

			//!!!!hardcoded
			//Component_CollisionShape2D
			{
				var collisionShape2DType = MetadataManager.GetType( "NeoAxis.Component_CollisionShape2D" );
				var rigidBody2DType = MetadataManager.GetType( "NeoAxis.Component_RigidBody2D" );
				if( collisionShape2DType != null && rigidBody2DType != null )
				{
					if( collisionShape2DType.IsAssignableFrom( objectType ) )
						return rigidBody2DType;
				}
			}

			//Component_Import3D
			if( typeof( Component_Import3D ).IsAssignableFrom( objectType.GetNetType() ) )
			{
				var componentType = objectType as Metadata.ComponentTypeInfo;
				if( componentType != null && componentType.BasedOnObject != null )
				{
					//!!!!если один вложенный ObjectInSpace, то можно корневой не создавать. но тогда сбрасывается трансформ у вложенного.

					var mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Component_Mesh;
					if( mesh != null )
						return MetadataManager.GetTypeOfNetType( typeof( Component_MeshInSpace ) );
					else
					{
						//Scene mode
						var sceneObjects = componentType.BasedOnObject.GetComponent( "Scene Objects" ) as Component_ObjectInSpace;
						if( sceneObjects != null && sceneObjects.GetProvidedType() != null )
							return sceneObjects.GetProvidedType();
					}
				}
			}

			//Component_RenderingEffect
			if( MetadataManager.GetTypeOfNetType( typeof( Component_RenderingEffect ) ).IsAssignableFrom( objectType ) )
			{
				var pipeline = Scene.RenderingPipeline.Value;
				if( pipeline != null )//&& pipeline.GetComponent( objectType, true ) == null )
				{
					var group = pipeline.GetComponent( "Scene Effects" );
					if( group != null )
						return objectType;
				}
			}

			//Component_Sound
			if( MetadataManager.GetTypeOfNetType( typeof( Component_Sound ) ).IsAssignableFrom( objectType ) )
				return MetadataManager.GetTypeOfNetType( typeof( Component_SoundSource ) );

			//Component by default
			if( MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( objectType ) )
				return objectType;

			return null;
		}

		Component CreateObjectByCreationData( Metadata.TypeInfo objectType, string referenceToObject )
		{
			Component createTo = CreateObjectGetDestinationSelected() as Component_Layer;
			if( createTo == null )
				createTo = Scene;

			//var overObject = GetMouseOverObjectForSelection();

			Component newObject = null;

			//!!!!hardcoded? может это как-то унифицировать?

			////ICanDropToScene
			////Component_ObjectInSpace, _Sky, _Fog
			//if( newObject == null && typeof( ICanDropToScene ).IsAssignableFrom( objectType.GetNetType() ) )
			//{
			//	var objectInSpace = Scene.CreateComponent( objectType, -1, false );
			//	newObject = objectInSpace;
			//}
			////Component_ObjectInSpace
			//if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( Component_ObjectInSpace ) ).IsAssignableFrom( objectType ) )
			//{
			//	var objectInSpace = Scene.CreateComponent( objectType, -1, false );
			//	newObject = objectInSpace;
			//}

			//Component_MeshGeometry_Procedural
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( Component_MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
			{
				var meshInSpace = createTo.CreateComponent<Component_MeshInSpace>( -1, false );
				newObject = meshInSpace;
				newObject.Name = EditorUtility.GetUniqueFriendlyName( newObject, objectType.GetUserFriendlyNameForInstance() );

				var mesh = meshInSpace.CreateComponent<Component_Mesh>();
				mesh.Name = "Mesh";
				var geometry = mesh.CreateComponent( objectType );
				geometry.Name = "Mesh Geometry";
				//geometry.Name = geometry.BaseType.GetUserFriendlyNameForInstance().Replace( '_', ' ' );
				meshInSpace.Mesh = new Reference<Component_Mesh>( null, ReferenceUtility.CalculateThisReference( meshInSpace, mesh ) );
			}

			//Component_Mesh
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( Component_Mesh ) ).IsAssignableFrom( objectType ) )
			{
				var meshInSpace = createTo.CreateComponent<Component_MeshInSpace>( -1, false );
				newObject = meshInSpace;

				//!!!!ссылка такая для всех?
				meshInSpace.Mesh = new Reference<Component_Mesh>( null, referenceToObject );
			}

			//Component_ParticleSystem
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( Component_ParticleSystem ) ).IsAssignableFrom( objectType ) )
			{
				var particleInSpace = createTo.CreateComponent<Component_ParticleSystemInSpace>( -1, false );
				newObject = particleInSpace;

				//!!!!ссылка такая для всех?
				particleInSpace.ParticleSystem = new Reference<Component_ParticleSystem>( null, referenceToObject );
			}

			//Component_CollisionShape
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( Component_CollisionShape ) ).IsAssignableFrom( objectType )/* &&
				objectType != MetadataManager.GetTypeOfNetType( typeof( Component_CollisionShape_Mesh ) )*/ )
			{
				var rigidBody = createTo.CreateComponent<Component_RigidBody>( -1, false );
				newObject = rigidBody;

				var shape = rigidBody.CreateComponent( objectType );
				shape.Name = "Collision Shape";
			}

			//!!!!hardcoded
			//Component_CollisionShape2D
			{
				var collisionShape2DType = MetadataManager.GetType( "NeoAxis.Component_CollisionShape2D" );
				var rigidBody2DType = MetadataManager.GetType( "NeoAxis.Component_RigidBody2D" );
				if( collisionShape2DType != null && rigidBody2DType != null )
				{
					if( newObject == null && collisionShape2DType.IsAssignableFrom( objectType ) )
					{
						var rigidBody = createTo.CreateComponent( rigidBody2DType, -1, false );
						newObject = rigidBody;

						var shape = rigidBody.CreateComponent( objectType );
						shape.Name = "Collision Shape";
					}
				}
			}

			//Component_Import3D
			if( newObject == null && typeof( Component_Import3D ).IsAssignableFrom( objectType.GetNetType() ) )
			{
				var componentType = objectType as Metadata.ComponentTypeInfo;
				if( componentType != null && componentType.BasedOnObject != null )
				{
					//!!!!если один вложенный ObjectInSpace, то можно корневой не создавать. но тогда сбрасывается трансформ у вложенного.

					var mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Component_Mesh;
					if( mesh != null )
					{
						//OneMesh mode
						var meshInSpace = createTo.CreateComponent<Component_MeshInSpace>( -1, false );
						newObject = meshInSpace;
						newObject.Name = EditorUtility.GetUniqueFriendlyName( newObject, objectType.GetUserFriendlyNameForInstance() );
						meshInSpace.Mesh = new Reference<Component_Mesh>( null, ReferenceUtility.CalculateResourceReference( mesh ) );
					}
					else
					{
						//Scene mode
						var sceneObjects = componentType.BasedOnObject.GetComponent( "Scene Objects" ) as Component_ObjectInSpace;
						if( sceneObjects != null && sceneObjects.GetProvidedType() != null )
						{
							var objectInSpace = createTo.CreateComponent( sceneObjects.GetProvidedType(), -1, false );
							newObject = objectInSpace;
						}
					}

					//var sceneObjects = componentType.BasedOnObject.GetComponentByName( "Scene Objects" ) as Component_ObjectInSpace;
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

			//Component_RenderingEffect
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( Component_RenderingEffect ) ).IsAssignableFrom( objectType ) )
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
			////Component_MeshInSpace.IMeshInSpaceChild as child of Component_MeshInSpace
			//if( newObject == null && typeof( Component_MeshInSpace.IMeshInSpaceChild ).IsAssignableFrom( objectType.GetNetType() ) )
			//{
			//	var overMeshInSpace = overObject as Component_MeshInSpace;
			//	if( overMeshInSpace != null )
			//	{
			//		var obj = overMeshInSpace.CreateComponent( objectType, -1, false );
			//		newObject = obj;
			//	}
			//}

			//Component_Sound
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( Component_Sound ) ).IsAssignableFrom( objectType ) )
			{
				var soundSource = createTo.CreateComponent<Component_SoundSource>( -1, false );
				newObject = soundSource;

				//!!!!ссылка такая для всех?
				soundSource.Sound = new Reference<Component_Sound>( null, referenceToObject );
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
					newObject.Name = ComponentUtility.GetNewObjectUniqueName( newObject );
				//set default configuration
				newObject.NewObjectSetDefaultConfiguration();
				//finish object creation
				newObject.Enabled = true;
			}

			return newObject;
		}

		bool CreateObjectFilterEqual( Metadata.TypeInfo objectType, string referenceToObject, Component_ObjectInSpace obj )
		{
			//Component_MeshGeometry_Procedural
			if( MetadataManager.GetTypeOfNetType( typeof( Component_MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
			{
				var meshInSpace = obj as Component_MeshInSpace;
				if( meshInSpace != null )
				{
					var mesh = meshInSpace.GetComponent( "Mesh" ) as Component_Mesh;
					if( mesh != null )
					{
						var meshGeometry = mesh.GetComponent( objectType );
						if( meshGeometry != null && meshGeometry.Name == "Mesh Geometry" )
							return true;
					}
				}
			}

			//Component_Mesh
			if( MetadataManager.GetTypeOfNetType( typeof( Component_Mesh ) ).IsAssignableFrom( objectType ) )
			{
				var meshInSpace = obj as Component_MeshInSpace;
				if( meshInSpace != null && meshInSpace.Mesh.GetByReference == referenceToObject )
					return true;
			}

			//Component_ParticleSystem
			if( MetadataManager.GetTypeOfNetType( typeof( Component_ParticleSystem ) ).IsAssignableFrom( objectType ) )
			{
				var particleInSpace = obj as Component_ParticleSystemInSpace;
				if( particleInSpace != null && particleInSpace.ParticleSystem.GetByReference == referenceToObject )
					return true;
			}

			//Component_CollisionShape
			if( MetadataManager.GetTypeOfNetType( typeof( Component_CollisionShape ) ).IsAssignableFrom( objectType ) /* &&
				objectType != MetadataManager.GetTypeOfNetType( typeof( Component_CollisionShape_Mesh ) )*/ )
			{
				var rigidBody = obj as Component_RigidBody;
				if( rigidBody != null )
				{
					var shape = rigidBody.GetComponent( objectType );
					if( shape != null && shape.Name == "Collision Shape" )
						return true;
				}
			}

			//!!!!Component_CollisionShape2D

			//Component_Import3D
			if( typeof( Component_Import3D ).IsAssignableFrom( objectType.GetNetType() ) )
			{
				var meshInSpace = obj as Component_MeshInSpace;
				if( meshInSpace != null )
				{
					var componentType = objectType as Metadata.ComponentTypeInfo;
					if( componentType != null && componentType.BasedOnObject != null )
					{
						var mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Component_Mesh;
						if( mesh != null )
						{
							if( meshInSpace.Mesh.GetByReference == ReferenceUtility.CalculateResourceReference( mesh ) )
								return true;
						}
						else
						{
							//!!!!

							////Scene mode
							//var sceneObjects = componentType.BasedOnObject.GetComponent( "Scene Objects" ) as Component_ObjectInSpace;
							//if( sceneObjects != null && sceneObjects.GetProvidedType() != null )
							//{
							//	var objectInSpace = Scene.CreateComponent( sceneObjects.GetProvidedType(), -1, false );
							//	newObject = objectInSpace;
							//}
						}
					}
				}
			}

			//Component_Sound
			if( MetadataManager.GetTypeOfNetType( typeof( Component_Sound ) ).IsAssignableFrom( objectType ) )
			{
				var soundSource = obj as Component_SoundSource;
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
			(var objectType, var referenceToObject) = EditorAPI.GetObjectToCreateByDropData( e );
			if( objectType != null )//|| memberFullSignature != "" || createNodeWithComponent != null )
			{
				var newObject = CreateObjectByCreationData( objectType, referenceToObject );
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

		public (bool found, Vector3 position, Component_ObjectInSpace collidedWith) CalculateCreateObjectPositionUnderCursor( Viewport viewport, Component_ObjectInSpace objectInSpace = null, Vector2? overrideMouse = null, Ray? overrideRay = null, bool allowSnap = true )
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

			return Component_Scene_Utility.CalculateCreateObjectPositionByRay( Scene, objectInSpace, ray, allowSnap2 );
		}

		public void CalculateCreateObjectPosition( Component_ObjectInSpace objectInSpace, Component_ObjectInSpace objectToTransform, Vector2? mouse = null )
		{
			var result = CalculateCreateObjectPositionUnderCursor( Viewport, objectInSpace, mouse );
			objectToTransform.Transform = new Transform( result.position, objectToTransform.Transform.Value.Rotation, objectToTransform.Transform.Value.Scale );
		}

		void CreateObject_Update()
		{
			if( objectToCreate != null && objectToCreate is Component_ObjectInSpace objectInSpace )
			{
				var objectToTransform = Component_ObjectInSpace_Utility.CalculateObjectToTransform( objectInSpace );
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
				//		var objectInSpace = obj as Component_ObjectInSpace;
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

			////!!!!тут удаляется. создается в _Create. может можно в одном месте это всё
			////Component_MeshInSpace.IMeshInSpaceChild as child of Component_MeshInSpace
			//if( dragDropCreateObject != null && dragDropCreateObject is Component_MeshInSpace.IMeshInSpaceChild meshInSpaceChild )
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
				var action = new UndoActionComponentCreateDelete( Document, newObjects, true );
				Document.UndoSystem.CommitAction( action );
				Document.Modified = true;

				objectTypeToCreate = null;
				objectToCreate = null;

				//select created object
				if( CreateObjectsMode != CreateObjectsModeEnum.Click )
					//if( CreateObjectsMode == CreateObjectsModeEnum.Drop )
					EditorAPI.SelectComponentsInMainObjectsWindow( this, new Component[] { obj } );

				EditorAPI.SelectDockWindow( this );
			}
		}

		void CreateSetProperty_Update( DragEventArgs dragEventArgs )
		{
			object setPropertyObject = null;
			//Component setPropertyObject = null;
			//Component_ObjectInSpace setPropertyObject = null;
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
					contentBrowserItem = CreateObjectGetSelectedContentBrowserItem();
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

					if( dragDropType != null )
					{
						//drop Material to
						if( MetadataManager.GetTypeOfNetType( typeof( Component_Material ) ).IsAssignableFrom( dragDropType ) )
						{
							//drop Material to Component_MeshInSpace.ReplaceMaterial
							var overMeshInSpace = overObject as Component_MeshInSpace;
							if( overMeshInSpace != null )
							{
								setPropertyObject = overMeshInSpace;
								setProperty = (Metadata.Property)overMeshInSpace.MetadataGetMemberBySignature( "property:ReplaceMaterial" );
								setPropertyValue = ReferenceUtility.MakeReference( typeof( Component_Material ), null, referenceValue );

								setPropertyVisualizeCanSelectObject = null;
							}

							//drop Material to Component_Billboard.Material
							var overBillboard = overObject as Component_Billboard;
							if( overBillboard != null )
							{
								setPropertyObject = overBillboard;
								setProperty = (Metadata.Property)overBillboard.MetadataGetMemberBySignature( "property:Material" );
								setPropertyValue = ReferenceUtility.MakeReference( typeof( Component_Material ), null, referenceValue );

								setPropertyVisualizeCanSelectObject = null;
							}
						}

						//drop Material from Component_Import3D to
						if( typeof( Component_Import3D ).IsAssignableFrom( dragDropType.GetNetType() ) )
						{
							//check Material component exists
							bool materialExists = false;
							{
								var componentType = dragDropType as Metadata.ComponentTypeInfo;
								if( componentType != null && componentType.BasedOnObject != null )
								{
									var material = componentType.BasedOnObject.GetComponent( "Material" ) as Component_Material;
									if( material != null )
										materialExists = true;
								}

							}

							if( materialExists )
							{
								var referenceValue2 = referenceValue + referenceValueAddSeparator.ToString() + "$Material";

								//drop Material to Component_MeshInSpace.ReplaceMaterial
								var overMeshInSpace = overObject as Component_MeshInSpace;
								if( overMeshInSpace != null )
								{
									setPropertyObject = overMeshInSpace;
									setProperty = (Metadata.Property)overMeshInSpace.MetadataGetMemberBySignature( "property:ReplaceMaterial" );
									setPropertyValue = ReferenceUtility.MakeReference( typeof( Component_Material ), null, referenceValue2 );

									setPropertyVisualizeCanSelectObject = null;
								}

								//drop Material to Component_Billboard.Material
								var overBillboard = overObject as Component_Billboard;
								if( overBillboard != null )
								{
									setPropertyObject = overBillboard;
									setProperty = (Metadata.Property)overBillboard.MetadataGetMemberBySignature( "property:Material" );
									setPropertyValue = ReferenceUtility.MakeReference( typeof( Component_Material ), null, referenceValue2 );

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
						//if( MetadataManager.GetTypeOfNetType( typeof( Component_ObjectInSpace ) ).IsAssignableFrom( propertyTypeUnref ) )
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

						//if( MetadataManager.GetTypeOfNetType( typeof( Component_RigidBody ) ).IsAssignableFrom( propertyTypeUnref ) )
						//{
						//	multiselection

						//	var overRigidBody = GetMouseOverObjectForSelection() as Component_RigidBody;
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

				EditorAPI.SelectDockWindow( this );

				return true;
			}

			return false;
		}

		void UpdateTransformToolActiveState()
		{
			transformTool.Active = !createByDropEntered && !createByClickEntered && !createByBrushEntered && ObjectCreationMode == null;
		}

		public bool CanSnap( out List<Component_ObjectInSpace> resultObjects )
		{
			resultObjects = new List<Component_ObjectInSpace>();
			foreach( var toolObject in transformTool.Objects )
			{
				var toolObject2 = toolObject as TransformToolObject_ObjectInSpace;
				if( toolObject2 != null )
				{
					var objectToTransform = Component_ObjectInSpace_Utility.CalculateObjectToTransform( toolObject2.SelectedObject );
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

			var property = (Metadata.Property)MetadataManager.GetTypeOfNetType( typeof( Component_ObjectInSpace ) ).MetadataGetMemberBySignature( "property:Transform" );
			var snapValue = ProjectSettings.Get.SceneEditorStepMovement.Value;

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
			return EditorLocalization.Translate( "SceneDocumentWindow", text );
		}

		protected override void GetTextInfoLeftTopCorner( List<string> lines )
		{
			base.GetTextInfoLeftTopCorner( lines );

			//statistics
			if( Scene != null && Scene.GetDisplayDevelopmentDataInThisApplication() && Scene.DisplayTextInfo )
			{
				var statistics = ViewportControl?.Viewport?.RenderingContext?.UpdateStatisticsPrevious;
				if( statistics != null )
				{
					lines.Add( Translate( "FPS" ) + ": " + statistics.FPS.ToString( "F1" ) );
					lines.Add( Translate( "Triangles" ) + ": " + statistics.Triangles.ToString() );
					lines.Add( Translate( "Lines" ) + ": " + statistics.Lines.ToString() );
					lines.Add( Translate( "Draw calls" ) + ": " + statistics.DrawCalls.ToString() );
					lines.Add( Translate( "Render targets" ) + ": " + statistics.RenderTargets.ToString() );
					lines.Add( Translate( "Dynamic textures" ) + ": " + statistics.DynamicTextures.ToString() );
					lines.Add( Translate( "Lights" ) + ": " + statistics.Lights.ToString() );
					lines.Add( Translate( "Reflection probes" ) + ": " + statistics.ReflectionProbes.ToString() );
				}
			}
		}

		protected override void GetTextInfoRightBottomCorner( List<string> lines )
		{
			base.GetTextInfoRightBottomCorner( lines );

			if( createByClickEntered )
				lines.Add( Translate( "Creating an object by click" ) );
			if( createByBrushEntered )
				lines.Add( Translate( "Creating an object by brush" ) );

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
						var objectInSpace = cloned as Component_ObjectInSpace;
						if( objectInSpace != null )
						{
							var objectToTransform = Component_ObjectInSpace_Utility.CalculateObjectToTransform( objectInSpace );
							if( objectToTransform == null )
								objectToTransform = objectInSpace;

							if( n == 0 )
							{
								CalculateCreateObjectPosition( objectInSpace, objectToTransform );
								addToPosition = objectToTransform.Transform.Value.Position - ( (Component_ObjectInSpace)c ).Transform.Value.Position;
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
					if( data.documentWindow.Document != Document )
					{
						//another document
						{
							var action = new UndoActionComponentCreateDelete( data.documentWindow.Document, components, false );
							data.documentWindow.Document.UndoSystem.CommitAction( action );
							data.documentWindow.Document.Modified = true;
						}
						{
							var action = new UndoActionComponentCreateDelete( Document, newObjects, true );
							Document.UndoSystem.CommitAction( action );
							Document.Modified = true;
						}
					}
					else
					{
						//same document
						var multiAction = new UndoMultiAction();
						multiAction.AddAction( new UndoActionComponentCreateDelete( Document, components, false ) );
						multiAction.AddAction( new UndoActionComponentCreateDelete( Document, newObjects, true ) );
						Document.UndoSystem.CommitAction( multiAction );
						Document.Modified = true;
					}
				}
				else
				{
					//copy
					var action = new UndoActionComponentCreateDelete( Document, newObjects, true );
					Document.UndoSystem.CommitAction( action );
					Document.Modified = true;
				}
			}

			return true;
		}

		public bool CanFocusCameraOnSelectedObject( out Component_ObjectInSpace[] objects )
		{
			if( !AllowCameraControl )
			{
				objects = null;
				return false;
			}

			objects = SelectedObjects.OfType<Component_ObjectInSpace>().ToArray();
			return objects.Length != 0;
		}

		public void FocusCameraOnSelectedObject( Component_ObjectInSpace[] objects )
		{
			Bounds bounds = NeoAxis.Bounds.Cleared;
			foreach( var obj in objects )
			{
				if( obj is Component_Light )
				{
					var b = new Bounds( obj.Transform.Value.Position );
					b.Expand( 0.1 );
					bounds.Add( b );
				}
				else
					bounds.Add( obj.SpaceBounds.CalculatedBoundingBox );
			}

			Component_Camera camera = Scene.Mode.Value == Component_Scene.ModeEnum._3D ? Scene.CameraEditor : Scene.CameraEditor2D;
			if( !bounds.IsCleared() && camera != null )
			{
				var needRectangle = new Rectangle( .4f, .3f, .6f, .7f );

				var lookTo = bounds.GetCenter();
				var points = bounds.ToPoints();

				if( Scene.Mode.Value == Component_Scene.ModeEnum._3D )
				{
					double distance = 1000;
					while( distance > 0.3 )
					{
						camera.SetPosition( lookTo - camera.TransformV.Rotation.GetForward() * distance );

						var viewport = ViewportControl.Viewport;

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
		}

		[Browsable( false )]
		public TransformTool TransformTool
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
				//Component_ObjectInSpace
				if( transformToolObject == null )
				{
					//!!!!для каких объектов не создавать?

					var objectInSpace = forObject as Component_ObjectInSpace;
					if( objectInSpace != null )
						return new TransformToolObject_ObjectInSpace( objectInSpace );
				}
			}

			return transformToolObject;
		}

		[Browsable( false )]
		public new WorkareaModeClass_Scene WorkareaMode
		{
			get { return (WorkareaModeClass_Scene)base.WorkareaMode; }
		}

		public void ResetWorkareaMode()
		{
			WorkareaModeSet( "" );
			transformTool.Mode = transformToolModeRestore;
		}

		public void ChangeCreateObjectsMode( CreateObjectsModeEnum mode )
		{
			if( CreateObjectsMode == mode )
				return;

			CreateObjectsMode = mode;

			//!!!!что-нибудь делать после изменения?
		}

		bool CreateByClick_Create()
		{
			(var objectType, var referenceToObject) = EditorAPI.GetSelectedObjectToCreate();
			if( objectType != null )//|| memberFullSignature != "" || createNodeWithComponent != null )
			{
				var typeWillCreated = CreateObjectWhatTypeWillCreated( objectType, referenceToObject );
				if( typeWillCreated != null && MetadataManager.GetTypeOfNetType( typeof( Component_ObjectInSpace ) ).IsAssignableFrom( typeWillCreated ) )
				{
					var newObject = CreateObjectByCreationData( objectType, referenceToObject );
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
				if( CreateByClick_Create() )
					createByClickEntered = true;
			}
			if( CreateObjectsMode == CreateObjectsModeEnum.Brush && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByBrush ) && ObjectCreationMode == null )
			{
				if( CreateByBrush_Create() )
					createByBrushEntered = true;
			}
		}

		void CreateByClick_Cancel()
		{
			CreateObject_Destroy();
			CreateSetProperty_Cancel();
			createByClickEntered = false;
		}

		private void ViewportControl_MouseLeave( object sender, EventArgs e )
		{
			if( CreateObjectsMode == CreateObjectsModeEnum.Click )
				CreateByClick_Cancel();
			if( CreateObjectsMode == CreateObjectsModeEnum.Brush )
				CreateByBrush_Cancel();
		}

		public override void WorkareaModeSet( string name, WorkareaModeClass instance = null )
		{
			base.WorkareaModeSet( name, instance );

			CreateByClick_Cancel();
			CreateByBrush_Cancel();

			if( new Rectangle( 0, 0, 1, 1 ).Contains( Viewport.MousePosition ) )
			{
				if( CreateObjectsMode == CreateObjectsModeEnum.Click && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByClick ) && ObjectCreationMode == null )
				{
					if( CreateByClick_Create() )
						createByClickEntered = true;
				}
				if( CreateObjectsMode == CreateObjectsModeEnum.Brush && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByBrush ) && ObjectCreationMode == null )
				{
					if( CreateByBrush_Create() )
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
						if( CreateByClick_Create() )
							createByClickEntered = true;
					}
					if( CreateObjectsMode == CreateObjectsModeEnum.Brush && ( WorkareaMode == null || WorkareaMode.AllowCreateObjectsByBrush ) )
					{
						if( CreateByBrush_Create() )
							createByBrushEntered = true;
					}
				}
			}

			base.ObjectCreationModeSet( mode );
		}

		bool CreateByBrush_Create()
		{
			var toGroupOfObjects = CreateObjectGetDestinationSelected() as Component_GroupOfObjects;

			if( toGroupOfObjects != null )
			{
				//when destination != null

				(var objectType, var referenceToObject) = EditorAPI.GetSelectedObjectToCreate();
				if( objectType != null )
				{
					//mesh
					{
						Component_Mesh mesh = null;
						Metadata.TypeInfo meshGeometryProcedural = null;
						{
							var componentType = objectType as Metadata.ComponentTypeInfo;
							if( componentType != null && componentType.BasedOnObject != null )
							{
								//Component_Mesh
								mesh = componentType.BasedOnObject as Component_Mesh;

								//Component_Import3D
								if( componentType.BasedOnObject is Component_Import3D )
									mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Component_Mesh;
							}

							//Component_MeshGeometry_Procedural
							if( MetadataManager.GetTypeOfNetType( typeof( Component_MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
								meshGeometryProcedural = objectType;
						}

						if( mesh != null || meshGeometryProcedural != null )
							return true;
					}

					//surface
					{
						Component_Surface surface = null;
						{
							var componentType = objectType as Metadata.ComponentTypeInfo;
							if( componentType != null && componentType.BasedOnObject != null )
								surface = componentType.BasedOnObject as Component_Surface;
						}

						if( surface != null )
							return true;
					}
				}
			}
			else
			{
				//when toGroupOfObjects == null

				(var objectType, var referenceToObject) = EditorAPI.GetSelectedObjectToCreate();
				if( objectType != null )
				{
					if( MetadataManager.GetTypeOfNetType( typeof( Component_Surface ) ).IsAssignableFrom( objectType ) )
						return true;

					var typeWillCreated = CreateObjectWhatTypeWillCreated( objectType, referenceToObject );
					if( typeWillCreated != null && MetadataManager.GetTypeOfNetType( typeof( Component_ObjectInSpace ) ).IsAssignableFrom( typeWillCreated ) )
						return true;
				}
			}

			return false;
		}

		void CreateByBrush_Cancel()
		{
			CreateByBrushPaintingEnd( false );
			createByBrushEntered = false;
		}

		bool CreateByBrushPaintingBegin( Viewport viewport )
		{
			if( CreateObjectsMode == CreateObjectsModeEnum.Brush && !viewport.MouseRelativeMode && createByBrushEntered )
			{
				var toGroupOfObjects = CreateObjectGetDestinationSelected() as Component_GroupOfObjects;
				if( CreateByBrushGetCenter( viewport, toGroupOfObjects, out _ ) )
				{
					createByBrushPainting = true;
					createByBrushPaintingDeleting = ( ModifierKeys & Keys.Shift ) != 0;

					//CreateObjectsTick() вызвать?
				}

				return true;
			}

			return false;
		}

		bool CreateByBrushPaintingEnd( bool cancel )
		{
			if( createByBrushPainting )
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
						undoMultiAction.AddAction( new Component_GroupOfObjects_Editor.UndoActionCreateDelete( createByBrushGroupOfObjects, createByBrushGroupOfObjectsObjectsCreated.ToArray(), true, false ) );
					}

					if( createByBrushGroupOfObjectsObjectsDeleted.Count != 0 )
					{
						undoMultiAction.AddAction( new Component_GroupOfObjects_Editor.UndoActionCreateDelete( createByBrushGroupOfObjects, createByBrushGroupOfObjectsObjectsDeleted.ToArray(), false, false ) );
					}

					if( createByBrushComponentsCreated.Count != 0 )
						undoMultiAction.AddAction( new UndoActionComponentCreateDelete( Document, createByBrushComponentsCreated, true ) );

					if( createByBrushComponentsToDelete.Count != 0 )
					{
						var objects = new List<Component>( createByBrushComponentsToDelete.Count );
						foreach( var item in createByBrushComponentsToDelete )
						{
							if( item.wasEnabled )
								item.obj.Enabled = true;
							objects.Add( item.obj );
						}
						undoMultiAction.AddAction( new UndoActionComponentCreateDelete( Document, objects, false ) );
					}

					if( undoMultiAction.Actions.Count != 0 )
						Document.CommitUndoAction( undoMultiAction );
				}

				createByBrushPainting = false;
				createByBrushPaintingDeleting = false;
				createByBrushGroupOfObjects = null;
				createByBrushGroupOfObjectsObjectsCreated.Clear();
				createByBrushGroupOfObjectsObjectsDeleted.Clear();
				createByBrushComponentsCreated.Clear();
				createByBrushComponentsToDelete.Clear();
				return true;
			}

			return false;
		}

		void CreateByBrushPaintingPaint( Viewport viewport )
		{
			var toGroupOfObjects = CreateObjectGetDestinationSelected() as Component_GroupOfObjects;
			var toolRadius = CreateObjectsBrushRadius;
			var toolStrength = CreateObjectsBrushStrength;
			var toolHardness = CreateObjectsBrushHardness;
			var random = new Random();

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

			if( CreateByBrushGetCenter( viewport, toGroupOfObjects, out var center ) )
			{
				if( toGroupOfObjects != null )
				{
					//when destination != null

					var destinationCachedBaseObjects = toGroupOfObjects.GetBaseObjects();

					if( !createByBrushPaintingDeleting )
					{
						//creating

						(var objectType, var referenceToObject) = EditorAPI.GetSelectedObjectToCreate();
						if( objectType != null )
						{
							//mesh
							{
								Component_Mesh mesh = null;
								ReferenceNoValue referenceToMesh = new ReferenceNoValue();
								Metadata.TypeInfo meshGeometryProcedural = null;
								{
									var componentType = objectType as Metadata.ComponentTypeInfo;
									if( componentType != null && componentType.BasedOnObject != null )
									{
										//!!!!пока только ресурсные ссылки поддерживаются

										//Component_Mesh
										mesh = componentType.BasedOnObject as Component_Mesh;
										if( mesh != null )
											referenceToMesh = ReferenceUtility.MakeResourceReference( mesh );

										//Component_Import3D
										if( componentType.BasedOnObject is Component_Import3D )
										{
											mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Component_Mesh;
											if( mesh != null )
												referenceToMesh = ReferenceUtility.MakeResourceReference( mesh );
										}
									}

									//Component_MeshGeometry_Procedural
									if( MetadataManager.GetTypeOfNetType( typeof( Component_MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
									{
										meshGeometryProcedural = objectType;
										referenceToMesh = new ReferenceNoValue( "this:$Mesh" );
									}
								}

								if( mesh != null || meshGeometryProcedural != null )
								{
									double minDistanceBetweenObjects;
									if( mesh != null )
										minDistanceBetweenObjects = mesh.Result.SpaceBounds.CalculatedBoundingSphere.Radius * 2;
									else
										minDistanceBetweenObjects = 1.5;

									//calculate object count
									int count;
									{
										var toolSquare = Math.PI * toolRadius * toolRadius;

										double radius = minDistanceBetweenObjects / 2;
										double objectSquare = Math.PI * radius * radius;
										if( objectSquare < 0.1 )
											objectSquare = 0.1;

										double maxCount = toolSquare / objectSquare;
										maxCount /= 10;

										count = (int)( toolStrength * (double)maxCount );
										count = Math.Max( count, 1 );
									}

									var data = new List<Component_GroupOfObjects.Object>( count );

									//find element
									Component_GroupOfObjectsElement_Mesh element = null;
									{
										var elements = toGroupOfObjects.GetComponents<Component_GroupOfObjectsElement_Mesh>();

										if( mesh != null )
											element = elements.FirstOrDefault( e => e.Mesh.Value == mesh && e.Enabled );

										if( meshGeometryProcedural != null )
										{
											foreach( var e in elements )
											{
												if( e.Enabled )
												{
													var mesh2 = e.GetComponent( "Mesh" ) as Component_Mesh;
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
										var elementMesh = toGroupOfObjects.CreateComponent<Component_GroupOfObjectsElement_Mesh>( enabled: false );

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
											var mesh2 = elementMesh.CreateComponent<Component_Mesh>();
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
										var bounds = new Bounds( center );
										bounds.Expand( toolRadius + minDistanceBetweenObjects );
										pointContainerFindFreePlace = new PointContainer3D( bounds, 100 );

										var item = new Component_GroupOfObjects.GetObjectsItem( Component_GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, true, bounds );
										toGroupOfObjects.GetObjects( item );
										foreach( var resultItem in item.Result )
										{
											ref var obj = ref toGroupOfObjects.ObjectGetData( resultItem.Object );
											if( obj.Element == element.Index )
												pointContainerFindFreePlace.Add( ref obj.Position );
										}
									}

									for( int n = 0; n < count; n++ )
									{
										Vector3? position = null;

										int counter = 0;
										while( counter < 20 )
										{
											var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

											//check by radius and by hardness
											var length = offset.Length();
											if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
											{
												var position2 = center.ToVector2() + offset;

												var result = Component_Scene_Utility.CalculateObjectPositionZ( Scene, toGroupOfObjects, center.Z, position2, destinationCachedBaseObjects );
												if( result.found )
												{
													var p = new Vector3( position2, result.positionZ );
													//move up to align
													{
														var mesh2 = element.Mesh.Value;
														if( mesh2 != null )
															p.Z += -mesh2.Result.SpaceBounds.CalculatedBoundingBox.Minimum.Z;
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
											var obj = new Component_GroupOfObjects.Object();
											obj.Element = (ushort)element.Index.Value;
											obj.Flags = Component_GroupOfObjects.Object.FlagsEnum.Enabled | Component_GroupOfObjects.Object.FlagsEnum.Visible;
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
								Component_Surface surface = null;
								{
									var componentType = objectType as Metadata.ComponentTypeInfo;
									if( componentType != null && componentType.BasedOnObject != null )
										surface = componentType.BasedOnObject as Component_Surface;
								}

								if( surface != null )
								{
									//calculate object count
									int count;
									{
										var toolSquare = Math.PI * toolRadius * toolRadius;

										//!!!!среднее от всех групп
										double minDistanceBetweenObjects;
										{
											var groups = surface.GetComponents<Component_SurfaceGroupOfElements>();
											if( groups.Length != 0 )
											{
												minDistanceBetweenObjects = 0;
												foreach( var group in groups )
													minDistanceBetweenObjects += group.MinDistanceBetweenObjects;
												minDistanceBetweenObjects /= groups.Length;
											}
											else
												minDistanceBetweenObjects = 1;
										}

										double radius = minDistanceBetweenObjects / 2;
										double objectSquare = Math.PI * radius * radius;
										if( objectSquare < 0.1 )
											objectSquare = 0.1;

										double maxCount = toolSquare / objectSquare;
										maxCount /= 10;

										count = (int)( toolStrength * (double)maxCount );
										count = Math.Max( count, 1 );
									}

									var data = new List<Component_GroupOfObjects.Object>( count );

									//find element
									var element = toGroupOfObjects.GetComponents<Component_GroupOfObjectsElement_Surface>().FirstOrDefault( e => e.Surface.Value == surface );

									//create element
									if( element == null )
									{
										var elementIndex = toGroupOfObjects.GetFreeElementIndex();

										var elementSurface = toGroupOfObjects.CreateComponent<Component_GroupOfObjectsElement_Surface>();

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

									//create point container to check by MinDistanceBetweenObjects
									PointContainer3D pointContainerFindFreePlace;
									{
										double minDistanceBetweenObjectsMax = 0;
										foreach( var group in surface.GetComponents<Component_SurfaceGroupOfElements>() )
											minDistanceBetweenObjectsMax = Math.Max( minDistanceBetweenObjectsMax, group.MinDistanceBetweenObjects );

										var bounds = new Bounds( center );
										bounds.Expand( toolRadius + minDistanceBetweenObjectsMax );
										pointContainerFindFreePlace = new PointContainer3D( bounds, 100 );

										var item = new Component_GroupOfObjects.GetObjectsItem( Component_GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, true, bounds );
										toGroupOfObjects.GetObjects( item );
										foreach( var resultItem in item.Result )
										{
											ref var obj = ref toGroupOfObjects.ObjectGetData( resultItem.Object );
											if( obj.Element == element.Index )
												pointContainerFindFreePlace.Add( ref obj.Position );
										}
									}

									for( int n = 0; n < count; n++ )
									{
										surface.GetRandomVariation( new Component_Surface.GetRandomVariationOptions(), random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );
										var surfaceGroup = surface.GetGroup( groupIndex );

										Vector3? position = null;

										int counter = 0;
										while( counter < 20 )
										{
											var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

											//check by radius and by hardness
											var length = offset.Length();
											if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
											{
												var position2 = center.ToVector2() + offset;

												var result = Component_Scene_Utility.CalculateObjectPositionZ( Scene, toGroupOfObjects, center.Z, position2, destinationCachedBaseObjects );
												if( result.found )
												{
													var p = new Vector3( position2, result.positionZ );

													//check by MinDistanceBetweenObjects
													if( surfaceGroup == null || !pointContainerFindFreePlace.Contains( new Sphere( p, surfaceGroup.MinDistanceBetweenObjects ) ) )
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
											var obj = new Component_GroupOfObjects.Object();
											obj.Element = (ushort)element.Index.Value;
											obj.VariationGroup = groupIndex;
											obj.VariationElement = elementIndex;
											obj.Flags = Component_GroupOfObjects.Object.FlagsEnum.Enabled | Component_GroupOfObjects.Object.FlagsEnum.Visible;
											obj.Position = position.Value + new Vector3( 0, 0, positionZ );
											obj.Rotation = rotation;
											obj.Scale = scale;
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

						}

					}
					else
					{
						//deleting

						(var objectType, var referenceToObject) = EditorAPI.GetSelectedObjectToCreate();
						if( objectType != null )
						{
							int elementIndex = -1;
							{
								//mesh
								{
									Component_Mesh mesh = null;
									Metadata.TypeInfo meshGeometryProcedural = null;
									{
										var componentType = objectType as Metadata.ComponentTypeInfo;
										if( componentType != null && componentType.BasedOnObject != null )
										{
											//Component_Mesh
											mesh = componentType.BasedOnObject as Component_Mesh;

											//Component_Import3D
											if( componentType.BasedOnObject is Component_Import3D )
												mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Component_Mesh;
										}

										//Component_MeshGeometry_Procedural
										if( MetadataManager.GetTypeOfNetType( typeof( Component_MeshGeometry_Procedural ) ).IsAssignableFrom( objectType ) )
											meshGeometryProcedural = objectType;
									}

									var elements = toGroupOfObjects.GetComponents<Component_GroupOfObjectsElement_Mesh>();

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
												var mesh2 = e.GetComponent( "Mesh" ) as Component_Mesh;
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
									Component_Surface surface = null;
									{
										var componentType = objectType as Metadata.ComponentTypeInfo;
										if( componentType != null && componentType.BasedOnObject != null )
											surface = componentType.BasedOnObject as Component_Surface;
									}

									if( surface != null )
									{
										var element = toGroupOfObjects.GetComponents<Component_GroupOfObjectsElement_Surface>().FirstOrDefault( e => e.Surface.Value == surface );
										if( element != null )
											elementIndex = element.Index;
									}
								}
							}

							if( elementIndex != -1 )
							{
								var bounds = new Bounds( center );
								bounds.Expand( toolRadius );

								var resultItem = new Component_GroupOfObjects.GetObjectsItem( Component_GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, false, bounds );
								toGroupOfObjects.GetObjects( resultItem );

								var indexesToDelete = new List<int>( resultItem.Result.Length );

								foreach( var item in resultItem.Result )
								{
									ref var obj = ref toGroupOfObjects.ObjectGetData( item.Object );

									if( obj.Element == elementIndex && ( center.ToVector2() - obj.Position.ToVector2() ).Length() <= toolRadius )
									{
										createByBrushGroupOfObjects = toGroupOfObjects;

										indexesToDelete.Add( item.Object );
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

					if( !createByBrushPaintingDeleting )
					{
						//creating

						(var objectType, var referenceToObject) = EditorAPI.GetSelectedObjectToCreate();
						if( objectType != null )
						{
							Component createTo = CreateObjectGetDestinationSelected() as Component_Layer;
							if( createTo == null )
								createTo = Scene;

							if( MetadataManager.GetTypeOfNetType( typeof( Component_Surface ) ).IsAssignableFrom( objectType ) )
							{
								//surface

								Component_Surface surface = null;
								{
									var componentType = objectType as Metadata.ComponentTypeInfo;
									if( componentType != null && componentType.BasedOnObject != null )
										surface = componentType.BasedOnObject as Component_Surface;
								}

								if( surface != null )
								{
									var surfaceAllMeshesSet = new ESet<Component_Mesh>( surface.GetAllMeshes() );

									//calculate object count
									int count;
									{
										var toolSquare = Math.PI * toolRadius * toolRadius;

										//!!!!среднее от всех групп
										double minDistanceBetweenObjects;
										{
											var groups = surface.GetComponents<Component_SurfaceGroupOfElements>();
											if( groups.Length != 0 )
											{
												minDistanceBetweenObjects = 0;
												foreach( var group in groups )
													minDistanceBetweenObjects += group.MinDistanceBetweenObjects;
												minDistanceBetweenObjects /= groups.Length;
											}
											else
												minDistanceBetweenObjects = 1;
										}

										double radius = minDistanceBetweenObjects / 2;
										double objectSquare = Math.PI * radius * radius;
										if( objectSquare < 0.1 )
											objectSquare = 0.1;

										double maxCount = toolSquare / objectSquare;
										maxCount /= 10;

										count = (int)( toolStrength * (double)maxCount );
										count = Math.Max( count, 1 );
									}

									for( int n = 0; n < count; n++ )
									{
										surface.GetRandomVariation( new Component_Surface.GetRandomVariationOptions(), random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );
										var surfaceGroup = surface.GetGroup( groupIndex );

										Vector3? position = null;

										int counter = 0;
										while( counter < 20 )
										{
											var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

											//check by radius and by hardness
											var length = offset.Length();
											if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
											{
												var position2 = center.ToVector2() + offset;

												//!!!!
												var result = Component_Scene_Utility.CalculateObjectPositionZ( Scene, null, center.Z, position2 );
												if( result.found )
												{
													var p = new Vector3( position2, result.positionZ );

													//check by MinDistanceBetweenObjects
													bool free = true;
													{
														var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, new Sphere( p, surfaceGroup.MinDistanceBetweenObjects ) );
														Scene.GetObjectsInSpace( getObjectsItem );
														foreach( var item in getObjectsItem.Result )
														{
															var obj = item.Object;

															if( ( center.ToVector2() - obj.TransformV.Position.ToVector2() ).Length() <= surfaceGroup.MinDistanceBetweenObjects )
															{
																var meshInSpace = obj as Component_MeshInSpace;
																if( meshInSpace != null && surfaceAllMeshesSet.Contains( meshInSpace.Mesh ) )
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
														break;
													}
												}
											}

											counter++;
										}

										if( position != null )
										{
											Component_ObjectInSpace newObject = null;
											{
												var elements = surfaceGroup.GetComponents();
												if( elementIndex < elements.Length )
												{
													var element = elements[ elementIndex ];

													//mesh element
													var elementMesh = element as Component_SurfaceElement_Mesh;
													if( elementMesh != null )
													{
														var meshInSpace = createTo.CreateComponent<Component_MeshInSpace>( enabled: false );
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

															meshInSpace.VisibilityDistance = elementMesh.VisibilityDistance;
															meshInSpace.CastShadows = elementMesh.CastShadows;
															meshInSpace.ReceiveDecals = elementMesh.ReceiveDecals;

															newObject = meshInSpace;
														}
													}

												}
											}

											if( newObject != null )
											{
												//set transform
												var objectToTransform = Component_ObjectInSpace_Utility.CalculateObjectToTransform( newObject );
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

								double minDistanceBetweenObjects = 1.5;
								{
									var componentType = objectType as Metadata.ComponentTypeInfo;
									if( componentType != null && componentType.BasedOnObject != null )
									{
										//Component_Mesh
										var mesh = componentType.BasedOnObject as Component_Mesh;
										if( mesh != null )
											minDistanceBetweenObjects = mesh.Result.SpaceBounds.CalculatedBoundingSphere.Radius * 2;

										//Component_Import3D
										if( componentType.BasedOnObject is Component_Import3D )
										{
											mesh = componentType.BasedOnObject.GetComponent( "Mesh" ) as Component_Mesh;
											if( mesh != null )
												minDistanceBetweenObjects = mesh.Result.SpaceBounds.CalculatedBoundingSphere.Radius * 2;
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
									maxCount /= 10;

									count = (int)( toolStrength * (double)maxCount );
									count = Math.Max( count, 1 );
								}


								for( int n = 0; n < count; n++ )
								{
									Vector3? position = null;

									int counter = 0;
									while( counter < 20 )
									{
										var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

										//check by radius and by hardness
										var length = offset.Length();
										if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
										{
											var position2 = center.ToVector2() + offset;

											var result = Component_Scene_Utility.CalculateObjectPositionZ( Scene, null, center.Z, position2 );
											if( result.found )
											{
												var p = new Vector3( position2, result.positionZ );

												//check by MinDistanceBetweenObjects
												bool free = true;
												{
													var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, new Sphere( p, minDistanceBetweenObjects ) );
													Scene.GetObjectsInSpace( getObjectsItem );
													foreach( var item in getObjectsItem.Result )
													{
														var obj = item.Object;
														if( CreateObjectFilterEqual( objectType, referenceToObject, obj ) && ( center.ToVector2() - obj.TransformV.Position.ToVector2() ).Length() <= minDistanceBetweenObjects )
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
										var newObject = CreateObjectByCreationData( objectType, referenceToObject );
										if( newObject != null )
										{
											var objectInSpace = newObject as Component_ObjectInSpace;
											if( objectInSpace != null )
											{
												var objectToTransform = Component_ObjectInSpace_Utility.CalculateObjectToTransform( objectInSpace );
												if( objectToTransform == null )
													objectToTransform = objectInSpace;

												var p = position.Value;
												//move up to align
												p.Z += objectToTransform.TransformV.Position.Z - objectToTransform.SpaceBounds.CalculatedBoundingBox.Minimum.Z;

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

						(var objectType, var referenceToObject) = EditorAPI.GetSelectedObjectToCreate();
						if( objectType != null )
						{
							if( MetadataManager.GetTypeOfNetType( typeof( Component_Surface ) ).IsAssignableFrom( objectType ) )
							{
								//surface

								Component_Surface surface = null;
								{
									var componentType = objectType as Metadata.ComponentTypeInfo;
									if( componentType != null && componentType.BasedOnObject != null )
										surface = componentType.BasedOnObject as Component_Surface;
								}

								if( surface != null )
								{
									var surfaceAllMeshesSet = new ESet<Component_Mesh>( surface.GetAllMeshes() );

									var bounds = new Bounds( center );
									bounds.Expand( toolRadius );

									var resultItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, bounds );
									Scene.GetObjectsInSpace( resultItem );

									var objectsToDelete = new List<Component>( resultItem.Result.Length );
									foreach( var item in resultItem.Result )
									{
										var obj = item.Object;

										if( ( center.ToVector2() - obj.TransformV.Position.ToVector2() ).Length() <= toolRadius )
										{
											var meshInSpace = obj as Component_MeshInSpace;
											if( meshInSpace != null && surfaceAllMeshesSet.Contains( meshInSpace.Mesh ) )
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
								if( typeWillCreated != null && MetadataManager.GetTypeOfNetType( typeof( Component_ObjectInSpace ) ).IsAssignableFrom( typeWillCreated ) )
								{
									var bounds = new Bounds( center );
									bounds.Expand( toolRadius );

									var resultItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, typeWillCreated, true, bounds );
									Scene.GetObjectsInSpace( resultItem );

									var objectsToDelete = new List<Component>( resultItem.Result.Length );
									foreach( var item in resultItem.Result )
									{
										var obj = item.Object;

										if( CreateObjectFilterEqual( objectType, referenceToObject, obj ) && ( center.ToVector2() - obj.TransformV.Position.ToVector2() ).Length() <= toolRadius )
											objectsToDelete.Add( obj );
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

		void CreateByBrushPaintingTick( Viewport viewport, float delta )
		{
			if( CreateObjectsMode == CreateObjectsModeEnum.Brush && createByBrushPainting && !viewport.MouseRelativeMode )
			{
				createByBrushPaintingTimer -= delta;
				while( createByBrushPaintingTimer < 0 )
				{
					CreateByBrushPaintingPaint( viewport );
					createByBrushPaintingTimer += .05f;
				}
			}
		}

		bool CreateByBrushGetCenter( Viewport viewport, Component_GroupOfObjects destination, out Vector3 center )
		{
			var ray = viewport.CameraSettings.GetRayByScreenCoordinates( viewport.MousePosition );

			if( destination != null )
			{
				//when destination != null

				double? minLength = null;
				Vector3 minPosition = Vector3.Zero;

				var baseObjects = destination.GetBaseObjects();
				foreach( var baseObject in baseObjects )
				{
					//terrain
					var terrain = baseObject as Component_Terrain;
					if( terrain != null )
					{
						if( terrain.GetPositionByRay( ray, false, out var position ) )
						{
							var length = ( position - ray.Origin ).Length();
							if( minLength == null || length < minLength.Value )
							{
								minLength = length;
								minPosition = position;
							}
						}
					}

					//mesh in space
					var meshInSpace = baseObject as Component_MeshInSpace;
					if( meshInSpace != null )
					{
						if( meshInSpace.RayCast( ray, Component_Mesh.CompiledData.RayCastMode.Auto, out var scale, out _ ) )
						{
							var position = ray.GetPointOnRay( scale );
							var length = ( position - ray.Origin ).Length();
							if( minLength == null || length < minLength.Value )
							{
								minLength = length;
								minPosition = position;
							}
						}
					}

					////group of objects
					//var groupOfObjects = baseObject as Component_GroupOfObjects;
					//if( groupOfObjects != null )
					//{
					//}

				}

				if( minLength != null )
				{
					center = minPosition;
					return true;
				}
			}
			else
			{
				//when toGroupOfObjects == null
				var result = CalculateCreateObjectPositionUnderCursor( Viewport, allowSnap: false );
				if( result.found )
				{
					center = result.position;
					return true;
				}
			}

			center = Vector3.Zero;
			return false;
		}

		void CreateByBrushRender( Viewport viewport )
		{
			if( CreateObjectsMode == CreateObjectsModeEnum.Brush && createByBrushEntered && !viewport.MouseRelativeMode )
			{
				var toGroupOfObjects = CreateObjectGetDestinationSelected() as Component_GroupOfObjects;

				if( CreateByBrushGetCenter( viewport, toGroupOfObjects, out var center ) )
				{
					bool deleting;
					if( createByBrushPainting )
						deleting = createByBrushPaintingDeleting;
					else
						deleting = ( ModifierKeys & Keys.Shift ) != 0;

					//!!!!в настройки редактора
					var color = !deleting ? new ColorValue( 1, 1, 0 ) : new ColorValue( 1, 0, 0 );
					viewport.Simple3DRenderer.SetColor( color );

					double radius = CreateObjectsBrushRadius;

					const double step = Math.PI / 32;
					Vector3 lastPos = Vector3.Zero;
					for( double angle = 0; angle <= Math.PI * 2 + step / 2; angle += step )
					{
						var position = new Vector3( center.X + Math.Cos( angle ) * radius, center.Y + Math.Sin( angle ) * radius, 0 );
						position.Z = Component_Scene_Utility.CalculateObjectPositionZ( Scene, toGroupOfObjects, center.Z, position.ToVector2() ).positionZ;

						if( angle != 0 )
						{
							const float zOffset = 0.2f;// .3f;
							viewport.Simple3DRenderer.AddLine( lastPos + new Vector3( 0, 0, zOffset ), position + new Vector3( 0, 0, zOffset ) );
						}

						lastPos = position;
					}



					//viewport.Simple3DRenderer.AddSphere( new Sphere( center, 0.1 ) );

					//{
					//	var destination = CreateObjectGetDestination();
					//	if( destination != null )
					//	{
					//		var bounds = new Bounds( center );
					//		bounds.Expand( radius );

					//		var resultItem = new Component_GroupOfObjects.GetObjectsItem( Component_GroupOfObjects.GetObjectsItem.CastTypeEnum.All, null, false, bounds );
					//		destination.GetObjects( resultItem );

					//		foreach( var item in resultItem.Result )
					//		{
					//			var index = item.Object;

					//			ref var objectMesh = ref destination.ObjectsMeshGetData( index );
					//			viewport.Simple3DRenderer.AddSphere( new Sphere( objectMesh.Position, 0.1 ) );
					//		}

					//	}
					//}
				}
			}
		}

		void UpdateCreateObjectsDestinationCachedList()
		{
			if( Time.Current > createObjectsDestinationLastUpdateTime + 1.0 )
			{
				createObjectsDestinationLastUpdateTime = Time.Current;

				var list = new List<Component>();
				foreach( var obj in Scene.GetComponents( checkChildren: true, onlyEnabledInHierarchy: true ) )
				{
					if( ( obj is Component_GroupOfObjects obj2 && obj2.EditorAllowUsePaintBrush ) || obj is Component_Layer )
						list.Add( obj );
				}
				//var list = Scene.GetComponents<Component_GroupOfObjects>( checkChildren: true, onlyEnabledInHierarchy: true );

				createObjectsDestinationCachedList.Clear();
				createObjectsDestinationCachedList.Add( (null, EditorLocalization.Translate( "General", "Root" )) );
				foreach( var obj in list )
					createObjectsDestinationCachedList.Add( (obj, obj.Name) );
			}
		}

		Component CreateObjectGetDestinationSelected()
		{
			if( createObjectsDestinationSelected != null )
			{
				if( createObjectsDestinationSelected.Disposed || createObjectsDestinationSelected.ParentRoot != Scene )
					createObjectsDestinationSelected = null;
			}

			return createObjectsDestinationSelected;

			//Component_GroupOfObjects result = null;

			//var action = EditorActions.GetByName( "Create Objects Destination" );
			//if( action != null )
			//{
			//	var selectedIndex = action.ListBox.SelectedIndex;
			//	if( selectedIndex >= 0 && selectedIndex < createObjectsDestinationCachedList.Count )
			//	{
			//		var obj = createObjectsDestinationCachedList[ selectedIndex ].Item1;
			//		if( obj != null && obj.EnabledInHierarchy )
			//			result = obj;
			//	}
			//}

			//return result;
		}

		void UpdateTerrainPaintLayersCachedList()
		{
			if( Time.Current > terrainPaintLayersLastUpdateTime + 1.0 )
			{
				terrainPaintLayersLastUpdateTime = Time.Current;

				var layers = new ESet<Component_PaintLayer>();
				{
					var terrains = Scene.GetComponents<Component_Terrain>( checkChildren: true );
					foreach( var terrain in terrains )
					{
						foreach( var layer in terrain.GetComponents<Component_PaintLayer>() )
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
			if( action != null )
			{
				var newIndex = action.ListBox.SelectedIndex;
				if( newIndex >= 0 && newIndex < terrainPaintLayersCachedList.Count )
					terrainPaintLayersSelected = terrainPaintLayersCachedList[ newIndex ].Obj;
				else
					terrainPaintLayersSelected = null;
			}
		}

		public Component_PaintLayer TerrainPaintLayersGetSelected()
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

			var radius = ProjectSettings.Get.SceneEditorSelectByDoubleClickRadius.Value;

			var overObject2 = overObject as Component_ObjectInSpace;
			if( overObject2 != null && radius > 0 )
			{
				var sphere = new Sphere( overObject2.TransformV.Position, radius );
				var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );
				Scene.GetObjectsInSpace( getObjectsItem );

				foreach( var item in getObjectsItem.Result )
				{
					var obj = item.Object;
					if( obj.VisibleInHierarchy && obj.CanBeSelectedInHierarchy && ( obj.TransformV.Position - overObject2.TransformV.Position ).Length() <= radius )
					{
						if( obj.BaseType == overObject2.BaseType )
						{
							//MeshInSpace specific
							var meshInSpace1 = obj as Component_MeshInSpace;
							var meshInSpace2 = overObject2 as Component_MeshInSpace;
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
			Component_Terrain terrain = null;

			foreach( var obj in SelectedObjects )
			{
				var terrain2 = obj as Component_Terrain;
				if( terrain2 != null )
					terrain = terrain2;

				var layer = obj as Component_PaintLayer;
				if( layer != null )
				{
					var terrain3 = layer.Parent as Component_Terrain;
					if( terrain3 != null )
						terrain = terrain3;
				}
			}

			if( terrain != null )
			{
				var list = new List<(Component_PaintLayer, int)>();

				foreach( var layer in terrain.GetComponents<Component_PaintLayer>() )
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

				var list2 = new List<(Component_PaintLayer, int)>( list );

				CollectionUtility.InsertionSort( list2, delegate ( (Component_PaintLayer, int) v1, (Component_PaintLayer, int) v2 )
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
					lines.Add( "The layers of this terrain are displayed in a different sequence than they are defined" );
					lines.Add( "in the list because the materials of the layers drawn at different stages of rendering" );
					lines.Add( "(deferred, opacity forward, transparent)." );
					//lines.Add( "The layers of this terrain are displayed in a different sequence than they are defined in the list because the materials of the layers drawn at different stages of rendering (deferred, opacity forward, transparent)." );
				}
			}
		}

	}
}
