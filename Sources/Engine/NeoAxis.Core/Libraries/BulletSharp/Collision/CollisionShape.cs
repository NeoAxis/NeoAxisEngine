using System;
using System.Runtime.InteropServices;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public abstract class CollisionShape : IDisposable
	{
		internal IntPtr Native;
		private bool _preventDelete;
		private bool _isDisposed;

		internal static CollisionShape GetManaged(IntPtr obj)
		{
			if (obj == IntPtr.Zero)
			{
				return null;
			}

			IntPtr userPtr = btCollisionShape_getUserPointer(obj);
			return GCHandle.FromIntPtr(userPtr).Target as CollisionShape;
		}

		internal CollisionShape(IntPtr native, bool preventDelete = false)
		{
			Native = native;
			if (preventDelete)
			{
				_preventDelete = true;
			}
			else
			{
				GCHandle handle = GCHandle.Alloc(this, GCHandleType.Weak);
				btCollisionShape_setUserPointer(native, GCHandle.ToIntPtr(handle));
			}
		}

		public BVector3 CalculateLocalInertia(double mass)
		{
			BVector3 inertia;
			btCollisionShape_calculateLocalInertia(Native, mass, out inertia);
			return inertia;
		}

		public void CalculateLocalInertia(double mass, out BVector3 inertia)
		{
			btCollisionShape_calculateLocalInertia(Native, mass, out inertia);
		}

		public int CalculateSerializeBufferSize()
		{
			return btCollisionShape_calculateSerializeBufferSize(Native);
		}

		public void CalculateTemporalAabbRef(ref BMatrix curTrans, ref BVector3 linvel, ref BVector3 angvel,
			double timeStep, out BVector3 temporalAabbMin, out BVector3 temporalAabbMax)
		{
			btCollisionShape_calculateTemporalAabb(Native, ref curTrans, ref linvel, ref angvel, timeStep, out temporalAabbMin, out temporalAabbMax);
		}

		public void CalculateTemporalAabb(BMatrix curTrans, BVector3 linvel, BVector3 angvel,
			double timeStep, out BVector3 temporalAabbMin, out BVector3 temporalAabbMax)
		{
			btCollisionShape_calculateTemporalAabb(Native, ref curTrans, ref linvel,
				ref angvel, timeStep, out temporalAabbMin, out temporalAabbMax);
		}

		public void GetAabbRef(ref BMatrix t, out BVector3 aabbMin, out BVector3 aabbMax)
		{
			btCollisionShape_getAabb(Native, ref t, out aabbMin, out aabbMax);
		}

		public void GetAabb(BMatrix t, out BVector3 aabbMin, out BVector3 aabbMax)
		{
			btCollisionShape_getAabb(Native, ref t, out aabbMin, out aabbMax);
		}

		public void GetBoundingSphere(out BVector3 center, out double radius)
		{
			btCollisionShape_getBoundingSphere(Native, out center, out radius);
		}

		public double GetContactBreakingThreshold(double defaultContactThresholdFactor)
		{
			return btCollisionShape_getContactBreakingThreshold(Native, defaultContactThresholdFactor);
		}

		public virtual string Serialize(IntPtr dataBuffer, Serializer serializer)
		{
			return Marshal.PtrToStringAnsi(btCollisionShape_serialize(Native, dataBuffer, serializer._native));
			/*
			IntPtr name = serializer.FindNameForPointer(_native);
			IntPtr namePtr = serializer.GetUniquePointer(name);
			Marshal.WriteIntPtr(dataBuffer, namePtr);
			if (namePtr != IntPtr.Zero)
			{
				serializer.SerializeName(name);
			}
			Marshal.WriteInt32(dataBuffer, IntPtr.Size, (int)ShapeType);
			//Marshal.WriteInt32(dataBuffer, IntPtr.Size + sizeof(int), 0); //padding
			return "btCollisionShapeData";
			*/
		}

		public void SerializeSingleShape(Serializer serializer)
		{
			int len = CalculateSerializeBufferSize();
			Chunk chunk = serializer.Allocate((uint)len, 1);
			string structType = Serialize(chunk.OldPtr, serializer);
			serializer.FinalizeChunk(chunk, structType, DnaID.Shape, Native);
		}

		public double AngularMotionDisc => btCollisionShape_getAngularMotionDisc(Native);

		public BVector3 AnisotropicRollingFrictionDirection
		{
			get
			{
				BVector3 value;
				btCollisionShape_getAnisotropicRollingFrictionDirection(Native, out value);
				return value;
			}
		}

		public bool IsCompound => btCollisionShape_isCompound(Native);

		public bool IsConcave => btCollisionShape_isConcave(Native);

		public bool IsConvex => btCollisionShape_isConvex(Native);

		public bool IsConvex2D => btCollisionShape_isConvex2d(Native);

		public bool IsDisposed => _isDisposed;

		public bool IsInfinite => btCollisionShape_isInfinite(Native);

		public bool IsNonMoving => btCollisionShape_isNonMoving(Native);

		public bool IsPolyhedral => btCollisionShape_isPolyhedral(Native);

		public bool IsSoftBody => btCollisionShape_isSoftBody(Native);

		public BVector3 LocalScaling
		{
			get
			{
				BVector3 value;
				btCollisionShape_getLocalScaling(Native, out value);
				return value;
			}
			set => btCollisionShape_setLocalScaling(Native, ref value);
		}

		public double Margin
		{
			get => btCollisionShape_getMargin(Native);
			set => btCollisionShape_setMargin(Native, value);
		}

		public string Name => Marshal.PtrToStringAnsi(btCollisionShape_getName(Native));

		public BroadphaseNativeType ShapeType => btCollisionShape_getShapeType(Native);

		public object UserObject { get; set; }

		public int UserIndex
		{
			get => btCollisionShape_getUserIndex(Native);
			set => btCollisionShape_setUserIndex(Native, value);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj);
		}

		public override int GetHashCode()
		{
			return Native.GetHashCode();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				_isDisposed = true;

				if (!_preventDelete)
				{
					IntPtr userPtr = btCollisionShape_getUserPointer(Native);
					GCHandle.FromIntPtr(userPtr).Free();

					btCollisionShape_delete(Native);
				}
			}
		}

		~CollisionShape()
		{
			Dispose(false);
		}
	}

    [StructLayout(LayoutKind.Sequential)]
	internal struct CollisionShapeData
	{
		public IntPtr Name;
		public int ShapeType;
		public int Padding;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(CollisionShapeData), fieldName).ToInt32(); }
	}
}
