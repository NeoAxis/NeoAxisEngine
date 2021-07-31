// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;

namespace NeoAxis
{
	//!!!!какой-то устаревший класс
	//!!!!threading

	/// <summary>
	/// Class for working with the application config.
	/// </summary>
	public static class EngineConfig
	{
		static string realFileName;
		static TextBlock textBlock;
		static List<Parameter> parameters = new List<Parameter>();

		///////////////////////////////////////////

		public delegate void RegisterConfigParameterDelegate( /*EngineConfig config, */Parameter parameter );
		public static event RegisterConfigParameterDelegate RegisterConfigParameter;

		//!!!!
		//public delegate void LoadEventDelegate( /*EngineConfig config, */TextBlock block, ref string error );
		//public static event LoadEventDelegate LoadEvent;

		public delegate void SaveEventDelegate( /*EngineConfig config, TextBlock block*/ );
		public static event SaveEventDelegate SaveEvent;

		///////////////////////////////////////////

		/// <summary>
		/// Represents a parameter for <see cref="EngineConfig"/>.
		/// </summary>
		public class Parameter
		{
			string groupPath;
			string name;
			FieldInfo field;
			PropertyInfo property;
			string defaultValue;

			//

			public string GroupPath
			{
				get { return groupPath; }
			}

			public string Name
			{
				get { return name; }
			}

			public FieldInfo Field
			{
				get { return field; }
			}

			public PropertyInfo Property
			{
				get { return property; }
			}

			public string DefaultValue
			{
				get { return defaultValue; }
			}

			public object GetValue()
			{
				if( field != null )
					return field.GetValue( null );
				else if( property != null )
					return property.GetValue( null, null );
				else
					return null;
			}

			internal Parameter( string groupPath, string name, FieldInfo field, PropertyInfo property )
			{
				this.groupPath = groupPath;
				this.name = name;
				this.field = field;
				this.property = property;

				object value = GetValue();
				if( value != null )
					defaultValue = value.ToString();
				else
					defaultValue = "";
			}

