// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The class is intended to register character resource.
	/// </summary>
	public class CharacterAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			//file extension
			var type = ResourceManager.RegisterType( "Character", new string[] { "character" }, typeof( Resource ) );
#if !DEPLOY
			Editor.PreviewImagesManager.RegisterResourceType( type );
#endif

			////editor actions
			//if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
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
			//if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
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
	}
}
