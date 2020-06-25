// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Converts the given object value to the specified type.
	/// </summary>
	public class Component_ConvertTo : Component
	{
		bool needUpdate = true;
		Metadata.TypeInfo initializedForType;
		List<PropertyImpl> properties = new List<PropertyImpl>();
		Dictionary<string, PropertyImpl> propertyBySignature = new Dictionary<string, PropertyImpl>();

		/// <summary>
		/// The object to convert.
		/// </summary>
		[DefaultValue( null )]
		public Reference<object> Source
		{
			get { if( _source.BeginGet() ) Source = _source.Get( this ); return _source.value; }
			set { if( _source.BeginSet( ref value ) ) { try { SourceChanged?.Invoke( this ); } finally { _source.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Source"/> property value changes.</summary>
		public event Action<Component_ConvertTo> SourceChanged;
		ReferenceField<object> _source = null;

		/// <summary>
		/// The type to convert the value parameter to.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Metadata.TypeInfo> DestinationType
		{
			get { if( _destinationType.BeginGet() ) DestinationType = _destinationType.Get( this ); return _destinationType.value; }
			set { if( _destinationType.BeginSet( ref value ) ) { try { DestinationTypeChanged?.Invoke( this ); } finally { _destinationType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DestinationType"/> property value changes.</summary>
		public event Action<Component_ConvertTo> DestinationTypeChanged;
		ReferenceField<Metadata.TypeInfo> _destinationType = new Reference<Metadata.TypeInfo>( null, "System.String" );

		/////////////////////////////////////////

		class PropertyImpl : Metadata.Property
		{
			Component_ConvertTo creator;
			string category;
			string displayName;

			////////////

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, Component_ConvertTo creator, string category, string displayName )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
				this.creator = creator;
				this.category = category;
				this.displayName = displayName;
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
				//always obj == Owner
				var c = (Component_ConvertTo)obj;

				object result = c.Source.Value;

				//!!!!Metadata.TypeInfo support

				//auto convert types
				if( result != null && !Type.GetNetType().IsAssignableFrom( result.GetType() ) )
				{
					var newValue = MetadataManager.AutoConvertValue( result, Type.GetNetType() );
					if( newValue == null )
						newValue = MetadataManager.AutoConvertValue( ReferenceUtility.GetUnreferencedValue( result ), Type.GetNetType() );
					result = newValue;
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
			}
		}

		/////////////////////////////////////////

		public Component_ConvertTo()
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

		void Update()
		{
			//check disabled
			if( !Enabled )
			{
				Clear();
				return;
			}

			var type = DestinationType.Value;

			//check for updates
			if( !needUpdate && type != initializedForType )
				needUpdate = true;

			//nothing to update
			if( !needUpdate )
				return;

			//do update

			Clear();
			initializedForType = type;
			needUpdate = false;

			if( type != null )
			{
				var p = new PropertyImpl( this, "Result", false, type, type, new Metadata.Parameter[ 0 ], true, this, "Convert To", "Result" );
				p.Description = "An object that represents the converted value.";

				properties.Add( p );
				propertyBySignature[ p.Signature ] = p;
			}
		}

		void Clear()
		{
			properties.Clear();
			propertyBySignature.Clear();
		}
	}
}
