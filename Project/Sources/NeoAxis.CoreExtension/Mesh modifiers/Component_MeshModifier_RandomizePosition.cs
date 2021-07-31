// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A mesh modifier that adds a random offset to the position of the vertices.
	/// </summary>
	[NewObjectDefaultName( "Randomize Position" )]
	[AddToResourcesWindow( @"Base\Scene common\Mesh modifiers\Randomize Position", 2 )]
	public class Component_MeshModifier_RandomizePosition : Component_MeshModifier
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
		public event Action<Component_MeshModifier_RandomizePosition> MultiplierChanged;
		ReferenceField<double> _multiplier = 1.1;

		/////////////////////////////////////////

		void ProcessVertex( float multiplier, float invMultiplier, ref Vector3F position, out Vector3F result )
		{
			var random = new Random( position.GetHashCode() );

			result = new Vector3F(
				position.X * random.Next( invMultiplier, multiplier ),
				position.Y * random.Next( invMultiplier, multiplier ),
				position.Z * random.Next( invMultiplier, multiplier ) );
		}

		protected override void OnApplyToMeshData( Component_Mesh.CompiledData compiledData )
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

		protected override void OnBakeIntoMesh( DocumentInstance document, UndoMultiAction undoMultiAction )
		{
			base.OnBakeIntoMesh( document, undoMultiAction );

			float multiplier = (float)Multiplier;
			float invMultiplier = multiplier != 0 ? ( 1.0f / multiplier ) : 0;

			var mesh = (Component_Mesh)Parent;
			var geometries = mesh.GetComponents<Component_MeshGeometry>();

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
							var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( geometry, property, oldValue ) );
							undoMultiAction.AddAction( undoAction );
						}
					}
				}
			}
		}
	}
}
