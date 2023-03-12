// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Mesh modifier for calculating texture coordinates in the form of a box.
	/// </summary>
	[NewObjectDefaultName( "Box UV" )]
	[AddToResourcesWindow( @"Base\Scene common\Mesh modifiers\Box UV", 1 )]
	public class MeshModifier_BoxUV : MeshModifier
	{
		/// <summary>
		/// The number of UV tiles.
		/// </summary>
		//!!!!impl [Range( 0, 10 )]
		[DefaultValue( "1 1 1" )]
		public Reference<Vector3> Tiles
		{
			get { if( tiles.BeginGet() ) Tiles = tiles.Get( this ); return tiles.value; }
			set { if( tiles.BeginSet( ref value ) ) { try { TilesChanged?.Invoke( this ); ShouldRecompileMesh(); } finally { tiles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Tiles"/> property value changes.</summary>
		public event Action<MeshModifier_BoxUV> TilesChanged;
		ReferenceField<Vector3> tiles = Vector3.One;

		///// <summary>
		///// The number of UV tiles per unit of the world.
		///// </summary>
		//[DefaultValue( "1 1" )]
		//public Reference<Vector2> TilesPerUnit
		//{
		//	get { if( tilesPerUnit.BeginGet() ) TilesPerUnit = tilesPerUnit.Get( this ); return tilesPerUnit.value; }
		//	set
		//	{
		//		if( tilesPerUnit.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				TilesPerUnitChanged?.Invoke( this );
		//				ShouldRecompileMesh();
		//			}
		//			finally { tilesPerUnit.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="TilesPerUnit"/> property value changes.</summary>
		//public event Action<MeshModifier_BoxUV> TilesPerUnitChanged;
		//ReferenceField<Vector2> tilesPerUnit = Vector2.One;

		///// <summary>
		///// Increases the value of object dimensions that is used to calculate UV.
		///// </summary>
		//[DefaultValue( "0 0 0" )]
		//public Reference<Vector3> ExtendBounds
		//{
		//	get { if( _extendBounds.BeginGet() ) ExtendBounds = _extendBounds.Get( this ); return _extendBounds.value; }
		//	set
		//	{
		//		if( _extendBounds.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				ExtendBoundsChanged?.Invoke( this );
		//				ShouldRecompileMesh();
		//			}
		//			finally { _extendBounds.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="ExtendBounds"/> property value changes.</summary>
		//public event Action<MeshModifier_BoxUV> ExtendBoundsChanged;
		//ReferenceField<Vector3> _extendBounds = Vector3.Zero;

		/////////////////////////////////////////

		void ProcessVertex( ref Vector3 tiles, ref Bounds bounds, ref Vector3F position, ref Vector3F normal, out Vector2F result )
		{
			var side = Vector3I.Zero;
			{
				float max = -1;
				//if( Math.Abs( normal.X ) > max )
				{
					max = Math.Abs( normal.X );
					side = new Vector3I( normal.X >= 0 ? 1 : -1, 0, 0 );
				}
				if( Math.Abs( normal.Y ) > max )
				{
					max = Math.Abs( normal.Y );
					side = new Vector3I( 0, normal.Y >= 0 ? 1 : -1, 0 );
				}
				if( Math.Abs( normal.Z ) > max )
				{
					max = Math.Abs( normal.Z );
					side = new Vector3I( 0, 0, normal.Z >= 0 ? 1 : -1 );
				}
			}

			int axis0 = 0;
			int axis1 = 0;
			bool invert = false;
			{
				if( side.X != 0 )
				{
					axis0 = 1;
					axis1 = 2;
					invert = side.X < 0;
				}
				else if( side.Y != 0 )
				{
					axis0 = 0;
					axis1 = 2;
					invert = side.Y < 0;
				}
				else if( side.Z != 0 )
				{
					axis0 = 0;
					axis1 = 1;
					invert = side.Z < 0;
				}
			}

			{
				double s0 = 0;
				{
					var d = bounds.Maximum[ axis0 ] - bounds.Minimum[ axis0 ];
					if( d != 0 )
						s0 = ( position[ axis0 ] - bounds.Minimum[ axis0 ] ) / d;
				}

				double s1 = 0;
				{
					var d = bounds.Maximum[ axis1 ] - bounds.Minimum[ axis1 ];
					if( d != 0 )
						s1 = ( position[ axis1 ] - bounds.Minimum[ axis1 ] ) / d;
				}

				var v = new Vector2( s0 * tiles[ axis0 ], -s1 * tiles[ axis1 ] );

				if( side.X != 0 )
				{
					if( invert )
						v.X = -v.X;
				}
				else if( side.Y != 0 )
				{
					if( !invert )
						v.X = -v.X;
				}
				else if( side.Z != 0 )
				{
					if( invert )
						v.X = -v.X;
				}

				result = v.ToVector2F();
			}
		}

		protected override void OnApplyToMeshData( Mesh.CompiledData compiledData )
		{
			base.OnApplyToMeshData( compiledData );

			var bounds = Bounds.Cleared;
			{
				foreach( var oper in compiledData.MeshData.RenderOperations )
				{
					if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out var positionElement ) && positionElement.Type == VertexElementType.Float3 )
					{
						var vertexBuffer = oper.VertexBuffers[ positionElement.Source ];
						var positions = vertexBuffer.ExtractChannel<Vector3F>( positionElement.Offset );

						foreach( var p in positions )
							bounds.Add( p );
					}
				}
				//calculated bounding box is not prepared at this time. must calculate by vertices position
				//var bounds = compiledData.SpaceBounds.CalculatedBoundingBox;

				//bounds.Expand( ExtendBounds );
			}

			var tiles = Tiles.Value;

			foreach( var oper in compiledData.MeshData.RenderOperations )
			{
				if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out var positionElement ) && positionElement.Type == VertexElementType.Float3 )
				{
					if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Normal, out var normalElement ) && ( normalElement.Type == VertexElementType.Float3 || normalElement.Type == VertexElementType.Half3 ) )
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate0, out var texCoordElement ) && texCoordElement.Type == VertexElementType.Float2 )
						{
							var positions = oper.VertexBuffers[ positionElement.Source ].ExtractChannel<Vector3F>( positionElement.Offset );

							Vector3F[] normals;
							if( normalElement.Type == VertexElementType.Float3 )
								normals = oper.VertexBuffers[ normalElement.Source ].ExtractChannel<Vector3F>( normalElement.Offset );
							else
								normals = CollectionUtility.ToVector3F( oper.VertexBuffers[ normalElement.Source ].ExtractChannel<Vector3H>( normalElement.Offset ) );

							var newTexCoords = new Vector2F[ positions.Length ];
							for( int n = 0; n < newTexCoords.Length; n++ )
								ProcessVertex( ref tiles, ref bounds, ref positions[ n ], ref normals[ n ], out newTexCoords[ n ] );

							var vertexBuffer = oper.VertexBuffers[ texCoordElement.Source ];
							vertexBuffer.MakeCopyOfData();
							vertexBuffer.WriteChannel( texCoordElement.Offset, newTexCoords );
						}
					}
				}
			}
		}

