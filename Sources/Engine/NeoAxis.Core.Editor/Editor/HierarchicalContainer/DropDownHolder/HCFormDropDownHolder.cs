#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class HCFormDropDownHolder : EngineForm, IDropDownHolder
	{
		HCDropDownHolderImpl impl;

		//

		public bool Resizable
		{
			get { return impl != null ? impl.Resizable : false; }
		}
		//public new Size MinimumSize
		//{
		//	get { return impl != null ? impl.MinimumSize : Size.Empty; }
		//}
		//public new Size MaximumSize
		//{
		//	get { return impl != null ? impl.MaximumSize : Size.Empty; }
		//}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= Internal.ComponentFactory.Krypton.Toolkit.PI.WS_EX_TOOLWINDOW;
				cp.Style |= Internal.ComponentFactory.Krypton.Toolkit.PI.WS_POPUP | Internal.ComponentFactory.Krypton.Toolkit.PI.WS_BORDER;

				return cp;
			}
		}

		public event System.EventHandler HolderClosed;

		public HCFormDropDownHolder( HCDropDownControl control )
		{
			//InitializeComponent();

			MinimumSize = new Size( 1, 1 );
			MaximumSize = new Size( 10000, 10000 );

			//DoubleBuffered = true;
			ResizeRedraw = true;
			AutoSize = false;

			//border
			BackColor = EditorAPI2.DarkTheme ? Color.FromArgb( 90, 90, 90 ) : Color.FromArgb( 240, 240, 240 );

			Closed += ( s, e ) => HolderClosed( s, e );

			FormBorderStyle = FormBorderStyle.None;
			StartPosition = FormStartPosition.Manual;

			Controls.Add( control );

			impl = new HCDropDownHolderImpl( this, control );
			impl.Initialize();
		}

		public void Show( Control openerControl )
		{
			if( openerControl == null )
				throw new ArgumentNullException( nameof( openerControl ) );
			Show( openerControl, openerControl.ClientRectangle );
		}

		public void Show( Control openerControl, System.Drawing.Rectangle area )
		{
			impl.Show( openerControl, area );
		}

		public void Close( bool commitChanges )
		{
			impl.Close( commitChanges );
			Close();
		}

		protected override void OnSizeChanged( EventArgs e )
		{
			impl?.OnSizeChanged();
			base.OnSizeChanged( e );
		}

		protected override void OnClosed( EventArgs e )
		{
			base.OnClosed( e );
			impl?.OnClosed();
		}

		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );
			if( Visible )
				impl?.OnOpened();
		}

		[DllImport( "user32.dll" )]
		internal/*obfuscator*/ static extern IntPtr GetActiveWindow();

		protected override void OnDeactivate( EventArgs e )
		{
			base.OnDeactivate( e );

			if( Disposing )
				return; // already closing

			if( !EditorAPI.ClosingApplication )
			{
				try
				{
					//HACK:
					// 1) check active window after 10 ms.
					// 2) if active window is main form or null, close holder.
					// it prevents from closing when the WPF forms open. eg. AvelonEdit CompletionWindow
					Task.Delay( 10 ).ContinueWith( ( t ) =>
					{
						var wnd = GetActiveWindow();
						if( wnd == IntPtr.Zero || wnd == EditorForm.Instance.Handle )
							Close( true );
					}, TaskScheduler.FromCurrentSynchronizationContext() );
				}
				catch { }
			}
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( impl != null )
				return impl.ProcessCmdKey( keyData );
			else
				return false;
		}

		protected override void WndProc( ref Message m )
		{
			if( impl != null && impl.ProcessResizing( ref m, false ) )
				return;
			base.WndProc( ref m );
		}

		public bool ProcessResizing( ref Message m )
		{
			if( impl != null )
				return impl.ProcessResizing( ref m );
			else
				return false;
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// HCFormDropDownHolder
			// 
			this.ClientSize = new System.Drawing.Size( 282, 253 );
			this.Name = "HCFormDropDownHolder";
			this.ResumeLayout( false );

		}
	}
}

#endif