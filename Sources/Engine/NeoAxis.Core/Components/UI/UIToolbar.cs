// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents a toolbar.
	/// </summary>
	public class UIToolbar : UIControl
	{
		/// <summary>
		/// Indents around buttons.
		/// </summary>
		[DefaultValue( "Units 8 8 8 8" )]
		public Reference<UIMeasureValueRectangle> BorderIndents
		{
			get { if( _borderIndents.BeginGet() ) BorderIndents = _borderIndents.Get( this ); return _borderIndents.value; }
			set { if( _borderIndents.BeginSet( ref value ) ) { try { BorderIndentsChanged?.Invoke( this ); } finally { _borderIndents.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BorderIndents"/> property value changes.</summary>
		public event Action<UIToolbar> BorderIndentsChanged;
		ReferenceField<UIMeasureValueRectangle> _borderIndents = new UIMeasureValueRectangle( UIMeasure.Units, 8, 8, 8, 8 );

		/// <summary>
		/// Indents between buttons.
		/// </summary>
		[DefaultValue( "Units 4 4" )]
		public Reference<UIMeasureValueVector2> IndentBetweenItems
		{
			get { if( _indentBetweenItems.BeginGet() ) IndentBetweenItems = _indentBetweenItems.Get( this ); return _indentBetweenItems.value; }
			set { if( _indentBetweenItems.BeginSet( ref value ) ) { try { IndentBetweenItemsChanged?.Invoke( this ); } finally { _indentBetweenItems.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndentBetweenItems"/> property value changes.</summary>
		public event Action<UIToolbar> IndentBetweenItemsChanged;
		ReferenceField<UIMeasureValueVector2> _indentBetweenItems = new UIMeasureValueVector2( UIMeasure.Units, 4, 4 );

		///////////////////////////////////////////

		public UIToolbar()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 600, 56 );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			//create buttons by default
			for( int n = 1; n <= 3; n++ )
			{
				var button = CreateComponent<UIButton>();
				button.Name = "Button " + n.ToString();
				button.Text = n.ToString();
				button.Size = new UIMeasureValueVector2( UIMeasure.Units, 40, 40 );
			}

			UpdateControls();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			UpdateControls();
		}

		void UpdateControls()
		{
			GetScreenRectangle( out var clientRect );
			var borderIndents = BorderIndents.Value;
			clientRect.LeftTop += GetScreenOffsetByValue( new UIMeasureValueVector2( borderIndents.Measure, borderIndents.Left, borderIndents.Top ) );
			clientRect.RightBottom -= GetScreenOffsetByValue( new UIMeasureValueVector2( borderIndents.Measure, borderIndents.Right, borderIndents.Bottom ) );

			var indentBetweenItems = GetScreenOffsetByValue( IndentBetweenItems );

			var currentPos = Vector2.Zero;
			var maxHeightInLine = 0.0;

			foreach( var control in GetComponents<UIControl>() )
			{
				control.GetScreenSize( out var controlSize );

				//wrap to next line
				if( currentPos.X != 0 && currentPos.X + controlSize.X > clientRect.Size.X )
				{
					currentPos.X = 0;
					currentPos.Y += maxHeightInLine + indentBetweenItems.Y;
				}

				control.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, clientRect.Left + currentPos.X, clientRect.Top + currentPos.Y, 0, 0 );

				currentPos.X += controlSize.X + indentBetweenItems.X;
				if( maxHeightInLine < controlSize.Y )
					maxHeightInLine = controlSize.Y;
			}
		}

	}
}
