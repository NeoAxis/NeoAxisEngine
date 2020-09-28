// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using System.Linq.Expressions;

namespace NeoAxis
{
	/// <summary>
	/// Specifies the engine metadata.
	/// </summary>
	public static partial class Metadata
	{
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		//public enum MemberType
		//{
		//	Method,
		//	Event,
		//	Property,

		//	//!!!!
		//	NestedType,

		//	//!!!!!name: Unknown
		//	Special,
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// An interface to provide engine's metadata.
		/// </summary>
		public interface IMetadataProvider
		{
			TypeInfo BaseType
			{
				get;
			}
			IEnumerable<Member> MetadataGetMembers( GetMembersContext context = null );
			Member MetadataGetMemberBySignature( string signature, GetMembersContext context = null );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Obtains information about the attributes of a member and provides access to member metadata.
		/// </summary>
		public abstract class Member
		{
			object owner;
			string name;
			string description = "";

			//!!!!для NestedType как тут
			bool isStatic;

			List<Attribute> additionalAttributes = new List<Attribute>();

			//!!!!public/private/protected

			//

			protected Member( object owner, string name, bool isStatic )
			{
				this.owner = owner;
				this.name = name;
				this.isStatic = isStatic;
			}

			public object Owner
			{
				get { return owner; }
			}

			public string Name
			{
				get { return name; }
			}

			public string Description
			{
				get { return description; }
				set { description = value; }
			}

			public bool Static
			{
				get { return isStatic; }
			}

			public abstract string Signature
			{
				get;
			}

			public List<Attribute> AdditionalAttributes
			{
				get { return additionalAttributes; }
			}

			protected abstract object[] OnGetCustomAttributes( Type attributeType, bool inherit );

			public object[] GetCustomAttributes( Type attributeType, bool inherit = true )
			{
				if( AdditionalAttributes.Count != 0 )
				{
					List<object> result = new List<object>( OnGetCustomAttributes( attributeType, inherit ) );
					foreach( var attr in AdditionalAttributes )
					{
						if( attributeType.IsAssignableFrom( attr.GetType() ) )
							result.Add( attr );
					}
					return result.ToArray();
				}
				else
					return OnGetCustomAttributes( attributeType, inherit );
			}

			public IEnumerable<T> GetCustomAttributes<T>( bool inherit = true ) where T : Attribute
			{
				foreach( T attr in OnGetCustomAttributes( typeof( T ), inherit ) )
					yield return attr;

				foreach( T attr in AdditionalAttributes.OfType<T>() )
					yield return attr;
			}

			public T GetCustomAttribute<T>( bool inherit = true ) where T : Attribute
			{
				return GetCustomAttributes<T>( inherit ).FirstOrDefault();
			}

			//public abstract MemberType MemberType
			//{
			//	get;
			//}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Keeps the attributes of a parameter and provides access to parameter metadata.
		/// </summary>
		public class Parameter
		{
			string name;
			//!!!!!!relative paths problem: может быть по ссылке относительно овнера. поэтому не можем сразу определить.
			TypeInfo type;

			bool isByReference;
			bool isIn;
			bool isOutput;
			//!!!!!надо ли? по сути это только для красоты синтаксиса
			bool isReturnValue;//у .net называется "bool IsRetval"

			bool isOptional;

			bool defaultValueSpecified;
			object defaultValue;
			//!!!!
			//Position

			//

			public Parameter( string name, TypeInfo type, bool isByReference, bool isIn, bool isOutput, bool isReturnValue, bool isOptional, bool defaultValueSpecified, object defaultValue )
			{
				this.name = name;
				this.type = type;
				this.isByReference = isByReference;
				this.isIn = isIn;
				this.isOutput = isOutput;
				this.isReturnValue = isReturnValue;
				this.isOptional = isOptional;
				this.defaultValueSpecified = defaultValueSpecified;
				this.defaultValue = defaultValue;
			}

			public string Name
			{
				get { return name; }
			}

			public TypeInfo Type
			{
				get { return type; }
			}

			public bool ByReference
			{
				get { return isByReference; }
			}

			public bool In
			{
				get { return isIn; }
			}

			public bool Output
			{
				get { return isOutput; }
			}

			public bool ReturnValue
			{
				get { return isReturnValue; }
			}

			public bool Optional
			{
				get { return isOptional; }
			}

			public bool DefaultValueSpecified
			{
				get { return defaultValueSpecified; }
			}

			public object DefaultValue
			{
				get { return defaultValue; }
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Keeps the attributes of a method and provides access to method metadata.
		/// </summary>
		public abstract class Method : Member
		{
			Parameter[] parameters;
			string signature;
			string displayName;
			bool isConstructor;
			bool isOperator;

			Parameter[] inputParametersCached;
			Parameter[] returnParametersCached;

			//

			public Method( object owner, string name, bool isStatic, Parameter[] parameters, bool isConstructor, bool isOperator )
				: base( owner, name, isStatic )
			{
				this.parameters = parameters;
				this.isConstructor = isConstructor;
				this.isOperator = isOperator;
			}

			public Parameter[] Parameters
			{
				get { return parameters; }
			}

			public bool Constructor
			{
				get { return isConstructor; }
			}

			public bool Operator
			{
				get { return isOperator; }
			}

			public override string Signature
			{
				get
				{
					if( signature == null )
					{
						StringBuilder b = new StringBuilder();

						b.Append( "method:" );
						b.Append( Name );
						b.Append( '(' );
						bool paramsWasAdded = false;
						for( int n = 0; n < parameters.Length; n++ )
						{
							var p = parameters[ n ];
							if( !p.ReturnValue )
							{
								if( paramsWasAdded )
									b.Append( ',' );

								if( !p.ReturnValue )
								{
									if( p.Output )
										b.Append( "out " );
									else if( p.ByReference )
										b.Append( "ref " );
								}
								b.Append( p.Type.Name );

								//!!!!name?
								//!!!!default value

								paramsWasAdded = true;
							}
						}
						b.Append( ')' );
						signature = b.ToString();
					}
					return signature;
				}
			}

			//!!!!!temp
			//static string GetTypeName( Type type )
			//{
			//	//!!!!!кеш

			//	CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider( "C#" );
			//	CodeTypeReferenceExpression typeReferenceExpression = new CodeTypeReferenceExpression( new CodeTypeReference( type ) );
			//	using( StringWriter writer = new StringWriter() )
			//	{
			//		codeDomProvider.GenerateCodeFromExpression( typeReferenceExpression, writer, new CodeGeneratorOptions() );
			//		return writer.GetStringBuilder().ToString();
			//	}
			//}

			//!!!!!temp
			//public static string TypeToCSharpString( Type type )
			//{
			//	try
			//	{
			//		return GetTypeName( type );
			//	}
			//	catch
			//	{
			//		string text = type.FullName.Replace( "+", "." );
			//		return text;
			//	}
			//}

			public override string ToString()
			{
				if( displayName == null )
				{
					StringBuilder b = new StringBuilder();

					string name = Constructor ? "Constructor" : Name;
					if( isOperator && name.Length > 3 && name.Substring( 0, 3 ) == "op_" )
						name = name.Substring( 3 );
					b.Append( name );
					//b.Append( Constructor ? "Constructor" : Name );

					b.Append( '(' );
					bool paramsWasAdded = false;
					for( int n = 0; n < parameters.Length; n++ )
					{
						var p = parameters[ n ];
						if( !p.ReturnValue )
						{
							if( paramsWasAdded )
								b.Append( ", " );
							//b.Append( paramsWasAdded ? ", " : " " );

							if( p.Output )
								b.Append( "out " );
							else if( p.ByReference )
								b.Append( "ref " );
							b.Append( p.Type );
							b.Append( ' ' );
							b.Append( p.Name );

							//!!!!default value

							paramsWasAdded = true;
						}
					}
					//if( paramsWasAdded )
					//	b.Append( ' ' );
					b.Append( ')' );

					//return param
					{
						//!!!!может быть несколько

						b.Append( " : " );

						Parameter returnParam = null;
						foreach( var p in parameters )
						{
							if( p.ReturnValue )
							{
								returnParam = p;
								break;
							}
						}
						if( returnParam != null )
							b.Append( returnParam.Type );
						else
							b.Append( "void" );
					}

					displayName = b.ToString();
				}

				return displayName;
			}

