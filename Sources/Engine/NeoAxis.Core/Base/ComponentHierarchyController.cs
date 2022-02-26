// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The class for managing the component hierarchy.
	/// </summary>
	public class ComponentHierarchyController
	{
		//!!!!threading

		internal Component rootComponent;
		internal Resource.Instance createdByResource;

		internal ESet<Component> objectsDeletionQueue = new ESet<Component>();
		bool hierarchyEnabled;
		//!!!!
		//bool hierarchyVisible;

		object lockObjectHierarchy = new object();

		//Simulation
		double simulationTime = -1;
		//!!!!
		//bool simulationEnabled;
		//bool systemPauseOfSimulationEnabled;
		//internal SimulationTypes simulationType;
		//internal SimulationStatuses simulationStatus = SimulationStatuses.StillNotSimulated;

		//!!!!serialization
		ESet<Flow> sleepingFlows = new ESet<Flow>();

		//

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

		//!!!!
		//public bool HierarchyVisible
		//{
		//	get { return hierarchyVisible; }
		//	set
		//	{
		//		if( hierarchyVisible == value )
		//			return;
		//		hierarchyVisible = value;

		//		rootComponent._UpdateVisibleInHierarchy( false );
		//	}
		//}

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

		//!!!!!!вызывать
		//!!!!что еще вызывать?
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

			//!!!!было
			//lastRenderTime = simulationTickTime;
			//lastRenderTimeStep = 0;
		}

		//!!!!MapObject
		//public static double SimulationTickDelta
		//{
		//	get { return simulationTickDelta; }
		//}

		//void SimulationStep()
		//{
		////send WorldTick message
		//if( SimulationType_IsServer() )
		//{
		//	//!!!!было
		//	//networkTickCounter++;
		//	//networkingInterface.SendWorldTickMessage();
		//}

		////physics world tick
		//if( SimulationType != SimulationTypes.ClientOnly )
		//{
		//	//physicsPerformanceCounter.Start();
		//	physicsScene.Simulate( SimulationTickDelta );
		//	//physicsPerformanceCounter.End();
		//}

		////timer ticks
		////entitySystemPerformanceCounter.Start();

		////MapObjects.PerformSimulationTick( simulationTickExecutedTime, SimulationType == SimulationTypes.ClientOnly );
		//{
		//	SimulationTick?.Invoke( this );

		//	//MapObject[] array = new MapObject[ objectsSubscribedToTicks.Count ];
		//	//objectsSubscribedToTicks.Keys.CopyTo( array, 0 );

		//	//foreach( MapObject obj in array )
		//	//{
		//	//	if( !obj.IsMustDestroy && obj.CreateTime != simulationTickTime )
		//	//	{
		//	//		//!!!!slowly. можно иметь ESet удаленных
		//	//		if( objectsSubscribedToTicks.ContainsKey( obj ) )
		//	//		{
		//	//			if( !clientTick )
		//	//				obj.CallTick();
		//	//			else
		//	//				obj.Client_CallTick();
		//	//		}
		//	//	}
		//	//}
		//}

		////entitySystemPerformanceCounter.End();
		//}

		public void SimulateOneStep()
		{
			if( rootComponent != null && rootComponent.EnabledInHierarchy )
				rootComponent.PerformSimulationStep();
			ProcessDelayedOperations();

			//proress sleeping flows
			Flow[] flows;
			lock( sleepingFlows )
			{
				flows = sleepingFlows.ToArray();
				sleepingFlows.Clear();
			}
			foreach( var flow in flows )
				flow.ContinueProcess();
		}

		public void PerformSimulationSteps()
		{
			ProcessDelayedOperations();

			//!!!!можно было бы: чтобы можно было вне зависимости от времени прокрутить. тогда это значит что в коде и классах объектов не должно быть EngineApp.Instance.Time.

			//!!!!было
			//simulationStatus = SimulationStatuses.AlreadySimulated;

			//!!!!было
			//if( SimulationType == SimulationTypes.ClientOnly )
			//	return;

			double time = EngineApp.EngineTime;

			//!!!!было
			//if( !simulationEnabled || systemPauseOfSimulationEnabled )
			//{
			//	simulationTickTime = time;
			//	return;
			//}

			//reset time
			if( simulationTime < 0 )
				simulationTime = time;

			//loop
			double delta = ProjectSettings.Get.General.SimulationStepsPerSecondInv;
			while( time > simulationTime + delta )
			{
				simulationTime += delta;
				SimulateOneStep();
			}
		}

		internal void AddSleepingFlow( Flow flow )
		{
			lock( sleepingFlows )
				sleepingFlows.AddWithCheckAlreadyContained( flow );
		}
	}
}