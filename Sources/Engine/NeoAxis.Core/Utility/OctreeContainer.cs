// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;

namespace NeoAxis
{
	/// <summary>
	/// The implementation of the Octree container.
	/// </summary>
	public class OctreeContainer
	{
		IntPtr nativeObject;

		//!!!!
		static ConcurrentArrayPool<int> getObjectsTempArrays = new ConcurrentArrayPool<int>( 4096 );
		static ConcurrentArrayPool<GetObjectsRayOutputData> getObjectsRayTempArrays = new ConcurrentArrayPool<GetObjectsRayOutputData>( 1024 );

		//

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr OctreeContainer_New( ref Bounds initialOctreeBounds, int amountOfObjectsOutsideOctreeBoundsToRebuld,
			ref Vector3 octreeBoundsRebuildExpand, ref Vector3 minNodeSize, int objectCountThresholdToCreateChildNodes, int maxNodeCount,
			int getObjectsInputDataSize );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern void OctreeContainer_Delete( IntPtr container );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern void OctreeContainer_UpdateSettings( IntPtr container, int amountOfObjectsOutsideOctreeBoundsToRebuld,
			ref Vector3 octreeBoundsRebuildExpand, ref Vector3 minNodeSize, int objectCountThresholdToCreateChildNodes, int maxNodeCount,
			[MarshalAs( UnmanagedType.U1 )] bool forceTreeRebuild );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern void OctreeContainer_RebuildTree( IntPtr container );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern int OctreeContainer_AddObject( IntPtr container, ref Vector3 boundsMin, ref Vector3 boundsMax, int group );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern void OctreeContainer_RemoveObject( IntPtr container, int objectIndex );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern void OctreeContainer_UpdateObject( IntPtr container, int objectIndex, ref Vector3 boundsMin,
			ref Vector3 boundsMax, int group );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe void OctreeContainer_GetDebugRenderLines( IntPtr container, out IntPtr outputData,
			out int outputDataItemCount );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe void OctreeContainer_Free( IntPtr container, IntPtr data );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe int OctreeContainer_GetObjects( IntPtr container, ref GetObjectsInputData inputData,
			int* outputArray, int outputArraySize );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe int OctreeContainer_GetObjectsRay( IntPtr container, ref GetObjectsInputData inputData,
			GetObjectsRayOutputData* outputArray, int outputArraySize );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe void OctreeContainer_GetStatistics( IntPtr container, out int objectCount, out Bounds octreeBounds,
			out int octreeNodeCount );

		[DllImport( OgreWrapper.library, CallingConvention = CallingConvention.Cdecl ), SuppressUnmanagedCodeSecurity]
		static extern unsafe void OctreeContainer_GetOctreeBoundsWithBoundsOfObjectsOutsideOctree( IntPtr container,
			out Bounds bounds );

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
		}

		///////////////////////////////////////////

		/// <summary>
		/// Represents returned data upon request from <see cref="OctreeContainer"/>.
		/// </summary>
		[StructLayout( LayoutKind.Sequential )]
		public struct GetObjectsRayOutputData
		{
			int objectIndex;
			double distanceNormalized;

			public GetObjectsRayOutputData( int objectIndex, double distanceNormalized )
			{
				this.objectIndex = objectIndex;
				this.distanceNormalized = distanceNormalized;
			}

			public int ObjectIndex
			{
				get { return objectIndex; }
				set { objectIndex = value; }
			}

			public double DistanceNormalized
			{
				get { return distanceNormalized; }
				set { distanceNormalized = value; }
			}
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
			internal int objectCountThresholdToCreateChildNodes = 10;
			internal int maxNodeCount = 300000;

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
		}

		///////////////////////////////////////////

		static bool neoAxisProprietaryLibraryLoaded;

