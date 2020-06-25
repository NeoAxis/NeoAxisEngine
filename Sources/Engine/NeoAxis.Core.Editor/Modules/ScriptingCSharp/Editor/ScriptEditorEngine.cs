// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using NeoAxis.Editor;
using RoslynPad.Roslyn;
//using RoslynPad.UI;
using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Script editing engine. Used Roslyn.
	/// </summary>
	sealed class ScriptEditorEngine : ScriptNotificationObject
	{
		static ScriptEditorEngine instance;

		public static ScriptEditorEngine Instance
		{
			get
			{
				if( instance == null )
					instance = new ScriptEditorEngine();
				return instance;
			}
		}

		Exception _lastError;
		public Exception LastError
		{
			get { return _lastError; }
			private set
			{
				SetProperty( ref _lastError, value );
				OnPropertyChanged( nameof( HasError ) );
			}
		}

		public bool HasError => LastError != null;

		//public double MinimumEditorFontSize => 8;
		//public double MaximumEditorFontSize => 72;

		//[EngineConfig( "C# Editor", "FontSize" )]
		//static double _editorFontSize;
		//public double EditorFontSize
		//{
		//	get { return _editorFontSize; }
		//	set
		//	{
		//		if( value < MinimumEditorFontSize || value > MaximumEditorFontSize ) return;

		//		if( SetProperty( ref _editorFontSize, value ) )
		//			EditorFontSizeChanged?.Invoke( value );
		//	}
		//}

		//public event Action<double> EditorFontSizeChanged;

		IServiceProvider _serviceProvider;
		public IServiceProvider GetServiceProvider()
		{
			if( _serviceProvider != null )
				return _serviceProvider;

			var container = new System.Composition.Hosting.ContainerConfiguration().WithAssembly( typeof( ScriptEditorEngine ).Assembly );
			_serviceProvider = container.CreateContainer().GetExport<IServiceProvider>();
			return _serviceProvider;
		}

		//public const string NuGetPathVariableName = "$NuGet";

		// settings
		//public bool OptimizeCompilation { get; set; }
		//public int LiveModeDelayMs { get; set; } = 2000;

		//!!!!
		//internal ScriptNuGetViewModel NuGet { get; private set; }
		internal NuGetConfiguration NuGetConfiguration { get; private set; }


		public ScriptEditorEngine()
		{
			//_editorFontSize = 13;

			EngineConfig.RegisterClassParameters( typeof( ScriptEditorEngine ) );
		}

		public void Initialize()
		{
			var uri = new Uri( typeof( ScriptEditorEngine ).Assembly.GetName().Name +
				";component/Modules/ScriptingCSharp/Resources/Resources.xaml", UriKind.Relative );
			Application.Current.Resources.MergedDictionaries.Add( new ResourceDictionary { Source = uri } );

			Application.Current.DispatcherUnhandledException += ( sender, args ) => OnUnhandledDispatcherException( args );

			try
			{
				//NuGet = new ScriptNuGetViewModel();
				//NuGetConfiguration = new NuGetConfiguration( NuGet.GlobalPackageFolder, NuGetPathVariableName );

				// init references and using
				ScriptingCSharpEngine.Init();

				new RoslynHost( NuGetConfiguration );
			}
			catch( Exception exc )
			{
				Log.Warning( "ScriptEditor initialization failed: \n\n" + exc.ToString() );
			}
		}

		internal void ClearError()
		{
			LastError = null;
		}

		//static void WriteOutput( ScriptViewModel viewModel )
		//{
		//	//!!!!
		//	foreach( var result in viewModel.Results )
		//		Log.Info( result.ToString() );

		//	//var outputWindow = EditorForm.Instance.WorkspaceController.FindWindow<OutputWindow>();
		//	//if (outputWindow == null)
		//	//	return;

		//	//foreach (var result in script.Results)
		//	//	outputWindow.Print(result.ToString());
		//	//outputWindow.Print("");
		//}

		protected override void OnPropertyChanged( [CallerMemberName] string propertyName = null )
		{
			if( propertyName == nameof( ScriptEditorEngine.HasError ) )
				Log.Error( LastError.ToString() );
		}

		void OnUnhandledDispatcherException( DispatcherUnhandledExceptionEventArgs args )
		{
			var exception = args.Exception;
			if( exception is OperationCanceledException )
			{
				args.Handled = true;
				return;
			}
			LastError = exception;
			args.Handled = true;
		}

		//public void UpdateSettings()
		//{
		//	try
		//	{
		//		CSharpEditorSettings.DisplayInfoMarkers = ProjectSettings.Get.CSharpEditor_DisplayInfoMarkers;
		//		CSharpEditorSettings.DisplayWarningMarkers = ProjectSettings.Get.CSharpEditor_DisplayWarningMarkers;
		//		CSharpEditorSettings.DisplayErrorMarkers = ProjectSettings.Get.CSharpEditor_DisplayErrorMarkers;
		//	}
		//	catch { }
		//}
	}
}