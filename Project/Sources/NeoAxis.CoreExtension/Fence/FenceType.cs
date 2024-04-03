// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Defines a fence type.
	/// </summary>
	[ResourceFileExtension( "fencetype" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Fence\Fence Type", 500 )]
	[EditorControl( typeof( FenceTypeEditor ) )]
	[Preview( typeof( FenceTypePreview ) )]
	[PreviewImage( typeof( FenceTypePreviewImage ) )]
#endif
	public class FenceType : Component
	{
		/// <summary>
		/// The mesh of the end post.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Post" )]
		public Reference<Mesh> EndPost
		{
			get { if( _endPost.BeginGet() ) EndPost = _endPost.Get( this ); return _endPost.value; }
			set { if( _endPost.BeginSet( this, ref value ) ) { try { EndPostChanged?.Invoke( this ); DataWasChanged(); } finally { _endPost.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EndPost"/> property value changes.</summary>
		public event Action<FenceType> EndPostChanged;
		ReferenceField<Mesh> _endPost = null;

		/// <summary>
		/// The mesh of the line post.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Post" )]
		public Reference<Mesh> LinePost
		{
			get { if( _linePost.BeginGet() ) LinePost = _linePost.Get( this ); return _linePost.value; }
			set { if( _linePost.BeginSet( this, ref value ) ) { try { LinePostChanged?.Invoke( this ); DataWasChanged(); } finally { _linePost.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinePost"/> property value changes.</summary>
		public event Action<FenceType> LinePostChanged;
		ReferenceField<Mesh> _linePost = null;

		/// <summary>
		/// The mesh of the corner post.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Post" )]
		public Reference<Mesh> CornerPost
		{
			get { if( _cornerPost.BeginGet() ) CornerPost = _cornerPost.Get( this ); return _cornerPost.value; }
			set { if( _cornerPost.BeginSet( this, ref value ) ) { try { CornerPostChanged?.Invoke( this ); DataWasChanged(); } finally { _cornerPost.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CornerPost"/> property value changes.</summary>
		public event Action<FenceType> CornerPostChanged;
		ReferenceField<Mesh> _cornerPost = null;

		/// <summary>
		/// The mesh of the step post.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Post" )]
		public Reference<Mesh> StepPost
		{
			get { if( _stepPost.BeginGet() ) StepPost = _stepPost.Get( this ); return _stepPost.value; }
			set { if( _stepPost.BeginSet( this, ref value ) ) { try { StepPostChanged?.Invoke( this ); DataWasChanged(); } finally { _stepPost.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="StepPost"/> property value changes.</summary>
		public event Action<FenceType> StepPostChanged;
		ReferenceField<Mesh> _stepPost = null;

		/// <summary>
		/// The replacement material of the post.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Post" )]
		public Reference<Material> PostReplaceMaterial
		{
			get { if( _postReplaceMaterial.BeginGet() ) PostReplaceMaterial = _postReplaceMaterial.Get( this ); return _postReplaceMaterial.value; }
			set { if( _postReplaceMaterial.BeginSet( this, ref value ) ) { try { PostReplaceMaterialChanged?.Invoke( this ); DataWasChanged(); } finally { _postReplaceMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PostReplaceMaterial"/> property value changes.</summary>
		public event Action<FenceType> PostReplaceMaterialChanged;
		ReferenceField<Material> _postReplaceMaterial = null;

		//[DefaultValue( null )]
		//[Category( "Post" )]
		//public Reference<Mesh> ArbitraryAnglePost
		//{
		//	get { if( _arbitraryAnglePost.BeginGet() ) ArbitraryAnglePost = _arbitraryAnglePost.Get( this ); return _arbitraryAnglePost.value; }
		//	set { if( _arbitraryAnglePost.BeginSet( this, ref value ) ) { try { ArbitraryAnglePostChanged?.Invoke( this ); DataWasChanged();} finally { _arbitraryAnglePost.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ArbitraryAnglePost"/> property value changes.</summary>
		//public event Action<Fence> ArbitraryAnglePostChanged;
		//ReferenceField<Mesh> _arbitraryAnglePost = null;

		//[DefaultValue( null )]
		//[Category( "Post" )]
		//[DisplayName( "Corner Post 90" )]
		//public Reference<Mesh> CornerPost90
		//{
		//	get { if( _cornerPost90.BeginGet() ) CornerPost90 = _cornerPost90.Get( this ); return _cornerPost90.value; }
		//	set { if( _cornerPost90.BeginSet( this, ref value ) ) { try { CornerPost90Changed?.Invoke( this );DataWasChanged(); } finally { _cornerPost90.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CornerPost90"/> property value changes.</summary>
		//public event Action<Fence> CornerPost90Changed;
		//ReferenceField<Mesh> _cornerPost90 = null;

		//[DefaultValue( null )]
		//[Category( "Post" )]
		//public Reference<Mesh> TeePost
		//{
		//	get { if( _teePost.BeginGet() ) TeePost = _teePost.Get( this ); return _teePost.value; }
		//	set { if( _teePost.BeginSet( this, ref value ) ) { try { TeePostChanged?.Invoke( this );DataWasChanged(); } finally { _teePost.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TeePost"/> property value changes.</summary>
		//public event Action<Fence> TeePostChanged;
		//ReferenceField<Mesh> _teePost = null;

		//[DefaultValue( null )]
		//[Category( "Post" )]
		//public Reference<Mesh> CrossPost
		//{
		//	get { if( _crossPost.BeginGet() ) CrossPost = _crossPost.Get( this ); return _crossPost.value; }
		//	set { if( _crossPost.BeginSet( this, ref value ) ) { try { CrossPostChanged?.Invoke( this ); DataWasChanged();} finally { _crossPost.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CrossPost"/> property value changes.</summary>
		//public event Action<Fence> CrossPostChanged;
		//ReferenceField<Mesh> _crossPost = null;

		//

		/// <summary>
		/// The length of the panel.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Panel" )]
		public Reference<double> PanelLength
		{
			get { if( _panelLength.BeginGet() ) PanelLength = _panelLength.Get( this ); return _panelLength.value; }
			set { if( _panelLength.BeginSet( this, ref value ) ) { try { PanelLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _panelLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PanelLength"/> property value changes.</summary>
		public event Action<FenceType> PanelLengthChanged;
		ReferenceField<double> _panelLength = 1.0;

		/// <summary>
		/// The mesh of the full panel.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Panel" )]
		public Reference<Mesh> FullPanel
		{
			get { if( _fullPanel.BeginGet() ) FullPanel = _fullPanel.Get( this ); return _fullPanel.value; }
			set { if( _fullPanel.BeginSet( this, ref value ) ) { try { FullPanelChanged?.Invoke( this ); DataWasChanged(); } finally { _fullPanel.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FullPanel"/> property value changes.</summary>
		public event Action<FenceType> FullPanelChanged;
		ReferenceField<Mesh> _fullPanel = null;

		/// <summary>
		/// The mesh of the half panel.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Panel" )]
		public Reference<Mesh> HalfPanel
		{
			get { if( _halfPanel.BeginGet() ) HalfPanel = _halfPanel.Get( this ); return _halfPanel.value; }
			set { if( _halfPanel.BeginSet( this, ref value ) ) { try { HalfPanelChanged?.Invoke( this ); DataWasChanged(); } finally { _halfPanel.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HalfPanel"/> property value changes.</summary>
		public event Action<FenceType> HalfPanelChanged;
		ReferenceField<Mesh> _halfPanel = null;

		/// <summary>
		/// The mesh of the quarter panel.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Panel" )]
		public Reference<Mesh> QuarterPanel
		{
			get { if( _quarterPanel.BeginGet() ) QuarterPanel = _quarterPanel.Get( this ); return _quarterPanel.value; }
			set { if( _quarterPanel.BeginSet( this, ref value ) ) { try { QuarterPanelChanged?.Invoke( this ); DataWasChanged(); } finally { _quarterPanel.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="QuarterPanel"/> property value changes.</summary>
		public event Action<FenceType> QuarterPanelChanged;
		ReferenceField<Mesh> _quarterPanel = null;

		/// <summary>
		/// Whether to use 3D geometry clipping by constructive solid geometry to fill spaces where 3D models cannot fit completely.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Panel" )]
		[DisplayName( "Fill Empty Space By CSG" )]
		public Reference<bool> FillEmptySpaceByCSG
		{
			get { if( _fillEmptySpaceByCSG.BeginGet() ) FillEmptySpaceByCSG = _fillEmptySpaceByCSG.Get( this ); return _fillEmptySpaceByCSG.value; }
			set { if( _fillEmptySpaceByCSG.BeginSet( this, ref value ) ) { try { FillEmptySpaceByCSGChanged?.Invoke( this ); DataWasChanged(); } finally { _fillEmptySpaceByCSG.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FillEmptySpaceByCSG"/> property value changes.</summary>
		public event Action<FenceType> FillEmptySpaceByCSGChanged;
		ReferenceField<bool> _fillEmptySpaceByCSG = true;

		/// <summary>
		/// The replacement material of the panel.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Panel" )]
		public Reference<Material> PanelReplaceMaterial
		{
			get { if( _panelReplaceMaterial.BeginGet() ) PanelReplaceMaterial = _panelReplaceMaterial.Get( this ); return _panelReplaceMaterial.value; }
			set { if( _panelReplaceMaterial.BeginSet( this, ref value ) ) { try { PanelReplaceMaterialChanged?.Invoke( this ); DataWasChanged(); } finally { _panelReplaceMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PanelReplaceMaterial"/> property value changes.</summary>
		public event Action<FenceType> PanelReplaceMaterialChanged;
		ReferenceField<Material> _panelReplaceMaterial = null;

		//

		int version;

		[Browsable( false )]
		public int Version
		{
			get { return version; }
		}

		public void DataWasChanged()
		{
			unchecked
			{
				version++;
			}
		}
	}
}
