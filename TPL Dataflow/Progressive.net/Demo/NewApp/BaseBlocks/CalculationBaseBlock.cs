using System.Threading.Tasks.Dataflow;

namespace NewApp.BaseBlocks
{
    public abstract class CalculationBaseBlock<TInput, TOutput> : BaseBlock, ICalculationTarget<TInput>, ICalculationOutput<TOutput>
    {
        protected TransformBlock<TInput, TOutput> ProcessingBlock;
        public override int InputCount() => ProcessingBlock.InputCount;
        public override int OutputCount() => ProcessingBlock.OutputCount;

        public TOutput ProcessItem(TInput item)
        {
            return DoWork(item);
        }

        public abstract TOutput DoWork(TInput item);


        public ITargetBlock<TOutput> ThenToTargetBlockWithoutDescription(ITargetBlock<TOutput> block)
        {
            GetOutput().LinkTo(block, new DataflowLinkOptions { PropagateCompletion = true });
            return block;
        }

        public ICalculationTarget<TOutput> Then(ICalculationTarget<TOutput> next)
        {
            ThenToTargetBlockWithoutDescription(next.GetTargetBlock());
            AddNextBlock(next.GetBaseBlock());
            return next;
        }

        public ITargetBlock<TInput> GetTargetBlock()
        {
            return ProcessingBlock;
        }

        public BaseBlock GetBaseBlock()
        {
            return this;
        }

        public ISourceBlock<TOutput> GetOutput()
        {
            return ProcessingBlock;
        }

        ICalculation<TOutput> ICalculationOutput<TOutput>.Then(ICalculation<TOutput> next)
        {
            var sourceBlock = GetOutput();
            var targetBlock = next.GetTargetBlock();
            sourceBlock.LinkTo(targetBlock, new DataflowLinkOptions { PropagateCompletion = true });

            AddNextBlock(next.GetBaseBlock());
            return next;
        }
    }
}
