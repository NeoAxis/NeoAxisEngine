#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public partial class SettingsCell_Properties : SettingsCell
	{
		public static bool AllowConfigureEvents = true;

		//

		bool propertiesHaveBeenUpdated;

		bool currentEvents;
		bool eventsHaveBeenUpdated;

		//

		public SettingsCell_Properties()
		{
			InitializeComponent();

			toolStripButtonEvents.Image = EditorResourcesCache.Events;
			toolStripButtonProperties.Image = EditorResourcesCache.Properties;

			toolStripButtonProperties.Text = EditorLocalization2.Translate( "SettingsWindow", toolStripButtonProperties.Text );
			toolStripButtonEvents.Text = EditorLocalization2.Translate( "SettingsWindow", toolStripButtonEvents.Text );

			toolStrip1.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();
		}

		private void SettingsCell_Properties_Load( object sender, EventArgs e )
		{
			timer1.Start();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			toolStrip1.Padding = new Padding( (int)EditorAPI2.DPIScale );
			toolStrip1.Size = new Size( 10, (int)( 21 * EditorAPI2.DPIScale + 2 ) );
			toolStripButtonProperties.Size = new Size( (int)( 20 * EditorAPI2.DPIScale ), (int)( 20 * EditorAPI2.DPIScale + 2 ) );
			toolStripButtonEvents.Size = new Size( (int)( 20 * EditorAPI2.DPIScale ), (int)( 20 * EditorAPI2.DPIScale + 2 ) );

			hierarchicalContainerProperties.OverrideGroupDisplayName += HierarchicalContainer1_OverrideGroupDisplayName;
			hierarchicalContainerProperties.OverridePropertyDisplayName += HierarchicalContainer1_OverridePropertyDisplayName;
			hierarchicalContainerProperties.OverrideMemberDescription += HierarchicalContainer1_OverrideMemberDescription;
			hierarchicalContainerEvents.OverrideGroupDisplayName += HierarchicalContainer1_OverrideGroupDisplayName;
			hierarchicalContainerEvents.OverridePropertyDisplayName += HierarchicalContainer1_OverridePropertyDisplayName;
			hierarchicalContainerEvents.OverrideMemberDescription += HierarchicalContainer1_OverrideMemberDescription;

			if( !AllowConfigureEvents )
			{
				toolStrip1.Visible = false;
				toolStripButtonEvents.Enabled = false;
				toolStripButtonProperties.Enabled = false;
			}

			UpdateControlsBounds();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		public void UpdateData()
		{
			if( Provider2.SelectedObjects != null && Provider2.SelectedObjects.Length != 0 )
			{
				var showToolbar = true;
				if( Provider2?.DocumentWindow2?.Document2 != null && Provider2.DocumentWindow2.Document2.SpecialMode == "ProjectSettingsUserMode" )
					showToolbar = false;

				toolStrip1.Visible = showToolbar && AllowConfigureEvents;

				UpdateControlsBounds();

				if( !propertiesHaveBeenUpdated )
				{
					hierarchicalContainerProperties.SetData( Provider2.DocumentWindow2, Provider2.SelectedObjects );
					propertiesHaveBeenUpdated = true;
				}
			}
			else
			{
				toolStrip1.Visible = false;
				hierarchicalContainerProperties.SetData( null, null, false );
				hierarchicalContainerEvents.SetData( null, null, false );
			}

			UpdateControlsChecked();
		}

		private void toolStripButtonProperties_Click( object sender, EventArgs e )
		{
			currentEvents = false;

			hierarchicalContainerProperties.Visible = true;
			hierarchicalContainerEvents.Visible = false;

			UpdateControlsChecked();
		}

		private void toolStripButtonEvents_Click( object sender, EventArgs e )
		{
			currentEvents = true;

			hierarchicalContainerEvents.Visible = true;

			if( !eventsHaveBeenUpdated )
			{
				hierarchicalContainerEvents.SetData( Provider2.DocumentWindow2, Provider2.SelectedObjects );
				eventsHaveBeenUpdated = true;
			}

			hierarchicalContainerProperties.Visible = false;

			UpdateControlsChecked();
		}

		void UpdateControlsChecked()
		{
			toolStripButtonProperties.Checked = !currentEvents;
			toolStripButtonEvents.Checked = currentEvents;
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			UpdateControlsChecked();
		}

		private void HierarchicalContainer1_OverrideGroupDisplayName( HierarchicalContainer sender, HCItemGroup group, ref string displayName )
		{
			try
			{
				if( Provider2.DocumentWindow2.Document2.SpecialMode == "ProjectSettingsUserMode" )
					displayName = EditorLocalization2.Translate( "ProjectSettings.Group", displayName );
				else
					displayName = EditorLocalization2.Translate( "Object.Group", displayName );
			}
			catch { }
		}

		private void HierarchicalContainer1_OverridePropertyDisplayName( HierarchicalContainer sender, HCItemProperty property, ref string displayName )
		{
			try
			{
				if( Provider2.DocumentWindow2.Document2.SpecialMode == "ProjectSettingsUserMode" )
					displayName = EditorLocalization2.Translate( "ProjectSettings.Property", displayName );
				else
					displayName = EditorLocalization2.Translate( "Object.Property", displayName );
			}
			catch { }
		}

		private void HierarchicalContainer1_OverrideMemberDescription( HierarchicalContainer sender, HCItemMember member, ref string description )
		{
			try
			{
				if( Provider2.DocumentWindow2.Document2.SpecialMode == "ProjectSettingsUserMode" )
					description = EditorLocalization2.Translate( "ProjectSettings.Description", description );
				else
					description = EditorLocalization2.Translate( "Object.Description", description );
			}
			catch { }
		}

		void UpdateControlsBounds()
		{
			if( AllowConfigureEvents )
			{
				hierarchicalContainerProperties.SetBounds( 0, toolStrip1.Bounds.Bottom, Size.Width, Size.Height - toolStrip1.Bounds.Bottom );
				hierarchicalContainerEvents.SetBounds( 0, toolStrip1.Bounds.Bottom, Size.Width, Size.Height - toolStrip1.Bounds.Bottom );
			}
			else
			{
				hierarchicalContainerProperties.SetBounds( 0, toolStrip1.Bounds.Top, Size.Width, Size.Height - toolStrip1.Bounds.Top );
				hierarchicalContainerEvents.SetBounds( 0, toolStrip1.Bounds.Top, Size.Width, Size.Height - toolStrip1.Bounds.Top );
			}
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControlsBounds();
		}

		protected override void OnParentFormResizeBegin( EventArgs e )
		{
			base.OnParentFormResizeBegin( e );

			if( !currentEvents && hierarchicalContainerProperties.RootItems.Count > 20 )
				hierarchicalContainerProperties.Visible = false;
			if( currentEvents && hierarchicalContainerEvents.RootItems.Count > 20 )
				hierarchicalContainerEvents.Visible = false;
		}

		protected override void OnParentFormResizeEnd( EventArgs e )
		{
			base.OnParentFormResizeEnd( e );

			if( !currentEvents )
				hierarchicalContainerProperties.Visible = true;
			if( currentEvents )
				hierarchicalContainerEvents.Visible = true;
		}
	}
}

#endif