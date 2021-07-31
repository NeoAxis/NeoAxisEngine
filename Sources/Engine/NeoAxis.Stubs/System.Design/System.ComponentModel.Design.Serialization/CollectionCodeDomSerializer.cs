using System.CodeDom;
using System.Collections;
using System.Reflection;

namespace System.ComponentModel.Design.Serialization
{
	public class CollectionCodeDomSerializer : CodeDomSerializer
	{
		protected bool MethodSupportsSerialization(MethodInfo method)
		{
			throw null;
		}

		public override object Serialize(IDesignerSerializationManager manager, object value)
		{
			throw null;
		}

		protected virtual object SerializeCollection(IDesignerSerializationManager manager, CodeExpression targetExpression, Type targetType, ICollection originalCollection, ICollection valuesToSerialize)
		{
			throw null;
		}

		public CollectionCodeDomSerializer()
		{
			throw null;
		}
	}
}
