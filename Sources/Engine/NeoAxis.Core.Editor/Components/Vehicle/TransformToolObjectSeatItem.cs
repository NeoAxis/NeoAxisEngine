// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Object implemetation of <see cref="TransformToolClass"/> for seat item.
	/// </summary>
	public class TransformToolObjectSeatItem : TransformToolObject
	{
		SeatItem selectedObject;

		//for undo
		Reference<Transform> beforeModifyTransform;

		//

		public TransformToolObjectSeatItem( SeatItem seatItem )
			: base( seatItem )
		{
			this.selectedObject = seatItem;
		}

		public SeatItem SelectedObject
		{
			get { return selectedObject; }
		}

		public SeatItem ObjectToTransform
		{
			get { return selectedObject; }// ObjectInSpaceUtility.CalculateObjectToTransform( selectedObject ); }
		}

		public Reference<Transform> BeforeModifyTransform
		{
			get { return beforeModifyTransform; }
		}

		public override bool IsAllowMove()
		{
			return ObjectToTransform != null;
		}

		public override bool IsAllowRotate()
		{
			return ObjectToTransform != null;
		}

		public override bool IsAllowScale()
		{
			return ObjectToTransform != null;
		}

		SeatItem GetObjectToTransformOrSelected()
		{
			var objectToTransform = ObjectToTransform;
			return objectToTransform != null ? objectToTransform : selectedObject;
		}

		public override Vector3 Position
		{
			get { return GetObjectToTransformOrSelected().Transform.Value.Position; }
			set
			{
				if( IsAllowMove() )
				{
					var objectToTransform = ObjectToTransform;
					if( objectToTransform != null )
						objectToTransform.Transform = objectToTransform.Transform.Value.UpdatePosition( value );
				}
			}
		}

		public override Quaternion Rotation
		{
			get { return GetObjectToTransformOrSelected().Transform.Value.Rotation; }
			set
			{
				if( IsAllowRotate() )
				{
					var objectToTransform = ObjectToTransform;
					if( objectToTransform != null )
						objectToTransform.Transform = objectToTransform.Transform.Value.UpdateRotation( value );
				}
			}
		}

		public override Vector3 Scale
		{
			get { return GetObjectToTransformOrSelected().Transform.Value.Scale; }
			set
			{
				if( IsAllowScale() )
				{
					var objectToTransform = ObjectToTransform;
					if( objectToTransform != null )
						objectToTransform.Transform = objectToTransform.Transform.Value.UpdateScale( value );
				}
			}
		}

		public override void OnModifyBegin()
		{
			base.OnModifyBegin();

			var objectToTransform = ObjectToTransform;
			if( objectToTransform != null )
				beforeModifyTransform = objectToTransform.Transform;
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
