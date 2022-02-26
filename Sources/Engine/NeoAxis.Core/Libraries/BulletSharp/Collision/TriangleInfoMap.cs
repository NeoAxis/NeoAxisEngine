using System;
using System.Runtime.InteropServices;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class TriangleInfo : IDisposable
	{
		internal IntPtr _native;

		internal TriangleInfo(IntPtr native)
		{
			_native = native;
		}

		public TriangleInfo()
		{
			_native = btTriangleInfo_new();
		}

		public double EdgeV0V1Angle
		{
			get => btTriangleInfo_getEdgeV0V1Angle(_native);
			set => btTriangleInfo_setEdgeV0V1Angle(_native, value);
		}

		public double EdgeV1V2Angle
		{
			get => btTriangleInfo_getEdgeV1V2Angle(_native);
			set => btTriangleInfo_setEdgeV1V2Angle(_native, value);
		}

		public double EdgeV2V0Angle
		{
			get => btTriangleInfo_getEdgeV2V0Angle(_native);
			set => btTriangleInfo_setEdgeV2V0Angle(_native, value);
		}

		public int Flags
		{
			get => btTriangleInfo_getFlags(_native);
			set => btTriangleInfo_setFlags(_native, value);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_native != IntPtr.Zero)
			{
				btTriangleInfo_delete(_native);
				_native = IntPtr.Zero;
			}
		}

		~TriangleInfo()
		{
			Dispose(false);
		}
	}

	public class TriangleInfoMap : IDisposable
	{
		internal IntPtr Native;
		bool _preventDelete;

		internal TriangleInfoMap(IntPtr native, bool preventDelete)
		{
			Native = native;
			_preventDelete = preventDelete;
		}

		public TriangleInfoMap()
		{
			Native = btTriangleInfoMap_new();
		}

		public int CalculateSerializeBufferSize()
		{
			return btTriangleInfoMap_calculateSerializeBufferSize(Native);
		}
		/*
		public void DeSerialize(TriangleInfoMapData data)
		{
			btTriangleInfoMap_deSerialize(Native, data._native);
		}
		*/
		public string Serialize(IntPtr dataBuffer, Serializer serializer)
		{
			return Marshal.PtrToStringAnsi(btTriangleInfoMap_serialize(Native, dataBuffer, serializer._native));
		}

		public double ConvexEpsilon
		{
			get => btTriangleInfoMap_getConvexEpsilon(Native);
			set => btTriangleInfoMap_setConvexEpsilon(Native, value);
		}

		public double EdgeDistanceThreshold
		{
			get => btTriangleInfoMap_getEdgeDistanceThreshold(Native);
			set => btTriangleInfoMap_setEdgeDistanceThreshold(Native, value);
		}

		public double EqualVertexThreshold
		{
			get => btTriangleInfoMap_getEqualVertexThreshold(Native);
			set => btTriangleInfoMap_setEqualVertexThreshold(Native, value);
		}

		public double MaxEdgeAngleThreshold
		{
			get => btTriangleInfoMap_getMaxEdgeAngleThreshold(Native);
			set => btTriangleInfoMap_setMaxEdgeAngleThreshold(Native, value);
		}

		public double PlanarEpsilon
		{
			get => btTriangleInfoMap_getPlanarEpsilon(Native);
			set => btTriangleInfoMap_setPlanarEpsilon(Native, value);
		}

		public double ZeroAreaThreshold
		{
			get => btTriangleInfoMap_getZeroAreaThreshold(Native);
			set => btTriangleInfoMap_setZeroAreaThreshold(Native, value);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Native != IntPtr.Zero)
			{
				if (!_preventDelete)
				{
					btTriangleInfoMap_delete(Native);
				}
				Native = IntPtr.Zero;
			}
		}

		~TriangleInfoMap()
		{
			Dispose(false);
		}
	}
}
