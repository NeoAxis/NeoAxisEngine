// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class for working with <see cref="GroupOfObjects"/>.
	/// </summary>
	public static class GroupOfObjectsUtility
	{
		public class GroupOfObjectsInstance
		{
			internal GroupOfObjects groupOfObjects;
			Dictionary<(Mesh mesh, Material, bool, double, bool, double, bool, bool), ElementsDictionaryValue> elementsDictionary = new Dictionary<(Mesh mesh, Material, bool, double, bool, double, bool, bool), ElementsDictionaryValue>();
			Dictionary<ushort, int> elementsInUse = new Dictionary<ushort, int>();

			//////////////////////////////////////////////

			struct ElementsDictionaryValue
			{
				public (Mesh mesh, Material, bool, double, bool, double, bool, bool) key;
				public GroupOfObjectsElement_Mesh element;
			}

			//////////////////////////////////////////////

			public ushort GetOrCreateGroupOfObjectsElement( Mesh mesh, Material replaceMaterial, bool castShadows, double visibilityDistanceFactor, bool receiveDecals, double motionBlurFactor, bool staticShadows, bool collision )
			{
				//!!!!может какие-то параметры в Object
				var key = (mesh, replaceMaterial, castShadows, visibilityDistanceFactor, receiveDecals, motionBlurFactor, staticShadows, collision);

				GroupOfObjectsElement_Mesh element = null;

				if( elementsDictionary.TryGetValue( key, out var elementItem2 ) )
					return (ushort)elementItem2.element.Index;

				if( element == null )
				{
					var elementIndex = groupOfObjects.GetFreeElementIndex();
					element = groupOfObjects.CreateComponent<GroupOfObjectsElement_Mesh>( enabled: false );
					element.Name = "Element " + elementIndex.ToString();
					element.Index = elementIndex;
					element.Mesh = mesh;
					element.ReplaceMaterial = replaceMaterial;
					element.AutoAlign = false;
					element.CastShadows = castShadows;
					element.VisibilityDistanceFactor = visibilityDistanceFactor;
					element.ReceiveDecals = receiveDecals;
					element.MotionBlurFactor = motionBlurFactor;
					element.StaticShadows = staticShadows;
					element.Collision = collision;
					element.Enabled = true;

					elementsDictionary[ key ] = new ElementsDictionaryValue() { key = key, element = element };

					//!!!!how without it? везде так
					groupOfObjects.ElementTypesCacheNeedUpdate();
				}

				return (ushort)element.Index.Value;
			}

			public void AddSubGroup( GroupOfObjects.SubGroup subGroup )
			{
				if( subGroup.Added )
					Log.Fatal( "GroupOfObjectsUtility: GroupOfObjectsInstance: AddSubGroup: subGroup.Added." );

				for( int n = 0; n < subGroup.Objects.Count; n++ )
				{
					ref var obj = ref subGroup.Objects.Array[ subGroup.Objects.Offset + n ];//var obj = subGroup.Objects[ n ];

					subGroup.elementsInUse.TryGetValue( obj.Element, out var count );
					count++;
					subGroup.elementsInUse[ obj.Element ] = count;
				}

				groupOfObjects.AddSubGroupToQueue( subGroup );

				foreach( var pair in subGroup.elementsInUse )
				{
					elementsInUse.TryGetValue( pair.Key, out var count );
					count += pair.Value;
					elementsInUse[ pair.Key ] = count;
				}

				subGroup.Added = true;
			}

			public void RemoveSubGroup( GroupOfObjects.SubGroup subGroup, bool canRemoveElements = true )
			{
				if( !subGroup.Added )
					Log.Fatal( "GroupOfObjectsUtility: GroupOfObjectsInstance: RemoveSubGroup: !subGroup.Added." );

				foreach( var pair in subGroup.elementsInUse )
				{
					elementsInUse.TryGetValue( pair.Key, out var count );
					count -= pair.Value;

					if( count > 0 )
						elementsInUse[ pair.Key ] = count;
					else
					{
						elementsInUse.Remove( pair.Key );

						//remove element component
						if( canRemoveElements )
						{
							var element = groupOfObjects.GetElement( pair.Key );
							if( element != null )//must be always not null
							{
								groupOfObjects.RemoveElementToQueue( element );

								foreach( var pair2 in elementsDictionary )
								{
									if( pair2.Value.element == element )
									{
										elementsDictionary.Remove( pair2.Key );
										break;
									}
								}
							}
						}
					}
				}

				////slowly
				//var objects = groupOfObjects.ObjectsGetData( subGroup.ObjectIndexes );

				////can't use subGroup.Objects because it is cleared for memory optimization
				//for( int n = 0; n < objects.Length; n++ )//for( int n = 0; n < subGroup.Objects.Count; n++ )
				//{
				//	ref var obj = ref objects[ n ];//var obj = subGroup.Objects[ n ];

				//	elementsInUse.TryGetValue( obj.Element, out var count );
				//	count--;

				//	if( count > 0 )
				//		elementsInUse[ obj.Element ] = count;
				//	else
				//	{
				//		elementsInUse.Remove( obj.Element );

				//		//remove element component
				//		if( canRemoveElements )
				//		{
				//			var element = groupOfObjects.GetElement( obj.Element );
				//			if( element != null )//must be always not null
				//			{
				//				groupOfObjects.RemoveElementToQueue( element );

				//				foreach( var pair in elementsDictionary )
				//				{
				//					if( pair.Value.element == element )
				//					{
				//						elementsDictionary.Remove( pair.Key );
				//						break;
				//					}
				//				}
				//			}
				//		}
				//	}
				//}

				groupOfObjects.RemoveSubGroupToQueue( subGroup );

				subGroup.Added = false;
			}

			public void Dispose()
			{
				groupOfObjects?.Dispose();
			}
		}

		public static GroupOfObjectsInstance GetOrCreateGroupOfObjects( Scene scene, string groupOfObjectsName, bool canCreate, Vector3? sectorSize = null )//, int? maxObjectsInGroup = null )
		{
			var group = scene.GetComponent<GroupOfObjects>( groupOfObjectsName );
			if( group == null && canCreate )
			{
				//need set ShowInEditor = false before AddComponent
				group = ComponentUtility.CreateComponent<GroupOfObjects>( null, false, false );
				group.NetworkMode = NetworkModeEnum.False;
				group.DisplayInEditor = false;
				scene.AddComponent( group, -1 );
				//var group = scene.CreateComponent<GroupOfObjects>();

				group.Name = groupOfObjectsName;
				group.SaveSupport = false;
				group.CloneSupport = false;
				group.NetworkMode = NetworkModeEnum.False;

				var groupInstance = new GroupOfObjectsInstance();
				groupInstance.groupOfObjects = group;

				group.AnyData = groupInstance;

				if( sectorSize.HasValue )
					group.SectorSize = sectorSize.Value;
				//if( maxObjectsInGroup.HasValue )
				//	group.MaxObjectsInGroup = maxObjectsInGroup.Value;

				group.Enabled = true;
			}

			return (GroupOfObjectsInstance)group.AnyData;
		}

		public static void DestroyGroupOfObjects( Scene scene, string groupOfObjectsName )
		{
			var group = scene.GetComponent<GroupOfObjects>( groupOfObjectsName );
			if( group != null )
				group.Dispose();
		}

		public static void UpdateGroupOfObjects( Scene scene, string groupOfObjectsName, Vector3 sectorSize )//, int maxObjectsInGroup )
		{
			var group = scene.GetComponent<GroupOfObjects>( groupOfObjectsName );
			if( group != null )
			{
				group.SectorSize = sectorSize;
				//group.MaxObjectsInGroup = maxObjectsInGroup;
			}
		}
	}
}
