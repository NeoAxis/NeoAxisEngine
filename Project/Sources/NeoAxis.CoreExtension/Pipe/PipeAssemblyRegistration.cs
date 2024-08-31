// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NeoAxis.Editor;

namespace NeoAxis
{
	public class PipeAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Pipe Type", new string[] { "pipetype" }, typeof( Resource ) );

#if !DEPLOY
			if( EngineApp.IsEditor )
			{
				SceneEditorUtility.CreateObjectWhatTypeWillCreatedEvent += Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent;
				SceneEditorUtility.CreateObjectByCreationDataEvent += Scene_DocumentWindow_CreateObjectByCreationDataEvent;

				EditorAPI.PreviewImagesManager_RegisterResourceType( "Pipe Type" );

				Product_Store.CreateScreenshots += Product_Store_CreateScreenshots;
			}
#endif
		}

#if !DEPLOY
		private void Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type )
		{
			if( MetadataManager.GetTypeOfNetType( typeof( PipeType ) ).IsAssignableFrom( objectType ) )
				type = MetadataManager.GetTypeOfNetType( typeof( Pipe ) );
		}

		private void Scene_DocumentWindow_CreateObjectByCreationDataEvent( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject )
		{
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( PipeType ) ).IsAssignableFrom( objectType ) )
			{
				var obj = createTo.CreateComponent<Pipe>( enabled: false );
				newObject = obj;
				obj.PipeType = new Reference<PipeType>( null, referenceToObject );
			}
		}

		private void Product_Store_CreateScreenshots( Product_Store sender, string[] files, ZipArchive archive, bool additional, ref int imageCounter, ref bool handled )
		{
			var virtualFileName = "";

			foreach( var file in files )
			{
				var ext = Path.GetExtension( file );
				if( !string.IsNullOrEmpty( ext ) )
				{
					var ext2 = ext.ToLower().Replace( ".", "" );
					if( ext2 == "pipetype" )
					{
						virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
						if( !string.IsNullOrEmpty( virtualFileName ) )
							break;
					}
				}
			}

			if( !string.IsNullOrEmpty( virtualFileName ) )
			{
				var pipeType = ResourceManager.LoadResource<PipeType>( virtualFileName );
				if( pipeType != null )
				{
					var objectInSpace = ComponentUtility.CreateComponent<Pipe>( null, false, true );
					objectInSpace.PipeType = pipeType;

					var panelLength = pipeType.OutsideDiameter.Value * 20;

					{
						var point = objectInSpace.CreateComponent<PipePoint>();
						point.Transform = new Transform( new Vector3( panelLength, 0, 0 ) );
					}

					{
						var point = objectInSpace.CreateComponent<PipePoint>();
						point.Transform = new Transform( new Vector3( panelLength, panelLength, 0 ) );
					}

					{
						var point = objectInSpace.CreateComponent<PipePoint>();
						point.Transform = new Transform( new Vector3( panelLength * 2, panelLength, 0 ) );
						point.Specialty = PipePoint.SpecialtyEnum.Socket;
					}

					{
						var point = objectInSpace.CreateComponent<PipePoint>();
						point.Transform = new Transform( new Vector3( panelLength * 3, panelLength, 0 ) );
					}

					var generator = new Product_Store.ImageGenerator();
					generator.CameraZoomFactor = 0.43;

					generator.CameraLookTo = new Vector3( panelLength * 2.0, panelLength * 0.5, 0 );

					var entry = archive.CreateEntry( "_ProductLogo.png" );
					using( var entryStream = entry.Open() )
						generator.Generate( objectInSpace, entryStream );//, ImageFormat.Png );
				}

				handled = true;
			}
		}
#endif
	}
}
