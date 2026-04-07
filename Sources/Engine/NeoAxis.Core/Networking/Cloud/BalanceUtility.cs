// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Utility functions to handle balance.
	/// </summary>
	public static class BalanceUtility
	{
		public static char CurrencyPrefix = '$';

		//

		public static string BalanceToString( double balance, bool addCurrencyPrefix, string format = "0.00" )
		{
			var balanceString = balance.ToString( format );

			if( addCurrencyPrefix )
				return $"{CurrencyPrefix}{balanceString}";
			else
				return balanceString;
		}
	}
}
