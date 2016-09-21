using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static System.Decimal;

namespace Common.Helpers
{
    public static class MathExtensions
    {
        public static decimal RatioHaving<T>(this IList<T> list, Func<T, bool> predicate)
        {
            return list.Count(predicate) / (decimal)list.Count;
        }

        public static decimal TailValueAtRisk([NotNull] this IEnumerable<decimal> list, double confidenceLevel)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list == null) throw new ArgumentNullException(nameof(list));
            IList<decimal> source1 = list.OrderBy(_ => _).ToList();
            if (!source1.Any())
                return Zero;
            int count = ValueAtRiskIndex(source1.Count, confidenceLevel);
            var source2 = (IList<decimal>) source1.Skip(count).ToList();
            if (!source2.Any())
                return source1.Last();
            return source2.Average();
        }

        private static int ValueAtRiskIndex(int count, double confidenceLevel)
        {
            return (int)Math.Floor(count * confidenceLevel);
        }
    }
}