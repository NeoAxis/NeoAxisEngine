// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

namespace NeoAxis
{
	/// <summary>
	/// The component for adding parameters to the virtual method.
	/// </summary>
	public class Component_MemberParameter : Component
	{
		//Type
		ReferenceField<Metadata.TypeInfo> _type = new Reference<Metadata.TypeInfo>( null, "System.String" );
		/// <summary>
		/// The type of the parameter.
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
						NeedUpdateParent();
					}
					finally { _type.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Type"/> property value changes.</summary>
		public event Action<Component_MemberParameter> TypeChanged;

		//ByReference
		ReferenceField<bool> _byReference;
		/// <summary>
		/// Whether the parameter explicitly passed by reference.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		public Reference<bool> ByReference
		{
			get
			{
				if( _byReference.BeginGet() )
					ByReference = _byReference.Get( this );
				return _byReference.value;
			}
			set
			{
				if( _byReference.BeginSet( ref value ) )
				{
					try
					{
						ByReferenceChanged?.Invoke( this );
						NeedUpdateParent();
					}
					finally { _byReference.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ByReference"/> property value changes.</summary>
		public event Action<Component_MemberParameter> ByReferenceChanged;

		//!!!!
		//!!!!name: Input. другие тоже
		////IsIn
		//Reference<bool> isIn;
		//[Serialize]
		//[DefaultValue( false )]
		//public virtual Reference<bool> In
		//{
		//	get
		//	{
		//		xx;reference old format
		//		if( !string.IsNullOrEmpty( isIn.GetByReference ) )
		//			In = isIn.GetValue( this );
		//		return isIn;
		//	}
		//	set
		//	{
		//		if( isIn == value ) return;
		//		isIn = value;
		//		ResetCache();
		//		InChanged?.Invoke( this );
		//	}
		//}
		//public event Action<Component_MemberParameter> InChanged;

		//Output
		ReferenceField<bool> _output;
		/// <summary>
		/// Whether the parameter is output.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		public Reference<bool> Output
		{
			get
			{
				if( _output.BeginGet() )
					Output = _output.Get( this );
				return _output.value;
			}
			set
			{
				if( _output.BeginSet( ref value ) )
				{
					try
					{
						OutputChanged?.Invoke( this );
						NeedUpdateParent();
					}
					finally { _output.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Output"/> property value changes.</summary>
		public event Action<Component_MemberParameter> OutputChanged;

		//ReturnValue
		ReferenceField<bool> _returnValue = false;
		/// <summary>
		/// Whether the parameter has a return value.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		public Reference<bool> ReturnValue
		{
			get
			{
				if( _returnValue.BeginGet() )
					ReturnValue = _returnValue.Get( this );
				return _returnValue.value;
			}
			set
			{
				if( _returnValue.BeginSet( ref value ) )
				{
					try
					{
						ReturnValueChanged?.Invoke( this );
						NeedUpdateParent();
					}
					finally { _returnValue.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReturnValue"/> property value changes.</summary>
		public event Action<Component_MemberParameter> ReturnValueChanged;

		//Optional
		ReferenceField<bool> _optional = false;
		/// <summary>
		/// Whether the parameter is optional.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		public Reference<bool> Optional
		{
			get
			{
				if( _optional.BeginGet() )
					Optional = _optional.Get( this );
				return _optional.value;
			}
			set
			{
				if( _optional.BeginSet( ref value ) )
				{
					try
					{
						OptionalChanged?.Invoke( this );
						NeedUpdateParent();
					}
					finally { _optional.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Optional"/> property value changes.</summary>
		public event Action<Component_MemberParameter> OptionalChanged;

		//!!!!старый формат ссылок
		//!!!!!object? где еще так
		//DefaultValue
		Reference<string> defaultValue;
		/// <summary>
		/// The default value of the parameter.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		public virtual Reference<string> DefaultValue
		{
			get
			{
				if( !string.IsNullOrEmpty( defaultValue.GetByReference ) )
					DefaultValue = defaultValue.GetValue( this );
				return defaultValue;
			}
			set
			{
				if( defaultValue == value ) return;
				defaultValue = value;
				NeedUpdateParent();
				DefaultValueChanged?.Invoke( this );
			}
		}
		/// <summary>Occurs when the <see cref="DefaultValue"/> property value changes.</summary>
		public event Action<Component_MemberParameter> DefaultValueChanged;



		//!!!!
		//Position

		//!!!!description?

		//!!!!было
		//Metadata.MemberCache cache;


		/////////////////////////////////////////

		//!!!!так? везде так
		public override string Name
		{
			get { return base.Name; }
			set
			{
				if( base.Name == value )
					return;
				base.Name = value;
				NeedUpdateParent();
			}
		}

		/////////////////////////////////////////

		//!!!!
		public virtual void NeedUpdateParent()
		{
			//!!!!не зависнет?

			var parent = Parent as Component_Member;
			if( parent != null )
				parent.NeedUpdateCreatedMembers();

			//!!!!было
			//cache?.Reset();
		}

		public virtual Metadata.Parameter CreateMetadataParameter()
		{
			var type = Type.Value;
			if( type == null )
			{
				//!!!!
				//Log.Warning( "The type with name \"{0}\" is not exists.", type );
				return null;
			}

			//!!!!
			bool defaultValueSpecified2 = false;
			object defaultValue2 = null;

			//!!!!!inIn

			var p = new Metadata.Parameter( Name, type, ByReference, false, Output, ReturnValue, Optional, defaultValueSpecified2, defaultValue2 );
			return p;
		}
	}
}
