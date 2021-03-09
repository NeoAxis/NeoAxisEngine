// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
using System.Linq;
using Fbx;

namespace NeoAxis.Import.FBX
{
	class ImportFBX : ImportGeneral
	{
		static bool loadedNativeLibrary;

		//

		public static void LoadNativeLibrary()
		{
			if( !loadedNativeLibrary )
			{
				NativeLibraryManager.PreLoadLibrary( "FbxWrapperNative" );
				loadedNativeLibrary = true;
			}
		}

		class ImportContext
		{
			public FbxManager manager;
			public FbxScene scene;
			public Settings settings;
			public string directoryName;
			public Component materialsGroup;
			public EDictionary<int, Component_Material> materialByIndex = new EDictionary<int, Component_Material>();
			public Component_Material defaultMaterial;
		}

		static bool HasTransformMatrixNegParity( Matrix3F m )
		{
			return Vector3F.Dot( Vector3F.Cross( m.Item0, m.Item1 ), m.Item2 ) < 0.0f ? true : false;
		}

		//static bool ContainsMeshesRecursive( Node node )
		//{
		//	if( node.HasMeshes )
		//		return true;
		//	foreach( var child in node.Children )
		//	{
		//		if( ContainsMeshesRecursive( child ) )
		//			return true;
		//	}
		//	return false;
		//}

		public static void DoImport( Settings settings, out string error )
		{
			error = "(NO ERROR MESSAGE)";
			LoadNativeLibrary();

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
				var realFileName = VirtualPathUtility.GetRealPathByVirtual( settings.virtualFileName );
				//VirtualFileStream stream = null;
				//ToDo : FromStream
				bool status;
				if( !string.IsNullOrEmpty( realFileName ) && File.Exists( realFileName ) )
				{
					status = importer.Initialize( realFileName, -1, setting );
				}
				else
				{
					error = "File is not exists.";
					return;
					//throw new NotImplementedException();
					//ToDo : ....
					//stream = VirtualFile.Open( settings.virtualFileName );
					//FbxStream fbxStream = null;
					//SWIGTYPE_p_void streamData = null;

					//status = impoter.Initialize( fbxStream, streamData, -1, setting );
				}

				if( !status )
					return;

				scene = FbxScene.Create( manager, "scene1" );
				status = importer.Import( scene );
				if( !status )
					return;

				error = "";
				var importContext = new ImportContext();
				importContext.manager = manager;
				importContext.scene = scene;
				importContext.settings = settings;
				importContext.directoryName = Path.GetDirectoryName( settings.virtualFileName );

				ImportScene( importContext );

				////create meshes (Scene mode)
				//if( settings.component.Mode.Value == Component_Import3D.ModeEnum.Scene /*&& scene.HasMeshes && scene.MeshCount != 0 */)
				//{
				//	importContext.meshesGroup = settings.component.GetComponentByName( "Meshes" ) as Component_Mesh;
				//	if( importContext.meshesGroup == null )
				//	{
				//		importContext.meshesGroup = settings.component.CreateComponent<Component>( -1, false );
				//		importContext.meshesGroup.Name = "Meshes";
				//	}
				//	else
				//		importContext.meshesGroup.Enabled = false;
				//}

				////enable groups
				//if( importContext.meshesGroup != null )
				//	importContext.meshesGroup.Enabled = true;
				//if( importContext.sceneObjectsGroup != null )
				//	importContext.sceneObjectsGroup.Enabled = true;

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
		}

