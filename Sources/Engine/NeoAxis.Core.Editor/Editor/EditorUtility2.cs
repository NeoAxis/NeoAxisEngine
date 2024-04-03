// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace NeoAxis.Editor
{
	public static class EditorUtility2
	{
		public static void ShowRenameComponentDialog( Component component )
		{
			var document = EditorAPI2.GetDocumentByComponent( component );

			var form = new OKCancelTextBoxForm( EditorLocalization2.TranslateLabel( "General", "Name:" ), component.Name, EditorLocalization2.Translate( "General", "Rename" ),
				delegate ( string text, ref string error )
				{
					if( !ComponentUtility.IsValidComponentName( text, out string error2 ) )
					{
						error = error2;
						return false;
					}
					return true;
				},
				delegate ( string text, ref string error )
				{
					if( text != component.Name )
					{
						var oldValue = component.Name;

						//change Name
						component.Name = text;

						//undo
						var undoItems = new List<UndoActionPropertiesChange.Item>();
						var property = (Metadata.Property)MetadataManager.GetTypeOfNetType(
							typeof( Component ) ).MetadataGetMemberBySignature( "property:Name" );
						undoItems.Add( new UndoActionPropertiesChange.Item( component, property, oldValue, new object[ 0 ] ) );

						var action = new UndoActionPropertiesChange( undoItems.ToArray() );
						document.UndoSystem.CommitAction( action );
						document.Modified = true;
					}
					return true;
				}
			);

			form.ShowDialog();
		}

		///////////////////////////////////////////////

		public delegate void ResourcesWindowItemVisibleFilterDelegate( ResourcesWindowItems.Item item, ref bool visible );
		public static event ResourcesWindowItemVisibleFilterDelegate ResourcesWindowItemVisibleFilter;

		public static bool PerformResourcesWindowItemVisibleFilter( ResourcesWindowItems.Item item )
		{
			var result = true;
			ResourcesWindowItemVisibleFilter?.Invoke( item, ref result );
			return result;
		}

		///////////////////////////////////////////////

		static string GetFixedName( string name )
		{
			char[] invalidChars = Path.GetInvalidFileNameChars();
			string trimmedName = name.Trim();
			StringBuilder builder = new StringBuilder();
			foreach( char c in trimmedName )
			{
				char fixedChar = c;
				if( Array.IndexOf<char>( invalidChars, fixedChar ) != -1 )
					fixedChar = '_';
				builder.Append( fixedChar );
			}
			return builder.ToString();
		}

		public static bool ShowOpenFileDialog( bool isFolderPicker, string initialDirectory, IEnumerable<(string rawDisplayName, string extensionList)> filters, out string[] fileNames )
		{
			var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
			dialog.IsFolderPicker = isFolderPicker;
			if( !string.IsNullOrEmpty( initialDirectory ) )
				dialog.InitialDirectory = initialDirectory;
			dialog.Multiselect = true;
			if( filters != null )
			{
				foreach( var filter in filters )
					dialog.Filters.Add( new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter( filter.rawDisplayName, filter.extensionList ) );
			}

			if( dialog.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok && dialog.FileNames.ToArray().Length != 0 )
			{
				fileNames = dialog.FileNames.ToArray();
				return true;
			}
			else
			{
				fileNames = null;
				return false;
			}
		}

		public static bool ShowOpenFileDialog( bool isFolderPicker, string initialDirectory, IEnumerable<(string rawDisplayName, string extensionList)> filters, out string fileName )
		{
			var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
			dialog.IsFolderPicker = isFolderPicker;
			if( !string.IsNullOrEmpty( initialDirectory ) )
				dialog.InitialDirectory = initialDirectory;
			dialog.Multiselect = false;
			if( filters != null )
			{
				foreach( var filter in filters )
					dialog.Filters.Add( new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter( filter.rawDisplayName, filter.extensionList ) );
			}

			if( dialog.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok )
			{
				fileName = dialog.FileName;
				return true;
			}
			else
			{
				fileName = null;
				return false;
			}
		}

		public static bool ShowSaveFileDialog( string initialDirectory, string initialFileName, string filter, out string resultFileName )
		{
			var dialog = new System.Windows.Forms.SaveFileDialog();
			if( !string.IsNullOrEmpty( initialDirectory ) )
				dialog.InitialDirectory = initialDirectory;
			if( !string.IsNullOrEmpty( initialFileName ) )
				dialog.FileName = initialFileName;
			if( !string.IsNullOrEmpty( filter ) )
				dialog.Filter = filter;
			dialog.RestoreDirectory = true;

			try
			{
				dialog.DefaultExt = Path.GetExtension( initialFileName ).Replace( ".", "" );
			}
			catch { }

			if( dialog.ShowDialog() == DialogResult.OK )
			{
				resultFileName = dialog.FileName;
				return true;
			}
			else
			{
				resultFileName = "";
				return false;
			}
		}

		public static void ExportComponentToFile( Component component )
		{
			var componentFolder = "";
			{
				var fileName = ComponentUtility.GetOwnedFileNameOfComponent( component );
				if( !string.IsNullOrEmpty( fileName ) )
					componentFolder = Path.GetDirectoryName( VirtualPathUtility.GetRealPathByVirtual( fileName ) );
			}

			var name = component.Name;
			if( string.IsNullOrEmpty( name ) )
				name = "Component";

			var extension = "component";
			{
				var attribs = component.GetType().GetCustomAttributes<ResourceFileExtensionAttribute>().ToArray();
				if( attribs.Length != 0 )
					extension = attribs[ 0 ].Extension;
			}

			var initialFileName = GetFixedName( name + "." + extension );
			if( !ShowSaveFileDialog( componentFolder, initialFileName, "All files (*.*)|*.*", out var saveAsFileName ) )
				return;

			var context = new Metadata.SaveContext();
			context.SaveRootComponentName = false;

			if( !ComponentUtility.SaveComponentToFile( component, saveAsFileName, context, out var error ) )
				EditorMessageBox.ShowWarning( error );
		}

		public static void PurgeCachedImages()
		{
			try
			{
				//delete files
				{
					var list = new List<string>();
					{
						var directory = Path.Combine( VirtualFileSystem.Directories.Project, "Caches\\Files" );
						if( Directory.Exists( directory ) )
						{
							//dds cache
							foreach( var fullPath in Directory.GetFiles( directory, "*.dds", SearchOption.AllDirectories ) )
							{
								var fileName = fullPath.Substring( directory.Length + 1 );
								fileName = fileName.Substring( 0, fileName.Length - 4 );

								if( !VirtualFile.Exists( fileName ) )
									list.Add( fullPath );
							}

							//preview images
							foreach( var fullPath in Directory.GetFiles( directory, "*.preview.png", SearchOption.AllDirectories ) )
							{
								var fileName = fullPath.Substring( directory.Length + 1 );
								fileName = fileName.Substring( 0, fileName.Length - 12 );

								if( !VirtualFile.Exists( fileName ) )
									list.Add( fullPath );
							}
						}
					}

					foreach( var fullPath in list )
					{
						File.Delete( fullPath );

						var fullPath2 = fullPath + ".info";
						if( File.Exists( fullPath2 ) )
							File.Delete( fullPath2 );
					}
				}

				//delete empty folders
				{
					var cacheDirectory = Path.Combine( VirtualFileSystem.Directories.Project, "Caches\\Files" );
					if( Directory.Exists( cacheDirectory ) )
					{
again2:;
						var again = false;
						foreach( var directory in Directory.GetDirectories( cacheDirectory, "*", SearchOption.AllDirectories ) )
						{
							if( Directory.GetFiles( directory ).Length == 0 && Directory.GetDirectories( directory ).Length == 0 )
							{
								Directory.Delete( directory, false );
								again = true;
							}
						}
						if( again )
							goto again2;
					}
				}
			}
			catch( Exception e )
			{
				Log.Warning( e.Message );
			}
		}

		public static Keys[] ConvertKeys( ProjectSettingsPage_Shortcuts.Keys2[] from )
		{
			var result = new Keys[ from.Length ];
			for( int n = 0; n < result.Length; n++ )
				result[ n ] = (Keys)from[ n ];
			return result;
		}

		public static ProjectSettingsPage_Shortcuts.Keys2[] ConvertKeys( Keys[] from )
		{
			var result = new ProjectSettingsPage_Shortcuts.Keys2[ from.Length ];
			for( int n = 0; n < result.Length; n++ )
				result[ n ] = (ProjectSettingsPage_Shortcuts.Keys2)from[ n ];
			return result;
		}

		public static void RegisterEditorExtensions( Assembly assembly, bool unregister )
		{
			Type[] types;
			try
			{
				types = assembly.GetTypes();
			}
			catch
			{
				return;
			}

			foreach( var type in types )
			{
				if( typeof( EditorExtensions ).IsAssignableFrom( type ) && !type.IsAbstract )
				{
					var constructor = type.GetConstructor( new Type[ 0 ] );
					var obj = (EditorExtensions)constructor.Invoke( new object[ 0 ] );

					if( unregister )
						obj.OnUnregister();
					else
						obj.OnRegister();
				}
			}
		}
	}
}
