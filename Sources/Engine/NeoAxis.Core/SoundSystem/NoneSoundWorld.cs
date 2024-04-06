// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	class NoneSoundWorld : SoundWorld
	{
		protected override bool InitLibrary( IntPtr hWnd, int maxReal2DChannels, int maxReal3DChannels )
		{
			return true;
		}

		protected override void ShutdownLibrary()
		{
		}

		protected override void OnUpdateLibrary()
		{
		}

		protected override void OnSetDopplerEffectScale( double dopplerScale )
		{
		}

		protected override void OnSetListener( Vector3 position, Vector3 velocity, Quaternion rotation )
		{
		}

		protected override SoundData Internal_SoundCreateDataBuffer( SoundModes mode, int channels, int frequency, int bufferSize, DataReadDelegate dataBufferCallback )
		{
			return null;
		}

		protected override bool Internal_RecordStart( SoundData sound )
		{
			return false;
		}

		protected override void Internal_RecordStop()
		{
		}

		protected override bool Internal_IsRecording()
		{
			return false;
		}

		//public override int GetRecordPosition()
		//{
		//   return 0;
		//}

		//public override string[] PlaybackDrivers
		//{
		//   get { return new string[ 0 ]; }
		//}

		protected override string[] Internal_RecordDrivers
		{
			get { return new string[ 0 ]; }
		}

		//public override int PlaybackDriver
		//{
		//   get { return -1; }
		//   set { }
		//}

		protected override int Internal_RecordDriver
		{
			get { return -1; }
			set { }
		}

		protected override string Internal_DriverName
		{
			get { return "NULL"; }
		}

		//protected override Sound _SoundCreate( VirtualFileStream stream, bool closeStreamAfterReading, SoundType soundType, SoundModes mode )
		//{
		//	return null;
		//}
	}
}
