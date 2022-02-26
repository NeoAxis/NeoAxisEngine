using System;
using System.Collections.Generic;
using System.Text;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
    internal static class ImageMaps
    {
        public static readonly int[,] Corner =
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 1, 1, 1, 3, 3, 3, 3 },
            { 0, 0, 0, 1, 1, 3, 3, 3, 3, 4, 5 },
            { 0, 0, 1, 1, 3, 3, 3, 4, 5, 6, 7 },
            { 0, 0, 1, 3, 3, 3, 4, 7, 8, 11, 13 },
            { 0, 0, 1, 3, 3, 4, 7, 10, 13, 18, 23 },
            { 0, 1, 3, 3, 4, 7, 10, 15, 20, 28, 33 },
            { 0, 1, 3, 3, 5, 8, 13, 20, 28, 38, 47 },
            { 0, 1, 3, 4, 6, 11, 18, 28, 38, 0, 0 },
            { 0, 1, 3, 5, 7, 13, 23, 33, 47, 0, 0 }
        };

        public static readonly int[,] Border =
        {
            { 0 },
            { 3 },
            { 4 },
            { 6 },
            { 10 },
            { 17 },
            { 28 },
            { 42 },
            { 61 }
        };
    }
}
