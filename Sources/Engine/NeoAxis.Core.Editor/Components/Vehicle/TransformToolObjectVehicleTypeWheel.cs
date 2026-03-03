// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Object implemetation of <see cref="TransformToolClass"/> for vehicle type wheel.
	/// </summary>
	public class TransformToolObjectVehicleTypeWheel : TransformToolObject
	{
		VehicleTypeWheel selectedObject;

		//for undo
		Reference<Vector3> beforeModifyTransform;

		//

		public TransformToolObjectVehicleTypeWheel( VehicleTypeWheel vehicleTypeWheel )
			: base( vehicleTypeWheel )
		{
			this.selectedObject = vehicleTypeWheel;
		}

		public VehicleTypeWheel SelectedObject
		{
			get { return selectedObject; }
		}

		public VehicleTypeWheel ObjectToTransform
		{
			get { return selectedObject; }// ObjectInSpaceUtility.CalculateObjectToTransform( selectedObject ); }
		}

		public Reference<Vector3> BeforeModifyTransform
		{
			get { return beforeModifyTransform; }
		}

		public override bool IsAllowMove()
		{
			return ObjectToTransform != null;
		}

		public override bool IsAllowRotate()
		{
			return false;//return ObjectToTransform != null;
		}

		public override bool IsAllowScale()
		{
			return false;//return ObjectToTransform != null;
		}

		VehicleTypeWheel GetObjectToTransformOrSelected()
		{
			var objectToTransform = ObjectToTransform;
			return objectToTransform != null ? objectToTransform : selectedObject;
		}

		public override Vector3 Position
		{
			get { return GetObjectToTransformOrSelected().Position.Value; }
			set
			{
				if( IsAllowMove() )
				{
					var objectToTransform = ObjectToTransform;
					if( objectToTransform != null )
						objectToTransform.Position = value;
				}
			}
		}

		public override Quaternion Rotation
		{
			get { return Quaternion.Identity; }
			set { }
		}

		public override Vector3 Scale
		{
			get { return Vector3.One; }
			set { }
		}

		public override void OnModifyBegin()
		{
			base.OnModifyBegin();

			var objectToTransform = ObjectToTransform;
			if( objectToTransform != null )
				beforeModifyTransform = objectToTransform.Position;
		}

		public override void OnModifyCommit()
		{
			base.OnModifyCommit();
		}

		public override void OnModifyCancel()
		{
			base.OnModifyCancel();
		}
	}
}
