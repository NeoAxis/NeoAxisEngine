using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
	public abstract class AxHost : Control, ISupportInitialize, ICustomTypeDescriptor
	{
		public sealed class ClsidAttribute : Attribute
		{
			public string Value
			{
				get
				{
					throw null;
				}
			}

			public ClsidAttribute(string clsid)
			{
				throw null;
			}
		}

		public sealed class TypeLibraryTimeStampAttribute : Attribute
		{
			public DateTime Value
			{
				get
				{
					throw null;
				}
			}

			public TypeLibraryTimeStampAttribute(string timestamp)
			{
				throw null;
			}
		}

		public class ConnectionPointCookie
		{
			public ConnectionPointCookie(object source, object sink, Type eventInterface)
			{
				throw null;
			}

			public void Disconnect()
			{
				throw null;
			}

			~ConnectionPointCookie()
			{
				throw null;
			}
		}

		public enum ActiveXInvokeKind
		{
			MethodInvoke,
			PropertyGet,
			PropertySet
		}

		public class InvalidActiveXStateException : Exception
		{
			public InvalidActiveXStateException(string name, ActiveXInvokeKind kind)
			{
				throw null;
			}

			public InvalidActiveXStateException()
			{
				throw null;
			}

			public override string ToString()
			{
				throw null;
			}
		}

		public class StateConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				throw null;
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				throw null;
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				throw null;
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				throw null;
			}

			public StateConverter()
			{
				throw null;
			}
		}

		[Serializable]
		public class State : ISerializable
		{
			public State(Stream ms, int storageType, bool manualUpdate, string licKey)
			{
				throw null;
			}

			protected State(SerializationInfo info, StreamingContext context)
			{
				throw null;
			}

			void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
			{
				throw null;
			}
		}

		protected delegate void AboutBoxDelegate();

		public class AxComponentEditor : WindowsFormsComponentEditor
		{
			public override bool EditComponent(ITypeDescriptorContext context, object obj, IWin32Window parent)
			{
				throw null;
			}

			public AxComponentEditor()
			{
				throw null;
			}
		}

		protected override CreateParams CreateParams
		{
			get
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

		public new ImeMode ImeMode
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

		public override Cursor Cursor
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

		public override ContextMenu ContextMenu
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

		protected override Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		public new virtual bool Enabled
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

		public override Font Font
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

		public new virtual bool RightToLeft
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

		public bool EditMode
		{
			get
			{
				throw null;
			}
		}

		public bool HasAboutBox
		{
			get
			{
				throw null;
			}
		}

		public override ISite Site
		{
			set
			{
				throw null;
			}
		}

		public State OcxState
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

		public ContainerControl ContainingControl
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

		public new event EventHandler MouseClick
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

		public new event EventHandler MouseDoubleClick
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

		public new event EventHandler BackColorChanged
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

		public new event EventHandler BindingContextChanged
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

		public new event EventHandler ContextMenuChanged
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

		public new event EventHandler CursorChanged
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

		public new event EventHandler EnabledChanged
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

		public new event EventHandler FontChanged
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

		public new event EventHandler RightToLeftChanged
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

		public new event EventHandler Click
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

		public new event DragEventHandler DragDrop
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

		public new event DragEventHandler DragEnter
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

		public new event DragEventHandler DragOver
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

		public new event EventHandler DragLeave
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

		public new event GiveFeedbackEventHandler GiveFeedback
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

		public new event HelpEventHandler HelpRequested
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

		public new event PaintEventHandler Paint
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

		public new event QueryContinueDragEventHandler QueryContinueDrag
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

		public new event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
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

		public new event EventHandler DoubleClick
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

		public new event EventHandler ImeModeChanged
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

		public new event LayoutEventHandler Layout
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

		public new event EventHandler MouseHover
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

		public new event MouseEventHandler MouseWheel
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

		public new event UICuesEventHandler ChangeUICues
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

		public new event EventHandler StyleChanged
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

		protected AxHost(string clsid)
		{
			throw null;
		}

		protected AxHost(string clsid, int flags)
		{
			throw null;
		}

		protected virtual void AttachInterfaces()
		{
			throw null;
		}

		protected bool PropsValid()
		{
			throw null;
		}

		public void BeginInit()
		{
			throw null;
		}

		public void EndInit()
		{
			throw null;
		}

		public void ShowAboutBox()
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnLostFocus(EventArgs e)
		{
			throw null;
		}

		public new void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
		{
			throw null;
		}

		protected override void CreateHandle()
		{
			throw null;
		}

		protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
		{
			throw null;
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			throw null;
		}

		protected override void DestroyHandle()
		{
			throw null;
		}

		protected virtual void OnInPlaceActive()
		{
			throw null;
		}

		protected override void SetVisibleCore(bool value)
		{
			throw null;
		}

		protected override bool IsInputChar(char charCode)
		{
			throw null;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			throw null;
		}

		public override bool PreProcessMessage(ref Message msg)
		{
			throw null;
		}

		protected void SetAboutBoxDelegate(AboutBoxDelegate d)
		{
			throw null;
		}

		public void DoVerb(int verb)
		{
			throw null;
		}

		protected virtual object CreateInstanceCore(Guid clsid)
		{
			throw null;
		}

		public void InvokeEditMode()
		{
			throw null;
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			throw null;
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			throw null;
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			throw null;
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			throw null;
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			throw null;
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			throw null;
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			throw null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			throw null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			throw null;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			throw null;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			throw null;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			throw null;
		}

		public object GetOcx()
		{
			throw null;
		}

		protected virtual void CreateSink()
		{
			throw null;
		}

		protected virtual void DetachSink()
		{
			throw null;
		}

		public bool HasPropertyPages()
		{
			throw null;
		}

		public void MakeDirty()
		{
			throw null;
		}

		public void ShowPropertyPages()
		{
			throw null;
		}

		public void ShowPropertyPages(Control control)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		protected static object GetIPictureFromPicture(Image image)
		{
			throw null;
		}

		protected static object GetIPictureFromCursor(Cursor cursor)
		{
			throw null;
		}

		protected static object GetIPictureDispFromPicture(Image image)
		{
			throw null;
		}

		protected static Image GetPictureFromIPicture(object picture)
		{
			throw null;
		}

		protected static Image GetPictureFromIPictureDisp(object picture)
		{
			throw null;
		}

		protected static Color GetColorFromOleColor(uint color)
		{
			throw null;
		}

		protected static uint GetOleColorFromColor(Color color)
		{
			throw null;
		}

		protected static object GetIFontFromFont(Font font)
		{
			throw null;
		}

		protected static Font GetFontFromIFont(object font)
		{
			throw null;
		}

		protected static object GetIFontDispFromFont(Font font)
		{
			throw null;
		}

		protected static Font GetFontFromIFontDisp(object font)
		{
			throw null;
		}

		protected static double GetOADateFromTime(DateTime time)
		{
			throw null;
		}

		protected static DateTime GetTimeFromOADate(double date)
		{
			throw null;
		}

		protected void RaiseOnMouseMove(object o1, object o2, object o3, object o4)
		{
			throw null;
		}

		protected void RaiseOnMouseMove(short button, short shift, float x, float y)
		{
			throw null;
		}

		protected void RaiseOnMouseMove(short button, short shift, int x, int y)
		{
			throw null;
		}

		protected void RaiseOnMouseUp(object o1, object o2, object o3, object o4)
		{
			throw null;
		}

		protected void RaiseOnMouseUp(short button, short shift, float x, float y)
		{
			throw null;
		}

		protected void RaiseOnMouseUp(short button, short shift, int x, int y)
		{
			throw null;
		}

		protected void RaiseOnMouseDown(object o1, object o2, object o3, object o4)
		{
			throw null;
		}

		protected void RaiseOnMouseDown(short button, short shift, float x, float y)
		{
			throw null;
		}

		protected void RaiseOnMouseDown(short button, short shift, int x, int y)
		{
			throw null;
		}
	}
}
