// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// 2D grid of UI controls.
	/// </summary>
	public class UIGrid : UIControl
	{
		/// <summary>
		/// The amount of columns.
		/// </summary>
		[DefaultValue( 4 )]
		public Reference<int> Columns
		{
			get { if( _columns.BeginGet() ) Columns = _columns.Get( this ); return _columns.value; }
			set
			{
				if( _columns.BeginSet( ref value ) )
				{
					try
					{
						ColumnsChanged?.Invoke( this );
						UpdateCells();
					}
					finally { _columns.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Columns"/> property value changes.</summary>
		public event Action<UIGrid> ColumnsChanged;
		ReferenceField<int> _columns = 4;

		/// <summary>
		/// The amount of rows.
		/// </summary>
		[DefaultValue( 4 )]
		public Reference<int> Rows
		{
			get { if( _rows.BeginGet() ) Rows = _rows.Get( this ); return _rows.value; }
			set
			{
				if( _rows.BeginSet( ref value ) )
				{
					try
					{
						RowsChanged?.Invoke( this );
						UpdateCells();
					}
					finally { _rows.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Rows"/> property value changes.</summary>
		public event Action<UIGrid> RowsChanged;
		ReferenceField<int> _rows = 4;

		/// <summary>
		/// Whether to manage position and size of the controls.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AutoPosition
		{
			get { if( _autoPosition.BeginGet() ) AutoPosition = _autoPosition.Get( this ); return _autoPosition.value; }
			set { if( _autoPosition.BeginSet( ref value ) ) { try { AutoPositionChanged?.Invoke( this ); } finally { _autoPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AutoPosition"/> property value changes.</summary>
		public event Action<UIGrid> AutoPositionChanged;
		ReferenceField<bool> _autoPosition = true;

		/// <summary>
		/// Whether to visualize borders of the controls.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> DisplayBorders
		{
			get { if( _displayBorders.BeginGet() ) DisplayBorders = _displayBorders.Get( this ); return _displayBorders.value; }
			set { if( _displayBorders.BeginSet( ref value ) ) { try { DisplayBordersChanged?.Invoke( this ); } finally { _displayBorders.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayBorders"/> property value changes.</summary>
		public event Action<UIGrid> DisplayBordersChanged;
		ReferenceField<bool> _displayBorders = true;

		/////////////////////////////////////////

		public UIGrid()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 400 );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			UpdateCells();
		}

		public void UpdateCells()
		{
			var actualControls = new ESet<Component>( Rows * Columns );

			//add new cells
			for( int nRow = 0; nRow < Rows; nRow++ )
			{
				for( int nColumn = 0; nColumn < Columns; nColumn++ )
				{
					var name = string.Format( "Cell {0} {1}", nRow, nColumn );
					var control = Components.GetByName( name );
					if( control == null )
					{
						control = CreateComponent<UIControl>();
						control.Name = name;
					}

					actualControls.Add( control );
				}
			}

			var toDelete = new ESet<Component>();

			foreach( var c in GetCells() )
			{
				if( !actualControls.Contains( c ) )
					toDelete.Add( c );
			}

			//delete old
			foreach( var c in toDelete )
				c.RemoveFromParent( false );

			if( AutoPosition )
				UpdateCellsPositionAndSize();
		}

		public List<UIControl> GetCells()
		{
			var result = new List<UIControl>( Rows * Columns );
			foreach( var control in GetComponents<UIControl>() )
			{
				if( GetCellIndex( control, out _ ) )
					result.Add( control );
			}
			return result;
		}

		bool GetCellIndex( UIControl control, out Vector2I index )
		{
			try
			{
				if( control.Name.Length > 5 && control.Name.Substring( 0, 5 ) == "Cell " )
				{
					var index2 = Vector2I.Parse( control.Name.Substring( 5 ).Trim() );
					index = new Vector2I( index2.Y, index2.X );
					return true;
				}
			}
			catch
			{
			}
			index = Vector2I.Zero;
			return false;
		}

		public virtual void UpdateCellsPositionAndSize()
		{
			if( Rows != 0 && Columns != 0 )
			{
				foreach( var control in GetCells() )
				{
					if( GetCellIndex( control, out var cellIndex ) )
					{
						var x = (double)cellIndex.X / Columns;
						var y = (double)cellIndex.Y / Rows;
						control.Margin = new UIMeasureValueRectangle( UIMeasure.Parent, x, y, 0, 0 );
						control.Size = new UIMeasureValueVector2( UIMeasure.Parent, 1.0 / Columns, 1.0 / Rows );
					}
				}
			}
		}

		//protected override void OnRenderUI( CanvasRenderer renderer )
		//{
		//	base.OnRenderUI( renderer );

		//	if( AutoPosition )
		//		UpdateCellsPositionAndSize();

		//	if( DisplayBorders )
		//	{
		//		foreach( var control in GetCells() )
		//		{
		//			control.GetScreenRectangle( out var rect );
		//			renderer.AddRectangle( rect, new ColorValue( 0.5, 0.5, 0.5 ) );
		//		}
		//	}
		//}
	}
}
