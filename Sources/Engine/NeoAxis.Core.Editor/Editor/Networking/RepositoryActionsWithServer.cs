#if CLOUD
#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using NeoAxis.Networking;
using System.Threading;
using System.Threading.Tasks;

namespace NeoAxis.Editor
{
	static class RepositoryActionsWithServer
	{
		static bool Contains( IEnumerable<string> selectedFolders, IEnumerable<string> selectedFiles, string fileName )
		{
			foreach( var folder in selectedFolders )
			{
				if( fileName.Length >= folder.Length && string.Compare( fileName.Substring( 0, folder.Length ), folder, true ) == 0 )
					return true;
			}

			foreach( var selectedFile in selectedFiles )
			{
				if( string.Compare( fileName, selectedFile, true ) == 0 )
					return true;
			}

			return false;
		}

		public async static void Get( ICollection<string> fullPathFolders, ICollection<string> fullPathFiles, long projectID, bool cancelFormTopMost, string cloudProjectFolder, IWin32Window windowOwner )
		{
			var cancelGet = false;

			//show message box window
			var cancelForm = new CancelForm( "Login...", "Repository Get" );
			cancelForm.SetTopMostMultithreaded( cancelFormTopMost );
			cancelForm.FormClosed += delegate ( object sender, FormClosedEventArgs e )
			{
				cancelGet = true;
				CloudProjectEnteringClient.CancelEntering();
			};
			cancelForm.Show( windowOwner );


			try
			{
				//var projectID = EngineInfo.CloudProjectInfo.ID;

				//request verification code from general manager to entering server manager
				var requestCodeResult = await GeneralManagerFunctions.RequestVerificationCodeToEnterProjectAsync( projectID, "DeveloperGet" );// "Edit" );
				if( !string.IsNullOrEmpty( requestCodeResult.Error ) )
				{
					Log.Warning( requestCodeResult.Error );
					ScreenNotifications2.Show( requestCodeResult.Error, true );
					return;
				}
				if( cancelGet )
					return;

				void FileSyncBeforeUpdateFiles( ClientNetworkService_FileSync sender, Dictionary<string, ClientNetworkService_FileSync.FileInfo> filesOnServer, ref string[] filesToDownload, ref string[] filesToDelete, ref bool cancel )
				{
					//update RepositoryServerState.config
					//RepositoryServerState.Init( sender.DestinationFolder );
					if( !RepositoryServerState.WriteConfigFile( filesOnServer.Values, out var error ) )
					{
						Log.Warning( "Unable to write repository server state config file. " + error );
						ScreenNotifications2.Show( "Unable to write repository server state config file. " + error, true );
						cancel = true;
						return;
					}

					//RepositoryLocal.Init( sender.DestinationFolder );
					if( !RepositoryLocal.GetFilesToGet( filesOnServer, out filesToDownload, out filesToDelete, out error ) )
					{
						Log.Warning( "Unable to get files to get. " + error );
						ScreenNotifications2.Show( "Unable to get files to get. " + error, true );
						cancel = true;
						return;
					}

					//filter by selected folder
					{
						var selectedFolders = fullPathFolders.Select( path => RepositoryUtility.GetAllFilesPathByReal( cloudProjectFolder, path ) ).ToArray();
						var selectedFiles = fullPathFiles.Select( path => RepositoryUtility.GetAllFilesPathByReal( cloudProjectFolder, path ) ).ToArray();
						//var selectedFolders = fullPathFolders.Select( path => VirtualPathUtility.GetAllFilesPathByReal( path ) ).ToArray();
						//var selectedFiles = fullPathFiles.Select( path => VirtualPathUtility.GetAllFilesPathByReal( path ) ).ToArray();

						{
							var list = new List<string>();
							foreach( var fileName in filesToDownload )
							{
								if( Contains( selectedFolders, selectedFiles, fileName ) )
									list.Add( fileName );
							}
							filesToDownload = list.ToArray();
						}

						{
							var list = new List<string>();
							foreach( var fileName in filesToDelete )
							{
								if( Contains( selectedFolders, selectedFiles, fileName ) )
									list.Add( fileName );
							}
							filesToDelete = list.ToArray();
						}

						//var folder = VirtualPathUtility.GetCloudProjectPathByReal( fullPathFolder );

						//{
						//	var list = new List<string>();
						//	foreach( var fileName in filesToDownload )
						//	{
						//		if( fileName.Length >= folder.Length && string.Compare( fileName.Substring( 0, folder.Length ), folder, true ) == 0 )
						//			list.Add( fileName );
						//	}
						//	filesToDownload = list.ToArray();
						//}

						//{
						//	var list = new List<string>();
						//	foreach( var fileName in filesToDelete )
						//	{
						//		if( fileName.Length >= folder.Length && string.Compare( fileName.Substring( 0, folder.Length ), folder, true ) == 0 )
						//			list.Add( fileName );
						//	}
						//	filesToDelete = list.ToArray();
						//}
					}

					//sort
					CollectionUtility.MergeSort( filesToDownload, delegate ( string f1, string f2 )
					{
						return string.Compare( f1 + " ", f2 + " ", false );
					}, true );
					CollectionUtility.MergeSort( filesToDelete, delegate ( string f1, string f2 )
					{
						return string.Compare( f1 + " ", f2 + " ", false );
					}, true );


					var toDownload = new List<string>();
					var toDelete = new List<string>();


					var formItems = new List<RepositoryItemsForm.Item>();
					foreach( var fileName in filesToDownload )
					{
						var formItem = new RepositoryItemsForm.Item();
						formItem.FileName = fileName;
						formItem.Prefix = "DOWNLOAD";
						formItem.Tag = "DOWNLOAD";
						formItems.Add( formItem );
					}
					foreach( var fileName in filesToDelete )
					{
						var formItem = new RepositoryItemsForm.Item();
						formItem.FileName = fileName;
						formItem.Prefix = "DELETE";
						formItem.Tag = "DELETE";
						formItems.Add( formItem );
					}

					if( formItems.Count == 0 )
					{
						ScreenNotifications2.Show( "No files to update." );
						filesToDownload = new string[ 0 ];
						filesToDelete = new string[ 0 ];
						//cancel = true;
						return;
					}

					cancelForm.SetTopMostMultithreaded( false );

					var form = new RepositoryItemsForm( "Get Updates", "Select files to update:", formItems.ToArray(), false );
					if( form.ShowDialog( windowOwner ) == DialogResult.Cancel )
					//if( form.ShowDialog() == DialogResult.Cancel )
					{
						cancel = true;
						return;
					}

					cancelForm.SetTopMostMultithreaded( cancelFormTopMost );

					foreach( var item in form.GetCheckedItems() )
					{
						if( (string)item.Tag == "DOWNLOAD" )
							toDownload.Add( item.FileName );
						if( (string)item.Tag == "DELETE" )
							toDelete.Add( item.FileName );
					}


					filesToDownload = toDownload.ToArray();
					filesToDelete = toDelete.ToArray();
				}
				if( cancelGet )
					return;

				//connect to server manager, update files
				var block = requestCodeResult.Data;
				var code = block.GetAttribute( "Code" );
				var serverAddress = block.GetAttribute( "ServerAddress" );
				var enteringResult = await CloudProjectEnteringClient.BeginEnteringAsync( serverAddress, projectID, code, false/*true*/, cloudProjectFolder, true, delegate ( string statusMessage )
				{
					cancelForm.LabelText = statusMessage + "...";
				}, FileSyncBeforeUpdateFiles );

				if( enteringResult == null || !string.IsNullOrEmpty( enteringResult.Error ) )
				{
					if( enteringResult != null )
					{
						Log.Warning( enteringResult.Error );
						ScreenNotifications2.Show( enteringResult.Error, true );
					}
					return;
				}
				if( cancelGet )
					return;

			}
			finally
			{
				cancelForm.CloseMultithreaded();
			}

			ScreenNotifications2.Show( "The update was done successfully." );

			EditorAPI2.FindWindow<ResourcesWindow>()?.Invalidate( true );
		}

