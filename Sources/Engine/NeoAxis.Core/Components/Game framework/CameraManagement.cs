// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// An object to manage camera.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Camera Management", -9997.5 )]
	public class CameraManagement : Component
	{
		public delegate void GetCameraSettingsEventDelegate( CameraManagement sender, GameMode gameMode, Viewport viewport, Camera cameraDefault, ref Viewport.CameraSettingsClass cameraSettings );
		public event GetCameraSettingsEventDelegate GetCameraSettingsEvent;

		public virtual Viewport.CameraSettingsClass GetCameraSettings( GameMode gameMode, Viewport viewport, Camera cameraDefault )
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

		public delegate void IsNeedMouseRelativeModeEventDelegate( CameraManagement sender, GameMode gameMode, ref bool result );
		public event IsNeedMouseRelativeModeEventDelegate IsNeedMouseRelativeModeEvent;

		public virtual bool IsNeedMouseRelativeMode( GameMode gameMode )
		{
			bool result = true;
			IsNeedMouseRelativeModeEvent?.Invoke( this, gameMode, ref result );

			return result;
		}

		/////////////////////////////////////////

		public delegate void PickInteractiveObjectEventDelegate( CameraManagement sender, GameMode gameMode, Viewport viewport, ref InteractiveObjectInterface result );
		public event PickInteractiveObjectEventDelegate PickInteractiveObjectEvent;

		public virtual InteractiveObjectInterface PickInteractiveObject( GameMode gameMode, Viewport viewport )
		{
			InteractiveObjectInterface result = null;
			PickInteractiveObjectEvent?.Invoke( this, gameMode, viewport, ref result );

			return result;
		}

		/////////////////////////////////////////

		public delegate void IsNeedRenderObjectInteractionEventDelegate( CameraManagement sender, GameMode gameMode, CanvasRenderer renderer, ref bool result );
		public event IsNeedRenderObjectInteractionEventDelegate IsNeedRenderObjectInteractionEvent;

		public virtual bool IsNeedRenderObjectInteraction( GameMode gameMode, CanvasRenderer renderer )
		{
			bool result = true;
			IsNeedRenderObjectInteractionEvent?.Invoke( this, gameMode, renderer, ref result );

			return result;
		}
	}
}
