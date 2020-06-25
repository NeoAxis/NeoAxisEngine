// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
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
	class WorkspaceControllerForForm : WorkspaceController
	{
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
			if( window != null && window is WorkspaceWindow workspaceWindow )
				return workspaceWindow.GetSelectedDockWindow();
			else
				return window;
		}

		private KryptonSpace GetActiveKryptonSpace()
		{
			KryptonSpace space = null;

			var form = Form.ActiveForm;

			if( form is EditorForm )
				space = form.ActiveControl as KryptonSpace;

			if( form is KryptonFloatingWindow floatingWindow )
				space = floatingWindow.FloatspaceControl;

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
			if( window is DocumentWindow wnd && wnd.IsWindowInWorkspace )
			{
				var workspaceWindow = FindWorkspaceWindow( wnd );
				if( workspaceWindow != null )
					workspaceWindow.WorkspaceController.SelectDockWindow( window );
			}
			else
			{
				base.SelectDockWindow( window );
			}
		}

		public override void AddDockWindow( DockWindow window, bool floatingWindow, bool select )
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

			var document = ( (DocumentWindow)window ).Document;
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
				return FindWindow<WorkspaceWindow>( w => w.Document == docWindow.Document );
			else
				return null;
		}

		public IEnumerable<DocumentWindow> FindWindowsRecursive( DocumentInstance doc )
		{
			return GetDockWindowsRecursive().OfType<DocumentWindow>().Where( w => w.Document == doc );
		}

		protected override void OnDockWindowCloseRequest( DockWindow window, out bool cancel )
		{
			if( window is WorkspaceWindow workspaceWindow )
			{
				var mainWindow = workspaceWindow.GetMainWindow();
				cancel = editorForm.ShowDialogAndSaveDocument( mainWindow ) == null;
			}
			else
			{
				base.OnDockWindowCloseRequest( window, out cancel );
			}
		}

		protected override void OnDockWindowSaving( DockWindow window, DockPageSavingEventArgs e )
		{
			//TODO: make BaseWindowConfig, use fabric method for WindowConfig creation and move this logic to base class.
			if( window is IDocumentWindow docWindow )
				WindowConfig.FromDocumentWindow( docWindow ).Save( e.XmlWriter );
			window.OnSaving( e.XmlWriter );
		}

		protected override void OnDockWindowLoading( DockWindow window, DockPageLoadingEventArgs e )
		{
			if( window == null )
			{
				var config = new WindowConfig();
				config.Load( e.XmlReader );

				if( !string.IsNullOrEmpty( config.RealFileName ) || !string.IsNullOrEmpty( config.SpecialMode ) )
				{
					var document = EditorAPI.CreateDocument( config.RealFileName, config.SpecialMode );
					if( document == null )
					{
						//some problems with document
						e.Page = null;
						return;
					}

					window = EditorAPI.CreateWindow( document );
					AddDockWindowInternal( window, false, e.Page );
				}
				else
				{
					//!!!!только такие? hardcoded?
					if( config.Type == typeof( StoreDocumentWindow ) || config.Type == typeof( StartPageWindow ) || config.Type == typeof( PackagesWindow ) )
					{
						var document = new DocumentInstance( "", null, "" );
						EditorAPI.Documents.Add( document );

						var documentWindow = (DocumentWindow)config.Type.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null );
						window = documentWindow;
						documentWindow.InitDocumentWindow( document, null, false, null );

						AddDockWindowInternal( window, false, e.Page );
					}
				}

			}
			else
			{
				//ok, window already created
			}

			window?.OnLoading( e.XmlReader );
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
