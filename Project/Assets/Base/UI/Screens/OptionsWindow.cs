// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	public class OptionsWindow : UIWindow
	{
		UITabControl GetTabControl() { return GetComponent<UITabControl>( "Tab Control" ); }
		UIButton GetButtonClose() { return GetComponent<UIButton>( "Button Close" ); }

		//General page controls
		UIControl GetPageGeneral() { return Components[ @"Tab Control\Page General\Control" ] as UIControl; }
		UISlider GetSliderSoundVolume() { return GetPageGeneral()?.Components[ "Slider Sound Volume" ] as UISlider; }
		UISlider GetSliderMusicVolume() { return GetPageGeneral()?.Components[ "Slider Music Volume" ] as UISlider; }
		UIList GetListVideoMode() { return GetPageGeneral()?.Components[ "List Video Mode" ] as UIList; }
		UICheck GetCheckDisplayFrameInfo() { return GetPageGeneral()?.Components[ "Check Display Frame Info" ] as UICheck; }
		UICheck GetCheckDisplaySceneInfo() { return GetPageGeneral()?.Components[ "Check Display Scene Info" ] as UICheck; }
		UICheck GetCheckDisplayEngineInfo() { return GetPageGeneral()?.Components[ "Check Display Engine Info" ] as UICheck; }
		UICheck GetCheckFullscreen() { return GetPageGeneral()?.Components[ "Check Fullscreen" ] as UICheck; }
		UICheck GetCheckVerticalSync() { return GetPageGeneral()?.Components[ "Check Vertical Sync" ] as UICheck; }
		UICheck GetCheckDisplayBackgroundScene() { return GetPageGeneral()?.Components[ "Check Display Background Scene" ] as UICheck; }
		UIControl GetTextRestartToApplyChanges() { return GetPageGeneral()?.Components[ "Restart To Apply Changes" ] as UIControl; }

		//Graphics page controls
		UIControl GetPageGraphics() { return Components[ @"Tab Control\Page Graphics\Control" ] as UIControl; }
		UIList GetListAntialiasingBasic() { return GetPageGraphics()?.Components[ "List Antialiasing Basic" ] as UIList; }
		UIList GetListAntialiasingMotion() { return GetPageGraphics()?.Components[ "List Antialiasing Motion" ] as UIList; }
		UIList GetListResolutionUpscaleMode() { return GetPageGraphics()?.Components[ "List Resolution Upscale Mode" ] as UIList; }
		UIList GetListResolutionUpscaleTechnique() { return GetPageGraphics()?.Components[ "List Resolution Upscale Technique" ] as UIList; }
		UICheck GetCheckSharpnessDefault() { return GetPageGraphics()?.Components[ "Check Sharpness Default" ] as UICheck; }
		UISlider GetSliderSharpness() { return GetPageGraphics()?.Components[ "Slider Sharpness" ] as UISlider; }
		UIText GetTextSharpness() { return GetPageGraphics()?.Components[ "Text Sharpness" ] as UIText; }

		//Graphics 2 page controls

		UIControl GetPageGraphics2() { return Components[ @"Tab Control\Page Graphics 2\Control" ] as UIControl; }

		UISlider GetSliderLOD() { return GetPageGraphics2()?.Components[ "Slider LOD" ] as UISlider; }
		UIText GetTextLOD() { return GetPageGraphics2()?.Components[ "Text LOD" ] as UIText; }

		UISlider GetSliderTexture() { return GetPageGraphics2()?.Components[ "Slider Texture" ] as UISlider; }
		UIText GetTextTexture() { return GetPageGraphics2()?.Components[ "Text Texture" ] as UIText; }

		UISlider GetSliderShadow() { return GetPageGraphics2()?.Components[ "Slider Shadow" ] as UISlider; }
		UIText GetTextShadow() { return GetPageGraphics2()?.Components[ "Text Shadow" ] as UIText; }

		UISlider GetSliderMotionBlur() { return GetPageGraphics2()?.Components[ "Slider Motion Blur" ] as UISlider; }
		UIText GetTextMotionBlur() { return GetPageGraphics2()?.Components[ "Text Motion Blur" ] as UIText; }

		UISlider GetSliderIndirectLighting() { return GetPageGraphics2()?.Components[ "Slider Indirect Lighting" ] as UISlider; }
		UIText GetTextIndirectLighting() { return GetPageGraphics2()?.Components[ "Text Indirect Lighting" ] as UIText; }

		UISlider GetSliderAO() { return GetPageGraphics2()?.Components[ "Slider AO" ] as UISlider; }
		UIText GetTextAO() { return GetPageGraphics2()?.Components[ "Text AO" ] as UIText; }

		UISlider GetSliderReflection() { return GetPageGraphics2()?.Components[ "Slider Reflection" ] as UISlider; }
		UIText GetTextReflection() { return GetPageGraphics2()?.Components[ "Text Reflection" ] as UIText; }

		UISlider GetSliderDOF() { return GetPageGraphics2()?.Components[ "Slider DOF" ] as UISlider; }
		UIText GetTextDOF() { return GetPageGraphics2()?.Components[ "Text DOF" ] as UIText; }

		UISlider GetSliderBloom() { return GetPageGraphics2()?.Components[ "Slider Bloom" ] as UISlider; }
		UIText GetTextBloom() { return GetPageGraphics2()?.Components[ "Text Bloom" ] as UIText; }

		//

		protected override void OnEnabledInSimulation()
		{
			if( GetTabControl() != null )
				GetTabControl().SelectedIndex = 0;

			if( GetButtonClose() != null )
			{
				GetButtonClose().Click += delegate ( UIButton sender )
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

			var checkDisplayFrameInfo = GetCheckDisplayFrameInfo();
			if( checkDisplayFrameInfo != null )
			{
				checkDisplayFrameInfo.Checked = SimulationApp.DisplayFrameInfo ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkDisplayFrameInfo.CheckedChanged += delegate ( UICheck obj )
				{
					SimulationApp.DisplayFrameInfo = obj.Checked.Value == UICheck.CheckValue.Checked;
				};
			}

			var checkDisplaySceneInfo = GetCheckDisplaySceneInfo();
			if( checkDisplaySceneInfo != null )
			{
				checkDisplaySceneInfo.Checked = SimulationApp.DisplaySceneInfo ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkDisplaySceneInfo.CheckedChanged += delegate ( UICheck obj )
				{
					SimulationApp.DisplaySceneInfo = obj.Checked.Value == UICheck.CheckValue.Checked;
				};
			}

			var checkDisplayEngineInfo = GetCheckDisplayEngineInfo();
			if( checkDisplayEngineInfo != null )
			{
				checkDisplayEngineInfo.Checked = SimulationApp.DisplayEngineInfo ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;
				checkDisplayEngineInfo.CheckedChanged += delegate ( UICheck obj )
				{
					SimulationApp.DisplayEngineInfo = obj.Checked.Value == UICheck.CheckValue.Checked;
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
				listVideoMode.ReadOnly = SystemSettings.MobileDevice || SystemSettings.CurrentPlatform == SystemSettings.Platform.Web;
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
				checkVerticalSync.ReadOnly = SystemSettings.MobileDevice || SystemSettings.CurrentPlatform == SystemSettings.Platform.Web;
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

			if( GetSliderSharpness() != null )
				GetSliderSharpness().ReadOnly = SimulationApp.Sharpness < 0;

			if( GetTextSharpness() != null )
				GetTextSharpness().Text = SimulationApp.Sharpness < 0 ? "" : SimulationApp.Sharpness.ToString( "F1" );

			if( GetTextLOD() != null )
				GetTextLOD().Text = SimulationApp.LODScale.ToString( "F1" );

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

			if( GetTextMotionBlur() != null )
				GetTextMotionBlur().Text = SimulationApp.MotionBlurMultiplier.ToString( "F1" );

			if( GetTextDOF() != null )
				GetTextDOF().Text = SimulationApp.DepthOfFieldBlurFactor.ToString( "F1" );

			if( GetTextBloom() != null )
				GetTextBloom().Text = SimulationApp.BloomScale.ToString( "F1" );
		}
	}
}