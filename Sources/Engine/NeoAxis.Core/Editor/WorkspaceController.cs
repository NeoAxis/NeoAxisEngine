// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Workspace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Xml;

namespace NeoAxis.Editor
{
	public abstract class WorkspaceController : IDisposable
	{
		protected bool disposing;
		protected Control ownerControl;
		protected EditorForm editorForm;
		protected KryptonDockingManager dockingManager;
		protected KryptonDockableWorkspace dockableWorkspaceControl;
		protected KryptonDockingControl dockingControl;

		protected Dictionary<KryptonPage, DockWindow> dockWindows = new Dictionary<KryptonPage, DockWindow>();

		//public event EventHandler<DockWindowEventArgs> DockWindowAdded;
		//public event EventHandler<DockWindowEventArgs> DockWindowRemoved;
		//public event EventHandler<DockWindowEventArgs> DockWindowVisibleChanged;


		internal virtual DockWindow GetSelectedDockWindow()
		{
			return null;
		}

		public virtual void SelectDockWindow( DockWindow window )
		{
			if( window == null )
				throw new ArgumentNullException( nameof( window ) );

			//show if hided
			if( !window.Visible )
				EditorForm.Instance.WorkspaceController.SetDockWindowVisibility( window, true );

			//slide out auto hidden page
			if( dockingControl != null )
			{
				var autoHiddenGroup = dockingManager.FindPageElement( window.KryptonPage ) as KryptonDockingAutoHiddenGroup;
				if( autoHiddenGroup != null )
				{
					foreach( var element in dockingControl )
					{
						var dockingEdge = element as KryptonDockingEdge;
						if( dockingEdge != null )
						{
							//find KryptonDockingEdgeAutoHidden
							KryptonDockingEdgeAutoHidden edgeAutoHidden = null;
							foreach( var e in dockingEdge )
							{
								if( e is KryptonDockingEdgeAutoHidden )
								{
									edgeAutoHidden = (KryptonDockingEdgeAutoHidden)e;
									break;
								}
							}

							if( edgeAutoHidden != null )
								edgeAutoHidden.SlidePageOut( window.KryptonPage, true );
						}
					}
				}
			}

			//select page
			var space = dockingManager.FindPageElement( window.KryptonPage ) as KryptonDockingSpace;
			if( space != null )
			{
				space.SelectPage( window.KryptonPage.UniqueName );

				//focus window
				var cell = space.CellForPage( window.KryptonPage.UniqueName );
				cell?.Focus();
			}
		}

		public ICollection<DockWindow> GetDockWindows()
		{
			return dockWindows.Values;
		}

		public DockWindow FindWindow( Type windowClass )
		{
			return GetDockWindows().FirstOrDefault( w => windowClass.IsAssignableFrom( w.GetType() ) );
		}

		public T FindWindow<T>() where T : DockWindow
		{
			return GetDockWindows().OfType<T>().FirstOrDefault();
		}

		public T FindWindow<T>( Func<T, bool> predicate )
		{
			return GetDockWindows().OfType<T>().FirstOrDefault( predicate );
		}

		public IDocumentWindow FindWindow( DocumentInstance doc )
		{
			return FindWindow<IDocumentWindow>( w => w.Document == doc );
		}

		DockWindow GetDockWindow( KryptonPage page )
		{
			if( page == null )
				return null;
			dockWindows.TryGetValue( page, out DockWindow result );
			return result;
		}

		public void ShowDockWindow( Type windowClass )
		{
			var w = FindWindow( windowClass );
			w?.KryptonPage?.Show();
		}
		//public void ShowDockWindow<T>() where T : DockWindow
		//{
		//	var w = FindWindow<T>();
		//	if( w != null && w.KryptonPage != null )
		//		w.KryptonPage.Show();
		//}

		public WorkspaceController( Control ownerControl, EditorForm editorForm )
		{
			this.ownerControl = ownerControl;
			this.editorForm = editorForm;

			this.dockableWorkspaceControl = ownerControl.Controls.OfType<KryptonDockableWorkspace>().First();

			dockingManager = new KryptonDockingManager();

			var dockingWorkspace = new KryptonDockingWorkspace( "DockingWorkspace", "Filler", dockableWorkspaceControl );
			dockingManager.Add( dockingWorkspace );

			if( this is WorkspaceControllerForForm )
			{
				dockingControl = new KryptonDockingControl( "DockingControl", ownerControl, dockingWorkspace );
				dockingManager.Add( dockingControl );
			}

			var floatingElement = new KryptonDockingFloating( "DockingFloating", editorForm );
			dockingManager.Add( floatingElement );

			dockingManager.PageLoading += DockingManager_PageLoading;
			dockingManager.PageSaving += DockingManager_PageSaving;
			dockingManager.RecreateLoadingPage += DockingManager_RecreateLoadingPage;

			dockingManager.OrphanedPages += DockingManager_OrphanedPages;
			dockingManager.PageCloseRequest += DockingManager_PageCloseRequest;

			dockableWorkspaceControl.WorkspaceCellAdding += DockableWorkspace_WorkspaceCellAdding;
			dockableWorkspaceControl.WorkspaceCellRemoved += DockableWorkspace_WorkspaceCellRemoved;
			dockableWorkspaceControl.CellPageInserting += DockableWorkspaceControl_CellPageInserting;
		}

