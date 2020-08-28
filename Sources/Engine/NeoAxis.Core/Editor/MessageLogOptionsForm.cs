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
	public partial class MessageLogOptionsForm : EngineForm
	{
		public MessageLogOptionsForm()
		{
			InitializeComponent();

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			Text = Translate( Text );

			hierarchicalContainer1.OverridePropertyDisplayName += HierarchicalContainer1_PropertyDisplayNameOverride;
			hierarchicalContainer1.OverridePropertyEnumItem += HierarchicalContainer1_OverridePropertyEnumItem;

			if( EditorLocalization.WideLanguage )
				hierarchicalContainer1.SplitterRatio = 0.5f;
		}

		[Browsable( false )]
		public MessageLogOptions Options { get; set; }

		private void MessageLogOptionsForm_Load( object sender, EventArgs e )
		{
			object[] objects = new object[ 1 ];
			objects[ 0 ] = Options;
			hierarchicalContainer1.SetData( null, objects );
		}

		private void buttonClose_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void HierarchicalContainer1_PropertyDisplayNameOverride( HierarchicalContainer sender, HCItemProperty property, ref string displayName )
		{
			displayName = Translate( displayName );
		}

		private void HierarchicalContainer1_OverridePropertyEnumItem( HierarchicalContainer sender, HCItemEnumDropDown property, ref string displayName, ref string description )
		{
			displayName = Translate( displayName );
			description = Translate( description );
		}

		string Translate( string text )
		{
			return EditorLocalization.Translate( "MessageLogOptionsForm", text );
		}
	}
}
