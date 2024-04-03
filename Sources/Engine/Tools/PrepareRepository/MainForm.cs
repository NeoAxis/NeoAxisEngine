// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NeoAxis;

namespace PrepareRepository
{
	public partial class MainForm : Form
	{
		string projectFolder;

		//

		public MainForm()
		{
			InitializeComponent();

			Log.Handlers.ErrorHandler += Handlers_ErrorHandler;
			Log.Handlers.WarningHandler += Handlers_WarningHandler;
		}

		private void Handlers_ErrorHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			MessageBox.Show( this, text, Text, MessageBoxButtons.OK, MessageBoxIcon.Error );
			handled = true;
		}

		private void Handlers_WarningHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			MessageBox.Show( this, text, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning );
			handled = true;
		}

		void DeletePathIfExists( string path )
		{
			var realPath = Path.Combine( projectFolder, path );
			if( Directory.Exists( realPath ) )
				Directory.Delete( realPath, true );
			else if( File.Exists( realPath ) )
				File.Delete( realPath );
		}

		private void buttonPrepare_Click( object sender, EventArgs e )
		{
			projectFolder = textBoxProjectPath.Text;

			if( !Directory.Exists( projectFolder ) )
			{
				Log.Warning( "Project folder is not exists." );
				return;
			}

			//delete not needed files
			{
				DeletePathIfExists( "Sources" );
				DeletePathIfExists( "packages" );
				DeletePathIfExists( ".vs" );
				//DeletePathIfExists( ".vscode" );
				DeletePathIfExists( ".editorconfig" );
				DeletePathIfExists( ".gitattributes" );
				DeletePathIfExists( ".gitignore" );

				DeletePathIfExists( @"Project\Sources\SampleWidgetWinForms" );
				DeletePathIfExists( @"Project\Sources\SampleWidgetWPF" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWinForms.exe" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWPF.exe" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWinForms.deps.json" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWinForms.runtimeconfig.dev.json" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWinForms.runtimeconfig.json" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWPF.deps.json" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWPF.runtimeconfig.dev.json" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWPF.runtimeconfig.json" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWinForms.pdb" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWPF.pdb" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWinForms.dll" );
				DeletePathIfExists( @"Project\Binaries\SampleWidgetWPF.dll" );

				//delete obj, .vs folders
				foreach( var directory in Directory.GetDirectories( projectFolder, "*", SearchOption.AllDirectories ) )
				{
					var name = Path.GetFileName( directory );
					if( name == "obj" || name == ".vs" )
						DeletePathIfExists( directory );
				}

				//delete .user files
				foreach( var fileName in Directory.GetFiles( projectFolder, "*", SearchOption.AllDirectories ) )
				{
					var ext = Path.GetExtension( fileName );
					if( ext == ".user" )
						DeletePathIfExists( fileName );
				}

				DeletePathIfExists( @"Project\Caches\Files" );
				DeletePathIfExists( @"Project\Caches\ShaderCache" );
				DeletePathIfExists( @"Project\EnginePackages" );
			}

			//prepare RepositoryServer.config
			{
				var serverOnlyFiles = new ESet<string>();
				{
					//find files in folders
					{
						var folders = new List<string>();
						folders.Add( @"Project\Binaries\NeoAxis.Internal\Tips" );
						folders.Add( @"Project\Binaries\NeoAxis.Internal\Localization" );
						folders.Add( @"Project\Binaries\NeoAxis.Internal\Tools\PlatformTools" );

						//no compile scripts
						folders.Add( @"Project\Binaries\NeoAxis.Internal\Platforms\Windows\dotnet" );

						//UIWebBrowser is disabled by default
						folders.Add( @"Project\Binaries\NeoAxis.Internal\Platforms\Windows\CefGlue" );

						//cubemap processing tools
						folders.Add( @"Project\Binaries\NeoAxis.Internal\Tools\Filament" );

						//satellite assemblies
						folders.Add( @"Project\Binaries\cs" );
						folders.Add( @"Project\Binaries\de" );
						folders.Add( @"Project\Binaries\es" );
						folders.Add( @"Project\Binaries\fr" );
						folders.Add( @"Project\Binaries\it" );
						folders.Add( @"Project\Binaries\ja" );
						folders.Add( @"Project\Binaries\ko" );
						folders.Add( @"Project\Binaries\pl" );
						folders.Add( @"Project\Binaries\pt-BR" );
						folders.Add( @"Project\Binaries\ru" );
						folders.Add( @"Project\Binaries\tr" );
						folders.Add( @"Project\Binaries\zh-Hans" );
						folders.Add( @"Project\Binaries\zh-Hant" );

						//project sources
						folders.Add( @"Project\Sources" );
						folders.Add( @"Project\Properties" );


						foreach( var folder in folders )
						{
							var folderFullPath = Path.Combine( projectFolder, folder );
							if( Directory.Exists( folderFullPath ) )
							{
								foreach( var fullPath in Directory.GetFiles( folderFullPath, "*", SearchOption.AllDirectories ) )
									serverOnlyFiles.Add( fullPath.Substring( projectFolder.Length + 1 ) );
							}
						}
					}

					//select files by name
					{
						serverOnlyFiles.Add( "Repository.settings" );

						//editor
						serverOnlyFiles.Add( @"Project\Binaries\NeoAxis.Editor.exe" );
						serverOnlyFiles.Add( @"Project\Binaries\NeoAxis.Editor.exe.config" );
						serverOnlyFiles.Add( @"Project\Binaries\NeoAxis.Editor.deps.json" );
						serverOnlyFiles.Add( @"Project\Binaries\NeoAxis.Editor.runtimeconfig.dev.json" );
						serverOnlyFiles.Add( @"Project\Binaries\NeoAxis.Editor.runtimeconfig.json" );
						serverOnlyFiles.Add( @"Project\Binaries\Project.dll" );
						serverOnlyFiles.Add( @"Project\Binaries\Project.deps.json" );
						serverOnlyFiles.Add( @"Project\Binaries\Project.pdb" );

						//import tools
						serverOnlyFiles.Add( @"Project\Binaries\NeoAxis.Internal\Platforms\Windows\FbxWrapperNative.dll" );
						serverOnlyFiles.Add( @"Project\Binaries\NeoAxis.Internal\Platforms\Windows\assimp-vc141-mt.dll" );

						//project sources
						serverOnlyFiles.Add( @"Project\Project.Android.csproj" );
						serverOnlyFiles.Add( @"Project\Project.Client.csproj" );
						serverOnlyFiles.Add( @"Project\Project.csproj" );
						serverOnlyFiles.Add( @"Project\Project.iOS.csproj" );
						serverOnlyFiles.Add( @"Project\Project.UWP.csproj" );
						serverOnlyFiles.Add( @"Project\Project.Web.csproj" );
						serverOnlyFiles.Add( @"Project\Build.Android.sln" );
						serverOnlyFiles.Add( @"Project\Build.UWP.sln" );
						serverOnlyFiles.Add( @"Project\Project.Client.sln" );
						serverOnlyFiles.Add( @"Project\Project.sln" );
						serverOnlyFiles.Add( @"Project\Project_sln_used_by_editor.txt" );
						serverOnlyFiles.Add( @"Project\Directory.Build.props" );
					}

					//debug files
					{
						var folder = Path.Combine( projectFolder, @"Project\Binaries" );

						var fullPaths = new List<string>();
						foreach( var fileName in Directory.GetFiles( folder, "*.pdb", SearchOption.TopDirectoryOnly ) )
							fullPaths.Add( fileName );
						foreach( var fileName in Directory.GetFiles( folder, "*.xml", SearchOption.TopDirectoryOnly ) )
							fullPaths.Add( fileName );
						foreach( var fileName in Directory.GetFiles( folder, "*.mdb", SearchOption.TopDirectoryOnly ) )
							fullPaths.Add( fileName );

						foreach( var fullPath in fullPaths )
							serverOnlyFiles.AddWithCheckAlreadyContained( fullPath.Substring( projectFolder.Length + 1 ) );
					}
				}

				var rootBlock = new TextBlock();
				foreach( var fileName in serverOnlyFiles )
				{
					var block = rootBlock.AddChild( "Item" );
					block.SetAttribute( "FileName", fileName );
					block.SetAttribute( "SyncMode", NeoAxis.Networking.RepositorySyncMode.ServerOnly.ToString() );
				}

				var path = Path.Combine( projectFolder, "RepositoryServer.config" );
				File.WriteAllText( path, rootBlock.DumpToString() );
			}

			////prepare Repository.settings
			//{
			//	var settings = new NeoAxis.Networking.RepositorySettingsFile.Settings();

			//	//developer can add shader and files cache later
			//	settings.IgnoreFolders.AddWithCheckAlreadyContained( @"Project\Caches\Files" );
			//	settings.IgnoreFolders.AddWithCheckAlreadyContained( @"Project\Caches\ShaderCache" );

			//	//C# scripts can't be compiled on client. need precompile it in the editor and sync with clients
			//	//settings.IgnoreFolders.AddWithCheckAlreadyContained( @"Project\Caches\CSharpScripts" );

			//	//ignore user files
			//	settings.IgnoreFolders.AddWithCheckAlreadyContained( @"Project\User settings" );

			//	//settings.IgnoreFolders.AddWithCheckAlreadyContained( @"Project\obj" );

			//	var path = Path.Combine( projectFolder, "Repository.settings" );
			//	if( !NeoAxis.Networking.RepositorySettingsFile.Save( path, settings, out var error ) )
			//	{
			//		Log.Warning( error );
			//		return;
			//	}
			//}

			MessageBox.Show( this, "Done.", Text, MessageBoxButtons.OK );
		}
	}
}
