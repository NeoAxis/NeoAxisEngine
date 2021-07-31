// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Component for the level of detail configuration.
	/// </summary>
	public class Component_MeshLevelOfDetail : Component
	{
		/// <summary>
		/// Mesh used for this level of detail.
		/// </summary>
		[Serialize]
		public Reference<Component_Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( ref value ) ) { try { MeshChanged?.Invoke( this ); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<Component_MeshLevelOfDetail> MeshChanged;
		ReferenceField<Component_Mesh> _mesh;

		/// <summary>
		/// Specifies the distance at which this level of detail will become active.
		/// </summary>
		[Serialize]
		//[DefaultValue( 0.0 )]
		public Reference<double> Distance
		{
			get { if( _distance.BeginGet() ) Distance = _distance.Get( this ); return _distance.value; }
			set { if( _distance.BeginSet( ref value ) ) { try { DistanceChanged?.Invoke( this ); } finally { _distance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Distance"/> property value changes.</summary>
		public event Action<Component_MeshLevelOfDetail> DistanceChanged;
		ReferenceField<double> _distance;
	}
}
