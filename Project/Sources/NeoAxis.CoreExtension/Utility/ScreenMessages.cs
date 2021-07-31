// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents a screen messages in the Player.
	/// </summary>
	public static class ScreenMessages
	{
		class MessageItem
		{
			public string Text;
			public double TimeRemaining;
		}
		static List<MessageItem> messages = new List<MessageItem>();

		//

		public static void PerformTick( float delta )
		{
			lock( messages )
			{
				for( int n = 0; n < messages.Count; n++ )
				{
					messages[ n ].TimeRemaining -= delta;
					if( messages[ n ].TimeRemaining <= 0 )
					{
						messages.RemoveAt( n );
						n--;
					}
				}
			}
		}

		public static void PerformRenderUI( Viewport viewport )
		{
			lock( messages )
			{
				var renderer = viewport.CanvasRenderer;
				var fontSize = renderer.DefaultFontSize;
				var offset = new Vector2( fontSize * renderer.AspectRatioInv * 0.8, fontSize * 0.6 );

				var pos = new Vector2( offset.X, 0.75 );// 1.0 - offset.Y );
				for( int n = messages.Count - 1; n >= 0; n-- )
				{
					var message = messages[ n ];

					var alpha = message.TimeRemaining;
					if( alpha > 1 )
						alpha = 1;
					CanvasRendererUtility.AddTextWithShadow( viewport, message.Text, pos, EHorizontalAlignment.Left, EVerticalAlignment.Bottom, new ColorValue( 1, 1, 1, alpha ) );

					pos.Y -= renderer.DefaultFontSize;
				}
			}
		}

		public static void Add( string text )
		{
			lock( messages )
			{
				var message = new MessageItem();
				message.Text = text;
				message.TimeRemaining = 5;
				messages.Add( message );

				while( messages.Count > 100 )
					messages.RemoveAt( 0 );
			}
		}
	}
}
