#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

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

			WindowTitle = EditorLocalization2.Translate( "Windows", WindowTitle );
		}

		public override bool HideOnRemoving { get { return true; } }

		protected override void OnCreatePanelControl( PanelData panel )
		{
			base.OnCreatePanelControl( panel );

			var component = panel.documentWindow.Document2?.ResultComponent;
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
									allParents.AddRangeWithCheckAlreadyContained( c.GetAllParents() );
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

					var showMembersString = browserBlock.GetAttribute( nameof( ContentBrowser.ShowMembers ) );
					if( !string.IsNullOrEmpty( showMembersString ) && bool.TryParse( showMembersString, out var value ) )
						browser.SetShowMembersModeObjects( value );
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

				browserBlock.SetAttribute( nameof( ContentBrowser.ShowMembers ), browser.ShowMembers.ToString() );
			}
		}
	}
}

#endif