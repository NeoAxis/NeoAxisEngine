// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	public class SplashScreen : UIControl
	{
		Component_ProjectSettings.EngineSplashScreenStyleEnum drawSplashScreen = Component_ProjectSettings.EngineSplashScreenStyleEnum.Disabled;

		int resetTimeCounter = 3;

		///////////////////////////////////////////

		bool gotoMainMenu;
		bool gotoMainMenuUpdated;

		///////////////////////////////////////////

		[Browsable( false )]
		public double PoweredByTime
		{
			get { return drawSplashScreen != Component_ProjectSettings.EngineSplashScreenStyleEnum.Disabled ? 2.0 : 0.0; }
		}

		[Serialize]
		[DefaultValue( 2.0 )]
		public double ProjectTime { get; set; } = 2.0;

		[Serialize]
		[DefaultValue( 0.5 )]
		public double FadingTime { get; set; } = 0.5;

		///////////////////////////////////////////

		protected override void OnEnabledInSimulation()
		{
			var originalDrawSplashScreen = ProjectSettings.Get.EngineSplashScreenStyle.Value;

			//get drawing engine splash settings. engine splash for Windows is rendered from another place
			if( SystemSettings.CurrentPlatform != SystemSettings.Platform.Windows )
				drawSplashScreen = originalDrawSplashScreen;

			//update background color
			if( originalDrawSplashScreen == Component_ProjectSettings.EngineSplashScreenStyleEnum.WhiteBackground )
				BackgroundColor = new ColorValue( 1, 1, 1 );
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

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
			{
				if( gotoMainMenuUpdated )
				{
					//restore cursor visibility
					EngineApp.ShowCursor = true;

					SimulationApp.ChangeUIScreen( @"Base\UI\Screens\MainMenuScreen.ui" );
				}

				if( resetTimeCounter == 0 && Time > GetTotalTime() )
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

			var value = curve.CalculateValueByTime( resetTimeCounter != 0 ? 0 : Time );
			poweredBy = MathEx.Saturate( value.X );
			project = MathEx.Saturate( value.Y );

			if( gotoMainMenu )
			{
				poweredBy = 0;
				project = 0;
			}
		}

		void UpdateImagesTransparency()
		{
			GetImagesTransparency( out var poweredBy, out var project );

			var image = Components[ "PoweredBy_BlackBackground" ] as UIImage;
			if( image != null )
			{
				image.Visible = drawSplashScreen == Component_ProjectSettings.EngineSplashScreenStyleEnum.BlackBackground;
				image.ColorMultiplier = new ColorValue( 1, 1, 1, poweredBy );
			}

			image = Components[ "PoweredBy_WhiteBackground" ] as UIImage;
			if( image != null )
			{
				image.Visible = drawSplashScreen == Component_ProjectSettings.EngineSplashScreenStyleEnum.WhiteBackground;
				image.ColorMultiplier = new ColorValue( 1, 1, 1, poweredBy );
			}

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

				//hide cursor
				EngineApp.ShowCursor = false;

				//reset time for waiting to load images
				if( resetTimeCounter > 0 )
				{
					resetTimeCounter--;
					ResetCreateTime();
				}
			}
		}
	}
}