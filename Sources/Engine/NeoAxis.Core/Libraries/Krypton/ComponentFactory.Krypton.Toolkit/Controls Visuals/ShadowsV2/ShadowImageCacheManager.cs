using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
    public class ShadowImageCacheManager : IDisposable
    {
        readonly Queue<Color> _queue = new Queue<Color>();
        readonly Dictionary<Color, ShadowImageCache> _cache = new Dictionary<Color, ShadowImageCache>();
        bool _disposed;

        public int MaxCached { get; private set; }

        public ShadowImageCache GetCached(Color color)
        {
            if (!_cache.TryGetValue(color, out ShadowImageCache result))
            {
                TrimQueue(MaxCached);

                result = new ShadowImageCache(color);

                _queue.Enqueue(color);
                _cache[color] = result;
            }

            return result;
        }

        void TrimQueue(int maxCached)
        {
            while (_queue.Count > maxCached)
            {
                var color = _queue.Dequeue();

                _cache[color].Dispose();
                _cache.Remove(color);
            }
        }

        public ShadowImageCacheManager()
            : this(10)
        {
        }

        public ShadowImageCacheManager(int maxCached)
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
