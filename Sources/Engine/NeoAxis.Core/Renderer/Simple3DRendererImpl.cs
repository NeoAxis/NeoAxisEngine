// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using SharpBgfx;
using System.Linq;

namespace NeoAxis
{
	class Simple3DRendererImpl : Simple3DRenderer
	{
		//!!!!если стенсил юзать, то можно рисовать правильнее. без наложений если альфа < 1

		//!!!!
		const int constBufferVertexCount = 1024;
		const int constBufferIndexCount = 4098;

		Viewport viewport;

		ColorValue currentColor = new ColorValue( -1, -1, -1, -1 );
		uint currentColorPacked;

		ColorValue currentColorInvisibleBehindObjects = new ColorValue( -1, -1, -1, -1 );
		uint currentColorInvisibleBehindObjectsPacked;

		bool currentDepthTest = true;
		bool currentDepthWrite = false;
		//bool specialDepthSettingsActivated;

		OcclusionQuery? currentOcclusionQuery;

		//!!!!impl
		//int nonOverlappingGroupCounter;
		//int nonOverlappingGroup;

		Stack<Item_DynamicallyCreated> freeItemsDynamicallyCreated = new Stack<Item_DynamicallyCreated>();
		List<Item> items = new List<Item>();

		Stack<RenderableItem_DynamicallyCreated> freeRenderablesDynamicallyCreated = new Stack<RenderableItem_DynamicallyCreated>();
		List<RenderableItem> preparedRenderables = new List<RenderableItem>();

		static GpuMaterialPass oneMaterial;

		//optimization
		static Vector3F[] boxShapePositions;
		static int[] boxShapeIndices;

		//optimization
		static Vector3F[] boundsTempPositions = new Vector3F[ 8 ];
		static int[] boundsIndices = new int[] {
			0, 3, 1,
			0, 2, 3,
			3, 6, 7,
			3, 2, 6,
			1, 7, 5,
			1, 3, 7,
			4, 7, 6,
			4, 5, 7,
			1, 4, 0,
			5, 4, 1,
			4, 2, 0,
			4, 6, 2 };

		static Vector3F[] addLineTempPositions = new Vector3F[ 8 ];

		static Matrix4 transformIdentity = Matrix4.Identity;

		//const double fastLineThicknessThreshold = 10;

		////////////////////////////////////////

		abstract class RenderableItem
		{
			public Item currentItem;
		}

		////////////////////////////////////////

		class RenderableItem_DynamicallyCreated : RenderableItem
		{
			public GpuVertexBuffer vertexBuffer;
			public int vertexCount;
			//!!!!глючит на HD7850
			//public GpuIndexBuffer indexBuffer;
			//public int indexCount;
		}

		////////////////////////////////////////

		class RenderableItem_VertexIndexData : RenderableItem
		{
			public VertexIndexData data;
		}

		///////////////////////////////////////////

		abstract class Item
		{
			public bool depthTest;
			public bool depthWrite;
			public OcclusionQuery? occlusionQuery;
			//public int nonOverlappingGroup;

			public Matrix4 transform;
			public bool transformIsIdentity;

			public bool wireframe;
			public bool culling;
		}

		///////////////////////////////////////////

		class Item_DynamicallyCreated : Item
		{
			public enum ItemType
			{
				Lines,
				Triangles,
			}
			public ItemType type;
			//public bool depthTest;
			//public bool depthWrite;

			public unsafe byte[] vertices = new byte[ constBufferVertexCount * sizeof( Vertex ) ];
			//public Vertex[] vertices = new Vertex[ constBufferVertexCount ];

			public int vertexCount;
			public int[] indices = new int[ constBufferIndexCount ];
			public int indexCount;
			public bool actualIndexData;

			////triangles specific
			//public Mat4 transform;
			//public bool wireframe;
			//public bool culling;
		}

		///////////////////////////////////////////

		class Item_VertexIndexData : Item
		{
			public VertexIndexData data;

			//!!!!
			public ColorValue color;
			public ColorValue colorInvisibleBehindObjects;

			//public bool depthTest;
			//public bool depthWrite;

			//public Mat4 transform;
			//public bool wireframe;
			//public bool culling;
		}

		/////////////////////////////////////////

		public Simple3DRendererImpl( Viewport viewport )
		{
			this.viewport = viewport;

			InitInternal();
		}

		public void Dispose()
		{
			ShutdownInternal();
		}

		public override Viewport Viewport
		{
			get { return viewport; }
		}

		public override void SetColor( ColorValue colorVisible, ColorValue colorInvisibleBehindObjects )//, bool depthWrite = false )
		{
			if( currentColor != colorVisible )
			{
				currentColor = colorVisible;
				currentColorPacked = RenderingSystem.ConvertColorValue( ref currentColor );
			}

			if( currentColorInvisibleBehindObjects != colorInvisibleBehindObjects )
			{
				currentColorInvisibleBehindObjects = colorInvisibleBehindObjects;
				currentColorInvisibleBehindObjectsPacked = RenderingSystem.ConvertColorValue( ref currentColorInvisibleBehindObjects );

				currentDepthTest = currentColorInvisibleBehindObjects == new ColorValue( 0, 0, 0, 0 );
			}

			//!!!!depth write is not supported
			currentDepthWrite = false;//depthWrite;
		}

		public override void SetOcclusionQuery( OcclusionQuery? query )
		{
			currentOcclusionQuery = query;
		}

		//public override void EnableNonOverlappingGroup()
		//{
		//	nonOverlappingGroupCounter++;
		//	nonOverlappingGroup = nonOverlappingGroupCounter;
		//}

		//public override void DisableNonOverlappingGroup()
		//{
		//	nonOverlappingGroup = 0;
		//}

		//public override void SetSpecialDepthSettings( bool depthTest, bool depthWrite )
		//{
		//	if( specialDepthSettingsActivated )
		//		Log.Fatal( "Simple3DRenderer: SetSpecialDepthSettings: Special settings already activated." );

		//	specialDepthSettingsActivated = true;
		//	currentDepthTest = depthTest;
		//	currentDepthWrite = depthWrite;
		//}

		//public override void RestoreDefaultDepthSettings()
		//{
		//	if( !specialDepthSettingsActivated )
		//		Log.Fatal( "Simple3DRenderer: RestoreDefaultDepthSettings: Nothing to restore. Special settings are not activated." );

		//	specialDepthSettingsActivated = false;
		//	currentDepthTest = true;
		//	currentDepthWrite = false;
		//}

		public override void AddBounds( Bounds bounds, bool solid, double lineThickness )
		{
			//!!!!группой рисовать. получается, могут быть вложенные группы
			//!!!!!!всем методам такое сделать

			if( solid )
			{
				//!!!!double

				var center = bounds.GetCenter().ToVector3F();
				var half = bounds.GetSize().ToVector3F() * .5f;

				boundsTempPositions[ 0 ] = center + new Vector3F( half.X, -half.Y, -half.Z );
				boundsTempPositions[ 1 ] = center + new Vector3F( half.X, -half.Y, half.Z );
				boundsTempPositions[ 2 ] = center + new Vector3F( half.X, half.Y, -half.Z );
				boundsTempPositions[ 3 ] = center + new Vector3F( half.X, half.Y, half.Z );
				boundsTempPositions[ 4 ] = center + new Vector3F( -half.X, -half.Y, -half.Z );
				boundsTempPositions[ 5 ] = center + new Vector3F( -half.X, -half.Y, half.Z );
				boundsTempPositions[ 6 ] = center + new Vector3F( -half.X, half.Y, -half.Z );
				boundsTempPositions[ 7 ] = center + new Vector3F( -half.X, half.Y, half.Z );

				//Matrix4 m = Matrix4.Identity;
				AddTriangles( boundsTempPositions, boundsIndices, ref transformIdentity, true, false, true );

				//Vector3 pos = bounds.GetCenter();
				//Vector3 scl = ( bounds.Maximum - pos ) * 2;
				//Matrix4 t = new Matrix4( Matrix3.FromScale( scl ), pos );

				//if( boxShapePositions == null )
				//	SimpleMeshGenerator.GenerateBox( new Vector3F( 1, 1, 1 ), out boxShapePositions, out boxShapeIndices );
				//AddTriangles( boxShapePositions, boxShapeIndices, t, false, true );
			}
			else
			{
				//!!!!при толстых линиях вроде как правильнее рисовать иначе, чтобы углы корректные были. где еще так?

				ref Vector3 bmin = ref bounds.Minimum;
				ref Vector3 bmax = ref bounds.Maximum;

				var lineThickness2 = lineThickness;

				////optimization
				//if( lineThickness2 == 0 )
				//{
				//	bounds.GetCenter( out var center );
				//	if( center.X - bmin.X < fastLineThicknessThreshold && center.Y - bmin.Y < fastLineThicknessThreshold && center.Z - bmin.Z < fastLineThicknessThreshold )
				//		lineThickness2 = GetThicknessByPixelSize( ref center, ProjectSettings.Get.LineThickness );
				//}

				AddLine( new Vector3( bmin.X, bmin.Y, bmin.Z ), new Vector3( bmax.X, bmin.Y, bmin.Z ), lineThickness2 );
				AddLine( new Vector3( bmax.X, bmin.Y, bmin.Z ), new Vector3( bmax.X, bmax.Y, bmin.Z ), lineThickness2 );
				AddLine( new Vector3( bmax.X, bmax.Y, bmin.Z ), new Vector3( bmin.X, bmax.Y, bmin.Z ), lineThickness2 );
				AddLine( new Vector3( bmin.X, bmax.Y, bmin.Z ), new Vector3( bmin.X, bmin.Y, bmin.Z ), lineThickness2 );

				AddLine( new Vector3( bmin.X, bmin.Y, bmax.Z ), new Vector3( bmax.X, bmin.Y, bmax.Z ), lineThickness2 );
				AddLine( new Vector3( bmax.X, bmin.Y, bmax.Z ), new Vector3( bmax.X, bmax.Y, bmax.Z ), lineThickness2 );
				AddLine( new Vector3( bmax.X, bmax.Y, bmax.Z ), new Vector3( bmin.X, bmax.Y, bmax.Z ), lineThickness2 );
				AddLine( new Vector3( bmin.X, bmax.Y, bmax.Z ), new Vector3( bmin.X, bmin.Y, bmax.Z ), lineThickness2 );

				AddLine( new Vector3( bmin.X, bmin.Y, bmin.Z ), new Vector3( bmin.X, bmin.Y, bmax.Z ), lineThickness2 );
				AddLine( new Vector3( bmax.X, bmin.Y, bmin.Z ), new Vector3( bmax.X, bmin.Y, bmax.Z ), lineThickness2 );
				AddLine( new Vector3( bmax.X, bmax.Y, bmin.Z ), new Vector3( bmax.X, bmax.Y, bmax.Z ), lineThickness2 );
				AddLine( new Vector3( bmin.X, bmax.Y, bmin.Z ), new Vector3( bmin.X, bmax.Y, bmax.Z ), lineThickness2 );
			}
		}

		public override void AddSphere( Matrix4 transform, double radius, int segments = 32, bool solid = false, double lineThickness = 0 )
		{
			if( solid )
			{
				SimpleMeshGenerator.GenerateSphere( (float)radius, segments, ( ( segments + 1 ) / 2 ) * 2, false, out Vector3F[] positions, out int[] indices );
				AddTriangles( positions, indices, transform, false, true );
			}
			else
			{
				Matrix4 t = transform;
				var r = radius;

				//var lineThickness2 = lineThickness;
				////optimization
				//if( lineThickness2 == 0 )
				//{
				//	scale

				//	if( radius < fastLineThicknessThreshold )
				//		lineThickness2 = GetThicknessByPixelSize( transform.GetTranslation(), ProjectSettings.Get.LineThickness );
				//}

				double angleStep = Math.PI / (double)segments;
				for( double angle = 0; angle < Math.PI * 2 - angleStep * .5; angle += angleStep )
				{
					double p1sin = Math.Sin( angle );
					double p1cos = Math.Cos( angle );
					double p2sin = Math.Sin( angle + angleStep );
					double p2cos = Math.Cos( angle + angleStep );

					AddLine( t * ( new Vector3( p1cos, p1sin, 0 ) * r ), t * ( new Vector3( p2cos, p2sin, 0 ) * r ), lineThickness );
					AddLine( t * ( new Vector3( 0, p1cos, p1sin ) * r ), t * ( new Vector3( 0, p2cos, p2sin ) * r ), lineThickness );
					AddLine( t * ( new Vector3( p1cos, 0, p1sin ) * r ), t * ( new Vector3( p2cos, 0, p2sin ) * r ), lineThickness );
				}
			}
		}

