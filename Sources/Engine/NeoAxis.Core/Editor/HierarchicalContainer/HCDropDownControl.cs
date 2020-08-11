// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using System.Drawing;

namespace NeoAxis.Editor
{
	public class HCDropDownControl : EUserControl
	{
		public bool Resizable { get; set; }
		public bool UseFormDropDownHolder { get; set; }
		public IDropDownHolder ParentHolder { get; set; }

		public HCDropDownControl()
		{
			MinimumSize = new Size( 1, 1 );
			MaximumSize = new Size( 10000, 10000 );
		}

		public virtual void OnCommitChanges()
		{ }

		public virtual void OnCancelChanges()
		{ }

		public virtual void OnHolderOpened()
		{ }

		public void AddOkCancelButtons( out KryptonButton buttonOK, out KryptonButton buttonCancel )
		{
			var size = DpiHelper.Default.ScaleValue( new Size( 94, 26 ) );

			buttonOK = new KryptonButton();
			buttonOK.Size = size;
			buttonOK.Text = "OK";
			buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonOK.Location = new Point( Width - size.Width - buttonOK.Width - 20, Height - buttonOK.Height - 10 );
			buttonOK.Click += ( s, e ) => ParentHolder.Close( true );
			Controls.Add( buttonOK );

			buttonCancel = new KryptonButton();
			buttonCancel.Size = size;
			buttonCancel.Text = "Cancel";
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.Location = new Point( Size - buttonCancel.Size - new Size( 10, 10 ) );
			buttonCancel.Click += ( s, e ) => ParentHolder.Close( false );
			Controls.Add( buttonCancel );

			Height += buttonCancel.Height + 10;

			buttonOK.Text = EditorLocalization.Translate( "General", buttonOK.Text );
			buttonCancel.Text = EditorLocalization.Translate( "General", buttonCancel.Text );
		}

		protected override void WndProc( ref Message m )
		{
			if( Parent is IDropDownHolder holder && holder.ProcessResizing( ref m ) )
				return;

			base.WndProc( ref m );
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams handleParam = base.CreateParams;
				handleParam.ExStyle |= 0x02000000;//WS_EX_COMPOSITED       
				return handleParam;
			}
		}
	}
}
