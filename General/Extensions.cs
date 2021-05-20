using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.General
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> list, int batchSize)
        {
            int i = 0;
            return list.GroupBy(x => (i++ / batchSize)).ToList();
        }
    }
}
