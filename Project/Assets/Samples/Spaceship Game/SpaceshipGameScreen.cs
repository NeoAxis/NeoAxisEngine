// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.IO;
using NeoAxis;

namespace Project
{
	public class SpaceshipGameScreen : BasicSceneScreen
	{
		//alternatively can move this code to GameLogic based class

		readonly double shipRangeX = 3; //readonly double shipRangeX = 7.7;
		readonly NeoAxis.Range shipRangeY = new NeoAxis.Range( -4.4, 6.6 );
		readonly double planetCreatePositionY = 9;
		readonly double planetDeletePositionY = -7;

		List<Sprite> sourcePlanets = new List<Sprite>();
		Sprite sourceShip;

		double planetCreateTimer;
		FastRandom planetCreateRandom = new FastRandom();

		double gameTime;
		double speedMultiplier = 1;
		double newGameRemainingTime = 10;

		double up;
		double left;

		///////////////////////////////////////////////

		protected override void OnEnabledInSimulationAndIsInstance()
		{
			base.OnEnabledInSimulationAndIsInstance();

			//get sourcePlanets
			for( int n = 1; ; n++ )
			{
				var name = $"Planet {n}";

				var sprite = Scene.GetComponent<Sprite>( name );
				if( sprite != null )
				{
					var sourcePlanet = (Sprite)sprite.Clone();
					sourcePlanets.Add( sourcePlanet );

					sprite.RemoveFromParent( false );
				}
				else
					break;
			}

			//get sourceShip
			{
				var sprite = Scene.GetComponent<Sprite>( "Spaceship" );
				if( sprite != null )
					sourceShip = (Sprite)sprite.Clone();
			}
		}

		protected override void OnTouchControlsUpdate( float delta )
		{
			//override default implemetation
			var enable = SystemSettings.MobileDevice;// && !GameMode.FreeCamera && GameMode.UseBuiltInCamera.Value != GameMode.BuiltInCameraEnum.None;
			TouchControlsEnable( enable );
		}

		bool IsCollided( RigidBody2D shipBody )
		{
			var edge = shipBody.Physics2DBody.ContactList;
			while( edge != null )
			{
				if( edge.Contact.IsTouching )
					return true;
				edge = edge.Next;
			}
			return false;
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			var ship = Scene.GetComponent( "Spaceship" );
			var inputProcessing = ship?.GetComponent<InputProcessing>();
			if( inputProcessing != null )
			{
				up = 0;
				left = 0;

				if( !IsAnyWindowOpened() )
				{
					//forward
					if( inputProcessing.IsKeyPressed( EKeys.W ) || inputProcessing.IsKeyPressed( EKeys.Up ) )
						up += 1.0;
					//backward
					if( inputProcessing.IsKeyPressed( EKeys.S ) || inputProcessing.IsKeyPressed( EKeys.Down ) )
						up -= 1.0;
					//left
					if( inputProcessing.IsKeyPressed( EKeys.A ) || inputProcessing.IsKeyPressed( EKeys.Left ) )
						left += 1.0;
					//turn right
					if( inputProcessing.IsKeyPressed( EKeys.D ) || inputProcessing.IsKeyPressed( EKeys.Right ) )
						left -= 1.0;

					//movement by joystick axes
					if( Math.Abs( inputProcessing.JoystickAxes[ 0 ] ) >= 0.01 )
						left -= inputProcessing.JoystickAxes[ 0 ];
					if( Math.Abs( inputProcessing.JoystickAxes[ 1 ] ) >= 0.01 )
						up += inputProcessing.JoystickAxes[ 1 ];

					//touch
					if( IsControlTouched( inputProcessing, "Left" ) )
						left += 1.0;
					if( IsControlTouched( inputProcessing, "Right" ) )
						left -= 1.0;
					if( IsControlTouched( inputProcessing, "Up" ) )
						up += 1.0;
					if( IsControlTouched( inputProcessing, "Down" ) )
						up -= 1.0;

					up = Math.Clamp( up, -1.0, 1.0 );
					left = Math.Clamp( left, -1.0, 1.0 );
				}

				//update the ship
				var shipBody = ship.GetComponent<RigidBody2D>();
				if( shipBody != null )
				{
					//change controls when outside of the range
					{
						var tr = shipBody.TransformV;
						if( tr.Position.X < -shipRangeX )
							left = -0.5;
						if( tr.Position.X > shipRangeX )
							left = 0.5;
						if( tr.Position.Y < shipRangeY.Minimum )
							up = 1;
						if( tr.Position.Y > shipRangeY.Maximum )
							up = -1;
					}

					//forward, backward
					if( up != 0 )
					{
						var dir = shipBody.TransformV.Rotation.GetForward().ToVector2();
						shipBody.ApplyForce( dir * up * 4.0 );
						//shipBody.ApplyForce( dir * up * 3.0 );
					}

					//strife left, right
					if( left != 0 )
					{
						var dir = shipBody.TransformV.Rotation.GetLeft().ToVector2();
						shipBody.ApplyForce( dir * left * 4.0 );
						//shipBody.ApplyForce( dir * left * 3.0 );
					}

					////clamp position
					//var tr = shipBody.TransformV;
					//if( tr.Position.X < -shipRangeX )
					//	tr = tr.UpdatePosition( new Vector3( -shipRangeX, tr.Position.Y, tr.Position.Z ) );
					//if( tr.Position.X > shipRangeX )
					//	tr = tr.UpdatePosition( new Vector3( shipRangeX, tr.Position.Y, tr.Position.Z ) );
					//if( tr.Position.Y < shipRangeY.Minimum )
					//	tr = tr.UpdatePosition( new Vector3( tr.Position.X, shipRangeY.Minimum, tr.Position.Z ) );
					//if( tr.Position.Y > shipRangeY.Maximum )
					//	tr = tr.UpdatePosition( new Vector3( tr.Position.X, shipRangeY.Maximum, tr.Position.Z ) );
					//if( shipBody.TransformV.Position != tr.Position )
					//	shipBody.TransformV = tr;

					//detect collision with asteroids
					if( IsCollided( shipBody ) )
					{
						ship.RemoveFromParent( true );
						SoundPlay2D( @"Samples\Spaceship Game\Sounds\Blow.ogg" );
					}
				}
			}

			//create new planets
			{
				planetCreateTimer -= NeoAxis.Time.SimulationDelta;
				if( planetCreateTimer <= 0 )
				{
					planetCreateTimer = planetCreateRandom.Next( 1 / speedMultiplier, 2 / speedMultiplier );
					//planetCreateTimer = planetCreateRandom.Next( 0.1, 1 );
					var sourcePlanet = sourcePlanets[ planetCreateRandom.Next( 0, sourcePlanets.Count - 1 ) ];

					var planet = (Sprite)sourcePlanet.Clone();
					var body = planet.GetComponent<RigidBody2D>();
					body.SetPosition( new Vector3( planetCreateRandom.Next( -shipRangeX, shipRangeX ), planetCreatePositionY, sourcePlanet.TransformV.Position.Z ) );
					body.AngularVelocity = planetCreateRandom.Next( -100.0, 100.0 );
					body.LinearVelocity = new Vector2( body.LinearVelocity.Value.X, body.LinearVelocity.Value.Y * speedMultiplier );
					Scene.AddComponent( planet );
				}
			}

			//delete old planets
			{
				var planets = Scene.GetComponents<Sprite>();
				foreach( var planet in planets )
				{
					if( planet.TransformV.Position.Y < planetDeletePositionY )
						planet.RemoveFromParent( true );
				}
			}

			//increase game time
			if( ship != null )
			{
				gameTime += NeoAxis.Time.SimulationDelta;
				speedMultiplier *= 1.0 + NeoAxis.Time.SimulationDelta * 0.01;
			}
			else
			{
				newGameRemainingTime -= NeoAxis.Time.SimulationDelta;
				if( newGameRemainingTime < 0 )
				{
					newGameRemainingTime = 10;

					gameTime = 0;
					speedMultiplier = 1;

					var planets = Scene.GetComponents<Sprite>();
					foreach( var planet in planets )
						planet.RemoveFromParent( true );

					var shipNew = (Sprite)sourceShip.Clone();
					Scene.AddComponent( shipNew );
				}
			}
		}

