// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	public static class RoadUtility
	{
		public const bool UpdateVisualDataInModifyingMode = false;

		//

		public class VisualData
		{
			public ObjectInSpace Owner;
			public Scene Scene;

			public GroupOfObjects GroupOfObjects;
			public List<GroupOfObjects.SubGroup> GroupOfObjectSubGroups = new List<GroupOfObjects.SubGroup>();
			//public GroupOfObjects GroupOfObjects;
			//public OpenList<GroupOfObjects.Object> GroupOfObjectObjects2 = new OpenList<GroupOfObjects.Object>();
			//public GroupOfObjects.SubGroup GroupOfObjectObjectsSubGroup;
			////public GroupOfObjects.SubGroup GroupOfObjectsSubGroup;
			////public List<int> GroupOfObjectObjects = new List<int>();

			public List<MeshInSpace> MeshesInSpace = new List<MeshInSpace>();

			//!!!!
			public List<Line3> DebugLines = new List<Line3>();

			public List<Mesh> MeshesToDispose = new List<Mesh>();

			//

			public VisualData( ObjectInSpace owner )
			{
				Owner = owner;
				Scene = Owner.ParentScene;
			}

			public void Dispose()
			{
				if( GroupOfObjects != null && GroupOfObjectSubGroups.Count != 0 )
				{
					foreach( var subGroup in GroupOfObjectSubGroups )
						GroupOfObjects.RemoveSubGroupToQueue( subGroup );
				}
				GroupOfObjectSubGroups.Clear();

				//if( group != null && GroupOfObjectObjects.Count != 0 )
				//{
				//	//!!!!может обновление сразу вызывается, тогда не круто
				//	group.ObjectsRemove( GroupOfObjectObjects.ToArray() );

				//	//!!!!
				//	//group.BeginUpdate();
				//	//group.ObjectsRemove( visualData.groupOfObjectObjects.ArraySegment );
				//	//if( !group.ObjectsExists() )
				//	//	group.EndUpdate();

				//}
				//GroupOfObjectObjects.Clear();

				foreach( var c in MeshesInSpace )
					c.Dispose();
				MeshesInSpace.Clear();

				foreach( var mesh in MeshesToDispose )
					mesh.Dispose();
				MeshesToDispose.Clear();

				DebugLines.Clear();
			}

			GroupOfObjects GetOrCreateGroupOfObjects( bool canCreate )
			{
				var name = "__GroupOfObjectsRoads";

				var group = Scene.GetComponent<GroupOfObjects>( name );
				if( group == null && canCreate )
				{
					//need set ShowInEditor = false before AddComponent
					group = ComponentUtility.CreateComponent<GroupOfObjects>( null, false, false );
					group.DisplayInEditor = false;
					Scene.AddComponent( group, -1 );
					//var group = scene.CreateComponent<GroupOfObjects>();

					group.Name = name;
					//group.CanBeSelected = false;
					group.SaveSupport = false;
					group.CloneSupport = false;

					group.AnyData = new Dictionary<Mesh, int>();


					//!!!!
					//!!!!как в домах сделать общий RoadManager
					//group.SectorSize = new Vector3( 100, 100, 10000 );


					group.Enabled = true;
				}

				return group;
			}

			static ushort GetOrCreateGroupOfObjectsElement( GroupOfObjects group, Mesh mesh )
			{
				//группы объектов нельзя использовать для геометрии пайпа, потому что мешей много. они придуманы для множества объектов одного типа

				var dictionary = (Dictionary<Mesh, int>)group.AnyData;

				GroupOfObjectsElement_Mesh element = null;

				if( dictionary.TryGetValue( mesh, out var elementIndex2 ) )
					return (ushort)elementIndex2;

				//var elements = group.GetComponents<GroupOfObjectsElement_Mesh>();
				//foreach( var e in elements )
				//{
				//	if( e.Mesh.Value == mesh )
				//	{
				//		element = e;
				//		break;
				//	}
				//}

				if( element == null )
				{
					var elementIndex = group.GetFreeElementIndex();
					element = group.CreateComponent<GroupOfObjectsElement_Mesh>( enabled: false );
					element.Name = "Element " + elementIndex.ToString();
					element.Index = elementIndex;
					element.Mesh = mesh;
					element.AutoAlign = false;
					element.Enabled = true;

					dictionary[ mesh ] = element.Index;

					group.ElementTypesCacheNeedUpdate();
				}

				return (ushort)element.Index.Value;
			}

			public unsafe void CreateMeshObject( Mesh mesh, Transform transform/*, bool useGroupOfObjects, RenderingPipeline.RenderSceneData.CutVolumeItem[] cutVolumes*/, ColorValue color, MeshInSpace.AdditionalItem[] additionalItems = null, bool collision = false )
			{
				//if( useGroupOfObjects && cutVolumes == null )
				//{
				//	var group = GetOrCreateGroupOfObjects( true );
				//	if( group != null )
				//	{
				//		var elementIndex = GetOrCreateGroupOfObjectsElement( group, mesh );
				//		var pos = transform.Position;
				//		var rot = transform.Rotation.ToQuaternionF();
				//		var scl = transform.Scale.ToVector3F();

				//		var obj = new GroupOfObjects.Object( elementIndex, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, pos, rot, scl, Vector4F.Zero, color, Vector4F.Zero, Vector4F.Zero, 0 );

				//		GroupOfObjectObjects2.Add( ref obj );

				//		////!!!!может обновление сразу вызывается, тогда не круто
				//		//var objects = group.ObjectsAdd( &obj, 1 );
				//		//GroupOfObjectObjects.AddRange( objects );
				//	}
				//}
				//else
				//{

				//need set ShowInEditor = false before AddComponent
				var meshInSpace = ComponentUtility.CreateComponent<MeshInSpace>( null, false, false );
				//generator.MeshInSpace = meshInSpace;
				meshInSpace.DisplayInEditor = false;
				Owner.AddComponent( meshInSpace, -1 );
				//var meshInSpace = CreateComponent<MeshInSpace>();

				meshInSpace.Name = "__Mesh In Space";
				meshInSpace.CanBeSelected = false;
				meshInSpace.SaveSupport = false;
				meshInSpace.CloneSupport = false;
				meshInSpace.Transform = transform;
				meshInSpace.Color = color;
				//meshInSpace.CutVolumes = cutVolumes;
				meshInSpace.Mesh = mesh;// ReferenceUtility.MakeThisReference( meshInSpace, mesh );
				meshInSpace.StaticShadows = true;
				meshInSpace.AdditionalItems = additionalItems;
				meshInSpace.Collision = collision;
				meshInSpace.Enabled = true;

				MeshesInSpace.Add( meshInSpace );

				//}
			}

			public void CreateObjectsFromMeshData( MeshData meshData, Transform transform, ColorValue color )
			{
				var meshes = new Dictionary<Vector2I, Mesh>();

				var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

				foreach( var item in meshData.MeshGeometries )
				{
					var key = new Vector2I( item.CastShadows ? 1 : 0, 0 );// item.DepthSortingLevel );

					if( !meshes.TryGetValue( key, out var mesh ) )
					{
						mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );
						mesh.CastShadows = item.CastShadows;
						meshes[ key ] = mesh;
					}

					var meshGeometry = mesh.CreateComponent<MeshGeometry>();
					meshGeometry.VertexStructure = vertexStructure;
					meshGeometry.Vertices = item.Vertices;
					meshGeometry.Indices = item.Indices;
					meshGeometry.Material = item.Material;
				}

				//!!!!
				//!!!!where else
				//mesh.VisibilityDistanceFactor = RoadType.VisibilityDistanceFactor;

				foreach( var item in meshes )
				{
					//var depthSortingLevel = item.Key.Y;
					var mesh = item.Value;

					mesh.MergeGeometriesWithEqualVertexStructureAndMaterial();
					mesh.Enabled = true;
					MeshesToDispose.Add( mesh );

					CreateMeshObject( mesh, transform, color );
					//CreateMeshObject( mesh, transform, false, null, color );
					////var tr = new Transform( TransformV.Position );
					////CreateMeshObject( mesh, tr, false, null, ColorMultiplier );
				}


				//var meshNoShadows = ComponentUtility.CreateComponent<Mesh>( null, true, false );
				//meshNoShadows.CastShadows = false;

				//var meshWithShadows = ComponentUtility.CreateComponent<Mesh>( null, true, false );
				//meshNoShadows.CastShadows = true;

				//var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

				//foreach( var item in meshData.MeshGeometries )
				//{
				//	var mesh = item.CastShadows ? meshWithShadows : meshNoShadows;

				//	var meshGeometry = mesh.CreateComponent<MeshGeometry>();
				//	meshGeometry.VertexStructure = vertexStructure;
				//	meshGeometry.Vertices = item.Vertices;
				//	meshGeometry.Indices = item.Indices;
				//	meshGeometry.Material = item.Material;
				//}

				////!!!!
				////!!!!where else
				////mesh.VisibilityDistanceFactor = RoadType.VisibilityDistanceFactor;

				//for( int n = 0; n < 2; n++ )
				//{
				//	var mesh = n == 0 ? meshNoShadows : meshWithShadows;

				//	if( mesh.GetComponents<MeshGeometry>().Length != 0 )
				//	{
				//		mesh.MergeGeometriesWithEqualVertexStructureAndMaterial();
				//		mesh.Enabled = true;
				//		MeshesToDispose.Add( mesh );

				//		CreateMeshObject( mesh, transform, false, null, color, zzzz );
				//		//var tr = new Transform( TransformV.Position );
				//		//CreateMeshObject( mesh, tr, false, null, ColorMultiplier );
				//	}
				//	else
				//		mesh.Dispose();
				//}
			}

			public void CreateGroupOfObjectsSubGroup( Surface.CompiledSurfaceData surface, SurfaceObjectsObjectItem[] items )
			{
				var group = GetOrCreateGroupOfObjects( true );
				if( group != null )
				{
					GroupOfObjects = group;

					var list = new OpenList<GroupOfObjects.Object>( items.Length );

					var color = ColorValue.One;

					for( int n = 0; n < items.Length; n++ )
					{
						ref var item = ref items[ n ];
						if( item.Initialized == 0 )
							continue;

						surface.GetMesh( item.VariationGroup, item.VariationElement, out var enabled, out var mesh, out _, out _, out _, out _, out _ );
						if( !enabled )
							continue;

						var elementIndex = GetOrCreateGroupOfObjectsElement( group, mesh );

						var pos = item.Position;
						var rot = item.Rotation;
						var scl = item.Scale;

						var obj = new GroupOfObjects.Object( elementIndex, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, pos, rot, scl, Vector4F.Zero, color, Vector4F.Zero, Vector4F.Zero, 0 );

						list.Add( ref obj );
					}

					var subGroup = new GroupOfObjects.SubGroup();
					subGroup.Objects = list.ArraySegment;
					group.AddSubGroupToQueue( subGroup );

					GroupOfObjectSubGroups.Add( subGroup );
				}
			}
		}

		///////////////////////////////////////////////

		public static Internal.Net3dBool.Vector3[] Convert( IList<Vector3> source )
		{
			var result = new Internal.Net3dBool.Vector3[ source.Count ];
			for( int n = 0; n < result.Length; n++ )
			{
				var v = source[ n ];
				result[ n ] = new Internal.Net3dBool.Vector3( v.X, v.Y, v.Z );
			}
			return result;
		}

		public static Vector3[] Convert( Internal.Net3dBool.Vector3[] source )
		{
			var result = new Vector3[ source.Length ];
			for( int n = 0; n < result.Length; n++ )
			{
				var v = source[ n ];
				result[ n ] = new Vector3( v.x, v.y, v.z );
			}
			return result;
		}

		///////////////////////////////////////////////

		public class MeshData
		{
			public List<RoadGeometryGenerator.MeshGeometryItem> MeshGeometries = new List<RoadGeometryGenerator.MeshGeometryItem>();
		}

		///////////////////////////////////////////////

		//!!!!

		public class RoadGeometryGenerator
		{
			public ModeEnum Mode;
			public RoadType RoadType;
			public int Lanes;
			//public Point[] Points;
			public Curve PositionCurve;
			public Curve UpCurve;

			//public bool GenerateCollision;
			//public List<MeshGeometryItem> MeshGeometries = new List<MeshGeometryItem>();

			/////////////////////

			public enum ModeEnum
			{
				Road,
				Crossroad,
			}

			/////////////////////

			public enum GeneratePartEnum
			{
				Surface,
				Markup,

				ShoulderLeft,
				ShoulderRight,

				SidewalkLeft1,//border side
				SidewalkLeft2,//border top
				SidewalkLeft3,//surface
				SidewalkLeft4,//border top
				SidewalkLeft5,//border side

				SidewalkRight1,
				SidewalkRight2,
				SidewalkRight3,
				SidewalkRight4,
				SidewalkRight5,

				//OverpassTopLeft,
				//OverpassTopRight,
				//OverpassSideLeft,
				//OverpassSideRight,
				//OverpassBottom,
			}

			/////////////////////

			public class MeshGeometryItem
			{
				public GeneratePartEnum Part;
				public byte[] Vertices;
				public Vector3F[] Positions;
				public int[] Indices;
				public Reference<Material> Material;
				public bool CastShadows = true;
			}

			/////////////////////

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void GetPositionByTime( double time, out Vector3 result )
			{
				PositionCurve.CalculateValueByTime( time, out result );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void GetUpByTime( double time, out Vector3 result )
			{
				UpCurve.CalculateValueByTime( time, out result );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void GetDirectionByTime( double time, out Vector3 result )
			{
				var lastPointTime = PositionCurve.Points[ PositionCurve.Points.Count - 1 ].Time;

				//!!!!0.01

				if( time < 0.01 )
				{
					GetPositionByTime( 0.01, out var v1 );
					GetPositionByTime( 0, out var v2 );
					Vector3.Subtract( ref v1, ref v2, out result );
					//dir = GetPositionByTime( 0.01 ) - GetPositionByTime( 0 );
				}
				else if( time > lastPointTime - 0.01 )
				{
					GetPositionByTime( lastPointTime, out var v1 );
					GetPositionByTime( lastPointTime - 0.01, out var v2 );
					Vector3.Subtract( ref v1, ref v2, out result );
					//dir = GetPositionByTime( lastPointTime ) - GetPositionByTime( lastPointTime - 0.01 );
				}
				else
				{
					GetPositionByTime( time + 0.01, out var v1 );
					GetPositionByTime( time - 0.01, out var v2 );
					Vector3.Subtract( ref v1, ref v2, out result );
					//dir = GetPositionByTime( time + 0.01 ) - GetPositionByTime( time - 0.01 );
				}

				result.Normalize();


				//var lastPointTime = PositionCurve.Points[ PositionCurve.Points.Count - 1 ].Time;

				//Vector3 dir;

				////!!!!0.01

				//if( time < 0.01 )
				//	dir = GetPositionByTime( 0.01 ) - GetPositionByTime( 0 );
				//else if( time > lastPointTime - 0.01 )
				//	dir = GetPositionByTime( lastPointTime ) - GetPositionByTime( lastPointTime - 0.01 );
				//else
				//	dir = GetPositionByTime( time + 0.01 ) - GetPositionByTime( time - 0.01 );

				//return dir.GetNormalize();
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public void GenerateRoadMeshData( Vector3 ownerPosition, List<double> timeSteps, double totalLength, GeneratePartEnum generatePart, List<MeshGeometryItem> meshGeometries, double markupOffset = 0, bool markupRoadside = false, bool markupSolid = false )
			{
				var generatorData = this;

				var roadType = generatorData.RoadType;
				var laneWidth = roadType.LaneWidth.Value;
				var roadsideEdgeWidth = roadType.RoadsideEdgeWidth.Value;

				var shoulderWidth = roadType.ShoulderWidth.Value;

				var sidewalkWidth = roadType.SidewalkWidth.Value;
				var sidewalkHeight = roadType.SidewalkHeight.Value;
				var sidewalkBorderWidth = roadType.SidewalkBorderWidth.Value;

				//var ownerPosition = TransformV.Position;
				var segmentsLength = roadType.SegmentsLength.Value;
				if( segmentsLength <= 0.001 )
					segmentsLength = 0.001;


				var tiling = RoadType.SidewalkUVTilingEnum.AlongRoad;
				double uvTilesLength = 0;

				if( generatePart == GeneratePartEnum.Surface )
					uvTilesLength = roadType.UVTilesLength.Value;
				else if( generatePart == GeneratePartEnum.Markup )
					uvTilesLength = 1.0;//!!!!MarkupUVTilesLength

				else if( generatePart == GeneratePartEnum.ShoulderLeft )
					uvTilesLength = roadType.ShoulderUVTilesLength.Value;
				else if( generatePart == GeneratePartEnum.ShoulderRight )
					uvTilesLength = roadType.ShoulderUVTilesLength.Value;

				else if( generatePart == GeneratePartEnum.SidewalkLeft1 || generatePart == GeneratePartEnum.SidewalkLeft2 || generatePart == GeneratePartEnum.SidewalkLeft4 || generatePart == GeneratePartEnum.SidewalkLeft5 )
					uvTilesLength = roadType.SidewalkBorderUVTilesLength.Value;
				else if( generatePart == GeneratePartEnum.SidewalkLeft3 )
				{
					uvTilesLength = roadType.SidewalkUVTilesLength.Value;
					tiling = RoadType.SidewalkUVTilingEnum.HorizontalTiling;
				}

				else if( generatePart == GeneratePartEnum.SidewalkRight1 || generatePart == GeneratePartEnum.SidewalkRight2 || generatePart == GeneratePartEnum.SidewalkRight4 || generatePart == GeneratePartEnum.SidewalkRight5 )
					uvTilesLength = roadType.SidewalkBorderUVTilesLength.Value;
				else if( generatePart == GeneratePartEnum.SidewalkRight3 )
				{
					uvTilesLength = roadType.SidewalkUVTilesLength.Value;
					tiling = RoadType.SidewalkUVTilingEnum.HorizontalTiling;
				}

				else //if( generatePart == GeneratePartEnum.Overpass )
					uvTilesLength = 0;//!!!! roadType.OverpassUVTilesLength.Value;


				var segmentsWidth = 1;
				//if( generatePart == GeneratePartEnum.Overpass )
				//	segmentsWidth = 5;

				var widthSteps = segmentsWidth + 1;


				//process steps

				int lengthStepsAdded = 0;

				double currentLength = 0;

				//!!!!

				generatorData.GetPositionByTime( timeSteps[ 0 ], out var previousPosition );
				generatorData.GetPositionByTime( timeSteps[ 0 ] + 0.01, out var previousPosition2 );
				Vector3.Subtract( ref previousPosition2, ref previousPosition, out var previousVector );
				previousVector.Normalize();
				generatorData.GetUpByTime( timeSteps[ 0 ], out var currentUp );

				//var previousPosition = generatorData.GetPositionByTime( timeSteps[ 0 ] );
				//var previousVector = ( generatorData.GetPositionByTime( timeSteps[ 0 ] + 0.01 ) - previousPosition ).GetNormalize();
				//var currentUp = generatorData.GetUpByTime( timeSteps[ 0 ] );

				//var previousPosition = logicalData.GetPositionByTime( points[ 0 ].TimeOnCurve );
				//var previousVector = ( logicalData.GetPositionByTime( points[ 0 ].TimeOnCurve + 0.01 ) - previousPosition ).GetNormalize();
				//var currentUp = points[ 0 ].Transform.Rotation.GetUp();

				double lengthRemainder = 0;
				var previousUsedVector = previousVector;

				var approximateLengthSteps = (int)( totalLength / segmentsLength );
				var approximateVertexCount = approximateLengthSteps * widthSteps;

				//!!!!можно на нативных массивах, тогда обнулять не нужно. индексы тоже. только тогда точно с запасом массив выделять
				var positions = new List<Vector3F>( approximateVertexCount );
				var normals = new List<Vector3F>( approximateVertexCount );
				var tangents = new List<Vector4F>( approximateVertexCount );
				var texCoords0 = new List<Vector2F>( approximateVertexCount );
				//var texCoords1 = new List<Vector2F>( approximateVertexCount );


				for( int nTime = 0; nTime < timeSteps.Count; nTime++ )
				{
					var time = timeSteps[ nTime ];
					var firstStep = nTime == 0;
					var lastStep = nTime == timeSteps.Count - 1;

					//!!!!может быть поворот в несколько шагов с одинаковым position. тогда совсем иначе всё

					generatorData.GetPositionByTime( time, out var position );

					//!!!!
					double stepLength = 0;
					{
						var vector2 = position - previousPosition;
						if( vector2 != Vector3.Zero )
							stepLength = vector2.Normalize();
					}
					//var vector = position - previousPosition;
					//double stepLength = 0;
					//if( vector != Vector3.Zero )
					//	stepLength = vector.Normalize();
					//else
					//	vector = previousVector;

					generatorData.GetDirectionByTime( time, out var vector );
					generatorData.GetUpByTime( time, out var up );

					Quaternion.LookAt( ref vector, ref up, out var rotation );

					bool skip = false;
					if( !firstStep && !lastStep )
					{
						if( lengthRemainder + stepLength < segmentsLength )
							skip = true;
						//!!!!погрешность угла указывать?
						else if( MathAlgorithms.GetVectorsAngle( vector, previousUsedVector ).InDegrees() < 0.25 )
						{
							//if( Mode == ModeEnum.Road )//!!!!
							skip = true;
						}

						//!!!!про скейлинг
					}

					if( !skip )
					{
						var width = generatorData.Lanes * laneWidth + roadsideEdgeWidth * 2;
						if( Mode == ModeEnum.Crossroad )
							width = 0;

						for( int widthStep = 0; widthStep < widthSteps; widthStep++ )
						{
							//!!!!наклоны, перепады высоты. или уже т.к. LookAt

							double widthOffset = 0;
							if( generatePart == GeneratePartEnum.Surface )
								widthOffset = ( widthStep == 0 ? width : -width ) * 0.5;
							else if( generatePart == GeneratePartEnum.Markup )
							{
								var w = markupRoadside ? roadType.MarkupRoadsideWidth.Value : roadType.MarkupDividingLaneWidth.Value;
								widthOffset = ( widthStep == 0 ? w : -w ) * 0.5 + markupOffset;
							}

							else if( generatePart == GeneratePartEnum.ShoulderLeft )
								widthOffset = widthStep == 1 ? ( width * 0.5 ) : ( width * 0.5 + shoulderWidth );
							else if( generatePart == GeneratePartEnum.ShoulderRight )
								widthOffset = -( widthStep == 0 ? ( width * 0.5 ) : ( width * 0.5 + shoulderWidth ) );

							else if( generatePart == GeneratePartEnum.SidewalkLeft1 )
								widthOffset = width * 0.5;
							else if( generatePart == GeneratePartEnum.SidewalkLeft2 )
								widthOffset = widthStep == 0 ? ( width * 0.5 + sidewalkBorderWidth ) : ( width * 0.5 );
							else if( generatePart == GeneratePartEnum.SidewalkLeft3 )
								widthOffset = widthStep == 1 ? ( width * 0.5 + sidewalkBorderWidth ) : ( width * 0.5 + sidewalkBorderWidth + sidewalkWidth );
							else if( generatePart == GeneratePartEnum.SidewalkLeft4 )
								widthOffset = widthStep == 1 ? ( width * 0.5 + sidewalkBorderWidth + sidewalkWidth ) : ( width * 0.5 + sidewalkBorderWidth + sidewalkWidth + sidewalkBorderWidth );
							else if( generatePart == GeneratePartEnum.SidewalkLeft5 )
								widthOffset = width * 0.5 + sidewalkBorderWidth + sidewalkWidth + sidewalkBorderWidth;

							else if( generatePart == GeneratePartEnum.SidewalkRight1 )
								widthOffset = -( width * 0.5 );
							else if( generatePart == GeneratePartEnum.SidewalkRight2 )
								widthOffset = -( widthStep == 1 ? ( width * 0.5 + sidewalkBorderWidth ) : ( width * 0.5 ) );
							else if( generatePart == GeneratePartEnum.SidewalkRight3 )
								widthOffset = -( widthStep == 0 ? ( width * 0.5 + sidewalkBorderWidth ) : ( width * 0.5 + sidewalkBorderWidth + sidewalkWidth ) );
							else if( generatePart == GeneratePartEnum.SidewalkRight4 )
								widthOffset = -( widthStep == 0 ? ( width * 0.5 + sidewalkBorderWidth + sidewalkWidth ) : ( width * 0.5 + sidewalkBorderWidth + sidewalkWidth + sidewalkBorderWidth ) );
							else if( generatePart == GeneratePartEnum.SidewalkRight5 )
								widthOffset = -( width * 0.5 + sidewalkBorderWidth + sidewalkWidth + sidewalkBorderWidth );

							//!!!!
							//else if( generatePart == GeneratePartEnum.OverpassTopLeft )
							//	widthOffset = widthStep == 0 ? ( width * 0.5 + sidewalkWidthLeft + overpassWidth ) : ( width * 0.5 + sidewalkWidthLeft );
							//else if( generatePart == GeneratePartEnum.OverpassTopRight )
							//	widthOffset = -( widthStep == 0 ? ( width * 0.5 + sidewalkWidthRight ) : ( width * 0.5 + sidewalkWidthRight + overpassWidth ) );
							//else if( generatePart == GeneratePartEnum.OverpassSideLeft )
							//	widthOffset = width * 0.5 + sidewalkWidthLeft + overpassWidth;
							//else if( generatePart == GeneratePartEnum.OverpassSideRight )
							//	widthOffset = -( width * 0.5 + sidewalkWidthRight + overpassWidth );
							//else if( generatePart == GeneratePartEnum.OverpassBottom )
							//{
							//	widthOffset = width * 0.5 + sidewalkWidthRight + overpassWidth;
							//	if( widthStep == 0 )
							//		widthOffset = -widthOffset;
							//}


							////else //if( generatePart == GeneratePartEnum.Overpass )
							////{
							////	if( widthStep == 0 )
							////		widthOffset = width * 0.5 + sidewalkWidth;
							////	else if( widthStep == 1 || widthStep == 2 )
							////		widthOffset = width * 0.5 + sidewalkWidth + overpassWidth;
							////	else if( widthStep == 3 || widthStep == 4 )
							////		widthOffset = -( width * 0.5 + sidewalkWidth + overpassWidth );
							////	else if( widthStep == 5 )
							////		widthOffset = -( width * 0.5 + sidewalkWidth );
							////}

							double heightOffset = 0;

							if( generatePart == GeneratePartEnum.Markup )
							{
								//heightOffset = 0.03;
							}
							else if( generatePart == GeneratePartEnum.SidewalkLeft1 )
							{
								if( widthStep == 0 )
									heightOffset = sidewalkHeight;
							}
							else if( generatePart == GeneratePartEnum.SidewalkLeft2 || generatePart == GeneratePartEnum.SidewalkLeft3 || generatePart == GeneratePartEnum.SidewalkLeft4 )
							{
								heightOffset = sidewalkHeight;
							}
							else if( generatePart == GeneratePartEnum.SidewalkLeft5 )
							{
								if( widthStep == 1 )
									heightOffset = sidewalkHeight;

								//!!!!maybe add property
								//underground
								if( widthStep == 0 )
									heightOffset = -sidewalkHeight;
							}
							else if( generatePart == GeneratePartEnum.SidewalkRight1 )
							{
								if( widthStep == 1 )
									heightOffset = sidewalkHeight;
							}
							else if( generatePart == GeneratePartEnum.SidewalkRight2 || generatePart == GeneratePartEnum.SidewalkRight3 || generatePart == GeneratePartEnum.SidewalkRight4 )
							{
								heightOffset = sidewalkHeight;
							}
							else if( generatePart == GeneratePartEnum.SidewalkRight5 )
							{
								if( widthStep == 0 )
									heightOffset = sidewalkHeight;

								//!!!!maybe add property
								//underground
								if( widthStep == 1 )
									heightOffset = -sidewalkHeight;
							}
							//!!!!
							//else if( generatePart == GeneratePartEnum.OverpassSideLeft )
							//{
							//	if( widthStep == 0 )
							//		heightOffset = -overpassHeight;
							//}
							//else if( generatePart == GeneratePartEnum.OverpassSideRight )
							//{
							//	if( widthStep == 1 )
							//		heightOffset = -overpassHeight;
							//}
							//else if( generatePart == GeneratePartEnum.OverpassBottom )
							//	heightOffset = -overpassHeight;

							var offset = rotation * new Vector3( 0, widthOffset, heightOffset );
							var pos = position - ownerPosition + offset;

							positions.Add( pos.ToVector3F() );


							var normal = rotation.GetUp();

							if( generatePart == GeneratePartEnum.SidewalkLeft1 )
								normal = rotation.GetRight();
							if( generatePart == GeneratePartEnum.SidewalkLeft5 )
								normal = rotation.GetLeft();

							if( generatePart == GeneratePartEnum.SidewalkRight1 )
								normal = rotation.GetLeft();
							if( generatePart == GeneratePartEnum.SidewalkRight5 )
								normal = rotation.GetRight();

							//!!!!
							//if( generatePart == GeneratePartEnum.OverpassSideLeft )
							//	normal = rotation.GetLeft();
							//else if( generatePart == GeneratePartEnum.OverpassSideRight )
							//	normal = rotation.GetRight();
							//else if( generatePart == GeneratePartEnum.OverpassBottom )
							//	normal *= -1;

							normals.Add( normal.ToVector3F() );


							var tangent = new Vector4F( vector.ToVector3F(), -1 );

							if( tiling == RoadType.SidewalkUVTilingEnum.HorizontalTiling )
								tangent = new Vector4F( Vector3F.XAxis, -1 );

							tangents.Add( tangent );


							//!!!!double. относительное смещение. чтобы значения не были слишком большие?


							var texCoordX = widthOffset;

							if( generatePart == GeneratePartEnum.SidewalkLeft1 )
								texCoordX = heightOffset;
							else if( generatePart == GeneratePartEnum.SidewalkLeft2 || generatePart == GeneratePartEnum.SidewalkLeft4 )
								texCoordX = widthOffset;
							else if( generatePart == GeneratePartEnum.SidewalkLeft3 )
								texCoordX = widthOffset - width * 0.5 + sidewalkBorderWidth;
							if( generatePart == GeneratePartEnum.SidewalkLeft5 )
								texCoordX = -heightOffset;

							if( generatePart == GeneratePartEnum.SidewalkRight1 )
								texCoordX = heightOffset;
							else if( generatePart == GeneratePartEnum.SidewalkRight2 || generatePart == GeneratePartEnum.SidewalkRight4 )
								texCoordX = -widthOffset;
							else if( generatePart == GeneratePartEnum.SidewalkRight3 )
								texCoordX = -( widthOffset - width * 0.5 + sidewalkBorderWidth );
							if( generatePart == GeneratePartEnum.SidewalkRight5 )
								texCoordX = -heightOffset;

							//!!!!
							//else if( generatePart == GeneratePartEnum.OverpassSideLeft )
							//	texCoordX -= heightOffset;
							//else if( generatePart == GeneratePartEnum.OverpassSideRight )
							//	texCoordX += heightOffset;

							if( tiling == RoadType.SidewalkUVTilingEnum.HorizontalTiling )
							{
								//!!!!очень большое значение будет?

								var pos2 = position + offset;
								texCoords0.Add( new Vector2( pos2.X * uvTilesLength, pos2.Y * uvTilesLength ).ToVector2F() );
							}
							else
							{
								texCoords0.Add( new Vector2( texCoordX * uvTilesLength, currentLength * uvTilesLength ).ToVector2F() );
							}

							//texCoords1.Add( new Vector2F( (float)currentLength, widthStep == 0 ? 0 : 1 ) );
						}

						lengthStepsAdded++;
					}

					if( !skip )
						lengthRemainder = 0;
					else
						lengthRemainder += stepLength;

					currentLength += stepLength;
					previousPosition = position;
					previousVector = vector;
					if( !skip )
						previousUsedVector = vector;
					//!!!!может обновлять только когда юзается. или как-то по другому реже
					currentUp = rotation.GetUp();
				}


				//indices
				var lengthSegmentsAdded = lengthStepsAdded - 1;
				var indexCount = lengthSegmentsAdded * segmentsWidth * 6;
				var indices = new int[ indexCount ];
				int currentIndex = 0;

				for( int lengthStep = 0; lengthStep < lengthSegmentsAdded; lengthStep++ )
				{
					for( int widthStep = 0; widthStep < segmentsWidth; widthStep++ )
					{
						indices[ currentIndex++ ] = ( segmentsWidth + 1 ) * lengthStep + widthStep;
						indices[ currentIndex++ ] = ( segmentsWidth + 1 ) * lengthStep + widthStep + 1;
						indices[ currentIndex++ ] = ( segmentsWidth + 1 ) * ( lengthStep + 1 ) + widthStep + 1;
						indices[ currentIndex++ ] = ( segmentsWidth + 1 ) * ( lengthStep + 1 ) + widthStep + 1;
						indices[ currentIndex++ ] = ( segmentsWidth + 1 ) * ( lengthStep + 1 ) + widthStep;
						indices[ currentIndex++ ] = ( segmentsWidth + 1 ) * lengthStep + widthStep;
					}
				}

				if( currentIndex != indexCount )
					Log.Fatal( "Road: GenerateMeshData: currentIndex != indexCount." );
				foreach( var index in indices )
					if( index < 0 || index >= positions.Count )
						Log.Fatal( "Road: GenerateMeshData: index < 0 || index >= positions.Count." );

				var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );
				//var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.All, true, out int vertexSize );

				var vertices = new byte[ vertexSize * positions.Count ];
				unsafe
				{
					fixed( byte* pVertices = vertices )
					{
						StandardVertex.StaticOneTexCoord* pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;
						//StandardVertex* pVertex = (StandardVertex*)pVertices;

						for( int n = 0; n < positions.Count; n++ )
						{
							pVertex->Position = positions[ n ];
							pVertex->Normal = normals[ n ];
							pVertex->Tangent = tangents[ n ];
							pVertex->Color = new ColorValue( 1, 1, 1, 1 );
							pVertex->TexCoord0 = texCoords0[ n ];
							//pVertex->TexCoord1 = texCoords1[ n ];

							pVertex++;
						}
					}
				}

				//add to result
				if( vertices.Length != 0 && indices.Length != 0 )
				{
					var meshGeometryItem = new MeshGeometryItem();
					meshGeometryItem.Part = generatePart;
					meshGeometryItem.Vertices = vertices;
					meshGeometryItem.Positions = positions.ToArray();
					meshGeometryItem.Indices = indices;

					if( generatePart == GeneratePartEnum.Surface )
						meshGeometryItem.Material = roadType.SurfaceMaterial;
					else if( generatePart == GeneratePartEnum.Markup )
						meshGeometryItem.Material = markupSolid ? roadType.MarkupMaterial : roadType.MarkupDottedMaterial;

					else if( generatePart == GeneratePartEnum.ShoulderLeft )
						meshGeometryItem.Material = roadType.ShoulderMaterial;
					else if( generatePart == GeneratePartEnum.ShoulderRight )
						meshGeometryItem.Material = roadType.ShoulderMaterial;

					else if( generatePart == GeneratePartEnum.SidewalkLeft1 || generatePart == GeneratePartEnum.SidewalkLeft2 || generatePart == GeneratePartEnum.SidewalkLeft4 || generatePart == GeneratePartEnum.SidewalkLeft5 )
						meshGeometryItem.Material = roadType.SidewalkBorderMaterial;
					else if( generatePart == GeneratePartEnum.SidewalkLeft3 )
						meshGeometryItem.Material = roadType.SidewalkMaterial;

					else if( generatePart == GeneratePartEnum.SidewalkRight1 || generatePart == GeneratePartEnum.SidewalkRight2 || generatePart == GeneratePartEnum.SidewalkRight4 || generatePart == GeneratePartEnum.SidewalkRight5 )
						meshGeometryItem.Material = roadType.SidewalkBorderMaterial;
					else if( generatePart == GeneratePartEnum.SidewalkRight3 )
						meshGeometryItem.Material = roadType.SidewalkMaterial;

					else //if( generatePart == GeneratePartEnum.Overpass)
						meshGeometryItem.Material = null;//!!!! roadType.OverpassMaterial;

					if( generatePart == GeneratePartEnum.Markup )
						meshGeometryItem.CastShadows = false;

					meshGeometries.Add( meshGeometryItem );
				}
			}

			//!!!!
			//[MethodImpl( (MethodImplOptions)512 )]
			//public void GenerateCrossingMarkingsMeshData( Vector3 ownerPosition, List<MeshGeometryItem> meshGeometries )
			//{
			//}
		}

		///////////////////////////////////////////////

		//!!!!

		public struct SurfaceObjectsObjectItem
		{
			//!!!!
			public int Initialized;

			public int VariationGroup;
			public int VariationElement;
			public Vector3 Position;
			public QuaternionF Rotation;
			public Vector3F Scale;
			//public ColorValue Color;
		}

		public static SurfaceObjectsObjectItem[] CalculateSurfaceObjects( Surface.CompiledSurfaceData surface, /*MeshData meshData, */Vector3 ownerPosition, double age, RoadGeometryGenerator.MeshGeometryItem geometry )
		{
			//var geometry = meshData.MeshGeometries.FirstOrDefault( i => i.Part == RoadGeometryGenerator.GeneratePartEnum.Surface );
			//if( geometry == null )
			//	return null;

			SurfaceObjectsObjectItem[] data = null;

			var boundsLocal = BoundsF.Cleared;
			foreach( var p in geometry.Positions )
				boundsLocal.Add( p );
			var bounds = boundsLocal.ToBounds() + ownerPosition;

			var maskValue = (int)( age * 255 );

			using( var meshTest = new MeshTest( geometry.Positions, geometry.Indices ) )
			{
				//!!!!
				float objectsDistribution = 1;
				float objectsScale = 1;
				//surfaceObjectsItem.GetObjectsColor( out var objectsColor );
				//var objectsDistribution = surfaceObjectsItem.GetObjectsDistribution();
				//var objectsScale = surfaceObjectsItem.GetObjectsScale();

				//var tile = surfaceObjectsItem.Owner;
				//var owner = tile.owner;

				//OpenList<SurfaceObjectsObjectItem> data = null;
				//var data = new OpenList<SurfaceObjectsObjectItem>( 1024 );// count );



				var fillPattern = surface.FillPattern;
				if( fillPattern != null )
				{
					var fillPatternSize = fillPattern.Size * objectsDistribution;
					if( fillPatternSize.X == 0.0 )
						fillPatternSize.X = 0.01;
					if( fillPatternSize.Y == 0.0 )
						fillPatternSize.Y = 0.01;

					var groups = surface.Groups;

					//if( groupOfObjects == null )
					//	groupOfObjects = owner.GetOrCreateGroupOfObjects( true );
					//var element = GetOrCreateElement( groupOfObjects, surface );

					//!!!!может есть общий множитель для всех слоев
					//var color = surfaceObjectsItem.Layer != null ? surfaceObjectsItem.Layer.Value.SurfaceObjectsColor : owner.SurfaceObjectsColor.Value;
					//var color = owner.SurfaceObjectsColor.Value;
					//if( layer != null )
					//	color *= layer.Value.SurfaceObjectsColor;


					//var bounds = tile.Bounds.CalculatedBoundingBox;
					var bounds2 = bounds.ToRectangle();

					var min = bounds.Minimum.ToVector2();
					for( int n = 0; n < 2; n++ )
					{
						min[ n ] /= fillPatternSize[ n ];
						min[ n ] = Math.Floor( min[ n ] );
						min[ n ] *= fillPatternSize[ n ];
					}

					var max = bounds.Maximum.ToVector2();
					for( int n = 0; n < 2; n++ )
					{
						max[ n ] /= fillPatternSize[ n ];
						max[ n ] = Math.Ceiling( max[ n ] );
						max[ n ] *= fillPatternSize[ n ];
					}


					int count = 0;
					for( var y = min.Y; y < max.Y - fillPatternSize.Y * 0.5; y += fillPatternSize.Y )
						for( var x = min.X; x < max.X - fillPatternSize.X * 0.5; x += fillPatternSize.X )
							count += fillPattern.Objects.Count;

					data = new SurfaceObjectsObjectItem[ count ];
					//data = new OpenList<SurfaceObjectsObjectItem>( count );

					var objectsToCreate = new (Vector2 positionXY, int groupIndex)[ count ];

					int counter = 0;
					for( var y = min.Y; y < max.Y - fillPatternSize.Y * 0.5; y += fillPatternSize.Y )
					{
						for( var x = min.X; x < max.X - fillPatternSize.X * 0.5; x += fillPatternSize.X )
						{
							for( int nObjectItem = 0; nObjectItem < fillPattern.Objects.Count; nObjectItem++ )
							{
								var objectItem = fillPattern.Objects[ nObjectItem ];

								var positionXY = new Vector2( x, y ) + objectItem.Position * objectsDistribution;
								var groupIndex = objectItem.Group;

								var regularAlignment = groups[ groupIndex ].RegularAlignment;
								if( regularAlignment != 0 )
								{
									positionXY /= regularAlignment;
									positionXY = new Vector2( (int)positionXY.X, (int)positionXY.Y );
									positionXY *= regularAlignment;
								}

								objectsToCreate[ counter++ ] = (positionXY, groupIndex);
							}
						}
					}

					//var touchGroups = surface.Groups;

					//calculate item.ObjectsMaxSize
					float checkRadius;
					{
						var maxRadius = 0.0;
						foreach( var mesh in surface.GetAllMeshes() )
						{
							if( mesh.Result != null )
								maxRadius = Math.Max( maxRadius, mesh.Result.SpaceBounds.BoundingSphere.Radius );
						}
						checkRadius = (float)( maxRadius * surface.GetMaxScale() );// * item.GetObjectsScale()

						//!!!!
						checkRadius *= 0.5f;
					}

					Parallel.For( 0, counter, delegate ( int nObjectItem )
					{
						ref var item = ref objectsToCreate[ nObjectItem ];
						ref var positionXY = ref item.positionXY;
						var groupIndex = (byte)item.groupIndex;

						if( bounds2.Contains( ref positionXY ) )
						{
							var ray = new Ray( new Vector3( positionXY, bounds.Maximum.Z + 1 ), new Vector3( 0, 0, -( bounds.GetSize().Z + 2 ) ) );
							var rayLocal = new RayF( ( ray.Origin - ownerPosition ).ToVector3F(), ray.Direction.ToVector3F() );

							var rayResults = meshTest.RayCast( rayLocal, MeshTest.Mode.OneClosest, false );
							if( rayResults.Length != 0 )
							{
								//also check by radius
								bool allow = true;
								{
									var steps = 4;
									for( int nStep = 0; nStep < steps; nStep++ )
									{
										var angle = (float)nStep / steps * MathEx.PI * 2;
										var offset = new Vector3F( MathEx.Cos( angle ) * checkRadius, MathEx.Sin( angle ) * checkRadius, 0 );

										var rayLocal2 = rayLocal;
										rayLocal2.Origin += offset;
										if( meshTest.RayCast( rayLocal2, MeshTest.Mode.OneClosest, false ).Length == 0 )
										{
											allow = false;
											break;
										}
									}
								}

								if( allow )
								{
									var random = new FastRandom( unchecked(nObjectItem * 77 + (int)( positionXY.X * 12422.7 + positionXY.Y * 1234.2 )), true );
									//works bad
									//var random = new FastRandom( unchecked(nObjectItem * 12 + (int)( positionXY.X * 11.7 + positionXY.Y * 13.2 )) );

									ref var rayResult = ref rayResults[ 0 ];
									var resultPosition = ray.GetPointOnRay( rayResult.Scale );
									var surfaceNormal = rayResult.Normal;

									var options = new Surface.GetRandomVariationOptions( groupIndex, surfaceNormal );
									surface.GetRandomVariation( options, random, out _, out var elementIndex, out var positionZ, out var rotation, out var scale );

									if( maskValue != 0 && ( maskValue == 255 || random.Next( 255 ) <= maskValue ) )
									{
										ref var obj = ref data[ nObjectItem ];
										obj.Initialized = 1;
										obj.VariationGroup = groupIndex;
										obj.VariationElement = elementIndex;
										obj.Position = new Vector3( positionXY, resultPosition.Z + positionZ );
										obj.Rotation = rotation;
										obj.Scale = scale * objectsScale;
										//obj.Color = objectsColor;
									}
								}
							}
						}
					} );
				}
			}

			return data;
		}


	}
}
