// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Windows.Forms;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Custom Splash Screen page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_CustomSplashScreen : ProjectSettingsPage
	{
		public enum EngineSplashScreenStyleEnum
		{
			WhiteBackground,
			BlackBackground,
			Disabled,
		}

		/// <summary>
		/// The style of the engine splash screen when the application is launched. Subscribe to NeoAxis Pro to enable the option.
		/// </summary>
		[Category( "Custom Splash Screen" )]
		[DefaultValue( EngineSplashScreenStyleEnum.WhiteBackground )]
		public Reference<EngineSplashScreenStyleEnum> EngineSplashScreenStyle
		{
			get { if( _engineSplashScreenStyle.BeginGet() ) EngineSplashScreenStyle = _engineSplashScreenStyle.Get( this ); return _engineSplashScreenStyle.value; }
			set { if( _engineSplashScreenStyle.BeginSet( ref value ) ) { try { EngineSplashScreenStyleChanged?.Invoke( this ); } finally { _engineSplashScreenStyle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EngineSplashScreenStyle"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CustomSplashScreen> EngineSplashScreenStyleChanged;
		ReferenceField<EngineSplashScreenStyleEnum> _engineSplashScreenStyle = EngineSplashScreenStyleEnum.WhiteBackground;

		/// <summary>
		/// The style of the engine splash screen when the application is launched. Subscribe to NeoAxis Pro to enable the option.
		/// </summary>
		[Category( "Custom Splash Screen" )]
		[DefaultValue( EngineSplashScreenStyleEnum.WhiteBackground )]
		[DisplayName( "Engine Splash Screen Style" )]
		public Reference<EngineSplashScreenStyleEnum> EngineSplashScreenStyleReadOnly
		{
			get { return EngineSplashScreenStyleEnum.WhiteBackground; }
		}

		//!!!!
		/////// <summary>
		/////// The total time of engine splash screen in seconds. See SplashScreen.cs to make customized engine logo.
		/////// </summary>
		///// <summary>
		///// The total time of engine splash screen in seconds.
		///// </summary>
		//[Category( "Custom Splash Screen" )]
		////[DisplayName( "Engine Splash Screen Time (Pro)" )]
		//[DefaultValue( 3.0 )]
		//public Reference<double> EngineSplashScreenTime
		//{
		//	get { if( _engineSplashScreenTime.BeginGet() ) EngineSplashScreenTime = _engineSplashScreenTime.Get( this ); return _engineSplashScreenTime.value; }
		//	set { if( _engineSplashScreenTime.BeginSet( ref value ) ) { try { EngineSplashScreenTimeChanged?.Invoke( this ); } finally { _engineSplashScreenTime.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="EngineSplashScreenTime"/> property value changes.</summary>
		//public event Action<ProjectSettingsComponentPage_CustomSplashScreen> EngineSplashScreenTimeChanged;
		//ReferenceField<double> _engineSplashScreenTime = 3.0;

		//!!!!

		///// <summary>
		///// Disables a default splash screen of the engine. See SplashScreen.cs to make customized engine logo.
		///// </summary>
		//[Category( "Custom Splash Screen" )]
		//[DefaultValue( false )]
		//public Reference<bool> CustomizeSplashScreen
		//{
		//	get { if( _customizeSplashScreen.BeginGet() ) CustomizeSplashScreen = _customizeSplashScreen.Get( this ); return _customizeSplashScreen.value; }
		//	set { if( _customizeSplashScreen.BeginSet( ref value ) ) { try { CustomizeSplashScreenChanged?.Invoke( this ); } finally { _customizeSplashScreen.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CustomizeSplashScreen"/> property value changes.</summary>
		//public event Action<ProjectSettingsComponentPage_CustomSplashScreen> CustomizeSplashScreenChanged;
		//ReferenceField<bool> _customizeSplashScreen = false;



		///// <summary>
		///// The customized engine splash screen image. Available only for Pro plan.
		///// </summary>
		//[Category( "Custom Splash Screen" )]
		//[DefaultValue( null )]
		//public Reference<Image> SplashScreenImage
		//{
		//	get { if( _splashScreenImage.BeginGet() ) SplashScreenImage = _splashScreenImage.Get( this ); return _splashScreenImage.value; }
		//	set { if( _splashScreenImage.BeginSet( ref value ) ) { try { SplashScreenImageChanged?.Invoke( this ); } finally { _splashScreenImage.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SplashScreenImage"/> property value changes.</summary>
		//public event Action<ProjectSettingsComponentPage_CustomSplashScreen> SplashScreenImageChanged;
		//ReferenceField<Image> _splashScreenImage = null;

		///// <summary>
		///// The total time of engine splash screen in seconds.
		///// </summary>
		//[Category( "Custom Splash Screen" )]
		//[DisplayName( "Engine Splash Screen Time (Pro)" )]
		//[DefaultValue( 3.0 )]
		//public double EngineSplashScreenTimeReadOnly
		//{
		//	get { return 3.0; }
		//}

		///// <summary>
		///// Whether to customize engine splash screen. Available only for Pro plan.
		///// </summary>
		//[Category( "Custom Splash Screen" )]
		//[DisplayName( "Customize Splash Screen" )]
		//[DefaultValue( false )]
		//public bool CustomizeSplashScreenReadOnly
		//{
		//	get { return false; }
		//}

		///// <summary>
		///// The customized engine splash screen image. Available only for Pro plan.
		///// </summary>
		//[Category( "Custom Splash Screen" )]
		//[DisplayName( "Splash Screen Image" )]
		//[DefaultValue( null )]
		//public Image SplashScreenImageReadOnly
		//{
		//	get { return null; }
		//}

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( UserMode )
			{
				if( member is Metadata.Property )
				{
					bool pro = false;
					if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
						pro = LoginUtility.GetLicenseCached().Contains( "Pro" );

					switch( member.Name )
					{
					case nameof( EngineSplashScreenStyle ):
						if( !pro )
							skip = true;
						break;

					case nameof( EngineSplashScreenStyleReadOnly ):
						if( pro )
							skip = true;
						break;
					}
				}
			}
		}
	}
}
