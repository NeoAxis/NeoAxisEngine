using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
	public class FolderNameEditor : System.Drawing.Design.UITypeEditor
	{
		protected sealed class FolderBrowser : Component
		{
			public FolderBrowserStyles Style
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

			public string DirectoryPath
			{
				get
				{
					throw null;
				}
			}

			public FolderBrowserFolder StartLocation
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

			public string Description
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

			public DialogResult ShowDialog()
			{
				throw null;
			}

			public DialogResult ShowDialog(IWin32Window owner)
			{
				throw null;
			}

			public FolderBrowser()
			{
				throw null;
			}
		}

		protected enum FolderBrowserFolder
		{
			Desktop = 0,
			Favorites = 6,
			MyComputer = 17,
			MyDocuments = 5,
			MyPictures = 39,
			NetAndDialUpConnections = 49,
			NetworkNeighborhood = 18,
			Printers = 4,
			Recent = 8,
			SendTo = 9,
			StartMenu = 11,
			Templates = 21
		}

		protected enum FolderBrowserStyles
		{
			BrowseForComputer = 0x1000,
			BrowseForEverything = 0x4000,
			BrowseForPrinter = 0x2000,
			RestrictToDomain = 2,
			RestrictToFilesystem = 1,
			RestrictToSubfolders = 8,
			ShowTextBox = 0x10
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			throw null;
		}

		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			throw null;
		}

		protected virtual void InitializeDialog(FolderBrowser folderBrowser)
		{
			throw null;
		}

		public FolderNameEditor()
		{
			throw null;
		}
	}
}
