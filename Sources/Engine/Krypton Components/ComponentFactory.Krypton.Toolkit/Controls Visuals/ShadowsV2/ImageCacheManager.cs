using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ComponentFactory.Krypton.Toolkit
{
    internal class ImageCacheManager : IDisposable
    {
        private readonly Queue<Color> _queue = new Queue<Color>();
        private readonly Dictionary<Color, ImageCache> _cache = new Dictionary<Color, ImageCache>();
        private bool _disposed;

        public int MaxCached { get; private set; }

        public ImageCache GetCached(Color color)
        {
            if (!_cache.TryGetValue(color, out ImageCache result))
            {
                TrimQueue(MaxCached);

                result = new ImageCache(color);

                _queue.Enqueue(color);
                _cache[color] = result;
            }

            return result;
        }

        private void TrimQueue(int maxCached)
        {
            while (_queue.Count > maxCached)
            {
                var color = _queue.Dequeue();

                _cache[color].Dispose();
                _cache.Remove(color);
            }
        }

        public ImageCacheManager()
            : this(10)
        {
        }

        public ImageCacheManager(int maxCached)
        {
            MaxCached = maxCached;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                TrimQueue(0);
                _disposed = true;
            }
        }
    }
}
