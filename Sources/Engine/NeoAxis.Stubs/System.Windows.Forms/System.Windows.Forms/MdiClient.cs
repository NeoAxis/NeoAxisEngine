using System.Drawing;

namespace System.Windows.Forms
{
	public sealed class MdiClient : Control
	{
		public new class ControlCollection : Control.ControlCollection
		{
			public ControlCollection(MdiClient owner)
			: base( owner )
			{
				throw null;
			}

			public override void Add(Control value)
			{
				throw null;
			}

			public override void Remove(Control value)
			{
				throw null;
			}
		}

		public override Image BackgroundImage
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

		public override ImageLayout BackgroundImageLayout
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

		protected override CreateParams CreateParams
		{
			get
			{
				throw null;
			}
		}

		public Form[] MdiChildren
		{
			get
			{
				throw null;
			}
		}

		public MdiClient()
		{
			throw null;
		}

		protected override Control.ControlCollection CreateControlsInstance()
		{
			throw null;
		}

		public void LayoutMdi(MdiLayout value)
		{
			throw null;
		}

		protected override void OnResize(EventArgs e)
		{
			throw null;
		}

		protected override void ScaleCore(float dx, float dy)
		{
			throw null;
		}

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			throw null;
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
