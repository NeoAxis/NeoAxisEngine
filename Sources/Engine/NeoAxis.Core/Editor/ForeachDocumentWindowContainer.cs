// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;

namespace NeoAxis.Editor
{
	//!!!!можно сделать более общим
	//!!!!!!или надо ли
	//!!!!name
	/// <summary>
	/// Represents a container of controls which create only for specific document.
	/// </summary>
	public partial class ForeachDocumentWindowContainer : DockWindow
	{
		/// <summary>
		/// Provides a panel data of <see cref="ForeachDocumentWindowContainer"/>.
		/// </summary>
		public class PanelData
		{
			public DocumentWindow documentWindow;
			public UserControl control;
		}
		List<PanelData> panels = new List<PanelData>();
		EDictionary<DocumentWindow, PanelData> panelByDocumentWindow = new EDictionary<DocumentWindow, PanelData>();

		PanelData selectedPanel;

		//

		public ForeachDocumentWindowContainer()
		{
			InitializeComponent();
		}

		public PanelData GetPanel( DocumentWindow documentWindow )
		{
			if( documentWindow != null )
			{
				PanelData panel;
				if( panelByDocumentWindow.TryGetValue( documentWindow, out panel ) )
					return panel;
			}
			return null;
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			UpdatePanels();
		}

		protected virtual void OnSelectedPanelChanged( PanelData oldSelectedPanel ) { }

		[Browsable( false )]
		public PanelData SelectedPanel
		{
			get { return selectedPanel; }
			set
			{
				if( selectedPanel == value )
					return;

				if( selectedPanel != null )
				{
					selectedPanel.control.Visible = false;
					selectedPanel.control.Enabled = false;
				}

				var old = selectedPanel;
				selectedPanel = value;

				if( selectedPanel != null )
				{
					selectedPanel.control.Enabled = true;
					selectedPanel.control.Visible = true;
					//selectedPanel.control.Focus();
				}

				OnSelectedPanelChanged( old );
			}
		}

		protected override void OnDestroy()
		{
			while( panels.Count != 0 )
				RemovePanel( panels[ panels.Count - 1 ] );

			base.OnDestroy();
		}

		protected virtual void OnCreatePanelControl( PanelData panel ) { }

		PanelData CreatePanel( DocumentWindow documentWindow, bool willSelected )
		{
			PanelData panel = new PanelData();
			panel.documentWindow = documentWindow;

			panels.Add( panel );
			panelByDocumentWindow.Add( panel.documentWindow, panel );

			//create control
			OnCreatePanelControl( panel );

			if( !willSelected && panel.control != null )
			{
				panel.control.Visible = false;
				panel.control.Enabled = false;
			}

			return panel;
		}

		void RemovePanel( PanelData panel )
		{
			//!!!!так?
			if( SelectedPanel == panel )
				SelectedPanel = null;

			var control = panel.control;
			control.Parent.Controls.Remove( control );
			control.Dispose();

			panels.Remove( panel );
			panelByDocumentWindow.Remove( panel.documentWindow );
		}

		private void ForeachDocumentContainer_Load( object sender, EventArgs e )
		{
			//to prevent crash designer. happens on ObjectsWindow
			if( !WinFormsUtility.IsDesignerHosted( this ) )
				timer1.Enabled = true;
		}

		public virtual void OnDocumentWindowSelectedObjectsChangedByUser( DocumentWindow documentWindow )
		{
			//!!!!new
			UpdatePanels();
		}

		void UpdatePanels()
		{
			var controller = EditorForm.Instance.WorkspaceController as WorkspaceControllerForForm;
			var documentWindows = controller.GetDockWindowsRecursive();
			var selectedWindow = controller.SelectedDocumentWindow;

			//no update for some windows
			if( selectedWindow != null )
			{
				if( selectedWindow.OpenAsSettings )
					return;
				if( selectedWindow.Document != null && selectedWindow.Document.SpecialMode == "ProjectSettingsUserMode" )
					return;
			}

			//remove old
			foreach( PanelData panel in panels.ToArray() )
			{
				if( !documentWindows.Contains( panel.documentWindow ) )
					RemovePanel( panel );
			}

			//create new
			if( selectedWindow != null && GetPanel( selectedWindow ) == null )
				CreatePanel( selectedWindow, true );

			//activate
			{
				bool setToNull = true;

				if( selectedWindow != null )
				{
					PanelData panel = GetPanel( selectedWindow );
					if( panel != null )
					{
						SelectedPanel = panel;
						setToNull = false;
					}
				}

				if( setToNull )
					SelectedPanel = null;
			}
		}
	}
}
