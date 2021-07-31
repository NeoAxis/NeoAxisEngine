namespace System.ComponentModel.Design
{
	public interface IComponentDesignerStateService
	{
		object GetState(IComponent component, string key);

		void SetState(IComponent component, string key, object value);
	}
}
