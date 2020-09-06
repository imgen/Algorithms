using System;
using System.Collections.Generic;

namespace Algorithms
{
    static class MiscUtils
    {
        public static void PrintOut<T>(this IList<T> a)
        {
            Console.WriteLine($"The contents of the array with length {a.Count} is as below:");
            foreach (var item in a)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
        }

        public static TSource FirstOrDefault<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate, 
            TSource defaultValue)
        {
            foreach(var item in source)
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            return defaultValue;
        }
    }
}