			public delegate void InvokeEventDelegate( Method sender, object obj, object[] parameters, ref object returnValue );
			public event InvokeEventDelegate InvokeEvent;

			public virtual object Invoke( object obj, object[] parameters )
			{
				object ret = null;
				InvokeEvent?.Invoke( this, obj, parameters, ref ret );
				return ret;
			}

			//public override MemberType MemberType
			//{
			//	get { return MemberType.Method; }
			//}

			//!!!!new. need?
			public Parameter[] GetInputParameters()
			{
				if( inputParametersCached == null )
				{
					List<Parameter> result = new List<Parameter>();
					foreach( var p in Parameters )
					{
						if( !p.ReturnValue )
							result.Add( p );
					}
					inputParametersCached = result.ToArray();
				}
				return inputParametersCached;
			}

			//!!!!new. need?
			public Parameter[] GetReturnParameters()
			{
				if( returnParametersCached == null )
				{
					List<Parameter> result = new List<Parameter>();
					foreach( var p in Parameters )
					{
						if( p.ReturnValue )
							result.Add( p );
					}
					returnParametersCached = result.ToArray();
				}
				return returnParametersCached;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!!!сериализовывать внутри иерархии. за пределы иерархии какие ограничения
		/// <summary>
		/// Keeps the attributes of an event and provides access to event metadata.
		/// </summary>
		public abstract class Event : Member
		{
			TypeInfo eventHandlerType;

			////!!!!!
			//bool serializable;

			//!!!!
			//bool cloneable;

			string signature;
			string displayName;

			//

			public Event( object owner, string name, bool isStatic, TypeInfo eventHandlerType )//Parameter[] parameters )
				: base( owner, name, isStatic )
			{
				this.eventHandlerType = eventHandlerType;
				//this.parameters = parameters;
			}

			public TypeInfo EventHandlerType
			{
				get { return eventHandlerType; }
			}

			//public bool Serializable
			//{
			//	get { return serializable; }
			//	set { serializable = value; }
			//}

			public override string Signature
			{
				get
				{
					if( signature == null )
					{
						StringBuilder b = new StringBuilder();
						b.Append( "event:" );
						b.Append( Name );
						//b.Append( '(' );
						//for( int n = 0; n < parameters.Length; n++ )
						//{
						//	if( n != 0 )
						//		b.Append( ',' );
						//	var p = parameters[ n ];
						//if( p.Out )
						//	b.Append( "out " );
						//else if( p.ByReference )
						//	b.Append( "ref " );
						////	if( p.IsOut )
						////		b.Append( p.IsIn ? "ref " : "out " );
						//	b.Append( p.TypeName );

						//	//!!!!name?
						//	//!!!!default value
						//}
						//b.Append( ')' );
						signature = b.ToString();
					}
					return signature;
				}
			}

			public override string ToString()
			{
				if( displayName == null )
				{
					StringBuilder b = new StringBuilder();
					b.Append( Name );

					//!!!!
					//b.Append( " : " );

					//!!!!
					//b.Append( EventHandlerType );

					//b.Append( '(' );
					//bool paramsWasAdded = false;
					//for( int n = 0; n < parameters.Length; n++ )
					//{
					//	var p = parameters[ n ];
					//	if( !p.ReturnValue )
					//	{
					//		b.Append( paramsWasAdded ? ", " : " " );

					//		if( p.Output )
					//			b.Append( "out " );
					//		else if( p.ByReference )
					//			b.Append( "ref " );
					//		//if( p.Out )
					//		//	b.Append( p.In ? "ref " : "out " );

					//		b.Append( p.Type );
					//		b.Append( ' ' );
					//		b.Append( p.Name );

					//		//!!!!default value

					//		paramsWasAdded = true;
					//	}
					//}
					//if( paramsWasAdded )
					//	b.Append( ' ' );
					//b.Append( ')' );



					//b.Append( '(' );
					//if( parameters.Length != 0 )
					//{
					//	b.Append( ' ' );
					//	for( int n = 0; n < parameters.Length; n++ )
					//	{
					//		if( n != 0 )
					//			b.Append( ", " );
					//		var p = parameters[ n ];
					//		if( p.IsOut )
					//			b.Append( p.IsIn ? "ref " : "out " );
					//		b.Append( p.TypeName );
					//		b.Append( ' ' );
					//		b.Append( Name );

					//		//!!!!default value
					//	}
					//	b.Append( ' ' );
					//}
					//b.Append( ')' );
					displayName = b.ToString();
				}
				return displayName;
			}

			public delegate void InvokeEventDelegate( Event sender, object obj, object[] parameters );
			public event InvokeEventDelegate InvokeEvent;

			public virtual void Invoke( object obj, object[] parameters )
			{
				InvokeEvent?.Invoke( this, obj, parameters );
			}

			//!!!!
			public abstract void AddEventHandler( object target, System.Delegate handler );
			public abstract void RemoveEventHandler( object target, System.Delegate handler );
			//!!!!
			public abstract void AddEventHandler( object target, Delegate handler );
			public abstract void RemoveEventHandler( object target, Delegate handler );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public abstract class Field : Member
		//{
		//	TypeInfo type;

		//	bool serializable;
		//	bool browsable = true;
		//	//default value

		//	//

		//	public Field( object creator, string name, string description, bool isStatic, TypeInfo type )
		//		: base( creator, name, description, isStatic )
		//	{
		//		this.type = type;
		//	}

		//	public TypeInfo Type
		//	{
		//		get { return type; }
		//	}

		//	public override string Signature
		//	{
		//		get
		//		{
		//			return "impl";
		//		}
		//	}

		//	public bool Serializable
		//	{
		//		get { return serializable; }
		//		set { serializable = value; }
		//	}

		//	public bool Browsable
		//	{
		//		get { return browsable; }
		//		set { browsable = value; }
		//	}

		//	public override string ToString()
		//	{
		//		return "impl";
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Keeps the attributes of a property and provides access to property metadata.
		/// </summary>
		public abstract class Property : Member
		{
			TypeInfo type;
			TypeInfo typeUnreferenced;
			Parameter[] indexers;
			bool readOnly;

			bool browsable = true;
			CloneType cloneable = CloneType.Auto;
			SerializeType serializable = SerializeType.Auto;

			bool defaultValueSpecified;
			object defaultValue;

			//!!!!!
			bool changedEvent = true;

			//!!!!!!
			string signature;
			string displayName;

			//bool isField;

			//!!!!коллекции

			//!!!!!bool virtual;
			//!!!!public/private/protected

			/////////////////////////////////////

			public Property( object owner, string name, bool isStatic, TypeInfo type, TypeInfo typeUnreferenced, Parameter[] indexers, bool readOnly )
				: base( owner, name, isStatic )
			{
				this.type = type;
				this.typeUnreferenced = typeUnreferenced;
				this.indexers = indexers;
				this.readOnly = readOnly;
			}

			public TypeInfo Type
			{
				get { return type; }
			}

			/// <summary>
			/// This need to determine non .NET type when it wrapped by the reference.
			/// </summary>
			public TypeInfo TypeUnreferenced
			{
				get
				{
					//postpone initialization
					if( typeUnreferenced == null )
						typeUnreferenced = ReferenceUtility.GetUnreferencedType( type );

					return typeUnreferenced;
				}
			}

			public Parameter[] Indexers
			{
				get { return indexers; }
			}

			public bool ReadOnly
			{
				get { return readOnly; }
			}

			public override string Signature
			{
				get
				{
					if( signature == null )
					{
						//!!!!!!

						StringBuilder b = new StringBuilder();
						b.Append( "property:" );
						b.Append( Name );

						if( Indexers.Length != 0 )
						{
							b.Append( "[" );
							for( int n = 0; n < indexers.Length; n++ )
							{
								var index = indexers[ n ];
								if( n != 0 )
									b.Append( ',' );
								if( index.Output )
									b.Append( "out " );
								else if( index.ByReference )
									b.Append( "ref " );
								b.Append( index.Type.Name );
								//b.Append( ' ' );
								//b.Append( index.Name );

								//!!!!default value
							}
							b.Append( "]" );
						}

						signature = b.ToString();
					}
					return signature;
				}
			}

			public bool Browsable
			{
				get { return browsable; }
				set { browsable = value; }
			}

			public CloneType Cloneable
			{
				get { return cloneable; }
				set { cloneable = value; }
			}

			public SerializeType Serializable
			{
				get { return serializable; }
				set { serializable = value; }
			}

			public bool DefaultValueSpecified
			{
				get { return defaultValueSpecified; }
				set { defaultValueSpecified = value; }
			}

			public object DefaultValue
			{
				get { return defaultValue; }
				set { defaultValue = value; }
			}

			public override string ToString()
			{
				if( displayName == null )
				{
					StringBuilder b = new StringBuilder();
					b.Append( Name );

					if( Indexers.Length != 0 )
					{
						b.Append( "[" );
						//b.Append( "[ " );

						for( int n = 0; n < indexers.Length; n++ )
						{
							var index = indexers[ n ];
							if( n != 0 )
								b.Append( ", " );
							if( index.Output )
								b.Append( "out " );
							else if( index.ByReference )
								b.Append( "ref " );
							b.Append( index.Type );
							b.Append( ' ' );
							b.Append( index.Name );

							//!!!!default value
						}
						b.Append( "]" );
						//b.Append( " ]" );
					}

					b.Append( " : " );
					b.Append( Type );

					displayName = b.ToString();
				}
				return displayName;
			}

			//public override MemberType MemberType
			//{
			//	get { return MemberType.Property; }
			//}

			//!!!!name
			public delegate void GetValueImplEventDelegate( Property sender, object obj, object[] index, ref object returnValue );
			public event GetValueImplEventDelegate GetValueImplEvent;

			//!!!!name
			public delegate void SetValueImplEventDelegate( Property sender, object obj, object value, object[] index );
			public event SetValueImplEventDelegate SetValueImplEvent;

			//!!!!
			public virtual object GetValue( object obj, object[] index )
			{
				object ret = null;
				GetValueImplEvent?.Invoke( this, obj, index, ref ret );
				return ret;
			}

			//!!!!
			public virtual void SetValue( object obj, object value, object[] index )
			{
				SetValueImplEvent?.Invoke( this, obj, value, index );
				//AllProperties_PerformEventSetValueCalled( obj, this, index );
			}

			[Browsable( false )]
			public bool HasIndexers
			{
				get { return indexers != null && indexers.Length != 0; }
			}

			/////////////////////////////////////

			//!!!!remove

			////!!!!new
			////!!!!!тут ли. тут уже про объекты
			////!!!!!есть еще AllLists_PerformChildrenChanged

			////!!!!пересмотреть. надо ли. может slowly. т.к. узкое место
			//public delegate void AllProperties_SetValueCalledDelegate( object obj, Property property, object[] index );
			//public static event AllProperties_SetValueCalledDelegate AllProperties_SetValueCalled;

			//public static void AllProperties_PerformEventSetValueCalled( object obj, Property property, object[] index )
			//{
			//	AllProperties_SetValueCalled?.Invoke( obj, property, index );
			//}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!not used
		//public abstract class NestedTypeMember : Member
		//{
		//	//!!!!!!!
		//	TypeInfo type;

		//	//!!!!
		//	//bool provideAsTypeEnum;
		//	//EDictionary<string, long> provideAsTypeEnumValues;

		//	//!!!!
		//	public NestedTypeMember( object creator, string name, string description, bool isStatic )
		//		: base( creator, name, description, isStatic )
		//	{
		//		//!!!!
		//	}

		//	//public override MemberType MemberType
		//	//{
		//	//	get { return MemberType.NestedType; }
		//	//}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a delegate, which is a data structure that refers to a static method or to a class instance and an instance method of that class.
		/// </summary>
		public class Delegate// : ICloneable
		{
			Method method;
			object target;

			System.Delegate netDelegate;

			//

			public Method Method
			{
				get { return method; }
			}

			public object Target
			{
				get { return target; }
			}

			public System.Delegate NetDelegate
			{
				get { return netDelegate; }
			}

			//public static Delegate Combine( Delegate a, Delegate b )
			//{
			//}

			//public static Delegate Combine( params Delegate[] delegates )
			//{
			//}

			//public static Delegate Remove( Delegate source, Delegate value )
			//{
			//}

			//public static Delegate RemoveAll( Delegate source, Delegate value )
			//{
			//}

			//public virtual object Clone()
			//{
			//}

			public object Invoke( params object[] args )
			{
				return method.Invoke( target, args );
			}

			//public override bool Equals( object obj )
			//{
			//}

			//public override int GetHashCode()
			//{
			//}

			//public static bool operator ==( Delegate d1, Delegate d2 )
			//{
			//}

			//public static bool operator !=( Delegate d1, Delegate d2 )
			//{
			//}

			//public virtual Delegate[] GetInvocationList()
			//{
			//}

			//!!!!

			public static Delegate Create( TypeInfo type, object target, Method method )
			{
				var d = new Delegate();
				d.method = method;
				d.target = target;

				//!!!!не Net поддержать

				//!!!!пока так. Component_CSharpScript specific
				var scriptMethod = method as Component_CSharpScript.MethodImpl;
				if( scriptMethod != null )
				{
					var netMethod = scriptMethod.CompiledMethod;
					var script = (Component_CSharpScript)target;
					//!!!!
					if( script.ScriptInstance != null )
						d.netDelegate = System.Delegate.CreateDelegate( type.GetNetType(), script.ScriptInstance, netMethod );
					//d.netDelegate = System.Delegate.CreateDelegate( type.GetNetType(), script.CompiledScript.Instance, netMethod );
				}
				else
				{
					var netMethod = (MethodInfo)( (NetTypeInfo.NetMethod)method ).NetMember;
					d.netDelegate = System.Delegate.CreateDelegate( type.GetNetType(), target, netMethod );
				}

				//MethodInfo netMethod;

				////!!!!пока так. Component_CSharpScript specific
				//var scriptMethod = method as Component_CSharpScript.MethodImpl;
				//if( scriptMethod != null )
				//	netMethod = scriptMethod.CompiledMethod;
				//else
				//	netMethod = (MethodInfo)( (NetTypeInfo.NetMethod)method ).NetMember;

				//d.netDelegate = System.Delegate.CreateDelegate( type.GetNetType(), target, netMethod );

				//d.netDelegate = Delegate.CreateDelegate( netEvent.EventHandlerType.GetNetType(), target, netMethod );

				return d;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The list of supported types of engine's metadata.
		/// </summary>
		public enum TypeClassification
		{
			Component,
			//!!!!Structure?
			Class,
			Enumeration,
			//!!!!!
			Delegate,
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		///// <summary>
		///// Defines access control level to members and types of the engine's metadata.
		///// </summary>
		//public enum AccessLevel
		//{
		//	Public,
		//	Private,
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a base class for type declarations for class types, interface types, array types, value types, enumeration types, type parameters, generic type definitions, and open or closed constructed generic types.
		/// </summary>
		public abstract class TypeInfo : IMetadataProvider
		{
			string name;
			string displayName;
			TypeInfo baseType;
			TypeClassification classification;

			//!!!!!так?
			EDictionary<string, long> enumElements;
			bool enumFlags;

			//!!!!genericParams. на базе Component_Result<T> что-то создать в редакторе
			//!!!!generics - это типа как абстрактный класс. еще слоты

			////!!!!new
			//object autoCreatedInstance;

			List<Attribute> additionalAttributes = new List<Attribute>();

			//

			protected TypeInfo( string name, string displayName, TypeInfo baseType, TypeClassification classification,
				EDictionary<string, long> enumElements, bool enumFlags )
			{
				this.name = name;
				this.displayName = displayName;
				this.baseType = baseType;
				this.classification = classification;
				//!!!!клонировать?
				this.enumElements = enumElements;
				this.enumFlags = enumFlags;
			}

			public string Name
			{
				get { return name; }
			}

			public string DisplayName
			{
				get { return displayName; }
			}

			public TypeInfo BaseType
			{
				get { return baseType; }
			}

			public TypeClassification Classification
			{
				get { return classification; }
			}

			public EDictionary<string, long> EnumElements
			{
				get { return enumElements; }
			}

			public bool EnumFlags
			{
				get { return enumFlags; }
			}

			//public object AutoCreatedInstance
			//{
			//	get { return autoCreatedInstance; }
			//	set { autoCreatedInstance = value; }
			//}

			public List<Attribute> AdditionalAttributes
			{
				get { return additionalAttributes; }
			}

			//!!!!нужен ли типу контекст?
			public virtual IEnumerable<Member> MetadataGetMembers( GetMembersContext context = null )
			{
				if( BaseType != null )
				{
					foreach( var m in BaseType.MetadataGetMembers( context ) )
						yield return m;
				}
			}

			public virtual Member MetadataGetMemberBySignature( string signature, GetMembersContext context = null )
			{
				if( BaseType != null )
				{
					var m = BaseType.MetadataGetMemberBySignature( signature, context );
					if( m != null )
						return m;
				}
				return null;
			}

			//public virtual Member MetadataGetMemberByName( MemberType type, string name, bool includeBaseTypes )
			//{
			//	if( includeBaseTypes && BaseType != null )
			//	{
			//		var m = BaseType.MetadataGetMemberByName( type, name, true );
			//		if( m != null )
			//			return m;
			//	}
			//	return null;
			//}

			public override string ToString()
			{
				return DisplayName;
			}

			public abstract object InvokeInstance( object[] constructorParams );

			public virtual Type GetNetType()
			{
				//!!!!!slowly?
				return BaseType.GetNetType();
			}

			protected virtual string OnGetUserFriendlyNameForInstance()
			{
				if( Name.Contains( '\\' ) || Name.Contains( '/' ) )
				{
					try
					{
						//!!!!new
						return PathUtility.GetFileNameWithoutExtension( Name );
						//return Path.GetFileName( Name );
					}
					catch { }
				}

				return Name;
			}

			public string GetUserFriendlyNameForInstance( bool fixInvalidCharacters = false )
			{
				var name = OnGetUserFriendlyNameForInstance();

				if( fixInvalidCharacters )
				{
					char[] invalidCharacters = new char[] { '/', '\\', ':' };
					if( name.IndexOfAny( invalidCharacters ) != -1 )
					{
						foreach( var c in invalidCharacters )
							name = name.Replace( c, '_' );
					}
				}

				return name;
			}

			//internal abstract void InitAutoCreatedInstance();

			public bool IsAssignableFrom( TypeInfo type )
			{
				var t = type;
				do
				{
					if( this == t )
						return true;
					t = t.BaseType;
				} while( t != null );
				return false;
			}

			protected abstract object[] OnGetCustomAttributes( Type attributeType, bool inherit );

			public object[] GetCustomAttributes( Type attributeType, bool inherit = true )
			{
				if( AdditionalAttributes.Count != 0 )
				{
					List<object> result = new List<object>( OnGetCustomAttributes( attributeType, inherit ) );
					foreach( var attr in AdditionalAttributes )
					{
						if( attributeType.IsAssignableFrom( attr.GetType() ) )
							result.Add( attr );
					}
					return result.ToArray();
				}
				else
					return OnGetCustomAttributes( attributeType, inherit );
			}

			////!!!!new. везде сделать "TypeInfo attributeType"?
			//public abstract object[] GetCustomAttributes( TypeInfo attributeType, bool inherit );

			//!!!!переопдеделять не только defaultValue нужно. еще Serializable, Browsable. в C# это имплементируется как новое свойство
			//public virtual object GetPropertyDefaultValue( Metadata.Property property )
			//{
			//	if( BaseType != null )
			//		return BaseType.GetPropertyDefaultValue( property );
			//	return null;
			//}

			////!!!!new
			//Dictionary<Metadata.Property, object> propertyOverriddenDefaultValues;

			//{
			//	//!!!!threading. в переопределенных методах тож

			//	if( propertyOverriddenDefaultValues != null )
			//	{
			//		if( propertyOverriddenDefaultValues.TryGetValue( property, out object value ) )
			//			return value;
			//	}

			//	xx xx;
			//	return null;
			//}

			//Dictionary<Metadata.Property, object> propertyOverriddenDefaultValues;

			public abstract bool Abstract { get; }

			public abstract void GetPropertyDefaultValue( Property property, out bool specified, out object value );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents type declarations for .NET class types, interface types, array types, value types, enumeration types, type parameters, generic type definitions, and open or closed constructed generic types.
		/// </summary>
		public class NetTypeInfo : TypeInfo
		{
			Type type;
			List<Member> members = new List<Member>();
			Dictionary<string, Member> memberBySignature = new Dictionary<string, Member>();
			Dictionary<string, Member> memberByName = new Dictionary<string, Member>();
			//!!!!memory?
			//Dictionary<string, Member> memberBySignatureTotal = new Dictionary<string, Member>();

			string namespaceCached;

			////////////

			/// <summary>
			/// Keeps the attributes of a .NET method and provides access to method metadata.
			/// </summary>
			public class NetMethod : Method
			{
				MethodBase netMember;

				public NetMethod( object owner, string name, bool isStatic, Parameter[] parameters, bool isConstructor, bool isOperator, MethodBase netMember )
					: base( owner, name, isStatic, parameters, isConstructor, isOperator )
				{
					this.netMember = netMember;
				}

				public MethodBase NetMember
				{
					get { return netMember; }
				}

				public override object Invoke( object obj, object[] parameters )
				{
					if( netMember != null )
					{
						ConstructorInfo c = netMember as ConstructorInfo;
						if( c != null )
							return c.Invoke( parameters );
						else
							return netMember.Invoke( obj, parameters );
					}
					return null;
				}

				protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
				{
					return netMember.GetCustomAttributes( attributeType, inherit );
				}
			}

			////////////

			/// <summary>
			/// Keeps the attributes of a .NET property and provides access to property metadata.
			/// </summary>
			public class NetProperty : Property
			{
				MemberInfo netMember;

				public NetProperty( object owner, string name, bool isStatic, TypeInfo type, TypeInfo typeUnreferenced, Parameter[] indexers, bool readOnly, MemberInfo netMember )
					: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
				{
					this.netMember = netMember;
				}

				public MemberInfo NetMember
				{
					get { return netMember; }
				}

				public override object GetValue( object obj, object[] index )
				{
					var p = netMember as PropertyInfo;
					if( p != null )
					{
						return p.GetValue( obj, index );
					}
					else
					{
						var f = netMember as FieldInfo;
						if( f != null )
							return f.GetValue( obj );
						else
							return null;
					}
				}

				public override void SetValue( object obj, object value, object[] index )
				{
					var p = netMember as PropertyInfo;
					if( p != null )
					{
						p.SetValue( obj, value, index );
					}
					else
					{
						var f = netMember as FieldInfo;
						if( f != null )
							f.SetValue( obj, value );
					}

					//AllProperties_PerformEventSetValueCalled( obj, this, index );
				}

				protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
				{
					return netMember.GetCustomAttributes( attributeType, inherit );
				}
			}

			////////////

			/// <summary>
			/// Keeps the attributes of a .NET event and provides access to event metadata.
			/// </summary>
			public class NetEvent : Event
			{
				EventInfo netMember;

				public NetEvent( object owner, string name, bool isStatic, TypeInfo eventHandlerType, EventInfo netMember )
					: base( owner, name, isStatic, eventHandlerType )
				{
					this.netMember = netMember;
				}

				public EventInfo NetMember
				{
					get { return netMember; }
				}

				protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
				{
					return netMember.GetCustomAttributes( attributeType, inherit );
				}

				public override void Invoke( object obj, object[] parameters )
				{
					try
					{
						//!!!!check

						var eventDelegate = (MulticastDelegate)Owner.GetType().GetField( netMember.Name,
							BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public ).GetValue( obj );

						if( eventDelegate != null )
						{
							foreach( var handler in eventDelegate.GetInvocationList() )
								handler.Method.Invoke( handler.Target, parameters );
						}
					}
					catch
					{
					}
				}

				//!!!!

				public override void AddEventHandler( object target, System.Delegate handler )
				{
					netMember.AddEventHandler( target, handler );
				}

				public override void RemoveEventHandler( object target, System.Delegate handler )
				{
					netMember.RemoveEventHandler( target, handler );
				}

				//!!!!

				public override void AddEventHandler( object target, Delegate handler )
				{
					//!!!!если метод не Net

					netMember.AddEventHandler( target, handler.NetDelegate );
				}

				public override void RemoveEventHandler( object target, Delegate handler )
				{
					//!!!!если метод не Net

					netMember.RemoveEventHandler( target, handler.NetDelegate );
				}
			}

			////////////

			internal NetTypeInfo( string name, string displayName, TypeInfo baseType, TypeClassification classification,
				EDictionary<string, long> enumElements, bool enumFlags, Type type )
				: base( name, displayName, baseType, classification, enumElements, enumFlags )
			{
				this.type = type;

				//InitMembers();

				////memberBySignatureTotal
				//{
				//	if( BaseType != null )
				//		BaseType.MetadataGetMembers(
				//	foreach( var m in memberBySignature.Values )
				//		memberBySignatureTotal[ m.Signature ] = m;
				//}
			}

			public Type Type
			{
				get { return type; }
			}

			//!!!!оператор сравнения и еще какие

			void AddMember( Member member )
			{
				members.Add( member );
				memberBySignature[ member.Signature ] = member;
				//memberByName[ member.MemberType + "$" + member.Name ] = member;
			}

			static object ConvertSimpleDefaultValue( Type memberType, object original )
			{
				if( original != null )
				{
					if( memberType == typeof( float ) )
					{
						if( original.GetType() == typeof( int ) )
							return (float)(int)original;
						else if( original.GetType() == typeof( double ) )
							return (float)(double)original;
					}
					else if( memberType == typeof( double ) )
					{
						if( original.GetType() == typeof( int ) )
							return (double)(int)original;
						else if( original.GetType() == typeof( float ) )
							return (double)(float)original;
					}
					else if( memberType == typeof( Degree ) )
					{
						if( original.GetType() == typeof( int ) )
							return (Degree)(int)original;
						else if( original.GetType() == typeof( float ) )
							return (Degree)(float)original;
						else if( original.GetType() == typeof( double ) )
							return (Degree)(double)original;
					}
					else if( memberType == typeof( DegreeF ) )
					{
						if( original.GetType() == typeof( int ) )
							return (DegreeF)(int)original;
						else if( original.GetType() == typeof( float ) )
							return (DegreeF)(float)original;
						else if( original.GetType() == typeof( double ) )
							return (DegreeF)(double)original;
					}
					else if( memberType == typeof( Radian ) )
					{
						if( original.GetType() == typeof( int ) )
							return (Radian)(int)original;
						else if( original.GetType() == typeof( float ) )
							return (Radian)(float)original;
						else if( original.GetType() == typeof( double ) )
							return (Radian)(double)original;
					}
					else if( memberType == typeof( RadianF ) )
					{
						if( original.GetType() == typeof( int ) )
							return (RadianF)(int)original;
						else if( original.GetType() == typeof( float ) )
							return (RadianF)(float)original;
						else if( original.GetType() == typeof( double ) )
							return (RadianF)(double)original;
					}
				}

				return original;
			}

			internal void InitMembers()
			{
				var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;

				//fields
				foreach( var netField in type.GetFields( flags ) )
				{
					var type = MetadataManager.GetTypeOfNetType( netField.FieldType );
					var m = new NetProperty( this, netField.Name, netField.IsStatic, type, null, Array.Empty<Parameter>(), false, netField );

					m.Description = "";

					foreach( var attrib in netField.GetCustomAttributes( true ) )
					{
						var b = attrib as BrowsableAttribute;
						if( b != null && !b.Browsable )
							m.Browsable = false;

						var c = attrib as CloneableAttribute;
						if( c != null )
							m.Cloneable = c.Type;

						var s = attrib as SerializeAttribute;
						if( s != null )
							m.Serializable = s.Type;

						var d = attrib as DefaultValueAttribute;
						if( d != null )
						{
							m.DefaultValueSpecified = true;
							m.DefaultValue = ConvertSimpleDefaultValue( netField.FieldType, d.Value );
							//m.DefaultValue = MetadataManager.ConvertDefaultValueToDemandType( type.GetNetType(), d.Value );
						}
					}

					//foreach( BrowsableAttribute attrib in netField.GetCustomAttributes( typeof( BrowsableAttribute ), true ) )
					//{
					//	if( !attrib.Browsable )
					//		m.Browsable = false;
					//}
					//foreach( CloneableAttribute attrib in netField.GetCustomAttributes( typeof( CloneableAttribute ), true ) )
					//	m.Cloneable = attrib.Type;
					//foreach( SerializeAttribute attrib in netField.GetCustomAttributes( typeof( SerializeAttribute ), true ) )
					//	m.Serializable = attrib.Type;
					//foreach( DefaultValueAttribute attrib in netField.GetCustomAttributes( typeof( DefaultValueAttribute ), true ) )
					//{
					//	m.DefaultValueSpecified = true;
					//	m.DefaultValue = ConvertSimpleDefaultValue( netField.FieldType, attrib.Value );
					//	//m.DefaultValue = MetadataManager.ConvertDefaultValueToDemandType( type.GetNetType(), attrib.Value );
					//}

					AddMember( m );
				}

				//properties
				foreach( var netProperty in type.GetProperties( flags ) )
				{
					//!!!!new
					var getMethod = netProperty.GetGetMethod();
					if( getMethod != null && getMethod.GetBaseDefinition() != getMethod )
						continue;

					var propertyType = MetadataManager.GetTypeOfNetType( netProperty.PropertyType );

					//!!!!!Accessors
					var netParameters = netProperty.GetIndexParameters();

					List<Parameter> indexers = new List<Parameter>();
					for( int n = 0; n < netParameters.Length; n++ )
					{
						var netParam = netParameters[ n ];

						var netType = netParam.ParameterType;
						bool isByReference = netType.IsByRef;
						if( isByReference )
							netType = netType.GetElementType();

						var type2 = MetadataManager.GetTypeOfNetType( netType );

						Parameter p = new Parameter( netParam.Name, type2, isByReference, netParam.IsIn, netParam.IsOut, netParam.IsRetval, netParam.IsOptional, netParam.HasDefaultValue, netParam.DefaultValue );
						indexers.Add( p );
					}

					var m = new NetProperty( this, netProperty.Name, ReflectionUtility.PropertyIsStatic( netProperty )/*.GetAccessors()[ 0 ].IsStatic*/, propertyType, null, indexers.Count != 0 ? indexers.ToArray() : Array.Empty<Parameter>(), !netProperty.CanWrite, netProperty );

					m.Description = "";

					foreach( var attrib in netProperty.GetCustomAttributes( true ) )
					{
						var b = attrib as BrowsableAttribute;
						if( b != null && !b.Browsable )
							m.Browsable = false;

						var c = attrib as CloneableAttribute;
						if( c != null )
							m.Cloneable = c.Type;

						var s = attrib as SerializeAttribute;
						if( s != null )
							m.Serializable = s.Type;

						var d = attrib as DefaultValueAttribute;
						if( d != null )
						{
							m.DefaultValueSpecified = true;

							////parse Reference<>
							//{
							//	var str = attrib.Value as string;
							//	var prefix = "reference:";
							//	if( str != null && str.Length > prefix.Length && str.Substring( 0, prefix.Length ) == prefix )
							//	{
							//		var referenceValue = str.Substring( prefix.Length );
							//		m.DefaultValue = ReferenceUtils.CreateReference( ReferenceUtils.GetUnreferencedType( netProperty.PropertyType ), null, referenceValue );
							//		continue;
							//	}
							//}

							m.DefaultValue = ConvertSimpleDefaultValue( ReferenceUtility.GetUnreferencedType( netProperty.PropertyType ), d.Value );
							//m.DefaultValue = MetadataManager.ConvertDefaultValueToDemandType( type.GetNetType(), attrib.Value );
						}

						var d2 = attrib as DefaultValueReferenceAttribute;
						if( d2 != null )
						{
							m.DefaultValueSpecified = true;
							m.DefaultValue = ReferenceUtility.MakeReference( ReferenceUtility.GetUnreferencedType( netProperty.PropertyType ), null, d2.ReferenceValue );
						}
					}

					//foreach( BrowsableAttribute attrib in netProperty.GetCustomAttributes( typeof( BrowsableAttribute ), true ) )
					//{
					//	if( !attrib.Browsable )
					//		m.Browsable = false;
					//}
					//foreach( CloneableAttribute attrib in netProperty.GetCustomAttributes( typeof( CloneableAttribute ), true ) )
					//	m.Cloneable = attrib.Type;
					//foreach( SerializeAttribute attrib in netProperty.GetCustomAttributes( typeof( SerializeAttribute ), true ) )
					//	m.Serializable = attrib.Type;
					//foreach( DefaultValueAttribute attrib in netProperty.GetCustomAttributes( typeof( DefaultValueAttribute ), true ) )
					//{
					//	m.DefaultValueSpecified = true;

					//	////parse Reference<>
					//	//{
					//	//	var str = attrib.Value as string;
					//	//	var prefix = "reference:";
					//	//	if( str != null && str.Length > prefix.Length && str.Substring( 0, prefix.Length ) == prefix )
					//	//	{
					//	//		var referenceValue = str.Substring( prefix.Length );
					//	//		m.DefaultValue = ReferenceUtils.CreateReference( ReferenceUtils.GetUnreferencedType( netProperty.PropertyType ), null, referenceValue );
					//	//		continue;
					//	//	}
					//	//}

					//	m.DefaultValue = ConvertSimpleDefaultValue( ReferenceUtility.GetUnreferencedType( netProperty.PropertyType ), attrib.Value );
					//	//m.DefaultValue = MetadataManager.ConvertDefaultValueToDemandType( type.GetNetType(), attrib.Value );
					//}
					//foreach( DefaultValueReferenceAttribute attrib in netProperty.GetCustomAttributes( typeof( DefaultValueReferenceAttribute ), true ) )
					//{
					//	m.DefaultValueSpecified = true;
					//	m.DefaultValue = ReferenceUtility.MakeReference( ReferenceUtility.GetUnreferencedType( netProperty.PropertyType ), null, attrib.ReferenceValue );
					//}

					AddMember( m );
				}

				//constructors
				foreach( var netMethod in type.GetConstructors( flags ) )
				{
					var netParams = netMethod.GetParameters();

					List<Parameter> parameters = new List<Parameter>();
					for( int n = 0; n < netParams.Length; n++ )
					{
						var netParam = netParams[ n ];

						var netType = netParam.ParameterType;
						bool isByReference = netType.IsByRef;
						if( isByReference )
							netType = netType.GetElementType();

						var type = MetadataManager.GetTypeOfNetType( netType );

						Parameter p = new Parameter( netParam.Name, type, isByReference, netParam.IsIn, netParam.IsOut, netParam.IsRetval, netParam.IsOptional, netParam.HasDefaultValue, netParam.DefaultValue );
						parameters.Add( p );
					}

					////return
					//{
					//	var paramType = MetadataManager.GetTypeOfNetType( type );
					//	//!!!!name
					//	Parameter p = new Parameter( "_return", paramType, false, false, true, true, false, false, null );
					//	parameters.Add( p );
					//}

					var m = new NetMethod( this, netMethod.Name, netMethod.IsStatic, parameters.Count != 0 ? parameters.ToArray() : Array.Empty<Parameter>(), true, false, netMethod );
					m.Description = "";

					//skip same constructors declared in base types
					if( BaseType != null && BaseType.MetadataGetMemberBySignature( m.Signature ) != null )
						continue;

					AddMember( m );
				}

				//methods
				foreach( var netMethod in type.GetMethods( flags ) )
				{
					if( netMethod.IsSpecialName )
					{
						if( netMethod.Name.Length > 4 && ( netMethod.Name.Substring( 0, 4 ) == "get_" || netMethod.Name.Substring( 0, 4 ) == "set_" ) )
							continue;
						if( netMethod.Name.Length > 4 && netMethod.Name.Substring( 0, 4 ) == "add_" )
							continue;
						if( netMethod.Name.Length > 7 && netMethod.Name.Substring( 0, 7 ) == "remove_" )
							continue;
					}

					if( netMethod.GetBaseDefinition() != netMethod )
						continue;

					var netParams = netMethod.GetParameters();

					List<Parameter> parameters = new List<Parameter>();
					for( int n = 0; n < netParams.Length; n++ )
					{
						var netParam = netParams[ n ];

						var netType = netParam.ParameterType;
						bool isByReference = netType.IsByRef;
						if( isByReference )
							netType = netType.GetElementType();

						var type = MetadataManager.GetTypeOfNetType( netType );

						Parameter p = new Parameter( netParam.Name, type, isByReference, netParam.IsIn, netParam.IsOut, netParam.IsRetval, netParam.IsOptional, netParam.HasDefaultValue, netParam.DefaultValue );
						parameters.Add( p );
					}

					if( netMethod.ReturnType != null && netMethod.ReturnType != typeof( void ) )
					{
						var netType = netMethod.ReturnType;
						bool isRef = netType.IsByRef;
						if( isRef )
							netType = netType.GetElementType();

						var paramType = MetadataManager.GetTypeOfNetType( netType );
						//"_return"
						Parameter p = new Parameter( "ReturnValue", paramType, isRef, false, true, true, false, false, null );
						parameters.Add( p );
					}

					bool isOperator = netMethod.IsSpecialName && netMethod.Name.Length > 3 && netMethod.Name.Substring( 0, 3 ) == "op_";

					var m = new NetMethod( this, netMethod.Name, netMethod.IsStatic, parameters.Count != 0 ? parameters.ToArray() : Array.Empty<Parameter>(), false, isOperator, netMethod );
					m.Description = "";
					AddMember( m );
				}

				//events
				foreach( var netEvent in type.GetEvents( flags ) )
				{
					var eventHandlerType = MetadataManager.GetTypeOfNetType( netEvent.EventHandlerType );

					//ParameterInfo param = netEvent.GetAddMethod().GetParameters()[ 0 ];
					//Type paramType = param.ParameterType;
					//MethodInfo method = paramType.GetMethod( "Invoke" );

					////if( addMethod.GetParameters().Length != 0 )
					////{
					////	var firstParam = addMethod.GetParameters()[ 0 ];

					////	Delegate d = firstParam.ParameterType as Delegate;
					////	if( d != null )
					////	{
					////	}
					////}

					////MethodInfo method = delegateType.GetMethod( "Invoke" );
					////Console.WriteLine( method.ReturnType.Name + " (ret)" );
					////foreach( ParameterInfo param in method.GetParameters() )
					////{
					////	Console.WriteLine( "{0} {1}", param.ParameterType.Name, param.Name );
					////}

					//var netParams = method.GetParameters();
					////var netParams = netEvent.GetAddMethod().GetParameters();

					//List<Parameter> parameters = new List<Parameter>();
					//for( int n = 0; n < netParams.Length; n++ )
					//{
					//	var netParam = netParams[ n ];

					//	var netType = netParam.ParameterType;
					//	bool isByReference = netType.IsByRef;
					//	if( isByReference )
					//		netType = netType.GetElementType();

					//	var type = MetadataManager.GetTypeOfNetType( netType );

					//	Parameter p = new Parameter( netParam.Name, type, isByReference, netParam.IsIn, netParam.IsOut, netParam.IsRetval,
					//		netParam.IsOptional, netParam.HasDefaultValue, netParam.DefaultValue );
					//	parameters.Add( p );
					//}

					var m = new NetEvent( this, netEvent.Name, netEvent.GetAddMethod().IsStatic, eventHandlerType, netEvent );
					m.Description = "";
					AddMember( m );
				}
			}

			public override IEnumerable<Member> MetadataGetMembers( GetMembersContext context = null )
			{
				foreach( var m in base.MetadataGetMembers( context ) )
					yield return m;
				foreach( var m in members )
					yield return m;
			}

			public override Member MetadataGetMemberBySignature( string signature, GetMembersContext context = null )
			{
				//!!!!memory?
				//if( includeBaseTypes )
				//{
				//	//optimization
				//	Member m;
				//	if( memberBySignatureTotal.TryGetValue( signature, out m ) )
				//		return m;
				//}
				//else
				//{
				Member m;
				if( memberBySignature.TryGetValue( signature, out m ) )
					return m;

				return base.MetadataGetMemberBySignature( signature, context );
				//}

				//return null;
			}

			//public override Member MetadataGetMemberByName( MemberType type, string name, bool includeBaseTypes )
			//{
			//	Member m;
			//	if( memberByName.TryGetValue( type.ToString() + "$" + name, out m ) )
			//		return m;

			//	return base.MetadataGetMemberByName( type, name, includeBaseTypes );
			//}

			public override object InvokeInstance( object[] constructorParams )
			{
				if( constructorParams == null )
					constructorParams = Array.Empty<object>();

				var constructor = ReflectionUtility.GetSuitableConstructor( type, constructorParams, true );

				object obj;
				if( constructor == null && constructorParams.Length == 0 )
				{
					obj = Activator.CreateInstance( type );
				}
				else
				{
					if( constructor != null )
						obj = constructor.Invoke( constructorParams );
					else
						obj = null;

					//if( constructor == null )
					//{
					//	xx xx;

					//	//!!!!!
					//	Log.Fatal( "impl" );
					//}

					//obj = constructor.Invoke( constructorParams );
				}
				return obj;
			}

			public override Type GetNetType()
			{
				return type;
			}

			//public override string ToString()
			//{
			//	//!!!!!ref не пашет

			//	var shortName = MetadataManager.GetNetTypeShortName( type );
			//	if( shortName != null )
			//		return shortName;

			//	return base.ToString();
			//}

			protected override string OnGetUserFriendlyNameForInstance()
			{
				var newName = TypeUtility.GetUserFriendlyNameForInstanceOfType( GetNetType() );
				if( !string.IsNullOrEmpty( newName ) )
					return newName;

				return base.OnGetUserFriendlyNameForInstance();
			}

			//internal override void InitAutoCreatedInstance()
			//{
			//	if( type.GetCustomAttributes( typeof( AutoCreateInstanceAttribute ), false ).Length != 0 )
			//		AutoCreatedInstance = InvokeInstance( null );
			//}

			/// <summary>
			/// Same as Type.Namespace. Faster.
			/// </summary>
			public string Namespace
			{
				get
				{
					if( namespaceCached == null )
						namespaceCached = type.Namespace;
					return namespaceCached;
				}
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				return type.GetCustomAttributes( attributeType, inherit );
			}
			//public override object[] GetCustomAttributes( TypeInfo attributeType, bool inherit )
			//{
			//	return type.GetCustomAttributes( attributeType.GetNetType(), inherit );
			//}

			public override bool Abstract
			{
				get { return type.IsAbstract; }
			}

			public override void GetPropertyDefaultValue( Property property, out bool specified, out object value )
			{
				specified = property.DefaultValueSpecified;
				value = property.DefaultValue;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents type declarations for custom class types, interface types, array types, value types, enumeration types, type parameters, generic type definitions, and open or closed constructed generic types.
		/// </summary>
		public class ComponentTypeInfo : TypeInfo
		{
			Resource resource;
			string pathInside;

			//!!!!только Component?
			//!!!!надо ли ссылкой? везде ли будет эта самообновляемость по ссылке? может надо будет принудительно перекинуться как-то?
			//!!!!object?
			Component basedOnObject;

			//

			public ComponentTypeInfo( string name, string displayName, TypeInfo baseType, TypeClassification classification,
				EDictionary<string, long> enumElements, bool enumFlags, Resource resource, string pathInside, Component basedOnObject )
				: base( name, displayName, baseType, classification, enumElements, enumFlags )
			{
				this.resource = resource;
				this.pathInside = pathInside;
				this.basedOnObject = basedOnObject;

				////!!!!тут?
				//InitAutoCreatedInstance();
			}

			public Resource Resource
			{
				get { return resource; }
			}

			public string PathInside
			{
				get { return pathInside; }
			}

			public Component BasedOnObject
			{
				get { return basedOnObject; }
			}

			public override IEnumerable<Member> MetadataGetMembers( GetMembersContext context = null )
			{
				//foreach( var m in base.MetadataGetMembers( includeBaseTypes ) )
				//	yield return m;

				foreach( var m in basedOnObject.MetadataGetMembers( context ) )
					yield return m;

				//if( basedOnObject != null )
				//{
				//	IMetadataProvider provider = basedOnObject as IMetadataProvider;
				//	if( provider != null )
				//	{
				//		//!!!!так?

				//		foreach( var m in provider.MetadataGetMembers( includeBaseTypes ) )
				//			yield return m;

				//		//var t = MetadataManager.GetTypeOfObject( basedOnObject );
				//		//foreach( var m in t.MetadataGetMembers( includeBaseTypes ) )
				//		//	yield return m;
				//	}
				//	else
				//	{
				//		//!!!!!
				//		Log.Fatal( "impl" );
				//	}
				//}
			}

			public override Member MetadataGetMemberBySignature( string signature, GetMembersContext context = null )
			{
				return basedOnObject.MetadataGetMemberBySignature( signature, context );

				//if( basedOnObject != null )
				//{
				//	IMetadataProvider provider = basedOnObject as IMetadataProvider;
				//	if( provider != null )
				//	{
				//		//!!!!так?
				//		return provider.MetadataGetMemberBySignature( signature, includeBaseTypes );
				//		//var t = MetadataManager.GetTypeOfObject( basedOnObject );
				//		//return t.MetadataGetMemberBySignature( signature, includeBaseTypes );
				//	}
				//	else
				//	{
				//		//!!!!!
				//		Log.Fatal( "impl" );
				//	}
				//}

				//return null;
				//return base.MetadataGetMemberBySignature( signature, includeBaseTypes );
			}

			//public override Member MetadataGetMemberByName( MemberType type, string name, bool includeBaseTypes )
			//{
			//	if( basedOnObject != null )
			//	{
			//		IMetadataProvider provider = basedOnObject as IMetadataProvider;
			//		if( provider != null )
			//		{
			//			//!!!!так?
			//			return provider.MetadataGetMemberByName( type, name, includeBaseTypes );
			//			//var t = MetadataManager.GetTypeOfObject( basedOnObject );
			//			//return t.MetadataGetMemberBySignature( signature, includeBaseTypes );
			//		}
			//		else
			//		{
			//			//!!!!!
			//			Log.Fatal( "impl" );
			//		}
			//	}

			//	return null;
			//	//return base.MetadataGetMemberByName( type, name, includeBaseTypes );
			//}

			//!!!!constructorParams?
			public override object InvokeInstance( object[] constructorParams )
			{
				if( constructorParams == null )
					constructorParams = Array.Empty<object>();

				//!!!!constructor parameters
				//!!!!Component

				var context = new CloneContext();
				context.typeOfCloning = CloneContext.TypeOfCloningEnum.CreateInstanceOfType;
				context.baseTypeForCreationInstance = this;
				var newComponent = basedOnObject.Clone( context );
				//var newComponent = basedOnObject.Clone( null, null, this );

				return newComponent;

				//Component component = basedOnObject as Component;
				//if( component != null )
				//{
				//	var newComponent = component.Clone( null, null, this );
				//	return newComponent;
				//}
				//else
				//{
				//	//!!!!!
				//	Log.Fatal( "impl" );
				//	return null;
				//}
			}

			protected override string OnGetUserFriendlyNameForInstance()
			{
				if( basedOnObject.Name != "" )
					return basedOnObject.Name;
				//var basedOnComponent = BasedOnObject as Component;
				//if( basedOnComponent != null && basedOnComponent.Name != "" )
				//	return basedOnComponent.Name;

				//!!!!!или по имени файла, если рутовый

				return base.OnGetUserFriendlyNameForInstance();
			}

			//internal override void InitAutoCreatedInstance()
			//{
			//	//!!!!impl
			//}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				//!!!!impl

				return GetNetType().GetCustomAttributes( attributeType, inherit );
			}
			//public override object[] GetCustomAttributes( TypeInfo attributeType, bool inherit )
			//{
			//	//!!!!impl
			//	return GetNetType().GetCustomAttributes( attributeType.GetNetType(), inherit );
			//}

			public override bool Abstract
			{
				get
				{
					//!!!!всегда false?
					return false;
				}
			}

			public override void GetPropertyDefaultValue( Property property, out bool specified, out object value )
			{
				if( property.Indexers == null || property.Indexers.Length == 0 )
				{
					if( basedOnObject.MetadataGetMemberBySignature( property.Signature ) == property )
					{
						specified = true;

						value = property.GetValue( basedOnObject, null );

						var iReference = value as IReference;
						if( iReference != null && !iReference.ReferenceSpecified )
							value = ReferenceUtility.GetUnreferencedValue( value );

						return;
					}
				}

				basedOnObject.BaseType.GetPropertyDefaultValue( property, out specified, out value );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public class ClarifyTypeAttribute : Attribute
		//{
		//	string typeName;
		//	Type type;

		//	public ClarifyTypeAttribute( string typeName )
		//	{
		//		this.typeName = typeName;
		//	}

		//	public ClarifyTypeAttribute( Type type )
		//	{
		//		this.type = type;
		//	}

		//	public string TypeName
		//	{
		//		get { return typeName; }
		//	}

		//	public Type Type
		//	{
		//		get { return type; }
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a settings for getting list of members of the type by <see cref="IMetadataProvider.MetadataGetMembers(GetMembersContext)"/> method.
		/// </summary>
		public class GetMembersContext
		{
			public bool filter = true;

			public GetMembersContext( bool filter = true )
			{
				this.filter = filter;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!может не тут

		/// <summary>
		/// Provides data when objects are loading.
		/// </summary>
		public class LoadContext
		{
			//!!!!public
			//!!!!threading?

			public string virtualFileName;

			//!!!!было before ComponentItems
			//!!!!why not "object" or IMetadataProvider
			//public Dictionary<Component, TextBlock> textBlockByComponent = new Dictionary<Component, TextBlock>();

			/// <summary>
			/// Represents a item of component for <see cref="LoadContext"/>.
			/// </summary>
			public class ComponentItem
			{
				//!!!!public

				public ComponentItem parent;
				public TextBlock textBlock;
				//!!!!
				public string name = "";

				public ComponentItem[] children;

				public Component component;
				public bool loaded;
				//!!!!
				public string error;
			}
			public ComponentItem rootComponentItem;

			public List<ComponentItem> allComponentItemsCreationOrder = new List<ComponentItem>();
			public List<ComponentItem> allComponentItemsSerializationOrder = new List<ComponentItem>();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Provides data when objects are saving.
		/// </summary>
		public class SaveContext
		{
			//!!!!public
			public string realFileName;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Provides data when objects are cloning.
		/// </summary>
		public class CloneContext
		{
			public enum TypeOfCloningEnum
			{
				Usual,
				CreateInstanceOfType,
			}
			public TypeOfCloningEnum typeOfCloning = TypeOfCloningEnum.Usual;
			public ComponentTypeInfo baseTypeForCreationInstance;
			//public TypeInfo baseTypeForCreationInstance;

			//!!!!было сделано но не юзалось
			//public Type overrideClass;

			//!!!!not used
			////!!!!new
			////!!!!public
			//public bool unreferenceProperties;

			//!!!!!
			//!!!!why not "object" or IMetadataProvider
			public EDictionary<Component, Component> newComponentsRedirection = new EDictionary<Component, Component>();
			public Queue<(Component, Component)> newComponentsQueue = new Queue<(Component, Component)>();

			//

			////!!!!!надо ли
			////!!!!public, virtual. event?
			//public virtual void CloneObject( ref object v )
			//{
			//	MetadataManager.Serialization.CloneObject_DefaultImpl( this, ref v );

			//	//if( v != null )
			//	//{
			//	//	var item = MetadataManager.GetSuitableSerializableType( v.GetType() );
			//	//	//var item = GetSuitableSerializableType( valueType );
			//	//	if( item != null )
			//	//	{
			//	//		item.Clone( this, ref v );
			//	//	}
			//	//	else
			//	//	{
			//	//	}
			//	//}
			//}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		///// <summary>
		///// Attribute to customize the user-friendly name a new object of the type.
		///// </summary>
		//[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
		//public class UserFriendlyNameForInstanceOfTypeAttribute : Attribute
		//{
		//	string name;

		//	//

		//	public UserFriendlyNameForInstanceOfTypeAttribute( string name )
		//	{
		//		this.name = name;
		//	}

		//	public string Name
		//	{
		//		get { return name; }
		//	}
		//}
	}
}
