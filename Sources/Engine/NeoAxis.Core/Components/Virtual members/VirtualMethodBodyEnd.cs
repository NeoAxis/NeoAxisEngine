// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	/// The ending of the body of the method in the flow graph.
	/// </summary>
	public class VirtualMethodBodyEnd : Component, IFlowGraphRepresentationData
	{
		bool needUpdate = true;

		Metadata.Member memberObject;
		internal List<PropertyImpl> properties = new List<PropertyImpl>();
		Dictionary<string, PropertyImpl> propertyBySignature = new Dictionary<string, PropertyImpl>();
		//!!!!by key is good?
		Dictionary<string, object> propertyValues = new Dictionary<string, object>();

		List<PropertyImpl> propertyMethodParameters;
		//PropertyImpl propertyMethodReturnParameter;
		PropertyImpl propertyPropertyValue;

		/////////////////////////////////////////

		//Body
		ReferenceField<VirtualMethodBody> _body;
		[Serialize]
		[FlowGraphBrowsable( false )]
		public Reference<VirtualMethodBody> Body
		{
			get
			{
				if( _body.BeginGet() )
					Body = _body.Get( this );
				return _body.value;
			}
			set
			{
				if( _body.BeginSet( ref value ) )
				{
					try
					{
						BodyChanged?.Invoke( this );

						//!!!!
					}
					finally { _body.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Body"/> property value changes.</summary>
		public event Action<VirtualMethodBodyEnd> BodyChanged;

		/////////////////////////////////////////

		internal class PropertyImpl : Metadata.Property
		{
			string category;
			string displayName;
			internal ParameterType parameterType;
			internal int invokeParameterIndex;

			//bool referenceSupport;
			//bool invoke;

			////////////

			public enum ParameterType
			{
				Parameter,
				ReturnValue,
			}

			////////////

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, string category, string displayName, ParameterType parameterType, int invokeParameterIndex )//, bool referenceSupport, bool invoke )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
				this.category = category;
				this.displayName = displayName;
				this.parameterType = parameterType;
				this.invokeParameterIndex = invokeParameterIndex;
				//this.referenceSupport = referenceSupport;
				//this.invoke = invoke;
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

				//always obj == Owner
				var c = (VirtualMethodBodyEnd)obj;

				object value;
				if( c.propertyValues.TryGetValue( Signature, out value ) )
				{
					//update value for Reference type
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
				var c = (VirtualMethodBodyEnd)obj;

				if( value != null )
					c.propertyValues[ Signature ] = value;
				else
					c.propertyValues.Remove( Signature );
			}
		}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//!!!!
			//var p = member as Metadata.Property;
			//if( p != null )
			//{
			//	switch( p.Name )
			//	{
			//	case "PropertyAccessorType":
			//		if( Owner.Value as VirtualProperty == null )
			//			skip = true;
			//		break;
			//	}
			//}
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
			//!!!!что с пересечениями имен

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
			var body = Body.Value;
			return body?.MemberObject;

			//var body = Body.Value;
			//var definition = body?.Definition.Value;
			//if( definition != null )
			//{
			//	var members = definition.CreatedMembers;
			//	if( members.Length != 0 )
			//		return members[ 0 ];
			//}
			//return null;
		}

		void Update()
		{
			//check disabled
			if( !Enabled )
			{
				Clear();
				return;
			}

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
				if( method != null )
				{
					//parameters
					propertyMethodParameters = new List<PropertyImpl>();
					int invokeParameterIndexCounter = 0;
					for( int nParameter = 0; nParameter < method.Parameters.Length; nParameter++ )
					{
						var parameter = method.Parameters[ nParameter ];

						if( parameter.ByReference || parameter.Output || parameter.ReturnValue )
						{
							//!!!!имя еще как-то фиксить?
							//string name = null;
							////!!!!так?
							//if( parameter.ReturnValue )
							//	name = "Return";
							//if( name == null )
							var name = parameter.Name.Substring( 0, 1 ).ToUpper() + parameter.Name.Substring( 1 );
							//name = parameter.Name;

							Type unrefNetType = parameter.Type.GetNetType();
							var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
							var type = MetadataManager.GetTypeOfNetType( refNetType );

							PropertyImpl.ParameterType parameterType;
							if( parameter.ReturnValue )
								parameterType = PropertyImpl.ParameterType.ReturnValue;
							else
								parameterType = PropertyImpl.ParameterType.Parameter;

							string namePrefix = "__parameter_";
							string displayName = TypeUtility.DisplayNameAddSpaces( name );

							var p = new PropertyImpl( this, namePrefix + name, false, type, parameter.Type, new Metadata.Parameter[ 0 ], false, "Parameters", displayName, parameterType, invokeParameterIndexCounter );
							p.Description = "";
							p.Serializable = SerializeType.Enable;

							properties.Add( p );
							propertyBySignature[ p.Signature ] = p;

							//if( parameter.ReturnValue )
							//	propertyMethodOutputParameter = p;
							//else
							propertyMethodParameters.Add( p );
						}

						if( !parameter.ReturnValue )
							invokeParameterIndexCounter++;
					}
				}

				//property
				var property = memberObject as Metadata.Property;
				if( property != null )
				{
					//value
					var body = Body.Value;
					if( body.PropertyAccessorType.Value == VirtualMethodBody.PropertyAccessorTypeEnum.Get )
					{
						Type unrefNetType = property.TypeUnreferenced.GetNetType();
						var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
						var type = MetadataManager.GetTypeOfNetType( refNetType );

						string namePrefix = "__value_";
						var p = new PropertyImpl( this, namePrefix + "ReturnValue", false, type, property.TypeUnreferenced, new Metadata.Parameter[ 0 ], false, "Members", "Return Value", PropertyImpl.ParameterType.ReturnValue, 0 );
						p.Description = "";
						p.Serializable = SerializeType.Enable;

						properties.Add( p );
						propertyBySignature[ p.Signature ] = p;
						propertyPropertyValue = p;
					}
				}

				//!!!!другие
			}
		}

		void Clear()
		{
			memberObject = null;
			properties.Clear();
			propertyBySignature.Clear();
			propertyMethodParameters = null;
			//propertyMethodReturnParameter = null;
			propertyPropertyValue = null;
		}

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.MethodBody;
		}
	}
}
