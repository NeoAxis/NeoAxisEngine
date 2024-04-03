#if !DEPLOY
// *****************************************************************************
// 
//  © Component Factory Pty Ltd 2012. All rights reserved.
//	The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, 17/267 Nepean Hwy, 
//  Seaford, Vic 3198, Australia and are supplied subject to licence terms.
// 
// *****************************************************************************

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using System.Reflection;
using Microsoft.Win32;
using System.Security;
using System.Collections.Specialized;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
    /// <summary>
    /// Base class that allows a form to have custom chrome applied. You should derive 
    /// a class from this that performs the specific chrome drawing that is required.
    /// </summary>
    [ToolboxItem(false)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class VisualForm : Form, IKryptonDebug
    {
        [Browsable( false )]
        public bool AllowLockFormUpdate { get; set; } = true;

        private System.Windows.Forms.Timer timer1;
        bool needUnlockFormUpdate;
        bool allowShadow;

        DropShadowManager dropShadowManager;
        ShadowImageCacheManager borderImageCacheManager;

        //#region Static Fields
        //private static readonly int DEFAULT_COMPOSITION_HEIGHT =
        //    DpiHelper.Default.ScaleValue(31);
        //#endregion

        #region Instance Fields
        private bool _activated;
        private bool _windowActive;
        private bool _trackingMouse;
        private bool _applyCustomChrome;
        //private bool _allowComposition;
        //private bool _applyComposition;
        //private bool _insideUpdateComposition;
        private bool _needLayout;
        private bool _captured;
        private bool _disposing;
        //private int _compositionHeight;
        private int _ignoreCount;
        private int _paintCount;
        private ViewBase _capturedElement;
        //private IKryptonComposition _compositionElement;
        private IPalette _localPalette;
        private IPalette _palette;
        private IRenderer _renderer;
        private PaletteMode _paletteMode;
        private PaletteRedirect _redirector;
        private NeedPaintHandler _needPaintDelegate;
        private ViewManager _viewManager;
        private IntPtr _screenDC;
        //private bool _isOnePixelBorder = false;
        #endregion

        [Browsable(false)]
        public abstract bool AllowFormChrome { get; set; }

        #region Events
        /// <summary>
        /// Occurs when the palette changes.
        /// </summary>
        [ Category("Property Changed")]
        [Description("Occurs when the value of the Palette property is changed.")]
        public event EventHandler PaletteChanged;

        /// <summary>
        /// Occurs when the use of custom chrome changes.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler ApplyCustomChromeChanged;

        /// <summary>
        /// Occurs when the active window setting changes.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler WindowActiveChanged;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the VisualForm class. 
        /// </summary>
        [SecuritySafeCritical]
        public VisualForm()
        {
            // Automatically redraw whenever the size of the window changes
            SetStyle( ControlStyles.ResizeRedraw, true);

            // We need to create and cache a device context compatible with the display
            _screenDC = PI.CreateCompatibleDC(IntPtr.Zero);

            // Setup the need paint delegate
            _needPaintDelegate = new NeedPaintHandler(OnNeedPaint);

            // Set the palette and renderer to the defaults as specified by the manager
            _localPalette = null;
            SetPalette(KryptonManager.CurrentGlobalPalette);
            _paletteMode = PaletteMode.Global;

            // We need to layout the view
            _needLayout = true;

            //// Default the composition height
            //_compositionHeight = DEFAULT_COMPOSITION_HEIGHT;

            // Create constant target for resolving palette delegates
            _redirector = CreateRedirector();

            // Hook into global static events
            KryptonManager.GlobalPaletteChanged += new EventHandler(OnGlobalPaletteChanged);
#if !DEPLOY
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
#endif		

            //!!!!betauser. .NET Core
            Font = new Font( new FontFamily( "Microsoft Sans Serif" ), 8f );

            //!!!!betauser .NET Core. to fix bug with Chrome initialization
            BackColor = KryptonDarkThemeUtility.DarkTheme ? Color.FromArgb( 54, 54, 54 ) : Color.FromArgb( 240, 240, 240 );

            this.Load += new System.EventHandler( this.VisualForm_Load );
        }

        /// <summary>
        /// Releases all resources used by the Control. 
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        [SecuritySafeCritical]
        protected override void Dispose(bool disposing)
        {
            _disposing = true;

            timer1?.Dispose();

            ReleaseShadow();

            if (disposing)
            {
                // Must unhook from the palette paint events
                if (_palette != null)
                {
                    _palette.PalettePaint -= new EventHandler<PaletteLayoutEventArgs>(OnNeedPaint);
                    _palette.ButtonSpecChanged -= new EventHandler(OnButtonSpecChanged);
                    _palette.AllowFormChromeChanged -= new EventHandler(OnAllowFormChromeChanged);
                    _palette.BasePaletteChanged -= new EventHandler(OnBaseChanged);
                    _palette.BaseRendererChanged -= new EventHandler(OnBaseChanged);
                }

                // Unhook from global static events
                KryptonManager.GlobalPaletteChanged -= new EventHandler(OnGlobalPaletteChanged);
#if !DEPLOY
                SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
#endif		
            }

            base.Dispose(disposing);

            if (ViewManager != null)
                ViewManager.Dispose();

            if (_screenDC != IntPtr.Zero)
                PI.DeleteDC(_screenDC);
        }
#endregion

#region Public
        /// <summary>
        /// Gets and sets a value indicating if palette chrome should be applied.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ApplyCustomChrome {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _applyCustomChrome; }

            [SecuritySafeCritical]
            internal set {
                // Only interested in changed values
                if (_applyCustomChrome == value)
                    return;

                // Cache old setting
                bool oldApplyCustomChrome = _applyCustomChrome;

                // Store the new setting
                _applyCustomChrome = value;

                //Debug.WriteLine($"EDITOR: ApplyCustomChrome() applying {_applyCustomChrome}");

                // If we need custom chrome drawing...
                if (_applyCustomChrome)
                {
                    try
                    {
                        // Only need to remove the window theme, if there is one
                        bool appThemed = PI.IsWine() || (PI.IsAppThemed() && PI.IsThemeActive());
                        if (appThemed)
                        {
                            //// Retest if composition should be applied
                            //UpdateComposition();
                            //
                            RecalcSizeFromClientSize();

                            // When using composition we do not remove the theme
                            //if (!ApplyComposition)
                            {
                                // Remove any theme that is currently drawing chrome
                                PI.SetWindowTheme(Handle, "", "");
                            }
                            //else
                            //{
                            //    // Force a WM_NCCALCSIZE to update for composition
                            //    PI.SetWindowTheme(Handle, null, null);
                            //}

                            // Call virtual method for initializing own chrome
                            WindowChromeStart();
                        }
                        else
                        {
                            // Set back to false. the operating system 
                            // is not capable of supporting our custom chrome implementation
                            _applyCustomChrome = false;
                        }
                    }
                    catch
                    {
                        // Failed and so cannot provide custom chrome
                        _applyCustomChrome = false;
                    }
                }
                else
                {
                    try
                    {
                        //// Retest if composition should be applied
                        //UpdateComposition();
                        //
                        RecalcSizeFromClientSize();

                        // Restore the application to previous theme setting
                        PI.SetWindowTheme(Handle, null, null);

                        // Call virtual method to reverse own chrome setup
                        WindowChromeEnd();
                    }
                    catch { }
                }

                // Raise event to notify a change in setting
                if (_applyCustomChrome != oldApplyCustomChrome)
                {
                    // Generate change event
                    OnApplyCustomChromeChanged(EventArgs.Empty);
                }

                if (_applyCustomChrome != oldApplyCustomChrome)
                {
                    UpdateShadowState();
                }

                //Debug.WriteLine($"EDITOR: ApplyCustomChrome() _applyCustomChrome={_applyCustomChrome} Size={Size} ClientSize={ClientSize}");
            }
        }

        ///// <summary>
        ///// Gets a value indicating if composition is being applied.
        ///// </summary>
        //[Browsable(false)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public bool ApplyComposition {
        //    get { return _applyComposition; }
        //}

        ///// <summary>
        ///// Gets a value indicating if composition is allowed to be applied to custom chrome.
        ///// </summary>
        //[Browsable(false)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public bool AllowComposition {
        //    get { return _allowComposition; }

        //    set {
        //        if (_allowComposition != value)
        //        {
        //            _allowComposition = value;

        //            // If custom chrome is not enabled, then no need to make changes
        //            if (ApplyCustomChrome)
        //            {
        //                UpdateComposition();
        //                UpdateShadowState();
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// used to update the size of the composition area.
        ///// </summary>
        //[Browsable(false)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public void RecalculateComposition()
        //{
        //    UpdateComposition();
        //}

        ///// <summary>
        ///// Gets and sets the interface to the composition interface cooperating with the form.
        ///// </summary>
        //[Browsable(false)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public IKryptonComposition Composition {
        //    get { return _compositionElement; }
        //    set { _compositionElement = value; }
        //}

        /// <summary>
        /// Gets or sets the palette to be applied.
        /// </summary>
        [Category("Visuals")]
        [Description("Palette applied to drawing.")]
        public PaletteMode PaletteMode {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _paletteMode; }

            set {
                if (_paletteMode != value)
                {
                    // Action despends on new value
                    switch (value)
                    {
                        case PaletteMode.Custom:
                            // Do nothing, you must assign a palette to the 
                            // 'Palette' property in order to get the custom mode
                            break;
                        default:
                            // Use the new value
                            _paletteMode = value;

                            // Get a reference to the standard palette from its name
                            _localPalette = null;
                            SetPalette(KryptonManager.GetPaletteForMode(_paletteMode));

                            // Must raise event to change palette in redirector
                            OnPaletteChanged(EventArgs.Empty);

                            // Need to layout again use new palette
                            PerformLayout();
                            break;
                    }
                }
            }
        }

        private bool ShouldSerializePaletteMode()
        {
            return (PaletteMode != PaletteMode.Global);
        }

        /// <summary>
        /// Resets the PaletteMode property to its default value.
        /// </summary>
        public void ResetPaletteMode()
        {
            PaletteMode = PaletteMode.Global;
        }

        /// <summary>
        /// Gets and sets the custom palette implementation.
        /// </summary>
        [Category("Visuals")]
        [Description("Custom palette applied to drawing.")]
        [DefaultValue(null)]
        public IPalette Palette {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _localPalette; }

            set {
                // Only interested in changes of value
                if (_localPalette != value)
                {
                    // Remember the starting palette
                    IPalette old = _localPalette;

                    // Use the provided palette value
                    SetPalette(value);

                    // If no custom palette is required
                    if (value == null)
                    {
                        // No custom palette, so revert back to the global setting
                        _paletteMode = PaletteMode.Global;

                        // Get the appropriate palette for the global mode
                        _localPalette = null;
                        SetPalette(KryptonManager.GetPaletteForMode(_paletteMode));
                    }
                    else
                    {
                        // No longer using a standard palette
                        _localPalette = value;
                        _paletteMode = PaletteMode.Custom;
                    }

                    // If real change has occured
                    if (old != _localPalette)
                    {
                        // Raise the change event
                        OnPaletteChanged(EventArgs.Empty);

                        // Need to layout again use new palette
                        PerformLayout();
                    }
                }
            }
        }

        /// <summary>
        /// Resets the Palette property to its default value.
        /// </summary>
        public void ResetPalette()
        {
            PaletteMode = PaletteMode.Global;
        }

        /// <summary>
        /// Gets access to the current renderer.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IRenderer Renderer {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _renderer; }
        }

        /// <summary>
        /// Fires the NeedPaint event.
        /// </summary>
        /// <param name="needLayout">Does the palette change require a layout.</param>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void PerformNeedPaint(bool needLayout)
        {
            OnNeedPaint(this, new NeedLayoutEventArgs(needLayout));
        }

        /// <summary>
        /// Gets the resolved palette to actually use when drawing.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IPalette GetResolvedPalette()
        {
            return _palette;
        }

        /// <summary>
        /// Create a tool strip renderer appropriate for the current renderer/palette pair.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public ToolStripRenderer CreateToolStripRenderer()
        {
            return Renderer.RenderToolStrip(GetResolvedPalette());
        }

        /// <summary>
        /// Send the provided system command to the window for processing.
        /// </summary>
        /// <param name="sysCommand">System command.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SecuritySafeCritical]
        public void SendSysCommand(int sysCommand)
        {
            SendSysCommand(sysCommand, IntPtr.Zero);
        }

        /// <summary>
        /// Send the provided system command to the window for processing.
        /// </summary>
        /// <param name="sysCommand">System command.</param>
        /// <param name="lParam">LPARAM value.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SecuritySafeCritical]
        public void SendSysCommand(int sysCommand, IntPtr lParam)
        {
            // Send window message to ourself
            PI.SendMessage(Handle, PI.WM_SYSCOMMAND, (IntPtr)sysCommand, lParam);
        }

