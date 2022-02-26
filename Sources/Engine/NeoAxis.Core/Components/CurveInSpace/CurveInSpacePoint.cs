// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents the point of the curve in the scene.
	/// </summary>
	//[EditorSettingsCell( typeof( CurveInSpacePointSettingsCell ) )]
	public class CurveInSpacePoint : ObjectInSpace
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
		public event Action<CurveInSpacePoint> TimeChanged;
		ReferenceField<double> _time = 0.0;

		//!!!!need "double?" support for properties
		//RoundedLineCurvatureRadius

		//

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			DataWasChanged();
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() )
					context2.disableShowingLabelForThisObject = true;

				//visualize handles for BezierPath
				if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
				{
					if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
					{
						var curveInSpace = Parent as CurveInSpace;
						if( curveInSpace != null && curveInSpace.CurveTypePosition.Value == CurveInSpace.CurveTypeEnum.BezierPath )
						{
							ColorValue color;
							if( context2.selectedObjects.Contains( this ) )
								color = ProjectSettings.Get.General.SelectedColor;
							else //if( context2.canSelectObjects.Contains( this ) )
								color = ProjectSettings.Get.General.CanSelectColor;
							color *= new ColorValue( 1, 1, 1, 0.5 );

							var renderer = context.Owner.Simple3DRenderer;
							renderer.SetColor( color, color * ProjectSettings.Get.General.HiddenByOtherObjectsColorMultiplier );

							//!!!!
							//CurveInSpacePointHandle

							var transform = TransformV;

							var offset = transform.Rotation.GetForward() * transform.Scale.MaxComponent();
							var handle1 = transform.Position - offset;
							var handle2 = transform.Position + offset;

							renderer.AddLine( transform.Position, handle1 );
							renderer.AddLine( transform.Position, handle2 );

							var size = transform.Scale.MaxComponent() * 0.025;
							renderer.AddSphere( handle1, size, 16, true );
							renderer.AddSphere( handle2, size, 16, true );
						}
					}
				}
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
			var obj = Parent as CurveInSpace;
			obj?.DataWasChanged();
		}
	}
}
