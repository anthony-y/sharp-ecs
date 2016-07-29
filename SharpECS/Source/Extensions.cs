using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpECS 
{
    public static class Extensions
    {
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(items);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in childSelector(next))
                    stack.Push(child);
            }
        }

        public static bool IsAvailable(this Entity entity)
        {
            return entity.State != EntityState.Cached;
        }
    }
}