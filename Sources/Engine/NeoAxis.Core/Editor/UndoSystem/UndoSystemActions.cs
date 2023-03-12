//#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Compound action of undo/redo system.
	/// </summary>
	public class UndoMultiAction : UndoSystem.Action
	{
		List<UndoSystem.Action> actions;

		public UndoMultiAction()
		{
			actions = new List<UndoSystem.Action>();
		}

		public UndoMultiAction( ICollection<UndoSystem.Action> actions )
		{
			this.actions = new List<UndoSystem.Action>( actions );
		}

		public void AddAction( UndoSystem.Action action )
		{
			actions.Add( action );
		}

		public void AddActions( IEnumerable<UndoSystem.Action> actions )
		{
			this.actions.AddRange( actions );
		}

		public List<UndoSystem.Action> Actions
		{
			get { return actions; }
		}

		protected internal override void Destroy()
		{
			for( int n = 0; n < actions.Count; n++ )
				actions[ n ].Destroy();
		}

		protected internal override void DoRedo()
		{
			for( int n = 0; n < actions.Count; n++ )
				actions[ n ].DoRedo();

			actions.Reverse();
		}

		protected internal override void DoUndo()
		{
			for( int n = 0; n < actions.Count; n++ )
				actions[ n ].DoUndo();

			actions.Reverse();
		}

		////!!!!new
		//public T GetOrAddAction<T>() where T : UndoSystem.Action
		//{
		//	foreach( var action2 in Actions )
		//	{
		//		if( action2 is T )
		//			return (T)action2;
		//	}

		//	var action = (T)typeof( T ).InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null );
		//	AddAction( action );
		//	return action;
		//}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!!
	//class UndoSetOfActions : UndoSystem.Action
	//{
	//	UndoSystem.Action[] actions;

	//	public UndoSetOfActions( UndoSystem.Action[] actions )
	//	{
	//		this.actions = (UndoSystem.Action[])actions.Clone();
	//		Array.Reverse( this.actions );
	//	}

	//	protected override void Destroy()
	//	{
	//		foreach( UndoSystem.Action action in actions )
	//		{
	//			MethodInfo method = action.GetType().GetMethod( "Destroy",
	//				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
	//			method.Invoke( action, new object[ 0 ] );
	//		}
	//	}

	//	protected override void DoRedo()
	//	{
	//		foreach( UndoSystem.Action action in actions )
	//		{
	//			MethodInfo method = action.GetType().GetMethod( "DoRedo",
	//				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
	//			method.Invoke( action, new object[ 0 ] );
	//		}
	//		Array.Reverse( actions );
	//	}

	//	protected override void DoUndo()
	//	{
	//		foreach( UndoSystem.Action action in actions )
	//		{
	//			MethodInfo method = action.GetType().GetMethod( "DoUndo",
	//				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
	//			method.Invoke( action, new object[ 0 ] );
	//		}
	//		Array.Reverse( actions );
	//	}
	//}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!!Object?
	/// <summary>
	/// The action of undo/redo system for creating and deleting objects.
	/// </summary>
	public class UndoActionComponentCreateDelete : UndoSystem.Action
	{
		DocumentInstance document;
		List<Component> objects;
		bool create;
		Dictionary<Component, RestoreData> dataToRestore = new Dictionary<Component, RestoreData>();

		class RestoreData
		{
			public Component parent;
			public int insertIndex;
		}

		//

		public UndoActionComponentCreateDelete( DocumentInstance document, ICollection<Component> objects, bool create )//, bool callDeleteObjects )
		{
			this.document = document;
			this.objects = new List<Component>( objects );
			this.create = create;

			//sort objects by parent index
			if( !create )
			{
				CollectionUtility.InsertionSort( this.objects, delegate ( Component c1, Component c2 )
				{
					if( c1.Parent != null && c1.Parent == c2.Parent )
						return c1.Parent.Components.IndexOf( c1 ) - c2.Parent.Components.IndexOf( c2 );
					return 0;
				} );
			}

			if( !create )//&& callDeleteObjects )
				DeleteObjects();
		}

		void CreateObjects()
		{
#if !DEPLOY
			ContentBrowserUtility.AllContentBrowsers_SuspendChildrenChangedEvent();
#endif
			ESet<ComponentHierarchyController> controllersToProcessDelayedOperations = new ESet<ComponentHierarchyController>();
			try
			{
				foreach( var obj in Objects )
				{
					//!!!!?
					if( obj.Disposed )
						continue;

					dataToRestore.TryGetValue( obj, out RestoreData data );
					if( data != null )
					{
						dataToRestore.Remove( obj );

						data.parent.AddComponent( obj, data.insertIndex );

						if( obj.ParentRoot?.HierarchyController != null )
							controllersToProcessDelayedOperations.AddWithCheckAlreadyContained( obj.ParentRoot?.HierarchyController );
					}
				}
			}
			finally
			{
				foreach( var c in controllersToProcessDelayedOperations )
					c.ProcessDelayedOperations();
#if !DEPLOY
				ContentBrowserUtility.AllContentBrowsers_ResumeChildrenChangedEvent();
#endif
			}
		}

		void DeleteObjects()
		{
#if !DEPLOY
			ContentBrowserUtility.AllContentBrowsers_SuspendChildrenChangedEvent();
#endif
			ESet<ComponentHierarchyController> controllersToProcessDelayedOperations = new ESet<ComponentHierarchyController>();

			List<Component> deleted = new List<Component>();

			try
			{
				foreach( var obj in Objects )
				{
					var parent = obj.Parent;
					if( parent != null )
					{
						RestoreData data = new RestoreData();
						data.parent = parent;
						data.insertIndex = parent.Components.IndexOf( obj );

						dataToRestore[ obj ] = data;

						obj.RemoveFromParent( true );

						deleted.Add( obj );

						if( obj.ParentRoot?.HierarchyController != null )
							controllersToProcessDelayedOperations.AddWithCheckAlreadyContained( obj.ParentRoot?.HierarchyController );
					}
				}
			}
			finally
			{
				foreach( var c in controllersToProcessDelayedOperations )
					c.ProcessDelayedOperations();
#if !DEPLOY
				ContentBrowserUtility.AllContentBrowsers_ResumeChildrenChangedEvent();
#endif
			}

#if !DEPLOY
			//update selected objects for document windows
			if( document != null )
			{
				foreach( var window in EditorAPI.GetAllDocumentWindowsOfDocument( document ) )
				{
					var selectedObjects = new ESet<object>( window.SelectedObjectsSet );
					bool updated = false;

					foreach( var obj in deleted )
					{
						if( selectedObjects.Remove( obj ) )
							updated = true;
					}

					if( updated )
						window.SelectObjects( selectedObjects );
				}
			}
			//!!!!так?
			//!!!!!!как-то слишком низкоуровнего из-за documentWindow?
			//if( SettingsWindow.Instance != null )
			//{
			//	SettingsWindow.PanelData panel = SettingsWindow.Instance.SelectedPanel;
			//	if( panel != null )
			//	{
			//		var selectedObjects = new ESet<object>( SettingsWindow.Instance.SelectedObjectsSet );

			//		foreach( var obj in deleted )
			//			selectedObjects.Remove( obj );

			//		if( !ESet<object>.IsEqual( selectedObjects, SettingsWindow.Instance.SelectedObjectsSet ) )
			//			SettingsWindow.Instance.SelectObjects( panel.documentWindow, selectedObjects );
			//	}
			//}
#endif
		}

		protected internal override void DoUndo()
		{
			if( create )
				DeleteObjects();
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
			//!!!!с этим кодом объект может удалиться на совсем, хотя его можно восстановить.
			//!!!!!!если удалить, восстановить, опять удалить. а если после восстановления подвигать, то норм всё (история удаления удалится)
			//foreach( var obj in Objects )
			//{
			//	if( obj.Parent == null )
			//		obj.Dispose();
			//}

			objects.Clear();
		}

		public DocumentInstance Document
		{
			get { return document; }
		}

		public List<Component> Objects
		{
			get { return objects; }
		}

		public override string ToString()
		{
			return string.Format( "{0}: Objects: {1}", ( create ? "Create" : "Delete" ), objects.Count );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The action of undo/redo system for moving components relative to other components.
	/// </summary>
	public class UndoActionComponentMove : UndoSystem.Action
	{
		DocumentInstance document;
		Component obj;
		RestoreData dataToRestore = new RestoreData();

		class RestoreData
		{
			public Component parent;
			public int insertIndex;
		}

		//

		public UndoActionComponentMove( DocumentInstance document, Component obj, Component oldParent, int oldIndex )
		{
			this.document = document;
			this.obj = obj;

			dataToRestore.parent = oldParent;
			dataToRestore.insertIndex = oldIndex;
		}

		void MoveObject()
		{
			//!!!!?
			if( obj.Disposed )
				return;

			if( dataToRestore.parent != obj.Parent )
			{
				var newRestoreData = new RestoreData();
				newRestoreData.parent = obj.Parent;
				newRestoreData.insertIndex = obj.Parent.Components.IndexOf( obj );

				obj.Parent.RemoveComponent( obj, false );
				dataToRestore.parent.AddComponent( obj, dataToRestore.insertIndex );

				dataToRestore = newRestoreData;
			}
			else
			{
				var newRestoreData = new RestoreData();
				newRestoreData.parent = dataToRestore.parent;
				newRestoreData.insertIndex = obj.Parent.Components.IndexOf( obj );

				dataToRestore.parent.Components.MoveTo( obj, dataToRestore.insertIndex );

				dataToRestore = newRestoreData;
			}
		}

		protected internal override void DoUndo()
		{
			MoveObject();
		}

		protected internal override void DoRedo()
		{
			DoUndo();
		}

		protected internal override void Destroy()
		{
			obj = null;
		}

		public DocumentInstance Document
		{
			get { return document; }
		}

		public Component Obj
		{
			get { return obj; }
		}

		public override string ToString()
		{
			return string.Format( "Move: Object: {0}", obj );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!
	//RegisterArray();
	//RegisterListSet( typeof( List<> ) );
	//RegisterListSet( typeof( ESet<> ) );
	//RegisterListSet( typeof( HashSet<> ) );
	//RegisterListSet( typeof( SortedSet<> ) );
	//RegisterListSet( typeof( Stack<> ) );
	//RegisterListSet( typeof( Queue<> ) );
	//RegisterDictionary( typeof( Dictionary<,> ) );
	//RegisterDictionary( typeof( EDictionary<,> ) );
	//RegisterDictionary( typeof( SortedList<,> ) );

	/// <summary>
	/// The action of undo/redo system for adding and removing items of a list.
	/// </summary>
	public class UndoActionListAddRemove : UndoSystem.Action
	{
		object list;
		List<int> objectIndexes;
		bool add;
		Dictionary<int, RestoreData> dataToRestore = new Dictionary<int, RestoreData>();

		//

		class RestoreData
		{
			public object objectToRestore;
		}

		//

		public UndoActionListAddRemove( object list, ICollection<int> objectIndexes, bool add )//, bool callDeleteObjects )
		{
			this.list = list;
			this.objectIndexes = new List<int>( objectIndexes );

			//!!!!!
			//if( this.objectIndexes.Count > 1 )
			//	Log.Fatal( "check this.objectIndexes.Count > 1" );

			CollectionUtility.MergeSort( this.objectIndexes, delegate ( int index1, int index2 )
			{
				if( index1 < index2 )
					return -1;
				if( index1 > index2 )
					return 1;
				return 0;
			} );

			this.add = add;

			if( !add )//&& callDeleteObjects )
				RemoveObjects();
			//else
			//{
			//ContentBrowserItem_NoSpecialization.AllObjects_PerformChildrenChanged( list );
			//}
		}

		void AddObjects()
		{
			MethodInfo methodInsert;
			var referenceList = list as IReferenceList;
			if( referenceList != null )
				methodInsert = list.GetType().GetMethod( "Insert", new Type[] { typeof( int ), referenceList.GetItemType() } );
			else
				methodInsert = list.GetType().GetMethod( "Insert" );

			foreach( var index in ObjectIndexes )
			{
				//if( obj.Disposed )
				//	continue;

				dataToRestore.TryGetValue( index, out RestoreData data );
				if( data != null )
				{
					//remove restore data
					dataToRestore.Remove( index );

					//insert to the list
					methodInsert.Invoke( list, new object[] { index, data.objectToRestore } );
					//ContentBrowserItem_NoSpecialization.AllObjects_PerformChildrenChanged( list );
				}
			}
			//ContentBrowserItem_NoSpecialization.AllObjects_PerformChildrenChanged( list );
		}

		void RemoveObjects()
		{
			var elementNetType = list.GetType().GetGenericArguments()[ 0 ];
			MethodInfo methodRemoveAt = list.GetType().GetMethod( "RemoveAt" );
			var propertyItem = list.GetType().GetProperty( "Item" );

			var reversed = new List<int>( ObjectIndexes );
			reversed.Reverse();

			foreach( var index in reversed )
			{
				//save data to restore
				var data = new RestoreData();
				data.objectToRestore = propertyItem.GetValue( list, new object[] { index } );
				dataToRestore[ index ] = data;

				//remove from the list
				methodRemoveAt.Invoke( list, new object[] { index } );
				//ContentBrowserItem_NoSpecialization.AllObjects_PerformChildrenChanged( list );
			}
			//ContentBrowserItem_NoSpecialization.AllObjects_PerformChildrenChanged( list );
		}

		protected internal override void DoUndo()
		{
			if( add )
				RemoveObjects();
			else
				AddObjects();

			add = !add;
		}

		protected internal override void DoRedo()
		{
			DoUndo();
		}

		protected internal override void Destroy()
		{
			//!!!!!?
			//foreach( var obj in Objects )
			//{
			//	if( obj.Parent == null )
			//		obj.Dispose();
			//}
			objectIndexes.Clear();
		}

		public object List
		{
			get { return list; }
		}

		public List<int> ObjectIndexes
		{
			get { return objectIndexes; }
		}

		public override string ToString()
		{
			return string.Format( "{0}: Objects: {1}", ( add ? "Add" : "Remove" ), objectIndexes.Count );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//public class UndoActionPropertyChange : UndoSystem.Action
	//{
	//	object obj;
	//	Metadata.Property property;
	//	object restoreValue;
	//	object[] indexes;

	//	public UndoActionPropertyChange( object obj, Metadata.Property property, object restoreValue, object[] indexes )
	//	{
	//		this.obj = obj;
	//		this.property = property;
	//		this.restoreValue = restoreValue;
	//		this.indexes = indexes;
	//	}

	//	public object Obj
	//	{
	//		get { return obj; }
	//		set { obj = value; }
	//	}

	//	public Metadata.Property Property
	//	{
	//		get { return property; }
	//		set { property = value; }
	//	}

	//	public object RestoreValue
	//	{
	//		get { return restoreValue; }
	//		set { restoreValue = value; }
	//	}

	//	public object[] Indexes
	//	{
	//		get { return indexes; }
	//		set { indexes = value; }
	//	}

	//	/////////////////////////////////////////

	//	protected internal override void DoUndo()
	//	{
	//		//SettingsCell_Properties.disablePropertyValueChanged = true;

	//		object newRestoreValue = Property.GetValue( Obj, Indexes );
	//		Property.SetValue( Obj, RestoreValue, Indexes );
	//		RestoreValue = newRestoreValue;

	//		//SettingsCell_Properties.disablePropertyValueChanged = false;
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
	//		return string.Format( "Property change: Name: {0}", Property.Name );
	//	}
	//}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!use UndoActionPropertyChange?
	/// <summary>
	/// The action of undo/redo system for changing properties.
	/// </summary>
	public class UndoActionPropertiesChange : UndoSystem.Action
	{
		IList<Item> items;

		/////////////////////////////////////////

		//!!!!или UndoMultiAction юзать
		/// <summary>
		/// Represents an item of <see cref="UndoActionPropertiesChange"/>.
		/// </summary>
		public class Item
		{
			object obj;
			Metadata.Property property;
			object restoreValue;
			object[] indexes;

			public Item( object obj, Metadata.Property property, object restoreValue, object[] indexes = null )
			{
				if( indexes == null )
					indexes = new object[ 0 ];

				if( property == null )
				{
					//fatal?
					Log.Fatal( "UndoActionPropertiesChange: Item: Constructor: property == null." );
				}

				this.obj = obj;
				this.property = property;
				this.restoreValue = restoreValue;
				this.indexes = indexes;
			}

			public object Obj
			{
				get { return obj; }
				set { obj = value; }
			}

			public Metadata.Property Property
			{
				get { return property; }
				set { property = value; }
			}

			public object RestoreValue
			{
				get { return restoreValue; }
				set { restoreValue = value; }
			}

			public object[] Indexes
			{
				get { return indexes; }
				set { indexes = value; }
			}
		}

		/////////////////////////////////////////

		public UndoActionPropertiesChange( IList<Item> items )
		{
			this.items = items;
		}

		public UndoActionPropertiesChange( Item item )
		{
			this.items = new Item[] { item };
		}

		protected internal override void DoUndo()
		{
			for( int n = 0; n < items.Count; n++ )
			{
				Item item = items[ n ];

				//SettingsCell_Properties.disablePropertyValueChanged = true;

				object newRestoreValue = item.Property.GetValue( item.Obj, item.Indexes );
				item.Property.SetValue( item.Obj, item.RestoreValue, item.Indexes );
				item.RestoreValue = newRestoreValue;

				//SettingsCell_Properties.disablePropertyValueChanged = false;
			}
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
			return string.Format( "Property change: Items: {0}", items.Count );
		}

		public IList<Item> Items
		{
			get { return items; }
		}

		public void PerformUndo()
		{
			DoUndo();
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//class UndoLayerCreateDeleteAction : UndoSystem.Action
	//{
	//	bool create;
	//	Map.EditorLayer[] layers;
	//	List<RestoreMapObjectItem> restoreMapObjects = new List<RestoreMapObjectItem>();

	//	///////////////////////////////////////////

	//	class RestoreMapObjectItem
	//	{
	//		public MapObject mapObject;
	//		public Map.EditorLayer originalLayer;
	//	}

	//	///////////////////////////////////////////

	//	public UndoLayerCreateDeleteAction( Map.EditorLayer layer, bool create )
	//	{
	//		this.create = create;
	//		layers = layer.GetAllChildrenRecursive();

	//		//layerSet
	//		Set<Map.EditorLayer> layerSet = new Set<Map.EditorLayer>( layers.Length );
	//		foreach( Map.EditorLayer layer2 in layers )
	//			layerSet.Add( layer2 );

	//		//initialize restoreMapObjects
	//		foreach( Entity entity in Map.Instance.Children )
	//		{
	//			MapObject mapObject = entity as MapObject;
	//			if( mapObject != null && layerSet.Contains( mapObject.EditorLayer ) )
	//			{
	//				RestoreMapObjectItem item = new RestoreMapObjectItem();
	//				item.mapObject = mapObject;
	//				item.originalLayer = mapObject.EditorLayer;
	//				restoreMapObjects.Add( item );
	//			}
	//		}

	//		if( !create )
	//		{
	//			//remove from world
	//			ExcludeLayerFromWorld();
	//		}
	//	}

	//	void IncludeLayerToWorld()
	//	{
	//		layers[ 0 ]._IncludeToWorld();
	//		MainForm.Instance.MapEntitiesForm.AddLayer( layers[ 0 ] );

	//		foreach( RestoreMapObjectItem item in restoreMapObjects )
	//		{
	//			item.mapObject.EditorLayer = item.originalLayer;
	//			MainForm.Instance.MapEntitiesForm.UpdateEntityLayer( item.mapObject );
	//		}

	//		MainForm.Instance.PropertiesForm.RefreshProperties();
	//	}

	//	void ExcludeLayerFromWorld()
	//	{
	//		//change layers of objects
	//		foreach( RestoreMapObjectItem item in restoreMapObjects )
	//		{
	//			item.mapObject.EditorLayer = layers[ 0 ].Parent;
	//			MainForm.Instance.MapEntitiesForm.UpdateEntityLayer( item.mapObject );
	//		}

	//		//exclude from world
	//		MainForm.Instance.MapEntitiesForm.RemoveLayer( layers[ 0 ] );
	//		layers[ 0 ]._ExcludeFromWorld();

	//		MainForm.Instance.PropertiesForm.RefreshProperties();
	//	}

	//	protected override void DoUndo()
	//	{
	//		if( create )
	//		{
	//			//remove from world
	//			ExcludeLayerFromWorld();
	//		}
	//		else
	//		{
	//			//add to world
	//			//restore from deleted state
	//			IncludeLayerToWorld();
	//		}

	//		create = !create;
	//	}

	//	protected override void DoRedo()
	//	{
	//		DoUndo();
	//	}

	//	protected override void Destroy()
	//	{
	//		if( !create )
	//		{
	//			layers[ 0 ]._IncludeToWorld();
	//			layers[ 0 ].Remove();
	//		}
	//	}

	//	public override string ToString()
	//	{
	//		return string.Format( "Layer {0}: {1}", ( create ? "Create" : "Delete" ), layers[ 0 ].Name );
	//	}
	//}
}

//#endif