using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
	public class ParentControlDesigner : ControlDesigner
	{
		protected virtual bool AllowControlLasso
		{
			get
			{
				throw null;
			}
		}

		protected virtual bool AllowGenericDragBox
		{
			get
			{
				throw null;
			}
		}

		protected virtual Point DefaultControlLocation
		{
			get
			{
				throw null;
			}
		}

		protected virtual bool DrawGrid
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

		protected override bool EnableDragRect
		{
			get
			{
				throw null;
			}
		}

		protected Size GridSize
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

		protected ToolboxItem MouseDragTool
		{
			get
			{
				throw null;
			}
		}

		public override IList SnapLines
		{
			get
			{
				throw null;
			}
		}

		protected virtual Control GetParentForComponent(IComponent component)
		{
			throw null;
		}

		protected void AddPaddingSnapLines(ref ArrayList snapLines)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		protected static void InvokeCreateTool(ParentControlDesigner toInvoke, ToolboxItem tool)
		{
			throw null;
		}

		public virtual bool CanParent(ControlDesigner controlDesigner)
		{
			throw null;
		}

		public virtual bool CanParent(Control control)
		{
			throw null;
		}

		protected void CreateTool(ToolboxItem tool)
		{
			throw null;
		}

		protected void CreateTool(ToolboxItem tool, Point location)
		{
			throw null;
		}

		protected void CreateTool(ToolboxItem tool, Rectangle bounds)
		{
			throw null;
		}

		protected virtual IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize)
		{
			throw null;
		}

		protected Control GetControl(object component)
		{
			throw null;
		}

		protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
		{
			throw null;
		}

		public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
		{
			throw null;
		}

		protected Rectangle GetUpdatedRect(Rectangle originalRect, Rectangle dragRect, bool updateSize)
		{
			throw null;
		}

		public override void Initialize(IComponent component)
		{
			throw null;
		}

		public override void InitializeNewComponent(IDictionary defaultValues)
		{
			throw null;
		}

		protected override void OnDragComplete(DragEventArgs de)
		{
			throw null;
		}

		protected override void OnDragDrop(DragEventArgs de)
		{
			throw null;
		}

		protected override void OnDragEnter(DragEventArgs de)
		{
			throw null;
		}

		protected override void OnDragLeave(EventArgs e)
		{
			throw null;
		}

		protected override void OnDragOver(DragEventArgs de)
		{
			throw null;
		}

		protected override void OnMouseDragBegin(int x, int y)
		{
			throw null;
		}

		protected override void OnMouseDragEnd(bool cancel)
		{
			throw null;
		}

		protected override void OnMouseDragMove(int x, int y)
		{
			throw null;
		}

		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			throw null;
		}

		protected override void OnSetCursor()
		{
			throw null;
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			throw null;
		}

		public ParentControlDesigner()
		{
			throw null;
		}
	}
}
