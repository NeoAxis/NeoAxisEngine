#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public interface IHCTextBoxSelect : IHCTextBox
	{
		Internal.ComponentFactory.Krypton.Toolkit.KryptonButton ButtonSelect { get; }
	}

	public class HCItemTextBoxSelect : HCItemTextBox
	{
		//bool buttonReferenceMouseLeftDownCanDragDrop;

		//

		public HCItemTextBoxSelect( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override EUserControl CreateControlInsidePropertyItemControl()
		{
			return new HCGridTextBoxSelect();
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			var control = (IHCTextBoxSelect)CreatedControlInsidePropertyItemControl;

			control.TextBox.MouseUp += Control_MouseUp_ResetDefaultValue;
			control.ButtonSelect.MouseUp += ( s, e ) => OnDropDownMouseButtonUp();
			control.ButtonSelect.MouseDown += ( s, e ) => OnDropDownMouseButtonDown();

			//if( control.ButtonSelect != null )
			//{
			//	control.ButtonSelect.Click += ButtonSelect_Click;
			//	control.ButtonSelect.MouseDown += ButtonSelect_MouseDown;
			//	control.ButtonSelect.MouseMove += ButtonSelect_MouseMove;
			//	control.ButtonSelect.MouseUp += ButtonSelect_MouseUp;
			//}
		}

		protected virtual void OnDropDownMouseButtonUp()
		{
		}

		protected virtual void OnDropDownMouseButtonDown()
		{
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (IHCTextBoxSelect)CreatedControlInsidePropertyItemControl;
			var buttonSelect = control.ButtonSelect;

			bool readOnly = !CanEditValue();
			buttonSelect.Enabled = !readOnly;

			//if( buttonSelect != null )
			//{
			//	var netType = Property.Type.GetNetType();
			//	bool isReferenceType = ReferenceUtils.IsReferenceType( netType );

			//	var values = GetValues();

			//	//!!!!multiselection
			//	var value = values[ 0 ];

			//	bool referenceSpecified = false;
			//	if( isReferenceType && value != null )
			//		referenceSpecified = ( (IReference)value ).ReferenceSpecified;

			//	if( buttonSelect.Enabled != !referenceSpecified )
			//		buttonSelect.Enabled = !referenceSpecified;
			//}
		}

		//protected virtual void ButtonSelect_Click( object sender, EventArgs e )
		//{
		//	var control = (IHCTextBoxSelect)CreatedControlInsidePropertyItemControl;

		//	var netType = Property.Type.GetNetType();
		//	bool isReferenceType = ReferenceUtils.IsReferenceType( netType );

		//	var values = GetValues();

		//	//!!!!multiselection
		//	var value = values[ 0 ];

		//	bool referenceSpecified = false;
		//	if( isReferenceType && value != null )
		//		referenceSpecified = ( (IReference)value ).ReferenceSpecified;

		//	EditorForm.Instance.ShowSetResourceWindow( Owner.DocumentWindow, ControlledObjects, Property, Indexers, referenceSpecified );
		//}

		//private void ButtonSelect_MouseDown( object sender, MouseEventArgs e )
		//{
		//	if( e.Button == MouseButtons.Left )
		//	{
		//		var netType = Property.Type.GetNetType();
		//		bool isReferenceType = ReferenceUtils.IsReferenceType( netType );

		//		var values = GetValues();

		//		//!!!!multiselection
		//		var value = values[ 0 ];

		//		bool referenceSpecified = false;
		//		if( isReferenceType && value != null )
		//			referenceSpecified = ( (IReference)value ).ReferenceSpecified;

		//		if( !referenceSpecified )
		//			buttonReferenceMouseLeftDownCanDragDrop = true;
		//	}
		//}

		//private void ButtonSelect_MouseMove( object sender, MouseEventArgs e )
		//{
		//	var control = (IHCTextBoxSelect)CreatedControlInsidePropertyItemControl;

		//	if( buttonReferenceMouseLeftDownCanDragDrop )
		//	{
		//		var button = control.ButtonSelect;
		//		if( !button.ClientRectangle.Contains( button.PointToClient( Control.MousePosition ) ) )
		//		{
		//			buttonReferenceMouseLeftDownCanDragDrop = false;

		//			var data = new DragDropSetResourceData();
		//			data.document = Owner.DocumentWindow.Document;
		//			data.controlledObjects = ControlledObjects;
		//			data.property = Property;
		//			data.indexers = Indexers;
		//			button.DoDragDrop( data, DragDropEffects.Link );
		//		}
		//	}
		//}

		//private void ButtonSelect_MouseUp( object sender, MouseEventArgs e )
		//{
		//	if( e.Button == MouseButtons.Left )
		//		buttonReferenceMouseLeftDownCanDragDrop = false;
		//}
	}
}
#endif