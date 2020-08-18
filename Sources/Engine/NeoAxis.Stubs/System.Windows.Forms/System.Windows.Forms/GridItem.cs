using System.ComponentModel;

namespace System.Windows.Forms
{
	public abstract class GridItem
	{
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

		public abstract GridItemCollection GridItems
		{
			get;
		}

		public abstract GridItemType GridItemType
		{
			get;
		}

		public abstract string Label
		{
			get;
		}

		public abstract GridItem Parent
		{
			get;
		}

		public abstract PropertyDescriptor PropertyDescriptor
		{
			get;
		}

		public abstract object Value
		{
			get;
		}

		public virtual bool Expandable
		{
			get
			{
				throw null;
			}
		}

		public virtual bool Expanded
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

		public abstract bool Select();

		protected GridItem()
		{
			throw null;
		}
	}
}
