// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents an element for configuring <see cref="Surface"/>.
	/// </summary>
	public abstract class SurfaceElement : Component
	{
		/// <summary>
		/// The probability of choosing this element from others when painting.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 10.0, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> Probability
		{
			get { if( _probability.BeginGet() ) Probability = _probability.Get( this ); return _probability.value; }
			set { if( _probability.BeginSet( this, ref value ) ) { try { ProbabilityChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _probability.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Probability"/> property value changes.</summary>
		public event Action<SurfaceElement> ProbabilityChanged;
		ReferenceField<double> _probability = 1.0;

		///////////////////////////////////////////////

		[Browsable( false )]
		public SurfaceGroupOfElements ParentSurfaceGroupOfElements
		{
			get { return Parent as SurfaceGroupOfElements; }
		}

		public void ShouldRecompileSurface()
		{
			ParentSurfaceGroupOfElements?.ShouldRecompileSurface();
		}

		protected override void OnAddedToParent()
		{
			base.OnAddedToParent();

			ShouldRecompileSurface();
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			base.OnRemovedFromParent( oldParent );

			var group = oldParent as SurfaceGroupOfElements;
			group?.ShouldRecompileSurface();
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			ShouldRecompileSurface();
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Represents a mesh element for configuring <see cref="Surface"/>.
	/// </summary>
	public class SurfaceElement_Mesh : SurfaceElement
	{
		/// <summary>
		/// The mesh used by the element.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( this, ref value ) ) { try { MeshChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<SurfaceElement_Mesh> MeshChanged;
		ReferenceField<Mesh> _mesh = null;

		/// <summary>
		/// Replaces all geometries of the mesh by another material.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		public Reference<Material> ReplaceMaterial
		{
			get { if( _replaceMaterial.BeginGet() ) ReplaceMaterial = _replaceMaterial.Get( this ); return _replaceMaterial.value; }
			set { if( _replaceMaterial.BeginSet( this, ref value ) ) { try { ReplaceMaterialChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _replaceMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReplaceMaterial"/> property value changes.</summary>
		public event Action<SurfaceElement_Mesh> ReplaceMaterialChanged;
		ReferenceField<Material> _replaceMaterial;

		/// <summary>
		/// The factor of maximum visibility distance. The maximum distance is calculated based on the size of the object on the screen.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> VisibilityDistanceFactor
		{
			get { if( _visibilityDistanceFactor.BeginGet() ) VisibilityDistanceFactor = _visibilityDistanceFactor.Get( this ); return _visibilityDistanceFactor.value; }
			set { if( _visibilityDistanceFactor.BeginSet( this, ref value ) ) { try { VisibilityDistanceFactorChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _visibilityDistanceFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<SurfaceElement_Mesh> VisibilityDistanceFactorChanged;
		ReferenceField<double> _visibilityDistanceFactor = 1.0;

		///// <summary>
		///// Maximum visibility range of the object.
		///// </summary>
		//[DefaultValue( 10000.0 )]
		//[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> VisibilityDistance
		//{
		//	get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
		//	set { if( _visibilityDistance.BeginSet( this, ref value ) ) { try { VisibilityDistanceChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _visibilityDistance.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		//public event Action<SurfaceElement_Mesh> VisibilityDistanceChanged;
		//ReferenceField<double> _visibilityDistance = 10000.0;

		/// <summary>
		/// Whether to cast shadows on the other surfaces.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> CastShadows
		{
			get { if( _castShadows.BeginGet() ) CastShadows = _castShadows.Get( this ); return _castShadows.value; }
			set { if( _castShadows.BeginSet( this, ref value ) ) { try { CastShadowsChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _castShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<SurfaceElement_Mesh> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// Whether it is possible to apply decals on the surface.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> ReceiveDecals
		{
			get { if( _receiveDecals.BeginGet() ) ReceiveDecals = _receiveDecals.Get( this ); return _receiveDecals.value; }
			set { if( _receiveDecals.BeginSet( this, ref value ) ) { try { ReceiveDecalsChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _receiveDecals.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<SurfaceElement_Mesh> ReceiveDecalsChanged;
		ReferenceField<bool> _receiveDecals = true;

		/// <summary>
		/// The multiplier of the motion blur effect.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		public Reference<double> MotionBlurFactor
		{
			get { if( _motionBlurFactor.BeginGet() ) MotionBlurFactor = _motionBlurFactor.Get( this ); return _motionBlurFactor.value; }
			set { if( _motionBlurFactor.BeginSet( this, ref value ) ) { try { MotionBlurFactorChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _motionBlurFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MotionBlurFactor"/> property value changes.</summary>
		public event Action<SurfaceElement_Mesh> MotionBlurFactorChanged;
		ReferenceField<double> _motionBlurFactor = 1.0;

		/// <summary>
		/// Whether to enable the static shadows optimization.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> StaticShadows
		{
			get { if( _staticShadows.BeginGet() ) StaticShadows = _staticShadows.Get( this ); return _staticShadows.value; }
			set { if( _staticShadows.BeginSet( this, ref value ) ) { try { StaticShadowsChanged?.Invoke( this ); } finally { _staticShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="StaticShadows"/> property value changes.</summary>
		public event Action<SurfaceElement_Mesh> StaticShadowsChanged;
		ReferenceField<bool> _staticShadows = true;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!Billboard
}
