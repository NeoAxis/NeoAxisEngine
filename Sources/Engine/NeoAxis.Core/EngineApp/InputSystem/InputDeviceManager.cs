// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NeoAxis
{
	/// <summary>
	/// Represents class for managing input devices.
	/// </summary>
	public abstract class InputDeviceManager
	{
		static InputDeviceManager instance;

		InputEventHandlerDelegate inputEventHandler;
		List<InputDevice> devices = new List<InputDevice>();

		///////////////////////////////////////////

		internal delegate void InputEventHandlerDelegate( InputEvent e );

		///////////////////////////////////////////

		public static InputDeviceManager Instance
		{
			get { return instance; }
		}

		internal static bool Init( InputDeviceManager instance, InputEventHandlerDelegate inputEventHandler )
		{
			Trace.Assert( InputDeviceManager.instance == null, "InputDeviceManager has been already created" );
			InputDeviceManager.instance = instance;

			if( !instance.InitInternal( inputEventHandler ) )
			{
				Shutdown();
				return false;
			}

			return true;
		}

		internal static void Shutdown()
		{
			if( instance != null )
			{
				instance.ShutdownInternal();
				instance = null;
			}
		}

		bool InitInternal( InputEventHandlerDelegate inputEventHandler )
		{
			this.inputEventHandler = inputEventHandler;

			if( !OnInit() )
				return false;

			return true;
		}

		void ShutdownInternal()
		{
			foreach( InputDevice device in devices )
				device.CallOnShutdown();
			devices.Clear();

			OnShutdown();

			inputEventHandler = null;
		}

		internal abstract bool OnInit();
		internal abstract void OnShutdown();

		/// <summary>
		/// Device must call this after creation to register itself in system 
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		public void RegisterDevice( InputDevice device )
		{
			Log.InvisibleInfo( "Register input device: " + device.Name );

			if( devices.Contains( device ) )
			{
				Log.Fatal( "Device \"{0}\" is already registered.", device );
				return;
			}

			devices.Add( device );
		}

		internal void UpdateDeviceState()
		{
			foreach( InputDevice device in devices )
				device.CallOnUpdateState();
		}

		/// <summary>
		/// Get registered devices. <b>Don't modify</b>.
		/// </summary>
		public List<InputDevice> Devices
		{
			get { return devices; }
		}

		public void SendEvent( InputEvent e )
		{
			inputEventHandler( e );
		}
	}
}
