// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	//[EditorSettingsCell( typeof( Component_CurveInSpacePoint_SettingsCell ) )]
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

				//visualize handles for BezierPath
				if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
				{
					if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
					{
						var curveInSpace = Parent as Component_CurveInSpace;
						if( curveInSpace != null && curveInSpace.CurveTypePosition.Value == Component_CurveInSpace.CurveTypeEnum.BezierPath )
						{
							ColorValue color;
							if( context2.selectedObjects.Contains( this ) )
								color = ProjectSettings.Get.SelectedColor;
							else //if( context2.canSelectObjects.Contains( this ) )
								color = ProjectSettings.Get.CanSelectColor;
							color *= new ColorValue( 1, 1, 1, 0.5 );

							var renderer = context.Owner.Simple3DRenderer;
							renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

							//!!!!
							//Component_CurveInSpacePointHandle

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
			var obj = Parent as Component_CurveInSpace;
			obj?.DataWasChanged();
		}
	}
}
