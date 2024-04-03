#if !DEPLOY
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
	public partial class ContentBrowserOptionsForm : EngineForm
	{
		public ContentBrowserOptionsForm( ContentBrowser browser )
		{
			Browser = browser;

			InitializeComponent();

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			Text = EditorLocalization2.Translate( "ContentBrowser", Text );
			EditorLocalization2.TranslateForm( "ContentBrowser", this );

			hierarchicalContainer1.OverridePropertyDisplayName += HierarchicalContainer1_PropertyDisplayNameOverride;
			hierarchicalContainer1.OverridePropertyEnumItem += HierarchicalContainer1_OverridePropertyEnumItem;

			if( EditorLocalization2.WideLanguage )
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
			UpdateControls();

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

#endif