		public virtual void Dispose()
		{
			disposing = true;

			dockingManager.PageLoading -= DockingManager_PageLoading;
			dockingManager.PageSaving -= DockingManager_PageSaving;
			dockingManager.RecreateLoadingPage -= DockingManager_RecreateLoadingPage;

			dockingManager.PageCloseRequest -= DockingManager_PageCloseRequest;
			dockingManager.OrphanedPages -= DockingManager_OrphanedPages;

			dockableWorkspaceControl.WorkspaceCellAdding -= DockableWorkspace_WorkspaceCellAdding;
			dockableWorkspaceControl.WorkspaceCellRemoved -= DockableWorkspace_WorkspaceCellRemoved;
			dockableWorkspaceControl.CellPageInserting -= DockableWorkspaceControl_CellPageInserting;

			// remove all managed windows. not only attached.
			foreach( var window in dockWindows.Values.ToList() )
				RemoveDockWindow( window, true );
		}

		public KryptonDockingDockspace AddToDockspace( DockWindow[] windows, DockingEdge edge )
		{
			var pages = new KryptonPage[ windows.Length ];
			for( int i = 0; i < windows.Length; i++ )
			{
				AddDockWindowInternal( windows[ i ], true );
				pages[ i ] = windows[ i ].KryptonPage;
			}

			return dockingManager.AddDockspace( "DockingControl", edge, pages );
		}

		[Browsable( false )]
		public KryptonDockingManager DockingManager
		{
			get { return dockingManager; }
		}

		public KryptonDockingDockspace AddToDockspaceStack( DockWindow[] windows, DockingEdge edge )
		{
			var pages = new KryptonPage[ windows.Length ][];
			for( int i = 0; i < windows.Length; i++ )
			{
				AddDockWindowInternal( windows[ i ], true );
				pages[ i ] = new KryptonPage[] { windows[ i ].KryptonPage };
			}

			var dockspace = dockingManager.AddDockspace( "DockingControl", edge, pages[ 0 ], pages );
			return dockspace;
		}

		//TODO: remove "bool select"
		KryptonDockingFloatspace AddToFloatspace( DockWindow window, bool select, Point location, Size clientSize )
		{
			AddDockWindowInternal( window, true );

			var floatingWindow = dockingManager.AddFloatingWindow( "DockingFloating", new KryptonPage[] { window.KryptonPage }, location, clientSize );

			//TEST: no need selection
			//if( select )
			//	SelectDockWindow( window );

			return floatingWindow.FloatspaceElement;
		}

		//TODO: remove "bool select"
		KryptonDockingFloatspace AddToFloatspace( DockWindow window, bool select )
		{
			//!!!!save settings for each window
			window.CalculateBigSizeForFloatingWindowDependingScreenSize( out Point position, out Size size );
			return AddToFloatspace( window, select, position, size );
		}

		KryptonDockingWorkspace AddToWorkspace( DockWindow window, bool select )
		{
			AddDockWindowInternal( window, false );

			var workspace = dockingManager.AddToWorkspace( "DockingWorkspace", new KryptonPage[] { window.KryptonPage } );

			// call PerformLayout() to workspace.DockableWorkspaceControl.ActiveCell update !
			workspace.DockableWorkspaceControl.PerformLayout();

			if( select )
				SelectDockWindow( window );

			return workspace;
		}

		KryptonDockingWorkspace InsertToWorkspace( DockWindow window, bool select, int index )
		{
			AddDockWindowInternal( window, false );

			var workspace = dockingManager.ResolvePath( "DockingWorkspace" ) as KryptonDockingWorkspace;

			workspace.CellInsert( workspace.DockableWorkspaceControl.ActiveCell, index, window.KryptonPage );

			// call PerformLayout() to workspace.DockableWorkspaceControl.ActiveCell update !
			//workspace.DockableWorkspaceControl.PerformLayout();

			if( select )
				SelectDockWindow( window );

			return workspace;
		}


