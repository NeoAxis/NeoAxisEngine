// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The class is intended to register character resource.
	/// </summary>
	public class CharacterAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Character Type", new string[] { "charactertype" }, typeof( Resource ) );
			ResourceManager.RegisterType( "Character Maker", new string[] { "charactermaker" }, typeof( Resource ) );
			//ResourceManager.RegisterType( "Character", new string[] { "character" }, typeof( Resource ) );

#if !DEPLOY
			if( EngineApp.IsEditor )
			{
				SceneEditorUtility.CreateObjectWhatTypeWillCreatedEvent += Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent;
				SceneEditorUtility.CreateObjectByCreationDataEvent += Scene_DocumentWindow_CreateObjectByCreationDataEvent;

				EditorAPI.PreviewImagesManager_RegisterResourceType( "Character Type" );
				//PreviewImagesManager.RegisterResourceType( "Character" );
			}
#endif

			////editor actions
			//if( EngineApp.IsEditor )
			//{
			//	//Character Display Physics
			//	{
			//		var a = new EditorAction();
			//		a.Name = "Character Display Physics";
			//		//a.Description = "";
			//		a.ImageSmall = Properties.Resources.Default_16;
			//		a.ImageBig = Properties.Resources.MeshCollision_32;
			//		a.QatSupport = true;
			//		a.RibbonText = ("Display", "Physics");
			//		EditorActions.Register( a );
			//	}
			//}

			////ribbon menu
			//if( EngineApp.IsEditor )
			//{
			//	var tab = new EditorRibbonDefaultConfiguration.Tab( "Character Editor", "CharacterEditor", MetadataManager.GetTypeOfNetType( typeof( Character ) ) );
			//	EditorRibbonDefaultConfiguration.Tabs.Add( tab );

			//	//Character
			//	{
			//		var group = new EditorRibbonDefaultConfiguration.Group( "Character" );
			//		tab.Groups.Add( group );
			//		group.AddAction( "Character Display Physics" );
			//	}
			//}

		}

#if !DEPLOY
		private void Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type )
		{
			if( MetadataManager.GetTypeOfNetType( typeof( CharacterType ) ).IsAssignableFrom( objectType ) )
				type = MetadataManager.GetTypeOfNetType( typeof( Character ) );
		}

		private void Scene_DocumentWindow_CreateObjectByCreationDataEvent( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject )
		{
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( CharacterType ) ).IsAssignableFrom( objectType ) )
			{
				var obj = createTo.CreateComponent<Character>( enabled: false );
				newObject = obj;
				obj.CharacterType = new Reference<CharacterType>( null, referenceToObject );
			}
		}
#endif

	}
}