		public override void AddCone( Matrix4 transform, int axis, SimpleMeshGenerator.ConeOrigin origin, double radius, double height, int edgeSegments, int basicSegments, bool solid, double lineThickness )
		//public override void AddCone( Vec3 position, Quat rotation, double length, double radius, int edgeSegments, int basicSegments, bool solid, double lineThickness )
		{
			if( solid )
			{
				SimpleMeshGenerator.GenerateCone( axis, origin, radius, height, edgeSegments, true, true, out Vector3F[] positions, out int[] indices );
				AddTriangles( positions, indices, transform, false, true );
			}
			else
			{
				Matrix4 t = transform;
				if( axis != 0 )
					t *= SimpleMeshGenerator.GetRotationMatrix( axis ).ToMatrix4();

				double offset = 0;
				if( origin == SimpleMeshGenerator.ConeOrigin.Center )
					offset -= height / 2;

				//Edges
				{
					float step = MathEx.PI * 2 / (float)edgeSegments;
					for( float a = 0; a < MathEx.PI * 2 - step * .5f; a += step )
					{
						Vector3 pos = t * new Vector3( offset, MathEx.Cos( a ) * radius, MathEx.Sin( a ) * radius );
						AddLine( t * new Vector3( height + offset, 0, 0 ), pos, lineThickness );
					}
				}

				//Basic
				{
					float step = MathEx.PI * 2 / (float)basicSegments;
					Vector3 oldPos = Vector3.Zero;
					for( float a = 0; a < MathEx.PI * 2 + step * .5f; a += step )
					{
						Vector3 pos = t * new Vector3( offset, MathEx.Cos( a ) * radius, MathEx.Sin( a ) * radius );
						if( a != 0 )
							AddLine( oldPos, pos, lineThickness );
						oldPos = pos;
					}
				}

				//var r = radius;
				//var h = length;

				//Vec3 topPoint = t * new Vec3( h * .5, 0, 0 );
				////Vec3 topPoint = t * new Vec3( h, 0, 0 );
				////Vec3 topPoint = t * new Vec3( 0, 0, h * .5 );

				//double angleStep = Math.PI * 2 / (double)16;
				//for( double angle = 0; angle < Math.PI * 2 - angleStep * .5f; angle += angleStep )
				//{
				//	double psin = Math.Sin( angle );
				//	double pcos = Math.Cos( angle );

				//	AddLine(
				//		topPoint,
				//		t * new Vec3( -h * .5f, pcos * r, psin * r ), lineThickness );
				//}

				//for( double angle = 0; angle < Math.PI * 2 - angleStep * .5f; angle += angleStep )
				//{
				//	double p1sin = Math.Sin( angle );
				//	double p1cos = Math.Cos( angle );
				//	double p2sin = Math.Sin( angle + angleStep );
				//	double p2cos = Math.Cos( angle + angleStep );

				//	Mat4 trans = t * Mat4.FromTranslate( new Vec3( h * -.5f, 0, 0 ) );
				//	AddLine(
				//		trans * ( new Vec3( 0, p1cos, p1sin ) * r ),
				//		trans * ( new Vec3( 0, p2cos, p2sin ) * r ), lineThickness );
				//}

				////Edges
				//{
				//	float step = MathEx.PI * 2 / (float)edgeSegments;
				//	for( float a = 0; a < MathEx.PI * 2 - step * .5f; a += step )
				//	{
				//		Vec3 pos = position + rotation * new Vec3( 0, MathEx.Cos( a ) * radius, MathEx.Sin( a ) * radius );
				//		AddLine( position + rotation.GetForward() * length, pos, lineThickness );
				//	}
				//}

				////Basic
				//{
				//	float step = MathEx.PI * 2 / (float)basicSegments;
				//	Vec3 oldPos = Vec3.Zero;
				//	for( float a = 0; a < MathEx.PI * 2 + step * .5f; a += step )
				//	{
				//		Vec3 pos = position + rotation * new Vec3( 0, MathEx.Cos( a ) * radius, MathEx.Sin( a ) * radius );
				//		if( a != 0 )
				//			AddLine( oldPos, pos, lineThickness );
				//		oldPos = pos;
				//	}
				//}
			}
		}

		//public override void AddCone( Cone cone, int edgeSegments, int basicSegments, bool solid = false, double lineThickness = 0 )
		//{
		//	Quat rotation = Quat.FromDirectionZAxisUp( capsule.GetDirection() );
		//	Mat4 t = new Mat4( rotation.ToMat3(), capsule.GetCenter() );
		//	AddCone( t, 0, SimpleMeshGenerator.ConeOrigin.Bottom, cone. capsule.Radius, capsule.GetLength(), segments, solid, lineThickness );
		//}

		public override void AddCapsule( Matrix4 transform, int axis, double radius, double height, int segments, bool solid, double lineThickness )
		{
			if( solid )
			{
				SimpleMeshGenerator.GenerateCapsule( axis, (float)radius, (float)height, segments, ( segments / 2 ) * 2 + 1, out Vector3F[] positions, out int[] indices );
				AddTriangles( positions, indices, transform, false, true );
			}
			else
			{
				Matrix4 t = transform;
				if( axis != 0 )
					t *= SimpleMeshGenerator.GetRotationMatrix( axis ).ToMatrix4();
				var r = radius;
				var h = height;

				double angleStep = Math.PI * 2 / (double)segments;

				for( double angle = 0; angle < Math.PI * 2 - angleStep * .5f; angle += angleStep )
				{
					double psin = Math.Sin( angle );
					double pcos = Math.Cos( angle );

					AddLine(
						t * new Vector3( h * .5, pcos * radius, psin * radius ),
						t * new Vector3( -h * .5, pcos * radius, psin * radius ), lineThickness );
					//viewport.DebugGeometry.AddLine(
					//	t * new Vec3( pcos * radius, psin * radius, h * .5 ),
					//	t * new Vec3( pcos * radius, psin * radius, -h * .5 ) );
				}

				for( int n = 0; n < 2; n++ )
				{
					for( double angle = 0; angle < Math.PI * 2 - angleStep * .5f; angle += angleStep )
					{
						double a = angle + ( ( n != 0 ) ? Math.PI : 0 );

						double p1sin = Math.Sin( a );
						double p1cos = Math.Cos( a );
						double p2sin = Math.Sin( a + angleStep );
						double p2cos = Math.Cos( a + angleStep );

						Matrix4 trans = t * Matrix4.FromTranslate( new Vector3( h * .5 * ( ( n != 0 ) ? -1.0f : 1.0f ), 0, 0 ) );
						//Mat4 trans = t * Mat4.FromTranslate( new Vec3( 0, 0, h * .5 * ( ( n != 0 ) ? -1.0f : 1.0f ) ) );

						AddLine(
							trans * ( new Vector3( 0, p1cos, p1sin ) * radius ),
							trans * ( new Vector3( 0, p2cos, p2sin ) * radius ), lineThickness );
						//viewport.DebugGeometry.AddLine(
						//	trans * ( new Vec3( p1cos, p1sin, 0 ) * radius ),
						//	trans * ( new Vec3( p2cos, p2sin, 0 ) * radius ) );

						if( angle < Math.PI - angleStep * .5f )
						{
							AddLine(
								trans * ( new Vector3( p1sin, 0, p1cos ) * radius ),
								trans * ( new Vector3( p2sin, 0, p2cos ) * radius ), lineThickness );
							AddLine(
								trans * ( new Vector3( p1sin, p1cos, 0 ) * radius ),
								trans * ( new Vector3( p2sin, p2cos, 0 ) * radius ), lineThickness );
							//viewport.DebugGeometry.AddLine(
							//	trans * ( new Vec3( 0, p1cos, p1sin ) * radius ),
							//	trans * ( new Vec3( 0, p2cos, p2sin ) * radius ) );
							//viewport.DebugGeometry.AddLine(
							//	trans * ( new Vec3( p1cos, 0, p1sin ) * radius ),
							//	trans * ( new Vec3( p2cos, 0, p2sin ) * radius ) );
						}
					}
				}


				//Vec3 position = capsule.GetCenter();
				//QuatF rotation = QuatF.FromDirectionZAxisUp( capsule.GetDirection().ToVec3F() );
				//float radius = (float)capsule.Radius;
				//float length = (float)capsule.GetLength();

				//Mat4 t = new Mat4( rotation.ToMat3(), position );

				//float angleStep = MathEx.PI * 2 / (float)segments;

				//for( float angle = 0; angle < MathEx.PI * 2 - angleStep * .5f; angle += angleStep )
				//{
				//	float psin = MathEx.Sin( angle );
				//	float pcos = MathEx.Cos( angle );

				//	AddLine(
				//		t * new Vec3( length * .5f, pcos * radius, psin * radius ),
				//		t * new Vec3( -length * .5f, pcos * radius, psin * radius ), lineThickness );
				//}

				//for( int n = 0; n < 2; n++ )
				//{
				//	for( float angle = 0; angle < MathEx.PI * 2 - angleStep * .5f; angle += angleStep )
				//	{
				//		float a = angle + ( ( n != 0 ) ? MathEx.PI : 0 );

				//		float p1sin = MathEx.Sin( a );
				//		float p1cos = MathEx.Cos( a );
				//		float p2sin = MathEx.Sin( a + angleStep );
				//		float p2cos = MathEx.Cos( a + angleStep );

				//		Mat4 trans = t * Mat4.FromTranslate(
				//			new Vec3( length * .5f * ( ( n != 0 ) ? -1.0f : 1.0f ), 0, 0 ) );

				//		AddLine(
				//			trans * ( new Vec3( 0, p1cos, p1sin ) * radius ),
				//			trans * ( new Vec3( 0, p2cos, p2sin ) * radius ), lineThickness );
				//		if( angle < MathEx.PI - angleStep * .5f )
				//		{
				//			AddLine(
				//				trans * ( new Vec3( p1sin, 0, p1cos ) * radius ),
				//				trans * ( new Vec3( p2sin, 0, p2cos ) * radius ), lineThickness );
				//			AddLine(
				//				trans * ( new Vec3( p1sin, p1cos, 0 ) * radius ),
				//				trans * ( new Vec3( p2sin, p2cos, 0 ) * radius ), lineThickness );
				//		}
				//	}
				//}
			}
		}

