// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NeoAxis.Editor;

namespace NeoAxis
{
	public class Item3DAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Item 3D Type", new string[] { "itemtype" }, typeof( Resource ) );

#if !DEPLOY
			if( EngineApp.IsEditor )
			{
				SceneEditorUtility.CreateObjectWhatTypeWillCreatedEvent += Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent;
				SceneEditorUtility.CreateObjectByCreationDataEvent += Scene_DocumentWindow_CreateObjectByCreationDataEvent;

				EditorAPI.PreviewImagesManager_RegisterResourceType( "Item 3D Type" );

				//Product_Store.CreateScreenshot += Product_Store_CreateScreenshot;
			}
#endif
		}

#if !DEPLOY
		private void Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type )
		{
			if( MetadataManager.GetTypeOfNetType( typeof( FlashlightType ) ).IsAssignableFrom( objectType ) )
				type = MetadataManager.GetTypeOfNetType( typeof( Flashlight ) );
			else if( MetadataManager.GetTypeOfNetType( typeof( Item3DType ) ).IsAssignableFrom( objectType ) )
				type = MetadataManager.GetTypeOfNetType( typeof( Item3D ) );
		}

		private void Scene_DocumentWindow_CreateObjectByCreationDataEvent( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject )
		{
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( FlashlightType ) ).IsAssignableFrom( objectType ) )
			{
				var obj = createTo.CreateComponent<Flashlight>( enabled: false );
				newObject = obj;
				if( !string.IsNullOrEmpty( referenceToObject ) )
					obj.ItemType = new ReferenceNoValue( referenceToObject );
			}
			else if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( Item3DType ) ).IsAssignableFrom( objectType ) )
			{
				var obj = createTo.CreateComponent<Item3D>( enabled: false );
				newObject = obj;
				if( !string.IsNullOrEmpty( referenceToObject ) )
					obj.ItemType = new ReferenceNoValue( referenceToObject );
			}

			if( newObject != null )
			{
				try
				{
					var name = Path.GetFileNameWithoutExtension( referenceToObject );
					if( !string.IsNullOrEmpty( name ) )
						newObject.Name = EditorUtility.GetUniqueFriendlyName( newObject, name );
				}
				catch { }
			}
		}

#endif
	}
}
