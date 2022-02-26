// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// An element of the voxelized data of the mesh.
	/// </summary>
	public class MeshVoxelizedElement : Component
	{
		//!!!!рекомпилить меш?

		/// <summary>
		/// The material of the element.
		/// </summary>
		[Cloneable( CloneType.Shallow )]
		public Reference<Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<MeshVoxelizedElement> MaterialChanged;
		ReferenceField<Material> _material;
	}
}
