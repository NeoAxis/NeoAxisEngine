// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

				Product_Store.CreateScreenshot += Product_Store_CreateScreenshot;
			}
#endif
		}

#if !DEPLOY
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

		private void Product_Store_CreateScreenshot( Product_Store sender, string[] files, ZipArchive archive, ref bool handled )
		{
			var virtualFileName = "";

			foreach( var file in files )
			{
				var ext = Path.GetExtension( file );
				if( !string.IsNullOrEmpty( ext ) )
				{
					var ext2 = ext.ToLower().Replace( ".", "" );
					if( ext2 == "fencetype" )
					{
						virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
						if( !string.IsNullOrEmpty( virtualFileName ) )
							break;
					}
				}
			}

			if( !string.IsNullOrEmpty( virtualFileName ) )
			{
				var fenceType = ResourceManager.LoadResource<FenceType>( virtualFileName );
				if( fenceType != null )
				{
					var objectInSpace = ComponentUtility.CreateComponent<Fence>( null, false, true );
					objectInSpace.FenceType = fenceType;

					var panelLength = fenceType.PanelLength.Value;

					{
						var point = objectInSpace.CreateComponent<FencePoint>();
						point.Transform = new Transform( new Vector3( panelLength * 2, 0, 0 ) );
					}

					{
						var point = objectInSpace.CreateComponent<FencePoint>();
						point.Transform = new Transform( new Vector3( panelLength * 2, panelLength * 2, 0 ) );
					}

					{
						var point = objectInSpace.CreateComponent<FencePoint>();
						point.Transform = new Transform( new Vector3( panelLength * 4, panelLength * 2, 0 ) );
					}

					{
						var point = objectInSpace.CreateComponent<FencePoint>();
						point.Transform = new Transform( new Vector3( panelLength * 6, panelLength * 2, 0 ) );
					}

					var generator = new Product_Store.ImageGenerator();
					generator.CameraZoomFactor = 0.5;

					generator.CameraLookTo = new Vector3( panelLength * 3.4, panelLength * 1.6, 0 );
					//var mesh = fenceType.Mesh.Value;
					//if( mesh != null )
					//	generator.CameraLookTo = new Vector3( mesh.Result.SpaceBounds.BoundingBox.GetSize().X * 0.15, 0, 0 );

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
