// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Represents a point of the <see cref="Component_Area"/>.
	/// </summary>
	public class Component_AreaPoint : Component_ObjectInSpace
	{
		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			var area = Parent as Component_Area;
			if( area != null )
			{
				if( !area.CheckNeedShowDevelopmentDataOfAreaAndPoints( context, mode ) )
				{
					var context2 = context.objectInSpaceRenderingContext;
					context2.disableShowingLabelForThisObject = true;
				}
			}
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() || !ParentScene.DisplayLabels )
				return false;
			return base.OnEnabledSelectionByCursor();
		}
	}
}
