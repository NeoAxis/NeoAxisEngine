using System.CodeDom;

namespace System.ComponentModel.Design.Serialization
{
	public class CodeDomSerializer : CodeDomSerializerBase
	{
		public virtual string GetTargetComponentName(CodeStatement statement, CodeExpression expression, Type targetType)
		{
			throw null;
		}

		public virtual object Deserialize(IDesignerSerializationManager manager, object codeObject)
		{
			throw null;
		}

		protected object DeserializeStatementToInstance(IDesignerSerializationManager manager, CodeStatement statement)
		{
			throw null;
		}

		public virtual object Serialize(IDesignerSerializationManager manager, object value)
		{
			throw null;
		}

		public virtual object SerializeAbsolute(IDesignerSerializationManager manager, object value)
		{
			throw null;
		}

		public virtual CodeStatementCollection SerializeMember(IDesignerSerializationManager manager, object owningObject, MemberDescriptor member)
		{
			throw null;
		}

		public virtual CodeStatementCollection SerializeMemberAbsolute(IDesignerSerializationManager manager, object owningObject, MemberDescriptor member)
		{
			throw null;
		}

		protected CodeExpression SerializeToReferenceExpression(IDesignerSerializationManager manager, object value)
		{
			throw null;
		}

		public CodeDomSerializer()
		{
			throw null;
		}
	}
}
