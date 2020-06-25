using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	public class SplashScreen : UIControl
	{
		const bool enablePoweredByTime = false;

		///////////////////////////////////////////

		bool gotoMainMenu;
		bool gotoMainMenuUpdated;

		///////////////////////////////////////////

		[Serialize]
		[DefaultValue( enablePoweredByTime ? 2.0 : 0.0 )]
		public double PoweredByTime { get; set; } = enablePoweredByTime ? 2.0 : 0.0;

		[Serialize]
		[DefaultValue( 1.0 )]
		public double ProjectTime { get; set; } = 1.0;

		[Serialize]
		[DefaultValue( 0.5 )]
		public double FadingTime { get; set; } = 0.5;

		///////////////////////////////////////////

		protected override void OnEnabledInSimulation()
		{
			PreloadImage();
			ResetCreateTime();
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( base.OnKeyDown( e ) )
				return true;

			if( e.Key == EKeys.Escape || e.Key == EKeys.Return || e.Key == EKeys.Space )
			{
				gotoMainMenu = true;
				return true;
			}

			return false;
		}

		protected override bool OnMouseDown( EMouseButtons button )
		{
			if( base.OnMouseDown( button ) )
				return true;

			if( button == EMouseButtons.Left || button == EMouseButtons.Right )
			{
				gotoMainMenu = true;
				return true;
			}

			return false;
		}

		void PreloadImage()
		{
			var image = Components[ "PoweredBy" ] as UIImage;
			if( image != null )
			{
				var v = image.SourceImage;
			}
			image = Components[ "Project" ] as UIImage;
			if( image != null )
			{
				var v = image.SourceImage;
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
			{
				if( gotoMainMenuUpdated )
				{
					// Restore cursor visibility.
					EngineApp.ShowCursor = true;

					SimulationApp.ChangeUIScreen( @"Base\UI\Screens\MainMenuScreen.ui" );
				}

				if( Time > GetTotalTime() )
					gotoMainMenu = true;
			}
		}

		double GetTotalTime()
		{
			return 1.0 + FadingTime + PoweredByTime + FadingTime + 1.0 + FadingTime + ProjectTime + FadingTime;
		}

		void GetImagesTransparency( out double poweredBy, out double project )
		{
			var curve = new CurveLine();
			curve.AddPoint( 0, new Vector3( 0, 0, 0 ) );
			curve.AddPoint( 1.0, new Vector3( 0, 0, 0 ) );
			curve.AddPoint( 1.0 + FadingTime, new Vector3( 1, 0, 0 ) );
			curve.AddPoint( 1.0 + FadingTime + PoweredByTime, new Vector3( 1, 0, 0 ) );
			curve.AddPoint( 1.0 + FadingTime + PoweredByTime + FadingTime, new Vector3( 0, 0, 0 ) );
			curve.AddPoint( 1.0 + FadingTime + PoweredByTime + FadingTime + 1.0, new Vector3( 0, 0, 0 ) );
			curve.AddPoint( 1.0 + FadingTime + PoweredByTime + FadingTime + 1.0 + FadingTime, new Vector3( 0, 1, 0 ) );
			curve.AddPoint( 1.0 + FadingTime + PoweredByTime + FadingTime + 1.0 + FadingTime + ProjectTime, new Vector3( 0, 1, 0 ) );
			curve.AddPoint( 1.0 + FadingTime + PoweredByTime + FadingTime + 1.0 + FadingTime + ProjectTime + FadingTime, new Vector3( 0, 0, 0 ) );

			var value = curve.CalculateValueByTime( Time );
			poweredBy = MathEx.Saturate( value.X );
			project = MathEx.Saturate( value.Y );

			if( !enablePoweredByTime )
				poweredBy = 0;

			if( gotoMainMenu )
			{
				poweredBy = 0;
				project = 0;
			}
		}

		void UpdateImagesTransparency()
		{
			GetImagesTransparency( out var poweredBy, out var project );

			var image = Components[ "PoweredBy" ] as UIImage;
			if( image != null )
				image.ColorMultiplier = new ColorValue( 1, 1, 1, poweredBy );

			image = Components[ "Project" ] as UIImage;
			if( image != null )
				image.ColorMultiplier = new ColorValue( 1, 1, 1, project );
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
				UpdateImagesTransparency();

			base.OnRenderUI( renderer );

			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
			{
				if( gotoMainMenu )
					gotoMainMenuUpdated = true;

				// Hide cursor.
				EngineApp.ShowCursor = false;
			}
		}
	}
}