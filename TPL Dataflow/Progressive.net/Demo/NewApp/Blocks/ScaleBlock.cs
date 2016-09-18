using System;
using System.Threading.Tasks.Dataflow;
using Common.Domain;
using NewApp.BaseBlocks;

namespace NewApp.Blocks
{
    public class ScaleBlock : CalculationSameBaseBlock<Round>
    {
        public sealed override string BlockName => GetType().Name;

        private readonly decimal _adjustmentFactor;

        public ScaleBlock(decimal adjustmentFactor, ExecutionDataflowBlockOptions options)
        {
            _adjustmentFactor = adjustmentFactor;
            ProcessingBlock = new TransformBlock<Round, Round>((Func<Round, Round>) ProcessItem, options);
        }

        public override Round DoWork(Round item)
        {
            foreach (var loss in item.Losses)
            {
                loss.Scale(_adjustmentFactor);
            }
            return item;
        }
    }
}