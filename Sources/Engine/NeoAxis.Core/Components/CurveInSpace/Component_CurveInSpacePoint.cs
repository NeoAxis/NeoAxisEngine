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
	/// Represents the point of the curve in the scene.
	/// </summary>
	public class Component_CurveInSpacePoint : Component_ObjectInSpace
	{
		/// <summary>
		/// The time of the point.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Serialize]
		public Reference<double> Time
		{
			get { if( _time.BeginGet() ) Time = _time.Get( this ); return _time.value; }
			set { if( _time.BeginSet( ref value ) ) { try { TimeChanged?.Invoke( this ); DataWasChanged(); } finally { _time.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Time"/> property value changes.</summary>
		public event Action<Component_CurveInSpacePoint> TimeChanged;
		ReferenceField<double> _time = 0.0;

		//!!!!need "double?" support for properties
		//RoundedLineCurvatureRadius

		//

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			DataWasChanged();
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() )
					context2.disableShowingLabelForThisObject = true;
			}
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "CurveInSpacePoint" );
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			DataWasChanged();
		}

		public void DataWasChanged()
		{
			var obj = Parent as Component_CurveInSpace;
			obj?.DataWasChanged();
		}
	}
}
