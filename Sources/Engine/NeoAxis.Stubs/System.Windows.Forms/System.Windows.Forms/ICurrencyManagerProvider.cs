namespace System.Windows.Forms
{
	public interface ICurrencyManagerProvider
	{
		CurrencyManager CurrencyManager
		{
			get;
		}

		CurrencyManager GetRelatedCurrencyManager(string dataMember);
	}
}
