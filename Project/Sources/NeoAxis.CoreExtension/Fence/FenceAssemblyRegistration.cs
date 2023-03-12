// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	public class FenceAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Fence Type", new string[] { "fencetype" }, typeof( Resource ) );

#if !DEPLOY
			if( EngineApp.IsEditor )
			{
				SceneEditor.CreateObjectWhatTypeWillCreatedEvent += Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent;
				SceneEditor.CreateObjectByCreationDataEvent += Scene_DocumentWindow_CreateObjectByCreationDataEvent;

				PreviewImagesManager.RegisterResourceType( "Fence Type" );
			}
#endif
		}

		private void Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type )
		{
			if( MetadataManager.GetTypeOfNetType( typeof( FenceType ) ).IsAssignableFrom( objectType ) )
				type = MetadataManager.GetTypeOfNetType( typeof( Fence ) );
		}

		private void Scene_DocumentWindow_CreateObjectByCreationDataEvent( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject )
		{
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( FenceType ) ).IsAssignableFrom( objectType ) )
			{
				var obj = createTo.CreateComponent<Fence>( enabled: false );
				newObject = obj;
				obj.FenceType = new Reference<FenceType>( null, referenceToObject );
			}
		}
	}
}
