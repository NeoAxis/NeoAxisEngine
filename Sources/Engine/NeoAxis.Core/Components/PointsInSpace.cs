// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Point cloud in the scene.
	/// </summary>
	[SettingsCell( "NeoAxis.Editor.PointsInSpaceSettingsCell" )]
	public class PointsInSpace : MeshInSpace
	{
		bool needUpdateBounds = true;
		bool needUpdateMesh = true;

		///////////////////////////////////////////////

		/// <summary>
		/// The maximum number of points to manage by this component.
		/// </summary>
		[Category( "Data" )]
		[DefaultValue( 100000 )]
		public Reference<int> PointLimit
		{
			get { if( _pointLimit.BeginGet() ) PointLimit = _pointLimit.Get( this ); return _pointLimit.value; }
			set { if( _pointLimit.BeginSet( this, ref value ) ) { try { PointLimitChanged?.Invoke( this ); } finally { _pointLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PointLimit"/> property value changes.</summary>
		public event Action<PointsInSpace> PointLimitChanged;
		ReferenceField<int> _pointLimit = 100000;

		/// <summary>
		/// Source positions of points in space.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Data" )]
		public Reference<Vector3F[]> Positions
		{
			get { if( _positions.BeginGet() ) Positions = _positions.Get( this ); return _positions.value; }
			set { if( _positions.BeginSet( this, ref value ) ) { try { PositionsChanged?.Invoke( this ); NeedUpdate(); } finally { _positions.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Positions"/> property value changes.</summary>
		public event Action<PointsInSpace> PositionsChanged;
		ReferenceField<Vector3F[]> _positions = null;

		/// <summary>
		/// Source colors of points in space.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Data" )]
		public Reference<ColorByte[]> Colors
		{
			get { if( _colors.BeginGet() ) Colors = _colors.Get( this ); return _colors.value; }
			set { if( _colors.BeginSet( this, ref value ) ) { try { ColorsChanged?.Invoke( this ); NeedUpdate(); } finally { _colors.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Colors"/> property value changes.</summary>
		public event Action<PointsInSpace> ColorsChanged;
		ReferenceField<ColorByte[]> _colors = null;

		public enum PointShapeEnum
		{
			Sphere,
			Box,
			//Billboard
		}

		/// <summary>
		/// The geometry shape used to visualize each point in the cloud.
		/// </summary>
		[Category( "Visualization" )]
		[DefaultValue( PointShapeEnum.Sphere )]
		public Reference<PointShapeEnum> PointShape
		{
			get { if( _pointShape.BeginGet() ) PointShape = _pointShape.Get( this ); return _pointShape.value; }
			set { if( _pointShape.BeginSet( this, ref value ) ) { try { PointShapeChanged?.Invoke( this ); NeedUpdate(); } finally { _pointShape.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PointShape"/> property value changes.</summary>
		public event Action<PointsInSpace> PointShapeChanged;
		ReferenceField<PointShapeEnum> _pointShape = PointShapeEnum.Sphere;

		/// <summary>
		/// The number of segments used to create the geometry of each point when using a sphere shape.
		/// </summary>
		[Category( "Visualization" )]
		[Range( 3, 20, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DefaultValue( 5 )]
		public Reference<int> PointSegments
		{
			get { if( _pointSegments.BeginGet() ) PointSegments = _pointSegments.Get( this ); return _pointSegments.value; }
			set { if( _pointSegments.BeginSet( this, ref value ) ) { try { PointSegmentsChanged?.Invoke( this ); NeedUpdate(); } finally { _pointSegments.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PointSegments"/> property value changes.</summary>
		public event Action<PointsInSpace> PointSegmentsChanged;
		ReferenceField<int> _pointSegments = 5;

		/// <summary>
		/// The size of each point in the cloud, in world units. This value determines how large each point appears when rendered.
		/// </summary>
		[Category( "Visualization" )]
		[Range( 0.01, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DefaultValue( 0.05 )]
		public Reference<double> PointSize
		{
			get { if( _pointSize.BeginGet() ) PointSize = _pointSize.Get( this ); return _pointSize.value; }
			set { if( _pointSize.BeginSet( this, ref value ) ) { try { PointSizeChanged?.Invoke( this ); NeedUpdate(); } finally { _pointSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PointSize"/> property value changes.</summary>
		public event Action<PointsInSpace> PointSizeChanged;
		ReferenceField<double> _pointSize = 0.05;

		///////////////////////////////////////////////

		public class SourceVisualizationData
		{
			public Vector3F[] Positions;
			public ColorByte[] Colors;
		}

		///////////////////////////////////////////////

		public PointsInSpace()
		{
		}

		public delegate void GetSourceVisualizationDataEventDelegate( PointsInSpace sender, ref SourceVisualizationData data );
		public event GetSourceVisualizationDataEventDelegate GetSourceVisualizationDataEvent;

		public virtual SourceVisualizationData GetSourceVisualizationData()
		{
			var result = new SourceVisualizationData();

			GetSourceVisualizationDataEvent?.Invoke( this, ref result );

			if( result.Positions == null && result.Colors == null )
			{
				result.Positions = Positions.Value;
				if( Colors.Value != null && Colors.Value.Length > 0 )
					result.Colors = Colors.Value;
			}

			//check validity
			if( result.Positions != null && result.Colors != null && result.Positions.Length != result.Colors.Length )
			{
				result.Positions = null;
				result.Colors = null;
			}
			if( result.Positions != null && result.Positions.Length == 0 )
			{
				result.Positions = null;
				result.Colors = null;
			}

			return result;
		}

		public void NeedUpdate()
		{
			needUpdateBounds = true;
			needUpdateMesh = true;
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( PointSegments ):
					if( PointShape.Value == PointShapeEnum.Box )
						skip = true;
					break;
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( needUpdateBounds )
			{
				needUpdateBounds = false;
				SpaceBoundsUpdate();
			}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			if( needUpdateMesh )
			{
				needUpdateMesh = false;
				MeshUpdate();
			}

			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var positions = Positions.Value;
			if( positions != null && positions.Length > 0 )
			{
				var localBounds = BoundsF.Cleared;
				localBounds.Add( positions );

				var tr = TransformV;
				var bounds = Bounds.Cleared;
				foreach( var localPoint in localBounds.ToPoints() )
					bounds.Add( tr * localPoint );
				newBounds = new SpaceBounds( bounds );
			}
		}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			context.thisObjectWasChecked = true;

			if( SpaceBounds.BoundingBox.Intersects( context.ray, out var scale ) )
				context.thisObjectResultRayScale = scale;
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			if( Positions.Value == null )
				Positions = new BoundsF( -0.5f, -0.5f, -0.5f, 0.5f, 0.5f, 0.5f ).ToPoints();
		}
		
		public bool DataExists()
		{
			return Positions.Value != null && Positions.Value.Length != 0;
		}

		public void ClearData()
		{
			Positions = null;
			Colors = null;
			NeedUpdate();
		}

		//static void WriteFloat( byte[] data, int offset, float value )
		//{
		//	Unsafe.WriteUnaligned( ref data[ offset ], value );
		//}

		static void WriteVector3F( byte[] data, int offset, Vector3F value )
		{
			Unsafe.WriteUnaligned( ref data[ offset ], value );
		}

		static void WriteColorValue( byte[] data, int offset, ColorValue value )
		{
			Unsafe.WriteUnaligned( ref data[ offset ], value );
		}

		//static void WriteColorByte( byte[] data, int offset, ColorByte value )
		//{
		//	Unsafe.WriteUnaligned( ref data[ offset ], value );
		//}

		void MeshUpdate()
		{
			var mesh = GetComponent<Mesh>( "Mesh" );
			if( mesh == null )
			{
				mesh = CreateComponent<Mesh>( enabled: false );
				mesh.Name = "Mesh";
			}
			else
				mesh.Enabled = false;

			mesh.CalculateExtractedDataAndBounds = false;

			//mesh.Billboard = true;


			//!!!!maybe support without recreation when vertex amount is the same. Need to check vertex structure.


			mesh.RemoveAllComponents( false );

			var sourceData = GetSourceVisualizationData();
			if( sourceData.Positions != null )
			{
				var geometry = mesh.CreateComponent<MeshGeometry>();
				geometry.Name = "Mesh Geometry";

				var channels = StandardVertex.Components.Position;// | StandardVertex.Components.TexCoord0;
				channels |= StandardVertex.Components.Normal;

				var hasColors = sourceData.Colors != null;
				if( hasColors )
					channels |= StandardVertex.Components.Color;

				var vertexStructure = StandardVertex.MakeStructure( channels, true, out int vertexSize );

				Vector3F[] shapePositions = null;
				int[] shapeIndices = null;
				{
					switch( PointShape.Value )
					{
					case PointShapeEnum.Box:
						SimpleMeshGenerator.GenerateBox( new Vector3( PointSize, PointSize, PointSize ), out shapePositions, out shapeIndices );
						break;
					case PointShapeEnum.Sphere:
						{
							var segments = Math.Max( PointSegments.Value, 3 );
							SimpleMeshGenerator.GenerateSphere( PointSize / 2, segments, segments - 1, false, out shapePositions, out shapeIndices );
						}
						break;

						//Billboard
						//var vertexCount = pointCount * 4;
						//var indexCount = pointCount * 6;

					}
				}

				var pointCount = sourceData.Positions.Length;
				var vertexCount = pointCount * shapePositions.Length;
				var indexCount = pointCount * shapeIndices.Length;

				var vertices = new byte[ vertexCount * vertexSize ];
				var indices = new int[ indexCount ];

				vertexStructure.GetElementBySemantic( VertexElementSemantic.Normal, out var normalElement );
				var normalOffset = normalElement.Offset;
				vertexStructure.GetElementBySemantic( VertexElementSemantic.Color0, out var colorElement );
				var colorOffset = colorElement.Offset;

				var pointSize = PointSize.Value;
				var halfSize = (float)( pointSize * 0.5 );

				//var billboardSize = BillboardSize.Value;
				//var halfSizeX = (float)( billboardSize.X * 0.5 );
				//var halfSizeY = (float)( billboardSize.Y * 0.5 );

				void WriteVertex( int vertexIndex, Vector3F position, Vector3F normal, ColorByte color )
				{
					var vertexOffset = vertexIndex * vertexSize;

					WriteVector3F( vertices, vertexOffset, position );

					//if( hasNormals )
					{

						//!!!!? can pack normals

						WriteVector3F( vertices, vertexOffset + normalOffset, normal );
					}

					if( hasColors )
					{
						WriteColorValue( vertices, vertexOffset + colorOffset, color.ToColorValue() );

						//!!!!can use packed colors

						//WriteColorByte( vertices, vertexOffset + colorOffset, color );
					}

					//if( hasTexCoords0 )
					//{
					//	WriteFloat( vertices, vertexOffset + texCoord0Offset, u );
					//	WriteFloat( vertices, vertexOffset + texCoord0Offset + 4, v );
					//}
				}

				//Box, Sphere
				{
					for( int nPoint = 0; nPoint < pointCount; nPoint++ )
					{
						var center = sourceData.Positions[ nPoint ];
						var color = hasColors ? sourceData.Colors[ nPoint ] : default;

						var startVertex = nPoint * shapePositions.Length;

						for( int n = 0; n < shapePositions.Length; n++ )
						{
							var shapePosition = shapePositions[ n ];
							WriteVertex( startVertex + n, center + shapePosition, shapePosition.GetNormalize(), color );
						}

						var startIndex = nPoint * shapeIndices.Length;
						for( int n = 0; n < shapeIndices.Length; n++ )
							indices[ startIndex + n ] = startVertex + shapeIndices[ n ];
					}
				}


				////Billboard
				//{
				//	for( int n = 0; n < pointCount; n++ )
				//	{
				//		var center = sourceData.Positions[ n ];
				//		var color = hasColors ? sourceData.Colors[ n ] : default;

				//		var v = n * 4;
				//		WriteVertex( v + 0, center + new Vector3F( -halfSizeX, 0, -halfSizeY ), 0, 1, color );
				//		WriteVertex( v + 1, center + new Vector3F( halfSizeX, 0, -halfSizeY ), 1, 1, color );
				//		WriteVertex( v + 2, center + new Vector3F( halfSizeX, 0, halfSizeY ), 1, 0, color );
				//		WriteVertex( v + 3, center + new Vector3F( -halfSizeX, 0, halfSizeY ), 0, 0, color );

				//		//WriteVertex( v + 0, center + new Vector3F( -halfSizeX, -halfSizeY, 0 ), 0, 1, color );
				//		//WriteVertex( v + 1, center + new Vector3F( halfSizeX, -halfSizeY, 0 ), 1, 1, color );
				//		//WriteVertex( v + 2, center + new Vector3F( halfSizeX, halfSizeY, 0 ), 1, 0, color );
				//		//WriteVertex( v + 3, center + new Vector3F( -halfSizeX, halfSizeY, 0 ), 0, 0, color );

				//		var i = n * 6;
				//		indices[ i + 0 ] = v + 0;
				//		indices[ i + 1 ] = v + 1;
				//		indices[ i + 2 ] = v + 2;
				//		indices[ i + 3 ] = v + 0;
				//		indices[ i + 4 ] = v + 2;
				//		indices[ i + 5 ] = v + 3;
				//	}
				//}

				geometry.VertexStructure = vertexStructure;
				geometry.Vertices = vertices;
				geometry.Indices = indices;
			}
			else
			{
				var geometry = mesh.CreateComponent<MeshGeometry_Sphere>();
				geometry.Name = "Mesh Geometry";
				geometry.Radius = 0.1;
			}

			mesh.Enabled = true;

			Mesh = ReferenceUtility.MakeThisReference( this, mesh );
		}
	}


	//!!!!

	//struct Point
	//{
	//	public Vector3F Position;
	//	public ColorByte Color;
	//}

	///// <summary>
	///// Represents a single LiDAR point.
	///// Uses Structs instead of Classes to prevent GC pressure when handling millions of points.
	///// </summary>
	//public struct LidarPoint
	//{
	//	// === 1. Geometry & Shading ===

	//	/// <summary>
	//	/// The local or world coordinates of the point.
	//	/// </summary>
	//	public Vector3F Position { get; set; }

	//	/// <summary>
	//	/// The direction the point face is pointing (crucial for dynamic lighting and custom shaders).
	//	/// </summary>
	//	public Vector3F Normal { get; set; }


	//	// === 2. Visuals & Intensity ===

	//	/// <summary>
	//	/// Color data stored as standard 32-bit RGBA (1 byte per channel).
	//	/// </summary>
	//	public ColorByte Color { get; set; }

	//	/// <summary>
	//	/// Laser return intensity. Typically raw 0-65535 (ushort) from LAS files, 
	//	/// or normalized depending on your workflow.
	//	/// </summary>
	//	public ushort Intensity { get; set; }


	//	// === 3. LAS Standard Classification & Pulse Data ===

	//	/// <summary>
	//	/// ASPRS Standard Classification (e.g., 2 = Ground, 6 = Building, 9 = Water).
	//	/// </summary>
	//	public byte Classification { get; set; }

	//	/// <summary>
	//	/// The pulse return number for this specific point (usually 1 to 5).
	//	/// </summary>
	//	public byte ReturnNumber { get; set; }

	//	/// <summary>
	//	/// Total number of returns given by the same laser pulse.
	//	/// </summary>
	//	public byte NumberOfReturns { get; set; }

	//	/// <summary>
	//	/// The angle at which the laser shot was emitted (-90 to +90 degrees).
	//	/// </summary>
	//	public short ScanAngle { get; set; }


	//	// === 4. Metadata & Trajectory ===

	//	/// <summary>
	//	/// Precise GPS timestamp of the point acquisition.
	//	/// </summary>
	//	public double GpsTime { get; set; }

	//	/// <summary>
	//	/// ID indicating the specific flight line or scan pass.
	//	/// </summary>
	//	public ushort PointSourceId { get; set; }

	//	/// <summary>
	//	/// Indicates if the point is at the very edge of a scan flight line track.
	//	/// </summary>
	//	public bool EdgeOfFlightLine { get; set; }
	//}

}
