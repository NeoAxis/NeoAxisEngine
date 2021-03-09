// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

namespace NeoAxis
{
	/// <summary>
	/// The class is intended for accessing component methods that are preferable to hide from public access. This is done to avoid cluttering up the component classes.
	/// </summary>
	public static class ComponentsHidePublic
	{
		/////////////////////////////////////////
		//Component

		public static void PerformUpdate( Component obj, float delta )
		{
			obj.PerformUpdate( delta );
		}

		public static void PerformSimulationStep( Component obj )
		{
			obj.PerformSimulationStep();
		}

		public static void VirtualMembersNeedUpdate( Component obj )
		{
			obj.VirtualMembersNeedUpdate();
		}

		public static Dictionary<string, object> VirtualMemberValuesGet( Component obj )
		{
			return obj.VirtualMemberValues;
		}

		public static void VirtualMemberValuesSet( Component obj, Dictionary<string, object> value )
		{
			obj.VirtualMemberValues = value;
		}

		/////////////////////////////////////////
		//Component_Scene

		public static void PerformGetRenderSceneData( Component_Scene obj, ViewportRenderingContext context )
		{
			obj.PerformGetRenderSceneData( context );
		}

		/////////////////////////////////////////
		//Component_ObjectInSpace

		public static int GetRenderSceneIndex( Component_ObjectInSpace obj )
		{
			return obj._internalRenderSceneIndex;
		}

		public static void SetRenderSceneIndex( Component_ObjectInSpace obj, int v )
		{
			obj._internalRenderSceneIndex = v;
		}

		public static void PerformGetRenderSceneData( Component_ObjectInSpace obj, ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			obj.PerformGetRenderSceneData( context, mode, modeGetObjectsItem );
		}
	}
}
