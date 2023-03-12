// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	public abstract class ParticleModule : Component
	{
		public void ShouldRecompileParticleSystem()
		{
			var emitter = Parent as ParticleEmitter;
			if( emitter != null )
				emitter.ShouldRecompileParticleSystem();
		}

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			ShouldRecompileParticleSystem();
		}

		protected override void OnAddedToParent()
		{
			base.OnAddedToParent();

			ShouldRecompileParticleSystem();
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			base.OnRemovedFromParent( oldParent );

			var system = oldParent.FindThisOrParent<ParticleEmitter>();
			if( system != null )
				system.ShouldRecompileParticleSystem();
		}
	}
}
