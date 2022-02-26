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
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Diagnostics;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace Internal.ComponentFactory.Krypton.Ribbon
{
    /// <summary>
    /// Represents a single ribbon quick access toolbar entry.
    /// </summary>
    [ToolboxItem(false)]
    [ToolboxBitmap(typeof(KryptonRibbonQATButton), "ToolboxBitmaps.KryptonRibbonQATButton.bmp")]
    [DefaultEvent("Click")]
    [DefaultProperty("Image")]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    public class KryptonRibbonQATButton : Component,
                                          IQuickAccessToolbarButton
    {
        #region Static Fields
        private static readonly Image _defaultImage = Properties.Resources.QATButtonDefault;
        #endregion

        #region Instance Fields
        private object _tag;
        private Image _image;
        private Image _toolTipImage;
        private Color _toolTipImageTransparentColor;
        private LabelStyle _toolTipStyle;
        private bool _visible;
        private bool _enabled;
        private bool _checked;
        private bool _isDropDownButton;
        private string _text;
        private string _toolTipTitle;
        private string _toolTipBody;
        private Keys _shortcutKeys;
        private KryptonCommand _command;
        private KryptonRibbon _ribbon;

        private KryptonContextMenu _kryptonContextMenu;
        private EventHandler _kcmFinishDelegate;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the quick access toolbar button has been clicked.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// Occurs when the drop down button type is pressed.
        /// </summary>
        [Description("Occurs when the drop down button type is pressed.")]
        public event EventHandler<ContextMenuArgs> DropDown;

        /// <summary>
        /// Occurs after the value of a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Identity
        /// <summary>
        /// Initialise a new instance of the KryptonRibbonQATButton class.
        /// </summary>
        public KryptonRibbonQATButton()
        {
            // Default fields
            _image = _defaultImage;
            _visible = true;
            _enabled = true;
            _text = "QAT Button";
            _shortcutKeys = Keys.None;
            _toolTipImageTransparentColor = Color.Empty;
            _toolTipTitle = string.Empty;
            _toolTipBody = string.Empty;
            _toolTipStyle = LabelStyle.ToolTip;

            _kryptonContextMenu = null;
        }
        #endregion

        #region Public
        /// <summary>
        /// Gets access to the owning ribbon control.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public KryptonRibbon Ribbon
        {
            get { return _ribbon; }
        }

        /// <summary>
        /// Gets and sets the application button image.
        /// </summary>
        [Bindable(true)]
        [Localizable(true)]
        [Category("Values")]
        [Description("Application button image.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        public Image Image
        {
            get { return _image; }

            set
            {
                if (_image != value)
                {

                    _image = value;
                    OnPropertyChanged("Image");

                    // Only need to update display if we are visible
                    if (Visible && (_ribbon != null))
                        _ribbon.PerformNeedPaint(false);
                }
            }
        }

        private bool ShouldSerializeImage()
        {
            return Image != _defaultImage;
        }

        /// <summary>
        /// Gets and sets the visible state of the ribbon quick access toolbar entry.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [Description("Determines whether the ribbon quick access toolbar entry is visible or hidden.")]
        [DefaultValue(true)]
        public bool Visible
        {
            get { return _visible; }

            set
            {
                if (value != _visible)
                {
                    _visible = value;
                    OnPropertyChanged("Visible");

                    // Must try and layout to show change
                    if( _ribbon != null)
                    {
                        _ribbon.PerformNeedPaint(true);
                        _ribbon.UpdateQAT();
                    }
                }
            }
        }

        /// <summary>
        /// Make the ribbon tab visible.
        /// </summary>
        public void Show()
        {
            Visible = true;
        }

        /// <summary>
        /// Make the ribbon tab hidden.
        /// </summary>
        public void Hide()
        {
            Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [Description("Indicates whether the control is in the checked state.")]
        [DefaultValue(false)]
        public bool Checked
        {
            get { return _checked; }
            set {
                if (value != _checked)
                {
                    _checked = value;

                    OnPropertyChanged("Checked");

                    // Must try and paint to show change
                    if (Visible && (_ribbon != null))
                        _ribbon.PerformNeedPaint(false);
                }
            }
        }

                /// <summary>
        /// 
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [Description("Indicates whether the control is DropDown type.")]
        [DefaultValue(false)]
        public bool IsDropDownButton
        {
            get { return _isDropDownButton; }
            set {
                if (value != _isDropDownButton)
                {
                    _isDropDownButton = value;

                    OnPropertyChanged("IsDropDownButton");

                    // Must try and paint to show change
                    if (Visible && (_ribbon != null))
                        _ribbon.PerformNeedPaint(false);
                }
            }
        }

        /// <summary>
        /// Gets and sets the enabled state of the ribbon quick access toolbar entry.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [Description("Determines whether the ribbon quick access toolbar entry is enabled.")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return _enabled; }

            set {
                if (value != _enabled)
                {
                    _enabled = value;

                    OnPropertyChanged("Enabled");

                    // Must try and paint to show change
                    if (Visible && (_ribbon != null))
                        _ribbon.PerformNeedPaint(false);
                }
            }
        }


        /// <summary>
        /// Gets and sets the display text of the quick access toolbar button.
        /// </summary>
        [Bindable(true)]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("QAT button text.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue("QAT Button")]
        public string Text
        {
            get { return _text; }

            set
            {
                // We never allow an empty text value
                if (string.IsNullOrEmpty(value))
                    value = "QAT Button";

                if (value != _text)
                {
                    _text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// Gets and sets the shortcut key combination.
        /// </summary>
        [Localizable(true)]
        [Category("Behavior")]
        [Description("Shortcut key combination to fire click event of the quick access toolbar button.")]
        public Keys ShortcutKeys
        {
            get { return _shortcutKeys; }
            set { _shortcutKeys = value; }
        }

        private bool ShouldSerializeShortcutKeys()
        {
            return (ShortcutKeys != Keys.None);
        }

        /// <summary>
        /// Resets the ShortcutKeys property to its default value.
        /// </summary>
        public void ResetShortcutKeys()
        {
            ShortcutKeys = Keys.None;
        }

        /// <summary>
        /// Gets and sets the tooltip label style for the quick access button.
        /// </summary>
        [Category("Appearance")]
        [Description("Tooltip style for the quick access toolbar button.")]
        [DefaultValue(typeof(LabelStyle), "ToolTip")]
        public LabelStyle ToolTipStyle
        {
            get { return _toolTipStyle; }
            set { _toolTipStyle = value; }
        }

        /// <summary>
        /// Gets and sets the image for the item ToolTip.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [Description("Display image associated ToolTip.")]
        [DefaultValue(null)]
        [Localizable(true)]
        public Image ToolTipImage
        {
            get { return _toolTipImage; }
            set { _toolTipImage = value; }
        }

        /// <summary>
        /// Gets and sets the color to draw as transparent in the ToolTipImage.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [Description("Color to draw as transparent in the ToolTipImage.")]
        [KryptonDefaultColorAttribute()]
        [Localizable(true)]
        public Color ToolTipImageTransparentColor
        {
            get { return _toolTipImageTransparentColor; }
            set { _toolTipImageTransparentColor = value; }
        }

        /// <summary>
        /// Gets and sets the title text for the item ToolTip.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [Description("Title text for use in associated ToolTip.")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue("")]
        [Localizable(true)]
        public string ToolTipTitle
        {
            get { return _toolTipTitle; }
            set { _toolTipTitle = value; }
        }

        /// <summary>
        /// Gets and sets the body text for the item ToolTip.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [Description("Body text for use in associated ToolTip.")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue("")]
        [Localizable(true)]
        public string ToolTipBody
        {
            get { return _toolTipBody; }
            set { _toolTipBody = value; }
        }

        /// <summary>
        /// Gets and sets the KryptonContextMenu for showing when the button is pressed.
        /// </summary>
        [Category("Behavior")]
        [Description("KryptonContextMenu to be shown when the button is pressed.")]
        [DefaultValue(null)]
        public KryptonContextMenu KryptonContextMenu
        {
            get { return _kryptonContextMenu; }

            set {
                if (value != _kryptonContextMenu)
                {
                    _kryptonContextMenu = value;
                    OnPropertyChanged("KryptonContextMenu");
                }
            }
        }

        /// <summary>
        /// Gets and sets the associated KryptonCommand.
        /// </summary>
        [Category("Behavior")]
        [Description("Command associated with the quick access toolbar button.")]
        [DefaultValue(null)]
        public KryptonCommand KryptonCommand
        {
            get { return _command; }

            set
            {
                if (_command != value)
                {
                    if (_command != null)
                        _command.PropertyChanged -= new PropertyChangedEventHandler(OnCommandPropertyChanged);

                    _command = value;
                    OnPropertyChanged("KryptonCommand");

                    if (_command != null)
                        _command.PropertyChanged += new PropertyChangedEventHandler(OnCommandPropertyChanged);

                    // Only need to update display if we are visible
                    if (Visible && (_ribbon != null))
                        _ribbon.PerformNeedPaint(false);
                }
            }
        }

        /// <summary>
        /// Gets and sets user-defined data associated with the object.
        /// </summary>
        [Category("Data")]
        [Description("User-defined data associated with the object.")]
        [TypeConverter(typeof(StringConverter))]
        [Bindable(true)]
        public object Tag
        {
            get { return _tag; }

            set
            {
                if (value != _tag)
                {
                    _tag = value;
                    OnPropertyChanged("Tag");
                }
            }
        }

        private bool ShouldSerializeTag()
        {
            return (Tag != null);
        }

        private void ResetTag()
        {
            Tag = null;
        }
        #endregion

        #region IQuickAccessToolbarButton
        /// <summary>
        /// Provides a back reference to the owning ribbon control instance.
        /// </summary>
        /// <param name="ribbon">Reference to owning instance.</param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void SetRibbon(KryptonRibbon ribbon)
        {
            _ribbon = ribbon;
        }

        /// <summary>
        /// Gets the entry image.
        /// </summary>
        /// <returns>Image value.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Image GetImage()
        {
            if (KryptonCommand != null)
                return KryptonCommand.ImageSmall;
            else
                return Image;
        }

        /// <summary>
        /// Gets the entry text.
        /// </summary>
        /// <returns>Text value.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string GetText()
        {
            if (KryptonCommand != null)
                return KryptonCommand.TextLine1;
            else
                return Text;
        }

        /// <summary>
        /// Gets the entry enabled state.
        /// </summary>
        /// <returns>Enabled value.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool GetEnabled()
        {
            if (KryptonCommand != null)
                return KryptonCommand.Enabled;
            else
                return Enabled;
        }

        /// <summary>
        /// Gets the entry shortcut keys state.
        /// </summary>
        /// <returns>ShortcutKeys value.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Keys GetShortcutKeys()
        {
            return ShortcutKeys;
        }

        /// <summary>
        /// Gets the entry visible state.
        /// </summary>
        /// <returns>Visible value.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool GetVisible()
        {
            return Visible;
        }

        /// <summary>
        /// Sets a new value for the visible state.
        /// </summary>
        /// <param name="visible"></param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void SetVisible(bool visible)
        {
            Visible = visible;
        }

        /// <summary>
        /// Gets the tooltip label style.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public LabelStyle GetToolTipStyle()
        {
            return ToolTipStyle;
        }

        /// <summary>
        /// Gets and sets the image for the item ToolTip.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Image GetToolTipImage()
        {
            return ToolTipImage;
        }

        /// <summary>
        /// Gets and sets the color to draw as transparent in the ToolTipImage.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Color GetToolTipImageTransparentColor()
        {
            return ToolTipImageTransparentColor;
        }

        /// <summary>
        /// Gets and sets the title text for the item ToolTip.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string GetToolTipTitle()
        {
            return ToolTipTitle;
        }

        /// <summary>
        /// Gets and sets the body text for the item ToolTip.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string GetToolTipBody()
        {
            return ToolTipBody;
        }

        /// <summary>
        /// Generates a Click event for a button.
        /// </summary>
        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        /// <summary>
        /// Generates a DropDown event for a button.
        /// </summary>
        public void PerformDropDown()
        {
            PerformDropDown(null);
        }

        /// <summary>
        /// Generates a DropDown event for a button.
        /// </summary>
        /// <param name="finishDelegate">Delegate fired during event processing.</param>
        public void PerformDropDown(EventHandler finishDelegate)
        {
            OnDropDown(finishDelegate);
        }
        #endregion

        #region Protected
        /// <summary>
        /// Handles a change in the property of an attached command.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">A PropertyChangedEventArgs that contains the event data.</param>
        protected virtual void OnCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            bool refresh = false;

            switch (e.PropertyName)
            {
                case "Text":
                    refresh = true;
                    OnPropertyChanged("Text");
                    break;
                case "ImageSmall":
                    refresh = true;
                    OnPropertyChanged("Image");
                    break;
                case "Enabled":
                    refresh = true;
                    OnPropertyChanged("Enabled");
                    break;
            }

            if (refresh)
            {
                // Only need to update display if we are visible
                if (Visible && (_ribbon != null))
                    _ribbon.PerformNeedPaint(false);
            }
        }
        
        /// <summary>
        /// Raises the Click event.
        /// </summary>
        /// <param name="e">An EventArgs containing the event data.</param>
        protected virtual void OnClick(EventArgs e)
        {
            // Perform processing that is common to any action that would dismiss
            // any popup controls such as the showing minimized group popup
            if (Ribbon != null)
                Ribbon.ActionOccured();

            if (Click != null)
                Click(this, e);

            // Clicking the button should execute the associated command
            if (KryptonCommand != null)
                KryptonCommand.PerformExecute();
        }

        /// <summary>
        /// Raises the DropDown event.
        /// </summary>
        /// <param name="finishDelegate">Delegate fired during event processing.</param>
        protected virtual void OnDropDown(EventHandler finishDelegate)
        {
            bool fireDelegate = true;

            if (!Ribbon.InDesignMode && Enabled && IsDropDownButton)
            {
                if (KryptonContextMenu != null)
                {
                    ContextMenuArgs contextArgs = new ContextMenuArgs(KryptonContextMenu);

                    // Generate an event giving a chance for the krypton context menu strip to 
                    // be shown to be provided/modified or the action even to be cancelled
                    if (DropDown != null)
                        DropDown(this, contextArgs);

                    // If user did not cancel and there is still a krypton context menu strip to show
                    if (!contextArgs.Cancel && (contextArgs.KryptonContextMenu != null))
                    {
                        Rectangle screenRect = Rectangle.Empty;

                        // Convert the view for the button into screen coordinates
                        ViewBase qatView = _ribbon?.GetViewForQATButton(this);
                        if (qatView != null)
                        {
                            screenRect = Ribbon.RectangleToScreen(qatView.ClientRectangle);

                            if (_ribbon.QATLocation == QATLocation.Above)
                            {
                                var ownerForm = _ribbon.FindForm() as VisualForm;
                                // If integrated into the caption area
                                if ((ownerForm != null) /*&& !ownerForm.ApplyComposition*/)
                                {
                                    // Adjust for the height/width of borders
                                    Padding borders = ownerForm.WindowMargin;
                                    screenRect.X -= borders.Left;
                                    screenRect.Y -= borders.Top;
                                }
                            }
                        }

                        if (CommonHelper.ValidKryptonContextMenu(contextArgs.KryptonContextMenu))
                        {
                            // Cache the finish delegate to call when the menu is closed
                            _kcmFinishDelegate = finishDelegate;

                            // Show at location we were provided, but need to convert to screen coordinates
                            contextArgs.KryptonContextMenu.Closed += new ToolStripDropDownClosedEventHandler(OnKryptonContextMenuClosed);
                            if (contextArgs.KryptonContextMenu.Show(this, new Point(screenRect.X, screenRect.Bottom + 1)))
                                fireDelegate = false;
                        }
                    }
                }
                //TODO: ContextMenuStrip not supported.
                //else if (ContextMenuStrip != null)
                //{
                //    ContextMenuArgs contextArgs = new ContextMenuArgs(ContextMenuStrip);

                //    // Generate an event giving a chance for the context menu strip to be
                //    // shown to be provided/modified or the action even to be cancelled
                //    if (DropDown != null)
                //        DropDown(this, contextArgs);

                //    // If user did not cancel and there is still a context menu strip to show
                //    if (!contextArgs.Cancel && (contextArgs.ContextMenuStrip != null))
                //    {
                //        Rectangle screenRect = Rectangle.Empty;

                //        // Convert the view for the button into screen coordinates
                //        ViewBase qatView = _ribbon?.GetViewForQATButton(this);
                //        if (qatView != null)
                //            screenRect = Ribbon.ViewRectangleToScreen(qatView);

                //        if (CommonHelper.ValidContextMenuStrip(contextArgs.ContextMenuStrip))
                //        {
                //            // Do not fire the delegate in this routine, wait for the popup manager to show it
                //            fireDelegate = false;

                //            //...show the context menu below and at th left of the button
                //            VisualPopupManager.Singleton.ShowContextMenuStrip(contextArgs.ContextMenuStrip,
                //                                                                new Point(screenRect.X, screenRect.Bottom + 1),
                //                                                                finishDelegate);
                //        }
                //    }
                //}
            }

            // Do we need to fire a delegate stating the click processing has finished?
            if (fireDelegate && (finishDelegate != null))
                finishDelegate(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of property that has changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Implementation
        private void OnKryptonContextMenuClosed(object sender, EventArgs e)
        {
            KryptonContextMenu kcm = (KryptonContextMenu)sender;
            kcm.Closed -= new ToolStripDropDownClosedEventHandler(OnKryptonContextMenuClosed);

            // Fire any associated finish delegate
            if (_kcmFinishDelegate != null)
            {
                _kcmFinishDelegate(this, e);
                _kcmFinishDelegate = null;
            }
        }
        #endregion
    }

}
