// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// A class for providing the creation of a <see cref="SurfaceArea"/> in the editor.
	/// </summary>
	public class SurfaceAreaCreationMode : AreaCreationMode
	{
		public SurfaceAreaCreationMode( DocumentWindowWithViewport documentWindow, Component creatingObject )
			: base( documentWindow, creatingObject )
		{
		}

		public new SurfaceArea CreatingObject
		{
			get { return (SurfaceArea)base.CreatingObject; }
		}

		protected override bool CalculatePointPosition( Viewport viewport, out Vector3 position, out ObjectInSpace collidedWith )
		{
			var result = base.CalculatePointPosition( viewport, out position, out collidedWith );

			if( result )
			{
				if( collidedWith != null )
				{
					Component obj = collidedWith;
					{
						var meshInSpace = collidedWith as MeshInSpace;
						if( meshInSpace != null )
						{
							var terrain = Terrain.GetTerrainByMeshInSpace( meshInSpace );
							if( terrain != null )
								obj = terrain;
						}
					}

					CreatingObject.AddBaseObject( obj );
				}
			}

			return result;
		}

		public override void Finish( bool cancel )
		{
			base.Finish( cancel );

			if( !cancel )
				CreatingObject.Surface = ReferenceUtility.MakeReference( @"Base\Surfaces\Default.surface" );
		}
	}
}
