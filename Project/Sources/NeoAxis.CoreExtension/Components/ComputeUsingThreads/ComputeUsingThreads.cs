//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Threading.Tasks;


//it is disabled because never used in practice


//namespace NeoAxis
//{
//	/// <summary>
//	/// Auxiliary class to make optimized calculations using threads.
//	/// </summary>
//	[AddToResourcesWindow( @"Base\Common\Advanced\Compute Using Threads", 0 )]
//#if !DEPLOY
//	[Editor.SettingsCell( typeof( Editor.ComputeUsingThreadsSettingsCell ) )]
//#endif
//	public class ComputeUsingThreads : Component
//	{
//		bool started;
//		volatile ComputeContext context;

//		///////////////////////////////////////////////

//		/// <summary>
//		/// The number of threads used.
//		/// </summary>
//		[DefaultValue( 8 )]
//		public Reference<int> ThreadCount
//		{
//			get { if( _threadCount.BeginGet() ) ThreadCount = _threadCount.Get( this ); return _threadCount.value; }
//			set { if( _threadCount.BeginSet( ref value ) ) { try { ThreadCountChanged?.Invoke( this ); Start(); } finally { _threadCount.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="ThreadCount"/> property value changes.</summary>
//		public event Action<ComputeUsingThreads> ThreadCountChanged;
//		ReferenceField<int> _threadCount = 8;

//		/// <summary>
//		/// Whether to start after component initialization.
//		/// </summary>
//		[DefaultValue( false )]
//		public Reference<bool> AutoStart
//		{
//			get { if( _autoStart.BeginGet() ) AutoStart = _autoStart.Get( this ); return _autoStart.value; }
//			set { if( _autoStart.BeginSet( ref value ) ) { try { AutoStartChanged?.Invoke( this ); } finally { _autoStart.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="AutoStart"/> property value changes.</summary>
//		public event Action<ComputeUsingThreads> AutoStartChanged;
//		ReferenceField<bool> _autoStart = false;

//		[Browsable( false )]
//		public bool Started
//		{
//			get { return started; }
//		}

//		[Browsable( false )]
//		public ComputeContext Context
//		{
//			get { return context; }
//		}

//		///////////////////////////////////////////////

//		public class ComputeContext
//		{
//			//public ComputeUsingThreads Owner;
//			public bool AllowCompute = true;

//			//!!!!use tasks?
//			public Task[] Tasks;

//			public bool CompletedSuccessfully;
//			public bool Faulted;
//			public bool Cancelled;

//			public object AnyData;

//			//

//			public bool Completed
//			{
//				get { return CompletedSuccessfully || Faulted || Cancelled; }
//			}
//		}

//		///////////////////////////////////////////////

//		protected virtual void OnComputeBegin()
//		{
//		}

//		public delegate void BeforeComputeDelegate( ComputeUsingThreads sender );
//		public event BeforeComputeDelegate ComputeBegin;

//		void PerformComputeBegin()
//		{
//			OnComputeBegin();
//			ComputeBegin?.Invoke( this );
//		}

//		//

//		protected virtual void OnComputeThread( int threadIndex ) { }

//		public delegate void ComputeThreadDelegate( ComputeUsingThreads sender, int threadIndex );
//		public event ComputeThreadDelegate ComputeThread;

//		void PerformComputeThread( int threadIndex )
//		{
//			OnComputeThread( threadIndex );
//			ComputeThread?.Invoke( this, threadIndex );
//		}

//		//

//		protected virtual void OnComputeEnd() { }

//		public delegate void ComputeEndDelegate( ComputeUsingThreads sender );
//		public event ComputeEndDelegate ComputeEnd;

//		void PerformComputeEnd()
//		{
//			OnComputeEnd();
//			ComputeEnd?.Invoke( this );
//		}

//		//

//		protected override void OnEnabledInHierarchyChanged()
//		{
//			base.OnEnabledInHierarchyChanged();

//			if( EnabledInHierarchyAndIsInstance )
//			{
//				if( AutoStart )
//					Start();
//			}
//			else
//				Stop();
//		}

//		public void Start()
//		{
//			if( !EnabledInHierarchyAndIsInstance )
//				return;
//			Stop();
//			started = true;
//		}

//		public void Stop()
//		{
//			if( started )
//			{
//				context = null;
//				started = false;
//			}
//		}

//		void ThreadFunction( object data2 )
//		{
//			var threadIndex = (int)data2;
//			PerformComputeThread( threadIndex );
//		}

//		public void PerformUpdate()
//		{
//			if( Started )
//			{
//				//start
//				if( context == null )
//				{
//					context = new ComputeContext();
//					//context.Owner = this;
//					PerformComputeBegin();

//					if( context.AllowCompute )
//					{
//						//create tasks
//						context.Tasks = new Task[ ThreadCount ];
//						for( int n = 0; n < context.Tasks.Length; n++ )
//							context.Tasks[ n ] = new Task( ThreadFunction, n );

//						for( int n = 0; n < context.Tasks.Length; n++ )
//							context.Tasks[ n ].Start();
//					}
//				}

//				//update
//				if( context != null )
//				{
//					var allCompletedSuccessfully = true;
//					var existsFaulted = false;
//					var existsCancelled = false;

//					for( int n = 0; n < context.Tasks.Length; n++ )
//					{
//						var task = context.Tasks[ n ];

//						if( !task.IsCompletedSuccessfully )
//							allCompletedSuccessfully = false;
//						if( task.IsFaulted )
//							existsFaulted = true;
//						if( task.IsCanceled )
//							existsCancelled = true;
//					}

//					if( allCompletedSuccessfully || existsFaulted || existsCancelled )
//					{
//						//end

//						if( existsFaulted )
//							context.Faulted = existsFaulted;
//						else if( existsCancelled )
//							context.Cancelled = existsCancelled;
//						else if( allCompletedSuccessfully )
//							context.CompletedSuccessfully = true;

//						PerformComputeEnd();

//						context = null;
//					}
//				}
//			}
//		}

//		protected override void OnUpdate( float delta )
//		{
//			base.OnUpdate( delta );

//			PerformUpdate();
//		}
//	}
//}
