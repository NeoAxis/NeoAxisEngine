using System.ComponentModel;

namespace System.Windows.Forms
{
	public abstract class FileDialog : CommonDialog
	{
		public bool AddExtension
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

		public virtual bool CheckFileExists
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

		public bool CheckPathExists
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

		public string DefaultExt
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

		public bool DereferenceLinks
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

		public string FileName
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

		public string[] FileNames
		{
			get
			{
				throw null;
			}
		}

		public string Filter
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

		public int FilterIndex
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

		public string InitialDirectory
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

		protected virtual IntPtr Instance
		{
			get
			{
				throw null;
			}
		}

		protected int Options
		{
			get
			{
				throw null;
			}
		}

		public bool RestoreDirectory
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

		public bool ShowHelp
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

		public bool SupportMultiDottedExtensions
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

		public string Title
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

		public bool ValidateNames
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

		public FileDialogCustomPlacesCollection CustomPlaces
		{
			get
			{
				throw null;
			}
		}

		public bool AutoUpgradeEnabled
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

		public event CancelEventHandler FileOk
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
		{
			throw null;
		}

		protected void OnFileOk(CancelEventArgs e)
		{
			throw null;
		}

		public override void Reset()
		{
			throw null;
		}

		protected override bool RunDialog(IntPtr hWndOwner)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
