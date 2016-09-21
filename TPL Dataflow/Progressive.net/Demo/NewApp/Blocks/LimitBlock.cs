using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using NewApp.BaseBlocks;
using NewApp.Domain;

namespace NewApp.Blocks
{
    public class LimitBlock : CalculationSameBaseBlock<Trial>
    {
        private readonly int _limit;
        public override string BlockName => "LimitBlock";

        public LimitBlock(int limit, ExecutionDataflowBlockOptions options)
        {
            _limit = limit;
            ProcessingBlock = new TransformBlock<Trial, Trial>((Func<Trial, Trial>)ProcessItem, options);
        }

        public override Trial DoWork(Trial item)
        {
            if(item.Losses!= null && item.Losses.Any())
            {
                var sumOfLosses = item.Losses.Sum(_ => _.Amount);
                var originalTotalSum = sumOfLosses;

                if (originalTotalSum > _limit)
                {
                    sumOfLosses = _limit;
                }

                if (sumOfLosses < originalTotalSum)
                {
                    var scaleFactor = sumOfLosses / originalTotalSum;
                    foreach (var loss in item.Losses)
                    {
                        loss.Scale(scaleFactor);
                    }
                }
            }

            return item;
        }
    }
}