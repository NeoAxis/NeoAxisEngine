// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

namespace NeoAxis
{
	/// <summary>
	/// A flat object in the scene, which faces the camera.
	/// </summary>
	public class Component_Billboard : Component_ObjectInSpace
	{
		static Component_Mesh billboardMesh;

		double transformPositionByTime1_Time;
		Vector3 transformPositionByTime1_Position;
		double transformPositionByTime2_Time;
		Vector3 transformPositionByTime2_Position;

		//List<SceneLODUtility.RenderingContextItem> renderingContextItems;

		/////////////////////////////////////////

		/// <summary>
		/// The size of the billboard.
		/// </summary>
		[DefaultValue( "1 1" )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<Vector2> Size
		{
			get { if( _size.BeginGet() ) Size = _size.Get( this ); return _size.value; }
			set
			{
				if( _size.BeginSet( ref value ) )
				{
					try
					{
						SizeChanged?.Invoke( this );
						SpaceBoundsUpdate();
					}
					finally { _size.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Size"/> property value changes.</summary>
		public event Action<Component_Billboard> SizeChanged;
		ReferenceField<Vector2> _size = new Vector2( 1, 1 );

		/// <summary>
		/// The rotation of the billboard.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( -360, 360 )]
		public Reference<Degree> Rotation
		{
			get { if( _rotation.BeginGet() ) Rotation = _rotation.Get( this ); return _rotation.value; }
			set { if( _rotation.BeginSet( ref value ) ) { try { RotationChanged?.Invoke( this ); } finally { _rotation.EndSet(); } } }
		}
		public event Action<Component_Billboard> RotationChanged;
		ReferenceField<Degree> _rotation = new Degree( 0.0 );

		/// <summary>
		/// The material of the billboard.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Material> Material
		{
			get
			{
				//fast exit optimization
				var cachedReference = _material.value.Value?.GetCachedResourceReference();
				if( cachedReference != null )
				{
					if( ReferenceEquals( _material.value.GetByReference, cachedReference ) )
						return _material.value;
					if( _material.value.GetByReference == cachedReference )
					{
						_material.value.GetByReference = cachedReference;
						return _material.value;
					}
				}

				if( _material.BeginGet_WithoutFastExitOptimization() ) Material = _material.Get( this ); return _material.value;
				//if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value;
			}
			set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<Component_Billboard> MaterialChanged;
		ReferenceField<Component_Material> _material = null;

		/// <summary>
		/// The base color and opacity multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		public event Action<Component_Billboard> ColorChanged;
		ReferenceField<ColorValue> _color = new ColorValue( 1, 1, 1 );

		//[DefaultValue( 1.0 )]
		//[Range( 0, 1 )]
		//public Reference<double> Opacity
		//{
		//	get { if( _opacity.BeginGet() ) Opacity = _opacity.Get( this ); return _opacity.value; }
		//	set { if( _opacity.BeginSet( ref value ) ) { try { OpacityChanged?.Invoke( this ); } finally { _opacity.EndSet(); } } }
		//}
		//public event Action<Component_Billboard> OpacityChanged;
		//ReferenceField<double> _opacity = 1.0;

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
		public event Action<Component_Billboard> VisibilityDistanceChanged;
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
		public event Action<Component_Billboard> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// Indent multiplier when rendering shadows to fix overlapping effect of the object with the shadow.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		public Reference<double> ShadowOffset
		{
			get { if( _shadowOffset.BeginGet() ) ShadowOffset = _shadowOffset.Get( this ); return _shadowOffset.value; }
			set { if( _shadowOffset.BeginSet( ref value ) ) { try { ShadowOffsetChanged?.Invoke( this ); } finally { _shadowOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowOffset"/> property value changes.</summary>
		public event Action<Component_Billboard> ShadowOffsetChanged;
		ReferenceField<double> _shadowOffset = 1.0;

		/// <summary>
		/// Whether to can apply decals the billboard.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> ReceiveDecals
		{
			get { if( _receiveDecals.BeginGet() ) ReceiveDecals = _receiveDecals.Get( this ); return _receiveDecals.value; }
			set { if( _receiveDecals.BeginSet( ref value ) ) { try { ReceiveDecalsChanged?.Invoke( this ); } finally { _receiveDecals.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<Component_Billboard> ReceiveDecalsChanged;
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
		public event Action<Component_Billboard> SpecialEffectsChanged;
		ReferenceField<List<ObjectSpecialRenderingEffect>> _specialEffects = new List<ObjectSpecialRenderingEffect>();

		[Browsable( false )]
		public Component_RenderingPipeline.RenderSceneData.CutVolumeItem[] CutVolumes { get; set; }

		/////////////////////////////////////////

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var tr = Transform.Value;
			var halfSize = Size.Value * tr.Scale.MaxComponent() * 0.5;
			var r = Math.Sqrt( halfSize.X * halfSize.X + halfSize.Y * halfSize.Y );

			var bounds = new Bounds( tr.Position );
			bounds.Expand( r );
			newBounds = new SpaceBounds( bounds, new Sphere( tr.Position, r ) );
		}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			context.thisObjectWasChecked = true;

			if( SpaceBounds.CalculatedBoundingSphere.Intersects( context.ray, out var scale1, out var scale2 ) )
			{
				var m = GetBillboardMesh();

				if( m.Result.ExtractedIndices.Length != 0 )
				{
					Quaternion meshFaceRotation = Quaternion.Identity;
					switch( m.Result.MeshData.BillboardMode )
					{
					case 1: meshFaceRotation = Quaternion.FromDirectionZAxisUp( Vector3.Zero - new Vector3( 0, -1, 0 ) ); break;
					case 2: meshFaceRotation = Quaternion.FromDirectionZAxisUp( Vector3.Zero - new Vector3( 0, 1, 0 ) ); break;
					case 3: meshFaceRotation = Quaternion.FromDirectionZAxisUp( Vector3.Zero - new Vector3( 1, 0, 0 ) ); break;
					case 4: meshFaceRotation = Quaternion.FromDirectionZAxisUp( Vector3.Zero - new Vector3( -1, 0, 0 ) ); break;
					}

					var rotation = context.viewport.CameraSettings.Rotation * meshFaceRotation.GetInverse();
					var tr = TransformV;
					var size2 = new Vector2( Size.Value.X * Math.Max( tr.Scale.X, tr.Scale.Y ), Size.Value.Y * tr.Scale.Z );
					var tranform = new Matrix4( rotation.ToMatrix3() * Matrix3.FromScale( new Vector3( size2.X, size2.X, size2.Y ) ), tr.Position );

					var vertices = m.Result.ExtractedVerticesPositions;
					var indices = m.Result.ExtractedIndices;
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

			if( mode == GetRenderSceneDataMode.InsideFrustum || ( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && CastShadows ) )
			{
				var size = Size.Value;
				//var color = Color.Value;
				//var opacity = Opacity.Value;
				//if( size != Vector2.Zero )//&& color.Alpha != 0 && opacity != 0 )
				//{

				var context2 = context.objectInSpaceRenderingContext;
				context2.disableShowingLabelForThisObject = true;

				var tr = Transform.Value;
				var cameraSettings = context.Owner.CameraSettings;

				var cameraDistanceMinSquared = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, SpaceBounds );
				var visibilityDistance = VisibilityDistance.Value;

				if( cameraDistanceMinSquared < visibilityDistance * visibilityDistance || mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum )
				{
					var allowOutlineSelect = context.renderingPipeline.UseRenderTargets && ProjectSettings.Get.SceneEditorSelectOutlineEffectEnabled;

					var item = new Component_RenderingPipeline.RenderSceneData.BillboardItem();
					item.Creator = this;
					SpaceBounds.CalculatedBoundingBox.GetCenter( out item.BoundingBoxCenter );
					item.BoundingSphere = SpaceBounds.CalculatedBoundingSphere;
					item.CastShadows = CastShadows;
					item.ShadowOffset = (float)ShadowOffset.Value;
					item.ReceiveDecals = ReceiveDecals;
					item.Material = Material;
					item.VisibilityDistance = (float)visibilityDistance;
					item.CutVolumes = CutVolumes;

					var specialEffects = SpecialEffects.Value;
					if( specialEffects != null && specialEffects.Count != 0 )
						item.SpecialEffects = specialEffects;

					//display outline effect of editor selection
					if( mode == GetRenderSceneDataMode.InsideFrustum && allowOutlineSelect && context2.selectedObjects.Contains( this ) )
					{
						if( context2.displayBillboardsCounter < context2.displayBillboardsMax )
						{
							context2.displayBillboardsCounter++;

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
					}

					//!!!!double
					item.Position = tr.Position.ToVector3F();
					item.Size = new Vector2( size.X * Math.Max( tr.Scale.X, tr.Scale.Y ), size.Y * tr.Scale.Z ).ToVector2F();
					item.Rotation = Rotation.Value.InRadians().ToRadianF();
					item.Color = Color;

					//PositionPreviousFrame
					var previousTime = time - context.Owner.LastUpdateTimeStep;
					if( !GetTransformPositionByTime( previousTime, out var previousPosition ) )
						previousPosition = tr.Position;
					item.PositionPreviousFrame = previousPosition.ToVector3F();

					context.FrameData.RenderSceneData.Billboards.Add( ref item );


					//display editor selection
					if( mode == GetRenderSceneDataMode.InsideFrustum )
					{
						if( ( !allowOutlineSelect && context2.selectedObjects.Contains( this ) ) || context2.canSelectObjects.Contains( this ) )// || context.dragDropCreateObject == this )
						{
							if( context2.displayBillboardsCounter < context2.displayBillboardsMax )
							{
								context2.displayBillboardsCounter++;

								ColorValue color;
								if( context2.selectedObjects.Contains( this ) )
									color = ProjectSettings.Get.SelectedColor;
								else
									color = ProjectSettings.Get.CanSelectColor;
								color.Alpha *= .5f;

								var viewport = context.Owner;
								if( viewport.Simple3DRenderer != null )
								{
									viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

									item.GetWorldMatrix( out var worldMatrix );

									//!!!!double
									worldMatrix.ToMatrix4( out var worldMatrix2 );
									viewport.Simple3DRenderer.AddMesh( GetBillboardMesh().Result, ref worldMatrix2, false, false );
								}
							}
						}
					}
				}

				//}
			}
		}

		public static Component_Mesh GetBillboardMesh()
		{
			if( billboardMesh == null || billboardMesh.Disposed )
				billboardMesh = null;

			if( billboardMesh == null )
			{
				var mesh = ComponentUtility.CreateComponent<Component_Mesh>( null, true, false );
				var geometry = mesh.CreateComponent<Component_MeshGeometry_Plane>();
				geometry.Axis = 1;
				mesh.Billboard = true;
				mesh.Enabled = true;

				billboardMesh = mesh;
			}

			return billboardMesh;
		}

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

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			Material = ReferenceUtility.MakeReference( @"Base\Components\Billboard default.material" );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( ShadowOffset ):
					if( !CastShadows )
						skip = true;
					break;
				}
			}
		}
	}
}
