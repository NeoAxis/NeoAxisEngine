// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Class for working with engine resources.
	/// </summary>
	public static class ResourceManager
	{
		static EDictionary<string, ResourceType> types = new EDictionary<string, ResourceType>();
		//Key: ToLower(), without dot
		static Dictionary<string, ResourceType> typeByFileExtension = new Dictionary<string, ResourceType>();

		//key: normalized path + lower case
		internal static EDictionary<string, Resource> resources = new EDictionary<string, Resource>();

		internal static object lockObject = new object();

		//!!!!!research
		//public enum LoadingWay
		//{
		//	RightNow,
		//	ReadyWhenUse,
		//	InBackground,
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a type of a resource.
		/// </summary>
		public class ResourceType
		{
			internal string name;
			internal ESet<string> fileExtensions = new ESet<string>();
			internal Type resourceClass;

			//!!!!!только виртуальные файловые имена?
			//!!!!!multithreading
			//public delegate Resource LoadResourceFunctionDelegate( string virtualFileName, bool loadInBackground, ref string error );
			//internal LoadResourceFunctionDelegate loadResourceFunction;

			public string Name
			{
				get { return name; }
			}

			public ICollection<string> FileExtensions
			{
				get { return fileExtensions.AsReadOnly(); }
			}

			public Type ResourceClass
			{
				get { return resourceClass; }
			}

			public void AddExtension( string extension )
			{
				fileExtensions.Add( extension );

				typeByFileExtension[ extension.ToLower() ] = this;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		internal static void Init()
		{
			RegisterStandardTypes();
		}

		internal static void Shutdown()
		{
			DisposeAllResources();

			types.Clear();
			typeByFileExtension.Clear();
			resources.Clear();
		}

		internal static string GetKey( string name )
		{
			return VirtualPathUtility.NormalizePath( name.ToLower() );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static Resource.Instance CreateResource( ResourceType resourceType, Resource.InstanceType instanceType, string name, bool wait,
			Metadata.TypeInfo createResultObjectWithType, bool componentCreateHierarchyController, bool? componentSetEnabled, out string error )
		{
			//!!!!
			if( !wait )
				Log.Fatal( "ResourceManager: CreateResource: wait == false. Background loading is not implemented." );

			error = "";

			//!!!!!threading

			if( string.IsNullOrEmpty( name ) )
			{
				Log.Fatal( "ResourceManager: CreateResource: The name can't be empty." );
				return null;
			}

			Resource res;
			Resource.Instance ins;
			bool insCreated = false;

			lock( lockObject )
			{
				//get or create resource
				res = GetByName( name );
				if( res == null )
				{
					//!!!!override loader

					ConstructorInfo constructor = resourceType.resourceClass.GetConstructor( new Type[ 0 ] );
					res = (Resource)constructor.Invoke( new object[ 0 ] );
					res.resourceType = resourceType;
					res.name = name;
					res.loadFromFile = true;

					//add to the list
					resources.Add( GetKey( name ), res );
				}

				//create instance
				if( instanceType == Resource.InstanceType.Resource )
				{
					//as Resource
					if( res.PrimaryInstance == null )
					{
						ins = res.CreateInstanceClassObject( instanceType, componentCreateHierarchyController, componentSetEnabled );
						res.PrimaryInstance = ins;
						insCreated = true;
					}
					else
						ins = res.PrimaryInstance;
				}
				else
				{
					//as Separate instance
					ins = res.CreateInstanceClassObject( instanceType, componentCreateHierarchyController, componentSetEnabled );
					insCreated = true;
				}
			}

			if( insCreated )
			{
				if( createResultObjectWithType != null )
				{
					object obj = createResultObjectWithType.InvokeInstance( null );

					var component = obj as Component;
					if( component != null )
					{
						if( componentSetEnabled != null )
							component.Enabled = componentSetEnabled.Value;
						if( componentCreateHierarchyController )
							ComponentUtility.CreateHierarchyControllerForRootComponent( component, ins, true );//, true );
					}

					ins.ResultObject = obj;
				}

				//resource is ready
				ins.Status = Resource.Instance.StatusEnum.Ready;
			}
			////_Load function
			//if( insCreated )
			//{
			//	//load

			//	if( wait )
			//	{
			//		LoadTaskFunction( ins );
			//	}
			//	else
			//	{
			//		Task task = new Task( LoadTaskFunction, ins );
			//		task.Start();
			//	}
			//}

			//wait
			if( wait )
			{
				//wait end of creation
				while( ins.Status == Resource.Instance.StatusEnum.CreationProcess )
				{
					//!!!!slow? maybe we can increase priority for this instance or something like this?

					if( VirtualFileSystem.MainThread == Thread.CurrentThread )
					{
						//process main thread tasks
						EngineThreading.ExecuteQueuedActionsFromMainThread();
					}

					//!!!!?
					Thread.Sleep( 0 );
				}
			}

			//!!!!так?
			if( wait && ins.Status == Resource.Instance.StatusEnum.Error )
			{
				error = ins.StatusError;
				ins.Dispose();
				return null;
			}

			return ins;
		}

		//!!!!!
		//public static Resource Create( string name, bool makeUniqueName, Type resourceClass, object[] constructorParams = null )
		//{
		//	if( constructorParams == null )
		//		constructorParams = new object[ 0 ];

		//	Resource res;

		//	lock( lockObject )
		//	{
		//		if( makeUniqueName )
		//		{
		//			//!!!!так? первый без цифры?

		//			if( GetByName( name ) != null )
		//			{
		//				string newName = name;
		//				do
		//				{
		//					newName = uniqueNameGenerator.Get( name );
		//				}
		//				while( GetByName( newName ) != null );

		//				name = newName;
		//			}
		//		}
		//		else
		//		{
		//			if( GetByName( name ) != null )
		//				Log.Fatal( "ResourceManager: Create: GetByName( name ) != null. Resource name: \"{0}\".", name );
		//		}

		//		ConstructorInfo constructor = ReflectionUtils.GetSuitableConstructor( resourceClass, constructorParams, true );
		//		res = (Resource)constructor.Invoke( constructorParams );
		//		res.name = name;

		//		//add to the list
		//		resources.Add( GetKey( name ), res );

		//		//object result = null;
		//		//if( typeof( Resource ).IsAssignableFrom( resourceClass ) )
		//		//{
		//		//	ConstructorInfo constructor = GetSuitableConstructor( resourceClass, constructorParams );
		//		//	ins = (Resource)constructor.Invoke( constructorParams );

		//		//	ins.name = name;
		//		//	AddInstance( ins );

		//		//	result = ins;
		//		//}
		//		//else
		//		//{
		//		//	ins = new Resource();

		//		//	ins.name = name;
		//		//	AddInstance( ins );

		//		//	ConstructorInfo constructor = GetSuitableConstructor( resourceClass, constructorParams );
		//		//	ins.ResultObject = constructor.Invoke( constructorParams );
		//		//}

		//		//ins.status = Resource.StatusEnum.Ready;

		//		//ins.OnCreate_SameThread();
		//		//if( i.OnCreateEnd() )
		//		//	i.status = ResourceInstance.ResourceStatus.Ready;
		//		//else
		//		//	i.status = ResourceInstance.ResourceStatus.Error;
		//	}

		//	return res;
		//}

		//!!!!!?
		//public static T Create<T>( string name, bool makeUniqueName, object[] constructorParams = null ) where T : Resource
		//{
		//	return (T)Create( name, makeUniqueName, typeof( T ), constructorParams );
		//}

		//!!!!!везде повтыкать где как юзается loadInBackground

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		static void LoadTaskFunction( object data )
		{
			Resource.Instance ins = (Resource.Instance)data;
			ins.PerformLoad();
		}

		public static Resource.Instance _Load( Resource.InstanceType instanceType, string name, bool wait,
			bool componentCreateHierarchyController, bool? componentSetEnabled, out string error )
		{
			error = "";

			//!!!!!threading

			if( string.IsNullOrEmpty( name ) )
			{
				error = "";
				return null;
			}

			//!!!!!на неналичие файла проверять. но не в локе?

			Resource res;
			Resource.Instance ins;
			bool insCreated = false;

			lock( lockObject )
			{
				//get or create resource
				res = GetByName( name );
				if( res == null )
				{
					//!!!!override loader

					var extension = Path.GetExtension( name );
					var resourceType = GetTypeByFileExtension( extension );
					if( resourceType == null )
					{
						//!!!!всё же как-то можно грузить?
						error = $"Loading of file type \'{extension}\' is not supported.";
						//error = "";
						return null;
					}

					ConstructorInfo constructor = resourceType.resourceClass.GetConstructor( new Type[ 0 ] );
					res = (Resource)constructor.Invoke( new object[ 0 ] );
					res.resourceType = resourceType;
					res.name = name;
					res.loadFromFile = true;

					//add to the list
					resources.Add( GetKey( name ), res );
				}

				//create instance
				if( instanceType == Resource.InstanceType.Resource )
				{
					//as Resource
					if( res.PrimaryInstance == null )
					{
						ins = res.CreateInstanceClassObject( instanceType, componentCreateHierarchyController, componentSetEnabled );
						res.PrimaryInstance = ins;
						insCreated = true;
					}
					else
						ins = res.PrimaryInstance;
				}
				else
				{
					//as Separate instance
					ins = res.CreateInstanceClassObject( instanceType, componentCreateHierarchyController, componentSetEnabled );
					insCreated = true;
				}
			}

			//begin loading
			if( insCreated )
			{
				if( wait )
				{
					LoadTaskFunction( ins );
				}
				else
				{
					Task task = new Task( LoadTaskFunction, ins );
					task.Start();
				}
			}

			//!!!!new
			//!!!!только InstanceType.Resource?
			//if wait == true and inside call ResourceManager._Load recursively with wait == true, then return null.
			if( wait && instanceType == Resource.InstanceType.Resource && !insCreated && ins.Status == Resource.Instance.StatusEnum.CreationProcess )
			{
				//так норм импорт работает, хотя это не совсем логично.
				return ins;
				//return null;

				//xx;
				//ESet<Resource.Instance> waitLoadingResources = new ESet<Resource.Instance>();
				//if( waitLoadingResources.Contains( ins ) )
				//	return null;
				//waitLoadingResources.Add( ins );
			}

			//wait
			if( wait )
			{
				//wait end of creation
				while( ins.Status == Resource.Instance.StatusEnum.CreationProcess )
				{
					//!!!!slow? maybe we can increase priority for this instance or something like this?

					if( VirtualFileSystem.MainThread == Thread.CurrentThread )
					{
						//process main thread tasks
						EngineThreading.ExecuteQueuedActionsFromMainThread();
					}

					//!!!!?
					Thread.Sleep( 0 );
				}
			}

			//!!!!так?
			if( wait && ins.Status == Resource.Instance.StatusEnum.Error )
			{
				error = ins.StatusError;
				ins.Dispose();
				return null;
			}

			return ins;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static Resource.Instance LoadResource( string name, bool wait, out string error )
		{
			return _Load( Resource.InstanceType.Resource, name, wait, true, null, out error );
		}

		public static Resource.Instance LoadResource( string name, bool wait )
		{
			string error;
			var ins = LoadResource( name, wait, out error );
			if( !string.IsNullOrEmpty( error ) )
			{
				var error2 = $"Unable to load resource \'{name}\'.\r\n\r\n" + error;
				Log.Error( error2 );
				return null;
			}
			else
				return ins;
		}

		public static T LoadResource<T>( string name, out string error ) where T : class
		{
			var ins = LoadResource( name, true, out error );
			if( ins == null )
				return null;
			//!!!!если не тот формат то тоже ошибка?
			return ins.ResultObject as T;
		}

		public static T LoadResource<T>( string name ) where T : class
		{
			string error;
			var c = LoadResource<T>( name, out error );
			if( !string.IsNullOrEmpty( error ) )
			{
				var error2 = $"Unable to load resource \'{name}\'.\r\n\r\n" + error;
				Log.Error( error2 );
				return null;
			}
			else
				return c;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static Resource.Instance LoadSeparateInstance( string name, bool wait, bool componentCreateHierarchyController, bool? componentSetEnabled,
			out string error )
		{
			return _Load( Resource.InstanceType.SeparateInstance, name, wait, componentCreateHierarchyController, componentSetEnabled, out error );
		}

		public static Resource.Instance LoadSeparateInstance( string name, bool wait, bool componentCreateHierarchyController, bool? componentSetEnabled )
		{
			string error;
			var ins = LoadSeparateInstance( name, wait, componentCreateHierarchyController, componentSetEnabled, out error );
			if( !string.IsNullOrEmpty( error ) )
			{
				var error2 = $"Unable to load resource \'{name}\'.\r\n\r\n" + error;
				Log.Error( error2 );
				return null;
			}
			else
				return ins;
		}

		public static T LoadSeparateInstance<T>( string name, bool componentCreateHierarchyController, bool? componentSetEnabled, out string error )
			where T : class
		{
			var ins = LoadSeparateInstance( name, true, componentCreateHierarchyController, componentSetEnabled, out error );
			if( ins == null )
				return null;
			//!!!!если не тот формат то тоже ошибка?
			return ins.ResultObject as T;
		}

		public static T LoadSeparateInstance<T>( string name, bool componentCreateHierarchyController, bool? componentSetEnabled ) where T : class
		{
			string error;
			var c = LoadSeparateInstance<T>( name, componentCreateHierarchyController, componentSetEnabled, out error );
			if( !string.IsNullOrEmpty( error ) )
			{
				var error2 = $"Unable to load resource \'{name}\'.\r\n\r\n" + error;
				Log.Error( error2 );
				return null;
			}
			else
				return c;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		internal static void RemoveInternal( Resource resource )
		{
			lock( lockObject )
				resources.Remove( GetKey( resource.name ) );
		}

		public static Resource GetByName( string name )
		{
			lock( lockObject )
			{
				Resource r;
				resources.TryGetValue( GetKey( name ), out r );
				return r;
			}
		}

		public static Resource[] GetAllResources()
		{
			lock( lockObject )
			{
				Resource[] array = new Resource[ resources.Count ];
				resources.Values.CopyTo( array, 0 );
				return array;
			}
		}

		public static ICollection<ResourceType> Types
		{
			get { return types.Values; }
		}

		public static ResourceType GetTypeByName( string name )
		{
			ResourceType t;
			types.TryGetValue( name, out t );
			return t;
		}

		public static ResourceType GetTypeByFileExtension( string extension )
		{
			string key = extension;
			if( key != "" && key[ 0 ] == '.' )
				key = key.Substring( 1 );
			key = key.ToLower();

			ResourceType type;
			typeByFileExtension.TryGetValue( key, out type );
			return type;
		}

		public static ResourceType RegisterType( string name, IEnumerable<string> fileExtensions, Type resourceClass )
		{
			if( GetTypeByName( name ) != null )
				Log.Fatal( $"ResourceManager: RegisterResourceType: The resource with name {name} is already registered." );

			ResourceType type = new ResourceType();
			type.name = name;
			type.resourceClass = resourceClass;
			//type.loadResourceFunction = loadFunction;
			types.Add( name, type );
			foreach( var v in fileExtensions )
				type.AddExtension( v );

			return type;
		}

		static void RegisterStandardTypes()
		{
			//!!!!где и как новые добавлять?
			//!!!!!!видать лучше где-то классами или атрибутами указывать

			RegisterType( "Assembly", new string[] { "dll" }, typeof( Resource_Assembly ) );

			//!!!!"object"?
			RegisterType( "Component", new string[] { "component", "class", "type" }, typeof( Resource ) );

			RegisterType( "Image", new string[] {
				"image", "jpg", "jif", "jpeg", "jpe", "tga", "targa", "dds", "png", "bmp", "psd", "hdr", "ico", "gif", "tif", "tiff",
				"exr", "j2k", "j2c", "jp2" },
				typeof( Resource_Image ) );

			RegisterType( "Sound", new string[] { "sound", "ogg", "wav" }, typeof( Resource_Sound ) );
			RegisterType( "Material", new string[] { "material" }, typeof( Resource ) );
			RegisterType( "Mesh", new string[] { "mesh" }, typeof( Resource ) );
			RegisterType( "Particle System", new string[] { "particle", "particleSystem" }, typeof( Resource ) );
			RegisterType( "Font", new string[] { "ttf" }, typeof( Resource_Font ) );
			RegisterType( "Object In Space", new string[] { "objectInSpace" }, typeof( Resource ) );
			RegisterType( "UI", new string[] { "ui" }, typeof( Resource ) );
			RegisterType( "UI Style", new string[] { "uistyle" }, typeof( Resource ) );
			RegisterType( "Scene", new string[] { "scene" }, typeof( Resource ) );
			RegisterType( "C# Script", new string[] { "csscript" }, typeof( Resource ) );
			RegisterType( "Flow Graph", new string[] { "flowgraph" }, typeof( Resource ) );
			RegisterType( "Product", new string[] { "product" }, typeof( Resource ) );
			RegisterType( "Store Product", new string[] { "store" }, typeof( Resource ) );
			RegisterType( "Surface", new string[] { "surface" }, typeof( Resource ) );
			RegisterType( "Sprite", new string[] { "sprite" }, typeof( Resource ) );
			//RegisterType( "Shader", new string[] { "shader" }, typeof( Resource ) );
			RegisterType( "Skybox", new string[] { "skybox" }, typeof( Resource ) );
			RegisterType( "Light", new string[] { "light" }, typeof( Resource ) );
		}

		internal static void DisposeAllResources()
		{
			again:;
			var array = GetAllResources();
			if( array.Length != 0 )
			{
				foreach( var ins in array )
					ins.Dispose();
				goto again;
			}
		}

		public static void TryReloadResource( string virtualPath )
		{
			var virtualPath2 = virtualPath;

			//!!!!good?
			if( Path.GetExtension( virtualPath2 ).ToLower() == ".settings" )
				virtualPath2 = virtualPath2.Substring( 0, virtualPath2.Length - ".settings".Length );

			var resource = GetByName( virtualPath2 );
			if( resource != null )
			{
				//!!!!

				try
				{
					resource.Dispose();
					LoadResource( virtualPath2, true );
				}
				catch( Exception e )
				{
					//!!!!
					Log.Warning( "Reload resource: File name: {0}. Error: {1}", virtualPath2, e.Message );
				}
			}
		}
	}
}
