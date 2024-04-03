// Copyright (c) 2016 Robert Rouhani <robert.rouhani@gmail.com> and other contributors (see CONTRIBUTORS file).
// Licensed under the MIT License - https://raw.github.com/Robmaister/SharpNav/master/LICENSE

using System;
using System.Runtime.InteropServices;

using Internal.SharpNav.Geometry;

namespace Internal.SharpNav.Pathfinding
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct RaycastHit
	{
		public float T;
		public Vector3 Normal;
		public int EdgeIndex;

		public bool IsHit { get { return T != float.MaxValue; } }
	}
}
