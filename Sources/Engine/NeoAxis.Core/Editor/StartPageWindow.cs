// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NeoAxis.Widget;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Start Page.
	/// </summary>
	public partial class StartPageWindow : DocumentWindow
	{
		string[] currentOpenScenes = new string[ 0 ];
		double currentOpenScenesLastUpdate;

		//

		public StartPageWindow()
		{
			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			if( EditorAPI.DarkTheme )
			{
				BackColor = Color.FromArgb( 40, 40, 40 );
				DarkThemeUtility.ApplyToToolTip( toolTip1 );
			}

			contentBrowserNewScene.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
			contentBrowserNewScene.Options.ListMode = ContentBrowser.ListModeEnum.Tiles;
			contentBrowserNewScene.UseSelectedTreeNodeAsRootForList = false;
			contentBrowserNewScene.Options.Breadcrumb = false;
			contentBrowserNewScene.Options.TileImageSize = 128;

			//add items
			try
			{
				var items = new List<ContentBrowser.Item>();

				foreach( var template in Component_Scene.NewObjectSettingsScene.GetTemplates() )
				{
					contentBrowserNewScene.ImageHelper.AddImage( template.Name, null, template.Preview );

					var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, template.ToString() );
					item.Tag = template;
					item.imageKey = template.Name;
					items.Add( item );
				}

				if( items.Count != 0 )
				{
					contentBrowserNewScene.SetData( items, false );
					contentBrowserNewScene.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );
				}
			}
			catch( Exception exc )
			{
				contentBrowserNewScene.SetError( "Error: " + exc.Message );
			}

			contentBrowserNewScene.ItemAfterChoose += ContentBrowserNewScene_ItemAfterChoose;


			contentBrowserOpenScene.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
			contentBrowserOpenScene.Options.ListMode = ContentBrowser.ListModeEnum.List;
			contentBrowserOpenScene.UseSelectedTreeNodeAsRootForList = false;
			contentBrowserOpenScene.Options.Breadcrumb = false;
			contentBrowserOpenScene.Options.ListImageSize = 18;
			contentBrowserOpenScene.Options.ListColumnWidth = 10000;

			UpdateOpenScenes();
		}

		private void StoreDocumentWindow_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			timer1.Start();

			UpdateControls();
		}

		void UpdateControls()
		{
			buttonCreateScene.Enabled = contentBrowserNewScene.SelectedItems.Length == 1;
			kryptonButtonOpenScene.Enabled = contentBrowserOpenScene.SelectedItems.Length == 1;

			kryptonButtonLightTheme.Enabled = ProjectSettings.Get.Theme.Value != Component_ProjectSettings.ThemeEnum.Light;
			kryptonButtonDarkTheme.Enabled = ProjectSettings.Get.Theme.Value != Component_ProjectSettings.ThemeEnum.Dark;
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			UpdateControls();

			var time = Time.Current;
			if( time > currentOpenScenesLastUpdate + 1.0 )
			{
				currentOpenScenesLastUpdate = time;
				UpdateOpenScenes();
			}
		}

		private void kryptonButtonOpenStore_Click( object sender, EventArgs e )
		{
			EditorAPI.OpenStore();
		}

		private void kryptonButtonLightTheme_Click( object sender, EventArgs e )
		{
			if( EditorMessageBox.ShowQuestion( "Set the light theme and restart the editor to apply changes?", MessageBoxButtons.OKCancel ) == DialogResult.Cancel )
				return;

			ProjectSettings.Get.Theme = Component_ProjectSettings.ThemeEnum.Light;
			ProjectSettings.SaveToFileAndUpdate();
			EditorAPI.BeginRestartApplication();
		}

		private void kryptonButtonDarkTheme_Click( object sender, EventArgs e )
		{
			if( EditorMessageBox.ShowQuestion( "Set the dark theme and restart the editor to apply changes?", MessageBoxButtons.OKCancel ) == DialogResult.Cancel )
				return;

			ProjectSettings.Get.Theme = Component_ProjectSettings.ThemeEnum.Dark;
			ProjectSettings.SaveToFileAndUpdate();
			EditorAPI.BeginRestartApplication();
		}

		private void kryptonButtonOpenScene_Click( object sender, EventArgs e )
		{
			if( contentBrowserOpenScene.SelectedItems.Length == 1 )
			{
				var item = contentBrowserOpenScene.SelectedItems[ 0 ];
				var fileName = item.Tag as string;

				if( !string.IsNullOrEmpty( fileName ) )
					EditorAPI.OpenFileAsDocument( VirtualPathUtility.GetRealPathByVirtual( fileName ), true, true );
			}
		}

		private void buttonCreateScene_Click( object sender, EventArgs e )
		{
			if( contentBrowserNewScene.SelectedItems.Length == 1 )
				CreateScene( contentBrowserNewScene.SelectedItems[ 0 ] );
		}

		private void ContentBrowserNewScene_ItemAfterChoose( ContentBrowser sender, ContentBrowser.Item item, ref bool handled )
		{
			if( item != null )
				CreateScene( item );
		}

		void CreateScene( ContentBrowser.Item item )
		{
			try
			{
				string fileName = null;
				for( int n = 1; ; n++ )
				{
					string f = string.Format( @"Scenes\New{0}.scene", n > 1 ? n.ToString() : "" );
					if( !VirtualFile.Exists( f ) )
					{
						fileName = f;
						break;
					}
				}

				if( !string.IsNullOrEmpty( fileName ) )
				{
					var realFileName = VirtualPathUtility.GetRealPathByVirtual( fileName );

					var template = (Component_Scene.NewObjectSettingsScene.TemplateClass)item.Tag;
					string name = template.Name + ".scene";
					var sourceFile = VirtualPathUtility.GetRealPathByVirtual( @"Base\Tools\NewResourceTemplates\" + name );

					var text = VirtualFile.ReadAllText( sourceFile );

					var directoryName = Path.GetDirectoryName( realFileName );
					if( !Directory.Exists( directoryName ) )
						Directory.CreateDirectory( directoryName );

					File.WriteAllText( realFileName, text );

					EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { realFileName } );
					EditorAPI.OpenFileAsDocument( realFileName, true, true );
				}
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				//Log.Warning( e.Message );
				return;
			}
		}

		void UpdateOpenScenes()
		{
			string[] newFiles = new string[ 0 ];
			try
			{
				newFiles = VirtualDirectory.GetFiles( "", "*.scene", SearchOption.AllDirectories );

				CollectionUtility.MergeSort( newFiles, delegate ( string name1, string name2 )
				{
					var s1 = name1.Replace( "\\", " \\" );
					var s2 = name2.Replace( "\\", " \\" );
					return string.Compare( s1, s2 );
				} );
			}
			catch { }

			bool needUpdate = !newFiles.SequenceEqual( currentOpenScenes );
			if( !needUpdate )
				return;

			currentOpenScenes = newFiles;

			//add items
			try
			{
				var items = new List<ContentBrowser.Item>();

				foreach( var file in currentOpenScenes )
				{
					if( !file.Contains( @"Base\Tools\NewResourceTemplates" ) )
					{
						var realFileName = VirtualPathUtility.GetRealPathByVirtual( file );

						var item = new ContentBrowserItem_File( contentBrowserOpenScene, null, realFileName, false );
						item.SetText( file );
						item.Tag = file;
						item.imageKey = "Resource";
						items.Add( item );

						//var item = new ContentBrowserItem_Virtual( contentBrowserOpenScene, null, file );
						//item.Tag = file;
						//item.imageKey = "Resource";
						//items.Add( item );
					}
				}

				contentBrowserOpenScene.SetData( items, false );
				if( items.Count != 0 )
					contentBrowserOpenScene.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );
			}
			catch( Exception exc )
			{
				contentBrowserOpenScene.SetError( "Error: " + exc.Message );
			}

		}
	}
}