		public override void AddCapsule( Capsule capsule, int segments, bool solid = false, double lineThickness = 0 )
		{
			Quaternion rotation = Quaternion.FromDirectionZAxisUp( capsule.GetDirection() );
			Matrix4 t = new Matrix4( rotation.ToMatrix3(), capsule.GetCenter() );
			AddCapsule( t, 0, capsule.Radius, capsule.GetLength(), segments, solid, lineThickness );

			//if( solid )
			//{
			//	Mat4 t = new Mat4( Quat.FromDirectionZAxisUp( capsule.Point2 - capsule.Point1 ).ToMat3(), capsule.GetCenter() );
			//	SimpleMeshGenerator.GenerateCapsule( 0, (float)capsule.radius, (float)capsule.GetLength(), segments, ( segments / 2 ) * 2 + 1, out Vec3F[] positions, out int[] indices );
			//	AddVertexIndexBuffer( positions, indices, t, false, true );
			//}
			//else
			//{
			//	Vec3 position = capsule.GetCenter();
			//	QuatF rotation = QuatF.FromDirectionZAxisUp( capsule.GetDirection().ToVec3F() );
			//	float radius = (float)capsule.Radius;
			//	float length = (float)capsule.GetLength();

			//	Mat4 t = new Mat4( rotation.ToMat3(), position );

			//	float angleStep = MathEx.PI * 2 / (float)segments;

			//	for( float angle = 0; angle < MathEx.PI * 2 - angleStep * .5f; angle += angleStep )
			//	{
			//		float psin = MathEx.Sin( angle );
			//		float pcos = MathEx.Cos( angle );

			//		AddLine(
			//			t * new Vec3( length * .5f, pcos * radius, psin * radius ),
			//			t * new Vec3( -length * .5f, pcos * radius, psin * radius ), lineThickness );
			//	}

			//	for( int n = 0; n < 2; n++ )
			//	{
			//		for( float angle = 0; angle < MathEx.PI * 2 - angleStep * .5f; angle += angleStep )
			//		{
			//			float a = angle + ( ( n != 0 ) ? MathEx.PI : 0 );

			//			float p1sin = MathEx.Sin( a );
			//			float p1cos = MathEx.Cos( a );
			//			float p2sin = MathEx.Sin( a + angleStep );
			//			float p2cos = MathEx.Cos( a + angleStep );

			//			Mat4 trans = t * Mat4.FromTranslate(
			//				new Vec3( length * .5f * ( ( n != 0 ) ? -1.0f : 1.0f ), 0, 0 ) );

			//			AddLine(
			//				trans * ( new Vec3( 0, p1cos, p1sin ) * radius ),
			//				trans * ( new Vec3( 0, p2cos, p2sin ) * radius ), lineThickness );
			//			if( angle < MathEx.PI - angleStep * .5f )
			//			{
			//				AddLine(
			//					trans * ( new Vec3( p1sin, 0, p1cos ) * radius ),
			//					trans * ( new Vec3( p2sin, 0, p2cos ) * radius ), lineThickness );
			//				AddLine(
			//					trans * ( new Vec3( p1sin, p1cos, 0 ) * radius ),
			//					trans * ( new Vec3( p2sin, p2cos, 0 ) * radius ), lineThickness );
			//			}
			//		}
			//	}
			//}
		}

		public override void AddCylinder( Matrix4 transform, int axis, double radius, double height, int segments = 32, bool solid = false, double lineThickness = 0 )
		{
			if( solid )
			{
				SimpleMeshGenerator.GenerateCylinder( axis, (float)radius, (float)height, segments, true, true, true, out Vector3F[] positions, out int[] indices );
				AddTriangles( positions, indices, transform, false, true );
			}
			else
			{
				Matrix4 t = transform;
				if( axis != 0 )
					t *= SimpleMeshGenerator.GetRotationMatrix( axis ).ToMatrix4();
				var r = radius;
				var h = height;

				double angleStep = Math.PI * 2 / 16;
				for( double angle = 0; angle < Math.PI * 2 - angleStep * .5f; angle += angleStep )
				{
					double psin = Math.Sin( angle );
					double pcos = Math.Cos( angle );

					AddLine(
						t * new Vector3( h * .5f, pcos * r, psin * r ),
						t * new Vector3( -h * .5f, pcos * r, psin * r ), lineThickness );
					//viewport.DebugGeometry.AddLine(
					//	t * new Vec3( pcos * Radius, psin * Radius, h * .5f ),
					//	t * new Vec3( pcos * Radius, psin * Radius, -h * .5f ) );
				}

				for( int n = 0; n < 2; n++ )
				{
					for( double angle = 0; angle < Math.PI * 2 - angleStep * .5f; angle += angleStep )
					{
						double a = angle + ( ( n != 0 ) ? Math.PI : 0 );

						double p1sin = Math.Sin( a );
						double p1cos = Math.Cos( a );
						double p2sin = Math.Sin( a + angleStep );
						double p2cos = Math.Cos( a + angleStep );

						Matrix4 trans = t * Matrix4.FromTranslate( new Vector3( h * .5f * ( ( n != 0 ) ? -1.0f : 1.0f ), 0, 0 ) );
						//Mat4 trans = t * Mat4.FromTranslate( new Vec3( 0, 0, h * .5f * ( ( n != 0 ) ? -1.0f : 1.0f ) ) );

						AddLine(
							trans * ( new Vector3( 0, p1cos, p1sin ) * r ),
							trans * ( new Vector3( 0, p2cos, p2sin ) * r ), lineThickness );
						//viewport.DebugGeometry.AddLine( 
						//	trans * ( new Vec3( p1cos, p1sin, 0 ) * Radius ),
						//	trans * ( new Vec3( p2cos, p2sin, 0 ) * Radius ) );
					}
				}
			}
		}

		public override void AddCylinder( Cylinder cylinder, int segments = 32, bool solid = false, double lineThickness = 0 )
		{
			Quaternion rotation = Quaternion.FromDirectionZAxisUp( cylinder.GetDirection() );
			Matrix4 t = new Matrix4( rotation.ToMatrix3(), cylinder.GetCenter() );
			AddCylinder( t, 0, cylinder.Radius, cylinder.GetLength(), segments, solid, lineThickness );
		}

		public unsafe override void AddBox( Box box, bool solid, double lineThickness )
		{
			if( solid )
			{
				Matrix4 t = new Matrix4( box.Axis * Matrix3.FromScale( box.Extents * 2 ), box.Center );

				if( boxShapePositions == null )
					SimpleMeshGenerator.GenerateBox( new Vector3F( 1, 1, 1 ), out boxShapePositions, out boxShapeIndices );
				AddTriangles( boxShapePositions, boxShapeIndices, t, false, true );
			}
			else
			{
				Vector3* p = stackalloc Vector3[ 8 ];
				box.ToPoints( p );
				//var p = new Vector3[ 8 ];
				//box.ToPoints( ref p );

				var lineThickness2 = lineThickness;
				////optimization
				//if( lineThickness2 == 0 )
				//{
				//	if( box.Extents.X < fastLineThicknessThreshold && box.Extents.Y < fastLineThicknessThreshold && box.Extents.Z < fastLineThicknessThreshold )
				//		lineThickness2 = GetThicknessByPixelSize( ref box.Center, ProjectSettings.Get.LineThickness );
				//}

				AddLine( p[ 0 ], p[ 1 ], lineThickness2 );
				AddLine( p[ 1 ], p[ 2 ], lineThickness2 );
				AddLine( p[ 2 ], p[ 3 ], lineThickness2 );
				AddLine( p[ 3 ], p[ 0 ], lineThickness2 );

				AddLine( p[ 4 ], p[ 5 ], lineThickness2 );
				AddLine( p[ 5 ], p[ 6 ], lineThickness2 );
				AddLine( p[ 6 ], p[ 7 ], lineThickness2 );
				AddLine( p[ 7 ], p[ 4 ], lineThickness2 );

				AddLine( p[ 0 ], p[ 4 ], lineThickness2 );
				AddLine( p[ 1 ], p[ 5 ], lineThickness2 );
				AddLine( p[ 2 ], p[ 6 ], lineThickness2 );
				AddLine( p[ 3 ], p[ 7 ], lineThickness2 );
			}
		}

		public override void AddArrow( Vector3 from, Vector3 to, double headHeight = 0, double headRadius = 0, bool solid = false, double lineThickness = 0 )
		{
			Vector3 dir = ( to - from ).GetNormalize();
			double length = ( to - from ).Length();

			if( headHeight == 0 )
				headHeight = length / 8;
			if( headRadius == 0 )
				headRadius = headHeight / 4;

			Vector3 headFrom = from + dir * ( length - headHeight );

			AddLine( from, headFrom, lineThickness );

			int segments = solid ? 16 : 8;

			Matrix4 t = new Matrix4( Quaternion.FromDirectionZAxisUp( dir ).ToMatrix3(), headFrom );
			AddCone( t, 0, SimpleMeshGenerator.ConeOrigin.Bottom, headRadius, headHeight, segments, segments, solid, lineThickness );
		}

		//public override void AddArrow( Vec3 from, Vec3 to, double arrowSize, bool noLine )
		//{
		//	if( !noLine )
		//		AddLine( from, to );

		//	Vec3 diff = to - from;
		//	float len = (float)diff.Length();

		//	diff.Normalize();

		//	if( len < arrowSize * 3 )
		//		arrowSize = len * ( 1.0f / 3.0f );

		//	QuatF rot;

		//	if( Math.Abs( diff.X ) >= .001f || Math.Abs( diff.Y ) >= .001f )
		//	{
		//		float dirh = MathEx.ATan( (float)diff.Y, (float)diff.X );
		//		float dirv = -MathEx.ATan( (float)diff.Z, (float)diff.ToVec2().Length() );
		//		float halfdirh = dirh * .5f;
		//		rot = new QuatF( new Vec3F( 0, 0, MathEx.Sin( halfdirh ) ), MathEx.Cos( halfdirh ) );
		//		float halfdirv = dirv * .5f;
		//		rot *= new QuatF( 0, MathEx.Sin( halfdirv ), 0, MathEx.Cos( halfdirv ) );
		//	}
		//	else
		//	{
		//		float halfDir = MathEx.PI * .25f * ( diff.Z > 0 ? -1 : 1 );
		//		rot = new QuatF( 0, MathEx.Sin( halfDir ), 0, MathEx.Cos( halfDir ) );
		//	}

		//	double radius = arrowSize / 6.0f;

		//	float step = MathEx.PI / 3;
		//	//!!!!new
		//	for( float a = 0; a < Math.PI * 2 - step * .5; a += step )
		//	//for( float a = 0; a < Math.PI * 2; a += step )
		//	{
		//		Vec3 start = new Vec3( -arrowSize, MathEx.Cos( a ) * radius, MathEx.Sin( a ) * radius );
		//		Vec3 end = new Vec3( -arrowSize, MathEx.Cos( a + step ) * radius, MathEx.Sin( a + step ) * radius );
		//		AddLine( to + ( start * rot.ToQuat() ), ( to + end * rot.ToQuat() ) );
		//		AddLine( to + ( start * rot.ToQuat() ), to );
		//	}
		//}

		unsafe void SetColors( Vertex* pVertex )
		{
			pVertex->color = currentColorPacked;
			pVertex->colorInvisibleBehindObjects = currentColorInvisibleBehindObjectsPacked;
		}

