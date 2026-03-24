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
		static string RemoveZerosAfterDot( string s )
		{
			int dotIndex = s.IndexOf( '.' );
			if( dotIndex == -1 )
				return s;
			string afterDot = s.Substring( dotIndex + 1 );
			if( afterDot.All( c => c == '0' ) )
				return s.Substring( 0, dotIndex );
			return s;
		}

		public static string BalanceToString( double balance, bool addDollarPrefix )
		{
			var balanceString = RemoveZerosAfterDot( balance.ToString( "F2" ) );

			if( addDollarPrefix )
				return $"${balanceString}";
			else
				return balanceString;
		}
	}
}
