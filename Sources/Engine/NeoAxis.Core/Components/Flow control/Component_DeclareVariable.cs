// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Component representing a declaration of a virtual variable.
	/// </summary>
	public class Component_DeclareVariable : Component, IFlowExecutionComponent, IFlowGraphRepresentationData
	{
		bool valuePropertyNeedUpdate = true;
		ValueProperty valueProperty;
		//!!!!ValueChanged event? где еще эвенты с созданными свойствами
		object valuePropertyValue;

		/////////////////////////////////////////

		/// <summary>
		/// The input of the node.
		/// </summary>
		public FlowInput Entry
		{
			get { return new FlowInput( this, nameof( Entry ) ); }
		}

		//Exit
		ReferenceField<FlowInput> _exit;
		/// <summary>
		/// The output of the node.
		/// </summary>
		[Serialize]
		public Reference<FlowInput> Exit
		{
			get
			{
				if( _exit.BeginGet() )
					Exit = _exit.Get( this );
				return _exit.value;
			}
			set
			{
				if( _exit.BeginSet( ref value ) )
				{
					try { ExitChanged?.Invoke( this ); }
					finally { _exit.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Exit"/> property value changes.</summary>
		public event Action<Component_DeclareVariable> ExitChanged;

		//Type
		ReferenceField<Metadata.TypeInfo> _type = new Reference<Metadata.TypeInfo>( null, "System.String" );
		/// <summary>
		/// The type of the variable.
		/// </summary>
		[Serialize]
		public Reference<Metadata.TypeInfo> Type
		{
			get
			{
				if( _type.BeginGet() )
					Type = _type.Get( this );
				return _type.value;
			}
			set
			{
				if( _type.BeginSet( ref value ) )
				{
					try
					{
						TypeChanged?.Invoke( this );
						//if( Enabled )
						//	NeedUpdateCreatedMembers();
						valuePropertyNeedUpdate = true;
					}
					finally { _type.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Type"/> property value changes.</summary>
		public event Action<Component_DeclareVariable> TypeChanged;

		//VariableName
		ReferenceField<string> _variableName = "";
		/// <summary>
		/// The name of the variable.
		/// </summary>
		[DefaultValue( "" )]
		[Serialize]
		public Reference<string> VariableName
		{
			get
			{
				if( _variableName.BeginGet() )
					VariableName = _variableName.Get( this );
				return _variableName.value;
			}
			set
			{
				if( _variableName.BeginSet( ref value ) )
				{
					try { VariableNameChanged?.Invoke( this ); }
					finally { _variableName.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="VariableName"/> property value changes.</summary>
		public event Action<Component_DeclareVariable> VariableNameChanged;

		/////////////////////////////////////

		class ValueProperty : Metadata.Property
		{
			public ValueProperty( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				List<object> result = new List<object>();

				//Category
				if( attributeType.IsAssignableFrom( typeof( CategoryAttribute ) ) )
					result.Add( new CategoryAttribute( "Declare Variable" ) );

				return result.ToArray();
			}

			public override object GetValue( object obj, object[] index )
			{
				var c = (Component_DeclareVariable)obj;

				object result = null;

				object value = null;
				bool valueGotFromFlow = false;

				//get value from current flow
				var currentFlow = Flow.CurrentFlow;
				if( currentFlow != null )
				{
					if( currentFlow.GetVariable( c.GetVariableName(), out object unrefValue ) )
					{
						value = ReferenceUtility.MakeReference( TypeUnreferenced.GetNetType(), unrefValue, "" );
						valueGotFromFlow = true;
					}
				}

				//get init value
				if( !valueGotFromFlow )
				{
					value = c.valuePropertyValue;

					//update value for Reference type
					IReference iReference = value as IReference;
					if( iReference != null )
					{
						//get value by reference
						value = iReference.GetValue( obj );
						SetValue( obj, value, Indexers );
					}
				}

				result = value;

				//check the type. result can contains value with another type after change the type of property.
				if( result != null && !Type.IsAssignableFrom( MetadataManager.MetadataGetType( result ) ) )
					result = null;
				if( result == null && Type.GetNetType().IsValueType )
					result = Type.InvokeInstance( null );

				return result;
			}

			public override void SetValue( object obj, object value, object[] index )
			{
				var c = (Component_DeclareVariable)obj;

				c.valuePropertyValue = value;

				//set variable to current flow
				var currentFlow = Flow.CurrentFlow;
				currentFlow?.SetVariable( c.GetVariableName(), ReferenceUtility.GetUnreferencedValue( value ) );
			}
		}

		/////////////////////////////////////

		public string GetVariableName()
		{
			var name = VariableName.Value;
			if( string.IsNullOrEmpty( name ) )
				name = GetPathFromRoot();
			return name;
		}

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeTitle = string.Format( "Declare Variable \'{0}\'", GetVariableName() );
			data.NodeContentType = FlowGraphNodeContentType.Flow;
		}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem )
		{
			if( valueProperty != null )
			{
				var value = valueProperty.GetValue( this, null );
				valueProperty.SetValue( this, value, null );
			}

			FlowInput next = Exit;
			if( next != null )
				flow.ExecutionStack.Push( new Flow.ExecutionStackItem( next ) );
		}

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			valuePropertyNeedUpdate = true;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			valuePropertyNeedUpdate = true;
		}

		protected override IEnumerable<Metadata.Member> OnMetadataGetMembers()
		{
			foreach( var member in base.OnMetadataGetMembers() )
				yield return member;

			UpdateValueProperty();
			if( valueProperty != null )
				yield return valueProperty;
		}

		protected override Metadata.Member OnMetadataGetMemberBySignature( string signature )
		{
			UpdateValueProperty();
			if( valueProperty != null && valueProperty.Signature == signature )
				return valueProperty;

			return base.OnMetadataGetMemberBySignature( signature );
		}

		void UpdateValueProperty()
		{
			//check disabled
			if( !Enabled )
			{
				Clear();
				return;
			}

			if( !valuePropertyNeedUpdate )
				return;

			//do update

			Clear();
			var valueType = Type.Value;
			if( valueType == null )
				return;

			valuePropertyNeedUpdate = false;

			Type unrefNetType = valueType.GetNetType();
			var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
			var type = MetadataManager.GetTypeOfNetType( refNetType );

			valueProperty = new ValueProperty( this, "Value", false, type, valueType, new Metadata.Parameter[ 0 ], false );
			valueProperty.Serializable = SerializeType.Enable;
		}

		void Clear()
		{
			valueProperty = null;
		}
	}
}
