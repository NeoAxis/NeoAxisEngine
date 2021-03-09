// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public interface IHCProperty
	{
		Label LabelName { get; }

		void ButtonExpandInit();
		KryptonButton ButtonExpand { get; }

		void ButtonDefaultValueInit();
		KryptonButton ButtonDefaultValue { get; }

		void ButtonReferenceInit();
		KryptonButton ButtonReference { get; }

		void ButtonReferenceSetToolTip( string value );
		void LabelNameSetToolTip( string value );
		void SetToolTip( Control control, string caption );

		void ButtonTypeInit();
		KryptonButton ButtonType { get; }
		//void ButtonTypeReferenceSetToolTip( string value );

		bool ShowOnlyEditorControl { get; set; }

		//!!!!name
		Control EditorControl { get; set; }
	}

	/// <summary>
	/// Represents a property item for <see cref="HierarchicalContainer"/>.
	/// </summary>
	public abstract class HCItemProperty : HCItemMember
	{
		Metadata.Property property;
		object[] indexers;
		Metadata.Property property_Dependent;

		EUserControl createdControlInsidePropertyItemControl;
		//bool showDetails;

		bool defaultValueButtonInitialized;
		bool? buttonDefaultValueCurrentEnabled;

		bool referenceSpecifiedCached;
		bool? buttonReferenceCurrentEnabled;

		object[] savedOldValuesToRestore;
		object[] savedOldValuesToRestore_Dependent;

		bool buttonReferenceInitialized;
		bool buttonReferenceMouseLeftDownCanDragDrop;

		string description;
		string displayName;

		bool expandButtonInitialized;
		bool? expandButtonImageExpanded;

		bool buttonTypeInitialized;

		//

		public HCItemProperty( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects )
		{
			this.property = property;
			this.indexers = indexers;

			try
			{
				var attribute = property.GetCustomAttribute<UndoDependentPropertyAttribute>();
				if( attribute != null )
				{
					var type = MetadataManager.MetadataGetType( controlledObjects[ 0 ] );
					property_Dependent = type.MetadataGetMemberBySignature( "property:" + attribute.PropertyName ) as Metadata.Property;
				}
			}
			catch { }
		}

		public Metadata.Property Property
		{
			get { return property; }
		}

		public object[] Indexers
		{
			get { return indexers; }
		}

		public override Metadata.Member Member
		{
			get { return property; }
		}

		private bool IsCollectionElement()
		{
			return indexers.Length != 0;
		}

		public string DisplayName
		{
			get
			{
				if( displayName == null )
				{
					var dnAttr = Property.GetCustomAttribute<DisplayNameAttribute>( true );
					if( dnAttr != null )
					{
						displayName = dnAttr.DisplayName ?? string.Empty;
					}
					else
					{
						if( IsCollectionElement() )
							displayName = GetDisplayNameFromIndex();
						else
							displayName = TypeUtility.DisplayNameAddSpaces( Property.Name );
					}

					Owner.PerformOverridePropertyDisplayName( this, ref displayName );
				}
				return displayName;
			}
		}

		private string GetDisplayNameFromIndex()
		{
			var b = new StringBuilder();
			//b.Append( "[ " );
			for( int n = 0; n < indexers.Length; n++ )
			{
				var index = indexers[ n ];
				if( n != 0 )
					b.Append( ", " );
				b.Append( indexers[ n ].ToString() );
			}
			//b.Append( " ]" );
			return b.ToString();
		}

		public string Description
		{
			get
			{
				if( description == null )
				{
					var descrAttr = Property.GetCustomAttribute<DescriptionAttribute>( true );
					if( descrAttr != null )
						description = descrAttr.Description;

					if( description == null )
					{
						var id = XmlDocumentationFiles.GetMemberId( property );
						if( !string.IsNullOrEmpty( id ) )
							description = XmlDocumentationFiles.GetMemberSummary( id );
					}

					if( description == null )
						description = "";

					Owner.PerformOverrideMemberDescription( this, ref description );
				}
				return description;
			}
		}

		public override EUserControl CreateControlImpl()
		{
			//if( Owner.GridMode )
			var control = new HCGridProperty();
			//else
			//	control = new HCFormProperty();

			var control2 = (IHCProperty)control;

			//if( control2.ButtonExpand != null )
			//	control2.ButtonExpand.Click += ButtonExpand_Click;

			//if( control2.ButtonDefaultValue != null )
			//{
			//	control2.ButtonDefaultValue.Click += ButtonDefaultValue_Click;
			//	control2.ButtonDefaultValue.MouseUp += Control_MouseUp_ResetDefaultValue;
			//}

			//if( control2.ButtonReference != null )
			//{
			//	control2.ButtonReference.Click += ButtonReference_Click;
			//	control2.ButtonReference.MouseDown += ButtonReference_MouseDown;
			//	control2.ButtonReference.MouseMove += ButtonReference_MouseMove;
			//	control2.ButtonReference.MouseUp += ButtonReference_MouseUp;
			//}
			//if( control2.PanelPreview != null )
			//	control2.PanelPreview.Click += PanelPreview_Click;
			//if( control2.ButtonDetails != null )
			//	control2.ButtonDetails.Click += ButtonDetails_Click;

			//if( control2.ButtonType != null )
			//	control2.ButtonType.Click += ButtonType_Click;

			var child = CreateControlInsidePropertyItemControl();

			// set control height based on inner control height
			control.Height = Math.Max( control.Height, child.Height );

			createdControlInsidePropertyItemControl = child;
			ControlInsidePropertyItemControlWasCreated();
			control2.EditorControl = child;

			//!!!!так?
			//subscribe for context menu
			control.MouseUp += Control_MouseUp_ResetDefaultValue;
			if( control2.LabelName != null )
				control2.LabelName.MouseUp += Control_MouseUp_ResetDefaultValue;

			child.MouseUp += Control_MouseUp_ResetDefaultValue;

			//drag and drop for property
			control.AllowDrop = true;
			control.DragEnter += Control_DragEnter;
			control.DragOver += Control_DragOver;
			control.DragLeave += Control_DragLeave;
			control.DragDrop += Control_DragDrop;

			return control;
		}

		public abstract EUserControl CreateControlInsidePropertyItemControl();

		public EUserControl CreatedControlInsidePropertyItemControl
		{
			get { return createdControlInsidePropertyItemControl; }
		}

		public virtual void ControlInsidePropertyItemControlWasCreated() { }

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (IHCProperty)CreatedControl;
			var values = GetValues();
			if( values == null )
				return;

			if( control.LabelName != null )
			{
				if( control.LabelName.Text != DisplayName )
					control.LabelName.Text = DisplayName;
				control.LabelNameSetToolTip( !string.IsNullOrEmpty( Description ) ? Description : DisplayName );
			}

			var netType = Property.Type.GetNetType();
			bool isReferenceType = ReferenceUtility.IsReferenceType( netType );

			bool referenceSpecified = false;
			string referenceValue = "";
			for( int nValue = 0; nValue < values.Length; nValue++ )
			{
				var value = values[ nValue ];

				if( isReferenceType && value != null )
				{
					var iReference = (IReference)value;
					if( iReference.ReferenceSpecified )
					{
						referenceSpecified = true;

						if( nValue == 0 )
							referenceValue = iReference.GetByReference;
						else
						{
							if( referenceValue != iReference.GetByReference )
								referenceValue = "";
						}
					}
				}
			}
			if( referenceValue == "" )
				referenceValue = "Different references are specified (multiselection).";

			referenceSpecifiedCached = referenceSpecified;

			UpdateDefaultValueButton();
			UpdateButtonReference( isReferenceType, referenceSpecified, referenceValue );




			////update enabled control depending specified reference
			//if( control.PanelPreview != null )
			//	control.PanelPreview.Enabled = !IsReadOnlyInHierarchy();
			//if( control.ButtonDetails != null )
			//	control.ButtonDetails.Enabled = !IsReadOnlyInHierarchy();

			UpdateExpandButton();
			//UpdatePreview();
			//UpdateDetails();

			UpdateButtonType();

			//!!!!
			// splitter is used only with Grid.
			if( control is HCGridProperty gridProp )
			{
				//if( true/*gridProp.Bounds.IntersectsWith( Owner.ClientRectangle )*/ )
				//{
				gridProp.SplitterPosition = Owner.SplitterPosition;
				gridProp.UpdateLayout();
				//}
				//else
				//{

				//}
			}
		}

		void UpdateDefaultValueButton()
		{
			var control = (IHCProperty)CreatedControl;

			bool canReset = CanResetToDefaultValue() && !control.ShowOnlyEditorControl;

			if( canReset && !defaultValueButtonInitialized )
			{
				defaultValueButtonInitialized = true;
				control.ButtonDefaultValueInit();
				control.ButtonDefaultValue.Click += ButtonDefaultValue_Click;
				control.ButtonDefaultValue.MouseUp += Control_MouseUp_ResetDefaultValue;
			}

			if( control.ButtonDefaultValue != null )
			{
				if( control.ButtonDefaultValue.Enabled != canReset )
					control.ButtonDefaultValue.Enabled = canReset;
				if( control.ButtonDefaultValue.Visible != canReset )
					control.ButtonDefaultValue.Visible = canReset;

				if( buttonDefaultValueCurrentEnabled != canReset && control.ButtonDefaultValue.Visible )
				{
					buttonDefaultValueCurrentEnabled = canReset;

					if( EditorAPI.DarkTheme )
						control.ButtonDefaultValue.Values.Image = canReset ? EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 2.0 ? "DefaultValueCircle_Big_Dark" : "DefaultValueCircle3_Dark" ) : null;
					else
						control.ButtonDefaultValue.Values.Image = canReset ? EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 2.0 ? "DefaultValueCircle_Big" : "DefaultValueCircle3" ) : null;
				}
			}
		}

		void UpdateButtonReference( bool isReferenceType, bool referenceSpecified, string referenceValue )//IReference iReference )
		{
			var control = (IHCProperty)CreatedControl;

			if( isReferenceType && !buttonReferenceInitialized )
			{
				buttonReferenceInitialized = true;
				control.ButtonReferenceInit();
				control.ButtonReference.Click += ButtonReference_Click;
				control.ButtonReference.MouseDown += ButtonReference_MouseDown;
				control.ButtonReference.MouseMove += ButtonReference_MouseMove;
				control.ButtonReference.MouseUp += ButtonReference_MouseUp;
			}

			if( control.ButtonReference != null )
			{
				var visible = isReferenceType && !control.ShowOnlyEditorControl;

				if( control.ButtonReference.Visible != visible )
					control.ButtonReference.Visible = visible;
				if( control.ButtonReference.Enabled != isReferenceType )
					control.ButtonReference.Enabled = isReferenceType;

				if( isReferenceType )
				{
					if( buttonReferenceCurrentEnabled != referenceSpecified && control.ButtonReference.Visible )
					{
						buttonReferenceCurrentEnabled = referenceSpecified;

						if( EditorAPI.DarkTheme )
							control.ButtonReference.Values.Image = buttonReferenceCurrentEnabled.Value ? EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 2.0 ? "ReferenceEnabled2_Dark" : "ReferenceEnabled2_10_Dark" ) : null;
						else
							control.ButtonReference.Values.Image = buttonReferenceCurrentEnabled.Value ? EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 2.0 ? "ReferenceEnabled2" : "ReferenceEnabled2_10" ) : null;

						//control.ButtonReference.StateCommon.Back.Image = buttonReferenceCurrentEnabled.Value ? Properties.Resources.ReferenceEnabled2_10 : null;
						//control.ButtonReference.StateCommon.Back.Draw = InheritBool.True;

						//control.ButtonReference.Image = buttonReferenceCurrentEnabled ?
						//	Properties.Resources.ReferenceEnabled2_10 : Properties.Resources.ReferenceDisabled2_10;
					}
				}

				control.ButtonReferenceSetToolTip( referenceSpecified ? referenceValue : Translate( "The reference is not specified." ) );
				//control.ButtonReferenceSetToolTip( referenceSpecified ? iReference.GetByReference : "The reference is not specified." );
			}
		}

		void UpdateExpandButton()
		{
			var control = (IHCProperty)CreatedControl;

			bool enable = ( CanExpand || Children.Count != 0 ) && !control.ShowOnlyEditorControl;

			if( enable && !expandButtonInitialized )
			{
				expandButtonInitialized = true;
				control.ButtonExpandInit();
				control.ButtonExpand.Click += ButtonExpand_Click;
			}

			if( control.ButtonExpand != null )
			{
				if( control.ButtonExpand.Visible != enable )
					control.ButtonExpand.Visible = enable;
				if( control.ButtonExpand.Enabled != enable )
					control.ButtonExpand.Enabled = enable;

				if( expandButtonImageExpanded != Expanded && control.ButtonExpand.Visible )
				{
					expandButtonImageExpanded = Expanded;

					if( EditorAPI.DarkTheme )
					{
						if( Expanded )
							control.ButtonExpand.Values.Image = EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 1.5 ? "Minus_Big_Dark" : "Minus_small_Dark" );
						else
							control.ButtonExpand.Values.Image = EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 1.5 ? "Plus_Big_Dark" : "Plus_small3_Dark" );
					}
					else
					{
						if( Expanded )
							control.ButtonExpand.Values.Image = EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 1.5 ? "Minus_Big" : "Minus_small" );
						else
							control.ButtonExpand.Values.Image = EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 1.5 ? "Plus_Big" : "Plus_small3" );
					}
				}
			}
		}

		void UpdateButtonType()
		{
			var control = (IHCProperty)CreatedControl;

			var unrefType = property.TypeUnreferenced.GetNetType();
			var enable = !ReferenceSpecifiedCached;
			//!!!!new
			//!typeof( ReferenceValueType_Resource ).IsAssignableFrom( unrefType )
			var visible = !property.ReadOnly && unrefType.IsClass && !unrefType.IsArray && !typeof( Component ).IsAssignableFrom( unrefType ) &&
				!unrefType.IsSealed && !typeof( Metadata.TypeInfo ).IsAssignableFrom( unrefType ) && !typeof( ReferenceValueType_Resource ).IsAssignableFrom( unrefType ) && !typeof( ReferenceValueType_Member ).IsAssignableFrom( unrefType ) && unrefType != typeof( object );

			//List<>
			if( visible && unrefType.IsGenericType && unrefType.GetGenericTypeDefinition() == typeof( List<> ) )
				visible = false;

			if( visible && !buttonTypeInitialized )
			{
				buttonTypeInitialized = true;
				control.ButtonTypeInit();
				control.ButtonType.Click += ButtonType_Click;
			}

			if( control.ButtonType != null )
			{
				if( control.ButtonType.Enabled != enable )
					control.ButtonType.Enabled = enable;
				if( control.ButtonType.Visible != visible )
					control.ButtonType.Visible = visible;
			}
		}

		private void ButtonExpand_Click( object sender, EventArgs e )
		{
			Expanded = !Expanded;
		}

		//public bool ShowDetails
		//{
		//	get { return showDetails; }
		//	set
		//	{
		//		if( showDetails == value )
		//			return;
		//		showDetails = value;

		//		UpdateDetails();
		//	}
		//}

		//void UpdateDetails()
		//{
		//	var control = (IHCProperty)CreatedControl;
		//	if( control != null && control.ButtonDetails != null )
		//		control.ButtonDetails.Visible = ShowDetails;
		//}

		//protected virtual void OnPreviewClick()
		//{
		//}

		//private void PanelPreview_Click( object sender, EventArgs e )
		//{
		//	OnPreviewClick();
		//}

		//protected virtual void OnDetailsClick()
		//{
		//}

		//private void ButtonDetails_Click( object sender, EventArgs e )
		//{
		//	OnDetailsClick();
		//}

		public object[] GetValues()
		{
			//!!!!try, catch - anti crash. срабатывает на undo, если был фокус на элементе в списке. TextBox1_LostFocus. хорошо бы без этого.
			try
			{
				object[] result = new object[ ControlledObjects.Length ];
				for( int n = 0; n < result.Length; n++ )
					result[ n ] = property.GetValue( ControlledObjects[ n ], indexers );
				return result;
			}
			catch
			{
				return null;
			}
		}

		public object[] GetValues_Dependent()
		{
			if( property_Dependent != null )
			{
				try
				{
					object[] result = new object[ ControlledObjects.Length ];
					for( int n = 0; n < result.Length; n++ )
						result[ n ] = property_Dependent.GetValue( ControlledObjects[ n ], new object[ 0 ] );
					return result;
				}
				catch
				{
					return null;
				}
			}
			return null;
		}

		public object[] SavedOldValuesToRestore
		{
			get { return savedOldValuesToRestore; }
		}

		public object[] SavedOldValuesToRestore_Dependent
		{
			get { return savedOldValuesToRestore_Dependent; }
		}

		public void SaveValuesToRestore()
		{
			savedOldValuesToRestore = GetValues();
			savedOldValuesToRestore_Dependent = GetValues_Dependent();
		}

		//!!!!try, catch
		public void RestoreSavedOldValues()
		{
			for( int n = 0; n < ControlledObjects.Length; n++ )
			{
				if( property_Dependent != null )
					property_Dependent.SetValue( ControlledObjects[ n ], savedOldValuesToRestore_Dependent[ n ], new object[ 0 ] );

				property.SetValue( ControlledObjects[ n ], savedOldValuesToRestore[ n ], indexers );
			}
		}

		public HCItemProperty GetItemInHierarchyToRestoreValues()
		{
			//if parent is value type or property is read only, then need update parent value. add undo to parent.
			bool updateParentProperty = false;
			HCItemProperty parentItemProperty = null;
			if( Parent != null )
			{
				parentItemProperty = Parent as HCItemProperty;
				if( parentItemProperty != null )
				{
					var parentNetType = parentItemProperty.Property.Type.GetNetType();
					var parentUnrefType = ReferenceUtility.GetUnreferencedType( parentNetType );

					if( parentUnrefType.IsValueType )
						updateParentProperty = true;
					if( Property.ReadOnly )
						updateParentProperty = true;
				}
			}

			//update parent
			if( updateParentProperty )
				return parentItemProperty.GetItemInHierarchyToRestoreValues();

			return this;
		}

		public void AddUndoActionWithSavedValuesToRestore()
		{
			if( SavedOldValuesToRestore == null )
				return;

			if( Owner.DocumentWindow != null )
			{
				DocumentInstance document = Owner.DocumentWindow.Document;
				if( document != null )
				{
					var undoItems = new List<UndoActionPropertiesChange.Item>();
					for( int n = 0; n < ControlledObjects.Length; n++ )
					{
						if( property_Dependent != null )
						{
							var undoItem2 = new UndoActionPropertiesChange.Item( ControlledObjects[ n ], property_Dependent, SavedOldValuesToRestore_Dependent[ n ], new object[ 0 ] );
							undoItems.Add( undoItem2 );
						}

						var undoItem = new UndoActionPropertiesChange.Item( ControlledObjects[ n ], property, SavedOldValuesToRestore[ n ], indexers );
						undoItems.Add( undoItem );
					}
					if( undoItems.Count != 0 )
					{
						var action = new UndoActionPropertiesChange( undoItems.ToArray() );
						document.UndoSystem.CommitAction( action );
						document.Modified = true;
					}
				}
			}
		}

		public class PropertySetValueData
		{
			public HCItemProperty itemProperty;
			public object unrefValue;
			public bool addUndo;
			public object value;
			public HCItemProperty parentItemProperty;
			public bool updateParentProperty;

			public bool setValueHandled;
			public bool addUndoHandled;
			public bool updateParentPropertyHandled;
		}

		public void SetValue( object value, bool addUndo )
		//public void SetValue( object unrefValue, bool addUndo )//, bool addUndo = true, object[] undoUseOldValues = null )
		{
			var data = new PropertySetValueData();
			data.itemProperty = this;
			data.unrefValue = ReferenceUtility.GetUnreferencedValue( value );
			//data.unrefValue = unrefValue;
			data.addUndo = addUndo;

			//!!!!try, catch?

			var netType = Property.Type.GetNetType();
			var unrefType = ReferenceUtility.GetUnreferencedType( netType );

			//!!!!unrefValue
			//if( unrefValue != null && unrefValue.

			if( ReferenceUtility.IsReferenceType( netType ) )
			{
				if( value != null && value.GetType() == netType )
					data.value = value;//for DefaultValueAttribute when reference specified
				else
					data.value = ReferenceUtility.MakeReference( ReferenceUtility.GetUnreferencedType( netType ), value, "" );
			}
			else
				data.value = value;

			//if parent is value type or property is read only, then need update parent value. add undo to parent.
			if( Parent != null )
			{
				data.parentItemProperty = Parent as HCItemProperty;
				if( data.parentItemProperty != null )
				{
					var parentNetType = data.parentItemProperty.Property.Type.GetNetType();
					var parentUnrefType = ReferenceUtility.GetUnreferencedType( parentNetType );

					if( parentUnrefType.IsValueType )
						data.updateParentProperty = true;
					if( Property.ReadOnly )
						data.updateParentProperty = true;
				}
			}

			//!!!!try, catch

			object[] oldValues = null;
			object[] oldValues_Dependent = null;
			//if( addUndo )
			//{
			//	if( undoUseOldValues != null )
			//		oldValues = undoUseOldValues;
			//	else
			//	{
			if( addUndo && Owner.DocumentWindow != null && !data.updateParentProperty )
			{
				if( property_Dependent != null )
				{
					oldValues_Dependent = new object[ ControlledObjects.Length ];
					for( int n = 0; n < ControlledObjects.Length; n++ )
						oldValues_Dependent[ n ] = property_Dependent.GetValue( ControlledObjects[ n ], new object[ 0 ] );
				}

				oldValues = new object[ ControlledObjects.Length ];
				for( int n = 0; n < ControlledObjects.Length; n++ )
					oldValues[ n ] = property.GetValue( ControlledObjects[ n ], indexers );
			}
			//	}
			//}

			HCExtensions.PerformOverridePropertySetValue( data );

			//set value
			if( !data.setValueHandled && !Property.ReadOnly )//!!!!for updateParentProperty = true
			{
				for( int n = 0; n < ControlledObjects.Length; n++ )
					property.SetValue( ControlledObjects[ n ], data.value, indexers );
			}

			//undo
			if( !data.addUndoHandled && addUndo && Owner.DocumentWindow != null && !data.updateParentProperty )
			{
				DocumentInstance document = Owner.DocumentWindow.Document;
				if( document != null )
				{
					var undoItems = new List<UndoActionPropertiesChange.Item>();
					for( int n = 0; n < ControlledObjects.Length; n++ )
					{
						//!!!!check
						if( property_Dependent != null )
						{
							var undoItem2 = new UndoActionPropertiesChange.Item( ControlledObjects[ n ], property_Dependent, oldValues_Dependent[ n ], new object[ 0 ] );
							undoItems.Add( undoItem2 );
						}

						var undoItem = new UndoActionPropertiesChange.Item( ControlledObjects[ n ], property, oldValues[ n ], indexers );
						undoItems.Add( undoItem );
					}
					if( undoItems.Count != 0 )
					{
						var action = new UndoActionPropertiesChange( undoItems.ToArray() );
						document.UndoSystem.CommitAction( action );
						document.Modified = true;
					}
				}
			}

			//update parent
			if( !data.updateParentPropertyHandled && data.updateParentProperty )
			{
				//!!!!multiselection

				data.parentItemProperty.SetValue( ControlledObjects[ 0 ], addUndo );
			}
		}

		//public void SetValue_Unreferenced( object unrefValue )//, bool addUndo = true, object[] undoUseOldValues = null )
		//{
		//	var netType = Property.Type.GetNetType();
		//	var unrefType = ReferenceUtils.GetUnreferencedType( netType );

		//	object value = null;
		//	if( ReferenceUtils.IsReferenceType( netType ) )
		//		value = ReferenceUtils.CreateReference( ReferenceUtils.GetUnreferencedType( netType ), unrefValue, "" );
		//	else
		//		value = unrefValue;

		//	for( int n = 0; n < ControlledObjects.Length; n++ )
		//	{
		//	}
		//}

		Component[] GetControlledComponents()//GetControlledObjectsAsComponentArray()
		{
			Component[] result = new Component[ ControlledObjects.Length ];
			for( int n = 0; n < result.Length; n++ )
			{
				var referenceList = ControlledObjects[ n ] as IReferenceList;
				if( referenceList != null )
					result[ n ] = referenceList.Owner;
				else
					result[ n ] = (Component)ControlledObjects[ n ];
			}
			return result;
		}

		private void ButtonReference_Click( object sender, EventArgs e )
		{
			EditorAPI.OpenSetReferenceWindow( Owner.DocumentWindow, GetControlledComponents(), ControlledObjects, property, indexers );
			//EditorForm.Instance.ShowSetReferenceWindow( Owner.DocumentWindow, GetControlledObjectsAsComponentArray(), property, indexers );
		}

		private void ButtonReference_MouseDown( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
				buttonReferenceMouseLeftDownCanDragDrop = true;
		}

		private void ButtonReference_MouseMove( object sender, MouseEventArgs e )
		{
			var control = (IHCProperty)CreatedControl;

			if( buttonReferenceMouseLeftDownCanDragDrop )
			{
				var button = control.ButtonReference;
				if( !button.ClientRectangle.Contains( button.PointToClient( Control.MousePosition ) ) )
				{
					buttonReferenceMouseLeftDownCanDragDrop = false;

					var data = new DragDropSetReferenceData();
					data.document = Owner.DocumentWindow.Document;
					data.controlledComponents = GetControlledComponents();
					data.propertyOwners = ControlledObjects;
					//data.controlledObjects = GetControlledObjectsAsComponentArray();
					data.property = property;
					data.indexers = indexers;
					button.DoDragDrop( data, DragDropEffects.Link );
				}
			}
		}

		string Translate( string text )
		{
			return EditorLocalization.Translate( "SettingsWindow", text );
		}

		private void ButtonReference_MouseUp( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
				buttonReferenceMouseLeftDownCanDragDrop = false;

			if( e.Button == MouseButtons.Right )
			{
				var control = (IHCProperty)CreatedControl;

				var items = new List<KryptonContextMenuItemBase>();

				//Reset Reference
				{
					var item = new KryptonContextMenuItem( Translate( "Reset Reference" ), null, delegate ( object s, EventArgs e2 )
					{
						if( referenceSpecifiedCached )
						{
							var values = GetValues();
							if( values != null )
							{
								var value = values[ 0 ];
								var unrefValue = ReferenceUtility.GetUnreferencedValue( value );
								//Component specific: set null for Component
								if( unrefValue as Component != null )
									unrefValue = null;

								SetValue( unrefValue, true );
							}
						}
					} );
					item.Enabled = referenceSpecifiedCached;
					items.Add( item );
				}

				items.Add( new KryptonContextMenuSeparator() );

				//Find In Resources
				{
					var item = new KryptonContextMenuItem( Translate( "Find Resource" ), null, delegate ( object s, EventArgs e2 )
					{
						if( referenceSpecifiedCached )
						{
							var values = GetValues();
							if( values != null )
							{
								var value = values[ 0 ];

								//!!!!по сути нужно ссылку открыть, не объект

								var unrefValue = ReferenceUtility.GetUnreferencedValue( value );
								if( unrefValue != null )
								{
									//select in Resources Window
									var component = unrefValue as Component;
									if( component != null )
									{
										var fileName = ComponentUtility.GetOwnedFileNameOfComponent( component );
										if( !string.IsNullOrEmpty( fileName ) )
										{
											var realFileName = VirtualPathUtility.GetRealPathByVirtual( fileName );
											EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { realFileName } );
											EditorAPI.SelectDockWindow( EditorAPI.FindWindow<ResourcesWindow>() );
										}
									}
								}
							}
						}
					} );

					item.Enabled = false;
					if( referenceSpecifiedCached )
					{
						var values = GetValues();
						if( values != null )
						{
							var value = values[ 0 ];

							var unrefValue = ReferenceUtility.GetUnreferencedValue( value );
							if( unrefValue != null )
							{
								//select in Resources Window
								var component = unrefValue as Component;
								if( component != null )
								{
									var fileName = ComponentUtility.GetOwnedFileNameOfComponent( component );
									if( !string.IsNullOrEmpty( fileName ) )
										item.Enabled = true;
								}

								//if( EditorAPI.IsDocumentObjectSupport( unrefValue ) )
								//item.Enabled = true;
							}
						}
					}
					//item.Enabled = referenceSpecifiedCached;
					items.Add( item );
				}

				//Open By Reference
				{
					var item = new KryptonContextMenuItem( Translate( "Open By Reference" ), null, delegate ( object s, EventArgs e2 )
					{
						if( referenceSpecifiedCached )
						{
							var values = GetValues();
							if( values != null )
							{
								var value = values[ 0 ];

								//!!!!по сути нужно ссылку открыть, не объект

								var unrefValue = ReferenceUtility.GetUnreferencedValue( value );
								if( unrefValue != null )
								{
									var document = EditorAPI.GetDocumentByObject( unrefValue );
									if( EditorAPI.IsDocumentObjectSupport( unrefValue ) )
										EditorAPI.OpenDocumentWindowForObject( document, unrefValue );
									else
									{
										//!!!!открыть файл если корневой в файле
									}

									//select in Resources Window
									{
										var component = unrefValue as Component;
										if( component != null )
										{
											var fileName = ComponentUtility.GetOwnedFileNameOfComponent( component );
											if( !string.IsNullOrEmpty( fileName ) )
											{
												var realFileName = VirtualPathUtility.GetRealPathByVirtual( fileName );
												EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { realFileName } );
												EditorAPI.SelectDockWindow( EditorAPI.FindWindow<ResourcesWindow>() );
											}
										}
									}
								}
							}
						}
					} );

					item.Enabled = false;
					if( referenceSpecifiedCached )
					{
						var values = GetValues();
						if( values != null )
						{
							var value = values[ 0 ];

							var unrefValue = ReferenceUtility.GetUnreferencedValue( value );
							if( unrefValue != null )
							{
								//if( EditorAPI.IsDocumentObjectSupport( unrefValue ) )
								item.Enabled = true;
							}
						}
					}
					//item.Enabled = referenceSpecifiedCached;
					items.Add( item );
				}

				EditorContextMenuWinForms.Show( items, CreatedControl );
			}
		}

		protected virtual void GetExpandablePropertiesFilter( Metadata.Property property, ref bool skip )
		{
		}

		public static bool IsOneDimensionArray( Type type )
		{
			return type.IsArray && type.GetArrayRank() == 1;
		}

		public static bool IsListType( Type type )
		{
			return type.IsGenericType &&
				( typeof( List<> ).IsAssignableFrom( type.GetGenericTypeDefinition() ) || typeof( ReferenceList<> ).IsAssignableFrom( type.GetGenericTypeDefinition() ) );
			//return typeof( IList ).IsAssignableFrom( type ) &&
			//	type.IsGenericType &&
			//	typeof( List<> ).IsAssignableFrom( type.GetGenericTypeDefinition() );
			//type.GetGenericTypeDefinition().IsAssignableFrom( typeof( List<> ) );
		}

		public override void Update()
		{
			//update expandable properties

			ESet<HierarchicalContainer.Item> newItems = new ESet<HierarchicalContainer.Item>();

			var values = GetValues();
			if( values == null )
				return;
			object[] unrefValues = new object[ values.Length ];
			for( int n = 0; n < values.Length; n++ )
			{
				if( values[ n ] != null )
					unrefValues[ n ] = ReferenceUtility.GetUnreferencedValue( values[ n ] );
			}

			//!!!!multiselection
			var unrefValue = unrefValues[ 0 ];

			if( unrefValue != null )
			{
				var unrefValueType = MetadataManager.MetadataGetType( unrefValue );

				//expandable HCExpandableAttribute
				var attribs = unrefValueType.GetCustomAttributes( typeof( HCExpandableAttribute ) );
				if( attribs.Length != 0 )
				{
					CanExpand = true;

					if( WasExpanded )//!!!!может проверять уже когда точно понятно что есть вложенные
					{
						Dictionary<Metadata.Property, HCItemProperty> itemByProperty = new Dictionary<Metadata.Property, HCItemProperty>();
						foreach( var item in Children )
						{
							HCItemProperty propertyItem = item as HCItemProperty;
							if( propertyItem != null )
								itemByProperty.Add( propertyItem.Property, propertyItem );
						}

						List<Metadata.Property> properties = new List<Metadata.Property>();
						foreach( var member in MetadataManager.MetadataGetMembers( unrefValue ) )
						{
							var property = member as Metadata.Property;

							//!!!!везде одинакого проверять
							if( property != null && property.Browsable && !property.HasIndexers && !property.Static )//!!!!&& !property.ReadOnly )
							{
								bool skip = false;
								GetExpandablePropertiesFilter( property, ref skip );
								if( !skip )
									properties.Add( property );
							}
						}

						foreach( var property in properties )
						{
							if( !itemByProperty.TryGetValue( property, out HCItemProperty item ) )
							{
								Type itemType = Owner.GetSuitableItemType( property );
								//var originalType = property.Type.GetNetType();
								//var unrefType = ReferenceUtils.GetUnreferencedType( originalType );
								//Type itemType = HCPropertyItemTypes.GetSuitableType( unrefType );

								var constructor = itemType.GetConstructor( new Type[] {
									typeof( HierarchicalContainer ),
									typeof( HierarchicalContainer.Item ),
									typeof( object[] ),
									typeof( Metadata.Property ),
									typeof( object[] )
								} );

								//!!!!multiselection

								//!!!!unrefValues check for nulls

								item = (HCItemProperty)constructor.Invoke( new object[] { Owner, this, unrefValues, property, property.Indexers } );
							}
							else
							{
								//update ControlledObjects

								item.ControlledObjects = unrefValues;
							}

							newItems.Add( item );
						}
					}
				}

				//Array (one dimension)
				if( IsOneDimensionArray( unrefValueType.GetNetType() ) )
				{
					CanExpand = true;

					if( WasExpanded )//!!!!может проверять уже когда точно понятно что есть вложенные
					{
						var elementType = unrefValueType.GetNetType().GetElementType();
						int length = (int)unrefValue.GetType().GetProperty( "Length" ).GetValue( unrefValue, null );

						//!!!!тип получать из итема, если это класс. для HCExpandableAttribute тоже?
						var interfaceNet = unrefValueType.GetNetType().GetInterface( "IList`1" );
						var _interface = MetadataManager.GetTypeOfNetType( interfaceNet );
						var itemProperty = (Metadata.Property)_interface.MetadataGetMemberBySignature( "property:Item[System.Int32]" );

						HCItemProperty itemPropertyLength = null;
						Dictionary<int, HCItemProperty> itemByIndex = new Dictionary<int, HCItemProperty>();
						foreach( var item in Children )
						{
							HCItemProperty propertyItem = item as HCItemProperty;
							if( propertyItem != null )
							{
								if( propertyItem.Property.Name == "Length" )
									itemPropertyLength = propertyItem;
								else if( propertyItem.Property.Name == "Item" )
								{
									int index = (int)propertyItem.indexers[ 0 ];
									itemByIndex.Add( index, propertyItem );
								}
							}
						}

						//Count property
						{
							HCItemProperty item = itemPropertyLength;

							if( itemPropertyLength == null )
							{
								var property = (Metadata.Property)unrefValueType.MetadataGetMemberBySignature( "property:Length" );

								Type itemType = Owner.GetSuitableItemType( property );

								var constructor = itemType.GetConstructor( new Type[] {
									typeof( HierarchicalContainer ),
									typeof( HierarchicalContainer.Item ),
									typeof( object[] ),
									typeof( Metadata.Property ),
									typeof( object[] )
								} );

								//!!!!unrefValues check for nulls

								item = (HCItemProperty)constructor.Invoke( new object[] { Owner, this, unrefValues, property, property.Indexers } );
							}
							else
							{
								//update ControlledObjects
								item.ControlledObjects = unrefValues;
							}

							newItems.Add( item );
						}

						//Item properties
						if( length <= ProjectSettings.Get.PropertiesMaxCountCollectionItemsToDisplay )
						{
							for( int n = 0; n < length; n++ )
							{
								if( !itemByIndex.TryGetValue( n, out HCItemProperty item ) )
								{
									Type itemType = Owner.GetSuitableItemType( itemProperty );

									var constructor = itemType.GetConstructor( new Type[] {
										typeof( HierarchicalContainer ),
										typeof( HierarchicalContainer.Item ),
										typeof( object[] ),
										typeof( Metadata.Property ),
										typeof( object[] )
									} );

									//!!!!unrefValues check for nulls

									item = (HCItemProperty)constructor.Invoke( new object[] { Owner, this, unrefValues, itemProperty, new object[] { n } } );
								}
								else
								{
									//update ControlledObjects
									item.ControlledObjects = unrefValues;
								}

								newItems.Add( item );
							}
						}
					}
				}

				//List, ReferenceList
				if( IsListType( unrefValueType.GetNetType() ) )
				{
					CanExpand = true;

					if( WasExpanded )//!!!!может проверять уже когда точно понятно что есть вложенные
					{
						//var elementType = unrefValueType.GetNetType().GetGenericArguments()[ 0 ];
						int count = (int)unrefValue.GetType().GetProperty( "Count" ).GetValue( unrefValue, null );

						//!!!!тип получать из итема, если это класс. для HCExpandableAttribute тоже?
						var itemProperty = (Metadata.Property)unrefValueType.MetadataGetMemberBySignature( "property:Item[System.Int32]" );

						HCItemProperty itemPropertyCount = null;
						Dictionary<int, HCItemProperty> itemByIndex = new Dictionary<int, HCItemProperty>();
						foreach( var item in Children )
						{
							HCItemProperty propertyItem = item as HCItemProperty;
							if( propertyItem != null )
							{
								if( propertyItem.Property.Name == "Count" )
									itemPropertyCount = propertyItem;
								else if( propertyItem.Property.Name == "Item" )
								{
									int index = (int)propertyItem.indexers[ 0 ];
									itemByIndex.Add( index, propertyItem );
								}
							}
						}

						//Count property
						{
							HCItemProperty item = itemPropertyCount;

							if( itemPropertyCount == null )
							{
								var property = (Metadata.Property)unrefValueType.MetadataGetMemberBySignature( "property:Count" );

								Type itemType = Owner.GetSuitableItemType( property );

								var constructor = itemType.GetConstructor( new Type[] {
									typeof( HierarchicalContainer ),
									typeof( HierarchicalContainer.Item ),
									typeof( object[] ),
									typeof( Metadata.Property ),
									typeof( object[] )
								} );

								//!!!!unrefValues check for nulls

								item = (HCItemProperty)constructor.Invoke( new object[] { Owner, this, unrefValues, property, property.Indexers } );
							}
							else
							{
								//update ControlledObjects
								item.ControlledObjects = unrefValues;
							}

							newItems.Add( item );
						}

						//Item properties
						if( count <= ProjectSettings.Get.PropertiesMaxCountCollectionItemsToDisplay )
						{
							for( int n = 0; n < count; n++ )
							{
								if( !itemByIndex.TryGetValue( n, out HCItemProperty item ) )
								{
									Type itemType = Owner.GetSuitableItemType( itemProperty );

									var constructor = itemType.GetConstructor( new Type[] {
										typeof( HierarchicalContainer ),
										typeof( HierarchicalContainer.Item ),
										typeof( object[] ),
										typeof( Metadata.Property ),
										typeof( object[] )
									} );

									//!!!!unrefValues check for nulls

									item = (HCItemProperty)constructor.Invoke( new object[] { Owner, this, unrefValues, itemProperty, new object[] { n } } );
								}
								else
								{
									//update ControlledObjects
									item.ControlledObjects = unrefValues;
								}

								newItems.Add( item );
							}
						}
					}
				}
			}

			//prepare list of items for deletion
			List<HierarchicalContainer.Item> itemsToDelete = new List<HierarchicalContainer.Item>();
			foreach( var item in Children )
			{
				if( !newItems.Contains( item ) )
					itemsToDelete.Add( item );
			}

			//replace rootItems list
			Children.Clear();
			Children.AddRange( newItems );

			//delete old items
			foreach( var item in itemsToDelete )
				item.Dispose();

			//!!!!new, было вверху. падало на коллекциях.
			base.Update();
		}

		bool IsReadOnlyProperty()
		{
			bool? readOnly = null;
			HCExtensions.PerformOverridePropertyReadOnly( this, ref readOnly );
			if( readOnly.HasValue )
				return readOnly.Value;

			return Property.ReadOnly;
		}

		public bool IsReferenceSpecifiedInHierarchy()
		{
			if( referenceSpecifiedCached )
				return true;

			var parentProperty = Parent as HCItemProperty;
			if( parentProperty != null && parentProperty.IsReferenceSpecifiedInHierarchy() )
				return true;

			return false;
		}

		public bool CanEditValue()
		{
			if( IsReadOnlyProperty() )
				return false;
			if( IsReferenceSpecifiedInHierarchy() )
				return false;

			return true;
		}

		void GetPropertyDefaultValue( out bool specified, out object value )
		{
			var c = GetOneControlledObject<Component>();
			if( c != null )
			{
				c.BaseType.GetPropertyDefaultValue( property, out specified, out value );
				return;
			}

			specified = property.DefaultValueSpecified;
			value = property.DefaultValue;
		}

		bool CanResetToDefaultValue()
		{
			var parentProperty = Parent as HCItemProperty;
			if( parentProperty != null && parentProperty.IsReferenceSpecifiedInHierarchy() )
				return false;

			GetPropertyDefaultValue( out var defaultValueSpecified, out var defaultValue );

			if( referenceSpecifiedCached )
			{
				//!!!!new
				//can't reset when DefaultValue equal reference value
				if( defaultValueSpecified )//if( property.DefaultValueSpecified )
				{
					//var defaultValue = property.DefaultValue;
					if( defaultValue != null && ReferenceUtility.IsReferenceType( defaultValue.GetType() ) )
					{
						var values = GetValues();
						if( values != null )
						{
							foreach( var value in values )
							{
								var value2 = value as IReference;
								if( value2 != null && ( (IReference)defaultValue ).GetByReference != value2.GetByReference )
									return true;
							}
							return false;

							//var value = values[ 0 ];

							//var value2 = value as IReference;
							//if( value2 != null && ( (IReference)defaultValue ).GetByReference == value2.GetByReference )
							//	return false;
						}
					}
				}

				return true;
			}

			var unrefType = ReferenceUtility.GetUnreferencedType( property.Type.GetNetType() );

			//collections
			//!!!!все типы коллекций
			if( ( IsOneDimensionArray( unrefType ) && !property.ReadOnly ) || IsListType( unrefType ) )
			{
				var values = GetValues();
				if( values != null )
				{
					foreach( var value in values )
					{
						object unrefValue = ReferenceUtility.GetUnreferencedValue( value );
						if( unrefValue != null )
						{
							var list = unrefValue as IList;
							if( list != null && list.Count != 0 )
								return true;
						}
					}
				}
				return false;
			}

			if( IsReadOnlyProperty() )
				return false;

			//DefaultValueSpecified
			if( defaultValueSpecified )//if( property.DefaultValueSpecified )
			{
				//var defaultValue = property.DefaultValue;

				var values = GetValues();
				if( values != null )
				{
					foreach( var value in values )
					{
						object unrefValue = ReferenceUtility.GetUnreferencedValue( value );

						bool equal;
						if( defaultValue != null )
						{
							//!!!!где еще также сделать

							if( unrefValue != null )
							{
								//!!!!можно еще конвертить между разными типами. float и double и т.д.

								if( !( unrefValue is string ) && defaultValue is string )
								{
									equal = unrefValue.ToString() == (string)defaultValue;
								}
								else
								{
									if( unrefValue.GetType().IsValueType )
										equal = unrefValue.Equals( defaultValue );
									else
										equal = Equals( unrefValue, defaultValue );
								}
							}
							else
								equal = false;
						}
						else
							equal = unrefValue == null;

						if( !equal )
							return true;
					}
				}
			}

			return false;
		}

		void ResetToDefault()
		{
			if( !CanResetToDefaultValue() )
				return;

			var unrefType = ReferenceUtility.GetUnreferencedType( property.Type.GetNetType() );

			//Array
			if( IsOneDimensionArray( unrefType ) && !Property.ReadOnly )
			{
				var unrefValue = Array.CreateInstance( unrefType.GetElementType(), 0 );
				SetValue( unrefValue, true );
				return;
			}

			//List
			if( IsListType( unrefType ) )
			{
				if( !Property.ReadOnly )
				{
					var unrefValue = unrefType.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null );
					SetValue( unrefValue, true );
				}
				else
				{
					var values = GetValues();
					if( values != null )
					{
						var undoActions = new List<UndoSystem.Action>();

						foreach( var value in values )
						{
							object unrefValue = ReferenceUtility.GetUnreferencedValue( value );

							var list = (IList)unrefValue;
							if( list != null )
							{
								//remove all items with undo
								var indexes = new List<int>();
								for( int n = 0; n < list.Count; n++ )
									indexes.Add( n );
								var undoAction = new UndoActionListAddRemove( list, indexes, false );

								undoActions.Add( undoAction );
							}
						}

						if( undoActions.Count != 0 )
						{
							var document = Owner.DocumentWindow.Document;
							document.UndoSystem.CommitAction( new UndoMultiAction( undoActions ) );
							document.Modified = true;
						}
					}
				}
				return;
			}

			//!!!!если DefaultValueSpecified не установлен?
			GetPropertyDefaultValue( out var defaultValueSpecified, out var defaultValue );
			//var defaultValue = property.DefaultValue;

			try
			{
				object unrefValue = defaultValue;

				//!!!!так?

				//!!!!можно еще конвертить между разными типами. float и double и т.д.

				//convert from string
				if( defaultValue != null )
				{
					if( unrefType != typeof( string ) && defaultValue is string )
					{
						if( typeof( ICanParseFromAndConvertToString ).IsAssignableFrom( unrefType ) )
						{
							var parseMethod = unrefType.GetMethod( "Parse", BindingFlags.Public | BindingFlags.Static );
							if( parseMethod != null )
								unrefValue = parseMethod.Invoke( null, new object[] { (string)defaultValue } );
						}
						else
							unrefValue = SimpleTypes.ParseValue( unrefType, (string)defaultValue );
					}
				}

				if( unrefValue == null && unrefType.IsValueType )
					unrefValue = Activator.CreateInstance( unrefType );

				SetValue( unrefValue, true );
			}
			catch
			{
				//!!!!
			}
		}

		void ShowResetToDefaultContextMenu()
		{
			var items = new List<KryptonContextMenuItemBase>();

			//Reset To Default
			{
				//!!!!для TypeInfo потом может default value переопределяться

				//!!!!можно показывать дефолтное значение

				var item = new KryptonContextMenuItem( Translate( "Reset To Default" ), null, delegate ( object s, EventArgs e2 )
				{
					ResetToDefault();
				} );
				item.Enabled = CanResetToDefaultValue();
				items.Add( item );
			}

			EditorContextMenuWinForms.Show( items, CreatedControl );
		}

		private void ButtonDefaultValue_Click( object sender, EventArgs e )
		{
			ShowResetToDefaultContextMenu();
		}

		public void Control_MouseUp_ResetDefaultValue( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Right )
				ShowResetToDefaultContextMenu();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void Control_DragEnter( object sender, DragEventArgs e )
		{
		}

		bool DragDropSetReferenceFromReferenceButton( DragEventArgs e, bool checkOnly )
		{
			DragDropSetReferenceData dragDropData = (DragDropSetReferenceData)e.Data.GetData( typeof( DragDropSetReferenceData ) );
			if( dragDropData != null )
			{
				var components = GetControlledComponents();

				var componentsFrom = dragDropData.controlledComponents;
				var componentFrom0 = componentsFrom[ 0 ];

				//!!!!как тут с multiselection?
				var componentTo = components[ 0 ];

				if( ReferenceUtility.CanMakeReferenceToObjectWithType( dragDropData.property.TypeUnreferenced, property.TypeUnreferenced ) &&
					( componentFrom0 != componentTo || dragDropData.property != property ) )
				{
					if( componentFrom0.ParentRoot == componentTo.ParentRoot )
					{
						//reference inside same resource

						if( !checkOnly )
						{
							string[] referenceValues = new string[ dragDropData.controlledComponents.Length ];
							for( int n = 0; n < referenceValues.Length; n++ )
								referenceValues[ n ] = ReferenceUtility.CalculateThisReference( componentsFrom[ n ], componentTo, property.Name );

							dragDropData.SetProperty( referenceValues );
						}

						return true;
					}
					else
					{
						//reference to another resource

						var resourceInstance = componentTo.ParentRoot?.HierarchyController.CreatedByResource;
						if( resourceInstance != null )
						{
							if( !checkOnly )
							{
								var referenceValue = ReferenceUtility.CalculateResourceReference( componentTo, property.Name );

								string[] referenceValues = new string[ dragDropData.controlledComponents.Length ];
								for( int n = 0; n < referenceValues.Length; n++ )
									referenceValues[ n ] = referenceValue;

								dragDropData.SetProperty( referenceValues );
							}

							return true;
						}
					}
				}
			}

			return false;
		}

		bool DragDropSetReferenceFromContentBrowser( DragEventArgs e, bool checkOnly )
		{
			var dragDropData = ContentBrowser.GetDroppingItemData( e.Data );
			if( dragDropData != null )
			{
				var item = dragDropData.Item;

				var components = GetControlledComponents();

				string[] referenceValues = new string[ components.Length ];
				for( int n = 0; n < components.Length; n++ )
				{
					item.CalculateReferenceValue( components[ n ], property.TypeUnreferenced, out string referenceValue, out bool canSet );
					referenceValues[ n ] = referenceValue;
					if( !canSet )
					{
						referenceValues = null;
						break;
					}
				}

				if( referenceValues != null )
				{
					if( !checkOnly )
						EditorUtility.SetPropertyReference( Owner.DocumentWindow.Document, ControlledObjects, property, indexers, referenceValues );
					//if( !checkOnly )
					//	EditorUtils.SetPropertyReference( Owner.DocumentWindow.Document, components, property, indexers, referenceValues );

					return true;
				}
			}

			return false;
		}

		private void Control_DragOver( object sender, DragEventArgs e )
		{
			e.Effect = DragDropEffects.None;
			if( DragDropSetReferenceFromReferenceButton( e, true ) )
				e.Effect = DragDropEffects.Link;
			if( DragDropSetReferenceFromContentBrowser( e, true ) )
				e.Effect = DragDropEffects.Link;
		}

		private void Control_DragLeave( object sender, EventArgs e )
		{
		}

		private void Control_DragDrop( object sender, DragEventArgs e )
		{
			DragDropSetReferenceFromReferenceButton( e, false );
			DragDropSetReferenceFromContentBrowser( e, false );
		}

		public bool ReferenceSpecifiedCached
		{
			get { return referenceSpecifiedCached; }
		}

		private void ButtonType_Click( object sender, EventArgs e )
		{
			var initData = new SelectTypeWindow.CreationDataClass();
			initData.initDocumentWindow = Owner.DocumentWindow;
			initData.initDemandedType = property.TypeUnreferenced;
			initData.initCanSelectNull = true;

			initData.WasSelected = delegate ( SelectTypeWindow window, Metadata.TypeInfo selectedType, ref bool cancel )
			{
				object obj = null;
				if( selectedType != null )
					obj = selectedType.InvokeInstance( null );
				SetValue( obj, true );
			};

			//bool cancel2 = false;
			//initData.WasSelected.Invoke( null, initData.initDemandedType, ref cancel2 );

			EditorAPI.OpenSelectTypeWindow( initData );
		}
	}
}
