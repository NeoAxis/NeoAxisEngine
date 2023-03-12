// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NeoAxis
{
	public partial class Scene
	{
		bool octreeCanCreate;
		OctreeContainer octree;
		List<ObjectInSpace> octreeObjects = new List<ObjectInSpace>();

		/////////////////////////////////////////

		/// <summary>
		/// Represents an item for <see cref="Scene.GetObjectsInSpace(IList{GetObjectsInSpaceItem})"/> method.
		/// </summary>
		public class GetObjectsInSpaceItem
		{
			//!!!!don't forget about Clone method

			//general
			public CastTypeEnum CastType;
			public Metadata.TypeInfo SelectedTypeOnly;
			public bool VisibleOnly;

			//shape
			public Bounds? Bounds;
			public Sphere? Sphere;
			public Box? Box;
			public Plane[] Planes;
			public Frustum Frustum;
			public Ray? Ray;

			//output
			public ResultItem[] Result;
			//volatile ResultItem[] result;
			//public ResultItem[] Result { get { return result; } set { result = value; } }

			//addition
			public object UserData;
			public IntPtr ExtensionData;
			public uint GroupMask = 0xFFFFFFFF;

			//bool checkChildrenOfObjectsInSpace;

			////////////////

			public enum CastTypeEnum
			{
				One,
				OneClosest,
				All,
			}

			////////////////

			/// <summary>
			/// Represents resulting data item of <see cref="GetObjectsInSpaceItem"/>.
			/// </summary>
			public struct ResultItem
			{
				public ObjectInSpace Object;
				//ray specific
				public Vector3 Position;
				public double DistanceScale;
			}

			////////////////

			[StructLayout( LayoutKind.Sequential )]
			public struct ExtensionDataStructure
			{
				//public int Mode;
				public IntPtr OcclusionCullingBuffer;
				//!!!!double
				public Matrix4F ViewProjectionMatrix;
				public int OcclusionCullingBufferCullNodes;
				public int OcclusionCullingBufferCullObjects;
			}

			////////////////

			public GetObjectsInSpaceItem()
			{
			}

			public GetObjectsInSpaceItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Bounds bounds )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Bounds = bounds;
			}

			public GetObjectsInSpaceItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Box box )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Box = box;
			}

			public GetObjectsInSpaceItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Sphere sphere )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Sphere = sphere;
			}

			public GetObjectsInSpaceItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Plane[] planes )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Planes = planes;
			}

			public GetObjectsInSpaceItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Plane[] planes, Bounds bounds )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Planes = planes;
				this.Bounds = bounds;
			}

			public GetObjectsInSpaceItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Frustum frustum )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Frustum = frustum;
			}

			public GetObjectsInSpaceItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Ray ray )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Ray = ray;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			bool BoundsPlanesIntersects( ref Bounds bounds, Plane[] planes )
			{
				bounds.GetCenter( out var boundsCenter );
				Vector3.Subtract( ref bounds.Maximum, ref boundsCenter, out var boundsHalfSize );
				for( int n = 0; n < planes.Length; n++ )
					if( planes[ n ].GetSide( ref boundsCenter, ref boundsHalfSize ) == Plane.Side.Positive )
						return false;
				return true;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public bool Intersects( ref Bounds bounds )
			{
				if( Bounds != null && Planes != null )
				{
					//Planes + Bounds
					if( Bounds.Value.Intersects( ref bounds ) || BoundsPlanesIntersects( ref bounds, Planes ) )
						return true;
				}
				else if( Planes != null )
				{
					//Planes
					if( BoundsPlanesIntersects( ref bounds, Planes ) )
						return true;
				}
				else if( Frustum != null )
				{
					//Frustum
					Bounds frustumBounds = new Bounds( Frustum.Points[ 0 ] );
					for( int n = 1; n < 8; n++ )
						frustumBounds.Add( Frustum.Points[ n ] );
					if( frustumBounds.Intersects( ref bounds ) || BoundsPlanesIntersects( ref bounds, Frustum.Planes ) )
						return true;
				}
				else if( Bounds != null )
				{
					//Bounds
					if( Bounds.Value.Intersects( ref bounds ) )
						return true;
				}
				else if( Box != null )
				{
					//Box
					if( Box.Value.Intersects( ref bounds ) )
						return true;
				}
				else if( Sphere != null )
				{
					//Sphere
					if( Sphere.Value.Intersects( ref bounds ) )
						return true;
				}
				else if( Ray != null )
				{
					//Ray
					if( bounds.Intersects( Ray.Value ) )
						return true;
				}

				return false;
			}

			public GetObjectsInSpaceItem Clone()
			{
				var result = new GetObjectsInSpaceItem();

				//general
				result.CastType = CastType;
				result.SelectedTypeOnly = SelectedTypeOnly;
				result.VisibleOnly = VisibleOnly;

				//shape
				result.Bounds = Bounds;
				result.Sphere = Sphere;
				result.Box = Box;
				result.Planes = Planes;
				result.Frustum = Frustum;
				result.Ray = Ray;

				//output
				result.Result = Result;

				//addition
				result.UserData = UserData;
				result.ExtensionData = ExtensionData;
				result.GroupMask = GroupMask;

				return result;
			}
		}

		/////////////////////////////////////////

		[Flags]
		public enum SceneObjectFlags
		{
			Logic = 1,
			Visual = 2,
			Occluder = 4,

			Custom1 = 8,
			Custom2 = 16,
			Custom3 = 32,
			Custom4 = 64,
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void ObjectsInSpace_ObjectUpdateBounds( ObjectInSpace obj )
		{
			if( octree != null )
			{
				var bounds = obj.SpaceBoundsOctreeOverride.HasValue ? obj.SpaceBoundsOctreeOverride.Value : obj.SpaceBounds.boundingBox;

				if( obj.sceneOctreeIndex == -1 )
				{
					//add object to the scene graph
					obj.sceneOctreeIndex = octree.AddObject( ref bounds, (uint)obj.PerformGetSceneObjectFlags() );

					//add to sceneGraphObjects
					while( octreeObjects.Count <= obj.sceneOctreeIndex )
						octreeObjects.Add( null );
					octreeObjects[ obj.sceneOctreeIndex ] = obj;
				}
				else
				{
					//update
					octree.UpdateObjectBounds( obj.sceneOctreeIndex, ref bounds );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void ObjectsInSpace_ObjectUpdateGroupMask( ObjectInSpace obj )
		{
			if( octree != null )
			{
				if( obj.sceneOctreeIndex == -1 )
				{
					var bounds = obj.SpaceBoundsOctreeOverride.HasValue ? obj.SpaceBoundsOctreeOverride.Value : obj.SpaceBounds.boundingBox;

					//add object to the scene graph
					obj.sceneOctreeIndex = octree.AddObject( ref bounds, (uint)obj.PerformGetSceneObjectFlags() );

					//add to sceneGraphObjects
					while( octreeObjects.Count <= obj.sceneOctreeIndex )
						octreeObjects.Add( null );
					octreeObjects[ obj.sceneOctreeIndex ] = obj;
				}
				else
				{
					//update
					octree.UpdateObjectGroupMask( obj.sceneOctreeIndex, (uint)obj.PerformGetSceneObjectFlags() );
				}
			}
		}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//internal void ObjectsInSpace_ObjectUpdate( ObjectInSpace obj )
		//{
		//	if( octree != null )
		//	{
		//		var groupMask = (uint)obj.PerformGetSceneObjectFlags();
		//		//uint groupMask = 0x1;
		//		//if( obj.PerformOcclusionCullingDataContains() )
		//		//	groupMask |= 0x2;

		//		var bounds = obj.SpaceBoundsOctreeOverride.HasValue ? obj.SpaceBoundsOctreeOverride.Value : obj.SpaceBounds.CalculatedBoundingBox;

		//		if( obj.sceneOctreeIndex == -1 )
		//		{
		//			//add object to the scene graph
		//			obj.sceneOctreeIndex = octree.AddObject( ref bounds, groupMask );// obj.sceneOctreeGroup );

		//			//add to sceneGraphObjects
		//			while( octreeObjects.Count <= obj.sceneOctreeIndex )
		//				octreeObjects.Add( null );
		//			octreeObjects[ obj.sceneOctreeIndex ] = obj;
		//		}
		//		else
		//		{
		//			//update
		//			octree.UpdateObject( obj.sceneOctreeIndex, ref bounds, groupMask );// obj.sceneOctreeGroup );
		//		}
		//	}
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void ObjectsInSpace_ObjectRemove( ObjectInSpace obj )
		{
			if( octree != null && obj.sceneOctreeIndex != -1 )
			{
				octreeObjects[ obj.sceneOctreeIndex ] = null;
				octree.RemoveObject( obj.sceneOctreeIndex );
				obj.sceneOctreeIndex = -1;
			}
		}

		void ObjectsInSpace_CreateOctree()
		{
			if( octree != null )
				Log.Fatal( "Scene: ObjectsInSpace_CreateOctree: sceneOctree != null." );

			//create octree
			var settings = new OctreeContainer.InitSettings();
			settings.InitialOctreeBounds = CalculateTotalBoundsOfObjectsInSpace( true );
			settings.AmountOfObjectsOutsideOctreeBoundsToRebuld = OctreeObjectCountOutsideOctreeToRebuld;
			settings.OctreeBoundsRebuildExpand = OctreeBoundsRebuildExpand;
			settings.MinNodeSize = OctreeMinNodeSize;
			settings.ObjectCountThresholdToCreateChildNodes = OctreeObjectCountToCreateChildNodes;
			settings.MaxNodeCount = OctreeMaxNodeCount;
			settings.ThreadingMode = OctreeThreadingMode;
			octree = new OctreeContainer( settings );

			//update objects
			foreach( var obj in GetComponents<ObjectInSpace>( false, true, true ) )
				ObjectsInSpace_ObjectUpdateBounds( obj );

			octree?.SetEngineTimeToGetStatistics( EngineApp.EngineTime );
		}

		void ObjectsInSpace_DestroyOctree()
		{
			if( octree != null )
			{
				for( int n = 0; n < octreeObjects.Count; n++ )
				{
					var obj = octreeObjects[ n ];
					if( obj != null )
						obj.sceneOctreeIndex = -1;
				}
				octreeObjects.Clear();

				octree.Dispose();
				octree = null;
			}
		}

		public void OctreeUpdate( bool forceRebuild )
		{
			if( octreeCanCreate && OctreeEnabled )
			{
				if( octree == null )
				{
					ObjectsInSpace_CreateOctree();
				}
				else
				{
					if( forceRebuild )
					{
						ObjectsInSpace_DestroyOctree();
						ObjectsInSpace_CreateOctree();
					}
					//else
					//{
					//	octree.UpdateSettings( OctreeObjectCountOutsideOctreeToRebuld, OctreeBoundsRebuildExpand, OctreeMinNodeSize, OctreeObjectCountToCreateChildNodes, OctreeMaxNodeCount, OctreeThreadingMode );//, false );
					//}
				}
			}
			else
				ObjectsInSpace_DestroyOctree();

			octree?.SetEngineTimeToGetStatistics( EngineApp.EngineTime );
		}

		public delegate void GetObjectsInSpaceOverrideDelegate( Scene scene, IList<GetObjectsInSpaceItem> items, ref bool handled );
		public event GetObjectsInSpaceOverrideDelegate GetObjectsInSpaceOverride;

		public void GetObjectsInSpace( IList<GetObjectsInSpaceItem> items )//, bool disableOctree = false )
		{
			if( items.Count == 0 )
				return;

			bool handled = false;
			GetObjectsInSpaceOverride?.Invoke( this, items, ref handled );
			if( handled )
				return;

			if( octree != null )//&& !disableOctree )
				GetObjectsInSpace_FromOctree( items );
			else
				GetObjectsInSpace_NoOctree( items );
		}

		public void GetObjectsInSpace( GetObjectsInSpaceItem item )//, bool disableOctree = false )
		{
			GetObjectsInSpace( new GetObjectsInSpaceItem[] { item } );//, disableOctree );
		}

		unsafe GetObjectsInSpaceItem.ResultItem[] GetObjectsInSpace_FromOctree_GetResult( GetObjectsInSpaceItem item, int* array, int outputCount )
		{
			var toAdd = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, outputCount * 4 );
			NativeUtility.ZeroMemory( toAdd, outputCount * 4 );
			var toAdd2 = (int*)toAdd;

			try
			{
				int length = 0;
				for( int n = 0; n < outputCount; n++ )
				{
					var obj = octreeObjects[ array[ n ] ];

					if( item.VisibleOnly && !obj.VisibleInHierarchy )
						continue;
					if( item.SelectedTypeOnly != null && !item.SelectedTypeOnly.IsAssignableFrom( obj.BaseType ) )
						continue;

					toAdd2[ n ] = 1;
					length++;

					if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.One )
						break;
				}

				if( length != 0 )
				{
					var result = new GetObjectsInSpaceItem.ResultItem[ length ];
					int current = 0;

					for( int n = 0; n < outputCount; n++ )
					{
						var obj = octreeObjects[ array[ n ] ];

						if( toAdd2[ n ] == 0 )
							continue;

						ref var resultItem = ref result[ current ];
						resultItem.Object = obj;
						current++;

						if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.One )
							break;
					}

					if( length != current )
						Log.Fatal( "Scene: GetObjectsInSpace_FromOctree_GetResult: Internal error. length != current." );

					return result;
				}
				else
					return Array.Empty<GetObjectsInSpaceItem.ResultItem>();
			}
			finally
			{
				NativeUtility.Free( toAdd );
			}


			//if( GetObjectsInSpace_FromOctree_GetResult_List == null )
			//	GetObjectsInSpace_FromOctree_GetResult_List = new List<GetObjectsInSpaceItem.ResultItem>( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.All ? outputCount : 4 );
			//var list = GetObjectsInSpace_FromOctree_GetResult_List;

			//try
			//{
			//	//List<GetObjectsInSpaceItem.ResultItem> resultList = null;

			//	for( int n = 0; n < outputCount; n++ )
			//	{
			//		var obj = octreeObjects[ array[ n ] ];

			//		if( item.VisibleOnly && !obj.VisibleInHierarchy )
			//			continue;
			//		if( item.SelectedTypeOnly != null && !item.SelectedTypeOnly.IsAssignableFrom( obj.BaseType ) )
			//			continue;

			//		//if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.Closest && resultList.Count != 0 )
			//		//{
			//		//	if( scale < resultList[ 0 ].DistanceScale )
			//		//		resultList.Clear();
			//		//	else
			//		//		continue;
			//		//}

			//		var resultItem = new GetObjectsInSpaceItem.ResultItem();
			//		resultItem.Object = obj;
			//		//resultItem.Position = item.Ray.Value.GetPointOnRay( scale );
			//		//resultItem.DistanceScale = scale;
			//		list.Add( resultItem );
			//		//if( resultList == null )
			//		//	resultList = new List<GetObjectsInSpaceItem.ResultItem>( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.All ? outputCount : 1 );
			//		//resultList.Add( resultItem );

			//		if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.One )
			//			break;
			//	}

			//	if( list.Count != 0 )
			//		return list.ToArray();
			//	else
			//		return Array.Empty<GetObjectsInSpaceItem.ResultItem>();
			//	//if( resultList != null )
			//	//	return resultList.ToArray();
			//	//else
			//	//	return Array.Empty<GetObjectsInSpaceItem.ResultItem>();
			//}
			//finally
			//{
			//	list.Clear();
			//}
		}

		unsafe GetObjectsInSpaceItem.ResultItem[] GetObjectsInSpace_FromOctree_GetResultRay( GetObjectsInSpaceItem item, OctreeContainer.GetObjectsRayOutputData* array, int outputCount )
		{
			//!!!!slowly

			List<GetObjectsInSpaceItem.ResultItem> resultList = null;

			for( int n = 0; n < outputCount; n++ )
			{
				var obj = octreeObjects[ array[ n ].ObjectIndex ];

				if( item.VisibleOnly && !obj.VisibleInHierarchy )
					continue;
				if( item.SelectedTypeOnly != null && !item.SelectedTypeOnly.IsAssignableFrom( obj.BaseType ) )
					continue;

				var scale = array[ n ].DistanceNormalized;

				if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.OneClosest && resultList != null )
				{
					if( scale < resultList[ 0 ].DistanceScale )
						resultList.Clear();
					else
						continue;
				}

				var resultItem = new GetObjectsInSpaceItem.ResultItem();
				resultItem.Object = obj;
				resultItem.Position = item.Ray.Value.GetPointOnRay( scale );
				resultItem.DistanceScale = scale;
				if( resultList == null )
					resultList = new List<GetObjectsInSpaceItem.ResultItem>( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.All ? outputCount : 1 );
				resultList.Add( resultItem );

				if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.One )
					break;
			}

			if( resultList != null )
				return resultList.ToArray();
			else
				return Array.Empty<GetObjectsInSpaceItem.ResultItem>();
		}

		unsafe void GetObjectsInSpace_FromOctree( IList<GetObjectsInSpaceItem> items )
		{
			void Calculate( int itemIndex )
			{
				var item = items[ itemIndex ];

				//!!!!что с bounding sphere?

				var groupMask = item.GroupMask;

				GetObjectsInSpaceItem.ResultItem[] resultArray = null;

				//!!!!
				int arrayLength = 8192;

				if( item.Planes != null && item.Bounds != null )
				{
					//Planes & Bounds
					var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, sizeof( int ) * arrayLength );
					if( octree.GetObjects( item.Planes, item.Bounds.Value, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount, item.ExtensionData ) )
						resultArray = GetObjectsInSpace_FromOctree_GetResult( item, array, outputCount );
					else
					{
						var array2 = new int[ outputCount ];
						fixed( int* pArray2 = array2 )
							if( octree.GetObjects( item.Planes, item.Bounds.Value, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2, item.ExtensionData ) )
								resultArray = GetObjectsInSpace_FromOctree_GetResult( item, pArray2, outputCount );
					}
					NativeUtility.Free( array );
				}
				else if( item.Planes != null )
				{
					//Planes
					var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, sizeof( int ) * arrayLength );
					if( octree.GetObjects( item.Planes, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
						resultArray = GetObjectsInSpace_FromOctree_GetResult( item, array, outputCount );
					else
					{
						var array2 = new int[ outputCount ];
						fixed( int* pArray2 = array2 )
							if( octree.GetObjects( item.Planes, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
								resultArray = GetObjectsInSpace_FromOctree_GetResult( item, pArray2, outputCount );
					}
					NativeUtility.Free( array );
				}
				else if( item.Frustum != null )
				{
					//Frustum
					var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, sizeof( int ) * arrayLength );
					if( octree.GetObjects( item.Frustum, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount, item.ExtensionData ) )
						resultArray = GetObjectsInSpace_FromOctree_GetResult( item, array, outputCount );
					else
					{
						var array2 = new int[ outputCount ];
						fixed( int* pArray2 = array2 )
							if( octree.GetObjects( item.Frustum, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2, item.ExtensionData ) )
								resultArray = GetObjectsInSpace_FromOctree_GetResult( item, pArray2, outputCount );
					}
					NativeUtility.Free( array );
				}
				else if( item.Bounds != null )
				{
					//Bounds
					var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, sizeof( int ) * arrayLength );
					if( octree.GetObjects( item.Bounds.Value, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
						resultArray = GetObjectsInSpace_FromOctree_GetResult( item, array, outputCount );
					else
					{
						var array2 = new int[ outputCount ];
						fixed( int* pArray2 = array2 )
							if( octree.GetObjects( item.Bounds.Value, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
								resultArray = GetObjectsInSpace_FromOctree_GetResult( item, pArray2, outputCount );
					}
					NativeUtility.Free( array );
				}
				else if( item.Box != null )
				{
					//Box
					var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, sizeof( int ) * arrayLength );
					if( octree.GetObjects( item.Box.Value, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
						resultArray = GetObjectsInSpace_FromOctree_GetResult( item, array, outputCount );
					else
					{
						var array2 = new int[ outputCount ];
						fixed( int* pArray2 = array2 )
							if( octree.GetObjects( item.Box.Value, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
								resultArray = GetObjectsInSpace_FromOctree_GetResult( item, pArray2, outputCount );
					}
					NativeUtility.Free( array );
				}
				else if( item.Sphere != null )
				{
					//Sphere
					var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, sizeof( int ) * arrayLength );
					if( octree.GetObjects( item.Sphere.Value, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
						resultArray = GetObjectsInSpace_FromOctree_GetResult( item, array, outputCount );
					else
					{
						var array2 = new int[ outputCount ];
						fixed( int* pArray2 = array2 )
							if( octree.GetObjects( item.Sphere.Value, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
								resultArray = GetObjectsInSpace_FromOctree_GetResult( item, pArray2, outputCount );
					}
					NativeUtility.Free( array );
				}
				else if( item.Ray != null )
				{
					//Ray
					var array = (OctreeContainer.GetObjectsRayOutputData*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, sizeof( OctreeContainer.GetObjectsRayOutputData ) * arrayLength );
					if( octree.GetObjects( item.Ray.Value, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
						resultArray = GetObjectsInSpace_FromOctree_GetResultRay( item, array, outputCount );
					else
					{
						var array2 = new OctreeContainer.GetObjectsRayOutputData[ outputCount ];
						fixed( OctreeContainer.GetObjectsRayOutputData* pArray2 = array2 )
							if( octree.GetObjects( item.Ray.Value, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
								resultArray = GetObjectsInSpace_FromOctree_GetResultRay( item, pArray2, outputCount );
					}
					NativeUtility.Free( array );
				}

				if( resultArray == null )
					resultArray = Array.Empty<GetObjectsInSpaceItem.ResultItem>();

				//Ray: sort
				if( item.Ray != null && item.CastType == GetObjectsInSpaceItem.CastTypeEnum.All )
				{
					//!!!!slowly?
					CollectionUtility.MergeSort( resultArray, delegate ( GetObjectsInSpaceItem.ResultItem v1, GetObjectsInSpaceItem.ResultItem v2 )
					{
						if( v1.DistanceScale < v2.DistanceScale )
							return -1;
						if( v1.DistanceScale > v2.DistanceScale )
							return 1;
						return 0;
					} );
				}

				item.Result = resultArray;
			}

			if( items.Count > 1 )
				Parallel.For( 0, items.Count, Calculate );
			else
				Calculate( 0 );
		}

		bool GetObjectsInSpace_NoOctree_BoundsPlanesIntersects( Bounds bounds, Plane[] planes )
		{
			var boundsCenter = bounds.GetCenter();
			var boundsHalfSize = bounds.Maximum - boundsCenter;
			foreach( var plane in planes )
				if( plane.GetSide( boundsCenter, boundsHalfSize ) == Plane.Side.Positive )
					return false;
			return true;
		}

		void GetObjectsInSpace_NoOctree( IList<GetObjectsInSpaceItem> items )
		{
			foreach( var item in items )
			{
				var resultList = new List<GetObjectsInSpaceItem.ResultItem>( 64 );
				foreach( var obj in GetComponents<ObjectInSpace>( false, true, true ) )
				{
					if( item.VisibleOnly && !obj.VisibleInHierarchy )
						continue;
					if( item.SelectedTypeOnly != null && !item.SelectedTypeOnly.IsAssignableFrom( obj.BaseType ) )
						continue;

					//!!!!что с bounding sphere?

					var bounds = obj.SpaceBounds.boundingBox;

					//!!!!если уже внутри бокса, то вроде как не надо (не выделять в редакторе)

					if( item.Bounds != null && item.Planes != null )
					{
						//Planes + Bounds
						if( item.Bounds.Value.Intersects( ref bounds ) || GetObjectsInSpace_NoOctree_BoundsPlanesIntersects( bounds, item.Planes ) )
						{
							var resultItem = new GetObjectsInSpaceItem.ResultItem();
							resultItem.Object = obj;
							resultList.Add( resultItem );

							if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.One )
								break;
						}
					}
					else if( item.Planes != null )
					{
						//Planes
						if( GetObjectsInSpace_NoOctree_BoundsPlanesIntersects( bounds, item.Planes ) )
						{
							var resultItem = new GetObjectsInSpaceItem.ResultItem();
							resultItem.Object = obj;
							resultList.Add( resultItem );

							if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.One )
								break;
						}
					}
					else if( item.Frustum != null )
					{
						//Frustum
						Plane[] planes = item.Frustum.Planes;
						Bounds frustumBounds = new Bounds( item.Frustum.Points[ 0 ] );
						for( int n = 1; n < 8; n++ )
							frustumBounds.Add( item.Frustum.Points[ n ] );
						if( frustumBounds.Intersects( ref bounds ) || GetObjectsInSpace_NoOctree_BoundsPlanesIntersects( bounds, planes ) )
						{
							var resultItem = new GetObjectsInSpaceItem.ResultItem();
							resultItem.Object = obj;
							resultList.Add( resultItem );

							if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.One )
								break;
						}
					}
					else if( item.Bounds != null )
					{
						//Bounds
						if( item.Bounds.Value.Intersects( ref bounds ) )
						{
							//if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.Closest && resultList.Count != 0 )
							//{
							//	if( scale < resultList[ 0 ].DistanceScale )
							//		resultList.Clear();
							//	else
							//		continue;
							//}

							var resultItem = new GetObjectsInSpaceItem.ResultItem();
							resultItem.Object = obj;
							//resultItem.Position = item.Ray.Value.GetPointOnRay( scale );
							//resultItem.DistanceScale = scale;
							resultList.Add( resultItem );

							if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.One )
								break;
						}
					}
					else if( item.Box != null )
					{
						//Box
						if( item.Box.Value.Intersects( ref bounds ) )
						{
							var resultItem = new GetObjectsInSpaceItem.ResultItem();
							resultItem.Object = obj;
							resultList.Add( resultItem );

							if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.One )
								break;
						}
					}
					else if( item.Sphere != null )
					{
						//Sphere
						if( item.Sphere.Value.Intersects( ref bounds ) )
						{
							var resultItem = new GetObjectsInSpaceItem.ResultItem();
							resultItem.Object = obj;
							resultList.Add( resultItem );

							if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.One )
								break;
						}
					}
					else if( item.Ray != null )
					{
						//Ray
						if( bounds.Intersects( item.Ray.Value, out double scale ) )
						{
							if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.OneClosest && resultList.Count != 0 )
							{
								if( scale < resultList[ 0 ].DistanceScale )
									resultList.Clear();
								else
									continue;
							}

							var resultItem = new GetObjectsInSpaceItem.ResultItem();
							resultItem.Object = obj;
							resultItem.Position = item.Ray.Value.GetPointOnRay( scale );
							resultItem.DistanceScale = scale;
							resultList.Add( resultItem );

							if( item.CastType == GetObjectsInSpaceItem.CastTypeEnum.One )
								break;
						}
					}
				}

				var resultArray = resultList.ToArray();

				//Ray: sort
				if( item.Ray != null && item.CastType == GetObjectsInSpaceItem.CastTypeEnum.All )
				{
					//!!!!slowly?
					CollectionUtility.MergeSort( resultArray, delegate ( GetObjectsInSpaceItem.ResultItem v1, GetObjectsInSpaceItem.ResultItem v2 )
					{
						if( v1.DistanceScale < v2.DistanceScale )
							return -1;
						if( v1.DistanceScale > v2.DistanceScale )
							return 1;
						return 0;
					} );
				}

				item.Result = resultArray;
			}
		}

		public bool GetOctreeStatistics( out int objectCount, out Bounds octreeBounds, out int octreeNodeCount, out double timeSinceLastFullRebuild )
		{
			if( octree != null )
			{
				octree.GetStatistics( out objectCount, out octreeBounds, out octreeNodeCount, out timeSinceLastFullRebuild );
				return true;
			}
			else
			{
				objectCount = 0;
				octreeBounds = Bounds.Zero;
				octreeNodeCount = 0;
				timeSinceLastFullRebuild = 0;
				return false;
			}
		}
	}
}
