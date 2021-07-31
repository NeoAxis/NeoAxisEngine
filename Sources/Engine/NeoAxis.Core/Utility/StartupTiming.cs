// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	public static class StartupTiming
	{
		static DateTime startTime;
		static bool startTimeInitialized;
		static DateTime endTime;
		static List<CounterItem> counters = new List<CounterItem>();
		static int currentLevel;

		//

		public struct CounterItem
		{
			public string Name;
			public int Level;
			public DateTime Start;
			public DateTime End;
			public string Details;
		}

		public static void TotalStart()
		{
			if( !startTimeInitialized )
			{
				startTime = DateTime.Now;
				startTimeInitialized = true;
			}
		}

		public static void TotalEnd()
		{
			endTime = DateTime.Now;
		}

		public static void CounterStart( string name )
		{
			var item = new CounterItem();
			item.Name = name;
			item.Level = currentLevel;
			item.Start = DateTime.Now;
			counters.Add( item );

			currentLevel++;
		}

		public static void CounterEnd( string name, string details = "" )
		{
			var index = counters.FindIndex( i => i.Name == name );
			if( index != -1 )
			{
				var item = counters[ index ];
				item.End = DateTime.Now;
				item.Details = details;
				counters[ index ] = item;

				currentLevel--;
			}
		}

		static string GetText( CounterItem item )
		{
			return new string( ' ', item.Level * 2 ) + item.Name + ": ";
		}

		public static List<string> GetStatisticsAsStringLines()
		{
			var result = new List<string>();

			var nameLength = 7;
			foreach( var c in counters )
				nameLength = Math.Max( nameLength, GetText( c ).Length );

			result.Add( "--------------------------------------------------------------" );
			result.Add( "Startup Timing" );
			result.Add( "--------------" );

			double other = ( endTime - startTime ).TotalSeconds;

			foreach( var c in counters )
			{
				var t = ( c.End - c.Start ).TotalSeconds;

				var text = GetText( c );

				var s = text + new string( ' ', nameLength - text.Length ) + t.ToString( "F3" );
				if( !string.IsNullOrEmpty( c.Details ) )
					s += " (" + c.Details + ")";
				result.Add( s );

				if( c.Level == 0 )
					other -= t;
			}

			result.Add( "Other: " + new string( ' ', nameLength - "Other: ".Length ) + other.ToString( "F3" ) );
			result.Add( "------" );
			result.Add( "Total: " + new string( ' ', nameLength - "Total: ".Length ) + ( endTime - startTime ).TotalSeconds.ToString( "F3" ) );
			result.Add( "--------------------------------------------------------------" );

			return result;
		}

		public static DateTime StartTime
		{
			get { return startTime; }
		}

		public static DateTime EndTime
		{
			get { return endTime; }
		}

		public static ICollection<CounterItem> Counters
		{
			get { return counters.AsReadOnly(); }
		}
	}
}