//        /// <summary>
//        /// 
//        /// </summary>
//        [Browsable(false)]
//        [EditorBrowsable(EditorBrowsableState.Never)]
//        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
//        public bool IsOnePixelBorder {
//            get { return _isOnePixelBorder; }
//            set {
//                if (_isOnePixelBorder == value)
//                    return;

//                _isOnePixelBorder = value;

//                RecalcSizeFromClientSize(); // should be before UpdateShadowType !
//                UpdateShadowState();
//            }
//        }

        ///// <summary>
        ///// 
        ///// </summary>
        //[Browsable(false)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public bool IsModernForm {
        //    get { return IsOnePixelBorder; }
        //}

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsCustomHeader {
            get {
                //betauser
                if( NeoAxis.Editor.WinFormsUtility.IsDesignerHosted( this ) )
                    return false;
                return AllowFormChrome;
                //return KryptonToolkitSettings.AllowFormChrome;
                //if (_palette.GetAllowFormChrome() == InheritBool.False)
                //    return false;
                //return ApplyCustomChrome && !ApplyComposition;
            }
        }

        /// <summary>
        /// Gets the size of the borders
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Padding WindowMargin {
            get { return CalculateDynamicWindowMargin(); }
        }

        /// <summary>
        /// Calculates the client margin based on the current form and form element settings
        /// </summary>
        /// <returns></returns>
        protected virtual Padding CalculateDynamicWindowMargin()
        {
            if( !IsCustomHeader)
                return CommonHelper.GetWindowRealNCMargin(CreateParams);

            Padding margin = new Padding();
            if (!CommonHelper.IsFormMaximized(this) || CommonHelper.IsFormMaximized(this) && this.MaximumSize != Size.Empty)
            {
                if (this.FormBorderStyle != FormBorderStyle.None)
                {
                    margin = _palette.GetFormMargin();

                    if (this.IsMdiChild && CommonHelper.IsFormMinimized(this))
                        margin.Top = Size.Height;
                    else
                        margin.Top = GetDynamicCaptionHeight();
                }
                else
                    margin = Padding.Empty;
            }
            else
            {
                margin = CommonHelper.GetWindowRealNCMargin(CreateParams);
                if (this.IsMdiChild)
                {
                    if (this.MainMenuStrip != null)
                        margin.Top -= SystemInformation.MenuHeight;
                }
                else
                {
                    margin.Top = GetDynamicCaptionHeight();
                }
            }
            return margin;
        }

        /// <summary>
        /// Gets the Caption Height according to the Styles applied and
        /// the current state of the form.
        /// </summary>
        /// <returns>A value representing the height of the caption.</returns>
        protected virtual int GetDynamicCaptionHeight()
        {
            if (this.FormBorderStyle == FormBorderStyle.None)
                return 0;

            int height = _palette.GetFormCaptionHeight() + _palette.GetFormMargin().Top;

            if (CommonHelper.IsFormMaximized(this) && this.MaximumSize == Size.Empty)
            {
                if (DWM.IsCompositionEnabled)
                    height += SystemInformation.FrameBorderSize.Height;
                else
                    height += SystemInformation.FixedFrameBorderSize.Height;
            }

            return height;
        }

        /// <summary>
        /// Gets a count of the number of paints that have occured.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PaintCount {
            get { return _paintCount; }
        }

        /// <summary>
        /// Gets and sets the active state of the window.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WindowActive {
            get { return _windowActive; }

            set {
                if (_windowActive != value)
                {
                    _windowActive = value;
                    OnWindowActiveChanged();
                }
            }
        }

        /// <summary>
        /// Request the non-client area be repainted.
        /// </summary>
        public void RedrawNonClient()
        {
            InvalidateNonClient(Rectangle.Empty, true);
        }

        /// <summary>
        /// Request the non-client area be recalculated.
        /// </summary>
        [SecuritySafeCritical]
        public void RecalcNonClient()
        {
            if (!IsDisposed && !Disposing && IsHandleCreated)
            {
                PI.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0,
                                (uint)(PI.SWP_NOACTIVATE | PI.SWP_NOMOVE |
                                       PI.SWP_NOZORDER | PI.SWP_NOSIZE |
                                       PI.SWP_NOOWNERZORDER | PI.SWP_FRAMECHANGED));
            }
        }
