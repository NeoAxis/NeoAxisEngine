//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;

//namespace NeoAxis
//{
//	[NewObjectDefaultName( "Spherical UV" )]
//	public class Component_MeshModifier_SphericalUV : Component_MeshModifier
//	{
//		/// <summary>
//		/// The number of UV tiles.
//		/// </summary>
//		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
//		[DefaultValue( "1 1" )]
//		public Reference<Vector2> Tiles
//		{
//			get { if( tiles.BeginGet() ) Tiles = tiles.Get( this ); return tiles.value; }
//			set { if( tiles.BeginSet( ref value ) ) { try { TilesChanged?.Invoke( this ); ShouldRecompileMesh(); } finally { tiles.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Tiles"/> property value changes.</summary>
//		public event Action<Component_MeshModifier_SphericalUV> TilesChanged;
//		ReferenceField<Vector2> tiles = Vector2.One;

//		/////////////////////////////////////////

//		void ProcessVertex( ref Vector2F tiles, ref Vector3F position, ref Vector3F normal, out Vector2F result )
//		{
//			SphericalDirectionF.FromVector( ref normal, out var dir );

//			var x = dir.Horizontal / ( MathEx.PI * 2 ) * tiles.X;
//			var y = -dir.Vertical / MathEx.PI * tiles.Y;

//			result = new Vector2F( x, y );
//		}

//		protected override void OnModify( Component_Mesh.CompiledData compiledData )
//		{
//			base.OnModify( compiledData );

//			var tiles = Tiles.Value.ToVector2F();

//			foreach( var oper in compiledData.MeshData.RenderOperations )
//			{
//				if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out var positionElement ) && positionElement.Type == VertexElementType.Float3 )
//				{
//					if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Normal, out var normalElement ) && normalElement.Type == VertexElementType.Float3 )
//					{
//						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate0, out var texCoordElement ) && texCoordElement.Type == VertexElementType.Float2 )
//						{
//							var positions = oper.VertexBuffers[ positionElement.Source ].ExtractChannel<Vector3F>( positionElement.Offset );
//							var normals = oper.VertexBuffers[ normalElement.Source ].ExtractChannel<Vector3F>( normalElement.Offset );

//							var newTexCoords = new Vector2F[ positions.Length ];
//							for( int n = 0; n < newTexCoords.Length; n++ )
//								ProcessVertex( ref tiles, ref positions[ n ], ref normals[ n ], out newTexCoords[ n ] );

//							var vertexBuffer = oper.VertexBuffers[ texCoordElement.Source ];
//							vertexBuffer.MakeCopyOfData();
//							vertexBuffer.WriteChannel( texCoordElement.Offset, newTexCoords );
//						}
//					}
//				}
//			}
//		}

//		protected override void OnBakeIntoMesh( DocumentInstance document, UndoMultiAction undoMultiAction )
//		{
//			base.OnBakeIntoMesh( document, undoMultiAction );

//			var mesh = (Component_Mesh)Parent;
//			var geometries = mesh.GetComponents<Component_MeshGeometry>();

//			var tiles = Tiles.Value.ToVector2F();

//			foreach( var geometry in geometries )
//			{
//				var positions = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
//				var normals = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Normal );
//				if( positions != null && normals != null )
//				{
//					var vertexStructure = geometry.VertexStructure.Value;
//					vertexStructure.GetInfo( out var vertexSize, out _ );

//					var oldValue = geometry.Vertices;
//					var vertices = geometry.Vertices.Value;
//					var vertexCount = vertices.Length / vertexSize;

//					var newTexCoords = new Vector2F[ vertexCount ];
//					for( int n = 0; n < vertexCount; n++ )
//						ProcessVertex( ref tiles, ref positions[ n ], ref normals[ n ], out newTexCoords[ n ] );

//					var newVertices = (byte[])vertices.Clone();
//					if( geometry.VerticesWriteChannel( VertexElementSemantic.TextureCoordinate0, newTexCoords, newVertices ) )
//					{
//						//update property
//						geometry.Vertices = newVertices;

//						//undo
//						if( undoMultiAction != null )
//						{
//							var property = (Metadata.Property)geometry.MetadataGetMemberBySignature( "property:Vertices" );
//							var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( geometry, property, oldValue ) );
//							undoMultiAction.AddAction( undoAction );
//						}
//					}
//				}
//			}

//		}
//	}
//}
