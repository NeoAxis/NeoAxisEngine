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
    public class PaletteOffice2016Black : PaletteOffice2016Base
    {
        #region Static Fields
        private static readonly Color _backColor = Color.FromArgb( 54, 54, 54 );//45, 45, 48 );//240, 240, 240);

        private static readonly ImageList _checkBoxList;
        private static readonly ImageList _galleryButtonList;
        private static readonly Image[] _radioButtonArray;
        private static readonly Image _silverDropDownButton = Properties.Resources._2010BlueDropDownButton;
        private static readonly Image _contextMenuSubMenu = Properties.Resources._2010BlueContextMenuSub;
        private static readonly Image _formClose = Properties.Resources._2016ButtonClose;
        private static readonly Image _formMax = Properties.Resources._2016ButtonMax;
        private static readonly Image _formMin = Properties.Resources._2016ButtonMin;
        private static readonly Image _formRestore = Properties.Resources._2016ButtonRestore;
        private static readonly Color[] _trackBarColors = new Color[] { Color.FromArgb(90,90,90),//196, 196, 196),  // Tick marks
                                                                        Color.FromArgb(90,90,90),//196, 196, 196),  // Top track
                                                                        Color.FromArgb(90,90,90),//196, 196, 196),  // Bottom track
                                                                        Color.FromArgb(90,90,90),//196, 196, 196),  // Fill track
                                                                        Color.FromArgb(150,150,150),//177, 214, 240),  // Fill position (pressed state)
                                                                        Color.Magenta                   // --
                                                                      };
        private static readonly Color[] _schemeColors = new Color[] { Color.FromArgb(230,230,230),// 59,  59,  59),    // TextLabelControl
                                                                      Color.FromArgb(230,230,230),// 59,  59,  59),    // TextButtonNormal
                                                                      Color.FromArgb(230,230,230),//Color.Black,                      // TextButtonChecked
                                                                      Color.FromArgb(90,90,90),//170, 170, 170),    // ButtonNormalBorder1
                                                                      Color.FromArgb(90,90,90),//170, 170, 170),    // ButtonNormalDefaultBorder
                                                                      Color.FromArgb(60,60,60),//253, 253, 253),    // ButtonNormalBack1
                                                                      Color.Red,                        // ButtonNormalBack2 -n
                                                                      Color.FromArgb(70,70,70),//253, 253, 253),    // ButtonNormalDefaultBack1
                                                                      Color.Red,                        // ButtonNormalDefaultBack2 -n
                                                                      Color.FromArgb(207, 212, 218),    // ButtonNormalNavigatorBack1
                                                                      Color.FromArgb(207, 212, 218),    // ButtonNormalNavigatorBack2
                                                                      _backColor,                       // PanelClient
                                                                      _backColor,                       // PanelAlternative
                                                                      Color.FromArgb(80,80,80),//213, 213, 213),    // ControlBorder
                                                                      Color.FromArgb(250, 253, 255),    // SeparatorHighBorder1
                                                                      Color.FromArgb(227, 232, 237),    // SeparatorHighBorder2
                                                                      Color.FromArgb(255, 255, 255),    // HeaderPrimaryBack1 -n
                                                                      Color.FromArgb(255, 255, 255),    // HeaderPrimaryBack2 -n
                                                                      Color.FromArgb(255, 255, 255),    // HeaderSecondaryBack1
                                                                      Color.FromArgb(255, 255, 255),    // HeaderSecondaryBack2-n
                                                                      Color.FromArgb( 130,130,130),//59,  59,  59),    // HeaderText
                                                                      Color.FromArgb( 59,  59,  59),    // StatusStripText
                                                                      Color.Red,                        // ButtonBorder
                                                                      Color.Red,                        // SeparatorLight -n
                                                                      Color.FromArgb(40,40,40),//210, 210, 210),    // SeparatorDark
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
                                                                      Color.FromArgb(90,90,90),//0  , 114, 198),    // FormBorderActive
                                                                      Color.FromArgb(70,70,70),//131, 131, 131),    // FormBorderInactive
                                                                      Color.FromArgb(228, 230, 232),    // FormBorderActiveLight
                                                                      Color.FromArgb(255, 255, 255),    // FormBorderActiveDark
                                                                      Color.FromArgb(248, 247, 247),    // FormBorderInactiveLight
                                                                      Color.FromArgb(248, 247, 247),    // FormBorderInactiveDark
                                                                      Color.FromArgb(101, 109, 117),    // FormBorderHeaderActive
                                                                      Color.FromArgb(131, 131, 131),    // FormBorderHeaderInactive
                                                                      Color.FromArgb(10,10,10),//42 , 87,  154),    // FormBorderHeaderActive1
                                                                      Color.FromArgb(228, 230, 232),    // FormBorderHeaderActive2
                                                                      Color.FromArgb(10,10,10),//42 , 87,  154),    // FormBorderHeaderInctive1
                                                                      Color.FromArgb(248, 247, 247),    // FormBorderHeaderInctive2
                                                                      Color.FromArgb(0  , 114, 198),    // FormShadowActive
                                                                      Color.FromArgb(131, 131, 131),    // FormShadowInactive
                                                                      Color.FromArgb(255, 255, 255),    // FormHeaderShortActive
                                                                      Color.FromArgb(185, 185, 185),//165, 185, 210),    // FormHeaderShortInactive
                                                                      Color.FromArgb(255, 255, 255),    // FormHeaderLongActive
                                                                      Color.FromArgb(185, 185, 185),//165, 185, 210),    // FormHeaderLongInactive
                                                                      Color.Empty,                      // FormButtonBorderTrack
                                                                      Color.FromArgb(54,54,54),//62, 109, 182),     // FormButtonBack1Track
                                                                      Color.Empty,						// FormButtonBack2Track
                                                                      Color.Empty,                      // FormButtonBorderPressed
                                                                      Color.FromArgb(40,40,40),//19,  64,  119),    // FormButtonBack1Pressed
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
                                                                      Color.FromArgb(255,255,255),//42, 87, 154),      // RibbonTabTextChecked -n
                                                                      _backColor,                       // RibbonTabSelected1
                                                                      Color.Empty,                      // RibbonTabSelected2
                                                                      Color.Empty,                      // RibbonTabSelected3
                                                                      Color.Empty,                      // RibbonTabSelected4
                                                                      Color.Empty,                      // RibbonTabSelected5
                                                                      Color.FromArgb(54,54,54),//62, 109, 182),     // RibbonTabTracking1
                                                                      Color.Empty,						// RibbonTabTracking2
                                                                      Color.FromArgb(255, 0, 0),        // RibbonTabHighlight1
                                                                      Color.Empty,                      // RibbonTabHighlight2
                                                                      Color.Empty,                      // RibbonTabHighlight3
                                                                      Color.Empty,                      // RibbonTabHighlight4
                                                                      Color.Empty,                      // RibbonTabHighlight5
                                                                      Color.Empty,//Color.FromArgb(255, 0, 0),        // RibbonTabSeparatorColor

                                                                      _backColor,                       // RibbonGroupsArea1 -n
                                                                      Color.FromArgb(40,40,40),//102,102,102),//213, 213, 213),    // RibbonGroupsArea2 -n
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
                                                                      //Color.FromArgb(102, 102, 102),    // RibbonGroupCollapsedBackT2
                                                                      //Color.FromArgb(102, 102, 102),    // RibbonGroupCollapsedBackT3
                                                                      //Color.FromArgb(102, 102, 102),    // RibbonGroupCollapsedBackT4
                                                                      //Color.FromArgb(102, 102, 102),    // RibbonGroupFrameBorder1
                                                                      //Color.FromArgb(102, 102, 102),    // RibbonGroupFrameBorder2
                                                                      //Color.FromArgb(102, 102, 102),    // RibbonGroupFrameInside1
                                                                      //Color.FromArgb(102, 102, 102),    // RibbonGroupFrameInside2
                                                                      Color.FromArgb(242, 244, 247),    // RibbonGroupCollapsedBackT2
                                                                      Color.FromArgb(238, 241, 245),    // RibbonGroupCollapsedBackT3
                                                                      Color.FromArgb(234, 235, 235),    // RibbonGroupCollapsedBackT4
                                                                      Color.FromArgb(208, 212, 217),    // RibbonGroupFrameBorder1
                                                                      Color.FromArgb(208, 212, 217),    // RibbonGroupFrameBorder2
                                                                      Color.FromArgb(254, 254, 254),    // RibbonGroupFrameInside1
                                                                      Color.FromArgb(254, 254, 254),    // RibbonGroupFrameInside2
                                                                      Color.Empty,                      // RibbonGroupFrameInside3
                                                                      Color.Empty,                      // RibbonGroupFrameInside4
                                                                      Color.FromArgb(230,230,230),// 59,  59,  59),    // RibbonGroupCollapsedText         
                                                                      Color.FromArgb(179, 185, 195),    // AlternatePressedBack1
                                                                      Color.FromArgb(216, 224, 224),    // AlternatePressedBack2
                                                                      Color.FromArgb(125, 125, 125),    // AlternatePressedBorder1
                                                                      Color.FromArgb(186, 186, 186),    // AlternatePressedBorder2
                                                                      Color.FromArgb(54,54,54),//62, 109, 182),	    // FormButtonBack1Checked
                                                                      Color.Empty,						// FormButtonBack2Checked -n
                                                                      Color.Empty,					    // FormButtonBorderCheck
                                                                      Color.FromArgb(54,54,54),//62, 109, 182),		// FormButtonBack1CheckTrack
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
                                                                      Color.FromArgb(54,54,54),//240, 240, 240),    // RibbonQATFullbar2                                                      
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
                                                                      Color.FromArgb(230,230,230),//Color.Black,                      // InputControlTextNormal
                                                                      Color.FromArgb(90,90,90),//168, 168, 168),    // InputControlTextDisabled
                                                                      Color.FromArgb(100,100,100),//212, 214, 217),    // InputControlBorderNormal
                                                                      Color.FromArgb(80,80,80),//187, 187, 187),    // InputControlBorderDisabled
                                                                      Color.FromArgb(40,40,40),//255, 255, 255),    // InputControlBackNormal
                                                                      _backColor,                       // InputControlBackDisabled
                                                                      Color.FromArgb(247, 247, 247),    // InputControlBackInactive
                                                                      Color.FromArgb(170, 170, 170),// Color.Black,                      // InputDropDownNormal1
                                                                      Color.Transparent,                // InputDropDownNormal2
                                                                      Color.FromArgb(80,80,80),// Color.FromArgb(172, 168, 153),    // InputDropDownDisabled1
                                                                      Color.Transparent,                // InputDropDownDisabled2
                                                                      Color.FromArgb(50,50,50),//240, 242, 245),    // ContextMenuHeadingBack
                                                                      Color.FromArgb(180,180,180),// 59,  59,  59),    // ContextMenuHeadingText
                                                                      Color.FromArgb( 55,  55,  55),//Color.White,                      // ContextMenuImageColumn
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
                                                                      Color.FromArgb(253, 253, 253),//250, 253, 255),    // SeparatorHighInternalBorder1
                                                                      Color.FromArgb(232, 232, 232),//227, 232, 237),    // SeparatorHighInternalBorder2
                                                                      Color.FromArgb(202, 202, 202),//198, 202, 205),    // RibbonGalleryBorder
                                                                      Color.FromArgb(255, 255, 255),    // RibbonGalleryBackNormal
                                                                      Color.FromArgb(255, 255, 255),    // RibbonGalleryBackTracking
                                                                      Color.FromArgb(250, 250, 250),    // RibbonGalleryBack1
                                                                      Color.FromArgb(231,231,231),//228, 231, 235),    // RibbonGalleryBack2
                                                                      Color.FromArgb(231,231,231),//229, 231, 235),    // RibbonTabTracking3
                                                                      Color.FromArgb(231, 233, 235),    // RibbonTabTracking4
                                                                      Color.FromArgb(40, 40, 40),    // RibbonGroupBorder3
                                                                      Color.FromArgb(40, 40, 40),    // RibbonGroupBorder4
                                                                      Color.FromArgb(40, 40, 40),    // RibbonGroupBorder5
                                                                      //Color.FromArgb(102, 102, 102),    // RibbonGroupBorder3
                                                                      //Color.FromArgb(102, 102, 102),    // RibbonGroupBorder4
                                                                      //Color.FromArgb(102, 102, 102),    // RibbonGroupBorder5
                                                                      //Color.FromArgb(176, 182, 188),    // RibbonGroupBorder3
                                                                      //Color.FromArgb(246, 247, 248),    // RibbonGroupBorder4
                                                                      //Color.FromArgb(249, 250, 250),    // RibbonGroupBorder5
                                                                      Color.FromArgb(150,150,150),//255,255,255),//102, 109, 124),    // RibbonGroupTitleText
                                                                      Color.FromArgb(156, 156, 156),//151, 156, 163),    // RibbonDropArrowLight
                                                                      Color.FromArgb( 50,50,50),//39,  49,  60),    // RibbonDropArrowDark
                                                                      Color.FromArgb(10,10,10),//239, 239, 239),    // HeaderDockInactiveBack1
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
        static PaletteOffice2016Black()
        {
            _checkBoxList = new ImageList();
            _checkBoxList.ImageSize = new Size(
                (int)Math.Round( 13 * DpiHelper.Default.DpiScaleFactor ),
                (int)Math.Round( 13 * DpiHelper.Default.DpiScaleFactor ) );
            _checkBoxList.ColorDepth = ColorDepth.Depth24Bit;

            Bitmap imageStrip = null;
            if( DpiHelper.Default.DpiScaleFactor >= 2.0f )
            {
                imageStrip = Properties.Resources.CB2016Blue_200_Dark;
            }
            else if( DpiHelper.Default.DpiScaleFactor >= 1.75f )
            {
                imageStrip = Properties.Resources.CB2016Blue_200_Dark; //downscale
            }
            else if( DpiHelper.Default.DpiScaleFactor >= 1.5f )
            {
                imageStrip = Properties.Resources.CB2016Blue_150_Dark;
            }
            else if( DpiHelper.Default.DpiScaleFactor >= 1.25f )
            {
                imageStrip = Properties.Resources.CB2016Blue_120_Dark;
            }
            else
            {
                imageStrip = Properties.Resources.CB2016Blue_Dark;
            }

            if( imageStrip.Size.Height != _checkBoxList.ImageSize.Height )
            {
                imageStrip = new Bitmap( imageStrip,
                        new Size( _checkBoxList.ImageSize.Width * 12, _checkBoxList.ImageSize.Height ) );
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

            _checkBoxList.Images.AddStrip( imageStrip );

            _galleryButtonList = new ImageList();
            _galleryButtonList.ImageSize = new Size( 13, 7 ); //!!!! without scaling ?
            _galleryButtonList.ColorDepth = ColorDepth.Depth24Bit;
            _galleryButtonList.TransparentColor = Color.Magenta;
            _galleryButtonList.Images.AddStrip( Properties.Resources.Gallery2010 );
            _radioButtonArray = new Image[]{Properties.Resources.RB2016BlueD,
                                            Properties.Resources.RB2016BlueN,
                                            Properties.Resources.RB2016BlueT,
                                            Properties.Resources.RB2016BlueP,
                                            Properties.Resources.RB2016BlueDC,
                                            Properties.Resources.RB2016BlueNC,
                                            Properties.Resources.RB2016BlueTC,
                                            Properties.Resources.RB2016BluePC};

            // !!!!: scale images (fix it)
            if( DpiHelper.Default.DpiScaleFactor != 1.0f )
            {
                for( int i = 0; i < _radioButtonArray.Length; i++ )
                {
                    _radioButtonArray[ i ] = new Bitmap( _radioButtonArray[ i ],
                        DpiHelper.Default.ScaleValue( _radioButtonArray[ i ].Size ) );
                }
            }
        }

    /// <summary>
    /// Initialize a new instance of the PaletteOffice2010Silver class.
    /// </summary>
    public PaletteOffice2016Black()
            : base( _schemeColors,
                   _checkBoxList,
                   _galleryButtonList,
                   _radioButtonArray,
                   _trackBarColors )
        {

            _disabledText = Color.FromArgb( 90, 90, 90 );
            _contextMenuBack = Color.FromArgb( 40, 40, 40 );
            _contextMenuBorder = Color.FromArgb( 90, 90, 90 );
            //_contextMenuBorder = Color.FromArgb( 40, 40, 40 );
            _contextMenuHeadingBorder = Color.FromArgb( 20, 20, 20 );

            //!!!!не проверялось
            _contextMenuImageBorderChecked = Color.FromArgb( 80, 80, 80 );

            _standaloneButtonBorderColors = new Color[]
            {
            Color.FromArgb(60, 60, 60),  // Button, Disabled, Border
            Color.FromArgb(130, 130, 130),   // Button, Tracking, Border 1
            Color.Empty,				    // Button, Tracking, Border 2
            Color.FromArgb(130, 130, 130),    // Button, Pressed, Border 1
            Color.Empty,				    // Button, Pressed, Border 2
            Color.FromArgb(150, 150, 150),    // Button, Checked, Border 1
            Color.Empty,                    // Button, Checked, Border 2
            };

            _standaloneButtonBackColors = new Color[]
            {
            Color.FromArgb(50, 50, 50), // Button, Disabled, Back 1
            Color.Empty,				   // Button, Disabled, Back 2
            Color.FromArgb(80, 80, 80), // Button, Tracking, Back 1
            Color.Empty,				   // Button, Tracking, Back 2
            Color.FromArgb(80, 80, 80), // Button, Pressed, Back 1
            Color.Empty,				   // Button, Pressed, Back 2
            Color.FromArgb(90, 90, 90), // Button, Checked, Back 1
            Color.Empty,			       // Button, Checked, Back 2
            Color.FromArgb(90, 90, 90), // Button, Checked Tracking, Back 1
            Color.Empty                    // Button, Checked Tracking, Back 2
            };

            _buttonBorderColors = new Color[]
            {
            Color.FromArgb(60, 60, 60),  // Button, Disabled, Border
            Color.FromArgb(90, 90, 90),  // Button, Tracking, Border 1
            Color.Empty,				    // Button, Tracking, Border 2
            Color.FromArgb(100, 100, 100),  // Button, Pressed, Border 1
            Color.Empty,				    // Button, Pressed, Border 2
            Color.FromArgb(80, 80, 80),  // Button, Checked, Border 1
            Color.Empty,                    // Button, Checked, Border 2
            };

            _buttonBackColors = new Color[]
            {
            Color.FromArgb(60, 60, 60), // Button, Disabled, Back 1
            Color.Empty,				   // Button, Disabled, Back 2
            Color.FromArgb(90, 90, 90), // Button, Tracking, Back 1
            Color.Empty,				   // Button, Tracking, Back 2
            Color.FromArgb(100, 100, 100), // Button, Pressed, Back 1
            Color.Empty,				   // Button, Pressed, Back 2
            Color.FromArgb(80, 80, 80), // Button, Checked, Back 1
            Color.Empty,			       // Button, Checked, Back 2
            Color.FromArgb(100, 100, 100), // Button, Checked Tracking, Back 1
            Color.Empty                    // Button, Checked Tracking, Back 2
            };

            _toolTipBack1 = Color.FromArgb( 20, 20, 20 );
            //_toolTipBack1 = Color.FromArgb( 54, 54, 54 );
            _toolTipBorder = Color.FromArgb( 110, 110, 110 );
            _toolTipText = Color.FromArgb( 230, 230, 230 );

            //Dark Red
            //_ribbonColors[ (int)SchemeOfficeColors.InputDropDownNormal1 ] = Color.Red;
            //_ribbonColors[ (int)SchemeOfficeColors.TextLabelControl ] = Color.Red;
            //_ribbonColors[ (int)SchemeOfficeColors.TextButtonNormal ] = Color.Red;

        }
        #endregion

                    #region Images
                    /// <summary>
                    /// Gets a drop down button image appropriate for the provided state.
                    /// </summary>
                    /// <param name="state">PaletteState for which image is required.</param>
        public override Image GetDropDownButtonImage( PaletteState state )
        {
            if( state != PaletteState.Disabled )
                return _silverDropDownButton;
            else
                return base.GetDropDownButtonImage( state );
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
        public override Image GetButtonSpecImage( PaletteButtonSpecStyle style,
                                                 PaletteState state )
        {
            switch( style )
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
                return base.GetButtonSpecImage( style, state );
            }
        }
        #endregion

        public override Color GetBorderColor1( PaletteBorderStyle style, PaletteState state )
        {
            ////!!!!
            //return Color.Red;

            switch( style )
            {
            case PaletteBorderStyle.ButtonCustom3: // Grid, combobox
            case PaletteBorderStyle.ButtonStandalone:
                switch( state )
                {
                case PaletteState.Disabled:
                    return _standaloneButtonBorderColors[ (int)ButtonBorderColors.Disabled ];
                //case PaletteState.Normal:
                //    if( style == PaletteBorderStyle.ButtonCustom3 ) // Grid
                //        return _ribbonColors[ (int)SchemeOfficeColors.InputControlBorderNormal ];
                //    else
                //        return _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalBorder ];
                //case PaletteState.NormalDefaultOverride:
                //    if( style == PaletteBorderStyle.ButtonCustom3 ) // Grid
                //        return _ribbonColors[ (int)SchemeOfficeColors.InputControlBorderNormal ];
                //    else
                //        return _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalDefaultBorder ];
                case PaletteState.CheckedNormal:
                    return _standaloneButtonBorderColors[ (int)ButtonBorderColors.Checked ];
                case PaletteState.Tracking:
                    return _standaloneButtonBorderColors[ (int)ButtonBorderColors.Tracking ];
                case PaletteState.Pressed:
                case PaletteState.CheckedPressed:
                case PaletteState.CheckedTracking:
                    return _standaloneButtonBorderColors[ (int)ButtonBorderColors.Checked ];
                    //default:
                    //    throw new ArgumentOutOfRangeException( "state" );
                }
                break;

            case PaletteBorderStyle.ButtonGallery:
            case PaletteBorderStyle.ButtonAlternate:
            case PaletteBorderStyle.ButtonLowProfile:
            case PaletteBorderStyle.ButtonBreadCrumb:
            case PaletteBorderStyle.ButtonListItem:
            case PaletteBorderStyle.ButtonCommand:
            case PaletteBorderStyle.ButtonButtonSpec:
            case PaletteBorderStyle.ButtonCluster:
            case PaletteBorderStyle.ButtonCustom1:
            case PaletteBorderStyle.ButtonCustom2:
            case PaletteBorderStyle.ContextMenuItemHighlight:
                switch( state )
                {
                case PaletteState.Disabled:
                    if( style == PaletteBorderStyle.ButtonGallery )
                        return _ribbonColors[ (int)SchemeOfficeColors.RibbonGalleryBack2 ];
                    else
                        return _buttonBorderColors[ 0 ];
                case PaletteState.Normal:
                    return _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalBorder ];
                case PaletteState.NormalDefaultOverride:
                    if( ( style == PaletteBorderStyle.ButtonLowProfile ) ||
                        ( style == PaletteBorderStyle.ButtonBreadCrumb ) ||
                        ( style == PaletteBorderStyle.ButtonListItem ) ||
                        ( style == PaletteBorderStyle.ButtonCommand ) ||
                        ( style == PaletteBorderStyle.ButtonButtonSpec ) ||
                        ( style == PaletteBorderStyle.ContextMenuItemHighlight ) )
                        return Color.Empty;
                    else
                        return _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalDefaultBorder ];
                case PaletteState.CheckedNormal:
                    return _buttonBorderColors[ 5 ];

                case PaletteState.Tracking:
                    if( style == PaletteBorderStyle.ContextMenuItemHighlight )
                        return Color.FromArgb( 70, 70, 70 );
                    else
                        return _buttonBorderColors[ 1 ];

                case PaletteState.Pressed:
                case PaletteState.CheckedPressed:
                    return _buttonBorderColors[ 3 ];
                case PaletteState.CheckedTracking:
                    return _buttonBorderColors[ 3 ];
                    //default:
                    //    throw new ArgumentOutOfRangeException( "state" );
                }
                break;

            case PaletteBorderStyle.ControlClient:
                if( state == PaletteState.Disabled )
                    return Color.FromArgb( 90, 90, 90 );
                break;

            }

            return base.GetBorderColor1( style, state );
        }

        public override Color GetBackColor1( PaletteBackStyle style, PaletteState state )
        {
            ////!!!!
            //return Color.Red;

            switch( style )
            {
            case PaletteBackStyle.ButtonStandalone:
            case PaletteBackStyle.ButtonCustom3:
                switch( state )
                {
                case PaletteState.Disabled:
                    return _standaloneButtonBackColors[ (int)ButtonBackColors.Disabled ];
                //case PaletteState.Normal:
                //    return _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalBack1 ];
                //case PaletteState.NormalDefaultOverride:
                //    return _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalDefaultBack1 ];
                case PaletteState.CheckedNormal:
                    return _standaloneButtonBackColors[ (int)ButtonBackColors.Checked ];
                case PaletteState.Tracking:
                    return _standaloneButtonBackColors[ (int)ButtonBackColors.Tracking ];
                case PaletteState.Pressed:
                case PaletteState.CheckedPressed:
                    return _standaloneButtonBackColors[ (int)ButtonBackColors.Checked ];
                case PaletteState.CheckedTracking:
                    return _standaloneButtonBackColors[ (int)ButtonBackColors.CheckedTracking ];
                    //default:
                    //    throw new ArgumentOutOfRangeException( "state" );
                }
                break;

            case PaletteBackStyle.ButtonGallery:
            case PaletteBackStyle.ButtonAlternate:
            case PaletteBackStyle.ButtonLowProfile:
            case PaletteBackStyle.ButtonBreadCrumb:
            case PaletteBackStyle.ButtonCommand:
            case PaletteBackStyle.ButtonButtonSpec:
            case PaletteBackStyle.ButtonCalendarDay:
            case PaletteBackStyle.ButtonCluster:
            case PaletteBackStyle.ButtonCustom1:
            case PaletteBackStyle.ButtonCustom2:
            case PaletteBackStyle.ButtonInputControl:
            case PaletteBackStyle.ContextMenuItemHighlight:

                switch( state )
                {
                case PaletteState.Disabled:
                    if( style == PaletteBackStyle.ButtonGallery )
                        return _ribbonColors[ (int)SchemeOfficeColors.RibbonGalleryBack1 ];
                    else
                        return _disabledBack;
                case PaletteState.Normal:
                    return _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalBack1 ];
                case PaletteState.NormalDefaultOverride:
                    return _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalDefaultBack1 ];
                case PaletteState.CheckedNormal:
                    if( style == PaletteBackStyle.ButtonInputControl )
                        return _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalBack1 ];
                    else
                        return _buttonBackColors[ 6 ];

                case PaletteState.Tracking:
                    if( style == PaletteBackStyle.ContextMenuItemHighlight )
                        return Color.FromArgb( 70, 70, 70 );
                    else
                        return _buttonBackColors[ 2 ];

                case PaletteState.Pressed:
                case PaletteState.CheckedPressed:
                    return _buttonBackColors[ 4 ];
                case PaletteState.CheckedTracking:
                    if( style == PaletteBackStyle.ButtonInputControl )
                        return _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalBack1 ];
                    else
                        return _buttonBackColors[ 8 ];
                    //default:
                    //    throw new ArgumentOutOfRangeException( "state" );
                }

                break;

            case PaletteBackStyle.TabLowProfile:
                if( state == PaletteState.CheckedNormal )
                    return Color.FromArgb( 80, 80, 80 );
                break;

            case PaletteBackStyle.TabDockAutoHidden:
            case PaletteBackStyle.TabDock:
                if( state.HasFlag( PaletteState.Tracking ) || state.HasFlag( PaletteState.Pressed ) || state.HasFlag( PaletteState.Checked ) )
                    return Color.FromArgb( 80, 80, 80 );
                else
                    return Color.FromArgb( 54, 54, 54 );
            }



            //return Color.FromArgb( 255, 0, 0 );

            //if( style.ToString().Length > 3 && style.ToString().Substring( 0, 3 ) == "Tab" )
            //    return Color.FromArgb( 0, 255, 0 );

            //if( style == PaletteBackStyle.TabDockAutoHidden )
            //    return Color.FromArgb( 0, 255, 0 );

            //if( style == PaletteBackStyle.TabCustom1 )
            //    return Color.FromArgb( 0, 0, 255 );

            //if( style == PaletteBackStyle.TabCustom2 )
            //    return Color.FromArgb( 255, 0, 0 );

            //if( style == PaletteBackStyle.TabCustom3 )
            //    return Color.FromArgb( 128, 0, 0 );

            //if( style == PaletteBackStyle.TabDock )
            //    return Color.FromArgb( 0, 129, 0 );

            //if( style == PaletteBackStyle.TabHighProfile )
            //    return Color.FromArgb( 0, 255, 0 );

            //if( style == PaletteBackStyle.TabStandardProfile )
            //    return Color.FromArgb( 0, 0, 255 );

            //if( style == PaletteBackStyle.TabLowProfile )
            //    return Color.FromArgb( 255, 0, 0 );

            //if( style == PaletteBackStyle.TabOneNote )
            //    return Color.FromArgb( 128, 0, 0 );

            //if( style == PaletteBackStyle.TabDock )
            //    return Color.FromArgb( 0, 129, 0 );

            //var c = base.GetBackColor1( style, state );
            //if( c == Color.White )
            ////if( c == Color.Empty )
            //{
            //    //c = Color.FromArgb( 255, 0, 0 );

            //    //Console.Write( c.ToString() + " " + style.ToString() + " " + state.ToString() );

            //}
            //return c;

            return base.GetBackColor1( style, state );
        }

        public override Color GetContentShortTextColor1( PaletteContentStyle style, PaletteState state )
        {
            switch( style )
            {
            case PaletteContentStyle.ButtonCustom3:
                if( state == PaletteState.Normal )
                    return Color.FromArgb( 230, 230, 230 );
                break;
            }

            //var c = base.GetContentShortTextColor1( style, state );

            //if( c == Color.Black )
            //{
            //    if( style == PaletteContentStyle.ButtonCustom3 )
            //    {
            //        if( state == PaletteState.Normal )
            //            return Color.Red;
            //    }

            //    //Console.Write( c.ToString() + " " + style.ToString() + " " + state.ToString() );
            //}

            //return c;

            return base.GetContentShortTextColor1( style, state );
        }

        public override Color GetElementColor1( PaletteElement element, PaletteState state )
        {
            ////!!!!
            //return Color.Red;

            switch( element )
            {
            //case PaletteElement.TrackBarTick:
            //    return _trackBarColors[ 0 ];
            case PaletteElement.TrackBarTrack: //border
                switch( state )
                {
                case PaletteState.Disabled:
                    return Color.FromArgb( 70, 70, 70 );// ControlPaint.LightLight( _trackBarColors[ 1 ] );
                default:
                    return Color.FromArgb( 70, 70, 70 ); //_trackBarColors[ 1 ];
                }
            case PaletteElement.TrackBarPosition:
                //return Color.Red;
                switch( state )
                {
                case PaletteState.Disabled:
                    return Color.FromArgb( 70, 70, 70 );// ControlPaint.LightLight( _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalBorder ] );
                case PaletteState.Normal:
                    return Color.FromArgb( 100, 100, 100 ); //_ribbonColors[ (int)SchemeOfficeColors.ButtonNormalBorder ];
                case PaletteState.Tracking:
                case PaletteState.FocusOverride:
                    return Color.FromArgb( 120, 120, 120 ); //_standaloneButtonBorderColors[ (int)ButtonBorderColors.Tracking ];
                case PaletteState.Pressed: //!!!! this is ignored because PaletteState.FocusOverride
                    return Color.FromArgb( 130, 130, 130 ); //_standaloneButtonBorderColors[ (int)ButtonBorderColors.Checked ];
                }
                break;
            }

            return base.GetElementColor1( element, state );
        }

        public override Color GetElementColor2( PaletteElement element, PaletteState state )
        {
            ////!!!!
            //return Color.Red;

            switch( element )
            {
            //case PaletteElement.TrackBarTick:
            //    return Color.Green;
            case PaletteElement.TrackBarTrack: //border
                switch( state )
                {
                case PaletteState.Disabled:
                    return Color.FromArgb( 70, 70, 70 );// ControlPaint.LightLight( _trackBarColors[ 3 ] );
                default:
                    return Color.FromArgb( 70, 70, 70 ); //_trackBarColors[ 3 ];
                }
            case PaletteElement.TrackBarPosition:
                switch( state )
                {
                case PaletteState.Disabled:
                    return Color.FromArgb( 60, 60, 60 ); //ControlPaint.LightLight( _ribbonColors[ (int)SchemeOfficeColors.ButtonNormalBack1 ] );
                case PaletteState.Normal:
                    return Color.FromArgb( 70, 70, 70 ); //_ribbonColors[ (int)SchemeOfficeColors.ButtonNormalBack1 ];
                case PaletteState.Tracking:
                    return Color.FromArgb( 80, 80, 80 ); //_standaloneButtonBackColors[ (int)ButtonBackColors.Tracking ];
                case PaletteState.Pressed:
                    return Color.FromArgb( 90, 90, 90 ); //_trackBarColors[ 4 ];
                }
                break;
            }

            return base.GetElementColor2( element, state );
        }
    }
}
