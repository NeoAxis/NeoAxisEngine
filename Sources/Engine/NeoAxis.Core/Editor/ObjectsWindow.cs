#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Objects Window.
	/// </summary>
	public partial class ObjectsWindow : ForeachDocumentWindowContainer
	{
		ContentBrowserOptions sharedOptions;

		//

		public ObjectsWindow()
		{
			InitializeComponent();

			//Config_Load();
			EngineConfig.SaveEvent += Config_SaveEvent;

			WindowTitle = EditorLocalization.Translate( "Windows", WindowTitle );
		}

		public override bool HideOnRemoving { get { return true; } }

		protected override void OnCreatePanelControl( PanelData panel )
		{
			base.OnCreatePanelControl( panel );

			var component = panel.documentWindow.Document?.ResultComponent;
			//var component = panel.documentWindow.ObjectOfWindow as Component;
			//if( panel.documentWindow.LoadedResource != null )
			//	component = panel.documentWindow.GetDocumentObject() as Component;

			if( component != null )
			{
				var browser = new ContentBrowser();// documentObject as Component );

				if( sharedOptions != null )
					browser.Options = sharedOptions;
				else
				{
					Config_Load( browser );
					sharedOptions = browser.Options;
				}

				browser.Mode = ContentBrowser.ModeEnum.Objects;
				browser.CanSelectObjectSettings = true;
				browser.Options.FilteringModeButton = false;
				browser.Options.DisplayPropertiesSortFilesBy = false;
				browser.Options.DisplayPropertiesOpenButton = false;
				browser.TreeViewBorderDraw = BorderSides.Top;
				browser.ListViewBorderDraw = BorderSides.Top;
				browser.MultiSelect = true;
				//!!!!было до redesign
				//browser.ItemAfterSelect += ContentBrowser_ItemAfterSelect;
				//browser.ItemAfterChoose += ContentBrowser_ItemAfterChoose;

				browser.Init( panel.documentWindow, component, /*null,*/ null );
				panel.control = browser;
				panel.control.Dock = DockStyle.Fill;
				Controls.Add( panel.control );
				browser.ItemAfterSelect += Browser_ItemAfterSelect;

				SelectItemsOfSelectedObjects( panel.documentWindow );
			}
			else
			{
				panel.control = new EUserControl();
				panel.control.Dock = DockStyle.Fill;
				Controls.Add( panel.control );
			}
		}

		private void Browser_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			if( selectedByUser )
			{
				var selectObjects = new List<object>();

				foreach( var item in items )
				{
					var documentWindow = item.Owner.DocumentWindow;
					if( documentWindow != null )
					{
						var panel = GetPanel( documentWindow );
						if( panel != null )
						{
							var componentItem = item as ContentBrowserItem_Component;
							if( componentItem != null )
								selectObjects.Add( componentItem.Component );
						}
					}
				}

				sender.DocumentWindow?.SelectObjects( selectObjects, false );
			}
		}

		public override void OnDocumentWindowSelectedObjectsChangedByUser( DocumentWindow documentWindow )
		{
			base.OnDocumentWindowSelectedObjectsChangedByUser( documentWindow );

			SelectItemsOfSelectedObjects( documentWindow );
		}

		void SelectItemsOfSelectedObjects( DocumentWindow documentWindow )
		{
			var panel = GetPanel( documentWindow );
			if( panel != null )
			{
				var browser = panel.control as ContentBrowser;
				if( browser != null )
				{
					var items = new List<ContentBrowser.Item>();

					if( documentWindow.SelectedObjects.Length != 0 )
					{
						//precreate and expand all parents' items
						{
							var allParents = new ESet<Component>();
							foreach( var obj in documentWindow.SelectedObjects )
							{
								var c = obj as Component;
								if( c != null )
									allParents.AddRangeWithCheckAlreadyContained( c.GetAllParents( false ) );
							}
							foreach( var c in allParents )
							{
								var i = browser.FindItemByContainedObject( c );
								if( i != null )
									browser.SelectItems( new ContentBrowser.Item[] { i }, true, false );
							}
						}

						//get items to select
						foreach( var item in browser.GetAllItems() )
						{
							var componentItem = item as ContentBrowserItem_Component;
							if( componentItem != null && documentWindow.SelectedObjectsSet.Contains( componentItem.Component ) )
								items.Add( item );
						}
					}

					browser.SelectItems( items.ToArray(), items.Count == 1 );
					//browser.SelectItems( items.ToArray() );
				}
			}
		}

		//!!!!было до redesign
		//private void ContentBrowser_ItemAfterSelect( ContentBrowser.ItemTreeNode node, ref bool handled )
		//{
		//	if( node != null )
		//	{
		//		//!!!!
		//		//selectedNode.SetMultiSelected( true, false );

		//		bool selected = false;

		//		//!!!!!обязательно компонетна?
		//		ContentBrowserItem_Component componentItem = node.item as ContentBrowserItem_Component;
		//		if( componentItem != null && componentItem.Component != null )
		//		{
		//			xx xx;

		//			EditorForm.Instance.SelectObjectSettings( componentItem.Owner.DocumentWindow, new object[] { componentItem.Component } );
		//			selected = true;
		//		}

		//		if( !selected )
		//		{
		//			xx xx;

		//			EditorForm.Instance.SelectObjectSettings( null, null );
		//		}

		//		//!!!!
		//		//if( MainForm.Instance != null )
		//		//	MainForm.Instance.UpdateLastSelectedResourcePath( GetNodePath( selectedNode ) );
		//	}
		//}

		//!!!!было до redesign
		//private void ContentBrowser_ItemAfterChoose( ContentBrowser.ItemTreeNode node, ref bool handled )
		//{
		//	//!!!!

		//	////!!!!
		//	////selectedNode.SetMultiSelected( true, false );

		//	//ContentBrowser_FileItem fileItem = node.item as ContentBrowser_FileItem;
		//	//if( fileItem != null && !fileItem.isDirectory )
		//	//{
		//	//	//!!!!!

		//	//	//!!!!не открывать одно и тоже несколько раз. хотя, по сути сцене можно же

		//	//	var ext = Path.GetExtension( fileItem.path ).ToLower();

		//	//	//!!!!
		//	//	//Resource type: Scene definition. а не сама загрузка. дефинишен будет в одном экземпляре всегда

		//	//	if( ResourceManager.GetResourceTypeByFileExtension( ext ) != null )
		//	//	{
		//	//		//!!!!!temp. тут меньше проверок, чем в AfterSelect

		//	//		EditorRoot.OpenResourceOrSceneAsDocument( fileItem.path, true );
		//	//		handled = true;
		//	//	}
		//	//	else if( ext == ".scene" )
		//	//	{
		//	//		EditorRoot.OpenResourceOrSceneAsDocument( fileItem.path, true );
		//	//		handled = true;
		//	//	}
		//	//	else
		//	//	{
		//	//	}
		//	//}

		//	////!!!!!

		//	////Item item;
		//	////if( itemByNode.TryGetValue( node, out item ) )
		//	////{
		//	////	var fileItem = item as ContentBrowser_FileItem;
		//	////	if( fileItem != null )
		//	////	{
		//	////		EditorRoot.OpenResourceAsDocument( fileItem.path, true );

		//	////		//!!!!!!тут? так?
		//	////		return true;
		//	////	}
		//	////}

		//}

		public override ObjectsInFocus GetObjectsInFocus()
		{
			var panel = SelectedPanel;
			if( panel != null )
			{
				var browser = panel.control as ContentBrowser;
				if( browser != null )
					return new ObjectsInFocus( browser.DocumentWindow, browser.GetSelectedContainedObjects() );
			}
			return null;
		}

		void Config_Load( ContentBrowser browser )
		{
			var windowBlock = EngineConfig.TextBlock.FindChild( "ObjectsWindow" );
			if( windowBlock != null )
			{
				var browserBlock = windowBlock.FindChild( "ContentBrowser" );
				if( browserBlock != null )
				{
					browser.Options.Load( browserBlock );
					//browser.LoadSettings( browserBlock );
				}
			}
		}

		void Config_SaveEvent()
		{
			var browser = SelectedPanel?.control as ContentBrowser;
			if( browser != null )
			{
				var configBlock = EngineConfig.TextBlock;

				var old = configBlock.FindChild( "ObjectsWindow" );
				if( old != null )
					configBlock.DeleteChild( old );

				var windowBlock = configBlock.AddChild( "ObjectsWindow" );
				var browserBlock = windowBlock.AddChild( "ContentBrowser" );
				browser.Options.Save( browserBlock );
				//browser.SaveSettings( browserBlock );
			}
		}
	}
}

#endif