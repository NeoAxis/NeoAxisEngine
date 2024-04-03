#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Linq;
using System.Drawing.Drawing2D;

namespace NeoAxis.Editor
{
	public class EngineListView : EUserControl
	{
		List<Item> items = new List<Item>();
		//!!!!везде проверить проверяется ли
		bool multiSelect;
		bool checkBoxes;

		List<Item> selectedItems = new List<Item>();
		ESet<Item> selectedItemsSet = new ESet<Item>();

		ModeClass mode;

		int currentItemIndex;

		EngineScrollBar scrollBarVertical;
		EngineScrollBar scrollBarHorizontal;

		//DateTime? doubleBufferCompositedTime;

		//private System.Windows.Forms.Timer timer10ms;

		System.Drawing.Rectangle? tempCachedClientRectangle;

		bool canDrag;
		bool canStartDrag;
		Point canStartDragPosition;
		Item[] canStartDragItems;

		///////////////////////////////////////////////

		public delegate void SelectedItemsChangedDelegate( EngineListView sender );
		public event SelectedItemsChangedDelegate SelectedItemsChanged;

		public delegate void BeforeStartDragDelegate( EngineListView sender, Item[] items, ref bool canStart );
		public event BeforeStartDragDelegate BeforeStartDrag;

		public delegate void ItemCheckedChangedDelegate( EngineListView sender, Item item );
		public event ItemCheckedChangedDelegate ItemCheckedChanged;

		public delegate void PaintAdditionsDelegate( EngineListView sender, PaintEventArgs e );
		public event PaintAdditionsDelegate PaintAdditions;

		///////////////////////////////////////////////

		public class Item
		{
			EngineListView owner;

			public Item( EngineListView owner )
			{
				this.owner = owner;
			}

			public EngineListView Owner
			{
				get { return owner; }
			}

			public string Text
			{
				get { return text; }
				set
				{
					if( text == value )
						return;
					text = value;
					owner?.Invalidate();
				}
			}
			string text = "";

			public object Tag;

			public Image Image
			{
				get { return image; }
				set
				{
					if( image == value )
						return;
					image = value;
					owner?.Invalidate();
				}
			}
			Image image;

			public string Description { get; set; } = "";

			public bool ShowTooltip { get; set; }

			public bool ShowDisabled
			{
				get { return showDisabled; }
				set
				{
					if( showDisabled == value )
						return;
					showDisabled = value;
					owner?.Invalidate();
				}
			}
			bool showDisabled;

			public bool Checked
			{
				get { return _checked; }
				set
				{
					if( _checked == value )
						return;
					_checked = value;
					owner?.ItemCheckedChanged?.Invoke( Owner, this );
					owner?.Invalidate();
				}
			}
			bool _checked;

			public override string ToString()
			{
				return Text;
			}
		}

		///////////////////////////////////////////////

		public abstract class ModeClass
		{
			EngineListView owner;

			public ModeClass( EngineListView owner )
			{
				this.owner = owner;
			}

			public EngineListView Owner
			{
				get { return owner; }
			}

			public Vector2I ItemSize;
			public bool ClampItemWidthByListViewWidth;

			public abstract void Init();

			public abstract void PaintItem( PaintEventArgs e, int itemIndex );

			public bool GetItemRectangle( int itemIndex, out System.Drawing.Rectangle rect )
			{
				rect = Owner.GetItemRectangle_NoScrolling( itemIndex );
				if( Owner.ScrollBarHorizontal.Visible )
					rect.X -= Owner.ScrollBarHorizontal.Value;
				if( Owner.ScrollBarVertical.Visible )
					rect.Y -= Owner.ScrollBarVertical.Value;

				//check item is not visible
				if( rect.Right < 0 || rect.Bottom < 0 )
					return false;
				var clientRect = Owner.GetClientRectangle();
				if( rect.Top > clientRect.Bottom || rect.Left > clientRect.Right )
					return false;

				return true;
			}

			public void GetItemColors( int itemIndex, out Color backColor, out Color textColor )
			{
				var item = Owner.Items[ itemIndex ];

				if( Owner.IsItemSelected( item ) )
				{
					if( Owner.Focused && Owner.CurrentItemIndex == itemIndex && Owner.Enabled )
					{
						backColor = EditorAPI2.DarkTheme ? Color.FromArgb( 70, 70, 70 ) : Color.FromArgb( 0, 120, 215 );
						textColor = EditorAPI2.DarkTheme ? Color.FromArgb( 255, 255, 255 ) : Color.FromArgb( 255, 255, 255 );
					}
					else
					{
						backColor = EditorAPI2.DarkTheme ? Color.FromArgb( 60, 60, 60 ) : Color.FromArgb( 240, 240, 240 );
						textColor = EditorAPI2.DarkTheme ? Color.FromArgb( 230, 230, 230 ) : Color.FromArgb( 0, 0, 0 );
					}
				}
				else
				{
					backColor = EditorAPI2.DarkTheme ? Color.FromArgb( 40, 40, 40 ) : Color.FromArgb( 255, 255, 255 );
					textColor = EditorAPI2.DarkTheme ? Color.FromArgb( 230, 230, 230 ) : Color.FromArgb( 0, 0, 0 );
				}

				if( item.ShowDisabled || !Owner.Enabled )
					textColor = Color.Gray;
			}

			public virtual bool IsOverImage( Point position ) { return false; }
		}

