// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using Fbx;

namespace NeoAxis.Import.FBX
{
	static class ProcessMesh
	{
		public static MeshData[] ProcMesh( FbxManager manager, FbxNode node, FbxMesh mesh, ImportOptions importOptions, Matrix4 additionalTransform )
		{
			var transform = additionalTransform * FbxMath.EvaluateGlobalTransform( node ).ToMatrix4();
			transform = transform * FbxMath.GetGeometryOffset( node ).ToMatrix4();
			var converter = new FbxGeometryConverter( manager );

			bool success = false;
			try
			{
				mesh = FbxMesh.Cast( converter.Triangulate( mesh, false ) ); //ToDo :? Может true? Чтобы не создавать второй mesh в Attribute
				success = true;
			}
			catch( Exception ex )
			{
				FbxImportLog.LogError( node, "Inside Triangulate error: " + ex );
			}
			if( !success || mesh == null )
				return null;

			MeshData data = ReadTriangles( mesh, node );
			var ret = ReadMaterialsAndSplitByMaterials( data );
			foreach( var m in ret )
				ReadMeshElements( m, importOptions, ref transform );

			return ret;
		}

		static void ReadMeshElements( MeshData data, ImportOptions importOptions, ref Matrix4 transform )
		{
			ReadUvSets( data );

			//flip with disabled FlipUVs, no flip when enabled
			if( !importOptions.ImportPostProcessFlags.HasFlag( ImportPostProcessFlags.FlipUVs ) )
			{
				CalcMiscProcess.FlipUVs( data.Vertices, data.VertexComponents );

				//ToDo : !!! Доделать FlipUV для Materials - там тоже Flip для Transform текстуры  .... tex->UVScaling(); tex->UVTranslation();
			}
			ReadColor( data );
			TransformVertices( data, transform );

			int uvSetIndexForNormalsAndTangents = 0; //ToDo : пока 0, может не всегда? 

			if( 3 <= data.PolygonSize )
			{
				ReadNormals( data, importOptions, uvSetIndexForNormalsAndTangents, /*data.CalcCache, */ref transform );
				if( importOptions.ImportPostProcessFlags.HasFlag( ImportPostProcessFlags.FixInfacingNormals ) && data.NormalsSource != TangentsAndNormalsSource.None )
				{
					if( CalcNormalsProcess.FixInfacingNormals( data.Vertices ) )
						FbxImportLog.LogMessage( data.Node, "Infacing Normals Fixed" );
				}

				ReadTangents( data, importOptions, uvSetIndexForNormalsAndTangents, /*data.CalcCache, */ref transform );
			}
		}



		static void ReadColor( MeshData data )
		{
			FbxLayerElementVertexColor pVertexColors = data.Mesh.GetElementVertexColor();
			if( pVertexColors == null )
			{
				for( int i = 0; i < data.Vertices.Length; i++ )
					data.Vertices[ i ].Vertex.Color = new ColorValue( 1, 1, 1 );
				return;
			}

			var mappingMode = pVertexColors.GetMappingMode();
			if( !CheckPolygonVertexOrControlPoint( mappingMode ) )
			{
				FbxImportLog.LogWarning( data.Node, $"has unsupported VertexColors mapping mode: {pVertexColors.GetMappingMode()}" );
				return;
			}
			data.VertexComponents |= StandardVertex.Components.Color;
			var indexArray = pVertexColors.GetReferenceMode() != FbxLayerElement.EReferenceMode.eDirect ? pVertexColors.GetIndexArray() : null;
			var directArray = pVertexColors.GetDirectArray();

			for( int i = 0; i < data.Vertices.Length; i++ )
			{
				ref VertexInfo vertex = ref data.Vertices[ i ];
				FbxColor color = null;
				switch( pVertexColors.GetMappingMode() )
				{
				case FbxLayerElement.EMappingMode.eByPolygonVertex:
					color = directArray.GetAt( indexArray?.GetAt( vertex.PolygonVertexIndex ) ?? vertex.PolygonVertexIndex );
					break;
				case FbxLayerElement.EMappingMode.eByControlPoint:
					color = directArray.GetAt( indexArray?.GetAt( vertex.ControlPointIndex ) ?? vertex.ControlPointIndex );
					break;
				}
				data.Vertices[ i ].Vertex.Color = color?.ToColorValue() ?? new ColorValue( 1, 1, 1 );
			}
		}

