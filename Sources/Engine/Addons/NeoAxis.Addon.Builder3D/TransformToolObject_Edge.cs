// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NeoAxis.Editor;

namespace NeoAxis.Addon.Builder3D
{
	// см. комментарии в TransformToolObject_Face
	//
	class TransformToolObject_Edge : TransformToolObject
	{
		public readonly BuilderWorkareaMode.Edge Edge;

		public Vector3 Vertex1Offset;
		public Vector3 Vertex2Offset;

		readonly List<BuilderWorkareaMode.Vertex> meshVertices;
		readonly MeshData meshData;
		readonly Func<Vector3> getOperationCenter;

		////for undo
		//Transform beforeModifyTransform;

		//

		public TransformToolObject_Edge( BuilderWorkareaMode.Edge edge, MeshData meshData , List<BuilderWorkareaMode.Vertex> meshVertices, Func<Vector3> getOperationCenter)
			: base( edge )
		{
			Edge = edge;
			this.meshVertices = meshVertices;
			this.meshData = meshData;
			this.getOperationCenter = getOperationCenter;
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
			//не двигает когда выделено несколько объектов
			return true;
		}

		public override Vector3 Position
		{
			get { return Edge.Position; }
			set
			{
				if( IsAllowMove() )
				{
					Edge.Position = value;
					var e = meshData.Edges[ Edge.Index ];
					meshVertices[ e.Vertex1 ].Position = Vertex1Offset + Position;
					meshVertices[ e.Vertex2 ].Position = Vertex2Offset + Position;
				}
			}
		}

		public override Quaternion Rotation
		{
			get { return Edge.Rotation; }
			set
			{
				if( IsAllowRotate() )
				{
					Edge.Rotation = value;

					var e = meshData.Edges[ Edge.Index ];
					var rotation = Edge.Rotation;

					meshVertices[ e.Vertex1 ].Position = rotation * Vertex1Offset + Position;
					meshVertices[ e.Vertex2 ].Position = rotation * Vertex2Offset + Position;
				}
			}
		}

		public override Vector3 Scale
		{
			get { return Edge.Scale; }
			set
			{
				if( IsAllowScale() )
				{
					Edge.Scale = value;

					var e = meshData.Edges[ Edge.Index ];

					var scale = Edge.Scale;
					var operationCenter = getOperationCenter();
					meshVertices[ e.Vertex1 ].Position = scale * ( Vertex1Offset + Position - operationCenter ) + operationCenter;
					meshVertices[ e.Vertex2 ].Position = scale * ( Vertex2Offset + Position - operationCenter ) + operationCenter;
				}
			}
		}

		public override void OnModifyBegin()
		{
			base.OnModifyBegin();
			//beforeModifyTransform = new Transform( vertex.Position, vertex.Rotation, vertex.Scale );

			var e = meshData.Edges[ Edge.Index ];
			Vertex1Offset = meshVertices[ e.Vertex1 ].Position - Edge.Position;
			Vertex2Offset = meshVertices[ e.Vertex2 ].Position - Edge.Position;
		}

		public override void OnModifyCommit()
		{
			base.OnModifyCommit();
			Edge.Rotation = Quaternion.Identity;
			Edge.Scale = Vector3.One;
		}

		public override void OnModifyCancel()
		{
			base.OnModifyCancel();
		}
	}
}
