using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
	public class ControlDesigner : ComponentDesigner
	{
		public class ControlDesignerAccessibleObject : AccessibleObject
		{
			public override Rectangle Bounds
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

			public override string DefaultAction
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
			}

			public override AccessibleObject Parent
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

			public override AccessibleStates State
			{
				get
				{
					throw null;
				}
			}

			public override string Value
			{
				get
				{
					throw null;
				}
			}

			public ControlDesignerAccessibleObject(ControlDesigner designer, Control control)
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

			public override AccessibleObject GetFocused()
			{
				throw null;
			}

			public override AccessibleObject GetSelected()
			{
				throw null;
			}

			public override AccessibleObject HitTest(int x, int y)
			{
				throw null;
			}
		}

		protected BehaviorService BehaviorService
		{
			get
			{
				throw null;
			}
		}

		public override ICollection AssociatedComponents
		{
			get
			{
				throw null;
			}
		}

		public virtual AccessibleObject AccessibilityObject
		{
			get
			{
				throw null;
			}
		}

		public virtual Control Control
		{
			get
			{
				throw null;
			}
		}

		protected virtual bool EnableDragRect
		{
			get
			{
				throw null;
			}
		}

		protected override IComponent ParentComponent
		{
			get
			{
				throw null;
			}
		}

		public virtual bool ParticipatesWithSnapLines
		{
			get
			{
				throw null;
			}
		}

		public bool AutoResizeHandles
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

		public virtual SelectionRules SelectionRules
		{
			get
			{
				throw null;
			}
		}

		public virtual IList SnapLines
		{
			get
			{
				throw null;
			}
		}

		protected override InheritanceAttribute InheritanceAttribute
		{
			get
			{
				throw null;
			}
		}

		public virtual int NumberOfInternalControlDesigners()
		{
			throw null;
		}

		public virtual ControlDesigner InternalControlDesigner(int internalControlIndex)
		{
			throw null;
		}

		protected void BaseWndProc(ref Message m)
		{
			throw null;
		}

		public virtual bool CanBeParentedTo(IDesigner parentDesigner)
		{
			throw null;
		}

		protected void DefWndProc(ref Message m)
		{
			throw null;
		}

		protected void DisplayError(Exception e)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		protected bool EnableDesignMode(Control child, string name)
		{
			throw null;
		}

		protected void EnableDragDrop(bool value)
		{
			throw null;
		}

		protected virtual ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
		{
			throw null;
		}

		public virtual GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
		{
			throw null;
		}

		protected virtual bool GetHitTest(Point point)
		{
			throw null;
		}

		protected void HookChildControls(Control firstChild)
		{
			throw null;
		}

		public override void Initialize(IComponent component)
		{
			throw null;
		}

		public override void InitializeExistingComponent(IDictionary defaultValues)
		{
			throw null;
		}

		public override void InitializeNewComponent(IDictionary defaultValues)
		{
			throw null;
		}

		public override void OnSetComponentDefaults()
		{
			throw null;
		}

		protected virtual void OnContextMenu(int x, int y)
		{
			throw null;
		}

		protected virtual void OnCreateHandle()
		{
			throw null;
		}

		protected virtual void OnDragEnter(DragEventArgs de)
		{
			throw null;
		}

		protected virtual void OnDragComplete(DragEventArgs de)
		{
			throw null;
		}

		protected virtual void OnDragDrop(DragEventArgs de)
		{
			throw null;
		}

		protected virtual void OnDragLeave(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDragOver(DragEventArgs de)
		{
			throw null;
		}

		protected virtual void OnGiveFeedback(GiveFeedbackEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseDragBegin(int x, int y)
		{
			throw null;
		}

		protected virtual void OnMouseDragEnd(bool cancel)
		{
			throw null;
		}

		protected virtual void OnMouseDragMove(int x, int y)
		{
			throw null;
		}

		protected virtual void OnMouseEnter()
		{
			throw null;
		}

		protected virtual void OnMouseHover()
		{
			throw null;
		}

		protected virtual void OnMouseLeave()
		{
			throw null;
		}

		protected virtual void OnPaintAdornments(PaintEventArgs pe)
		{
			throw null;
		}

		protected virtual void OnSetCursor()
		{
			throw null;
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			throw null;
		}

		protected void UnhookChildControls(Control firstChild)
		{
			throw null;
		}

		protected virtual void WndProc(ref Message m)
		{
			throw null;
		}

		public ControlDesigner()
		{
			throw null;
		}
	}
}
