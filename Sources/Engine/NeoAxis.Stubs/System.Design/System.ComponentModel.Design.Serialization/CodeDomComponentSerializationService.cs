using System.Collections;
using System.IO;

namespace System.ComponentModel.Design.Serialization
{
	public sealed class CodeDomComponentSerializationService : ComponentSerializationService
	{
		public CodeDomComponentSerializationService()
		{
			throw null;
		}

		public CodeDomComponentSerializationService(IServiceProvider provider)
		{
			throw null;
		}

		public override SerializationStore CreateStore()
		{
			throw null;
		}

		public override SerializationStore LoadStore(Stream stream)
		{
			throw null;
		}

		public override void Serialize(SerializationStore store, object value)
		{
			throw null;
		}

		public override void SerializeAbsolute(SerializationStore store, object value)
		{
			throw null;
		}

		public override void SerializeMember(SerializationStore store, object owningObject, MemberDescriptor member)
		{
			throw null;
		}

		public override void SerializeMemberAbsolute(SerializationStore store, object owningObject, MemberDescriptor member)
		{
			throw null;
		}

		public override ICollection Deserialize(SerializationStore store)
		{
			throw null;
		}

		public override ICollection Deserialize(SerializationStore store, IContainer container)
		{
			throw null;
		}

		public override void DeserializeTo(SerializationStore store, IContainer container, bool validateRecycledTypes, bool applyDefaults)
		{
			throw null;
		}
	}
}