		static void ReadNormals( MeshData data, ImportOptions importOptions, int uvSetIndex, /*CalcMeshCache context, */ref Matrix4 transform )
		{
			var source = ReadRawNormals( data, importOptions, uvSetIndex );
			if( source == TangentsAndNormalsSource.None )
				return;
			//if( importOptions.ImportPostProcessFlags.HasFlag( ImportPostProcessFlags.SmoothNormals ) && source != TangentsAndNormalsSource.FromFile )
			//	CalcNormalsProcess.SmoothNormals( data.Vertices, context.VertexFinder, context.PositionEpsilon );

			bool normalWarningSent = false;
			//If the normals are loded by FBX - the original vertex coordinates were used (not transformed)
			if( source == TangentsAndNormalsSource.FromFile /*|| source == TangentsAndNormalsSource.CalculatedByFbxSdk*/ )
			{
				transform.Decompose( out _, out Matrix3 geometryTransformR, out _ );
				for( int i = 0; i < data.Vertices.Length; i++ )
				{
					ref StandardVertex vertex = ref data.Vertices[ i ].Vertex;
					vertex.Normal = ( geometryTransformR * vertex.Normal ).ToVector3F();
					if( !FbxMath.IsNotEmptyVector( vertex.Normal ) && !normalWarningSent )
					{
						FbxImportLog.LogMessage( data.Node, "Has zero or NaN normals" );
						normalWarningSent = true;
					}
				}
			}
			data.VertexComponents |= StandardVertex.Components.Normal;
			data.NormalsSource = source;
		}

		static TangentsAndNormalsSource ReadRawNormals( MeshData data, ImportOptions importOptions, int uvSetIndex )
		{
			var opt = importOptions.NormalsOptions;

			FbxLayerElementNormal pNormals = data.Mesh.GetElementNormal( uvSetIndex );
			bool normalsPresent = pNormals != null;

			//Loading from file.
			if( normalsPresent && ( opt == NormalsAndTangentsLoadOptions.FromFileIfPresentOrCalculate /*|| opt == NormalsAndTangentsLoadOptions.FromFileIfPresentOrCalculateByFbxSdk */) )
			{
				if( ReadRawNormalsByFbx( data ) )
					return TangentsAndNormalsSource.FromFile;
				FbxImportLog.LogError( data.Node, "normals calculation error in FBX SDK" );
				return TangentsAndNormalsSource.None;
			}

			/*
			//calculating by FBX
			if( !normalsPresent && opt == NormalsAndTangentsLoadOptions.FromFileIfPresentOrCalculateByFbxSdk || opt == NormalsAndTangentsLoadOptions.AlwaysCalculateByFbxSdk )
			{
				//Option pOverwrite: if true than calculates even if the normals data are present. no uvSetIndex
				if( data.Mesh.GenerateNormals( true ) &&   
					ReadRawNormalsByFbx( data )
				    )
						return TangentsAndNormalsSource.CalculatedByFbxSdk;
				FbxImportLog.LogError( data.Node, "normals calculation error in FBX SDK" );
				return TangentsAndNormalsSource.None;
			}
			*/

			//calculating normals by custom algorithm.
			CalcNormalsProcess.CalculateNormals( data.Vertices, normalizeVectorNormal: true );
			return TangentsAndNormalsSource.Calculated;
		}

		static void ReadTangents( MeshData data, ImportOptions importOptions, int uvSetIndex, /*CalcMeshCache context, */ref Matrix4 transform )
		{
			data.TangentsSource = TangentsAndNormalsSource.None;
			if( !data.VertexComponents.HasFlag( StandardVertex.Components.Normal ) )
			{
				FbxImportLog.LogError( data.Node, "Failed to compute tangents. Need normals" );
				return;
			}
			var source = ReadRawTangents( data, importOptions, uvSetIndex, out Vector3F[] tangents, out Vector3F[] bitangents );
			if( source == TangentsAndNormalsSource.None )
				return;
			//if( importOptions.ImportPostProcessFlags.HasFlag( ImportPostProcessFlags.SmoothTangents ) && source != TangentsAndNormalsSource.FromFile )
			//	CalcTangentsProcess.SmoothTangents( data.Vertices, context.VertexFinder, tangents, bitangents, context.PositionEpsilon );

			//If mesh is loded by FBX - the original vertex coordinates were used (not transformed)
			if( source == TangentsAndNormalsSource.FromFile /*|| source == TangentsAndNormalsSource.CalculatedByFbxSdk*/ )
			{
				transform.Decompose( out _, out Matrix3 geometryTransformR, out _ );
				for( int i = 0; i < data.Vertices.Length; i++ )
				{
					tangents[ i ] = ( geometryTransformR * tangents[ i ] ).ToVector3F();
					bitangents[ i ] = ( geometryTransformR * bitangents[ i ] ).ToVector3F();
				}
			}

			bool tangentWarningSent = false;
			//now fill vertex.tangent
			for( int i = 0; i < data.Vertices.Length; i++ )
			{
				ref StandardVertex vertex = ref data.Vertices[ i ].Vertex;
				vertex.Tangent = CalcTangent( tangents[ i ], bitangents[ i ], vertex.Normal );
				if( !FbxMath.IsNotEmptyVector( vertex.Tangent ) && !tangentWarningSent )
				{
					FbxImportLog.LogMessage( data.Node, "Has zero or NaN tangents" );
					tangentWarningSent = true;
				}
			}
			data.VertexComponents |= StandardVertex.Components.Tangent;
			data.TangentsSource = source;
		}