		static void ImportScene( ImportContext context )
		{
			var settings = context.settings;
			var scene = context.scene;

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

			//get materials data
			var materialsData = GetMaterialsData( context );

			//create Materials group
			context.materialsGroup = context.settings.component.GetComponent( "Materials" );
			if( context.materialsGroup == null /*&& materialsData.Count != 0*/ && settings.updateMaterials )
			{
				context.materialsGroup = context.settings.component.CreateComponent<Component>();
				context.materialsGroup.Name = "Materials";
			}

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

			//-------------------------

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

			var sceneLoader = new SceneLoader();

			var mode = settings.component.Mode.Value;
			if( mode == Component_Import3D.ModeEnum.Auto )
				mode = Component_Import3D.ModeEnum.OneMesh;

			//create one mesh (OneMesh mode)
			if( mode == Component_Import3D.ModeEnum.OneMesh && settings.updateMeshes )
			{
				sceneLoader.Load( scene, context.manager, options, additionalTransform );
				Component_Skeleton skeletonComponent = CreateSkeletonComponent( context, sceneLoader.Skeleton, out int[] newIndexFromOldIndex, out SkeletonBone[] oldBoneFromNewIndex, additionalTransform );

				var mesh = settings.component.CreateComponent<Component_Mesh>( enabled: false );
				mesh.Name = "Mesh";

				//!!!!пока меши не индексируются/не инстансятся.
				foreach( var geom in sceneLoader.Geometries )
					ImportGeometry( context, mesh, geom, newIndexFromOldIndex );

				if( skeletonComponent != null )
				{
					mesh.AddComponent( skeletonComponent );
					mesh.Skeleton = ReferenceUtility.MakeThisReference( mesh, skeletonComponent );
					InitAnimations( context, sceneLoader, mesh, oldBoneFromNewIndex, additionalTransform );
				}

				if( settings.component.MergeMeshGeometries )
					mesh.MergeGeometriesWithEqualVertexStructureAndMaterial();

				mesh.Enabled = true;
			}

			//create meshes, object in space (Meshes mode)
			if( mode == Component_Import3D.ModeEnum.Meshes /*&& scene.HasMeshes && scene.MeshCount != 0*/ )
			{
				sceneLoader.Load( scene, context.manager, options, additionalTransform );
				Component_Skeleton skeletonComponent = CreateSkeletonComponent( context, sceneLoader.Skeleton, out int[] newIndexFromOldIndex, out SkeletonBone[] oldBoneFromNewIndex, additionalTransform );

				var meshesGroup = settings.component.GetComponent( "Meshes" );

				//Meshes
				if( settings.updateMeshes )
				{
					meshesGroup = settings.component.CreateComponent<Component>( enabled: false );
					meshesGroup.Name = "Meshes";

					//ToDo : Сейчас все Geometry как отдельные Mesh, выяснить могут ли в FBX Geometry быть дочерние?
					foreach( var geom in sceneLoader.Geometries )
					{
						var mesh = meshesGroup.CreateComponent<Component_Mesh>();
						mesh.Name = GetFixedName( geom.Name );
						ImportGeometry( context, mesh, geom, newIndexFromOldIndex );

						//if (mesh.Components.Count != 0)
						//	mesh.Name = mesh.Components.ToArray()[0].Name;
						//else
						//		mesh.Dispose();

						if( settings.component.MergeMeshGeometries )
							mesh.MergeGeometriesWithEqualVertexStructureAndMaterial();
					}

					//ToDo :? Skeleton добавлять в Meshes ? 
					//if (skeletonComponent != null)
					//{
					//	meshesGroup.AddComponent(skeletonComponent);
					//	meshesGroup.Skeleton = ReferenceUtils.MakeThisReference(meshesGroup, skeletonComponent);
					//	InitAnimations(sceneLoader, meshesGroup, oldBoneFromNewIndex);
					//}
					meshesGroup.Enabled = true;
				}

				//Object In Space
				if( settings.updateObjectsInSpace && meshesGroup != null )
				{
					var objectInSpace = settings.component.CreateComponent<Component_ObjectInSpace>( enabled: false );
					objectInSpace.Name = "Object In Space";

					foreach( var mesh in meshesGroup.Components )
					{
						var meshInSpace = objectInSpace.CreateComponent<Component_MeshInSpace>();
						meshInSpace.Name = mesh.Name;
						meshInSpace.CanBeSelected = false;
						meshInSpace.Mesh = ReferenceUtility.MakeReference<Component_Mesh>( null, ReferenceUtility.CalculateRootReference( mesh ) );

						//Transform
						//!!!!transform?
						var pos = Vector3.Zero;
						var rot = Quaternion.Identity;
						var scl = Vector3.One;
						//( globalTransform * node.Transform.ToMat4() ).Decompose( out var pos, out Quat rot, out var scl );

						var transformOffset = meshInSpace.CreateComponent<Component_TransformOffset>();
						transformOffset.Name = "Transform Offset";
						transformOffset.PositionOffset = pos;
						transformOffset.RotationOffset = rot;
						transformOffset.ScaleOffset = scl;

						transformOffset.Source = ReferenceUtility.MakeThisReference( transformOffset, objectInSpace, "Transform" );
						meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, transformOffset, "Result" );
						//transformOffset.Source = ReferenceUtils.CreateReference<Transform>(null, ReferenceUtils.CalculateThisReference(transformOffset, objectInSpace, "Transform"));
						//meshInSpace.Transform = ReferenceUtils.CreateReference<Transform>(null, ReferenceUtils.CalculateThisReference(meshInSpace, transformOffset, "Result"));
					}

					objectInSpace.Enabled = true;
				}
			}


