// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Specifies a read access interface to <see cref="Reference{T}"/>.
	/// </summary>
	public interface IReference
	{
		Type UnderlyingType
		{
			get;
		}

		object ValueAsObject
		{
			get;
		}

		string GetByReference
		{
			get;
		}

		bool ReferenceSpecified
		{
			get;
		}

		//!!!!по сути не должно быть. надо юзать ReferenceField<T>
		IReference GetValue( object owner );

		//!!!!new
		/// <summary>
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="outObject">outObject can be null for static members.</param>
		/// <param name="outMember"></param>
		void GetMember( object owner, out object outObject, out Metadata.Member outMember );

		//!!!!new
		bool Equals( IReference reference );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies a reference of the engine.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct Reference<T> : IReference  // where T : IComparable<T>
	{
		T value;
		string getByReference;

		//

		public Reference( T value, string getByReference = "" )
		{
			this.value = value;
			this.getByReference = getByReference;
		}

		[Serialize]
		[Browsable( false )]
		public T Value
		{
			get { return value; }

			//set { this.value = value; }
			//Set is need for serialization.
			//no public because can't use Set for properties because this is struct.
			internal set { this.value = value; }
		}

		[Serialize]
		[Browsable( false )]
		public string GetByReference
		{
			get { return getByReference; }

			//!!!!new: IReference
			set { getByReference = value; }
			//internal set { getByReference = value; }

			////set { getByReference = value; }
			////Set is need for serialization.
			////no public because can't use Set for properties because this is struct.
		}

		[Browsable( false )]
		public bool ReferenceSpecified
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return !string.IsNullOrEmpty( getByReference ); }
		}

		[Browsable( false )]
		public bool ReferenceOrValueSpecified
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return !string.IsNullOrEmpty( getByReference ) || value != null; }
		}

		[Browsable( false )]
		public object ValueAsObject
		{
			get { return value; }
			set { this.value = (T)value; }
		}

		[Browsable( false )]
		public Type UnderlyingType
		{
			get { return typeof( T ); }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Reference<T>( T value )
		{
			return new Reference<T>( value );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return obj is Reference<T> && Equals( (Reference<T>)obj );
			//return ( obj is Reference<T> && this == (Reference<T>)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( IReference reference )
		{
			return reference is Reference<T> && Equals( (Reference<T>)reference );
			//return ( reference is Reference<T> && this == (Reference<T>)reference );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			var getByRef = GetByReference;
			if( getByRef == null )
				getByRef = "";

			if( Value != null )
				return Value.GetHashCode() ^ getByRef.GetHashCode();
			else
				return getByRef.GetHashCode();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static bool IsEqual( ref T v1, ref T v2 )
		{
			if( v1 != null && v2 == null )
				return false;
			if( v1 == null && v2 != null )
				return false;
			if( v1 == null && v2 == null )
				return true;
			return v1.Equals( v2 );
		}

		//!!!!new
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static bool IsEqualGetByReference( string v1, string v2 )
		{
			if( string.IsNullOrEmpty( v1 ) && string.IsNullOrEmpty( v2 ) )
				return true;
			return v1 == v2;
		}

		//!!!!new
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Reference<T> reference )
		{
			return IsEqual( ref value, ref reference.value ) && IsEqualGetByReference( GetByReference, reference.GetByReference );
		}

		//public static bool operator ==( Reference<T> v1, Reference<T> v2 )
		//{
		//	return IsEqual( ref v1.value, ref v2.value ) && IsEqualGetByReference( v1.GetByReference, v2.GetByReference );
		//	////!!!!!так? еще как-то переопределять?
		//	//return IsEqual( ref v1.value, ref v2.value ) && v1.GetByReference == v2.GetByReference;
		//}

		//public static bool operator !=( Reference<T> v1, Reference<T> v2 )
		//{
		//	return !IsEqual( ref v1.value, ref v2.value ) || !IsEqualGetByReference( v1.GetByReference, v2.GetByReference );
		//	////!!!!!так? еще как-то переопределять?
		//	//return !IsEqual( ref v1.value, ref v2.value ) || v1.GetByReference != v2.GetByReference;
		//}

		public override string ToString()
		{
			//!!!!
			object v = Value as object;
			return string.Format( "{0}", v != null ? v : "null" );

			//			if( !string.IsNullOrEmpty( GetByReference ) )
			//				return string.Format( "Reference: {0}", GetByReference );
			//			else
			//			{
			//				object v = Value as object;
			//				return string.Format( "Value: {0}", v != null ? v : "null" );
			//			}

			//if( !string.IsNullOrEmpty( GetByReference ) )
			//	return string.Format( "Reference: \"{0}\"", GetByReference );
			//else
			//{
			//	object v = Value as object;
			//	return string.Format( "Value: \"{0}\"", v != null ? v : "null" );
			//}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator T( Reference<T> r )
		{
			return r.Value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Reference<T> GetValue( object owner )
		{
			if( !string.IsNullOrEmpty( getByReference ) )
			{
				var value = MetadataManager.GetValueByReference( typeof( T ), owner, getByReference );
				return new Reference<T>( (T)value, getByReference );
			}
			return this;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		IReference IReference.GetValue( object owner )
		{
			if( !string.IsNullOrEmpty( getByReference ) )
			{
				var value = MetadataManager.GetValueByReference( typeof( T ), owner, getByReference );
				return new Reference<T>( (T)value, getByReference );
			}
			return this;
		}

		//!!!!ValueTuple<object, Metadata.Property> 
		//!!!!new. так оставить?
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetMember( object owner, out object outObject, out Metadata.Member outMember )
		{
			if( !string.IsNullOrEmpty( getByReference ) )
			{
				MetadataManager.GetMemberByReference( typeof( T ), owner, getByReference, out outObject, out outMember );
			}
			else
			{
				outObject = null;
				outMember = null;
			}
		}

		//!!!!
		//!!!!по сути это частность только для путей. может сделать FilePathReference
		//internal string GetFullPath( string path )
		//{
		//	xx xx;//таких методов не будет? они внутри struct Reference?

		//	//!!!!!проверять CreatedFromMapObjectType? везде так сделать
		//	if( Parent != null && Parent.Type != null )
		//		return VirtualPathUtils.ConvertRelativePathToFull( Path.GetDirectoryName( Parent.Type.Name ), path );
		//	else
		//		return path;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Reference<T>( ReferenceNoValue r )
		{
			return new Reference<T>( default, r.GetByReference );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies a structure for creating a field and property with <see cref="Reference{T}"/> support.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct ReferenceField<T>
	{
		public Reference<T> value;
		bool getLock;
		bool setLock;
		//UpdateLock getLock;
		//UpdateLock setLock;

		//

		//struct UpdateLock
		//{
		//	bool locked;

		//	public bool Enter()
		//	{
		//		if( locked )
		//			return false;
		//		locked = true;
		//		return true;
		//	}

		//	public void Exit()
		//	{
		//		locked = false;
		//	}
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator ReferenceField<T>( T initValue )
		{
			var result = new ReferenceField<T>();
			result.value = initValue;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator ReferenceField<T>( Reference<T> initValue )
		{
			var result = new ReferenceField<T>();
			result.value = initValue;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		bool GetFastExitOptimized()
		{
			var component = value.Value as Component;
			if( component != null )
			{
				var cachedReference = component.GetCachedResourceReference();
				if( cachedReference != null )
				{
					if( ReferenceEquals( value.GetByReference, cachedReference ) )
						return true;
					if( value.GetByReference == cachedReference )
					{
						value.GetByReference = cachedReference;
						return true;
					}
				}
			}

			return false;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool BeginGet()
		{
			if( !value.ReferenceSpecified )
				return false;
			if( getLock )
				return false;
			if( GetFastExitOptimized() )
				return false;
			getLock = true;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal bool BeginGet_WithoutFastExitOptimization()
		{
			if( !value.ReferenceSpecified )
				return false;
			if( getLock )
				return false;
			//if( GetFastExitOptimized() )
			//	return false;
			getLock = true;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Reference<T> Get( object owner )
		{
			//не может быть исключения наружу.

			try
			{
				return value.GetValue( owner );
			}
			catch
			{
				//!!!!как-то обрабатывать?

				return value;
			}
			finally
			{
				getLock = false;
				//getLock.Exit();
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool BeginSet( ref Reference<T> newValue )
		{
			if( value.Equals( newValue ) )
				return false;
			//!!!!здесь проверять лок? если выше, то останется старое значение
			if( setLock )
				return false;

			value = newValue;

			////!!!!здесь проверять лок? если выше, то останется старое значение
			//if( setLock )
			//	return false;
			setLock = true;
			return true;
			//return setLock.Enter();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void EndSet()
		{
			setLock = false;
			//setLock.Exit();
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies a structure of <see cref="Reference{T}"/> value without real value. The structure is used to transfer the value of the reference without specifying the value type.
	/// </summary>
	public struct ReferenceNoValue
	{
		string getByReference;

		//

		public ReferenceNoValue( string getByReference )
		{
			this.getByReference = getByReference;
		}

		public string GetByReference
		{
			get { return getByReference; }
		}

		public bool ReferenceSpecified
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return !string.IsNullOrEmpty( getByReference ); }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is ReferenceNoValue && this == (ReferenceNoValue)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ReferenceNoValue reference )
		{
			return this == reference;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			var getByRef = GetByReference;
			if( getByRef == null )
				getByRef = "";
			return getByRef.GetHashCode();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( ReferenceNoValue v1, ReferenceNoValue v2 )
		{
			return v1.GetByReference == v2.GetByReference;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( ReferenceNoValue v1, ReferenceNoValue v2 )
		{
			return v1.GetByReference != v2.GetByReference;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public override string ToString()
		{
			if( getByReference != null )
				return getByReference;
			else
				return "(null)";
		}

		//public Reference<T> To<T>()
		//{
		//	return new Reference<T>( default, getByReference );
		//}
	}
}
