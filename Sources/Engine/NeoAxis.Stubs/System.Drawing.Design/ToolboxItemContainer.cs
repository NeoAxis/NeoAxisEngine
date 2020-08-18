using System.Collections;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace System.Drawing.Design
{
	[Serializable]
	public class ToolboxItemContainer : ISerializable
	{
		public bool IsCreated
		{
			get
			{
				throw null;
			}
		}

		public bool IsTransient
		{
			get
			{
				throw null;
			}
		}

		public virtual IDataObject ToolboxData
		{
			get
			{
				throw null;
			}
		}

		protected ToolboxItemContainer(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		public ToolboxItemContainer(ToolboxItem item)
		{
			throw null;
		}

		public ToolboxItemContainer(IDataObject data)
		{
			throw null;
		}

		public void UpdateFilter(ToolboxItem item)
		{
			throw null;
		}

		public override bool Equals(object obj)
		{
			throw null;
		}

		public virtual ICollection GetFilter(ICollection creators)
		{
			throw null;
		}

		public override int GetHashCode()
		{
			throw null;
		}

		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		public virtual ToolboxItem GetToolboxItem(ICollection creators)
		{
			throw null;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}
	}
}
