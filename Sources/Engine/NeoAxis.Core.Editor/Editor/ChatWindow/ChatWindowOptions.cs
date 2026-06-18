// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Text;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents options for <see cref="ChatWindow"/>.
	/// </summary>
	public class ChatWindowOptions : Metadata.IMetadataProvider
	{
		public delegate void ConfigureDelegate( ChatWindowOptions sender );
		public static event ConfigureDelegate Configure;

		/////////////////////////////////////////

		ChatWindowOptionsForm owner = null;

		/////////////////////////////////////////

		[DefaultValue( "" )]
		[DisplayName( "NeoX Access Token" )]
		[Description( "Log in to the NeoX app to enable the Chat AI, or specify the access token." )]
		public string NeoXAccessToken { get; set; } = "";

		/////////////////////////////////////

		public class PropertyImpl : Metadata.Property
		{
			IList<Attribute> attributes;
			string category;
			object value;

			public delegate void ValueChangedDelegate( PropertyImpl sender );
			public event ValueChangedDelegate ValueChanged;

			//

			public PropertyImpl( ChatWindowOptions owner, string name, Metadata.TypeInfo type, IList<Attribute> attributes, string category, object value, bool readOnly = false )
				: base( owner, name, false, type, type, new Metadata.Parameter[ 0 ], readOnly )
			{
				this.attributes = attributes;
				this.category = category;
				this.value = value;
			}

			public IList<Attribute> Attributes
			{
				get { return attributes; }
				set { attributes = value; }
			}

			public string Category
			{
				get { return category; }
				set { category = value; }
			}

			public object Value
			{
				get { return value; }
				set { this.value = value; }
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				List<object> result = new List<object>();

				if( attributes != null )
				{
					foreach( var a in attributes )
					{
						if( attributeType.IsAssignableFrom( a.GetType() ) )
							result.Add( a );
					}
				}

				//Category
				if( attributeType.IsAssignableFrom( typeof( CategoryAttribute ) ) )
				{
					if( !string.IsNullOrEmpty( category ) )
						result.Add( new CategoryAttribute( category ) );
				}

				return result.ToArray();
			}

			public override object GetValue( object obj, object[] index )
			{
				return value;
			}

			public override void SetValue( object obj, object value, object[] index )
			{
				bool changed = !Equals( this.value, value );

				this.value = value;

				if( changed )
					ValueChanged?.Invoke( this );
			}
		}

		/////////////////////////////////////

		public ChatWindowOptions( ChatWindowOptionsForm owner )
		{
			this.owner = owner;

			Configure?.Invoke( this );
		}

		[Browsable( false )]
		public Metadata.TypeInfo BaseType
		{
			get { return MetadataManager.GetTypeOfNetType( GetType() ); }
		}

		protected virtual void MetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			//var p = member as Metadata.Property;
			//if( p != null )
			//{
			//	switch( p.Name )
			//	{
			//	case nameof( PanelMode_Resources ):
			//		if( owner.Mode != ContentBrowser.ModeEnum.Resources )
			//			skip = true;
			//		break;
			//	}
			//}
		}

		public virtual IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context = null )
		{
			foreach( var m in BaseType.MetadataGetMembers( context ) )
			{
				bool skip = false;
				if( context == null || context.Filter )
					MetadataGetMembersFilter( context, m, ref skip );
				if( !skip )
					yield return m;
			}
		}

		public virtual Metadata.Member MetadataGetMemberBySignature( string signature, Metadata.GetMembersContext context = null )
		{
			var result = BaseType.MetadataGetMemberBySignature( signature, context );
			if( result != null )
				return result;

			return null;
		}
	}
}