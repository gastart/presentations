using System.Collections.Generic;
using System.Linq;
using Common.Domain;
using NewApp.BaseBlocks;
using NewApp.Domain;

namespace NewApp.Blocks
{
    public class RiskMeasuresBlock : CalculationSameBaseBlock<Round>
    {
        public sealed override string BlockName => GetType().Name;

        private readonly List<RoundMetric> _roundMetrics;

        public RiskMeasuresBlock()
        {
            _roundMetrics = new List<RoundMetric>();
        }

        public override Round DoWork(Round item)
        {
            var metric = new RoundMetric
            {
                RoundNumber = item.Id,
                TotalLoss = item.Losses.Sum(x => x.Amount),
                MaxLoss = item.Losses.Max()
            };

            _roundMetrics.Add(metric);

            return item;


        }

        public CalculationResult GetCalculationResult()
        {
            var result = new CalculationResult(_roundMetrics);
            return result;
        }
    }
}