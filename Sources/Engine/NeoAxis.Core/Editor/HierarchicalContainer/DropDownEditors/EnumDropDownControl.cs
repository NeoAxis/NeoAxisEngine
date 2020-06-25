// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class EnumDropDownControl : HCDropDownControl
	{
		private class EnumInfo
		{
			public bool SetByCode;
			public object Value;

			public EnumInfo( object value )
			{
				Value = value;
			}

			public string DisplayName
			{
				get
				{
					//TODO: we can use DisplayAttribute: https://msdn.microsoft.com/ru-ru/library/system.componentmodel.dataannotations.displayattribute(v=vs.110).aspx
					// or own custom attribute.
					//EnumExtension.GetValueDisplayName()
					return TypeUtility.DisplayNameAddSpaces( Value.ToString() );
				}
			}

			public string Description
			{
				get { return EnumExtension.GetValueDescription( Value ); }
			}
		}

		HCItemEnumDropDown itemProperty;
		HCItemProperty propertyItemForUndoSupport;
		bool valueWasChanged;
		object currentValue;
		bool isFlag;
		Type propertyType;
		Type enumDataType;
		int dropdownHeight = 200;

		public int DropDownHeight
		{
			get { return dropdownHeight; }
			set { dropdownHeight = value; }
		}

		public EnumDropDownControl()
		{
			InitializeComponent();

			if( EditorAPI.DarkTheme )
			{
				listViewEnum.BackColor = Color.FromArgb( 54, 54, 54 );
				listViewEnum.ForeColor = Color.FromArgb( 240, 240, 240 );
			}
		}

		public EnumDropDownControl( HCItemEnumDropDown itemProperty )
		{
			InitializeComponent();

			this.itemProperty = itemProperty;

			propertyType = ReferenceUtility.GetUnreferencedType( itemProperty.Property.Type.GetNetType() );
			if( propertyType.IsEnum )
			{
				enumDataType = Enum.GetUnderlyingType( propertyType );
				isFlag = propertyType.IsDefined( typeof( FlagsAttribute ), false );
			}

			currentValue = ReferenceUtility.GetUnreferencedValue( itemProperty.GetValues()[ 0 ] );
			propertyItemForUndoSupport = itemProperty.GetItemInHierarchyToRestoreValues();
			propertyItemForUndoSupport.SaveValuesToRestore();

			listViewEnum.Items.Clear();
			listViewEnum.CheckBoxes = isFlag;

			foreach( var val in Enum.GetValues( propertyType ) )
			{
				var info = new EnumInfo( val );

				var displayName = info.DisplayName;
				var description = info.Description;
				itemProperty.Owner.PerformOverridePropertyEnumItem( itemProperty, ref displayName, ref description );

				listViewEnum.Items.Add( new ListViewItem
				{
					Text = displayName,
					ToolTipText = description,
					Tag = info
				} );

				//listViewEnum.Items.Add( new ListViewItem
				//{
				//	Text = info.DisplayName,
				//	ToolTipText = info.Description,
				//	Tag = info
				//} );
			}

			UpdateCheckState();

			int itemCount = listViewEnum.Items.Count;
			var height = itemCount * ( Font.Height + DpiHelper.Default.ScaleValue( 4 ) ) + 3;
			var width = ( (IHCProperty)itemProperty.CreatedControl ).EditorControl.Width; //TODO: bad incapsulation. why this control should know about EditorControl.Width ?

			Size = new Size( width, Math.Min( dropdownHeight, height ) );

			if( EditorAPI.DarkTheme )
			{
				listViewEnum.BackColor = Color.FromArgb( 54, 54, 54 );
				listViewEnum.ForeColor = Color.FromArgb( 240, 240, 240 );
			}
		}

		private void SelectCurrentItem()
		{
			// make initial selection after the parent shows
			if( !Visible )
				return;

			if( isFlag )
			{
				// select the first checked one
				foreach( ListViewItem item in listViewEnum.CheckedItems )
				{
					item.Selected = true;
					item.Focused = true;
					item.EnsureVisible();
					break;
				}
			}
			else
			{
				foreach( ListViewItem item in listViewEnum.Items )
				{
					EnumInfo info = item.Tag as EnumInfo;
					if( info.Value.Equals( currentValue ) )
					{
						item.Selected = true;
						item.Focused = true;
						item.EnsureVisible();
						break;
					}
				}
			}
		}

		public override void OnHolderOpened()
		{
			base.OnHolderOpened();

			// focus listview to visualize selection of current item.
			// maybe there is more suitable event ?
			listViewEnum.Focus();
			SelectCurrentItem();
		}

		public override void OnCommitChanges()
		{
			base.OnCommitChanges();

			if( valueWasChanged )
				propertyItemForUndoSupport.AddUndoActionWithSavedValuesToRestore();
		}

		public override void OnCancelChanges()
		{
			base.OnCancelChanges();

			if( valueWasChanged )
				propertyItemForUndoSupport.RestoreSavedOldValues();
		}

		private void UpdateItemProperty()
		{
			if( itemProperty != null && itemProperty.CanEditValue() )
				itemProperty.SetValue( currentValue, false );
			valueWasChanged = true;
		}

		private void listViewEnum_ItemCheck( object sender, ItemCheckEventArgs e )
		{
			Debug.Assert( isFlag );

			EnumInfo info = listViewEnum.Items[ e.Index ].Tag as EnumInfo;

			if( info.SetByCode )
			{
				info.SetByCode = false;
				return;
			}

			if( e.NewValue == CheckState.Checked )
			{
				if( IsZero( enumDataType, info.Value ) )  // user is chekcing the item with zero value ( None )
					currentValue = Enum.ToObject( currentValue.GetType(), 0 );
				else
					AddBits( enumDataType, ref currentValue, info.Value );
			}
			else
			{
				object prevValue = currentValue;
				RemoveBits( enumDataType, ref currentValue, info.Value );

				if( IsZeroValueSituation() )
					currentValue = prevValue;
			}

			e.NewValue = e.CurrentValue;

			UpdateItemProperty();
			UpdateCheckState(); // this will change the check box on the list view item
		}

		private bool IsZeroValueSituation()
		{
			if( IsZero( enumDataType, currentValue ) )
			{
				if( !Enum.IsDefined( propertyType, currentValue ) )
					return true;
			}
			return false;
		}

		private void listViewEnum_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( isFlag )
				return;

			if( listViewEnum.SelectedItems.Count == 0 )
				return;

			ListView listView = (ListView)sender;
			EnumInfo info = listView.SelectedItems[ 0 ].Tag as EnumInfo;

			if( !info.Value.Equals( currentValue ) )
			{
				currentValue = info.Value;
				UpdateItemProperty();
			}
		}

		private void listViewEnum_MouseDoubleClick( object sender, MouseEventArgs e )
		{
			ParentHolder.Close( true );
		}

		private void listViewEnum_MouseClick( object sender, MouseEventArgs e )
		{
			if( !isFlag )
				ParentHolder.Close( true );
		}

		private void listViewEnum_SizeChanged( object sender, EventArgs e )
		{
			listViewEnum.Columns[ 0 ].Width = listViewEnum.Width;
			listViewEnum.Invalidate();
			this.Invalidate();
		}

		private void UpdateCheckState()
		{
			if( !isFlag )
				return;

			foreach( ListViewItem item in listViewEnum.Items )
			{
				EnumInfo info = item.Tag as EnumInfo;
				bool bitExist = DoBitsExist( Enum.GetUnderlyingType( propertyType ), currentValue, info.Value );
				if( item.Checked != bitExist )
				{
					info.SetByCode = true;
					item.Checked = bitExist;
				}
			}
		}

		private bool DoBitsExist( Type enumDataType, object value, object bits )
		{
			// zero needs special treatment, because you cannot do bitwise operations using zeros
			bool valueIsZero = IsZero( enumDataType, value );
			bool bitsIsZero = IsZero( enumDataType, bits );

			if( valueIsZero && bitsIsZero )
			{
				return true;
			}
			else if( valueIsZero && !bitsIsZero )
			{
				return false;
			}
			else if( !valueIsZero && bitsIsZero )
			{
				return false;
			}

			// otherwise (!valueIsZero && !bitsIsZero)

			if( enumDataType == typeof( Int16 ) )
			{
				Int16 val = Convert.ToInt16( value );
				Int16 bts = Convert.ToInt16( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( UInt16 ) )
			{
				UInt16 val = Convert.ToUInt16( value );
				UInt16 bts = Convert.ToUInt16( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( Int32 ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( UInt32 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				UInt32 bts = Convert.ToUInt32( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( Int64 ) )
			{
				Int64 val = Convert.ToInt64( value );
				Int64 bts = Convert.ToInt64( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( UInt64 ) )
			{
				UInt64 val = Convert.ToUInt64( value );
				UInt64 bts = Convert.ToUInt64( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( SByte ) )
			{
				SByte val = Convert.ToSByte( value );
				SByte bts = Convert.ToSByte( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( Byte ) )
			{
				Byte val = Convert.ToByte( value );
				Byte bts = Convert.ToByte( bits );
				return ( ( val & bts ) == bts );
			}
			return false;
		}

		private void RemoveBits( Type enumDataType, ref object value, object bits )
		{
			if( enumDataType == typeof( Int16 ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt16 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				UInt32 bts = Convert.ToUInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Int32 ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt32 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				UInt32 bts = Convert.ToUInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Int64 ) )
			{
				Int64 val = Convert.ToInt64( value );
				Int64 bts = Convert.ToInt64( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt64 ) )
			{
				UInt64 val = Convert.ToUInt64( value );
				UInt64 bts = Convert.ToUInt64( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( SByte ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Byte ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
		}

		private void AddBits( Type enumDataType, ref object value, object bits )
		{
			if( enumDataType == typeof( Int16 ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt16 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				UInt32 bts = Convert.ToUInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Int32 ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt32 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				UInt32 bts = Convert.ToUInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Int64 ) )
			{
				Int64 val = Convert.ToInt64( value );
				Int64 bts = Convert.ToInt64( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt64 ) )
			{
				UInt64 val = Convert.ToUInt64( value );
				UInt64 bts = Convert.ToUInt64( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( SByte ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Byte ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
		}

		private bool IsZero( Type enumDataType, object value )
		{
			if( enumDataType == typeof( Int16 ) )
			{
				Int16 val = Convert.ToInt16( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( UInt16 ) )
			{
				UInt16 val = Convert.ToUInt16( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( Int32 ) )
			{
				Int32 val = Convert.ToInt32( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( UInt32 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( Int64 ) )
			{
				Int64 val = Convert.ToInt64( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( UInt64 ) )
			{
				UInt64 val = Convert.ToUInt64( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( SByte ) )
			{
				SByte val = Convert.ToSByte( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( Byte ) )
			{
				Byte val = Convert.ToByte( value );
				return ( val == 0 );
			}
			return false;
		}
	}
}
