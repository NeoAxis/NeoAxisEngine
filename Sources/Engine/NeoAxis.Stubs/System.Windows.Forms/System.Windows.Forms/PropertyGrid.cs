using Microsoft.Win32;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
	public class PropertyGrid : ContainerControl, IComPropertyBrowser
	{
		public class PropertyTabCollection : ICollection, IEnumerable
		{
			public int Count
			{
				get
				{
					throw null;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					throw null;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					throw null;
				}
			}

			public PropertyTab this[int index]
			{
				get
				{
					throw null;
				}
			}

			public void AddTabType(Type propertyTabType)
			{
				throw null;
			}

			public void AddTabType(Type propertyTabType, PropertyTabScope tabScope)
			{
				throw null;
			}

			public void Clear(PropertyTabScope tabScope)
			{
				throw null;
			}

			void ICollection.CopyTo(Array dest, int index)
			{
				throw null;
			}

			public IEnumerator GetEnumerator()
			{
				throw null;
			}

			public void RemoveTabType(Type propertyTabType)
			{
				throw null;
			}
		}

		public override bool AutoScroll
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public override Color BackColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public override Image BackgroundImage
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public override ImageLayout BackgroundImageLayout
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public AttributeCollection BrowsableAttributes
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public virtual bool CanShowCommands
		{
			get
			{
				throw null;
			}
		}

		public Color CategoryForeColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color CommandsBackColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color CommandsForeColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color CommandsLinkColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color CommandsActiveLinkColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color CommandsDisabledLinkColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color CommandsBorderColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public virtual bool CommandsVisible
		{
			get
			{
				throw null;
			}
		}

		public virtual bool CommandsVisibleIfAvailable
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Point ContextMenuDefaultLocation
		{
			get
			{
				throw null;
			}
		}

		public new ControlCollection Controls
		{
			get
			{
				throw null;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		protected virtual Type DefaultTabType
		{
			get
			{
				throw null;
			}
		}

		protected bool DrawFlatToolbar
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public override Color ForeColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color HelpBackColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color HelpForeColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color HelpBorderColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public virtual bool HelpVisible
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color SelectedItemWithFocusBackColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color SelectedItemWithFocusForeColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color DisabledItemForeColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color CategorySplitterColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public bool CanShowVisualStyleGlyphs
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		bool IComPropertyBrowser.InPropertySet
		{
			get
			{
				throw null;
			}
		}

		public Color LineColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public new Padding Padding
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public PropertySort PropertySort
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public PropertyTabCollection PropertyTabs
		{
			get
			{
				throw null;
			}
		}

		public object SelectedObject
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public object[] SelectedObjects
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public PropertyTab SelectedTab
		{
			get
			{
				throw null;
			}
		}

		public GridItem SelectedGridItem
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public override ISite Site
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public override string Text
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public bool LargeButtons
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public virtual bool ToolbarVisible
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		protected ToolStripRenderer ToolStripRenderer
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color ViewBackColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color ViewForeColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color ViewBorderColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		protected virtual Bitmap SortByPropertyImage
		{
			get
			{
				throw null;
			}
		}

		protected virtual Bitmap SortByCategoryImage
		{
			get
			{
				throw null;
			}
		}

		protected virtual Bitmap ShowPropertyPageImage
		{
			get
			{
				throw null;
			}
		}

		public bool UseCompatibleTextRendering
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public new event EventHandler BackgroundImageChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler BackgroundImageLayoutChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler ForeColorChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler PaddingChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler TextChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event KeyEventHandler KeyDown
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event KeyPressEventHandler KeyPress
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event KeyEventHandler KeyUp
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event MouseEventHandler MouseDown
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event MouseEventHandler MouseUp
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event MouseEventHandler MouseMove
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler MouseEnter
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler MouseLeave
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public event PropertyValueChangedEventHandler PropertyValueChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		event ComponentRenameEventHandler IComPropertyBrowser.ComComponentNameChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public event PropertyTabChangedEventHandler PropertyTabChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public event EventHandler PropertySortChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public event SelectedGridItemChangedEventHandler SelectedGridItemChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public event EventHandler SelectedObjectsChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public PropertyGrid()
		{
			throw null;
		}

		public void CollapseAllGridItems()
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected virtual PropertyTab CreatePropertyTab(Type tabType)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		void IComPropertyBrowser.DropDownDone()
		{
			throw null;
		}

		bool IComPropertyBrowser.EnsurePendingChangesCommitted()
		{
			throw null;
		}

		public void ExpandAllGridItems()
		{
			throw null;
		}

		void IComPropertyBrowser.HandleF4()
		{
			throw null;
		}

		void IComPropertyBrowser.LoadState(RegistryKey optRoot)
		{
			throw null;
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			throw null;
		}

		protected override void OnGotFocus(EventArgs e)
		{
			throw null;
		}

		protected override void ScaleCore(float dx, float dy)
		{
			throw null;
		}

		protected override void OnMouseDown(MouseEventArgs me)
		{
			throw null;
		}

		protected override void OnMouseMove(MouseEventArgs me)
		{
			throw null;
		}

		protected override void OnMouseUp(MouseEventArgs me)
		{
			throw null;
		}

		protected override void OnResize(EventArgs e)
		{
			throw null;
		}

		protected void OnComComponentNameChanged(ComponentRenameEventArgs e)
		{
			throw null;
		}

		protected void OnNotifyPropertyValueUIItemsChanged(object sender, EventArgs e)
		{
			throw null;
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			throw null;
		}

		protected virtual void OnPropertySortChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnPropertyTabChanged(PropertyTabChangedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelectedObjectsChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			throw null;
		}

		public override void Refresh()
		{
			throw null;
		}

		public void RefreshTabs(PropertyTabScope tabScope)
		{
			throw null;
		}

		public void ResetSelectedProperty()
		{
			throw null;
		}

		void IComPropertyBrowser.SaveState(RegistryKey optRoot)
		{
			throw null;
		}

		protected void ShowEventsButton(bool value)
		{
			throw null;
		}

		protected override void OnSystemColorsChanged(EventArgs e)
		{
			throw null;
		}

		protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
