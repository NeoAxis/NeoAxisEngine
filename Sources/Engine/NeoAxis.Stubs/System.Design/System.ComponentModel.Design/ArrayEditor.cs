namespace System.ComponentModel.Design
{
	public class ArrayEditor : CollectionEditor
	{
		public ArrayEditor(Type type)
			:base(type)
		{
			throw null;
		}

		protected override Type CreateCollectionItemType()
		{
			throw null;
		}

		protected override object[] GetItems(object editValue)
		{
			throw null;
		}

		protected override object SetItems(object editValue, object[] value)
		{
			throw null;
		}
	}
}
