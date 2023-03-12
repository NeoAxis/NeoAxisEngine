// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// The task of the character is not to do anything for a certain time.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character AI Wait", -8990 )]
	[NewObjectDefaultName( "Wait" )]
	public class CharacterAITask_Wait : AITask
	{
		[Browsable( false )]
		[Serialize]
		[DefaultValue( 0 )]
		public double currentTime;

		/// <summary>
		/// The time to wait in seconds.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> Time
		{
			get { if( _time.BeginGet() ) Time = _time.Get( this ); return _time.value; }
			set { if( _time.BeginSet( ref value ) ) { try { TimeChanged?.Invoke( this ); } finally { _time.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Time"/> property value changes.</summary>
		public event Action<CharacterAITask_Wait> TimeChanged;
		ReferenceField<double> _time = 1.0;

		///////////////////////////////////////////////

		protected override void OnPerformTaskSimulationStep()
		{
			base.OnPerformTaskSimulationStep();

			var ai = FindParent<CharacterAI>();
			if( ai != null )
			{
				currentTime += NeoAxis.Time.SimulationDelta;

				if( currentTime >= Time.Value )
				{
					//task is done
					if( DeleteTaskWhenReach )
						Dispose();
				}
			}
		}
	}
}
