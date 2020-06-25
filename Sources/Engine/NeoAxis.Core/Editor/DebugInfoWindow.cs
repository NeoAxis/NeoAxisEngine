// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Debug Info Window.
	/// </summary>
	public partial class DebugInfoWindow : DockWindow
	{
		string currentContent = "";

		List<ContentBrowser.Item> dataItems = new List<ContentBrowser.Item>();

		//System.Drawing.Font richTextBox1FontOriginal;
		//string richTextBox1FontCurrent = "";

		////!!!!не заюзано
		//public delegate void ProcessCmdKeyEventDelegate( OutputWindow sender, ref Message msg, Keys keyData, ref bool handled );
		//public event ProcessCmdKeyEventDelegate ProcessCmdKeyEvent;

		//

		public DebugInfoWindow()
		{
			InitializeComponent();

			contentBrowserData.RemoveTreeViewIconsColumn();

			WindowTitle = EditorLocalization.Translate( "Windows", WindowTitle );
		}

		public override bool HideOnRemoving { get { return true; } }

		private void richTextBox1_PreviewKeyDown( object sender, PreviewKeyDownEventArgs e )
		{
			//!!!!!!было в старом
			//if( e.KeyCode == Keys.F4 && e.Control )
			//	Hide();
		}

		//public RichTextBox RichTextBox
		//{
		//	get { return richTextBox1; }
		//}

		private void DebugInfoForm_Load( object sender, EventArgs e )
		{
			if( EditorUtility.IsDesignerHosted( this ) )
				return;

			//Translate();

			//richTextBox1.Select();
			UpdateList();

			timer1.Start();
		}

		//void Translate()
		//{
		//TabText = ToolsLocalization.Translate( "OutputForm", TabText );
		//}

		//public void UpdateFonts( string fontForm )
		//{
		//if( richTextBox1FontOriginal == null )
		//	richTextBox1FontOriginal = richTextBox1.Font;

		//if( richTextBox1FontCurrent != fontForm )
		//{
		//	if( !string.IsNullOrEmpty( fontForm ) && fontForm[ 0 ] != '(' )
		//	{
		//		try
		//		{
		//			System.Drawing.FontConverter fontConverter = new System.Drawing.FontConverter();
		//			richTextBox1.Font = (System.Drawing.Font)fontConverter.ConvertFromString( fontForm );
		//		}
		//		catch { }
		//	}
		//	else
		//		richTextBox1.Font = richTextBox1FontOriginal;

		//	richTextBox1FontCurrent = fontForm;
		//}
		//}

		//protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		//{
		//	//if( ProcessCmdKeyEvent != null )
		//	//{
		//	//	bool handled = false;
		//	//	ProcessCmdKeyEvent( this, ref msg, keyData, ref handled );
		//	//	if( handled )
		//	//		return true;
		//	//}

		//	return base.ProcessCmdKey( ref msg, keyData );
		//}

		//!!!!dynamic update
		void UpdateList()
		{
			var items = new List<ContentBrowser.Item>();

			ContentBrowserItem_Virtual firstItem = null;

			foreach( var page in DebugInfoPage.AllPages )
			{
				var item = new ContentBrowserItem_Virtual( contentBrowserList, null, EditorLocalization.Translate( "DebugInfoWindow", page.Title ) );
				item.Tag = page;
				items.Add( item );

				if( firstItem == null )
					firstItem = item;
			}

			contentBrowserList.SetData( items, false );
			contentBrowserList.SelectItems( new ContentBrowser.Item[] { firstItem } );
		}

		private void contentBrowserList_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			UpdateContent();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || EditorUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			UpdateContent();
		}

		void UpdateContent()
		{
			if( EditorUtility.IsDesignerHosted( this ) )
				return;

			var lines = new List<string>();
			if( contentBrowserList.SelectedItems.Length != 0 )
			{
				var page = (DebugInfoPage)( (ContentBrowserItem_Virtual)contentBrowserList.SelectedItems[ 0 ] ).Tag;
				lines = page.Content;
			}

			if( dataItems == null || dataItems.Count != lines.Count )
			{
				dataItems = new List<ContentBrowser.Item>();
				for( int n = 0; n < lines.Count; n++ )
				{
					var item = new ContentBrowserItem_Virtual( contentBrowserData, null, lines[ n ] );
					dataItems.Add( item );
				}
				contentBrowserData.SetData( dataItems, false );
			}
			else
			{
				for( int n = 0; n < lines.Count; n++ )
					( (ContentBrowserItem_Virtual)dataItems[ n ] ).SetText( lines[ n ] );
			}
		}
	}
}
