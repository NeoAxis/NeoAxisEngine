// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	/// A class for grouping components of the scene.
	/// </summary>
	public class Component_Layer : Component, IComponent_VisibleInHierarchy, IComponent_CanBeSelectedInHierarchy
	{
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
				if( _visible.BeginSet( ref value ) )
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
		public event Action<Component_Layer> VisibleChanged;
		ReferenceField<bool> _visible = true;

		[Browsable( false )]
		public bool VisibleInHierarchy
		{
			get
			{
				//!!!!slowly

				if( !Visible )
					return false;

				var p = Parent as IComponent_VisibleInHierarchy;
				if( p != null )
					return p.VisibleInHierarchy;
				else
					return true;

				//return visibleInHierarchy;
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
			set { if( _canBeSelected.BeginSet( ref value ) ) { try { CanBeSelectedChanged?.Invoke( this ); } finally { _canBeSelected.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CanBeSelected"/> property value changes.</summary>
		public event Action<Component_Layer> CanBeSelectedChanged;
		ReferenceField<bool> _canBeSelected = true;

		[Browsable( false )]
		public bool CanBeSelectedInHierarchy
		{
			get
			{
				if( !CanBeSelected )
					return false;

				var p = Parent as IComponent_CanBeSelectedInHierarchy;
				if( p != null )
					return p.CanBeSelectedInHierarchy;
				else
					return true;
			}
		}
	}
}
