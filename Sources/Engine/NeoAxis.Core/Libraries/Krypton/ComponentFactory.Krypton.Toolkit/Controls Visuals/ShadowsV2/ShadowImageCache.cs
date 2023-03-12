#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
    public class ShadowImageCache : IDisposable
    {
        bool _disposed;

        public Color BaseColor { get; private set; }
        public Bitmap CornerNW { get; private set; }
        public Bitmap CornerNE { get; private set; }
        public Bitmap CornerSE { get; private set; }
        public Bitmap CornerSW { get; private set; }
        public Bitmap BorderN { get; private set; }
        public Bitmap BorderE { get; private set; }
        public Bitmap BorderS { get; private set; }
        public Bitmap BorderW { get; private set; }

        public ShadowImageCache(Color baseColor)
        {
            BaseColor = baseColor;

            CornerNW = Build(ImageMaps.Corner, 0);
            CornerNE = Build(ImageMaps.Corner, 1);
            CornerSE = Build(ImageMaps.Corner, 2);
            CornerSW = Build(ImageMaps.Corner, 3);
            BorderN = Build(ImageMaps.Border, 0);
            BorderE = Build(ImageMaps.Border, 1);
            BorderS = Build(ImageMaps.Border, 2);
            BorderW = Build(ImageMaps.Border, 3);
        }

        Bitmap Build(int[,] map, int rotation)
        {
            int width = map.GetLength(1);
            int height = map.GetLength(0);

            if (rotation % 2 == 1)
            {
                int tmp = width;
                width = height;
                height = tmp;
            }

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int alpha = GetAlpha(map, x, y, width, height, rotation);

                    bitmap.SetPixel(x, y, Color.FromArgb(alpha, BaseColor));
                }
            }

            return bitmap;
        }

        int GetAlpha(int[,] map, int x, int y, int width, int height, int rotation)
        {
            int nx;
            int ny;

            switch (rotation % 4)
            {
                case 0:
                    nx = x;
                    ny = y;
                    break;

                case 1:
                    nx = y;
                    ny = (width - 1) - x;
                    break;

                case 2:
                    nx = (width - 1) - x;
                    ny = (height - 1) - y;
                    break;

                case 3:
                    nx = (height - 1) - y;
                    ny = x;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("rotation");
            }

            return map[ny, nx];
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (CornerNW != null)
                {
                    CornerNW.Dispose();
                    CornerNW = null;
                }

                if (CornerNE != null)
                {
                    CornerNE.Dispose();
                    CornerNE = null;
                }

                if (CornerSE != null)
                {
                    CornerSE.Dispose();
                    CornerSE = null;
                }

                if (CornerSW != null)
                {
                    CornerSW.Dispose();
                    CornerSW = null;
                }

                if (BorderN != null)
                {
                    BorderN.Dispose();
                    BorderN = null;
                }

                if (BorderE != null)
                {
                    BorderE.Dispose();
                    BorderE = null;
                }

                if (BorderS != null)
                {
                    BorderS.Dispose();
                    BorderS = null;
                }

                if (BorderW != null)
                {
                    BorderW.Dispose();
                    BorderW = null;
                }

                _disposed = true;
            }
        }
    }
}

#endif