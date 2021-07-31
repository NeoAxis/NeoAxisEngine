using System.Collections;

namespace System.ComponentModel.Design
{
	public abstract class EventBindingService : IEventBindingService
	{
		protected EventBindingService(IServiceProvider provider)
		{
			throw null;
		}

		protected abstract string CreateUniqueMethodName(IComponent component, EventDescriptor e);

		protected virtual void FreeMethod(IComponent component, EventDescriptor e, string methodName)
		{
			throw null;
		}

		protected abstract ICollection GetCompatibleMethods(EventDescriptor e);

		protected object GetService(Type serviceType)
		{
			throw null;
		}

		protected abstract bool ShowCode();

		protected abstract bool ShowCode(int lineNumber);

		protected abstract bool ShowCode(IComponent component, EventDescriptor e, string methodName);

		protected virtual void UseMethod(IComponent component, EventDescriptor e, string methodName)
		{
			throw null;
		}

		protected virtual void ValidateMethodName(string methodName)
		{
			throw null;
		}

		string IEventBindingService.CreateUniqueMethodName(IComponent component, EventDescriptor e)
		{
			throw null;
		}

		ICollection IEventBindingService.GetCompatibleMethods(EventDescriptor e)
		{
			throw null;
		}

		EventDescriptor IEventBindingService.GetEvent(PropertyDescriptor property)
		{
			throw null;
		}

		PropertyDescriptorCollection IEventBindingService.GetEventProperties(EventDescriptorCollection events)
		{
			throw null;
		}

		PropertyDescriptor IEventBindingService.GetEventProperty(EventDescriptor e)
		{
			throw null;
		}

		bool IEventBindingService.ShowCode()
		{
			throw null;
		}

		bool IEventBindingService.ShowCode(int lineNumber)
		{
			throw null;
		}

		bool IEventBindingService.ShowCode(IComponent component, EventDescriptor e)
		{
			throw null;
		}
	}
}