			object LoadValue( TextBlock block, Type valueType )
			{
				if( valueType.IsGenericType && valueType.Name == typeof( List<> ).Name )
				{
					//List<>
					Type[] genericArguments = valueType.GetGenericArguments();
					Trace.Assert( genericArguments.Length == 1 );
					Type itemType = genericArguments[ 0 ];

					MethodInfo methodAdd = valueType.GetMethod( "Add" );

					TextBlock listBlock = block.FindChild( name );

					if( listBlock == null )
						return null;

					//create list
					object value;
					{
						value = valueType.InvokeMember( "", BindingFlags.Public |
							BindingFlags.NonPublic | BindingFlags.CreateInstance |
							BindingFlags.Instance, null, null, null );
						//ConstructorInfo constructor = valueType.GetConstructor( new Type[ 0 ] );
						//value = constructor.Invoke( null );
					}

					if( itemType == typeof( string ) )
					{
						for( int n = 0; ; n++ )
						{
							string attributeName = string.Format( "item{0}", n );

							if( !listBlock.AttributeExists( attributeName ) )
								break;

							string itemValue = listBlock.GetAttribute( attributeName );
							methodAdd.Invoke( value, new object[] { itemValue } );
						}
					}
					else
					{
						List<FieldInfo> itemFields = new List<FieldInfo>();
						{
							foreach( FieldInfo field in itemType.GetFields( BindingFlags.Instance |
								BindingFlags.NonPublic | BindingFlags.Public ) )
							{
								if( field.GetCustomAttributes( typeof( EngineConfigAttribute ), true ).Length != 0 )
									itemFields.Add( field );
							}
						}
						List<PropertyInfo> itemProperties = new List<PropertyInfo>();
						{
							foreach( PropertyInfo property in itemType.GetProperties( BindingFlags.Instance |
								BindingFlags.NonPublic | BindingFlags.Public ) )
							{
								if( property.GetCustomAttributes( typeof( EngineConfigAttribute ), true ).Length != 0 )
									itemProperties.Add( property );
							}
						}

						foreach( TextBlock itemBlock in listBlock.Children )
						{
							object itemValue = itemType.InvokeMember( "", BindingFlags.Public |
								BindingFlags.NonPublic | BindingFlags.CreateInstance |
								BindingFlags.Instance, null, null, null );
							//ConstructorInfo constructor = itemType.GetConstructor( new Type[ 0 ] );
							//object itemValue = constructor.Invoke( null );

							//item fields
							foreach( FieldInfo itemField in itemFields )
							{
								string itemName = ( (EngineConfigAttribute)itemField.GetCustomAttributes( typeof( EngineConfigAttribute ), true )[ 0 ] ).GetName( itemField );

								if( !itemBlock.AttributeExists( itemName ) )
									continue;

								string strValue = itemBlock.GetAttribute( itemName );

								try
								{
									object v = SimpleTypes.ParseValue(
										itemField.FieldType, strValue );

									if( v == null )
										Log.Error( "Config: Not implemented parameter type \"{0}\"", itemField.FieldType.ToString() );
									if( v != null )
										itemField.SetValue( itemValue, v );
								}
								catch( Exception e )
								{
									Log.Warning( "Config: {0} ({1})", itemName, e.Message );
								}
							}

							//property fields
							foreach( PropertyInfo itemProperty in itemProperties )
							{
								string itemName = ( (EngineConfigAttribute)itemProperty.GetCustomAttributes( typeof( EngineConfigAttribute ), true )[ 0 ] ).GetName( itemProperty );

								if( !itemBlock.AttributeExists( itemName ) )
									continue;

								string strValue = itemBlock.GetAttribute( itemName );

								try
								{
									object v = SimpleTypes.ParseValue(
										itemProperty.PropertyType, strValue );

									if( v == null )
										Log.Error( "Config: Not implemented parameter type \"{0}\"", itemProperty.PropertyType.ToString() );
									if( v != null )
										itemProperty.SetValue( itemValue, v, null );
								}
								catch( Exception e )
								{
									Log.Warning( "Config: {0} ({1})", itemName, e.Message );
								}
							}

							methodAdd.Invoke( value, new object[] { itemValue } );
						}
					}
					return value;
				}

				// default load
				if( block.AttributeExists( name ) )
				{
					string strValue = block.GetAttribute( name );

					try
					{
						object value = SimpleTypes.ParseValue( valueType, strValue );

						if( value == null )
							Log.Error( "Config: Not implemented parameter type \"{0}\"", valueType.ToString() );

						return value;
					}
					catch( Exception e )
					{
						Log.Warning( "Config: {0} ({1})", name, e.Message );
						return null;
					}
				}
				return null;
			}

			internal void Load( TextBlock textBlock )
			{
				string[] list = groupPath.Split( "\\/".ToCharArray() );

				TextBlock block = textBlock;
				foreach( string dir in list )
				{
					TextBlock child = block.FindChild( dir );
					if( child == null )
						return;
					block = child;
				}

				//if( !block.IsAttributeExist( name ) )
				//   return true;

				//string strValue = block.GetAttribute( name );

				try
				{
					Type valueType = null;
					object lastValue = null;
					if( field != null )
					{
						valueType = field.FieldType;
						lastValue = field.GetValue( null );
					}
					if( property != null )
					{
						valueType = property.PropertyType;
						lastValue = property.GetValue( null, null );
					}

					object value = LoadValue( block, valueType );

					if( value != null )
					{
						if( field != null )
							field.SetValue( null, value );
						if( property != null )
							property.SetValue( null, value, null );
					}

					////field
					//if( field != null )
					//{
					//   object value = SimpleTypesUtils.GetSimpleTypeValue( field.FieldType, strValue );
					//   if( value == null )
					//   {
					//      Log.Error( "Config: Not implemented parameter type \"{0}\"",
					//         field.FieldType.ToString() );
					//      return false;
					//   }
					//   field.SetValue( null, value );
					//}

					////property
					//if( property != null )
					//{
					//   object value = SimpleTypesUtils.GetSimpleTypeValue( property.PropertyType, strValue );
					//   if( value == null )
					//   {
					//      Log.Error( "Config: Not implemented parameter type \"{0}\"",
					//         property.PropertyType.ToString() );
					//      return false;
					//   }
					//   property.SetValue( null, value, null );
					//}
				}
				catch( FormatException e )
				{
					string s = "";
					if( field != null )
						s = field.FieldType.ToString();
					else if( property != null )
						s = property.PropertyType.ToString();
					Log.Warning( "Config : Invalid parameter format \"{0}\". {1}.", s, e.Message );
				}
			}

