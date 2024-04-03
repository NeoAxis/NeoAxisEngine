// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
#if !DEPLOY
	[EditorControl( "NeoAxis.Editor.SpriteEditor", true )]
	[Preview( "NeoAxis.Editor.SpritePreview" )]
	[AddToResourcesWindow( @"Base\2D\Sprite", -8000 )]
#endif
	public class Sprite : MeshInSpace
	{
		/// <summary>
		/// UV texture coordinates.
		/// </summary>
		[DefaultValue( "0 1 1 0" )]
		public Reference<Rectangle> UV
		{
			get { if( _uV.BeginGet() ) UV = _uV.Get( this ); return _uV.value; }
			set { if( _uV.BeginSet( this, ref value ) ) { try { UVChanged?.Invoke( this ); } finally { _uV.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UV"/> property value changes.</summary>
		public event Action<Sprite> UVChanged;
		ReferenceField<Rectangle> _uV = new Rectangle( 0, 1, 1, 0 );

		/////////////////////////////////////////

		public override Mesh MeshOutput
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
