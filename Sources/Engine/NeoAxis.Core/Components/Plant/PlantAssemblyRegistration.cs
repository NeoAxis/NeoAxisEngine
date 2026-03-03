// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

//to fix scripts compilation
#if DEPLOY
namespace NeoAxis
{
	public class PlantType
	{
	}

	public class PlantGenerator
	{
		public enum ElementTypeEnum
		{
		}
	}
}
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if !DEPLOY
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	public class PlantAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Plant Type", new string[] { "planttype" }, typeof( Resource ) );

			//.plant maybe a unique instance of the type


			//if( EngineApp.IsEditor )
			//{
			//SceneEditor.CreateObjectWhatTypeWillCreatedEvent += Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent;
			//SceneEditor.CreateObjectByCreationDataEvent += Scene_DocumentWindow_CreateObjectByCreationDataEvent;

			//PreviewImagesManager.RegisterResourceType( "Plant" );
			//}
		}

		//private void Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type )
		//{
		//	if( MetadataManager.GetTypeOfNetType( typeof( Plant ) ).IsAssignableFrom( objectType ) )
		//		type = MetadataManager.GetTypeOfNetType( typeof( MeshInSpace ) );
		//}

		//private void Scene_DocumentWindow_CreateObjectByCreationDataEvent( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject )
		//{
		//	if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( PlantType ) ).IsAssignableFrom( objectType ) )
		//	{
		//		var obj = createTo.CreateComponent<MeshInSpace>( enabled: false );
		//		newObject = obj;

		//		var mesh = obj.CreateComponent<PlantMesh>();
		//		mesh.Name = "Plant Mesh";
		//		mesh.PlantType = new Reference<PlantType>( null, referenceToObject );

		//		obj.Mesh = ReferenceUtility.MakeThisReference( obj, mesh );
		//	}
		//}
	}
}
#endif