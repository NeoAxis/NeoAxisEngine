//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;

//namespace NeoAxis
//{

//	//!!!!

//	//!!!!как будет в виде ноды в FlowGraph

//	//!!!!типичные случаи:
//	//- выставляет Enabled/Active = true, false в определенном интервале. выходит в конце значение восстанавливается

//	//!!!!, IFlowGraphRepresentationData

//	public class Component_Timeline : Component, IFlowExecutionComponent
//	{
//		[DefaultValue( 10.0 )]
//		public Reference<double> Length
//		{
//			get { if( _length.BeginGet() ) Length = _length.Get( this ); return _length.value; }
//			set { if( _length.BeginSet( ref value ) ) { try { LengthChanged?.Invoke( this ); } finally { _length.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Length"/> property value changes.</summary>
//		public event Action<Component_Timeline> LengthChanged;
//		ReferenceField<double> _length = 10.0;

//		//!!!!bool Looping;

//		/////////////////////////////////////////

//		//!!!!
//		public class ExecutionContext
//		{
//		}

//		/////////////////////////////////////////

//		//!!!!execution context?
//		//!!!!указывать "owner". тот который типа _this. как в Component_Method
//		public ExecutionContext Play()
//		{
//			var context = new ExecutionContext();

//			//!!!!

//			foreach( var track in GetComponents<Component_TimelineTrack>() )
//			{
//				if( track.Enabled )
//					track.CallOnPlay( context );
//			}

//			return context;
//		}

//		public void FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem )
//		{
//			//!!!!
//		}
//	}

//	public class Component_TimelineTrack : Component
//	{
//		//!!!!
//		//bool Locked;

//		//!!!!
//		protected virtual void OnPlay( Component_Timeline.ExecutionContext context )
//		{
//			foreach( var c in GetComponents<Component_TimelineElement>() )
//			{
//				if( c.Enabled )
//					c.CallOnPlay( context );
//			}
//		}

//		internal void CallOnPlay( Component_Timeline.ExecutionContext context )
//		{
//			OnPlay( context );
//		}
//	}

//	//!!!!name: Component_Timeline Node/Block/Segment
//	public class Component_TimelineElement : Component
//	{
//		[DefaultValue( 0.0 )]
//		public Reference<double> StartTime
//		{
//			get { if( _startTime.BeginGet() ) StartTime = _startTime.Get( this ); return _startTime.value; }
//			set { if( _startTime.BeginSet( ref value ) ) { try { StartTimeChanged?.Invoke( this ); } finally { _startTime.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="StartTime"/> property value changes.</summary>
//		public event Action<Component_TimelineElement> StartTimeChanged;
//		ReferenceField<double> _startTime = 0.0;

//		[DefaultValue( 1.0 )]
//		public Reference<double> Length
//		{
//			get { if( _length.BeginGet() ) Length = _length.Get( this ); return _length.value; }
//			set { if( _length.BeginSet( ref value ) ) { try { LengthChanged?.Invoke( this ); } finally { _length.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Length"/> property value changes.</summary>
//		public event Action<Component_TimelineElement> LengthChanged;
//		ReferenceField<double> _length = 1.0;

//		/////////////////////////////////////////

//		protected virtual void OnPlay( Component_Timeline.ExecutionContext context )
//		{
//		}

//		internal void CallOnPlay( Component_Timeline.ExecutionContext context )
//		{
//			OnPlay( context );
//		}
//	}

//	//!!!!
//	//!!!!call method тоже можно
//	public class Component_TimelineElement_SetProperty : Component_TimelineElement
//	{


//		//!!!!Value для всего элемента

//		protected override void OnPlay( Component_Timeline.ExecutionContext context )
//		{
//			base.OnPlay( context );

//			//!!!!
//		}
//	}

//	//!!!!
//	//public class Component_TimelineKeyFrame : Component
//	public class Component_TimelineElement_SetProperty_KeyFrame : Component
//	{
//		[DefaultValue( 0.0 )]
//		public Reference<double> Time
//		{
//			get { if( _time.BeginGet() ) Time = _time.Get( this ); return _time.value; }
//			set { if( _time.BeginSet( ref value ) ) { try { TimeChanged?.Invoke( this ); } finally { _time.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Time"/> property value changes.</summary>
//		public event Action<Component_TimelineElement_SetProperty_KeyFrame> TimeChanged;
//		ReferenceField<double> _time = 0.0;

//		//!!!!
//		//object Value;
//	}

//}
