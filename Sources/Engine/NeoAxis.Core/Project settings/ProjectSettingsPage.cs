// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents a page of the project settings.
	/// </summary>
	public class ProjectSettingsPage : Component
	{
		[Browsable( false )]
		public bool UserMode
		{
			get
			{
				var settings = ParentRoot as ProjectSettingsComponent;
				if( settings != null )
					return settings.UserMode;
				else
					return false;
			}
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( UserMode )
			{
				if( member is Metadata.Property )
				{
					if( member.Name == "Name" || member.Name == "Enabled" || member.Name == "ScreenLabel" || member.Name == "NetworkMode" )
						skip = true;

					if( EditorAPI.DarkTheme )
					{
						if( member.Name.Contains( "LightTheme" ) )
							skip = true;
					}
					else
					{
						if( member.Name.Contains( "DarkTheme" ) )
							skip = true;
					}
				}
			}
		}
	}
}
