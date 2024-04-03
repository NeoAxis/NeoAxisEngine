// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A class for grouping components of the scene.
	/// </summary>
	public class Layer : Component, IVisibleInHierarchy, ICanBeSelectedInHierarchy
	{
		IVisibleInHierarchy parentIVisibleInHierarchy;

		/////////////////////////////////////////

		/// <summary>
		/// Whether the object and its children are visible.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		public Reference<bool> Visible
		{
			get { if( _visible.BeginGet() ) Visible = _visible.Get( this ); return _visible.value; }
			set
			{
				if( _visible.BeginSet( this, ref value ) )
				{
					try
					{
						VisibleChanged?.Invoke( this );

						//OnVisibleChanged();

						//_UpdateVisibleInHierarchy( false );

						//transform = new Reference<Transform>( new Transform( visible, rotation, scale ), transform.GetByReference );
						//VisibleChanged?.Invoke( this );
						//TransformChanged?.Invoke( this );
					}
					finally { _visible.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Visible"/> property value changes.</summary>
		public event Action<Layer> VisibleChanged;
		ReferenceField<bool> _visible = true;

		[Browsable( false )]
		public bool VisibleInHierarchy
		{
			get
			{
				if( !Visible )
					return false;

				if( parentIVisibleInHierarchy != null )
					return parentIVisibleInHierarchy.VisibleInHierarchy;
				else
					return true;

				//if( !Visible )
				//	return false;

				//var p = Parent as IVisibleInHierarchy;
				//if( p != null )
				//	return p.VisibleInHierarchy;
				//else
				//	return true;
			}
		}

		/// <summary>
		/// Whether the object and its children are selectable in the editor.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		public Reference<bool> CanBeSelected
		{
			get { if( _canBeSelected.BeginGet() ) CanBeSelected = _canBeSelected.Get( this ); return _canBeSelected.value; }
			set { if( _canBeSelected.BeginSet( this, ref value ) ) { try { CanBeSelectedChanged?.Invoke( this ); } finally { _canBeSelected.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CanBeSelected"/> property value changes.</summary>
		public event Action<Layer> CanBeSelectedChanged;
		ReferenceField<bool> _canBeSelected = true;

		/// <summary>
		/// Whether to call Update methods and event for children components in the editor. The parameter is used to optimize performance.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> ChildrenUpdateInEditor
		{
			get { if( _childrenUpdateInEditor.BeginGet() ) ChildrenUpdateInEditor = _childrenUpdateInEditor.Get( this ); return _childrenUpdateInEditor.value; }
			set { if( _childrenUpdateInEditor.BeginSet( this, ref value ) ) { try { ChildrenUpdateInEditorChanged?.Invoke( this ); } finally { _childrenUpdateInEditor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ChildrenUpdateInEditor"/> property value changes.</summary>
		public event Action<Layer> ChildrenUpdateInEditorChanged;
		ReferenceField<bool> _childrenUpdateInEditor = true;

		/// <summary>
		/// Whether to call Update methods and event for children components in the simulation. The parameter is used to optimize performance.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> ChildrenUpdateInSimulation
		{
			get { if( _childrenUpdateInSimulation.BeginGet() ) ChildrenUpdateInSimulation = _childrenUpdateInSimulation.Get( this ); return _childrenUpdateInSimulation.value; }
			set { if( _childrenUpdateInSimulation.BeginSet( this, ref value ) ) { try { ChildrenUpdateInSimulationChanged?.Invoke( this ); } finally { _childrenUpdateInSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ChildrenUpdateInSimulation"/> property value changes.</summary>
		public event Action<Layer> ChildrenUpdateInSimulationChanged;
		ReferenceField<bool> _childrenUpdateInSimulation = true;

		/// <summary>
		/// Whether to call SimulationStep methods and event for children components. The parameter is used to optimize performance.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> ChildrenStepSimulation
		{
			get { if( _childrenStepSimulation.BeginGet() ) ChildrenStepSimulation = _childrenStepSimulation.Get( this ); return _childrenStepSimulation.value; }
			set { if( _childrenStepSimulation.BeginSet( this, ref value ) ) { try { ChildrenStepSimulationChanged?.Invoke( this ); } finally { _childrenStepSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ChildrenStepSimulation"/> property value changes.</summary>
		public event Action<Layer> ChildrenStepSimulationChanged;
		ReferenceField<bool> _childrenStepSimulation = true;

		[Browsable( false )]
		public bool CanBeSelectedInHierarchy
		{
			get
			{
				if( !CanBeSelected )
					return false;

				var p = Parent as ICanBeSelectedInHierarchy;
				if( p != null )
					return p.CanBeSelectedInHierarchy;
				else
					return true;
			}
		}

		protected override void OnAddedToParent()
		{
			parentIVisibleInHierarchy = Parent as IVisibleInHierarchy;

			base.OnAddedToParent();
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			parentIVisibleInHierarchy = null;

			base.OnRemovedFromParent( oldParent );
		}

		internal override void OnUpdateBefore( float delta, ref bool childrenUpdate )
		{
			base.OnUpdateBefore( delta, ref childrenUpdate );

			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
			{
				if( !ChildrenUpdateInEditor )
					childrenUpdate = false;
			}
			else
			{
				if( !ChildrenUpdateInSimulation )
					childrenUpdate = false;
			}
		}

		internal override void OnSimulationStepBefore( ref bool childrenSimulationStep )
		{
			base.OnSimulationStepBefore( ref childrenSimulationStep );

			if( !ChildrenStepSimulation )
				childrenSimulationStep = false;
		}
	}
}
