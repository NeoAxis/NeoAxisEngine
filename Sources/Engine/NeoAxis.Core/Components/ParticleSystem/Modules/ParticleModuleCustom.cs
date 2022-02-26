// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Custom particle module.
	/// </summary>
	[NewObjectDefaultName( "Custom Module" )]
	public class ParticleModuleCustom : ParticleModule
	{
		protected virtual void OnUpdateBefore( ParticleSystemInSpace particleSystemInSpace, float delta, ref ParticleSystemInSpace.Particle particle ) { }

		public delegate void UpdateBeforeDelegate( ParticleSystemInSpace particleSystemInSpace, float delta, ref ParticleSystemInSpace.Particle particle );
		public event UpdateBeforeDelegate UpdateBefore;

		public void PerformUpdateBefore( ParticleSystemInSpace particleSystemInSpace, float delta, ref ParticleSystemInSpace.Particle particle )
		{
			OnUpdateBefore( particleSystemInSpace, delta, ref particle );
			UpdateBefore?.Invoke( particleSystemInSpace, delta, ref particle );
		}


		protected virtual void OnUpdateAfter( ParticleSystemInSpace particleSystemInSpace, float delta, ref ParticleSystemInSpace.Particle particle ) { }

		public delegate void UpdateAfterDelegate( ParticleSystemInSpace particleSystemInSpace, float delta, ref ParticleSystemInSpace.Particle particle );
		public event UpdateAfterDelegate UpdateAfter;

		public void PerformUpdateAfter( ParticleSystemInSpace particleSystemInSpace, float delta, ref ParticleSystemInSpace.Particle particle )
		{
			OnUpdateAfter( particleSystemInSpace, delta, ref particle );
			UpdateAfter?.Invoke( particleSystemInSpace, delta, ref particle );
		}
	}
}
