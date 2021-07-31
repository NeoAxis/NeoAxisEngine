// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// The component provides the ability to perform several branches in succession.
	/// </summary>
	public class Component_Sequence : Component, IFlowExecutionComponent, IFlowGraphRepresentationData
	{
		bool needUpdate = true;
		int initializedCount;
		List<PropertyImpl> properties = new List<PropertyImpl>();
		Dictionary<string, PropertyImpl> propertyBySignature = new Dictionary<string, PropertyImpl>();
		Dictionary<string, object> propertyValues = new Dictionary<string, object>();

		/////////////////////////////////////////

		/// <summary>
		/// The input of the node.
		/// </summary>
		public FlowInput Entry
		{
			get { return new FlowInput( this, nameof( Entry ) ); }
		}

		/// <summary>
		/// The number of sequences.
		/// </summary>
		[DefaultValue( 4 )]
		[FlowGraphBrowsable( false )]
		public int Count { get; set; } = 4;

		/////////////////////////////////////////

		class PropertyImpl : Metadata.Property
		{
			Component_Sequence creator;
			string category;
			string displayName;
			internal int parameterIndex;

			////////////

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, Component_Sequence creator, string category, string displayName, int parameterIndex )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
				this.creator = creator;
				this.category = category;
				this.displayName = displayName;
				this.parameterIndex = parameterIndex;
			}

			public string Category
			{
				get { return category; }
				set { category = value; }
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				List<object> result = new List<object>();

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

				//always obj == Owner
				var c = (Component_Sequence)obj;

				object value;
				if( c.propertyValues.TryGetValue( Signature, out value ) )
				{
					IReference iReference = value as IReference;
					if( iReference != null )
					{
						value = iReference.GetValue( obj );
						SetValue( obj, value, Indexers );
					}
					result = value;
				}

				//check the type. result can contains value with another type after change the type of property.
				if( result != null && !Type.IsAssignableFrom( MetadataManager.MetadataGetType( result ) ) )
					result = null;
				if( result == null && Type.GetNetType().IsValueType )
					result = Type.InvokeInstance( null );

				return result;
			}

			public override void SetValue( object obj, object value, object[] index )
			{
				var c = (Component_Sequence)obj;

				if( value != null )
					c.propertyValues[ Signature ] = value;
				else
					c.propertyValues.Remove( Signature );
			}
		}

		/////////////////////////////////////////

		public Component_Sequence()
		{
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

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.Flow;
		}

		void Update()
		{
			//check disabled
			if( !Enabled )
			{
				Clear();
				return;
			}

			//check for updates
			if( !needUpdate && initializedCount != Count )
				needUpdate = true;

			//nothing to update
			if( !needUpdate )
				return;

			//do update

			Clear();
			initializedCount = Count;
			needUpdate = false;

			//Cases
			for( int nBody = 0; nBody < Count; nBody++ )
			{
				var p = new PropertyImpl( this, "Body" + nBody.ToString(), false, MetadataManager.GetTypeOfNetType( typeof( Reference<FlowInput> ) ), MetadataManager.GetTypeOfNetType( typeof( FlowInput ) ), new Metadata.Parameter[ 0 ], false, this, "Bodies", "Body " + ( nBody + 1 ).ToString(), nBody );
				p.Description = "";
				p.Serializable = SerializeType.Enable;

				properties.Add( p );
				propertyBySignature[ p.Signature ] = p;
			}
		}

		void Clear()
		{
			properties.Clear();
			propertyBySignature.Clear();
		}

		PropertyImpl GetProperty( int index )
		{
			foreach( var p in properties )
			{
				if( p.parameterIndex == index )
					return p;
			}
			return null;
		}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem, ref bool sleep )
		{
			bool isEntry = entryItem.FlowInput != null && entryItem.FlowInput.PropertyName == nameof( Entry );

			int number = -1;
			if( !isEntry )
			{
				if( flow.InternalVariables.TryGetValue( this, out var value ) )
					number = (int)value;
			}

			number++;

			var p = GetProperty( number );
			if( p != null )
			{
				flow.InternalVariables[ this ] = number;

				//reply after Body
				flow.ExecutionStack.Push( new Flow.ExecutionStackItem( this ) );

				//go to Body
				var v = (FlowInput)ReferenceUtility.GetUnreferencedValue( p.GetValue( this, null ) );
				if( v != null )
				{
					flow.ExecutionStack.Push( new Flow.ExecutionStackItem( v.Owner ) );
					return;
				}
			}
			else
			{
				//end
				flow.InternalVariables.Remove( this );
			}
		}
	}
}
