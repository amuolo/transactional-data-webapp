using System.Collections.Concurrent;

namespace App.Extensions;

public static class DictionaryExtensions
{
    public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>
    (this IEnumerable<TValue> source, Func<TValue, TKey> valueSelector)
    {
        return new ConcurrentDictionary<TKey, TValue>
                   (source.ToDictionary(valueSelector));
    }
}
