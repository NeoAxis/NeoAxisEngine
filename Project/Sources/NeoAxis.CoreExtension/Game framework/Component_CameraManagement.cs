// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// An object to manage camera.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Camera Management", -9997.5 )]
	public class Component_CameraManagement : Component
	{
		public delegate void GetCameraSettingsEventDelegate( Component_CameraManagement sender, Component_GameMode gameMode, Viewport viewport, Component_Camera cameraDefault, ref Viewport.CameraSettingsClass cameraSettings );
		public event GetCameraSettingsEventDelegate GetCameraSettingsEvent;

		public virtual Viewport.CameraSettingsClass GetCameraSettings( Component_GameMode gameMode, Viewport viewport, Component_Camera cameraDefault )
		{
			//event
			Viewport.CameraSettingsClass result = null;
			GetCameraSettingsEvent?.Invoke( this, gameMode, viewport, cameraDefault, ref result );

			//default behavior
			if( result == null )
			{
				result = new Viewport.CameraSettingsClass( viewport, cameraDefault.AspectRatio, cameraDefault.FieldOfView, cameraDefault.NearClipPlane, cameraDefault.FarClipPlane, gameMode.FreeCameraPosition, gameMode.FreeCameraDirection.GetVector(), Vector3.ZAxis, ProjectionType.Perspective, 1, cameraDefault.Exposure, cameraDefault.EmissiveFactor );
			}

			return result;
		}

		/////////////////////////////////////////

		public delegate void IsNeedMouseRelativeModeEventDelegate( Component_CameraManagement sender, Component_GameMode gameMode, ref bool result );
		public event IsNeedMouseRelativeModeEventDelegate IsNeedMouseRelativeModeEvent;

		public virtual bool IsNeedMouseRelativeMode( Component_GameMode gameMode )
		{
			bool result = true;
			IsNeedMouseRelativeModeEvent?.Invoke( this, gameMode, ref result );

			return result;
		}

		/////////////////////////////////////////

		public delegate void PickInteractiveObjectEventDelegate( Component_CameraManagement sender, Component_GameMode gameMode, UIControl playScreen, Viewport viewport, ref IComponent_InteractiveObject result );
		public event PickInteractiveObjectEventDelegate PickInteractiveObjectEvent;

		public virtual IComponent_InteractiveObject PickInteractiveObject( Component_GameMode gameMode, UIControl playScreen, Viewport viewport )
		{
			IComponent_InteractiveObject result = null;
			PickInteractiveObjectEvent?.Invoke( this, gameMode, playScreen, viewport, ref result );

			return result;
		}

		/////////////////////////////////////////

		public delegate void IsNeedRenderObjectInteractionEventDelegate( Component_CameraManagement sender, Component_GameMode gameMode, UIControl playScreen, CanvasRenderer renderer, ref bool result );
		public event IsNeedRenderObjectInteractionEventDelegate IsNeedRenderObjectInteractionEvent;

		public virtual bool IsNeedRenderObjectInteraction( Component_GameMode gameMode, UIControl playScreen, CanvasRenderer renderer )
		{
			bool result = true;
			IsNeedRenderObjectInteractionEvent?.Invoke( this, gameMode, playScreen, renderer, ref result );

			return result;
		}

	}
}