#if !DEPLOY
		protected override void OnBakeIntoMesh( Editor.DocumentInstance document, Editor.UndoMultiAction undoMultiAction )
		{
			base.OnBakeIntoMesh( document, undoMultiAction );

			var mesh = (Mesh)Parent;
			var geometries = mesh.GetComponents<MeshGeometry>();

			var bounds = Bounds.Cleared;
			{
				foreach( var geometry in geometries )
				{
					var positions = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
					if( positions != null )
					{
						foreach( var p in positions )
							bounds.Add( p );
					}
				}
				//bounds.Expand( ExtendBounds );
			}

			var tiles = Tiles.Value;

			foreach( var geometry in geometries )
			{
				var positions = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
				var normals = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Normal );
				if( positions != null && normals != null )
				{
					var vertexStructure = geometry.VertexStructure.Value;
					vertexStructure.GetInfo( out var vertexSize, out _ );

					var oldValue = geometry.Vertices;
					var vertices = geometry.Vertices.Value;
					var vertexCount = vertices.Length / vertexSize;

					var newTexCoords = new Vector2F[ vertexCount ];
					for( int n = 0; n < vertexCount; n++ )
						ProcessVertex( ref tiles, ref bounds, ref positions[ n ], ref normals[ n ], out newTexCoords[ n ] );

					var newVertices = (byte[])vertices.Clone();
					if( geometry.VerticesWriteChannel( VertexElementSemantic.TextureCoordinate0, newTexCoords, newVertices ) )
					{
						//update property
						geometry.Vertices = newVertices;

						//undo
						if( undoMultiAction != null )
						{
							var property = (Metadata.Property)geometry.MetadataGetMemberBySignature( "property:Vertices" );
							var undoAction = new Editor.UndoActionPropertiesChange( new Editor.UndoActionPropertiesChange.Item( geometry, property, oldValue ) );
							undoMultiAction.AddAction( undoAction );
						}
					}
				}
			}
		}
#endif

	}
}
