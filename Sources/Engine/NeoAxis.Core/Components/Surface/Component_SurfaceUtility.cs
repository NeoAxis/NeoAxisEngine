// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Linq;

namespace NeoAxis
{
	static class Component_SurfaceUtility
	{
		public static void CreatePreviewObjects( Component_Scene scene, Component_Surface surface )
		{
			DestroyPreviewObjects( scene );

			var center = Vector3.Zero;

			//!!!!среднее от всех групп
			double minDistanceBetweenObjects;
			{
				var groups = surface.GetComponents<Component_SurfaceGroupOfElements>();
				if( groups.Length != 0 )
				{
					minDistanceBetweenObjects = 0;
					foreach( var group in groups )
						minDistanceBetweenObjects += group.MinDistanceBetweenObjects;
					minDistanceBetweenObjects /= groups.Length;
				}
				else
					minDistanceBetweenObjects = 1;
			}

			//!!!!
			var toolRadius = minDistanceBetweenObjects * 5;// CreateObjectsBrushRadius;
			var toolStrength = 1.0;// CreateObjectsBrushStrength;
			var toolHardness = 0;// CreateObjectsBrushHardness;
			var random = new Random( 0 );

			double GetHardnessFactor( double length )
			{
				if( length == 0 || length <= toolHardness * toolRadius )
					return 1;
				else
				{
					double c;
					if( toolRadius - toolRadius * toolHardness != 0 )
						c = ( length - toolRadius * toolHardness ) / ( toolRadius - toolRadius * toolHardness );
					else
						c = 0;
					return (float)Math.Cos( Math.PI / 2 * c );
				}
			}

			//calculate object count
			int count;
			{
				var toolSquare = Math.PI * toolRadius * toolRadius;

				double radius = minDistanceBetweenObjects / 2;
				double objectSquare = Math.PI * radius * radius;
				if( objectSquare < 0.1 )
					objectSquare = 0.1;

				double maxCount = toolSquare / objectSquare;
				maxCount /= 10;

				count = (int)( toolStrength * (double)maxCount );
				count = Math.Max( count, 1 );


				count *= 20;
			}

			var data = new List<Component_GroupOfObjects.Object>( count );

			//create point container to check by MinDistanceBetweenObjects
			PointContainer3D pointContainerFindFreePlace;
			{
				double minDistanceBetweenObjectsMax = 0;
				foreach( var group in surface.GetComponents<Component_SurfaceGroupOfElements>() )
					minDistanceBetweenObjectsMax = Math.Max( minDistanceBetweenObjectsMax, group.MinDistanceBetweenObjects );

				var bounds = new Bounds( center );
				bounds.Expand( toolRadius + minDistanceBetweenObjectsMax );
				pointContainerFindFreePlace = new PointContainer3D( bounds, 100 );
			}

			for( int n = 0; n < count; n++ )
			{
				surface.GetRandomVariation( new Component_Surface.GetRandomVariationOptions(), random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );
				var surfaceGroup = surface.GetGroup( groupIndex );

				Vector3? position = null;

				int counter = 0;
				while( counter < 20 )
				{
					var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

					//check by radius and by hardness
					var length = offset.Length();
					if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
					{
						var position2 = center.ToVector2() + offset;

						//var result = Component_Scene_Utility.CalculateObjectPositionZ( Scene, toGroupOfObjects, center.Z, position2, destinationCachedBaseObjects );
						//if( result.found )
						//{

						var p = new Vector3( position2, 0 );// result.positionZ );

						//check by MinDistanceBetweenObjects
						if( surfaceGroup == null || !pointContainerFindFreePlace.Contains( new Sphere( p, surfaceGroup.MinDistanceBetweenObjects ) ) )
						{
							//found place to create
							position = p;
							break;
						}

						//}
					}

					counter++;
				}

				if( position != null )
				{
					var objPosition = position.Value + new Vector3( 0, 0, positionZ );
					var objRotation = rotation;
					var objScale = scale;


					var surfaceElement = surfaceGroup.GetElement( elementIndex );

					var surfaceElementMesh = surfaceElement as Component_SurfaceElement_Mesh;
					if( surfaceElementMesh != null )
					{
						var meshInSpace = scene.CreateComponent<Component_MeshInSpace>( enabled: false );
						meshInSpace.Transform = new Transform( objPosition, objRotation, objScale );

						//!!!!так копировать?
						meshInSpace.Mesh = surfaceElementMesh.Mesh;
						if( meshInSpace.Mesh.Value == null )
							meshInSpace.Mesh = ResourceUtility.MeshInvalid;

						//!!!!так копировать?
						if( surfaceElementMesh.ReplaceMaterial.ReferenceSpecified )
							meshInSpace.ReplaceMaterial = surfaceElementMesh.ReplaceMaterial;

						meshInSpace.Enabled = true;
					}

					//add to point container
					pointContainerFindFreePlace.Add( ref objPosition );
				}
			}
		}

		public static void DestroyPreviewObjects( Component_Scene scene )
		{
			foreach( var c in scene.GetComponents<Component_MeshInSpace>() )
				c.Dispose();
		}
	}
}
