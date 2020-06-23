// *****************************************************************************
// 
//  © Component Factory Pty Ltd 2012. All rights reserved.
//	The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, 17/267 Nepean Hwy, 
//  Seaford, Vic 3198, Australia and are supplied subject to licence terms.
// 
//
// *****************************************************************************

using System;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

namespace ComponentFactory.Krypton.Toolkit
{
	/// <summary>
	/// Provides the White color scheme variant of the Office 2016 palette.
	/// </summary>
    public class PaletteOffice2016Blue : PaletteOffice2016Base
    {
        #region Static Fields
        private static readonly Color _backColor = Color.FromArgb(240, 240, 240);

        private static readonly ImageList _checkBoxList;
        private static readonly ImageList _galleryButtonList;
        private static readonly Image[] _radioButtonArray;
        private static readonly Image _silverDropDownButton = Properties.Resources._2010BlueDropDownButton;
        private static readonly Image _contextMenuSubMenu = Properties.Resources._2010BlueContextMenuSub;
        private static readonly Image _formClose = Properties.Resources._2016ButtonClose;
        private static readonly Image _formMax = Properties.Resources._2016ButtonMax;
        private static readonly Image _formMin = Properties.Resources._2016ButtonMin;
        private static readonly Image _formRestore = Properties.Resources._2016ButtonRestore;
        private static readonly Color[] _trackBarColors = new Color[] { Color.FromArgb(196, 196, 196),  // Tick marks
                                                                        Color.FromArgb(196, 196, 196),  // Top track
                                                                        Color.FromArgb(196, 196, 196),  // Bottom track
                                                                        Color.FromArgb(196, 196, 196),  // Fill track
                                                                        Color.FromArgb(177, 214, 240),  // Fill position (pressed state)
                                                                        Color.Magenta                   // --
                                                                      };
        private static readonly Color[] _schemeColors = new Color[] { Color.FromArgb( 59,  59,  59),    // TextLabelControl
                                                                      Color.FromArgb( 59,  59,  59),    // TextButtonNormal
                                                                      Color.Black,                      // TextButtonChecked
                                                                      Color.FromArgb(170, 170, 170),    // ButtonNormalBorder1
                                                                      Color.FromArgb(170, 170, 170),    // ButtonNormalDefaultBorder
                                                                      Color.FromArgb(253, 253, 253),    // ButtonNormalBack1
                                                                      Color.Red,                        // ButtonNormalBack2 -n
                                                                      Color.FromArgb(253, 253, 253),    // ButtonNormalDefaultBack1
                                                                      Color.Red,                        // ButtonNormalDefaultBack2 -n
                                                                      Color.FromArgb(207, 212, 218),    // ButtonNormalNavigatorBack1
                                                                      Color.FromArgb(207, 212, 218),    // ButtonNormalNavigatorBack2
                                                                      _backColor,                       // PanelClient
                                                                      _backColor,                       // PanelAlternative
                                                                      Color.FromArgb(213, 213, 213),    // ControlBorder
                                                                      Color.FromArgb(250, 253, 255),    // SeparatorHighBorder1
                                                                      Color.FromArgb(227, 232, 237),    // SeparatorHighBorder2
                                                                      Color.FromArgb(255, 255, 255),    // HeaderPrimaryBack1 -n
                                                                      Color.FromArgb(255, 255, 255),    // HeaderPrimaryBack2 -n
                                                                      Color.FromArgb(255, 255, 255),    // HeaderSecondaryBack1
                                                                      Color.FromArgb(255, 255, 255),    // HeaderSecondaryBack2-n
                                                                      Color.FromArgb( 59,  59,  59),    // HeaderText
                                                                      Color.FromArgb( 59,  59,  59),    // StatusStripText
                                                                      Color.Red,                        // ButtonBorder
                                                                      Color.Red,                        // SeparatorLight -n
                                                                      Color.FromArgb(210, 210, 210),    // SeparatorDark
                                                                      Color.FromArgb(191, 191, 191),    // GripLight
                                                                      Color.FromArgb(191, 191, 191),    // GripDark
                                                                      _backColor,                       // ToolStripBack
                                                                      Color.FromArgb(0  , 114, 198),    // StatusStripLight
                                                                      Color.FromArgb(0  , 114, 198),    // StatusStripDark
                                                                      Color.White,                      // ImageMargin
                                                                      _backColor,                       // ToolStripBegin
                                                                      _backColor,                       // ToolStripMiddle
                                                                      _backColor,                       // ToolStripEnd
                                                                      _backColor,                       // OverflowBegin
                                                                      _backColor,                       // OverflowMiddle
                                                                      _backColor,                       // OverflowEnd
                                                                      Color.Empty,                      // ToolStripBorder -n
                                                                      Color.FromArgb(0  , 114, 198),    // FormBorderActive
                                                                      Color.FromArgb(131, 131, 131),    // FormBorderInactive
                                                                      Color.FromArgb(228, 230, 232),    // FormBorderActiveLight
                                                                      Color.FromArgb(255, 255, 255),    // FormBorderActiveDark
                                                                      Color.FromArgb(248, 247, 247),    // FormBorderInactiveLight
                                                                      Color.FromArgb(248, 247, 247),    // FormBorderInactiveDark
                                                                      Color.FromArgb(101, 109, 117),    // FormBorderHeaderActive
                                                                      Color.FromArgb(131, 131, 131),    // FormBorderHeaderInactive
                                                                      Color.FromArgb(42 , 87,  154),    // FormBorderHeaderActive1
                                                                      Color.FromArgb(228, 230, 232),    // FormBorderHeaderActive2
                                                                      Color.FromArgb(42 , 87,  154),    // FormBorderHeaderInctive1
                                                                      Color.FromArgb(248, 247, 247),    // FormBorderHeaderInctive2
                                                                      Color.FromArgb(0  , 114, 198),    // FormShadowActive
                                                                      Color.FromArgb(131, 131, 131),    // FormShadowInactive
                                                                      Color.FromArgb(255, 255, 255),    // FormHeaderShortActive
                                                                      Color.FromArgb(165, 185, 210),    // FormHeaderShortInactive
                                                                      Color.FromArgb(255, 255, 255),    // FormHeaderLongActive
                                                                      Color.FromArgb(165, 185, 210),    // FormHeaderLongInactive
                                                                      Color.Empty,                      // FormButtonBorderTrack
                                                                      Color.FromArgb(62, 109, 182),     // FormButtonBack1Track
                                                                      Color.Empty,						// FormButtonBack2Track
                                                                      Color.Empty,                      // FormButtonBorderPressed
                                                                      Color.FromArgb(19,  64,  119),    // FormButtonBack1Pressed
                                                                      Color.Empty,                      // FormButtonBack2Pressed
                                                                      Color.Black,                      // TextButtonFormNormal
                                                                      Color.Black,                      // TextButtonFormTracking
                                                                      Color.Black,                      // TextButtonFormPressed
                                                                      Color.Blue,                       // LinkNotVisitedOverrideControl
                                                                      Color.Purple,                     // LinkVisitedOverrideControl
                                                                      Color.Red,                        // LinkPressedOverrideControl
                                                                      Color.Blue,                       // LinkNotVisitedOverridePanel
                                                                      Color.Purple,                     // LinkVisitedOverridePanel
                                                                      Color.Red,                        // LinkPressedOverridePanel
                                                                      Color.FromArgb( 59,  59,  59),    // TextLabelPanel
                                                                      Color.FromArgb(255, 255, 255),    // RibbonTabTextNormal -n
                                                                      Color.FromArgb(42, 87, 154),      // RibbonTabTextChecked -n
                                                                      _backColor,                       // RibbonTabSelected1
                                                                      Color.Empty,                      // RibbonTabSelected2
                                                                      Color.Empty,                      // RibbonTabSelected3
                                                                      Color.Empty,                      // RibbonTabSelected4
                                                                      Color.Empty,                      // RibbonTabSelected5
                                                                      Color.FromArgb(62, 109, 182),     // RibbonTabTracking1
                                                                      Color.Empty,						// RibbonTabTracking2
                                                                      Color.FromArgb(255, 0, 0),        // RibbonTabHighlight1
                                                                      Color.Empty,                      // RibbonTabHighlight2
                                                                      Color.Empty,                      // RibbonTabHighlight3
                                                                      Color.Empty,                      // RibbonTabHighlight4
                                                                      Color.Empty,                      // RibbonTabHighlight5
                                                                      Color.Empty,//Color.FromArgb(255, 0, 0),        // RibbonTabSeparatorColor

                                                                      _backColor,                       // RibbonGroupsArea1 -n
                                                                      Color.FromArgb(213, 213, 213),    // RibbonGroupsArea2 -n
                                                                      Color.Empty,						// RibbonGroupsArea3 -n
                                                                      Color.Empty,                      // RibbonGroupsArea4 -n
                                                                      Color.Empty,                      // RibbonGroupsArea5 -n

                                                                      Color.Empty,                      // RibbonGroupBorder1 -n
                                                                      Color.Empty,                      // RibbonGroupBorder2 -n
                                                                      Color.Empty,                      // RibbonGroupTitle1
                                                                      Color.Empty,                      // RibbonGroupTitle2
                                                                      Color.Empty,                      // RibbonGroupBorderContext1
                                                                      Color.Empty,                      // RibbonGroupBorderContext2
                                                                      Color.Empty,                      // RibbonGroupTitleContext1
                                                                      Color.Empty,                      // RibbonGroupTitleContext2
                                                                      Color.FromArgb(148, 149, 152),    // RibbonGroupDialogDark
                                                                      Color.FromArgb(180, 182, 183),    // RibbonGroupDialogLight
                                                                      Color.Empty,                      // RibbonGroupTitleTracking1
                                                                      Color.Empty,                      // RibbonGroupTitleTracking2
                                                                      Color.Empty,                      // RibbonMinimizeBarDark
                                                                      Color.Empty,                      // RibbonMinimizeBarLight
                                                                      Color.Empty,                      // RibbonGroupCollapsedBorder1
                                                                      Color.Empty,                      // RibbonGroupCollapsedBorder2
                                                                      Color.Empty,                      // RibbonGroupCollapsedBorder3
                                                                      Color.Empty,                      // RibbonGroupCollapsedBorder4
                                                                      Color.Empty,                      // RibbonGroupCollapsedBack1
                                                                      Color.Empty,                      // RibbonGroupCollapsedBack2
                                                                      Color.Empty,                      // RibbonGroupCollapsedBack3
                                                                      Color.Empty,                      // RibbonGroupCollapsedBack4
                                                                      Color.Empty,                      // RibbonGroupCollapsedBorderT1
                                                                      Color.Empty,                      // RibbonGroupCollapsedBorderT2
                                                                      Color.Empty,                      // RibbonGroupCollapsedBorderT3
                                                                      Color.Empty,                      // RibbonGroupCollapsedBorderT4
                                                                      Color.Empty,                      // RibbonGroupCollapsedBackT1
                                                                      Color.FromArgb(242, 244, 247),    // RibbonGroupCollapsedBackT2
                                                                      Color.FromArgb(238, 241, 245),    // RibbonGroupCollapsedBackT3
                                                                      Color.FromArgb(234, 235, 235),    // RibbonGroupCollapsedBackT4
                                                                      Color.FromArgb(208, 212, 217),    // RibbonGroupFrameBorder1
                                                                      Color.FromArgb(208, 212, 217),    // RibbonGroupFrameBorder2
                                                                      Color.FromArgb(254, 254, 254),    // RibbonGroupFrameInside1
                                                                      Color.FromArgb(254, 254, 254),    // RibbonGroupFrameInside2
                                                                      Color.Empty,                      // RibbonGroupFrameInside3
                                                                      Color.Empty,                      // RibbonGroupFrameInside4
                                                                      Color.FromArgb( 59,  59,  59),    // RibbonGroupCollapsedText         
                                                                      Color.FromArgb(179, 185, 195),    // AlternatePressedBack1
                                                                      Color.FromArgb(216, 224, 224),    // AlternatePressedBack2
                                                                      Color.FromArgb(125, 125, 125),    // AlternatePressedBorder1
                                                                      Color.FromArgb(186, 186, 186),    // AlternatePressedBorder2
                                                                      Color.FromArgb(62, 109, 182),	    // FormButtonBack1Checked
                                                                      Color.Empty,						// FormButtonBack2Checked -n
                                                                      Color.Empty,					    // FormButtonBorderCheck
                                                                      Color.FromArgb(62, 109, 182),		// FormButtonBack1CheckTrack
                                                                      Color.Empty,					    // FormButtonBack2CheckTrack -n
                                                                      Color.FromArgb(180, 180, 180),    // RibbonQATMini1
                                                                      Color.FromArgb(210, 215, 221),    // RibbonQATMini2
                                                                      Color.FromArgb(195, 200, 206),    // RibbonQATMini3
                                                                      Color.FromArgb(10, Color.White),  // RibbonQATMini4
                                                                      Color.FromArgb(32, Color.White),  // RibbonQATMini5                                                       
                                                                      Color.FromArgb(200, 200, 200),    // RibbonQATMini1I
                                                                      Color.FromArgb(233, 234, 238),    // RibbonQATMini2I
                                                                      Color.FromArgb(223, 224, 228),    // RibbonQATMini3I
                                                                      Color.FromArgb(10, Color.White),  // RibbonQATMini4I
                                                                      Color.FromArgb(32, Color.White),  // RibbonQATMini5I                                                       
                                                                      Color.Red,                        // RibbonQATFullbar1 -n                                                 
                                                                      Color.FromArgb(240, 240, 240),    // RibbonQATFullbar2                                                      
                                                                      Color.Red,                        // RibbonQATFullbar3 -n                                                   
                                                                      Color.FromArgb(59, 59, 59),	    // RibbonQATButtonDark                                                      
                                                                      Color.White,                      // RibbonQATButtonLight                                                      
                                                                      Color.FromArgb(233, 237, 241),    // RibbonQATOverflow1                                                      
                                                                      Color.FromArgb(138, 144, 150),    // RibbonQATOverflow2                                                      
                                                                      Color.FromArgb(191, 195, 199),    // RibbonGroupSeparatorDark                                                      
                                                                      Color.FromArgb(255, 255, 255),    // RibbonGroupSeparatorLight                                                      
                                                                      Color.FromArgb(231, 234, 238),    // ButtonClusterButtonBack1                                                      
                                                                      Color.FromArgb(241, 243, 243),    // ButtonClusterButtonBack2                                                      
                                                                      Color.FromArgb(197, 198, 199),    // ButtonClusterButtonBorder1                                                      
                                                                      Color.FromArgb(157, 158, 159),    // ButtonClusterButtonBorder2                                                      
                                                                      Color.FromArgb(238, 238, 244),    // NavigatorMiniBackColor                                                    
                                                                      Color.White,    // GridListNormal1                                                    
                                                                      Color.White,    // GridListNormal2                                                    
                                                                      Color.FromArgb(203, 207, 212),    // GridListPressed1                                                    
                                                                      Color.White,                      // GridListPressed2                                                    
                                                                      Color.FromArgb(186, 189, 194),    // GridListSelected                                                    
                                                                      Color.FromArgb(238, 241, 247),    // GridSheetColNormal1                                                    
                                                                      Color.FromArgb(218, 222, 227),    // GridSheetColNormal2                                                    
                                                                      Color.FromArgb(255, 223, 107),    // GridSheetColPressed1                                                    
                                                                      Color.FromArgb(255, 252, 230),    // GridSheetColPressed2                                                    
                                                                      Color.FromArgb(255, 211,  89),    // GridSheetColSelected1
                                                                      Color.FromArgb(255, 239, 113),    // GridSheetColSelected2
                                                                      Color.FromArgb(223, 227, 232),    // GridSheetRowNormal                                                   
                                                                      Color.FromArgb(255, 223, 107),    // GridSheetRowPressed
                                                                      Color.FromArgb(245, 210,  87),    // GridSheetRowSelected
                                                                      Color.FromArgb(218, 220, 221),    // GridDataCellBorder
                                                                      Color.FromArgb(183, 219, 255),    // GridDataCellSelected
                                                                      Color.Black,                      // InputControlTextNormal
                                                                      Color.FromArgb(168, 168, 168),    // InputControlTextDisabled
                                                                      Color.FromArgb(212, 214, 217),    // InputControlBorderNormal
                                                                      Color.FromArgb(187, 187, 187),    // InputControlBorderDisabled
                                                                      Color.FromArgb(255, 255, 255),    // InputControlBackNormal
                                                                      _backColor,                       // InputControlBackDisabled
                                                                      Color.FromArgb(247, 247, 247),    // InputControlBackInactive
                                                                      Color.Black,                      // InputDropDownNormal1
                                                                      Color.Transparent,                // InputDropDownNormal2
                                                                      Color.FromArgb(172, 168, 153),    // InputDropDownDisabled1
                                                                      Color.Transparent,                // InputDropDownDisabled2
                                                                      Color.FromArgb(240, 242, 245),    // ContextMenuHeadingBack
                                                                      Color.FromArgb( 59,  59,  59),    // ContextMenuHeadingText
                                                                      Color.White,                      // ContextMenuImageColumn
                                                                      Color.FromArgb(224, 227, 231),    // AppButtonBack1
                                                                      Color.FromArgb(224, 227, 231),    // AppButtonBack2
                                                                      Color.FromArgb(135, 140, 146),    // AppButtonBorder
                                                                      Color.FromArgb(224, 227, 231),    // AppButtonOuter1
                                                                      Color.FromArgb(224, 227, 231),    // AppButtonOuter2
                                                                      Color.FromArgb(224, 227, 231),    // AppButtonOuter3
                                                                      Color.Empty,                      // AppButtonInner1
                                                                      Color.FromArgb(135, 140, 146),    // AppButtonInner2
                                                                      Color.White,                      // AppButtonMenuDocs
                                                                      Color.Black,                      // AppButtonMenuDocsText
                                                                      Color.FromArgb(250, 253, 255),    // SeparatorHighInternalBorder1
                                                                      Color.FromArgb(227, 232, 237),    // SeparatorHighInternalBorder2
                                                                      Color.FromArgb(198, 202, 205),    // RibbonGalleryBorder
                                                                      Color.FromArgb(255, 255, 255),    // RibbonGalleryBackNormal
                                                                      Color.FromArgb(255, 255, 255),    // RibbonGalleryBackTracking
                                                                      Color.FromArgb(250, 250, 250),    // RibbonGalleryBack1
                                                                      Color.FromArgb(228, 231, 235),    // RibbonGalleryBack2
                                                                      Color.FromArgb(229, 231, 235),    // RibbonTabTracking3
                                                                      Color.FromArgb(231, 233, 235),    // RibbonTabTracking4
                                                                      Color.FromArgb(176, 182, 188),    // RibbonGroupBorder3
                                                                      Color.FromArgb(246, 247, 248),    // RibbonGroupBorder4
                                                                      Color.FromArgb(249, 250, 250),    // RibbonGroupBorder5
                                                                      Color.FromArgb(102, 109, 124),    // RibbonGroupTitleText
                                                                      Color.FromArgb(151, 156, 163),    // RibbonDropArrowLight
                                                                      Color.FromArgb( 39,  49,  60),    // RibbonDropArrowDark
                                                                      Color.FromArgb(239, 239, 239),    // HeaderDockInactiveBack1
                                                                      Color.Empty,                      // HeaderDockInactiveBack2 -n
                                                                      Color.FromArgb(161, 169, 179),    // ButtonNavigatorBorder
                                                                      Color.Black,                      // ButtonNavigatorText
                                                                      Color.FromArgb(207, 213, 220),    // ButtonNavigatorTrack1
                                                                      Color.FromArgb(232, 234, 238),    // ButtonNavigatorTrack2
                                                                      Color.FromArgb(191, 196, 202),    // ButtonNavigatorPressed1
                                                                      Color.FromArgb(225, 226, 230),    // ButtonNavigatorPressed2
                                                                      Color.FromArgb(222, 227, 234),    // ButtonNavigatorChecked1
                                                                      Color.FromArgb(206, 214, 221),    // ButtonNavigatorChecked2
                                                                      Color.Empty,                      // ToolTipBottom -n
        };
        #endregion

