// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Threading;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Base class of all components.
	/// </summary>
	[ResourceFileExtension( "component" )]
	//!!!![EditorDocumentWindow( typeof( Component_DocumentWindow ) )]
	[EditorDocumentWindow( typeof( DocumentWindow ) )]
	//[EditorDocumentWindow( typeof( DocumentWindowWithViewport ) )]//!!!! можно без сцены нойс нарисовать
	[EditorSettingsCell( typeof( SettingsCell_Properties ), true )]
	public class Component : Metadata.IMetadataProvider/*, ISettingsProvider*/, IDisposable
	{
		Metadata.TypeInfo baseType;

		//inside hierarchy threading: мысли: как только прицепили объект, то он может получать какие-то сообщения и эвенты
		Component parent;
		LinkedListNode<Component> parentListNode;

		ComponentSet components;

		string name = "";

		//!!!!
		//Metadata.TypeClassification classification = Metadata.TypeClassification.Component;

		//!!!!name
		//Metadata.AccessLevel typeAccessLevel = Metadata.AccessLevel.Public;
		//bool provideAsType;

		//!!!!
		Metadata.ComponentTypeInfo providedTypeCached;
		//!!!!internal
		internal bool providedTypeAllow = true;

		//virtual members
		//!!!!threading
		bool virtualMembersNeedUpdate = true;
		List<Metadata.Member> virtualMembers;
		Dictionary<string, Metadata.Member> virtualMemberBySignature;

		//!!!!надо систематизировать доступ к свойству. только по сигнатуре или как-то так
		//!!!!не надо ли что-то удалять
		//!!!!by name?
		Dictionary<string, object> virtualMemberValues;
		//internal Metadata.MemberValues virtualMemberValues;

		//!!!!!
		//!!!!!name
		//bool showComponentsInSettings;

		//!!!!reference?
		bool enabled = true;
		bool enabledInHierarchy;

		bool removeFromParentQueued;

		internal ComponentHierarchyController hierarchyController;

		//!!!!threading? где еще
		ThreadSafeExchangeBool disposed;

		bool cachedResourceReferenceInitialized;
		string cachedResourceReference;

		UniqueNameGenerator uniqueNameGenerator;

		//!!!!new
		//!!!!name
		internal bool createdByBaseType;
		/// <summary>
		/// Whether the object is created using a base type.
		/// </summary>
		[Browsable( false )]
		public bool CreatedByBaseType
		{
			get { return createdByBaseType; }
			//set { createdByBaseType = value; }
		}

		//!!!!может спрятать куда-нибудь из свойств? какие еще свойства и методы также тогда?
		/// <summary>
		/// Gets or sets the configuration data of the object's settings in the editor.
		/// </summary>
		[Browsable( false )]
		[Serialize]
		public string EditorDocumentConfiguration { get; set; }

		//EDictionary<string, string> editorSettings;

		//long uin;
		//static long uinCounter;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public Component()
		{
			parentListNode = new LinkedListNode<Component>( this );
			//!!!!может не сразу создавать
			components = new ComponentSet( this );

			//uinCounter++;
			//uin = uinCounter;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		#region Enabled

		/// <summary>
		/// Called when value of <see cref="Enabled"/> property is changed.
		/// </summary>
		protected virtual void OnEnabledChanged() { }

		internal virtual void OnEnabledInHierarchyChanged_Before() { }
		internal virtual void OnEnabledInHierarchyChanged_After() { }

		/// <summary>
		/// Called when value of <see cref="EnabledInHierarchy"/> property is changed.
		/// </summary>
		protected virtual void OnEnabledInHierarchyChanged()
		{
		}

		/// <summary>
		/// Occurs when the <see cref="Enabled"/> property value changes.
		/// </summary>
		public event Action<Component> EnabledChanged;
		/// <summary>
		/// Occurs when the <see cref="EnabledInHierarchy"/> property value changes.
		/// </summary>
		public event Action<Component> EnabledInHierarchyChanged;

		/// <summary>
		/// Whether the component is enabled. See also <see cref="EnabledInHierarchy"/> property.
		/// </summary>
		[DefaultValue( true )]
		//[Category( "Component" )]
		public /*virtual */bool Enabled
		{
			get { return enabled; }
			set
			{
				if( enabled == value )
					return;
				enabled = value;

				OnEnabledChanged();
				EnabledChanged?.Invoke( this );

				_UpdateEnabledInHierarchy( false );
			}
		}

		/// <summary>
		/// Determines whether the object is attached to a hierarchy of the components and enabled. The object will be enabled only when all parents are enabled, and the property <see cref="Enabled"/> is enabled.
		/// </summary>
		[Browsable( false )]
		public bool EnabledInHierarchy
		{
			get { return enabledInHierarchy; }
		}

		/// <summary>
		/// Called when the object is attached to a hierarchy of the components and is enabled. See also <see cref="EnabledInHierarchy"/> property.
		/// </summary>
		protected virtual void OnEnabled() { }
		/// <summary>
		/// Occurs when the object is attached to a hierarchy of the components and is enabled. See also <see cref="EnabledInHierarchy"/> property.
		/// </summary>
		public event Action<Component> EnabledEvent;

		/// <summary>
		/// Called when the object is attached to a hierarchy of the components and is enabled. The method is called only in simulation application. See also <see cref="EnabledInHierarchy"/> property.
		/// </summary>
		protected virtual void OnEnabledInSimulation() { }
		/// <summary>
		/// Occurs when the object is attached to a hierarchy of the components and is enabled. The method is called only in simulation application. See also <see cref="EnabledInHierarchy"/> property.
		/// </summary>
		public event Action<Component> EnabledInSimulation;

		/// <summary>
		/// Called when the object is detached from a hierarchy of the components or is disabled. See also <see cref="EnabledInHierarchy"/> property.
		/// </summary>
		protected virtual void OnDisabled() { }
		/// <summary>
		/// Occurs when the object is disabled from a hierarchy of the components or is disabled. See also <see cref="EnabledInHierarchy"/> property.
		/// </summary>
		public event Action<Component> DisabledEvent;

		/// <summary>
		/// Called when the object is detached from a hierarchy of the components or is disabled. The method is called only in simulation application. See also <see cref="EnabledInHierarchy"/> property.
		/// </summary>
		protected virtual void OnDisabledInSimulation() { }
		/// <summary>
		/// Occurs when the object is detached from a hierarchy of the components or is disabled. The method is called only in simulation application. See also <see cref="EnabledInHierarchy"/> property.
		/// </summary>
		public event Action<Component> DisabledInSimulation;

		internal void _UpdateEnabledInHierarchy( bool forceDisableBeforeRemove )
		{
			bool demand;
			if( Enabled && !forceDisableBeforeRemove )
			{
				if( parent != null )
					demand = parent.EnabledInHierarchy;
				else
				{
					if( hierarchyController != null )
						demand = hierarchyController.HierarchyEnabled;
					else
						demand = false;
				}
			}
			else
				demand = false;

			if( enabledInHierarchy != demand )
			{
				enabledInHierarchy = demand;

				if( !EnabledInHierarchy )
				{
					cachedResourceReferenceInitialized = false;
					cachedResourceReference = null;

					if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
					{
						OnDisabledInSimulation();
						DisabledInSimulation?.Invoke( this );
					}

					DisabledEvent?.Invoke( this );
					OnDisabled();
				}

				OnEnabledInHierarchyChanged_Before();

				//notify components
				{
					Component[] array = new Component[ components.Count ];
					components.CopyTo( array, 0 );
					foreach( Component c in array )
					{
						if( c.Parent == this )
							c._UpdateEnabledInHierarchy( false );
					}
				}

				OnEnabledInHierarchyChanged();
				OnEnabledInHierarchyChanged_After();
				EnabledInHierarchyChanged?.Invoke( this );

				if( EnabledInHierarchy )
				{
					OnEnabled();
					EnabledEvent?.Invoke( this );

					if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
					{
						OnEnabledInSimulation();
						EnabledInSimulation?.Invoke( this );
					}
				}
			}
		}

		#endregion

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#region Serialization

		//!!!!еще SaveSupportAttribute? хотя оно ведь указывается выше, когда свойство помечается. а еще есть ISaveSupport?

		//!!!!!по сути этот параметр юзером в редакторе не ставится, т.к. его нет? или типа как шаблон для объекта, который не будет сохраняться?

		/// <summary>
		/// Whether the object supports saving to a file.
		/// </summary>
		[DefaultValue( true )]
		//[Serialize]
		[Browsable( false )]
		public Reference<bool> SaveSupport
		{
			get { if( _saveSupport.BeginGet() ) SaveSupport = _saveSupport.Get( this ); return _saveSupport.value; }
			set { if( _saveSupport.BeginSet( ref value ) ) { try { SaveSupportChanged?.Invoke( this ); } finally { _saveSupport.EndSet(); } } }
		}
		/// <summary>
		/// Occurs after changing value of <see cref="SaveSupport"/> property.
		/// </summary>
		public event Action<Component> SaveSupportChanged;
		ReferenceField<bool> _saveSupport = true;

		//!!!!!по сути этот параметр юзером в редакторе не ставится, т.к. его нет? или типа как шаблон для объекта, который не будет сохраняться?

		/// <summary>
		/// Whether the object supports cloning.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Browsable( false )]
		public Reference<bool> CloneSupport
		{
			get { if( _cloneSupport.BeginGet() ) CloneSupport = _cloneSupport.Get( this ); return _cloneSupport.value; }
			set { if( _cloneSupport.BeginSet( ref value ) ) { try { CloneSupportChanged?.Invoke( this ); } finally { _cloneSupport.EndSet(); } } }
		}
		/// <summary>
		/// Occurs after changing value of <see cref="CloneSupport"/> property.
		/// </summary>
		public event Action<Component> CloneSupportChanged;
		ReferenceField<bool> _cloneSupport = true;

		//

		public delegate void LoadEventDelegate( Component sender, Metadata.LoadContext context, TextBlock block, ref string error );
		/// <summary>
		/// Occurs during object loading.
		/// </summary>
		public event LoadEventDelegate LoadEvent;

		public delegate void SaveEventDelegate( Component sender, Metadata.SaveContext context, TextBlock block, ref bool skipSave, ref string error );
		/// <summary>
		/// Occurs during object saving.
		/// </summary>
		public event SaveEventDelegate SaveEvent;

		public delegate void CloneEventDelegate( Component sender, Metadata.CloneContext context, Component newComponent );
		/// <summary>
		/// Occurs when object is cloned.
		/// </summary>
		public event CloneEventDelegate CloneEvent;

		//

		//internal bool _LoadContextComponentItems( Metadata.LoadContext context, TextBlock block, out string error )
		//{
		//}

		//internal bool _LoadHierarchy( Metadata.LoadContext context, TextBlock block, out string error )
		//{
		//	error = "";

		//	context.textBlockByComponent[ this ] = block;

		//	//load Name property
		//	if( block.IsAttributeExist( "Name" ) )
		//		Name = block.GetAttribute( "Name" );

		//	//components
		//	foreach( TextBlock componentBlock in block.Children )
		//	{
		//		if( componentBlock.Name == ".component" )
		//		{
		//			//!!!!!!что тут с пересечениями

		//			if( string.IsNullOrEmpty( componentBlock.Data ) )
		//			{
		//				//!!!!
		//				Log.Fatal( "impl" );
		//			}

		//			//!!!!
		//			if( componentBlock.Data == "_Dev\\Import3D\\ColladaTest2.dae:Originals\\Materials\\ColladaTestStoneStatic" )
		//				Log.Warning( "dfgdfg" );

		//			//чтобы заинстансить объект, то он будет клонирован. чтобы клонировать, то нужно загрузить его свойства

		//			Metadata.TypeInfo type = MetadataManager.GetType( componentBlock.Data );
		//			if( type == null )
		//			{
		//				//!!!!
		//				//Metadata.TypeInfo type2 = MetadataManager.GetType( componentBlock.Data );
		//				//if( type2 == null )
		//				//	Log.Fatal( "impl" );

		//				//!!!!
		//				Log.Fatal( "impl" );
		//			}

		//			var component = (Component)type.InvokeInstance( null );

		//			//!!!!!!так?
		//			AddComponent( component );

		//			string error3;
		//			if( !component._LoadHierarchy( context, componentBlock, out error3 ) )
		//			{
		//				//!!!!
		//				Log.Fatal( "impl" );

		//				//!!!!!?
		//				//RemoveComponent
		//				//component = null;
		//			}
		//		}
		//	}

		//	return true;
		//}

		//!!!!ссылки на ресурсные объекты тоже сохранять

		//protected virtual bool OnLoad_Old( Metadata.LoadContext context, TextBlock block, out string error )
		//{
		//	//новый порядок загрузки. теперь сначала компоненты. это нужно, чтобы сначала Component_Member загрузить и создать метаданные
		//	////serialize parameters
		//	//if( !MetadataManager.Serialization.LoadSerializableMembers( context, this, block, out error ) )
		//	//	return false;

		//	//components
		//	foreach( var component in GetComponents() )
		//	{
		//		TextBlock componentBlock;
		//		if( context.textBlockByComponent.TryGetValue( component, out componentBlock ) )
		//		{
		//			string error3;
		//			if( !component._LoadFromTextBlock( context, componentBlock, out error3 ) )
		//			{
		//				//!!!!
		//				Log.Fatal( "impl" );

		//				//!!!!!?
		//				//RemoveComponent
		//				//component = null;
		//			}
		//		}
		//	}

		//	//serialize parameters
		//	if( !MetadataManager.Serialization.LoadSerializableMembers( context, this, block, out error ) )
		//		return false;


		//	////components
		//	//foreach( TextBlock componentBlock in block.Children )
		//	//{
		//	//	if( componentBlock.Name == ".component" )
		//	//	{
		//	//		if( string.IsNullOrEmpty( componentBlock.Data ) )
		//	//		{
		//	//			//!!!!
		//	//			Log.Fatal( "impl" );
		//	//		}

		//	//		Metadata.TypeInfo type = MetadataManager.GetType( componentBlock.Data );
		//	//		if( type == null )
		//	//		{
		//	//			//!!!!
		//	//			Log.Fatal( "impl" );
		//	//		}

		//	//		var component = (Component)type.InvokeInstance( null );

		//	//		//!!!!!!так?
		//	//		AddComponent( component );

		//	//		string error3;
		//	//		if( !component.Load2( context, componentBlock, out error3 ) )
		//	//		{
		//	//			//!!!!
		//	//			Log.Fatal( "impl" );

		//	//			//!!!!!?
		//	//			//RemoveComponent
		//	//			//component = null;
		//	//		}
		//	//	}
		//	//}

		//	return true;
		//}

		/// <summary>
		/// Called during object loading.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="block"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		protected virtual bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			error = "";
			return true;
		}

		internal bool _Load( Metadata.LoadContext context, TextBlock block, out string error )
		{
			error = "";

			//serialize parameters
			if( !MetadataManager.Serialization.LoadSerializableMembers( context, this, block, out error ) )
				return false;

			if( !OnLoad( context, block, out error ) )
				return false;

			LoadEvent?.Invoke( this, context, block, ref error );
			if( !string.IsNullOrEmpty( error ) )
				return false;

			return true;
		}

		//internal bool _LoadFromTextBlock( Metadata.LoadContext context, TextBlock block, out string error )
		//{
		//	error = "";

		//	if( !OnLoad( context, block, out error ) )
		//		return false;

		//	LoadEvent?.Invoke( this, context, block, ref error );
		//	if( !string.IsNullOrEmpty( error ) )
		//		return false;

		//	return true;
		//}

		/// <summary>
		/// Called during object saving.
		/// </summary>
		protected virtual bool OnSave( Metadata.SaveContext context, TextBlock block, ref bool skipSave, out string error )
		{
			error = "";
			return true;
		}

		internal bool _SaveToTextBlock( Metadata.SaveContext context, TextBlock parentBlock, out bool skipSave, out string error )
		{
			error = "";

			skipSave = !SaveSupport;
			if( !skipSave )
			{
				TextBlock block = parentBlock.AddChild( ".component", MetadataManager.MetadataGetType( this ).Name );

				if( CreatedByBaseType )
				{
					block.SetAttribute( "CreatedByBaseType", "True" );
					GetBaseTypeIndex( out int baseHierarchyIndex, out string baseName, out int baseNameIndex );
					block.SetAttribute( "CreatedByBaseTypeHierarchyIndex", baseHierarchyIndex.ToString() );
					if( baseNameIndex != 0 )
						block.SetAttribute( "CreatedByBaseTypeNameIndex", baseNameIndex.ToString() );
				}

				{
					//deleted components which created by base type
					if( BaseType != null )
					{
						var componentBaseType = BaseType as Metadata.ComponentTypeInfo;
						if( componentBaseType != null && componentBaseType.BasedOnObject != null )
						{
							foreach( var baseComponent in componentBaseType.BasedOnObject.Components )
							{
								bool check = false;
								//!!!!только это проверять?
								if( baseComponent.SaveSupport && !baseComponent.TypeOnly )
									check = true;

								if( check )
								{
									baseComponent.GetBaseTypeIndex( out int baseHierarchyIndex, out string baseName, out int baseNameIndex );

									bool found = false;
									foreach( var c in components )
									{
										if( c.CreatedByBaseType )
										{
											c.GetBaseTypeIndex( out int hierarchyIndex, out string name, out int nameIndex );
											if( baseHierarchyIndex == hierarchyIndex && baseName == name && baseNameIndex == nameIndex )
											{
												found = true;
												break;
											}
										}
									}

									if( !found )
									{
										TextBlock b = block.AddChild( ".componentCreatedByBaseTypeWasDeleted" );
										b.SetAttribute( "CreatedByBaseTypeHierarchyIndex", baseHierarchyIndex.ToString() );
										b.SetAttribute( "CreatedByBaseTypeName", baseName );
										if( baseNameIndex != 0 )
											b.SetAttribute( "CreatedByBaseTypeNameIndex", baseNameIndex.ToString() );
									}
								}
							}
						}
					}

					//serialize members
					if( !MetadataManager.Serialization.SaveSerializableMembers( context, this, block, out error ) )
						return false;

					//components
					foreach( Component component in components )
					{
						if( component.SaveSupport && component.TypeSettingsIsPublic() )
						{
							//bool skip = false;

							////Type Settings filter
							//var baseComponentType = BaseType as Metadata.ComponentTypeInfo;
							//if( baseComponentType != null && baseComponentType.BasedOnObject.TypeSettingsIsPrivateObject( component ) )
							//	skip = true;

							//if( !skip )
							//{
							if( !component._SaveToTextBlock( context, block, out _, out error ) )
								return false;
							//}
						}
					}
				}

				if( !OnSave( context, block, ref skipSave, out error ) )
					return false;
				if( !string.IsNullOrEmpty( error ) )
					return false;

				if( !skipSave )
				{
					SaveEvent?.Invoke( this, context, block, ref skipSave, ref error );
					if( !string.IsNullOrEmpty( error ) )
						return false;
				}

				if( skipSave )
					parentBlock.DeleteChild( block );
			}

			return true;
		}

		internal void _CloneHierarchy( Metadata.CloneContext context, Component newObject )//, bool skipTypeOnly )
		{
			context.newComponentsRedirection[ this ] = newObject;
			context.newComponentsQueue.Enqueue( (this, newObject) );

			//load Name property
			newObject.Name = Name;

			//components
			foreach( var child in GetComponents() )
			{
				if( !child.CloneSupport )
					continue;
				if( context.typeOfCloning == Metadata.CloneContext.TypeOfCloningEnum.CreateInstanceOfType && child.TypeOnly )
					continue;
				//if( skipTypeOnly && child.TypeOnly )
				//	continue;

				//!!!!!!что тут с пересечениями

				//!!!!CloneSupport

				var newChild = child._CloneInstance();
				//var newChild = child.Clone( context );

				//!!!!рутовому нужно createdByBaseType = true?
				switch( context.typeOfCloning )
				{
				case Metadata.CloneContext.TypeOfCloningEnum.Usual:
					newChild.createdByBaseType = child.CreatedByBaseType;
					break;
				case Metadata.CloneContext.TypeOfCloningEnum.CreateInstanceOfType:
					newChild.createdByBaseType = true;
					break;
				}

				//!!!!в какой очереди вызывать?
				newObject.AddComponent( newChild );
				child._CloneHierarchy( context, newChild );//, skipTypeOnly );
			}
		}

		/// <summary>
		/// Called when the object is cloned.
		/// </summary>
		protected virtual void OnClone( Metadata.CloneContext context, Component newObject ) { }

		//protected virtual void OnClone( Metadata.CloneContext context, Component newObject )
		//{
		//	MetadataManager.Serialization.CloneMemberValues( context, this, newObject );
		//}

		internal Component _CloneInstance(/*было, но не юзалось Type overrideClass = null, */Metadata.TypeInfo overrideBaseType = null )
		{
			Type netClass = GetType();
			//Type netClass = overrideClass != null ? overrideClass : GetType();

			//parameters?
			ConstructorInfo constructor = netClass.GetConstructor( Array.Empty<Type>() );
			Component newObject = (Component)constructor.Invoke( Array.Empty<object>() );
			newObject.BaseType = overrideBaseType != null ? overrideBaseType : BaseType;

			return newObject;
		}

		//!!!!!может сразу возможность цеплять за родителя? тогда и про Enabled разобраться. из минусов - типа как смешиваем создание и включение в мир. хотя для кого-то это был бы плюс может быть
		/// <summary>
		/// Creates a copy of the object.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public Component Clone( Metadata.CloneContext context = null )
		//public Component Clone( Metadata.CloneContext context = null, Type overrideClass = null, Metadata.TypeInfo overrideBaseType = null )
		{
			if( context == null )
				context = new Metadata.CloneContext();

			//baseTypeForCreationInstanceOfType
			//Metadata.TypeInfo overrideBaseType = null

			Component newObject = _CloneInstance( /*overrideClass, */context.baseTypeForCreationInstance/* overrideBaseType */);

			////!!!!так?
			//bool skipTypeOnly = overrideBaseType != null;

			_CloneHierarchy( context, newObject );//, skipTypeOnly );

			while( context.newComponentsQueue.Count != 0 )
			{
				var pair = context.newComponentsQueue.Dequeue();
				var sourceC = pair.Item1;
				var newC = pair.Item2;

				MetadataManager.Serialization.CloneMemberValues( context, sourceC, newC );

				sourceC.OnClone( context, newC );
				sourceC.CloneEvent?.Invoke( sourceC, context, newC );
			}

			return newObject;
		}

		#endregion

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#region Components

		//!!!!
		//[TypeConverter( typeof( CollectionTypeConverter ) )]
		//!!!!!![Editor( typeof( EditorBaseObjectComponentsEditor ), typeof( UITypeEditor ) )]
		/// <summary>
		/// Gets the collection of the child components.
		/// </summary>
		[Browsable( false )]
		public ComponentSet Components
		{
			get { return components; }
		}

		//!!!!по сути методы лишние

		/// <summary>
		/// Finds a child component by name and number in the case when there are several components with the specified name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="nameIndex"></param>
		/// <returns></returns>
		public Component GetComponentByNameWithIndex( string name, int nameIndex )
		{
			return Components.GetByNameWithIndex( name, nameIndex );
		}

		/// <summary>
		/// Finds a child component by name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="onlyEnabledInHierarchy"></param>
		/// <returns></returns>
		public Component GetComponent( string name, bool onlyEnabledInHierarchy = false )
		{
			return Components.GetByName( name, onlyEnabledInHierarchy );
		}

		/// <summary>
		/// Finds a child component by name with the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="onlyEnabledInHierarchy"></param>
		/// <returns></returns>
		public T GetComponent<T>( string name, bool onlyEnabledInHierarchy = false )// where T : Component
		{
			return Components.GetByName<T>( name, onlyEnabledInHierarchy );
		}

		/// <summary>
		/// Finds a child component by path.
		/// </summary>
		/// <param name="nameOrPath"></param>
		/// <returns></returns>
		public Component GetComponentByPath( string nameOrPath )
		{
			return Components.GetByPath( nameOrPath );
		}

		//public Component GetComponent( string name, bool checkChildren = false, bool onlyEnabledInHierarchy = false )
		//{
		//	//!!!!может string namePath?

		//	foreach( Component component in components )
		//	{
		//		if( ( !onlyEnabledInHierarchy || component.EnabledInHierarchy ) && name == component.Name )
		//			return component;
		//	}
		//	if( checkChildren )
		//	{
		//		foreach( Component component in components )
		//		{
		//чтобы дальше не пошло EnabledInHierarchy
		//			Component component2 = component.GetComponent( name, true, onlyEnabledInHierarchy );
		//			if( component2 != null )
		//				return component2;
		//		}
		//	}
		//	return null;
		//}

		/// <summary>
		/// Finds a child component of the specified type.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <param name="checkChildren"></param>
		/// <param name="onlyEnabledInHierarchy"></param>
		/// <returns></returns>
		public Component GetComponent( Metadata.TypeInfo componentClass, bool checkChildren = false, bool onlyEnabledInHierarchy = false/*, bool depthFirstSearch = false*/ )
		{
			if( components.Count != 0 )
			{
				//!!!!no bool reverse

				//if( depthFirstSearch )
				//{
				//	foreach( Component component in components )
				//	{
				//		//!!!!( componentClass == typeof( Component ) ||
				//		//!!!!!!ниже еще

				//		if( !onlyEnabledInHierarchy || component.EnabledInHierarchy )
				//		{
				//			if( componentClass.IsAssignableFrom( component.BaseType ) )
				//				return component;

				//			if( checkChildren )
				//			{
				//				Component component2 = component.GetComponent( componentClass, true, onlyEnabledInHierarchy, true );
				//				if( component2 != null )
				//					return component2;
				//			}
				//		}
				//	}
				//}
				//else
				//{
				foreach( Component component in components )
				{
					//!!!!( componentClass == typeof( Component ) ||
					//!!!!!!ниже еще

					if( ( !onlyEnabledInHierarchy || component.EnabledInHierarchy ) && componentClass.IsAssignableFrom( component.BaseType ) )
						return component;
				}
				if( checkChildren )
				{
					foreach( Component component in components )
					{
						if( !onlyEnabledInHierarchy || component.EnabledInHierarchy )
						{
							Component component2 = component.GetComponent( componentClass, true, onlyEnabledInHierarchy );
							if( component2 != null )
								return component2;
						}
					}
				}
				//}
			}
			return null;
		}

		/// <summary>
		/// Finds a child component of the specified type.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <param name="checkChildren"></param>
		/// <param name="onlyEnabledInHierarchy"></param>
		/// <returns></returns>
		public Component GetComponent( Type componentClass, bool checkChildren = false, bool onlyEnabledInHierarchy = false )
		{
			if( components.Count != 0 )
			{
				foreach( Component component in components )
				{
					if( ( !onlyEnabledInHierarchy || component.EnabledInHierarchy ) && ( componentClass == typeof( Component ) || componentClass.IsAssignableFrom( component.GetType() ) ) )
						return component;
				}
				if( checkChildren )
				{
					foreach( Component component in components )
					{
						if( !onlyEnabledInHierarchy || component.EnabledInHierarchy )
						{
							Component component2 = component.GetComponent( componentClass, true, onlyEnabledInHierarchy );
							if( component2 != null )
								return component2;
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Finds a child component of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="checkChildren"></param>
		/// <param name="onlyEnabledInHierarchy"></param>
		/// <returns></returns>
		public T GetComponent<T>( bool checkChildren = false, bool onlyEnabledInHierarchy = false ) where T : class// Component
		{
			return (T)(object)GetComponent( typeof( T ), checkChildren, onlyEnabledInHierarchy );
			//return (T)GetComponent( typeof( T ), checkChildren, onlyEnabledInHierarchy );
		}

		void GetComponentsRecursive<T>( Metadata.TypeInfo componentClass, bool reverse, bool checkChildren, bool onlyEnabledInHierarchy, bool depthFirstSearch, ref List<T> list ) where T : class //Component
		{
			if( components.Count != 0 )
			{
				var items = ( reverse && components.Count > 1 ) ? components.Reverse() : components;

				if( depthFirstSearch )
				{
					foreach( Component component in items )
					{
						if( !onlyEnabledInHierarchy || component.EnabledInHierarchy )
						{
							if( componentClass.IsAssignableFrom( component.BaseType ) )
							{
								if( list == null )
									list = new List<T>( 32 );
								list.Add( (T)(object)component );
								//list.Add( (T)component );
							}
							if( checkChildren )
								component.GetComponentsRecursive( componentClass, reverse, true, onlyEnabledInHierarchy, true, ref list );
						}
					}
				}
				else
				{
					foreach( Component component in items )
					{
						if( ( !onlyEnabledInHierarchy || component.EnabledInHierarchy ) && componentClass.IsAssignableFrom( component.BaseType ) )
						{
							if( list == null )
								list = new List<T>( 32 );
							list.Add( (T)(object)component );
							//list.Add( (T)component );
						}
					}
					if( checkChildren )
					{
						foreach( Component component in items )
						{
							if( !onlyEnabledInHierarchy || component.EnabledInHierarchy )
								component.GetComponentsRecursive( componentClass, reverse, true, onlyEnabledInHierarchy, false, ref list );
						}
					}
				}
			}
		}

		void GetComponentsRecursive<T>( Type componentClass, bool reverse, bool checkChildren, bool onlyEnabledInHierarchy, bool depthFirstSearch, ref List<T> list ) where T : class //where T : Component
		{
			if( components.Count != 0 )
			{
				var items = ( reverse && components.Count > 1 ) ? components.Reverse() : components;

				if( depthFirstSearch )
				{
					foreach( Component component in items )
					{
						if( !onlyEnabledInHierarchy || component.EnabledInHierarchy )
						{
							if( componentClass == typeof( Component ) || componentClass.IsAssignableFrom( component.GetType() ) )
							{
								if( list == null )
									list = new List<T>( 32 );
								list.Add( (T)(object)component );
								//list.Add( (T)component );
							}
							if( checkChildren )
								component.GetComponentsRecursive( componentClass, reverse, true, onlyEnabledInHierarchy, true, ref list );
						}
					}
				}
				else
				{
					foreach( Component component in items )
					{
						if( ( !onlyEnabledInHierarchy || component.EnabledInHierarchy ) && ( componentClass == typeof( Component ) || componentClass.IsAssignableFrom( component.GetType() ) ) )
						{
							if( list == null )
								list = new List<T>( 32 );
							list.Add( (T)(object)component );
							//list.Add( (T)component );
						}
					}
					if( checkChildren )
					{
						foreach( Component component in items )
						{
							if( !onlyEnabledInHierarchy || component.EnabledInHierarchy )
								component.GetComponentsRecursive( componentClass, reverse, true, onlyEnabledInHierarchy, false, ref list );
						}
					}
				}
			}
		}

		/// <summary>
		/// Finds child components of the specified type.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <param name="reverse"></param>
		/// <param name="checkChildren"></param>
		/// <param name="onlyEnabledInHierarchy"></param>
		/// <returns></returns>
		public Component[] GetComponents( Metadata.TypeInfo componentClass, bool reverse = false, bool checkChildren = false, bool onlyEnabledInHierarchy = false, bool depthFirstSearch = false )
		{
			List<Component> list = null;
			GetComponentsRecursive( componentClass, reverse, checkChildren, onlyEnabledInHierarchy, depthFirstSearch, ref list );
			return list != null ? list.ToArray() : Array.Empty<Component>();
		}

		/// <summary>
		/// Finds child components of the specified type.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <param name="reverse"></param>
		/// <param name="checkChildren"></param>
		/// <param name="onlyEnabledInHierarchy"></param>
		/// <returns></returns>
		public Component[] GetComponents( Type componentClass, bool reverse = false, bool checkChildren = false, bool onlyEnabledInHierarchy = false, bool depthFirstSearch = false )
		{
			List<Component> list = null;
			GetComponentsRecursive( componentClass, reverse, checkChildren, onlyEnabledInHierarchy, depthFirstSearch, ref list );
			return list != null ? list.ToArray() : Array.Empty<Component>();
		}

		/// <summary>
		/// Finds child components.
		/// </summary>
		/// <param name="reverse"></param>
		/// <param name="checkChildren"></param>
		/// <param name="onlyEnabledInHierarchy"></param>
		/// <returns></returns>
		public Component[] GetComponents( bool reverse = false, bool checkChildren = false, bool onlyEnabledInHierarchy = false, bool depthFirstSearch = false )
		{
			return GetComponents( typeof( Component ), reverse, checkChildren, onlyEnabledInHierarchy, depthFirstSearch );
		}

		/// <summary>
		/// Finds child components of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reverse"></param>
		/// <param name="checkChildren"></param>
		/// <param name="onlyEnabledInHierarchy"></param>
		/// <returns></returns>
		public T[] GetComponents<T>( bool reverse = false, bool checkChildren = false, bool onlyEnabledInHierarchy = false, bool depthFirstSearch = false ) where T : class //Component
		{
			List<T> list = null;
			GetComponentsRecursive( typeof( T ), reverse, checkChildren, onlyEnabledInHierarchy, depthFirstSearch, ref list );
			return list != null ? list.ToArray() : Array.Empty<T>(); //new T[ 0 ];
		}

		void GetComponentsRecursive<T>( Type componentClass, bool reverse, bool checkChildren, bool onlyEnabledInHierarchy, bool depthFirstSearch, Action<T> action ) where T : class //where T : Component
		{
			if( components.Count != 0 )
			{
				var items = ( reverse && components.Count > 1 ) ? components.Reverse() : components;

				if( depthFirstSearch )
				{
					foreach( Component component in items )
					{
						if( !onlyEnabledInHierarchy || component.EnabledInHierarchy )
						{
							if( componentClass == typeof( Component ) || componentClass.IsAssignableFrom( component.GetType() ) )
								action( (T)(object)component );

							if( checkChildren )
								component.GetComponentsRecursive( componentClass, reverse, true, onlyEnabledInHierarchy, true, action );
						}
					}
				}
				else
				{
					foreach( Component component in items )
					{
						if( ( !onlyEnabledInHierarchy || component.EnabledInHierarchy ) && ( componentClass == typeof( Component ) || componentClass.IsAssignableFrom( component.GetType() ) ) )
							action( (T)(object)component );
					}
					if( checkChildren )
					{
						foreach( Component component in items )
						{
							if( !onlyEnabledInHierarchy || component.EnabledInHierarchy )
								component.GetComponentsRecursive( componentClass, reverse, true, onlyEnabledInHierarchy, false, action );
						}
					}
				}
			}
		}

		//public Component[] GetComponents( Metadata.TypeInfo componentClass, bool reverse = false, bool checkChildren = false, bool onlyEnabledInHierarchy = false )
		//{
		//	List<Component> list = null;
		//	GetComponentsRecursive( componentClass, reverse, checkChildren, onlyEnabledInHierarchy, ref list );
		//	return list != null ? list.ToArray() : Array.Empty<Component>();
		//}

		//public Component[] GetComponents( Type componentClass, bool reverse = false, bool checkChildren = false, bool onlyEnabledInHierarchy = false )
		//{
		//	List<Component> list = null;
		//	GetComponentsRecursive( componentClass, reverse, checkChildren, onlyEnabledInHierarchy, ref list );
		//	return list != null ? list.ToArray() : Array.Empty<Component>();
		//}

		/// <summary>
		/// Finds child components.
		/// </summary>
		/// <param name="reverse"></param>
		/// <param name="checkChildren"></param>
		/// <param name="onlyEnabledInHierarchy"></param>
		/// <param name="depthFirstSearch"></param>
		/// <param name="action"></param>
		public void GetComponents( bool reverse, bool checkChildren, bool onlyEnabledInHierarchy, bool depthFirstSearch, Action<Component> action )
		{
			GetComponentsRecursive( typeof( Component ), reverse, checkChildren, onlyEnabledInHierarchy, depthFirstSearch, action );
		}

		/// <summary>
		/// Finds child components of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reverse"></param>
		/// <param name="checkChildren"></param>
		/// <param name="onlyEnabledInHierarchy"></param>
		/// <param name="depthFirstSearch"></param>
		/// <param name="action"></param>
		public void GetComponents<T>( bool reverse, bool checkChildren, bool onlyEnabledInHierarchy, bool depthFirstSearch, Action<T> action ) where T : class
		{
			GetComponentsRecursive( typeof( T ), reverse, checkChildren, onlyEnabledInHierarchy, depthFirstSearch, action );
		}

		//public IEnumerable< Component> GetComponentsWhere( bool reverse = false, bool checkChildren = false, bool onlyEnabledInHierarchy = false, Func<TSource, bool> predicate )
		//{
		//}

		//public Component[] GetAllComponentsByClass( Type componentClass, bool includeChildren = false )
		//{
		//	List<Component> list = null;
		//	GetAllComponentsByClassRecursive( componentClass, includeChildren, ref list );
		//	return list != null ? list.ToArray() : emptyComponentArray;
		//}

		//public Component GetComponentByClass( Type componentClass, bool includeChildren = false )
		//{
		//	foreach( Component component in components )
		//	{
		//		if( componentClass.IsAssignableFrom( component.GetType() ) )
		//			return component;
		//		if( includeChildren )
		//		{
		//			Component component2 = component.GetComponentByClass( componentClass, true );
		//			if( component2 != null )
		//				return component2;
		//		}
		//	}
		//	return null;
		//}

		//public T[] GetAllComponentsByClass<T>( bool includeChildren = false ) where T : Component
		//{
		//	List<Component> list = null;
		//	GetAllComponentsByClassRecursive( typeof( T ), includeChildren, ref list );
		//	if( list != null )
		//	{
		//		T[] a = new T[ list.Count ];
		//		for( int n = 0; n < list.Count; n++ )
		//			a[ n ] = (T)list[ n ];
		//		return a;
		//	}
		//	else
		//		return new T[ 0 ];
		//}

		////!!!!GetFirstComponentByClass? как проще имена сделать?
		//public T GetComponentByClass<T>( bool includeChildren = false ) where T : Component
		//{
		//	return (T)GetComponentByClass( typeof( T ), includeChildren );
		//}

		//void GetAllComponentsByClassRecursive( string className, bool includeChildren, ref List<Component> list )
		//{
		//	foreach( Component component in components )
		//	{
		//		if( component.GetType().Name == className || component.GetType().FullName == className )
		//		{
		//			if( list == null )
		//				list = new List<Component>( 32 );
		//			list.Add( component );
		//		}
		//		if( includeChildren )
		//			component.GetAllComponentsByClassRecursive( className, true, ref list );
		//	}
		//}

		//public Component[] GetAllComponentsByClass( string className, bool includeChildren = false )
		//{
		//	List<Component> list = null;
		//	GetAllComponentsByClassRecursive( className, includeChildren, ref list );
		//	return list != null ? list.ToArray() : emptyComponentArray;
		//}

		//public Component GetComponentByClass( string className, bool includeChildren = false )
		//{
		//	foreach( Component component in components )
		//	{
		//		if( component.GetType().Name == className || component.GetType().FullName == className )
		//			return component;
		//		if( includeChildren )
		//		{
		//			Component component2 = component.GetComponentByName( className, true );
		//			if( component2 != null )
		//				return component2;
		//		}
		//	}
		//	return null;
		//}

		//void GetAllComponentsByNameRecursive<T>( string name, bool includeChildren, ref List<T> list ) where T : Component
		//{
		//	foreach( Component component2 in components )
		//	{
		//		T component = component2 as T;
		//		if( component != null )
		//		{
		//			if( component.Name == name )
		//			{
		//				if( list == null )
		//					list = new List<T>( 32 );
		//				list.Add( component );
		//			}
		//			if( includeChildren )
		//				component.GetAllComponentsByNameRecursive( name, true, ref list );
		//		}
		//	}
		//}

		//public T[] GetAllComponentsByName<T>( string name, bool includeChildren = false ) where T : Component
		//{
		//	List<T> list = null;
		//	GetAllComponentsByNameRecursive( name, includeChildren, ref list );
		//	return list != null ? list.ToArray() : new T[ 0 ];
		//}

		//public Component[] GetAllComponentsByName( string name, bool includeChildren = false )
		//{
		//	List<Component> list = null;
		//	GetAllComponentsByNameRecursive( name, includeChildren, ref list );
		//	return list != null ? list.ToArray() : emptyComponentArray;
		//}

		//public T GetComponentByName<T>( string name, bool includeChildren = false ) where T : Component
		//{
		//	foreach( Component component3 in components )
		//	{
		//		T component = component3 as T;
		//		if( component != null )
		//		{
		//			if( component.Name == name )
		//				return component;
		//			if( includeChildren )
		//			{
		//				T component2 = component.GetComponentByName<T>( name, true );
		//				if( component2 != null )
		//					return component2;
		//			}
		//		}
		//	}
		//	return null;
		//}

		//public Component GetComponentByName( string name, bool includeChildren = false )
		//{
		//	return GetComponentByName<Component>( name, includeChildren );
		//}

		/// <summary>
		/// Adds a component as a child.
		/// </summary>
		/// <param name="component"></param>
		/// <param name="insertIndex"></param>
		public void AddComponent( Component component, int insertIndex = -1 )
		{
			if( component.HierarchyController != null )
				Log.Fatal( "Component: AddComponent: component.HierarchyController != null." );
			if( component.Parent != null )
				Log.Fatal( "Component: AddComponent: component.Parent != null. This component is already attached to another parent." );
			if( component.Disposed )
				Log.Fatal( "Component: AddComponent: component.Disposed == true." );
			if( Disposed )
				Log.Fatal( "Component: AddComponent: Disposed == true." );

			if( components.linkedList == null )
				components.linkedList = new LinkedList<Component>();

			component.parent = this;

			bool addLast = insertIndex < 0 || insertIndex >= components.Count;

			if( addLast )
			{
				components.linkedList.AddLast( component.parentListNode );
			}
			else
			{
				//!!!!slowly2?

				bool wasAdded = false;
				int index = 0;
				for( var iterator = components.linkedList.First; iterator != null; iterator = iterator.Next )
				{
					if( index == insertIndex )
					{
						components.linkedList.AddBefore( iterator, component.parentListNode );
						wasAdded = true;
						break;
					}
					index++;
				}
				if( !wasAdded )
					components.linkedList.AddLast( component.parentListNode );
			}

			components.ComponentsByNameAdd( component, addLast );

			component._UpdateEnabledInHierarchy( false );

			//!!!!!порядок норм?
			//inside hierarchy threading: мысли: по сути родителю нельзя оповещать. чтобы он не парился о поточности от низов
			OnComponentAdded( component );
			component.OnAddedToParent();
			ComponentAdded?.Invoke( this, component );
			component.AddedToParent?.Invoke( component );

			ComponentsChanged?.Invoke( this );
		}

		void SetUniqueName( Component component )
		{
			//!!!!threading?

			if( uniqueNameGenerator == null )
				uniqueNameGenerator = new UniqueNameGenerator();

			do
			{
				var componentName = uniqueNameGenerator.Get( component.BaseType.ToString() );
				if( GetComponent( componentName ) == null )
				{
					component.Name = componentName;
					break;
				}
			}
			while( true );
		}

		//no constructorParams because serialization of components
		/// <summary>
		/// Creates a child component.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="insertIndex"></param>
		/// <param name="enabled"></param>
		/// <param name="setUniqueName"></param>
		/// <returns></returns>
		public Component CreateComponent( Metadata.TypeInfo type, int insertIndex = -1, bool enabled = true, bool setUniqueName = false )
		{
			Component component = (Component)type.InvokeInstance( null );
			if( setUniqueName )
				SetUniqueName( component );
			component.Enabled = enabled;
			AddComponent( component, insertIndex );
			return component;
		}

		//no constructorParams because serialization of components
		/// <summary>
		/// Creates a child component.
		/// </summary>
		/// <param name="classType"></param>
		/// <param name="insertIndex"></param>
		/// <param name="enabled"></param>
		/// <param name="setUniqueName"></param>
		/// <returns></returns>
		public Component CreateComponent( Type classType, int insertIndex = -1, bool enabled = true, bool setUniqueName = false )
		{
			ConstructorInfo constructor = classType.GetConstructor( Array.Empty<Type>() );
			Component component = (Component)constructor.Invoke( Array.Empty<object>() );
			if( setUniqueName )
				SetUniqueName( component );
			component.Enabled = enabled;
			AddComponent( component, insertIndex );
			return component;
		}

		/// <summary>
		/// Creates a child component.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="insertIndex"></param>
		/// <param name="enabled"></param>
		/// <param name="setUniqueName"></param>
		/// <returns></returns>
		public T CreateComponent<T>( int insertIndex = -1, bool enabled = true, bool setUniqueName = false ) where T : Component
		{
			return (T)CreateComponent( typeof( T ), insertIndex, enabled, setUniqueName );
		}

		/// <summary>
		/// Removes a child component.
		/// </summary>
		/// <param name="component"></param>
		/// <param name="queued"></param>
		public void RemoveComponent( Component component, bool queued )
		{
			if( component.Parent != this )
				Log.Fatal( "Component: RemoveComponent: component.Parent != this." );
			component.RemoveFromParent( queued );
		}

		/// <summary>
		/// Removes all child components.
		/// </summary>
		/// <param name="queued"></param>
		public void RemoveAllComponents( bool queued )
		{
			foreach( var c in Components.ToArray() )
				c.RemoveFromParent( queued );
		}

		#endregion

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a collection of child components.
		/// </summary>
		public sealed class ComponentSet : ICollection<Component>, ICollection
		{
			Component owner;

			internal LinkedList<Component> linkedList;
			internal Dictionary<string, List<Component>> componentsByName;

			object _syncRoot;

			//

			internal ComponentSet( Component owner )
			{
				this.owner = owner;
			}

			/// <summary>
			/// Extracts the name and index from the path string.
			/// </summary>
			/// <param name="value"></param>
			/// <param name="name"></param>
			/// <param name="nameIndex"></param>
			/// <returns></returns>
			public static bool ParsePathNameWithIndex( string value, out string name, out int nameIndex )
			{
				if( value.Length > 0 && value[ 0 ] == '$' )
				{
					//!!!!потом может быть несколько итемов с данными

					if( value.Length > 2 && value[ 1 ] == '$' && value[ 2 ] == 'n' )
					{
						//!!!!slowly2

						int endIndex = value.IndexOf( '$', 3 );
						if( endIndex != -1 )
						{
							var indexStr = value.Substring( 3, endIndex - 3 );
							try
							{
								nameIndex = int.Parse( indexStr );
							}
							catch
							{
								name = "";
								nameIndex = 0;
								return false;
							}

							name = value.Substring( endIndex + 1 );
							return true;
						}
						else
						{
							name = "";
							nameIndex = 0;
							return false;
						}
					}
					else
					{
						name = value.Substring( 1 );
						nameIndex = 0;
						return true;
					}
				}
				else
				{
					name = value;
					nameIndex = 0;
					return true;
				}
			}

			/// <summary>
			/// Finds a child component by name and number in the case when there are several components with the specified name.
			/// </summary>
			/// <param name="name"></param>
			/// <param name="nameIndex"></param>
			/// <returns></returns>
			public Component GetByNameWithIndex( string name, int nameIndex )
			{
				if( componentsByName != null )
				{
					if( componentsByName.TryGetValue( name, out var list ) )
					{
						if( nameIndex >= 0 && nameIndex < list.Count )
							return list[ nameIndex ];
					}
				}
				else
				{
					int currentIndex = 0;
					foreach( var c in this )
					{
						if( c.Name == name )
						{
							if( currentIndex == nameIndex )
								return c;
							currentIndex++;
						}
					}
				}
				return null;
			}

			/// <summary>
			/// Finds a child component by name.
			/// </summary>
			/// <param name="name"></param>
			/// <param name="onlyEnabledInHierarchy"></param>
			/// <returns></returns>
			public Component GetByName( string name, bool onlyEnabledInHierarchy = false )
			{
				if( componentsByName != null )
				{
					if( componentsByName.TryGetValue( name, out var list ) )
					{
						for( int n = 0; n < list.Count; n++ )
						{
							var c = list[ n ];
							if( !onlyEnabledInHierarchy || c.EnabledInHierarchy )
								return c;
						}
					}
				}
				else
				{
					foreach( var c in this )
					{
						if( c.Name == name && ( !onlyEnabledInHierarchy || c.EnabledInHierarchy ) )
							return c;
					}
				}
				return null;
			}

			/// <summary>
			/// Finds a child component by name with the specified type.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="name"></param>
			/// <param name="onlyEnabledInHierarchy"></param>
			/// <returns></returns>
			public T GetByName<T>( string name, bool onlyEnabledInHierarchy = false ) //where T : Component
			{
				if( componentsByName != null )
				{
					if( componentsByName.TryGetValue( name, out var list ) )
					{
						for( int n = 0; n < list.Count; n++ )
						{
							var c = list[ n ];
							if( ( !onlyEnabledInHierarchy || c.EnabledInHierarchy ) && typeof( T ).IsAssignableFrom( c.GetType() ) )
								return (T)(object)c;
						}
					}
				}
				else
				{
					foreach( var c in this )
					{
						if( c.Name == name && ( !onlyEnabledInHierarchy || c.EnabledInHierarchy ) && typeof( T ).IsAssignableFrom( c.GetType() ) )
							return (T)(object)c;
					}
				}
				return (T)(object)null;
			}

			/// <summary>
			/// Finds a child component by path.
			/// </summary>
			/// <param name="nameOrPath"></param>
			/// <returns></returns>
			public Component GetByPath( string nameOrPath )
			{
				//!!!!поддержка вверх - ".."?

				//!!!!slowly

				//!!!!!slowly. можно если больше например 10 элементов, то создавать Dictionary

				//name as path
				int n = nameOrPath.IndexOfAny( new char[] { '/', '\\' } );
				if( n != -1 )
				{
					//!!!!slowly2

					string s = nameOrPath.Substring( 0, n );
					if( ParsePathNameWithIndex( s, out string name, out int nameIndex ) )
					{
						var c = GetByNameWithIndex( name, nameIndex );
						if( c != null )
							return c.Components[ nameOrPath.Substring( n + 1 ) ];

						//foreach( var c in this )
						//{
						//	if( ( !onlyEnabledInHierarchy || c.EnabledInHierarchy ) && c.Name == s )
						//		return c.Components[ nameOrNamePath.Substring( n + 1 ), false, onlyEnabledInHierarchy ];
						//}
					}
					return null;
				}
				else
				{
					if( ParsePathNameWithIndex( nameOrPath, out string name, out int nameIndex ) )
						return GetByNameWithIndex( name, nameIndex );
					return null;
				}
			}

			//!!!!было
			//public Component this[ string nameOrNamePath, bool checkChildren = false, bool onlyEnabledInHierarchy = false ]
			//{
			//	get { return Get( nameOrNamePath, checkChildren, onlyEnabledInHierarchy ); }
			//}

			/// <summary>
			/// Finds a child component by path.
			/// </summary>
			/// <param name="nameOrPath"></param>
			/// <returns></returns>
			public Component this[ string nameOrPath ]
			{
				get { return GetByPath( nameOrPath ); }
			}

			//!!!!!
			//!!!!TypeInfo
			//public Component this[ Type classType ]
			//{
			//	get
			//	{
			//		//!!!!
			//		Log.Fatal( "impl" );
			//		return null;
			//	}
			//}

			/// <summary>
			/// Gets the number of child components.
			/// </summary>
			public int Count
			{
				get { return linkedList != null ? linkedList.Count : 0; }
			}

			/// <summary>
			/// Is the collection read only. Always returns false.
			/// </summary>
			public bool IsReadOnly
			{
				get { return false; }
			}

			/// <summary>
			/// Gets the object to be synchronized for multi-threaded operation.
			/// </summary>
			public object SyncRoot
			{
				get
				{
					if( _syncRoot == null )
						Interlocked.CompareExchange( ref _syncRoot, new object(), null );
					return _syncRoot;
				}
			}

			/// <summary>
			/// Is the collection synchronized. Always returns false.
			/// </summary>
			public bool IsSynchronized
			{
				get { return false; }
			}

			/// <summary>
			/// Determines whether the object contains the specified child component.
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public bool Contains( Component key )
			{
				if( linkedList != null )
					return ReferenceEquals( linkedList, key.parentListNode.List );
				else
					return false;
			}

			/// <summary>
			/// Copies a list of child components to an array.
			/// </summary>
			/// <param name="array"></param>
			/// <param name="arrayIndex"></param>
			public void CopyTo( Component[] array, int arrayIndex )
			{
				int n = arrayIndex;
				foreach( Component v in this )
				{
					array[ n ] = v;
					n++;
				}
			}

			/// <summary>
			/// Copies a list of child components to an array.
			/// </summary>
			/// <param name="array"></param>
			/// <param name="arrayIndex"></param>
			public void CopyTo( Array array, int arrayIndex )
			{
				int n = arrayIndex;
				foreach( Component v in this )
				{
					array.SetValue( v, n );
					n++;
				}
			}

			/// <summary>
			/// Copies a list of child components to an array.
			/// </summary>
			/// <returns></returns>
			public Component[] ToArray()
			{
				Component[] array = new Component[ Count ];
				CopyTo( array, 0 );
				return array;
			}

			/// <summary>
			/// Returns an enumerator for traversing child components.
			/// </summary>
			/// <returns></returns>
			public IEnumerator<Component> GetEnumerator()
			{
				if( linkedList != null )
					return linkedList.GetEnumerator();
				else
					return Enumerable.Empty<Component>().GetEnumerator();
			}

			/// <summary>
			/// Returns an enumerator for traversing child components.
			/// </summary>
			/// <returns></returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				if( linkedList != null )
					return linkedList.GetEnumerator();
				else
					return Enumerable.Empty<Component>().GetEnumerator();
			}

			/// <summary>
			/// Returns a collection of child components in the reverse order.
			/// </summary>
			/// <returns></returns>
			public IEnumerable<Component> Reverse()
			{
				if( linkedList != null )
					return linkedList.Reverse();
				else
					return Enumerable.Empty<Component>();
			}

			//!!!!!так оставить?

			/// <summary>
			/// Adds a component as a child.
			/// </summary>
			/// <param name="item"></param>
			public void Add( Component item )
			{
				owner.AddComponent( item );
				//Log.Fatal( "Component: Components: Add: To add components need use AddComponent method." );
			}

			/// <summary>
			/// Removes all child components.
			/// </summary>
			public void Clear()
			{
				owner.RemoveAllComponents( false );
				//Log.Fatal( "Component: Components: Clear: To remove all components need use RemoveAllComponents method." );
			}

			/// <summary>
			/// Removes all child components.
			/// </summary>
			/// <param name="queued"></param>
			public void Clear( bool queued )
			{
				owner.RemoveAllComponents( queued );
				//Log.Fatal( "Component: Components: Clear: To remove all components need use RemoveAllComponents method." );
			}

			/// <summary>
			/// Removes a child component.
			/// </summary>
			/// <param name="item"></param>
			/// <returns></returns>
			public bool Remove( Component item )
			{
				owner.RemoveComponent( item, false );
				//Log.Fatal( "Component: Components: Clear: To remove components need use RemoveComponent method." );
				return true;
			}

			/// <summary>
			/// Removes a child component.
			/// </summary>
			/// <param name="item"></param>
			/// <param name="queued"></param>
			/// <returns></returns>
			public bool Remove( Component item, bool queued )
			{
				owner.RemoveComponent( item, queued );
				//Log.Fatal( "Component: Components: Clear: To remove components need use RemoveComponent method." );
				return true;
			}

			/// <summary>
			/// Moves the child component to another location relative to other children.
			/// </summary>
			/// <param name="item"></param>
			/// <param name="newPosition"></param>
			public void MoveTo( Component item, int newPosition )
			{
				//!!!!slowly2?

				if( linkedList == null )
					linkedList = new LinkedList<Component>();

				linkedList.Remove( item.parentListNode );

				bool wasAdded = false;
				int index = 0;
				for( var iterator = linkedList.First; iterator != null; iterator = iterator.Next )
				{
					if( index == newPosition )
					{
						linkedList.AddBefore( iterator, item.parentListNode );
						wasAdded = true;
						break;
					}
					index++;
				}
				if( !wasAdded )
					linkedList.AddLast( item.parentListNode );

				ComponentsByNameUpdateForMoveTo( item );

				owner.ComponentsChanged?.Invoke( owner );
			}

			/// <summary>
			/// Determines the position of the child components relative to other.
			/// </summary>
			/// <param name="item"></param>
			/// <returns></returns>
			public int IndexOf( Component item )
			{
				//!!!!slowly2?

				int n = 0;
				foreach( Component v in this )
				{
					if( v == item )
						return n;
					n++;
				}
				return -1;
			}

			//public ICollection<T> AsReadOnly()
			//{
			//	if( readOnlyCollection == null )
			//		readOnlyCollection = new ReadOnlyICollection<T>( this );
			//	return readOnlyCollection;
			//}

			//public void AddRange( IEnumerable<T> collection )
			//{
			//	foreach( T v in collection )
			//		Add( v );
			//}

			/// <summary>
			/// Finds a unique name by child components.
			/// </summary>
			/// <param name="prefix"></param>
			/// <param name="removePrefixEndNumbers"></param>
			/// <param name="startNumber"></param>
			/// <returns></returns>
			public string GetUniqueName( string prefix, bool removePrefixEndNumbers, int startNumber )
			{
				//!!!!slowly? option to use bool useUniqueNameGenerator

				string prefix2 = prefix;

				if( removePrefixEndNumbers )
				{
					//!!!!can be faster

					while( prefix2.Length > 0 )
					{
						var c = prefix2[ prefix2.Length - 1 ];
						if( c >= '0' && c <= '9' )
							prefix2 = prefix2.Substring( 0, prefix2.Length - 1 );
						else
							break;
					}

					prefix2 = prefix2.Trim();
				}

				for( int counter = startNumber; ; counter++ )
				{
					string name = prefix2 + " " + counter.ToString();
					if( GetByName( name ) == null )
						return name;
				}
			}

			internal void ComponentsByNameAdd( Component component, bool addLast )
			{
				//!!!!8 норм?
				if( componentsByName == null && linkedList != null && linkedList.Count > 8 )
				{
					componentsByName = new Dictionary<string, List<Component>>( 16 );
					foreach( var c in this )
					{
						if( !componentsByName.TryGetValue( c.name, out var list ) )
						{
							list = new List<Component>();
							componentsByName[ c.name ] = list;
						}
						list.Add( c );
					}
					return;
				}

				if( componentsByName != null )
				{
					//!!!!slowly2? сохранять индекс

					if( !componentsByName.TryGetValue( component.name, out var list ) )
					{
						list = new List<Component>();
						componentsByName[ component.name ] = list;
					}

					if( addLast )
					{
						list.Add( component );
					}
					else
					{
						list.Clear();
						foreach( var c in this )
						{
							if( c.name == component.name )
								list.Add( c );
						}
					}
				}
			}

			internal void ComponentsByNameRemove( Component component )
			{
				if( componentsByName != null )
				{
					if( componentsByName.TryGetValue( component.name, out var list ) )
					{
						for( int n = list.Count - 1; n >= 0; n-- )
						{
							if( ReferenceEquals( list[ n ], component ) )
							{
								list.RemoveAt( n );
								break;
							}
						}
						//list.Remove( component );

						if( list.Count == 0 )
							componentsByName.Remove( component.name );
					}
				}
			}

			void ComponentsByNameUpdateForMoveTo( Component component )
			{
				if( componentsByName != null )
				{
					if( componentsByName.TryGetValue( component.name, out var list ) )
					{
						//!!!!достаточно просто пересортировать список если так можно

						list.Clear();
						foreach( var c in this )
						{
							if( c.name == component.name )
								list.Add( c );
						}
					}
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//[Browsable( false )]
		//public long UIN
		//{
		//	get { return uin; }
		//}

		/// <summary>
		/// Gets the parent object.
		/// </summary>
		[Browsable( false )]
		public Component Parent
		{
			get { return parent; }
		}

		/// <summary>
		/// Gets the parent root object.
		/// </summary>
		[Browsable( false )]
		public Component ParentRoot
		{
			get
			{
				//!!!!!!slowly. когда включено EnabledInHierarchy, то можно по идее локально root хранить
				Component c = this;
				while( c.Parent != null )
					c = c.Parent;
				return c;
			}
		}

		/// <summary>
		/// The name of the component.
		/// </summary>
		[Serialize]
		public virtual string Name
		{
			get { return name; }
			set
			{
				if( name == value )
					return;

				parent?.components.ComponentsByNameRemove( this );
				name = value;
				parent?.components.ComponentsByNameAdd( this, false );

				NameChanged?.Invoke( this );
				//ComponentUtils.PerformAllComponents_NameChanged( this );
			}
		}
		/// <summary>
		/// Occurs when component name is changed.
		/// </summary>
		public event Action<Component> NameChanged;

		/// <summary>
		/// Sets the display of the on-screen label of the object in the scene editor.
		/// </summary>
		[DefaultValue( ScreenLabelEnum.Auto )]
		public Reference<ScreenLabelEnum> ScreenLabel
		{
			get { if( _screenLabel.BeginGet() ) ScreenLabel = _screenLabel.Get( this ); return _screenLabel.value; }
			set { if( _screenLabel.BeginSet( ref value ) ) { try { ScreenLabelChanged?.Invoke( this ); } finally { _screenLabel.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScreenLabel"/> property value changes.</summary>
		public event Action<Component> ScreenLabelChanged;
		ReferenceField<ScreenLabelEnum> _screenLabel = ScreenLabelEnum.Auto;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!queued = true. в других местах тоже
		/// <summary>
		/// Detaches the object from its parent.
		/// </summary>
		/// <param name="queued"></param>
		public virtual void RemoveFromParent( bool queued )
		{
			if( Parent == null )
				return;

			var controller = ParentRoot.HierarchyController;

			if( queued && controller != null )
			{
				if( !controller.objectsDeletionQueue.Contains( this ) )
					controller.objectsDeletionQueue.Add( this );
				removeFromParentQueued = true;
			}
			else
			{
				if( controller != null )
					controller.objectsDeletionQueue.Remove( this );

				var oldParent = parent;

				_UpdateEnabledInHierarchy( true );

				parent.components.ComponentsByNameRemove( this );
				if( parent.components.linkedList != null )
					parent.components.linkedList.Remove( parentListNode );
				parent = null;

				//!!!!!нужны ли эвенты до начала отсоединения? как тогда они вместе с этими будут существовать?

				//!!!!!порядок норм?
				RemovedFromParent?.Invoke( this, oldParent );
				oldParent.ComponentRemoved?.Invoke( oldParent, this );
				OnRemovedFromParent( oldParent );
				oldParent.OnComponentRemoved( this );

				removeFromParentQueued = false;

				oldParent.ComponentsChanged?.Invoke( oldParent );
			}
		}

		/// <summary>
		/// Whether the object is placed in the detach queue from the parent.
		/// </summary>
		[Browsable( false )]
		public bool RemoveFromParentQueued
		{
			get { return removeFromParentQueued; }
		}

		//!!!!префикс какой-то в именах? по сути для юзера группироваться будет по своему всё равно

		//!!!!new
		/// <summary>
		/// Occurs when the set of children is changed.
		/// </summary>
		public event Action<Component> ComponentsChanged;

		public delegate void ComponentAddedDelegate( Component sender, Component component );
		/// <summary>
		/// Occurs when a child component is added.
		/// </summary>
		public event ComponentAddedDelegate ComponentAdded;

		/// <summary>
		/// Called when a child component is added.
		/// </summary>
		protected virtual void OnComponentAdded( Component component )
		{
		}

		public delegate void ComponentRemovedDelegate( Component sender, Component component );
		/// <summary>
		/// Occurs when a child component is removed.
		/// </summary>
		public event ComponentRemovedDelegate ComponentRemoved;

		/// <summary>
		/// Called when a child component is removed.
		/// </summary>
		protected virtual void OnComponentRemoved( Component component )
		{
		}

		public delegate void AddedToParentDelegate( Component sender );
		/// <summary>
		/// Occurs when a component is added to the parent.
		/// </summary>
		public event AddedToParentDelegate AddedToParent;

		//!!!!!!по сути нужен эвент что в иерархию добавили. а как же OnEnabledInHierarchyChanged?
		/// <summary>
		/// Called when a component is added to the parent.
		/// </summary>
		protected virtual void OnAddedToParent()
		{
		}

		public delegate void RemovedFromParentDelegate( Component sender, Component oldParent );
		/// <summary>
		/// Occurs when a component is removed from the parent.
		/// </summary>
		public event RemovedFromParentDelegate RemovedFromParent;

		/// <summary>
		/// Called when a component is removed from the parent.
		/// </summary>
		/// <param name="oldParent"></param>
		protected virtual void OnRemovedFromParent( Component oldParent )
		{
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!!что тут про поточность?
		//в такой конструкции таки нужен On метод. потому что эвент нужно вызывать позже
		/// <summary>
		/// Called when preloading the resources used by the object.
		/// </summary>
		protected virtual void OnPreloadResources() { }
		/// <summary>
		/// Occurs when preloading the resources used by the object.
		/// </summary>
		public event Action<Component> PreloadResourcesEvent;

		//!!!!вызывать
		//!!!!threading
		/// <summary>
		/// Preload resources used by the object.
		/// </summary>
		public void PreloadResources()
		{
			foreach( Component c in GetComponents() )
			{
				//!!!!!как проверять что компонента включена, не в очереди для удаления?
				//public bool IsEnabledNotQueuedToRemove()

				if( !c.RemoveFromParentQueued )
					c.PreloadResources();
			}

			OnPreloadResources();
			PreloadResourcesEvent?.Invoke( this );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Gets the object hierarchy control controller.
		/// </summary>
		[Browsable( false )]
		public ComponentHierarchyController HierarchyController
		{
			get { return hierarchyController; }
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//[Browsable( false )]
		//public ReferenceToThisObjectClass ReferenceToThisObject
		//{
		//	get
		//	{
		//		рутовый может ссылаться на resource instance. тогда не будет в мире двух ссылок на один объект

		//		if( referenceToThisObject == null )
		//		{
		//			referenceToThisObject = new ReferenceToThisObjectClass();
		//			referenceToThisObject.component = this;
		//		}
		//		return referenceToThisObject;
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public delegate void GetSettingsEventDelegate( Component sender, SettingsProvider provider, int objectIndex );
		//public event GetSettingsEventDelegate GetSettingsEvent;

		////!!!!!надо ли OnGetSettings и GetSettings. другие места тоже также
		//public virtual void OnGetSettings( SettingsProvider provider, int objectIndex )
		//{
		//	//!!!!всегда ли надо
		//	//if( group.objects.Length == 1 )//!!!!!!
		//	//if( objectIndex == 0 )//!!!!!
		//	//	provider.AddCell( typeof( SettingsHeader_ObjectInfo ), true );

		//	provider.AddCell( typeof( SettingsCell_Properties ), true );

		//	//if( objectIndex == 0 )//!!!!!
		//	//{
		//	//	//!!!!!!
		//	//	if( provider.showComponentsTree )
		//	//		provider.AddCell( typeof( SettingsCell_Components ) );
		//	//}
		//}

		//public void GetSettings( SettingsProvider provider, int objectIndex )
		//{
		//	OnGetSettings( provider, objectIndex );
		//	GetSettingsEvent?.Invoke( this, provider, objectIndex );
		//}

		////!!!!name
		//public delegate void GetSettingsPropertyEventDelegate( Component sender, SettingsProvider provider, Metadata.Property property, ref bool skip );
		//public event GetSettingsPropertyEventDelegate GetSettingsPropertyEvent;

		//protected virtual void OnGetSettingsProperty( SettingsProvider provider, Metadata.Property property, ref bool skip )
		//{
		//	//!!!!!
		//	//!!!!по именам проверять тупняк как-то
		//	//if( property.Name == "ProvideAsTypeEnum" || property.Name == "ProvideAsTypeEnumValues" )
		//	//{
		//	//	if( !ProvideAsType )
		//	//		skip = true;
		//	//}
		//}

		//public void GetSettingsProperty( SettingsProvider provider, Metadata.Property property, ref bool skip )
		//{
		//	OnGetSettingsProperty( provider, property, ref skip );
		//	GetSettingsPropertyEvent?.Invoke( this, provider, property, ref skip );
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Gets the base type of the object.
		/// </summary>
		[Browsable( false )]
		public Metadata.TypeInfo BaseType
		{
			get
			{
				//'basedOnType' can be initialized from another place. as example during cloning from the type.
				if( baseType == null )
					baseType = MetadataManager.GetTypeOfNetType( GetType() );
				return baseType;
			}
			//!!!!
			internal set
			{
				baseType = value;
				//!!!!
			}
		}

		/// <summary>
		/// Called upon receipt of the object's metadata members.
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<Metadata.Member> OnMetadataGetMembers()//Metadata.GetMembersContext context )
		{
			foreach( var m in BaseType.MetadataGetMembers( getMemberContextNoFilter ) )
				yield return m;

			//virtual members
			VirtualMembersUpdate();
			if( virtualMembers != null )
			{
				foreach( var m in virtualMembers )
					yield return m;
			}
		}

		public delegate void MetadataGetMembersDelegate( Component sender, Metadata.GetMembersContext context, IList<Metadata.Member> members );
		/// <summary>
		/// Occurs upon receipt of the object's metadata members.
		/// </summary>
		public event MetadataGetMembersDelegate MetadataGetMembersEvent;

		static Metadata.GetMembersContext getMemberContextNoFilter = new Metadata.GetMembersContext( false );

		/// <summary>
		/// Returns the object metadata members.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context = null )
		{
			foreach( var m in OnMetadataGetMembers() )
			//foreach( var m in OnMetadataGetMembers( context ) )
			{
				bool skip = false;
				if( context == null || context.filter )
					MetadataGetMemberFilterInternal( context, m, ref skip );
				if( !skip )
					yield return m;
			}

			//event
			if( MetadataGetMembersEvent != null )
			{
				List<Metadata.Member> list = new List<Metadata.Member>();
				MetadataGetMembersEvent( this, getMemberContextNoFilter, list );
				//MetadataGetMembersEvent( this, context, list );
				foreach( var m in list )
				{
					bool skip = false;
					if( context == null || context.filter )
						MetadataGetMemberFilterInternal( context, m, ref skip );
					if( !skip )
						yield return m;
				}
			}
		}

		/// <summary>
		/// Called when the object metadata member is received by name.
		/// </summary>
		/// <param name="signature"></param>
		/// <returns></returns>
		protected virtual Metadata.Member OnMetadataGetMemberBySignature( string signature )//, Metadata.GetMembersContext context )
		{
			//virtual members
			VirtualMembersUpdate();
			if( virtualMemberBySignature != null && virtualMemberBySignature.TryGetValue( signature, out Metadata.Member m ) )
			{
				//bool skip = false;
				//MetadataGetMemberFilterInternal( context, m, ref skip );
				//if( !skip )
				return m;
			}

			//base type
			var m2 = BaseType.MetadataGetMemberBySignature( signature, getMemberContextNoFilter );// context );
			if( m2 != null )
				return m2;

			return null;
		}

		public delegate void MetadataGetMemberBySignatureDelegate( Component sender, string signature, Metadata.GetMembersContext context,
			ref Metadata.Member member );
		/// <summary>
		/// Occurs when the object metadata member is received by name.
		/// </summary>
		public event MetadataGetMemberBySignatureDelegate MetadataGetMemberBySignatureEvent;

		/// <summary>
		/// Returns the object metadata member by name.
		/// </summary>
		/// <param name="signature"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public Metadata.Member MetadataGetMemberBySignature( string signature, Metadata.GetMembersContext context = null )
		{
			var m = OnMetadataGetMemberBySignature( signature );//, context );
			if( m != null )
			{
				bool skip = false;
				if( context == null || context.filter )
					MetadataGetMemberFilterInternal( context, m, ref skip );
				if( !skip )
					return m;
			}

			//event
			MetadataGetMemberBySignatureEvent?.Invoke( this, signature, context, ref m );
			if( m != null )
			{
				bool skip = false;
				if( context == null || context.filter )
					MetadataGetMemberFilterInternal( context, m, ref skip );
				if( !skip )
					return m;
			}

			return null;
		}

		/// <summary>
		/// Called when filtering receive members of the object metadata.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="member"></param>
		/// <param name="skip"></param>
		protected virtual void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
		}

		public delegate void MetadataGetMembersFilterEventDelegate( Component sender, Metadata.GetMembersContext context, Metadata.Member member,
			ref bool skip );
		/// <summary>
		/// Occurs when filtering receive members of the object metadata.
		/// </summary>
		public event MetadataGetMembersFilterEventDelegate MetadataGetMembersFilterEvent;

		void MetadataGetMemberFilterInternal( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			OnMetadataGetMembersFilter( context, member, ref skip );
			if( skip )
				return;
			MetadataGetMembersFilterEvent?.Invoke( this, context, member, ref skip );
			if( skip )
				return;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Whether the object is disposed.
		/// </summary>
		[Browsable( false )]
		public bool Disposed
		{
			get { return disposed.Get(); }
		}

		/// <summary>
		/// Occurs when the object is disposed.
		/// </summary>
		public event Action<Component> DisposeEvent;
		/// <summary>
		/// Called when the object is disposed.
		/// </summary>
		protected virtual void OnDispose() { }

		internal virtual void OnDispose_After()
		{
			//!!!!!так?

			//!!!!вызывать?
			//Enabled = false;

			RemoveFromParent( false );

			var controller = HierarchyController;
			if( controller != null )
			{
				controller.ProcessDelayedOperations();
				controller.HierarchyEnabled = false;
			}

			while( Components.Count != 0 )
			{
				foreach( var child in GetComponents() )
					child.Dispose();
			}

			if( controller != null )
				controller.ProcessDelayedOperations();

			if( controller != null )
			{
				var ins = controller.CreatedByResource;
				if( ins != null )
					ins.Dispose();
			}

			//!!!!эвенты чистить?
		}

		/// <summary>
		/// Detaches the object from the hierarchy and deletes the object data.
		/// </summary>
		public void Dispose()
		{
			if( !disposed.Set( true ) )
			{
				DisposeEvent?.Invoke( this );
				OnDispose();
				OnDispose_After();
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		////here?
		//public delegate void AllComponents_OverrideToStringDelegate( Component sender, Component component );
		//public static event AllComponents_OverrideToStringDelegate AllComponents_OverrideToString;

		/// <summary>
		/// Returns the name and type of the object as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			//!!!!!!возможность всем переопределить. или только в этой иерархии. ну это уж определить и так можно

			string str;

			str = Name;//!!!! DisplayName;
			if( string.IsNullOrEmpty( str ) )
			{
				var typeName = BaseType.ToString();

				if( !string.IsNullOrEmpty( Name ) )
					str = string.Format( "{0} - {1}", Name, typeName );
				else
					str = typeName;
			}

			return str;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!а надо ли. мешает же адресации
		//Reference<string> displayName = "";
		//[Serialize]
		//public virtual Reference<string> DisplayName
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( displayName.GetByReference ) )
		//			DisplayName = displayName.GetValue( this );
		//		return displayName;
		//	}
		//	set
		//	{
		//		if( displayName == value )
		//			return;
		//		displayName = value;
		//		DisplayNameChanged?.Invoke( this );
		//		ComponentUtils.PerformAllComponents_DisplayNameChanged( this );
		//	}
		//}
		//public event Action<Component> DisplayNameChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		internal void GetBaseTypeIndex( out int hierarchyIndex, out string name, out int nameIndex )
		{
			//!!!!bool?


			//bool baseTypeIsResourceInstance = false;
			//if( Parent != null )
			//{
			//	//!!!!так?

			//	var root = Parent.ParentRoot;

			//	if( root != null && root.HierarchyController != null && root.HierarchyController.CreatedByResource != null &&
			//		 root.HierarchyController.CreatedByResource.InstanceType == Resource.InstanceType.Resource )
			//	{
			//		baseTypeIsResourceInstance = true;
			//	}
			//}

			//if( baseTypeIsResourceInstance )
			//{
			//!!!!slowly

			hierarchyIndex = 0;
			var type = BaseType;
			while( type != null )
			{
				hierarchyIndex++;
				type = type.BaseType;
			}

			name = this.name;

			nameIndex = 0;
			if( parent != null )
			{
				if( parent.components.componentsByName != null )
				{
					//!!!!slowly2

					if( parent.components.componentsByName.TryGetValue( name, out var list ) )
					{
						var index = list.LastIndexOf( this );
						//var index = list.IndexOf( this );
						if( index != -1 )
							nameIndex = index;
					}
				}
				else
				{
					foreach( var child in parent.Components )
					{
						if( child == this )
							break;
						if( child.name == name )
							nameIndex++;
					}
				}
			}

			//return true;
			//}

			//hierarchyIndex = 0;
			//name = "";
			//nameIndex = 0;
			//return false;
		}

		internal int GetNameIndexFromParent()
		{
			int nameIndex = 0;
			if( parent != null )
			{
				if( parent.components.componentsByName != null )
				{
					//!!!!slowly2. кешировать индекс?

					if( parent.components.componentsByName.TryGetValue( name, out var list ) )
					{
						var index = list.LastIndexOf( this );
						//var index = list.IndexOf( this );
						if( index != -1 )
							nameIndex = index;
					}
				}
				else
				{
					//!!!!slowly2. можно без foreach.
					//!!!!!!много где еще так
					foreach( var child in parent.Components )
					{
						if( child == this )
							break;
						if( child.name == name )
							nameIndex++;
					}
				}
			}
			return nameIndex;
		}

		/// <summary>
		/// Returns the path to the object from the parent.
		/// </summary>
		/// <returns></returns>
		public string GetPathFromParent()
		{
			StringBuilder result = new StringBuilder( Name.Length + 2 );
			result.Append( '$' );
			var nameIndex = GetNameIndexFromParent();
			if( nameIndex != 0 )
				result.AppendFormat( "$n{0}$", nameIndex );
			result.Append( Name );
			return result.ToString();
		}

		/// <summary>
		/// Returns the object's access path from the root object.
		/// </summary>
		/// <param name="displayableForUser"></param>
		/// <returns></returns>
		public string GetPathFromRoot( bool displayableForUser = false )
		{
			if( Parent != null )
			{
				//!!!!!slowly?

				Stack<string> names = new Stack<string>();
				var c = this;
				do
				{
					names.Push( c.GetPathFromParent() );
					c = c.Parent;
				} while( c.Parent != null );

				var b = new StringBuilder();
				while( names.Count != 0 )
				{
					if( b.Length != 0 )
						b.Append( '\\' );
					b.Append( names.Pop() );
				}

				if( displayableForUser )
					b.Replace( "$", "" );

				return b.ToString();
			}
			else
				return "";
		}

		//public string GetNamePathToAccessFromRootDisplayableToUser()
		//{
		//	return GetPathFromRoot().Replace( "$", "" );
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		//[Serialize]
		//[DefaultValue( Metadata.TypeClassification.Component )]
		////[RefreshProperties( RefreshProperties.All )]//!!!!!!
		//public Metadata.TypeClassification Classification
		//{
		//	get { return classification; }
		//	set
		//	{
		//		if( classification == value )
		//			return;
		//		classification = value;

		//		//!!!!
		//	}
		//}

		//!!!!internal?
		//!!!!name
		[Browsable( false )]
		internal virtual bool TypeOnly
		{
			get { return false; }
		}

		////!!!!browsable?
		//[Serialize]
		//[DefaultValue( false )]
		//public bool TypeOnly
		//{
		//	get { return typeOnly; }
		//	set { typeOnly = value; }
		//}

		//!!!!
		//[Serialize]
		//[DefaultValue( Metadata.AccessLevel.Public )]
		//public Metadata.AccessLevel TypeAccessLevel
		//{
		//	get { return typeAccessLevel; }
		//	set
		//	{
		//		if( typeAccessLevel == value )
		//			return;
		//		typeAccessLevel = value;

		//		//!!!!
		//	}
		//}

		//[Serialize]
		//[DefaultValue( false )]
		//[RefreshProperties( RefreshProperties.All )]//!!!!!!
		//public bool ProvideAsType
		//{
		//	get { return provideAsType; }
		//	set { provideAsType = value; }
		//}

		//[Serialize]
		//[DefaultValue( false )]
		//[RefreshProperties( RefreshProperties.All )]//!!!!!!
		//public bool ProvideAsTypeEnum
		//{
		//	get { return provideAsTypeEnum; }
		//	set { provideAsTypeEnum = value; }
		//}

		////!!!!!как редактировать?
		//[Serialize]
		//public EDictionary<string, long> ProvideAsTypeEnumValues
		//{
		//	get { return provideAsTypeEnumValues; }
		//	set { provideAsTypeEnumValues = value; }
		//}

		static bool CompareEnums( EDictionary<string, long> d1, EDictionary<string, long> d2 )
		{
			if( d1 == null || d2 == null )
				return d1 == null && d2 == null;
			if( ReferenceEquals( d1, d2 ) )
				return true;

			if( d1.Count != d2.Count )
				return false;

			var enum1 = d1.GetEnumerator();
			var enum2 = d2.GetEnumerator();
			while( enum1.MoveNext() )
			{
				enum2.MoveNext();
				if( enum1.Current.Key != enum2.Current.Key || enum1.Current.Value != enum2.Current.Value )
					return false;
			}

			return true;
		}

		//!!!!
		internal Metadata.TypeInfo GetProvidedTypeInternal( EDictionary<string, long> enumElements, bool enumFlags )
		{
			//!!!!slowly

			//!!!!когда не включен? когда включен?

			//!!!!так? (ParentRoot != null && ParentRoot.HierarchyController != null )
			//!!!!if( ProvideAsType || ( ParentRoot == this && ParentRoot.HierarchyController != null ) )
			if( providedTypeAllow )
			{
				var namePath = GetPathFromRoot();

				//!!!!?
				Resource owner = null;
				if( ParentRoot != null && ParentRoot.HierarchyController != null && ParentRoot.HierarchyController.CreatedByResource != null )
					owner = ParentRoot.HierarchyController.CreatedByResource.Owner;

				if( owner != null )//!!!!null не может быть?
				{
					string name;
					if( !string.IsNullOrEmpty( namePath ) && namePath != "$" )
						name = string.Format( "{0}|{1}", owner.Name, namePath );
					else
						name = owner.Name;

					//!!!!
					Metadata.TypeClassification classification = Metadata.TypeClassification.Component;
					if( enumElements != null )
						classification = Metadata.TypeClassification.Enumeration;

					//!!!!что еще проверять. а может что-то тут не надо проверять
					//!!!!!check
					if( providedTypeCached == null ||
						providedTypeCached.Name != name ||
						providedTypeCached.BaseType != BaseType ||
						providedTypeCached.Classification != classification ||
						!CompareEnums( providedTypeCached.EnumElements, enumElements ) ||
						providedTypeCached.EnumFlags != enumFlags ||
						providedTypeCached.Resource != owner )
					{
						//!!!!!
						string displayName2 = name;

						providedTypeCached = new Metadata.ComponentTypeInfo( name, displayName2, BaseType, classification, enumElements, enumFlags,
							owner, namePath, this );
					}
				}
				else
					providedTypeCached = null;
			}
			//!!!!
			//else
			//	providedTypeCached = null;

			return providedTypeCached;
		}

		/// <summary>
		/// Returns a type when the object is used as a type.
		/// </summary>
		/// <returns></returns>
		public virtual Metadata.TypeInfo GetProvidedType()
		{
			return GetProvidedTypeInternal( null, false );


			////!!!!когда не включен? когда включен?

			////!!!!так? (ParentRoot != null && ParentRoot.HierarchyController != null )
			////!!!!if( ProvideAsType || ( ParentRoot == this && ParentRoot.HierarchyController != null ) )
			//{
			//	var namePath = GetNamePathToAccessFromRoot();

			//	//!!!!?
			//	Resource owner = null;
			//	if( ParentRoot != null && ParentRoot.HierarchyController != null && ParentRoot.HierarchyController.CreatedByResource != null )
			//		owner = ParentRoot.HierarchyController.CreatedByResource.Owner;

			//	if( owner == null )
			//	{
			//		//!!!!!
			//		Log.Fatal( "impl" );
			//	}

			//	string name;
			//	if( !string.IsNullOrEmpty( namePath ) )
			//		name = string.Format( "{0}:{1}", owner.Name, namePath );
			//	else
			//		name = owner.Name;

			//	//!!!!!ошибка если null
			//	EDictionary<string, long> enumValues = new EDictionary<string, long>();
			//	//EDictionary<string, long> enumValues = null;

			//	//!!!!что еще проверять. а может что-то тут не надо проверять
			//	//!!!!!check
			//	if( providedTypeCached == null || providedTypeCached.Name != name || providedTypeCached.BaseType != BaseType ||
			//		providedTypeCached.Classification != Classification || providedTypeCached.EnumValues != enumValues ||
			//		providedTypeCached.Resource != owner )
			//	{
			//		//!!!!!
			//		string displayName2 = name;

			//		providedTypeCached = new Metadata.ComponentTypeInfo( name, displayName2, BaseType, Classification, enumValues,
			//			owner, namePath, this );
			//	}
			//	else
			//	{
			//		providedTypeCached = null;
			//	}
			//}
			////else
			////	providedTypeCached = null;

			//return providedTypeCached;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		//[Serialize]
		//[DefaultValue( false )]
		//public bool ShowComponentsInSettings
		//{
		//	get { return showComponentsInSettings; }
		//	set
		//	{
		//		if( showComponentsInSettings == value )
		//			return;
		//		showComponentsInSettings = value;

		//		//!!!!!
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		////!!!!
		////!!!!name: ExpectedComponents, ExpectedChildren
		////!!!!тут? какие еще убрать из Component
		//public class ExpectedComponentItem
		//{
		//	string name;
		//	bool optional;
		//	Metadata.TypeInfo expectedType;

		//	//

		//	public ExpectedComponentItem( string name, bool optional, Metadata.TypeInfo expectedType = null )
		//	{
		//		this.name = name;
		//		this.optional = optional;
		//		this.expectedType = expectedType;
		//	}

		//	public string Name
		//	{
		//		get { return name; }
		//	}

		//	public bool Optional
		//	{
		//		get { return optional; }
		//	}

		//	public Metadata.TypeInfo ExpectedType
		//	{
		//		get { return expectedType; }
		//	}
		//}

		////!!!!или через yield
		//protected virtual void OnGetExpectedComponents( List<ExpectedComponentItem> list )
		//{
		//}

		//public delegate void GetExpectedComponentsEventDelegate( Component sender, List<ExpectedComponentItem> list );
		//public event GetExpectedComponentsEventDelegate GetExpectedComponentsEvent;

		//public IList<ExpectedComponentItem> GetExpectedComponents()
		//{
		//	var list = new List<ExpectedComponentItem>( 8 );
		//	OnGetExpectedComponents( list );
		//	GetExpectedComponentsEvent?.Invoke( this, list );
		//	return list;
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		//[Serialize]
		//[Browsable( false )]
		//public EDictionary<string, string> EditorSettings
		//{
		//	get { return editorSettings; }
		//	set { editorSettings = value; }
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		internal virtual void OnUpdate_Before( float delta ) { }
		/// <summary>
		/// Called during the update process of all objects.
		/// </summary>
		/// <param name="delta"></param>
		protected virtual void OnUpdate( float delta ) { }
		public delegate void UpdateEventDelegate( Component sender, float delta );
		/// <summary>
		/// Occurs during the update process of all objects.
		/// </summary>
		public event UpdateEventDelegate UpdateEvent;

		internal void PerformUpdate( float delta )
		{
			OnUpdate_Before( delta );

			//children
			int componentsCount = components.Count;
			if( componentsCount != 0 )
			{
				var list = new List<Component>( componentsCount );
				foreach( var c in components )
				{
					if( c.EnabledInHierarchy && !c.RemoveFromParentQueued )
						list.Add( c );
				}
				//GetComponents( false, false, true, delegate ( Component c )
				//{
				//	list.Add( c );
				//} );
				for( int n = 0; n < list.Count; n++ )
				{
					var c = list[ n ];
					if( c.EnabledInHierarchy && !c.RemoveFromParentQueued )//second check if changed during enumeration
						c.PerformUpdate( delta );
				}
			}
			//foreach( var c in GetComponents( false, false, true ) )
			//{
			//	if( c.EnabledInHierarchy && !c.RemoveFromParentQueued )//second check if changed during enumeration
			//		c.PerformUpdate( delta );
			//}

			//!!!!тут?
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
				MethodInvokeUpdate( delta );

			OnUpdate( delta );
			UpdateEvent?.Invoke( this, delta );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		internal virtual void OnSimulationStep_Before() { }
		/// <summary>
		/// Called during the simulation step.
		/// </summary>
		protected virtual void OnSimulationStep() { }
		/// <summary>
		/// Occurs during the simulation step.
		/// </summary>
		public event Action<Component> SimulationStep;

		internal void PerformSimulationStep()
		{
			OnSimulationStep_Before();

			//children
			int componentsCount = components.Count;
			if( componentsCount != 0 )
			{
				var list = new List<Component>( componentsCount );
				foreach( var c in components )
				{
					if( c.EnabledInHierarchy && !c.RemoveFromParentQueued )
						list.Add( c );
				}
				//GetComponents( false, false, true, delegate ( Component c )
				//{
				//	list.Add( c );
				//} );
				for( int n = 0; n < list.Count; n++ )
				{
					var c = list[ n ];
					if( c.EnabledInHierarchy && !c.RemoveFromParentQueued )//second check if changed during enumeration
						c.PerformSimulationStep();
				}
			}
			//foreach( var c in GetComponents( false, false, true ) )
			//{
			//	if( c.EnabledInHierarchy && !c.RemoveFromParentQueued )//second check if changed during enumeration
			//		c.PerformSimulationStep();
			//}

			//!!!!тут?
			MethodInvokeUpdate( Time.SimulationDelta );

			OnSimulationStep();

			SimulationStep?.Invoke( this );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void VirtualMembersUpdate()
		{
			//!!!!!threading

			//!!!!проверить не всё время обновляется ли

			if( virtualMembersNeedUpdate )
			{
				virtualMembersNeedUpdate = false;
				virtualMembers = null;
				virtualMemberBySignature = null;

				foreach( var child in GetComponents<Component_Member>() )
				{
					if( child.Enabled )
						child.UpdateParentVirtualMembers( ref virtualMembers, ref virtualMemberBySignature );
				}
			}
		}

		internal void VirtualMembersNeedUpdate()
		{
			virtualMembersNeedUpdate = true;
		}

		[Browsable( false )]
		internal Dictionary<string, object> VirtualMemberValues
		{
			get { return virtualMemberValues; }
			set { virtualMemberValues = value; }
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Whether the object is read only in the editor.
		/// </summary>
		[Browsable( false )]
		[Serialize]
		[DefaultValue( false )]
		public virtual bool EditorReadOnly
		{
			get { return editorReadOnly; }
			set { editorReadOnly = value; }
		}
		bool editorReadOnly;

		/// <summary>
		/// Whether the object is read-only in the editor with respect to parent objects.
		/// </summary>
		[Browsable( false )]
		public bool EditorReadOnlyInHierarchy
		{
			get
			{
				if( EditorReadOnly )
					return true;
				if( Parent != null )
					return Parent.EditorReadOnlyInHierarchy;
				else
					return false;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Finds the parent or self of the specified type.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <returns></returns>
		public Component FindThisOrParent( Metadata.TypeInfo componentClass )
		{
			var c = this;
			while( c != null )
			{
				if( componentClass.IsAssignableFrom( c.BaseType ) )
					return c;
				c = c.Parent;
			}
			return null;
		}

		/// <summary>
		/// Finds the parent or self of the specified type.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <returns></returns>
		public Component FindThisOrParent( Type componentClass )
		{
			var c = this;
			while( c != null )
			{
				if( componentClass.IsAssignableFrom( c.GetType() ) )
					return c;
				c = c.Parent;
			}
			return null;
		}

		/// <summary>
		/// Finds the parent or self of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T FindThisOrParent<T>() where T : class//Component
		{
			var c = this;
			while( c != null )
			{
				var p2 = c as T;
				if( p2 != null )
					return p2;
				c = c.Parent;
			}
			return null;
		}

		/// <summary>
		/// Finds the parent of the specified type.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <returns></returns>
		public Component FindParent( Metadata.TypeInfo componentClass )
		{
			var c = Parent;
			while( c != null )
			{
				if( componentClass.IsAssignableFrom( c.BaseType ) )
					return c;
				c = c.Parent;
			}
			return null;
		}

		/// <summary>
		/// Finds the parent of the specified type.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <returns></returns>
		public Component FindParent( Type componentClass )
		{
			var c = Parent;
			while( c != null )
			{
				if( componentClass.IsAssignableFrom( c.GetType() ) )
					return c;
				c = c.Parent;
			}
			return null;
		}

		//!!!!!!для сцен тоже такое юзать, чтобы не было ограничение на её рутовой?
		/// <summary>
		/// Finds the parent of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T FindParent<T>() where T : class//Component
		{
			var c = Parent;
			while( c != null )
			{
				var p2 = c as T;
				if( p2 != null )
					return p2;
				c = c.Parent;
			}
			return null;
		}

		/// <summary>
		/// Gets all parents of the component.
		/// </summary>
		/// <param name="makeOrderFromTopToBottom"></param>
		/// <returns></returns>
		public ICollection<Component> GetAllParents( bool makeOrderFromTopToBottom )
		{
			var list = new List<Component>();
			var current = Parent;
			while( current != null )
			{
				list.Add( current );
				current = current.Parent;
			}
			if( makeOrderFromTopToBottom )
				list.Reverse();
			return list;//.ToArray();
		}

		/// <summary>
		/// Whether to show the object in the editor.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Browsable( false )]
		public Reference<bool> DisplayInEditor
		{
			get { if( _displayInEditor.BeginGet() ) DisplayInEditor = _displayInEditor.Get( this ); return _displayInEditor.value; }
			set { if( _displayInEditor.BeginSet( ref value ) ) { try { DisplayInEditorChanged?.Invoke( this ); } finally { _displayInEditor.EndSet(); } } }
		}
		/// <summary>
		/// Occurs when value of <see cref="DisplayInEditor"/> property is changed.
		/// </summary>
		public event Action<Component> DisplayInEditorChanged;
		ReferenceField<bool> _displayInEditor = true;

		/////////////////////////////////////////

		//!!!!так?
		/// <summary>
		/// Called when the object is created in the editor. Designed to configure the initial state.
		/// </summary>
		/// <param name="createdFromNewObjectWindow"></param>
		public virtual void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false ) { }
		/// <summary>
		/// Called during object creation in the editor. Designed to configure the initial state.
		/// </summary>
		public virtual void NewObjectSetDefaultConfigurationUpdate() { }

		/////////////////////////////////////////

		/// <summary>
		/// Object settings when used as a type.
		/// </summary>
		[Browsable( false )]
		[Serialize]
		public string[] TypeSettingsPrivateObjects
		{
			get;
			set;
		}

		/////////////////////////////////////////

		//[Serialize]
		//[DefaultValue( "" )]
		//public string Tag { get; set; } = "";

		/////////////////////////////////////////

		//!!!!serialization

		//threading support?
		class MethodInvokeItem
		{
			public string name;
			public Metadata.Method methodVirtual;
			public MethodInfo methodNative;//need it because engine's metadata is not supports not public members.
			public object[] parameters;
			public double remainingTime;
			public double repeatRate;
		}
		List<MethodInvokeItem> invokingMethods;

		/// <summary>
		/// Calls a method by name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parameters"></param>
		/// <param name="time"></param>
		/// <param name="repeatRate"></param>
		/// <returns></returns>
		public object MethodInvoke( string name, object[] parameters = null, double time = 0, double repeatRate = 0 )
		{
			if( parameters == null )
				parameters = Array.Empty<object>();

			if( ObjectEx.FindMethod( this, name, parameters, out var methodVirtual, out var methodNative, out var newParameters ) )
			{
				if( time == 0 && repeatRate == 0 )
					return ObjectEx.MethodInvoke( this, methodVirtual, methodNative, newParameters );
				else
				{
					var item = new MethodInvokeItem();
					item.name = name;
					item.methodVirtual = methodVirtual;
					item.methodNative = methodNative;
					item.parameters = newParameters;
					item.remainingTime = time;
					item.repeatRate = repeatRate;

					if( invokingMethods == null )
						invokingMethods = new List<MethodInvokeItem>();
					invokingMethods.Add( item );

					if( item.remainingTime == 0 )
					{
						ObjectEx.MethodInvoke( this, item.methodVirtual, item.methodNative, item.parameters );
						item.remainingTime = item.repeatRate;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Checks if a method is running over time. You can start executing such methods using <see cref="MethodInvoke"/> method.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool IsMethodInvoking( string name )
		{
			if( invokingMethods != null )
			{
				foreach( var item in invokingMethods )
					if( item.name == name )
						return true;
			}
			return false;
		}

		/// <summary>
		/// Cancels the execution of selected method that run over time. You can start executing such methods using <see cref="MethodInvoke"/> method.
		/// </summary>
		/// <param name="name"></param>
		public void MethodInvokeCancel( string name )
		{
			if( invokingMethods != null )
			{
				for( int n = invokingMethods.Count - 1; n >= 0; n-- )
				{
					if( invokingMethods[ n ].name == name )
						invokingMethods.RemoveAt( n );
				}
				if( invokingMethods.Count == 0 )
					invokingMethods = null;
			}
		}

		/// <summary>
		/// Cancels the execution of all methods that run over time. You can start executing such methods using <see cref="MethodInvoke"/> method.
		/// </summary>
		public void MethodInvokeCancelAll()
		{
			invokingMethods = null;
		}

		void MethodInvokeUpdate( double delta )
		{
			if( invokingMethods != null )
			{
				List<MethodInvokeItem> itemsToRemove = null;

				for( int nItem = 0; nItem < invokingMethods.Count; nItem++ )
				{
					var item = invokingMethods[ nItem ];

					item.remainingTime -= delta;

					again:;
					if( item.remainingTime <= 0 )
					{
						ObjectEx.MethodInvoke( this, item.methodVirtual, item.methodNative, item.parameters );

						if( item.repeatRate > 0 )
						{
							item.remainingTime += item.repeatRate;
							goto again;
						}
						else
						{
							if( itemsToRemove == null )
								itemsToRemove = new List<MethodInvokeItem>();
							itemsToRemove.Add( item );
						}
					}
				}

				if( itemsToRemove != null )
				{
					foreach( var item in itemsToRemove )
						invokingMethods.Remove( item );
					if( invokingMethods.Count == 0 )
						invokingMethods = null;
				}
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Gets the value of a property by name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="indexers"></param>
		/// <param name="unreferenceValue"></param>
		/// <returns></returns>
		public object PropertyGet( string name, object[] indexers = null, bool unreferenceValue = true )
		{
			return ObjectEx.PropertyGet( this, name, indexers, unreferenceValue );
		}

		/// <summary>
		/// Gets the value of a property by name.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="indexers"></param>
		/// <param name="unreferenceValue"></param>
		/// <returns></returns>
		public T PropertyGet<T>( string name, object[] indexers = null, bool unreferenceValue = true )
		{
			return ObjectEx.PropertyGet<T>( this, name, indexers, unreferenceValue );
		}

		/// <summary>
		/// Sets the value of a property by name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="indexers"></param>
		/// <returns></returns>
		public bool PropertySet( string name, object value, object[] indexers = null )
		{
			return ObjectEx.PropertySet( this, name, value, indexers );
		}

		/////////////////////////////////////////

		////!!!!больше параметров по идее можно
		//public void SendMessage( string methodName, object parameter = null )
		//{
		//	//!!!!slowly

		//	if( EnabledInHierarchy )
		//	{

		//		foreach( var member in MetadataGetMembers() )
		//		{
		//			var method = member as Metadata.Method;
		//			if( method != null && method.Name == methodName )
		//			{
		//				//!!!!impl

		//				if( parameter != null )
		//				{
		//					//method.GetReturnParameters

		//				}
		//				else
		//				{
		//				}

		//				//method.Parameters
		//			}
		//		}
		//	}
		//}

		//public void SendMessageBroadcast( string methodName, object parameter = null )
		//{
		//	//!!!!slowly

		//	if( EnabledInHierarchy )
		//	{
		//		SendMessage( methodName, null );
		//		foreach( var c in GetComponents( onlyEnabledInHierarchy: true ) )
		//			c.SendMessageBroadcast( methodName, parameter );
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//protected virtual void OnApplicationPause( bool paused )
		//{
		//	foreach( var c in GetComponents( false, false, true ) )
		//	{
		//		if( c.EnabledInHierarchy && !c.RemoveFromParentQueued )//second check if changed during enumeration
		//			c.PerformApplicationPause( paused );
		//	}
		//}
		//public delegate void ApplicationPauseDelegate( Component sender, bool paused );
		//public event ApplicationPauseDelegate ApplicationPause;

		//internal void PerformApplicationPause( bool paused )
		//{
		//	OnApplicationPause( paused );
		//	ApplicationPause?.Invoke( this, paused );
		//}

		/////////////////////////////////////////

		/// <summary>
		/// Calls <see cref="Log.Info(object)"/>.
		/// </summary>
		/// <param name="message"></param>
		public static void Print( object message )
		{
			Log.Info( message );
		}

		/////////////////////////////////////////

		/////////////////////////////////////////

		bool TypeSettingsIsPrivateObject( Component child )//, bool checkBaseTypes )
		{
			//!!!!slowly?

			if( ComponentUtility.TypeSettingsPrivateObjectsContains( TypeSettingsPrivateObjects, child ) )
				return true;
			//var list = TypeSettingsPrivateObjects;
			//if( list != null )
			//{
			//	var findName = child.GetPathFromParent();
			//	foreach( var name in list )
			//		if( name == findName )
			//			return true;
			//}

			//if( checkBaseTypes )
			{
				var baseType = BaseType as Metadata.ComponentTypeInfo;
				if( baseType != null && baseType.BasedOnObject.TypeSettingsIsPrivateObject( child/*, true */) )
					return true;
			}

			return false;
		}

		////!!!!new
		//inter bool _TypeSettingsIsThisPrivateObject()//Component child, bool checkBaseTypes = true )
		//{
		//	var baseComponentType = Parent?.BaseType as Metadata.ComponentTypeInfo;
		//	if( baseComponentType != null && baseComponentType.BasedOnObject.TypeSettingsIsPrivateObject( this ) )
		//		return true;
		//	return false;
		//}

		////!!!!было Metadata.Property property
		bool TypeSettingsIsPrivateObject( Metadata.Member member )//, bool checkBaseTypes )
		{
			//!!!!slowly?

			if( ComponentUtility.TypeSettingsPrivateObjectsContains( TypeSettingsPrivateObjects, member ) )
				return true;
			//var list = TypeSettingsPrivateObjects;
			//if( list != null )
			//{
			//	foreach( var name in list )
			//		if( name == member.Name )
			//			return true;
			//}

			//if( checkBaseTypes )
			{
				var baseType = BaseType as Metadata.ComponentTypeInfo;
				if( baseType != null && baseType.BasedOnObject.TypeSettingsIsPrivateObject( member/*, true */) )
					return true;
			}

			return false;
		}

		/// <summary>
		/// Checks whether the object provided as a type is public.
		/// </summary>
		/// <returns></returns>
		public bool TypeSettingsIsPublic()
		{
			var baseComponentType = Parent?.BaseType as Metadata.ComponentTypeInfo;
			if( baseComponentType != null && baseComponentType.BasedOnObject.TypeSettingsIsPrivateObject( this ) )
				return false;

			return true;
		}

		/// <summary>
		/// Checks whether the member is set up as public.
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public bool TypeSettingsIsPublicMember( Metadata.Member member )
		{
			var baseComponentType = BaseType as Metadata.ComponentTypeInfo;
			if( baseComponentType != null && baseComponentType.BasedOnObject.TypeSettingsIsPrivateObject( member ) )
				return false;
			return true;
		}

		/// <summary>
		/// The user data of the component.
		/// </summary>
		[Browsable( false )]
		public object AnyData { get; set; }

		internal string GetCachedResourceReference()
		{
			//!!!!threading

			if( !cachedResourceReferenceInitialized )
			{
				if( EnabledInHierarchy )
				{
					var res = ParentRoot.HierarchyController?.CreatedByResource;
					if( res != null && res.InstanceType == Resource.InstanceType.Resource )
					{
						var owner = res.Owner;
						if( owner != null && owner.LoadFromFile )
						{
							var v = owner.Name;
							if( Parent != null )
								v += "|" + GetPathFromRoot();
							cachedResourceReference = v;
						}
					}
				}

				cachedResourceReferenceInitialized = true;
			}

			return cachedResourceReference;
		}

		/// <summary>
		/// Determines when the object is attached to a hierarchy of the components, is enabled and the object if not part of a resource (it is usual object instance). The object will be enabled only when all parents are enabled, and the property <see cref="Enabled"/> is enabled.
		/// </summary>
		[Browsable( false )]
		public bool EnabledInHierarchyAndIsNotResource
		{
			get
			{
				if( EnabledInHierarchy )
				{
					var ins = ComponentUtility.GetResourceInstanceByComponent( this );
					var isResource = ins != null && ins.InstanceType == Resource.InstanceType.Resource;
					if( !isResource )
						return true;
				}
				return false;
			}
		}

		public struct ScreenLabelInfo
		{
			public string LabelName;
			public bool DisplayInCorner;

			public ScreenLabelInfo( string labelName, bool displayInCorner = false )
			{
				LabelName = labelName;
				DisplayInCorner = displayInCorner;
			}
		}

		public virtual ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo();
		}

		protected virtual void OnEditorGetTextInfoCenterBottomCorner( List<string> lines )
		{
		}

		internal void PerformEditorGetTextInfoCenterBottomCorner( List<string> lines )
		{
			OnEditorGetTextInfoCenterBottomCorner( lines );
		}

	}
}
