// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Specifies a resource of the engine.
	/// </summary>
	public class Resource : ThreadSafeDisposable
	{
		internal ResourceManager.ResourceType resourceType;
		internal string name = "";
		internal bool loadFromFile;
		internal object creator;

		volatile Instance primaryInstance;
		//volatile TextBlock loadedBlock;
		volatile object loadedAnyData;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum InstanceType
		{
			Resource,
			SeparateInstance,
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//check: если сепаратный то не обязательно диспосить
		/// <summary>
		/// Represents an instance of <see cref="Resource"/>.
		/// </summary>
		public class Instance : ThreadSafeDisposable
		{
			Resource owner;
			InstanceType instanceType;
			bool componentCreateHierarchyController;
			bool? componentSetEnabled;

			internal volatile StatusEnum status = StatusEnum.CreationProcess;
			internal volatile string statusError = "";

			volatile object resultObject;

			////////////////

			public enum StatusEnum
			{
				CreationProcess,
				Error,
				//PartiallyReady,
				Ready
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			public Instance( Resource owner, InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
			{
				this.owner = owner;
				this.instanceType = instanceType;
				this.componentCreateHierarchyController = componentCreateHierarchyController;
				this.componentSetEnabled = componentSetEnabled;
			}

			public Resource Owner
			{
				get { return owner; }
			}

			public InstanceType InstanceType
			{
				get { return instanceType; }
			}

			public bool ComponentCreateHierarchyController
			{
				get { return componentCreateHierarchyController; }
			}

			public bool? ComponentSetEnabled
			{
				get { return componentSetEnabled; }
			}

			public StatusEnum Status
			{
				get { return status; }
				set
				{
					if( status == value )
						return;
					status = value;

					StatusChanged?.Invoke( this );
					AllInstances_StatusChanged?.Invoke( this );
				}
			}
			public event Action<Instance> StatusChanged;
			public static event Action<Instance> AllInstances_StatusChanged;

			public string StatusError
			{
				get { return statusError; }
				set
				{
					if( statusError == value )
						return;
					statusError = value;
				}
			}

			protected override void OnDispose()
			{
				//!!!!что было бы если диспоснуть недозагруженный. другой поток юзает то бишь

				IDisposable disposable = ResultObject as IDisposable;
				if( disposable != null )
					disposable.Dispose();

				if( instanceType == InstanceType.Resource && Owner != null )
					Owner.Dispose();

				//!!!!тут?
				DisposedEvent?.Invoke( this );
				AllInstances_DisposedEvent?.Invoke( this );
			}
			public event Action<Instance> DisposedEvent;
			public static event Action<Instance> AllInstances_DisposedEvent;

			public virtual object ResultObject
			{
				get { return resultObject; }
				set { resultObject = value; }
			}

			public Component ResultComponent
			{
				get { return ResultObject as Component; }
				set { ResultObject = value; }
			}

			public delegate void LoadOverrideDelegate( Instance instance, ref TextBlock loadedBlock, ref bool handled );
			public static event LoadOverrideDelegate LoadOverride;

			protected virtual void Load( ref TextBlock loadedBlock )
			{
				//override behavior by event
				if( ResultObject == null )
				{
					bool handled = false;
					LoadOverride?.Invoke( this, ref loadedBlock, ref handled );
					if( handled )
						return;
				}

				//default behavior
				if( ResultObject == null )
				{
					//load block
					if( loadedBlock/*Owner.LoadedBlock*/ == null )
					{
						string error;
						loadedBlock/*Owner.LoadedBlock*/ = TextBlockUtility.LoadFromVirtualFile( Owner.Name, out error );
						if( loadedBlock/*Owner.LoadedBlock*/ == null )
						{
							StatusError = error;
							Status = StatusEnum.Error;
							return;
						}
					}

					//parse text block
					string error2;
					var component = ComponentUtility.LoadComponentFromTextBlock( null, loadedBlock/*Owner.LoadedBlock*/, Owner.Name, this, componentSetEnabled,
						componentCreateHierarchyController, out error2 );
					//var component = ComponentUtils.LoadComponentFromTextBlock( Owner.LoadedBlock, Owner.Name, null, out error2 );
					if( component == null )
					{
						StatusError = error2;
						Status = StatusEnum.Error;
						return;
					}

					//!!!!

					//if( componentSetEnabled != null )
					//	component.Enabled = componentSetEnabled.Value;

					//xx xx;
					//xx xx;//сначала выключить иерархию? потом всем разом включится?

					////!!!!возможно это раньше нужно делать, т.к. в GetProvidedType() что-то там надо
					//if( componentCreateHierarchyController )
					//	ComponentUtils.CreateHierarchyControllerForRootComponent( component, this, true );//, true );

					//resource is ready
					//ResultObject = component;
					Status = StatusEnum.Ready;
				}
			}

			internal void PerformLoad()
			{
				TextBlock loadedBlock = null;

				Load( ref loadedBlock );
			}

			//!!!!threading
			public virtual IEnumerable<Metadata.TypeInfo> MetadataGetTypes()
			{
				//!!!!public/private

				var rootComponent = ResultComponent;
				if( rootComponent != null )
				{
					{
						//!!!!а не всегда ли есть тип
						var type = rootComponent.GetProvidedType();
						if( type != null )
							yield return type;
					}

					foreach( var c in rootComponent.GetComponents<Component>( false, true ) )
					{
						//!!!!а не всегда ли есть тип
						var type = c.GetProvidedType();
						if( type != null )
							yield return type;
					}
				}
			}

			//!!!!!!додумать про сохранение типов
			//!!!!знать кем выставлено
			//!!!!!!!!т.е. в Reference<> добавить переменную того, кто выставил значение?

			//!!!!threading
			public virtual Metadata.TypeInfo MetadataGetType( string pathInside )
			{
				//!!!!public/private
				//!!!!получается, тут только по иерархии компонент бегается по именам. можно было бы расширить?

				var c = GetComponentByPathInside( pathInside );
				if( c != null )
					return c.GetProvidedType();

				return null;
			}

			public virtual Component GetComponentByPathInside( string pathInside )
			{
				var c = ResultComponent;
				if( c != null )
				{
					if( string.IsNullOrEmpty( pathInside ) )
						return c;
					return c.Components[ pathInside ];
				}
				return null;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public Resource()
		{
		}

		public ResourceManager.ResourceType ResourceType
		{
			get { return resourceType; }
		}

		public string Name
		{
			get { return name; }
		}

		public bool LoadFromFile
		{
			get { return loadFromFile; }
		}

		public object Creator
		{
			get { return creator; }
			set { creator = value; }
		}

		protected override void OnDispose()
		{
			//!!!!!threading

			ResourceManager.RemoveInternal( this );

			primaryInstance?.Dispose();
		}

		public Instance PrimaryInstance
		{
			get { return primaryInstance; }
			set { primaryInstance = value; }
		}

		//public TextBlock LoadedBlock
		//{
		//	get { return loadedBlock; }
		//	set { loadedBlock = value; }
		//}

		public object LoadedAnyData
		{
			get { return loadedAnyData; }
			set { loadedAnyData = value; }
		}

		public virtual Instance CreateInstanceClassObject( InstanceType instanceType, bool componentCreateHierarchyController, bool? componentSetEnabled )
		{
			return new Instance( this, instanceType, componentCreateHierarchyController, componentSetEnabled );
		}

		//!!!!
		public virtual string GetSaveAddFileExtension()
		{
			return "";
		}

		public bool FileWasDeleted
		{
			get; set;
		}
	}
}