		public override void AddLine( Vector3 start, Vector3 end, double thickness )
		{
			if( thickness >= 0 )
			{
				Vector3 direction = end - start;
				double length;
				if( direction != Vector3.Zero )
					length = direction.Normalize();
				else
				{
					direction = Vector3.XAxis;
					length = 0;
				}

				double thicknessStart;
				double thicknessEnd;
				if( thickness == 0 )
				{
					//double halfLength = length * 0.5;

					//if( halfLength < fastLineThicknessThreshold )
					//{
					//	var center = start + direction * halfLength;
					//	thicknessStart = GetThicknessByPixelSize( ref center, ProjectSettings.Get.LineThickness );
					//	thicknessEnd = thicknessStart;
					//}
					//else
					//{
					thicknessStart = GetThicknessByPixelSize( ref start, ProjectSettings.Get.LineThickness );
					thicknessEnd = GetThicknessByPixelSize( ref end, ProjectSettings.Get.LineThickness );
					//}
				}
				else
				{
					thicknessStart = thickness;
					thicknessEnd = thickness;
				}

				//!!!!double
				var directionF = direction.ToVector3F();
				QuaternionF.FromDirectionZAxisUp( ref directionF, out var rotation );
				rotation.ToMatrix3( out var rotationMatrix );

				var halfThicknessStart = (float)thicknessStart * 0.5f;
				var halfThicknessEnd = (float)thicknessEnd * 0.5f;
				var start2 = start.ToVector3F();
				var end2 = end.ToVector3F();

				Vector3F[] positions = addLineTempPositions;
				positions[ 0 ] = end2 + rotationMatrix * new Vector3F( halfThicknessEnd, -halfThicknessEnd, -halfThicknessEnd );
				positions[ 1 ] = end2 + rotationMatrix * new Vector3F( halfThicknessEnd, -halfThicknessEnd, halfThicknessEnd );
				positions[ 2 ] = end2 + rotationMatrix * new Vector3F( halfThicknessEnd, halfThicknessEnd, -halfThicknessEnd );
				positions[ 3 ] = end2 + rotationMatrix * new Vector3F( halfThicknessEnd, halfThicknessEnd, halfThicknessEnd );
				positions[ 4 ] = start2 + rotationMatrix * new Vector3F( -halfThicknessStart, -halfThicknessStart, -halfThicknessStart );
				positions[ 5 ] = start2 + rotationMatrix * new Vector3F( -halfThicknessStart, -halfThicknessStart, halfThicknessStart );
				positions[ 6 ] = start2 + rotationMatrix * new Vector3F( -halfThicknessStart, halfThicknessStart, -halfThicknessStart );
				positions[ 7 ] = start2 + rotationMatrix * new Vector3F( -halfThicknessStart, halfThicknessStart, halfThicknessStart );
				//positions[ 0 ] = new Vector3( half.X, -half.Y, -half.Z );
				//positions[ 1 ] = new Vector3( half.X, -half.Y, half.Z );
				//positions[ 2 ] = new Vector3( half.X, half.Y, -half.Z );
				//positions[ 3 ] = new Vector3( half.X, half.Y, half.Z );
				//positions[ 4 ] = new Vector3( -half.X, -half.Y, -half.Z );
				//positions[ 5 ] = new Vector3( -half.X, -half.Y, half.Z );
				//positions[ 6 ] = new Vector3( -half.X, half.Y, -half.Z );
				//positions[ 7 ] = new Vector3( -half.X, half.Y, half.Z );

				if( boxShapePositions == null )
					SimpleMeshGenerator.GenerateBox( new Vector3F( 1, 1, 1 ), out boxShapePositions, out boxShapeIndices );

				AddVertexIndexBuffer_Array( positions, boxShapeIndices, ref transformIdentity, true, false, true );
				//AddTriangles( positions, boxShapeIndices, false, true );






				//double length = ( end - start ).Length();
				//double thickness2 = thickness;
				//if( thickness2 == 0 )
				//	thickness2 = GetThicknessByPixelSize( ( start + end ) * 0.5, ProjectSettings.Get.LineThickness );

				////if( drawShadows )
				////{
				////	//!!!!в конфиге настраивать?
				////	length += thickness2;
				////	thickness2 *= 2.7f;
				////	//thickness2 *= 2.5f;// 3;
				////}

				//Vector3 direction = end - start;
				//if( direction == Vector3.Zero )
				//	direction = Vector3.XAxis;
				//direction.Normalize();
				////!!!!?
				//Quaternion rotation = Quaternion.FromDirectionZAxisUp( direction );

				//Matrix4 t = new Matrix4( rotation.ToMatrix3() * Matrix3.FromScale( new Vector3( length, thickness2, thickness2 ) ), ( start + end ) * .5f );

				//if( boxShapePositions == null )
				//	SimpleMeshGenerator.GenerateBox( new Vector3F( 1, 1, 1 ), out boxShapePositions, out boxShapeIndices );

				//Vector3F[] positions = new Vector3F[ boxShapePositions.Length ];
				//for( int n = 0; n < positions.Length; n++ )
				//	positions[ n ] = ( t * boxShapePositions[ n ] ).ToVector3F();

				//AddTriangles( positions, boxShapeIndices, false, true );

				////AddTriangles( boxShapePositions, boxShapeIndices, t, false, true );
			}
			else
			{
				Item_DynamicallyCreated item = null;

				//try use last item
				if( items.Count != 0 )
				{
					Item_DynamicallyCreated lastItem = items[ items.Count - 1 ] as Item_DynamicallyCreated;

					if( lastItem != null &&
						lastItem.type == Item_DynamicallyCreated.ItemType.Lines &&
						lastItem.depthTest == currentDepthTest &&
						lastItem.depthWrite == currentDepthWrite &&
						Equal( lastItem.occlusionQuery, currentOcclusionQuery ) &&
						//lastItem.nonOverlappingGroup == nonOverlappingGroup &&
						lastItem.vertexCount + 2 <= constBufferVertexCount &&
						lastItem.indexCount + 2 <= constBufferIndexCount )
					{
						item = lastItem;
					}
				}

				if( item == null )
				{
					//use new item

					if( freeItemsDynamicallyCreated.Count == 0 )
						freeItemsDynamicallyCreated.Push( new Item_DynamicallyCreated() );
					item = freeItemsDynamicallyCreated.Pop();

					item.type = Item_DynamicallyCreated.ItemType.Lines;
					item.depthTest = currentDepthTest;
					item.depthWrite = currentDepthWrite;
					item.occlusionQuery = currentOcclusionQuery;
					//item.nonOverlappingGroup = nonOverlappingGroup;
					item.transform = Matrix4.Identity;
					item.transformIsIdentity = true;
					item.culling = false;
					item.vertexCount = 0;
					item.indexCount = 0;
					item.actualIndexData = false;

					items.Add( item );
				}

				unsafe
				{
					fixed( byte* pVertices2 = item.vertices )
					{
						Vertex* pVertices = (Vertex*)pVertices2;
						Vertex* pVertex = pVertices + item.vertexCount;

						//!!!!!как-то неправильно. тогда матрицу выставлять?
						pVertex->position = start.ToVector3F();
						SetColors( pVertex );
						pVertex++;

						//!!!!!как-то неправильно. тогда матрицу выставлять?
						pVertex->position = end.ToVector3F();
						SetColors( pVertex );
					}
				}

				item.indices[ item.indexCount++ ] = item.vertexCount++;
				item.indices[ item.indexCount++ ] = item.vertexCount++;
				//item.vertexCount += 2;
			}
		}

		unsafe void AddVertexIndexBuffer_Array( Vector3F[] vertices, int[] indices, ref Matrix4 transform, bool transformIsIdentity, bool wireframe, bool culling )
		{
			int triangleCount;
			if( indices != null )
				triangleCount = indices.Length / 3;
			else
				triangleCount = vertices.Length / 3;

			Item_DynamicallyCreated item = null;

			//try use last item
			if( items.Count != 0 )
			{
				var lastItem = items[ items.Count - 1 ] as Item_DynamicallyCreated;
				if( lastItem != null && lastItem.vertexCount + 3 <= constBufferVertexCount && lastItem.indexCount + 3 <= constBufferIndexCount )
				{
					if( lastItem.type == Item_DynamicallyCreated.ItemType.Triangles &&
						lastItem.depthTest == currentDepthTest &&
						lastItem.depthWrite == currentDepthWrite &&
						Equal( lastItem.occlusionQuery, currentOcclusionQuery ) &&
						//lastItem.nonOverlappingGroup == nonOverlappingGroup &&
						( ( lastItem.transformIsIdentity && transformIsIdentity ) || lastItem.transform == transform ) &&
						lastItem.wireframe == wireframe &&
						lastItem.culling == culling )
					{
						item = lastItem;
					}
				}
			}

			//!!!!глючит на HD7850
			////try push indexes data
			//if( indices != null )
			//{
			//	if( ( item != null && item.vertexCount + vertices.Length <= constBufferVertexCount && item.indexCount + indices.Length <= constBufferIndexCount ) || ( item == null && vertices.Length <= constBufferVertexCount && indices.Length <= constBufferIndexCount ) )
			//	{
			//		if( item == null )
			//		{
			//			//get new item

			//			if( freeItemsDynamicallyCreated.Count == 0 )
			//				freeItemsDynamicallyCreated.Push( new Item_DynamicallyCreated() );
			//			item = freeItemsDynamicallyCreated.Pop();

			//			item.type = Item_DynamicallyCreated.ItemType.Triangles;
			//			item.depthTest = currentDepthTest;
			//			item.depthWrite = currentDepthWrite;
			//			item.occlusionQuery = currentOcclusionQuery;
			//			item.vertexCount = 0;
			//			item.indexCount = 0;
			//			item.actualIndexData = false;
			//			item.transform = transform;
			//			item.transformIsIdentity = transformIsIdentity;
			//			item.wireframe = wireframe;
			//			item.culling = culling;

			//			items.Add( item );
			//		}

			//		int oldVertexCount = item.vertexCount;

			//		fixed ( byte* pVertices2 = item.vertices )
			//		{
			//			Vertex* pVertices = (Vertex*)pVertices2;
			//			Vertex* pVertex = pVertices + item.vertexCount;

			//			for( int n = 0; n < vertices.Length; n++ )
			//			{
			//				pVertex->position = vertices[ n ];
			//				SetColors( pVertex );
			//				pVertex++;
			//			}
			//		}

			//		item.vertexCount += vertices.Length;
			//		for( int n = 0; n < indices.Length; n++ )
			//			item.indices[ item.indexCount++ ] = indices[ n ] + oldVertexCount;

			//		item.actualIndexData = true;

			//		return;
			//	}
			//}

			for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
			{
				if( item == null || item.vertexCount + 3 > constBufferVertexCount || item.indexCount + 3 > constBufferIndexCount )
				{
					//get new item

					if( freeItemsDynamicallyCreated.Count == 0 )
						freeItemsDynamicallyCreated.Push( new Item_DynamicallyCreated() );
					item = freeItemsDynamicallyCreated.Pop();

					item.type = Item_DynamicallyCreated.ItemType.Triangles;
					item.depthTest = currentDepthTest;
					item.depthWrite = currentDepthWrite;
					item.occlusionQuery = currentOcclusionQuery;
					//item.nonOverlappingGroup = nonOverlappingGroup;
					item.vertexCount = 0;
					item.indexCount = 0;
					item.actualIndexData = false;
					item.transform = transform;
					item.transformIsIdentity = transformIsIdentity;
					item.wireframe = wireframe;
					item.culling = culling;

					items.Add( item );
				}

				if( indices != null )
				{
					int index0 = indices[ nTriangle * 3 + 0 ];
					int index1 = indices[ nTriangle * 3 + 1 ];
					int index2 = indices[ nTriangle * 3 + 2 ];

					fixed( byte* pVertices2 = item.vertices )
					{
						Vertex* pVertices = (Vertex*)pVertices2;
						Vertex* pVertex = pVertices + item.vertexCount;

						pVertex->position = vertices[ index0 ];
						SetColors( pVertex );
						pVertex++;

						pVertex->position = vertices[ index1 ];
						SetColors( pVertex );
						pVertex++;

						pVertex->position = vertices[ index2 ];
						SetColors( pVertex );
					}
				}
				else
				{
					fixed( byte* pVertices2 = item.vertices )
					{
						Vertex* pVertices = (Vertex*)pVertices2;
						Vertex* pVertex = pVertices + item.vertexCount;

						pVertex->position = vertices[ nTriangle * 3 + 0 ];
						SetColors( pVertex );
						pVertex++;

						pVertex->position = vertices[ nTriangle * 3 + 1 ];
						SetColors( pVertex );
						pVertex++;

						pVertex->position = vertices[ nTriangle * 3 + 2 ];
						SetColors( pVertex );
					}
				}

				item.indices[ item.indexCount++ ] = item.vertexCount++;
				item.indices[ item.indexCount++ ] = item.vertexCount++;
				item.indices[ item.indexCount++ ] = item.vertexCount++;
				//item.vertexCount += 3;
			}
		}

