// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Linq;
using NeoAxis.Import.FBX;
using Fbx;

namespace NeoAxis.Import
{
	class ImportMegascans : ImportGeneral
	{
		class ImportContext
		{
			public Settings settings;
			public ImportMegascansFormat.Welcome json;
			public string directoryName;
			public Component materialsGroup;
			public EDictionary<int, Component_Material> materialByIndex = new EDictionary<int, Component_Material>();
		}

		public static void DoImport( Settings settings, out string error )
		{
			try
			{
				var json = ImportMegascansFormat.Welcome.FromJson( VirtualFile.ReadAllText( settings.virtualFileName ) );

				var context = new ImportContext();
				context.settings = settings;
				context.json = json;
				context.directoryName = Path.GetDirectoryName( settings.virtualFileName );

				ImportJson( context, out error );
			}
			catch( Exception e )
			{
				error = e.Message;
				return;
			}
		}

		static string SelectTextureFromList( List<string> list )
		{
			//select jpg between jpg, exr
			CollectionUtility.SelectionSort( list, delegate ( string v1, string v2 )
			{
				var ext1 = Path.GetExtension( v1.ToLower() );
				var ext2 = Path.GetExtension( v2.ToLower() );

				if( ext1 != ext2 )
				{
					if( ext1 == ".jpg" )
						return -1;
					if( ext2 == ".jpg" )
						return 1;
				}

				return 0;
			} );

			if( list.Count != 0 )
				return list[ 0 ];
			else
				return "";
		}

		static string GetMapsTextureName( ImportContext context, string type )
		{
			var foundList = new List<string>();
			foreach( var item in context.json.Maps )
			{
				if( item.Type == type && !string.IsNullOrEmpty( item.Uri ) )
				{
					var fileName = Path.Combine( context.directoryName, VirtualPathUtility.NormalizePath( item.Uri ) );
					if( VirtualFile.Exists( fileName ) )
						foundList.Add( fileName );
				}
			}

			return SelectTextureFromList( foundList );
		}

		static string GetBillboardsTextureName( ImportContext context, string type )
		{
			var foundList = new List<string>();
			foreach( var item in context.json.Billboards )
			{
				if( item.Type == type && !string.IsNullOrEmpty( item.Uri ) )
				{
					var fileName = Path.Combine( context.directoryName, VirtualPathUtility.NormalizePath( item.Uri ) );
					if( VirtualFile.Exists( fileName ) )
						foundList.Add( fileName );
				}
			}

			return SelectTextureFromList( foundList );
		}

		static string GetComponentsTextureName( ImportContext context, string type )
		{
			var foundList = new List<string>();
			foreach( var item in context.json.Components )
			{
				if( item.Type == type )
				{
					foreach( var uri in item.Uris )
					{
						foreach( var resolution in uri.Resolutions )
						{
							foreach( var format in resolution.Formats )
							{
								var fileName = Path.Combine( context.directoryName, VirtualPathUtility.NormalizePath( format.Uri ) );
								if( VirtualFile.Exists( fileName ) )
									foundList.Add( fileName );
							}
						}
					}
				}
			}

			return SelectTextureFromList( foundList );
		}

