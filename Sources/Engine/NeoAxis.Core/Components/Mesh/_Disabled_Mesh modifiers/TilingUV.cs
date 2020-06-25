//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;

//namespace NeoAxis
//{
//	[NewObjectDefaultName( "Tiling UV" )]
//	public class Component_MeshModifier_TilingUV : Component_MeshModifier
//	{
//		/// <summary>
//		/// The rotation of UV mapping in degrees.
//		/// </summary>
//		[DefaultValue( 0 )]
//		[Range( 0, 360 )]
//		public Reference<double> Rotation
//		{
//			get { if( _rotation.BeginGet() ) Rotation = _rotation.Get( this ); return _rotation.value; }
//			set
//			{
//				if( _rotation.BeginSet( ref value ) )
//				{
//					try
//					{
//						RotationChanged?.Invoke( this );
//						ShouldRecompileMesh();
//					}
//					finally { _rotation.EndSet(); }
//				}
//			}
//		}
//		/// <summary>Occurs when the <see cref="Rotation"/> property value changes.</summary>
//		public event Action<Component_MeshModifier_TilingUV> RotationChanged;
//		ReferenceField<double> _rotation = 0;

//		/// <summary>
//		/// The number of UV tiles per unit of the world.
//		/// </summary>
//		[DefaultValue( "1 1" )]
//		public Reference<Vector2> TilesPerUnit
//		{
//			get { if( tilesPerUnit.BeginGet() ) TilesPerUnit = tilesPerUnit.Get( this ); return tilesPerUnit.value; }
//			set
//			{
//				if( tilesPerUnit.BeginSet( ref value ) )
//				{
//					try
//					{
//						TilesPerUnitChanged?.Invoke( this );
//						ShouldRecompileMesh();
//					}
//					finally { tilesPerUnit.EndSet(); }
//				}
//			}
//		}
//		/// <summary>Occurs when the <see cref="TilesPerUnit"/> property value changes.</summary>
//		public event Action<Component_MeshModifier_TilingUV> TilesPerUnitChanged;
//		ReferenceField<Vector2> tilesPerUnit = Vector2.One;

//		/////////////////////////////////////////
//		Vector2F[] ProcessTexCoords( Vector2F[] texCoords)
//		{
//			var newTexCoords = new Vector2F[ texCoords.Length ];
//			Matrix2 transform = Matrix2.FromScale( TilesPerUnit.Value ) *  Matrix2.FromRotate( new DegreeF(Rotation.Value).InRadians());
		
//			for( int n = 0; n < texCoords.Length; n++ )
//				newTexCoords[ n ] = ( transform * texCoords[ n ] ).ToVector2F();
			
//			return newTexCoords;
//		}


//		protected override void OnApplyToMeshData( Component_Mesh.CompiledData compiledData )
//		{
//			base.OnApplyToMeshData( compiledData );

//			foreach( var oper in compiledData.MeshData.RenderOperations )
//			{
//				oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate0, out VertexElement texCoordElement );
//				var vertexBuffer = oper.VertexBuffers[ texCoordElement.Source ];
//				var texCoords = vertexBuffer.ExtractChannel<Vector2F>( texCoordElement.Offset );

//				var newTexCoords = ProcessTexCoords( texCoords );

//				vertexBuffer.MakeCopyOfData();
//				vertexBuffer.WriteChannel( texCoordElement.Offset, newTexCoords );
//			}
//		}

//		protected override void OnBakeIntoMesh( DocumentInstance document, UndoMultiAction undoMultiAction )
//		{
//			base.OnBakeIntoMesh( document, undoMultiAction );

//			var mesh = (Component_Mesh)Parent;
//			var geometries = mesh.GetComponents<Component_MeshGeometry>();

//			foreach( var geometry in geometries )
//			{
//				var texCoords = geometry.VerticesExtractChannel<Vector2F>( VertexElementSemantic.TextureCoordinate0 );
//				if( texCoords != null )
//				{
//					var oldValue = geometry.Vertices;
//					var vertices = geometry.Vertices.Value;

//					//var vertexStructure = geometry.VertexStructure.Value;
//					//vertexStructure.GetInfo( out var vertexSize, out _ );
//					//var vertexCount = vertices.Length / vertexSize;

//					var newTexCoords = ProcessTexCoords( texCoords );

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
