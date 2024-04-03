//#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Object implemetation of <see cref="ITransformTool"/>.
	/// </summary>
	public abstract class TransformToolObject
	{
		object controlledObject;
		//bool modifying;

		//

		protected TransformToolObject( object controlledObject )
		{
			this.controlledObject = controlledObject;
		}

		public object ControlledObject { get { return controlledObject; } }
		//public abstract object ControlledObject { get; }

		public virtual bool IsAllowMove() { return false; }
		public virtual bool IsAllowRotate() { return false; }
		public virtual bool IsAllowScale() { return false; }

		public abstract Vector3 Position { get; set; }
		public abstract Quaternion Rotation { get; set; }
		public abstract Vector3 Scale { get; set; }

		//public bool Modifying
		//{
		//	get { return modifying; }
		//}

		public virtual void OnModifyBegin() { }
		public virtual void OnModifyCommit() { }
		public virtual void OnModifyCancel() { }
		//public virtual void OnModifyBegin() { modifying = true; }
		//public virtual void OnModifyCommit() { modifying = false; }
		//public virtual void OnModifyCancel() { modifying = false; }
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum TransformToolMode
	{
		None,
		Position,
		Rotation,
		PositionRotation,
		Scale,
		Undefined
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum TransformToolCoordinateSystemMode
	{
		World,
		Local
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// A interface for tool for editing the transformation of objects.
	/// </summary>
	public interface ITransformTool
	{
		TransformToolMode Mode { get; set; }
		TransformToolCoordinateSystemMode CoordinateSystemMode { get; set; }
		List<TransformToolObject> Objects { get; }
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class TransformToolUtility
	{
		public delegate void ChangeMofidyStateDelegate( ITransformTool sender );
		public static event ChangeMofidyStateDelegate AllInstances_ModifyBegin;
		public static event ChangeMofidyStateDelegate AllInstances_ModifyCommit;
		public static event ChangeMofidyStateDelegate AllInstances_ModifyCancel;

		internal static void PerformAllInstances_ModifyBegin( ITransformTool sender )
		{
			AllInstances_ModifyBegin?.Invoke( sender );
		}

		internal static void PerformAllInstances_ModifyCommit( ITransformTool sender )
		{
			AllInstances_ModifyCommit?.Invoke( sender );
		}

		internal static void PerformAllInstances_ModifyCancel( ITransformTool sender )
		{
			AllInstances_ModifyCancel?.Invoke( sender );
		}
	}
}
//#endif