		public virtual void AddDockWindow( DockWindow window, bool floatingWindow, bool select )
		{
			if( floatingWindow )
				AddToFloatspace( window, select );
			else
				AddToWorkspace( window, select );
		}

		protected void AddDockWindowInternal( DockWindow window, bool allowDocking, KryptonPage existPage = null )
		{
			CreatePage( window, true, existPage );

			dockWindows.Add( window.KryptonPage, window );
			window.KryptonPage.Disposed += KryptonPage_Disposed;
			window.KryptonPage.VisibleChanged += KryptonPage_VisibleChanged;

			if( !allowDocking )
				window.KryptonPage.ClearFlags( KryptonPageFlags.DockingAllowAutoHidden | KryptonPageFlags.DockingAllowDocked );

			OnDockWindowAdded( window );
		}

		protected virtual void OnDockWindowAdded( DockWindow window )
		{
			//DockWindowAdded?.Invoke(this, new DockWindowEventArgs(window));
		}

		public void ReplaceDockWindow( DockWindow oldWindow, DockWindow newWindow, bool disposeOldWindow, bool select )
		{
			if( IsWindowFloating( oldWindow ) )
			{
				var space = (KryptonDockingFloatspace)dockingManager.FindPageElement( oldWindow.KryptonPage );
				var floatingWindow = space.GetParentType( typeof( KryptonDockingFloatingWindow ) ) as KryptonDockingFloatingWindow;

				var pos = floatingWindow.FloatingWindow.Location;
				var size = floatingWindow.FloatingWindow.ClientSize;
				RemoveDockWindow( oldWindow, disposeOldWindow );
				AddToFloatspace( newWindow, select, pos, size );
			}
			else
			{
				int index = GetDockWindowIndex( oldWindow );
				if( index == -1 )
					Log.Warning( $"Window {oldWindow} not found." );
				else
				{
					RemoveDockWindow( oldWindow, disposeOldWindow );
					InsertToWorkspace( newWindow, select, index );
				}
			}
		}

		int GetDockWindowIndex( DockWindow window )
		{
			var cell = GetWorkspaceCell( window );
			if( cell != null )
				return cell.Pages.IndexOf( window.KryptonPage );
			else
				return -1;
		}

		bool IsWindowFloating( DockWindow window )
		{
			var space = dockingManager.FindPageElement( window.KryptonPage );
			return space is KryptonDockingFloatspace;
		}

		public virtual void RemoveDockWindow( DockWindow window, bool dispose )
		{
			RemoveDockWindow( window.KryptonPage, dispose );
		}

		void RemoveDockWindow( KryptonPage page, bool dispose )
		{
			page.Disposed -= KryptonPage_Disposed;
			page.VisibleChanged -= KryptonPage_VisibleChanged;

			Debug.Assert( dockWindows.ContainsKey( page ) );
			dockingManager.RemovePage( page, dispose );

			if( !dockWindows.ContainsKey( page ) )
				return;

			var removedWindow = dockWindows[ page ];
			dockWindows.Remove( page );
			OnDockWindowRemoved( removedWindow );
		}

		public void CloseDockWindow( DockWindow window )
		{
			OnDockWindowCloseRequest( window, out bool cancel );
			if( !cancel )
				RemoveDockWindow( window, true );
		}

		protected virtual void OnDockWindowRemoved( DockWindow window )
		{
			//DockWindowRemoved?.Invoke(this, new DockWindowEventArgs(window));
		}

		public void LoadLayoutFromFile( string filename )
		{
			dockingManager.LoadConfigFromFile( filename );
		}

		public void SaveLayoutToFile( string filename )
		{
			dockingManager.SaveConfigToFile( filename );
		}

		public void LoadLayoutFromString( string text )
		{
			using( var stream = new MemoryStream( System.Text.Encoding.Unicode.GetBytes( text ) ) )
				dockingManager.LoadConfigFromStream( stream );
		}

		public string SaveLayoutToString()
		{
			using( var stream = new MemoryStream() )
			{
				dockingManager.SaveConfigToStream( stream, System.Text.Encoding.Unicode, Formatting.None );
				return System.Text.Encoding.Unicode.GetString( stream.ToArray() );
			}
		}

