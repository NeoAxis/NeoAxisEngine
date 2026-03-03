// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// An object to make demo scenes.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Demo Mode", -3000 )]
	public class DemoMode : Component
	{
		/// <summary>
		/// The type of object to control by the player.
		/// </summary>
		[DefaultValueReference( "NeoAxis.Character" )]
		[NetworkSynchronize( false )]
		public Reference<Metadata.TypeInfo> ObjectTypeControlledByPlayer
		{
			get { if( _objectTypeControlledByPlayer.BeginGet() ) ObjectTypeControlledByPlayer = _objectTypeControlledByPlayer.Get( this ); return _objectTypeControlledByPlayer.value; }
			set { if( _objectTypeControlledByPlayer.BeginSet( this, ref value ) ) { try { ObjectTypeControlledByPlayerChanged?.Invoke( this ); } finally { _objectTypeControlledByPlayer.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectTypeControlledByPlayer"/> property value changes.</summary>
		public event Action<DemoMode> ObjectTypeControlledByPlayerChanged;
		ReferenceField<Metadata.TypeInfo> _objectTypeControlledByPlayer = new Reference<Metadata.TypeInfo>( null, "NeoAxis.Character" );

		/////////////////////////////////////////

		//enum WalkMode
		//{
		//	None,
		//	FirstPerson,
		//	ThirdPerson,
		//}

		bool showKeys = true;
		//WalkMode walkMode = WalkMode.None;
		Viewport.CameraSettingsClass lastCameraSettings;

		/////////////////////////////////////////

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy && EngineApp.IsSimulation )
			{
				var scene = FindParent<Scene>();
				if( scene != null )
				{
					var gameMode = scene.GetComponent<GameMode>();
					if( gameMode != null )
					{
						gameMode.InputMessageEvent += GameMode_InputMessageEvent;
						gameMode.GetCameraSettingsEvent += GameMode_GetCameraSettingsEvent;
						gameMode.RenderUI += GameMode_RenderUI;

						//reset camera
						gameMode.UseBuiltInCamera = GameMode.BuiltInCameraEnum.None;
					}
				}
			}
		}

		private void GameMode_InputMessageEvent( GameMode sender, InputMessage message )
		{
			var keyDown = message as InputMessageKeyDown;
			if( keyDown != null )
			{
				if( keyDown.Key == EKeys.K )
				{
					showKeys = !showKeys;
					message.Handled = true;
					return;
				}

				//if( keyDown.Key == EKeys.F )
				//{
				//	if( walkMode != WalkMode.FirstPerson )
				//		walkMode = WalkMode.FirstPerson;
				//	else
				//		walkMode = WalkMode.None;
				//	UpdateWalkMode();
				//	message.Handled = true;
				//	return;
				//}

				//if( keyDown.Key == EKeys.H )
				//{
				//	if( walkMode != WalkMode.ThirdPerson )
				//		walkMode = WalkMode.ThirdPerson;
				//	else
				//		walkMode = WalkMode.None;
				//	UpdateWalkMode();
				//	message.Handled = true;
				//	return;
				//}
			}
		}

		private void GameMode_GetCameraSettingsEvent( GameMode sender, Viewport viewport, Camera cameraDefault, ref Viewport.CameraSettingsClass cameraSettings )
		{
			lastCameraSettings = cameraSettings;
			if( cameraSettings == null && cameraDefault != null )
				lastCameraSettings = new Viewport.CameraSettingsClass( viewport, cameraDefault );
		}

		public delegate void ShowKeysEventDelegate( DemoMode sender, List<string> lines );
		public event ShowKeysEventDelegate ShowKeysEvent;

		private void GameMode_RenderUI( GameMode sender, CanvasRenderer renderer )
		{
			if( showKeys )
			{
				var lines = new List<string>();

				lines.Add( "F6 - first or third person camera" );
				lines.Add( "F7 - free camera" );
				lines.Add( "W A S D Q E Shift - camera control" );
				lines.Add( "K - show keys" );

				//lines.Add( "K - show keys" );
				////lines.Add( "" );
				////lines.Add( "F6 - first or third person camera" );
				//lines.Add( "F - walk first person" );
				//lines.Add( "H - walk third person" );
				////lines.Add( "" );
				//lines.Add( "F7 - free camera" );
				//lines.Add( "W A S D Q E - camera control" );
				////lines.Add( "" );
				////lines.Add( "Esc - menu" );

				ShowKeysEvent?.Invoke( this, lines );

				CanvasRendererUtility.AddTextLinesWithShadow( renderer.ViewportForScreenCanvasRenderer, renderer.DefaultFont, renderer.DefaultFontSize, lines, new Rectangle( 0.1, 0, 1, 0.95 ), EHorizontalAlignment.Left, EVerticalAlignment.Bottom, new ColorValue( 1, 1, 1 ) );
				//renderer.AddTextLines( lines, new Vector2( 0.1, 0.95 ), EHorizontalAlignment.Left, EVerticalAlignment.Bottom, 0, new ColorValue( 1, 1, 1 ) );
			}
		}

		public void UpdateWalkMode()
		{
			var scene = FindParent<Scene>();
			if( scene != null )
			{
				var gameMode = scene.GetComponent<GameMode>();
				if( gameMode != null )
				{
					//create or destroy a character
					if( lastCameraSettings != null )
					{
						var position = lastCameraSettings.Position;
						var direction = lastCameraSettings.Direction;

						//if( walkMode != WalkMode.None )
						if( gameMode.UseBuiltInCamera.Value != GameMode.BuiltInCameraEnum.None )
						{
							var obj = gameMode.ObjectControlledByPlayer.Value;
							if( obj == null )
							{
								var objectType = ObjectTypeControlledByPlayer.Value;
								if( objectType == null )
									objectType = MetadataManager.GetTypeOfNetType( typeof( Character ) );

								obj = scene.CreateComponent( objectType, enabled: false, setUniqueName: true );
								obj.NewObjectSetDefaultConfiguration();
								obj.Enabled = true;

								if( obj is Character character2 )
								{
									//Character specific
									character2.SetTransformAndTurnToDirectionInstantly( new Transform( position, Quaternion.FromDirectionZAxisUp( direction ) ) );
								}
								//else if( obj is Vehicle vehicle )
								//{
								//	//Vehicle specific
								//	vehicle.SetTransform( transform );
								//}
								//else if( obj is ObjectInSpace objectInSpace )
								//{
								//	//ObjectInSpace specific
								//	objectInSpace.SetPosition( transform.Position );
								//	objectInSpace.SetRotation( transform.Rotation );
								//}

								gameMode.ObjectControlledByPlayer = obj;
							}
						}
						else
						{
							var obj = gameMode.ObjectControlledByPlayer.Value;
							if( obj != null )
							{
								gameMode.ObjectControlledByPlayer = null;
								obj.RemoveFromParent( true );
							}
						}
					}

					////update camera type
					//switch( walkMode )
					//{
					//case WalkMode.None:
					//	gameMode.UseBuiltInCamera = GameMode.BuiltInCameraEnum.None;
					//	break;
					//case WalkMode.FirstPerson:
					//	gameMode.UseBuiltInCamera = GameMode.BuiltInCameraEnum.FirstPerson;
					//	break;
					//case WalkMode.ThirdPerson:
					//	gameMode.UseBuiltInCamera = GameMode.BuiltInCameraEnum.ThirdPerson;
					//	break;
					//}

					//var viewport = GameMode.PlayScreen?.ParentContainer?.Viewport;
					//viewport?.KeysAndMouseButtonUpAll();
					//viewport?.NotifyInstantCameraMovement();
				}
			}
		}
	}
}
