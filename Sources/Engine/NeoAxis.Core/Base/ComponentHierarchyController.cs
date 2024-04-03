// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// The class for managing the component hierarchy.
	/// </summary>
	public class ComponentHierarchyController
	{
		//!!!!threading?

		internal Component rootComponent;
		internal Resource.Instance createdByResource;

		internal ESet<Component> objectsDeletionQueue = new ESet<Component>();
		bool hierarchyEnabled;

		object lockObjectHierarchy = new object();

		//Simulation
		double simulationTime = -1;
		//bool simulationEnabled;
		//bool systemPauseOfSimulationEnabled;
		//internal SimulationTypes simulationType;
		//internal SimulationStatuses simulationStatus = SimulationStatuses.StillNotSimulated;

		//!!!!serialization?
		ESet<Flow> sleepingFlows = new ESet<Flow>();

		internal NetworkServerInterface networkServerInterface;
		internal NetworkClientInterface networkClientInterface;

		//!!!!если динамическое свойство чтобы не копилось
		Dictionary<Metadata.Property, PropertyChangedHandler> networkComponentPropertyChangedEventHandlers = new Dictionary<Metadata.Property, PropertyChangedHandler>();

		internal bool loading;

		/////////////////////////////////////////

		public class NetworkServerInterface
		{
			public delegate void AddComponentDelegate( NetworkServerInterface sender, Component child, bool createComponent );
			public event AddComponentDelegate AddComponent;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void PerformAddComponent( Component child, bool createComponent )
			{
				AddComponent?.Invoke( this, child, createComponent );
			}

			public delegate void RemoveFromParentDelegate( NetworkServerInterface sender, Component component, bool queued, bool disposing );
			public event RemoveFromParentDelegate RemoveFromParent;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void PerformRemoveFromParent( Component component, bool queued, bool disposing )
			{
				RemoveFromParent?.Invoke( this, component, queued, disposing );
			}

			public delegate void PropertyChangedEventDelegate( NetworkServerInterface sender, Component component, Metadata.Property property );
			public event PropertyChangedEventDelegate PropertyChangedEvent;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void PerformPropertyChangedEvent( Component component, Metadata.Property property )
			{
				PropertyChangedEvent?.Invoke( this, component, property );
			}

			public delegate void SimulationStepDelegate( NetworkServerInterface sender );
			public event SimulationStepDelegate SimulationStep;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void PerformSimulationStep()
			{
				SimulationStep?.Invoke( this );
			}

			public delegate void BeginNetworkMessageDelegate( NetworkServerInterface sender, Component component, IList<ServerNetworkService_Components.ClientItem> clientRecipients, ServerNetworkService_Components.ClientItem clientRecipient, IList<ServerNetworkService_Users.UserInfo> userRecipients, ServerNetworkService_Users.UserInfo userRecipient, bool toEveryone, string message, ref ArrayDataWriter writer );
			public event BeginNetworkMessageDelegate BeginNetworkMessage;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public ArrayDataWriter PerformBeginNetworkMessage( Component component, IList<ServerNetworkService_Components.ClientItem> clientRecipients, ServerNetworkService_Components.ClientItem clientRecipient, IList<ServerNetworkService_Users.UserInfo> userRecipients, ServerNetworkService_Users.UserInfo userRecipient, bool toEveryone, string message )
			{
				ArrayDataWriter writer = null;
				BeginNetworkMessage?.Invoke( this, component, clientRecipients, clientRecipient, userRecipients, userRecipient, toEveryone, message, ref writer );
				return writer;
			}

			public delegate void EndNetworkMessageDelegate( NetworkServerInterface sender );
			public event EndNetworkMessageDelegate EndNetworkMessage;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void PerformEndNetworkMessage()
			{
				EndNetworkMessage?.Invoke( this );
			}

			public delegate void GetComponentByNetworkIDDelegate( NetworkServerInterface sender, long networkID, ref Component component );
			public event GetComponentByNetworkIDDelegate GetComponentByNetworkID;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public Component PerformGetComponentByNetworkID( long networkID )
			{
				Component component = null;
				GetComponentByNetworkID?.Invoke( this, networkID, ref component );
				return component;
			}

			public delegate void ChangeNetworkModeDelegate( NetworkServerInterface sender, Component component );
			public event ChangeNetworkModeDelegate ChangeNetworkMode;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void PerformChangeNetworkMode( Component component )
			{
				ChangeNetworkMode?.Invoke( this, component );
			}

			public delegate void NetworkModeAddUserDelegate( NetworkServerInterface sender, ServerNetworkService_Components.ClientItem clientItem, Component component );
			public event NetworkModeAddUserDelegate NetworkModeAddUser;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void PerformNetworkModeAddUser( ServerNetworkService_Components.ClientItem clientItem, Component component )
			{
				NetworkModeAddUser?.Invoke( this, clientItem, component );
			}

			public delegate void NetworkModeRemoveUserDelegate( NetworkServerInterface sender, ServerNetworkService_Components.ClientItem clientItem, Component component );
			public event NetworkModeRemoveUserDelegate NetworkModeRemoveUser;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void PerformNetworkModeRemoveUser( ServerNetworkService_Components.ClientItem clientItem, Component component )
			{
				NetworkModeRemoveUser?.Invoke( this, clientItem, component );
			}
		}

		/////////////////////////////////////////

		public class NetworkClientInterface
		{
			public delegate void BeginNetworkMessageDelegate( NetworkClientInterface sender, Component component, string message, ref ArrayDataWriter writer );
			public event BeginNetworkMessageDelegate BeginNetworkMessage;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public ArrayDataWriter PerformBeginNetworkMessage( Component component, string message )
			{
				ArrayDataWriter writer = null;
				BeginNetworkMessage?.Invoke( this, component, message, ref writer );
				return writer;
			}

			public delegate void EndNetworkMessageDelegate( NetworkClientInterface sender );
			public event EndNetworkMessageDelegate EndNetworkMessage;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void PerformEndNetworkMessage()
			{
				EndNetworkMessage?.Invoke( this );
			}

			public delegate void GetComponentByNetworkIDDelegate( NetworkClientInterface sender, long networkID, ref Component component );
			public event GetComponentByNetworkIDDelegate GetComponentByNetworkID;
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public Component PerformGetComponentByNetworkID( long networkID )
			{
				Component component = null;
				GetComponentByNetworkID?.Invoke( this, networkID, ref component );
				return component;
			}
		}

		/////////////////////////////////////////

		class PropertyChangedHandler
		{
			public Metadata.Property property;
			public ComponentHierarchyController controller;
			public Delegate _delegate;

			public PropertyChangedHandler( Metadata.Property property, ComponentHierarchyController controller )
			{
				this.property = property;
				this.controller = controller;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void NetworkComponentPropertyChangedEventHandler( Component component )
			{
				if( !component.networkDisableChangedEvents )
					controller.networkServerInterface?.PerformPropertyChangedEvent( component, property );
			}
		}

		/////////////////////////////////////////

		public ComponentHierarchyController()
		{
		}

		public Component RootComponent
		{
			get { return rootComponent; }
		}

		public Resource.Instance CreatedByResource
		{
			get { return createdByResource; }
		}

		public bool HierarchyEnabled
		{
			get { return hierarchyEnabled; }
			set
			{
				if( hierarchyEnabled == value )
					return;
				hierarchyEnabled = value;

				rootComponent._UpdateEnabledInHierarchy( false );
			}
		}

		void ProcessObjectsDeletionQueue()
		{
			while( objectsDeletionQueue.Count != 0 )
			{
				var e = objectsDeletionQueue.GetEnumerator();
				e.MoveNext();
				Component c = e.Current;

				if( c.Parent != null )
					c.RemoveFromParent( false );
				else
					objectsDeletionQueue.Remove( c );
			}
		}

		public void ProcessDelayedOperations()
		{
			ProcessObjectsDeletionQueue();
		}

		public object LockObjectHierarchy
		{
			get { return lockObjectHierarchy; }
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		///// <summary>
		///// Gets the current time of simulation.
		///// </summary>
		public double SimulationTime
		{
			get { return simulationTime; }
		}

		///// <summary>
		///// To reset current simulation time.
		///// </summary>
		///// <remarks>
		///// This method need call, after there was a long loading.
		///// This method is called, that the timer did not try to catch up with lagged behind time.
		///// </remarks>
		public void ResetSimulationTime()
		{
			simulationTime = -1;
			//simulationTickTime = EngineApp.Instance.EngineTime;

			//lastRenderTime = simulationTickTime;
			//lastRenderTimeStep = 0;
		}

		void ProcessSleepingFlows()
		{
			Flow[] flows;
			lock( sleepingFlows )
			{
				flows = sleepingFlows.ToArray();
				sleepingFlows.Clear();
			}
			foreach( var flow in flows )
				flow.ContinueProcess();
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void SimulateOneStep()
		{
			if( rootComponent != null && rootComponent.EnabledInHierarchy )
				rootComponent.PerformSimulationStep();
			ProcessDelayedOperations();

			ProcessSleepingFlows();

			networkServerInterface?.PerformSimulationStep();
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void SimulateOneStepClient()
		{
			if( rootComponent != null && rootComponent.EnabledInHierarchy )
				rootComponent.PerformSimulationStepClient();
			ProcessDelayedOperations();

			ProcessSleepingFlows();
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void PerformSimulationSteps()
		{
			ProcessDelayedOperations();

			//if( SimulationType == SimulationTypes.ClientOnly )
			//	return;

			double time = EngineApp.EngineTime;

			//if( !simulationEnabled || systemPauseOfSimulationEnabled )
			//{
			//	simulationTickTime = time;
			//	return;
			//}

			//reset time
			if( simulationTime < 0 )
				simulationTime = time;

			//!!!!new
			//too big pause
			if( time > simulationTime + 0.25 )
				simulationTime = time;

			//loop
			double delta = ProjectSettings.Get.General.SimulationStepsPerSecondInv;
			while( time > simulationTime + delta )
			{
				simulationTime += delta;
				SimulateOneStep();
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void PerformSimulationStepClient()
		{
			double time = EngineApp.EngineTime;

			//!!!!сглаживать или может иначе, например, по шагам
			simulationTime = time;

			SimulateOneStepClient();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void AddSleepingFlow( Flow flow )
		{
			lock( sleepingFlows )
				sleepingFlows.AddWithCheckAlreadyContained( flow );
		}

		/////////////////////////////////////////
		//networking

		[MethodImpl( (MethodImplOptions)512 )]
		internal void NetworkSubscribeToEventsChanged( Component component )
		{
			if( !component.networkSubscribedToEvents )
			{
				var context = NetworkUtility.metadataGetMembersContextNoFilter;//new Metadata.GetMembersContext( false );

				foreach( var member in component.MetadataGetMembers( context ).ToArray() )
				{
					var property = member as Metadata.Property;
					if( property != null && property.NetworkMode && MetadataManager.Serialization.IsMemberSerializable( property ) )
					{
						var netEvent = component.MetadataGetMemberBySignature( $"event:{property.Name}Changed", context ) as Metadata.NetTypeInfo.NetEvent;
						if( netEvent != null )
						{
							var handlerType = netEvent.NetMember.EventHandlerType;

							if( !networkComponentPropertyChangedEventHandlers.TryGetValue( property, out var handler ) )
							{
								var method = typeof( PropertyChangedHandler ).GetMethod( nameof( PropertyChangedHandler.NetworkComponentPropertyChangedEventHandler ), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
								if( method == null )
									Log.Fatal( "ComponentHierarchyController: NetworkSubscribeToEvents: method == null." );

								handler = new PropertyChangedHandler( property, this );
								handler._delegate = Delegate.CreateDelegate( handlerType, handler, method );

								networkComponentPropertyChangedEventHandlers[ property ] = handler;
							}

							//!!!!проверить не копятся ли
							//Log.Info( networkComponentPropertyChangedEventHandlers.Count.ToString() );


							//нестандартные эвенты наверное могут давать ассершен
							try
							{
								netEvent.NetMember.AddEventHandler( component, handler._delegate );
							}
							catch { }
						}
					}
				}

				component.networkSubscribedToEvents = true;
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		internal void NetworkUnsubscribeToEventsChanged( Component component )
		{
			if( component.networkSubscribedToEvents )
			{
				var context = NetworkUtility.metadataGetMembersContextNoFilter;//new Metadata.GetMembersContext( false );

				foreach( var member in component.MetadataGetMembers( context ).ToArray() )
				{
					var property = member as Metadata.Property;
					if( property != null && property.NetworkMode && MetadataManager.Serialization.IsMemberSerializable( property ) )
					{
						var netEvent = component.MetadataGetMemberBySignature( $"event:{property.Name}Changed", context ) as Metadata.NetTypeInfo.NetEvent;
						if( netEvent != null )
						{
							try
							{
								if( networkComponentPropertyChangedEventHandlers.TryGetValue( property, out var handler ) )
									netEvent.NetMember.RemoveEventHandler( component, handler._delegate );
							}
							catch { }
						}
					}
				}

				component.networkSubscribedToEvents = false;
			}
		}

		public bool NetworkIsServer
		{
			get { return networkServerInterface != null; }
		}

		public bool NetworkIsClient
		{
			get { return networkClientInterface != null; }
		}

		public bool NetworkIsSingle
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return networkServerInterface == null && networkClientInterface == null; }
		}

		public bool NetworkIsSingleOrClient
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return networkServerInterface == null; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Component GetComponentByNetworkID( long networkID )
		{
			if( networkServerInterface != null )
				return networkServerInterface.PerformGetComponentByNetworkID( networkID );
			if( networkClientInterface != null )
				return networkClientInterface.PerformGetComponentByNetworkID( networkID );
			return null;
		}

		public bool Loading
		{
			get { return loading; }
		}
	}
}