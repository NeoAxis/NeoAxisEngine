// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	//!!!!

	/// <summary>
	/// Represents a data to specify layer of <see cref="Component_Surface"/> or material on an object.
	/// </summary>
	public class Component_SurfaceLayer : Component
	{
		[DefaultValue( null )]
		public Reference<Component_Surface> Surface
		{
			get { if( _surface.BeginGet() ) Surface = _surface.Get( this ); return _surface.value; }
			set { if( _surface.BeginSet( ref value ) ) { try { SurfaceChanged?.Invoke( this ); } finally { _surface.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface"/> property value changes.</summary>
		public event Action<Component_SurfaceLayer> SurfaceChanged;
		ReferenceField<Component_Surface> _surface = null;

		[DefaultValue( null )]
		public Reference<Component_Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<Component_SurfaceLayer> MaterialChanged;
		ReferenceField<Component_Material> _material = null;

		[DefaultValue( null )]
		public Reference<byte[]> Mask
		{
			get { if( _mask.BeginGet() ) Mask = _mask.Get( this ); return _mask.value; }
			set { if( _mask.BeginSet( ref value ) ) { try { MaskChanged?.Invoke( this ); } finally { _mask.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mask"/> property value changes.</summary>
		public event Action<Component_SurfaceLayer> MaskChanged;
		ReferenceField<byte[]> _mask = null;
	}
}
