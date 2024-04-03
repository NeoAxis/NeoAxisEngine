// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
		//Scene

		public static void PerformGetRenderSceneData( Scene obj, ViewportRenderingContext context )
		{
			obj.PerformGetRenderSceneData( context );
		}

		//public static void PerformGetRenderSceneDataAfterObjects( Scene obj, ViewportRenderingContext context )
		//{
		//	obj.PerformGetRenderSceneDataAfterObjects( context );
		//}

		/////////////////////////////////////////
		//ObjectInSpace

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static int GetRenderSceneIndex( ObjectInSpace obj )
		{
			return obj._internalRenderSceneIndex;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void SetRenderSceneIndex( ObjectInSpace obj, int v )
		{
			obj._internalRenderSceneIndex = v;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void PerformGetRenderSceneData( ObjectInSpace obj, ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			obj.PerformGetRenderSceneData( context, mode, modeGetObjectsItem );
		}
	}
}
