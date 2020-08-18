using System.ComponentModel;

namespace System.Drawing.Design
{
	public interface IPropertyValueUIService
	{
		event EventHandler PropertyUIValueItemsChanged;

		void AddPropertyValueUIHandler(PropertyValueUIHandler newHandler);

		PropertyValueUIItem[] GetPropertyUIValueItems(ITypeDescriptorContext context, PropertyDescriptor propDesc);

		void NotifyPropertyValueUIItemsChanged();

		void RemovePropertyValueUIHandler(PropertyValueUIHandler newHandler);
	}
}
