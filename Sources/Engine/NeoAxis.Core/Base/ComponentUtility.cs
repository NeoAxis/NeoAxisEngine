// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// An auxiliary class for working with components.
	/// </summary>
	public static class ComponentUtility
	{
		public static Component CreateComponent( Metadata.TypeInfo type, object[] constructorParams, bool createHierarchyController,
			bool componentEnable )
		{
			var component = (Component)type.InvokeInstance( constructorParams );
			component.Enabled = componentEnable;

			if( createHierarchyController )
				CreateHierarchyControllerForRootComponent( component, null, true );//, true );

			return component;
		}

		public static Component CreateComponent( Type componentClass, object[] constructorParams, bool createHierarchyController, bool componentEnable )
		{
			return CreateComponent( MetadataManager.GetTypeOfNetType( componentClass ), constructorParams, createHierarchyController, componentEnable );
		}

		public static T CreateComponent<T>( object[] constructorParams, bool createHierarchyController, bool componentEnable ) where T : Component
		{
			return (T)CreateComponent( typeof( T ), constructorParams, createHierarchyController, componentEnable );
		}

		public static void CreateHierarchyControllerForRootComponent( Component rootComponent, Resource.Instance createdByResource,
			bool hierarchyEnabled )
		{
			if( rootComponent.HierarchyController != null )
				Log.Fatal( "ComponentManager: CreateHierarchyControllerForRootComponent: rootComponent.HierarchyController != null." );
			if( rootComponent.Parent != null )
				Log.Fatal( "ComponentManager: CreateHierarchyControllerForRootComponent: rootComponent.Parent != null." );

			var controller = new ComponentHierarchyController();
			controller.rootComponent = rootComponent;
			controller.createdByResource = createdByResource;
			rootComponent.hierarchyController = controller;

			controller.HierarchyEnabled = hierarchyEnabled;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public static bool _SaveToTextBlockInternal( Component component, Metadata.SaveContext context, TextBlock parentBlock, out bool skipSave, out string error )
		//{
		//	return component._SaveToTextBlock( context, parentBlock, out skipSave, out error );
		//}

		static bool LoadContext_InitComponentItemsRecursive( Metadata.LoadContext context, Metadata.LoadContext.ComponentItem componentItem, out string error )
		{
			context.AllComponentItemsCreationOrder.Add( componentItem );

			var block = componentItem.TextBlock;

			//load Name property
			if( block.AttributeExists( "Name" ) )
				componentItem.Name = block.GetAttribute( "Name" );

			//components
			List<Metadata.LoadContext.ComponentItem> children = new List<Metadata.LoadContext.ComponentItem>();
			foreach( TextBlock childBlock in block.Children )
			{
				if( childBlock.Name == ".component" )
				{
					//!!!!!!что тут с пересечениями

					if( string.IsNullOrEmpty( childBlock.Data ) )
					{
						error = "Invalid format. Component type is not specified.";
						return false;
					}

					Metadata.LoadContext.ComponentItem child = new Metadata.LoadContext.ComponentItem();
					child.Parent = componentItem;
					child.TextBlock = childBlock;
					if( !LoadContext_InitComponentItemsRecursive( context, child, out error ) )
						return false;

					children.Add( child );
				}
			}
			componentItem.Children = children.ToArray();

			context.AllComponentItemsSerializationOrder.Add( componentItem );

			error = "";
			return true;
		}

		static void LoadContext_ProcessComponentCreation( Metadata.LoadContext context, Metadata.LoadContext.ComponentItem item,
			out bool changed, out bool finished, ESet<string> unableToGetTypeName )
		{
			changed = false;

			if( item.Component == null && ( item.Parent != null && item.Parent.Component != null || item.Parent == null ) )
			{
				Metadata.TypeInfo type = MetadataManager.GetType( item.TextBlock.Data );
				if( type != null )
				{
					//use component by base type
					if( item.Parent != null && item.Parent.Component != null && item.TextBlock.AttributeExists( "CreatedByBaseType" ) )
					{
						bool createdByBaseType = bool.Parse( item.TextBlock.GetAttribute( "CreatedByBaseType" ) );
						if( createdByBaseType )
						{
							int hierarchyIndex = int.Parse( item.TextBlock.GetAttribute( "CreatedByBaseTypeHierarchyIndex", "0" ) );
							int nameIndex = int.Parse( item.TextBlock.GetAttribute( "CreatedByBaseTypeNameIndex", "0" ) );

							Component useComponent = null;

							foreach( var c in item.Parent.Component.Components )
							{
								if( c.Name == item.Name )
								{
									//!!!!return true проверять может таки
									c.GetBaseTypeIndex( out int baseHierarchyIndex, out string baseName, out int baseNameIndex );
									if( baseHierarchyIndex == hierarchyIndex && baseNameIndex == nameIndex )
									{
										useComponent = c;
										break;
									}
								}
							}

							if( useComponent != null )
								item.Component = useComponent;
						}
					}

					if( item.Component == null )
					{
						//create
						item.Component = (Component)type.InvokeInstance( null );

						//remove was deleted components created by base type
						{
							//!!!!slowly maybe

							//reverse order
							foreach( TextBlock childBlock in item.TextBlock.Children.GetReverse() )
							{
								if( childBlock.Name == ".componentCreatedByBaseTypeWasDeleted" )
								{
									int hierarchyIndex = int.Parse( childBlock.GetAttribute( "CreatedByBaseTypeHierarchyIndex", "0" ) );
									string name = childBlock.GetAttribute( "CreatedByBaseTypeName" );
									int nameIndex = int.Parse( childBlock.GetAttribute( "CreatedByBaseTypeNameIndex", "0" ) );

									Component foundComponent = null;
									foreach( var c in item.Component.Components )
									{
										if( c.Name == name )
										{
											//!!!!return true проверять может таки
											c.GetBaseTypeIndex( out int baseHierarchyIndex, out string baseName, out int baseNameIndex );
											if( baseHierarchyIndex == hierarchyIndex && baseNameIndex == nameIndex )
											{
												foundComponent = c;
												break;
											}
										}
									}

									if( foundComponent != null )
									{
										foundComponent.RemoveFromParent( false );
										foundComponent.Dispose();
									}
								}
							}

						}

						item.Component.providedTypeAllow = false;

						item.Component.Name = item.Name;

						//!!!!сортировать

						item.Parent.Component.AddComponent( item.Component );
					}

					changed = true;
				}
				else
					unableToGetTypeName.AddWithCheckAlreadyContained( item.TextBlock.Data );
			}

			finished = item.Component != null;
		}

		static void LoadContext_ProcessComponentsCreation( Metadata.LoadContext context, out bool changed, out bool finished, ESet<string> unableToGetTypeName )
		{
			changed = false;
			finished = true;

			foreach( var item in context.AllComponentItemsCreationOrder )
			{
				LoadContext_ProcessComponentCreation( context, item, out bool changed2, out bool finished2, unableToGetTypeName );
				if( changed2 )
					changed = true;
				if( !finished2 )
					finished = false;
			}
		}

		static bool LoadContext_IsAllChildrenLoaded( Metadata.LoadContext context, Metadata.LoadContext.ComponentItem item )
		{
			foreach( var child in item.Children )
			{
				if( !child.Loaded )
					return false;
			}
			return true;
		}

		static bool LoadContext_ProcessComponentSerialization( Metadata.LoadContext context, Metadata.LoadContext.ComponentItem item,
			out bool changed, out bool finished, out string error )
		{
			changed = false;

			if( item.Component != null && !item.Loaded && LoadContext_IsAllChildrenLoaded( context, item ) )
			{
				if( !item.Component._Load( context, item.TextBlock, out var error2 ) )
				{
					error = error2 + $" Component \'{item.Component.GetPathFromRoot( true )}\'.";
					finished = false;
					return false;
				}

				item.Component.providedTypeAllow = true;
				item.Loaded = true;
				changed = true;
			}

			finished = item.Loaded;
			error = "";
			return true;
		}

		static bool LoadContext_ProcessComponentsSerialization( Metadata.LoadContext context, out bool changed, out bool finished, out string error )
		{
			changed = false;
			finished = true;

			foreach( var item in context.AllComponentItemsSerializationOrder )
			{
				if( !LoadContext_ProcessComponentSerialization( context, item, out bool changed2, out bool finished2, out error ) )
					return false;
				if( changed2 )
					changed = true;
				if( !finished2 )
					finished = false;
			}

			error = "";
			return true;
		}

		static bool LoadContext_ProcessComponentItems( Metadata.LoadContext context, out string error )
		{
			//базовое поведение:
			//сначала вглубь создаем компоненты
			//далее загружаем свойства

			//если нельзя создать компоненту, то помечаем "есть дыра"
			//если есть дыра, то после первого обхода смысла запускать еще раз первый нет
			//нужно второй запускать (сериализацию)
			//она для всех не пройдет, т.к. есть дыры

			//если были изменения, то всё сначала

			//!!!!сортировать компоненты

			int noChangesCounter = 0;

			again:;

			var unableToGetTypeName = new ESet<string>();
			LoadContext_ProcessComponentsCreation( context, out var changed1, out var finished, unableToGetTypeName );
			if( !LoadContext_ProcessComponentsSerialization( context, out var changed2, out finished, out error ) )
				return false;

			//check can't load
			if( !changed1 && !changed2 )
			{
				noChangesCounter++;
				if( noChangesCounter == 10 )
				{
					//error

					if( unableToGetTypeName.Count != 0 )
					{
						error = "Types with next names are not exists:";
						//error = "Not all types are available. Types with next names are not available:";
						foreach( var name in unableToGetTypeName )
							error += "\r\n" + name;
					}
					else
						error = "Unknown error, no error info.";

					context.RootComponentItem?.Component?.Dispose();
					return false;
				}
			}
			else
				noChangesCounter = 0;

			if( !finished )
				goto again;

			if( !context.RootComponentItem.Loaded )
				Log.Fatal( "never happen" );

			error = "";
			return true;
		}

		//!!!!
		//static void LoadContext_ProcessNotCompiledComponentItems( Metadata.LoadContext context, out bool changed )
		//{
		//	changed = false;

		//	var items = context.GetNotCompiledItems();
		//	if( items.Count != 0 )
		//	{
		//		//creation stage
		//		foreach( var item in items )
		//		{
		//			//try create component
		//			if( item.component == null && item.parent != null && item.parent.component != null )
		//			{
		//				//!!!!
		//				xx xx;
		//				//if( componentBlock.Data == "_Dev\\Import3D\\ColladaTest2.dae:Originals\\Materials\\ColladaTestStoneStatic" )
		//				//	Log.Warning( "dfgdfg" );

		//				xx xx;

		//				bool readyOnly = xx;

		//				Metadata.TypeInfo type = MetadataManager.GetType( item.textBlock.Data );

		//				xx xx;//загружен тип должен быть

		//				if( type == null )
		//				{
		//					xx xx;

		//					//!!!!
		//					Log.Fatal( "impl" );
		//				}

		//				xx xx;//а также проверять на loaded. клонироваться же


		//				item.component = (Component)type.InvokeInstance( null );
		//				item.component.providedTypeAllow = false;

		//				//!!!!!!так?
		//				item.parent.component.AddComponent( item.component );

		//				//!!!!
		//				//string error3;
		//				//if( !component._LoadHierarchy( context, componentBlock, out error3 ) )
		//				//{
		//				//	//!!!!
		//				//	Log.Fatal( "impl" );

		//				//	//!!!!!?
		//				//	//RemoveComponent
		//				//	//component = null;
		//				//}

		//				changed = true;
		//			}
		//		}

		//		//if changed on creation stage then reply creation stage
		//		if( changed )
		//			return;

		//		//loading properties stage
		//		{
		//			xx xx;

		//		}

		//		xx xx;

		//		xx xx;
		//		//changed = true;
		//	}
		//}

		//static void LoadContext_ProcessComponentItemsRecursive( Metadata.LoadContext.ComponentItem componentItem )
		//{
		//}

		//public static bool LoadComponentFromTextBlock( Component component, Metadata.LoadContext overrideContextObject, TextBlock block, string loadedFromFile, out string error )
		//{
		//	xx xx;

		//	xx xx;

		//	loadedFromFile = VirtualPathUtils.NormalizePath( loadedFromFile );

		//	Metadata.LoadContext context = overrideContextObject;
		//	if( context == null )
		//		context = new Metadata.LoadContext();
		//	context.virtualFileName = loadedFromFile;

		//	error = "";

		//	if( block.Children.Count != 1 )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	TextBlock componentBlock = block.Children[ 0 ];
		//	if( componentBlock.Name != ".component" )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	if( string.IsNullOrEmpty( componentBlock.Data ) )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	//get type
		//	Metadata.TypeInfo type = MetadataManager.GetType( componentBlock.Data );
		//	if( type == null )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	xx xx;
		//	//create instance
		//	var component = (Component)type.InvokeInstance( null );

		//	//create context, init tree structure of components
		//	context.rootComponentItem = new Metadata.LoadContext.ComponentItem();
		//	context.rootComponentItem.textBlock = componentBlock;
		//	LoadContext_InitComponentItemsRecursive( context, context.rootComponentItem );

		//	//processing
		//	context.rootComponentItem.component = component;
		//	LoadContext_ProcessComponentItems( context );

		//	if( !context.rootComponentItem.loaded )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	return true;
		//}

		public static Component LoadComponentFromTextBlock( Metadata.LoadContext overrideContextObject, TextBlock block, string loadedFromFile,
			Resource.Instance createdByResource, bool? componentSetEnabled, bool componentCreateHierarchyController, out string error )
		{
			loadedFromFile = VirtualPathUtility.NormalizePath( loadedFromFile );

			Metadata.LoadContext context = overrideContextObject;
			if( context == null )
				context = new Metadata.LoadContext();
			context.VirtualFileName = loadedFromFile;

			error = "";

			if( block.Children.Count != 1 )
			{
				error = "Invalid format. block.Children.Count != 1";
				return null;
			}

			TextBlock componentBlock = block.Children[ 0 ];
			if( componentBlock.Name != ".component" )
			{
				error = "Invalid format. componentBlock.Name != \".component\"";
				return null;
			}

			if( string.IsNullOrEmpty( componentBlock.Data ) )
			{
				error = "Invalid format. string.IsNullOrEmpty( componentBlock.Data )";
				return null;
			}

			//get type
			Metadata.TypeInfo type = MetadataManager.GetType( componentBlock.Data );
			if( type == null )
			{
				error = $"Type with name \'{componentBlock.Data}\' is not exists.";
				return null;
			}

			//create instance
			var component = (Component)type.InvokeInstance( null );

			//set ResultObject for resource
			if( createdByResource != null )
				createdByResource.ResultObject = component;

			//create hierarchy controller. disabled at creation.
			if( componentCreateHierarchyController )
				CreateHierarchyControllerForRootComponent( component, createdByResource, false );//, true );

			//create context, init tree structure of components
			context.RootComponentItem = new Metadata.LoadContext.ComponentItem();
			context.RootComponentItem.TextBlock = componentBlock;

			if( !LoadContext_InitComponentItemsRecursive( context, context.RootComponentItem, out error ) )
			{
				component?.Dispose();
				return null;
			}

			//processing
			context.RootComponentItem.Component = component;
			if( !LoadContext_ProcessComponentItems( context, out error ) )
			{
				component?.Dispose();
				return null;
			}

			if( !context.RootComponentItem.Loaded )
			{
				error = "Root component is not loaded.";
				component?.Dispose();
				return null;
			}

			//set Enabled
			if( componentSetEnabled != null )
				component.Enabled = componentSetEnabled.Value;
			//enable hierarchy controller
			if( component.HierarchyController != null )
				component.HierarchyController.HierarchyEnabled = true;

			return component;
		}

		//static Component LoadComponentFromTextBlockInternal( Metadata.LoadContext overrideContextObject, TextBlock block, string loadedFromFile,
		//Resource.Instance createdByResource, bool? componentSetEnabled, bool componentCreateHierarchyController, out string error )

		//public static Component LoadComponentFromTextBlock( Resource.Instance createdByResource, Metadata.LoadContext overrideContextObject,
		//	bool? componentSetEnabled, bool componentCreateHierarchyController, out string error )
		//{
		//	var block = createdByResource.Owner.LoadedBlock;
		//	var loadedFromFile = createdByResource.Owner.Name;
		//	return LoadComponentFromTextBlockInternal( createdByResource, block, loadedFromFile, overrideContextObject, componentSetEnabled,
		//		componentCreateHierarchyController, out error );
		//}

		//public static Component LoadComponentFromTextBlock( TextBlock block, string loadedFromFile, Metadata.LoadContext overrideContextObject,
		//	bool? componentSetEnabled, bool componentCreateHierarchyController, out string error )
		//{
		//	return LoadComponentFromTextBlockInternal( null, block, loadedFromFile, overrideContextObject, componentSetEnabled,
		//		componentCreateHierarchyController, out error );
		//}

		//public static Component LoadComponentFromTextBlockXX( Resource.Instance ins, Metadata.LoadContext overrideContextObject,
		//bool? componentSetEnabled, bool componentCreateHierarchyController, out string error )
		////public static Component LoadComponentFromTextBlock( TextBlock block, string loadedFromFile, Metadata.LoadContext overrideContextObject,
		////	out string error )
		//{
		//	var block = ins.Owner.LoadedBlock;
		//	var loadedFromFile = ins.Owner.Name;

		//	loadedFromFile = VirtualPathUtils.NormalizePath( loadedFromFile );

		//	Metadata.LoadContext context = overrideContextObject;
		//	if( context == null )
		//		context = new Metadata.LoadContext();
		//	context.virtualFileName = loadedFromFile;

		//	error = "";

		//	if( block.Children.Count != 1 )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	TextBlock componentBlock = block.Children[ 0 ];
		//	if( componentBlock.Name != ".component" )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	if( string.IsNullOrEmpty( componentBlock.Data ) )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	Metadata.TypeInfo type = MetadataManager.GetType( componentBlock.Data );
		//	if( type == null )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	var component = (Component)type.InvokeInstance( null );

		//	xx xx;
		//	//set ResultObject for resource instance
		//	ins.ResultObject = component;

		//	//create disabled hierarchy controller
		//	if( componentCreateHierarchyController )
		//		ComponentUtils.CreateHierarchyControllerForRootComponent( component, ins, false );//, true );

		//	//create context, init tree structure of components
		//	context.rootComponentItem = new Metadata.LoadContext.ComponentItem();
		//	context.rootComponentItem.textBlock = componentBlock;
		//	LoadContext_InitComponentItemsRecursive( context.rootComponentItem );


		//	xx xx;
		//	//processing
		//	bool changed;
		//	do
		//	{
		//		LoadContext_ProcessNotCompiledComponentItems( context, out changed );
		//	} while( changed );

		//	if( !context.rootComponentItem.loaded )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	xx xx;//сортировать компоненты где-то



		//	if( componentSetEnabled != null )
		//		component.Enabled = componentSetEnabled.Value;

		//	//enable hierarchy controller
		//	if( component.HierarchyController != null )
		//		component.HierarchyController.HierarchyEnabled = true;


		//	xx xx;//всё в этом методе




		//	//!!!!_LoadComponentFromTextBlockInternal

		//	string error4;
		//	if( !component._LoadHierarchy( context, componentBlock, out error4 ) )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );

		//		//!!!!!?
		//		component = null;
		//	}

		//	xx xx;

		//	string error3;
		//	if( !component._LoadFromTextBlock( context, componentBlock, out error3 ) )
		//	{
		//		//!!!!
		//		Log.Fatal( "impl" );

		//		//!!!!!?
		//		component = null;
		//	}

		//	return component;
		//}

		//!!!!такой метод полезен или открыть Component._Save
		//public static bool SaveComponentToTextBlock( Component component, string realFileName, Component.SaveContext overrideContextObject,
		//	out string error )
		//{
		//}

		public static bool SaveComponentToFile( Component component, string realFileName, Metadata.SaveContext overrideContextObject, out string error )
		{
			realFileName = VirtualPathUtility.NormalizePath( realFileName );

			Metadata.SaveContext context = overrideContextObject;
			if( context == null )
				context = new Metadata.SaveContext();
			//!!!!strange overwrite
			context.RealFileName = realFileName;

			component.HierarchyController?.ProcessDelayedOperations();

			//!!!!было
			//map.fileName = virtualFileName;

			string realDirectoryName = Path.GetDirectoryName( realFileName );
			if( !Directory.Exists( realDirectoryName ) )
			{
				try
				{
					Directory.CreateDirectory( realDirectoryName );
				}
				catch
				{
					error = string.Format( "Unable to create directory \"{0}\".", realDirectoryName );
					return false;
				}
			}

			TextBlock rootBlock = new TextBlock();
			//!!!!!".createdBy"
			//mapBlock.SetAttribute( ".engineVersion", EngineVersionInformation.Version );

			//_SaveToTextBlockInternal

			component._SaveToTextBlock( context, rootBlock, true, out _, out error );
			if( !string.IsNullOrEmpty( error ) )
				return false;

			try
			{
				using( StreamWriter writer = new StreamWriter( realFileName ) )
				{
					writer.Write( $"// Made with {EngineInfo.NameWithVersion}.\r\n\r\n" );
					writer.Write( rootBlock.DumpToString() );
				}
			}
			catch
			{
				error = string.Format( "Unable to save file \"{0}\".", realFileName );
				return false;
			}

			//Map.Instance.savedRealFileDirectory = Map.Instance.GetRealFileDirectory();

			error = "";
			return true;
		}

		public static TextBlock SaveComponentToTextBlock( Component component, Metadata.SaveContext overrideContextObject, out string error )
		{
			Metadata.SaveContext context = overrideContextObject;
			if( context == null )
				context = new Metadata.SaveContext();

			component.HierarchyController?.ProcessDelayedOperations();

			var rootBlock = new TextBlock();

			component._SaveToTextBlock( context, rootBlock, true, out _, out error );
			if( !string.IsNullOrEmpty( error ) )
				return null;

			error = "";
			return rootBlock;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public delegate void OverrideCreationOfComponentDelegate( string creationInfo, ref Component component, ref string error );
		//public static event OverrideCreationOfComponentDelegate OverrideCreationOfComponent;

		//public class Class
		//{
		//	public Class[] GetAllClassHierarchy()
		//	{
		//		List<Class> list = new List<Class>();

		//		Class item = this;
		//		do
		//		{
		//			list.Add( item );
		//			if( item.classType.BaseType != null )
		//				item = GetClass( item.classType.BaseType );
		//		} while( item != null );

		//		list.Reverse();
		//		return list.ToArray();
		//	}
		//}

		//public static Component CreateComponentByReferenceTypeOrClassName( string typeName, bool instanceCreation )
		//{
		//	//OverrideCreationOfComponent
		//	{
		//		Component c = null;
		//		string error = "";
		//		OverrideCreationOfComponent?.Invoke( typeName, ref c, ref error );
		//		if( !string.IsNullOrEmpty( error ) )
		//		{
		//			//!!!!!
		//			Log.Fatal( "impl" );
		//		}
		//		if( c != null )
		//			return c;
		//	}

		//	return null;
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!optimization. может как-то более по объектно подписываться

		//!!!!а может и не надо эти все AllComponents_

		//public static event Action<Component> AllComponents_NameChanged;
		//public static event Action<Component> AllComponents_DisplayNameChanged;

		//internal static void PerformAllComponents_NameChanged( Component obj )
		//{
		//	AllComponents_NameChanged?.Invoke( obj );
		//}

		//internal static void PerformAllComponents_DisplayNameChanged( Component obj )
		//{
		//	AllComponents_DisplayNameChanged?.Invoke( obj );
		//}

		//!!!!name
		[MethodImpl( (MethodImplOptions)512 )]
		public static string GetOwnedFileNameOfComponent( Component component )
		{
			var root = component.ParentRoot;
			if( root != null && root.HierarchyController != null && root.HierarchyController.CreatedByResource != null )
				return root.HierarchyController.CreatedByResource.Owner.Name;
			return "";
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public static Component FindNearestCommonParent( IList<Component> components )
		{
			if( components.Count == 0 )
				return null;
			if( components.Count == 1 )
				return components[ 0 ].Parent;

			var parents = new ICollection<Component>[ components.Count ];
			for( int n = 0; n < parents.Length; n++ )
				parents[ n ] = components[ n ].GetAllParents( false );

			foreach( var parent in parents[ 0 ] )
			{
				bool allContains = true;

				for( int n = 1; n < parents.Length; n++ )
				{
					var list = parents[ n ];
					if( !list.Contains( parent ) )
					{
						allContains = false;
						break;
					}
				}

				if( allContains )
					return parent;
			}

			return null;

			//var parent = components[ 0 ].Parent;
			//while( parent != null )
			//{
			//	bool allSame = true;
			//	for( int n = 1; n < components.Count; n++ )
			//	{
			//		if( components[ n ].Parent != parent )
			//		{
			//			allSame = false;
			//			break;
			//		}
			//	}
			//	if( allSame )
			//		return parent;

			//	parent = parent.Parent;
			//}
			//return null;
		}

		//!!!!проверять при изменении свойства Name
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool IsValidComponentName( string name, out string error )
		{
			//!!!!что-то еще?

			if( name.Length != 0 && name[ 0 ] == '$' )
			{
				error = "The name cannot begin from \'$\'.";
				return false;
			}
			error = "";
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool IsChildInHierarchy( Component parent, Component child )
		{
			var current = child.Parent;
			while( current != null )
			{
				if( current == parent )
					return true;
				current = current.Parent;
			}
			return false;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static int HowDeepChildInHierarchy( Component parent, Component child )
		{
			int level = 0;

			var current = child;
			do
			{
				if( current == parent )
					return level;
				current = current.Parent;
				level++;
			} while( current != null );

			return -1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Resource.Instance GetResourceInstanceByComponent( Component component )
		{
			return component.ParentRoot?.HierarchyController?.CreatedByResource;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public static string GetNewObjectUniqueName( Component newObject )
		{
			var name = newObject.BaseType.GetUserFriendlyNameForInstance( true );

			//get unique name
			if( newObject.Parent != null && newObject.Parent.GetComponent( name ) != null )
			{
				for( int n = 2; ; n++ )
				{
					string newName = name + " " + n.ToString();
					if( newObject.Parent.GetComponent( newName ) == null )
					{
						name = newName;
						break;
					}
				}
			}

			return name;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool TypeSettingsPrivateObjectsContains( string[] typeSettingsPrivateObjects, Component component )
		{
			if( typeSettingsPrivateObjects != null )
			{
				var findName = component.GetPathFromParent();
				foreach( var name in typeSettingsPrivateObjects )
					if( name == findName )
						return true;
			}
			return false;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool TypeSettingsPrivateObjectsContains( string[] typeSettingsPrivateObjects, Metadata.Member member )
		{
			if( typeSettingsPrivateObjects != null )
			{
				foreach( var name in typeSettingsPrivateObjects )
					if( name == member.Name )
						return true;
			}
			return false;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public static List<Component> GetComponentsWithoutChildren( ICollection<Component> list )
		{
			var set = new ESet<Component>( list.Count );
			set.AddRangeWithCheckAlreadyContained( list );

			var newList = new List<Component>( list.Count );

			foreach( var obj in list )
			{
				var allParents = obj.GetAllParents( false );

				if( !allParents.Any( p => set.Contains( p ) ) )
					newList.Add( obj );
			}

			return newList;
		}

	}
}
