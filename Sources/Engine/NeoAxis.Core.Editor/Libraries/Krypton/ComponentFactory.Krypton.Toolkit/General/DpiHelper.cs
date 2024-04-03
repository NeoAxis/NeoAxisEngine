#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
    /// <summary>
    /// 
    /// </summary>
    public class DpiHelper
    {
        private static DpiHelper _defaultProvider;

        private const float LogicalDpi = 96.0f;
        private float _dpi;
        private float _dpiScale;
        private InterpolationMode interpolationMode = InterpolationMode.Invalid;
        private const bool enableImagesScaling = true;

        /// <summary>
        /// 
        /// </summary>
        public static DpiHelper Default {
            get {
                if (_defaultProvider == null)
                    _defaultProvider = new DpiHelper();
                return _defaultProvider;
            }
            set {
                _defaultProvider = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsScalingRequired
        {
            get {
                return DpiScaleFactor != 1.0f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableImagesScaling
        {
            get {
                return enableImagesScaling;
            }
        }

        private InterpolationMode InterpolationMode
        {
            get {
                if (interpolationMode == InterpolationMode.Invalid)
                {
                    int dpiScalePercent = (int)Math.Round(DpiScaleFactor * 100);

                    // We will prefer NearestNeighbor algorithm for 200, 300, 400, etc zoom factors, in which each pixel become a 2x2, 3x3, 4x4, etc rectangle. 
                    // This produces sharp edges in the scaled image and doesn't cause distorsions of the original image.
                    // For any other scale factors we will prefer a high quality resizing algorith. While that introduces fuzziness in the resulting image, 
                    // it will not distort the original (which is extremely important for small zoom factors like 125%, 150%).
                    // We'll use Bicubic in those cases, except on reducing (zoom < 100, which we shouldn't have anyway), in which case Linear produces better 
                    // results because it uses less neighboring pixels.
                    if ((dpiScalePercent % 100) == 0)
                    {
                        interpolationMode = InterpolationMode.NearestNeighbor;
                    }
                    else if (dpiScalePercent < 100)
                    {
                        interpolationMode = InterpolationMode.HighQualityBilinear;
                    }
                    else
                    {
                        interpolationMode = InterpolationMode.HighQualityBicubic;
                    }
                }
                return interpolationMode;
            }
        }

        public float Dpi
        {
            get
            {
                if( _dpi == 0.0 )
                    _dpi = this.GetDpi();
                return _dpi;
            }
        }

        public float DpiScaleFactor {
            get {
                if (_dpiScale == 0.0)
                    _dpiScale = this.GetDpiScaleFactor();
                return _dpiScale;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int ScaleValue(int value)
        {
            // maybe Math.Round ?
            return (int)(value * DpiScaleFactor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public int ScaleValue(int value, float scale)
        {
            // maybe Math.Round ?
            return (int)(value * scale);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Size ScaleValue(Size size)
        {
            return new Size(ScaleValue(size.Width), ScaleValue(size.Height));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public Size ScaleValue(Size size, float scale)
        {
            return new Size(ScaleValue(size.Width, scale), ScaleValue(size.Height, scale));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="padding"></param>
        /// <returns></returns>
        public Padding ScaleValue(Padding padding)
        {
            return new Padding(ScaleValue(padding.Left), ScaleValue(padding.Top),
                ScaleValue(padding.Right), ScaleValue(padding.Bottom));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="padding"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public Padding ScaleValue(Padding padding, float scale)
        {
            return new Padding(ScaleValue(padding.Left, scale), ScaleValue(padding.Top, scale),
                ScaleValue(padding.Right, scale), ScaleValue(padding.Bottom, scale));
        }

        float GetDpi()
        {
            Size deviceDpi = GetDeviceDPI();
            return deviceDpi.Height;
        }

        float GetDpiScaleFactor()
        {
            Size deviceDpi = GetDeviceDPI();
            return deviceDpi.Height / LogicalDpi;
        }

        public void ScaleBitmapLogicalToDevice( ref Bitmap logicalBitmap )
        {
            if( logicalBitmap == null || !enableImagesScaling )
                return;

            Bitmap deviceBitmap = CreateScaledBitmap( logicalBitmap );
            if( deviceBitmap != null )
            {
                logicalBitmap.Dispose();
                logicalBitmap = deviceBitmap;
            }
        }

        //public Bitmap ScaleBitmapLogicalToDevice(Bitmap logicalBitmap)
        //{
        //    if (logicalBitmap == null || !enableImagesScaling)
        //        return null;

        //    Bitmap deviceBitmap = CreateScaledBitmap(logicalBitmap);
        //    if (deviceBitmap != null) {
        //        logicalBitmap.Dispose();
        //        return deviceBitmap;
        //    }
        //    return null;
        //}

        public Bitmap ScaleBitmapToSize(Bitmap image, int size, bool disposeSourceImage = false, InterpolationMode? mode = null)
        {
            return ScaleBitmapToSize(image, new Size(size, size), disposeSourceImage, mode);
        }

        public Bitmap ScaleBitmapToSize(Bitmap image, Size size, bool disposeSourceImage = false, InterpolationMode? mode = null)
        {
            Bitmap deviceBitmap = ScaleBitmapToSizeInternal(image, size, mode);
            if (deviceBitmap != null)
            {
                if (disposeSourceImage)
                    image.Dispose();
                return deviceBitmap;
            }
            return null;
        }

        private Bitmap CreateScaledBitmap(Bitmap logicalImage)
        {
            Size deviceImageSize = ScaleValue(logicalImage.Size);
            return ScaleBitmapToSizeInternal(logicalImage, deviceImageSize);
        }

        private Bitmap ScaleBitmapToSizeInternal(Bitmap logicalImage, Size deviceImageSize, InterpolationMode? mode = null)
        {
            Bitmap deviceImage = new Bitmap(deviceImageSize.Width, deviceImageSize.Height, logicalImage.PixelFormat);

            using (Graphics graphics = Graphics.FromImage(deviceImage))
            {
                graphics.InterpolationMode = mode ?? InterpolationMode;

                RectangleF sourceRect = new RectangleF(0, 0, logicalImage.Size.Width, logicalImage.Size.Height);
                RectangleF destRect = new RectangleF(0, 0, deviceImageSize.Width, deviceImageSize.Height);

                // Specify a source rectangle shifted by half of pixel to account for GDI+ considering the source origin the center of top-left pixel
                // Failing to do so will result in the right and bottom of the bitmap lines being interpolated with the graphics' background color,
                // and will appear black even if we cleared the background with transparent color. 
                // The apparition of these artifacts depends on the interpolation mode, on the dpi scaling factor, etc.
                // E.g. at 150% DPI, Bicubic produces them and NearestNeighbor is fine, but at 200% DPI NearestNeighbor also shows them.
                sourceRect.Offset(-0.5f, -0.5f);

                graphics.DrawImage(logicalImage, destRect, sourceRect, GraphicsUnit.Pixel);
            }

            return deviceImage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Size GetDeviceDPI()
        {
            IntPtr hDC = PI.GetDC(IntPtr.Zero);
            try
            {
                return new Size(PI.GetDeviceCaps(hDC, PI.LOGPIXELSX), PI.GetDeviceCaps(hDC, PI.LOGPIXELSY));
            }
            finally
            {
                PI.ReleaseDC(IntPtr.Zero, hDC);
            }
        }

    }
}

#endif