#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Workspace;
using Internal.ComponentFactory.Krypton.Docking;
using NeoAxis;
using System.Reflection;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	//!!!!опцией скрывать ли окно, если нет данных

	/// <summary>
	/// Represents the Preview Window.
	/// </summary>
	public partial class PreviewWindow : DockWindow
	{
		//!!!!!memory. много виевпортов. где еще

		//!!!!
		const int maxCachedCount = 10;

		//!!!!
		static PreviewWindow instance;

		public class PanelData
		{
			//!!!!

			public object[] objects;
			//public SettingsLevel2Window.PanelData settingsPanel;
			public PreviewControl control;
		}
		List<PanelData> panels = new List<PanelData>();

		PanelData selectedPanel;

		double lastUpdateTime;

		//

		public PreviewWindow()
		{
			if( instance != null )
				Log.Fatal( "PreviewWindow: Constructor: instance != null." );
			instance = this;

			InitializeComponent();

			WindowTitle = EditorLocalization2.Translate( "Windows", WindowTitle );
		}

		public override bool HideOnRemoving { get { return true; } }

		public static PreviewWindow Instance
		{
			get { return instance; }
		}

		static bool ArrayEquals( object[] array1, object[] array2 )
		{
			if( array1.Length != array2.Length )
				return false;
			for( int n = 0; n < array1.Length; n++ )
				if( !ReferenceEquals( array1[ n ], array2[ n ] ) )
					return false;
			return true;
		}

		PanelData GetPanel( object[] objects )
		{
			foreach( var panel in panels )
			{
				if( ArrayEquals( panel.objects, objects ) )
					return panel;
			}
			return null;

			//foreach( var panel in panels )
			//{
			//	if( panel.settingsPanel == settingsPanel )
			//		return panel;
			//}
			//return null;
		}

		//PanelData GetPanel( SettingsLevel2Window.PanelData settingsPanel )
		//{
		//	foreach( var panel in panels )
		//	{
		//		if( panel.settingsPanel == settingsPanel )
		//			return panel;
		//	}
		//	return null;
		//}

		protected override void OnDestroy()
		{
			RemoveCachedPanels();

			base.OnDestroy();
		}

		Type GetPreviewClass( object obj )
		{
			//!!!!!как не из C# делать. event

			//!!!!было
			////!!!!какой-то хак
			////show only if no scene
			//var objectInSpace = obj as ObjectInSpace;
			//if( objectInSpace != null && objectInSpace.ParentScene != null )
			//	return null;

			var attribs = (PreviewAttribute[])obj.GetType().GetCustomAttributes( typeof( PreviewAttribute ), true );
			if( attribs.Length != 0 )
			{
				var attrib = attribs[ 0 ];
				if( !string.IsNullOrEmpty( attrib.PreviewClassName ) )
				{
					var type = EditorUtility.GetTypeByName( attrib.PreviewClassName );
					if( type == null )
						Log.Warning( $"PreviewWindow: GetPreviewClass: Class with name \"{attrib.PreviewClassName}\" is not found." );
					return type;
				}
				else
					return attrib.PreviewClass;
			}

			return null;
		}

		PanelData CreatePanel( object[] objects, bool willSelected )
		//PanelData CreatePanel( SettingsLevel2Window.PanelData settingsPanel, bool willSelected )
		{
			PanelData panel = new PanelData();
			panels.Add( panel );
			panel.objects = objects;
			//panel.settingsPanel = settingsPanel;

			PreviewControl control = null;
			{
				if( objects.Length == 1 )
				//if( settingsPanel.selectedObjects.Length == 1 )
				{
					var obj = objects[ 0 ];
					//var obj = settingsPanel.selectedObjects[ 0 ];

					//!!!!!как не из C# делать

					var previewClass = GetPreviewClass( obj );
					if( previewClass != null )
					{
						if( typeof( CanvasBasedPreview ).IsAssignableFrom( previewClass ) )
						{
							var preview = (CanvasBasedPreview)Activator.CreateInstance( previewClass );
							var control2 = new PreviewControlWithViewport_CanvasBasedPreview( preview );
							preview.owner = control2;
							control = control2;
						}
						else
						{
							control = (PreviewControl)previewClass.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null );
						}
					}

					//bool allow = true;

					////!!!!какой-то хак
					////show only if no scene
					//var objectInSpace = obj as ObjectInSpace;
					//if( objectInSpace != null && objectInSpace.ParentScene != null )
					//	allow = false;

					//if( allow )
					//{
					//	var attribs = (EditorPreviewControlAttribute[])obj.GetType().GetCustomAttributes( typeof( EditorPreviewControlAttribute ), true );
					//	foreach( EditorPreviewControlAttribute attr in attribs )
					//	{
					//		control = (PreviewControl)attr.PreviewClass.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic |
					//			BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null );
					//		break;
					//	}
					//}
				}

				//!!!!!
				if( control == null )
					control = new PreviewControl();
			}

			control.Panel = panel;

			panel.control = control;
			control.Dock = DockStyle.Fill;
			Controls.Add( panel.control );

			//hide
			if( !willSelected && panel.control != null )
			{
				panel.control.Visible = false;
				panel.control.Enabled = false;
			}

			return panel;
		}

		void RemovePanel( PanelData panel )
		{
			if( SelectedPanel == panel )
				SelectedPanel = null;

			var control = panel.control;
			control.Parent.Controls.Remove( control );
			control.Dispose();

			panels.Remove( panel );
		}

		public void RemoveCachedPanels()
		{
			while( panels.Count != 0 )
				RemovePanel( panels[ panels.Count - 1 ] );
		}

		public void SelectObjects( object[] objects )// SettingsLevel2Window.PanelData settingsPanel )
		{
			if( EditorAPI.ClosingApplication )
				return;

			//if( settingsPanel != null )
			if( objects.Length != 0 )
			{
				//find cached panel
				PanelData panel = GetPanel( objects );// settingsPanel );

				bool isAlreadySelected = panel != null && SelectedPanel == panel;
				if( !isAlreadySelected )
				{
					if( panel != null )
					{
						//move to last position (best priority)
						panels.Remove( panel );
						panels.Add( panel );
					}

					//create new panel
					if( panel == null )
					{
						//remove cached
						while( panels.Count >= maxCachedCount )
							RemovePanel( panels[ 0 ] );

						panel = CreatePanel( objects, true );
						//panel = CreatePanel( settingsPanel, true );
					}

					SelectedPanel = panel;
				}
			}
			else
				SelectedPanel = null;
		}

		[Browsable( false )]
		public PanelData SelectedPanel
		{
			get { return selectedPanel; }
			set
			{
				if( selectedPanel == value )
					return;

				var old = selectedPanel;
				selectedPanel = value;

				if( selectedPanel != null )
				{
					selectedPanel.control.Enabled = true;
					selectedPanel.control.Visible = true;
					//selectedPanel.control.Focus();
				}

				if( old != null )
				{
					old.control.Visible = false;
					old.control.Enabled = false;
				}

				//if( selectedPanel != null )
				//{
				//	selectedPanel.control.Visible = false;
				//	selectedPanel.control.Enabled = false;
				//}

				//selectedPanel = value;

				//if( selectedPanel != null )
				//{
				//	selectedPanel.control.Enabled = true;
				//	selectedPanel.control.Visible = true;
				//	//selectedPanel.control.Focus();
				//}

				//!!!!можно приоритеты менять для кешированных. где еще
			}
		}

		object FindWithPreviewClass( object start )
		{
			var objectToSelect = start;

			again:;
			if( objectToSelect != null && GetPreviewClass( objectToSelect ) == null )
			{
				var c = objectToSelect as Component;
				if( c != null )
					objectToSelect = c.Parent;
				else
					objectToSelect = null;

				goto again;
			}

			return objectToSelect;
		}

		void UpdateSelectedPanel()
		{
			var objectsToSelect = new List<object>();
			foreach( var obj in EditorForm.Instance.GetObjectsInFocus().Objects )
			{
				object objectToSelect = null;

				var item = obj as ContentBrowser.Item;
				if( item != null )
				{
					if( item.ContainedObject != null )
						objectToSelect = item.ContainedObject;
				}
				else
					objectToSelect = obj;

				objectToSelect = FindWithPreviewClass( objectToSelect );

				if( objectToSelect != null )
					objectsToSelect.Add( objectToSelect );
			}

			if( objectsToSelect.Count == 0 )
			{
				var window = EditorAPI2.SelectedDocumentWindow;
				if( window != null )
				{
					var obj = FindWithPreviewClass( window.ObjectOfWindow );
					if( obj != null )
						objectsToSelect.Add( obj );
				}
			}

			while( objectsToSelect.Count > 1 )
				objectsToSelect.RemoveAt( objectsToSelect.Count - 1 );

			SelectObjects( objectsToSelect.ToArray() );


			//SelectObjects( EditorForm.Instance.GetObjectsInFocus().objects );

			//SettingsLevel2Window.PanelData panel2 = null;

			//var window = EditorForm.Instance.WorkspaceController.FindWindow<SettingsWindow>();
			//if( window != null )
			//{
			//	var panel = window.SelectedPanel;
			//	if( panel != null )
			//	{
			//		var window2 = panel.GetControl<SettingsLevel2Window>();
			//		if( window2 != null )
			//			panel2 = window2.SelectedPanel;
			//	}
			//}

			//SelectObjects( panel2 );
		}


		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
				return;

			double updateTime = 0.05;
			{
				var count = EditorForm.Instance.GetObjectsInFocus().Objects.Length;
				if( count > 2000 )
					updateTime = 2.0;
				else if( count > 500 )
					updateTime = 1.0;
				else if( count > 250 )
					updateTime = 0.5;
				else if( count > 100 )
					updateTime = 0.1;
				else
					updateTime = 0.05;
			}
			if( EngineApp.GetSystemTime() - lastUpdateTime < updateTime )
				return;

			UpdateSelectedPanel();

			lastUpdateTime = EngineApp.GetSystemTime();
		}

		public override ObjectsInFocus GetObjectsInFocus()
		{
			return null;
		}

		public override Vector2I DefaultAutoHiddenSlideSize
		{
			get { return ( new Vector2( 280, 200 ) * EditorAPI2.DPIScale ).ToVector2I(); }
		}
	}
}

#endif