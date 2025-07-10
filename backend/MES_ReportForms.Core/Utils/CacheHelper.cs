using Microsoft.Extensions.Caching.Memory;

namespace MES_ReportForms.Core.Utils
{
    /// <summary>
    /// CacheHelper class
    /// </summary>
    public static class CacheHelper
    {
        static MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// 调用函数并缓存结果，下次从缓存中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">被调用的函数</param>
        /// <param name="cacheKey">缓存key</param>
        /// <param name="minutes">缓存时长分钟数</param>
        /// <returns></returns>
        public static T CacheFunc<T>(Func<Task<T>> func, string cacheKey, int minutes)
        {
            Mutex mutex = new Mutex(false, typeof(CacheHelper).FullName + cacheKey);
            try
            {
                mutex.WaitOne();
                var cacheObject = cache.Get<T>(cacheKey);
                if (cacheObject != null)
                    return cacheObject;

                var funcResult = func().Result;
                cache.Set(cacheKey, funcResult, DateTimeOffset.Now.AddMinutes(minutes));
                return funcResult;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        public static void RemoveCache(string cacheKey)
        {
            cache.Remove(cacheKey);
        }
    }
}