			void SaveValue( TextBlock block, object value )
			{
				Type valueType = value.GetType();

				if( valueType.IsGenericType && valueType.Name == typeof( List<> ).Name )
				{
					//List<>
					Type[] genericArguments = valueType.GetGenericArguments();
					Trace.Assert( genericArguments.Length == 1 );
					Type itemType = genericArguments[ 0 ];

					int count = (int)valueType.GetProperty( "Count" ).GetValue( value, null );

					PropertyInfo itemPropertyInfo = valueType.GetProperty( "Item" );

					//delete old blocks
					{
						ggg:
						TextBlock b = block.FindChild( name );
						if( b != null )
						{
							block.DeleteChild( b );
							goto ggg;
						}
					}

					TextBlock listBlock = block.AddChild( name );

					if( itemType == typeof( string ) )
					{
						for( int n = 0; n < count; n++ )
						{
							object itemObject = itemPropertyInfo.GetValue( value, new object[] { n } );
							listBlock.SetAttribute( string.Format( "item{0}", n ), itemObject.ToString() );
						}
					}
					else
					{
						List<FieldInfo> itemFields = new List<FieldInfo>();
						{
							foreach( FieldInfo field in itemType.GetFields( BindingFlags.Instance |
								BindingFlags.NonPublic | BindingFlags.Public ) )
							{
								if( field.GetCustomAttributes( typeof( EngineConfigAttribute ), true ).Length != 0 )
									itemFields.Add( field );
							}
						}
						List<PropertyInfo> itemProperties = new List<PropertyInfo>();
						{
							foreach( PropertyInfo property in itemType.GetProperties( BindingFlags.Instance |
								BindingFlags.NonPublic | BindingFlags.Public ) )
							{
								if( property.GetCustomAttributes( typeof( EngineConfigAttribute ), true ).Length != 0 )
									itemProperties.Add( property );
							}
						}

						for( int n = 0; n < count; n++ )
						{
							TextBlock itemBlock = listBlock.AddChild( "item" );

							object itemObject = itemPropertyInfo.GetValue( value, new object[] { n } );

							//item fields
							foreach( FieldInfo itemField in itemFields )
							{
								string itemName = ( (EngineConfigAttribute)itemField.GetCustomAttributes( typeof( EngineConfigAttribute ), true )[ 0 ] ).GetName( itemField );

								object obj = itemField.GetValue( itemObject );
								if( obj != null )
									itemBlock.SetAttribute( itemName, obj.ToString() );
							}

							foreach( PropertyInfo itemProperty in itemProperties )
							{
								string itemName = ( (EngineConfigAttribute)itemProperty.GetCustomAttributes( typeof( EngineConfigAttribute ), true )[ 0 ] ).GetName( itemProperty );

								object obj = itemProperty.GetValue( itemObject, null );
								if( obj != null )
									itemBlock.SetAttribute( itemName, obj.ToString() );
							}
						}
					}
					return;
				}

				//default save
				block.SetAttribute( name, value.ToString() );
			}

			internal void Save( TextBlock textBlock )
			{
				string[] list = groupPath.Split( "\\/".ToCharArray() );

				TextBlock block = textBlock;
				foreach( string dir in list )
				{
					TextBlock child = block.FindChild( dir );
					if( child == null )
						child = block.AddChild( dir );
					block = child;
				}

				object value = null;
				if( field != null )
					value = field.GetValue( null );
				else if( property != null )
					value = property.GetValue( null, null );
				if( value != null )
					SaveValue( block, value );
			}

		}

		///////////////////////////////////////////