		protected virtual KryptonPage CreatePage( DockWindow window, bool createCloseButton, KryptonPage existPage = null )
		{
			var uniqueName = window.IsSystemWindow ? window.GetType().Name : string.Empty;

			// unique name only for DockWindow.
			if( window is DocumentWindow )
				Debug.Assert( string.IsNullOrEmpty( uniqueName ) );

			KryptonPage page = existPage ?? CreatePage( uniqueName, createCloseButton );
			page.associatedDockWindow = window;

			window.Dock = DockStyle.Fill;

			var size = window.DefaultAutoHiddenSlideSize;
			page.AutoHiddenSlideSize = new Size( size.X, size.Y );

			page.Controls.Add( window );
			return page;
		}

		protected KryptonPage CreatePage( string uniqueName, bool createCloseButton )
		{
			KryptonPage page = new KryptonPage( "No data", null, uniqueName );
			page.TextTitle = "description";
			page.TextDescription = "description";

			if( createCloseButton )
			{
				var bsa = new ButtonSpecAny();
				bsa.Tag = page;
				bsa.Type = PaletteButtonSpecStyle.Close;
				bsa.Click += ( s, e ) =>
				{
					var closingPage = (KryptonPage)( (ButtonSpecAny)s ).Tag;
					//!!!!TEST:
					dockingManager.CloseRequest( new string[] { closingPage.UniqueName } );
					//dockableWorkspaceControl.ClosePage( closingPage );
				};
				page.ButtonSpecs.Add( bsa );
			}

			return page;
		}

		protected KryptonWorkspaceCell GetWorkspaceCell( DockWindow window )
		{
			var space = dockingManager.FindPageElement( window.KryptonPage ) as KryptonDockingSpace;
			if( space != null )
				return space.CellForPage( window.KryptonPage.UniqueName );
			else
				return null;
		}

		protected virtual void OnDockWindowCloseRequest( DockWindow window, out bool cancel )
		{
			if( window is DocumentWindow )
				cancel = editorForm.ShowDialogAndSaveDocument( window );
			else
				cancel = false; // for non document windows.
		}

		protected virtual void OnDockWindowSaving( DockWindow window, DockPageSavingEventArgs e )
		{
		}

		protected virtual void OnDockWindowLoading( DockWindow window, DockPageLoadingEventArgs e )
		{
		}

		private void KryptonPage_Disposed( object sender, EventArgs e )
		{
			// editor shutdown or crush ?
			if( !ownerControl.IsHandleCreated )
				return;

			// если удаляется воркспейс, игнорируем логику удаления его дочерних окон.
			if( ownerControl.Disposing || ownerControl.IsDisposed )
				return;

			var page = (KryptonPage)sender;

			//bool needUnlockEditorForm = false;
			////try
			////{
			////	var dockWindow = page.GetDockWindow();
			////	if( dockWindow != null && IsWindowFloating( dockWindow ) )
			////{
			//WinFormsUtility.LockFormUpdate( EditorForm.Instance );
			//needUnlockEditorForm = true;
			////}
			////}
			////catch { }

			try
			{
				RemoveDockWindow( page, true );
			}
			finally
			{
				//if( needUnlockEditorForm )
				//	WinFormsUtility.LockFormUpdate( null );
			}
		}

		private void KryptonPage_VisibleChanged( object sender, EventArgs e )
		{
			if( ownerControl.Disposing )
				return;

			var page = (KryptonPage)sender;

			if( page.LastVisibleSet != page.Visible ) // what the LastVisibleSet ?
				return;

			//DockWindowVisibleChanged?.Invoke( this, new DockWindowEventArgs( page.GetDockWindow() ) );
		}

		private void DockingManager_RecreateLoadingPage( object sender, RecreateLoadingPageEventArgs e )
		{
			e.Page = CreatePage( e.UniqueName, true );
		}

		private void DockingManager_PageSaving( object sender, DockPageSavingEventArgs e )
		{
			var window = e.Page.GetDockWindow();
			if( window == null )
				return;

			OnDockWindowSaving( window, e );
		}

		private void DockingManager_PageLoading( object sender, DockPageLoadingEventArgs e )
		{
			if( e.Page is KryptonStorePage )
				return;

			if( e.XmlReader.Name != "CPD" )
				throw new ArgumentException( "Expected 'CPD' element was not found" );

			var window = e.Page.GetDockWindow();
			OnDockWindowLoading( window, e );
		}

		private void DockingManager_OrphanedPages( object sender, PagesEventArgs e )
		{
			//hide pages. by default it will disposed.
			//foreach (var p in e.Pages)
			//	p.Hide();

			e.Pages.Clear();
		}

