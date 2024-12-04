using System.Collections;

namespace SmoothGL.Content.Internal;

public static class EnumerableExtension
{
    public static IEnumerable<(TFirst, TSecond)> ZipWithRemainder<TFirst, TSecond>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        out IEnumerable<TFirst> firstRemainder,
        out IEnumerable<TSecond> secondRemainder
    )
    {
        var firstList = first.ToList();
        var secondList = second.ToList();
        
        var minLength = Math.Min(firstList.Count, secondList.Count);
        
        firstRemainder = firstList.Skip(minLength);
        secondRemainder = secondList.Skip(minLength);
        
        return firstList.Zip(secondList);
    }
    
    public static Type? GetItemType(this IEnumerable enumerable) =>
        enumerable.GetType().GetInterfaces()
            .Where(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(interfaceType => interfaceType.GetGenericArguments()[0])
            .FirstOrDefault();
}
