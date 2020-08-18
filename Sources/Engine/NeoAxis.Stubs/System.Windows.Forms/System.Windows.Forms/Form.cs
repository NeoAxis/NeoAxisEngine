using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class Form : ContainerControl
	{
		public new class ControlCollection : Control.ControlCollection
		{
			public ControlCollection(Form owner)
				:base(owner)
			{
				throw null;
			}

			public override void Add(Control value)
			{
				throw null;
			}

			public override void Remove(Control value)
			{
				throw null;
			}
		}

		public IButtonControl AcceptButton
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

		public static Form ActiveForm
		{
			get
			{
				throw null;
			}
		}

		public Form ActiveMdiChild
		{
			get
			{
				throw null;
			}
		}

		public bool AllowTransparency
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

		public bool AutoScale
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

		public virtual Size AutoScaleBaseSize
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

		public override bool AutoSize
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

		public AutoSizeMode AutoSizeMode
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

		public override AutoValidate AutoValidate
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

		public FormBorderStyle FormBorderStyle
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

		public IButtonControl CancelButton
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

		public new Size ClientSize
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

		public bool ControlBox
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

		protected override CreateParams CreateParams
		{
			get
			{
				throw null;
			}
		}

		protected override ImeMode DefaultImeMode
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

		public Rectangle DesktopBounds
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

		public Point DesktopLocation
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

		public DialogResult DialogResult
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

		public bool HelpButton
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

		public Icon Icon
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

		public bool IsMdiChild
		{
			get
			{
				throw null;
			}
		}

		public bool IsMdiContainer
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

		public bool IsRestrictedWindow
		{
			get
			{
				throw null;
			}
		}

		public bool KeyPreview
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

		public new Point Location
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

		protected Rectangle MaximizedBounds
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

		public override Size MaximumSize
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

		public MenuStrip MainMenuStrip
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

		public new Padding Margin
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

		public MainMenu Menu
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

		public override Size MinimumSize
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

		public bool MaximizeBox
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

		public Form[] MdiChildren
		{
			get
			{
				throw null;
			}
		}

		public Form MdiParent
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

		public MainMenu MergedMenu
		{
			get
			{
				throw null;
			}
		}

		public bool MinimizeBox
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

		public bool Modal
		{
			get
			{
				throw null;
			}
		}

		public double Opacity
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

		public Form[] OwnedForms
		{
			get
			{
				throw null;
			}
		}

		public Form Owner
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

		public Rectangle RestoreBounds
		{
			get
			{
				throw null;
			}
		}

		public virtual bool RightToLeftLayout
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

		public bool ShowInTaskbar
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

		public bool ShowIcon
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

		protected virtual bool ShowWithoutActivation
		{
			get
			{
				throw null;
			}
		}

		public new Size Size
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

		public SizeGripStyle SizeGripStyle
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

		public FormStartPosition StartPosition
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

		public new int TabIndex
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

		public new bool TabStop
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

		public bool TopLevel
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

		public bool TopMost
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

		public Color TransparencyKey
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

		public FormWindowState WindowState
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

		public new event EventHandler AutoSizeChanged
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

		public new event EventHandler AutoValidateChanged
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

		public event CancelEventHandler HelpButtonClicked
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

		public event EventHandler MaximizedBoundsChanged
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

		public event EventHandler MaximumSizeChanged
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

		public new event EventHandler MarginChanged
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

		public event EventHandler MinimumSizeChanged
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

		public new event EventHandler TabIndexChanged
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

		public new event EventHandler TabStopChanged
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

		public event EventHandler Activated
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

		public event CancelEventHandler Closing
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

		public event EventHandler Closed
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

		public event EventHandler Deactivate
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

		public event FormClosingEventHandler FormClosing
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

		public event FormClosedEventHandler FormClosed
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

		public event EventHandler Load
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

		public event EventHandler MdiChildActivate
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

		public event EventHandler MenuComplete
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

		public event EventHandler MenuStart
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

		public event InputLanguageChangedEventHandler InputLanguageChanged
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

		public event InputLanguageChangingEventHandler InputLanguageChanging
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

		public event EventHandler RightToLeftLayoutChanged
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

		public event EventHandler Shown
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

		public event DpiChangedEventHandler DpiChanged
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

		public event EventHandler ResizeBegin
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

		public event EventHandler ResizeEnd
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

		public Form()
		{
			throw null;
		}

		protected override void SetVisibleCore(bool value)
		{
			throw null;
		}

		public void Activate()
		{
			throw null;
		}

		protected void ActivateMdiChild(Form form)
		{
			throw null;
		}

		public void AddOwnedForm(Form ownedForm)
		{
			throw null;
		}

		protected override void AdjustFormScrollbars(bool displayScrollbars)
		{
			throw null;
		}

		protected void ApplyAutoScaling()
		{
			throw null;
		}

		public void Close()
		{
			throw null;
		}

		protected override Control.ControlCollection CreateControlsInstance()
		{
			throw null;
		}

		protected override void CreateHandle()
		{
			throw null;
		}

		protected override void DefWndProc(ref Message m)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public static SizeF GetAutoScaleSize(Font font)
		{
			throw null;
		}

		protected void CenterToParent()
		{
			throw null;
		}

		protected void CenterToScreen()
		{
			throw null;
		}

		public void LayoutMdi(MdiLayout value)
		{
			throw null;
		}

		protected virtual void OnActivated(EventArgs e)
		{
			throw null;
		}

		protected override void OnBackgroundImageChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnBackgroundImageLayoutChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnClosing(CancelEventArgs e)
		{
			throw null;
		}

		protected virtual void OnClosed(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnFormClosing(FormClosingEventArgs e)
		{
			throw null;
		}

		protected virtual void OnFormClosed(FormClosedEventArgs e)
		{
			throw null;
		}

		protected override void OnCreateControl()
		{
			throw null;
		}

		protected virtual void OnDeactivate(EventArgs e)
		{
			throw null;
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnEnter(EventArgs e)
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

		protected virtual void OnHelpButtonClicked(CancelEventArgs e)
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			throw null;
		}

		protected virtual void OnLoad(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMaximizedBoundsChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMaximumSizeChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMinimumSizeChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnInputLanguageChanged(InputLanguageChangedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnInputLanguageChanging(InputLanguageChangingEventArgs e)
		{
			throw null;
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMdiChildActivate(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMenuStart(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMenuComplete(EventArgs e)
		{
			throw null;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			throw null;
		}

		protected override void OnResize(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDpiChanged(DpiChangedEventArgs e)
		{
			throw null;
		}

		protected virtual bool OnGetDpiScaledSize(int deviceDpiOld, int deviceDpiNew, ref Size desiredSize)
		{
			throw null;
		}

		protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnShown(EventArgs e)
		{
			throw null;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			throw null;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			throw null;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			throw null;
		}

		protected override bool ProcessDialogChar(char charCode)
		{
			throw null;
		}

		protected override bool ProcessKeyPreview(ref Message m)
		{
			throw null;
		}

		protected override bool ProcessTabKey(bool forward)
		{
			throw null;
		}

		public void RemoveOwnedForm(Form ownedForm)
		{
			throw null;
		}

		protected override void Select(bool directed, bool forward)
		{
			throw null;
		}

		protected override void ScaleCore(float x, float y)
		{
			throw null;
		}

		protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
		{
			throw null;
		}

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			throw null;
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			throw null;
		}

		protected override void SetClientSizeCore(int x, int y)
		{
			throw null;
		}

		public void SetDesktopBounds(int x, int y, int width, int height)
		{
			throw null;
		}

		public void SetDesktopLocation(int x, int y)
		{
			throw null;
		}

		public void Show(IWin32Window owner)
		{
			throw null;
		}

		public DialogResult ShowDialog()
		{
			throw null;
		}

		public DialogResult ShowDialog(IWin32Window owner)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		protected override void UpdateDefaultButton()
		{
			throw null;
		}

		protected virtual void OnResizeBegin(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnResizeEnd(EventArgs e)
		{
			throw null;
		}

		protected override void OnStyleChanged(EventArgs e)
		{
			throw null;
		}

		public override bool ValidateChildren()
		{
			throw null;
		}

		public override bool ValidateChildren(ValidationConstraints validationConstraints)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
