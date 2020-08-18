using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
	public class ToolStripPanel : ContainerControl, IComponent, IDisposable
	{
		public class ToolStripPanelRowCollection : ArrangedElementCollection, IList, ICollection, IEnumerable
		{
			public virtual ToolStripPanelRow this[int index]
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

			bool IList.IsReadOnly
			{
				get
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

			public ToolStripPanelRowCollection(ToolStripPanel owner)
			{
				throw null;
			}

			public ToolStripPanelRowCollection(ToolStripPanel owner, ToolStripPanelRow[] value)
			{
				throw null;
			}

			public int Add(ToolStripPanelRow value)
			{
				throw null;
			}

			public void AddRange(ToolStripPanelRow[] value)
			{
				throw null;
			}

			public void AddRange(ToolStripPanelRowCollection value)
			{
				throw null;
			}

			public bool Contains(ToolStripPanelRow value)
			{
				throw null;
			}

			public virtual void Clear()
			{
				throw null;
			}

			void IList.Clear()
			{
				throw null;
			}

			bool IList.Contains(object value)
			{
				throw null;
			}

			void IList.RemoveAt(int index)
			{
				throw null;
			}

			void IList.Remove(object value)
			{
				throw null;
			}

			int IList.Add(object value)
			{
				throw null;
			}

			int IList.IndexOf(object value)
			{
				throw null;
			}

			void IList.Insert(int index, object value)
			{
				throw null;
			}

			public int IndexOf(ToolStripPanelRow value)
			{
				throw null;
			}

			public void Insert(int index, ToolStripPanelRow value)
			{
				throw null;
			}

			public void Remove(ToolStripPanelRow value)
			{
				throw null;
			}

			public void RemoveAt(int index)
			{
				throw null;
			}

			public void CopyTo(ToolStripPanelRow[] array, int index)
			{
				throw null;
			}
		}

		public override bool AllowDrop
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

		public override bool AutoScroll
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

		public new Size AutoScrollMargin
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

		public new Size AutoScrollMinSize
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

		public override bool AutoSize
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

		protected override Padding DefaultPadding
		{
			get
			{
				throw null;
			}
		}

		protected override Padding DefaultMargin
		{
			get
			{
				throw null;
			}
		}

		public Padding RowMargin
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

		public override DockStyle Dock
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

		public override LayoutEngine LayoutEngine
		{
			get
			{
				throw null;
			}
		}

		public bool Locked
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

		public Orientation Orientation
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

		public ToolStripRenderer Renderer
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

		public ToolStripRenderMode RenderMode
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

		public ToolStripPanelRow[] Rows
		{
			get
			{
				throw null;
			}
		}

		public new int TabIndex
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

		public new event EventHandler AutoSizeChanged
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

		public event EventHandler RendererChanged
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

		public new event EventHandler TabIndexChanged
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

		public new event EventHandler TextChanged
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

		public ToolStripPanel()
		{
			throw null;
		}

		public void BeginInit()
		{
			throw null;
		}

		public void EndInit()
		{
			throw null;
		}

		protected override ControlCollection CreateControlsInstance()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			throw null;
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			throw null;
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			throw null;
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRendererChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnParentChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnDockChanged(EventArgs e)
		{
			throw null;
		}

		public void Join(ToolStrip toolStripToDrag)
		{
			throw null;
		}

		public void Join(ToolStrip toolStripToDrag, int row)
		{
			throw null;
		}

		public void Join(ToolStrip toolStripToDrag, int x, int y)
		{
			throw null;
		}

		public void Join(ToolStrip toolStripToDrag, Point location)
		{
			throw null;
		}

		public ToolStripPanelRow PointToRow(Point clientLocation)
		{
			throw null;
		}
	}
}
