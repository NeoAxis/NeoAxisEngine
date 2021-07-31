using System.Drawing;

namespace System.Windows.Forms
{
	public sealed class HtmlWindow
	{
		public HtmlDocument Document
		{
			get
			{
				throw null;
			}
		}

		public object DomWindow
		{
			get
			{
				throw null;
			}
		}

		public HtmlWindowCollection Frames
		{
			get
			{
				throw null;
			}
		}

		public HtmlHistory History
		{
			get
			{
				throw null;
			}
		}

		public bool IsClosed
		{
			get
			{
				throw null;
			}
		}

		public string Name
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public HtmlWindow Opener
		{
			get
			{
				throw null;
			}
		}

		public HtmlWindow Parent
		{
			get
			{
				throw null;
			}
		}

		public Point Position
		{
			get
			{
				throw null;
			}
		}

		public Size Size
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public string StatusBarText
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Uri Url
		{
			get
			{
				throw null;
			}
		}

		public HtmlElement WindowFrameElement
		{
			get
			{
				throw null;
			}
		}

		public event HtmlElementErrorEventHandler Error
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

		public event HtmlElementEventHandler GotFocus
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

		public event HtmlElementEventHandler Load
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

		public event HtmlElementEventHandler LostFocus
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

		public event HtmlElementEventHandler Resize
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

		public event HtmlElementEventHandler Scroll
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

		public event HtmlElementEventHandler Unload
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

		public void Alert(string message)
		{
			throw null;
		}

		public void AttachEventHandler(string eventName, EventHandler eventHandler)
		{
			throw null;
		}

		public void Close()
		{
			throw null;
		}

		public bool Confirm(string message)
		{
			throw null;
		}

		public void DetachEventHandler(string eventName, EventHandler eventHandler)
		{
			throw null;
		}

		public void Focus()
		{
			throw null;
		}

		public void MoveTo(int x, int y)
		{
			throw null;
		}

		public void MoveTo(Point point)
		{
			throw null;
		}

		public void Navigate(Uri url)
		{
			throw null;
		}

		public void Navigate(string urlString)
		{
			throw null;
		}

		public HtmlWindow Open(string urlString, string target, string windowOptions, bool replaceEntry)
		{
			throw null;
		}

		public HtmlWindow Open(Uri url, string target, string windowOptions, bool replaceEntry)
		{
			throw null;
		}

		public HtmlWindow OpenNew(string urlString, string windowOptions)
		{
			throw null;
		}

		public HtmlWindow OpenNew(Uri url, string windowOptions)
		{
			throw null;
		}

		public string Prompt(string message, string defaultInputValue)
		{
			throw null;
		}

		public void RemoveFocus()
		{
			throw null;
		}

		public void ResizeTo(int width, int height)
		{
			throw null;
		}

		public void ResizeTo(Size size)
		{
			throw null;
		}

		public void ScrollTo(int x, int y)
		{
			throw null;
		}

		public void ScrollTo(Point point)
		{
			throw null;
		}

		public static bool operator ==(HtmlWindow left, HtmlWindow right)
		{
			throw null;
		}

		public static bool operator !=(HtmlWindow left, HtmlWindow right)
		{
			throw null;
		}

		public override int GetHashCode()
		{
			throw null;
		}

		public override bool Equals(object obj)
		{
			throw null;
		}
	}
}
