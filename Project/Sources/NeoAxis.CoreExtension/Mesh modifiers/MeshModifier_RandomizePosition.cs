// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A mesh modifier that adds a random offset to the position of the vertices.
	/// </summary>
	[NewObjectDefaultName( "Randomize Position" )]
	[AddToResourcesWindow( @"Base\Scene common\Mesh modifiers\Randomize Position", 2 )]
	public class MeshModifier_RandomizePosition : MeshModifier
	{
		[DefaultValue( 1.1 )]
		[Range( 1, 2, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> Multiplier
		{
			get { if( _multiplier.BeginGet() ) Multiplier = _multiplier.Get( this ); return _multiplier.value; }
			set
			{
				if( value < 1 )
					value = new Reference<double>( 1, value.GetByReference );
				if( _multiplier.BeginSet( ref value ) )
				{
					try
					{
						MultiplierChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _multiplier.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Multiplier"/> property value changes.</summary>
		public event Action<MeshModifier_RandomizePosition> MultiplierChanged;
		ReferenceField<double> _multiplier = 1.1;

		/////////////////////////////////////////

		void ProcessVertex( float multiplier, float invMultiplier, ref Vector3F position, out Vector3F result )
		{
			var random = new FastRandom( position.GetHashCode() );

			result = new Vector3F(
				position.X * random.Next( invMultiplier, multiplier ),
				position.Y * random.Next( invMultiplier, multiplier ),
				position.Z * random.Next( invMultiplier, multiplier ) );
		}

		protected override void OnApplyToMeshData( Mesh.CompiledData compiledData )
		{
			base.OnApplyToMeshData( compiledData );

			float multiplier = (float)Multiplier;
			float invMultiplier = multiplier != 0 ? ( 1.0f / multiplier ) : 0;

			foreach( var oper in compiledData.MeshData.RenderOperations )
			{
				oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out VertexElement positionElement );
				var vertexBuffer = oper.VertexBuffers[ positionElement.Source ];
				var positions = vertexBuffer.ExtractChannel<Vector3F>( positionElement.Offset );

				var newPositions = new Vector3F[ positions.Length ];
				for( int n = 0; n < positions.Length; n++ )
					ProcessVertex( multiplier, invMultiplier, ref positions[ n ], out newPositions[ n ] );

				vertexBuffer.MakeCopyOfData();
				vertexBuffer.WriteChannel( positionElement.Offset, newPositions );
			}
		}

#if !DEPLOY
		protected override void OnBakeIntoMesh( Editor.DocumentInstance document, Editor.UndoMultiAction undoMultiAction )
		{
			base.OnBakeIntoMesh( document, undoMultiAction );

			float multiplier = (float)Multiplier;
			float invMultiplier = multiplier != 0 ? ( 1.0f / multiplier ) : 0;

			var mesh = (Mesh)Parent;
			var geometries = mesh.GetComponents<MeshGeometry>();

			foreach( var geometry in geometries )
			{
				var positions = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
				if( positions != null )
				{
					var vertexStructure = geometry.VertexStructure.Value;
					vertexStructure.GetInfo( out var vertexSize, out _ );

					var oldValue = geometry.Vertices;
					var vertices = geometry.Vertices.Value;
					var vertexCount = vertices.Length / vertexSize;

					var newPositions = new Vector3F[ positions.Length ];
					for( int n = 0; n < positions.Length; n++ )
						ProcessVertex( multiplier, invMultiplier, ref positions[ n ], out newPositions[ n ] );

					var newVertices = (byte[])vertices.Clone();
					if( geometry.VerticesWriteChannel( VertexElementSemantic.Position, newPositions, newVertices ) )
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
