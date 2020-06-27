// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace NeoAxis.Editor
{
	public partial class Component_Sound_PreviewControl : PreviewControl
	{
		Sound sound;
		SoundVirtualChannel channel;

		//

		public Component_Sound_PreviewControl()
		{
			InitializeComponent();
		}

		private void Component_Sound_SettingsCell_Load( object sender, EventArgs e )
		{
			timer1.Start();
		}

		private void buttonPlay_Click( object sender, EventArgs e )
		{
			Clicked( false );
		}

		private void buttonLoopPlay_Click( object sender, EventArgs e )
		{
			Clicked( true );
		}

		void Clicked( bool loop )
		{
			if( channel != null && !channel.Stopped )
				Stop();
			else
				Play( loop );
		}

		void Play( bool loop )
		{
			var soundComponent = ObjectForPreview as Component_Sound;

			if( soundComponent != null && soundComponent.Result != null )
			{
				long length = 0;
				string fileName = soundComponent.LoadFile.Value.ResourceName;
				if( !string.IsNullOrEmpty( fileName ) )
				{
					try
					{
						length = VirtualFile.GetLength( fileName );
						//using( var stream = VirtualFile.Open( fileName ) )
						//	length = (int)stream.Length;
					}
					catch { }
				}

				if( length != 0 )
				{
					SoundModes mode = 0;
					if( Path.GetExtension( fileName ).ToLower() == ".ogg" && length > 400000 )
						mode |= SoundModes.Stream;
					if( loop )
						mode |= SoundModes.Loop;

					sound = soundComponent.Result.LoadSoundByMode( mode );
					if( sound != null )
						channel = SoundWorld.SoundPlay( null, sound, EngineApp.DefaultSoundChannelGroup, .5f );
				}
			}
		}

		void Stop()
		{
			if( channel != null )
			{
				channel.Stop();
				channel = null;
			}
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			if( channel != null && channel.Stopped )
				channel = null;

			buttonPlay.Text = channel == null ? "Play" : "Stop";
			buttonLoopPlay.Text = channel == null ? "Loop Play" : "Stop";
			//buttonPlay.Text = ToolsLocalization.Translate( "SoundVideoResourceEditor",
			//	( channel == null ) ? "Play" : "Stop" );
		}
	}
}
