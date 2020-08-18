namespace System.Windows.Forms.Layout
{
	public abstract class LayoutEngine
	{
		public virtual void InitLayout(object child, BoundsSpecified specified)
		{
			throw null;
		}

		public virtual bool Layout(object container, LayoutEventArgs layoutEventArgs)
		{
			throw null;
		}

		protected LayoutEngine()
		{
			throw null;
		}
	}
}
