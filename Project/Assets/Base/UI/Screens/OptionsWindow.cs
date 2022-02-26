// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	public class OptionsWindow : UIWindow
	{
		protected override void OnEnabledInSimulation()
		{
			if( Components[ "Button Close" ] != null )
			{
				( (UIButton)Components[ "Button Close" ] ).Click += delegate ( UIButton sender )
				{
					Dispose();
				};
			}

			var sliderSound = GetSliderSoundVolume();
			if( sliderSound != null )
			{
				sliderSound.Value = SimulationApp.SoundVolume;
				sliderSound.ValueChanged += delegate ( UISlider obj )
				{
					SimulationApp.SoundVolume = obj.Value;
				};
			}

			var sliderMusic = GetSliderMusicVolume();
			if( sliderMusic != null )
			{
				sliderMusic.Value = SimulationApp.MusicVolume;
				sliderMusic.ValueChanged += delegate ( UISlider obj )
				{
					SimulationApp.MusicVolume = obj.Value;
				};
			}

			var checkStatistics = GetCheckDisplayViewportStatistics();
			if( checkStatistics != null )
			{
				checkStatistics.Checked = SimulationApp.DisplayViewportStatistics ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkStatistics.CheckedChanged += delegate ( UICheck obj )
				{
					SimulationApp.DisplayViewportStatistics = obj.Checked.Value == UICheck.CheckValue.Checked;
				};
			}

			var listAntialiasingBasic = GetListAntialiasingBasic();
			if( listAntialiasingBasic != null )
			{
				listAntialiasingBasic.SelectItem( SimulationApp.AntialiasingBasic );
				listAntialiasingBasic.SelectedIndexChanged += delegate ( UIList sender )
				{
					SimulationApp.AntialiasingBasic = sender.SelectedItem;
				};
			}

			var listAntialiasingMotion = GetListAntialiasingMotion();
			if( listAntialiasingMotion != null )
			{
				listAntialiasingMotion.SelectItem( SimulationApp.AntialiasingMotion );
				listAntialiasingMotion.SelectedIndexChanged += delegate ( UIList sender )
				{
					SimulationApp.AntialiasingMotion = sender.SelectedItem;
				};

				if( SystemSettings.LimitedDevice )
				{
					listAntialiasingMotion.ReadOnly = true;
					listAntialiasingMotion.Items.RemoveAt( 2 );
				}
			}

			var listResolutionUpscaleMode = GetListResolutionUpscaleMode();
			if( listResolutionUpscaleMode != null )
			{
				listResolutionUpscaleMode.SelectItem( SimulationApp.ResolutionUpscaleMode );
				listResolutionUpscaleMode.SelectedIndexChanged += delegate ( UIList sender )
				{
					SimulationApp.ResolutionUpscaleMode = sender.SelectedItem;
				};
			}

			var listResolutionUpscaleTechnique = GetListResolutionUpscaleTechnique();
			if( listResolutionUpscaleTechnique != null )
			{
				listResolutionUpscaleTechnique.SelectItem( SimulationApp.ResolutionUpscaleTechnique );
				listResolutionUpscaleTechnique.SelectedIndexChanged += delegate ( UIList sender )
				{
					SimulationApp.ResolutionUpscaleTechnique = sender.SelectedItem;
				};

				//FSR is not supported on mobile
				if( SystemSettings.LimitedDevice )
					listResolutionUpscaleTechnique.Items.RemoveAt( 3 );
			}

			var listVideoMode = GetListVideoMode();
			if( listVideoMode != null )
			{
				foreach( var mode in SystemSettings.VideoModes )
				{
					listVideoMode.Items.Add( $"{mode.X}x{mode.Y}" );
					if( mode == SimulationApp.VideoMode )
						listVideoMode.SelectedIndex = listVideoMode.Items.Count - 1;
				}
				listVideoMode.SelectedIndexChanged += delegate ( UIList sender )
				{
					if( sender.SelectedIndex > 0 )
					{
						var s = sender.SelectedItem;
						var array = s.Split( new char[] { 'x' }, StringSplitOptions.RemoveEmptyEntries );
						SimulationApp.VideoMode = new Vector2I( int.Parse( array[ 0 ].Trim() ), int.Parse( array[ 1 ].Trim() ) );
					}
					else
						SimulationApp.VideoMode = Vector2I.Zero;
					ShowTextRestartToApplyChanges();
				};
				listVideoMode.ReadOnly = SystemSettings.MobileDevice;
			}

			var checkFullscreen = GetCheckFullscreen();
			if( checkFullscreen != null )
			{
				checkFullscreen.Checked = SimulationApp.Fullscreen ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkFullscreen.CheckedChanged += delegate ( UICheck obj )
				{
					SimulationApp.Fullscreen = obj.Checked.Value == UICheck.CheckValue.Checked;
					ShowTextRestartToApplyChanges();
				};
				checkFullscreen.ReadOnly = SystemSettings.MobileDevice;
			}

			var checkVerticalSync = GetCheckVerticalSync();
			if( checkVerticalSync != null )
			{
				checkVerticalSync.Checked = SimulationApp.VerticalSync ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkVerticalSync.CheckedChanged += delegate ( UICheck obj )
				{
					SimulationApp.VerticalSync = obj.Checked.Value == UICheck.CheckValue.Checked;
					ShowTextRestartToApplyChanges();
				};
				checkVerticalSync.ReadOnly = SystemSettings.MobileDevice;
			}

			var textRestartToApplyChanges = GetTextRestartToApplyChanges();
			if( textRestartToApplyChanges != null )
				textRestartToApplyChanges.Visible = false;

			var checkDisplayBackgroundScene = GetCheckDisplayBackgroundScene();
			if( checkDisplayBackgroundScene != null )
			{
				checkDisplayBackgroundScene.Checked = SimulationApp.DisplayBackgroundScene ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkDisplayBackgroundScene.CheckedChanged += delegate ( UICheck obj )
				{
					SimulationApp.DisplayBackgroundScene = obj.Checked.Value == UICheck.CheckValue.Checked;
				};
			}


			//Sharpness

			var checkSharpnessDefault = GetCheckSharpnessDefault();
			if( checkSharpnessDefault != null )
			{
				checkSharpnessDefault.Checked = SimulationApp.Sharpness >= 0 ? UICheck.CheckValue.Unchecked : UICheck.CheckValue.Checked;
				checkSharpnessDefault.CheckedChanged += delegate ( UICheck obj )
				{
					var sliderSharpness2 = GetSliderSharpness();
					if( sliderSharpness2 != null )
						sliderSharpness2.Value = obj.Checked.Value == UICheck.CheckValue.Checked ? -1.0 : 1.0;
				};
			}

			var sliderSharpness = GetSliderSharpness();
			if( sliderSharpness != null )
			{
				sliderSharpness.Value = SimulationApp.Sharpness;
				sliderSharpness.ValueChanged += delegate ( UISlider obj )
				{
					SimulationApp.Sharpness = obj.Value;
				};
			}
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( e.Key == EKeys.Escape )
			{
				Dispose();
				return true;
			}

			return base.OnKeyDown( e );
		}

		//General page controls

		UISlider GetSliderSoundVolume()
		{
			return Components[ @"Tab Control\Page General\Control\Slider Sound Volume" ] as UISlider;
		}

		UISlider GetSliderMusicVolume()
		{
			return Components[ @"Tab Control\Page General\Control\Slider Music Volume" ] as UISlider;
		}

		UIList GetListVideoMode()
		{
			return Components[ @"Tab Control\Page General\Control\List Video Mode" ] as UIList;
		}

		UICheck GetCheckDisplayViewportStatistics()
		{
			return Components[ @"Tab Control\Page General\Control\Check Display Viewport Statistics" ] as UICheck;
		}

		UICheck GetCheckFullscreen()
		{
			return Components[ @"Tab Control\Page General\Control\Check Fullscreen" ] as UICheck;
		}

		UICheck GetCheckVerticalSync()
		{
			return Components[ @"Tab Control\Page General\Control\Check Vertical Sync" ] as UICheck;
		}

		UICheck GetCheckDisplayBackgroundScene()
		{
			return Components[ @"Tab Control\Page General\Control\Check Display Background Scene" ] as UICheck;
		}

		UIControl GetTextRestartToApplyChanges()
		{
			return Components[ @"Tab Control\Page General\Control\Restart To Apply Changes" ] as UIControl;
		}

		void ShowTextRestartToApplyChanges()
		{
			var textRestartToApplyChanges = GetTextRestartToApplyChanges();
			if( textRestartToApplyChanges != null )
				textRestartToApplyChanges.Visible = true;
		}

		//Graphics page controls

		UIList GetListAntialiasingBasic()
		{
			return Components[ @"Tab Control\Page Graphics\Control\List Antialiasing Basic" ] as UIList;
		}

		UIList GetListAntialiasingMotion()
		{
			return Components[ @"Tab Control\Page Graphics\Control\List Antialiasing Motion" ] as UIList;
		}

		UIList GetListResolutionUpscaleMode()
		{
			return Components[ @"Tab Control\Page Graphics\Control\List Resolution Upscale Mode" ] as UIList;
		}

		UIList GetListResolutionUpscaleTechnique()
		{
			return Components[ @"Tab Control\Page Graphics\Control\List Resolution Upscale Technique" ] as UIList;
		}

		UICheck GetCheckSharpnessDefault()
		{
			return Components[ @"Tab Control\Page Graphics\Control\Check Sharpness Default" ] as UICheck;
		}

		UISlider GetSliderSharpness()
		{
			return Components[ @"Tab Control\Page Graphics\Control\Slider Sharpness" ] as UISlider;
		}

		UIText GetTextSharpness()
		{
			return Components[ @"Tab Control\Page Graphics\Control\Text Sharpness" ] as UIText;
		}


		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			var sliderSharpness = GetSliderSharpness();
			if( sliderSharpness != null )
				sliderSharpness.ReadOnly = SimulationApp.Sharpness < 0;

			var textSharpness = GetTextSharpness();
			if( textSharpness != null )
				textSharpness.Text = SimulationApp.Sharpness < 0 ? "" : SimulationApp.Sharpness.ToString( "F1" );
		}
	}
}