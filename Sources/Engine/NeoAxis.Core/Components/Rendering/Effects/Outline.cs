// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Outline screen effect for objects.
	/// </summary>
	[DefaultOrderOfEffect( 14 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_RenderingEffect_Outline : Component_RenderingEffect
	{
		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Effect" )]
		public Reference<double> Intensity
		{
			get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
			set { if( _intensity.BeginSet( ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Outline> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// Size multiplier.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Scale
		{
			get { if( _scale.BeginGet() ) Scale = _scale.Get( this ); return _scale.value; }
			set { if( _scale.BeginSet( ref value ) ) { try { ScaleChanged?.Invoke( this ); } finally { _scale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Scale"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Outline> ScaleChanged;
		ReferenceField<double> _scale = 1.0;

		/// <summary>
		/// The interval of affected groups of objects to use by the effect.
		/// </summary>
		[DefaultValue( "0 100000" )]
		public Reference<RangeI> GroupsInterval
		{
			get { if( _groupsInterval.BeginGet() ) GroupsInterval = _groupsInterval.Get( this ); return _groupsInterval.value; }
			set { if( _groupsInterval.BeginSet( ref value ) ) { try { GroupsIntervalChanged?.Invoke( this ); } finally { _groupsInterval.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GroupsInterval"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Outline> GroupsIntervalChanged;
		ReferenceField<RangeI> _groupsInterval = new RangeI( 0, 100000 );

		/////////////////////////////////////////

		unsafe void RenderObjects( ViewportRenderingContext context, Component_RenderingPipeline_Basic.FrameData frameData, ref Component_Image actualTexture, double scale, ColorValue color, Vector4 anyData, List<Vector2I> renderableItems )
		{
			var pipeline = context.RenderingPipeline as Component_RenderingPipeline_Basic;
			var owner = context.Owner;


			float scaleFactor = 1.0f / 200.0f;

			float scaleV = scaleFactor * (float)scale * (float)Scale.Value;
			var aspectRatio = (float)context.CurrentViewport.SizeInPixels.X / (float)context.CurrentViewport.SizeInPixels.Y;
			var effectScale = new Vector2F( scaleV / aspectRatio, scaleV );
			var effectAnyData = anyData.ToVector4F();

			var scaleSizeInPixels = (int)( scaleV * (float)context.CurrentViewport.SizeInPixels.Y );


			var scissor = RectangleI.Cleared;
			{
				var points = new Vector3[ 8 ];

				for( int nRenderableItem = 0; nRenderableItem < renderableItems.Count; nRenderableItem++ )
				{
					var renderableItem = renderableItems[ nRenderableItem ];

					if( renderableItem.X == 0 )
					{
						//meshes
						ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];

						meshItem.BoundingSphere.ToBounds( out var bounds );
						bounds.ToPoints( ref points );
					}
					else
					{
						//billboards
						ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];

						billboardItem.BoundingSphere.ToBounds( out var bounds );
						bounds.ToPoints( ref points );
					}

					foreach( var p in points )
					{
						owner.CameraSettings.ProjectToScreenCoordinates( p, out var screenPos, false );
						var pixel = ( screenPos * owner.SizeInPixels.ToVector2() ).ToVector2I();
						scissor.Add( pixel );
					}
				}
			}

			if( scissor.IsCleared() )
				return;

			scissor.Expand( scaleSizeInPixels );

			if( !scissor.Intersects( new RectangleI( 0, 0, owner.SizeInPixels.X, owner.SizeInPixels.Y ) ) )
				return;
			scissor = scissor.Intersection( new RectangleI( 0, 0, owner.SizeInPixels.X, owner.SizeInPixels.Y ) );


			//create mask render target

			double sizeMultiplier = 2;
			var targetSize = ( actualTexture.Result.ResultSize.ToVector2() * sizeMultiplier ).ToVector2I();

			var maskTexture = context.RenderTarget2D_Alloc( targetSize, PixelFormat.A8R8G8B8 );
			var depthTexture = context.RenderTarget2D_Alloc( targetSize, PixelFormat.Depth24S8, 0 );

			//render objects to the mask texture
			{
				MultiRenderTarget mrt;
				{
					var items = new List<MultiRenderTarget.Item>();
					items.Add( new MultiRenderTarget.Item( maskTexture ) );
					//if( pipeline.GetUseMultiRenderTargets() )
					//{
					//	items.Add( new MultiRenderTarget.Item( normalTexture ) );
					//	if( motionTexture != null )
					//		items.Add( new MultiRenderTarget.Item( motionTexture ) );
					//}
					items.Add( new MultiRenderTarget.Item( depthTexture ) );
					mrt = context.MultiRenderTarget_Create( items.ToArray() );
				}

				//!!!!double
				Matrix4F viewMatrix = owner.CameraSettings.ViewMatrix.ToMatrix4F();
				Matrix4F projectionMatrix = owner.CameraSettings.ProjectionMatrix.ToMatrix4F();

				context.SetViewport( mrt.Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.All, ColorValue.Zero );


				//use debug mode to detect pixels of the object
				pipeline.SetViewportOwnerSettingsUniform( context, Component_RenderingPipeline_Basic.DebugModeEnum.Normal );

				if( frameData.LightsInFrustumSorted.Length != 0 )
				{
					//use ambient light pass
					var lightIndex = frameData.LightsInFrustumSorted[ 0 ];
					var lightItem = frameData.Lights[ lightIndex ];

					//set lightItem uniforms
					lightItem.Bind( pipeline, context );

					//process objects
					for( int nRenderableItem = 0; nRenderableItem < renderableItems.Count; nRenderableItem++ )
					{
						var renderableItem = renderableItems[ nRenderableItem ];

						if( renderableItem.X == 0 )
						{
							//meshes
							ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
							var meshData = meshItem.MeshData;

							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
							{
								var oper = meshData.RenderOperations[ nOperation ];
								var materialData = Component_RenderingPipeline_Basic.GetMeshMaterialData( ref meshItem, oper, nOperation, true );

								if( materialData.AllPasses.Count != 0 )
								{
									//bind material data
									materialData.BindCurrentFrameData( context, false, true );

									var passesGroup = materialData.passesByLightType[ (int)Component_Light.TypeEnum.Ambient ];
									bool receiveShadows = false;
									GpuMaterialPass pass = passesGroup.passWithoutShadows;

									pipeline.ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true );

									var batchInstancing = meshItem.BatchingInstanceBuffer != null;

									pipeline.BindRenderOperationData( context, frameData, materialData, batchInstancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref meshItem.Color, oper.VertexStructureContainsColor, false, meshItem.VisibilityDistance );

									if( !batchInstancing )
										fixed( Matrix4F* p = &meshItem.Transform )
											Bgfx.SetTransform( (float*)p );

									pipeline.RenderOperation( context, oper, pass, null, meshItem.CutVolumes, meshItem.BatchingInstanceBuffer );
								}
							}
						}
						else if( renderableItem.X == 1 )
						{
							//billboards
							ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
							var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
							{
								var oper = meshData.RenderOperations[ nOperation ];
								var materialData = Component_RenderingPipeline_Basic.GetBillboardMaterialData( ref billboardItem, true );

								if( materialData.AllPasses.Count != 0 )
								{
									//bind material data
									materialData.BindCurrentFrameData( context, false, true );

									var passesGroup = materialData.passesByLightType[ (int)Component_Light.TypeEnum.Ambient ];
									bool receiveShadows = false;
									GpuMaterialPass pass = passesGroup.passWithoutShadows;

									pipeline.ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, nRenderableItem == 0 );

									pipeline.BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, oper.UnwrappedUV, ref billboardItem.Color, oper.VertexStructureContainsColor, false, billboardItem.VisibilityDistance );

									billboardItem.GetWorldMatrix( out var worldMatrix );
									Bgfx.SetTransform( (float*)&worldMatrix );

									pipeline.RenderOperation( context, oper, pass, null, billboardItem.CutVolumes, null );
								}
							}
						}
					}
				}

				//restore debug mode
				pipeline.SetViewportOwnerSettingsUniform( context );

				//clear temp render targets
				context.DynamicTexture_Free( depthTexture );
			}

			//create outline render target
			var outlineTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, PixelFormat.L8 );

			{
				context.SetViewport( outlineTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\Outline\Outline_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"maskTexture"*/, maskTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				//context.objectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
				////if( depthTexture == null )
				////{
				////}
				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"depthTexture"*/, depthTexture,
				//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				//context.objectsDuringUpdate.namedTextures.TryGetValue( "normalTexture", out var normalTexture );
				//if( normalTexture == null )
				//	normalTexture = ResourceUtility.WhiteTexture2D;
				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2/*"normalTexture"*/, normalTexture,
				//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				shader.Parameters.Set( "intensity", (float)Intensity );
				shader.Parameters.Set( "effectColor", color.ToVector4F() );
				shader.Parameters.Set( "effectScale", effectScale );
				shader.Parameters.Set( "effectAnyData", effectAnyData );

				Bgfx.SetScissor( scissor.Left, scissor.Top, scissor.Size.X, scissor.Size.Y );
				context.RenderQuadToCurrentViewport( shader );
				Bgfx.SetScissor( -1 );
			}

			//final pass. draw to actual texture by means alpha blending
			{
				context.SetViewport( actualTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\Outline\Final_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"outlineTexture"*/, outlineTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );
				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"outlineTexture"*/, outlineTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				shader.Parameters.Set( "intensity", (float)Intensity );
				shader.Parameters.Set( "effectColor", color.ToVector4F() );
				shader.Parameters.Set( "effectScale", effectScale );
				shader.Parameters.Set( "effectAnyData", effectAnyData );

				shader.Parameters.Set( "sourceSizeInv", new Vector2F( 1, 1 ) / outlineTexture.Result.ResultSize.ToVector2F() );

				Bgfx.SetScissor( scissor.Left, scissor.Top, scissor.Size.X, scissor.Size.Y );
				context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.AlphaBlend );
				Bgfx.SetScissor( -1 );
			}

			//clear temp render targets
			context.DynamicTexture_Free( maskTexture );
			context.DynamicTexture_Free( outlineTexture );
		}

		void ProcessEffect( ViewportRenderingContext context, Component_RenderingPipeline_Basic.FrameData frameData, ref Component_Image actualTexture )
		{
			var groupsInterval = GroupsInterval.Value;

			var items = new Dictionary<(int, double, ColorValue, Vector4), List<Vector2I>>();

			foreach( var renderableItem in frameData.RenderableGroupsInFrustum )
			{
				List<ObjectSpecialRenderingEffect> effects = null;
				if( renderableItem.X == 0 )
				{
					ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
					effects = meshItem.SpecialEffects;
				}
				else
				{
					ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
					effects = billboardItem.SpecialEffects;
				}

				if( effects != null )
				{
					for( int n = 0; n < effects.Count; n++ )
					{
						var effectData = effects[ n ] as ObjectSpecialRenderingEffect_Outline;
						if( effectData != null && effectData.Enabled && effectData.Scale != 0 && effectData.Color.Alpha > 0 )
						{
							if( effectData.Group >= groupsInterval.Minimum && effectData.Group <= groupsInterval.Maximum )
							{
								var key = (effectData.Group, effectData.Scale, effectData.Color, effectData.AnyData);
								if( !items.TryGetValue( key, out var list ) )
								{
									list = new List<Vector2I>();
									items.Add( key, list );
								}

								list.Add( renderableItem );
							}
						}
					}
				}
			}

			//convert to list
			var items2 = new List<(int, double, ColorValue, Vector4, List<Vector2I>)>( items.Count );
			foreach( var pair in items )
			{
				var key = pair.Key;
				items2.Add( (key.Item1, key.Item2, key.Item3, key.Item4, pair.Value) );
			}

			//sort by group index
			CollectionUtility.MergeSort( items2, delegate ( (int, double, ColorValue, Vector4, List<Vector2I>) a, (int, double, ColorValue, Vector4, List<Vector2I>) b )
			{
				if( a.Item1 < b.Item1 )
					return -1;
				if( a.Item1 > b.Item1 )
					return 1;
				return 0;
			}, true );

			//render
			foreach( var item in items2 )
				RenderObjects( context, frameData, ref actualTexture, item.Item2, item.Item3, item.Item4, item.Item5 );
		}

		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			ProcessEffect( context, (Component_RenderingPipeline_Basic.FrameData)frameData, ref actualTexture );
		}

		/////////////////////////////////////////

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "ScreenEffect", true );
		}
	}
}
