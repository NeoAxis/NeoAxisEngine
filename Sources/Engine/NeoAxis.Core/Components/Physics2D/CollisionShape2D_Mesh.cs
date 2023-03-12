// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Internal.tainicom.Aether.Physics2D;
using Internal.tainicom.Aether.Physics2D.Dynamics;
using Internal.tainicom.Aether.Physics2D.Common;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Mesh-based 2D collision shape.
	/// </summary>
	[NewObjectDefaultName( "Mesh Shape" )]
	[AddToResourcesWindow( @"Base\2D\Mesh Shape 2D", -7995 )]
	public class CollisionShape2D_Mesh : CollisionShape2D
	{
		Vector3F[] processedVertices;
		int[] processedIndices;
		int[] processedTrianglesToSourceIndex;

		[Browsable( false )]
		public bool CheckValidData { get; set; } = true;
		[Browsable( false )]
		public bool MergeEqualVerticesRemoveInvalidTriangles { get; set; } = true;

		/////////////////////////////////////////

		/// <summary>
		/// The mesh used by the collision shape.
		/// </summary>
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set
			{
				if( _mesh.BeginSet( ref value ) )
				{
					try
					{
						MeshChanged?.Invoke( this );
						NeedUpdateCachedArea();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _mesh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<CollisionShape2D_Mesh> MeshChanged;
		ReferenceField<Mesh> _mesh;

		/// <summary>
		/// The reference to the vertex array of the mesh.
		/// </summary>
		[Cloneable( CloneType.Shallow )]
		public Reference<Vector3F[]> Vertices
		{
			get { if( _vertices.BeginGet() ) Vertices = _vertices.Get( this ); return _vertices.value; }
			set
			{
				if( _vertices.BeginSet( ref value ) )
				{
					try
					{
						VerticesChanged?.Invoke( this );
						NeedUpdateCachedArea();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _vertices.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Vertices"/> property value changes.</summary>
		public event Action<CollisionShape2D_Mesh> VerticesChanged;
		ReferenceField<Vector3F[]> _vertices;

		/// <summary>
		/// The reference to the index array of the mesh.
		/// </summary>
		[Cloneable( CloneType.Shallow )]
		public Reference<int[]> Indices
		{
			get { if( _indices.BeginGet() ) Indices = _indices.Get( this ); return _indices.value; }
			set
			{
				if( _indices.BeginSet( ref value ) )
				{
					try
					{
						IndicesChanged?.Invoke( this );
						NeedUpdateCachedArea();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _indices.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Indices"/> property value changes.</summary>
		public event Action<CollisionShape2D_Mesh> IndicesChanged;
		ReferenceField<int[]> _indices;

		/// <summary>
		/// Enumerates the types of collision shape being created internally.
		/// </summary>
		public enum ShapeTypeEnum
		{
			Auto,
			TriangleMesh,
			Convex,
		}

		/// <summary>
		/// Defines the type of collision shape being created internally.
		/// </summary>
		[DefaultValue( ShapeTypeEnum.Auto )]
		public Reference<ShapeTypeEnum> ShapeType
		{
			get { if( _shapeType.BeginGet() ) ShapeType = _shapeType.Get( this ); return _shapeType.value; }
			set
			{
				if( _shapeType.BeginSet( ref value ) )
				{
					try
					{
						ShapeTypeChanged?.Invoke( this );
						NeedUpdateCachedArea();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _shapeType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ShapeType"/> property value changes.</summary>
		public event Action<CollisionShape2D_Mesh> ShapeTypeChanged;
		ReferenceField<ShapeTypeEnum> _shapeType = ShapeTypeEnum.Auto;

		/////////////////////////////////////////

		public bool GetSourceData( out Vector3F[] vertices, out int[] indices )
		{
			var mesh = Mesh.Value;
			if( mesh != null )
			{
				if( mesh.Result != null )
				{
					vertices = mesh.Result.ExtractedVerticesPositions;
					indices = mesh.Result.ExtractedIndices;
				}
				else
				{
					vertices = null;
					indices = null;
				}
			}
			else
			{
				vertices = Vertices;
				indices = Indices;
			}

			return vertices != null && vertices.Length != 0 && indices != null && indices.Length != 0;
		}

		protected internal override IList<Fixture> CreateShape( Body body, Transform shapeTransform, List<Vector2> rigidBodyLocalPoints )
		{
			var epsilon = 0.0001f;

			//clear data
			processedVertices = null;
			processedIndices = null;
			processedTrianglesToSourceIndex = null;

			//get source geometry
			if( !GetSourceData( out var sourceVertices, out var sourceIndices ) )
				return null;

			//check valid data
			if( CheckValidData )
			{
				if( !MathAlgorithms.CheckValidVertexIndexBuffer( sourceVertices.Length, sourceIndices, false ) )
				{
					Log.Info( "CollisionShape2D_Mesh: CreateShape: Invalid source data." );
					return null;
				}
			}

			//process geometry
			if( MergeEqualVerticesRemoveInvalidTriangles )
			{
				MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( sourceVertices, sourceIndices, epsilon, epsilon, true, true, out processedVertices, out processedIndices, out processedTrianglesToSourceIndex );
			}
			else
			{
				processedVertices = sourceVertices;
				processedIndices = sourceIndices;
			}

			if( ShapeType.Value == ShapeTypeEnum.Auto && ParentRigidBody.MotionType.Value == RigidBody2D.MotionTypeEnum.Dynamic && MathAlgorithms.IsMeshConvex( processedVertices, processedIndices, epsilon ) || ShapeType.Value == ShapeTypeEnum.Convex )
			{
				//convex

				var points = new List<Vector2>( processedVertices.Length );
				Bounds bounds = Bounds.Cleared;
				foreach( var p in processedVertices )
				{
					var p2 = shapeTransform * new Vector3( p.ToVector2(), 0 );
					points.Add( p2.ToVector2() );
					bounds.Add( p2 );
					//points.Add( ( shapeTransform * new Vector3( p.ToVector2(), 0 ) ).ToVector2() );
				}

				var fixtures = new List<Fixture>();
				{
					var currentList = new Vertices( points.Count );

					for( int vertex = 0; vertex < points.Count; vertex++ )
					{
						currentList.Add( Physics2DUtility.Convert( points[ vertex ] ) );

						if( currentList.Count == Settings.MaxPolygonVertices )
						{
							fixtures.Add( body.CreatePolygon( currentList, 0 ) );

							currentList = new Vertices();
							currentList.Add( Physics2DUtility.Convert( points[ 0 ] ) );
							currentList.Add( Physics2DUtility.Convert( points[ vertex ] ) );
						}
					}

					if( currentList.Count >= 3 )
						fixtures.Add( body.CreatePolygon( currentList, 0 ) );
				}

				//rigidBodyLocalPoints
				{
					var r = bounds.ToRectangle();
					rigidBodyLocalPoints.Add( r.LeftTop );
					rigidBodyLocalPoints.Add( r.RightTop );
					rigidBodyLocalPoints.Add( r.RightBottom );
					rigidBodyLocalPoints.Add( r.LeftBottom );
				}

				if( fixtures.Count != 0 )
					return fixtures.ToArray();

				//var points = new List<Vector2>();
				//foreach( var p in processedVertices )
				//	points.Add( ( shapeTransform * new Vector3( p.ToVector2(), 0 ) ).ToVector2() );

				//var vertices = new Vertices( points.Count );
				//foreach( var p in points )
				//	vertices.Add( Physics2DUtility.Convert( p ) );

				//if( vertices.Count > 1 )
				//	return new Fixture[] { body.CreatePolygon( vertices, 0 ) };
			}
			else
			{
				//chain shapes

				var fixtures = new List<Fixture>();
				Rectangle bounds = Rectangle.Cleared;


				//!!!!

				ESet<(Vector2, Vector2)> wasAdded = new ESet<(Vector2, Vector2)>();

				void Add( Vector2 p1, Vector2 p2 )
				{
					if( p1 != p2 && !wasAdded.Contains( (p1, p2) ) && !wasAdded.Contains( (p2, p1) ) )
					{
						wasAdded.Add( (p1, p2) );

						fixtures.Add( body.CreateEdge( Physics2DUtility.Convert( p1 ), Physics2DUtility.Convert( p2 ) ) );

						bounds.Add( p1 );
						bounds.Add( p2 );
					}
				}

				for( int nTriangle = 0; nTriangle < processedIndices.Length / 3; nTriangle++ )
				{
					var v0 = shapeTransform * processedVertices[ processedIndices[ nTriangle * 3 + 0 ] ];
					var v1 = shapeTransform * processedVertices[ processedIndices[ nTriangle * 3 + 1 ] ];
					var v2 = shapeTransform * processedVertices[ processedIndices[ nTriangle * 3 + 2 ] ];

					Add( v0.ToVector2(), v1.ToVector2() );
					Add( v1.ToVector2(), v2.ToVector2() );
					Add( v2.ToVector2(), v0.ToVector2() );
				}





				//rectangle

				//for( int nTriangle = 0; nTriangle < processedIndices.Length / 3; nTriangle++ )
				//{
				//	//!!!!slowly shapeTransform * 
				//	var v0 = shapeTransform * processedVertices[ processedIndices[ nTriangle * 3 + 0 ] ];
				//	var v1 = shapeTransform * processedVertices[ processedIndices[ nTriangle * 3 + 1 ] ];
				//	var v2 = shapeTransform * processedVertices[ processedIndices[ nTriangle * 3 + 2 ] ];
				//	bounds.Add( v0 );
				//	bounds.Add( v1 );
				//	bounds.Add( v2 );
				//}

				//var vertices = new Vertices();
				//var r = bounds.ToRectangle();
				//vertices.Add( Physics2DUtility.Convert( r.LeftTop ) );
				//vertices.Add( Physics2DUtility.Convert( r.RightTop ) );
				//vertices.Add( Physics2DUtility.Convert( r.RightBottom ) );
				//vertices.Add( Physics2DUtility.Convert( r.LeftBottom ) );



				//local points for space bounds calculation
				rigidBodyLocalPoints.Add( bounds.LeftTop );
				rigidBodyLocalPoints.Add( bounds.RightTop );
				rigidBodyLocalPoints.Add( bounds.RightBottom );
				rigidBodyLocalPoints.Add( bounds.LeftBottom );

				return fixtures.ToArray();

				//return new Fixture[] { body.CreateLoopShape( vertices ) };
			}

			return null;
		}

		public bool GetProcessedData( out Vector3F[] processedVertices, out int[] processedIndices, out int[] processedTrianglesToSourceIndex )
		{
			processedVertices = this.processedVertices;
			processedIndices = this.processedIndices;
			processedTrianglesToSourceIndex = this.processedTrianglesToSourceIndex;
			return processedVertices != null;
		}

		public bool GetData( out Vector3F[] vertices, out int[] indices )
		{
			if( GetProcessedData( out vertices, out indices, out _ ) )
				return true;
			return GetSourceData( out vertices, out indices );
		}

		public bool GetTriangleSourceData( int triangleID, bool applyWorldTransform, out Triangle triangle )
		{
			if( !GetSourceData( out var vertices, out var indices ) )
			{
				triangle = new Triangle();
				return false;
			}
			return GetTriangleData( triangleID, applyWorldTransform, vertices, indices, out triangle );
		}

		public bool GetTriangleProcessedData( int triangleID, bool applyWorldTransform, out Triangle triangle )
		{
			if( !GetProcessedData( out var vertices, out var indices, out _ ) )
			{
				triangle = new Triangle();
				return false;
			}
			return GetTriangleData( triangleID, applyWorldTransform, vertices, indices, out triangle );
		}

		bool GetTriangleData( int triangleID, bool applyWorldTransform, Vector3F[] vertices, int[] indices, out Triangle triangle )
		{
			if( applyWorldTransform )
			{
				var t = ParentRigidBody.Transform.Value.ToMatrix4();
				var local = TransformRelativeToParent.Value;
				if( local != null && !local.IsIdentity )
					t *= local.ToMatrix4();
				return MathAlgorithms.GetTriangleData( triangleID, ref t, vertices, indices, out triangle );
			}
			else
				return MathAlgorithms.GetTriangleData( triangleID, null, vertices, indices, out triangle );
		}

		protected internal override void Render( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered )
		{
			Matrix4 t = bodyTransform.ToMatrix4();
			var local = TransformRelativeToParent.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();

			if( GetData( out var vertices, out var indices ) )
			{
				if( indices != null )
					viewport.Simple3DRenderer.AddTriangles( vertices, indices, t, !solid, false );
				else
					viewport.Simple3DRenderer.AddTriangles( vertices, t, !solid, false );
				verticesRendered += vertices.Length;
			}

			//var points = new List<Vector3>( Points.Count );
			//foreach( var p in Points )
			//	points.Add( t * new Vector3( p.Value, 0 ) );

			//if( points.Count > 1 )
			//{
			//	if( solid )
			//	{
			//		Vector3 center = Vector3.Zero;
			//		foreach( var p in points )
			//			center += p;
			//		center /= points.Count;

			//		var vertices = new List<Vector3>( points.Count * 3 );
			//		for( int n = 0; n < points.Count; n++ )
			//		{
			//			var p1 = points[ n ];
			//			var p2 = points[ ( n + 1 ) % points.Count ];

			//			vertices.Add( center );
			//			vertices.Add( p1 );
			//			vertices.Add( p2 );
			//		}

			//		viewport.Simple3DRenderer.AddTriangles( vertices, false, true );
			//		verticesRendered += vertices.Count;
			//	}
			//	else
			//	{
			//		for( int n = 0; n < points.Count; n++ )
			//		{
			//			var p1 = points[ n ];
			//			var p2 = points[ ( n + 1 ) % points.Count ];
			//			viewport.Simple3DRenderer.AddLine( p1, p2 );
			//		}
			//		verticesRendered += points.Count * 2;
			//	}
			//}
		}

		//double AreaOfTriangle( ref Vector2 pt1, ref Vector2 pt2, ref Vector2 pt3 )
		//{
		//	double a = ( pt1 - pt2 ).Length();
		//	double b = ( pt2 - pt3 ).Length();
		//	double c = ( pt3 - pt1 ).Length();
		//	double s = ( a + b + c ) / 2;
		//	return Math.Sqrt( s * ( s - a ) * ( s - b ) * ( s - c ) );
		//}

		protected override double OnCalculateArea()
		{
			var result = 0.0;

			//!!!!
			result = 1;

			//var scale = TransformRelativeToParent.Value.Scale.ToVector2();

			//var points = new List<Vector2>( Points.Count );
			//foreach( var p in Points )
			//	points.Add( p.Value * scale );

			//Vector2 center = Vector2.Zero;
			//foreach( var p in points )
			//	center += p;
			//center /= points.Count;

			//for( int n = 0; n < points.Count; n++ )
			//{
			//	var p1 = points[ n ];
			//	var p2 = points[ ( n + 1 ) % points.Count ];
			//	result += AreaOfTriangle( ref center, ref p1, ref p2 );
			//}

			return result;
		}
	}
}