#endregion

//#region Public Chrome
//        /// <summary>
//        /// Perform layout on behalf of the composition element using our root element.
//        /// </summary>
//        /// <param name="context">Layout context.</param>
//        /// <param name="compRect">Rectangle for composition element.</param>
//        public virtual void WindowChromeCompositionLayout(ViewLayoutContext context,
//                                                          Rectangle compRect)
//        {
//        }

//        /// <summary>
//        /// Perform painting on behalf of the composition element using our root element.
//        /// </summary>
//        /// <param name="context">Rendering context.</param>
//        public virtual void WindowChromeCompositionPaint(RenderContext context)
//        {
//        }
//#endregion

#region Public IKryptonDebug
        /// <summary>
        /// Reset the internal counters.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void KryptonResetCounters()
        {
            ViewManager.ResetCounters();
        }

        /// <summary>
        /// Gets the number of layout cycles performed since last reset.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int KryptonLayoutCounter {
            get { return ViewManager.LayoutCounter; }
        }

        /// <summary>
        /// Gets the number of paint cycles performed since last reset.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int KryptonPaintCounter {
            get { return ViewManager.PaintCounter; }
        }
#endregion

#region Protected
        /// <summary>
        /// Gets and sets the ViewManager instance.
        /// </summary>
        protected ViewManager ViewManager {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _viewManager; }
            set { _viewManager = value; }
        }

        /// <summary>
        /// Gets access to the palette redirector.
        /// </summary>
        protected PaletteRedirect Redirector {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _redirector; }
        }

        /// <summary>
        /// Gets access to the need paint delegate.
        /// </summary>
        protected NeedPaintHandler NeedPaintDelegate {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _needPaintDelegate; }
        }

        /// <summary>
        /// Convert a screen location to a window location.
        /// </summary>
        /// <param name="screenPt">Screen point.</param>
        /// <returns>Point in window coordinates.</returns>
        protected Point ScreenToWindow(Point screenPt)
        {
            // First of all convert to client coordinates
            Point clientPt = PointToClient(screenPt);

            // Now adjust to take into account the top and left borders
            Padding borders = WindowMargin;
            clientPt.Offset(borders.Left, (/*ApplyComposition ? 0 : */borders.Top));

            return clientPt;
        }

        /// <summary>
        /// Request the non-client area be repainted.
        /// </summary>
        public void InvalidateNonClient()
        {
            InvalidateNonClient(Rectangle.Empty, true);
        }

        /// <summary>
        /// Request the non-client area be repainted.
        /// </summary>
        /// <param name="invalidRect">Area to invalidate.</param>
        protected void InvalidateNonClient(Rectangle invalidRect)
        {
            InvalidateNonClient(invalidRect, true);
        }

        /// <summary>
        /// Request the non-client area be repainted.
        /// </summary>
        /// <param name="invalidRect">Area to invalidate.</param>
        /// <param name="excludeClientArea">Should client area be excluded.</param>
        [SecuritySafeCritical]
        protected void InvalidateNonClient(Rectangle invalidRect,
                                           bool excludeClientArea)
        {
            try
            {
                if( !IsDisposed && !Disposing && IsHandleCreated )
                {
                    if( invalidRect.IsEmpty )
                    {
                        Padding realWindowBorders = WindowMargin;
                        Rectangle realWindowRectangle = RealWindowRectangle;

                        invalidRect = new Rectangle( -realWindowBorders.Left,
                                                    -realWindowBorders.Top,
                                                    realWindowRectangle.Width,
                                                    realWindowRectangle.Height );
                    }

                    using( Region invalidRegion = new Region( invalidRect ) )
                    {
                        if( excludeClientArea )
                            invalidRegion.Exclude( ClientRectangle );

                        using( Graphics g = Graphics.FromHwnd( Handle ) )
                        {
                            IntPtr hRgn = invalidRegion.GetHrgn( g );

                            PI.RedrawWindow( Handle, IntPtr.Zero, hRgn,
                                            (uint)( PI.RDW_FRAME | PI.RDW_UPDATENOW | PI.RDW_INVALIDATE ) );

                            PI.DeleteObject( hRgn );
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Gets rectangle that is the real window rectangle based on Win32 API call.
        /// </summary>
        protected Rectangle RealWindowRectangle {
            [SecuritySafeCritical]
            get {
                // Grab the actual current size of the window, this is more accurate than using
                // the 'this.Size' which is out of date when performing a resize of the window.
                PI.RECT windowRect = new PI.RECT();
                PI.GetWindowRect(Handle, ref windowRect);

                // Create rectangle that encloses the entire window
                return new Rectangle(0, 0,
                                     windowRect.right - windowRect.left,
                                     windowRect.bottom - windowRect.top);
            }
        }
#endregion

#region Protected Override
        /// <summary>
        /// Raises the HandleCreated event.
        /// </summary>
        /// <param name="e">An EventArgs containing the event data.</param>
        [SecuritySafeCritical]
        protected override void OnHandleCreated(EventArgs e)
        {
            if( RealAllowLockFormUpdate )
            {
                KryptonWinFormsUtility.LockFormUpdate( this );
                needUnlockFormUpdate = true;
            }

            // Can fail on versions before XP SP1
            try
            {
                // Prevent the OS from drawing the non-client area in classic look
                // if the application stops responding to windows messages
                PI.DisableProcessWindowsGhosting();
            }
            catch { }

            base.OnHandleCreated(e);
        }

        /// <summary>
        /// Start capturing mouse input for a particular element that is inside the chrome.
        /// </summary>
        /// <param name="element">Target element for the capture events.</param>
        protected void StartCapture(ViewBase element)
        {
            // Capture mouse input so we notice the WM_LBUTTONUP when the mouse is released
            Capture = true;
            _captured = true;

            // Remember the view element that wants the mouse input during capture
            _capturedElement = element;
        }

        /// <summary>
        /// Raises the Resize event.
        /// </summary>
        /// <param name="e">An EventArgs containing the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            //!!!!new
            //// Allow an extra region change to occur during resize
            //ResumePaint();

            base.OnResize(e);

            if (ApplyCustomChrome && !((MdiParent != null) && CommonHelper.IsFormMaximized(this)))
                PerformNeedPaint(true);

            //!!!!new
            //// Reverse the resume from earlier
            //SuspendPaint();
        }

        private int _inWmWindowPosChanged;
        private bool _creatingHandle = false;
        private Size _savedClientSize;
        private bool _shouldPatchClientSize;

        [Flags]
        enum DwmWindowAttribute : uint
        {
            NCRenderingEnabled = 1,
            NCRenderingPolicy,
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,
            NonClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,
            HasIconicBitmap,
            DisallowPeek,
            ExcludedFromPeek,
            Cloak,
            Cloaked,
            FreezeRepresentation,
            PassiveUpdateMode,
            UseHostBackdropBrush,
            UseImmersiveDarkMode = 20,
            WindowCornerPreference = 33,
            BorderColor,
            CaptionColor,
            TextColor,
            VisibleFrameBorderThickness,
            Last
        }

        [DllImport( "dwmapi.dll", PreserveSig = true )]
        static extern int DwmSetWindowAttribute( IntPtr hwnd, DwmWindowAttribute attr, ref int attrValue, int attrSize );

        public void ApplyWindows11DarkTheme()
        {
            if( IsHandleCreated && !NeoAxis.Editor.WinFormsUtility.IsDesignerHosted( this ) && KryptonDarkThemeUtility.DarkTheme )
            {
                try
                {
                    int value = 1;
                    DwmSetWindowAttribute( Handle, DwmWindowAttribute.UseImmersiveDarkMode, ref value, 4 );
                }
                catch { }
            }
        }

        protected override void CreateHandle()
        {
            bool shouldPatchSize = !_creatingHandle;
            _creatingHandle = true;

            if( !NeoAxis.Editor.WinFormsUtility.IsDesignerHosted( this ) )//betauser
            {
                if( shouldPatchSize )
                    if( WindowState != FormWindowState.Minimized && !DesignMode )
                        Size = SizeFromClientSize( ClientSize );
            }

            if (!IsHandleCreated && !IsDisposed)
                base.CreateHandle();

            ApplyWindows11DarkTheme();

            _creatingHandle = false;
        }

        private void RecalcSizeFromClientSize()
        {
            if( !NeoAxis.Editor.WinFormsUtility.IsDesignerHosted( this ) )//betauser
            {
                // HACK? We break the bounds for restoring from maximized state if we call SetClientSizeCore in the maximized state.
                // see Form.restoredWindowBounds etc.
                if( WindowState == FormWindowState.Maximized )
                    return;
                //

                //Debug.WriteLine($"EDITOR RecalcSizeFromClientSize()");
                SetClientSizeCore( ClientSize.Width, ClientSize.Height );
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if( IsCustomHeader && FormBorderStyle != FormBorderStyle.None && ( ControlBox || !string.IsNullOrEmpty( Text ) ) && WindowState == FormWindowState.Normal )
                PatchClientSize();
            else if( _shouldPatchClientSize && WindowState != FormWindowState.Minimized )
                PatchClientSize();

            base.OnSizeChanged(e);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            if( _shouldPatchClientSize )
                PatchClientSize();
            base.OnClientSizeChanged(e);
        }

        private void PatchClientSize()
        {
            _savedClientSize = GetClientSizeFromSize( Size );

            if( ClientSize != _savedClientSize )
            {
                //var before = ClientSize;

                FieldInfo fiWidth = typeof( Control ).GetField( "_clientWidth", BindingFlags.Instance | BindingFlags.NonPublic );
                FieldInfo fiHeight = typeof( Control ).GetField( "_clientHeight", BindingFlags.Instance | BindingFlags.NonPublic );
                fiWidth.SetValue( this, _savedClientSize.Width );
                fiHeight.SetValue( this, _savedClientSize.Height );

                //Debug.WriteLine($"EDITOR: PatchClientSize(): ClientSize={before} => {ClientSize}");
            }
        }

        /// <summary>
        /// Recalc Size from ClientSize
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected override void SetClientSizeCore(int x, int y)
        {
            base.SetClientSizeCore( x, y );

            //if( !IsCustomHeader )
            //    return;

            //try
            //{
            //    FieldInfo clientWidthField = typeof( Control ).GetField( "_clientWidth", BindingFlags.Instance | BindingFlags.NonPublic );
            //    FieldInfo clientHeightField = typeof( Control ).GetField( "_clientHeight", BindingFlags.Instance | BindingFlags.NonPublic );
            //    FieldInfo formStateSetClientSizeField = typeof( Form ).GetField( "FormStateSetClientSize", BindingFlags.Static | BindingFlags.NonPublic );
            //    FieldInfo formStateField = typeof( Form ).GetField( "formState", BindingFlags.Instance | BindingFlags.NonPublic );

            //    if( clientWidthField != null && clientHeightField != null &&
            //        formStateField != null && formStateSetClientSizeField != null )
            //    {
            //        Size = SizeFromClientSize( new Size( x, y ) );

            //        clientWidthField.SetValue( this, x );
            //        clientHeightField.SetValue( this, y );

            //        //Debug.WriteLine($"EDITOR: SetClientSizeCore(): ClientSize={new Size(x, y)} Size={Size}");

            //        BitVector32.Section index = (BitVector32.Section)formStateSetClientSizeField.GetValue( this );
            //        BitVector32 bitVector32 = (BitVector32)formStateField.GetValue( this );

            //        bitVector32[ index ] = 1;
            //        formStateField.SetValue( this, bitVector32 );

            //        this.OnClientSizeChanged( EventArgs.Empty );

            //        bitVector32[ index ] = 0;
            //        formStateField.SetValue( this, bitVector32 );
            //    }
            //    else
            //        base.SetClientSizeCore( x, y );
            //}
            //catch { }
        }

        /// <summary>
        /// Returns Size for ClientSize
        /// </summary>
        /// <param name="clientSize"></param>
        /// <returns></returns>
        protected override Size SizeFromClientSize(Size clientSize)
        {
            if (!IsCustomHeader)
                return base.SizeFromClientSize(clientSize);

            clientSize.Width += WindowMargin.Horizontal;
            clientSize.Height += WindowMargin.Vertical;

            return clientSize;
        }

        /// <summary>
        /// Returns ClientSize for Size
        /// </summary>
        /// <param name="formSize"></param>
        /// <returns></returns>
		protected virtual Size GetClientSizeFromSize(Size formSize)
        {
            if (formSize == Size.Empty)
                return Size.Empty;

            Size size1 = SizeFromClientSize( Size.Empty );
            Size size2 = new Size( formSize.Width - size1.Width, formSize.Height - size1.Height );
            if( this.WindowState != FormWindowState.Maximized )
                return size2;

            PI.RECT rect = new PI.RECT( 0, 0, size2.Width, size2.Height );
            //Taskbar.CorrectRECTByAutoHide(ref rect);
            return new Size( rect.right, rect.bottom );
        }

        /// <summary>
        /// Performs the work of setting the specified bounds of this control.
        /// </summary>
        /// <param name="x">The new Left property value of the control.</param>
        /// <param name="y">The new Top property value of the control.</param>
        /// <param name="width">The new Width property value of the control.</param>
        /// <param name="height">The new Height property value of the control.</param>
        /// <param name="specified">A bitwise combination of the BoundsSpecified values.</param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // Patch form size when restoring
            Size size = PatchFormSizeInRestoreWindowBoundsIfNecessary( width, height );
            //size = CalcPreferredSizeCore( size );
            base.SetBoundsCore( x, y, size.Width, size.Height, specified );
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="size"></param>
        ///// <returns></returns>
        //protected virtual Size CalcPreferredSizeCore(Size size)
        //{
        //    //// With the Aero glass appearance we need to reduce height by the top border, 
        //    //// otherwise each time the window is maximized and restored it grows in size
        //    //if (ApplyComposition && FormBorderStyle != FormBorderStyle.None)
        //    //    size.Height -= WindowMargin.Top;

        //    return size;
        //}

        /// <summary>
        /// Patch form size when restoring
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected virtual Size PatchFormSizeInRestoreWindowBoundsIfNecessary(int width, int height)
        {
            if( IsCustomHeader && WindowState == FormWindowState.Normal && _inWmWindowPosChanged > 0 )
            {
                try
                {
                    FieldInfo restoredWindowBoundsSpecifiedField = typeof( Form ).GetField( "restoredWindowBoundsSpecified", BindingFlags.Instance | BindingFlags.NonPublic );
                    BoundsSpecified restoredSpecified = (BoundsSpecified)restoredWindowBoundsSpecifiedField.GetValue( this );
                    if( ( restoredSpecified & BoundsSpecified.Size ) != BoundsSpecified.None )
                    {
                        FieldInfo restoredWindowBoundsField = typeof( Form ).GetField( "restoredWindowBounds", BindingFlags.Instance | BindingFlags.NonPublic );
                        if( restoredWindowBoundsField != null )
                        {
                            Rectangle restoredWindowBounds = (Rectangle)restoredWindowBoundsField.GetValue( this );
                            if( IsFormStateClientSizeSet() )
                            {
                                width = restoredWindowBounds.Width + WindowMargin.Horizontal;
                                height = restoredWindowBounds.Height + WindowMargin.Vertical;
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return new Size(width, height);
        }

        private bool IsFormStateClientSizeSet()
        {
            FieldInfo formStateExWindowBoundsWidthIsClientSize = typeof( Form ).GetField( "FormStateExWindowBoundsWidthIsClientSize", BindingFlags.Static | BindingFlags.NonPublic );
            FieldInfo formStateEx = typeof( Form ).GetField( "formStateEx", BindingFlags.Instance | BindingFlags.NonPublic );
            if( formStateExWindowBoundsWidthIsClientSize == null || formStateEx == null )
                return false;
            BitVector32.Section index = (BitVector32.Section)formStateExWindowBoundsWidthIsClientSize.GetValue( this );
            BitVector32 state = (BitVector32)formStateEx.GetValue( this );
            return state[ index ] == 1;
        }

        /// <summary>
        /// Raises the Activated event.
        /// </summary>
        /// <param name="e">An EventArgs containing the event data.</param>
        protected override void OnActivated(EventArgs e)
        {
            WindowActive = true;
            base.OnActivated(e);
        }

        /// <summary>
        /// Raises the Deactivate event.
        /// </summary>
        /// <param name="e">An EventArgs containing the event data.</param>
        protected override void OnDeactivate(EventArgs e)
        {
            WindowActive = false;
            base.OnDeactivate(e);
        }

        /// <summary>
        /// Raises the PaintBackground event.
        /// </summary>
        /// <param name="e">A PaintEventArgs containing event data.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //// If drawing with custom chrome and composition
            //if( ApplyCustomChrome && ApplyComposition)
            //{
            //    Rectangle compositionRect = new Rectangle(0, 0, Width, _compositionHeight);

            //    // Draw the extended area inside the client in black, this ensures
            //    // it is treated as transparent by the desktop window manager
            //    e.Graphics.FillRectangle(Brushes.Black, compositionRect);

            //    // Exclude the composition area from the rest of the background painting
            //    e.Graphics.SetClip(compositionRect, CombineMode.Exclude);
            //}

            base.OnPaintBackground(e);
        }

        /// <summary>
        /// Raises the Shown event.
        /// </summary>
        /// <param name="e">An EventArgs containing event data.</param>
        protected override void OnShown(EventArgs e)
        {
            //// Under Windows7 a modal window with custom chrome under the DWM
            //// will sometimes not be drawn when first shown.
            //if( Environment.OSVersion.Version.Major >= 6 )
            //    PerformNeedPaint( true );

            base.OnShown(e);
        }
#endregion

#region Protected Virtual
        /// <summary>
        /// Suspend processing of non-client painting.
        /// </summary>
        protected virtual void SuspendPaint()
        {
            _ignoreCount++;
        }

        /// <summary>
        /// Resume processing of non-client painting.
        /// </summary>
        protected virtual void ResumePaint()
        {
            _ignoreCount--;
        }

        /// <summary>
        /// Create the redirector instance.
        /// </summary>
        /// <returns>PaletteRedirect derived class.</returns>
        protected virtual PaletteRedirect CreateRedirector()
        {
            return new PaletteRedirect(_palette);
        }

        /// <summary>
        /// Processes a notification from palette storage of a button spec change.
        /// </summary>
        /// <param name="sender">Source of notification.</param>
        /// <param name="e">An EventArgs containing event data.</param>
        protected virtual void OnButtonSpecChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Raises the PaletteChanged event.
        /// </summary>
        /// <param name="e">An EventArgs containing the event data.</param>
        protected virtual void OnPaletteChanged(EventArgs e)
        {
            // Update the redirector with latest palette
            Redirector.Target = _palette;

            // A new palette source means we need to layout and redraw
            OnNeedPaint(Palette, new NeedLayoutEventArgs(true));

            if (PaletteChanged != null)
                PaletteChanged(this, e);
        }

        /// <summary>
        /// Raises the ApplyCustomChrome event.
        /// </summary>
        /// <param name="e">An EventArgs containing the event data.</param>
        protected virtual void OnApplyCustomChromeChanged(EventArgs e)
        {
            if (ApplyCustomChromeChanged != null)
                ApplyCustomChromeChanged(this, e);
        }

        /// <summary>
        /// Occurs when the AllowFormChromeChanged event is fired for the current palette.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">An EventArgs containing the event data.</param>
        protected virtual void OnAllowFormChromeChanged(object sender, EventArgs e)
        {
        }

        // HACK ?
        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreRepaint { get; set; } = false;

        /// <summary>
        /// Processes a notification from palette storage of a paint and optional layout required.
        /// </summary>
        /// <param name="sender">Source of notification.</param>
        /// <param name="e">An NeedLayoutEventArgs containing event data.</param>
        protected virtual void OnNeedPaint(object sender, NeedLayoutEventArgs e)
        {
            Debug.Assert(e != null);

            // Validate incoming reference
            if (e == null) throw new ArgumentNullException("e");

            if (IgnoreRepaint)
                return;

            // Do nothing unless we are applying custom chrome
            if (ApplyCustomChrome)
            {
                //// If using composition drawing
                //if (ApplyComposition)
                //{
                //    // Ask the composition element top handle need paint event
                //    if (_compositionElement != null)
                //        _compositionElement.CompNeedPaint(e.NeedLayout);
                //}
                //else
                {
                    // Do we need to recalc the border size as well as invalidate?
                    if (e.NeedLayout)
                        _needLayout = true;

                    InvalidateNonClient();
                }
            }
        }

        /// <summary>
        /// Process Windows-based messages.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        [SecuritySafeCritical]
        [DebuggerNonUserCode]
        protected override void WndProc(ref Message m)
        {
            bool processed = false;

            // We do not process the message if on an MDI child, because doing so prevents the 
            // LayoutMdi call on the parent from working and cascading/tiling the children
            //if ((m.Msg == (int)PI.WM_NCCALCSIZE) && _themedApp &&
            //    ((MdiParent == null) || ApplyCustomChrome))
            if( ApplyCustomChrome || MdiParent == null)
            {
                switch (m.Msg)
                {
                    case PI.WM_NCCALCSIZE:
                        processed = OnWM_NCCALCSIZE(ref m);
                        break;
                    case PI.WM_GETMINMAXINFO:
                        OnWM_GETMINMAXINFO(ref m);
                        //WmGetMinMaxInfo(m.HWnd, m.LParam);
                        /* Setting handled to false enables the application to process it's own Min/Max requirements,
						 * as mentioned by jason.bullard (comment from September 22, 2011) on http://gallery.expression.microsoft.com/ZuneWindowBehavior/ */
                        processed = false;
                        break;
                }
            }

            // Do we need to override message processing?
            if (ApplyCustomChrome && !IsDisposed && !Disposing)
            {
                switch (m.Msg)
                {
                    case PI.WM_MOVE:
                        {
                            if( IsCustomHeader && MdiParent == null && WindowState != FormWindowState.Maximized )
                                _shouldPatchClientSize = true;
                        }
                        break;
                    case PI.WM_WINDOWPOSCHANGED:
                        {
                            if( IsCustomHeader && MdiParent == null && WindowState != FormWindowState.Maximized )
                                _shouldPatchClientSize = true;

                            _inWmWindowPosChanged++;
                            base.WndProc(ref m);
                            _inWmWindowPosChanged--;

                            processed = true;
                        }
                        break;
                    case PI.WM_NCPAINT:
                        //if (!ApplyComposition)
                        {
                            if (_ignoreCount <= 0)
                                processed = OnWM_NCPAINT(ref m);
                            else
                                processed = true;
                        }
                        break;
                    case PI.WM_NCHITTEST:
                        //if (ApplyComposition)
                        //    processed = OnCompWM_NCHITTEST(ref m);
                        //else
                            processed = OnWM_NCHITTEST(ref m);
                        break;
                    case PI.WM_NCACTIVATE:
                        processed = OnWM_NCACTIVATE(ref m);
                        break;
                    case PI.WM_NCMOUSEMOVE:
                        processed = OnWM_NCMOUSEMOVE(ref m);
                        break;
                    case PI.WM_NCLBUTTONDOWN:
                        processed = OnWM_NCLBUTTONDOWN(ref m);
                        break;
                    case PI.WM_NCLBUTTONUP:
                        processed = OnWM_NCLBUTTONUP(ref m);
                        break;
                    case PI.WM_MOUSEMOVE:
                        if (_captured)
                            processed = OnWM_MOUSEMOVE(ref m);
                        break;
                    case PI.WM_LBUTTONUP:
                        if (_captured)
                            processed = OnWM_LBUTTONUP(ref m);
                        break;
                    case PI.WM_NCMOUSELEAVE:
                        if (!_captured)
                            processed = OnWM_NCMOUSELEAVE(ref m);

                        //if (ApplyComposition)
                        //{
                        //    // Must repaint the composition area not that mouse has left
                        //    if (_compositionElement != null)
                        //        _compositionElement.CompNeedPaint(true);
                        //}
                        break;
                    case PI.WM_NCLBUTTONDBLCLK:
                        processed = OnWM_NCLBUTTONDBLCLK(ref m);
                        break;
                    case PI.WM_SYSCOMMAND:
                        // Is this the command for closing the form?
                        if ((int)m.WParam.ToInt64() == PI.SC_CLOSE)
                        {
                            PropertyInfo pi = typeof(Form).GetProperty("CloseReason",
                                                                        BindingFlags.Instance |
                                                                        BindingFlags.SetProperty |
                                                                        BindingFlags.NonPublic);

                            // Update form with the reason for the close
                            pi.SetValue(this, CloseReason.UserClosing, null);
                        }

                        if ((int)m.WParam.ToInt64() != 61696)
                            processed = OnPaintNonClient(ref m);
                        break;
                    case PI.WM_INITMENU:
                    case PI.WM_SETTEXT:
                    case PI.WM_HELP:
                        processed = OnPaintNonClient(ref m);
                        break;
                    case 0x00AE:
                        // Mystery message causes title bar buttons to draw, we want to 
                        // prevent that and ignoring the messages seems to do no harm.
                        processed = true;
                        break;
                    //case 0xC1BC:
                    //    // Under Windows7 a modal window with custom chrome under the DWM
                    //    // will sometimes not be drawn when first shown. So we spot the window
                    //    // message used to indicate a window is shown and manually request layout
                    //    // and paint of the non-client area to get it shown.
                    //    PerformNeedPaint(true);
                        //break;
                }
            }

            // If the message has not been handled, let base class process it
            if (!processed && m.Msg != PI.WM_GETMINMAXINFO)
                base.WndProc(ref m);

            _shouldPatchClientSize = false;
        }

        /// <summary>
        /// Creates and populates the MINMAXINFO structure for a maximized window.
        /// Puts the structure into memory address given by lParam.
        /// Only used to process a WM_GETMINMAXINFO message.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual void OnWM_GETMINMAXINFO(ref Message m)
        {
            // !!!!: dont use Screen.FromHandle(m.HWnd) in OnWM_GETMINMAXINFO
            // this is the cause of performance loss. When the form is moved, there are big lags/delays.

            PI.MINMAXINFO mmi = (PI.MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(PI.MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = PI.MonitorFromWindow(m.HWnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                PI.MONITORINFO monitorInfo = new PI.MONITORINFO();
                PI.GetMonitorInfo(monitor, monitorInfo);
                PI.RECT rcWorkArea = monitorInfo.rcWork;
                PI.RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, m.LParam, true);
        }

        /// <summary>
        /// Process the WM_NCCALCSIZE message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnWM_NCCALCSIZE(ref Message m)
        {
            // Get the border sizing needed around the client area
            Padding borders = WindowMargin;

            //Debug.WriteLine($"EDITOR: OnWM_NCCALCSIZE() RealWindowBorders={borders} ApplyComposition={ApplyComposition}");

            if (m.WParam == PI.TRUE)
            {
                //// If using composition in the custom chrome
                //if (ApplyComposition)
                //{
                //    // Do not provide any border at the top, instead we extend the glass
                //    // at the top into the client area so that we can custom draw onto the
                //    // extended glass area. 
                //    borders.Top = 0;

                //    // workaround for auto-hide taskbar form overlap.
                //    if (CommonHelper.IsFormMaximized(this))
                //        borders.Bottom += 2; // 2 pixels for taskbar
                //}

                PI.NCCALCSIZE_PARAMS calcsize = (PI.NCCALCSIZE_PARAMS)m.GetLParam(typeof(PI.NCCALCSIZE_PARAMS));

                // Reduce provided RECT by the borders
                calcsize.rectProposed.left += borders.Left;
                calcsize.rectProposed.top += borders.Top;
                calcsize.rectProposed.right -= borders.Right;
                calcsize.rectProposed.bottom -= borders.Bottom;

                //!!!!
                //if( debug )
                //{
                //    MessageBox.Show( calcsize.rectProposed.ToRectangle().ToString() + " " + borders.ToString() );

                //    //if( !debug2 )
                //    //{
                //    //    calcsize.rectProposed.left = 720;
                //    //    calcsize.rectProposed.top = 243;
                //    //    calcsize.rectProposed.right = 720 + 479;
                //    //    calcsize.rectProposed.bottom = 243 + 632;

                //    //    //calcsize.rectProposed.left = 0;
                //    //    //calcsize.rectProposed.top = 0;
                //    //    //calcsize.rectProposed.right = 0;
                //    //    //calcsize.rectProposed.bottom = 0;
                //    //}
                //    debug2 = true;
                //}

                Marshal.StructureToPtr(calcsize, m.LParam, false);

                // Message processed, do not pass onto base class for processing
                return true;
            }
            else
            {
                PI.RECT rect = (PI.RECT)Marshal.PtrToStructure(m.LParam, typeof(PI.RECT));

                //!!!!
                //MessageBox.Show( rect.ToRectangle().ToString() );

                rect.top += borders.Top;
                rect.bottom -= borders.Bottom;
                rect.left += borders.Left;
                rect.right -= borders.Right;
                //if (MdiParent != null)
                //	BaseWndProc(ref m);
                
                //!!!!
                //if( debug )
                //{
                //    rect.left = 720;
                //    rect.top = 243;
                //    rect.right = 720 + 479;
                //    rect.bottom = 243 + 632;
                //}


                Marshal.StructureToPtr(rect, m.LParam, false);
                m.Result = IntPtr.Zero;
                return true;
            }
        }

        bool _inNCPaint_WINEHACK;

        /// <summary>
        /// Process the WM_NCPAINT message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnWM_NCPAINT(ref Message m)
        {
            // HACK: prevent form client size from changing at chrome mode switch
            if ( _inNCPaint_WINEHACK /*&& PI.IsWine()*/)
            {
                //Debug.Assert(PI.IsWine());
                m.Result = (IntPtr)(1);
                return true;
            }
            _inNCPaint_WINEHACK = true;
            //

            // Perform actual paint operation
            if (!_disposing)
                OnNonClientPaint(m.HWnd);

            _inNCPaint_WINEHACK = false;

            // We have handled the message
            m.Result = (IntPtr)(1);

            // Message processed, do not pass onto base class for processing
            return true;
        }

        /// <summary>
        /// Process the WM_NCHITTEST message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnWM_NCHITTEST(ref Message m)
        {
            // Extract the point in screen coordinates
            Point screenPoint = new Point((int)m.LParam.ToInt64());

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);

            // Perform hit testing
            m.Result = WindowChromeHitTest(windowPoint, false);

            // Message processed, do not pass onto base class for processing
            return true;
        }

        /// <summary>
        /// Process the WM_NCHITTEST message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnCompWM_NCHITTEST(ref Message m)
        {
            // Let the desktop window manager process it first
            IntPtr result;
            PI.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, out result);
            m.Result = result;

            // If no result returned then let the base window routine process it
            if (m.Result == (IntPtr)PI.HTNOWHERE)
                DefWndProc(ref m);

            // If the window proc has decided it is in the CAPTION or CLIENT areas
            // then we might have something of our own in that area that we want to
            // override the return value for. So process it ourself.
            if ((m.Result == (IntPtr)PI.HTCAPTION) ||
                (m.Result == (IntPtr)PI.HTCLIENT))
            {
                // Extract the point in screen coordinates
                Point screenPoint = new Point((int)m.LParam.ToInt64());

                // Convert to window coordinates
                Point windowPoint = ScreenToWindow(screenPoint);

                // Perform hit testing
                m.Result = WindowChromeHitTest(windowPoint, true);
            }

            // Message processed, do not pass onto base class for processing
            return true;
        }

        /// <summary>
        /// Process the WM_NCACTIVATE message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnWM_NCACTIVATE(ref Message m)
        {
            // Cache the new active state
            WindowActive = m.WParam == PI.TRUE;

            //if (!ApplyComposition)
            {
                // The first time an MDI child gets an WM_NCACTIVATE, let it process as normal
                if ((MdiParent != null) && !_activated)
                    _activated = true;
                else
                {
                    // Allow default processing of activation change
                    m.Result = PI.TRUE;

                    // Message processed, do not pass onto base class for processing
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Process a windows message that requires the non client area be repainted.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnPaintNonClient(ref Message m)
        {
            // Let window be updated with new text
            DefWndProc(ref m);

            // Need a repaint to show change
            InvalidateNonClient();

            // Message processed, do not pass onto base class for processing
            return true;
        }

        /// <summary>
        /// Process the WM_NCMOUSEMOVE message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnWM_NCMOUSEMOVE(ref Message m)
        {
            // Extract the point in screen coordinates
            Point screenPoint = new Point((int)m.LParam.ToInt64());

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);

            //// In composition we need to adjust for the left window border
            //if (ApplyComposition)
            //    windowPoint.X -= WindowMargin.Left;

            // Perform actual mouse movement actions
            WindowChromeNonClientMouseMove(windowPoint);

            // If we are not tracking when the mouse leaves the non-client window
            if (!_trackingMouse)
            {
                PI.TRACKMOUSEEVENTS tme = new PI.TRACKMOUSEEVENTS();

                // This structure needs to know its own size in bytes
                tme.cbSize = (uint)Marshal.SizeOf(typeof(PI.TRACKMOUSEEVENTS));
                tme.dwHoverTime = 100;

                // We need to know then the mouse leaves the non client window area
                tme.dwFlags = (int)(PI.TME_LEAVE | PI.TME_NONCLIENT);

                // We want to track our own window
                tme.hWnd = Handle;

                // Call Win32 API to start tracking
                PI.TrackMouseEvent(ref tme);

                // Do not need to track again until mouse reenters the window
                _trackingMouse = true;
            }

            // Indicate that we processed the message
            m.Result = IntPtr.Zero;

            // Message processed, do not pass onto base class for processing
            return true;
        }

        /// <summary>
        /// Process the WM_NCLBUTTONDOWN message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>4
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnWM_NCLBUTTONDOWN(ref Message m)
        {
            // Extract the point in screen coordinates
            Point screenPoint = new Point((int)m.LParam.ToInt64());

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);

            //// In composition we need to adjust for the left window border
            //if (ApplyComposition)
            //    windowPoint.X -= WindowMargin.Left;

            // Perform actual mouse down processing
            return WindowChromeLeftMouseDown(windowPoint);
        }

        /// <summary>
        /// Process the WM_LBUTTONUP message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnWM_NCLBUTTONUP(ref Message m)
        {
            // Extract the point in screen coordinates
            Point screenPoint = new Point((int)m.LParam.ToInt64());

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);

            //// In composition we need to adjust for the left window border
            //if (ApplyComposition)
            //    windowPoint.X -= WindowMargin.Left;

            // Perform actual mouse up processing
            return WindowChromeLeftMouseUp(windowPoint);
        }

        /// <summary>
        /// Process the WM_NCMOUSELEAVE message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnWM_NCMOUSELEAVE(ref Message m)
        {
            // Next time the mouse enters the window we need to track it leaving
            _trackingMouse = false;

            // Perform actual mouse leave actions
            WindowChromeMouseLeave();

            // Indicate that we processed the message
            m.Result = IntPtr.Zero;

            // Need a repaint to show change
            InvalidateNonClient();

            // Message processed, do not pass onto base class for processing
            return true;
        }

        /// <summary>
        /// Process the OnWM_MOUSEMOVE message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnWM_MOUSEMOVE(ref Message m)
        {
            // Extract the point in client coordinates
            Point clientPoint = new Point((int)m.LParam);

            // Convert to screen coordinates
            Point screenPoint = PointToScreen(clientPoint);

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);

            // Perform actual mouse movement actions
            WindowChromeNonClientMouseMove(windowPoint);

            return true;
        }

        /// <summary>
        /// Process the WM_LBUTTONUP message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnWM_LBUTTONUP(ref Message m)
        {
            // Capture has now expired
            _captured = false;
            Capture = false;

            // No longer have a target element for events
            _capturedElement = null;

            // Next time the mouse enters the window we need to track it leaving
            _trackingMouse = false;

            // Extract the point in client coordinates
            Point clientPoint = new Point((int)m.LParam);

            // Convert to screen coordinates
            Point screenPoint = PointToScreen(clientPoint);

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);

            // Pass message onto the view elements
            ViewManager.MouseUp(new MouseEventArgs(MouseButtons.Left, 0, windowPoint.X, windowPoint.Y, 0), windowPoint);

            // Pass message onto the view elements
            ViewManager.MouseLeave(EventArgs.Empty);

            // Need a repaint to show change
            InvalidateNonClient();

            return true;
        }

        /// <summary>
        /// Process the WM_NCLBUTTONDBLCLK message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        [SecuritySafeCritical]
        protected virtual bool OnWM_NCLBUTTONDBLCLK(ref Message m)
        {
            // !!!!: WM_NCLBUTTONDBLCLK redirection optionally allow fast clicking of button, 
            // otherwise some repeated clicks are missed.

            return OnWM_NCLBUTTONDOWN(ref m);

            /*
            // Extract the point in screen coordinates
            Point screenPoint = new Point((int)m.LParam.ToInt64());

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);

            // Find the view element under the mouse
            ViewBase pointView = ViewManager.Root.ViewFromPoint(windowPoint);

            if (pointView != null)
            {
                // Try and find a mouse controller for the active view
                IMouseController controller = pointView.FindMouseController();

                // Eat the message
                if (controller != null)
                    return true;
            }

            return false;
            */
        }

        /// <summary>
        /// Perform chrome window painting in the non-client areas.
        /// </summary>
        /// <param name="hWnd">Window handle of window being painted.</param>
        [SecuritySafeCritical]
        protected virtual void OnNonClientPaint(IntPtr hWnd)
        {
            // Create rectangle that encloses the entire window
            Rectangle windowBounds = RealWindowRectangle;

            // We can only draw a window that has some size
            if ((windowBounds.Width > 0) && (windowBounds.Height > 0))
            {
                // Get the device context for this window
                IntPtr hDC = PI.GetWindowDC(Handle);

                // If we managed to get a device context
                if (hDC != IntPtr.Zero)
                {
                    try
                    {
                        // Find the rectangle that covers the client area of the form
                        Padding borders = WindowMargin;
                        Rectangle clipClientRect = new Rectangle(borders.Left, borders.Top,
                                                                 windowBounds.Width - borders.Horizontal,
                                                                 windowBounds.Height - borders.Vertical);

                        bool minimized = CommonHelper.IsFormMinimized(this);

                        // After excluding the client area, is there anything left to draw?
                        if (minimized || (clipClientRect.Width > 0) && (clipClientRect.Height > 0))
                        {
                            // If not minimized we need to clip the client area
                            if (!minimized)
                            {
                                // Exclude client area from being drawn into and bit blitted
                                PI.ExcludeClipRect(hDC, clipClientRect.Left, clipClientRect.Top,
                                                        clipClientRect.Right, clipClientRect.Bottom);
                            }

                            // Create one the correct size and cache for future drawing
                            IntPtr hBitmap = PI.CreateCompatibleBitmap(hDC, windowBounds.Width, windowBounds.Height);

                            // If we managed to get a compatible bitmap
                            if (hBitmap != IntPtr.Zero)
                            {
                                try
                                {
                                    // Must use the screen device context for the bitmap when drawing into the 
                                    // bitmap otherwise the Opacity and RightToLeftLayout will not work correctly.
                                    PI.SelectObject(_screenDC, hBitmap);

                                    // Drawing is easier when using a Graphics instance
                                    using (Graphics g = Graphics.FromHdc(_screenDC))
                                        WindowChromePaint(g, windowBounds);

                                    // Now blit from the bitmap to the screen
                                    PI.BitBlt(hDC, 0, 0, windowBounds.Width, windowBounds.Height, _screenDC, 0, 0, PI.SRCCOPY);
                                }
                                finally
                                {
                                    // Delete the temporary bitmap
                                    PI.DeleteObject(hBitmap);
                                }
                            }
                            else
                            {
                                // Drawing is easier when using a Graphics instance
                                using (Graphics g = Graphics.FromHdc(hDC))
                                    WindowChromePaint(g, windowBounds);
                            }
                        }
                    }
                    finally
                    {
                        // Must always release the device context
                        PI.ReleaseDC(Handle, hDC);
                    }
                }
            }

            // Bump the number of paints that have occured
            _paintCount++;
        }

        /// <summary>
        /// Called when the active state of the window changes.
        /// </summary>
        protected virtual void OnWindowActiveChanged()
        {
            UpdateShadowColor();
            if (WindowActiveChanged != null)
                WindowActiveChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets and sets the need to layout the view.
        /// </summary>
        protected bool NeedLayout {
            get { return _needLayout; }
            set { _needLayout = value; }
        }
#endregion

#region Protected Chrome
        /// <summary>
        /// Perform setup for custom chrome.
        /// </summary>
        protected virtual void WindowChromeStart()
        {
        }

        /// <summary>
        /// Perform cleanup when custom chrome ending.
        /// </summary>
        protected virtual void WindowChromeEnd()
        {
        }

        /// <summary>
        /// Perform hit testing.
        /// </summary>
        /// <param name="pt">Point in window coordinates.</param>
        /// <param name="composition">Are we performing composition.</param>
        /// <returns></returns>
        protected virtual IntPtr WindowChromeHitTest(Point pt, bool composition)
        {
            return (IntPtr)PI.HTCLIENT;
        }

        /// <summary>
        /// Perform painting of the window chrome.
        /// </summary>
        /// <param name="g">Graphics instance to use for drawing.</param>
        /// <param name="bounds">Bounds enclosing the window chrome.</param>
        protected virtual void WindowChromePaint(Graphics g, Rectangle bounds)
        {
        }

        /// <summary>
        /// Perform non-client mouse movement processing.
        /// </summary>
        /// <param name="pt">Point in window coordinates.</param>
        protected virtual void WindowChromeNonClientMouseMove(Point pt)
        {
            ViewManager.MouseMove(new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0), pt);
        }

        /// <summary>
        /// Process the left mouse down event.
        /// </summary>
        /// <param name="pt">Window coordinate of the mouse up.</param>
        /// <returns>True if event is processed; otherwise false.</returns>
        protected virtual bool WindowChromeLeftMouseDown(Point pt)
        {
            ViewManager.MouseDown(new MouseEventArgs(MouseButtons.Left, 1, pt.X, pt.Y, 0), pt);

            // If we moused down on a active view element
            if (ViewManager.ActiveView != null)
            {
                // Ask the controller if the mouse down should be ignored by wnd proc processing
                IMouseController controller = ViewManager.ActiveView.FindMouseController();
                if (controller != null)
                    return controller.IgnoreVisualFormLeftButtonDown;
            }

            return false;
        }

        /// <summary>
        /// Process the left mouse up event.
        /// </summary>
        /// <param name="pt">Window coordinate of the mouse up.</param>
        /// <returns>True if event is processed; otherwise false.</returns>
        protected virtual bool WindowChromeLeftMouseUp(Point pt)
        {
            ViewManager.MouseUp(new MouseEventArgs(MouseButtons.Left, 0, pt.X, pt.Y, 0), pt);

            // By default, we have not handled the mouse up event
            return false;
        }

        /// <summary>
        /// Perform mouse leave processing.
        /// </summary>
        protected virtual void WindowChromeMouseLeave()
        {
            // Pass message onto the view elements
            ViewManager.MouseLeave(EventArgs.Empty);
        }
#endregion

#region Implementation
        //private void UpdateComposition()
        //{
        //    if (!_insideUpdateComposition)
        //    {
        //        // Prevent reentrancy
        //        _insideUpdateComposition = true;

        //        // Are we allowed to apply composition to the window
        //        bool applyComposition = !DesignMode &&
        //                                TopLevel &&
        //                                ApplyCustomChrome &&
        //                                AllowComposition &&
        //                                DWM.IsCompositionEnabled;

        //        // Only need to process changes in value
        //        if (ApplyComposition != applyComposition)
        //        {
        //            //Debug.WriteLine("EDITOR: UpdateComposition() applying " + applyComposition);

        //            _applyComposition = applyComposition;

        //            // If we are compositing then show the composition interface
        //            if (Composition != null)
        //            {
        //                Composition.CompVisible = _applyComposition;
        //                Composition.CompOwnerForm = this;
        //                _compositionHeight = Composition.CompHeight;
        //            }
        //            else
        //                _compositionHeight = DEFAULT_COMPOSITION_HEIGHT;

        //            // With composition we extend the top into the client area
        //            DWM.ExtendFrameIntoClientArea(Handle, new Padding(0, (_applyComposition ? _compositionHeight : 0), 0, 0));

        //            // A change in composition when using custom chrome must turn custom chrome
        //            // off and on again to have it reprocess correctly to the new composition state
        //            if (ApplyCustomChrome)
        //            {
        //                ApplyCustomChrome = false;
        //                ApplyCustomChrome = true;
        //            }

        //            //Debug.WriteLine("EDITOR: UpdateComposition() appled " + _applyComposition);
        //        }
        //        else if (ApplyComposition)
        //        {
        //            int newCompHeight = DEFAULT_COMPOSITION_HEIGHT;
        //            if (Composition != null)
        //                newCompHeight = Composition.CompHeight;

        //            // Check if there is a change in the composition height
        //            if (newCompHeight != _compositionHeight)
        //            {
        //                // Apply the new height requirement
        //                _compositionHeight = newCompHeight;
        //                DWM.ExtendFrameIntoClientArea(Handle, new Padding(0, (_applyComposition ? _compositionHeight : 0), 0, 0));

        //                //Debug.WriteLine("EDITOR: UpdateComposition() frame extended " + _applyComposition);
        //            }
        //        }

        //        _insideUpdateComposition = false;
        //    }
        //}

        private void OnGlobalPaletteChanged(object sender, EventArgs e)
        {
            // We only care if we are using the global palette
            if (PaletteMode == PaletteMode.Global)
            {
                // Update ourself with the new global palette
                _localPalette = null;
                SetPalette(KryptonManager.CurrentGlobalPalette);
                Redirector.Target = _palette;

                // A new palette source means we need to layout and redraw
                OnNeedPaint(Palette, new NeedLayoutEventArgs(true));
            }
        }

        [SecuritySafeCritical]
        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
#if !DEPLOY
            // If a change has occured that could effect the color table then it needs regenerating
            switch( e.Category)
            {
                case UserPreferenceCategory.Icon:
                case UserPreferenceCategory.Menu:
                case UserPreferenceCategory.Color:
                case UserPreferenceCategory.VisualStyle:
                case UserPreferenceCategory.General:
                case UserPreferenceCategory.Window:
                case UserPreferenceCategory.Desktop:
                    //UpdateComposition();
                    PerformNeedPaint(true);
                    break;
            }
#endif		
        }

        private void SetPalette(IPalette palette)
        {
            if (palette != _palette)
            {
                // Unhook from current palette events
                if (_palette != null)
                {
                    _palette.PalettePaint -= new EventHandler<PaletteLayoutEventArgs>(OnNeedPaint);
                    _palette.ButtonSpecChanged -= new EventHandler(OnButtonSpecChanged);
                    _palette.AllowFormChromeChanged -= new EventHandler(OnAllowFormChromeChanged);
                    _palette.BasePaletteChanged -= new EventHandler(OnBaseChanged);
                    _palette.BaseRendererChanged -= new EventHandler(OnBaseChanged);
                }

                // Remember the new palette
                _palette = palette;

                // Get the renderer associated with the palette
                _renderer = _palette.GetRenderer();

                // Hook to new palette events
                if (_palette != null)
                {
                    _palette.PalettePaint += new EventHandler<PaletteLayoutEventArgs>(OnNeedPaint);
                    _palette.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
                    _palette.AllowFormChromeChanged += new EventHandler(OnAllowFormChromeChanged);
                    _palette.BasePaletteChanged += new EventHandler(OnBaseChanged);
                    _palette.BaseRendererChanged += new EventHandler(OnBaseChanged);
                }

                RecalcSizeFromClientSize();

                UpdateShadowColor();
            }
        }

        private void OnBaseChanged(object sender, EventArgs e)
        {
            // Change in base renderer or base palette require we fetch the latest renderer
            _renderer = _palette.GetRenderer();
        }
#endregion

#region Shadows

        void UpdateShadowState()
        {
            if( RealAllowLockFormUpdate && !allowShadow )
                return;

            if (DesignMode || !IsHandleCreated || IsDisposed)
                return;

            if( IsCustomHeader )//&& IsOnePixelBorder)
                CreateFormShadow();
            else
                ReleaseShadow();
        }

        void CreateFormShadow()
        {
            if (borderImageCacheManager == null)
                borderImageCacheManager = new ShadowImageCacheManager();

            if (dropShadowManager == null)
            {
                dropShadowManager = new DropShadowManager(this);
                UpdateShadowColor();
            }
        }

        void ReleaseShadow()
        {
            if (dropShadowManager != null)
            {
                dropShadowManager.Dispose();
                dropShadowManager = null;
            }

            if (borderImageCacheManager != null)
            {
                borderImageCacheManager.Dispose();
                borderImageCacheManager = null;
            }
        }

        void UpdateShadowColor()
        {
            if (dropShadowManager != null)
            {
                var color = KryptonDarkThemeUtility.DarkTheme ? Color.FromArgb( 30, 30, 30 ) : Color.FromArgb( 150, 150, 150 );
                dropShadowManager.ImageCache = borderImageCacheManager.GetCached(color);
            }
        }

        #endregion

        private void VisualForm_Load( object sender, EventArgs e )
        {
            if( RealAllowLockFormUpdate )
            {
                this.timer1 = new System.Windows.Forms.Timer();// this.components );
                this.timer1.Interval = 10;
                this.timer1.Tick += new System.EventHandler( this.timer1_Tick );
                this.timer1.Start();
            }
        }

        private void timer1_Tick( object sender, EventArgs e )
        {
            if( !IsHandleCreated )//|| WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
                return;

            if( needUnlockFormUpdate )
            {
                allowShadow = true;
                UpdateShadowState();

                Invalidate( true );
                KryptonWinFormsUtility.LockFormUpdate( null );

                needUnlockFormUpdate = false;
                timer1.Stop();
            }
        }

        [Browsable(false)]
        bool RealAllowLockFormUpdate
        {
            get
            {
                return AllowLockFormUpdate && ApplyCustomChrome;
            }
        }

        protected override void OnResizeBegin( EventArgs e )
        {
            base.OnResizeBegin( e );

            ReleaseShadow();
        }

        protected override void OnResizeEnd( EventArgs e )
        {
            base.OnResizeEnd( e );

            UpdateShadowState();
        }

    }
}

#endif