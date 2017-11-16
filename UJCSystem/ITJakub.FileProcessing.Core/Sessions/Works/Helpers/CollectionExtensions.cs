using System;
using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Sessions.Works.Helpers
{
    public static class CollectionExtensions
    {
        public static Dictionary<TKey, TVal> ToDictionarySafe<TKey, TVal>(this IEnumerable<TVal> collection, Func<TVal, TKey> keySelector)
        {
            var result = new Dictionary<TKey, TVal>();
            var duplicitItemKeys = new List<TKey>();

            foreach (var item in collection)
            {
                var key = keySelector.Invoke(item);

                if (result.ContainsKey(key))
                {
                    duplicitItemKeys.Add(key);
                }
                else
                {
                    result.Add(key, item);
                }
            }

            foreach (var duplicitItemKey in duplicitItemKeys)
            {
                result.Remove(duplicitItemKey);
            }

            return result;
        }

        public static Dictionary<TKey, List<TVal>> ToDictionaryMultipleValues<TKey, TVal>(this IEnumerable<TVal> collection, Func<TVal, TKey> keySelector)
        {
            var result = new Dictionary<TKey, List<TVal>>();
            
            foreach (var item in collection)
            {
                var key = keySelector.Invoke(item);

                if (result.ContainsKey(key))
                {
                    result[key].Add(item);
                }
                else
                {
                    result.Add(key, new List<TVal> {item});
                }
            }

            return result;
        }
    }
}
