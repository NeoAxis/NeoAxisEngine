//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;

//namespace NeoAxis
//{
//	public class Component_MeshModifier_Unwrap : Component_MeshModifier
//	{
//		protected override void OnModify( Component_Mesh.CompiledData compiledData )
//		{
//			base.OnModify( compiledData );

//			//float multiplier = (float)1.1;
//			//float invMultiplier = 1.0f / multiplier;

//			//foreach( var oper in compiledData.MeshData.RenderOperations )
//			//{
//			//	//foreach( var vb in oper.vertexBuffers )
//			//	//{

//			//	Random r = new Random( 0 );

//			//	oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out VertexElement positionElement );
//			//	var vertexBuffer = oper.VertexBuffers[ positionElement.Source ];
//			//	Vector3F[] positions = vertexBuffer.ExtractChannel<Vector3F>( positionElement.Offset );

//			//	Vector3F[] newPositions = new Vector3F[ positions.Length ];
//			//	for( int n = 0; n < positions.Length; n++ )
//			//	{
//			//		Vector3F src = positions[ n ];

//			//		newPositions[ n ] = new Vector3F(
//			//			src.X * r.Next( invMultiplier, multiplier ),
//			//			src.Y * r.Next( invMultiplier, multiplier ),
//			//			src.Z * r.Next( invMultiplier, multiplier ) );
//			//	}

//			//	vertexBuffer.MakeCopyOfData();
//			//	vertexBuffer.WriteChannel( positionElement.Offset, newPositions );
//			//	//}
//			//}
//		}
//	}
//}
