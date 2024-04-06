#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Auxiliary class for <see cref="GroupOfObjects"/>.
	/// </summary>
	public class GroupOfObjectsUndo
	{
		public class UndoActionCreateDelete : UndoSystem.Action
		{
			GroupOfObjects groupOfObjects;
			GroupOfObjects.Object[] objects;
			bool create;

			///////////////////////////////////////////

			public UndoActionCreateDelete( GroupOfObjects groupOfObjects, int[] indexes, bool create, bool callDestroyObjects )
			{
				this.groupOfObjects = groupOfObjects;
				this.objects = groupOfObjects.ObjectsGetData( indexes );
				this.create = create;

				if( !create && callDestroyObjects )
					DestroyObjects();
			}

			public UndoActionCreateDelete( GroupOfObjects groupOfObjects, GroupOfObjects.Object[] objects, bool create, bool callDestroyObjects )
			{
				this.groupOfObjects = groupOfObjects;
				this.objects = objects;
				this.create = create;

				if( !create && callDestroyObjects )
					DestroyObjects();
			}

			void CreateObjects()
			{
				var newIndexes = groupOfObjects.ObjectsAdd( objects );
				objects = groupOfObjects.ObjectsGetData( newIndexes );
			}

			void DestroyObjects()
			{
				var removedCount = groupOfObjects.ObjectsRemove( objects );
				if( removedCount != objects.Length )
					Log.Warning( "GroupOfObjectsEditor: DestroyObjects: removedCount != objects.Length." );
			}

			protected internal override void DoUndo()
			{
				if( create )
					DestroyObjects();
				else
					CreateObjects();

				create = !create;
			}

			protected internal override void DoRedo()
			{
				DoUndo();
			}

			protected internal override void Destroy()
			{
			}

			public override string ToString()
			{
				return string.Format( "GroupOfObjects: {0}", create ? "Create" : "Delete" );
			}
		}

		/////////////////////////////////////////

		//public class UndoActionChangeData_Mesh : UndoSystem.Action
		//{
		//	GroupOfObjects groupOfObjects;
		//	int[] indexes;
		//	GroupOfObjects.ObjectMesh[] restoreData;

		//	///////////////////////////////////////////

		//	public UndoActionChangeData_Mesh( GroupOfObjects groupOfObjects, int[] indexes, GroupOfObjects.ObjectMesh[] restoreData )
		//	{
		//		this.groupOfObjects = groupOfObjects;
		//		this.indexes = indexes;
		//		this.restoreData = restoreData;
		//	}

		//	protected internal override void DoUndo()
		//	{
		//		var newRestoreData = groupOfObjects.ObjectsMeshGetData( indexes );

		//		for( int n = 0; n < indexes.Length; n++ )
		//		{
		//			var index = indexes[ n ];
		//			ref var objectMesh = ref groupOfObjects.ObjectsMeshGetData( index );

		//			objectMesh = restoreData[ index ];
		//		}

		//		restoreData = newRestoreData;

		//		//!!!!
		//		groupOfObjects.CreateSectors();
		//	}

		//	protected internal override void DoRedo()
		//	{
		//		DoUndo();
		//	}

		//	protected internal override void Destroy()
		//	{
		//	}

		//	public override string ToString()
		//	{
		//		return string.Format( "GroupOfObjects: ObjectMesh Change" );
		//	}
		//}

	}
}

#endif