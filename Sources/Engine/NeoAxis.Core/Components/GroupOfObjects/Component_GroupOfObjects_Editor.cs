// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Auxiliary class for <see cref="Component_GroupOfObjects"/>.
	/// </summary>
	public class Component_GroupOfObjects_Editor
	{
		public class UndoActionCreateDelete : UndoSystem.Action
		{
			Component_GroupOfObjects groupOfObjects;
			Component_GroupOfObjects.Object[] objects;
			bool create;

			///////////////////////////////////////////

			public UndoActionCreateDelete( Component_GroupOfObjects groupOfObjects, int[] indexes, bool create, bool callDestroyObjects )
			{
				this.groupOfObjects = groupOfObjects;
				this.objects = groupOfObjects.ObjectsGetData( indexes );
				this.create = create;

				if( !create && callDestroyObjects )
					DestroyObjects();
			}

			public UndoActionCreateDelete( Component_GroupOfObjects groupOfObjects, Component_GroupOfObjects.Object[] objects, bool create, bool callDestroyObjects )
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
					Log.Fatal( "Component_GroupOfObjects_Editor: DestroyObjects: removedCount != objects.Length." );
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
		//	Component_GroupOfObjects groupOfObjects;
		//	int[] indexes;
		//	Component_GroupOfObjects.ObjectMesh[] restoreData;

		//	///////////////////////////////////////////

		//	public UndoActionChangeData_Mesh( Component_GroupOfObjects groupOfObjects, int[] indexes, Component_GroupOfObjects.ObjectMesh[] restoreData )
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
