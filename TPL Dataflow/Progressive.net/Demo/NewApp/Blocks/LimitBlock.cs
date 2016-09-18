using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Common.Domain;
using NewApp.BaseBlocks;

namespace NewApp.Blocks
{
    public class LimitBlock : CalculationSameBaseBlock<Round>
    {
        private readonly int _limit;
        public override string BlockName => "LimitBlock";

        public LimitBlock(int limit, ExecutionDataflowBlockOptions options)
        {
            _limit = limit;
            ProcessingBlock = new TransformBlock<Round, Round>((Func<Round, Round>)ProcessItem, options);
        }

        public override Round DoWork(Round item)
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

            return item;
        }
    }
}