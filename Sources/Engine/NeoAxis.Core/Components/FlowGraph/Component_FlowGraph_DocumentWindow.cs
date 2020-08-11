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
using System.Drawing;
using ComponentFactory.Krypton.Toolkit;
using System.Linq;
using NeoAxis.Widget;

namespace NeoAxis.Editor
{
	public partial class Component_FlowGraph_DocumentWindow : DocumentWindowWithViewport
	{
		//!!!!рисовать по бокам что-то (стрелки), если ноды за пределами экрана есть
		//!!!!в preview по идее можно более наглядно показывать, что где-то еще за экраном есть (типа как миникарта в старкрафте)

		////!!!!а это не в стиле? что еще туда
		//readonly ColorValue overMouseColorMultiplier = new ColorValue( 1, 1, 0 );
		//readonly ColorValue selectedObjectColorMultiplier = new ColorValue( 0, 1, 0 );
		//readonly ColorValue allowMakeLinkColorForPinCircle = new ColorValue( 1, 0, 0 );
		//readonly ColorValue overMouseEditPinValueColor = new ColorValue( 1, 0, 0 );
		//readonly ColorValue engineLogoColor = new ColorValue( .38f, .7f, .95f );

		//!!!!а это не в стиле? что еще туда
		Component_Font nodeFont;
		//!!!!not used, maybe need
		//EngineFont nodeFontValue;
		//Component_Font nodeFontToolTip;
		//Component_Font nodeFontComment;

		static float[] zoomTable = new float[] { .1f, .2f, .35f, .5f, .6f, .7f, .8f, .9f, 1, 1.1f, 1.2f, 1.3f, 1.5f, 1.75f, 2.0f, 2.5f, 3.0f };

		//depending system DPI
		float cellSize;

		//!!!!
		//Second: input flag.
		Component_FlowGraphNode.Representation.Connector referenceCreationSocketFrom;

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
		Vector2 selectByRectangle_StartPosInUnits;
		Vector2 selectByRectangle_LastMousePositionInUnits;

		//node move
		bool nodeMove_Enabled;
		bool nodeMove_Activated;
		bool nodeMove_Cloned;
		Vector2I nodeMove_StartMousePositionInPixels;
		Vector2 nodeMove_StartMousePositionInUnits;
		Component_FlowGraphNode nodeMove_StartOverNode;
		//!!!!проверять не удалились ли. или запретить удаление. где еще так
		ESet<Component_FlowGraphNode> nodeMove_Nodes;
		Dictionary<Component_FlowGraphNode, Vector2I> nodeMove_StartPositions;

		//reference selection
		Component_FlowGraphNode.Representation.Item mouseOverReference;

		//drag and drop
		Component dragDropObject;
		//!!!!
		DragDropSetReferenceData dragDropSetReferenceData;
		bool dragDropSetReferenceDataCanSet;
		string[] dragDropSetReferenceDataCanSetReferenceValues;

		///////////////////////////////////////////

		public class PinInputMouseSelection
		{
			public Component_FlowGraphNode.Representation.Item socket;

			public PinInputMouseSelection( Component_FlowGraphNode.Representation.Item socket )
			{
				this.socket = socket;
			}
		}

		///////////////////////////////////////////

		public Component_FlowGraph_DocumentWindow()
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
		public Component_FlowGraph FlowGraph
		{
			get { return ObjectOfWindow as Component_FlowGraph; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			//!!!!!

			timer50.Start();

			if( ObjectOfWindow != null )
				SelectObjects( new object[] { ObjectOfWindow } );
		}

		////!!!!
		//string GetDemandTitle()
		//{
		//	return FlowGraph.Name;
		//}

		private void GraphDocument_FormClosing( object sender, FormClosingEventArgs e )
		{
		}

		//private void timer1_Tick( object sender, EventArgs e )
		//{
		//!!!!было
		//if( Text != GetDemandTitle() )
		//	Text = GetDemandTitle();

		//!!!!было
		//if( ActiveForm == BlueprintEditorForm.Instance )
		//	renderTargetUserControl1.AutomaticUpdateFPS = IsScrollingViewActivated() ? 100 : 50;
		//else
		//	renderTargetUserControl1.AutomaticUpdateFPS = 0;
		//}

		//public void UpdateEditingFocusedData()
		//{
		//   logicClass.CustomScriptCode = scintilla1.Text;
		//}

		//public void UpdateFonts()
		//{
		//}

		public float GetZoom()
		{
			if( FlowGraph.EditorZoomIndex >= 0 && FlowGraph.EditorZoomIndex < zoomTable.Length )
				return zoomTable[ FlowGraph.EditorZoomIndex ];
			return 1;
		}

		double GetScreenCellSizeX()
		{
			return (double)cellSize / (double)ViewportControl.Viewport.SizeInPixels.X;
		}

		double GetScreenCellSizeY()
		{
			return (double)cellSize / (double)ViewportControl.Viewport.SizeInPixels.Y;
		}

		double GetEditorScrollPositionX()
		{
			return FlowGraph.EditorScrollPosition.X - ConvertScreenToUnitX( 0.5, false );
		}

		double GetEditorScrollPositionY()
		{
			return FlowGraph.EditorScrollPosition.Y - ConvertScreenToUnitY( 0.5, false );
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

		public Rectangle SelectByRectangle_GetRectangleInUnits()
		{
			Rectangle rect = new Rectangle( selectByRectangle_StartPosInUnits );
			rect.Add( selectByRectangle_LastMousePositionInUnits );
			return rect;
		}

		//!!!!!

		void UpdateFonts( CanvasRenderer renderer )
		{
			if( nodeFont == null )
				nodeFont = ResourceManager.LoadResource<Component_Font>( @"Base\Fonts\FlowGraphEditor.ttf" );

			//int height = renderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
			//float screenCellSize = (float)cellSize / (float)height;
			//float demandFontHeight = screenCellSize * GetZoom();

			//if( nodeFont == null || nodeFont.Height != demandFontHeight )
			//{
			//	nodeFont = EngineFontManager.Instance.LoadFont( "FlowGraphEditor", demandFontHeight );
			//	//nodeFontValue = EngineFontManager.Instance.LoadFont( "FlowchartEditor", demandFontHeight * .8f );
			//}

			//float zoom = GetZoom();
			//if( zoom < .5f )
			//	zoom = .5f;
			//float demandFontCommentHeight = screenCellSize * zoom * 1.4f;
			//if( nodeFontComment == null || nodeFontComment.Height != demandFontCommentHeight )
			//	nodeFontComment = EngineFontManager.Instance.LoadFont( "FlowGraphEditor", demandFontCommentHeight );
		}

		public void GetFontSizes( CanvasRenderer renderer, out float nodeFontSize, out float nodeFontSizeComment )
		{
			int height = renderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
			float screenCellSize = (float)cellSize / (float)height;
			nodeFontSize = screenCellSize * GetZoom();

			float zoom = GetZoom();
			if( zoom < .5f )
				zoom = .5f;
			nodeFontSizeComment = screenCellSize * zoom * 1.4f;
		}

		private void renderTargetUserControl1_KeyUp( object sender, KeyEventArgs e )
		{
			//!!!!!
			//if( e.KeyCode == Keys.Apps )
			//   ShowContextMenu( new Point( 0, 0 ) );
		}

		//!!!!
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

			//!!!!copy code from ContentBrowser

			//!!!!!
			//Editor
			{
				//!!!!кнопками открывать еще, рядом с "..."

				//!!!!
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Editor" ), EditorResourcesCache.Edit, delegate ( object s, EventArgs e2 )
				{
					//!!!!
					EditorAPI.OpenDocumentWindowForObject( Document, oneSelectedComponent );
				} );
				item.Enabled = oneSelectedComponent != null && EditorAPI.IsDocumentObjectSupport( oneSelectedComponent );
				items.Add( item );
			}

			//!!!!!
			//Settings
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Settings" ), EditorResourcesCache.Settings, delegate ( object s, EventArgs e2 )
				{
					//!!!!new
					bool canUseAlreadyOpened = !ModifierKeys.HasFlag( Keys.Shift );
					//!!!!"true"

					EditorAPI.ShowObjectSettingsWindow( Document, oneSelectedComponent, canUseAlreadyOpened );
				} );
				//!!!!!
				//!!!!если много выделенных
				item.Enabled = oneSelectedComponent != null;
				items.Add( item );
			}

			items.Add( new KryptonContextMenuSeparator() );

