// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	partial class WorkspaceWindow : DockWindow, IDocumentWindow
	{
		WorkspaceControllerForWindow workspaceController;

		//

		public DocumentInstance Document { get; private set; }

		public bool IsDocumentSaved()
		{
			return !Document.Modified;
		}

		public WorkspaceController WorkspaceController
		{
			get { return workspaceController; }
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
			strings2.TextTabbedDocument = EditorLocalization.Translate( "Docking", strings2.TextTabbedDocument );
			strings2.TextAutoHide = EditorLocalization.Translate( "Docking", strings2.TextAutoHide );
			strings2.TextClose = EditorLocalization.Translate( "Docking", strings2.TextClose );
			strings2.TextCloseAllButThis = EditorLocalization.Translate( "Docking", strings2.TextCloseAllButThis );
			strings2.TextDock = EditorLocalization.Translate( "Docking", strings2.TextDock );
			strings2.TextFloat = EditorLocalization.Translate( "Docking", strings2.TextFloat );
			strings2.TextHide = EditorLocalization.Translate( "Docking", strings2.TextHide );
			strings2.TextWindowLocation = EditorLocalization.Translate( "Docking", strings2.TextWindowLocation );
		}

		public void Init( DocumentInstance document )
		{
			if( document == null )
				throw new ArgumentNullException( nameof( document ) );

			Document = document;
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

			// for debug:
			//var sel = GetSelectedDockWindow();
			//var selTile = sel != null ? GetSelectedDockWindow().WindowTitle : "null";
			//title += " [" + selTile + "]";

			return title;
		}

		protected override void Dispose( bool disposing )
		{
			workspaceController.Dispose();
			base.Dispose( disposing );
		}

		protected internal override void OnShowTitleContextMenu( KryptonContextMenuItems items )
		{
			Document.OnShowTitleContextMenu( this, items );
		}

		public override ObjectsInFocus GetObjectsInFocus()
		{
			var documentWindow = GetSelectedDockWindow() as DocumentWindow;
			if( documentWindow != null )
				return new ObjectsInFocus( documentWindow, documentWindow.SelectedObjects );
			return null;
		}
	}
}