// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Particle system in the scene.
	/// </summary>
	[EditorSettingsCell( typeof( Component_ParticleSystemInSpace_SettingsCell ) )]
	public class Component_ParticleSystemInSpace : Component_ObjectInSpace
	{
		//creation
		Component_ParticleSystem.CompiledData currentParticleSystem;
		int currentMustRecreateCounter;

		double transformPositionByTime1_Time;
		Vector3[] transformPositionByTime1_Position;
		double transformPositionByTime2_Time;
		Vector3[] transformPositionByTime2_Position;

		List<SceneLODUtility.RenderingContextItem> billboardRenderingContextItems;

		Emitter[] Emitters;
		Particle[] Objects;
		OpenList<bool> removedObjects;//Count = capacity of Particles. Can be null.
		Stack<int> freeObjects = new Stack<int>();

		float playingTime;
		bool playingEnded;

		Random random;

		//!!!!удалять. когда опции меняются тоже
		//public Batch batch;

		/////////////////////////////////////////

		/// <summary>
		/// The particle system used by the object.
		/// </summary>
		public Reference<Component_ParticleSystem> ParticleSystem
		{
			get
			{
				//fast exit optimization
				var cachedReference = _particleSystem.value.Value?.GetCachedResourceReference();
				if( cachedReference != null )
				{
					if( ReferenceEquals( _particleSystem.value.GetByReference, cachedReference ) )
						return _particleSystem.value;
					if( _particleSystem.value.GetByReference == cachedReference )
					{
						_particleSystem.value.GetByReference = cachedReference;
						return _particleSystem.value;
					}
				}
				if( _particleSystem.BeginGet_WithoutFastExitOptimization() ) ParticleSystem = _particleSystem.Get( this ); return _particleSystem.value;
				//if( _particleSystem.BeginGet() ) ParticleSystem = _particleSystem.Get( this ); return _particleSystem.value;
			}
			set
			{
				if( _particleSystem.BeginSet( ref value ) )
				{
					try
					{
						ParticleSystemChanged?.Invoke( this );
						RecreateData( false );
					}
					finally { _particleSystem.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ParticleSystem"/> property value changes.</summary>
		public event Action<Component_ParticleSystemInSpace> ParticleSystemChanged;
		ReferenceField<Component_ParticleSystem> _particleSystem;

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
		public event Action<Component_ParticleSystemInSpace> ReplaceMaterialChanged;
		ReferenceField<Component_Material> _replaceMaterial;

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
		public event Action<Component_ParticleSystemInSpace> ColorChanged;
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
		public event Action<Component_ParticleSystemInSpace> VisibilityDistanceChanged;
		ReferenceField<double> _visibilityDistance = 10000.0;

		/// <summary>
		/// Whether to cast shadows on the other surfaces.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> CastShadows
		{
			get { if( _castShadows.BeginGet() ) CastShadows = _castShadows.Get( this ); return _castShadows.value; }
			set { if( _castShadows.BeginSet( ref value ) ) { try { CastShadowsChanged?.Invoke( this ); } finally { _castShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<Component_ParticleSystemInSpace> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// Whether it is possible to apply decals to the surface.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> ReceiveDecals
		{
			get { if( _receiveDecals.BeginGet() ) ReceiveDecals = _receiveDecals.Get( this ); return _receiveDecals.value; }
			set { if( _receiveDecals.BeginSet( ref value ) ) { try { ReceiveDecalsChanged?.Invoke( this ); } finally { _receiveDecals.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<Component_ParticleSystemInSpace> ReceiveDecalsChanged;
		ReferenceField<bool> _receiveDecals = true;

		/// <summary>
		/// Whether to display the shapes of the emitters.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> DisplayEmitters
		{
			get { if( _displayEmitters.BeginGet() ) DisplayEmitters = _displayEmitters.Get( this ); return _displayEmitters.value; }
			set { if( _displayEmitters.BeginSet( ref value ) ) { try { DisplayEmittersChanged?.Invoke( this ); } finally { _displayEmitters.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayEmitters"/> property value changes.</summary>
		public event Action<Component_ParticleSystemInSpace> DisplayEmittersChanged;
		ReferenceField<bool> _displayEmitters = false;

		/// <summary>
		/// Whether the particle system is in active state.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> Activated
		{
			get { if( _activated.BeginGet() ) Activated = _activated.Get( this ); return _activated.value; }
			set { if( _activated.BeginSet( ref value ) ) { try { ActivatedChanged?.Invoke( this ); } finally { _activated.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Activated"/> property value changes.</summary>
		public event Action<Component_ParticleSystemInSpace> ActivatedChanged;
		ReferenceField<bool> _activated = true;

		public float PlayingTime
		{
			get { return playingTime; }
		}

		[Browsable( false )]
		public bool PlayingEnded
		{
			get { return playingEnded; }
		}

		/////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		struct Emitter
		{
			public float StartTime;
			public float Duration;
			public float AvailableTimeToSpawn;

			public List<SceneLODUtility.RenderingContextItem> meshRenderingContextItems;
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents a particle for <see cref="Component_ParticleSystemInSpace"/>.
		/// </summary>
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public struct Particle
		{
			//constants
			public int Emitter;
			public float Lifetime;
			public float StartSize;
			public float GravityMultiplier;
			public ColorValue StartColor;

			public float Time;

			public Vector3 Position;
			public Matrix3F Rotation;
			//public Vector3F Scale;
			public float Size;

			public Vector3F LinearVelocity;
			public Vector3F AngularVelocity;

			public ColorValue Color;

			public Vector4F AnyData;
			//public Vector4F Special1;
			//public Vector4F Special2;
		}

		/////////////////////////////////////////

		//public class Batch
		//{
		//	//!!!!

		//	//public ushort Element;
		//	//public byte VariationGroup;
		//	//public byte VariationElement;

		//	//public List<int> Objects = new List<int>( 128 );

		//	////public Vector3 ObjectsCenter;
		//	//public Vector3 BoundingBoxCenter;
		//	//public Bounds BoundingBoxObjectPositions;
		//	//public Sphere BoundingSphere;

		//	public GpuVertexBuffer BatchingInstanceBufferMesh;
		//	public GpuVertexBuffer BatchingInstanceBufferBillboard;

		//	//!!!!
		//	//public List<SceneLOD.RenderingContextItem> renderingContextItems;
		//}

		/////////////////////////////////////////

		class MeshLodItem
		{
			public SceneLODUtility.RenderingContextItem contextItem;
			public int itemLod = 0;
			public float itemLodValue = 0;
			public int item2Lod = 0;
		}

		/////////////////////////////////////////

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			base.OnGetRenderSceneData( context, mode );

			//update old positions for motion blur
			var time = context.Owner.LastUpdateTime;
			if( time != transformPositionByTime1_Time )
			{
				transformPositionByTime2_Time = transformPositionByTime1_Time;
				transformPositionByTime2_Position = transformPositionByTime1_Position;
				transformPositionByTime1_Time = time;

				//!!!!slowly

				transformPositionByTime1_Position = null;

				var tr = Transform.Value;
				ref var trMatrix = ref tr.ToMatrix4();

				transformPositionByTime1_Position = new Vector3[ ObjectsGetCapacity() ];
				for( int n = 0; n < transformPositionByTime1_Position.Length; n++ )
					transformPositionByTime1_Position[ n ] = new Vector3( double.NaN, double.NaN, double.NaN );
				foreach( var particleIndex in ObjectsGetAll() )
				{
					ref var particle = ref Objects[ particleIndex ];

					Vector3 p;
					if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.Local )
						Matrix4.Multiply( ref trMatrix, ref particle.Position, out p );
					else
						p = particle.Position;
					transformPositionByTime1_Position[ particleIndex ] = p;
				}
			}

			if( mode == GetRenderSceneDataMode.InsideFrustum || ( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && CastShadows ) )
			{
				var particleSystemResult = ParticleSystem.Value?.Result;

				//check for update
				if( particleSystemResult != currentParticleSystem )
				{
					RecreateData( true );
					particleSystemResult = currentParticleSystem;
				}

				if( particleSystemResult != null && ObjectsGetCount() != 0 )
				{
					//var context2 = context.objectInSpaceRenderingContext;
					//context2.disableShowingLabelForThisObject = true;

					var tr = Transform.Value;
					ref var trMatrix = ref tr.ToMatrix4();
					//!!!!double
					trMatrix.ToMatrix4F( out var trMatrixF );
					var trPosition = tr.Position;

					//update and get LOD info
					var maxDistance = Math.Min( VisibilityDistance, currentParticleSystem.Owner.VisibilityDistance );
					var billboardLodContextItem = SceneLODUtility.UpdateAndGetContextItem( ref billboardRenderingContextItems, context, null, maxDistance, ref trPosition );

					bool skip = false;
					//!!!!impl для мешей. сейчас резко появляются из far если меши
					if( billboardLodContextItem.currentLOD == -1 && billboardLodContextItem.transitionTime == 0 )
						skip = true;

					if( !skip )
					{
						MeshLodItem[] meshEmitterLodContexts = null;

						var replaceMaterial = ReplaceMaterial.Value;
						var color = Color.Value;

						//bool batching = particleSystemResult.Owner.Batching;

						//if( batching )
						//{
						//}
						//else
						{
							//PositionPreviousFrame
							var previousTime = time - context.Owner.LastUpdateTimeStep;
							GetTransformPositionByTime( previousTime, out var previousPositions );

							//init general parameters of billboard item
							var billboardItem = new Component_RenderingPipeline.RenderSceneData.BillboardItem();
							billboardItem.Creator = this;
							SpaceBounds.CalculatedBoundingBox.GetCenter( out billboardItem.BoundingBoxCenter );
							billboardItem.BoundingSphere = SpaceBounds.CalculatedBoundingSphere;

							{
								//get data for rendering
								float itemLodValue = 0;
								{
									if( billboardLodContextItem.currentLOD == 0 && billboardLodContextItem.toLOD == -1 )
										itemLodValue = billboardLodContextItem.transitionTime;
									else if( billboardLodContextItem.currentLOD == -1 && billboardLodContextItem.toLOD == 0 )
										itemLodValue = -billboardLodContextItem.transitionTime;
								}
								//set LOD value
								billboardItem.LODValue = itemLodValue;
							}

							//init general parameters of mesh item
							var meshItem = new Component_RenderingPipeline.RenderSceneData.MeshItem();
							meshItem.Creator = this;
							meshItem.BoundingBoxCenter = billboardItem.BoundingBoxCenter;
							meshItem.BoundingSphere = billboardItem.BoundingSphere;

							//!!!!slowly? ObjectsGetAll
							foreach( var particleIndex in ObjectsGetAll() )
							{
								ref var particle = ref Objects[ particleIndex ];

								if( particle.Emitter < currentParticleSystem.Emitters.Length )
								{
									var compiledEmitter = currentParticleSystem.Emitters[ particle.Emitter ];

									var renderingMode = compiledEmitter.Owner.RenderingMode.Value;
									if( renderingMode == Component_ParticleEmitter.RenderingModeEnum.Billboard )
									{
										//Billboard rendering mode

										billboardItem.CastShadows = compiledEmitter.CastShadows && CastShadows;

										if( mode == GetRenderSceneDataMode.InsideFrustum || ( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && billboardItem.CastShadows ) )
										{
											billboardItem.ReceiveDecals = compiledEmitter.ReceiveDecals && ReceiveDecals;
											billboardItem.Material = replaceMaterial ?? compiledEmitter.Material;

											//Position
											Vector3 p;
											if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.Local )
												Matrix4.Multiply( ref trMatrix, ref particle.Position, out p );
											else
												p = particle.Position;
											//!!!!double
											billboardItem.Position = p.ToVector3F();

											//Size
											if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.Local )
											{
												billboardItem.Size = new Vector2F( particle.Size * Math.Max( (float)tr.Scale.X, (float)tr.Scale.Y ), particle.Size * (float)tr.Scale.Z );
											}
											else
												billboardItem.Size = new Vector2F( particle.Size, particle.Size );

											if( billboardItem.Size.X > 0 && billboardItem.Size.Y > 0 )
											{
												//Rotation
												//!!!!slowly
												billboardItem.Rotation = MathEx.DegreeToRadian( particle.Rotation.ToAngles().Roll );

												//Color
												billboardItem.Color = particle.Color * color;

												//PositionPreviousFrame
												if( previousPositions != null && particleIndex < previousPositions.Length )
												{
													var p2 = previousPositions[ particleIndex ];
													if( !double.IsNaN( p2.X ) )
													{
														//!!!!double
														billboardItem.PositionPreviousFrame = p2.ToVector3F();
													}
													else
														billboardItem.PositionPreviousFrame = billboardItem.Position;
												}
												else
													billboardItem.PositionPreviousFrame = billboardItem.Position;

												//add
												context.FrameData.RenderSceneData.Billboards.Add( ref billboardItem );
											}
										}
									}
									else if( renderingMode == Component_ParticleEmitter.RenderingModeEnum.Mesh )
									{
										//Mesh rendering mode

										var mesh = compiledEmitter.Mesh;
										if( mesh == null || mesh.Result == null )
											mesh = ResourceUtility.MeshInvalid;


										meshItem.MeshData = mesh.Result.MeshData;
										meshItem.CastShadows = compiledEmitter.CastShadows && CastShadows;
										meshItem.ReceiveDecals = compiledEmitter.ReceiveDecals && ReceiveDecals;
										meshItem.ReplaceMaterial = replaceMaterial ?? compiledEmitter.Material;
										meshItem.Color = particle.Color * color;

										var lods = meshItem.MeshData.LODs;
										ref var emitter = ref Emitters[ particle.Emitter ];

										if( meshEmitterLodContexts == null )
											meshEmitterLodContexts = new MeshLodItem[ Emitters.Length ];

										MeshLodItem lodMeshItem = null;
										if( meshEmitterLodContexts[ particle.Emitter ] != null )
											lodMeshItem = meshEmitterLodContexts[ particle.Emitter ];
										else
										{
											lodMeshItem = new MeshLodItem();
											lodMeshItem.contextItem = SceneLODUtility.UpdateAndGetContextItem( ref emitter.meshRenderingContextItems, context, mesh, maxDistance, ref trPosition );

											//get data for rendering
											//lodMeshItem.itemLod = 0;
											//lodMeshItem.itemLodValue = 0;
											//lodMeshItem.item2Lod = 0;
											{
												int maxLOD = lods != null ? lods.Length : 0;

												lodMeshItem.itemLod = lodMeshItem.contextItem.currentLOD;
												if( lodMeshItem.itemLod > maxLOD )
													lodMeshItem.itemLod = maxLOD;
												lodMeshItem.item2Lod = lodMeshItem.contextItem.toLOD;
												if( lodMeshItem.item2Lod > maxLOD )
													lodMeshItem.item2Lod = maxLOD;
												lodMeshItem.itemLodValue = lodMeshItem.contextItem.transitionTime;
											}

											meshEmitterLodContexts[ particle.Emitter ] = lodMeshItem;
										}

										bool skip2 = false;
										if( lodMeshItem.contextItem.currentLOD == -1 && lodMeshItem.contextItem.transitionTime == 0 )
											skip2 = true;

										if( !skip2 )
										{
											//var lods = meshItem.MeshData.LODs;

											////get data for rendering
											//int itemLod = 0;
											//float itemLodValue = 0;
											//int item2Lod = 0;
											//{
											//	int maxLOD = lods != null ? lods.Length : 0;

											//	itemLod = contextItem.currentLOD;
											//	if( itemLod > maxLOD )
											//		itemLod = maxLOD;
											//	item2Lod = contextItem.toLOD;
											//	if( item2Lod > maxLOD )
											//		item2Lod = maxLOD;
											//	itemLodValue = contextItem.transitionTime;
											//}

											//select LOD
											if( lodMeshItem.itemLod > 0 )
											{
												ref var lod = ref lods[ lodMeshItem.itemLod - 1 ];
												var lodMeshData = lod.Mesh?.Result?.MeshData;
												if( lodMeshData != null )
												{
													meshItem.MeshData = lodMeshData;
													meshItem.CastShadows = compiledEmitter.CastShadows && CastShadows && lod.Mesh.CastShadows;
												}
											}

											//set LOD value
											meshItem.LODValue = lodMeshItem.itemLodValue;

											bool skipItem1 = false;
											bool skipItem2 = false;

											//calculate MeshInstanceOne
											if( meshItem.MeshData.BillboardMode != 0 )
											{
												//Position
												Vector3 position;
												if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.Local )
													Matrix4.Multiply( ref trMatrix, ref particle.Position, out position );
												else
													position = particle.Position;

												//Size
												Vector2F size;
												if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.Local )
												{
													var scale = tr.Scale;
													var scaleH = (float)Math.Max( scale.X, scale.Y );
													size = new Vector2F( particle.Size * scaleH, particle.Size * (float)scale.Z );
												}
												else
													size = new Vector2F( particle.Size, particle.Size );

												if( size.X > 0 && size.Y > 0 )
												{
													ref var result = ref meshItem.Transform;
													result.Item0.X = size.X;
													result.Item0.Y = 0;
													result.Item0.Z = 0;
													result.Item0.W = 0;
													result.Item1.X = 0;
													result.Item1.Y = size.X;
													result.Item1.Z = 0;
													result.Item1.W = 0;
													result.Item2.X = 0;
													result.Item2.Y = 0;
													result.Item2.Z = size.Y;
													result.Item2.W = 0;
													//!!!!double
													result.Item3.X = (float)position.X;
													result.Item3.Y = (float)position.Y;
													result.Item3.Z = (float)position.Z;
													result.Item3.W = 1;
												}
												else
													skipItem1 = true;
											}
											else
											{
												//Transform
												Matrix3F.FromScale( particle.Size, out var scl );
												Matrix3F.Multiply( ref particle.Rotation, ref scl, out var mat3 );
												//!!!!double
												var positionF = particle.Position.ToVector3F();
												if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.Local )
												{
													var mat4 = new Matrix4F( ref mat3, ref positionF );
													Matrix4F.Multiply( ref trMatrixF, ref mat4, out meshItem.Transform );
												}
												else
													meshItem.Transform = new Matrix4F( ref mat3, ref positionF );
											}

											//PositionPreviousFrame
											if( previousPositions != null && particleIndex < previousPositions.Length )
											{
												var p2 = previousPositions[ particleIndex ];
												if( !double.IsNaN( p2.X ) )
												{
													//!!!!double
													meshItem.PositionPreviousFrame = p2.ToVector3F();
												}
												else
													meshItem.Transform.GetTranslation( out meshItem.PositionPreviousFrame );
											}
											else
												meshItem.Transform.GetTranslation( out meshItem.PositionPreviousFrame );

											////layers
											//meshItem.Layers = Layers;

											//add first item
											if( lodMeshItem.itemLod >= 0 && !skipItem1 )
											{
												////set AnimationData from event
												//GetRenderSceneDataAddToFrameData?.Invoke( this, context, mode, ref meshItem );

												//add the item to render
												context.FrameData.RenderSceneData.Meshes.Add( ref meshItem );
											}

											//add second item
											if( lodMeshItem.item2Lod >= 0 && lodMeshItem.itemLodValue != 0 )
											{
												var item0BillboardMode = meshItem.MeshData.BillboardMode;

												meshItem.MeshData = mesh.Result.MeshData;
												meshItem.CastShadows = compiledEmitter.CastShadows && CastShadows;

												//select LOD
												if( lodMeshItem.item2Lod != 0 )
												{
													ref var lod = ref lods[ lodMeshItem.item2Lod - 1 ];
													var lodMeshData = lod.Mesh?.Result?.MeshData;
													if( lodMeshData != null )
													{
														meshItem.MeshData = lodMeshData;
														meshItem.CastShadows = compiledEmitter.CastShadows && CastShadows && lod.Mesh.CastShadows;
													}
												}

												//set LOD value
												meshItem.LODValue = -meshItem.LODValue;

												//calculate MeshInstanceOne
												if( item0BillboardMode != meshItem.MeshData.BillboardMode )
												{
													//set matrix
													if( meshItem.MeshData.BillboardMode != 0 )
													{
														//Position
														Vector3 position;
														if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.Local )
															Matrix4.Multiply( ref trMatrix, ref particle.Position, out position );
														else
															position = particle.Position;

														//Size
														Vector2F size;
														if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.Local )
														{
															var scale = tr.Scale;
															var scaleH = (float)Math.Max( scale.X, scale.Y );
															size = new Vector2F( particle.Size * scaleH, particle.Size * (float)scale.Z );
														}
														else
															size = new Vector2F( particle.Size, particle.Size );

														if( size.X > 0 && size.Y > 0 )
														{
															ref var result = ref meshItem.Transform;
															result.Item0.X = size.X;
															result.Item0.Y = 0;
															result.Item0.Z = 0;
															result.Item0.W = 0;
															result.Item1.X = 0;
															result.Item1.Y = size.X;
															result.Item1.Z = 0;
															result.Item1.W = 0;
															result.Item2.X = 0;
															result.Item2.Y = 0;
															result.Item2.Z = size.Y;
															result.Item2.W = 0;
															//!!!!double
															result.Item3.X = (float)position.X;
															result.Item3.Y = (float)position.Y;
															result.Item3.Z = (float)position.Z;
															result.Item3.W = 1;
														}
														else
															skipItem2 = true;
													}
													else
													{
														//Transform
														Matrix3F.FromScale( particle.Size, out var scl );
														Matrix3F.Multiply( ref particle.Rotation, ref scl, out var mat3 );
														//!!!!double
														var positionF = particle.Position.ToVector3F();
														if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.Local )
														{
															var mat4 = new Matrix4F( ref mat3, ref positionF );
															Matrix4F.Multiply( ref trMatrixF, ref mat4, out meshItem.Transform );
														}
														else
															meshItem.Transform = new Matrix4F( ref mat3, ref positionF );
													}
												}

												if( !skipItem2 )
												{
													////set AnimationData from event
													//GetRenderSceneDataAddToFrameData?.Invoke( this, context, mode, ref meshItem );

													//add the item to render
													context.FrameData.RenderSceneData.Meshes.Add( ref meshItem );
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}

			//display emitters
			if( mode == GetRenderSceneDataMode.InsideFrustum && ParentScene.GetDisplayDevelopmentDataInThisApplication() && DisplayEmitters )
			{
				//!!!!if( context2.displayLightsCounter < context2.displayLightsMax )

				if( currentParticleSystem != null )
				{
					var viewport = context.Owner;
					var context2 = context.objectInSpaceRenderingContext;

					var tr = Transform.Value;
					int verticesRendered = 0;

					foreach( var emitter in currentParticleSystem.Emitters )
					{
						bool emitterSelected = context2.selectedObjects.Contains( emitter.Owner );

						foreach( var shape in emitter.Owner.GetComponents<Component_ParticleEmitterShape>( false, false, false ) )
						{
							if( shape.Enabled )
							{
								ColorValue color;
								if( emitterSelected || context2.selectedObjects.Contains( shape ) )
									color = ProjectSettings.Get.SelectedColor;
								else
								{
									//!!!!add editor options
									color = new ColorValue( 0, 0, 0.8 );
								}
								viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
								shape.PerformRender( viewport, tr, false, ref verticesRendered );
							}
						}
					}
				}
			}
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			//!!!!slowly?
			//с отступом считать, реже удалять

			if( currentParticleSystem != null )
			{
				var bounds = Bounds.Cleared;

				var tr = Transform.Value;
				ref var trMatrix = ref tr.ToMatrix4();

				foreach( var particleIndex in ObjectsGetAll() )
				{
					ref var particle = ref Objects[ particleIndex ];

					Vector3 p;
					if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.Local )
						p = trMatrix * particle.Position;
					else
						p = particle.Position;

					var size = new Vector2F( particle.Size, particle.Size );

					Vector2 size2;
					if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.Local )
						size2 = new Vector2( size.X * Math.Max( tr.Scale.X, tr.Scale.Y ), size.Y * tr.Scale.Z );
					else
						size2 = size;

					if( size2.X > 0 && size2.Y > 0 )
					{
						var halfSize = size2.MaxComponent() * 0.5;
						if( halfSize < 0.001 )
							halfSize = 0.001;

						var b = new Bounds( p.X - halfSize, p.Y - halfSize, p.Z - halfSize, p.X + halfSize, p.Y + halfSize, p.Z + halfSize );
						bounds.Add( ref b );
					}

				}

				if( !bounds.IsCleared() )
					newBounds = new SpaceBounds( bounds );
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
				CreateData( false );
			else
				DestroyData();
		}

		protected override void OnUpdate( float delta )
		{
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor && currentParticleSystem != null )
			{
				bool wasUpdated = false;

				float remainingDelta = delta;
				if( remainingDelta > 0.0001 )
				{
					float step = Math.Min( remainingDelta, Time.SimulationDelta );
					Simulate( step * currentParticleSystem.Owner.SimulationSpeed, out var wasUpdated2 );
					remainingDelta -= step;

					if( wasUpdated2 )
						wasUpdated = true;
				}

				if( wasUpdated )
					SpaceBoundsUpdate();
			}
		}

		protected override void OnSimulationStep()
		{
			if( currentParticleSystem != null )
			{
				Simulate( Time.SimulationDelta * currentParticleSystem.Owner.SimulationSpeed, out var wasUpdated );
				if( wasUpdated )
					SpaceBoundsUpdate();
			}
		}

		void CreateData( bool canSavePreviousState )
		{
			//check must recreate simulation data
			bool mustRecreate = true;
			{
				if( canSavePreviousState && currentParticleSystem != null && Emitters != null )
				{
					var newParticleSystem = ParticleSystem.Value?.Result;
					if( newParticleSystem != null )
					{
						if( newParticleSystem.Owner.MustRecreateInstancesCounter == currentMustRecreateCounter &&
							newParticleSystem.Looping == currentParticleSystem.Looping &&
							newParticleSystem.SimulationSpace == currentParticleSystem.SimulationSpace &&
							newParticleSystem.Emitters.Length == Emitters.Length )
						{
							//неправильно проверять, т.к. потеряется поддержка динамического обновления

							//bool same = true;

							//for( int n = 0; n < Emitters.Length; n++ )
							//{
							//	var newEmitter = newParticleSystem.Emitters[ n ];
							//	var emitter = Emitters[ n ];

							//	if( !newEmitter.Equals( emitter ) )
							//	{
							//		same = false;
							//		break;
							//	}
							//}

							//if( same )
							mustRecreate = false;
						}
					}
				}
			}

			//destroy old data
			if( mustRecreate )
				DestroyData();

			//update current particle system
			currentParticleSystem = ParticleSystem.Value?.Result;
			if( currentParticleSystem == null )
			{
				DestroyData();
				return;
			}
			currentMustRecreateCounter = currentParticleSystem.Owner.MustRecreateInstancesCounter;

			//create simulation data
			if( !currentParticleSystem.Looping || mustRecreate || playingEnded )
			{
				playingTime = 0;
				playingEnded = false;
			}

			//create emitters
			if( mustRecreate )
				Emitters = new Emitter[ currentParticleSystem.Emitters.Length ];

			//update emitters
			if( random == null )
				random = new Random();
			for( int n = 0; n < Emitters.Length; n++ )
			{
				ref var emitter = ref Emitters[ n ];
				var compiledEmitter = currentParticleSystem.Emitters[ n ];

				emitter.StartTime = compiledEmitter.StartTime.GenerateValue( random );
				emitter.Duration = compiledEmitter.Duration.GenerateValue( random );
			}
		}

		unsafe void DestroyData()
		{
			BatchDestroy();
			currentParticleSystem = null;
			Emitters = null;
			ObjectsSet( null, 0 );
		}

		public void RecreateData( bool canSavePreviousState )
		{
			if( EnabledInHierarchy )
			{
				CreateData( canSavePreviousState );
				SpaceBoundsUpdate();
			}
		}

		public void Play()
		{
			RecreateData( true );
		}

		//maybe add GetLinearVelocityByRenderingData()
		bool GetTransformPositionByTime( double time, out Vector3[] position )
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
			position = null;
			return false;
		}

		public void ResetLodTransitionStates( ViewportRenderingContext resetOnlySpecifiedContext = null )
		{
			SceneLODUtility.ResetLodTransitionStates( ref billboardRenderingContextItems, resetOnlySpecifiedContext );

			if( Emitters != null )
			{
				for( int nEmitter = 0; nEmitter < Emitters.Length; nEmitter++ )
				{
					ref var emitter = ref Emitters[ nEmitter ];
					SceneLODUtility.ResetLodTransitionStates( ref emitter.meshRenderingContextItems, resetOnlySpecifiedContext );
				}
			}
		}

		void RemovedObjectsInit()
		{
			if( removedObjects == null )
			{
				int currentCapacity = ObjectsGetCapacity();

				removedObjects = new OpenList<bool>( currentCapacity );
				for( int index = 0; index < currentCapacity; index++ )
					removedObjects.Add( false );

				foreach( var index in freeObjects )
					removedObjects.Data[ index ] = true;
			}
		}

		public unsafe void ObjectsSet( Particle* data, int count )//, bool fullRecreateInternalData )
		{
			//if( fullRecreateInternalData )
			//{

			if( count != 0 )
			{
				var array = new Particle[ count ];
				fixed ( Particle* pArray = array )
					NativeUtility.CopyMemory( pArray, data, array.Length * sizeof( Particle ) );
				Objects = array;
			}
			else
				Objects = null;
			removedObjects = null;
			freeObjects.Clear();

			//}
			//else
			//{
			//}
		}

		public unsafe void ObjectsSet( ArraySegment<Particle> data )//, bool fullRecreateInternalData )
		{
			fixed ( Particle* pData = data.Array )
				ObjectsSet( pData + data.Offset, data.Count );//, fullRecreateInternalData );
		}

		public unsafe void ObjectsSet( Particle[] data )//, bool fullRecreateInternalData )
		{
			fixed ( Particle* pData = data )
				ObjectsSet( pData, data.Length );//, fullRecreateInternalData );
		}

		public unsafe void/*int[]*/ ObjectsAdd( Particle* data, int count )
		{
			RemovedObjectsInit();

			//expand capacity
			if( freeObjects.Count < count )
			{
				int currentCapacity = ObjectsGetCapacity();
				int currentCount = ObjectsGetCount();
				int demandCount = currentCount + count;
				int demandCapacity = Math.Max( MathEx.NextPowerOfTwo( demandCount ), currentCapacity );
				if( demandCapacity < 4 ) demandCapacity = 4;

				var newData = new Particle[ demandCapacity ];
				if( Objects != null )
				{
					for( int n = 0; n < currentCapacity; n++ )
						newData[ n ] = Objects[ n ];
				}
				Objects = newData;

				for( int index = demandCapacity - 1; index >= currentCapacity; index-- )
				{
					freeObjects.Push( index );
					removedObjects.Add( true );
				}
			}

			//var addedIndexes = new int[ count ];

			for( int n = 0; n < count; n++ )
			{
				var index = freeObjects.Pop();

				ref var obj = ref Objects[ index ];
				obj = data[ n ];
				//if( obj.UniqueIdentifier == 0 )
				//	obj.UniqueIdentifier = GetUniqueIdentifier();

				removedObjects.Data[ index ] = false;

				//addedIndexes[ n ] = index;
			}

			//return addedIndexes;
		}

		public unsafe void/*int[]*/ ObjectsAdd( ArraySegment<Particle> data )
		{
			fixed ( Particle* pData = data.Array )
				ObjectsAdd( pData + data.Offset, data.Count );
		}

		public unsafe void/*int[]*/ ObjectsAdd( Particle[] data )
		{
			fixed ( Particle* pData = data )
				ObjectsAdd( pData, data.Length );
		}

		public unsafe void ObjectsRemove( int* indexes, int count )
		{
			RemovedObjectsInit();

			for( int n = count - 1; n >= 0; n-- )
			{
				var index = indexes[ n ];
				freeObjects.Push( index );
				removedObjects.Data[ index ] = true;
			}
		}

		public void ObjectsRemove( List<int> indexes )
		{
			RemovedObjectsInit();

			for( int n = indexes.Count - 1; n >= 0; n-- )
			{
				var index = indexes[ n ];
				freeObjects.Push( index );
				removedObjects.Data[ index ] = true;
			}
		}

		public unsafe void ObjectsRemove( ArraySegment<int> indexes )
		{
			fixed ( int* pData = indexes.Array )
				ObjectsRemove( pData + indexes.Offset, indexes.Count );
		}

		public unsafe void ObjectsRemove( int[] indexes )
		{
			fixed ( int* pData = indexes )
				ObjectsRemove( pData, indexes.Length );
		}

		int ObjectsGetCapacity()
		{
			return Objects != null ? Objects.Length : 0;
		}

		public int ObjectsGetCount()
		{
			return ObjectsGetCapacity() - freeObjects.Count;
		}

		public List<int> ObjectsGetAll()
		{
			int capacity = ObjectsGetCapacity();
			var result = new List<int>( capacity );
			for( int index = 0; index < capacity; index++ )
			{
				//check is removed
				if( removedObjects != null && removedObjects.Data[ index ] )
					continue;
				result.Add( index );
			}
			return result;
		}

		public Particle[] ObjectsGetData( IList<int> indexes )
		{
			var result = new Particle[ indexes.Count ];
			if( indexes.Count != 0 )
			{
				int currentResult = 0;
				foreach( var index in indexes )
					result[ currentResult++ ] = Objects[ index ];
			}
			return result;
		}

		//public void ObjectsGetData( int index, out ObjectMesh data )
		//{
		//	data = ObjectsMesh[ index ];
		//}

		public ref Particle ObjectGetData( int index )
		{
			return ref Objects[ index ];
		}

		///// <summary>
		///// Removes objects by their unique identifiers.
		///// </summary>
		///// <param name="data"></param>
		//public int ObjectsRemove( Particle[] data )
		//{
		//	var set = new ESet<long>( data.Length );
		//	for( int n = 0; n < data.Length; n++ )
		//	{
		//		if( data[ n ].UniqueIdentifier == 0 )
		//			Log.Fatal( "Component_GroupOfObjects: ObjectsRemove: data[ n ].UniqueIdentifier == 0." );
		//		set.Add( data[ n ].UniqueIdentifier );
		//	}

		//	var indexesToRemove = new List<int>( data.Length );

		//	foreach( var index in ObjectsGetAll() )
		//	{
		//		ref var obj = ref ObjectGetData( index );
		//		if( set.Contains( obj.UniqueIdentifier ) )
		//			indexesToRemove.Add( index );
		//	}

		//	ObjectsRemove( indexesToRemove.ToArray() );

		//	return indexesToRemove.Count;
		//}

		public bool ObjectsExists()
		{
			return ObjectsGetCount() != 0;
		}

		public void ClearObjects()
		{
			ObjectsSet( new Particle[ 0 ] );//, true );
		}

		/////////////////////////////////////////

		void Simulate( float delta, out bool wasUpdated )
		{
			wasUpdated = false;

			if( random == null )
				random = new Random();

			//update playing time
			if( !playingEnded )
			{
				if( Activated )
					playingTime += delta;

				//check for replay
				{
					bool replay = true;
					for( int nEmitter = 0; nEmitter < Emitters.Length; nEmitter++ )
					{
						ref var emitter = ref Emitters[ nEmitter ];
						if( playingTime < emitter.StartTime + emitter.Duration )
						{
							replay = false;
							break;
						}
					}

					if( replay )
					{
						playingTime = 0;
						if( !currentParticleSystem.Looping || !Activated )
							playingEnded = true;

						//update emitters
						if( !playingEnded && Emitters.Length == currentParticleSystem.Emitters.Length )
						{
							for( int n = 0; n < Emitters.Length; n++ )
							{
								ref var emitter = ref Emitters[ n ];
								var compiledEmitter = currentParticleSystem.Emitters[ n ];

								emitter.StartTime = compiledEmitter.StartTime.GenerateValue( random );
								emitter.Duration = compiledEmitter.Duration.GenerateValue( random );
							}
						}
					}
				}
			}

			//emit particles
			if( !playingEnded && Activated )
			{
				//!!!!slowly
				var tr = Transform.Value;
				ref var trMatrix = ref tr.ToMatrix4();
				trMatrix.ToMatrix3( out var trMatrix3 );
				trMatrix3.ToMatrix3F( out var trMatrix3F );
				var trScale = tr.Scale;

				for( int nEmitter = 0; nEmitter < Emitters.Length; nEmitter++ )
				{
					ref var emitter = ref Emitters[ nEmitter ];
					var compiledEmitter = currentParticleSystem.Emitters[ nEmitter ];

					if( playingTime >= emitter.StartTime && playingTime < emitter.StartTime + emitter.Duration )
						emitter.AvailableTimeToSpawn += delta;

					//!!!!чтобы не зависло

					while( emitter.AvailableTimeToSpawn > 0 )
					{
						//calculate spawn time
						float spawnTime = 0;
						{
							var spawnRate = Math.Abs( compiledEmitter.SpawnRate.GenerateValue( random ) );
							if( spawnRate != 0 )
								spawnTime = 1.0f / spawnRate;
						}

						if( spawnTime <= 0 )
							break;

						//update available time to spawn
						emitter.AvailableTimeToSpawn -= spawnTime;

						//emit
						var spawnCount = compiledEmitter.SpawnCount.GenerateValue( random );
						for( int nSpawn = 0; nSpawn < spawnCount; nSpawn++ )
						{
							if( ObjectsGetCount() < currentParticleSystem.Owner.MaxParticles )
							{
								var particle = new Particle();

								particle.Emitter = nEmitter;
								particle.Lifetime = compiledEmitter.Lifetime.GenerateValue( random );

								particle.StartSize = compiledEmitter.Size.GenerateValue( random );
								if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.World )
									particle.StartSize *= (float)trScale.MaxComponent();
								particle.Size = particle.StartSize;

								particle.GravityMultiplier = compiledEmitter.GravityMultiplier.GenerateValue( random );

								//get shape
								var shapes = compiledEmitter.Shapes;
								if( shapes.Length > 0 )
								{
									int shapeIndex;
									if( shapes.Length > 1 )
									{
										unsafe
										{
											var groupProbabilities = stackalloc double[ shapes.Length ];
											for( int n = 0; n < shapes.Length; n++ )
												groupProbabilities[ n ] = shapes[ n ].Probability;
											shapeIndex = RandomUtility.GetRandomIndexByProbabilities( random, groupProbabilities, shapes.Length );
										}
									}
									else
										shapeIndex = 0;

									var shape = shapes[ shapeIndex ];
									var shapeTransform = shape.Transform.Value;

									Vector3 position;
									{
										shape.PerformGetLocalPosition( random, out var localPosition );
										if( !shapeTransform.IsIdentity )
											Matrix4.Multiply( ref shapeTransform.ToMatrix4(), ref localPosition, out position );
										else
											position = localPosition;
									}

									//Position
									if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.World )
										Matrix4.Multiply( ref trMatrix, ref position, out particle.Position );
									else
										particle.Position = position;

									//if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.World )
									//	particle.Scale = Vector3F.One * (float)trScale.MaxComponent();
									//else
									//particle.Scale = Vector3F.One;

									//LinearVelocity
									{
										Vector3F direction;
										if( compiledEmitter.Direction == Component_ParticleEmitter.DirectionEnum.EmitterDirection )
											direction = shapeTransform.Rotation.GetForward().ToVector3F();
										else //if( compiledEmitter.Direction == Component_ParticleEmitter.DirectionEnum.FromCenterOfEmitter )
										{
											var d = position - shapeTransform.Position;
											while( d == Vector3.Zero )
												d = new SphericalDirection( random.Next( Math.PI * 2 ), random.Next( -Math.PI / 2, Math.PI / 2 ) ).GetVector();
											direction = d.ToVector3F().GetNormalize();
										}

										var dispersionAngle = compiledEmitter.DispersionAngle.GenerateValue( random );
										if( dispersionAngle != 0 )
										{
											var axisAngle = random.Next( MathEx.PI * 2 );

											//!!!!slowly

											var r = QuaternionF.FromDirectionZAxisUp( direction );

											r *= QuaternionF.FromRotateByX( axisAngle ) * QuaternionF.FromRotateByY( MathEx.DegreeToRadian( dispersionAngle ) );

											direction = r.GetForward();
										}

										var speed = compiledEmitter.Speed.GenerateValue( random );

										if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.World )
											particle.LinearVelocity = trMatrix3F * ( direction.GetNormalize() * speed );
										else
											particle.LinearVelocity = direction.GetNormalize() * speed;
									}

									//Rotation
									{
										if( compiledEmitter.Owner.RenderingMode.Value == Component_ParticleEmitter.RenderingModeEnum.Mesh )
										{
											if( compiledEmitter.RotateAlongMovement && particle.LinearVelocity != Vector3F.Zero )
											{
												//!!!!slowly
												QuaternionF.FromDirectionZAxisUp( ref particle.LinearVelocity, out var q );
												q.ToMatrix3( out particle.Rotation );
											}
											else
											{
												if( currentParticleSystem.SimulationSpace == Component_ParticleSystem.SimulationSpaceEnum.World )
													particle.Rotation = trMatrix3F;
												else
													particle.Rotation = Matrix3F.Identity;
											}
										}
										else
											particle.Rotation = Matrix3F.Identity;

										{
											var rotation = compiledEmitter.Rotation.GenerateValue( random );
											if( rotation.X != 0 )
											{
												Matrix3F.FromRotateByX( MathEx.DegreeToRadian( rotation.X ), out var m );
												Matrix3F.Multiply( ref particle.Rotation, ref m, out var m2 );
												particle.Rotation = m2;
											}
											if( rotation.Y != 0 )
											{
												Matrix3F.FromRotateByY( MathEx.DegreeToRadian( rotation.Y ), out var m );
												Matrix3F.Multiply( ref particle.Rotation, ref m, out var m2 );
												particle.Rotation = m2;
											}
											if( rotation.Z != 0 )
											{
												Matrix3F.FromRotateByZ( MathEx.DegreeToRadian( rotation.Z ), out var m );
												Matrix3F.Multiply( ref particle.Rotation, ref m, out var m2 );
												particle.Rotation = m2;
											}
										}
									}

									//AngularVelocity
									particle.AngularVelocity = compiledEmitter.AngularVelocity.GenerateValue( random );

									//Color
									compiledEmitter.Color.GenerateValue( random, out particle.StartColor );
									particle.Color = particle.StartColor;

									//add
									unsafe
									{
										ObjectsAdd( &particle, 1 );
									}

									wasUpdated = true;
								}
							}
						}
					}
				}
			}

			//update particles
			if( Objects != null )
			{
				var scene = ParentScene;

				//!!!!GC
				List<int> toDelete = null;

				//!!!!slowly? ObjectsGetAll
				foreach( var particleIndex in ObjectsGetAll() )
				{
					ref var particle = ref Objects[ particleIndex ];

					if( particle.Emitter < currentParticleSystem.Emitters.Length )
					{
						var emitter = currentParticleSystem.Emitters[ particle.Emitter ];

						//custom modules
						{
							var items = emitter.CustomModules;
							for( int n = 0; n < items.Length; n++ )
								items[ n ].PerformUpdateBefore( this, delta, ref particle );
						}

						particle.Time += delta;
						if( particle.Time >= particle.Lifetime )
						{
							//delete
							if( toDelete == null )
								toDelete = new List<int>( 32 );
							toDelete.Add( particleIndex );

							wasUpdated = true;
						}
						else
						{
							//update linear, angular velocity
							{
								//LinearVelocityByTime
								{
									var items = emitter.LinearVelocityByTimes;
									for( int n = 0; n < items.Length; n++ )
									{
										ref var item = ref items[ n ];
										item.Calculate( particle.Lifetime, particle.Time, out var value );
										particle.LinearVelocity = value;
									}
								}

								//AngularVelocityByTime
								{
									var items = emitter.AngularVelocityByTimes;
									for( int n = 0; n < items.Length; n++ )
									{
										ref var item = ref items[ n ];
										item.Calculate( particle.Lifetime, particle.Time, out var value );
										particle.AngularVelocity = value;
									}
								}

								//GravityMultiplier
								if( particle.GravityMultiplier != 0 && scene != null )
									particle.LinearVelocity += scene.Gravity.Value.ToVector3F() * particle.GravityMultiplier * delta;

								//LinearAccelerationByTime
								{
									var items = emitter.LinearAccelerationByTimes;
									for( int n = 0; n < items.Length; n++ )
									{
										ref var item = ref items[ n ];
										item.Calculate( particle.Lifetime, particle.Time, out var value );
										particle.LinearVelocity += value * delta;
									}
								}

								//AngularAccelerationByTime
								{
									var items = emitter.AngularAccelerationByTimes;
									for( int n = 0; n < items.Length; n++ )
									{
										ref var item = ref items[ n ];
										item.Calculate( particle.Lifetime, particle.Time, out var value );
										particle.AngularVelocity += value * delta;
									}
								}
							}

							//update position
							{
								float linearSpeedMultiplier = 1;

								//LinearSpeedMultiplierByTime
								{
									var items = emitter.LinearSpeedMultiplierByTimes;
									for( int n = 0; n < items.Length; n++ )
									{
										ref var item = ref items[ n ];
										linearSpeedMultiplier *= item.Calculate( particle.Lifetime, particle.Time );
									}
								}

								particle.Position += particle.LinearVelocity * delta * linearSpeedMultiplier;
							}

							//update rotation
							{
								float angularSpeedMultiplier = 1;

								//AngularSpeedMultiplierByTime
								{
									var items = emitter.AngularSpeedMultiplierByTimes;
									for( int n = 0; n < items.Length; n++ )
									{
										ref var item = ref items[ n ];
										angularSpeedMultiplier *= item.Calculate( particle.Lifetime, particle.Time );
									}
								}

								if( angularSpeedMultiplier != 0 && particle.AngularVelocity != Vector3F.Zero )
								{
									var multiplier = MathEx.DegreeToRadian( delta * angularSpeedMultiplier );

									if( particle.AngularVelocity.X != 0 )
									{
										var v = particle.AngularVelocity.X * multiplier;
										Matrix3F.FromRotateByX( v, out var m );
										Matrix3F.Multiply( ref particle.Rotation, ref m, out var m2 );
										particle.Rotation = m2;
									}
									if( particle.AngularVelocity.Y != 0 )
									{
										var v = particle.AngularVelocity.Y * multiplier;
										Matrix3F.FromRotateByY( v, out var m );
										Matrix3F.Multiply( ref particle.Rotation, ref m, out var m2 );
										particle.Rotation = m2;
									}
									if( particle.AngularVelocity.Z != 0 )
									{
										var v = particle.AngularVelocity.Z * multiplier;
										Matrix3F.FromRotateByZ( v, out var m );
										Matrix3F.Multiply( ref particle.Rotation, ref m, out var m2 );
										particle.Rotation = m2;
									}
								}
							}

							//update size, color
							{
								particle.Size = particle.StartSize;
								particle.Color = particle.StartColor;

								//SizeMultiplierByTime
								{
									var items = emitter.SizeMultiplierByTimes;
									for( int n = 0; n < items.Length; n++ )
									{
										ref var item = ref items[ n ];
										particle.Size *= item.Calculate( particle.Lifetime, particle.Time );
									}
								}

								//ColorMultiplierByTime
								{
									var items = emitter.ColorMultiplierByTimes;
									for( int n = 0; n < items.Length; n++ )
									{
										ref var item = ref items[ n ];
										item.Calculate( particle.Lifetime, particle.Time, out var color );
										particle.Color *= color;
									}
								}
							}

							//custom modules
							{
								var items = emitter.CustomModules;
								for( int n = 0; n < items.Length; n++ )
									items[ n ].PerformUpdateAfter( this, delta, ref particle );
							}

							wasUpdated = true;
						}

					}
				}

				if( toDelete != null )
					ObjectsRemove( toDelete );
			}

		}

		//!!!!вызывать
		void BatchDestroy()
		{
			//if( batch != null )
			//{
			//	batch.BatchingInstanceBufferMesh?.Dispose();
			//	batch.BatchingInstanceBufferMesh = null;
			//	batch.BatchingInstanceBufferBillboard?.Dispose();
			//	batch.BatchingInstanceBufferBillboard = null;
			//	batch = null;
			//}
		}
	}
}
