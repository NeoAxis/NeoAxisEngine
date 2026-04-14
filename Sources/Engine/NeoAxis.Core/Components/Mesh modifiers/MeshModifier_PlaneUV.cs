// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Mesh modifier for calculating texture coordinates in the form of a box.
	/// </summary>
	[NewObjectDefaultName( "Plane UV" )]
	[AddToResourcesWindow( @"Base\Scene common\Mesh modifiers\Plane UV", 1 )]
	public class MeshModifier_PlaneUV : MeshModifier
	{
		/// <summary>
		/// The number of UV tiles.
		/// </summary>
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DefaultValue( "1 1" )]
		public Reference<Vector2> Tiles
		{
			get { if( tiles.BeginGet() ) Tiles = tiles.Get( this ); return tiles.value; }
			set { if( tiles.BeginSet( this, ref value ) ) { try { TilesChanged?.Invoke( this ); ShouldRecompileMesh(); } finally { tiles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Tiles"/> property value changes.</summary>
		public event Action<MeshModifier_PlaneUV> TilesChanged;
		ReferenceField<Vector2> tiles = Vector2.One;

		/// <summary>
		/// The offset of UV coordinates.
		/// </summary>
		[Range( -10, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DefaultValue( "0 0" )]
		public Reference<Vector2> Offset
		{
			get { if( _offset.BeginGet() ) Offset = _offset.Get( this ); return _offset.value; }
			set { if( _offset.BeginSet( this, ref value ) ) { try { OffsetChanged?.Invoke( this ); ShouldRecompileMesh(); } finally { _offset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Offset"/> property value changes.</summary>
		public event Action<MeshModifier_PlaneUV> OffsetChanged;
		ReferenceField<Vector2> _offset = Vector2.Zero;

		[DefaultValue( AxisEnum.XY )]
		public Reference<AxisEnum> Axis
		{
			get { if( _axis.BeginGet() ) Axis = _axis.Get( this ); return _axis.value; }
			set { if( _axis.BeginSet( this, ref value ) ) { try { AxisChanged?.Invoke( this ); ShouldRecompileMesh(); } finally { _axis.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Axis"/> property value changes.</summary>
		public event Action<MeshModifier_PlaneUV> AxisChanged;
		ReferenceField<AxisEnum> _axis = AxisEnum.XY;

		/////////////////////////////////////////

		public enum AxisEnum
		{
			XY,
			XZ,
			YZ,
		}

		/////////////////////////////////////////

		void ProcessVertex( ref Vector2 tiles, ref Vector2 offset, ref Bounds bounds, ref Vector3F position, ref Vector3F normal, out Vector2F result )
		{
			int axis0 = 0;
			int axis1 = 1;

			switch( Axis.Value )
			{
			case AxisEnum.XY: axis0 = 0; axis1 = 1; break;
			case AxisEnum.XZ: axis0 = 0; axis1 = 2; break;
			case AxisEnum.YZ: axis0 = 1; axis1 = 2; break;
			}

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

			var v = tiles * new Vector2( s0, s1 ) + offset;
			//var v = new Vector2( s0 * tiles[ axis0 ], s1 * tiles[ axis1 ] ) + new Vector2( offset[ axis0 ], offset[ axis1 ] );

			//var offset2 = new Vector2( offset[ axis0 ], offset[ axis1 ] );
			//var v = new Vector2( s0 * tiles[ axis0 ], -s1 * tiles[ axis1 ] ) + offset2;

			result = v.ToVector2F();
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
			var offset = Offset.Value;

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
								ProcessVertex( ref tiles, ref offset, ref bounds, ref positions[ n ], ref normals[ n ], out newTexCoords[ n ] );

							var vertexBuffer = oper.VertexBuffers[ texCoordElement.Source ];
							vertexBuffer.MakeCopyOfData();
							vertexBuffer.WriteChannel( texCoordElement.Offset, newTexCoords );
						}
					}
				}
			}
		}

		protected override void OnBakeIntoMesh( Editor.IDocumentInstance document, Editor.UndoMultiAction undoMultiAction )
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
			var offset = Offset.Value;

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
						ProcessVertex( ref tiles, ref offset, ref bounds, ref positions[ n ], ref normals[ n ], out newTexCoords[ n ] );

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
	}
}