		///////////////////////////////////////////////

		public class DefaultListMode : ModeClass
		{
			public bool DisplayImages;
			public Vector2I ImageSize;
			public int MarginLeft;
			public int MarginImageText;
			public int MarginRight;

			//

			public DefaultListMode( EngineListView owner, int imageSize )
				: base( owner )
			{
				ItemSize = new Vector2I( 10000, (int)( 17.0f * EditorAPI2.DPIScale ) );
				ClampItemWidthByListViewWidth = true;

				DisplayImages = true;

				var imageSize2 = (int)( (float)imageSize * EditorAPI2.DPIScale );
				//var imageSize2 = (int)( 16.0f * EditorAPI.DPIScale );

				//var imageSize2 = GetOptimalIconSize( (int)( 16.0f * EditorAPI.DPIScale ) );
				ImageSize = new Vector2I( imageSize2, imageSize2 );

				MarginLeft = (int)( EditorAPI2.DPIScale * 4.0f );
				MarginImageText = (int)( EditorAPI2.DPIScale * 2.0f );
				MarginRight = (int)( EditorAPI2.DPIScale * 2.0f );
			}

			public override void Init()
			{
			}

			public override void PaintItem( PaintEventArgs e, int itemIndex )
			{
				var item = Owner.Items[ itemIndex ];
				if( !GetItemRectangle( itemIndex, out var rect ) )
					return;
				GetItemColors( itemIndex, out var backColor, out var textColor );

				//background
				using( Brush brush = new SolidBrush( backColor ) )
					e.Graphics.FillRectangle( brush, rect );


				int offset = MarginLeft;

				//image
				if( DisplayImages || Owner.CheckBoxes )
				{
					var image = item.Image;

					if( Owner.CheckBoxes && image == null )
					{
						if( item.Checked )
						{
							var color = EditorAPI2.DarkTheme ? Color.FromArgb( 230, 230, 230 ) : Color.FromArgb( 50, 50, 50 );
							using( var brush = new SolidBrush( color ) )
							{
								var points = new Vector2[]
								{
									new Vector2( 290.04, 33.286 ),
									new Vector2( 118.861, 204.427 ),
									new Vector2( 52.32, 137.907 ),
									new Vector2( 0, 190.226 ),
									new Vector2( 118.861, 309.071 ),
									new Vector2( 342.357, 85.606 ),
								};
								var points2 = new Vector2[ points.Length ];
								for( int n = 0; n < points2.Length; n++ )
									points2[ n ] = points[ n ] / new Vector2( 342.357, 342.357 );


								float left = rect.Left + offset;
								float top = rect.Top + ( ItemSize.Y - ImageSize.Y ) / 2;

								left += (float)ImageSize.X * 0.1f;
								top += (float)ImageSize.X * 0.1f;

								var points3 = new PointF[ points.Length ];
								for( int n = 0; n < points3.Length; n++ )
								{
									var p = points2[ n ];
									points3[ n ] = new PointF( left + (float)p.X * ImageSize.X * 0.8f, top + (float)p.Y * ImageSize.Y * 0.8f );
								}

								e.Graphics.FillPolygon( brush, points3 );
							}
						}
					}
					else
					{
						if( image == null || image.PixelFormat == System.Drawing.Imaging.PixelFormat.DontCare )
						{
							image = EditorResourcesCache.GetDefaultImage( ImageSize.Y );
							//if( ImageSize.Y > 32 )
							//	image = Properties.Resources.Default_512;
							//else if( ImageSize.Y > 16 )
							//	image = Properties.Resources.Default_32;
							//else
							//	image = Properties.Resources.Default_16;
						}

						e.Graphics.DrawImage( image, rect.Left + offset, rect.Top + ( ItemSize.Y - ImageSize.Y ) / 2, ImageSize.X, ImageSize.Y );
					}

					offset += ImageSize.X + MarginImageText;
				}

				//text
				if( !string.IsNullOrEmpty( item.Text ) )
				{
					var normalTextFormatFlags = TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsTranslateTransform;

					var flags = normalTextFormatFlags | TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
					//if( wordWrap )
					//	flags |= TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

					var rect2 = new System.Drawing.Rectangle( rect.Left + offset, rect.Top, rect.Width - offset - MarginRight, rect.Height );

					TextRenderer.DrawText( e.Graphics, item.Text, Owner.Font, rect2, textColor, backColor, flags );
				}
			}

			public override bool IsOverImage( Point position )
			{
				if( DisplayImages || Owner.CheckBoxes )
				{
					if( position.X >= MarginLeft && position.X <= MarginLeft + ImageSize.X )
						return true;
				}
				return false;
			}
		}

		///////////////////////////////////////////////

		public class DragData : DataObject
		{
			public EngineListView ListView;
			public Item[] Items;
		}

		///////////////////////////////////////////////

