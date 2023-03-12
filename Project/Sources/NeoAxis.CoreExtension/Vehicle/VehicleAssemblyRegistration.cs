// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The class is intended to register vehicle resources.
	/// </summary>
	public class VehicleAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Vehicle Type", new string[] { "vehicletype" }, typeof( Resource ) );
			//ResourceManager.RegisterType( "Vehicle", new string[] { "vehicle" }, typeof( Resource ) );

#if !DEPLOY
			if( EngineApp.IsEditor )
			{
				SceneEditor.CreateObjectWhatTypeWillCreatedEvent += Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent;
				SceneEditor.CreateObjectByCreationDataEvent += Scene_DocumentWindow_CreateObjectByCreationDataEvent;

				PreviewImagesManager.RegisterResourceType( "Vehicle Type" );
				//PreviewImagesManager.RegisterResourceType( "Vehicle" );
			}
#endif
		}

#if !DEPLOY
		private void Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type )
		{
			if( MetadataManager.GetTypeOfNetType( typeof( VehicleType ) ).IsAssignableFrom( objectType ) )
				type = MetadataManager.GetTypeOfNetType( typeof( Vehicle ) );
		}

		private void Scene_DocumentWindow_CreateObjectByCreationDataEvent( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject )
		{
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( VehicleType ) ).IsAssignableFrom( objectType ) )
			{
				var obj = createTo.CreateComponent<Vehicle>( enabled: false );
				newObject = obj;
				obj.VehicleType = new Reference<VehicleType>( null, referenceToObject );
			}
		}
#endif
	}
}
