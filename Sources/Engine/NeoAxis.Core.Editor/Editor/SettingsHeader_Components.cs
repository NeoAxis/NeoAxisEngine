#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Workspace;
using Internal.ComponentFactory.Krypton.Docking;
using NeoAxis;

namespace NeoAxis.Editor
{
	public partial class SettingsHeader_Components : SettingsHeader
	{
		//!!!!было
		//!!!!не надо. в SettingsHeader есть
		//DocumentWindow document;

		//!!!!может нормально встроить. какие-то настройки в свойствах настраивать
		ContentBrowser componentsBrowser;

		//

		public SettingsHeader_Components()
		{
			InitializeComponent();
		}

		//!!!!было
		//public void Init( DocumentWindow document )
		//{
		//	this.document = document;
		//}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			Component root = null;
			//!!!!а если != 1
			if( SettingsPanel.selectedObjects.Length == 1 )
				root = SettingsPanel.selectedObjects[ 0 ] as Component;

			componentsBrowser = new ContentBrowser();
			componentsBrowser.TreeViewBorderDraw = BorderSides.All;
			componentsBrowser.Mode = ContentBrowser.ModeEnum.Objects;
			componentsBrowser.Options.FilteringModeButton = false;
			//!!!!
			//componentsBrowser.Options.SearchBar = false;
			componentsBrowser.Options.DisplayPropertiesSortFilesBy = false;
			componentsBrowser.Options.DisplayPropertiesOpenButton = false;
			componentsBrowser.ThisIsSettingsWindow = true;
			componentsBrowser.MultiSelect = true;
			componentsBrowser.ItemAfterSelect += ComponentsBrowser_ItemAfterSelect;
			componentsBrowser.ItemAfterChoose += ComponentsBrowser_ItemAfterChoose;

			componentsBrowser.Init( SettingsPanel.documentWindow, root, /*null, */null );
			//componentsBrowser.Init( document, root );
			componentsBrowser.Dock = DockStyle.Fill;
			Controls.Add( componentsBrowser );
		}

		private void ComponentsBrowser_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			//!!!!
			//selectedNode.SetMultiSelected( true, false );

			var selectObjects = new List<object>();

			foreach( var item in items )
			{
				var componentItem = item as ContentBrowserItem_Component;
				if( componentItem != null && componentItem.Component != null )
					selectObjects.Add( componentItem.Component );
			}

			if( selectObjects.Count == 0 )
			{
				//!!!!может рутовый выделять
			}

			SettingsPanel.GetControl<SettingsLevel2Window>().SelectObjects( SettingsPanel.documentWindow, selectObjects );
		}

		private void ComponentsBrowser_ItemAfterChoose( ContentBrowser sender, ContentBrowser.Item item, ref bool handled )
		{
		}

		//!!!!!
		public void SelectObjects( Component[] objs )
		{
			//!!!!!slowly

			ESet<Component> objsSet = new ESet<Component>( objs );

			List<ContentBrowser.Item> items = new List<ContentBrowser.Item>();

			foreach( var item in componentsBrowser.GetAllItems() )
			{
				ContentBrowserItem_Component item2 = item as ContentBrowserItem_Component;
				if( item2 != null )
				{
					if( objsSet.Contains( item2.Component ) )
						items.Add( item );
				}
			}

			componentsBrowser.SelectItems( items.ToArray() );
		}

		public int CalculateHeight()
		{
			return componentsBrowser != null ? componentsBrowser.CalculateHeight() : -1;
		}
	}
}

#endif