		static TangentsAndNormalsSource ReadRawTangents( MeshData data, ImportOptions importOptions, int uvSetIndex, out Vector3F[] tangents, out Vector3F[] bitangents )
		{
			tangents = null;
			bitangents = null;
			var opt = importOptions.TangentsOptions;

			FbxLayerElementTangent pTangents = data.Mesh.GetElementTangent( uvSetIndex ); //If not present for a specified index then returns null
			FbxLayerElementBinormal pBinormals = data.Mesh.GetElementBinormal( uvSetIndex );
			bool tangentsPresent = pTangents != null && pBinormals != null;

			//Loading from file.
			if( tangentsPresent && ( opt == NormalsAndTangentsLoadOptions.FromFileIfPresentOrCalculate /*|| opt == NormalsAndTangentsLoadOptions.FromFileIfPresentOrCalculateByFbxSdk*/ ) )
			{
				if( ReadRawTangentsByFbx( data.Vertices, data.Node, pTangents, pBinormals, out tangents, out bitangents ) )
					return TangentsAndNormalsSource.FromFile;
				FbxImportLog.LogError( data.Node, "tangents calculation error in FBX SDK" );
				return TangentsAndNormalsSource.None;
			}

			/*
			 //calculating by FBX
			if( !tangentsPresent && opt == NormalsAndTangentsLoadOptions.FromFileIfPresentOrCalculateByFbxSdk || opt == NormalsAndTangentsLoadOptions.AlwaysCalculateByFbxSdk )
			{
				//From GenerateTangentsData description: return true if successfully generated tangents data, or if already available and pOverwrite is false.
				//Option pOverwrite: if true than calculates even if the tangents data are present.
				if( data.Mesh.GenerateTangentsData( uvSetIndex, true ) &&
					ReadRawTangentsByFbx( data.Vertices, data.Node, data.Mesh.GetElementTangent( uvSetIndex ), data.Mesh.GetElementBinormal( uvSetIndex ), out tangents, out bitangents ) )
						return TangentsAndNormalsSource.CalculatedByFbxSdk;
				FbxImportLog.LogError( data.Node, "tangents calculation error in FBX SDK" );
				return TangentsAndNormalsSource.None;
			}
			*/

			//calculating tangents by custom algorithm. getTexCoord can be passed as null in CalcTangents, for texCoord0
			CalcTangentsProcess.GetTextureCoord getTexCoord = uvSetIndex == 0 ? null : CreateTextureCoordGetter( uvSetIndex );
			CalcTangentsProcess.CalculateTangents( data.Vertices, out tangents, out bitangents, getTexCoord );
			return TangentsAndNormalsSource.Calculated;
		}

		//when the function is called the normals must be present.
		static bool ReadRawNormalsByFbx( MeshData data )
		{
			int vertexIndexInsidePolygon = -1; //Можно, реализовать без этого индекса, через DirectArray.
			int polygonIndex = -1;
			bool hasErrorWithNormals = false;
			for( int i = 0; i < data.Vertices.Length; i++ )
			{
				ref VertexInfo vertex = ref data.Vertices[ i ];
				if( vertex.PolygonIndex != polygonIndex )
				{
					vertexIndexInsidePolygon = 0;
					polygonIndex = vertex.PolygonIndex;
				}
				else
					vertexIndexInsidePolygon++;

				FbxVector4 vertexNormal = new FbxVector4();
				if( !data.Mesh.GetPolygonVertexNormal( polygonIndex, vertexIndexInsidePolygon, vertexNormal ) && !hasErrorWithNormals )
				{
					FbxImportLog.LogWarning( data.Node, "has corrupted normals." );
					hasErrorWithNormals = true;
				}

				vertex.Vertex.Normal = vertexNormal.ToVector3F();
			}
			return true;
		}

