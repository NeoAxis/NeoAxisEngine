#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Resources Window.
	/// </summary>
	public partial class ResourcesWindow : DockWindow
	{
		public ResourcesWindow()
		{
			InitializeComponent();

			//!!!!title. в дизайнере ведь круче

			//var data = new ContentBrowser.ResourcesModeDataClass();
			//data.selectionMode = ResourceSelectionMode.None;
			resourcesBrowser1.Init( null, null, /*data, */null );
			resourcesBrowser1.Options.PanelMode = ContentBrowser.PanelModeEnum.TwoPanelsSplitHorizontally;
			resourcesBrowser1.Options.SplitterPosition = 3.0 / 5.0;
			resourcesBrowser1.Options.EditorButton = false;
			resourcesBrowser1.Options.SettingsButton = false;
			resourcesBrowser1.Options.DisplayPropertiesEditorSettingsButtons = false;

			////!!!!только для главной выставлять
			//resourcesBrowser1.thisIsMainResourcesWindow = true;

			Config_Load();
			EngineConfig.SaveEvent += Config_SaveEvent;

			WindowTitle = EditorLocalization2.Translate( "Windows", WindowTitle );
		}

		public override bool HideOnRemoving { get { return true; } }

		[Browsable( false )]
		public ContentBrowser ContentBrowser1
		{
			get { return resourcesBrowser1; }
		}

		public override ObjectsInFocus GetObjectsInFocus()
		{
			return new ObjectsInFocus( null, resourcesBrowser1.SelectedItems );
			//return new ObjectsInFocus( null, resourcesBrowser1.GetSelectedContainedObjects() );
		}

		void Config_Load()
		{
			var windowBlock = EngineConfig.TextBlock.FindChild( nameof( ResourcesWindow ) );
			if( windowBlock != null )
			{
				var browserBlock = windowBlock.FindChild( "ContentBrowser" );
				if( browserBlock != null )
				{
					ContentBrowser1.Options.Load( browserBlock );

					var filteringModeName = browserBlock.GetAttribute( nameof( ContentBrowser.FilteringMode ) );
					if( !string.IsNullOrEmpty( filteringModeName ) )
						ContentBrowser1.FilteringMode = ContentBrowser.FilteringModes.FirstOrDefault( m => m.Name == filteringModeName );
				}
			}
		}

		void Config_SaveEvent()
		{
			var configBlock = EngineConfig.TextBlock;

			var old = configBlock.FindChild( nameof( ResourcesWindow ) );
			if( old != null )
				configBlock.DeleteChild( old );

			var windowBlock = configBlock.AddChild( nameof( ResourcesWindow ) );
			var browserBlock = windowBlock.AddChild( "ContentBrowser" );
			ContentBrowser1.Options.Save( browserBlock );

			if( ContentBrowser1.FilteringMode != null )
				browserBlock.SetAttribute( nameof( ContentBrowser.FilteringMode ), ContentBrowser1.FilteringMode.Name );
		}
	}
}

#endif