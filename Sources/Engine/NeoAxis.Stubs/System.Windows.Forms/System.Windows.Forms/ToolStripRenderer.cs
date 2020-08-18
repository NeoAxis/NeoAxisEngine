using System.Drawing;

namespace System.Windows.Forms
{
	public abstract class ToolStripRenderer
	{
		public event ToolStripArrowRenderEventHandler RenderArrow
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

		public event ToolStripRenderEventHandler RenderToolStripBackground
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

		public event ToolStripPanelRenderEventHandler RenderToolStripPanelBackground
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

		public event ToolStripContentPanelRenderEventHandler RenderToolStripContentPanelBackground
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

		public event ToolStripRenderEventHandler RenderToolStripBorder
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

		public event ToolStripItemRenderEventHandler RenderButtonBackground
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

		public event ToolStripItemRenderEventHandler RenderDropDownButtonBackground
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

		public event ToolStripItemRenderEventHandler RenderOverflowButtonBackground
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

		public event ToolStripGripRenderEventHandler RenderGrip
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

		public event ToolStripItemRenderEventHandler RenderItemBackground
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

		public event ToolStripItemImageRenderEventHandler RenderItemImage
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

		public event ToolStripItemImageRenderEventHandler RenderItemCheck
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

		public event ToolStripItemTextRenderEventHandler RenderItemText
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

		public event ToolStripRenderEventHandler RenderImageMargin
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

		public event ToolStripItemRenderEventHandler RenderLabelBackground
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

		public event ToolStripItemRenderEventHandler RenderMenuItemBackground
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

		public event ToolStripItemRenderEventHandler RenderToolStripStatusLabelBackground
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

		public event ToolStripRenderEventHandler RenderStatusStripSizingGrip
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

		public event ToolStripItemRenderEventHandler RenderSplitButtonBackground
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

		public event ToolStripSeparatorRenderEventHandler RenderSeparator
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

		protected ToolStripRenderer()
		{
			throw null;
		}

		public static Image CreateDisabledImage(Image normalImage)
		{
			throw null;
		}

		public void DrawArrow(ToolStripArrowRenderEventArgs e)
		{
			throw null;
		}

		public void DrawToolStripBackground(ToolStripRenderEventArgs e)
		{
			throw null;
		}

		public void DrawGrip(ToolStripGripRenderEventArgs e)
		{
			throw null;
		}

		public void DrawItemBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		public void DrawImageMargin(ToolStripRenderEventArgs e)
		{
			throw null;
		}

		public void DrawLabelBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		public void DrawButtonBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		public void DrawToolStripBorder(ToolStripRenderEventArgs e)
		{
			throw null;
		}

		public void DrawDropDownButtonBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		public void DrawOverflowButtonBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		public void DrawItemImage(ToolStripItemImageRenderEventArgs e)
		{
			throw null;
		}

		public void DrawItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			throw null;
		}

		public void DrawItemText(ToolStripItemTextRenderEventArgs e)
		{
			throw null;
		}

		public void DrawMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		public void DrawSplitButton(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		public void DrawToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		public void DrawStatusStripSizingGrip(ToolStripRenderEventArgs e)
		{
			throw null;
		}

		public void DrawSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			throw null;
		}

		public void DrawToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
		{
			throw null;
		}

		public void DrawToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
		{
			throw null;
		}

		protected static void ScaleArrowOffsetsIfNeeded()
		{
			throw null;
		}

		protected static void ScaleArrowOffsetsIfNeeded(int dpi)
		{
			throw null;
		}

		protected virtual void OnRenderArrow(ToolStripArrowRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderGrip(ToolStripGripRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderImageMargin(ToolStripRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
		{
			throw null;
		}
	}
}
