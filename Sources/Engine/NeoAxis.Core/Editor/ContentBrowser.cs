// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ComponentFactory.Krypton.Toolkit;
using System.IO;
using Aga.Controls.Tree;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a rich control to work with hierarchical data in tree or list form.
	/// </summary>
	public partial class ContentBrowser : EUserControl
	{
		static List<ContentBrowser> allInstances = new List<ContentBrowser>();

		static List<ContentBrowserFilteringMode> filteringModes = new List<ContentBrowserFilteringMode>();

		//init data
		bool initialized;
		ModeEnum mode;
		DocumentWindow documentWindow;
		object rootObject;
		//ResourcesModeDataClass resourcesModeData;
		SetReferenceModeDataClass setReferenceModeData;

		bool preloadResourceOnSelection = true;
		bool canSelectObjectSettings;

		bool readOnlyHierarchy;
		//!!!!
		bool thisIsSettingsWindow;

		////for select new resources
		//internal bool thisIsMainResourcesWindow;

		ContentBrowserOptions options;

		//

		PanelModeEnum panelMode = PanelModeEnum.TwoPanelsSplitHorizontally;
		ListModeEnum listMode = ListModeEnum.List;
		int currentListImageSizeScaled = 1;
		int currentListTileColumnWidthScaled = 1;

		ESet<Item> items = new ESet<Item>();
		Dictionary<ItemTreeNode, Item> itemByNode = new Dictionary<ItemTreeNode, Item>();
		Dictionary<Item, ItemTreeNode> nodeByItem = new Dictionary<Item, ItemTreeNode>();
		//need only for optimization
		List<Item> rootItems = new List<Item>();

		ContentBrowserItem_Virtual favoritesItem;
		ContentBrowserItem_File dataItem;

		internal SortByItems updatedDataSortBy;
		internal bool updatedDataSortByAscending;
		////!!!!!
		////!!!![Config( "ResourcesForm", "sortBy" )]
		//public static SortByItems sortBy = SortByItems.Name;

		////!!!![Config( "ResourcesForm", "sortByAscending" )]
		//public static bool sortByAscending = true;

		bool showToolBar = true;

		//bool lastDoubleClick;

		bool loaded;

		//!!!!сохранять в конфиге
		ContentBrowserFilteringMode filteringMode;

		TreeModel treeViewModel;

		ContentBrowserImageHelper imageHelper = new ContentBrowserImageHelper();

		bool selectItemsMethodCalling;
		//!!!!new
		bool setDataMethodCalling;

		string[] needSelectFiles;
		bool needSelectFilesExpandNodes;
		//List<string> needSelectFiles = new List<string>();
		DateTime needSelectFiles_LastUpdateTime = DateTime.Now;

		bool splitterMoving;

		string currentToolTipTreeView = "~~~";
		string currentToolTipListView = "~~~";

		bool multiSelect;

		bool treeView_SelectionChangedEnabled = true;

		bool treeViewUpdating;

		double lastUpdateTime;

		EngineListView.ModeClass/*Type*/ listViewModeOverride;

		KryptonBreadCrumb breadCrumb;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum ModeEnum
		{
			Resources,
			Objects,
			SetReference,
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum PanelModeEnum
		{
			Tree,
			List,
			TwoPanelsSplitHorizontally,
			TwoPanelsSplitVertically,
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum ListModeEnum
		{
			Auto,
			List,
			Tiles,
			//!!!!потом Details,
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum SortByItems
		{
			Name,
			Date,
			Type,
			Size,
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public class ResourcesModeDataClass
		//{
		//	public ResourceSelectionMode selectionMode;
		//	public Metadata.TypeInfo demandedType;
		//	public bool allowNull;
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public class SetReferenceModeDataClass
		{
			public Component[] selectedComponents;
			public object[] propertyOwners;
			//public Component[] selectedObjects;
			public Metadata.Property property;
			public object[] propertyIndexes;
			public bool allowNull = true;

			public Metadata.TypeInfo demandedType;

			//!!!!так?
			public bool newObjectWindow;
			public bool newObjectWindowFileCreation;
			//public bool newObjectWindowComponentClassOnly;

			//!!!!так?
			public bool selectTypeWindow;
			public Metadata.TypeInfo selectTypeDemandedType;
			public bool selectTypeWindowCanSelectAbstractClass;

			//!!!!так?
			//public delegate void AdditionalCheckCanSetDelegate( Metadata.TypeInfo typeToCheck, ref bool canSet );
			//public AdditionalCheckCanSetDelegate additionalCheckCanSet;

			//

			public Metadata.TypeInfo DemandedType
			{
				get
				{
					if( demandedType != null )
						return demandedType;
					else
						return property.TypeUnreferenced;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		////!!!!new
		//public delegate void ItemBeforeSelectDelegate( ItemTreeNode node, ref bool handled );
		//public event ItemBeforeSelectDelegate ItemBeforeSelect;

		public delegate void ItemAfterSelectDelegate( ContentBrowser sender, IList<Item> items, bool selectedByUser, ref bool handled );
		public event ItemAfterSelectDelegate ItemAfterSelect;

		public delegate void ItemAfterChooseDelegate( ContentBrowser sender, Item item, ref bool handled );
		public event ItemAfterChooseDelegate ItemAfterChoose;

		public delegate void OverrideItemTextDelegate( ContentBrowser sender, Item item, ref string text );
		public event OverrideItemTextDelegate OverrideItemText;

		public delegate void ShowContextMenuEventDelegate( ContentBrowser sender, List<KryptonContextMenuItemBase> items );
		public event ShowContextMenuEventDelegate ShowContextMenuEvent;

		public delegate void KeyDownOverrideDelegate( ContentBrowser browser, object sender, KeyEventArgs e, ref bool handled );
		public event KeyDownOverrideDelegate KeyDownOverride;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public abstract class Item : IDisposable
		{
			ContentBrowser owner;

			Item parent;
			public string imageKey;

			public bool chooseByDoubleClickAndReturnKey;

			public bool expandAtStartup;
			//!!!!
			public bool expandAllAtStartup;

			public bool wasExpanded;
			public bool childNodesCreated;

			public bool ShowDisabled
			{
				get { return showDisabled; }
				set
				{
					if( showDisabled == value )
						return;
					showDisabled = value;

					UpdateImage();

					var listItem = owner.GetListItemByItem( this );
					if( listItem != null )
					{
						listItem.ShowDisabled = ShowDisabled;
						owner.UpdateListItemImage( listItem );
					}
				}
			}

			bool showDisabled;
			//public bool showDisabled;

			//

			public Item( ContentBrowser owner, Item parent )
			{
				this.owner = owner;
				this.parent = parent;
			}

			public ContentBrowser Owner
			{
				get { return owner; }
			}

			public Item Parent
			{
				get { return parent; }
			}

			public abstract string Text
			{
				get;
			}

			public override string ToString()
			{
				return Text;
			}

			public abstract IList<Item> GetChildren( bool onlyAlreadyCreated );// bool forceUpdate );

			//!!!!всегда ли вызывается
			public abstract void Dispose();

			//!!!!!old: каждый раз новые дает ведь

			public event Action<Item> TextChanged;
			public event Action<Item> TextColorChanged;
			public event Action<Item> ChildrenChanged;

			public void PerformTextChanged()
			{
				TextChanged?.Invoke( this );
			}

			public void PerformTextColorChanged()
			{
				TextColorChanged?.Invoke( this );
			}

			public virtual void PerformChildrenChanged()
			{
				ChildrenChanged?.Invoke( this );
			}

			//!!!!!!
			public virtual object ContainedObject
			{
				get { return null; }
			}

			public void GetChildrenOnlyAlreadyCreatedRecursive( List<Item> result )
			{
				foreach( var child in GetChildren( true ) )
				{
					result.Add( child );
					child.GetChildrenOnlyAlreadyCreatedRecursive( result );
				}
			}

			public object Tag { get; set; }

			//special for engine
			public virtual void CalculateReferenceValue( Component from, Metadata.TypeInfo expectedType, out string referenceValue, out bool canSet )
			{
				referenceValue = "";
				canSet = false;
			}

			public virtual string GetDescription() { return ""; }
			//public virtual string ToolTip { get { return ""; } }

			public virtual void LightweightUpdate() { }

			//public virtual string GetDescription() { return ""; }
			//public virtual string GetDescription2() { return ""; }

			public virtual bool CanDoDragDrop() { return false; }

			public bool ShowDisabledInHierarchy
			{
				get
				{
					if( showDisabled )
						return true;
					return Parent != null && Parent.ShowDisabledInHierarchy;
				}
			}

			internal void UpdateImage()
			{
				if( owner.nodeByItem.TryGetValue( this, out var itemNode ) )
				{
					var image = owner.imageHelper.GetImageScaledForTreeView( imageKey, showDisabled );
					if( image == null )
						image = ContentBrowserImageHelperBasicImages.Helper.GetImageScaledForTreeView( imageKey, showDisabled );
					itemNode.Image = image;
				}
			}
		}

		//!!!!
		//    public abstract bool CanSearch { get; }
		//    public Image Icon { get; }

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public class ItemTreeNode : Node
		{
			public Item item;
			string textCached = "";

			public ItemTreeNode( Item item )
			{
				this.item = item;
			}

			public override string ToString()
			{
				return item.ToString();
			}

			public void UpdateText()
			{
				var newText = item.Text;
				item.Owner.OverrideItemText?.Invoke( item.Owner, item, ref newText );

				if( textCached != newText )
				{
					textCached = newText;
					Text = textCached;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		class DummyTreeNode : Node
		{
			public DummyTreeNode()
			{
				Text = "DUMMY";
			}

			public override string ToString()
			{
				return "DUMMY";
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public class DragDropItemData
		{
			public Item Item;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static List<ContentBrowser> AllInstances
		{
			get { return allInstances; }
		}

		public static List<ContentBrowserFilteringMode> FilteringModes
		{
			get { return filteringModes; }
		}

		[DefaultValue( BorderSides.All )]
		[Category( "Appearance" )]
		public BorderSides TreeViewBorderDraw
		{
			//TODO: replace impl. by KryptonBorderEdge

			get { return treeBordersContainer.BorderSides; }
			set { treeBordersContainer.BorderSides = value; }
		}

		BorderSides listViewBorderDraw = BorderSides.All;
		[DefaultValue( BorderSides.All )]
		[Category( "Appearance" )]
		public BorderSides ListViewBorderDraw
		{
			get { return listViewBorderDraw; }
			set
			{
				if( listViewBorderDraw == value )
					return;
				listViewBorderDraw = value;

				kryptonBorderEdgeT.Visible = ( value & BorderSides.Top ) != 0;
				kryptonBorderEdgeB.Visible = ( value & BorderSides.Bottom ) != 0;
				kryptonBorderEdgeL.Visible = ( value & BorderSides.Left ) != 0;
				kryptonBorderEdgeR.Visible = ( value & BorderSides.Right ) != 0;
			}
		}

		public ContentBrowser()
		{
			InitializeComponent();

			var tsbc = toolStripForListView.Items.OfType<ToolStripBreadCrumbHost>().First();
			breadCrumb = tsbc.BreadCrumb;
			breadCrumb.SelectedItemChanged += breadCrumb_SelectedItemChanged;

			options = new ContentBrowserOptions( this );

			treeViewModel = new TreeModel();
			treeView.Model = treeViewModel;

			SetPanelMode( PanelModeEnum.Tree, true );

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			listView.CanDrag = true;
			listView.BeforeStartDrag += ListView_BeforeStartDrag;
			//!!!!
			//listView.DropSink = new ContentBrowserDropSink();
			//listView.IsSimpleDragSource = true;
			//listView.CanDrop += listView_CanDrop;
			//listView.Dropped += listView_Dropped;

			MultiSelectUpdate();

			toolStripButtonOpen.Image = EditorResourcesCache.Edit;
			toolStripButtonClone.Image = EditorResourcesCache.Clone;
			toolStripButtonCopy.Image = EditorResourcesCache.Copy;
			toolStripButtonCut.Image = EditorResourcesCache.Cut;
			toolStripButtonPaste.Image = EditorResourcesCache.Paste;
			toolStripButtonDelete.Image = EditorResourcesCache.Delete;
			toolStripButtonEditor.Image = EditorResourcesCache.Edit;
			toolStripButtonMoveDown.Image = EditorResourcesCache.MoveDown;
			toolStripButtonMoveUp.Image = EditorResourcesCache.MoveUp;
			toolStripButtonNewFolder.Image = EditorResourcesCache.NewFolder;
			toolStripButtonNewObject.Image = EditorResourcesCache.New;
			toolStripButtonNewResource.Image = EditorResourcesCache.New;
			toolStripButtonOptions.Image = EditorResourcesCache.Options;
			toolStripButtonRename.Image = EditorResourcesCache.Rename;
			toolStripButtonSettings.Image = EditorResourcesCache.Settings;
			toolStripButtonShowMembers.Image = EditorResourcesCache.Type;
			toolStripButtonUp.Image = EditorResourcesCache.MoveUp;
			toolStripDropDownButtonFilteringMode.Image = EditorResourcesCache.Selection;
			toolStripButtonSearch.Image = EditorResourcesCache.Focus;

			foreach( var item in toolStripForTreeView.Items )
			{
				var button = item as ToolStripButton;
				if( button != null )
					button.Text = EditorLocalization.Translate( "ContentBrowser", button.Text );

				var button2 = item as ToolStripDropDownButton;
				if( button2 != null )
					button2.Text = EditorLocalization.Translate( "ContentBrowser", button2.Text );
			}

			toolStripForTreeView.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();
			toolStripForListView.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();
			toolStripSearch.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();

			if( EditorAPI.DarkTheme )
			{
				UpdateTreeViewListViewForDarkTheme();

				//!!!!
				//breadCrumb.OverrideToolStripRenderer = DarkThemeUtility.GetToolbarToolStripRenderer();
			}
		}

		public ModeEnum Mode
		{
			get { return mode; }
			set { mode = value; }
		}

		public void Init( DocumentWindow documentWindow, object rootObject, /*ResourcesModeDataClass resourcesModeData,*/
			SetReferenceModeDataClass setReferenceModeData )
		{
			this.initialized = true;
			this.documentWindow = documentWindow;
			this.rootObject = rootObject;
			//this.resourcesModeData = resourcesModeData;
			this.setReferenceModeData = setReferenceModeData;
		}

		[Browsable( false )]
		public bool Initialized
		{
			get { return initialized; }
		}

		[Browsable( false )]
		public DocumentWindow DocumentWindow
		{
			get { return documentWindow; }
		}

		[Browsable( false )]
		public object RootObject
		{
			get { return rootObject; }
		}

		//[Browsable( false )]
		//public ResourcesModeDataClass ResourcesModeData
		//{
		//	get { return resourcesModeData; }
		//}

		[Browsable( false )]
		public SetReferenceModeDataClass SetReferenceModeData
		{
			get { return setReferenceModeData; }
		}

		[DefaultValue( true )]
		public bool PreloadResourceOnSelection
		{
			get { return preloadResourceOnSelection; }
			set { preloadResourceOnSelection = value; }
		}

		[DefaultValue( false )]
		public bool CanSelectObjectSettings
		{
			get { return canSelectObjectSettings; }
			set { canSelectObjectSettings = value; }
		}

		[Browsable( false )]
		public bool ReadOnlyHierarchy
		{
			get { return readOnlyHierarchy; }
			set { readOnlyHierarchy = value; }
		}

		[Browsable( false )]
		public bool ThisIsSettingsWindow
		{
			get { return thisIsSettingsWindow; }
			set { thisIsSettingsWindow = value; }
		}

		private void ContentBrowser_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			allInstances.Add( this );

			//update toolstrip sizes
			{
				var dpiScale = Math.Min( EditorAPI.DPIScale, 2 );

				void UpdateSize( ToolStripItem item )
				{
					int width = 20;
					if( item is ToolStripDropDownButton )
						width = 28;
					item.Size = new Size( (int)( width * dpiScale ), (int)( 20 * dpiScale + 2 ) );
					//item.Size = new Size( (int)( width * EditorAPI.DPIScale + 2 ), (int)( 20 * EditorAPI.DPIScale + 2 ) );
				}

				toolStripForTreeView.Padding = new Padding( (int)dpiScale );
				toolStripForTreeView.Size = new Size( 10, (int)( 21 * dpiScale + 2 ) );

				foreach( var item in toolStripForTreeView.Items )
				{
					var button = item as ToolStripButton;
					if( button != null )
						UpdateSize( button );

					var button2 = item as ToolStripDropDownButton;
					if( button2 != null )
						UpdateSize( button2 );
				}


				toolStripForListView.Padding = new Padding( (int)dpiScale );
				toolStripForListView.Size = new Size( 10, (int)( 21 * dpiScale + 2 ) );

				//toolStripBreadCrumbHost.Padding = new Padding( (int)EditorAPI.DPIScale );
				//toolStripBreadCrumbHost.Size = new Size( 10, (int)( 21 * EditorAPI.DPIScale ) );

				UpdateSize( toolStripButtonUp );
			}

			//calibrate ItemHeight depending system DPI
			if( Math.Abs( DpiHelper.Default.DpiScaleFactor - 1.0 ) < 0.001 )
				treeView.RowHeight++;
			if( DpiHelper.Default.DpiScaleFactor > 1.499 )
				treeView.RowHeight++;
			if( DpiHelper.Default.DpiScaleFactor > 1.999 )
				treeView.RowHeight++;
			//if( EditorAPI.DPI < 96.0 * 1.25 )
			//	treeView.RowHeight++;
			////change ItemHeight depending system DPI
			//if( EditorAPI.DPI > 96 )
			//{
			//	double h = (double)treeView.RowHeight * EditorAPI.DPI / 96 / 1.1;
			//	treeView.RowHeight = (int)h;
			//}

			kryptonSplitContainerTree.Panel2MinSize = (int)( kryptonSplitContainerTree.Panel2MinSize * DpiHelper.Default.DpiScaleFactor );
			kryptonSplitContainerTree.SplitterDistance = 10000;
			kryptonSplitContainerTreeSub1.Panel2MinSize = (int)( kryptonSplitContainerTreeSub1.Panel2MinSize * DpiHelper.Default.DpiScaleFactor );
			kryptonSplitContainerTreeSub1.SplitterDistance = 10000;
			kryptonSplitContainerTreeSub2.Panel2MinSize = (int)( kryptonSplitContainerTreeSub2.Panel2MinSize * DpiHelper.Default.DpiScaleFactor );
			kryptonSplitContainerTreeSub2.SplitterDistance = 10000;

			//!!!!
			//InitData();

			//InitFilteringModes();
			UpdateToolBar();
			//UpdateSize();

			UpdateListMode( true );
			UpdateBreadcrumbVisibility();

			if( EditorAPI.DarkTheme )
			{
				UpdateTreeViewListViewForDarkTheme();

				treeBordersContainer.BorderColor = Color.FromArgb( 80, 80, 80 );

				breadCrumb.StateCommon.BreadCrumb.Content.ShortText.Color1 = Color.FromArgb( 230, 230, 230 );

				//!!!!

				breadCrumb.StateCommon.BreadCrumb.Back.Color1 = Color.FromArgb( 70, 70, 70 );
				//breadCrumb.StateCommon.BreadCrumb.Border.Color1 = Color.FromArgb( 90, 90, 90 );

				breadCrumb.StateTracking.BreadCrumb.Back.Color1 = Color.FromArgb( 80, 80, 80 );
				//breadCrumb.StateTracking.BreadCrumb.Border.Color1 = Color.FromArgb( 100,100,100 );

				breadCrumb.StatePressed.BreadCrumb.Back.Color1 = Color.FromArgb( 90, 90, 90 );

				//breadCrumb.StatePressed.BreadCrumb.Border.Color1 = Color.FromArgb( 110, 110, 110 );

				//breadCrumb.StateTracking.BreadCrumb.Back.Color1 = Color.Red;
				//breadCrumb.StateTracking.BreadCrumb.Border.Color1 = Color.Red;

				//breadCrumb.StateTracking.BreadCrumb.Content.ShortText.Color1 = Color.FromArgb( 230, 230, 230 );
				//breadCrumb.StatePressed.BreadCrumb.Content.ShortText.Color1 = Color.FromArgb( 230, 230, 230 );
				//breadCrumb.StateTracking.BreadCrumb.Back.Color1 = Color.FromArgb( 60, 60, 60 );
				//breadCrumb.StateTracking.BreadCrumb.Border.Color1 = Color.FromArgb( 60, 60, 60 );
				//breadCrumb.StatePressed.BreadCrumb.Back.Color1 = Color.FromArgb( 60, 60, 60 );
				//breadCrumb.StatePressed.BreadCrumb.Border.Color1 = Color.FromArgb( 60, 60, 60 );
				//breadCrumb.StateDisabled.BreadCrumb.Back.Color1 = Color.Red;
				//breadCrumb.StateDisabled.BreadCrumb.Border.Color1 = Color.Red;

				EditorThemeUtility.ApplyDarkThemeToToolTip( toolTip1 );
				EditorThemeUtility.ApplyDarkThemeToToolTip( toolTip2 );
			}

			//!!!!так?
			//update Show Members button
			toolStripButtonShowMembers.Enabled = Mode == ModeEnum.Objects;
			toolStripButtonShowMembers.Visible = toolStripButtonShowMembers.Enabled;

			if( panelMode != options.PanelMode )
				SetPanelMode( options.PanelMode, true );

			UpdateSplitterDistanceFromOptions();

			timer50ms.Start();

			if( Initialized )
				UpdateData();

			//ItemBeforeSelect += ContentBrowser_ItemBeforeSelect;
			ItemAfterSelect += ContentBrowser_ItemAfterSelect;
			//ItemAfterChoose += ContentBrowser_ItemAfterChoose;

			//!!!!только если есть файловые итемы
			//!!!!тут?
			VirtualFileWatcher.Update += VirtualFileWatcher_Update;
			Resource.Instance.AllInstances_StatusChanged += ResourceInstance_AllInstances_StatusChanged;
			Resource.Instance.AllInstances_DisposedEvent += ResourceInstance_AllInstances_DisposedEvent;

			toolStripDropDownButtonFilteringMode.Click += ToolStripDropDownButtonFilteringMode_Click;

			//initialize or not works
			toolTip2.SetToolTip( this.listView, "ToolTip" );

			engineScrollBarTreeVertical.Scroll += EngineScrollBarTreeVertical_Scroll;
			engineScrollBarTreeHorizontal.Scroll += EngineScrollBarTreeHorizontal_Scroll;

			//UpdateSize();
			UpdateScrollBars();

			loaded = true;
		}

		void UpdateTreeViewListViewForDarkTheme()
		{
			//treeView.BackColor = Color.FromArgb( 30, 30, 30 );
			treeView.BackColor = Color.FromArgb( 40, 40, 40 );
			treeView.FullRowSelectActiveColor = Color.FromArgb( 70, 70, 70 );
			treeView.FullRowSelectInactiveColor = Color.FromArgb( 60, 60, 60 );

			//listView.BackColor = Color.FromArgb( 30, 30, 30 );
			listView.BackColor = Color.FromArgb( 40, 40, 40 );
			listView.ForeColor = Color.FromArgb( 230, 230, 230 );

			kryptonSplitContainerTreeSub1.StateCommon.Back.Color1 = Color.FromArgb( 40, 40, 40 );
			kryptonSplitContainerTreeSub2.StateCommon.Back.Color1 = Color.FromArgb( 47, 47, 47 );
			kryptonSplitContainerTreeSub2.Panel2.StateCommon.Color1 = Color.FromArgb( 47, 47, 47 );
		}

		protected override void OnDestroy()
		{
			if( !EditorAPI.ClosingApplication )
				ClearData();

			base.OnDestroy();

			if( !WinFormsUtility.IsDesignerHosted( this ) )
			{
				//!!!!

				VirtualFileWatcher.Update -= VirtualFileWatcher_Update;
				Resource.Instance.AllInstances_StatusChanged -= ResourceInstance_AllInstances_StatusChanged;
				Resource.Instance.AllInstances_DisposedEvent -= ResourceInstance_AllInstances_DisposedEvent;
			}

			imageHelper?.Dispose();

			allInstances.Remove( this );
		}

		void ClearData()
		{
			Item[] toDispose = items.ToArray();

			items = new ESet<Item>();
			itemByNode = new Dictionary<ItemTreeNode, Item>();
			nodeByItem = new Dictionary<Item, ItemTreeNode>();
			rootItems.Clear();

			TreeViewBeginUpdate();
			treeViewModel.Nodes.Clear();
			TreeViewEndUpdate();

			//listView.BeginUpdate();
			listView.ClearItems();
			//listView.EndUpdate();

			foreach( var item in toDispose )
			{
				item.TextChanged -= Item_TextChanged;
				item.TextColorChanged -= Item_TextColorChanged;
				item.ChildrenChanged -= Item_ChildrenChanged;

				item.Dispose();
			}
		}

		public void SetPanelMode( PanelModeEnum mode, bool forceUpdate = false )
		{
			if( mode != panelMode || forceUpdate )
			{
				panelMode = mode;

				kryptonSplitContainer1.Panel1Collapsed = panelMode == PanelModeEnum.List;
				kryptonSplitContainer1.Panel2Collapsed = panelMode == PanelModeEnum.Tree;

				if( panelMode == PanelModeEnum.TwoPanelsSplitHorizontally || panelMode == PanelModeEnum.TwoPanelsSplitVertically )
					kryptonSplitContainer1.Orientation = panelMode == PanelModeEnum.TwoPanelsSplitHorizontally ? Orientation.Horizontal : Orientation.Vertical;

				if( panelMode != PanelModeEnum.Tree )
					UpdateList();
				//!!!!
				//InitData();
			}
		}

		private void Item_TextChanged( Item item )
		{
			if( EditorAPI.ClosingApplication )
				return;
			if( DocumentWindow?.Document != null && DocumentWindow.Document.Destroyed )
				return;

			var node = GetNodeByItem( item );
			if( node != null )
				node.UpdateText();

			var listItem = GetListItemByItem( item );
			if( listItem != null )
				listItem.Text = item.Text;
		}

		private void Item_TextColorChanged( Item item )
		{
			if( EditorAPI.ClosingApplication )
				return;
			if( DocumentWindow?.Document != null && DocumentWindow.Document.Destroyed )
				return;

			var node = GetNodeByItem( item );
			if( node != null )
				node.NotifyModel();
		}

		public ItemTreeNode GetNodeByItem( Item item )
		{
			nodeByItem.TryGetValue( item, out ItemTreeNode node );
			return node;
		}

		public Item GetItemByNode( ItemTreeNode node )
		{
			if( node == null )
				return null;

			itemByNode.TryGetValue( node, out Item item );
			return item;
		}

		void AddChildNodes( Item item, SetDataContext setDataContext )
		{
			if( item.Parent != null )
				AddChildNodes( item.Parent, setDataContext );

			if( !item.childNodesCreated )
			{
				item.childNodesCreated = true;

				var itemNode = GetNodeByItem( item );
				if( itemNode == null )
					Log.Fatal( "ContentBrowser: AddChildrenNodes: itemNode == null." );

				//remove dummy node
				if( itemNode.Nodes.Count == 1 && itemNode.Nodes[ 0 ] is DummyTreeNode )
				{
					TreeViewBeginUpdate();
					itemNode.Nodes.Clear();
				}

				//add nodes
				foreach( var childItem in item.GetChildren( false ) )
					AddItem( item, childItem, -1, setDataContext );
			}
		}


		void AddItem( Item parent, Item item, int insertPosition, SetDataContext setDataContext )
		{
			TreeViewBeginUpdate();

			ItemTreeNode itemNode = new ItemTreeNode( item );
			items.Add( item );
			itemByNode[ itemNode ] = item;
			nodeByItem[ item ] = itemNode;
			itemNode.UpdateText();

			item.UpdateImage();
			//itemNode.Image = imageHelper.GetImageForTreeNode( item.imageKey );

			if( parent != null )
			{
				var parentNode = GetNodeByItem( parent );
				if( insertPosition != -1 )
					parentNode.Nodes.Insert( insertPosition, itemNode );
				else
					parentNode.Nodes.Add( itemNode );
			}
			else
			{
				if( insertPosition != -1 )
					rootItems.Insert( insertPosition, item );
				else
					rootItems.Add( item );

				if( insertPosition != -1 )
					treeViewModel.Nodes.Insert( insertPosition, itemNode );
				else
					treeViewModel.Nodes.Add( itemNode );
			}

			//add dummy node
			if( item.GetChildren( false ).Count != 0 )
				itemNode.Nodes.Add( new DummyTreeNode() );
			//foreach( var child in item.GetChildren() )// false ) )
			//	AddItem( item, child, -1 );

			//expand nodes
			if( setDataContext != null )
			{
				if( item.expandAllAtStartup )
					setDataContext.nodesToExpandAllAtStartup.Add( itemNode );
				else if( item.expandAtStartup )
					setDataContext.nodesToExpandAtStartup.Add( itemNode );
			}
			else
			{
				if( item.expandAllAtStartup )
					FindTreeViewNode( itemNode ).ExpandAll();
				else if( item.expandAtStartup )
					FindTreeViewNode( itemNode ).Expand();
			}

			item.TextChanged += Item_TextChanged;
			item.TextColorChanged += Item_TextColorChanged;
			item.ChildrenChanged += Item_ChildrenChanged;

			////select demanded file node
			//if( needSelectFiles.Count != 0 )
			//{
			//	var fileItem = item as ContentBrowserItem_File;
			//	if( fileItem != null )
			//	{
			//		bool contains = false;
			//		foreach( var path in needSelectFiles )
			//		{
			//			if( string.Compare( path, fileItem.FullPath, true ) == 0 )
			//				contains = true;
			//		}
			//		if( contains )
			//		{
			//			if( nodeByItem.TryGetValue( item, out ItemTreeNode node2 ) )
			//			{
			//				treeView.SelectedNode = FindTreeViewNode( node2 );
			//				treeView.EnsureVisible( treeView.SelectedNode );
			//			}
			//		}
			//	}
			//}
		}

		public void RemoveItem( Item item )
		{
			//!!!!!список еще есть

			var node = GetNodeByItem( item );

			item.TextChanged -= Item_TextChanged;
			item.TextColorChanged -= Item_TextColorChanged;
			item.ChildrenChanged -= Item_ChildrenChanged;

			if( node != null )
			{
				foreach( var childNode in new List<Node>( node.Nodes ) )
				{
					ItemTreeNode itemChildNode = childNode as ItemTreeNode;
					if( itemChildNode != null )
					{
						var childItem = GetItemByNode( itemChildNode );
						if( childItem != null )
							RemoveItem( childItem );
					}
				}

				TreeViewBeginUpdate();

				if( node.Parent != null )
					node.Parent.Nodes.Remove( node );
				else
					treeViewModel.Nodes.Remove( node );

				itemByNode.Remove( node );
				nodeByItem.Remove( item );
			}

			items.Remove( item );
			rootItems.Remove( item );
		}

		internal void Item_ChildrenChanged( Item item )
		{
			if( EditorAPI.ClosingApplication || !IsHandleCreated )
				return;
			if( DocumentWindow?.Document != null && DocumentWindow.Document.Destroyed )
				return;

			var node = GetNodeByItem( item );
			if( node == null )
				return;

			if( ContentBrowserUtility.allContentBrowsers_SuspendChildrenChangedEvent )
			{
				ContentBrowserUtility.allContentBrowsers_SuspendChildrenChangedEvent_Items.AddWithCheckAlreadyContained( (this, item) );
				return;
			}

			var newItems = item.GetChildren( false );

			//!!!!было. тормозит при открытии мемберов (небольшое изменение)
			//treeView.BeginUpdate();

			//try
			//{
			if( !item.childNodesCreated )
			//if( !item.wasExpanded )
			{
				//child nodes was not created
				////was not expanded

				if( newItems.Count != 0 )
				{
					//add dummy node
					if( node.Nodes.Count == 0 )
					{
						TreeViewBeginUpdate();
						node.Nodes.Add( new DummyTreeNode() );
					}
				}
				else
				{
					//remove dummy node
					if( node.Nodes.Count == 1 && node.Nodes[ 0 ] is DummyTreeNode )
					{
						TreeViewBeginUpdate();
						node.Nodes.Clear();
					}
				}
			}
			else
			{
				//child nodes was created
				////was expanded

				//!!!!new
				//remove old
				try
				{
					var newItemsSet = new ESet<Item>( newItems );

					var itemsToRemove = new List<Item>();
					foreach( ItemTreeNode childNode in node.Nodes )
					{
						if( !newItemsSet.Contains( childNode.item ) )
							itemsToRemove.Add( childNode.item );
					}

					if( itemsToRemove.Count != 0 )
					{
						foreach( var childItem in itemsToRemove.GetReverse() )
							RemoveItem( childItem );
					}
				}
				catch { }

				for( int n = 0; n < newItems.Count; n++ )
				{
					var newItem = newItems[ n ];

					//check need update
					var childNode = GetNodeByItem( newItem );
					if( childNode == null || ( n >= node.Nodes.Count || childNode != node.Nodes[ n ] || childNode.Parent != node ) )
					{
						if( childNode != null )
						{
							//change position
							TreeViewBeginUpdate();
							childNode.Parent.Nodes.Remove( childNode );//childNode.Remove();
							node.Nodes.Insert( n, childNode );
						}
						else
						{
							//add new
							AddItem( item, newItem, n, null );
						}
					}
				}

				//!!!!old, need?
				//remove old
				try//can crash because dummy item still exists. strange
				{
					while( node.Nodes.Count > newItems.Count )
					{
						var childNode = (ItemTreeNode)node.Nodes[ node.Nodes.Count - 1 ];
						var childItem = GetItemByNode( childNode );
						if( childItem != null )
							RemoveItem( childItem );
					}
				}
				catch { }
			}
			//}
			//finally
			//{
			//	//!!!!было. тормозит при открытии мемберов (небольшое изменение)
			//	//treeView.EndUpdate();
			//}

			//update list
			if( GetTreeSelectedItems().Contains( item ) )
				UpdateList();
		}

		class SetDataContext
		{
			public List<ItemTreeNode> nodesToExpandAtStartup = new List<ItemTreeNode>();
			public List<ItemTreeNode> nodesToExpandAllAtStartup = new List<ItemTreeNode>();
		}

		public void SetData( IList<Item> roots, bool showPlusMinus = true )
		{
			setDataMethodCalling = true;

			ClearData();

			TreeViewBeginUpdate();

			if( treeView.ShowPlusMinus != showPlusMinus )
				treeView.ShowPlusMinus = showPlusMinus;

			//optimization. detach model.
			var setDataContext = new SetDataContext();
			treeView.Model = null;

			foreach( var item in roots )
				AddItem( null, item, -1, setDataContext );

			//precreate to get the ability to find items
			foreach( var rootItem in roots )
			{
				AddChildNodes( rootItem, setDataContext );

				//!!!!так?

				List<Item> list = new List<Item>( 16384 );
				rootItem.GetChildrenOnlyAlreadyCreatedRecursive( list );
				foreach( var item in list )
				{
					var parent = item.Parent;
					if( parent != null && ( parent is ContentBrowserItem_Type || parent is ContentBrowserItem_Virtual ) )
						AddChildNodes( parent, setDataContext );
				}
			}

			//optimization. attach model.
			treeView.Model = treeViewModel;
			//expand nodes from context. must done after reattach model.
			foreach( var node in setDataContext.nodesToExpandAllAtStartup )
				FindTreeViewNode( node ).ExpandAll();
			foreach( var node in setDataContext.nodesToExpandAtStartup )
				FindTreeViewNode( node ).Expand();

			TreeViewEndUpdate();

			//!!!!new
			UpdateScrollBars();

			UpdateBreadcrumb();
			UpdateList();

			setDataMethodCalling = false;
		}

		//public void SetError( string text )
		//{
		//	//TODO: switch to list.
		//	Enabled = false;

		//	//!!!!
		//	//listView.EmptyListMsgFont = this.Font;
		//	////listView.EmptyListMsgOverlay = null;
		//	//listView.EmptyListMsg = text;
		//}

		public void AddRootItem( Item item )
		{
			AddItem( null, item, -1, null );
		}

		//private void treeView_BeforeSelect( object sender, TreeViewCancelEventArgs e )
		//{
		//	//ItemTreeNode node = e.Node as ItemTreeNode;
		//	//bool handled = false;
		//	//ItemBeforeSelect?.Invoke( node, ref handled );
		//}

		void PerformTreeView_SelectionChanged()
		{
			bool selectedByUser = !selectItemsMethodCalling && !setDataMethodCalling;

			//!!!!!

			UpdateBreadcrumb();
			UpdateList();

			bool handled = false;
			ItemAfterSelect?.Invoke( this, GetTreeSelectedItems(), selectedByUser, ref handled );

			UpdateListMode( false );
		}

		private void treeView_SelectionChanged( object sender, EventArgs e )
		{
			if( treeView_SelectionChangedEnabled )
				PerformTreeView_SelectionChanged();

			//bool selectedByUser = !selectItemsMethodCalling && !setDataMethodCalling;

			////!!!!!

			//UpdateBreadcrumb();
			//UpdateList();

			//bool handled = false;
			//ItemAfterSelect?.Invoke( this, GetTreeSelectedItems(), selectedByUser, ref handled );

			//UpdateListMode( false );
		}

		private void treeView_MouseDown( object sender, MouseEventArgs e )
		{
			////!!!!good?
			////maybe better another variant: https://stackoverflow.com/questions/1249312/disable-expanding-after-doubleclick
			//if( e.Clicks > 1 )
			//	lastDoubleClick = true;
			//else
			//	lastDoubleClick = false;
		}

		private void treeView_NodeMouseDoubleClick( object sender, TreeNodeAdvMouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
			{
				ItemTreeNode node = GetItemTreeNode( treeView.SelectedNode );
				if( node != null )
				{
					if( node.Nodes.Count == 0 || node.item.chooseByDoubleClickAndReturnKey )
					{
						TreeViewTryChoose();

						e.Handled = true;
					}
				}
				//!!!!было, но для chooseByDoubleClickAndReturnKey неправильно
				//if( e.Node != treeView.SelectedNode )
				//	return;
				//ItemTreeNode node = treeView.SelectedNode as ItemTreeNode;
				//if( e.Node.Nodes.Count == 0 || ( node != null && node.item.chooseByDoubleClickAndReturnKey ) )
				//	TryChoose();
			}
		}

		bool IsLetterOrNumber( Keys key )
		{
			if( key >= Keys.D0 && key <= Keys.D9 )
				return true;
			if( key >= Keys.A && key <= Keys.Z )
				return true;
			return false;
		}

		private void treeView_KeyDown( object sender, KeyEventArgs e )
		{
			//KeyDownOverride
			{
				bool handled = false;
				KeyDownOverride?.Invoke( this, sender, e, ref handled );
				if( handled )
					return;
			}

			if( e.KeyCode == Keys.Return )
			{
				ItemTreeNode node = GetItemTreeNode( treeView.SelectedNode );
				if( node != null )
				{
					TreeViewTryChoose();
					e.Handled = true;
					return;
				}
			}

			//rename
			{
				var shortcuts = EditorAPI.GetActionShortcuts( "Rename" );
				if( shortcuts != null )
				{
					foreach( var shortcut in shortcuts )
					{
						Keys keys = e.KeyCode | ModifierKeys;
						if( shortcut == keys )
						{
							RenameBegin();
							return;
						}
					}
				}

				//if( e.KeyCode == Keys.F2 )
				//{
				//	RenameBegin();
				//	return;
				//}
			}

			//process shortcuts
			if( ModifierKeys != Keys.None || !IsLetterOrNumber( e.KeyCode ) )
			{
				if( EditorAPI.ProcessShortcuts( e.KeyCode ) )
				{
					e.Handled = true;
					return;
				}
			}
		}

		void TreeViewTryChoose()
		{
			ItemTreeNode node = GetItemTreeNode( treeView.SelectedNode );
			if( node != null )
			{
				bool handled = false;
				PerformItemAfterChoose( node.item, ref handled );
				//ItemAfterChoose?.Invoke( this, node.item, ref handled );
			}
		}

		private void ContentBrowser_Resize( object sender, EventArgs e )
		{
			//UpdateSize();
		}

		private void treeView_MouseClick( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Right )
			{
				//update selected node
				var nodeAdv = treeView.GetNodeAt( e.Location );
				ItemTreeNode node = GetItemTreeNode( nodeAdv );
				if( node != null && !treeView.SelectedNodes.Contains( nodeAdv ) )
					treeView.SelectedNode = nodeAdv;

				var item = GetItemByNode( node );
				ShowContextMenu( item, treeView, e.Location );
			}
		}

		private void treeView_MouseUp( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Right )
			{
				var node = GetItemTreeNode( treeView.GetNodeAt( e.Location ) );
				if( node == null )
					ShowContextMenu( null, treeView, e.Location );
			}
		}

		public Item[] GetAllItems()
		{
			Item[] array = new Item[ nodeByItem.Keys.Count ];
			nodeByItem.Keys.CopyTo( array, 0 );
			return array;
		}

		public void SelectItems( IList<Item> items, bool expandNodes = false, bool considerAsSelectedByUser = false )
		//public void SelectItems( Item[] items, bool expandNodes = false )
		{
			if( items == null )
				items = new Item[ 0 ];

			if( !considerAsSelectedByUser )
				selectItemsMethodCalling = true;

			try
			{
				//!!!!!какие-то проверки, что-то еще

				//!!!!list support

				if( items.Count != 0 )
				{
					var nodes = new List<TreeNodeAdv>();
					foreach( var item in items )
					{
						nodeByItem.TryGetValue( item, out var node );
						if( node != null )
						{
							var nodeAdv = FindTreeViewNode( node );
							if( nodeAdv != null )
								nodes.Add( nodeAdv );
						}
					}

					if( nodes.Count != 0 )
					{
						//select
						if( nodes.Count > 1 )
						{
							TreeViewBeginUpdate();
							treeView_SelectionChangedEnabled = false;
							try
							{
								treeView.ClearSelection();
								foreach( var node in nodes )
									node.IsSelected = true;
							}
							finally
							{
								treeView_SelectionChangedEnabled = true;
							}
							PerformTreeView_SelectionChanged();
							//TreeViewEndUpdate();
						}
						else if( nodes.Count == 1 )
							treeView.SelectedNode = nodes[ 0 ];
						else
							treeView.SelectedNode = null;

						//!!!!ensure multiselection
						//expand, ensure visible
						if( expandNodes )
						{
							foreach( var node in nodes )
							{
								node.Expand();

								//ensure visible children when expand
								var children = node.Children;
								if( children.Count != 0 )
								{
									var last = children[ children.Count - 1 ];
									treeView.EnsureVisible( last );
								}
							}
						}

						//ensure visible
						if( nodes.Count > 1 )
						{
							int maxNodeIndex = nodes[ 0 ].Row, maxNodeId = 0;
							int minNodeIndex = nodes[ 0 ].Row, minNodeId = 0;

							for( int i = 0; i <= nodes.Count - 1; i++ )
							{
								if( nodes[ i ].Row > maxNodeIndex )
								{
									maxNodeIndex = nodes[ i ].Row;
									maxNodeId = i;
								}
								if( nodes[ i ].Row < minNodeIndex )
								{
									minNodeIndex = nodes[ i ].Row;
									minNodeId = i;
								}
							}

							treeView.EnsureVisible( nodes[ maxNodeId ] );
							treeView.EnsureVisible( nodes[ minNodeId ] );
						}
						else if( nodes.Count == 1 )
							treeView.EnsureVisible( nodes[ 0 ] );


						// EditorMessageBox.ShowWarning( nodes[0].Index.ToString() );

					}
					else
						treeView.SelectedNode = null;
				}
				else
					treeView.SelectedNode = null;
			}
			finally
			{
				if( !considerAsSelectedByUser )
					selectItemsMethodCalling = false;
			}
		}

		[Browsable( false )]
		public Item[] SelectedItems
		{
			get
			{
				List<Item> result = new List<Item>();

				if( listView.SelectedItems.Count != 0 )
				{
					foreach( var item in listView.SelectedItems )
						result.Add( (Item)item.Tag );
				}
				else
					result.AddRange( GetTreeSelectedItems() );

				return result.ToArray();
			}
		}

		//!!!!!надо ли
		//public int GetParentIndex( Item item )
		//{
		//	//slowly

		//	if( item.Parent != null )
		//	{
		//		//!!!!рефрешить будет?
		//		//!!!!
		//		Log.Warning( "impl" );

		//		return item.Parent.GetChildren( false ).IndexOf( item );
		//	}
		//	else
		//		return rootItems.IndexOf( item );
		//}

		private void timer50ms_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
				return;

			//UpdateSize();
			UpdateScrollBars();

			double updateTime = 0.05;
			{
				var count = GetSelectedComponents().Count;
				if( count > 2000 )
					updateTime = 2.0;
				else if( count > 500 )
					updateTime = 1.0;
				else if( count > 250 )
					updateTime = 0.5;
				else if( count > 100 )
					updateTime = 0.35;
				else
					updateTime = 0.2;
			}
			if( EngineApp.GetSystemTime() - lastUpdateTime < updateTime )
				return;

			UpdateToolBar();

			if( panelMode != options.PanelMode )
				SetPanelMode( options.PanelMode, true );
			UpdateListMode( false );
			UpdateBreadcrumbVisibility();

			////!!!!!
			////update Text of nodes for items with class ContentBrowserItem_NoSpecialization
			//foreach( var node in itemByNode.Keys )
			//{
			//	if( node.item is ContentBrowserItem_NoSpecialization )
			//		node.UpdateText();
			//}

			//update SortBy, SortByAscending
			if( Mode == ModeEnum.Resources || Mode == ModeEnum.SetReference )
			{
				if( updatedDataSortBy != options.SortFilesBy || updatedDataSortByAscending != options.SortFilesByAscending )
				{
					updatedDataSortBy = options.SortFilesBy;
					updatedDataSortByAscending = options.SortFilesByAscending;

					//!!!!итемы схлопываются. но не во всех случаях.

					foreach( var item in GetAllItems() )
					{
						var fileItem = item as ContentBrowserItem_File;
						if( fileItem != null && fileItem.IsDirectory )
							fileItem.PerformChildrenChanged();
					}
				}
			}

			UpdateNeedSelectFiles();

			UpdateSplitterDistanceFromOptions();

			//LightweightUpdate
			foreach( var item in GetAllItems() )
				item.LightweightUpdate();

			//update Button Up
			bool enable = breadCrumb.SelectedItem?.Parent != null;
			if( toolStripButtonUp.Enabled != enable )
				toolStripButtonUp.Enabled = enable;

			TreeViewEndUpdate();

			//теперь выше
			//UpdateSize();
			//UpdateScrollBars();

			//white blinking
			if( !listView.Visible )
				listView.Visible = true;

			lastUpdateTime = EngineApp.GetSystemTime();
		}

		public void UpdateData()
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			updatedDataSortBy = options.SortFilesBy;
			updatedDataSortByAscending = options.SortFilesByAscending;

			if( EditorAPI.ClosingApplication )
				return;

			if( Mode == ModeEnum.Objects )
				UpdateData_Objects();
			else if( Mode == ModeEnum.Resources || Mode == ModeEnum.SetReference )
				UpdateData_ResourcesAndSetReference();
			else
				Log.Fatal( "impl" );
		}

		void UpdateData_Objects()
		{
			//!!!!!

			List<Item> roots = new List<Item>();

			//ContentBrowser_VirtualItem qaItem = null;
			//{
			//	qaItem = new ContentBrowser_VirtualItem( null, "Quick access" );
			//	qaItem.children.Add( new ContentBrowser_VirtualItem( qaItem, "item 1" ) );
			//	qaItem.children.Add( new ContentBrowser_VirtualItem( qaItem, "item 2" ) );
			//	qaItem.children.Add( new ContentBrowser_VirtualItem( qaItem, "item 3" ) );

			//	qaItem.expandAtStartup = true;
			//}

			var rootComponent = rootObject as Component;
			if( rootComponent != null )
			{
				//Component

				var dataItem = new ContentBrowserItem_Component( this, null, rootComponent );

				dataItem.expandAtStartup = true;
				//!!!!глубже экспандить?

				//!!!!!
				//!!!!можно в зависимости от количества объектов. если мало, то разворачивать
				//!!!!!!еще запоминать можно
				if( thisIsSettingsWindow )
					dataItem.expandAllAtStartup = true;

				roots.Add( dataItem );
			}
			//else if( ContentBrowserItem_List.IsList( rootObject ) )
			//{
			//	var item = new ContentBrowserItem_List( this, null, rootObject, null );
			//	item.expandAtStartup = true;
			//	roots.Add( item );
			//}
			else
			{
				//var item = new ContentBrowserItem_NoSpecialization( this, null, rootObject, null );
				//item.expandAtStartup = true;
				//roots.Add( item );
			}

			SetData( roots );
		}

		bool ResourcesAndSetReference_SkipNetTypeCompletely( Type netType )
		{
			if( netType.IsArray )
				return true;

			//skip not definition generic types
			if( netType.IsGenericType && !netType.IsGenericTypeDefinition )
				return true;

			//!!!!что-то еще?
			//!!!!Browsable attribute?

			return false;
		}

		string GetTypeImageKey( Metadata.TypeInfo type )
		{
			switch( type.Classification )
			{
			case Metadata.TypeClassification.Enumeration: return "Enum";
			case Metadata.TypeClassification.Delegate: return "Delegate";
			}
			return "Class";
		}

		void AddNestedTypesRecursive( ContentBrowserItem_Type item )
		{
			Type netType = item.type.GetNetType();

			var netNestedTypes = new List<Type>( netType.GetNestedTypes() );
			//sort types
			CollectionUtility.MergeSort( netNestedTypes, delegate ( Type v1, Type v2 )
			{
				return string.Compare( v1.Name, v2.Name );
			} );

			//!!!!binding attr?
			foreach( var netNestedType in netNestedTypes )
			{
				if( !ResourcesAndSetReference_SkipNetTypeCompletely( netNestedType ) ||
					( Mode == ModeEnum.SetReference && setReferenceModeData.selectTypeWindow ) )//!!!!?
				{
					bool skip = false;
					if( Mode == ModeEnum.SetReference && netNestedType.IsEnum )
						skip = true;

					if( !skip && Mode == ModeEnum.SetReference && setReferenceModeData.newObjectWindow )
					{
						if( !ContentBrowserUtility.ContainsComponentClasses( netType ) )
							skip = true;
						//if( !ReferenceUtils.TypeOrItsMembersCanReferencedToComponentType( netType ) )
						//	skip = true;
					}
					if( !skip && Mode == ModeEnum.SetReference && setReferenceModeData.selectTypeWindow )
					{
						if( !ContentBrowserUtility.ContainsType( setReferenceModeData.selectTypeDemandedType.GetNetType(), netType ) )
							skip = true;
						//if( !ContentBrowserUtils.ContainsType( setReferenceModeData.demandedType.GetNetType(), netType ) )
						//	skip = true;
					}

					if( !skip )
					{
						Metadata.TypeInfo nestedType = MetadataManager.GetTypeOfNetType( netNestedType );
						if( nestedType != null )
						{
							var name = MetadataManager.GetNetTypeName( netNestedType, true, false );
							var nestedItem = new ContentBrowserItem_Type( this, item, nestedType, name );
							nestedItem.imageKey = GetTypeImageKey( nestedType );
							item.nestedTypeChildren.Add( nestedItem );

							AddNestedTypesRecursive( nestedItem );
						}
					}
				}
			}
		}

		//void Resources_SelectType_RemoveExcessTypeItems( ContentBrowserItem_Virtual classesItem )
		//{
		//	//!!!!slowly. или может выше медленно

		//	List<Item> allChildren = new List<Item>( 16384 );
		//	classesItem.GetChildrenOnlyAlreadyCreatedRecursive( allChildren );

		//	//enumerate reverse
		//	foreach( var item in allChildren.GetReverse() )
		//	{
		//		//check can create
		//		bool allow;
		//		{
		//			var typeItem = item as ContentBrowserItem_Type;
		//			if( typeItem != null )
		//			{
		//				allow = true;

		//				var netType = typeItem.Type.GetNetType();
		//				if( netType.IsAbstract )
		//					allow = false;

		//				if( resourcesModeData.demandedType != null && !resourcesModeData.demandedType.IsAssignableFrom( typeItem.Type ) )
		//					allow = false;

		//				//!!!!что-то еще?
		//			}
		//			else
		//				allow = false;
		//		}

		//		bool remove = !allow && item.GetChildren( true ).Count == 0;
		//		if( remove )
		//		{
		//			var parent = item.Parent;

		//			var typeItem = parent as ContentBrowserItem_Type;
		//			var virtualItem = parent as ContentBrowserItem_Virtual;

		//			if( typeItem != null )
		//				typeItem.nestedTypeChildren.Remove( item );
		//			else if( virtualItem != null )
		//				virtualItem.children.Remove( item );
		//			else
		//			{
		//				//!!!!что-то еще?
		//			}
		//		}
		//	}
		//}

		//!!!!пока так
		void Resources_RemoveExcessClassesItemsByFilteringMode( ContentBrowserItem_Virtual classesItem )
		{
			List<Item> allChildren = new List<Item>( 16384 );
			classesItem.GetChildrenOnlyAlreadyCreatedRecursive( allChildren );

			//enumerate reverse
			foreach( var item in allChildren.GetReverse() )
			{
				//check can create
				bool allow;
				{
					var typeItem = item as ContentBrowserItem_Type;
					if( typeItem != null )
					{
						allow = true;

						if( !FilteringMode.AddItem( typeItem ) )
							allow = false;

						//!!!!что-то еще?
					}
					else
						allow = false;
				}

				bool remove = !allow && item.GetChildren( false ).Count == 0;
				//bool remove = !allow && item.GetChildren( true ).Count == 0;
				if( remove )
				{
					var parent = item.Parent;

					var typeItem = parent as ContentBrowserItem_Type;
					var virtualItem = parent as ContentBrowserItem_Virtual;

					if( typeItem != null )
						typeItem.nestedTypeChildren.Remove( item );
					else if( virtualItem != null )
						virtualItem.children.Remove( item );
					else
					{
						//!!!!что-то еще?
					}
				}
			}
		}

		void SetReference_RemoveExcessTypeItems( ContentBrowserItem_Virtual classesItem )
		{
			//!!!!slowly. или может выше медленно

			List<Item> allChildren = new List<Item>( 16384 );
			classesItem.GetChildrenOnlyAlreadyCreatedRecursive( allChildren );

			//enumerate reverse
			foreach( var item in allChildren.GetReverse() )
			{
				//check need to add
				bool allow;
				{
					var typeItem = item as ContentBrowserItem_Type;
					if( typeItem != null )
					{
						allow = false;

						var demandedType = SetReferenceModeData.DemandedType;

						if( MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ).IsAssignableFrom( demandedType ) )
						{
							//specialization for Metadata.TypeInfo
							allow = true;

							//!!!!нужно это убрать, если включить возможность получать тип из мембера.
							if( SetReferenceModeData.newObjectWindow )
							{
								var type = typeItem.type;
								if( !MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( type ) || type.Abstract )
									allow = false;
							}
							if( SetReferenceModeData.selectTypeWindow )
							{
								var type = typeItem.type;
								if( !setReferenceModeData.selectTypeDemandedType.IsAssignableFrom( type ) || type.Abstract && !setReferenceModeData.selectTypeWindowCanSelectAbstractClass )
									allow = false;
								//if( !setReferenceModeData.demandedType.IsAssignableFrom( type ) || type.Abstract && !setReferenceModeData.selectTypeWindowCanSelectAbstractClass )
								//	allow = false;
							}
						}
						else
						{
							//default behaviour
							foreach( var member in typeItem.Type.MetadataGetMembers() )
							{
								if( ContentBrowserUtility.ContentBrowserSetReferenceModeCheckAllowAddMember( demandedType, member, true ) )
								{
									allow = true;
									break;
								}
							}
						}
					}
					else
						allow = false;
				}

				//bool existChildren = item.GetChildren().Count != 0;
				//bool existChildren = false;
				//{
				//	//skip DUMMY
				//	foreach( var i in item.GetChildren() )
				//	{
				//		var virtualItem = i as ContentBrowserItem_Virtual;
				//		if( virtualItem == null || !virtualItem.Dummy )
				//		{
				//			existChildren = true;
				//			break;
				//		}
				//	}
				//}

				//bool remove = !existChildren && !allow;
				bool remove = !allow && item.GetChildren( true ).Count == 0;
				if( remove )
				{
					var parent = item.Parent;

					var typeItem = parent as ContentBrowserItem_Type;
					var virtualItem = parent as ContentBrowserItem_Virtual;

					if( typeItem != null )
						typeItem.nestedTypeChildren.Remove( item );
					else if( virtualItem != null )
						virtualItem.children.Remove( item );
					else
					{
						//!!!!что-то еще?
					}
				}
			}
		}

		void UpdateData_ResourcesAndSetReference()
		{
			List<Item> roots = new List<Item>();

			//!!!!
			////quick access
			//ContentBrowserItem_Virtual qaItem = null;
			//{
			//	qaItem = new ContentBrowserItem_Virtual( this, null, "Quick access" );

			//	qaItem.children.Add( new ContentBrowserItem_Virtual( this, qaItem, "item 1" ) );
			//	qaItem.children.Add( new ContentBrowserItem_Virtual( this, qaItem, "item 2" ) );
			//	qaItem.children.Add( new ContentBrowserItem_Virtual( this, qaItem, "item 3" ) );

			//	qaItem.expandAtStartup = true;
			//}
			//if( qaItem != null )
			//	roots.Add( qaItem );

			/////////////////////////////////////
			//this
			if( Mode == ModeEnum.SetReference && setReferenceModeData.selectedComponents != null )
			{
				ESet<Component> processed = new ESet<Component>();

				Component thisComponent = null;
				{
					foreach( var obj in setReferenceModeData.selectedComponents )
					{
						var c = obj as Component;
						if( c != null )
						{
							if( thisComponent == null )
								thisComponent = c;
							else
							{
								if( thisComponent != c )
								{
									thisComponent = null;
									break;
								}
							}
						}
					}
				}

				if( thisComponent != null )
				{
					var item = new ContentBrowserItem_Component( this, null, thisComponent );
					item.SpecialTextPrefix = "this: ";
					item.ReferenceSelectionMode = ContentBrowserItem_Component.ReferenceSelectionModeEnum.This;
					roots.Add( item );
				}
			}

			/////////////////////////////////////
			//root
			if( Mode == ModeEnum.SetReference && setReferenceModeData.selectedComponents != null )
			{
				Component rootComponent = null;
				{
					foreach( var obj in setReferenceModeData.selectedComponents )
					{
						var c = obj as Component;
						if( c != null )
						{
							if( rootComponent == null )
								rootComponent = c.ParentRoot;
							else
							{
								if( rootComponent != c.ParentRoot )
								{
									rootComponent = null;
									break;
								}
							}
						}
					}
				}

				if( rootComponent != null )
				{
					var item = new ContentBrowserItem_Component( this, null, rootComponent );
					item.SpecialTextPrefix = "root: ";
					item.ReferenceSelectionMode = ContentBrowserItem_Component.ReferenceSelectionModeEnum.Root;
					roots.Add( item );
				}
			}

			///////////////////////////////////////
			////General
			//if( Mode == ModeEnum.SetReference && SetReferenceModeData.newObjectWindowFileCreation && ( FilteringMode == null || FilteringMode.AddGroupGeneral ) )
			////if( ( Mode == ModeEnum.Resources && FilteringMode == null ) || ( Mode == ModeEnum.SetReference &&
			////	MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ).IsAssignableFrom( SetReferenceModeData.DemandedType ) &&
			////	SetReferenceModeData.newObjectWindowFileCreation ) )
			//{
			//	var generalItem = new ContentBrowserItem_Virtual( this, null, "General" );
			//	//!!!!
			//	generalItem.imageKey = "Folder";
			//	generalItem.expandAtStartup = true;
			//	roots.Add( generalItem );

			//	if( EditorGeneralResources.groups.Count != 1 )
			//		Log.Fatal( "ContentBrowser: UpdateData: EditorGeneralResources.groups.Count != 1. No implementation." );

			//	var group = EditorGeneralResources.groups[ 0 ];

			//	foreach( var sourceItem in group.types )
			//	{
			//		//if( sourceItem.type != null )
			//		//{
			//		var type = MetadataManager.GetTypeOfNetType( sourceItem.type );
			//		var item = new ContentBrowserItem_Type( this, generalItem, type, sourceItem.name );
			//		generalItem.children.Add( item );
			//		item.imageKey = GetTypeImageKey( type );
			//		item.memberCreationDisable = true;
			//		item.showDisabled = sourceItem.disabled;
			//		//}
			//		//else
			//		//{
			//		//	var item = new ContentBrowserItem_Virtual( this, generalItem, sourceItem.name );
			//		//	generalItem.children.Add( item );
			//		//	//item.imageKey = ;
			//		//	item.showDisabled = sourceItem.disabled;
			//		//}
			//	}
			//}

			/////////////////////////////////////
			//Base, Addons
			if( ( Mode == ModeEnum.Resources && ( FilteringMode == null || FilteringMode.AddGroupsBaseTypesAddonsProject ) ) || ( Mode == ModeEnum.SetReference && MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ).IsAssignableFrom( SetReferenceModeData.DemandedType ) && !setReferenceModeData.selectTypeWindow ) )
			{
				ResourcesWindowItems.PrepareItems();

				Dictionary<string, Item> browserItems = new Dictionary<string, ContentBrowser.Item>();

				Item GetBrowserItemByPath( string path )
				{
					browserItems.TryGetValue( path, out var item );
					return item;
				}

				//var newRoots = new List<Item>();

				foreach( var item in ResourcesWindowItems.Items )
				{
					//skip
					bool skip = false;
					if( Mode == ModeEnum.SetReference )
					{
						if( setReferenceModeData.newObjectWindow && !setReferenceModeData.newObjectWindowFileCreation )
						{
							if( !typeof( Component ).IsAssignableFrom( item.Type ) )
								skip = true;
						}
					}
					if( skip )
						continue;

					var strings = item.Path.Split( new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries );

					string path = "";
					for( int n = 0; n < strings.Length; n++ )
					{
						path = Path.Combine( path, strings[ n ] );

						//get parent item
						ContentBrowserItem_Virtual parentItem = null;
						if( n != 0 )
							parentItem = GetBrowserItemByPath( Path.GetDirectoryName( path ) ) as ContentBrowserItem_Virtual;

						if( GetBrowserItemByPath( path ) == null )
						{
							//add item

							Item browserItem = null;

							//is folder
							bool isFolder = n < strings.Length - 1;
							if( isFolder )
							{
								var text = EditorLocalization.Translate( "ContentBrowser.Group", strings[ n ] );
								var browserItem2 = new ContentBrowserItem_Virtual( this, parentItem, text );

								//var browserItem2 = new ContentBrowserItem_Virtual( this, parentItem, strings[ n ] );

								browserItem = browserItem2;
								browserItem.imageKey = "Folder";

								ResourcesWindowItems.GroupDescriptions.TryGetValue( path, out var description );
								if( !string.IsNullOrEmpty( description ) )
									browserItem2.Description = description;
							}
							else
							{
								var type = MetadataManager.GetTypeOfNetType( item.Type );
								var browserItem2 = new ContentBrowserItem_Type( this, parentItem, type, strings[ n ] );
								browserItem = browserItem2;
								browserItem2.imageKey = GetTypeImageKey( type );
								browserItem2.memberCreationDisable = true;
								browserItem2.ShowDisabled = item.Disabled;
							}

							if( parentItem != null )
								parentItem.children.Add( browserItem );

							browserItems.Add( path, browserItem );
							if( n == 0 )
							{
								roots.Add( browserItem );
								//newRoots.Add( browserItem );
							}
						}
					}
				}

				//expand Base at startup
				{
					var baseItem = GetBrowserItemByPath( "Base" );
					if( baseItem != null )
						baseItem.expandAtStartup = true;
				}

				//foreach( var group in ResourcesWindowItems.Groups )
				//{
				//	var topItem = new ContentBrowserItem_Virtual( this, null, group.DisplayName );
				//	topItem.imageKey = "Folder";//"AssemblyList"
				//	if( group.Type == ResourcesWindowItems.GroupTypeEnum.BaseTypes )
				//		topItem.expandAtStartup = true;
				//	roots.Add( topItem );

				//	Dictionary<string, ContentBrowserItem_Virtual> subgroupsItems = new Dictionary<string, ContentBrowserItem_Virtual>();

				//	foreach( var typeItem in group.Types )
				//	{
				//		var parentItem = topItem;
				//		if( !string.IsNullOrEmpty( typeItem.subgroup ) )
				//		{
				//			var names = typeItem.subgroup.Split( new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries );
				//			string currentName = "";
				//			foreach( var name in names )
				//			{
				//				if( currentName != "" )
				//					currentName += "\\";
				//				currentName += name;

				//				if( !subgroupsItems.TryGetValue( currentName, out var childItem ) )
				//				{
				//					childItem = new ContentBrowserItem_Virtual( this, parentItem, name );
				//					childItem.imageKey = "Folder";
				//					parentItem.children.Add( childItem );

				//					subgroupsItems[ currentName ] = childItem;
				//				}

				//				parentItem = childItem;
				//			}
				//		}

				//		var type = MetadataManager.GetTypeOfNetType( typeItem.type );
				//		var item = new ContentBrowserItem_Type( this, parentItem, type, typeItem.name );
				//		parentItem.children.Add( item );
				//		item.imageKey = GetTypeImageKey( type );
				//		item.memberCreationDisable = true;
				//		item.showDisabled = typeItem.disabled;
				//	}
				//}


			}

			/////////////////////////////////////
			//Favorites
			if( ( Mode == ModeEnum.Resources && ( FilteringMode == null || FilteringMode.AddGroupsBaseTypesAddonsProject ) ) || ( Mode == ModeEnum.SetReference && MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ).IsAssignableFrom( SetReferenceModeData.DemandedType ) && !setReferenceModeData.selectTypeWindow ) )
			{
				favoritesItem = new ContentBrowserItem_Virtual( this, null, EditorLocalization.Translate( "ContentBrowser.Group", "Favorites" ) );
				favoritesItem.imageKey = "Folder";

				var types = new List<Metadata.TypeInfo>();
				foreach( var typeName in EditorFavorites.Favorites.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries ) )
				{
					var type = MetadataManager.GetType( typeName );
					if( type != null )
						types.Add( type );
				}

				foreach( var type in types )
				{
					//skip
					bool skip = false;
					if( Mode == ModeEnum.SetReference )
					{
						if( setReferenceModeData.newObjectWindow && !setReferenceModeData.newObjectWindowFileCreation )
						{
							if( !typeof( Component ).IsAssignableFrom( type.GetNetType() ) )
								skip = true;
						}
					}
					if( skip )
						continue;

					var typeItem = new ContentBrowserItem_Type( this, favoritesItem, type, type.DisplayName );
					typeItem.imageKey = GetTypeImageKey( type );
					typeItem.memberCreationDisable = true;
					favoritesItem.children.Add( typeItem );
				}

				if( Mode == ModeEnum.SetReference )
					SetReference_RemoveExcessTypeItems( favoritesItem );

				//remove excess classes in Resources and filtering mode
				if( Mode == ModeEnum.Resources && FilteringMode != null )
					Resources_RemoveExcessClassesItemsByFilteringMode( favoritesItem );

				if( favoritesItem != null )
					roots.Add( favoritesItem );
			}

			///////////////////////////////////////
			////Base Types
			//if( ( Mode == ModeEnum.Resources && FilteringMode == null ) || ( Mode == ModeEnum.SetReference &&
			//	MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ).IsAssignableFrom( SetReferenceModeData.DemandedType ) &&
			//	!setReferenceModeData.selectTypeWindow ) )
			//{
			//	var topItem = new ContentBrowserItem_Virtual( this, null, "Base types" );
			//	topItem.imageKey = "Folder";
			//	//baseTypesItem.imageKey = "AssemblyList";
			//	topItem.expandAtStartup = true;
			//	roots.Add( topItem );

			//	foreach( var group in EditorBaseTypes.groups )
			//	{
			//		var groupItem = new ContentBrowserItem_Virtual( this, topItem, group.name );
			//		groupItem.imageKey = "Folder";
			//		topItem.children.Add( groupItem );

			//		foreach( var typeItem in group.types )
			//		{
			//			var type = MetadataManager.GetTypeOfNetType( typeItem.type );
			//			var item = new ContentBrowserItem_Type( this, groupItem, type, typeItem.name );
			//			groupItem.children.Add( item );
			//			item.imageKey = GetTypeImageKey( type );
			//			item.memberCreationDisable = true;
			//			item.showDisabled = typeItem.disabled;
			//		}
			//	}
			//}

			///////////////////////////////////////
			////Add-ons
			//if( Mode == ModeEnum.Resources /*&&
			//	( resourcesModeData.selectionMode == ResourceSelectionMode.None ||
			//	resourcesModeData.selectionMode == ResourceSelectionMode.Type ||
			//	resourcesModeData.selectionMode == ResourceSelectionMode.Member )*/ ||
			//	Mode == ModeEnum.SetReference )
			//{
			//	ContentBrowserItem_Virtual topItem = null;

			//	topItem = new ContentBrowserItem_Virtual( this, null, "Add-ons" );
			//	topItem.imageKey = "Folder";
			//	//addonsItem.expandAtStartup = true;

			//	//!!!!
			//	//foreach( var group in EditorBaseTypes.groups )
			//	//{
			//	//	var groupItem = new ContentBrowserItem_Virtual( this, addonsItem, group.name );
			//	//	//!!!!
			//	//	groupItem.imageKey = "Folder";
			//	//	addonsItem.children.Add( groupItem );

			//	//	foreach( var typeItem in group.types )
			//	//	{
			//	//		var type = MetadataManager.GetTypeOfNetType( typeItem.type );
			//	//		var item = new ContentBrowserItem_Type( this, groupItem, type, typeItem.name );
			//	//		groupItem.children.Add( item );
			//	//		item.imageKey = GetTypeImageKey( type );
			//	//		item.memberCreationDisable = true;
			//	//		item.showDisabled = typeItem.disabled;
			//	//	}
			//	//}

			//	if( topItem != null )
			//		roots.Add( topItem );
			//}

			///////////////////////////////////////
			////Project
			//if( Mode == ModeEnum.Resources /*&&
			//	( resourcesModeData.selectionMode == ResourceSelectionMode.None ||
			//	resourcesModeData.selectionMode == ResourceSelectionMode.Type ||
			//	resourcesModeData.selectionMode == ResourceSelectionMode.Member )*/ ||
			//	Mode == ModeEnum.SetReference )
			//{
			//	ContentBrowserItem_Virtual topItem = null;

			//	topItem = new ContentBrowserItem_Virtual( this, null, "Project" );
			//	topItem.imageKey = "Folder";
			//	//addonsItem.expandAtStartup = true;

			//	//!!!!
			//	//foreach( var group in EditorBaseTypes.groups )
			//	//{
			//	//	var groupItem = new ContentBrowserItem_Virtual( this, addonsItem, group.name );
			//	//	//!!!!
			//	//	groupItem.imageKey = "Folder";
			//	//	addonsItem.children.Add( groupItem );

			//	//	foreach( var typeItem in group.types )
			//	//	{
			//	//		var type = MetadataManager.GetTypeOfNetType( typeItem.type );
			//	//		var item = new ContentBrowserItem_Type( this, groupItem, type, typeItem.name );
			//	//		groupItem.children.Add( item );
			//	//		item.imageKey = GetTypeImageKey( type );
			//	//		item.memberCreationDisable = true;
			//	//		item.showDisabled = typeItem.disabled;
			//	//	}
			//	//}

			//	if( topItem != null )
			//		roots.Add( topItem );
			//}

			/////////////////////////////////////
			//All types
			if( ( Mode == ModeEnum.Resources || Mode == ModeEnum.SetReference ) && ( FilteringMode == null || FilteringMode.AddGroupAllTypes ) )
			{
				ContentBrowserItem_Virtual classesItem = null;

				//!!!!"Classes"? так-то не только классы. "Types", "Native types", "Built-in types"
				classesItem = new ContentBrowserItem_Virtual( this, null, EditorLocalization.Translate( "ContentBrowser.Group", "All types" ) );
				//classesItem = new ContentBrowserItem_Virtual( this, null, "Classes" );
				classesItem.imageKey = "Folder";
				//classesItem.imageKey = "AssemblyList";

				//!!!!slowly. сразу раскладывать по неймспейсам можно
				//!!!!!!можно еще не проверять какие-то сборки, если нужны только Component (от которых зависит NeoAxis.Core.dll)
				List<Metadata.NetTypeInfo> types = new List<Metadata.NetTypeInfo>( 16384 );
				ESet<string> namespacesSet = new ESet<string>();

				foreach( var type in MetadataManager.NetTypes )
				{
					var netType = type.Type;

					if( !netType.IsNested && !ResourcesAndSetReference_SkipNetTypeCompletely( netType ) )
					{
						bool skip = false;
						if( Mode == ModeEnum.SetReference && netType.IsEnum )
							skip = true;

						if( !skip && Mode == ModeEnum.SetReference && setReferenceModeData.newObjectWindow )
						{
							if( !ContentBrowserUtility.ContainsComponentClasses( netType ) )
								skip = true;
							//if( !ReferenceUtils.TypeOrItsMembersCanReferencedToComponentType( netType ) )
							//	skip = true;
						}
						if( !skip && Mode == ModeEnum.SetReference && setReferenceModeData.selectTypeWindow )
						{
							if( !ContentBrowserUtility.ContainsType( setReferenceModeData.selectTypeDemandedType.GetNetType(), netType ) )
								skip = true;
							//if( !ContentBrowserUtils.ContainsType( setReferenceModeData.demandedType.GetNetType(), netType ) )
							//	skip = true;
						}

						if( !skip )
						{
							types.Add( type );

							//!!!! type can be without namespace
							if( !string.IsNullOrEmpty( type.Namespace ) )
								namespacesSet.AddWithCheckAlreadyContained( type.Namespace );
						}
					}
				}

				//sort namespaces
				List<string> namespacesList = new List<string>( namespacesSet );
				CollectionUtility.MergeSort( namespacesList, delegate ( string v1, string v2 )
				{
					return string.Compare( v1, v2 );
				} );

				//sort types
				CollectionUtility.MergeSort( types, delegate ( Metadata.NetTypeInfo v1, Metadata.NetTypeInfo v2 )
				{
					return string.Compare( v1.ToString(), v2.ToString() );
				} );

				foreach( string ns in namespacesList )
				{
					//add namespace
					var namespaceItem = new ContentBrowserItem_Virtual( this, classesItem, ns );
					namespaceItem.imageKey = "Namespace";
					classesItem.children.Add( namespaceItem );

					foreach( var type in types )
					{
						var netType = type.Type;

						if( type.Namespace == ns )//if( netType.Namespace == ns )
						{
							//add item
							var name = MetadataManager.GetNetTypeName( netType, true, false );
							var item = new ContentBrowserItem_Type( this, namespaceItem, type, name );
							item.imageKey = GetTypeImageKey( type );
							namespaceItem.children.Add( item );

							//add nested types
							AddNestedTypesRecursive( item );
						}
					}
				}

				//!!!!

				//!!!!
				//Most usable classes
				//Categorized/By category
				//Components
				//группирование по коллекции, простые типы или как-то так
				//группирование по общему. например все классы террейна в группе террейна
				//.NET
				//Assemblies
				//Namespaces

				//All

				//if( Mode == ModeEnum.Resources && resourcesModeData.selectionMode == ResourceSelectionMode.Type )
				//	Resources_SelectType_RemoveExcessTypeItems( classesItem );
				if( Mode == ModeEnum.SetReference )
					SetReference_RemoveExcessTypeItems( classesItem );

				//remove excess classes in Resources and filtering mode
				if( Mode == ModeEnum.Resources && FilteringMode != null )
				{
					Resources_RemoveExcessClassesItemsByFilteringMode( classesItem );

					//!!!!может не так
					//expand at startup class items for filtering mode
					var children = classesItem.GetChildren( true );
					if( children.Count == 1 )
					{
						classesItem.expandAtStartup = true;
						if( children[ 0 ].GetChildren( true ).Count <= 10 )
							children[ 0 ].expandAtStartup = true;
					}
				}

				//expand for select type mode
				if( Mode == ModeEnum.SetReference && setReferenceModeData.selectTypeWindow )
				{
					var children = classesItem.GetChildren( true );
					if( children.Count < 20 )
					{
						classesItem.expandAtStartup = true;
						if( children.Count == 1 )
						{
							if( children[ 0 ].GetChildren( true ).Count <= 10 )
								children[ 0 ].expandAtStartup = true;
						}
					}
				}

				if( classesItem != null )
					roots.Add( classesItem );
			}

			///////////////////////////////////////
			////Solution
			////!!!!hg
			//if( Mode == ModeEnum.Resources && ( FilteringMode == null || FilteringMode.AddSolution ) )
			//{
			//	ContentBrowserItem_Solution solutionItem = null;

			//	solutionItem = new ContentBrowserItem_Solution( this, null );
			//	solutionItem.imageKey = "Folder";

			//	if( solutionItem != null )
			//		roots.Add( solutionItem );
			//}

			/////////////////////////////////////
			//files
			if( FilteringMode == null || FilteringMode.AddFiles )
			{
				dataItem = new ContentBrowserItem_File( this, null, VirtualFileSystem.Directories.Assets, true );//, true );
				dataItem.SetText( EditorLocalization.Translate( "ContentBrowser.Group", "Assets" ) );
				dataItem.imageKey = "Folder";
				dataItem.expandAtStartup = true;
				if( Mode == ModeEnum.SetReference && setReferenceModeData.selectTypeWindow )
					dataItem.expandAtStartup = false;

				if( FilteringMode != null && FilteringMode.ExpandAllFileItemsAtStartup )
					dataItem.expandAllAtStartup = true;
			}
			if( dataItem != null )
				roots.Add( dataItem );

			/////////////////////////////////////
			//Null
			if( /*Mode == ModeEnum.Resources && resourcesModeData.allowNull ||*/ Mode == ModeEnum.SetReference && SetReferenceModeData.allowNull )
			//if( ( Mode == ModeEnum.Resources && resourcesModeData.allowNull &&
			//	( resourcesModeData.selectionMode == ResourceSelectionMode.File ||
			//	resourcesModeData.selectionMode == ResourceSelectionMode.Type ||
			//	resourcesModeData.selectionMode == ResourceSelectionMode.Member ) ) ||
			//	Mode == ModeEnum.SetReference )
			{
				var item = new ContentBrowserItem_Null( this, null, "Null" );
				if( item != null )
					roots.Add( item );
			}

			//!!!!!что еще. как свои добавлять

			SetData( roots );
		}

		string Translate( string text )
		{
			return EditorLocalization.Translate( "ContentBrowser", text );
		}

		public bool CanNewObject( out Item newParentItem )
		{
			if( ReadOnlyHierarchy )
			{
				newParentItem = null;
				return false;
			}

			Item parentItem = null;
			if( SelectedItems.Length == 1 )
				parentItem = SelectedItems[ 0 ];

			//!!!!когда еще нельзя создавать?

			if( parentItem != null && parentItem.ContainedObject != null )
			{
				//this is Component
				var componentItem = parentItem as ContentBrowserItem_Component;
				if( componentItem != null && componentItem.Component != null && !componentItem.Component.EditorReadOnlyInHierarchy && EditorAPI.GetDocumentByObject( componentItem.Component ) != null )
				{
					newParentItem = parentItem;
					return true;
				}

				////this is List
				//var noSpecItem = parentItem as ContentBrowserItem_NoSpecialization;
				//if( noSpecItem != null && ContentBrowserItem_NoSpecialization.IsList( noSpecItem.SourceObject ) )
				//{
				//	newParentItem = parentItem;
				//	return true;
				//}

				////this is element of the List
				//var parentItem2 = parentItem.Parent as ContentBrowserItem_NoSpecialization;
				//if( parentItem2 != null && ContentBrowserItem_NoSpecialization.IsList( parentItem2.SourceObject ) )
				//{
				//	newParentItem = parentItem2;
				//	return true;
				//}
			}

			newParentItem = null;
			return false;
		}

		//!!!!
		public void TryNewObject( Metadata.TypeInfo lockType )//Item originalParentItem )
		{
			Item parentItem;
			if( !CanNewObject( out parentItem ) )
				return;

			//!!!!что еще так сразу создавать

			////!!!!прям так?
			////List
			//var parentNoSpecItem = parentItem as ContentBrowserItem_NoSpecialization;
			//if( parentNoSpecItem != null && ContentBrowserItem_NoSpecialization.IsList( parentNoSpecItem.SourceObject ) )
			//{
			//	var list = parentNoSpecItem.SourceObject;

			//	var elementNetType = list.GetType().GetGenericArguments()[ 0 ];
			//	if( elementNetType.IsValueType || elementNetType == typeof( string ) )
			//	{
			//		object value = null;

			//		if( elementNetType == typeof( string ) )
			//			value = "";
			//		else
			//			value = MetadataManager.GetTypeOfNetType( elementNetType ).InvokeInstance( null );

			//		int lastCount = (int)list.GetType().GetProperty( "Count" ).GetValue( list );

			//		MethodInfo methodAdd = list.GetType().GetMethod( "Add" );
			//		methodAdd.Invoke( list, new object[] { value } );

			//		if( DocumentWindow != null && DocumentWindow.Document != null )
			//		{
			//			var document = DocumentWindow.Document;

			//			List<int> objectIndexes = new List<int>();
			//			objectIndexes.Add( lastCount );

			//			var action = new UndoActionListAddRemove( list, objectIndexes, true );
			//			document.UndoSystem.CommitAction( action );

			//			document.Modified = true;

			//			//!!!!
			//			//Log.Warning( "impl" );

			//			//!!!!
			//			//select new item
			//			SelectItems( new Item[] { parentItem.GetChildren( false )[ lastCount ] } );
			//		}

			//		return;
			//	}
			//}

			//!!!!one choose
			{
				//!!!!
			}

			//!!!!
			var parent = parentItem.ContainedObject;

			var data = new NewObjectWindow.CreationDataClass();
			data.initDocumentWindow = DocumentWindow;

			data.initParentObjects = new List<object>();
			//!!!!multiselection
			data.initParentObjects.Add( parent );

			data.createdFromContentBrowser = this;

			data.initLockType = lockType;
			data.initSupportAutoCreateAndClose = lockType != null;

			//!!!!
			//if( parent is Component )
			//	initData.initDemandedType = MetadataManager.GetTypeOfNetType( typeof( Component ) );

			EditorAPI.OpenNewObjectWindow( data );

			//!!!!
			//EditorForm.Instance.ShowNewObjectWindow( "", DocumentWindow, parent, demandedType, false );
		}

		//!!!!

		//void FileCreatedEvent( string realPath )
		//{
		//	//string path = VirtualFileSystem.GetVirtualPathByReal( realPath );

		//	//if( Directory.Exists( realPath ) )
		//	//{
		//	//	string[] strings = path.Split( new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries );
		//	//	string path2 = "";
		//	//	foreach( string s in strings )
		//	//	{
		//	//		path2 = Path.Combine( path2, s );
		//	//		UpdateAddDirectory( path2 );
		//	//	}
		//	//}
		//	//else
		//	//{
		//	//	string[] strings = Path.GetDirectoryName( path ).Split( new char[] { '/', '\\' },
		//	//		StringSplitOptions.RemoveEmptyEntries );
		//	//	string path2 = "";
		//	//	foreach( string s in strings )
		//	//	{
		//	//		path2 = Path.Combine( path2, s );
		//	//		UpdateAddDirectory( path2 );
		//	//	}

		//	//	UpdateAddResource( path );

		//	//	//load resource
		//	//	string extension = Path.GetExtension( path );
		//	//	if( !string.IsNullOrEmpty( extension ) )
		//	//	{
		//	//		extension = extension.Substring( 1 );
		//	//		ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
		//	//		if( resourceType != null )
		//	//		{
		//	//			bool needRestoreWatch = false;
		//	//			try
		//	//			{
		//	//				if( WatchFileSystem )
		//	//				{
		//	//					WatchFileSystem = false;
		//	//					needRestoreWatch = true;
		//	//				}

		//	//				if( resourceType.DoOutsideAddResource( path ) )
		//	//					resourceType.DoLoadResource( path );
		//	//			}
		//	//			finally
		//	//			{
		//	//				if( needRestoreWatch )
		//	//					WatchFileSystem = true;
		//	//			}
		//	//		}
		//	//	}
		//	//}
		//}

		//void FileDeletedEvent( string realPath )
		//{
		//	//!!!!

		//	//string path = VirtualFileSystem.GetVirtualPathByReal( realPath );

		//	//TreeNode node = GetNodeByPath( path );
		//	//if( node != null )
		//	//{
		//	//	if( node.Tag != null )
		//	//	{
		//	//		//unload resource
		//	//		string extension = Path.GetExtension( path );
		//	//		if( !string.IsNullOrEmpty( extension ) )
		//	//		{
		//	//			extension = extension.Substring( 1 );
		//	//			ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
		//	//			if( resourceType != null )
		//	//				resourceType.DoUnloadResource( path );
		//	//		}
		//	//	}

		//	//	node.Remove();
		//	//	OnTreeNodeRemoved( (MyTreeNode)node );
		//	//}
		//}

		//void FileRenamedEvent( string realPath, string oldRealPath )
		//{
		//	//!!!!

		//	//string path = VirtualFileSystem.GetVirtualPathByReal( realPath );
		//	//string oldPath = VirtualFileSystem.GetVirtualPathByReal( oldRealPath );

		//	////update registered resource types
		//	//{
		//	//	string oldExtension = Path.GetExtension( oldPath );
		//	//	string extension = Path.GetExtension( path );

		//	//	if( !string.IsNullOrEmpty( oldExtension ) &&
		//	//		!string.IsNullOrEmpty( extension ) &&
		//	//		string.Compare( oldExtension, extension, true ) == 0 )
		//	//	{
		//	//		//use ResourceType.OnResourceRenamed

		//	//		extension = extension.Substring( 1 );
		//	//		ResourceType resourceType = ResourceTypeManager.Instance.
		//	//			GetByExtension( extension );

		//	//		if( resourceType != null )
		//	//			resourceType.OnResourceRenamed( path, oldPath );
		//	//	}
		//	//	else
		//	//	{
		//	//		//unload old resource
		//	//		if( !string.IsNullOrEmpty( oldExtension ) )
		//	//		{
		//	//			oldExtension = oldExtension.Substring( 1 );
		//	//			ResourceType resourceType = ResourceTypeManager.Instance.
		//	//				GetByExtension( oldExtension );
		//	//			if( resourceType != null )
		//	//				resourceType.DoUnloadResource( oldPath );
		//	//		}

		//	//		//load new resource
		//	//		if( !string.IsNullOrEmpty( extension ) )
		//	//		{
		//	//			extension = extension.Substring( 1 );
		//	//			ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
		//	//			if( resourceType != null )
		//	//				resourceType.DoLoadResource( path );
		//	//		}
		//	//	}
		//	//}

		//	//TreeNode node = GetNodeByPath( oldPath );
		//	//if( node != null )
		//	//{
		//	//	string newName = Path.GetFileName( realPath );
		//	//	node.Text = newName;
		//	//	node.Name = newName;
		//	//	UpdateNodeIcon( node );
		//	//}

		//	//if( !VirtualFile.Exists( path ) && VirtualDirectory.Exists( path ) )
		//	//	OnDirectoryRenamed( path, oldPath );
		//}

		//void FileChangedEvent( string realPath )
		//{
		//	//!!!!

		//	//string path = VirtualFileSystem.GetVirtualPathByReal( realPath );

		//	//string extension = Path.GetExtension( path );
		//	//if( !string.IsNullOrEmpty( extension ) )
		//	//{
		//	//	extension = extension.Substring( 1 );
		//	//	ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
		//	//	if( resourceType != null )
		//	//	{
		//	//		resourceType.DoUnloadResource( path );
		//	//		resourceType.DoLoadResource( path );
		//	//	}
		//	//}
		//}

		//!!!!
		private void VirtualFileWatcher_Update( FileSystemEventArgs args )
		{
			//!!!!что еще

			switch( args.ChangeType )
			{
			case WatcherChangeTypes.Created:
			case WatcherChangeTypes.Deleted:
				{
					var directoryName = Path.GetDirectoryName( args.FullPath );
					string virtualPath = VirtualPathUtility.GetVirtualPathByReal( directoryName );
					CallPerformChildrenChanged( virtualPath );
				}
				break;

			case WatcherChangeTypes.Renamed:
				{
					RenamedEventArgs args2 = (RenamedEventArgs)args;

					//!!!!!так-ли

					var directoryNameOld = Path.GetDirectoryName( args2.OldFullPath );
					string virtualPathOld = VirtualPathUtility.GetVirtualPathByReal( directoryNameOld );
					CallPerformChildrenChanged( virtualPathOld );

					var directoryName = Path.GetDirectoryName( args.FullPath );
					string virtualPath = VirtualPathUtility.GetVirtualPathByReal( directoryName );
					if( string.Compare( virtualPathOld, virtualPath, true ) != 0 )
						CallPerformChildrenChanged( virtualPath );
				}
				break;

			case WatcherChangeTypes.Changed:
				{
					//!!!!!

				}
				break;
			}
		}

		//!!!!!
		//public void PerformTextChanged()

		//!!!!
		void CallPerformChildrenChanged( string virtualPath )
		{
			if( dataItem != null )
			{
				var names = virtualPath.Split( new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries );

				string current = "";
				ContentBrowserItem_File currentItem = dataItem;

				for( int n = 0; n < names.Length; n++ )
				{
					current = Path.Combine( current, names[ n ] );

					string realPath = VirtualPathUtility.GetRealPathByVirtual( current );
					string key = ContentBrowserItem_File.GetFileChildrenKey( realPath );

					if( !currentItem.FileChildren.TryGetValue( key, out Item item ) )
					{
						//!!!!
						return;
					}

					currentItem = (ContentBrowserItem_File)item;
				}

				if( currentItem == null )
				{
					//!!!!
					return;
				}

				//update children. need update parent items for C# filtering mode (when directories without items are hided)
				{
					Item item = currentItem;
					while( item != null )
					{
						item.PerformChildrenChanged();
						item = item.Parent;
					}
				}
				//currentItem.PerformChildrenChanged();
			}
		}

		//!!!!
		private void ResourceInstance_AllInstances_StatusChanged( Resource.Instance ins )
		{
			if( EngineApp.Closing )
				return;
			if( ins.InstanceType != Resource.InstanceType.Resource )
				return;

			//!!!!ошибочные как-то показывать можно (красным, например)

			//!!!!

			switch( ins.Status )
			{
			case Resource.Instance.StatusEnum.CreationProcess:
				break;

			case Resource.Instance.StatusEnum.Error:
				break;

			case Resource.Instance.StatusEnum.Ready:
				{
					//!!!!!
					if( ins.Owner.LoadFromFile )
					{
						var virtualPath = ins.Owner.Name;
						CallPerformChildrenChanged( virtualPath );
					}
				}
				break;
			}
		}

		//!!!!
		private void ResourceInstance_AllInstances_DisposedEvent( Resource.Instance ins )
		{
			if( EngineApp.Closing )
				return;
			if( ins.InstanceType != Resource.InstanceType.Resource )
				return;

			//!!!!!
			if( ins.Owner.LoadFromFile )
			{
				var virtualPath = ins.Owner.Name;
				CallPerformChildrenChanged( virtualPath );
			}
		}

		//!!!!new
		void PreloadNode( Item item )
		{
			//!!!!copy code from ContentBrowser_ItemAfterSelect


			//!!!!
			bool canLoadResources = true;

			//!!!!
			//select object settings
			if( item != null && canLoadResources )
			{
				//!!!!
				//selectedNode.SetMultiSelected( true, false );

				ContentBrowserItem_File fileItem = item as ContentBrowserItem_File;
				if( fileItem != null && !fileItem.IsDirectory )
				{
					//!!!!!

					//!!!!
					//Resource type: Scene definition. а не сама загрузка. дефинишен будет в одном экземпляре всегда

					//!!!!не делать предпросмотр для карты, т.к. долго. что еще?
					var ext = Path.GetExtension( fileItem.FullPath );
					if( ResourceManager.GetTypeByFileExtension( ext ) != null )
					{
						string virtualName = VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath );
						if( string.IsNullOrEmpty( virtualName ) )
						{
							//!!!!
						}

						//как separate открывать?
						//!!!!!!Wait
						//!!!!!.Separate
						var ins = ResourceManager.LoadResource( virtualName, true );
					}
				}
			}
		}

		//private void ContentBrowser_ItemBeforeSelect( ItemTreeNode node, ref bool handled )
		//{
		//	//!!!!copy code from ContentBrowser_ItemAfterSelect


		//	//!!!!
		//	bool canLoadResources = true;

		//	//!!!!
		//	//select object settings
		//	if( node != null && canLoadResources )
		//	{
		//		//!!!!
		//		//selectedNode.SetMultiSelected( true, false );

		//		ContentBrowserItem_File fileItem = node.item as ContentBrowserItem_File;
		//		if( fileItem != null && !fileItem.IsDirectory )
		//		{
		//			//!!!!!

		//			//!!!!
		//			//Resource type: Scene definition. а не сама загрузка. дефинишен будет в одном экземпляре всегда

		//			//!!!!не делать предпросмотр для карты, т.к. долго. что еще?
		//			var ext = Path.GetExtension( fileItem.FullPath );
		//			if( ResourceManager.GetTypeByFileExtension( ext ) != null )
		//			{
		//				string virtualName = VirtualPathUtils.GetVirtualPathByReal( fileItem.FullPath );
		//				if( string.IsNullOrEmpty( virtualName ) )
		//				{
		//					//!!!!
		//				}

		//				//как separate открывать?
		//				//!!!!!!Wait
		//				//!!!!!.Separate
		//				var ins = ResourceManager.LoadResource( virtualName, true );
		//			}
		//		}
		//	}
		//}

		void SettingsWindowSelectObjects()
		{
			var objects = new List<object>();

			//!!!!
			//selectedNode.SetMultiSelected( true, false );

			foreach( var item in SelectedItems )
			{
				//bool selected = false;

				//_File
				var fileItem = item as ContentBrowserItem_File;
				if( fileItem != null && !fileItem.IsDirectory )
				{
					//!!!!!

					//!!!!надо ли все выделенные грузить. Это про мультивыделение.

					//!!!!
					//Resource type: Scene definition. а не сама загрузка. дефинишен будет в одном экземпляре всегда

					//!!!!не делать предпросмотр для карты, т.к. долго. что еще?
					var ext = Path.GetExtension( fileItem.FullPath );
					if( ResourceManager.GetTypeByFileExtension( ext ) != null )
					{
						string virtualName = VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath );
						if( string.IsNullOrEmpty( virtualName ) )
						{
							//!!!!
						}

						//!!!!wait
						var ins = ResourceManager.LoadResource( virtualName, true );

						if( ins != null && ins.ResultObject != null )
						{
							var component = ins.ResultComponent;
							if( component != null )
								objects.Add( component );
						}
					}
				}

				//_Component
				var componentItem = item as ContentBrowserItem_Component;
				if( componentItem != null && componentItem.Component != null )
					objects.Add( componentItem.Component );

				////!!!!
				//if( documentWindow == null )
				//{
				//	//_Type
				//	var typeItem = item as ContentBrowserItem_Type;
				//	if( typeItem != null && typeItem.type != null )
				//		objects.Add( typeItem.type );

				//	//_Member
				//	var memberItem = item as ContentBrowserItem_Member;
				//	if( memberItem != null && memberItem.Member != null )
				//		objects.Add( memberItem.Member );
				//}
			}

			////!!!!new
			//if( objects.Count == 0 && documentWindow.ObjectOfWindow != null )
			//	objects.Add( documentWindow.ObjectOfWindow );

			SettingsWindow.Instance?.SelectObjects( documentWindow, objects );
		}

		private void ContentBrowser_ItemAfterSelect( ContentBrowser sender, IList<Item> items, bool selectedByUser, ref bool handled )
		{
			if( EditorAPI.ClosingApplication )
				return;
			if( DocumentWindow?.Document != null && DocumentWindow.Document.Destroyed )
				return;

			//!!!!same as ContentBrowser_ItemBeforeSelect

			//!!!!
			//bool canLoadResources = true;

			//!!!!
			//select object settings
			//if( node != null )//&& canLoadResources )
			//{
			//!!!!
			//selectedNode.SetMultiSelected( true, false );

			//bool selected = false;

			var unableToLoad = false;

			if( PreloadResourceOnSelection )
			{
				foreach( var item in items )// if( item != null )//&& canLoadResources )
				{
					ContentBrowserItem_File fileItem = item as ContentBrowserItem_File;
					if( fileItem != null && !fileItem.IsDirectory )
					{
						//!!!!!

						//!!!!
						//Resource type: Scene definition. а не сама загрузка. дефинишен будет в одном экземпляре всегда

						//!!!!не делать предпросмотр для карты, т.к. долго. что еще?
						var ext = Path.GetExtension( fileItem.FullPath );
						if( ResourceManager.GetTypeByFileExtension( ext ) != null )
						{
							string virtualName = VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath );
							if( string.IsNullOrEmpty( virtualName ) )
							{
								//!!!!
							}

							//как separate открывать?
							//!!!!!!Wait
							//!!!!!.Separate
							var ins = ResourceManager.LoadResource( virtualName, true );
							if( ins == null )
								unableToLoad = true;

							//if( CanSelectObjectSettings && selectedByUser && ins != null && ins.ResultObject != null )
							//{
							//	//!!!!!видать, потом по другому будет. получать что/где сейчас выделено?

							//	var component = ins.ResultComponent;
							//	if( component != null )
							//	{
							//		SettingsWindowSelectObjects( new object[] { component } );
							//		//EditorForm.Instance.SelectObjectSettings( DocumentWindow, new object[] { component } );
							//		selected = true;
							//	}
							//}
						}
					}
					else
					{
						//if( CanSelectObjectSettings && selectedByUser )
						//{
						//	//same as in ObjectsWindow
						//	ContentBrowserItem_Component componentItem = node.item as ContentBrowserItem_Component;
						//	if( componentItem != null && componentItem.Component != null )
						//	{
						//		SettingsWindowSelectObjects( new object[] { componentItem.Component } );
						//		//EditorForm.Instance.SelectObjectSettings( DocumentWindow, new object[] { componentItem.Component } );
						//		selected = true;
						//	}
						//}
					}
				}
			}

			//if( CanSelectObjectSettings && selectedByUser )
			//{
			//	if( !selected )
			//	{
			//		SettingsWindowSelectObjects( new object[ 0 ] );
			//		//EditorForm.Instance.SelectObjectSettings( null, null );
			//	}
			//}

			if( CanSelectObjectSettings && selectedByUser && !unableToLoad )
				SettingsWindowSelectObjects();
			//}
		}

		//!!!!
		void OpenFile( ContentBrowserItem_File fileItem, ref bool handled )
		{
			//!!!!!

			//!!!!можно открыть просто как текст

			//!!!!не открывать одно и тоже несколько раз. хотя, по сути сцене можно же

			//!!!!
			//Resource type: Scene definition. а не сама загрузка. дефинишен будет в одном экземпляре всегда

			if( EditorAPI.IsDocumentFileSupport( fileItem.FullPath ) )
			{
				//!!!!!temp. тут меньше проверок, чем в AfterSelect

				//!!!!can use already opened

				EditorAPI.OpenFileAsDocument( fileItem.FullPath, true, true );
				//string virtualName = VirtualPathUtils.GetVirtualPathByReal( fileItem.path );
				//if( string.IsNullOrEmpty( virtualName ) )
				//{
				//	//!!!!
				//}
				//EditorForm.Instance.OpenResourceAsDocument( virtualName, true, true );

				handled = true;
			}
			else
			{
				//!!!!
			}
		}

		void PerformItemAfterChoose( Item item, ref bool handled )
		{
			ItemAfterChoose?.Invoke( this, item, ref handled );
			if( handled )
				return;

			//default behaviour

			//}

			//private void ContentBrowser_ItemAfterChoose( ContentBrowser sender, Item item, ref bool handled )
			//{
			//!!!!

			//!!!!
			//selectedNode.SetMultiSelected( true, false );

			//!!!!не всем нужно
			//!!!!контекстное меню еще
			if( Mode == ModeEnum.Resources )//!!!!
			{
				ContentBrowserItem_File fileItem = item as ContentBrowserItem_File;
				if( fileItem != null && !fileItem.IsDirectory )
				{
					OpenFile( fileItem, ref handled );
				}
			}

			//!!!!!

			//Item item;
			//if( itemByNode.TryGetValue( node, out item ) )
			//{
			//	var fileItem = item as ContentBrowser_FileItem;
			//	if( fileItem != null )
			//	{
			//		EditorRoot.OpenResourceAsDocument( fileItem.path, true );

			//		//!!!!!!тут? так?
			//		return true;
			//	}
			//}

		}

		void Resort()
		{
			//!!!!
		}

		void AddSortByToContextMenu( List<KryptonContextMenuItemBase> items )
		{
			KryptonContextMenuItem item;

			KryptonContextMenuItem itemSortBy = new KryptonContextMenuItem( Translate( "Sort by" ), null );
			List<KryptonContextMenuItemBase> items2 = new List<KryptonContextMenuItemBase>();

			//Name
			item = new KryptonContextMenuItem( Translate( "Name" ), null, delegate ( object sender, EventArgs e )
			{
				if( options.SortFilesBy != SortByItems.Name )
				{
					options.SortFilesBy = SortByItems.Name;
					Resort();
				}
			} );
			item.Checked = options.SortFilesBy == SortByItems.Name;
			items2.Add( item );

			//Date
			item = new KryptonContextMenuItem( Translate( "Date" ), null, delegate ( object sender, EventArgs e )
			{
				if( options.SortFilesBy != SortByItems.Date )
				{
					options.SortFilesBy = SortByItems.Date;
					Resort();
				}
			} );
			item.Checked = options.SortFilesBy == SortByItems.Date;
			items2.Add( item );

			//Type
			item = new KryptonContextMenuItem( Translate( "Type" ), null, delegate ( object sender, EventArgs e )
			{
				if( options.SortFilesBy != SortByItems.Type )
				{
					options.SortFilesBy = SortByItems.Type;
					Resort();
				}
			} );
			item.Checked = options.SortFilesBy == SortByItems.Type;
			items2.Add( item );

			//Size
			item = new KryptonContextMenuItem( Translate( "Size" ), null, delegate ( object sender, EventArgs e )
			{
				if( options.SortFilesBy != SortByItems.Size )
				{
					options.SortFilesBy = SortByItems.Size;
					Resort();
				}
			} );
			item.Checked = options.SortFilesBy == SortByItems.Size;
			items2.Add( item );

			//separator
			items2.Add( new KryptonContextMenuSeparator() );

			//Ascending
			item = new KryptonContextMenuItem( Translate( "Ascending" ), null, delegate ( object sender, EventArgs e )
			{
				options.SortFilesByAscending = !options.SortFilesByAscending;
				Resort();
			} );
			item.Checked = options.SortFilesByAscending;
			items2.Add( item );


			itemSortBy.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
			items.Add( itemSortBy );
		}

		void ShowContextMenu( Item contentItem, Control locationControl, Point locationPoint )
		{
			//!!!!совсем нет меню в read only?
			if( ReadOnlyHierarchy )
				return;

			//DocumentInstance document = null;
			//if( DocumentWindow != null )
			//	document = DocumentWindow.Document;

			var items = new List<KryptonContextMenuItemBase>();

			//_Component
			if( contentItem != null )
			{
				var componentItem = contentItem as ContentBrowserItem_Component;
				if( componentItem != null )
				{
					var documentOfComponent = EditorAPI.GetDocumentByObject( componentItem.Component );

					//!!!!!слой, он как папка. ассоциация с ComponentsBrowser и ResourcesBrowser
					//!!!!!!!name: группа?
					//!!!!!!тут же еще Load/Save Selection. запоминать выделенные
					//!!!!!!!!!!а с ResourcesBrowser есть ассоциации?

					//folder specific. //!!!!а на ресурсе почему нельзя?

					//!!!!!!
					//ContentBrowser_FileItem currentOrParentDirectoryItem = null;
					//{
					//	var i = contentItem as ContentBrowser_FileItem;
					//	if( i != null )
					//	{
					//		if( !i.isDirectory )
					//		{
					//			ContentBrowser_FileItem parent = i.Parent as ContentBrowser_FileItem;
					//			if( parent != null && parent.isDirectory )
					//				currentOrParentDirectoryItem = parent;
					//		}
					//		else
					//			currentOrParentDirectoryItem = i;
					//	}
					//}

					//if( document != null )
					{
						//!!!!надо ли всем иконки. везде так
						//!!!!отсортировать более логично (с учетом как в тулбаре). везде так

						//!!!!вторым уровнем криптонового меню часто создаваемые?

						//!!!!!cut, copy


						//!!!!!
						//Editor
						{
							//!!!!кнопками открывать еще, рядом с "..."

							//!!!!

							//!!!!! imageListContextMenu.Images[ "Delete_16.png" ],
							//!!!!!name
							var item = new KryptonContextMenuItem( Translate( "Editor" ), EditorResourcesCache.Edit,
								delegate ( object s, EventArgs e2 )
								{
									EditorAPI.OpenDocumentWindowForObject( DocumentWindow != null ? DocumentWindow.Document : null, componentItem.Component );
								} );
							item.Enabled = componentItem.Component != null &&
								EditorAPI.IsDocumentObjectSupport( componentItem.Component ) &&
								!componentItem.Component.EditorReadOnlyInHierarchy;
							items.Add( item );
						}

						//!!!!!
						//Settings
						{
							//!!!!! imageListContextMenu.Images[ "Delete_16.png" ],
							//!!!!!name
							var item = new KryptonContextMenuItem( Translate( "Settings" ), EditorResourcesCache.Settings,
								delegate ( object s, EventArgs e2 )
								{
									//!!!!
									if( componentItem != null )
									{
										bool canUseAlreadyOpened = !ModifierKeys.HasFlag( Keys.Shift );
										EditorAPI.ShowObjectSettingsWindow( DocumentWindow.Document, componentItem.Component, canUseAlreadyOpened );
									}
									else
									{
										//!!!!!
										Log.Warning( "impl" );
									}
								} );

							item.Enabled = componentItem.Component != null && !componentItem.Component.EditorReadOnlyInHierarchy && documentOfComponent != null;

							var res = ComponentUtility.GetResourceInstanceByComponent( componentItem.Component );
							if( res != null && res.InstanceType == Resource.InstanceType.Resource )
								item.Enabled = false;

							//item.Enabled = !rootNodeSelected;
							items.Add( item );
						}

						items.Add( new KryptonContextMenuSeparator() );

						//New object
						{
							EditorContextMenuWinForms.AddNewObjectItem( items, CanNewObject( out _ ), delegate ( Metadata.TypeInfo type )
							{
								TryNewObject( type );
							} );

							////!!!!! imageListContextMenu.Images[ "New_16.png" ] );
							//KryptonContextMenuItem item = new KryptonContextMenuItem( Translate( "New object" ), Properties.Resources.New_16,
							//	delegate ( object s, EventArgs e2 )
							//	{
							//		TryNewObject();
							//	} );
							//Item dummy;
							//item.Enabled = CanNewObject( out dummy );
							//items.Add( item );
						}

						//separator
						items.Add( new KryptonContextMenuSeparator() );

						//Cut
						{
							var item = new KryptonContextMenuItem( Translate( "Cut" ), EditorResourcesCache.Cut,
								delegate ( object s, EventArgs e2 )
								{
									Cut();
								} );
							item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Cut" );
							item.Enabled = CanCut();
							items.Add( item );
						}

						//Copy
						{
							var item = new KryptonContextMenuItem( Translate( "Copy" ), EditorResourcesCache.Copy,
								delegate ( object s, EventArgs e2 )
								{
									Copy();
								} );
							item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Copy" );
							item.Enabled = CanCopy();
							items.Add( item );
						}

						//Paste
						{
							var item = new KryptonContextMenuItem( Translate( "Paste" ), EditorResourcesCache.Paste,
								delegate ( object s, EventArgs e2 )
								{
									Paste();
								} );
							item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Paste" );
							item.Enabled = CanPaste( out _, out _, out _, out _ );
							items.Add( item );
						}

						//Clone
						{
							var item = new KryptonContextMenuItem( Translate( "Duplicate" ), EditorResourcesCache.Clone,
								delegate ( object s, EventArgs e2 )
								{
									TryCloneObjects();
								} );
							item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Duplicate" );
							item.Enabled = CanCloneObjects( out _ );
							items.Add( item );
						}

						//separator
						items.Add( new KryptonContextMenuSeparator() );

						//Delete
						{
							var item = new KryptonContextMenuItem( Translate( "Delete" ), EditorResourcesCache.Delete,
								delegate ( object s, EventArgs e2 )
								{
									TryDeleteObjects();
								} );
							item.Enabled = CanDeleteObjects( out _ );
							items.Add( item );
						}

						//Rename
						{
							var item = new KryptonContextMenuItem( Translate( "Rename" ), null,
								delegate ( object s, EventArgs e2 )
								{
									RenameBegin();
								} );
							item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rename" );
							item.Enabled = CanRename();
							items.Add( item );
						}
					}

					//!!!!было
					//EditorContextMenu.AddAdditionalActionsToMenu( items );//, DocumentWindow );

					//if( items.Count != 0 )
					//	items.Add( new KryptonContextMenuSeparator() );

					////!!!!этого нет в Resources, хотя там тоже компонента. есть только во вложенных
					////Utils
					//{
					//	var utilsItem = new KryptonContextMenuItem( Translate( "Utils" ), null );
					//	List<KryptonContextMenuItemBase> items2 = new List<KryptonContextMenuItemBase>();

					//	{
					//		var item = new KryptonContextMenuItem( Translate( "Show object details" ), null,
					//			delegate ( object s, EventArgs e2 )
					//			{
					//				EditorUtils.ShowObjectDetailsAsDocument( componentItem.Component );
					//			} );

					//		item.Enabled = componentItem != null && componentItem.Component != null;
					//		items2.Add( item );
					//	}

					//	utilsItem.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
					//	items.Add( utilsItem );
					//}

					//string nodePath = GetNodePath( node );

					//if( node.Tag == null )
					//{
					//	//item = new KryptonContextMenuItem( Translate("New Folder" ),
					//	//   imageListContextMenu.Images[ "NewFolder_16.png" ],
					//	//   delegate( object s, EventArgs e2 ) { NewDirectory( nodePath ); } );
					//	//menu.Items.Add( item );

					//	{
					//		KryptonContextMenuItem newResourceItem = new KryptonContextMenuItem( Translate( "New" ),
					//			imageListContextMenu.Images[ "New_16.png" ] );

					//		menuItem = new KryptonContextMenuItem( Translate( "Folder" ),
					//			imageListContextMenu.Images[ "Folder_16.png" ], delegate ( object s, EventArgs e2 )
					//			{
					//				NewDirectory( nodePath );
					//			} );
					//		newResourceItem.DropDownItems.Add( menuItem );

					//		newResourceItem.DropDownItems.Add( new ToolStripSeparator() );

					//		//KryptonContextMenuItem newResourceItem = new KryptonContextMenuItem(
					//		//   Translate("New Resource" ),
					//		//   imageListContextMenu.Images[ "NewResource_16.png" ] );

					//		foreach( ResourceType type in ResourceTypeManager.Instance.Types )
					//		{
					//			if( type.AllowNewResource )
					//			{
					//				string displayName = Translate( type.DisplayName );
					//				menuItem = new KryptonContextMenuItem( displayName, type.Icon, NewResourceMenuClick );
					//				menuItem.Tag = new Pair<string, ResourceType>( nodePath, type );
					//				newResourceItem.DropDownItems.Add( menuItem );
					//			}
					//		}

					//		menu.Items.Add( newResourceItem );
					//	}

					//	//item = new KryptonContextMenuItem( Translate("New Resource" ),
					//	//   imageListContextMenu.Images[ "NewResource_16.png" ],
					//	//   delegate( object s, EventArgs e2 ) { NewResource( nodePath ); } );
					//	//menu.Items.Add( item );

					////separator
					//if( menu.Items.Count != 0 )
					//	menu.Items.Add( new ToolStripSeparator() );
					//{
					//	var item = new KryptonContextMenuItem( Translate( "Open folder in Explorer" ), null, delegate ( object s, EventArgs e2 )
					//	{
					//		//!!!!!!
					//		//string realPath = VirtualFileSystem.GetRealPathByVirtual( nodePath );
					//		//Win32Utils.ShellExecuteEx( null, realPath );
					//	} );
					//	menu.Items.Add( item );
					//}

					//if( node.Tag != null )
					//{
					//	//Edit
					//	{
					//		string extension = Path.GetExtension( nodePath );
					//		if( !string.IsNullOrEmpty( extension ) )
					//		{
					//			extension = extension.Substring( 1 );
					//			ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
					//			if( resourceType != null )
					//			{
					//				menuItem = new KryptonContextMenuItem( Translate( "Edit" ), imageListContextMenu.Images[ "Edit_16.png" ],
					//					delegate ( object s, EventArgs e2 )
					//					{
					//						TryBeginEditMode();
					//					} );
					//				menu.Items.Add( menuItem );
					//			}
					//		}
					//	}

					//	//Open in External Program
					//	{
					//		menuItem = new KryptonContextMenuItem( Translate( "Open in External Program" ), null, delegate ( object s, EventArgs e2 )
					//		{
					//			string realPath = VirtualFileSystem.GetRealPathByVirtual( nodePath );
					//			Win32Utils.ShellExecuteEx( null, realPath );
					//		} );
					//		menu.Items.Add( menuItem );
					//	}
					//}

					////Add resource type specific items
					//if( node.Tag as string != null )
					//{
					//	string path = GetNodePath( node );
					//	string extension = Path.GetExtension( path );
					//	if( !string.IsNullOrEmpty( extension ) )
					//	{
					//		extension = extension.Substring( 1 );
					//		ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
					//		if( resourceType != null )
					//			resourceType.DoResourcesTreeContextMenu( path, menu );
					//	}
					//}

					//if( selectedNodes.Count != 0 )
					//{
					//	bool rootNodeSelected = false;
					//	{
					//		foreach( MyTreeNode n in selectedNodes )
					//			if( n.Parent == null )
					//				rootNodeSelected = true;
					//	}

					//	//if( !rootNodeSelected )
					//	{
					//		//separator
					//		menu.Items.Add( new ToolStripSeparator() );

					//		//cut
					//		menuItem = new KryptonContextMenuItem( Translate( "Cut" ), imageListContextMenu.Images[ "Cut_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { CutCopyFiles( true ); } );
					//		menuItem.Enabled = !rootNodeSelected;
					//		menu.Items.Add( menuItem );

					//		//copy
					//		menuItem = new KryptonContextMenuItem( Translate( "Copy" ), imageListContextMenu.Images[ "Copy_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { CutCopyFiles( false ); } );
					//		menu.Items.Add( menuItem );
					//	}

					//	//paste
					//	//if( IsExistsDataToPaste() )
					//	if( node.Tag == null )
					//	{
					//		////separator
					//		//if( rootNodeSelected )
					//		//   menu.Items.Add( new ToolStripSeparator() );

					//		string directoryToPaste = null;
					//		if( IsExistsDataToPaste() )
					//			directoryToPaste = GetSelectedDirectoryToPaste();

					//		//string directoryToPaste = GetSelectedDirectoryToPaste();
					//		//if( directoryToPaste != null )
					//		{
					//			menuItem = new KryptonContextMenuItem( Translate( "Paste" ), imageListContextMenu.Images[ "Paste_16.png" ],
					//				delegate ( object s, EventArgs e2 ) { PasteFiles( directoryToPaste ); } );
					//			menuItem.Enabled = directoryToPaste != null;
					//			menu.Items.Add( menuItem );
					//		}
					//	}

					//	//if( !rootNodeSelected )
					//	{
					//		//separator
					//		menu.Items.Add( new ToolStripSeparator() );

					//		//delete
					//		menuItem = new KryptonContextMenuItem( Translate( "Delete" ), imageListContextMenu.Images[ "Delete_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { TryDeleteObjects(); } );
					//		menuItem.Enabled = !rootNodeSelected;
					//		menu.Items.Add( menuItem );
					//	}
					//	//}

					//	//if( ( ( node.Tag == null && node.Parent != null ) || node.Tag != null ) && selectedNodes.Count == 1 )
					//	{
					//		//rename
					//		menuItem = new KryptonContextMenuItem( Translate( "Rename" ), null, delegate ( object s, EventArgs e2 )
					//		{
					//			//Try begin edit label
					//			if( node != treeView.SelectedNode )
					//				return;
					//			if( !node.IsEditing )
					//				node.BeginEdit();
					//		} );
					//		menuItem.Enabled = selectedNodes.Count == 1 && !rootNodeSelected;
					//		menu.Items.Add( menuItem );
					//	}
					//}

					////Sort by, Refresh
					//{
					//	//separator
					//	menu.Items.Add( new ToolStripSeparator() );

					//	AddSortByToContextMenu( menu );

					//	//Refresh
					//	{
					//		menuItem = new KryptonContextMenuItem( Translate( "Refresh" ), imageListContextMenu.Images[ "Refresh_16.png" ],
					//			delegate ( object s, EventArgs e2 )
					//			{
					//				RefreshPath( (MyTreeNode)node );
					//			} );
					//		menu.Items.Add( menuItem );
					//	}
					//}



					////separator
					//menu.Items.Add( new ToolStripSeparator() );

					////!!!!!?
					//{
					//	var item = new KryptonContextMenuItem( Translate( "Properties" ), null, delegate ( object s, EventArgs e2 )
					//	{
					//		//!!!!!
					//		//string realPath = VirtualPathUtils.GetRealPathByVirtual( nodePath );
					//		//Win32Utils.ShellExecuteEx( "properties", realPath );
					//	} );
					//	menu.Items.Add( item );
					//}
				}
			}

			//_File
			if( contentItem != null )
			{
				//folder specific. //!!!!а на ресурсе почему нельзя?

				ContentBrowserItem_File currentOrParentDirectoryItem = null;
				var fileItem = contentItem as ContentBrowserItem_File;
				{
					if( fileItem != null )
					{
						if( !fileItem.IsDirectory )
						{
							var parent = fileItem.Parent as ContentBrowserItem_File;
							if( parent != null && parent.IsDirectory )
								currentOrParentDirectoryItem = parent;
						}
						else
							currentOrParentDirectoryItem = fileItem;
					}
				}

				if( fileItem != null )
				{

					//!!!!!
					//Open
					if( fileItem != null && !fileItem.IsDirectory )
					{
						var item = new KryptonContextMenuItem( Translate( "Open" ), null, delegate ( object s, EventArgs e2 )
						{
							//!!!!!!
							bool handled = false;
							OpenFile( fileItem, ref handled );
						} );

						//!!!!было
						//item.Font = new Font( item.Font, FontStyle.Bold );

						item.Enabled = EditorAPI.IsDocumentFileSupport( fileItem.FullPath );
						//item.Enabled = currentOrParentDirectoryItem != null;
						items.Add( item );
					}

					//Open with
					if( fileItem != null && !fileItem.IsDirectory )
					{
						//!!!!! imageListContextMenu.Images[ "New_16.png" ] );
						KryptonContextMenuItem itemOpenWith = new KryptonContextMenuItem( Translate( "Open with" ), null );
						//!!!!!
						itemOpenWith.Enabled = currentOrParentDirectoryItem != null || contentItem.Parent == null;

						List<KryptonContextMenuItemBase> items2 = new List<KryptonContextMenuItemBase>();

						//!!!!!движковые сначала

						//!!!!!
						//Text editor
						{
							var item = new KryptonContextMenuItem( Translate( "Text editor" ), null,// imageListContextMenu.Images[ "NewFolder_16.png" ],
							   delegate ( object s, EventArgs e2 )
							   {
								   EditorAPI.OpenFileAsDocument( fileItem.FullPath, true, true, specialMode: "TextEditor" );

								   //var text = File.ReadAllText( fileItem.FullPath );

								   //EditorForm.Instance.OpenTextAsDocument( text, Path.GetFileName( fileItem.path ) + " text", true );
								   //EditorAPI.OpenTextAsDocument( text, "Text of " + Path.GetFileName( fileItem.FullPath ), true );

								   //Log.Warning( "impl" );
								   //!!!!NewDirectory( nodePath );
							   } );
							items2.Add( item );
						}

						//!!!!!
						//External application
						{
							var item = new KryptonContextMenuItem( Translate( "External app" ), null,// imageListContextMenu.Images[ "NewFolder_16.png" ],
							delegate ( object s, EventArgs e2 )
							{
								string filePath = fileItem.FullPath;
								try
								{
									Process.Start( "rundll32.exe", "shell32.dll, OpenAs_RunDLL " + filePath );
								}
								catch( Exception e )
								{
									EditorMessageBox.ShowWarning( e.Message );
								}
							} );
							items2.Add( item );
						}

						////separator
						//items2.Add( new KryptonContextMenuSeparator() );

						////impl
						//{
						//	var item = new KryptonContextMenuItem( Translate( "impl" ), null,// imageListContextMenu.Images[ "NewFolder_16.png" ],
						//	   delegate ( object s, EventArgs e2 )
						//	   {
						//		   //!!!!!
						//	   } );
						//	items2.Add( item );
						//}

						itemOpenWith.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
						items.Add( itemOpenWith );
					}

					if( fileItem.IsDirectory )
					{
						//separator
						if( items.Count != 0 )
							items.Add( new KryptonContextMenuSeparator() );

						//New folder
						{
							var item = new KryptonContextMenuItem( Translate( "New Folder" ), EditorResourcesCache.NewFolder,
							   delegate ( object s, EventArgs e2 )
							   {
								   NewFolder();
							   } );
							item.Enabled = CanNewFolder( out _ );
							items.Add( item );
						}

						//New resource
						{
							EditorContextMenuWinForms.AddNewResourceItem( items, CanNewResource( out _ ), delegate ( Metadata.TypeInfo type )
							{
								NewResource( type );
							} );

							//var item = new KryptonContextMenuItem( Translate( "New Resource" ), Properties.Resources.New_16,
							//   delegate ( object s, EventArgs e2 )
							//   {
							//	   NewResource();
							//   } );
							//item.Enabled = CanNewResource( out _ );
							//items.Add( item );
						}

						//Import
						{
							var item = new KryptonContextMenuItem( Translate( "Import" ), EditorResourcesCache.Import,
							   delegate ( object s, EventArgs e2 )
							   {
								   ImportResource();
							   } );
							item.Enabled = CanImportResource( out _ );
							items.Add( item );
						}
					}

					////New
					//{
					//	KryptonContextMenuItem itemNew = new KryptonContextMenuItem( Translate( "New" ), null );//!!!!! imageListContextMenu.Images[ "New_16.png" ] );
					//	itemNew.Enabled = currentOrParentDirectoryItem != null;

					//	//Folder
					//	{
					//		var item = new KryptonContextMenuItem( Translate( "Folder" ), null,// imageListContextMenu.Images[ "NewFolder_16.png" ],
					//		   delegate ( object s, EventArgs e2 )
					//		   {
					//			   //!!!!
					//			   Log.Warning( "impl" );
					//			   //!!!!NewDirectory( nodePath );
					//		   } );
					//		itemNew.DropDownItems.Add( item );
					//	}

					//	//separator
					//	itemNew.DropDownItems.Add( new ToolStripSeparator() );

					//	//Resource
					//	{
					//		var item = new KryptonContextMenuItem( Translate( "Resource" ), null,// imageListContextMenu.Images[ "NewFolder_16.png" ],
					//		   delegate ( object s, EventArgs e2 )
					//		   {
					//			   string virtualPath = VirtualPathUtils.GetVirtualPathByReal( currentOrParentDirectoryItem.FullPath );
					//			   EditorForm.Instance.ShowNewObjectWindow( virtualPath, null );
					//		   } );
					//		item.Enabled = currentOrParentDirectoryItem != null &&
					//			VirtualPathUtils.GetVirtualPathByReal( currentOrParentDirectoryItem.FullPath ) != "";
					//		itemNew.DropDownItems.Add( item );
					//	}

					//	menu.Items.Add( itemNew );
					//}

					//string nodePath = GetNodePath( node );

					//if( node.Tag == null )
					//{
					//	//item = new KryptonContextMenuItem( Translate("New Folder" ),
					//	//   imageListContextMenu.Images[ "NewFolder_16.png" ],
					//	//   delegate( object s, EventArgs e2 ) { NewDirectory( nodePath ); } );
					//	//menu.Items.Add( item );

					//	{
					//		KryptonContextMenuItem newResourceItem = new KryptonContextMenuItem( Translate( "New" ),
					//			imageListContextMenu.Images[ "New_16.png" ] );

					//		menuItem = new KryptonContextMenuItem( Translate( "Folder" ),
					//			imageListContextMenu.Images[ "Folder_16.png" ], delegate ( object s, EventArgs e2 )
					//			{
					//				NewDirectory( nodePath );
					//			} );
					//		newResourceItem.DropDownItems.Add( menuItem );

					//		newResourceItem.DropDownItems.Add( new ToolStripSeparator() );

					//		//KryptonContextMenuItem newResourceItem = new KryptonContextMenuItem(
					//		//   Translate("New Resource" ),
					//		//   imageListContextMenu.Images[ "NewResource_16.png" ] );

					//		foreach( ResourceType type in ResourceTypeManager.Instance.Types )
					//		{
					//			if( type.AllowNewResource )
					//			{
					//				string displayName = Translate( type.DisplayName );
					//				menuItem = new KryptonContextMenuItem( displayName, type.Icon, NewResourceMenuClick );
					//				menuItem.Tag = new Pair<string, ResourceType>( nodePath, type );
					//				newResourceItem.DropDownItems.Add( menuItem );
					//			}
					//		}

					//		menu.Items.Add( newResourceItem );
					//	}

					//	//item = new KryptonContextMenuItem( Translate("New Resource" ),
					//	//   imageListContextMenu.Images[ "NewResource_16.png" ],
					//	//   delegate( object s, EventArgs e2 ) { NewResource( nodePath ); } );
					//	//menu.Items.Add( item );

					//Open folder in Explorer
					if( fileItem.IsDirectory )
					{
						//separator
						if( items.Count != 0 )
							items.Add( new KryptonContextMenuSeparator() );

						var item = new KryptonContextMenuItem( Translate( "Open Folder in Explorer" ), null, delegate ( object s, EventArgs e2 )
						{
							//!!!!!!
							string realPath = currentOrParentDirectoryItem.FullPath;// VirtualPathUtils.GetRealPathByVirtual( nodePath );
							Win32Utility.ShellExecuteEx( null, realPath );
						} );
						item.Enabled = currentOrParentDirectoryItem != null;
						items.Add( item );
					}

					//separator
					if( items.Count != 0 )
						items.Add( new KryptonContextMenuSeparator() );

					//if( node.Tag != null )
					//{
					//	//Edit
					//	{
					//		string extension = Path.GetExtension( nodePath );
					//		if( !string.IsNullOrEmpty( extension ) )
					//		{
					//			extension = extension.Substring( 1 );
					//			ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
					//			if( resourceType != null )
					//			{
					//				menuItem = new KryptonContextMenuItem( Translate( "Edit" ), imageListContextMenu.Images[ "Edit_16.png" ],
					//					delegate ( object s, EventArgs e2 )
					//					{
					//						TryBeginEditMode();
					//					} );
					//				menu.Items.Add( menuItem );
					//			}
					//		}
					//	}

					//	//Open in External Program
					//	{
					//		menuItem = new KryptonContextMenuItem( Translate( "Open in External Program" ), null, delegate ( object s, EventArgs e2 )
					//		{
					//			string realPath = VirtualFileSystem.GetRealPathByVirtual( nodePath );
					//			Win32Utils.ShellExecuteEx( null, realPath );
					//		} );
					//		menu.Items.Add( menuItem );
					//	}
					//}

					////Add resource type specific items
					//if( node.Tag as string != null )
					//{
					//	string path = GetNodePath( node );
					//	string extension = Path.GetExtension( path );
					//	if( !string.IsNullOrEmpty( extension ) )
					//	{
					//		extension = extension.Substring( 1 );
					//		ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
					//		if( resourceType != null )
					//			resourceType.DoResourcesTreeContextMenu( path, menu );
					//	}
					//}

					//if( selectedNodes.Count != 0 )
					//{
					//	bool rootNodeSelected = false;
					//	{
					//		foreach( MyTreeNode n in selectedNodes )
					//			if( n.Parent == null )
					//				rootNodeSelected = true;
					//	}

					//	//if( !rootNodeSelected )
					//	{
					//		//separator
					//		menu.Items.Add( new ToolStripSeparator() );

					//		//cut
					//		menuItem = new KryptonContextMenuItem( Translate( "Cut" ), imageListContextMenu.Images[ "Cut_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { CutCopyFiles( true ); } );
					//		menuItem.Enabled = !rootNodeSelected;
					//		menu.Items.Add( menuItem );

					//		//copy
					//		menuItem = new KryptonContextMenuItem( Translate( "Copy" ), imageListContextMenu.Images[ "Copy_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { CutCopyFiles( false ); } );
					//		menu.Items.Add( menuItem );
					//	}

					//	//paste
					//	//if( IsExistsDataToPaste() )
					//	if( node.Tag == null )
					//	{
					//		////separator
					//		//if( rootNodeSelected )
					//		//   menu.Items.Add( new ToolStripSeparator() );

					//		string directoryToPaste = null;
					//		if( IsExistsDataToPaste() )
					//			directoryToPaste = GetSelectedDirectoryToPaste();

					//		//string directoryToPaste = GetSelectedDirectoryToPaste();
					//		//if( directoryToPaste != null )
					//		{
					//			menuItem = new KryptonContextMenuItem( Translate( "Paste" ), imageListContextMenu.Images[ "Paste_16.png" ],
					//				delegate ( object s, EventArgs e2 ) { PasteFiles( directoryToPaste ); } );
					//			menuItem.Enabled = directoryToPaste != null;
					//			menu.Items.Add( menuItem );
					//		}
					//	}

					//	//if( !rootNodeSelected )
					//	{
					//		//separator
					//		menu.Items.Add( new ToolStripSeparator() );

					//		//delete
					//		menuItem = new KryptonContextMenuItem( Translate( "Delete" ), imageListContextMenu.Images[ "Delete_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { TryDeleteObjects(); } );
					//		menuItem.Enabled = !rootNodeSelected;
					//		menu.Items.Add( menuItem );
					//	}
					//	//}

					//	//if( ( ( node.Tag == null && node.Parent != null ) || node.Tag != null ) && selectedNodes.Count == 1 )
					//	{
					//		//rename
					//		menuItem = new KryptonContextMenuItem( Translate( "Rename" ), null, delegate ( object s, EventArgs e2 )
					//		{
					//			//Try begin edit label
					//			if( node != treeView.SelectedNode )
					//				return;
					//			if( !node.IsEditing )
					//				node.BeginEdit();
					//		} );
					//		menuItem.Enabled = selectedNodes.Count == 1 && !rootNodeSelected;
					//		menu.Items.Add( menuItem );
					//	}
					//}

					////Sort by, Refresh
					//{
					//	//separator
					//	menu.Items.Add( new ToolStripSeparator() );

					//	AddSortByToContextMenu( menu );

					//	//Refresh
					//	{
					//		menuItem = new KryptonContextMenuItem( Translate( "Refresh" ), imageListContextMenu.Images[ "Refresh_16.png" ],
					//			delegate ( object s, EventArgs e2 )
					//			{
					//				RefreshPath( (MyTreeNode)node );
					//			} );
					//		menu.Items.Add( menuItem );
					//	}
					//}

					//Cut
					{
						var item = new KryptonContextMenuItem( Translate( "Cut" ), EditorResourcesCache.Cut,
							delegate ( object s, EventArgs e2 )
							{
								Cut();
							} );
						item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Cut" );
						item.Enabled = CanCut();
						items.Add( item );
					}

					//Copy
					{
						var item = new KryptonContextMenuItem( Translate( "Copy" ), EditorResourcesCache.Copy,
							delegate ( object s, EventArgs e2 )
							{
								Copy();
							} );
						item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Copy" );
						item.Enabled = CanCopy();
						items.Add( item );
					}

					//Paste
					if( fileItem.IsDirectory || ( !fileItem.IsDirectory && CanPaste( out _, out _, out _, out _ ) ) )
					{
						var item = new KryptonContextMenuItem( Translate( "Paste" ), EditorResourcesCache.Paste,
							delegate ( object s, EventArgs e2 )
							{
								Paste();
							} );
						item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Paste" );
						item.Enabled = CanPaste( out _, out _, out _, out _ );
						items.Add( item );
					}

					//separator
					items.Add( new KryptonContextMenuSeparator() );

					//Delete
					{
						var item = new KryptonContextMenuItem( Translate( "Delete" ), EditorResourcesCache.Delete,
							delegate ( object s, EventArgs e2 )
							{
								TryDeleteObjects();
							} );
						item.Enabled = CanDeleteObjects( out _ );
						items.Add( item );
					}

					//Rename
					{
						var item = new KryptonContextMenuItem( Translate( "Rename" ), null,
							delegate ( object s, EventArgs e2 )
							{
								RenameBegin();
							} );
						item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rename" );
						item.Enabled = CanRename();
						items.Add( item );
					}

					//separator
					items.Add( new KryptonContextMenuSeparator() );

					//Sort by
					if( fileItem != null && fileItem.IsDirectory )
					{
						AddSortByToContextMenu( items );

						//separator
						items.Add( new KryptonContextMenuSeparator() );
					}

					//Properties
					{
						var item = new KryptonContextMenuItem( Translate( "Properties" ), null, delegate ( object s, EventArgs e2 )
						{
							//!!!!!
							string realPath = fileItem.FullPath;
							//string realPath = currentOrParentDirectoryItem.FullPath;// VirtualPathUtils.GetRealPathByVirtual( nodePath );
							Win32Utility.ShellExecuteEx( "properties", realPath );
						} );
						item.Enabled = fileItem != null;
						//item.Enabled = currentOrParentDirectoryItem != null;
						items.Add( item );
					}
				}
			}

			//_Type
			if( contentItem != null )
			{
				var typeItem = contentItem as ContentBrowserItem_Type;
				if( typeItem != null )
				{
					//New resource
					{
						var item = new KryptonContextMenuItem( Translate( "New Resource of This Type" ), EditorResourcesCache.New,
						   delegate ( object s, EventArgs e2 )
						   {
							   var initData = new NewObjectWindow.CreationDataClass();
							   initData.initFileCreationDirectory = VirtualDirectory.Exists( "New" ) ? "New" : "";
							   //initData.initFileCreationDirectory = VirtualDirectory.Exists( "Created" ) ? "Created" : "";
							   initData.initLockType = typeItem.Type;
							   initData.createdFromContentBrowser = this;
							   EditorAPI.OpenNewObjectWindow( initData );
						   } );
						item.Enabled =
							MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( typeItem.Type ) ||
							MetadataManager.GetTypeOfNetType( typeof( NewResourceType ) ).IsAssignableFrom( typeItem.Type );
						items.Add( item );
					}

					//Favorites
					if( contentItem.Parent != favoritesItem )
					{
						//Add to Favorites

						var item = new KryptonContextMenuItem( Translate( "Add to Favorites" ), EditorResourcesCache.Add,
						   delegate ( object s, EventArgs e2 )
						   {
							   EditorFavorites.Add( typeItem.Type );

							   //!!!!можно обновлять не полностью
							   if( EditorAPI.FindWindow<ResourcesWindow>().ContentBrowser1 != this )
								   EditorAPI.FindWindow<ResourcesWindow>().ContentBrowser1.UpdateData();
							   UpdateData();
						   } );
						item.Enabled =
							MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( typeItem.Type ) &&
							!EditorFavorites.Contains( typeItem.Type );
						// ||
						//MetadataManager.GetTypeOfNetType( typeof( NewResourceType ) ).IsAssignableFrom( typeItem.Type );
						items.Add( item );
					}
					else
					{
						//Remove from Favorites

						var item = new KryptonContextMenuItem( Translate( "Remove from Favorites" ), EditorResourcesCache.Delete,
						   delegate ( object s, EventArgs e2 )
						   {
							   EditorFavorites.Remove( typeItem.Type );

							   //!!!!можно обновлять не полностью
							   if( EditorAPI.FindWindow<ResourcesWindow>().ContentBrowser1 != this )
								   EditorAPI.FindWindow<ResourcesWindow>().ContentBrowser1.UpdateData();
							   UpdateData();
						   } );
						item.Enabled =
							MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( typeItem.Type );
						// ||
						//MetadataManager.GetTypeOfNetType( typeof( NewResourceType ) ).IsAssignableFrom( typeItem.Type );
						items.Add( item );
					}
				}
			}

			var menuType = EditorContextMenuWinForms.MenuTypeEnum.General;
			if( Mode == ModeEnum.Resources )
				menuType = EditorContextMenuWinForms.MenuTypeEnum.Resources;
			else if( DocumentWindow != null )
				menuType = EditorContextMenuWinForms.MenuTypeEnum.Document;

			EditorContextMenuWinForms.AddActionsToMenu( menuType, items );

			ShowContextMenuEvent?.Invoke( this, items );

			EditorContextMenuWinForms.Show( items, locationControl, locationPoint );
		}

		//!!!!!
		//void ShowContextMenuNotOverNode( Point location )
		//{
		//    //cancel if edit mode activated
		//    if( IsResourceEditModeActive != null )
		//    {
		//        ActiveEventArgs e = new ActiveEventArgs();
		//        IsResourceEditModeActive( e );
		//        if( e.Active )
		//        {
		//            ShowContextMenuForEditMode( location );
		//            return;
		//        }
		//    }

		//    ContextMenuStrip menu = new ContextMenuStrip();
		//    EditorBase.Theme.EditorTheme.ApplyStyle( menu );
		//    menu.Font = MainForm.GetFont( MainForm.fontContextMenu, menu.Font );

		//    AddSortByToContextMenu( menu );

		//    menu.Show( treeView, location );
		//}

		//!!!!!
		//void BeginNewDirectoryCreation( string directory )
		//{
		//	MyTreeNode parentNode = GetNodeByPath( directory );
		//	Trace.Assert( parentNode != null );

		//	string name;
		//	{
		//		for( int n = 1; ; n++ )
		//		{
		//			name = "New Folder";
		//			if( n != 1 )
		//				name += " (" + n.ToString() + ")";

		//			bool finded = false;
		//			foreach( TreeNode child in parentNode.Nodes )
		//			{
		//				if( string.Compare( child.Name, name, true ) == 0 )
		//				{
		//					finded = true;
		//					break;
		//				}
		//			}
		//			if( !finded )
		//				break;
		//		}
		//	}

		//	string realDir = Path.Combine( VirtualFileSystem.GetRealPathByVirtual( directory ), name );

		//	try
		//	{
		//		Directory.CreateDirectory( realDir );
		//	}
		//	catch( Exception e )
		//	{
		//		Log.Warning( e.Message );
		//		return;
		//	}

		//	TreeNode node = new MyTreeNode( name, VirtualDirectory.IsInArchive( directory ),
		//		IsNeedHideItem( name ) || parentNode.HideNode );
		//	node.Name = node.Text;
		//	UpdateNodeIcon( node );
		//	parentNode.Nodes.Add( node );

		//	treeView.SelectedNode = node;
		//	node.BeginEdit();
		//}

		//!!!!!
		//public void BeginNewDirectoryCreation()
		//{
		//	TreeNode node = treeView.SelectedNode;
		//	if( node == null )
		//		return;

		//	string nodePath = GetNodePath( node );
		//	if( node.Tag != null )
		//		nodePath = Path.GetDirectoryName( nodePath );

		//	NewDirectory( nodePath );
		//}

		//public delegate void CanDeleteObjectsEventDelegate( ContentBrowser sender, ref bool handled, ref List<Item> resultItemsToDelete );
		//public event CanDeleteObjectsEventDelegate CanDeleteObjectsEvent;

		public bool CanDeleteObjects( out List<Item> resultItemsToDelete )
		{
			resultItemsToDelete = new List<Item>();

			if( ReadOnlyHierarchy )
				return false;

			//bool handled = false;
			//CanDeleteObjectsEvent?.Invoke( this, ref handled, ref resultItemsToDelete );
			//if( !handled )
			{
				//!!!!multiselection

				foreach( var item in SelectedItems )
				{
					if( item.Parent != null )
					{
						var componentItem = item as ContentBrowserItem_Component;
						if( componentItem != null )
						{
							var component = componentItem.Component;
							if( component != null && component.Parent != null && !component.EditorReadOnlyInHierarchy && EditorAPI.GetDocumentByObject( component ) != null )
								resultItemsToDelete.Add( componentItem );
						}

						//var noSpecItem = item as ContentBrowserItem_NoSpecialization;
						//if( noSpecItem != null && noSpecItem.CanDelete() )
						//	resultItemsToDelete.Add( noSpecItem );
					}

					var fileItem = item as ContentBrowserItem_File;
					if( fileItem != null )
						resultItemsToDelete.Add( fileItem );
				}
			}

			if( resultItemsToDelete.Count == 0 )
				return false;

			return true;
		}

		//public delegate void TryDeleteObjectsEventDelegate( ContentBrowser sender, ref bool handled );
		//public event TryDeleteObjectsEventDelegate TryDeleteObjectsEvent;

		public void TryDeleteObjects()
		{
			//bool handled = false;
			//TryDeleteObjectsEvent?.Invoke( this, ref handled );
			//if( handled )
			//	return;

			//!!!!mutliselection
			//!!!!!игнорить выделенные-вложенные. где еще так

			if( !CanDeleteObjects( out List<Item> itemsToDelete ) )
				return;

			//!!!!
			////no delete in edit mode
			//if( IsResourceEditModeActive != null )
			//{
			//	ActiveEventArgs e = new ActiveEventArgs();
			//	IsResourceEditModeActive( e );
			//	if( e.Active )
			//	{
			//		Log.Warning( ToolsLocalization.Translate( "ResourcesForm", "Need to leave the editing mode first." ) );
			//		return;
			//	}
			//}

			//!!!!!
			////close current opened resource
			//if( ResourceChange != null )
			//{
			//	CancelEventArgs cancelEventArgs = new CancelEventArgs();
			//	ResourceChange( null, cancelEventArgs );
			//	Trace.Assert( !cancelEventArgs.Cancel );
			//}

			string text;
			if( itemsToDelete.Count == 1 )
			{
				string template = Translate( "Are you sure you want to delete \'{0}\'?" );

				var item = itemsToDelete[ 0 ];

				string name;

				var fileItem = item as ContentBrowserItem_File;
				if( fileItem != null )
					name = fileItem.FullPath;
				else if( item.ContainedObject != null )
					name = item.ContainedObject.ToString();
				else
				{
					//!!!!
					name = item.ToString();
				}

				text = string.Format( template, name );
			}
			else
			{
				string template = Translate( "Are you sure you want to delete selected objects?" );
				text = string.Format( template, itemsToDelete.Count );
			}

			if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.No )
				return;

			Item itemToSelectAfterDelection = null;
			{
				Item itemWithMinIndex = null;
				int minIndex = 0;
				foreach( var item in itemsToDelete )
				{
					if( item.Parent != null )
					{
						int index = item.Parent.GetChildren( true ).IndexOf( item );
						if( itemWithMinIndex == null || index < minIndex )
						{
							itemWithMinIndex = item;
							minIndex = index;
						}
					}
				}
				if( itemWithMinIndex != null )
				{
					if( minIndex > 0 )
						itemToSelectAfterDelection = itemWithMinIndex.Parent.GetChildren( true )[ minIndex - 1 ];
					else
						itemToSelectAfterDelection = itemWithMinIndex.Parent;
				}
			}

			var rootComponent = rootObject as Component;
			if( rootComponent != null )
			{
				//Component

				//!!!!!всегда ли компоненты?

				List<Component> deleted = new List<Component>();
				foreach( var item in itemsToDelete )
					deleted.Add( (Component)item.ContainedObject );

				//update document
				if( DocumentWindow != null && DocumentWindow.Document != null )
				{
					var document = DocumentWindow.Document;
					var action = new UndoActionComponentCreateDelete( document, deleted, false );
					document.UndoSystem.CommitAction( action );
					document.Modified = true;
				}
				else
				{
					Log.Warning( "Can't be here." );
					//foreach( var item in selectedItemsToDelete )
					//{
					//	//item.Component.RemoveFromParent( false );
					//	//item.Component.Dispose();
					//	deleted.Add( item.Component );
					//}
				}
			}
			//else if( rootObject != null && ContentBrowserItem_NoSpecialization.IsList( rootObject ) )
			//{
			//	xx

			//	//List

			//	List<int> objectIndexesToDelete = new List<int>();
			//	foreach( var item in itemsToDelete )
			//	{
			//		var noSpecItem = item as ContentBrowserItem_NoSpecialization;
			//		if( noSpecItem != null )
			//			objectIndexesToDelete.Add( noSpecItem.listIndex );
			//	}

			//	//update document
			//	if( DocumentWindow != null && DocumentWindow.Document != null )
			//	{
			//		var document = DocumentWindow.Document;

			//		var action = new UndoActionListAddRemove( rootObject, objectIndexesToDelete, false );
			//		document.UndoSystem.CommitAction( action );

			//		document.Modified = true;
			//	}
			//	else
			//	{
			//		Log.Warning( "Can't be here." );
			//		//foreach( var item in selectedItemsToDelete )
			//		//{
			//		//	//item.Component.RemoveFromParent( false );
			//		//	//item.Component.Dispose();
			//		//	deleted.Add( item.Component );
			//		//}
			//	}
			//}
			else
			{
				foreach( var item in itemsToDelete )
				{
					var fileItem = item as ContentBrowserItem_File;
					if( fileItem != null )
					{
						try
						{
							if( Directory.Exists( fileItem.FullPath ) )
							{
								Directory.Delete( fileItem.FullPath, true );
							}
							else
							{
								if( File.Exists( fileItem.FullPath ) )
								{
									//cs file specific
									if( Path.GetExtension( fileItem.FullPath ).ToLower() == ".cs" )
										CSharpProjectFileUtility.UpdateProjectFile( null, new string[] { fileItem.FullPath }, out _ );

									File.Delete( fileItem.FullPath );
									//!!!!!
									//deleted = true;
								}
							}
						}
						catch( Exception e )
						{
							Log.Error( e.Message );
						}
					}
					else
					{
						//!!!!?
					}
				}
			}

			//select item
			if( itemToSelectAfterDelection != null )
				SelectItems( new Item[] { itemToSelectAfterDelection }, false, true );
		}

		public bool CanCloneObjects( out List<Item> resultItemsToClone )
		{
			resultItemsToClone = new List<Item>();

			if( ReadOnlyHierarchy )
				return false;
			if( documentWindow == null || documentWindow.Document == null )
				return false;

			//!!!!mutliselection

			//!!!!вложенные друг в друга

			foreach( var item in SelectedItems )
			{
				if( item.Parent != null )
				{
					var componentItem = item as ContentBrowserItem_Component;
					if( componentItem != null )
					{
						var component = componentItem.Component;
						if( component != null && component.Parent != null && !component.EditorReadOnlyInHierarchy && EditorAPI.GetDocumentByObject( component ) != null )
							resultItemsToClone.Add( componentItem );
					}

					//!!!!

					//var noSpecItem = item as ContentBrowserItem_NoSpecialization;
					//if( noSpecItem != null && noSpecItem.CanDelete() )
					//	resultObjectsToClone.Add( noSpecItem );
				}

				//!!!!

				//var fileItem = item as ContentBrowserItem_File;
				//if( fileItem != null )
				//	resultItemsToDelete.Add( fileItem );
			}

			if( resultItemsToClone.Count == 0 )
				return false;

			return true;
		}

		public void TryCloneObjects()
		{
			//!!!!mutliselection
			//!!!!!игнорить выделенные-вложенные. где еще так

			if( !CanCloneObjects( out List<Item> itemsToClone ) )
				return;

			List<Component> newObjects = new List<Component>();
			foreach( var item in itemsToClone )
			{
				var componentItem = item as ContentBrowserItem_Component;
				if( componentItem != null && componentItem.Component != null && componentItem.Component.Parent != null )
				{
					var obj = componentItem.Component;
					var newObject = EditorUtility.CloneComponent( obj );
					newObjects.Add( newObject );
				}

				//!!!!
			}

			//!!!!выделить новые
			////select objects
			//{
			//	List<object> selectObjects = new List<object>();
			//	//!!!!все выделить?
			//	selectObjects.AddRange( newObjects );

			//	SelectObjects( selectObjects );
			//}

			if( newObjects.Count == 0 )
				return;

			//add to undo with deletion
			var action = new UndoActionComponentCreateDelete( documentWindow.Document, newObjects, true );
			documentWindow.Document.UndoSystem.CommitAction( action );
			documentWindow.Document.Modified = true;
		}

		public int CalculateHeight()
		{
			return toolStripForTreeView.Height + DpiHelper.Default.ScaleValue( treeView.RowHeight ) * treeView.ItemCount;
			//return toolStrip1.Height + treeView.ItemHeight * treeView.GetNodeCount( true );
		}

		[Browsable( false )]
		public ESet<Item> Items
		{
			get { return items; }
		}

		private void treeView_Expanding( object sender, TreeViewAdvEventArgs e )
		{
			var node = GetItemTreeNode( e.Node );
			if( node == null )
				return;

			var item = GetItemByNode( GetItemTreeNode( e.Node ) );

			//if( lastDoubleClick && e.Action == TreeViewAction.Expand && item != null && item.chooseByDoubleClickAndReturnKey )
			//{
			//	e.Cancel = true;
			//	return;
			//}

			//update wasExpanded parameter
			if( item != null && !item.wasExpanded )
			{
				item.wasExpanded = true;

				AddChildNodes( item, null );

				////update children of _Member items. gradual children creation.
				//var memberItem = item as ContentBrowserItem_Member;
				//if( memberItem != null )
				//{
				//	foreach( var item2 in memberItem.GetChildren() )
				//		item2.PerformChildrenChanged();
				//}
			}
		}

		private void treeView_Collapsing( object sender, TreeViewAdvEventArgs e )
		{
			//var item = GetItemByNode( GetItemTreeNode( e.Node ) );

			//if( lastDoubleClick && e.Action == TreeViewAction.Collapse && item != null && item.chooseByDoubleClickAndReturnKey )
			//{
			//	e.Cancel = true;
			//	return;
			//}
		}

		private void toolStripButtonOptions_Click( object sender, EventArgs e )
		{
			OptionsClick();// false );
		}

		void OptionsClick()//bool forceClose )
		{
			var form = new ContentBrowserOptionsForm( this );
			//form.Browser = this;

			if( EditorForm.Instance == null )
			{
				form.ShowDialog();
			}
			else
			{
				EditorForm.Instance.WorkspaceController.BlockAutoHideAndDoAction( this, () =>
				{
					form.ShowDialog();
				} );
			}

			//ContentBrowserOptionsForm.createdForms.TryGetValue( this, out ContentBrowserOptionsForm form );
			//if( form != null && !form.IsHandleCreated )
			//	form = null;

			//if( form == null )
			//{
			//	if( !forceClose )
			//	{
			//		form = new ContentBrowserOptionsForm();
			//		form.Browser = this;
			//		form.Show();
			//	}
			//}
			//else
			//	form.Close();
		}

		//[TypeConverter( typeof( ExpandableObjectConverter ) )]
		[Browsable( false )]
		public ContentBrowserOptions Options
		{
			get { return options; }
			set { options = value; }
		}

		//!!!!?
		List<Component> GetSelectedComponents()
		{
			var result = new List<Component>();
			foreach( var item in SelectedItems )
			{
				var componentItem = item as ContentBrowserItem_Component;
				if( componentItem != null && componentItem.Component != null )
					result.Add( componentItem.Component );
			}
			return result;
		}

		void UpdateToolBar()
		{
			bool show = ShowToolBar && !ReadOnlyHierarchy;
			if( toolStripForTreeView.Visible != show )
				toolStripForTreeView.Visible = show;

			//update Visible of items

			//toolStripButtonOptions.Visible = Options.OptionsButton;

			toolStripSeparatorFilteringMode.Visible = false;//Options.ShowFilteringModeButton;
			toolStripDropDownButtonFilteringMode.Visible = Options.FilteringModeButton && Mode == ModeEnum.Resources;

			toolStripButtonShowMembers.Visible = Options.MembersButton && Mode == ModeEnum.Objects;

			toolStripSeparatorOpen.Visible = Options.OpenButton && ( Mode == ModeEnum.Resources || Mode == ModeEnum.SetReference );
			toolStripButtonOpen.Visible = Options.OpenButton && ( Mode == ModeEnum.Resources || Mode == ModeEnum.SetReference );
			toolStripButtonOpen.Enabled = CanOpenFile( out _ );

			toolStripSeparatorEditSettings.Visible = Options.EditorButton || Options.SettingsButton;
			toolStripButtonEditor.Visible = Options.EditorButton;
			toolStripButtonSettings.Visible = Options.SettingsButton;

			toolStripSeparatorButtonsForEditing.Visible = Options.ButtonsForEditing;

			toolStripButtonNewFolder.Visible = Options.ButtonsForEditing && ( Mode == ModeEnum.Resources || Mode == ModeEnum.SetReference );
			toolStripButtonNewResource.Visible = Options.ButtonsForEditing && ( Mode == ModeEnum.Resources || Mode == ModeEnum.SetReference );
			toolStripButtonNewObject.Visible = Options.ButtonsForEditing && Mode == ModeEnum.Objects;
			toolStripButtonDelete.Visible = Options.ButtonsForEditing;
			toolStripButtonRename.Visible = Options.ButtonsForEditing;
			//toolStripButtonDuplicate.Visible = Options.ShowButtonsForEditing;
			//!!!!Mode == ModeEnum.Objects
			toolStripButtonClone.Visible = Options.ButtonsForEditing && Mode == ModeEnum.Objects;
			toolStripButtonMoveUp.Visible = Options.ButtonsForEditing && Mode == ModeEnum.Objects;
			toolStripButtonMoveDown.Visible = Options.ButtonsForEditing && Mode == ModeEnum.Objects;
			//toolStripButtonMoveTo.Visible = Options.ShowButtonsForEditing && Mode == ModeEnum.Objects;
			toolStripButtonCut.Visible = Options.ButtonsForEditing;
			toolStripButtonCopy.Visible = Options.ButtonsForEditing;
			toolStripButtonPaste.Visible = Options.ButtonsForEditing;

			toolStripSeparatorSearch.Visible = Options.SearchButton && Mode == ModeEnum.Objects;
			toolStripButtonSearch.Visible = Options.SearchButton && Mode == ModeEnum.Objects;

			toolStripSeparatorSearchBar.Visible = Options.SearchBar;
			toolStripTextBoxSearch.Visible = Options.SearchBar;


			//update Enabled

			var selectedComponents = GetSelectedComponents();
			toolStripButtonEditor.Enabled = selectedComponents.Count == 1 && DocumentWindow != null && DocumentWindow.Document != null && EditorAPI.IsDocumentObjectSupport( selectedComponents[ 0 ] ) && !selectedComponents[ 0 ].EditorReadOnlyInHierarchy;
			toolStripButtonSettings.Enabled = selectedComponents.Count == 1 && DocumentWindow != null && DocumentWindow.Document != null && !selectedComponents[ 0 ].EditorReadOnlyInHierarchy;

			toolStripButtonNewFolder.Enabled = CanNewFolder( out _ );
			toolStripButtonNewResource.Enabled = CanNewResource( out _ );
			toolStripButtonNewObject.Enabled = CanNewObject( out _ );
			toolStripButtonDelete.Enabled = CanDeleteObjects( out _ );
			toolStripButtonClone.Enabled = CanCloneObjects( out _ );
			toolStripButtonMoveUp.Enabled = CanMoveUp( out _ );
			toolStripButtonMoveDown.Enabled = CanMoveDown( out _ );

			toolStripButtonCut.Enabled = CanCut();
			toolStripButtonCopy.Enabled = CanCopy();
			toolStripButtonPaste.Enabled = CanPaste( out _, out _, out _, out _ );
			toolStripButtonRename.Enabled = CanRename();

			////update filtering modes
			//foreach( ToolStripMenuItem item in toolStripDropDownButtonFilteringMode.DropDownItems )
			//{
			//	bool check = item.Tag == filteringMode;
			//	if( item.Checked != check )
			//		item.Checked = check;
			//}
		}

		private void toolStripButtonEditor_Click( object sender, EventArgs e )
		{
			var selectedComponents = GetSelectedComponents();
			if( selectedComponents.Count == 1 && DocumentWindow != null && DocumentWindow.Document != null )
			{
				EditorAPI.OpenDocumentWindowForObject( DocumentWindow.Document, selectedComponents[ 0 ] );
			}
		}

		private void toolStripButtonSettings_Click( object sender, EventArgs e )
		{
			var selectedComponents = GetSelectedComponents();
			if( selectedComponents.Count == 1 && DocumentWindow != null && DocumentWindow.Document != null )
			{
				bool canUseAlreadyOpened = !ModifierKeys.HasFlag( Keys.Shift );
				EditorAPI.ShowObjectSettingsWindow( DocumentWindow.Document, selectedComponents[ 0 ], canUseAlreadyOpened );
			}
		}

		private void toolStripButtonNewFolder_Click( object sender, EventArgs e )
		{
			NewFolder();
		}

		private void toolStripButtonNewResource_Click( object sender, EventArgs e )
		{
			NewResource( null );
		}

		private void toolStripButtonNewObject_Click( object sender, EventArgs e )
		{
			TryNewObject( null );
		}

		private void toolStripButtonDelete_Click( object sender, EventArgs e )
		{
			TryDeleteObjects();
		}

		private void toolStripButtonRename_Click( object sender, EventArgs e )
		{
			RenameBegin();
		}

		private void toolStripButtonClone_Click( object sender, EventArgs e )
		{
			TryCloneObjects();
		}

		private void toolStripButtonMoveUp_Click( object sender, EventArgs e )
		{
			TryMoveUp();
		}

		private void toolStripButtonMoveDown_Click( object sender, EventArgs e )
		{
			TryMoveDown();
		}

		private void toolStripButtonCut_Click( object sender, EventArgs e )
		{
			Cut();
		}

		private void toolStripButtonCopy_Click( object sender, EventArgs e )
		{
			Copy();
		}

		private void toolStripButtonPaste_Click( object sender, EventArgs e )
		{
			Paste();
		}

		//!!!!multiselection
		//!!!!!!MoveDown тоже
		//!!!!только компоненты?
		bool CanMoveUp( out Component component )
		{
			var selectedComponents = GetSelectedComponents();
			if( selectedComponents.Count == 1 && SelectedItems[ 0 ].Parent != null && DocumentWindow != null && DocumentWindow.Document != null &&
				!selectedComponents[ 0 ].EditorReadOnlyInHierarchy )
			{
				var c = selectedComponents[ 0 ];

				var parent = c.Parent;
				if( parent != null )
				{
					int index = parent.Components.IndexOf( c );
					if( index > 0 )
					{
						component = c;
						return true;
					}
				}
			}
			component = null;
			return false;
		}

		bool CanMoveDown( out Component component )
		{
			var selectedComponents = GetSelectedComponents();
			if( selectedComponents.Count == 1 && SelectedItems[ 0 ].Parent != null && DocumentWindow != null && DocumentWindow.Document != null &&
				!selectedComponents[ 0 ].EditorReadOnlyInHierarchy )
			{
				var c = selectedComponents[ 0 ];

				var parent = c.Parent;
				if( parent != null )
				{
					int index = parent.Components.IndexOf( c );
					if( index >= 0 && index < parent.Components.Count - 1 )
					{
						component = c;
						return true;
					}
				}
			}
			component = null;
			return false;
		}

		void TryMoveUp()
		{
			if( !CanMoveUp( out Component component ) )
				return;

			//SelectItems(
			//public void SelectItems( Item[] items, bool expandNodes = false )

			var parent = component.Parent;
			int oldIndex = parent.Components.IndexOf( component );
			int newIndex = oldIndex - 1;

			parent.Components.MoveTo( component, newIndex );

			var action = new UndoActionComponentMove( DocumentWindow.Document, component, parent, oldIndex );
			DocumentWindow.Document.UndoSystem.CommitAction( action );
			DocumentWindow.Document.Modified = true;

			//select item
			{
				var item = FindItemByContainedObject( component );
				if( item != null )
					SelectItems( new Item[] { item } );
			}
		}

		void TryMoveDown()
		{
			if( !CanMoveDown( out Component component ) )
				return;

			var parent = component.Parent;
			int oldIndex = parent.Components.IndexOf( component );
			int newIndex = oldIndex + 1;
			parent.Components.MoveTo( component, newIndex );

			var action = new UndoActionComponentMove( DocumentWindow.Document, component, parent, oldIndex );
			DocumentWindow.Document.UndoSystem.CommitAction( action );
			DocumentWindow.Document.Modified = true;

			//select item
			{
				var item = FindItemByContainedObject( component );
				if( item != null )
					SelectItems( new Item[] { item } );
			}
		}

		public Item FindItemByContainedObject( object containedObject )
		{
			//!!!!slowly

			foreach( var item in GetAllItems() )
			{
				if( item.ContainedObject == containedObject )
					return item;
			}
			return null;
		}

		[DefaultValue( true )]
		public bool ShowToolBar
		{
			get { return showToolBar; }
			set { showToolBar = value; }
		}

		//!!!!
		//private static NodePosition DropTargetLocationToNodePosition( DropTargetLocation location )
		//{
		//	if( location == DropTargetLocation.AboveItem )
		//		return NodePosition.Before;
		//	else if( location == DropTargetLocation.BelowItem )
		//		return NodePosition.After;
		//	else
		//		return NodePosition.Inside;
		//}

		internal static DragDropItemData GetDroppingItemData( IDataObject data )
		{
			if( data is EngineListView.DragData listViewData )
			{
				// item from list
				var listItem = listViewData.Items[ 0 ];
				var item = (Item)listItem.Tag;
				return new DragDropItemData() { Item = item };
			}
			else
			{
				// item from tree
				return (DragDropItemData)data.GetData( typeof( DragDropItemData ) );
			}
		}

		internal static DragDropSetReferenceData GetDroppingRefData( IDataObject data )
		{
			return (DragDropSetReferenceData)data.GetData( typeof( DragDropSetReferenceData ) );
		}

		//!!!!
		//// can drop to list
		//private void listView_CanDrop( object sender, OlvDropEventArgs e )
		//{
		//if( e.DropTargetItem == null )
		//	return;

		//var listItem = (ListItem)e.DropTargetItem.RowObject;
		//var targetItem = listItem.Item;
		//if( targetItem == null )
		//	return;

		//if( !targetItem.CanDoDragDrop() )
		//	return;
		//PreloadNode( targetItem );

		//var nodePosition = DropTargetLocationToNodePosition( e.DropTargetLocation );

		//var droppingRefData = GetDroppingRefData( e.DragEventArgs.Data );
		//if( droppingRefData != null )
		//{
		//	if( DragDropSetReferenceFromReferenceButton( droppingRefData, targetItem, nodePosition, true ) )
		//		e.Effect = DragDropEffects.Link;
		//}
		//else
		//{
		//	var droppingItemData = GetDroppingItemData( e.DragEventArgs.Data );
		//	e.Effect = DragDropItemFromContentBrowser( droppingItemData, targetItem, nodePosition, true, out Component _ );
		//}
		//}

		//!!!!
		//// drop to list
		//private void listView_Dropped( object sender, OlvDropEventArgs e )
		//{
		//var listItem = (ListItem)e.DropTargetItem.RowObject;
		//var targetItem = listItem.Item;
		//if( targetItem == null )
		//	return;

		//var nodePosition = DropTargetLocationToNodePosition( e.DropTargetLocation );

		//var droppingRefData = GetDroppingRefData( e.DragEventArgs.Data );
		//if( droppingRefData != null )
		//{
		//	DragDropSetReferenceFromReferenceButton( droppingRefData, targetItem, nodePosition, false );
		//}
		//else
		//{
		//	var droppingItemData = GetDroppingItemData( e.DragEventArgs.Data );
		//	DragDropItemFromContentBrowser( droppingItemData, targetItem, nodePosition, false, out Component _ );
		//}

		//UpdateList();
		//}

		// can drop to tree
		private void treeView_DragOver( object sender, DragEventArgs e )
		{
			e.Effect = DragDropEffects.None;
			//!!!!
			//-Drag and drop: Загрузка ресурса при подведении на _File. Лучше грузить с задержкой, чтобы всё подряд не грузилось.
			//-Drag and drop: Раскрывать пункты в дереве при наведении. Видать с задержкой. Где еще подобное?

			var node = GetItemTreeNode( treeView.GetNodeAt( treeView.PointToClient( new Point( e.X, e.Y ) ) ) );
			var targetNode = GetItemTreeNode( treeView.DropPosition.Node );

			Debug.Assert( node == targetNode );

			var targetItem = GetItemByNode( node );
			if( targetItem == null )
				return;

			PreloadNode( targetItem );

			var droppingRefData = GetDroppingRefData( e.Data );
			if( droppingRefData != null )
			{
				if( DragDropSetReferenceFromReferenceButton( droppingRefData, targetNode.item, treeView.DropPosition.Position, true ) )
					e.Effect = DragDropEffects.Link;
			}
			else
			{
				var droppingItemData = GetDroppingItemData( e.Data );
				var r = DragDropItemFromContentBrowser( droppingItemData, targetItem, treeView.DropPosition.Position, true, out Component _ );
				if( r != DragDropEffects.None )
					e.Effect = r;
			}

			//if( DragDropItemFromContentBrowser( e, true ) )
			//	e.Effect = DragDropEffects.Link;

			//update HighlightDropPosition
			treeView.HighlightDropPosition = e.Effect != DragDropEffects.None;
		}

		// drop to tree
		private void treeView_DragDrop( object sender, DragEventArgs e )
		{
			var targetNode = GetItemTreeNode( treeView.DropPosition.Node );
			var targetItem = GetItemByNode( targetNode );

			var droppingRefData = GetDroppingRefData( e.Data );
			if( droppingRefData != null )
			{
				DragDropSetReferenceFromReferenceButton( droppingRefData, targetItem, treeView.DropPosition.Position, false );
			}
			else
			{
				var droppingItem = GetDroppingItemData( e.Data );
				DragDropItemFromContentBrowser( droppingItem, targetItem, treeView.DropPosition.Position, false, out Component newObject );

				//select
				if( newObject != null )
				{
					var newItem = FindItemByContainedObject( newObject );
					if( newItem != null )
						SelectItems( new Item[] { newItem }, considerAsSelectedByUser: true );
					treeView.Focus();
				}
			}
		}

		// drag drop process from tree
		private void treeView_ItemDrag( object sender, ItemDragEventArgs e )
		{
			if( ReadOnlyHierarchy )
				return;

			//!!!!!multiselection. drop
			ItemTreeNode itemNode = null;
			{
				var nodes = e.Item as TreeNodeAdv[];
				if( nodes != null && nodes.Length == 1 )
					itemNode = GetItemTreeNode( nodes[ 0 ] );
			}

			//var itemNode = e.Item as ItemTreeNode;
			if( itemNode != null )
			{
				var item = itemNode.item;

				if( item.CanDoDragDrop() )
				{
					PreloadNode( item );
					DoDragDrop( new DragDropItemData() { Item = item }, DragDropEffects.Link );
				}
			}
		}

		bool DragDropSetReferenceFromReferenceButton( DragDropSetReferenceData dragDropData, Item targetItem, NodePosition nodePosition, bool checkOnly )
		{
			if( dragDropData != null && nodePosition == NodePosition.Inside )
			{
				if( targetItem != null )
				{
					string[] referenceValues = new string[ dragDropData.controlledComponents.Length ];
					for( int n = 0; n < dragDropData.controlledComponents.Length; n++ )
					{
						targetItem.CalculateReferenceValue( dragDropData.controlledComponents[ n ], dragDropData.property.TypeUnreferenced, out string referenceValue, out bool canSet );
						referenceValues[ n ] = referenceValue;
						if( !canSet )
						{
							referenceValues = null;
							break;
						}
					}

					if( referenceValues != null )
					{
						if( !checkOnly )
						{
							dragDropData.SetProperty( referenceValues );

							//!!!!активировать окно?
						}

						return true;
					}
				}
			}

			return false;
		}

		DragDropEffects DragDropItemFromContentBrowser( DragDropItemData droppingItemData, Item targetItem, NodePosition nodePosition, bool checkOnly, out Component newObject )
		{
			newObject = null;

			var droppingItem = droppingItemData.Item;

			if( droppingItem == null || targetItem == null || droppingItem == targetItem )
				return DragDropEffects.None;

			//operations with components
			//!!!!
			//if( nodePosition == NodePosition.Before || nodePosition == NodePosition.After )
			{
				//get source data. type, component
				Metadata.TypeInfo droppingObjectType = null;
				Component droppingComponent = null;
				{
					//_Type
					var typeItem = droppingItem as ContentBrowserItem_Type;
					if( typeItem != null )
					{
						var type = typeItem.Type;

						//!!!!генериковому нужно указать типы

						if( MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( type ) && !type.Abstract )
							droppingObjectType = type;
					}

					//_File
					var fileItem = droppingItem as ContentBrowserItem_File;
					if( fileItem != null && !fileItem.IsDirectory )
					{
						//!!!!не делать предпросмотр для карты, т.к. долго. что еще?
						var ext = Path.GetExtension( fileItem.FullPath );
						if( ResourceManager.GetTypeByFileExtension( ext ) != null )
						{
							var res = ResourceManager.GetByName( VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath ) );
							var type = res?.PrimaryInstance?.ResultComponent?.GetProvidedType();
							if( type != null )
								droppingObjectType = type;
						}
					}

					//_Component
					var componentItem = droppingItem as ContentBrowserItem_Component;
					if( componentItem != null && componentItem.Component != null )
					{
						var component = componentItem.Component;

						if( ComponentUtility.GetResourceInstanceByComponent( component )?.InstanceType == Resource.InstanceType.SeparateInstance )
							droppingComponent = component;
						else
							droppingObjectType = component.GetProvidedType();
					}
				}

				if( droppingObjectType != null || droppingComponent != null )
				{
					//!!!!
					//Mode == ModeEnum.Objects

					//get target component
					Component targetComponent = null;
					{
						if( targetItem != null )
						{
							var targetItemComponent = targetItem as ContentBrowserItem_Component;
							if( targetItemComponent != null )
								targetComponent = targetItemComponent.Component;
						}
					}

					if( targetComponent != null && DocumentWindow?.Document != null )
					{
						if( nodePosition == NodePosition.Inside )
						{
							//add child to targetComponent

							if( droppingObjectType != null )
							{
								if( !checkOnly )
								{
									//create
									newObject = targetComponent.CreateComponent( droppingObjectType, -1, false );
									newObject.Name = ComponentUtility.GetNewObjectUniqueName( newObject );
									newObject.NewObjectSetDefaultConfiguration();
									newObject.Enabled = true;

									//add to undo list
									var newObjects = new List<Component>();
									newObjects.Add( newObject );
									var action = new UndoActionComponentCreateDelete( DocumentWindow.Document, newObjects, true );
									DocumentWindow.Document.UndoSystem.CommitAction( action );
									DocumentWindow.Document.Modified = true;
								}

								return DragDropEffects.Link;
							}

							if( droppingComponent != null && !ComponentUtility.IsChildInHierarchy( droppingComponent, targetComponent ) )
							{
								bool makeClone = ( ModifierKeys.HasFlag( Keys.Shift ) || ModifierKeys.HasFlag( Keys.Control ) ) || droppingComponent.ParentRoot != targetComponent.ParentRoot || droppingComponent.Parent == null;

								if( !checkOnly )
								{
									string text;
									if( makeClone )
										text = Translate( "Make copy of the object?" );
									else
										text = Translate( "Move this object to the new place?" );
									//if( makeClone )
									//	text = "Are you sure you want to make copy of the object?";
									//else
									//	text = "Are you sure you want to move this object to the new place?";

									if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
									{
										if( makeClone )
										{
											//create
											newObject = droppingComponent.Clone();
											newObject.Name = ComponentUtility.GetNewObjectUniqueName( newObject );
											newObject.NewObjectSetDefaultConfiguration();
											targetComponent.AddComponent( newObject );

											//add to undo list
											var newObjects = new List<Component>();
											newObjects.Add( newObject );
											var action = new UndoActionComponentCreateDelete( DocumentWindow.Document, newObjects, true );
											DocumentWindow.Document.CommitUndoAction( action );
										}
										else
										{
											//change parent

											var oldParent = droppingComponent.Parent;
											int oldIndex = oldParent.Components.IndexOf( droppingComponent );

											droppingComponent.Parent.RemoveComponent( droppingComponent, false );
											targetComponent.AddComponent( droppingComponent );

											var action = new UndoActionComponentMove( DocumentWindow.Document, droppingComponent, oldParent, oldIndex );
											DocumentWindow.Document.CommitUndoAction( action );
										}
									}
								}

								return DragDropEffects.Link;
							}
						}
						else
						{
							//add before or after targetComponent

							var parent = targetComponent.Parent;
							if( parent != null )
							{
								if( droppingObjectType != null )
								{
									if( !checkOnly )
									{
										int insertIndex = -1;
										var index = parent.Components.IndexOf( targetComponent );
										if( index != -1 )
										{
											insertIndex = index;
											if( nodePosition == NodePosition.After )
												insertIndex++;
										}

										//create
										newObject = targetComponent.Parent.CreateComponent( droppingObjectType, insertIndex, false );
										newObject.Name = ComponentUtility.GetNewObjectUniqueName( newObject );
										newObject.NewObjectSetDefaultConfiguration();
										newObject.Enabled = true;

										//add to undo list
										var newObjects = new List<Component>();
										newObjects.Add( newObject );
										var action = new UndoActionComponentCreateDelete( DocumentWindow.Document, newObjects, true );
										DocumentWindow.Document.UndoSystem.CommitAction( action );
										DocumentWindow.Document.Modified = true;
									}

									return DragDropEffects.Link;
								}

								if( droppingComponent != null && !ComponentUtility.IsChildInHierarchy( droppingComponent, targetComponent ) )
								{
									bool makeClone = ( ModifierKeys.HasFlag( Keys.Shift ) || ModifierKeys.HasFlag( Keys.Control ) ) || droppingComponent.ParentRoot != targetComponent.ParentRoot || droppingComponent.Parent == null;

									bool skip = false;
									if( !makeClone )
									{
										if( droppingComponent.Parent == parent )
										{
											var i1 = parent.Components.IndexOf( targetComponent );
											var i2 = parent.Components.IndexOf( droppingComponent );
											if( i1 == i2 )
												skip = true;
											if( nodePosition == NodePosition.Before && i1 - 1 == i2 )
												skip = true;
											if( nodePosition == NodePosition.After && i1 + 1 == i2 )
												skip = true;
										}
									}

									if( !skip )
									{
										if( !checkOnly )
										{
											string text;
											if( makeClone )
												text = Translate( "Make copy of the object?" );
											else
												text = Translate( "Move this object to the new place?" );
											//if( makeClone )
											//	text = "Are you sure you want to make copy of the object?";
											//else
											//	text = "Are you sure you want to move this object to the new place?";

											if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
											{
												int insertIndex = -1;
												var index = parent.Components.IndexOf( targetComponent );
												if( index != -1 )
												{
													insertIndex = index;
													if( nodePosition == NodePosition.After )
														insertIndex++;
												}

												if( makeClone )
												{
													//create
													newObject = droppingComponent.Clone();
													newObject.Name = ComponentUtility.GetNewObjectUniqueName( newObject );
													newObject.NewObjectSetDefaultConfiguration();
													targetComponent.Parent.AddComponent( newObject, insertIndex );

													//add to undo list
													var newObjects = new List<Component>();
													newObjects.Add( newObject );
													var action = new UndoActionComponentCreateDelete( DocumentWindow.Document, newObjects, true );
													DocumentWindow.Document.CommitUndoAction( action );
												}
												else
												{
													//change parent

													var oldParent = droppingComponent.Parent;
													int oldIndex = oldParent.Components.IndexOf( droppingComponent );

													int insertIndexFixed = insertIndex;
													if( targetComponent.Parent == oldParent && insertIndexFixed > oldIndex )
														insertIndexFixed--;

													droppingComponent.Parent.RemoveComponent( droppingComponent, false );
													targetComponent.Parent.AddComponent( droppingComponent, insertIndexFixed );

													var action = new UndoActionComponentMove( DocumentWindow.Document, droppingComponent, oldParent, oldIndex );
													DocumentWindow.Document.CommitUndoAction( action );
												}
											}
										}

										return DragDropEffects.Link;
									}
								}
							}
						}
					}
				}
			}

			//file operations
			if( treeView.DropPosition.Position == NodePosition.Inside )
			{
				//get source data
				var droppingFileItem = droppingItem as ContentBrowserItem_File;
				if( droppingFileItem != null )
				{
					//get target component
					string targetFolder = null;
					{
						if( targetItem != null )
						{
							var targetItemFile = targetItem as ContentBrowserItem_File;
							if( targetItemFile != null && targetItemFile.IsDirectory )
								targetFolder = targetItemFile.FullPath;
						}
					}

					//!!!!помимо себя в себя нужно вложенные проверять еще
					if( !string.IsNullOrEmpty( targetFolder ) && droppingFileItem.FullPath != targetFolder )
					{
						bool copy = ModifierKeys.HasFlag( Keys.Shift ) || ModifierKeys.HasFlag( Keys.Control );

						bool skip = false;

						if( !skip )
						{
							if( !checkOnly )
							{
								string text;
								if( copy )
									text = Translate( "Copy files?" );
								else
									text = Translate( "Move files?" );

								if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
								{
									//!!!!multiselection
									CutCopyFiles( new string[] { droppingFileItem.FullPath }, !copy, targetFolder );
								}
							}

							//!!!!Copy, Move не работают
							return DragDropEffects.Link;
							//if( copy )
							//	return DragDropEffects.Copy;
							//else
							//	return DragDropEffects.Move;
							//return true;
						}
					}
				}
			}

			return DragDropEffects.None;
		}

		private void toolStripButtonShowMembers_Click( object sender, EventArgs e )
		{
			//!!!!в опции "показывать кнопку"

			toolStripButtonShowMembers.Checked = !toolStripButtonShowMembers.Checked;

			if( Initialized )
				UpdateData();
		}

		//!!!!в опции?
		[Browsable( false )]
		public bool ShowMembers
		{
			get
			{
				//!!!!
				if( Mode == ModeEnum.Objects )
					return toolStripButtonShowMembers.Checked;
				else
					return true;
			}
		}

		private void treeView_Enter( object sender, EventArgs e )
		{
			//!!!!treeView_Enter? может родителя?

			if( CanSelectObjectSettings )
				SettingsWindowSelectObjects();
		}

		[Browsable( false )]
		public ContentBrowserFilteringMode FilteringMode
		{
			get { return filteringMode; }
			set
			{
				if( filteringMode == value )
					return;
				filteringMode = value;

				if( loaded )
				{
					//!!!!лучше как срез обновлять, т.е. основной список уже есть. типа как search.
					UpdateData();
				}
			}
		}

		//void InitFilteringModes()
		//{
		//	var allToolStripMenuItem = new ToolStripMenuItem();
		//	allToolStripMenuItem.Checked = true;
		//	allToolStripMenuItem.Name = "allToolStripMenuItem";
		//	allToolStripMenuItem.Text = EditorLocalization.Translate( "ContentBrowser.FilteringMode", "All" );
		//	allToolStripMenuItem.Click += allToolStripMenuItem_Click;
		//	toolStripDropDownButtonFilteringMode.DropDownItems.Add( allToolStripMenuItem );

		//	foreach( var mode in filteringModes )
		//	{
		//		var item = new ToolStripMenuItem();
		//		item.Text = EditorLocalization.Translate( "ContentBrowser.FilteringMode", mode.Name );
		//		item.Tag = mode;
		//		item.Click += ToolStripMenuItemFilteringModeItem_Click;
		//		toolStripDropDownButtonFilteringMode.DropDownItems.Add( item );
		//	}
		//}

		//private void allToolStripMenuItem_Click( object sender, EventArgs e )
		//{
		//	FilteringMode = null;
		//}

		//private void ToolStripMenuItemFilteringModeItem_Click( object sender, EventArgs e )
		//{
		//	FilteringMode = (ContentBrowserFilteringMode)( (ToolStripMenuItem)sender ).Tag;
		//}

		ItemTreeNode GetItemTreeNode( TreeNodeAdv treeViewNode )
		{
			if( treeViewNode != null )
				return treeViewNode.Tag as ItemTreeNode;
			return null;
		}

		TreeNodeAdv FindTreeViewNode( Node findNode )
		{
			if( findNode == null )
				Log.Fatal( "ContentBrowser: FindTreeViewNode: findNode == null." );

			//!!!!slowly

			foreach( var treeViewNode in treeView.AllNodes )
			{
				var node = treeViewNode.Tag as Node;
				if( node == findNode )
					return treeViewNode;
			}

			Log.Fatal( "ContentBrowser: FindTreeViewNode: Internal error: Node is not found." );
			return null;
		}

		ICollection<TreeNodeAdv> FindTreeViewNodes( ICollection<Node> findNodes )
		{
			//!!!!slowly

			var setOfNodes = new ESet<Node>( findNodes );
			var result = new List<TreeNodeAdv>();

			foreach( var treeViewNode in treeView.AllNodes )
			{
				var node = treeViewNode.Tag as Node;
				if( node != null && setOfNodes.Contains( node ) )
					result.Add( treeViewNode );
			}

			return result;
		}

		void CutCopy( bool cut )
		{
			var selectedItems = SelectedItems;

			//_File
			if( selectedItems.Length != 0 && selectedItems.All( item => item is ContentBrowserItem_File ) )
			{
				List<string> list = new List<string>();
				foreach( var item in selectedItems )
				{
					var fileItem = (ContentBrowserItem_File)item;
					list.Add( fileItem.FullPath );
				}

				//#if BCL_CLIPBOARD
				IDataObject dataObject = new DataObject( DataFormats.FileDrop, list.ToArray() );
				MemoryStream stream = new MemoryStream();
				stream.Write( new byte[] { (byte)( cut ? 2 : 5 ), 0, 0, 0 }, 0, 4 );
				stream.SetLength( 4 );
				dataObject.SetData( "Preferred DropEffect", stream );
				Clipboard.SetDataObject( dataObject );
				//#else
				//				WindowsClipboard.SetFileDropPaths( list, cut, EditorForm.Instance.Handle );
				//#endif
			}

			//_Component
			if( selectedItems.Length != 0 && selectedItems.All( item => item is ContentBrowserItem_Component ) )
			{
				var data = new ObjectCutCopyPasteData( DocumentWindow, cut, GetSelectedComponents().ToArray() );
				ClipboardManager.CopyToClipboard( data );
			}
		}

		public bool CanCut()
		{
			//_File
			var selectedItems = SelectedItems;
			if( selectedItems.Length != 0 && selectedItems.All( item => item is ContentBrowserItem_File ) )
				return true;

			//_Component
			if( selectedItems.Length != 0 && selectedItems.All( item => item is ContentBrowserItem_Component ) )
			{
				if( GetSelectedComponents().All( c => c.Parent != null && EditorAPI.GetDocumentByObject( c ) != null ) )
					return true;
			}

			return false;
		}

		public void Cut()
		{
			if( CanCut() )
				CutCopy( true );
		}

		public bool CanCopy()
		{
			//_File
			var selectedItems = SelectedItems;
			if( selectedItems.Length != 0 && selectedItems.All( item => item is ContentBrowserItem_File ) )
				return true;

			//_Component
			if( selectedItems.Length != 0 && selectedItems.All( item => item is ContentBrowserItem_Component ) )
				return true;

			return false;
		}

		public void Copy()
		{
			if( CanCopy() )
				CutCopy( false );
		}

		public bool CanPaste( out string[] filePaths, out bool cut, out string destinationFolder, out Component destinationParent )
		{
			filePaths = null;
			cut = false;
			destinationFolder = null;
			destinationParent = null;

			var selectedItems = SelectedItems;
			if( selectedItems.Length == 1 )
			{
				//_File
				var fileItem = selectedItems[ 0 ] as ContentBrowserItem_File;
				if( fileItem != null )//&& fileItem.IsDirectory )
				{
					if( !fileItem.IsDirectory )
						fileItem = fileItem.Parent as ContentBrowserItem_File;
					if( fileItem != null && fileItem.IsDirectory )
					{
						// currently we have 0xc0000409 crush with BCL Clipboard class
						// https://developercommunity.visualstudio.com/content/problem/395979/application-is-crashed-with-the-0xc0000409-excepti.html
#if BCL_CLIPBOARD
						IDataObject data = null;
						try
						{
							data = Clipboard.GetDataObject();
						}
						catch { }
						if( data != null )
						{
							MemoryStream stream = data.GetData( "Preferred DropEffect" ) as MemoryStream;
							if( stream != null )
							{
								int flag = stream.ReadByte();
								if( flag == 2 || flag == 5 )
								{
									filePaths = (string[])data.GetData( DataFormats.FileDrop );
									cut = flag == 2;
									destinationFolder = fileItem.FullPath;
									return true;
								}
							}
						}
#else
						var paths = WindowsClipboard.GetFileDropPaths( EditorForm.Instance.Handle );
						if( paths.Count > 0 )
						{
							filePaths = paths.ToArray();
							cut = WindowsClipboard.IsCutPrefferdDropEffect( EditorForm.Instance.Handle );
							destinationFolder = fileItem.FullPath;
							return true;
						}
#endif
					}
				}

				//_Component
				var componentItem = selectedItems[ 0 ] as ContentBrowserItem_Component;
				if( componentItem != null && !readOnlyHierarchy && EditorAPI.GetDocumentByObject( componentItem.Component ) != null )
				{
					if( ClipboardManager.CheckAvailableInClipboard<ObjectCutCopyPasteData>() )
					{
						destinationParent = componentItem.Component;
						return true;
					}
				}
			}

			return false;
		}

		static void CopyRealFileDirectoryRecursive( string sourceDirectory, string destDirectory )
		{
			string[] directories = Directory.GetDirectories( sourceDirectory );
			Directory.CreateDirectory( destDirectory );
			foreach( string childDirectory in directories )
				CopyRealFileDirectoryRecursive( childDirectory, Path.Combine( destDirectory, Path.GetFileName( childDirectory ) ) );
			foreach( string fileName in Directory.GetFiles( sourceDirectory ) )
				File.Copy( fileName, Path.Combine( destDirectory, Path.GetFileName( fileName ) ) );
		}

		void CutCopyFiles( string[] filePaths, bool cut, string destinationFolder )
		{
			//!!!!потом заимплементить, чтобы было с диалогом виндовса.

			foreach( string realFilePath in filePaths )
			{
				try
				{
					string newRealFilePath = Path.Combine( destinationFolder, Path.GetFileName( realFilePath ) );

					//rename if put to same directory and file already exists
					if( File.Exists( realFilePath ) )
					{
						if( string.Compare( destinationFolder, Path.GetDirectoryName( realFilePath ), true ) == 0 )
						{
							for( int counter = 1; ; counter++ )
							{
								string checkRealFilePath = destinationFolder + Path.DirectorySeparatorChar;
								checkRealFilePath += Path.GetFileNameWithoutExtension( realFilePath );
								if( counter != 1 )
									checkRealFilePath += counter.ToString();
								if( Path.GetExtension( realFilePath ) != null )
									checkRealFilePath += Path.GetExtension( realFilePath );

								//file
								if( !File.Exists( checkRealFilePath ) )
								{
									newRealFilePath = checkRealFilePath;
									break;
								}
							}
						}

						//create
						if( cut )
							File.Move( realFilePath, newRealFilePath );
						else
							File.Copy( realFilePath, newRealFilePath );

						continue;
					}

					//rename if put to same directory and directory already exists
					if( Directory.Exists( realFilePath ) )
					{
						for( int counter = 1; ; counter++ )
						{
							string checkRealFilePath = Path.GetDirectoryName( newRealFilePath ) + Path.DirectorySeparatorChar;
							checkRealFilePath += Path.GetFileName( newRealFilePath );
							if( counter != 1 )
								checkRealFilePath += counter.ToString();

							//directory
							if( !Directory.Exists( checkRealFilePath ) )
							{
								newRealFilePath = checkRealFilePath;
								break;
							}
						}

						//create
						CopyRealFileDirectoryRecursive( realFilePath, newRealFilePath );
						if( cut )
							Directory.Delete( realFilePath, true );

						continue;
					}
				}
				catch( Exception e )
				{
					Log.Error( e.Message );
					return;
				}
			}
		}

		public void Paste()
		{
			if( CanPaste( out string[] filePaths, out bool cut, out string destinationFolder, out Component destinationParent ) )
			{
				//_File
				if( filePaths != null )
					CutCopyFiles( filePaths, cut, destinationFolder );

				//_Component
				if( destinationParent != null )
				{
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

						foreach( var c in components )
						{
							var cloned = c.Clone();
							destinationParent.AddComponent( cloned );

							newObjects.Add( cloned );
						}

						if( data.cut )
						{
							//cut
							if( data.documentWindow.Document != DocumentWindow.Document )
							{
								//another document
								{
									var action = new UndoActionComponentCreateDelete( data.documentWindow.Document, components, false );
									data.documentWindow.Document.UndoSystem.CommitAction( action );
									data.documentWindow.Document.Modified = true;
								}
								{
									var action = new UndoActionComponentCreateDelete( DocumentWindow.Document, newObjects, true );
									DocumentWindow.Document.UndoSystem.CommitAction( action );
									DocumentWindow.Document.Modified = true;
								}
							}
							else
							{
								//same document
								var multiAction = new UndoMultiAction();
								multiAction.AddAction( new UndoActionComponentCreateDelete( DocumentWindow.Document, components, false ) );
								multiAction.AddAction( new UndoActionComponentCreateDelete( DocumentWindow.Document, newObjects, true ) );
								DocumentWindow.Document.UndoSystem.CommitAction( multiAction );
								DocumentWindow.Document.Modified = true;
							}
						}
						else
						{
							//copy
							var action = new UndoActionComponentCreateDelete( DocumentWindow.Document, newObjects, true );
							DocumentWindow.Document.UndoSystem.CommitAction( action );
							DocumentWindow.Document.Modified = true;
						}

						//select items
						if( newObjects.Count == 1 )
						{
							var itemParent = FindItemByContainedObject( destinationParent );
							if( itemParent != null )
							{
								//expand parent
								SelectItems( new Item[] { itemParent }, true );

								var item = FindItemByContainedObject( newObjects[ 0 ] );
								if( item != null )
									SelectItems( new Item[] { item }, false, true );
							}
						}
					}
				}
			}
		}

		bool CanRename()
		{
			var selectedItems = SelectedItems;
			if( selectedItems.Length == 1 )
			{
				//_File
				var fileItem = selectedItems[ 0 ] as ContentBrowserItem_File;
				if( fileItem != null )
					return true;

				//_Component
				var componentItem = selectedItems[ 0 ] as ContentBrowserItem_Component;
				if( componentItem != null && documentWindow != null && documentWindow.Document != null && EditorAPI.GetDocumentByObject( componentItem.Component ) != null )
					return true;
			}

			return false;
		}

		void RenameBegin()
		{
			if( CanRename() )
			{
				//!!!!treeView label edit is not supported.

				var item = SelectedItems[ 0 ];

				//_File
				var fileItem = item as ContentBrowserItem_File;
				if( fileItem != null )
				{
					var folder = Path.GetDirectoryName( fileItem.FullPath );

					var form = new OKCancelTextBoxForm( Translate( "Name" ) + ":", Path.GetFileName( fileItem.FullPath ), Translate( "Rename" ),
						delegate ( string text, ref string error )
						{
							if( string.IsNullOrEmpty( text ) )
							{
								error = Translate( "The name is not specified." );
								return false;
							}

							if( text.IndexOfAny( Path.GetInvalidPathChars() ) != -1 )
							{
								error = Translate( "Invalid file path." );
								return false;
							}

							try
							{
								if( text.Contains( '\\' ) || text.Contains( '/' ) )
								{
									if( Path.GetFileName( text ).IndexOfAny( Path.GetInvalidFileNameChars() ) != -1 )
									{
										error = Translate( "Invalid file path." );
										return false;
									}
								}
							}
							catch { }

							var newPath = Path.Combine( folder, text );

							if( fileItem.FullPath == newPath )
							{
								error = Translate( "Same name." );
								return false;
							}

							if( ( File.Exists( newPath ) || Directory.Exists( newPath ) ) && string.Compare( fileItem.FullPath, newPath, true ) != 0 )
							{
								error = Translate( "A file or folder with same name already exists." );
								return false;
							}

							return true;
						},
						delegate ( string text, ref string error )
						{
							var newPath = Path.Combine( folder, text );

							try
							{
								if( string.Compare( fileItem.FullPath, newPath, true ) == 0 )
								{
									//rename when only case changed
									if( fileItem.IsDirectory )
									{
										var tempName = newPath + "_";
										while( Directory.Exists( tempName ) )
											tempName += "_";
										Directory.Move( fileItem.FullPath, tempName );
										Directory.Move( tempName, newPath );
									}
									else
									{
										var tempName = newPath + "_";
										while( File.Exists( tempName ) )
											tempName += "_";
										File.Move( fileItem.FullPath, tempName );
										File.Move( tempName, newPath );
									}
								}
								else
								{
									//default rename
									if( fileItem.IsDirectory )
										Directory.Move( fileItem.FullPath, newPath );
									else
										File.Move( fileItem.FullPath, newPath );
								}
							}
							catch( Exception e )
							{
								error = e.Message;
								return false;
							}

							return true;
						}
					);

					form.ShowDialog();
				}

				//_Component
				var componentItem = item as ContentBrowserItem_Component;
				if( componentItem != null )
				{
					var component = componentItem.Component;
					EditorUtility.ShowRenameComponentDialog( component );
				}
			}
		}

		public string GetDirectoryPathOfSelectedFileOrParentDirectoryItem()
		{
			ContentBrowserItem_File currentOrParentDirectoryItem = null;

			//!!!!multiselection. если выделены все на одном уровне.

			var selectedItems = SelectedItems;
			if( selectedItems.Length == 1 )
			{
				var item = selectedItems[ 0 ];

				var fileItem = item as ContentBrowserItem_File;
				if( fileItem != null )
				{
					if( !fileItem.IsDirectory )
					{
						var parent = fileItem.Parent as ContentBrowserItem_File;
						if( parent != null && parent.IsDirectory )
							currentOrParentDirectoryItem = parent;
					}
					else
						currentOrParentDirectoryItem = fileItem;
				}
			}

			if( currentOrParentDirectoryItem != null && VirtualPathUtility.GetVirtualPathByReal( currentOrParentDirectoryItem.FullPath, out string dummy ) )
				return currentOrParentDirectoryItem.FullPath;
			return null;
		}

		bool CanNewFolder( out string directoryPath )
		{
			directoryPath = GetDirectoryPathOfSelectedFileOrParentDirectoryItem();
			return !string.IsNullOrEmpty( directoryPath );
		}

		void NewFolder()
		{
			if( CanNewFolder( out string directoryPath ) )
			{
				var form = new OKCancelTextBoxForm( Translate( "Name" ) + ":", Translate( "New Folder" ), Translate( "New Folder" ),
					delegate ( string text, ref string error )
					{
						if( string.IsNullOrEmpty( text ) )
						{
							error = Translate( "The name is not specified." );
							return false;
						}

						if( text.IndexOfAny( Path.GetInvalidPathChars() ) != -1 )
						{
							error = Translate( "Invalid file path." );
							return false;
						}

						try
						{
							if( text.Contains( '\\' ) || text.Contains( '/' ) )
							{
								if( Path.GetFileName( text ).IndexOfAny( Path.GetInvalidFileNameChars() ) != -1 )
								{
									error = Translate( "Invalid file path." );
									return false;
								}
							}
						}
						catch { }

						var path = Path.Combine( directoryPath, text );
						if( File.Exists( path ) || Directory.Exists( path ) )
						{
							error = Translate( "A file or folder with same name already exists." );
							return false;
						}

						return true;
					},
					delegate ( string text, ref string error )
					{
						var path = Path.Combine( directoryPath, text );

						try
						{
							Directory.CreateDirectory( path );
						}
						catch( Exception e )
						{
							error = e.Message;
							return false;
						}

						//!!!!выделить итем

						return true;
					}
				);

				form.ShowDialog();
			}
		}

		bool CanNewResource( out string directoryPath )
		{
			directoryPath = GetDirectoryPathOfSelectedFileOrParentDirectoryItem();
			return !string.IsNullOrEmpty( directoryPath );
		}

		void NewResource( Metadata.TypeInfo lockType )
		{
			if( CanNewResource( out string directoryPath ) )
			{
				var initData = new NewObjectWindow.CreationDataClass();
				initData.initFileCreationDirectory = VirtualPathUtility.GetVirtualPathByReal( directoryPath );
				//initData.initDemandedType = MetadataManager.GetTypeOfNetType( typeof( Component ) );
				initData.createdFromContentBrowser = this;

				initData.initLockType = lockType;

				EditorAPI.OpenNewObjectWindow( initData );
			}
		}

		bool CanImportResource( out string directoryPath )
		{
			directoryPath = GetDirectoryPathOfSelectedFileOrParentDirectoryItem();
			return !string.IsNullOrEmpty( directoryPath );
		}

		void ImportResource()
		{
			if( CanImportResource( out string directoryPath ) )
				EditorAPI.OpenImportWindow( VirtualPathUtility.GetVirtualPathByReal( directoryPath ) );
		}

		private void nodeTextBox1_DrawText( object sender, Aga.Controls.Tree.NodeControls.DrawTextEventArgs e )
		{
			var itemNode = GetItemTreeNode( e.Node );
			if( itemNode != null )
			{
				var item = itemNode.item;
				if( item != null && item.ShowDisabledInHierarchy )
					e.TextColor = Color.Gray;
			}
		}

		public void TreeViewHideVScroll()
		{
			//можно внутри TreeViewAdv Enabled флаг проверять
			treeView.VScrollAlwaysHide = true;
		}

		public void NeedSelectFilesOrDirectories( string[] realPaths, bool expandNodes )
		{
			needSelectFiles = realPaths;
			needSelectFilesExpandNodes = expandNodes;
			needSelectFiles_LastUpdateTime = DateTime.Now;
		}

		void UpdateNeedSelectFiles()
		{
			//try select paths. but if doesn't exists then later.
			if( needSelectFiles != null )
			{
				if( ContentBrowserUtility.SelectFileItems( this, needSelectFiles, needSelectFilesExpandNodes ) )
					needSelectFiles = null;

				//List<string> later = new List<string>();
				//foreach( var realPath in needSelectFiles.ToArray() )
				//{
				//	if( !ContentBrowserUtils.SelectItemByRealFilePath( this, realPath ) )
				//		later.Add( realPath );
				//}
				//needSelectFiles.Clear();
				//needSelectFiles.AddRange( later );
			}

			//update needSelectFiles
			if( needSelectFiles != null )
			{
				var span = DateTime.Now - needSelectFiles_LastUpdateTime;
				if( span.TotalSeconds > 3 )
					needSelectFiles = null;
			}
		}

		[Browsable( false )]
		public ContentBrowserItem_File DataItem
		{
			get { return dataItem; }
		}

		//!!!!multiselection
		public bool CanOpenFile( out ContentBrowserItem_File fileItem )
		{
			if( SelectedItems.Length == 1 )
			{
				var fileItem2 = SelectedItems[ 0 ] as ContentBrowserItem_File;
				if( fileItem2 != null && !fileItem2.IsDirectory )
				{
					fileItem = fileItem2;
					return true;
				}
			}

			fileItem = null;
			return false;
		}

		private void toolStripButtonOpen_Click( object sender, EventArgs e )
		{
			if( !CanOpenFile( out var fileItem ) )
				return;

			bool handled = false;
			OpenFile( fileItem, ref handled );
		}

		public object[] GetSelectedContainedObjects()
		{
			List<object> result = new List<object>();
			foreach( var item in SelectedItems )
			{
				var obj = item.ContainedObject;
				if( obj != null )
					result.Add( obj );
			}
			return result.ToArray();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void UpdateList()
		{
			if( panelMode != PanelModeEnum.Tree )
			{
				////skip update
				//if( treeView.SelectedNodes.Count <= 1 )
				//{
				//	TreeNodeAdv rootNode = null;
				//	if( UseSelectedTreeNodeAsRootForList && treeView.SelectedNode != null )
				//		rootNode = treeView.SelectedNode;
				//	else
				//		rootNode = GetTreeRootOrFirstNode();

				//	if( rootNode != GetTreeRootOrFirstNode() )
				//	{
				//		if( GetTreeSelectedItems().Count == 1 )
				//		{
				//			var newItems = new List<ListItem>();

				//			var selectedItem = GetTreeSelectedItems()[ 0 ];
				//			foreach( var childItem in selectedItem.GetChildren( true ) )
				//			{
				//				var listItem = new ListItem( childItem );
				//				newItems.Add( listItem );
				//			}

				//			var current = new List<ListItem>();
				//			if( listView.Objects != null )
				//			{
				//				foreach( ListItem item in listView.Objects )
				//					current.Add( item );
				//			}

				//			bool needUpdate = false;
				//			if( current.Count == newItems.Count )
				//			{
				//				for( int n = 0; n < current.Count; n++ )
				//					if( current[ n ].Item != newItems[ n ].Item )
				//						needUpdate = true;
				//			}
				//			else
				//				needUpdate = true;

				//			if( !needUpdate )
				//				return;
				//		}
				//	}

				//}

				//var oldSelectedObjects = new List<Item>();
				//if( listView.SelectedObjects.Count != 0 )
				//{
				//	foreach( ListItem item in listView.SelectedObjects )
				//		oldSelectedObjects.Add( item.Item );
				//}

				//listView.SelectedObject = null;

				var listItems = new List<EngineListView.Item>();

				if( treeView.SelectedNodes.Count <= 1 )
				{
					TreeNodeAdv rootNode;
					if( UseSelectedTreeNodeAsRootForList && treeView.SelectedNode != null )
						rootNode = treeView.SelectedNode;
					else
						rootNode = GetTreeRootOrFirstNode();

					if( rootNode == GetTreeRootOrFirstNode() )
					{
						foreach( var treeItem in rootNode.Children )
						{
							var item = GetItemByNode( GetItemTreeNode( treeItem ) );

							var listItem = new EngineListView.Item( listView );
							listItem.Tag = item;
							listItem.Text = item.Text;
							listItem.Description = item.GetDescription();
							listItem.ShowTooltip = !string.IsNullOrEmpty( listItem.Description );

							listItems.Add( listItem );
						}
					}
					else
					{
						if( GetTreeSelectedItems().Count == 1 )
						{
							var selectedItem = GetTreeSelectedItems()[ 0 ];

							foreach( var childItem in selectedItem.GetChildren( true ) )
							{
								var listItem = new EngineListView.Item( listView );
								listItem.Tag = childItem;
								listItem.Text = childItem.Text;
								listItem.Description = childItem.GetDescription();
								listItem.ShowTooltip = !string.IsNullOrEmpty( listItem.Description );

								listItems.Add( listItem );
							}
						}
					}
				}

				//skip update
				{
					var current = new List<EngineListView.Item>();
					foreach( var item in listView.Items )
						current.Add( item );

					bool needUpdate = false;
					if( current.Count == listItems.Count )
					{
						for( int n = 0; n < current.Count; n++ )
							if( current[ n ].Tag != listItems[ n ].Tag )
								needUpdate = true;
					}
					else
						needUpdate = true;

					if( !needUpdate )
						return;
				}

				//!!!!
				listView.SelectedItem = null;

				////bool needUpdate = true;
				//bool needUpdate = false;
				//{
				//	var current = new List<ListItem>();
				//	if( listView.Objects != null )
				//	{
				//		foreach( ListItem item in listView.Objects )
				//			current.Add( item );
				//	}

				//	if( current.Count == listItems.Count )
				//	{
				//		for( int n = 0; n < current.Count; n++ )
				//		{
				//			if( current[ n ].Item != listItems[ n ].Item )
				//				needUpdate = true;
				//		}
				//	}
				//	else
				//		needUpdate = true;


				//	//needUpdate = true;
				//	//needUpdate = !listItems.SequenceEqual( current );
				//}

				//Log.Info( needUpdate.ToString() );

				//if( needUpdate )
				//{
				listView.SetItems( listItems );
				//}

				foreach( var listItem in listView.Items )
				{
					var item = GetItemByListViewItem( listItem );
					if( item != null )
						listItem.ShowDisabled = item.ShowDisabled;
				}

				UpdateListImages();

				//if( needUpdate )
				//{
				// select item in list.
				if( !UseSelectedTreeNodeAsRootForList )
				{
					var selected = GetItemTreeNode( treeView.SelectedNode );
					listView.SelectedItem = GetListItemByItem( selected?.item );
				}
				////!!!!new gg
				//else
				//{
				//	var listItemsToSelect = new List<ListItem>();
				//	foreach( var item in oldSelectedItems )
				//	{
				//		var listItem = GetListItemByItem( item );
				//		if( listItem != null )
				//			listItemsToSelect.Add( listItem );
				//	}
				//	listView.SelectedObjects = listItemsToSelect;
				//}
				//}

				//if( !needUpdate )
				//{
				//	listView.SelectedObjects = oldSelectedObjects;
				//}

			}
		}

		Item GetItemByListViewItem( EngineListView.Item listItem )
		{
			if( listItem != null )
				return listItem.Tag as Item;
			return null;
		}

		private void listView_MouseClick( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Right )
			{
				var item = GetItemByListViewItem( listView.GetItemAt( e.Location ) );
				if( item != null )
					ShowContextMenu( item, listView, e.Location );

				////!!!!!!?
				//ItemTreeNode node = GetItemTreeNode( treeView.GetNodeAt( e.Location ) );
				//if( node != null )
				//	treeView.SelectedNode = FindTreeViewNode( node );

				//var item = GetItemByNode( node );
				//ShowContextMenu( item, treeView, e.Location );
			}
		}

		private void listView_MouseUp( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Right )
			{
				var item = GetItemByListViewItem( listView.GetItemAt( e.Location ) );
				if( item == null )
				{
					if( SelectedItems.Length == 1 )
						item = SelectedItems[ 0 ];

					ShowContextMenu( item, listView, e.Location );
				}
			}
		}

		public EngineListView.Item GetListItemByItem( Item item )
		{
			if( item == null )
				return null;
			foreach( var listItem in listView.Items )
				if( listItem.Tag == item )
					return listItem;
			return null;
		}

		private void listView_SelectedItemsChanged( EngineListView sender )//object sender, ListViewItemSelectionChangedEventArgs e )
		{
			//if( sender.SelectedItems.Count == 0 )
			//	return;
			//// raise event only at selection, not deselection
			//if( !e.IsSelected )
			//	return;

			bool selectedByUser = !selectItemsMethodCalling && !setDataMethodCalling;

			var items = new List<Item>();
			if( listView.SelectedItems.Count != 0 )
			{
				foreach( var listItem in listView.SelectedItems )
					items.Add( (Item)listItem.Tag );
			}
			if( items.Count == 0 )
				items.AddRange( SelectedItems );

			bool handled = false;
			ItemAfterSelect?.Invoke( this, items, selectedByUser, ref handled );
		}

		void ListViewGoDeeperOrChoose( Item item )
		{
			try//!!!!new
			{
				////strange, but need. something strange inside ObjectListView
				//listView.SelectedObject = null;

				//if children of the item is not created then create
				var parentItem = item.Parent;
				if( parentItem != null )
				{
					SelectItems( new Item[] { parentItem }, true, false );
				}
				else
				{
					// root selected
				}

				bool handled = false;
				if( item.GetChildren( true ).Count == 0 || item.chooseByDoubleClickAndReturnKey )
				{
					//restore selection
					var listItem = GetListItemByItem( item );
					if( listItem != null )
						listView.SelectedItem = listItem;

					PerformItemAfterChoose( item, ref handled );
					//ItemAfterChoose?.Invoke( this, item, ref handled );
				}

				if( !handled )
				{
					if( item.GetChildren( true ).Count != 0 )
					{
						SelectItems( new Item[] { item }, true, true );

						//select first item
						if( listView.Items.Count > 0 )
							listView.SelectedItem = listView.Items[ 0 ];
					}
				}
			}
			catch { }
		}

		private void listView_MouseDoubleClick( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
			{
				var item = GetItemByListViewItem( listView.GetItemAt( e.Location ) );
				if( item != null )
					ListViewGoDeeperOrChoose( item );
			}
		}

		private void listView_KeyDown( object sender, KeyEventArgs e )
		{
			//KeyDownOverride
			{
				bool handled = false;
				KeyDownOverride?.Invoke( this, sender, e, ref handled );
				if( handled )
					return;
			}

			if( e.KeyCode == Keys.Return )
			{
				var item = GetItemByListViewItem( listView.SelectedItem );
				if( item != null )
					ListViewGoDeeperOrChoose( item );
			}

			if( e.KeyCode == Keys.Back )
			{
				var node = GetItemTreeNode( treeView.SelectedNode );
				if( node != null )
				{
					Item[] toSelect;
					if( node.item.Parent != null )
						toSelect = new Item[] { node.item.Parent };
					else
						toSelect = new Item[ 0 ];

					SelectItems( toSelect, false, true );
					listView.SelectedItem = GetListItemByItem( node.item );
				}
			}

			//rename
			{
				var shortcuts = EditorAPI.GetActionShortcuts( "Rename" );
				if( shortcuts != null )
				{
					foreach( var shortcut in shortcuts )
					{
						Keys keys = e.KeyCode | ModifierKeys;
						if( shortcut == keys )
						{
							RenameBegin();
							break;
						}
					}
				}

				//if( e.KeyCode == Keys.F2 )
				//	RenameBegin();
			}

			//process shortcuts
			if( ModifierKeys != Keys.None || !IsLetterOrNumber( e.KeyCode ) )
			{
				if( EditorAPI.ProcessShortcuts( e.KeyCode ) )
				{
					e.Handled = true;
					return;
				}
			}
		}

		ListModeEnum GetDemandedListMode()
		{
			var result = options.ListMode;

			if( result == ListModeEnum.Auto )
			{
				if( GetTreeSelectedItems().Any( i => i is ContentBrowserItem_File ) )
					result = ListModeEnum.Tiles;
				else
					result = ListModeEnum.List;
			}

			return result;
		}

		void UpdateListMode( bool forceUpdate )
		{
			var demandedListMode = GetDemandedListMode();

			int demandedImageSize = (int)( (float)( demandedListMode == ListModeEnum.Tiles ? options.TileImageSize : options.ListImageSize ) * EditorAPI.DPIScale );
			var demandedListTileColumnWidth = (int)( (float)options.ListColumnWidth * EditorAPI.DPIScale );

			if( listMode != demandedListMode || currentListImageSizeScaled != demandedImageSize || currentListTileColumnWidthScaled != demandedListTileColumnWidth || forceUpdate )
			{
				listMode = demandedListMode;
				currentListImageSizeScaled = demandedImageSize;
				currentListTileColumnWidthScaled = demandedListTileColumnWidth;

				if( listViewModeOverride != null )
				{
					listView.Mode = listViewModeOverride;
					//try
					//{
					//	listView.Mode = (EngineListView.ModeClass)listViewModeOverride.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { listView } );
					//}
					//catch
					//{
					//	listView.Mode = (EngineListView.ModeClass)listViewModeOverride.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { listView, this } );
					//}
				}
				else
				{
					switch( listMode )
					{
					case ListModeEnum.List:
						listView.Mode = new ContentBrowserListModeList( this );
						break;
					case ListModeEnum.Tiles:
						listView.Mode = new ContentBrowserListModeTiles( this );
						break;
					}
				}

				UpdateListImages();
			}
		}

		void UpdateListItemImage( EngineListView.Item listItem )
		{
			var item = (Item)listItem.Tag;

			var image = imageHelper.GetImage( item.imageKey, currentListImageSizeScaled, item.ShowDisabled );
			if( image == null )
				image = ContentBrowserImageHelperBasicImages.Helper.GetImage( item.imageKey, currentListImageSizeScaled, item.ShowDisabled );
			listItem.Image = image;
		}

		public void UpdateListImages()
		{
			foreach( var listItem in listView.Items )
				UpdateListItemImage( listItem );

			listView.UpdateScrollBars();
			listView.Invalidate();
		}

		private void treeView_MouseMove( object sender, MouseEventArgs e )
		{
			var newToolTip = "";

			var node = GetItemTreeNode( treeView.GetNodeAt( treeView.PointToClient( MousePosition ) ) );
			if( node != null )
				newToolTip = node.item.GetDescription();// ToolTip;

			if( currentToolTipTreeView != newToolTip )
			{
				toolTip1.SetToolTip( treeView, newToolTip );

				// Hide the tooltip in order to refresh.
				// If hiding is not done, the tooltip refreshes immediately.
				// Oherwise, it is shown again after the delay configured in tooltip.InitialDelay
				toolTip1.Hide( treeView );

				currentToolTipTreeView = newToolTip;
			}
		}

		////!!!!
		//public void Redraw()
		//{
		//	treeView.FullUpdate();
		//}

		bool isBreadcrumbVisible = true;
		bool treeOrCrumbUpdatingInProcess;

		void UpdateBreadcrumbVisibility()
		{
			//!!!! should work when called from ContentBrowser_Load, to avoid repainting when options.Breadcrumb == false
			//if( !breadCrumb.Created )
			//	return;

			if( isBreadcrumbVisible == options.Breadcrumb )
				return;

			isBreadcrumbVisible = options.Breadcrumb;

			OnBreadcrumbVisibilityChanged( isBreadcrumbVisible );
		}

		protected virtual void OnBreadcrumbVisibilityChanged( bool visible )
		{
			var panel2 = kryptonSplitContainer1.Panel2;
			if( visible )
			{
				if( !panel2.Controls.Contains( toolStripForListView ) )
				{
					panel2.Controls.Add( toolStripForListView );
					panel2.Controls.Add( kryptonBorderEdgeT );
				}

				UpdateBreadcrumb();
			}
			else
			{
				if( panel2.Controls.Contains( toolStripForListView ) )
				{
					panel2.Controls.Remove( toolStripForListView );
					panel2.Controls.Remove( kryptonBorderEdgeT );
				}
			}
		}

		private void breadCrumb_SelectedItemChanged( object sender, EventArgs e )
		{
			UpdateTreeViewSelection( breadCrumb.SelectedItem );
		}

		private void toolStripButtonUp_Click( object sender, EventArgs e )
		{
			if( breadCrumb.SelectedItem.Parent != null )
				breadCrumb.SelectedItem = breadCrumb.SelectedItem.Parent;
		}

		void UpdateBreadcrumb()
		{
			if( panelMode != PanelModeEnum.Tree )
			{
				//FIX THIS: breadcrumb fills every selection. its not optimal, bad design.
				FillBreadcrumb();

				UpdateBreadcrumbSelection( treeView.SelectedNode );
			}
		}

		//!!!!
		bool UseFirstTreeNodeAsRoot
		{
			get { return mode == ModeEnum.Objects; }
		}

		internal bool UseSelectedTreeNodeAsRootForList { get; set; } = true;

		TreeNodeAdv GetTreeRootOrFirstNode()
		{
			if( UseFirstTreeNodeAsRoot && treeView.Root.Children.Count != 0 )
				return treeView.Root.Children.First();
			return treeView.Root;
		}

		void FillBreadcrumb()
		{
			try
			{
				treeOrCrumbUpdatingInProcess = true;

				if( UseFirstTreeNodeAsRoot )
					breadCrumb.RootItem.ShortText = GetTreeRootOrFirstNode().ToString();
				else
					breadCrumb.RootItem.ShortText = EditorLocalization.Translate( "General", "Root" );

				breadCrumb.RootItem.Items.Clear();
				FillBreadcrumb( GetTreeRootOrFirstNode(), breadCrumb.RootItem );
			}
			finally
			{
				treeOrCrumbUpdatingInProcess = false;
			}
		}

		void FillBreadcrumb( TreeNodeAdv treeItem, KryptonBreadCrumbItem breadCrumbItem )
		{
			foreach( var item in treeItem.Children )
			{
				if( IsContainerNode( item ) )
				{
					var bcItem = new KryptonBreadCrumbItem( item.ToString() );
					breadCrumbItem.Items.Add( bcItem );
					FillBreadcrumb( item, bcItem );
				}
			}
		}

		void UpdateTreeViewSelection( KryptonBreadCrumbItem breadCrumbItem )
		{
			if( treeOrCrumbUpdatingInProcess )
				return;

			try
			{
				treeOrCrumbUpdatingInProcess = true;
				var node = FindTreeNodeForCrumbItem( breadCrumbItem );
				node?.Expand();
				treeView.SelectedNode = node;
			}
			finally
			{
				treeOrCrumbUpdatingInProcess = false;
			}
		}

		TreeNodeAdv FindTreeNodeForCrumbItem( KryptonBreadCrumbItem crumbItem )
		{
			if( crumbItem == null )
				return null;

			Stack<string> names = new Stack<string>();
			var crumb = crumbItem;
			while( crumb.Parent != null )
			{
				names.Push( crumb.ShortText );
				crumb = crumb.Parent;
			}

			TreeNodeAdv node = GetTreeRootOrFirstNode();
			while( names.Count > 0 )
			{
				var name = names.Pop();
				node = node.Children.First( n => n.ToString() == name );
			}
			return node;
		}

		void UpdateBreadcrumbSelection( TreeNodeAdv treeNode )
		{
			if( treeOrCrumbUpdatingInProcess )
				return;

			try
			{
				treeOrCrumbUpdatingInProcess = true;
				breadCrumb.SelectedItem = FindCrumbItemForTreeNode( treeNode );
			}
			finally
			{
				treeOrCrumbUpdatingInProcess = false;
			}
		}

		KryptonBreadCrumbItem FindCrumbItemForTreeNode( TreeNodeAdv treeNode )
		{
			Stack<string> names = new Stack<string>();
			if( treeNode != null )
			{
				var root = GetTreeRootOrFirstNode();
				while( treeNode != root )
				{
					if( IsContainerNode( treeNode ) )
						names.Push( treeNode.ToString() );
					treeNode = treeNode.Parent;
				}
			}
			var crumb = breadCrumb.RootItem;
			while( names.Count > 0 )
				crumb = crumb.Items[ names.Pop() ];
			return crumb;
		}

		bool IsContainerNode( TreeNodeAdv treeItem )
		{
			// is have childs or empty directory.

			if( treeItem.Children.Count > 0 ) // treeItem.CanExpand ?
				return true;

			var node = GetItemTreeNode( treeItem );
			if( node == null )
				return false;
			//!!!!
			return node.item is ContentBrowserItem_File file && file.IsDirectory;
		}

		[Browsable( false )]
		public List<Item> RootItems
		{
			get { return rootItems; }
		}

		[DefaultValue( false )]
		public bool MultiSelect
		{
			get { return multiSelect; }
			set
			{
				if( multiSelect == value )
					return;
				multiSelect = value;
				MultiSelectUpdate();
			}
		}

		void MultiSelectUpdate()
		{
			if( treeView != null )
				treeView.SelectionMode = multiSelect ? TreeSelectionMode.Multi : TreeSelectionMode.Single;
			if( listView != null )
				listView.MultiSelect = multiSelect;
		}

		List<Item> GetTreeSelectedItems()
		{
			var result = new List<Item>();

			//if( Options.PanelMode != PanelModeEnum.List ) //!!!!new
			//{
			foreach( var node2 in treeView.SelectedNodes )
			{
				var item = GetItemTreeNode( node2 )?.item;
				if( item != null )
					result.Add( item );
			}
			//}

			return result;
		}

		void UpdateSplitterDistanceFromOptions()
		{
			if( !splitterMoving )
			{
				if( panelMode == PanelModeEnum.TwoPanelsSplitHorizontally || panelMode == PanelModeEnum.TwoPanelsSplitVertically )
				{
					var size = kryptonSplitContainer1.Orientation == Orientation.Horizontal ? kryptonSplitContainer1.Height : kryptonSplitContainer1.Width;
					if( size != 0 )
					{
						double v = options.SplitterPosition * size;
						kryptonSplitContainer1.SplitterDistance = (int)v;
					}
				}
			}
		}

		private void kryptonSplitContainer1_SplitterMoving( object sender, SplitterCancelEventArgs e )
		{
			splitterMoving = true;
		}

		private void kryptonSplitContainer1_SplitterMoved( object sender, SplitterEventArgs e )
		{
			splitterMoving = false;

			//update options
			if( panelMode == PanelModeEnum.TwoPanelsSplitHorizontally || panelMode == PanelModeEnum.TwoPanelsSplitVertically )
			{
				var size = kryptonSplitContainer1.Orientation == Orientation.Horizontal ? kryptonSplitContainer1.Height : kryptonSplitContainer1.Width;
				if( size != 0 )
					options.SplitterPosition = (double)kryptonSplitContainer1.SplitterDistance / size;
			}
		}

		void TreeViewBeginUpdate()
		{
			if( treeViewUpdating )
				return;

			treeView.BeginUpdate();
			treeViewUpdating = true;
		}

		void TreeViewEndUpdate()
		{
			if( treeViewUpdating )
			{
				treeViewUpdating = false;
				treeView.EndUpdate();
			}
		}

		public void RemoveTreeViewIconsColumn()
		{
			treeView.NodeControls.RemoveAt( 0 );
			treeView.NodeControls[ 0 ].LeftMargin = 0;
		}

		[Browsable( false )]
		public TreeViewAdv TreeView
		{
			get { return treeView; }
		}

		private void toolStripButtonSearch_Click( object sender, EventArgs e )
		{
			var form = new ContentBrowserSearchForm();
			form.Browser = this;

			EditorForm.Instance.WorkspaceController.BlockAutoHideAndDoAction( this, () =>
			{
				form.ShowDialog();
			} );
		}

		[Browsable( false )]
		public EngineListView.ModeClass ListViewModeOverride
		{
			get { return listViewModeOverride; }
			set { listViewModeOverride = value; }
		}

		//[Browsable( false )]
		//public Type ListViewModeOverride
		//{
		//	get { return listViewModeOverride; }
		//	set { listViewModeOverride = value; }
		//}

		public Item FindItemByTag( object tag )
		{
			foreach( var item in GetAllItems() )
				if( item.Tag == tag )
					return item;
			return null;
		}

		private void ToolStripDropDownButtonFilteringMode_Click( object sender, EventArgs e )
		{
			if( ReadOnlyHierarchy )
				return;

			var items = new List<KryptonContextMenuItemBase>();

			{
				var item = new KryptonContextMenuItem( EditorLocalization.Translate( "ContentBrowser.FilteringMode", "All" ), null,
				   delegate ( object s, EventArgs e2 )
				   {
					   FilteringMode = null;
				   } );
				item.Checked = filteringMode == null;
				items.Add( item );
			}

			foreach( var mode in filteringModes )
			{
				var item = new KryptonContextMenuItem( EditorLocalization.Translate( "ContentBrowser.FilteringMode", mode.Name ), null,
				   delegate ( object s, EventArgs e2 )
				   {
					   FilteringMode = (ContentBrowserFilteringMode)( (KryptonContextMenuItem)s ).Tag;
				   } );
				item.Checked = filteringMode == mode;
				item.Tag = mode;
				items.Add( item );
			}

			EditorContextMenuWinForms.AddActionsToMenu( EditorContextMenuWinForms.MenuTypeEnum.None, items );

			ShowContextMenuEvent?.Invoke( this, items );

			EditorContextMenuWinForms.Show( items, this, PointToClient( Cursor.Position ) );
		}

		public void SetEnabled( bool value )
		{
			if( Enabled == value )
				return;

			Enabled = value;
			listView.Enabled = value;
			TreeViewHideVScroll();

			//white blinking fix
			if( !Enabled )
				listView.Visible = false;
		}

		private void EngineScrollBarTreeVertical_Scroll( object sender, EngineScrollBarEventArgs e )
		{
			treeView.VScrollBar.Value = (int)engineScrollBarTreeVertical.Value;
		}

		private void EngineScrollBarTreeHorizontal_Scroll( object sender, EngineScrollBarEventArgs e )
		{
			treeView.HScrollBar.Value = (int)engineScrollBarTreeHorizontal.Value;
		}

		void UpdateScrollBarsTree()
		{
			if( treeView.HScrollBar.Size != new Size( treeView.HScrollBar.Size.Width, 0 ) )
				treeView.HScrollBar.Size = new Size( treeView.HScrollBar.Size.Width, 0 );
			if( treeView.HScrollBar.TabStop )
				treeView.HScrollBar.TabStop = false;

			if( treeView.VScrollBar.Size != new Size( 0, treeView.VScrollBar.Size.Height ) )
				treeView.VScrollBar.Size = new Size( 0, treeView.VScrollBar.Size.Height );
			if( treeView.VScrollBar.TabStop )
				treeView.VScrollBar.TabStop = false;

			bool updating1 = engineScrollBarTreeVertical.MouseUpDownStatus;//&& engineScrollBar1.MouseScrollBarArea == EnhancedScrollBarMouseLocation.Thumb;
			if( !updating1 )
			{
				var rows = (int)( (double)treeView.Height / (double)treeView.RowHeightScaled ) - 1;
				engineScrollBarTreeVertical.ItemSize = treeView.RowHeightScaled;//Indent
				engineScrollBarTreeVertical.Maximum = Math.Max( treeView.VScrollBar.Maximum - rows, 0 );

				engineScrollBarTreeVertical.SmallChange = treeView.VScrollBar.SmallChange;
				engineScrollBarTreeVertical.LargeChange = treeView.VScrollBar.LargeChange;
				engineScrollBarTreeVertical.Value = treeView.VScrollBar.Value;
			}

			bool updating2 = engineScrollBarTreeHorizontal.MouseUpDownStatus;//&& engineScrollBar2.MouseScrollBarArea == EnhancedScrollBarMouseLocation.Thumb;
			if( !updating2 )
			{
				engineScrollBarTreeHorizontal.Maximum = Math.Max( treeView.HScrollBar.Maximum - treeView.ClientSize.Width, 0 );
				engineScrollBarTreeHorizontal.SmallChange = treeView.HScrollBar.SmallChange;
				engineScrollBarTreeHorizontal.LargeChange = treeView.HScrollBar.LargeChange;
				engineScrollBarTreeHorizontal.Value = treeView.HScrollBar.Value;
			}

			kryptonSplitContainerTree.Panel2Collapsed = !treeView.HScrollBar.Visible;
			kryptonSplitContainerTreeSub1.Panel2Collapsed = !treeView.VScrollBar.Visible;
			kryptonSplitContainerTreeSub2.Panel2Collapsed = !treeView.VScrollBar.Visible;
		}

		void UpdateScrollBars()
		{
			UpdateScrollBarsTree();
		}

		private void listView_MouseMove( object sender, MouseEventArgs e )
		{
			var newToolTip = "";

			var pos = listView.PointToClient( MousePosition );
			var item = listView.GetItemAt( pos );
			if( item != null && item.ShowTooltip )
				newToolTip = item.Description;

			if( currentToolTipListView != newToolTip )
			{
				toolTip2.SetToolTip( this.listView, newToolTip );

				// Hide the tooltip in order to refresh.
				// If hiding is not done, the tooltip refreshes immediately.
				// Oherwise, it is shown again after the delay configured in tooltip.InitialDelay
				toolTip2.Hide( listView );

				currentToolTipListView = newToolTip;
			}
		}

		public void AddImageKey( string key, Image imageSmall, Image imageBig )
		{
			if( imageSmall == null && imageBig == null )
				return;

			if( imageSmall == null )
			{
				AddImageKey( key, imageBig );
				return;
			}

			if( imageBig == null )
			{
				AddImageKey( key, imageSmall );
				return;
			}

			imageHelper.AddImage( key, imageSmall, imageBig );
		}

		public void AddImageKey( string key, Image image )
		{
			imageHelper.AddImage( key, null, image );
		}

		//public void AddImageKey( string key, ICollection<Image> images )
		//{
		//}

		public EngineListView GetListView()
		{
			return listView;
		}

		private void ListView_BeforeStartDrag( EngineListView sender, EngineListView.Item[] items, ref bool canStart )
		{
			canStart = items.Length == 1;
		}

	}
}
