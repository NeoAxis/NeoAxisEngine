// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a container to manage controls with the ability to set up the structure of controls in the form of a hierarchy.
	/// </summary>
	public partial class HierarchicalContainer : EUserControl, IMessageFilter
	{
		static Dictionary<Type, Type> itemTypeByPropertyType = new Dictionary<Type, Type>();

		//bool gridMode = true;
		//bool readOnlyHierarchy;

		DocumentWindow documentWindow;//engine's special
		object[] selectedObjects;

		//ESet<Item> allItems = new ESet<Item>();
		List<Item> rootItems = new List<Item>();

		//bool firstUpdate = true;
		bool duringUpdate;

		bool destroying;

		IDropDownHolder dropDownHolder;

		internal const bool DrawSplitter = true;
		internal const int SpliterWidth = 6;
		//const bool EnableCompositionOnDragging = true;
		float splitterRatio = 2.0f / 5.0f;
		SplitterState splitterState = SplitterState.None;
		int splitterDragValue = 0;
		Point splitterDragPos = Point.Empty;
		ContentModeEnum contentMode = ContentModeEnum.Properties;

		internal static int CreatedControlsCount { get; set; } = 0;
		internal static int CreatedItemsCount { get; set; } = 0;

		double lastUpdateTime;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Flags]
		enum SplitterState
		{
			None = 0x0,
			Hovered = 0x1,
			Dragged = 0x2
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum ContentModeEnum
		{
			Properties,
			Events
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public delegate void BeforeTimerUpdateDelegate( HierarchicalContainer sender, ref bool cancelUpdate );
		public event BeforeTimerUpdateDelegate BeforeTimerUpdate;

		public delegate void OverridePropertyDisplayNameDelegate( HierarchicalContainer sender, HCItemProperty property, ref string displayName );
		public event OverridePropertyDisplayNameDelegate OverridePropertyDisplayName;

		public delegate void OverrideMemberDescriptionDelegate( HierarchicalContainer sender, HCItemMember member, ref string description );
		public event OverrideMemberDescriptionDelegate OverrideMemberDescription;

		public delegate void OverridePropertyEnumItemDelegate( HierarchicalContainer sender, HCItemEnumDropDown property, ref string displayName, ref string description );
		public event OverridePropertyEnumItemDelegate OverridePropertyEnumItem;

		public delegate void OverrideGroupDisplayNameDelegate( HierarchicalContainer sender, HCItemGroup group, ref string displayName );
		public event OverrideGroupDisplayNameDelegate OverrideGroupDisplayName;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		static HierarchicalContainer()
		{
			RegisterDefaultItemTypeByPropertyType();
		}

		public HierarchicalContainer()
		{
			InitializeComponent();

			//AutoScroll = false;
			////!!!!так?
			//HorizontalScroll.Maximum = 0;
			//AutoScroll = false;
			//VerticalScroll.Visible = false;
			//AutoScroll = true;

			//VerticalScroll.Enabled = true;
			//VerticalScroll.Visible = true;
			//VerticalScroll.Minimum = 0;
			//VerticalScroll.Maximum = 2000;
			//VerticalScroll.Value = 0;

			//RegisterDefaultItemTypeByPropertyType();

			// register filter for redirect scroll event to childs
			Application.AddMessageFilter( this );

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			engineScrollBar1.Visible = false;
			//UpdateControls();

			panel1.MouseLeave += Panel1_MouseLeave;
			panel1.MouseMove += Panel1_MouseMove;
			panel1.MouseDown += Panel1_MouseDown;
			panel1.MouseUp += Panel1_MouseUp;
		}

		//public bool GridMode
		//{
		//	get { return gridMode; }
		//	set { gridMode = value; }
		//}

		//[Browsable( false )]
		//public bool ReadOnlyHierarchy
		//{
		//	get { return readOnlyHierarchy; }
		//	set { readOnlyHierarchy = value; }
		//}

		[Browsable( false )]
		public DocumentWindow DocumentWindow
		{
			get { return documentWindow; }
		}

		[Browsable( false )]
		public object[] SelectedObjects
		{
			get { return selectedObjects; }
		}

		[Browsable( false )]
		public List<Item> RootItems
		{
			get { return rootItems; }
		}

		// 0.0 - 1.0
		public float SplitterRatio
		{
			get { return splitterRatio; }
			set
			{
				var newValue = Math.Max( 50.0f / Width, Math.Min( value, ( Width - 50 ) / (float)Width ) );
				if( splitterRatio != newValue )
				{
					splitterRatio = newValue;
					//OnGridSplitterChanged();

					Invalidate( true );
					//Invalidate();
				}
			}
		}

		public int SplitterPosition
		{
			get { return (int)Math.Round( SplitterRatio * Width ); }
			set { SplitterRatio = value / (float)Width; }
		}

		public ContentModeEnum ContentMode
		{
			get { return contentMode; }
			set
			{
				if( contentMode == value )
					return;
				contentMode = value;

				//PerformUpdate();
				//ContentModeChanged?.Invoke( this, contentMode );
			}
		}

		[DefaultValue( true )]
		public bool DisplayGroups { get; set; } = true;

		private void HierarchicalContainer_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			//engineScrollBar1.Scroll += EngineScrollBar1_Scroll;

			//// need to disable AutoScroll, otherwise disabling the horizontal scrollbar doesn't work
			//AutoScroll = false;
			//// disable horizontal scrollbar
			//VerticalScroll.Enabled = false;
			//VerticalScroll.Visible = false;
			//// restore AutoScroll
			//AutoScroll = true;
		}

		//private void EngineScrollBar1_Scroll( object sender, EngineScrollBarEventArgs e )
		//{
		//	PerformUpdate( true );
		//}

		private void Panel1_MouseLeave( object sender, EventArgs e )
		{
			//if( !GridMode )
			//	return;

			if( splitterState.HasFlag( SplitterState.Hovered ) )
			{
				splitterState &= ~SplitterState.Hovered;
				Cursor = Cursors.Default;
			}
		}

		private void Panel1_MouseMove( object sender, MouseEventArgs e )
		{
			//if( !GridMode )
			//	return;

			if( splitterState.HasFlag( SplitterState.Dragged ) )
			{
				SplitterPosition = splitterDragValue + e.X - splitterDragPos.X;
				OnGridSplitterChanged();
			}
			else
			{
				int splitterPos = SplitterPosition;
				bool splitterHovered = e.Button == MouseButtons.None &&
					e.X < splitterPos && e.X >= splitterPos - SpliterWidth;
				if( splitterHovered && !splitterState.HasFlag( SplitterState.Hovered ) )
				{
					splitterState |= SplitterState.Hovered;
					Cursor = Cursors.VSplit;
				}
				else if( !splitterHovered && splitterState.HasFlag( SplitterState.Hovered ) )
				{
					splitterState &= ~SplitterState.Hovered;
					Cursor = Cursors.Default;
				}
			}
		}

		private void Panel1_MouseDown( object sender, MouseEventArgs e )
		{
			//if( GridMode )
			//{
			if( splitterState.HasFlag( SplitterState.Hovered ) )
			{
				splitterState |= SplitterState.Dragged;
				splitterDragPos = e.Location;
				splitterDragValue = this.SplitterPosition;
				//!!!!было
				//if( EnableCompositionOnDragging )
				//	SetFormLevelDoubleBuffering( this, true );
				//timer50ms.Stop();
			}
			//}
		}

		private void Panel1_MouseUp( object sender, MouseEventArgs e )
		{
			//if( !GridMode )
			//	return;

			if( splitterState.HasFlag( SplitterState.Dragged ) )
			{
				splitterState &= ~SplitterState.Dragged;
				//!!!!было
				//if( EnableCompositionOnDragging )
				//	SetFormLevelDoubleBuffering( this, false );
				//timer50ms.Start();
			}
		}

		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );

			////if( !GridMode )
			////	return;

			//if( splitterState.HasFlag( SplitterState.Hovered ) )
			//{
			//	splitterState &= ~SplitterState.Hovered;
			//	Cursor = Cursors.Default;
			//}
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			////if( !GridMode )
			////	return;

			//if( splitterState.HasFlag( SplitterState.Dragged ) )
			//{
			//	SplitterPosition = splitterDragValue + e.X - splitterDragPos.X;
			//	OnGridSplitterChanged();
			//}
			//else
			//{
			//	int splitterPos = SplitterPosition;
			//	bool splitterHovered = e.Button == MouseButtons.None &&
			//		e.X < splitterPos && e.X >= splitterPos - SpliterWidth;
			//	if( splitterHovered && !splitterState.HasFlag( SplitterState.Hovered ) )
			//	{
			//		splitterState |= SplitterState.Hovered;
			//		Cursor = Cursors.VSplit;
			//	}
			//	else if( !splitterHovered && splitterState.HasFlag( SplitterState.Hovered ) )
			//	{
			//		splitterState &= ~SplitterState.Hovered;
			//		Cursor = Cursors.Default;
			//	}
			//}
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			////if( GridMode )
			////{
			//if( splitterState.HasFlag( SplitterState.Hovered ) )
			//{
			//	splitterState |= SplitterState.Dragged;
			//	splitterDragPos = e.Location;
			//	splitterDragValue = this.SplitterPosition;
			//	//!!!!было
			//	//if( EnableCompositionOnDragging )
			//	//	SetFormLevelDoubleBuffering( this, true );
			//	//timer50ms.Stop();
			//}
			////}

			base.OnMouseDown( e );
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			////if( !GridMode )
			////	return;

			//if( splitterState.HasFlag( SplitterState.Dragged ) )
			//{
			//	splitterState &= ~SplitterState.Dragged;
			//	//!!!!было
			//	//if( EnableCompositionOnDragging )
			//	//	SetFormLevelDoubleBuffering( this, false );
			//	//timer50ms.Start();
			//}
		}

		protected override void OnDestroy()
		{
			Application.RemoveMessageFilter( this );

			destroying = true;
			foreach( var item in RootItems.ToArray() )
				item.Dispose();
			RootItems.Clear();

			base.OnDestroy();
		}

		protected override void OnResize( EventArgs e )
		{
			if( !IsHandleCreated )
				return;

			PerformUpdate();
			//// update layout before resize
			//UpdateItemsLayout();

			base.OnResize( e );
		}

		void OnGridSplitterChanged()
		{
			//if( !GridMode )
			//	return;

			PerformUpdate( true );
			//UpdateItemsLayout();
		}

		protected override Point ScrollToControl( Control activeControl )
		{
			// prevent AutoScroll on focus or content resize - will always scroll to top.
			// solution: Just don't scroll. Won't be needed here anyway.
			return this.AutoScrollPosition;
			//return base.ScrollToControl(activeControl);
		}

		protected override void OnLostFocus( EventArgs e )
		{
			base.OnLostFocus( e );

			// Emulate leaving mouse if losing focus to something that might be a dropdown popup
			//if( !Application.OpenForms.OfType<Form>().Any( c => c.Focused || c.ContainsFocus ) )
			//	this.OnMouseLeave( EventArgs.Empty );
		}

		private void timer50ms_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
				return;

			bool cancel = false;
			BeforeTimerUpdate?.Invoke( this, ref cancel );
			if( cancel )
				return;

			//!!!!когда не обновлять?
			PerformUpdate();
		}

		public void PerformUpdate( bool forceUpdate = false )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( Destroyed || destroying )
				return;

			double updateTime = 0.05;
			if( SelectedObjects != null )
			{
				if( SelectedObjects.Length > 2000 )
					updateTime = 2.0;
				else if( SelectedObjects.Length > 500 )
					updateTime = 1.0;
				else if( SelectedObjects.Length > 250 )
					updateTime = 0.5;
				else if( SelectedObjects.Length > 100 )
					updateTime = 0.35;
				else
					updateTime = 0.1;
			}

			if( EngineApp.GetSystemTime() - lastUpdateTime < updateTime && !forceUpdate )
				return;

			//if( EngineApp.GetSystemTime() - lastUpdateTime < 0.04 )
			//	return;

			if( duringUpdate )
				return;
			try
			{
				duringUpdate = true;

				//start timer
				//if( firstUpdate )
				timer50ms?.Start();

				UpdateItems();

				//firstUpdate = false;
			}
			finally
			{
				lastUpdateTime = EngineApp.GetSystemTime();
				duringUpdate = false;
			}
		}

		public void SetData( DocumentWindow documentWindow, object[] objects, bool callPerformUpdate = true )
		{
			this.documentWindow = documentWindow;
			this.selectedObjects = objects;

			if( callPerformUpdate )
				PerformUpdate();

			//start timer
			timer50ms?.Start();

			//if( callPerformUpdate )
			//	PerformUpdate();
			//else
			//{
			//	//start timer
			//	timer50ms?.Start();
			//}
		}

		IEnumerable<Item> GetNewItems( object[] objects )
		{
			var itemByGroupName = new Dictionary<string, HCItemGroup>();
			var itemByMember = new Dictionary<Metadata.Member, HCItemMember>();
			foreach( var item in rootItems )
			{
				var groupItem = item as HCItemGroup;
				if( groupItem != null )
					itemByGroupName.Add( groupItem.Name, groupItem );
				var memberItem = item as HCItemMember;
				if( memberItem != null )
					itemByMember.Add( memberItem.Member, memberItem );
			}

			var groups = new EDictionary<string, List<Metadata.Member>>();
			//var members = MetadataManager.MetadataGetMembers( obj );
			//if( ReverseItems )
			//	members = members.Reverse();

			var firstObject = objects[ 0 ];

			var membersToAdd = new EDictionary<Metadata.Member, int>( 128 );

			//foreach( var member in members )
			foreach( var member in MetadataManager.MetadataGetMembers( firstObject ) )
			{
				if( contentMode == ContentModeEnum.Properties && !( member is Metadata.Property ) )
					continue;
				if( contentMode == ContentModeEnum.Events && !( member is Metadata.Event ) )
					continue;

				if( EditorUtility.IsMemberVisible( member ) )
				{
					bool skip = false;

					//Type Settings filter
					var component = firstObject as Component;
					if( component != null && !component.TypeSettingsIsPublicMember( member ) )
						skip = true;
					//if( component != null )
					//{
					//var baseComponentType = component.BaseType as Metadata.ComponentTypeInfo;
					//if( baseComponentType != null && baseComponentType.BasedOnObject.TypeSettingsIsPrivateObject( member ) )
					//	skip = true;
					//}

					if( !skip )
						membersToAdd[ member ] = 1;
				}
			}

			for( int nObject = 1; nObject < objects.Length; nObject++ )
			{
				var obj = objects[ nObject ];
				foreach( var member in MetadataManager.MetadataGetMembers( obj ) )
				{
					if( membersToAdd.TryGetValue( member, out var counter ) )
						membersToAdd[ member ] = counter + 1;
				}
			}

			foreach( var pair in membersToAdd )
			{
				var member = pair.Key;
				var counter = pair.Value;
				if( counter == objects.Length )
				{
					var groupName = TypeUtility.GetUserFriendlyCategory( member );

					List<Metadata.Member> list;
					if( !groups.TryGetValue( groupName, out list ) )
					{
						list = new List<Metadata.Member>();
						groups.Add( groupName, list );
					}
					list.Add( member );
				}
			}

			////foreach( var member in members )
			//foreach( var member in MetadataManager.MetadataGetMembers( obj ) )
			//{
			//	if( contentMode == ContentModeEnum.Properties && !( member is Metadata.Property ) )
			//		continue;
			//	if( contentMode == ContentModeEnum.Events && !( member is Metadata.Event ) )
			//		continue;

			//	if( EditorUtility.IsMemberVisible( member ) )
			//	{
			//		bool skip = false;

			//		//Type Settings filter
			//		var component = obj as Component;
			//		if( component != null && !component.TypeSettingsIsPublicMember( member ) )
			//			skip = true;
			//		//if( component != null )
			//		//{
			//		//var baseComponentType = component.BaseType as Metadata.ComponentTypeInfo;
			//		//if( baseComponentType != null && baseComponentType.BasedOnObject.TypeSettingsIsPrivateObject( member ) )
			//		//	skip = true;
			//		//}

			//		if( !skip )
			//		{
			//			var groupName = TypeUtility.GetUserFriendlyCategory( member );

			//			List<Metadata.Member> list;
			//			if( !groups.TryGetValue( groupName, out list ) )
			//			{
			//				list = new List<Metadata.Member>();
			//				groups.Add( groupName, list );
			//			}
			//			list.Add( member );
			//		}
			//	}
			//}

			//!!!!change order

			//reverse groups for Events mode			
			if( ReverseGroups )//if( contentMode == ContentModeEnum.Events )
			{
				var newGroups = new EDictionary<string, List<Metadata.Member>>( groups.Count );
				foreach( var pair in groups.Reverse() )
					newGroups.Add( pair.Key, pair.Value );
				groups = newGroups;
			}

			foreach( var groupPair in groups )
			{
				var groupName = groupPair.Key;
				var list = groupPair.Value;

				//update group
				if( DisplayGroups )
				{
					if( !itemByGroupName.TryGetValue( groupName, out HCItemGroup groupItem ) )
						groupItem = new HCItemGroup( this, null, groupName );
					yield return groupItem;
				}

				//update properties of the group
				foreach( var member in list )
				{
					if( !itemByMember.TryGetValue( member, out var item ) )
					{
						Type itemType = GetSuitableItemType( member );
						//var originalType = property.Type.GetNetType();
						//var unrefType = ReferenceUtils.GetUnreferencedType( originalType );
						//Type itemType = HCPropertyItemTypes.GetSuitableType( unrefType );

						//!!!!может быть null. Component.MetadataGetMembers в _InvokeMember
						if( itemType != null )
						{
							var property = member as Metadata.Property;
							if( property != null )
							{
								var constructor = itemType.GetConstructor( new Type[] {
										typeof( HierarchicalContainer ),
										typeof( Item ),
										typeof( object[] ),
										typeof( Metadata.Property ),
										typeof( object[] )
									} );

								item = (HCItemMember)constructor.Invoke( new object[] { this, null, SelectedObjects, property, property.Indexers } );
							}

							var _event = member as Metadata.Event;
							if( _event != null )
							{
								var constructor = itemType.GetConstructor( new Type[] {
									typeof( HierarchicalContainer ),
									typeof( Item ),
									typeof( object[] ),
									typeof( Metadata.Event )
								} );

								item = (HCItemMember)constructor.Invoke( new object[] { this, null, SelectedObjects, _event } );
							}
						}
					}

					if( item != null )
						yield return item;
				}
			}
		}

		//!!!!temp. не обновляется в Shortcuts
		internal void UpdateItems()
		//void UpdateItems()
		{
			////!!!!new
			//UpdateControls();

			ESet<Item> newRootItems = new ESet<Item>();

			if( SelectedObjects != null && SelectedObjects.Length != 0 )
			{
				foreach( var item in GetNewItems( SelectedObjects ) )
					newRootItems.Add( item );
			}

			//prepare list of items for deletion
			List<Item> itemsToDelete = new List<Item>();
			foreach( var item in rootItems )
			{
				if( !newRootItems.Contains( item ) )
					itemsToDelete.Add( item );
			}

			//replace rootItems list
			rootItems.Clear();
			rootItems.AddRange( newRootItems );

			if( itemsToDelete.Count != 0 )
			{
				SuspendLayout();

				//delete old items
				foreach( var item in itemsToDelete )
					item.Dispose();

				ResumeLayout( false );
			}

			foreach( var item in rootItems )
				item.Update();

			//update items layout
			if( rootItems.Count != 0 )
			{
				// create item controls at first.
				try
				{
					foreach( var item in rootItems )
						item.CreateControl();
				}
				catch { }

				var needVerticalScroll = IsVerticalScrollNeeded( out var itemsHeight );

				if( engineScrollBar1.Visible != needVerticalScroll )
					engineScrollBar1.Visible = needVerticalScroll;

				engineScrollBar1.Maximum = Math.Max( itemsHeight - Height, 0 );
				engineScrollBar1.SmallChange = 30;
				engineScrollBar1.LargeChange = Height;

				//engineScrollBar1.Maximum = Math.Max( itemsHeight - engineScrollBar1.TrackLength, 0 );
				//engineScrollBar1.Maximum = itemsHeight;

				//engineScrollBarTreeVertical.SmallChange = treeView.VScrollBar.SmallChange;
				//engineScrollBarTreeVertical.LargeChange = treeView.VScrollBar.LargeChange;
				//engineScrollBarTreeVertical.Value = treeView.VScrollBar.Value;

				// then update layout.
				UpdateItemsLayout( needVerticalScroll );

				//!!!!
				UpdateControls( needVerticalScroll );
			}
			else
			{
				if( engineScrollBar1.Visible )
					engineScrollBar1.Visible = false;
			}
		}

		bool IsVerticalScrollNeeded( out int itemsHeight )
		{
			itemsHeight = 0;
			//int itemsHeight = 0;
			foreach( var item in rootItems )
				itemsHeight += item.GetTotalHeight();
			return itemsHeight > Height;
		}

		void UpdateItemsLayout( bool needVerticalScroll )
		{
			SuspendLayout();

			int positionY = 0;
			int tabIndex = 0;
			foreach( var item in rootItems )
				item.UpdateLayout( ref positionY, ref tabIndex, needVerticalScroll );

			ResumeLayout( true );
		}

		protected override void AdjustFormScrollbars( bool displayScrollbars )
		{
			//!!!!было

			//// update items layout before scroll appears. it prevent
			//// redrawing with the horizontal scroll showing.

			//if( !VerticalScroll.Visible && IsVerticalScrollNeeded() )
			//	UpdateItemsLayout( true );

			//!!!!new
			displayScrollbars = false;

			// show scroll
			base.AdjustFormScrollbars( displayScrollbars );
		}

		public bool IsDropDownOpen()
		{
			return dropDownHolder != null;
		}

		public void ToggleDropDown( HCDropDownControl control, HCItemProperty itemProperty )
		{
			if( dropDownHolder == null )
			{
				if( control.SpecialHolder )
					dropDownHolder = new HCFormDropDownHolder( control );
				else
					dropDownHolder = new HCToolStripDropDownHolder( control );

				dropDownHolder.Show( itemProperty.CreatedControlInsidePropertyItemControl );
				dropDownHolder.HolderClosed += DropDownHolder_Closed;
			}
			else
			{
				dropDownHolder.Close();
			}
		}

		private void DropDownHolder_Closed( object sender, EventArgs e )
		{
			dropDownHolder.HolderClosed -= DropDownHolder_Closed;
			dropDownHolder = null;
		}

		[Browsable( false )]
		public Dictionary<Type, Type> ItemTypeByPropertyType
		{
			get { return itemTypeByPropertyType; }
		}

		public static void RegisterItemTypeByPropertyType( Type propertyType, Type itemType )
		{
			itemTypeByPropertyType[ propertyType ] = itemType;
		}

		static void RegisterDefaultItemTypeByPropertyType()
		{
			RegisterItemTypeByPropertyType( typeof( object ), typeof( HCItemLabel ) );
			RegisterItemTypeByPropertyType( typeof( Enum ), typeof( HCItemEnumDropDown ) );
			//RegisterItemTypeByPropertyType( typeof( Enum ), typeof( HCItemComboBox ) );

			//simple types
			foreach( var item in SimpleTypes.Types )
				RegisterItemTypeByPropertyType( item.Type, typeof( HCItemTextBox ) );

			//can parse from and convert to string
			//!!!!

			RegisterItemTypeByPropertyType( typeof( bool ), typeof( HCItemCheckBox ) );
			RegisterItemTypeByPropertyType( typeof( ColorValue ), typeof( HCItemColorValue ) );
			RegisterItemTypeByPropertyType( typeof( ColorValuePowered ), typeof( HCItemColorValuePowered ) );

			RegisterItemTypeByPropertyType( typeof( int ), typeof( HCItemTextBoxNumeric ) );
			RegisterItemTypeByPropertyType( typeof( uint ), typeof( HCItemTextBoxNumeric ) );
			RegisterItemTypeByPropertyType( typeof( long ), typeof( HCItemTextBoxNumeric ) );
			RegisterItemTypeByPropertyType( typeof( ulong ), typeof( HCItemTextBoxNumeric ) );
			RegisterItemTypeByPropertyType( typeof( float ), typeof( HCItemTextBoxNumeric ) );
			RegisterItemTypeByPropertyType( typeof( double ), typeof( HCItemTextBoxNumeric ) );
			RegisterItemTypeByPropertyType( typeof( DegreeF ), typeof( HCItemTextBoxNumeric ) );
			RegisterItemTypeByPropertyType( typeof( Degree ), typeof( HCItemTextBoxNumeric ) );
			RegisterItemTypeByPropertyType( typeof( RadianF ), typeof( HCItemTextBoxNumeric ) );
			RegisterItemTypeByPropertyType( typeof( Radian ), typeof( HCItemTextBoxNumeric ) );
			RegisterItemTypeByPropertyType( typeof( Range ), typeof( HCItemRange ) );
			RegisterItemTypeByPropertyType( typeof( RangeF ), typeof( HCItemRange ) );
			RegisterItemTypeByPropertyType( typeof( RangeI ), typeof( HCItemRange ) );
			RegisterItemTypeByPropertyType( typeof( Vector2 ), typeof( HCItemRange ) );
			RegisterItemTypeByPropertyType( typeof( Vector2F ), typeof( HCItemRange ) );
			RegisterItemTypeByPropertyType( typeof( Vector2I ), typeof( HCItemRange ) );

			//RegisterItemTypeByPropertyType( typeof( List<> ), typeof( HCItemCollection ) );
			//RegisterItemTypeByPropertyType( typeof( ReferenceList<> ), typeof( HCItemCollection ) );

			//!!!!
			RegisterItemTypeByPropertyType( typeof( Component_ProjectSettings.RibbonAndToolbarActionsClass ), typeof( HCItemProjectRibbonAndToolbarActions ) );
			RegisterItemTypeByPropertyType( typeof( Component_ProjectSettings.ShortcutSettingsClass ), typeof( HCItemProjectShortcuts ) );
		}

		Type GetSuitableItemTypeByPropertyType( Type propertyType )
		{
			//collection
			if( typeof( ICollection ).IsAssignableFrom( propertyType ) )
				return typeof( HCItemCollection );
			if( propertyType.IsGenericType && typeof( ReferenceList<> ).IsAssignableFrom( propertyType.GetGenericTypeDefinition() ) )
				return typeof( HCItemCollection );

			Type type;
			if( propertyType.IsGenericType )
				type = propertyType.GetGenericTypeDefinition();
			else
				type = propertyType;

			var current = type;

			do
			{
				if( current == typeof( object ) )
				{
					if( typeof( ICanParseFromAndConvertToString ).IsAssignableFrom( type ) )
						return typeof( HCItemTextBox );
				}

				itemTypeByPropertyType.TryGetValue( current, out Type type2 );
				if( type2 != null )
					return type2;

				current = current.BaseType;
			} while( current != null );

			return null;
		}

		public delegate void GetSuitableItemTypeOverrideDelegate( HierarchicalContainer sender, Metadata.Member member, ref Type itemType );
		public event GetSuitableItemTypeOverrideDelegate GetSuitableItemTypeOverride;

		public virtual Type GetSuitableItemType( Metadata.Member member )
		{
			Type itemType = null;

			//override item type
			GetSuitableItemTypeOverride?.Invoke( this, member, ref itemType );

			if( itemType == null )
			{
				var editorAttr = (EditorAttribute)member.GetCustomAttributes( typeof( EditorAttribute ), true ).FirstOrDefault();
				if( editorAttr != null )
				{
					var editorType = EditorUtility.GetTypeByName( editorAttr.EditorTypeName );
					if( typeof( HCItemProperty ).IsAssignableFrom( editorType ) )
						return editorType;
				}

				var property = member as Metadata.Property;
				if( property != null )
				{
					//var attribs = property.GetCustomAttributes( typeof( SelectBaseAttribute ), true );
					//if( attribs.Length != 0 )
					//{
					//	//get item type by SelectBaseAttribute
					//	var attrib = (SelectBaseAttribute)attribs[ 0 ];
					//	itemType = attrib.GetHierarchicalContainerItemType();
					//}
					//else
					//{
					//get item type by property type
					var originalType = property.Type.GetNetType();
					var unrefType = ReferenceUtility.GetUnreferencedType( originalType );
					itemType = GetSuitableItemTypeByPropertyType( unrefType );
					//}
				}

				var _event = member as Metadata.Event;
				if( _event != null )
					return EditorAssemblyInterface.Instance.GetTypeByName( "NeoAxis.Editor.HCItemEvent" );// typeof( HCItemEvent );
			}

			return itemType;
		}

		[DllImport( "user32.dll" )]
		private static extern IntPtr WindowFromPoint( Point pnt );

		bool IsChildOrThis( Control control )
		{
			var p = control;
			while( p != null )
			{
				if( p == this )
					return true;
				p = p.Parent;
			}
			return false;
		}

		bool IMessageFilter.PreFilterMessage( ref Message m )
		{
			if( !Enabled || !Visible || IsDisposed )
				return false;

			if( m.Msg == PI.WM_MOUSEWHEEL )
			{
				//don't filter when scroll bar is not visible
				if( !engineScrollBar1.Visible )
					return false;
				//if( VerticalScroll == null || !VerticalScroll.Visible )
				//	return false;

				Point pos = new Point( PI.LOWORD( (int)m.LParam ), PI.HIWORD( (int)m.LParam ) );

				try //crashed once on ObjectsWindow. access to disposed object assertion.
				{
					Control foundControl = GetChildAtPoint( PointToClient( pos ) );

					if( foundControl == null )
						return false;
					if( foundControl == engineScrollBar1 )
						return false;

					var windowOverCursor = WindowFromPoint( pos );
					if( windowOverCursor != IntPtr.Zero )
					{
						var controlOverCursor = FromHandle( windowOverCursor );
						if( !IsChildOrThis( controlOverCursor ) )
							return false;
					}

					//!!!!было
					//if( !( foundControl is IHCProperty ) )
					//	return false;

					if( dropDownHolder != null && dropDownHolder.Visible )
						return false;

					if( IsDropDownListAtPoint( pos ) )
						return false;

					int delta = (short)( ( ( (long)m.WParam ) >> 0x10 ) & 0xffff );
					int scrollStep = delta / 120;//System.Windows.Input.Mouse.MouseWheelDeltaForOneLine;
					var newValue = engineScrollBar1.Value - 100 * scrollStep;
					engineScrollBar1.Value = newValue;

					//// send message to HierarchicalContainer, and ignore other controls
					//PI.SendMessage( this.Handle, m.Msg, m.WParam, m.LParam );

					return true;
				}
				catch { }
			}

			return false;
		}

		//// use WS_EX_COMPOSITED to avoid flickering e.g. at splitter drag
		//private static void SetFormLevelDoubleBuffering( ScrollableControl control, bool enable )
		//{
		//	// stop scrollbar animation to avoid glitch.
		//	if( enable && ( control.HorizontalScroll.Visible || control.VerticalScroll.Visible ) )
		//	{
		//		// https://stackoverflow.com/questions/562029/stopping-scroll-bar-fade-in-vista-net-or-winapi
		//		if( PI.IsThemeActive() )
		//			PI.SetWindowTheme( control.Handle, null, null );
		//	}

		//	//// Activate double buffering at the form level.  All child controls will be double buffered as well.

		//	//int windowStyle = PI.GetWindowLong( control.Handle, PI.GWL_EXSTYLE );
		//	//if( enable )
		//	//	PI.SetWindowLong( control.Handle, PI.GWL_EXSTYLE, (IntPtr)( windowStyle | PI.WS_EX_COMPOSITED ) );
		//	//else
		//	//	PI.SetWindowLong( control.Handle, PI.GWL_EXSTYLE, (IntPtr)( windowStyle & ~PI.WS_EX_COMPOSITED ) );
		//}

		private bool IsDropDownListAtPoint( Point pos )
		{
			IntPtr hWnd = PI.WindowFromPoint( new PI.POINT( pos ) );
			if( hWnd != IntPtr.Zero )
			{
				string className = PI.GetClassName( hWnd );
				if( className == "ComboLBox" )
					return true;
			}

			return false;
		}

		[DefaultValue( false )]
		public bool ReverseGroups { get; set; }

		//[DefaultValue( false )]
		//public bool ReverseItems { get; set; }

		public void PerformOverridePropertyDisplayName( HCItemProperty property, ref string displayName )
		{
			OverridePropertyDisplayName?.Invoke( this, property, ref displayName );
		}

		public void PerformOverrideMemberDescription( HCItemMember member, ref string description )
		{
			OverrideMemberDescription?.Invoke( this, member, ref description );
		}

		public void PerformOverridePropertyEnumItem( HCItemEnumDropDown property, ref string displayName, ref string description )
		{
			OverridePropertyEnumItem?.Invoke( this, property, ref displayName, ref description );
		}

		public void PerformOverrideGroupDisplayName( HCItemGroup group, ref string displayName )
		{
			OverrideGroupDisplayName?.Invoke( this, group, ref displayName );
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams handleParam = base.CreateParams;
				handleParam.ExStyle |= 0x02000000;//WS_EX_COMPOSITED       
				return handleParam;
			}
		}

		[Browsable( false )]
		public Control ContentPanel
		{
			get { return panel1; }
		}

		void UpdateControls( bool needVerticalScroll )
		{
			if( needVerticalScroll )
			{
				var offset = ScrollBarPosition;
				panel1.SetBounds( 0, -offset, ClientRectangle.Right - engineScrollBar1.Size.Width, ClientRectangle.Height + offset );
			}
			else
				panel1.SetBounds( 0, 0, ClientRectangle.Right, ClientRectangle.Height );

			engineScrollBar1.SetBounds( ClientRectangle.Right - engineScrollBar1.Size.Width, 0, engineScrollBar1.Size.Width, ClientRectangle.Height );

			//engineScrollBar1.Location = new Point( ClientRectangle.Right - engineScrollBar1.Size.Width, 0 );
			//engineScrollBar1.Size = new Size( engineScrollBar1.Size.Width, ClientRectangle.Height );
		}

		[Browsable( false )]
		public int ScrollBarWidth
		{
			get { return engineScrollBar1.Width; }
		}

		[Browsable( false )]
		public int ScrollBarPosition
		{
			get { return engineScrollBar1.Value; }
		}

		public override bool AutoScroll
		{
			get { return false; }
			set { base.AutoScroll = value; }
		}
	}
}
