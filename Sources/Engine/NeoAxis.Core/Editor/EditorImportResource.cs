// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Linq;
#if !PROJECT_DEPLOY
using Microsoft.WindowsAPICodePack.Dialogs;
#endif

namespace NeoAxis.Editor
{
	public static class EditorImportResource
	{
		public static void Import( string[] fileNames, string destRealFolder )//Initial )
		{
			try
			{
				if( !Directory.Exists( destRealFolder ) )
					Directory.CreateDirectory( destRealFolder );
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return;
			}

			////check files inside project Data folder
			//{
			//	foreach( var fileName in fileNames )
			//	{
			//		var virtualPath = VirtualPathUtils.GetVirtualPathByReal( fileName );
			//		if( !string.IsNullOrEmpty( virtualPath ) )
			//		{
			//			Log.Warning( "Unable to import from project \'Data\' folder. The file already inside the project." );
			//			return;
			//		}
			//	}
			//}

			//processing

			//!!!!пока просто копирование
			//нужно зависимые скопировать
			//также может какую-то обработку делать

			////select folder if exists files outside Assets folder
			//string destRealFolder = "";
			//{
			//	var existsOutside = fileNames.Any( fileName => string.IsNullOrEmpty( VirtualPathUtility.GetVirtualPathByReal( fileName ) ) );
			//	if( existsOutside )
			//	{
			//		if( string.IsNullOrEmpty( destRealFolderInitial ) )
			//		{
			//			again:;
			//			destRealFolder = VirtualPathUtility.GetRealPathByVirtual( "Import" );
			//			if( !Directory.Exists( destRealFolder ) )
			//				destRealFolder = VirtualPathUtility.GetRealPathByVirtual( "" );

			//			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			//			dialog.InitialDirectory = destRealFolder;
			//			dialog.IsFolderPicker = true;
			//			if( dialog.ShowDialog() == CommonFileDialogResult.Ok )
			//			{
			//				destRealFolder = dialog.FileName;

			//				if( !VirtualPathUtility.GetVirtualPathByReal( destRealFolder, out _ ) )
			//				{
			//					EditorMessageBox.ShowWarning( "Need select folder inside Assets folder." );
			//					//Log.Warning( "Need select folder inside Data folder." );
			//					goto again;
			//				}
			//			}
			//		}
			//		else
			//			destRealFolder = destRealFolderInitial;
			//	}
			//}

			List<string> fileNamesToSelect = new List<string>();

			foreach( var fileName in fileNames )
			{
				var virtualPath = VirtualPathUtility.GetVirtualPathByReal( fileName );
				if( !string.IsNullOrEmpty( virtualPath ) )
				{
					//already inside Data folder
					fileNamesToSelect.Add( fileName );
				}
				else
				{
					//copy to Data folder

					string destinationFileName;
					try
					{
						destinationFileName = Path.Combine( destRealFolder, Path.GetFileName( fileName ) );

						if( Directory.Exists( fileName ) )
							IOUtility.CopyDirectory( fileName, destinationFileName );
						else
							File.Copy( fileName, destinationFileName );
					}
					catch( Exception e )
					{
						EditorMessageBox.ShowWarning( e.Message );
						//Log.Warning( e.Message );
						return;
					}

					fileNamesToSelect.Add( destinationFileName );
				}
			}

			//select new files
			EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( fileNamesToSelect.ToArray() );
			EditorAPI.SelectDockWindow( EditorAPI.FindWindow<ResourcesWindow>() );
		}
	}
}
