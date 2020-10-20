// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Object Settings Window.
	/// </summary>
	public partial class ObjectSettingsWindow : DocumentWindow
	{
		static string projectSettingsUserModeLastSelectedTab = "General";

		const int maxCachedCount = 10;

		//DocumentWindow documentWindow;
		//object rootObject;

		List<PanelData> panels = new List<PanelData>();

		PanelData selectedPanel;

		bool readOnlyHierarchy;

		bool needRecreatePanels;
		object[] needRecreatePanelsSelectObjects;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public class PanelData : PanelDataWithTableLayout
		{
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public ObjectSettingsWindow()
		{
			InitializeComponent();

			objectsBrowser1.Options.FilteringModeButton = false;
			objectsBrowser1.Options.DisplayPropertiesSortFilesBy = false;
			objectsBrowser1.Options.DisplayPropertiesOpenButton = false;

			if( EditorAPI.DarkTheme )
			{
				kryptonSplitContainer1.StateNormal.Back.Color1 = Color.FromArgb( 54, 54, 54 );

				//var container = child as KryptonSplitContainer;
				//if( container != null )
				//	container.StateNormal.Back.Color1 = Color.FromArgb( 40, 40, 40 );
			}

			EditorLocalization.TranslateForm( "ObjectSettingsWindow", this );
		}

		public override void InitDocumentWindow( DocumentInstance document, object objectOfWindow, bool openAsSettings, Dictionary<string, object> windowTypeSpecificOptions )
		{
			base.InitDocumentWindow( document, objectOfWindow, openAsSettings, windowTypeSpecificOptions );

			if( objectsBrowser1 != null )
				objectsBrowser1.Init( this, objectOfWindow, /*null, */null );

			//!!!!
			//special for Project Settings
			if( document.SpecialMode == "ProjectSettingsUserMode" )
				ReadOnlyHierarchy = true;

			if( document.SpecialMode != "ProjectSettingsUserMode" )
			{
				kryptonButtonApply.Enabled = false;
				kryptonButtonApply.Visible = false;
			}

			if( document.SpecialMode == "ProjectSettingsUserMode" )
			{
				objectsBrowser1.OverrideItemText += ObjectsBrowser1_OverrideItemText;
			}
		}

		private void ObjectsBrowser1_OverrideItemText( ContentBrowser sender, ContentBrowser.Item item, ref string text )
		{
			text = EditorLocalization.Translate( "ProjectSettings.Page", text );
		}

		[Browsable( false )]
		public bool ReadOnlyHierarchy
		{
			get { return readOnlyHierarchy; }
			set
			{
				readOnlyHierarchy = value;
				if( objectsBrowser1 != null )
					objectsBrowser1.ReadOnlyHierarchy = value;
			}
		}

		private void ObjectSettingsWindow_Load( object sender, EventArgs e )
		{
			//!!!!!так?

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			var items = objectsBrowser1.GetAllItems();
			if( items.Length != 0 )
			{
				ContentBrowser.Item itemToSelect = null;
				if( Document != null && Document.SpecialMode == "ProjectSettingsUserMode" && projectSettingsUserModeLastSelectedTab != "" )
					itemToSelect = items.FirstOrDefault( i => i.Text == projectSettingsUserModeLastSelectedTab );
				if( itemToSelect == null )
					itemToSelect = items[ 0 ];

				objectsBrowser1.SelectItems( new ContentBrowser.Item[] { itemToSelect } );
			}

			timer1.Start();

			UpdateControlsLocation();
		}

		protected override void OnDestroy()
		{
			RemoveCachedPanels();

			base.OnDestroy();
		}

		void UpdateControlsLocation()
		{
			objectSettingsHeader_ObjectInfo1.Width = ClientRectangle.Width - 18;
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( KryptonPage == null || KryptonPage.Parent == null )
				return;
			//if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
			//	return;

			//recreate panels
			if( needRecreatePanels )
			{
				object[] selectedObjects = needRecreatePanelsSelectObjects;

				needRecreatePanels = false;
				needRecreatePanelsSelectObjects = null;

				RemoveCachedPanels();

				if( selectedObjects != null )
					SelectObjects( selectedObjects );//, this );
			}
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControlsLocation();
		}

		//[Browsable( false )]
		//public DocumentWindow DocumentWindow
		//{
		//	get { return documentWindow; }
		//}

		//[Browsable( false )]
		//public object RootObject
		//{
		//	get { return rootObject; }
		//}

		private void objectsBrowser1_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			//!!!!
			//selectedNode.SetMultiSelected( true, false );

			var selectObjects = new List<object>();

			foreach( var item in items )
			{
				if( item is ContentBrowserItem_Component )//|| node.item is ContentBrowserItem_NoSpecialization )
				{
					if( item.ContainedObject != null )
						selectObjects.Add( item.ContainedObject );
				}
			}

			if( selectObjects.Count == 0 )
				selectObjects.Add( ObjectOfWindow );

			SelectObjects( selectObjects );//, this );

			if( Document != null && Document.SpecialMode == "ProjectSettingsUserMode" )
			{
				if( items.Count == 1 )
					projectSettingsUserModeLastSelectedTab = items[ 0 ].Text;
				//else
				//	projectSettingsUserModeLastSelectedTab = "";
			}
		}

		private void objectsBrowser1_ItemAfterChoose( ContentBrowser sender, ContentBrowser.Item item, ref bool handled )
		{
		}

		bool IsEqual( object[] key1, object[] key2 )
		{
			if( key1.Length != key2.Length )
				return false;
			for( int n = 0; n < key1.Length; n++ )
			{
				if( key1[ n ] != key2[ n ] )
					return false;
			}
			return true;
		}

		PanelData GetPanel( object[] key )
		{
			foreach( var panel in panels )
			{
				if( IsEqual( panel.selectedObjects, key ) )
					return panel;
			}
			return null;
		}

		PanelData CreatePanel( /*DocumentWindow documentWindow, */object[] key, bool willSelected )
		{
			PanelData panel = new PanelData();
			panels.Add( panel );
			panel.selectedObjects = key;
			panel.CreateAndAddPanel( panelSettings );

			//hide
			if( !willSelected && panel.layoutPanel != null )
			{
				panel.layoutPanel.Visible = false;
				panel.layoutPanel.Enabled = false;
			}

			SettingsProvider.Create( this/*documentWindow*/, panel.selectedObjects, panel.layoutPanel, null, true );

			//!!!!!было
			////init panel
			//if( panel.selectedObjects != null && panel.selectedObjects.Length != 0 )//!!!!так?
			//{
			//try
			//{
			//	//!!!!
			//	//!!!!может раньше
			//	//!!!!где еще такое
			//	layoutPanel.SuspendLayout();

			//	//!!!!!

			//	//if( clear )
			//	//	Clear();

			//	//UpdateBegin?.Invoke( this );
			//	//AllProviders_UpdateBegin?.Invoke( this );

			//	//OnUpdate();

			//	xx xx;

			//	{
			//		var header = new SettingsHeader_ObjectInfo();
			//		header.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			//		layoutPanel.Controls.Add( header );
			//	}

			//	{
			//		var header = new SettingsHeader_Components();
			//		header.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			//		layoutPanel.Controls.Add( header );

			//		//!!!!!

			//		////!!!!не так видать. надо не создавать совсем контрол
			//		//bool showComponents = false;

			//		//if( panel.selectedObjects.Length == 1 )
			//		//{
			//		//	var obj = panel.selectedObjects[ 0 ] as Component;
			//		//	if( obj != null && obj.ShowComponentsInSettings )
			//		//		showComponents = true;
			//		//}

			//		////!!!!!
			//		//if( !showComponents )
			//		//	header.Visible = false;
			//	}

			//	//!!!!
			//	//separator
			//	{
			//		var header = new GroupBox();
			//		header.Size = new Size( 20, 20 );
			//		header.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			//		layoutPanel.Controls.Add( header );
			//	}

			//	{
			//		var window = new SettingsLevel2Window();
			//		window.Dock = DockStyle.Fill;
			//		layoutPanel.Controls.Add( window );
			//	}

			//	//select root of level 2
			//	//!!!!всегда Component? если нет, то и окна этого нет
			//	xx xx;
			//	if( panel.GetControl<SettingsHeader_Components>() != null )
			//		panel.GetControl<SettingsHeader_Components>().SelectObjects( new Component[] { (Component)panel.selectedObjects[ 0 ] } );

			//	//UpdateEnd?.Invoke( this );
			//	//AllProviders_UpdateEnd?.Invoke( this );

			//	//provider.PerformUpdate( false );
			//}
			//finally
			//{
			//	layoutPanel.ResumeLayout();
			//}
			//}

			return panel;
		}

		void RemovePanel( PanelData panel )
		{
			if( SelectedPanel == panel )
				SelectedPanel = null;

			var control = panel.layoutPanel;
			control.Parent.Controls.Remove( control );
			control.Dispose();

			panels.Remove( panel );
		}

		public void RemoveCachedPanels()
		{
			while( panels.Count != 0 )
				RemovePanel( panels[ panels.Count - 1 ] );
		}

		public void SelectObjects( IList<object> objects )//, DocumentWindow documentWindow )
		{
			if( EditorAPI.ClosingApplication )
				return;

			object[] key = new object[ objects.Count ];
			objects.CopyTo( key, 0 );

			PanelData panel = GetPanel( key );

			//find cached panel
			if( panel != null )
			{
				//move to last position (best priority)
				panels.Remove( panel );
				panels.Add( panel );
			}

			//create new panel
			if( panel == null )
			{
				//remove cached
				while( panels.Count >= maxCachedCount )
					RemovePanel( panels[ 0 ] );

				panel = CreatePanel( /*documentWindow, */key, true );
			}

			SelectedPanel = panel;

			//!!!!!
		}

		[Browsable( false )]
		public PanelData SelectedPanel
		{
			get { return selectedPanel; }
			set
			{
				if( selectedPanel == value )
					return;

				var old = selectedPanel;
				selectedPanel = value;

				if( selectedPanel != null )
				{
					selectedPanel.layoutPanel.Enabled = true;
					selectedPanel.layoutPanel.Visible = true;
					//selectedPanel.control.Focus();
				}

				if( old != null )
				{
					old.layoutPanel.Visible = false;
					old.layoutPanel.Enabled = false;
				}

				//if( selectedPanel != null )
				//{
				//	selectedPanel.layoutPanel.Visible = false;
				//	selectedPanel.layoutPanel.Enabled = false;
				//}

				//selectedPanel = value;

				//if( selectedPanel != null )
				//{
				//	selectedPanel.layoutPanel.Enabled = true;
				//	selectedPanel.layoutPanel.Visible = true;
				//	//selectedPanel.control.Focus();
				//}
			}
		}

		private void kryptonButtonOK_Click( object sender, EventArgs e )
		{
			Close();
		}

		public override ObjectsInFocus GetObjectsInFocus()
		{
			return new ObjectsInFocus( objectsBrowser1.DocumentWindow, objectsBrowser1.GetSelectedContainedObjects() );
		}

		protected override bool CanUpdateSettingsWindowsSelectedObjects()
		{
			return false;
		}

		private void kryptonButtonApply_Click( object sender, EventArgs e )
		{
			Document.Save();
		}

		//!!!!
		//protected override CreateParams CreateParams
		//{
		//	get
		//	{
		//		CreateParams handleParam = base.CreateParams;
		//		handleParam.ExStyle |= 0x02000000;//WS_EX_COMPOSITED       
		//		return handleParam;
		//	}
		//}

		protected override void OnKryptonPageParentChanged()
		{
			base.OnKryptonPageParentChanged();

			//recreate panels when parent of KryptonPage was changed. need to restore _COMPOSITED flag
			if( KryptonPage != null && KryptonPage.Parent == null )
			{
				if( SelectedPanel != null )
				{
					needRecreatePanels = true;
					needRecreatePanelsSelectObjects = SelectedPanel.selectedObjects;
					//else
					//	needRecreatePanelsSelectObjects = null;

					RemoveCachedPanels();
				}
			}
		}
	}

	public class PanelDataWithTableLayout
	{
		//!!!!public

		public object[] selectedObjects;
		public TableLayoutPanel layoutPanel;

		//

		public TableLayoutPanel CreateAndAddPanel( Control owner )
		{
			TableLayoutPanel layoutPanel = new TableLayoutPanel();
			layoutPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 50f ) );
			layoutPanel.Tag = this;
			layoutPanel.SetBounds( 0, 0, owner.Width, owner.Height );
			layoutPanel.Dock = DockStyle.Fill;

			owner.Controls.Add( layoutPanel );

			this.layoutPanel = layoutPanel;

			return layoutPanel;
		}

		public T GetControl<T>() where T : Control
		{
			if( layoutPanel != null )
			{
				foreach( Control c in layoutPanel.Controls )
					if( typeof( T ).IsAssignableFrom( c.GetType() ) )
						return (T)c;
			}
			return null;
		}
	}
}