#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Docking;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Workspace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace NeoAxis.Editor
{
	///// <summary>
	///// Workspace controller for WorkspaceWindow
	///// In WorkspaceControllerForWindow can only be DocumentWindow.
	///// </summary>
	class WorkspaceControllerForWindow : WorkspaceController
	{
		// class for window settings serialization/deserialization
		internal class WindowConfig
		{
			internal string ObjectPath { get; set; }
			internal bool OpenAsSettings { get; set; }

			internal static WindowConfig FromComponent( Component comp )
			{
				return new WindowConfig()
				{
					OpenAsSettings = EditorAPI.GetDocumentWindowClass( comp ) == typeof( ObjectSettingsWindow ), //TODO: refactor
					ObjectPath = comp.GetPathFromRoot()
				};
			}

			public static WindowConfig FromDocumentWindow( DocumentWindow window )
			{
				var obj = (Component)window.ObjectOfWindow;
				return new WindowConfig()
				{
					OpenAsSettings = window.OpenAsSettings,
					ObjectPath = obj.GetPathFromRoot()
				};
			}

			internal void Save( XmlWriter xmlWriter )
			{
				xmlWriter.WriteAttributeString( "ObjPath", ObjectPath );
				if( OpenAsSettings )
					xmlWriter.WriteAttributeString( "OpenAsSettings", OpenAsSettings.ToString() );
			}

			internal void Load( XmlReader xmlReader )
			{
				ObjectPath = xmlReader.GetAttribute( "ObjPath" );
				bool.TryParse( xmlReader.GetAttribute( "OpenAsSettings" ), out bool result );
				OpenAsSettings = result;
			}
		}

		bool relocationProcess = false;

		protected WorkspaceWindow WorkspaceWindow
		{
			get { return (WorkspaceWindow)ownerControl; }
		}

		public DocumentWindow GetMainWindow()
		{
			return GetDockWindows().OfType<DocumentWindow>().FirstOrDefault( wnd => wnd.IsMainWindowInWorkspace );
		}

		internal override DockWindow GetSelectedDockWindow()
		{
			//var space = WorkspaceWindow.ActiveControl as KryptonSpace;
			//if( space == null )
			//	return null;

			var activePage = dockableWorkspaceControl.ActivePage;
			//var activePage = space.ActivePage;
			if( activePage == null )
				return null;

			var window = activePage.GetDockWindow();
			return window;
		}

		public WorkspaceControllerForWindow( Control ownerControl, EditorForm ownerForm )
			: base( ownerControl, ownerForm )
		{
		}

		public override void AddDockWindow( DockWindow window, bool floatingWindow, bool select )
		{
			Debug.Assert( window is DocumentWindow );

			var documentWindow = (DocumentWindow)window;

			if( documentWindow.Document == null )
				throw new ArgumentNullException( nameof( documentWindow.Document ) );

			if( documentWindow.Document != WorkspaceWindow.Document )
				throw new ArgumentException( nameof( documentWindow.Document ) + " should be equal to the " + nameof( WorkspaceWindow.Document ) );

			base.AddDockWindow( window, floatingWindow, select );
		}

		protected override void OnDockWindowAdded( DockWindow window )
		{
			base.OnDockWindowAdded( window );

			var documentWindow = (DocumentWindow)window;
			documentWindow.IsWindowInWorkspace = true;
		}

		protected override void OnDockWindowRemoved( DockWindow window )
		{
			Debug.Assert( window is DocumentWindow );
			Debug.Assert( ( (DocumentWindow)window ).IsWindowInWorkspace );

			var documentWindow = (DocumentWindow)window;
			documentWindow.IsWindowInWorkspace = false;

			if( !relocationProcess & !disposing )
			{
				var ownerFormController = editorForm.WorkspaceController;

				// если закрывается "главное" окно, то закрываем все связанные окна в воркспейсе и сам воркспейс.
				if( documentWindow.IsMainWindowInWorkspace )
				{
					// при закрытия воркспейса его дочерние окна будут удалены.
					ownerFormController.RemoveDockWindow( WorkspaceWindow, true );
				}
				else
				{
					// если после закрытия остаётся одно окно, то закрываем воркспейс.
					// а оставшееся окно перемещаем в корневой воркспейс.
					bool needWindowsRelocation = dockWindows.Count() == 1;
					if( needWindowsRelocation )
					{
						relocationProcess = true;
						var relocationWindow = dockWindows.First().Value;
						this.RemoveDockWindow( relocationWindow, false );

						ownerFormController.ReplaceDockWindow( WorkspaceWindow, relocationWindow, true, true );

						relocationProcess = false;
					}
				}
			}

			base.OnDockWindowRemoved( window );
		}

		protected override void OnDockWindowSaving( DockWindow window, DockPageSavingEventArgs e )
		{
			//TODO: make BaseWindowConfig, use fabric method for WindowConfig creation and move this logic to base class.
			if( window is DocumentWindow docWindow )
				WindowConfig.FromDocumentWindow( docWindow ).Save( e.XmlWriter );
			//window.OnSaving( e.XmlWriter );
		}

		protected override void OnDockWindowLoading( DockWindow window, DockPageLoadingEventArgs e )
		{
			if( window == null )
			{
				var config = new WindowConfig();
				config.Load( e.XmlReader );

				window = EditorAPI.CreateWindowImpl( WorkspaceWindow.Document, config.ObjectPath, config.OpenAsSettings );
				AddDockWindowInternal( window, false, e.Page );
			}
			else
			{
				//window already created
			}

			//window.OnLoading( e.XmlReader );
		}

		protected override void OnDockWindowCloseRequest( DockWindow window, out bool cancel )
		{
			//disable closing 'Root object' on second level
			var documentWindow = window as DocumentWindow;
			if( documentWindow != null && documentWindow.ObjectOfWindow == documentWindow.Document.ResultObject && !documentWindow.OpenAsSettings )
			{
				cancel = true;
				return;
			}

			base.OnDockWindowCloseRequest( window, out cancel );
		}

	}
}

#endif