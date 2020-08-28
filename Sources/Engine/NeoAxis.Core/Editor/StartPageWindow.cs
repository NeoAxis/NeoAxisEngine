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
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Start Page.
	/// </summary>
	public partial class StartPageWindow : DocumentWindow, ControlDoubleBufferComposited.IDoubleBufferComposited
	{
		string[] currentOpenScenes = new string[ 0 ];
		double currentOpenScenesLastUpdate;

		static Image previewImageNewUIControl;
		static Image previewImageNewResource;

		//

		public StartPageWindow()
		{
			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			if( EditorAPI.DarkTheme )
			{
				BackColor = Color.FromArgb( 40, 40, 40 );
				EditorThemeUtility.ApplyDarkThemeToToolTip( toolTip1 );
			}

			contentBrowserNewScene.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
			contentBrowserNewScene.Options.ListMode = ContentBrowser.ListModeEnum.Tiles;
			contentBrowserNewScene.UseSelectedTreeNodeAsRootForList = false;
			contentBrowserNewScene.Options.Breadcrumb = false;
			contentBrowserNewScene.Options.TileImageSize = 100;
			//contentBrowserNewScene.Options.TileImageSize = 128;

			//add items
			try
			{
				var items = new List<ContentBrowser.Item>();

				//scenes
				foreach( var template in Component_Scene.NewObjectSettingsScene.GetTemplates() )
				{
					contentBrowserNewScene.AddImageKey( template.Name, template.Preview );

					var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, template.ToString() + " scene" );
					item.Tag = template;
					item.imageKey = template.Name;

					items.Add( item );
				}

				//UI Control
				{
					var path = VirtualPathUtility.GetRealPathByVirtual( @"Base\Tools\NewResourceTemplates\UIControl.ui" );
					if( File.Exists( path ) )
					{
						var name = Path.GetFileNameWithoutExtension( path );

						if( previewImageNewUIControl == null )
						{
							var previewPath = Path.Combine( Path.GetDirectoryName( path ), name + ".png" );
							previewImageNewUIControl = File.Exists( previewPath ) ? Image.FromFile( previewPath ) : null;
						}

						if( previewImageNewUIControl != null )
							contentBrowserNewScene.AddImageKey( name, previewImageNewUIControl );

						var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, "UI Control" );
						item.Tag = "UIControl";
						if( previewImageNewUIControl != null )
							item.imageKey = name;

						items.Add( item );
					}
				}

				//Select resource
				{
					var name = "SelectResource";

					if( previewImageNewResource == null )
					{
						var previewPath = VirtualPathUtility.GetRealPathByVirtual( @"Base\Tools\NewResourceTemplates\Resource.png" );
						previewImageNewResource = File.Exists( previewPath ) ? Image.FromFile( previewPath ) : null;
					}

					if( previewImageNewResource != null )
						contentBrowserNewScene.AddImageKey( name, previewImageNewResource );

					var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, "Select type" );
					item.Tag = "Resource";
					if( previewImageNewResource != null )
						item.imageKey = name;

					items.Add( item );
				}

				////!!!!
				//{
				//	var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, "C# Class Library" );
				//	//!!!!
				//	//item.Tag = template;
				//	item.imageKey = "";//template.Name;

				//	item.ShowDisabled = true;

				//	items.Add( item );
				//}

				////!!!!
				//{
				//	var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, "Executable App" );
				//	//!!!!
				//	//item.Tag = template;
				//	item.imageKey = "";//template.Name;

				//	item.ShowDisabled = true;

				//	items.Add( item );
				//}

				if( items.Count != 0 )
				{
					contentBrowserNewScene.SetData( items, false );
					contentBrowserNewScene.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );
				}
			}
			catch( Exception exc )
			{
				Log.Warning( exc.Message );
				//contentBrowserNewScene.SetError( "Error: " + exc.Message );
			}

			contentBrowserNewScene.ItemAfterChoose += ContentBrowserNewScene_ItemAfterChoose;


			contentBrowserOpenScene.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
			contentBrowserOpenScene.Options.ListMode = ContentBrowser.ListModeEnum.List;
			contentBrowserOpenScene.UseSelectedTreeNodeAsRootForList = false;
			contentBrowserOpenScene.Options.Breadcrumb = false;

			var imageSize = EditorAPI.DPIScale >= 1.25f ? 13 : 16;

			contentBrowserOpenScene.Options.ListImageSize = imageSize;
			contentBrowserOpenScene.Options.ListColumnWidth = 10000;
			contentBrowserOpenScene.ListViewModeOverride = new EngineListView.DefaultListMode( contentBrowserOpenScene.GetListView(), imageSize );

			contentBrowserOpenScene.PreloadResourceOnSelection = false;

			UpdateOpenScenes();

			WindowTitle = EditorLocalization.Translate( "StartPageWindow", WindowTitle );
		}

		private void StartPageWindow_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			EditorLocalization.TranslateForm( "StartPageWindow", this );

			timer1.Start();

			UpdateControls();
		}

		void UpdateControls()
		{
			buttonCreateScene.Enabled = contentBrowserNewScene.SelectedItems.Length == 1;
			kryptonButtonOpenScene.Enabled = contentBrowserOpenScene.SelectedItems.Length == 1;

			kryptonButtonLightTheme.Enabled = ProjectSettings.Get.Theme.Value != Component_ProjectSettings.ThemeEnum.Light;
			kryptonButtonDarkTheme.Enabled = ProjectSettings.Get.Theme.Value != Component_ProjectSettings.ThemeEnum.Dark;

			kryptonCheckBoxMinimizeRibbon.Checked = EditorForm.Instance.kryptonRibbon.MinimizedMode;
			kryptonCheckBoxShowQATBelowRibbon.Checked = EditorForm.Instance.kryptonRibbon.QATLocation == ComponentFactory.Krypton.Ribbon.QATLocation.Below;
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
			if( EditorMessageBox.ShowQuestion( "Set the light theme and restart the editor to apply changes?", EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
				return;

			ProjectSettings.Get.Theme = Component_ProjectSettings.ThemeEnum.Light;
			ProjectSettings.SaveToFileAndUpdate();
			EditorAPI.BeginRestartApplication();
		}

		private void kryptonButtonDarkTheme_Click( object sender, EventArgs e )
		{
			if( EditorMessageBox.ShowQuestion( "Set the dark theme and restart the editor to apply changes?", EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
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
			{
				var item = contentBrowserNewScene.SelectedItems[ 0 ];
				CreateResource( item );
			}
		}

		private void ContentBrowserNewScene_ItemAfterChoose( ContentBrowser sender, ContentBrowser.Item item, ref bool handled )
		{
			if( item != null )
				CreateResource( item );
		}

		void CreateResource( ContentBrowser.Item item )
		{
			if( item.Tag as Component_Scene.NewObjectSettingsScene.TemplateClass != null )
				CreateScene( item );
			else if( item.Tag as string != null && item.Tag as string == "UIControl" )
				CreateUIControl();
			else
				SelectCreateResource();
		}

		void CreateScene( ContentBrowser.Item item )
		{
			try
			{
				var prefix = VirtualDirectory.Exists( "Scenes" ) ? @"Scenes\" : "";

				string fileName = null;
				for( int n = 1; ; n++ )
				{
					string f = prefix + string.Format( @"New{0}.scene", n > 1 ? n.ToString() : "" );
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

		void CreateUIControl()
		{
			try
			{
				var prefix = VirtualDirectory.Exists( "UI" ) ? @"UI\" : "";

				string fileName = null;
				for( int n = 1; ; n++ )
				{
					string f = prefix + string.Format( @"New{0}.ui", n > 1 ? n.ToString() : "" );
					if( !VirtualFile.Exists( f ) )
					{
						fileName = f;
						break;
					}
				}

				if( !string.IsNullOrEmpty( fileName ) )
				{
					var realFileName = VirtualPathUtility.GetRealPathByVirtual( fileName );

					var sourceFile = VirtualPathUtility.GetRealPathByVirtual( @"Base\Tools\NewResourceTemplates\UIControl.ui" );

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

		void SelectCreateResource()
		{
			var initData = new NewObjectWindow.CreationDataClass();
			EditorAPI.OpenNewObjectWindow( initData );
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
						item.imageKey = "Scene";

						items.Add( item );
					}
				}

				contentBrowserOpenScene.SetData( items, false );
				if( items.Count != 0 )
					contentBrowserOpenScene.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );
			}
			catch( Exception exc )
			{
				Log.Warning( exc.Message );
				//contentBrowserOpenScene.SetError( "Error: " + exc.Message );
			}

		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams handleParam = base.CreateParams;
				handleParam.ExStyle |= 0x02000000;//WS_EX_COMPOSITED       
				return handleParam;
			}
		}

		private void kryptonCheckBoxMinimizeRibbon_CheckedChanged( object sender, EventArgs e )
		{
			EditorForm.Instance.kryptonRibbon.MinimizedMode = kryptonCheckBoxMinimizeRibbon.Checked;
		}

		private void kryptonCheckBoxShowQATBelowRibbon_CheckedChanged( object sender, EventArgs e )
		{
			EditorForm.Instance.kryptonRibbon.QATLocation = kryptonCheckBoxShowQATBelowRibbon.Checked ? ComponentFactory.Krypton.Ribbon.QATLocation.Below : ComponentFactory.Krypton.Ribbon.QATLocation.Above;
		}
	}
}
