// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public partial class HCGridProperty : UserControl, IHCProperty
	{
		string buttonReferenceCurrentToolTip;
		int splitterPosition;
		Control editorControl;

		KryptonButton buttonExpand;
		readonly Size buttonExpandSize = DpiHelper.Default.ScaleValue( new Size( 14, 14 ) );

		KryptonButton buttonDefaultValue;
		readonly Size buttonDefaultValueSize = DpiHelper.Default.ScaleValue( new Size( 12, 12 ) );

		KryptonButton buttonReference;
		readonly Size buttonReferenceSize = DpiHelper.Default.ScaleValue( new Size( 22, 18 ) );

		KryptonButton buttonType;

		bool showOnlyEditorControl;

		//

		public HCGridProperty()
		{
			InitializeComponent();

			//DoubleBuffered = true;
			ResizeRedraw = true;

			DarkThemeUtility.ApplyToForm( this );
			DarkThemeUtility.ApplyToToolTip( propertyToolTip );

			// for debug
			//BackColor = Random.Generate(ColorValue.Zero, ColorValue.One).ToColor();
		}

		public Label LabelName
		{
			get { return labelName; }
		}

		public void LabelNameSetToolTip( string value )
		{
			if( propertyToolTip.GetToolTip( labelName ) == string.Empty )
				propertyToolTip.SetToolTip( labelName, value );
		}

		public void ButtonExpandInit()
		{
			buttonExpand = new KryptonButton();

			buttonExpand.Location = new Point( 2, DpiHelper.Default.ScaleValue( 5 ) );
			buttonExpand.Size = buttonExpandSize;
			if( !EditorAPI.DarkTheme )
				buttonExpand.StateNormal.Back.Color1 = Color.WhiteSmoke;
			buttonExpand.TabIndex = 0;
			if( EditorAPI.DarkTheme )
				buttonExpand.Values.Image = EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 1.5 ? "Plus_Big_Dark" : "Plus_small3_Dark" );
			else
				buttonExpand.Values.Image = EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 1.5 ? "Plus_Big" : "Plus_small3" );
			buttonExpand.Values.Text = "";
			buttonExpand.Visible = false;

			Controls.Add( buttonExpand );
		}

		public KryptonButton ButtonExpand
		{
			get { return buttonExpand; }
		}

		public void ButtonDefaultValueInit()
		{
			buttonDefaultValue = new KryptonButton();

			buttonDefaultValue.Location = new Point( 0, DpiHelper.Default.ScaleValue( 6 ) );
			buttonDefaultValue.Size = buttonDefaultValueSize;
			buttonDefaultValue.StateCommon.Border.Draw = InheritBool.False;
			buttonDefaultValue.StateCommon.Border.DrawBorders = PaletteDrawBorders.Top | PaletteDrawBorders.Bottom | PaletteDrawBorders.Left | PaletteDrawBorders.Right;
			buttonDefaultValue.StateDisabled.Back.Draw = InheritBool.False;
			buttonDefaultValue.StateNormal.Back.Draw = InheritBool.False;
			buttonDefaultValue.TabIndex = 2;
			propertyToolTip.SetToolTip( this.buttonDefaultValue, EditorLocalization.Translate( "SettingsWindow", "Reset to default." ) );
			buttonDefaultValue.Values.Text = "";

			Controls.Add( buttonDefaultValue );
		}

		public KryptonButton ButtonDefaultValue
		{
			get { return buttonDefaultValue; }
		}

		public void ButtonReferenceInit()
		{
			buttonReference = new KryptonButton();

			buttonReference.Location = new Point( 0, DpiHelper.Default.ScaleValue( 3 ) );
			buttonReference.Size = buttonReferenceSize;
			if( !EditorAPI.DarkTheme )
				buttonReference.StateNormal.Back.Color1 = Color.WhiteSmoke;
			buttonReference.TabIndex = 3;
			buttonReference.Values.Text = "";
			buttonReference.Visible = false;

			Controls.Add( buttonReference );
		}

		public KryptonButton ButtonReference
		{
			get { return buttonReference; }
		}

		public void ButtonReferenceSetToolTip( string value )
		{
			if( buttonReferenceCurrentToolTip != value && buttonReference != null )
			{
				buttonReferenceCurrentToolTip = value;
				propertyToolTip.SetToolTip( buttonReference, value );
			}
		}

		public void SetToolTip( Control control, string caption )
		{
			if( propertyToolTip.GetToolTip( control ) != caption )
				propertyToolTip.SetToolTip( control, caption );
		}

		public void ButtonTypeInit()
		{
			buttonType = new KryptonButton();

			buttonType.Location = new Point( 0, DpiHelper.Default.ScaleValue( 3 ) );
			buttonType.Size = DpiHelper.Default.ScaleValue( new Size( 22, 18 ) );
			buttonType.TabIndex = 6;
			propertyToolTip.SetToolTip( buttonType, EditorLocalization.Translate( "SettingsWindow", "Select a class type." ) );
			buttonType.Values.Text = "...";
			buttonType.Visible = false;

			Controls.Add( buttonType );
		}

		public KryptonButton ButtonType
		{
			get { return buttonType; }
		}

		public Control EditorControl
		{
			get { return editorControl; }
			set
			{
				if( editorControl != null )
					Controls.Remove( editorControl );
				editorControl = value;
				Controls.Add( editorControl );

				// default
				SplitterPosition = (int)( this.Width / 2.5f );
			}
		}

		public int SplitterPosition
		{
			get { return splitterPosition; }
			set
			{
				if( splitterPosition == value )
					return;
				splitterPosition = value;
				UpdateLayout();
			}
		}

		protected override void WndProc( ref Message m )
		{
			// pass mouse events to parent.
			if( m.Msg == PI.WM_NCHITTEST )
				m.Result = (IntPtr)PI.HTTRANSPARENT;
			else
				base.WndProc( ref m );
		}

		internal virtual void UpdateLayout()
		{
			if( labelName.Width != splitterPosition - buttonExpandSize.Width - HierarchicalContainer.SpliterWidth - 5 )
				labelName.Width = splitterPosition - buttonExpandSize.Width - HierarchicalContainer.SpliterWidth - 5;
			if( labelName.Visible != !ShowOnlyEditorControl )
				labelName.Visible = !ShowOnlyEditorControl;

			int offset = 1;

			if( buttonDefaultValue != null )
			{
				if( buttonDefaultValue.Location != new Point( splitterPosition + offset, buttonDefaultValue.Location.Y ) )
					buttonDefaultValue.Location = new Point( splitterPosition + offset, buttonDefaultValue.Location.Y );
			}
			offset += buttonDefaultValueSize.Width + 1;

			if( buttonReference != null )
			{
				if( buttonReference.Location != new Point( splitterPosition + offset, buttonReference.Location.Y ) )
					buttonReference.Location = new Point( splitterPosition + offset, buttonReference.Location.Y );
			}
			offset += buttonReferenceSize.Width + 3;

			if( buttonType != null && buttonType.Visible )
			{
				if( buttonType.Location != new Point( splitterPosition + offset, buttonType.Location.Y ) )
					buttonType.Location = new Point( splitterPosition + offset, buttonType.Location.Y );
				offset += buttonType.Width + 1;
			}

			if( ShowOnlyEditorControl )
			{
				if( editorControl.Dock != DockStyle.Fill )
					editorControl.Dock = DockStyle.Fill;
			}
			else
			{
				if( editorControl.Location != new Point( splitterPosition + offset, 0 ) )
					editorControl.Location = new Point( splitterPosition + offset, 0 );
				if( editorControl.Width != Width - EditorControl.Location.X - 2 )
					editorControl.Width = Width - EditorControl.Location.X - 2;
			}
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			if( HierarchicalContainer.DrawSplitter )
			{
#if !ANDROID
				var color = EditorAPI.DarkTheme ? Color.FromArgb( 65, 65, 65 ) : Color.FromArgb( 225, 225, 225 );

				// splitter debug draw
				using( Pen myPen = new Pen( color ) )
				{
					var owner = Parent as HierarchicalContainer;
					//!!!!
					if( owner == null && Parent != null )
						owner = Parent.Parent as HierarchicalContainer;

					myPen.Width = HierarchicalContainer.SpliterWidth;
					int pos = owner.SplitterPosition - HierarchicalContainer.SpliterWidth / 2;
					e.Graphics.DrawLine( myPen, pos, 0, pos, this.Height );
				}
#endif //!ANDROID
			}
		}

		public override string ToString()
		{
			return nameof( HCGridProperty ) + ": " + labelName.Text;
		}

		[Browsable( false )]
		public bool ShowOnlyEditorControl
		{
			get { return showOnlyEditorControl; }
			set { showOnlyEditorControl = value; }
		}
	}
}
