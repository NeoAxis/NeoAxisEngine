// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !PROJECT_DEPLOY
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NeoAxis.Editor;

namespace NeoAxis.Addon.Builder3D
{
	class TransformToolObject_Vertex : TransformToolObject
	{
		BuilderWorkareaMode.Vertex vertex;

		////for undo
		//Transform beforeModifyTransform;

		public Vector3 PositionBeforeScaling;
		readonly Func<Vector3> getOperationCenter;

		public TransformToolObject_Vertex( BuilderWorkareaMode.Vertex vertex, Func<Vector3> getOperationCenter )
			: base( vertex )
		{
			this.vertex = vertex;
			this.getOperationCenter = getOperationCenter;
		}

		public BuilderWorkareaMode.Vertex Vertex
		{
			get { return vertex; }
		}

		//public Transform BeforeModifyTransform
		//{
		//	get { return beforeModifyTransform; }
		//}

		public override bool IsAllowMove()
		{
			return true;
		}

		public override bool IsAllowRotate()
		{
			return true;
		}

		public override bool IsAllowScale()
		{
			return true;
		}

		public override Vector3 Position
		{
			get { return vertex.Position; }
			set
			{
				if( IsAllowMove() )
				{
					vertex.Position = value;
				}
			}
		}

		public override Quaternion Rotation
		{
			get { return vertex.Rotation; }
			set
			{
				if( IsAllowRotate() )
					vertex.Rotation = value;
			}
		}

		public override Vector3 Scale
		{
			get { return vertex.Scale; }
			set
			{
				if( IsAllowScale() )
				{
					vertex.Scale = value;
					var operationCenter = getOperationCenter();
					Vertex.Position = Vertex.Scale * ( PositionBeforeScaling - operationCenter ) + operationCenter;
				}
			}
		}

		public override void OnModifyBegin()
		{
			base.OnModifyBegin();

			PositionBeforeScaling = vertex.Position;

			//beforeModifyTransform = new Transform( vertex.Position, vertex.Rotation, vertex.Scale );
		}

		public override void OnModifyCommit()
		{
			base.OnModifyCommit();
			vertex.Scale = Vector3.One;
			vertex.Rotation = Quaternion.Identity;
		}

		public override void OnModifyCancel()
		{
			base.OnModifyCancel();
		}
	}
}
#endif