// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Linq;
using System.Reflection;

namespace NeoAxis
{
	class CompiledScript
	{
		Type type;
		//object instance;
		FieldInfo[] fields;
		PropertyInfo[] properties;
		MethodInfo[] methods;

		//

		internal static CompiledScript CreateFrom( Assembly assembly )
		{
			// get first type with GeneratedScriptAttribute
			var type = assembly.GetTypes().First( t => t.GetCustomAttribute<CSharpScriptGeneratedAttribute>() != null );
			return CreateFrom( type );
		}

		internal static CompiledScript CreateFrom( Type type )
		{
			var bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

			var fields = type.GetFields( bindingFlags );
			var properties = type.GetProperties( bindingFlags );

			//!!!!new
			var methods2 = type.GetMethods( bindingFlags );
			//var methods2 = type.GetMethods( BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			var methods = ( methods2.Any( m => m.IsPublic ) ? methods2.Where( m => m.IsPublic ) : methods2 ).ToArray();

			//var instance = Activator.CreateInstance( type );
			return new CompiledScript( type/* instance*/, fields, properties, methods );
		}

		internal CompiledScript( Type type, FieldInfo[] fields, PropertyInfo[] properties, MethodInfo[] methods )
		{
			this.fields = fields;
			this.properties = properties;
			this.methods = methods;
			this.type = type;
		}

		public Type Type
		{
			get { return type; }
		}

		//public object Instance
		//{
		//	get { return instance; }
		//}

		public FieldInfo[] Fields
		{
			get { return fields; }
		}

		public PropertyInfo[] Properties
		{
			get { return properties; }
		}

		public MethodInfo[] Methods
		{
			get { return methods; }
		}

		//public object InvokeMethod( MethodInfo method, object[] parameters, out string error )
		//{
		//	error = "";

		//	try
		//	{
		//		return method.Invoke( !method.IsStatic ? instance : null, parameters );
		//	}
		//	catch( Exception e )
		//	{
		//		error = e.Message;
		//		return null;
		//	}
		//}

		//!!!!
		public void SetField( object instance, string name, object value, out string error )
		{
			error = "";

			//!!!!slowly?

			var field = fields.FirstOrDefault( f => f.Name == name );
			if( field != null )
				field.SetValue( instance, value );
			else
				error = name + " field or property not found.";
		}
	}
}
