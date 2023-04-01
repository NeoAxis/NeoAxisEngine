// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

#if !DEPLOY
			if( EngineApp.IsEditor )
			{
				SceneEditor.CreateObjectWhatTypeWillCreatedEvent += Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent;
				SceneEditor.CreateObjectByCreationDataEvent += Scene_DocumentWindow_CreateObjectByCreationDataEvent;

				PreviewImagesManager.RegisterResourceType( "Vehicle Type" );

				Product_Store.CreateScreenshot += Product_Store_CreateScreenshot;
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

		private void Product_Store_CreateScreenshot( Product_Store sender, string[] files, ZipArchive archive, ref bool handled )
		{
			var virtualFileName = "";

			foreach( var file in files )
			{
				var ext = Path.GetExtension( file );
				if( !string.IsNullOrEmpty( ext ) )
				{
					var ext2 = ext.ToLower().Replace( ".", "" );
					if( ext2 == "vehicletype" )
					{
						virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
						if( !string.IsNullOrEmpty( virtualFileName ) )
							break;
					}
				}
			}

			if( !string.IsNullOrEmpty( virtualFileName ) )
			{
				var vehicleType = ResourceManager.LoadResource<VehicleType>( virtualFileName );
				if( vehicleType != null )
				{
					var vehicle = ComponentUtility.CreateComponent<Vehicle>( null, false, true );
					vehicle.VehicleType = vehicleType;

					var generator = new Product_Store.ImageGenerator();
					generator.CameraZoomFactor = 0.42;

					var mesh = vehicleType.Mesh.Value;
					if( mesh != null )
						generator.CameraLookTo = new Vector3( mesh.Result.SpaceBounds.BoundingBox.GetSize().X * 0.15, 0, 0 );

					var entry = archive.CreateEntry( "_ProductLogo.png" );
					using( var entryStream = entry.Open() )
						generator.Generate( vehicle, entryStream );//, ImageFormat.Png );
				}

				handled = true;
			}
		}
#endif
	}
}
