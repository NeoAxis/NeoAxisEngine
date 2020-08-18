using Microsoft.Win32;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
	public sealed class Application
	{
		public delegate bool MessageLoopCallback();

		public static bool AllowQuit
		{
			get
			{
				throw null;
			}
		}

		public static RegistryKey CommonAppDataRegistry
		{
			get
			{
				throw null;
			}
		}

		public static string CommonAppDataPath
		{
			get
			{
				throw null;
			}
		}

		public static string CompanyName
		{
			get
			{
				throw null;
			}
		}

		public static CultureInfo CurrentCulture
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

		public static InputLanguage CurrentInputLanguage
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

		public static string ExecutablePath
		{
			get
			{
				throw null;
			}
		}

		public static string LocalUserAppDataPath
		{
			get
			{
				throw null;
			}
		}

		public static bool MessageLoop
		{
			get
			{
				throw null;
			}
		}

		public static FormCollection OpenForms
		{
			get
			{
				throw null;
			}
		}

		public static string ProductName
		{
			get
			{
				throw null;
			}
		}

		public static string ProductVersion
		{
			get
			{
				throw null;
			}
		}

		public static bool RenderWithVisualStyles
		{
			get
			{
				throw null;
			}
		}

		public static string SafeTopLevelCaptionFormat
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

		public static string StartupPath
		{
			get
			{
				throw null;
			}
		}

		public static bool UseWaitCursor
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

		public static string UserAppDataPath
		{
			get
			{
				throw null;
			}
		}

		public static RegistryKey UserAppDataRegistry
		{
			get
			{
				throw null;
			}
		}

		public static VisualStyleState VisualStyleState
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

		public static event EventHandler ApplicationExit
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

		public static event EventHandler Idle
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

		public static event EventHandler EnterThreadModal
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

		public static event EventHandler LeaveThreadModal
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

		public static event ThreadExceptionEventHandler ThreadException
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

		public static event EventHandler ThreadExit
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

		public static void RegisterMessageLoop(MessageLoopCallback callback)
		{
			throw null;
		}

		public static void UnregisterMessageLoop()
		{
			throw null;
		}

		public static void AddMessageFilter(IMessageFilter value)
		{
			throw null;
		}

		public static bool FilterMessage(ref Message message)
		{
			throw null;
		}

		public static void DoEvents()
		{
			throw null;
		}

		public static void EnableVisualStyles()
		{
			throw null;
		}

		public static void Exit()
		{
			throw null;
		}

		public static void Exit(CancelEventArgs e)
		{
			throw null;
		}

		public static void ExitThread()
		{
			throw null;
		}

		public static ApartmentState OleRequired()
		{
			throw null;
		}

		public static void OnThreadException(Exception t)
		{
			throw null;
		}

		public static void RaiseIdle(EventArgs e)
		{
			throw null;
		}

		public static void RemoveMessageFilter(IMessageFilter value)
		{
			throw null;
		}

		public static void Restart()
		{
			throw null;
		}

		public static void Run()
		{
			throw null;
		}

		public static void Run(Form mainForm)
		{
			throw null;
		}

		public static void Run(ApplicationContext context)
		{
			throw null;
		}

		public static void SetCompatibleTextRenderingDefault(bool defaultValue)
		{
			throw null;
		}

		public static bool SetSuspendState(PowerState state, bool force, bool disableWakeEvent)
		{
			throw null;
		}

		public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode)
		{
			throw null;
		}

		public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode, bool threadScope)
		{
			throw null;
		}
	}
}
