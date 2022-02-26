using Internal.BulletSharp.Math;
using System;
using System.Runtime.InteropServices;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class CompoundShapeChild
	{
		internal IntPtr Native;

		private CollisionShape _childShape;

		internal CompoundShapeChild(IntPtr native, CollisionShape childShape)
		{
			Native = native;
			_childShape = childShape;
		}

		public double ChildMargin
		{
			get => btCompoundShapeChild_getChildMargin(Native);
			set => btCompoundShapeChild_setChildMargin(Native, value);
		}

		public CollisionShape ChildShape
		{
			get => _childShape;
			set
			{
				btCompoundShapeChild_setChildShape(Native, value.Native);
				_childShape = value;
			}
		}

		public BroadphaseNativeType ChildShapeType
		{
			get => btCompoundShapeChild_getChildShapeType(Native);
			set => btCompoundShapeChild_setChildShapeType(Native, value);
		}

		public DbvtNode Node
		{
			get
			{
				IntPtr ptr = btCompoundShapeChild_getNode(Native);
				return (ptr != IntPtr.Zero) ? new DbvtNode(ptr) : null;
			}
			set => btCompoundShapeChild_setNode(Native, (value != null) ? value.Native : IntPtr.Zero);
		}

		public BMatrix Transform
		{
			get
			{
				BMatrix value;
				btCompoundShapeChild_getTransform(Native, out value);
				return value;
			}
			set => btCompoundShapeChild_setTransform(Native, ref value);
		}
	}

	public class CompoundShape : CollisionShape
	{
		private CompoundShapeChildArray _childList;

		public CompoundShape(bool enableDynamicAabbTree = true, int initialChildCapacity = 0)
			: base(btCompoundShape_new(enableDynamicAabbTree, initialChildCapacity))
		{
			_childList = new CompoundShapeChildArray(Native);
		}

		public void AddChildShapeRef(ref BMatrix localTransform, CollisionShape shape)
		{
			_childList.AddChildShape(ref localTransform, shape);
		}

		public void AddChildShape(BMatrix localTransform, CollisionShape shape)
		{
			_childList.AddChildShape(ref localTransform, shape);
		}

	   public void CalculatePrincipalAxisTransform(double[] masses, ref BMatrix principal,
			out BVector3 inertia)
		{
			btCompoundShape_calculatePrincipalAxisTransform(Native, masses,
				ref principal, out inertia);
		}

		public void CreateAabbTreeFromChildren()
		{
			btCompoundShape_createAabbTreeFromChildren(Native);
		}

		public CollisionShape GetChildShape(int index)
		{
			return _childList[index].ChildShape;
		}

		public void GetChildTransform(int index, out BMatrix value)
		{
			btCompoundShape_getChildTransform(Native, index, out value);
		}

		public BMatrix GetChildTransform(int index)
		{
			BMatrix value;
			btCompoundShape_getChildTransform(Native, index, out value);
			return value;
		}

		public void RecalculateLocalAabb()
		{
			btCompoundShape_recalculateLocalAabb(Native);
		}

		public void RemoveChildShape(CollisionShape shape)
		{
			_childList.RemoveChildShape(shape);
		}

		public void RemoveChildShapeByIndex(int childShapeIndex)
		{
			_childList.RemoveChildShapeByIndex(childShapeIndex);
		}

		public void UpdateChildTransform(int childIndex, BMatrix newChildTransform,
			bool shouldRecalculateLocalAabb = true)
		{
			btCompoundShape_updateChildTransform(Native, childIndex, ref newChildTransform,
				shouldRecalculateLocalAabb);
		}

		public CompoundShapeChildArray ChildList => _childList;

		public Dbvt DynamicAabbTree => new Dbvt(btCompoundShape_getDynamicAabbTree(Native), true);

		public int NumChildShapes => _childList.Count;

		public int UpdateRevision => btCompoundShape_getUpdateRevision(Native);
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct CompoundShapeData
	{
		public CollisionShapeData CollisionShapeData;
		public IntPtr ChildShapePtr;
		public int NumChildShapes;
		public float CollisionMargin;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(CompoundShapeData), fieldName).ToInt32(); }
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct CompoundShapeChildData
	{
		public TransformFloatData Transform;
		public IntPtr ChildShape;
		public int ChildShapeType;
		public float ChildMargin;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(CompoundShapeChildData), fieldName).ToInt32(); }
	}
}
