// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents the point of the fence in the scene.
	/// </summary>
	public class FencePoint : CurveInSpacePoint
	{
		/// <summary>
		/// Whether to enable the post.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> PostMeshEnable
		{
			get { if( _postMeshEnable.BeginGet() ) PostMeshEnable = _postMeshEnable.Get( this ); return _postMeshEnable.value; }
			set { if( _postMeshEnable.BeginSet( this, ref value ) ) { try { PostMeshEnableChanged?.Invoke( this ); DataWasChanged(); } finally { _postMeshEnable.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PostMeshEnable"/> property value changes.</summary>
		public event Action<FencePoint> PostMeshEnableChanged;
		ReferenceField<bool> _postMeshEnable = true;
		//[DefaultValue( true )]
		//public Reference<bool> PostVisible
		//{
		//	get { if( _postVisible.BeginGet() ) PostVisible = _postVisible.Get( this ); return _postVisible.value; }
		//	set { if( _postVisible.BeginSet( this, ref value ) ) { try { PostVisibleChanged?.Invoke( this ); DataWasChanged(); } finally { _postVisible.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PostVisible"/> property value changes.</summary>
		//public event Action<FencePoint> PostVisibleChanged;
		//ReferenceField<bool> _postVisible = true;

		//!!!!maybe optimize like MeshInSpace
		/// <summary>
		/// Replaces the post mesh for the point.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Mesh> PostMeshReplace
		{
			get { if( _postMeshReplace.BeginGet() ) PostMeshReplace = _postMeshReplace.Get( this ); return _postMeshReplace.value; }
			set { if( _postMeshReplace.BeginSet( this, ref value ) ) { try { PostMeshReplaceChanged?.Invoke( this ); DataWasChanged(); } finally { _postMeshReplace.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PostMeshReplace"/> property value changes.</summary>
		public event Action<FencePoint> PostMeshReplaceChanged;
		ReferenceField<Mesh> _postMeshReplace = null;

		/// <summary>
		/// Whether to enable the panel between current and next point.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> PanelMeshEnable
		{
			get { if( _panelMeshEnable.BeginGet() ) PanelMeshEnable = _panelMeshEnable.Get( this ); return _panelMeshEnable.value; }
			set { if( _panelMeshEnable.BeginSet( this, ref value ) ) { try { PanelMeshEnableChanged?.Invoke( this ); DataWasChanged(); } finally { _panelMeshEnable.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PanelMeshEnable"/> property value changes.</summary>
		public event Action<FencePoint> PanelMeshEnableChanged;
		ReferenceField<bool> _panelMeshEnable = true;
		//[DefaultValue( true )]
		//public Reference<bool> PanelVisible
		//{
		//	get { if( _panelVisible.BeginGet() ) PanelVisible = _panelVisible.Get( this ); return _panelVisible.value; }
		//	set { if( _panelVisible.BeginSet( this, ref value ) ) { try { PanelVisibleChanged?.Invoke( this ); DataWasChanged(); } finally { _panelVisible.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PanelVisible"/> property value changes.</summary>
		//public event Action<FencePoint> PanelVisibleChanged;
		//ReferenceField<bool> _panelVisible = true;

		/// <summary>
		/// Replaces the panel mesh between current and next point.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Mesh> PanelMeshReplace
		{
			get { if( _panelMeshReplace.BeginGet() ) PanelMeshReplace = _panelMeshReplace.Get( this ); return _panelMeshReplace.value; }
			set { if( _panelMeshReplace.BeginSet( this, ref value ) ) { try { PanelMeshReplaceChanged?.Invoke( this ); DataWasChanged(); } finally { _panelMeshReplace.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PanelMeshReplace"/> property value changes.</summary>
		public event Action<FencePoint> PanelMeshReplaceChanged;
		ReferenceField<Mesh> _panelMeshReplace = null;


		//public enum SpecialtyEnum
		//{
		//	None,
		//	NoPost,
		//}

		///// <summary>
		///// Whether to add special mesh to the point.
		///// </summary>
		//[DefaultValue( SpecialtyEnum.None )]
		//public Reference<SpecialtyEnum> Specialty
		//{
		//	get { if( _specialty.BeginGet() ) Specialty = _specialty.Get( this ); return _specialty.value; }
		//	set { if( _specialty.BeginSet( this, ref value ) ) { try { SpecialtyChanged?.Invoke( this ); DataWasChanged(); } finally { _specialty.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Specialty"/> property value changes.</summary>
		//public event Action<FencePoint> SpecialtyChanged;
		//ReferenceField<SpecialtyEnum> _specialty = SpecialtyEnum.None;
	}
}
