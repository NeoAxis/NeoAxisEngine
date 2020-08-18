using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
	public class DocumentDesigner : ScrollableControlDesigner, IRootDesigner, IDesigner, IDisposable, IToolboxUser
	{
		public override SelectionRules SelectionRules
		{
			get
			{
				throw null;
			}
		}

		ViewTechnology[] SupportedTechnologies
		{
			get
			{
				throw null;
			}
		}

		ViewTechnology[] IRootDesigner.SupportedTechnologies
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
		{
			throw null;
		}

		protected virtual bool GetToolSupported(ToolboxItem tool)
		{
			throw null;
		}

		public override void Initialize(IComponent component)
		{
			throw null;
		}

		protected override void OnContextMenu(int x, int y)
		{
			throw null;
		}

		protected override void OnCreateHandle()
		{
			throw null;
		}

		protected virtual void EnsureMenuEditorService(IComponent c)
		{
			throw null;
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			throw null;
		}

		protected virtual void ToolPicked(ToolboxItem tool)
		{
			throw null;
		}

		object IRootDesigner.GetView(ViewTechnology technology)
		{
			throw null;
		}

		bool IToolboxUser.GetToolSupported(ToolboxItem tool)
		{
			throw null;
		}

		void IToolboxUser.ToolPicked(ToolboxItem tool)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}

		public DocumentDesigner()
		{
			throw null;
		}
	}
}
