// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The class is intended to register seat resources.
	/// </summary>
	public class SeatAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Seat Type", new string[] { "seattype" }, typeof( Resource ) );

#if !DEPLOY
			if( EngineApp.IsEditor )
			{
				SceneEditorUtility.CreateObjectWhatTypeWillCreatedEvent += Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent;
				SceneEditorUtility.CreateObjectByCreationDataEvent += Scene_DocumentWindow_CreateObjectByCreationDataEvent;

				EditorAPI.PreviewImagesManager_RegisterResourceType( "Seat Type" );

				Product_Store.CreateScreenshot += Product_Store_CreateScreenshot;
			}
#endif
		}

#if !DEPLOY
		private void Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type )
		{
			if( MetadataManager.GetTypeOfNetType( typeof( SeatType ) ).IsAssignableFrom( objectType ) )
				type = MetadataManager.GetTypeOfNetType( typeof( Seat ) );
		}

		private void Scene_DocumentWindow_CreateObjectByCreationDataEvent( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject )
		{
			if( newObject == null && MetadataManager.GetTypeOfNetType( typeof( SeatType ) ).IsAssignableFrom( objectType ) )
			{
				var obj = createTo.CreateComponent<Seat>( enabled: false );
				newObject = obj;
				obj.SeatType = new Reference<SeatType>( null, referenceToObject );
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
					if( ext2 == "seattype" )
					{
						virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
						if( !string.IsNullOrEmpty( virtualFileName ) )
							break;
					}
				}
			}

			if( !string.IsNullOrEmpty( virtualFileName ) )
			{
				var seatType = ResourceManager.LoadResource<SeatType>( virtualFileName );
				if( seatType != null )
				{
					var seat = ComponentUtility.CreateComponent<Seat>( null, false, true );
					seat.SeatType = seatType;

					var generator = new Product_Store.ImageGenerator();
					generator.CameraZoomFactor = 0.42;

					var mesh = seatType.Mesh.Value;
					if( mesh != null )
						generator.CameraLookTo = new Vector3( mesh.Result.SpaceBounds.BoundingBox.GetSize().X * 0.15, 0, 0 );

					var entry = archive.CreateEntry( "_ProductLogo.png" );
					using( var entryStream = entry.Open() )
						generator.Generate( seat, entryStream );//, ImageFormat.Png );
				}

				handled = true;
			}
		}
#endif
	}
}
