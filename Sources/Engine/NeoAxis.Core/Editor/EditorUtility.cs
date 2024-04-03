// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace NeoAxis.Editor
{
	public static class EditorUtility
	{
		internal static Metadata.GetMembersContext getMemberContextNoFilter = new Metadata.GetMembersContext( false );

#if !DEPLOY
		public static bool AllowConfigureComponentTypeSettings = true;
		public static bool AllowSeparateSettings = true;

		//

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

		public static void SetPropertyReference( IDocumentInstance document, object/* Component*/[] objects, Metadata.Property property, object[] indexers, string[] referenceValues )
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

		static bool GetRenderingEffectDefaultOrderOfEffect( Metadata.TypeInfo type, out double value )
		{
			var attribs = type.GetCustomAttributes( typeof( RenderingEffect.DefaultOrderOfEffectAttribute ), true );
			if( attribs.Length != 0 )
			{
				value = ( (RenderingEffect.DefaultOrderOfEffectAttribute)attribs[ 0 ] ).Value;
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
			if( MetadataManager.GetTypeOfNetType( typeof( RenderingEffect ) ).IsAssignableFrom( objectType ) )
			{
				if( GetRenderingEffectDefaultOrderOfEffect( objectType, out var value ) )
				{
					int index = 0;
					foreach( var child in parent.Components )
					{
						if( MetadataManager.GetTypeOfNetType( typeof( RenderingEffect ) ).IsAssignableFrom( child.BaseType ) )
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

		public delegate void IsMemberVisibleOverrideDelegate( object obj, Metadata.Member member, ref bool visible );
		public static event IsMemberVisibleOverrideDelegate IsMemberVisibleOverride;

		public static bool IsMemberVisible( object obj, Metadata.Member member )
		{
			bool result = true;

			if( member is Metadata.Property property )
				result = property.Browsable && !property.HasIndexers && !property.Static /*!!!! && !property.ReadOnly */;
			else if( member is Metadata.Event evnt )
				result = !evnt.Static;
			else
				Log.Fatal( "internal error." );

			IsMemberVisibleOverride?.Invoke( obj, member, ref result );

			return result;
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

		///////////////////////////////////////////////

		public delegate void ComponentDisplayInEditorFilterDelegate( Component obj, ref bool display );
		public static event ComponentDisplayInEditorFilterDelegate ComponentDisplayInEditorFilter;

		public static bool PerformComponentDisplayInEditorFilter( Component obj )
		{
			var result = true;
			ComponentDisplayInEditorFilter?.Invoke( obj, ref result );
			return result;
		}

		///////////////////////////////////////////////

		public delegate void RibbonTabVisibleFilterDelegate( IEditorRibbonDefaultConfigurationTab tab, ref bool visible );
		public static event RibbonTabVisibleFilterDelegate RibbonTabVisibleFilter;

		public static bool PerformRibbonTabVisibleFilter( IEditorRibbonDefaultConfigurationTab tab )
		{
			var result = true;
			RibbonTabVisibleFilter?.Invoke( tab, ref result );
			return result;
		}

		///////////////////////////////////////////////

		public delegate void EditorActionVisibleFilterDelegate( IEditorAction action, ref bool visible );
		public static event EditorActionVisibleFilterDelegate EditorActionVisibleFilter;

		public static bool PerformEditorActionVisibleFilter( IEditorAction action )
		{
			var result = true;
			EditorActionVisibleFilter?.Invoke( action, ref result );
			return result;
		}
#endif
	}
}
