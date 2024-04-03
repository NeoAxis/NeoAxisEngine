#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Toolkit;
using System.Linq;

namespace NeoAxis.Editor
{
	public partial class UIControlEditor : DocumentWindowWithViewport
	{
		//!!!!рисовать по бокам что-то (стрелки), если ноды за пределами экрана есть
		//!!!!в preview по идее можно более наглядно показывать, что где-то еще за экраном есть (типа как миникарта в старкрафте)

		//!!!!need?
		//!!!!rename
		//!!!!а это не в стиле? что еще туда
		FontComponent controlFont;
		//!!!!not used, maybe need
		//EngineFont nodeFontValue;
		//EngineFont controlFontToolTip;
		//EngineFont controlFontComment;

		static float[] zoomTable = new float[] { .1f, .2f, .35f, .5f, .6f, .7f, .8f, .9f, 1, 1.1f, 1.2f, 1.3f, 1.5f, 1.75f, 2.0f, 2.5f, 3.0f };

		//depending system DPI
		float cellSize;

		//Scrolling
		bool scrollView_Enabled;
		bool scrollView_Activated;
		Vector2 scrollView_StartScrollPosition;
		Vector2I scrollView_StartMousePositionInPixels;
		Vector2 scrollView_StartMousePositionInScreen;

		//Select by rectangle
		bool selectByRectangle_Enabled;
		bool selectByRectangle_Activated;
		Vector2 selectByRectangle_StartPosInScreen;
		//Vec2 selectByRectangle_StartPosInUnits;
		Vector2 selectByRectangle_LastMousePositionInScreen;
		//Vec2 selectByRectangle_LastMousePositionInUnits;

		//control move
		bool controlMove_Enabled;
		bool controlMove_Activated;
		bool controlMove_Cloned;
		Vector2I controlMove_StartMousePositionInPixels;
		Vector2 controlMove_StartMousePositionInScreen;
		//Vec2 controlMove_StartMousePositionInUnits;
		UIControl controlMove_StartOverControl;
		//!!!!проверять не удалились ли. или запретить удаление. где еще так
		ESet<UIControl> controlMove_Controls;
		Dictionary<UIControl, StartPosition> controlMove_StartPositions;
		MoveModeEnum controlMove_Mode;

		//drag and drop
		bool dragDropEntered;
		Component dragDropObject;
		//!!!!
		DragDropSetReferenceData dragDropSetReferenceData;
		bool dragDropSetReferenceDataCanSet;
		//!!!!
		//string[] dragDropSetReferenceDataCanSetReferenceValues;

		ImageComponent renderTexture;
		UIControl renderTextureControl;

		bool needRecreateDisplayObject;

		///////////////////////////////////////////

		class StartPosition
		{
			public Reference<UIMeasureValueVector2> size;
			public Reference<EHorizontalAlignment> horizontalAlignment;
			public Reference<EVerticalAlignment> verticalAlignment;
			public Reference<UIMeasureValueRectangle> margin;
		}

		///////////////////////////////////////////

		[Flags]
		enum MoveModeEnum
		{
			Move = 1,
			ResizeLeft = 2,
			ResizeTop = 4,
			ResizeRight = 8,
			ResizeBottom = 16,
		}

		///////////////////////////////////////////

		public UIControlEditor()
		{
			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			CalculateCellSize();

			//UpdateFonts();

			//!!!!было
			//Text = GetDemandTitle();
		}

