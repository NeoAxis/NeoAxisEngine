// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	// DualQuaternionSkinning не поддерживает scaling, поэтому если есть scaling то используется LBS :
	// - В Ogre не нашлось обработки scale с DualQuaternionSkinning
	// - Статьи о проблемах scaling:
	//    http://rodolphe-vaillant.fr/?e=78
	//    https://disney-animation.s3.amazonaws.com/uploads/production/publication_asset/98/asset/dualQ.pdf
	//

	/// <summary>
	/// An animation controller for the mesh in space.
	/// </summary>
	/// <remarks>
	/// Implemented linear skinning algorithm.
	/// </remarks>
	public class Component_MeshInSpaceAnimationController : Component//, Component_MeshInSpace.IMeshInSpaceChild//Component_MeshInSpaceController
	{
		//double currentAnimationTime;
		double currentEngineTime;

		bool needResetToOriginalMesh;
		bool needRecreateModifiableMesh;

		double calculatedForTime { get; set; } = -1;
		Component_SkeletonBone[] bones;
		Component_SkeletonAnimationTrack.CalculateBoneTransformsItem[] boneTransforms;
		BoneGlobalTransformItem[] boneGlobalTransforms;
		Matrix4F[] transformMatrixRelativeToSkin; //transform from bind pose to a current pose

		//!!!!disable DQS
		//Component_SkeletonAnimationTrack.CalculateBoneTransformsItem[] transformRelativeToSkin;
		//bool hasScale { get; set; }

		/////////////////////////////////////////

		[Browsable( false )]
		public Component_MeshInSpace ParentMeshInSpace
		{
			get { return Parent as Component_MeshInSpace; }
		}

		/// <summary>
		/// The animation used by the controller.
		/// </summary>
		public Reference<Component_Animation> PlayAnimation
		{
			get { if( _playAnimation.BeginGet() ) PlayAnimation = _playAnimation.Get( this ); return _playAnimation.value; }
			set
			{
				if( _playAnimation.BeginSet( ref value ) )
				{
					try
					{
						PlayAnimationChanged?.Invoke( this );

						ResetSkeletonAndAnimation( _playAnimation.value.Value == null );

						if( _playAnimation.value.Value != null )
						{
							var state = new AnimationStateClass();
							state.Animations.Add( new AnimationStateClass.AnimationItem() { Animation = _playAnimation.value, Speed = Speed, AutoRewind = _autoRewind.value } );
							//state.Animations = new[] { new AnimationStateClass.AnimationItem() { Animation = _playAnimation.value, Speed = Speed, AutoRewind = _autoRewind.value } };
							SetAnimationState( state, false );
						}
						else
							SetAnimationState( null, false );

						//if( EnabledInHierarchy )
						//	ModifiableMesh_CreateDestroy();
					}
					finally { _playAnimation.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="PlayAnimation"/> property value changes.</summary>
		public event Action<Component_MeshInSpaceAnimationController> PlayAnimationChanged;
		ReferenceField<Component_Animation> _playAnimation;

		/// <summary>
		/// Animation speed multiplier.
		/// </summary>
		[Range( 0.0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[DefaultValue( 1.0 )]
		public Reference<double> Speed
		{
			get { if( _speed.BeginGet() ) Speed = _speed.Get( this ); return _speed.value; }
			set
			{
				if( _speed.BeginSet( ref value ) )
				{
					try
					{
						SpeedChanged?.Invoke( this );

						var playAnimation = PlayAnimation.Value;
						if( playAnimation != null )
						{
							var state = new AnimationStateClass();
							state.Animations.Add( new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = _speed.value, AutoRewind = AutoRewind } );
							//state.Animations = new[] { new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = _speed.value, AutoRewind = AutoRewind } };
							SetAnimationState( state, false );
						}
					}
					finally { _speed.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Speed"/> property value changes.</summary>
		public event Action<Component_MeshInSpaceAnimationController> SpeedChanged;
		ReferenceField<double> _speed = 1.0;

		/// <summary>
		/// Whether to rewind to the start when playing ended.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AutoRewind
		{
			get { if( _autoRewind.BeginGet() ) AutoRewind = _autoRewind.Get( this ); return _autoRewind.value; }
			set
			{
				if( _autoRewind.BeginSet( ref value ) )
				{
					try
					{
						AutoRewindChanged?.Invoke( this );

						var playAnimation = PlayAnimation.Value;
						if( playAnimation != null )
						{
							var state = new AnimationStateClass();
							state.Animations.Add( new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = Speed, AutoRewind = _autoRewind.value } );
							//state.Animations = new[] { new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = Speed, AutoRewind = _autoRewind.value } };
							SetAnimationState( state, false );
						}
					}
					finally { _autoRewind.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AutoRewind"/> property value changes.</summary>
		public event Action<Component_MeshInSpaceAnimationController> AutoRewindChanged;
		ReferenceField<bool> _autoRewind = true;

		[Browsable( false )]
		public AnimationStateClass AnimationState { get { return animationState; } }
		AnimationStateClass animationState;

		/// <summary>
		/// The skeleton used by the controller.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Skeleton> ReplaceSkeleton
		{
			get { if( _replaceSkeleton.BeginGet() ) ReplaceSkeleton = _replaceSkeleton.Get( this ); return _replaceSkeleton.value; }
			set
			{
				if( _replaceSkeleton.BeginSet( ref value ) )
				{
					try
					{
						ReplaceSkeletonChanged?.Invoke( this );

						ResetSkeletonAndAnimation( false );//?? false?

						var playAnimation = PlayAnimation.Value;
						if( playAnimation != null )
						{
							var state = new AnimationStateClass();
							state.Animations.Add( new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = Speed, AutoRewind = _autoRewind.value } );
							//state.Animations = new[] { new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = Speed, AutoRewind = _autoRewind.value } };
							SetAnimationState( state, false );
						}
						else
							SetAnimationState( null, false );
					}
					finally { _replaceSkeleton.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReplaceSkeleton"/> property value changes.</summary>
		public event Action<Component_MeshInSpaceAnimationController> ReplaceSkeletonChanged;
		ReferenceField<Component_Skeleton> _replaceSkeleton;

		[DefaultValue( 0.2 )]
		[Range( 0, 0.5 )]
		public Reference<double> InterpolationTime
		{
			get { if( _interpolationTime.BeginGet() ) InterpolationTime = _interpolationTime.Get( this ); return _interpolationTime.value; }
			set { if( _interpolationTime.BeginSet( ref value ) ) { try { InterpolationTimeChanged?.Invoke( this ); } finally { _interpolationTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InterpolationTime"/> property value changes.</summary>
		public event Action<Component_MeshInSpaceAnimationController> InterpolationTimeChanged;
		ReferenceField<double> _interpolationTime = 0.2;

		/// <summary>
		/// Whether to display the skeleton.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> DisplaySkeleton
		{
			get { if( _displaySkeleton.BeginGet() ) DisplaySkeleton = _displaySkeleton.Get( this ); return _displaySkeleton.value; }
			set { if( _displaySkeleton.BeginSet( ref value ) ) { try { DisplaySkeletonChanged?.Invoke( this ); } finally { _displaySkeleton.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplaySkeleton"/> property value changes.</summary>
		public event Action<Component_MeshInSpaceAnimationController> DisplaySkeletonChanged;
		ReferenceField<bool> _displaySkeleton = false;

		///// <summary>
		///// Overrides default skin deformation algorithm.
		///// </summary>
		//[DefaultValue( Component_Skeleton.SkinningModeEnum.Auto )]
		//public Reference<Component_Skeleton.SkinningModeEnum> OverrideSkinningMode
		//{
		//	get { if( _overrideSkinningMode.BeginGet() ) OverrideSkinningMode = _overrideSkinningMode.Get( this ); return _overrideSkinningMode.value; }
		//	set { if( _overrideSkinningMode.BeginSet( ref value ) ) { try { OverrideSkinningModeChanged?.Invoke( this ); } finally { _overrideSkinningMode.EndSet(); } } }
		//}
		//public event Action<Component_MeshInSpaceAnimationController> OverrideSkinningModeChanged;
		//ReferenceField<Component_Skeleton.SkinningModeEnum> _overrideSkinningMode = Component_Skeleton.SkinningModeEnum.Auto;

		/// <summary>
		/// Whether to calculate skinning by means CPU instead GPU.
		/// </summary>
		[Category( "Debug" )]
		[DisplayName( "Calculate On CPU" )]
		[DefaultValue( false )]
		public Reference<bool> CalculateOnCPU
		{
			get { if( _calculateOnCPU.BeginGet() ) CalculateOnCPU = _calculateOnCPU.Get( this ); return _calculateOnCPU.value; }
			set
			{
				if( _calculateOnCPU.BeginSet( ref value ) )
				{
					try
					{
						CalculateOnCPUChanged?.Invoke( this );
						needRecreateModifiableMesh = true;
					}
					finally { _calculateOnCPU.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CalculateOnCPU"/> property value changes.</summary>
		public event Action<Component_MeshInSpaceAnimationController> CalculateOnCPUChanged;
		ReferenceField<bool> _calculateOnCPU = false;

		/////////////////////////////////////////

		struct BoneGlobalTransformItem
		{
			public bool HasValue;
			public Matrix4F Value;

			public BoneGlobalTransformItem( bool hasValue, ref Matrix4F value )
			{
				HasValue = hasValue;
				Value = value;
			}
		}

		/////////////////////////////////////////

		public class AnimationStateClass
		{
			public class AnimationItem
			{
				public Component_Animation Animation;
				public bool AutoRewind = true;
				public double Speed = 1;
				public float Factor = 1;
				public bool InterpolationFading;

				public double? CurrentTime;
				public float? CurrentFactor;
			}
			public List<AnimationItem> Animations = new List<AnimationItem>();//public AnimationItem[] Animations;

			public delegate void AdditionalBoneTransformsUpdateDelegate( Component_MeshInSpaceAnimationController controller, AnimationStateClass animationState, Component_Skeleton skeleton, Component_SkeletonAnimationTrack.CalculateBoneTransformsItem[] result, ref bool updateTwice, int updateIteration );
			public AdditionalBoneTransformsUpdateDelegate AdditionalBoneTransformsUpdate;
		}

		/////////////////////////////////////////

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( ParentMeshInSpace != null )
			{
				if( EnabledInHierarchy )
				{
					ParentMeshInSpace.GetRenderSceneDataBefore += ParentMeshInSpace_GetRenderSceneDataBefore;
					ParentMeshInSpace.GetRenderSceneDataAddToFrameData += ParentMeshInSpace_GetRenderSceneDataAddToFrameData;
				}
				else
				{
					ParentMeshInSpace.GetRenderSceneDataBefore -= ParentMeshInSpace_GetRenderSceneDataBefore;
					ParentMeshInSpace.GetRenderSceneDataAddToFrameData -= ParentMeshInSpace_GetRenderSceneDataAddToFrameData;

					if( ParentMeshInSpace.ModifiableMesh_CreatedByObject == this )
						ParentMeshInSpace.ModifiableMesh_Destroy();
					needRecreateModifiableMesh = false;
				}
			}

			//touch PlayAnimation
			if( EnabledInHierarchy )
			{
				var a = PlayAnimation.Value;
			}
		}

		void ParentMeshInSpace_GetRenderSceneDataBefore( Component_ObjectInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			//check need modifiable mesh
			bool needModifiableMesh = CheckNeedModifiableMesh();

			//check need recreate
			if( needRecreateModifiableMesh )
			{
				if( ParentMeshInSpace.ModifiableMesh_CreatedByObject == this )
					ParentMeshInSpace.ModifiableMesh_Destroy();
				needRecreateModifiableMesh = false;
			}

			//recreate
			if( needModifiableMesh )
			{
				if( ParentMeshInSpace.ModifiableMesh == null )
				{
					if( CalculateOnCPU )
					{
						var flags = Component_MeshInSpace.ModifiableMeshCreationFlags.VertexBuffersCreateDuplicate | Component_MeshInSpace.ModifiableMeshCreationFlags.VertexBuffersDynamic;

						ParentMeshInSpace.ModifiableMesh_Create( this, flags );
					}

					//var flags = Component_MeshInSpace.ModifiableMeshCreationFlags.VertexBuffersCreateDuplicate;
					//if( CalculateOnCPU )
					//	flags |= Component_MeshInSpace.ModifiableMeshCreationFlags.VertexBuffersDynamic;
					//else
					//	flags |= Component_MeshInSpace.ModifiableMeshCreationFlags.VertexBuffersComputeWrite;
					//ParentMeshInSpace.ModifiableMesh_Create( this, flags );

					needRecreateModifiableMesh = false;
				}
			}
			else
			{
				if( ParentMeshInSpace.ModifiableMesh_CreatedByObject == this )
				{
					ParentMeshInSpace.ModifiableMesh_Destroy();
					needRecreateModifiableMesh = false;
				}
			}

			//update data
			if( ParentMeshInSpace.ModifiableMesh_CreatedByObject == this )
				UpdateModifiableMesh( context );

			if( DisplaySkeleton )
				RenderSkeleton( context.Owner );
		}

		////!!!!temp?
		//public void _Update()
		//{
		//	//!!!!

		//	if( !CalculateOnCPU )
		//	{
		//		Component_Skeleton skeleton = ReplaceSkeleton;
		//		if( skeleton == null )
		//			skeleton = ParentMeshInSpace?.Mesh.Value?.Skeleton;

		//		if( skeleton != null )
		//		{
		//			q;

		//			var animation = PlayAnimation.Value;
		//			if( animation != null )
		//			{
		//				UpdateAnimationTime();

		//				//settings.animationStates = new AnimationStateItem[ 1 ];
		//				//settings.animationStates[ 0 ] = new AnimationStateItem( animation, currentLocalTime, 1 );

		//				var skeletonAnimation = animation as Component_SkeletonAnimation;
		//				var track = skeletonAnimation?.Track.Value;

		//				if( track != null || CalculateBoneTransforms != null )
		//				{
		//					Update( skeleton, track, currentAnimationTime );

		//					//if( transformMatrixRelativeToSkin != null && transformMatrixRelativeToSkin.Length != 0 )
		//					//{
		//					//	item.AnimationData = new Component_RenderingPipeline.RenderSceneData.MeshItem.AnimationDataClass();

		//					//	bool dualQuaternion = false;// GetSkinningMode( skeleton ) == Component_Skeleton.SkinningModeEnum.DualQuaternion;
		//					//	if( dualQuaternion )
		//					//		item.AnimationData.Mode = 2;
		//					//	else
		//					//		item.AnimationData.Mode = 1;

		//					//	//create dynamic texture
		//					//	var size = new Vector2I( 4, MathEx.NextPowerOfTwo( transformMatrixRelativeToSkin.Length ) );
		//					//	var bonesTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, Component_Image.TypeEnum._2D, size, PixelFormat.Float32RGBA, 0, false );

		//					//	//try get array from texture to minimize memory allocations
		//					//	var surfaces = bonesTexture.Result.GetData();
		//					//	if( surfaces == null )
		//					//		surfaces = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( 0, 0, new byte[ size.X * size.Y * 16 ] ) };
		//					//	var data = surfaces[ 0 ].data;

		//					//	//copy data to the texture
		//					//	unsafe
		//					//	{
		//					//		fixed ( byte* pData2 = data )
		//					//		{
		//					//			Matrix4F* pData = (Matrix4F*)pData2;
		//					//			for( int n = 0; n < transformMatrixRelativeToSkin.Length; n++ )
		//					//				pData[ n ] = transformMatrixRelativeToSkin[ n ];
		//					//		}
		//					//	}
		//					//	bonesTexture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( 0, 0, data ) } );

		//					//	item.AnimationData.BonesTexture = bonesTexture;
		//					//}
		//				}
		//			}
		//		}
		//	}
		//}

		public Component_Skeleton GetSkeleton()
		{
			Component_Skeleton skeleton = ReplaceSkeleton;
			if( skeleton == null )
				skeleton = ParentMeshInSpace?.Mesh.Value?.Skeleton;
			return skeleton;
		}

		private void ParentMeshInSpace_GetRenderSceneDataAddToFrameData( Component_MeshInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, ref Component_RenderingPipeline.RenderSceneData.MeshItem item )
		{
			if( !CalculateOnCPU )
			{
				var skeleton = GetSkeleton();
				if( skeleton != null )
				{
					//var animation = PlayAnimation.Value;
					//if( animation != null )
					//{
					UpdateAnimationTime();

					//settings.animationStates = new AnimationStateItem[ 1 ];
					//settings.animationStates[ 0 ] = new AnimationStateItem( animation, currentLocalTime, 1 );

					//var skeletonAnimation = animation as Component_SkeletonAnimation;
					//var track = skeletonAnimation?.Track.Value;

					//if( track != null || CalculateBoneTransforms != null )
					//{
					try
					{
						Update( skeleton );//, track, currentAnimationTime );
					}
					catch { }

					if( transformMatrixRelativeToSkin != null && transformMatrixRelativeToSkin.Length != 0 )
					{
						item.AnimationData = new Component_RenderingPipeline.RenderSceneData.MeshItem.AnimationDataClass();

						bool dualQuaternion = false;// GetSkinningMode( skeleton ) == Component_Skeleton.SkinningModeEnum.DualQuaternion;
						if( dualQuaternion )
							item.AnimationData.Mode = 2;
						else
							item.AnimationData.Mode = 1;

						//create dynamic texture
						var size = new Vector2I( 4, MathEx.NextPowerOfTwo( transformMatrixRelativeToSkin.Length ) );
						var bonesTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, Component_Image.TypeEnum._2D, size, PixelFormat.Float32RGBA, 0, false );

						//try get array from texture to minimize memory allocations
						var surfaces = bonesTexture.Result.GetData();
						if( surfaces == null )
							surfaces = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( 0, 0, new byte[ size.X * size.Y * 16 ] ) };
						var data = surfaces[ 0 ].data;

						//copy data to the texture
						unsafe
						{
							fixed( byte* pData2 = data )
							{
								Matrix4F* pData = (Matrix4F*)pData2;
								for( int n = 0; n < transformMatrixRelativeToSkin.Length; n++ )
									pData[ n ] = transformMatrixRelativeToSkin[ n ];
							}
						}
						bonesTexture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( 0, 0, data ) } );

						item.AnimationData.BonesTexture = bonesTexture;
					}
					//}
					//}
				}
			}
		}

		void RenderSkeleton( Viewport viewport )
		{
			// ParentMeshInSpace.Transform is automaticaly applyed to ParentMeshInSpace.Mesh, skeleton must be transformed manually
			var transformMatrix = ParentMeshInSpace?.Transform.Value?.ToMatrix4() ?? Matrix4.Identity;

			var skeletonArrows = GetCurrentAnimatedSkeletonArrows();
			if( skeletonArrows != null )
			{
				var color = new ColorValue( 0, 0.5, 1, 0.7 ); //ToDo : Вынести в другое место.
				viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

				foreach( var arrow in skeletonArrows )
					viewport.Simple3DRenderer.AddArrow( transformMatrix * arrow.Start, transformMatrix * arrow.End );
			}
		}


		bool CheckNeedModifiableMesh()
		{
			if( CalculateOnCPU )
			{
				return true;

				//if( ReplaceSkeleton.ReferenceSpecified )
				//	return true;
				//var mesh = ParentMeshInSpace?.Mesh.Value;
				//if( mesh != null && mesh.Skeleton.ReferenceSpecified )
				//	return true;

				//if( PlayAnimation.ReferenceSpecified )
				//	return true;
			}

			return false;
		}

		void UpdateModifiableMesh( ViewportRenderingContext context )
		{
			var originalMesh = ParentMeshInSpace.Mesh.Value;
			var modifiableMesh = ParentMeshInSpace.ModifiableMesh;

			var skeleton = GetSkeleton();
			if( skeleton != null )
			{
				//!!!!сериализовывать

				//var animation = PlayAnimation.Value;
				//if( animation != null )
				//{
				UpdateAnimationTime();

				//settings.animationStates = new AnimationStateItem[ 1 ];
				//settings.animationStates[ 0 ] = new AnimationStateItem( animation, currentLocalTime, 1 );

				//var skeletonAnimation = animation as Component_SkeletonAnimation;
				//var track = skeletonAnimation?.Track.Value;

				//if( track != null || CalculateBoneTransforms != null )
				//{
				Update( skeleton );//, track, currentAnimationTime );
				CalculateCPU( skeleton, originalMesh, modifiableMesh );
				//	}
				//}

				if( needResetToOriginalMesh )
				{
					needResetToOriginalMesh = false;
					if( CalculateOnCPU )
						ResetToOriginalMesh( originalMesh, modifiableMesh );
				}
			}
		}

		/////////////////////////////////////////

		void UpdateAnimationTime()
		{
			double t = EngineApp.EngineTime;
			double timeStep = t - currentEngineTime;
			currentEngineTime = t;

			if( animationState != null && animationState.Animations != null )
			{
				var interpolationItem = InterpolationTime.Value;

				Stack<int> itemsToRemove = null;

				for( int n = 0; n < animationState.Animations.Count; n++ )
				{
					var item = animationState.Animations[ n ];
					if( item.Animation == null )
						continue;

					//update CurrentTime, detect 'remove'

					bool remove = false;

					if( item.CurrentTime == null )
						item.CurrentTime = 0;

					item.CurrentTime += timeStep * item.Speed;

					if( item.CurrentTime >= item.Animation.Length )
					{
						if( item.AutoRewind )
							item.CurrentTime -= item.Animation.Length;
						else
							remove = true;
					}
					else if( item.CurrentTime < 0 )
					{
						if( item.AutoRewind )
							item.CurrentTime += item.Animation.Length;
						else
							remove = true;
					}


					//!!!!неперематываемые анимации фейдить вначале и в конце

					//update CurrentFactor

					if( item.CurrentFactor == null )
						item.CurrentFactor = 0;

					if( item.CurrentFactor < item.Factor )
					{
						if( interpolationItem != 0 )
						{
							item.CurrentFactor += (float)( timeStep / interpolationItem );
							if( item.CurrentFactor > item.Factor )
								item.CurrentFactor = item.Factor;
						}
						else
							item.CurrentFactor = item.Factor;
					}
					else if( item.CurrentFactor > item.Factor )
					{
						if( interpolationItem != 0 )
						{
							item.CurrentFactor -= (float)( timeStep / interpolationItem );
							if( item.CurrentFactor < item.Factor )
								item.CurrentFactor = item.Factor;
						}
						else
							item.CurrentFactor = item.Factor;
					}

					if( item.CurrentFactor == 0 && item.InterpolationFading )
						remove = true;

					if( remove )
					{
						if( itemsToRemove == null )
							itemsToRemove = new Stack<int>();
						itemsToRemove.Push( n );
					}
				}

				if( itemsToRemove != null )
				{
					while( itemsToRemove.Count != 0 )
						animationState.Animations.RemoveAt( itemsToRemove.Pop() );
				}
			}


			//double t = EngineApp.EngineTime;
			//double increment = t - currentEngineTime;
			//currentEngineTime = t;
			//currentAnimationTime += increment * Speed;

			//var animation = PlayAnimation.Value as Component_SkeletonAnimation;
			//if( animation != null )
			//{
			//	double animationStartTime = animation.TrackStartTime;
			//	double animationLength = animation.Length;
			//	if( animationLength > 0 )
			//	{
			//		if( AutoRewind )
			//		{
			//			while( currentAnimationTime > animationStartTime + animationLength )
			//				currentAnimationTime -= animationLength;
			//			while( currentAnimationTime < animationStartTime )
			//				currentAnimationTime += animationLength;
			//		}
			//		else
			//			MathEx.Clamp( ref currentAnimationTime, animationStartTime, animationStartTime + animationLength );
			//	}
			//	else
			//		currentAnimationTime = animationStartTime;
			//}
			//else
			//	currentAnimationTime = 0;
		}

		public IList<Line3F> GetCurrentAnimatedSkeletonArrows()
		{
			var skeleton = GetSkeleton();
			if( skeleton == null )
				return null;

			//var skeleton = ReplaceSkeleton.Value ?? ParentMeshInSpace?.Mesh.Value?.Skeleton;
			//if( skeleton == null || PlayAnimation.Value == null )
			//	return null;
			//var skeletonAnimationTrack = ( PlayAnimation.Value as Component_SkeletonAnimation )?.Track;
			//if( skeletonAnimationTrack == null )
			//	return null;

			UpdateAnimationTime();
			Update( skeleton );//, skeletonAnimationTrack, currentAnimationTime );
			if( bones == null )
				return null;
			return GetSkeletonArrows( skeleton );
		}

		/////////////////////////////////////////

		struct SourceChannel<TBuffer> where TBuffer : unmanaged
		{
			public bool Exists;
			VertexElement sourceElement;
			GpuVertexBuffer sourceBuffer;
			public TBuffer[] SourceData;

			public SourceChannel( Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation sourceOper, VertexElementSemantic semantic, VertexElementType type )
				: this()
			{
				if( sourceOper.VertexStructure.GetElementBySemantic( semantic, out sourceElement ) && sourceElement.Type == type )
				{
					sourceBuffer = sourceOper.VertexBuffers[ sourceElement.Source ];
					SourceData = sourceBuffer.ExtractChannel<TBuffer>( sourceElement.Offset );
					Exists = true;
				}
			}
		}

		/////////////////////////////////////////

		struct ChannelFloat3
		{
			public bool Exists;
			VertexElement destElement;
			GpuVertexBuffer destBuffer;
			public Vector3F[] SourceData;
			public Vector3F[] DestData;

			public ChannelFloat3(
				Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation sourceOper,
				Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation destOper,
				VertexElementSemantic semantic
			) : this()
			{
				if( sourceOper.VertexStructure.GetElementBySemantic( semantic, out VertexElement sourceElement ) && sourceElement.Type == VertexElementType.Float3 )
				{
					var sourceBuffer = sourceOper.VertexBuffers[ sourceElement.Source ];
					SourceData = sourceBuffer.ExtractChannel<Vector3F>( sourceElement.Offset );
					if( destOper.VertexStructure.GetElementBySemantic( semantic, out destElement ) && destElement.Type == VertexElementType.Float3 )
					{
						destBuffer = destOper.VertexBuffers[ destElement.Source ];
						DestData = new Vector3F[ destBuffer.VertexCount ];
						Exists = true;
					}
				}
			}

			public void WriteChannel()
			{
				if( Exists )
					destBuffer.WriteChannel( destElement.Offset, DestData );
			}
		}

		/////////////////////////////////////////

		struct ChannelFloat4
		{
			public bool Exists;
			VertexElement destElement;
			GpuVertexBuffer destBuffer;
			public Vector4F[] SourceData;
			public Vector4F[] DestData;

			public ChannelFloat4(
				Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation sourceOper,
				Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation destOper,
				VertexElementSemantic semantic
			) : this()
			{
				if( sourceOper.VertexStructure.GetElementBySemantic( semantic, out VertexElement sourceElement ) && sourceElement.Type == VertexElementType.Float4 )
				{
					var sourceBuffer = sourceOper.VertexBuffers[ sourceElement.Source ];
					SourceData = sourceBuffer.ExtractChannel<Vector4F>( sourceElement.Offset );
					if( destOper.VertexStructure.GetElementBySemantic( semantic, out destElement ) && destElement.Type == VertexElementType.Float4 )
					{
						destBuffer = destOper.VertexBuffers[ destElement.Source ];
						DestData = new Vector4F[ destBuffer.VertexCount ];
						Exists = true;
					}
				}
			}

			public void WriteChannel()
			{
				if( Exists )
					destBuffer.WriteChannel( destElement.Offset, DestData );
			}
		}

		/////////////////////////////////////////

		//Component_Skeleton.SkinningModeEnum GetSkinningMode( Component_Skeleton skeleton )
		//{
		//	var _override = OverrideSkinningMode.Value;
		//	if( _override != Component_Skeleton.SkinningModeEnum.Auto )
		//		return _override;

		//	var selected = skeleton.SkinningMode.Value;
		//	if( selected != Component_Skeleton.SkinningModeEnum.Auto )
		//		return selected;

		//	if( !hasScale )
		//		return Component_Skeleton.SkinningModeEnum.DualQuaternion;
		//	else
		//		return Component_Skeleton.SkinningModeEnum.Linear;
		//}

		protected virtual void CalculateCPU( Component_Skeleton skeleton, Component_Mesh originalMesh, Component_Mesh modifiableMesh )
		{
			bool dualQuaternion = false;// GetSkinningMode( skeleton ) == Component_Skeleton.SkinningModeEnum.DualQuaternion;

			for( int nOper = 0; nOper < modifiableMesh.Result.MeshData.RenderOperations.Count; nOper++ )
			{
				var sourceOper = originalMesh.Result.MeshData.RenderOperations[ nOper ];
				var destOper = modifiableMesh.Result.MeshData.RenderOperations[ nOper ];

				var position = new ChannelFloat3( sourceOper, destOper, VertexElementSemantic.Position );
				if( position.Exists )
				{
					var normal = new ChannelFloat3( sourceOper, destOper, VertexElementSemantic.Normal );
					var tangent = new ChannelFloat4( sourceOper, destOper, VertexElementSemantic.Tangent );

					var blendIndices = new SourceChannel<Vector4I>( sourceOper, VertexElementSemantic.BlendIndices, VertexElementType.Integer4 );
					var blendWeights = new SourceChannel<Vector4F>( sourceOper, VertexElementSemantic.BlendWeights, VertexElementType.Float4 );

					if( !blendIndices.Exists || !blendWeights.Exists )
						continue;

					if( normal.Exists )
					{
						if( tangent.Exists )
						{
							TransformVertices(
								dualQuaternion,
								position.SourceData, normal.SourceData, tangent.SourceData, blendIndices.SourceData, blendWeights.SourceData,
								position.DestData, normal.DestData, tangent.DestData
							);
						}
						else
						{
							TransformVertices(
								dualQuaternion,
								position.SourceData, normal.SourceData, blendIndices.SourceData, blendWeights.SourceData,
								position.DestData, normal.DestData
							);
						}
					}
					else
					{
						TransformVertices(
							dualQuaternion,
							position.SourceData, blendIndices.SourceData, blendWeights.SourceData,
							position.DestData
						);
					}

					position.WriteChannel();
					normal.WriteChannel();
					tangent.WriteChannel();
				}
			}
		}

		static void ResetToOriginalMesh( Component_Mesh originalMesh, Component_Mesh modifiableMesh )
		{
			for( int nOper = 0; nOper < modifiableMesh.Result.MeshData.RenderOperations.Count; nOper++ )
			{
				var sourceOper = originalMesh.Result.MeshData.RenderOperations[ nOper ];
				var destOper = modifiableMesh.Result.MeshData.RenderOperations[ nOper ];

				var channelsSemantic = new[] { VertexElementSemantic.Position, VertexElementSemantic.Normal, VertexElementSemantic.Tangent, VertexElementSemantic.Bitangent };
				foreach( var semantic in channelsSemantic )
				{
					var channel = new ChannelFloat3( sourceOper, destOper, semantic );
					if( channel.Exists )
					{
						for( int n = 0; n < channel.DestData.Length; n++ )
							channel.DestData[ n ] = channel.SourceData[ n ];
						channel.WriteChannel();
					}
				}
			}
		}

		/////////////////////////////////////////

		void ResetSkeletonAndAnimation( bool needResetToOriginalMesh )
		{
			if( needResetToOriginalMesh )
				this.needResetToOriginalMesh = true;
			currentEngineTime = EngineApp.EngineTime;

			//currentAnimationTime = ( PlayAnimation.Value as Component_SkeletonAnimation )?.TrackStartTime ?? 0;


			calculatedForTime = -1;
			boneGlobalTransforms = null;
			bones = null;
			transformMatrixRelativeToSkin = null;
			//!!!!disable DQS
			//transformRelativeToSkin = null;
			//hasScale = false;
		}

		public delegate void CalculateBoneTransformsDelegate( Component_MeshInSpaceAnimationController sender, Component_SkeletonAnimationTrack.CalculateBoneTransformsItem[] result/*, ref bool handled*/ );
		public event CalculateBoneTransformsDelegate CalculateBoneTransforms;

		struct UpdateAnimationItem
		{
			public Component_Animation Animation;
			public double CurrentTime;
			public float CurrentFactor;
		}

		void Update( Component_Skeleton skeleton )
		{
			var time = EngineApp.EngineTime;
			//var time = currentAnimationTime;

			if( time != calculatedForTime )
			{
				var resetAnimation = true;

				//update general data
				calculatedForTime = time;
				bones = skeleton.GetBones();

				//calculate bone transforms

				if( boneTransforms == null || boneTransforms.Length != bones.Length )
					boneTransforms = new Component_SkeletonAnimationTrack.CalculateBoneTransformsItem[ bones.Length ];
				var boneTransformsInitialized = false;


				if( animationState != null )
				{
					var animations = new List<UpdateAnimationItem>( animationState.Animations.Count );
					for( int n = 0; n < animationState.Animations.Count; n++ )
					{
						var item = animationState.Animations[ n ];
						if( item.CurrentFactor.HasValue && item.CurrentFactor > 0 )
						{
							var item2 = new UpdateAnimationItem();
							item2.Animation = item.Animation;
							item2.CurrentTime = item.CurrentTime.HasValue ? item.CurrentTime.Value : 0;
							item2.CurrentFactor = item.CurrentFactor.Value;
							animations.Add( item2 );
						}
					}

					if( animations.Count != 0 )
					{

						//update skeleton twice because during calculation the data of bone transforms taken from previous update. it is used in Character component
						var updateTwice = false;

						for( int nIteration = 0; nIteration < ( updateTwice ? 2 : 1 ); nIteration++ )
						{
							if( animations.Count > 1 )
							{
								//normalize factors
								{
									var total = 0.0f;
									for( int n = 0; n < animations.Count; n++ )
										total += animations[ n ].CurrentFactor;

									for( int n = 0; n < animations.Count; n++ )
									{
										var item = animations[ n ];
										item.CurrentFactor /= total;
										animations[ n ] = item;
									}
								}

								var transforms = new Component_SkeletonAnimationTrack.CalculateBoneTransformsItem[ boneTransforms.Length ];
								var count = 0;

								for( int nAnimation = 0; nAnimation < animations.Count; nAnimation++ )
								{
									var item = animations[ nAnimation ];

									var skeletonAnimation = item.Animation as Component_SkeletonAnimation;
									if( skeletonAnimation != null )
									{
										var track = skeletonAnimation?.Track.Value;
										if( track != null )
										{
											track?.CalculateBoneTransforms( skeletonAnimation.TrackStartTime + item.CurrentTime, transforms );

											for( int n = 0; n < boneTransforms.Length; n++ )
											{
												ref var boneSource = ref transforms[ n ];
												ref var boneDest = ref boneTransforms[ n ];

												if( count == 0 )
												{
													boneDest.Position = boneSource.Position * item.CurrentFactor;
													boneDest.Rotation = boneSource.Rotation;
													boneDest.Scale = boneSource.Scale * item.CurrentFactor;
												}
												else
												{
													boneDest.Position += boneSource.Position * item.CurrentFactor;
													boneDest.Rotation = QuaternionF.Slerp( boneDest.Rotation, boneSource.Rotation, item.CurrentFactor );
													boneDest.Scale += boneSource.Scale * item.CurrentFactor;
												}
											}

											count++;
											boneTransformsInitialized = true;
										}
									}
								}

								//normalize rotation
								if( count != 0 )
								{
									for( int n = 0; n < boneTransforms.Length; n++ )
									{
										ref var item = ref boneTransforms[ n ];
										item.Rotation.Normalize();
									}
								}
							}
							else
							{
								var item = animations[ 0 ];

								var skeletonAnimation = item.Animation as Component_SkeletonAnimation;
								if( skeletonAnimation != null )
								{
									var track = skeletonAnimation?.Track.Value;
									if( track != null )
									{
										track?.CalculateBoneTransforms( skeletonAnimation.TrackStartTime + item.CurrentTime, boneTransforms );
										boneTransformsInitialized = true;
									}
								}
							}

							if( boneTransformsInitialized )
							{
								if( animationState != null && animationState.AdditionalBoneTransformsUpdate != null )
									animationState.AdditionalBoneTransformsUpdate( this, animationState, skeleton, boneTransforms, ref updateTwice, nIteration );

								CalculateBoneTransforms?.Invoke( this, boneTransforms );


								//calculate transformMatrixRelativeToSkin, transformRelativeToSkin, boneGlobalTransforms, hasScale

								//if( transformMatrixRelativeToSkin == null || transformMatrixRelativeToSkin.Length != bones.Length )
								//	transformMatrixRelativeToSkin = new Matrix4F[ bones.Length ];

								//!!!!disable DQS
								//if( transformRelativeToSkin == null || transformRelativeToSkin.Length != bones.Length )
								//	transformRelativeToSkin = new Component_SkeletonAnimationTrack.CalculateBoneTransformsItem[ bones.Length ];


								if( boneGlobalTransforms == null || boneGlobalTransforms.Length != bones.Length )
									boneGlobalTransforms = new BoneGlobalTransformItem[ bones.Length ];
								var matrixZero = Matrix4F.Zero;
								for( int i = 0; i < bones.Length; i++ )
									boneGlobalTransforms[ i ] = new BoneGlobalTransformItem( false, ref matrixZero );

								foreach( var b in bones )
									GetBoneGlobalTransformRecursive( skeleton, b );



								//!!!!disable DQS
								//hasScale = false;

								if( transformMatrixRelativeToSkin == null || transformMatrixRelativeToSkin.Length != bones.Length )
									transformMatrixRelativeToSkin = new Matrix4F[ bones.Length ];

								for( int i = 0; i < bones.Length; i++ )
								{
									var bone = bones[ i ];

									Matrix4F m;
									if( bone != null )
										Matrix4F.Multiply( ref GetBoneGlobalTransformRecursive( skeleton, bone ), ref bone.GetTransformMatrixInverse(), out m );
									else
										m = Matrix4F.Identity;
									transformMatrixRelativeToSkin[ i ] = m;

									//!!!!disable DQS

									//m.Decompose( out Vector3F t, out QuaternionF r, out Vector3F s );

									//transformRelativeToSkin[ i ] = new Component_SkeletonAnimationTrack.CalculateBoneTransformsItem { Position = t, Rotation = r, Scale = s };

									////if the scale differs from 1.0 more than this value, then the scaling is present and DualQuaternionSkinning can not be used.
									//const float EpsilonForScale = 1e-3f;
									//if( Math.Abs( 1.0f - s.X ) > EpsilonForScale || Math.Abs( 1.0f - s.Y ) > EpsilonForScale || Math.Abs( 1.0f - s.Y ) > EpsilonForScale )
									//	hasScale = true;
								}

								resetAnimation = false;
							}
						}
					}
				}

				if( resetAnimation )
					ResetSkeletonAndAnimation( false );
			}
		}

		ref Matrix4F GetBoneGlobalTransformRecursive( Component_Skeleton skeleton, Component_SkeletonBone bone )
		{
			int boneIndex = bone.GetCachedBoneIndex( skeleton );

			if( !boneGlobalTransforms[ boneIndex ].HasValue )
			{
				ToMatrix4( ref boneTransforms[ boneIndex ], out var res );
				if( bone.Parent is Component_SkeletonBone parent )
					res = GetBoneGlobalTransformRecursive( skeleton, parent ) * res;
				boneGlobalTransforms[ boneIndex ] = new BoneGlobalTransformItem( true, ref res );
			}
			return ref boneGlobalTransforms[ boneIndex ].Value;
		}

		IList<Line3F> GetSkeletonArrows( Component_Skeleton skeleton )
		{
			var result = new List<Line3F>( bones.Length );

			for( int i = 0; i < bones.Length; i++ )
			{
				var b1 = bones[ i ];
				if( !( b1.Parent is Component_SkeletonBone b0 ) )
					continue;

				var parentIndex = b0.GetCachedBoneIndex( skeleton );
				if( parentIndex == -1 )
					continue;

				ref var m0 = ref boneGlobalTransforms[ parentIndex ];
				ref var m1 = ref boneGlobalTransforms[ i ];
				if( m0.HasValue && m1.HasValue )
				{
					m0.Value.GetTranslation( out var t0 );
					m1.Value.GetTranslation( out var t1 );
					result.Add( new Line3F( t0, t1 ) );
				}
			}

			return result;
		}

		void TransformVertices(
			bool dualQuaternion,
			Vector3F[] position, Vector3F[] normal, Vector4F[] tangent, Vector4I[] blendIndex, Vector4F[] blendWeight,
			Vector3F[] newPosition, Vector3F[] newNormal, Vector4F[] newTangent )
		{
			if( dualQuaternion )
			{
				//!!!!disable DQS
				//for( int n = 0; n < newPosition.Length; n++ )
				//{
				//	newPosition[ n ] = TransformByDualQuaternionSkinning(
				//		position[ n ], normal[ n ], tangent[ n ], blendIndex[ n ], blendWeight[ n ],
				//		out newNormal[ n ], out newTangent[ n ] );
				//}
			}
			else
			{
				for( int n = 0; n < newPosition.Length; n++ )
				{
					newPosition[ n ] = TransformVertexByLinearBlendingSkinning(
						position[ n ], normal[ n ], tangent[ n ], blendIndex[ n ], blendWeight[ n ],
						out newNormal[ n ], out newTangent[ n ] );
				}
			}
		}

		void TransformVertices(
			bool dualQuaternion,
			Vector3F[] position, Vector3F[] normal, Vector4I[] blendIndex, Vector4F[] blendWeight,
			Vector3F[] newPosition, Vector3F[] newNormal )
		{
			if( dualQuaternion )
			{
				//!!!!disable DQS
				//for( int n = 0; n < newPosition.Length; n++ )
				//{
				//	newPosition[ n ] = TransformByDualQuaternionSkinning(
				//		position[ n ], normal[ n ], Vector4F.One, blendIndex[ n ], blendWeight[ n ],
				//		out newNormal[ n ], out _ );
				//}
			}
			else
			{
				for( int n = 0; n < newPosition.Length; n++ )
				{
					newPosition[ n ] = TransformVertexByLinearBlendingSkinning(
						position[ n ], normal[ n ], Vector4F.One, blendIndex[ n ], blendWeight[ n ],
						out newNormal[ n ], out _ );
				}
			}
		}

		void TransformVertices(
			bool dualQuaternion,
			Vector3F[] position, Vector4I[] blendIndex, Vector4F[] blendWeight,
			Vector3F[] newPosition )
		{
			if( dualQuaternion )
			{
				//!!!!disable DQS
				//for( int n = 0; n < newPosition.Length; n++ )
				//	newPosition[ n ] = TransformByDualQuaternionSkinning( position[ n ], blendIndex[ n ], blendWeight[ n ] );
			}
			else
			{
				for( int n = 0; n < newPosition.Length; n++ )
					newPosition[ n ] = TransformVertexByLinearBlendingSkinning( position[ n ], blendIndex[ n ], blendWeight[ n ] );
			}
		}

		Vector3F TransformVertexByLinearBlendingSkinning( Vector3F position, Vector4I blendIndex, Vector4F blendWeight )
		{
			Vector4F position4 = new Vector4F( position, 1.0f );
			if( !GetVertexTransformByLinearBlendingSkinning( blendIndex, blendWeight, out Matrix4F matrix ) )
				return position;
			else
				return ( matrix * position4 ).ToVector3F();
		}

		Vector3F TransformVertexByLinearBlendingSkinning( Vector3F position, Vector3F normal, Vector4F tangent, Vector4I blendIndex, Vector4F blendWeight, out Vector3F newNormal, out Vector4F newTangent )
		{
			Vector4F position4 = new Vector4F( position, 1.0f );
			if( !GetVertexTransformByLinearBlendingSkinning( blendIndex, blendWeight, out Matrix4F matrix ) )
			{
				newNormal = normal;
				newTangent = tangent;
				return position;
			}
			else
			{
				matrix.Decompose( out Vector3F t, out QuaternionF rot, out Vector3F s );
				newNormal = CalculateBlendNormal( normal, rot );
				newTangent = new Vector4F( CalculateBlendNormal( tangent.ToVector3F(), rot ), tangent.W );
				return ( matrix * position4 ).ToVector3F();
			}
		}

		//!!!!disable DQS
		//Vector3F TransformByDualQuaternionSkinning( Vector3F position, Vector4I blendIndex, Vector4F blendWeight )
		//{
		//	if( !GetVertexTransformByDualQuaternionSkinning( blendIndex, blendWeight, out DualQuaternionF dq, out Vector3F scale ) )
		//		return position;
		//	else
		//	{
		//		dq.Normalize();
		//		return CalculateBlendPosition( scale * position, dq ); //! Dual Quaternion skinning does not fully support scaling (animation will have defects, if it have different scales)
		//	}
		//}

		//!!!!disable DQS
		//Vector3F TransformByDualQuaternionSkinning( Vector3F position, Vector3F normal, Vector4F tangent, Vector4I blendIndex, Vector4F blendWeight, out Vector3F newNormal, out Vector4F newTangent )
		//{
		//	if( !GetVertexTransformByDualQuaternionSkinning( blendIndex, blendWeight, out DualQuaternionF dq, out Vector3F scale ) )
		//	{
		//		newNormal = normal;
		//		newTangent = tangent;
		//		return position;
		//	}
		//	else
		//	{
		//		dq.Normalize();
		//		newNormal = CalculateBlendNormal( normal, dq.Q0 );
		//		newTangent = new Vector4F( CalculateBlendNormal( tangent.ToVector3F(), dq.Q0 ), tangent.W );
		//		return CalculateBlendPosition( scale * position, dq );  //! Dual Quaternion skinning does not fully support scaling (animation will have defects, if it have different scales)
		//	}
		//}

		//From Ogre HLSL
		//void SGX_CalculateBlendNormal( in float3 vIn, in float2x4 blendDQ, out float3 vOut )
		//{
		//	vOut = ( vIn + 2.0 * cross( blendDQ[0].yzw, cross( blendDQ[0].yzw, vIn ) + blendDQ[0].x * vIn ) );
		//}
		//Warning : Ogre HLSL has W component of Quaternion in the first (X) component of vector.

		Vector3F CalculateBlendNormal( Vector3F vIn, QuaternionF blendQ )
		{
			Vector3F blendDQ_0_yzw = new Vector3F( blendQ.X, blendQ.Y, blendQ.Z );
			float blendDQ_0_x = blendQ.W;
			return vIn + 2.0f * Vector3F.Cross( blendDQ_0_yzw, Vector3F.Cross( blendDQ_0_yzw, vIn ) + blendDQ_0_x * vIn );
		}

		//Vector3F CalculateBlendNormal( Vector3F vIn, DualQuaternionF blendDQ )
		//{
		//	Vector3F blendDQ_0_yzw = new Vector3F( blendDQ.Q0.X, blendDQ.Q0.Y, blendDQ.Q0.Z );
		//	float blendDQ_0_x = blendDQ.Q0.W;
		//	return vIn + 2.0f * Vector3F.Cross( blendDQ_0_yzw, Vector3F.Cross( blendDQ_0_yzw, vIn ) + blendDQ_0_x * vIn );
		//}


		//From Ogre HLSL
		//void SGX_CalculateBlendPosition( in float3 position, in float2x4 blendDQ, out float4 vOut )
		//{
		//	float3 blendPosition = position + 2.0 * cross( blendDQ[0].yzw, cross( blendDQ[0].yzw, position ) + blendDQ[0].x * position );
		//	float3 trans = 2.0 * ( blendDQ[0].x * blendDQ[1].yzw - blendDQ[1].x * blendDQ[0].yzw + cross( blendDQ[0].yzw, blendDQ[1].yzw ) );
		//	blendPosition += trans;

		//	vOut = float4( blendPosition, 1.0 );
		//}
		//
		//Warning : Ogre HLSL has W component of Quaternion in the first (X) component of vector.
		// Also described in article: http://dev.theomader.com/dual-quaternion-skinning/
		Vector3F CalculateBlendPosition( Vector3F position, DualQuaternionF blendDQ )
		{
			Vector3F blendDQ_0_yzw = new Vector3F( blendDQ.Q0.X, blendDQ.Q0.Y, blendDQ.Q0.Z );
			Vector3F blendDQ_1_yzw = new Vector3F( blendDQ.Qe.X, blendDQ.Qe.Y, blendDQ.Qe.Z );
			float blendDQ_0_x = blendDQ.Q0.W;
			float blendDQ_1_x = blendDQ.Qe.W;

			var blendPosition = position + 2.0f * Vector3F.Cross( blendDQ_0_yzw, Vector3F.Cross( blendDQ_0_yzw, position ) + blendDQ_0_x * position );
			var trans = 2.0f * ( blendDQ_0_x * blendDQ_1_yzw - blendDQ_1_x * blendDQ_0_yzw + Vector3F.Cross( blendDQ_0_yzw, blendDQ_1_yzw ) );
			blendPosition += trans;
			return blendPosition;
		}

		/////////////////////////////////////////

		static void ToMatrix4( ref Component_SkeletonAnimationTrack.CalculateBoneTransformsItem keyframe, out Matrix4F result )
		{
			keyframe.Rotation.ToMatrix3( out var rot );
			Matrix3F.FromScale( ref keyframe.Scale, out var scl );
			Matrix3F.Multiply( ref rot, ref scl, out var rot2 );
			result = new Matrix4F( rot2, keyframe.Position );
			//return new Matrix4F( keyframe.Rotation.ToMatrix3() * Matrix3F.FromScale( keyframe.Scale ), keyframe.Position );
		}

		static float Dot( ref QuaternionF q1, ref QuaternionF q2 )
		{
			return q1.W * q2.W + q1.X * q2.X + q1.Y * q2.Y + q1.Z * q2.Z;
		}

		struct DualQuaternionF
		{
			public QuaternionF Q0, Qe;

			public DualQuaternionF( QuaternionF q, Vector3F t )
			{
				float w = -0.5f * ( t.X * q.X + t.Y * q.Y + t.Z * q.Z );
				float x = 0.5f * ( t.X * q.W + t.Y * q.Z - t.Z * q.Y );
				float y = 0.5f * ( -t.X * q.Z + t.Y * q.W + t.Z * q.X );
				float z = 0.5f * ( t.X * q.Y - t.Y * q.X + t.Z * q.W );

				Q0 = q;
				Qe = new QuaternionF( x, y, z, w );
			}

			public static DualQuaternionF operator +( DualQuaternionF dq1, DualQuaternionF dq2 )
			{
				return new DualQuaternionF
				{
					Q0 = dq1.Q0 + dq2.Q0,
					Qe = dq1.Qe + dq2.Qe
				};
			}

			public static DualQuaternionF operator *( DualQuaternionF dq, float scalar )
			{
				return new DualQuaternionF
				{
					Q0 = dq.Q0 * scalar,
					Qe = dq.Qe * scalar
				};
			}

			public void Normalize()
			{
				float lengthInverse = 1 / Q0.Length();
				Q0 *= lengthInverse;
				Qe *= lengthInverse;
			}

			////Перед вызовом сделать Normalize()
			//public void ToRotationTranslation( out Quaternion rot, out Vector3 translation )
			//{
			//	rot = q0;
			//	translation = new Vector3(
			//		2.0 * ( -qe.W * q0.X + qe.X * q0.W - qe.Y * q0.Z + qe.Z * q0.Y ),
			//		2.0 * ( -qe.W * q0.Y + qe.X * q0.Z + qe.Y * q0.W - qe.Z * q0.X ),
			//		2.0 * ( -qe.W * q0.Z - qe.X * q0.Y + qe.Y * q0.X + qe.Z * q0.W )
			//	);
			//}
		}

		bool GetVertexTransformByLinearBlendingSkinning( Vector4I blendIndex, Vector4F blendWeight, out Matrix4F matrix )
		{
			int index = blendIndex.X;
			if( index == -1 )
			{
				matrix = Matrix4F.Identity;
				return false;
			}
			matrix = blendWeight.X * transformMatrixRelativeToSkin[ index ];

			index = blendIndex.Y;
			if( index == -1 )
				return true;
			matrix += blendWeight.Y * transformMatrixRelativeToSkin[ index ];

			index = blendIndex.Z;
			if( index == -1 )
				return true;
			matrix += blendWeight.Z * transformMatrixRelativeToSkin[ index ];

			index = blendIndex.W;
			if( index == -1 )
				return true;
			matrix += blendWeight.W * transformMatrixRelativeToSkin[ index ];

			return true;
		}

		//!!!!disable DQS
		//bool GetVertexTransformByDualQuaternionSkinning( Vector4I blendIndex, Vector4F blendWeight, out DualQuaternionF sumDq, out Vector3F scale )
		//{
		//	DualQuaternionF dq;

		//	float weight = blendWeight.X;
		//	int index = blendIndex.X;
		//	if( index == -1 )
		//	{
		//		sumDq = new DualQuaternionF( QuaternionF.Identity, Vector3F.Zero );
		//		scale = Vector3F.One;
		//		return false;
		//	}
		//	var t = transformRelativeToSkin[ index ];
		//	sumDq = new DualQuaternionF( t.Rotation, t.Position ) * weight;
		//	scale = t.Scale * weight;
		//	var pivotR = t.Rotation;

		//	//------------
		//	weight = blendWeight.Y;
		//	index = blendIndex.Y;
		//	if( index == -1 )
		//		return true;
		//	t = transformRelativeToSkin[ index ];
		//	dq = new DualQuaternionF( t.Rotation, t.Position );
		//	sumDq += Dot( ref pivotR, ref t.Rotation ) < 0 ? dq * ( -weight ) : dq * weight; //AntipodalityAdjustment
		//	scale += t.Scale * weight;

		//	//------------
		//	weight = blendWeight.Z;
		//	index = blendIndex.Z;
		//	if( index == -1 )
		//		return true;
		//	t = transformRelativeToSkin[ index ];
		//	dq = new DualQuaternionF( t.Rotation, t.Position );
		//	sumDq += Dot( ref pivotR, ref t.Rotation ) < 0 ? dq * ( -weight ) : dq * weight; //AntipodalityAdjustment				
		//	scale += t.Scale * weight;

		//	//------------
		//	weight = blendWeight.W;
		//	index = blendIndex.W;
		//	if( index == -1 )
		//		return true;
		//	t = transformRelativeToSkin[ index ];
		//	dq = new DualQuaternionF( t.Rotation, t.Position );
		//	sumDq += Dot( ref pivotR, ref t.Rotation ) < 0 ? dq * ( -weight ) : dq * weight; //AntipodalityAdjustment		
		//	scale += t.Scale * weight;

		//	return true;
		//}

		///// <summary>
		///// Gets the current animation playback time.
		///// </summary>
		//[Browsable( false )]
		//public double CurrentAnimationTime
		//{
		//	get { return currentAnimationTime; }
		//}

		/// <summary>
		/// Gets the engine time the animation was last updated.
		/// </summary>
		[Browsable( false )]
		public double CurrentEngineTime
		{
			get { return currentEngineTime; }
		}

		public int GetBoneIndex( string name )
		{
			if( bones != null )
			{
				for( int n = 0; n < bones.Length; n++ )
					if( bones[ n ].Name == name )
						return n;
			}
			return -1;
		}

		public int GetBoneIndex( Component_SkeletonBone bone )
		{
			if( bones != null )
			{
				for( int n = 0; n < bones.Length; n++ )
					if( bones[ n ] == bone )
						return n;
			}
			return -1;
		}

		public bool GetBoneGlobalTransform( int boneIndex, ref Matrix4F value )
		{
			if( boneGlobalTransforms != null && boneIndex < boneGlobalTransforms.Length )
			{
				ref var item = ref boneGlobalTransforms[ boneIndex ];
				if( item.HasValue )
					value = item.Value;
				return item.HasValue;
			}
			return false;
		}

		public void SetAnimationState( AnimationStateClass newState, bool allowInterpolation )
		{
			//!!!!slowly

			//склеить. добавить новые, сохранить старые

			var oldValue = animationState;

			animationState = newState;

			if( animationState != null && oldValue != null )
			{
				foreach( var state in animationState.Animations )
				{
					//copy CurrentTime from old to new if it not initialized
					if( state.CurrentTime == null || state.CurrentFactor == null )
					{
						//find state in old list of animations with same animation of the 'state'
						var oldState = oldValue.Animations.FirstOrDefault( i => i.Animation == state.Animation );
						if( oldState != null && oldState.CurrentTime != null )
							state.CurrentTime = oldState.CurrentTime;
						if( oldState != null && oldState.CurrentFactor != null )
							state.CurrentFactor = oldState.CurrentFactor;
					}
				}

				//copy from old
				if( allowInterpolation )
				{
					foreach( var oldState in oldValue.Animations )
					{
						var state = animationState.Animations.FirstOrDefault( i => i.Animation == oldState.Animation );
						if( state == null )
						{
							oldState.InterpolationFading = true;
							oldState.Factor = 0;

							animationState.Animations.Add( oldState );
						}
					}
				}
			}
		}

		[Browsable( false )]
		public Component_SkeletonBone[] Bones
		{
			get { return bones; }
		}
	}
}
