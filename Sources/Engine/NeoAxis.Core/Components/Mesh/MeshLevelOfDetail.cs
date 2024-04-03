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
	/// <summary>
	/// Component for the level of detail configuration.
	/// </summary>
	public class MeshLevelOfDetail : Component
	{
		/// <summary>
		/// Mesh used for this level of detail.
		/// </summary>
		[Serialize]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( this, ref value ) ) { try { MeshChanged?.Invoke( this ); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<MeshLevelOfDetail> MeshChanged;
		ReferenceField<Mesh> _mesh;

		/// <summary>
		/// Specifies the distance at which this level of detail will become active. For voxel LODs is also another factor exists, voxel LOD will activate only when the size of a voxel is less than 1 pixel on the screen.
		/// </summary>
		// Specifies the distance at which this level of detail will become active. 100000 value means it is a voxel LOD, this value is not used. The distance for the voxel LOD is calculated depending on object size on the screen during the rendering.
		[Serialize]
		//[DefaultValue( 0.0 )]
		public Reference<double> Distance
		{
			get { if( _distance.BeginGet() ) Distance = _distance.Get( this ); return _distance.value; }
			set { if( _distance.BeginSet( this, ref value ) ) { try { DistanceChanged?.Invoke( this ); } finally { _distance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Distance"/> property value changes.</summary>
		public event Action<MeshLevelOfDetail> DistanceChanged;
		ReferenceField<double> _distance;
	}
}