		unsafe void AddVertexIndexBuffer_IList( IList<Vector3F> vertices, IList<int> indices, ref Matrix4 transform, bool transformIsIdentity, bool wireframe, bool culling )
		{
			int triangleCount;
			if( indices != null )
				triangleCount = indices.Count / 3;
			else
				triangleCount = vertices.Count / 3;

			Item_DynamicallyCreated item = null;

			//try use last item
			if( items.Count != 0 )
			{
				var lastItem = items[ items.Count - 1 ] as Item_DynamicallyCreated;
				if( lastItem != null && lastItem.vertexCount + 3 <= constBufferVertexCount && lastItem.indexCount + 3 <= constBufferIndexCount )
				{
					if( lastItem.type == Item_DynamicallyCreated.ItemType.Triangles &&
						lastItem.depthTest == currentDepthTest &&
						lastItem.depthWrite == currentDepthWrite &&
						Equal( lastItem.occlusionQuery, currentOcclusionQuery ) &&
						//lastItem.nonOverlappingGroup == nonOverlappingGroup &&
						( ( lastItem.transformIsIdentity && transformIsIdentity ) || lastItem.transform == transform ) &&
						lastItem.wireframe == wireframe &&
						lastItem.culling == culling )
					{
						item = lastItem;
					}
				}
			}

			//!!!!глючит на HD7850
			////try push indexes data
			//if( indices != null )
			//{
			//	if( ( item != null && item.vertexCount + vertices.Count <= constBufferVertexCount && item.indexCount + indices.Count <= constBufferIndexCount ) || ( item == null && vertices.Count <= constBufferVertexCount && indices.Count <= constBufferIndexCount ) )
			//	{
			//		if( item == null )
			//		{
			//			//get new item

			//			if( freeItemsDynamicallyCreated.Count == 0 )
			//				freeItemsDynamicallyCreated.Push( new Item_DynamicallyCreated() );
			//			item = freeItemsDynamicallyCreated.Pop();

			//			item.type = Item_DynamicallyCreated.ItemType.Triangles;
			//			item.depthTest = currentDepthTest;
			//			item.depthWrite = currentDepthWrite;
			//			item.occlusionQuery = currentOcclusionQuery;
			//			item.vertexCount = 0;
			//			item.indexCount = 0;
			//			item.actualIndexData = false;
			//			item.transform = transform;
			//			item.transformIsIdentity = transformIsIdentity;
			//			item.wireframe = wireframe;
			//			item.culling = culling;

			//			items.Add( item );
			//		}

			//		int oldVertexCount = item.vertexCount;

			//		fixed ( byte* pVertices2 = item.vertices )
			//		{
			//			Vertex* pVertices = (Vertex*)pVertices2;
			//			Vertex* pVertex = pVertices + item.vertexCount;

			//			for( int n = 0; n < vertices.Count; n++ )
			//			{
			//				pVertex->position = vertices[ n ];
			//				SetColors( pVertex );
			//				pVertex++;
			//			}
			//		}

			//		item.vertexCount += vertices.Count;
			//		for( int n = 0; n < indices.Count; n++ )
			//			item.indices[ item.indexCount++ ] = indices[ n ] + oldVertexCount;

			//		item.actualIndexData = true;

			//		return;
			//	}
			//}

			for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
			{
				if( item == null || item.vertexCount + 3 > constBufferVertexCount || item.indexCount + 3 > constBufferIndexCount )
				{
					//get new item

					if( freeItemsDynamicallyCreated.Count == 0 )
						freeItemsDynamicallyCreated.Push( new Item_DynamicallyCreated() );
					item = freeItemsDynamicallyCreated.Pop();

					item.type = Item_DynamicallyCreated.ItemType.Triangles;
					item.depthTest = currentDepthTest;
					item.depthWrite = currentDepthWrite;
					item.occlusionQuery = currentOcclusionQuery;
					//item.nonOverlappingGroup = nonOverlappingGroup;
					item.vertexCount = 0;
					item.indexCount = 0;
					item.actualIndexData = false;
					item.transform = transform;
					item.transformIsIdentity = transformIsIdentity;
					item.wireframe = wireframe;
					item.culling = culling;

					items.Add( item );
				}

				if( indices != null )
				{
					int index0 = indices[ nTriangle * 3 + 0 ];
					int index1 = indices[ nTriangle * 3 + 1 ];
					int index2 = indices[ nTriangle * 3 + 2 ];

					fixed( byte* pVertices2 = item.vertices )
					{
						Vertex* pVertices = (Vertex*)pVertices2;
						Vertex* pVertex = pVertices + item.vertexCount;

						pVertex->position = vertices[ index0 ];
						SetColors( pVertex );
						pVertex++;

						pVertex->position = vertices[ index1 ];
						SetColors( pVertex );
						pVertex++;

						pVertex->position = vertices[ index2 ];
						SetColors( pVertex );
					}
				}
				else
				{
					fixed( byte* pVertices2 = item.vertices )
					{
						Vertex* pVertices = (Vertex*)pVertices2;
						Vertex* pVertex = pVertices + item.vertexCount;

						pVertex->position = vertices[ nTriangle * 3 + 0 ];
						SetColors( pVertex );
						pVertex++;

						pVertex->position = vertices[ nTriangle * 3 + 1 ];
						SetColors( pVertex );
						pVertex++;

						pVertex->position = vertices[ nTriangle * 3 + 2 ];
						SetColors( pVertex );
					}
				}

				item.indices[ item.indexCount++ ] = item.vertexCount++;
				item.indices[ item.indexCount++ ] = item.vertexCount++;
				item.indices[ item.indexCount++ ] = item.vertexCount++;
				//item.vertexCount += 3;
			}
		}

		void AddTriangles( IList<Vector3F> vertices, IList<int> indices, ref Matrix4 transform, bool transformIsIdentity, bool wireframe, bool culling )
		{
			if( indices != null )
			{
				if( indices.Count % 3 != 0 )
					Log.Fatal( "Simple3DRenderer: AddTriangles: Invalid index count." );
			}
			else
			{
				if( vertices.Count % 3 != 0 )
					Log.Fatal( "Simple3DRenderer: AddTriangles: Invalid vertex count." );
			}

			if( wireframe )
			{
				//convert to lines. no wireframe triangles in bgfx.

				//!!!!slowly

				Vector3[] verticesTransformed = new Vector3[ vertices.Count ];
				{
					bool identity = transformIsIdentity;// || transform == Matrix4.Identity;
					for( int n = 0; n < verticesTransformed.Length; n++ )
					{
						Vector3 v = vertices[ n ].ToVector3();
						if( !identity )
							Matrix4.Multiply( ref transform, ref v, out verticesTransformed[ n ] );//v = transform * v;
						else
							verticesTransformed[ n ] = v;
					}
				}

				if( indices != null )
				{
					if( indices.Count != 0 )
					{
						int[] indicesArray;
						indicesArray = indices as int[];
						if( indicesArray == null )
							indicesArray = indices.ToArray();

						unsafe
						{
							int destInBytes = indicesArray.Length * 2 * sizeof( uint );
							int* dest = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, destInBytes );

							int destCount;
							fixed( int* pIndices = indicesArray )
							{
								destCount = (int)Bgfx.TopologyConvert( TopologyConvert.TriListToLineList, (IntPtr)dest, (uint)destInBytes, (IntPtr)pIndices, (uint)indices.Count, true );
							}

							for( int nLine = 0; nLine < destCount / 2; nLine++ )
							{
								var v0 = verticesTransformed[ dest[ nLine * 2 + 0 ] ];
								var v1 = verticesTransformed[ dest[ nLine * 2 + 1 ] ];

								AddLineThin( v0, v1 );
							}

							NativeUtility.Free( dest );
						}

						////!!!!allocate on stack for small arrays? везде такое
						//int[] dest = new int[ indicesArray.Length * 2 ];
						//int destCount;
						//unsafe
						//{
						//	fixed ( int* pDest = dest, pIndices = indicesArray )
						//	{
						//		destCount = (int)Bgfx.TopologyConvert( TopologyConvert.TriListToLineList, (IntPtr)pDest, (uint)dest.Length * sizeof( uint ), (IntPtr)pIndices, (uint)indices.Count, true );
						//	}
						//}

						//for( int nLine = 0; nLine < destCount / 2; nLine++ )
						//{
						//	var v0 = verticesTransformed[ dest[ nLine * 2 + 0 ] ];
						//	var v1 = verticesTransformed[ dest[ nLine * 2 + 1 ] ];

						//	AddLine( v0, v1, 0 );
						//}
					}
				}
				else
				{
					for( int nTriangle = 0; nTriangle < vertices.Count / 3; nTriangle++ )
					{
						var v0 = verticesTransformed[ nTriangle * 3 + 0 ];
						var v1 = verticesTransformed[ nTriangle * 3 + 1 ];
						var v2 = verticesTransformed[ nTriangle * 3 + 2 ];

						AddLineThin( v0, v1 );
						AddLineThin( v1, v2 );
						AddLineThin( v2, v0 );
					}
				}
			}
			else
			{
				Vector3F[] verticesArray = vertices as Vector3F[];
				int[] indicesArray = indices as int[];
				if( verticesArray != null && ( indices == null || indicesArray != null ) )
				{
					//optimization for arrays
					AddVertexIndexBuffer_Array( verticesArray, indicesArray, ref transform, transformIsIdentity, wireframe, culling );
				}
				else
				{
					//default implementation
					AddVertexIndexBuffer_IList( vertices, indices, ref transform, transformIsIdentity, wireframe, culling );
				}
			}
		}

		public override void AddTriangles( IList<Vector3F> vertices, IList<int> indices, ref Matrix4 transform, bool wireframe, bool culling )
		{
			AddTriangles( vertices, indices, ref transform, false, wireframe, culling );
		}

		public override void AddTriangles( IList<Vector3F> vertices, IList<int> indices, bool wireframe, bool culling )
		{
			//var transform = Matrix4.Identity;
			AddTriangles( vertices, indices, ref transformIdentity, true, wireframe, culling );
		}

		public override void AddTriangles( IList<Vector3> vertices, IList<int> indices, ref Matrix4 transform, bool wireframe, bool culling )
		{
			Vector3F[] array = new Vector3F[ vertices.Count ];
			for( int n = 0; n < vertices.Count; n++ )
				array[ n ] = vertices[ n ].ToVector3F();
			AddTriangles( array, indices, ref transform, false, wireframe, culling );
		}

		public override void AddTriangles( IList<Vector3> vertices, IList<int> indices, bool wireframe, bool culling )
		{
			Vector3F[] array = new Vector3F[ vertices.Count ];
			for( int n = 0; n < vertices.Count; n++ )
				array[ n ] = vertices[ n ].ToVector3F();
			//var transform = Matrix4.Identity;
			AddTriangles( array, indices, ref transformIdentity, true, wireframe, culling );
		}

		public override void AddTriangles( IList<Vector3F> vertices, ref Matrix4 transform, bool wireframe, bool culling )
		{
			AddTriangles( vertices, null, ref transform, false, wireframe, culling );
		}

		public override void AddTriangles( IList<Vector3F> vertices, bool wireframe, bool culling )
		{
			//var transform = Matrix4.Identity;
			AddTriangles( vertices, null, ref transformIdentity, true, wireframe, culling );
		}

		public override void AddTriangles( IList<Vector3> vertices, ref Matrix4 transform, bool wireframe, bool culling )
		{
			AddTriangles( vertices, null, ref transform, wireframe, culling );
		}

		public override void AddTriangles( IList<Vector3> vertices, bool wireframe, bool culling )
		{
			Vector3F[] array = new Vector3F[ vertices.Count ];
			for( int n = 0; n < vertices.Count; n++ )
				array[ n ] = vertices[ n ].ToVector3F();
			//var transform = Matrix4.Identity;
			AddTriangles( array, null, ref transformIdentity, true, wireframe, culling );
		}

		/////////////////////////////////////////

		void InitInternal()
		{
			SetColor( new ColorValue( 1, 1, 1 ) );

			//add RenderSystem listener event for "DeviceRestored"
			RenderingSystem.RenderSystemEvent += RenderSystemEvent;
		}

