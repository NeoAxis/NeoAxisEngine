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
using System.Linq;
using ComponentFactory.Krypton.Ribbon;
#if !PROJECT_DEPLOY
using Microsoft.WindowsAPICodePack.Dialogs;
#endif
using ComponentFactory.Krypton.Toolkit;
using System.IO;

namespace NeoAxis.Editor
{
	public static class EditorStandardActions
	{
		static bool initialized;

		//

		internal static void Register()
		{
			if( initialized )
				return;
			initialized = true;

#if !PROJECT_DEPLOY
			RegisterGeneral();
			RegisterWindows();
			RegisterScene();
			RegisterTerrain();
			RegisterMesh();
			RegisterScripting();
			RegisterImage();
			RegisterTools();
			RegisterCamera();
#endif
		}

		/////////////////////////////////////////

#if !PROJECT_DEPLOY

		static void RegisterGeneral()
		{
			//Restart
			{
				var a = new EditorAction();
				a.Name = "Restart";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Restarts the editor.";
				a.ImageSmall = Properties.Resources.Refresh_16;
				a.ImageBig = Properties.Resources.Refresh_32;
				a.QatSupport = true;
				a.QatAddByDefault = true;
				a.RibbonText = ("Restart", "App");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
					//context.Checked = Time.Current % 0.4 < 0.2;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					EditorAPI.BeginRestartApplication();
				};
				EditorActions.Register( a );
			}

			//New Resource
			{
				var a = new EditorAction();
				a.Name = "New Resource";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Creates a new resource in the project.";
				a.ImageSmall = Properties.Resources.New_16;
				a.ImageBig = Properties.Resources.New_32;
				a.QatSupport = true;
				a.QatAddByDefault = true;
				a.RibbonText = ("New", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var initData = new NewObjectWindow.CreationDataClass();

					var window = EditorAPI.FindWindow<ResourcesWindow>();
					string directory = window.ContentBrowser1.GetDirectoryPathOfSelectedFileOrParentDirectoryItem();
					if( !string.IsNullOrEmpty( directory ) )
						initData.initFileCreationDirectory = VirtualPathUtility.GetVirtualPathByReal( directory );

					//initData.initFileCreationDirectory = VirtualDirectory.Exists( "New" ) ? "New" : "";
					//initData.initDemandedType = MetadataManager.GetTypeOfNetType( typeof( Component ) );
					EditorAPI.OpenNewObjectWindow( initData );
				};
				EditorActions.Register( a );
			}

			//Import Resource
			{
				var a = new EditorAction();
				a.Name = "Import Resource";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Imports resources to the project.";
				a.ImageSmall = Properties.Resources.Import_16;
				a.ImageBig = Properties.Resources.Import_32;
				a.QatSupport = true;
				a.QatAddByDefault = true;
				a.RibbonText = ("Import", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					string initialDestinationFolder = "";

					var selectedItems = EditorAPI.FindWindow<ResourcesWindow>().ContentBrowser1.SelectedItems;
					if( selectedItems.Length == 1 && selectedItems[ 0 ] is ContentBrowserItem_File fileItem )
					{
						if( fileItem.IsDirectory )
							initialDestinationFolder = VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath );
					}

					EditorAPI.OpenImportWindow( initialDestinationFolder );
				};
				EditorActions.Register( a );
			}

			//Save
			{
				var a = new EditorAction();
				a.Name = "Save";
				a.Description = "Saves the current document.";
				a.ImageSmall = Properties.Resources.Save_16;
				a.ImageBig = Properties.Resources.Save_32;
				a.ShortcutKeys = new Keys[] { Keys.Control | Keys.S };
				a.QatSupport = true;
				a.QatAddByDefault = true;
				EditorActions.Register( a );
			}

			//Save As
			{
				var a = new EditorAction();
				a.Name = "Save As";
				a.Description = "Saves the current document in a different location, or with a different file name.";
				a.ImageSmall = Properties.Resources.Save_16;
				a.ImageBig = Properties.Resources.Save_32;
				a.QatSupport = true;
				a.QatAddByDefault = true;
				EditorActions.Register( a );
			}

