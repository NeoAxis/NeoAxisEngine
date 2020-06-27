// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Text;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public static class EditorUtility
	{
		internal static Metadata.GetMembersContext getMemberContextNoFilter = new Metadata.GetMembersContext( false );

		public static Component CloneComponent( Component source )
		{
			var newObject = source.Clone();

			newObject.Name = source.Parent.Components.GetUniqueName( source.Name, true, 2 );

			int index = source.Parent.Components.IndexOf( source );
			if( index != -1 )
				index++;
			source.Parent.AddComponent( newObject, index );

			return newObject;
		}

		//!!!!если надо, то как EditorAction
		//public static void ShowObjectDetailsAsDocument( object obj )
		//{
		//	List<string> lines = new List<string>( 256 );

		//	lines.Add( obj.ToString() );
		//	lines.Add( "" );
		//	lines.Add( "----------------------------------------------------------------------------------------------------" );

		//	lines.Add( "Inheritance Hierarchy" );
		//	lines.Add( "" );

		//	var type = MetadataManager.MetadataGetType( obj );
		//	int step = 0;
		//	do
		//	{
		//		lines.Add( new string( ' ', step * 3 ) + type.ToString() );

		//		step++;
		//		type = type.BaseType;
		//	} while( type != null );

		//	//lines.Add( "Type \'" + MetadataManager.MetadataGetType( obj ).ToString() + "\'" );
		//	//lines.Add( ".NET type \'" + obj.GetType().ToString() + "\'" );
		//	lines.Add( "" );
		//	lines.Add( "----------------------------------------------------------------------------------------------------" );
		//	lines.Add( "" );

		//	foreach( var member in MetadataManager.MetadataGetMembers( obj ) )
		//	{
		//		Metadata.Method method = member as Metadata.Method;
		//		if( method != null )
		//		{
		//			lines.Add( method.Signature );
		//			lines.Add( "    " + method.ToString() );
		//			lines.Add( "" );
		//		}
		//	}

		//	lines.Add( "----------------------------------------------------------------------------------------------------" );
		//	lines.Add( "" );

		//	foreach( var member in MetadataManager.MetadataGetMembers( obj ) )
		//	{
		//		Metadata.Event evt = member as Metadata.Event;
		//		if( evt != null )
		//		{
		//			lines.Add( evt.Signature );
		//			lines.Add( "    " + evt.ToString() );
		//			lines.Add( "" );
		//		}
		//	}

		//	lines.Add( "----------------------------------------------------------------------------------------------------" );
		//	lines.Add( "" );

		//	foreach( var member in MetadataManager.MetadataGetMembers( obj ) )
		//	{
		//		Metadata.Property prop = member as Metadata.Property;
		//		if( prop != null )
		//		{
		//			lines.Add( prop.Signature );
		//			lines.Add( "    " + prop.ToString() );
		//			lines.Add( "" );
		//		}
		//	}

		//	//lines.Add( "Type: " + MetadataManager.MetadataGetType( obj ).ToString() );
		//	//lines.Add( "Net type: " + obj.GetType().ToString() );
		//	//lines.Add( "" );
		//	//lines.Add( "ToString(): " + obj.ToString() );
		//	//lines.Add( "" );
		//	//lines.Add( "----------------------------------------------------------------------------------------------------" );
		//	//lines.Add( "Metadata:" );



		//	//!!!!!!
		//	//if( component != null )
		//	//{
		//	//	List<string> list = new List<string>();

		//	//	var members = component.MetadataGetMembers( true );

		//	//	list.Add( component.ToString() );

		//	//	list.Add( "" );
		//	//	list.Add( "Events:" );
		//	//	foreach( var m in members )
		//	//	{
		//	//		var evn = m as Metadata.Event;
		//	//		if( evn != null )
		//	//			list.Add( evn.ToString() + " - " + evn.Signature );
		//	//	}

		//	//	list.Add( "" );
		//	//	list.Add( "Properties:" );
		//	//	foreach( var m in members )
		//	//	{
		//	//		var p = m as Metadata.Property;
		//	//		if( p != null )
		//	//			list.Add( p.ToString() + " - " + p.Signature );
		//	//	}

		//	//	list.Add( "" );
		//	//	list.Add( "Methods:" );
		//	//	foreach( var m in members )
		//	//	{
		//	//		var method = m as Metadata.Method;
		//	//		if( method != null )
		//	//			list.Add( method.ToString() + " - " + method.Signature );
		//	//	}

		//	//	//!!!!!!log out
		//	//	{
		//	//		Log.Info( "" );
		//	//		Log.Info( "" );
		//	//		Log.Info( "---------------------------------- START -------------------------------------" );

		//	//		foreach( var t in list )
		//	//			Log.Info( t );

		//	//		Log.Info( "----------------------------------- END --------------------------------------" );
		//	//	}

		//	//	viewport.GuiRenderer.AddTextLines( list, new Vec2( .03, .1 ), EHorizontalAlign.Left, EVerticalAlign.Top, 0,
		//	//		new ColorValue( 1, 1, 0 ) );
		//	//}

		//	StringBuilder text = new StringBuilder();
		//	foreach( var line in lines )
		//		text.Append( line + "\r\n" );
		//	EditorForm.Instance.OpenTextAsDocument( text.ToString(), MetadataManager.MetadataGetType( obj ).ToString(), true );
		//}

		public static void SetPropertyReference( DocumentInstance document, object/* Component*/[] objects, Metadata.Property property, object[] indexers, string[] referenceValues )
		{
			var netType = property.Type.GetNetType();
			var underlyingType = ReferenceUtility.GetUnderlyingType( netType );

			var undoItems = new List<UndoActionPropertiesChange.Item>();

			//!!!!try, catch? где еще

			for( int n = 0; n < objects.Length; n++ )
			{
				var obj = objects[ n ];

				var value = ReferenceUtility.MakeReference( underlyingType, null, referenceValues[ n ] );
				var oldValue = (IReference)property.GetValue( obj, indexers );

				//bool change = true;
				//if( /*skipSame && */oldValue != null && value.GetByReference == oldValue.GetByReference )
				//	change = false;

				if( !value.Equals( oldValue ) )
				{
					property.SetValue( obj, value, indexers );
					undoItems.Add( new UndoActionPropertiesChange.Item( obj, property, oldValue, indexers ) );
				}
			}

			//undo
			if( undoItems.Count != 0 )
			{
				var action = new UndoActionPropertiesChange( undoItems.ToArray() );
				document.UndoSystem.CommitAction( action );
				document.Modified = true;
			}
		}

		//public static void SetPropertyResourceName( DocumentInstance document, object[] objects, Metadata.Property property, object[] indexers, string resourceName )
		//{
		//	var netType = property.Type.GetNetType();
		//	var underlyingType = ReferenceUtils.GetUnderlyingType( netType );

		//	var value = ReferenceUtils.CreateReference( underlyingType, resourceName, "" );

		//	List<UndoActionPropertiesChange.Item> undoItems = new List<UndoActionPropertiesChange.Item>();

		//	//!!!!try, catch? где еще

		//	foreach( var obj in objects )
		//	{
		//		//!!!!не обязательно Reference
		//		object oldValue = (IReference)property.GetValue( obj, indexers );

		//		//bool change = true;
		//		//if( /*skipSame && */oldValue != null && value.GetByReference == oldValue.GetByReference )
		//		//	change = false;

		//		if( !value.Equals( oldValue ) )
		//		{
		//			property.SetValue( obj, value, indexers );
		//			undoItems.Add( new UndoActionPropertiesChange.Item( obj, property, oldValue, indexers ) );
		//		}
		//	}

		//	//undo
		//	if( undoItems.Count != 0 )
		//	{
		//		var action = new UndoActionPropertiesChange( undoItems.ToArray() );
		//		document.UndoSystem.CommitAction( action );
		//		document.Modified = true;
		//	}
		//}

		internal static void RegisterEditorExtensions( Assembly assembly )
		{
			Type[] types = null;
			try
			{
				types = assembly.GetTypes();
			}
			catch
			{
				return;
			}

			foreach( var type in types )
			{
				if( typeof( EditorExtensions ).IsAssignableFrom( type ) && !type.IsAbstract )
				{
					var constructor = type.GetConstructor( new Type[ 0 ] );
					var obj = (EditorExtensions)constructor.Invoke( new object[ 0 ] );
					obj.Register();
				}
			}
		}

		//!!!!тут?
		public static DocumentInstance GetDocumentByComponent( Component component )
		{
			var parentRoot = component.ParentRoot;
			foreach( var document in EditorForm.Instance.Documents )
			{
				if( document.ResultComponent == parentRoot )
					return document;
			}
			return null;
		}

		public static void ShowRenameComponentDialog( Component component )
		{
			var document = GetDocumentByComponent( component );

			var form = new OKCancelTextBoxForm( EditorLocalization.TranslateLabel( "General", "Name:" ), component.Name, EditorLocalization.Translate( "General", "Rename" ),
				delegate ( string text, ref string error )
				{
					if( !ComponentUtility.IsValidComponentName( text, out string error2 ) )
					{
						error = error2;
						return false;
					}
					return true;
				},
				delegate ( string text, ref string error )
				{
					if( text != component.Name )
					{
						var oldValue = component.Name;

						//change Name
						component.Name = text;

						//undo
						var undoItems = new List<UndoActionPropertiesChange.Item>();
						var property = (Metadata.Property)MetadataManager.GetTypeOfNetType(
							typeof( Component ) ).MetadataGetMemberBySignature( "property:Name" );
						undoItems.Add( new UndoActionPropertiesChange.Item( component, property, oldValue, new object[ 0 ] ) );

						var action = new UndoActionPropertiesChange( undoItems.ToArray() );
						document.UndoSystem.CommitAction( action );
						document.Modified = true;
					}
					return true;
				}
			);

			form.ShowDialog();
		}

		static bool GetRenderingEffectDefaultOrderOfEffect( Metadata.TypeInfo type, out double value )
		{
			var attribs = type.GetCustomAttributes( typeof( Component_RenderingEffect.DefaultOrderOfEffectAttribute ), true );
			if( attribs.Length != 0 )
			{
				value = ( (Component_RenderingEffect.DefaultOrderOfEffectAttribute)attribs[ 0 ] ).Value;
				return true;
			}
			value = -1;
			return false;
		}

		//!!!!везде метод применить там где "CreateComponent("?
		//!!!!расширять метод
		public static int GetNewObjectInsertIndex( Component parent, Metadata.TypeInfo objectType )
		{
			//Rendering effects
			if( MetadataManager.GetTypeOfNetType( typeof( Component_RenderingEffect ) ).IsAssignableFrom( objectType ) )
			{
				if( GetRenderingEffectDefaultOrderOfEffect( objectType, out var value ) )
				{
					int index = 0;
					foreach( var child in parent.Components )
					{
						if( MetadataManager.GetTypeOfNetType( typeof( Component_RenderingEffect ) ).IsAssignableFrom( child.BaseType ) )
						{
							if( GetRenderingEffectDefaultOrderOfEffect( child.BaseType, out var childValue ) )
							{
								if( value < childValue )
									return index;
							}
						}
						index++;
					}
				}
			}

			return -1;
		}

		public static bool IsMemberVisible( Metadata.Member member )
		{
			if( member is Metadata.Property property )
				return property.Browsable && !property.HasIndexers && !property.Static /*!!!! && !property.ReadOnly */;
			else if( member is Metadata.Event evnt )
				return !evnt.Static;

			Log.Fatal( "internal error." );
			return true;
		}

		public static Type GetTypeByName( string typeName )
		{
			var typeName2 = typeName;
			//remove detailed info. need for EditorAttribute
			{
				var index = typeName2.IndexOf( "," );
				if( index != -1 )
					typeName2 = typeName2.Substring( 0, index );
			}

			//find in NeoAxis.Core.Editor.dll
			var type = EditorAssemblyInterface.Instance.GetTypeByName( typeName2 );
			if( type != null )
				return type;

			//find in public types
			var type2 = MetadataManager.GetType( typeName2 );
			if( type2 != null )
				return type2.GetNetType();

			//find in internal type
			var type3 = Assembly.GetExecutingAssembly().GetType( typeName2 );
			if( type3 != null )
				return type3;

			return null;
		}

		public static void ShowScreenNotificationObjectsCloned( int amount )
		{
			string text;
			if( amount == 1 )
				text = "The object was duplicated.";
			else
				text = "Objects were duplicated.";
			text = EditorLocalization.Translate( "General", text );

			ScreenNotifications.Show( text );
		}

		public static string GetUniqueFriendlyName( Component component, string namePrefix = "" )
		{
			string prefix;
			if( !string.IsNullOrEmpty( namePrefix ) )
				prefix = namePrefix;
			else
				prefix = component.BaseType.GetUserFriendlyNameForInstance();

			if( component.Parent.GetComponent( prefix ) == null )
				return prefix;
			return component.Parent.Components.GetUniqueName( prefix, false, 2 );
		}
	}
}
