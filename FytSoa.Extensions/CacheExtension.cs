using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed
{
    public static class CacheExtension
    {
        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key)
        {
            var data = await cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(data))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(data);
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, Task<T> func)
        {
            var data = await cache.GetAsync<T>(key);

            if (data == null)
            {
                data = await func;

                await cache.SetAsync(key, data);
            }

            return data;
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, Task<T> func, DistributedCacheEntryOptions option = null)
        {
            var data = await cache.GetAsync<T>(key);

            if (data == null)
            {
                data = await func;

                await cache.SetAsync(key, data, option);
            }

            return data;
        }

        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, int expirationMinute) =>
        SetAsync(cache, key, value, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinute)
        });

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (value == null)
            {
                return;
            }

            var json = JsonConvert.SerializeObject(value);

            await cache.RemoveAsync(key);

            await cache.SetStringAsync(key, json);
        }

        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions option)
        {
            if (string.IsNullOrEmpty(key))
            {
                return Task.Delay(1);
            }

            if (value == null)
            {
                return Task.Delay(1);
            }

            var json = JsonConvert.SerializeObject(value);


            return cache.SetStringAsync(key, json, option);
        }
    }
}
