using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
	public class ComponentTray : ScrollableControl, IExtenderProvider
	{
		public bool AutoArrange
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

		public int ComponentCount
		{
			get
			{
				throw null;
			}
		}

		public bool ShowLargeIcons
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

		public ComponentTray(IDesigner mainDesigner, IServiceProvider serviceProvider)
		{
			throw null;
		}

		public virtual void AddComponent(IComponent component)
		{
			throw null;
		}

		bool IExtenderProvider.CanExtend(object component)
		{
			throw null;
		}

		protected virtual bool CanCreateComponentFromTool(ToolboxItem tool)
		{
			throw null;
		}

		protected virtual bool CanDisplayComponent(IComponent component)
		{
			throw null;
		}

		public void CreateComponentFromTool(ToolboxItem tool)
		{
			throw null;
		}

		protected void DisplayError(Exception e)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public IComponent GetNextComponent(IComponent component, bool forward)
		{
			throw null;
		}

		public Point GetLocation(IComponent receiver)
		{
			throw null;
		}

		public Point GetTrayLocation(IComponent receiver)
		{
			throw null;
		}

		protected override object GetService(Type serviceType)
		{
			throw null;
		}

		public bool IsTrayComponent(IComponent comp)
		{
			throw null;
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnGiveFeedback(GiveFeedbackEventArgs gfevent)
		{
			throw null;
		}

		protected override void OnDragDrop(DragEventArgs de)
		{
			throw null;
		}

		protected override void OnDragEnter(DragEventArgs de)
		{
			throw null;
		}

		protected override void OnDragLeave(EventArgs e)
		{
			throw null;
		}

		protected override void OnDragOver(DragEventArgs de)
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			throw null;
		}

		protected virtual void OnLostCapture()
		{
			throw null;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			throw null;
		}

		protected virtual void OnSetCursor()
		{
			throw null;
		}

		public virtual void RemoveComponent(IComponent component)
		{
			throw null;
		}

		public void SetLocation(IComponent receiver, Point location)
		{
			throw null;
		}

		public void SetTrayLocation(IComponent receiver, Point location)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
