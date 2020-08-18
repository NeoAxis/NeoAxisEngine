namespace System.Windows.Forms
{
	public sealed class ToolStripManager
	{
		public static ToolStripRenderer Renderer
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

		public static ToolStripManagerRenderMode RenderMode
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

		public static bool VisualStylesEnabled
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

		public static event EventHandler RendererChanged
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

		public static ToolStrip FindToolStrip(string toolStripName)
		{
			throw null;
		}

		public static void LoadSettings(Form targetForm)
		{
			throw null;
		}

		public static void LoadSettings(Form targetForm, string key)
		{
			throw null;
		}

		public static void SaveSettings(Form sourceForm)
		{
			throw null;
		}

		public static void SaveSettings(Form sourceForm, string key)
		{
			throw null;
		}

		public static bool IsValidShortcut(Keys shortcut)
		{
			throw null;
		}

		public static bool IsShortcutDefined(Keys shortcut)
		{
			throw null;
		}

		public static bool Merge(ToolStrip sourceToolStrip, ToolStrip targetToolStrip)
		{
			throw null;
		}

		public static bool Merge(ToolStrip sourceToolStrip, string targetName)
		{
			throw null;
		}

		public static bool RevertMerge(ToolStrip targetToolStrip)
		{
			throw null;
		}

		public static bool RevertMerge(ToolStrip targetToolStrip, ToolStrip sourceToolStrip)
		{
			throw null;
		}

		public static bool RevertMerge(string targetName)
		{
			throw null;
		}
	}
}
