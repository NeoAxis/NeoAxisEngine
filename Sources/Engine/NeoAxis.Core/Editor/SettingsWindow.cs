// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Settings Window.
	/// </summary>
	public partial class SettingsWindow : DockWindow
	{
		//!!!!
		const int maxCachedCount = 10;

		//!!!!как вариант получать через EditorForm.Instance.GetDockWindow
		static SettingsWindow instance;

		List<PanelData> panels = new List<PanelData>();

		PanelData selectedPanel;

		bool settingsDisplayHierarchyOfObjectsInSettingsWindow;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public class PanelData : PanelDataWithTableLayout
		{
			//!!!!public

			public DocumentWindow documentWindow;
			public ESet<object> selectedObjectsSet;

			//

			public new T GetControl<T>() where T : Control
			{
				return GetChildControl<T>( layoutPanel );
			}

			private T GetChildControl<T>( Control control ) where T : Control
			{
				if( control == null )
					return null;

				var child = control.Controls.OfType<T>().FirstOrDefault();
				if( child != null )
					return child;

				foreach( Control item in control.Controls )
				{
					child = GetChildControl<T>( item );
					if( child != null )
						return child;
				}

				return null;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public SettingsWindow()
		{
			if( instance != null )
				Log.Fatal( "SettingsWindow: Constructor: instance != null." );
			instance = this;

			InitializeComponent();

			if( !EditorUtility.IsDesignerHosted( this ) )
				settingsDisplayHierarchyOfObjectsInSettingsWindow = ProjectSettings.Get.DisplayHierarchyOfObjectsInSettingsWindow;

			WindowTitle = EditorLocalization.Translate( "Windows", WindowTitle );
		}

		public override bool HideOnRemoving { get { return true; } }

		private void ContentDocument_Load( object sender, EventArgs e )
		{
		}

		public static SettingsWindow Instance
		{
			get { return instance; }
		}

		bool IsEqual( object[] key1, object[] key2 )
		{
			if( key1.Length != key2.Length )
				return false;
			for( int n = 0; n < key1.Length; n++ )
			{
				if( key1[ n ] != key2[ n ] )
					return false;
			}
			return true;
		}

		PanelData GetPanel( object[] key )
		{
			foreach( var panel in panels )
			{
				if( IsEqual( panel.selectedObjects, key ) )
					return panel;
			}
			return null;
		}

		protected override void OnDestroy()
		{
			RemoveCachedPanels();

			base.OnDestroy();
		}

		PanelData CreatePanel( DocumentWindow documentWindow, object[] key )//, bool willSelected )
		{
			PanelData panel = new PanelData();
			panels.Add( panel );
			panel.documentWindow = documentWindow;
			panel.selectedObjects = key;
			panel.selectedObjectsSet = new ESet<object>( panel.selectedObjects );

			panel.CreateAndAddPanel( this );

			////hide
			//if( !willSelected && panel.layoutPanel != null )
			//{
			//	panel.layoutPanel.Visible = false;
			//	panel.layoutPanel.Enabled = false;
			//}

			//init panel
			if( panel.selectedObjects != null && panel.selectedObjects.Length != 0 )//!!!!так?
			{
				//no sense. same speed
				//bool canSuspendLayout = !ProjectSettings.Get.DisplayHierarchyOfObjectsInSettingsWindow.Value;

				try
				{
					//!!!!
					//!!!!может раньше

					//if( canSuspendLayout )
					//	panel.layoutPanel.SuspendLayout();

					//!!!! отключено. для правильного расчёта splitContainer.SplitterDistance. см ниже.
					//layoutPanel.SuspendLayout();

					//!!!!!

					//if( clear )
					//	Clear();

					//UpdateBegin?.Invoke( this );
					//AllProviders_UpdateBegin?.Invoke( this );

					//OnUpdate();


					var objInfoHeader = new SettingsHeader_ObjectInfo();
					objInfoHeader.Dock = DockStyle.Fill;
					panel.layoutPanel.Controls.Add( objInfoHeader );

					bool allComponentsSelected = Array.TrueForAll( panel.selectedObjects, obj => obj is Component );
					if( allComponentsSelected )
					{
						if( ProjectSettings.Get.DisplayHierarchyOfObjectsInSettingsWindow.Value )
						{
							//with components hierarchy on second level

							var splitContainer = new KryptonSplitContainer();
							splitContainer.SplitterWidth = 8;
							splitContainer.Orientation = Orientation.Horizontal;
							splitContainer.Dock = DockStyle.Fill;
							DarkThemeUtility.ApplyToSplitter( splitContainer );
							panel.layoutPanel.Controls.Add( splitContainer );

							{
								var componentsHeader = new SettingsHeader_Components();
								//header.Init( documentWindow );
								componentsHeader.Dock = DockStyle.Fill;
								splitContainer.Panel1.Controls.Add( componentsHeader );

								// components panel autosize.
								float splitterPercent = (float)componentsHeader.CalculateHeight() / splitContainer.Height;
								float minCoef = 0.33f;
								if( splitterPercent < minCoef )
									splitContainer.SplitterDistance = componentsHeader.CalculateHeight() + 25;
								else
									splitContainer.SplitterDistance = (int)( splitContainer.Height * minCoef );
							}

							{
								var window = new SettingsLevel2Window();
								window.Dock = DockStyle.Fill;
								splitContainer.Panel2.Controls.Add( window );
							}

							//теперь ниже
							////select root of level 2
							////!!!!всегда Component? если нет, то и окна этого нет
							//panel.GetControl<SettingsHeader_Components>()?.SelectObjects( new Component[] { (Component)panel.selectedObjects[ 0 ] } );

							//UpdateEnd?.Invoke( this );
							//AllProviders_UpdateEnd?.Invoke( this );

							//provider.PerformUpdate( false );
						}
						else
						{
							//without components hierarchy on second level

							var window = new SettingsLevel2Window();
							window.Dock = DockStyle.Fill;
							panel.layoutPanel.Controls.Add( window );

							window.SelectObjects( panel.documentWindow, panel.selectedObjects );
						}
					}
					else
					{
						//not components. can be flowchart Connector.

						//!!!!можно показать свойства в гриде read only

					}

				}
				finally
				{
					//if( canSuspendLayout )
					//	panel.layoutPanel.ResumeLayout();

					//!!!! отключено. для правильного расчёта splitContainer.SplitterDistance
					//layoutPanel.ResumeLayout();
				}
			}

			return panel;
		}

		void RemovePanel( PanelData panel )
		{
			if( SelectedPanel == panel )
				SelectedPanel = null;

			var control = panel.layoutPanel;
			control.Parent.Controls.Remove( control );
			control.Dispose();

			panels.Remove( panel );
		}

		public void RemoveCachedPanels()
		{
			while( panels.Count != 0 )
				RemovePanel( panels[ panels.Count - 1 ] );
		}

		public void SelectObjects( DocumentWindow documentWindow, ICollection<object> objects )
		{
			if( EditorAPI.ClosingApplication )
				return;
			//!!!!new
			var document = documentWindow?.Document;
			if( document != null && document.Destroyed )
				return;

			if( objects == null )
				objects = new object[ 0 ];

			object[] key = new object[ objects.Count ];
			objects.CopyTo( key, 0 );

			PanelData panel = GetPanel( key );

			//find cached panel
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

				panel = CreatePanel( documentWindow, key );//, true );
			}

			//select first object
			if( panel.selectedObjects != null && panel.selectedObjects.Length != 0 )//!!!!так?
			{
				bool allComponentsSelected = Array.TrueForAll( panel.selectedObjects, obj => obj is Component );
				if( allComponentsSelected )
				{
					//select root of level 2
					//!!!!всегда Component? если нет, то и окна этого нет

					var list = new List<Component>();
					foreach( var obj in panel.selectedObjects )
						list.Add( (Component)obj );
					panel.GetControl<SettingsHeader_Components>()?.SelectObjects( list.ToArray() );

					//panel.GetControl<SettingsHeader_Components>()?.SelectObjects( new Component[] { (Component)panel.selectedObjects[ 0 ] } );
				}
			}

			SelectedPanel = panel;
		}

		[Browsable( false )]
		public PanelData SelectedPanel
		{
			get { return selectedPanel; }
			set
			{
				if( selectedPanel == value )
					return;

				if( selectedPanel != null )
				{
					selectedPanel.layoutPanel.Visible = false;
					selectedPanel.layoutPanel.Enabled = false;
				}

				selectedPanel = value;

				if( selectedPanel != null )
				{
					selectedPanel.layoutPanel.Enabled = true;
					selectedPanel.layoutPanel.Visible = true;
					//selectedPanel.control.Focus();
				}
			}
		}

		//!!!!надо ли?
		[Browsable( false )]
		public object[] SelectedObjects//public object[] _SelectedObjects
		{
			get
			{
				var panel = SelectedPanel;
				if( panel == null )
					return new object[ 0 ];
				return panel.selectedObjects;
			}
		}

		//!!!!надо ли?
		[Browsable( false )]
		public ESet<object> SelectedObjectsSet//public ESet<object> _SelectedObjectsSet
		{
			get
			{
				var panel = SelectedPanel;
				if( panel == null )
					return new ESet<object>();
				return panel.selectedObjectsSet;
			}
		}

		//!!!!надо ли?
		public bool IsObjectSelected( object obj )
		{
			return SelectedObjectsSet.Contains( obj );
		}
		//public bool _IsObjectSelected( object obj )
		//{
		//	return _SelectedObjectsSet.Contains( obj );
		//}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || EditorUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			UpdateEnabled();

			//check for DisplayHierarchyOfObjectsInSettingsWindow update
			if( settingsDisplayHierarchyOfObjectsInSettingsWindow != ProjectSettings.Get.DisplayHierarchyOfObjectsInSettingsWindow.Value )
			{
				settingsDisplayHierarchyOfObjectsInSettingsWindow = ProjectSettings.Get.DisplayHierarchyOfObjectsInSettingsWindow.Value;

				DocumentWindow documentWindow = null;
				ESet<object> selectedObjectsSet = null;
				if( SelectedPanel != null )
				{
					documentWindow = SelectedPanel.documentWindow;
					selectedObjectsSet = SelectedPanel.selectedObjectsSet;
				}

				RemoveCachedPanels();

				if( selectedObjectsSet != null )
					SelectObjects( documentWindow, selectedObjectsSet );
			}
		}

		void UpdateEnabled()
		{
			//!!!!slowly

			bool enable = false;

			if( SelectedPanel != null && SelectedPanel.selectedObjects != null && SelectedPanel.selectedObjects.Length != 0 )
			{
				var objs = SelectedPanel.selectedObjects;

				enable = true;

				foreach( object obj in objs )
				{
					Component c = obj as Component;
					if( c != null )
					{
						if( c.ParentRoot.HierarchyController != null )
						{
							var ins = c.ParentRoot.HierarchyController.CreatedByResource;
							if( ins != null )
							{
								if( EditorAPI.GetDocumentByResource( ins ) == null )
									enable = false;
							}
						}
						if( c.EditorReadOnlyInHierarchy )
							enable = false;
					}
				}
			}

			Enabled = enable;
		}

		public override ObjectsInFocus GetObjectsInFocus()
		{
			var panel = SelectedPanel;
			if( panel != null )
			{
				var window2 = panel.GetControl<SettingsLevel2Window>();
				if( window2 != null )
				{
					var panel2 = window2.SelectedPanel;
					if( panel2 != null )
						return new ObjectsInFocus( panel.documentWindow, panel2.selectedObjects );
				}
			}
			return null;
		}
	}
}