			////create meshes, object in space (Meshes mode)
			//if( settings.component.Mode.Value == Component_Import3D.ModeEnum.Meshes /*&& scene.HasMeshes && scene.MeshCount != 0*/ )
			//{
			//	var meshesGroup = settings.component.GetComponent( "Meshes" );

			//	//Meshes
			//	if( settings.updateMeshes )
			//	{
			//		meshesGroup = settings.component.CreateComponent<Component>( enable: false );
			//		meshesGroup.Name = "Meshes";

			//		//!!!!правильно ли работает? может как-то иначе в случае FBX на меши делить

			//		for( int i = 0; i < rootNode.GetChildCount(); i++ )
			//		{
			//			var node = rootNode.GetChild( i );

			//			//!!!!transform?

			//			var mesh = meshesGroup.CreateComponent<Component_Mesh>();
			//			InitMeshGeometriesRecursive( importContext, node, additionalTransform, mesh );

			//			if( mesh.Components.Count != 0 )
			//			{
			//				mesh.Name = mesh.Components.ToArray()[ 0 ].Name;

			//				//!!!!transform?

			//				if( settings.component.MergeMeshGeometries )
			//					mesh.MergeGeometriesWithEqualVertexStructureAndMaterial();
			//			}
			//			else
			//				mesh.Dispose();
			//		}

			//		meshesGroup.Enabled = true;
			//	}

			//	//Object In Space
			//	if( settings.updateObjectsInSpace && meshesGroup != null )
			//	{
			//		var objectInSpace = settings.component.CreateComponent<Component_ObjectInSpace>( enable: false );
			//		objectInSpace.Name = "Object In Space";

			//		foreach( var mesh in meshesGroup.Components )
			//		{
			//			var meshInSpace = objectInSpace.CreateComponent<Component_MeshInSpace>();
			//			meshInSpace.Name = mesh.Name;
			//			meshInSpace.CanBeSelected = false;
			//			meshInSpace.Mesh = ReferenceUtils.CreateReference<Component_Mesh>( null, ReferenceUtils.CalculateRootReference( mesh ) );

			//			//Transform
			//			//!!!!transform?
			//			var pos = Vec3.Zero;
			//			var rot = Quat.Identity;
			//			var scl = Vec3.One;
			//			//( globalTransform * node.Transform.ToMat4() ).Decompose( out var pos, out Quat rot, out var scl );

			//			var transformOffset = meshInSpace.CreateComponent<Component_TransformOffset>();
			//			transformOffset.Name = "Transform Offset";
			//			transformOffset.PositionOffset = pos;
			//			transformOffset.RotationOffset = rot;
			//			transformOffset.ScaleOffset = scl;
			//			transformOffset.Source = ReferenceUtils.CreateReference<Transform>( null,
			//				ReferenceUtils.CalculateThisReference( transformOffset, objectInSpace, "Transform" ) );

			//			meshInSpace.Transform = ReferenceUtils.CreateReference<Transform>( null,
			//				ReferenceUtils.CalculateThisReference( meshInSpace, transformOffset, "Result" ) );
			//		}

			//		objectInSpace.Enabled = true;
			//	}
			//}

		}

