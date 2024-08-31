// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The class for working with a procedurally generated GUI.
	/// </summary>
	public static class ProcedureUI
	{
		/// <summary>
		/// Represents a form for <see cref="ProcedureUI"/>.
		/// </summary>
		public abstract class Form
		{
			public abstract Button CreateButton( string text, Button.SizeEnum size = Button.SizeEnum.Standard );
			public abstract Check CreateCheck( string text );
			public abstract Edit CreateEdit( string text = "" );
			public abstract Text CreateText( string text );

			public abstract void AddRow( IEnumerable<Control> controls );

			public object AnyData { get; set; }
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents a control for <see cref="ProcedureUI"/>.
		/// </summary>
		public abstract class Control
		{
			Form parent;
			public Form Parent { get { return parent; } }

			public abstract bool Enabled { get; set; }
			public abstract bool Visible { get; set; }
			public abstract string Text { get; set; }

			public object AnyData { get; set; }

			//

			public Control( Form parent )
			{
				this.parent = parent;
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents a button for <see cref="ProcedureUI"/>.
		/// </summary>
		public abstract class Button : Control
		{
			public abstract event Action<Button> Click;

			public enum SizeEnum
			{
				Standard,
				Long,
			}
			//public abstract SizeEnum Size { get; set; }

			public Button( Form parent ) : base( parent ) { }
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents a check box for <see cref="ProcedureUI"/>.
		/// </summary>
		public abstract class Check : Control
		{
			public enum CheckValue
			{
				Unchecked,
				Checked,
				Indeterminate,
			}
			public abstract CheckValue Checked { get; set; }
			public abstract event Action<Check> CheckedChanged;

			public abstract event Action<Check> Click;

			//public enum StateEnum
			//{
			//	Unchecked,
			//	Checked,
			//	Indeterminate,
			//}
			//public abstract StateEnum State { get; set; }
			//public abstract event Action<Check> StateChanged;

			//public bool? Checked
			//{
			//	get
			//	{
			//		switch( State )
			//		{
			//		case StateEnum.Checked: return true;
			//		case StateEnum.Unchecked: return false;
			//		default: return null;
			//		}
			//	}
			//	set
			//	{
			//		if( value.HasValue )
			//			State = value.Value ? StateEnum.Checked : StateEnum.Unchecked;
			//		else
			//			State = StateEnum.Indeterminate;
			//	}
			//}

			public Check( Form parent ) : base( parent ) { }
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents a edit box for <see cref="ProcedureUI"/>.
		/// </summary>
		public abstract class Edit : Control
		{
			public Edit( Form parent ) : base( parent ) { }
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents a text label for <see cref="ProcedureUI"/>.
		/// </summary>
		public abstract class Text : Control
		{
			public virtual bool Bold { get; set; } = false;

			public Text( Form parent ) : base( parent ) { }
		}

		/////////////////////////////////////////

		//public class Row
		//{
		//	public List<Control> Controls { get; } = new List<Control>();

		//	public Row()
		//	{
		//	}

		//	public Row( IEnumerable<Control> controls )
		//	{
		//		if( controls != null )
		//			Controls.AddRange( controls );
		//	}
		//}

	}
}