		public EngineListView()
		{
			// We use double buffering to reduce drawing flicker
			SetStyle( ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true );

			// We need to repaint entire control whenever resized
			SetStyle( ControlStyles.ResizeRedraw, true );

			// Yes, we want to be drawn double buffered by default
			DoubleBuffered = true;

			SetStyle( ControlStyles.Selectable, true );
			TabStop = true;

			mode = new DefaultListMode( this, 16 );

			scrollBarVertical = new EngineScrollBar();
			scrollBarVertical.TabIndex = 1;
			scrollBarVertical.Scroll += ScrollBarVertical_Scroll;
			scrollBarVertical.TabStop = false;
			Controls.Add( scrollBarVertical );

			scrollBarHorizontal = new EngineScrollBar();
			scrollBarHorizontal.TabIndex = 2;
			scrollBarHorizontal.Orientation = Orientation.Horizontal;
			scrollBarHorizontal.Scroll += ScrollBarHorizontal_Scroll;
			scrollBarHorizontal.TabStop = false;
			Controls.Add( scrollBarHorizontal );

			//timer10ms = new Timer();
			//this.timer10ms.Interval = 10;
			//this.timer10ms.Tick += new System.EventHandler( this.timer10ms_Tick );
			//timer10ms.Start();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			UpdateScrollBars();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			//timer10ms.Stop();
			//timer10ms.Dispose();
		}

		public System.Drawing.Rectangle GetClientRectangle()
		{
			if( tempCachedClientRectangle.HasValue )
				return tempCachedClientRectangle.Value;

			var rect = new System.Drawing.Rectangle( 0, 0, Width, Height );

			if( scrollBarVertical != null && scrollBarVertical.Visible )
				rect.Width -= scrollBarVertical.Width + 2;
			if( scrollBarHorizontal != null && scrollBarHorizontal.Visible )
				rect.Height -= scrollBarHorizontal.Height + 2;

			return rect;
		}

		protected override void OnPaint( PaintEventArgs e )
		{
#if !DEPLOY

			tempCachedClientRectangle = null;
			tempCachedClientRectangle = GetClientRectangle();

			e.Graphics.InterpolationMode = InterpolationMode.High;

			//background
			{
				var rect = GetClientRectangle();

				var backColor = EditorAPI2.DarkTheme ? Color.FromArgb( 40, 40, 40 ) : Color.FromArgb( 255, 255, 255 );
				using( var brush = new SolidBrush( backColor ) )
					e.Graphics.FillRectangle( brush, rect );
			}

			//items
			for( int n = 0; n < items.Count; n++ )
			{
				var item = items[ n ];
				mode.PaintItem( e, n );
			}


			//background under scrollbars

			if( scrollBarVertical != null && scrollBarVertical.Visible )
			{
				var rect = new System.Drawing.Rectangle( scrollBarVertical.Location.X - 1, 0, Width - ( scrollBarVertical.Location.X - 1 ), Height );

				var backColor = EditorAPI2.DarkTheme ? Color.FromArgb( 47, 47, 47 ) : Color.FromArgb( 240, 240, 240 );
				using( var brush = new SolidBrush( backColor ) )
					e.Graphics.FillRectangle( brush, rect );
			}

			if( scrollBarHorizontal != null && scrollBarHorizontal.Visible )
			{
				var rect = new System.Drawing.Rectangle( 0, scrollBarHorizontal.Location.Y - 1, Width, Height - ( scrollBarHorizontal.Location.Y - 1 ) );

				var backColor = EditorAPI2.DarkTheme ? Color.FromArgb( 47, 47, 47 ) : Color.FromArgb( 240, 240, 240 );
				using( var brush = new SolidBrush( backColor ) )
					e.Graphics.FillRectangle( brush, rect );
			}
#endif

			PaintAdditions?.Invoke( this, e );

			tempCachedClientRectangle = null;
		}

		[Browsable( false )]
		public IReadOnlyList<Item> Items
		{
			get { return items; }
		}

		[Browsable( false )]
		public bool SetItemsScrollBarPositionReset = true;

		public void SetItems( ICollection<Item> items )
		{
			this.items = new List<Item>( items );

			//!!!!видимо сбросить SelectedItems
			//!!!!!что еще

			if( scrollBarVertical != null )
			{
				if( SetItemsScrollBarPositionReset )
				{
					scrollBarVertical.Value = 0;
					scrollBarHorizontal.Value = 0;
				}

				UpdateScrollBars();
			}

			Invalidate();
		}

		public void ClearItems()
		{
			SetItems( new List<Item>() );
		}

		[Browsable( false )]
		public IReadOnlyList<Item> SelectedItems
		{
			get { return selectedItems; }
			set
			{
				if( selectedItems.SequenceEqual( value ) )
					return;

				if( value != null )
					selectedItems = new List<Item>( value );
				else
					selectedItems = new List<Item>();

				selectedItemsSet = new ESet<Item>();
				foreach( var item in selectedItems )
					selectedItemsSet.AddWithCheckAlreadyContained( item );

				SelectedItemsChanged?.Invoke( this );

				Invalidate();
			}
		}

		[Browsable( false )]
		public Item SelectedItem
		{
			get
			{
				if( SelectedItems.Count == 1 )
					return SelectedItems[ 0 ];
				return null;
			}
			set
			{
				if( SelectedItem == value )
					return;

				var list = new List<Item>();
				if( value != null )
					list.Add( value );
				SelectedItems = list;
			}
		}

		public bool MultiSelect
		{
			get { return multiSelect; }
			set
			{
				if( multiSelect == value )
					return;
				multiSelect = value;
			}
		}

		[Browsable( false )]
		public ModeClass Mode
		{
			get { return mode; }
			set
			{
				if( mode == value )
					return;
				mode = value;

				UpdateScrollBars();
				Invalidate();
			}
		}

