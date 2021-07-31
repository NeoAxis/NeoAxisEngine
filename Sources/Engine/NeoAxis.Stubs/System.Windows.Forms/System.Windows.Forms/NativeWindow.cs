namespace System.Windows.Forms
{
	public class NativeWindow : MarshalByRefObject, IWin32Window
	{
		public IntPtr Handle
		{
			get
			{
				throw null;
			}
		}

		public NativeWindow()
		{
			throw null;
		}

		~NativeWindow()
		{
			throw null;
		}

		public void AssignHandle(IntPtr handle)
		{
			throw null;
		}

		public virtual void CreateHandle(CreateParams cp)
		{
			throw null;
		}

		public void DefWndProc(ref Message m)
		{
			throw null;
		}

		public virtual void DestroyHandle()
		{
			throw null;
		}

		public static NativeWindow FromHandle(IntPtr handle)
		{
			throw null;
		}

		protected virtual void OnHandleChange()
		{
			throw null;
		}

		protected virtual void OnThreadException(Exception e)
		{
			throw null;
		}

		public virtual void ReleaseHandle()
		{
			throw null;
		}

		protected virtual void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
