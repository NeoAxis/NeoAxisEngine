using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{
	public class InputLanguageChangingEventArgs : CancelEventArgs
	{
		public InputLanguage InputLanguage
		{
			get
			{
				throw null;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				throw null;
			}
		}

		public bool SysCharSet
		{
			get
			{
				throw null;
			}
		}

		public InputLanguageChangingEventArgs(CultureInfo culture, bool sysCharSet)
		{
			throw null;
		}

		public InputLanguageChangingEventArgs(InputLanguage inputLanguage, bool sysCharSet)
		{
			throw null;
		}
	}
}
