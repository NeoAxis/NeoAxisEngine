using System.ComponentModel;

namespace System.Windows.Forms.PropertyGridInternal
{
	public interface IRootGridEntry
	{
		AttributeCollection BrowsableAttributes
		{
			get;
			set;
		}

		void ResetBrowsableAttributes();

		void ShowCategories(bool showCategories);
	}
}