		public async static void Commit( ICollection<string> fullPathFolders, ICollection<string> fullPathFiles, long projectID, bool allowCompileScripts, bool cancelFormTopMost, string cloudProjectFolder, IWin32Window windowOwner )
		{
			var cancelCommit = false;

			//compile before commit
			if( allowCompileScripts )
			{
				//var result = EditorMessageBox.ShowQuestion( "Compile scripts before the commit?", EMessageBoxButtons.YesNoCancel );
				//if( result == EDialogResult.Cancel )
				//	return;

				//if( result == EDialogResult.Yes )

				if( ProjectSettings.Get.Repository.CompileScriptsBeforeCommit )
				{
					//Project.dll, Project.Client.dll
					if( !EditorAPI2.BuildProjectSolution( false ) )
						return;

					//C# scripts
					if( ScriptingCSharpEngine.CanCompileScripts )
					{
						//ScreenNotifications.Show( EditorLocalization.Translate( "General", "Building C# scripts..." ) );
						//ScreenNotifications.ShowAllImmediately();

						if( !ScriptingCSharpEngine.ScriptCacheCompile( out var error ) )
						{
							Log.Warning( error );
							ScreenNotifications2.Show( error, true );
							return;
						}

						ScreenNotifications2.Show( EditorLocalization2.Translate( "General", "C# scripts built successfully." ) );
						ScreenNotifications2.ShowAllImmediately();
					}
				}
			}

			//show message box window
			var cancelForm = new CancelForm( "Login...", "Repository Commit" );
			cancelForm.SetTopMostMultithreaded( cancelFormTopMost );
			cancelForm.FormClosed += delegate ( object sender, FormClosedEventArgs e )
			{
				cancelCommit = true;
				CloudProjectEnteringClient.CancelEntering();
				CloudProjectCommitClient.CancelCommit();
			};
			cancelForm.Show( windowOwner );
			//cancelForm.Show();


			try
			{
				//var projectID = EngineInfo.CloudProjectInfo.ID;

				//request verification code from general manager to entering server manager
				var requestCodeResult = await GeneralManagerFunctions.RequestVerificationCodeToEnterProjectAsync( projectID, "DeveloperEdit" ); //"Edit" );
				if( !string.IsNullOrEmpty( requestCodeResult.Error ) )
				{
					cancelForm.CloseMultithreaded();
					Log.Warning( requestCodeResult.Error );
					ScreenNotifications2.Show( requestCodeResult.Error, true );
					return;
				}
				if( cancelCommit )
					return;


				//update repository server state
				{
					void FileSyncBeforeUpdateFiles( ClientNetworkService_FileSync sender, Dictionary<string, ClientNetworkService_FileSync.FileInfo> filesOnServer, ref string[] filesToDownload, ref string[] filesToDelete, ref bool cancel )
					{
						//update RepositoryServerState.config
						//RepositoryServerState.Init( sender.DestinationFolder );
						if( !RepositoryServerState.WriteConfigFile( filesOnServer.Values, out var error ) )
						{
							cancelForm.CloseMultithreaded();
							Log.Warning( "Unable to write repository server state config file. " + error );
							ScreenNotifications2.Show( "Unable to write repository server state config file. " + error, true );
							cancel = true;
							return;
						}

						filesToDownload = new string[ 0 ];
						filesToDelete = new string[ 0 ];
					}

					//connect to server manager, update files
					var block = requestCodeResult.Data;
					var code = block.GetAttribute( "Code" );
					var serverAddress = block.GetAttribute( "ServerAddress" );
					var enteringResult = await CloudProjectEnteringClient.BeginEnteringAsync( serverAddress, projectID, code, true, cloudProjectFolder, true, delegate ( string statusMessage )
					{
						cancelForm.LabelText = statusMessage + "...";
					}, FileSyncBeforeUpdateFiles );

					if( enteringResult == null || !string.IsNullOrEmpty( enteringResult.Error ) )
					{
						if( enteringResult != null )
						{
							cancelForm.CloseMultithreaded();
							Log.Warning( enteringResult.Error );
							ScreenNotifications2.Show( enteringResult.Error, true );
						}
						return;
					}
				}
				if( cancelCommit )
					return;


				//commit
				{
					//RepositoryLocal.Init( sender.DestinationFolder );
					if( !RepositoryLocal.GetFilesToCommit( out var filesToUpload, out var filesToDelete, out var error ) )
					{
						cancelForm.CloseMultithreaded();
						Log.Warning( "Unable to get files to commit. " + error );
						ScreenNotifications2.Show( "Unable to get files to commit. " + error, true );
						return;
					}

					//filter by selected folder
					{
						var selectedFolders = fullPathFolders.Select( path => RepositoryUtility.GetAllFilesPathByReal( cloudProjectFolder, path ) ).ToArray();
						var selectedFiles = fullPathFiles.Select( path => RepositoryUtility.GetAllFilesPathByReal( cloudProjectFolder, path ) ).ToArray();
						//var selectedFolders = fullPathFolders.Select( path => VirtualPathUtility.GetAllFilesPathByReal( path ) ).ToArray();
						//var selectedFiles = fullPathFiles.Select( path => VirtualPathUtility.GetAllFilesPathByReal( path ) ).ToArray();

						{
							var list = new List<string>();
							foreach( var fileName in filesToUpload )
							{
								if( Contains( selectedFolders, selectedFiles, fileName ) )
									list.Add( fileName );
							}
							filesToUpload = list.ToArray();
						}

						{
							var list = new List<string>();
							foreach( var fileName in filesToDelete )
							{
								if( Contains( selectedFolders, selectedFiles, fileName ) )
									list.Add( fileName );
							}
							filesToDelete = list.ToArray();
						}

						//var folder = VirtualPathUtility.GetCloudProjectPathByReal( fullPathFolder );

						//{
						//	var list = new List<string>();
						//	foreach( var fileName in filesToUpload )
						//	{
						//		if( fileName.Length >= folder.Length && string.Compare( fileName.Substring( 0, folder.Length ), folder, true ) == 0 )
						//			list.Add( fileName );
						//	}
						//	filesToUpload = list.ToArray();
						//}

						//{
						//	var list = new List<string>();
						//	foreach( var fileName in filesToDelete )
						//	{
						//		if( fileName.Length >= folder.Length && string.Compare( fileName.Substring( 0, folder.Length ), folder, true ) == 0 )
						//			list.Add( fileName );
						//	}
						//	filesToDelete = list.ToArray();
						//}
					}

					//sort
					CollectionUtility.MergeSort( filesToUpload, delegate ( string f1, string f2 )
					{
						return string.Compare( f1 + " ", f2 + " ", false );
					}, true );
					CollectionUtility.MergeSort( filesToDelete, delegate ( string f1, string f2 )
					{
						return string.Compare( f1 + " ", f2 + " ", false );
					}, true );


					var toUpload = new List<string>();
					var toDelete = new List<string>();


					var formItems = new List<RepositoryItemsForm.Item>();
					foreach( var fileName in filesToUpload )
					{
						var formItem = new RepositoryItemsForm.Item();
						formItem.FileName = fileName;
						formItem.Prefix = "UPLOAD";
						formItem.Tag = "UPLOAD";
						formItems.Add( formItem );
					}
					foreach( var fileName in filesToDelete )
					{
						var formItem = new RepositoryItemsForm.Item();
						formItem.FileName = fileName;
						formItem.Prefix = "DELETE";
						formItem.Tag = "DELETE";
						formItems.Add( formItem );
					}

					if( formItems.Count == 0 )
					{
						//cancelForm.LabelText = "No files to commit.";
						//cancelForm.ButtonText = "OK";
						//cancelForm.Activate();

						//while( !cancelForm.IsDisposed )
						//	await Task.Delay( 1 );

						cancelForm.CloseMultithreaded();
						ScreenNotifications2.Show( "No files to commit." );
						return;
					}

					cancelForm.SetTopMostMultithreaded( false );

					var form = new RepositoryItemsForm( "Commit", "Select files to commit:", formItems.ToArray(), false );
					if( form.ShowDialog( windowOwner ) == DialogResult.Cancel )
						return;

					cancelForm.SetTopMostMultithreaded( cancelFormTopMost );

					foreach( var item in form.GetCheckedItems() )
					{
						if( (string)item.Tag == "UPLOAD" )
							toUpload.Add( item.FileName );
						if( (string)item.Tag == "DELETE" )
							toDelete.Add( item.FileName );
					}


					filesToUpload = toUpload.ToArray();
					filesToDelete = toDelete.ToArray();


					//get a list of files to commit
					var filesToCommit = new List<ClientNetworkService_Commit.FileItem>();
					foreach( var fileToUpload in filesToUpload )
					{
						var commitItem = new ClientNetworkService_Commit.FileItem();
						commitItem.FileName = fileToUpload;

						//FullPath, Length
						commitItem.FullPath = RepositoryUtility.GetRealPathByAllFiles( cloudProjectFolder, commitItem.FileName );
						//commitItem.FullPath = VirtualPathUtility.GetRealPathByAllFiles( commitItem.FileName );
						var fileInfo = new FileInfo( commitItem.FullPath );
						commitItem.Length = fileInfo.Length;

						//SyncMode
						if( RepositoryLocal.GetFileItemData( fileToUpload, out var status, out var syncMode ) )
							commitItem.SyncMode = syncMode.HasValue ? syncMode.Value : RepositorySyncMode.Synchronize;
						else
						{
							var serverItem = RepositoryServerState.GetFileItem( commitItem.FileName );
							if( serverItem != null )
								commitItem.SyncMode = serverItem.SyncMode;
						}

						filesToCommit.Add( commitItem );
					}
					foreach( var fileToDelete in filesToDelete )
					{
						var commitItem = new ClientNetworkService_Commit.FileItem();
						commitItem.FileName = fileToDelete;
						commitItem.Delete = true;
						filesToCommit.Add( commitItem );
					}


					//upload and update RepositoryLocal.config
					if( filesToCommit.Count != 0 )
					{
						//connect to server manager, commit files
						var block = requestCodeResult.Data;
						var code = block.GetAttribute( "Code" );
						var serverAddress = block.GetAttribute( "ServerAddress" );
						var commitResult = await CloudProjectCommitClient.BeginCommitAsync( serverAddress, projectID, code, filesToCommit, delegate ( string statusMessage )
						{
							cancelForm.LabelText = statusMessage + "...";
						} );

						if( commitResult == null || !string.IsNullOrEmpty( commitResult.Error ) )
						{
							if( commitResult != null )
							{
								cancelForm.CloseMultithreaded();
								Log.Warning( commitResult.Error );
								ScreenNotifications2.Show( commitResult.Error, true );
							}
							return;
						}


						//remove processed items from RepositoryLocal.config
						{
							var fileNames = new ESet<string>();
							foreach( var fileToCommit in filesToCommit )
								fileNames.AddWithCheckAlreadyContained( fileToCommit.FileName );
							//fileNames.AddRangeWithCheckAlreadyContained( filesToUpload );
							//fileNames.AddRangeWithCheckAlreadyContained( filesToDelete );

							if( !RepositoryLocal.RemoveFileItems( fileNames ) )
							{
								cancelForm.CloseMultithreaded();
								Log.Warning( "Unable to remove file items from local repository." );
								ScreenNotifications2.Show( "Unable to remove file items from local repository.", true );
								return;
							}
						}

					}
				}
				if( cancelCommit )
					return;


				//update repository server state
				{
					void FileSyncBeforeUpdateFiles( ClientNetworkService_FileSync sender, Dictionary<string, ClientNetworkService_FileSync.FileInfo> filesOnServer, ref string[] filesToDownload, ref string[] filesToDelete, ref bool cancel )
					{
						//update RepositoryServerState.config
						//RepositoryServerState.Init( sender.DestinationFolder );
						if( !RepositoryServerState.WriteConfigFile( filesOnServer.Values, out var error ) )
						{
							cancelForm.CloseMultithreaded();
							Log.Warning( "Unable to write repository server state config file. " + error );
							ScreenNotifications2.Show( "Unable to write repository server state config file. " + error, true );
							cancel = true;
							return;
						}

						filesToDownload = new string[ 0 ];
						filesToDelete = new string[ 0 ];
					}

					//connect to server manager, update files
					var block = requestCodeResult.Data;
					var code = block.GetAttribute( "Code" );
					var serverAddress = block.GetAttribute( "ServerAddress" );
					var enteringResult = await CloudProjectEnteringClient.BeginEnteringAsync( serverAddress, projectID, code, true, cloudProjectFolder, true, delegate ( string statusMessage )
					{
						cancelForm.LabelText = statusMessage + "...";
					}, FileSyncBeforeUpdateFiles );

					if( enteringResult == null || !string.IsNullOrEmpty( enteringResult.Error ) )
					{
						if( enteringResult != null )
						{
							cancelForm.CloseMultithreaded();
							Log.Warning( enteringResult.Error );
							ScreenNotifications2.Show( enteringResult.Error, true );
						}
						return;
					}
				}
				if( cancelCommit )
					return;

				cancelForm.CloseMultithreaded();
				ScreenNotifications2.Show( "The commit was done successfully." );

				EditorAPI2.FindWindow<ResourcesWindow>()?.Invalidate( true );
			}
			finally
			{
				cancelForm.CloseMultithreaded();
			}
		}
	}
}
#endif
#endif