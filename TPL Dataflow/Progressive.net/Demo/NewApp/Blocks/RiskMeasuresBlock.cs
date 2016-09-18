using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using NewApp.BaseBlocks;
using NewApp.Domain;

namespace NewApp.Blocks
{
    public class RiskMeasuresBlock : CalculationSameBaseBlock<Round>
    {
        public sealed override string BlockName => GetType().Name;

        // Shared state! Locking or mdop 1
        private readonly List<RoundMetric> _roundMetrics;

        public RiskMeasuresBlock(ExecutionDataflowBlockOptions options)
        {
            _roundMetrics = new List<RoundMetric>();
            ProcessingBlock = new TransformBlock<Round, Round>((Func<Round, Round>)ProcessItem, options);
        }

        public override Round DoWork(Round item)
        {
            var metric = new RoundMetric { RoundNumber = item.Id };

            if (item.Losses != null && item.Losses.Any())
            {
                metric.TotalLoss = item.Losses.Sum(x => x.Amount);
                metric.MaxLoss = item.Losses.Max(x => x.Amount);
            }

            _roundMetrics.Add(metric);
           
            return item;
        }

        public CalculationResult GetCalculationResult()
        {
            var result = new CalculationResult(_roundMetrics);
            return result;
        }

        public int GetProcessedItemsCount()
        {
            return _roundMetrics.Count;
        }
    }
}