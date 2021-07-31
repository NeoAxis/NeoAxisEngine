// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// The component used to set the value of a virtual variable.
	/// </summary>
	public class Component_SetVariable : Component, IFlowGraphRepresentationData, IFlowExecutionComponent
	{
		bool valuePropertyNeedUpdate = true;
		ValueProperty valueProperty;
		Metadata.TypeInfo valuePropertyCreatedWithType;
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
		public event Action<Component_SetVariable> ExitChanged;

		//Variable
		ReferenceField<Component_DeclareVariable> _variable;
		/// <summary>
		/// The variable whose value needs to be set.
		/// </summary>
		[Serialize]
		public Reference<Component_DeclareVariable> Variable
		{
			get
			{
				if( _variable.BeginGet() )
					Variable = _variable.Get( this );
				return _variable.value;
			}
			set
			{
				if( _variable.BeginSet( ref value ) )
				{
					try
					{
						VariableChanged?.Invoke( this );
						valuePropertyNeedUpdate = true;
					}
					finally { _variable.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Variable"/> property value changes.</summary>
		public event Action<Component_SetVariable> VariableChanged;

		//!!!!указывать VariableName как второй способ?

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
					result.Add( new CategoryAttribute( "Set Variable" ) );

				return result.ToArray();
			}

			public override object GetValue( object obj, object[] index )
			{
				object result = null;

				var c = (Component_SetVariable)obj;
				var value = c.valuePropertyValue;

				//update value for Reference type
				IReference iReference = value as IReference;
				if( iReference != null )
				{
					//get value by reference
					value = iReference.GetValue( obj );
					SetValue( obj, value, Indexers );
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
				var c = (Component_SetVariable)obj;
				c.valuePropertyValue = value;
			}
		}

		/////////////////////////////////////

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			var variable = Variable.Value;
			string variableName = variable != null ? variable.GetVariableName() : "Null";
			data.NodeTitle = string.Format( "Set Variable \'{0}\'", variableName );

			data.NodeContentType = FlowGraphNodeContentType.Flow;
		}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem, ref bool sleep )
		{
			var variable = Variable.Value;
			if( variable != null && valueProperty != null )
			{
				var value = ReferenceUtility.GetUnreferencedValue( valueProperty.GetValue( this, null ) );
				flow.SetVariable( variable.GetVariableName(), value );
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

			var valueType = Variable.Value?.Type.Value;

			if( !valuePropertyNeedUpdate && valueType != valuePropertyCreatedWithType )
				valuePropertyNeedUpdate = true;

			if( !valuePropertyNeedUpdate )
				return;

			//do update

			Clear();
			if( valueType == null )
				return;

			valuePropertyNeedUpdate = false;
			valuePropertyCreatedWithType = valueType;

			Type unrefNetType = valueType.GetNetType();
			var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
			var type = MetadataManager.GetTypeOfNetType( refNetType );

			valueProperty = new ValueProperty( this, "Value", false, type, valueType, new Metadata.Parameter[ 0 ], false );
			valueProperty.Serializable = SerializeType.Enable;
		}

		void Clear()
		{
			valueProperty = null;
			valuePropertyCreatedWithType = null;
		}
	}
}
