// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	public class AvatarSettings
	{
		public string NamedCharacter { get; set; } = "";

		//

		public bool Load( TextBlock block )
		{
			NamedCharacter = block.GetAttribute( "NamedCharacter" );

			return true;
		}

		public void Save( TextBlock block )
		{
			block.SetAttribute( "NamedCharacter", NamedCharacter );
		}
	}
}