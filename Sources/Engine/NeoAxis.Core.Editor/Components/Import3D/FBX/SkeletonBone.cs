// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using Internal.Fbx;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Import.FBX
{
	class AnimTrack
	{
		public AnimationLayer[] Layers;
		//ToDo :??? BakeLayers как в FbxAnimStack.BakeLayers(), для этого в AnimationLayer добавить BlendMode, BlendWeight. (Animation layers это несколько соединяемых анимаций - например, идущий и бегущий меш, с возможностью плавного перехода)
	}

	class AnimationLayer
	{
		public string Name;
		//Keyframes for the properties LclTranslation,LclRotation,LclScaling of the bone node. If null the propery is not animated and a static value of the property must be used.
		public KeyFrame[] Transform;

		//BlendMode //ToDo : Сделать чтение BlendMode, и по нему соединение нескольких Layers.
	}

	struct KeyFrame
	{
		public double TimeInSeconds;
		public Matrix4 Value;

		public override string ToString()
		{
			return $"({TimeInSeconds} : {Value})";
		}
	}

	//Skin contains clusters. Cluster has the points with weights and a link to a skeleton bone of FbxNode type.
	class SkeletonBone
	{
		//The animation stack must have at least one animation layer known as the base layer.For blended animation, more than one animation layer is required.



		public FbxNode Node;
		public FbxAMatrix geometryOffset;

		public SceneLoader scene;
		//FbxScene scene;
		//Skeleton skeleton;
		public FbxCluster cluster;
		public FbxAMatrix globalMeshTransform;
		public AnimTrack[] AnimTracks;
		public Matrix4 InitialTransform;

		public List<SkeletonBone> Children = new List<SkeletonBone>();


		public string Name
		{
			get { return Node.GetName(); }
		}

		public SkeletonBone ParentBone { get; }

		//public int GetKeyFrameCount( int trackIndex ) { return animationTrackKeyFrames[ trackIndex ].Count; }

		//public double KTimeToFrameTime( FbxTime keyTime )
		//{
		//	return keyTime.GetSecondDouble() * frameRate;
		//}

		//public FbxTime FrameTimeToKTime( double frameTime )
		//{
		//	FbxTime kTime = new FbxTime();
		//	kTime.SetSecondDouble( frameTime / frameRate );
		//	return kTime;
		//}

		public FbxSkin skin;

		public SkeletonBone( FbxNode boneNode, SkeletonBone parentBone, FbxSkin[] skins, SceneLoader scene )
		{
			//this.scene = scene;
			this.scene = scene;
			Node = boneNode;
			ParentBone = parentBone;
			//animationTrackKeyFrames = new List<HashSet<double>>();
			cluster = Skeleton.FindCluster( Node, skins, out skin ); //Поиск по всем skins во всех mesh. Можно было бы сделать эффективнее.

			globalMeshTransform = new FbxAMatrix();
			globalMeshTransform.SetIdentity();
			geometryOffset = new FbxAMatrix();
			geometryOffset.SetIdentity();
			if( cluster != null )
			{
				FbxNode meshNode = skin.GetGeometry()?.GetNode();
				if( meshNode != null )
				{
					globalMeshTransform = meshNode.EvaluateGlobalTransform( FbxExtensions.ToFbxTime( -1 ) ); // FbxMath.GetGlobalPosition( meshNode, FbxExtensions.ToFbxTime( -1 ), null );
					geometryOffset = FbxMath.GetGeometryOffset( meshNode );
				}
			}
			InitialTransform = GetInitialTransform().ToMatrix4(); //Во внешнем коде надо использовать это значение, т.к. результат GetInitialTransform() может зависеть от установленного AnimationTrack
		}

		//public bool GetHasAnimation()
		//{
		//	if( AnimTracks == null || AnimTracks.Length == 0 )
		//		return false;
		//	foreach( var track in AnimTracks )
		//	{
		//		if(track.Layers == null || track.Layers.Length == 0)
		//			continue;
		//		foreach( var layer in track.Layers )
		//		{
		//			if( layer.Transform != null && layer.Transform.Length != 0 )
		//				return true;
		//		}
		//	}
		//	return false;
		//}


		static KeyFrame3TimeComparerClass keyFrameTimeComparer;
		static KeyFrame3TimeComparerClass KeyFrameTimeComparer
		{
			get
			{
				if( keyFrameTimeComparer == null )
					keyFrameTimeComparer = new KeyFrame3TimeComparerClass();
				return keyFrameTimeComparer;
			}
		}
		class KeyFrame3TimeComparerClass : IComparer<KeyFrame>
		{
			public int Compare( KeyFrame x, KeyFrame y )
			{
				return x.TimeInSeconds.CompareTo( y.TimeInSeconds );
			}
		}


		//ToDo : учет associate model?
		// GlobalTransform of the bone at the binding moment.
		//
		// (Comments from fbxcluster.h)
		// Transformation matrices. A link has three transformation matrices:
		// - Transform refers to the global initial transform of the geometry node(geometry is the attribute of the node) that contains the link node.
		// - TransformLink refers to global initial transform of the link(bone) node.
		// - TransformAssociateModel refers to the global initial transform of the associate model.
		//
		// For example, given a mesh binding with several bones(links), Transform is the global transform 
		// of the mesh at the binding moment, TransformLink is the global transform of the bone(link)
		// at the binding moment, TransformAssociateModel is the global transform of the associate model 
		// at the binding moment.

		FbxAMatrix GetInitialTransform()
		{
			if( cluster == null )
				return FbxMath.EvaluateGlobalTransform( Node );

			//Each cluster is associated with a FbxNode (bone)
			//cluster.GetLink().GetUniqueID() == Node.GetUniqueID();


			//??? Учитывать ли AxisSystem?
			//??? Не совсем понятны отличаи globalMeshTransform == skin.GetGeometry().GetNode().EvaluateGlobalTransform(), от cluster.GetTransformMatrix()

			//matrix associated with the geometry node containing the link. (global transform of the mesh at the binding moment)
			FbxAMatrix globalMeshInitialTransform = new FbxAMatrix();
			cluster.GetTransformMatrix( globalMeshInitialTransform );

			//Get matrix associated with the link node (bind pose,  bone transform) at the binding moment
			FbxAMatrix boneInitialTransform = new FbxAMatrix();
			cluster.GetTransformLinkMatrix( boneInitialTransform );
			//Дополнительная матрица которую cluster ассоциирует с Node - Трансформация в момент привязки.

			//bind pose matrix that is local for the mesh = Inverse(TransformMatrix) * TransformLinkMatrix
			FbxAMatrix localBindPose = ( globalMeshInitialTransform.mul( geometryOffset ).Inverse() ).mul( boneInitialTransform );

			//globalMeshTransform * Inverse(globalMeshInitialTransform) * boneInitialTransform
			return globalMeshTransform.mul( geometryOffset ).mul( localBindPose );
		}



		public AnimTrack LoadAnimationTrack( FbxNode node, AnimationTrackInfo animationTrack )
		{
			double[] times = LoadKeyTimes( node, animationTrack );
			var ret = new AnimTrack();
			ret.Layers = new AnimationLayer[ animationTrack.GetLayerCount() ];
			for( int i = 0; i < ret.Layers.Length; i++ )
				ret.Layers[ i ] = LoadLayer( node, animationTrack.GetLayer( i ), times );
			return ret;
		}

		const string XChannelName = "X";
		const string YChannelName = "Y";
		const string ZChannelName = "Z";

		static double[] LoadKeyTimes( FbxNode node, AnimationTrackInfo animationTrack )
		{
			HashSet<double> keyFrames = new HashSet<double>();

			LoadKeyFrames( animationTrack, node.LclTranslation, keyFrames, XChannelName, YChannelName, ZChannelName );
			LoadKeyFrames( animationTrack, node.LclRotation, keyFrames, XChannelName, YChannelName, ZChannelName );
			LoadKeyFrames( animationTrack, node.LclScaling, keyFrames, XChannelName, YChannelName, ZChannelName );

			return keyFrames.OrderBy( _ => _ ).ToArray();
		}

		//ToDo : ??? Только для одного Layer грузить? Хотя у Layers должны быть одинаковые отсчеты времени, если они должны смешиваться?
		static void LoadKeyFrames( AnimationTrackInfo animationTrack,
			FbxPropertyTFbxDouble3 transformProperty,
			HashSet<double> keyFrames, string xChannelName,
			string yChannelName, string zChannelName )
		{
			for( int i = 0; i < animationTrack.GetLayerCount(); i++ )
			{
				FbxAnimLayer pAnimLayer = animationTrack.GetLayer( i );
				FbxAnimCurve pAnimCurve = null;

				pAnimCurve = transformProperty.GetCurve( pAnimLayer, xChannelName );
				if( pAnimCurve != null )
				{
					for( int j = 0; j < pAnimCurve.KeyGetCount(); j++ )
					{
						FbxAnimCurveKey key = pAnimCurve.KeyGet( j );
						//ToDo :? Учитывать key.GetInterpolation();
						double time = key.GetTime().GetSecondDouble();
						keyFrames.Add( time );
					}
				}

				pAnimCurve = transformProperty.GetCurve( pAnimLayer, yChannelName );
				if( pAnimCurve != null )
				{
					for( int j = 0; j < pAnimCurve.KeyGetCount(); j++ )
					{
						FbxAnimCurveKey key = pAnimCurve.KeyGet( j );
						double time = key.GetTime().GetSecondDouble();
						keyFrames.Add( time );
					}
				}

				pAnimCurve = transformProperty.GetCurve( pAnimLayer, zChannelName );
				if( pAnimCurve != null )
				{
					for( int j = 0; j < pAnimCurve.KeyGetCount(); j++ )
					{
						FbxAnimCurveKey key = pAnimCurve.KeyGet( j );
						double time = key.GetTime().GetSecondDouble();
						keyFrames.Add( time );
					}
				}
			}
		}

		AnimationLayer LoadLayer( FbxNode node, FbxAnimLayer animLayer, double[] keyTimes )
		{
			//ToDo : animLayer.BlendMode
			var ret = new AnimationLayer();
			ret.Name = animLayer.GetName();
			ret.Transform = new KeyFrame[ keyTimes.Length ];
			for( int i = 0; i < keyTimes.Length; i++ )
			{
				var t = new FbxTime();
				t.SetSecondDouble( keyTimes[ i ] );
				var transform = node.EvaluateLocalTransform( t );
				ret.Transform[ i ] = new KeyFrame { TimeInSeconds = keyTimes[ i ], Value = transform.ToMatrix4() };
			}
			return ret;
		}

		//static KeyFrame3[] LoadAnimCurve(FbxAnimLayer animLayer, FbxPropertyTFbxDouble3 transformProperty, string channelName)
		//{
		//	FbxAnimCurve animCurve = transformProperty.GetCurve(animLayer, channelName);
		//	if (animCurve == null)
		//		return null;
		//	int count = animCurve.KeyGetCount();
		//	if (count == 0)
		//		return null;
		//	var ret = new KeyFrame[count];
		//	for (int i = 0; i < count; i++)
		//	{
		//		FbxAnimCurveKey key = animCurve.KeyGet(i);
		//		ret[i] = new KeyFrame {TimeInSeconds = key.GetTime().GetSecondDouble(), Value = key.GetValue()};
		//	}
		//	return ret;
		//}


		//public double GetKeyFrameTime( int trackIndex, int frameIndex )
		//{
		//	HashSet<double> keyFrames = animationTrackKeyFrames[ trackIndex ];
		//	int i = 0;
		//	foreach( var d in keyFrames )
		//	{
		//		if( i < frameIndex )
		//			i++;
		//		else
		//			return d;
		//	}
		//	return 0;
		//}

		//public FbxMatrix GetTransform( int trackIndex, double time )
		//{
		//	scene.SetCurrentAnimationTrack( trackIndex );
		//	FbxTime fbxTime = new FbxTime();
		//	fbxTime.SetSecondDouble(time);

		//	if( cluster != null )
		//		return FbxMath.GetGlobalPosition( cluster.GetLink(), fbxTime, null ).ToFbxMatrix();
		//	else
		//		return FbxMath.GetGlobalPosition( Node, fbxTime, null ).ToFbxMatrix(); //??? FbxPose? Без FbxPose эквивалетно node.EvaluateGlobalTransform
		//}

	}
}