		[Browsable( false )]
		public int CurrentItemIndex
		{
			get { return currentItemIndex; }
			set
			{
				if( currentItemIndex == value )
					return;
				currentItemIndex = value;

				//update selected items
				if( !MultiSelect )
				{
					if( currentItemIndex >= 0 && currentItemIndex < Items.Count )
						SelectedItem = Items[ currentItemIndex ];
					else
						SelectedItem = null;
				}

				Invalidate();
			}
		}

		[Browsable( false )]
		public Item CurrentItem
		{
			get
			{
				if( currentItemIndex >= 0 && currentItemIndex < Items.Count )
					return items[ currentItemIndex ];
				return null;
			}
			set
			{
				if( value != null )
					CurrentItemIndex = GetItemIndex( value );
				else
					CurrentItemIndex = -1;
			}
		}

		public bool CheckBoxes
		{
			get { return checkBoxes; }
			set
			{
				if( checkBoxes == value )
					return;
				checkBoxes = value;
			}
		}

		[Browsable( false )]
		public Item[] CheckedItems
		{
			get { return Items.Where( i => i.Checked ).ToArray(); }
		}

		public int GetItemIndexAt( Point position, out bool overImage )
		{
			var position2 = position;
			if( scrollBarHorizontal != null && scrollBarHorizontal.Visible )
				position2.X += scrollBarHorizontal.Value;
			if( scrollBarVertical != null && scrollBarVertical.Visible )
				position2.Y += scrollBarVertical.Value;

			//!!!!slowly

			for( int n = 0; n < Items.Count; n++ )
			{
				var rect = GetItemRectangle_NoScrolling( n );
				if( rect.Contains( position2 ) )
				{
					overImage = mode.IsOverImage( position2 );
					return n;
				}
			}

			overImage = false;
			return -1;
		}

		public int GetItemIndexAt( Point position )
		{
			return GetItemIndexAt( position, out _ );
		}

		public Item GetItemAt( Point position )
		{
			var itemIndex = GetItemIndexAt( position );
			return itemIndex != -1 ? Items[ itemIndex ] : null;
		}

		public bool IsItemSelected( Item item )
		{
			return selectedItemsSet.Contains( item );
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			if( !Focused )
				Focus();

			base.OnMouseDown( e );

			if( e.Button == MouseButtons.Left || e.Button == MouseButtons.Right )
			{
				var itemIndex = GetItemIndexAt( e.Location, out var overImage );
				var item = itemIndex != -1 ? Items[ itemIndex ] : null;

				var toSelect = new ESet<Item>( SelectedItems.ToArray() );
				if( ( ModifierKeys & Keys.Control ) == 0 && ( ModifierKeys & Keys.Shift ) == 0 )
					toSelect.Clear();

				if( ( ModifierKeys & Keys.Shift ) != 0 )
				{
					if( CurrentItemIndex != -1 && itemIndex != -1 )
					{
						var from = Math.Min( CurrentItemIndex, itemIndex );
						var to = Math.Max( CurrentItemIndex, itemIndex );
						for( int n = from; n <= to; n++ )
							toSelect.AddWithCheckAlreadyContained( Items[ n ] );
					}
				}

				if( item != null )
					toSelect.AddWithCheckAlreadyContained( item );

				SelectedItems = toSelect.ToArray();
				CurrentItemIndex = itemIndex;
				EnsureVisibleCurrentItem();

				if( CheckBoxes && item != null && overImage )
					item.Checked = !item.Checked;
			}

			//drag
			if( e.Button == MouseButtons.Left && CanDrag )
			{
				var cursor = Cursor.Position;
				var items = SelectedItems.ToArray();

				if( items.Length != 0 )
				{
					var item = GetItemAt( PointToClient( cursor ) );
					if( item != null && items.Contains( item ) )
					{
						canStartDrag = true;
						canStartDragPosition = cursor;
						canStartDragItems = items;
					}
				}
			}
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			CanStartDragReset();
		}

		void CanStartDragReset()
		{
			canStartDrag = false;
			canStartDragPosition = new Point( 0, 0 );
			canStartDragItems = null;
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			if( canStartDrag )
			{
				var buttons = MouseButtons;
				if( ( buttons & MouseButtons.Left ) != 0 )//|| ( buttons & MouseButtons.Right ) != 0 || ( buttons & MouseButtons.Middle ) != 0 )
				{
					var threshold = 4;
					var cursor = Cursor.Position;

					if( Math.Abs( canStartDragPosition.X - cursor.X ) > threshold || Math.Abs( canStartDragPosition.Y - cursor.Y ) > threshold )
					{
						var canStart = true;
						BeforeStartDrag?.Invoke( this, canStartDragItems, ref canStart );

						if( canStart )
						{
							var data = new DragData();
							data.ListView = this;
							data.Items = canStartDragItems;
							DoDragDrop( data, DragDropEffects.Link );

							CanStartDragReset();
						}
					}
				}
			}
		}

		protected override void OnMouseWheel( MouseEventArgs e )
		{
			base.OnMouseWheel( e );

			if( scrollBarVertical != null && scrollBarVertical.Visible )
			{
				int scrollStep = e.Delta / 120;//System.Windows.Input.Mouse.MouseWheelDeltaForOneLine;
				var newValue = scrollBarVertical.Value - 80/*100*/ * scrollStep;
				MathEx.Clamp( ref newValue, 0, scrollBarVertical.Maximum );

				if( scrollBarVertical.Value != newValue )
				{
					scrollBarVertical.Value = newValue;
					Invalidate();
				}
			}
		}

