using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Common.Domain;
using Common.Helpers;
using static System.Decimal;

namespace Presentation
{
    public class StatisticsBlock
    {
        public TransformBlock<Trial, Trial> Block;
        private readonly List<Metric> _values;

        public StatisticsBlock()
        {
            _values = new List<Metric>();
            Func<Trial, Trial > action = x =>
            {
                _values.Add(new Metric
                {
                    Total = x.Losses.Any() ? x.Losses.Sum(l => l.Amount) : 0,
                    Max = x.Losses.Any() ? x.Losses.Max(l => l.Amount) : 0
                });

                return x;
            };

            Block = new TransformBlock<Trial, Trial>(action, new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 1});
        }

        public decimal Average()
        {
            return _values.Average(x => x.Total);
        }

        public decimal Tvar10()
        {
            return _values.Select(m => -m.Total).TailValueAtRisk(0.9);
        }

        public decimal Poh()
        {
            return _values.RatioHaving(m => m.Total > Zero);
        }
    }

    class Metric
    {
        public decimal Total { get; set; }
        public decimal Max { get; set; }
    }

   
}