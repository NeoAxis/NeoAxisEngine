// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Component representing the switch-statement.
	/// </summary>
	public class Component_Switch : Component, IFlowExecutionComponent, IFlowGraphRepresentationData
	{
		bool needUpdate = true;
		List<string> initializedForCases = new List<string>();
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
		/// The control variable of the switch statement.
		/// </summary>
		[DefaultValue( "" )]
		public Reference<string> Selection
		{
			get { if( _selection.BeginGet() ) Selection = _selection.Get( this ); return _selection.value; }
			set { if( _selection.BeginSet( ref value ) ) { try { SelectionChanged?.Invoke( this ); } finally { _selection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Selection"/> property value changes.</summary>
		public event Action<Component_Switch> SelectionChanged;
		ReferenceField<string> _selection = "";

		/// <summary>
		/// The list of switch cases.
		/// </summary>
		[FlowGraphBrowsable( false )]
		public List<string> Cases { get; set; } = new List<string>();

		/////////////////////////////////////////

		class PropertyImpl : Metadata.Property
		{
			Component_Switch creator;
			string category;
			string displayName;
			internal int parameterIndex;

			////////////

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, Component_Switch creator, string category, string displayName, int parameterIndex )
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
				var c = (Component_Switch)obj;

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
				var c = (Component_Switch)obj;

				if( value != null )
					c.propertyValues[ Signature ] = value;
				else
					c.propertyValues.Remove( Signature );
			}
		}

		/////////////////////////////////////////

		public Component_Switch()
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
			if( !needUpdate && !Cases.SequenceEqual( initializedForCases ) )
				needUpdate = true;

			//nothing to update
			if( !needUpdate )
				return;

			//do update

			Clear();
			initializedForCases = new List<string>( Cases );
			needUpdate = false;

			//Cases
			for( int nCase = 0; nCase < Cases.Count; nCase++ )
			{
				var caseValue = Cases[ nCase ];

				var p = new PropertyImpl( this, "Case" + nCase.ToString(), false, MetadataManager.GetTypeOfNetType( typeof( Reference<FlowInput> ) ), MetadataManager.GetTypeOfNetType( typeof( FlowInput ) ), new Metadata.Parameter[ 0 ], false, this, "Cases", caseValue, nCase );
				p.Description = "";
				p.Serializable = SerializeType.Enable;

				properties.Add( p );
				propertyBySignature[ p.Signature ] = p;
			}

			//Default
			{
				var p = new PropertyImpl( this, "Default", false, MetadataManager.GetTypeOfNetType( typeof( Reference<FlowInput> ) ), MetadataManager.GetTypeOfNetType( typeof( FlowInput ) ), new Metadata.Parameter[ 0 ], false, this, "Cases", "Default", -1 );
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

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem )
		{
			var selection = Selection.Value;

			if( selection != null )
			{
				//!!!!always compare via strings?
				var selectionStr = selection.ToString();

				for( int nCase = 0; nCase < Cases.Count; nCase++ )
				{
					if( selectionStr == Cases[ nCase ] )
					{
						var p = GetProperty( nCase );
						if( p != null )
						{
							var v = (FlowInput)ReferenceUtility.GetUnreferencedValue( p.GetValue( this, null ) );
							if( v != null )
							{
								flow.ExecutionStack.Push( new Flow.ExecutionStackItem( v ) );
								return;
							}
						}
					}
				}
			}

			//Default
			{
				var p = GetProperty( -1 );
				if( p != null )
				{
					var v = (FlowInput)ReferenceUtility.GetUnreferencedValue( p.GetValue( this, null ) );
					if( v != null )
						flow.ExecutionStack.Push( new Flow.ExecutionStackItem( v ) );
				}
			}
		}
	}
}
