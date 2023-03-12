// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	static class BuilderCommonFunctions
	{
		public static (int lowVertex, int highVertex) OrderVertices( int vertex1, int vertex2 ) => vertex1 < vertex2 ? (vertex1, vertex2) : (vertex2, vertex1);

		public static void CalculateNormal( BuilderMeshData.FaceVertex p0, BuilderMeshData.FaceVertex p1, BuilderMeshData.FaceVertex p2 )
		{
			var pos0 = p0.RawVertex.Position;
			var normal = Vector3F.Cross( p1.RawVertex.Position - pos0, p2.RawVertex.Position - pos0 ).GetNormalize();
			p0.RawVertex.Normal = p1.RawVertex.Normal = p2.RawVertex.Normal = normal;
		}

		public static Vector3F CalculateNormal( Vector3F p0, Vector3F p1, Vector3F p2 )
		{
			return Vector3F.Cross( p1 - p0, p2 - p0 ).GetNormalize();
		}


		//The normal is recalculated. But the tangent is only rotated with a triangle plane, so oldNormal is needed. 
		//ToDo : Есть проблемы с этим алгоритмом. После нескольких перетаскиваний вертексов, и возврата на место. Tangents повернуты в плоскости треугольинка (не такие как были). Возможно, так просто не получится. Не учитываются смещения в плоскости треугольинка.
		public static void CalculateTriangleTangentsAndNormalByOldNormalRotation( BuilderMeshData.FaceVertex v0, BuilderMeshData.FaceVertex v1, BuilderMeshData.FaceVertex v2, Vector3F oldNormal )
		{
			var newNormal = CalculateNormal( v0.RawVertex.Position, v1.RawVertex.Position, v2.RawVertex.Position );
			var rotationMatrix = GetRotationMatrix( oldNormal, newNormal );
			v0.RawVertex.Normal = newNormal;
			v1.RawVertex.Normal = newNormal;
			v2.RawVertex.Normal = newNormal;

			if( rotationMatrix == null )
				return;

			v0.RawVertex.Tangent = Transform( v0.RawVertex.Tangent );
			v1.RawVertex.Tangent = Transform( v1.RawVertex.Tangent );
			v2.RawVertex.Tangent = Transform( v2.RawVertex.Tangent );

			Vector4F Transform( Vector4F t )
			{
				var res = rotationMatrix.Value * t.ToVector3F();
				return new Vector4F( res.X, res.Y, res.Z, t.W );
			}
		}

		//При вычислении tangent учитываются tangents у соседних треугольников (корректируются с учетом поворота треугольников).
		//Т.к. соседних треугольников несколько, то для каждой вершины получается несколько возможных tangent. Выбирается комбинация в которой различия tangent в 3 вершинах минимальны.
		//Считается что треугольник v0,v1,v2 еще не добавлен.
		//Алгоритм нечеткий.
		//
		internal static void CalculateTriangleTangentsAndNormalByAdjacent( BuilderMeshData meshData, BuilderMeshData.FaceVertex v0, BuilderMeshData.FaceVertex v1, BuilderMeshData.FaceVertex v2 )
		{
			CalculateNormal( v0, v1, v2 );
			Vector3F normal = v0.RawVertex.Normal;

			//ToDo : Исключать из поиска текущий треугольник, если он уже добавлен. Хотя для BridgeEdges он еще не добавлен. Если использовать для коррекции tangent после MoveVertex, то уже добавлен.

			var triangles0 = GetTrianglesByVertex( meshData, v0.Vertex );
			var triangles1 = GetTrianglesByVertex( meshData, v1.Vertex );
			var triangles2 = GetTrianglesByVertex( meshData, v2.Vertex );

			//ToDo !!! Если нет соседних треугольников, например только один треугольник в меше. Задавать какой-то дефолтный tangent ?

			var tangC0 = GetTangentCandidates( triangles0, normal );
			var tangC1 = GetTangentCandidates( triangles1, normal );
			var tangC2 = GetTangentCandidates( triangles2, normal );

			// find the closest tingents among the canditates tang0,tang1,tang2.

			int fi0 = -1;
			int fi1 = -1;
			int fi2 = -1;
			float maxVal = Single.MinValue;
			for( int i0 = 0; i0 < tangC0.Count; i0++ )
				for( int i1 = 0; i1 < tangC1.Count; i1++ )
					for( int i2 = 0; i2 < tangC2.Count; i2++ )
					{
						//ToDo ??? Как учитывать Tangent.W ?
						float val = Vector3F.Dot( tangC0[ i0 ].tangent.ToVector3F(), tangC1[ i1 ].tangent.ToVector3F() );
						val += Vector3F.Dot( tangC0[ i0 ].tangent.ToVector3F(), tangC2[ i2 ].tangent.ToVector3F() );
						val += Vector3F.Dot( tangC1[ i1 ].tangent.ToVector3F(), tangC2[ i2 ].tangent.ToVector3F() );

						//weigh больше у тех tangent у которых normal треугольника мало отличается от normal текущего треугольника. Предпочтительнее брать из них.
						val += tangC0[ i0 ].weigh * tangC1[ i1 ].weigh / 2;
						val += tangC0[ i0 ].weigh * tangC2[ i2 ].weigh / 2;
						val += tangC1[ i1 ].weigh * tangC2[ i2 ].weigh / 2;

						//? Или так
						//val += tangC0[ i0 ].weigh / 2;
						//val += tangC1[ i1 ].weigh / 2;
						//val += tangC2[ i2 ].weigh / 2;


						if( maxVal < val )
						{
							maxVal = val;
							fi0 = i0;
							fi1 = i1;
							fi2 = i2;
						}
					}

			if( fi0 == -1 )
			{
				//ToDo ??? Warning, Log ?
				return;
			}

			var tangent0 = tangC0[ fi0 ].tangent;
			var tangent1 = tangC1[ fi1 ].tangent;
			var tangent2 = tangC2[ fi2 ].tangent;

			//------------

			//??? Дополнительная коррекция. Tangent который больше отличается от остальных заменяется на первый из 2 остальных. Но может быть не всегда она нужна?
			float val01 = Vector3F.Dot( tangent0.ToVector3F(), tangent1.ToVector3F() );
			float val02 = Vector3F.Dot( tangent0.ToVector3F(), tangent2.ToVector3F() );
			float val12 = Vector3F.Dot( tangent1.ToVector3F(), tangent2.ToVector3F() );
			if( val02 <= val01 && val12 <= val01 )
				tangent2 = tangent0;
			else if( val01 <= val02 && val12 <= val02 )
				tangent1 = tangent0;
			else if( val01 <= val12 && val02 <= val12 )
				tangent0 = tangent1;

			//------------

			v0.RawVertex.Tangent = tangent0;
			v1.RawVertex.Tangent = tangent1;
			v2.RawVertex.Tangent = tangent2;

			//-------------------
			List<(Vector4F tangent, float weigh)> GetTangentCandidates( List<BuilderMeshData.FaceVertex> triangles, Vector3F currentTriangleNormal )
			{
				var ret = new List<(Vector4F, float weigh)>();
				for( int i = 0; i < triangles.Count; i += 3 )
				{
					var n = BuilderCommonFunctions.CalculateNormal( triangles[ i ].RawVertex.Position, triangles[ i + 1 ].RawVertex.Position, triangles[ i + 2 ].RawVertex.Position );
					var t = triangles[ i ].RawVertex.Tangent;
					ret.Add( (
						new Vector4F( BuilderCommonFunctions.Rotate( n, currentTriangleNormal, t.ToVector3F() ), t.W ),
						( 1 + Vector3F.Dot( n, currentTriangleNormal ) ) / 2
					) );
				}
				return ret;
			}
		}

		//в треугольниках точки переупорядочены, так что на первом месте идет точка для vertexIndex, но не нарушен порядок следования точек против часовой стрелки.

		public static List<BuilderMeshData.FaceVertex> GetTrianglesByVertex( BuilderMeshData meshData, int vertexIndex )
		{
			var ret = new List<BuilderMeshData.FaceVertex>();
			for( int i = 0; i < meshData.Faces.Count; i++ )
			{
				var f = meshData.Faces[ i ];
				for( int j = 0; j < f.Triangles.Count; j++ )
				{
					if( f.Triangles[ j ].Vertex == vertexIndex )
					{
						ret.Add( f.Triangles[ j ] );

						int start = j - j % 3;
						if( j == start )
						{
							ret.Add( f.Triangles[ start + 1 ] );
							ret.Add( f.Triangles[ start + 2 ] );
						}
						else if( j == start + 1 )
						{
							ret.Add( f.Triangles[ start + 2 ] );
							ret.Add( f.Triangles[ start ] );
						}
						else if( j == start + 2 )
						{
							ret.Add( f.Triangles[ start ] );
							ret.Add( f.Triangles[ start + 1 ] );
						}
					}
				}
			}
			return ret;
		}


		//rotates vectorToRotate by the same angle as between fromVector and toVector
		public static Vector3F Rotate( Vector3F fromVector, Vector3F toVector, Vector3F vectorToRotate )
		{
			var mat = GetRotationMatrix( fromVector, toVector );
			if( mat == null )
				return vectorToRotate;
			return mat.Value * vectorToRotate;
		}

		//ToDo : Может вращение вокруг оси проще через Quaternion?
		//Get rotation matrix so that:  ReturnValue*fromVector == toVector
		internal/*obfuscator*/ static Matrix3F? GetRotationMatrix( Vector3F fromVector, Vector3F toVector )
		{
			const double epsilon = 1e-5f;

			fromVector = fromVector.GetNormalize();
			toVector = toVector.GetNormalize();
			var axis = Vector3.Cross( fromVector, toVector );
			if( axis.Length() < epsilon )
				return null;
			axis = axis.GetNormalize();
			double angle = Math.Acos( Vector3.Dot( fromVector, toVector ) );
			return FromAngleAxis( (float)angle, axis.ToVector3F() );
		}

		/// <summary>
		/// Creates a rotation matrix for a rotation about an arbitrary axis.
		/// </summary>
		/// <param name="radians">Rotation angle, in radians</param>
		/// <param name="axis">Rotation axis, which should be a normalized vector.</param>
		/// <returns>The rotation matrix</returns>
		static Matrix3F FromAngleAxis( float radians, Vector3F axis )
		{
			float x = axis.X;
			float y = axis.Y;
			float z = axis.Z;

			float sin = (float)Math.Sin( (double)radians );
			float cos = (float)Math.Cos( (double)radians );

			float xx = x * x;
			float yy = y * y;
			float zz = z * z;
			float xy = x * y;
			float xz = x * z;
			float yz = y * z;

			Matrix3F m = new Matrix3F(
			xx + ( cos * ( 1.0f - xx ) ),
			( xy - ( cos * xy ) ) + ( sin * z ),
			( xz - ( cos * xz ) ) - ( sin * y ),

			( xy - ( cos * xy ) ) - ( sin * z ),
			yy + ( cos * ( 1.0f - yy ) ),
			( yz - ( cos * yz ) ) + ( sin * x ),

			( xz - ( cos * xz ) ) + ( sin * y ),
			( yz - ( cos * yz ) ) - ( sin * x ),
			zz + ( cos * ( 1.0f - zz ) )
				);

			return m;
		}

		static bool ExistsMeshStructure( Mesh mesh )
		{
			if( mesh.Structure != null )
				return true;

			var meshGeometries = mesh.GetComponents<MeshGeometry>();
			if( meshGeometries.Length == 0 )
				return false;

			foreach( var meshGeometry in meshGeometries )
			{
				if( meshGeometry is MeshGeometry_Procedural meshGeometryProcedural )
				{
					if( !meshGeometryProcedural.ExistsMeshStructure() )
						return false;

					//VertexElement[] vertexStructure = null;
					//byte[] vertices = null;
					//int[] indices = null;
					//Material material = null;
					//Mesh.StructureClass structure = null;
					//meshGeometryProcedural.GetProceduralGeneratedData( ref vertexStructure, ref vertices, ref indices, ref material, ref structure );

					//if( structure == null )
					//	return false;
				}
				else
					return false;
			}

			return true;
		}

		//ToDo ? Проверять перестановки VertexElement местами? Может уже где-то есть такая функция?

		public static bool IsSameVertexStructure( Mesh.ExtractedData meshExtractedStructure, List<int> geometriesInSelection )
		{
			if( geometriesInSelection.Count == 1 )
				return true;

			var first = meshExtractedStructure.MeshGeometries[ geometriesInSelection[ 0 ] ];
			for( int i = 1; i < geometriesInSelection.Count; i++ )
			{
				var g = meshExtractedStructure.MeshGeometries[ geometriesInSelection[ i ] ];
				if( !IsSameVertexStructure( first.VertexStructure, g.VertexStructure ) )
					return false;
			}
			return true;
		}

		public static bool IsSameVertexStructure( VertexElement[] a1, VertexElement[] a2 )
		{
			if( a1 == a2 )
				return true;
			if( a1.Length != a2.Length )
				return false;
			for( int i = 0; i < a1.Length; i++ )
				if( a1[ i ] != a2[ i ] )
					return false;
			return true;
		}

		public static string GetUniqueFriendlyName( Component component )
		{
			var defaultName = component.BaseType.GetUserFriendlyNameForInstance();
			if( component.Parent.GetComponent( defaultName ) == null )
				return defaultName;
			return component.Parent.Components.GetUniqueName( defaultName, false, 2 );
		}

		//make the name unique for a component which is already named and added to parent.
		public static void EnsureNameIsUnique( Component component )
		{
			var name = component.Name;
			if( component.Parent.GetComponents().Count( _ => _.Name == name ) <= 1 )
				return;
			component.Name = component.Parent.Components.GetUniqueName( name, true, 2 );
		}

		//public static bool CheckValidMesh( Mesh mesh, out string error )
		//{
		//	if( !ExistsMeshStructure( mesh ) )
		//	{
		//		error = "Mesh structure does not exist. You can build the structure in the mesh settings or in the Mesh Editor.";
		//		return false;
		//	}

		//	//проверить валидны ли данные? нет ли например неправильных индексов. что еще

		//	error = "";
		//	return true;
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="document"></param>
		/// <param name="meshInSpace"></param>
		/// <param name="mesh"></param>
		/// <param name="undoForMesh"></param>
		/// <param name="needUndoForNextActions"></param>
		/// <returns>Returns True when the external mesh was copied.</returns>
		public static void CopyExternalMesh( DocumentInstance document, MeshInSpace meshInSpace, ref Mesh mesh, List<UndoSystem.Action> undoForMesh, ref bool undoLocked )
		{
			if( mesh.Parent != meshInSpace )
			{
				//create a new mesh
				var newMesh = (Mesh)mesh.Clone();
				newMesh.Name = "Mesh";

				//fix references
				{
					foreach( var geometry in newMesh.GetComponents<MeshGeometry>() )
					{
						var reference = geometry.Material.GetByReference;
						if( !string.IsNullOrEmpty( reference ) )
						{
							ReferenceUtility.ParseReference( reference, out var type, out var content );
							if( type == "root" )
							{
								var resourceName = ComponentUtility.GetOwnedFileNameOfComponent( mesh );
								if( !string.IsNullOrEmpty( resourceName ) )
								{
									var newReference = resourceName + "|" + content;
									geometry.Material = ReferenceUtility.MakeReference( newReference );
								}
							}
						}
					}
				}

				meshInSpace.AddComponent( newMesh );

				if( !undoLocked )
					undoForMesh.Add( new UndoActionComponentCreateDelete( document, new Component[] { newMesh }, true ) );

				//change Mesh property in MeshInSpace
				if( !undoLocked )
				{
					var property = (Metadata.Property)meshInSpace.MetadataGetMemberBySignature( "property:" + nameof( MeshInSpace.Mesh ) );
					undoForMesh.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( meshInSpace, property, meshInSpace.Mesh ) ) );
				}
				meshInSpace.Mesh = ReferenceUtility.MakeThisReference( meshInSpace, newMesh );

				mesh = newMesh;

				undoLocked = true;
			}
		}

		public static void ConvertProceduralMeshGeometries( DocumentInstance document, Mesh mesh, List<UndoSystem.Action> undoMeshChanges, ref bool undoMeshChangesLocked )
		{
			var meshGeometries = mesh.GetComponents<MeshGeometry>();
			if( meshGeometries.Any( geometry => geometry is MeshGeometry_Procedural ) )
			{
				var allProcedural = meshGeometries.All( geometry => geometry is MeshGeometry_Procedural );

				var oldList = meshGeometries;
				var oldStructure = mesh.Structure;



				var newList = new List<MeshGeometry>();
				mesh.Structure = null;

				for( int i = 0; i < meshGeometries.Length; i++ )
				{
					var meshGeometry = meshGeometries[ i ];

					//convert to usual MeshGeometry
					if( meshGeometry is MeshGeometry_Procedural meshGeometryProcedural )
					{
						VertexElement[] vertexStructure = null;
						byte[] vertices = null;
						int[] indices = null;
						Material material = null;
						byte[] voxelData = null;
						byte[] clusterData = null;
						Mesh.StructureClass structure = null;
						meshGeometryProcedural.GetProceduralGeneratedData( ref vertexStructure, ref vertices, ref indices, ref material, ref voxelData, ref clusterData, ref structure );

						var newMeshGeometry = mesh.CreateComponent<MeshGeometry>();
						newMeshGeometry.Name = meshGeometry.Name;
						newMeshGeometry.VertexStructure = vertexStructure;
						newMeshGeometry.Vertices = vertices;
						newMeshGeometry.Indices = indices;
						newMeshGeometry.Material = meshGeometry.Material;

						//!!!!clusters, voxels?

						//concat structures
						if( allProcedural )
							mesh.Structure = Mesh.StructureClass.Concat( mesh.Structure, structure, i );

						newList.Add( newMeshGeometry );
					}
					else
					{
						var newMeshGeometry = mesh.CreateComponent<MeshGeometry>();
						//var newMeshGeometry = mesh.CreateComponent<MeshGeometry>( insertIndex );
						newMeshGeometry.Name = meshGeometry.Name;
						newMeshGeometry.VertexStructure = meshGeometry.VertexStructure;
						newMeshGeometry.Vertices = meshGeometry.Vertices;
						newMeshGeometry.Indices = meshGeometry.Indices;
						newMeshGeometry.Material = meshGeometry.Material;

						////concat structures. If the geometry is procedural it is not in a structure yet.
						//mesh.Structure = Mesh.StructureClass.Concat( mesh.Structure, structure, i );

						newList.Add( newMeshGeometry );
					}
				}

				//build structure
				if( !allProcedural )
					mesh.BuildStructure();



				//delete old mesh geometries and add to undo
				if( !undoMeshChangesLocked )
					undoMeshChanges.Add( new UndoActionComponentCreateDelete( document, oldList.ToArray(), false ) );
				else
				{
					foreach( var meshGeometry in oldList )
						meshGeometry.Dispose();
				}

				//add created geometries to undo
				if( !undoMeshChangesLocked )
					undoMeshChanges.Add( new UndoActionComponentCreateDelete( document, newList.ToArray(), true ) );

				//add structure update to undo
				if( !undoMeshChangesLocked )
				{
					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Mesh.Structure ) );
					undoMeshChanges.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, oldStructure ) ) );
				}

				undoMeshChangesLocked = true;


				////!!!!?
				//bool hasOrdinary = meshGeometries.Any( geometry => !( geometry is MeshGeometry_Procedural ) );
				//if( !hasOrdinary )
				//	needUndoForNextActions = false; //??? Если были и обычные geometry и procedural? Как правильно needUndoForNextActions? Пока так: undo не нужен только если все procedural

				////!!!!right? !needUndoForNextActions
				//if( undoMultiAction != null && !needUndoForNextActions )
				//{
				//	//add structure update to undo
				//	var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Mesh.Structure ) );
				//	undoMultiAction.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, mesh.Structure?.Clone() ) ) );
				//}

				//for( int i = 0; i < meshGeometries.Length; i++ )
				//{
				//	var meshGeometry = meshGeometries[ i ];

				//	//convert to usual MeshGeometry
				//	if( meshGeometry is MeshGeometry_Procedural meshGeometryProcedural )
				//	{
				//		VertexElement[] vertexStructure = null;
				//		byte[] vertices = null;
				//		int[] indices = null;
				//		Material material = null;
				//		Mesh.StructureClass structure = null;
				//		meshGeometryProcedural.GetProceduralGeneratedData( ref vertexStructure, ref vertices, ref indices, ref material, ref structure );

				//		var insertIndex = meshGeometryProcedural.Parent.Components.IndexOf( meshGeometryProcedural );

				//		var newMeshGeometry = mesh.CreateComponent<MeshGeometry>( insertIndex );
				//		newMeshGeometry.Name = meshGeometry.Name;
				//		newMeshGeometry.VertexStructure = vertexStructure;
				//		newMeshGeometry.Vertices = vertices;
				//		newMeshGeometry.Indices = indices;
				//		newMeshGeometry.Material = meshGeometryProcedural.Material;

				//		//concat structures. If the geometry is procedural it is not in a structure yet.
				//		mesh.Structure = Mesh.StructureClass.Concat( mesh.Structure, structure, i );

				//		//delete old mesh geometry
				//		if( undoMultiAction != null )
				//			undoMultiAction.AddAction( new UndoActionComponentCreateDelete( document, new Component[] { meshGeometry }, false ) );
				//		else
				//			meshGeometry.Dispose();

				//		//add created geometry to undo
				//		if( undoMultiAction != null )
				//			undoMultiAction.AddAction( new UndoActionComponentCreateDelete( document, new Component[] { newMeshGeometry }, true ) );
				//	}
				//}

			}
		}
	}
}
#endif