		void EnsureVisibleCurrentItem()
		{
			if( CurrentItemIndex != -1 && Items.Count != 0 )
				EnsureVisible( Items[ CurrentItemIndex ] );
		}

		void ProcessKeySetCurrentItem( int itemIndex )
		{
			Item item;
			if( itemIndex >= 0 && itemIndex < Items.Count )
				item = Items[ itemIndex ];
			else
				item = null;

			var toSelect = new ESet<Item>( SelectedItems.ToArray() );
			if( ( ModifierKeys & Keys.Control ) == 0 && ( ModifierKeys & Keys.Shift ) == 0 )
				toSelect.Clear();

			if( ( ModifierKeys & Keys.Shift ) != 0 )
			{
				if( CurrentItemIndex != -1 && itemIndex != -1 )
				{
					var from = Math.Min( CurrentItemIndex, itemIndex );
					var to = Math.Max( CurrentItemIndex, itemIndex );
					for( int n = from; n <= to; n++ )
						toSelect.AddWithCheckAlreadyContained( Items[ n ] );
				}
			}

			if( item != null )
				toSelect.AddWithCheckAlreadyContained( item );

			SelectedItems = toSelect.ToArray();
			CurrentItemIndex = itemIndex;
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( Items.Count != 0 )
			{
				var keyCode = keyData & Keys.KeyCode;

				if( keyCode == Keys.Up )
				{
					if( CurrentItemIndex != 0 )
						ProcessKeySetCurrentItem( Math.Max( CurrentItemIndex - GetItemCountPerRow(), 0 ) );
					EnsureVisibleCurrentItem();
					return true;
				}
				else if( keyCode == Keys.Down )
				{
					if( CurrentItemIndex != Items.Count - 1 )
						ProcessKeySetCurrentItem( Math.Min( CurrentItemIndex + GetItemCountPerRow(), Items.Count - 1 ) );
					EnsureVisibleCurrentItem();
					return true;
				}
				else if( keyCode == Keys.Home )
				{
					if( CurrentItemIndex != 0 )
						ProcessKeySetCurrentItem( 0 );
					EnsureVisibleCurrentItem();
					return true;
				}
				else if( keyCode == Keys.End )
				{
					if( CurrentItemIndex != Items.Count - 1 )
						ProcessKeySetCurrentItem( Items.Count - 1 );
					EnsureVisibleCurrentItem();
					return true;
				}
				else if( keyCode == Keys.PageUp && Mode.ItemSize.Y != 0 )
				{
					var index = CurrentItemIndex;
					index -= Math.Max( GetClientRectangle().Height / Mode.ItemSize.Y * GetItemCountPerRow(), 1 );
					if( index < 0 )
						index = 0;
					ProcessKeySetCurrentItem( index );
					EnsureVisibleCurrentItem();
					return true;
				}
				else if( keyCode == Keys.PageDown )
				{
					var index = CurrentItemIndex;
					index += Math.Max( GetClientRectangle().Height / Mode.ItemSize.Y * GetItemCountPerRow(), 1 );
					if( index > Items.Count - 1 )
						index = Items.Count - 1;
					ProcessKeySetCurrentItem( index );
					EnsureVisibleCurrentItem();
					return true;
				}
				else if( keyCode == Keys.Left )
				{
					if( CurrentItemIndex != 0 )
						ProcessKeySetCurrentItem( Math.Max( CurrentItemIndex - 1, 0 ) );
					EnsureVisibleCurrentItem();
					return true;
				}
				else if( keyCode == Keys.Right )
				{
					if( CurrentItemIndex != Items.Count - 1 )
						ProcessKeySetCurrentItem( CurrentItemIndex + 1 );
					EnsureVisibleCurrentItem();
					return true;
				}
				else if( keyCode == Keys.Space )
				{
					if( CheckBoxes && Enabled )
					{
						if( SelectedItems.Count != 0 )
						{
							var allChecked = SelectedItems.All( i => i.Checked );
							foreach( var item in SelectedItems )
								item.Checked = !allChecked;
						}
						else
						{
							var item = CurrentItem;
							if( item != null )
								item.Checked = !item.Checked;
						}
						return true;
					}
				}
			}

			return base.ProcessCmdKey( ref msg, keyData );
		}

		protected override void OnGotFocus( EventArgs e )
		{
			base.OnGotFocus( e );

			Invalidate();
		}

		protected override void OnLostFocus( EventArgs e )
		{
			base.OnLostFocus( e );

			Invalidate();
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			UpdateScrollBars();
		}

		void UpdateScrollBarsBounds()
		{
			var size = (int)( EditorAPI2.DPIScale * 16.0f );

			if( scrollBarVertical.Visible && !scrollBarHorizontal.Visible )
				scrollBarVertical.SetBounds( Width - size - 1, 0, size, Height );
			else if( !scrollBarVertical.Visible && scrollBarHorizontal.Visible )
				scrollBarHorizontal.SetBounds( 0, Height - size - 1, Width, size );
			else
			{
				scrollBarVertical.SetBounds( Width - size - 1, 0, size, Height - size );
				scrollBarHorizontal.SetBounds( 0, Height - size - 1, Width - size, size );
			}
		}

