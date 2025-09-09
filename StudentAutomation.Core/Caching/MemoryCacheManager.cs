using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using StudentAutomation.Core.Utilities.IoC;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;

namespace StudentAutomation.Core.Caching
{
    public class MemoryCacheManager : ICacheManager
    {
        private IMemoryCache _memoryCache;
        private static readonly ConcurrentDictionary<string, bool> _keys = new();

        public MemoryCacheManager()
        {
            _memoryCache = ServiceTool.ServiceProvider.GetService<IMemoryCache>();
        }

        public void Add(string key, object value, int duration)
        {
            _memoryCache.Set(key, value, TimeSpan.FromMinutes(duration));
            _keys.TryAdd(key, true); // 👈 Key'i takip et
        }

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public object Get(string key)
        {
            return _memoryCache.Get(key);
        }

        public bool IsAdd(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
            _keys.TryRemove(key, out _); // 👈 Key'i sil
        }

        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var keysToRemove = _keys.Keys.Where(k => regex.IsMatch(k)).ToList();
            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
                _keys.TryRemove(key, out _);
                Console.WriteLine($"🗑 Cache kaldırıldı: {key}");
            }
        }
    }

}