		static bool ReadRawTangentsByFbx( VertexInfo[] vertices, FbxNode node, FbxLayerElementTangent fbxTangents, FbxLayerElementBinormal fbxBinormals, out Vector3F[] tangents, out Vector3F[] bitangents )
		{
			tangents = null;
			bitangents = null;

			var tangentsDirectArray = fbxTangents.GetDirectArray();
			var tangentsIndexArray = fbxTangents.GetReferenceMode() != FbxLayerElement.EReferenceMode.eDirect ? fbxTangents.GetIndexArray() : null;
			var tangentsMappingMode = fbxTangents.GetMappingMode();
			var binormalsDirectArray = fbxBinormals.GetDirectArray();
			var binormalsIndexArray = fbxBinormals.GetReferenceMode() != FbxLayerElement.EReferenceMode.eDirect ? fbxBinormals.GetIndexArray() : null;
			var binormalsMappingMode = fbxBinormals.GetMappingMode();

			if( !CheckPolygonVertexOrControlPoint( tangentsMappingMode ) )
			{
				FbxImportLog.LogWarning( node, $"has unsupported tangents mapping mode: {tangentsMappingMode}" );
				return false;
			}
			if( !CheckPolygonVertexOrControlPoint( binormalsMappingMode ) )
			{
				FbxImportLog.LogWarning( node, $"has unsupported binormals mapping mode: {binormalsMappingMode}" );
				return false;
			}

			tangents = new Vector3F[ vertices.Length ];
			bitangents = new Vector3F[ vertices.Length ];

			for( int i = 0; i < vertices.Length; i++ )
			{
				ref VertexInfo vertex = ref vertices[ i ];
				FbxVector4 sourceTangent = null;
				switch( tangentsMappingMode )
				{
				case FbxLayerElement.EMappingMode.eByPolygonVertex:
					sourceTangent = tangentsDirectArray.GetAt( tangentsIndexArray?.GetAt( vertex.PolygonVertexIndex ) ?? vertex.PolygonVertexIndex );
					break;

				case FbxLayerElement.EMappingMode.eByControlPoint:
					sourceTangent = tangentsDirectArray.GetAt( tangentsIndexArray?.GetAt( vertex.ControlPointIndex ) ?? vertex.ControlPointIndex );
					break;
				}

				if( sourceTangent == null )
					FbxImportLog.LogWarning( node, "FBX SDK internal error" );
				else
					tangents[ i ] = sourceTangent.ToVector3F();


				FbxVector4 sourceBinormal = null;
				switch( binormalsMappingMode )
				{
				case FbxLayerElement.EMappingMode.eByPolygonVertex:
					sourceBinormal = binormalsDirectArray.GetAt( binormalsIndexArray?.GetAt( vertex.PolygonVertexIndex ) ?? vertex.PolygonVertexIndex );
					break;

				case FbxLayerElement.EMappingMode.eByControlPoint:
					sourceBinormal = binormalsDirectArray.GetAt( binormalsIndexArray?.GetAt( vertex.ControlPointIndex ) ?? vertex.ControlPointIndex );
					break;
				}
				if( sourceBinormal == null )
					FbxImportLog.LogWarning( node, "FBX SDK internal error" );
				else
					bitangents[ i ] = sourceBinormal.ToVector3F();
			}
			return true;
		}

		static CalcTangentsProcess.GetTextureCoord CreateTextureCoordGetter( int uvSetIndex )
		{
			switch( uvSetIndex )
			{
			case 0: return ( ref StandardVertex vertex ) => vertex.TexCoord0;
			case 1: return ( ref StandardVertex vertex ) => vertex.TexCoord1;
			case 2: return ( ref StandardVertex vertex ) => vertex.TexCoord2;
			case 3: return ( ref StandardVertex vertex ) => vertex.TexCoord3;
			default: throw new ArgumentException( "Unsupported UV set Index" );
			}
		}

		static bool CheckPolygonVertexOrControlPoint( FbxLayerElement.EMappingMode mappingMode )
		{
			return mappingMode == FbxLayerElement.EMappingMode.eByPolygonVertex || mappingMode == FbxLayerElement.EMappingMode.eByControlPoint;
		}

