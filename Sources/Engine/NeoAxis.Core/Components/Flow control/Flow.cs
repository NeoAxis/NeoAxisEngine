// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// An interface for components to support flows of the engine.
	/// </summary>
	public interface IFlowExecutionComponent
	{
		void FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem );
	}

	/// <summary>
	/// A flow instance of the engine.
	/// </summary>
	public class Flow
	{
		[ThreadStatic]
		static List<Flow> flowStack = new List<Flow>();

		//

		Stack<ExecutionStackItem> executionStack = new Stack<ExecutionStackItem>();

		//!!!!юзать как переменную?
		//object initParameter;
		//!!!!можно переменные юзать
		//object userData;
		Dictionary<string, object> variables = new Dictionary<string, object>();

		Dictionary<object, object> internalVariables = new Dictionary<object, object>();

		///////////////////////////////////////////////

		/// <summary>
		/// Represents a stack item of execution of <see cref="Flow"/>.
		/// </summary>
		public class ExecutionStackItem
		{
			FlowInput flowInput;
			IFlowExecutionComponent component;

			public ExecutionStackItem( FlowInput flowInput )
			{
				this.flowInput = flowInput;
				this.component = flowInput.Owner;
			}

			public ExecutionStackItem( IFlowExecutionComponent component )
			{
				this.component = component;
			}

			public FlowInput FlowInput
			{
				get { return flowInput; }
			}

			public IFlowExecutionComponent Component
			{
				get { return component; }
			}
		}

		///////////////////////////////////////////////

		static void InitFlowStack()
		{
			if( flowStack == null )
				flowStack = new List<Flow>();
		}

		public static Flow[] FlowStack
		{
			get
			{
				InitFlowStack();
				return flowStack.ToArray();
			}
		}

		public static Flow CurrentFlow
		{
			get
			{
				InitFlowStack();
				if( flowStack.Count != 0 )
					return flowStack[ flowStack.Count - 1 ];
				return null;
			}
		}

		public static Flow ExecuteWithoutRemoveFromStack( FlowInput entry, IEnumerable<Tuple<string, object>> initVariables = null )
		{
			InitFlowStack();

			var flow = new Flow();
			//flow.initParameter = initParameter;
			flowStack.Add( flow );

			if( initVariables != null )
			{
				foreach( var t in initVariables )
					flow.SetVariable( t.Item1, t.Item2 );
			}
			//if( initParameter != null )
			//	flow.SetVariable( "InitParameter", initParameter );

			flow.Process( entry );

			return flow;
		}

		public static void RemoveFromStack( Flow flow )
		{
			flowStack.RemoveAt( flowStack.Count - 1 );
		}

		public static void Execute( FlowInput entry, IEnumerable<Tuple<string, object>> initVariables = null )
		{
			var flow = ExecuteWithoutRemoveFromStack( entry, initVariables );
			RemoveFromStack( flow );
			//return flow;
		}

		//public static Flow Execute( FlowInput entry, IEnumerable<Tuple<string, object>> initVariables = null )
		////public static Flow Run( FlowInput entry, object initParameter = null )
		//{
		//	InitFlowStack();

		//	var flow = new Flow();
		//	//flow.initParameter = initParameter;
		//	flowStack.Add( flow );

		//	if( initVariables != null )
		//	{
		//		foreach( var t in initVariables )
		//			flow.SetVariable( t.Item1, t.Item2 );
		//	}
		//	//if( initParameter != null )
		//	//	flow.SetVariable( "InitParameter", initParameter );

		//	flow.Process( entry );

		//	flowStack.RemoveAt( flowStack.Count - 1 );

		//	return flow;
		//}

		///////////////////////////////////////////////

		//public object InitParameter
		//{
		//	get { return initParameter; }
		//}

		//public object UserData
		//{
		//	get { return userData; }
		//	set { userData = value; }
		//}

		public Dictionary<string, object> Variables
		{
			get { return variables; }
		}

		public object GetVariable( string name )
		{
			variables.TryGetValue( name, out object value );
			return value;
		}

		public bool GetVariable( string name, out object value )
		{
			return variables.TryGetValue( name, out value );
		}

		public void SetVariable( string name, object value )
		{
			variables[ name ] = value;
		}

		//!!!!так? другие типы тоже надо указывать как-то
		public void SetVariable( string name, string stringValue )
		{
			variables[ name ] = stringValue;
		}

		public static object GetVariableFromCurrentFlow( string name )
		{
			if( CurrentFlow != null )
			{
				CurrentFlow.variables.TryGetValue( name, out object value );
				return value;
			}
			return null;
		}

		//!!!!rename
		public static T GetVariableFromCurrentFlow_Generic<T>( string name )
		{
			object value = null;
			if( CurrentFlow != null )
				CurrentFlow.variables.TryGetValue( name, out value );

			var expectedType = typeof( T );

			//auto convert types
			if( value != null && !expectedType.IsAssignableFrom( value.GetType() ) )
			{
				var newValue = MetadataManager.AutoConvertValue( value, expectedType );
				if( newValue == null )
					newValue = MetadataManager.AutoConvertValue( ReferenceUtility.GetUnreferencedValue( value ), expectedType );
				value = newValue;
			}
			//default for value types
			if( value == null && expectedType.IsValueType )
				value = Activator.CreateInstance( expectedType );

			return (T)value;
		}

		//!!!!пока так. потом удалить
		public static string GetVariableFromCurrentFlow_String( string name )
		{
			return GetVariableFromCurrentFlow_Generic<string>( name );
		}
		public static int GetVariableFromCurrentFlow_Integer( string name )
		{
			return GetVariableFromCurrentFlow_Generic<int>( name );
		}
		public static double GetVariableFromCurrentFlow_Double( string name )
		{
			return GetVariableFromCurrentFlow_Generic<double>( name );
		}

		public static void SetVariableToCurrentFlow( string name, object value )
		{
			if( CurrentFlow != null )
				CurrentFlow.variables[ name ] = value;
		}

		//!!!!так? другие типы тоже надо указывать как-то
		public static void SetVariableToCurrentFlow( string name, string stringValue )
		{
			if( CurrentFlow != null )
				CurrentFlow.variables[ name ] = stringValue;
		}

		//!!!!for Component_SetVariable
		public static void SetVariableToCurrentFlow( Component_DeclareVariable variable, object value )
		{
			if( CurrentFlow != null )
				CurrentFlow.variables[ variable.GetVariableName() ] = value;
		}

		public static object GetVariableFromAnyFlowInStack( string name )
		{
			InitFlowStack();
			foreach( var flow in flowStack.GetReverse() )
			{
				if( flow.GetVariable( name, out object value ) )
					return value;
			}
			return null;
		}

		public static Dictionary<string, object> GetAllVariablesFromFlowStack()
		{
			var result = new Dictionary<string, object>();
			InitFlowStack();
			foreach( var flow in flowStack )
			{
				foreach( var item in flow.Variables )
					result[ item.Key ] = item.Value;
			}
			return result;
		}

		public Dictionary<object, object> InternalVariables
		{
			get { return internalVariables; }
		}

		public Stack<ExecutionStackItem> ExecutionStack
		{
			get { return executionStack; }
		}

		void Process( FlowInput entry )
		{
			if( entry != null )
				ExecutionStack.Push( new ExecutionStackItem( entry ) );

			while( ExecutionStack.Count != 0 )
			{
				var item = ExecutionStack.Pop();
				item.Component.FlowExecution( this, item );
			}
		}
	}
}
