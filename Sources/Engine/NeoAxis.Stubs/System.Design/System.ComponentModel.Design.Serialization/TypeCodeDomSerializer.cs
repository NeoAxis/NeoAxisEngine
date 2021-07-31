using System.CodeDom;
using System.Collections;

namespace System.ComponentModel.Design.Serialization
{
	public class TypeCodeDomSerializer : CodeDomSerializerBase
	{
		public virtual object Deserialize(IDesignerSerializationManager manager, CodeTypeDeclaration declaration)
		{
			throw null;
		}

		protected virtual CodeMemberMethod GetInitializeMethod(IDesignerSerializationManager manager, CodeTypeDeclaration declaration, object value)
		{
			throw null;
		}

		protected virtual CodeMemberMethod[] GetInitializeMethods(IDesignerSerializationManager manager, CodeTypeDeclaration declaration)
		{
			throw null;
		}

		public virtual CodeTypeDeclaration Serialize(IDesignerSerializationManager manager, object root, ICollection members)
		{
			throw null;
		}

		public TypeCodeDomSerializer()
		{
			throw null;
		}
	}
}