		static void ImportGeometry( ImportContext importContext, Component parent, MeshData geom, int[] newIndexFromOldIndex )
		{
			var geometry = parent.CreateComponent<Component_MeshGeometry>();
			geometry.Name = ImportGeneral.GetFixedName( geom.Name );

			CalcIndices.CalculateIndicesAndMergeEqualVertices( geom, out StandardVertex[] vertices, out int[] indices );
			//CalcIndices.CalculateIndicesBySpatialSort( geom, out StandardVertex[] vertices, out int[] indices );
			//CalcIndices.CalculateIndicesByOctree( m, out StandardVertexF[] verticesO, out int[] indicesO );

			if( newIndexFromOldIndex != null )
			{
				for( int i = 0; i < vertices.Length; i++ )
				{
					ref Vector4I bi = ref vertices[ i ].BlendIndices;
					if( bi.X != -1 )
						bi.X = newIndexFromOldIndex[ bi.X ];
					if( bi.Y != -1 )
						bi.Y = newIndexFromOldIndex[ bi.Y ];
					if( bi.Z != -1 )
						bi.Z = newIndexFromOldIndex[ bi.Z ];
					if( bi.W != -1 )
						bi.W = newIndexFromOldIndex[ bi.W ];
				}
			}
			geometry.SetVertexDataWithRemovingHoles( vertices, geom.VertexComponents );
			geometry.Indices = indices;

			importContext.materialByIndex.TryGetValue( geom.MaterialIndex, out Component_Material material );

			if( material == null )
			{
				if( importContext.settings.updateMaterials )
				{
					if( importContext.defaultMaterial == null && importContext.materialsGroup != null )
					{
						importContext.defaultMaterial = importContext.materialsGroup.CreateComponent<Component_Material>();
						importContext.defaultMaterial.Name = "Default";
					}
					geometry.Material = importContext.defaultMaterial;
				}
			}

			if( material != null )
				geometry.Material = ReferenceUtility.MakeRootReference( material );
		}

		//newIndexFromOld - an array mapping from old bone indices to a new : ret[oldIndex]==newIndex
		static Component_Skeleton CreateSkeletonComponent( ImportContext importContext, Skeleton skeleton, out int[] newIndexFromOldIndex, out SkeletonBone[] oldBoneFromNewIndex, Matrix4 additionalTransform )
		{
			newIndexFromOldIndex = null;
			oldBoneFromNewIndex = null;
			if( skeleton == null )
				return null;

			var skeletonComponent = new Component_Skeleton();
			skeletonComponent.Name = "Skeleton";

			var oldBones = new Dictionary<Component_SkeletonBone, SkeletonBone>();
			foreach( var firstLevelBone in skeleton.RootBone.Children )
				InitBoneRecursive( skeletonComponent, firstLevelBone, skeleton, oldBones, additionalTransform );

			var allBones = skeletonComponent.GetBones(); //contains information about new bone indices
			int maxOldIndex = oldBones.Count == 0 ? -1 : oldBones.Values.Select( _ => skeleton.GetBoneIndexByNode( _.Node ) ).Max();
			newIndexFromOldIndex = new int[ maxOldIndex + 1 ];
			for( int i = 0; i < newIndexFromOldIndex.Length; i++ )
				newIndexFromOldIndex[ i ] = -1;
			for( int newIndex = 0; newIndex < allBones.Length; newIndex++ )
			{
				var bone = oldBones[ allBones[ newIndex ] ];
				newIndexFromOldIndex[ skeleton.GetBoneIndexByNode( bone.Node ) ] = newIndex;
			}

			oldBoneFromNewIndex = new SkeletonBone[ allBones.Length ];
			for( int boneIndex = 0; boneIndex < oldBoneFromNewIndex.Length; boneIndex++ )
				oldBoneFromNewIndex[ boneIndex ] = oldBones[ allBones[ boneIndex ] ];

			return skeletonComponent;
		}