		public static bool Init( string fileName, out string error )
		{
			error = null;

			realFileName = VirtualPathUtility.GetRealPathByVirtual( fileName );

			if( !File.Exists( realFileName ) )
			{
				textBlock = new TextBlock();
				return true;
			}

			textBlock = TextBlockUtility.LoadFromRealFile( realFileName, out error );
			if( textBlock == null )
				return false;

			foreach( Parameter parameter in parameters )
				parameter.Load( textBlock );

			//!!!!
			//LoadEvent?.Invoke( /*this, */textBlock, ref error );
			//if( !string.IsNullOrEmpty( error ) )
			//	return false;

			//if( !parameter.Load( textBlock ) )
			//   return false;

			return true;
		}

		public static bool Save()
		{
			if( textBlock == null )
				textBlock = new TextBlock();

			foreach( Parameter parameter in parameters )
				parameter.Save( textBlock );

			SaveEvent?.Invoke( /*this, textBlock */);

			try
			{
				string directoryName = Path.GetDirectoryName( realFileName );
				if( directoryName != "" && !Directory.Exists( directoryName ) )
					Directory.CreateDirectory( directoryName );

				using( StreamWriter writer = new StreamWriter( realFileName ) )
				{
					writer.Write( textBlock.DumpToString() );
				}
			}
			catch
			{
				Log.Error( "Unable to save file \"{0}\".", realFileName );
				return false;
			}
			return true;
		}

		static bool RegisterParameter( string groupPath, string name, FieldInfo field, PropertyInfo property )
		{
			Trace.Assert( field != null || property != null );

			foreach( Parameter parameter in parameters )
			{
				if( parameter.GroupPath == groupPath && parameter.Field == field &&
					parameter.Property == property )
				{
					//Log.Error( "Config: Parameter already registered \"{0}\"", groupPath + "\\" + field.Name );
					return false;
				}
			}

			Parameter p = new Parameter( groupPath, name, field, property );
			parameters.Add( p );

			if( textBlock != null )
				p.Load( textBlock );

			RegisterConfigParameter?.Invoke( /*this, */p );
			if( EngineApp.Instance != null )
				EngineApp.PerformRegisterConfigParameter( p );

			return true;
		}

		//!!!!может делать это автоматически всей сборке
		public static void RegisterClassParameters( Type classType )
		{
			Type loopClassType = classType;
			while( loopClassType != null )
			{
				FieldInfo[] fields = loopClassType.GetFields( BindingFlags.Static | BindingFlags.NonPublic |
					BindingFlags.Public );

				//Fields
				foreach( FieldInfo field in fields )
				{
					object[] attributes = field.GetCustomAttributes( typeof( EngineConfigAttribute ), true );

					if( attributes.Length == 0 )
						continue;
					Trace.Assert( attributes.Length == 1 );
					EngineConfigAttribute attribute = (EngineConfigAttribute)attributes[ 0 ];

					string name = attribute.GetName( field );
					RegisterParameter( attribute.GroupPath, name, field, null );
				}

				//Properties
				PropertyInfo[] properties = loopClassType.GetProperties( BindingFlags.Static | BindingFlags.NonPublic |
					BindingFlags.Public );

				foreach( PropertyInfo property in properties )
				{
					object[] attributes = property.GetCustomAttributes( typeof( EngineConfigAttribute ), true );

					if( attributes.Length == 0 )
						continue;
					Trace.Assert( attributes.Length == 1 );
					EngineConfigAttribute attribute = (EngineConfigAttribute)attributes[ 0 ];

					string name = attribute.GetName( property );
					RegisterParameter( attribute.GroupPath, name, null, property );
				}

				loopClassType = loopClassType.BaseType;
			}
		}

		public static ReadOnlyCollection<Parameter> Parameters
		{
			get { return parameters.AsReadOnly(); }
		}

		public static Parameter GetParameter( string groupPath, string name )
		{
			foreach( Parameter parameter in parameters )
				if( parameter.GroupPath == groupPath && parameter.Name == name )
					return parameter;
			return null;
		}

		public static TextBlock TextBlock
		{
			get { return textBlock; }
		}
	}
}
