namespace Core.CodeUtilities.Collections;

public static class CollectionExtenssions
{
    public static void ForEach<T>(this T[] array, Action<T> action)
    {
        foreach (T item in array)
        {
            action(item);
        }
    }
}
