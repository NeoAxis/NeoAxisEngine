// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	public class OptionsWindow : UIWindow
	{
		public UITabControl GetTabControl() { return GetComponent<UITabControl>( "Tab Control" ); }
		public UIButton GetButtonClose() { return GetComponent<UIButton>( "Button Close" ); }

		//General page controls

		public UIControl GetPageGeneral() { return Components[ @"Tab Control\Page General\Control" ] as UIControl; }
		public UISlider GetSliderSoundVolume() { return GetPageGeneral()?.Components[ "Slider Sound Volume" ] as UISlider; }
		public UIText GetTextSoundVolume() { return GetPageGeneral()?.Components[ "Text Sound Volume" ] as UIText; }
		public UISlider GetSliderMusicVolume() { return GetPageGeneral()?.Components[ "Slider Music Volume" ] as UISlider; }
		public UIText GetTextMusicVolume() { return GetPageGeneral()?.Components[ "Text Music Volume" ] as UIText; }
		public UISlider GetSliderMouseSensitivity() { return GetPageGeneral()?.Components[ "Slider Mouse Sensitivity" ] as UISlider; }
		public UIText GetTextMouseSensitivity() { return GetPageGeneral()?.Components[ "Text Mouse Sensitivity" ] as UIText; }
		public UICheck GetCheckDisplayFrameInfo() { return GetPageGeneral()?.Components[ "Check Display Frame Info" ] as UICheck; }
		public UICheck GetCheckDisplaySceneInfo() { return GetPageGeneral()?.Components[ "Check Display Scene Info" ] as UICheck; }
		public UICheck GetCheckDisplayEngineInfo() { return GetPageGeneral()?.Components[ "Check Display Engine Info" ] as UICheck; }
		public UICheck GetCheckDisplayBackgroundScene() { return GetPageGeneral()?.Components[ "Check Display Background Scene" ] as UICheck; }
		public UIText GetTextKeys() { return GetPageGeneral()?.Components[ "Text Keys" ] as UIText; }

		//Graphics page controls

		public UIControl GetPageGraphics() { return Components[ @"Tab Control\Page Graphics\Control" ] as UIControl; }

		public UISlider GetSliderBrightness() { return GetPageGraphics()?.Components[ "Slider Brightness" ] as UISlider; }
		public UIText GetTextBrightness() { return GetPageGraphics()?.Components[ "Text Brightness" ] as UIText; }

		public UISlider GetSliderExposure() { return GetPageGraphics()?.Components[ "Slider Exposure" ] as UISlider; }
		public UIText GetTextExposure() { return GetPageGraphics()?.Components[ "Text Exposure" ] as UIText; }

		public UICheck GetCheckVerticalSync() { return GetPageGraphics()?.Components[ "Check Vertical Sync" ] as UICheck; }

		public UICombo GetComboWindowedMode() { return GetPageGraphics()?.Components[ "Combo Windowed Mode" ] as UICombo; }
		public UICombo GetComboVideoMode() { return GetPageGraphics()?.Components[ "Combo Video Mode" ] as UICombo; }

		public UICombo GetComboAntialiasingBasic() { return GetPageGraphics()?.Components[ "Combo Antialiasing Basic" ] as UICombo; }
		public UICombo GetComboAntialiasingAdditional() { return GetPageGraphics()?.Components[ "Combo Antialiasing Additional" ] as UICombo; }
		public UICombo GetComboAntialiasingMotion() { return GetPageGraphics()?.Components[ "Combo Antialiasing Motion" ] as UICombo; }
		public UICombo GetComboResolutionUpscaleMode() { return GetPageGraphics()?.Components[ "Combo Resolution Upscale Mode" ] as UICombo; }
		public UICombo GetComboResolutionUpscaleTechnique() { return GetPageGraphics()?.Components[ "Combo Resolution Upscale Technique" ] as UICombo; }
		public UISlider GetSliderSharpness() { return GetPageGraphics()?.Components[ "Slider Sharpness" ] as UISlider; }
		public UIText GetTextSharpness() { return GetPageGraphics()?.Components[ "Text Sharpness" ] as UIText; }

		public UIControl GetTextRestartToApplyChanges() { return GetPageGraphics()?.Components[ "Restart To Apply Changes" ] as UIControl; }

		//Graphics 2 page controls

		public UIControl GetPageGraphics2() { return Components[ @"Tab Control\Page Graphics 2\Control" ] as UIControl; }

		public UISlider GetSliderLOD() { return GetPageGraphics2()?.Components[ "Slider LOD" ] as UISlider; }
		public UIText GetTextLOD() { return GetPageGraphics2()?.Components[ "Text LOD" ] as UIText; }

		public UISlider GetSliderLODShadows() { return GetPageGraphics2()?.Components[ "Slider LOD Shadows" ] as UISlider; }
		public UIText GetTextLODShadows() { return GetPageGraphics2()?.Components[ "Text LOD Shadows" ] as UIText; }

		public UISlider GetSliderTexture() { return GetPageGraphics2()?.Components[ "Slider Texture" ] as UISlider; }
		public UIText GetTextTexture() { return GetPageGraphics2()?.Components[ "Text Texture" ] as UIText; }

		public UISlider GetSliderShadow() { return GetPageGraphics2()?.Components[ "Slider Shadow" ] as UISlider; }
		public UIText GetTextShadow() { return GetPageGraphics2()?.Components[ "Text Shadow" ] as UIText; }

		public UISlider GetSliderMotionBlur() { return GetPageGraphics2()?.Components[ "Slider Motion Blur" ] as UISlider; }
		public UIText GetTextMotionBlur() { return GetPageGraphics2()?.Components[ "Text Motion Blur" ] as UIText; }

		public UISlider GetSliderIndirectLighting() { return GetPageGraphics2()?.Components[ "Slider Indirect Lighting" ] as UISlider; }
		public UIText GetTextIndirectLighting() { return GetPageGraphics2()?.Components[ "Text Indirect Lighting" ] as UIText; }

		public UISlider GetSliderAO() { return GetPageGraphics2()?.Components[ "Slider AO" ] as UISlider; }
		public UIText GetTextAO() { return GetPageGraphics2()?.Components[ "Text AO" ] as UIText; }

		public UISlider GetSliderReflection() { return GetPageGraphics2()?.Components[ "Slider Reflection" ] as UISlider; }
		public UIText GetTextReflection() { return GetPageGraphics2()?.Components[ "Text Reflection" ] as UIText; }

		public UISlider GetSliderReflectionScreenSpace() { return GetPageGraphics2()?.Components[ "Slider Reflection Screen Space" ] as UISlider; }
		public UIText GetTextReflectionScreenSpace() { return GetPageGraphics2()?.Components[ "Text Reflection Screen Space" ] as UIText; }

		public UISlider GetSliderDOF() { return GetPageGraphics2()?.Components[ "Slider DOF" ] as UISlider; }
		public UIText GetTextDOF() { return GetPageGraphics2()?.Components[ "Text DOF" ] as UIText; }

		public UISlider GetSliderBloom() { return GetPageGraphics2()?.Components[ "Slider Bloom" ] as UISlider; }
		public UIText GetTextBloom() { return GetPageGraphics2()?.Components[ "Text Bloom" ] as UIText; }

		public UISlider GetSliderMicroparticlesInAir() { return GetPageGraphics2()?.Components[ "Slider Microparticles In Air" ] as UISlider; }
		public UIText GetTextMicroparticlesInAir() { return GetPageGraphics2()?.Components[ "Text Microparticles In Air" ] as UIText; }

		///////////////////////////////////////////////

		public delegate void EnabledInSimulationStaticDelegate( OptionsWindow sender );
		/// <summary>
		/// Static event may be used to change the options window without changing the code.
		/// </summary>
		public static event EnabledInSimulationStaticDelegate EnabledInSimulationStatic;

		///////////////////////////////////////////////

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			EnabledInSimulationStatic?.Invoke( this );

			if( GetTabControl() != null )
				GetTabControl().SelectedIndex = 0;

			if( GetButtonClose() != null )
			{
				GetButtonClose().Click += delegate ( UIButton sender )
				{
					Dispose();
				};
			}

			//Souund volume
			var sliderSound = GetSliderSoundVolume();
			if( sliderSound != null )
			{
				sliderSound.Value = SimulationApp.SoundVolume;
				sliderSound.ValueChanged += delegate ( UISlider obj )
				{
					SimulationApp.SoundVolume = obj.Value;
				};
			}

			//Music volume
			var sliderMusic = GetSliderMusicVolume();
			if( sliderMusic != null )
			{
				sliderMusic.Value = SimulationApp.MusicVolume;
				sliderMusic.ValueChanged += delegate ( UISlider obj )
				{
					SimulationApp.MusicVolume = obj.Value;
				};
			}

			//Mouse sensitivity
			var sliderMouseSensitivity = GetSliderMouseSensitivity();
			if( sliderMouseSensitivity != null )
			{
				sliderMouseSensitivity.Value = SimulationApp.MouseSensitivity;
				sliderMouseSensitivity.ValueChanged += delegate ( UISlider obj )
				{
					SimulationApp.MouseSensitivity = obj.Value;
				};
			}

			//Display frame info
			var checkDisplayFrameInfo = GetCheckDisplayFrameInfo();
			if( checkDisplayFrameInfo != null )
			{
				checkDisplayFrameInfo.Checked = SimulationApp.DisplayFrameInfo ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkDisplayFrameInfo.CheckedChanged += delegate ( UICheck obj )
				{
					SimulationApp.DisplayFrameInfo = obj.Checked.Value == UICheck.CheckValue.Checked;
				};
			}

			//Display scene info
			var checkDisplaySceneInfo = GetCheckDisplaySceneInfo();
			if( checkDisplaySceneInfo != null )
			{
				checkDisplaySceneInfo.Checked = SimulationApp.DisplaySceneInfo ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkDisplaySceneInfo.CheckedChanged += delegate ( UICheck obj )
				{
					SimulationApp.DisplaySceneInfo = obj.Checked.Value == UICheck.CheckValue.Checked;
				};
			}

			//Display engine info
			var checkDisplayEngineInfo = GetCheckDisplayEngineInfo();
			if( checkDisplayEngineInfo != null )
			{
				checkDisplayEngineInfo.Checked = SimulationApp.DisplayEngineInfo ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkDisplayEngineInfo.CheckedChanged += delegate ( UICheck obj )
				{
					SimulationApp.DisplayEngineInfo = obj.Checked.Value == UICheck.CheckValue.Checked;
				};
			}

			//Antialiasing basic technique
			var comboAntialiasingBasic = GetComboAntialiasingBasic();
			if( comboAntialiasingBasic != null )
			{
				//remove SSAA on mobile
				if( SystemSettings.LimitedDevice )
				{
					for( int n = comboAntialiasingBasic.Items.Count - 1; n >= 0; n-- )
					{
						if( ( (string)comboAntialiasingBasic.Items[ n ].Value ).Contains( "SSAA" ) )
							comboAntialiasingBasic.RemoveItem( n );
					}
				}

				comboAntialiasingBasic.SelectItemByValue( SimulationApp.AntialiasingBasic );
				comboAntialiasingBasic.SelectedIndexChanged += delegate ( UICombo sender )
				{
					SimulationApp.AntialiasingBasic = (string)sender.SelectedItem.Value;
				};
			}

			//Antialiasing additional technique
			var comboAntialiasingAdditional = GetComboAntialiasingAdditional();
			if( comboAntialiasingAdditional != null )
			{
				if( SystemSettings.LimitedDevice )
					comboAntialiasingAdditional.ReadOnly = true;

				comboAntialiasingAdditional.SelectItemByValue( SimulationApp.AntialiasingAdditional );
				comboAntialiasingAdditional.SelectedIndexChanged += delegate ( UICombo sender )
				{
					SimulationApp.AntialiasingAdditional = (string)sender.SelectedItem.Value;
				};
			}

			//Antialiasing motion technique
			var comboAntialiasingMotion = GetComboAntialiasingMotion();
			if( comboAntialiasingMotion != null )
			{
				if( SystemSettings.LimitedDevice )
				{
					comboAntialiasingMotion.ReadOnly = true;
					comboAntialiasingMotion.RemoveItem( 2 );
				}

				comboAntialiasingMotion.SelectItemByValue( SimulationApp.AntialiasingMotion );
				comboAntialiasingMotion.SelectedIndexChanged += delegate ( UICombo sender )
				{
					SimulationApp.AntialiasingMotion = (string)sender.SelectedItem.Value;
				};
			}

			//Resolution upscale mode
			var comboResolutionUpscaleMode = GetComboResolutionUpscaleMode();
			if( comboResolutionUpscaleMode != null )
			{
				comboResolutionUpscaleMode.SelectItemByValue( SimulationApp.ResolutionUpscaleMode );
				comboResolutionUpscaleMode.SelectedIndexChanged += delegate ( UICombo sender )
				{
					SimulationApp.ResolutionUpscaleMode = (string)sender.SelectedItem.Value;
				};
			}

			//Resolution upscale technique
			var comboResolutionUpscaleTechnique = GetComboResolutionUpscaleTechnique();
			if( comboResolutionUpscaleTechnique != null )
			{
				comboResolutionUpscaleTechnique.SelectItemByValue( SimulationApp.ResolutionUpscaleTechnique );
				comboResolutionUpscaleTechnique.SelectedIndexChanged += delegate ( UICombo sender )
				{
					SimulationApp.ResolutionUpscaleTechnique = (string)sender.SelectedItem.Value;
				};

				//FSR is not supported on mobile
				if( SystemSettings.LimitedDevice )
					comboResolutionUpscaleTechnique.RemoveItem( 3 );
			}

			//Windowed mode
			var comboWindowedMode = GetComboWindowedMode();
			if( comboWindowedMode != null )
			{
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
				{
					//Web specific for windowed mode

					//remove Borderless mode
					comboWindowedMode.RemoveItem( 1 );

					//set current value
					comboWindowedMode.SelectedIndex = SimulationApp.WindowedMode == WindowedModeEnum.Windowed ? 1 : 0;

					//change value
					comboWindowedMode.SelectedIndexChanged += delegate ( UICombo sender )
					{
						SimulationApp.WindowedMode = sender.SelectedIndex == 1 ? WindowedModeEnum.Windowed : WindowedModeEnum.Fullscreen;
						EngineApp.SetWindowedMode( SimulationApp.WindowedMode, EngineApp.WindowedModeSize );
					};
					comboWindowedMode.ReadOnly = SystemSettings.MobileDevice;
				}
				else
				{
					//Common for windowed mode

					//set current value
					comboWindowedMode.SelectedIndex = (int)SimulationApp.WindowedMode;

					//change value
					comboWindowedMode.SelectedIndexChanged += delegate ( UICombo sender )
					{
						SimulationApp.WindowedMode = (WindowedModeEnum)sender.SelectedIndex;

						MessageBoxWindow.Show( this, "Change the windowed mode right now?", "Confirm", EMessageBoxButtons.YesNo, EMessageBoxIcon.Question, null, delegate ( MessageBoxWindow sender2, EDialogResult result, object anyData )
						{
							if( result == EDialogResult.Yes )
								EngineApp.SetWindowedMode( SimulationApp.WindowedMode, EngineApp.WindowedModeSize );
							else
								ShowTextRestartToApplyChanges();
						} );

						//ShowTextRestartToApplyChanges();
					};
					comboWindowedMode.ReadOnly = SystemSettings.MobileDevice;
				}
			}

			//Video mode
			var comboVideoMode = GetComboVideoMode();
			if( comboVideoMode != null )
			{
				foreach( var mode in SystemSettings.VideoModes )
				{
					comboVideoMode.AddItem( $"{mode.X}x{mode.Y}" );
					if( mode == SimulationApp.VideoMode )
						comboVideoMode.SelectedIndex = comboVideoMode.Items.Count - 1;
				}
				comboVideoMode.SelectedIndexChanged += delegate ( UICombo sender )
				{
					if( sender.SelectedIndex > 0 )
					{
						var s = (string)sender.SelectedItem.Value;
						var array = s.Split( new char[] { 'x' }, StringSplitOptions.RemoveEmptyEntries );
						SimulationApp.VideoMode = new Vector2I( int.Parse( array[ 0 ].Trim() ), int.Parse( array[ 1 ].Trim() ) );
					}
					else
						SimulationApp.VideoMode = Vector2I.Zero;

					MessageBoxWindow.Show( this, "Change the windowed mode right now?", "Confirm", EMessageBoxButtons.YesNo, EMessageBoxIcon.Question, null, delegate ( MessageBoxWindow sender2, EDialogResult result, object anyData )
					{
						if( result == EDialogResult.Yes )
						{
							var fullscreenSize = SimulationApp.VideoMode;
							if( fullscreenSize == Vector2I.Zero )
								fullscreenSize = EngineApp.GetScreenSize();
							EngineApp.SetWindowedMode( EngineApp.WindowedMode, fullscreenSize );
						}
						else
							ShowTextRestartToApplyChanges();
					} );

					//ShowTextRestartToApplyChanges();
				};
				comboVideoMode.ReadOnly = SystemSettings.MobileDevice || SystemSettings.CurrentPlatform == SystemSettings.Platform.Web;
			}

			//Vertical sync
			var checkVerticalSync = GetCheckVerticalSync();
			if( checkVerticalSync != null )
			{
				checkVerticalSync.Checked = SimulationApp.VerticalSync ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkVerticalSync.CheckedChanged += delegate ( UICheck obj )
				{
					SimulationApp.VerticalSync = obj.Checked.Value == UICheck.CheckValue.Checked;
					ShowTextRestartToApplyChanges();
				};
				checkVerticalSync.ReadOnly = SystemSettings.MobileDevice || SystemSettings.CurrentPlatform == SystemSettings.Platform.Web;
			}

			//Restart to apply changes text
			var textRestartToApplyChanges = GetTextRestartToApplyChanges();
			if( textRestartToApplyChanges != null )
				textRestartToApplyChanges.Visible = false;

			//Display background scene
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
			{
				var slider = GetSliderSharpness();
				if( slider != null )
				{
					slider.Value = SimulationApp.Sharpness;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.Sharpness = obj.Value;
					};
				}
			}

			//Brightness
			{
				var slider = GetSliderBrightness();
				if( slider != null )
				{
					slider.Value = SimulationApp.Brightness;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.Brightness = obj.Value;
					};
				}
			}

			//Exposure
			{
				var slider = GetSliderExposure();
				if( slider != null )
				{
					slider.Value = SimulationApp.Exposure;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.Exposure = obj.Value;
					};
				}
			}

			//LOD
			{
				var slider = GetSliderLOD();
				if( slider != null )
				{
					slider.Value = SimulationApp.LODScale;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.LODScale = obj.Value;
					};
				}
			}

			//LOD shadows
			{
				var slider = GetSliderLODShadows();
				if( slider != null )
				{
					slider.Value = SimulationApp.LODScaleShadows;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.LODScaleShadows = obj.Value;
					};
				}
			}

			//Texture quality
			{
				var slider = GetSliderTexture();
				if( slider != null )
				{
					slider.Value = SimulationApp.TextureQuality;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.TextureQuality = obj.Value;
					};
				}
			}

			//Shadow quality
			{
				var slider = GetSliderShadow();
				if( slider != null )
				{
					slider.Value = SimulationApp.ShadowQuality;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.ShadowQuality = obj.Value;
					};
				}
			}

			//Indirect lighting
			{
				var slider = GetSliderIndirectLighting();
				if( slider != null )
				{
					slider.Value = SimulationApp.IndirectLightingMultiplier;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.IndirectLightingMultiplier = obj.Value;
					};
				}
			}

			//Ambient occlusion
			{
				var slider = GetSliderAO();
				if( slider != null )
				{
					slider.Value = SimulationApp.AmbientOcclusionMultiplier;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.AmbientOcclusionMultiplier = obj.Value;
					};
				}
			}

			//Reflections
			{
				var slider = GetSliderReflection();
				if( slider != null )
				{
					slider.Value = SimulationApp.ReflectionMultiplier;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.ReflectionMultiplier = obj.Value;
					};
				}
			}

			//Screen space reflections
			{
				var slider = GetSliderReflectionScreenSpace();
				if( slider != null )
				{
					slider.Value = SimulationApp.ReflectionScreenSpaceMultiplier;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.ReflectionScreenSpaceMultiplier = obj.Value;
					};
				}
			}

			//Motion blur
			{
				var slider = GetSliderMotionBlur();
				if( slider != null )
				{
					slider.Value = SimulationApp.MotionBlurMultiplier;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.MotionBlurMultiplier = obj.Value;
					};
				}
			}

			//Depth of field
			{
				var slider = GetSliderDOF();
				if( slider != null )
				{
					slider.Value = SimulationApp.DepthOfFieldBlurFactor;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.DepthOfFieldBlurFactor = obj.Value;
					};
				}
			}

			//Bloom
			{
				var slider = GetSliderBloom();
				if( slider != null )
				{
					slider.Value = SimulationApp.BloomScale;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.BloomScale = obj.Value;
					};
				}
			}

			//Microparticles in air
			{
				var slider = GetSliderMicroparticlesInAir();
				if( slider != null )
				{
					slider.Value = SimulationApp.MicroparticlesInAirMultiplier;
					slider.ValueChanged += delegate ( UISlider obj )
					{
						SimulationApp.MicroparticlesInAirMultiplier = obj.Value;
					};
				}
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

		void ShowTextRestartToApplyChanges()
		{
			var textRestartToApplyChanges = GetTextRestartToApplyChanges();
			if( textRestartToApplyChanges != null )
				textRestartToApplyChanges.Visible = true;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( GetTextSoundVolume() != null )
				GetTextSoundVolume().Text = SimulationApp.SoundVolume.ToString( "F1" );

			if( GetTextMusicVolume() != null )
				GetTextMusicVolume().Text = SimulationApp.MusicVolume.ToString( "F1" );

			if( GetTextMouseSensitivity() != null )
				GetTextMouseSensitivity().Text = SimulationApp.MouseSensitivity.ToString( "F1" );

			if( GetTextSharpness() != null )
				GetTextSharpness().Text = SimulationApp.Sharpness.ToString( "F1" );

			if( GetTextBrightness() != null )
				GetTextBrightness().Text = SimulationApp.Brightness.ToString( "F1" );

			if( GetTextExposure() != null )
				GetTextExposure().Text = SimulationApp.Exposure.ToString( "F1" );

			if( GetTextLOD() != null )
				GetTextLOD().Text = SimulationApp.LODScale.ToString( "F1" );

			if( GetTextLODShadows() != null )
				GetTextLODShadows().Text = SimulationApp.LODScaleShadows.ToString( "F1" );

			if( GetTextTexture() != null )
				GetTextTexture().Text = SimulationApp.TextureQuality.ToString( "F1" );

			if( GetTextShadow() != null )
				GetTextShadow().Text = SimulationApp.ShadowQuality.ToString( "F1" );

			if( GetTextIndirectLighting() != null )
				GetTextIndirectLighting().Text = SimulationApp.IndirectLightingMultiplier.ToString( "F1" );

			if( GetTextAO() != null )
				GetTextAO().Text = SimulationApp.AmbientOcclusionMultiplier.ToString( "F1" );

			if( GetTextReflection() != null )
				GetTextReflection().Text = SimulationApp.ReflectionMultiplier.ToString( "F1" );

			if( GetTextReflectionScreenSpace() != null )
				GetTextReflectionScreenSpace().Text = SimulationApp.ReflectionScreenSpaceMultiplier.ToString( "F1" );

			if( GetTextMotionBlur() != null )
				GetTextMotionBlur().Text = SimulationApp.MotionBlurMultiplier.ToString( "F1" );

			if( GetTextDOF() != null )
				GetTextDOF().Text = SimulationApp.DepthOfFieldBlurFactor.ToString( "F1" );

			if( GetTextBloom() != null )
				GetTextBloom().Text = SimulationApp.BloomScale.ToString( "F1" );

			if( GetTextMicroparticlesInAir() != null )
				GetTextMicroparticlesInAir().Text = SimulationApp.MicroparticlesInAirMultiplier.ToString( "F1" );

			var comboWindowedMode = GetComboWindowedMode();
			var comboVideoMode = GetComboVideoMode();
			if( comboWindowedMode != null && comboVideoMode != null )
				comboVideoMode.ReadOnly = (WindowedModeEnum)comboWindowedMode.SelectedIndex == WindowedModeEnum.Borderless || SystemSettings.MobileDevice || SystemSettings.CurrentPlatform == SystemSettings.Platform.Web;
		}
	}
}