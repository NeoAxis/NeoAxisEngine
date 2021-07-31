using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
	public sealed class BehaviorService : IDisposable
	{
		public BehaviorServiceAdornerCollection Adorners
		{
			get
			{
				throw null;
			}
		}

		public Graphics AdornerWindowGraphics
		{
			get
			{
				throw null;
			}
		}

		public Behavior CurrentBehavior
		{
			get
			{
				throw null;
			}
		}

		public event BehaviorDragDropEventHandler BeginDrag
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public event BehaviorDragDropEventHandler EndDrag
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public event EventHandler Synchronize
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public Point AdornerWindowPointToScreen(Point p)
		{
			throw null;
		}

		public Point AdornerWindowToScreen()
		{
			throw null;
		}

		public Point ControlToAdornerWindow(Control c)
		{
			throw null;
		}

		public Point MapAdornerWindowPoint(IntPtr handle, Point pt)
		{
			throw null;
		}

		public Rectangle ControlRectInAdornerWindow(Control c)
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		public Behavior GetNextBehavior(Behavior behavior)
		{
			throw null;
		}

		public void Invalidate()
		{
			throw null;
		}

		public void Invalidate(Rectangle rect)
		{
			throw null;
		}

		public void Invalidate(Region r)
		{
			throw null;
		}

		public void SyncSelection()
		{
			throw null;
		}

		public Behavior PopBehavior(Behavior behavior)
		{
			throw null;
		}

		public void PushBehavior(Behavior behavior)
		{
			throw null;
		}

		public void PushCaptureBehavior(Behavior behavior)
		{
			throw null;
		}

		public Point ScreenToAdornerWindow(Point p)
		{
			throw null;
		}
	}
}
