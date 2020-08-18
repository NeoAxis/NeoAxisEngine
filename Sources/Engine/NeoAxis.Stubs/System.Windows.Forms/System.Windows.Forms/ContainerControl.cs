using System.Drawing;

namespace System.Windows.Forms
{
	public class ContainerControl : ScrollableControl, IContainerControl
	{
		public SizeF AutoScaleDimensions
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

		protected SizeF AutoScaleFactor
		{
			get
			{
				throw null;
			}
		}

		public AutoScaleMode AutoScaleMode
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

		public virtual AutoValidate AutoValidate
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

		public override BindingContext BindingContext
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

		protected override bool CanEnableIme
		{
			get
			{
				throw null;
			}
		}

		public Control ActiveControl
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

		public SizeF CurrentAutoScaleDimensions
		{
			get
			{
				throw null;
			}
		}

		public Form ParentForm
		{
			get
			{
				throw null;
			}
		}

		public event EventHandler AutoValidateChanged
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

		public ContainerControl()
		{
			throw null;
		}

		bool IContainerControl.ActivateControl(Control control)
		{
			throw null;
		}

		protected override void AdjustFormScrollbars(bool displayScrollbars)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		protected virtual void OnAutoValidateChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnCreateControl()
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			throw null;
		}

		protected override void OnParentChanged(EventArgs e)
		{
			throw null;
		}

		public void PerformAutoScale()
		{
			throw null;
		}

		protected override bool ProcessDialogChar(char charCode)
		{
			throw null;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			throw null;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			throw null;
		}

		protected virtual bool ProcessTabKey(bool forward)
		{
			throw null;
		}

		protected override void Select(bool directed, bool forward)
		{
			throw null;
		}

		protected virtual void UpdateDefaultButton()
		{
			throw null;
		}

		public bool Validate()
		{
			throw null;
		}

		public bool Validate(bool checkAutoValidate)
		{
			throw null;
		}

		public virtual bool ValidateChildren()
		{
			throw null;
		}

		public virtual bool ValidateChildren(ValidationConstraints validationConstraints)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
