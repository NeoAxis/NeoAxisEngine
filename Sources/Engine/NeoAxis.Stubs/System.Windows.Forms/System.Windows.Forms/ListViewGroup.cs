using System.Runtime.Serialization;

namespace System.Windows.Forms
{
	[Serializable]
	public sealed class ListViewGroup : ISerializable
	{
		public string Header
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public HorizontalAlignment HeaderAlignment
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public ListView.ListViewItemCollection Items
		{
			get
			{
				throw null;
			}
		}

		public ListView ListView
		{
			get
			{
				throw null;
			}
		}

		public string Name
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public object Tag
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public ListViewGroup()
		{
			throw null;
		}

		public ListViewGroup(string key, string headerText)
		{
			throw null;
		}

		public ListViewGroup(string header)
		{
			throw null;
		}

		public ListViewGroup(string header, HorizontalAlignment headerAlignment)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}
	}
}
