// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	public partial class Component_Scene
	{
		IPhysics2DWorldData physics2DWorldData;

		/////////////////////////////////////////

		public interface IPhysics2DWorldData
		{
			void Destroy();
			void UpdateGravity();
			void Simulate();
			void ViewportUpdateBegin( Viewport viewport );
		}

		/////////////////////////////////////////

		public delegate IPhysics2DWorldData Physics2DGetWorldDataInitDelegate( Component_Scene scene );

		public IPhysics2DWorldData Physics2DGetWorldData( bool canInit, Physics2DGetWorldDataInitDelegate init )
		{
			if( physics2DWorldData == null && canInit )
				physics2DWorldData = init( this );
			return physics2DWorldData;
		}

		void Physics2DWorldDestroy()
		{
			physics2DWorldData?.Destroy();
			physics2DWorldData = null;
		}

		[DisplayName( "Physics 2D Simulation Step After" )]
		public event Action<Component_Scene> Physics2DSimulationStepAfter;

		void Physics2DSimulate()
		{
			if( physics2DWorldData != null )
			{
				physics2DWorldData.Simulate();
				Physics2DSimulationStepAfter?.Invoke( this );
			}
		}

		void Physics2DUpdateGravity()
		{
			physics2DWorldData?.UpdateGravity();
		}

		void Physics2DViewportUpdateBegin( Viewport viewport )
		{
			physics2DWorldData?.ViewportUpdateBegin( viewport );
		}
	}
}
