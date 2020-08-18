using System.ComponentModel;

namespace System.Windows.Forms
{
	public class HelpProvider : Component, IExtenderProvider
	{
		public virtual string HelpNamespace
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

		public HelpProvider()
		{
			throw null;
		}

		public virtual bool CanExtend(object target)
		{
			throw null;
		}

		public virtual string GetHelpKeyword(Control ctl)
		{
			throw null;
		}

		public virtual HelpNavigator GetHelpNavigator(Control ctl)
		{
			throw null;
		}

		public virtual string GetHelpString(Control ctl)
		{
			throw null;
		}

		public virtual bool GetShowHelp(Control ctl)
		{
			throw null;
		}

		public virtual void SetHelpString(Control ctl, string helpString)
		{
			throw null;
		}

		public virtual void SetHelpKeyword(Control ctl, string keyword)
		{
			throw null;
		}

		public virtual void SetHelpNavigator(Control ctl, HelpNavigator navigator)
		{
			throw null;
		}

		public virtual void SetShowHelp(Control ctl, bool value)
		{
			throw null;
		}

		public virtual void ResetShowHelp(Control ctl)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
