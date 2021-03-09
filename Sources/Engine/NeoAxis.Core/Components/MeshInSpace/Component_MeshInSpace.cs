// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Mesh in the scene.
	/// </summary>
	public class Component_MeshInSpace : Component_ObjectInSpace
	{
		//creation
		Component_Mesh.CompiledData usedMeshDataWhenInitialized;

		//modifiable mesh
		const string modifiableMesh_Name = "ModifiableMesh";
		Component_Mesh modifiableMesh;
		List<ThreadSafeDisposable> modifiableMesh_BuffersToDispose;
		Component modifiableMesh_CreatedByObject;

		double transformPositionByTime1_Time;
		Vector3 transformPositionByTime1_Position;
		double transformPositionByTime2_Time;
		Vector3 transformPositionByTime2_Position;

		////controller
		//Component_MeshInSpaceController activeController;

		public delegate void GetRenderSceneDataAddToFrameDataDelegate( Component_MeshInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, ref Component_RenderingPipeline.RenderSceneData.MeshItem item );
		public event GetRenderSceneDataAddToFrameDataDelegate GetRenderSceneDataAddToFrameData;

		//List<SceneLODUtility.RenderingContextItem> renderingContextItems;

		bool paintLayersNeedUpdate = true;
		Component_RenderingPipeline.RenderSceneData.LayerItem[] paintLayers;

		//!!!!new
		bool transparentRenderingAddOffsetWhenSortByDistance;
		//[Browsable( false )]
		//public bool TransparentRenderingAddOffsetWhenSortByDistance { get; set; }

		/////////////////////////////////////////

		////!!!!так?
		//public interface IMeshInSpaceChild
		//{
		//	void ParentMeshInSpace_GetRenderSceneData();
		//}

		/////////////////////////////////////////

		/// <summary>
		/// The mesh used by the mesh object.
		/// </summary>
		[Serialize]
		public Reference<Component_Mesh> Mesh
		{
			get
			{
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
				if( _mesh.BeginSet( ref value ) )
				{
					try
					{
						MeshChanged?.Invoke( this );
						RecreateData();
					}
					finally { _mesh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<Component_MeshInSpace> MeshChanged;
		ReferenceField<Component_Mesh> _mesh;

		/// <summary>
		/// Replaces all geometries of the mesh by another material.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		public Reference<Component_Material> ReplaceMaterial
		{
			get { if( _replaceMaterial.BeginGet() ) ReplaceMaterial = _replaceMaterial.Get( this ); return _replaceMaterial.value; }
			set { if( _replaceMaterial.BeginSet( ref value ) ) { try { ReplaceMaterialChanged?.Invoke( this ); } finally { _replaceMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReplaceMaterial"/> property value changes.</summary>
		public event Action<Component_MeshInSpace> ReplaceMaterialChanged;
		ReferenceField<Component_Material> _replaceMaterial;

		/// <summary>
		/// Replaces selected geometries of the mesh by another material.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		public ReferenceList<Component_Material> ReplaceMaterialSelectively
		{
			get { return _replaceMaterialSelectively; }
		}
		public delegate void ReplaceMaterialSelectivelyChangedDelegate( Component_MeshInSpace sender );
		public event ReplaceMaterialSelectivelyChangedDelegate ReplaceMaterialSelectivelyChanged;
		ReferenceList<Component_Material> _replaceMaterialSelectively;

		/// <summary>
		/// The base color and opacity multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Component_MeshInSpace> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

		/// <summary>
		/// Maximum visibility range of the object.
		/// </summary>
		[DefaultValue( 10000.0 )]
		[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> VisibilityDistance
		{
			get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
			set { if( _visibilityDistance.BeginSet( ref value ) ) { try { VisibilityDistanceChanged?.Invoke( this ); } finally { _visibilityDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		public event Action<Component_MeshInSpace> VisibilityDistanceChanged;
		ReferenceField<double> _visibilityDistance = 10000.0;

		/// <summary>
		/// Whether to cast shadows on the other surfaces.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		public Reference<bool> CastShadows
		{
			get { if( _castShadows.BeginGet() ) CastShadows = _castShadows.Get( this ); return _castShadows.value; }
			set { if( _castShadows.BeginSet( ref value ) ) { try { CastShadowsChanged?.Invoke( this ); } finally { _castShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<Component_MeshInSpace> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// Whether it is possible to apply decals on the surface.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> ReceiveDecals
		{
			get { if( _receiveDecals.BeginGet() ) ReceiveDecals = _receiveDecals.Get( this ); return _receiveDecals.value; }
			set { if( _receiveDecals.BeginSet( ref value ) ) { try { ReceiveDecalsChanged?.Invoke( this ); } finally { _receiveDecals.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<Component_MeshInSpace> ReceiveDecalsChanged;
		ReferenceField<bool> _receiveDecals = true;

		/// <summary>
		/// Specifies settings for special object effects, such as an outline effect.
		/// </summary>
		[Cloneable( CloneType.Deep )]
		public Reference<List<ObjectSpecialRenderingEffect>> SpecialEffects
		{
			get { if( _specialEffects.BeginGet() ) SpecialEffects = _specialEffects.Get( this ); return _specialEffects.value; }
			set { if( _specialEffects.BeginSet( ref value ) ) { try { SpecialEffectsChanged?.Invoke( this ); } finally { _specialEffects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SpecialEffects"/> property value changes.</summary>
		public event Action<Component_MeshInSpace> SpecialEffectsChanged;
		ReferenceField<List<ObjectSpecialRenderingEffect>> _specialEffects = new List<ObjectSpecialRenderingEffect>();

		[Browsable( false )]
		public Component_RenderingPipeline.RenderSceneData.LayerItem[] PaintLayersReplace
		{
			get { return paintLayersReplace; }
			set
			{
				paintLayersReplace = value;
				paintLayersNeedUpdate = true;
			}
		}
		Component_RenderingPipeline.RenderSceneData.LayerItem[] paintLayersReplace;

		[Browsable( false )]
		public Component_RenderingPipeline.RenderSceneData.CutVolumeItem[] CutVolumes { get; set; }

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public Component_MeshInSpace()
		{
			_replaceMaterialSelectively = new ReferenceList<Component_Material>( this, () => ReplaceMaterialSelectivelyChanged?.Invoke( this ) );
		}

		public static bool IsTwoSided( Component_Mesh mesh, Component_Material replaceMaterial )
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

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			//update Transform when reference is specified
			if( EnabledInHierarchy && VisibleInHierarchy )
			{
				var t = Transform;
			}
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			var time = context.Owner.LastUpdateTime;
			if( time != transformPositionByTime1_Time )
			{
				transformPositionByTime2_Time = transformPositionByTime1_Time;
				transformPositionByTime2_Position = transformPositionByTime1_Position;
				transformPositionByTime1_Time = time;
				transformPositionByTime1_Position = TransformV.Position;
			}

			//if( EnabledInHierarchy )
			//{
			//	foreach( var iChild in GetComponents<IMeshInSpaceChild>( false, false, true ) )
			//		iChild.ParentMeshInSpace_GetRenderSceneData();

			//	//ModifiableMesh_CreateDestroy();
			//}

			//!!!!slowly?

			if( mode == GetRenderSceneDataMode.InsideFrustum || ( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && CastShadows ) )
			{
				//!!!!тут?
				if( Mesh.Value?.Result != usedMeshDataWhenInitialized )
					RecreateData();

				var mesh = MeshOutput;
				if( mesh != null && mesh.Result != null )
				{
					var context2 = context.objectInSpaceRenderingContext;
					context2.disableShowingLabelForThisObject = true;

					if( mode == GetRenderSceneDataMode.InsideFrustum || ( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && mesh.Result.MeshData.CastShadows ) )
					{
						var cameraSettings = context.Owner.CameraSettings;
						var tr = Transform.Value;

						var cameraDistanceMinSquared = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, SpaceBounds );
						var visibilityDistance = Math.Min( VisibilityDistance, mesh.Result.MeshData.VisibilityDistance );

						if( cameraDistanceMinSquared < visibilityDistance * visibilityDistance || mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum )
						{
							var cameraDistanceMin = MathEx.Sqrt( cameraDistanceMinSquared );
							var cameraDistanceMaxSquared = SceneLODUtility.GetCameraDistanceMax( cameraSettings, SpaceBounds );
							cameraDistanceMaxSquared *= cameraDistanceMaxSquared;

							var allowOutlineSelect = Math.Max( context2.selectedObjects.Count, context2.canSelectObjects.Count ) <= 1000 && context.renderingPipeline.UseRenderTargets && ProjectSettings.Get.SceneEditorSelectOutlineEffectEnabled;

							var item = new Component_RenderingPipeline.RenderSceneData.MeshItem();
							item.Creator = this;
							item.BoundingSphere = SpaceBounds.CalculatedBoundingSphere;
							item.BoundingBoxCenter = item.BoundingSphere.Origin;
							//SpaceBounds.CalculatedBoundingBox.GetCenter( out item.BoundingBoxCenter );
							//item.MeshData = mesh.Result.MeshData;
							//item.CastShadows = CastShadows && item.MeshData.CastShadows;
							item.ReceiveDecals = ReceiveDecals;
							item.ReplaceMaterial = ReplaceMaterial;
							if( ReplaceMaterialSelectively.Count != 0 )
							{
								//!!!!может fixed массив юзать если влазит
								item.ReplaceMaterialSelectively = new Component_Material[ ReplaceMaterialSelectively.Count ];
								for( int n = 0; n < ReplaceMaterialSelectively.Count; n++ )
									item.ReplaceMaterialSelectively[ n ] = ReplaceMaterialSelectively[ n ].Value;
							}
							item.Color = Color;
							item.TransparentRenderingAddOffsetWhenSortByDistance = transparentRenderingAddOffsetWhenSortByDistance;
							item.VisibilityDistance = (float)visibilityDistance;
							item.CutVolumes = CutVolumes;

							var specialEffects = SpecialEffects.Value;
							if( specialEffects != null && specialEffects.Count != 0 )
								item.SpecialEffects = specialEffects;

							//display outline effect of editor selection
							if( mode == GetRenderSceneDataMode.InsideFrustum && allowOutlineSelect && context2.selectedObjects.Contains( this ) )
							{
								var color = ProjectSettings.Get.SelectedColor.Value;
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

							//PositionPreviousFrame
							var previousTime = time - context.Owner.LastUpdateTimeStep;
							if( !GetTransformPositionByTime( previousTime, out var previousPosition ) )
								previousPosition = tr.Position;
							//!!!!double
							item.PositionPreviousFrame = previousPosition.ToVector3F();


							int item0BillboardMode = 0;

							SceneLODUtility.GetDemandedLODs( context, mesh, cameraDistanceMinSquared, cameraDistanceMaxSquared, out var lodState );
							for( int nLodItem = 0; nLodItem < lodState.Count; nLodItem++ )
							{
								lodState.GetItem( nLodItem, out var lodLevel, out var lodRange );


								item.MeshData = mesh.Result.MeshData;
								if( lodLevel > 0 )
								{
									ref var lod = ref mesh.Result.MeshData.LODs[ lodLevel - 1 ];
									var lodMeshData = lod.Mesh?.Result?.MeshData;
									if( lodMeshData != null )
										item.MeshData = lodMeshData;
								}

								item.CastShadows = CastShadows && item.MeshData.CastShadows;
								item.LODValue = SceneLODUtility.GetLodValue( lodRange, cameraDistanceMin );
								//item.LODRange = lodRange;

								//calculate MeshInstanceOne
								if( nLodItem == 0 )
									item0BillboardMode = item.MeshData.BillboardMode;
								if( nLodItem == 0 || item0BillboardMode != item.MeshData.BillboardMode )
								{
									if( item.MeshData.BillboardMode != 0 )
									{
										var position = tr.Position;
										var scale = tr.Scale;
										var scaleH = (float)Math.Max( scale.X, scale.Y );

										ref var result = ref item.Transform;
										result.Item0.X = scaleH;
										result.Item0.Y = 0;
										result.Item0.Z = 0;
										result.Item0.W = 0;
										result.Item1.X = 0;
										result.Item1.Y = scaleH;
										result.Item1.Z = 0;
										result.Item1.W = 0;
										result.Item2.X = 0;
										result.Item2.Y = 0;
										result.Item2.Z = (float)scale.Z;
										result.Item2.W = 0;
										//!!!!double
										result.Item3.X = (float)position.X;
										result.Item3.Y = (float)position.Y;
										result.Item3.Z = (float)position.Z;
										result.Item3.W = 1;

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
										//!!!!double
										matrix.ToMatrix4F( out item.Transform );
									}
								}

								//layers
								{
									var a1 = item.MeshData.PaintLayers;
									var a2 = GetPaintLayers();
									if( a1 != null && a2 != null )
									{
										var z = new Component_RenderingPipeline.RenderSceneData.LayerItem[ a1.Length + a2.Length ];
										a1.CopyTo( z, 0 );
										a2.CopyTo( z, a1.Length );
										item.Layers = z;
									}
									else
										item.Layers = a1 ?? a2;
								}

								//add to render
								{
									//set AnimationData from event
									GetRenderSceneDataAddToFrameData?.Invoke( this, context, mode, ref item );

									//add the item to render
									context.FrameData.RenderSceneData.Meshes.Add( ref item );
								}
							}


							//display editor selection
							if( mode == GetRenderSceneDataMode.InsideFrustum )
							{
								if( ( !allowOutlineSelect && context2.selectedObjects.Contains( this ) ) || context2.canSelectObjects.Contains( this ) )
								{
									ColorValue color;
									if( context2.selectedObjects.Contains( this ) )
										color = ProjectSettings.Get.SelectedColor;
									else
										color = ProjectSettings.Get.CanSelectColor;

									var renderer = context.Owner.Simple3DRenderer;
									if( renderer != null )
									{
										color.Alpha *= .5f;
										renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

										if( Math.Max( context2.selectedObjects.Count, context2.canSelectObjects.Count ) > 1000 )
											renderer.AddBounds( SpaceBounds.CalculatedBoundingBox, true );
										else
										{
											bool twoSided = IsTwoSided( mesh, ReplaceMaterial ) || ParentScene.Mode.Value == Component_Scene.ModeEnum._2D;
											renderer.AddMesh( mesh.Result, item.Transform, false, !twoSided );
										}
									}
								}
							}
						}
					}
				}
			}
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			Component_Mesh m = MeshOutput;
			var result = m?.Result;
			if( result != null )
			{
				if( result.MeshData.BillboardMode != 0 )
				{
					var tr = Transform.Value;
					var meshSphere = result.MeshData.SpaceBounds.CalculatedBoundingSphere;
					newBounds = new SpaceBounds( new Sphere( tr.Position, meshSphere.Radius * tr.Scale.MaxComponent() ) );
				}
				else
				{
					//!!!!slowly

					var meshSpaceBounds = result.SpaceBounds;

					//!!!!ниже уже в потоке можно

					//!!!!можно считать оптимальнее?
					var b = SpaceBounds.Multiply( Transform, meshSpaceBounds );
					newBounds = SpaceBounds.Merge( newBounds, b );

					//Vec3[] points = m.Result.Bounds.ToPoints();
					//var mat4 = Transform.Value.ToMat4();
					//var b = Bounds.Cleared;
					//foreach( var p in points )
					//	b.Add( mat4 * p );
				}
			}
		}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			//!!!!может еще что-то проверять?
			Component_Mesh mesh = MeshOutput;
			if( mesh != null && mesh.Result != null )
			{
				context.thisObjectWasChecked = true;

				if( mesh.Result.MeshData.BillboardMode != 0 )
				{
					//billboard mode

					if( SpaceBounds.CalculatedBoundingSphere.Intersects( context.ray, out var scale1, out var scale2 ) )
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

					var bounds = SpaceBounds.CalculatedBoundingBox;
					if( bounds.Intersects( context.ray, out double scale ) )
					{
						if( mesh.Result.ExtractedIndices != null )
						{
							//check by geometry

							//!!!!can be cached?
							Transform.Value.ToMatrix4().GetInverse( out var transInv );
							Ray localRay = transInv * context.ray;

							int minTriangles = 30;

							Component_Mesh.CompiledData.RayCastMode mode;
							if( mesh.Result.ExtractedIndices.Length > minTriangles * 3 )
								mode = Component_Mesh.CompiledData.RayCastMode.OctreeOptimizedCached;
							else
								mode = Component_Mesh.CompiledData.RayCastMode.BruteforceNoCache;

							bool twoSided = true;
							//bool twoSided = IsTwoSided( mesh, ReplaceMaterial );

							if( mesh.Result.RayCast( localRay, mode, twoSided, out double scale2, out int dummy ) )
								context.thisObjectResultRayScale = scale2;
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
		//	//Component_Mesh m = MeshOutput;

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

		public delegate void MeshOutputOverrideDelegate( Component_MeshInSpace sender, ref Component_Mesh result );
		public event MeshOutputOverrideDelegate MeshOutputOverride;

		[Browsable( false )]
		public virtual Component_Mesh MeshOutput
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

			if( EnabledInHierarchy )
				CreateData();
			else
				DestroyData();
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
		public Component_Mesh ModifiableMesh
		{
			get { return modifiableMesh; }
		}

		[Browsable( false )]
		public Component ModifiableMesh_CreatedByObject
		{
			get { return modifiableMesh_CreatedByObject; }
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

		public void ModifiableMesh_Create( Component createbyObject, ModifiableMeshCreationFlags creationFlags )// bool vertexBuffersUpdateDynamic, bool indexBufferUpdateDynamic )
		{
			ModifiableMesh_Destroy();

			Component_Mesh mesh = Mesh;
			if( mesh != null )
			{
				//need set ShowInEditor = false before AddComponent
				modifiableMesh = ComponentUtility.CreateComponent<Component_Mesh>( null, false, false );
				modifiableMesh.DisplayInEditor = false;
				AddComponent( modifiableMesh, -1 );
				//modifiableMesh = CreateComponent<Component_Mesh>( -1, false );

				modifiableMesh.Name = modifiableMesh_Name;
				modifiableMesh.SaveSupport = false;
				modifiableMesh.CloneSupport = false;
				modifiableMesh.AllowDisposeBuffers = false;

				var geometry = modifiableMesh.CreateComponent<Component_MeshGeometry>();
				//geometry.Name = "Mesh Geometry";

				//!!!!потом может быть другой размер меша и т.д.

				modifiableMesh.MeshCompileEvent += delegate ( Component_Mesh sender, Component_Mesh.CompiledData compiledData )
				{
					foreach( var sourceOp in mesh.Result.MeshData.RenderOperations )
					{
						var op = new Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation( geometry );
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

						op.VertexBuffers = new List<GpuVertexBuffer>();
						foreach( var sourceBuffer in sourceOp.VertexBuffers )
						{
							if( creationFlags.HasFlag( ModifiableMeshCreationFlags.VertexBuffersCreateDuplicate ) )
							{
								GpuVertexBuffer buffer = null;
								if( creationFlags.HasFlag( ModifiableMeshCreationFlags.VertexBuffersDynamic ) )
								{
									buffer = GpuBufferManager.CreateVertexBuffer( (byte[])sourceBuffer.Vertices.Clone(), sourceBuffer.VertexDeclaration, GpuBufferFlags.Dynamic );
								}
								else if( creationFlags.HasFlag( ModifiableMeshCreationFlags.VertexBuffersComputeWrite ) )
								{
									buffer = GpuBufferManager.CreateVertexBuffer( sourceBuffer.VertexCount, sourceBuffer.VertexDeclaration, GpuBufferFlags.ComputeWrite );
								}

								//var buffer = GpuBufferManager.CreateVertexBuffer( (byte[])sourceBuffer.Vertices.Clone(), sourceBuffer.VertexDeclaration, creationFlags.HasFlag( ModifiableMeshCreationFlags.VertexBuffersUpdateDynamic ) );// vertexBuffersUpdateDynamic );

								op.VertexBuffers.Add( buffer );

								if( modifiableMesh_BuffersToDispose == null )
									modifiableMesh_BuffersToDispose = new List<ThreadSafeDisposable>();
								modifiableMesh_BuffersToDispose.Add( buffer );
							}
							else
								op.VertexBuffers.Add( sourceBuffer );
						}
						op.VertexStartOffset = sourceOp.VertexStartOffset;
						op.VertexCount = sourceOp.VertexCount;

						//index buffer
						if( sourceOp.IndexBuffer != null )
						{
							if( creationFlags.HasFlag( ModifiableMeshCreationFlags.IndexBufferCreateDuplicate ) )
							{
								GpuIndexBuffer buffer = null;
								if( creationFlags.HasFlag( ModifiableMeshCreationFlags.IndexBufferDynamic ) )
								{
									buffer = GpuBufferManager.CreateIndexBuffer( (int[])sourceOp.IndexBuffer.Indices.Clone(), GpuBufferFlags.Dynamic );
								}
								else if( creationFlags.HasFlag( ModifiableMeshCreationFlags.IndexBufferComputeWrite ) )
								{
									buffer = GpuBufferManager.CreateIndexBuffer( sourceOp.IndexBuffer.IndexCount, GpuBufferFlags.ComputeWrite );
								}
								op.IndexBuffer = buffer;

								//op.IndexBuffer = GpuBufferManager.CreateIndexBuffer( (int[])sourceOp.IndexBuffer.Indices.Clone(), creationFlags.HasFlag( ModifiableMeshCreationFlags.IndexBufferUpdateDynamic ) );// indexBufferUpdateDynamic );

								if( modifiableMesh_BuffersToDispose == null )
									modifiableMesh_BuffersToDispose = new List<ThreadSafeDisposable>();
								modifiableMesh_BuffersToDispose.Add( op.IndexBuffer );
							}
							else
								op.IndexBuffer = sourceOp.IndexBuffer;
						}
						op.IndexStartOffset = sourceOp.IndexStartOffset;
						op.IndexCount = sourceOp.IndexCount;
						//}

						//material
						op.Material = sourceOp.Material;

						//var item = new Component_Mesh.CompiledData.RenderOperationItem();
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
				modifiableMesh.ResultCompile();

				modifiableMesh_CreatedByObject = createbyObject;
			}
		}

		//public event Action<Component_MeshInSpace> ModifiableMesh_DestroyEvent;

		public void ModifiableMesh_Destroy()
		{
			if( modifiableMesh != null )
			{
				modifiableMesh.RemoveFromParent( true );
				modifiableMesh.Dispose();
				modifiableMesh = null;

				if( modifiableMesh_BuffersToDispose != null )
				{
					foreach( var buffer in modifiableMesh_BuffersToDispose )
						buffer.Dispose();
					modifiableMesh_BuffersToDispose = null;
				}

				modifiableMesh_CreatedByObject = null;
				//ModifiableMesh_DestroyEvent?.Invoke( this );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//[Browsable( false )]
		//public Component_MeshInSpaceController ActiveController
		//{
		//	get { return activeController; }
		//}

		//public virtual void UpdateActiveController()
		//{
		//	Component_MeshInSpaceController controllerToActivate = null;
		//	if( EnabledInHierarchy )
		//	{
		//		var mesh = Mesh.Value;
		//		if( mesh != null )
		//			controllerToActivate = FindController();
		//	}

		//	if( controllerToActivate != activeController )
		//	{
		//		activeController.Deactivate();
		//		xx xx;
		//	}





		//	xx xx;
		//	if( controllerToActivate != null && controllerToActivate.CheckNeedModifiableMesh( this ) )
		//		needCreate = true;


		//	xx xx;
		//}

		//xx xx;
		//Component_MeshInSpaceController FindController()
		//{
		//	//!!!!slowly
		//	//!!!!может быть несколько?

		//	return GetComponent<Component_MeshInSpaceController>( false, true );
		//}

		////bool ModifiableMesh_CheckNeedToCreate()
		////{
		////	if( EnabledInHierarchy )
		////	{
		////		var mesh = Mesh.Value;
		////		if( mesh != null )
		////		{
		////			var controller = GetController();
		////			if( controller != null && controller.CheckNeedModifiableMesh( this ) )
		////				return true;
		////		}
		////	}
		////	return false;
		////}

		////!!!!когда еще вызывать?
		///*public*/
		//void ModifiableMesh_CreateDestroy()
		//{
		//	xx xx;
		//	bool needCreate = false;
		//	Component_MeshInSpaceController controller = null;

		//	if( EnabledInHierarchy )
		//	{
		//		var mesh = Mesh.Value;
		//		if( mesh != null )
		//		{
		//			controller = FindController();

		//			xx xx;
		//			if( controller != null && controller.CheckNeedModifiableMesh( this ) )
		//				needCreate = true;
		//		}
		//	}

		//	if( controller != ActiveController )
		//		ModifiableMesh_Destroy();

		//	if( controller != null )
		//	{
		//		activeController = controller;

		//		xx xx;

		//		//create
		//		if( modifiableMesh == null )
		//			ModifiableMesh_Create();

		//		//update
		//		controller.UpdateModifiableMesh( this );
		//	}
		//	else
		//		ModifiableMesh_Destroy();


		//	//if( ModifiableMesh_CheckNeedToCreate() )
		//	//{
		//	//	//create
		//	//	if( modifiableMesh == null )
		//	//		ModifiableMesh_Create();

		//	//	//update
		//	//	ModifiableMesh_Update();
		//	//}
		//	//else
		//	//	ModifiableMesh_Destroy();
		//}

		//void ModifiableMesh_Update()
		//{
		//	var controller = GetController();
		//	controller?.UpdateModifiableMesh( this );
		//}

		void CreateData()
		{
			usedMeshDataWhenInitialized = Mesh.Value?.Result;

			////!!!!так?
			//ModifiableMesh_CreateDestroy();
		}

		void DestroyData()
		{
			ModifiableMesh_Destroy();

			//xx xx;//было в ModifiableMesh_Destroy
			//activeController = null;

			usedMeshDataWhenInitialized = null;
		}

		void RecreateData()
		{
			if( EnabledInHierarchy )
			{
				DestroyData();
				CreateData();
				SpaceBoundsUpdate();
			}
		}

		//!!!!new
		public bool RayCast( Ray ray, Component_Mesh.CompiledData.RayCastMode mode, out double scale, out int triangleIndex )
		{
			//!!!!может еще что-то проверять?
			Component_Mesh mesh = MeshOutput;
			if( mesh != null && mesh.Result != null )
			{
				var bounds = SpaceBounds.CalculatedBoundingBox;
				if( bounds.Intersects( ray, out _ ) )
				{
					//check by geometry

					//!!!!can be cached?
					Transform.Value.ToMatrix4().GetInverse( out var transInv );
					Ray localRay = transInv * ray;

					bool twoSided = true;
					//bool twoSided = IsTwoSided( mesh, ReplaceMaterial );

					if( mesh.Result.RayCast( localRay, mode, twoSided, out scale, out triangleIndex ) )
						return true;
				}
			}

			scale = 0;
			triangleIndex = -1;
			return false;
		}

		//!!!!

		//public class SweepTestItem
		//{
		//	public Matrix4 From;
		//	public Matrix4 To;
		//	public Bounds? ShapeBounds;

		//	public double Scale;
		//}

		//public bool SweepTest( SweepTestItem item )

		static bool PlanesContains( List<Plane> list, ref Plane plane, double epsilon )
		{
			for( int n = 0; n < list.Count; n++ )
				if( list[ n ].Equals( ref plane, epsilon ) )
					return true;
			return false;
		}

		//!!!!
		internal bool _Intersects( Vector3[] vertices, int[] indices )
		{
			//!!!!может еще что-то проверять?
			Component_Mesh.CompiledData m = MeshOutput?.Result;
			if( m != null )
			{
				//!!!!can be cached?
				Transform.Value.ToMatrix4().GetInverse( out var transInv );

				var localVertices = new Vector3[ vertices.Length ];
				for( int n = 0; n < vertices.Length; n++ )
					localVertices[ n ] = transInv * vertices[ n ];

				var planes = new List<Plane>( indices.Length / 3 );
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

				if( m._Intersects( planes.ToArray(), ref bounds ) )
					return true;
			}

			return false;
		}

		//internal bool _Intersects( Plane[] planes )
		//{
		//	//!!!!может еще что-то проверять?
		//	Component_Mesh.CompiledData m = MeshOutput?.Result;
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

		//public void ResetLodTransitionStates( ViewportRenderingContext resetOnlySpecifiedContext = null )
		//{
		//	SceneLODUtility.ResetLodTransitionStates( ref renderingContextItems, resetOnlySpecifiedContext );
		//}

		Component_RenderingPipeline.RenderSceneData.LayerItem[] CalculatePaintLayersByComponents()
		{
			var items = new List<Component_RenderingPipeline.RenderSceneData.LayerItem>();

			foreach( var layer in GetComponents<Component_PaintLayer>() )
			{
				if( layer.Enabled )
				{
					//if( layer.MaskImage.Value != null || layer.Mask.Value != null )
					var image = layer.GetImage( out var uniqueMaskDataCounter );
					if( image != null )
					{
						var item = new Component_RenderingPipeline.RenderSceneData.LayerItem();
						item.SetMaterialWithAbilityToCompileTransparentMaskVariation( layer.Material, layer.BlendMode );
						item.Mask = image;
						item.UniqueMaskDataCounter = uniqueMaskDataCounter;
						item.Color = layer.Color;
						items.Add( item );
					}
				}
			}

			if( items.Count != 0 )
				return items.ToArray();
			else
				return null;
		}

		Component_RenderingPipeline.RenderSceneData.LayerItem[] GetPaintLayers()
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

	}
}