		private void DockingManager_PageCloseRequest( object sender, CloseRequestEventArgs e )
		{
			var page = dockingManager.PageForUniqueName( e.UniqueName );
			var window = page.GetDockWindow();
			if( window != null )
			{
				OnDockWindowCloseRequest( window, out bool cancel );
				if( cancel )
					e.CloseRequest = DockingCloseRequest.None;
				else
					e.CloseRequest = window.HideOnRemoving ? DockingCloseRequest.HidePage : DockingCloseRequest.RemovePageAndDispose;
			}
		}

		// for "Close" context menu for Krypton Navigator
		private void KryptonCell_CloseAction( object sender, CloseActionEventArgs e )
		{
			var window = e.Item.GetDockWindow();
			if( window != null )
			{
				OnDockWindowCloseRequest( window, out bool cancel );
				if( cancel )
					e.Action = CloseButtonAction.None;
				else
					e.Action = window.HideOnRemoving ? CloseButtonAction.HidePage : CloseButtonAction.RemovePageAndDispose;
			}
		}

		private void KryptonCell_ShowContextMenu( object sender, ShowContextMenuArgs e )
		{
			var window = e.Item.GetDockWindow();
			if( window != null )
			{
				var menuItems = new KryptonContextMenuItems();
				window.OnShowTitleContextMenu( menuItems );

				if( menuItems.Items.Count != 0 )
				{
					//should we create e.KryptonContextMenu if null ?
					e.KryptonContextMenu.Items.Insert( 0, menuItems );
					e.KryptonContextMenu.Items.Insert( 1, new KryptonContextMenuSeparator() );
				}
			}
		}

		private void DockableWorkspace_WorkspaceCellAdding( object sender, WorkspaceCellEventArgs e )
		{
			e.Cell.CloseAction += KryptonCell_CloseAction;
			e.Cell.ShowContextMenu += KryptonCell_ShowContextMenu;
		}

		private void DockableWorkspace_WorkspaceCellRemoved( object sender, WorkspaceCellEventArgs e )
		{
			e.Cell.ShowContextMenu -= KryptonCell_ShowContextMenu;
			e.Cell.CloseAction -= KryptonCell_CloseAction;
		}

		private void DockableWorkspaceControl_CellPageInserting( object sender, KryptonPageEventArgs e )
		{
			var window = GetDockWindow( e.Item ) as DocumentWindow;
			if( window == null )
				return;

			// select workspace window (tab in navigator), if not selected, after page inserting.

			var formWorkspaceController = editorForm.WorkspaceController;
			if( formWorkspaceController.SelectedDocumentWindow == window )
				return;

			var workspaceWindow = formWorkspaceController.FindWorkspaceWindow( window );
			if( workspaceWindow != null )
			{
				var workspaceWindowForSelectedWindow = formWorkspaceController.FindWorkspaceWindow( formWorkspaceController.SelectedDocumentWindow );
				if( workspaceWindow != workspaceWindowForSelectedWindow )
					editorForm.WorkspaceController.SelectDockWindow( workspaceWindow );
			}
		}

		// Auto Hidden windows helper methods:

		internal void RepaintAutoHiddenWindow( DockWindow window )
		{
			var ahg = dockingManager.FindPageElement( window.KryptonPage ) as KryptonDockingAutoHiddenGroup;
			if( ahg != null )
				ahg.AutoHiddenGroupControl.PerformNeedPaint( true );
		}

		internal void HideAutoHiddenWindowWithoutAnimation( DockWindow window )
		{
			var slidePanel = ownerControl.Controls.OfType<KryptonAutoHiddenSlidePanel>()
							.FirstOrDefault( sp => sp.Page == window.KryptonPage );
			if( slidePanel != null )
				slidePanel.HideUniqueName();
		}

		internal void BlockAutoHideForAutoHiddenWindow( DockWindow window, bool value )
		{
			var slidePanel = ownerControl.Controls.OfType<KryptonAutoHiddenSlidePanel>()
							.FirstOrDefault( sp => sp.Page == window.KryptonPage );
			if( slidePanel != null ) // is not auto hidden if slidePanel == null
				slidePanel.BlockAutoHide = value;
		}

		internal void BlockAutoHideAndDoAction( Control control, Action action )
		{
			var window = control is DockWindow ?
				(DockWindow)control : FindParentDockWindow( control );

			BlockAutoHideForAutoHiddenWindow( window, true );
			try { action(); }
			finally { BlockAutoHideForAutoHiddenWindow( window, false ); }
		}

		static DockWindow FindParentDockWindow( Control control )
		{
			Control parent = control;
			while( ( parent = parent.Parent ) != null )
			{
				if( parent is KryptonPage page )
					return page.GetDockWindow();
			}
			return null;
		}
	}
}
