using System.CodeDom;

namespace System.ComponentModel.Design.Serialization
{
	public abstract class MemberCodeDomSerializer : CodeDomSerializerBase
	{
		public abstract void Serialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor, CodeStatementCollection statements);

		public abstract bool ShouldSerialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor);

		protected MemberCodeDomSerializer()
		{
			throw null;
		}
	}
}
