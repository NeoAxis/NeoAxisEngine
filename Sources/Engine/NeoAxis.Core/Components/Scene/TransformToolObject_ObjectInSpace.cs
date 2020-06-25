// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Object implemetation of <see cref="TransformTool"/> for object in space.
	/// </summary>
	public class TransformToolObject_ObjectInSpace : TransformToolObject
	{
		Component_ObjectInSpace selectedObject;
		//Component_ObjectInSpace objectToTransform;

		//for undo
		Reference<Transform> beforeModifyTransform;

		//

		public TransformToolObject_ObjectInSpace( Component_ObjectInSpace selectedObject )
			: base( selectedObject )
		{
			this.selectedObject = selectedObject;
			//objectToTransform = Component_ObjectInSpace_Utility.CalculateObjectToTransform( this.selectedObject );
		}

		public Component_ObjectInSpace SelectedObject
		{
			get { return selectedObject; }
		}

		public Component_ObjectInSpace ObjectToTransform
		{
			get
			{
				return Component_ObjectInSpace_Utility.CalculateObjectToTransform( selectedObject );
				//return objectToTransform;
			}
		}

		public Reference<Transform> BeforeModifyTransform
		{
			get { return beforeModifyTransform; }
		}

		public override bool IsAllowMove()
		{
			return ObjectToTransform != null;
			//if( objectInSpace.Transform.ReferenceSpecified )
			//	return false;
			//return true;
		}

		public override bool IsAllowRotate()
		{
			return ObjectToTransform != null;
			//if( objectInSpace.Transform.ReferenceSpecified )
			//	return false;
			//return true;
		}

		public override bool IsAllowScale()
		{
			return ObjectToTransform != null;

			//if( objectInSpace.Transform.ReferenceSpecified )
			//	return false;

			////!!!!было
			////string reason;
			////if( !objectInSpace.IsAllowToChangeScale( out reason ) )
			////	return false;

			//return true;
		}

		Component_ObjectInSpace GetObjectToTransformOrSelected()
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

				//!!!!было
				//if( objectInSpace.AutoVerticalAlignment != MapObject.AutoVerticalAlignments.None )
				//	EntitiesEditManager.Instance.UpdateObjectVerticalAlignment( objectInSpace, objectInSpace.AutoVerticalAlignment );
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

				//!!!!было
				//if( objectInSpace.AutoVerticalAlignment != MapObject.AutoVerticalAlignments.None )
				//	EntitiesEditManager.Instance.UpdateObjectVerticalAlignment( objectInSpace, objectInSpace.AutoVerticalAlignment );
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

				//!!!!было
				//if( objectInSpace.AutoVerticalAlignment != MapObject.AutoVerticalAlignments.None )
				//	EntitiesEditManager.Instance.UpdateObjectVerticalAlignment( objectInSpace, objectInSpace.AutoVerticalAlignment );
			}
		}

		public override void OnModifyBegin()
		{
			base.OnModifyBegin();

			var objectToTransform = ObjectToTransform;
			if( objectToTransform != null )
				beforeModifyTransform = objectToTransform.Transform;

			//objectInSpace.Editor_BeginTransformModifying();
		}

		public override void OnModifyCommit()
		{
			base.OnModifyCommit();

			//objectInSpace.Editor_CommitTransformModifying();
		}

		public override void OnModifyCancel()
		{
			base.OnModifyCancel();

			//objectInSpace.Editor_CancelTransformModifying();
		}
	}
}
