// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NeoAxis.Widget;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the package manager.
	/// </summary>
	public partial class PackagesWindow : DocumentWindow
	{
		PackageManager.PackageInfo selectedPackage;

		volatile List<PackageManager.PackageInfo> downloadedListOfPackages;
		volatile bool needUpdateList;

		volatile string downloadingPackageName = "";
		volatile string downloadingAddress = "";
		volatile string downloadingDestinationPath = "";
		volatile float downloadProgress;

		volatile WebClient downloadingClient;

		volatile bool needUpdatePackageControls;

		string needSelectPackage;

		/////////////////////////////////////////

		public PackagesWindow()
		{
			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			if( EditorAPI.DarkTheme )
			{
				EditorThemeUtility.ApplyDarkThemeToForm( kryptonSplitContainer1.Panel1 );
				EditorThemeUtility.ApplyDarkThemeToForm( kryptonSplitContainer1.Panel2 );
			}

			double distance = 22.0 * EditorAPI.DPIScale;
			kryptonSplitContainer2.Panel1MinSize = (int)distance;
			kryptonSplitContainer2.SplitterDistance = (int)distance;

			if( EditorAPI.DPIScale >= 2 )
				this.buttonUpdateList.Values.Image = global::NeoAxis.Properties.Resources.Refresh_32;

			WindowTitle = EditorLocalization.Translate( "PackagesWindow", WindowTitle );
		}

		private void PackagesDocumentWindow_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			//contentBrowser1.ImageHelper.AddImage( "DefaultDisabled",
			//	ToolStripRenderer.CreateDisabledImage( Properties.Resources.Default_16 ),
			//	ToolStripRenderer.CreateDisabledImage( Properties.Resources.Default_32 ) );

			try
			{
				if( !Directory.Exists( PackageManager.PackagesFolder ) )
					Directory.CreateDirectory( PackageManager.PackagesFolder );
			}
			catch( Exception e2 )
			{
				Log.Warning( e2.Message );
			}

			PerformRefreshList();
			UpdatePackageControls();

			EditorLocalization.TranslateForm( "PackagesWindow", kryptonSplitContainer2.Panel1 );
			EditorLocalization.TranslateForm( "PackagesWindow", this );

			timer1.Start();

			UpdateControls();

			EditorAPI.ClosingApplicationChanged += EditorAPI_ClosingApplicationChanged;
		}

		protected override void OnDestroy()
		{
			EditorAPI.ClosingApplicationChanged -= EditorAPI_ClosingApplicationChanged;

			base.OnDestroy();
		}

		void UpdateControls()
		{
			if( !string.IsNullOrEmpty( downloadingAddress ) )
			{
				try
				{
					progressBarPackageProgress.Value = (int)( downloadProgress * 100 );
				}
				catch { }
			}

			bool downloading = false;
			if( !string.IsNullOrEmpty( downloadingAddress ) && selectedPackage != null && selectedPackage.Name == downloadingPackageName )
				downloading = true;

			if( progressBarPackageProgress.Visible != downloading )
				progressBarPackageProgress.Visible = downloading;

			kryptonButtonDownload.Text = downloading ? "Stop Download" : "Download";

			buttonUpdateList.Location = new Point( kryptonSplitContainer2.ClientSize.Width - buttonUpdateList.Size.Width, 0 );

			var panelSize = kryptonSplitContainer1.Panel2.Size;

			labelExPackageName.Size = new Size( panelSize.Width - labelExPackageName.Location.X, labelExPackageName.Size.Height );
			labelExPackageDeveloper.Size = new Size( panelSize.Width - labelExPackageDeveloper.Location.X, labelExPackageDeveloper.Size.Height );
			labelExPackageVersion.Size = new Size( panelSize.Width - labelExPackageVersion.Location.X, labelExPackageVersion.Size.Height );
			labelExPackageSize.Size = new Size( panelSize.Width - labelExPackageSize.Location.X, labelExPackageSize.Size.Height );
			labelExPackageStatus.Size = new Size( panelSize.Width - labelExPackageStatus.Location.X, labelExPackageStatus.Size.Height );
			progressBarPackageProgress.Size = new Size( panelSize.Width - progressBarPackageProgress.Location.X, progressBarPackageProgress.Size.Height );

			labelExPackageInfo.Size = new Size( panelSize.Width - labelExPackageInfo.Location.X, panelSize.Height - labelExPackageInfo.Location.Y );
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			if( needUpdateList )
			{
				needUpdateList = false;
				UpdateList();
			}

			if( needUpdatePackageControls )
			{
				needUpdatePackageControls = false;
				UpdatePackageControls();
			}

			if( !string.IsNullOrEmpty( needSelectPackage ) )
			{
				foreach( var item in contentBrowser1.GetAllItems() )
				{
					var info = item.Tag as PackageManager.PackageInfo;
					if( info != null && info.Name == needSelectPackage )
					{
						contentBrowser1.SelectItems( new ContentBrowser.Item[] { item } );
						needSelectPackage = null;
						break;
					}
				}
			}

			UpdateControls();
		}

		void UpdateList()
		{
			string lastSelected = "";
			if( contentBrowser1.SelectedItems.Length != 0 )
				lastSelected = contentBrowser1.SelectedItems[ 0 ].Text;

			var items = new List<ContentBrowser.Item>();
			var alreadyAdded = new ESet<string>();

			foreach( var info in PackageManager.GetPackagesInfo() )
			{
				if( !alreadyAdded.Contains( info.Name ) )
				{
					var item = new ContentBrowserItem_Virtual( contentBrowser1, null, info.ToString() );
					item.Tag = info;
					item.ShowDisabled = !PackageManager.IsInstalled( info.Name );
					//item.imageKey = PackageManager.IsInstalled( info.Name ) ? null : "DefaultDisabled";
					items.Add( item );

					alreadyAdded.AddWithCheckAlreadyContained( info.Name );
				}
			}

			if( downloadedListOfPackages != null )
			{
				foreach( var info in downloadedListOfPackages )
				{
					if( !alreadyAdded.Contains( info.Name ) )
					{
						var item = new ContentBrowserItem_Virtual( contentBrowser1, null, info.ToString() );
						item.Tag = info;
						item.ShowDisabled = !PackageManager.IsInstalled( info.Name );
						//item.imageKey = PackageManager.IsInstalled( info.Name ) ? null : "DefaultDisabled";
						items.Add( item );

						alreadyAdded.AddWithCheckAlreadyContained( info.Name );
					}
				}
			}

			contentBrowser1.SetData( items.ToArray(), false );

			if( items.Count != 0 )
			{
				if( !string.IsNullOrEmpty( lastSelected ) )
				{
					foreach( var item in items )
					{
						if( item.Text == lastSelected )
						{
							contentBrowser1.SelectItems( new ContentBrowser.Item[] { item } );
							break;
						}
					}
				}

				if( contentBrowser1.SelectedItems.Length == 0 )
					contentBrowser1.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );
			}

			needUpdatePackageControls = true;
		}

		private void contentBrowser1_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			if( items.Count != 0 )
				selectedPackage = (PackageManager.PackageInfo)items[ 0 ].Tag;
			else
				selectedPackage = null;

			UpdatePackageControls();
		}

		void ShowRestartLabel()
		{
			labelRestart.Visible = true;
		}

		static string HtmlToPlainText( string html )
		{
			if( string.IsNullOrEmpty( html ) )
				return "";

			const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
			const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
			const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
			var lineBreakRegex = new Regex( lineBreak, RegexOptions.Multiline );
			var stripFormattingRegex = new Regex( stripFormatting, RegexOptions.Multiline );
			var tagWhiteSpaceRegex = new Regex( tagWhiteSpace, RegexOptions.Multiline );

			var text = html;
			//Decode html specific characters
			text = System.Net.WebUtility.HtmlDecode( text );
			//Remove tag whitespace/line breaks
			text = tagWhiteSpaceRegex.Replace( text, "><" );
			//Replace <br /> with line breaks
			text = lineBreakRegex.Replace( text, Environment.NewLine );
			//Strip formatting
			text = stripFormattingRegex.Replace( text, string.Empty );

			return text;
		}

		void UpdatePackageControls()
		{
			if( selectedPackage != null )
			{
				//!!!!только раз парсить может. или как-то иначе
				var archiveInfo = PackageManager.ReadPackageArchiveInfo( selectedPackage.FullFilePath, out var error );

				string text = "";
				//string text = selectedPackage.Name + " " + selectedPackage.Version + "\r\n\r\n";

				var description = HtmlToPlainText( selectedPackage.Description ).Trim( new char[] { ' ', '\r', '\n' } );
				if( !string.IsNullOrEmpty( description ) )
					text += description + "\r\n\r\n";
				else if( archiveInfo != null )
					text += archiveInfo.Description + "\r\n\r\n";

				//text += "Files:\r\n";
				if( archiveInfo != null )
				{
					int count = 0;
					foreach( var file in archiveInfo.Files )
					{
						if( count > 1000 )
						{
							text += "..." + "\r\n";
							break;
						}
						text += file + "\r\n";
						count++;
					}
				}
				else if( !string.IsNullOrEmpty( selectedPackage.Files ) )
					text += selectedPackage.Files.Trim( new char[] { ' ', '\r', '\n' } );
				//else
				//	text += "Unable to read package info. " + error;

				labelExPackageName.Text = selectedPackage.GetDisplayName();

				if( !string.IsNullOrEmpty( selectedPackage.Author ) )
					labelExPackageDeveloper.Text = selectedPackage.Author;
				else
					labelExPackageDeveloper.Text = archiveInfo != null ? archiveInfo.Author : "";

				labelExPackageVersion.Text = selectedPackage.Version;
				if( !string.IsNullOrEmpty( selectedPackage.Date ) )
					labelExPackageVersion.Text += ", " + GetDateAsString( selectedPackage.Date );

				var size = selectedPackage.Size;
				if( size == 0 )
				{
					try
					{
						if( File.Exists( selectedPackage.FullFilePath ) )
							size = new FileInfo( selectedPackage.FullFilePath ).Length;
					}
					catch { }
				}
				labelExPackageSize.Text = GetSizeAsString( size );

				labelExPackageInfo.Text = text;

				bool installed = PackageManager.IsInstalled( selectedPackage.Name );
				bool downloaded = !installed && archiveInfo != null;

				bool downloading = false;
				if( !string.IsNullOrEmpty( downloadingAddress ) && selectedPackage.Name == downloadingPackageName )
					downloading = true;

				bool canDownload = !string.IsNullOrEmpty( selectedPackage.Download );// || selectedPackage.OnlyPro && EngineApp.IsProPlan && selectedPackage.SecureDownload;

				bool subscribeToPro = false;//selectedPackage.OnlyPro && !EngineApp.IsProPlan;

				if( installed )
				{
					labelExPackageStatus.Text = "Installed";
					labelExPackageStatus.StateCommon.Content.Color1 = Color.Green;
				}
				else if( downloaded )
				{
					labelExPackageStatus.Text = "Downloaded, but not installed";// "Downloaded";
					labelExPackageStatus.StateCommon.Content.Color1 = Color.Red;
				}
				else if( downloading )
				{
					labelExPackageStatus.Text = "Downloading";
					labelExPackageStatus.StateCommon.Content.Color1 = Color.Green;// Color.Red;
				}
				else if( canDownload )
				{
					labelExPackageStatus.Text = "Not downloaded";
					labelExPackageStatus.StateCommon.Content.Color1 = Color.Red;
				}
				//else if( selectedPackage.OnlyPro && !EngineApp.IsProPlan )
				//{
				//	labelExPackageStatus.Text = "";
				//	labelExPackageStatus.StateCommon.Content.Color1 = Color.Red;
				//}
				else
				{
					labelExPackageStatus.Text = "";//"Download is not available";
					labelExPackageStatus.StateCommon.Content.Color1 = Color.Red;
				}

				if( subscribeToPro )
				{
					kryptonButtonBuy.Text = "Subscribe to Pro";
					kryptonButtonBuy.Enabled = true;
					kryptonButtonBuy.Tag = "SubscribeToPro";
				}
				else
				{
					//!!!!
					kryptonButtonBuy.Text = "Buy";
					kryptonButtonBuy.Enabled = false;// !installed && info != null;
					kryptonButtonBuy.Tag = null;
				}

				kryptonButtonDownload.Enabled = !installed && canDownload;
				kryptonButtonInstall.Enabled = !installed && archiveInfo != null;
				kryptonButtonUninstall.Enabled = installed;// && archiveInfo != null;
				kryptonButtonDelete.Enabled = !installed && File.Exists( selectedPackage.FullFilePath );

			}

			kryptonSplitContainer1.Panel2.Visible = selectedPackage != null;

			kryptonButtonBuy.Visible = selectedPackage != null;
			kryptonButtonDownload.Visible = selectedPackage != null;
			kryptonButtonInstall.Visible = selectedPackage != null;
			kryptonButtonUninstall.Visible = selectedPackage != null;
			kryptonButtonDelete.Visible = selectedPackage != null;

			UpdateControls();
		}

		string Translate( string text )
		{
			return EditorLocalization.Translate( "PackagesWindow", text );
		}

		private void kryptonButtonInstall_Click( object sender, EventArgs e )
		{
			var info = PackageManager.ReadPackageArchiveInfo( selectedPackage.FullFilePath, out var error );
			if( info == null )
				return;

			var filesToCopy = new List<string>();
			foreach( var file in info.Files )
			{
				var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
				filesToCopy.Add( fullName );
			}

			var text = string.Format( Translate( "Install {0}?\n\n{1} files will created." ), selectedPackage.GetDisplayName(), filesToCopy.Count );
			//var text = string.Format( Translate( "Install {0}?\r\n\r\n{1} files will created." ), selectedPackage.Name, filesToCopy.Count );
			//var text = $"Install {selectedPackage.Name}?\r\n\r\n{filesToCopy.Count} files will created.";

			if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
				return;

			var notification = ScreenNotifications.ShowSticky( "Installing the package..." );

			try
			{
				using( var archive = ZipFile.OpenRead( selectedPackage.FullFilePath ) )
				{
					foreach( var entry in archive.Entries )
					{
						var fileName = entry.FullName;
						bool directory = fileName[ fileName.Length - 1 ] == '/';
						if( fileName != "Package.info" && !directory )
						{
							var fullPath = Path.Combine( VirtualFileSystem.Directories.Project, fileName );

							var directoryName = Path.GetDirectoryName( fullPath );
							if( !Directory.Exists( directoryName ) )
								Directory.CreateDirectory( directoryName );

							entry.ExtractToFile( fullPath, true );
						}
					}
				}

				PackageManager.ChangeInstalledState( selectedPackage.Name, true );
			}
			catch( Exception e2 )
			{
				EditorMessageBox.ShowWarning( e2.Message );
				return;
			}
			finally
			{
				notification.Close();
			}

			if( !string.IsNullOrEmpty( info.AddCSharpFilesToProject ) )
			{
				var toAdd = new ESet<string>();

				var path = Path.Combine( VirtualFileSystem.Directories.Assets, info.AddCSharpFilesToProject );
				if( Directory.Exists( path ) )
				{
					var fullPaths = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true );
					var files = Directory.GetFiles( path, "*.cs", SearchOption.AllDirectories );
					foreach( var file in files )
					{
						if( !fullPaths.Contains( file ) )
							toAdd.AddWithCheckAlreadyContained( file );
					}
				}

				//	if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".cs" )
				//	{
				//		bool added = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fileItem.FullPath );
				//		if( !added )
				//			toAdd.Add( fileItem.FullPath );
				//	}

				if( toAdd.Count != 0 )
				{
					if( CSharpProjectFileUtility.UpdateProjectFile( toAdd, null, out var error2 ) )
					{
						if( toAdd.Count > 1 )
							Log.Info( EditorLocalization.Translate( "General", "Items have been added to the Project.csproj." ) );
						else
							Log.Info( EditorLocalization.Translate( "General", "The item has been added to the Project.csproj." ) );
					}
					else
						Log.Warning( error2 );
				}
			}

			needUpdateList = true;

			if( info.MustRestart )
				ShowRestartLabel();

			if( !string.IsNullOrEmpty( info.OpenAfterInstall ) )
			{
				var realFileName = VirtualPathUtility.GetRealPathByVirtual( info.OpenAfterInstall );

				if( info.MustRestart )
				{
					EditorSettingsSerialization.OpenFileAtStartup = realFileName;
				}
				else
				{
					EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { realFileName }, Directory.Exists( realFileName ) );
					EditorAPI.OpenFileAsDocument( realFileName, true, true );
				}
			}

			ScreenNotifications.Show( EditorLocalization.Translate( "General", "The package has been successfully installed." ) );

			//restart application
			if( info.MustRestart )
			{
				var text2 = EditorLocalization.Translate( "General", "To apply changes need restart the editor. Restart?" );
				if( EditorMessageBox.ShowQuestion( text2, EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
					EditorAPI.BeginRestartApplication();
			}
		}

		bool IsDirectoryEmpty( string path )
		{
			return !Directory.EnumerateFileSystemEntries( path ).Any();
		}

		private void kryptonButtonUninstall_Click( object sender, EventArgs e )
		{
			var filesToDelete = new List<string>();
			bool mustRestart = false;

			//get list of files to delete
			if( File.Exists( selectedPackage.FullFilePath ) )
			{
				//get list of files from the package archive

				var info = PackageManager.ReadPackageArchiveInfo( selectedPackage.FullFilePath, out var error );
				if( info == null )
				{
					ScreenNotifications.Show( "Could not read the package info.", true );
					Log.Warning( error );
					return;
				}

				foreach( var file in info.Files )
				{
					var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
					if( File.Exists( fullName ) )
						filesToDelete.Add( file );
				}

				mustRestart = info.MustRestart;
			}
			else
			{
				//get list of files from selectedPackage.Files in case when the archive file is not exists

				var str = selectedPackage.Files.Trim( new char[] { ' ', '\r', '\n' } );

				var files = str.Split( new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );
				foreach( var file in files )
				{
					var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
					if( File.Exists( fullName ) )
						filesToDelete.Add( file );
				}

				//!!!!mustRestart
				mustRestart = true;
			}

			if( filesToDelete.Count == 0 )
				return;

			var text = string.Format( Translate( "Uninstall {0}?\n\n{1} files will deleted." ), selectedPackage.GetDisplayName(), filesToDelete.Count );
			if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
				return;

			var filesToDeletionAtStartup = new List<string>();

			try
			{
				//remove cs files from Project.csproj
				try
				{
					var toRemove = new List<string>();

					foreach( var file in filesToDelete )
					{
						var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
						if( Path.GetExtension( fullName ).ToLower() == ".cs" )
							toRemove.Add( VirtualPathUtility.NormalizePath( fullName ) );
					}

					if( toRemove.Count != 0 )
						CSharpProjectFileUtility.UpdateProjectFile( null, toRemove, out _ );
				}
				catch { }

				//delete files
				foreach( var file in filesToDelete )
				{
					var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );

					try
					{
						File.Delete( fullName );
					}
					catch( UnauthorizedAccessException )
					{
						filesToDeletionAtStartup.Add( file );
					}
					catch( IOException )
					{
						filesToDeletionAtStartup.Add( file );
					}
				}

				//delete empty folders
				{
					var allFolders = new ESet<string>();
					foreach( var file in filesToDelete )
					{
						var f = Path.GetDirectoryName( file );
						while( !string.IsNullOrEmpty( f ) )
						{
							allFolders.AddWithCheckAlreadyContained( f );
							f = Path.GetDirectoryName( f );
						}
					}

					var list = allFolders.ToArray();
					CollectionUtility.MergeSort( list, delegate ( string f1, string f2 )
					{
						var levels1 = f1.Split( new char[] { '\\' } ).Length;
						var levels2 = f2.Split( new char[] { '\\' } ).Length;
						return levels2 - levels1;
					} );

					foreach( var folder in list )
					{
						var fullName = Path.Combine( VirtualFileSystem.Directories.Project, folder );

						if( Directory.Exists( fullName ) && IsDirectoryEmpty( fullName ) )
							Directory.Delete( fullName );
					}
				}

				PackageManager.ChangeInstalledState( selectedPackage.Name, false );
			}
			catch( Exception e2 )
			{
				EditorMessageBox.ShowWarning( e2.Message );
				return;
			}

			if( filesToDeletionAtStartup.Count != 0 )
				PackageManager.AddFilesToDeletionAtStartup( filesToDeletionAtStartup );

			needUpdateList = true;

			if( mustRestart )
				ShowRestartLabel();

			ScreenNotifications.Show( EditorLocalization.Translate( "General", "The package has been successfully uninstalled." ) );
		}

		void UpdateListItemsProperties()
		{
			bool needRefresh = false;

			foreach( var item in contentBrowser1.GetAllItems() )
			{
				var info = item.Tag as PackageManager.PackageInfo;
				if( info != null )
				{
					item.ShowDisabled = !PackageManager.IsInstalled( info.Name );

					//var newValue = PackageManager.IsInstalled( info.Name ) ? null : "DefaultDisabled";

					//if( item.imageKey != newValue )
					//{
					//	item.imageKey = newValue;
					//	//!!!!можно поддержать обновление imageKey
					//	needRefresh = true;
					//}
				}
			}
			contentBrowser1.Invalidate();

			if( needRefresh )
				needUpdateList = true;
		}

		void ThreadGetStoreItems()
		{
			try
			{
				string xml = "";
				string url = @"https://www.neoaxis.com/api/get_store_items";

				var request = (HttpWebRequest)WebRequest.Create( url );

				using( var response = (HttpWebResponse)request.GetResponse() )
				using( var stream = response.GetResponseStream() )
				using( var reader = new StreamReader( stream ) )
					xml = reader.ReadToEnd();

				XmlDocument xDoc = new XmlDocument();
				xDoc.LoadXml( xml );

				var packages = new List<PackageManager.PackageInfo>();

				foreach( XmlNode itemNode in xDoc.GetElementsByTagName( "item" ) )
				{
					var info = new PackageManager.PackageInfo();
					//info.Name = "None";
					//info.Version = "1.0.0.0";

					foreach( XmlNode child in itemNode.ChildNodes )
					{
						if( child.Name == "name" )
							info.Name = child.InnerText;
						if( child.Name == "author" )
							info.Author = child.InnerText;
						if( child.Name == "version" )
							info.Version = child.InnerText;
						//if( child.Name == "only_pro" && !string.IsNullOrEmpty( child.InnerText ) )
						//	info.OnlyPro = (bool)SimpleTypes.ParseValue( typeof( bool ), child.InnerText );
						if( child.Name == "size" )
						{
							double.TryParse( child.InnerText, out var size );
							info.Size = (long)size;
						}
						if( child.Name == "download" )
							info.Download = child.InnerText;
						if( child.Name == "secure_download" && !string.IsNullOrEmpty( child.InnerText ) )
							info.SecureDownload = (bool)SimpleTypes.ParseValue( typeof( bool ), child.InnerText );
						if( child.Name == "description" )
							info.Description = child.InnerText;
						if( child.Name == "date" )
							info.Date = child.InnerText;
						if( child.Name == "files" )
							info.Files = child.InnerText;
					}

					packages.Add( info );
				}

				//sort by date
				CollectionUtility.MergeSort( packages, delegate ( PackageManager.PackageInfo p1, PackageManager.PackageInfo p2 )
				{
					if( p1.Date != p2.Date )
						return -string.Compare( p1.Date, p2.Date );
					return 0;
				} );

				downloadedListOfPackages = packages;

				needUpdateList = true;
			}
			catch { }
		}

		void PerformRefreshList()
		{
			UpdateList();

			Thread thread1 = new Thread( ThreadGetStoreItems );
			thread1.Start();
		}

		private void buttonUpdateList_Click( object sender, EventArgs e )
		{
			PerformRefreshList();
		}

		string GetSizeAsString( long size )
		{
			if( size == 0 )
				return "";

			var s = size / 1024 / 1024;
			if( s >= 1 )
				return s.ToString() + " MB";
			else
			{
				s = size / 1024;
				return s.ToString() + " KB";
			}
		}

		string GetDateAsString( string value )
		{
			var v2 = value.Replace( "-", "" );
			DateTime dt = DateTime.ParseExact( v2, "yyyyMMdd", CultureInfo.InvariantCulture );
			return dt.ToString( "dd MMMM yyyy", CultureInfo.InvariantCulture );
			//return dt.ToString();
		}

		private void kryptonButtonDownload_Click( object sender, EventArgs e )
		{
			if( !string.IsNullOrEmpty( downloadingAddress ) )
			{
				downloadingClient?.CancelAsync();
				downloadingPackageName = "";
				downloadingAddress = "";
				//downloadingDestinationPath = "";
				downloadProgress = 0;
				downloadingClient = null;
				return;
			}

			if( selectedPackage != null && ( !string.IsNullOrEmpty( selectedPackage.Download )/* || selectedPackage.OnlyPro && EngineApp.IsProPlan && selectedPackage.SecureDownload*/ ) )
			{
				downloadingPackageName = selectedPackage.Name;

				if( !string.IsNullOrEmpty( selectedPackage.Download ) )
				{
					downloadingAddress = selectedPackage.Download;
					downloadingDestinationPath = Path.Combine( PackageManager.PackagesFolder, Path.GetFileName( downloadingAddress ) );
				}
				//else if( selectedPackage.OnlyPro && EngineApp.IsProPlan && selectedPackage.SecureDownload )
				//{
				//	if( !LoginUtility.GetCurrentLicense( out var email, out var hash ) )
				//		return;

				//	var item = selectedPackage.Name.Replace( ' ', '_' );
				//	var version = selectedPackage.Version;

				//	var email64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( email.ToLower() ) ).Replace( "=", "" );
				//	var hash64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( hash ) ).Replace( "=", "" );
				//	var item64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( item ) ).Replace( "=", "" );
				//	var version64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( version ) ).Replace( "=", "" );

				//	downloadingAddress = $@"https://www.neoaxis.com/api/secure_download?email={email64}&hash={hash64}&item={item64}&version={version64}";

				//	var fileName = $"{item}-{version}.neoaxispackage";
				//	downloadingDestinationPath = Path.Combine( PackageManager.PackagesFolder, fileName );
				//}

				downloadProgress = 0;

				Thread thread1 = new Thread( ThreadDownload );
				thread1.Start();

				UpdatePackageControls();
			}
		}

		void ThreadDownload()
		{
			//var fileName = Path.Combine( PackageManager.PackagesFolder, Path.GetFileName( downloadingAddress ) );
			var cancelled = false;
			Exception error = null;

			try
			{
				using( WebClient client = new WebClient() )
				{
					downloadingClient = client;

					var tempFileName = Path.GetTempFileName();

					client.DownloadProgressChanged += delegate ( object sender, DownloadProgressChangedEventArgs e )
					{
						//check already ended
						if( cancelled )
							return;

						if( e.TotalBytesToReceive != 0 )
							downloadProgress = MathEx.Saturate( (float)e.BytesReceived / (float)e.TotalBytesToReceive );
					};

					client.DownloadFileCompleted += delegate ( object sender, AsyncCompletedEventArgs e )
					{
						//check already ended
						if( cancelled )
							return;

						//releases blocked thread
						lock( e.UserState )
							Monitor.Pulse( e.UserState );

						cancelled = e.Cancelled;
						error = e.Error;

						//copy to destination path
						if( !cancelled && error == null )
							File.Copy( tempFileName, downloadingDestinationPath );

						try
						{
							File.Delete( tempFileName );
						}
						catch { }
					};

					using( var task = client.DownloadFileTaskAsync( new Uri( downloadingAddress ), tempFileName ) )
					{
						while( !string.IsNullOrEmpty( downloadingAddress ) && !task.Wait( 10 ) )
						{
						}
					}

					//var syncObject = new object();
					//lock( syncObject )
					//{
					//	client.DownloadFileAsync( new Uri( downloadingAddress ), downloadingDestinationPath, syncObject );

					//	//This would block the thread until download completes
					//	Monitor.Wait( syncObject );
					//}

					downloadingClient = null;
				}
			}
			catch( Exception e )
			{
				Log.Warning( e.Message );
				return;
			}
			finally
			{
				try
				{
					if( !cancelled )
					{
						if( File.Exists( downloadingDestinationPath ) && new FileInfo( downloadingDestinationPath ).Length == 0 )
						{
							File.Delete( downloadingDestinationPath );
							cancelled = true;
						}
					}
					if( !cancelled && !File.Exists( downloadingDestinationPath ) )
						cancelled = true;

					if( cancelled || error != null )
					{
						if( File.Exists( downloadingDestinationPath ) )
							File.Delete( downloadingDestinationPath );
					}
				}
				catch { }

				if( error != null && !cancelled )
					Log.Warning( ( error.InnerException ?? error ).Message );

				downloadingPackageName = "";
				downloadingAddress = "";
				downloadingDestinationPath = "";
				downloadProgress = 0;
				downloadingClient = null;
			}

			needUpdateList = true;

			if( !cancelled )
			{
				if( error != null )
					ScreenNotifications.Show( EditorLocalization.Translate( "General", "Error downloading the package." ), true );
				else
					ScreenNotifications.Show( EditorLocalization.Translate( "General", "The package has been successfully downloaded." ) );
			}
		}

		private void kryptonButtonDelete_Click( object sender, EventArgs e )
		{
			if( selectedPackage == null )
				return;
			if( !File.Exists( selectedPackage.FullFilePath ) )
				return;

			var template = Translate( "Are you sure you want to delete \'{0}\'?" );
			var text = string.Format( template, selectedPackage.FullFilePath );

			if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.No )
				return;

			try
			{
				File.Delete( selectedPackage.FullFilePath );
			}
			catch( Exception e2 )
			{
				EditorMessageBox.ShowWarning( e2.Message );
				return;
			}

			needUpdateList = true;
		}

		private void EditorAPI_ClosingApplicationChanged()
		{
			if( EditorAPI.ClosingApplication )
			{
				try
				{
					downloadingClient?.CancelAsync();
				}
				catch { }
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

		public void NeedSelectPackage( string packageName )
		{
			//can be available later, after download the list of packages from the server.
			needSelectPackage = packageName;
		}

		private void kryptonButtonBuy_Click( object sender, EventArgs e )
		{
			var tagStr = kryptonButtonBuy.Tag as string;
			if( tagStr != null && tagStr == "SubscribeToPro" )
			{
				Process.Start( new ProcessStartInfo( "https://www.neoaxis.com/licensing" ) { UseShellExecute = true } );
				return;
			}

			//!!!!

		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( Created )
				UpdateControls();
		}
	}
}
