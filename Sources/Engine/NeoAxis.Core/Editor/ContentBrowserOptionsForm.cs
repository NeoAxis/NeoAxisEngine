// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class ContentBrowserOptionsForm : EngineForm
	{
		public ContentBrowserOptionsForm( ContentBrowser browser )
		{
			Browser = browser;

			InitializeComponent();

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			Text = EditorLocalization.Translate( "ContentBrowser", Text );
			EditorLocalization.TranslateForm( "ContentBrowser", this );

			hierarchicalContainer1.OverridePropertyDisplayName += HierarchicalContainer1_PropertyDisplayNameOverride;
			hierarchicalContainer1.OverridePropertyEnumItem += HierarchicalContainer1_OverridePropertyEnumItem;

			if( EditorLocalization.WideLanguage )
				hierarchicalContainer1.SplitterRatio = 0.5f;

			if( Browser != null )
			{
				object[] objects = new object[ 1 ];
				objects[ 0 ] = Browser.Options;
				hierarchicalContainer1.SetData( null, objects );

				//fix delayed update
				hierarchicalContainer1.UpdateItems();
			}
		}

		[Browsable( false )]
		public ContentBrowser Browser { get; set; }

		private void ContentBrowserOptionsForm_Load( object sender, EventArgs e )
		{
			//if( Browser != null )
			//{
			//	object[] objects = new object[ 1 ];
			//	objects[ 0 ] = Browser.Options;
			//	hierarchicalContainer1.SetData( null, objects );
			//}
		}

		private void buttonClose_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void HierarchicalContainer1_PropertyDisplayNameOverride( HierarchicalContainer sender, HCItemProperty property, ref string displayName )
		{
			displayName = EditorLocalization.Translate( "ContentBrowser.Options", displayName );
		}

		private void HierarchicalContainer1_OverridePropertyEnumItem( HierarchicalContainer sender, HCItemEnumDropDown property, ref string displayName, ref string description )
		{
			displayName = EditorLocalization.Translate( "ContentBrowser.Options", displayName );
			description = EditorLocalization.Translate( "ContentBrowser.Options", description );
		}
	}
}
