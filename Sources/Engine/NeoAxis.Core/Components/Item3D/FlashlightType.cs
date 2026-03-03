// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// A definition of flashlight item.
	/// </summary>
	[NewObjectDefaultName( "Flashlight" )]
	[AddToResourcesWindow( @"Addons\Item 3D\Flashlight Type", 540 )]
	public class FlashlightType : Item3DType
	{
		/// <summary>
		/// The material when not active.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Material> InactiveMaterial
		{
			get { if( _inactiveMaterial.BeginGet() ) InactiveMaterial = _inactiveMaterial.Get( this ); return _inactiveMaterial.value; }
			set { if( _inactiveMaterial.BeginSet( this, ref value ) ) { try { InactiveMaterialChanged?.Invoke( this ); } finally { _inactiveMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InactiveMaterial"/> property value changes.</summary>
		public event Action<FlashlightType> InactiveMaterialChanged;
		ReferenceField<Material> _inactiveMaterial = null;
	}
}
