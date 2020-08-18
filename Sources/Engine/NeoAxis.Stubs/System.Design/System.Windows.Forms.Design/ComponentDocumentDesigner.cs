using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
	public class ComponentDocumentDesigner : ComponentDesigner, IRootDesigner, IDesigner, IDisposable, IToolboxUser, ITypeDescriptorFilterService
	{
		public Control Control
		{
			get
			{
				throw null;
			}
		}

		public bool TrayAutoArrange
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

		public bool TrayLargeIcon
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

		public ViewTechnology[] SupportedTechnologies
		{
			get
			{
				throw null;
			}
		}

		protected override void Dispose(bool disposing)
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

		protected override void PreFilterProperties(IDictionary properties)
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

		bool ITypeDescriptorFilterService.FilterAttributes(IComponent component, IDictionary attributes)
		{
			throw null;
		}

		bool ITypeDescriptorFilterService.FilterEvents(IComponent component, IDictionary events)
		{
			throw null;
		}

		bool ITypeDescriptorFilterService.FilterProperties(IComponent component, IDictionary properties)
		{
			throw null;
		}

		public ComponentDocumentDesigner()
		{
			throw null;
		}
	}
}
