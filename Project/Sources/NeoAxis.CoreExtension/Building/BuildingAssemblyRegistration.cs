// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	public class BuildingAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Building Type", new string[] { "buildingtype" }, typeof( Resource ) );
			ResourceManager.RegisterType( "Building", new string[] { "building" }, typeof( Resource ) );

#if !DEPLOY
			if( EngineApp.IsEditor )
			{
				SceneEditor.CreateObjectWhatTypeWillCreatedEvent += Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent;
				SceneEditor.CreateObjectByCreationDataEvent += Scene_DocumentWindow_CreateObjectByCreationDataEvent;

				PreviewImagesManager.RegisterResourceType( "Building Type" );
				//need?
				//PreviewImagesManager.RegisterResourceType( "Building" );
			}
#endif
		}

		private void Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type )
		{
			if( MetadataManager.GetTypeOfNetType( typeof( BuildingType ) ).IsAssignableFrom( objectType ) )
				type = MetadataManager.GetTypeOfNetType( typeof( Building ) );
		}

		private void Scene_DocumentWindow_CreateObjectByCreationDataEvent( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject )
		{
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( BuildingType ) ).IsAssignableFrom( objectType ) )
			{
				var obj = createTo.CreateComponent<Building>( enabled: false );
				newObject = obj;
				obj.BuildingType = new Reference<BuildingType>( null, referenceToObject );
			}
		}
	}
}
