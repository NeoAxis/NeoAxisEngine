// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using NeoAxis;
using System.Reflection;

namespace NeoAxis.Editor
{
	public partial class NewObjectSettingsCell : NewObjectCell
	{
		NewObjectSettings settings;

		public NewObjectSettingsCell()
		{
			InitializeComponent();
		}

		public bool Init( Type settingsClass, NewObjectWindow window )// bool fileCreation )
		{
			settings = (NewObjectSettings)settingsClass.InvokeMember( "",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
				null, null, null );

			if( !settings.Init( window ) )// fileCreation );
				return false;

			hierarchicalContainer1.OverrideGroupDisplayName += HierarchicalContainer1_OverrideGroupDisplayName;
			hierarchicalContainer1.OverridePropertyDisplayName += HierarchicalContainer1_OverridePropertyDisplayName;
			hierarchicalContainer1.OverrideMemberDescription += HierarchicalContainer1_OverrideMemberDescription;

			hierarchicalContainer1.SetData( null, new object[] { settings } );

			return true;
		}

		public override bool ReadyToCreate( out string reason )
		{
			if( !base.ReadyToCreate( out reason ) )
				return false;

			if( !settings.ReadyToCreate( out reason ) )
				return false;

			return true;
		}

		public override bool ObjectCreation( ObjectCreationContext context )
		{
			if( !base.ObjectCreation( context ) )
				return false;

			if( !settings.Creation( context ) )
				return false;

			return true;
		}

		private void HierarchicalContainer1_OverrideGroupDisplayName( HierarchicalContainer sender, HCItemGroup group, ref string displayName )
		{
			displayName = EditorLocalization.Translate( "Object.Group", displayName );
		}

		private void HierarchicalContainer1_OverridePropertyDisplayName( HierarchicalContainer sender, HCItemProperty property, ref string displayName )
		{
			displayName = EditorLocalization.Translate( "Object.Property", displayName );
		}

		private void HierarchicalContainer1_OverrideMemberDescription( HierarchicalContainer sender, HCItemMember member, ref string description )
		{
			description = EditorLocalization.Translate( "Object.Description", description );
		}
	}
}
