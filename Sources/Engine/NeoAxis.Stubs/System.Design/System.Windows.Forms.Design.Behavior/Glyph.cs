using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
	public abstract class Glyph
	{
		public virtual Behavior Behavior
		{
			get
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

		protected Glyph(Behavior behavior)
		{
			throw null;
		}

		public abstract Cursor GetHitTest(Point p);

		public abstract void Paint(PaintEventArgs pe);

		protected void SetBehavior(Behavior behavior)
		{
			throw null;
		}
	}
}
