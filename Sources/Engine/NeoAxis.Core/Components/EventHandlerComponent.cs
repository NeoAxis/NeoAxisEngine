// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Component representing an event handler.
	/// </summary>
	[NewObjectDefaultName( "Event Handler" )]
	public class EventHandlerComponent : Component, IFlowGraphRepresentationData
	{
		bool duringUpdate;

		Metadata.Event subscribedEvent;
		object subscribedTarget;
		Delegate subscribedHandler;//Metadata.Delegate subscribedHandler;

		List<PropertyImpl> properties = new List<PropertyImpl>();
		Dictionary<string, PropertyImpl> propertyBySignature = new Dictionary<string, PropertyImpl>();
		List<PropertyImpl> propertyMethodParameters;

		object[] parameterValues;

		/////////////////////////////////////////

		/// <summary>
		/// The event to which the handler is subscribed.
		/// </summary>
		[Serialize]
		public Reference<ReferenceValueType_Event> Event
		{
			get { if( _event.BeginGet() ) Event = _event.Get( this ); return _event.value; }
			set
			{
				if( _event.BeginSet( this, ref value ) )
				{
					try
					{
						EventChanged?.Invoke( this );
						UpdateSubscription();
					}
					finally { _event.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Event"/> property value changes.</summary>
		public event Action<EventHandlerComponent> EventChanged;
		ReferenceField<ReferenceValueType_Event> _event;

		/// <summary>
		/// The object that generates the event.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		public Reference<Component> Sender
		{
			get { if( _sender.BeginGet() ) Sender = _sender.Get( this ); return _sender.value; }
			set
			{
				if( _sender.BeginSet( this, ref value ) )
				{
					try
					{
						SenderChanged?.Invoke( this );
						UpdateSubscription();
					}
					finally { _sender.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Sender"/> property value changes.</summary>
		public event Action<EventHandlerComponent> SenderChanged;
		ReferenceField<Component> _sender;

		/// <summary>
		/// Event handler method.
		/// </summary>
		[DefaultValue( null )]
		[Serialize]
		public Reference<ReferenceValueType_Method> HandlerMethod
		{
			get { if( _handlerMethod.BeginGet() ) HandlerMethod = _handlerMethod.Get( this ); return _handlerMethod.value; }
			set
			{
				if( _handlerMethod.BeginSet( this, ref value ) )
				{
					try
					{
						HandlerMethodChanged?.Invoke( this );
						UpdateSubscription();
					}
					finally { _handlerMethod.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="HandlerMethod"/> property value changes.</summary>
		public event Action<EventHandlerComponent> HandlerMethodChanged;
		ReferenceField<ReferenceValueType_Method> _handlerMethod = null;

		/// <summary>
		/// Event handler as a flow.
		/// </summary>
		[Serialize]
		public Reference<FlowInput> HandlerFlow
		{
			get { if( _handlerFlow.BeginGet() ) HandlerFlow = _handlerFlow.Get( this ); return _handlerFlow.value; }
			set
			{
				if( _handlerFlow.BeginSet( this, ref value ) )
				{
					try
					{
						HandlerFlowChanged?.Invoke( this );
						UpdateSubscription();
					}
					finally { _handlerFlow.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="HandlerFlow"/> property value changes.</summary>
		public event Action<EventHandlerComponent> HandlerFlowChanged;
		ReferenceField<FlowInput> _handlerFlow;

		[Flags]
		public enum WhenEnableEnum
		{
			Editor = 1,
			Simulation = 2,
			Resource = 4,
			Instance = 8,
		}

		/// <summary>
		/// Allows to configure in which cases the handler will be enabled. Editor - in the editor; Simulation - when Play; Resource - when the object is loaded as a resource; Instance - usual instance of the object.
		/// </summary>
		[FlowGraphBrowsable( false )]
		[DefaultValue( WhenEnableEnum.Simulation | WhenEnableEnum.Instance )]
		public Reference<WhenEnableEnum> WhenEnable
		{
			get { if( _whenEnable.BeginGet() ) WhenEnable = _whenEnable.Get( this ); return _whenEnable.value; }
			set { if( _whenEnable.BeginSet( this, ref value ) ) { try { WhenEnableChanged?.Invoke( this ); UpdateSubscription(); } finally { _whenEnable.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WhenEnable"/> property value changes.</summary>
		public event Action<EventHandlerComponent> WhenEnableChanged;
		ReferenceField<WhenEnableEnum> _whenEnable = WhenEnableEnum.Simulation | WhenEnableEnum.Instance;

		/////////////////////////////////////////

		class PropertyImpl : Metadata.Property
		{
			EventHandlerComponent creator;
			string category;
			string displayName;
			int invokeParameterIndex;

			////////////

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, EventHandlerComponent creator, string category, string displayName, int invokeParameterIndex )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
				this.creator = creator;
				this.category = category;
				this.displayName = displayName;
				this.invokeParameterIndex = invokeParameterIndex;
			}

			public string Category
			{
				get { return category; }
				set { category = value; }
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				List<object> result = new List<object>();

				//!!!!может это в Metadata.Property?
				//Category
				if( attributeType.IsAssignableFrom( typeof( CategoryAttribute ) ) )
				{
					if( !string.IsNullOrEmpty( category ) )
						result.Add( new CategoryAttribute( category ) );
				}
				//DisplayName
				if( attributeType.IsAssignableFrom( typeof( DisplayNameAttribute ) ) )
				{
					if( !string.IsNullOrEmpty( displayName ) )
						result.Add( new DisplayNameAttribute( displayName ) );
				}

				return result.ToArray();
			}

			public override object GetValue( object obj, object[] index )
			{
				object result = null;

				var values = creator.parameterValues;
				if( values != null && invokeParameterIndex < values.Length )
					result = values[ invokeParameterIndex ];

				//check the type. result can contains value with another type after change the type of property.
				if( result != null && !Type.IsAssignableFrom( MetadataManager.MetadataGetType( result ) ) )
					result = null;
				if( result == null && Type.GetNetType().IsValueType )
					result = Type.InvokeInstance( null );

				return result;
			}

			public override void SetValue( object obj, object value, object[] index )
			{
			}
		}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Sender ):
					var eventValue = Event.Value;
					if( eventValue != null )
					{
						var _event = eventValue.Member as Metadata.Event;
						if( _event != null && _event.Static )
							skip = true;
					}
					break;

				case nameof( HandlerMethod ):
					if( HandlerFlow.ReferenceSpecified )
						skip = true;
					break;

				case nameof( HandlerFlow ):
					if( HandlerMethod.ReferenceSpecified )
						skip = true;
					break;
				}
			}
		}

		public void UpdateSubscription()
		{
			if( duringUpdate )
				return;
			duringUpdate = true;

			try
			{
				var subscribed = false;

				if( EnabledInHierarchy )
				{
					var whenEnable = WhenEnable.Value;
					if( ( ( whenEnable & WhenEnableEnum.Editor ) != 0 ) && EngineApp.IsEditor ||
						( ( whenEnable & WhenEnableEnum.Simulation ) != 0 ) && EngineApp.IsSimulation )
					{
						var ins = ComponentUtility.GetResourceInstanceByComponent( this );
						var isResource = ins != null && ins.InstanceType == Resource.InstanceType.Resource;

						if( ( ( whenEnable & WhenEnableEnum.Resource ) != 0 ) && isResource ||
							( ( whenEnable & WhenEnableEnum.Instance ) != 0 ) && !isResource )
						{
							subscribed = true;
							Subscribe();
						}
					}
				}
				//else
				//	Unsubscribe();

				if( !subscribed )
					Unsubscribe();
			}
			finally
			{
				duringUpdate = false;
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			UpdateSubscription();
		}

		void Subscribe()
		{
			Unsubscribe();

			var eventValue = Event.Value;
			if( eventValue != null )
			{
				var _event = eventValue.Member as Metadata.Event;
				if( _event != null )
				{

					//HandlerMethod
					var methodValue = HandlerMethod.Value;
					if( methodValue != null )
					{
						var method = methodValue.Member as Metadata.Method;
						if( method != null )
						{
							try
							{
								var handler = Metadata.Delegate.Create( _event.EventHandlerType, method.Static ? null : methodValue.Object, method );

								object target = null;
								if( !_event.Static )
								{
									target = Sender.Value;
									if( target == null )
										target = eventValue.Object;
								}

								_event.AddEventHandler( target, handler );

								subscribedEvent = _event;
								subscribedTarget = target;
								subscribedHandler = handler.NetDelegate;
							}
							catch( Exception e )
							{
								Log.Warning( "EventHandlerComponent: Subscribe: " + e.Message + " (" + GetPathFromRoot( true ) + ")." );
							}
						}
					}

					//HandlerFlow
					var flowValue = HandlerFlow.Value;
					if( flowValue != null )
					{
						try
						{
							//!!!!need support virtual types (far future)

							var parameters = _event.EventHandlerType.GetNetType().GetMethod( "Invoke" ).GetParameters();

							Delegate handler;

							if( CanUseSimpleHandlerMethodMode( parameters ) )
							{
								//simple mode. use SimpleHandler methods.

								var methodName = "SimpleHandler" + parameters.Length.ToString();
								var method = GetType().GetMethod( methodName, BindingFlags.Instance | BindingFlags.NonPublic );
								handler = Delegate.CreateDelegate( _event.EventHandlerType.GetNetType(), this, method );
								//var handler = Metadata.Delegate.Create( _event.EventHandlerType, method.Static ? null : this, method );

							}
							else
							{
								// dynamic method mode. ref, out, value types supported.
								//TODO: check performance. cache ?
								handler = BuildDynamicHandler( _event.EventHandlerType.GetNetType(), Handler );
							}


							object target = null;
							if( !_event.Static )
							{
								target = Sender.Value;
								if( target == null )
									target = eventValue.Object;
							}

							_event.AddEventHandler( target, handler );

							subscribedEvent = _event;
							subscribedTarget = target;
							subscribedHandler = handler;






							//parameters
							propertyMethodParameters = new List<PropertyImpl>();
							int invokeParameterIndexCounter = 0;
							for( int nParameter = 0; nParameter < parameters.Length; nParameter++ )
							//for( int nParameter = 0; nParameter < method.Parameters.Length; nParameter++ )
							{
								var parameter = parameters[ nParameter ];//var parameter = method.Parameters[ nParameter ];

								if( !parameter.IsOut && !parameter.IsRetval )//if( !parameter.Output && !parameter.ReturnValue )
								{
									//!!!!имя еще как-то фиксить?
									//string name = null;
									//if( name == null )
									var name = parameter.Name.Substring( 0, 1 ).ToUpper() + parameter.Name.Substring( 1 );
									//name = parameter.Name;

									string namePrefix = "__parameter_";
									string displayName = TypeUtility.DisplayNameAddSpaces( name );

									var parameterType = MetadataManager.GetTypeOfNetType( parameter.ParameterType );
									var p = new PropertyImpl( this, namePrefix + name, false, parameterType, parameterType, new Metadata.Parameter[ 0 ], true, this, "Parameters", displayName, invokeParameterIndexCounter );
									//var p = new PropertyImpl( this, namePrefix + name, false, parameter.Type, parameter.Type, new Metadata.Parameter[ 0 ], true, this, "Parameters", displayName, invokeParameterIndexCounter );
									p.Description = "";
									//p.Serializable = true;

									properties.Add( p );
									propertyBySignature[ p.Signature ] = p;
									propertyMethodParameters.Add( p );
								}

								//!!!!может индекс юзать?
								if( !parameter.IsRetval )//if( !parameter.ReturnValue )
									invokeParameterIndexCounter++;
							}

						}
						catch( Exception e )
						{
							Log.Warning( e.Message );
						}
					}

				}
			}
		}

		static Delegate BuildDynamicHandler( Type delegateType, Action<object[]> func )
		{
			// https://stackoverflow.com/a/15705945/6382179
			// https://github.com/buunguyen/fasterflect/blob/3b7fd55bc0009cd1cc691ad0eea7d59de132117e/Fasterflect/Fasterflect/Extensions/Services/EventExtensions.cs

			var invokeMethod = delegateType.GetMethod( "Invoke" );
			var parameters = invokeMethod.GetParameters().Select( parm => Expression.Parameter( parm.ParameterType, parm.Name ) ).ToArray();
			var instance = func.Target == null ? null : Expression.Constant( func.Target );
			var converted = parameters.Select( parm => Expression.Convert( parm, typeof( object ) ) );
			var call = Expression.Call( instance, func.Method, Expression.NewArrayInit( typeof( object ), converted ) );
			var body = invokeMethod.ReturnType == typeof( void ) ? (Expression)call : Expression.Convert( call, invokeMethod.ReturnType );
			var expr = Expression.Lambda( delegateType, body, parameters );
			return expr.Compile();
		}

		void Unsubscribe()
		{
			if( subscribedEvent != null )
			{
				try
				{
					subscribedEvent.RemoveEventHandler( subscribedTarget, subscribedHandler );
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
				}

				subscribedEvent = null;
				subscribedTarget = null;
				subscribedHandler = null;
			}

			properties.Clear();
			propertyBySignature.Clear();
			propertyMethodParameters = null;
		}

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.FlowStart;
		}

		protected override IEnumerable<Metadata.Member> OnMetadataGetMembers()
		{
			foreach( var member in base.OnMetadataGetMembers() )
				yield return member;

			foreach( var p in properties )
				yield return p;
		}

		protected override Metadata.Member OnMetadataGetMemberBySignature( string signature )
		{
			if( propertyBySignature.TryGetValue( signature, out PropertyImpl p ) )
				return p;

			return base.OnMetadataGetMemberBySignature( signature );
		}

		void Handler( object[] parameterValues )
		{
			//set properties
			this.parameterValues = parameterValues;

			//flow start
			var input = HandlerFlow.Value;
			if( input != null )
				Flow.Execute( ParentRoot?.HierarchyController, input, null );
		}

		bool CanUseSimpleHandlerMethodMode( ParameterInfo[] parameters )
		{
			if( parameters.Length > 6 )
				return false;

			foreach( var p in parameters )
			{
				if( p.ParameterType.IsValueType )
					return false;
				if( p.ParameterType.IsByRef )
					return false;
				if( p.IsOut )
					return false;
				if( p.IsRetval )
					return false;
			}

			return true;
		}

		void SimpleHandler0() { Handler( new object[ 0 ] ); }
		void SimpleHandler1( object p1 ) { Handler( new object[] { p1 } ); }
		void SimpleHandler2( object p1, object p2 ) { Handler( new object[] { p1, p2 } ); }
		void SimpleHandler3( object p1, object p2, object p3 ) { Handler( new object[] { p1, p2, p3 } ); }
		void SimpleHandler4( object p1, object p2, object p3, object p4 ) { Handler( new object[] { p1, p2, p3, p4 } ); }
		void SimpleHandler5( object p1, object p2, object p3, object p4, object p5 ) { Handler( new object[] { p1, p2, p3, p4, p5 } ); }
		void SimpleHandler6( object p1, object p2, object p3, object p4, object p5, object p6 ) { Handler( new object[] { p1, p2, p3, p4, p5, p6 } ); }
	}
}
