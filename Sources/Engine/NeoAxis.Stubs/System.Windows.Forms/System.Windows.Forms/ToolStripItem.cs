using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public abstract class ToolStripItem : Component, IDropTarget, IComponent, IDisposable
	{
		public class ToolStripItemAccessibleObject : AccessibleObject
		{
			public override string DefaultAction
			{
				get
				{
					throw null;
				}
			}

			public override string Description
			{
				get
				{
					throw null;
				}
			}

			public override string Help
			{
				get
				{
					throw null;
				}
			}

			public override string KeyboardShortcut
			{
				get
				{
					throw null;
				}
			}

			public override string Name
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

			public override AccessibleRole Role
			{
				get
				{
					throw null;
				}
			}

			public override AccessibleStates State
			{
				get
				{
					throw null;
				}
			}

			public override Rectangle Bounds
			{
				get
				{
					throw null;
				}
			}

			public override AccessibleObject Parent
			{
				get
				{
					throw null;
				}
			}

			public ToolStripItemAccessibleObject(ToolStripItem ownerItem)
			{
				throw null;
			}

			public override void DoDefaultAction()
			{
				throw null;
			}

			public override int GetHelpTopic(out string fileName)
			{
				throw null;
			}

			public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
			{
				throw null;
			}

			public void AddState(AccessibleStates state)
			{
				throw null;
			}

			public override string ToString()
			{
				throw null;
			}
		}

		public AccessibleObject AccessibilityObject
		{
			get
			{
				throw null;
			}
		}

		public string AccessibleDefaultActionDescription
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

		public string AccessibleDescription
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

		public string AccessibleName
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

		public AccessibleRole AccessibleRole
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

		public ToolStripItemAlignment Alignment
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

		public virtual bool AllowDrop
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

		public bool AutoSize
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

		public bool AutoToolTip
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

		public bool Available
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

		public virtual Image BackgroundImage
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

		public virtual ImageLayout BackgroundImageLayout
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

		public virtual Color BackColor
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

		public virtual Rectangle Bounds
		{
			get
			{
				throw null;
			}
		}

		public Rectangle ContentRectangle
		{
			get
			{
				throw null;
			}
		}

		public virtual bool CanSelect
		{
			get
			{
				throw null;
			}
		}

		public AnchorStyles Anchor
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

		public DockStyle Dock
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

		protected virtual bool DefaultAutoToolTip
		{
			get
			{
				throw null;
			}
		}

		protected virtual Padding DefaultPadding
		{
			get
			{
				throw null;
			}
		}

		protected virtual Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		protected virtual ToolStripItemDisplayStyle DefaultDisplayStyle
		{
			get
			{
				throw null;
			}
		}

		public virtual ToolStripItemDisplayStyle DisplayStyle
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

		public bool DoubleClickEnabled
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

		public virtual bool Enabled
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

		public virtual Color ForeColor
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

		public virtual Font Font
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

		public int Height
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

		public ContentAlignment ImageAlign
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

		public virtual Image Image
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

		public Color ImageTransparentColor
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

		public int ImageIndex
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

		public string ImageKey
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

		public ToolStripItemImageScaling ImageScaling
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

		public bool IsDisposed
		{
			get
			{
				throw null;
			}
		}

		public bool IsOnDropDown
		{
			get
			{
				throw null;
			}
		}

		public bool IsOnOverflow
		{
			get
			{
				throw null;
			}
		}

		public Padding Margin
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

		public MergeAction MergeAction
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

		public int MergeIndex
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

		public ToolStrip Owner
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

		public ToolStripItem OwnerItem
		{
			get
			{
				throw null;
			}
		}

		public ToolStripItemOverflow Overflow
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

		public virtual Padding Padding
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

		public ToolStripItemPlacement Placement
		{
			get
			{
				throw null;
			}
		}

		public virtual bool Pressed
		{
			get
			{
				throw null;
			}
		}

		public virtual RightToLeft RightToLeft
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

		public bool RightToLeftAutoMirrorImage
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

		public virtual bool Selected
		{
			get
			{
				throw null;
			}
		}

		public virtual Size Size
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

		public virtual string Text
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

		public virtual ContentAlignment TextAlign
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

		public TextImageRelation TextImageRelation
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

		public string ToolTipText
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

		public bool Visible
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

		public int Width
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

		public event EventHandler AvailableChanged
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

		public event EventHandler BackColorChanged
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

		public event EventHandler Click
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

		public event EventHandler DisplayStyleChanged
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

		public event EventHandler DoubleClick
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

		public event DragEventHandler DragDrop
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

		public event DragEventHandler DragEnter
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

		public event DragEventHandler DragOver
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

		public event EventHandler DragLeave
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

		public event EventHandler EnabledChanged
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

		public event EventHandler ForeColorChanged
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

		public event GiveFeedbackEventHandler GiveFeedback
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

		public event EventHandler LocationChanged
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

		public event MouseEventHandler MouseDown
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

		public event EventHandler MouseEnter
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

		public event EventHandler MouseLeave
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

		public event EventHandler MouseHover
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

		public event MouseEventHandler MouseMove
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

		public event MouseEventHandler MouseUp
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

		public event EventHandler OwnerChanged
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

		public event PaintEventHandler Paint
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

		public event QueryContinueDragEventHandler QueryContinueDrag
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

		public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
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

		public event EventHandler RightToLeftChanged
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

		public event EventHandler TextChanged
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

		public event EventHandler VisibleChanged
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

		protected ToolStripItem()
		{
			throw null;
		}

		protected ToolStripItem(string text, Image image, EventHandler onClick)
		{
			throw null;
		}

		protected ToolStripItem(string text, Image image, EventHandler onClick, string name)
		{
			throw null;
		}

		protected virtual AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
		{
			throw null;
		}

		public ToolStrip GetCurrentParent()
		{
			throw null;
		}

		public virtual Size GetPreferredSize(Size constrainingSize)
		{
			throw null;
		}

		public void Invalidate()
		{
			throw null;
		}

		public void Invalidate(Rectangle r)
		{
			throw null;
		}

		protected virtual void OnBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnBoundsChanged()
		{
			throw null;
		}

		protected virtual void OnClick(EventArgs e)
		{
			throw null;
		}

		void IDropTarget.OnDragEnter(DragEventArgs dragEvent)
		{
			throw null;
		}

		void IDropTarget.OnDragOver(DragEventArgs dragEvent)
		{
			throw null;
		}

		void IDropTarget.OnDragLeave(EventArgs e)
		{
			throw null;
		}

		void IDropTarget.OnDragDrop(DragEventArgs dragEvent)
		{
			throw null;
		}

		protected virtual void OnAvailableChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDragEnter(DragEventArgs dragEvent)
		{
			throw null;
		}

		protected virtual void OnDragOver(DragEventArgs dragEvent)
		{
			throw null;
		}

		protected virtual void OnDragLeave(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDragDrop(DragEventArgs dragEvent)
		{
			throw null;
		}

		protected virtual void OnDisplayStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEvent)
		{
			throw null;
		}

		protected virtual void OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEvent)
		{
			throw null;
		}

		protected virtual void OnDoubleClick(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnLocationChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseEnter(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseMove(MouseEventArgs mea)
		{
			throw null;
		}

		protected virtual void OnMouseHover(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseLeave(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseDown(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseUp(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnPaint(PaintEventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
		{
			throw null;
		}

		protected virtual void OnParentForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnOwnerChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRightToLeftChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnTextChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnVisibleChanged(EventArgs e)
		{
			throw null;
		}

		public void PerformClick()
		{
			throw null;
		}

		public void Select()
		{
			throw null;
		}

		protected virtual void SetVisibleCore(bool visible)
		{
			throw null;
		}

		public virtual void ResetBackColor()
		{
			throw null;
		}

		public virtual void ResetDisplayStyle()
		{
			throw null;
		}

		public virtual void ResetForeColor()
		{
			throw null;
		}

		public virtual void ResetFont()
		{
			throw null;
		}

		public virtual void ResetImage()
		{
			throw null;
		}

		public void ResetMargin()
		{
			throw null;
		}

		public void ResetPadding()
		{
			throw null;
		}

		public virtual void ResetRightToLeft()
		{
			throw null;
		}

		public virtual void ResetTextDirection()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
