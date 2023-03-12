// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Internal.SharpBgfx;
using System.Globalization;
using NeoAxis;

namespace Internal//NeoAxis
{
	/// <summary>
	/// Represents a page with rendering statistics for Debug Window of the editor.
	/// </summary>
	public class DebugInfo_RenderStatistics : DebugInfo
	{
		public override string Title
		{
			get { return "Render: Statistics"; }
		}

		public override List<string> Content
		{
			get
			{
				var lines = new List<string>();

				NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;
				nfi.NumberGroupSeparator = " ";

				var stats = Bgfx.GetStats();

				foreach( var p in stats.GetType().GetProperties() )
				{
					if( p.Name == "Views" || p.Name == "BackbufferWidth" || p.Name == "BackbufferHeight" || p.Name == "TextBufferWidth" || p.Name == "TextBufferHeight" )
						continue;

					var value = p.GetValue( stats );

					string valueString = null;

					if( value.GetType() == typeof( int ) )
						valueString = ( (int)value ).ToString( "N0", nfi );
					else if( value.GetType() == typeof( long ) )
						valueString = ( (long)value ).ToString( "N0", nfi );

					if( valueString == null )
						valueString = value.ToString();

					lines.Add( string.Format( "{0}: {1}", TypeUtility.DisplayNameAddSpaces( p.Name ), valueString ) );
				}

				return lines;
			}
		}
	}
}
