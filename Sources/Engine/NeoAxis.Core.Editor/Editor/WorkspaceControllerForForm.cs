#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Docking;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Workspace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace NeoAxis.Editor
{
	///// <summary>
	///// Workspace controller for main form.
	///// </summary>
	public class WorkspaceControllerForForm : WorkspaceController
	{
		//!!!!
		//public List<DockWindow> lastSelectedDockWindows = new List<DockWindow>();
		//public bool needSelectLastSelectedDockWindow;

		// class for window settings serialization/deserialization
		internal class WindowConfig
		{
			internal string RealFileName { get; set; }
			internal Type Type { get; set; }
			internal string SpecialMode { get; set; }

			internal static WindowConfig FromDocumentWindow( IDocumentWindow window )
			{
				var document = window.Document;
				return new WindowConfig()
				{
					RealFileName = document.RealFileName,
					Type = window.GetType(),
					SpecialMode = document.SpecialMode
				};
			}

			internal void Save( XmlWriter xmlWriter )
			{
				if( !string.IsNullOrEmpty( RealFileName ) )
					xmlWriter.WriteAttributeString( "FileName", VirtualPathUtility.GetVirtualPathByReal( RealFileName ) );
				else
					xmlWriter.WriteAttributeString( "Type", Type.FullName );

				if( !string.IsNullOrEmpty( SpecialMode ) )
					xmlWriter.WriteAttributeString( "SpecialMode", SpecialMode );
			}

			internal void Load( XmlReader xmlReader )
			{
				var virtualFileName = xmlReader.GetAttribute( "FileName" ) ?? "";
				if( !string.IsNullOrEmpty( virtualFileName ) )
					RealFileName = VirtualPathUtility.GetRealPathByVirtual( virtualFileName );

				var typeName = xmlReader.GetAttribute( "Type" );
				if( !string.IsNullOrEmpty( typeName ) )
				{
					var type = MetadataManager.GetType( typeName );
					if( type != null )
						Type = type.GetNetType();
				}

				SpecialMode = xmlReader.GetAttribute( "SpecialMode" ) ?? "";
			}
		}

		List<DocumentWindow> lastSelectedDocumentWindows = new List<DocumentWindow>();

		public DocumentWindow SelectedDocumentWindow //SelectedDockWindow ?
		{
			get
			{
				//remove deleted
				lastSelectedDocumentWindows = lastSelectedDocumentWindows.Where( w => !w.Destroyed ).ToList();

				var window = GetSelectedDockWindow();
				if( window != null && window is DocumentWindow documentWindow )
				{
					if( !documentWindow.OpenAsSettings && !( documentWindow is TipsWindow ) )// don't select window opened as settings
					{
						lastSelectedDocumentWindows.Remove( documentWindow );
						lastSelectedDocumentWindows.Add( documentWindow );
					}
				}

				if( lastSelectedDocumentWindows.Count != 0 )
					return lastSelectedDocumentWindows[ lastSelectedDocumentWindows.Count - 1 ];
				else
					return null;
			}
		}

		internal override DockWindow GetSelectedDockWindow()
		{
			var space = GetActiveKryptonSpace();
			if( space == null )
				return null;

			var activePage = space.ActivePage;
			if( activePage == null )
				return null;

			var window = activePage.GetDockWindow();

			//!!!!maybe check for other windows too?

			//return Resources window only when it focused
			if( window as ResourcesWindow != null && !window.ContainsFocus )
				window = null;

			if( window != null && window is WorkspaceWindow workspaceWindow )
				return workspaceWindow.GetSelectedDockWindow();
			else
				return window;
		}

		private KryptonSpace GetActiveKryptonSpace()
		{
			KryptonSpace space = null;

			var form = Form.ActiveForm;
			if( form != null )
			{
				if( form is EditorForm )
					space = form.ActiveControl as KryptonSpace;
				if( form is KryptonFloatingWindow floatingWindow )
					space = floatingWindow.FloatspaceControl;
			}

			return space;
		}


		public WorkspaceControllerForForm( Control ownerControl, EditorForm ownerForm )
			: base( ownerControl, ownerForm )
		{
		}

		//!!!!name
		//!!!!ESet?
		public List<DockWindow> GetDockWindowsRecursive()
		{
			var result = new List<DockWindow>();
			foreach( var wnd in dockWindows.Values )
			{
				result.Add( wnd );
				if( wnd is WorkspaceWindow workspaceWindow )
				{
					foreach( var wwnd in workspaceWindow.WorkspaceController.GetDockWindows() )
						result.Add( wwnd );
				}
			}
			return result;
		}

		public override void SelectDockWindow( DockWindow window )
		{
			if( window != null )
			{
				if( window is DocumentWindow wnd && wnd.IsWindowInWorkspace )
				{
					var workspaceWindow = FindWorkspaceWindow( wnd );
					if( workspaceWindow != null )
						workspaceWindow.WorkspaceController.SelectDockWindow( window );
				}
				else
					base.SelectDockWindow( window );
			}
		}

		public override void AddDockWindow( DockWindow window, bool floatingWindow, bool select )
		{
			//bool needUnlockEditorForm = false;
			//if( floatingWindow )
			//{
			//	WinFormsUtility.LockFormUpdate( EditorForm.Instance );
			//	needUnlockEditorForm = true;
			//}

			try
			{
				//!!!! fix it. floating window always selected
				if( floatingWindow )
					select = true;

				// если окно не DocumentWindow, с воркспесом не работаем. просто добавляем его.
				if( !( window is DocumentWindow ) )
				{
					base.AddDockWindow( window, floatingWindow, select );
					return;
				}

				var document = ( (DocumentWindow)window ).Document2;
				var windowsForDocument = FindWindowsRecursive( document );

				if( windowsForDocument.Count() == 0 )
				{
					// первое окно с таким документом. просто добавляем его без воркспейса.
					base.AddDockWindow( window, floatingWindow, select );
					return;
				}
				else
				{
					// если есть воркспейс с документом, добавляем в воркспейс

					var workspaceWindow = FindWorkspaceWindow( window );
					if( workspaceWindow != null )
					{
						workspaceWindow.WorkspaceController.AddDockWindow( window, floatingWindow, select );
					}
					else
					{
						// иначе создаём воркспейс и помещаем в него два окна.

						Debug.Assert( windowsForDocument.Count() == 1 );
						var relocationWindow = windowsForDocument.First();

						//prevent blinking
						relocationWindow.Size = new Size( 1, 1 );
						window.Size = new Size( 1, 1 );

						workspaceWindow = new WorkspaceWindow( editorForm );
						workspaceWindow.Init( document );

						// удаляем перемещаемое окно и на его место ставим воркспейс.
						ReplaceDockWindow( relocationWindow, workspaceWindow, false, select );

						// перемещаем перемещаемое окно в новый воркспейс.
						workspaceWindow.WorkspaceController.AddDockWindow( relocationWindow, false, false );

						// добавляем новое окно. выделяем его в последнюю очередь.
						workspaceWindow.WorkspaceController.AddDockWindow( window, floatingWindow, select );
					}
				}
			}
			finally
			{
				//if( needUnlockEditorForm )
				//	WinFormsUtility.LockFormUpdate( null );
			}
		}

		public override void RemoveDockWindow( DockWindow window, bool dispose )
		{
			if( window is DocumentWindow wnd && wnd.IsWindowInWorkspace )
			{
				var workspaceWindow = FindWorkspaceWindow( wnd );
				if( workspaceWindow != null )
					workspaceWindow.WorkspaceController.RemoveDockWindow( window, dispose );
			}
			else
			{
				base.RemoveDockWindow( window, dispose );
			}
		}

		public void SetDockWindowVisibility( DockWindow window, bool visible )
		{
			if( visible )
				dockingManager.ShowPage( window.KryptonPage );
			else
				dockingManager.HidePage( window.KryptonPage );
		}

		//!!!! dont use 'Recursive' in name. these are details of the implementation that the user should not know.
		public DocumentWindow FindWindowRecursive( DocumentInstance doc, object obj, Type type = null )
		{
			foreach( var window in FindWindowsRecursive( doc ) )
			{
				if( type != null && window.GetType() != type )
					continue;

				if( ReferenceEquals( window.ObjectOfWindow, obj ) )
					return window;
			}

			return null;
		}

		public WorkspaceWindow FindWorkspaceWindow( DocumentInstance doc )
		{
			return FindWindow<WorkspaceWindow>( w => w.Document == doc );
		}

		public WorkspaceWindow FindWorkspaceWindow( DockWindow window )
		{
			if( window is DocumentWindow docWindow )
				return FindWindow<WorkspaceWindow>( w => w.Document == docWindow.Document2 );
			else
				return null;
		}

		public IEnumerable<DocumentWindow> FindWindowsRecursive( DocumentInstance doc )
		{
			return GetDockWindowsRecursive().OfType<DocumentWindow>().Where( w => w.Document2 == doc );
		}

		protected override void OnDockWindowCloseRequest( DockWindow window, out bool cancel )
		{
			if( window is WorkspaceWindow workspaceWindow )
			{
				var mainWindow = workspaceWindow.GetMainWindow();
				cancel = editorForm.ShowDialogAndSaveDocument( mainWindow );
			}
			else
			{
				base.OnDockWindowCloseRequest( window, out cancel );
			}

			//needSelectLastSelectedDockWindow = true;
		}

		protected override void OnDockWindowSaving( DockWindow window, DockPageSavingEventArgs e )
		{
			//TODO: make BaseWindowConfig, use fabric method for WindowConfig creation and move this logic to base class.
			if( window is IDocumentWindow docWindow )
				WindowConfig.FromDocumentWindow( docWindow ).Save( e.XmlWriter );
			//window.OnSaving( e.XmlWriter );
		}

		protected override void OnDockWindowLoading( DockWindow window, DockPageLoadingEventArgs e )
		{
			if( window == null )
			{
				var config = new WindowConfig();
				config.Load( e.XmlReader );

				if( !string.IsNullOrEmpty( config.RealFileName ) || !string.IsNullOrEmpty( config.SpecialMode ) )
				{
					var document = EditorAPI2.CreateDocument( config.RealFileName, config.SpecialMode );
					if( document == null )
					{
						//some problems with document
						e.Page = null;
						return;
					}

					window = EditorAPI2.CreateWindow( document );
					AddDockWindowInternal( window, false, e.Page );
				}
				else
				{
					if( config.Type != null && config.Type.GetCustomAttribute<RestoreDockWindowAfterEditorReloadAttribute>( true ) != null )
					{
						//if( config.Type == typeof( StoreDocumentWindow ) || config.Type == typeof( StartPageWindow ) || config.Type == typeof( PackagesWindow ) )
						//{

						var document = new DocumentInstance( "", null, "" );
						EditorAPI2.Documents.Add( document );

						var documentWindow = (DocumentWindow)config.Type.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null );
						window = documentWindow;
						documentWindow.InitDocumentWindow( document, null, false, null );

						AddDockWindowInternal( window, false, e.Page );
					}
				}

			}
			else
			{
				//window already created
			}

			//window?.OnLoading( e.XmlReader );
		}

		public void SaveAdditionalConfig()
		{
			var configBlock = EngineConfig.TextBlock;

			var old = configBlock.FindChild( "Docking" );
			if( old != null )
				configBlock.DeleteChild( old );

			EditorAPI2.GetRestartApplication( out _, out var resetWindowsSettings );
			if( !resetWindowsSettings )
			{
				var block = configBlock.AddChild( "Docking" );

				//save auto hided sizes
				if( dockingControl != null )
				{
					//get auto hide pages
					var autoHidePageNames = new ESet<string>();
					foreach( var element in dockingControl )
					{
						var dockingEdge = element as KryptonDockingEdge;
						if( dockingEdge != null )
						{
							foreach( var e in dockingEdge )
							{
								var edgeAutoHidden = e as KryptonDockingEdgeAutoHidden;
								if( edgeAutoHidden != null )
								{
									foreach( var e2 in edgeAutoHidden )
									{
										var autoHiddenGroup = e2 as KryptonDockingAutoHiddenGroup;
										if( autoHiddenGroup != null )
										{
											foreach( var page in autoHiddenGroup.AutoHiddenGroupControl.Pages )
												autoHidePageNames.AddWithCheckAlreadyContained( page.UniqueName );
										}
									}
								}
							}
						}
					}

					//get all pages
					var allPages = new Dictionary<string, KryptonPage>();
					foreach( var page in DockingManager.Pages )
						allPages[ page.UniqueName ] = page;

					//save
					foreach( var name in autoHidePageNames )
					{
						if( allPages.TryGetValue( name, out var page ) )
						{
							var pageBlock = block.AddChild( "Page" );
							pageBlock.SetAttribute( "Name", page.UniqueName );
							pageBlock.SetAttribute( "AutoHiddenSlideSize", $"{page.AutoHiddenSlideSize.Width} {page.AutoHiddenSlideSize.Height}" );
						}
					}
				}
			}
		}

		protected override KryptonPage CreatePage( DockWindow window, bool createCloseButton, KryptonPage existPage = null )
		{
			var page = base.CreatePage( window, createCloseButton, existPage );

			//read auto hide size from config
			var configBlock = EngineConfig.TextBlock;
			var block = configBlock.FindChild( "Docking" );
			if( block != null )
			{
				foreach( var pageBlock in block.Children )
				{
					if( pageBlock.Name == "Page" )
					{
						var name = pageBlock.GetAttribute( "Name" );
						if( page.UniqueName == name )
						{
							try
							{
								var size = Vector2I.Parse( pageBlock.GetAttribute( "AutoHiddenSlideSize" ) );
								page.AutoHiddenSlideSize = new Size( size.X, size.Y );
							}
							catch { }

							break;
						}
					}
				}
			}

			return page;
		}

	}

	//public class WorkspaceDockingManager : KryptonDockingManager
	//{
	//	public override void SaveElementToXml( XmlWriter xmlWriter )
	//	{
	//		//!!!! at this moment only KryptonDockingControl supported.
	//		var elementsToSave = this.OfType<KryptonDockingControl>().ToList();

	//		// Output docking lement
	//		xmlWriter.WriteStartElement( XmlElementName );
	//		xmlWriter.WriteAttributeString( "N", Name );
	//		xmlWriter.WriteAttributeString( "C", elementsToSave.Count.ToString() );

	//		// Output an element per child
	//		foreach( IDockingElement child in elementsToSave )
	//			child.SaveElementToXml( xmlWriter );

	//		// Terminate the workspace element
	//		xmlWriter.WriteFullEndElement();
	//	}
	//}

}

#endif