		public void UpdateScrollBars()
		{
			if( scrollBarVertical == null )
				return;

			//var size = (int)( EditorAPI.DPIScale * 16.0f );

			bool updating1 = scrollBarVertical.MouseUpDownStatus;//&& engineScrollBar1.MouseScrollBarArea == EnhancedScrollBarMouseLocation.Thumb;
			if( !updating1 )
			{
				scrollBarVertical.Maximum = Math.Max( GetSizeOfAllItems().Y - Height, 0 );
				scrollBarVertical.SmallChange = 30;
				scrollBarVertical.LargeChange = Height;

				if( scrollBarVertical.Value > scrollBarVertical.Maximum )
					scrollBarVertical.Value = scrollBarVertical.Maximum;

				//scrollBarVertical.Value = (int)scrollViewer.VerticalOffset;

				//!!!!
				//scrollBarVertical.ItemSize = Mode.ItemSize.Y;

				//scrollBarVertical.SmallChange = zzzzzz;

				//var rows = (int)( (double)treeView.Height / (double)treeView.RowHeightScaled ) - 1;
				//engineScrollBarTreeVertical.ItemSize = treeView.RowHeightScaled;//Indent
				//engineScrollBarTreeVertical.Maximum = Math.Max( treeView.VScrollBar.Maximum - rows, 0 );

				//engineScrollBarTreeVertical.SmallChange = treeView.VScrollBar.SmallChange;
				//engineScrollBarTreeVertical.LargeChange = treeView.VScrollBar.LargeChange;
				//engineScrollBarTreeVertical.Value = treeView.VScrollBar.Value;
			}

			//!!!!
			bool updating2 = scrollBarHorizontal.MouseUpDownStatus;//&& engineScrollBar2.MouseScrollBarArea == EnhancedScrollBarMouseLocation.Thumb;
			if( !updating2 )
			{
				//!!!!

				//scrollBarVertical.Maximum = Math.Max( treeView.HScrollBar.Maximum - treeView.ClientSize.Width, 0 );
				//scrollBarVertical.SmallChange = treeView.HScrollBar.SmallChange;
				//scrollBarVertical.LargeChange = treeView.HScrollBar.LargeChange;
				//scrollBarVertical.Value = treeView.HScrollBar.Value;
			}

			//update visibility

			//!!!!
			scrollBarHorizontal.Visible = false;

			//!!!!GetClientRectangle() зависит от видимости скроллов

			UpdateScrollBarsBounds();
			scrollBarVertical.Visible = GetSizeOfAllItems().Y > GetClientRectangle().Height && Items.Count != 0;

			UpdateScrollBarsBounds();
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

		public Vector2I GetSizeOfAllItems()
		{
			//!!!!что со скроллом
			var clientWidth = GetClientRectangle().Width;

			if( Items.Count == 0 || clientWidth == 0 )
				return new Vector2I( 0, 0 );

			//wide list mode
			if( Mode.ClampItemWidthByListViewWidth && Mode.ItemSize.X >= clientWidth )
				return new Vector2I( clientWidth, Mode.ItemSize.Y * Items.Count );

			int countPerRow = Math.Max( clientWidth / Mode.ItemSize.X, 1 );
			if( countPerRow == 1 )
				return new Vector2I( Mode.ItemSize.X, Mode.ItemSize.Y * Items.Count );

			var y = Items.Count / countPerRow;
			if( y * countPerRow != Items.Count )
				y++;
			return new Vector2I( Mode.ItemSize.X * countPerRow, Mode.ItemSize.Y * y );
		}

		public System.Drawing.Rectangle GetItemRectangle_NoScrolling( int itemIndex )
		{
			//!!!!что со скроллом
			var clientWidth = GetClientRectangle().Width;

			if( Items.Count == 0 || clientWidth == 0 )
				return new System.Drawing.Rectangle( 0, 0, 0, 0 );

			//wide list mode
			if( Mode.ClampItemWidthByListViewWidth && Mode.ItemSize.X >= clientWidth )
				return new System.Drawing.Rectangle( 0, Mode.ItemSize.Y * itemIndex, clientWidth, Mode.ItemSize.Y );

			int countPerRow = Math.Max( clientWidth / Mode.ItemSize.X, 1 );
			var y = itemIndex / countPerRow;
			var x = itemIndex % countPerRow;
			return new System.Drawing.Rectangle( Mode.ItemSize.X * x, Mode.ItemSize.Y * y, Mode.ItemSize.X, Mode.ItemSize.Y );
		}

		public bool GetItemRectangle( int itemIndex, out System.Drawing.Rectangle rect )
		{
			return mode.GetItemRectangle( itemIndex, out rect );
		}

		public int GetItemCountPerRow()
		{
			//!!!!что со скроллом
			var clientWidth = GetClientRectangle().Width;

			int countPerRow;
			if( Mode.ItemSize.X != 0 )
				countPerRow = Math.Max( clientWidth / Mode.ItemSize.X, 1 );
			else
				countPerRow = 1;
			return countPerRow;
		}

		private void ScrollBarVertical_Scroll( object sender, EngineScrollBarEventArgs e )
		{
			//doubleBufferCompositedTime = DateTime.Now + TimeSpan.FromSeconds( 0.5 );
			//ControlDoubleBufferComposited.DisableComposited( this );

			Invalidate();
		}

		private void ScrollBarHorizontal_Scroll( object sender, EngineScrollBarEventArgs e )
		{
			//doubleBufferCompositedTime = DateTime.Now + TimeSpan.FromSeconds( 0.5 );
			//ControlDoubleBufferComposited.DisableComposited( this );

			Invalidate();
		}

		//private void timer10ms_Tick( object sender, EventArgs e )
		//{
		//	if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
		//		return;
		//	if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
		//		return;

		//	//if( doubleBufferCompositedTime.HasValue && ( DateTime.Now - doubleBufferCompositedTime.Value ).TotalSeconds > 0 )
		//	//{
		//	//	ControlDoubleBufferComposited.RestoreComposited( this );
		//	//	doubleBufferCompositedTime = null;
		//	//}
		//}

		[Browsable( false )]
		public EngineScrollBar ScrollBarVertical
		{
			get { return scrollBarVertical; }
		}

		[Browsable( false )]
		public EngineScrollBar ScrollBarHorizontal
		{
			get { return scrollBarHorizontal; }
		}

		public int GetItemIndex( Item item )
		{
			//!!!!slowly

			for( int n = 0; n < Items.Count; n++ )
				if( Items[ n ] == item )
					return n;
			return -1;
		}

		public void EnsureVisible( Item item )
		{
			//!!!!impl horizontal scroll

			var itemIndex = GetItemIndex( item );
			if( itemIndex == -1 )
				return;

			if( scrollBarVertical != null && scrollBarVertical.Visible )
			{
				var rect = GetItemRectangle_NoScrolling( itemIndex );

				var scrollMin = scrollBarVertical.Value;
				var scrollMax = scrollMin + GetClientRectangle().Height;

				var newValue = scrollBarVertical.Value;

				if( scrollMin > rect.Top )
					newValue = rect.Top;
				else if( scrollMax < rect.Bottom )
					newValue = rect.Bottom - GetClientRectangle().Height;

				MathEx.Clamp( ref newValue, 0, scrollBarVertical.Maximum );

				if( scrollBarVertical.Value != newValue )
				{
					scrollBarVertical.Value = newValue;
					Invalidate();
				}
			}
		}

		public bool CanDrag
		{
			get { return canDrag; }
			set { canDrag = value; }
		}



		//public static int GetOptimalIconSize( int requestedImageSize )
		//{
		//	if( requestedImageSize > 16 && requestedImageSize <= 20 )
		//		return 16;
		//	if( requestedImageSize > 32 && requestedImageSize <= 40 )
		//		return 32;
		//	return requestedImageSize;
		//}

		//!!!!

		//void UpdateScrollBarsList()
		//{
		//	//!!!!
		//	bool updating1 = engineScrollBarListVertical.MouseUpDownStatus;//&& engineScrollBar1.MouseScrollBarArea == EnhancedScrollBarMouseLocation.Thumb;
		//	if( !updating1 )
		//	{
		//		Win32.SCROLLINFO si = new Win32.SCROLLINFO();
		//		si.cbSize = (uint)Marshal.SizeOf( si );
		//		si.fMask = (int)Win32.ScrollInfoMask.SIF_ALL;
		//		if( Win32.GetScrollInfo( listView.Handle, (int)Win32.SBOrientation.SB_VERT, ref si ) )
		//		{
		//			//!!!!
		//			engineScrollBarListVertical.Maximum = Math.Max( si.nMax - kryptonSplitContainerListSub1.Panel1.Height, 0 );
		//			//engineScrollBarListVertical.Maximum = Math.Max( si.nMax - listView.ClientSize.Height, 0 );
		//			//engineScrollBarListVertical.Maximum = si.nMax;

		//			engineScrollBarListVertical.SmallChange = 30;
		//			engineScrollBarListVertical.LargeChange = kryptonSplitContainerListSub1.Panel1.Height;
		//			engineScrollBarListVertical.Value = si.nPos;
		//		}
		//	}

		//	//!!!!
		//	bool updating2 = engineScrollBarListHorizontal.MouseUpDownStatus;//&& engineScrollBar1.MouseScrollBarArea == EnhancedScrollBarMouseLocation.Thumb;
		//	if( !updating2 )
		//	{
		//		Win32.SCROLLINFO si = new Win32.SCROLLINFO();
		//		si.cbSize = (uint)Marshal.SizeOf( si );
		//		si.fMask = (int)Win32.ScrollInfoMask.SIF_ALL;
		//		if( Win32.GetScrollInfo( listView.Handle, (int)Win32.SBOrientation.SB_HORZ, ref si ) )
		//		{
		//			//!!!!
		//			engineScrollBarListHorizontal.Maximum = Math.Max( si.nMax - kryptonSplitContainerListSub1.Panel1.Width, 0 );
		//			//engineScrollBarListHorizontal.Maximum = Math.Max( si.nMax - listView.ClientSize.Width, 0 );
		//			//engineScrollBarListHorizontal.Maximum = si.nMax;

		//			engineScrollBarListHorizontal.SmallChange = 30;
		//			engineScrollBarListHorizontal.LargeChange = kryptonSplitContainerListSub1.Panel1.Width;
		//			engineScrollBarListHorizontal.Value = si.nPos;
		//		}
		//	}

		//	int wndStyle = Win32.GetWindowLong( listView.Handle, Win32.GWL_STYLE );
		//	bool hVisible = ( wndStyle & Win32.WS_HSCROLL ) != 0;
		//	bool vVisible = ( wndStyle & Win32.WS_VSCROLL ) != 0;

		//	if( listMode == ListModeEnum.List || engineScrollBarListHorizontal.Maximum == 0 )
		//		hVisible = false;
		//	if( engineScrollBarListVertical.Maximum == 0 )
		//		vVisible = false;

		//	kryptonSplitContainerList.Panel2Collapsed = !hVisible;
		//	kryptonSplitContainerListSub1.Panel2Collapsed = !vVisible;
		//	kryptonSplitContainerListSub2.Panel2Collapsed = !vVisible;

		//	//additional hide. чтобы не вышло так, что оригинальный виден, скиновый не виден
		//	if( !vVisible )
		//		Win32.ShowScrollBar( listView.Handle, (int)Win32.SBOrientation.SB_VERT, false );
		//	if( !hVisible )
		//		Win32.ShowScrollBar( listView.Handle, (int)Win32.SBOrientation.SB_HORZ, false );

		//	//bool vVisible = false;
		//	//{
		//	//	Win32.SCROLLBARINFO psbi = new Win32.SCROLLBARINFO();
		//	//	psbi.cbSize = Marshal.SizeOf( psbi );
		//	//	int nResult = Win32.GetScrollBarInfo( listView.Handle, Win32.OBJID_VSCROLL, ref psbi );
		//	//	if( nResult != 0 )
		//	//		vVisible = true;
		//	//}

		//	//bool hVisible = false;
		//	//{
		//	//	Win32.SCROLLBARINFO psbi = new Win32.SCROLLBARINFO();
		//	//	psbi.cbSize = Marshal.SizeOf( psbi );
		//	//	int nResult = Win32.GetScrollBarInfo( listView.Handle, Win32.OBJID_HSCROLL, ref psbi );
		//	//	if( nResult != 0 )
		//	//		hVisible = true;
		//	//}

		//	//Win32.ShowScrollBar( listView.Handle, (int)Win32.SBOrientation.SB_VERT, false );
		//	//Win32.ShowScrollBar( listView.Handle, (int)Win32.SBOrientation.SB_HORZ, false );
		//}


		//void UpdateSize()
		//{
		//var panel = kryptonSplitContainerListSub1.Panel1;

		////!!!!

		//int add = 0;
		//if( ( Time.Current - lastResizeTime ) < 0.5 )
		//	add = 20;// 100;
		//listView.SetBounds( 0, 0,
		//	panel.Width + SystemInformation.VerticalScrollBarWidth + 1 + add,
		//	panel.Height + SystemInformation.HorizontalScrollBarHeight + 1 + add );

		////int add = 0;
		////if( ( Time.Current - lastResizeTime ) < 0.5 )
		////	add = 100;
		////listView.SetBounds( 0, 0,
		////	panel.Width + ( engineScrollBarListVertical.Visible ? SystemInformation.VerticalScrollBarWidth : 0 ) + 1 + add,
		////	panel.Height + ( engineScrollBarListHorizontal.Visible ? SystemInformation.HorizontalScrollBarHeight : 0 ) + 1 + add );

		////listView.SetBounds( 0, 0, panel.Width + SystemInformation.VerticalScrollBarWidth + 1, panel.Height + SystemInformation.HorizontalScrollBarHeight + 1 );

		////listView.SetBounds( 0, 0, panel.Width, panel.Height );

		////!!!!было
		////!!!!?
		////kryptonPanel.Size = ClientSize - new Size( kryptonPanel.Location.X, kryptonPanel.Location.Y );
		//}


		//		class ContentBrowserDropSink : SimpleDropSink
		//		{
		//			public ContentBrowserDropSink()
		//			{
		//				CanDropBetween = true;
		//				FeedbackColor = Color.Black;
		//			}

		//#if !ANDROID

		//			protected override void DrawBetweenLine( Graphics g, int x1, int y1, int x2, int y2 )
		//			{
		//				//if( ColumnIsPrimary && CellHorizontalAlignment == HorizontalAlignment.Left )
		//				{
		//					x1 += 3;
		//					x2 -= 3;
		//				}

		//				using( Pen p = new Pen( this.FeedbackColor, 3.0f ) )
		//					g.DrawLine( p, x1, y1, x2, y2 );
		//			}

		//			protected override void DrawFeedbackBackgroundTarget( Graphics g, System.Drawing.Rectangle bounds )
		//			{
		//				float penWidth = 12.0f;
		//				var r = bounds;
		//				r.Inflate( (int)-penWidth / 2, (int)-penWidth / 2 );
		//				using( Pen p = new Pen( Color.FromArgb( 128, this.FeedbackColor ), penWidth ) )
		//					g.DrawRectangle( p, r );
		//			}

		//			protected override void DrawFeedbackItemTarget( Graphics g, System.Drawing.Rectangle bounds )
		//			{
		//#if HIGHLIGHT_ITEM_AT_DRAG
		//			if( this.DropTargetItem == null )
		//				return;

		//			var r = this.CalculateDropTargetRectangle( this.DropTargetItem, this.DropTargetSubItemIndex );

		//			//if( ColumnIsPrimary && CellHorizontalAlignment == HorizontalAlignment.Left )
		//			{
		//				r.X += 3;
		//				r.Width -= 3;
		//			}

		//			using( SolidBrush b = new SolidBrush( Color.FromArgb( 48, this.FeedbackColor ) ) )
		//				g.FillRectangle( b, r );
		//			//using( Pen p = new Pen( this.FeedbackColor, 3.0f ) )
		//			//	g.DrawRectangle( p, r );
		//#endif
		//			}

		//#endif //!ANDROID

		//		}


	}
}

#endif