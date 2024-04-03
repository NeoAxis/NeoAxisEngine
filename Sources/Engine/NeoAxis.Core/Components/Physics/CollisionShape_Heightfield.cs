//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Runtime.InteropServices;
//using System.Reflection;
//using System.IO;
//using BulletSharp;
//using BulletSharp.Math;

//namespace NeoAxis
//{
//	/// <summary>
//	/// Heightfield collision shape.
//	/// </summary>
//	public class CollisionShape_Heightfield : CollisionShape
//	{
//		//TriangleIndexVertexArray indexVertexArrays;

//		//!!!!все настройки
//		//PerTriangleMaterial[] perTriangleMaterials;
//		//short[] perTriangleMaterialIndices;

//		/////////////////////////////////////////

//		[DefaultValue( "0 0" )]
//		public Reference<Vector2I> VertexCount
//		{
//			get { if( _vertexCount.BeginGet() ) VertexCount = _vertexCount.Get( this ); return _vertexCount.value; }
//			set
//			{
//				if( _vertexCount.BeginSet( this, ref value ) )
//				{
//					try
//					{
//						VertexCountChanged?.Invoke( this );
//						NeedUpdateCachedVolume();
//						if( EnabledInHierarchy )
//							RecreateBody();
//						//!!!!bounds update?
//						//!!!!what else
//					}
//					finally { _vertexCount.EndSet(); }
//				}
//			}
//		}
//		/// <summary>Occurs when the <see cref="VertexCount"/> property value changes.</summary>
//		public event Action<CollisionShape_Heightfield> VertexCountChanged;
//		ReferenceField<Vector2I> _vertexCount;

//		/// <summary>
//		/// The reference to the height array of the heightfield.
//		/// </summary>
//		[Serialize]
//		[Cloneable( CloneType.Shallow )]//!!!!или только для типов Shallow? а как обычно - полностью?
//		public Reference<float[]> Heights
//		{
//			get { if( _heights.BeginGet() ) Heights = _heights.Get( this ); return _heights.value; }
//			set
//			{
//				if( _heights.BeginSet( this, ref value ) )
//				{
//					try
//					{
//						HeightsChanged?.Invoke( this );
//						NeedUpdateCachedVolume();
//						if( EnabledInHierarchy )
//							RecreateBody();
//						//!!!!bounds update?
//						//!!!!what else
//					}
//					finally { _heights.EndSet(); }
//				}
//			}
//		}
//		/// <summary>Occurs when the <see cref="Heights"/> property value changes.</summary>
//		public event Action<CollisionShape_Heightfield> HeightsChanged;
//		ReferenceField<float[]> _heights;

//		/////////////////////////////////////////

//		//!!!!
//		//public bool GetSourceData( out Vector3F[] vertices, out int[] indices )
//		//{
//		//	var mesh = Mesh.Value;
//		//	if( mesh != null )
//		//	{
//		//		if( mesh.Result != null )
//		//		{
//		//			vertices = mesh.Result.ExtractedVerticesPositions;
//		//			indices = mesh.Result.ExtractedIndices;
//		//		}
//		//		else
//		//		{
//		//			vertices = null;
//		//			indices = null;
//		//		}
//		//	}
//		//	else
//		//	{
//		//		vertices = Vertices;
//		//		indices = Indices;
//		//	}

//		//	return vertices != null && vertices.Length != 0 && indices != null && indices.Length != 0;
//		//}

//		protected internal override CollisionShape CreateShape()
//		{
//			//var epsilon = 0.0001f;

//			////clear data
//			//processedVertices = null;
//			//processedIndices = null;
//			//processedTrianglesToSourceIndex = null;

//			////get source geometry
//			//if( !GetSourceData( out var sourceVertices, out var sourceIndices ) )
//			//	return null;

//			////process geometry
//			////!!!!slowly. later use cached precalculated bullet shape.
//			//MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( sourceVertices, sourceIndices, epsilon, out processedVertices, out processedIndices, out processedTrianglesToSourceIndex );

//			//create bullet shape

//			var heights = Heights.Value;

//			float minHeight = float.MaxValue;
//			float maxHeight = float.MinValue;
//			foreach( var h in heights )
//			{
//				if( h < minHeight )
//					minHeight = h;
//				if( h > maxHeight )
//					maxHeight = h;
//			}

