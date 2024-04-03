// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NeoAxis
{
	/// <summary>
	/// An animation controller for the mesh in space.
	/// </summary>
	/// <remarks>
	/// Implemented linear skinning algorithm.
	/// </remarks>
	public class MeshInSpaceAnimationController : Component//, MeshInSpace.IMeshInSpaceChild//MeshInSpaceController
	{
		double currentEngineTime;

		bool needResetToOriginalMesh;
		bool needRecreateModifiableMesh;

		double calculatedForTime { get; set; } = -1;
		SkeletonBone[] bones;
		Dictionary<string, int> boneByName;
		int[] boneParents;
		SkeletonAnimationTrack.CalculateBoneTransformsItem[] boneTransforms;
		SkeletonAnimationTrack.CalculateBoneTransformsItem[] boneGlobalTransforms;
		Matrix4F[] transformMatrixRelativeToSkin; //transform from bind pose to a current pose

		RenderingPipeline.RenderSceneData.MeshItem.AnimationDataClass cachedAnimationData;

		static OpenList<UpdateAnimationItem> tempAnimations;

		//disable DQS
		//SkeletonAnimationTrack.CalculateBoneTransformsItem[] transformRelativeToSkin;
		//bool hasScale { get; set; }
		//// DualQuaternionSkinning не поддерживает scaling, поэтому если есть scaling то используется LBS :
		//// - В Ogre не нашлось обработки scale с DualQuaternionSkinning
		//// - Статьи о проблемах scaling:
		////    http://rodolphe-vaillant.fr/?e=78
		////    https://disney-animation.s3.amazonaws.com/uploads/production/publication_asset/98/asset/dualQ.pdf
		////

		/////////////////////////////////////////

		public interface IParentAnimationTriggerProcess
		{
			void AnimationTriggerFired( MeshInSpaceAnimationController sender, AnimationStateClass animationState, AnimationStateClass.AnimationItem animationItem, AnimationTrigger trigger );
		}

		/////////////////////////////////////////

		[Browsable( false )]
		public MeshInSpace ParentMeshInSpace
		{
			get { return Parent as MeshInSpace; }
		}

		/// <summary>
		/// The animation used by the controller.
		/// </summary>
		public Reference<Animation> PlayAnimation
		{
			get { if( _playAnimation.BeginGet() ) PlayAnimation = _playAnimation.Get( this ); return _playAnimation.value; }
			set
			{
				if( _playAnimation.BeginSet( this, ref value ) )
				{
					try
					{
						PlayAnimationChanged?.Invoke( this );

						ResetSkeletonAndAnimation( _playAnimation.value.Value == null );

						if( _playAnimation.value.Value != null )
						{
							var state = new AnimationStateClass();
							state.Animations.Add( new AnimationStateClass.AnimationItem() { Animation = _playAnimation.value, Speed = Speed, AutoRewind = AutoRewind, FreezeOnEnd = FreezeOnEnd } );
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
		public event Action<MeshInSpaceAnimationController> PlayAnimationChanged;
		ReferenceField<Animation> _playAnimation;

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
				if( _speed.BeginSet( this, ref value ) )
				{
					try
					{
						SpeedChanged?.Invoke( this );

						var playAnimation = PlayAnimation.Value;
						if( playAnimation != null )
						{
							var state = new AnimationStateClass();
							state.Animations.Add( new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = _speed.value, AutoRewind = AutoRewind, FreezeOnEnd = FreezeOnEnd } );
							//state.Animations = new[] { new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = _speed.value, AutoRewind = AutoRewind } };
							SetAnimationState( state, false );
						}
					}
					finally { _speed.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Speed"/> property value changes.</summary>
		public event Action<MeshInSpaceAnimationController> SpeedChanged;
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
				if( _autoRewind.BeginSet( this, ref value ) )
				{
					try
					{
						AutoRewindChanged?.Invoke( this );

						var playAnimation = PlayAnimation.Value;
						if( playAnimation != null )
						{
							var state = new AnimationStateClass();
							state.Animations.Add( new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = Speed, AutoRewind = _autoRewind.value, FreezeOnEnd = FreezeOnEnd } );
							//state.Animations = new[] { new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = Speed, AutoRewind = _autoRewind.value } };
							SetAnimationState( state, false );
						}
					}
					finally { _autoRewind.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AutoRewind"/> property value changes.</summary>
		public event Action<MeshInSpaceAnimationController> AutoRewindChanged;
		ReferenceField<bool> _autoRewind = true;

		/// <summary>
		/// Whether to freeze playing on the end of the animation.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> FreezeOnEnd
		{
			get { if( _freezeOnEnd.BeginGet() ) FreezeOnEnd = _freezeOnEnd.Get( this ); return _freezeOnEnd.value; }
			set
			{
				if( _freezeOnEnd.BeginSet( this, ref value ) )
				{
					try
					{
						FreezeOnEndChanged?.Invoke( this );

						var playAnimation = PlayAnimation.Value;
						if( playAnimation != null )
						{
							var state = new AnimationStateClass();
							state.Animations.Add( new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = Speed, AutoRewind = AutoRewind, FreezeOnEnd = _freezeOnEnd.value } );
							//state.Animations = new[] { new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = Speed, FreezeOnEnd = _freezeOnEnd.value } };
							SetAnimationState( state, false );
						}
					}
					finally { _freezeOnEnd.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="FreezeOnEnd"/> property value changes.</summary>
		public event Action<MeshInSpaceAnimationController> FreezeOnEndChanged;
		ReferenceField<bool> _freezeOnEnd = false;

		[Browsable( false )]
		public AnimationStateClass AnimationState { get { return animationState; } }
		AnimationStateClass animationState;

		/// <summary>
		/// The skeleton used by the controller.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Skeleton> ReplaceSkeleton
		{
			get { if( _replaceSkeleton.BeginGet() ) ReplaceSkeleton = _replaceSkeleton.Get( this ); return _replaceSkeleton.value; }
			set
			{
				if( _replaceSkeleton.BeginSet( this, ref value ) )
				{
					try
					{
						ReplaceSkeletonChanged?.Invoke( this );

						ResetSkeletonAndAnimation( false );//?? false?

						var playAnimation = PlayAnimation.Value;
						if( playAnimation != null )
						{
							var state = new AnimationStateClass();
							state.Animations.Add( new AnimationStateClass.AnimationItem() { Animation = playAnimation, Speed = Speed, AutoRewind = AutoRewind, FreezeOnEnd = FreezeOnEnd } );
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
		public event Action<MeshInSpaceAnimationController> ReplaceSkeletonChanged;
		ReferenceField<Skeleton> _replaceSkeleton;

		[DefaultValue( 0.2 )]
		[Range( 0, 0.5 )]
		public Reference<double> InterpolationTime
		{
			get { if( _interpolationTime.BeginGet() ) InterpolationTime = _interpolationTime.Get( this ); return _interpolationTime.value; }
			set { if( _interpolationTime.BeginSet( this, ref value ) ) { try { InterpolationTimeChanged?.Invoke( this ); } finally { _interpolationTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InterpolationTime"/> property value changes.</summary>
		public event Action<MeshInSpaceAnimationController> InterpolationTimeChanged;
		ReferenceField<double> _interpolationTime = 0.2;

		/// <summary>
		/// Whether to display the skeleton.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> DisplaySkeleton
		{
			get { if( _displaySkeleton.BeginGet() ) DisplaySkeleton = _displaySkeleton.Get( this ); return _displaySkeleton.value; }
			set { if( _displaySkeleton.BeginSet( this, ref value ) ) { try { DisplaySkeletonChanged?.Invoke( this ); } finally { _displaySkeleton.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplaySkeleton"/> property value changes.</summary>
		public event Action<MeshInSpaceAnimationController> DisplaySkeletonChanged;
		ReferenceField<bool> _displaySkeleton = false;

		///// <summary>
		///// Overrides default skin deformation algorithm.
		///// </summary>
		//[DefaultValue( Skeleton.SkinningModeEnum.Auto )]
		//public Reference<Skeleton.SkinningModeEnum> OverrideSkinningMode
		//{
		//	get { if( _overrideSkinningMode.BeginGet() ) OverrideSkinningMode = _overrideSkinningMode.Get( this ); return _overrideSkinningMode.value; }
		//	set { if( _overrideSkinningMode.BeginSet( this, ref value ) ) { try { OverrideSkinningModeChanged?.Invoke( this ); } finally { _overrideSkinningMode.EndSet(); } } }
		//}
		//public event Action<MeshInSpaceAnimationController> OverrideSkinningModeChanged;
		//ReferenceField<Skeleton.SkinningModeEnum> _overrideSkinningMode = Skeleton.SkinningModeEnum.Auto;

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
				if( _calculateOnCPU.BeginSet( this, ref value ) )
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
		public event Action<MeshInSpaceAnimationController> CalculateOnCPUChanged;
		ReferenceField<bool> _calculateOnCPU = false;

		/////////////////////////////////////////

		//struct BoneGlobalTransformItem
		//{
		//	public bool HasValue;
		//	public Matrix4F Value;

		//	public BoneGlobalTransformItem( bool hasValue, ref Matrix4F value )
		//	{
		//		HasValue = hasValue;
		//		Value = value;
		//	}
		//}

		/////////////////////////////////////////

		public class AnimationStateClass
		{
			public List<AnimationItem> Animations = new List<AnimationItem>();
			//public QuaternionF RootBoneRotation = QuaternionF.Identity;

			//

			public class AnimationItem
			{
				//first way
				public Animation Animation;

				//second way. the ability to merge two animations
				public Animation Animation2;
				public float Animation2Factor;
				public int AnimationItemTag;

				public bool AutoRewind = true;
				public bool FreezeOnEnd;
				public double Speed = 1;
				public float Factor = 1;
				public bool InterpolationFading;

				public bool ReplaceMode;

				public string[] AffectBonesWithChildren;
				//public string[] SkipBonesWithChildren;

				public double? CurrentTime;
				public float? CurrentFactor;
			}

			public delegate void AdditionalBoneTransformsUpdateDelegate( MeshInSpaceAnimationController controller, AnimationStateClass animationState, Skeleton skeleton, SkeletonAnimationTrack.CalculateBoneTransformsItem[] result );
			public AdditionalBoneTransformsUpdateDelegate AdditionalBoneTransformsUpdate;

			//

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public AnimationItem FindItemWithoutAnimationItemTag( Animation animation )
			{
				for( int n = 0; n < Animations.Count; n++ )
				{
					var item = Animations[ n ];
					if( item.AnimationItemTag == 0 && item.Animation == animation )
						return item;
				}
				return null;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public AnimationItem FindItemByAnimationItemTag( int animationItemTag )
			{
				for( int n = 0; n < Animations.Count; n++ )
				{
					var item = Animations[ n ];
					if( item.AnimationItemTag == animationItemTag )
						return item;
				}
				return null;
			}
		}

		/////////////////////////////////////////

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( ParentMeshInSpace != null )
			{
				if( EnabledInHierarchyAndIsInstance )
				{
					ParentMeshInSpace.GetRenderSceneDataBefore += ParentMeshInSpace_GetRenderSceneDataBefore;
					ParentMeshInSpace.GetRenderSceneDataAddToFrameData += ParentMeshInSpace_GetRenderSceneDataAddToFrameData;
				}
				else
				{
					ParentMeshInSpace.GetRenderSceneDataBefore -= ParentMeshInSpace_GetRenderSceneDataBefore;
					ParentMeshInSpace.GetRenderSceneDataAddToFrameData -= ParentMeshInSpace_GetRenderSceneDataAddToFrameData;

					if( ParentMeshInSpace.ModifiableMeshCreatedByObject == this )
						ParentMeshInSpace.ModifiableMeshDestroy();
					needRecreateModifiableMesh = false;
				}
			}

			//touch PlayAnimation
			if( EnabledInHierarchyAndIsInstance )
				PlayAnimation.Touch();
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void ParentMeshInSpace_GetRenderSceneDataBefore( ObjectInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			//check need modifiable mesh
			bool needModifiableMesh = CheckNeedModifiableMesh();

			//check need recreate
			if( needRecreateModifiableMesh )
			{
				if( ParentMeshInSpace.ModifiableMeshCreatedByObject == this )
					ParentMeshInSpace.ModifiableMeshDestroy();
				needRecreateModifiableMesh = false;
			}

			//recreate and update
			if( needModifiableMesh )
			{
				if( ParentMeshInSpace.ModifiableMesh == null )
				{
					if( CalculateOnCPU )
					{
						var flags = MeshInSpace.ModifiableMeshCreationFlags.VertexBuffersCreateDuplicate | MeshInSpace.ModifiableMeshCreationFlags.VertexBuffersDynamic;

						ParentMeshInSpace.ModifiableMeshCreate( this, flags );
					}

					//var flags = MeshInSpace.ModifiableMeshCreationFlags.VertexBuffersCreateDuplicate;
					//if( CalculateOnCPU )
					//	flags |= MeshInSpace.ModifiableMeshCreationFlags.VertexBuffersDynamic;
					//else
					//	flags |= MeshInSpace.ModifiableMeshCreationFlags.VertexBuffersComputeWrite;
					//ParentMeshInSpace.ModifiableMesh_Create( this, flags );

					needRecreateModifiableMesh = false;
				}

				//update data
				if( ParentMeshInSpace.ModifiableMeshCreatedByObject == this )
					UpdateModifiableMesh( context );
			}
			else
			{
				if( ParentMeshInSpace.ModifiableMeshCreatedByObject == this )
				{
					ParentMeshInSpace.ModifiableMeshDestroy();
					needRecreateModifiableMesh = false;
				}
			}

			if( DisplaySkeleton )
				RenderSkeleton( context.Owner, null );
		}

		public Skeleton GetSkeleton()
		{
			Skeleton skeleton = ReplaceSkeleton;
			if( skeleton == null )
				skeleton = ParentMeshInSpace?.Mesh.Value?.Skeleton;
			return skeleton;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		private void ParentMeshInSpace_GetRenderSceneDataAddToFrameData( MeshInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, ref RenderingPipeline.RenderSceneData.MeshItem item, ref bool skip )
		{
			if( !CalculateOnCPU )
			{
				var skeleton = GetSkeleton();
				if( skeleton != null )
				{
					//skip animated objects for reflection cubemap generation
					if( context.Owner.Mode == Viewport.ModeEnum.ReflectionProbeCubemap )
					{
						var probe = context.Owner.AnyData as ReflectionProbe;
						if( probe != null && !probe.AnimatedObjects )
						{
							skip = true;
							return;
						}
					}

					//can't calculate in thread because AnimationTriggerFired
					UpdateAnimationState();

					try
					{
						Update( skeleton, context );//, track, currentAnimationTime );
					}
					catch { }

					if( transformMatrixRelativeToSkin != null && transformMatrixRelativeToSkin.Length != 0 && RenderingSystem.SkeletalAnimation )
					{
						if( cachedAnimationData == null )
							cachedAnimationData = new RenderingPipeline.RenderSceneData.MeshItem.AnimationDataClass();
						var animationData = cachedAnimationData;

						animationData.BonesIndex = context.AnimationBonesData.Count;
						item.AnimationData = animationData;

						context.AnimationBonesData.Add( transformMatrixRelativeToSkin );


						////bool dualQuaternion = false;// GetSkinningMode( skeleton ) == Skeleton.SkinningModeEnum.DualQuaternion;
						////if( dualQuaternion )
						////	item.AnimationData.Mode = 2;
						////else
						//item.AnimationData.Mode = 1;

						////create dynamic texture
						//var size = new Vector2I( 4, MathEx.NextPowerOfTwo( transformMatrixRelativeToSkin.Length ) );

						//maybe optionally in the settings
						////use half on mobile
						//if( SystemSettings.LimitedDevice )
						//{
						//	//Float16RGBA

						//	var bonesTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, size, PixelFormat.Float16RGBA, 0, false );

						//	//try get array from texture to minimize memory allocations
						//	var surfaces = bonesTexture.Result.GetData();
						//	if( surfaces == null )
						//		surfaces = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( new byte[ size.X * size.Y * 8 ] ) };
						//	var data = surfaces[ 0 ].Data;

						//	//copy data to the texture
						//	unsafe
						//	{
						//		fixed( byte* pData2 = data.Array )
						//		{
						//			Matrix4H* pData = (Matrix4H*)pData2;
						//			for( int n = 0; n < transformMatrixRelativeToSkin.Length; n++ )
						//				transformMatrixRelativeToSkin[ n ].ToMatrix4H( out pData[ n ] );
						//		}
						//	}
						//	bonesTexture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( data ) } );

						//	item.AnimationData.BonesTexture = bonesTexture;
						//}
						//else
						//{
						//	//Float32RGBA

						//	var bonesTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, size, PixelFormat.Float32RGBA, 0, false );

						//	//try get array from texture to minimize memory allocations
						//	var surfaces = bonesTexture.Result.GetData();
						//	if( surfaces == null )
						//		surfaces = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( new byte[ size.X * size.Y * 16 ] ) };
						//	var data = surfaces[ 0 ].Data;

						//	//copy data to the texture
						//	unsafe
						//	{
						//		fixed( byte* pData2 = data.Array )
						//		{
						//			Matrix4F* pData = (Matrix4F*)pData2;
						//			for( int n = 0; n < transformMatrixRelativeToSkin.Length; n++ )
						//				pData[ n ] = transformMatrixRelativeToSkin[ n ];
						//		}
						//	}
						//	bonesTexture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( data ) } );

						//	item.AnimationData.BonesTexture = bonesTexture;
						//}
					}
				}
			}
		}

		public bool RenderSkeleton( Viewport viewport, ESet<object> selectedObjects )
		{
			// ParentMeshInSpace.Transform is automaticaly applyed to ParentMeshInSpace.Mesh, skeleton must be transformed manually
			var transformMatrix = ParentMeshInSpace?.Transform.Value?.ToMatrix4() ?? Matrix4.Identity;

			var globalBoneTransforms = GetBoneGlobalTransforms( true );
			if( globalBoneTransforms != null )
			{
				var renderer = viewport.Simple3DRenderer;

				for( int n = 0; n < bones.Length; n++ )
				{
					var bone = bones[ n ];

					var parentIndex = boneParents[ n ];
					if( parentIndex != -1 )
					{
						ref var tr0 = ref boneGlobalTransforms[ parentIndex ];
						ref var tr1 = ref boneGlobalTransforms[ n ];

						var pos0 = transformMatrix * tr0.Position;
						var pos1 = transformMatrix * tr1.Position;

						var selected = false;
						if( selectedObjects != null )
							selected = selectedObjects.Contains( bone );

						{
							var color = new ColorValue( 0, 0.5, 1, 0.7 );
							if( selected )
								color = ProjectSettings.Get.Colors.SelectedColor;

							renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							renderer.AddArrow( pos0, pos1 );
						}

						var length = (float)( pos1 - pos0 ).Length() / 5;

						{
							var color = new ColorValue( 1, 0, 0, 0.7 );
							renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							renderer.AddLine( pos1, pos1 + tr1.Rotation * new Vector3F( length, 0, 0 ) );
						}

						{
							var color = new ColorValue( 0, 1, 0, 0.7 );
							renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							renderer.AddLine( pos1, pos1 + tr1.Rotation * new Vector3F( 0, length, 0 ) );
						}

						{
							var color = new ColorValue( 0, 0, 1, 0.7 );
							renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							renderer.AddLine( pos1, pos1 + tr1.Rotation * new Vector3F( 0, 0, length ) );
						}
					}
				}

				return true;
			}

			return false;

			//var skeletonArrows = GetCurrentAnimatedSkeletonArrows();
			//if( skeletonArrows != null )
			//{
			//	var color = new ColorValue( 0, 0.5, 1, 0.7 ); //ToDo : Вынести в другое место.
			//	viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

			//	foreach( var arrow in skeletonArrows )
			//		viewport.Simple3DRenderer.AddArrow( transformMatrix * arrow.Start, transformMatrix * arrow.End );
			//}
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
				//сериализовывать

				//var animation = PlayAnimation.Value;
				//if( animation != null )
				//{
				UpdateAnimationState();

				//settings.animationStates = new AnimationStateItem[ 1 ];
				//settings.animationStates[ 0 ] = new AnimationStateItem( animation, currentLocalTime, 1 );

				//var skeletonAnimation = animation as SkeletonAnimation;
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

		public delegate void AnimationTriggerFiredDelegate( MeshInSpaceAnimationController sender, AnimationStateClass animationState, AnimationStateClass.AnimationItem animationItem, AnimationTrigger trigger );
		public event AnimationTriggerFiredDelegate AnimationTriggerFired;

		[MethodImpl( (MethodImplOptions)512 )]
		void UpdateAnimationState()
		{
			double t = EngineApp.EngineTime;
			double timeStep = t - currentEngineTime;
			currentEngineTime = t;

			if( timeStep > 0 )
			{
				var animations = animationState?.Animations;
				if( animations != null )
				{
					var interpolationTime = InterpolationTime.Value;

					Stack<int> itemsToRemove = null;

					for( int n = 0; n < animations.Count; n++ )
					{
						var item = animations[ n ];
						if( item.Animation == null )
							continue;

						//update CurrentTime, detect 'remove'

						bool remove = false;

						if( !item.CurrentTime.HasValue )
							item.CurrentTime = 0;

						var currentTime = item.CurrentTime.Value;
						var newTime = currentTime + timeStep * item.Speed;

						//triggers
						if( item.Animation.Components.Count != 0 )
						{
							//!!!!по идее триггеры лучше в OnUpdate, не во время рендера
							//!!!!threading

							//!!!!GC
							foreach( var trigger in item.Animation.GetComponents<AnimationTrigger>() )
							{
								if( trigger.Enabled )
								{
									var triggerTime = trigger.Time.Value;

									if( item.Speed > 0 )
									{
										if( currentTime < triggerTime && newTime >= triggerTime )
										{
											AnimationTriggerFired?.Invoke( this, animationState, item, trigger );

											var parentProcess = Parent as IParentAnimationTriggerProcess;
											parentProcess?.AnimationTriggerFired( this, animationState, item, trigger );
										}
									}
									else
									{
										if( currentTime > triggerTime && newTime <= triggerTime )
										{
											AnimationTriggerFired?.Invoke( this, animationState, item, trigger );

											var parentProcess = Parent as IParentAnimationTriggerProcess;
											parentProcess?.AnimationTriggerFired( this, animationState, item, trigger );
										}
									}
								}
							}
						}

						if( newTime >= item.Animation.Length )
						{
							if( item.FreezeOnEnd )
								newTime = item.Animation.Length;
							else if( item.AutoRewind )
							{
								//!!!!slowly maybe in some cases
								do
								{
									newTime -= item.Animation.Length;
								}
								while( newTime >= item.Animation.Length && item.Animation.Length > 0 );
							}
							else
								remove = true;
						}
						else if( newTime < 0 )
						{
							if( item.FreezeOnEnd )
								newTime = 0;
							else if( item.AutoRewind )
							{
								//!!!!slowly maybe in some cases
								do
								{
									newTime += item.Animation.Length;
								}
								while( newTime < 0 && item.Animation.Length > 0 );
							}
							else
								remove = true;
						}

						item.CurrentTime = newTime;


						//!!!!неперематываемые анимации фейдить вначале и в конце

						//update CurrentFactor

						var currentFactor = item.CurrentFactor.HasValue ? item.CurrentFactor.Value : 0.0f;

						if( currentFactor < item.Factor )
						{
							if( interpolationTime != 0 )
							{
								currentFactor += (float)( timeStep / interpolationTime );
								if( currentFactor > item.Factor )
									currentFactor = item.Factor;
							}
							else
								currentFactor = item.Factor;
						}
						else if( currentFactor > item.Factor )
						{
							if( interpolationTime != 0 )
							{
								currentFactor -= (float)( timeStep / interpolationTime );
								if( currentFactor < item.Factor )
									currentFactor = item.Factor;
							}
							else
								currentFactor = item.Factor;
						}

						if( currentFactor == 0 && item.InterpolationFading )
							remove = true;

						item.CurrentFactor = currentFactor;

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
							animations.RemoveAt( itemsToRemove.Pop() );
					}
				}
			}
		}

		public IList<Line3F> GetCurrentAnimatedSkeletonArrows()
		{
			var skeleton = GetSkeleton();
			if( skeleton == null )
				return null;

			//var skeleton = ReplaceSkeleton.Value ?? ParentMeshInSpace?.Mesh.Value?.Skeleton;
			//if( skeleton == null || PlayAnimation.Value == null )
			//	return null;
			//var skeletonAnimationTrack = ( PlayAnimation.Value as SkeletonAnimation )?.Track;
			//if( skeletonAnimationTrack == null )
			//	return null;

			UpdateAnimationState();
			Update( skeleton );//, skeletonAnimationTrack, currentAnimationTime );
			if( bones == null )
				return null;
			return GetSkeletonArrows();// skeleton );
		}

		/////////////////////////////////////////

		struct SourceChannel<TBuffer> where TBuffer : unmanaged
		{
			public bool Exists;
			VertexElement sourceElement;
			GpuVertexBuffer sourceBuffer;
			public TBuffer[] SourceData;

			public SourceChannel( RenderingPipeline.RenderSceneData.MeshDataRenderOperation sourceOper, VertexElementSemantic semantic, VertexElementType type )
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
				RenderingPipeline.RenderSceneData.MeshDataRenderOperation sourceOper,
				RenderingPipeline.RenderSceneData.MeshDataRenderOperation destOper,
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
				RenderingPipeline.RenderSceneData.MeshDataRenderOperation sourceOper,
				RenderingPipeline.RenderSceneData.MeshDataRenderOperation destOper,
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

		//Skeleton.SkinningModeEnum GetSkinningMode( Skeleton skeleton )
		//{
		//	var _override = OverrideSkinningMode.Value;
		//	if( _override != Skeleton.SkinningModeEnum.Auto )
		//		return _override;

		//	var selected = skeleton.SkinningMode.Value;
		//	if( selected != Skeleton.SkinningModeEnum.Auto )
		//		return selected;

		//	if( !hasScale )
		//		return Skeleton.SkinningModeEnum.DualQuaternion;
		//	else
		//		return Skeleton.SkinningModeEnum.Linear;
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		protected virtual void CalculateCPU( Skeleton skeleton, Mesh originalMesh, Mesh modifiableMesh )
		{
			//bool dualQuaternion = false;// GetSkinningMode( skeleton ) == Skeleton.SkinningModeEnum.DualQuaternion;

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
								//dualQuaternion,
								position.SourceData, normal.SourceData, tangent.SourceData, blendIndices.SourceData, blendWeights.SourceData,
								position.DestData, normal.DestData, tangent.DestData
							);
						}
						else
						{
							TransformVertices(
								//dualQuaternion,
								position.SourceData, normal.SourceData, blendIndices.SourceData, blendWeights.SourceData,
								position.DestData, normal.DestData
							);
						}
					}
					else
					{
						TransformVertices(
							//dualQuaternion,
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

		static void ResetToOriginalMesh( Mesh originalMesh, Mesh modifiableMesh )
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

			calculatedForTime = -1;
			boneTransforms = null;
			boneGlobalTransforms = null;
			bones = null;
			boneByName = null;
			boneParents = null;
			transformMatrixRelativeToSkin = null;
			//disable DQS
			//transformRelativeToSkin = null;
			//hasScale = false;
		}

		public delegate void CalculateBoneTransformsDelegate( MeshInSpaceAnimationController sender, SkeletonAnimationTrack.CalculateBoneTransformsItem[] result/*, ref bool handled*/ );
		public event CalculateBoneTransformsDelegate CalculateBoneTransforms;

		struct UpdateAnimationItem
		{
			//first way
			public Animation Animation;

			//second way. the ability to merge two animations
			public Animation Animation2;
			public float Animation2Factor;
			public int AnimationItemTag;

			public double CurrentTime;
			public float CurrentFactor;

			public bool ReplaceMode;

			public string[] AffectBonesWithChildren;
			//public string[] SkipBonesWithChildren;
		}

		void UpdateTaskMethod()
		{
			try
			{
				//calculate result global transforms
				CalculateGlobalBoneTransforms();

				var skeleton = GetSkeleton();
				//var skeletonNormalized = false;// skeleton != null && skeleton.Normalized;

				for( int n = 0; n < bones.Length; n++ )
				{
					var bone = bones[ n ];
					boneGlobalTransforms[ n ].ToMatrix( out var matrix );
					Matrix4F.Multiply( ref matrix, ref bone.GetTransformMatrixInverse( /*skeletonNormalized, animationState != null */), out transformMatrixRelativeToSkin[ n ] );
				}
			}
			catch { }
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void Update( Skeleton skeleton, ViewportRenderingContext context = null )
		{
			var time = EngineApp.EngineTime;
			if( time == calculatedForTime )
				return;

			var resetAnimation = true;

			//update common data
			calculatedForTime = time;
			//!!!!threading
			skeleton.GetBones( false, out bones, out boneByName, out boneParents );

			//calculate bone transforms

			if( boneTransforms == null || boneTransforms.Length != bones.Length )
				boneTransforms = new SkeletonAnimationTrack.CalculateBoneTransformsItem[ bones.Length ];
			for( int n = 0; n < boneTransforms.Length; n++ )
				boneTransforms[ n ].Flags = 0;

			var boneTransformsInitialized = false;

			if( animationState != null )
			{
				//!!!!threading
				if( tempAnimations == null )
					tempAnimations = new OpenList<UpdateAnimationItem>( animationState.Animations.Count );
				tempAnimations.Clear();
				var animations = tempAnimations;

				for( int n = 0; n < animationState.Animations.Count; n++ )
				{
					var item = animationState.Animations[ n ];
					if( item.CurrentFactor.HasValue && item.CurrentFactor > 0 )
					{
						var item2 = new UpdateAnimationItem();
						item2.Animation = item.Animation;
						item2.Animation2 = item.Animation2;
						item2.Animation2Factor = item.Animation2Factor;
						item2.AnimationItemTag = item.AnimationItemTag;

						item2.CurrentTime = item.CurrentTime.HasValue ? item.CurrentTime.Value : 0;
						item2.CurrentFactor = item.CurrentFactor.Value;

						item2.ReplaceMode = item.ReplaceMode;

						item2.AffectBonesWithChildren = item.AffectBonesWithChildren;
						//item2.SkipBonesWithChildren = item.SkipBonesWithChildren;

						animations.Add( ref item2 );
					}
				}

				if( animations.Count != 0 )
				{
					var transforms = stackalloc SkeletonAnimationTrack.CalculateBoneTransformsItem[ boneTransforms.Length ];
					var transforms2 = stackalloc SkeletonAnimationTrack.CalculateBoneTransformsItem[ boneTransforms.Length ];
					var affectBonesBuffer = stackalloc bool[ boneTransforms.Length ];

					if( animations.Count > 1 )
					{
						//several animations variant

						//normalize factors
						{
							var total = 0.0f;
							for( int n = 0; n < animations.Count; n++ )
							{
								ref var item = ref animations.Data[ n ];
								if( !item.ReplaceMode )
									total += item.CurrentFactor;
							}

							if( total != 0 )
							{
								for( int n = 0; n < animations.Count; n++ )
								{
									ref var item = ref animations.Data[ n ];
									if( !item.ReplaceMode )
										item.CurrentFactor /= total;
								}
							}
						}

						var count = 0;

						for( int nAnimation = 0; nAnimation < animations.Count; nAnimation++ )
						{
							ref var item = ref animations.Data[ nAnimation ];

							var skeletonAnimation = item.Animation as SkeletonAnimation; //!!!!slowly? where else
							if( skeletonAnimation != null )
							{
								var track = skeletonAnimation.Track.Value; //!!!!slowly? where else
								if( track != null )
								{
									//get affected bones when AffectBonesWithChildren initialized
									bool* affectBones = null;

									if( item.AffectBonesWithChildren != null )
									{
										affectBones = affectBonesBuffer;
										NativeUtility.ZeroMemory( affectBones, bones.Length );

										foreach( var rootBoneName in item.AffectBonesWithChildren )
										{
											var rootBoneIndex = GetBoneIndex( rootBoneName );
											if( rootBoneIndex != -1 )
												affectBones[ rootBoneIndex ] = true;
										}
										for( int n = 0; n < bones.Length; n++ )
										{
											var boneParent = boneParents[ n ];
											if( boneParent != -1 && affectBones[ boneParent ] )
												affectBones[ n ] = true;
										}
									}

									//!!!!
									//if( item.SkipBonesWithHierarchy != null )
									//{
									//	affectBones = new bool[ boneTransforms.Length ];
									//	for( int n = 0; n < affectBones.Length; n++ )
									//		affectBones[ n ] = true;

									//	foreach( var rootBoneName in item.SkipBonesWithHierarchy )
									//	{
									//		var rootBoneIndex = GetBoneIndex( rootBoneName );
									//		if( rootBoneIndex != -1 )
									//		{
									//			var rootBone = bones[ rootBoneIndex ];

									//			for( int n = 0; n < boneTransforms.Length; n++ )
									//			{
									//				var bone = bones[ n ];

									//				if( bone.GetAllParents().Contains( rootBone ) || rootBone == bone )
									//				{
									//					affectBones[ n ] = false;
									//				}
									//			}
									//		}
									//	}
									//}


									//calculate bone transforms
									//!!!!threading
									track.CalculateBoneTransforms( bones, boneByName, skeletonAnimation, skeletonAnimation.TrackStartTime + item.CurrentTime, transforms );

									//apply Animation2
									var skeletonAnimation2 = item.Animation2 as SkeletonAnimation;
									if( skeletonAnimation2 != null )
									{
										var track2 = skeletonAnimation2.Track.Value;
										if( track2 != null )
										{
											//!!!!threading
											track2.CalculateBoneTransforms( bones, boneByName, skeletonAnimation, skeletonAnimation.TrackStartTime + item.CurrentTime, transforms2 );

											for( int n = 0; n < boneTransforms.Length; n++ )
											{
												ref var transform1 = ref transforms[ n ];
												ref var transform2 = ref transforms2[ n ];

												Vector3F.Lerp( ref transform1.Position, ref transform2.Position, item.Animation2Factor, out transform1.Position );
												QuaternionF.Slerp( ref transform1.Rotation, ref transform2.Rotation, item.Animation2Factor, out transform1.Rotation );
												Vector3F.Lerp( ref transform1.Scale, ref transform2.Scale, item.Animation2Factor, out transform1.Scale );
											}
										}
									}

									for( int n = 0; n < boneTransforms.Length; n++ )
									{
										if( affectBones != null && !affectBones[ n ] )
											continue;

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
											if( item.ReplaceMode )
											{
												Vector3F.Lerp( ref boneDest.Position, ref boneSource.Position, item.CurrentFactor, out boneDest.Position );
												QuaternionF.Slerp( ref boneDest.Rotation, ref boneSource.Rotation, item.CurrentFactor, out boneDest.Rotation );
												Vector3F.Lerp( ref boneDest.Scale, ref boneSource.Scale, item.CurrentFactor, out boneDest.Scale );
											}
											else
											{
												boneDest.Position += boneSource.Position * item.CurrentFactor;
												QuaternionF.Slerp( ref boneDest.Rotation, ref boneSource.Rotation, item.CurrentFactor, out boneDest.Rotation );
												boneDest.Scale += boneSource.Scale * item.CurrentFactor;
											}
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
						//one animation variant

						ref var item = ref animations.Data[ 0 ];

						var skeletonAnimation = item.Animation as SkeletonAnimation;
						if( skeletonAnimation != null )
						{
							var track = skeletonAnimation?.Track.Value;
							if( track != null )
							{
								fixed( SkeletonAnimationTrack.CalculateBoneTransformsItem* transforms3 = boneTransforms )
									track.CalculateBoneTransforms( bones, boneByName, skeletonAnimation, skeletonAnimation.TrackStartTime + item.CurrentTime, transforms3 );

								boneTransformsInitialized = true;
							}
						}
					}

					if( boneTransformsInitialized )
					{
						////apply root bone rotation
						//if( boneTransforms.Length != 0 && animationState.RootBoneRotation != QuaternionF.Identity )
						//{
						//	ref var item = ref boneTransforms[ 0 ];
						//	item.Rotation = animationState.RootBoneRotation * item.Rotation;
						//}

						//calculate global bone transforms first time without additional modifications

						if( boneGlobalTransforms == null || boneGlobalTransforms.Length != bones.Length )
							boneGlobalTransforms = new SkeletonAnimationTrack.CalculateBoneTransformsItem[ bones.Length ];

						//need update all boneGlobalTransforms bones
						for( int n = 0; n < boneGlobalTransforms.Length; n++ )
							boneGlobalTransforms[ n ].NeedUpdate = true;

						//additional modifications
						if( animationState.AdditionalBoneTransformsUpdate != null )
							animationState.AdditionalBoneTransformsUpdate( this, animationState, skeleton, boneTransforms );

						//!!!!here?
						CalculateBoneTransforms?.Invoke( this, boneTransforms );


						if( transformMatrixRelativeToSkin == null || transformMatrixRelativeToSkin.Length != bones.Length )
							transformMatrixRelativeToSkin = new Matrix4F[ bones.Length ];


						//!!!!move to tasks more, but harder

						//calculate by means tasks
						//!!!!
						if( context != null )
						{
							var task = Task.Run( UpdateTaskMethod );
							context.AnimationBonesDataTasks.Add( task );
						}
						else
							UpdateTaskMethod();

						////calculate result global transforms
						//CalculateGlobalBoneTransforms();

						//for( int n = 0; n < bones.Length; n++ )
						//{
						//	var bone = bones[ n ];
						//	boneGlobalTransforms[ n ].ToMatrix( out var matrix );
						//	Matrix4F.Multiply( ref matrix, ref bone.GetTransformMatrixInverse(), out transformMatrixRelativeToSkin[ n ] );
						//}


						resetAnimation = false;


						////disable DQS
						////hasScale = false;

						////disable DQS
						////if( transformRelativeToSkin == null || transformRelativeToSkin.Length != bones.Length )
						////	transformRelativeToSkin = new SkeletonAnimationTrack.CalculateBoneTransformsItem[ bones.Length ];

						////disable DQS
						////m.Decompose( out Vector3F t, out QuaternionF r, out Vector3F s );
						////transformRelativeToSkin[ i ] = new SkeletonAnimationTrack.CalculateBoneTransformsItem { Position = t, Rotation = r, Scale = s };
						//////if the scale differs from 1.0 more than this value, then the scaling is present and DualQuaternionSkinning can not be used.
						////const float EpsilonForScale = 1e-3f;
						////if( Math.Abs( 1.0f - s.X ) > EpsilonForScale || Math.Abs( 1.0f - s.Y ) > EpsilonForScale || Math.Abs( 1.0f - s.Y ) > EpsilonForScale )
						////	hasScale = true;
					}
				}
			}
			else
			{
				//!!!!new
				//!!!!slowly

				for( int n = 0; n < boneTransforms.Length; n++ )
				{
					var transform = bones[ n ].Transform.Value;

					Transform parentTransform;
					if( boneParents[ n ] != -1 )
						parentTransform = bones[ boneParents[ n ] ].Transform.Value;
					else
						parentTransform = null;

					Matrix4 m;
					//if( skeleton.Normalized )
					//{
					//	//don't apply rotation and scale at this stage for normalized skeletons
					//	if( parentTransform != null )
					//		m = parentTransform.ToMatrix4( true, false, false ).GetInverse() * transform.ToMatrix4( true, false, false );
					//	else
					//		m = transform.ToMatrix4( true, false, false );
					//}
					//else
					//{
					if( parentTransform != null )
						m = parentTransform.ToMatrix4().GetInverse() * transform.ToMatrix4();
					else
						m = transform.ToMatrix4();
					//}

					m.Decompose( out Vector3 pos, out Quaternion rot, out Vector3 scl );

					ref var item = ref boneTransforms[ n ];
					item.Position = pos.ToVector3F();
					item.Rotation = rot.ToQuaternionF();
					item.Scale = scl.ToVector3F();
				}

				{
					//calculate global bone transforms first time without additional modifications

					if( boneGlobalTransforms == null || boneGlobalTransforms.Length != bones.Length )
						boneGlobalTransforms = new SkeletonAnimationTrack.CalculateBoneTransformsItem[ bones.Length ];

					//need update all boneGlobalTransforms bones
					for( int n = 0; n < boneGlobalTransforms.Length; n++ )
						boneGlobalTransforms[ n ].NeedUpdate = true;

					if( transformMatrixRelativeToSkin == null || transformMatrixRelativeToSkin.Length != bones.Length )
						transformMatrixRelativeToSkin = new Matrix4F[ bones.Length ];

					//calculate by means tasks
					if( context != null )
					{
						var task = Task.Run( UpdateTaskMethod );
						context.AnimationBonesDataTasks.Add( task );
					}
					else
						UpdateTaskMethod();

					resetAnimation = false;
				}
			}

			if( resetAnimation )
				ResetSkeletonAndAnimation( false );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void CalculateGlobalBoneTransforms()
		{
			for( int n = 0; n < bones.Length; n++ )
			{
				ref var boneGlobalTransform = ref boneGlobalTransforms[ n ];

				var parentIndex = boneParents[ n ];
				if( parentIndex != -1 )
				{
					if( boneGlobalTransforms[ parentIndex ].NeedUpdate || boneGlobalTransform.NeedUpdate )
					{
						ref var boneTransform = ref boneTransforms[ n ];
						ref var parentTransform = ref boneGlobalTransforms[ parentIndex ];

						//!!!!non uniform transform rotation depending scale?

						if( ( boneTransform.Flags & SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalPosition ) != 0 )
							boneGlobalTransform.Position = boneTransform.Position;
						else
						{
							Vector3F.Multiply( ref boneTransform.Position, ref parentTransform.Scale, out var v1 );
							QuaternionF.Multiply( ref parentTransform.Rotation, ref v1, out var v2 );
							Vector3F.Add( ref parentTransform.Position, ref v2, out boneGlobalTransform.Position );
							//boneGlobalTransform.Position = parentTransform.Position + parentTransform.Rotation * ( boneTransform.Position * parentTransform.Scale );
						}

						if( ( boneTransform.Flags & SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalRotation ) != 0 )
							boneGlobalTransform.Rotation = boneTransform.Rotation;
						else
						{
							QuaternionF.Multiply( ref parentTransform.Rotation, ref boneTransform.Rotation, out boneGlobalTransform.Rotation );
							//boneGlobalTransform.Rotation = parentTransform.Rotation * boneTransform.Rotation;
						}

						if( ( boneTransform.Flags & SkeletonAnimationTrack.CalculateBoneTransformsItem.FlagsEnum.GlobalScale ) != 0 )
							boneGlobalTransform.Scale = boneTransform.Scale;
						else
						{
							Vector3F.Multiply( ref parentTransform.Scale, ref boneTransform.Scale, out boneGlobalTransform.Scale );
							//boneGlobalTransform.Scale = parentTransform.Scale * boneTransform.Scale;
						}

						boneGlobalTransforms[ n ].NeedUpdate = true;
					}
				}
				else
				{
					if( boneGlobalTransform.NeedUpdate )
					{
						ref var boneTransform = ref boneTransforms[ n ];
						boneGlobalTransform.Position = boneTransform.Position;
						boneGlobalTransform.Rotation = boneTransform.Rotation;
						boneGlobalTransform.Scale = boneTransform.Scale;
					}
				}
			}

			for( int n = 0; n < boneGlobalTransforms.Length; n++ )
				boneGlobalTransforms[ n ].NeedUpdate = false;
		}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//ref Matrix4F GetBoneGlobalTransformRecursive( Skeleton skeleton, SkeletonBone bone )
		//{
		//	int boneIndex = bone.GetCachedBoneIndex( skeleton );

		//	if( !boneGlobalTransforms[ boneIndex ].HasValue )
		//	{
		//		ToMatrix4( ref boneTransforms[ boneIndex ], out var res );
		//		if( bone.Parent is SkeletonBone parent )
		//			res = GetBoneGlobalTransformRecursive( skeleton, parent ) * res;
		//		boneGlobalTransforms[ boneIndex ] = new BoneGlobalTransformItem( true, ref res );
		//	}
		//	return ref boneGlobalTransforms[ boneIndex ].Value;
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		IList<Line3F> GetSkeletonArrows()// Skeleton skeleton )
		{
			var result = new List<Line3F>( bones.Length );

			for( int n = 0; n < bones.Length; n++ )
			{
				var parentIndex = boneParents[ n ];
				if( parentIndex != -1 )
				{
					ref var tr0 = ref boneGlobalTransforms[ parentIndex ];
					ref var tr1 = ref boneGlobalTransforms[ n ];
					result.Add( new Line3F( tr0.Position, tr1.Position ) );
				}
			}

			//for( int i = 0; i < bones.Length; i++ )
			//{
			//	var b1 = bones[ i ];
			//	if( !( b1.Parent is SkeletonBone b0 ) )
			//		continue;

			//	var parentIndex = b0.GetCachedBoneIndex( skeleton );
			//	if( parentIndex == -1 )
			//		continue;

			//	ref var m0 = ref boneGlobalTransforms[ parentIndex ];
			//	ref var m1 = ref boneGlobalTransforms[ i ];
			//	if( m0.HasValue && m1.HasValue )
			//	{
			//		m0.Value.GetTranslation( out var t0 );
			//		m1.Value.GetTranslation( out var t1 );
			//		result.Add( new Line3F( t0, t1 ) );
			//	}
			//}

			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void TransformVertices(
			//bool dualQuaternion,
			Vector3F[] position, Vector3F[] normal, Vector4F[] tangent, Vector4I[] blendIndex, Vector4F[] blendWeight,
			Vector3F[] newPosition, Vector3F[] newNormal, Vector4F[] newTangent )
		{
			//if( dualQuaternion )
			//{
			//for( int n = 0; n < newPosition.Length; n++ )
			//{
			//	newPosition[ n ] = TransformByDualQuaternionSkinning(
			//		position[ n ], normal[ n ], tangent[ n ], blendIndex[ n ], blendWeight[ n ],
			//		out newNormal[ n ], out newTangent[ n ] );
			//}
			//}
			//else
			//{

			for( int n = 0; n < newPosition.Length; n++ )
			{
				TransformVertexByLinearBlendingSkinning(
				   ref position[ n ], ref normal[ n ], ref tangent[ n ], ref blendIndex[ n ], ref blendWeight[ n ],
				   out newPosition[ n ], out newNormal[ n ], out newTangent[ n ] );
			}

			//}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void TransformVertices(
			//bool dualQuaternion,
			Vector3F[] position, Vector3F[] normal, Vector4I[] blendIndex, Vector4F[] blendWeight,
			Vector3F[] newPosition, Vector3F[] newNormal )
		{
			//if( dualQuaternion )
			//{
			//for( int n = 0; n < newPosition.Length; n++ )
			//{
			//	newPosition[ n ] = TransformByDualQuaternionSkinning(
			//		position[ n ], normal[ n ], Vector4F.One, blendIndex[ n ], blendWeight[ n ],
			//		out newNormal[ n ], out _ );
			//}
			//}
			//else
			//{

			var vector4FOne = Vector4F.One;

			for( int n = 0; n < newPosition.Length; n++ )
			{
				TransformVertexByLinearBlendingSkinning(
					ref position[ n ], ref normal[ n ], ref vector4FOne, ref blendIndex[ n ], ref blendWeight[ n ],
					out newPosition[ n ], out newNormal[ n ], out _ );
			}

			//}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void TransformVertices(
			//bool dualQuaternion,
			Vector3F[] position, Vector4I[] blendIndex, Vector4F[] blendWeight,
			Vector3F[] newPosition )
		{
			//if( dualQuaternion )
			//{
			//disable DQS
			//for( int n = 0; n < newPosition.Length; n++ )
			//	newPosition[ n ] = TransformByDualQuaternionSkinning( position[ n ], blendIndex[ n ], blendWeight[ n ] );
			//}
			//else
			//{

			for( int n = 0; n < newPosition.Length; n++ )
				TransformVertexByLinearBlendingSkinning( ref position[ n ], ref blendIndex[ n ], ref blendWeight[ n ], out newPosition[ n ] );

			//}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void TransformVertexByLinearBlendingSkinning( ref Vector3F position, ref Vector4I blendIndex, ref Vector4F blendWeight, out Vector3F newPosition )
		{
			var position4 = new Vector4F( position, 1.0f );
			if( !GetVertexTransformByLinearBlendingSkinning( ref blendIndex, ref blendWeight, out Matrix4F matrix ) )
				newPosition = position;
			else
			{
				Matrix4F.Multiply( ref matrix, ref position4, out var newPosition4 );
				newPosition = newPosition4.ToVector3F();
				//result = ( matrix * position4 ).ToVector3F();
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void TransformVertexByLinearBlendingSkinning( ref Vector3F position, ref Vector3F normal, ref Vector4F tangent, ref Vector4I blendIndex, ref Vector4F blendWeight, out Vector3F newPosition, out Vector3F newNormal, out Vector4F newTangent )
		{
			var position4 = new Vector4F( position, 1.0f );
			if( !GetVertexTransformByLinearBlendingSkinning( ref blendIndex, ref blendWeight, out var matrix ) )
			{
				newPosition = position;
				newNormal = normal;
				newTangent = tangent;
			}
			else
			{
				Matrix4F.Multiply( ref matrix, ref position4, out var newPosition4 );
				newPosition = newPosition4.ToVector3F();
				//newPosition = ( matrix * position4 ).ToVector3F();

				matrix.Decompose( out _, out QuaternionF rot, out _ );
				CalculateBlendNormal( ref normal, ref rot, out newNormal );
				CalculateBlendTangentNormal( ref tangent, ref rot, out newTangent );
				//newTangent = new Vector4F( CalculateBlendNormal( tangent.ToVector3F(), rot ), tangent.W );
			}
		}

		//disable DQS
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

		//disable DQS
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void CalculateBlendNormal( ref Vector3F vIn, ref QuaternionF blendQ, out Vector3F result )
		{
			var blendDQ_0_yzw = new Vector3F( blendQ.X, blendQ.Y, blendQ.Z );
			var blendDQ_0_x = blendQ.W;

			Vector3F.Cross( ref blendDQ_0_yzw, ref vIn, out var cross1 );
			var p = cross1 + blendDQ_0_x * vIn;
			Vector3F.Cross( ref blendDQ_0_yzw, ref p, out var cross2 );
			result = vIn + 2.0f * cross2;
			//result = vIn + 2.0f * Vector3F.Cross( blendDQ_0_yzw, Vector3F.Cross( blendDQ_0_yzw, vIn ) + blendDQ_0_x * vIn );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void CalculateBlendTangentNormal( ref Vector4F vIn, ref QuaternionF blendQ, out Vector4F result )
		{
			var vIn3 = vIn.ToVector3F();

			var blendDQ_0_yzw = new Vector3F( blendQ.X, blendQ.Y, blendQ.Z );
			var blendDQ_0_x = blendQ.W;

			Vector3F.Cross( ref blendDQ_0_yzw, ref vIn3, out var cross1 );
			var p = cross1 + blendDQ_0_x * vIn3;
			Vector3F.Cross( ref blendDQ_0_yzw, ref p, out var cross2 );
			result = new Vector4F( vIn3 + 2.0f * cross2, vIn.W );
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

		////Warning : Ogre HLSL has W component of Quaternion in the first (X) component of vector.
		//// Also described in article: http://dev.theomader.com/dual-quaternion-skinning/
		//Vector3F CalculateBlendPosition( Vector3F position, DualQuaternionF blendDQ )
		//{
		//	Vector3F blendDQ_0_yzw = new Vector3F( blendDQ.Q0.X, blendDQ.Q0.Y, blendDQ.Q0.Z );
		//	Vector3F blendDQ_1_yzw = new Vector3F( blendDQ.Qe.X, blendDQ.Qe.Y, blendDQ.Qe.Z );
		//	float blendDQ_0_x = blendDQ.Q0.W;
		//	float blendDQ_1_x = blendDQ.Qe.W;

		//	var blendPosition = position + 2.0f * Vector3F.Cross( blendDQ_0_yzw, Vector3F.Cross( blendDQ_0_yzw, position ) + blendDQ_0_x * position );
		//	var trans = 2.0f * ( blendDQ_0_x * blendDQ_1_yzw - blendDQ_1_x * blendDQ_0_yzw + Vector3F.Cross( blendDQ_0_yzw, blendDQ_1_yzw ) );
		//	blendPosition += trans;
		//	return blendPosition;
		//}

		/////////////////////////////////////////

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//static void ToMatrix4( ref SkeletonAnimationTrack.CalculateBoneTransformsItem keyframe, out Matrix4F result )
		//{
		//	keyframe.Rotation.ToMatrix3( out var rot );
		//	Matrix3F.FromScale( ref keyframe.Scale, out var scl );
		//	Matrix3F.Multiply( ref rot, ref scl, out var rot2 );
		//	result = new Matrix4F( rot2, keyframe.Position );
		//	//return new Matrix4F( keyframe.Rotation.ToMatrix3() * Matrix3F.FromScale( keyframe.Scale ), keyframe.Position );
		//}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//static float Dot( ref QuaternionF q1, ref QuaternionF q2 )
		//{
		//	return q1.W * q2.W + q1.X * q2.X + q1.Y * q2.Y + q1.Z * q2.Z;
		//}

		//struct DualQuaternionF
		//{
		//	public QuaternionF Q0, Qe;

		//	public DualQuaternionF( QuaternionF q, Vector3F t )
		//	{
		//		float w = -0.5f * ( t.X * q.X + t.Y * q.Y + t.Z * q.Z );
		//		float x = 0.5f * ( t.X * q.W + t.Y * q.Z - t.Z * q.Y );
		//		float y = 0.5f * ( -t.X * q.Z + t.Y * q.W + t.Z * q.X );
		//		float z = 0.5f * ( t.X * q.Y - t.Y * q.X + t.Z * q.W );

		//		Q0 = q;
		//		Qe = new QuaternionF( x, y, z, w );
		//	}

		//	public static DualQuaternionF operator +( DualQuaternionF dq1, DualQuaternionF dq2 )
		//	{
		//		return new DualQuaternionF
		//		{
		//			Q0 = dq1.Q0 + dq2.Q0,
		//			Qe = dq1.Qe + dq2.Qe
		//		};
		//	}

		//	public static DualQuaternionF operator *( DualQuaternionF dq, float scalar )
		//	{
		//		return new DualQuaternionF
		//		{
		//			Q0 = dq.Q0 * scalar,
		//			Qe = dq.Qe * scalar
		//		};
		//	}

		//	public void Normalize()
		//	{
		//		float lengthInverse = 1 / Q0.Length();
		//		Q0 *= lengthInverse;
		//		Qe *= lengthInverse;
		//	}

		//	////Перед вызовом сделать Normalize()
		//	//public void ToRotationTranslation( out Quaternion rot, out Vector3 translation )
		//	//{
		//	//	rot = q0;
		//	//	translation = new Vector3(
		//	//		2.0 * ( -qe.W * q0.X + qe.X * q0.W - qe.Y * q0.Z + qe.Z * q0.Y ),
		//	//		2.0 * ( -qe.W * q0.Y + qe.X * q0.Z + qe.Y * q0.W - qe.Z * q0.X ),
		//	//		2.0 * ( -qe.W * q0.Z - qe.X * q0.Y + qe.Y * q0.X + qe.Z * q0.W )
		//	//	);
		//	//}
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		bool GetVertexTransformByLinearBlendingSkinning( ref Vector4I blendIndex, ref Vector4F blendWeight, out Matrix4F matrix )
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

		//disable DQS
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

		/// <summary>
		/// Gets the engine time the animation was last updated.
		/// </summary>
		[Browsable( false )]
		public double CurrentEngineTime
		{
			get { return currentEngineTime; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int GetBoneIndex( string name )
		{
			if( boneByName != null && boneByName.TryGetValue( name, out var result ) )
				return result;
			return -1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int GetBoneIndex( SkeletonBone bone )
		{
			return GetBoneIndex( bone.Name );
		}

		public SkeletonAnimationTrack.CalculateBoneTransformsItem[] GetBoneGlobalTransforms( bool callUpdate = false )
		{
			if( callUpdate )
			{
				var skeleton = GetSkeleton();
				if( skeleton == null )
					return null;

				UpdateAnimationState();
				Update( skeleton );
				if( bones == null )
					return null;
			}

			return boneGlobalTransforms;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool GetBoneGlobalTransform( int boneIndex, out SkeletonAnimationTrack.CalculateBoneTransformsItem item )
		{
			if( boneGlobalTransforms != null && boneIndex < boneGlobalTransforms.Length )
			{
				item = boneGlobalTransforms[ boneIndex ];
				return true;
			}
			else
			{
				item = new SkeletonAnimationTrack.CalculateBoneTransformsItem();
				return false;
			}
		}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//public bool GetBoneGlobalTransform( int boneIndex, out Matrix4F value )
		//{
		//	if( boneGlobalTransforms != null && boneIndex < boneGlobalTransforms.Length )
		//	{
		//		ref var item = ref boneGlobalTransforms[ boneIndex ];
		//		if( item.HasValue )
		//		{
		//			value = item.Value;
		//			return true;
		//		}
		//	}
		//	value = Matrix4F.Identity;
		//	return false;
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		public void SetAnimationState( AnimationStateClass newState, bool allowInterpolation )
		{
			//!!!!GC because SetAnimationState is called each update?

			//!!!!threading

			//!!!!объекты вдалеке можно реже обновлять
			//!!!!в Character вообще не обновлять анимации пока не видно. хотя есть триггеры и может что-то еще

			var oldValue = animationState;
			animationState = newState;

			if( animationState != null && oldValue != null )
			{
				for( int nState = 0; nState < animationState.Animations.Count; nState++ )
				{
					var state = animationState.Animations[ nState ];

					//copy CurrentTime from old to new if it not initialized
					if( state.CurrentTime == null || state.CurrentFactor == null )
					{
						//find state in old list of animations with same animation of the 'state'

						AnimationStateClass.AnimationItem oldState;
						if( state.AnimationItemTag != 0 )
							oldState = oldValue.FindItemByAnimationItemTag( state.AnimationItemTag );
						else
							oldState = oldValue.FindItemWithoutAnimationItemTag( state.Animation );

						if( oldState != null )
						{
							if( oldState.CurrentTime != null )
								state.CurrentTime = oldState.CurrentTime;
							if( oldState.CurrentFactor != null )
								state.CurrentFactor = oldState.CurrentFactor;
						}
					}
				}

				//copy from old
				if( allowInterpolation )
				{
					for( int nOldState = 0; nOldState < oldValue.Animations.Count; nOldState++ )
					{
						var oldState = oldValue.Animations[ nOldState ];

						AnimationStateClass.AnimationItem state;
						if( oldState.AnimationItemTag != 0 )
							state = animationState.FindItemByAnimationItemTag( oldState.AnimationItemTag );
						else
							state = animationState.FindItemWithoutAnimationItemTag( oldState.Animation );

						if( state == null )
						{
							oldState.InterpolationFading = true;
							oldState.Factor = 0;

							var added = false;

							//insert before ReplaceMode animations when it not ReplaceMode animation
							if( !oldState.ReplaceMode )
							{
								for( int n = animationState.Animations.Count - 1; n >= 0; n-- )
								{
									if( !animationState.Animations[ n ].ReplaceMode )
									{
										animationState.Animations.Insert( n + 1, oldState );
										added = true;
									}
								}
							}

							if( !added )
								animationState.Animations.Add( oldState );
						}
					}
				}
			}
		}

		[Browsable( false )]
		public SkeletonBone[] Bones
		{
			get { return bones; }
		}

		[Browsable( false )]
		public int[] BoneParents
		{
			get { return boneParents; }
		}
	}
}
