// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ComponentModel;

namespace NeoAxis
{
	public interface IComponent_VisibleInHierarchy
	{
		[Browsable( false )]
		bool VisibleInHierarchy { get; }
	}

	public interface IComponent_CanBeSelectedInHierarchy
	{
		[Browsable( false )]
		bool CanBeSelectedInHierarchy { get; }
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface IComponent_Terrain
	{
		bool GetPositionByRay( Ray ray, bool considerHoles, out Vector3 position );
		Rectangle GetBounds2();
		double GetHeight( Vector2 position, bool considerHoles );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An interface for navigation mesh pathfinding.
	/// </summary>
	public interface IComponent_Pathfinding
	{
		void FindPath( Component_Pathfinding_FindPathContext context );
	}

	/// <summary>
	/// The data to execution finding path.
	/// </summary>
	public class Component_Pathfinding_FindPathContext
	{
		public Vector3 Start;
		public Vector3 End;

		public double StepSize = 0.5;
		public double Slop = 0.01;
		public Vector3 PolygonPickExtents = new Vector3( 2, 2, 2 );
		//public int MaxPolygonPath = 512;
		public int MaxSmoothPath = 2048;
		//public int MaxSteerPoints = 16;

		public bool Finished;
		public Vector3[] Path;
		public string Error = string.Empty;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class PathfindingUtility
	{
		public static List<IComponent_Pathfinding> Instances = new List<IComponent_Pathfinding>();
	}

}
