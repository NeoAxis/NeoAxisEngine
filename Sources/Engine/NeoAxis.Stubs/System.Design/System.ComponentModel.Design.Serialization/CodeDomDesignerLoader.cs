using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;

namespace System.ComponentModel.Design.Serialization
{
	public abstract class CodeDomDesignerLoader : BasicDesignerLoader, INameCreationService, IDesignerSerializationService
	{
		protected abstract CodeDomProvider CodeDomProvider
		{
			get;
		}

		protected abstract ITypeResolutionService TypeResolutionService
		{
			get;
		}

		public override void Dispose()
		{
			throw null;
		}

		protected override void Initialize()
		{
			throw null;
		}

		protected override bool IsReloadNeeded()
		{
			throw null;
		}

		protected override void OnBeginLoad()
		{
			throw null;
		}

		protected override void OnBeginUnload()
		{
			throw null;
		}

		protected override void OnEndLoad(bool successful, ICollection errors)
		{
			throw null;
		}

		protected abstract CodeCompileUnit Parse();

		protected override void PerformFlush(IDesignerSerializationManager manager)
		{
			throw null;
		}

		protected override void PerformLoad(IDesignerSerializationManager manager)
		{
			throw null;
		}

		protected virtual void OnComponentRename(object component, string oldName, string newName)
		{
			throw null;
		}

		protected abstract void Write(CodeCompileUnit unit);

		ICollection IDesignerSerializationService.Deserialize(object serializationData)
		{
			throw null;
		}

		object IDesignerSerializationService.Serialize(ICollection objects)
		{
			throw null;
		}

		string INameCreationService.CreateName(IContainer container, Type dataType)
		{
			throw null;
		}

		bool INameCreationService.IsValidName(string name)
		{
			throw null;
		}

		void INameCreationService.ValidateName(string name)
		{
			throw null;
		}

		protected CodeDomDesignerLoader()
		{
			throw null;
		}
	}
}
