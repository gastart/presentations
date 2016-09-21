using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using NewApp.BaseBlocks;
using NewApp.Domain;

namespace NewApp.Blocks
{
    public class ScaleBlock : CalculationSameBaseBlock<Trial>
    {
        public sealed override string BlockName => GetType().Name;

        private readonly decimal _adjustmentFactor;

        public ScaleBlock(decimal adjustmentFactor, ExecutionDataflowBlockOptions options)
        {
            _adjustmentFactor = adjustmentFactor;
            ProcessingBlock = new TransformBlock<Trial, Trial>((Func<Trial, Trial>) ProcessItem, options);
        }

        public override Trial DoWork(Trial item)
        {
            if (item.Losses != null && item.Losses.Any())
            {
                foreach (var loss in item.Losses)
                {
                    loss.Scale(_adjustmentFactor);
                }
            }
            return item;
        }
    }
}