// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	class NULLSoundWorld : SoundWorld
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

		protected override Sound _SoundCreateDataBuffer( SoundModes mode, int channels, int frequency, int bufferSize, DataReadDelegate dataBufferCallback )
		{
			return null;
		}

		protected override bool _RecordStart( Sound sound )
		{
			return false;
		}

		protected override void _RecordStop()
		{
		}

		protected override bool _IsRecording()
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

		protected override string[] _RecordDrivers
		{
			get { return new string[ 0 ]; }
		}

		//public override int PlaybackDriver
		//{
		//   get { return -1; }
		//   set { }
		//}

		protected override int _RecordDriver
		{
			get { return -1; }
			set { }
		}

		protected override string _DriverName
		{
			get { return "NULL"; }
		}

		//protected override Sound _SoundCreate( VirtualFileStream stream, bool closeStreamAfterReading, SoundType soundType, SoundModes mode )
		//{
		//	return null;
		//}
	}
}