			//New object
			{
				EditorContextMenuWinForms.AddNewObjectItem( items, CanNewObject( out _ ), delegate ( Metadata.TypeInfo type )
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

		//!!!!
		//void ShowContextMenu( Point location )
		//{
		//	ContextMenuStrip menu = new ContextMenuStrip();
		//	//!!!!
		//	//menu.Font = MainForm.GetFont( MainForm.fontContextMenu, menu.Font );

		//	KryptonContextMenuItem item;

		//	//BlueprintGraphNodeObjectsCallMember specific
		//	if( GetSelectedNodes_ObjectsCallMember().Count != 0 )
		//	{
		//		item = new KryptonContextMenuItem( "Select Call Member", null, delegate ( object s, EventArgs e2 )
		//		{
		//			SelectedNodes_CallMemberNode_SelectCallMember();
		//		} );
		//		menu.Items.Add( item );
		//		menu.Items.Add( new ToolStripSeparator() );
		//	}

		//	//!!!!!по сути выбирать правильнее через pin input.
		//	//BlueprintGraphNodeObjectsGetEntity specific
		//	if( GetSelectedNodes_ObjectsGetEntity().Count != 0 )
		//	{
		//		item = new KryptonContextMenuItem( "Select Entity", null, delegate ( object s, EventArgs e2 )
		//		{
		//			SelectedNodes_GetEntityNode_SelectEntity();
		//		} );
		//		menu.Items.Add( item );
		//		menu.Items.Add( new ToolStripSeparator() );
		//	}

		//	//BlueprintGraphNodeObjectsEvent specific
		//	if( GetSelectedNodes_ObjectsEvent().Count != 0 )
		//	{
		//		//select event
		//		item = new KryptonContextMenuItem( "Select Event", null, delegate ( object s, EventArgs e2 )
		//		{
		//			SelectedNodes_ObjectsEventNode_SelectEvent();
		//		} );
		//		menu.Items.Add( item );

		//		//select entities
		//		{
		//			Type type = null;
		//			EventInfo event_ = null;
		//			if( SelectedNodes_ObjectsEventNode_SelectEntities_GetEvent( out type, out event_ ) )
		//			{
		//				if( BlueprintGraphNodeObjectsEvent.IsAllowSelectEntity( type, event_ ) )
		//				{
		//					item = new KryptonContextMenuItem( "Select Entities", null, delegate ( object s, EventArgs e2 )
		//					{
		//						SelectedNodes_ObjectsEventNode_SelectEntities();
		//					} );
		//					menu.Items.Add( item );
		//				}
		//			}
		//		}

		//		menu.Items.Add( new ToolStripSeparator() );
		//	}

		//	//BlueprintGraphNodeObjectsIs specific
		//	if( GetSelectedNodes_Is().Count != 0 )
		//	{
		//		item = new KryptonContextMenuItem( "Change Type", null, delegate ( object s, EventArgs e2 )
		//		{
		//			SelectedNodes_Is_SelectType();
		//		} );
		//		menu.Items.Add( item );
		//		menu.Items.Add( new ToolStripSeparator() );
		//	}

		//	//GetSelectedNodes_ListNodes
		//	if( GetSelectedNodes_ListNodes().Count != 0 )
		//	{
		//		item = new KryptonContextMenuItem( "Change Item Type", null, delegate ( object s, EventArgs e2 )
		//		{
		//			SelectedNodes_ListNodes_ChangeItemType();
		//		} );
		//		menu.Items.Add( item );
		//		menu.Items.Add( new ToolStripSeparator() );
		//	}

		//	//Add Node
		//	{
		//		KryptonContextMenuItem addNodeItem = new KryptonContextMenuItem( "Add Node", imageList1.Images[ "Add_16.png" ] );

		//		List<BlueprintManager.NodeTypeItem> types = new List<BlueprintManager.NodeTypeItem>( BlueprintManager.GetNodeTypes() );
		//		List<BlueprintManager.NodeTypeItem> types2 = new List<BlueprintManager.NodeTypeItem>();
		//		foreach( BlueprintManager.NodeTypeItem type in types )
		//		{
		//			if( BlueprintManager.IsAllowToCreateNodeForGraphType( graph.GraphType, type ) )
		//				types2.Add( type );
		//		}

		//		types2.Sort( delegate ( BlueprintManager.NodeTypeItem item1, BlueprintManager.NodeTypeItem item2 )
		//		{
		//			string text1 = item1.Category + "/" + item1.TypeName;
		//			string text2 = item2.Category + "/" + item2.TypeName;
		//			return string.Compare( text1, text2, false );
		//		} );

		//		foreach( BlueprintManager.NodeTypeItem typeItem in types2 )
		//		{
		//			string[] categories = typeItem.Category.Split( new char[] { '/' } );
		//			AddNodeTypeMenuItemRecursive( typeItem, addNodeItem, categories, 0, location );
		//		}

		//		menu.Items.Add( addNodeItem );
		//	}

		//	//Clone
		//	{
		//		item = new KryptonContextMenuItem( "Clone", imageList1.Images[ "Copy_16.png" ], delegate ( object s, EventArgs e2 )
		//		{
		//			CloneSelectedNodesAndLinks( true, true );
		//		} );
		//		item.Enabled = GetSelectedNodes().Count != 0;
		//		menu.Items.Add( item );
		//	}

		//	//Delete
		//	//if( CurrentAction != null )
		//	{
		//		item = new KryptonContextMenuItem( "Delete", imageList1.Images[ "Delete_16.png" ], delegate ( object s, EventArgs e2 )
		//		{
		//			TryDeleteSelectedObjects();
		//		} );
		//		item.Enabled = selectedObjects.Count != 0;
		//		menu.Items.Add( item );
		//	}

		//	if( menu.Items.Count != 0 )
		//		menu.Show( renderTargetUserControl1, location );
		//}

		//!!!!было
		//!!!!WorkareaMode
		object GetMouseOverObject()
		//Component_FlowchartNode GetMouseOverNode()
		{
			//!!!!всегда получать или кешировать?
			//!!!!если уже удален? удалить весь список, если кто-то удален

			foreach( Component_FlowGraphNode node in FlowGraph.GetComponents<Component_FlowGraphNode>( true ) )
			{
				var style = node.GetResultStyle( FlowGraph );

				var obj = style.GetMouseOverObject( this, node );
				if( obj != null )
					return obj;
			}

			return null;
		}
		//object GetMouseOverControlledObject()
		//{
		//	//!!!!если уже удален? удалить весь список, если кто-то удален

		//	foreach( var obj in mouseOverObjects.Reverse() )
		//	{
		//		return obj;
		//	}

		//	//if( mouseOverObjects.Count != 0 )
		//	//	return mouseOverObjects[ mouseOverObjects.Count - 1 ];//.obj;
		//	return null;
		//}

		//bool IsSelectableObject( object obj )
		//{
		//	//!!!!
		//	return true;

		//	//if( obj != null )
		//	//{
		//	//	if( obj is Component_FlowchartNode )
		//	//		return true;
		//	//	//!!!!!
		//	//	//if( obj is Component_FlowchartNode.Link )
		//	//	//	return true;
		//	//}
		//	//return false;
		//}

		//!!!!было
		//void EditPinValue( Component_FlowchartNode.Pin pin, Point location )
		//{
		//{
		//	string oldValue = pin.Owner.GetPinInputValue( pin.Name );
		//	string newValue = oldValue;

		//	Type type = BlueprintManager.FindType( pin.Type );
		//	bool isEntity = type != null && typeof( Entity ).IsAssignableFrom( type );
		//	bool isEntityType = type != null && typeof( EntityType ).IsAssignableFrom( type );

		//	if( isEntity )
		//	{
		//		ContextMenuStrip menu = new ContextMenuStrip();
		//		//!!!!
		//		//menu.Font = MainForm.GetFont( MainForm.fontContextMenu, menu.Font );

		//		KryptonContextMenuItem item;

		//		//Select from list
		//		item = new KryptonContextMenuItem( "List", null, delegate ( object s, EventArgs e2 )
		//		{
		//			Entity entity = Entities.Instance.GetByName( oldValue );
		//			if( MapEditorInterface.Instance.EntityUITypeEditorEditValue( null, type, ref entity ) )
		//			{
		//				if( entity != null )
		//					newValue = entity.Name;
		//				else
		//					newValue = "";
		//			}

		//			if( newValue != oldValue )
		//			{
		//				if( !string.IsNullOrEmpty( newValue ) )
		//					pin.Owner.SetPinInputValue( pin.Name, newValue );
		//				else
		//					pin.Owner.ResetPinInputValue( pin.Name );
		//				pin.Owner.Owner.NeedUpdateBackwardLinkCache();
		//				BlueprintEditorForm.Instance.SetModified( pin.Owner.Owner.Owner );
		//			}

		//		} );
		//		menu.Items.Add( item );

		//		//Specify name manually
		//		item = new KryptonContextMenuItem( "String", null, delegate ( object s, EventArgs e2 )
		//		{
		//			string caption = "Enter Value";// ToolsLocalization.Translate( "Various", "Blueprint Editor" );
		//			OKCancelTextBoxDialog dialog = new OKCancelTextBoxDialog( oldValue,
		//				string.Format( "Enter value of \"{0}\" type:", pin.Type ), caption, null );
		//			if( dialog.ShowDialog() == DialogResult.OK )
		//				newValue = dialog.TextBoxText;

		//			if( newValue != oldValue )
		//			{
		//				if( !string.IsNullOrEmpty( newValue ) )
		//					pin.Owner.SetPinInputValue( pin.Name, newValue );
		//				else
		//					pin.Owner.ResetPinInputValue( pin.Name );
		//				pin.Owner.Owner.NeedUpdateBackwardLinkCache();
		//				BlueprintEditorForm.Instance.SetModified( pin.Owner.Owner.Owner );
		//			}
		//		} );
		//		menu.Items.Add( item );

		//		menu.Show( renderTargetUserControl1, location );
		//		return;
		//	}
		//	else if( isEntityType )
		//	{
		//		//!!!!!

		//		ContextMenuStrip menu = new ContextMenuStrip();
		//		//!!!!
		//		//menu.Font = MainForm.GetFont( MainForm.fontContextMenu, menu.Font );

		//		KryptonContextMenuItem item;

		//		//Select from list
		//		item = new KryptonContextMenuItem( "List", null, delegate ( object s, EventArgs e2 )
		//		{
		//			string currentPath = "";
		//			EntityType oldType = EntityTypes.Instance.GetByName( oldValue );
		//			if( oldType != null )
		//				currentPath = oldType.FilePath;

		//			Predicate<string> shouldAddDelegate = delegate ( string path )
		//			{
		//				EntityType t = EntityTypes.Instance.FindByFilePath( path );
		//				if( t == null )
		//					return false;
		//				return type.IsAssignableFrom( t.GetType() );
		//			};

		//			ChooseResourceForm dialog = new ChooseResourceForm( ResourceTypeManager.Instance.GetByName( "EntityType" ),
		//				true, shouldAddDelegate, currentPath, false );
		//			//!!!!!
		//			//dialog.UpdateFonts( MainForm.fontForm, MainForm.fontTreeControl );
		//			if( dialog.ShowDialog() == DialogResult.OK )
		//			{
		//				EntityType newType = EntityTypes.Instance.FindByFilePath( dialog.FilePath );
		//				if( newType != null )
		//					newValue = newType.Name;
		//				else
		//					newValue = "";
		//			}

		//			if( newValue != oldValue )
		//			{
		//				if( !string.IsNullOrEmpty( newValue ) )
		//					pin.Owner.SetPinInputValue( pin.Name, newValue );
		//				else
		//					pin.Owner.ResetPinInputValue( pin.Name );
		//				pin.Owner.Owner.NeedUpdateBackwardLinkCache();
		//				BlueprintEditorForm.Instance.SetModified( pin.Owner.Owner.Owner );
		//			}

		//		} );
		//		menu.Items.Add( item );

		//		//Specify name manually
		//		item = new KryptonContextMenuItem( "String", null, delegate ( object s, EventArgs e2 )
		//		{
		//			string caption = "Enter Value";// ToolsLocalization.Translate( "Various", "Blueprint Editor" );
		//			OKCancelTextBoxDialog dialog = new OKCancelTextBoxDialog( oldValue,
		//				string.Format( "Enter value of \"{0}\" type:", pin.Type ), caption, null );
		//			if( dialog.ShowDialog() == DialogResult.OK )
		//				newValue = dialog.TextBoxText;

		//			if( newValue != oldValue )
		//			{
		//				if( !string.IsNullOrEmpty( newValue ) )
		//					pin.Owner.SetPinInputValue( pin.Name, newValue );
		//				else
		//					pin.Owner.ResetPinInputValue( pin.Name );
		//				pin.Owner.Owner.NeedUpdateBackwardLinkCache();
		//				BlueprintEditorForm.Instance.SetModified( pin.Owner.Owner.Owner );
		//			}
		//		} );
		//		menu.Items.Add( item );

		//		menu.Show( renderTargetUserControl1, location );
		//		return;
		//	}
		//	else if( pin.Type == "System.Boolean" )
		//	{
		//		if( oldValue == "1" || string.Compare( oldValue, "True", true ) == 0 )
		//			newValue = "False";
		//		else
		//			newValue = "True";
		//	}
		//	else if( type != null && type.IsEnum )
		//	{
		//		ContextMenuStrip menu = new ContextMenuStrip();
		//		//!!!!
		//		//menu.Font = MainForm.GetFont( MainForm.fontContextMenu, menu.Font );

		//		KryptonContextMenuItem item;

		//		//Select from list
		//		item = new KryptonContextMenuItem( "List", null, delegate ( object s, EventArgs e2 )
		//		{
		//			BlueprintChooseEnumForm dialog = new BlueprintChooseEnumForm( type, oldValue );
		//			if( dialog.ShowDialog() == DialogResult.OK )
		//				newValue = dialog.GetSelectedValue();

		//			if( newValue != oldValue )
		//			{
		//				if( !string.IsNullOrEmpty( newValue ) )
		//					pin.Owner.SetPinInputValue( pin.Name, newValue );
		//				else
		//					pin.Owner.ResetPinInputValue( pin.Name );
		//				pin.Owner.Owner.NeedUpdateBackwardLinkCache();
		//				BlueprintEditorForm.Instance.SetModified( pin.Owner.Owner.Owner );
		//			}

		//		} );
		//		menu.Items.Add( item );

		//		//Specify name manually
		//		item = new KryptonContextMenuItem( "String", null, delegate ( object s, EventArgs e2 )
		//		{
		//			string caption = "Enter Value";// ToolsLocalization.Translate( "Various", "Blueprint Editor" );
		//			OKCancelTextBoxDialog dialog = new OKCancelTextBoxDialog( oldValue,
		//				string.Format( "Enter value of \"{0}\" type:", pin.Type ), caption, null );
		//			if( dialog.ShowDialog() == DialogResult.OK )
		//				newValue = dialog.TextBoxText;

		//			if( newValue != oldValue )
		//			{
		//				if( !string.IsNullOrEmpty( newValue ) )
		//					pin.Owner.SetPinInputValue( pin.Name, newValue );
		//				else
		//					pin.Owner.ResetPinInputValue( pin.Name );
		//				pin.Owner.Owner.NeedUpdateBackwardLinkCache();
		//				BlueprintEditorForm.Instance.SetModified( pin.Owner.Owner.Owner );
		//			}
		//		} );
		//		menu.Items.Add( item );

		//		menu.Show( renderTargetUserControl1, location );
		//		return;
		//	}
		//	else
		//	{
		//		string caption = "Enter Value";// ToolsLocalization.Translate( "Various", "Blueprint Editor" );
		//		OKCancelTextBoxDialog dialog = new OKCancelTextBoxDialog( oldValue,
		//			string.Format( "Enter value of \"{0}\" type:", pin.Type ), caption, delegate ( string text )
		//			{
		//				if( !string.IsNullOrEmpty( text ) )
		//				{
		//					try
		//					{
		//						Type simpleType = BlueprintManager.GetSimpleTypeFromFullTypeName( pin.Type );
		//						if( simpleType != null )
		//							SimpleTypesUtils.GetSimpleTypeValue( simpleType, text );
		//					}
		//					catch( Exception e )
		//					{
		//						Log.Warning( e.Message );
		//						return false;
		//					}
		//				}
		//				return true;
		//			} );

		//		if( dialog.ShowDialog() == DialogResult.OK )
		//			newValue = dialog.TextBoxText;
		//	}

		//	if( newValue != oldValue )
		//	{
		//		if( !string.IsNullOrEmpty( newValue ) )
		//			pin.Owner.SetPinInputValue( pin.Name, newValue );
		//		else
		//			pin.Owner.ResetPinInputValue( pin.Name );
		//		pin.Owner.Owner.NeedUpdateBackwardLinkCache();
		//		BlueprintEditorForm.Instance.SetModified( pin.Owner.Owner.Owner );
		//	}
		//}

		string TranslateContextMenu( string text )
		{
			return EditorContextMenuWinForms.Translate( text );
		}

		//!!!!!было
		//void TryDeleteSelectedObjects()
		//{
		//	if( selectedObjects.Count == 0 )
		//		return;

		//	string text = Translate( "Are you sure you want to delete selected objects?" );
		//	string caption = ToolsLocalization.Translate( "Various", "Blueprint Editor" );
		//	if( MessageBox.Show( text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) != DialogResult.Yes )
		//		return;

		//	object[] objects = selectedObjects.ToArray();

		//	foreach( object obj in objects )
		//	{
		//		if( obj is BlueprintGraphNodeFunction_Input )
		//		{
		//			Log.Warning( "You can't delete \"Function Input\" node." );
		//			return;
		//		}
		//		if( obj is BlueprintGraphNodeFunction_Output )
		//		{
		//			Log.Warning( "You can't delete \"Function Output\" node." );
		//			return;
		//		}
		//	}

		//	ClearObjectSelection( true );

		//	//delete links
		//	foreach( object obj in objects )
		//	{
		//		Component_FlowchartNode.Link link = obj as Component_FlowchartNode.Link;
		//		if( link != null )
		//			link.Owner.DeleteLink( link );
		//	}

		//	//delete input links for nodes which will deleted
		//	foreach( object obj in objects )
		//	{
		//		Component_FlowchartNode node = obj as Component_FlowchartNode;
		//		if( node != null )
		//		{
		//			foreach( Component_FlowchartNode.Pin input in node.GetSettings().Inputs )
		//			{
		//				foreach( Blueprint.Graph.GetAllLinksFromInputPin_Result result in graph.GetAllLinksFromInputPin( node, input.Name ) )
		//				{
		//					foreach( Component_FlowchartNode.Link link in result.OutputNode.GetAllOutputLinks( result.OutputPinName ) )
		//					{
		//						if( link.InputNode == node.Name )
		//							result.OutputNode.DeleteLink( link );
		//					}
		//				}
		//			}
		//		}
		//	}

		//	//delete nodes
		//	foreach( object obj in objects )
		//	{
		//		Component_FlowchartNode node = obj as Component_FlowchartNode;
		//		if( node != null )
		//			graph.DeleteNode( node );
		//	}

		//	BlueprintEditorForm.Instance.SetModified( graph.Owner );
		//}

		//!!!!было
		//private void renderTargetUserControl1_KeyDown( object sender, KeyEventArgs e )
		//{
		//	if( e.KeyCode == EKeys.Delete )
		//		TryDeleteSelectedObjects();
		//}

		////!!!!!enums

		public bool CanCreateReferenceDragDropSetReference( DragDropSetReferenceData dragDropData, Component_FlowGraphNode.Representation.Connector to,
			out string[] outReferenceValues )
		//public bool CanCreateReferenceDragDropSetReference( DragDropSetReferenceData dragDropData, Component objectTo, Metadata.Property propertyTo )
		{
			//reference to Component
			var itemToObject = to.item as Component_FlowGraphNode.Representation.ItemThisObject;
			if( itemToObject != null )
			{
				Component componentTo = itemToObject.Owner.Owner.ControlledObject;
				if( componentTo != null && ReferenceUtility.CanMakeReferenceToObjectWithType( dragDropData.property.TypeUnreferenced,
					MetadataManager.MetadataGetType( componentTo ) ) )
				{
					if( componentTo.ParentRoot.HierarchyController != null &&
						componentTo.ParentRoot.HierarchyController.CreatedByResource.InstanceType == Resource.InstanceType.Resource )
					{
						//reference to component of type
						var type = componentTo.GetProvidedType();
						if( type != null )
						{
							outReferenceValues = new string[ dragDropData.controlledComponents.Length ];
							for( int n = 0; n < outReferenceValues.Length; n++ )
								outReferenceValues[ n ] = type.Name;
							return true;
						}
					}
					else
					{
						if( dragDropData.controlledComponents[ 0 ].ParentRoot == componentTo.ParentRoot )
						{
							outReferenceValues = new string[ dragDropData.controlledComponents.Length ];
							for( int n = 0; n < outReferenceValues.Length; n++ )
								outReferenceValues[ n ] = ReferenceUtility.CalculateThisReference( dragDropData.controlledComponents[ n ], componentTo );
							return true;
						}
					}
				}
			}

			//reference to property
			var itemToProperty = to.item as Component_FlowGraphNode.Representation.ItemProperty;
			if( itemToProperty != null &&
				ReferenceUtility.CanMakeReferenceToObjectWithType( dragDropData.property.TypeUnreferenced, itemToProperty.Property.TypeUnreferenced ) )
			{
				Component componentTo = itemToProperty.Owner.Owner.ControlledObject;
				if( componentTo != null )
				{
					//!!!!как-то это выделить для всего, сцен и т.д.

					if( componentTo.ParentRoot.HierarchyController != null &&
						componentTo.ParentRoot.HierarchyController.CreatedByResource.InstanceType == Resource.InstanceType.Resource )
					{
						//reference to component of type
						var type = componentTo.GetProvidedType();
						if( type != null )
						{
							outReferenceValues = new string[ dragDropData.controlledComponents.Length ];
							for( int n = 0; n < outReferenceValues.Length; n++ )
							{
								//!!!!check
								if( componentTo.Parent == null )
									outReferenceValues[ n ] = type.Name + "|" + itemToProperty.Property.Name;
								else
									outReferenceValues[ n ] = type.Name + "\\" + itemToProperty.Property.Name;
								//outReferenceValues[ n ] = type.Name;
							}
							return true;
						}
					}
					else
					{
						if( dragDropData.controlledComponents[ 0 ].ParentRoot == componentTo.ParentRoot )
						{
							outReferenceValues = new string[ dragDropData.controlledComponents.Length ];
							for( int n = 0; n < outReferenceValues.Length; n++ )
							{
								outReferenceValues[ n ] = ReferenceUtility.CalculateThisReference(
									dragDropData.controlledComponents[ n ], componentTo, itemToProperty.Property.Name );
							}
							return true;
						}
					}
				}
			}

			outReferenceValues = null;
			return false;
		}

		public bool CanCreateReference( Component_FlowGraphNode.Representation.Connector c1, Component_FlowGraphNode.Representation.Connector c2 )
		{
			if( c1.item == c2.item )
				return false;
			if( c1.input == c2.input )
				return false;

			Component_FlowGraphNode.Representation.Connector connectorFrom;
			Component_FlowGraphNode.Representation.Connector connectorTo;
			if( !c1.input )
			{
				connectorFrom = c2;
				connectorTo = c1;
			}
			else
			{
				connectorFrom = c1;
				connectorTo = c2;
			}

			//!!!!что-то еще проверять?

			var itemFrom = connectorFrom.item as Component_FlowGraphNode.Representation.ItemProperty;
			if( itemFrom != null )
			{
				//reference to Component
				var itemToObject = connectorTo.item as Component_FlowGraphNode.Representation.ItemThisObject;
				if( itemToObject != null )
				{
					var controlledComponent = itemToObject.Owner.Owner.ControlledObject.Value as Component;
					if( controlledComponent != null )
					{
						var type = MetadataManager.MetadataGetType( controlledComponent );
						if( ReferenceUtility.CanMakeReferenceToObjectWithType( itemFrom.Property.TypeUnreferenced, type ) )
							return true;
					}
					return false;
				}

				//reference to property
				var itemToProperty = connectorTo.item as Component_FlowGraphNode.Representation.ItemProperty;
				if( itemToProperty != null )
				{
					if( ReferenceUtility.CanMakeReferenceToObjectWithType( itemFrom.Property.TypeUnreferenced, itemToProperty.Property.TypeUnreferenced ) )
						return true;
					return false;

					//Type typeFrom = ReferenceUtils.GetUnreferencedType( itemFrom.Property.Type.GetNetType() );
					//Type typeTo = ReferenceUtils.GetUnreferencedType( itemTo.Property.Type.GetNetType() );
					//if( typeTo.IsAssignableFrom( typeFrom ) )
					//	return true;
					//if( MetadataManager.CanAutoConvertType( typeFrom, typeTo ) )
					//	return true;
				}
			}

			return false;
		}

		//!!!!!было
		//List<Component_FlowchartNode> CloneSelectedNodesAndLinks( bool addOffsetToNodePosition, bool selectClonedNodes )
		//{
		//	List<Component_FlowchartNode> clonedNodes = new List<Component_FlowchartNode>();

		//	Dictionary<Component_FlowchartNode, Component_FlowchartNode> sourceToClonedNodes =
		//		new Dictionary<Component_FlowchartNode, Component_FlowchartNode>();

		//	List<Component_FlowchartNode> sourceNodes = GetSelectedNodes();
		//	foreach( Component_FlowchartNode sourceNode in sourceNodes )
		//	{
		//		Component_FlowchartNode clonedNode = sourceNode.Owner.CreateNode(
		//			BlueprintManager.GetNodeTypeByName( sourceNode.NodeTypeName ) );
		//		clonedNode.OnClone( sourceNode );

		//		if( addOffsetToNodePosition )
		//			clonedNode.NodePosition = clonedNode.NodePosition + new Vec2I( 1, 1 );

		//		clonedNodes.Add( clonedNode );

		//		sourceToClonedNodes[ sourceNode ] = clonedNode;
		//	}

		//	//clone links
		//	foreach( Component_FlowchartNode sourceNode in sourceNodes )
		//	{
		//		Component_FlowchartNode clonedNode = sourceToClonedNodes[ sourceNode ];
		//		foreach( Component_FlowchartNode.Link sourceLink in sourceNode.Links )
		//		{
		//			Component_FlowchartNode sourceInputNode = graph.GetNodeByName( sourceLink.InputNode );
		//			if( sourceInputNode != null )
		//			{
		//				Component_FlowchartNode nodeTo;
		//				if( sourceToClonedNodes.TryGetValue( sourceInputNode, out nodeTo ) )
		//					clonedNode.AddLink( sourceLink.OutputName, nodeTo, sourceLink.InputName );
		//			}
		//		}
		//	}

		//	if( selectClonedNodes )
		//	{
		//		ClearObjectSelection( false );
		//		foreach( Component_FlowchartNode node in clonedNodes )
		//			SelectObject( node, true, false );
		//		UpdatePropertiesForm();
		//	}

		//	//add screen message
		//	{
		//		string text;
		//		if( clonedNodes.Count == 1 )
		//		{
		//			text = Translate( "1 node was cloned" );
		//		}
		//		else
		//		{
		//			string template = Translate( "{0} nodes were cloned" );
		//			text = string.Format( template, clonedNodes.Count );
		//		}
		//		AddScreenMessage( text );
		//	}

		//	BlueprintEditorForm.Instance.SetModified( graph.Owner );

		//	return clonedNodes;
		//}

		//!!!!!было
		//void renderTargetUserControl1_Tick( RenderTargetUserControl sender, float delta )
		//{
		//	//!!!!тут?
		//	if( graph.GraphType == Blueprint.Graph.GraphTypes.Function )
		//		UpdateFunctionInputOutputNodes();
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );
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
				referenceCreationSocketFrom = null;

				var mouseOverObject = GetMouseOverObject();
				var mouseOverNode = mouseOverObject as Component_FlowGraphNode;
				var mouseOverSocket = mouseOverObject as Component_FlowGraphNode.Representation.Connector;

				if( mouseOverNode != null )
				{
					//if( IsSelectableObject( obj ) )
					//{

					//Component_FlowchartNode node = node as Component_FlowchartNode;
					//if( node != null )
					//{

					//prepare to node moving

					Vector2 mouse = viewport.MousePosition;
					Vector2I mouseInPixels = ( mouse * viewport.SizeInPixels.ToVector2() ).ToVector2I();

					nodeMove_Enabled = true;
					nodeMove_Activated = false;
					nodeMove_Cloned = false;
					nodeMove_StartOverNode = mouseOverNode;
					nodeMove_StartMousePositionInPixels = mouseInPixels;
					nodeMove_StartMousePositionInUnits = ConvertScreenToUnit( mouse, true );

					nodeMove_Nodes = new ESet<Component_FlowGraphNode>();
					nodeMove_Nodes.Add( mouseOverNode );

					var objectToNodes = GetObjectToNodesDictionary();

					foreach( var selectedObject in SelectedObjectsSet )
					{
						var n = selectedObject as Component_FlowGraphNode;
						if( n != null )
							nodeMove_Nodes.AddWithCheckAlreadyContained( n );

						var c = selectedObject as Component;
						if( c != null )
						{
							if( objectToNodes.TryGetValue( c, out List<Component_FlowGraphNode> nodes ) )
							{
								foreach( var n2 in nodes )
								{
									var startNodeObject = nodeMove_StartOverNode.ControlledObject.Value;
									if( startNodeObject != null && n2.ControlledObject.Value == startNodeObject )
										continue;

									nodeMove_Nodes.AddWithCheckAlreadyContained( n2 );
								}
							}
						}
					}

					nodeMove_StartPositions = new Dictionary<Component_FlowGraphNode, Vector2I>();
					foreach( var n in nodeMove_Nodes )
						nodeMove_StartPositions.Add( n, n.Position );

					handled = true;
					return;
				}
				else if( mouseOverSocket != null )
				{
					referenceCreationSocketFrom = mouseOverSocket;

					//!!!!было
					//}
					//else
					//{
					//	//!!!!

					//	var socket = obj as Component_FlowchartNode.Representation.Socket;
					//	if( socket != null )
					//	{
					//		referenceCreationSocketFrom = socket;
					//	}
					//	else
					//	{
					//		PinInputMouseSelection pinSelection = obj as PinInputMouseSelection;
					//		//!!!!было
					//		//if( pinSelection != null )
					//		//	EditPinValue( pinSelection.pin, e.Location );
					//	}
					//}

					handled = true;
					return;
				}
				else
				{
					//activate selection by rectangle
					selectByRectangle_Enabled = true;
					selectByRectangle_Activated = false;
					selectByRectangle_StartPosInScreen = viewport.MousePosition;
					selectByRectangle_StartPosInUnits = ConvertScreenToUnit( selectByRectangle_StartPosInScreen, true );
					selectByRectangle_LastMousePositionInUnits = selectByRectangle_StartPosInUnits;

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
				scrollView_StartScrollPosition = FlowGraph.EditorScrollPosition;
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
			//nodeMove
			//scrollView
			//select by rectangle
			//context menu

			object mouseOverObject = GetMouseOverObject();
			var mouseOverSocket = mouseOverObject as Component_FlowGraphNode.Representation.Connector;


			var selectedObjects = new ESet<object>( SelectedObjectsSet );

			//!!!!это как в сцене может сделать. чтобы, если не двигали, то выделять
			//!!!!проверку, чтобы ниже с ректанглом не пересеклось?
			if( button == EMouseButtons.Left )
			{
				//update selected objects
				bool allowSelect = true;

				//!!!!это как в сцене может сделать. чтобы, если не двигали, то выделять
				if( nodeMove_Activated || referenceCreationSocketFrom != null )
					allowSelect = false;
				if( allowSelect )
				{
					bool shiftPressed = ( Form.ModifierKeys & Keys.Shift ) != 0;
					if( !shiftPressed )
						selectedObjects.Clear();

					//select node
					var node = mouseOverObject as Component_FlowGraphNode;
					if( node != null )
					{
						var obj = node.ControlledObject.Value;
						if( obj != null )
						{
							if( !selectedObjects.Contains( obj ) )
								selectedObjects.Add( obj );
							else
								selectedObjects.Remove( obj );
						}
						else
						{
							//select node without assigned controlled object
							selectedObjects.Add( node );
						}
					}

					//select reference
					if( mouseOverReference != null )
					{
						//!!!!проверить валидная ли

						selectedObjects.Add( mouseOverReference );

						//!!!!где-то еще обнулять?
						mouseOverReference = null;
					}
				}

				//create reference
				if( referenceCreationSocketFrom != null )
				{
					if( mouseOverSocket != null && CanCreateReference( referenceCreationSocketFrom, mouseOverSocket ) )
					{
						Component_FlowGraphNode.Representation.Connector connectorFrom;
						Component_FlowGraphNode.Representation.Connector connectorTo;

						bool isFlow = false;
						{
							var item = referenceCreationSocketFrom.item as Component_FlowGraphNode.Representation.ItemProperty;
							if( item != null )
							{
								if( MetadataManager.GetTypeOfNetType( typeof( FlowInput ) ).IsAssignableFrom( item.Property.TypeUnreferenced ) )
									isFlow = true;
							}
						}

						if( isFlow )
						{
							if( referenceCreationSocketFrom.input )
							{
								connectorFrom = mouseOverSocket;
								connectorTo = referenceCreationSocketFrom;
							}
							else
							{
								connectorFrom = referenceCreationSocketFrom;
								connectorTo = mouseOverSocket;
							}
						}
						else
						{
							if( referenceCreationSocketFrom.input )
							{
								connectorFrom = referenceCreationSocketFrom;
								connectorTo = mouseOverSocket;
							}
							else
							{
								connectorFrom = mouseOverSocket;
								connectorTo = referenceCreationSocketFrom;
							}
						}

						var itemFrom = connectorFrom.item as Component_FlowGraphNode.Representation.ItemProperty;

						var objFrom = connectorFrom.item.Owner.Owner.ControlledObject.Value;
						var objTo = connectorTo.item.Owner.Owner.ControlledObject.Value;

						string referenceValue = "";
						{
							//reference to Component
							var itemToObject = connectorTo.item as Component_FlowGraphNode.Representation.ItemThisObject;
							if( itemToObject != null )
								referenceValue = ReferenceUtility.CalculateThisReference( objFrom, objTo );

							//reference to property
							var itemToProperty = connectorTo.item as Component_FlowGraphNode.Representation.ItemProperty;
							if( itemToProperty != null )
								referenceValue = ReferenceUtility.CalculateThisReference( objFrom, objTo, itemToProperty.Property.Name );
						}
						//string referenceValue = ReferenceUtils.CalculateThisReference( objFrom, objTo, itemToProperty.Property.Name );

						var netType = itemFrom.Property.Type.GetNetType();
						var underlyingType = ReferenceUtility.GetUnderlyingType( netType );
						object value = ReferenceUtility.MakeReference( underlyingType, null, referenceValue );

						var undoItems = new List<UndoActionPropertiesChange.Item>();

						var oldValue = itemFrom.Property.GetValue( objFrom, null );
						itemFrom.Property.SetValue( objFrom, value, null );
						undoItems.Add( new UndoActionPropertiesChange.Item( objFrom, itemFrom.Property, oldValue, null ) );

						//undo
						if( undoItems.Count != 0 )
						{
							var action = new UndoActionPropertiesChange( undoItems.ToArray() );
							Document.UndoSystem.CommitAction( action );
							Document.Modified = true;
						}
					}

					//!!!!!где еще выключать?
					referenceCreationSocketFrom = null;
				}

				//node move
				if( nodeMove_Activated )
				{
					//undo

					//!!!!!!отменять, если отменили по Escape

					if( !nodeMove_Cloned )
					{
						//changed

						var undoItems = new List<UndoActionPropertiesChange.Item>();

						foreach( var node in nodeMove_Nodes )
						{
							var oldValue = nodeMove_StartPositions[ node ];
							var property = (Metadata.Property)MetadataManager.GetTypeOfNetType( node.GetType() ).
								MetadataGetMemberBySignature( "property:Position" );
							var undoItem = new UndoActionPropertiesChange.Item( node, property, oldValue, null );
							undoItems.Add( undoItem );
						}

						if( undoItems.Count != 0 )
						{
							var action = new UndoActionPropertiesChange( undoItems.ToArray() );
							Document.UndoSystem.CommitAction( action );
							Document.Modified = true;
						}
					}
					else
					{
						//cloned
						var action = new UndoActionComponentCreateDelete( Document, nodeMove_Nodes.ToArray(), true );
						Document.UndoSystem.CommitAction( action );
						Document.Modified = true;

						//update selected objects to update Settings Window
						SelectObjects( SelectedObjects, forceUpdate: true );
					}
				}
				nodeMove_Enabled = false;
				nodeMove_Activated = false;
				nodeMove_Cloned = false;
				nodeMove_StartOverNode = null;
				nodeMove_Nodes = null;
				nodeMove_StartPositions = null;
				nodeMove_StartMousePositionInPixels = Vector2I.Zero;
				nodeMove_StartMousePositionInUnits = Vector2.Zero;
			}

			//select by rectangle
			if( button == EMouseButtons.Left )
			{
				if( selectByRectangle_Enabled )
				{
					if( selectByRectangle_Activated )//!!!!new
					{
						bool shiftPressed = ( Form.ModifierKeys & Keys.Shift ) != 0;
						if( !shiftPressed )
							selectedObjects.Clear();

						if( selectByRectangle_Activated )
						{
							foreach( var node in SelectByRectangle_GetNodes() )
								selectedObjects.AddWithCheckAlreadyContained( node );
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

			//!!!!
			if( nodeMove_Enabled && !nodeMove_Activated )
			{
				Vector2I mouseInPixels = ( mouse * viewport.SizeInPixels.ToVector2() ).ToVector2I();
				Vector2I diff = nodeMove_StartMousePositionInPixels - mouseInPixels;
				if( Math.Abs( diff.X ) > 2 || Math.Abs( diff.Y ) > 2 )
				{
					nodeMove_Activated = true;

					//Clone
					if( ( ModifierKeys & Keys.Shift ) != 0 )//if( EngineApp.Instance.IsKeyPressed( EKeys.Shift ) )
					{
						Component_FlowGraphNode nodeMove_StartOverNodeSource = nodeMove_StartOverNode;
						ESet<Component_FlowGraphNode> nodeMove_NodesSource = nodeMove_Nodes;
						Dictionary<Component_FlowGraphNode, Vector2I> nodeMove_StartPositionsSource = nodeMove_StartPositions;

						nodeMove_StartOverNode = null;
						nodeMove_Nodes = new ESet<Component_FlowGraphNode>();
						nodeMove_StartPositions = new Dictionary<Component_FlowGraphNode, Vector2I>();

						//!!!!нужно как-то разом клонировать, чтобы ссылки остались?

						foreach( var sourceNode in nodeMove_NodesSource )
						{
							var node = (Component_FlowGraphNode)EditorUtility.CloneComponent( sourceNode );

							if( nodeMove_StartOverNodeSource == sourceNode )
								nodeMove_StartOverNode = node;
							nodeMove_Nodes.Add( node );
							nodeMove_StartPositions[ node ] = nodeMove_StartPositionsSource[ sourceNode ];
						}

						nodeMove_Cloned = true;

						SelectObjects( nodeMove_Nodes.ToArray(), updateSettingsWindowSelectObjects: false );

						//add screen message
						EditorUtility.ShowScreenNotificationObjectsCloned( nodeMove_Nodes.Count );


						////!!!!было
						//if( false )
						////if( !IsObjectSelected( nodeMoveStartOverNode ) )
						//{
						//	//!!!!было

						//	//bool shiftPressed = ( Form.ModifierKeys & Keys.Shift ) != 0;
						//	//if( !shiftPressed )
						//	//	ClearObjectSelection( true );
						//	//SelectObject( nodeMoveStartOverNode, true, true );
						//}
						//else
						//{
						//	//!!!!было
						//	////Clone
						//	//bool shiftPressed = ( Form.ModifierKeys & Keys.Shift ) != 0;
						//	//if( shiftPressed )
						//	//{
						//	//	List<Component_FlowchartNode> selectedNodes = GetSelectedNodes();
						//	//	if( selectedNodes.Count != 0 )
						//	//	{
						//	//		int nodeMoveStartOverNodeIndex = selectedNodes.IndexOf( nodeMoveStartOverNode );

						//	//		List<Component_FlowchartNode> nodes = CloneSelectedNodesAndLinks( false, true );

						//	//		ClearObjectSelection( false );
						//	//		foreach( Component_FlowchartNode node in nodes )
						//	//			SelectObject( node, true, false );
						//	//		//!!!!!!?
						//	//		UpdatePropertiesForm();

						//	//		nodeMoveStartOverNode = nodes[ nodeMoveStartOverNodeIndex ];
						//	//		nodeMoveStartPositions = new Dictionary<Component_FlowchartNode, Vec2I>();
						//	//		foreach( Component_FlowchartNode n in GetSelectedNodes() )
						//	//			nodeMoveStartPositions.Add( n, n.NodePosition );

						//	//		//!!!!на экран лог
						//	//	}
						//	//}
						//}
					}
				}
			}
			if( nodeMove_Activated )
			{
				Vector2 mouseInUnits = ConvertScreenToUnit( viewport.MousePosition, true );
				Vector2 offsetInUnits = mouseInUnits - nodeMove_StartMousePositionInUnits;
				if( offsetInUnits.X < 0 )
					offsetInUnits.X -= .5f;
				if( offsetInUnits.Y < 0 )
					offsetInUnits.Y -= .5f;

				foreach( var node in nodeMove_Nodes )
					node.Position = nodeMove_StartPositions[ node ] + offsetInUnits.ToVector2I();
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
				FlowGraph.EditorScrollPosition = scrollView_StartScrollPosition - ConvertScreenToUnit( mouseDiff, false );
			}

			//update select by rectangle
			if( selectByRectangle_Enabled )
			{
				Vector2 diffPixels = ( viewport.MousePosition - selectByRectangle_StartPosInScreen ) * viewport.SizeInPixels.ToVector2();
				if( Math.Abs( diffPixels.X ) >= 3 || Math.Abs( diffPixels.Y ) >= 3 )
					selectByRectangle_Activated = true;

				selectByRectangle_LastMousePositionInUnits = ConvertScreenToUnit( viewport.MousePosition, true );
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
					if( FlowGraph.EditorZoomIndex < zoomTable.Length - 1 )
					{
						FlowGraph.EditorZoomIndex++;
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
					if( FlowGraph.EditorZoomIndex > 0 )
					{
						FlowGraph.EditorZoomIndex--;
						updated = true;
					}
				}
			}

			if( updated )
			{
				Vector2 newUnitsOnScreen = ConvertScreenToUnit( new Vector2( 1, 1 ), false );

				Vector2 oldUnitsOffsetToCursorPosition = oldUnitsOnScreen * ( mouse - new Vector2( 0.5, 0.5 ) );
				Vector2 newUnitsOffsetToCursorPosition = newUnitsOnScreen * ( mouse - new Vector2( 0.5, 0.5 ) );

				Vector2 v = FlowGraph.EditorScrollPosition;
				v += oldUnitsOffsetToCursorPosition;
				v -= newUnitsOffsetToCursorPosition;
				FlowGraph.EditorScrollPosition = v;

				AddScreenMessage( string.Format( "Zoom {0}", GetZoom() ) );
			}

			handled = true;
		}

		protected override void Viewport_Tick( Viewport viewport, float delta )
		{
			base.Viewport_Tick( viewport, delta );
		}

		protected override void Viewport_UpdateBegin( Viewport viewport )
		{
			base.Viewport_UpdateBegin( viewport );
		}

		List<Component_FlowGraphNode> GetNodesByRectangle( Rectangle rectInUnits )
		{
			List<Component_FlowGraphNode> result = new List<Component_FlowGraphNode>();

			foreach( Component_FlowGraphNode node in FlowGraph.GetComponents<Component_FlowGraphNode>( false ) )
			{
				var style = node.GetResultStyle( FlowGraph );
				if( style.IsIntersectsWithRectangle( this, node, rectInUnits ) )
					result.Add( node );
			}

			return result;
		}

		ESet<Component_FlowGraphNode> SelectByRectangle_GetNodes()
		{
			ESet<Component_FlowGraphNode> result = new ESet<Component_FlowGraphNode>();
			if( selectByRectangle_Activated )
			{
				foreach( var node in GetNodesByRectangle( SelectByRectangle_GetRectangleInUnits() ) )
					result.Add( node );
			}
			return result;
		}
		//ESet<object> GetControlledObjectsBySelectRectangle()
		//{
		//	ESet<object> result = new ESet<object>();

		//	if( selectByRectangle_Activated )
		//	{
		//		foreach( var node in GetNodesByRectangle( SelectByRectangle_GetRectangleInUnits() ) )
		//		{
		//			var obj = node.ControlledObject.Value;
		//			if( obj != null )
		//				result.AddWithCheckAlreadyContained( obj );
		//		}
		//	}

		//	return result;
		//}

		Dictionary<Component, List<Component_FlowGraphNode>> GetObjectToNodesDictionary()
		{
			var result = new Dictionary<Component, List<Component_FlowGraphNode>>();

			foreach( Component_FlowGraphNode node in FlowGraph.GetComponents<Component_FlowGraphNode>( false ) )
			{
				Component obj = node.ControlledObject.Value;
				if( obj != null )
				{
					if( !result.TryGetValue( obj, out List<Component_FlowGraphNode> list ) )
					{
						list = new List<Component_FlowGraphNode>();
						result.Add( obj, list );
					}
					list.Add( node );
				}
			}

			return result;
		}

		public Component_FlowGraphStyle GetFlowGraphStyle()
		{
			Component_FlowGraphStyle flowGraphStyle = FlowGraph.Style;
			if( flowGraphStyle == null )
			{
				flowGraphStyle = Component_FlowGraphStyle_Default.Instance;
				//flowchartStyle = (Component_FlowchartStyle_Default)MetadataManager.GetTypeOfNetType( typeof( Component_FlowchartStyle_Default ) ).AutoCreatedInstance;
				//flowchartStyle = ResourceManager.GetProvidedReadOnlyInstanceAsResource<Component_FlowchartStyle>();
			}

			return flowGraphStyle;
		}

		bool CanSelectObjects()
		{
			if( nodeMove_Activated || dragDropSetReferenceData != null )
				return false;
			return true;
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			//!!!!по идее рисовать можно было бы в симуляции тоже

			var renderer = viewport.CanvasRenderer;

			//!!!!
			var mouseOverObject = GetMouseOverObject();
			var mouseOverNode = mouseOverObject as Component_FlowGraphNode;
			var mouseOverSocket = mouseOverObject as Component_FlowGraphNode.Representation.Connector;
			//mouseOverObjects.Clear();

			var referenceSelectionStates = new Dictionary<Component_FlowGraphNode.Representation.Item, EditorRenderSelectionState>();
			{
				if( mouseOverReference != null && CanSelectObjects() )
					referenceSelectionStates[ mouseOverReference ] = EditorRenderSelectionState.CanSelect;
				foreach( var obj in SelectedObjects )
				{
					var item = obj as Component_FlowGraphNode.Representation.Item;
					if( item != null )
						referenceSelectionStates[ item ] = EditorRenderSelectionState.Selected;
				}
			}

			UpdateFonts( renderer );

			//get flow graph style
			var flowGraphStyle = GetFlowGraphStyle();

			//render background
			flowGraphStyle.RenderBackground( this );

			//!!!!
			//RectI visibleCells = GetVisibleCells();

			var objectToNodes = GetObjectToNodesDictionary();
			var selectByRectangleNodes = SelectByRectangle_GetNodes();

			Component_FlowGraphNode.Representation.Item newMouseOverReference = null;

			//!!!!
			//draw references
			foreach( Component_FlowGraphNode node in FlowGraph.GetComponents<Component_FlowGraphNode>( false ) )
			{
				//!!!!!было if( !( node is BlueprintGraphNodeComment ) )
				//{

				//get style
				var style = node.GetResultStyle( FlowGraph );

				//render references
				style.RenderNodeReferences( this, node, objectToNodes, referenceSelectionStates, out Component_FlowGraphNode.Representation.Item outMouseOver );

				if( outMouseOver != null && newMouseOverReference == null )
					newMouseOverReference = outMouseOver;
			}

			//!!!!тут?
			//update mouseOverReference
			if( referenceCreationSocketFrom == null && mouseOverObject == null )
				mouseOverReference = newMouseOverReference;
			else
				mouseOverReference = null;

			//draw nodes
			{
				//!!!!!было
				////draw Comment nodes before
				//foreach( Component_FlowchartNode node in Flowchart.GetComponents<Component_FlowchartNode>( false ) )
				//{
				//	if( node is BlueprintGraphNodeComment )
				//	{
				//		Component_FlowchartNode.Settings settings = node.GetSettings();

				//		RectI rectWithOneBorder = new RectI(
				//			node.NodePosition - new Vec2I( 1, 1 ),
				//			node.NodePosition + settings.Size + new Vec2I( 1, 1 ) );

				//		if( visibleCells.IsIntersectsRect( rectWithOneBorder ) )
				//			DrawNode( renderer, node, mouseOverObject );
				//	}
				//}

				//draw not Comment nodes
				foreach( Component_FlowGraphNode node in FlowGraph.GetComponents<Component_FlowGraphNode>( false ) )
				{
					//!!!!!было if( !( node is BlueprintGraphNodeComment ) )
					//{

					//get style
					var style = node.GetResultStyle( FlowGraph );

					//node selection state
					var selectionStateNode = EditorRenderSelectionState.None;

					if( selectByRectangleNodes.Contains( node ) && CanSelectObjects() )
						selectionStateNode = EditorRenderSelectionState.CanSelect;

					if( IsObjectSelected( node ) )
						selectionStateNode = EditorRenderSelectionState.Selected;
					else
					{
						//select node without assigned controlled object
						if( referenceCreationSocketFrom == null )
						{
							if( mouseOverNode == node && node.ControlledObject.Value == null && CanSelectObjects() )
								selectionStateNode = EditorRenderSelectionState.CanSelect;
						}
					}

					//controlled object selection state
					var selectionStateControlledObject = EditorRenderSelectionState.None;
					object controlledObject = node.ControlledObject.Value;
					if( controlledObject != null )
					{
						if( referenceCreationSocketFrom == null )
						{
							if( mouseOverNode != null && mouseOverNode.ControlledObject.Value == controlledObject && CanSelectObjects() )
								selectionStateControlledObject = EditorRenderSelectionState.CanSelect;
						}
						if( IsObjectSelected( controlledObject ) )
							selectionStateControlledObject = EditorRenderSelectionState.Selected;
					}

					//render
					style.RenderNode( this, node, selectionStateNode, selectionStateControlledObject, mouseOverObject, referenceCreationSocketFrom,
						dragDropSetReferenceData );//, mouseOverObjects );

					//!!!!!было
					//if( referenceCreationSocketFrom == null && ( mouseOverObject == node || IsSelectModeIntersectWithRectangle( rectNodeInUnits ) ) )
					//	nodeColorMultiplierWithSelection = overMouseColorMultiplier;
					//if( IsObjectSelected( node ) )
					//	nodeColorMultiplierWithSelection = selectedObjectColorMultiplier;
				}
			}

			//!!!!
			//draw reference creation
			if( referenceCreationSocketFrom != null )//!!!!!проверить что не удален
			{
				var node = referenceCreationSocketFrom.item.Owner.Owner;

				//get style
				var style = node.GetResultStyle( FlowGraph );

				//!!!!!!

				Vector2 fromPositionInUnits = style.GetSocketPositionInUnits( referenceCreationSocketFrom.item, referenceCreationSocketFrom.input );

				Vector2 toPositionInUnits;
				if( mouseOverSocket != null && CanCreateReference( referenceCreationSocketFrom, mouseOverSocket ) )
					toPositionInUnits = style.GetSocketPositionInUnits( mouseOverSocket.item, mouseOverSocket.input );
				else
					toPositionInUnits = ConvertScreenToUnit( ViewportControl.Viewport.MousePosition, true );

				flowGraphStyle.RenderReference( this, fromPositionInUnits, referenceCreationSocketFrom.input, toPositionInUnits, new ColorValue( 1, 1, 1 ), out bool mouseOver );

				//if( mouseOver )
				//   mouseOverObjects.Add( link );
			}

			//draw selection rectangle
			if( selectByRectangle_Enabled && selectByRectangle_Activated )
			{
				Rectangle rect = new Rectangle( ConvertUnitToScreen( selectByRectangle_StartPosInUnits ) );
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

			//!!!!!
			////show tooltip
			//{
			//	Vec2 pixelInScreen = Vec2.One / renderer.ViewportForScreenGuiRenderer.DimensionsInPixels.Size.ToVec2();

			//	float demandFontHeight = (float)16 / (float)renderer.ViewportForScreenGuiRenderer.DimensionsInPixels.Size.Y;
			//	if( fontToolTip == null || fontToolTip.Height != demandFontHeight )
			//		fontToolTip = EngineFontManager.Instance.LoadFont( "FlowchartEditor", demandFontHeight );

			//	string text = "";
			//	if( mouseOverObject != null )
			//	{
			//		Component_FlowchartNode node = mouseOverObject as Component_FlowchartNode;
			//		if( node != null )
			//		{
			//			string error = node.GetSettings().Error;
			//			if( error != null )
			//				text = error;
			//		}

			//		Component_FlowchartNode.Pin pin = mouseOverObject as Component_FlowchartNode.Pin;
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

			//update dragDropSetReferenceData
			dragDropSetReferenceDataCanSet = false;
			if( dragDropSetReferenceData != null && mouseOverSocket != null )
			{
				bool isFlow = MetadataManager.GetTypeOfNetType( typeof( FlowInput ) ).IsAssignableFrom( dragDropSetReferenceData.property.TypeUnreferenced );

				if( isFlow )
				{
					if( mouseOverSocket.input )
					{
						if( CanCreateReferenceDragDropSetReference( dragDropSetReferenceData, mouseOverSocket, out dragDropSetReferenceDataCanSetReferenceValues ) )
							dragDropSetReferenceDataCanSet = true;
					}
				}
				else
				{
					if( !mouseOverSocket.input )
					{
						if( CanCreateReferenceDragDropSetReference( dragDropSetReferenceData, mouseOverSocket, out dragDropSetReferenceDataCanSetReferenceValues ) )
							dragDropSetReferenceDataCanSet = true;
					}
				}
			}
			//if( dragDropSetReferenceData != null )
			//{
			//	if( mouseOverSocket != null && !mouseOverSocket.input )
			//	{
			//		if( CanCreateReferenceDragDropSetReference( dragDropSetReferenceData, mouseOverSocket, out dragDropSetReferenceDataCanSetReferenceValues ) )
			//			dragDropSetReferenceDataCanSet = true;
			//	}
			//}

			//render background
			flowGraphStyle.RenderForeground( this );
		}

		protected override void Viewport_UpdateEnd( Viewport viewport )
		{
			base.Viewport_UpdateEnd( viewport );
		}

		[Browsable( false )]
		public Component_Font NodeFont
		{
			get { return nodeFont; }
		}

		//[Browsable( false )]
		//public EngineFont NodeFontValue
		//{
		//	get { return nodeFontValue; }
		//}

		//[Browsable( false )]
		//public Component_Font NodeFontToolTip
		//{
		//	get { return nodeFontToolTip; }
		//}

		[Browsable( false )]
		public Component_Font NodeFontComment
		{
			get { return nodeFont; }// nodeFontComment; }
		}

		public override void EditorActionGetState( EditorAction.GetStateContext context )
		{
			base.EditorActionGetState( context );
		}

		public override void EditorActionClick( EditorAction.ClickContext context )
		{
			base.EditorActionClick( context );
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

			//can't create for references
			foreach( var obj in SelectedObjects )
			{
				var referenceItem = obj as Component_FlowGraphNode.Representation.Item;
				if( referenceItem != null )
					return false;
			}

			//can create without selected objects
			if( parentsForNewObjects.Count == 0 )
				parentsForNewObjects.Add( FlowGraph );
			return true;
			//return parentsForNewObjects.Count != 0;
		}

		public void TryNewObject( Metadata.TypeInfo lockType )
		{
			if( !CanNewObject( out List<Component> parentsForNewObjects ) )
				return;

			bool nodeCreationOnGraph = false;
			Vector2I nodeCreationOnGraphPosition = Vector2I.Zero;
			if( parentsForNewObjects.Count == 1 && parentsForNewObjects[ 0 ] == FlowGraph )
			{
				nodeCreationOnGraph = true;
				nodeCreationOnGraphPosition = ConvertScreenToUnit( ViewportControl.Viewport.MousePosition, true ).ToVector2I();
			}

			var data = new NewObjectWindow.CreationDataClass();
			data.initDocumentWindow = this;
			data.initParentObjects = new List<object>();
			data.initParentObjects.AddRange( parentsForNewObjects );

			//object parent;
			//bool nodeCreationOnGraph = false;
			//Vector2I nodeCreationOnGraphPosition = Vector2I.Zero;
			//if( parentsForNewObjects.Count != 0 )
			//{
			//	parent = parentsForNewObjects[ 0 ];
			//}
			//else
			//{
			//	parent = FlowGraph;
			//	nodeCreationOnGraph = true;
			//	nodeCreationOnGraphPosition = ConvertScreenToUnit( ViewportControl.Viewport.MousePosition, true ).ToVector2I();
			//}

			//var data = new NewObjectWindow.CreationDataClass();
			//data.initDocumentWindow = this;
			//data.initParentObjects = new List<object>();

			//data.initParentObjects.Add( parent );

			//!!!!
			//if( parent is Component )
			//	data.initDemandedType = MetadataManager.GetTypeOfNetType( typeof( Component ) );


			data.beforeCreateObjectsFunction = delegate ( NewObjectWindow window, Metadata.TypeInfo selectedType )
			{
				if( nodeCreationOnGraph && !typeof( Component_FlowGraphNode ).IsAssignableFrom( selectedType.GetNetType() ) )
				{
					window.creationData.createdObjects = new List<object>();

					//create node with object inside of selected type
					var node = FlowGraph.CreateComponent<Component_FlowGraphNode>( -1, false );
					window.creationData.createdObjects.Add( node );
					window.creationData.createdComponentsOnTopLevel.Add( node );

					//create component of selected type as child of the node
					var c = node.CreateComponent( selectedType );

					//!!!!имя должно быть обязательно, чтобы ссылку настроить
					//!!!!!!может если нет имени, то ошибку создавать (в ReferenceUtils.CalculateThisReference)
					bool disableFileCreation = false;
					if( !window.ApplyCreationSettingsToObject( c, ref disableFileCreation ) )
						return false;

					window.creationData.createdObjects.Add( c );

					var prefix = "Node " + c.Name;
					node.Name = FlowGraph.Components.GetUniqueName( prefix, false, 1 );

					//set ControlledObject of the node
					//!!!!как-то сложно?
					node.ControlledObject = new Reference<Component>( null, ReferenceUtility.CalculateThisReference( node, c ) );
				}

				return true;
			};

			//set NodePosition
			data.additionActionBeforeEnabled = delegate ( NewObjectWindow window )
			{
				if( nodeCreationOnGraph )
				{
					foreach( var obj in data.createdComponentsOnTopLevel )
					{
						var node = obj as Component_FlowGraphNode;
						if( node != null )
							node.Position = nodeCreationOnGraphPosition;
					}
				}
			};

			data.initLockType = lockType;
			data.initSupportAutoCreateAndClose = lockType != null;

			EditorAPI.OpenNewObjectWindow( data );
		}

		public override bool CanDeleteObjects( out List<object> resultObjectsToDelete )
		{
			resultObjectsToDelete = new List<object>();

			foreach( var obj in SelectedObjects )
			{
				var component = obj as Component;
				if( component != null && component.Parent != null )
					resultObjectsToDelete.Add( component );

				var referenceItem = obj as Component_FlowGraphNode.Representation.Item;
				if( referenceItem != null )
					resultObjectsToDelete.Add( referenceItem );
			}

			if( resultObjectsToDelete.Count == 0 )
				return false;

			return true;
		}

		public override bool TryDeleteObjects()
		{
			//!!!!!игнорить выделенные-вложенные. где еще так

			if( !CanDeleteObjects( out List<object> objectsToDelete ) )
				return false;

			string text;
			if( objectsToDelete.Count == 1 )
			{
				string template = EditorLocalization.Translate( "DocumentWindow", "Are you sure you want to delete \'{0}\'?" );
				var name = objectsToDelete[ 0 ].ToString();
				text = string.Format( template, name );
			}
			else
			{
				string template = EditorLocalization.Translate( "DocumentWindow", "Are you sure you want to delete selected objects?" );
				text = string.Format( template, objectsToDelete.Count );
			}

			if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.No )
				return false;

			SelectObjects( null );

			//!!!!так? может для компонент отдельными действиями. свойства тоже списком итемов
			List<Component> componentsToDelete = new List<Component>();
			List<UndoActionPropertiesChange.Item> propertiesChange = new List<UndoActionPropertiesChange.Item>();

			foreach( var objectToDelete in objectsToDelete )
			{
				var component = objectToDelete as Component;
				if( component != null )
					componentsToDelete.Add( component );

				var referenceItem = objectToDelete as Component_FlowGraphNode.Representation.ItemProperty;
				if( referenceItem != null )
				{
					var obj = referenceItem.Owner.Owner.ControlledObject.Value;
					var property = referenceItem.Property;

					var oldValue = property.GetValue( obj, null );

					object unrefSetValue = null;
					{
						var unrefType = property.TypeUnreferenced;

						//get default value
						if( property.DefaultValueSpecified )
						{
							unrefSetValue = property.DefaultValue;

							//convert default value from string
							if( unrefSetValue != null )
							{
								if( unrefType.GetNetType() != typeof( string ) && unrefSetValue is string )
									unrefSetValue = SimpleTypes.ParseValue( unrefType.GetNetType(), (string)unrefSetValue );
							}
						}

						if( unrefSetValue != null && !unrefType.IsAssignableFrom( MetadataManager.MetadataGetType( unrefSetValue ) ) )
							unrefSetValue = null;
					}

					var value = ReferenceUtility.MakeReference( ReferenceUtility.GetUnreferencedType( property.Type.GetNetType() ), unrefSetValue, "" );
					//var unrefOldValue = ReferenceUtils.GetUnreferencedValue( oldValue );
					//var value = ReferenceUtils.CreateReference( ReferenceUtils.GetUnreferencedType( property.Type.GetNetType() ), unrefOldValue, "" );

					property.SetValue( obj, value, null );

					propertiesChange.Add( new UndoActionPropertiesChange.Item( obj, property, oldValue, null ) );
				}
			}

			UndoSystem.Action resultAction = null;
			if( componentsToDelete.Count != 0 && propertiesChange.Count != 0 )
			{
				UndoMultiAction multiAction = new UndoMultiAction();
				multiAction.Actions.Add( new UndoActionComponentCreateDelete( Document, componentsToDelete, false ) );
				multiAction.Actions.Add( new UndoActionPropertiesChange( propertiesChange.ToArray() ) );
				resultAction = multiAction;
			}
			else if( componentsToDelete.Count != 0 )
				resultAction = new UndoActionComponentCreateDelete( Document, componentsToDelete, false );
			else if( propertiesChange.Count != 0 )
				resultAction = new UndoActionPropertiesChange( propertiesChange.ToArray() );

			Document.UndoSystem.CommitAction( resultAction );
			//var action = new UndoActionComponentCreateDelete( this, objectsToDelete, false );
			//Document.UndoSystem.CommitAction( action );
			Document.Modified = true;

			return true;
		}

		public override bool CanCloneObjects( out List<Component> resultObjectsToClone )
		{
			resultObjectsToClone = new List<Component>();

			var objectToNodes = GetObjectToNodesDictionary();

			//!!!!или из transform tool брать?
			foreach( var obj in SelectedObjects )
			{
				var component = obj as Component;
				if( component != null )
				{
					if( objectToNodes.TryGetValue( component, out List<Component_FlowGraphNode> nodes ) )
					{
						foreach( var node in nodes )
							resultObjectsToClone.Add( node );
					}
					else
					{
						if( component.Parent != null )
							resultObjectsToClone.Add( component );
					}
				}
			}

			if( resultObjectsToClone.Count == 0 )
				return false;

			return true;
		}

		private void timer50_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			//!!!!slowly?
			foreach( Component_FlowGraphNode node in FlowGraph.GetComponents<Component_FlowGraphNode>( false ) )
				node.RepresentationNeedUpdate();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//bool DragDropSetReferenceFromReferenceButton( DragEventArgs e, bool checkOnly )
		//{
		//	DragDropSetReferenceData dragDropData = (DragDropSetReferenceData)e.Data.GetData( typeof( DragDropSetReferenceData ) );
		//	if( dragDropData != null )
		//	{
		//		xx xx;

		//		var node = treeView.GetNodeAt( treeView.PointToClient( Cursor.Position ) ) as ItemTreeNode;
		//		if( node != null )
		//		{
		//			var item = node.item;

		//			string[] referenceValues = new string[ dragDropData.controlledObjects.Length ];
		//			for( int n = 0; n < dragDropData.controlledObjects.Length; n++ )
		//			{
		//				item.CalculateReferenceValue( dragDropData.controlledObjects[ n ], dragDropData.property, out string referenceValue, out bool canSet );
		//				referenceValues[ n ] = referenceValue;
		//				if( !canSet )
		//				{
		//					referenceValues = null;
		//					break;
		//				}
		//			}

		//			if( referenceValues != null )
		//			{
		//				if( !checkOnly )
		//					dragDropData.SetProperty( referenceValues );

		//				return true;
		//			}
		//		}
		//	}

		//	return false;
		//}

		private void Component_FlowGraph_DocumentWindow_DragEnter( object sender, DragEventArgs e )
		{
			DragDropObjectCreate( e );

			////!!!!
			//DragDropSetReferenceData dragDropData = (DragDropSetReferenceData)e.Data.GetData( typeof( DragDropSetReferenceData ) );
			//if( dragDropData != null )
			//	dragDropSetReferenceData = dragDropData;
		}

		private void Component_FlowGraph_DocumentWindow_DragOver( object sender, DragEventArgs e )
		{
			e.Effect = DragDropEffects.None;

			//!!!!пока так
			ViewportControl?.PerformMouseMove();

			DragDropObjectUpdate();
			if( dragDropObject != null )
				e.Effect = DragDropEffects.Link;

			DragDropSetReferenceData dragDropData = (DragDropSetReferenceData)e.Data.GetData( typeof( DragDropSetReferenceData ) );
			if( dragDropData != null )
				dragDropSetReferenceData = dragDropData;
			if( dragDropSetReferenceData != null && dragDropSetReferenceDataCanSet )
				e.Effect = DragDropEffects.Link;

			//!!!!пока так
			ViewportControl.TryRender();
		}

		private void Component_FlowGraph_DocumentWindow_DragLeave( object sender, EventArgs e )
		{
			DragDropObjectDestroy();

			dragDropSetReferenceData = null;

			//!!!!пока так
			ViewportControl.TryRender();
		}

		private void Component_FlowGraph_DocumentWindow_DragDrop( object sender, DragEventArgs e )
		{
			DragDropObjectCommit();

			if( dragDropSetReferenceData != null )
			{
				if( dragDropSetReferenceDataCanSet )
				{
					dragDropSetReferenceData.SetProperty( dragDropSetReferenceDataCanSetReferenceValues );
					dragDropSetReferenceDataCanSet = false;
				}
				dragDropSetReferenceData = null;
			}
		}

		void DragDropObjectCreate( DragEventArgs e )
		{
			Metadata.TypeInfo createComponentType = null;
			string memberFullSignature = "";
			Component memberThisPropertySetReferenceToObject = null;
			Component createNodeWithComponent = null;
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
								createComponentType = type;
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
							createComponentType = type;
					}

					//_Member
					var memberItem = item as ContentBrowserItem_Member;
					if( memberItem != null )
					{
						var member = memberItem.Member;

						//!!!!так?

						var type = member.Owner as Metadata.TypeInfo;
						if( type != null )
							memberFullSignature = string.Format( "{0}|{1}", type.Name, member.Signature );

						//!!!!если не из ресурса?

						var component = member.Owner as Component;
						if( component != null )
							memberFullSignature = ReferenceUtility.CalculateResourceReference( component, member.Signature );

						//!!!!new
						var parentItemComponent = item.Parent as ContentBrowserItem_Component;
						if( parentItemComponent != null )
						{
							var parentComponent = parentItemComponent.Component;

							//!!!!только этой иерархии?
							if( parentComponent.ParentRoot == Document.ResultComponent )
								memberThisPropertySetReferenceToObject = parentComponent;
						}
					}

					//_Component
					var componentItem = item as ContentBrowserItem_Component;
					if( componentItem != null )
					{
						var component = componentItem.Component;

						if( FlowGraph.ParentRoot == component.ParentRoot )
						{
							//add node with component
							createNodeWithComponent = component;
						}
						else
						{
							var resourceInstance = component.ParentRoot?.HierarchyController.CreatedByResource;
							if( resourceInstance != null )
							{
								//create component of type
								createComponentType = component.GetProvidedType();
							}
						}
					}
				}
			}

			if( createComponentType != null || memberFullSignature != "" || createNodeWithComponent != null )
			{
				//start creation

				//create node
				var node = FlowGraph.CreateComponent<Component_FlowGraphNode>( -1, false );
				bool skipCreation = false;

				Component controlledObject = null;

				//specialization of the flow graph
				bool specializationHandled = false;
				var specialization = FlowGraph.Specialization.Value;
				if( specialization != null )
				{
					var context = new Component_FlowGraphSpecialization.DragDropObjectCreateInitNodeContext();
					context.createComponentType = createComponentType;
					context.memberFullSignature = memberFullSignature;
					//? memberThisPropertySetReferenceToObject
					context.createNodeWithComponent = createNodeWithComponent;

					specialization.DragDropObjectCreateInitNode( node, context, ref specializationHandled );

					if( specializationHandled )
						controlledObject = context.controlledObject;
				}

				//default behaviour
				if( !specializationHandled )
				{
					//Component_CSharpScript
					if( !skipCreation && controlledObject == null && createComponentType != null && MetadataManager.GetTypeOfNetType( typeof( Component_CSharpScript ) ).IsAssignableFrom( createComponentType ) )
					{
						var type = createComponentType as Metadata.ComponentTypeInfo;
						if( type != null && type.BasedOnObject != null )
						{
							var scriptType = type.BasedOnObject as Component_CSharpScript;
							if( scriptType != null )
							{
								if( scriptType.CompiledOneMethod != null )
								{
									var invokeMember = node.CreateComponent<Component_InvokeMember>();
									controlledObject = invokeMember;
									invokeMember.Name = "Invoke Member";

									var memberFullPath = createComponentType.Name + "|" + scriptType.CompiledOneMethod.Signature;
									invokeMember.Member = new Reference<ReferenceValueType_Member>( null, memberFullPath );
								}
							}

							if( controlledObject == null )
								skipCreation = true;
						}
					}

					//create component
					if( !skipCreation && controlledObject == null && createComponentType != null && createComponentType != MetadataManager.GetTypeOfNetType( typeof( Component_FlowGraphNode ) ) )
					{
						var obj = node.CreateComponent( createComponentType );
						obj.Name = createComponentType.GetUserFriendlyNameForInstance( true );
						obj.NewObjectSetDefaultConfiguration();
						controlledObject = obj;
					}

					//Component_InvokeMember
					if( !skipCreation && controlledObject == null && memberFullSignature != "" )
					{
						var obj = node.CreateComponent<Component_InvokeMember>();
						obj.Name = "Invoke Member";
						obj.Member = new Reference<ReferenceValueType_Member>( null, memberFullSignature );

						if( memberThisPropertySetReferenceToObject != null )
						{
							var p = obj.MetadataGetMemberBySignature( "property:" + obj.GetThisPropertyName() ) as Metadata.Property;
							if( p != null )
							{
								var referenceName = ReferenceUtility.CalculateThisReference( obj, memberThisPropertySetReferenceToObject );
								var value = ReferenceUtility.MakeReference( p.TypeUnreferenced.GetNetType(), null, referenceName );
								p.SetValue( obj, value, new object[ 0 ] );
							}
						}

						controlledObject = obj;
					}

					//reference to object directly without creation child
					if( !skipCreation && createNodeWithComponent != null )
						controlledObject = createNodeWithComponent;
				}

				//if( !window.ApplyCreationSettingsToObject( c ) )
				//	return false;

				if( skipCreation )
				{
					node.Dispose();
					dragDropObject = null;
				}
				else
				{
					//configure node with created object inside
					if( controlledObject != null )
					{
						var prefix = "Node " + controlledObject.Name;
						node.Name = FlowGraph.Components.GetUniqueName( prefix, false, 1 );

						//set ControlledObject of the node
						node.ControlledObject = new Reference<Component>( null, ReferenceUtility.CalculateThisReference( node, controlledObject ) );
					}
					node.Enabled = true;
					dragDropObject = node;

					DragDropObjectUpdate();
				}
			}
		}

		void DragDropObjectDestroy()
		{
			if( dragDropObject != null )
			{
				dragDropObject.RemoveFromParent( true );
				dragDropObject.Dispose();
				dragDropObject = null;
			}
		}

		void CalculateDropPosition( Component_FlowGraphNode node )
		{
			var viewport = ViewportControl.Viewport;
			Vector2 mouse = viewport.MousePosition;
			if( !new Rectangle( 0, 0, 1, 1 ).Contains( mouse ) )
				mouse = new Vector2( 0.5, 0.5 );

			//!!!!положение зависит от стиля
			var positionInUnits = ConvertScreenToUnit( mouse, true ) - node.GetRepresentation().Size.ToVector2() / 2 + new Vector2( 1, 1 );
			node.Position = positionInUnits.ToVector2I();
		}

		void DragDropObjectUpdate()
		{
			if( dragDropObject != null )
			{
				var node = dragDropObject as Component_FlowGraphNode;
				if( node != null )
					CalculateDropPosition( node );
			}
		}

		void DragDropObjectCommit()
		{
			if( dragDropObject != null )
			{
				//add to undo with deletion
				var newObjects = new List<Component>();
				newObjects.Add( dragDropObject );
				var action = new UndoActionComponentCreateDelete( Document, newObjects, true );
				Document.UndoSystem.CommitAction( action );
				Document.Modified = true;

				dragDropObject = null;

				EditorAPI.SelectDockWindow( this );

				//!!!!выделять логичнее ControlledObject
				//SelectObjects( newObjects.ToArray() );
			}
		}

		void CalculateCellSize()
		{
			cellSize = 14;

			float dpi = EditorAPI.DPI;
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
		//			destinationParent = FlowGraph;
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
				Vector2I addToPosition = Vector2I.Zero;

				for( int n = 0; n < components.Count; n++ )
				{
					var c = components[ n ];

					var cloned = c.Clone();
					if( destinationParent.GetComponent( c.Name ) == null )
						cloned.Name = c.Name;
					else
						cloned.Name = destinationParent.Components.GetUniqueName( c.Name, true, 2 );
					destinationParent.AddComponent( cloned );

					//node position
					if( destinationParent == FlowGraph )
					{
						var node = cloned as Component_FlowGraphNode;
						if( node != null )
						{
							if( n == 0 )
							{
								CalculateDropPosition( node );
								addToPosition = node.Position - ( (Component_FlowGraphNode)c ).Position;
							}
							else
								node.Position += addToPosition;
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

		protected override object OnGetSelectObjectWhenNoSelectedObjects()
		{
			//!!!!
			//Material specific
			var specialization = FlowGraph?.Specialization.Value;
			if( specialization as Component_FlowGraphSpecialization_Shader != null )
			{
				if( FlowGraph.Parent as Component_Material != null )
					return FlowGraph.Parent;
			}

			return base.OnGetSelectObjectWhenNoSelectedObjects();
		}
	}
}
