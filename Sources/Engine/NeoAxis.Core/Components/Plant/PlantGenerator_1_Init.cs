// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	public partial class PlantGenerator
	{
		public PlantType PlantType;
		public int Seed;
		public double Age;
		//public double Dead;
		//public double Fired;
		//public double Season;
		public int LOD;
		//public int LODQuality;
		public Transform WorldTransform;
		public string MeshRealFileName = "";

		public PlantMaterial[] Materials;
		public Mesh.CompiledData WriteToCompiledData;
		public Mesh WriteToMesh;

		public FastRandom Randomizer;

		public double Height;

		public object AnyData;

		public Bounds Bounds;

		//

		public PlantGenerator( PlantType plantType, int seed, double age/*, double dead, double fired, double season*/, int lod, Transform worldTransform, Mesh.CompiledData writeToCompiledData, Mesh writeToMesh )
		{
			PlantType = plantType;
			Seed = seed;
			Age = age;
			if( Age <= 0 )
				Age = PlantType.MatureAge;
			//Dead = dead;
			//Fired = fired;
			//Season = season;
			LOD = lod;
			//LODQuality = lodBillboard ? 0 : lod;
			WorldTransform = worldTransform;

			Materials = PlantType.GetComponents<PlantMaterial>( onlyEnabledInHierarchy: true );
			if( PlantType == null )
				PlantType = new PlantType();
			WriteToCompiledData = writeToCompiledData;
			WriteToMesh = writeToMesh;
		}

		public void Generate()
		{
			Randomizer = new FastRandom( Seed, true );

			Height = Randomizer.Next( PlantType.MatureHeight.Value.Minimum, PlantType.MatureHeight.Value.Maximum );
			if( Age < PlantType.MatureAge )
				Height *= Age / PlantType.MatureAge;

			GenerateStructure();
			GenerateMeshData();
		}
	}
}
#endif