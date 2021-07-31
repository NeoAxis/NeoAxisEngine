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
using System.Collections.Concurrent;

namespace NeoAxis
{
	/// <summary>
	/// The component for adding a virtual property to the parent component.
	/// </summary>
	//[EditorNewObjectSettings( typeof( NewObjectSettingsProperty ) )]//!!!!impl
	public class Component_Property : Component_Member
	{
		static ConcurrentDictionary<string, object> virtualMemberValuesStatic = new ConcurrentDictionary<string, object>();

		PropertyImpl createdProperty;
		Metadata.Event createdChangedEvent;

		bool defaultValuePropertyNeedUpdate = true;
		DefaultValueProperty defaultValueProperty;
		object defaultValuePropertyValue;

		/////////////////////////////////////////

		/// <summary>
		/// The type of the property.
		/// </summary>
		[Serialize]
		public Reference<Metadata.TypeInfo> Type
		{
			get { if( _type.BeginGet() ) Type = _type.Get( this ); return _type.value; }
			set
			{
				if( _type.BeginSet( ref value ) )
				{
					try
					{
						TypeChanged?.Invoke( this );
						if( Enabled )
							NeedUpdateCreatedMembers();
						defaultValuePropertyNeedUpdate = true;
					}
					finally { _type.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Type"/> property value changes.</summary>
		public event Action<Component_Property> TypeChanged;
		ReferenceField<Metadata.TypeInfo> _type = new Reference<Metadata.TypeInfo>( null, "System.String" );

		//ReferenceSupport
		ReferenceField<bool> _referenceSupport = true;
		/// <summary>
		/// Whether references are supported by the property.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		public Reference<bool> ReferenceSupport
		{
			get
			{
				if( _referenceSupport.BeginGet() )
					ReferenceSupport = _referenceSupport.Get( this );
				return _referenceSupport.value;
			}
			set
			{
				if( _referenceSupport.BeginSet( ref value ) )
				{
					try
					{
						ReferenceSupportChanged?.Invoke( this );
						if( Enabled )
							NeedUpdateCreatedMembers();
					}
					finally { _referenceSupport.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReferenceSupport"/> property value changes.</summary>
		public event Action<Component_Property> ReferenceSupportChanged;

		//!!!!кеш
		//[Serialize]
		//public List<Parameter> Indexers
		//{
		//	get { return indexers; }
		//}

		////!!!!или ReferenceList<TypeInfo>
		////Indexers
		//List<Component_MemberParameter> indexers = new List<Component_MemberParameter>();
		///// <summary>
		///// The indexers of the property.
		///// </summary>
		//[Serialize]
		//public virtual List<Component_MemberParameter> Indexers
		//{
		//	get { return indexers; }
		//}

		//xx xx;
		//public double GetInput( int index )
		//{
		//	//!!!!так?
		//	//!!!!slowly?
		//	if( !string.IsNullOrEmpty( inputs[ index ].GetByReference ) )
		//	{
		//		//!!!!если нет эвента, то можно не проверять

		//		var newValue = inputs[ index ].GetValue( this );
		//		if( newValue != inputs[ index ] )
		//		{
		//			inputs[ index ] = newValue;
		//			InputChanged?.Invoke( this, index );
		//		}
		//	}
		//	return inputs[ index ];
		//}

		//xx xx;
		//public delegate void InputChangedDelegate( Component_FloatingSum sender, int index );
		//public event InputChangedDelegate IndexerChanged;


		//public event Action<Component_Property> IndexersChanged;

		//Reference<List<Component_MemberParameter>> indexers = new List<Component_MemberParameter>();
		//[Serialize]
		//public virtual Reference<List<Component_MemberParameter>> Indexers
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( indexers.GetByReference ) )
		//			Indexers = indexers.GetValue( this );
		//		return indexers;
		//	}
		//	set
		//	{
		//		if( indexers == value ) return;
		//		indexers = value;
		//		ResetCache();
		//		IndexersChanged?.Invoke( this );
		//	}
		//}
		//public event Action<Component_Property> IndexersChanged;

		//ReadOnly
		ReferenceField<bool> _readOnly;
		/// <summary>
		/// Whether the property is read-only.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		public Reference<bool> ReadOnly
		{
			get
			{
				if( _readOnly.BeginGet() )
					ReadOnly = _readOnly.Get( this );
				return _readOnly.value;
			}
			set
			{
				if( _readOnly.BeginSet( ref value ) )
				{
					try
					{
						ReadOnlyChanged?.Invoke( this );
						if( Enabled )
							NeedUpdateCreatedMembers();
					}
					finally { _readOnly.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReadOnly"/> property value changes.</summary>
		public event Action<Component_Property> ReadOnlyChanged;

		//Browsable
		ReferenceField<bool> _browsable = true;
		/// <summary>
		/// Whether the property is browsable.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		public Reference<bool> Browsable
		{
			get
			{
				if( _browsable.BeginGet() )
					Browsable = _browsable.Get( this );
				return _browsable.value;
			}
			set
			{
				if( _browsable.BeginSet( ref value ) )
				{
					try
					{
						BrowsableChanged?.Invoke( this );
						if( createdProperty != null )
							createdProperty.Browsable = Browsable;
						else
							NeedUpdateCreatedMembers();
					}
					finally { _browsable.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Browsable"/> property value changes.</summary>
		public event Action<Component_Property> BrowsableChanged;

		//Category
		ReferenceField<string> _category = "Members";
		/// <summary>
		/// The category under which the property is displayed in the properties window.
		/// </summary>
		[Serialize]
		[DefaultValue( "Members" )]
		public Reference<string> Category
		{
			get
			{
				if( _category.BeginGet() )
					Category = _category.Get( this );
				return _category.value;
			}
			set
			{
				if( _category.BeginSet( ref value ) )
				{
					try
					{
						CategoryChanged?.Invoke( this );
						if( createdProperty != null )
							createdProperty.Category = Category;
						else
							NeedUpdateCreatedMembers();
					}
					finally { _category.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Category"/> property value changes.</summary>
		public event Action<Component_Property> CategoryChanged;

		/// <summary>
		/// Whether the property is serializable.
		/// </summary>
		[Serialize]
		[DefaultValue( SerializeType.Auto )]
		public Reference<SerializeType> Serializable
		{
			get { if( _serializable.BeginGet() ) Serializable = _serializable.Get( this ); return _serializable.value; }
			set
			{
				if( _serializable.BeginSet( ref value ) )
				{
					try
					{
						SerializableChanged?.Invoke( this );
						if( createdProperty != null )
							createdProperty.Serializable = Serializable;
						else
							NeedUpdateCreatedMembers();
					}
					finally { _serializable.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Serializable"/> property value changes.</summary>
		public event Action<Component_Property> SerializableChanged;
		ReferenceField<SerializeType> _serializable = SerializeType.Auto;

		//!!!!default value
		//Cloneable
		ReferenceField<CloneType> _cloneable = CloneType.Deep;
		/// <summary>
		/// The cloning mode of the property.
		/// </summary>
		[Serialize]
		[DefaultValue( CloneType.Deep )]
		public Reference<CloneType> Cloneable
		{
			get
			{
				if( _cloneable.BeginGet() )
					Cloneable = _cloneable.Get( this );
				return _cloneable.value;
			}
			set
			{
				if( _cloneable.BeginSet( ref value ) )
				{
					try
					{
						CloneableChanged?.Invoke( this );
						if( createdProperty != null )
							createdProperty.Cloneable = Cloneable;
						else
							NeedUpdateCreatedMembers();
					}
					finally { _cloneable.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Cloneable"/> property value changes.</summary>
		public event Action<Component_Property> CloneableChanged;

		////GetAccessor
		//ReferenceField<Component_MethodBody> _getAccessor;
		///// <summary>
		///// The get accessor method of the property.
		///// </summary>
		//[Serialize]
		//public Reference<Component_MethodBody> GetAccessor
		//{
		//	get
		//	{
		//		if( _getAccessor.BeginGet() )
		//			GetAccessor = _getAccessor.Get( this );
		//		return _getAccessor.value;
		//	}
		//	set
		//	{
		//		if( _getAccessor.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				GetAccessorChanged?.Invoke( this );
		//				//!!!!need?
		//				if( Enabled )
		//					NeedUpdateCreatedMembers();
		//			}
		//			finally { _getAccessor.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Property> GetAccessorChanged;

		////SetAccessor
		//ReferenceField<Component_MethodBody> _setAccessor;
		///// <summary>
		///// The set accessor method of the property.
		///// </summary>
		//[Serialize]
		//public Reference<Component_MethodBody> SetAccessor
		//{
		//	get
		//	{
		//		if( _setAccessor.BeginGet() )
		//			SetAccessor = _setAccessor.Get( this );
		//		return _setAccessor.value;
		//	}
		//	set
		//	{
		//		if( _setAccessor.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				SetAccessorChanged?.Invoke( this );
		//				//!!!!need?
		//				if( Enabled )
		//					NeedUpdateCreatedMembers();
		//			}
		//			finally { _setAccessor.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Property> SetAccessorChanged;

		//ChangedEvent
		ReferenceField<bool> _changedEvent = false;
		/// <summary>
		/// Whether the property invoke notification event if property value has changed.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		public Reference<bool> ChangedEvent
		{
			get
			{
				//!!!!
				//if( _changedEvent.BeginGet() )
				//	ChangedEvent = _changedEvent.Get( this );
				return _changedEvent.value;
			}
			//!!!!
			//set
			//{
			//	if( _changedEvent.BeginSet( ref value ) )
			//	{
			//		try
			//		{
			//			ChangedEventChanged?.Invoke( this );
			//			if( Enabled )
			//				NeedUpdateParentVirtualMembers();
			//		}
			//		finally { _changedEvent.EndSet(); }
			//	}
			//}
		}
		//!!!!
		//public event Action<Component_Property> ChangedEventChanged;

		//DefaultValueSpecified
		ReferenceField<bool> _defaultValueSpecified;
		/// <summary>
		/// Whether the property has a default value.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		public Reference<bool> DefaultValueSpecified
		{
			get
			{
				if( _defaultValueSpecified.BeginGet() )
					DefaultValueSpecified = _defaultValueSpecified.Get( this );
				return _defaultValueSpecified.value;
			}
			set
			{
				if( _defaultValueSpecified.BeginSet( ref value ) )
				{
					try
					{
						DefaultValueSpecifiedChanged?.Invoke( this );

						defaultValuePropertyNeedUpdate = true;
						if( createdProperty != null )
							UpdateDefaultValue( createdProperty );
						else
							NeedUpdateCreatedMembers();
					}
					finally { _defaultValueSpecified.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DefaultValueSpecified"/> property value changes.</summary>
		public event Action<Component_Property> DefaultValueSpecifiedChanged;

		////DefaultValue
		//ReferenceField<object> _defaultValue;
		//[Serialize]
		////[DefaultValue( "" )]
		//public Reference<object> DefaultValue
		//{
		//	get
		//	{
		//		if( _defaultValue.BeginGet() )
		//			DefaultValue = _defaultValue.Get( this );
		//		return _defaultValue.value;
		//	}
		//	set
		//	{
		//		if( _defaultValue.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				DefaultValueChanged?.Invoke( this );
		//				if( createdProperty != null )
		//					UpdateDefaultValue( createdProperty );
		//				else
		//					NeedUpdateParentVirtualMembers();
		//			}
		//			finally { _defaultValue.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Property> DefaultValueChanged;

		/////////////////////////////////////

		class PropertyImpl : Metadata.Property
		{
			Component_Property creator;
			string category;
			bool referenceSupport;

			//

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, Component_Property creator, string category, bool referenceSupport )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
				this.creator = creator;
				this.category = category;
				this.referenceSupport = referenceSupport;
			}

			public string Category
			{
				get { return category; }
				set { category = value; }
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				List<object> result = new List<object>();

				//!!!!может это в Metadata.Property? везде так
				//Category
				if( attributeType.IsAssignableFrom( typeof( CategoryAttribute ) ) )
				{
					if( !string.IsNullOrEmpty( category ) )
						result.Add( new CategoryAttribute( category ) );
				}

				return result.ToArray();
			}

			string GetKey()
			{
				//!!!!так?
				return Signature;
			}

			string GetVirtualMemberValuesStaticKey()
			{
				var resourceName = creator.ParentRoot.HierarchyController?.CreatedByResource?.Owner.Name;
				if( !string.IsNullOrEmpty( resourceName ) )
					return resourceName + "|" + ( (Component)Owner ).GetPathFromRoot() + "\\" + Name;
				return null;
			}

			public override object GetValue( object obj, object[] index )
			{
				object result = null;

				//!!!!
				//if( creator.GetAccessor.ReferenceSpecified )
				//{
				//	//with Get accessor

				//	var body = creator.GetAccessor.Value;
				//	if( body != null )
				//	{
				//		result = body.Invoke( obj, new object[ 0 ] );

				//		//!!!!
				//	}
				//}
				//else
				{
					//without Get accessor

					string key;
					IDictionary<string, object> dictionary;
					if( !Static )
					{
						Component c = (Component)obj;
						key = GetKey();
						dictionary = c.VirtualMemberValues;
					}
					else
					{
						key = GetVirtualMemberValuesStaticKey();
						dictionary = virtualMemberValuesStatic;
					}

					//Component c = (Component)obj;
					//if( c.VirtualMemberValues != null && c.VirtualMemberValues.TryGetValue( GetKey(), out object value ) )
					if( !string.IsNullOrEmpty( key ) && dictionary != null && dictionary.TryGetValue( key, out object value ) )
					{
						//update value for Reference type
						if( referenceSupport )
						{
							IReference iReference = value as IReference;
							if( iReference != null )
							{
								//get value by reference
								//!!!!new
								value = iReference.GetValue( obj );
								SetValue( obj, value, Indexers );
								//value = iReference.GetValue( Creator );
								//SetValue( Creator, value, Indexers );
							}
						}

						result = value;
					}

					//if no value then get default value
					if( result == null && DefaultValueSpecified && creator.defaultValueProperty != null )
						result = creator.defaultValueProperty.GetValue( creator, null );
				}

				//check the type. result can contains value with another type after change the type of property.
				//!!!!
				//var unrefType = TypeUnreferenced;
				if( result != null && !Type.IsAssignableFrom( MetadataManager.MetadataGetType( result ) ) )
					result = null;
				if( result == null && Type.GetNetType().IsValueType )
					result = Type.InvokeInstance( null );

				return result;
			}

			public override void SetValue( object obj, object value, object[] index )
			{
				//!!!!indexers

				//!!!!
				//if( creator.SetAccessor.ReferenceSpecified )
				//{
				//	//with Set accessor

				//	var body = creator.SetAccessor.Value;
				//	if( body != null )
				//		body.Invoke( obj, new object[] { value } );
				//}
				//else
				{
					//without Set accessor

					if( !Static )
					{
						Component c = (Component)obj;
						if( c.VirtualMemberValues == null )
							c.VirtualMemberValues = new Dictionary<string, object>();
						c.VirtualMemberValues[ GetKey() ] = value;
					}
					else
					{
						var key = GetVirtualMemberValuesStaticKey();
						if( !string.IsNullOrEmpty( key ) )
							virtualMemberValuesStatic[ key ] = value;
					}
				}
			}
		}

		/////////////////////////////////////

		class DefaultValueProperty : Metadata.Property
		{
			public DefaultValueProperty( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				List<object> result = new List<object>();

				//Category
				if( attributeType.IsAssignableFrom( typeof( CategoryAttribute ) ) )
					result.Add( new CategoryAttribute( "Property" ) );

				return result.ToArray();
			}

			public override object GetValue( object obj, object[] index )
			{
				object result = null;

				var c = (Component_Property)obj;
				var value = c.defaultValuePropertyValue;

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
				var c = (Component_Property)obj;

				var oldValue = c.defaultValuePropertyValue;
				c.defaultValuePropertyValue = value;

				if( !Equals( oldValue, value ) )
				{
					if( c.createdProperty != null )
						c.UpdateDefaultValue( c.createdProperty );
				}
			}
		}

		/////////////////////////////////////////

		//!!!!
		//public class NewObjectSettingsProperty : NewObjectSettings
		//{
		//	bool createFlowGraphWithGetSetAccessors = false;

		//	[DefaultValue( false )]
		//	[Category( "Options" )]
		//	public bool CreateFlowGraphWithGetSetAccessors
		//	{
		//		get { return createFlowGraphWithGetSetAccessors; }
		//		set { createFlowGraphWithGetSetAccessors = value; }
		//	}

		//	public override bool Creation( NewObjectCell.ObjectCreationContext context )
		//	{
		//		var property = (Component_Property)context.newObject;

		//		if( CreateFlowGraphWithGetSetAccessors )
		//		{
		//			var graph = property.CreateComponent<Component_FlowGraph>();
		//			graph.Name = "Flow Graph";

		//			Component_MethodBody bodyGet;
		//			{
		//				var node = graph.CreateComponent<Component_FlowGraphNode>();

		//				bodyGet = node.CreateComponent<Component_MethodBody>();
		//				bodyGet.Name = "Method Body Get";
		//				bodyGet.PropertyAccessorType = Component_MethodBody.PropertyAccessorTypeEnum.Get;

		//				node.Name = "Node " + bodyGet.Name;
		//				node.Position = new Vector2I( -25, -12 );
		//				node.ControlledObject = new Reference<Component>( null, ReferenceUtility.CalculateThisReference( node, bodyGet ) );
		//			}

		//			Component_MethodBodyEnd bodyEndGet;
		//			{
		//				var node = graph.CreateComponent<Component_FlowGraphNode>();

		//				bodyEndGet = node.CreateComponent<Component_MethodBodyEnd>();
		//				bodyEndGet.Name = "Method Body Get End";

		//				node.Name = "Node " + bodyEndGet.Name + " Get";
		//				node.Position = new Vector2I( 15, -12 );
		//				node.ControlledObject = new Reference<Component>( null, ReferenceUtility.CalculateThisReference( node, bodyEndGet ) );
		//			}

		//			Component_MethodBody bodySet;
		//			{
		//				var node = graph.CreateComponent<Component_FlowGraphNode>();

		//				bodySet = node.CreateComponent<Component_MethodBody>();
		//				bodySet.Name = "Method Body Set";
		//				bodySet.PropertyAccessorType = Component_MethodBody.PropertyAccessorTypeEnum.Set;

		//				node.Name = "Node " + bodySet.Name + " Set";
		//				node.Position = new Vector2I( -25, 8 );
		//				node.ControlledObject = new Reference<Component>( null, ReferenceUtility.CalculateThisReference( node, bodySet ) );
		//			}

		//			property.GetAccessor = new Reference<Component_MethodBody>( null, ReferenceUtility.CalculateThisReference( property, bodyGet ) );
		//			property.SetAccessor = new Reference<Component_MethodBody>( null, ReferenceUtility.CalculateThisReference( property, bodySet ) );

		//			bodyGet.Definition = new Reference<Component_Member>( null, ReferenceUtility.CalculateThisReference( bodyGet, property ) );
		//			bodyGet.BodyEnd = new Reference<Component_MethodBodyEnd>( null, ReferenceUtility.CalculateThisReference( bodyGet, bodyEndGet ) );
		//			bodyEndGet.Body = new Reference<Component_MethodBody>( null, ReferenceUtility.CalculateThisReference( bodyEndGet, bodyGet ) );

		//			bodySet.Definition = new Reference<Component_Member>( null, ReferenceUtility.CalculateThisReference( bodySet, property ) );

		//			//!!!!выделять, открывать созданные
		//		}

		//		return true;
		//	}
		//}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//!!!!slowly?
			//!!!!так?
			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case "DefaultValue":
					if( !DefaultValueSpecified.Value )
						skip = true;
					break;

				case "SetAccessor":
					if( ReadOnly )
						skip = true;
					break;
				}
			}
		}

		void UpdateDefaultValue( PropertyImpl property )
		{
			property.DefaultValueSpecified = DefaultValueSpecified.Value;
			if( property.DefaultValueSpecified && defaultValueProperty != null )
			{
				//!!!!need check type?

				property.DefaultValue = ReferenceUtility.GetUnreferencedValue( defaultValueProperty.GetValue( this, null ) );
			}
			else
				property.DefaultValue = null;
		}

		public override void NeedUpdateCreatedMembers()
		{
			base.NeedUpdateCreatedMembers();

			createdProperty = null;
			createdChangedEvent = null;
		}

		PropertyImpl CreateProperty()
		{
			//!!!!
			List<Component_MemberParameter> indexers = new List<Component_MemberParameter>();
			//List<Component_MemberParameter> indexers = Indexers;

			List<Metadata.Parameter> indexers2 = new List<Metadata.Parameter>( indexers.Count );

			//!!!!
			//foreach( var index in indexers )
			//{
			//	if( Cache == null )
			//		Log.Fatal( "Component_Property: CreateMetadataMember: Cache == null." );

			//	Metadata.Parameter p = index.CreateMetadataParameter( Cache );
			//	if( p == null )
			//	{
			//		//!!!!
			//		return null;
			//	}

			//	indexers2.Add( p );
			//}

			//!!!!!
			var unrefType = Type.Value;// MetadataManager.GetType( Type );
			if( unrefType == null )
			{
				//!!!!
				//Log.Warning( "The type with name \"{0}\" is not exists.", type );
				return null;
			}

			var resultType = unrefType;
			if( ReferenceSupport )
			{
				Type unrefNetType = unrefType.GetNetType();
				var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
				resultType = MetadataManager.GetTypeOfNetType( refNetType );
			}

			var property = new PropertyImpl( Parent, Name, Static, resultType, unrefType, indexers2.ToArray(), ReadOnly, this, Category, ReferenceSupport );
			property.Description = Description;
			property.Serializable = Serializable;
			property.Cloneable = Cloneable;
			property.Browsable = Browsable;

			UpdateDefaultValue( property );

			//!!!!ChangedEvent

			return property;
		}

		protected override void CreateMembers( List<Metadata.Member> created )
		{
			//!!!!ChangedEvent

			var property = CreateProperty();
			if( property != null )
			{
				createdProperty = property;
				created.Add( property );
			}
		}

		//public override void UpdateParentVirtualMembers( ref List<Metadata.Member> members, ref Dictionary<string, Metadata.Member> memberBySignature )
		//{
		//	base.UpdateParentVirtualMembers( ref members, ref memberBySignature );

		//	xx xx;
		//	//create
		//	if( createdProperty == null && Parent != null )
		//	{
		//		createdProperty = CreateProperty();

		//		//!!!!!changed event
		//	}

		//	xx xx;
		//	//add to parent
		//	if( createdProperty != null )
		//	{
		//		if( members == null )
		//		{
		//			members = new List<Metadata.Member>();
		//			memberBySignature = new Dictionary<string, Metadata.Member>();
		//		}
		//		members.Add( createdProperty );
		//		memberBySignature[ createdProperty.Signature ] = createdProperty;
		//		if( createdChangedEvent != null )
		//		{
		//			members.Add( createdChangedEvent );
		//			memberBySignature[ createdChangedEvent.Signature ] = createdChangedEvent;
		//		}
		//	}
		//}

		//!!!!cache?
		public string GetDisplayName()
		{
			StringBuilder b = new StringBuilder();

			//!!!!показывать короткие имена. везде так
			b.Append( Type );
			b.Append( ' ' );
			b.Append( Name );

			//!!!!
			List<Component_MemberParameter> indexers = new List<Component_MemberParameter>();
			//List<Component_MemberParameter> indexers = Indexers;
			if( indexers.Count != 0 )
			{
				b.Append( "[" );
				for( int n = 0; n < indexers.Count; n++ )
				{
					if( n != 0 )
						b.Append( ", " );
					var p = indexers[ n ];
					if( p.ByReference )
						b.Append( "ref " );
					else if( p.Output )
						b.Append( "out " );
					b.Append( p.Type );
					b.Append( ' ' );
					b.Append( Name );

					//!!!!default value
				}
				b.Append( "]" );
			}

			return b.ToString();
		}

		public override string ToString()
		{
			return GetDisplayName();
		}

		//public override void OnGetSettings( SettingsProvider provider, int objectIndex )
		//{
		//	base.OnGetSettings( provider, objectIndex );

		//	//!!!!
		//	//provider.AddCell( typeof( MembersSettingsCell_Property ), true );
		//}

		//public override Metadata.Member CreatedMember
		//{
		//	get { return createdProperty; }
		//}

		protected override IEnumerable<Metadata.Member> OnMetadataGetMembers()
		{
			foreach( var member in base.OnMetadataGetMembers() )
				yield return member;

			UpdateDefaultValueProperty();
			if( defaultValueProperty != null )
				yield return defaultValueProperty;
		}

		protected override Metadata.Member OnMetadataGetMemberBySignature( string signature )
		{
			UpdateDefaultValueProperty();
			if( defaultValueProperty != null && defaultValueProperty.Signature == signature )
				return defaultValueProperty;

			return base.OnMetadataGetMemberBySignature( signature );
		}

		void UpdateDefaultValueProperty()
		{
			if( !defaultValuePropertyNeedUpdate )
				return;

			//clear
			defaultValueProperty = null;

			if( !DefaultValueSpecified )
			{
				defaultValuePropertyNeedUpdate = false;
				return;
			}

			var unrefType = Type.Value;
			if( unrefType == null )
				return;

			defaultValuePropertyNeedUpdate = false;

			Type unrefNetType = unrefType.GetNetType();
			var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
			var type = MetadataManager.GetTypeOfNetType( refNetType );

			defaultValueProperty = new DefaultValueProperty( this, "DefaultValue", false, type, unrefType, new Metadata.Parameter[ 0 ], false );
			defaultValueProperty.Serializable = SerializeType.Enable;
		}

		//protected override void OnEnabledChanged()
		//{
		//	base.OnEnabledChanged();

		//	//!!!!need? где еще надо обновлять
		//	defaultValuePropertyNeedUpdate = true;
		//}

		//protected override void OnEnabledInHierarchyChanged()
		//{
		//	base.OnEnabledInHierarchyChanged();

		//	//!!!!need? где еще надо обновлять
		//	defaultValuePropertyNeedUpdate = true;
		//}
	}
}
