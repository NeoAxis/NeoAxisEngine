#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Internal.ComponentFactory.Krypton.Toolkit;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class WorkspaceWindow : DockWindow, IDocumentWindow
	{
		WorkspaceControllerForWindow workspaceController;

		//

		DocumentInstance document;

		public event DocumentWindowSelectedObjectsChangedDelegate SelectedObjectsChanged;

		public DocumentInstance Document2
		{
			get { return document; }
			set { document = value; }
		}

		public bool IsDocumentSaved()
		{
			return !Document.Modified;
		}

		public WorkspaceController WorkspaceController
		{
			get { return workspaceController; }
		}

		public IDocumentInstance Document
		{
			get { return document; }
			set { document = (DocumentInstance)value; }
		}

		public object[] SelectedObjects
		{
			get { return null; }
		}

		public ESet<object> SelectedObjectsSet
		{
			get { return null; }
		}

		public object ObjectOfWindow
		{
			get { return null; }
		}

		public bool OpenAsSettings
		{
			get { return false; }
		}

		public Dictionary<string, object> WindowTypeSpecificOptions
		{
			get { return null; }
		}

		public bool IsWindowInWorkspace
		{
			get { return true; } //!!!!?
			set { }
		}

		public DocumentWindow GetMainWindow()
		{
			return workspaceController.GetMainWindow();
		}

		public DockWindow GetSelectedDockWindow()
		{
			return workspaceController.GetSelectedDockWindow();
		}

		public WorkspaceWindow( EditorForm ownerForm )
		{
			InitializeComponent();
			workspaceController = new WorkspaceControllerForWindow( this, ownerForm );

			var strings2 = workspaceController.DockingManager.Strings;
			strings2.TextTabbedDocument = EditorLocalization2.Translate( "Docking", strings2.TextTabbedDocument );
			strings2.TextAutoHide = EditorLocalization2.Translate( "Docking", strings2.TextAutoHide );
			strings2.TextClose = EditorLocalization2.Translate( "Docking", strings2.TextClose );
			strings2.TextCloseAllButThis = EditorLocalization2.Translate( "Docking", strings2.TextCloseAllButThis );
			strings2.TextDock = EditorLocalization2.Translate( "Docking", strings2.TextDock );
			strings2.TextFloat = EditorLocalization2.Translate( "Docking", strings2.TextFloat );
			strings2.TextHide = EditorLocalization2.Translate( "Docking", strings2.TextHide );
			strings2.TextWindowLocation = EditorLocalization2.Translate( "Docking", strings2.TextWindowLocation );
		}

		public void Init( DocumentInstance document )
		{
			if( document == null )
				throw new ArgumentNullException( nameof( document ) );

			Document2 = document;
			UpdateWindowTitle();
		}

		public void InitFromConfig( DocumentInstance document )
		{
			Init( document );

			var config = document.ResultComponent.EditorDocumentConfiguration;
			workspaceController.LoadLayoutFromString( config );
		}

		protected override string GetResultWindowTitle()
		{
			var title = Path.GetFileName( Document.RealFileName );
			if( Document.Modified )
				title += "*";
			return title;
		}

		protected override void Dispose( bool disposing )
		{
			workspaceController.Dispose();
			base.Dispose( disposing );
		}

		protected internal override void OnShowTitleContextMenu( KryptonContextMenuItems items )
		{
			Document2.OnShowTitleContextMenu( this, items );
		}

		public override ObjectsInFocus GetObjectsInFocus()
		{
			var documentWindow = GetSelectedDockWindow() as DocumentWindow;
			if( documentWindow != null )
				return new ObjectsInFocus( documentWindow, documentWindow.SelectedObjects );
			return null;
		}

		public bool SaveDocument()
		{
			return false;
		}

		public bool IsObjectSelected( object obj )
		{
			return false;
		}

		public void SelectObjects( ICollection<object> objects, bool updateForeachDocumentWindowContainers = true, bool updateSettingsWindowSelectObjects = true, bool forceUpdate = false )
		{
		}
	}
}
#endif