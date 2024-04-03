// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	[HCExpandable]
	public struct CurvePoint1F : ICanParseFromAndConvertToString
	{
		//[DefaultValue( 0.0 )]
		[Serialize]
		public float Point { get; set; }

		//[DefaultValue( 0.0 )]
		[Serialize]
		public float Value { get; set; }

		//

		public CurvePoint1F( float point, float value )
		{
			Point = point;
			Value = value;
		}

		public override string ToString()
		{
			return string.Format( "{0} {1}", Point, Value );
		}

		public static CurvePoint1F Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] values = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

			if( values.Length != 2 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form \'Point Value\'.", text ) );

			try
			{
				return new CurvePoint1F( float.Parse( values[ 0 ] ), float.Parse( values[ 1 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the value must be decimal numbers." );
			}
		}
	}
}
