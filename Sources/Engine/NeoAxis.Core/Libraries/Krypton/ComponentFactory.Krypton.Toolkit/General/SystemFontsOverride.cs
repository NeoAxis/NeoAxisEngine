using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ComponentFactory.Krypton.Toolkit
{
    /// <summary>
    /// 
    /// </summary>
    public static class SystemFontsOverride
    {
        /// <summary>
        /// 
        /// </summary>
        public static Font BaseFont { get; }
        /// <summary>
        /// 
        /// </summary>
        public static bool Override { get; }
        /// <summary>
        /// 
        /// </summary>
        public static Font CaptionFont { get; }
        /// <summary>
        /// 
        /// </summary>
        public static Font DefaultFont { get; }
        /// <summary>
        /// 
        /// </summary>
        public static Font DialogFont { get; }
        /// <summary>
        /// 
        /// </summary>
        public static Font IconTitleFont { get; }
        /// <summary>
        /// 
        /// </summary>
        public static Font MenuFont { get; }
        /// <summary>
        /// 
        /// </summary>
        public static Font MessageBoxFont { get; }
        /// <summary>
        /// 
        /// </summary>
        public static Font SmallCaptionFont { get; }
        /// <summary>
        /// 
        /// </summary>
        public static Font StatusFont { get; }

        static SystemFontsOverride()
        {
            Override = true;

            //TODO: should we create Font obj ? we can replace it by FontInfo with name and size.

            BaseFont = new Font("Segoe UI", 9);

            CaptionFont = Override ? new Font("Segoe UI", SystemFonts.CaptionFont.SizeInPoints) : SystemFonts.CaptionFont;
            DefaultFont = Override ? new Font("Microsoft Sans Serif", SystemFonts.DefaultFont.SizeInPoints) : SystemFonts.DefaultFont;
            DialogFont = Override ? new Font("Tahoma", SystemFonts.DialogFont.SizeInPoints) : SystemFonts.DialogFont;
            IconTitleFont = Override ? new Font("Segoe UI", SystemFonts.IconTitleFont.SizeInPoints) : SystemFonts.IconTitleFont;
            MenuFont = Override ? new Font("Segoe UI", SystemFonts.MenuFont.SizeInPoints) : SystemFonts.MenuFont;
            MessageBoxFont = Override ? new Font("Segoe UI", SystemFonts.MessageBoxFont.SizeInPoints) : SystemFonts.MessageBoxFont;
            SmallCaptionFont = Override ? new Font("Segoe UI", SystemFonts.SmallCaptionFont.SizeInPoints) : SystemFonts.SmallCaptionFont;
            StatusFont = Override ? new Font("Segoe UI", SystemFonts.StatusFont.SizeInPoints) : SystemFonts.StatusFont;
        }
    }
}
