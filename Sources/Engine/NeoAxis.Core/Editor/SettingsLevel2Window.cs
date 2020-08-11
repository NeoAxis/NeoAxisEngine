// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class SettingsLevel2Window : EUserControl
	{
		//!!!!
		const int maxCachedCount = 10;

		public class PanelData : PanelDataWithTableLayout
		{
		}
		List<PanelData> panels = new List<PanelData>();

		PanelData selectedPanel;

		//

		public SettingsLevel2Window()
		{
			InitializeComponent();
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

		protected override void OnDestroy()
		{
			RemoveCachedPanels();

			base.OnDestroy();
		}

		PanelData CreatePanel( DocumentWindow document, object[] key, bool willSelected )
		{
			PanelData panel = new PanelData();
			panels.Add( panel );
			panel.selectedObjects = key;

			panel.CreateAndAddPanel( this );

			//hide
			if( !willSelected && panel.layoutPanel != null )
			{
				panel.layoutPanel.Visible = false;
				panel.layoutPanel.Enabled = false;
			}

			SettingsProvider.Create( document, panel.selectedObjects, panel.layoutPanel, null, true );

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

		public void SelectObjects( DocumentWindow document, IList<object> objects )
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

				panel = CreatePanel( document, key, true );
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

				//!!!!тут ли
				//PreviewWindow.Instance?.SelectObjects( selectedPanel );
			}
		}
	}
}