		static void LoadNeoAxisProprietaryLibrary()
		{
			if( !neoAxisProprietaryLibraryLoaded )
			{
				neoAxisProprietaryLibraryLoaded = true;
				NativeLibraryManager.PreLoadLibrary( OgreWrapper.library );
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
					sizeof( GetObjectsInputData ) );
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

		public void UpdateSettings( int amountOfObjectsOutsideOctreeBoundsToRebuld, Vector3 octreeBoundsRebuildExpand,
			Vector3 minNodeSize, int objectCountThresholdToCreateChildNodes, int maxNodeCount, bool forceTreeRebuild )
		{
			OctreeContainer_UpdateSettings( nativeObject, amountOfObjectsOutsideOctreeBoundsToRebuld, ref octreeBoundsRebuildExpand,
				ref minNodeSize, objectCountThresholdToCreateChildNodes, maxNodeCount, forceTreeRebuild );
		}

		public void RebuildTree()
		{
			OctreeContainer_RebuildTree( nativeObject );
		}

		public int AddObject( Bounds bounds, int group )
		{
			if( bounds.IsCleared() )
				Log.Fatal( "OctreeContainer: AddObject: Invalid bounds." );
			if( group < 0 )
				Log.Fatal( "OctreeContainer: AddObject: group < 0." );
			if( group >= 32 )
				Log.Fatal( "OctreeContainer: AddObject: group >= 32." );

			Vector3 boundsMin = bounds.Minimum;
			Vector3 boundsMax = bounds.Maximum;
			return OctreeContainer_AddObject( nativeObject, ref boundsMin, ref boundsMax, group );
		}

		public void RemoveObject( int objectIndex )
		{
			OctreeContainer_RemoveObject( nativeObject, objectIndex );
		}

		public void UpdateObject( int objectIndex, Bounds bounds, int group )
		{
			if( bounds.IsCleared() )
				Log.Fatal( "OctreeContainer: UpdateObject: Invalid bounds." );
			if( group < 0 )
				Log.Fatal( "OctreeContainer: UpdateObject: group < 0." );
			if( group >= 32 )
				Log.Fatal( "OctreeContainer: UpdateObject: group >= 32." );

			Vector3 boundsMin = bounds.Minimum;
			Vector3 boundsMax = bounds.Maximum;
			OctreeContainer_UpdateObject( nativeObject, objectIndex, ref boundsMin, ref boundsMax, group );
		}

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

					//!!!!new ColorValue( 1, 1, 1, 0.25f )
					//viewport.Simple3DRenderer.SetColor( color, color * new ColorValue( 1, 1, 1, 0.25f ) );
					viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier * new ColorValue( 1, 1, 1, 0.5 ) );
					viewport.Simple3DRenderer.AddLineThin( lines[ n ].start, lines[ n ].end );
					//camera.DebugGeometry.AddBounds( new Bounds( lines[ n ].start, lines[ n ].end ) );
				}

				OctreeContainer_Free( nativeObject, data );
			}
		}

		/////////////////////////////////////////

		unsafe bool GetObjects( ref GetObjectsInputData inputData, int* outputArray, int outputArrayLength, out int outputCount )
		{
			outputCount = OctreeContainer_GetObjects( nativeObject, ref inputData, outputArray, outputArrayLength );
			if( outputCount <= outputArrayLength )
				return true;
			else
				return false;
		}

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

		public unsafe bool GetObjects( Sphere sphere, uint groupMask, ModeEnum mode, int* outputArray, int outputArrayLength, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Sphere;
			inputData.sphereCenter = sphere.Origin;
			inputData.sphereRadius = sphere.Radius;
			//fix invalid radius
			if( inputData.sphereRadius < 0 )
				inputData.sphereRadius = -inputData.sphereRadius;
			return GetObjects( ref inputData, outputArray, outputArrayLength, out outputCount );
		}

		public unsafe bool GetObjects( Box box, uint groupMask, ModeEnum mode, int* outputArray, int outputArrayLength, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Box;
			inputData.box = box;
			return GetObjects( ref inputData, outputArray, outputArrayLength, out outputCount );
		}

		public unsafe bool GetObjects( Plane[] planes, uint groupMask, ModeEnum mode, int* outputArray, int outputArrayLength, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			fixed ( Plane* pPlanes = planes )
			{
				inputData.planes = (IntPtr)pPlanes;
				inputData.planesUseAdditionalBounds = 0;
				return GetObjects( ref inputData, outputArray, outputArrayLength, out outputCount );
			}
		}

		public unsafe bool GetObjects( Plane[] planes, Bounds bounds, uint groupMask, ModeEnum mode, int* outputArray, int outputArrayLength, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			fixed ( Plane* pPlanes = planes )
			{
				inputData.planes = (IntPtr)pPlanes;
				inputData.planesUseAdditionalBounds = 1;
				inputData.bounds = bounds;
				return GetObjects( ref inputData, outputArray, outputArrayLength, out outputCount );
			}
		}

		public unsafe bool GetObjects( Frustum frustum, uint groupMask, ModeEnum mode, int* outputArray, int outputArrayLength, out int outputCount )
		{
			Plane[] planes = frustum.Planes;
			Bounds bounds = new Bounds( frustum.Points[ 0 ] );
			for( int n = 1; n < 8; n++ )
				bounds.Add( frustum.Points[ n ] );
			return GetObjects( planes, bounds, groupMask, mode, outputArray, outputArrayLength, out outputCount );
		}

		unsafe bool GetObjectsRay( ref GetObjectsInputData inputData, GetObjectsRayOutputData* outputArray, int outputArrayLength, out int outputCount )
		{
			outputCount = OctreeContainer_GetObjectsRay( nativeObject, ref inputData, outputArray, outputArrayLength );
			if( outputCount <= outputArrayLength )
				return true;
			else
				return false;
		}

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

		unsafe bool GetObjects( ref GetObjectsInputData inputData, int[] outputArray, out int outputCount )
		{
			if( outputArray != null )
			{
				fixed ( int* pOutputArray = outputArray )
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

		unsafe int[] GetObjects( ref GetObjectsInputData inputData )
		{
			var tempArray = getObjectsTempArrays.Get();
			try
			{
				int count;
				if( GetObjects( ref inputData, tempArray, out count ) )
				{
					int[] array = new int[ count ];
					Array.Copy( tempArray, 0, array, 0, count );
					return array;
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
				getObjectsTempArrays.Free( tempArray );
			}
		}

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

		public bool GetObjects( Sphere sphere, uint groupMask, ModeEnum mode, int[] outputArray, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Sphere;
			inputData.sphereCenter = sphere.Origin;
			inputData.sphereRadius = sphere.Radius;
			//fix invalid radius
			if( inputData.sphereRadius < 0 )
				inputData.sphereRadius = -inputData.sphereRadius;
			return GetObjects( ref inputData, outputArray, out outputCount );
		}

		public int[] GetObjects( Sphere sphere, uint groupMask, ModeEnum mode )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Sphere;
			inputData.sphereCenter = sphere.Origin;
			inputData.sphereRadius = sphere.Radius;
			//fix invalid radius
			if( inputData.sphereRadius < 0 )
				inputData.sphereRadius = -inputData.sphereRadius;
			return GetObjects( ref inputData );
		}

		public bool GetObjects( Box box, uint groupMask, ModeEnum mode, int[] outputArray, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Box;
			inputData.box = box;
			return GetObjects( ref inputData, outputArray, out outputCount );
		}

		public int[] GetObjects( Box box, uint groupMask, ModeEnum mode )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Box;
			inputData.box = box;
			return GetObjects( ref inputData );
		}

		public bool GetObjects( Frustum frustum, uint groupMask, ModeEnum mode, int[] outputArray, out int outputCount )
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
				fixed ( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 1;
					inputData.bounds = bounds;
					return GetObjects( ref inputData, outputArray, out outputCount );
				}
			}
		}

		public int[] GetObjects( Frustum frustum, uint groupMask, ModeEnum mode )
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
				fixed ( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 1;
					inputData.bounds = bounds;
					return GetObjects( ref inputData );
				}
			}
		}

		public bool GetObjects( Plane[] planes, uint groupMask, int[] outputArray, ModeEnum mode, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			unsafe
			{
				fixed ( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 0;
					return GetObjects( ref inputData, outputArray, out outputCount );
				}
			}
		}

		public int[] GetObjects( Plane[] planes, uint groupMask, ModeEnum mode )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			unsafe
			{
				fixed ( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 0;
					return GetObjects( ref inputData );
				}
			}
		}

		public bool GetObjects( Plane[] planes, Bounds bounds, uint groupMask, ModeEnum mode, int[] outputArray, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			unsafe
			{
				fixed ( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 1;
					inputData.bounds = bounds;
					return GetObjects( ref inputData, outputArray, out outputCount );
				}
			}
		}

		public int[] GetObjects( Plane[] planes, Bounds bounds, uint groupMask, ModeEnum mode )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.mode = mode;
			inputData.type = GetObjectsTypes.Planes;
			inputData.planeCount = planes.Length;
			unsafe
			{
				fixed ( Plane* pPlanes = planes )
				{
					inputData.planes = (IntPtr)pPlanes;
					inputData.planesUseAdditionalBounds = 1;
					inputData.bounds = bounds;
					return GetObjects( ref inputData );
				}
			}
		}

		unsafe bool GetObjectsRay( ref GetObjectsInputData inputData, GetObjectsRayOutputData[] outputArray, out int outputCount )
		{
			if( outputArray != null )
			{
				fixed ( GetObjectsRayOutputData* pOutputArray = outputArray )
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

		public bool GetObjects( Ray ray, uint groupMask, GetObjectsRayOutputData[] outputArray, out int outputCount )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.type = GetObjectsTypes.Ray;
			inputData.ray = ray;
			return GetObjectsRay( ref inputData, outputArray, out outputCount );
		}

		public GetObjectsRayOutputData[] GetObjects( Ray ray, uint groupMask )
		{
			GetObjectsInputData inputData = new GetObjectsInputData();
			inputData.groupMask = groupMask;
			inputData.type = GetObjectsTypes.Ray;
			inputData.ray = ray;

			var tempArray = getObjectsRayTempArrays.Get();
			try
			{
				int count;
				if( GetObjectsRay( ref inputData, tempArray, out count ) )
				{
					GetObjectsRayOutputData[] array = new GetObjectsRayOutputData[ count ];
					Array.Copy( tempArray, 0, array, 0, count );
					return array;
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
				getObjectsRayTempArrays.Free( tempArray );
			}
		}

		public void GetStatistics( out int objectCount, out Bounds octreeBounds, out int octreeNodeCount )
		{
			OctreeContainer_GetStatistics( nativeObject, out objectCount, out octreeBounds, out octreeNodeCount );
		}

		public Bounds GetOctreeBoundsWithBoundsOfObjectsOutsideOctree()
		{
			Bounds bounds;
			OctreeContainer_GetOctreeBoundsWithBoundsOfObjectsOutsideOctree( nativeObject, out bounds );
			return bounds;
		}
	}
}