//			//!!!!temp
//			//!!!!leak
//			var data = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Physics, heights.Length * 4 );
//			unsafe
//			{
//				fixed ( float* pHeights = heights )
//				{
//					NativeUtility.CopyMemory( data, (IntPtr)pHeights, heights.Length * 4 );
//				}
//			}
//			//Marshal.Copy( heights, 0, data, heights.Length );

//			HeightfieldTerrainShape shape;

//			//!!!!
//			shape = new HeightfieldTerrainShape( VertexCount.Value.X, VertexCount.Value.Y, data, 1, minHeight, maxHeight, 2, PhyScalarType.Single, false );

//			//unsafe
//			//{
//			//	fixed ( float* pHeights = heights )
//			//	{
//			//		shape = new HeightfieldTerrainShape( VertexCount.Value.X, VertexCount.Value.Y, (IntPtr)pHeights, 1, minHeight, maxHeight, 2, PhyScalarType.Single, false );
//			//	}
//			//}

//			//!!!!
//			shape.SetUseDiamondSubdivision( true );
//			//shape.SetUseZigzagSubdivision();

//			return shape;
//		}

//		//!!!!

//		//public bool GetProcessedData( out Vector3F[] processedVertices, out int[] processedIndices, out int[] processedTrianglesToSourceIndex )
//		//{
//		//	processedVertices = this.processedVertices;
//		//	processedIndices = this.processedIndices;
//		//	processedTrianglesToSourceIndex = this.processedTrianglesToSourceIndex;
//		//	return processedVertices != null;
//		//}

//		//public bool GetData( out Vector3F[] vertices, out int[] indices )
//		//{
//		//	if( GetProcessedData( out vertices, out indices, out _ ) )
//		//		return true;
//		//	return GetSourceData( out vertices, out indices );
//		//}

//		//public bool GetTriangleSourceData( int triangleID, bool applyWorldTransform, out Triangle triangle )
//		//{
//		//	if( !GetSourceData( out var vertices, out var indices ) )
//		//	{
//		//		triangle = new Triangle();
//		//		return false;
//		//	}
//		//	return GetTriangleData( triangleID, applyWorldTransform, vertices, indices, out triangle );
//		//}

//		//public bool GetTriangleProcessedData( int triangleID, bool applyWorldTransform, out Triangle triangle )
//		//{
//		//	if( !GetProcessedData( out var vertices, out var indices, out _ ) )
//		//	{
//		//		triangle = new Triangle();
//		//		return false;
//		//	}
//		//	return GetTriangleData( triangleID, applyWorldTransform, vertices, indices, out triangle );
//		//}

//		//private bool GetTriangleData( int triangleID, bool applyWorldTransform, Vector3F[] vertices, int[] indices, out Triangle triangle )
//		//{
//		//	if( applyWorldTransform )
//		//	{
//		//		var t = ParentRigidBody.Transform.Value.ToMatrix4();
//		//		var local = TransformRelativeToParent.Value;
//		//		if( local != null && !local.IsIdentity )
//		//			t *= local.ToMatrix4();
//		//		return MathAlgorithms.GetTriangleData( triangleID, ref t, vertices, indices, out triangle );
//		//	}
//		//	else
//		//		return MathAlgorithms.GetTriangleData( triangleID, null, vertices, indices, out triangle );
//		//}

//		protected internal override void DebugRender( Viewport viewport, Transform bodyTransform, bool solid )
//		{
//			Matrix4 t = bodyTransform.ToMatrix4();
//			var local = TransformRelativeToParent.Value;
//			if( !local.IsIdentity )
//				t *= local.ToMatrix4();

//			//!!!!

//			//if( GetData( out Vector3F[] vertices, out int[] indices ) )
//			//{
//			//	if( indices != null )
//			//		viewport.Simple3DRenderer.AddTriangles( vertices, indices, t, !solid, false );
//			//	else
//			//		viewport.Simple3DRenderer.AddTriangles( vertices, t, !solid, false );
//			//}
//		}

//		protected override double OnCalculateVolume()
//		{
//			//!!!!
//			return 1;
//		}

//		internal override Vector3 GetCenterOfMassPositionNotScaledByParent()
//		{
//			return base.GetCenterOfMassPositionNotScaledByParent();
//		}
//	}
//}
