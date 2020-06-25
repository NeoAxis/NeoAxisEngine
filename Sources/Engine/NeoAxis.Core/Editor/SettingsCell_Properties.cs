// Copyright (C) NeoAxis Group Ltd. 8 Copthall, R/oseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class SettingsCell_Properties : SettingsCell
	{
		public SettingsCell_Properties()
		{
			InitializeComponent();

			toolStripButtonEvents.Image = EditorResourcesCache.Events;
			toolStripButtonProperties.Image = EditorResourcesCache.Properties;

			toolStripButtonProperties.Text = EditorLocalization.Translate( "SettingsWindow", toolStripButtonProperties.Text );
			toolStripButtonEvents.Text = EditorLocalization.Translate( "SettingsWindow", toolStripButtonEvents.Text );

			if( EditorAPI.DarkTheme )
				toolStrip1.Renderer = DarkThemeUtility.GetToolbarToolStripRenderer();
		}

		private void SettingsCell_Properties_Load( object sender, EventArgs e )
		{
			timer1.Start();

			if( EditorUtility.IsDesignerHosted( this ) )
				return;

			hierarchicalContainer1.OverrideGroupDisplayName += HierarchicalContainer1_OverrideGroupDisplayName;
			hierarchicalContainer1.OverridePropertyDisplayName += HierarchicalContainer1_OverridePropertyDisplayName;
			hierarchicalContainer1.OverrideMemberDescription += HierarchicalContainer1_OverrideMemberDescription;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		public void UpdateData()
		{
			if( Provider.SelectedObjects != null && Provider.SelectedObjects.Length != 0 )
			{
				var showToolbar = true;
				if( Provider?.DocumentWindow?.Document != null && Provider.DocumentWindow.Document.SpecialMode == "ProjectSettingsUserMode" )
					showToolbar = false;

				hierarchicalContainer1.SetData( Provider.DocumentWindow, Provider.SelectedObjects, false );
				toolStrip1.Visible = true && showToolbar;

				if( !showToolbar )
				{
					if( hierarchicalContainer1.Location != new Point( 0, 0 ) )
						hierarchicalContainer1.Location = new Point( 0, 0 );
				}
				//if( !showToolbar )
				//{
				//	if( hierarchicalContainer1.Dock != DockStyle.Fill )
				//		hierarchicalContainer1.Dock = DockStyle.Fill;
				//}
				//var dock = showToolbar ? DockStyle.None : DockStyle.Fill;
				//if( hierarchicalContainer1.Dock != dock )
				//	hierarchicalContainer1.Dock = dock;
			}
			else
			{
				hierarchicalContainer1.SetData( null, null, false );
				toolStrip1.Visible = false;
			}
		}

		private void toolStripButtonProperties_Click( object sender, EventArgs e )
		{
			hierarchicalContainer1.ReverseGroups = false;
			hierarchicalContainer1.ContentMode = HierarchicalContainer.ContentModeEnum.Properties;
		}

		private void toolStripButtonEvents_Click( object sender, EventArgs e )
		{
			hierarchicalContainer1.ReverseGroups = true;
			hierarchicalContainer1.ContentMode = HierarchicalContainer.ContentModeEnum.Events;
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || EditorUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			toolStripButtonProperties.Checked = hierarchicalContainer1.ContentMode == HierarchicalContainer.ContentModeEnum.Properties;
			toolStripButtonEvents.Checked = hierarchicalContainer1.ContentMode == HierarchicalContainer.ContentModeEnum.Events;
		}

		private void HierarchicalContainer1_OverrideGroupDisplayName( HierarchicalContainer sender, HCItemGroup group, ref string displayName )
		{
			try
			{
				if( Provider.DocumentWindow.Document.SpecialMode == "ProjectSettingsUserMode" )
					displayName = EditorLocalization.Translate( "ProjectSettings.Group", displayName );
				else
					displayName = EditorLocalization.Translate( "Object.Group", displayName );
			}
			catch { }
		}

		private void HierarchicalContainer1_OverridePropertyDisplayName( HierarchicalContainer sender, HCItemProperty property, ref string displayName )
		{
			try
			{
				if( Provider.DocumentWindow.Document.SpecialMode == "ProjectSettingsUserMode" )
					displayName = EditorLocalization.Translate( "ProjectSettings.Property", displayName );
				else
					displayName = EditorLocalization.Translate( "Object.Property", displayName );
			}
			catch { }
		}

		private void HierarchicalContainer1_OverrideMemberDescription( HierarchicalContainer sender, HCItemMember member, ref string description )
		{
			try
			{
				if( Provider.DocumentWindow.Document.SpecialMode == "ProjectSettingsUserMode" )
					description = EditorLocalization.Translate( "ProjectSettings.Description", description );
				else
					description = EditorLocalization.Translate( "Object.Description", description );
			}
			catch { }
		}

		private void SettingsCell_Properties_Resize( object sender, EventArgs e )
		{
			hierarchicalContainer1?.SetBounds( 0, toolStrip1.Bounds.Bottom, Size.Width, Size.Height - toolStrip1.Bounds.Bottom );
		}
	}
}
