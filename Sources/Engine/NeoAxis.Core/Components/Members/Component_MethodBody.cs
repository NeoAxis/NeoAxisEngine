// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	/// The beginning of the body of the method in the flow graph.
	/// </summary>
	public class Component_MethodBody : Component, IFlowGraphRepresentationData
	{
		bool needUpdate = true;

		Metadata.Member memberObject;
		List<PropertyImpl> properties = new List<PropertyImpl>();
		Dictionary<string, PropertyImpl> propertyBySignature = new Dictionary<string, PropertyImpl>();

		PropertyImpl propertyThis;
		List<PropertyImpl> propertyMethodParameters;
		PropertyImpl propertyPropertyValue;

		[ThreadStatic]
		Stack<InvokeStackItem> invokeStack;

		/////////////////////////////////////////

		//Definition
		ReferenceField<Component_Member> _definition;
		[FlowGraphBrowsable( false )]
		public Reference<Component_Member> Definition
		{
			get
			{
				if( _definition.BeginGet() )
					Definition = _definition.Get( this );
				return _definition.value;
			}
			set
			{
				if( _definition.BeginSet( ref value ) )
				{
					try
					{
						DefinitionChanged?.Invoke( this );

						//!!!!
					}
					finally { _definition.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Definition"/> property value changes.</summary>
		public event Action<Component_MethodBody> DefinitionChanged;

		//PropertyAccessorType
		public enum PropertyAccessorTypeEnum
		{
			Get,
			Set,
		}
		ReferenceField<PropertyAccessorTypeEnum> _propertyAccessorType;
		[Serialize]
		[FlowGraphBrowsable( false )]
		public Reference<PropertyAccessorTypeEnum> PropertyAccessorType
		{
			get
			{
				if( _propertyAccessorType.BeginGet() )
					PropertyAccessorType = _propertyAccessorType.Get( this );
				return _propertyAccessorType.value;
			}
			set
			{
				if( _propertyAccessorType.BeginSet( ref value ) )
				{
					try
					{
						PropertyAccessorTypeChanged?.Invoke( this );

						//!!!!
					}
					finally { _propertyAccessorType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="PropertyAccessorType"/> property value changes.</summary>
		public event Action<Component_MethodBody> PropertyAccessorTypeChanged;

		//BodyEnd
		ReferenceField<Component_MethodBodyEnd> _bodyEnd;
		[Serialize]
		[FlowGraphBrowsable( false )]
		public Reference<Component_MethodBodyEnd> BodyEnd
		{
			get
			{
				if( _bodyEnd.BeginGet() )
					BodyEnd = _bodyEnd.Get( this );
				return _bodyEnd.value;
			}
			set
			{
				if( _bodyEnd.BeginSet( ref value ) )
				{
					try
					{
						BodyEndChanged?.Invoke( this );

						//!!!!
					}
					finally { _bodyEnd.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BodyEnd"/> property value changes.</summary>
		public event Action<Component_MethodBody> BodyEndChanged;

		//Flow
		ReferenceField<FlowInput> _flow;
		[Serialize]
		public Reference<FlowInput> Flow
		{
			get
			{
				if( _flow.BeginGet() )
					Flow = _flow.Get( this );
				return _flow.value;
			}
			set
			{
				if( _flow.BeginSet( ref value ) )
				{
					try { FlowChanged?.Invoke( this ); }
					finally { _flow.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Flow"/> property value changes.</summary>
		public event Action<Component_MethodBody> FlowChanged;

		/////////////////////////////////////////

		class PropertyImpl : Metadata.Property
		{
			Component_MethodBody creator;
			string category;
			string displayName;
			ParameterType parameterType;
			int invokeParameterIndex;

			////////////

			public enum ParameterType
			{
				This,
				Parameter,
			}

			////////////

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, Component_MethodBody creator, string category, string displayName, ParameterType parameterType, int invokeParameterIndex )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
				this.creator = creator;
				this.category = category;
				this.displayName = displayName;
				this.parameterType = parameterType;
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

				if( creator.invokeStack != null && creator.invokeStack.Count != 0 )
				{
					var invokeItem = creator.invokeStack.Peek();

					switch( parameterType )
					{
					case ParameterType.This:
						result = invokeItem.obj;
						break;

					case ParameterType.Parameter:
						if( invokeParameterIndex < invokeItem.parameters.Length )
							result = invokeItem.parameters[ invokeParameterIndex ];
						break;
					}
				}

				//!!!!надо это?

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

		class InvokeStackItem
		{
			public object obj;
			public object[] parameters;
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
				case "PropertyAccessorType":
					if( Definition.Value as Component_Property == null )
						skip = true;
					break;
				}
			}
		}

		protected override IEnumerable<Metadata.Member> OnMetadataGetMembers()
		{
			foreach( var member in base.OnMetadataGetMembers() )
				yield return member;

			Update();
			foreach( var p in properties )
				yield return p;
		}

		protected override Metadata.Member OnMetadataGetMemberBySignature( string signature )
		{
			Update();
			if( propertyBySignature.TryGetValue( signature, out PropertyImpl p ) )
				return p;

			return base.OnMetadataGetMemberBySignature( signature );
		}

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			needUpdate = true;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			needUpdate = true;
		}

		Metadata.Member GetNeededMember()
		{
			var definition = Definition.Value;
			if( definition != null )
			{
				var members = definition.CreatedMembers;
				if( members != null && members.Length != 0 )
					return members[ 0 ];
			}
			return null;
		}

		void Update()
		{
			//check disabled
			if( !Enabled )
			{
				Clear();
				return;
			}

			//!!!!slowly?
			//get actual member
			Metadata.Member newMemberObject = GetNeededMember();

			//check for updates
			if( !needUpdate && newMemberObject != memberObject )
				needUpdate = true;

			//nothing to update
			if( !needUpdate )
				return;

			//do update

			Clear();
			memberObject = newMemberObject;
			needUpdate = false;

			if( memberObject != null )
			{
				//method
				var method = memberObject as Metadata.Method;
				if( method != null && method.Owner != null )
				{
					//This
					if( !method.Static && !method.Constructor )
					{
						Metadata.TypeInfo creatorType = method.Owner as Metadata.TypeInfo;
						if( creatorType == null )
							creatorType = MetadataManager.MetadataGetType( method.Owner );

						string namePrefix = "__this_";
						var p = new PropertyImpl( this, namePrefix + "This", false, creatorType, creatorType, new Metadata.Parameter[ 0 ], true, this, "Parameters", "This", PropertyImpl.ParameterType.This, 0 );
						p.Description = "";
						//p.Serializable = true;

						properties.Add( p );
						propertyBySignature[ p.Signature ] = p;
						propertyThis = p;
					}

					//parameters
					propertyMethodParameters = new List<PropertyImpl>();
					int invokeParameterIndexCounter = 0;
					for( int nParameter = 0; nParameter < method.Parameters.Length; nParameter++ )
					{
						var parameter = method.Parameters[ nParameter ];

						if( !parameter.Output && !parameter.ReturnValue )
						{
							//!!!!имя еще как-то фиксить?
							//string name = null;
							//if( name == null )
							var name = parameter.Name.Substring( 0, 1 ).ToUpper() + parameter.Name.Substring( 1 );
							//name = parameter.Name;

							string namePrefix = "__parameter_";
							string displayName = TypeUtility.DisplayNameAddSpaces( name );

							var p = new PropertyImpl( this, namePrefix + name, false, parameter.Type, parameter.Type, new Metadata.Parameter[ 0 ], true, this, "Members", displayName, PropertyImpl.ParameterType.Parameter, invokeParameterIndexCounter );
							p.Description = "";
							//p.Serializable = true;

							properties.Add( p );
							propertyBySignature[ p.Signature ] = p;
							propertyMethodParameters.Add( p );
						}

						//!!!!может индекс юзать?
						if( !parameter.ReturnValue )
							invokeParameterIndexCounter++;
					}
				}

				//property
				var property = memberObject as Metadata.Property;
				if( property != null )
				{
					//This
					if( !property.Static )
					{
						Metadata.TypeInfo creatorType = property.Owner as Metadata.TypeInfo;
						if( creatorType == null )
							creatorType = MetadataManager.MetadataGetType( property.Owner );

						string namePrefix = "__this_";
						var p = new PropertyImpl( this, namePrefix + "This", false, creatorType, creatorType, new Metadata.Parameter[ 0 ], true, this, "Members", "This", PropertyImpl.ParameterType.This, 0 );
						p.Description = "";
						//p.Serializable = true;

						properties.Add( p );
						propertyBySignature[ p.Signature ] = p;
						propertyThis = p;
					}

					//value
					if( PropertyAccessorType.Value == PropertyAccessorTypeEnum.Set )
					{
						//!!!!может быть тип свойства со ссылкой. так как сейчас норм?

						string namePrefix = "__value_";
						var p = new PropertyImpl( this, namePrefix + "Value", false, property.Type, property.TypeUnreferenced, new Metadata.Parameter[ 0 ], true, this, "Members", "Value", PropertyImpl.ParameterType.Parameter, 0 );
						p.Description = "";
						//p.Serializable = true;

						properties.Add( p );
						propertyBySignature[ p.Signature ] = p;
						propertyPropertyValue = p;
					}

					//!!!!Indexers

				}

				//!!!!другие
			}
		}

		void Clear()
		{
			memberObject = null;
			properties.Clear();
			propertyBySignature.Clear();
			propertyThis = null;
			propertyMethodParameters = null;
			propertyPropertyValue = null;
		}

		public object Invoke( object obj, object[] parameters )
		{
			object returnValue = null;

			var invokeItem = new InvokeStackItem();
			invokeItem.obj = obj;
			invokeItem.parameters = parameters;

			if( invokeStack == null )
				invokeStack = new Stack<InvokeStackItem>();
			invokeStack.Push( invokeItem );

			//execute flow
			Flow flow = null;
			if( Flow.Value != null )
				flow = NeoAxis.Flow.ExecuteWithoutRemoveFromStack( ParentRoot?.HierarchyController, Flow, null );

			//get ref, out, return value parameters
			var bodyEnd = BodyEnd.Value;
			if( bodyEnd != null )
			{
				foreach( var p in bodyEnd.properties )
				{
					switch( p.parameterType )
					{
					case Component_MethodBodyEnd.PropertyImpl.ParameterType.Parameter:
						if( p.invokeParameterIndex < parameters.Length )
							parameters[ p.invokeParameterIndex ] = ReferenceUtility.GetUnreferencedValue( p.GetValue( bodyEnd, null ) );
						break;

					case Component_MethodBodyEnd.PropertyImpl.ParameterType.ReturnValue:
						returnValue = ReferenceUtility.GetUnreferencedValue( p.GetValue( bodyEnd, null ) );
						break;
					}
				}
			}

			if( flow != null )
				NeoAxis.Flow.RemoveFromStack( flow );

			invokeStack.Pop();

			//returnValue = EngineApp.EngineTime % 4.0 > 2.0;

			return returnValue;
		}

		[Browsable( false )]
		public Metadata.Member MemberObject
		{
			get { return memberObject; }
		}

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.MethodBody;
		}
	}
}
