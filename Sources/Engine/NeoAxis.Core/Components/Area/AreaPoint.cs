// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents a point of the <see cref="Area"/>.
	/// </summary>
	public class AreaPoint : ObjectInSpace
	{
		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			var area = Parent as Area;
			if( area != null )
			{
				if( !area.CheckNeedShowDevelopmentDataOfAreaAndPoints( context, mode ) )
				{
					var context2 = context.ObjectInSpaceRenderingContext;
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

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "AreaPoint" );
		}
	}
}
