using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Navigator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace NeoAxis.Editor
{
	//class EngineKryptonDockingManager : KryptonDockingManager
	//{
	//	protected override void OnAutoHiddenShowingStateChanged( AutoHiddenShowingStateEventArgs e )
	//	{
	//		base.OnAutoHiddenShowingStateChanged( e );

	//		if( e.NewState == DockingAutoHiddenShowState.SlidingOut && EditorForm.Instance != null )
	//		{
	//			WinFormsUtility.LockFormUpdate( EditorForm.Instance );
	//			EditorForm.Instance.unlockFormUpdateInTimer = DateTime.Now;
	//		}
	//	}
	//}

	/// <summary>
	/// Helper class, to create initial DocumentWindow workspace configuration, and save to XML config.
	/// 
	/// only for wokspaces child windows.
	/// </summary>
	public static class KryptonConfigGenerator
	{
		public static string CreateEditorDocumentXmlConfiguration( IEnumerable<Component> components, Component selected = null )
		{
			var config = new List<WorkspaceControllerForWindow.WindowConfig>();
			int selectedIndex = -1;

			foreach( var comp in components )
			{
				config.Add( WorkspaceControllerForWindow.WindowConfig.FromComponent( comp ) );

				if( selected == comp )
					selectedIndex = config.Count - 1;
			}
			return CreateEditorDocumentXmlConfiguration( config, selectedIndex );
		}

		internal static string CreateEditorDocumentXmlConfiguration( List<WorkspaceControllerForWindow.WindowConfig> config, int selectedIndex = -1 )
		{
			var dockingManager = new KryptonDockingManager();
			dockingManager.PageSaving += DockingManager_PageSaving;

			var dockingWorkspace = new KryptonDockingWorkspace( "DockingWorkspace" );
			dockingManager.Add( dockingWorkspace );

			var pages = new List<KryptonPage>();
			foreach( var cfg in config )
				pages.Add( new KryptonPage() { Tag = cfg } );

			var workspace = dockingManager.AddToWorkspace( "DockingWorkspace", pages.ToArray() );
			if( selectedIndex != -1 )
				workspace.SelectPage( pages[ selectedIndex ].UniqueName );

			using( var stream = new MemoryStream() )
			{
				dockingManager.SaveConfigToStream( stream, System.Text.Encoding.Unicode, Formatting.None );
				dockingManager.Dispose();

				return System.Text.Encoding.Unicode.GetString( stream.ToArray() );
			}
		}

		private static void DockingManager_PageSaving( object sender, DockPageSavingEventArgs e )
		{
			var wc = e.Page.Tag as WorkspaceControllerForWindow.WindowConfig;
			wc.Save( e.XmlWriter );
		}
	}
}
