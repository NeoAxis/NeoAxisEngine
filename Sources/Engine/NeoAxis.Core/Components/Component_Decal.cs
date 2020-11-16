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
	/// Decal in the scene.
	/// </summary>
	public class Component_Decal : Component_ObjectInSpace
	{
		/// <summary>
		/// The material of a decal.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Material> Material
		{
			get
			{
				//!!!!fast exit optimization

				if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value;
			}
			set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<Component_Decal> MaterialChanged;
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
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Component_Decal> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

		public enum NormalsModeEnum
		{
			Surface,
			VectorOfDecal,
		}

		/// <summary>
		/// Decal normals calculation mode.
		/// </summary>
		[DefaultValue( NormalsModeEnum.Surface )]
		public Reference<NormalsModeEnum> NormalsMode
		{
			get { if( _normalsMode.BeginGet() ) NormalsMode = _normalsMode.Get( this ); return _normalsMode.value; }
			set { if( _normalsMode.BeginSet( ref value ) ) { try { NormalsModeChanged?.Invoke( this ); } finally { _normalsMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="NormalsMode"/> property value changes.</summary>
		public event Action<Component_Decal> NormalsModeChanged;
		ReferenceField<NormalsModeEnum> _normalsMode = NormalsModeEnum.Surface;

		//!!!!можно еще по размеру выключать

		/// <summary>
		/// Maximum visibility range of the object.
		/// </summary>
		[DefaultValue( 10000.0 )]
		[Range( 1, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 7 )]
		public Reference<double> VisibilityDistance
		{
			get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
			set { if( _visibilityDistance.BeginSet( ref value ) ) { try { VisibilityDistanceChanged?.Invoke( this ); } finally { _visibilityDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		public event Action<Component_Decal> VisibilityDistanceChanged;
		ReferenceField<double> _visibilityDistance = 10000.0;

		//!!!!
		//[DefaultValue( 10.0 )]
		//[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 2 )]
		//public Reference<double> VisibilityFadeDistance
		//{
		//	get { if( _visibilityFadeDistance.BeginGet() ) VisibilityFadeDistance = _visibilityFadeDistance.Get( this ); return _visibilityFadeDistance.value; }
		//	set { if( _visibilityFadeDistance.BeginSet( ref value ) ) { try { VisibilityFadeDistanceChanged?.Invoke( this ); } finally { _visibilityFadeDistance.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VisibilityFadeDistance"/> property value changes.</summary>
		//public event Action<Component_Decal> VisibilityFadeDistanceChanged;
		//ReferenceField<double> _visibilityFadeDistance = 10.0;

		/// <summary>
		/// Determines a decal order rendering relative to others.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( -100, 100 )]
		public Reference<double> SortOrder
		{
			get { if( _sortOrder.BeginGet() ) SortOrder = _sortOrder.Get( this ); return _sortOrder.value; }
			set { if( _sortOrder.BeginSet( ref value ) ) { try { SortOrderChanged?.Invoke( this ); } finally { _sortOrder.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SortOrder"/> property value changes.</summary>
		public event Action<Component_Decal> SortOrderChanged;
		ReferenceField<double> _sortOrder = 0.0;

		/////////////////////////////////////////

		Box GetBox()
		{
			var tr = Transform.Value;
			tr.Rotation.ToMatrix3( out var rot );
			return new Box( tr.Position, new Vector3( tr.Scale.X, tr.Scale.Y, tr.Scale.Z ) * 0.5, rot );
		}

		public void DebugDraw( Viewport viewport )
		{
			var renderer = viewport.Simple3DRenderer;
			var points = GetBox().ToPoints();

			renderer.AddArrow( points[ 0 ], points[ 1 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 1 ], points[ 2 ], -1 );
			renderer.AddArrow( points[ 3 ], points[ 2 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 3 ], points[ 0 ], -1 );

			renderer.AddArrow( points[ 4 ], points[ 5 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 5 ], points[ 6 ], -1 );
			renderer.AddArrow( points[ 7 ], points[ 6 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 7 ], points[ 4 ], -1 );

			renderer.AddLine( points[ 0 ], points[ 4 ], -1 );
			renderer.AddLine( points[ 1 ], points[ 5 ], -1 );
			renderer.AddLine( points[ 2 ], points[ 6 ], -1 );
			renderer.AddLine( points[ 3 ], points[ 7 ], -1 );
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var cameraSettings = context.Owner.CameraSettings;
				var tr = Transform.Value;

				bool skip = false;
				var maxDistance = VisibilityDistance.Value;
				if( maxDistance < cameraSettings.FarClipDistance && ( cameraSettings.Position - tr.Position ).LengthSquared() > maxDistance * maxDistance )
					skip = true;

				if( !skip )
				{
					{
						var item = new Component_RenderingPipeline.RenderSceneData.DecalItem();
						item.Creator = this;
						item.BoundingBox = SpaceBounds.CalculatedBoundingBox;
						//item.BoundingSphere = SpaceBounds.CalculatedBoundingSphere;

						item.Position = tr.Position;
						item.Rotation = tr.Rotation.ToQuaternionF();
						item.Scale = tr.Scale.ToVector3F();

						item.Material = Material;
						item.Color = Color;
						item.NormalsMode = NormalsMode.Value;
						item.SortOrder = SortOrder;

						context.FrameData.RenderSceneData.Decals.Add( ref item );
					}

					//display editor selection
					{
						var context2 = context.objectInSpaceRenderingContext;

						bool show = ( ParentScene.GetDisplayDevelopmentDataInThisApplication() && ParentScene.DisplayDecals ) || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
						if( show )
						{
							if( context2.displayDecalsCounter < context2.displayDecalsMax )
							{
								context2.displayDecalsCounter++;

								ColorValue color;
								if( context2.selectedObjects.Contains( this ) )
									color = ProjectSettings.Get.SelectedColor;
								else if( context2.canSelectObjects.Contains( this ) )
									color = ProjectSettings.Get.CanSelectColor;
								else
									color = ProjectSettings.Get.SceneShowDecalColor;

								var viewport = context.Owner;
								if( viewport.Simple3DRenderer != null )
								{
									viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
									DebugDraw( viewport );
								}
							}
						}
						//if( !show )
						//	context.disableShowingLabelForThisObject = true;
					}
				}
			}
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			newBounds = new SpaceBounds( GetBox().ToBounds() );
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() || !ParentScene.DisplayLabels )
				return false;
			//if( !ParentScene.GetShowDevelopmentDataInThisApplication() || !ParentScene.ShowDecals )
			//	return false;
			return base.OnEnabledSelectionByCursor();
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//var p = member as Metadata.Property;
			//if( p != null )
			//{
			//	switch( p.Name )
			//	{
			//	case nameof( SpotlightInnerAngle ):
			//		if( Type.Value != TypeEnum.Spotlight )
			//			skip = true;
			//		break;
			//}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			Transform = new Transform( TransformV.Position, Quaternion.LookAt( new Vector3( 0, 0, -1 ), new Vector3( 0, 1, 0 ) ) );
			Material = ReferenceUtility.MakeReference( @"Base\Components\Decal default.material" );
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Decal" );
		}
	}
}
