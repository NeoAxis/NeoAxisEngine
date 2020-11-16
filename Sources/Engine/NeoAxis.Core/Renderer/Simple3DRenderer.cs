// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	//!!!!группы. для того, чтобы корректно рисовать пачку прозрачных объектов
	//!!!!!!тени тут тоже в группе
	//BeginGroup, EndGroup?

	//!!!!NearClipDistance по идее можно быть отличной от сцены


	/// <summary>
	/// Defines a class for a rendering of simple and dynamic geometry.
	/// </summary>
	public abstract class Simple3DRenderer
	{
		public abstract Viewport Viewport
		{
			get;
		}

		/////////////////////////////////////////

		public abstract bool _ViewportRendering_PrepareRenderables();
		public abstract void _ViewportRendering_RenderToCurrentViewport( ViewportRenderingContext context );
		public abstract void _Clear();

		/////////////////////////////////////////

		public abstract double GetThicknessByPixelSize( Vector3 worldPosition, double sizeInPixels );

		/////////////////////////////////////////

		//!!!!depth write is not supported
		public abstract void SetColor( ColorValue colorVisible, ColorValue colorInvisibleBehindObjects );//, bool depthWrite = false );

		public void SetColor( ColorValue color, bool depthTest = true )//, bool depthWrite = false )
		{
			//!!!!можно три режима ввести, чтобы когда не проверять, то за 1 раз рисовать. с другой стороны можно всегда 1 раз рисовать

			SetColor( color, depthTest ? new ColorValue( 0, 0, 0, 0 ) : color );//, depthWrite );
		}

		public abstract void SetOcclusionQuery( SharpBgfx.OcclusionQuery? query );

		//public abstract void EnableNonOverlappingGroup();
		//public abstract void DisableNonOverlappingGroup();

		/////////////////////////////////////////

		///// <summary>
		///// Sets the color.
		///// </summary>
		//public abstract ColorValue Color { get; set; }

		////!!!!!особые способы рисования, типа прозрачно с такой то полупрозрачностью если видно и такой-то если не видно
		///// <summary>
		///// Sets the depth test and depth write settings.
		///// Must call <see cref="RestoreDefaultDepthSettings"/> after adding geometry.
		///// </summary>
		///// <param name="depthTest"></param>
		///// <param name="depthWrite"></param>
		///// <remarks>
		///// Default settings: depth test = <b>true</b>, depth write = <b>false</b>.
		///// </remarks>
		//public abstract void SetSpecialDepthSettings( bool depthTest, bool depthWrite );

		///// <summary>
		///// Restore default depth settings after call <see cref="SetSpecialDepthSettings"/>.
		///// </summary>
		///// <remarks>
		///// Default settings: depth test = <b>true</b>, depth write = <b>false</b>.
		///// </remarks>
		//public abstract void RestoreDefaultDepthSettings();

		/// <summary>
		/// Renders a line.
		/// </summary>
		/// <param name="start">The line start position.</param>
		/// <param name="end">The line end position.</param>
		/// <param name="thickness"></param>
		public abstract void AddLine( Vector3 start, Vector3 end, double thickness = 0 );

		/// <summary>
		/// Renders a line.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <param name="thickness"></param>
		public void AddLine( Line3 line, double thickness = 0 )
		{
			AddLine( line.Start, line.End, thickness );
		}

		/// <summary>
		/// Renders a line.
		/// </summary>
		/// <param name="start">The line start position.</param>
		/// <param name="end">The line end position.</param>
		public void AddLineThin( Vector3 start, Vector3 end )
		{
			AddLine( start, end, -1 );
		}

		/// <summary>
		/// Renders a line.
		/// </summary>
		/// <param name="line">The line.</param>
		public void AddLineThin( Line3 line )
		{
			AddLineThin( line.Start, line.End );
		}

		///// <summary>
		///// Renders a line.
		///// </summary>
		///// <param name="start">The line start position.</param>
		///// <param name="end">The line end position.</param>
		///// <param name="thickness"></param>
		//public abstract void AddLine( Vector3 start, Vector3 end, double thickness = 0 );

		///// <summary>
		///// Renders a line.
		///// </summary>
		///// <param name="line">The line.</param>
		///// <param name="thickness"></param>
		//public void AddLine( Line3 line, double thickness = 0 )
		//{
		//	AddLine( line.Start, line.End, thickness );
		//}

		/// <summary>
		/// Renders a bounds.
		/// </summary>
		/// <param name="bounds">The bounds.</param>
		public abstract void AddBounds( Bounds bounds, bool solid = false, double lineThickness = 0 );

		public abstract void AddSphere( Matrix4 transform, double radius, int segments = 32, bool solid = false, double lineThickness = 0 );

		/// <summary>
		/// Renders a sphere.
		/// </summary>
		/// <param name="sphere">The sphere</param>
		/// <param name="segments">The count of lines in circles of sphere.</param>
		public void AddSphere( Sphere sphere, int segments = 32, bool solid = false, double lineThickness = 0 )
		{
			AddSphere( Matrix4.FromTranslate( sphere.Origin ), sphere.Radius, segments, solid, lineThickness );
		}

		/// <summary>
		/// Renders a sphere.
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="radius"></param>
		/// <param name="segments">The count of lines in circles of sphere.</param>
		public void AddSphere( Vector3 origin, double radius, int segments = 32, bool solid = false, double lineThickness = 0 )
		{
			AddSphere( new Sphere( origin, radius ), segments, solid, lineThickness );
		}

		/// <summary>
		/// Renders a box.
		/// </summary>
		/// <param name="box"></param>
		public abstract void AddBox( Box box, bool solid = false, double lineThickness = 0 );

		public abstract void AddCone( Matrix4 transform, int axis, SimpleMeshGenerator.ConeOrigin origin, double radius, double height, int edgeSegments, int basicSegments, bool solid = false, double lineThickness = 0 );
		//public abstract void AddCone( Vec3 position, Quat rotation, double length, double radius, int edgeSegments, int basicSegments, bool solid = false, double lineThickness = 0 );

		//!!!!странная структура конуса. сложный какой-то
		//public abstract void AddCone( Cone cone, int edgeSegments, int basicSegments, bool solid = false, double lineThickness = 0 );

		//!!!!два segments как у конуса
		public abstract void AddCapsule( Matrix4 transform, int axis, double radius, double height, int segments = 32, bool solid = false, double lineThickness = 0 );

		//!!!!два segments как у конуса
		public abstract void AddCapsule( Capsule capsule, int segments = 32, bool solid = false, double lineThickness = 0 );

		//!!!!два segments как у конуса
		public abstract void AddCylinder( Matrix4 transform, int axis, double radius, double height, int segments = 32, bool solid = false, double lineThickness = 0 );

		//!!!!два segments как у конуса
		public abstract void AddCylinder( Cylinder cylinder, int segments = 32, bool solid = false, double lineThickness = 0 );

		/// <summary>
		/// Renders an arrow.
		/// </summary>
		public abstract void AddArrow( Vector3 from, Vector3 to, double headHeight = 0, double headRadius = 0, bool solid = false, double lineThickness = 0 );

		public void AddArrow( Ray ray, double headHeight = 0, double headRadius = 0, bool solid = false, double lineThickness = 0 )
		{
			AddArrow( ray.Origin, ray.Origin + ray.Direction, headHeight, headRadius, solid, lineThickness );
		}

		///// <summary>
		///// Renders an arrow.
		///// </summary>
		///// <param name="from">The arrow start position.</param>
		///// <param name="to">The arrow end position.</param>
		///// <param name="arrowSize">The size of arrow.</param>
		///// <param name="noLine">Whether to draw directing line.</param>
		//public abstract void AddArrow( Vec3 from, Vec3 to, double arrowSize, bool noLine );

		///// <summary>
		///// Renders an arrow.
		///// </summary>
		///// <param name="arrow">The arrow.</param>
		///// <param name="arrowSize">The size of arrow.</param>
		///// <param name="noLine">Whether to draw directing line.</param>
		//public void AddArrow( Line3 arrow, double arrowSize, bool noLine )
		//{
		//	AddArrow( arrow.Start, arrow.End, arrowSize, noLine );
		//}

		///// <summary>
		///// Renders an arrow.
		///// </summary>
		///// <param name="arrow">The arrow.</param>
		///// <param name="arrowSize">The size of arrow.</param>
		///// <param name="noLine">Whether to draw directing line.</param>
		//public void AddArrow( Ray arrow, double arrowSize, bool noLine )
		//{
		//	AddArrow( arrow.Origin, arrow.Origin + arrow.Direction, arrowSize, noLine );
		//}

		///// <summary>
		///// Renders an arrow.
		///// </summary>
		///// <param name="from">The arrow start position.</param>
		///// <param name="to">The arrow end position.</param>
		///// <param name="arrowSize">The size of arrow.</param>
		//public void AddArrow( Vec3 from, Vec3 to, double arrowSize )
		//{
		//	AddArrow( from, to, arrowSize, false );
		//}

		///// <summary>
		///// Renders an arrow.
		///// </summary>
		///// <param name="arrow">The arrow.</param>
		///// <param name="arrowSize">The size of arrow.</param>
		//public void AddArrow( Line3 arrow, double arrowSize )
		//{
		//	AddArrow( arrow.Start, arrow.End, arrowSize );
		//}

		///// <summary>
		///// Renders an arrow.
		///// </summary>
		///// <param name="arrow">The arrow ray.</param>
		///// <param name="arrowSize">The size of arrow.</param>
		//public void AddArrow( Ray arrow, double arrowSize )
		//{
		//	AddArrow( arrow.Origin, arrow.Origin + arrow.Direction, arrowSize );
		//}

		///// <summary>
		///// Renders an arrow.
		///// </summary>
		///// <param name="from">The arrow start position.</param>
		///// <param name="to">The arrow end position.</param>
		//public void AddArrow( Vec3 from, Vec3 to )
		//{
		//	AddArrow( from, to, 1.0f );
		//}

		///// <summary>
		///// Renders an arrow.
		///// </summary>
		///// <param name="arrow">The arrow.</param>
		//public void AddArrow( Line3 arrow )
		//{
		//	AddArrow( arrow.Start, arrow.End );
		//}

		///// <summary>
		///// Renders an arrow.
		///// </summary>
		///// <param name="arrow">The arrow ray.</param>
		//public void AddArrow( Ray arrow )
		//{
		//	AddArrow( arrow.Origin, arrow.Origin + arrow.Direction );
		//}

		/////////////////////////////////////////
		//AddTriangles with transform

		/// <summary>
		/// Renders a vertex/index buffer.
		/// </summary>
		/// <param name="vertices">The vertices.</param>
		/// <param name="indices">The indices.</param>
		/// <param name="transform">The world transformation.</param>
		/// <param name="wireframe">The wireframe mode flag.</param>
		/// <param name="culling">The culling flag.</param>
		public abstract void AddTriangles( IList<Vector3> vertices, IList<int> indices, ref Matrix4 transform, bool wireframe, bool culling );
		/// <summary>
		/// Renders a vertex/index buffer.
		/// </summary>
		/// <param name="vertices">The vertices.</param>
		/// <param name="indices">The indices.</param>
		/// <param name="transform">The world transformation.</param>
		/// <param name="wireframe">The wireframe mode flag.</param>
		/// <param name="culling">The culling flag.</param>
		public void AddTriangles( IList<Vector3> vertices, IList<int> indices, Matrix4 transform, bool wireframe, bool culling )
		{
			AddTriangles( vertices, indices, ref transform, wireframe, culling );
		}

		/// <summary>
		/// Renders a vertex/index buffer.
		/// </summary>
		/// <param name="vertices">The vertices.</param>
		/// <param name="indices">The indices.</param>
		/// <param name="transform">The world transformation.</param>
		/// <param name="wireframe">The wireframe mode flag.</param>
		/// <param name="culling">The culling flag.</param>
		public abstract void AddTriangles( IList<Vector3F> vertices, IList<int> indices, ref Matrix4 transform, bool wireframe, bool culling );
		/// <summary>
		/// Renders a vertex/index buffer.
		/// </summary>
		/// <param name="vertices">The vertices.</param>
		/// <param name="indices">The indices.</param>
		/// <param name="transform">The world transformation.</param>
		/// <param name="wireframe">The wireframe mode flag.</param>
		/// <param name="culling">The culling flag.</param>
		public void AddTriangles( IList<Vector3F> vertices, IList<int> indices, Matrix4 transform, bool wireframe, bool culling )
		{
			AddTriangles( vertices, indices, ref transform, wireframe, culling );
		}

		public abstract void AddTriangles( IList<Vector3F> vertices, ref Matrix4 transform, bool wireframe, bool culling );
		public void AddTriangles( IList<Vector3F> vertices, Matrix4 transform, bool wireframe, bool culling )
		{
			AddTriangles( vertices, ref transform, wireframe, culling );
		}

		public abstract void AddTriangles( IList<Vector3> vertices, ref Matrix4 transform, bool wireframe, bool culling );
		public void AddTriangles( IList<Vector3> vertices, Matrix4 transform, bool wireframe, bool culling )
		{
			AddTriangles( vertices, ref transform, wireframe, culling );
		}

		/////////////////////////////////////////
		//AddTriangles without transform

		/// <summary>
		/// Renders a vertex/index buffer.
		/// </summary>
		/// <param name="vertices">The vertices.</param>
		/// <param name="indices">The indices.</param>
		/// <param name="wireframe">The wireframe mode flag.</param>
		/// <param name="culling">The culling flag.</param>
		public abstract void AddTriangles( IList<Vector3> vertices, IList<int> indices, bool wireframe, bool culling );

		/// <summary>
		/// Renders a vertex/index buffer.
		/// </summary>
		/// <param name="vertices">The vertices.</param>
		/// <param name="indices">The indices.</param>
		/// <param name="wireframe">The wireframe mode flag.</param>
		/// <param name="culling">The culling flag.</param>
		public abstract void AddTriangles( IList<Vector3F> vertices, IList<int> indices, bool wireframe, bool culling );

		public abstract void AddTriangles( IList<Vector3F> vertices, bool wireframe, bool culling );

		public abstract void AddTriangles( IList<Vector3> vertices, bool wireframe, bool culling );

		/////////////////////////////////////////

		/// <summary>
		/// Represents vertex data for <see cref="Simple3DRenderer"/>.
		/// </summary>
		[StructLayout( LayoutKind.Sequential )]
		public struct Vertex
		{
			public Vector3F position;
			//!!!!maybe return back to uint?
			//public ColorValue color;
			//public ColorValue colorInvisibleBehindObjects;
			public uint color;
			public uint colorInvisibleBehindObjects;

			public Vertex( Vector3F position, uint color, uint colorInvisibleBehindObjects )
			{
				this.position = position;
				this.color = color;
				this.colorInvisibleBehindObjects = colorInvisibleBehindObjects;
			}

			//public Vertex( Vector3F position, ColorValue color, ColorValue colorInvisibleBehindObjects )
			//{
			//	this.position = position;
			//	this.color = color;
			//	this.colorInvisibleBehindObjects = colorInvisibleBehindObjects;
			//}
		}

		/// <summary>
		/// Renders a line.
		/// </summary>
		/// <param name="start">The line start position.</param>
		/// <param name="end">The line end position.</param>
		public abstract void AddLine( Vertex start, Vertex end );//, double thickness = 0 );

		/////////////////////////////////////////
		//AddTriangles vertex buffer with transform

		/// <summary>
		/// Renders a vertex/index buffer.
		/// </summary>
		/// <param name="vertices">The vertices.</param>
		/// <param name="indices">The indices.</param>
		/// <param name="transform">The world transformation.</param>
		/// <param name="wireframe">The wireframe mode flag.</param>
		/// <param name="culling">The culling flag.</param>
		public abstract void AddTriangles( IList<Vertex> vertices, IList<int> indices, ref Matrix4 transform, bool wireframe, bool culling );
		/// <summary>
		/// Renders a vertex/index buffer.
		/// </summary>
		/// <param name="vertices">The vertices.</param>
		/// <param name="indices">The indices.</param>
		/// <param name="transform">The world transformation.</param>
		/// <param name="wireframe">The wireframe mode flag.</param>
		/// <param name="culling">The culling flag.</param>
		public void AddTriangles( IList<Vertex> vertices, IList<int> indices, Matrix4 transform, bool wireframe, bool culling )
		{
			AddTriangles( vertices, indices, ref transform, wireframe, culling );
		}

		public abstract void AddTriangles( IList<Vertex> vertices, ref Matrix4 transform, bool wireframe, bool culling );
		public void AddTriangles( IList<Vertex> vertices, Matrix4 transform, bool wireframe, bool culling )
		{
			AddTriangles( vertices, ref transform, wireframe, culling );
		}

		/////////////////////////////////////////
		//AddTriangles vertex buffer without transform

		/// <summary>
		/// Renders a vertex/index buffer.
		/// </summary>
		/// <param name="vertices">The vertices.</param>
		/// <param name="indices">The indices.</param>
		/// <param name="wireframe">The wireframe mode flag.</param>
		/// <param name="culling">The culling flag.</param>
		public abstract void AddTriangles( IList<Vertex> vertices, IList<int> indices, bool wireframe, bool culling );

		public abstract void AddTriangles( IList<Vertex> vertices, bool wireframe, bool culling );

		/////////////////////////////////////////

		/// <summary>
		/// Represents a vertex/index data for using in <see cref="Simple3DRenderer"/>.
		/// </summary>
		public class VertexIndexData
		{
			public RenderOperationType OperationType = RenderOperationType.TriangleList;

			//public VertexElement[] VertexStructure;
			public IList<GpuVertexBuffer> VertexBuffers;
			public int VertexStartOffset;
			public int VertexCount;
			public GpuIndexBuffer IndexBuffer;
			public int IndexStartOffset;
			public int IndexCount;

			public int BillboardMode;

			//

			public bool ContainsDisposedBuffers()
			{
				if( VertexBuffers != null )
				{
					for( int n = 0; n < VertexBuffers.Count; n++ )
						if( VertexBuffers[ n ].Disposed )
							return true;
				}
				if( IndexBuffer != null && IndexBuffer.Disposed )
					return true;
				return false;
			}
		}

		public abstract void AddVertexIndexData( VertexIndexData data, ref Matrix4 transform, bool wireframe, bool culling );
		public void AddVertexIndexData( VertexIndexData data, Matrix4 transform, bool wireframe, bool culling )
		{
			AddVertexIndexData( data, ref transform, wireframe, culling );
		}

		//bool transformAlreadyAppliedRelativeCameraOffset
		public abstract void AddMesh( Component_Mesh.CompiledData meshData, ref Matrix4 transform, bool wireframe, bool culling );
		public void AddMesh( Component_Mesh.CompiledData meshData, Matrix4 transform, bool wireframe, bool culling )
		{
			AddMesh( meshData, ref transform, wireframe, culling );
		}

		/////////////////////////////////////////

		public abstract void AddRectangle( ref Rectangle rectangle, ref Matrix4 transform, bool solid = false, double lineThickness = 0 );

		public void AddRectangle( Rectangle rectangle, Matrix4 transform, bool solid = false, double lineThickness = 0 )
		{
			AddRectangle( ref rectangle, ref transform, solid, lineThickness );
		}

		public abstract void AddEllipse( ref Vector2 dimensions, int edges, ref Matrix4 transform, bool solid = false, double lineThickness = 0 );

		public void AddEllipse( Vector2 dimensions, int edges, Matrix4 transform, bool solid = false, double lineThickness = 0 )
		{
			AddEllipse( ref dimensions, edges, ref transform, solid, lineThickness );
		}
	}
}
