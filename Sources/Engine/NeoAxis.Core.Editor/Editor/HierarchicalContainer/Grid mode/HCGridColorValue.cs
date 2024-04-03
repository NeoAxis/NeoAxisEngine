#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public partial class HCGridColorValue : EUserControl, IHCColorValue
	{
		public HCGridColorValue()
		{
			InitializeComponent();

			textBox1.Location = new Point( 0, DpiHelper.Default.ScaleValue( 3 ) );
			textBox1.AutoSize = false;
			textBox1.Height = DpiHelper.Default.ScaleValue( 18 );

			previewButton.Location = new Point( previewButton.Location.X, DpiHelper.Default.ScaleValue( 3 ) );
		}

		protected override void OnParentChanged( EventArgs e )
		{
			base.OnParentChanged( e );

			////!!!!
			//if( Parent != null )
			//{
			//	if( Parent.Tag == null )
			//	{
			//		Parent.Tag = 1;

			//		textBox1.Tag = 4;

			//		System.Diagnostics.Debug.WriteLine( "START" );
			//	}
			//}
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			//textBox1.ForceControlLayout( true );
		}

		public EngineTextBox TextBox
		{
			get { return textBox1; }
		}

		public HCColorPreviewButton PreviewButton
		{
			get { return previewButton; }
		}
	}
}

#endif