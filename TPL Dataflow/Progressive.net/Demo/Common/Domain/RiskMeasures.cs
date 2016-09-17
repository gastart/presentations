using System;
using System.Collections.Generic;
using System.Linq;
using static System.Decimal;

namespace Common.Domain
{
    public class RiskMeasures
    {
        public static decimal Average(List<AggreatedRound> limitedRounds)
        {
            return limitedRounds.Average(x => x.AggreatedLoss);
        }

        public static decimal MaxLoss(List<AggreatedRound> limitedRounds)
        {
            var max = limitedRounds.Max(x => x.AggreatedLoss);
            return max;
        }

        public static decimal TailValueAtRisk(List<AggreatedRound> list, double confidenceLevel)
        {
            List<decimal> source1 = list.OrderBy(_ => _.AggreatedLoss).Select(x => x.AggreatedLoss).ToList();
            if (!source1.Any())
                return Zero;
            int count = ValueAtRiskIndex(source1.Count, confidenceLevel);
            var source2 = source1.Skip(count).ToList() as IList<Decimal>;
            if (!source2.Any())
                return source1.Last();
            return source2.Average();
        }

        private static int ValueAtRiskIndex(int count, double confidenceLevel)
        {
            return (int)Math.Floor((double)count * confidenceLevel);
        }
    }
}