		static void InitAnimations( ImportContext importContext, SceneLoader sceneLoader, Component_Mesh parentComponent, SkeletonBone[] oldBonesFromNewIndex, Matrix4 additionalTransform )
		{
			var animationsComponent = parentComponent.CreateComponent<Component>();
			animationsComponent.Name = "Animations";

			for( int trackIndex = 0; trackIndex < sceneLoader.AnimationTracks.Count; trackIndex++ )
			{
				var animationTrack = sceneLoader.AnimationTracks[ trackIndex ];
				for( int layerIndex = 0; layerIndex < animationTrack.GetLayerCount(); layerIndex++ )
				{
					var skeletonAnimationTrackComponent = animationsComponent.CreateComponent<Component_SkeletonAnimationTrack>();

					var name = animationTrack.Name;
					//add prefix to name
					var prefix = "Track ";
					if( name.Length < prefix.Length || name.Substring( 0, prefix.Length ) != prefix )
						name = ( prefix + name ).Trim();
					skeletonAnimationTrackComponent.Name = name;

					var skeletonAnimationComponent = animationsComponent.CreateComponent<Component_SkeletonAnimation>();
					skeletonAnimationComponent.Name = string.IsNullOrEmpty( animationTrack.Name ) ? "Animation" : animationTrack.Name;

					var trackData = new List<Component_SkeletonAnimationTrack.KeyFrame>();
					for( int boneIndex = 0; boneIndex < oldBonesFromNewIndex.Length; boneIndex++ )
					{
						var bone = oldBonesFromNewIndex[ boneIndex ];
						//int oldIndex = skeleton.GetBoneIndexByNode(bone.Node);

						KeyFrame[] keyframes = bone.AnimTracks[ trackIndex ].Layers[ layerIndex ].Transform;
						if( keyframes == null || keyframes.Length == 0 )
						{
							keyframes = new KeyFrame[ 1 ];
							//if no animation in that case the one frame with double.NaN time.
							var transform = bone.Node.EvaluateLocalTransform().ToMatrix4();
							if( bone.ParentBone?.ParentBone == null ) // additionalTransform is applied only to a root bone. But the root bone is skipped, so the child of the root becomes a new root
								transform = additionalTransform * transform;
							keyframes[ 0 ] = new KeyFrame { TimeInSeconds = double.NaN, Value = transform };
						}

						for( int i = 0; i < keyframes.Length; i++ )
						{
							var transform = keyframes[ i ].Value;
							if( bone.ParentBone?.ParentBone == null ) // additionalTransform is applied only to a root bone. But the root bone is skipped, so the child of the root becomes a new root
								transform = additionalTransform * transform;
							transform.Decompose( out Vector3 t, out Quaternion r, out Vector3 s );
							trackData.Add( new Component_SkeletonAnimationTrack.KeyFrame
							{
								BoneIndex = boneIndex,
								Position = t.ToVector3F(),
								Rotation = r.ToQuaternionF(),
								Scale = s.ToVector3F(), //Scale нельзя умножать на importContext.settings.component.Scale.Value ?
								Time = (float)keyframes[ i ].TimeInSeconds
							} );
						}
					}

					//find min,max time and fill float.NaN time (for non animated bones with single keyframe) with minTime.
					float minTime = float.PositiveInfinity;
					float maxTime = float.NegativeInfinity;
					bool empty = true;
					for( int i = 0; i < trackData.Count; i++ )
					{
						float t = trackData[ i ].Time;
						if( float.IsNaN( t ) ) //no animation - static key frame
							continue;
						empty = false;
						if( t < minTime )
							minTime = t;
						if( maxTime < t )
							maxTime = t;
					}
					if( empty )
					{
						maxTime = 0;
						minTime = 0;
					}
					for( int i = 0; i < trackData.Count; i++ )
					{
						if( float.IsNaN( trackData[ i ].Time ) )
						{
							var e = trackData[ i ];
							e.Time = minTime;
							trackData[ i ] = e;
						}
					}

					skeletonAnimationTrackComponent.KeyFrames = Component_SkeletonAnimationTrack.ToBytes( trackData );
					skeletonAnimationComponent.Track = ReferenceUtility.MakeThisReference( skeletonAnimationComponent, skeletonAnimationTrackComponent );

					skeletonAnimationComponent.TrackStartTime = minTime;
					skeletonAnimationComponent.Length = maxTime - minTime;
				}
			}
		}

		static void InitBoneRecursive( Component parentComponent, SkeletonBone bone, Skeleton skeleton, Dictionary<Component_SkeletonBone, SkeletonBone> oldBones, Matrix4 additionalTransform )
		{
			var boneComponent = parentComponent.CreateComponent<Component_SkeletonBone>();
			boneComponent.Name = bone.Name;
			var transform = bone.InitialTransform;
			transform = additionalTransform * transform;
			transform.Decompose( out Vector3 translation, out Quaternion rotation, out Vector3 scale );
			var initialTransform = new Transform( translation, rotation, scale );
			boneComponent.Transform = initialTransform;
			oldBones[ boneComponent ] = bone;

			foreach( var childBone in bone.Children )
				InitBoneRecursive( boneComponent, childBone, skeleton, oldBones, additionalTransform );
		}

		/////////////////////////////////////////
		// Materials data

