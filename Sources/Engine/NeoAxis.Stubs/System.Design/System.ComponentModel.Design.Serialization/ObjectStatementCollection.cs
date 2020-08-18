using System.CodeDom;
using System.Collections;

namespace System.ComponentModel.Design.Serialization
{
	public sealed class ObjectStatementCollection : IEnumerable
	{
		public CodeStatementCollection this[object statementOwner]
		{
			get
			{
				throw null;
			}
		}

		public bool ContainsKey(object statementOwner)
		{
			throw null;
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			throw null;
		}

		public void Populate(ICollection statementOwners)
		{
			throw null;
		}

		public void Populate(object owner)
		{
			throw null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw null;
		}
	}
}
