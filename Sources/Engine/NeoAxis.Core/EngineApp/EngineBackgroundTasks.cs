//// Copyright 2006Ц2026 Ivan Efimov. All rights reserved.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;

//namespace NeoAxis
//{
//	//!!!!!надо?

//	/// <summary>
//	/// This class is used to perform engine's operations in a background.
//	/// </summary>
//	public static class EngineBackgroundTasks
//	{
//		public delegate void UpdateDelegate();
//		public static event UpdateDelegate Update;

//		///////////////////////////////////////////

//		public static void PerformUpdate()
//		{
//			//!!!!!вызывать

//			//!!!!!!
//			Log.Fatal( "impl" );
//			//xx xx;//вызывать
//			//это тоже: Log.FlushCachedLog();

//			Update?.Invoke();
//		}
//	}
//}
