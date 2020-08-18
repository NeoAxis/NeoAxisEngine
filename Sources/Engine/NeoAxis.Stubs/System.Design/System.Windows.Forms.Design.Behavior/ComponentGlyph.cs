using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
	public class ComponentGlyph : Glyph
	{
		public IComponent RelatedComponent
		{
			get
			{
				throw null;
			}
		}

		public ComponentGlyph(IComponent relatedComponent, Behavior behavior)
			:base(behavior)
		{
			throw null;
		}

		public ComponentGlyph(IComponent relatedComponent)
			:base(null)
		{
			throw null;
		}

		public override Cursor GetHitTest(Point p)
		{
			throw null;
		}

		public override void Paint(PaintEventArgs pe)
		{
			throw null;
		}
	}
}