		protected override void Scene_RenderEvent( Scene scene, Viewport viewport )
		{
			base.Scene_RenderEvent( scene, viewport );

			//draw range lines
			{
				var renderer = viewport.Simple3DRenderer;
				renderer.SetColor( new ColorValue( 1, 1, 0, .2 ) );
				var z = -1;
				renderer.AddLine( new Vector3( -shipRangeX, shipRangeY.Minimum, z ), new Vector3( -shipRangeX, shipRangeY.Maximum, z ), 0.05 );
				renderer.AddLine( new Vector3( shipRangeX, shipRangeY.Minimum, z ), new Vector3( shipRangeX, shipRangeY.Maximum, z ), 0.05 );
			}
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );

			//the example of drawing over control
			if( EngineApp.IsSimulation )
			{
				var imageControl = GetComponent<UIControl>( "Control Render Example" );
				if( imageControl != null )
				{
					var rectangle = imageControl.GetScreenRectangle();

					var center = rectangle.GetCenter();
					var to = center + new Vector2( rectangle.Size.X / 2 * -left, rectangle.Size.Y / 2 * -up );

					renderer.AddLine( center, to, new ColorValue( 1, 1, 0 ) );

					var r = new Rectangle( to );
					r.Expand( new Vector2( renderer.AspectRatioInv * 0.005, 0.005 ) );
					renderer.AddFillEllipse( r, 32, new ColorValue( 1, 1, 0 ) );
				}
			}

			//draw game time
			if( EngineApp.IsSimulation )
			{
				{
					var text = $"{gameTime.ToString( "0.0" )} - {speedMultiplier.ToString( "0.0" )}";
					renderer.AddText( text.ToString(), new Vector2( 0.5, 1.0 - renderer.DefaultFontSize / 2 ), EHorizontalAlignment.Center, EVerticalAlignment.Bottom );
				}

				if( newGameRemainingTime < 10 )
				{
					var secondsToStart = (int)newGameRemainingTime + 1;
					var text = $"{secondsToStart} seconds until new game";
					renderer.AddText( text.ToString(), new Vector2( 0.5, 1.0 - renderer.DefaultFontSize * 2 ), EHorizontalAlignment.Center, EVerticalAlignment.Bottom );
				}
			}
		}
	}
}