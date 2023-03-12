#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	[ToolboxItem( false ), DesignerCategory( "" )]
	public class HCToolStripDropDownHolder : ToolStripDropDown, IDropDownHolder
	{
		HCDropDownHolderImpl impl;
		Control openerControl; // control that calls holder.

		DropShadowManager dropShadowManager;
		ShadowImageCacheManager borderImageCacheManager;

		/////////////////////////////////////////

		class MyColorTable : ProfessionalColorTable
		{
			public MyColorTable()
			{
				base.UseSystemColors = false;
			}

			public override Color MenuBorder
			{
				get
				{
					return EditorAPI.DarkTheme ? Color.FromArgb( 90, 90, 90 ) : Color.FromArgb( 200, 200, 200 );
				}
			}
		}

		/////////////////////////////////////////

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

		// public bool NonInteractive => impl.NonInteractive; //TODO: implement NonInteractive if needed

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= Internal.ComponentFactory.Krypton.Toolkit.PI.WS_EX_NOACTIVATE;
				//if( NonInteractive )
				//	cp.ExStyle |= ComponentFactory.Krypton.Toolkit.PI.WS_EX_TRANSPARENT | ComponentFactory.Krypton.Toolkit.PI.WS_EX_LAYERED | ComponentFactory.Krypton.Toolkit.PI.WS_EX_TOOLWINDOW;
				return cp;
			}
		}

		public event System.EventHandler HolderClosed;

		public HCToolStripDropDownHolder( HCDropDownControl control )
		{
			//DoubleBuffered = true;
			ResizeRedraw = true;
			AutoSize = false;

			RenderMode = ToolStripRenderMode.Professional; // only border color
			Renderer = new ToolStripProfessionalRenderer( new MyColorTable() );
			//RenderMode = ToolStripRenderMode.System; // only border color

			DropShadowEnabled = false;

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

		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );

			KryptonWinFormsUtility.LockFormUpdate( this );

			UpdateShadowState();
		}

		protected override void Dispose( bool disposing )
		{
			ReleaseShadow();

			base.Dispose( disposing );
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

			KryptonWinFormsUtility.LockFormUpdate( null );
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

		public bool ProcessResizing( ref Message m )
		{
			if( impl != null )
				return impl.ProcessResizing( ref m );
			else
				return false;
		}

		void UpdateShadowState()
		{
			if( DesignMode || !IsHandleCreated || IsDisposed )
				return;

			//if( IsCustomHeader )//&& IsOnePixelBorder)
			CreateFormShadow();
			//else
			//    ReleaseShadow();
		}

		void CreateFormShadow()
		{
			if( borderImageCacheManager == null )
				borderImageCacheManager = new ShadowImageCacheManager();

			if( dropShadowManager == null )
			{
				dropShadowManager = new DropShadowManager( this );
				UpdateShadowColor();
			}
		}

		void ReleaseShadow()
		{
			if( dropShadowManager != null )
			{
				dropShadowManager.Dispose();
				dropShadowManager = null;
			}

			if( borderImageCacheManager != null )
			{
				borderImageCacheManager.Dispose();
				borderImageCacheManager = null;
			}
		}

		void UpdateShadowColor()
		{
			if( dropShadowManager != null )
			{
				var color = KryptonDarkThemeUtility.DarkTheme ? Color.FromArgb( 30, 30, 30 ) : Color.FromArgb( 150, 150, 150 );
				dropShadowManager.ImageCache = borderImageCacheManager.GetCached( color );
			}
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			UpdateShadowState();
		}

	}
}

#endif