			//Save All
			{
				var a = new EditorAction();
				a.Name = "Save All";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Saves all modified documents.";
				a.ImageSmall = Properties.Resources.SaveAll_16;
				a.ImageBig = Properties.Resources.SaveAll_32;
				a.QatSupport = true;
				a.QatAddByDefault = true;
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					if( EditorAPI.ExistsModifiedDocuments() )
						context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					EditorAPI.SaveDocuments();
				};
				EditorActions.Register( a );
			}

			//Undo
			{
				var a = new EditorAction();
				a.Name = "Undo";
				a.Description = "Undoes the last action.";
				a.ImageSmall = Properties.Resources.Undo_16;
				a.ImageBig = Properties.Resources.Undo_32;
				a.ShortcutKeys = new Keys[] { Keys.Control | Keys.Z };
				a.QatSupport = true;
				a.QatAddByDefault = true;
				EditorActions.Register( a );
			}

			//Redo
			{
				var a = new EditorAction();
				a.Name = "Redo";
				a.Description = "Redos the last action.";
				a.ImageSmall = Properties.Resources.Redo_16;
				a.ImageBig = Properties.Resources.Redo_32;
				a.ShortcutKeys = new Keys[] { Keys.Control | Keys.Y };
				a.QatSupport = true;
				a.QatAddByDefault = true;
				EditorActions.Register( a );
			}

			//Cut
			{
				var a = new EditorAction();
				a.Name = "Cut";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				//a.Description = "Clone selected object.";
				a.ImageSmall = Properties.Resources.Cut_16;
				a.ImageBig = Properties.Resources.Cut_32;
				a.ShortcutKeys = new Keys[] { Keys.Control | Keys.X };
				a.QatSupport = true;
				a.QatAddByDefault = false;

				//!!!!может не так
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var items = context.ObjectsInFocus.Objects.OfType<ContentBrowser.Item>().ToArray();
					if( items.Length != 0 && items[ 0 ].Owner.CanCut() )
						context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var items = context.ObjectsInFocus.Objects.OfType<ContentBrowser.Item>().ToArray();
					if( items.Length != 0 )
						items[ 0 ].Owner.Cut();
				};

				EditorActions.Register( a );
			}

			//Copy
			{
				var a = new EditorAction();
				a.Name = "Copy";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				//a.Description = "Clone selected object.";
				a.ImageSmall = Properties.Resources.Copy_16;
				a.ImageBig = Properties.Resources.Copy_32;
				a.ShortcutKeys = new Keys[] { Keys.Control | Keys.C, Keys.Control | Keys.Insert };
				a.QatSupport = true;
				a.QatAddByDefault = false;

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var items = context.ObjectsInFocus.Objects.OfType<ContentBrowser.Item>().ToArray();
					if( items.Length != 0 && items[ 0 ].Owner.CanCopy() )
						context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var items = context.ObjectsInFocus.Objects.OfType<ContentBrowser.Item>().ToArray();
					if( items.Length != 0 )
						items[ 0 ].Owner.Copy();
				};

				EditorActions.Register( a );
			}

			//Paste
			{
				var a = new EditorAction();
				a.Name = "Paste";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				//a.Description = "Clone selected object.";
				a.ImageSmall = Properties.Resources.Paste_16;
				a.ImageBig = Properties.Resources.Paste_32;
				a.ShortcutKeys = new Keys[] { Keys.Control | Keys.V, Keys.Shift | Keys.Insert };
				a.QatSupport = true;
				a.QatAddByDefault = false;

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var items = context.ObjectsInFocus.Objects.OfType<ContentBrowser.Item>().ToArray();
					if( items.Length != 0 && items[ 0 ].Owner.CanPaste( out _, out _, out _, out _ ) )
						context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var items = context.ObjectsInFocus.Objects.OfType<ContentBrowser.Item>().ToArray();
					if( items.Length != 0 )
						items[ 0 ].Owner.Paste();
				};

				EditorActions.Register( a );
			}

			//Duplicate
			{
				var a = new EditorAction();
				a.Name = "Duplicate";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Duplicates selected objects.";
				//!!!!Clone_16, 32
				a.ImageSmall = Properties.Resources.Copy_16;
				a.ImageBig = Properties.Resources.Copy_32;
				a.ShortcutKeys = new Keys[] { Keys.Control | Keys.D };
				a.QatSupport = true;
				a.QatAddByDefault = true;
				a.RibbonText = ("Copy", "");
				EditorActions.Register( a );
			}

			//Delete
			{
				var a = new EditorAction();
				a.Name = "Delete";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Deletes selected objects.";
				a.ImageSmall = Properties.Resources.Delete_16;
				a.ImageBig = Properties.Resources.Delete_32;
				a.ShortcutKeys = new Keys[] { Keys.Delete };//, Keys.Control | Keys.D };
				a.QatSupport = true;
				a.QatAddByDefault = true;

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>().ToArray();
					if( fileItems.Length != 0 )
					{
						//!!!!если не Assets

						if( fileItems[ 0 ].Owner.CanDeleteObjects( out _ ) )
							context.Enabled = true;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>().ToArray();
					if( fileItems.Length != 0 )
					{
						//!!!!если не Assets

						fileItems[ 0 ].Owner.TryDeleteObjects();
					}
				};

				EditorActions.Register( a );
			}

			//Rename
			{
				var a = new EditorAction();
				a.Name = "Rename";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Renames the selected object.";
				a.ImageSmall = Properties.Resources.Rename_16;
				a.ImageBig = Properties.Resources.Rename_32;
				a.ShortcutKeys = new Keys[] { Keys.F2 };
				a.QatSupport = true;
				a.RibbonText = ("Rename", "");
				EditorActions.Register( a );
			}

			//Select
			{
				var a = new EditorAction();
				a.Name = "Select";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Selection mode of the transform tool.";
				a.ImageSmall = Properties.Resources.Select_16;
				a.ImageBig = Properties.Resources.Select_32;
				a.ShortcutKeys = new Keys[] { Keys.R };
				a.QatSupport = true;
				a.QatAddByDefault = true;
				EditorActions.Register( a );
			}

			//Move
			{
				var a = new EditorAction();
				a.Name = "Move";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Movement mode of the transform tool.";
				a.ImageSmall = Properties.Resources.Move_16;
				a.ImageBig = Properties.Resources.Move_32;
				a.ImageBig_Dark = Properties.Resources.Move_32_Dark;
				a.ShortcutKeys = new Keys[] { Keys.T };
				a.QatSupport = true;
				a.QatAddByDefault = true;
				EditorActions.Register( a );
			}

			//Rotate
			{
				var a = new EditorAction();
				a.Name = "Rotate";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Rotation mode of the transform tool.";
				a.ImageSmall = Properties.Resources.Rotate_16;
				a.ImageBig = Properties.Resources.Rotate_32;
				a.ShortcutKeys = new Keys[] { Keys.Y };
				a.QatSupport = true;
				a.QatAddByDefault = true;
				EditorActions.Register( a );
			}

			//Scale
			{
				var a = new EditorAction();
				a.Name = "Scale";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Scaling mode of the transform tool.";
				a.ImageSmall = Properties.Resources.Scale_16;
				a.ImageBig = Properties.Resources.Scale_32;
				a.ImageBig_Dark = Properties.Resources.Scale_32_Dark;
				a.ShortcutKeys = new Keys[] { Keys.U };
				a.QatSupport = true;
				a.QatAddByDefault = true;
				EditorActions.Register( a );
			}

			//Move, Rotate, Scale
			{
				var a = new EditorAction();
				a.Name = "Move, Rotate, Scale";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Select 'move rotate and scale' tool.";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				//!!!!так? другие тоже
				//a.ShortcutKeys = Keys.I;
				a.QatSupport = true;
				//a.qatAddByDefault = true;
				a.RibbonText = ("Move", "Rotate");
				EditorActions.Register( a );
			}

			//Transform Using Local Coordinates
			{
				var a = new EditorAction();
				a.Name = "Transform Using Local Coordinates";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Enables the use of local transform coordinates.";
				a.ImageSmall = Properties.Resources.TransformUsingLocalCoordinates_16;
				a.ImageBig = Properties.Resources.TransformUsingLocalCoordinates_32;
				a.ImageBig_Dark = Properties.Resources.TransformUsingLocalCoordinates_32_Dark;
				a.QatSupport = true;
				a.QatAddByDefault = true;
				a.RibbonText = ("Local", "");
				//a.ribbonText = ("Local", "Transform");
				EditorActions.Register( a );
			}

			//Play
			{
				var a = new EditorAction();
				a.Name = "Play";
				a.Description = "Saves and runs the document in the player application.";
				a.ImageSmall = Properties.Resources.PlayGreen_16;
				a.ImageBig = Properties.Resources.PlayGreen_32;
				a.ShortcutKeys = new Keys[] { Keys.F5 };
				a.QatSupport = true;
				a.QatAddByDefault = true;
				EditorActions.Register( a );
			}

			//Run Player
			{
				var a = new EditorAction();
				a.Name = "Run Player";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Runs the player application.";
				a.ImageSmall = Properties.Resources.PlayGreen_16;
				a.ImageBig = Properties.Resources.PlayGreen_32;
				a.ShortcutKeys = new Keys[] { Keys.F6 };
				a.QatSupport = true;
				a.QatAddByDefault = true;
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					EditorAPI.SaveDocuments();
					RunSimulation.Run( "", RunSimulation.RunMethod.Player );
				};
				EditorActions.Register( a );
			}

			//Project settings
			{
				var a = new EditorAction();
				a.Name = "Project Settings";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Opens the settings of the project.";
				a.ImageSmall = Properties.Resources.Options_16;
				a.ImageBig = Properties.Resources.Options_32;
				a.ImageBig_Dark = Properties.Resources.Options_32_Dark;
				a.QatSupport = true;
				a.QatAddByDefault = true;
				a.RibbonText = ("Settings", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					EditorAPI.ShowProjectSettings();
				};
				EditorActions.Register( a );
			}

			//Manual
			{
				var a = new EditorAction();
				a.Name = "Manual";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Opens the online documentation.";
				a.ImageSmall = Properties.Resources.Help_16;
				a.ImageBig = Properties.Resources.Help_32;
				a.QatSupport = true;
				a.RibbonText = ("Manual", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					Process.Start( "https://www.neoaxis.com/docs/html/Manual_Root.htm" );
				};
				EditorActions.Register( a );
			}

			//Tips
			{
				var a = new EditorAction();
				a.Name = "Tips";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Opens the Tips window.";
				//a.Description = "Opens the Tip of the Day.";
				a.ImageSmall = Properties.Resources.Grow_16;
				a.ImageBig = Properties.Resources.Grow_32;
				a.QatSupport = true;
				a.RibbonText = ("Tips", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
					context.Checked = EditorAPI.FindWindow<TipsWindow>() != null;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					EditorAPI.ShowTips();
				};
				EditorActions.Register( a );
			}

			//Packages
			{
				var a = new EditorAction();
				a.Name = "Packages";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Opens the package manager.";
				a.ImageSmall = Properties.Resources.Addon_16;
				a.ImageBig = Properties.Resources.Addon_32;
				a.QatSupport = true;
				a.RibbonText = ("Addons", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
					context.Checked = EditorAPI.FindWindow<PackagesWindow>() != null;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					EditorAPI.OpenOrClosePackages();
				};
				EditorActions.Register( a );
			}

			////Addons
			//{
			//	var a = new EditorAction();
			//	a.Name = "Addons";
			//	a.CommonType = EditorAction.CommonTypeEnum.General;
			//	a.Description = "Configure downloadable packages.";
			//	a.ImageSmall = Properties.Resources.Addon_16;
			//	a.ImageBig = Properties.Resources.Addon_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("Addons", "");
			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//		context.Enabled = true;
			//	};
			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//		var form = new PackageManagerForm();
			//		form.ShowDialog();
			//	};
			//	EditorActions.Register( a );
			//}

			//Asset Store
			{
				var a = new EditorAction();
				a.Name = "Store";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Opens the Asset Store.";
				a.ImageSmall = Properties.Resources.Shop_16;
				a.ImageBig = Properties.Resources.Shop_32;
				a.QatSupport = true;
				a.RibbonText = ("Store", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
					context.Checked = EditorAPI.FindWindow<StoreDocumentWindow>() != null;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					EditorAPI.OpenOrCloseStore();
				};
				EditorActions.Register( a );
			}
		}

		/////////////////////////////////////////

		static void RegisterWindows()
		{
			//Resources
			EditorActions.RegisterDockWindowAction( "Resources Window", ("Resources", ""), typeof( ResourcesWindow ) );

			//{
			//	var a = new EditorAction();
			//	a.name = "Resources Window";
			//	a.imageSmall = Properties.Resources.Default_16;
			//	a.imageBig = Properties.Resources.Default_32;
			//	a.qatSupport = true;
			//	a.ribbonText = ("Resources", "");
			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//		var window = EditorForm.Instance.WorkspaceController.FindWindow<ResourcesWindow>();
			//		if( window != null )
			//		{
			//			context.Enabled = true;
			//			context.Checked = window != null && window.Visible;
			//		}
			//	};
			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//		var window = EditorForm.Instance.WorkspaceController.FindWindow<ResourcesWindow>();
			//		if( window != null )
			//			EditorForm.Instance.WorkspaceController.SetDockWindowVisibility( window, !window.Visible );
			//	};
			//	EditorActions.Register( a );
			//}

			////Resources New
			//{
			//	var a = new EditorAction();
			//	a.Name = "Resources Window New";
			//	a.ImageSmall = Properties.Resources.Window_16;
			//	a.ImageBig = Properties.Resources.Window_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("Resources", "New");
			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//	};
			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//	};
			//	EditorActions.Register( a );
			//}

			//Objects
			EditorActions.RegisterDockWindowAction( "Objects Window", ("Objects", ""), typeof( ObjectsWindow ) );

			////Objects New
			//{
			//	var a = new EditorAction();
			//	a.Name = "Objects Window New";
			//	a.ImageSmall = Properties.Resources.Window_16;
			//	a.ImageBig = Properties.Resources.Window_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("Objects", "New");
			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//	};
			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//	};
			//	EditorActions.Register( a );
			//}

			EditorActions.RegisterDockWindowAction( "Settings Window", ("Settings", ""), typeof( SettingsWindow ) );
			//EditorActions.RegisterDockWindowAction( "Solution Explorer", ("Solution", "Explorer"), typeof( SolutionExplorer ) );
			EditorActions.RegisterDockWindowAction( "Preview Window", ("Preview", ""), typeof( PreviewWindow ) );
			EditorActions.RegisterDockWindowAction( "Message Log Window", ("Message", "Log"), typeof( MessageLogWindow ) );
			EditorActions.RegisterDockWindowAction( "Output Window", ("Output", ""), typeof( OutputWindow ) );
			EditorActions.RegisterDockWindowAction( "Debug Info Window", ("Debug", "Info"), typeof( DebugInfoWindow ) );

			//EditorActions.RegisterDockWindowAction( "Tips Window", ("Tips", ""), typeof( TipsWindow ) );

			//Start Page
			{
				var a = new EditorAction();
				a.Name = "Start Page";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Opens the start page.";
				a.ImageSmall = Properties.Resources.Window_16;
				a.ImageBig = Properties.Resources.Window_32;
				a.QatSupport = true;
				a.RibbonText = ("Start", "Page");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
					context.Checked = EditorAPI.FindWindow<StartPageWindow>() != null;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					EditorAPI.OpenOrCloseStartPage();
				};
				EditorActions.Register( a );
			}

			//Reset
			{
				var a = new EditorAction();
				a.Name = "Reset Windows Settings";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.Refresh_16;
				a.ImageBig = Properties.Resources.Refresh_32;
				a.QatSupport = true;
				a.RibbonText = ("Reset", "Settings");
				a.Description = "Resets windows settings.";
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					if( EditorMessageBox.ShowQuestion( EditorLocalization.Translate( "General", "Reset windows settings and restart the editor?" ), MessageBoxButtons.YesNo ) == DialogResult.Yes )
						EditorAPI.BeginRestartApplication( true );
				};
				EditorActions.Register( a );
			}

			////Viewport No Split
			//{
			//	var a = new EditorAction();
			//	a.Name = "Viewport No Split";
			//	a.ImageSmall = Properties.Resources.ViewportNoSplit_16;
			//	a.ImageBig = Properties.Resources.ViewportNoSplit_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("No Split", "");
			//	a.Description = "Enables a viewport configuration with one viewport.";
			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//		//!!!!
			//		context.Enabled = true;
			//	};
			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//	};
			//	EditorActions.Register( a );
			//}

			////Viewport 2 Split
			//{
			//	var a = new EditorAction();
			//	a.Name = "Viewport 2 Split";
			//	a.ImageSmall = Properties.Resources.Viewport2Split_16;
			//	a.ImageBig = Properties.Resources.Viewport2Split_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("2 Split", "");
			//	a.Description = "Enables a viewport configuration with 2 viewports.";
			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//	};
			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//	};
			//	EditorActions.Register( a );
			//}

			////Viewport 3 Split
			//{
			//	var a = new EditorAction();
			//	a.Name = "Viewport 3 Split";
			//	a.ImageSmall = Properties.Resources.Viewport3Split_16;
			//	a.ImageBig = Properties.Resources.Viewport3Split_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("3 Split", "");
			//	a.Description = "Enables a viewport configuration with 3 viewports.";
			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//	};
			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//	};
			//	EditorActions.Register( a );
			//}

			////Viewport 4 Split
			//{
			//	var a = new EditorAction();
			//	a.Name = "Viewport 4 Split";
			//	a.ImageSmall = Properties.Resources.Viewport4Split_16;
			//	a.ImageBig = Properties.Resources.Viewport4Split_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("4 Split", "");
			//	a.Description = "Enables a viewport configuration with 4 viewports.";
			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//	};
			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//	};
			//	EditorActions.Register( a );
			//}

			////Add Viewport
			//{
			//	var a = new EditorAction();
			//	a.Name = "Add Viewport";
			//	a.ImageSmall = Properties.Resources.AddViewport_16;
			//	a.ImageBig = Properties.Resources.AddViewport_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("Add", "Viewport");
			//	a.Description = "Addes an additional viewport to the document.";
			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//	};
			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//	};
			//	EditorActions.Register( a );
			//}

			//Find Resource
			{
				var a = new EditorAction();
				a.Name = "Find Resource";
				a.ImageSmall = Properties.Resources.Focus_16;
				a.ImageBig = Properties.Resources.Focus_32;
				a.QatSupport = true;
				a.RibbonText = ("Find", "Resource");
				a.Description = "Finds a selected resource in the Resources Window.";
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
				};
				EditorActions.Register( a );
			}
		}

		/////////////////////////////////////////

		static void RegisterScene()
		{
			//Scene Display Development Data In Editor
			{
				var a = new EditorAction();
				a.Name = "Scene Display Development Data In Editor";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Development", "Data");
				a.ContextMenuText = "Display In Editor";// a.Name.Replace( "Scene ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					//!!!!сцена всегда корневая? везде так

					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = true;
						context.Checked = scene.DisplayDevelopmentDataInEditor;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplayDevelopmentDataInEditor;

					scene.DisplayDevelopmentDataInEditor = !scene.DisplayDevelopmentDataInEditor;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayDevelopmentDataInEditor" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Development Data In Simulation
			{
				var a = new EditorAction();
				a.Name = "Scene Display Development Data In Simulation";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Development", "Data");
				a.ContextMenuText = "Display In Simulation";// a.Name.Replace( "Scene ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					//!!!!сцена всегда корневая? везде так

					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = true;
						context.Checked = scene.DisplayDevelopmentDataInSimulation;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplayDevelopmentDataInSimulation;

					scene.DisplayDevelopmentDataInSimulation = !scene.DisplayDevelopmentDataInSimulation;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayDevelopmentDataInSimulation" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Text Info
			{
				var a = new EditorAction();
				a.Name = "Scene Display Text Info";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Text", "Info");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplayTextInfo;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplayTextInfo;

					scene.DisplayTextInfo = !scene.DisplayTextInfo;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayTextInfo" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Labels
			{
				var a = new EditorAction();
				a.Name = "Scene Display Labels";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Labels", "");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplayLabels;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplayLabels;

					scene.DisplayLabels = !scene.DisplayLabels;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayLabels" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Lights
			{
				var a = new EditorAction();
				a.Name = "Scene Display Lights";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Lights", "");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplayLights;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplayLights;

					scene.DisplayLights = !scene.DisplayLights;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayLights" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Decals
			{
				var a = new EditorAction();
				a.Name = "Scene Display Decals";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Decals", "");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplayDecals;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplayDecals;

					scene.DisplayDecals = !scene.DisplayDecals;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayDecals" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Reflection Probes
			{
				var a = new EditorAction();
				a.Name = "Scene Display Reflection Probes";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Reflection", "Probes");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplayReflectionProbes;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplayReflectionProbes;

					scene.DisplayReflectionProbes = !scene.DisplayReflectionProbes;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayReflectionProbes" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Cameras
			{
				var a = new EditorAction();
				a.Name = "Scene Display Cameras";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Cameras", "");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplayCameras;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplayCameras;

					scene.DisplayCameras = !scene.DisplayCameras;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayCameras" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Physical Objects
			{
				var a = new EditorAction();
				a.Name = "Scene Display Physical Objects";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Physical", "Objects");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplayPhysicalObjects;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplayPhysicalObjects;

					scene.DisplayPhysicalObjects = !scene.DisplayPhysicalObjects;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayPhysicalObjects" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Areas
			{
				var a = new EditorAction();
				a.Name = "Scene Display Areas";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Areas", "");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplayAreas;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplayAreas;

					scene.DisplayAreas = !scene.DisplayAreas;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayAreas" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Sensors
			{
				var a = new EditorAction();
				a.Name = "Scene Display Sensors";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Sensors", "");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplaySensors;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplaySensors;

					scene.DisplaySensors = !scene.DisplaySensors;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplaySensors" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Sound Sources
			{
				var a = new EditorAction();
				a.Name = "Scene Display Sound Sources";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Sound", "Sources");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplaySoundSources;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplaySoundSources;

					scene.DisplaySoundSources = !scene.DisplaySoundSources;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplaySoundSources" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Object In Space Bounds
			{
				var a = new EditorAction();
				a.Name = "Scene Display Object In Space Bounds";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Object", "Bounds");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplayObjectInSpaceBounds;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplayObjectInSpaceBounds;

					scene.DisplayObjectInSpaceBounds = !scene.DisplayObjectInSpaceBounds;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayObjectInSpaceBounds" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Display Scene Octree
			{
				var a = new EditorAction();
				a.Name = "Scene Display Scene Octree";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Scene", "Octree");
				a.ContextMenuText = a.Name.Replace( "Scene Display ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.DisplaySceneOctree;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.DisplaySceneOctree;

					scene.DisplaySceneOctree = !scene.DisplaySceneOctree;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplaySceneOctree" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Scene Frustum Culling Test
			{
				var a = new EditorAction();
				a.Name = "Scene Frustum Culling Test";
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Frustum", "Test");
				a.ContextMenuText = a.Name.Replace( "Scene ", "" );
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component_Scene;
					if( scene != null )
					{
						context.Enabled = scene.DisplayDevelopmentDataInEditor || scene.DisplayDevelopmentDataInSimulation;
						context.Checked = context.Enabled && scene.FrustumCullingTest;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = context.ObjectsInFocus.DocumentWindow.Document.ResultComponent as Component_Scene;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = scene.FrustumCullingTest;

					scene.FrustumCullingTest = !scene.FrustumCullingTest;

					var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:FrustumCullingTest" );
					var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			////Scene Display Development Data In Simulation
			//{
			//	var a = new EditorAction();
			//	a.name = "Scene Display Development Data In Simulation";
			//	a.imageSmall = Properties.Resources.Default_16;
			//	a.imageBig = Properties.Resources.Default_32;
			//	a.qatSupport = true;
			//	a.ribbonText = ("In Simulation", "");
			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//		xx xx;

			//		//!!!!сцена всегда корневая? везде так

			//		var scene = documentWindow?.Document.ResultComponent as Component_Scene;
			//		if( scene != null )
			//		{
			//			context.Enabled = true;
			//			context.Checked = scene.DisplayDevelopmentData;
			//		}
			//	};
			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//		xx xx;

			//		var scene = documentWindow.Document.ResultComponent as Component_Scene;
			//		var document = documentWindow.Document;

			//		var oldValue = scene.DisplayDevelopmentData;

			//		scene.DisplayDevelopmentData = !scene.DisplayDevelopmentData;

			//		var property = (Metadata.Property)scene.MetadataGetMemberBySignature( "property:DisplayDevelopmentData" );
			//		var undoItem = new UndoActionPropertiesChange.Item( scene, property, oldValue, new object[ 0 ] );
			//		var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
			//		document.UndoSystem.CommitAction( undoAction );
			//		document.Modified = true;
			//	};
			//	EditorActions.Register( a );
			//}

			//Snap All Axes
			{
				var a = new EditorAction();
				a.Name = "Snap All Axes";
				a.Description = "Aligns the position of the selected objects by all axes.";
				a.ImageSmall = Properties.Resources.MoveSnap_16;
				a.ImageBig = Properties.Resources.MoveSnap_32;
				a.QatSupport = true;
				a.RibbonText = ("Snap", "All");
				EditorActions.Register( a );
			}

			//Snap X
			{
				var a = new EditorAction();
				a.Name = "Snap X";
				a.Description = "Aligns the position of the selected objects by X axis.";
				a.ImageSmall = Properties.Resources.MoveSnapX_16;
				a.ImageBig = Properties.Resources.MoveSnapX_32;
				a.QatSupport = true;
				a.RibbonText = ("Snap X", "");
				EditorActions.Register( a );
			}

			//Snap Y
			{
				var a = new EditorAction();
				a.Name = "Snap Y";
				a.Description = "Aligns the position of the selected objects by Y axis.";
				a.ImageSmall = Properties.Resources.MoveSnapY_16;
				a.ImageBig = Properties.Resources.MoveSnapY_32;
				a.QatSupport = true;
				a.RibbonText = ("Snap Y", "");
				EditorActions.Register( a );
			}

			//Snap Z
			{
				var a = new EditorAction();
				a.Name = "Snap Z";
				a.Description = "Aligns the position of the selected objects by Z axis.";
				a.ImageSmall = Properties.Resources.MoveSnapZ_16;
				a.ImageBig = Properties.Resources.MoveSnapZ_32;
				a.QatSupport = true;
				a.RibbonText = ("Snap Z", "");
				EditorActions.Register( a );
			}

			//Focus Camera On Selected Object
			{
				var a = new EditorAction();
				a.Name = "Focus Camera On Selected Object";
				a.Description = "Focuses the camera on the selected object.";
				a.ImageSmall = Properties.Resources.Focus_16;
				a.ImageBig = Properties.Resources.Focus_32;
				a.QatSupport = true;
				a.RibbonText = ("Focus", "Camera");
				a.ShortcutKeys = new Keys[] { Keys.F };
				EditorActions.Register( a );
			}

			//Creation Of Objects By Drag & Drop
			{
				var a = new EditorAction();
				a.Name = "Create Objects By Drag & Drop";
				a.Description = "The mode of creation objects by Drag & Drop.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.CreationObjectsDrop_32;
				a.QatSupport = true;
				a.RibbonText = ("Drop", "");
				//!!!!?
				//a.ShortcutKeys = new Keys[] { Keys.F };
				EditorActions.Register( a );
			}

			//Creation Of Objects By Click
			{
				var a = new EditorAction();
				a.Name = "Create Objects By Click";
				a.Description = "The mode of creation objects by mouse click.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.CreationObjectsClick_32;
				a.QatSupport = true;
				a.RibbonText = ("Click", "");
				//!!!!?
				//a.ShortcutKeys = new Keys[] { Keys.F };
				EditorActions.Register( a );
			}

			//Creation Of Objects By Brush
			{
				var a = new EditorAction();
				a.Name = "Create Objects By Brush";
				a.Description = "The mode of creation objects by brush.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.CreationObjectsBrush_32;
				a.QatSupport = true;
				a.RibbonText = ("Brush", "");
				//!!!!?
				//a.ShortcutKeys = new Keys[] { Keys.F };
				EditorActions.Register( a );
			}

			//Creating Objects Brush Radius
			{
				var a = new EditorAction();
				a.Name = "Create Objects Brush Radius";
				a.ActionType = EditorAction.ActionTypeEnum.Slider;
				a.Slider.Minimum = 0;// 0.1;
				a.Slider.Maximum = 100;
				a.Slider.ExponentialPower = 3;
				a.Slider.Value = Component_Scene_DocumentWindow.CreateObjectsBrushRadius;
				//!!!!
				//a.Description = "Focus the camera on the selected object.";
				a.RibbonText = ("Radius", "");
				EditorActions.Register( a );
			}

			//Creating Objects Brush Strength
			{
				var a = new EditorAction();
				a.Name = "Create Objects Brush Strength";
				a.ActionType = EditorAction.ActionTypeEnum.Slider;
				a.Slider.Value = Component_Scene_DocumentWindow.CreateObjectsBrushStrength;
				//!!!!
				//a.Description = "Focus the camera on the selected object.";
				a.RibbonText = ("Strength", "");
				EditorActions.Register( a );
			}

			//Creating Objects Brush Hardness
			{
				var a = new EditorAction();
				a.Name = "Create Objects Brush Hardness";
				a.ActionType = EditorAction.ActionTypeEnum.Slider;
				a.Slider.Value = Component_Scene_DocumentWindow.CreateObjectsBrushHardness;
				//!!!!
				//a.Description = "Focus the camera on the selected object.";
				a.RibbonText = ("Hardness", "");
				EditorActions.Register( a );
			}

			//Creating Objects Destination
			{
				var a = new EditorAction();
				a.Name = "Create Objects Destination";
				a.ActionType = EditorAction.ActionTypeEnum.ListBox;
				//a.ListBox.Items.Add( "Root" );
				//!!!!
				//a.Description = "Focus the camera on the selected object.";
				a.RibbonText = ("Destination", "");
				EditorActions.Register( a );
			}








			//!!!!тут? это не только про сцену
			//New Object
			{
				var a = new EditorAction();
				a.Name = "New Object";
				a.Description = "Add new object to the selected object.";
				a.ImageSmall = Properties.Resources.New_16;
				a.ImageBig = Properties.Resources.New_32;
				a.QatSupport = true;
				//a.QatAddByDefault = true;
				a.RibbonText = ("New", "Object");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var componentOfDocument = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component;
					if( componentOfDocument != null )
					{
						var parents = new List<Component>();
						if( context.ObjectsInFocus.DocumentWindow.SelectedObjects.Length != 0 )
							parents.AddRange( context.ObjectsInFocus.DocumentWindow.SelectedObjects.OfType<Component>() );
						else
							parents.Add( componentOfDocument );

						if( parents.Count != 0 )
							context.Enabled = true;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var componentOfDocument = context.ObjectsInFocus.DocumentWindow?.Document.ResultComponent as Component;
					if( componentOfDocument != null )
					{
						var parents = new List<Component>();
						if( context.ObjectsInFocus.DocumentWindow.SelectedObjects.Length != 0 )
							parents.AddRange( context.ObjectsInFocus.DocumentWindow.SelectedObjects.OfType<Component>() );
						else
							parents.Add( componentOfDocument );

						if( parents.Count != 0 )
						{
							var initData = new NewObjectWindow.CreationDataClass();
							initData.initDocumentWindow = context.ObjectsInFocus.DocumentWindow;
							initData.initParentObjects = new List<object>();
							initData.initParentObjects.AddRange( parents );

							EditorAPI.OpenNewObjectWindow( initData );
						}
					}
				};
				EditorActions.Register( a );
			}


			////Box geometry
			//{
			//	var a = new EditorAction_NewComponent( typeof( Component_MeshGeometry_Box ) );
			//	a.Name = "Box";
			//	EditorActions.Register( a );
			//}

			////Sphere geometry
			//{
			//	var a = new EditorAction_NewComponent( typeof( Component_MeshGeometry_Sphere ) );
			//	a.Name = "Sphere";
			//	EditorActions.Register( a );
			//}

			////Cylinder geometry
			//{
			//	var a = new EditorAction_NewComponent( typeof( Component_MeshGeometry_Cylinder ) );
			//	a.Name = "Cylinder";
			//	EditorActions.Register( a );
			//}

			////Cone geometry
			//{
			//	var a = new EditorAction_NewComponent( typeof( Component_MeshGeometry_Cone ) );
			//	a.Name = "Cone";
			//	EditorActions.Register( a );
			//}

			////Capsule geometry
			//{
			//	var a = new EditorAction_NewComponent( typeof( Component_MeshGeometry_Capsule ) );
			//	a.Name = "Capsule";
			//	EditorActions.Register( a );
			//}

			////Light
			//{
			//	var a = new EditorAction_NewComponent( typeof( Component_Light ) );
			//	a.Name = "Light";
			//	a.Description = "Add new light.";
			//	EditorActions.Register( a );
			//}

			////Skybox
			//{
			//	var a = new EditorAction_NewComponent( "NeoAxis.Component_Skybox" );
			//	a.Name = "Skybox";
			//	EditorActions.Register( a );
			//}

			////Fog
			//{
			//	var a = new EditorAction_NewComponent( "NeoAxis.Component_Fog" );
			//	a.Name = "Fog";
			//	EditorActions.Register( a );
			//}

			////Character
			//{
			//	var a = new EditorAction_NewComponent( "NeoAxis.Component_Character" );
			//	a.Name = "Character";
			//	EditorActions.Register( a );
			//}

			////Camera
			//{
			//	var a = new EditorAction_NewComponent( typeof( Component_Camera ) );
			//	a.Name = "Camera";
			//	EditorActions.Register( a );
			//}

			////Sensor
			//{
			//	var a = new EditorAction_NewComponent( typeof( Component_Sensor ) );
			//	a.Name = "Sensor";
			//	EditorActions.Register( a );
			//}

			////ParticleSystem
			//{
			//	var a = new EditorAction_NewComponent( typeof( Component_ParticleSystem ) );
			//	a.Name = "Particle System";
			//	EditorActions.Register( a );
			//}

			////Material
			//{
			//	var a = new EditorAction_NewComponent( typeof( Component_Material ) );
			//	a.Name = "Material";
			//	a.Description = "Add new material.";
			//	EditorActions.Register( a );
			//}
		}

		/////////////////////////////////////////

		static void RegisterTerrain()
		{
			//Terrain Geometry Raise
			{
				var a = new EditorAction();
				a.Name = "Terrain Geometry Raise";
				a.Description = "The mode of increasing the height of the terrain. Holding down the Shift key will decrease the height.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.TerrainRaise_32;
				a.QatSupport = true;
				a.RibbonText = ("Raise", "");
				EditorActions.Register( a );
			}

			//Terrain Geometry Lower
			{
				var a = new EditorAction();
				a.Name = "Terrain Geometry Lower";
				a.Description = "The mode of decreasing the height of the terrain. Holding down the Shift key will increase the height.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.TerrainLower_32;
				a.QatSupport = true;
				a.RibbonText = ("Lower", "");
				EditorActions.Register( a );
			}

			//Terrain Geometry Smooth
			{
				var a = new EditorAction();
				a.Name = "Terrain Geometry Smooth";
				a.Description = "The mode of smoothing the height of the terrain.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.TerrainSmooth_32;
				a.QatSupport = true;
				a.RibbonText = ("Smooth", "");
				EditorActions.Register( a );
			}

			//Terrain Geometry Flatten
			{
				var a = new EditorAction();
				a.Name = "Terrain Geometry Flatten";
				a.Description = "The mode of flattening the height of the terrain.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.TerrainFlatten_32;
				a.QatSupport = true;
				a.RibbonText = ("Flatten", "");
				EditorActions.Register( a );
			}

			////Terrain Hole Add
			//{
			//	var a = new EditorAction();
			//	a.Name = "Terrain Hole Add";
			//	//!!!!
			//	//a.Description = "Focus the camera on the selected object.";
			//	a.ImageSmall = Properties.Resources.Default_16;
			//	a.ImageBig = Properties.Resources.Default_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("Add", "");
			//	EditorActions.Register( a );
			//}

			////Terrain Hole Delete
			//{
			//	var a = new EditorAction();
			//	a.Name = "Terrain Hole Delete";
			//	//!!!!
			//	//a.Description = "Focus the camera on the selected object.";
			//	a.ImageSmall = Properties.Resources.Default_16;
			//	a.ImageBig = Properties.Resources.Default_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("Delete", "");
			//	EditorActions.Register( a );
			//}

			//Terrain Shape Circle
			{
				var a = new EditorAction();
				a.Name = "Terrain Shape Circle";
				a.Description = "The circle shape mode of the editing tool.";
				a.ImageSmall = Properties.Resources.Circle_16;
				a.ImageBig = Properties.Resources.Circle_32;
				a.QatSupport = true;
				a.RibbonText = ("Circle", "");
				EditorActions.Register( a );
			}

			//Terrain Shape Square
			{
				var a = new EditorAction();
				a.Name = "Terrain Shape Square";
				a.Description = "The square shape mode of the editing tool.";
				a.ImageSmall = Properties.Resources.Square_16;
				a.ImageBig = Properties.Resources.Square_32;
				a.QatSupport = true;
				a.RibbonText = ("Square", "");
				EditorActions.Register( a );
			}

			//Terrain Tool Radius
			{
				var a = new EditorAction();
				a.Name = "Terrain Tool Radius";
				a.ActionType = EditorAction.ActionTypeEnum.Slider;
				a.Slider.Minimum = 0;// 0.1;
				a.Slider.Maximum = 100;
				a.Slider.ExponentialPower = 3;
				a.Slider.Value = Component_Scene_DocumentWindow.TerrainToolRadius;
				a.Description = "Tool size.";
				a.RibbonText = ("Radius", "");
				EditorActions.Register( a );
			}

			//Terrain Tool Strength
			{
				var a = new EditorAction();
				a.Name = "Terrain Tool Strength";
				a.ActionType = EditorAction.ActionTypeEnum.Slider;
				a.Slider.Value = Component_Scene_DocumentWindow.TerrainToolStrength;
				a.Description = "The strength of impact of the editing tool.";
				a.RibbonText = ("Strength", "");
				EditorActions.Register( a );
			}

			//Terrain Tool Hardness
			{
				var a = new EditorAction();
				a.Name = "Terrain Tool Hardness";
				a.ActionType = EditorAction.ActionTypeEnum.Slider;
				a.Slider.Value = Component_Scene_DocumentWindow.TerrainToolHardness;
				a.Description = "The hardness of the editing tool. Determines the strength of impact depending on the distance to the center of the tool.";
				a.RibbonText = ("Hardness", "");
				EditorActions.Register( a );
			}

			//Terrain Paint Paint
			{
				var a = new EditorAction();
				a.Name = "Terrain Paint Paint";
				a.Description = "The mode of paint a layer. Holding down the Shift key will erasing a layer.";
				a.ImageSmall = Properties.Resources.Paint_16;
				a.ImageBig = Properties.Resources.Paint_32;
				a.QatSupport = true;
				a.RibbonText = ("Paint", "");
				EditorActions.Register( a );
			}

			//Terrain Paint Clear
			{
				var a = new EditorAction();
				a.Name = "Terrain Paint Clear";
				a.Description = "The mode of erasing a layer. Holding down the Shift key will paint a layer.";
				a.ImageSmall = Properties.Resources.Eraser_16;
				a.ImageBig = Properties.Resources.Eraser_32;
				a.QatSupport = true;
				a.RibbonText = ("Clear", "");
				EditorActions.Register( a );
			}

			//Terrain Paint Smooth
			{
				var a = new EditorAction();
				a.Name = "Terrain Paint Smooth";
				a.Description = "The mode of smoothing a layer.";
				a.ImageSmall = Properties.Resources.PaintSmooth_16;
				a.ImageBig = Properties.Resources.PaintSmooth_32;
				a.QatSupport = true;
				a.RibbonText = ("Smooth", "");
				EditorActions.Register( a );
			}

			//Terrain Paint Flatten
			{
				var a = new EditorAction();
				a.Name = "Terrain Paint Flatten";
				a.Description = "The mode of flattening a layer.";
				a.ImageSmall = Properties.Resources.PaintFlatten_16;
				a.ImageBig = Properties.Resources.PaintFlatten_32;
				a.QatSupport = true;
				a.RibbonText = ("Flatten", "");
				EditorActions.Register( a );
			}

			//Terrain Paint Layers
			{
				var a = new EditorAction();
				a.Name = "Terrain Paint Layers";
				a.ActionType = EditorAction.ActionTypeEnum.ListBox;
				a.ListBox.Length = 400;
				a.ListBox.Mode = EditorAction.ListBoxSettings.ModeEnum.Tiles;
				a.Description = "The list of layers to paint.";
				a.RibbonText = ("Layers", "");
				EditorActions.Register( a );
			}

		}

		/////////////////////////////////////////

		static void RegisterMesh()
		{
			//Mesh Display Pivot
			{
				var a = new EditorAction();
				a.Name = "Mesh Display Pivot";
				a.Description = "Whether to display the pivot.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshPivot_32;
				a.QatSupport = true;
				a.RibbonText = ("Pivot", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
					{
						context.Enabled = true;
						context.Checked = mesh.EditorDisplayPivot;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as Component_Mesh;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = mesh.EditorDisplayPivot;

					mesh.EditorDisplayPivot = !mesh.EditorDisplayPivot;

					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayPivot" );
					var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Mesh Display Bounds
			{
				var a = new EditorAction();
				a.Name = "Mesh Display Bounds";
				a.Description = "Whether to display the bounding box.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshBounds_32;//Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Bounds", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
					{
						context.Enabled = true;
						context.Checked = mesh.EditorDisplayBounds;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as Component_Mesh;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = mesh.EditorDisplayBounds;

					mesh.EditorDisplayBounds = !mesh.EditorDisplayBounds;

					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayBounds" );
					var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Mesh Display Triangles
			{
				var a = new EditorAction();
				a.Name = "Mesh Display Triangles";
				a.Description = "Whether to display the triangles.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshTriangles_32;
				a.QatSupport = true;
				a.RibbonText = ("Triangles", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
					{
						context.Enabled = true;
						context.Checked = mesh.EditorDisplayTriangles;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as Component_Mesh;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = mesh.EditorDisplayTriangles;

					mesh.EditorDisplayTriangles = !mesh.EditorDisplayTriangles;

					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayTriangles" );
					var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Mesh Display Vertices
			{
				var a = new EditorAction();
				a.Name = "Mesh Display Vertices";
				a.Description = "Whether to display the vertices.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshVertices_32;// Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Vertices", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
					{
						context.Enabled = true;
						context.Checked = mesh.EditorDisplayVertices;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as Component_Mesh;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = mesh.EditorDisplayVertices;

					mesh.EditorDisplayVertices = !mesh.EditorDisplayVertices;

					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayVertices" );
					var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Mesh Display Normals
			{
				var a = new EditorAction();
				a.Name = "Mesh Display Normals";
				a.Description = "Whether to display the normals.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshNormals_32;
				a.QatSupport = true;
				a.RibbonText = ("Normals", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null && mesh.Result != null && ( mesh.Result.ExtractedVerticesComponents & StandardVertex.Components.Normal ) != 0 )
					{
						context.Enabled = true;
						context.Checked = mesh.EditorDisplayNormals;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as Component_Mesh;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = mesh.EditorDisplayNormals;

					mesh.EditorDisplayNormals = !mesh.EditorDisplayNormals;

					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayNormals" );
					var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Mesh Display Tangents
			{
				var a = new EditorAction();
				a.Name = "Mesh Display Tangents";
				a.Description = "Whether to display the tangent vectors.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshTangents_32;
				a.QatSupport = true;
				a.RibbonText = ("Tangents", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null && mesh.Result != null && ( mesh.Result.ExtractedVerticesComponents & StandardVertex.Components.Tangent ) != 0 )
					{
						context.Enabled = true;
						context.Checked = mesh.EditorDisplayTangents;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as Component_Mesh;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = mesh.EditorDisplayTangents;

					mesh.EditorDisplayTangents = !mesh.EditorDisplayTangents;

					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayTangents" );
					var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Mesh Display Binormals
			{
				var a = new EditorAction();
				a.Name = "Mesh Display Binormals";
				a.Description = "Whether to display the binormal vectors.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshBinormals_32;
				a.QatSupport = true;
				a.RibbonText = ("Binormals", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null && mesh.Result != null && ( mesh.Result.ExtractedVerticesComponents & StandardVertex.Components.Normal ) != 0 && ( mesh.Result.ExtractedVerticesComponents & StandardVertex.Components.Tangent ) != 0 )
					{
						context.Enabled = true;
						context.Checked = mesh.EditorDisplayBinormals;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as Component_Mesh;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = mesh.EditorDisplayBinormals;

					mesh.EditorDisplayBinormals = !mesh.EditorDisplayBinormals;

					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayBinormals" );
					var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Mesh Display Vertex Color
			{
				var a = new EditorAction();
				a.Name = "Mesh Display Vertex Color";
				a.Description = "Whether to display the vertex color.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshVertexColor_32;
				a.QatSupport = true;
				a.RibbonText = ("Vertex", "Color");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null && mesh.Result != null && ( mesh.Result.ExtractedVerticesComponents & StandardVertex.Components.Color ) != 0 )
					{
						context.Enabled = true;
						context.Checked = mesh.EditorDisplayVertexColor;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as Component_Mesh;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = mesh.EditorDisplayVertexColor;

					mesh.EditorDisplayVertexColor = !mesh.EditorDisplayVertexColor;

					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayVertexColor" );
					var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Mesh Display UV
			{
				var a = new EditorAction();
				a.Name = "Mesh Display UV";
				a.Description = "Specifies a texture coordinates channel to display.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshUV_32;
				a.QatSupport = true;
				a.RibbonText = ("UV", "");
				a.ActionType = EditorAction.ActionTypeEnum.DropDown;
				a.DropDownContextMenu = new KryptonContextMenu();

				var items = new List<KryptonContextMenuItemBase>();

				EventHandler clickHandler = delegate ( object s, EventArgs e2 )
				{
					var item = (KryptonContextMenuItem)s;

					var documentWindow = EditorAPI.SelectedDocumentWindow;
					var mesh = documentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
					{
						var newValue = (int)item.Tag;
						var oldValue = mesh.EditorDisplayUV;
						if( newValue != oldValue )
						{
							mesh.EditorDisplayUV = newValue;

							var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayUV" );
							var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
							var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
							documentWindow.Document.UndoSystem.CommitAction( undoAction );
							documentWindow.Document.Modified = true;
						}
					}
				};

				//None
				{
					var item = new KryptonContextMenuItem( "None", null, clickHandler );
					item.Tag = -1;
					items.Add( item );
				}

				items.Add( new KryptonContextMenuSeparator() );

				//UV Channels
				for( int n = 0; n < 4; n++ )
				{
					var item = new KryptonContextMenuItem( "UV Channel " + n.ToString(), null, clickHandler );
					item.Tag = n;
					items.Add( item );
				}

				a.DropDownContextMenu.Items.Add( new KryptonContextMenuItems( items.ToArray() ) );

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null && mesh.Result != null )
					{
						var components = mesh.Result.ExtractedVerticesComponents;
						if( ( components & StandardVertex.Components.TexCoord0 ) != 0 || ( components & StandardVertex.Components.TexCoord1 ) != 0 ||
							( components & StandardVertex.Components.TexCoord2 ) != 0 || ( components & StandardVertex.Components.TexCoord3 ) != 0 )
						{
							context.Enabled = true;

							var menuItems = ( (KryptonContextMenuItems)context.Action.DropDownContextMenu.Items[ 0 ] ).Items;
							foreach( var item in menuItems )
							{
								if( item.Tag != null )
								{
									var item2 = (KryptonContextMenuItem)item;

									int index = (int)item.Tag;
									switch( index )
									{
									case 0: item2.Visible = ( StandardVertex.Components.TexCoord0 & components ) != 0; break;
									case 1: item2.Visible = ( StandardVertex.Components.TexCoord1 & components ) != 0; break;
									case 2: item2.Visible = ( StandardVertex.Components.TexCoord2 & components ) != 0; break;
									case 3: item2.Visible = ( StandardVertex.Components.TexCoord3 & components ) != 0; break;
									}
									item2.Checked = mesh.EditorDisplayUV == index;
								}
							}

							//!!!!
							context.Checked = mesh.EditorDisplayUV != -1;
						}
					}
				};

				EditorActions.Register( a );
			}

			//Mesh Display LOD
			{
				var a = new EditorAction();
				a.Name = "Mesh Display LOD";
				a.Description = "Specifies a level of detail to display.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.LOD_32;
				a.QatSupport = true;
				a.RibbonText = ("LOD", "");
				a.ActionType = EditorAction.ActionTypeEnum.DropDown;
				a.DropDownContextMenu = new KryptonContextMenu();

				var items = new List<KryptonContextMenuItemBase>();

				EventHandler clickHandler = delegate ( object s, EventArgs e2 )
				{
					var item = (KryptonContextMenuItem)s;

					var documentWindow = EditorAPI.SelectedDocumentWindow;
					var mesh = documentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
					{
						var newValue = (int)item.Tag;
						var oldValue = mesh.EditorDisplayLOD;
						if( newValue != oldValue )
						{
							mesh.EditorDisplayLOD = newValue;

							var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayLOD" );
							var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
							var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
							documentWindow.Document.UndoSystem.CommitAction( undoAction );
							documentWindow.Document.Modified = true;
						}
					}
				};

				int lodMaxCount = 40;
				for( int n = 0; n < lodMaxCount; n++ )
				{
					var item = new KryptonContextMenuItem( $"LOD {n}", null, clickHandler );
					item.Tag = n;
					items.Add( item );
				}

				a.DropDownContextMenu.Items.Add( new KryptonContextMenuItems( items.ToArray() ) );

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					var lods = mesh?.Result?.MeshData?.LODs;
					if( lods != null )
					{
						context.Enabled = true;

						int lodCount = lods.Length + 1;

						var menuItems = ( (KryptonContextMenuItems)context.Action.DropDownContextMenu.Items[ 0 ] ).Items;
						foreach( var item in menuItems )
						{
							int tag = (int)item.Tag;
							item.Visible = tag < lodCount;

							var item2 = (KryptonContextMenuItem)item;
							item2.Checked = tag == mesh.EditorDisplayLOD;
						}

						context.Checked = mesh.EditorDisplayLOD != 0;
					}
				};

				EditorActions.Register( a );
			}

			//Mesh Display Collision
			{
				const string bodyName = "Collision Definition";

				var a = new EditorAction();
				a.Name = "Mesh Display Collision";
				a.Description = "Whether to display the collision.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshCollision_32;
				a.QatSupport = true;
				a.RibbonText = ("Collision", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null && mesh.GetComponent( bodyName ) as Component_RigidBody != null )
					{
						context.Enabled = true;
						context.Checked = mesh.EditorDisplayCollision;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as Component_Mesh;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = mesh.EditorDisplayCollision;

					mesh.EditorDisplayCollision = !mesh.EditorDisplayCollision;

					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayCollision" );
					var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Mesh Display Skeleton
			{
				var a = new EditorAction();
				a.Name = "Mesh Display Skeleton";
				a.Description = "Whether to display the skeleton.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshSkeleton_32;
				a.QatSupport = true;
				a.RibbonText = ("Skeleton", "");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null && mesh.Result != null && mesh.GetComponent<Component_Skeleton>() != null )
					{
						context.Enabled = true;
						context.Checked = mesh.EditorDisplaySkeleton;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as Component_Mesh;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = mesh.EditorDisplaySkeleton;

					mesh.EditorDisplaySkeleton = !mesh.EditorDisplaySkeleton;

					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplaySkeleton" );
					var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Mesh Play Animation
			{
				var a = new EditorAction();
				a.Name = "Mesh Play Animation";
				a.Description = "Specifies the animation to play.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MeshAnimation_32;
				a.QatSupport = true;
				a.RibbonText = ("Animation", "");
				a.ActionType = EditorAction.ActionTypeEnum.DropDown;
				a.DropDownContextMenu = new KryptonContextMenu();

				//menu can't be empty
				{
					var items = new List<KryptonContextMenuItemBase>();
					items.Add( new KryptonContextMenuSeparator() );
					a.DropDownContextMenu.Items.Add( new KryptonContextMenuItems( items.ToArray() ) );
				}

				a.DropDownContextMenu.Opening += delegate ( object sender, CancelEventArgs e )
				{
					var menu = (KryptonContextMenu)sender;

					menu.Items.Clear();

					var tuple = ((DocumentInstance, Component_Mesh))menu.Tag;
					var document = tuple.Item1;
					var mesh = tuple.Item2;
					var animations = mesh.GetComponents<Component_Animation>( checkChildren: true );

					var items = new List<KryptonContextMenuItemBase>();

					void Handler( object s, EventArgs e2 )
					{
						var menuItem = (KryptonContextMenuItem)s;

						var newValue = (string)menuItem.Tag;
						var oldValue = mesh.EditorPlayAnimation;
						if( newValue != oldValue )
						{
							mesh.EditorPlayAnimation = newValue;

							var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorPlayAnimation" );
							var undoItem = new UndoActionPropertiesChange.Item( mesh, property, oldValue, new object[ 0 ] );
							var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
							document.UndoSystem.CommitAction( undoAction );
							document.Modified = true;
						}
					}

					//None
					{
						var item = new KryptonContextMenuItem( "None", null, Handler );
						item.Tag = "";
						item.Checked = string.IsNullOrEmpty( mesh.EditorPlayAnimation );
						items.Add( item );
					}

					items.Add( new KryptonContextMenuSeparator() );

					//animations
					foreach( var animation in animations )
					{
						var item = new KryptonContextMenuItem( string.IsNullOrEmpty( animation.Name ) ? "'No name'" : animation.Name, null, Handler );
						item.Tag = animation.GetPathFromRoot();
						item.Checked = mesh.EditorPlayAnimation == animation.GetPathFromRoot();
						items.Add( item );
					}

					menu.Items.Add( new KryptonContextMenuItems( items.ToArray() ) );
				};

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null && mesh.Result != null )
					{
						var animations = mesh.GetComponents<Component_Animation>( checkChildren: true );
						var canPlay = animations.Length != 0;

						context.Enabled = canPlay;
						context.Checked = animations.Any( animation => animation.GetPathFromRoot() == mesh.EditorPlayAnimation );

						a.DropDownContextMenu.Tag = (context.ObjectsInFocus.DocumentWindow.Document, mesh);
					}
				};

				EditorActions.Register( a );
			}

			//Mesh Add Collision
			{
				const string bodyName = "Collision Definition";

				var a = new EditorAction();
				a.Name = "Mesh Add Collision";
				a.ImageSmall = Properties.Resources.Add_16;
				a.ImageBig = Properties.Resources.MeshCollision_32;// Add_32;
				a.QatSupport = true;
				a.RibbonText = ("Add", "Collision");
				a.ActionType = EditorAction.ActionTypeEnum.DropDown;
				a.Description = "Precalculates the collision of the mesh.";

				//!!!!
				//a.ContextMenuSupport = true;

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
						context.Enabled = mesh.GetComponent( bodyName ) as Component_RigidBody == null;
				};

				//context menu
				{
					a.DropDownContextMenu = new KryptonContextMenu();

					var items = new List<KryptonContextMenuItemBase>();

					EventHandler clickHandler = delegate ( object s, EventArgs e2 )
					{
						var item = (KryptonContextMenuItem)s;

						var documentWindow = EditorAPI.SelectedDocumentWindow;
						EditorPhysicsUtility.AddCollision( documentWindow, (string)item.Tag );
					};

					var names = new string[] { "Box", "Sphere", "Capsule", "Cylinder", "Convex", "Convex Decomposition", "Mesh" };
					foreach( var name in names )
					{
						var item = new KryptonContextMenuItem( name, null, clickHandler );
						item.Tag = name;
						items.Add( item );
					}

					a.DropDownContextMenu.Items.Add( new KryptonContextMenuItems( items.ToArray() ) );
				}

				EditorActions.Register( a );
			}

			//Mesh Delete Collision
			{
				const string bodyName = "Collision Definition";

				var a = new EditorAction();
				a.Name = "Mesh Delete Collision";
				a.ImageSmall = Properties.Resources.Delete_16;
				a.ImageBig = Properties.Resources.Delete_32;
				a.QatSupport = true;
				a.RibbonText = ("Delete", "Collision");
				a.Description = "Deletes the collision body of the mesh.";

				//!!!!
				//a.ContextMenuSupport = true;

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
						context.Enabled = mesh.GetComponent( bodyName ) as Component_RigidBody != null;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var text = string.Format( EditorLocalization.Translate( "General", "Delete \'{0}\'?" ), bodyName );
					if( EditorMessageBox.ShowQuestion( text, MessageBoxButtons.YesNo ) == DialogResult.Yes )
					{
						var mesh = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as Component_Mesh;
						var collision = mesh.GetComponent( bodyName ) as Component_RigidBody;
						if( collision != null )
						{
							var document = context.ObjectsInFocus.DocumentWindow.Document;
							var undoAction = new UndoActionComponentCreateDelete( document, new Component[] { collision }, false );
							document.UndoSystem.CommitAction( undoAction );
							document.Modified = true;
						}
					}
				};
				EditorActions.Register( a );
			}

			//Mesh Add Structure
			{
				var a = new EditorAction();
				a.Name = "Mesh Add Structure";
				a.ImageSmall = Properties.Resources.Add_16;
				a.ImageBig = Properties.Resources.MeshStructure_32;// Add_32;
				a.QatSupport = true;
				a.RibbonText = ("Add", "Structure");
				a.Description = "Generates the structure of the mesh.";

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
						context.Enabled = mesh.Structure == null;
					//context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
					{
						var document = context.ObjectsInFocus.DocumentWindow.Document;

						var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Component_Mesh.Structure ) );
						var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, mesh.Structure ) );

						mesh.RebuildStructure();

						document.CommitUndoAction( undoAction );
					}
				};
				EditorActions.Register( a );
			}

			//Mesh Delete Structure
			{
				var a = new EditorAction();
				a.Name = "Mesh Delete Structure";
				a.ImageSmall = Properties.Resources.Delete_16;
				a.ImageBig = Properties.Resources.Delete_32;
				a.QatSupport = true;
				a.RibbonText = ("Delete", "Structure");
				a.Description = "Deletes the structure of the mesh.";

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
						context.Enabled = mesh.Structure != null;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					if( EditorMessageBox.ShowQuestion( EditorLocalization.Translate( "General", "Delete structure?" ), MessageBoxButtons.YesNo ) == DialogResult.Yes )
					{
						var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
						if( mesh != null )
						{
							var document = context.ObjectsInFocus.DocumentWindow.Document;

							var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Component_Mesh.Structure ) );
							var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, mesh.Structure ) );

							mesh.Structure = null;

							document.CommitUndoAction( undoAction );
						}
					}
				};
				EditorActions.Register( a );
			}

			foreach( var type in GetAllMeshModifiers() )
				RegisterNewMeshModifier( type );

			//Mesh Add Paint Layer
			{
				var a = new EditorAction();
				a.Name = "Mesh Add Paint Layer";
				a.ImageSmall = Properties.Resources.Layers_16;
				a.ImageBig = Properties.Resources.Layers_32;
				a.QatSupport = true;
				a.RibbonText = ("Add", "Layer");
				a.Description = "Adds a masked paint layer.";
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
						context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
					if( mesh != null )
					{
						var documentWindow = context.ObjectsInFocus.DocumentWindow;

						var modifier = mesh.CreateComponent<Component_PaintLayer>( enabled: false );
						modifier.Name = GetUniqueFriendlyName( modifier );
						modifier.Enabled = true;

						documentWindow.Focus();

						//undo
						var newObjects = new Component[] { modifier };
						var document = documentWindow.Document;
						var action = new UndoActionComponentCreateDelete( document, newObjects, true );
						document.CommitUndoAction( action );
						documentWindow.SelectObjects( newObjects.ToArray() );
					}
				};
				EditorActions.Register( a );
			}

			AssemblyUtility.RegisterAssemblyEvent += AssemblyUtility_RegisterAssemblyEvent;
		}

		private static void AssemblyUtility_RegisterAssemblyEvent( Assembly assembly )
		{
			//add new mesh modifiers
			foreach( var netType in assembly.GetTypes() )
			{
				try
				{
					if( typeof( Component_MeshModifier ).IsAssignableFrom( netType ) && !netType.IsAbstract )
					{
						var type = MetadataManager.GetTypeOfNetType( netType );
						RegisterNewMeshModifier( type );
					}
				}
				catch { }
			}
		}

		/////////////////////////////////////////

		static void RegisterScripting()
		{
			//Add C# files to Project.csproj
			{
				var a = new EditorAction();
				a.Name = "Add C# files to Project.csproj";
				a.Description = "Adds C# files to the Project.csproj.";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.Add_16;
				a.ImageBig = Properties.Resources.Add_32;
				a.QatSupport = true;
				a.ContextMenuSupport = EditorContextMenu.MenuTypeEnum.Resources;
				a.RibbonText = ("Add", "");

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
									var fullPaths = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true );
									var files = Directory.GetFiles( fileItem.FullPath, "*.cs", SearchOption.AllDirectories );
									var existNotAdded = files.Any( f => !fullPaths.Contains( f ) );
									if( !existNotAdded )
										skip = true;
								}

								if( !skip )
								{
									context.Enabled = true;
									break;
								}
							}

							if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".cs" )
							{
								bool added = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fileItem.FullPath );
								if( !added )
								{
									context.Enabled = true;
									break;
								}
							}
						}
					}
				};

				a.Click += delegate ( EditorAction.ClickContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
					{
						var toAdd = new ESet<string>();

						var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
						foreach( var fileItem in fileItems )
						{
							if( fileItem.IsDirectory )
							{
								var fullPaths = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true );
								var files = Directory.GetFiles( fileItem.FullPath, "*.cs", SearchOption.AllDirectories );
								foreach( var file in files )
								{
									if( !fullPaths.Contains( file ) )
										toAdd.AddWithCheckAlreadyContained( file );
								}
							}

							if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".cs" )
							{
								bool added = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fileItem.FullPath );
								if( !added )
									toAdd.Add( fileItem.FullPath );
							}
						}

						if( toAdd.Count != 0 )
						{
							if( CSharpProjectFileUtility.UpdateProjectFile( toAdd, null, out var error ) )
							{
								if( toAdd.Count > 1 )
									Log.Info( EditorLocalization.Translate( "General", "Items have been added to the Project.csproj." ) );
								else
									Log.Info( EditorLocalization.Translate( "General", "The item has been added to the Project.csproj." ) );
							}
							else
								Log.Warning( error );
						}
					}
				};

				EditorActions.Register( a );
			}

			//Remove C# files from Project.csproj
			{
				var a = new EditorAction();
				a.Name = "Remove C# files from Project.csproj";
				a.Description = "Removes C# files from the Project.csproj.";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.Delete_16;
				a.ImageBig = Properties.Resources.Delete_32;
				a.QatSupport = true;
				a.ContextMenuSupport = EditorContextMenu.MenuTypeEnum.Resources;
				a.RibbonText = ("Remove", "");

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
									var fullPaths = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true );
									var files = Directory.GetFiles( fileItem.FullPath, "*.cs", SearchOption.AllDirectories );
									var existAdded = files.Any( f => fullPaths.Contains( f ) );
									if( !existAdded )
										skip = true;
								}

								if( !skip )
								{
									context.Enabled = true;
									break;
								}
							}

							if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".cs" )
							{
								bool added = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fileItem.FullPath );
								if( added )
								{
									context.Enabled = true;
									break;
								}
							}
						}
					}
				};

				a.Click += delegate ( EditorAction.ClickContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
					{
						var toRemove = new ESet<string>();

						var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
						foreach( var fileItem in fileItems )
						{
							if( fileItem.IsDirectory )
							{
								var fullPaths = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true );
								var files = Directory.GetFiles( fileItem.FullPath, "*.cs", SearchOption.AllDirectories );
								foreach( var file in files )
								{
									if( fullPaths.Contains( file ) )
										toRemove.AddWithCheckAlreadyContained( file );
								}
							}

							if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".cs" )
							{
								bool added = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fileItem.FullPath );
								if( added )
									toRemove.Add( fileItem.FullPath );
							}
						}

						if( toRemove.Count != 0 )
						{
							if( CSharpProjectFileUtility.UpdateProjectFile( null, toRemove, out var error ) )
							{
								if( toRemove.Count > 1 )
									Log.Info( EditorLocalization.Translate( "General", "Items have been removed from the Project.csproj." ) );
								else
									Log.Info( EditorLocalization.Translate( "General", "The item has been removed from the Project.csproj." ) );
							}
							else
								Log.Warning( error );
						}
					}
				};

				EditorActions.Register( a );
			}

			////Add to Project.csproj
			//{
			//	var a = new EditorAction();
			//	a.Name = "Add to Project.csproj";
			//	a.ImageSmall = Properties.Resources.Add_16;
			//	a.ImageBig = Properties.Resources.Add_32;
			//	a.QatSupport = true;
			//	a.ContextMenuSupport = true;
			//	a.RibbonText = ("Add", "");

			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//		if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
			//		{
			//			var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
			//			foreach( var fileItem in fileItems )
			//			{
			//				if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".cs" )
			//				{
			//					bool added = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fileItem.FullPath );
			//					if( !added )
			//					{
			//						context.Enabled = true;
			//						break;
			//					}
			//				}
			//			}
			//		}
			//	};

			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//		if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
			//		{
			//			var toAdd = new List<string>();

			//			var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
			//			foreach( var fileItem in fileItems )
			//			{
			//				if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".cs" )
			//				{
			//					bool added = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fileItem.FullPath );
			//					if( !added )
			//						toAdd.Add( fileItem.FullPath );
			//				}
			//			}

			//			if( toAdd.Count != 0 )
			//			{
			//				if( CSharpProjectFileUtility.UpdateProjectFile( toAdd, null, out var error ) )
			//				{
			//					if( toAdd.Count > 1 )
			//						Log.Info( $"{toAdd.Count} items were added to Project.csproj." );
			//					else
			//						Log.Info( "1 item was added to Project.csproj." );
			//				}
			//				else
			//					Log.Warning( error );
			//			}
			//		}
			//	};

			//	EditorActions.Register( a );
			//}

			////Remove from Project.csproj
			//{
			//	var a = new EditorAction();
			//	a.Name = "Remove from Project.csproj";
			//	a.ImageSmall = Properties.Resources.Delete_16;
			//	a.ImageBig = Properties.Resources.Delete_32;
			//	a.QatSupport = true;
			//	a.ContextMenuSupport = true;
			//	a.RibbonText = ("Remove", "");

			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//		if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
			//		{
			//			var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
			//			foreach( var fileItem in fileItems )
			//			{
			//				if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".cs" )
			//				{
			//					bool added = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fileItem.FullPath );
			//					if( added )
			//					{
			//						context.Enabled = true;
			//						break;
			//					}
			//				}
			//			}
			//		}
			//	};

			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//		if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
			//		{
			//			var toRemove = new List<string>();

			//			var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
			//			foreach( var fileItem in fileItems )
			//			{
			//				if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".cs" )
			//				{
			//					bool added = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fileItem.FullPath );
			//					if( added )
			//						toRemove.Add( fileItem.FullPath );
			//				}
			//			}

			//			if( toRemove.Count != 0 )
			//			{
			//				if( CSharpProjectFileUtility.UpdateProjectFile( null, toRemove, out var error ) )
			//				{
			//					if( toRemove.Count > 1 )
			//						Log.Info( $"{toRemove.Count} items were removed from Project.csproj." );
			//					else
			//						Log.Info( "1 item was removed from Project.csproj." );
			//				}
			//				else
			//					Log.Warning( error );
			//			}
			//		}
			//	};

			//	EditorActions.Register( a );
			//}

			////Add C# files to Project.csproj
			//{
			//	var a = new EditorAction();
			//	a.Name = "Add C# files to Project.csproj";
			//	a.ImageSmall = Properties.Resources.Add_16;
			//	a.ImageBig = Properties.Resources.Add_32;
			//	a.QatSupport = true;
			//	a.ContextMenuSupport = true;
			//	a.RibbonText = ("Add", "Folder");

			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//		if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
			//		{
			//			var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
			//			foreach( var fileItem in fileItems )
			//			{
			//				if( fileItem.IsDirectory )
			//				{
			//					bool skip = false;

			//					if( context.Holder == EditorAction.HolderEnum.ContextMenu )
			//					{
			//						var fullPaths = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true );
			//						var files = Directory.GetFiles( fileItem.FullPath, "*.cs", SearchOption.AllDirectories );
			//						var existNotAdded = files.Any( f => !fullPaths.Contains( f ) );
			//						if( !existNotAdded )
			//							skip = true;
			//					}

			//					if( !skip )
			//					{
			//						context.Enabled = true;
			//						break;
			//					}
			//				}
			//			}
			//		}
			//	};

			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//		if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
			//		{
			//			var toAdd = new ESet<string>();

			//			var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
			//			foreach( var fileItem in fileItems )
			//			{
			//				if( fileItem.IsDirectory )
			//				{
			//					var fullPaths = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true );
			//					var files = Directory.GetFiles( fileItem.FullPath, "*.cs", SearchOption.AllDirectories );
			//					foreach( var file in files )
			//					{
			//						if( !fullPaths.Contains( file ) )
			//							toAdd.AddWithCheckAlreadyContained( file );
			//					}
			//				}
			//			}

			//			if( toAdd.Count != 0 )
			//			{
			//				if( CSharpProjectFileUtility.UpdateProjectFile( toAdd, null, out var error ) )
			//				{
			//					if( toAdd.Count > 1 )
			//						Log.Info( $"{toAdd.Count} items were added to Project.csproj." );
			//					else
			//						Log.Info( "1 item was added to Project.csproj." );
			//				}
			//				else
			//					Log.Warning( error );
			//			}
			//		}
			//	};

			//	EditorActions.Register( a );
			//}

			////Remove C# files from Project.csproj
			//{
			//	var a = new EditorAction();
			//	a.Name = "Remove C# files from Project.csproj";
			//	a.ImageSmall = Properties.Resources.Delete_16;
			//	a.ImageBig = Properties.Resources.Delete_32;
			//	a.QatSupport = true;
			//	a.ContextMenuSupport = true;
			//	a.RibbonText = ("Remove", "Folder");

			//	a.GetState += delegate ( EditorAction.GetStateContext context )
			//	{
			//		if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
			//		{
			//			var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
			//			foreach( var fileItem in fileItems )
			//			{
			//				if( fileItem.IsDirectory )
			//				{
			//					bool skip = false;

			//					if( context.Holder == EditorAction.HolderEnum.ContextMenu )
			//					{
			//						var fullPaths = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true );
			//						var files = Directory.GetFiles( fileItem.FullPath, "*.cs", SearchOption.AllDirectories );
			//						var existAdded = files.Any( f => fullPaths.Contains( f ) );
			//						if( !existAdded )
			//							skip = true;
			//					}

			//					if( !skip )
			//					{
			//						context.Enabled = true;
			//						break;
			//					}
			//				}
			//			}
			//		}
			//	};

			//	a.Click += delegate ( EditorAction.ClickContext context )
			//	{
			//		if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
			//		{
			//			var toRemove = new ESet<string>();

			//			var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
			//			foreach( var fileItem in fileItems )
			//			{
			//				if( fileItem.IsDirectory )
			//				{
			//					var fullPaths = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true );
			//					var files = Directory.GetFiles( fileItem.FullPath, "*.cs", SearchOption.AllDirectories );
			//					foreach( var file in files )
			//					{
			//						if( fullPaths.Contains( file ) )
			//							toRemove.AddWithCheckAlreadyContained( file );
			//					}
			//				}
			//			}

			//			if( toRemove.Count != 0 )
			//			{
			//				if( CSharpProjectFileUtility.UpdateProjectFile( null, toRemove, out var error ) )
			//				{
			//					if( toRemove.Count > 1 )
			//						Log.Info( $"{toRemove.Count} items were removed from Project.csproj." );
			//					else
			//						Log.Info( "1 item was removed from Project.csproj." );
			//				}
			//				else
			//					Log.Warning( error );
			//			}
			//		}
			//	};

			//	EditorActions.Register( a );
			//}

			//Build Project's Solution
			{
				var a = new EditorAction();
				a.Name = "Build Project's Solution";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.Build_16;
				a.ImageSmall_Dark = Properties.Resources.Build_16_Dark;
				a.ImageBig = Properties.Resources.Build_32;
				a.ImageBig_Dark = Properties.Resources.Build_32_Dark;
				a.QatSupport = true;
				a.RibbonText = ("Build", "Solution");
				a.Description = "Builds the project's solution.";
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					EditorAPI.SaveDocuments();
					//// save cs documents in editor, before compilation.
					//EditorForm.Instance.SaveDocuments(doc => doc.Name.EndsWith(".cs"));

#if !PROJECT_DEPLOY
					CSharpProjectFileUtility.CheckToRemoveNotExistsFilesFromProject();

					var rebuild = Control.ModifierKeys.HasFlag( Keys.Control );

					if( rebuild || CSharpProjectFileUtility.CompilationIsRequired() )
					{
						if( CSharpProjectFileUtility.Compile( rebuild, outputDirOverride: CSharpProjectFileUtility.GetProjectTempDir() ) )
						{
							var text = EditorLocalization.Translate( "General", "Project.sln was built successfully.\r\n\r\nTo apply changes need restart the editor. Restart?" );
							if( EditorMessageBox.ShowQuestion( text, MessageBoxButtons.YesNo ) == DialogResult.Yes )
							{
								EditorAPI.BeginRestartApplication();
							}
						}
						//else
						//{
						//}
					}
					else
					{
						EditorMessageBox.ShowInfo( EditorLocalization.Translate( "General", "Project.sln compilation skipped because output file is up-to-date.\r\n\r\nYou can rebuild the solution. To do this, hold the Control key while clicking the \'Build Solution\'." ) );
					}
#endif
				};
				EditorActions.Register( a );
			}

			//Build and Apply Project's Solution
			{
				var a = new EditorAction();
				a.Name = "Build and Apply Project's Solution";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.Update_16;
				a.ImageBig = Properties.Resources.Update_32;
				a.QatSupport = true;
				a.RibbonText = ("Apply", "");// "Solution");
				a.Description = "Builds the project's solution and updates the editor with a new code without restarting (hot refresh). (The feature is not implemented)";
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
				};
				EditorActions.Register( a );
			}

			//Open Project's Solution in external IDE
			{
				var a = new EditorAction();
				a.Name = "Open Project Solution in External IDE";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.External_16;
				a.ImageBig = Properties.Resources.External_32;
				a.QatSupport = true;
				a.RibbonText = ("Project", "Solution");
				a.Description = "Opens Project solution in an external IDE.";
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					Process.Start( Path.Combine( VirtualFileSystem.Directories.Project, "Project.sln" ) );
				};
				EditorActions.Register( a );
			}

			//Open Sources Solution in external IDE
			{
				var a = new EditorAction();
				a.Name = "Open Sources Solution in External IDE";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.External_16;
				a.ImageBig = Properties.Resources.External_32;
				a.QatSupport = true;
				a.RibbonText = ("Sources", "Solution");
				a.Description = "Opens Sources solution in an external IDE.";
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					Process.Start( Path.Combine( VirtualFileSystem.Directories.Project, "..\\NeoAxis.Managed.sln" ) );
				};
				EditorActions.Register( a );
			}

			//C# File
			{
				var a = new EditorAction_NewResource( typeof( NewResourceType_CSharpClass ) );
				a.Name = "C# File";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Creates a new C# file in the project.";
				a.RibbonText = ("C# File", "");
				//override New icons
				a.ImageSmall = Properties.Resources.NewCSharp_16;
				a.ImageBig = Properties.Resources.NewCSharp_32;
				EditorActions.Register( a );
			}

			//C# Script
			{
				var a = new EditorAction_NewResource( typeof( Component_CSharpScript ) );
				a.Name = "C# Script";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Creates a new C# script file.";
				a.RibbonText = ("C#", "Script");
				//override New icons
				a.ImageSmall = Properties.Resources.NewCSharp_16;
				a.ImageBig = Properties.Resources.NewCSharp_32;
				EditorActions.Register( a );
			}

			//Flow Graph
			{
				var a = new EditorAction_NewResource( typeof( Component_FlowGraph ) );
				a.Name = "Flow Graph";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.Description = "Creates a new flow graph file.";
				a.RibbonText = ("Flow", "Graph");
				EditorActions.Register( a );
			}

			//Comment Selection
			{
				var a = new EditorAction();
				a.Name = "Comment Selection";
				a.Description = "Comments the selected text.";
				a.ImageSmall = Properties.Resources.Comment_16;
				a.ImageBig = Properties.Resources.Comment_32;
				a.ShortcutKeys = new Keys[] { Keys.Control | Keys.K };
				a.QatSupport = true;
				a.RibbonText = ("Comment", "");
				EditorActions.Register( a );
			}

			//Uncomment Selection
			{
				var a = new EditorAction();
				a.Name = "Uncomment Selection";
				a.Description = "Uncomments the selected text.";
				a.ImageSmall = Properties.Resources.Uncomment_16;
				a.ImageBig = Properties.Resources.Uncomment_32;
				a.ShortcutKeys = new Keys[] { Keys.Control | Keys.U };
				a.QatSupport = true;
				a.RibbonText = ("Uncomment", "");
				EditorActions.Register( a );
			}

			////Rename Symbol
			//{
			//	var a = new EditorAction();
			//	a.Name = "Rename Symbol";
			//	//a.Description = "";
			//	a.ImageSmall = Properties.Resources.Rename_16;
			//	a.ImageBig = Properties.Resources.Rename_32;
			//	a.ShortcutKeys = new Keys[] { Keys.F2 };
			//	a.QatSupport = true;
			//	a.RibbonText = ("Rename", "");
			//	EditorActions.Register( a );
			//}

			//Format Document
			{
				var a = new EditorAction();
				a.Name = "Format Document";
				a.Description = "Does auto formatting of selected text.";
				a.ImageSmall = Properties.Resources.FormatDocument_16;
				a.ImageBig = Properties.Resources.FormatDocument_32;
				a.ShortcutKeys = new Keys[] { Keys.Control | Keys.R };
				a.QatSupport = true;
				a.RibbonText = ("Format", "");
				EditorActions.Register( a );
			}

			//Go To Definition
			{
				var a = new EditorAction();
				a.Name = "Go To Definition";
				//a.Description = "";
				//a.ImageSmall = Properties.Resources.FormatDocument_16;
				//a.ImageBig = Properties.Resources.FormatDocument_32;
				a.ShortcutKeys = new Keys[] { Keys.Alt | Keys.Z, Keys.F12 };
				a.QatSupport = true;
				a.RibbonText = ("Definition", "");
				EditorActions.Register( a );
			}

			//Add Property Code
			{
				var a = new EditorAction();
				a.Name = "Add Property Code";
				a.Description = "Adds property code for a component class.";
				a.ImageSmall = Properties.Resources.New_16;
				a.ImageBig = Properties.Resources.New_32;
				//a.ShortcutKeys = new Keys[] { Keys.Control | Keys.R };
				a.QatSupport = true;
				a.RibbonText = ("Add", "Property");
				EditorActions.Register( a );
			}

			//Debug Start
			{
				var a = new EditorAction();
				a.Name = "Debug Start";
				a.Description = "Start debugging. (The feature is not implemented)";
				a.ImageSmall = Properties.Resources.PlayGreen_16;
				a.ImageBig = Properties.Resources.PlayGreen_32;
				//a.ShortcutKeys = new Keys[] { Keys.Control | Keys.R };
				a.QatSupport = true;
				a.RibbonText = ("Start", "");
				EditorActions.Register( a );
			}

			//Debug Break
			{
				var a = new EditorAction();
				a.Name = "Debug Break";
				a.Description = "Debugging break all. (The feature is not implemented)";
				a.ImageSmall = Properties.Resources.Pause_16;
				a.ImageBig = Properties.Resources.Pause_32;
				//a.ShortcutKeys = new Keys[] { Keys.Control | Keys.R };
				a.QatSupport = true;
				a.RibbonText = ("Break", "");
				EditorActions.Register( a );
			}

			//Debug Stop
			{
				var a = new EditorAction();
				a.Name = "Debug Stop";
				a.Description = "Stop debugging. (The feature is not implemented)";
				//!!!!
				a.ImageSmall = Properties.Resources.DebugCircle_16;
				a.ImageBig = Properties.Resources.DebugCircle_32;
				//a.ShortcutKeys = new Keys[] { Keys.Control | Keys.R };
				a.QatSupport = true;
				a.RibbonText = ("Stop", "");
				EditorActions.Register( a );
			}

			//Debug Next Statement
			{
				var a = new EditorAction();
				a.Name = "Debug Next Statement";
				a.Description = "Show next statement. (The feature is not implemented)";
				//!!!!
				a.ImageSmall = Properties.Resources.DebugCircle_16;
				a.ImageBig = Properties.Resources.DebugCircle_32;
				//a.ShortcutKeys = new Keys[] { Keys.Control | Keys.R };
				a.QatSupport = true;
				a.RibbonText = ("Next", "");
				EditorActions.Register( a );
			}

			//Debug Step Into
			{
				var a = new EditorAction();
				a.Name = "Debug Step Into";
				a.Description = "Step into. (The feature is not implemented)";
				//!!!!
				a.ImageSmall = Properties.Resources.DebugCircle_16;
				a.ImageBig = Properties.Resources.DebugCircle_32;
				//a.ShortcutKeys = new Keys[] { Keys.Control | Keys.R };
				a.QatSupport = true;
				a.RibbonText = ("Step", "Into");
				EditorActions.Register( a );
			}

			//Debug Step Over
			{
				var a = new EditorAction();
				a.Name = "Debug Step Over";
				a.Description = "Step over. (The feature is not implemented)";
				//!!!!
				a.ImageSmall = Properties.Resources.DebugCircle_16;
				a.ImageBig = Properties.Resources.DebugCircle_32;
				//a.ShortcutKeys = new Keys[] { Keys.Control | Keys.R };
				a.QatSupport = true;
				a.RibbonText = ("Step", "Over");
				EditorActions.Register( a );
			}

			//Debug Step Out
			{
				var a = new EditorAction();
				a.Name = "Debug Step Out";
				a.Description = "Step out. (The feature is not implemented)";
				//!!!!
				a.ImageSmall = Properties.Resources.DebugCircle_16;
				a.ImageBig = Properties.Resources.DebugCircle_32;
				//a.ShortcutKeys = new Keys[] { Keys.Control | Keys.R };
				a.QatSupport = true;
				a.RibbonText = ("Step", "Out");
				EditorActions.Register( a );
			}

			////Find in Files
			//{
			//	var a = new EditorAction();
			//	a.Name = "Find in Files";
			//	a.Description = "Find and replace in files.";
			//	a.ImageSmall = Properties.Resources.Focus_16;
			//	a.ImageBig = Properties.Resources.Focus_32;
			//	//a.ShortcutKeys = new Keys[] { Keys.Control | Keys.R };
			//	a.QatSupport = true;
			//	a.RibbonText = ("Find", "");
			//	EditorActions.Register( a );
			//}

		}

		/////////////////////////////////////////

		class ImageCompressionTag
		{
			public string name;
			public bool isState;
			public ICollection<string> fileNames;

			public ImageCompressionTag( string name, bool isState )
			{
				this.name = name;
				this.isState = isState;
			}
		}

		/////////////////////////////////////////

		class ProcessEnvironmentCubemapTag
		{
			public int size;
			public bool isDelete;
			public ICollection<string> fileNames;

			public ProcessEnvironmentCubemapTag( int size, bool isDelete )
			{
				this.size = size;
				this.isDelete = isDelete;
			}
		}

		static void RegisterImage()
		{
			//Image Compression
			{
				var a = new EditorAction();
				a.Name = "Image Compression";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.Compress_16;
				a.ImageBig = Properties.Resources.Compress_32;
				a.QatSupport = true;
				a.RibbonText = ("Image", "Compression");
				a.ActionType = EditorAction.ActionTypeEnum.DropDown;
				a.ContextMenuSupport = EditorContextMenu.MenuTypeEnum.Resources;

				EventHandler clickHandler = delegate ( object s, EventArgs e2 )
				{
					var item = (KryptonContextMenuItem)s;
					var tag = (ImageCompressionTag)item.Tag;

					if( tag.isState )
					{
						var value = tag.name;
						if( value == "Auto" )
							value = "";

						foreach( var fileName in tag.fileNames )
						{
							//update settings
							if( !Component_Image_Settings.SetParameter( fileName, "Compression", value, out var error ) )
							{
								Log.Error( error );
								return;
							}

							//delete old file
							if( !Component_Image.DeleteCompressedFile( fileName, out error ) )
							{
								Log.Error( error );
								return;
							}

							//update when already loaded
							var resource = ResourceManager.GetByName( fileName );
							var texture = resource?.PrimaryInstance?.ResultComponent as Component_Image;
							if( texture != null )
								texture.ShouldRecompile = true;
						}
					}
					else if( tag.name == "Update" )
					{
						foreach( var fileName in tag.fileNames )
						{
							//delete old file
							if( !Component_Image.DeleteCompressedFile( fileName, out var error ) )
							{
								Log.Error( error );
								return;
							}

							//update when already loaded
							var resource = ResourceManager.GetByName( fileName );
							var texture = resource?.PrimaryInstance?.ResultComponent as Component_Image;
							if( texture != null )
								texture.ShouldRecompile = true;
						}
					}

				};

				{
					var items = new List<KryptonContextMenuItemBase>();

					//states
					var list = new List<string>();
					list.Add( "Auto" );
					list.Add( "NoCompression" );
					list.Add( "DXT1" );
					list.Add( "DXT5" );
					list.Add( "NormalMap" );
					foreach( var name in list )
					{
						var item = new KryptonContextMenuItem( EditorLocalization.Translate( "ImageCompression", TypeUtility.DisplayNameAddSpaces( name ) ), null, clickHandler );
						item.Tag = new ImageCompressionTag( name, true );
						items.Add( item );
					}

					items.Add( new KryptonContextMenuSeparator() );

					//Update
					{
						var item = new KryptonContextMenuItem( EditorLocalization.Translate( "ImageCompression", "Update" ), null, clickHandler );
						item.Tag = new ImageCompressionTag( "Update", false );
						items.Add( item );
					}

					a.DropDownContextMenu = new KryptonContextMenu();
					a.DropDownContextMenu.Items.Add( new KryptonContextMenuItems( items.ToArray() ) );
				}

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
					{
						var fileNames = new List<string>();

						var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
						foreach( var fileItem in fileItems )
						{
							if( !fileItem.IsDirectory )
							{
								var resourceType = ResourceManager.GetTypeByFileExtension( Path.GetExtension( fileItem.FullPath ) );
								if( resourceType?.Name == "Image" )
								{
									var fileName = VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath );
									if( !string.IsNullOrEmpty( fileName ) )
										fileNames.Add( fileName );
								}
							}
						}

						if( fileNames.Count != 0 )
						{
							context.Enabled = true;

							string[] parameters = new string[ fileNames.Count ];
							for( int n = 0; n < parameters.Length; n++ )
							{
								var compression = Component_Image_Settings.GetParameter( fileNames[ n ], "Compression", out var error );
								if( string.IsNullOrEmpty( compression ) )
									compression = "Auto";
								parameters[ n ] = compression;
							}

							var items = (KryptonContextMenuItems)a.DropDownContextMenu.Items[ 0 ];
							foreach( var item in items.Items )
							{
								var menuItem = item as KryptonContextMenuItem;
								if( menuItem != null )
								{
									var tag = (ImageCompressionTag)menuItem.Tag;
									tag.fileNames = fileNames;

									menuItem.Checked = parameters.Any( p => p == tag.name );
								}
							}
						}
					}
				};

				EditorActions.Register( a );
			}

			//Process Environment Cubemap
			{
				var a = new EditorAction();
				a.Name = "Process Environment Cubemap";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Process", "Env Cubemap");
				a.ActionType = EditorAction.ActionTypeEnum.DropDown;
				a.ContextMenuSupport = EditorContextMenu.MenuTypeEnum.Resources;

				EventHandler clickHandler = delegate ( object s, EventArgs e2 )
				{
					var item = (KryptonContextMenuItem)s;
					var tag = (ProcessEnvironmentCubemapTag)item.Tag;

					if( !tag.isDelete )
					{
						foreach( var fileName in tag.fileNames )
						{
							if( !CubemapProcessing.GetOrGenerate( fileName, true, tag.size, out _, out _, out var error ) )
							{
								Log.Error( error );
								break;
							}
						}
					}
					else
					{
						var filesToDelete = new List<string>();
						foreach( var fileName in tag.fileNames )
						{
							var names = new string[] { "_Gen.info", "_GenEnv.dds", "_GenIrr.dds" };
							foreach( var name in names )
							{
								var fileName2 = VirtualPathUtility.GetRealPathByVirtual( fileName + name );
								if( File.Exists( fileName2 ) )
									filesToDelete.Add( fileName2 );
							}
						}

						var text = EditorLocalization.Translate( "ProcessEnvironmentCubemap", "Are you sure you want to delete these files?" );
						if( EditorMessageBox.ShowQuestion( text, MessageBoxButtons.YesNo ) == DialogResult.Yes )
						{
							try
							{
								foreach( var fileName in filesToDelete )
									File.Delete( fileName );
							}
							catch( Exception e )
							{
								Log.Error( e.Message );
							}
						}
					}
				};

				{
					var items = new List<KryptonContextMenuItemBase>();

					var list = new List<int>();
					list.Add( 0 );
					list.Add( 64 );
					list.Add( 128 );
					list.Add( 256 );
					list.Add( 512 );
					list.Add( 1024 );
					list.Add( 2048 );
					list.Add( 4096 );
					list.Add( 8192 );
					foreach( var size in list )
					{
						var name = size.ToString();
						if( size == 0 )
							name = EditorLocalization.Translate( "ProcessEnvironmentCubemap", "Auto Size" );

						var item = new KryptonContextMenuItem( name, null, clickHandler );
						item.Tag = new ProcessEnvironmentCubemapTag( size, false );
						items.Add( item );
					}

					items.Add( new KryptonContextMenuSeparator() );

					//Update
					{
						var item = new KryptonContextMenuItem( EditorLocalization.Translate( "ProcessEnvironmentCubemap", "Delete" ), null, clickHandler );
						item.Tag = new ProcessEnvironmentCubemapTag( 0, true );
						items.Add( item );
					}

					a.DropDownContextMenu = new KryptonContextMenu();
					a.DropDownContextMenu.Items.Add( new KryptonContextMenuItems( items.ToArray() ) );
				}

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
					{
						var fileNames = new List<string>();

						var fileItems = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>();
						foreach( var fileItem in fileItems )
						{
							if( !fileItem.IsDirectory )
							{
								var resourceType = ResourceManager.GetTypeByFileExtension( Path.GetExtension( fileItem.FullPath ) );
								if( resourceType?.Name == "Image" )
								{
									var fileName = VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath );
									if( !string.IsNullOrEmpty( fileName ) )
										fileNames.Add( fileName );
								}
							}
						}

						if( fileNames.Count != 0 )
						{
							context.Enabled = true;

							var items = (KryptonContextMenuItems)a.DropDownContextMenu.Items[ 0 ];
							foreach( var item in items.Items )
							{
								var menuItem = item as KryptonContextMenuItem;
								if( menuItem != null )
								{
									var tag = (ProcessEnvironmentCubemapTag)menuItem.Tag;
									tag.fileNames = fileNames;

									if( tag.isDelete )
									{
										bool can = false;
										foreach( var fileName in fileNames )
										{
											var names = new string[] { "_Gen.info", "_GenEnv.dds", "_GenIrr.dds" };
											foreach( var name in names )
											{
												var fileName2 = fileName + name;
												if( VirtualFile.Exists( fileName2 ) )
												{
													can = true;
													break;
												}
											}
										}
										menuItem.Enabled = can;
									}
								}
							}
						}
					}
				};

				EditorActions.Register( a );
			}
		}

		/////////////////////////////////////////

		static void RegisterTools()
		{
			//Purge Images
			{
				var a = new EditorAction();
				a.Name = "Purge Images";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.Refresh_16;
				a.ImageBig = Properties.Resources.Refresh_32;
				a.QatSupport = true;
				a.RibbonText = ("Purge", "Images");
				a.Description = "Deletes unnecessary auto generated compressed images from the cache.";
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					context.Enabled = true;
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var list = new List<string>();
					{
						var directory = Path.Combine( VirtualFileSystem.Directories.Project, "Caches\\Files" );
						if( Directory.Exists( directory ) )
						{
							foreach( var fullPath in Directory.GetFiles( directory, "*.dds", SearchOption.AllDirectories ) )
							{
								var fileName = fullPath.Substring( directory.Length + 1 );
								fileName = fileName.Substring( 0, fileName.Length - 4 );

								if( !VirtualFile.Exists( fileName ) )
									list.Add( fullPath );
							}
						}
					}

					if( list.Count != 0 )
					{
						var text = string.Format( EditorLocalization.Translate( "General", "{0} unnecessary images found. Delete them?" ), list.Count ) + "\r\n\r\n";

						int counter = 0;
						foreach( var fileName in list )
						{
							text += fileName + "\r\n";

							counter++;
							if( counter > 10 )
							{
								text += "...";
								break;
							}
						}

						if( EditorMessageBox.ShowQuestion( text, MessageBoxButtons.YesNo ) == DialogResult.Yes )
						{
							try
							{
								foreach( var fullPath in list )
								{
									File.Delete( fullPath );

									var fullPath2 = fullPath + ".info";
									if( File.Exists( fullPath2 ) )
										File.Delete( fullPath2 );
								}

								EditorMessageBox.ShowInfo( EditorLocalization.Translate( "General", "Done." ) );
							}
							catch( Exception e )
							{
								EditorMessageBox.ShowWarning( e.Message );
							}
						}
					}
					else
						EditorMessageBox.ShowInfo( EditorLocalization.Translate( "General", "Unnecessary compressed images not found." ) );
				};
				EditorActions.Register( a );
			}

			//Create NeoAxis Baking
			{
				var a = new EditorAction();
				a.Name = "Create NeoAxis Baking File";
				a.CommonType = EditorAction.CommonTypeEnum.General;
				a.ImageSmall = Properties.Resources.Package_16;
				a.ImageBig = Properties.Resources.Package_32;
				a.QatSupport = true;
				//a.ContextMenuSupport = true;
				a.RibbonText = ("NeoAxis", "Baking");
				a.Description = "Creates a NeoAxis Baking file from selected folders and files.";

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
					{
						var fullPaths = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>().Select( item => item.FullPath ).ToArray();
						if( fullPaths.Length != 0 )
						{
							//check folders and files have one parent folder
							try
							{
								string parentFolder = "";

								bool first = true;
								foreach( var fullPath in fullPaths )
								{
									if( first )
										parentFolder = Path.GetDirectoryName( fullPath );
									else
									{
										var folder = Path.GetDirectoryName( fullPath );
										if( parentFolder != folder )
											return;
									}

									first = false;
								}
							}
							catch
							{
								return;
							}

							context.Enabled = true;
						}
					}
				};

				a.Click += delegate ( EditorAction.ClickContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow == null && context.ObjectsInFocus.Objects.Length != 0 )
					{
						var fullPaths = context.ObjectsInFocus.Objects.OfType<ContentBrowserItem_File>().Select( item => item.FullPath ).ToArray();
						if( fullPaths.Length != 0 )
						{
							string defaultFileName;
							{
								if( fullPaths.Length > 1 )
								{
									string parentFolder = "";
									foreach( var fullPath in fullPaths )
									{
										parentFolder = Path.GetDirectoryName( fullPath );
										break;
									}
									defaultFileName = Path.Combine( parentFolder, Path.GetFileName( parentFolder ) + ".neoaxisbaking" );
								}
								else
								{
									var fullPath = fullPaths[ 0 ];
									var parentFolder = Path.GetDirectoryName( fullPath );
									defaultFileName = Path.Combine( parentFolder, Path.GetFileName( fullPath ) + ".neoaxisbaking" );
								}
							}

							var form = new OKCancelTextBoxForm( EditorLocalization.Translate( "Baking", "File name" ) + ":", defaultFileName, EditorLocalization.Translate( "Baking", "NeoAxis Baking" ),
								delegate ( string text, ref string error )
							{
								if( string.IsNullOrEmpty( text ) )
								{
									error = EditorLocalization.Translate( "Baking", "Please specify destination file name." );
									return false;
								}

								//!!!!или спрашивать overwrite
								if( File.Exists( text ) || Directory.Exists( text ) )
								{
									error = EditorLocalization.Translate( "Baking", "A file or folder with the same name already exists." );
									return false;
								}

								return true;
							}, delegate ( string text, ref string error )
							{
								var result = EditorMessageBox.ShowQuestion( EditorLocalization.Translate( "Baking", "Compress archive?" ), MessageBoxButtons.YesNoCancel );
								if( result == DialogResult.Cancel )
									return false;

								bool compress = result == DialogResult.Yes;

								var success = BakingFile.Create( fullPaths, compress, text, out error );
								if( success )
									ScreenNotifications.Show( EditorLocalization.Translate( "Baking", "The file was built successfully." ) );
								return success;
							} );

							form.ShowDialog();
						}
					}
				};

				EditorActions.Register( a );
			}

		}

		/////////////////////////////////////////

		static void RegisterCamera()
		{
			//Load Camera Settings
			{
				var a = new EditorAction();
				a.Name = "Load Camera Settings";
				a.Description = "Copy camera settings of the current viewport from selected camera.";
				//a.ImageSmall = Properties.Resources.Default_16;
				//a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.ContextMenuSupport = EditorContextMenu.MenuTypeEnum.Document;

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow as Component_Scene_DocumentWindow != null )
					{
						if( context.ObjectsInFocus.Objects.Count( obj => obj is Component_Camera ) == 1 )
							context.Enabled = true;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = ( context.ObjectsInFocus.DocumentWindow as Component_Scene_DocumentWindow ).Scene;
					var camera = (Component_Camera)context.ObjectsInFocus.Objects.First( obj => obj is Component_Camera );

					var viewportCamera = scene.Mode.Value == Component_Scene.ModeEnum._3D ? scene.CameraEditor.Value : scene.CameraEditor2D.Value;
					if( viewportCamera != null )
					{
						viewportCamera.Transform = camera.Transform;
						viewportCamera.Projection = camera.Projection;
						viewportCamera.FieldOfView = camera.FieldOfView;
						viewportCamera.Height = camera.Height;
						viewportCamera.AspectRatio = camera.AspectRatio;
						viewportCamera.FixedUp = camera.FixedUp;
						viewportCamera.NearClipPlane = camera.NearClipPlane;
						viewportCamera.FarClipPlane = camera.FarClipPlane;
					}
				};
				EditorActions.Register( a );
			}

			//Save Camera Settings
			{
				var a = new EditorAction();
				a.Name = "Save Camera Settings";
				a.Description = "Copy camera settings of the current viewport to selected camera.";
				//a.ImageSmall = Properties.Resources.Default_16;
				//a.ImageBig = Properties.Resources.Default_32;
				a.QatSupport = true;
				a.ContextMenuSupport = EditorContextMenu.MenuTypeEnum.Document;

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow as Component_Scene_DocumentWindow != null )
					{
						if( context.ObjectsInFocus.Objects.Count( obj => obj is Component_Camera ) == 1 )
							context.Enabled = true;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var scene = ( context.ObjectsInFocus.DocumentWindow as Component_Scene_DocumentWindow ).Scene;
					var camera = (Component_Camera)context.ObjectsInFocus.Objects.First( obj => obj is Component_Camera );

					var viewportCamera = scene.Mode.Value == Component_Scene.ModeEnum._3D ? scene.CameraEditor.Value : scene.CameraEditor2D.Value;
					if( viewportCamera != null )
					{
						var undoItems = new List<UndoActionPropertiesChange.Item>();

						var context2 = new Metadata.GetMembersContext( false );
						undoItems.Add( new UndoActionPropertiesChange.Item( camera, (Metadata.Property)camera.MetadataGetMemberBySignature( "property:Transform", context2 ), camera.Transform ) );
						undoItems.Add( new UndoActionPropertiesChange.Item( camera, (Metadata.Property)camera.MetadataGetMemberBySignature( "property:Projection", context2 ), camera.Projection ) );
						undoItems.Add( new UndoActionPropertiesChange.Item( camera, (Metadata.Property)camera.MetadataGetMemberBySignature( "property:FieldOfView", context2 ), camera.FieldOfView ) );
						undoItems.Add( new UndoActionPropertiesChange.Item( camera, (Metadata.Property)camera.MetadataGetMemberBySignature( "property:Height", context2 ), camera.Height ) );
						undoItems.Add( new UndoActionPropertiesChange.Item( camera, (Metadata.Property)camera.MetadataGetMemberBySignature( "property:AspectRatio", context2 ), camera.AspectRatio ) );
						undoItems.Add( new UndoActionPropertiesChange.Item( camera, (Metadata.Property)camera.MetadataGetMemberBySignature( "property:FixedUp", context2 ), camera.FixedUp ) );
						undoItems.Add( new UndoActionPropertiesChange.Item( camera, (Metadata.Property)camera.MetadataGetMemberBySignature( "property:NearClipPlane", context2 ), camera.NearClipPlane ) );
						undoItems.Add( new UndoActionPropertiesChange.Item( camera, (Metadata.Property)camera.MetadataGetMemberBySignature( "property:FarClipPlane", context2 ), camera.FarClipPlane ) );

						camera.Transform = viewportCamera.Transform;
						camera.Projection = viewportCamera.Projection;
						camera.FieldOfView = viewportCamera.FieldOfView;
						camera.Height = viewportCamera.Height;
						camera.AspectRatio = viewportCamera.AspectRatio;
						camera.FixedUp = viewportCamera.FixedUp;
						camera.NearClipPlane = viewportCamera.NearClipPlane;
						camera.FarClipPlane = viewportCamera.FarClipPlane;

						var document = context.ObjectsInFocus.DocumentWindow.Document;
						document.UndoSystem.CommitAction( new UndoActionPropertiesChange( undoItems ) );
						document.Modified = true;
					}

				};
				EditorActions.Register( a );
			}
		}

		internal static List<Metadata.TypeInfo> GetAllMeshModifiers()
		{
			var result = new List<Metadata.TypeInfo>();

			foreach( var type in MetadataManager.NetTypes )
			{
				if( MetadataManager.GetTypeOfNetType( typeof( Component_MeshModifier ) ).IsAssignableFrom( type ) && !type.Abstract )
					result.Add( type );
			}

			CollectionUtility.InsertionSort( result, delegate ( Metadata.TypeInfo type1, Metadata.TypeInfo type2 )
			{
				if( type1 == MetadataManager.GetTypeOfNetType( typeof( Component_MeshModifier ) ) )
					return -1;
				if( type2 == MetadataManager.GetTypeOfNetType( typeof( Component_MeshModifier ) ) )
					return 1;

				var name1 = type1.GetUserFriendlyNameForInstance();
				var name2 = type2.GetUserFriendlyNameForInstance();
				return string.Compare( name1, name2 );
			} );

			return result;
		}

		static string GetUniqueFriendlyName( Component component )
		{
			var defaultName = component.BaseType.GetUserFriendlyNameForInstance();
			if( component.Parent.GetComponent( defaultName ) == null )
				return defaultName;
			return component.Parent.Components.GetUniqueName( defaultName, false, 2 );
		}

		static void RegisterNewMeshModifier( Metadata.TypeInfo type )
		{
			var name = type.GetUserFriendlyNameForInstance();

			var a = new EditorAction();
			a.Name = "Mesh Add Modifier " + name;
			a.Description = "Adds a new mesh modifier.";
			a.ImageSmall = Properties.Resources.Modify_16;
			a.ImageBig = Properties.Resources.Modify_32;
			a.QatSupport = true;
			a.RibbonText = (name, "");
			a.ContextMenuText = name;
			a.UserData = type;
			a.GetState += delegate ( EditorAction.GetStateContext context )
			{
				var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
				if( mesh != null )
					context.Enabled = true;
			};
			a.Click += delegate ( EditorAction.ClickContext context )
			{
				var mesh = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as Component_Mesh;
				if( mesh != null )
				{
					var documentWindow = context.ObjectsInFocus.DocumentWindow;
					var type2 = (Metadata.TypeInfo)a.UserData;

					var modifier = mesh.CreateComponent( type2, enabled: false );
					modifier.Name = GetUniqueFriendlyName( modifier );
					modifier.Enabled = true;

					documentWindow.Focus();

					//undo
					var newObjects = new Component[] { modifier };
					var document = context.ObjectsInFocus.DocumentWindow.Document;
					var action = new UndoActionComponentCreateDelete( document, newObjects, true );
					document.CommitUndoAction( action );
					documentWindow.SelectObjects( newObjects.ToArray() );
				}
			};
			EditorActions.Register( a );
		}

#endif

	}
}