        #region Identity
        static PaletteOffice2016Blue()
        {
            _checkBoxList = new ImageList();
            _checkBoxList.ImageSize = new Size(
                (int)Math.Round(13 * DpiHelper.Default.DpiScaleFactor),
                (int)Math.Round(13 * DpiHelper.Default.DpiScaleFactor));
            _checkBoxList.ColorDepth = ColorDepth.Depth24Bit;

            Bitmap imageStrip = null;
            if (DpiHelper.Default.DpiScaleFactor >= 2.0f)
            {
                imageStrip = Properties.Resources.CB2016Blue_200;
            }
            else if (DpiHelper.Default.DpiScaleFactor >= 1.75f)
            {
                imageStrip = Properties.Resources.CB2016Blue_200; //downscale
            }
            else if (DpiHelper.Default.DpiScaleFactor >= 1.5f)
            {
                imageStrip = Properties.Resources.CB2016Blue_150;
            }
            else if (DpiHelper.Default.DpiScaleFactor >= 1.25f)
            {
                imageStrip = Properties.Resources.CB2016Blue_120;
            }
            else
            {
                imageStrip = Properties.Resources.CB2016Blue;
            }

            if (imageStrip.Size.Height != _checkBoxList.ImageSize.Height)
            {
                imageStrip = new Bitmap(imageStrip,
                        new Size(_checkBoxList.ImageSize.Width * 12, _checkBoxList.ImageSize.Height));
            }

            //    if (DpiHelper.Default.DpiScaleFactor == 1.0f)
            //    imageStrip = Properties.Resources.CB2016Blue;
            //else if (DpiHelper.Default.DpiScaleFactor == 1.25f)
            //    imageStrip = Properties.Resources.CB2016Blue_120;
            //else if (DpiHelper.Default.DpiScaleFactor == 1.5f)
            //    imageStrip = Properties.Resources.CB2016Blue_150;
            //else if (DpiHelper.Default.DpiScaleFactor > 1.5f)
            //{
            //}
            //else if (DpiHelper.Default.DpiScaleFactor > 1.5f)
            //{
            //    // scale image if scale >= 200%
            //    imageStrip = new Bitmap(Properties.Resources.CB2016Blue_150,
            //        new Size(_checkBoxList.ImageSize.Width * 12, _checkBoxList.ImageSize.Height));
            //}

            _checkBoxList.Images.AddStrip(imageStrip);

            _galleryButtonList = new ImageList();
            _galleryButtonList.ImageSize = new Size(13, 7); //!!!! without scaling ?
            _galleryButtonList.ColorDepth = ColorDepth.Depth24Bit;
            _galleryButtonList.TransparentColor = Color.Magenta;
            _galleryButtonList.Images.AddStrip(Properties.Resources.Gallery2010);
            _radioButtonArray = new Image[]{Properties.Resources.RB2016BlueD,
                                            Properties.Resources.RB2016BlueN,
                                            Properties.Resources.RB2016BlueT,
                                            Properties.Resources.RB2016BlueP,
                                            Properties.Resources.RB2016BlueDC,
                                            Properties.Resources.RB2016BlueNC,
                                            Properties.Resources.RB2016BlueTC,
                                            Properties.Resources.RB2016BluePC};

            // !!!!: scale images (fix it)
            if ( DpiHelper.Default.DpiScaleFactor != 1.0f )
            {
                for ( int i = 0; i < _radioButtonArray.Length; i++ )
                {
                    _radioButtonArray[i] = new Bitmap( _radioButtonArray[i],
                        DpiHelper.Default.ScaleValue( _radioButtonArray[i].Size ) );
                }
            }
        }

