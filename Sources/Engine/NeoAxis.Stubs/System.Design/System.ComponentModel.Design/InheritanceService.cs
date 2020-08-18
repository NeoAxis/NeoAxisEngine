using System.Reflection;

namespace System.ComponentModel.Design
{
	public class InheritanceService : IInheritanceService, IDisposable
	{
		public InheritanceService()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		protected virtual void Dispose(bool disposing)
		{
			throw null;
		}

		public void AddInheritedComponents(IComponent component, IContainer container)
		{
			throw null;
		}

		protected virtual void AddInheritedComponents(Type type, IComponent component, IContainer container)
		{
			throw null;
		}

		protected virtual bool IgnoreInheritedMember(MemberInfo member, IComponent component)
		{
			throw null;
		}

		public InheritanceAttribute GetInheritanceAttribute(IComponent component)
		{
			throw null;
		}
	}
}
