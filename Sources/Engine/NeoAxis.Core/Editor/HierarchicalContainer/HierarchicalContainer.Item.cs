// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using ComponentFactory.Krypton.Toolkit;
using System.Diagnostics;

namespace NeoAxis.Editor
{
	public partial class HierarchicalContainer
	{
		/// <summary>
		/// Represents an item of the <see cref="HierarchicalContainer"/>.
		/// </summary>
		public abstract class Item : IDisposable
		{
			HierarchicalContainer owner;
			Item parent;
			List<Item> children = new List<Item>();

			EUserControl createdControl;

			bool canExpand;
			bool expanded;
			bool wasExpanded;

			//

			public Item( HierarchicalContainer owner, Item parent )
			{
				this.owner = owner;
				this.parent = parent;
			}

			public HierarchicalContainer Owner
			{
				get { return owner; }
			}

			public Item Parent
			{
				get { return parent; }
			}

			public List<Item> Children
			{
				get { return children; }
			}

			internal int GetTotalHeight()
			{
				int result = CreatedControl?.Height ?? 0;
				if( Expanded )
				{
					foreach( var item in Children )
						result += item.GetTotalHeight();
				}
				return result;
			}

			public abstract EUserControl CreateControlImpl();

			public virtual void UpdateControl() { }

			// update structure
			public virtual void Update()
			{
				foreach( var item in Children )
					item.Update();
			}

			public virtual void CreateControl()
			{
				if( CreatedControl == null )
				{

					try
					{
						// disable layout optimization
						KryptonToolkitSettings.DisableLayout = true;
						CreatedControl = CreateControlImpl();
					}
					catch( Exception )
					{
						//TODO: this is not fatal, we can disable item and show message.
						throw;
					}
					finally
					{
						KryptonToolkitSettings.DisableLayout = false;
					}

					Owner.ContentPanel.Controls.Add( CreatedControl );

					CreatedItemsCount++;

					unsafe
					{
						unchecked
						{
							CreatedControlsCount += CreatedControl.GetTotalControlsCount();
						}
					}
				}

				foreach( var item in Children )
					item.CreateControl();
			}

			// update layout
			public virtual void UpdateLayout( ref int positionY, ref int tabIndex, bool needVerticalScroll )
			{
				if( VisibleDependingExpandedFlag )
				{
					if( CreatedControl != null )
					{
						CreatedControl.SuspendLayout();

						var newLocation = new Point( 0, positionY );
						//var newLocation = new Point( 0, positionY - Owner.ScrollBarPosition );

						//var newLocation = new Point( 0, positionY - Owner.VerticalScroll.Value );

						if( CreatedControl.Location != newLocation )
							CreatedControl.Location = newLocation;

						int scrollBarOffset = 0;
						if( /*!Owner.VerticalScroll.Visible && */needVerticalScroll )
							scrollBarOffset = Owner.ScrollBarWidth + 1;

						if( CreatedControl.Width != Owner.ClientSize.Width - scrollBarOffset )
							CreatedControl.Width = Owner.ClientSize.Width - scrollBarOffset;

						//var newLocation = new Point( 0, positionY - Owner.VerticalScroll.Value );
						//if( CreatedControl.Location != newLocation )
						//	CreatedControl.Location = newLocation;

						//int scrollBarOffset = 0;
						//if( !Owner.VerticalScroll.Visible && needVerticalScroll )
						//	scrollBarOffset = SystemInformation.VerticalScrollBarWidth;

						//if( CreatedControl.Width != Owner.ClientSize.Width - scrollBarOffset )
						//	CreatedControl.Width = Owner.ClientSize.Width - scrollBarOffset;



						UpdateControl();

						if( !CreatedControl.Visible )
							CreatedControl.Visible = true;
						CreatedControl.ResumeLayout( true );

						positionY += CreatedControl.Height;

						if( CreatedControl.TabIndex != tabIndex )
							CreatedControl.TabIndex = tabIndex;
						tabIndex++;
					}
				}
				else
				{
					if( CreatedControl != null && CreatedControl.Visible )
						CreatedControl.Visible = false;
				}

				foreach( var item in Children )
					item.UpdateLayout( ref positionY, ref tabIndex, needVerticalScroll );
			}

			public virtual void Dispose()
			{
				foreach( var child in Children.ToArray() )
					child.Dispose();
				Children.Clear();

				if( CreatedControl != null )
				{
					if( CreatedControl.Parent != null )
						CreatedControl.Parent.Controls.Remove( CreatedControl );
					CreatedControl.Dispose();
					CreatedControl = null;
				}
			}

			public EUserControl CreatedControl
			{
				get { return createdControl; }
				set { createdControl = value; }
			}

			public bool CanExpand
			{
				get { return canExpand; }
				set { canExpand = value; }
			}

			public bool Expanded
			{
				get { return expanded; }
				set
				{
					expanded = value;
					if( expanded )
						wasExpanded = true;
				}
			}

			public bool WasExpanded
			{
				get { return wasExpanded; }
				set { wasExpanded = value; }
			}

			public bool VisibleDependingExpandedFlag
			{
				get
				{
					if( Parent != null )
						return Parent.VisibleDependingExpandedFlag && Parent.Expanded;
					return true;
				}
			}
		}
	}
}
