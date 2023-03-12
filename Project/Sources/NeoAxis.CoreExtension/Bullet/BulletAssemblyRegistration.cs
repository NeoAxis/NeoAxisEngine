// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The class is intended to register bullet resources.
	/// </summary>
	public class BulletAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Bullet Type", new string[] { "bullettype" }, typeof( Resource ) );

#if !DEPLOY
			if( EngineApp.IsEditor )
			{
				SceneEditor.CreateObjectWhatTypeWillCreatedEvent += Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent;
				SceneEditor.CreateObjectByCreationDataEvent += Scene_DocumentWindow_CreateObjectByCreationDataEvent;

				PreviewImagesManager.RegisterResourceType( "Bullet Type" );
			}
#endif
		}

#if !DEPLOY
		private void Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type )
		{
			if( MetadataManager.GetTypeOfNetType( typeof( BulletType ) ).IsAssignableFrom( objectType ) )
				type = MetadataManager.GetTypeOfNetType( typeof( Bullet ) );
		}

		private void Scene_DocumentWindow_CreateObjectByCreationDataEvent( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject )
		{
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( BulletType ) ).IsAssignableFrom( objectType ) )
			{
				var obj = createTo.CreateComponent<Bullet>( enabled: false );
				newObject = obj;
				obj.BulletType = new Reference<BulletType>( null, referenceToObject );
			}
		}
#endif
	}
}
