// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// A component to configure access to external parameter for material or screen effect. Its used in the visual graph editor.
	/// </summary>
	public class ShaderParameter : Component
	{
		PropertyImpl property;
		Metadata.TypeInfo createdPropertyReturnType;
		bool needUpdate = true;

		//!!!!

		[Serialize]
		public Reference<object> Source
		{
			get { if( _source.BeginGet() ) Source = _source.Get( this ); return _source.value; }
			set
			{
				if( _source.BeginSet( ref value ) )
				{
					try
					{
						SourceChanged?.Invoke( this );

						//!!!!так? всё время обновляется когда flowchart открыт
						needUpdate = true;
					}
					finally { _source.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Source"/> property value changes.</summary>
		public event Action<ShaderParameter> SourceChanged;
		ReferenceField<object> _source;

		/// <summary>
		/// Manually specified return type of parameter. It property must be specified when can't get access to a member in the editor. As example can't get access to objects of a current scene, because current scene is not available.
		/// </summary>
		[DefaultValue( null )]
		[FlowGraphBrowsable( false )]
		public Reference<Metadata.TypeInfo> ManuallySpecifiedReturnType
		{
			get { if( _manuallySpecifiedReturnType.BeginGet() ) ManuallySpecifiedReturnType = _manuallySpecifiedReturnType.Get( this ); return _manuallySpecifiedReturnType.value; }
			set
			{
				if( _manuallySpecifiedReturnType.BeginSet( ref value ) )
				{
					try
					{
						ManuallySpecifiedReturnTypeChanged?.Invoke( this );

						needUpdate = true;
					}
					finally { _manuallySpecifiedReturnType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ManuallySpecifiedReturnType"/> property value changes.</summary>
		public event Action<ShaderParameter> ManuallySpecifiedReturnTypeChanged;
		ReferenceField<Metadata.TypeInfo> _manuallySpecifiedReturnType = null;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		class PropertyImpl : Metadata.Property
		{
			string category;
			bool referenceSupport;

			//

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, string category, bool referenceSupport )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
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

				//!!!!может это в Metadata.Property?
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

			public override object GetValue( object obj, object[] index )
			{
				//!!!!какие-то проверки

				ShaderParameter c = (ShaderParameter)obj;

				//!!!!new
				object result = null;
				try
				{
					result = c.Source.Value;
				}
				catch { }
				//var result = c.Source.Value;

				//check the type. result can contains value with another type after change the type of property.
				if( result != null && !Type.IsAssignableFrom( MetadataManager.MetadataGetType( result ) ) )
					result = null;

				//!!!!default value

				if( result == null && Type.GetNetType().IsValueType )
					result = Type.InvokeInstance( null );

				return result;
			}

			public override void SetValue( object obj, object value, object[] index )
			{
				//ShaderParameter c = (ShaderParameter)obj;
				//if( c.propertyValues == null )
				//	c.propertyValues = new Dictionary<string, object>();

				////!!!!indexers

				//c.propertyValues[ GetKey() ] = value;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		Metadata.TypeInfo GetDemandedReturnType()
		{
			var manuallySpecifiedReturnType = ManuallySpecifiedReturnType.Value;
			if( manuallySpecifiedReturnType != null )
				return manuallySpecifiedReturnType;

			if( Source.ReferenceSpecified )
			{
				Source.GetMember( this, out object outObject, out Metadata.Member outMember );
				if( outMember != null )
				{
					var outProperty = outMember as Metadata.Property;
					if( outProperty != null )
						return outProperty.Type;
					else
					{
						var outMethod = outMember as Metadata.Method;
						if( outMethod != null )
						{
							var parameters = outMethod.GetReturnParameters();
							if( parameters.Length != 0 )
								return parameters[ 0 ].Type;
						}
					}
				}
			}
			return null;
		}

		void Update()
		{
			if( needUpdate )
			{
				//!!!!slowly?

				var demandedReturnType = GetDemandedReturnType();
				if( demandedReturnType != createdPropertyReturnType || property == null )
				{
					Clear();

					if( Enabled )//!!!!?
					{
						needUpdate = false;

						if( demandedReturnType != null )
						{
							//!!!!TypeInfo?
							Type unrefNetType = ReferenceUtility.GetUnreferencedType( demandedReturnType.GetNetType() );
							var type = MetadataManager.GetTypeOfNetType( unrefNetType );

							//!!!!
							var typeUnreferenced = type;

							var p = new PropertyImpl( this, "Output", false, type, typeUnreferenced, new Metadata.Parameter[ 0 ], true, "Parameter", false );
							p.Description = "";

							property = p;
							createdPropertyReturnType = demandedReturnType;
						}
					}
				}
			}
		}

		void Clear()
		{
			property = null;
			createdPropertyReturnType = null;
		}

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			//!!!!так?
			//!!!!где еще надо обновлять
			needUpdate = true;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			//if( !EnabledInHierarchy )
			//	Clear();

			//!!!!надо
			needUpdate = true;
		}

		protected override IEnumerable<Metadata.Member> OnMetadataGetMembers()
		{
			foreach( var member in base.OnMetadataGetMembers() )
				yield return member;

			Update();
			if( property != null )
				yield return property;
		}

		protected override Metadata.Member OnMetadataGetMemberBySignature( string signature )
		{
			Update();
			if( property != null && property.Signature == signature )
				return property;

			return base.OnMetadataGetMemberBySignature( signature );
		}

		[Browsable( false )]
		public Metadata.Property Property
		{
			get { return property; }
		}

		public object GetValue()
		{
			if( property != null )
				return property.GetValue( this, null );
			return null;
		}
	}
}
