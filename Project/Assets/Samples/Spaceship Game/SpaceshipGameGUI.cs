using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.IO;
using NeoAxis;

namespace Project
{
	//!!!!base on BasicSceneScreen?

	public class SpaceshipGameGUI : NeoAxis.UIControl
	{
		double lastSpeedingUp;
		double lastTurning;

		//

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			//hide GUI controls on PC
			if( !SystemSettings.MobileDevice )
			{
				var controlNames = new string[] { "Left", "Right", "Up", "Down" };

				foreach( var controlName in controlNames )
				{
					var control = GetComponent( controlName ) as UIControl;
					if( control != null )
						control.Enabled = false;
				}
			}
		}

		bool IsAnyWindowOpened()
		{
			return ParentRoot.GetComponent<UIWindow>( true, true ) != null;
		}

		bool IsPointInsideControl( string controlName, Vector2 screenPosition )
		{
			var control = GetComponent( controlName ) as UIControl;
			if( control != null )
				return control.GetScreenRectangle().Contains( screenPosition );
			return false;
		}

		bool IsControlTouched( InputProcessing inputProcessing, string controlName )
		{
			foreach( var pointer in inputProcessing.TouchPointers )
			{
				if( IsPointInsideControl( controlName, pointer.Position ) )
					return true;
			}
			return false;
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			var ship = Scene.First.GetComponent( "Spaceship" );
			var inputProcessing = ship?.GetComponent<InputProcessing>();
			if( inputProcessing != null )
			{
				lastSpeedingUp = 0;
				lastTurning = 0;

				if( !IsAnyWindowOpened() )
				{
					//forward
					if( inputProcessing.IsKeyPressed( EKeys.W ) || inputProcessing.IsKeyPressed( EKeys.Up ) )
						lastSpeedingUp += 1.0;
					//backward
					if( inputProcessing.IsKeyPressed( EKeys.S ) || inputProcessing.IsKeyPressed( EKeys.Down ) )
						lastSpeedingUp -= 1.0;
					//left
					if( inputProcessing.IsKeyPressed( EKeys.A ) || inputProcessing.IsKeyPressed( EKeys.Left ) )
						lastTurning += 1.0;
					//turn right
					if( inputProcessing.IsKeyPressed( EKeys.D ) || inputProcessing.IsKeyPressed( EKeys.Right ) )
						lastTurning -= 1.0;

					//movement by joystick axes
					if( Math.Abs( inputProcessing.JoystickAxes[ 0 ] ) >= 0.01 )
						lastTurning -= inputProcessing.JoystickAxes[ 0 ];
					if( Math.Abs( inputProcessing.JoystickAxes[ 1 ] ) >= 0.01 )
						lastSpeedingUp += inputProcessing.JoystickAxes[ 1 ];

					//touch
					if( IsControlTouched( inputProcessing, "Left" ) )
						lastTurning += 1.0;
					if( IsControlTouched( inputProcessing, "Right" ) )
						lastTurning -= 1.0;
					if( IsControlTouched( inputProcessing, "Up" ) )
						lastSpeedingUp += 1.0;
					if( IsControlTouched( inputProcessing, "Down" ) )
						lastSpeedingUp -= 1.0;
				}

				//update the ship
				var body = ship.GetComponent<RigidBody2D>();
				if( body != null )
				{
					//forward, backward
					if( lastSpeedingUp != 0 )
					{
						var dir = body.TransformV.Rotation.GetForward().ToVector2();
						body.ApplyForce( dir * lastSpeedingUp * 1.0 );
					}

					//turn left, right
					if( lastTurning != 0 )
						body.ApplyTorque( lastTurning * 1.0 );
				}
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
					var to = center + new Vector2( rectangle.Size.X / 2 * -lastTurning, rectangle.Size.Y / 2 * -lastSpeedingUp );

					renderer.AddLine( center, to, new ColorValue( 1, 1, 0 ) );

					var r = new Rectangle( to );
					r.Expand( new Vector2( renderer.AspectRatioInv * 0.005, 0.005 ) );
					renderer.AddFillEllipse( r, 32, new ColorValue( 1, 1, 0 ) );
				}
			}
		}

		//old. the example of getting fields from C# script
		//bool GetShipControl( out double speedingUp, out double turning )
		//{
		//	//get fields from the script
		//	var script = Scene.First.GetComponentByPath( @"Spaceship\Input Processing\C# Script" ) as CSharpScript;
		//	if( script != null )
		//	{
		//		speedingUp = script.GetCompiledPropertyValue<double>( "LastSpeedingUp" );
		//		turning = script.GetCompiledPropertyValue<double>( "LastTurning" );
		//		return true;
		//	}

		//	speedingUp = 0;
		//	turning = 0;
		//	return false;
		//}

	}
}