// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Component for setting item type of <see cref="Component_GroupOfObjects"/>.
	/// </summary>
	[EditorSettingsCell( typeof( Component_GroupOfObjectsElement_SettingsCell ) )]
	public abstract class Component_GroupOfObjectsElement : Component
	{
		/// <summary>
		/// The identifier of the element in the group of objects.
		/// </summary>
		[DefaultValue( 0 )]
		public Reference<int> Index
		{
			get { if( _index.BeginGet() ) Index = _index.Get( this ); return _index.value; }
			set { if( _index.BeginSet( ref value ) ) { try { IndexChanged?.Invoke( this ); } finally { _index.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Index"/> property value changes.</summary>
		public event Action<Component_GroupOfObjectsElement> IndexChanged;
		ReferenceField<int> _index = 0;

		[DefaultValue( true )]
		public Reference<bool> AutoAlign
		{
			get { if( _autoAlign.BeginGet() ) AutoAlign = _autoAlign.Get( this ); return _autoAlign.value; }
			set { if( _autoAlign.BeginSet( ref value ) ) { try { AutoAlignChanged?.Invoke( this ); } finally { _autoAlign.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AutoAlign"/> property value changes.</summary>
		public event Action<Component_GroupOfObjectsElement> AutoAlignChanged;
		ReferenceField<bool> _autoAlign = true;

		//

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			( Parent as Component_GroupOfObjects )?.ElementTypesCacheNeedUpdate();
		}

		public bool ObjectsExists()
		{
			var groupOfObjects = Parent as Component_GroupOfObjects;
			if( groupOfObjects != null )
			{
				foreach( var index in groupOfObjects.ObjectsGetAll() )
				{
					ref var obj = ref groupOfObjects.ObjectGetData( index );
					if( obj.Element == Index )
						return true;
				}
			}
			return false;
		}

		public List<int> GetObjectsOfElement()
		{
			var groupOfObjects = Parent as Component_GroupOfObjects;
			if( groupOfObjects != null )
			{
				var indexes = groupOfObjects.ObjectsGetAll();
				var list = new List<int>( indexes.Count );
				foreach( var index in indexes )
				{
					ref var obj = ref groupOfObjects.ObjectGetData( index );
					if( obj.Element == Index )
						list.Add( index );
				}
				return list;
			}
			return new List<int>();
		}

		public virtual void ResetColors( UndoMultiAction undoMultiAction )
		{
			var groupOfObjects = Parent as Component_GroupOfObjects;
			if( groupOfObjects != null )
			{
				var indexes = GetObjectsOfElement();
				var newObjects = groupOfObjects.ObjectsGetData( indexes );

				for( int n = 0; n < indexes.Count; n++ )
				{
					var index = indexes[ n ];
					ref var obj = ref newObjects[ n ];

					obj.UniqueIdentifier = 0;
					obj.Color = ColorValue.One;
				}

				//delete and undo to delete
				undoMultiAction.AddAction( new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true ) );

				//add new data
				var newIndexes = groupOfObjects.ObjectsAdd( newObjects );
				undoMultiAction.AddAction( new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, newIndexes, true, false ) );
			}
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Component for setting mesh item type of <see cref="Component_GroupOfObjects"/>.
	/// </summary>
	public class Component_GroupOfObjectsElement_Mesh : Component_GroupOfObjectsElement
	{
		/// <summary>
		/// The mesh used by the mesh object.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set
			{
				if( _mesh.BeginSet( ref value ) )
				{
					try
					{
						MeshChanged?.Invoke( this );
						( Parent as Component_GroupOfObjects )?.ElementTypesCacheNeedUpdate();
					}
					finally { _mesh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<Component_GroupOfObjectsElement_Mesh> MeshChanged;
		ReferenceField<Component_Mesh> _mesh = null;

		/// <summary>
		/// Replaces all geometries of the mesh by another material.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		public Reference<Component_Material> ReplaceMaterial
		{
			get { if( _replaceMaterial.BeginGet() ) ReplaceMaterial = _replaceMaterial.Get( this ); return _replaceMaterial.value; }
			set
			{
				if( _replaceMaterial.BeginSet( ref value ) )
				{
					try
					{
						ReplaceMaterialChanged?.Invoke( this );
						( Parent as Component_GroupOfObjects )?.ElementTypesCacheNeedUpdate();
					}
					finally { _replaceMaterial.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReplaceMaterial"/> property value changes.</summary>
		public event Action<Component_GroupOfObjectsElement_Mesh> ReplaceMaterialChanged;
		ReferenceField<Component_Material> _replaceMaterial;

		/// <summary>
		/// Maximum visibility range of the object.
		/// </summary>
		[DefaultValue( 10000.0 )]
		[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> VisibilityDistance
		{
			get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
			set
			{
				if( _visibilityDistance.BeginSet( ref value ) )
				{
					try
					{
						VisibilityDistanceChanged?.Invoke( this );
						( Parent as Component_GroupOfObjects )?.ElementTypesCacheNeedUpdate();
					}
					finally { _visibilityDistance.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		public event Action<Component_GroupOfObjectsElement_Mesh> VisibilityDistanceChanged;
		ReferenceField<double> _visibilityDistance = 10000.0;

		/// <summary>
		/// Whether to cast shadows on the other surfaces.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> CastShadows
		{
			get { if( _castShadows.BeginGet() ) CastShadows = _castShadows.Get( this ); return _castShadows.value; }
			set
			{
				if( _castShadows.BeginSet( ref value ) )
				{
					try
					{
						CastShadowsChanged?.Invoke( this );
						( Parent as Component_GroupOfObjects )?.ElementTypesCacheNeedUpdate();
					}
					finally { _castShadows.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<Component_GroupOfObjectsElement_Mesh> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// Whether it is possible to apply decals the surface.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> ReceiveDecals
		{
			get { if( _receiveDecals.BeginGet() ) ReceiveDecals = _receiveDecals.Get( this ); return _receiveDecals.value; }
			set
			{
				if( _receiveDecals.BeginSet( ref value ) )
				{
					try
					{
						ReceiveDecalsChanged?.Invoke( this );
						( Parent as Component_GroupOfObjects )?.ElementTypesCacheNeedUpdate();
					}
					finally { _receiveDecals.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<Component_GroupOfObjectsElement_Mesh> ReceiveDecalsChanged;
		ReferenceField<bool> _receiveDecals = true;

		/////////////////////////////////////////

		public virtual void UpdateAlignment( UndoMultiAction undoMultiAction )
		{
			var random = new Random();

			var groupOfObjects = Parent as Component_GroupOfObjects;
			if( groupOfObjects != null )
			{
				//var surface = Surface.Value;
				//if( surface != null )
				//{

				var indexes = GetObjectsOfElement();
				var scene = groupOfObjects.FindParent<Component_Scene>();

				var newObjects = groupOfObjects.ObjectsGetData( indexes );

				for( int n = 0; n < indexes.Count; n++ )
				{
					var index = indexes[ n ];
					ref var obj = ref newObjects[ n ];

					double positionZ = 0;

					obj.UniqueIdentifier = 0;
					if( AutoAlign && scene != null )
					{
						var r = Component_Scene_Utility.CalculateObjectPositionZ( scene, groupOfObjects, obj.Position.Z, obj.Position.ToVector2() );
						if( r.found )
							obj.Position.Z = r.positionZ + positionZ;
					}
					//obj.Rotation = rotation;
					//obj.Scale = scale;
				}

				//delete and undo to delete
				undoMultiAction.AddAction( new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true ) );

				//add new data
				var newIndexes = groupOfObjects.ObjectsAdd( newObjects );
				undoMultiAction.AddAction( new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, newIndexes, true, false ) );

				//}
			}
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Component for setting surface item type of <see cref="Component_GroupOfObjects"/>.
	/// </summary>
	public class Component_GroupOfObjectsElement_Surface : Component_GroupOfObjectsElement
	{
		[DefaultValue( null )]
		public Reference<Component_Surface> Surface
		{
			get { if( _surface.BeginGet() ) Surface = _surface.Get( this ); return _surface.value; }
			set
			{
				if( _surface.BeginSet( ref value ) )
				{
					try
					{
						SurfaceChanged?.Invoke( this );
						( Parent as Component_GroupOfObjects )?.ElementTypesCacheNeedUpdate();
					}
					finally { _surface.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Surface"/> property value changes.</summary>
		public event Action<Component_GroupOfObjectsElement_Surface> SurfaceChanged;
		ReferenceField<Component_Surface> _surface = null;

		/////////////////////////////////////////

		public virtual void UpdateVariations( bool randomizeGroups, UndoMultiAction undoMultiAction )
		{
			var random = new Random();

			var groupOfObjects = Parent as Component_GroupOfObjects;
			if( groupOfObjects != null )
			{
				var surface = Surface.Value;
				if( surface != null )
				{
					var indexes = GetObjectsOfElement();
					var scene = groupOfObjects.FindParent<Component_Scene>();

					var newObjects = groupOfObjects.ObjectsGetData( indexes );

					for( int n = 0; n < indexes.Count; n++ )
					{
						var index = indexes[ n ];
						ref var obj = ref newObjects[ n ];

						Component_Surface.GetRandomVariationOptions options;
						if( randomizeGroups )
							options = new Component_Surface.GetRandomVariationOptions();
						else
							options = new Component_Surface.GetRandomVariationOptions( obj.VariationGroup );
						surface.GetRandomVariation( options, random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );

						obj.UniqueIdentifier = 0;
						obj.VariationGroup = groupIndex;
						obj.VariationElement = elementIndex;
						if( AutoAlign && scene != null )
						{
							var r = Component_Scene_Utility.CalculateObjectPositionZ( scene, groupOfObjects, obj.Position.Z, obj.Position.ToVector2() );
							if( r.found )
								obj.Position.Z = r.positionZ + positionZ;
						}
						obj.Rotation = rotation;
						obj.Scale = scale;
					}

					//delete and undo to delete
					undoMultiAction.AddAction( new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true ) );

					//add new data
					var newIndexes = groupOfObjects.ObjectsAdd( newObjects );
					undoMultiAction.AddAction( new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, newIndexes, true, false ) );
				}
			}
		}

		public virtual void UpdateAlignment( UndoMultiAction undoMultiAction )
		{
			var random = new Random();

			var groupOfObjects = Parent as Component_GroupOfObjects;
			if( groupOfObjects != null )
			{
				var surface = Surface.Value;
				if( surface != null )
				{
					var indexes = GetObjectsOfElement();
					var scene = groupOfObjects.FindParent<Component_Scene>();

					var newObjects = groupOfObjects.ObjectsGetData( indexes );

					for( int n = 0; n < indexes.Count; n++ )
					{
						var index = indexes[ n ];
						ref var obj = ref newObjects[ n ];

						double positionZ = 0;

						var group = surface.GetGroup( obj.VariationGroup );
						if( group != null )
						{
							//PositionZRange
							var positionZRange = group.PositionZRange.Value;
							if( positionZRange.Minimum != positionZRange.Maximum )
								positionZ = random.Next( positionZRange.Minimum, positionZRange.Maximum );
							else
								positionZ = positionZRange.Minimum;

						}

						//Component_Surface.GetRandomVariationOptions options;
						//if( randomizeGroups )
						//	options = new Component_Surface.GetRandomVariationOptions();
						//else
						//	options = new Component_Surface.GetRandomVariationOptions( obj.VariationGroup );
						//surface.GetRandomVariation( options, random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );

						obj.UniqueIdentifier = 0;
						//obj.VariationGroup = groupIndex;
						//obj.VariationElement = elementIndex;
						if( AutoAlign && scene != null )
						{
							var r = Component_Scene_Utility.CalculateObjectPositionZ( scene, groupOfObjects, obj.Position.Z, obj.Position.ToVector2() );
							if( r.found )
								obj.Position.Z = r.positionZ + positionZ;
						}
						//obj.Rotation = rotation;
						//obj.Scale = scale;
					}

					//delete and undo to delete
					undoMultiAction.AddAction( new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true ) );

					//add new data
					var newIndexes = groupOfObjects.ObjectsAdd( newObjects );
					undoMultiAction.AddAction( new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, newIndexes, true, false ) );
				}
			}
		}

	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//public class Component_GroupOfObjectsElement_Billboard : Component_GroupOfObjectsElement
	//{
	//	[DefaultValue( null )]
	//	public Reference<Component_Material> Material
	//	{
	//		get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
	//		set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
	//	}
	//	public event Action<Component_GroupOfObjectsElement_Billboard> MaterialChanged;
	//	ReferenceField<Component_Material> _material = null;
	//}
}
