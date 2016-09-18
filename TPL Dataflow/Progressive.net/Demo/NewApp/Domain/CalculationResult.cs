using System.Collections.Generic;
using System.Linq;
using static System.Decimal;

namespace NewApp.Domain
{
    public class CalculationResult
    {
        public object TailValueAtRiskResult10Years { get; set; }
        public object ProbabilityOfHit { get; set; }
        public decimal AverageCededLoss { get; set; }

        public CalculationResult(List<RoundMetric> roundMetrics)
        {
            AverageCededLoss = roundMetrics.Average(_ => _.TotalLoss);
            ProbabilityOfHit = roundMetrics.RatioHaving(m => m.TotalLoss > Zero);
            TailValueAtRiskResult10Years = -roundMetrics.Select(m => -m.TotalLoss).TailValueAtRisk(0.9);
        }

        public override string ToString()
        {
            return $"ACL:{AverageCededLoss}, POH: {ProbabilityOfHit}, TVar10: {TailValueAtRiskResult10Years}";
        }
    }
}