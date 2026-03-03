// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Utility functions for handling balances in different currencies.
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


		//public static bool BalanceParse( string balanceString, out Dictionary<string, double> balance, out string error )
		//{
		//	try
		//	{
		//		balance = new Dictionary<string, double>();
		//		var strings = balanceString.Split( new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
		//		foreach( var s in strings )
		//		{
		//			var s2 = s.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
		//			if( s2.Length != 2 )
		//				throw new Exception( "Invalid format." );

		//			var value = double.Parse( s2[ 0 ] );
		//			var currency = s2[ 1 ];
		//			balance[ currency ] = value;
		//		}
		//		error = string.Empty;
		//		return true;
		//	}
		//	catch( Exception e )
		//	{
		//		balance = new Dictionary<string, double>();
		//		error = e.Message;
		//		return false;
		//	}
		//}

		////!!!!where to use
		//public static string GetStringFormatForCurrency( string currency )
		//{
		//	if( currency == "BTC" )
		//		return "0.##########";
		//	else if( currency == "ETH" )
		//		return "0.####################";
		//	return "F2";
		//}

		////!!!!where to use
		//public static double GetCurrencyEpsilon( string currency )
		//{
		//	if( currency == "BTC" )
		//		return 0.0000000001;
		//	else if( currency == "ETH" )
		//		return 0.00000000000000000001;
		//	return 0.00000001;
		//}

		//public static string GetValueForCurrency( string currency, double value )
		//{
		//	var format = GetStringFormatForCurrency( currency );
		//	var formatted = value.ToString( format );

		//	// Remove trailing zeros and dot if necessary
		//	if( formatted.Contains( '.' ) )
		//		formatted = formatted.TrimEnd( '0' ).TrimEnd( '.' );

		//	return formatted;
		//}

		//public static string BalanceToString( Dictionary<string, double> balance )
		//{
		//	var builder = new StringBuilder();
		//	foreach( var item in balance )
		//	{
		//		if( builder.Length != 0 )
		//			builder.Append( "; " );
		//		builder.Append( $"{GetValueForCurrency( item.Key, item.Value )} {item.Key}" );
		//		//builder.Append( $"{item.Value.ToString( GetStringFormatForCurrency( item.Key ) )} {item.Key}" );
		//	}
		//	return builder.ToString();
		//}

		//public static string BalanceToString( string currency, double amount, bool usePrefixForm = false )
		//{
		//	if( usePrefixForm && currency == "USD" )
		//	{
		//		//!!!!add ','

		//		return $"${GetValueForCurrency( currency, amount )}";
		//	}
		//	return $"{GetValueForCurrency( currency, amount )} {currency}";
		//}

		//public static bool BalanceAnyPositive( Dictionary<string, double> balance )
		//{
		//	foreach( var item in balance )
		//	{
		//		if( item.Value > 0 )
		//			return true;
		//	}
		//	return false;
		//}

		//public static void BalanceUpdate( Dictionary<string, double> balance, string currency, double value )
		//{
		//	//update
		//	if( balance.ContainsKey( currency ) )
		//		balance[ currency ] += value;
		//	else
		//		balance[ currency ] = value;

		//	//delete zero values
		//	BalanceDeleteZeroValues( balance );
		//}

		//public static void BalanceDeleteZeroValues( Dictionary<string, double> balance )
		//{
		//	var keysToRemove = new List<string>();
		//	foreach( var item in balance )
		//	{
		//		var epsilon = GetCurrencyEpsilon( item.Key );
		//		if( Math.Abs( item.Value ) < epsilon )
		//			keysToRemove.Add( item.Key );
		//	}
		//	foreach( var key in keysToRemove )
		//		balance.Remove( key );
		//}
	}
}
