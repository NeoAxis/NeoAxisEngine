// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	public static class CharacterUtility
	{
		public static bool FindFreePlace( Scene scene, double characterHeight, double characterRadius, Vector3 position, double maxRadius, double minHeight, double maxHeight, Character skipCharacter, out Vector3 freePlacePosition )
		{
			freePlacePosition = position;

			var from = position + new Vector3( 0, 0, maxHeight );
			var to = position + new Vector3( 0, 0, minHeight );

			for( var radius = 0.0; radius <= maxRadius; radius += characterRadius )
			{
				for( var angle = 0.0; angle < Math.PI * 2; angle += Math.PI / 8 )
				{
					var offset = new Vector3( Math.Cos( angle ) * radius, Math.Sin( angle ) * radius, 0 );
					var from2 = from + offset;
					var to2 = to + offset;

					var capsule = new Capsule( from2 + new Vector3( 0, 0, characterRadius ), from2 + new Vector3( 0, 0, characterHeight - characterRadius ), characterRadius );
					var direction = to2 - from2;

					var volumeTestItem = new PhysicsVolumeTestItem( capsule, direction, PhysicsVolumeTestItem.ModeEnum.OneClosestForEach );
					scene.PhysicsVolumeTest( volumeTestItem );

					var existsSomething = false;

					foreach( var resultItem in volumeTestItem.Result )
					{
						if( skipCharacter != null && resultItem.Body == skipCharacter.PhysicalBody )
							continue;

						if( resultItem.DistanceScale == 0 )
						{
							existsSomething = true;
							break;
						}

						freePlacePosition = Vector3.Lerp( from2, to2, resultItem.DistanceScale );
						return true;
					}

					if( !existsSomething )
					{
						freePlacePosition = position + offset;
						return true;
					}

					//one check when radius is 0
					if( radius == 0 )
						continue;
				}
			}

			return false;
		}

		public static bool FindFreePlace( Character character, Vector3 position, double maxRadius, double minHeight, double maxHeight, out Vector3 freePlacePosition )
		{
			var scene = character.ParentScene;
			if( scene != null )
			{
				var characterHeight = character.TypeCached.Height * character.GetScaleFactor();
				var characterRadius = character.TypeCached.Radius * character.GetScaleFactor();

				return FindFreePlace( scene, characterHeight, characterRadius, position, maxRadius, minHeight, maxHeight, character, out freePlacePosition );
			}

			freePlacePosition = position;
			return false;
		}

		public static Character FindClosestCharacter( Scene scene, Sphere sphere )
		{
			var volumeTestItem = new PhysicsVolumeTestItem( sphere, Vector3.Zero, PhysicsVolumeTestItem.ModeEnum.OneClosestForEach );
			scene.PhysicsVolumeTest( volumeTestItem );

			Character closestCharacter = null;
			var closestCharacterDistance = 0.0;

			foreach( var resultItem in volumeTestItem.Result )
			{
				var character = resultItem.Body.Owner as Character;
				if( character != null )
				{
					var distance = ( character.GetCenteredPosition() - sphere.Center ).Length();
					if( closestCharacter == null || distance < closestCharacterDistance )
					{
						closestCharacter = character;
						closestCharacterDistance = distance;
					}
				}
			}

			return closestCharacter;
		}
	}
}
