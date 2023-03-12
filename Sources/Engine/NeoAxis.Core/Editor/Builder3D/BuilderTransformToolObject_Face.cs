// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{

	//! Реализация хранит положение центра face в Position (среднее арифметическое), и координаты вертексов из этого face относительно центра Position
	//  При перемещении меняется Position и расчитываются новые положения вертексов на основе относительных Positions.
	//  Хранение вертексов в относительных координатах помогает избежать проблемы : один вертекс может перемещаться дважды если входит в два face,
	//  сейчас это не влияет на результат, хотя положение вертекса может несколько раз перезаписываться(одинаковыми значениями).
	//  Для альтернативного варианта - при изменении Position вычислять offset, и прибавлять его к абсолютным координатам - есть сложности отсекать смещение вертекса второй раз от второго face.
	//
	class BuilderTransformToolObject_Face : TransformToolObject
	{
		public readonly BuilderWorkareaMode.Face Face;
		public Vector3[] VertexOffsets;

		readonly List<BuilderWorkareaMode.Vertex> meshVertices;
		readonly BuilderMeshData meshData;
		readonly Func<Vector3> getOperationCenter;


		////for undo
		//Transform beforeModifyTransform;

		public BuilderTransformToolObject_Face( BuilderWorkareaMode.Face face, BuilderMeshData meshData, List<BuilderWorkareaMode.Vertex> meshVertices, Func<Vector3> getOperationCenter )
			: base( face )
		{
			Face = face;
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
			get { return Face.Position; }
			set
			{
				if( IsAllowMove() )
				{
					Face.Position = value;

					try
					{
						var triangles = meshData.Faces[ Face.Index ].Triangles;
						for( int i = 0; i < triangles.Count; i++ )
							meshVertices[ triangles[ i ].Vertex ].Position = VertexOffsets[ i ] + Position;
					}
					catch { }
				}
			}
		}


		public override Quaternion Rotation
		{
			get { return Face.Rotation; }
			set
			{
				if( IsAllowRotate() )
				{
					Face.Rotation = value;

					try
					{
						var triangles = meshData.Faces[ Face.Index ].Triangles;
						for( int i = 0; i < triangles.Count; i++ )
							meshVertices[ triangles[ i ].Vertex ].Position = Face.Rotation * VertexOffsets[ i ] + Face.Position;
					}
					catch { }
				}
			}
		}

		public override Vector3 Scale
		{
			get { return Face.Scale; }
			set
			{
				if( IsAllowScale() )
				{
					Face.Scale = value;
					var operationCenter = getOperationCenter();
					try
					{
						var triangles = meshData.Faces[ Face.Index ].Triangles;
						for( int i = 0; i < triangles.Count; i++ )
							meshVertices[ triangles[ i ].Vertex ].Position = Face.Scale * ( VertexOffsets[ i ] + Face.Position - operationCenter ) + operationCenter;
					}
					catch { }
				}
			}
		}

		public override void OnModifyBegin()
		{
			base.OnModifyBegin();
			//beforeModifyTransform = new Transform( vertex.Position, vertex.Rotation, vertex.Scale );

			try
			{
				var triangles = meshData.Faces[ Face.Index ].Triangles;
				var vertexOffsets = new Vector3[ triangles.Count ];
				for( int i = 0; i < triangles.Count; i++ )
					vertexOffsets[ i ] = meshVertices[ triangles[ i ].Vertex ].Position - Face.Position;
				VertexOffsets = vertexOffsets;
			}
			catch { }
		}

		public override void OnModifyCommit()
		{
			base.OnModifyCommit();

			try
			{
				for( int i = 0; i < VertexOffsets.Length; i++ )
					VertexOffsets[ i ] = Face.Rotation * ( Scale * VertexOffsets[ i ] );
				Face.Rotation = Quaternion.Identity;
				Face.Scale = Vector3.One;
			}
			catch { }
		}

		public override void OnModifyCancel()
		{
			base.OnModifyCancel();
		}
	}
}
#endif