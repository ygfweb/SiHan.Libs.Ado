using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SiHan.Libs.Ado
{
    /// <summary>
    /// 映射缓冲池
    /// </summary>
    internal sealed class MappingCachePool
    {
        private static readonly LazyConcurrentDictionary<Type, TableMapper> Cache = new LazyConcurrentDictionary<Type, TableMapper>();
        public static TableMapper GetOrAdd<T>() where T : BaseEntity
        {
            return Cache.GetOrAdd(typeof(T), t =>
            {
                TableMapper mapper = ReflectionHelper.ToMapper<T>();
                return mapper;
            });
        }
    }
}