		void ShutdownInternal()
		{
			//remove RenderSystem listener event for "DeviceRestored"
			RenderingSystem.RenderSystemEvent -= RenderSystemEvent;

			_Clear();

			while( freeRenderablesDynamicallyCreated.Count != 0 )
			{
				RenderableItem_DynamicallyCreated renderableItem = freeRenderablesDynamicallyCreated.Pop();

				renderableItem.vertexBuffer?.Dispose();
				renderableItem.vertexBuffer = null;

				//!!!!глючит на HD7850
				//renderableItem.indexBuffer?.Dispose();
				//renderableItem.indexBuffer = null;

				//if( renderableItem.material != null )
				//{
				//	renderableItem.material.Dispose( true );
				//	renderableItem.material = null;
				//}
			}
		}

		unsafe void PrepareRenderables()
		{
			foreach( Item item in items )
			{
				var itemDynamicallyCreated = item as Item_DynamicallyCreated;
				if( itemDynamicallyCreated != null )
				{
					RenderableItem_DynamicallyCreated renderableItem = GetFreeRenderableDynamicallyCreated();
					if( renderableItem == null )
						return;

					renderableItem.currentItem = item;
					renderableItem.vertexCount = itemDynamicallyCreated.vertexCount;
					//!!!!глючит на HD7850
					//renderableItem.indexCount = itemDynamicallyCreated.indexCount;

					renderableItem.vertexBuffer.SetData( itemDynamicallyCreated.vertices, renderableItem.vertexCount );

					//!!!!глючит на HD7850
					//renderableItem.indexBuffer.SetData( itemDynamicallyCreated.indices, renderableItem.indexCount );




					//fixed ( Vertex* itemV = itemDynamicallyCreated.vertices )
					//	renderableItem.vertexBuffer.Write( (IntPtr)itemV, renderableItem.vertexCount );

					//fixed ( int* itemV = itemDynamicallyCreated.indices )
					//	renderableItem.indexBuffer.Write( (IntPtr)itemV, renderableItem.indexCount );

					//byte[] vertices = new byte[ buffer.Vertices.Length ];
					//fixed ( byte* pV = vertices )
					//fixed ( Vertex* itemV = itemDynamicallyCreated.vertices )
					//	NativeUtils.CopyMemory( (IntPtr)pV, (IntPtr)itemV, vertices.Length );
					//buffer.SetData( vertices );

					//!!!!!
					//DebugGeometryRenderable.SetSquaredViewDepth( renderable, squaredViewDepth );

					preparedRenderables.Add( renderableItem );
				}

				var itemVertexIndexData = item as Item_VertexIndexData;
				if( itemVertexIndexData != null )
				{
					var renderableItem = new RenderableItem_VertexIndexData();
					renderableItem.currentItem = item;
					renderableItem.data = itemVertexIndexData.data;
					preparedRenderables.Add( renderableItem );
				}
			}

			//create material
			if( preparedRenderables.Count != 0 && oneMaterial == null )
			{
				string error;

				var vertexProgram = GpuProgramManager.GetProgram( "Simple3DRenderer_Vertex_", GpuProgramType.Vertex,
					@"Base\Shaders\Simple3DRenderer_vs.sc", null, out error );
				if( !string.IsNullOrEmpty( error ) )
					Log.Fatal( error );

				var fragmentProgram = GpuProgramManager.GetProgram( "Simple3DRenderer_Fragment_", GpuProgramType.Fragment,
					@"Base\Shaders\Simple3DRenderer_fs.sc", null, out error );
				if( !string.IsNullOrEmpty( error ) )
					Log.Fatal( error );

				oneMaterial = new GpuMaterialPass( vertexProgram, fragmentProgram );
			}
		}

		public override void _Clear()
		{
			//free renderables
			foreach( RenderableItem renderableItem in preparedRenderables )
			{
				var renderableItemDynamicallyCreated = renderableItem as RenderableItem_DynamicallyCreated;
				if( renderableItemDynamicallyCreated != null )
					freeRenderablesDynamicallyCreated.Push( renderableItemDynamicallyCreated );
			}
			preparedRenderables.Clear();

			//free items
			foreach( Item item in items )
			{
				var itemDynamicallyCreated = item as Item_DynamicallyCreated;
				if( itemDynamicallyCreated != null )
					freeItemsDynamicallyCreated.Push( itemDynamicallyCreated );
			}
			items.Clear();

			SetColor( new ColorValue( 1, 1, 1 ) );
			//nonOverlappingGroupCounter = 0;
			//nonOverlappingGroup = 0;
		}

		void RenderSystemEvent( RenderSystemEvent name )
		{
			if( name == NeoAxis.RenderSystemEvent.DeviceRestored || name == NeoAxis.RenderSystemEvent.DeviceLost )
				_Clear();
		}

		unsafe RenderableItem_DynamicallyCreated GetFreeRenderableDynamicallyCreated()
		{
			if( freeRenderablesDynamicallyCreated.Count == 0 )
			{
				//create renderable item
				var renderableItem = new RenderableItem_DynamicallyCreated();

				//create vertexData
				{
					var declaration = new VertexLayout().Begin()
						.Add( VertexAttributeUsage.Position, 3, VertexAttributeType.Float )
						.Add( VertexAttributeUsage.Color0, 4, VertexAttributeType.UInt8, true, true )
						.Add( VertexAttributeUsage.Color1, 4, VertexAttributeType.UInt8, true, true )
						.End();
					//var declaration = new VertexLayout().Begin()
					//	.Add( VertexAttributeUsage.Position, 3, VertexAttributeType.Float )
					//	.Add( VertexAttributeUsage.TexCoord0, 4, VertexAttributeType.Float )
					//	.Add( VertexAttributeUsage.TexCoord1, 4, VertexAttributeType.Float )
					//	.End();

					if( sizeof( Vertex ) != declaration.Stride )
						Log.Fatal( "Simple3DRendererImpl: GetFreeRenderableDynamicallyCreated: sizeof( Vertex ) != declaration.Stride." );

					renderableItem.vertexBuffer = GpuBufferManager.CreateVertexBuffer( constBufferVertexCount, declaration, GpuBufferFlags.Dynamic );
					//byte[] vertices = new byte[ declaration.Stride * constBufferVertexCount ];
					//renderableItem.vertexBuffer = GpuBufferManager.CreateVertexBuffer( vertices, declaration, GpuBufferFlags.Dynamic );
					//renderableItem.vertexBuffer = GpuBufferManager.CreateVertexBuffer( sizeof( Vertex ), constBufferVertexCount, null, true );
					if( renderableItem.vertexBuffer == null )
						return null;

					//!!!!глючит на HD7850
					//renderableItem.indexBuffer = GpuBufferManager.CreateIndexBuffer( constBufferIndexCount, GpuBufferFlags.Dynamic );
					////int[] indices = new int[ constBufferIndexCount ];
					////renderableItem.indexBuffer = GpuBufferManager.CreateIndexBuffer( indices, GpuBufferFlags.Dynamic );
					//if( renderableItem.indexBuffer == null )
					//	return null;
				}

				freeRenderablesDynamicallyCreated.Push( renderableItem );
			}

			return freeRenderablesDynamicallyCreated.Pop();
		}

		[StructLayout( LayoutKind.Sequential )]
		struct Simple3DRendererVertexData
		{
			public ColorValue color;
			public ColorValue colorInvisibleBehindObjects;

			public float useColorFromUniform;
			public float depthTextureAvailable;
			public float billboardMode;
			public float unused1;
			//public Vector2F unused1;
		}

		public override bool _ViewportRendering_PrepareRenderables()
		{
			PrepareRenderables();

			return preparedRenderables.Count != 0;
		}

		unsafe public override void _ViewportRendering_RenderToCurrentViewport( ViewportRenderingContext context )
		{
			if( preparedRenderables.Count == 0 )
				return;

			var owner = context.Owner;
			//Mat4F viewMatrix = owner.CameraSettings.ViewMatrix.ToMat4F();
			//Mat4F projectionMatrix = owner.CameraSettings.ProjectionMatrix.ToMat4F();
			//Bgfx.SetViewTransform( context.CurrentViewNumber, (float*)&viewMatrix, (float*)&projectionMatrix );

			//!!!!double
			Vector3F cameraPosition = owner.CameraSettings.Position.ToVector3F();
			context.SetUniform( "u_cameraPosition", ParameterType.Vector3, 1, &cameraPosition );

			var simple3DRendererVertexUniform = GpuProgramManager.RegisterUniform( "u_simple3DRendererVertex", UniformType.Vector4, sizeof( Simple3DRendererVertexData ) / sizeof( Vector4F ) );
			var simple3DRendererVertexData = new Simple3DRendererVertexData();

			Simple3DRendererVertexData currentSimple3DRendererVertex = new Simple3DRendererVertexData();
			currentSimple3DRendererVertex.color = new ColorValue( -10000, 0, 0, 0 );

			Component_Image depthTexture;
			bool depthTextureAvailable;
			{
				context.objectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out depthTexture );
				if( depthTexture != null )
					depthTextureAvailable = true;
				else
				{
					depthTexture = ResourceUtility.WhiteTexture2D;
					depthTextureAvailable = false;
				}
			}

			//Bgfx.SetStencil(StencilFlags.

			//!!!!
			//context.SetViewport( simpleDataTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.Color, new ColorValue( 0, 0, 0, 0 ) );
			//Bgfx.SetStencil( zzzzz, zzzzzz );

