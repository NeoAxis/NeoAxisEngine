// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Custom particle module.
	/// </summary>
	[NewObjectDefaultName( "Custom Module" )]
	public class Component_ParticleModuleCustom : Component_ParticleModule
	{
		protected virtual void OnUpdateBefore( Component_ParticleSystemInSpace particleSystemInSpace, float delta, ref Component_ParticleSystemInSpace.Particle particle ) { }

		public delegate void UpdateBeforeDelegate( Component_ParticleSystemInSpace particleSystemInSpace, float delta, ref Component_ParticleSystemInSpace.Particle particle );
		public event UpdateBeforeDelegate UpdateBefore;

		public void PerformUpdateBefore( Component_ParticleSystemInSpace particleSystemInSpace, float delta, ref Component_ParticleSystemInSpace.Particle particle )
		{
			OnUpdateBefore( particleSystemInSpace, delta, ref particle );
			UpdateBefore?.Invoke( particleSystemInSpace, delta, ref particle );
		}


		protected virtual void OnUpdateAfter( Component_ParticleSystemInSpace particleSystemInSpace, float delta, ref Component_ParticleSystemInSpace.Particle particle ) { }

		public delegate void UpdateAfterDelegate( Component_ParticleSystemInSpace particleSystemInSpace, float delta, ref Component_ParticleSystemInSpace.Particle particle );
		public event UpdateAfterDelegate UpdateAfter;

		public void PerformUpdateAfter( Component_ParticleSystemInSpace particleSystemInSpace, float delta, ref Component_ParticleSystemInSpace.Particle particle )
		{
			OnUpdateAfter( particleSystemInSpace, delta, ref particle );
			UpdateAfter?.Invoke( particleSystemInSpace, delta, ref particle );
		}
	}
}
