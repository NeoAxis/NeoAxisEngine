// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents splash screen of the editor.
	/// </summary>
	public partial class SplashForm : Form
	{
		static SplashForm instance;

		float time;
		bool allowClose;

		public SplashForm()
		{
			instance = this;
			InitializeComponent();

			//if( WinFormsUtility.IsDesignerHosted( this ) )
			//	return;

			//var image = Properties.Resources.EditorSplash;
			//Size = image.Size;
			//BackgroundImage = image;
		}

		public static SplashForm Instance
		{
			get { return instance; }
		}

		public bool AllowClose
		{
			get { return allowClose; }
			set
			{
				if( value && !allowClose )
					timer1.Start();

				allowClose = value;
			}
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			time += (float)timer1.Interval / 1000.0f;

			bool allowAlpha = true;
			//bool allowAlpha = false;
			//{
			//	if( RenderSystem.Instance != null && RenderSystem.Instance.IsDirect3D() )
			//	{
			//		if( RenderSystem.Instance.GPUIsGeForce() || RenderSystem.Instance.GPUIsRadeon() )
			//			allowAlpha = true;
			//	}
			//}

			float opacity = 1.0f;

			if( allowAlpha )
			{
				if( time > 0 )
					opacity = ( 1.0f - time ) / 1;
				if( opacity < 0 )
					opacity = 0;
			}

			if( Opacity != opacity )
				Opacity = opacity;

			if( time > 1 )
			{
				timer1.Stop();
				Close();
			}
		}

		private void SplashForm_FormClosed( object sender, FormClosedEventArgs e )
		{
			instance = null;
		}
	}
}
