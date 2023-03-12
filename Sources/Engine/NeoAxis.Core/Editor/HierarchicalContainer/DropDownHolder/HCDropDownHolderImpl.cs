#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	class HCDropDownHolderImpl
	{
		Control holder;
		HCDropDownControl innerControl; // child control. the control that is hosted in the holder

		bool resizableTop;
		bool resizableLeft = true;
		//VS.VisualStyleRenderer sizeGripRenderer;
		bool nonInteractive;

		public bool Resizable { get; set; }
		//public Size MinimumSize { get; set; }
		//public Size MaximumSize { get; set; }
		public bool CommitOnClosing { get; set; }
		public bool FocusOnOpen { get; set; } = true;

		public bool NonInteractive
		{
			get { return nonInteractive; }
			set
			{
				if( value != nonInteractive )
				{
					nonInteractive = value;
					//if( IsHandleCreated ) RecreateHandle();
				}
			}
		}

		public HCDropDownHolderImpl( Control holder, HCDropDownControl control )
		{
			this.holder = holder;

			holder.AutoSize = false;
			holder.Margin = Padding.Empty;
			holder.Padding = new Padding( 1 ); // for borders

			innerControl = control ?? throw new ArgumentNullException( "control" );
			innerControl.ParentHolder = (IDropDownHolder)holder;

			if( holder is HCFormDropDownHolder )
				innerControl.Location = new Point( 1, 1 );
			else
				innerControl.Location = Point.Empty;

			//innerControl.RegionChanged += ( sender, e ) => UpdateRegion();
			//innerControl.Paint += ( sender, e ) => PaintSizeGrip( e );

			Resizable = innerControl.Resizable;
		}

		internal void Initialize()
		{
			//MinimumSize = innerControl.MinimumSize + holder.Padding.Size;
			//innerControl.MinimumSize = innerControl.Size;
			//MaximumSize = innerControl.MaximumSize + holder.Padding.Size;
			//innerControl.MaximumSize = innerControl.Size;
			holder.Size = innerControl.Size + holder.Padding.Size;
			//UpdateRegion();
		}

		internal void OnSizeChanged()
		{
			if( innerControl != null )
			{
				//innerControl.MinimumSize = holder.Size - holder.Padding.Size;
				//innerControl.MaximumSize = holder.Size - holder.Padding.Size;
				innerControl.Size = holder.Size - holder.Padding.Size;
			}
		}

		internal void Show( Control openerControl, System.Drawing.Rectangle area )
		{
			var location = CalculateLocation( openerControl, area );
			holder.Location = CalculateDropDownLocation( 
				openerControl.PointToScreen( location ), ToolStripDropDownDirection.BelowLeft ).Location;
			holder.Show();
		}

		internal Point CalculateLocation( Control openerControl, System.Drawing.Rectangle area )
		{
			resizableTop = false;
			resizableLeft = true;

			Point location = openerControl.PointToScreen( new Point( area.Width, area.Top + area.Height + 2 ) );
			//Point location = openerControl.PointToScreen( new Point( area.Width, area.Top + area.Height ) );
			System.Drawing.Rectangle screen = Screen.FromControl( openerControl ).WorkingArea;
			if( location.X - holder.Size.Width < screen.Left )
			{
				resizableLeft = false;
				location.X = screen.Left;
			}
			if( location.Y + holder.Size.Height > ( screen.Top + screen.Height ) )
			{
				resizableTop = true;
				location.Y -= holder.Size.Height + area.Height + 7;
				//location.Y -= holder.Size.Height + area.Height + 5;
			}
			location = openerControl.PointToClient( location );
			return location;
		}

		System.Drawing.Rectangle CalculateDropDownLocation( Point start, ToolStripDropDownDirection dropDownDirection )
		{
			Point offset = Point.Empty;
			System.Drawing.Rectangle dropDownBounds = new System.Drawing.Rectangle( Point.Empty, holder.Size );
			// calculate the offset from the upper left hand corner of the item.
			switch( dropDownDirection )
			{
				case ToolStripDropDownDirection.AboveLeft:
					offset.X = -dropDownBounds.Width;
					offset.Y = -dropDownBounds.Height;
					break;
				case ToolStripDropDownDirection.AboveRight:
					offset.Y = -dropDownBounds.Height;
					break;
				case ToolStripDropDownDirection.BelowRight:
				case ToolStripDropDownDirection.Right:
					break;
				case ToolStripDropDownDirection.BelowLeft:
				case ToolStripDropDownDirection.Left:
					offset.X = -dropDownBounds.Width;
					break;
			}
			dropDownBounds.Location = new Point( start.X + offset.X, start.Y + offset.Y );
			return dropDownBounds;
		}

		//void UpdateRegion()
		//{
		//	if( holder.Region != null )
		//	{
		//		holder.Region.Dispose();
		//		holder.Region = null;
		//	}
		//	if( innerControl.Region != null )
		//	{
		//		holder.Region = innerControl.Region.Clone();
		//	}
		//}

		internal void Close( bool commitChanges )
		{
			CommitOnClosing = commitChanges;
		}

		internal void OnOpened()
		{
			innerControl.OnHolderOpened();
			if( FocusOnOpen )
				innerControl.Focus();
		}

		internal void OnClosed()
		{
			if( CommitOnClosing )
				innerControl.OnCommitChanges();
			else
				innerControl.OnCancelChanges();
		}

		internal bool ProcessCmdKey( Keys keyData )
		{
			if( keyData == Keys.Escape )
			{
				Close( false );
				return true;
			}
			else if( keyData == Keys.Enter )
			{
				Close( true );
				return true;
			}
			return false;
		}

		internal bool ProcessResizing( ref Message m, bool contentControl = true )
		{
			//if( m.Msg == ComponentFactory.Krypton.Toolkit.PI.WM_NCACTIVATE && m.WParam != IntPtr.Zero && _childPopup != null && _childPopup.Visible )
			//{
			//	_childPopup.Hide();
			//}
			if( !Resizable && !NonInteractive )
			{
				return false;
			}
			if( m.Msg == Internal.ComponentFactory.Krypton.Toolkit.PI.WM_NCHITTEST )
			{
				return OnNcHitTest( ref m, contentControl );
			}
			//else if( m.Msg == ComponentFactory.Krypton.Toolkit.PI.WM_GETMINMAXINFO )
			//{
			//	return OnGetMinMaxInfo( ref m );
			//}
			return false;
		}

		//[SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode )]
		//bool OnGetMinMaxInfo( ref Message m )
		//{
		//	ComponentFactory.Krypton.Toolkit.PI.MINMAXINFO minmax = ComponentFactory.Krypton.Toolkit.PI.MINMAXINFO.GetFrom( m.LParam );
		//	if( !MaximumSize.IsEmpty )
		//		minmax.ptMaxTrackSize = new ComponentFactory.Krypton.Toolkit.PI.POINT( MaximumSize.Width, MaximumSize.Height );
		//	minmax.ptMinTrackSize = new ComponentFactory.Krypton.Toolkit.PI.POINT( MinimumSize.Width, MinimumSize.Height );
		//	Marshal.StructureToPtr( minmax, m.LParam, false );
		//	return true;
		//}

		bool OnNcHitTest( ref Message m, bool contentControl )
		{
			if( NonInteractive )
			{
				m.Result = (IntPtr)Internal.ComponentFactory.Krypton.Toolkit.PI.HTTRANSPARENT;
				return true;
			}

			int x = Cursor.Position.X; // NativeMethods.LOWORD(m.LParam);
			int y = Cursor.Position.Y; // NativeMethods.HIWORD(m.LParam);
			Point clientLocation = holder.PointToClient( new Point( x, y ) );

			GripBounds gripBouns = new GripBounds( contentControl ? innerControl.ClientRectangle : holder.ClientRectangle );
			IntPtr transparent = new IntPtr( Internal.ComponentFactory.Krypton.Toolkit.PI.HTTRANSPARENT );

			if( resizableTop )
			{
				if( resizableLeft && gripBouns.TopLeft.Contains( clientLocation ) )
				{
					m.Result = contentControl ? transparent : (IntPtr)Internal.ComponentFactory.Krypton.Toolkit.PI.HTTOPLEFT;
					return true;
				}
				if( !resizableLeft && gripBouns.TopRight.Contains( clientLocation ) )
				{
					m.Result = contentControl ? transparent : (IntPtr)Internal.ComponentFactory.Krypton.Toolkit.PI.HTTOPRIGHT;
					return true;
				}
				if( gripBouns.Top.Contains( clientLocation ) )
				{
					m.Result = contentControl ? transparent : (IntPtr)Internal.ComponentFactory.Krypton.Toolkit.PI.HTTOP;
					return true;
				}
			}
			else
			{
				if( resizableLeft && gripBouns.BottomLeft.Contains( clientLocation ) )
				{
					m.Result = contentControl ? transparent : (IntPtr)Internal.ComponentFactory.Krypton.Toolkit.PI.HTBOTTOMLEFT;
					return true;
				}
				if( !resizableLeft && gripBouns.BottomRight.Contains( clientLocation ) )
				{
					m.Result = contentControl ? transparent : (IntPtr)Internal.ComponentFactory.Krypton.Toolkit.PI.HTBOTTOMRIGHT;
					return true;
				}
				if( gripBouns.Bottom.Contains( clientLocation ) )
				{
					m.Result = contentControl ? transparent : (IntPtr)Internal.ComponentFactory.Krypton.Toolkit.PI.HTBOTTOM;
					return true;
				}
			}
			if( resizableLeft && gripBouns.Left.Contains( clientLocation ) )
			{
				m.Result = contentControl ? transparent : (IntPtr)Internal.ComponentFactory.Krypton.Toolkit.PI.HTLEFT;
				return true;
			}
			if( !resizableLeft && gripBouns.Right.Contains( clientLocation ) )
			{
				m.Result = contentControl ? transparent : (IntPtr)Internal.ComponentFactory.Krypton.Toolkit.PI.HTRIGHT;
				return true;
			}
			return false;
		}

		//void PaintSizeGrip( PaintEventArgs e )
		//{
		//	if( e == null || e.Graphics == null || !Resizable )
		//		return;

		//	Size clientSize = innerControl.ClientSize;
		//	using( Bitmap gripImage = new Bitmap( 0x10, 0x10 ) )
		//	{
		//		using( Graphics g = Graphics.FromImage( gripImage ) )
		//		{
		//			if( Application.RenderWithVisualStyles )
		//			{
		//				if( sizeGripRenderer == null )
		//					sizeGripRenderer = new VS.VisualStyleRenderer( VS.VisualStyleElement.Status.Gripper.Normal );
		//				sizeGripRenderer.DrawBackground( g, new System.Drawing.Rectangle( 0, 0, 0x10, 0x10 ) );
		//			}
		//			else
		//			{
		//				ControlPaint.DrawSizeGrip( g, innerControl.BackColor, 0, 0, 0x10, 0x10 );
		//			}
		//		}
		//		GraphicsState gs = e.Graphics.Save();
		//		e.Graphics.ResetTransform();
		//		if( resizableTop )
		//		{
		//			if( resizableLeft )
		//			{
		//				e.Graphics.RotateTransform( 180 );
		//				e.Graphics.TranslateTransform( -clientSize.Width, -clientSize.Height );
		//			}
		//			else
		//			{
		//				e.Graphics.ScaleTransform( 1, -1 );
		//				e.Graphics.TranslateTransform( 0, -clientSize.Height );
		//			}
		//		}
		//		else if( resizableLeft )
		//		{
		//			e.Graphics.ScaleTransform( -1, 1 );
		//			e.Graphics.TranslateTransform( -clientSize.Width, 0 );
		//		}
		//		e.Graphics.DrawImage( gripImage, clientSize.Width - 0x10, clientSize.Height - 0x10 + 1, 0x10, 0x10 );
		//		e.Graphics.Restore( gs );
		//	}
		//}
	}
}

#endif