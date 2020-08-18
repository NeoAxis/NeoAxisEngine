using System.ComponentModel;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
	public class FlowLayoutPanel : Panel, IExtenderProvider
	{
		public override LayoutEngine LayoutEngine
		{
			get
			{
				throw null;
			}
		}

		public FlowDirection FlowDirection
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

		public bool WrapContents
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

		public FlowLayoutPanel()
		{
			throw null;
		}

		bool IExtenderProvider.CanExtend(object obj)
		{
			throw null;
		}

		public bool GetFlowBreak(Control control)
		{
			throw null;
		}

		public void SetFlowBreak(Control control, bool value)
		{
			throw null;
		}
	}
}
