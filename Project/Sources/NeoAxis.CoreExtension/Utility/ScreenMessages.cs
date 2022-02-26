// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents the functionality of screen messages in the Player app.
	/// </summary>
	public static class ScreenMessages
	{
		public static double MessageShowingTime { get; set; } = 5;

		///////////////////////////////////////////////

		public class MessageItem
		{
			internal string text;
			internal double timeRemaining;

			public string Text { get { return text; } }
			public double TimeRemaining { get { return timeRemaining; } }

			internal MessageItem( string text, double timeRemaining )
			{
				this.text = text;
				this.timeRemaining = timeRemaining;
			}
		}
		static List<MessageItem> messages = new List<MessageItem>();

		///////////////////////////////////////////////

		public delegate void RenderUIDelegate( Viewport viewport, ref bool handled );
		public static event RenderUIDelegate RenderUI;

		public delegate void AddEventDelegate( ref string text, ref bool skip );
		public static event AddEventDelegate AddEvent;

		///////////////////////////////////////////////

		public static MessageItem[] GetMessages()
		{
			lock( messages )
				return messages.ToArray();
		}

		public static void Clear()
		{
			lock( messages )
				messages.Clear();
		}

		public static void PerformTick( float delta )
		{
			lock( messages )
			{
				for( int n = 0; n < messages.Count; n++ )
				{
					messages[ n ].timeRemaining -= delta;
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
			var handled = false;
			RenderUI?.Invoke( viewport, ref handled );
			if( handled )
				return;

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
			var text2 = text;
			var skip = false;
			AddEvent?.Invoke( ref text2, ref skip );
			if( skip )
				return;

			lock( messages )
			{
				var message = new MessageItem( text2, MessageShowingTime );
				messages.Add( message );

				while( messages.Count > 100 )
					messages.RemoveAt( 0 );
			}
		}
	}
}
