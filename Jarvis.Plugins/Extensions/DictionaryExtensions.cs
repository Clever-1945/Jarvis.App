namespace Jarvis.Plugins.Extensions;

public static class DictionaryExtensions
{
    public static Dictionary<TK, TV> ToDictionarySelf<TK, TV, TC>(this IEnumerable<TC> enumerable, Func<TC, TK> key, Func<TC, TV> value)
    {
        var dictionary = new Dictionary<TK, TV>();
        if (enumerable != null)
        {
            foreach (var instance in enumerable)
            {
                if (instance != null)
                {
                    var k = key(instance);
                    if (!dictionary.ContainsKey(k))
                    {
                        dictionary[k] = value(instance);
                    }
                }
            }
        }

        return dictionary;
    }
}