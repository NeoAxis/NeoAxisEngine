// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	public abstract class Component_MeshGeometry_Procedural : Component_MeshGeometry
	{
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

		protected override void OnGetDataOfThisObject( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Component_Material material, ref Component_Mesh.StructureClass structure )
		{
			GetProceduralGeneratedData( ref vertexStructure, ref vertices, ref indices, ref material, ref structure );
		}

		public abstract void GetProceduralGeneratedData( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Component_Material material, ref Component_Mesh.StructureClass structure );

		public virtual bool ExistsMeshStructure()
		{
			return true;
		}
	}
}
