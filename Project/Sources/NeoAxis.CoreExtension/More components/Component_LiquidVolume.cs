// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The component to manage liquid volumes.
	/// </summary>
	public class Component_LiquidVolume : Component_ObjectInSpace
	{
		//!!!!Shape property

		/// <summary>
		/// The material of liquid surface.
		/// </summary>
		[DefaultValueReference( @"Base\Components\Liquid Volume\Water Surface.material" )]
		public Reference<Component_Material> MaterialSurface
		{
			get { if( _materialSurface.BeginGet() ) MaterialSurface = _materialSurface.Get( this ); return _materialSurface.value; }
			set { if( _materialSurface.BeginSet( ref value ) ) { try { MaterialSurfaceChanged?.Invoke( this ); MeshShouldRecompile(); } finally { _materialSurface.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialSurface"/> property value changes.</summary>
		public event Action<Component_LiquidVolume> MaterialSurfaceChanged;
		ReferenceField<Component_Material> _materialSurface = new Reference<Component_Material>( null, @"Base\Components\Liquid Volume\Water Surface.material" );

		//!!!!MaterialUnderSurface

		/// <summary>
		/// The number of vertices per unit of the world.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 5, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> TessellationPerUnit
		{
			get { if( _tessellationPerUnit.BeginGet() ) TessellationPerUnit = _tessellationPerUnit.Get( this ); return _tessellationPerUnit.value; }
			set { if( _tessellationPerUnit.BeginSet( ref value ) ) { try { TessellationPerUnitChanged?.Invoke( this ); MeshShouldRecompile(); } finally { _tessellationPerUnit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TessellationPerUnit"/> property value changes.</summary>
		public event Action<Component_LiquidVolume> TessellationPerUnitChanged;
		ReferenceField<double> _tessellationPerUnit = 0.0;

		/// <summary>
		/// The number of UV tiles per unit of the world.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[DisplayName( "UV Tiles Per Unit" )]
		public Reference<double> UVTilesPerUnit
		{
			get { if( _uVTilesPerUnit.BeginGet() ) UVTilesPerUnit = _uVTilesPerUnit.Get( this ); return _uVTilesPerUnit.value; }
			set { if( _uVTilesPerUnit.BeginSet( ref value ) ) { try { UVTilesPerUnitChanged?.Invoke( this ); MeshShouldRecompile(); } finally { _uVTilesPerUnit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVTilesPerUnit"/> property value changes.</summary>
		public event Action<Component_LiquidVolume> UVTilesPerUnitChanged;
		ReferenceField<double> _uVTilesPerUnit = 1.0;

		///////////////////////////////////////////////

		protected override void OnEnabledInHierarchyChanged()
		{
			UpdateSettings();

			base.OnEnabledInHierarchyChanged();
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			MeshShouldRecompile();
		}

		public Box GetBox()
		{
			var tr = Transform.Value;
			tr.Rotation.ToMatrix3( out var rot );
			return new Box( tr.Position, new Vector3( tr.Scale.X, tr.Scale.Y, tr.Scale.Z ) * 0.5, rot );
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			//!!!!bounding sphere

			//switch( Shape.Value )
			//{
			//case ShapeEnum.Box:
			newBounds = new SpaceBounds( GetBox().ToBounds() );
			//	break;

			//case ShapeEnum.Sphere:
			//	newBounds = new SpaceBounds( GetSphere() );
			//	break;

			//case ShapeEnum.Ray:
			//	{
			//		var ray = GetRay();
			//		Bounds b = new Bounds( ray.Origin );
			//		b.Add( ray.GetEndPoint() );
			//		newBounds = new SpaceBounds( b );
			//	}
			//	break;
			//}
		}

		protected virtual void RenderShape( RenderingContext context )
		{
			//switch( Shape.Value )
			//{
			//case ShapeEnum.Box:
			context.viewport.Simple3DRenderer.AddBox( GetBox() );
			//	break;

			//case ShapeEnum.Sphere:
			//	context.viewport.Simple3DRenderer.AddSphere( GetSphere() );
			//	break;

			//case ShapeEnum.Ray:
			//	{
			//		var ray = GetRay();
			//		context.viewport.Simple3DRenderer.AddLine( ray.Origin, ray.GetEndPoint() );
			//	}
			//	break;
			//}
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( EnabledInHierarchy && VisibleInHierarchy )
			{
				var item = new Component_RenderingPipeline.RenderSceneData.TransparentRenderingAddOffsetWhenSortByDistanceVolumeItem();
				item.Box = GetBox();
				context.FrameData.RenderSceneData.TransparentRenderingAddOffsetWhenSortByDistanceVolumes.Add( item );
			}

			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				bool show = ( ParentScene.GetDisplayDevelopmentDataInThisApplication() && ParentScene.DisplayVolumes ) || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
				if( show )
				{
					//if( context2.displayVolumesCounter < context2.displayVolumesMax )
					//{
					//	context2.displayVolumesCounter++;

					ColorValue color;
					if( context2.selectedObjects.Contains( this ) )
						color = ProjectSettings.Get.SelectedColor;
					else if( context2.canSelectObjects.Contains( this ) )
						color = ProjectSettings.Get.CanSelectColor;
					else
						color = ProjectSettings.Get.SceneShowVolumeColor;

					if( color.Alpha != 0 )
					{
						context.Owner.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
						RenderShape( context2 );
					}

					//}
				}
			}
		}

		public Component_MeshInSpace GetSurface()
		{
			return GetComponent<Component_MeshInSpace>( "Surface" );
		}

		public Component_Mesh GetSurfaceMesh()
		{
			var surface = GetSurface();
			if( surface != null )
				return surface.GetComponent<Component_Mesh>( "Mesh" );
			return null;
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			//create surface

			var meshInSpace = CreateComponent<Component_MeshInSpace>( enabled: false );
			meshInSpace.Name = "Surface";
			meshInSpace.CanBeSelected = false;

			var mesh = meshInSpace.CreateComponent<Component_Mesh>();
			mesh.Name = "Mesh";
			meshInSpace.Mesh = ReferenceUtility.MakeThisReference( meshInSpace, mesh );

			var geometry = mesh.CreateComponent<Component_MeshGeometry_Plane>();
			geometry.Name = "Mesh Geometry";
			geometry.Material = ReferenceUtility.MakeThisReference( geometry, this, "MaterialSurface" );

			var transformOffset = meshInSpace.CreateComponent<Component_TransformOffset>();
			transformOffset.Name = "Attach Transform Offset";
			transformOffset.PositionOffset = new Vector3( 0, 0, 0.5 );
			transformOffset.Source = ReferenceUtility.MakeThisReference( transformOffset, this, "Transform" );

			meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, transformOffset, "Result" );

			meshInSpace.Enabled = true;

			UpdateSettings();
		}

		void UpdateSettings()
		{
			var surface = GetSurface();
			if( surface != null )
				surface.SetTransparentRenderingAddOffsetWhenSortByDistance( true );

			var surfaceMesh = GetSurfaceMesh();
			if( surfaceMesh != null )
			{
				var geometry = surfaceMesh.GetComponent<Component_MeshGeometry_Plane>();
				if( geometry != null )
				{
					var size = GetBox().Extents.ToVector2() * 2;

					var segments = ( size * TessellationPerUnit.Value ).ToVector2I();
					if( segments.X < 1 )
						segments.X = 1;
					if( segments.Y < 1 )
						segments.Y = 1;
					geometry.Segments = segments;

					geometry.UVTilesInTotal = size * UVTilesPerUnit;
				}
			}
		}

		void MeshShouldRecompile()
		{
			UpdateSettings();

			var surfaceMesh = GetSurfaceMesh();
			if( surfaceMesh != null )
				surfaceMesh.ShouldRecompile = true;
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Volume" );
		}
	}
}
