// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A spawn point helper.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Spawn Point", -4000 )]
	[NewObjectDefaultName( "Spawn Point" )]
	public class SpawnPoint : ObjectInSpace
	{
		/// <summary>
		/// The number of the team.
		/// </summary>
		[DefaultValue( TeamEnum.None )]
		public Reference<TeamEnum> Team
		{
			get { if( _team.BeginGet() ) Team = _team.Get( this ); return _team.value; }
			set { if( _team.BeginSet( ref value ) ) { try { TeamChanged?.Invoke( this ); } finally { _team.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Team"/> property value changes.</summary>
		public event Action<SpawnPoint> TeamChanged;
		ReferenceField<TeamEnum> _team = TeamEnum.None;

		/// <summary>
		/// A string property to store any data.
		/// </summary>
		[DefaultValue( "" )]
		public Reference<string> AnyText
		{
			get { if( _anyText.BeginGet() ) AnyText = _anyText.Get( this ); return _anyText.value; }
			set { if( _anyText.BeginSet( ref value ) ) { try { AnyTextChanged?.Invoke( this ); } finally { _anyText.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnyText"/> property value changes.</summary>
		public event Action<SpawnPoint> AnyTextChanged;
		ReferenceField<string> _anyText = "";

		//

		public enum TeamEnum
		{
			None,
			[DisplayNameEnum( "Team 1" )]
			Team1,
			[DisplayNameEnum( "Team 2" )]
			Team2,
			//[DisplayNameEnum( "Team 3" )]
			//Team3,
			//[DisplayNameEnum( "Team 4" )]
			//Team4,
		}

	}
}