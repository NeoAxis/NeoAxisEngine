// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// An octree container to optimize getting list of objects by volume.
	/// </summary>
	public class OctreeContainer : IDisposable
	{
		IntPtr nativeObject;

		//

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr OctreeContainer_New( ref Bounds initialOctreeBounds, int amountOfObjectsOutsideOctreeBoundsToRebuld, ref Vector3 octreeBoundsRebuildExpand, ref Vector3 minNodeSize, int objectCountThresholdToCreateChildNodes, int maxNodeCount, ThreadingModeEnum threadingMode, int getObjectsInputDataSize, double engineTimeToGetStatistics );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern void OctreeContainer_Delete( IntPtr container );

		//[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		//static extern void OctreeContainer_UpdateSettings( IntPtr container, int amountOfObjectsOutsideOctreeBoundsToRebuld, ref Vector3 octreeBoundsRebuildExpand, ref Vector3 minNodeSize, int objectCountThresholdToCreateChildNodes, int maxNodeCount, ThreadingModeEnum threadingMode );//, [MarshalAs( UnmanagedType.U1 )] bool forceTreeRebuild );

		//[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		//static extern void OctreeContainer_RebuildTree( IntPtr container );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern int OctreeContainer_AddObject( IntPtr container, ref Vector3 boundsMin, ref Vector3 boundsMax, uint groupMask );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern void OctreeContainer_RemoveObject( IntPtr container, int objectIndex );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern void OctreeContainer_UpdateObjectBounds( IntPtr container, int objectIndex, ref Vector3 boundsMin, ref Vector3 boundsMax );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern void OctreeContainer_UpdateObjectGroupMask( IntPtr container, int objectIndex, uint groupMask );

		//[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		//static extern void OctreeContainer_UpdateObject( IntPtr container, int objectIndex, ref Vector3 boundsMin, ref Vector3 boundsMax, uint groupMask );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe void OctreeContainer_GetDebugRenderLines( IntPtr container, out IntPtr outputData, out int outputDataItemCount );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe void OctreeContainer_Free( IntPtr container, IntPtr data );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe int OctreeContainer_GetObjects( IntPtr container, ref GetObjectsInputData inputData, int* outputArray, int outputArraySize );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe int OctreeContainer_GetObjectsRay( IntPtr container, ref GetObjectsInputData inputData, GetObjectsRayOutputData* outputArray, int outputArraySize );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe void OctreeContainer_GetStatistics( IntPtr container, out int objectCount, out Bounds octreeBounds, out int octreeNodeCount, out double timeSinceLastFullRebuild );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe void OctreeContainer_GetOctreeBoundsWithBoundsOfObjectsOutsideOctree( IntPtr container, out Bounds bounds );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe void OctreeContainer_SetEngineTimeToGetStatistics( IntPtr container, double engineTime );

		///////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		struct DebugRenderLine
		{
			public Vector3 start;
			public Vector3 end;
			public ColorValue color;
		}

		///////////////////////////////////////////

		enum GetObjectsTypes
		{
			Bounds,
			Sphere,
			Box,
			Planes,
			Ray,
		}

		///////////////////////////////////////////

		public enum ModeEnum
		{
			All,
			One,
		}

		///////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		struct GetObjectsInputData
		{
			public uint groupMask;
			public GetObjectsTypes type;
			public Bounds bounds;
			public Vector3 sphereCenter;
			public double sphereRadius;
			public Box box;
			public int planeCount;
			public IntPtr planes;
			public int planesUseAdditionalBounds;
			public Ray ray;
			public ModeEnum mode;
			public IntPtr extensionData;
		}

		///////////////////////////////////////////

		/// <summary>
		/// Represents returned data upon request from <see cref="OctreeContainer"/>.
		/// </summary>
		[StructLayout( LayoutKind.Sequential )]
		public struct GetObjectsRayOutputData
		{
			public int ObjectIndex;
			public double DistanceNormalized;

			public GetObjectsRayOutputData( int objectIndex, double distanceNormalized )
			{
				ObjectIndex = objectIndex;
				DistanceNormalized = distanceNormalized;
			}

			//public int ObjectIndex
			//{
			//	get { return objectIndex; }
			//	set { objectIndex = value; }
			//}

			//public double DistanceNormalized
			//{
			//	get { return distanceNormalized; }
			//	set { distanceNormalized = value; }
			//}
		}

		///////////////////////////////////////////

		public enum ThreadingModeEnum
		{
			SingleThreaded,
			BackgroundThread,
			//MultiBackgroundThreads,
		}

		///////////////////////////////////////////

		/// <summary>
		/// Represents initialization settings of <see cref="OctreeContainer"/>.
		/// </summary>
		public class InitSettings
		{
			internal Bounds initialOctreeBounds = new Bounds( -1, -1, -1, 1, 1, 1 );
			internal int amountOfObjectsOutsideOctreeBoundsToRebuld = 30;
			internal Vector3 octreeBoundsRebuildExpand = new Vector3( 50, 50, 50 );
			internal Vector3 minNodeSize = new Vector3( 10, 10, 10 );
			internal int objectCountThresholdToCreateChildNodes = 50;
			internal int maxNodeCount = 100000;
			internal ThreadingModeEnum threadingMode = ThreadingModeEnum.SingleThreaded;

			//

			public Bounds InitialOctreeBounds
			{
				get { return initialOctreeBounds; }
				set { initialOctreeBounds = value; }
			}

			public int AmountOfObjectsOutsideOctreeBoundsToRebuld
			{
				get { return amountOfObjectsOutsideOctreeBoundsToRebuld; }
				set { amountOfObjectsOutsideOctreeBoundsToRebuld = value; }
			}

			public Vector3 OctreeBoundsRebuildExpand
			{
				get { return octreeBoundsRebuildExpand; }
				set { octreeBoundsRebuildExpand = value; }
			}

			public Vector3 MinNodeSize
			{
				get { return minNodeSize; }
				set { minNodeSize = value; }
			}

			public int ObjectCountThresholdToCreateChildNodes
			{
				get { return objectCountThresholdToCreateChildNodes; }
				set { objectCountThresholdToCreateChildNodes = value; }
			}

			public int MaxNodeCount
			{
				get { return maxNodeCount; }
				set { maxNodeCount = value; }
			}

			public ThreadingModeEnum ThreadingMode
			{
				get { return threadingMode; }
				set { threadingMode = value; }
			}
		}

		///////////////////////////////////////////

		static bool neoAxisProprietaryLibraryLoaded;

		static void LoadNeoAxisProprietaryLibrary()
		{
			if( !neoAxisProprietaryLibraryLoaded )
			{
				neoAxisProprietaryLibraryLoaded = true;
				NativeUtility.PreloadLibrary( OgreWrapper.library );
			}
		}

		///////////////////////////////////////////

		public OctreeContainer( InitSettings initSettings )
		{
			unsafe
			{
				LoadNeoAxisProprietaryLibrary();

				//fix minNodeSize
				for( int axis = 0; axis < 3; axis++ )
				{
					if( initSettings.minNodeSize[ axis ] < 0.01 )
						initSettings.minNodeSize[ axis ] = 0.01;
				}

				//fix initialOctreeBounds
				for( int axis = 0; axis < 3; axis++ )
				{
					if( initSettings.initialOctreeBounds.GetSize()[ axis ] <= initSettings.minNodeSize[ axis ] )
					{
						Vector3 v = Vector3.Zero;
						v[ axis ] = initSettings.minNodeSize[ axis ] * 0.501;
						initSettings.initialOctreeBounds.Expand( v );
					}
				}

				nativeObject = OctreeContainer_New(
					ref initSettings.initialOctreeBounds,
					initSettings.amountOfObjectsOutsideOctreeBoundsToRebuld,
					ref initSettings.octreeBoundsRebuildExpand,
					ref initSettings.minNodeSize,
					initSettings.objectCountThresholdToCreateChildNodes,
					initSettings.maxNodeCount,
					initSettings.threadingMode,
					sizeof( GetObjectsInputData ), 
					EngineApp.EngineTime );
			}
		}

		public void Dispose()
		{
			if( nativeObject != IntPtr.Zero )
			{
				OctreeContainer_Delete( nativeObject );
				nativeObject = IntPtr.Zero;
			}
		}

		//public void UpdateSettings( int amountOfObjectsOutsideOctreeBoundsToRebuld, Vector3 octreeBoundsRebuildExpand, Vector3 minNodeSize, int objectCountThresholdToCreateChildNodes, int maxNodeCount, ThreadingModeEnum threadingMode )//, bool forceTreeRebuild )
		//{
		//	OctreeContainer_UpdateSettings( nativeObject, amountOfObjectsOutsideOctreeBoundsToRebuld, ref octreeBoundsRebuildExpand,
		//		ref minNodeSize, objectCountThresholdToCreateChildNodes, maxNodeCount, threadingMode );//, forceTreeRebuild );
		//}

		//public void RebuildTree()
		//{
		//	OctreeContainer_RebuildTree( nativeObject );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int AddObject( ref Bounds bounds, uint groupMask )
		{
			if( bounds.IsCleared() )
				Log.Fatal( "OctreeContainer: AddObject: Invalid bounds." );

			return OctreeContainer_AddObject( nativeObject, ref bounds.Minimum, ref bounds.Maximum, groupMask );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int AddObject( Bounds bounds, uint groupMask )
		{
			return AddObject( ref bounds, groupMask );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void RemoveObject( int objectIndex )
		{
			OctreeContainer_RemoveObject( nativeObject, objectIndex );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void UpdateObjectBounds( int objectIndex, ref Bounds bounds )
		{
			if( bounds.IsCleared() )
				Log.Fatal( "OctreeContainer: UpdateObjectBounds: Invalid bounds." );

			OctreeContainer_UpdateObjectBounds( nativeObject, objectIndex, ref bounds.Minimum, ref bounds.Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void UpdateObjectBounds( int objectIndex, Bounds bounds )
		{
			UpdateObjectBounds( objectIndex, ref bounds );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void UpdateObjectGroupMask( int objectIndex, uint groupMask )
		{
			OctreeContainer_UpdateObjectGroupMask( nativeObject, objectIndex, groupMask );
		}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//public void UpdateObject( int objectIndex, ref Bounds bounds, uint groupMask )
		//{
		//	if( bounds.IsCleared() )
		//		Log.Fatal( "OctreeContainer: UpdateObject: Invalid bounds." );

		//	OctreeContainer_UpdateObject( nativeObject, objectIndex, ref bounds.Minimum, ref bounds.Maximum, groupMask );
		//	//Vector3 boundsMin = bounds.Minimum;
		//	//Vector3 boundsMax = bounds.Maximum;
		//	//OctreeContainer_UpdateObject( nativeObject, objectIndex, ref boundsMin, ref boundsMax, groupMask );
		//}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//public void UpdateObject( int objectIndex, Bounds bounds, uint groupMask )
		//{
		//	UpdateObject( objectIndex, ref bounds, groupMask );
		//}

		public void DebugRender( Viewport viewport )
		{
			unsafe
			{
				IntPtr data;
				int dataItemCount;
				OctreeContainer_GetDebugRenderLines( nativeObject, out data, out dataItemCount );

				DebugRenderLine* lines = (DebugRenderLine*)data;
				for( int n = 0; n < dataItemCount; n++ )
				{
					ref var color = ref lines[ n ].color;

					viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier * new ColorValue( 1, 1, 1, 0.5 ) );
					viewport.Simple3DRenderer.AddLineThin( lines[ n ].start, lines[ n ].end );
				}

				OctreeContainer_Free( nativeObject, data );
			}
		}

		/////////////////////////////////////////

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		unsafe bool GetObjects( ref GetObjectsInputData inputData, int* outputArray, int outputArrayLength, out int outputCount )
		{
			outputCount = OctreeContainer_GetObjects( nativeObject, ref inputData, outputArray, outputArrayLength );
			if( outputCount <= outputArrayLength )
				return true;
			else
				return false;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe bool GetObjects( Bounds bounds, uint groupMask, ModeEnum mode, int* outputArray, int outputArrayLength, out int outputCount )
		{
			//fix invalid bounds
			Bounds fixedBounds = new Bounds( bounds.Minimum );
			fixedBounds.Add( bounds.Maximum );

			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Bounds;
			inputData.bounds = fixedBounds;
			return GetObjects( ref inputData, outputArray, outputArrayLength, out outputCount );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe bool GetObjects( Sphere sphere, uint groupMask, ModeEnum mode, int* outputArray, int outputArrayLength, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Sphere;
			inputData.sphereCenter = sphere.Center;
			inputData.sphereRadius = sphere.Radius;
			//fix invalid radius
			if( inputData.sphereRadius < 0 )
				inputData.sphereRadius = -inputData.sphereRadius;
			return GetObjects( ref inputData, outputArray, outputArrayLength, out outputCount );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe bool GetObjects( Box box, uint groupMask, ModeEnum mode, int* outputArray, int outputArrayLength, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Box;
			inputData.box = box;
			return GetObjects( ref inputData, outputArray, outputArrayLength, out outputCount );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe bool GetObjects( Plane[] planes, uint groupMask, ModeEnum mode, int* outputArray, int outputArrayLength, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			fixed( Plane* pPlanes = planes )
			{
				inputData.planes = (IntPtr)pPlanes;
				inputData.planesUseAdditionalBounds = 0;
				return GetObjects( ref inputData, outputArray, outputArrayLength, out outputCount );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe bool GetObjects( Plane[] planes, Bounds bounds, uint groupMask, ModeEnum mode, int* outputArray, int outputArrayLength, out int outputCount, IntPtr extensionData )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			fixed( Plane* pPlanes = planes )
			{
				inputData.planes = (IntPtr)pPlanes;
				inputData.planesUseAdditionalBounds = 1;
				inputData.bounds = bounds;
				inputData.extensionData = extensionData;
				return GetObjects( ref inputData, outputArray, outputArrayLength, out outputCount );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe bool GetObjects( Frustum frustum, uint groupMask, ModeEnum mode, int* outputArray, int outputArrayLength, out int outputCount, IntPtr extensionData )
		{
			Plane[] planes = frustum.Planes;
			Bounds bounds = new Bounds( frustum.Points[ 0 ] );
			for( int n = 1; n < 8; n++ )
				bounds.Add( frustum.Points[ n ] );
			return GetObjects( planes, bounds, groupMask, mode, outputArray, outputArrayLength, out outputCount, extensionData );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		unsafe bool GetObjectsRay( ref GetObjectsInputData inputData, GetObjectsRayOutputData* outputArray, int outputArrayLength, out int outputCount )
		{
			outputCount = OctreeContainer_GetObjectsRay( nativeObject, ref inputData, outputArray, outputArrayLength );
			if( outputCount <= outputArrayLength )
				return true;
			else
				return false;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe bool GetObjects( Ray ray, uint groupMask, ModeEnum mode, GetObjectsRayOutputData* outputArray, int outputArrayLength, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Ray;
			inputData.ray = ray;
			return GetObjectsRay( ref inputData, outputArray, outputArrayLength, out outputCount );
		}

		/////////////////////////////////////////

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe bool GetObjects( ref GetObjectsInputData inputData, int[] outputArray, out int outputCount )
		{
			if( outputArray != null )
			{
				fixed( int* pOutputArray = outputArray )
				{
					outputCount = OctreeContainer_GetObjects( nativeObject, ref inputData, pOutputArray, outputArray.Length );
					if( outputCount <= outputArray.Length )
						return true;
					else
						return false;
				}
			}
			else
			{
				outputCount = OctreeContainer_GetObjects( nativeObject, ref inputData, null, 0 );
				return true;
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe int[] GetObjects( ref GetObjectsInputData inputData )
		{
			const int tempSize = 4096;//!!!!
			int* temp = null;
			try
			{
				temp = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, sizeof( int ) * tempSize );
				if( GetObjects( ref inputData, temp, tempSize, out var count ) )
				{
					if( count > 0 )
					{
						int[] array = new int[ count ];
						fixed( int* pArray = array )
							NativeUtility.CopyMemory( pArray, temp, sizeof( int ) * count );
						return array;
					}
					else
						return Array.Empty<int>();
				}
				else
				{
					//create bigger array
					int[] array = new int[ count ];
					int count2;
					if( !GetObjects( ref inputData, array, out count2 ) )
						Log.Fatal( "OctreeCointainer: GetObjects: Internal error: !GetObjects( ref inputData, array, out count2 )." );
					if( count != count2 )
						Log.Fatal( "OctreeCointainer: GetObjects: Internal error: count != count2." );
					return array;
				}
			}
			finally
			{
				NativeUtility.Free( temp );
			}

			//var tempArray = getObjectsTempArrays.Get();
			//try
			//{
			//	int count;
			//	if( GetObjects( ref inputData, temp, out count ) )
			//	{
			//		int[] array = new int[ count ];
			//		Array.Copy( temp, 0, array, 0, count );
			//		return array;
			//	}
			//	else
			//	{
			//		//create bigger array
			//		int[] array = new int[ count ];
			//		int count2;
			//		zzzzz;
			//		if( !GetObjects( ref inputData, array, out count2 ) )
			//			Log.Fatal( "OctreeCointainer: GetObjects: Internal error: !GetObjects( ref inputData, array, out count2 )." );
			//		zzzzz;
			//		if( count != count2 )
			//			Log.Fatal( "OctreeCointainer: GetObjects: Internal error: count != count2." );
			//		return array;
			//	}
			//}
			//finally
			//{
			//	getObjectsTempArrays.Free( temp );
			//}


			//var tempArray = getObjectsTempArrays.Get();
			//try
			//{
			//	int count;
			//	if( GetObjects( ref inputData, tempArray, out count ) )
			//	{
			//		int[] array = new int[ count ];
			//		Array.Copy( tempArray, 0, array, 0, count );
			//		return array;
			//	}
			//	else
			//	{
			//		//create bigger array
			//		int[] array = new int[ count ];
			//		int count2;
			//		if( !GetObjects( ref inputData, array, out count2 ) )
			//			Log.Fatal( "OctreeCointainer: GetObjects: Internal error: !GetObjects( ref inputData, array, out count2 )." );
			//		if( count != count2 )
			//			Log.Fatal( "OctreeCointainer: GetObjects: Internal error: count != count2." );
			//		return array;
			//	}
			//}
			//finally
			//{
			//	getObjectsTempArrays.Free( tempArray );
			//}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool GetObjects( Bounds bounds, uint groupMask, ModeEnum mode, int[] outputArray, out int outputCount )
		{
			//fix invalid bounds
			Bounds fixedBounds = new Bounds( bounds.Minimum );
			fixedBounds.Add( bounds.Maximum );

			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Bounds;
			inputData.bounds = fixedBounds;
			return GetObjects( ref inputData, outputArray, out outputCount );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int[] GetObjects( Bounds bounds, uint groupMask, ModeEnum mode )
		{
			//fix invalid bounds
			Bounds fixedBounds = new Bounds( bounds.Minimum );
			fixedBounds.Add( bounds.Maximum );

			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Bounds;
			inputData.bounds = fixedBounds;
			return GetObjects( ref inputData );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool GetObjects( Sphere sphere, uint groupMask, ModeEnum mode, int[] outputArray, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Sphere;
			inputData.sphereCenter = sphere.Center;
			inputData.sphereRadius = sphere.Radius;
			//fix invalid radius
			if( inputData.sphereRadius < 0 )
				inputData.sphereRadius = -inputData.sphereRadius;
			return GetObjects( ref inputData, outputArray, out outputCount );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int[] GetObjects( Sphere sphere, uint groupMask, ModeEnum mode )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Sphere;
			inputData.sphereCenter = sphere.Center;
			inputData.sphereRadius = sphere.Radius;
			//fix invalid radius
			if( inputData.sphereRadius < 0 )
				inputData.sphereRadius = -inputData.sphereRadius;
			return GetObjects( ref inputData );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool GetObjects( Box box, uint groupMask, ModeEnum mode, int[] outputArray, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Box;
			inputData.box = box;
			return GetObjects( ref inputData, outputArray, out outputCount );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int[] GetObjects( Box box, uint groupMask, ModeEnum mode )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Box;
			inputData.box = box;
			return GetObjects( ref inputData );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool GetObjects( Frustum frustum, uint groupMask, ModeEnum mode, int[] outputArray, out int outputCount, IntPtr extensionData )
		{
			Plane[] planes = frustum.Planes;

			Bounds bounds = new Bounds( frustum.Points[ 0 ] );
			for( int n = 1; n < 8; n++ )
				bounds.Add( frustum.Points[ n ] );

			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			unsafe
			{
				fixed( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 1;
					inputData.bounds = bounds;
					inputData.extensionData = extensionData;
					return GetObjects( ref inputData, outputArray, out outputCount );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int[] GetObjects( Frustum frustum, uint groupMask, ModeEnum mode, IntPtr extensionData )
		{
			Plane[] planes = frustum.Planes;

			Bounds bounds = new Bounds( frustum.Points[ 0 ] );
			for( int n = 1; n < 8; n++ )
				bounds.Add( frustum.Points[ n ] );

			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			unsafe
			{
				fixed( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 1;
					inputData.bounds = bounds;
					inputData.extensionData = extensionData;
					return GetObjects( ref inputData );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool GetObjects( Plane[] planes, uint groupMask, int[] outputArray, ModeEnum mode, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			unsafe
			{
				fixed( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 0;
					return GetObjects( ref inputData, outputArray, out outputCount );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int[] GetObjects( Plane[] planes, uint groupMask, ModeEnum mode )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			unsafe
			{
				fixed( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 0;
					return GetObjects( ref inputData );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool GetObjects( Plane[] planes, Bounds bounds, uint groupMask, ModeEnum mode, int[] outputArray, out int outputCount, IntPtr extensionData )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			unsafe
			{
				fixed( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 1;
					inputData.bounds = bounds;
					inputData.extensionData = extensionData;
					return GetObjects( ref inputData, outputArray, out outputCount );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int[] GetObjects( Plane[] planes, Bounds bounds, uint groupMask, ModeEnum mode, IntPtr extensionData )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			unsafe
			{
				fixed( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 1;
					inputData.bounds = bounds;
					inputData.extensionData = extensionData;
					return GetObjects( ref inputData );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		unsafe bool GetObjectsRay( ref GetObjectsInputData inputData, GetObjectsRayOutputData[] outputArray, out int outputCount )
		{
			if( outputArray != null )
			{
				fixed( GetObjectsRayOutputData* pOutputArray = outputArray )
				{
					outputCount = OctreeContainer_GetObjectsRay( nativeObject, ref inputData, pOutputArray, outputArray.Length );
					if( outputCount <= outputArray.Length )
						return true;
					else
						return false;
				}
			}
			else
			{
				outputCount = OctreeContainer_GetObjectsRay( nativeObject, ref inputData, null, 0 );
				return true;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool GetObjects( Ray ray, uint groupMask, ModeEnum mode, GetObjectsRayOutputData[] outputArray, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.type = GetObjectsTypes.Ray;
			inputData.ray = ray;
			inputData.mode = mode;
			return GetObjectsRay( ref inputData, outputArray, out outputCount );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public GetObjectsRayOutputData[] GetObjects( Ray ray, uint groupMask, ModeEnum mode )
		{
			unsafe
			{
				GetObjectsInputData inputData = new GetObjectsInputData();
				inputData.groupMask = groupMask;
				inputData.type = GetObjectsTypes.Ray;
				inputData.ray = ray;
				inputData.mode = mode;

				const int tempSize = 1024;
				GetObjectsRayOutputData* temp = null;
				try
				{
					temp = (GetObjectsRayOutputData*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, sizeof( GetObjectsRayOutputData ) * tempSize );
					if( GetObjectsRay( ref inputData, temp, tempSize, out var count ) )
					{
						if( count > 0 )
						{
							var array = new GetObjectsRayOutputData[ count ];
							fixed( GetObjectsRayOutputData* pArray = array )
								NativeUtility.CopyMemory( pArray, temp, sizeof( GetObjectsRayOutputData ) * count );
							return array;
						}
						else
							return Array.Empty<GetObjectsRayOutputData>();
					}
					else
					{
						//create bigger array
						GetObjectsRayOutputData[] array = new GetObjectsRayOutputData[ count ];
						int count2;
						if( !GetObjectsRay( ref inputData, array, out count2 ) )
							Log.Fatal( "OctreeCointainer: GetObjects: Internal error: !GetObjects( ref inputData, array, out count2 )." );
						if( count != count2 )
							Log.Fatal( "OctreeCointainer: GetObjects: Internal error: count != count2." );
						return array;
					}
				}
				finally
				{
					NativeUtility.Free( temp );
				}
			}

			//GetObjectsInputData inputData = new GetObjectsInputData();
			//inputData.groupMask = groupMask;
			//inputData.type = GetObjectsTypes.Ray;
			//inputData.ray = ray;

			//var tempArray = getObjectsRayTempArrays.Get();
			//try
			//{
			//	int count;
			//	if( GetObjectsRay( ref inputData, tempArray, out count ) )
			//	{
			//		GetObjectsRayOutputData[] array = new GetObjectsRayOutputData[ count ];
			//		Array.Copy( tempArray, 0, array, 0, count );
			//		return array;
			//	}
			//	else
			//	{
			//		//create bigger array
			//		GetObjectsRayOutputData[] array = new GetObjectsRayOutputData[ count ];
			//		int count2;
			//		if( !GetObjectsRay( ref inputData, array, out count2 ) )
			//			Log.Fatal( "OctreeCointainer: GetObjects: Internal error: !GetObjects( ref inputData, array, out count2 )." );
			//		zzzz;//может измениться из другого потока
			//		if( count != count2 )
			//			Log.Fatal( "OctreeCointainer: GetObjects: Internal error: count != count2." );
			//		return array;
			//	}
			//}
			//finally
			//{
			//	getObjectsRayTempArrays.Free( tempArray );
			//}
		}

		public void GetStatistics( out int objectCount, out Bounds octreeBounds, out int octreeNodeCount, out double timeSinceLastFullRebuild )
		{
			OctreeContainer_GetStatistics( nativeObject, out objectCount, out octreeBounds, out octreeNodeCount, out timeSinceLastFullRebuild );
		}

		public void GetOctreeBoundsWithBoundsOfObjectsOutsideOctree( out Bounds bounds )
		{
			OctreeContainer_GetOctreeBoundsWithBoundsOfObjectsOutsideOctree( nativeObject, out bounds );
		}

		public Bounds GetOctreeBoundsWithBoundsOfObjectsOutsideOctree()
		{
			GetOctreeBoundsWithBoundsOfObjectsOutsideOctree( out var bounds );
			return bounds;
		}

		public void SetEngineTimeToGetStatistics( double engineTime )
		{
			OctreeContainer_SetEngineTimeToGetStatistics( nativeObject, engineTime );
		}
	}
}
