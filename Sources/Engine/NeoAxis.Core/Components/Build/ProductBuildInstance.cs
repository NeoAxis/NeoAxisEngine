// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoAxis.Editor
{
	public class ProductBuildInstance
	{
		Product configuration;
		string destinationFolder;
		bool run;
		volatile StateEnum state = StateEnum.Building;
		volatile float progress;
		volatile string error = "";

		Task buildTask;
		volatile bool requestCancel;

		bool buildFunctionFinished;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum StateEnum
		{
			Building,
			Cancelled,
			Error,
			Success,
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public Product Configuration
		{
			get { return configuration; }
		}

		public string DestinationFolder
		{
			get { return destinationFolder; }
		}

		////!!!!почему имя папки получается из имени пакета
		//public string ProductFolder
		//{
		//	get { return Path.Combine( destinationFolder, configuration.ProductName ); }
		//}

		public bool Run
		{
			get { return run; }
		}

		public StateEnum State
		{
			get { return state; }
			set { state = value; }
		}

		public float Progress
		{
			get { return progress; }
			set
			{
				if( value > 1 )
					value = 1;
				progress = value;
			}
		}

		public void SetProgressWithRange( double progress, Range progressRange )
		{
			if( progress > 1 )
				progress = 1;
			var progress2 = progressRange.Minimum + progress * progressRange.Size;
			Progress = (float)progress2;
		}

		public string Error
		{
			get { return error; }
			set { error = value; }
		}

		public bool RequestCancel
		{
			get { return requestCancel; }
			set { requestCancel = value; }
		}

		public bool BuildFunctionFinished
		{
			get { return buildFunctionFinished; }
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		static void BuildFunction( object obj )
		{
			var instance = (ProductBuildInstance)obj;
			instance.configuration.BuildFunction( instance );
			instance.buildFunctionFinished = true;
		}

		public static ProductBuildInstance Start( Product configuration, string destinationFolder, bool run )
		{
			var instance = new ProductBuildInstance();
			instance.configuration = configuration;
			instance.destinationFolder = destinationFolder;
			instance.run = run;

			if( configuration.CanBuildFromThread )
			{
				instance.buildTask = new Task( BuildFunction, instance );
				instance.buildTask.Start();
			}
			else
				BuildFunction( instance );

			return instance;

			//System.Threading.CancellationTokenSource deployCTS;
			//
			//deployCTS = new System.Threading.CancellationTokenSource();
			//var progress = new Progress<int>( p => deployProgressBarValue = p );
			//await Task.Run( () => deployment.Deploy( run, progress, deployCTS.Token ), deployCTS.Token );
			//
			//deployCTS.Cancel( false );
		}
	}
}
