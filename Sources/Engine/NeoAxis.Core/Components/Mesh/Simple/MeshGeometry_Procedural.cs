// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Base component for mesh geometry procedural generation.
	/// </summary>
	public abstract class MeshGeometry_Procedural : MeshGeometry
	{
		/// <summary>
		/// The position, rotation and scale of the geometry relative to parent.
		/// </summary>
		[Serialize]
		[DefaultValue( Transform.IdentityAsString )]
		public Reference<Transform> LocalTransform
		{
			get { if( _localTransform.BeginGet() ) LocalTransform = _localTransform.Get( this ); return _localTransform.value; }
			set
			{
				if( _localTransform.BeginSet( this, ref value ) )
				{
					try
					{
						LocalTransformChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _localTransform.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LocalTransform"/> property value changes.</summary>
		public event Action<MeshGeometry_Procedural> LocalTransformChanged;
		ReferenceField<Transform> _localTransform = Transform.Identity;

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				if( p.Name == nameof( VertexStructure ) || p.Name == nameof( UnwrappedUV ) || p.Name == nameof( Vertices ) || p.Name == nameof( Indices ) )
				{
					skip = true;
					return;
				}
			}
		}

		protected override void OnGetDataOfThisObject( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Material material, ref byte[] voxelData, ref byte[] clusterData, ref Mesh.StructureClass structure )
		{
			GetProceduralGeneratedData( ref vertexStructure, ref vertices, ref indices, ref material, ref voxelData, ref clusterData, ref structure );
		}

		public abstract void GetProceduralGeneratedData( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Material material, ref byte[] voxelData, ref byte[] clusterData, ref Mesh.StructureClass structure );

		public virtual bool ExistsMeshStructure()
		{
			return true;
		}

		protected void ApplyLocalTransform( Vector3F[] positions, Vector3F[] normals, Vector4F[] tangents )
		{
			var localTransform = LocalTransform.Value;
			if( localTransform != null && !localTransform.IsIdentity )
			{
				ref var transform = ref localTransform.ToMatrix4();

				var transformRotation = localTransform.Rotation.ToMatrix3();
				if( !localTransform.IsScaleOne && ( localTransform.Scale.X != localTransform.Scale.Y || localTransform.Scale.X != localTransform.Scale.Z ) )
				{
					var scaleMatrix = Matrix3.FromScale( localTransform.Scale );
					Matrix3.Multiply( ref transformRotation, ref scaleMatrix, out var rotationScaleMatrix );
					transformRotation = rotationScaleMatrix.ToMatrix3F();
				}

				for( int n = 0; n < positions.Length; n++ )
				{
					positions[ n ] = ( transform * positions[ n ] ).ToVector3F();
					normals[ n ] = ( transformRotation * normals[ n ] ).GetNormalize().ToVector3F();
					var tangent3 = ( transformRotation * tangents[ n ].ToVector3() ).GetNormalize().ToVector3F();
					tangents[ n ] = new Vector4F( tangent3, tangents[ n ].W );
				}
			}
		}

		protected void ApplyLocalTransform( Vector3[] positions, Vector3[] normals, Vector4[] tangents )
		{
			var localTransform = LocalTransform.Value;
			if( localTransform != null && !localTransform.IsIdentity )
			{
				ref var transform = ref localTransform.ToMatrix4();

				var transformRotation = localTransform.Rotation.ToMatrix3();
				if( !localTransform.IsScaleOne && ( localTransform.Scale.X != localTransform.Scale.Y || localTransform.Scale.X != localTransform.Scale.Z ) )
				{
					var scaleMatrix = Matrix3.FromScale( localTransform.Scale );
					Matrix3.Multiply( ref transformRotation, ref scaleMatrix, out var rotationScaleMatrix );
					transformRotation = rotationScaleMatrix.ToMatrix3F();
				}

				for( int n = 0; n < positions.Length; n++ )
				{
					positions[ n ] = transform * positions[ n ];
					normals[ n ] = ( transformRotation * normals[ n ] ).GetNormalize();
					var tangent3 = ( transformRotation * tangents[ n ].ToVector3() ).GetNormalize();
					tangents[ n ] = new Vector4( tangent3, tangents[ n ].W );
				}
			}
		}
	}
}
