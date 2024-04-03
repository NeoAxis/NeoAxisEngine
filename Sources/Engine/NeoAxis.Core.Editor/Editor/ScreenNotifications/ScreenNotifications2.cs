#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading;

namespace NeoAxis.Editor
{
	public static class ScreenNotifications2
	{
		//!!!!в конфиг
		static int durationInSeconds = 5;
		static List<NotificationItem> items = new List<NotificationItem>();
		static Queue<(string, bool, bool)> notificationsInQueue = new Queue<(string, bool, bool)>();

		/////////////////////////////////////////

		//public enum NotificationType
		//{
		//	Info,
		//	Warning,
		//	Error
		//}

		/////////////////////////////////////////

		public class NotificationItem
		{
			string text;
			bool error;
			//NotificationType type;
			internal ScreenNotificationForm notificationForm;

			internal NotificationItem( string text, bool error )// NotificationType type )
			{
				this.text = text;
				this.error = error;
				//this.type = type;
			}

			public string Text
			{
				get { return text; }
			}

			public bool Error
			{
				get { return error; }
			}
			//public NotificationType Type
			//{
			//	get { return type; }
			//}
		}

		/////////////////////////////////////////

		public class StickyNotificationItem : NotificationItem, ScreenNotifications.IStickyNotificationItem
		{
			public object UserData { get; set; }

			internal StickyNotificationItem( string text, bool error )
				: base( text, error )
			{
			}

			public void Close()
			{
				ScreenNotifications2.Close( this );
			}
		}

		/////////////////////////////////////////

		static void Close( NotificationItem item )
		{
			items.Remove( item );

			if( item.notificationForm != null )
			{
				try
				{
					item.notificationForm.Close();
					item.notificationForm.Dispose();
				}
				catch { }
			}
		}

		public static int DurationInSeconds
		{
			get { return durationInSeconds; }
			set { durationInSeconds = value; }
		}

		public static void Show( string text, bool error = false )// NotificationType type = NotificationType.Info )
		{
			Show( text, error, false );
		}

		public static StickyNotificationItem ShowSticky( string text, bool error = false )// NotificationType type = NotificationType.Info )
		{
			return (StickyNotificationItem)Show( text, error, true );
		}

		public delegate void ShowBeforeDelegate( string text, bool error, bool sticky, ref bool handled );
		public static event ShowBeforeDelegate ShowBefore;

		internal static NotificationItem Show( string text, bool error, bool sticky )
		{
			var handled = false;
			ShowBefore?.Invoke( text, error, sticky, ref handled );

			if( handled || !EngineApp.IsEditor )
			{
				if( sticky )
					return new StickyNotificationItem( text, error );
				else
					return new NotificationItem( text, error );
			}

			Thread thread = Thread.CurrentThread;
			if( Log.MainThread == null || thread == Log.MainThread )
			{
				NotificationItem item;
				if( sticky )
					item = new StickyNotificationItem( text, error );
				else
					item = new NotificationItem( text, error );
				items.Add( item );

				if( EditorForm.Instance != null && !EngineApp.Closing )
				{
					//!!!!?
					string title = "";

					int duration = sticky ? -1 : DurationInSeconds;

					item.notificationForm = new ScreenNotificationForm( title, text, error, duration );
					item.notificationForm.Show();
					if( sticky )
					{
						item.notificationForm.Update();
						EditorAPI2.ApplicationDoEvents( true );//Application.DoEvents();
					}
				}

				return item;
			}
			else
			{
				//the method was called from another thread

				lock( notificationsInQueue )
				{
					while( notificationsInQueue.Count > 100 )
						notificationsInQueue.Dequeue();
					notificationsInQueue.Enqueue( (text, error, sticky) );
				}
				return null;
			}
		}

		public /*internal*/ static void Update()
		{
			lock( notificationsInQueue )
			{
				while( notificationsInQueue.Count != 0 )
				{
					var value = notificationsInQueue.Dequeue();
					Show( value.Item1, value.Item2, value.Item3 );
				}
			}
		}

		public static void ShowAllImmediately()
		{
			Update();

			foreach( var item in items )
				item.notificationForm?.SetMaxOpacity();

			EditorAPI2.ApplicationDoEvents( true );//Application.DoEvents();
		}
	}
}

#endif