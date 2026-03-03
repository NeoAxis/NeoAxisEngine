#if !NO_SERVER
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.CloudServer
{
	public static class Actions
	{
		static List<ActionItem> actions = new List<ActionItem>();
		static DateTime lastUpdateTime;

		///////////////////////////////////////////////

		public class ActionItem
		{
			public string ID;
			public DateTime CreationTime;
			public string Text;
			public bool Stoppable;
			public StatusEnum Status;
			public int Progress;
			public DateTime EndTime;

			/////////////////////

			public enum StatusEnum
			{
				Processing,
				Success,
				Failed,
			}

			/////////////////////

			public delegate void StoppedDelegate( ActionItem sender );
			public event StoppedDelegate Stopped;

			/////////////////////

			public void End( bool success )
			{
				lock( actions )
				{
					EndTime = DateTime.UtcNow;
					Status = success ? StatusEnum.Success : StatusEnum.Failed;
				}
			}

			public int GetRemainingTimeInSeconds()
			{
				lock( actions )
				{
					//24 hours
					if( Status != StatusEnum.Processing )
						return 24 * 60 * 60 - (int)( DateTime.UtcNow - EndTime ).TotalSeconds;
					return 24 * 60 * 60;
				}
			}

			internal void CallStopped()
			{
				Stopped?.Invoke( this );
			}
		}

		///////////////////////////////////////////////

		public static ActionItem Start( string text, bool stoppable )
		{
			var action = new ActionItem();
			action.CreationTime = DateTime.UtcNow;
			action.Text = text;
			action.Stoppable = stoppable;
			action.Status = ActionItem.StatusEnum.Processing;
			action.Progress = 0;
			action.ID = Guid.NewGuid().ToString( "N" );
			lock( actions )
				actions.Add( action );
			return action;
		}

		public static ActionItem[] GetActions()
		{
			lock( actions )
				return actions.ToArray();
		}

		public static void Stop( string id )
		{
			lock( actions )
			{
				var action = GetActions().FirstOrDefault( a => a.ID == id );
				if( action != null && action.Stoppable && action.Status == ActionItem.StatusEnum.Processing )
				{
					action.End( false );
					action.CallStopped();
				}
			}
		}

		static void RemoveOldActions()
		{
			lock( actions )
			{
				var oldList = GetActions();
				foreach( var action in oldList )
				{
					if( action.GetRemainingTimeInSeconds() <= 0 )
						actions.Remove( action );
				}
			}
		}

		public static void Update( DateTime utcNow )
		{
			if( ( utcNow - lastUpdateTime ).TotalSeconds > 10 )
			{
				RemoveOldActions();
				lastUpdateTime = DateTime.UtcNow;
			}
		}
	}
}
#endif