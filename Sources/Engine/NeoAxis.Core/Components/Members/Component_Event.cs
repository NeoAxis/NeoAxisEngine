// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// The component for adding a virtual event to the parent component.
	/// </summary>
	public class Component_Event : Component_Member
	{
		Metadata.Event createdEvent;

		//

		[DefaultValue( null )]
		[Serialize]
		public Reference<Metadata.TypeInfo> EventHandlerType
		{
			get { if( _eventHandlerType.BeginGet() ) EventHandlerType = _eventHandlerType.Get( this ); return _eventHandlerType.value; }
			set
			{
				if( _eventHandlerType.BeginSet( ref value ) )
				{
					try
					{
						//!!!!update

						EventHandlerTypeChanged?.Invoke( this );
					}
					finally { _eventHandlerType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="EventHandlerType"/> property value changes.</summary>
		public event Action<Component_Event> EventHandlerTypeChanged;
		ReferenceField<Metadata.TypeInfo> _eventHandlerType = null;

		////!!!!!может делегат?
		////!!!!сбрасывать кеш
		//Reference<List<Component_MemberParameter>> parameters = new List<Component_MemberParameter>();
		//[Serialize]
		//public virtual Reference<List<Component_MemberParameter>> Parameters
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( parameters.GetByReference ) )
		//			Parameters = parameters.GetValue( this );
		//		return parameters;
		//	}
		//	set
		//	{
		//		if( parameters == value ) return;
		//		parameters = value;
		//		ParametersChanged?.Invoke( this );
		//		NeedUpdateCreatedMembers();
		//	}
		//}
		//public event Action<Component_Event> ParametersChanged;


		//!!!!!
		//bool serializable;

		//!!!!
		//bool cloneable;

		/////////////////////////////////////

		class EventImpl : Metadata.Event
		{
			Component_Event creator;

			public EventImpl( object owner, string name, bool isStatic, Metadata.TypeInfo eventHandlerType, Component_Event creator )
				: base( owner, name, isStatic, eventHandlerType )
			{
				this.creator = creator;
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				//!!!!!
				return Array.Empty<object>();
			}

			public override void Invoke( object obj, object[] parameters )
			{
				//!!!!
			}

			//!!!!

			public override void AddEventHandler( object target, Delegate handler )
			{
				//!!!!
			}

			public override void RemoveEventHandler( object target, Delegate handler )
			{
				//!!!!
			}

			public override void AddEventHandler( object target, Metadata.Delegate handler )
			{
				//!!!!
			}

			public override void RemoveEventHandler( object target, Metadata.Delegate handler )
			{
				//!!!!
			}
		}

		/////////////////////////////////////

		public Component_Event()
		{
			ComponentsChanged += Component_Event_ComponentsChanged;
		}

		void Component_Event_ComponentsChanged( Component obj )
		{
			NeedUpdateCreatedMembers();
		}

		//!!!!cache?
		public string GetDisplayName()
		{
			var b = new StringBuilder();
			b.Append( Name );

			//!!!!

			//b.Append( '(' );
			//bool paramsWasAdded = false;

			//List<Component_MemberParameter> parameters2 = Parameters;
			//for( int n = 0; n < parameters2.Count; n++ )
			//{
			//	var p = parameters2[ n ];
			//	if( !p.ReturnValue )
			//	{
			//		b.Append( paramsWasAdded ? ", " : " " );

			//		if( p.ByReference )
			//			b.Append( "ref " );
			//		else if( p.Output )
			//			b.Append( "out " );
			//		//if( p.Output )
			//		//	b.Append( p.In ? "ref " : "out " );

			//		b.Append( p.Type );
			//		b.Append( ' ' );
			//		b.Append( p.Name );

			//		//!!!!default value

			//		paramsWasAdded = true;
			//	}
			//}
			//if( paramsWasAdded )
			//	b.Append( ' ' );
			//b.Append( ')' );

			//b.Append( '(' );
			//if( parameters.Length != 0 )
			//{
			//	b.Append( ' ' );
			//	for( int n = 0; n < parameters.Length; n++ )
			//	{
			//		if( n != 0 )
			//			b.Append( ", " );
			//		var p = parameters[ n ];
			//		if( p.IsOut )
			//			b.Append( p.IsIn ? "ref " : "out " );
			//		b.Append( p.TypeName );
			//		b.Append( ' ' );
			//		b.Append( Name );

			//		//!!!!default value
			//	}
			//	b.Append( ' ' );
			//}
			//b.Append( ')' );
			return b.ToString();
		}

		public override string ToString()
		{
			return GetDisplayName();
		}

		public override void NeedUpdateCreatedMembers()
		{
			base.NeedUpdateCreatedMembers();

			createdEvent = null;
		}

		EventImpl CreateEvent()
		{
			Metadata.TypeInfo eventHandlerType = null;

			//EventHandlerType property
			var eventHandlerTypeProperty = EventHandlerType;
			if( eventHandlerTypeProperty.ReferenceSpecified )
			{
				eventHandlerType = eventHandlerTypeProperty;
			}
			else
			{
				//!!!!

				//var parameters = new List<Component_MemberParameter>();
				//foreach( var p in GetComponents<Component_MemberParameter>( false, false ) )
				//{
				//	if( p.Enabled )
				//		parameters.Add( p );
				//}

				//var parameters2 = new List<Metadata.Parameter>();
				//foreach( var p in parameters )
				//{
				//	Metadata.Parameter p2 = p.CreateMetadataParameter();
				//	if( p2 == null )
				//	{
				//		//!!!!
				//		return null;
				//	}

				//	parameters2.Add( p2 );
				//}

				////!!!!?
				//var args = new List<Type>();
				//foreach( var param in netMethod.GetParameters() )
				//	args.Add( param.ParameterType );
				//args.Add( netMethod.ReturnType );
				//var delDecltype = Expression.GetDelegateType( args.ToArray() );
			}

			if( eventHandlerType != null )
			{
				var _event = new EventImpl( Parent, Name, Static, eventHandlerType, this );
				_event.Description = Description;
				return _event;
			}
			else
				return null;
		}

		protected override void CreateMembers( List<Metadata.Member> created )
		{
			var _event = CreateEvent();
			if( _event != null )
			{
				createdEvent = _event;
				created.Add( _event );
			}
		}
	}
}
