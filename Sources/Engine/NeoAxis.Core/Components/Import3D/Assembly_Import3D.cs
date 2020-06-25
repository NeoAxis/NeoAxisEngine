// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using NeoAxis;

namespace NeoAxis.Import
{
	/// <summary>
	/// The class is intended to register import content resource.
	/// </summary>
	public class Assembly_Import3D : AssemblyUtility.AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Import 3D", new string[] {
				"fbx", "3d", "3ds", "ac", "ac3d", "acc", "ase", "ask", "b3d", "blend", "bvh", "cob", "csm", "dae", "dxf",
				"enff", "hmp", "ifc", "irr", "irrmesh", "lwo", "lws", "lxo", "md2", "md3", "md5anim", "md5camera", "md5mesh",
				"mdc", "mdl", "mot", "ms3d", "ndo", "nff", "obj", "off", "pk3", "ply", "x", "q3d", "q3s", "assxml", "gltf", "glb", "json" },
				typeof( Resource_Import3D ) );
		}
	}
}