			foreach( var renderableItem in preparedRenderables )
			{
				Item item = renderableItem.currentItem;

				var renderableItem_DynamicallyCreated = renderableItem as RenderableItem_DynamicallyCreated;
				var renderableItem_VertexIndexData = renderableItem as RenderableItem_VertexIndexData;
				var item_DynamicallyCreated = renderableItem.currentItem as Item_DynamicallyCreated;
				var item_VertexIndexData = renderableItem.currentItem as Item_VertexIndexData;

				if( renderableItem_DynamicallyCreated != null && renderableItem_DynamicallyCreated.vertexCount == 0 )
					continue;

				//!!!!double
				Matrix4F worldMatrix;
				if( renderableItem.currentItem.transformIsIdentity )
					worldMatrix = Matrix4F.Identity;
				else
					renderableItem.currentItem.transform.ToMatrix4F( out worldMatrix );
				//var worldMatrix = renderableItem.currentItem.transform.ToMatrix4F();

				var pass = oneMaterial;
				//foreach( var pass in oneMaterial.Result.AllPasses )
				{
					//int iterationCount = !item.depthTest ? 2 : 1;
					//for( int nIteration = 0; nIteration < iterationCount; nIteration++ )
					//{
					//update pass
					{
						pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
						pass.DestinationBlendFactor = SceneBlendFactor.OneMinusSourceAlpha;

						pass.DepthCheck = false;
						//pass.DepthCheck = true;
						//if( iterationCount == 2 )
						//	pass.DepthFunction = ( nIteration == 0 ) ? CompareFunction.Greater : CompareFunction.LessEqual;
						//else
						//	pass.DepthFunction = CompareFunction.LessEqual;

						//pass.DepthCheck = item.depthTest;

						pass.DepthWrite = item.depthWrite;
						pass.CullingMode = item.culling ? CullingMode.Clockwise : CullingMode.None;
					}

					//bind depth texture
					context.BindTexture( new ViewportRenderingContext.BindTextureData( 0/*"depthTexture"*/, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					//set uniform
					{
						if( item_DynamicallyCreated != null )
						{
							simple3DRendererVertexData.color = ColorValue.Zero;
							simple3DRendererVertexData.colorInvisibleBehindObjects = ColorValue.Zero;
							simple3DRendererVertexData.useColorFromUniform = -1;
							simple3DRendererVertexData.billboardMode = 0;
						}
						else if( item_VertexIndexData != null )
						{
							simple3DRendererVertexData.color = item_VertexIndexData.color;
							simple3DRendererVertexData.colorInvisibleBehindObjects = item_VertexIndexData.colorInvisibleBehindObjects;
							simple3DRendererVertexData.useColorFromUniform = 1;
							simple3DRendererVertexData.billboardMode = item_VertexIndexData.data.BillboardMode;
						}

						simple3DRendererVertexData.depthTextureAvailable = depthTextureAvailable ? 1 : -1;

						//float selectColor;
						//if( iterationCount == 2 )
						//	selectColor = ( nIteration == 0 ) ? -1 : 1;
						//else
						//	selectColor = 1;
						//simple3DRendererVertexData.selectColor = selectColor;

						if( NativeUtility.CompareMemory( &currentSimple3DRendererVertex, &simple3DRendererVertexData, sizeof( Simple3DRendererVertexData ) ) != 0 )
						{
							currentSimple3DRendererVertex = simple3DRendererVertexData;

							Bgfx.SetUniform( simple3DRendererVertexUniform, &simple3DRendererVertexData, sizeof( Simple3DRendererVertexData ) / sizeof( Vector4F ) );
						}
					}

					List<ParameterContainer> containers = null;
					//List<ParameterContainer> containers = new List<ParameterContainer>();
					//containers.Add( generalContainer );

					//Clip planes
					//virtual void setClipPlanes(const PlaneList& clipPlanes);
					//virtual void resetClipPlanes();

					Bgfx.SetTransform( (float*)&worldMatrix );

					//_DynamicallyCreated
					if( item_DynamicallyCreated != null )
					{
						if( !renderableItem_DynamicallyCreated.vertexBuffer.Disposed )
						{
							var operationType = item_DynamicallyCreated.type == Item_DynamicallyCreated.ItemType.Lines ? RenderOperationType.LineList : RenderOperationType.TriangleList;
							context.SetVertexBuffer( 0, renderableItem_DynamicallyCreated.vertexBuffer, 0, renderableItem_DynamicallyCreated.vertexCount );
							//!!!!глючит на HD7850
							//if( item_DynamicallyCreated.actualIndexData )
							//	context.SetIndexBuffer( renderableItem_DynamicallyCreated.indexBuffer, 0, renderableItem_DynamicallyCreated.indexCount );
							context.SetPassAndSubmit( pass, operationType, containers, item_DynamicallyCreated.occlusionQuery );

							if( operationType == RenderOperationType.TriangleList )
								context.UpdateStatisticsCurrent.Triangles += renderableItem_DynamicallyCreated.vertexCount / 3;
							else if( operationType == RenderOperationType.LineList )
								context.UpdateStatisticsCurrent.Lines += renderableItem_DynamicallyCreated.vertexCount / 2;
						}
					}
					//_VertexIndexData
					else if( item_VertexIndexData != null )
					{
						var data = item_VertexIndexData.data;

						if( !data.ContainsDisposedBuffers() )
						{
							for( int n = 0; n < data.VertexBuffers.Count; n++ )
								context.SetVertexBuffer( n, data.VertexBuffers[ n ], data.VertexStartOffset, data.VertexCount );
							if( data.IndexBuffer != null )
								context.SetIndexBuffer( data.IndexBuffer, data.IndexStartOffset, data.IndexCount );
							context.SetPassAndSubmit( pass, data.OperationType, containers, item_VertexIndexData.occlusionQuery );

							if( data.OperationType == RenderOperationType.TriangleList )
								context.UpdateStatisticsCurrent.Triangles += data.IndexBuffer != null ? data.IndexCount / 3 : data.VertexCount / 3;
							else if( data.OperationType == RenderOperationType.LineList )
								context.UpdateStatisticsCurrent.Lines += data.IndexBuffer != null ? data.IndexCount / 2 : data.VertexCount / 2;
						}
					}

					//}
				}
			}
		}

		double GetThicknessByPixelSize( ref Vector3 worldPosition, double sizeInPixels )
		{
			if( sizeInPixels != 0 )
			{
				//!!!!так?

				//!!!!slowly

				Vector2 viewportSize = viewport.SizeInPixels.ToVector2();
				var cameraSettings = viewport.CameraSettings;

				if( !cameraSettings.ProjectToScreenCoordinates( ref worldPosition, out var objScreenPosition ) )
					return 0;
				double screenOffsetY = sizeInPixels / viewportSize.Y;

				var screenPosition = objScreenPosition + new Vector2( 0, screenOffsetY );
				cameraSettings.GetRayByScreenCoordinates( ref screenPosition, out var rayOffsetted );
				//Ray rayOffsetted = cameraSettings.GetRayByScreenCoordinates( objScreenPosition + new Vector2( 0, screenOffsetY ) );

				Plane.FromVectors( ref cameraSettings.up, ref cameraSettings.right, ref worldPosition, out var plane );
				//Plane plane = Plane.FromVectors( cameraSettings.Up, Vector3.Cross( cameraSettings.Direction, cameraSettings.Up ), worldPosition );

				double scale;
				if( !plane.Intersects( ref rayOffsetted, out scale ) )
					scale = 0;
				rayOffsetted.GetPointOnRay( scale, out var posOffsetted );
				double thickness = ( worldPosition - posOffsetted ).Length();
				return thickness;
			}
			else
				return 0;
		}

		public override double GetThicknessByPixelSize( Vector3 worldPosition, double sizeInPixels )
		{
			return GetThicknessByPixelSize( ref worldPosition, sizeInPixels );
		}

		/////////////////////////////////////////

		public override void AddLine( Vertex start, Vertex end )//, double thickness = 0 )
		{
			//if( thickness != 0 )
			//{
			//	//!!!!можно было бы цилиндрами рисовать
			//	//!!!!!!или сразу не экран проецировать

			//	double length = ( end - start ).Length();
			//	double thickness2 = thickness;
			//	//if( drawShadows )
			//	//{
			//	//	//!!!!в конфиге настраивать?
			//	//	length += thickness2;
			//	//	thickness2 *= 2.7f;
			//	//	//thickness2 *= 2.5f;// 3;
			//	//}

			//	Vec3 direction = end - start;
			//	if( direction == Vec3.Zero )
			//		direction = Vec3.XAxis;
			//	direction.Normalize();
			//	//!!!!?
			//	Quat rotation = Quat.FromDirectionZAxisUp( direction );

			//	Mat4 t = new Mat4( rotation.ToMat3() * Mat3.FromScale( new Vec3( length, thickness2, thickness2 ) ), ( start + end ) * .5f );

			//	if( boxShapePositions == null )
			//		SimpleMeshGenerator.GenerateBox( new Vec3F( 1, 1, 1 ), out boxShapePositions, out boxShapeIndices );
			//	AddTriangles( boxShapePositions, boxShapeIndices, t, false, true );
			//}
			//else
			{
				Item_DynamicallyCreated item = null;

				//try use last item
				if( items.Count != 0 )
				{
					Item_DynamicallyCreated lastItem = items[ items.Count - 1 ] as Item_DynamicallyCreated;

					if( lastItem != null &&
						lastItem.type == Item_DynamicallyCreated.ItemType.Lines &&
						lastItem.depthTest == currentDepthTest &&
						lastItem.depthWrite == currentDepthWrite &&
						Equal( lastItem.occlusionQuery, currentOcclusionQuery ) &&
						//lastItem.nonOverlappingGroup == nonOverlappingGroup &&
						lastItem.vertexCount + 2 <= constBufferVertexCount &&
						lastItem.indexCount + 2 <= constBufferIndexCount )
					{
						item = lastItem;
					}
				}

				if( item == null )
				{
					//use new item

					if( freeItemsDynamicallyCreated.Count == 0 )
						freeItemsDynamicallyCreated.Push( new Item_DynamicallyCreated() );
					item = freeItemsDynamicallyCreated.Pop();

					item.type = Item_DynamicallyCreated.ItemType.Lines;
					item.depthTest = currentDepthTest;
					item.depthWrite = currentDepthWrite;
					item.occlusionQuery = currentOcclusionQuery;
					//item.nonOverlappingGroup = nonOverlappingGroup;
					item.transform = Matrix4.Identity;
					item.transformIsIdentity = true;
					item.culling = false;
					item.vertexCount = 0;
					item.indexCount = 0;
					item.actualIndexData = false;

					items.Add( item );
				}

				unsafe
				{
					fixed( byte* pVertices2 = item.vertices )
					{
						Vertex* pVertices = (Vertex*)pVertices2;
						Vertex* pVertex = pVertices + item.vertexCount;

						*pVertex = start;
						pVertex++;

						*pVertex = end;
					}
				}

				item.indices[ item.indexCount++ ] = item.vertexCount++;
				item.indices[ item.indexCount++ ] = item.vertexCount++;
				//item.vertexCount += 2;
			}
		}

		void AddVertexIndexBuffer_Array( Vertex[] vertices, int[] indices, ref Matrix4 transform, bool transformIsIdentity, bool wireframe, bool culling )
		{
			int triangleCount;
			if( indices != null )
				triangleCount = indices.Length / 3;
			else
				triangleCount = vertices.Length / 3;

			Item_DynamicallyCreated item = null;

			//!!!!?
			//try use last item

			for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
			{
				if( item == null || item.vertexCount + 3 > constBufferVertexCount || item.indexCount + 3 > constBufferIndexCount )
				{
					//get new item

					if( freeItemsDynamicallyCreated.Count == 0 )
						freeItemsDynamicallyCreated.Push( new Item_DynamicallyCreated() );
					item = freeItemsDynamicallyCreated.Pop();

					item.type = Item_DynamicallyCreated.ItemType.Triangles;
					item.depthTest = currentDepthTest;
					item.depthWrite = currentDepthWrite;
					item.occlusionQuery = currentOcclusionQuery;
					//item.nonOverlappingGroup = nonOverlappingGroup;
					item.vertexCount = 0;
					item.indexCount = 0;
					item.actualIndexData = false;
					item.transform = transform;
					item.transformIsIdentity = transformIsIdentity;
					item.wireframe = wireframe;
					item.culling = culling;

					items.Add( item );
				}

				unsafe
				{
					if( indices != null )
					{
						int index0 = indices[ nTriangle * 3 + 0 ];
						int index1 = indices[ nTriangle * 3 + 1 ];
						int index2 = indices[ nTriangle * 3 + 2 ];

						fixed( byte* pVertices2 = item.vertices )
						{
							Vertex* pVertices = (Vertex*)pVertices2;
							Vertex* pVertex = pVertices + item.vertexCount;
							*pVertex = vertices[ index0 ];
							pVertex++;
							*pVertex = vertices[ index1 ];
							pVertex++;
							*pVertex = vertices[ index2 ];
						}
					}
					else
					{
						fixed( byte* pVertices2 = item.vertices )
						{
							Vertex* pVertices = (Vertex*)pVertices2;
							Vertex* pVertex = pVertices + item.vertexCount;
							*pVertex = vertices[ nTriangle * 3 + 0 ];
							pVertex++;
							*pVertex = vertices[ nTriangle * 3 + 1 ];
							pVertex++;
							*pVertex = vertices[ nTriangle * 3 + 2 ];
						}
					}
				}

				item.indices[ item.indexCount++ ] = item.vertexCount++;
				item.indices[ item.indexCount++ ] = item.vertexCount++;
				item.indices[ item.indexCount++ ] = item.vertexCount++;
				//item.vertexCount += 3;
			}
		}

		void AddVertexIndexBuffer_IList( IList<Vertex> vertices, IList<int> indices, ref Matrix4 transform, bool transformIsIdentity, bool wireframe, bool culling )
		{
			int triangleCount;
			if( indices != null )
				triangleCount = indices.Count / 3;
			else
				triangleCount = vertices.Count / 3;

			Item_DynamicallyCreated item = null;

			//!!!!?
			//try use last item

			for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
			{
				if( item == null || item.vertexCount + 3 > constBufferVertexCount || item.indexCount + 3 > constBufferIndexCount )
				{
					//get new item

					if( freeItemsDynamicallyCreated.Count == 0 )
						freeItemsDynamicallyCreated.Push( new Item_DynamicallyCreated() );
					item = freeItemsDynamicallyCreated.Pop();

					item.type = Item_DynamicallyCreated.ItemType.Triangles;
					item.depthTest = currentDepthTest;
					item.depthWrite = currentDepthWrite;
					item.occlusionQuery = currentOcclusionQuery;
					//item.nonOverlappingGroup = nonOverlappingGroup;
					item.vertexCount = 0;
					item.indexCount = 0;
					item.actualIndexData = false;
					item.transform = transform;
					item.transformIsIdentity = transformIsIdentity;
					item.wireframe = wireframe;
					item.culling = culling;

					items.Add( item );
				}

				unsafe
				{
					if( indices != null )
					{
						int index0 = indices[ nTriangle * 3 + 0 ];
						int index1 = indices[ nTriangle * 3 + 1 ];
						int index2 = indices[ nTriangle * 3 + 2 ];

						fixed( byte* pVertices2 = item.vertices )
						{
							Vertex* pVertices = (Vertex*)pVertices2;
							Vertex* pVertex = pVertices + item.vertexCount;
							*pVertex = vertices[ index0 ];
							pVertex++;
							*pVertex = vertices[ index1 ];
							pVertex++;
							*pVertex = vertices[ index2 ];
						}
					}
					else
					{
						fixed( byte* pVertices2 = item.vertices )
						{
							Vertex* pVertices = (Vertex*)pVertices2;
							Vertex* pVertex = pVertices + item.vertexCount;
							*pVertex = vertices[ nTriangle * 3 + 0 ];
							pVertex++;
							*pVertex = vertices[ nTriangle * 3 + 1 ];
							pVertex++;
							*pVertex = vertices[ nTriangle * 3 + 2 ];
						}
					}
				}

				item.indices[ item.indexCount++ ] = item.vertexCount++;
				item.indices[ item.indexCount++ ] = item.vertexCount++;
				item.indices[ item.indexCount++ ] = item.vertexCount++;
				//item.vertexCount += 3;
			}
		}

		void AddTriangles( IList<Vertex> vertices, IList<int> indices, ref Matrix4 transform, bool transformIsIdentity, bool wireframe, bool culling )
		{
			if( indices != null )
			{
				if( indices.Count % 3 != 0 )
					Log.Fatal( "Simple3DRenderer: AddTriangles: Invalid index count." );
			}
			else
			{
				if( vertices.Count % 3 != 0 )
					Log.Fatal( "Simple3DRenderer: AddTriangles: Invalid vertex count." );
			}

			if( wireframe )
			{
				//convert to lines. no wireframe triangles in bgfx.

				//!!!!slowly

				Vector3[] verticesTransformed = new Vector3[ vertices.Count ];
				{
					bool identity = transformIsIdentity;// transform == Matrix4.Identity;
					for( int n = 0; n < verticesTransformed.Length; n++ )
					{
						Vector3 v = vertices[ n ].position.ToVector3();
						if( !identity )
							Matrix4.Multiply( ref transform, ref v, out verticesTransformed[ n ] );//v = transform * v;
						else
							verticesTransformed[ n ] = v;
					}
				}

				if( indices != null )
				{
					int[] indicesArray;
					indicesArray = indices as int[];
					if( indicesArray == null )
						indicesArray = indices.ToArray();

					//AddTriangleList( verticesTransformed, indicesArray );
					//AddLineList( verticesTransformed, tempIndices );

					int[] dest = new int[ indicesArray.Length * 2 ];
					int destCount;
					unsafe
					{
						fixed( int* pDest = dest, pIndices = indicesArray )
						{
							destCount = (int)Bgfx.TopologyConvert( TopologyConvert.TriListToLineList, (IntPtr)pDest, (uint)dest.Length * sizeof( uint ), (IntPtr)pIndices, (uint)indices.Count, true );
						}
					}

					for( int nLine = 0; nLine < destCount / 2; nLine++ )
					{
						int index0 = dest[ nLine * 2 + 0 ];
						int index1 = dest[ nLine * 2 + 1 ];

						var p0 = verticesTransformed[ index0 ];
						var p1 = verticesTransformed[ index1 ];

						var v0 = new Vertex( p0.ToVector3F(), vertices[ index0 ].color, vertices[ index0 ].colorInvisibleBehindObjects );
						var v1 = new Vertex( p1.ToVector3F(), vertices[ index1 ].color, vertices[ index1 ].colorInvisibleBehindObjects );

						AddLine( v0, v1 );
					}
				}
				else
				{
					for( int nTriangle = 0; nTriangle < vertices.Count / 3; nTriangle++ )
					{
						int index0 = nTriangle * 3 + 0;
						int index1 = nTriangle * 3 + 1;
						int index2 = nTriangle * 3 + 2;

						var p0 = verticesTransformed[ index0 ];
						var p1 = verticesTransformed[ index1 ];
						var p2 = verticesTransformed[ index2 ];

						var v0 = new Vertex( p0.ToVector3F(), vertices[ index0 ].color, vertices[ index0 ].colorInvisibleBehindObjects );
						var v1 = new Vertex( p1.ToVector3F(), vertices[ index1 ].color, vertices[ index1 ].colorInvisibleBehindObjects );
						var v2 = new Vertex( p2.ToVector3F(), vertices[ index2 ].color, vertices[ index2 ].colorInvisibleBehindObjects );

						AddLine( v0, v1 );
						AddLine( v1, v2 );
						AddLine( v2, v0 );
					}
				}
			}
			else
			{
				Vertex[] verticesArray = vertices as Vertex[];
				int[] indicesArray = indices as int[];
				if( verticesArray != null && ( indices == null || indicesArray != null ) )
				{
					//optimization for arrays
					AddVertexIndexBuffer_Array( verticesArray, indicesArray, ref transform, transformIsIdentity, wireframe, culling );
				}
				else
				{
					//default implementation
					AddVertexIndexBuffer_IList( vertices, indices, ref transform, transformIsIdentity, wireframe, culling );
				}
			}
		}

		public override void AddTriangles( IList<Vertex> vertices, IList<int> indices, ref Matrix4 transform, bool wireframe, bool culling )
		{
			AddTriangles( vertices, indices, ref transform, false, wireframe, culling );
		}

		public override void AddTriangles( IList<Vertex> vertices, IList<int> indices, bool wireframe, bool culling )
		{
			//var transform = Matrix4.Identity;
			AddTriangles( vertices, indices, ref transformIdentity, true, wireframe, culling );
		}

		public override void AddTriangles( IList<Vertex> vertices, ref Matrix4 transform, bool wireframe, bool culling )
		{
			AddTriangles( vertices, null, ref transform, false, wireframe, culling );
		}

		public override void AddTriangles( IList<Vertex> vertices, bool wireframe, bool culling )
		{
			//var transform = Matrix4.Identity;
			AddTriangles( vertices, null, ref transformIdentity, true, wireframe, culling );
		}

		public override void AddVertexIndexData( VertexIndexData data, ref Matrix4 transform, bool wireframe, bool culling )
		{
			//!!!!
			if( wireframe )
				Log.Fatal( "Simple3DRendererImpl: AddVertexIndexData: wireframe mode is not supported." );

			if( data.ContainsDisposedBuffers() )
				return;

			var item = new Item_VertexIndexData();
			item.data = data;
			item.color = currentColor;
			item.colorInvisibleBehindObjects = currentColorInvisibleBehindObjects;
			item.depthTest = currentDepthTest;
			item.depthWrite = currentDepthWrite;
			item.occlusionQuery = currentOcclusionQuery;
			//item.nonOverlappingGroup = nonOverlappingGroup;
			item.transform = transform;
			//!!!!
			item.transformIsIdentity = false;

			item.wireframe = wireframe;
			item.culling = culling;
			items.Add( item );
		}

		public override void AddMesh( Component_Mesh.CompiledData meshData, ref Matrix4 transform, bool wireframe, bool culling )
		{
			//!!!!может порядок менять полезно?

			foreach( var oper in meshData.MeshData.RenderOperations )
			{
				//var oper = item.operation;
				if( oper.ContainsDisposedBuffers() )
					continue;

				ref Matrix4 worldMatrix = ref transform;
				//if( !item.transform.IsIdentity )
				//	worldMatrix *= item.transform.ToMat4();

				var data = new VertexIndexData();
				data.OperationType = RenderOperationType.TriangleList;
				//data.VertexStructure = oper.vertexStructure;
				data.VertexBuffers = oper.VertexBuffers;
				data.VertexStartOffset = oper.VertexStartOffset;
				data.VertexCount = oper.VertexCount;
				data.IndexBuffer = oper.IndexBuffer;
				data.IndexStartOffset = oper.IndexStartOffset;
				data.IndexCount = oper.IndexCount;
				data.BillboardMode = meshData.MeshData.BillboardMode;

				AddVertexIndexData( data, ref worldMatrix, wireframe, culling );
			}
		}

		//!!!!no sense
		static bool Equal( OcclusionQuery? v1, OcclusionQuery? v2 )
		{
			int handle1 = v1.HasValue ? v1.Value.handle : -1;
			int handle2 = v2.HasValue ? v2.Value.handle : -1;
			return handle1 == handle2;
		}


		public unsafe override void AddRectangle( ref Rectangle rectangle, ref Matrix4 transform, bool solid, double lineThickness )
		{
			if( solid )
			{
				var p = new Vector3[ 4 ];
				p[ 0 ] = new Vector3( rectangle.Left, rectangle.Top, 0 );
				p[ 1 ] = new Vector3( rectangle.Right, rectangle.Top, 0 );
				p[ 2 ] = new Vector3( rectangle.Right, rectangle.Bottom, 0 );
				p[ 3 ] = new Vector3( rectangle.Left, rectangle.Bottom, 0 );

				var indices = new int[] { 0, 1, 2, 2, 3, 0 };
				AddTriangles( p, indices, ref transform, false, true );
			}
			else
			{
				Vector3* p = stackalloc Vector3[ 4 ];
				p[ 0 ] = transform * new Vector3( rectangle.Left, rectangle.Top, 0 );
				p[ 1 ] = transform * new Vector3( rectangle.Right, rectangle.Top, 0 );
				p[ 2 ] = transform * new Vector3( rectangle.Right, rectangle.Bottom, 0 );
				p[ 3 ] = transform * new Vector3( rectangle.Left, rectangle.Bottom, 0 );

				AddLine( p[ 0 ], p[ 1 ], lineThickness );
				AddLine( p[ 1 ], p[ 2 ], lineThickness );
				AddLine( p[ 2 ], p[ 3 ], lineThickness );
				AddLine( p[ 3 ], p[ 0 ], lineThickness );
			}
		}

		public override void AddEllipse( ref Vector2 dimensions, int edges, ref Matrix4 transform, bool solid, double lineThickness )
		{
			if( solid )
			{
				var r = dimensions * 0.5f;

				var p = new Vector3[ edges + 1 ];
				//p[ 0 ] = Vector3.Zero;

				for( int n = 0; n < edges; n++ )
				{
					double angle = (double)n / (double)edges * Math.PI * 2;
					double p1sin = Math.Sin( angle );
					double p1cos = Math.Cos( angle );
					p[ n + 1 ] = new Vector3( p1cos * r.X, p1sin * r.Y, 0 );
				}

				var indices = new int[ edges * 3 ];
				for( int n = 0; n < edges; n++ )
				{
					//indices[ n * 3 + 0 ] = 0;
					indices[ n * 3 + 1 ] = n + 1;
					indices[ n * 3 + 2 ] = ( n + 1 ) % edges + 1;
				}

				AddTriangles( p, indices, ref transform, false, true );
			}
			else
			{
				ref var t = ref transform;
				var r = dimensions * 0.5f;

				double angleStep = Math.PI * 2 / (double)edges;
				for( double angle = 0; angle < Math.PI * 2 - angleStep * .5; angle += angleStep )
				{
					double p1sin = Math.Sin( angle );
					double p1cos = Math.Cos( angle );
					double p2sin = Math.Sin( angle + angleStep );
					double p2cos = Math.Cos( angle + angleStep );

					AddLine( t * new Vector3( p1cos * r.X, p1sin * r.Y, 0 ), t * new Vector3( p2cos * r.X, p2sin * r.Y, 0 ), lineThickness );
				}
			}
		}

	}
}
