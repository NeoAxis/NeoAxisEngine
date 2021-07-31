// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Sprite in the scene.
	/// </summary>
	[ResourceFileExtension( "sprite" )]
	[EditorDocumentWindow( typeof( Component_Sprite_DocumentWindow ), true )]
	[EditorPreviewControl( typeof( Component_Sprite_Preview ) )]
	[AddToResourcesWindow( @"Base\2D\Sprite", -8000 )]
	public class Component_Sprite : Component_MeshInSpace
	{
		/// <summary>
		/// UV texture coordinates.
		/// </summary>
		[DefaultValue( "0 1 1 0" )]
		public Reference<Rectangle> UV
		{
			get { if( _uV.BeginGet() ) UV = _uV.Get( this ); return _uV.value; }
			set { if( _uV.BeginSet( ref value ) ) { try { UVChanged?.Invoke( this ); } finally { _uV.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UV"/> property value changes.</summary>
		public event Action<Component_Sprite> UVChanged;
		ReferenceField<Rectangle> _uV = new Rectangle( 0, 1, 1, 0 );

		/////////////////////////////////////////

		public override Component_Mesh MeshOutput
		{
			get
			{
				var mesh = base.MeshOutput;
				if( mesh == null )
					mesh = SpriteMeshManager.GetMesh( UV.Value.ToRectangleF() );
				return mesh;
			}
		}
	}
}
