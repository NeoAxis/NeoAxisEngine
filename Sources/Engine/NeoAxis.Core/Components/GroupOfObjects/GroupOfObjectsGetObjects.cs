// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	public partial class GroupOfObjects
	{
		/// <summary>
		/// Represents data to get items from <see cref="GroupOfObjects"/> object.
		/// </summary>
		public class GetObjectsItem
		{
			//general
			public CastTypeEnum CastType;
			public Metadata.TypeInfo SelectedTypeOnly;
			//!!!!
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

			//addition
			public object UserData;

			////////////////

			public enum CastTypeEnum
			{
				One,
				//!!!!OneClosest,
				All,
			}

			////////////////

			/// <summary>
			/// Represents result data item of <see cref="GetObjectsItem"/>.
			/// </summary>
			public struct ResultItem
			{
				public int Object;
				//!!!!
				//public Vector3? Position;
				//public double DistanceScale;
			}

			////////////////

			public GetObjectsItem()
			{
			}

			public GetObjectsItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Bounds bounds )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Bounds = bounds;
			}

			public GetObjectsItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Box box )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Box = box;
			}

			public GetObjectsItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Sphere sphere )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Sphere = sphere;
			}

			public GetObjectsItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Plane[] planes )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Planes = planes;
			}

			public GetObjectsItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Plane[] planes, Bounds bounds )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Planes = planes;
				this.Bounds = bounds;
			}

			public GetObjectsItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Frustum frustum )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Frustum = frustum;
			}

			public GetObjectsItem( CastTypeEnum castType, Metadata.TypeInfo selectedTypeOnly, bool visibleOnly, Ray ray )
			{
				this.CastType = castType;
				this.SelectedTypeOnly = selectedTypeOnly;
				this.VisibleOnly = visibleOnly;
				this.Ray = ray;
			}
		}

		/////////////////////////////////////////

		public void GetObjects( IList<GetObjectsItem> items )
		{
			if( items.Count == 0 )
				return;

			var scene = FindParent<Scene>();
			if( scene == null )
			{
				for( int nItem = 0; nItem < items.Count; nItem++ )
					items[ nItem ].Result = Array.Empty<GetObjectsItem.ResultItem>();
				return;
			}

			//!!!!маски использовать для выбора из octree
			//!!!!!!или сделать "ObjectInSpace_GroupOfObjects"
			var objectInSpaceType = MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) );

			//!!!!Parallel

			for( int nItem = 0; nItem < items.Count; nItem++ )
			{
				var item = items[ nItem ];

				var getObjectsInSpaceItem = new Scene.GetObjectsInSpaceItem();
				getObjectsInSpaceItem.CastType = Scene.GetObjectsInSpaceItem.CastTypeEnum.All;
				getObjectsInSpaceItem.SelectedTypeOnly = objectInSpaceType;
				getObjectsInSpaceItem.VisibleOnly = false;
				getObjectsInSpaceItem.Bounds = item.Bounds;
				getObjectsInSpaceItem.Sphere = item.Sphere;
				getObjectsInSpaceItem.Box = item.Box;
				getObjectsInSpaceItem.Planes = item.Planes;
				getObjectsInSpaceItem.Frustum = item.Frustum;
				getObjectsInSpaceItem.Ray = item.Ray;

				scene.GetObjectsInSpace( getObjectsInSpaceItem );

				//!!!!256
				var list = new List<GetObjectsItem.ResultItem>( 256 );

				for( int nResultItem = 0; nResultItem < getObjectsInSpaceItem.Result.Length; nResultItem++ )
				{
					ref var resultItem = ref getObjectsInSpaceItem.Result[ nResultItem ];

					var sector = resultItem.Object.AnyData as Sector;
					if( sector != null && sector.owner == this )
					{
						//!!!!может поделить только на int[] и для луча
						//!!!!!!для GetObjctsInSpace которые для _ObjectInSpace тоже?

						foreach( var index in sector.Objects )
						{
							var item2 = new GetObjectsItem.ResultItem();
							item2.Object = index;
							//item2.Position = xxx;
							list.Add( item2 );

							if( item.CastType == GetObjectsItem.CastTypeEnum.One )
								goto end;
						}

						//!!!!

						//item.Result

						//public struct ResultItem
						//{
						//	public int Object;
						//	public Vector3? Position;
						//	public double DistanceScale;
						//}

					}
				}

				end:;
				//!!!!
				item.Result = list.ToArray();

			}
		}

		public void GetObjects( GetObjectsItem item )
		{
			GetObjects( new GetObjectsItem[] { item } );
		}

		//unsafe GetObjectsItem.ResultItem[] GetObjects_FromOctree_GetResult( GetObjectsItem item, int* array, int outputCount )
		//{
		//	var toAdd = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Other, outputCount * 4 );
		//	NativeUtility.ZeroMemory( toAdd, outputCount * 4 );
		//	var toAdd2 = (int*)toAdd;

		//	try
		//	{
		//		int length = 0;
		//		for( int n = 0; n < outputCount; n++ )
		//		{
		//			var obj = octreeObjects[ array[ n ] ];

		//			if( item.VisibleOnly && !obj.VisibleInHierarchy )
		//				continue;
		//			if( item.SelectedTypeOnly != null && !item.SelectedTypeOnly.IsAssignableFrom( obj.BaseType ) )
		//				continue;

		//			toAdd2[ n ] = 1;
		//			length++;

		//			if( item.CastType == GetObjectsItem.CastTypeEnum.One )
		//				break;
		//		}

		//		if( length != 0 )
		//		{
		//			var result = new GetObjectsItem.ResultItem[ length ];
		//			int current = 0;

		//			for( int n = 0; n < outputCount; n++ )
		//			{
		//				var obj = octreeObjects[ array[ n ] ];

		//				if( toAdd2[ n ] == 0 )
		//					continue;

		//				ref var resultItem = ref result[ current ];
		//				resultItem.Object = obj;
		//				current++;

		//				if( item.CastType == GetObjectsItem.CastTypeEnum.One )
		//					break;
		//			}

		//			if( length != current )
		//				Log.Fatal( "Scene: GetObjects_FromOctree_GetResult: length != current." );

		//			return result;
		//		}
		//		else
		//			return Array.Empty<GetObjectsItem.ResultItem>();
		//	}
		//	finally
		//	{
		//		NativeUtility.Free( toAdd );
		//	}


		//	//if( GetObjects_FromOctree_GetResult_List == null )
		//	//	GetObjects_FromOctree_GetResult_List = new List<GetObjectsItem.ResultItem>( item.CastType == GetObjectsItem.CastTypeEnum.All ? outputCount : 4 );
		//	//var list = GetObjects_FromOctree_GetResult_List;

		//	//try
		//	//{
		//	//	//List<GetObjectsItem.ResultItem> resultList = null;

		//	//	for( int n = 0; n < outputCount; n++ )
		//	//	{
		//	//		var obj = octreeObjects[ array[ n ] ];

		//	//		if( item.VisibleOnly && !obj.VisibleInHierarchy )
		//	//			continue;
		//	//		if( item.SelectedTypeOnly != null && !item.SelectedTypeOnly.IsAssignableFrom( obj.BaseType ) )
		//	//			continue;

		//	//		//if( item.CastType == GetObjectsItem.CastTypeEnum.Closest && resultList.Count != 0 )
		//	//		//{
		//	//		//	if( scale < resultList[ 0 ].DistanceScale )
		//	//		//		resultList.Clear();
		//	//		//	else
		//	//		//		continue;
		//	//		//}

		//	//		var resultItem = new GetObjectsItem.ResultItem();
		//	//		resultItem.Object = obj;
		//	//		//resultItem.Position = item.Ray.Value.GetPointOnRay( scale );
		//	//		//resultItem.DistanceScale = scale;
		//	//		list.Add( resultItem );
		//	//		//if( resultList == null )
		//	//		//	resultList = new List<GetObjectsItem.ResultItem>( item.CastType == GetObjectsItem.CastTypeEnum.All ? outputCount : 1 );
		//	//		//resultList.Add( resultItem );

		//	//		if( item.CastType == GetObjectsItem.CastTypeEnum.One )
		//	//			break;
		//	//	}

		//	//	if( list.Count != 0 )
		//	//		return list.ToArray();
		//	//	else
		//	//		return Array.Empty<GetObjectsItem.ResultItem>();
		//	//	//if( resultList != null )
		//	//	//	return resultList.ToArray();
		//	//	//else
		//	//	//	return Array.Empty<GetObjectsItem.ResultItem>();
		//	//}
		//	//finally
		//	//{
		//	//	list.Clear();
		//	//}
		//}

		//unsafe GetObjectsItem.ResultItem[] GetObjects_FromOctree_GetResultRay( GetObjectsItem item, OctreeContainer.GetObjectsRayOutputData* array, int outputCount )
		//{
		//	//!!!!slowly

		//	List<GetObjectsItem.ResultItem> resultList = null;

		//	for( int n = 0; n < outputCount; n++ )
		//	{
		//		var obj = octreeObjects[ array[ n ].ObjectIndex ];

		//		if( item.VisibleOnly && !obj.VisibleInHierarchy )
		//			continue;
		//		if( item.SelectedTypeOnly != null && !item.SelectedTypeOnly.IsAssignableFrom( obj.BaseType ) )
		//			continue;

		//		var scale = array[ n ].DistanceNormalized;

		//		if( item.CastType == GetObjectsItem.CastTypeEnum.OneClosest && resultList != null )
		//		{
		//			if( scale < resultList[ 0 ].DistanceScale )
		//				resultList.Clear();
		//			else
		//				continue;
		//		}

		//		var resultItem = new GetObjectsItem.ResultItem();
		//		resultItem.Object = obj;
		//		resultItem.Position = item.Ray.Value.GetPointOnRay( scale );
		//		resultItem.DistanceScale = scale;
		//		if( resultList == null )
		//			resultList = new List<GetObjectsItem.ResultItem>( item.CastType == GetObjectsItem.CastTypeEnum.All ? outputCount : 1 );
		//		resultList.Add( resultItem );

		//		if( item.CastType == GetObjectsItem.CastTypeEnum.One )
		//			break;
		//	}

		//	if( resultList != null )
		//		return resultList.ToArray();
		//	else
		//		return Array.Empty<GetObjectsItem.ResultItem>();
		//}

		//unsafe void GetObjects_FromOctree( IList<GetObjectsItem> items )
		//{
		//	void Calculate( int itemIndex )
		//	{
		//		var item = items[ itemIndex ];

		//		//!!!!что с bounding sphere?

		//		//!!!!
		//		var groupMask = 0xFFFFFFFF;

		//		GetObjectsItem.ResultItem[] resultArray = null;

		//		//!!!!
		//		int arrayLength = 8192;

		//		if( item.Planes != null && item.Bounds != null )
		//		{
		//			//Planes & Bounds
		//			var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Other, sizeof( int ) * arrayLength );
		//			if( octree.GetObjects( item.Planes, item.Bounds.Value, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
		//				resultArray = GetObjects_FromOctree_GetResult( item, array, outputCount );
		//			else
		//			{
		//				var array2 = new int[ outputCount ];
		//				fixed ( int* pArray2 = array2 )
		//					if( octree.GetObjects( item.Planes, item.Bounds.Value, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
		//						resultArray = GetObjects_FromOctree_GetResult( item, pArray2, outputCount );
		//			}
		//			NativeUtility.Free( array );
		//		}
		//		else if( item.Planes != null )
		//		{
		//			//Planes
		//			var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Other, sizeof( int ) * arrayLength );
		//			if( octree.GetObjects( item.Planes, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
		//				resultArray = GetObjects_FromOctree_GetResult( item, array, outputCount );
		//			else
		//			{
		//				var array2 = new int[ outputCount ];
		//				fixed ( int* pArray2 = array2 )
		//					if( octree.GetObjects( item.Planes, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
		//						resultArray = GetObjects_FromOctree_GetResult( item, pArray2, outputCount );
		//			}
		//			NativeUtility.Free( array );
		//		}
		//		else if( item.Frustum != null )
		//		{
		//			//Frustum
		//			var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Other, sizeof( int ) * arrayLength );
		//			if( octree.GetObjects( item.Frustum, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
		//				resultArray = GetObjects_FromOctree_GetResult( item, array, outputCount );
		//			else
		//			{
		//				var array2 = new int[ outputCount ];
		//				fixed ( int* pArray2 = array2 )
		//					if( octree.GetObjects( item.Frustum, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
		//						resultArray = GetObjects_FromOctree_GetResult( item, pArray2, outputCount );
		//			}
		//			NativeUtility.Free( array );
		//		}
		//		else if( item.Bounds != null )
		//		{
		//			//Bounds
		//			var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Other, sizeof( int ) * arrayLength );
		//			if( octree.GetObjects( item.Bounds.Value, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
		//				resultArray = GetObjects_FromOctree_GetResult( item, array, outputCount );
		//			else
		//			{
		//				var array2 = new int[ outputCount ];
		//				fixed ( int* pArray2 = array2 )
		//					if( octree.GetObjects( item.Bounds.Value, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
		//						resultArray = GetObjects_FromOctree_GetResult( item, pArray2, outputCount );
		//			}
		//			NativeUtility.Free( array );
		//		}
		//		else if( item.Box != null )
		//		{
		//			//Box
		//			var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Other, sizeof( int ) * arrayLength );
		//			if( octree.GetObjects( item.Box.Value, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
		//				resultArray = GetObjects_FromOctree_GetResult( item, array, outputCount );
		//			else
		//			{
		//				var array2 = new int[ outputCount ];
		//				fixed ( int* pArray2 = array2 )
		//					if( octree.GetObjects( item.Box.Value, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
		//						resultArray = GetObjects_FromOctree_GetResult( item, pArray2, outputCount );
		//			}
		//			NativeUtility.Free( array );
		//		}
		//		else if( item.Sphere != null )
		//		{
		//			//Sphere
		//			var array = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Other, sizeof( int ) * arrayLength );
		//			if( octree.GetObjects( item.Sphere.Value, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
		//				resultArray = GetObjects_FromOctree_GetResult( item, array, outputCount );
		//			else
		//			{
		//				var array2 = new int[ outputCount ];
		//				fixed ( int* pArray2 = array2 )
		//					if( octree.GetObjects( item.Sphere.Value, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
		//						resultArray = GetObjects_FromOctree_GetResult( item, pArray2, outputCount );
		//			}
		//			NativeUtility.Free( array );
		//		}
		//		else if( item.Ray != null )
		//		{
		//			//Ray
		//			var array = (OctreeContainer.GetObjectsRayOutputData*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Other, sizeof( OctreeContainer.GetObjectsRayOutputData ) * arrayLength );
		//			if( octree.GetObjects( item.Ray.Value, groupMask, OctreeContainer.ModeEnum.All, array, arrayLength, out var outputCount ) )
		//				resultArray = GetObjects_FromOctree_GetResultRay( item, array, outputCount );
		//			else
		//			{
		//				var array2 = new OctreeContainer.GetObjectsRayOutputData[ outputCount ];
		//				fixed ( OctreeContainer.GetObjectsRayOutputData* pArray2 = array2 )
		//					if( octree.GetObjects( item.Ray.Value, groupMask, OctreeContainer.ModeEnum.All, pArray2, outputCount, out var outputCount2 ) )
		//						resultArray = GetObjects_FromOctree_GetResultRay( item, pArray2, outputCount );
		//			}
		//			NativeUtility.Free( array );
		//		}

		//		if( resultArray == null )
		//			resultArray = Array.Empty<GetObjectsItem.ResultItem>();

		//		//Ray: sort
		//		if( item.Ray != null && item.CastType == GetObjectsItem.CastTypeEnum.All )
		//		{
		//			CollectionUtility.MergeSort( resultArray, delegate ( GetObjectsItem.ResultItem v1, GetObjectsItem.ResultItem v2 )
		//			{
		//				if( v1.DistanceScale < v2.DistanceScale )
		//					return -1;
		//				if( v1.DistanceScale > v2.DistanceScale )
		//					return 1;
		//				return 0;
		//			} );
		//		}

		//		item.Result = resultArray;
		//	}

		//	if( items.Count > 1 )
		//		Parallel.For( 0, items.Count, Calculate );
		//	else
		//		Calculate( 0 );
		//}

		//bool GetObjects_NoOctree_BoundsPlanesIntersects( Bounds bounds, Plane[] planes )
		//{
		//	var boundsCenter = bounds.GetCenter();
		//	var boundsHalfSize = bounds.Maximum - boundsCenter;
		//	foreach( var plane in planes )
		//		if( plane.GetSide( boundsCenter, boundsHalfSize ) == Plane.Side.Positive )
		//			return false;
		//	return true;
		//}

		//void GetObjects_NoOctree( IList<GetObjectsItem> items )
		//{
		//	foreach( var item in items )
		//	{
		//		var resultList = new List<GetObjectsItem.ResultItem>( 64 );
		//		foreach( var obj in GetComponents<ObjectInSpace>( false, true, true ) )
		//		{
		//			if( item.VisibleOnly && !obj.VisibleInHierarchy )
		//				continue;
		//			if( item.SelectedTypeOnly != null && !item.SelectedTypeOnly.IsAssignableFrom( obj.BaseType ) )
		//				continue;

		//			//!!!!что с bounding sphere?

		//			var bounds = obj.SpaceBounds.CalculatedBoundingBox;

		//			//!!!!если уже внутри бокса, то вроде как не надо (не выделять в редакторе)

		//			if( item.Bounds != null && item.Planes != null )
		//			{
		//				//Planes + Bounds
		//				if( item.Bounds.Value.Intersects( ref bounds ) || GetObjects_NoOctree_BoundsPlanesIntersects( bounds, item.Planes ) )
		//				{
		//					var resultItem = new GetObjectsItem.ResultItem();
		//					resultItem.Object = obj;
		//					resultList.Add( resultItem );

		//					if( item.CastType == GetObjectsItem.CastTypeEnum.One )
		//						break;
		//				}
		//			}
		//			else if( item.Planes != null )
		//			{
		//				//Planes
		//				if( GetObjects_NoOctree_BoundsPlanesIntersects( bounds, item.Planes ) )
		//				{
		//					var resultItem = new GetObjectsItem.ResultItem();
		//					resultItem.Object = obj;
		//					resultList.Add( resultItem );

		//					if( item.CastType == GetObjectsItem.CastTypeEnum.One )
		//						break;
		//				}
		//			}
		//			else if( item.Frustum != null )
		//			{
		//				//Frustum
		//				Plane[] planes = item.Frustum.Planes;
		//				Bounds frustumBounds = new Bounds( item.Frustum.Points[ 0 ] );
		//				for( int n = 1; n < 8; n++ )
		//					frustumBounds.Add( item.Frustum.Points[ n ] );
		//				if( frustumBounds.Intersects( ref bounds ) || GetObjects_NoOctree_BoundsPlanesIntersects( bounds, planes ) )
		//				{
		//					var resultItem = new GetObjectsItem.ResultItem();
		//					resultItem.Object = obj;
		//					resultList.Add( resultItem );

		//					if( item.CastType == GetObjectsItem.CastTypeEnum.One )
		//						break;
		//				}
		//			}
		//			else if( item.Bounds != null )
		//			{
		//				//Bounds
		//				if( item.Bounds.Value.Intersects( ref bounds ) )
		//				{
		//					//if( item.CastType == GetObjectsItem.CastTypeEnum.Closest && resultList.Count != 0 )
		//					//{
		//					//	if( scale < resultList[ 0 ].DistanceScale )
		//					//		resultList.Clear();
		//					//	else
		//					//		continue;
		//					//}

		//					var resultItem = new GetObjectsItem.ResultItem();
		//					resultItem.Object = obj;
		//					//resultItem.Position = item.Ray.Value.GetPointOnRay( scale );
		//					//resultItem.DistanceScale = scale;
		//					resultList.Add( resultItem );

		//					if( item.CastType == GetObjectsItem.CastTypeEnum.One )
		//						break;
		//				}
		//			}
		//			else if( item.Box != null )
		//			{
		//				//Box
		//				if( item.Box.Value.Intersects( ref bounds ) )
		//				{
		//					var resultItem = new GetObjectsItem.ResultItem();
		//					resultItem.Object = obj;
		//					resultList.Add( resultItem );

		//					if( item.CastType == GetObjectsItem.CastTypeEnum.One )
		//						break;
		//				}
		//			}
		//			else if( item.Sphere != null )
		//			{
		//				//Sphere
		//				if( item.Sphere.Value.Intersects( ref bounds ) )
		//				{
		//					var resultItem = new GetObjectsItem.ResultItem();
		//					resultItem.Object = obj;
		//					resultList.Add( resultItem );

		//					if( item.CastType == GetObjectsItem.CastTypeEnum.One )
		//						break;
		//				}
		//			}
		//			else if( item.Ray != null )
		//			{
		//				//Ray
		//				if( bounds.Intersects( item.Ray.Value, out double scale ) )
		//				{
		//					if( item.CastType == GetObjectsItem.CastTypeEnum.OneClosest && resultList.Count != 0 )
		//					{
		//						if( scale < resultList[ 0 ].DistanceScale )
		//							resultList.Clear();
		//						else
		//							continue;
		//					}

		//					var resultItem = new GetObjectsItem.ResultItem();
		//					resultItem.Object = obj;
		//					resultItem.Position = item.Ray.Value.GetPointOnRay( scale );
		//					resultItem.DistanceScale = scale;
		//					resultList.Add( resultItem );

		//					if( item.CastType == GetObjectsItem.CastTypeEnum.One )
		//						break;
		//				}
		//			}
		//		}

		//		var resultArray = resultList.ToArray();

		//		//Ray: sort
		//		if( item.Ray != null && item.CastType == GetObjectsItem.CastTypeEnum.All )
		//		{
		//			CollectionUtility.MergeSort( resultArray, delegate ( GetObjectsItem.ResultItem v1, GetObjectsItem.ResultItem v2 )
		//			{
		//				if( v1.DistanceScale < v2.DistanceScale )
		//					return -1;
		//				if( v1.DistanceScale > v2.DistanceScale )
		//					return 1;
		//				return 0;
		//			} );
		//		}

		//		item.Result = resultArray;
		//	}
		//}

	}
}
