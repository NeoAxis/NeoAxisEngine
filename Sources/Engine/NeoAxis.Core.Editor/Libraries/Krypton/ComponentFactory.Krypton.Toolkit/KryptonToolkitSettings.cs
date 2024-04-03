#if !DEPLOY
using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
    public static class KryptonToolkitSettings
    {
        public static bool DisableLayout;

        public static NeoAxis.ProjectSettingsPage_General.CustomizeWindowsStyleEnum CustomizeWindowsStyle = NeoAxis.ProjectSettingsPage_General.CustomizeWindowsStyleEnum.Auto;

        static bool? windows11;

        static bool IsWindows11()
        {
            if( windows11 == null )
            {
                windows11 = false;

                try
                {
                    var kernelPath = Path.Combine( Environment.SystemDirectory, "Kernel32.dll" );
                    var kernel = FileVersionInfo.GetVersionInfo( kernelPath );

                    using( var hklm = RegistryKey.OpenBaseKey( RegistryHive.LocalMachine, RegistryView.Registry64 ) )
                    {
                        using( var version = hklm.OpenSubKey( @"Software\Microsoft\Windows NT\CurrentVersion", false ) )
                        {
                            if( kernel.ProductMajorPart == 10 && kernel.ProductBuildPart >= 22000 )
                                windows11 = true;
                        }
                    }
                }
                catch { }
            }
            return windows11.Value;

            //bug. fixed in .NET 5
            //var windows11OrMore = Environment.OSVersion.Version.Build >= 22000;
            //return windows11OrMore;
        }

        public static bool GetResultCustomizeWindowsStyle( bool mainForm )
        {
            var result = CustomizeWindowsStyle;

            if( result.HasFlag( NeoAxis.ProjectSettingsPage_General.CustomizeWindowsStyleEnum.Auto ) )
            {
                if( IsWindows11() )
                    result = NeoAxis.ProjectSettingsPage_General.CustomizeWindowsStyleEnum.MainForm;
                else
                    result = NeoAxis.ProjectSettingsPage_General.CustomizeWindowsStyleEnum.MainForm | NeoAxis.ProjectSettingsPage_General.CustomizeWindowsStyleEnum.AdditionalForms;
            }

            if( mainForm )
                return result.HasFlag( NeoAxis.ProjectSettingsPage_General.CustomizeWindowsStyleEnum.MainForm );
            else
                return result.HasFlag( NeoAxis.ProjectSettingsPage_General.CustomizeWindowsStyleEnum.AdditionalForms );
        }
    }
}

#endif