		static List<MaterialData> GetMaterialsData( ImportContext context, bool onlyOneMaterial )
		{
			var result = new List<MaterialData>();

			var json = context.json;

			int materialIndexCounter = 0;
			int materialNameCounter = 1;

			//maps
			if( json.Maps != null )
			{
				var data = new MaterialData();
				data.Index = materialIndexCounter;
				materialIndexCounter++;

				if( onlyOneMaterial )
					data.Name = "Material";
				else
				{
					data.Name = $"Material {materialNameCounter}";
					materialNameCounter++;
				}

				string textureName;

				textureName = GetMapsTextureName( context, "albedo" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.BaseColorTexture = textureName;

				textureName = GetMapsTextureName( context, "metalness" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.MetallicTexture = textureName;

				textureName = GetMapsTextureName( context, "roughness" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.RoughnessTexture = textureName;

				textureName = GetMapsTextureName( context, "normal" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.NormalTexture = textureName;

				if( context.settings.component.MaterialDisplacement )
				{
					textureName = GetMapsTextureName( context, "displacement" );
					if( !string.IsNullOrEmpty( textureName ) )
						data.DisplacementTexture = textureName;
				}

				textureName = GetMapsTextureName( context, "ao" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.AmbientOcclusionTexture = textureName;

				textureName = GetMapsTextureName( context, "opacity" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.OpacityTexture = textureName;

				result.Add( data );
			}

			//billboards
			if( json.Billboards != null )
			{
				var data = new MaterialData();
				data.Index = -1;
				if( onlyOneMaterial )
					data.Name = "Material";
				else
					data.Name = "Material Billboards";
				//data.ShadingModel = Component_Material.ShadingModelEnum.Simple;

				string textureName;

				textureName = GetBillboardsTextureName( context, "albedo" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.BaseColorTexture = textureName;

				textureName = GetBillboardsTextureName( context, "metalness" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.MetallicTexture = textureName;

				textureName = GetBillboardsTextureName( context, "roughness" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.RoughnessTexture = textureName;

				textureName = GetBillboardsTextureName( context, "normal" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.NormalTexture = textureName;

				if( context.settings.component.MaterialDisplacement )
				{
					textureName = GetBillboardsTextureName( context, "displacement" );
					if( !string.IsNullOrEmpty( textureName ) )
						data.DisplacementTexture = textureName;
				}

				textureName = GetBillboardsTextureName( context, "ao" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.AmbientOcclusionTexture = textureName;

				textureName = GetBillboardsTextureName( context, "opacity" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.OpacityTexture = textureName;

				result.Add( data );
			}

			//components
			if( json.Components != null )
			{
				var data = new MaterialData();
				data.Index = materialIndexCounter;
				materialIndexCounter++;

				if( onlyOneMaterial )
					data.Name = "Material";
				else
					data.Name = $"Material {materialNameCounter}";
				materialNameCounter++;

				string textureName;

				textureName = GetComponentsTextureName( context, "albedo" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.BaseColorTexture = textureName;

				textureName = GetComponentsTextureName( context, "metalness" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.MetallicTexture = textureName;

				textureName = GetComponentsTextureName( context, "roughness" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.RoughnessTexture = textureName;

				textureName = GetComponentsTextureName( context, "normal" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.NormalTexture = textureName;

				if( context.settings.component.MaterialDisplacement )
				{
					textureName = GetComponentsTextureName( context, "displacement" );
					if( !string.IsNullOrEmpty( textureName ) )
						data.DisplacementTexture = textureName;
				}

				textureName = GetComponentsTextureName( context, "ao" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.AmbientOcclusionTexture = textureName;

				textureName = GetComponentsTextureName( context, "opacity" );
				if( !string.IsNullOrEmpty( textureName ) )
					data.OpacityTexture = textureName;

				result.Add( data );
			}

			return result;
		}

		unsafe static void MeshGetIsBillboard( ImportContext context, Component_Mesh destinationMesh )
		{
			bool isBillboard = false;

			var geometries = destinationMesh.GetComponents<Component_MeshGeometry>();
			if( geometries.Length == 1 )
			{
				var geometry = geometries[ 0 ];

				try
				{
					var vertexStructure = geometry.VertexStructure.Value;
					var vertices = geometry.Vertices.Value;
					var indices = geometry.Indices.Value;

					if( vertexStructure?.Length != 0 && vertices?.Length != 0 && indices?.Length != 0 )
					{
						vertexStructure.GetInfo( out var vertexSize, out _ );
						int vertexCount = vertices.Length / vertexSize;

						if( indices.Length >= 3 )
						{
							if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out var positionElement ) )
							{
								if( positionElement.Type == VertexElementType.Float3 && positionElement.Source == 0 )
								{
									int offset = positionElement.Offset;

									var p = new Vector3[ vertexCount ];
									for( int nVertex = 0; nVertex < vertexCount; nVertex++ )
									{
										fixed ( byte* pVertices = vertices )
											p[ nVertex ] = *(Vector3F*)( pVertices + nVertex * vertexSize + offset );
									}

									var normal1 = Plane.FromPoints( p[ indices[ 0 ] ], p[ indices[ 1 ] ], p[ indices[ 2 ] ] ).Normal;
									if( Math.Abs( normal1.X ) > 0.95f || Math.Abs( normal1.Y ) > 0.95f )
									{
										bool canBeBillboard = true;

										for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
										{
											var index0 = indices[ nTriangle * 3 + 0 ];
											var index1 = indices[ nTriangle * 3 + 1 ];
											var index2 = indices[ nTriangle * 3 + 2 ];
											var normal2 = Plane.FromPoints( p[ index0 ], p[ index1 ], p[ index2 ] ).Normal;

											if( !normal1.Equals( normal2, 0.01 ) )
											{
												canBeBillboard = false;
												break;
											}
										}

										if( canBeBillboard )
											isBillboard = true;
									}
								}
							}
						}
					}
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
				}
			}

			if( isBillboard )
			{
				destinationMesh.Billboard = true;

				foreach( var geometry in geometries )
				{
					if( context.materialByIndex.TryGetValue( -1, out var material ) )
						geometry.Material = ReferenceUtility.MakeRootReference( material );
				}
			}
		}

		static bool LoadFBX( ImportContext context, string virtualFileName, out List<MeshData> geometries )
		{
			geometries = null;

			var settings = context.settings;

			ImportFBX.LoadNativeLibrary();

			FbxManager manager = null;
			FbxIOSettings setting = null;
			FbxImporter importer = null;
			FbxScene scene = null;
			try
			{
				manager = FbxManager.Create();
				setting = FbxIOSettings.Create( manager, "IOSRoot" );
				manager.SetIOSettings( setting );

				importer = FbxImporter.Create( manager, "" );
				var realFileName = VirtualPathUtility.GetRealPathByVirtual( virtualFileName );
				//VirtualFileStream stream = null;
				//ToDo : FromStream
				bool status;
				if( !string.IsNullOrEmpty( realFileName ) && File.Exists( realFileName ) )
				{
					status = importer.Initialize( realFileName, -1, setting );
				}
				else
				{
					return false;
					//throw new NotImplementedException();
					//ToDo : ....
					//stream = VirtualFile.Open( settings.virtualFileName );
					//FbxStream fbxStream = null;
					//SWIGTYPE_p_void streamData = null;

					//status = impoter.Initialize( fbxStream, streamData, -1, setting );
				}

				if( !status )
					return false;

				scene = FbxScene.Create( manager, "scene1" );
				status = importer.Import( scene );
				if( !status )
					return false;

				//convert axis
				if( context.settings.component.ForceFrontXAxis )
				{
					//Через такой конструктор не получится создать такие же оси как EPreDefinedAxisSystem.eMax - Front Axis имеет обратное направление, а направление задать нельзя.
					//new FbxAxisSystem( FbxAxisSystem.EUpVector.eZAxis, FbxAxisSystem.EFrontVector.eParityOdd, FbxAxisSystem.ECoordSystem.eRightHanded );
					//FromFBX Docs:
					//The enum values ParityEven and ParityOdd denote the first one and the second one of the remain two axes in addition to the up axis.
					//For example if the up axis is X, the remain two axes will be Y And Z, so the ParityEven is Y, and the ParityOdd is Z ;

					//We desire to convert the scene from Y-Up to Z-Up. Using the predefined axis system: Max (UpVector = +Z, FrontVector = -Y, CoordSystem = +X (RightHanded))
					var maxAxisSystem = new FbxAxisSystem( FbxAxisSystem.EPreDefinedAxisSystem.eMax );

					if( !scene.GetGlobalSettings().GetAxisSystem().eq( maxAxisSystem ) )
						maxAxisSystem.ConvertScene( scene ); //No conversion will take place if the scene current axis system is equal to the new one. So condition can be removed.
				}

				//convert units
				if( !scene.GetGlobalSettings().GetSystemUnit().eq( FbxSystemUnit.m ) )
					FbxSystemUnit.m.ConvertScene( scene );

				var additionalTransform = new Matrix4( settings.component.Rotation.Value.ToMatrix3() * Matrix3.FromScale( settings.component.Scale ), settings.component.Position );

				var options = new ImportOptions
				{
					NormalsOptions = NormalsAndTangentsLoadOptions.FromFileIfPresentOrCalculate,
					TangentsOptions = NormalsAndTangentsLoadOptions.FromFileIfPresentOrCalculate,
					ImportPostProcessFlags = ImportPostProcessFlags.FixInfacingNormals
				};
				options.ImportPostProcessFlags |= ImportPostProcessFlags.SmoothNormals | ImportPostProcessFlags.SmoothTangents;
				if( context.settings.component.FlipUVs )
					options.ImportPostProcessFlags |= ImportPostProcessFlags.FlipUVs;
				//if( importContext.settings.component.MergeMeshGeometries )
				//	options.ImportPostProcessFlags |= ImportPostProcessFlags.MergeGeometriesByMaterials;

				var sceneLoader = new SceneLoader();
				sceneLoader.Load( scene, manager, options, additionalTransform );

				geometries = sceneLoader.Geometries;
				//foreach( var geometry in sceneLoader.Geometries )
				//	ImportGeometry( context, destinationMesh, geometry );

				////check is it a billboard
				//MeshGetIsBillboard( context, destinationMesh );

				//stream?.Dispose();
			}
			finally
			{
				//Особенности удаления.
				//Создается через функцию: impoter = FbxImporter.Create(manager, "");
				//В таких случаях(создание не через конструктор, а возврат указателя из функции) SWIG задает флажок что объект не владеет ссылкой, поэтому Dispose ничего не делает.
				//Хотя в SWIG можно задать в конфигурации: %newobject FbxImporter::Create; Тогда объект будет владеть ссылкой. Но все равно в С++ наследники FbxObject не имеют public destructor
				//поэтому в Dispose вставлен: throw new MethodAccessException("C++ destructor does not have public access"). Поэтому удалять только через Destroy.

				try { scene?.Destroy(); } catch { }
				try { importer?.Destroy(); } catch { }
				try { setting?.Destroy(); } catch { }
				try { manager?.Destroy(); } catch { }
			}

			return true;
		}

		class LOD
		{
			public List<MeshData> geometries = new List<MeshData>();
		}

		class Variation
		{
			public List<LOD> lods = new List<LOD>();
		}

		static void CreateMesh( ImportContext context, List<LOD> lods, string name, Component parent )
		{
			var settings = context.settings;

			var mesh = parent.CreateComponent<Component_Mesh>( enabled: false );
			mesh.Name = name;

			{
				var lodData = lods[ 0 ];
				foreach( var geometry in lodData.geometries )
					ImportGeometry( context, mesh, geometry );
				MeshGetIsBillboard( context, mesh );
				if( settings.component.MergeMeshGeometries )
					mesh.MergeGeometriesWithEqualVertexStructureAndMaterial();
			}

			for( int nLod = 1; nLod < lods.Count; nLod++ )
			{
				var lod = mesh.CreateComponent<Component_MeshLevelOfDetail>();
				lod.Name = $"LOD {nLod}";
				lod.Distance = settings.component.LODDistance * nLod;

				var lodMesh = lod.CreateComponent<Component_Mesh>();
				lodMesh.Name = "Mesh";

				var lodData = lods[ nLod ];
				foreach( var geometry in lodData.geometries )
					ImportGeometry( context, lodMesh, geometry );
				MeshGetIsBillboard( context, lodMesh );
				if( settings.component.MergeMeshGeometries )
					lodMesh.MergeGeometriesWithEqualVertexStructureAndMaterial();

				lod.Mesh = ReferenceUtility.MakeThisReference( lod, lodMesh );
			}

			mesh.Enabled = true;
		}

		static void ImportJson( ImportContext context, out string error )
		{
			error = "";

			var settings = context.settings;
			var json = context.json;

			bool onlyOneMaterial = /*json.Maps != null &&*/ json.Models == null && json.Meshes == null;// && json.Components == null;

			if( onlyOneMaterial )
				settings.disableDeletionUnusedMaterials = true;

			//get materials data
			var materialsData = GetMaterialsData( context, onlyOneMaterial );

			//init materials group
			if( !onlyOneMaterial )
			{
				context.materialsGroup = context.settings.component.GetComponent( "Materials" );
				if( context.materialsGroup == null && materialsData.Count != 0 && settings.updateMaterials )
				{
					context.materialsGroup = context.settings.component.CreateComponent<Component>();
					context.materialsGroup.Name = "Materials";
				}
			}
			else
				context.materialsGroup = context.settings.component;

			//create materials
			foreach( var data in materialsData )
			{
				Component_Material material = null;
				if( context.settings.updateMaterials )
					material = CreateMaterial( context.materialsGroup, data );
				else
				{
					if( context.materialsGroup != null )
						material = context.materialsGroup.GetComponent( data.Name ) as Component_Material;
				}
				if( material != null )
					context.materialByIndex.Add( data.Index, material );
			}

			if( settings.updateMeshes )
			{
				var variations = new Dictionary<int, Variation>();
				var geometriesByFile = new Dictionary<string, List<MeshData>>();

				//meshes
				if( json.Meshes != null )
				{
					//!!!!add Mode = OneMesh support?

					var fileNames = new List<string>();
					foreach( var mesh in json.Meshes )
					{
						if( mesh.Uris != null )
						{
							foreach( var uri in mesh.Uris )
							{
								var fileName = Path.Combine( context.directoryName, VirtualPathUtility.NormalizePath( uri.Uri ) );
								if( VirtualFile.Exists( fileName ) )
									fileNames.Add( fileName );
							}
						}
					}

					if( fileNames.Count != 0 )
					{
						var lodRange = settings.component.LODRange.Value;

						for( int lodIndex = 0; lodIndex < fileNames.Count; lodIndex++ )
						{
							if( lodIndex >= lodRange.Minimum && lodIndex <= lodRange.Maximum )
							{
								var fileName = fileNames[ lodIndex ];
								if( !LoadFBX( context, fileName, out var geometries ) )
								{
									error = $"Unable to load \"{fileName}\".";
									return;
								}

								for( int nGeometry = 0; nGeometry < geometries.Count; nGeometry++ )
								{
									var geometry = geometries[ nGeometry ];

									int variationIndex = nGeometry + 1;
									if( !variations.TryGetValue( variationIndex, out var variation ) )
									{
										variation = new Variation();
										variations[ variationIndex ] = variation;
									}

									var lod = new LOD();
									lod.geometries.Add( geometry );
									var indexToAdd = lodIndex - lodRange.Minimum;
									while( indexToAdd >= variation.lods.Count )
										variation.lods.Add( new LOD() );
									variation.lods[ indexToAdd ] = lod;
								}
							}
						}
					}
				}

				//models
				if( json.Models != null )
				{
					var lodRange = settings.component.LODRange.Value;

					foreach( var model in json.Models )
					{
						var lodIndex = (int)model.Lod;
						if( lodIndex >= lodRange.Minimum && lodIndex <= lodRange.Maximum )
						{
							var variationIndex = (int)model.Variation;

							if( !variations.TryGetValue( variationIndex, out var variation ) )
							{
								variation = new Variation();
								variations[ variationIndex ] = variation;
							}

							var fileName = Path.Combine( context.directoryName, VirtualPathUtility.NormalizePath( model.Uri ) );
							if( VirtualFile.Exists( fileName ) )
							{
								if( !geometriesByFile.TryGetValue( fileName, out var geometries ) )
								{
									if( !LoadFBX( context, fileName, out geometries ) )
									{
										error = $"Unable to load \"{fileName}\".";
										return;
									}
									geometriesByFile[ fileName ] = geometries;
								}

								var lod = new LOD();
								lod.geometries.AddRange( geometries );
								var indexToAdd = lodIndex - lodRange.Minimum;
								while( indexToAdd >= variation.lods.Count )
									variation.lods.Add( new LOD() );
								variation.lods[ indexToAdd ] = lod;
							}
						}
					}
				}

				int meshCount = variations.Count;
				if( meshCount > 0 )
				{
					//init meshes group
					Component meshesGroup = null;
					if( meshCount > 1 )
					{
						meshesGroup = context.settings.component.GetComponent( "Meshes" );
						if( meshesGroup == null )
						{
							meshesGroup = context.settings.component.CreateComponent<Component>();
							meshesGroup.Name = "Meshes";
						}
					}
					else if( meshCount == 1 )
						meshesGroup = context.settings.component;

					//create meshes
					foreach( var pair in variations )
					{
						var variationIndex = pair.Key;
						var variation = pair.Value;

						if( variation.lods.Count != 0 )
						{
							string name;
							if( meshCount != 1 )
								name = $"Mesh {variationIndex}";
							else
								name = "Mesh";

							CreateMesh( context, variation.lods, name, meshesGroup );
						}
					}
				}



				//var fileNames = new List<string>();
				//foreach( var mesh in json.Meshes )
				//{
				//	if( mesh.Uris != null )
				//	{
				//		foreach( var uri in mesh.Uris )
				//		{
				//			var fileName = Path.Combine( context.directoryName, VirtualPathUtility.NormalizePath( uri.Uri ) );
				//			if( VirtualFile.Exists( fileName ) )
				//				fileNames.Add( fileName );
				//		}
				//	}
				//}

				//if( fileNames.Count != 0 )
				//{
				//	var lods = new List<LOD>();

				//	for( int nLod = 0; nLod < fileNames.Count; nLod++ )
				//	{
				//		if( nLod < settings.component.LODCount )
				//		{
				//			var fileName = fileNames[ nLod ];
				//			if( !LoadFBX( context, fileName, out var geometries ) )
				//			{
				//				error = $"Unable to load \"{fileName}\".";
				//				return;
				//			}

				//			var lod = new LOD();
				//			lod.geometries = geometries;
				//			lods.Add( lod );
				//		}
				//	}

				//	CreateMesh( context, lods, "Mesh", context.settings.component );
				//}



				////load data
				//{
				//	//models
				//	if( json.Models != null )
				//	{
				//		var variations = new EDictionary<int, List<string>>();
				//		foreach( var model in json.Models )
				//		{
				//			var fileName = Path.Combine( context.directoryName, VirtualPathUtility.NormalizePath( model.Uri ) );
				//			if( VirtualFile.Exists( fileName ) )
				//			{
				//				if( !LoadFBXToMesh( context, fileName, out var geometries ) )
				//				{
				//					error = $"Unable to load \"{fileName}\".";
				//					return;
				//				}
				//				loadedData[ fileName ] = geometries;
				//				//fileNames.Add( fileName );
				//			}
				//		}
				//	}

				//}

				//xx xx;


				////calculate mesh count
				//int meshCount = 0;
				//{
				//	//models
				//	if( json.Models != null )
				//	{
				//		var variations = new EDictionary<int, List<string>>();
				//		foreach( var model in json.Models )
				//		{
				//			var variation = (int)model.Variation;
				//			if( !variations.TryGetValue( variation, out var fileNames ) )
				//			{
				//				fileNames = new List<string>();
				//				variations[ variation ] = fileNames;
				//			}

				//			var fileName = Path.Combine( context.directoryName, VirtualPathUtility.NormalizePath( model.Uri ) );
				//			if( VirtualFile.Exists( fileName ) )
				//			{
				//				xx xx;

				//				fileNames.Add( fileName );
				//			}
				//		}

				//		foreach( var pair in variations )
				//		{
				//			var fileNames = pair.Value;

				//			if( fileNames.Count != 0 )
				//				meshCount++;
				//		}
				//	}

				//	//one mesh in the source
				//	if( json.Meshes != null )
				//	{
				//		var fileNames = new List<string>();

				//		foreach( var mesh in json.Meshes )
				//		{
				//			if( mesh.Uris != null )
				//			{
				//				foreach( var uri in mesh.Uris )
				//				{
				//					var fileName = Path.Combine( context.directoryName, VirtualPathUtility.NormalizePath( uri.Uri ) );
				//					if( VirtualFile.Exists( fileName ) )
				//						fileNames.Add( fileName );
				//				}
				//			}
				//		}

				//		if( fileNames.Count != 0 )
				//			meshCount++;
				//	}
				//}

				////init meshes group
				//Component meshesGroup = null;
				//if( meshCount > 1 )
				//{
				//	meshesGroup = context.settings.component.GetComponent( "Meshes" );
				//	if( meshesGroup == null )
				//	{
				//		meshesGroup = context.settings.component.CreateComponent<Component>();
				//		meshesGroup.Name = "Meshes";
				//	}
				//}
				//else if( meshCount == 1 )
				//	meshesGroup = context.settings.component;

				////models
				//if( json.Models != null )
				//{
				//	var variations = new EDictionary<int, List<string>>();
				//	foreach( var model in json.Models )
				//	{
				//		var variation = (int)model.Variation;
				//		if( !variations.TryGetValue( variation, out var fileNames ) )
				//		{
				//			fileNames = new List<string>();
				//			variations[ variation ] = fileNames;
				//		}

				//		var fileName = Path.Combine( context.directoryName, VirtualPathUtility.NormalizePath( model.Uri ) );
				//		if( VirtualFile.Exists( fileName ) )
				//			fileNames.Add( fileName );
				//	}

				//	foreach( var pair in variations )
				//	{
				//		var variation = pair.Key;
				//		var fileNames = pair.Value;

				//		if( fileNames.Count != 0 )
				//		{
				//			if( !CreateMesh( context, fileNames, $"Mesh {variation}", meshesGroup, out error ) )
				//				return;
				//		}
				//	}
				//}

				////one mesh in the source
				//if( json.Meshes != null )
				//{
				//	var fileNames = new List<string>();

				//	foreach( var mesh in json.Meshes )
				//	{
				//		if( mesh.Uris != null )
				//		{
				//			foreach( var uri in mesh.Uris )
				//			{
				//				var fileName = Path.Combine( context.directoryName, VirtualPathUtility.NormalizePath( uri.Uri ) );
				//				if( VirtualFile.Exists( fileName ) )
				//					fileNames.Add( fileName );
				//			}
				//		}
				//	}

				//	if( fileNames.Count != 0 )
				//	{
				//		if( !CreateMesh( context, fileNames, "Mesh", context.settings.component, out error ) )
				//			return;
				//	}
				//}

			}


			//	//Object In Space
			//	if( settings.updateObjectsInSpace && meshesGroup != null )
			//	{
			//		var objectInSpace = settings.component.CreateComponent<Component_ObjectInSpace>( enabled: false );
			//		objectInSpace.Name = "Object In Space";

			//		foreach( var mesh in meshesGroup.Components )
			//		{
			//			var meshInSpace = objectInSpace.CreateComponent<Component_MeshInSpace>();
			//			meshInSpace.Name = mesh.Name;
			//			meshInSpace.CanBeSelected = false;
			//			meshInSpace.Mesh = ReferenceUtility.MakeReference<Component_Mesh>( null, ReferenceUtility.CalculateRootReference( mesh ) );

			//			//Transform
			//			//!!!!transform?
			//			var pos = Vector3.Zero;
			//			var rot = Quaternion.Identity;
			//			var scl = Vector3.One;
			//			//( globalTransform * node.Transform.ToMat4() ).Decompose( out var pos, out Quat rot, out var scl );

			//			var transformOffset = meshInSpace.CreateComponent<Component_TransformOffset>();
			//			transformOffset.Name = "Transform Offset";
			//			transformOffset.PositionOffset = pos;
			//			transformOffset.RotationOffset = rot;
			//			transformOffset.ScaleOffset = scl;

			//			transformOffset.Source = ReferenceUtility.MakeThisReference( transformOffset, objectInSpace, "Transform" );
			//			meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, transformOffset, "Result" );
			//			//transformOffset.Source = ReferenceUtils.CreateReference<Transform>(null, ReferenceUtils.CalculateThisReference(transformOffset, objectInSpace, "Transform"));
			//			//meshInSpace.Transform = ReferenceUtils.CreateReference<Transform>(null, ReferenceUtils.CalculateThisReference(meshInSpace, transformOffset, "Result"));
			//		}

			//		objectInSpace.Enabled = true;
			//	}
			//}
		}

		static void ImportGeometry( ImportContext context, Component parent, MeshData geom )
		{
			var geometry = parent.CreateComponent<Component_MeshGeometry>();
			geometry.Name = GetFixedName( geom.Name );

			CalcIndices.CalculateIndicesAndMergeEqualVertices( geom, out StandardVertex[] vertices, out int[] indices );
			//CalcIndices.CalculateIndicesBySpatialSort( geom, out StandardVertex[] vertices, out int[] indices );
			//CalcIndices.CalculateIndicesByOctree( m, out StandardVertexF[] verticesO, out int[] indicesO );

			geometry.SetVertexDataWithRemovingHoles( vertices, geom.VertexComponents );
			geometry.Indices = indices;

			//set first material
			context.materialByIndex.TryGetValue( 0, out var material );
			if( material != null )
				geometry.Material = ReferenceUtility.MakeRootReference( material );

		}
	}
}
