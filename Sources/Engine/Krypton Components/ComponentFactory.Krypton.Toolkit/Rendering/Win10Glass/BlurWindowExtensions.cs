using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//Seb add
namespace ComponentFactory.Krypton.Toolkit
{
    /// <summary>
    /// Manages the Windows blur
    /// </summary>
    public static class BlurWindowExtensions
    {
        static class Interop
        {
            [DllImport("user32.dll")]
            internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttribData data);

            [StructLayout(LayoutKind.Sequential)]
            internal struct WindowCompositionAttribData
            {
                public WindowCompositionAttribute Attribute;
                public IntPtr Data;
                public int SizeOfData;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct AccentPolicy
            {
                public AccentState AccentState;
                public AccentFlags AccentFlags;
                public int GradientColor;
                public int AnimationId;
            }

            [Flags]
            internal enum AccentFlags
            {
                // ...
                DrawLeftBorder = 0x20,
                DrawTopBorder = 0x40,
                DrawRightBorder = 0x80,
                DrawBottomBorder = 0x100,
                DrawAllBorders = (DrawLeftBorder | DrawTopBorder | DrawRightBorder | DrawBottomBorder)
                // ...
            }

            internal enum WindowCompositionAttribute
            {
                // ...
                WCA_ACCENT_POLICY = 19
                // ...
            }

            internal enum AccentState
            {
                ACCENT_DISABLED = 0,
                ACCENT_ENABLE_GRADIENT = 1,
                ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
                ACCENT_ENABLE_BLURBEHIND = 3,
                ACCENT_INVALID_STATE = 4
            }
        }

        public static void EnableBlur(this Form window)
        {
            if (SystemInformation.HighContrast)
            {
                return; // Blur is not useful in high contrast mode
            }

            SetAccentPolicy(window, Interop.AccentState.ACCENT_ENABLE_BLURBEHIND);
        }

        public static void DisableBlur(this Form window)
        {
            SetAccentPolicy(window, Interop.AccentState.ACCENT_DISABLED);
        }

        private static void SetAccentPolicy(Form window, Interop.AccentState accentState)
        {
           // var windowHelper = new WindowInteropHelper(window);

            var accent = new Interop.AccentPolicy();
            accent.AccentState = accentState;
            accent.AccentFlags = GetAccentFlagsForTaskbarPosition();

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new Interop.WindowCompositionAttribData();
            data.Attribute = Interop.WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            Interop.SetWindowCompositionAttribute(window.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        private static Interop.AccentFlags GetAccentFlagsForTaskbarPosition()
        {
            var flags = Interop.AccentFlags.DrawAllBorders;

            //switch (TaskbarService.TaskbarPosition)
            //{
            //    case TaskbarPosition.Top:
            //        flags &= ~Interop.AccentFlags.DrawTopBorder;
            //        break;

            //    case TaskbarPosition.Bottom:
            //        flags &= ~Interop.AccentFlags.DrawBottomBorder;
            //        break;

            //    case TaskbarPosition.Left:
            //        flags &= ~Interop.AccentFlags.DrawLeftBorder;
            //        break;

            //    case TaskbarPosition.Right:
            //        flags &= ~Interop.AccentFlags.DrawRightBorder;
            //        break;
            //}

            return flags;
        }
    }
}
