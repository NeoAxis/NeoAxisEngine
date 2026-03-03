#if !UWP
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NeoAxis
{
	public sealed class NetMemoryDump
	{
		// Count by runtime Type.
		readonly Dictionary<Type, TypeInfo> _types = new Dictionary<Type, TypeInfo>();

		// Avoid infinite recursion on object graphs with cycles.
		readonly HashSet<object> _visited = new HashSet<object>( ReferenceEqualityComparer.Instance );

		///////////////////////////////////////////////

		sealed class TypeInfo
		{
			public Type Type { get; }
			public long Count { get; set; }

			public TypeInfo( Type type )
			{
				Type = type;
				Count = 0;
			}
		}

		///////////////////////////////////////////////

		public void CollectFromAllStaticFields( Assembly[] assembliesToScan )
		{
			foreach( var assembly in assembliesToScan )
			{
				Type[] types;
				try
				{
					types = assembly.GetTypes();
				}
				catch( ReflectionTypeLoadException ex )
				{
					types = ex.Types?.Where( t => t != null ).ToArray() ?? Array.Empty<Type>();
				}
				catch
				{
					// Anything else: skip this assembly
					continue;
				}

				foreach( var type in types )
				{
					if( type == null )
						continue;

					// Skip generic type definitions and some system internals
					if( type.IsGenericTypeDefinition )
						continue;

					CollectFromTypeStaticFields( type );
				}
			}
		}

		void CollectFromTypeStaticFields( Type type )
		{
			FieldInfo[] fields;
			try
			{
				fields = type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );
			}
			catch
			{
				return;
			}

			foreach( var field in fields )
			{
				// Skip pointer, by-ref and similar weird types
				var ft = field.FieldType;
				if( ft.IsPointer || ft.IsByRef )
					continue;

				// We only want reference-type roots
				if( !IsReferenceLike( ft ) )
					continue;

				object value;
				try
				{
					value = field.GetValue( null );
				}
				catch
				{
					// Some runtime static fields throw when accessed
					continue;
				}

				if( value == null )
					continue;

				TraverseObjectGraph( value );
			}
		}

		static bool IsReferenceLike( Type t )
		{
			if( t == null )
				return false;

			if( t.IsPrimitive || t.IsEnum )
				return false;

			if( t == typeof( string ) )
				return true;

			if( t.IsClass )
				return true;

			if( typeof( IEnumerable ).IsAssignableFrom( t ) )
				return true;

			return false;
		}

		void TraverseObjectGraph( object root )
		{
			if( root == null )
				return;

			// check for already processed
			if( !_visited.Add( root ) )
				return;

			RegisterType( root.GetType() );

			var type = root.GetType();

			// Stop for primitive, enum, string
			if( type.IsPrimitive || type.IsEnum || type == typeof( string ) )
				return;

			// Arrays
			if( type.IsArray )
			{
				var arr = root as Array;
				if( arr != null )
				{
					foreach( var item in arr )
					{
						if( item != null )
							SafeTraverseChild( item );
					}
				}
				return;
			}

			// Enumerables (collections, lists, etc.)
			try
			{
				if( root is IEnumerable enumerable && !( root is string ) )
				{
					foreach( var item in enumerable )
					{
						if( item != null )
							SafeTraverseChild( item );
					}
				}
			}
			catch
			{
				return;
			}

			FieldInfo[] fields;
			try
			{
				fields = type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			}
			catch
			{
				return;
			}

			foreach( var field in fields )
			{
				var ft = field.FieldType;

				if( ft.IsPointer || ft.IsByRef )
					continue;

				if( !IsReferenceLike( ft ) )
					continue;

				object value;
				try
				{
					value = field.GetValue( root );
				}
				catch
				{
					continue;
				}

				if( value != null )
					SafeTraverseChild( value );
			}
		}

		void SafeTraverseChild( object obj )
		{
			try
			{
				TraverseObjectGraph( obj );
			}
			catch
			{
				// In ASP.NET you never want diagnostic code to bring the app down
			}
		}

		void RegisterType( Type type )
		{
			if( type == null )
				return;

			if( !_types.TryGetValue( type, out var info ) )
			{
				info = new TypeInfo( type );
				_types[ type ] = info;
			}

			info.Count++;
		}

		public IEnumerable<(Type Type, long Count)> GetAllTypesWithCounts()
		{
			foreach( var kvp in _types )
				yield return (kvp.Key, kvp.Value.Count);
		}

		string GetTypeName( Type type )
		{
			return ( type.Namespace ?? "<null>" ) + "." + ( type.Name ?? "<null>" );
		}

		/// <summary>
		/// Show first N types with maximal amount (occurrence count), ordered descending by Count.
		/// </summary>
		public string GetTopTypesAsText( int top = 100 )
		{
			var topTypes = GetAllTypesWithCounts().OrderByDescending( x => x.Count ).Take( top ).ToArray();

			var sb = new System.Text.StringBuilder();

			int index = 0;
			foreach( var (type, count) in topTypes )
			{
				if( type == null )
					continue;

				var typeName = GetTypeName( type );

				//add generic arguments if any
				if( type.IsGenericType )
				{
					var genericArgs = type.GetGenericArguments();
					typeName += "<" + string.Join( ", ", genericArgs.Select( t => GetTypeName( t ) ) ) + ">";
				}

				sb.AppendFormat( "#{0,3}: Count={1,8} | Type={2}", ++index, count, typeName );
				sb.AppendLine();
			}

			return sb.ToString();
		}

		public string DumpTopStaticTypes( Assembly[] assembliesToScan, int top = 100 )
		{
			CollectFromAllStaticFields( assembliesToScan );
			return GetTopTypesAsText( top );
		}
	}
}
#endif