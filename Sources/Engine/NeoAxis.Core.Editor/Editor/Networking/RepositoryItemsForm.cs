#if CLOUD
#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using NeoAxis.Networking;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public partial class RepositoryItemsForm : EngineForm
	{
		Item[] items;
		bool mustSelectSomething;

		//public delegate bool CheckDelegate( ref string error );
		//CheckDelegate checkHandler;

		//public delegate bool OKDelegate( ref string error );
		//OKDelegate okHandler;

		//bool loaded;

		/////////////////////////////////////

		public class Item
		{
			public string FileName;
			public string Prefix;
			public object Tag;
		}

		/////////////////////////////////////

		public RepositoryItemsForm( string caption, string text, Item[] items, bool mustSelectSomething )
		//, object optionalObjectProvider, CheckDelegate checkHandler = null, OKDelegate okHandler = null )
		{
			this.items = items;
			this.mustSelectSomething = mustSelectSomething;

			//this.checkHandler = checkHandler;
			//this.okHandler = okHandler;

			InitializeComponent();

			if( string.IsNullOrEmpty( caption ) )
				Text = EngineInfo.NameWithVersion;
			else
				Text = caption;

			labelText.Text = text;

			EditorThemeUtility.ApplyDarkThemeToForm( this );
		}

		private void RepositoryItemsForm_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			UpdateControls();

			if( EditorAPI2.DarkTheme )
				bordersContainer1.BorderColor = Color.FromArgb( 80, 80, 80 );


			var listItems = new List<EngineListView.Item>();

			foreach( var item in items )
			{
				string text;
				if( !string.IsNullOrEmpty( item.Prefix ) )
					text = item.Prefix + " " + item.FileName;
				else
					text = item.FileName;

				var listItem = new EngineListView.Item( engineListView1 );
				listItem.Text = text;
				listItem.Checked = true;
				listItem.Tag = item;
				listItems.Add( listItem );
			}

			//foreach( var fileInfo in filesDownload )
			//{
			//	var item = new EngineListView.Item( engineListView1 );
			//	item.Text = "DOWNLOAD " + fileInfo.FileName;
			//	item.Checked = true;
			//	listItems.Add( item );
			//}

			//foreach( var fileName in filesToDelete )
			//{
			//	var item = new EngineListView.Item( engineListView1 );
			//	item.Text = "DELETE " + fileName;
			//	item.Checked = true;
			//	listItems.Add( item );
			//}

			engineListView1.SetItems( listItems );

			//loaded = true;

			//Translate();

			timer1.Start();
		}

		//[Browsable( false )]
		//public CheckDelegate CheckHandler
		//{
		//	get { return checkHandler; }
		//	set { checkHandler = value; }
		//}

		//[Browsable( false )]
		//public OKDelegate OKHandler
		//{
		//	get { return okHandler; }
		//	set { okHandler = value; }
		//}

		private void RenameResourceDialog_FormClosing( object sender, FormClosingEventArgs e )
		{
			if( DialogResult == DialogResult.OK )
			{
				if( mustSelectSomething && GetCheckedItems().Length == 0 )
				{
					e.Cancel = true;
					return;
				}

				//string error = "";
				//if( okHandler != null && !okHandler( ref error ) )
				//{
				//	e.Cancel = true;
				//	labelError.Text = error;
				//	return;
				//}
			}
		}

		void UpdateControls()
		{
			buttonCancel.Location = new Point( ClientSize.Width - buttonCancel.Size.Width - DpiHelper.Default.ScaleValue( 12 ), ClientSize.Height - buttonCancel.Size.Height - DpiHelper.Default.ScaleValue( 12 ) );
			buttonOK.Location = new Point( buttonCancel.Location.X - buttonOK.Size.Width - DpiHelper.Default.ScaleValue( 8 ), buttonCancel.Location.Y );
			kryptonButtonSelectAll.Location = new Point( kryptonButtonSelectAll.Location.X, buttonOK.Location.Y );
			kryptonButtonDeselectAll.Location = new Point( kryptonButtonDeselectAll.Location.X, buttonOK.Location.Y );
			labelSelected.Location = new Point( labelSelected.Location.X, buttonOK.Location.Y + DpiHelper.Default.ScaleValue( 6 ) );

			bordersContainer1.Size = new Size( ClientSize.Width - DpiHelper.Default.ScaleValue( 12 ) - bordersContainer1.Location.X, buttonOK.Location.Y - DpiHelper.Default.ScaleValue( 8 ) - bordersContainer1.Location.Y );
			engineListView1.Size = new Size( ClientSize.Width - DpiHelper.Default.ScaleValue( 12 ) - engineListView1.Location.X - 2, buttonOK.Location.Y - DpiHelper.Default.ScaleValue( 8 ) - engineListView1.Location.Y - 2 );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControls();
		}

		private void kryptonButtonSelectAll_Click( object sender, EventArgs e )
		{
			foreach( var item in engineListView1.Items )
				item.Checked = true;
		}

		private void kryptonButtonDeselectAll_Click( object sender, EventArgs e )
		{
			foreach( var item in engineListView1.Items )
				item.Checked = false;
		}

		//void Translate()
		//{
		//	buttonOK.Text = ToolsLocalization.Translate( "OKCancelTextBoxDialog", buttonOK.Text );
		//	buttonCancel.Text = ToolsLocalization.Translate( "OKCancelTextBoxDialog", buttonCancel.Text );
		//}

		public Item[] GetCheckedItems()
		{
			var result = new List<Item>();
			foreach( var listItem in engineListView1.Items )
			{
				if( listItem.Checked )
					result.Add( (Item)listItem.Tag );
			}
			return result.ToArray();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			var checkedItems = GetCheckedItems();
			buttonOK.Enabled = !mustSelectSomething || checkedItems.Length != 0;
			labelSelected.Text = string.Format( "{0} selected", checkedItems.Length );
		}
	}
}
#endif
#endif