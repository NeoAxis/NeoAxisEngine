using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
	public class ToolStrip : ScrollableControl, IComponent, IDisposable
	{
		public class ToolStripAccessibleObject : ControlAccessibleObject
		{
			public override AccessibleRole Role
			{
				get
				{
					throw null;
				}
			}

			public ToolStripAccessibleObject(ToolStrip owner)
				:base(owner)
			{
				throw null;
			}

			public override AccessibleObject HitTest(int x, int y)
			{
				throw null;
			}

			public override AccessibleObject GetChild(int index)
			{
				throw null;
			}

			public override int GetChildCount()
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

		public new Point AutoScrollPosition
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

		public bool AllowItemReorder
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

		public bool AllowMerge
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

		public override AnchorStyles Anchor
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

		public new Color BackColor
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

		public override BindingContext BindingContext
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

		public bool CanOverflow
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

		public new bool CausesValidation
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

		public new ControlCollection Controls
		{
			get
			{
				throw null;
			}
		}

		public override Cursor Cursor
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

		public override Font Font
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

		protected override Size DefaultSize
		{
			get
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

		protected virtual DockStyle DefaultDock
		{
			get
			{
				throw null;
			}
		}

		protected virtual Padding DefaultGripMargin
		{
			get
			{
				throw null;
			}
		}

		protected virtual bool DefaultShowItemToolTips
		{
			get
			{
				throw null;
			}
		}

		public virtual ToolStripDropDownDirection DefaultDropDownDirection
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

		public override Rectangle DisplayRectangle
		{
			get
			{
				throw null;
			}
		}

		public new Color ForeColor
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

		public ToolStripGripStyle GripStyle
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

		public ToolStripGripDisplayStyle GripDisplayStyle
		{
			get
			{
				throw null;
			}
		}

		public Padding GripMargin
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

		public Rectangle GripRectangle
		{
			get
			{
				throw null;
			}
		}

		public new bool HasChildren
		{
			get
			{
				throw null;
			}
		}

		public new HScrollProperties HorizontalScroll
		{
			get
			{
				throw null;
			}
		}

		public Size ImageScalingSize
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

		public ImageList ImageList
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

		public bool IsCurrentlyDragging
		{
			get
			{
				throw null;
			}
		}

		public virtual ToolStripItemCollection Items
		{
			get
			{
				throw null;
			}
		}

		public bool IsDropDown
		{
			get
			{
				throw null;
			}
		}

		public LayoutSettings LayoutSettings
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

		public ToolStripLayoutStyle LayoutStyle
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

		public ToolStripOverflowButton OverflowButton
		{
			get
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
		}

		public bool Stretch
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

		public bool ShowItemToolTips
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

		public virtual ToolStripTextDirection TextDirection
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

		public new VScrollProperties VerticalScroll
		{
			get
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

		public event EventHandler BeginDrag
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

		public new event EventHandler CausesValidationChanged
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

		public new event ControlEventHandler ControlAdded
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

		public new event EventHandler CursorChanged
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

		public new event ControlEventHandler ControlRemoved
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

		public event EventHandler EndDrag
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

		public new event EventHandler ForeColorChanged
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

		public event ToolStripItemEventHandler ItemAdded
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

		public event ToolStripItemClickedEventHandler ItemClicked
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

		public event ToolStripItemEventHandler ItemRemoved
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

		public event EventHandler LayoutCompleted
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

		public event EventHandler LayoutStyleChanged
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

		public event PaintEventHandler PaintGrip
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

		public ToolStrip()
		{
			throw null;
		}

		public ToolStrip(params ToolStripItem[] items)
		{
			throw null;
		}

		protected virtual LayoutSettings CreateLayoutSettings(ToolStripLayoutStyle layoutStyle)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public virtual ToolStripItem GetNextItem(ToolStripItem start, ArrowDirection direction)
		{
			throw null;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			throw null;
		}

		protected override bool IsInputChar(char charCode)
		{
			throw null;
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			throw null;
		}

		protected override bool ProcessCmdKey(ref Message m, Keys keyData)
		{
			throw null;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			throw null;
		}

		protected virtual void OnBeginDrag(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnEndDrag(EventArgs e)
		{
			throw null;
		}

		protected override void OnDockChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRendererChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnInvalidated(InvalidateEventArgs e)
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnItemClicked(ToolStripItemClickedEventArgs e)
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			throw null;
		}

		protected virtual void OnLayoutCompleted(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnLayoutStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnLostFocus(EventArgs e)
		{
			throw null;
		}

		protected override void OnLeave(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseDown(MouseEventArgs mea)
		{
			throw null;
		}

		protected override void OnMouseMove(MouseEventArgs mea)
		{
			throw null;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseCaptureChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseUp(MouseEventArgs mea)
		{
			throw null;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			throw null;
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			throw null;
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnScroll(ScrollEventArgs se)
		{
			throw null;
		}

		protected override void OnTabStopChanged(EventArgs e)
		{
			throw null;
		}

		protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
		{
			throw null;
		}

		public new Control GetChildAtPoint(Point point)
		{
			throw null;
		}

		public new Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
		{
			throw null;
		}

		public ToolStripItem GetItemAt(int x, int y)
		{
			throw null;
		}

		public ToolStripItem GetItemAt(Point point)
		{
			throw null;
		}

		protected virtual void RestoreFocus()
		{
			throw null;
		}

		public void ResetMinimumSize()
		{
			throw null;
		}

		protected static void SetItemParent(ToolStripItem item, ToolStrip parent)
		{
			throw null;
		}

		protected override void SetVisibleCore(bool visible)
		{
			throw null;
		}

		protected override void Select(bool directed, bool forward)
		{
			throw null;
		}

		public new void SetAutoScrollMargin(int x, int y)
		{
			throw null;
		}

		protected virtual void SetDisplayedItems()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override ControlCollection CreateControlsInstance()
		{
			throw null;
		}
	}
}
