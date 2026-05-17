// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Toolkit;
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
	public partial class ChatWindowOptionsForm : EngineForm
	{
		ChatWindowOptions options;
		
		//

		public ChatWindowOptionsForm()
		{
			InitializeComponent();

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			Text = EditorLocalization2.Translate( "ContentBrowser", Text );
			EditorLocalization2.TranslateForm( "ContentBrowser", this );

			hierarchicalContainer1.OverridePropertyDisplayName += HierarchicalContainer1_PropertyDisplayNameOverride;
			hierarchicalContainer1.OverridePropertyEnumItem += HierarchicalContainer1_OverridePropertyEnumItem;

			if( EditorLocalization2.WideLanguage )
				hierarchicalContainer1.SplitterRatio = 0.5f;

			options = new ChatWindowOptions( this );
			//load settings
			try
			{
				NeoAxisStoreImplementation.LoadFromRegistry( out var token );
				options.NeoXAccessToken = token;
			}
			catch { }

			hierarchicalContainer1.SetData( null, [ options ] );

			//fix delayed update
			hierarchicalContainer1.UpdateItems();
		}

		private void ChatWindowOptionsForm_Load( object sender, EventArgs e )
		{
			UpdateControls();
		}

		private void buttonClose_Click( object sender, EventArgs e )
		{
			//save settings
			NeoAxisStoreImplementation.SaveToRegistry( options.NeoXAccessToken );

			Close();
		}

		private void HierarchicalContainer1_PropertyDisplayNameOverride( HierarchicalContainer sender, HCItemProperty property, ref string displayName )
		{
			displayName = EditorLocalization2.Translate( "ContentBrowser.Options", displayName );
		}

		private void HierarchicalContainer1_OverridePropertyEnumItem( HierarchicalContainer sender, HCItemEnumDropDown property, ref string displayName, ref string description )
		{
			displayName = EditorLocalization2.Translate( "ContentBrowser.Options", displayName );
			description = EditorLocalization2.Translate( "ContentBrowser.Options", description );
		}

		void UpdateControls()
		{
			buttonClose.Location = new Point( ClientSize.Width - buttonClose.Size.Width - DpiHelper.Default.ScaleValue( 12 ), ClientSize.Height - buttonClose.Size.Height - DpiHelper.Default.ScaleValue( 12 ) );
			hierarchicalContainer1.Size = new Size( ClientSize.Width - DpiHelper.Default.ScaleValue( 12 ) - hierarchicalContainer1.Location.X, buttonClose.Location.Y - DpiHelper.Default.ScaleValue( 8 ) - hierarchicalContainer1.Location.Y );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControls();
		}
	}
}