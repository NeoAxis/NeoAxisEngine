// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.Fbx;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

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

				Product_Store.CreateScreenshots += Product_Store_CreateScreenshots;

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

		private void Product_Store_CreateScreenshots( Product_Store sender, string[] files, ZipArchive archive, bool additional, ref int imageCounter, ref bool handled )
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
					if( !additional )
					{
						//logo

						var vehicle = ComponentUtility.CreateComponent<Vehicle>( null, false, true );
						vehicle.VehicleType = vehicleType;

						var generator = new Product_Store.ImageGenerator();
						generator.CameraZoomFactor = 0.42;

						var mesh = vehicleType.Mesh.Value;
						if( mesh != null )
							generator.CameraLookTo = new Vector3( mesh.Result.SpaceBounds.BoundingBox.GetSize().X * 0.15, 0, 0 );


						//!!!!
						//generator.BeforeSceneEnable += delegate ( Product_Store.ImageGenerator sender )
						//{
						//	var scene = sender.Scene;

						//	//sky
						//	{
						//		var sky = scene.CreateComponent<Sky>();
						//		sky.Name = "Sky";

						//		sky.Cubemap = ReferenceUtility.MakeReference( "Content\\Environments\\Basic Library\\Outdoor\\Textures\\kloofendal_48d_partly_cloudy_4k.hdr" );
						//		sky.CubemapMultiplier = new ColorValuePowered( 1, 1, 1, 1, 2 );
						//	}

						//};


						var entry = archive.CreateEntry( "_ProductLogo.png" );
						using( var entryStream = entry.Open() )
							generator.Generate( vehicle, entryStream );//, ImageFormat.Png );
					}
					else
					{
						//additional images

						var cameraDistanceFactor = 1.0;
						{
							var mesh = vehicleType.Mesh.Value;
							if( mesh?.Result != null )
							{
								var b = mesh.Result.SpaceBounds.BoundingBox;
								var side = b.GetSize().MaxComponent();
								if( side > 5 )
									cameraDistanceFactor = side / 5.0;
							}
						}

						for( int nImage = 0; nImage < 5; nImage++ )
						{
							//in the scene
							if( nImage <= 3 )
							{
								var generator = new Product_Store.ImageGenerator();
								generator.SceneName = "Base\\Tools\\Store\\PreviewVehicle.scene";
								generator.CameraName = "Camera " + ( nImage + 1 ).ToString();
								generator.ImageSizeRender = ( generator.ImageSizeRender.ToVector2() * 1.5 ).ToVector2I();
								generator.ImageSizeOutput = ( generator.ImageSizeOutput.ToVector2() * 1.5 ).ToVector2I();

								generator.BeforeSceneEnable += delegate ( Product_Store.ImageGenerator sender )
								{
									var scene = sender.Scene;
									var vehicle = scene.GetComponent<Vehicle>( "Vehicle" );

									vehicle.VehicleType = vehicleType;

									var p = vehicle.TransformV.Position;
									var height = vehicleType.CalculateHeightToBottomOfWheels();

									if( vehicleType.Chassis.Value == VehicleType.ChassisEnum.Tracks )
									{
										var m = vehicleType.TrackFragmentMesh.Value;
										if( m?.Result != null )
										{
											// / 2 is good?
											height -= m.Result.SpaceBounds.BoundingBox.GetSize().Z / 2;
										}
									}

									if( height == 0 )
									{
										//no wheels
										var mesh = vehicleType.Mesh.Value;
										if( mesh?.Result != null )
										{
											var b = mesh.Result.SpaceBounds.BoundingBox;
											height = b.Minimum.Z;
										}
									}

									vehicle.Transform = new Transform( new Vector3( p.X, p.Y, -height ) );
								};

								generator.AfterSceneEnable += delegate ( Product_Store.ImageGenerator sender )
								{
									var scene = sender.Scene;
									var vehicle = scene.GetComponent<Vehicle>( "Vehicle" );

									//do it after scene enable to use lights data from dynamic data

									//headlights, brakes
									{
										vehicle.Brake = 1;
										vehicle.HandBrake = 1;

										var lights = vehicle?.DynamicData.Lights;
										if( lights != null )
										{
											var existsHeadlightsLow = lights.FirstOrDefault( l => l.LightTypePurpose == Vehicle.DynamicDataClass.LightTypePurpose.Headlight_Low ) != null;
											var existsHeadlightsHigh = lights.FirstOrDefault( l => l.LightTypePurpose == Vehicle.DynamicDataClass.LightTypePurpose.Headlight_High ) != null;

											if( nImage == 0 )
											{
												if( existsHeadlightsLow )
													vehicle.HeadlightsLow = 1;
												else if( existsHeadlightsHigh )
													vehicle.HeadlightsHigh = 1;
											}
											else
											{
												if( existsHeadlightsHigh )
													vehicle.HeadlightsHigh = 1;
												else if( existsHeadlightsLow )
													vehicle.HeadlightsLow = 1;
											}

											if( nImage == 3 )
											{
												vehicle.LeftTurnSignal = 1;
												vehicle.RightTurnSignal = 1;
											}
										}

										var dummy = false;
										vehicle.SimulateVisualHeadlights( ref dummy, true );
										vehicle.SimulateVisualBrake( ref dummy, true );
										vehicle.SimulateVisualTurnSignals( ref dummy, true );
										vehicle.UpdateLightComponents();
									}

									//add driver
									{
										var gameMode = (GameMode)scene.GetGameMode();

										//do
										{
											//find a free seat
											var seatIndex = vehicle.GetFreeSeat();
											if( seatIndex != -1 )
											{
												var character = scene.CreateComponent<Character>( enabled: false );
												//character.CharacterType = playerCharacter.CharacterType;
												character.Enabled = true;

												//put to the seat
												vehicle.PutObjectToSeat( gameMode, seatIndex, character );
											}
											//else
											//	break;
										}
										//while( true );
									}

									//update camera
									if( cameraDistanceFactor != 1.0 )
									{
										var camera = scene.GetComponent<Camera>( generator.CameraName );

										var tr = camera.TransformV;
										var distance = ( vehicle.TransformV.Position - tr.Position ).Length();

										var newDistance = distance * cameraDistanceFactor;

										var heightOffset = ( newDistance - distance ) * 0.2;

										camera.Transform = new Transform( tr.Position - ( newDistance - distance ) * tr.Rotation.GetForward() + new Vector3( 0, 0, heightOffset ), tr.Rotation );
									}
								};

								{
									var entry = archive.CreateEntry( $"_Image{imageCounter}.png" );
									using( var entryStream = entry.Open() )
										generator.Generate( null/*vehicle*/, entryStream );
									imageCounter++;
								}
							}

							//debug data
							if( nImage == 4 )
							{
								var vehicleTypeClone = (VehicleType)vehicleType.Clone();
								vehicleTypeClone.EditorDisplayWheels = true;
								vehicleTypeClone.EditorDisplaySeats = true;
								vehicleTypeClone.EditorDisplayPhysics = true;

								var vehicle = ComponentUtility.CreateComponent<Vehicle>( null, false, true );

								vehicle.VehicleType = vehicleTypeClone;
								vehicle.DebugVisualization = true;

								var generator = new Product_Store.ImageGenerator();
								generator.CameraZoomFactor = 0.42;
								generator.ImageSizeRender = ( generator.ImageSizeRender.ToVector2() * 1.5 ).ToVector2I();
								generator.ImageSizeOutput = ( generator.ImageSizeOutput.ToVector2() * 1.5 ).ToVector2I();
								generator.ImageSizeRender /= 2;

								var mesh = vehicleTypeClone.Mesh.Value;
								if( mesh != null )
									generator.CameraLookTo = new Vector3( 0, 0, 0 );

								generator.CameraDirection = new Vector3( 0, 0.8, -0.4 ).GetNormalize();

								generator.BeforeSceneEnable += delegate ( Product_Store.ImageGenerator sender )
								{
									generator.Scene.DisplayPhysicalObjects = true;
								};

								{
									var entry = archive.CreateEntry( $"_Image{imageCounter}.png" );
									using( var entryStream = entry.Open() )
										generator.Generate( vehicle, entryStream );
									imageCounter++;
								}
							}
						}
					}
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
