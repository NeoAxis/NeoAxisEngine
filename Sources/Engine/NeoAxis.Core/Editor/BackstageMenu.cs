#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Drawing;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Ribbon;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.IO.Compression;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Navigator;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Backstage of the editor.
	/// </summary>
	public partial class BackstageMenu : BackstageAppMenu
	{
		static bool backstageVisible;

		bool doInitActionsInTimer;

		public static string needStartBuildProduct;
		public static bool needStartBuildProductAndRun;

		/////////////////////////////////////////

		public class MyKryptonNavigator : KryptonNavigator
		{
			[Browsable( false )]
			public BackstageMenu owner;

			protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
			{
				if( keyData == Keys.Escape )
				{
					owner.Hide();
					return true;
				}

				return base.ProcessCmdKey( ref msg, keyData );
			}
		}

		/////////////////////////////////////////

		public BackstageMenu()
		{
			InitializeComponent();

			kryptonButtonBack.Values.Image = Properties.Resources.BackstageButtonBack;

			kryptonNavigator1.owner = this;
			kryptonNavigator1.AllowPageReorder = false;
			kryptonNavigator1.AllowTabFocus = false;

			kryptonLinkLabel1.LinkClicked += KryptonLinkLabel1_LinkClicked;

			// downscale back image. optional. we can downscale image on krypton render level based on button size
			//kryptonButtonBack.Values.Image = DpiHelper.Default.ScaleBitmapToSize(
			//	(Bitmap)kryptonButtonBack.Values.Image, DpiHelper.Default.ScaleValue( 45 ), false,
			//	System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic );

			kryptonNavigator1.Bar.ItemMinimumSize = DpiHelper.Default.ScaleValue( new Size( 160, 50 ) );
			kryptonNavigator1.Bar.BarFirstItemInset = DpiHelper.Default.ScaleValue( 65 );

			kryptonLabel12.Text = EngineInfo.NameWithoutVersion;

			try
			{
				var assembly = Assembly.GetExecutingAssembly();
				var fvi = FileVersionInfo.GetVersionInfo( assembly.Location );
				string version = fvi.FileVersion;
				kryptonLabelEngineVersion.Text = version;
			}
			catch { }

			//kryptonLinkLabelTokenWhatIsIt.LinkClicked += KryptonLinkLabelTokenWhatIsIt_LinkClicked;
		}

		[Browsable( false )]
		public KryptonPage DefaultPage;

		public void SelectDefaultPage()
		{
			if( !string.IsNullOrEmpty( needStartBuildProduct ) && kryptonPageBuild != null )
			{
				kryptonNavigator1.SelectedPage = kryptonPageBuild;
			}
			else
			{
				if( DefaultPage != null )
					kryptonNavigator1.SelectedPage = DefaultPage;
				else
					kryptonNavigator1.SelectedPage = kryptonPageInfo;
			}
		}

		public string Translate( string text )
		{
			return EditorLocalization.Translate( "Backstage", text );
		}

		private void BackstageMenu_Load( object sender, EventArgs e )
		{
			if( DesignMode )
				return;
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			if( EditorAPI.DarkTheme )
			{
				kryptonNavigator1.StateCommon.Panel.Color1 = Color.FromArgb( 10, 10, 10 );
				kryptonNavigator1.StateSelected.Tab.Back.Color1 = Color.FromArgb( 60, 60, 60 );
				kryptonNavigator1.StatePressed.Tab.Back.Color1 = Color.FromArgb( 60, 60, 60 );
				kryptonNavigator1.StateTracking.Tab.Back.Color1 = Color.FromArgb( 50, 50, 50 );

				//kryptonNavigator1.StateSelected.Tab.Back.Color1 = Color.FromArgb( 70, 70, 70 );
				//kryptonNavigator1.StatePressed.Tab.Back.Color1 = Color.FromArgb( 60, 60, 60 );
				//kryptonNavigator1.StateTracking.Tab.Back.Color1 = Color.FromArgb( 60, 60, 60 );
				kryptonButtonBack.StateCommon.Back.Color1 = Color.FromArgb( 10, 10, 10 );

				BackColor = Color.FromArgb( 10, 10, 10 );

				//kryptonNavigator1.StateCommon.Panel.Color1 = Color.FromArgb( 40, 40, 40 );
				//kryptonNavigator1.StateSelected.Tab.Back.Color1 = Color.FromArgb( 54, 54, 54 );

				//kryptonNavigator1.StateCommon.Panel.Color1 = Color.FromArgb( 54, 54, 54 );
				//kryptonNavigator1.StateSelected.Tab.Back.Color1 = Color.FromArgb( 70, 70, 70 );
				//kryptonNavigator1.StatePressed.Tab.Back.Color1 = Color.FromArgb( 60, 60, 60 );
				//kryptonNavigator1.StateTracking.Tab.Back.Color1 = Color.FromArgb( 60, 60, 60 );
				//kryptonButtonBack.StateCommon.Back.Color1 = Color.FromArgb( 54, 54, 54 );

				foreach( var page in kryptonNavigator1.Pages )
				{
					//page.StateCommon.Page.Color1 = Color.FromArgb( 80, 80, 80 );
					page.StateCommon.Page.Color1 = Color.FromArgb( 40, 40, 40 );

					//page.StateCommon.Page.Color1 = Color.FromArgb( 54, 54, 54 );

					EditorThemeUtility.ApplyDarkThemeToForm( page );
				}

				kryptonLabelLoginError.StateCommon.ShortText.Color1 = Color.Red;
				kryptonLabelInstallPlatformTools.StateCommon.ShortText.Color1 = Color.Red;

				//labelExTokenTransactions.StateCommon.Back.Color1 = Color.FromArgb( 40, 40, 40 );

				//restore colors after apply the dark theme
				kryptonLinkLabel1.LabelStyle = LabelStyle.Custom1;
				kryptonLinkLabel1.StateCommon.ShortText.Color1 = Color.FromArgb( 0, 110, 190 );

				////restore colors after apply the dark theme
				//kryptonLinkLabelTokenWhatIsIt.LabelStyle = LabelStyle.Custom1;
				//kryptonLinkLabelTokenWhatIsIt.StateCommon.ShortText.Color1 = Color.FromArgb( 0, 110, 190 );
			}

			//translate
			{
				foreach( var page in kryptonNavigator1.Pages )
					page.Text = Translate( page.Text );

				EditorLocalization.TranslateForm( "Backstage", kryptonPageInfo );
				EditorLocalization.TranslateForm( "Backstage", kryptonPageNew );
				EditorLocalization.TranslateForm( "Backstage", kryptonPageBuild );
				EditorLocalization.TranslateForm( "Backstage", kryptonPageLogin );
			}

			if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient )
				kryptonPageNew.Visible = false;

			LoginLoad();
			if( DefaultPage == kryptonPageBuild )
				PackagingInit();

			timer1.Start();
		}

		private void BackstageMenu_VisibleChanged( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			if( Visible )
			{
				KryptonWinFormsUtility.LockFormUpdate( EditorForm.Instance );
				EditorForm.Instance.unlockFormUpdateInTimer = DateTime.Now + TimeSpan.FromSeconds( 0.2 );

				InfoInit();
				NewInit();
				//OpenInit();
				packagingNeedInit = true;

				doInitActionsInTimer = true;
			}

			backstageVisible = Visible;
			//EditorForm.Instance.UpdateVisibilityOfFloatingWindows( !Visible );
		}

		private void kryptonNavigator1_TabClicked( object sender, Internal.ComponentFactory.Krypton.Navigator.KryptonPageEventArgs e )
		{
			//if( e.Item == kryptonPageSettings )
			//{
			//	Hide();
			//	EditorAPI.ShowProjectSettings();
			//	return;
			//}

			if( e.Item == kryptonPageBuild )
			{
				if( packagingNeedInit )
					PackagingInit();
				return;
			}

			if( e.Item == kryptonPageExit )
			{
				if( EditorMessageBox.ShowQuestion( Translate( "Exit the app?" ), EMessageBoxButtons.OKCancel ) == EDialogResult.OK )
					EditorForm.Instance.Close();
				return;
			}
		}

		private void kryptonButtonBack_Click( object sender, EventArgs e )
		{
			Hide();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
				return;

			InfoUpdate();
			NewUpdate();
			//OpenUpdate();
			PackagingUpdate();
			LoginUpdate();

			if( Visible && doInitActionsInTimer )
			{
				doInitActionsInTimer = false;

				kryptonNavigator1.Focus();
				kryptonNavigator1.Select();
				kryptonTextBoxInfoName.Select( 0, 0 );

				//start build product
				if( !string.IsNullOrEmpty( needStartBuildProduct ) )
				{
					var productName = needStartBuildProduct;
					var run = needStartBuildProductAndRun;

					needStartBuildProduct = null;
					needStartBuildProductAndRun = false;

					PackagingInit( productName );
					PackagingUpdate();

					if( CanPackageProject() )
						PackageCreate( run );
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void InfoInit()
		{
			//InfoUpdate();
		}

		void InfoUpdate()
		{
			if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient )
				kryptonTextBoxInfoName.Text = ProjectSettings.Get != null ? ProjectSettings.Get.General.CloudProjectName.Trim() : "";
			else
				kryptonTextBoxInfoName.Text = ProjectSettings.Get != null ? ProjectSettings.Get.General.ProjectName.Value.Trim() : "";

			kryptonTextBoxInfoLocation.Text = VirtualFileSystem.Directories.Project;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		volatile bool creationInProgress;
		Task creationTask;
		volatile int creationProgressBarValue;
		string creationDirectory;

		void NewInit()
		{
			{
				string folder = "";
				for( int n = 1; ; n++ )
				{
					folder = Path.Combine( GetNeoAxisProjectsFolder(), "New Project " + n.ToString() );
					if( !Directory.Exists( folder ) )
						break;
				}
				kryptonTextBoxNewFolder.Text = folder;
			}

			var items = new List<ContentBrowser.Item>();

			ContentBrowserItem_Virtual item;

			item = new ContentBrowserItem_Virtual( objectsBrowserNew, null, Translate( "Copy of this project" ) );
			item.Tag = "Copy";
			var firstItem = item;
			items.Add( item );

			item = new ContentBrowserItem_Virtual( objectsBrowserNew, null, Translate( "Initial project (Use NeoAxis Launcher or initial source package to create default project)" ) );
			item.ShowDisabled = true;
			item.Tag = "DefaultProjectDisabled";
			items.Add( item );

			objectsBrowserNew.SetData( items, false );

			objectsBrowserNew.SelectItems( new ContentBrowser.Item[] { firstItem } );
		}

		private void kryptonButtonNewBrowse_Click( object sender, EventArgs e )
		{
			var path = kryptonTextBoxNewFolder.Text;

			while( true )
			{
				if( !string.IsNullOrEmpty( path ) && !Directory.Exists( path ) )
				{
					try
					{
						path = Path.GetDirectoryName( path );
						continue;
					}
					catch { }
				}
				break;
			}

			if( EditorUtility.ShowOpenFileDialog( true, path, null, out string fileName ) )
				kryptonTextBoxNewFolder.Text = fileName;
		}

		void NewUpdate()
		{
			kryptonButtonNewCreate.Enabled = CanCreateProject();
			kryptonTextBoxNewFolder.Enabled = !creationInProgress;
			kryptonButtonNewBrowse.Enabled = !creationInProgress;
			objectsBrowserNew.Enabled = !creationInProgress;

			progressBarNew.Value = creationProgressBarValue;
			progressBarNew.Visible = creationInProgress;
			kryptonButtonNewCancel.Visible = creationInProgress;
		}

		bool CanCreateProject()
		{
			string folder = kryptonTextBoxNewFolder.Text.Trim();
			if( string.IsNullOrEmpty( folder ) )
				return false;
			if( !Path.IsPathRooted( folder ) )
				return false;
			if( objectsBrowserNew.SelectedItems.Length == 0 )
				return false;

			if( objectsBrowserNew.SelectedItems[ 0 ].ShowDisabled )
				return false;
			//var tag = objectsBrowserNew.SelectedItems[ 0 ].Tag as string;
			//if( tag != "Copy" && tag != "Clean" )
			//	return false;

			if( creationInProgress )
				return false;
			return true;
		}

		private void kryptonButtonNewCreate_Click( object sender, EventArgs e )
		{
			string folder = kryptonTextBoxNewFolder.Text.Trim();
			if( string.IsNullOrEmpty( folder ) )
				return;

			try
			{
				while( Directory.Exists( folder ) && !IOUtility.IsDirectoryEmpty( folder ) )
				{
					var text = string.Format( Translate( "Destination folder \'{0}\' is not empty. Clear folder and continue?" ), folder );
					var result = EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel );
					if( result == EDialogResult.Cancel )
						return;

					IOUtility.ClearDirectory( folder );
				}

				if( !Directory.Exists( folder ) )
					Directory.CreateDirectory( folder );

				creationInProgress = true;
				progressBarNew.Visible = true;
				kryptonButtonNewCancel.Visible = true;
				creationDirectory = folder;

				var tag = objectsBrowserNew.SelectedItems[ 0 ].Tag as string;
				creationTask = new Task( CreationFunction, tag );
				creationTask.Start();
			}
			catch( Exception ex )
			{
				EditorMessageBox.ShowWarning( ex.Message );
				return;
			}
		}

		void CreationFunction( object parameter )
		{
			var tag = (string)parameter;

			creationProgressBarValue = 0;

			try
			{
				string sourcePath = VirtualFileSystem.Directories.Project;

				FileInfo[] allFiles = new DirectoryInfo( sourcePath ).GetFiles( "*.*", SearchOption.AllDirectories );

				long totalLength = 0;
				foreach( var fileInfo in allFiles )
					totalLength += fileInfo.Length;

				foreach( string dirPath in Directory.GetDirectories( sourcePath, "*", SearchOption.AllDirectories ) )
				{
					if( Directory.Exists( dirPath ) )
						Directory.CreateDirectory( dirPath.Replace( sourcePath, creationDirectory ) );
				}

				long processedLength = 0;
				foreach( var fileInfo in allFiles )
				{
					if( File.Exists( fileInfo.FullName ) )
						File.Copy( fileInfo.FullName, fileInfo.FullName.Replace( sourcePath, creationDirectory ), false );

					processedLength += fileInfo.Length;
					creationProgressBarValue = (int)( (double)processedLength / (double)totalLength * 100.0 );
					if( creationProgressBarValue > 100 )
						creationProgressBarValue = 100;

					if( !creationInProgress )
						return;
				}

				creationProgressBarValue = 100;

				//open folder
				try
				{
					Win32Utility.ShellExecuteEx( null, creationDirectory );
					//Process.Start( "explorer.exe", creationDirectory );
				}
				catch { }

				//end
				creationInProgress = false;

				ScreenNotifications.Show( Translate( "The project was created successfully." ) );
			}
			catch( Exception ex )
			{
				EditorMessageBox.ShowWarning( ex.Message );
			}
		}

		private void kryptonButtonNewCancel_Click( object sender, EventArgs e )
		{
			//!!!!delete not finished?
			//!!!!ask by MessageBox about cancelling?

			creationInProgress = false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		bool packagingNeedInit;
		ProductBuildInstance packageBuildInstance;

		void PackagingInit( string selectProductName = "" )
		{
			packagingNeedInit = false;

			//files
			string[] files;
			try
			{
				files = VirtualDirectory.GetFiles( "", "*.product", SearchOption.AllDirectories );
			}
			catch
			{
				files = new string[ 0 ];
			}

			var items = new List<ContentBrowser.Item>();
			ContentBrowser.Item selectItem = null;

			kryptonLabelInstallPlatformTools.Visible = false;

			foreach( var virtualPath in files )
			{
				string fileName = Path.GetFileName( virtualPath );

				var alreadyAddedImages = new ESet<string>();

				var packageComponent = ResourceManager.LoadResource<Product>( virtualPath );
				if( packageComponent != null )
				{
					string imageKey = packageComponent.Platform.ToString();

					bool imageExists = false;
					if( !alreadyAddedImages.Contains( imageKey ) )
					{
						var image16 = Properties.Resources.ResourceManager.GetObject( imageKey + "_16", Properties.Resources.Culture ) as Image;
						var image32 = Properties.Resources.ResourceManager.GetObject( imageKey + "_32", Properties.Resources.Culture ) as Image;
						if( image16 != null )
						{
							contentBrowserPackage.AddImageKey( imageKey, image16, image32 );

							alreadyAddedImages.Add( imageKey );
							imageExists = true;
						}
					}

					string packageName = packageComponent.Name;//ProductName.Value;
					if( string.IsNullOrEmpty( packageName ) )
						packageName = "\'No name\'";

					var text = string.Format( "{0} - {1} - {2}", packageName, packageComponent.Platform, virtualPath );
					var item = new ContentBrowserItem_Virtual( contentBrowserPackage, null, text );
					item.Tag = packageComponent;
					if( imageExists )
						item.imageKey = imageKey;

					if( !IsPlatformInstalled( packageComponent.Platform ) )
					{
						item.ShowDisabled = true;
						//if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.Standalone )
						kryptonLabelInstallPlatformTools.Visible = true;
					}

					items.Add( item );

					if( !string.IsNullOrEmpty( selectProductName ) && selectProductName == virtualPath )
						selectItem = item;
				}
			}

			CollectionUtility.MergeSort( items, delegate ( ContentBrowser.Item item1, ContentBrowser.Item item2 )
			{
				var c1 = (Product)item1.Tag;
				var c2 = (Product)item2.Tag;

				var order1 = c1.SortOrder.Value;
				var order2 = c2.SortOrder.Value;

				if( order1 < order2 )
					return -1;
				if( order1 > order2 )
					return 1;

				return string.Compare( c1.Name, c2.Name );
			} );

			contentBrowserPackage.SetData( items, false );

			if( selectItem != null )
				contentBrowserPackage.SelectItems( new ContentBrowser.Item[] { selectItem } );
			else if( items.Count != 0 )
				contentBrowserPackage.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );
		}

		private void kryptonButtonPackageBrowse_Click( object sender, EventArgs e )
		{
			if( EditorUtility.ShowOpenFileDialog( true, kryptonTextBoxPackageDestinationFolder.Text, null, out string fileName ) )
				kryptonTextBoxPackageDestinationFolder.Text = fileName;
		}

		void PackagingUpdate()
		{
#if !DEPLOY
			//check ended
			if( packageBuildInstance != null && ( packageBuildInstance.State != ProductBuildInstance.StateEnum.Building || packageBuildInstance.BuildFunctionFinished ) )
			{
				var instance2 = packageBuildInstance;
				packageBuildInstance = null;

				if( instance2.State == ProductBuildInstance.StateEnum.Error )
					EditorMessageBox.ShowWarning( instance2.Error );
			}

			//update controls

			var building = packageBuildInstance != null && packageBuildInstance.State == ProductBuildInstance.StateEnum.Building;

			var selectedProduct = GetSelectedBuildConfiguration();

			kryptonButtonPackageCreate.Enabled = CanPackageProject();
			kryptonButtonPackageCreateAndRun.Enabled = kryptonButtonPackageCreate.Enabled && selectedProduct != null && selectedProduct.SupportsBuildAndRun;

			if( selectedProduct as Product_Store != null )
				kryptonButtonPackageCreateAndRun.Text = "Build and Upload";
			else
				kryptonButtonPackageCreateAndRun.Text = "Build and Run";

			kryptonTextBoxPackageDestinationFolder.Enabled = !building;
			kryptonButtonPackageBrowse.Enabled = !building;
			contentBrowserPackage.Enabled = !building;
			if( packageBuildInstance != null )
				progressBarBuild.Value = (int)( packageBuildInstance.Progress * 100 );
			progressBarBuild.Visible = building;
			kryptonButtonBuildCancel.Visible = building;
#endif
		}

		bool CanPackageProject()
		{
			var selectedConfiguration = GetSelectedBuildConfiguration();
			if( selectedConfiguration == null || !IsPlatformInstalled( selectedConfiguration.Platform ) )
				return false;
			string folder = kryptonTextBoxPackageDestinationFolder.Text.Trim();
			if( string.IsNullOrEmpty( folder ) )
				return false;
			if( !Path.IsPathRooted( folder ) )
				return false;
			if( contentBrowserPackage.SelectedItems.Length == 0 )
				return false;
			if( packageBuildInstance != null )
				return false;

			return true;
		}

		Product GetSelectedBuildConfiguration()
		{
			if( contentBrowserPackage.SelectedItems.Length != 0 )
				return (Product)contentBrowserPackage.SelectedItems[ 0 ].Tag;
			return null;
		}

		void PackageCreate( bool run )
		{
#if !DEPLOY
			string folder = kryptonTextBoxPackageDestinationFolder.Text.Trim();
			if( string.IsNullOrEmpty( folder ) )
				return;

			var product = GetSelectedBuildConfiguration();
			if( product == null )
				return;

			//clear destination folder
			if( Directory.Exists( folder ) && !IOUtility.IsDirectoryEmpty( folder ) && !( product is Product_Store ) )
			{
				var text = string.Format( Translate( "Destination folder \'{0}\' is not empty. Clear folder and continue?" ), folder );
				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
					return;

				//delete
				try
				{
					DirectoryInfo info = new DirectoryInfo( folder );
					foreach( FileInfo file in info.GetFiles() )
						file.Delete();
					foreach( DirectoryInfo directory in info.GetDirectories() )
						directory.Delete( true );
				}
				catch( Exception e )
				{
					EditorMessageBox.ShowWarning( e.Message );
					return;
				}
			}

			//check login
			if( product is Product_Store )
			{
				var authorEmail = "";
				{
					if( LoginUtility.GetCurrentLicense( out var email, out _ ) )
					{
						if( LoginUtility.GetRequestedFullLicenseInfo( out var license, out _, out var error2 ) )
						{
							if( !string.IsNullOrEmpty( license ) )
								authorEmail = email;
						}
					}
				}

				if( string.IsNullOrEmpty( authorEmail ) )
				{
					EditorMessageBox.ShowWarning( "Please login to build store products." );
					return;
				}
			}

			//start
			try
			{
				if( !Directory.Exists( folder ) )
					Directory.CreateDirectory( folder );

				packageBuildInstance = ProductBuildInstance.Start( product/*GetSelectedBuildConfiguration()*/, folder, run );
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return;
			}

			PackagingUpdate();
#endif
		}

		private void kryptonButtonPackageCreate_Click( object sender, EventArgs e )
		{
			PackageCreate( false );
		}

		private void kryptonButtonPackageCreateAndRun_Click( object sender, EventArgs e )
		{
			PackageCreate( true );
		}

		private void kryptonButtonPackageCancel_Click( object sender, EventArgs e )
		{
			if( packageBuildInstance != null )
				packageBuildInstance.RequestCancel = true;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//void OpenInit()
		//{
		//	var items = new List<ContentBrowser.Item>();

		//	//ContentBrowserItem_Virtual item;

		//	//item = new ContentBrowserItem_Virtual( objectsBrowserOpen, null, "impl Recent Projects" );
		//	//var firstItem = item;
		//	//items.Add( item );
		//	objectsBrowserOpen.SetData( items, false );

		//	//objectsBrowserOpen.SelectItems( new ContentBrowser.Item[] { firstItem } );
		//}

		//void OpenUpdate()
		//{
		//	kryptonButtonOpen.Enabled = false;
		//}

		private void KryptonLinkLabel1_LinkClicked( object sender, EventArgs e )
		{
			Process.Start( new ProcessStartInfo( "https://www.neoaxis.com/" ) { UseShellExecute = true } );
		}

		string GetNeoAxisProjectsFolder()
		{
			return Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "NeoAxis" );
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

		public static bool BackstageVisible
		{
			get { return backstageVisible; }
		}

		/////////////////////////////////////////

		bool IsValidLogin( string login )
		{
			if( string.IsNullOrEmpty( login.Trim() ) )
				return false;

			return true;

			//try
			//{
			//	var addr = new System.Net.Mail.MailAddress( email );
			//	return addr.Address == email;
			//}
			//catch
			//{
			//	return false;
			//}
		}

		void LoginLoad()
		{
			kryptonLabelLoginError.Text = "";

			//if( LoginUtility.GetCurrentLicense( out var email, out _ ) )
			//	kryptonTextBoxLoginEmail.Text = email;

			//LoginUpdate();
		}

		bool loginFirstUpdate = true;

		void LoginUpdate()
		{
			//всё время запрашивается из реестра когда открыт бекстейдж

			if( loginFirstUpdate )
			{
				if( LoginUtility.GetCurrentLicense( out var email, out _ ) )
					kryptonTextBoxLoginEmail.Text = email;

				loginFirstUpdate = false;
			}

			{
				string text;
				//string tokenTransactions = "";
				string error = "";
				if( LoginUtility.GetCurrentLicense( out var email, out _ ) )
				{
					text = email;
					if( LoginUtility.GetRequestedFullLicenseInfo( out var license, out _, /*out tokenTransactions,*/ out var error2 ) )
					{
						//if( !string.IsNullOrEmpty( license ) )
						//	text += " - " + license + " license";
						if( !string.IsNullOrEmpty( license ) )
							text += " You are awesome!";
						error = error2;
					}
					else
						text += " - Not registered";
				}
				else
					text = "Not registered";

				kryptonLabelLicense.Text = text;
				kryptonLabelLoginError.Text = error;

				//labelExTokenTransactions.Text = "Transactions:";
				//kryptonLabelTokenBalance.Text = "0";

				//try
				//{
				//	//  "07/01/2020 +200|07/02/2020 +500"

				//	double balance = 0;

				//	foreach( var s in tokenTransactions.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries ) )
				//	{
				//		var s2 = s.Split( new char[] { ' ' } );
				//		var v = double.Parse( s2[ 1 ] );
				//		balance += v;
				//	}

				//	labelExTokenTransactions.Text = "Transactions:\r\n" + tokenTransactions.Replace( "|", "\r\n" );
				//	kryptonLabelTokenBalance.Text = balance.ToString( "F0" );
				//}
				//catch( Exception e )
				//{
				//	Log.Warning( e.Message );
				//}
			}

			{
				var email = kryptonTextBoxLoginEmail.Text.Trim().ToLower();
				var password = kryptonTextBoxLoginPassword.Text;
				kryptonButtonLogin.Enabled = !string.IsNullOrEmpty( email ) && !string.IsNullOrEmpty( password );
			}
		}

		private void kryptonButtonLogin_Click( object sender, EventArgs e )
		{
			var email = kryptonTextBoxLoginEmail.Text.Trim().ToLower();
			var password = kryptonTextBoxLoginPassword.Text;

			if( string.IsNullOrEmpty( email ) || string.IsNullOrEmpty( password ) )
				return;

			if( !IsValidLogin( email ) )
			{
				EditorMessageBox.ShowWarning( "Invalid login." );
				return;
			}

			LoginUtility.SetCurrentLicense( kryptonTextBoxLoginEmail.Text, kryptonTextBoxLoginPassword.Text );
		}

		private void kryptonButtonRegister_Click( object sender, EventArgs e )
		{
			Process.Start( new ProcessStartInfo( EngineInfo.StoreAddress + "/my-account/" ) { UseShellExecute = true } );
		}

		bool IsPlatformInstalled( SystemSettings.Platform platform )
		{
			if( platform == SystemSettings.Platform.Store )
				return true;
			var path = Path.Combine( VirtualFileSystem.Directories.EngineInternal, "Platforms", platform.ToString() );
			return Directory.Exists( path );
		}

		private void kryptonButtonDonate_Click( object sender, EventArgs e )
		{
			Process.Start( new ProcessStartInfo( "https://www.neoaxis.com/support/donate" ) { UseShellExecute = true } );
		}

		private void kryptonButtonSubscribeToPro_Click( object sender, EventArgs e )
		{
			Process.Start( new ProcessStartInfo( "https://www.neoaxis.com/licensing" ) { UseShellExecute = true } );
		}

		//private void KryptonLinkLabelTokenWhatIsIt_LinkClicked( object sender, EventArgs e )
		//{
		//	Process.Start( new ProcessStartInfo( "https://www.neoaxis.com/neoaxis/token" ) { UseShellExecute = true } );
		//}

		//private void kryptonButtonTokenBuy_Click( object sender, EventArgs e )
		//{
		//	Process.Start( new ProcessStartInfo( "https://www.neoaxis.com/neoaxis/token" ) { UseShellExecute = true } );
		//}
	}
}
#endif