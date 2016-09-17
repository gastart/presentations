using System;
using System.Collections.Generic;
using System.Linq;

namespace NewApp.Domain
{
    public static class MathExtensions
    {
        public static Decimal RatioHaving<T>(this IList<T> list, Func<T, bool> predicate)
        {
            return (Decimal)list.Count<T>(predicate) / (Decimal)list.Count;
        }

        public static Decimal TailValueAtRisk(this IEnumerable<Decimal> list, double confidenceLevel)
        {
            IList<Decimal> source1 = (IList<Decimal>)list.OrderBy<Decimal, Decimal>((Func<Decimal, Decimal>)(_ => _)).ToList<Decimal>();
            if (!source1.Any<Decimal>())
                return Decimal.Zero;
            int count = ValueAtRiskIndex(source1.Count, confidenceLevel);
            IList<Decimal> source2 = (IList<Decimal>)source1.Skip<Decimal>(count).ToList<Decimal>();
            if (!source2.Any<Decimal>())
                return source1.Last<Decimal>();
            return source2.Average();
        }

        private static int ValueAtRiskIndex(int count, double confidenceLevel)
        {
            return (int)Math.Floor((double)count * confidenceLevel);
        }
    }
}