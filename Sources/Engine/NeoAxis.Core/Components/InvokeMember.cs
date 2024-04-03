// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The component for invoking methods, properties and fields.
	/// </summary>
	public class InvokeMember : Component, IFlowGraphRepresentationData, IFlowExecutionComponent
	{
		bool needUpdate = true;

		//!!!!threading

		Metadata.Member memberObject;
		List<PropertyImpl> properties = new List<PropertyImpl>();
		Dictionary<string, PropertyImpl> propertyBySignature = new Dictionary<string, PropertyImpl>();
		Dictionary<string, object> propertyValues = new Dictionary<string, object>();

		PropertyImpl propertyThis;
		List<PropertyImpl> propertyMethodParameters;
		PropertyImpl propertyMethodReturnParameter;
		PropertyImpl propertyPropertyValue;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The member to invoke.
		/// </summary>
		[Serialize]
		//[SelectMember]
		[FlowGraphBrowsable( false )]
		public Reference<ReferenceValueType_Member> Member
		{
			get { if( _member.BeginGet() ) Member = _member.Get( this ); return _member.value; }
			set
			{
				if( _member.BeginSet( this, ref value ) )
				{
					try
					{
						MemberChanged?.Invoke( this );
						needUpdate = true;
					}
					finally { _member.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Member"/> property value changes.</summary>
		public event Action<InvokeMember> MemberChanged;
		ReferenceField<ReferenceValueType_Member> _member;

		/// <summary>
		/// Whether the member is support flow control.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[FlowGraphBrowsable( false )]
		public Reference<bool> FlowSupport
		{
			get { if( _flowSupport.BeginGet() ) FlowSupport = _flowSupport.Get( this ); return _flowSupport.value; }
			set
			{
				if( _flowSupport.BeginSet( this, ref value ) )
				{
					try
					{
						FlowSupportChanged?.Invoke( this );
						needUpdate = true;
					}
					finally { _flowSupport.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="FlowSupport"/> property value changes.</summary>
		public event Action<InvokeMember> FlowSupportChanged;
		ReferenceField<bool> _flowSupport;

		//!!!!
		////InvokeEachGet
		//ReferenceField<bool> _invokeEachGet;
		//[DefaultValue( false )]
		//[Serialize]
		//[FlowchartBrowsable( false )]
		//public Reference<bool> InvokeEachGet
		//{
		//	get
		//	{
		//		if( _invokeEachGet.BeginGet() )
		//			InvokeEachGet = _invokeEachGet.Get( this );
		//		return _invokeEachGet.value;
		//	}
		//	set
		//	{
		//		if( _invokeEachGet.BeginSet( this, ref value ) )
		//		{
		//			try { InvokeEachGetChanged?.Invoke( this ); }
		//			finally { _invokeEachGet.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<InvokeMember> InvokeEachGetChanged;

		/// <summary>
		/// The input of the node.
		/// </summary>
		public FlowInput Entry
		{
			get { return new FlowInput( this, nameof( Entry ) ); }
		}

		/// <summary>
		/// The exit of the node.
		/// </summary>
		[Serialize]
		public Reference<FlowInput> Exit
		{
			get { if( _exit.BeginGet() ) Exit = _exit.Get( this ); return _exit.value; }
			set { if( _exit.BeginSet( this, ref value ) ) { try { ExitChanged?.Invoke( this ); } finally { _exit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Exit"/> property value changes.</summary>
		public event Action<InvokeMember> ExitChanged;
		ReferenceField<FlowInput> _exit;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Entry ):
				case nameof( Exit ):
					if( !FlowSupport )
						skip = true;
					break;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/*public */
		class PropertyImpl : Metadata.Property
		{
			string category;
			string displayName;
			bool referenceSupport;
			Metadata.Parameter parameter;
			bool invoke;

			//

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, string category, string displayName, bool referenceSupport, Metadata.Parameter parameter, bool invoke )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
				this.category = category;
				this.displayName = displayName;
				this.referenceSupport = referenceSupport;
				this.parameter = parameter;
				this.invoke = invoke;
			}

			public string Category
			{
				get { return category; }
				set { category = value; }
			}

			public Metadata.Parameter Parameter
			{
				get { return parameter; }
				set { parameter = value; }
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

				//always obj == Owner
				InvokeMember c = (InvokeMember)obj;

				//!!!!так?
				if( invoke && !c.FlowSupport )
					c.Invoke();

				object value;
				if( c.propertyValues.TryGetValue( Signature, out value ) )
				{
					//update value for Reference type
					if( referenceSupport )
					{
						//convert from ReferenceNoValue
						if( value != null && value is ReferenceNoValue )
						{
							value = ReferenceUtility.MakeReference( ReferenceUtility.GetUnreferencedType( Type.GetNetType() ), ( (ReferenceNoValue)value ).GetByReference );
						}

						IReference iReference = value as IReference;
						if( iReference != null )
						{
							//!!!!try catch? где еще так

							//get value by reference
							//!!!!
							value = iReference.GetValue( obj );
							//value = iReference.GetValue( Owner );

							//!!!!?
							if( !ReadOnly )//!!!!new
							{
								//!!!!
								SetValue( obj, value, Indexers );
								//SetValue( Owner, value, Indexers );
							}
						}
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
				InvokeMember c = (InvokeMember)obj;

				if( value != null )
					c.propertyValues[ Signature ] = value;
				else
					c.propertyValues.Remove( Signature );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Browsable( false )]
		public Metadata.Member MemberObject
		{
			get { return memberObject; }
		}

		//static Metadata.Member GetMemberByName( string name )
		//{
		//	if( !string.IsNullOrEmpty( name ) )
		//	{
		//		int splitIndex = name.IndexOf( '|' );
		//		if( splitIndex != -1 )
		//		{
		//			string typeName = name.Substring( 0, splitIndex );
		//			string sign = name.Substring( splitIndex + 1 );
		//			if( !string.IsNullOrEmpty( typeName ) && !string.IsNullOrEmpty( sign ) )
		//			{
		//				var type = MetadataManager.GetType( typeName );
		//				if( type != null )
		//				{
		//					var member = type.MetadataGetMemberBySignature( sign );
		//					if( member != null )
		//						return member;
		//				}
		//			}
		//		}
		//	}
		//	return null;
		//}

		Metadata.Member GetNeededMember()
		{
			var memberValue = Member.Value;
			if( memberValue != null )
				return memberValue.Member;
			return null;
		}

		//protected virtual Metadata.TypeInfo GetOverrideParameterType( Metadata.Parameter parameter, out bool error )
		//{
		//	error = false;
		//	return parameter.Type;
		//}

		public string GetThisPropertyName()
		{
			return "__this_This";
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
			var newMemberObject = GetNeededMember();

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
				//bool unableToInit = false;

				//method
				var method = memberObject as Metadata.Method;
				if( method != null )
				{
					//This
					if( !method.Static && !method.Constructor )
					{
						Metadata.TypeInfo creatorType = method.Owner as Metadata.TypeInfo;
						if( creatorType == null )
							creatorType = MetadataManager.MetadataGetType( method.Owner );

						Type unrefNetType = creatorType.GetNetType();
						var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
						var type = MetadataManager.GetTypeOfNetType( refNetType );

						//string namePrefix = "__this_";
						var p = new PropertyImpl( this, GetThisPropertyName()/* namePrefix + "This"*/, false, type, creatorType, new Metadata.Parameter[ 0 ], false, "Members", "This", true, null, false );
						p.Description = "";
						p.Serializable = SerializeType.Enable;

						properties.Add( p );
						propertyBySignature[ p.Signature ] = p;
						propertyThis = p;
					}

					//parameters
					propertyMethodParameters = new List<PropertyImpl>();
					for( int nParameter = 0; nParameter < method.Parameters.Length; nParameter++ )
					{
						var parameter = method.Parameters[ nParameter ];

						////!!!!имя еще как-то фиксить?
						//string name = null;
						//!!!!
						//if( parameter.ReturnValue )
						//	name = "ReturnValue";
						//if( name == null )
						var name = parameter.Name.Substring( 0, 1 ).ToUpper() + parameter.Name.Substring( 1 );

						//!!!!поддержать ByReference. еще какие?
						bool readOnly = parameter.Output || parameter.ReturnValue;

						//!!!!
						//var parameterType = GetOverrideParameterType( parameter, out bool error );
						//if( error )
						//{
						//	unableToInit = true;
						//	goto unableToInitLabel;
						//}
						//var parameterType = parameter.Type;

						Metadata.TypeInfo type;
						bool referenceSupport = !readOnly;
						if( referenceSupport )
						{
							Type unrefNetType = parameter.Type.GetNetType();
							var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
							type = MetadataManager.GetTypeOfNetType( refNetType );
						}
						else
							type = parameter.Type;

						bool invoke = parameter.ReturnValue || parameter.ByReference || parameter.Output;

						string namePrefix = "__parameter_";
						string displayName = TypeUtility.DisplayNameAddSpaces( name );

						var p = new PropertyImpl( this, namePrefix + name, false, type, parameter.Type, new Metadata.Parameter[ 0 ], readOnly, "Members", displayName, referenceSupport, parameter, invoke );
						p.Description = "";
						if( !readOnly )
							p.Serializable = SerializeType.Enable;

						properties.Add( p );
						propertyBySignature[ p.Signature ] = p;

						//!!!!так? если несколько?
						if( parameter.ReturnValue )
							propertyMethodReturnParameter = p;
						else
							propertyMethodParameters.Add( p );
					}

					//Constructor output
					if( method.Constructor )
					{
						Metadata.TypeInfo creatorType = method.Owner as Metadata.TypeInfo;
						if( creatorType == null )
							creatorType = MetadataManager.MetadataGetType( method.Owner );

						//"Output"
						string namePrefix = "__returnvalue_";
						var p = new PropertyImpl( this, namePrefix + "ReturnValue", false, creatorType, creatorType, new Metadata.Parameter[ 0 ], true, "Members", "Return Value", false, null, true );
						p.Description = "";
						//p.Serializable = true;

						properties.Add( p );
						propertyBySignature[ p.Signature ] = p;
						propertyMethodReturnParameter = p;
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

						Type unrefNetType = creatorType.GetNetType();
						var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
						var type = MetadataManager.GetTypeOfNetType( refNetType );

						//string namePrefix = "__this_";
						var p = new PropertyImpl( this, GetThisPropertyName()/* namePrefix + "This"*/, false, type, creatorType, new Metadata.Parameter[ 0 ], false, "Members", "This", true, null, false );
						p.Description = "";
						p.Serializable = SerializeType.Enable;

						properties.Add( p );
						propertyBySignature[ p.Signature ] = p;
						propertyThis = p;
					}

					//value
					{
						bool setMode = FlowSupport && !property.ReadOnly;

						Metadata.TypeInfo type;
						if( setMode )
						{
							Type unrefNetType = property.TypeUnreferenced.GetNetType();
							var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
							type = MetadataManager.GetTypeOfNetType( refNetType );
						}
						else
							type = property.TypeUnreferenced;

						string namePrefix = "__value_";
						var p = new PropertyImpl( this, namePrefix + "Value", false, type, property.TypeUnreferenced, new Metadata.Parameter[ 0 ], !setMode, "Members", "Value", setMode, null, true );
						p.Description = "";
						if( setMode )
							p.Serializable = SerializeType.Enable;

						properties.Add( p );
						propertyBySignature[ p.Signature ] = p;
						propertyPropertyValue = p;
					}

					//!!!!Indexers

				}

				//!!!!другие

				//unableToInitLabel:
				//if( unableToInit )
				//{
				//	Clear();
				//	needUpdate = true;
				//}
			}
		}

		void Clear()
		{
			memberObject = null;
			properties.Clear();
			propertyBySignature.Clear();
			propertyThis = null;
			propertyMethodParameters = null;
			propertyMethodReturnParameter = null;
			propertyPropertyValue = null;
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

		void Invoke()
		{
			//!!!!

			//!!!!try catch

			if( MemberObject != null )
			{
				//method
				{
					var method = MemberObject as Metadata.Method;
					if( method != null )
					{
						object obj = null;
						if( !method.Static && propertyThis != null )
						{
							obj = ReferenceUtility.GetUnreferencedValue( propertyThis.GetValue( this, null ) );

							//if( obj == null && !( method.Owner is Metadata.TypeInfo ) )
							//{
							//	obj = method.Owner;
							//	//Metadata.TypeInfo creatorType = method.Creator as Metadata.TypeInfo;
							//	//if( creatorType == null )
							//	//	creatorType = MetadataManager.MetadataGetType( method.Creator );
							//}
						}

						var methodInputParameters = method.GetInputParameters();

						object[] parameters = new object[ propertyMethodParameters.Count ];
						for( int n = 0; n < parameters.Length; n++ )
						{
							var value = propertyMethodParameters[ n ].GetValue( this, null );

							if( methodInputParameters.Length == parameters.Length )
							{
								var expectedType = methodInputParameters[ n ].Type.GetNetType();

								//!!!!еще случай когда приведение к базовому типу (типы - ссылки)
								if( value != null && value.GetType() != expectedType )
								{
									value = ReferenceUtility.GetUnreferencedValue( value );

									//auto convert types
									if( value != null && !expectedType.IsAssignableFrom( value.GetType() ) )
									{
										var newValue = MetadataManager.AutoConvertValue( value, expectedType );
										if( newValue == null )
											newValue = MetadataManager.AutoConvertValue( value, expectedType );
										value = newValue;
									}
								}
							}
							else
							{
								//must never happen
								value = ReferenceUtility.GetUnreferencedValue( value );
							}


							//var value = ReferenceUtility.GetUnreferencedValue( propertyMethodParameters[ n ].GetValue( this, null ) );


							//var value = propertyMethodParameters[ n ].GetValue( this, null );

							//if( methodInputParameters.Length == parameters.Length )
							//{
							//	var expectedType = methodInputParameters[ n ].Type.GetNetType();

							//	//auto convert types
							//	if( value != null && !expectedType.IsAssignableFrom( value.GetType() ) )
							//	{
							//		var newValue = MetadataManager.AutoConvertValue( value, expectedType );
							//		if( newValue == null )
							//			newValue = MetadataManager.AutoConvertValue( ReferenceUtility.GetUnreferencedValue( value ), expectedType );
							//		value = newValue;
							//	}
							//	//default for value types
							//	if( value == null && expectedType.IsValueType )
							//		value = Activator.CreateInstance( expectedType );
							//}
							//else
							//{
							//	//must never happen
							//	value = ReferenceUtility.GetUnreferencedValue( value );
							//}


							//подвисает только в дебаггере.
							////!!!!чтобы не подвисало. даже с try, catch. на int.Parse
							//{
							//	var type = propertyMethodParameters[ n ].TypeUnreferenced;

							//	if( value != null && !type.IsAssignableFrom( MetadataManager.MetadataGetType( value ) ) )
							//		value = null;
							//	if( value == null && type.GetNetType().IsValueType )
							//		value = type.InvokeInstance( null );
							//}

							parameters[ n ] = value;
						}

						if( method.Static || method.Constructor || obj != null )
						{
							//!!!!пока так
							try
							{

								var result = method.Invoke( obj, parameters );

								//ref, out
								for( int n = 0; n < parameters.Length; n++ )
								{
									var p = propertyMethodParameters[ n ];
									if( ( p.Parameter.ByReference || p.Parameter.Output ) && !p.Parameter.ReturnValue )
										p.SetValue( this, parameters[ n ], null );
								}

								//return
								propertyMethodReturnParameter?.SetValue( this, result, null );
								//if( propertyMethodReturnParameter != null )
								//{
								//	var key = propertyMethodReturnParameter.GetKey();
								//	if( result != null )
								//		propertyValues[ key ] = result;
								//	else
								//		propertyValues.Remove( key );
								//}

							}
							catch
							{
								//!!!!
							}
						}
					}
				}

				//property
				{
					var property = MemberObject as Metadata.Property;
					if( property != null )
					{
						object obj = null;
						if( !property.Static && propertyThis != null )
							obj = ReferenceUtility.GetUnreferencedValue( propertyThis.GetValue( this, null ) );

						if( property.Static || obj != null )
						{
							bool setMode = FlowSupport && !property.ReadOnly;
							if( setMode )
							{
								//set

								//!!!!indexers
								object[] indexers = new object[ 0 ];
								//object[] parameters = new object[ propertyParameters.Length ];
								//for( int n = 0; n < parameters.Length; n++ )
								//	parameters[ n ] = propertyParameters[ n ].GetValue( this );

								if( propertyPropertyValue != null )
								{
									var value = propertyPropertyValue.GetValue( this, null );

									var expectedType = property.Type.GetNetType();

									//!!!!еще случай когда приведение к базовому типу (типы - ссылки)
									if( value != null && value.GetType() != expectedType )
									{
										value = ReferenceUtility.GetUnreferencedValue( value );

										//auto convert types
										if( value != null && !expectedType.IsAssignableFrom( value.GetType() ) )
										{
											var newValue = MetadataManager.AutoConvertValue( value, expectedType );
											if( newValue == null )
												newValue = MetadataManager.AutoConvertValue( value, expectedType );
											value = newValue;
										}
									}

									//var value = ReferenceUtility.GetUnreferencedValue( propertyPropertyValue.GetValue( this, null ) );

									//var expectedType = property.Type.GetNetType();

									//var value = propertyPropertyValue.GetValue( this, null );

									//var expectedType = property.Type.GetNetType();

									////auto convert types
									//if( value != null && !expectedType.IsAssignableFrom( value.GetType() ) )
									//{
									//	var newValue = MetadataManager.AutoConvertValue( value, expectedType );
									//	if( newValue == null )
									//		newValue = MetadataManager.AutoConvertValue( ReferenceUtility.GetUnreferencedValue( value ), expectedType );
									//	value = newValue;
									//}

									//default for value types
									if( value == null && expectedType.IsValueType )
										value = Activator.CreateInstance( expectedType );

									property.SetValue( obj, value, indexers );
								}
							}
							else
							{
								//get

								//!!!!indexers
								object[] indexers = new object[ 0 ];
								//object[] parameters = new object[ propertyParameters.Length ];
								//for( int n = 0; n < parameters.Length; n++ )
								//	parameters[ n ] = propertyParameters[ n ].GetValue( this );

								var result = property.GetValue( obj, indexers );
								propertyPropertyValue?.SetValue( this, result, null );
								//propertyValues[ propertyPropertyValue.GetKey() ] = result;
							}
						}
					}
				}
			}
			else
			{
				//!!!!
			}
		}

		public virtual void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			if( MemberObject != null )
			{
				data.NodeTitle = MemberObject.ToString();

				var property = MemberObject as Metadata.Property;
				if( property != null )
				{
					bool setMode = FlowSupport && !property.ReadOnly;
					if( setMode )
						data.NodeTitle += " (set)";
					else
						data.NodeTitle += " (get)";
				}
			}

			if( FlowSupport )
				data.NodeContentType = FlowGraphNodeContentType.Flow;
		}

		//[Browsable( false )]
		//public string FlowchartNodeTitle
		//{
		//	get
		//	{
		//		//!!!!может по другому

		//		if( MemberObject != null )
		//			return MemberObject.ToString();
		//		else
		//			return null;
		//	}
		//}

		//[Browsable( false )]
		//public Texture FlowchartNodeRenderTexture
		//{
		//	get { return null; }
		//}

		//[Browsable( false )]
		//public FlowchartNodeContentType FlowchartNodeContentType
		//{
		//	get
		//	{
		//		if( FlowSupport )
		//			return FlowchartNodeContentType.Flow;
		//		else
		//			return FlowchartNodeContentType.Default;
		//	}
		//}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem, ref bool sleep )
		{
			Invoke();

			FlowInput next = Exit;
			if( next != null )
				flow.ExecutionStack.Push( new Flow.ExecutionStackItem( next ) );
		}

		//!!!!new. need this method to get ability set value before Enabled
		public void SetPropertyValue( string signature, object value )
		{
			if( value != null )
				propertyValues[ signature ] = value;
			else
				propertyValues.Remove( signature );
		}
	}
}
