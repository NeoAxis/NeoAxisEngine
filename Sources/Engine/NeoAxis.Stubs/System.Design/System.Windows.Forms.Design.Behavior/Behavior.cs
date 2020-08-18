using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
	public abstract class Behavior
	{
		public virtual Cursor Cursor
		{
			get
			{
				throw null;
			}
		}

		public virtual bool DisableAllCommands
		{
			get
			{
				throw null;
			}
		}

		protected Behavior()
		{
			throw null;
		}

		protected Behavior(bool callParentBehavior, BehaviorService behaviorService)
		{
			throw null;
		}

		public virtual MenuCommand FindCommand(CommandID commandId)
		{
			throw null;
		}

		public virtual void OnLoseCapture(Glyph g, EventArgs e)
		{
			throw null;
		}

		public virtual bool OnMouseDoubleClick(Glyph g, MouseButtons button, Point mouseLoc)
		{
			throw null;
		}

		public virtual bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
		{
			throw null;
		}

		public virtual bool OnMouseEnter(Glyph g)
		{
			throw null;
		}

		public virtual bool OnMouseHover(Glyph g, Point mouseLoc)
		{
			throw null;
		}

		public virtual bool OnMouseLeave(Glyph g)
		{
			throw null;
		}

		public virtual bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
		{
			throw null;
		}

		public virtual bool OnMouseUp(Glyph g, MouseButtons button)
		{
			throw null;
		}

		public virtual void OnDragDrop(Glyph g, DragEventArgs e)
		{
			throw null;
		}

		public virtual void OnDragEnter(Glyph g, DragEventArgs e)
		{
			throw null;
		}

		public virtual void OnDragLeave(Glyph g, EventArgs e)
		{
			throw null;
		}

		public virtual void OnDragOver(Glyph g, DragEventArgs e)
		{
			throw null;
		}

		public virtual void OnGiveFeedback(Glyph g, GiveFeedbackEventArgs e)
		{
			throw null;
		}

		public virtual void OnQueryContinueDrag(Glyph g, QueryContinueDragEventArgs e)
		{
			throw null;
		}
	}
}
