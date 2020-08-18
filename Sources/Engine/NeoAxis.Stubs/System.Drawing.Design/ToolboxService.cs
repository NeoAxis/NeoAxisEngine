using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;

namespace System.Drawing.Design
{
	public abstract class ToolboxService : IToolboxService, IComponentDiscoveryService
	{
		protected abstract CategoryNameCollection CategoryNames
		{
			get;
		}

		protected abstract string SelectedCategory
		{
			get;
			set;
		}

		protected abstract ToolboxItemContainer SelectedItemContainer
		{
			get;
			set;
		}

		CategoryNameCollection IToolboxService.CategoryNames
		{
			get
			{
				throw null;
			}
		}

		string IToolboxService.SelectedCategory
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

		protected virtual ToolboxItemContainer CreateItemContainer(ToolboxItem item, IDesignerHost link)
		{
			throw null;
		}

		protected virtual ToolboxItemContainer CreateItemContainer(IDataObject dataObject)
		{
			throw null;
		}

		protected virtual void FilterChanged()
		{
			throw null;
		}

		protected abstract IList GetItemContainers();

		protected abstract IList GetItemContainers(string categoryName);

		public static ToolboxItem GetToolboxItem(Type toolType)
		{
			throw null;
		}

		public static ToolboxItem GetToolboxItem(Type toolType, bool nonPublic)
		{
			throw null;
		}

		public static ICollection GetToolboxItems(Assembly a, string newCodeBase)
		{
			throw null;
		}

		public static ICollection GetToolboxItems(Assembly a, string newCodeBase, bool throwOnError)
		{
			throw null;
		}

		public static ICollection GetToolboxItems(AssemblyName an)
		{
			throw null;
		}

		public static ICollection GetToolboxItems(AssemblyName an, bool throwOnError)
		{
			throw null;
		}

		protected virtual bool IsItemContainer(IDataObject dataObject, IDesignerHost host)
		{
			throw null;
		}

		protected bool IsItemContainerSupported(ToolboxItemContainer container, IDesignerHost host)
		{
			throw null;
		}

		protected abstract void Refresh();

		protected virtual void SelectedItemContainerUsed()
		{
			throw null;
		}

		protected virtual bool SetCursor()
		{
			throw null;
		}

		public static void UnloadToolboxItems()
		{
			throw null;
		}

		void IToolboxService.AddCreator(ToolboxItemCreatorCallback creator, string format)
		{
			throw null;
		}

		void IToolboxService.AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host)
		{
			throw null;
		}

		void IToolboxService.AddLinkedToolboxItem(ToolboxItem toolboxItem, IDesignerHost host)
		{
			throw null;
		}

		void IToolboxService.AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, IDesignerHost host)
		{
			throw null;
		}

		void IToolboxService.AddToolboxItem(ToolboxItem toolboxItem)
		{
			throw null;
		}

		void IToolboxService.AddToolboxItem(ToolboxItem toolboxItem, string category)
		{
			throw null;
		}

		ToolboxItem IToolboxService.DeserializeToolboxItem(object serializedObject)
		{
			throw null;
		}

		ToolboxItem IToolboxService.DeserializeToolboxItem(object serializedObject, IDesignerHost host)
		{
			throw null;
		}

		ToolboxItem IToolboxService.GetSelectedToolboxItem()
		{
			throw null;
		}

		ToolboxItem IToolboxService.GetSelectedToolboxItem(IDesignerHost host)
		{
			throw null;
		}

		ToolboxItemCollection IToolboxService.GetToolboxItems()
		{
			throw null;
		}

		ToolboxItemCollection IToolboxService.GetToolboxItems(IDesignerHost host)
		{
			throw null;
		}

		ToolboxItemCollection IToolboxService.GetToolboxItems(string category)
		{
			throw null;
		}

		ToolboxItemCollection IToolboxService.GetToolboxItems(string category, IDesignerHost host)
		{
			throw null;
		}

		bool IToolboxService.IsSupported(object serializedObject, IDesignerHost host)
		{
			throw null;
		}

		bool IToolboxService.IsSupported(object serializedObject, ICollection filterAttributes)
		{
			throw null;
		}

		bool IToolboxService.IsToolboxItem(object serializedObject)
		{
			throw null;
		}

		bool IToolboxService.IsToolboxItem(object serializedObject, IDesignerHost host)
		{
			throw null;
		}

		void IToolboxService.Refresh()
		{
			throw null;
		}

		void IToolboxService.RemoveCreator(string format)
		{
			throw null;
		}

		void IToolboxService.RemoveCreator(string format, IDesignerHost host)
		{
			throw null;
		}

		void IToolboxService.RemoveToolboxItem(ToolboxItem toolboxItem)
		{
			throw null;
		}

		void IToolboxService.RemoveToolboxItem(ToolboxItem toolboxItem, string category)
		{
			throw null;
		}

		void IToolboxService.SelectedToolboxItemUsed()
		{
			throw null;
		}

		object IToolboxService.SerializeToolboxItem(ToolboxItem toolboxItem)
		{
			throw null;
		}

		bool IToolboxService.SetCursor()
		{
			throw null;
		}

		void IToolboxService.SetSelectedToolboxItem(ToolboxItem toolboxItem)
		{
			throw null;
		}

		ICollection IComponentDiscoveryService.GetComponentTypes(IDesignerHost designerHost, Type baseType)
		{
			throw null;
		}

		protected ToolboxService()
		{
			throw null;
		}
	}
}
