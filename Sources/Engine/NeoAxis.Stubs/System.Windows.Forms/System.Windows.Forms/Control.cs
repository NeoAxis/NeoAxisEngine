using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
	public class Control : Component, IDropTarget, ISynchronizeInvoke, IWin32Window, IComponent, IDisposable, IBindableComponent
	{
		public class ControlCollection : ArrangedElementCollection, IList, ICollection, IEnumerable, ICloneable
		{
			public Control Owner
			{
				get
				{
					throw null;
				}
			}

			public virtual Control this[int index]
			{
				get
				{
					throw null;
				}
			}

			public virtual Control this[string key]
			{
				get
				{
					throw null;
				}
			}

			public ControlCollection(Control owner)
			{
				throw null;
			}

			public virtual bool ContainsKey(string key)
			{
				throw null;
			}

			public virtual void Add(Control value)
			{
				throw null;
			}

			int IList.Add(object control)
			{
				throw null;
			}

			public virtual void AddRange(Control[] controls)
			{
				throw null;
			}

			object ICloneable.Clone()
			{
				throw null;
			}

			public bool Contains(Control control)
			{
				throw null;
			}

			public Control[] Find(string key, bool searchAllChildren)
			{
				throw null;
			}

			public override IEnumerator GetEnumerator()
			{
				throw null;
			}

			public int IndexOf(Control control)
			{
				throw null;
			}

			public virtual int IndexOfKey(string key)
			{
				throw null;
			}

			public virtual void Remove(Control value)
			{
				throw null;
			}

			void IList.Remove(object control)
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

			public virtual void Clear()
			{
				throw null;
			}

			public int GetChildIndex(Control child)
			{
				throw null;
			}

			public virtual int GetChildIndex(Control child, bool throwException)
			{
				throw null;
			}

			public virtual void SetChildIndex(Control child, int newIndex)
			{
				throw null;
			}
		}

		public class ControlAccessibleObject : AccessibleObject
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

			public IntPtr Handle
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

			public override AccessibleObject Parent
			{
				get
				{
					throw null;
				}
			}

			public Control Owner
			{
				get
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

			public ControlAccessibleObject(Control ownerControl)
			{
				throw null;
			}

			public override int GetHelpTopic(out string fileName)
			{
				throw null;
			}

			public void NotifyClients(AccessibleEvents accEvent)
			{
				throw null;
			}

			public void NotifyClients(AccessibleEvents accEvent, int childID)
			{
				throw null;
			}

			public void NotifyClients(AccessibleEvents accEvent, int objectID, int childID)
			{
				throw null;
			}

			public override bool RaiseLiveRegionChanged()
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

		public virtual AnchorStyles Anchor
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

		public virtual bool AutoSize
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

		public virtual Point AutoScrollOffset
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

		public virtual LayoutEngine LayoutEngine
		{
			get
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

		public virtual BindingContext BindingContext
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

		public int Bottom
		{
			get
			{
				throw null;
			}
		}

		public Rectangle Bounds
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

		public bool CanFocus
		{
			get
			{
				throw null;
			}
		}

		protected override bool CanRaiseEvents
		{
			get
			{
				throw null;
			}
		}

		public bool CanSelect
		{
			get
			{
				throw null;
			}
		}

		public bool Capture
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

		public bool CausesValidation
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

		public static bool CheckForIllegalCrossThreadCalls
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

		public Rectangle ClientRectangle
		{
			get
			{
				throw null;
			}
		}

		public Size ClientSize
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

		public string CompanyName
		{
			get
			{
				throw null;
			}
		}

		public bool ContainsFocus
		{
			get
			{
				throw null;
			}
		}

		public virtual ContextMenu ContextMenu
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

		public virtual ContextMenuStrip ContextMenuStrip
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

		public ControlCollection Controls
		{
			get
			{
				throw null;
			}
		}

		public bool Created
		{
			get
			{
				throw null;
			}
		}

		protected virtual CreateParams CreateParams
		{
			get
			{
				throw null;
			}
		}

		public virtual Cursor Cursor
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

		public ControlBindingsCollection DataBindings
		{
			get
			{
				throw null;
			}
		}

		public static Color DefaultBackColor
		{
			get
			{
				throw null;
			}
		}

		protected virtual Cursor DefaultCursor
		{
			get
			{
				throw null;
			}
		}

		public static Font DefaultFont
		{
			get
			{
				throw null;
			}
		}

		public static Color DefaultForeColor
		{
			get
			{
				throw null;
			}
		}

		protected virtual Padding DefaultMargin
		{
			get
			{
				throw null;
			}
		}

		protected virtual Size DefaultMaximumSize
		{
			get
			{
				throw null;
			}
		}

		protected virtual Size DefaultMinimumSize
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

		public int DeviceDpi
		{
			get
			{
				throw null;
			}
		}

		public virtual Rectangle DisplayRectangle
		{
			get
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

		public bool Disposing
		{
			get
			{
				throw null;
			}
		}

		public virtual DockStyle Dock
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

		protected virtual bool DoubleBuffered
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

		public virtual bool Focused
		{
			get
			{
				throw null;
			}
		}

		public virtual Font Font
		{
			[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Windows.Forms.Control+ActiveXFontMarshaler, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
			get
			{
				throw null;
			}
			[param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Windows.Forms.Control+ActiveXFontMarshaler, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
			set
			{
				throw null;
			}
		}

		protected int FontHeight
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

		public IntPtr Handle
		{
			get
			{
				throw null;
			}
		}

		public bool HasChildren
		{
			get
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

		public bool IsHandleCreated
		{
			get
			{
				throw null;
			}
		}

		public bool InvokeRequired
		{
			get
			{
				throw null;
			}
		}

		public bool IsAccessible
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

		public bool IsMirrored
		{
			get
			{
				throw null;
			}
		}

		public int Left
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

		public Point Location
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

		public virtual Size MaximumSize
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

		public virtual Size MinimumSize
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

		public static Keys ModifierKeys
		{
			get
			{
				throw null;
			}
		}

		public static MouseButtons MouseButtons
		{
			get
			{
				throw null;
			}
		}

		public static Point MousePosition
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

		public Control Parent
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

		public string ProductName
		{
			get
			{
				throw null;
			}
		}

		public string ProductVersion
		{
			get
			{
				throw null;
			}
		}

		public bool RecreatingHandle
		{
			get
			{
				throw null;
			}
		}

		public Region Region
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

		protected bool ResizeRedraw
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

		public int Right
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

		protected virtual bool ScaleChildren
		{
			get
			{
				throw null;
			}
		}

		public override ISite Site
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

		public int TabIndex
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

		public bool TabStop
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

		public int Top
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

		public Control TopLevelControl
		{
			get
			{
				throw null;
			}
		}

		public bool UseWaitCursor
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

		public IWindowTarget WindowTarget
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

		public Size PreferredSize
		{
			get
			{
				throw null;
			}
		}

		public Padding Padding
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

		protected virtual bool CanEnableIme
		{
			get
			{
				throw null;
			}
		}

		protected virtual ImeMode DefaultImeMode
		{
			get
			{
				throw null;
			}
		}

		public ImeMode ImeMode
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

		protected virtual ImeMode ImeModeBase
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

		protected static ImeMode PropagatingImeMode
		{
			get
			{
				throw null;
			}
		}

		public event EventHandler AutoSizeChanged
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

		public event EventHandler BackgroundImageChanged
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

		public event EventHandler BackgroundImageLayoutChanged
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

		public event EventHandler BindingContextChanged
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

		public event EventHandler CausesValidationChanged
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

		public event EventHandler ClientSizeChanged
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

		public event EventHandler ContextMenuChanged
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

		public event EventHandler ContextMenuStripChanged
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

		public event EventHandler CursorChanged
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

		public event EventHandler DockChanged
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

		public event EventHandler FontChanged
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

		public event EventHandler MarginChanged
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

		public event EventHandler RegionChanged
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

		public event EventHandler SizeChanged
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

		public event EventHandler TabIndexChanged
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

		public event EventHandler TabStopChanged
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

		public event ControlEventHandler ControlAdded
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

		public event ControlEventHandler ControlRemoved
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

		public event EventHandler HandleCreated
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

		public event EventHandler HandleDestroyed
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

		public event HelpEventHandler HelpRequested
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

		public event InvalidateEventHandler Invalidated
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

		public event EventHandler PaddingChanged
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

		public event EventHandler Enter
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

		public event EventHandler GotFocus
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

		public event KeyEventHandler KeyDown
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

		public event KeyPressEventHandler KeyPress
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

		public event KeyEventHandler KeyUp
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

		public event LayoutEventHandler Layout
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

		public event EventHandler Leave
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

		public event EventHandler LostFocus
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

		public event MouseEventHandler MouseClick
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

		public event MouseEventHandler MouseDoubleClick
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

		public event EventHandler MouseCaptureChanged
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

		public event EventHandler DpiChangedBeforeParent
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

		public event EventHandler DpiChangedAfterParent
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

		public event MouseEventHandler MouseWheel
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

		public event EventHandler Move
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

		public event PreviewKeyDownEventHandler PreviewKeyDown
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

		public event EventHandler Resize
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

		public event UICuesEventHandler ChangeUICues
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

		public event EventHandler StyleChanged
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

		public event EventHandler SystemColorsChanged
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

		public event CancelEventHandler Validating
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

		public event EventHandler Validated
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

		public event EventHandler ParentChanged
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

		public event EventHandler ImeModeChanged
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

		public Control()
		{
			throw null;
		}

		public Control(string text)
		{
			throw null;
		}

		public Control(string text, int left, int top, int width, int height)
		{
			throw null;
		}

		public Control(Control parent, string text)
		{
			throw null;
		}

		public Control(Control parent, string text, int left, int top, int width, int height)
		{
			throw null;
		}

		protected virtual AccessibleObject GetAccessibilityObjectById(int objectId)
		{
			throw null;
		}

		protected void SetAutoSizeMode(AutoSizeMode mode)
		{
			throw null;
		}

		protected AutoSizeMode GetAutoSizeMode()
		{
			throw null;
		}

		public void ResetBindings()
		{
			throw null;
		}

		public virtual Size GetPreferredSize(Size proposedSize)
		{
			throw null;
		}

		protected void AccessibilityNotifyClients(AccessibleEvents accEvent, int objectID, int childID)
		{
			throw null;
		}

		public IAsyncResult BeginInvoke(Delegate method)
		{
			throw null;
		}

		public IAsyncResult BeginInvoke(Delegate method, params object[] args)
		{
			throw null;
		}

		public void BringToFront()
		{
			throw null;
		}

		public bool Contains(Control ctl)
		{
			throw null;
		}

		protected virtual AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected virtual ControlCollection CreateControlsInstance()
		{
			throw null;
		}

		public Graphics CreateGraphics()
		{
			throw null;
		}

		protected virtual void CreateHandle()
		{
			throw null;
		}

		public void CreateControl()
		{
			throw null;
		}

		protected virtual void DefWndProc(ref Message m)
		{
			throw null;
		}

		protected virtual void DestroyHandle()
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

		public void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
		{
			throw null;
		}

		public object EndInvoke(IAsyncResult asyncResult)
		{
			throw null;
		}

		public Form FindForm()
		{
			throw null;
		}

		protected bool GetTopLevel()
		{
			throw null;
		}

		protected void RaiseKeyEvent(object key, KeyEventArgs e)
		{
			throw null;
		}

		protected void RaiseMouseEvent(object key, MouseEventArgs e)
		{
			throw null;
		}

		public bool Focus()
		{
			throw null;
		}

		public static Control FromChildHandle(IntPtr handle)
		{
			throw null;
		}

		public static Control FromHandle(IntPtr handle)
		{
			throw null;
		}

		public Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
		{
			throw null;
		}

		public Control GetChildAtPoint(Point pt)
		{
			throw null;
		}

		public IContainerControl GetContainerControl()
		{
			throw null;
		}

		protected virtual Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
		{
			throw null;
		}

		public Control GetNextControl(Control ctl, bool forward)
		{
			throw null;
		}

		protected bool GetStyle(ControlStyles flag)
		{
			throw null;
		}

		public void Hide()
		{
			throw null;
		}

		protected virtual void InitLayout()
		{
			throw null;
		}

		public void Invalidate(Region region)
		{
			throw null;
		}

		public void Invalidate(Region region, bool invalidateChildren)
		{
			throw null;
		}

		public void Invalidate()
		{
			throw null;
		}

		public void Invalidate(bool invalidateChildren)
		{
			throw null;
		}

		public void Invalidate(Rectangle rc)
		{
			throw null;
		}

		public void Invalidate(Rectangle rc, bool invalidateChildren)
		{
			throw null;
		}

		public object Invoke(Delegate method)
		{
			throw null;
		}

		public object Invoke(Delegate method, params object[] args)
		{
			throw null;
		}

		protected void InvokePaint(Control c, PaintEventArgs e)
		{
			throw null;
		}

		protected void InvokePaintBackground(Control c, PaintEventArgs e)
		{
			throw null;
		}

		public static bool IsKeyLocked(Keys keyVal)
		{
			throw null;
		}

		protected virtual bool IsInputChar(char charCode)
		{
			throw null;
		}

		protected virtual bool IsInputKey(Keys keyData)
		{
			throw null;
		}

		public static bool IsMnemonic(char charCode, string text)
		{
			throw null;
		}

		public int LogicalToDeviceUnits(int value)
		{
			throw null;
		}

		public Size LogicalToDeviceUnits(Size value)
		{
			throw null;
		}

		public void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap)
		{
			throw null;
		}

		protected virtual void NotifyInvalidate(Rectangle invalidatedArea)
		{
			throw null;
		}

		protected void InvokeOnClick(Control toInvoke, EventArgs e)
		{
			throw null;
		}

		protected virtual void OnAutoSizeChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnBackgroundImageChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnBackgroundImageLayoutChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnBindingContextChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnCausesValidationChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnContextMenuChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnContextMenuStripChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnCursorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDockChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRightToLeftChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnNotifyMessage(Message m)
		{
			throw null;
		}

		protected virtual void OnParentBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentBackgroundImageChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentBindingContextChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentCursorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentFontChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentRightToLeftChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnPrint(PaintEventArgs e)
		{
			throw null;
		}

		protected virtual void OnTabIndexChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnTabStopChanged(EventArgs e)
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

		protected virtual void OnParentChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnClick(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnClientSizeChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnControlAdded(ControlEventArgs e)
		{
			throw null;
		}

		protected virtual void OnControlRemoved(ControlEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCreateControl()
		{
			throw null;
		}

		protected virtual void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnLocationChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnHandleDestroyed(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDoubleClick(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDragEnter(DragEventArgs drgevent)
		{
			throw null;
		}

		protected virtual void OnDragOver(DragEventArgs drgevent)
		{
			throw null;
		}

		protected virtual void OnDragLeave(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDragDrop(DragEventArgs drgevent)
		{
			throw null;
		}

		protected virtual void OnGiveFeedback(GiveFeedbackEventArgs gfbevent)
		{
			throw null;
		}

		protected virtual void OnEnter(EventArgs e)
		{
			throw null;
		}

		protected void InvokeGotFocus(Control toInvoke, EventArgs e)
		{
			throw null;
		}

		protected virtual void OnGotFocus(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnHelpRequested(HelpEventArgs hevent)
		{
			throw null;
		}

		protected virtual void OnInvalidated(InvalidateEventArgs e)
		{
			throw null;
		}

		protected virtual void OnKeyDown(KeyEventArgs e)
		{
			throw null;
		}

		protected virtual void OnKeyPress(KeyPressEventArgs e)
		{
			throw null;
		}

		protected virtual void OnKeyUp(KeyEventArgs e)
		{
			throw null;
		}

		protected virtual void OnLayout(LayoutEventArgs levent)
		{
			throw null;
		}

		protected virtual void OnLeave(EventArgs e)
		{
			throw null;
		}

		protected void InvokeLostFocus(Control toInvoke, EventArgs e)
		{
			throw null;
		}

		protected virtual void OnLostFocus(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMarginChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseDoubleClick(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseClick(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseCaptureChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseDown(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseEnter(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseLeave(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDpiChangedBeforeParent(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDpiChangedAfterParent(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseHover(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseMove(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseUp(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseWheel(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMove(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnPaint(PaintEventArgs e)
		{
			throw null;
		}

		protected virtual void OnPaddingChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnPaintBackground(PaintEventArgs pevent)
		{
			throw null;
		}

		protected virtual void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent)
		{
			throw null;
		}

		protected virtual void OnRegionChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnResize(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			throw null;
		}

		protected virtual void OnSizeChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnChangeUICues(UICuesEventArgs e)
		{
			throw null;
		}

		protected virtual void OnStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnSystemColorsChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnValidating(CancelEventArgs e)
		{
			throw null;
		}

		protected virtual void OnValidated(EventArgs e)
		{
			throw null;
		}

		protected virtual void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
		{
			throw null;
		}

		public void PerformLayout()
		{
			throw null;
		}

		public void PerformLayout(Control affectedControl, string affectedProperty)
		{
			throw null;
		}

		public Point PointToClient(Point p)
		{
			throw null;
		}

		public Point PointToScreen(Point p)
		{
			throw null;
		}

		public virtual bool PreProcessMessage(ref Message msg)
		{
			throw null;
		}

		public PreProcessControlState PreProcessControlMessage(ref Message msg)
		{
			throw null;
		}

		protected virtual bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			throw null;
		}

		protected virtual bool ProcessDialogChar(char charCode)
		{
			throw null;
		}

		protected virtual bool ProcessDialogKey(Keys keyData)
		{
			throw null;
		}

		protected virtual bool ProcessKeyEventArgs(ref Message m)
		{
			throw null;
		}

		protected virtual bool ProcessKeyPreview(ref Message m)
		{
			throw null;
		}

		protected void RaiseDragEvent(object key, DragEventArgs e)
		{
			throw null;
		}

		protected void RaisePaintEvent(object key, PaintEventArgs e)
		{
			throw null;
		}

		public virtual void ResetBackColor()
		{
			throw null;
		}

		public virtual void ResetCursor()
		{
			throw null;
		}

		public virtual void ResetFont()
		{
			throw null;
		}

		public virtual void ResetForeColor()
		{
			throw null;
		}

		public virtual void ResetRightToLeft()
		{
			throw null;
		}

		protected void RecreateHandle()
		{
			throw null;
		}

		public Rectangle RectangleToClient(Rectangle r)
		{
			throw null;
		}

		public Rectangle RectangleToScreen(Rectangle r)
		{
			throw null;
		}

		protected static bool ReflectMessage(IntPtr hWnd, ref Message m)
		{
			throw null;
		}

		public virtual void Refresh()
		{
			throw null;
		}

		protected void ResetMouseEventArgs()
		{
			throw null;
		}

		public virtual void ResetText()
		{
			throw null;
		}

		public void ResumeLayout()
		{
			throw null;
		}

		public void ResumeLayout(bool performLayout)
		{
			throw null;
		}

		public void Scale(float ratio)
		{
			throw null;
		}

		public void Scale(float dx, float dy)
		{
			throw null;
		}

		public void Scale(SizeF factor)
		{
			throw null;
		}

		protected virtual void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			throw null;
		}

		protected virtual void ScaleCore(float dx, float dy)
		{
			throw null;
		}

		public void Select()
		{
			throw null;
		}

		protected virtual void Select(bool directed, bool forward)
		{
			throw null;
		}

		public bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
		{
			throw null;
		}

		public void SendToBack()
		{
			throw null;
		}

		public void SetBounds(int x, int y, int width, int height)
		{
			throw null;
		}

		public void SetBounds(int x, int y, int width, int height, BoundsSpecified specified)
		{
			throw null;
		}

		protected virtual void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			throw null;
		}

		protected virtual void SetClientSizeCore(int x, int y)
		{
			throw null;
		}

		protected virtual Size SizeFromClientSize(Size clientSize)
		{
			throw null;
		}

		protected void SetStyle(ControlStyles flag, bool value)
		{
			throw null;
		}

		protected void SetTopLevel(bool value)
		{
			throw null;
		}

		protected virtual void SetVisibleCore(bool value)
		{
			throw null;
		}

		protected HorizontalAlignment RtlTranslateAlignment(HorizontalAlignment align)
		{
			throw null;
		}

		protected LeftRightAlignment RtlTranslateAlignment(LeftRightAlignment align)
		{
			throw null;
		}

		protected ContentAlignment RtlTranslateAlignment(ContentAlignment align)
		{
			throw null;
		}

		protected HorizontalAlignment RtlTranslateHorizontal(HorizontalAlignment align)
		{
			throw null;
		}

		protected LeftRightAlignment RtlTranslateLeftRight(LeftRightAlignment align)
		{
			throw null;
		}

		public void Show()
		{
			throw null;
		}

		public void SuspendLayout()
		{
			throw null;
		}

		public void Update()
		{
			throw null;
		}

		protected void UpdateBounds(int x, int y, int width, int height)
		{
			throw null;
		}

		protected void UpdateBounds(int x, int y, int width, int height, int clientWidth, int clientHeight)
		{
			throw null;
		}

		protected void UpdateZOrder()
		{
			throw null;
		}

		protected void UpdateStyles()
		{
			throw null;
		}

		protected virtual void WndProc(ref Message m)
		{
			throw null;
		}

		void IDropTarget.OnDragEnter(DragEventArgs drgEvent)
		{
			throw null;
		}

		void IDropTarget.OnDragOver(DragEventArgs drgEvent)
		{
			throw null;
		}

		void IDropTarget.OnDragLeave(EventArgs e)
		{
			throw null;
		}

		void IDropTarget.OnDragDrop(DragEventArgs drgEvent)
		{
			throw null;
		}

		protected virtual void OnImeModeChanged(EventArgs e)
		{
			throw null;
		}

		public void ResetImeMode()
		{
			throw null;
		}
	}
}
