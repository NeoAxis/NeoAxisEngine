// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Attribute to specify the resource file name extension.
	/// </summary>
	public class ResourceFileExtensionAttribute : Attribute
	{
		string extension;

		//

		public ResourceFileExtensionAttribute( string extension )
		{
			this.extension = extension;
		}

		public string Extension
		{
			get { return extension; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////!!!!name
	//[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
	//public class AutoCreateInstanceAttribute : Attribute
	//{
	//	public AutoCreateInstanceAttribute()
	//	{
	//	}
	//}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Attribute tagging method or constructor as a function of autoconversion types.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method | AttributeTargets.Constructor )]
	public class AutoConvertTypeAttribute : Attribute
	{
		public AutoConvertTypeAttribute()
		{
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies the type to configure a reference to a resource.
	/// </summary>
	public class ReferenceValueType_Resource
	{
		string resourceName;

		public ReferenceValueType_Resource()
		{
		}

		public ReferenceValueType_Resource( string resourceName )
		{
			this.resourceName = resourceName;
		}

		public string ResourceName
		{
			get { return resourceName; }
			set { this.resourceName = value; }
		}

		//public static implicit operator string( ReferenceValueType_Resource v )
		//{
		//	return v.resourceName;
		//}

		public override bool Equals( object obj )
		{
			return ( obj is ReferenceValueType_Resource && this == (ReferenceValueType_Resource)obj );
		}

		//public static bool operator ==( ReferenceValueType_Resource v1, ReferenceValueType_Resource v2 )
		//{
		//	return v1.value == v2.value;
		//}

		//public static bool operator !=( ReferenceValueType_Resource v1, ReferenceValueType_Resource v2 )
		//{
		//	return v1.value != v2.value;
		//}

		bool EqualsImpl( ReferenceValueType_Resource a )
		{
			if( ReferenceEquals( this, a ) )
				return true;
			return resourceName == a.resourceName;
		}

		public static bool operator ==( ReferenceValueType_Resource a, ReferenceValueType_Resource b )
		{
			bool aNull = ReferenceEquals( a, null );
			bool bNull = ReferenceEquals( b, null );
			if( aNull || bNull )
				return aNull && bNull;
			return a.EqualsImpl( b );
		}

		public static bool operator !=( ReferenceValueType_Resource a, ReferenceValueType_Resource b )
		{
			bool aNull = ReferenceEquals( a, null );
			bool bNull = ReferenceEquals( b, null );
			if( aNull || bNull )
				return !( aNull && bNull );
			return !a.EqualsImpl( b );
		}

		public override int GetHashCode()
		{
			return resourceName.GetHashCode();
		}

		public override string ToString()
		{
			return ResourceName;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies the type to configure a reference to a type member.
	/// </summary>
	public class ReferenceValueType_Member
	{
		Metadata.Member member;
		object obj;

		//

		public ReferenceValueType_Member()
		{
		}

		public ReferenceValueType_Member( Metadata.Member member, object obj )
		{
			this.member = member;
			this.obj = obj;
		}

		public Metadata.Member Member
		{
			get { return member; }
			set { member = value; }
		}

		public object Object
		{
			get { return obj; }
			set { obj = value; }
		}

		public override bool Equals( object obj )
		{
			return ( obj is ReferenceValueType_Member && this == (ReferenceValueType_Member)obj );
		}

		//public static bool operator ==( ReferenceValueType_Member v1, ReferenceValueType_Member v2 )
		//{
		//	return ( v1.member == v2.member && v1.obj == v2.obj );
		//}

		//public static bool operator !=( ReferenceValueType_Member v1, ReferenceValueType_Member v2 )
		//{
		//	return ( v1.member != v2.member || v1.obj != v2.obj );
		//}

		bool EqualsImpl( ReferenceValueType_Member a )
		{
			if( ReferenceEquals( this, a ) )
				return true;
			return member == a.member && obj == a.obj;
		}

		public static bool operator ==( ReferenceValueType_Member a, ReferenceValueType_Member b )
		{
			bool aNull = ReferenceEquals( a, null );
			bool bNull = ReferenceEquals( b, null );
			if( aNull || bNull )
				return aNull && bNull;
			return a.EqualsImpl( b );
		}

		public static bool operator !=( ReferenceValueType_Member a, ReferenceValueType_Member b )
		{
			bool aNull = ReferenceEquals( a, null );
			bool bNull = ReferenceEquals( b, null );
			if( aNull || bNull )
				return !( aNull && bNull );
			return !a.EqualsImpl( b );
		}

		public override int GetHashCode()
		{
			if( member != null )
			{
				int hash = member.GetHashCode();
				if( obj != null )
					hash ^= obj.GetHashCode();
				return hash;
			}
			return base.GetHashCode();
		}

		public override string ToString()
		{
			if( Member != null )
				return Member.ToString();
			else
				return "(Null)";
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies the type to configure a reference to a type method.
	/// </summary>
	public class ReferenceValueType_Method : ReferenceValueType_Member
	{
		public ReferenceValueType_Method( Metadata.Method member, object obj )
			: base( member, obj )
		{
		}
	}

	/// <summary>
	/// Specifies the type to configure a reference to a type property.
	/// </summary>
	public class ReferenceValueType_Property : ReferenceValueType_Member
	{
		public ReferenceValueType_Property( Metadata.Property member, object obj )
			: base( member, obj )
		{
		}
	}

	/// <summary>
	/// Specifies the type to configure a reference to a type event.
	/// </summary>
	public class ReferenceValueType_Event : ReferenceValueType_Member
	{
		public ReferenceValueType_Event( Metadata.Event member, object obj )
			: base( member, obj )
		{
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies the default reference value.
	/// </summary>
	[AttributeUsage( AttributeTargets.All )]
	public class DefaultValueReferenceAttribute : Attribute
	{
		string referenceValue;

		public DefaultValueReferenceAttribute( string referenceValue )
		{
			this.referenceValue = referenceValue;
		}

		public string ReferenceValue
		{
			get { return referenceValue; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Enumerates possible ways to clone field and property values.
	/// </summary>
	public enum CloneType
	{
		Auto,
		Disable,
		Shallow,
		Deep,
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies how the field or property is cloned.
	/// </summary>
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class CloneableAttribute : Attribute
	{
		CloneType type;

		//

		public CloneableAttribute( CloneType type = CloneType.Deep )
		{
			this.type = type;
		}

		public CloneType Type
		{
			get { return type; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Enumerate possible ways to serialize field and property values.
	/// </summary>
	public enum SerializeType
	{
		Auto,
		Disable,
		Enable,
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies that a field or property supports serialization.
	/// </summary>
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class SerializeAttribute : Attribute
	{
		SerializeType type;
		string memberName;
		//SerializationModes supportedSerializationModes = SerializationModes.StillNotSimulated | SerializationModes.AlreadySimulated;

		//[Flags]
		//public enum SerializationModes
		//{
		//	StillNotSimulated = 1 << 0,
		//	AlreadySimulated = 1 << 1,
		//}

		public SerializeType Type
		{
			get { return type; }
		}

		/// <summary>
		/// Gets the property name.
		/// </summary>
		public string MemberName
		{
			get { return memberName; }
		}

		//public SerializationModes SupportedSerializationModes
		//{
		//	get { return supportedSerializationModes; }
		//}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		public SerializeAttribute( SerializeType type = SerializeType.Enable, string memberName = "" )
		{
			this.type = type;
			this.memberName = memberName;
		}

		public SerializeAttribute( bool enable, string memberName = "" )
		{
			this.type = enable ? SerializeType.Enable : SerializeType.Disable;
			this.memberName = memberName;
		}

		///// <summary>
		///// Initializes a new instance of the class.
		///// </summary>
		///// <param name="memberName">The property name.</param>
		///// <param name="supportedSerializationModes">The supported serialization modes. Only for entity instance serialization, not for entity types.</param>
		//public SerializeAttribute( string memberName, SerializationModes supportedSerializationModes )
		//{
		//	this.memberName = memberName;
		//	this.supportedSerializationModes = supportedSerializationModes;
		//}

		///// <summary>
		///// Initializes a new instance of the class.
		///// </summary>
		///// <param name="memberName">The property name.</param>
		//public SerializeAttribute( string memberName )
		//{
		//	this.memberName = memberName;
		//}

		///// <summary>
		///// Initializes a new instance of the class.
		///// </summary>
		///// <param name="supportedSerializationModes">The supported serialization modes. Only for entity instance serialization, not for entity types.</param>
		//public SerializeAttribute( SerializationModes supportedSerializationModes )
		//{
		//	this.supportedSerializationModes = supportedSerializationModes;
		//}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface IComponent_VisibleInHierarchy
	{
		[Browsable( false )]
		bool VisibleInHierarchy { get; }
	}

	public interface IComponent_CanBeSelectedInHierarchy
	{
		[Browsable( false )]
		bool CanBeSelectedInHierarchy { get; }
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An interface confirming that the type has a static method Parse(string text) and value can converted to string by means ToString().
	/// </summary>
	public interface ICanParseFromAndConvertToString
	{
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//public interface ISerialize
	//{
	//	bool Load( object obj, Metadata.Property property, Metadata.LoadContext context, TextBlock block, out string error );
	//	bool Save( object obj, Metadata.Property property, Metadata.SaveContext context, TextBlock block, ref bool skipSave, out string error );
	//}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An attribute to specify default name for new instances of the type.
	/// </summary>
	public class NewObjectDefaultNameAttribute : Attribute
	{
		string name;

		public NewObjectDefaultNameAttribute( string name )
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum AutoTrueFalse
	{
		Auto,
		True,
		False,
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum EDialogResult
	{
		None = 0,
		OK = 1,
		Cancel = 2,
		Abort = 3,
		Retry = 4,
		Ignore = 5,
		Yes = 6,
		No = 7
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum EMessageBoxButtons
	{
		OK = 0,
		OKCancel = 1,
		AbortRetryIgnore = 2,
		YesNoCancel = 3,
		YesNo = 4,
		RetryCancel = 5
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum ScreenLabelEnum
	{
		Auto,
		AlwaysDisplay,
		NeverDisplay,
	}

}
