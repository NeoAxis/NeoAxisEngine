// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// A mesh instance in the scene.
	/// </summary>
	public class MeshInSpace : ObjectInSpace, IPhysicalObject
	{
		static FastRandom staticRandom = new FastRandom( 0 );

		//creation
		Mesh.CompiledData usedMeshDataWhenInitialized;

		//modifiable mesh
		const string modifiableMeshName = "ModifiableMesh";
		Mesh modifiableMesh;
		List<ThreadSafeDisposable> modifiableMeshBuffersToDispose;
		Component modifiableMeshCreatedByObject;

		double transformPositionByTime1_Time;
		Vector3 transformPositionByTime1_Position;
		double transformPositionByTime2_Time;
		Vector3 transformPositionByTime2_Position;

		protected virtual void OnGetRenderSceneDataAddToFrameData( ViewportRenderingContext context, GetRenderSceneDataMode mode, ref RenderingPipeline.RenderSceneData.MeshItem item, ref bool skip ) { }

		public delegate void GetRenderSceneDataAddToFrameDataDelegate( MeshInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, ref RenderingPipeline.RenderSceneData.MeshItem item, ref bool skip );
		public event GetRenderSceneDataAddToFrameDataDelegate GetRenderSceneDataAddToFrameData;

		//List<SceneLODUtility.RenderingContextItem> renderingContextItems;

		bool paintLayersNeedUpdate = true;
		RenderingPipeline.RenderSceneData.LayerItem[] paintLayers;

		bool transparentRenderingAddOffsetWhenSortByDistance;
		//[Browsable( false )]
		//public bool TransparentRenderingAddOffsetWhenSortByDistance { get; set; }

		//!!!!optimize when relative camera rendering will introducted
		Transform occluderCacheTransform;
		Vector3F[] occluderCacheVerticesOriginal;
		Vector3[] occluderCacheVertices;
		int[] occluderCacheIndices;

		//collision
		Scene.PhysicsWorldClass.Body physicalBody;
		Vector3 physicalBodyCreatedTransformScale;
		bool duringCreateDestroy;
		bool updatePropertiesWithoutUpdatingBody;
		Sound collisionSound;
		float collisionSoundTimeInterval;
		double collisionSoundMinSpeedChange;
		float collisionSoundRemainingTime;
		//!!!!contact data

		//static mode
		bool needUpdateStaticModeAndStaticShadows;
		bool staticModeEnabled;
		GroupOfObjectsUtility.GroupOfObjectsInstance staticModeGroupOfObjects;
		List<GroupOfObjects.SubGroup> staticModeGroupOfObjectSubGroups;

		AdditionalItem[] additionalItems;
		AdditionalItemPreviousTransform[] additionalItemsPreviousTransform;

		[Browsable( false )]
		public Transform TransformVisualOverride { get; set; }

		//!!!!такое не работает когда ссылки есть у параметров
		//!!!!
		//!!!!надо затирать невыставленные. то что выставляется, должно обнуляться
		//RenderingPipeline.RenderSceneData.MeshItem item;

		/////////////////////////////////////////

		//public interface IMeshInSpaceChild
		//{
		//	void ParentMeshInSpace_GetRenderSceneData();
		//}

		/////////////////////////////////////////

		/// <summary>
		/// The mesh used by the mesh object.
		/// </summary>
		[Serialize]
		public Reference<Mesh> Mesh
		{
			get
			{
				//!!!!MeshChanged event
				//fast exit optimization
				var cachedReference = _mesh.value.Value?.GetCachedResourceReference();
				if( cachedReference != null )
				{
					if( ReferenceEquals( _mesh.value.GetByReference, cachedReference ) )
						return _mesh.value;
					if( _mesh.value.GetByReference == cachedReference )
					{
						_mesh.value.GetByReference = cachedReference;
						return _mesh.value;
					}
				}

				if( _mesh.BeginGet_WithoutFastExitOptimization() ) Mesh = _mesh.Get( this ); return _mesh.value;
				//if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value;
			}
			set
			{
				if( _mesh.BeginSet( this, ref value ) )
				{
					try
					{
						MeshChanged?.Invoke( this );
						//!!!!может не сразу обновлять? хотя для Static оно самоудалится в очереди обновления
						RecreateData();
					}
					finally { _mesh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<MeshInSpace> MeshChanged;
		ReferenceField<Mesh> _mesh;

		/// <summary>
		/// Whether to enable a static mode which improves rendering speed. Internally static objects are grouped into batches.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Static
		{
			get { if( _static.BeginGet() ) Static = _static.Get( this ); return _static.value; }
			set
			{
				if( _static.BeginSet( this, ref value ) )
				{
					try
					{
						StaticChanged?.Invoke( this );

						if( EnabledInHierarchy )
							ParentScene?.ObjectsInSpace_ObjectUpdateSceneObjectFlags( this );
						NeedUpdateStaticModeAndStaticShadows();

						//if( !_static.value )
						//	ResetLightStaticShadowsCache();
					}
					finally { _static.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Static"/> property value changes.</summary>
		public event Action<MeshInSpace> StaticChanged;
		ReferenceField<bool> _static = false;

		/// <summary>
		/// Whether to allow static shadow optimization for this object.
		/// </summary>
		[DefaultValue( true )]// false )]
		public Reference<bool> StaticShadows
		{
			get { if( _staticShadows.BeginGet() ) StaticShadows = _staticShadows.Get( this ); return _staticShadows.value; }
			set
			{
				if( _staticShadows.BeginSet( this, ref value ) )
				{
					try
					{
						StaticShadowsChanged?.Invoke( this );
						NeedUpdateStaticModeAndStaticShadows();
						//if( EnabledInHierarchyAndIsInstance )
						//	ResetLightStaticShadowsCache();
					}
					finally { _staticShadows.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="StaticShadows"/> property value changes.</summary>
		public event Action<MeshInSpace> StaticShadowsChanged;
		ReferenceField<bool> _staticShadows = true;//false;

		/// <summary>
		/// Replaces all geometries of the mesh by another material.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		public Reference<Material> ReplaceMaterial
		{
			get { if( _replaceMaterial.BeginGet() ) ReplaceMaterial = _replaceMaterial.Get( this ); return _replaceMaterial.value; }
			set { if( _replaceMaterial.BeginSet( this, ref value ) ) { try { ReplaceMaterialChanged?.Invoke( this ); NeedUpdateStaticModeAndStaticShadows(); } finally { _replaceMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReplaceMaterial"/> property value changes.</summary>
		public event Action<MeshInSpace> ReplaceMaterialChanged;
		ReferenceField<Material> _replaceMaterial;

		//disabled. still never used but take 2 memory allocations
		///// <summary>
		///// Replaces selected geometries of the mesh by another material.
		///// </summary>
		//[Serialize]
		//[Cloneable( CloneType.Deep )]
		//public ReferenceList<Material> ReplaceMaterialSelectively
		//{
		//	get { return _replaceMaterialSelectively; }
		//}
		//public delegate void ReplaceMaterialSelectivelyChangedDelegate( MeshInSpace sender );
		//public event ReplaceMaterialSelectivelyChangedDelegate ReplaceMaterialSelectivelyChanged;
		//ReferenceList<Material> _replaceMaterialSelectively;

		/// <summary>
		/// The base color and opacity multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set
			{
				if( _color.BeginSet( this, ref value ) )
				{
					try
					{
						ColorChanged?.Invoke( this );
						NeedUpdateStaticModeAndStaticShadows();
						//item.Color = Color;
						//item.ColorForInstancingData = RenderingPipeline.GetColorForInstancingData( ref item.Color );
					}
					finally { _color.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<MeshInSpace> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

		/// <summary>
		/// The factor of maximum visibility distance. The maximum distance is calculated based on the size of the object on the screen.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> VisibilityDistanceFactor
		{
			get { if( _visibilityDistanceFactor.BeginGet() ) VisibilityDistanceFactor = _visibilityDistanceFactor.Get( this ); return _visibilityDistanceFactor.value; }
			set { if( _visibilityDistanceFactor.BeginSet( this, ref value ) ) { try { VisibilityDistanceFactorChanged?.Invoke( this ); NeedUpdateStaticModeAndStaticShadows(); } finally { _visibilityDistanceFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<MeshInSpace> VisibilityDistanceFactorChanged;
		ReferenceField<double> _visibilityDistanceFactor = 1.0;

		///// <summary>
		///// Maximum visibility range of the object.
		///// </summary>
		//[DefaultValue( 10000.0 )]
		//[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> VisibilityDistance
		//{
		//	get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
		//	set { if( _visibilityDistance.BeginSet( this, ref value ) ) { try { VisibilityDistanceChanged?.Invoke( this ); } finally { _visibilityDistance.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		//public event Action<MeshInSpace> VisibilityDistanceChanged;
		//ReferenceField<double> _visibilityDistance = 10000.0;

		/// <summary>
		/// Whether to cast shadows on the other surfaces.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		public Reference<bool> CastShadows
		{
			get { if( _castShadows.BeginGet() ) CastShadows = _castShadows.Get( this ); return _castShadows.value; }
			set { if( _castShadows.BeginSet( this, ref value ) ) { try { CastShadowsChanged?.Invoke( this ); NeedUpdateStaticModeAndStaticShadows(); } finally { _castShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<MeshInSpace> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// Whether it is possible to apply decals on the surface.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> ReceiveDecals
		{
			get { if( _receiveDecals.BeginGet() ) ReceiveDecals = _receiveDecals.Get( this ); return _receiveDecals.value; }
			set
			{
				if( _receiveDecals.BeginSet( this, ref value ) )
				{
					try
					{
						ReceiveDecalsChanged?.Invoke( this );
						NeedUpdateStaticModeAndStaticShadows();
						//item.ReceiveDecals = ReceiveDecals;
					}
					finally { _receiveDecals.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<MeshInSpace> ReceiveDecalsChanged;
		ReferenceField<bool> _receiveDecals = true;

		/// <summary>
		/// The multiplier of the motion blur effect.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		public Reference<double> MotionBlurFactor
		{
			get { if( _motionBlurFactor.BeginGet() ) MotionBlurFactor = _motionBlurFactor.Get( this ); return _motionBlurFactor.value; }
			set
			{
				if( _motionBlurFactor.BeginSet( this, ref value ) )
				{
					try
					{
						MotionBlurFactorChanged?.Invoke( this );
						NeedUpdateStaticModeAndStaticShadows();
						//item.MotionBlurFactor = (float)MotionBlurFactor;
					}
					finally { _motionBlurFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotionBlurFactor"/> property value changes.</summary>
		public event Action<MeshInSpace> MotionBlurFactorChanged;
		ReferenceField<double> _motionBlurFactor = 1.0;

		/// <summary>
		/// Whether to object should be used as an occluder for occlusion culling.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Occluder
		{
			get { if( _occluder.BeginGet() ) Occluder = _occluder.Get( this ); return _occluder.value; }
			set
			{
				if( _occluder.BeginSet( this, ref value ) )
				{
					try
					{
						OccluderChanged?.Invoke( this );

						if( EnabledInHierarchy )
							ParentScene?.ObjectsInSpace_ObjectUpdateSceneObjectFlags( this );
					}
					finally { _occluder.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Occluder"/> property value changes.</summary>
		public event Action<MeshInSpace> OccluderChanged;
		ReferenceField<bool> _occluder = false;

		/// <summary>
		/// Specifies settings for special object effects, such as an outline effect.
		/// </summary>
		[Cloneable( CloneType.Deep )]
		public Reference<List<ObjectSpecialRenderingEffect>> SpecialEffects
		{
			get { if( _specialEffects.BeginGet() ) SpecialEffects = _specialEffects.Get( this ); return _specialEffects.value; }
			set { if( _specialEffects.BeginSet( this, ref value ) ) { try { SpecialEffectsChanged?.Invoke( this ); } finally { _specialEffects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SpecialEffects"/> property value changes.</summary>
		public event Action<MeshInSpace> SpecialEffectsChanged;
		ReferenceField<List<ObjectSpecialRenderingEffect>> _specialEffects = new List<ObjectSpecialRenderingEffect>();

		/// <summary>
		/// Whether to add a collision body. A shape of the body will take from Collision Definition of the mesh.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Collision
		{
			get { if( _collision.BeginGet() ) Collision = _collision.Get( this ); return _collision.value; }
			set { if( _collision.BeginSet( this, ref value ) ) { try { CollisionChanged?.Invoke( this ); RecreateBody(); } finally { _collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Collision"/> property value changes.</summary>
		public event Action<MeshInSpace> CollisionChanged;
		ReferenceField<bool> _collision = false;

		/// <summary>
		/// The initial linear velocity of the body.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[NetworkSynchronize( false )]//don't sync to optimize networking
		public Reference<Vector3> PhysicalBodyLinearVelocity
		{
			get { if( _physicalBodyLinearVelocity.BeginGet() ) PhysicalBodyLinearVelocity = _physicalBodyLinearVelocity.Get( this ); return _physicalBodyLinearVelocity.value; }
			set
			{
				if( _physicalBodyLinearVelocity.BeginSet( this, ref value ) )
				{
					try
					{
						PhysicalBodyLinearVelocityChanged?.Invoke( this );
						if( physicalBody != null && !updatePropertiesWithoutUpdatingBody )
							physicalBody.LinearVelocity = value.Value.ToVector3F();
					}
					finally { _physicalBodyLinearVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="PhysicalBodyLinearVelocity"/> property value changes.</summary>
		public event Action<MeshInSpace> PhysicalBodyLinearVelocityChanged;
		ReferenceField<Vector3> _physicalBodyLinearVelocity = Vector3.Zero;

		/// <summary>
		/// The initial angular velocity of the body.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[NetworkSynchronize( false )]//don't sync to optimize networking
		public Reference<Vector3> PhysicalBodyAngularVelocity
		{
			get { if( _physicalBodyAngularVelocity.BeginGet() ) PhysicalBodyAngularVelocity = _physicalBodyAngularVelocity.Get( this ); return _physicalBodyAngularVelocity.value; }
			set
			{
				if( _physicalBodyAngularVelocity.BeginSet( this, ref value ) )
				{
					try
					{
						PhysicalBodyAngularVelocityChanged?.Invoke( this );
						if( physicalBody != null && !updatePropertiesWithoutUpdatingBody )
							physicalBody.AngularVelocity = value.Value.ToVector3F();
					}
					finally { _physicalBodyAngularVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="PhysicalBodyAngularVelocity"/> property value changes.</summary>
		public event Action<MeshInSpace> PhysicalBodyAngularVelocityChanged;
		ReferenceField<Vector3> _physicalBodyAngularVelocity = Vector3.Zero;

		[Browsable( false )]
		public RenderingPipeline.RenderSceneData.LayerItem[] PaintLayersReplace
		{
			get { return paintLayersReplace; }
			set
			{
				paintLayersReplace = value;
				paintLayersNeedUpdate = true;
			}
		}
		RenderingPipeline.RenderSceneData.LayerItem[] paintLayersReplace;

		[Browsable( false )]
		public RenderingPipeline.RenderSceneData.CutVolumeItem[] CutVolumes { get; set; }

		[Browsable( false )]
		public AdditionalItem[] AdditionalItems
		{
			get { return additionalItems; }
			set
			{
				additionalItems = value;
				NeedUpdateStaticModeAndStaticShadows();
			}
		}

		[Browsable( false )]
		public bool EditorAllowRenderSelection { get; set; } = true;

		///////////////////////////////////////////////

		public struct AdditionalItem
		{
			public Mesh Mesh;
			//!!!!enum TransformSpace: World, Local?
			public Vector3 Position;
			public Quaternion Rotation;
			public Vector3 Scale;
			public ColorValue Color;
			public Material ReplaceMaterial;

			public AdditionalItem( Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, ColorValue color )
			{
				Mesh = mesh;
				Position = position;
				Rotation = rotation;
				Scale = scale;
				Color = color;
				ReplaceMaterial = null;
			}
		}

		/////////////////////////////////////////

		public struct AdditionalItemPreviousTransform
		{
			public double transformPositionByTime1_Time;
			public Vector3 transformPositionByTime1_Position;
			public double transformPositionByTime2_Time;
			public Vector3 transformPositionByTime2_Position;
		}

		///////////////////////////////////////////////

		public MeshInSpace()
		{
			//_replaceMaterialSelectively = new ReferenceList<Material>( this, () => ReplaceMaterialSelectivelyChanged?.Invoke( this ) );

			////_additionalMeshes = new ReferenceList<Mesh>( this, () => AdditionalMeshesChanged?.Invoke( this ) );

			////item.Creator = this;
			////item.ReceiveDecals = true;
			////item.MotionBlurFactor = 1;
			////item.Color = ColorValue.One;
			////item.ColorForInstancingData = RenderingPipeline.ColorOneForInstancingData;
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( PhysicalBodyLinearVelocity ):
				case nameof( PhysicalBodyAngularVelocity ):
					if( !Collision )
						skip = true;
					if( PhysicalBody != null && PhysicalBody.MotionType == PhysicsMotionType.Static )
						skip = true;
					break;

				//!!!!impl?
				//case nameof( ReplaceMaterialSelectively ):
				case nameof( SpecialEffects ):
					//case nameof( AdditionalMeshes ):
					//case nameof( AdditionalItems ):
					if( Static )
						skip = true;
					break;

				case nameof( StaticShadows ):
					if( Static )
						skip = true;
					break;

					//case nameof( AdditionalMeshes ):
					//	//!!!!impl?
					//	if( Static )
					//		skip = true;
					//	var additionalItems = AdditionalItems;
					//	if( !additionalItems.ReferenceSpecified && ( additionalItems.Value == null || additionalItems.Value.Count == 0 ) )
					//		skip = true;
					//	break;
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal static bool IsTwoSided( Mesh mesh, Material replaceMaterial )
		{
			if( replaceMaterial != null && replaceMaterial.TwoSided )
				return true;
			if( mesh.Result != null )
			{
				var operations = mesh.Result.MeshData.RenderOperations;
				for( int n = 0; n < operations.Count; n++ )
				{
					var oper = operations[ n ];
					if( oper.Material != null && oper.Material.TwoSided )
						return true;
				}
			}
			return false;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void GetTransformWithOptimizedCases( ref Transform cached )
		{
			if( cached == null )
				cached = TransformVisualOverride ?? TransformV;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( needUpdateStaticModeAndStaticShadows )
			{
				UpdateStaticMode();
				if( StaticShadows )
					ResetLightStaticShadowsCache();
			}

			//touch Transform when reference is specified to update space bounds
			if( !staticModeEnabled && TransformHasReference && VisibleInHierarchy )
			{
				Transform.Touch();
				//var t = Transform;
			}
		}

		internal void Scene_GetRenderSceneData2ForGroupOfObjects()// Scene scene, ViewportRenderingContext context )
		{
			if( needUpdateStaticModeAndStaticShadows )
				UpdateStaticMode();
		}

		//private void Scene_GetRenderSceneData2ForGroupOfObjects( Scene scene, ViewportRenderingContext context )
		//{
		//	if( needUpdateStaticModeAndStaticShadows )
		//		UpdateStaticMode();
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( staticModeEnabled )
			{
#if !DEPLOY
				var context2 = context.ObjectInSpaceRenderingContext;
				if( !context2.selectedObjects.Contains( this ) && !context2.canSelectObjects.Contains( this ) )
				{
					context2.disableShowingLabelForThisObject = staticModeGroupOfObjectSubGroups != null;
					return;
				}
#else
				var context2 = context.ObjectInSpaceRenderingContext;
				context2.disableShowingLabelForThisObject = true;
				return;
#endif
			}

			//!!!!если mode == GI то лоды не нужно выбирать. другие свойства видимо тоже

			//!!!!везде в объектах OnGetRenderSceneData
			//!!!!тут суть в том что оно только раз для объекта вызывается

			Transform tr = null;

			var time = context.Owner.LastUpdateTime;
			if( time != transformPositionByTime1_Time )
			{
				transformPositionByTime2_Time = transformPositionByTime1_Time;
				transformPositionByTime2_Position = transformPositionByTime1_Position;
				transformPositionByTime1_Time = time;
				GetTransformWithOptimizedCases( ref tr );
				transformPositionByTime1_Position = tr.Position;
				//context.ConvertToRelative( tr.Position, out transformPositionByTime1_PositionRelative );
			}

			//if( EnabledInHierarchy )
			//{
			//	foreach( var iChild in GetComponents<IMeshInSpaceChild>( false, false, true ) )
			//		iChild.ParentMeshInSpace_GetRenderSceneData();

			//	//ModifiableMesh_CreateDestroy();
			//}

			if( mode == GetRenderSceneDataMode.InsideFrustum || ( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && CastShadows ) || mode == GetRenderSceneDataMode.GlobalIllumination )
			{
				//mesh
				{
					//!!!!тут?
					if( Mesh.Value?.Result != usedMeshDataWhenInitialized )
						RecreateData();

					var mesh = MeshOutput;
					if( mesh != null && mesh.Result != null )
					{
						var meshDataLOD0 = mesh.Result.MeshData;

						var context2 = context.ObjectInSpaceRenderingContext;
						context2.disableShowingLabelForThisObject = true;

						if( mode == GetRenderSceneDataMode.InsideFrustum || ( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && meshDataLOD0.CastShadows ) || mode == GetRenderSceneDataMode.GlobalIllumination )
						{
							var cameraSettings = context.Owner.CameraSettings;
							//var tr = Transform.Value;

							var cameraDistanceMinSquared = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, SpaceBounds );

							var boundingSize = (float)SpaceBounds.boundingSphere.Radius * 2;
							var visibilityDistance = (float)( context.GetVisibilityDistanceByObjectSize( boundingSize ) * VisibilityDistanceFactor * meshDataLOD0.VisibilityDistanceFactor );

							if( cameraDistanceMinSquared < visibilityDistance * visibilityDistance /*|| mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum*/ )
							{
								var cameraDistanceMin = MathEx.Sqrt( cameraDistanceMinSquared );
								var cameraDistanceMaxSquared = SceneLODUtility.GetCameraDistanceMax( cameraSettings, SpaceBounds );
								cameraDistanceMaxSquared *= cameraDistanceMaxSquared;

#if !DEPLOY
								var allowOutlineSelect = EngineApp.IsEditor && Math.Max( context2.selectedObjects.Count, context2.canSelectObjects.Count ) <= 1000 && context.renderingPipeline.UseRenderTargets && ProjectSettings.Get.SceneEditor.SceneEditorSelectOutlineEffectEnabled;
#endif

								var item = new RenderingPipeline.RenderSceneData.MeshItem();
								item.Creator = this;
								item.BoundingSphere = SpaceBounds.boundingSphere;
								//item.BoundingBoxCenter = item.BoundingSphere.Center;
								//SpaceBounds.CalculatedBoundingBox.GetCenter( out item.BoundingBoxCenter );
								//item.MeshData = mesh.Result.MeshData;
								//item.CastShadows = CastShadows && item.MeshData.CastShadows;
								item.ReceiveDecals = ReceiveDecals;
								item.MotionBlurFactor = (float)MotionBlurFactor;
								item.ReplaceMaterial = ReplaceMaterial;
								//if( ReplaceMaterialSelectively.Count != 0 )
								//{
								//	//!!!!может fixed массив юзать если влазит. или хранить локальный массив
								//	//!!!!GC
								//	item.ReplaceMaterialSelectively = new Material[ ReplaceMaterialSelectively.Count ];
								//	for( int n = 0; n < ReplaceMaterialSelectively.Count; n++ )
								//		item.ReplaceMaterialSelectively[ n ] = ReplaceMaterialSelectively[ n ].Value;
								//}
								item.Color = Color;
								RenderingPipeline.GetColorForInstancingData( ref item.Color, out item.ColorForInstancingData1, out item.ColorForInstancingData2 );
								item.TransparentRenderingAddOffsetWhenSortByDistance = transparentRenderingAddOffsetWhenSortByDistance;
								item.VisibilityDistance = visibilityDistance;
								item.CutVolumes = CutVolumes;
								item.StaticShadows = StaticShadows;

								var specialEffects = SpecialEffects.Value;
								if( specialEffects != null && specialEffects.Count != 0 )
									item.SpecialEffects = specialEffects;

#if !DEPLOY
								//display outline effect of editor selection
								if( allowOutlineSelect && mode == GetRenderSceneDataMode.InsideFrustum && context2.selectedObjects.Contains( this ) && EditorAllowRenderSelection )
								{
									var color = ProjectSettings.Get.Colors.SelectedColor.Value;
									//color.Alpha *= .5f;

									var effect = new ObjectSpecialRenderingEffect_Outline();
									effect.Group = int.MaxValue;
									effect.Color = color;

									if( item.SpecialEffects != null )
										item.SpecialEffects = new List<ObjectSpecialRenderingEffect>( item.SpecialEffects );
									else
										item.SpecialEffects = new List<ObjectSpecialRenderingEffect>();
									item.SpecialEffects.Add( effect );
								}
#endif

								//PositionPreviousFrame
								var previousTime = time - context.Owner.LastUpdateTimeStep;
								if( !GetTransformPositionByTime( previousTime, out var previousPosition ) )
								{
									GetTransformWithOptimizedCases( ref tr );
									previousPosition = tr.Position;
									//context.ConvertToRelative( tr.Position, out previousPosition );
									////previousPosition = tr.Position;
								}

								//MeshDataLastVoxelLOD
								var meshDataLods = meshDataLOD0.LODs;
								if( meshDataLods != null )
								{
									var lastLOD = meshDataLods[ meshDataLods.Length - 1 ];
									if( lastLOD.VoxelGridSize != 0 )
										item.MeshDataLastVoxelLOD = lastLOD.Mesh?.Result?.MeshData;
								}

								int item0BillboardMode = 0;

								SceneLODUtility.GetDemandedLODs( context, meshDataLOD0, cameraDistanceMinSquared, cameraDistanceMaxSquared, boundingSize, out var lodState );
								for( int nLodItem = 0; nLodItem < lodState.Count; nLodItem++ )
								{
									lodState.GetItem( nLodItem, out var lodLevel, out var lodRange );

									item.MeshData = meshDataLOD0;
									item.MeshDataLOD0 = meshDataLOD0;

									//MeshDataShadows, MeshDataShadowsForceBestLOD
									var lodScaleShadows = context.LODScaleShadowsSquared * meshDataLOD0.LODScaleShadows;
									if( lodScaleShadows <= lodLevel )
									{
										//select last LOD for shadows
										item.MeshDataShadows = item.MeshDataLastVoxelLOD;
									}
									else if( lodScaleShadows >= 100 )
									{
										//select the best LOD for shadows
										item.MeshDataShadows = meshDataLOD0;
										item.MeshDataShadowsForceBestLOD = true;
									}

									if( lodLevel > 0 )
									{
										ref var lod = ref meshDataLods[ lodLevel - 1 ];
										var lodMeshData = lod.Mesh?.Result?.MeshData;
										if( lodMeshData != null )
											item.MeshData = lodMeshData;
									}

									GetTransformWithOptimizedCases( ref tr );
									item.PreviousFramePositionChange = ( tr.Position - previousPosition ).ToVector3F();
									//item.PositionPreviousFrameRelative = previousPosition;

									////context.ConvertToRelative( ref previousPosition, out item.PositionPreviousFrameRelative );
									//////item.PositionPreviousFrame = previousPosition.ToVector3F();

									item.CastShadows = CastShadows && item.MeshData.CastShadows && cameraDistanceMinSquared < context.GetShadowVisibilityDistanceSquared( visibilityDistance );

									item.LODValue = SceneLODUtility.GetLodValue( context, lodRange, cameraDistanceMin );
									//item.LODRange = lodRange;

									//calculate MeshInstanceOne
									if( nLodItem == 0 )
										item0BillboardMode = item.MeshData.BillboardMode;
									if( nLodItem == 0 || item0BillboardMode != item.MeshData.BillboardMode )
									{
										GetTransformWithOptimizedCases( ref tr );

										if( item.MeshData.BillboardMode != 0 )
										{
											var position = tr.Position;
											var rotation = tr.Rotation.ToQuaternionF();
											var scale = tr.Scale;
											var scaleH = (float)Math.Max( scale.X, scale.Y );
											var scaleV = (float)scale.Z;

											Vector3F offset;
											if( item.MeshData.BillboardPositionOffset != Vector3F.Zero )
												offset = rotation * ( item.MeshData.BillboardPositionOffset * Math.Max( scaleH, scaleV ) );
											else
												offset = Vector3F.Zero;

											ref var result = ref item.TransformRelative;
											result.Item0.X = scaleH;
											result.Item0.Y = 0;
											result.Item0.Z = rotation.X;
											result.Item0.W = 0;
											result.Item1.X = rotation.Y;
											result.Item1.Y = scaleH;
											result.Item1.Z = rotation.Z;
											result.Item1.W = 0;
											result.Item2.X = rotation.W;
											result.Item2.Y = 0;
											result.Item2.Z = (float)scale.Z;
											result.Item2.W = 0;
											result.Item3.X = (float)( position.X - context.OwnerCameraSettingsPosition.X + offset.X );
											result.Item3.Y = (float)( position.Y - context.OwnerCameraSettingsPosition.Y + offset.Y );
											result.Item3.Z = (float)( position.Z - context.OwnerCameraSettingsPosition.Z + offset.Z );
											result.Item3.W = 1;

											//item.PositionPreviousFrameRelative += offset;

											//var scaleX = Math.Max( tr.Scale.X, tr.Scale.Y );
											//var scale = new Vector3( scaleX, scaleX, tr.Scale.Z );
											//Matrix3.FromScale( ref scale, out var mat3 );
											//var matrix = new Matrix4( mat3, tr.Position );
											//double
											//matrix.ToMatrix4F( out item.MeshInstanceOne );
										}
										else
										{
											ref var matrix = ref tr.ToMatrix4();
											context.ConvertToRelative( ref matrix, out item.TransformRelative );
											//matrix.ToMatrix4F( out item.Transform );
										}
									}

									//layers
									{
										var a1 = item.MeshData.PaintLayers;
										var a2 = GetPaintLayers();
										if( a1 != null && a2 != null )
										{
											//!!!!GC
											var z = new RenderingPipeline.RenderSceneData.LayerItem[ a1.Length + a2.Length ];
											a1.CopyTo( z, 0 );
											a2.CopyTo( z, a1.Length );
											item.Layers = z;
										}
										else
											item.Layers = a1 ?? a2;
									}

									//tessellation
									if( /*cameraDistanceMin < context.TessellationDistance &&*/mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum || mode == GetRenderSceneDataMode.InsideFrustum )
									{
										var enable = false;

										if( item.MeshData == meshDataLOD0 )
										{
											if( item.ReplaceMaterial != null )
											{
												if( item.ReplaceMaterial.Result != null )
													enable = item.ReplaceMaterial.Result.tessellationQuality != 0;
											}
											else
												enable = meshDataLOD0.ContainsTessellation;

											//if( item.ReplaceMaterialSelectively != null )
											//{
											//	for( int n = 0; n < item.ReplaceMaterialSelectively.Length; n++ )
											//	{
											//		var result = item.ReplaceMaterialSelectively[ n ].Result;
											//		if( result != null && result.tessellationQuality != 0 )
											//			enable = true;
											//	}
											//}
										}

										//set true only for first lod, set false to second (it can be true because item memory is shared)
										item.Tessellation = enable;
									}

									//add to render
									//!!!!double drawing static mode when selected. нужно динамическое отключение в группе объектов
									if( !staticModeEnabled || context2.selectedObjects.Contains( this ) )
									{
										var skip = false;
										OnGetRenderSceneDataAddToFrameData( context, mode, ref item, ref skip );
										//set AnimationData from event
										GetRenderSceneDataAddToFrameData?.Invoke( this, context, mode, ref item, ref skip );

										//add the item to render
										if( !skip )
											context.FrameData.RenderSceneData.Meshes.Add( ref item );
									}
								}

#if !DEPLOY
								//display editor selection
								if( EngineApp.IsEditor && mode == GetRenderSceneDataMode.InsideFrustum && EditorAllowRenderSelection )
								{
									if( ( !allowOutlineSelect && context2.selectedObjects.Contains( this ) ) || context2.canSelectObjects.Contains( this ) )
									{
										ColorValue color;
										if( context2.selectedObjects.Contains( this ) )
											color = ProjectSettings.Get.Colors.SelectedColor;
										else
											color = ProjectSettings.Get.Colors.CanSelectColor;

										var renderer = context.Owner.Simple3DRenderer;
										if( renderer != null )
										{
											color.Alpha *= .5f;
											renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

											if( Math.Max( context2.selectedObjects.Count, context2.canSelectObjects.Count ) > 1000 )
												renderer.AddBounds( SpaceBounds.boundingBox, true );
											else
											{
												bool twoSided = IsTwoSided( mesh, ReplaceMaterial ) || ParentScene.Mode.Value == Scene.ModeEnum._2D;
												GetTransformWithOptimizedCases( ref tr );
												renderer.AddMesh( mesh.Result, ref tr.ToMatrix4(), false, !twoSided );
												//renderer.AddMesh( mesh.Result, item.Transform, false, !twoSided );
											}
										}
									}
								}
#endif

							}
						}
					}
				}

				//additional items
				if( additionalItems != null && additionalItems.Length != 0 && !staticModeEnabled )
				{
					var context2 = context.ObjectInSpaceRenderingContext;

					//!!!!maybe no sense to save time for each item
					//!!!!good? maybe can do smooth update
					if( additionalItemsPreviousTransform == null || additionalItems.Length != additionalItemsPreviousTransform.Length )
						additionalItemsPreviousTransform = new AdditionalItemPreviousTransform[ additionalItems.Length ];


					//!!!!parallel?

					for( int nItem = 0; nItem < additionalItems.Length; nItem++ )
					{
						ref var additionalItem = ref additionalItems[ nItem ];
						var mesh = additionalItem.Mesh;

						if( mesh?.Result == null )
							continue;

						var meshDataLOD0 = mesh.Result.MeshData;

						if( mode == GetRenderSceneDataMode.InsideFrustum || ( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && meshDataLOD0.CastShadows ) || mode == GetRenderSceneDataMode.GlobalIllumination )
						{
							var cameraSettings = context.Owner.CameraSettings;
							//var tr = Transform.Value;

							GetTransformWithOptimizedCases( ref tr );
							Vector3 pos = tr.Position;
							Quaternion rot = tr.Rotation;
							Vector3 scl = tr.Scale;
							{
								pos += rot * ( additionalItem.Position * scl );
								rot *= additionalItem.Rotation;
								scl *= additionalItem.Scale;
							}
							//var tr2 = Transform.Value;
							//tr2 = tr2.ApplyOffset( additionalItem.Position, additionalItem.Rotation, additionalItem.Scale );


							ref var previousTransform = ref additionalItemsPreviousTransform[ nItem ];

							if( time != previousTransform.transformPositionByTime1_Time )
							{
								previousTransform.transformPositionByTime2_Time = previousTransform.transformPositionByTime1_Time;
								previousTransform.transformPositionByTime2_Position = previousTransform.transformPositionByTime1_Position;
								previousTransform.transformPositionByTime1_Time = time;
								previousTransform.transformPositionByTime1_Position = pos;
								//context.ConvertToRelative( ref pos, out previousTransform.transformPositionByTime1_PositionRelative );
								////previousTransform.transformPositionByTime1_Position = pos;
							}

							//additionalItemMesh.Position

							var cameraDistanceMinSquared = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, SpaceBounds );

							var boundingSize = (float)SpaceBounds.boundingSphere.Radius * 2;
							var visibilityDistance = (float)( context.GetVisibilityDistanceByObjectSize( boundingSize ) * VisibilityDistanceFactor * meshDataLOD0.VisibilityDistanceFactor );

							if( cameraDistanceMinSquared < visibilityDistance * visibilityDistance /*|| mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum*/ )
							{
								var cameraDistanceMin = MathEx.Sqrt( cameraDistanceMinSquared );
								var cameraDistanceMaxSquared = SceneLODUtility.GetCameraDistanceMax( cameraSettings, SpaceBounds );
								cameraDistanceMaxSquared *= cameraDistanceMaxSquared;

#if !DEPLOY
								var allowOutlineSelect = EngineApp.IsEditor && Math.Max( context2.selectedObjects.Count, context2.canSelectObjects.Count ) <= 1000 && context.renderingPipeline.UseRenderTargets && ProjectSettings.Get.SceneEditor.SceneEditorSelectOutlineEffectEnabled;
#endif

								var item = new RenderingPipeline.RenderSceneData.MeshItem();
								item.Creator = this;
								item.BoundingSphere = SpaceBounds.boundingSphere;
								//item.BoundingBoxCenter = item.BoundingSphere.Center;
								//SpaceBounds.CalculatedBoundingBox.GetCenter( out item.BoundingBoxCenter );
								//item.MeshData = mesh.Result.MeshData;
								//item.CastShadows = CastShadows && item.MeshData.CastShadows;
								item.ReceiveDecals = ReceiveDecals;
								item.MotionBlurFactor = (float)MotionBlurFactor;
								item.StaticShadows = StaticShadows;

								if( additionalItem.ReplaceMaterial != null )
									item.ReplaceMaterial = additionalItem.ReplaceMaterial;
								else
								{
									item.ReplaceMaterial = ReplaceMaterial;
									//if( ReplaceMaterialSelectively.Count != 0 )
									//{
									//	//!!!!может fixed массив юзать если влазит
									//	//!!!!GC
									//	item.ReplaceMaterialSelectively = new Material[ ReplaceMaterialSelectively.Count ];
									//	for( int n = 0; n < ReplaceMaterialSelectively.Count; n++ )
									//		item.ReplaceMaterialSelectively[ n ] = ReplaceMaterialSelectively[ n ].Value;
									//}
								}

								item.Color = Color * additionalItem.Color;
								RenderingPipeline.GetColorForInstancingData( ref item.Color, out item.ColorForInstancingData1, out item.ColorForInstancingData2 );
								item.TransparentRenderingAddOffsetWhenSortByDistance = transparentRenderingAddOffsetWhenSortByDistance;
								item.VisibilityDistance = visibilityDistance;
								item.CutVolumes = CutVolumes;

								var specialEffects = SpecialEffects.Value;
								if( specialEffects != null && specialEffects.Count != 0 )
									item.SpecialEffects = specialEffects;

#if !DEPLOY
								//display outline effect of editor selection
								if( allowOutlineSelect && mode == GetRenderSceneDataMode.InsideFrustum && context2.selectedObjects.Contains( this ) )
								{
									var color = ProjectSettings.Get.Colors.SelectedColor.Value;
									//color.Alpha *= .5f;

									var effect = new ObjectSpecialRenderingEffect_Outline();
									effect.Group = int.MaxValue;
									effect.Color = color;

									if( item.SpecialEffects != null )
										item.SpecialEffects = new List<ObjectSpecialRenderingEffect>( item.SpecialEffects );
									else
										item.SpecialEffects = new List<ObjectSpecialRenderingEffect>();
									item.SpecialEffects.Add( effect );
								}
#endif

								//PositionPreviousFrame
								var previousTime = time - context.Owner.LastUpdateTimeStep;
								if( !AdditionalItemsGetTransformPositionByTime( ref previousTransform, previousTime, out var previousPosition ) )
								{
									previousPosition = pos;
									//context.ConvertToRelative( ref pos, out previousPosition );
									////previousPosition = pos;
								}

								//MeshDataLastVoxelLOD
								var meshDataLods = meshDataLOD0.LODs;
								if( meshDataLods != null )
								{
									var lastLOD = meshDataLods[ meshDataLods.Length - 1 ];
									if( lastLOD.VoxelGridSize != 0 )
										item.MeshDataLastVoxelLOD = lastLOD.Mesh?.Result?.MeshData;
								}

								int item0BillboardMode = 0;

								SceneLODUtility.GetDemandedLODs( context, meshDataLOD0, cameraDistanceMinSquared, cameraDistanceMaxSquared, boundingSize, out var lodState );
								for( int nLodItem = 0; nLodItem < lodState.Count; nLodItem++ )
								{
									lodState.GetItem( nLodItem, out var lodLevel, out var lodRange );

									item.MeshData = meshDataLOD0;
									item.MeshDataLOD0 = meshDataLOD0;

									//MeshDataShadows, MeshDataShadowsForceBestLOD
									var lodScaleShadows = context.LODScaleShadowsSquared * meshDataLOD0.LODScaleShadows;
									if( lodScaleShadows <= lodLevel )
									{
										//select last LOD for shadows
										item.MeshDataShadows = item.MeshDataLastVoxelLOD;
									}
									else if( lodScaleShadows >= 100 )
									{
										//select the best LOD for shadows
										item.MeshDataShadows = meshDataLOD0;
										item.MeshDataShadowsForceBestLOD = true;
									}

									if( lodLevel > 0 )
									{
										ref var lod = ref meshDataLods[ lodLevel - 1 ];
										var lodMeshData = lod.Mesh?.Result?.MeshData;
										if( lodMeshData != null )
											item.MeshData = lodMeshData;
									}

									item.PreviousFramePositionChange = ( pos - previousPosition ).ToVector3F();
									//item.PositionPreviousFrameRelative = previousPosition;
									////item.PositionPreviousFrame = previousPosition.ToVector3F();

									item.CastShadows = CastShadows && item.MeshData.CastShadows && cameraDistanceMinSquared < context.GetShadowVisibilityDistanceSquared( visibilityDistance );

									item.LODValue = SceneLODUtility.GetLodValue( context, lodRange, cameraDistanceMin );
									//item.LODRange = lodRange;

									//calculate MeshInstanceOne
									if( nLodItem == 0 )
										item0BillboardMode = item.MeshData.BillboardMode;
									if( nLodItem == 0 || item0BillboardMode != item.MeshData.BillboardMode )
									{
										if( item.MeshData.BillboardMode != 0 )
										{
											var position = pos;//tr2.Position;
											var rotation = rot.ToQuaternionF();//tr2.Rotation.ToQuaternionF();
											var scale = scl;//tr2.Scale;
															//var position = tr2.Position;
															//var rotation = tr2.Rotation.ToQuaternionF();
															//var scale = tr2.Scale;
											var scaleH = (float)Math.Max( scale.X, scale.Y );
											var scaleV = (float)scale.Z;

											Vector3F offset;
											if( item.MeshData.BillboardPositionOffset != Vector3F.Zero )
												offset = rotation * ( item.MeshData.BillboardPositionOffset * Math.Max( scaleH, scaleV ) );
											else
												offset = Vector3F.Zero;

											ref var result = ref item.TransformRelative;
											result.Item0.X = scaleH;
											result.Item0.Y = 0;
											result.Item0.Z = rotation.X;
											result.Item0.W = 0;
											result.Item1.X = rotation.Y;
											result.Item1.Y = scaleH;
											result.Item1.Z = rotation.Z;
											result.Item1.W = 0;
											result.Item2.X = rotation.W;
											result.Item2.Y = 0;
											result.Item2.Z = (float)scale.Z;
											result.Item2.W = 0;
											result.Item3.X = (float)( position.X - context.OwnerCameraSettingsPosition.X + offset.X );
											result.Item3.Y = (float)( position.Y - context.OwnerCameraSettingsPosition.Y + offset.Y );
											result.Item3.Z = (float)( position.Z - context.OwnerCameraSettingsPosition.Z + offset.Z );
											result.Item3.W = 1;

											//item.PositionPreviousFrameRelative += offset;

											//var scaleX = Math.Max( tr.Scale.X, tr.Scale.Y );
											//var scale = new Vector3( scaleX, scaleX, tr.Scale.Z );
											//Matrix3.FromScale( ref scale, out var mat3 );
											//var matrix = new Matrix4( mat3, tr.Position );
											//double
											//matrix.ToMatrix4F( out item.MeshInstanceOne );
										}
										else
										{
											rot.ToMatrix3( out var rot2 );
											Matrix3.FromScale( ref scl, out var scl2 );
											Matrix3.Multiply( ref rot2, ref scl2, out var mat3 );
											var matrix = new Matrix4( ref mat3, ref pos );

											context.ConvertToRelative( ref matrix, out item.TransformRelative );
											//matrix.ToMatrix4F( out item.Transform );
										}
									}

									//layers
									{
										var a1 = item.MeshData.PaintLayers;
										var a2 = GetPaintLayers();
										if( a1 != null && a2 != null )
										{
											//!!!!GC
											var z = new RenderingPipeline.RenderSceneData.LayerItem[ a1.Length + a2.Length ];
											a1.CopyTo( z, 0 );
											a2.CopyTo( z, a1.Length );
											item.Layers = z;
										}
										else
											item.Layers = a1 ?? a2;
									}

									//tessellation
									if( /*cameraDistanceMin < context.TessellationDistance &&*/ mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum || mode == GetRenderSceneDataMode.InsideFrustum )
									{
										var enable = false;

										if( item.MeshData == meshDataLOD0 )
										{
											if( item.ReplaceMaterial != null )
											{
												if( item.ReplaceMaterial.Result != null )
													enable = item.ReplaceMaterial.Result.tessellationQuality != 0;
											}
											else
												enable = meshDataLOD0.ContainsTessellation;

											//if( item.ReplaceMaterialSelectively != null )
											//{
											//	for( int n = 0; n < item.ReplaceMaterialSelectively.Length; n++ )
											//	{
											//		var result = item.ReplaceMaterialSelectively[ n ].Result;
											//		if( result != null && result.tessellationQuality != 0 )
											//			enable = true;
											//	}
											//}
										}

										//set true only for first lod, set false to second (it can be true because item memory is shared)
										item.Tessellation = enable;
									}

									//add to render
									//!!!!double drawing static mode when selected. нужно динамическое отключение в группе объектов
									if( !staticModeEnabled || context2.selectedObjects.Contains( this ) )
									{
										var skip = false;
										OnGetRenderSceneDataAddToFrameData( context, mode, ref item, ref skip );
										//set AnimationData from event
										GetRenderSceneDataAddToFrameData?.Invoke( this, context, mode, ref item, ref skip );

										//add the item to render
										if( !skip )
											context.FrameData.RenderSceneData.Meshes.Add( ref item );
									}
								}

#if !DEPLOY
								//display editor selection
								if( EngineApp.IsEditor && mode == GetRenderSceneDataMode.InsideFrustum )
								{
									if( ( !allowOutlineSelect && context2.selectedObjects.Contains( this ) ) || context2.canSelectObjects.Contains( this ) )
									{
										ColorValue color;
										if( context2.selectedObjects.Contains( this ) )
											color = ProjectSettings.Get.Colors.SelectedColor;
										else
											color = ProjectSettings.Get.Colors.CanSelectColor;

										var renderer = context.Owner.Simple3DRenderer;
										if( renderer != null )
										{
											color.Alpha *= .5f;
											renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

											if( Math.Max( context2.selectedObjects.Count, context2.canSelectObjects.Count ) > 1000 )
												renderer.AddBounds( SpaceBounds.boundingBox, true );
											else
											{
												bool twoSided = IsTwoSided( mesh, ReplaceMaterial ) || ParentScene.Mode.Value == Scene.ModeEnum._2D;

												rot.ToMatrix3( out var rot2 );
												Matrix3.FromScale( ref scl, out var scl2 );
												Matrix3.Multiply( ref rot2, ref scl2, out var mat3 );
												var matrix = new Matrix4( ref mat3, ref pos );

												renderer.AddMesh( mesh.Result, ref matrix, false, !twoSided );

												//renderer.AddMesh( mesh.Result, ref tr2.ToMatrix4(), false, !twoSided );
												////renderer.AddMesh( mesh.Result, item.Transform, false, !twoSided );
											}
										}
									}
								}
#endif

							}
						}

						//}
					}
				}
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			var result = MeshOutput?.Result;
			if( result != null )
			{
				if( result.MeshData.BillboardMode != 0 )
				{
					var tr = Transform.Value;
					ref var meshSphere = ref result.MeshData.SpaceBounds.boundingSphere;
					newBounds = new SpaceBounds( new Sphere( tr.Position, meshSphere.Radius * tr.Scale.MaxComponent() ) );
				}
				else
				{
					var meshBoundLocal = result.SpaceBounds;
					var meshBoundsTransformed = SpaceBounds.Multiply( Transform, meshBoundLocal );
					newBounds = SpaceBounds.Merge( newBounds, meshBoundsTransformed );
				}

				//bounds prediction to skip small updates in future steps
				if( PhysicalBody != null && PhysicalBody.Active )
				{
					ref var realBounds = ref newBounds.boundingBox;

					if( !SpaceBoundsOctreeOverride.HasValue || !SpaceBoundsOctreeOverride.Value.Contains( ref realBounds ) )
					{
						//calculated extended bounds. predict for 2-3 seconds

						var trPosition = TransformV.Position;

						var bTotal = realBounds;
						var b2 = new Bounds( trPosition );
						b2.Add( trPosition + PhysicalBody.LinearVelocity * ( 2.0f + staticRandom.NextFloat() ) );
						b2.Expand( newBounds.boundingSphere.Radius * 1.1 );
						bTotal.Add( ref b2 );

						SpaceBoundsOctreeOverride = bTotal;
					}
				}
				else
					SpaceBoundsOctreeOverride = null;
			}
		}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			//!!!!additional items?

			Mesh mesh = MeshOutput;
			if( mesh != null && mesh.Result != null )
			{
				context.thisObjectWasChecked = true;

				if( mesh.Result.MeshData.BillboardMode != 0 )
				{
					//billboard mode

					if( SpaceBounds.BoundingSphere.Intersects( context.ray, out var scale1, out var scale2 ) )
					{
						if( mesh.Result.ExtractedIndices.Length != 0 )
						{
							Quaternion meshFaceRotation = Quaternion.Identity;
							switch( mesh.Result.MeshData.BillboardMode )
							{
							case 1: meshFaceRotation = Quaternion.FromDirectionZAxisUp( Vector3.Zero - new Vector3( 0, -1, 0 ) ); break;
							case 2: meshFaceRotation = Quaternion.FromDirectionZAxisUp( Vector3.Zero - new Vector3( 0, 1, 0 ) ); break;
							case 3: meshFaceRotation = Quaternion.FromDirectionZAxisUp( Vector3.Zero - new Vector3( 1, 0, 0 ) ); break;
							case 4: meshFaceRotation = Quaternion.FromDirectionZAxisUp( Vector3.Zero - new Vector3( -1, 0, 0 ) ); break;
							}

							var rotation = context.viewport.CameraSettings.Rotation * meshFaceRotation.GetInverse();
							var tr = TransformV;
							var scaleH = (float)Math.Max( tr.Scale.X, tr.Scale.Y );
							var tranform = new Matrix4( rotation.ToMatrix3() * Matrix3.FromScale( new Vector3( scaleH, scaleH, tr.Scale.Z ) ), tr.Position );

							var vertices = mesh.Result.ExtractedVerticesPositions;
							var indices = mesh.Result.ExtractedIndices;
							for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
							{
								var vertex0 = vertices[ indices[ nTriangle * 3 + 0 ] ];
								var vertex1 = vertices[ indices[ nTriangle * 3 + 1 ] ];
								var vertex2 = vertices[ indices[ nTriangle * 3 + 2 ] ];

								var v0 = tranform * vertex0;
								var v1 = tranform * vertex1;
								var v2 = tranform * vertex2;

								if( MathAlgorithms.IntersectTriangleRay( ref v0, ref v1, ref v2, ref context.ray, out var rayScale ) )
								{
									context.thisObjectResultRayScale = rayScale;
									break;
								}
							}
						}
						else
						{
							double scale = Math.Min( scale1, scale2 );
							context.thisObjectResultRayScale = scale;
						}
					}
				}
				else
				{
					//usual mesh mode

					ref var bounds = ref SpaceBounds.boundingBox;
					if( bounds.Intersects( context.ray, out double scale ) )
					{
						if( mesh.Result.ExtractedIndices != null )
						{
							//check by geometry

							//!!!!can be cached?
							Transform.Value.ToMatrix4().GetInverse( out var transInv );
							Ray localRay = transInv * context.ray;

							var rayCastResult = mesh.Result.RayCast( localRay, NeoAxis.Mesh.CompiledData.RayCastModes.Auto, true );
							if( rayCastResult != null )
								context.thisObjectResultRayScale = rayCastResult.Scale;

							//int minTriangles = 30;

							//Mesh.CompiledData.RayCastMode mode;
							//if( mesh.Result.ExtractedIndices.Length > minTriangles * 3 )
							//	mode = NeoAxis.Mesh.CompiledData.RayCastMode.OctreeOptimizedCached;
							//else
							//	mode = NeoAxis.Mesh.CompiledData.RayCastMode.BruteforceNoCache;

							//bool twoSided = true;
							//bool twoSided = IsTwoSided( mesh, ReplaceMaterial );

							//if( mesh.Result.RayCast( localRay, mode, twoSided, out double scale2, out _, out _ ) )
							//	context.thisObjectResultRayScale = scale2;
						}
						else
							context.thisObjectResultRayScale = scale;
					}
				}
			}
		}

		//protected override void OnRender( RenderingContext context )
		//{
		//	base.OnRender( context );

		//	//!!!!если это не редактор, то не всегда нужно проверять. там выше код для disableShowingLabelForThisObject

		//	var mesh = Mesh.Value;

		//	//!!!!тут?
		//	if( mesh?.Result != usedMeshDataWhenInitialized )
		//		RecreateData();
		//	//CheckForUpdateDataWhenMeshChanged();

		//	var m = ModifiableMesh != null ? ModifiableMesh : mesh;
		//	//Mesh m = MeshOutput;

		//	if( m != null && m.Result != null )
		//	{
		//		context.disableShowingLabelForThisObject = true;

		//		if( context.selectedObjects.Contains( this ) || context.canSelectObjects.Contains( this ) )
		//		{
		//			ColorValue color;
		//			if( context.selectedObjects.Contains( this ) )
		//				color = ProjectSettings.Get.SelectedColor;
		//			else
		//				color = ProjectSettings.Get.CanSelectColor;

		//			var viewport = context.viewport;
		//			if( viewport.Simple3DRenderer != null )
		//			{
		//				//!!!!use DynamicMeshManager
		//				//!!!!!!или может проще сам меш особо рисовать

		//				color.Alpha *= .5f;
		//				viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

		//				if( Math.Max( context.selectedObjects.Count, context.canSelectObjects.Count ) > 1000 )
		//					context.viewport.Simple3DRenderer.AddBounds( SpaceBounds.CalculatedBoundingBox, true );
		//				else
		//					context.viewport.Simple3DRenderer.AddMesh( m.Result, Transform.Value.ToMatrix4(), false, true );
		//			}
		//		}
		//	}
		//}

		public delegate void MeshOutputOverrideDelegate( MeshInSpace sender, ref Mesh result );
		public event MeshOutputOverrideDelegate MeshOutputOverride;

		[Browsable( false )]
		public virtual Mesh MeshOutput
		{
			get
			{
				var m = ModifiableMesh;
				if( m == null )
					m = Mesh;
				MeshOutputOverride?.Invoke( this, ref m );
				return m;
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				CreateData();
				//после загрузки создается через шейп, т.к. срабатывает RecreateBody()
				if( physicalBody == null )
					CreateBody();

				if( StaticShadows )
				{
					needUpdateStaticModeAndStaticShadows = true;
					//ResetLightStaticShadowsCache();
				}
			}
			else
			{
				if( StaticShadows )
					ResetLightStaticShadowsCache();

				DestroyData();
				DestroyBody();
			}

			var scene = ParentScene;
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
					scene.meshInSpaces.AddWithCheckAlreadyContained( this );
				else
					scene.meshInSpaces.Remove( this );

				//if( EnabledInHierarchyAndIsInstance )
				//	scene.GetRenderSceneData2ForGroupOfObjects += Scene_GetRenderSceneData2ForGroupOfObjects;
				//else
				//	scene.GetRenderSceneData2ForGroupOfObjects -= Scene_GetRenderSceneData2ForGroupOfObjects;
			}
		}

		//public void CheckForUpdateDataWhenMeshChanged()
		//{
		//	if( Mesh.Value?.Result != usedMeshDataWhenInitialized )
		//		RecreateData();
		//}

		//[Browsable( false )]
		//public int _InternalRenderSceneIndex
		//{
		//	get { return _internalRenderSceneIndex; }
		//	set { _internalRenderSceneIndex = value; }
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Browsable( false )]
		public Mesh ModifiableMesh
		{
			get { return modifiableMesh; }
		}

		[Browsable( false )]
		public Component ModifiableMeshCreatedByObject
		{
			get { return modifiableMeshCreatedByObject; }
		}

		[Flags]
		public enum ModifiableMeshCreationFlags
		{
			VertexBuffersCreateDuplicate = 1,
			VertexBuffersDynamic = 2,
			VertexBuffersComputeWrite = 4,
			IndexBufferCreateDuplicate = 8,
			IndexBufferDynamic = 16,
			IndexBufferComputeWrite = 32,
		}

		public void ModifiableMeshCreate( Component createbyObject, ModifiableMeshCreationFlags creationFlags )// bool vertexBuffersUpdateDynamic, bool indexBufferUpdateDynamic )
		{
			ModifiableMeshDestroy();

			Mesh mesh = Mesh;
			if( mesh != null )
			{
				//need set ShowInEditor = false before AddComponent
				modifiableMesh = ComponentUtility.CreateComponent<Mesh>( null, false, false );
				modifiableMesh.NetworkMode = NetworkModeEnum.False;
				modifiableMesh.DisplayInEditor = false;
				AddComponent( modifiableMesh, -1 );
				//modifiableMesh = CreateComponent<Mesh>( -1, false );

				modifiableMesh.Name = modifiableMeshName;
				modifiableMesh.SaveSupport = false;
				modifiableMesh.CloneSupport = false;
				modifiableMesh.AllowDisposeBuffers = false;

				var geometry = modifiableMesh.CreateComponent<MeshGeometry>();
				//geometry.Name = "Mesh Geometry";

				//!!!!потом может быть другой размер меша и т.д.

				modifiableMesh.MeshCompileEvent += delegate ( Mesh sender, Mesh.CompiledData compiledData )
				{
					foreach( var sourceOp in mesh.Result.MeshData.RenderOperations )
					{
						var op = new RenderingPipeline.RenderSceneData.MeshDataRenderOperation( geometry, sourceOp.MeshGeometryIndex );
						//op.Creator = geometry;//createdMeshToUpdate;
						//op.disposeBuffersByCreator = true;

						//if( creationFlags.HasFlag( ModifiableMeshCreationFlags.DuplicateOnlyGeometryChannels ) )
						//{
						//!!!!если несколько источников

						//var vertexStructure = new List<VertexElement>();
						//foreach( var element in sourceOp.VertexStructure )
						//{
						//	if( element.Semantic != VertexElementSemantic.Position &&
						//		element.Semantic != VertexElementSemantic.Normal &&
						//		element.Semantic != VertexElementSemantic.Tangent &&
						//		element.Semantic != VertexElementSemantic.Bitangent &&
						//		element.Semantic != VertexElementSemantic.BlendIndices &&
						//		element.Semantic != VertexElementSemantic.BlendWeights )
						//	{
						//		vertexStructure.Add( element );
						//	}
						//}

						////source vertex buffers
						//op.VertexBuffers = new List<GpuVertexBuffer>();
						//foreach( var sourceBuffer in sourceOp.VertexBuffers )
						//	op.VertexBuffers.Add( sourceBuffer );

						////create vertex buffer
						//{
						//	//!!!!какие не создавать.
						//	//!!!!binormal еще.
						//	var source = op.VertexBuffers.Count;
						//	vertexStructure.Add( new VertexElement( source, 0, VertexElementType.Float3, VertexElementSemantic.Position ) );
						//	vertexStructure.Add( new VertexElement( source, 12, VertexElementType.Float3, VertexElementSemantic.Normal ) );
						//	vertexStructure.Add( new VertexElement( source, 24, VertexElementType.Float4, VertexElementSemantic.Tangent ) );

						//	//!!!!какие не создавать.
						//	//!!!!binormal еще.
						//	var declaration = new VertexLayout().Begin()
						//		.Add( VertexAttributeUsage.Position, 3, VertexAttributeType.Float )
						//		.Add( VertexAttributeUsage.Normal, 3, VertexAttributeType.Float )
						//		.Add( VertexAttributeUsage.Tangent, 4, VertexAttributeType.Float )
						//		.End();

						//	var vertices = new byte[ declaration.Stride * sourceOp.VertexCount ];

						//	var buffer = GpuBufferManager.CreateVertexBuffer( vertices, declaration, creationFlags.HasFlag( ModifiableMeshCreationFlags.VertexBuffersUpdateDynamic ) );// vertexBuffersUpdateDynamic );
						//	op.VertexBuffers.Add( buffer );

						//	if( modifiableMesh_BuffersToDispose == null )
						//		modifiableMesh_BuffersToDispose = new List<ThreadSafeDisposable>();
						//	modifiableMesh_BuffersToDispose.Add( buffer );
						//}

						//op.VertexStructure = vertexStructure.ToArray();

						//op.VertexStartOffset = sourceOp.VertexStartOffset;
						//op.VertexCount = sourceOp.VertexCount;

						////index buffer
						//if( sourceOp.IndexBuffer != null )
						//	op.IndexBuffer = sourceOp.IndexBuffer;
						//op.IndexStartOffset = sourceOp.IndexStartOffset;
						//op.IndexCount = sourceOp.IndexCount;
						//}
						//else
						//{

						//vertex buffers

						op.VertexStructure = sourceOp.VertexStructure;
						op.VertexStructureContainsColor = sourceOp.VertexStructureContainsColor;
						op.UnwrappedUV = sourceOp.UnwrappedUV;

						var vertexBuffers = new List<GpuVertexBuffer>();//op.VertexBuffers = new List<GpuVertexBuffer>();
						foreach( var sourceBuffer in sourceOp.VertexBuffers )
						{
							if( ( creationFlags & ModifiableMeshCreationFlags.VertexBuffersCreateDuplicate ) != 0 )
							{
								GpuVertexBuffer buffer = null;
								if( ( creationFlags & ModifiableMeshCreationFlags.VertexBuffersDynamic ) != 0 )
								{
									buffer = GpuBufferManager.CreateVertexBuffer( (byte[])sourceBuffer.Vertices.Clone(), sourceBuffer.VertexDeclaration, GpuBufferFlags.Dynamic | GpuBufferFlags.ComputeRead );
								}
								else if( ( creationFlags & ModifiableMeshCreationFlags.VertexBuffersComputeWrite ) != 0 )
								{
									buffer = GpuBufferManager.CreateVertexBuffer( sourceBuffer.VertexCount, sourceBuffer.VertexDeclaration, GpuBufferFlags.ComputeWrite | GpuBufferFlags.ComputeRead );
								}

								//var buffer = GpuBufferManager.CreateVertexBuffer( (byte[])sourceBuffer.Vertices.Clone(), sourceBuffer.VertexDeclaration, creationFlags.HasFlag( ModifiableMeshCreationFlags.VertexBuffersUpdateDynamic ) );// vertexBuffersUpdateDynamic );

								vertexBuffers.Add( buffer );

								if( modifiableMeshBuffersToDispose == null )
									modifiableMeshBuffersToDispose = new List<ThreadSafeDisposable>();
								modifiableMeshBuffersToDispose.Add( buffer );
							}
							else
								vertexBuffers.Add( sourceBuffer );
						}
						op.VertexBuffers = vertexBuffers.ToArray();
						op.VertexStartOffset = sourceOp.VertexStartOffset;
						op.VertexCount = sourceOp.VertexCount;

						//index buffer
						if( sourceOp.IndexBuffer != null )
						{
							if( ( creationFlags & ModifiableMeshCreationFlags.IndexBufferCreateDuplicate ) != 0 )
							{
								GpuIndexBuffer buffer = null;
								if( ( creationFlags & ModifiableMeshCreationFlags.IndexBufferDynamic ) != 0 )
								{
									buffer = GpuBufferManager.CreateIndexBuffer( (int[])sourceOp.IndexBuffer.Indices.Clone(), GpuBufferFlags.Dynamic | GpuBufferFlags.ComputeRead );
								}
								else if( ( creationFlags & ModifiableMeshCreationFlags.IndexBufferComputeWrite ) != 0 )
								{
									buffer = GpuBufferManager.CreateIndexBuffer( sourceOp.IndexBuffer.IndexCount, GpuBufferFlags.ComputeWrite | GpuBufferFlags.ComputeRead );
								}
								op.IndexBuffer = buffer;

								//op.IndexBuffer = GpuBufferManager.CreateIndexBuffer( (int[])sourceOp.IndexBuffer.Indices.Clone(), creationFlags.HasFlag( ModifiableMeshCreationFlags.IndexBufferUpdateDynamic ) );// indexBufferUpdateDynamic );

								if( modifiableMeshBuffersToDispose == null )
									modifiableMeshBuffersToDispose = new List<ThreadSafeDisposable>();
								modifiableMeshBuffersToDispose.Add( op.IndexBuffer );
							}
							else
								op.IndexBuffer = sourceOp.IndexBuffer;
						}
						op.IndexStartOffset = sourceOp.IndexStartOffset;
						op.IndexCount = sourceOp.IndexCount;
						//}

						//material
						op.Material = sourceOp.Material;

						//var item = new Mesh.CompiledData.RenderOperationItem();
						//item.operation = op;
						//item.transform = sourceItem.transform;

						//op.transform = sourceOp.transform;


						//apply transform
						//op.transform = NeoAxis.Transform.Identity;
						//if( !sourceOp.transform.IsIdentity )
						//{
						//	//!!!!check
						//	//!!!!slowly

						//	Mat4F mat4 = sourceOp.transform.ToMat4().ToMat4F();
						//	Mat3F mat3 = mat4.ToMat3();

						//	//positions
						//	{
						//		if( op.vertexDeclaration.GetElementBySemantic( VertexElementSemantic.Position, out VertexElement element ) &&
						//			element.Type == VertexElementType.Float3 )
						//		{
						//			var buffer = op.vertexBuffers[ element.Source ];
						//			var ar = buffer.ExtractChannelFloat3( element.Offset );
						//			for( int n = 0; n < ar.Length; n++ )
						//				ar[ n ] = mat4 * ar[ n ];
						//			buffer.WriteChannel( element.Offset, ar );
						//		}
						//	}

						//	//normals
						//	{
						//		if( op.vertexDeclaration.GetElementBySemantic( VertexElementSemantic.Normal, out VertexElement element ) &&
						//			element.Type == VertexElementType.Float3 )
						//		{
						//			var buffer = op.vertexBuffers[ element.Source ];
						//			var ar = buffer.ExtractChannelFloat3( element.Offset );
						//			for( int n = 0; n < ar.Length; n++ )
						//				ar[ n ] = ( mat3 * ar[ n ] ).GetNormalize();
						//			buffer.WriteChannel( element.Offset, ar );
						//		}
						//	}

						//	//tangents
						//	{
						//		if( op.vertexDeclaration.GetElementBySemantic( VertexElementSemantic.Tangent, out VertexElement element ) &&
						//			element.Type == VertexElementType.Float4 )
						//		{
						//			var buffer = op.vertexBuffers[ element.Source ];
						//			var ar = buffer.ExtractChannelFloat4( element.Offset );
						//			for( int n = 0; n < ar.Length; n++ )
						//				ar[ n ] = new Vec4F( ( mat3 * ar[ n ].ToVec3F() ).GetNormalize(), ar[ n ].W );
						//			buffer.WriteChannel( element.Offset, ar );
						//		}
						//	}
						//}

						compiledData.MeshData.RenderOperations.Add( op );
					}

					//!!!!tangents
					//SimpleMeshGenerator.GenerateBox( Vec3F.One, out vertices, out normals, out Vec4F[] tangents, out Vec2F[] texCoords, out indices );
					//meshCreated.Indices = indices;

					//meshCreated.Vertices = new byte[ vertexSize * vertices.Length ];
					//unsafe
					//{
					//	fixed ( byte* pVertices = meshCreated.Vertices.Value )
					//	{
					//		StandardVertexF* pVertex = (StandardVertexF*)pVertices;

					//		for( int n = 0; n < vertices.Length; n++ )
					//		{
					//			pVertex->position = vertices[ n ];
					//			pVertex->normal = normals[ n ];

					//			//!!!!
					//			//pVertex->tangent = xx;

					//			pVertex->color = ColorValue.FromColor( System.Drawing.Color.LightGray );

					//			//!!!!temp. как-то указывать способ натягивания? 
					//			pVertex->texCoord0 = pVertex->position.ToVec2() * 2;
					//			pVertex->texCoord1 = pVertex->texCoord0;
					//			pVertex->texCoord2 = pVertex->texCoord0;
					//			pVertex->texCoord3 = pVertex->texCoord0;

					//			pVertex++;
					//		}
					//	}
					//}
				};

				modifiableMesh.Enabled = true;
				modifiableMesh.PerformResultCompile();

				modifiableMeshCreatedByObject = createbyObject;
			}
		}

		//public event Action<MeshInSpace> ModifiableMesh_DestroyEvent;

		public void ModifiableMeshDestroy()
		{
			if( modifiableMesh != null )
			{
				modifiableMesh.RemoveFromParent( true );
				modifiableMesh.Dispose();
				modifiableMesh = null;

				if( modifiableMeshBuffersToDispose != null )
				{
					foreach( var buffer in modifiableMeshBuffersToDispose )
						buffer.Dispose();
					modifiableMeshBuffersToDispose = null;
				}

				modifiableMeshCreatedByObject = null;
				//ModifiableMesh_DestroyEvent?.Invoke( this );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void CreateData()
		{
			usedMeshDataWhenInitialized = Mesh.Value?.Result;

			////!!!!так?
			//ModifiableMesh_CreateDestroy();
		}

		void DestroyData()
		{
			ModifiableMeshDestroy();
			DestroyStaticModeData();

			//было в ModifiableMesh_Destroy
			//activeController = null;

			usedMeshDataWhenInitialized = null;
		}

		void RecreateData()
		{
			if( EnabledInHierarchyAndIsInstance )
			{
				DestroyData();
				CreateData();
				SpaceBoundsUpdate();
			}

			RecreateBody();
			NeedUpdateStaticModeAndStaticShadows();

			//if( StaticShadows )
			//	ResetLightStaticShadowsCache();
		}

		public Mesh.CompiledData.RayCastResult RayCast( Ray ray, Mesh.CompiledData.RayCastModes mode )
		{

			//!!!!additional items?


			//!!!!может еще что-то проверять?
			Mesh mesh = MeshOutput;
			if( mesh != null && mesh.Result != null )
			{
				ref var bounds = ref SpaceBounds.boundingBox;
				if( bounds.Intersects( ray, out _ ) )
				{
					//check by geometry

					//!!!!can be cached?
					Transform.Value.ToMatrix4().GetInverse( out var transInv );
					Ray localRay = transInv * ray;

					var rayCastResult = mesh.Result.RayCast( localRay, NeoAxis.Mesh.CompiledData.RayCastModes.Auto, true );
					if( rayCastResult != null )
						return rayCastResult;
				}
			}

			return null;
		}

		//public bool RayCast( Ray ray, Mesh.CompiledData.RayCastMode mode, out double scale, out Vector3F normal, out int triangleIndex )
		//{
		//	//!!!!может еще что-то проверять?
		//	Mesh mesh = MeshOutput;
		//	if( mesh != null && mesh.Result != null )
		//	{
		//		var bounds = SpaceBounds.CalculatedBoundingBox;
		//		if( bounds.Intersects( ray, out _ ) )
		//		{
		//			//check by geometry

		//			//!!!!can be cached?
		//			Transform.Value.ToMatrix4().GetInverse( out var transInv );
		//			Ray localRay = transInv * ray;

		//			bool twoSided = true;
		//			//bool twoSided = IsTwoSided( mesh, ReplaceMaterial );

		//			if( mesh.Result.RayCast( localRay, mode, twoSided, out scale, out normal, out triangleIndex ) )
		//				return true;
		//		}
		//	}

		//	scale = 0;
		//	triangleIndex = -1;
		//	normal = Vector3F.Zero;
		//	return false;
		//}

		//!!!!

		//public class SweepTestItem
		//{
		//	public Matrix4 From;
		//	public Matrix4 To;
		//	public Bounds? ShapeBounds;

		//	public double Scale;
		//}

		//public bool SweepTest( SweepTestItem item )

		static bool PlanesContains( OpenList<Plane> list, ref Plane plane, double epsilon )
		{
			for( int n = 0; n < list.Count; n++ )
			{
				ref var p = ref list.Data[ n ];
				if( p.Equals( ref plane, epsilon ) )
					return true;
			}
			return false;
		}

		//!!!!
		internal bool _Intersects( Vector3[] vertices, int[] indices )
		{
			//!!!!может еще что-то проверять?
			Mesh.CompiledData m = MeshOutput?.Result;
			if( m != null )
			{
				//!!!!can be cached?
				Transform.Value.ToMatrix4().GetInverse( out var transInv );

				var localVertices = new Vector3[ vertices.Length ];
				for( int n = 0; n < vertices.Length; n++ )
					localVertices[ n ] = transInv * vertices[ n ];

				var planes = new OpenList<Plane>( indices.Length / 3 );
				//var planes = new Plane[ indices.Length / 3 ];
				for( int n = 0; n < indices.Length / 3; n++ )
				{
					ref var v0 = ref localVertices[ indices[ n * 3 + 0 ] ];
					ref var v1 = ref localVertices[ indices[ n * 3 + 1 ] ];
					ref var v2 = ref localVertices[ indices[ n * 3 + 2 ] ];
					Plane.FromPoints( ref v0, ref v1, ref v2, out var plane );

					if( !PlanesContains( planes, ref plane, 0.0001 ) )
						planes.Add( plane );
					//planes[ n ] = plane;
				}

				Bounds bounds = Bounds.Cleared;
				bounds.Add( localVertices );

				if( m._IntersectsFast( planes.ToArray(), ref bounds ) )
					return true;
			}

			return false;
		}

		//internal bool _Intersects( Plane[] planes )
		//{
		//	//!!!!может еще что-то проверять?
		//	Mesh.CompiledData m = MeshOutput?.Result;
		//	if( m != null )
		//	{
		//		//m._Intersects(

		//		//!!!!can be optimized
		//		var transInv = Transform.Value.ToMatrix4().GetInverse();//.GetTranspose();

		//		var localPlanes = new Plane[ planes.Length ];
		//		for( int n = 0; n < planes.Length; n++ )
		//		{
		//			var p = planes[ n ];
		//			var p2 = ( transInv * p.ToVector4() ).ToPlane();
		//			localPlanes[ n ] = p2;
		//		}

		//		if( m._Intersects( localPlanes ) )
		//			return true;
		//	}

		//	return false;
		//}

		//maybe add GetLinearVelocityByRenderingData()
		bool GetTransformPositionByTime( double time, out Vector3 position )
		{
			if( Math.Abs( transformPositionByTime2_Time - time ) < 0.00001 )
			{
				position = transformPositionByTime2_Position;
				return true;
			}
			if( Math.Abs( transformPositionByTime1_Time - time ) < 0.00001 )
			{
				position = transformPositionByTime1_Position;
				return true;
			}
			position = Vector3.Zero;
			return false;
		}

		bool AdditionalItemsGetTransformPositionByTime( ref AdditionalItemPreviousTransform previousTransform, double time, out Vector3 position )
		{
			if( Math.Abs( previousTransform.transformPositionByTime2_Time - time ) < 0.00001 )
			{
				position = previousTransform.transformPositionByTime2_Position;
				return true;
			}
			if( Math.Abs( previousTransform.transformPositionByTime1_Time - time ) < 0.00001 )
			{
				position = previousTransform.transformPositionByTime1_Position;
				return true;
			}
			position = Vector3.Zero;
			return false;
		}

		//public void ResetLodTransitionStates( ViewportRenderingContext resetOnlySpecifiedContext = null )
		//{
		//	SceneLODUtility.ResetLodTransitionStates( ref renderingContextItems, resetOnlySpecifiedContext );
		//}

		RenderingPipeline.RenderSceneData.LayerItem[] CalculatePaintLayersByComponents()
		{
			var items = new List<RenderingPipeline.RenderSceneData.LayerItem>();

			foreach( var layer in GetComponents<PaintLayer>() )
			{
				if( layer.Enabled )
				{
					var mask = layer.GetMaskImage( out var uniqueMaskDataCounter );
					if( mask != null )
						items.Add( new RenderingPipeline.RenderSceneData.LayerItem( layer, mask, uniqueMaskDataCounter, true ) );
				}
			}

			if( items.Count != 0 )
				return items.ToArray();
			else
				return null;
		}

		RenderingPipeline.RenderSceneData.LayerItem[] GetPaintLayers()
		{
			if( PaintLayersReplace != null )
				return PaintLayersReplace;
			else
			{
				if( paintLayersNeedUpdate )
				{
					paintLayers = CalculatePaintLayersByComponents();
					paintLayersNeedUpdate = false;
				}
				return paintLayers;
			}
		}

		public void PaintLayersNeedUpdate()
		{
			paintLayersNeedUpdate = true;
		}

		public void SetTransparentRenderingAddOffsetWhenSortByDistance( bool value )
		{
			transparentRenderingAddOffsetWhenSortByDistance = value;
		}

		protected override Scene.SceneObjectFlags OnGetSceneObjectFlags()
		{
			var result = ( EngineApp.IsSimulation && Static ) ? Scene.SceneObjectFlags.Logic : base.OnGetSceneObjectFlags();
			if( Occluder )
				result |= Scene.SceneObjectFlags.Occluder;
			return result;
		}

		//protected override bool OnOcclusionCullingDataContains()
		//{
		//	if( Occluder )
		//		return true;

		//	return base.OnOcclusionCullingDataContains();
		//}

		protected override void OnOcclusionCullingDataGet( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem, OpenList<RenderingPipeline.OccluderItem> occluders )
		{
			base.OnOcclusionCullingDataGet( context, mode, modeGetObjectsItem, occluders );

			if( Occluder )
			{
				//!!!!тут?
				if( Mesh.Value?.Result != usedMeshDataWhenInitialized )
					RecreateData();

				var mesh = MeshOutput;
				if( mesh != null && mesh.Result != null )
				{
					//!!!!use special lod or last?

					var tr = Transform.Value;
					var vertices = mesh.Result.ExtractedVerticesPositions;
					var indices = mesh.Result.ExtractedIndices;

					if( vertices != null && indices != null )
					{
						if( occluderCacheTransform == null || tr != occluderCacheTransform || !ReferenceEquals( vertices, occluderCacheVerticesOriginal ) || !ReferenceEquals( indices, occluderCacheIndices ) )
						{
							occluderCacheTransform = tr;
							occluderCacheVerticesOriginal = vertices;

							ref var matrix = ref tr.ToMatrix4();

							occluderCacheVertices = new Vector3[ vertices.Length ];
							for( int n = 0; n < occluderCacheVertices.Length; n++ )
							{
								var v = vertices[ n ].ToVector3();
								Matrix4.Multiply( ref matrix, ref v, out occluderCacheVertices[ n ] );
								//occluderCacheVertices[ n ] = tr * vertices[ n ];
							}

							occluderCacheIndices = indices;
						}


						var item = new RenderingPipeline.OccluderItem();
						item.Center = occluderCacheTransform.Position;
						item.Vertices = occluderCacheVertices;
						item.Indices = occluderCacheIndices;
						occluders.Add( ref item );
					}
				}
			}
		}

		protected override void OnTransformChanged()
		{

			//!!!!additional items


			NeedUpdateStaticModeAndStaticShadows();

			if( physicalBody != null && !updatePropertiesWithoutUpdatingBody )
			{
				var bodyTransform = Transform.Value;

				if( physicalBodyCreatedTransformScale != bodyTransform.Scale )
				{
					RecreateBody();
				}
				else
				{
					var pos = bodyTransform.Position;
					var rot = bodyTransform.Rotation.ToQuaternionF();
					var activate = true;
					physicalBody?.SetTransform( ref pos, ref rot, activate );
				}
			}

			base.OnTransformChanged();
		}

		[Browsable( false )]
		public Scene.PhysicsWorldClass.Body PhysicalBody
		{
			get { return physicalBody; }
		}

		Scene.PhysicsWorldClass GetPhysicsWorldData()
		{
			var scene = ParentScene;
			if( scene != null )
				return scene.PhysicsWorld;
			return null;
		}

		static void SetMaterial( RigidBody collisionDefinition, Scene.PhysicsWorldClass.Body b )
		{
			//!!!!impl all params

			//material settings
			PhysicalMaterial mat = collisionDefinition.Material.Value;
			if( mat != null )
			{
				b.Friction = (float)mat.Friction;
				b.Restitution = (float)mat.RigidRestitution;
			}
			else
			{
				b.Friction = (float)collisionDefinition.MaterialFriction;
				b.Restitution = (float)collisionDefinition.MaterialRestitution;
			}
		}

		//!!!!additional items. rename CreateBodies?

		protected virtual RigidBody OnGetCollisionShapeData( Mesh.CompiledData meshResult )
		{
			var collisionDefinition = meshResult.Owner.GetComponent<RigidBody>( "Collision Definition" );
			if( collisionDefinition == null )
				collisionDefinition = meshResult.GetAutoCollisionDefinition();

			return collisionDefinition;
		}

		protected virtual void OnCollisionBodyCreated() { }

		void CreateBody()
		{
			if( !Collision )
				return;

			duringCreateDestroy = true;

			var physicsWorldData = GetPhysicsWorldData();
			if( physicsWorldData != null )
			{
				if( physicalBody != null )
					Log.Fatal( "RigidBody: CreateBody: physicalBody != null." );
				if( !EnabledInHierarchyAndIsInstance )
					Log.Fatal( "RigidBody: CreateBody: !EnabledInHierarchyAndIsInstance." );

				var bodyTransform = Transform.Value;

				var meshResult = MeshOutput?.Result;
				if( meshResult != null )
				{
					var collisionDefinition = OnGetCollisionShapeData( meshResult );
					if( collisionDefinition != null )
					{
						var nativeShape = physicsWorldData.AllocateShape( collisionDefinition, bodyTransform.Scale );
						if( nativeShape != null )
						{
							var motionType = collisionDefinition.MotionType.Value;
							//if( Static )
							//	motionType = PhysicsMotionType.Static;

							var activate = motionType == PhysicsMotionType.Dynamic;

							////!!!!optionally can be not activated
							//activate = false;

							var pos = bodyTransform.Position;
							var rot = bodyTransform.Rotation.ToQuaternionF();

							var centerOfMassOffset = collisionDefinition.CenterOfMassOffset.Value.ToVector3F();
							//var centerOfMassManual = collisionDefinition.CenterOfMassManual.Value;
							//var centerOfMassPosition = collisionDefinition.CenterOfMassPosition.Value.ToVector3F();
							//impl?
							//var inertiaTensorFactor = Vector3F.One;//InertiaTensorFactor.Value.ToVector3F();

							var body = physicsWorldData.CreateRigidBody( nativeShape, true, this, motionType, (float)collisionDefinition.LinearDamping.Value, (float)collisionDefinition.AngularDamping.Value, ref pos, ref rot, activate, (float)collisionDefinition.Mass, ref centerOfMassOffset, /*centerOfMassManual, ref centerOfMassPosition, ref inertiaTensorFactor, */collisionDefinition.MotionQuality.Value, collisionDefinition.CharacterMode );

							if( body != null )
							{
								if( motionType != PhysicsMotionType.Static )
								{
									body.LinearVelocity = PhysicalBodyLinearVelocity.Value.ToVector3F();
									body.AngularVelocity = PhysicalBodyAngularVelocity.Value.ToVector3F();
									body.GravityFactor = (float)collisionDefinition.GravityFactor;
								}

								if( motionType == PhysicsMotionType.Dynamic )
								{

									//!!!!

								}

								SetMaterial( collisionDefinition, body );

								physicalBody = body;
								physicalBodyCreatedTransformScale = bodyTransform.Scale;

								OnCollisionBodyCreated();

								//collision sound
								collisionSound = collisionDefinition.CollisionSound;
								if( collisionSound != null )
								{
									collisionSoundTimeInterval = (float)collisionDefinition.CollisionSoundTimeInterval;
									collisionSoundMinSpeedChange = collisionDefinition.CollisionSoundMinSpeedChange;
								}
							}
						}
						else
							DestroyBody();
					}


					//var collisionDefinition = mesh.GetComponent<RigidBody>( "Collision Definition" );
					//if( collisionDefinition != null )
					//{
					//	var nativeShape = physicsWorldData.AllocateShape( collisionDefinition, bodyTransform.Scale );
					//	if( nativeShape != null )
					//	{
					//		var motionType = collisionDefinition.MotionType.Value;
					//		var activate = motionType == PhysicsMotionType.Dynamic;
					//		var pos = bodyTransform.Position;
					//		var rot = bodyTransform.Rotation.ToQuaternionF();

					//		//use local variable to prevent double update inside properties
					//		//!!!!
					//		var centerOfMassManual = false;//CenterOfMassManual.Value;
					//		var centerOfMassPosition = Vector3F.Zero;//CenterOfMassPosition.Value.ToVector3F();
					//		var inertiaTensorFactor = Vector3F.One;//InertiaTensorFactor.Value.ToVector3F();

					//		var body = physicsWorldData.CreateRigidBody( nativeShape, true, this, motionType, (float)collisionDefinition.LinearDamping.Value, (float)collisionDefinition.AngularDamping.Value, ref pos, ref rot, activate, (float)collisionDefinition.Mass, centerOfMassManual, ref centerOfMassPosition, ref inertiaTensorFactor, collisionDefinition.MotionQuality.Value );

					//		if( body != null )
					//		{
					//			//!!!!как kinematic?
					//			if( motionType != PhysicsMotionType.Static )
					//			{
					//				body.LinearVelocity = PhysicalBodyLinearVelocity.Value.ToVector3F();
					//				body.AngularVelocity = PhysicalBodyAngularVelocity.Value.ToVector3F();
					//				body.GravityFactor = (float)collisionDefinition.GravityFactor;
					//			}

					//			if( motionType == PhysicsMotionType.Dynamic )
					//			{

					//				//!!!!

					//			}

					//			SetMaterial( collisionDefinition, body );

					//			physicalBody = body;
					//			physicalBodyCreatedTransformScale = bodyTransform.Scale;
					//		}
					//	}
					//	else
					//		DestroyBody();
					//}
					//else
					//{
					//	Log.Warning( "MeshInSpace: CreateBody: Can't create collision body for \'{0}\'. The mesh not contains Collision Definition.", mesh.GetPathFromRoot( true ) );
					//}
				}
			}

			duringCreateDestroy = false;
		}

		void DestroyBody()
		{
			duringCreateDestroy = true;

			//!!!!что еще удалять? констрейнты?

			if( physicalBody != null )
			{
				physicalBodyCreatedTransformScale = Vector3.Zero;
				physicalBody.Dispose();
				physicalBody = null;

				collisionSound = null;
			}

			duringCreateDestroy = false;
		}

		public void RecreateBody()
		{
			if( EnabledInHierarchyAndIsInstance && !duringCreateDestroy )
			{
				DestroyBody();
				CreateBody();
			}
		}

		//public void RenderPhysicalObject( ViewportRenderingContext context, out int verticesRendered )
		//{
		//	if( physicalBody != null )
		//		physicalBody.RenderPhysicalObject( context, out verticesRendered );
		//	else
		//		verticesRendered = 0;
		//}

		protected virtual void OnUpdateDataFromPhysicalBody( Transform currentTransform, ref Transform newTransform, ref Vector3F linearVelocity, ref Vector3F angularVelocity )
		{
		}

		[MethodImpl( (MethodImplOptions)512 )]
		internal void UpdateDataFromPhysicalBody()
		{
			if( physicalBody != null && physicalBody.MotionType != PhysicsMotionType.Static )
			{
				var pos = physicalBody.Position;
				var rot = physicalBody.Rotation;
				var linearVelocity = physicalBody.LinearVelocity;
				var angularVelocity = physicalBody.AngularVelocity;

				var currentTransform = Transform.Value;
				var newTransform = new Transform( pos, rot, currentTransform.Scale );

				OnUpdateDataFromPhysicalBody( currentTransform, ref newTransform, ref linearVelocity, ref angularVelocity );

				try
				{
					updatePropertiesWithoutUpdatingBody = true;
					Transform = newTransform;
					PhysicalBodyLinearVelocity = linearVelocity.ToVector3();
					PhysicalBodyAngularVelocity = angularVelocity.ToVector3();
				}
				finally
				{
					updatePropertiesWithoutUpdatingBody = false;
				}

				//process collision sound
				if( collisionSound != null )
				{
					if( collisionSoundRemainingTime > 0 )
					{
						collisionSoundRemainingTime -= Time.SimulationDelta;
						if( collisionSoundRemainingTime < 0 )
							collisionSoundRemainingTime = 0;
					}

					if( collisionSoundRemainingTime == 0 && physicalBody.Active && physicalBody.ContactsExist() )
					{
						var changeSquared = ( physicalBody.LinearVelocity - physicalBody.PreviousLinearVelocity ).LengthSquared();
						var minChangeSquared = MathEx.Square( collisionSoundMinSpeedChange );
						if( changeSquared > minChangeSquared )
							CollisionSoundPlay();
					}
				}
			}
		}

		//!!!!?
		//public bool Active
		//public void Activate()
		//public void WantsDeactivation()
		//public void ApplyForce( Vector3 force, Vector3 relativePosition )

		//[Browsable( false )]
		//bool StaticModeEnabled
		//{
		//	get { return staticModeEnabled; }
		//}

		void NeedUpdateStaticModeAndStaticShadows()
		{
			needUpdateStaticModeAndStaticShadows = true;
		}

		//GroupOfObjects StaticMode_GetOrCreateGroupOfObjects( bool canCreate )
		//{
		//	var scene = ParentScene;
		//	if( scene == null )
		//		return null;

		//	var name = "__GroupOfObjectsMeshInSpaceStatic";

		//	var group = scene.GetComponent<GroupOfObjects>( name );
		//	if( group == null && canCreate )
		//	{
		//		//need set ShowInEditor = false before AddComponent
		//		group = ComponentUtility.CreateComponent<GroupOfObjects>( null, false, false );
		//		group.DisplayInEditor = false;
		//		scene.AddComponent( group, -1 );
		//		//var group = scene.CreateComponent<GroupOfObjects>();

		//		group.Name = name;
		//		//group.CanBeSelected = false;
		//		group.SaveSupport = false;
		//		group.CloneSupport = false;
		//		group.NetworkMode = NetworkModeEnum.False;

		//		group.AnyData = new Dictionary<(Mesh, Material, bool, double, bool, double), int>();


		//		//!!!!такое указывать в сцене?

		//		//UpdateGroupOfObjects( scene, group );

		//		group.Enabled = true;
		//	}

		//	return group;
		//}

		////!!!!удалять элементы? где еще так
		//static ushort StaticMode_GetOrCreateGroupOfObjectsElement( GroupOfObjects group, Mesh mesh, Material replaceMaterial, bool castShadows, double visibilityDistanceFactor, bool receiveDecals, double motionBlurFactor )
		//{
		//	var key = (mesh, replaceMaterial, castShadows, visibilityDistanceFactor, receiveDecals, motionBlurFactor);

		//	var dictionary = (Dictionary<(Mesh mesh, Material, bool, double, bool, double), int>)group.AnyData;

		//	GroupOfObjectsElement_Mesh element = null;

		//	if( dictionary.TryGetValue( key, out var elementIndex2 ) )
		//		return (ushort)elementIndex2;

		//	if( element == null )
		//	{
		//		var elementIndex = group.GetFreeElementIndex();
		//		element = group.CreateComponent<GroupOfObjectsElement_Mesh>( enabled: false );
		//		element.Name = "Element " + elementIndex.ToString();
		//		element.Index = elementIndex;
		//		element.Mesh = mesh;
		//		element.ReplaceMaterial = replaceMaterial;
		//		element.AutoAlign = false;
		//		element.CastShadows = castShadows;
		//		element.VisibilityDistanceFactor = visibilityDistanceFactor;
		//		element.ReceiveDecals = receiveDecals;
		//		element.MotionBlurFactor = motionBlurFactor;

		//		element.Enabled = true;

		//		dictionary[ key ] = element.Index;

		//		//!!!!how without it? везде так
		//		group.ElementTypesCacheNeedUpdate();
		//	}

		//	return (ushort)element.Index.Value;
		//}

		void UpdateStaticMode()
		{
			DestroyStaticModeData();

			if( Static )
			{
				var scene = ParentScene;
				if( scene != null )
				{
					if( staticModeGroupOfObjects == null )
					{
						staticModeGroupOfObjects = GroupOfObjectsUtility.GetOrCreateGroupOfObjects( scene, "__GroupOfObjectsMeshInSpaceStatic", true, scene.MeshInSpaceStaticModeSectorSize );//, scene.MeshInSpaceStaticModeMaxObjectsInGroup );
					}

					if( staticModeGroupOfObjects != null )
					{
						var objects = new OpenList<GroupOfObjects.Object>( 1 + ( additionalItems != null ? additionalItems.Length : 0 ) );

						{
							var mesh = Mesh.Value;
							//подставлять Invalid? красный
							//if( mesh == null )
							//	mesh = ResourceUtility.MeshInvalid;

							if( mesh != null )
							{
								var tr = TransformV;

								var elementIndex = staticModeGroupOfObjects.GetOrCreateGroupOfObjectsElement( mesh, ReplaceMaterial, CastShadows, VisibilityDistanceFactor, ReceiveDecals, MotionBlurFactor, StaticShadows, false );

								var obj = new GroupOfObjects.Object( elementIndex, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, tr.Position, tr.Rotation.ToQuaternionF(), tr.Scale.ToVector3F(), Vector4F.Zero, Color, Vector4F.Zero, Vector4F.Zero, 0 );

								objects.Add( ref obj );
							}

							if( additionalItems != null )
							{
								for( int n = 0; n < additionalItems.Length; n++ )
								{
									ref var additionalItem = ref additionalItems[ n ];

									var tr = TransformV;
									Vector3 pos = tr.Position;
									Quaternion rot = tr.Rotation;
									Vector3 scl = tr.Scale;
									{
										pos += rot * ( additionalItem.Position * scl );
										rot *= additionalItem.Rotation;
										scl *= additionalItem.Scale;
									}
									//var tr2 = Transform.Value;
									//tr2 = tr2.ApplyOffset( additionalItem.Position, additionalItem.Rotation, additionalItem.Scale );

									var elementIndex = staticModeGroupOfObjects.GetOrCreateGroupOfObjectsElement( additionalItem.Mesh, additionalItem.ReplaceMaterial ?? ReplaceMaterial, CastShadows, VisibilityDistanceFactor, ReceiveDecals, MotionBlurFactor, StaticShadows, false );

									var obj = new GroupOfObjects.Object( elementIndex, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, pos, rot.ToQuaternionF(), scl.ToVector3F(), Vector4F.Zero, additionalItem.Color, Vector4F.Zero, Vector4F.Zero, 0 );

									//var obj = new GroupOfObjects.Object( elementIndex, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, tr2.Position, tr2.Rotation.ToQuaternionF(), tr2.Scale.ToVector3F(), Vector4F.Zero, additionalItem.Color, Vector4F.Zero, Vector4F.Zero, 0 );

									objects.Add( ref obj );
								}
							}
						}

						if( objects.Count != 0 )
						{
							var subGroup = new GroupOfObjects.SubGroup( objects.ArraySegment );
							staticModeGroupOfObjects.AddSubGroup( subGroup );
							if( staticModeGroupOfObjectSubGroups == null )
								staticModeGroupOfObjectSubGroups = new List<GroupOfObjects.SubGroup>();
							staticModeGroupOfObjectSubGroups.Add( subGroup );
						}

						staticModeEnabled = true;
					}
				}

				//ResetLightStaticShadowsCache();
			}

			needUpdateStaticModeAndStaticShadows = false;
		}

		void DestroyStaticModeData()
		{
			//if( Static )
			//	ResetLightStaticShadowsCache();

			if( staticModeGroupOfObjectSubGroups != null )
			{
				for( int n = 0; n < staticModeGroupOfObjectSubGroups.Count; n++ )
					staticModeGroupOfObjects?.RemoveSubGroup( staticModeGroupOfObjectSubGroups[ n ] );
				staticModeGroupOfObjectSubGroups = null;
			}
			//staticModeGroupOfObjects = null;

			staticModeEnabled = false;
		}

		public void NotifyInstantMovement()
		{
			transformPositionByTime2_Time = 0;
			transformPositionByTime1_Time = 0;

			if( additionalItemsPreviousTransform != null )
			{
				for( int n = 0; n < additionalItemsPreviousTransform.Length; n++ )
				{
					ref var previousTransform = ref additionalItemsPreviousTransform[ n ];
					previousTransform.transformPositionByTime2_Time = 0;
					previousTransform.transformPositionByTime1_Time = 0;
				}
			}
		}

		public delegate void CollisionSoundPlayBeforeDelegate( MeshInSpace sender, ref Sound sound, ref double volume, ref bool handled );
		public event CollisionSoundPlayBeforeDelegate CollisionSoundPlayBefore;

		public void CollisionSoundPlay()
		{
			if( !SoundWorld.BackendNull )
			{
				var sound = collisionSound;
				var volume = 1.0;

				var handled = false;
				CollisionSoundPlayBefore?.Invoke( this, ref sound, ref volume, ref handled );
				if( handled )
					return;

				if( sound != null )
				{
					//specify position of collision?
					ParentScene?.SoundPlay( sound, TransformV.Position, volume: volume );

					collisionSoundRemainingTime = collisionSoundTimeInterval;
				}
			}

			if( NetworkIsServer && collisionSound != null )
			{
				BeginNetworkMessageToEveryone( "CollisionSoundPlay" );
				EndNetworkMessage();
			}
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "CollisionSoundPlay" )
				CollisionSoundPlay();

			return true;
		}

		void ResetLightStaticShadowsCache()
		{
			var scene = ParentScene;
			if( scene != null && scene.HierarchyController != null && scene.HierarchyController.HierarchyEnabled && !scene.IsResource )
			{
				var bounds = SpaceBounds.BoundingBox;
				var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, MetadataManager.GetTypeOfNetType( typeof( Light ) ), true, bounds );
				//var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, bounds );
				scene.GetObjectsInSpace( item );

				foreach( var resultItem in item.Result )
				{
					var light = resultItem.Object as Light;
					light?.ResetStaticShadowsCache();
				}
			}
		}
	}
}
