#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Editor
{
	public class ObjectCreationModeAttribute : Attribute
	{
		Type creationModeClass;
		string creationModeClassName;

		//

		public ObjectCreationModeAttribute( Type creationModeClass )
		{
			this.creationModeClass = creationModeClass;
		}

		public ObjectCreationModeAttribute( string creationModeClassName )
		{
			this.creationModeClassName = creationModeClassName;
		}

		public Type CreationModeClass
		{
			get { return creationModeClass; }
		}

		public string CreationModeClassName
		{
			get { return creationModeClassName; }
		}
	}
}
#endif