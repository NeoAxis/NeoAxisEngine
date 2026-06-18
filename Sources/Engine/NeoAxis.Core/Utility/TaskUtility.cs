// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoAxis
{
	public static class TaskUtility
	{
		static bool tracking;
		static ConcurrentDictionary<Task, TaskInfo> tasks = new ConcurrentDictionary<Task, TaskInfo>();
		static DateTime lastUpdateTime;
		static string tasksInfo = "";

		///////////////////////////////////////////////

		public enum TaskLifetimeEnum
		{
			Minutes,
			Hour,
			Forever,
		}

		///////////////////////////////////////////////

		public struct TaskInfo
		{
			public TaskLifetimeEnum Lifetime;
			public string Name;
			public DateTime StartTime;
		}

		///////////////////////////////////////////////

		/// <summary>
		/// Enable tracking of tasks. Must call Update() method periodically.
		/// </summary>
		public static void EnableTracking()
		{
			tracking = true;
		}

		/// <summary>
		/// Run a task with a parameter.
		/// </summary>
		public static Task Run( TaskLifetimeEnum lifetime, string name, Func<object, Task> function, object state )
		{
			var info = new TaskInfo();
			info.Lifetime = lifetime;
			info.Name = name;
			info.StartTime = DateTime.UtcNow;

			var task = new Task<Task>( function, state );
			task.Start();
			if( tracking )
				tasks[ task ] = info;
			return task;
		}

		/// <summary>
		/// Run a task without a parameter.
		/// </summary>
		public static Task Run( TaskLifetimeEnum lifetime, string name, Func<Task> function )
		{
			var info = new TaskInfo();
			info.Lifetime = lifetime;
			info.Name = name;
			info.StartTime = DateTime.UtcNow;

			var task = new Task<Task>( function );
			task.Start();
			if( tracking )
				tasks[ task ] = info;
			return task;
		}

		public static void Update( DateTime utcNow )
		{
			if( ( utcNow - lastUpdateTime ).TotalMinutes > 1 )
			{
				lastUpdateTime = utcNow;

				var info = $"{tasks.Count}";

				foreach( var pair in tasks )
				{
					var task = pair.Key;
					var taskInfo = pair.Value;

					if( task.IsCompleted )
					{
						//delete completed tasks
						tasks.TryRemove( task, out _ );
					}
					else
					{
						var time = ( utcNow - taskInfo.StartTime ).TotalMinutes;

						var freezed = false;

						switch( taskInfo.Lifetime )
						{
						case TaskLifetimeEnum.Minutes:
							if( time > 10 )
								freezed = true;
							break;

						case TaskLifetimeEnum.Hour:
							if( time > 60 )
								freezed = true;
							break;
						}

						if( freezed )
						{
							if( !string.IsNullOrEmpty( info ) )
								info += ", ";
							info += $"{taskInfo.Name} {time.ToString( "F2" )} minutes";
						}
					}
				}

				tasksInfo = info;
			}
		}

		public static string GetTracingInfoAsString()
		{
			return tasksInfo;
		}
	}
}