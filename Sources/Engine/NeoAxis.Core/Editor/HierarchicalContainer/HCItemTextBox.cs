// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public interface IHCTextBox
	{
		//Label Label1 { get; }
		EngineTextBox TextBox { get; }
	}

	/// <summary>
	/// Represents a text box item for property for <see cref="HierarchicalContainer"/>.
	/// </summary>
	public class HCItemTextBox : HCItemProperty
	{
		bool textBoxFocused;
		bool textBoxModifying;
		bool textBoxDisableTextChangedEvent;

		//object valueBeforeCommit;
		//string valueBeforeCommit;
		bool invalidValue;

		//

		public HCItemTextBox( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override EUserControl CreateControlInsidePropertyItemControl()
		{
			//if( Owner.GridMode )
			return new HCGridTextBox();
			//else
			//	return new HCFormTextBox();
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			var control = (IHCTextBox)CreatedControlInsidePropertyItemControl;
			control.TextBox.GotFocus += TextBox1_GotFocus;
			control.TextBox.LostFocus += TextBox1_LostFocus;
			control.TextBox.KeyDown += TextBox1_KeyDown;
			control.TextBox.TextChanged += TextBox1_TextChanged;
		}

		private void TextBox1_GotFocus( object sender, EventArgs e )
		{
			textBoxFocused = true;

			if( !invalidValue )
			{
				var control = (IHCTextBox)CreatedControlInsidePropertyItemControl;
				//valueBeforeCommit = ParseValueFromTextBox( out invalidValue );
				//valueBeforeCommit = control.TextBox1.Text;
			}
		}

		void TextBoxApplyChanges()
		{
			TextBoxTextChanged();

			//var control = (IHCTextBox)CreatedControlInsidePropertyItemControl;
			//if( control.TextBox1.Text != valueBeforeCommit || invalidValue )
			//{
			//	valueBeforeCommit = control.TextBox1.Text;
			//	TextBoxTextChanged();
			//}

			textBoxModifying = false;
		}

		private void TextBox1_LostFocus( object sender, EventArgs e )
		{
			textBoxFocused = false;

			if( textBoxModifying )//!!!!new
			{
				var control = (IHCTextBox)CreatedControlInsidePropertyItemControl;
				if( !control.TextBox.ReadOnly )
					TextBoxApplyChanges();
			}
		}

		private void TextBox1_KeyDown( object sender, KeyEventArgs e )
		{
			var control = (IHCTextBox)CreatedControlInsidePropertyItemControl;
			if( !control.TextBox.ReadOnly )
			{
				if( e.KeyCode == Keys.Return )
					TextBoxApplyChanges();
			}
		}

		private void TextBox1_TextChanged( object sender, EventArgs e )
		{
			if( textBoxDisableTextChangedEvent )
				return;

			if( textBoxFocused )
				textBoxModifying = true;
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (IHCTextBox)CreatedControlInsidePropertyItemControl;

			//if( control.Label1 != null )
			//	control.Label1.Text = Property.Name;

			bool readOnly = !CanEditValue();
			//bool readOnly = IsReadOnlyInHierarchy() || PropertyReadOnly;
			if( readOnly )
				invalidValue = false;

			// make read only
			if( control.TextBox.LikeLabel != readOnly )
				control.TextBox.LikeLabel = readOnly;

			//update text from property
			if( !textBoxModifying && !invalidValue )
			{
				var values = GetValues();
				if( values != null )
				{
					var stringValueResult = "";

					for( int nValue = 0; nValue < values.Length; nValue++ )
					{
						var value = values[ nValue ];
						var netType = Property.Type.GetNetType();
						var unrefValue = ReferenceUtility.GetUnreferencedValue( value );

						string stringValue;
						if( unrefValue != null )
						{
							if( unrefValue is double )
								stringValue = ( (double)unrefValue ).ToString( "0.#################" );
							else if( unrefValue is float )
								stringValue = ( (float)unrefValue ).ToString( "0.########" );
							else
								stringValue = unrefValue.ToString();
						}
						else
							stringValue = "";

						if( nValue == 0 )
							stringValueResult = stringValue;
						else
						{
							if( stringValueResult != stringValue )
								stringValueResult = "";
						}
					}

					if( control.TextBox.Text != stringValueResult )
					{
						textBoxDisableTextChangedEvent = true;
						control.TextBox.Text = stringValueResult;
						textBoxDisableTextChangedEvent = false;
					}
				}
			}

			control.TextBox.SetError( invalidValue ? "Invalid value" : "" );
		}

		object ParseValueFromTextBox( out bool invalid )
		{
			var control = (IHCTextBox)CreatedControlInsidePropertyItemControl;
			var netType = Property.Type.GetNetType();
			var unrefType = ReferenceUtility.GetUnreferencedType( netType );

			try
			{
				invalid = false;

				var text = control.TextBox.Text;

				if( typeof( ICanParseFromAndConvertToString ).IsAssignableFrom( unrefType ) )
				{
					var parseMethod = unrefType.GetMethod( "Parse", BindingFlags.Public | BindingFlags.Static );
					if( parseMethod != null )
						return parseMethod.Invoke( null, new object[] { text } );
				}

				return SimpleTypes.ParseValue( unrefType, text );
			}
			catch
			{
				invalid = true;
				return null;
			}
		}

		protected virtual void TextBoxTextChanged()
		{
			var values = GetValues();
			if( values == null )
				return;

			var value = values[ 0 ];
			var unrefValue = ReferenceUtility.GetUnreferencedValue( value );

			var v = ParseValueFromTextBox( out invalidValue );
			if( !invalidValue )
			{
				try
				{
					if( !Equals( v, unrefValue ) || values.Length > 1 )
						SetValue( v, true );
				}
				catch { }
			}

			//var control = (IHCTextBox)CreatedControlInsidePropertyItemControl;
			//var netType = Property.Type.GetNetType();
			//var unrefType = ReferenceUtils.GetUnreferencedType( netType );

			//object v = null;
			//try
			//{
			//	v = SimpleTypesUtils.ParseValue( unrefType, control.TextBox1.Text );

			//	invalidValue = false;
			//	SetValue( v, true );
			//}
			//catch
			//{
			//	invalidValue = true;
			//}
		}
	}
}