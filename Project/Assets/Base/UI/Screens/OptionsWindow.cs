using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	public class OptionsWindow : UIWindow
	{
		protected override void OnEnabledInSimulation()
		{
			if( Components[ "Button Close" ] != null )
				( (UIButton)Components[ "Button Close" ] ).Click += ButtonClose_Click;

			var sliderSound = GetSliderSoundVolume();
			if( sliderSound != null )
			{
				sliderSound.Value = SimulationApp.SoundVolume;
				sliderSound.ValueChanged += SliderSound_ValueChanged;
			}

			var sliderMusic = GetSliderMusicVolume();
			if( sliderMusic != null )
			{
				sliderMusic.Value = SimulationApp.MusicVolume;
				sliderMusic.ValueChanged += SliderMusic_ValueChanged;
			}

			var checkStatistics = GetCheckDisplayViewportStatistics();
			if( checkStatistics != null )
			{
				checkStatistics.Checked = SimulationApp.DisplayViewportStatistics ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkStatistics.CheckedChanged += CheckStatistics_CheckedChanged;
			}

			var listAntialiasing = GetListAntialiasing();
			if( listAntialiasing != null )
			{
				listAntialiasing.SelectItem( SimulationApp.Antialiasing );
				listAntialiasing.SelectedIndexChanged += ListAntialiasing_SelectedIndexChanged;
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
				listVideoMode.SelectedIndexChanged += ListVideoMode_SelectedIndexChanged;
			}

			var checkFullscreen = GetCheckFullscreen();
			if( checkFullscreen != null )
			{
				checkFullscreen.Checked = SimulationApp.Fullscreen ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkFullscreen.CheckedChanged += CheckFullscreen_CheckedChanged;
			}

			var checkVerticalSync = GetCheckVerticalSync();
			if( checkVerticalSync != null )
			{
				checkVerticalSync.Checked = SimulationApp.VerticalSync ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkVerticalSync.CheckedChanged += CheckVerticalSync_CheckedChanged;
			}

			var textRestartToApplyChanges = GetTextRestartToApplyChanges();
			if( textRestartToApplyChanges != null )
				textRestartToApplyChanges.Visible = false;
				
			var checkDisplayBackgroundScene = GetCheckDisplayBackgroundScene();
			if( checkDisplayBackgroundScene != null )
			{
				checkDisplayBackgroundScene.Checked = SimulationApp.DisplayBackgroundScene ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkDisplayBackgroundScene.CheckedChanged += CheckDisplayBackgroundScene_CheckedChanged;
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

		private void SliderSound_ValueChanged( UISlider obj )
		{
			SimulationApp.SoundVolume = obj.Value;
		}

		private void SliderMusic_ValueChanged( UISlider obj )
		{
			SimulationApp.MusicVolume = obj.Value;
		}

		private void CheckStatistics_CheckedChanged( UICheck obj )
		{
			SimulationApp.DisplayViewportStatistics = obj.Checked.Value == UICheck.CheckValue.Checked;
		}

		private void ListAntialiasing_SelectedIndexChanged( UIList sender )
		{
			SimulationApp.Antialiasing = sender.SelectedItem;
		}

		private void ListVideoMode_SelectedIndexChanged( UIList sender )
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
		}

		private void CheckFullscreen_CheckedChanged( UICheck obj )
		{
			SimulationApp.Fullscreen = obj.Checked.Value == UICheck.CheckValue.Checked;
			ShowTextRestartToApplyChanges();
		}

		private void CheckVerticalSync_CheckedChanged( UICheck obj )
		{
			SimulationApp.VerticalSync = obj.Checked.Value == UICheck.CheckValue.Checked;
			ShowTextRestartToApplyChanges();
		}
		
		private void CheckDisplayBackgroundScene_CheckedChanged( UICheck obj )
		{
			SimulationApp.DisplayBackgroundScene = obj.Checked.Value == UICheck.CheckValue.Checked;
		}

		void ButtonClose_Click( UIButton sender )
		{
			Dispose();
		}

		UISlider GetSliderSoundVolume()
		{
			return Components[ "Slider Sound Volume" ] as UISlider;
		}

		UISlider GetSliderMusicVolume()
		{
			return Components[ "Slider Music Volume" ] as UISlider;
		}

		UICheck GetCheckDisplayViewportStatistics()
		{
			return Components[ "Check Display Viewport Statistics" ] as UICheck;
		}

		UIList GetListAntialiasing()
		{
			return Components[ "List Antialiasing" ] as UIList;
		}

		UIList GetListVideoMode()
		{
			return Components[ "List Video Mode" ] as UIList;
		}

		UICheck GetCheckFullscreen()
		{
			return Components[ "Check Fullscreen" ] as UICheck;
		}

		UICheck GetCheckVerticalSync()
		{
			return Components[ "Check Vertical Sync" ] as UICheck;
		}

		UIControl GetTextRestartToApplyChanges()
		{
			return Components[ "Restart To Apply Changes" ] as UIControl;
		}

		void ShowTextRestartToApplyChanges()
		{
			var textRestartToApplyChanges = GetTextRestartToApplyChanges();
			if( textRestartToApplyChanges != null )
				textRestartToApplyChanges.Visible = true;
		}
		
		UICheck GetCheckDisplayBackgroundScene()
		{
			return Components[ "Check Display Background Scene" ] as UICheck;
		}
	}
}