using System.Linq;
using System.Threading.Tasks.Dataflow;
using NewApp.BaseBlocks;
using NewApp.Domain;

namespace NewApp.Blocks
{
    public class CustomJoinBlock : BaseBlock, ICalculationOutput<Round>, ICalculationTarget<Round>
    {
        public sealed override string BlockName => "CustomJoinBlock";

        private readonly BatchBlock<Round> _batchBlock;

        private readonly BufferBlock<Round> _outputBuffer;
        private readonly ActionBlock<Round[]> _joinAction;
        private int _sourceCount;


        public CustomJoinBlock(int n, ExecutionDataflowBlockOptions options)
        {
            _sourceCount = n;

            _outputBuffer = new BufferBlock<Round>(options);

            // A BatchBlock with non-greedy mode set will wait for one item from each connected input. Hence it will
            // work like a join-block but with arbitrary number of inputs.

            _batchBlock = new BatchBlock<Round>(n);

            // Pretty important that this is only handled by one single thread
            var actionBlockOptions = new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 1};
            actionBlockOptions.MaxDegreeOfParallelism = 1;
            _joinAction = new ActionBlock<Round[]>(items =>
            {
                var joined = Join(items);
                _outputBuffer.SendAsync(joined).Wait();
            }, actionBlockOptions);

            _joinAction.Completion.ContinueWith(_ =>
            {

                if (_joinAction.Completion.IsFaulted)
                {
                    ((IDataflowBlock)_outputBuffer).Fault(_joinAction.Completion.Exception);
                }
                else
                {
                    _outputBuffer.Complete();
                }

            });

            _batchBlock.LinkTo(_joinAction, new DataflowLinkOptions() { PropagateCompletion = true });
        }

        public override int InputCount()
        {
            return _batchBlock.OutputCount * _batchBlock.BatchSize;
        }

        public override int OutputCount() => _outputBuffer.Count;

        private Round Join(Round[] items)
        {

            var first = items.First();
            var result = new Round { Id = first.Id };

            var allLosses = items.SelectMany(_ => _.Losses).OrderBy(_ => _.OccuredAt).ToList();
          
            result.Losses.AddRange(allLosses);

            return result;
        }

        public ISourceBlock<Round> GetOutput()
        {
            return _outputBuffer;
        }

        public ICalculation<Round> Then(ICalculation<Round> next)
        {
            ThenToTargetBlockWithoutDescription(next.GetTargetBlock());
            AddNextBlock(next.GetBaseBlock());
            return next;
        }

        public ITargetBlock<Round> Then(ITargetBlock<Round> nullTarget)
        {
            ThenToTargetBlockWithoutDescription(nullTarget);
            return nullTarget;
        }

        public ITargetBlock<Round> ThenToTargetBlockWithoutDescription(ITargetBlock<Round> block)
        {
            GetOutput().LinkTo(block, new DataflowLinkOptions { PropagateCompletion = true });
            return block;
        }

        public ICalculationTarget<Round> Then(ICalculationTarget<Round> next)
        {
            ThenToTargetBlockWithoutDescription(next.GetTargetBlock());
            AddNextBlock(next.GetBaseBlock());
            return next;
        }

        public ITargetBlock<Round> GetTargetBlock()
        {
            return _batchBlock;
        }

        public BaseBlock GetBaseBlock()
        {
            return this;
        }
    }
}