		static List<MaterialData> GetMaterialsData( ImportContext importContext )
		{
			var result = new List<MaterialData>();

			var scene = importContext.scene;
			for( int nMaterial = 0; nMaterial < scene.GetMaterialCount(); nMaterial++ )
			{
				var fbxMaterial = scene.GetMaterial( nMaterial );

				var data = new MaterialData();
				data.Index = nMaterial;
				data.Name = GetFixedName( fbxMaterial.GetName() );

				try
				{
					//FbxSurfaceLambert
					if( fbxMaterial.GetRuntimeClassId().Is( FbxSurfaceLambert.ClassId ) )
					{
						var lMaterial = FbxSurfaceLambert.Cast( fbxMaterial );


						//data.BaseColor = new ColorValue( lMaterial.Diffuse.Get().ToVector3() );



						//need improve SWIG
						//physically based material data?
						//https://stackoverflow.com/questions/19634369/read-texture-filename-from-fbx-with-fbx-sdk-c



						//TwoSided

						//Opacity

						//lMaterial.Diffuse.Get().ToDouble2()

						//diffuse color

						//material.Opacity = 1.0 - lMaterial.TransparencyFactor.Get(); //TransparencyFactor==1 when transparent

						//FbxFileTexture.ClassId


						//var property = lMaterial.FindProperty( FbxSurfaceMaterial.sDiffuse );
						//if( property.IsValid() )
						//{
						//	int srcObjectCount = property.GetSrcObjectCount();// FbxFileTexture );

						//	for( int n = 0; n < srcObjectCount; n++ )
						//	{
						//		var fbxObject = property.GetSrcObject( n );

						//		//var fileTexture = fbxObject as FbxFileTexture;
						//		//if( fileTexture != null )
						//		//{
						//		//	var fileName = fileTexture.GetFileName();

						//		//	Log.Info( fileName );

						//		//}
						//	}
						//}


						//const FbxProperty lProperty = pMaterial->FindProperty( pPropertyName );

						//if( lProperty.IsValid() )
						//{
						//	const int lTextureCount = lProperty.GetSrcObjectCount<FbxFileTexture>();
						//	if( lTextureCount )
						//	{
						//		//!!!!new: 0?
						//		const FbxFileTexture* lTexture = lProperty.GetSrcObject<FbxFileTexture>( 0 );
						//		textureName = std::string( lTexture->GetFileName() );
						//		return true;
						//	}
						//}

						//return false;

						////blending
						//if( fbxMaterial.BlendMode == BlendMode.Additive )
						//{
						//	block.SetAttribute( "blending", "AlphaAdd" );
						//}
						//else
						//{
						//	if( aiMaterial.Opacity < 1 )
						//		block.SetAttribute( "blending", "AlphaBlend" );
						//}

						////specular color
						//if( aiMaterial.HasColorSpecular )
						//{
						//	ColorValue scale = aiMaterial.ColorSpecular.ToColorValue();
						//	if( scale != new ColorValue( 0, 0, 0 ) )
						//	{
						//		float power = Math.Max( Math.Max( scale.Red, scale.Green ), scale.Blue );
						//		if( power > 1 )
						//		{
						//			scale.Red /= power;
						//			scale.Green /= power;
						//			scale.Blue /= power;
						//			block.SetAttribute( "specularPower", power.ToString() );
						//		}
						//		block.SetAttribute( "specularColor", scale.ToString() );

						//		//specular shininess
						//		float shininess = aiMaterial.ShininessStrength * 20;
						//		block.SetAttribute( "specularShininess", shininess.ToString() );
						//	}
						//}

						////emission color
						//if( aiMaterial.HasColorEmissive )
						//{
						//	ColorValue scale = aiMaterial.ColorEmissive.ToColorValue();
						//	if( scale != new ColorValue( 0, 0, 0 ) )
						//	{
						//		float power = Math.Max( Math.Max( scale.Red, scale.Green ), scale.Blue );
						//		if( power > 1 )
						//		{
						//			scale.Red /= power;
						//			scale.Green /= power;
						//			scale.Blue /= power;
						//			block.SetAttribute( "emissionPower", power.ToString() );
						//		}
						//		block.SetAttribute( "emissionColor", scale.ToString() );
						//	}
						//}

					}
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
				}

				result.Add( data );
			}

			return result;
		}
	}
}