		[Browsable( false )]
		public UIControl Control
		{
			get { return ObjectOfWindow as UIControl; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( Document2 != null )
				Document2.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;

			//if( ObjectOfWindow != null )
			//	SelectObjects( new object[] { ObjectOfWindow } );
		}

		protected override void OnDestroy()
		{
			if( Document2 != null )
				Document2.UndoSystem.ListOfActionsChanged -= UndoSystem_ListOfActionsChanged;

			base.OnDestroy();
		}

		////!!!!
		//string GetDemandTitle()
		//{
		//	return Control.Name;
		//}

		private void GraphDocument_FormClosing( object sender, FormClosingEventArgs e )
		{
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			//!!!!было
			//if( Text != GetDemandTitle() )
			//	Text = GetDemandTitle();

			//!!!!было
			//if( ActiveForm == BlueprintEditorForm.Instance )
			//	renderTargetUserControl1.AutomaticUpdateFPS = IsScrollingViewActivated() ? 100 : 50;
			//else
			//	renderTargetUserControl1.AutomaticUpdateFPS = 0;
		}

		//public void UpdateEditingFocusedData()
		//{
		//   logicClass.CustomScriptCode = scintilla1.Text;
		//}

		//public void UpdateFonts()
		//{
		//}

		public float GetZoom()
		{
			if( Control.EditorZoomIndex >= 0 && Control.EditorZoomIndex < zoomTable.Length )
				return zoomTable[ Control.EditorZoomIndex ];
			return 1;
		}

		double GetScreenCellSizeX()
		{
			return (double)cellSize / (double)ViewportControl2.Viewport.SizeInPixels.X;
		}

		double GetScreenCellSizeY()
		{
			return (double)cellSize / (double)ViewportControl2.Viewport.SizeInPixels.Y;
		}

		double GetEditorScrollPositionX()
		{
			return Control.EditorScrollPosition.X - ConvertScreenToUnitX( 0.5, false );
		}

		double GetEditorScrollPositionY()
		{
			return Control.EditorScrollPosition.Y - ConvertScreenToUnitY( 0.5, false );
		}

		public double ConvertUnitToScreenX( double posX )
		{
			double screen = ( posX - GetEditorScrollPositionX() ) * GetScreenCellSizeX();
			screen *= GetZoom();
			return screen;
		}

		public double ConvertUnitToScreenY( double posY )
		{
			double screen = ( posY - GetEditorScrollPositionY() ) * GetScreenCellSizeY();
			screen *= GetZoom();
			return screen;
		}

		public Vector2 ConvertUnitToScreen( Vector2 vector )
		{
			return new Vector2(
				ConvertUnitToScreenX( vector.X ),
				ConvertUnitToScreenY( vector.Y ) );
		}

		public Rectangle ConvertUnitToScreen( Rectangle rect )
		{
			return new Rectangle(
				ConvertUnitToScreenX( rect.Left ),
				ConvertUnitToScreenY( rect.Top ),
				ConvertUnitToScreenX( rect.Right ),
				ConvertUnitToScreenY( rect.Bottom ) );
		}

		public double ConvertScreenToUnitX( double screenX, bool applyScrollPosition )
		{
			double v = screenX / GetScreenCellSizeX() / GetZoom();
			if( applyScrollPosition )
				v += GetEditorScrollPositionX();
			return v;
		}

		public double ConvertScreenToUnitY( double screenY, bool applyScrollPosition )
		{
			double v = screenY / GetScreenCellSizeY() / GetZoom();
			if( applyScrollPosition )
				v += GetEditorScrollPositionY();
			return v;
		}

		public Vector2 ConvertScreenToUnit( Vector2 screen, bool applyScrollPosition )
		{
			return new Vector2(
				ConvertScreenToUnitX( screen.X, applyScrollPosition ),
				ConvertScreenToUnitY( screen.Y, applyScrollPosition ) );
		}

		public RectangleI GetVisibleCells()
		{
			Vector2I from = ConvertScreenToUnit( Vector2.Zero, true ).ToVector2I() - new Vector2I( 1, 1 );
			Vector2I to = ConvertScreenToUnit( Vector2.One, true ).ToVector2I() + new Vector2I( 1, 1 );
			return new RectangleI( from, to );
		}

		public Rectangle SelectByRectangle_GetRectangleInScreen()
		{
			Rectangle rect = new Rectangle( selectByRectangle_StartPosInScreen );
			rect.Add( selectByRectangle_LastMousePositionInScreen );
			return rect;
		}

		//public Rect SelectByRectangle_GetRectangleInUnits()
		//{
		//	Rect rect = new Rect( selectByRectangle_StartPosInUnits );
		//	rect.Add( selectByRectangle_LastMousePositionInUnits );
		//	return rect;
		//}

		void UpdateFonts( CanvasRenderer renderer )
		{
			if( controlFont == null )
				controlFont = ResourceManager.LoadResource<FontComponent>( @"Base\Fonts\FlowGraphEditor.ttf" );
		}

		//void UpdateFontSize( CanvasRenderer renderer )
		//{
		//	int height = renderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
		//	float screenCellSize = (float)cellSize / (float)height;
		//	float demandFontHeight = screenCellSize * GetZoom();

		//	if( controlFont == null || controlFont.Height != demandFontHeight )
		//	{
		//		controlFont = EngineFontManager.Instance.LoadFont( "FlowGraphEditor", demandFontHeight );
		//		//nodeFontValue = EngineFontManager.Instance.LoadFont( "FlowchartEditor", demandFontHeight * .8f );
		//	}

		//	float zoom = GetZoom();
		//	if( zoom < .5f )
		//		zoom = .5f;
		//	float demandFontCommentHeight = screenCellSize * zoom * 1.4f;
		//	if( controlFontComment == null || controlFontComment.Height != demandFontCommentHeight )
		//		controlFontComment = EngineFontManager.Instance.LoadFont( "FlowGraphEditor", demandFontCommentHeight );
		//}

		private void renderTargetUserControl1_KeyUp( object sender, KeyEventArgs e )
		{
			//!!!!!
			//if( e.KeyCode == Keys.Apps )
			//   ShowContextMenu( new Point( 0, 0 ) );
		}

		void ShowContextMenu()//Point locationPoint )
		{
			var items = new List<KryptonContextMenuItemBase>();

			//!!!!
			//!!!!где ниже не только для одного можно?
			Component oneSelectedComponent = null;
			{
				if( SelectedObjects.Length == 1 )
					oneSelectedComponent = SelectedObjects[ 0 ] as Component;
			}

			//Editor
			{
				//!!!!кнопками открывать еще, рядом с "..."

				var item = new KryptonContextMenuItem( TranslateContextMenu( "Editor" ), EditorResourcesCache.Edit, delegate ( object s, EventArgs e2 )
				{
					//!!!!
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
					TryNewObject( type );
				} );

				//KryptonContextMenuItem item = new KryptonContextMenuItem( Translate( "New object" ), Properties.Resources.New_16, delegate ( object s, EventArgs e2 )
				//{
				//	TryNewObject();
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
					EditorUtility2.ExportComponentToFile( oneSelectedComponent );
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

			EditorContextMenuWinForms.AddActionsToMenu( EditorContextMenuWinForms.MenuTypeEnum.Document, items );

			EditorContextMenuWinForms.Show( items, this );
		}

		(UIControl, MoveModeEnum) GetMouseOverObject( bool onlyCanBeSelected )
		{
			var viewport = ViewportControl2.Viewport;
			Vector2 mouse = viewport.MousePosition;

			var resizeBorderSize = new Vector2( EditorAPI2.DPI / 96.0 * 4.0, EditorAPI2.DPI / 96.0 * 4.0 ) / viewport.SizeInPixels.ToVector2();

			foreach( var control in GetControls( false ).GetReverse() )
			{
				bool skip = false;
				if( onlyCanBeSelected && !IsCanBeSelected( control ) )
					skip = true;
				if( dragDropObject != null && ( control == dragDropObject || control.GetAllParents().Contains( dragDropObject ) ) )
					skip = true;
				if( !control.EnabledInHierarchy )
					skip = true;
				if( !control.VisibleInHierarchy )
					skip = true;

				if( !skip )
				{
					var rect = GetControlScreenRectangle( control );
					rect.Expand( resizeBorderSize / 2 );
					if( rect.Contains( mouse ) )
					{
						MoveModeEnum mode = 0;

						if( mouse.X < rect.Left + resizeBorderSize.X )
							mode |= MoveModeEnum.ResizeLeft;
						else if( mouse.X > rect.Right - resizeBorderSize.X )
							mode |= MoveModeEnum.ResizeRight;

						if( mouse.Y < rect.Top + resizeBorderSize.Y )
							mode |= MoveModeEnum.ResizeTop;
						else if( mouse.Y > rect.Bottom - resizeBorderSize.Y )
							mode |= MoveModeEnum.ResizeBottom;

						if( mode == 0 )
							mode = MoveModeEnum.Move;

						return (control, mode);
					}
				}
			}

			return (null, MoveModeEnum.Move);
		}

		string TranslateContextMenu( string text )
		{
			return EditorContextMenuWinForms.Translate( text );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );
		}

		protected override void ViewportControl_ViewportDestroyed( EngineViewportControl sender )
		{
			RenderTextureDestroy();

			base.ViewportControl_ViewportDestroyed( sender );
		}

		protected override void Viewport_KeyDown( Viewport viewport, KeyEvent e, ref bool handled )
		{
			base.Viewport_KeyDown( viewport, e, ref handled );
			if( handled )
				return;
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
		}

		protected override void Viewport_MouseDown( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			base.Viewport_MouseDown( viewport, button, ref handled );
			if( handled )
				return;

			if( button == EMouseButtons.Left )
			{
				var mouseOverObjectData = GetMouseOverObject( true );
				var mouseOverObject = mouseOverObjectData.Item1;
				var mouseOverObjectMoveMode = mouseOverObjectData.Item2;

				if( mouseOverObject != null )
				{
					//prepare to control moving

					Vector2 mouse = viewport.MousePosition;
					Vector2I mouseInPixels = ( mouse * viewport.SizeInPixels.ToVector2() ).ToVector2I();

					controlMove_Enabled = true;
					controlMove_Activated = false;
					controlMove_Cloned = false;
					controlMove_StartOverControl = mouseOverObject;
					controlMove_StartMousePositionInPixels = mouseInPixels;
					controlMove_StartMousePositionInScreen = mouse;
					//controlMove_StartMousePositionInUnits = ConvertScreenToUnit( mouse, true );

					controlMove_Controls = new ESet<UIControl>();
					controlMove_Controls.Add( mouseOverObject );

					foreach( var selectedObject in SelectedObjectsSet )
					{
						var control = selectedObject as UIControl;
						if( control != null )
							controlMove_Controls.AddWithCheckAlreadyContained( control );
					}

					controlMove_StartPositions = new Dictionary<UIControl, StartPosition>();
					foreach( var control in controlMove_Controls )
					{
						var start = new StartPosition();
						start.size = control.Size;
						start.horizontalAlignment = control.HorizontalAlignment;
						start.verticalAlignment = control.VerticalAlignment;
						start.margin = control.Margin;

						//!!!!
						//start.screenPosition = GetControlScreenRectangle( control ).LeftTop;
						//start.screenPosition = control.GetScreenPosition();

						controlMove_StartPositions.Add( control, start );
					}

					controlMove_Mode = mouseOverObjectMoveMode;

					handled = true;
					return;
				}
				else
				{
					//activate selection by rectangle
					selectByRectangle_Enabled = true;
					selectByRectangle_Activated = false;
					selectByRectangle_StartPosInScreen = viewport.MousePosition;
					//selectByRectangle_StartPosInUnits = ConvertScreenToUnit( selectByRectangle_StartPosInScreen, true );
					selectByRectangle_LastMousePositionInScreen = selectByRectangle_StartPosInScreen;
					//selectByRectangle_LastMousePositionInUnits = selectByRectangle_StartPosInUnits;

					handled = true;
					return;
				}
			}

			//scroll view
			if( button == EMouseButtons.Right )
			{
				Vector2 mouse = viewport.MousePosition;
				Vector2I mouseInPixels = ( mouse * viewport.SizeInPixels.ToVector2() ).ToVector2I();

				scrollView_Enabled = true;
				scrollView_Activated = false;
				scrollView_StartScrollPosition = Control.EditorScrollPosition;
				scrollView_StartMousePositionInPixels = mouseInPixels;
				scrollView_StartMousePositionInScreen = mouse;

				handled = true;
				return;
			}
		}

		protected override void Viewport_MouseUp( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			base.Viewport_MouseUp( viewport, button, ref handled );

			//below:
			//controlMove
			//scrollView
			//select by rectangle
			//context menu

			var mouseOverObject = GetMouseOverObject( true ).Item1;

			var selectedObjects = new ESet<object>( SelectedObjectsSet );

			//!!!!это как в сцене может сделать. чтобы, если не двигали, то выделять
			//!!!!проверку, чтобы ниже с ректанглом не пересеклось?
			if( button == EMouseButtons.Left )
			{
				//update selected objects
				bool allowSelect = true;

				//!!!!это как в сцене может сделать. чтобы, если не двигали, то выделять
				if( controlMove_Activated )
					allowSelect = false;
				if( allowSelect )
				{
					bool shiftPressed = ( Form.ModifierKeys & Keys.Shift ) != 0;
					if( !shiftPressed )
						selectedObjects.Clear();

					//select control
					if( mouseOverObject != null )
					{
						if( !selectedObjects.Contains( mouseOverObject ) )
							selectedObjects.Add( mouseOverObject );
						else
							selectedObjects.Remove( mouseOverObject );
					}
				}

				//control move
				if( controlMove_Activated )
				{
					//undo

					//!!!!!!отменять, если отменили по Escape

					if( !controlMove_Cloned )
					{
						//changed

						var undoItems = new List<UndoActionPropertiesChange.Item>();

						foreach( var control in controlMove_Controls )
						{
							var oldValue = controlMove_StartPositions[ control ];

							{
								var p = (Metadata.Property)MetadataManager.GetTypeOfNetType( control.GetType() ).MetadataGetMemberBySignature( "property:Size" );
								undoItems.Add( new UndoActionPropertiesChange.Item( control, p, oldValue.size, null ) );
							}
							{
								var p = (Metadata.Property)MetadataManager.GetTypeOfNetType( control.GetType() ).MetadataGetMemberBySignature( "property:HorizontalAlignment" );
								undoItems.Add( new UndoActionPropertiesChange.Item( control, p, oldValue.horizontalAlignment, null ) );
							}
							{
								var p = (Metadata.Property)MetadataManager.GetTypeOfNetType( control.GetType() ).MetadataGetMemberBySignature( "property:VerticalAlignment" );
								undoItems.Add( new UndoActionPropertiesChange.Item( control, p, oldValue.verticalAlignment, null ) );
							}
							{
								var p = (Metadata.Property)MetadataManager.GetTypeOfNetType( control.GetType() ).MetadataGetMemberBySignature( "property:Margin" );
								undoItems.Add( new UndoActionPropertiesChange.Item( control, p, oldValue.margin, null ) );
							}
						}

						if( undoItems.Count != 0 )
						{
							var action = new UndoActionPropertiesChange( undoItems.ToArray() );
							Document2.UndoSystem.CommitAction( action );
							Document2.Modified = true;
						}
					}
					else
					{
						//cloned
						var action = new UndoActionComponentCreateDelete( Document2, controlMove_Controls.ToArray(), true );
						Document2.UndoSystem.CommitAction( action );
						Document2.Modified = true;

						//update selected objects to update Settings Window
						SelectObjects( SelectedObjects, forceUpdate: true );
					}
				}
				controlMove_Enabled = false;
				controlMove_Activated = false;
				controlMove_Cloned = false;
				controlMove_StartOverControl = null;
				controlMove_Controls = null;
				controlMove_StartPositions = null;
				controlMove_StartMousePositionInPixels = Vector2I.Zero;
				controlMove_StartMousePositionInScreen = Vector2.Zero;
				//controlMove_StartMousePositionInUnits = Vec2.Zero;
				controlMove_Mode = MoveModeEnum.Move;
			}

			//select by rectangle
			if( button == EMouseButtons.Left )
			{
				if( selectByRectangle_Enabled )
				{
					if( selectByRectangle_Activated )
					{
						bool shiftPressed = ( Form.ModifierKeys & Keys.Shift ) != 0;
						if( !shiftPressed )
							selectedObjects.Clear();

						if( selectByRectangle_Activated )
						{
							foreach( var control in SelectByRectangle_GetControls() )
								selectedObjects.AddWithCheckAlreadyContained( control );
						}

						if( selectByRectangle_Activated )
							handled = true;
					}

					selectByRectangle_Enabled = false;
					selectByRectangle_Activated = false;
				}
			}

			//scroll view
			if( button == EMouseButtons.Right )
			{
				if( scrollView_Activated )
					handled = true;

				scrollView_Enabled = false;
				scrollView_Activated = false;
			}

			//update selected objects
			SelectObjects( selectedObjects );

			//context menu
			if( !handled && button == EMouseButtons.Right )
				ShowContextMenu();
		}

		protected override void Viewport_MouseDoubleClick( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			base.Viewport_MouseDoubleClick( viewport, button, ref handled );
			if( handled )
				return;
		}

		protected override void Viewport_MouseMove( Viewport viewport, Vector2 mouse )//, ref bool handled )
		{
			base.Viewport_MouseMove( viewport, mouse );//, ref handled );

			//!!!!!handled

			if( controlMove_Enabled && !controlMove_Activated )
			{
				Vector2I mouseInPixels = ( mouse * viewport.SizeInPixels.ToVector2() ).ToVector2I();
				Vector2I diff = controlMove_StartMousePositionInPixels - mouseInPixels;
				if( Math.Abs( diff.X ) > 2 || Math.Abs( diff.Y ) > 2 )
				{
					controlMove_Activated = true;

					//Clone
					if( ( ModifierKeys & Keys.Shift ) != 0 )
					{
						var nodeMove_StartOverNodeSource = controlMove_StartOverControl;
						var nodeMove_ControlsSource = controlMove_Controls;
						var nodeMove_StartPositionsSource = controlMove_StartPositions;

						controlMove_StartOverControl = null;
						controlMove_Controls = new ESet<UIControl>();
						controlMove_StartPositions = new Dictionary<UIControl, StartPosition>();

						//!!!!нужно как-то разом клонировать, чтобы ссылки остались?

						foreach( var sourceControl in nodeMove_ControlsSource )
						{
							var control = (UIControl)EditorUtility.CloneComponent( sourceControl );

							if( nodeMove_StartOverNodeSource == sourceControl )
								controlMove_StartOverControl = control;
							controlMove_Controls.Add( control );
							controlMove_StartPositions[ control ] = nodeMove_StartPositionsSource[ sourceControl ];
						}

						controlMove_Cloned = true;

						SelectObjects( controlMove_Controls.ToArray(), updateSettingsWindowSelectObjects: false );

						//add screen message
						EditorUtility.ShowScreenNotificationObjectsCloned( controlMove_Controls.Count );
					}
				}
			}
			if( controlMove_Activated )
			{
				Vector2 offsetInScreen = ConvertMainScreenToPreviewScreen( viewport.MousePosition - controlMove_StartMousePositionInScreen, true );

				foreach( var control in controlMove_Controls )
				{
					var start = controlMove_StartPositions[ control ];
					var startMargin = start.margin.Value;
					var startSize = start.size.Value;

					var control2 = GetPreviewControlByMain( control );
					if( control2 == null )
						control2 = control;

					Vector2 marginOffset = Vector2.Zero;
					if( startMargin.Measure == UIMeasure.Parent )
					{
						if( control2.ParentControl != null )
							marginOffset = offsetInScreen * control2.ParentControl.GetScreenSize();
					}
					else
						marginOffset = control2.ConvertOffset( new UIMeasureValueVector2( UIMeasure.Screen, offsetInScreen ), startMargin.Measure );

					//!!!!
					Vector2 sizeOffset = Vector2.Zero;
					if( startSize.Measure == UIMeasure.Parent )
					{
						if( control2.ParentControl != null )
							sizeOffset = offsetInScreen * control2.ParentControl.GetScreenSize();
					}
					else
						sizeOffset = control2.ConvertOffset( new UIMeasureValueVector2( UIMeasure.Screen, offsetInScreen ), startSize.Measure );

					Rectangle margin = startMargin.Value;
					Vector2 size = startSize.Value;
					var marginLeft = false;
					var marginRight = false;
					var marginTop = false;
					var marginBottom = false;
					var sizeX = false;
					var sizeY = false;

					switch( control.HorizontalAlignment.Value )
					{
					case EHorizontalAlignment.Left:
						{
							if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeLeft ) )
							{
								margin.Left += marginOffset.X;
								marginLeft = true;
								size.X -= sizeOffset.X;
								sizeX = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeRight ) )
							{
								size.X += sizeOffset.X;
								sizeX = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.Move ) )
							{
								margin.Left += marginOffset.X;
								marginLeft = true;
							}
						}
						break;

					case EHorizontalAlignment.Center:
						{
							if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeLeft ) )
							{
								margin.Left += marginOffset.X;
								marginLeft = true;
								size.X -= sizeOffset.X;
								sizeX = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeRight ) )
							{
								margin.Right -= marginOffset.X;
								marginRight = true;
								size.X += sizeOffset.X;
								sizeX = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.Move ) )
							{
								margin.Left += marginOffset.X;
								margin.Right -= marginOffset.X;
								marginLeft = true;
								marginRight = true;
							}
						}
						break;

					case EHorizontalAlignment.Right:
						{
							if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeLeft ) )
							{
								size.X -= sizeOffset.X;
								sizeX = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeRight ) )
							{
								margin.Right -= marginOffset.X;
								marginRight = true;
								size.X += sizeOffset.X;
								sizeX = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.Move ) )
							{
								margin.Right -= marginOffset.X;
								marginRight = true;
							}
						}
						break;

					case EHorizontalAlignment.Stretch:
						{
							if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeLeft ) )
							{
								margin.Left += marginOffset.X;
								marginLeft = true;
								size.X -= sizeOffset.X;
								sizeX = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeRight ) )
							{
								margin.Right -= marginOffset.X;
								marginRight = true;
								size.X += sizeOffset.X;
								sizeX = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.Move ) )
							{
								margin.Left += marginOffset.X;
								margin.Right -= marginOffset.X;
								marginLeft = true;
								marginRight = true;
							}
						}
						break;
					}

					switch( control.VerticalAlignment.Value )
					{
					case EVerticalAlignment.Top:
						{
							if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeTop ) )
							{
								margin.Top += marginOffset.Y;
								marginTop = true;
								size.Y -= sizeOffset.Y;
								sizeY = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeBottom ) )
							{
								size.Y += sizeOffset.Y;
								sizeY = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.Move ) )
							{
								margin.Top += marginOffset.Y;
								marginTop = true;
							}
						}
						break;

					case EVerticalAlignment.Center:
						{
							if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeTop ) )
							{
								margin.Top += marginOffset.Y;
								marginTop = true;
								size.Y -= sizeOffset.Y;
								sizeY = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeBottom ) )
							{
								margin.Bottom -= marginOffset.Y;
								marginBottom = true;
								size.Y += sizeOffset.Y;
								sizeY = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.Move ) )
							{
								margin.Top += marginOffset.Y;
								margin.Bottom -= marginOffset.Y;
								marginTop = true;
								marginBottom = true;
							}
						}
						break;

					case EVerticalAlignment.Bottom:
						{
							if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeTop ) )
							{
								size.Y -= sizeOffset.Y;
								sizeY = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeBottom ) )
							{
								margin.Bottom -= marginOffset.Y;
								marginBottom = true;
								size.Y += sizeOffset.Y;
								sizeY = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.Move ) )
							{
								margin.Bottom -= marginOffset.Y;
								marginBottom = true;
							}
						}
						break;

					case EVerticalAlignment.Stretch:
						{
							if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeTop ) )
							{
								margin.Top += marginOffset.Y;
								marginTop = true;
								size.Y -= sizeOffset.Y;
								sizeY = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.ResizeBottom ) )
							{
								margin.Bottom -= marginOffset.Y;
								marginBottom = true;
								size.Y += sizeOffset.Y;
								sizeY = true;
							}
							else if( controlMove_Mode.HasFlag( MoveModeEnum.Move ) )
							{
								margin.Top += marginOffset.Y;
								margin.Bottom -= marginOffset.Y;
								marginTop = true;
								marginBottom = true;
							}
						}
						break;
					}

					// Step movement.
					if( ModifierKeys.HasFlag( Keys.Control ) )
					{
						double marginSnap = ProjectSettings.Get.UIEditor.GetUIEditorStepMovement( startMargin.Measure );
						if( marginSnap != 0 )
						{
							var v2 = margin;
							Vector2 snapVec = new Vector2( marginSnap, marginSnap );
							v2 += snapVec / 2;
							v2 /= snapVec;
							v2 = new RectangleI( (int)v2.Left, (int)v2.Top, (int)v2.Right, (int)v2.Bottom ).ToRectangle();
							v2 *= snapVec;

							if( marginLeft )
								margin.Left = v2.Left;
							if( marginTop )
								margin.Top = v2.Top;
							if( marginRight )
								margin.Right = v2.Right;
							if( marginBottom )
								margin.Bottom = v2.Bottom;
						}

						double sizeSnap = ProjectSettings.Get.UIEditor.GetUIEditorStepMovement( startSize.Measure );
						if( sizeSnap != 0 )
						{
							var v2 = size;
							Vector2 snapVec = new Vector2( sizeSnap, sizeSnap );
							v2 += snapVec / 2;
							v2 /= snapVec;
							v2 = new Vector2I( (int)v2.X, (int)v2.Y ).ToVector2();
							v2 *= snapVec;

							if( sizeX )
								size.X = v2.X;
							if( sizeY )
								size.Y = v2.Y;
						}
					}

					if( marginLeft || marginTop || marginRight || marginBottom )
						control.Margin = new UIMeasureValueRectangle( startMargin.Measure, margin );
					if( sizeX || sizeY )
						control.Size = new UIMeasureValueVector2( startSize.Measure, size );

					needRecreateDisplayObject = true;
				}
			}

			//scrolling view
			if( scrollView_Enabled )
			{
				Vector2I mouseInPixels = ( mouse * viewport.SizeInPixels.ToVector2() ).ToVector2I();
				Vector2I diff = scrollView_StartMousePositionInPixels - mouseInPixels;
				if( Math.Abs( diff.X ) > 2 || Math.Abs( diff.Y ) > 2 )
					scrollView_Activated = true;
			}
			if( scrollView_Activated )
			{
				Vector2 mouseDiff = mouse - scrollView_StartMousePositionInScreen;
				Control.EditorScrollPosition = scrollView_StartScrollPosition - ConvertScreenToUnit( mouseDiff, false );
			}

			//update select by rectangle
			if( selectByRectangle_Enabled )
			{
				Vector2 diffPixels = ( viewport.MousePosition - selectByRectangle_StartPosInScreen ) * viewport.SizeInPixels.ToVector2();
				if( Math.Abs( diffPixels.X ) >= 3 || Math.Abs( diffPixels.Y ) >= 3 )
					selectByRectangle_Activated = true;

				selectByRectangle_LastMousePositionInScreen = viewport.MousePosition;
				//selectByRectangle_LastMousePositionInUnits = ConvertScreenToUnit( viewport.MousePosition, true );
			}
		}

		protected override void Viewport_MouseRelativeModeChanged( Viewport viewport, ref bool handled )
		{
			base.Viewport_MouseRelativeModeChanged( viewport, ref handled );
		}

		protected override void Viewport_MouseWheel( Viewport viewport, int delta, ref bool handled )
		{
			base.Viewport_MouseWheel( viewport, delta, ref handled );
			if( handled )
				return;

			//!!!!handled

			Vector2 mouse = viewport.MousePosition;
			Vector2 oldUnitsOnScreen = ConvertScreenToUnit( new Vector2( 1, 1 ), false );

			//int delta = e.Delta;
			bool updated = false;

			if( delta > 0 )
			{
				int steps = delta / 120;
				if( steps == 0 )
					steps = 1;

				for( int n = 0; n < steps; n++ )
				{
					if( Control.EditorZoomIndex < zoomTable.Length - 1 )
					{
						Control.EditorZoomIndex++;
						updated = true;
					}
				}
			}
			else if( delta < 0 )
			{
				int steps = -delta / 120;
				if( steps == 0 )
					steps = 1;

				for( int n = 0; n < steps; n++ )
				{
					if( Control.EditorZoomIndex > 0 )
					{
						Control.EditorZoomIndex--;
						updated = true;
					}
				}
			}

			if( updated )
			{
				Vector2 newUnitsOnScreen = ConvertScreenToUnit( new Vector2( 1, 1 ), false );

				Vector2 oldUnitsOffsetToCursorPosition = oldUnitsOnScreen * ( mouse - new Vector2( 0.5, 0.5 ) );
				Vector2 newUnitsOffsetToCursorPosition = newUnitsOnScreen * ( mouse - new Vector2( 0.5, 0.5 ) );

				Vector2 v = Control.EditorScrollPosition;
				v += oldUnitsOffsetToCursorPosition;
				v -= newUnitsOffsetToCursorPosition;
				Control.EditorScrollPosition = v;

				AddScreenMessage( string.Format( "Zoom {0}", GetZoom() ) );
			}

			handled = true;
		}

		protected override void Viewport_Tick( Viewport viewport, float delta )
		{
			base.Viewport_Tick( viewport, delta );

			RenderTextureUpdate( delta );
		}

		protected override void Viewport_UpdateBegin( Viewport viewport )
		{
			base.Viewport_UpdateBegin( viewport );
		}

		bool IsCanBeSelected( UIControl control )
		{
			if( control.VisibleInHierarchy && control.CanBeSelectedInHierarchy && control.TypeSettingsIsPublic() )
				return true;

			return false;
		}

		List<UIControl> GetControlsByScreenRectangle( Rectangle screenRect )
		{
			var all = new List<UIControl>();
			foreach( var control in GetControls( false ) )
			{
				if( IsCanBeSelected( control ) )
				{
					var rect = GetControlScreenRectangle( control );
					if( rect.Intersects( screenRect ) )
						all.Add( control );
				}
			}

			//remove parents

			var result = new List<UIControl>( all.Count );

			var allParents = new ESet<Component>( all.Count );
			foreach( var c in all )
				allParents.AddRangeWithCheckAlreadyContained( c.GetAllParents() );

			foreach( var c in all )
				if( !allParents.Contains( c ) )
					result.Add( c );

			return result;
		}

		ESet<UIControl> SelectByRectangle_GetControls()
		{
			var result = new ESet<UIControl>();
			if( selectByRectangle_Activated )
			{
				foreach( var control in GetControlsByScreenRectangle( SelectByRectangle_GetRectangleInScreen() ) )
					result.Add( control );
			}
			return result;
		}

		bool CanSelectObjects()
		{
			if( controlMove_Activated )
				return false;
			return true;
		}

		protected virtual void RenderBackground()
		{
			var window = this;

			var viewport = window.ViewportControl2.Viewport;
			var renderer = viewport.CanvasRenderer;

			RectangleI visibleCells = window.GetVisibleCells();

			//draw background
			renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new ColorValue( .17f, .17f, .17f ) );

			//draw grid
			if( window.GetZoom() > .5f && ProjectSettings.Get.UIEditor.UIEditorDisplayGrid )
			{
				var lines = new List<CanvasRenderer.LineItem>( 256 );

				{
					ColorValue color = new ColorValue( .2f, .2f, .2f );
					for( int x = visibleCells.Left; x <= visibleCells.Right; x++ )
					{
						if( x % 10 != 0 )
						{
							var floatX = (float)window.ConvertUnitToScreenX( x );
							lines.Add( new CanvasRenderer.LineItem( new Vector2F( floatX, 0 ), new Vector2F( floatX, 1 ), color ) );
						}
					}
					for( int y = visibleCells.Top; y <= visibleCells.Bottom; y++ )
					{
						if( y % 10 != 0 )
						{
							var floatY = (float)window.ConvertUnitToScreenY( y );
							lines.Add( new CanvasRenderer.LineItem( new Vector2F( 0, floatY ), new Vector2F( 1, floatY ), color ) );
						}
					}
				}

				{
					ColorValue color = new ColorValue( .1f, .1f, .1f );
					for( int x = visibleCells.Left; x <= visibleCells.Right; x++ )
					{
						if( x % 10 == 0 )
						{
							var floatX = (float)window.ConvertUnitToScreenX( x );
							lines.Add( new CanvasRenderer.LineItem( new Vector2F( floatX, 0 ), new Vector2F( floatX, 1 ), color ) );
						}
					}
					for( int y = visibleCells.Top; y <= visibleCells.Bottom; y++ )
					{
						if( y % 10 == 0 )
						{
							var floatY = (float)window.ConvertUnitToScreenY( y );
							lines.Add( new CanvasRenderer.LineItem( new Vector2F( 0, floatY ), new Vector2F( 1, floatY ), color ) );
						}
					}
				}

				viewport.CanvasRenderer.AddLines( lines );
			}
		}

		ColorValue GetColorMultiplierSelectionState( RenderSelectionState selectionState )
		{
			switch( selectionState )
			{
			case RenderSelectionState.CanSelect: return new ColorValue( 1, 1, 0 );
			case RenderSelectionState.Selected: return new ColorValue( 0, 1, 0 );
			}
			return new ColorValue( 1, 1, 1 );
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			//!!!!по идее рисовать можно было бы в симуляции тоже

			var renderer = viewport.CanvasRenderer;

			var mouseOverObjectData = GetMouseOverObject( true );
			var mouseOverObject = mouseOverObjectData.Item1;
			var mouseOverObjectMoveMode = mouseOverObjectData.Item2;

			UpdateFonts( renderer );

			RenderBackground();
			RenderScreenRectangle();

			//update, display render target
			{
				//create, recreate
				if( renderTexture != null && renderTexture.CreateSize.Value != GetViewportDemandSize() )
					RenderTextureDestroy();
				if( renderTexture == null )
					RenderTextureCreate();

				if( needRecreateDisplayObject )
					RenderTextureControlCreate();

				//display on a screen
				if( renderTexture != null )
				{
					Rectangle rect = GetViewportRectangleInScreenCoords();
					renderer.PushTextureFilteringMode( CanvasRenderer.TextureFilteringMode.Point );
					renderer.AddQuad( rect, new Rectangle( 0, 0, 1, 1 ), renderTexture, ColorValue.One, true );
					renderer.PopTextureFilteringMode();
				}
			}

			//display drop info. highlight parent
			if( dragDropEntered && dragDropObject != null && dragDropObject is UIControl )
			{
				var parent = GetMouseOverObject( false ).Item1;
				if( parent == null )
					parent = Control;

				if( parent != null )
				{
					var rect = GetControlScreenRectangle( parent );
					var color = ProjectSettings.Get.Colors.CanSelectColor;
					renderer.AddRectangle( rect, color );
				}
			}

			//display selection of objects
			{
				//RectI visibleCells = GetVisibleCells();

				var selectByRectangleControls = SelectByRectangle_GetControls();

				foreach( var control in GetControls( true ) )
				{
					//node selection state
					var selectionStateControl = RenderSelectionState.None;

					if( dragDropEntered )
					{
						if( control == dragDropObject )
							selectionStateControl = RenderSelectionState.CanSelect;
					}
					else
					{
						if( CanSelectObjects() )
						{
							if( mouseOverObject == control )
							{
								selectionStateControl = RenderSelectionState.CanSelect;

								ViewportControl2.OneFrameChangeCursor = GetCursorByMoveMode( mouseOverObjectMoveMode );
							}

							if( selectByRectangleControls.Contains( control ) )
								selectionStateControl = RenderSelectionState.CanSelect;
						}
						//if( ( mouseOverObject == control || selectByRectangleControls.Contains( control ) ) && CanSelectObjects() )
						//	selectionStateControl = EditorRenderSelectionState.CanSelect;

						if( IsObjectSelected( control ) )
							selectionStateControl = RenderSelectionState.Selected;
					}

					if( selectionStateControl != RenderSelectionState.None )
					{
						var nodeColorMultiplierWithSelection = GetColorMultiplierSelectionState( selectionStateControl );
						//ColorValue objColorMultiplierWithSelection = GetColorMultiplierSelectionState( selectionStateControlledObject );

						var rect = GetControlScreenRectangle( control );

						//Rect nodeSelectionRectInUnits = nodeRectInUnits;
						////nodeSelectionRectInUnits.Expand( .4 );
						//Rect nodeSelectionRect = window.ConvertUnitToScreen( nodeSelectionRectInUnits );

						ColorValue color;
						if( selectionStateControl == RenderSelectionState.Selected )
							color = ProjectSettings.Get.Colors.SelectedColor;
						else
							color = ProjectSettings.Get.Colors.CanSelectColor;

						var color2 = color;
						color2.Alpha *= .5f;

						//!!!!if( control != Control )
						renderer.AddQuad( rect, color2 * nodeColorMultiplierWithSelection );
						renderer.AddRectangle( rect, color * nodeColorMultiplierWithSelection );
					}
				}
			}

			//draw selection rectangle
			if( selectByRectangle_Enabled && selectByRectangle_Activated )
			{
				Rectangle rect = new Rectangle( selectByRectangle_StartPosInScreen );
				//Rect rect = new Rect( ConvertUnitToScreen( selectByRectangle_StartPosInUnits ) );
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

			//!!!!
			////show tooltip
			//{
			//	Vec2 pixelInScreen = Vec2.One / renderer.ViewportForScreenGuiRenderer.DimensionsInPixels.Size.ToVec2();

			//	float demandFontHeight = (float)16 / (float)renderer.ViewportForScreenGuiRenderer.DimensionsInPixels.Size.Y;
			//	if( fontToolTip == null || fontToolTip.Height != demandFontHeight )
			//		fontToolTip = EngineFontManager.Instance.LoadFont( "FlowchartEditor", demandFontHeight );

			//	string text = "";
			//	if( mouseOverObject != null )
			//	{
			//		FlowchartNode node = mouseOverObject as FlowchartNode;
			//		if( node != null )
			//		{
			//			string error = node.GetSettings().Error;
			//			if( error != null )
			//				text = error;
			//		}

			//		FlowchartNode.Pin pin = mouseOverObject as FlowchartNode.Pin;
			//		if( pin != null )
			//		{
			//			//!!!!было
			//			text = pin.Type;
			//			//text = BlueprintManager.GetDisplayNameForType( pin.Type );
			//		}
			//	}

			//	if( !string.IsNullOrEmpty( text ) )
			//	{
			//		float length = fontToolTip.GetTextLength( renderer, text );
			//		Vec2 mouse = ViewportControl.Viewport.MousePosition;

			//		Rect rect =
			//			new Rect( mouse, mouse + new Vec2( length + pixelInScreen.X * 8, fontToolTip.Height + pixelInScreen.X * 2 ) ) +
			//			pixelInScreen * new Vec2( 16, 0 );

			//		renderer.AddQuad( rect, new ColorValue( .4f, .4f, .4f ) );
			//		renderer.AddText( fontToolTip, text, rect.LeftTop + pixelInScreen * new Vec2( 4, 1 ), EHorizontalAlign.Left,
			//			EVerticalAlign.Top, new ColorValue( 1, 1, 1 ) );
			//		renderer.AddRectangle( rect, new ColorValue( .6f, .6f, .6f ) );
			//	}
			//}

			if( controlMove_Enabled )
				ViewportControl2.OneFrameChangeCursor = GetCursorByMoveMode( controlMove_Mode );
		}

		protected override void Viewport_UpdateEnd( Viewport viewport )
		{
			base.Viewport_UpdateEnd( viewport );
		}

		[Browsable( false )]
		public FontComponent ControlFont
		{
			get { return controlFont; }
		}

		//[Browsable( false )]
		//public EngineFont NodeFontValue
		//{
		//	get { return nodeFontValue; }
		//}

		//[Browsable( false )]
		//public EngineFont NodeFontToolTip
		//{
		//	get { return controlFontToolTip; }
		//}

		//[Browsable( false )]
		//public EngineFont NodeFontComment
		//{
		//	get { return controlFontComment; }
		//}

		public override void EditorActionGetState( EditorActionGetStateContext context )
		{
			base.EditorActionGetState( context );

			//!!!!тут?

			switch( context.Action.Name )
			{
			case "Snap All Axes":
			case "Snap X":
			case "Snap Y":
				if( CanSnap( out _ ) )
					context.Enabled = true;
				break;
			}
		}

		public override void EditorActionClick( EditorActionClickContext context )
		{
			base.EditorActionClick( context );

			switch( context.Action.Name )
			{
			case "Snap All Axes":
			case "Snap X":
			case "Snap Y":
				Snap( context.Action );
				break;
			}
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
				parentsForNewObjects.Add( Control );
			return true;
		}

		public void TryNewObject( Metadata.TypeInfo lockType )
		{
			if( !CanNewObject( out List<Component> parentsForNewObjects ) )
				return;

			bool setCreationPosition = false;
			Vector2 creationPositionInUnits = Vector2.Zero;
			if( parentsForNewObjects.Count == 1 )//&& parentsForNewObjects[ 0 ] == Control )
			{
				var parent = parentsForNewObjects[ 0 ] as UIControl;
				if( parent != null )
				{
					setCreationPosition = true;

					var screen = ConvertMainScreenToPreviewScreen( ViewportControl2.Viewport.MousePosition );

					var parent2 = GetPreviewControlByMain( parent );
					if( parent2 == null )
						parent2 = parent;

					var diff = screen - parent2.GetScreenPosition();
					creationPositionInUnits = parent2.ConvertOffset( new UIMeasureValueVector2( UIMeasure.Screen, diff ), UIMeasure.Units );
				}
			}

			var data = new NewObjectWindow.CreationDataClass();
			data.initDocumentWindow = this;
			data.initParentObjects = new List<object>();
			data.initParentObjects.AddRange( parentsForNewObjects );

			//data.beforeCreateObjectsFunction = delegate ( NewObjectWindow window, Metadata.TypeInfo selectedType )
			//{
			//	if( nodeCreationOnFlowchart && !typeof( FlowchartNode ).IsAssignableFrom( selectedType.GetNetType() ) )
			//	{
			//		window.creationData.createdObjects = new List<object>();

			//		//create node with object inside of selected type
			//		var node = Control.CreateComponent<FlowchartNode>( -1, false );
			//		window.creationData.createdObjects.Add( node );
			//		window.creationData.createdComponentsOnTopLevel.Add( node );

			//		//create component of selected type as child of the node
			//		var c = node.CreateComponent( selectedType );

			//		//!!!!имя должно быть обязательно, чтобы ссылку настроить
			//		//!!!!!!может если нет имени, то ошибку создавать (в ReferenceUtils.CalculateThisReference)
			//		if( !window.ApplyCreationSettingsToObject( c ) )
			//			return false;

			//		window.creationData.createdObjects.Add( c );

			//		//!!!!
			//		//!!!!надо уникальное
			//		node.Name = "Node" + c.Name;

			//		//set ControlledObject of the node
			//		//!!!!как-то сложно?
			//		node.ControlledObject = new Reference<Component>( null, ReferenceUtils.CalculateThisReference( node, c ) );
			//	}

			//	return true;
			//};

			//set position
			data.additionActionBeforeEnabled = delegate ( NewObjectWindow window )
			{
				if( setCreationPosition )
				{
					foreach( var obj in data.createdComponentsOnTopLevel )
					{
						var control = obj as UIControl;
						if( control != null )
							control.Margin = new UIMeasureValueRectangle( UIMeasure.Units, creationPositionInUnits.X, creationPositionInUnits.Y, 0, 0 );
					}
				}
			};

			data.initLockType = lockType;
			data.initSupportAutoCreateAndClose = lockType != null;

			EditorAPI2.OpenNewObjectWindow( data );
		}

		public Rectangle GetViewportRectangleInUnits()
		{
			var settingsAspectRatio = ProjectSettings.Get.UIEditor.UIEditorAspectRatio.Value;

			var x = 0.9 / GetScreenCellSizeX();
			var y = 0.9 / GetScreenCellSizeY();

			Vector2 size;
			if( x < y * settingsAspectRatio )
				size = new Vector2( x, x / settingsAspectRatio );
			else
				size = new Vector2( y * settingsAspectRatio, y );

			return new Rectangle( -size / 2, size / 2 );
		}

		public Rectangle GetViewportRectangleInScreenCoords()
		{
			return ConvertUnitToScreen( GetViewportRectangleInUnits() );
		}

		void RenderScreenRectangle()
		{
			var viewport = ViewportControl2.Viewport;
			var renderer = viewport.CanvasRenderer;

			Rectangle rect = GetViewportRectangleInScreenCoords();

			//!!!!color
			var color = new ColorValue( 0.5, 0.5, 0.5, 0.5 );
			//var color = new ColorValue( 0, 0, 1, 0.5 );

			renderer.AddLine( new Vector2( 0, rect.Top ), new Vector2( 1, rect.Top ), color );
			renderer.AddLine( new Vector2( 0, rect.Bottom ), new Vector2( 1, rect.Bottom ), color );
			renderer.AddLine( new Vector2( rect.Left, 0 ), new Vector2( rect.Left, 1 ), color );
			renderer.AddLine( new Vector2( rect.Right, 0 ), new Vector2( rect.Right, 1 ), color );
			//renderer.AddRectangle( rect, color );
		}

		Vector2I GetViewportDemandSize()
		{
			var size = ( ViewportControl2.Viewport.SizeInPixels.ToVector2() * GetViewportRectangleInScreenCoords().Size ).ToVector2I();
			if( size.X < 1 )
				size.X = 1;
			if( size.Y < 1 )
				size.Y = 1;
			return size;
		}

		/////////////////////////////////////////

		//public double ConvertViewportUnitToScreenX( double posX )
		//{
		//	var screenRect = GetViewportRectangleInScreenCoords();

		//	xx;

		//	double screen = ( posX - GetEditorScrollPositionX() ) * GetScreenCellSizeX();
		//	screen *= GetZoom();
		//	return screen;
		//}

		//public double ConvertViewportUnitToScreenY( double posY )
		//{
		//	xx;

		//	double screen = ( posY - GetEditorScrollPositionY() ) * GetScreenCellSizeY();
		//	screen *= GetZoom();
		//	return screen;
		//}

		//public Vec2 ConvertViewportUnitToScreen( Vec2 vector )
		//{
		//	return new Vec2(
		//		ConvertViewportUnitToScreenX( vector.X ),
		//		ConvertViewportUnitToScreenY( vector.Y ) );
		//}

		//public Rect ConvertViewportUnitToScreen( Rect rect )
		//{
		//	return new Rect(
		//		ConvertViewportUnitToScreenX( rect.Left ),
		//		ConvertViewportUnitToScreenY( rect.Top ),
		//		ConvertViewportUnitToScreenX( rect.Right ),
		//		ConvertViewportUnitToScreenY( rect.Bottom ) );
		//}

		public double ConvertPreviewScreenToMainScreenX( double screenX )
		{
			var screenRect = GetViewportRectangleInScreenCoords();
			return screenRect.Left + screenX * screenRect.Size.X;
		}

		public double ConvertPreviewScreenToMainScreenY( double screenY )
		{
			var screenRect = GetViewportRectangleInScreenCoords();
			return screenRect.Top + screenY * screenRect.Size.Y;
		}

		public Vector2 ConvertPreviewScreenToMainScreen( Vector2 screen )
		{
			return new Vector2(
				ConvertPreviewScreenToMainScreenX( screen.X ),
				ConvertPreviewScreenToMainScreenY( screen.Y ) );
		}

		public Rectangle ConvertPreviewScreenToMainScreen( Rectangle rect )
		{
			return new Rectangle(
				ConvertPreviewScreenToMainScreenX( rect.Left ),
				ConvertPreviewScreenToMainScreenY( rect.Top ),
				ConvertPreviewScreenToMainScreenX( rect.Right ),
				ConvertPreviewScreenToMainScreenY( rect.Bottom ) );
		}

		public double ConvertMainScreenToPreviewScreenX( double screenX, bool offset = false )
		{
			var screenRect = GetViewportRectangleInScreenCoords();
			var v = screenX;
			if( !offset )
				v -= screenRect.Left;
			if( screenRect.Size.X != 0 )
				v /= screenRect.Size.X;
			return v;
		}

		public double ConvertMainScreenToPreviewScreenY( double screenY, bool offset = false )
		{
			var screenRect = GetViewportRectangleInScreenCoords();
			var v = screenY;
			if( !offset )
				v -= screenRect.Top;
			if( screenRect.Size.Y != 0 )
				v /= screenRect.Size.Y;
			return v;
		}

		public Vector2 ConvertMainScreenToPreviewScreen( Vector2 screen, bool offset = false )
		{
			return new Vector2(
				ConvertMainScreenToPreviewScreenX( screen.X, offset ),
				ConvertMainScreenToPreviewScreenY( screen.Y, offset ) );
		}

		public Rectangle ConvertMainScreenToPreviewScreen( Rectangle rect )
		{
			return new Rectangle(
				ConvertMainScreenToPreviewScreenX( rect.Left ),
				ConvertMainScreenToPreviewScreenY( rect.Top ),
				ConvertMainScreenToPreviewScreenX( rect.Right ),
				ConvertMainScreenToPreviewScreenY( rect.Bottom ) );
		}

		/////////////////////////////////////////

		void RenderTextureCreate()
		{
			RenderTextureDestroy();

			var texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
			texture.CreateType = ImageComponent.TypeEnum._2D;
			texture.CreateSize = GetViewportDemandSize();
			texture.CreateMipmaps = false;
			texture.CreateFormat = PixelFormat.A8R8G8B8;
			texture.CreateUsage = ImageComponent.Usages.RenderTarget;
			texture.CreateFSAA = 0;
			texture.Enabled = true;

			//!!!!!как проверять ошибки создания текстур? везде так
			//if( texture == null )
			//{
			//	//!!!!!
			//	Log.Fatal( "ViewportRenderingPipeline: RenderTarget_Alloc: Unable to create texture." );
			//	return null;
			//}

			var renderTexture2 = texture.Result.GetRenderTarget();
			var viewport = renderTexture2.AddViewport( true, true );
			viewport.RenderingPipelineCreate();
			viewport.RenderingPipelineCreated.UseRenderTargets = false;
			viewport.RenderingPipelineCreated.BackgroundColorOverride = new ColorValue( 0, 0, 0, 0 );

			renderTexture = texture;

			viewport.UpdateBeforeOutput += RenderTextureViewport_UpdateBeforeOutput;

			RenderTextureControlCreate();
		}

		void RenderTextureDestroy()
		{
			if( renderTexture != null )
			{
				RenderTextureControlDestroy();

				renderTexture.Dispose();
				renderTexture = null;
			}
		}

		void RenderTextureControlCreate()
		{
			RenderTextureControlDestroy();

			if( renderTexture != null )
			{
				var viewport = renderTexture.Result.GetRenderTarget().Viewports[ 0 ];

				renderTextureControl = (UIControl)Control.Clone();
				viewport.UIContainer.AddComponent( renderTextureControl );

				needRecreateDisplayObject = false;

				//!!!!new
				//!!!!new
				RenderTextureUpdate( 0.0001f );
				//RenderTextureUpdate( 0 );
			}
		}

		void RenderTextureControlDestroy()
		{
			renderTextureControl?.Dispose();
			renderTextureControl = null;
		}

		void RenderTextureUpdate( float delta )
		{
			//update render texture's viewport
			if( renderTexture != null && ViewportControl2.IsAllowRender() && !needRecreateDisplayObject )
			{
				var viewport = renderTexture.Result.GetRenderTarget().Viewports[ 0 ];
				viewport.PerformTick( delta );
				viewport.Update( true );
			}
		}

		private void RenderTextureViewport_UpdateBeforeOutput( Viewport viewport )
		{
			//!!!!
			viewport.UIContainer.PerformRenderUI( viewport.CanvasRenderer );
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			needRecreateDisplayObject = true;
		}

		private void UIControl_DocumentWindow_DragEnter( object sender, DragEventArgs e )
		{
			dragDropEntered = true;

			DragDropObjectCreate( e );

			////!!!!
			//DragDropSetReferenceData dragDropData = (DragDropSetReferenceData)e.Data.GetData( typeof( DragDropSetReferenceData ) );
			//if( dragDropData != null )
			//	dragDropSetReferenceData = dragDropData;
		}

		private void UIControl_DocumentWindow_DragOver( object sender, DragEventArgs e )
		{
			e.Effect = DragDropEffects.None;

			//!!!!пока так
			ViewportControl2?.PerformMouseMove();

			DragDropObjectUpdate();
			if( dragDropObject != null )
				e.Effect = DragDropEffects.Link;

			DragDropSetReferenceData dragDropData = (DragDropSetReferenceData)e.Data.GetData( typeof( DragDropSetReferenceData ) );
			if( dragDropData != null )
				dragDropSetReferenceData = dragDropData;
			if( dragDropSetReferenceData != null && dragDropSetReferenceDataCanSet )
				e.Effect = DragDropEffects.Link;

			//!!!!пока так
			ViewportControl2.TryRender();
		}

		private void UIControl_DocumentWindow_DragLeave( object sender, EventArgs e )
		{
			DragDropObjectDestroy();

			dragDropEntered = false;
			dragDropSetReferenceData = null;

			//!!!!пока так
			ViewportControl2.TryRender();
		}

		private void UIControl_DocumentWindow_DragDrop( object sender, DragEventArgs e )
		{
			DragDropObjectCommit();

			if( dragDropSetReferenceData != null )
			{
				if( dragDropSetReferenceDataCanSet )
				{
					//!!!!
					dragDropSetReferenceData.SetProperty( null );
					//dragDropSetReferenceData.SetProperty( dragDropSetReferenceDataCanSetReferenceValues );
					dragDropSetReferenceDataCanSet = false;
				}
				dragDropSetReferenceData = null;
			}

			dragDropEntered = false;
		}

		void DragDropObjectCreate( DragEventArgs e )
		{
			Metadata.TypeInfo objectType = null;
			//string memberFullSignature = "";
			//Component memberThisPropertySetReferenceToObject = null;
			//Component createNodeWithComponent = null;
			{
				var dragDropData = ContentBrowser.GetDroppingItemData( e.Data );
				if( dragDropData != null )
				{
					var item = dragDropData.Item;
					//!!!!не все итемы можно создать.

					//_File
					var fileItem = item as ContentBrowserItem_File;
					if( fileItem != null && !fileItem.IsDirectory )
					{
						//!!!!не делать предпросмотр для карты, т.к. долго. что еще?
						var ext = Path.GetExtension( fileItem.FullPath );
						if( ResourceManager.GetTypeByFileExtension( ext ) != null )
						{
							var res = ResourceManager.GetByName( VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath ) );

							var type = res?.PrimaryInstance?.ResultComponent?.GetProvidedType();
							if( type != null )
								objectType = type;
						}
					}

					//_Type
					var typeItem = item as ContentBrowserItem_Type;
					if( typeItem != null )
					{
						var type = typeItem.Type;

						//!!!!для ноды без объекта

						//!!!!генериковому нужно указать типы

						if( MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( type ) && !type.Abstract )
							objectType = type;
					}

					////_Member
					//var memberItem = item as ContentBrowserItem_Member;
					//if( memberItem != null )
					//{
					//	var member = memberItem.Member;

					//	//!!!!так?

					//	var type = member.Owner as Metadata.TypeInfo;
					//	if( type != null )
					//		memberFullSignature = string.Format( "{0}|{1}", type.Name, member.Signature );

					//	//!!!!если не из ресурса?

					//	var component = member.Owner as Component;
					//	if( component != null )
					//		memberFullSignature = ReferenceUtility.CalculateResourceReference( component, member.Signature );

					//	//!!!!new
					//	var parentItemComponent = item.Parent as ContentBrowserItem_Component;
					//	if( parentItemComponent != null )
					//	{
					//		var parentComponent = parentItemComponent.Component;

					//		//!!!!только этой иерархии?
					//		if( parentComponent.ParentRoot == Document.ResultComponent )
					//			memberThisPropertySetReferenceToObject = parentComponent;
					//	}
					//}

					//_Component
					var componentItem = item as ContentBrowserItem_Component;
					if( componentItem != null )
					{
						var component = componentItem.Component;

						if( Control.ParentRoot == component.ParentRoot )
						{
							////add node with component
							//createNodeWithComponent = component;
						}
						else
						{
							var resourceInstance = component.ParentRoot?.HierarchyController.CreatedByResource;
							if( resourceInstance != null )
							{
								//create component of type
								objectType = component.GetProvidedType();
							}
						}
					}
				}
			}

			if( objectType != null && ( MetadataManager.GetTypeOfNetType( typeof( UIControl ) ).IsAssignableFrom( objectType ) || MetadataManager.GetTypeOfNetType( typeof( ImageComponent ) ).IsAssignableFrom( objectType ) ) || MetadataManager.GetTypeOfNetType( typeof( Scene ) ).IsAssignableFrom( objectType ) )
			//|| memberFullSignature != "" || createNodeWithComponent != null )
			{
				//!!!!
				//var overObject = GetMouseOverObjectForSelection();

				//Component newObject = null;

				//create component

				UIControl newObject;

				if( MetadataManager.GetTypeOfNetType( typeof( ImageComponent ) ).IsAssignableFrom( objectType ) )
				{
					var image = Control.CreateComponent<UIImage>();
					image.SourceImage = ReferenceUtility.MakeReference( objectType.Name );
					newObject = image;
				}
				else if( MetadataManager.GetTypeOfNetType( typeof( Scene ) ).IsAssignableFrom( objectType ) )
				{
					var image = Control.CreateComponent<UIRenderTarget>();
					image.Scene = ReferenceUtility.MakeReference( objectType.Name );
					newObject = image;
				}
				else
					newObject = (UIControl)Control.CreateComponent( objectType );

				//obj.Name = objectType.GetUserFriendlyNameForInstance( true );

				//node.Name = Control.Components.GetUniqueName( prefix, false, 1 );

				//obj.NewObjectSetDefaultConfiguration();
				//newObject = obj;

				////InvokeMember
				//if( memberFullSignature != "" )
				//{
				//	var obj = node.CreateComponent<InvokeMember>();
				//	obj.Name = "Invoke Member";
				//	obj.Member = new Reference<ReferenceValueType_Member>( null, memberFullSignature );

				//	if( memberThisPropertySetReferenceToObject != null )
				//	{
				//		var p = obj.MetadataGetMemberBySignature( "property:" + obj.GetThisPropertyName() ) as Metadata.Property;
				//		if( p != null )
				//		{
				//			var referenceName = ReferenceUtility.CalculateThisReference( obj, memberThisPropertySetReferenceToObject );
				//			var value = ReferenceUtility.MakeReference( p.TypeUnreferenced.GetNetType(), referenceName );
				//			p.SetValue( obj, value, new object[ 0 ] );
				//		}
				//	}

				//	controlledObject = obj;
				//}

				////reference to object directly without creation child
				//if( createNodeWithComponent != null )
				//	controlledObject = createNodeWithComponent;

				//if( !window.ApplyCreationSettingsToObject( c ) )
				//	return false;

				//if( newObject != null )
				//{

				//set Size
				if( newObject.Size.Value == new UIMeasureValueVector2( UIMeasure.Parent, 1, 1 ) )
					newObject.Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 400 );

				//set name
				newObject.Name = ComponentUtility.GetNewObjectUniqueName( newObject );

				newObject.NewObjectSetDefaultConfiguration();

				//finish object creation
				newObject.Enabled = true;
				dragDropObject = newObject;

				DragDropObjectUpdate();

				//}
			}
		}

		void DragDropObjectDestroy()
		{
			if( dragDropObject != null )
			{
				dragDropObject.RemoveFromParent( true );
				dragDropObject.Dispose();
				dragDropObject = null;

				needRecreateDisplayObject = true;
			}
		}

		void CalculateDropPosition( UIControl control, bool centered )
		{
			var viewport = ViewportControl2.Viewport;
			Vector2 mouse = viewport.MousePosition;
			if( !new Rectangle( 0, 0, 1, 1 ).Contains( mouse ) )
				mouse = new Vector2( 0.5, 0.5 );

			var control2 = GetPreviewControlByMain( control );
			if( control2 == null )
				control2 = control;

			var v = ConvertMainScreenToPreviewScreen( mouse );
			if( centered )
				v -= control2.GetScreenSize() / 2;

			//minus parent position
			Vector2 parentScreenPosition = Vector2.Zero;
			UIControl parent2 = control2.Parent as UIControl;
			if( parent2 != null )
				parentScreenPosition = parent2.GetScreenPosition();
			v -= parentScreenPosition;

			var v2 = control2.ConvertOffset( new UIMeasureValueVector2( UIMeasure.Screen, v ), UIMeasure.Units );

			// Step movement.
			if( ModifierKeys.HasFlag( Keys.Control ) )
			{
				var snap = ProjectSettings.Get.UIEditor.GetUIEditorStepMovement( UIMeasure.Units );
				if( snap != 0 )
				{
					Vector2 snapVec = new Vector2( snap, snap );
					v2 += snapVec / 2;
					v2 /= snapVec;
					v2 = new Vector2I( (int)v2.X, (int)v2.Y ).ToVector2();
					v2 *= snapVec;
				}
			}

			control.Margin = new UIMeasureValueRectangle( UIMeasure.Units, v2.X, v2.Y, 0, 0 );

			//control.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, v.X, v.Y, 0, 0 );


			//var screen = ConvertMainScreenToPreviewScreen( ViewportControl.Viewport.MousePosition );
			//var diff = screen - parent.GetScreenPosition();
			//creationPositionInUnits = parent.ConvertOffset( new UIMeasureValueVector2( UIMeasure.Screen, diff ), UIMeasure.Units );

			////!!!!положение зависит от стиля
			//var positionInUnits = ConvertScreenToUnit( mouse, true ) - control.GetRepresentation().Size.ToVector2() / 2 + new Vector2( 1, 1 );
			//control.Position = positionInUnits.ToVector2I();

			needRecreateDisplayObject = true;
		}

		void DragDropObjectUpdate()
		{
			if( dragDropObject != null && dragDropObject is UIControl control )
			{
				//update parent
				{
					var parent = GetMouseOverObject( false ).Item1;
					if( parent == null )
						parent = Control;

					if( parent != null && control.Parent != parent )
					{
						control.RemoveFromParent( false );
						parent.AddComponent( control );
					}
				}

				//update location
				CalculateDropPosition( control, true );
			}
		}

		void DragDropObjectCommit()
		{
			if( dragDropObject != null )
			{
				var obj = dragDropObject;

				//add to undo with deletion
				var newObjects = new List<Component>();
				newObjects.Add( dragDropObject );
				var action = new UndoActionComponentCreateDelete( Document2, newObjects, true );
				Document2.UndoSystem.CommitAction( action );
				Document2.Modified = true;

				dragDropObject = null;

				//select created object
				EditorAPI2.SelectComponentsInMainObjectsWindow( this, new Component[] { obj } );

				EditorAPI2.SelectDockWindow( this );
			}
		}

		void CalculateCellSize()
		{
			cellSize = 14;

			float dpi = EditorAPI2.DPI;
			if( dpi > 96 )
			{
				cellSize *= dpi / 96;
				cellSize = (int)cellSize;
			}
		}

		//bool CanPaste( out Component destinationParent )
		//{
		//	if( ClipboardManager.CheckAvailableInClipboard<ObjectCutCopyPasteData>() )
		//	{
		//		if( SelectedObjects.Length == 0 )
		//		{
		//			destinationParent = Control;
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
				//!!!!
				//Vector2 addToPositionInUnits = Vector2.Zero;

				for( int n = 0; n < components.Count; n++ )
				{
					var c = components[ n ];

					var cloned = c.Clone();
					if( destinationParent.GetComponent( c.Name ) == null )
						cloned.Name = c.Name;
					else
						cloned.Name = destinationParent.Components.GetUniqueName( c.Name, true, 2 );
					destinationParent.AddComponent( cloned );

					//control position
					//if( destinationParent == Control )
					{
						var control = cloned as UIControl;
						if( control != null )
						{
							if( n == 0 )
							{
								CalculateDropPosition( control, false );

								//!!!!
								//addToPosition = control.Margin.Value.Value.LeftTop - ( (FlowchartNode)c ).NodePosition;
							}
							else
							{
								//!!!!
								//control.NodePosition += addToPosition;
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

		UIControl GetPreviewControlByMain( UIControl control )
		{
			if( renderTextureControl != null )
			{
				if( control == Control )
					return renderTextureControl;
				else
					return renderTextureControl.GetComponentByPath( control.GetPathFromRoot() ) as UIControl;
			}
			return null;
		}

		Rectangle GetControlScreenRectangle( UIControl control )
		{
			var previewControl = GetPreviewControlByMain( control );
			if( previewControl != null )
				return ConvertPreviewScreenToMainScreen( previewControl.GetScreenRectangle() );
			return new Rectangle( 0, 0, 1, 1 );
		}

		List<UIControl> GetControls( bool addRoot )
		{
			var result = new List<UIControl>( 256 );
			if( addRoot )
				result.Add( Control );
			result.AddRange( Control.GetComponents<UIControl>( false, true ) );
			return result;
		}

		public bool CanSnap( out List<UIControl> resultObjects )
		{
			resultObjects = new List<UIControl>();
			foreach( var control in SelectedObjects.OfType<UIControl>() )
				resultObjects.Add( control );
			return resultObjects.Count != 0;
		}

		public void Snap( IEditorAction action )
		{
			if( !CanSnap( out var objects ) )
				return;

			var property = (Metadata.Property)MetadataManager.GetTypeOfNetType( typeof( UIControl ) ).MetadataGetMemberBySignature( "property:Margin" );

			var undoItems = new List<UndoActionPropertiesChange.Item>();
			foreach( var obj in objects )
			{
				var oldValue = obj.Margin;

				var newValue = obj.Margin.Value;

				var snapValue = ProjectSettings.Get.UIEditor.GetUIEditorStepMovement( newValue.Measure );
				if( snapValue != 0 )
				{
					if( action.Name == "Snap All Axes" || action.Name == "Snap X" )
					{
						newValue.Left = ( (long)( newValue.Left / snapValue + ( newValue.Left > 0 ? 0.5 : -0.5 ) ) ) * snapValue;
						newValue.Right = ( (long)( newValue.Right / snapValue + ( newValue.Right > 0 ? 0.5 : -0.5 ) ) ) * snapValue;
					}
					if( action.Name == "Snap All Axes" || action.Name == "Snap Y" )
					{
						newValue.Top = ( (long)( newValue.Top / snapValue + ( newValue.Top > 0 ? 0.5 : -0.5 ) ) ) * snapValue;
						newValue.Bottom = ( (long)( newValue.Bottom / snapValue + ( newValue.Bottom > 0 ? 0.5 : -0.5 ) ) ) * snapValue;
					}
				}
				obj.Margin = new UIMeasureValueRectangle( newValue.Measure, newValue.Value );

				undoItems.Add( new UndoActionPropertiesChange.Item( obj, property, oldValue, null ) );
			}

			if( undoItems.Count != 0 )
			{
				var undoAction = new UndoActionPropertiesChange( undoItems.ToArray() );
				Document2.UndoSystem.CommitAction( undoAction );
				Document2.Modified = true;
			}
		}

		Cursor GetCursorByMoveMode( MoveModeEnum mode )
		{
			switch( mode )
			{
			case MoveModeEnum.ResizeLeft | MoveModeEnum.ResizeTop:
			case MoveModeEnum.ResizeRight | MoveModeEnum.ResizeBottom:
				return KryptonCursors.SizeNWSE;

			case MoveModeEnum.ResizeLeft | MoveModeEnum.ResizeBottom:
			case MoveModeEnum.ResizeRight | MoveModeEnum.ResizeTop:
				return KryptonCursors.SizeNESW;

			case MoveModeEnum.ResizeLeft:
			case MoveModeEnum.ResizeRight:
				return KryptonCursors.SizeWE;

			case MoveModeEnum.ResizeTop:
			case MoveModeEnum.ResizeBottom:
				return KryptonCursors.SizeNS;

			case MoveModeEnum.Move:
				return KryptonCursors.SizeAll;
			}

			return Cursors.Default;
		}
	}
}

#endif