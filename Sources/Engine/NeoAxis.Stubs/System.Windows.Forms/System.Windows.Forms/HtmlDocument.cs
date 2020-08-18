using System.Drawing;

namespace System.Windows.Forms
{
	public sealed class HtmlDocument
	{
		public HtmlElement ActiveElement
		{
			get
			{
				throw null;
			}
		}

		public HtmlElement Body
		{
			get
			{
				throw null;
			}
		}

		public string Domain
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

		public string Title
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

		public HtmlWindow Window
		{
			get
			{
				throw null;
			}
		}

		public Color BackColor
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

		public Color ForeColor
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

		public Color LinkColor
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

		public Color ActiveLinkColor
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

		public Color VisitedLinkColor
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

		public bool Focused
		{
			get
			{
				throw null;
			}
		}

		public object DomDocument
		{
			get
			{
				throw null;
			}
		}

		public string Cookie
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

		public bool RightToLeft
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

		public string Encoding
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

		public string DefaultEncoding
		{
			get
			{
				throw null;
			}
		}

		public HtmlElementCollection All
		{
			get
			{
				throw null;
			}
		}

		public HtmlElementCollection Links
		{
			get
			{
				throw null;
			}
		}

		public HtmlElementCollection Images
		{
			get
			{
				throw null;
			}
		}

		public HtmlElementCollection Forms
		{
			get
			{
				throw null;
			}
		}

		public event HtmlElementEventHandler Click
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

		public event HtmlElementEventHandler ContextMenuShowing
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

		public event HtmlElementEventHandler Focusing
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

		public event HtmlElementEventHandler LosingFocus
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

		public event HtmlElementEventHandler MouseDown
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

		public event HtmlElementEventHandler MouseLeave
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

		public event HtmlElementEventHandler MouseMove
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

		public event HtmlElementEventHandler MouseOver
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

		public event HtmlElementEventHandler MouseUp
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

		public event HtmlElementEventHandler Stop
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

		public void Write(string text)
		{
			throw null;
		}

		public void ExecCommand(string command, bool showUI, object value)
		{
			throw null;
		}

		public void Focus()
		{
			throw null;
		}

		public HtmlElement GetElementById(string id)
		{
			throw null;
		}

		public HtmlElement GetElementFromPoint(Point point)
		{
			throw null;
		}

		public HtmlElementCollection GetElementsByTagName(string tagName)
		{
			throw null;
		}

		public HtmlDocument OpenNew(bool replaceInHistory)
		{
			throw null;
		}

		public HtmlElement CreateElement(string elementTag)
		{
			throw null;
		}

		public object InvokeScript(string scriptName, object[] args)
		{
			throw null;
		}

		public object InvokeScript(string scriptName)
		{
			throw null;
		}

		public void AttachEventHandler(string eventName, EventHandler eventHandler)
		{
			throw null;
		}

		public void DetachEventHandler(string eventName, EventHandler eventHandler)
		{
			throw null;
		}

		public static bool operator ==(HtmlDocument left, HtmlDocument right)
		{
			throw null;
		}

		public static bool operator !=(HtmlDocument left, HtmlDocument right)
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
