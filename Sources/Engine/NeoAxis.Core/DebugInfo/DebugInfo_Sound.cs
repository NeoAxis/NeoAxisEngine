// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using NeoAxis;

namespace Internal//NeoAxis
{
	/// <summary>
	/// Represents a page with sound system information for Debug Window of the editor.
	/// </summary>
	public class DebugInfo_Sound : DebugInfo
	{
		public override string Title
		{
			get { return "Sound"; }
		}

		public override List<string> Content
		{
			get
			{
				var lines = new List<string>();

				int activeChannelCount = SoundWorld.ActiveVirtual2DChannels.Count + SoundWorld.ActiveVirtual3DChannels.Count;

				lines.Add( Translate( "Active channels" ) + ": " + activeChannelCount.ToString() );
				//lines.Add( "" );

				for( int nChannels = 0; nChannels < 2; nChannels++ )
				{
					var activeChannels = nChannels == 0 ? SoundWorld.ActiveVirtual2DChannels : SoundWorld.ActiveVirtual3DChannels;

					if( activeChannels.Count != 0 )
					{
						lines.Add( "" );
						if( nChannels == 0 )
							lines.Add( Translate( "2D" ) );
						else
							lines.Add( Translate( "3D" ) );

						foreach( var virtualChannel in activeChannels )
						{
							StringBuilder text = new StringBuilder();

							if( virtualChannel.CurrentRealChannel != null )
								text.Append( Translate( "Real" ) + " - " );
							else
								text.Append( Translate( "Virtual" ) + " - " );

							string soundName;
							if( virtualChannel.Sound.Name != null )
								soundName = virtualChannel.Sound.Name;
							else
								soundName = "DataBuffer";

							text.AppendFormat( "{0}; {1} {2}; {3} {4}", soundName, Translate( "Volume" ), virtualChannel.GetTotalVolume().ToString( "F2" ), Translate( "Time" ), virtualChannel.Time.ToString( "F2" ) );

							lines.Add( text.ToString() );
						}
					}
				}

				return lines;
			}
		}
	}
}
