// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	[ToolboxItem( false ), DesignerCategory( "" )]
	public class HCToolStripDropDownHolder : ToolStripDropDown, IDropDownHolder
	{
		HCDropDownHolderImpl impl;
		Control openerControl; // control that calls holder.

		public bool Resizable
		{
			get { return impl != null ? impl.Resizable : false; }
		}
		public new Size MinimumSize
		{
			get { return impl != null ? impl.MinimumSize : Size.Empty; }
		}
		public new Size MaximumSize
		{
			get { return impl != null ? impl.MaximumSize : Size.Empty; }
		}

		// public bool NonInteractive => impl.NonInteractive; //TODO: implement NonInteractive if needed

		protected override CreateParams CreateParams
		{
			[SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode )]
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= PI.WS_EX_NOACTIVATE;
				//if( NonInteractive )
				//	cp.ExStyle |= PI.WS_EX_TRANSPARENT | PI.WS_EX_LAYERED | PI.WS_EX_TOOLWINDOW;
				return cp;
			}
		}

		public event EventHandler HolderClosed;

		public HCToolStripDropDownHolder( HCDropDownControl control )
		{
			//DoubleBuffered = true;
			ResizeRedraw = true;
			AutoSize = false;
			RenderMode = ToolStripRenderMode.System; // only border color

			//DropShadowEnabled = false;

			Closed += ( s, e ) => HolderClosed( s, e );

			impl = new HCDropDownHolderImpl( this, control );
			impl.Initialize();

			var controlHost = new ToolStripControlHost( control )
			{
				Margin = Padding.Empty,
				Padding = Padding.Empty
			};

			Items.Add( controlHost );
		}

		protected override void OnSizeChanged( EventArgs e )
		{
			impl?.OnSizeChanged();
			base.OnSizeChanged( e );
		}

		public void Show( Control openerControl )
		{
			if( openerControl == null )
				throw new ArgumentNullException( nameof( openerControl ) );
			Show( openerControl, openerControl.ClientRectangle );
		}

		public void Show( Control openerControl, System.Drawing.Rectangle area )
		{
			this.openerControl = openerControl ?? throw new ArgumentNullException( "sourceControl" );
			var location = impl.CalculateLocation( openerControl, area );
			Show( openerControl, location, ToolStripDropDownDirection.BelowLeft );
		}

		public void Close( bool commitChanges )
		{
			impl.Close( commitChanges );
			Close( ToolStripDropDownCloseReason.CloseCalled );
		}

		protected override void OnOpened( EventArgs e )
		{
			base.OnOpened( e );
			impl?.OnOpened();
		}

		protected override void OnClosed( ToolStripDropDownClosedEventArgs e )
		{
			base.OnClosed( e );
			impl?.OnClosed();
		}

		protected override void OnClosing( ToolStripDropDownClosingEventArgs e )
		{
			if( e.CloseReason == ToolStripDropDownCloseReason.AppFocusChange ||
				e.CloseReason == ToolStripDropDownCloseReason.AppClicked )
			{
				if( impl != null )
					impl.CommitOnClosing = true;
			}

			if( e.CloseReason == ToolStripDropDownCloseReason.ItemClicked )
				e.Cancel = true;

			if( e.CloseReason == ToolStripDropDownCloseReason.AppClicked )
			{
				// prevents the clicking of items from closing the dropdown.
				var control = ( openerControl is IHCProperty ) ?
					( (IHCProperty)openerControl ).EditorControl : openerControl;
				if( control.ClientRectangle.Contains( control.PointToClient( Cursor.Position ) ) )
					e.Cancel = true;
			}

			base.OnClosing( e );
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( impl != null )
				return impl.ProcessCmdKey( keyData );
			else
				return false;
		}

		[SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode )]
		protected override void WndProc( ref Message m )
		{
			if( impl != null && impl.ProcessResizing( ref m, false ) )
				return;
			try
			{
				base.WndProc( ref m );
			}
			catch { }
		}

		[SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode )]
		public bool ProcessResizing( ref Message m )
		{
			if( impl != null )
				return impl.ProcessResizing( ref m );
			else
				return false;
		}
	}
}
