using System.Collections;
using System.Drawing;

namespace System.Windows.Forms
{
	public class LinkLabel : Label, IButtonControl
	{
		public class LinkCollection : IList, ICollection, IEnumerable
		{
			public virtual Link this[int index]
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

			object IList.this[int index]
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

			public virtual Link this[string key]
			{
				get
				{
					throw null;
				}
			}

			public int Count
			{
				get
				{
					throw null;
				}
			}

			public bool LinksAdded
			{
				get
				{
					throw null;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					throw null;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					throw null;
				}
			}

			bool IList.IsFixedSize
			{
				get
				{
					throw null;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					throw null;
				}
			}

			public LinkCollection(LinkLabel owner)
			{
				throw null;
			}

			public Link Add(int start, int length)
			{
				throw null;
			}

			public Link Add(int start, int length, object linkData)
			{
				throw null;
			}

			public int Add(Link value)
			{
				throw null;
			}

			int IList.Add(object value)
			{
				throw null;
			}

			void IList.Insert(int index, object value)
			{
				throw null;
			}

			public bool Contains(Link link)
			{
				throw null;
			}

			public virtual bool ContainsKey(string key)
			{
				throw null;
			}

			bool IList.Contains(object link)
			{
				throw null;
			}

			public int IndexOf(Link link)
			{
				throw null;
			}

			int IList.IndexOf(object link)
			{
				throw null;
			}

			public virtual int IndexOfKey(string key)
			{
				throw null;
			}

			public virtual void Clear()
			{
				throw null;
			}

			void ICollection.CopyTo(Array dest, int index)
			{
				throw null;
			}

			public IEnumerator GetEnumerator()
			{
				throw null;
			}

			public void Remove(Link value)
			{
				throw null;
			}

			public void RemoveAt(int index)
			{
				throw null;
			}

			public virtual void RemoveByKey(string key)
			{
				throw null;
			}

			void IList.Remove(object value)
			{
				throw null;
			}
		}

		public class Link
		{
			public string Description
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

			public bool Enabled
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

			public int Length
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

			public object LinkData
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

			public int Start
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

			public object Tag
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

			public bool Visited
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

			public Link()
			{
				throw null;
			}

			public Link(int start, int length)
			{
				throw null;
			}

			public Link(int start, int length, object linkData)
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

		public Color DisabledLinkColor
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

		public new FlatStyle FlatStyle
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

		public LinkArea LinkArea
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

		public LinkBehavior LinkBehavior
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

		public LinkCollection Links
		{
			get
			{
				throw null;
			}
		}

		public bool LinkVisited
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

		protected Cursor OverrideCursor
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

		public new bool TabStop
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

		public override string Text
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

		public new Padding Padding
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

		DialogResult IButtonControl.DialogResult
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

		public new bool UseCompatibleTextRendering
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

		public new event EventHandler TabStopChanged
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

		public event LinkLabelLinkClickedEventHandler LinkClicked
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

		public LinkLabel()
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override void CreateHandle()
		{
			throw null;
		}

		protected Link PointInLink(int x, int y)
		{
			throw null;
		}

		void IButtonControl.NotifyDefault(bool value)
		{
			throw null;
		}

		protected override void OnGotFocus(EventArgs e)
		{
			throw null;
		}

		protected override void OnLostFocus(EventArgs e)
		{
			throw null;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
		{
			throw null;
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			throw null;
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnAutoSizeChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnTextAlignChanged(EventArgs e)
		{
			throw null;
		}

		void IButtonControl.PerformClick()
		{
			throw null;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			throw null;
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			throw null;
		}

		protected override void Select(bool directed, bool forward)
		{
			throw null;
		}

		protected override void WndProc(ref Message msg)
		{
			throw null;
		}
	}
}
