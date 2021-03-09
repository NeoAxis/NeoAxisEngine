// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace NeoAxis
{
	class Component_StoreProduct_EditorExtensions : EditorExtensions
	{
		public override void Register()
		{
			//Upload to the Store
			{
				var a = new EditorAction();
				//!!!!
				a.Name = "Prepare for the Store";
				//a.Name = "Upload to the Store";
				a.Description = "Uploads the selected product or all products in the selected folder to the NeoAxis Store.";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.Package_16;
				a.ImageBig = Properties.Resources.Package_32;
				a.QatSupport = true;
				a.ContextMenuSupport = EditorContextMenuWinForms.MenuTypeEnum.Resources;
				a.RibbonText = ("Upload", "");

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
					{
						var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
						foreach( var fileItem in fileItems )
						{
							if( fileItem.IsDirectory )
							{
								bool skip = false;

								if( context.Holder == EditorAction.HolderEnum.ContextMenu )
								{
									var files = Directory.GetFiles( fileItem.FullPath, "*.store", SearchOption.AllDirectories );
									if( files.Length == 0 )
										skip = true;
								}

								if( !skip )
								{
									context.Enabled = true;
									break;
								}
							}

							if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".store" )
							{
								context.Enabled = true;
								break;
							}
						}
					}
				};

				a.Click += delegate ( EditorAction.ClickContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
					{
						var realFileNames = new ESet<string>();
						{
							var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
							foreach( var fileItem in fileItems )
							{
								if( fileItem.IsDirectory )
									realFileNames.AddRangeWithCheckAlreadyContained( Directory.GetFiles( fileItem.FullPath, "*.store", SearchOption.AllDirectories ) );

								if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".store" )
								{
									realFileNames.AddWithCheckAlreadyContained( fileItem.FullPath );
									break;
								}
							}
						}

						var virtualFileNames = new List<string>();
						foreach( var realFileName in realFileNames )
						{
							var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( realFileName );
							if( VirtualFile.Exists( virtualFileName ) )
								virtualFileNames.Add( virtualFileName );
						}
						if( virtualFileNames.Count == 0 )
							return;

						var text = "Upload selected products to the store?\n\n";
						foreach( var fileName in virtualFileNames )
							text += fileName + "\n";
						if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
							return;

						//process
						var item = ScreenNotifications.ShowSticky( "Processing..." );
						try
						{
							foreach( var fileName in virtualFileNames )
							{
								var res = ResourceManager.LoadResource( fileName, true );
								if( res != null )
								{
									var product = res.ResultComponent as Component_StoreProduct;
									if( product != null )
									{
										if( !product.BuildArchive() )
											return;
									}
								}
							}
						}
						catch( Exception e )
						{
							Log.Warning( e.Message );
							return;
						}
						finally
						{
							item.Close();
						}

						if( virtualFileNames.Count > 1 )
							ScreenNotifications.Show( "The products were prepared successfully." );
						else
							ScreenNotifications.Show( "The product was prepared successfully." );

						//open folder in the Explorer
						Win32Utility.ShellExecuteEx( null, Component_StoreProduct.writeToDirectory );

					}
				};

				EditorActions.Register( a );
			}
		}
	}
}