        /// <summary>
        /// Initialize a new instance of the PaletteOffice2010Silver class.
		/// </summary>
        public PaletteOffice2016Blue()
            : base(_schemeColors, 
                   _checkBoxList, 
                   _galleryButtonList, 
                   _radioButtonArray,
                   _trackBarColors)
        {
		}
		#endregion

        #region Images
        /// <summary>
        /// Gets a drop down button image appropriate for the provided state.
        /// </summary>
        /// <param name="state">PaletteState for which image is required.</param>
        public override Image GetDropDownButtonImage(PaletteState state)
        {
            if (state != PaletteState.Disabled)
                return _silverDropDownButton;
            else
                return base.GetDropDownButtonImage(state);
        }

        /// <summary>
        /// Gets an image indicating a sub-menu on a context menu item.
        /// </summary>
        /// <returns>Appropriate image for drawing; otherwise null.</returns>
        public override Image GetContextMenuSubMenuImage()
        {
            return _contextMenuSubMenu;
        }
        #endregion

        #region ButtonSpec
        /// <summary>
        /// Gets the image to display for the button.
        /// </summary>
        /// <param name="style">Style of button spec.</param>
        /// <param name="state">State for which image is required.</param>
        /// <returns>Image value.</returns>
        public override Image GetButtonSpecImage(PaletteButtonSpecStyle style,
                                                 PaletteState state)
        {
            switch (style)
            {
                case PaletteButtonSpecStyle.FormClose:
                    return _formClose;
                case PaletteButtonSpecStyle.FormMin:
                    return _formMin;
                case PaletteButtonSpecStyle.FormMax:
                    return _formMax;
                case PaletteButtonSpecStyle.FormRestore:
                    return _formRestore;
                default:
                    return base.GetButtonSpecImage(style, state);
            }
        }
        #endregion
    }
}
