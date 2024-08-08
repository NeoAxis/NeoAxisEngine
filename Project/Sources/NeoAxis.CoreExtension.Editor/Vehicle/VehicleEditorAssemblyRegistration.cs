// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace NeoAxis.Editor
{
	/// <summary>
	/// The class is intended to register vehicle resources.
	/// </summary>
	public class VehicleEditorAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			if( EngineApp.IsEditor )
			{
				SceneEditorUtility.CreateObjectWhatTypeWillCreatedEvent += Scene_DocumentWindow_CreateObjectWhatTypeWillCreatedEvent;
				SceneEditorUtility.CreateObjectByCreationDataEvent += Scene_DocumentWindow_CreateObjectByCreationDataEvent;

				EditorAPI.PreviewImagesManager_RegisterResourceType( "Vehicle Type" );

				Product_Store.CreateScreenshot += Product_Store_CreateScreenshot;

				RegisterEditorActions();
				RegisterVehicleEditorRibbonTab();
			}
		}

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

		static void RegisterEditorActions()
		{
			//Vehicle Display Wheels
			{
				var a = new EditorAction();
				a.Name = "Vehicle Display Wheels";
				a.Description = "Whether to display the debug visualization of the wheels.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.ArrangeUp_32;//Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Wheels", "");
				a.GetState += delegate ( EditorActionGetStateContext context )
				{
					var vehicleType = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as VehicleType;
					if( vehicleType != null )
					{
						context.Enabled = true;
						context.Checked = vehicleType.EditorDisplayWheels;
					}
				};
				a.Click += delegate ( EditorActionClickContext context )
				{
					var vehicleType = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as VehicleType;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = vehicleType.EditorDisplayWheels;

					vehicleType.EditorDisplayWheels = !vehicleType.EditorDisplayWheels;

					var property = (Metadata.Property)vehicleType.MetadataGetMemberBySignature( "property:EditorDisplayWheels" );
					var undoItem = new UndoActionPropertiesChange.Item( vehicleType, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Vehicle Display Seats
			{
				var a = new EditorAction();
				a.Name = "Vehicle Display Seats";
				a.Description = "Whether to display the debug visualization of the seats.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.ArrangeUp_32;//Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Seats", "");
				a.GetState += delegate ( EditorActionGetStateContext context )
				{
					var vehicleType = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as VehicleType;
					if( vehicleType != null )
					{
						context.Enabled = true;
						context.Checked = vehicleType.EditorDisplaySeats;
					}
				};
				a.Click += delegate ( EditorActionClickContext context )
				{
					var vehicleType = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as VehicleType;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = vehicleType.EditorDisplaySeats;

					vehicleType.EditorDisplaySeats = !vehicleType.EditorDisplaySeats;

					var property = (Metadata.Property)vehicleType.MetadataGetMemberBySignature( "property:EditorDisplaySeats" );
					var undoItem = new UndoActionPropertiesChange.Item( vehicleType, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Vehicle Display Lights
			{
				var a = new EditorAction();
				a.Name = "Vehicle Display Lights";
				a.Description = "Whether to display the debug visualization of the lights.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.ArrangeUp_32;//Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Lights", "");
				a.GetState += delegate ( EditorActionGetStateContext context )
				{
					var vehicleType = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as VehicleType;
					if( vehicleType != null )
					{
						context.Enabled = true;
						context.Checked = vehicleType.EditorDisplayLights;
					}
				};
				a.Click += delegate ( EditorActionClickContext context )
				{
					var vehicleType = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as VehicleType;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = vehicleType.EditorDisplayLights;

					vehicleType.EditorDisplayLights = !vehicleType.EditorDisplayLights;

					var property = (Metadata.Property)vehicleType.MetadataGetMemberBySignature( "property:EditorDisplayLights" );
					var undoItem = new UndoActionPropertiesChange.Item( vehicleType, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}

			//Vehicle Display Physics
			{
				var a = new EditorAction();
				a.Name = "Vehicle Display Physics";
				a.Description = "Whether to display the debug visualization of the physics.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.ArrangeUp_32;//Default_32;
				a.QatSupport = true;
				a.RibbonText = ("Physics", "");
				a.GetState += delegate ( EditorActionGetStateContext context )
				{
					var vehicleType = context.ObjectsInFocus.DocumentWindow?.ObjectOfWindow as VehicleType;
					if( vehicleType != null )
					{
						context.Enabled = true;
						context.Checked = vehicleType.EditorDisplayPhysics;
					}
				};
				a.Click += delegate ( EditorActionClickContext context )
				{
					var vehicleType = context.ObjectsInFocus.DocumentWindow.ObjectOfWindow as VehicleType;
					var document = context.ObjectsInFocus.DocumentWindow.Document;

					var oldValue = vehicleType.EditorDisplayPhysics;

					vehicleType.EditorDisplayPhysics = !vehicleType.EditorDisplayPhysics;

					var property = (Metadata.Property)vehicleType.MetadataGetMemberBySignature( "property:EditorDisplayPhysics" );
					var undoItem = new UndoActionPropertiesChange.Item( vehicleType, property, oldValue, new object[ 0 ] );
					var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );
					document.UndoSystem.CommitAction( undoAction );
					document.Modified = true;
				};
				EditorActions.Register( a );
			}
		}

		static void RegisterVehicleEditorRibbonTab()
		{
			var tab = new EditorRibbonDefaultConfiguration.Tab( "Vehicle Editor", "ComponentTypeSpecific", MetadataManager.GetTypeOfNetType( typeof( VehicleType ) ) );
			EditorRibbonDefaultConfiguration.AddTab( tab );

			//Display
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Display" );
				tab.Groups2.Add( group );

				group.AddAction( "Vehicle Display Wheels" );
				group.AddAction( "Vehicle Display Seats" );
				group.AddAction( "Vehicle Display Lights" );
				group.AddAction( "Vehicle Display Physics" );
			}
		}
	}
}
