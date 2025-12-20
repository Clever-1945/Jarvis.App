using Jarvis.App.Interfaces;

namespace Jarvis.App.Extensions;

public static class ICloneExtensions
{
    public static T[] CloneArray<T>(this IClone<T>[] array)
    {
        if (array == null || array.Length < 1)
            return new T[] { };

        var next = new T[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            next[i] = array[i].Clone();
        }

        return next;
    }
}