		static Vector4F CalcTangent( Vector3F tangent, Vector3F binormal, Vector3F normal )
		{
			tangent = tangent.GetNormalize();
			binormal = binormal.GetNormalize();

			float parity;
			if( Vector3F.Dot( Vector3F.Cross( tangent, binormal ), normal ) >= 0 )
				parity = -1;
			else
				parity = 1;
			return new Vector4F( tangent, parity );
		}

		static void TransformVertices( MeshData data, Matrix4 transform )
		{
			for( int i = 0; i < data.Vertices.Length; i++ )
				data.Vertices[ i ].Vertex.Position = ( transform * data.Vertices[ i ].Vertex.Position ).ToVector3F();
		}

		static void ReadUvSets( MeshData data )
		{
			var uvSets = FbxExtensions.GetElementUVs( data.Mesh ).Take( 4 ).ToArray();

			for( int uvSetIndex = 0; uvSetIndex < uvSets.Length; uvSetIndex++ )
			{
				FbxLayerElementUV uvElement = uvSets[ uvSetIndex ];
				//index array, which holds the index referenced to the uv data
				var indexArray = uvElement.GetReferenceMode() != FbxLayerElement.EReferenceMode.eDirect ? uvElement.GetIndexArray() : null;
				var directArray = uvElement.GetDirectArray();
				var mappingMode = uvElement.GetMappingMode();
				if( !CheckPolygonVertexOrControlPoint( mappingMode ) )
				{
					FbxImportLog.LogError( data.Node, $"has unsupported UVSet mapping mode: {mappingMode}" );
					continue;
				}

				for( int i = 0; i < data.Vertices.Length; i++ )
				{
					ref VertexInfo vertex = ref data.Vertices[ i ];
					FbxVector2 uvValue = null;
					//the UV index depends on the reference mode
					switch( mappingMode )
					{
					case FbxLayerElement.EMappingMode.eByPolygonVertex:
						uvValue = directArray.GetAt( indexArray?.GetAt( vertex.PolygonVertexIndex ) ?? vertex.PolygonVertexIndex );
						break;
					case FbxLayerElement.EMappingMode.eByControlPoint:
						uvValue = directArray.GetAt( indexArray?.GetAt( vertex.ControlPointIndex ) ?? vertex.ControlPointIndex );
						break;
					}

					if( uvValue != null )
					{
						Vector2F val = uvValue.ToVector2().ToVector2F();
						switch( uvSetIndex )
						{
						case 0: vertex.Vertex.TexCoord0 = val; break;
						case 1: vertex.Vertex.TexCoord1 = val; break;
						case 2: vertex.Vertex.TexCoord2 = val; break;
						case 3: vertex.Vertex.TexCoord3 = val; break;
						}
					}
					else
						FbxImportLog.LogWarning( data.Node, "FBX SDK Internal Error" ); //Impossible condition
				}

				switch( uvSetIndex )
				{
				case 0: data.VertexComponents |= StandardVertex.Components.TexCoord0; break;
				case 1: data.VertexComponents |= StandardVertex.Components.TexCoord1; break;
				case 2: data.VertexComponents |= StandardVertex.Components.TexCoord2; break;
				case 3: data.VertexComponents |= StandardVertex.Components.TexCoord3; break;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fbxMesh"></param>
		/// <param name="node"></param>
		/// <returns>MeshData - only triangles</returns>
		static MeshData ReadTriangles( FbxMesh fbxMesh, FbxNode node )
		{
			var polygonVertices = FbxExtensions.GetPolygonVertices( fbxMesh );
			FbxVector4Array pControlPoints = FbxVector4Array.frompointer( fbxMesh.GetControlPoints() );
			int polygonCount = fbxMesh.GetPolygonCount();
			var meshAr = new MeshData
			{
				Mesh = fbxMesh,
				Node = node,
				PolygonSize = 3,
				VertexComponents = StandardVertex.Components.Position
			};
			var vertices = new List<VertexInfo>();

			int degenerateTrianglesCount = 0;
			for( int polygonIndex = 0; polygonIndex < polygonCount; polygonIndex++ )
			{
				int startIndex = fbxMesh.GetPolygonVertexIndex( polygonIndex );
				if( startIndex == -1 )
					continue;

				var polygonPoints = new VertexInfo[ 3 ];
				int polygonSize = fbxMesh.GetPolygonSize( polygonIndex );

				System.Diagnostics.Debug.Assert( polygonSize <= 3 );

				for( int j = 0; j < polygonSize; j++ )
				{
					int controlPointIndex = polygonVertices[ startIndex + j ];

					Vector3F position = pControlPoints.getitem( controlPointIndex ).ToVector3().ToVector3F();
					polygonPoints[ j ] = new VertexInfo
					{
						ControlPointIndex = controlPointIndex,
						PolygonIndex = polygonIndex,
						PolygonVertexIndex = startIndex + j,
						Vertex = new StandardVertex { Position = position }
					};

				}
				RemoveEqualPoints( polygonPoints, ref polygonSize );
				if( polygonSize == 3 && !MathAlgorithms.IsDegenerateTriangle( ref polygonPoints[ 0 ].Vertex.Position, ref polygonPoints[ 1 ].Vertex.Position, ref polygonPoints[ 2 ].Vertex.Position ) )
					for( int i = 0; i < polygonSize; i++ )
						vertices.Add( polygonPoints[ i ] );
				else
					degenerateTrianglesCount++;
			}
			if( degenerateTrianglesCount != 0 )
				FbxImportLog.LogMessage( node, $"Removed degenerate triangles count : {degenerateTrianglesCount}" );
			meshAr.Vertices = vertices.ToArray();
			return meshAr;
		}

		//remove similar points, and reduce count.
		static bool RemoveEqualPoints( VertexInfo[] points, ref int count )
		{
			int oldCount = count;
			System.Diagnostics.Debug.Assert( count == 1 || count == 2 || count == 3 );

			if( count == 2 )
			{
				if( points[ 0 ].Vertex.Position == points[ 1 ].Vertex.Position )
					count = 1;
			}
			else if( count == 3 )
			{
				if( points[ 0 ].Vertex.Position == points[ 2 ].Vertex.Position )
					count = 2;
				if( points[ 0 ].Vertex.Position == points[ 1 ].Vertex.Position )
				{
					if( count == 2 )
						count = 1;
					else
					{
						points[ 1 ] = points[ 2 ];
						count = 2;
					}
				}
			}

			return count != oldCount;
		}

		static MeshData[] ReadMaterialsAndSplitByMaterials( MeshData data )
		{
			int materialCount = data.Node.GetMaterialCount();
			if( materialCount == 0 )
			{
				data.MaterialIndex = -1;
				data.MaterialName = null;
				return new[] { data };
			}

			FbxLayerElementMaterial elementMaterial = data.Mesh.GetElementMaterial();
			var mappingMode = elementMaterial.GetMappingMode();
			var refMode = elementMaterial.GetReferenceMode();
			if( refMode != FbxLayerElement.EReferenceMode.eIndexToDirect )
			{
				FbxImportLog.LogError( data.Node, $"has unsupported materials reference mode: {refMode}" );
				return new[] { data };
			}

			if( mappingMode == FbxLayerElement.EMappingMode.eAllSame )
			{
				data.MaterialIndex = elementMaterial.GetIndexArray().at( 0 );
				data.MaterialName = data.Node.GetMaterial( data.MaterialIndex ).GetName();
				return new[] { data };
			}
			else if( mappingMode == FbxLayerElement.EMappingMode.eByPolygon )
			{
				var splitVertices = new List<VertexInfo>[ materialCount + 1 ]; // if index is -1, it is writen to [materialCount]
				var indexArray = elementMaterial.GetIndexArray();
				for( int i = 0; i < data.Vertices.Length; i++ )
				{
					int materialIndex = indexArray.GetAt( data.Vertices[ i ].PolygonIndex );
					if( materialIndex < 0 )
						materialIndex = materialCount;
					var list = splitVertices[ materialIndex ];
					if( list == null )
					{
						list = new List<VertexInfo>();
						splitVertices[ materialIndex ] = list;
					}
					list.Add( data.Vertices[ i ] );
				}

				return splitVertices
					.Select( ( vertices, index ) =>
						vertices == null ? null : new MeshData
						{
							Mesh = data.Mesh,
							Node = data.Node,
							VertexComponents = data.VertexComponents,
							PolygonSize = data.PolygonSize,
							MaterialIndex = index == materialCount ? -1 : index,
							MaterialName = index == materialCount ? null : data.Node.GetMaterial( index ).GetName(),
							Vertices = splitVertices[ index ].ToArray()
						} )
					.Where( _ => _ != null )
					.ToArray();
			}
			else
			{
				FbxImportLog.LogError( data.Node, $"has unsupported materials mapping mode: {mappingMode}" );
				return new[] { data };
			}
		}
	}
}