//Comments from FBX SDK, FbxCluster.h  : 
// FbxCluster - Class for clusters (links). 
//* A cluster, or link, is an entity acting on a geometry (FbxGeometry).
//* More precisely, the cluster acts on a subset of the geometry's control points.
//* For each control point that the cluster acts on, the intensity of the cluster's
//* action is modulated by a weight. The link mode (ELinkMode) specifies how
//* the weights are taken into account.
//*
//* The cluster's link node specifies the node (FbxNode) that influences the
//* control points of the cluster. If the node is animated, the control points
//* will move accordingly.
//*
//* A cluster is usually part of a skin (\see FbxDeformer, FbxSkin). For example,
//* imagine a mesh representing a humanoid, and imagine a skeleton made of bones. 
//* Each bone is represented by a node in FBX.
//* To bind the geometry to the nodes, 
//* we create a skin (FbxSkin). The skin has many clusters, each one corresponding
//* to a bone.
//* Each node influences some control
//* points of the mesh. A node has a high influence on some of the points (high weight)
//* and lower influence on some other points (low weight). Some points of the mesh
//* are not affected at all by the bone, so they would not be part of the corresponding
//* cluster.
//*


//Два метода сохранения информации об анимации:
//1) Получать информацию об анимации для отдельных property LocalTranslation, LocalRotation, LocalScaling, из них вычислять bones в разные моменты времени.
//минусы: Возможно, может потребоваться воспроизводить большую часть алгоритма FbxMath.cs: EvaluateGlobalTransform.
//       И для него сохранять(в NeoAxis) много property: pre/post rotation, pivots,... (много их), AxisSystem(для ForceFrontxAxis). 
//       Даже если они не все анимированные, могут понадобиться для вычисления CalculateGlobalTransform для bone.
//2) Получить сразу GlobalTransform(или Local, который FBX вычисляет через Global для parent) для всех keyframes через FbxNode.EvaluateGlobalTransform 
//   и уже потом разделить на translation, rotation и сохранить эти данные. 
//
// Используется второй метод для LclTranslation, LclRotation, LclScaling. Но если будут еще анимированные property координат, например RotationPivot (маловероятно что такое будут анимировать), 
// тогда надо для него добавить отсчеты времени (иначе пропадут это промежуточные точки) .
