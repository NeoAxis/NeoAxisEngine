#if !DEPLOY
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
using Internal.ComponentFactory.Krypton.Toolkit;
using System.IO;
using Internal.Aga.Controls.Tree;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a rich control to work with hierarchical data in tree or list form.
	/// </summary>
	public partial class ContentBrowser : EUserControl
	{
		public static bool AllowAllTypes = true;

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

		List<Type> resourcesWindowTypesAdded = new List<Type>();

		ContentBrowserItem_Favorites favoritesItem;
		//ContentBrowserItem_File dataItem;

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

		ContentBrowserFilteringMode filteringMode;

		TreeModel treeViewModel;

		EditorImageHelper imageHelper = new EditorImageHelper();

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

		internal bool needUpdateImages;

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
			public Metadata.TypeInfo[] selectTypeDemandedTypes;//public Metadata.TypeInfo selectTypeDemandedType;
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

		public delegate void UpdateDataEventDelegate( ContentBrowser sender, IList<Item> roots );
		public static event UpdateDataEventDelegate AllInstancesUpdateDataEvent;
		public event UpdateDataEventDelegate UpdateDataEvent;

		////!!!!new
		//public delegate void ItemBeforeSelectDelegate( ItemTreeNode node, ref bool handled );
		//public event ItemBeforeSelectDelegate ItemBeforeSelect;

		public delegate void ItemAfterSelectDelegate( ContentBrowser sender, IList<Item> items, bool selectedByUser, ref bool handled );
		public event ItemAfterSelectDelegate ItemAfterSelect;

		public delegate void ItemAfterChooseDelegate( ContentBrowser sender, Item item, ref bool handled );
		public event ItemAfterChooseDelegate ItemAfterChoose;

		public delegate void OverrideItemTextDelegate( ContentBrowser sender, Item item, ref string text );
		public event OverrideItemTextDelegate OverrideItemText;

		public delegate void ShowContextMenuEventDelegate( ContentBrowser sender, Item contentItem, List<KryptonContextMenuItemBase> items );
		public event ShowContextMenuEventDelegate ShowContextMenuEvent;

		public delegate void KeyDownOverrideDelegate( ContentBrowser browser, object sender, KeyEventArgs e, ref bool handled );
		public event KeyDownOverrideDelegate KeyDownOverride;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public abstract class Item : IDisposable
		{
			ContentBrowser owner;

			Item parent;
			public string imageKey;
			public Image image;

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

			public delegate void GetChildrenFilterEventDelegate( Item item, ref IList<Item> list );
			public static event GetChildrenFilterEventDelegate GetChildrenFilterEvent;

			public IList<Item> GetChildrenFilter( bool onlyAlreadyCreated )
			{
				var result = GetChildren( onlyAlreadyCreated );
				GetChildrenFilterEvent?.Invoke( this, ref result );
				return result;
			}

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

			public virtual object ContainedObject
			{
				get { return null; }
			}

			public void GetChildrenOnlyAlreadyCreatedRecursive( List<Item> result )
			{
				foreach( var child in GetChildrenFilter( true ) )
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
					Image image = null;

					//preview image
					var fileItem = this as ContentBrowserItem_File;
					if( fileItem != null && !fileItem.IsDirectory )
						image = PreviewImagesManager.GetImageForResource( fileItem.FullPath, true );

					if( image == null )
						image = this.image;
					if( image == null )
						image = owner.imageHelper.GetImageScaledForTreeView( imageKey, showDisabled );
					if( image == null )
						image = EditorImageHelperBasicImages.Helper.GetImageScaledForTreeView( imageKey, showDisabled );

					itemNode.Image = image;
				}
			}

			public ICollection<Item> GetAllParents( bool makeOrderFromTopToBottom )
			{
				var list = new List<Item>();
				var current = Parent;
				while( current != null )
				{
					list.Add( current );
					current = current.Parent;
				}
				if( makeOrderFromTopToBottom )
					list.Reverse();
				return list;//.ToArray();
			}

			public virtual Image[] GetCornerImages() { return null; }
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
			treeView.DrawRowEvent += TreeView_DrawRowEvent;
			treeView.DrawIconAddition += TreeView_DrawIconAddition;

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
					button.Text = EditorLocalization2.Translate( "ContentBrowser", button.Text );

				var button2 = item as ToolStripDropDownButton;
				if( button2 != null )
					button2.Text = EditorLocalization2.Translate( "ContentBrowser", button2.Text );
			}

			toolStripForTreeView.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();
			toolStripForListView.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();
			toolStripSearch.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();

			if( EditorAPI2.DarkTheme )
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
				var dpiScale = Math.Min( EditorAPI2.DPIScale, 2 );

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

			if( EditorAPI2.DarkTheme )
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
			if( DocumentWindow?.Document2 != null && DocumentWindow.Document2.Destroyed )
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
			if( DocumentWindow?.Document2 != null && DocumentWindow.Document2.Destroyed )
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
				foreach( var childItem in item.GetChildrenFilter( false ) )
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
			if( item.GetChildrenFilter( false ).Count != 0 )
				itemNode.Nodes.Add( new DummyTreeNode() );
			//foreach( var child in item.GetChildrenFiltered() )// false ) )
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
			if( DocumentWindow?.Document2 != null && DocumentWindow.Document2.Destroyed )
				return;

			var node = GetNodeByItem( item );
			if( node == null )
				return;

			if( ContentBrowserUtility.allContentBrowsers_SuspendChildrenChangedEvent )
			{
				ContentBrowserUtility.allContentBrowsers_SuspendChildrenChangedEvent_Items.AddWithCheckAlreadyContained( (this, item) );
				return;
			}

			var newItems = item.GetChildrenFilter( false );

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
				var shortcuts = EditorAPI2.GetActionShortcuts( "Rename" );
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
				if( EditorAPI2.ProcessShortcuts( e.KeyCode, true ) )
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

		void GetAllItemsByItemHierarchyRecursive( bool onlyAlreadyCreatedChildren, Item item, List<Item> list )
		{
			list.Add( item );
			foreach( var child in item.GetChildren( onlyAlreadyCreatedChildren ) )
				GetAllItemsByItemHierarchyRecursive( onlyAlreadyCreatedChildren, child, list );
		}

		public List<Item> GetAllItemsByItemHierarchy( bool onlyAlreadyCreatedChildren )
		{
			var list = new List<Item>();
			foreach( var item in rootItems )
				GetAllItemsByItemHierarchyRecursive( onlyAlreadyCreatedChildren, item, list );
			return list;
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

		public void SelectItemsList( IList<Item> items, bool considerAsSelectedByUser = false )
		{
			if( !considerAsSelectedByUser )
				selectItemsMethodCalling = true;

			try
			{
				if( items != null && items.Count != 0 )
				{
					var parent = items[ 0 ].Parent;
					if( parent != null )
						SelectItems( new[] { parent } );
					else
						SelectItems( new Item[ 0 ] );

					var listItems = new List<EngineListView.Item>();
					foreach( var item in items )
					{
						var listItem = GetListItemByItem( item );
						if( listItem != null )
							listItems.Add( listItem );
					}
					listView.SelectedItems = listItems;

					foreach( var listItem in listItems )
						listView.EnsureVisible( listItem );
				}
				else
				{
					//reset selected items list

					//!!!!

					listView.SelectedItems = new List<EngineListView.Item>();

					//SelectItems( new Item[ 0 ] );
				}
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

		[Browsable( false )]
		public Item[] SelectedItemsOnlyListView
		{
			get
			{
				if( listView.SelectedItems.Count != 0 )
				{
					List<Item> result = new List<Item>( listView.SelectedItems.Count );
					foreach( var item in listView.SelectedItems )
						result.Add( (Item)item.Tag );
					return result.ToArray();
				}
				else
					return Array.Empty<Item>();
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

		//		return item.Parent.GetChildrenFiltered( false ).IndexOf( item );
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
						{
							var fileItem = item as ContentBrowserItem_File;
							if( fileItem != null && fileItem.IsDirectory )
								fileItem.PerformChildrenChanged();
						}

						//Launcher specific
						{
							var fileItem = item as LauncherEditRepositoryContentBrowserItem;
							if( fileItem != null && fileItem.IsDirectory )
								fileItem.PerformChildrenChanged();
						}
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

			//update images
			if( needUpdateImages )
			{
				needUpdateImages = false;
				foreach( var item in GetAllItems() )
					item.UpdateImage();
				UpdateListImages();
			}

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

			var roots = new List<Item>();

			if( Mode == ModeEnum.Objects )
				UpdateData_Objects( roots );
			else if( Mode == ModeEnum.Resources || Mode == ModeEnum.SetReference )
				UpdateData_ResourcesAndSetReference( roots );
			//else
			//	Log.Fatal( "impl" );

			//to add custom items from the event
			AllInstancesUpdateDataEvent?.Invoke( this, roots );
			UpdateDataEvent?.Invoke( this, roots );

			SetData( roots );
		}

		void UpdateData_Objects( List<Item> roots )
		{
			//!!!!!

			//List<Item> roots = new List<Item>();

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

			//SetData( roots );
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

		internal static string GetTypeImageKey( Metadata.TypeInfo type )
		{
			switch( type.Classification )
			{
			case Metadata.TypeClassification.Enumeration: return "Enum";
			case Metadata.TypeClassification.Delegate: return "Delegate";
			}

			return EditorImageHelperComponentTypes.GetImageKey( type );
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
						if( !setReferenceModeData.selectTypeDemandedTypes.Any( t => ContentBrowserUtility.ContainsType( t.GetNetType(), netType ) ) )
							skip = true;
						//if( !ContentBrowserUtility.ContainsType( setReferenceModeData.selectTypeDemandedType.GetNetType(), netType ) )
						//	skip = true;
						////if( !ContentBrowserUtils.ContainsType( setReferenceModeData.demandedType.GetNetType(), netType ) )
						////	skip = true;
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

		//		bool remove = !allow && item.GetChildrenFiltered( true ).Count == 0;
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
		internal /*obfuscator*/ void Resources_RemoveExcessClassesItemsByFilteringMode( Item classesItem )
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

				bool remove = !allow && item.GetChildrenFilter( false ).Count == 0;
				//bool remove = !allow && item.GetChildrenFiltered( true ).Count == 0;
				if( remove )
				{
					var parent = item.Parent;

					var typeItem = parent as ContentBrowserItem_Type;
					var virtualItem = parent as ContentBrowserItem_Virtual;

					if( typeItem != null )
						typeItem.DeleteNestedTypeChild( item );
					else if( virtualItem != null )
						virtualItem.DeleteChild( item );
					else
					{
						//!!!!что-то еще?
					}
				}
			}
		}

		internal /*obfuscator*/ void SetReference_RemoveExcessTypeItems( Item classesItem )
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

								if( !setReferenceModeData.selectTypeDemandedTypes.Any( t => t.IsAssignableFrom( type ) ) )
									allow = false;
								if( type.Abstract && !setReferenceModeData.selectTypeWindowCanSelectAbstractClass )
									allow = false;

								//if( !setReferenceModeData.selectTypeDemandedType.IsAssignableFrom( type ) || type.Abstract && !setReferenceModeData.selectTypeWindowCanSelectAbstractClass )
								//	allow = false;
								////if( !setReferenceModeData.demandedType.IsAssignableFrom( type ) || type.Abstract && !setReferenceModeData.selectTypeWindowCanSelectAbstractClass )
								////	allow = false;
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

				//bool existChildren = item.GetChildrenFiltered().Count != 0;
				//bool existChildren = false;
				//{
				//	//skip DUMMY
				//	foreach( var i in item.GetChildrenFiltered() )
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
				bool remove = !allow && item.GetChildrenFilter( true ).Count == 0;
				if( remove )
				{
					var parent = item.Parent;

					var typeItem = parent as ContentBrowserItem_Type;
					var virtualItem = parent as ContentBrowserItem_Virtual;

					if( typeItem != null )
						typeItem.DeleteNestedTypeChild( item );
					else if( virtualItem != null )
						virtualItem.DeleteChild( item );
					else
					{
						//!!!!что-то еще?
					}
				}
			}
		}

		void UpdateData_ResourcesAndSetReference( List<Item> roots )
		{
			//List<Item> roots = new List<Item>();

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
				resourcesWindowTypesAdded.Clear();

				Dictionary<string, Item> browserItems = new Dictionary<string, Item>();

				Item GetBrowserItemByPath( string path )
				{
					browserItems.TryGetValue( path, out var item );
					return item;
				}

				foreach( var item in ResourcesWindowItems.Items )
				{
					//custom filtering
					if( !EditorUtility2.PerformResourcesWindowItemVisibleFilter( item ) )
						continue;

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

					resourcesWindowTypesAdded.Add( item.Type );

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
								var text = EditorLocalization2.Translate( "ContentBrowser.Group", strings[ n ] );
								var browserItem2 = new ContentBrowserItem_Virtual( this, parentItem, text );

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
								roots.Add( browserItem );
						}
					}
				}

				//expand Base at startup
				{
					var baseItem = GetBrowserItemByPath( "Base" );
					if( baseItem != null )
						baseItem.expandAtStartup = true;
				}
			}

			/////////////////////////////////////
			//Favorites
			if( EditorFavorites.AllowFavorites && ( ( Mode == ModeEnum.Resources && ( FilteringMode == null || FilteringMode.AddGroupFavorites ) ) || ( Mode == ModeEnum.SetReference && MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ).IsAssignableFrom( SetReferenceModeData.DemandedType ) && !setReferenceModeData.selectTypeWindow ) ) )
			{
				favoritesItem = new ContentBrowserItem_Favorites( this, null, EditorLocalization2.Translate( "ContentBrowser.Group", "Favorites" ) );
				favoritesItem.imageKey = "Folder";

				if( Mode == ModeEnum.SetReference )
					SetReference_RemoveExcessTypeItems( (Item)favoritesItem );

				//remove excess classes in Resources and filtering mode
				if( Mode == ModeEnum.Resources && FilteringMode != null )
					Resources_RemoveExcessClassesItemsByFilteringMode( (Item)favoritesItem );

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
			if( AllowAllTypes && ( ( Mode == ModeEnum.Resources || Mode == ModeEnum.SetReference ) && ( FilteringMode == null || FilteringMode.AddGroupAllTypes ) ) )
			{
				var allTypesItem = new ContentBrowserItem_Virtual( this, null, EditorLocalization2.Translate( "ContentBrowser.Group", "All types" ) );
				allTypesItem.imageKey = "Folder";
				allTypesItem.Tag = "All types";

				UpdateAllTypesItem( allTypesItem, false );

				if( allTypesItem != null )
					roots.Add( allTypesItem );
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
				var dataItem = new ContentBrowserItem_File( this, null, VirtualFileSystem.Directories.Assets, true );
				dataItem.SetText( EditorLocalization2.Translate( "ContentBrowser.Group", "Assets" ) );
				dataItem.imageKey = "Folder";
				dataItem.expandAtStartup = true;
				if( Mode == ModeEnum.SetReference && setReferenceModeData.selectTypeWindow )
					dataItem.expandAtStartup = false;

				if( FilteringMode != null && FilteringMode.ExpandAllFileItemsAtStartup )
					dataItem.expandAllAtStartup = true;

				roots.Add( dataItem );
			}
			//if( dataItem != null )
			//	roots.Add( dataItem );

			if( Mode == ModeEnum.Resources && ( FilteringMode != null && FilteringMode.AddAllFiles ) )
			{
				var dataItem = new ContentBrowserItem_File( this, null, VirtualFileSystem.Directories.AllFiles, true );
				dataItem.SetText( EditorLocalization2.Translate( "ContentBrowser.Group", "All files" ) );
				dataItem.imageKey = "Folder";
				roots.Add( dataItem );
			}

			//if( Mode == ModeEnum.Resources && ( FilteringMode != null && FilteringMode.AddAllFiles ) )
			//{
			//	try
			//	{
			//		//!!!!sort by

			//		var folders = Directory.GetDirectories( VirtualFileSystem.Directories.Project, "*", SearchOption.TopDirectoryOnly );
			//		foreach( var folder in folders )
			//		{
			//			var name = Path.GetFileName( folder );
			//			if( name != "Assets" )
			//			{
			//				var dataItem2 = new ContentBrowserItem_File( this, null, folder, true );
			//				dataItem2.SetText( name );
			//				dataItem2.imageKey = "Folder";

			//				roots.Add( dataItem2 );
			//			}
			//		}

			//		var files = Directory.GetFiles( VirtualFileSystem.Directories.Project, "*", SearchOption.TopDirectoryOnly );
			//		foreach( var file in files )
			//		{
			//			var name = Path.GetFileName( file );

			//			var dataItem2 = new ContentBrowserItem_File( this, null, file, false );//, true );
			//			dataItem2.SetText( name );
			//			dataItem2.imageKey = "Resource";

			//			roots.Add( dataItem2 );
			//		}

			//	}
			//	catch { }
			//}


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

			//SetData( roots );
		}

		string Translate( string text )
		{
			return EditorLocalization2.Translate( "ContentBrowser", text );
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
				if( componentItem != null && componentItem.Component != null && !componentItem.Component.EditorReadOnlyInHierarchy && EditorAPI2.GetDocumentByObject( componentItem.Component ) != null )
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
			//			SelectItems( new Item[] { parentItem.GetChildrenFiltered( false )[ lastCount ] } );
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

			EditorAPI2.OpenNewObjectWindow( data );

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

		private void VirtualFileWatcher_Update( FileSystemEventArgs args )
		{
			try
			{
				switch( args.ChangeType )
				{
				case WatcherChangeTypes.Created:
				case WatcherChangeTypes.Deleted:
					{
						var directoryName = Path.GetDirectoryName( args.FullPath );
						CallPerformChildrenChanged( directoryName );
						//string virtualPath = VirtualPathUtility.GetVirtualPathByReal( directoryName, true );
						//CallPerformChildrenChanged( virtualPath );
					}
					break;

				case WatcherChangeTypes.Renamed:
					{
						RenamedEventArgs args2 = (RenamedEventArgs)args;

						var directoryNameOld = Path.GetDirectoryName( args2.OldFullPath );
						CallPerformChildrenChanged( directoryNameOld );
						//string virtualPathOld = VirtualPathUtility.GetVirtualPathByReal( directoryNameOld, true );
						//CallPerformChildrenChanged( virtualPathOld );

						var directoryName = Path.GetDirectoryName( args.FullPath );
						if( string.Compare( directoryNameOld, directoryName, true ) != 0 )
							CallPerformChildrenChanged( directoryName );

						//string virtualPath = VirtualPathUtility.GetVirtualPathByReal( directoryName, true );
						//if( string.Compare( virtualPathOld, virtualPath, true ) != 0 )
						//	CallPerformChildrenChanged( virtualPath );
					}
					break;

					//case WatcherChangeTypes.Changed:
					//	{
					//	}
					//	break;
				}
			}
			catch { }
		}

		void CallPerformChildrenChanged( string realPath )// string virtualPath )
		{
			//update project folders

			var allFilesPath = VirtualPathUtility.GetAllFilesPathByReal( realPath );
			if( !string.IsNullOrEmpty( allFilesPath ) )
			{
				foreach( var currentItem in ContentBrowserUtility.FindCreatedItemsByRealPath( this, realPath ) )
				{
					//update children. need update parent items for C# filtering mode (when directories without items are hided)
					Item item = currentItem;
					while( item != null )
					{
						item.PerformChildrenChanged();
						item = item.Parent;
					}
				}
			}

			//var projectPath = VirtualPathUtility.GetProjectPathByReal( realPath );
			//if( !string.IsNullOrEmpty( projectPath ) )
			//{
			//	var currentItem = ContentBrowserUtility.FindCreatedItemByRealPath( this, realPath );

			//	//update children. need update parent items for C# filtering mode (when directories without items are hided)
			//	if( currentItem != null )
			//	{
			//		Item item = currentItem;
			//		while( item != null )
			//		{
			//			item.PerformChildrenChanged();
			//			item = item.Parent;
			//		}
			//	}
			//}

			////update Assets
			//var virtualPath = VirtualPathUtility.GetVirtualPathByReal( realPath, true );
			//if( dataItem != null && !string.IsNullOrEmpty( virtualPath ) )
			//{
			//	var names = virtualPath.Split( new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries );

			//	string current = "";
			//	ContentBrowserItem_File currentItem = dataItem;

			//	for( int n = 0; n < names.Length; n++ )
			//	{
			//		current = Path.Combine( current, names[ n ] );

			//		string realPath2 = VirtualPathUtility.GetRealPathByVirtual( current );
			//		string key = ContentBrowserItem_File.GetFileChildrenKey( realPath2 );

			//		if( !currentItem.FileChildren.TryGetValue( key, out Item item ) )
			//		{
			//			currentItem = null;
			//			break;
			//		}

			//		currentItem = (ContentBrowserItem_File)item;
			//	}

			//	//update children. need update parent items for C# filtering mode (when directories without items are hided)
			//	if( currentItem != null )
			//	{
			//		Item item = currentItem;
			//		while( item != null )
			//		{
			//			item.PerformChildrenChanged();
			//			item = item.Parent;
			//		}
			//	}
			//}

			//update Favorites
			if( favoritesItem != null )
			{
				//var realPath = VirtualPathUtility.GetRealPathByVirtual( virtualPath );

				foreach( var item2 in favoritesItem.GetChildren( true ).ToArray() )
				{
					var fileItem = item2 as ContentBrowserItem_File;
					if( fileItem != null && fileItem.FullPath == realPath )
						fileItem.PerformChildrenChanged();
				}
			}
		}

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
					if( ins.Owner.LoadFromFile )
					{
						var virtualPath = ins.Owner.Name;

						string realPath = VirtualPathUtility.GetRealPathByVirtual( virtualPath );
						CallPerformChildrenChanged( realPath );
						//CallPerformChildrenChanged( virtualPath );
					}
				}
				break;
			}
		}

		private void ResourceInstance_AllInstances_DisposedEvent( Resource.Instance ins )
		{
			if( EngineApp.Closing )
				return;
			if( ins.InstanceType != Resource.InstanceType.Resource )
				return;

			if( ins.Owner.LoadFromFile )
			{
				var virtualPath = ins.Owner.Name;

				string realPath = VirtualPathUtility.GetRealPathByVirtual( virtualPath );
				CallPerformChildrenChanged( realPath );
				//CallPerformChildrenChanged( virtualPath );
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
						string virtualName = VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath, true );
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
						string virtualName = VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath, true );
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
			if( DocumentWindow?.Document2 != null && DocumentWindow.Document2.Destroyed )
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
							string virtualName = VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath, true );
							if( string.IsNullOrEmpty( virtualName ) )
							{
								//!!!!
							}

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

			if( EditorAPI2.IsDocumentFileSupport( fileItem.FullPath ) )
			{
				//!!!!!temp. тут меньше проверок, чем в AfterSelect

				//!!!!can use already opened

				EditorAPI2.OpenFileAsDocument( fileItem.FullPath, true, true );
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
							if( component != null && component.Parent != null && !component.EditorReadOnlyInHierarchy && EditorAPI2.GetDocumentByObject( component ) != null )
								resultItemsToDelete.Add( componentItem );
						}

						//var noSpecItem = item as ContentBrowserItem_NoSpecialization;
						//if( noSpecItem != null && noSpecItem.CanDelete() )
						//	resultItemsToDelete.Add( noSpecItem );
					}

					var fileItem = item as ContentBrowserItem_File;
					if( fileItem != null && fileItem.FullPath != VirtualFileSystem.Directories.Assets )
						resultItemsToDelete.Add( fileItem );
				}
			}

			//remove children which inside selected parents
			resultItemsToDelete = GetItemsWithoutChildren( resultItemsToDelete );

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
						int index = item.Parent.GetChildrenFilter( true ).IndexOf( item );
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
						itemToSelectAfterDelection = itemWithMinIndex.Parent.GetChildrenFilter( true )[ minIndex - 1 ];
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
				if( DocumentWindow != null && DocumentWindow.Document2 != null )
				{
					var document = DocumentWindow.Document2;
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

		static List<Item> GetItemsWithoutChildren( ICollection<Item> list )
		{
			var set = new ESet<Item>( list.Count );
			set.AddRangeWithCheckAlreadyContained( list );

			var newList = new List<Item>( list.Count );

			foreach( var obj in list )
			{
				var allParents = obj.GetAllParents( false );

				if( !allParents.Any( p => set.Contains( p ) ) )
					newList.Add( obj );
			}

			return newList;
		}

		public bool CanCloneObjects( out List<Item> resultItemsToClone )
		{
			resultItemsToClone = new List<Item>();

			if( ReadOnlyHierarchy )
				return false;
			if( documentWindow == null || documentWindow.Document2 == null )
				return false;

			foreach( var item in SelectedItems )
			{
				if( item.Parent != null )
				{
					var componentItem = item as ContentBrowserItem_Component;
					if( componentItem != null )
					{
						var component = componentItem.Component;
						if( component != null && component.Parent != null && !component.EditorReadOnlyInHierarchy && EditorAPI2.GetDocumentByObject( component ) != null )
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

			//remove children which inside selected parents
			resultItemsToClone = GetItemsWithoutChildren( resultItemsToClone );

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
			var action = new UndoActionComponentCreateDelete( documentWindow.Document2, newObjects, true );
			documentWindow.Document2.UndoSystem.CommitAction( action );
			documentWindow.Document2.Modified = true;
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
				//	foreach( var item2 in memberItem.GetChildrenFiltered() )
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
			toolStripDropDownButtonFilteringMode.Visible = ContentBrowserOptions.AllowFilteringModeButton && Options.FilteringModeButton && Mode == ModeEnum.Resources;

			toolStripButtonShowMembers.Visible = ContentBrowserOptions.AllowMembersButton && Options.MembersButton && Mode == ModeEnum.Objects;

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
			toolStripButtonEditor.Enabled = selectedComponents.Count == 1 && DocumentWindow != null && DocumentWindow.Document2 != null && EditorAPI2.IsDocumentObjectSupport( selectedComponents[ 0 ] ) && !selectedComponents[ 0 ].EditorReadOnlyInHierarchy;
			toolStripButtonSettings.Enabled = selectedComponents.Count == 1 && DocumentWindow != null && DocumentWindow.Document2 != null && !selectedComponents[ 0 ].EditorReadOnlyInHierarchy;

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
			if( selectedComponents.Count == 1 && DocumentWindow != null && DocumentWindow.Document2 != null )
			{
				EditorAPI2.OpenDocumentWindowForObject( DocumentWindow.Document2, selectedComponents[ 0 ] );
			}
		}

		private void toolStripButtonSettings_Click( object sender, EventArgs e )
		{
			var selectedComponents = GetSelectedComponents();
			if( selectedComponents.Count == 1 && DocumentWindow != null && DocumentWindow.Document2 != null )
			{
				bool canUseAlreadyOpened = !ModifierKeys.HasFlag( Keys.Shift );
				EditorAPI2.ShowObjectSettingsWindow( DocumentWindow.Document2, selectedComponents[ 0 ], canUseAlreadyOpened );
			}
		}

		private void toolStripButtonNewFolder_Click( object sender, EventArgs e )
		{
			NewFolder();
		}

		private void toolStripButtonNewResource_Click( object sender, EventArgs e )
		{
			NewResource( null, true );
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
			if( selectedComponents.Count == 1 && SelectedItems[ 0 ].Parent != null && DocumentWindow != null && DocumentWindow.Document2 != null &&
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
			if( selectedComponents.Count == 1 && SelectedItems[ 0 ].Parent != null && DocumentWindow != null && DocumentWindow.Document2 != null &&
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

			var action = new UndoActionComponentMove( DocumentWindow.Document2, component, parent, oldIndex );
			DocumentWindow.Document2.UndoSystem.CommitAction( action );
			DocumentWindow.Document2.Modified = true;

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

			var action = new UndoActionComponentMove( DocumentWindow.Document2, component, parent, oldIndex );
			DocumentWindow.Document2.UndoSystem.CommitAction( action );
			DocumentWindow.Document2.Modified = true;

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

		public /*internal */static DragDropItemData GetDroppingItemData( IDataObject data )
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

					if( targetComponent != null && DocumentWindow?.Document2 != null )
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
									var action = new UndoActionComponentCreateDelete( DocumentWindow.Document2, newObjects, true );
									DocumentWindow.Document2.UndoSystem.CommitAction( action );
									DocumentWindow.Document2.Modified = true;
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
											var action = new UndoActionComponentCreateDelete( DocumentWindow.Document2, newObjects, true );
											DocumentWindow.Document2.CommitUndoAction( action );
										}
										else
										{
											//change parent

											var oldParent = droppingComponent.Parent;
											int oldIndex = oldParent.Components.IndexOf( droppingComponent );

											droppingComponent.Parent.RemoveComponent( droppingComponent, false );
											targetComponent.AddComponent( droppingComponent );

											var action = new UndoActionComponentMove( DocumentWindow.Document2, droppingComponent, oldParent, oldIndex );
											DocumentWindow.Document2.CommitUndoAction( action );
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
										var action = new UndoActionComponentCreateDelete( DocumentWindow.Document2, newObjects, true );
										DocumentWindow.Document2.UndoSystem.CommitAction( action );
										DocumentWindow.Document2.Modified = true;
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
													var action = new UndoActionComponentCreateDelete( DocumentWindow.Document2, newObjects, true );
													DocumentWindow.Document2.CommitUndoAction( action );
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

													var action = new UndoActionComponentMove( DocumentWindow.Document2, droppingComponent, oldParent, oldIndex );
													DocumentWindow.Document2.CommitUndoAction( action );
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
									ContentBrowserUtility.CutCopyFiles( new string[] { droppingFileItem.FullPath }, !copy, targetFolder );
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
			toolStripButtonShowMembers.Checked = !toolStripButtonShowMembers.Checked;

			if( Initialized )
				UpdateData();
		}

		[Browsable( false )]
		public bool ShowMembers
		{
			get
			{
				if( Mode == ModeEnum.Objects )
					return toolStripButtonShowMembers.Checked;
				else
					return true;
			}
		}

		public void SetShowMembersModeObjects( bool value )
		{
			toolStripButtonShowMembers.Checked = value;
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

		//ICollection<TreeNodeAdv> FindTreeViewNodes( ICollection<Node> findNodes )
		//{
		//	//!!!!slowly

		//	var setOfNodes = new ESet<Node>( findNodes );
		//	var result = new List<TreeNodeAdv>();

		//	foreach( var treeViewNode in treeView.AllNodes )
		//	{
		//		var node = treeViewNode.Tag as Node;
		//		if( node != null && setOfNodes.Contains( node ) )
		//			result.Add( treeViewNode );
		//	}

		//	return result;
		//}

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
				if( GetSelectedComponents().All( c => c.Parent != null && EditorAPI2.GetDocumentByObject( c ) != null ) )
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
						if( EditorForm.Instance != null )
						{
							var paths = WindowsClipboard.GetFileDropPaths( EditorForm.Instance.Handle );
							if( paths.Count > 0 )
							{
								filePaths = paths.ToArray();
								cut = WindowsClipboard.IsCutPrefferdDropEffect( EditorForm.Instance.Handle );
								destinationFolder = fileItem.FullPath;
								return true;
							}
						}
#endif
					}
				}

				//_Component
				var componentItem = selectedItems[ 0 ] as ContentBrowserItem_Component;
				if( componentItem != null && !readOnlyHierarchy && EditorAPI2.GetDocumentByObject( componentItem.Component ) != null )
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

		public void Paste()
		{
			if( CanPaste( out string[] filePaths, out bool cut, out string destinationFolder, out Component destinationParent ) )
			{
				//_File
				if( filePaths != null )
					ContentBrowserUtility.CutCopyFiles( filePaths, cut, destinationFolder );

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
							if( data.documentWindow.Document2 != DocumentWindow.Document2 )
							{
								//another document
								{
									var action = new UndoActionComponentCreateDelete( data.documentWindow.Document2, components, false );
									data.documentWindow.Document2.UndoSystem.CommitAction( action );
									data.documentWindow.Document2.Modified = true;
								}
								{
									var action = new UndoActionComponentCreateDelete( DocumentWindow.Document2, newObjects, true );
									DocumentWindow.Document2.UndoSystem.CommitAction( action );
									DocumentWindow.Document2.Modified = true;
								}
							}
							else
							{
								//same document
								var multiAction = new UndoMultiAction();
								multiAction.AddAction( new UndoActionComponentCreateDelete( DocumentWindow.Document2, components, false ) );
								multiAction.AddAction( new UndoActionComponentCreateDelete( DocumentWindow.Document2, newObjects, true ) );
								DocumentWindow.Document2.UndoSystem.CommitAction( multiAction );
								DocumentWindow.Document2.Modified = true;
							}
						}
						else
						{
							//copy
							var action = new UndoActionComponentCreateDelete( DocumentWindow.Document2, newObjects, true );
							DocumentWindow.Document2.UndoSystem.CommitAction( action );
							DocumentWindow.Document2.Modified = true;
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
				if( fileItem != null && fileItem.FullPath != VirtualFileSystem.Directories.Assets )
					return true;

				//_Component
				var componentItem = selectedItems[ 0 ] as ContentBrowserItem_Component;
				if( componentItem != null && documentWindow != null && documentWindow.Document2 != null && EditorAPI2.GetDocumentByObject( componentItem.Component ) != null )
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
										ContentBrowserUtility.CutCopyFiles( new string[] { fileItem.FullPath }, true, newPath, true, true );

										//var tempName = newPath + "_";
										//while( Directory.Exists( tempName ) )
										//	tempName += "_";
										//Directory.Move( fileItem.FullPath, tempName );
										//Directory.Move( tempName, newPath );
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
									{
										ContentBrowserUtility.CutCopyFiles( new string[] { fileItem.FullPath }, true, newPath, true );
										//Directory.Move( fileItem.FullPath, newPath );
									}
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

					//select without file extension
					if( !fileItem.IsDirectory )
					{
						try
						{
							var s = Path.GetFileNameWithoutExtension( form.TextBoxText );
							form.TextBoxName.Select( 0, s.Length );
						}
						catch { }
					}

					form.ShowDialog();
				}

				//_Component
				var componentItem = item as ContentBrowserItem_Component;
				if( componentItem != null )
				{
					var component = componentItem.Component;
					EditorUtility2.ShowRenameComponentDialog( component );
				}
			}
		}

		public string GetDirectoryPathOfSelectedFileOrParentDirectoryItem()
		{
			ContentBrowserItem_File currentOrParentDirectoryItem = null;

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

			if( currentOrParentDirectoryItem != null )
				return currentOrParentDirectoryItem.FullPath;
			//if( currentOrParentDirectoryItem != null && VirtualPathUtility.GetVirtualPathByReal( currentOrParentDirectoryItem.FullPath, false, out string dummy ) )
			//	return currentOrParentDirectoryItem.FullPath;

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
			if( !string.IsNullOrEmpty( directoryPath ) )
			{
				var insideAssets = false;
				try
				{
					var virtualPath = VirtualPathUtility.GetVirtualPathByReal( directoryPath, true );
					insideAssets = directoryPath == VirtualFileSystem.Directories.Assets || !string.IsNullOrEmpty( virtualPath );
				}
				catch { }

				if( insideAssets )
					return true;
			}

			return false;

			//directoryPath = GetDirectoryPathOfSelectedFileOrParentDirectoryItem();
			//return !string.IsNullOrEmpty( directoryPath );
		}

		void NewResource( Metadata.TypeInfo lockType, bool assetsFolderOnly )
		{
			if( CanNewResource( out string directoryPath ) )
			{
				var initData = new NewObjectWindow.CreationDataClass();
				initData.initFileCreationDirectory = VirtualPathUtility.GetVirtualPathByReal( directoryPath, assetsFolderOnly );
				//initData.initDemandedType = MetadataManager.GetTypeOfNetType( typeof( Component ) );
				initData.createdFromContentBrowser = this;

				initData.initLockType = lockType;

				EditorAPI2.OpenNewObjectWindow( initData );
			}
		}

		bool CanNewTextFileWhenOutsideAssets( out string directoryPath )
		{
			directoryPath = GetDirectoryPathOfSelectedFileOrParentDirectoryItem();
			return !string.IsNullOrEmpty( directoryPath );
		}

		void NewTextFileWhenOutsideAssets()
		{
			if( CanNewTextFileWhenOutsideAssets( out string directoryPath ) )
			{
				string realFileName;
				for( int n = 1; ; n++ )
				{
					realFileName = Path.Combine( directoryPath, string.Format( "New File{0}.txt", n > 1 ? n.ToString() : "" ) );
					if( !File.Exists( realFileName ) )
						break;
				}

				try
				{
					File.WriteAllText( realFileName, "" );
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
					return;
				}

				//select and open
				EditorAPI2.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { realFileName } );
				EditorAPI2.OpenFileAsDocument( realFileName, true, true );
			}
		}

		//bool CanImportResource( out string directoryPath )
		//{
		//	directoryPath = GetDirectoryPathOfSelectedFileOrParentDirectoryItem();

		//	if( !string.IsNullOrEmpty( directoryPath ) )
		//	{
		//		var virtualPath = VirtualPathUtility.GetVirtualPathByReal( directoryPath, true );
		//		var insideAssets = directoryPath == VirtualFileSystem.Directories.Assets || !string.IsNullOrEmpty( virtualPath );
		//		if( insideAssets )
		//			return true;
		//	}

		//	return false;

		//	//return !string.IsNullOrEmpty( directoryPath );
		//}

		//void ImportResource()
		//{
		//	if( CanImportResource( out string directoryPath ) )
		//		EditorAPI.OpenImportWindow( VirtualPathUtility.GetVirtualPathByReal( directoryPath, true ) );
		//}

		private void TreeView_DrawRowEvent( TreeViewAdv treeView, TreeNodeAdv node, ref bool skip )
		{
			//skip drawing of removed component
			var itemNode = GetItemTreeNode( node );
			if( itemNode != null )
			{
				var item = itemNode.item;
				if( item != null )
				{
					var component = item.ContainedObject as Component;
					if( component != null )
					{
						if( component.ParentRoot != rootObject && component.ParentRoot?.HierarchyController == null && item.Parent != null )
							skip = true;
					}
				}
			}
		}

		private void TreeView_DrawIconAddition( TreeViewAdv treeView, TreeNodeAdv node, DrawContext context, Image image, System.Drawing.Rectangle r, float factor )
		{
			var itemNode = GetItemTreeNode( node );
			if( itemNode != null )
			{
				var item = itemNode.item;
				if( item != null )
				{
					var cornerImages = item.GetCornerImages();
					if( cornerImages != null )
					{
						var size = new Vector2F( image.Width, image.Height ) * ( factor >= 0 ? factor : 1 );

						for( int n = 0; n < cornerImages.Length; n++ )
						{
							var cornerImage = cornerImages[ n ];
							if( cornerImage != null )
							{
								//!!!!antialiasing. set mode or scale icons

								var scaledSizeInteger = new Vector2F( size.X / 1.5f, size.Y / 1.5f ).ToVector2I();
								context.Graphics.DrawImage( cornerImage, r.X, r.Bottom - scaledSizeInteger.Y, scaledSizeInteger.X, scaledSizeInteger.Y );

								//context.Graphics.DrawImage( cornerImage, (int)( r.X + size.X * n / 1.5 ), (int)( r.Y + size.Y / 4.0 ), (int)( size.X / 1.5 ), (int)( size.Y / 1.5 ) );

								//context.Graphics.DrawImage( cornerImage, r.X + size.X * n / 2, r.Y + size.Y / 2, size.X / 2, size.Y / 2 );

								////context.Graphics.DrawImage( image2, r.X, r.Y + size.Y * 2 / 3, size.X / 3, size.Y / 3 );

								////if( factor < 0.0f )
								////	context.Graphics.DrawImage( image, r );
								////else if( factor == 1.0f )
								////	context.Graphics.DrawImage( image, r.X, r.Y, image.Width, image.Height );
								////else
								////	context.Graphics.DrawImage( image, r.X, r.Y, image.Width * factor, image.Height * factor );
							}
						}
					}
				}
			}
		}

		private void nodeTextBox1_DrawText( object sender, Internal.Aga.Controls.Tree.NodeControls.DrawTextEventArgs e )
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

		//[Browsable( false )]
		//public ContentBrowserItem_File DataItem
		//{
		//	get { return dataItem; }
		//}

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
				//			foreach( var childItem in selectedItem.GetChildrenFiltered( true ) )
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

							foreach( var childItem in selectedItem.GetChildrenFilter( true ) )
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
				if( item.GetChildrenFilter( true ).Count == 0 || item.chooseByDoubleClickAndReturnKey )
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
					if( item.GetChildrenFilter( true ).Count != 0 )
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
				var shortcuts = EditorAPI2.GetActionShortcuts( "Rename" );
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
				if( EditorAPI2.ProcessShortcuts( e.KeyCode, true ) )
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

			int demandedImageSize = (int)( (float)( demandedListMode == ListModeEnum.Tiles ? options.TileImageSize : options.ListImageSize ) * EditorAPI2.DPIScale );
			var demandedListTileColumnWidth = (int)( (float)options.ListColumnWidth * EditorAPI2.DPIScale );

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

				listView.Mode?.Init();

				UpdateListImages();
			}
		}

		void UpdateListItemImage( EngineListView.Item listItem )
		{
			var item = (Item)listItem.Tag;

			Image image = null;

			//preview image
			var fileItem = item as ContentBrowserItem_File;
			if( fileItem != null && !fileItem.IsDirectory )
				image = PreviewImagesManager.GetImageForResource( fileItem.FullPath, false );

			if( image == null )
				image = item.image;
			if( image == null )
				image = imageHelper.GetImage( item.imageKey, currentListImageSizeScaled, item.ShowDisabled );
			if( image == null )
				image = EditorImageHelperBasicImages.Helper.GetImage( item.imageKey, currentListImageSizeScaled, item.ShowDisabled );

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

		public void UpdateBreadcrumb()
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
					breadCrumb.RootItem.ShortText = EditorLocalization2.Translate( "General", "Root" );

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

			EditorForm.Instance?.WorkspaceController.BlockAutoHideAndDoAction( this, () =>
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
				var item = new KryptonContextMenuItem( EditorLocalization2.Translate( "ContentBrowser.FilteringMode", "Basic" ), null,
				   delegate ( object s, EventArgs e2 )
				   {
					   FilteringMode = null;
				   } );
				item.Checked = filteringMode == null;
				items.Add( item );
			}

			foreach( var mode in filteringModes )
			{
				var item = new KryptonContextMenuItem( EditorLocalization2.Translate( "ContentBrowser.FilteringMode", mode.Name ), null,
				   delegate ( object s, EventArgs e2 )
				   {
					   FilteringMode = (ContentBrowserFilteringMode)( (KryptonContextMenuItem)s ).Tag;
				   } );
				item.Checked = filteringMode == mode;
				item.Tag = mode;
				items.Add( item );
			}

			EditorContextMenuWinForms.AddActionsToMenu( EditorActionContextMenuType.None, items );

			ShowContextMenuEvent?.Invoke( this, null, items );

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

		public List<Item> GetItemsByListView()
		{
			var result = new List<Item>();

			for( int n = 0; n < listView.Items.Count; n++ )
			{
				//if( listView.GetItemRectangle( n, out _ ) )
				//{
				var listItem = listView.Items[ n ];
				var item = listItem.Tag as Item;
				if( item != null )
					result.Add( item );
				//}
			}

			return result;
		}

		//!!!!GetVisibleItems
		public List<Item> GetVisibleItemsByListView()
		{
			var result = new List<Item>();

			for( int n = 0; n < listView.Items.Count; n++ )
			{
				if( listView.GetItemRectangle( n, out _ ) )
				{
					var listItem = listView.Items[ n ];
					var item = listItem.Tag as Item;
					if( item != null )
						result.Add( item );
				}
			}

			return result;
		}

		bool FavoritesCheckEnableItem( bool addToFavorites )
		{
			foreach( var item in SelectedItems )
			{
				var typeItem = item as ContentBrowserItem_Type;
				if( typeItem != null && MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( typeItem.Type ) )
				{
					// ||
					//MetadataManager.GetTypeOfNetType( typeof( NewResourceType ) ).IsAssignableFrom( typeItem.Type );

					var contains = EditorFavorites.Contains( typeItem.Type.Name );
					if( addToFavorites && !contains || !addToFavorites && contains )
						return true;
				}

				var fileItem = item as ContentBrowserItem_File;
				if( fileItem != null )
				{
					var contains = EditorFavorites.Contains( VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath ) );
					if( addToFavorites && !contains || !addToFavorites && contains )
						return true;
				}
			}

			return false;
		}

		void FavoritesAddSelectedItems()
		{
			foreach( var item in SelectedItems )
			{
				var typeItem = item as ContentBrowserItem_Type;
				if( typeItem != null )
					EditorFavorites.Add( typeItem.Type.Name );

				var fileItem = item as ContentBrowserItem_File;
				if( fileItem != null )
					EditorFavorites.Add( VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath ) );
			}
		}

		void FavoritesRemoveSelectedItems()
		{
			foreach( var item in SelectedItems )
			{
				var typeItem = item as ContentBrowserItem_Type;
				if( typeItem != null )
					EditorFavorites.Remove( typeItem.Type.Name );

				var fileItem = item as ContentBrowserItem_File;
				if( fileItem != null )
					EditorFavorites.Remove( VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath ) );
			}
		}

		public void UpdateAllTypesItem( ContentBrowserItem_Virtual specifiedAllTypesItem = null, bool callPerformChildrenChanged = true )
		{
			var allTypesItem = specifiedAllTypesItem;
			if( allTypesItem == null )
				allTypesItem = rootItems.FirstOrDefault( i => i.Tag as string == "All types" ) as ContentBrowserItem_Virtual;
			if( allTypesItem == null )
				return;

			allTypesItem.children.Clear();

			//!!!!slowly. сразу раскладывать по неймспейсам можно
			//!!!!!!можно еще не проверять какие-то сборки, если нужны только Component (от которых зависит NeoAxis.Core.dll)
			var types = new List<Metadata.NetTypeInfo>( 16384 );
			var namespacesSet = new ESet<string>();

			foreach( var type in MetadataManager.GetNetTypes() )
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
						if( !setReferenceModeData.selectTypeDemandedTypes.Any( t => ContentBrowserUtility.ContainsType( t.GetNetType(), netType ) ) )
							skip = true;

						//if( !ContentBrowserUtility.ContainsType( setReferenceModeData.selectTypeDemandedType.GetNetType(), netType ) )
						//	skip = true;
						////if( !ContentBrowserUtils.ContainsType( setReferenceModeData.demandedType.GetNetType(), netType ) )
						////	skip = true;
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
			var namespacesList = new List<string>( namespacesSet );
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
				var namespaceItem = new ContentBrowserItem_Virtual( this, allTypesItem, ns );
				namespaceItem.imageKey = "Namespace";
				allTypesItem.children.Add( namespaceItem );

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
				SetReference_RemoveExcessTypeItems( allTypesItem );

			//remove excess classes in Resources and filtering mode
			if( Mode == ModeEnum.Resources && FilteringMode != null )
			{
				Resources_RemoveExcessClassesItemsByFilteringMode( allTypesItem );

				//!!!!может не так
				//expand at startup class items for filtering mode
				var children = allTypesItem.GetChildrenFilter( true );
				if( children.Count == 1 )
				{
					allTypesItem.expandAtStartup = true;
					if( children[ 0 ].GetChildrenFilter( true ).Count <= 10 )
						children[ 0 ].expandAtStartup = true;
				}
			}

			//expand for select type mode
			if( Mode == ModeEnum.SetReference && setReferenceModeData.selectTypeWindow )
			{
				var children = allTypesItem.GetChildrenFilter( true );
				if( children.Count < 20 )
				{
					allTypesItem.expandAtStartup = true;
					if( children.Count == 1 )
					{
						if( children[ 0 ].GetChildrenFilter( true ).Count <= 10 )
							children[ 0 ].expandAtStartup = true;
					}
				}
			}

			if( callPerformChildrenChanged )
				allTypesItem.PerformChildrenChanged();
		}

		public void UpdateDataIfResourcesWindowTypesChanged()
		{
			var newList = new List<Type>();

			foreach( var item in ResourcesWindowItems.Items )
			{
				//custom filtering
				if( !EditorUtility2.PerformResourcesWindowItemVisibleFilter( item ) )
					continue;

				newList.Add( item.Type );
			}

			if( !Enumerable.SequenceEqual( resourcesWindowTypesAdded, newList ) )
				UpdateData();
		}

	}
}

#endif