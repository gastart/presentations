using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace NewApp.BaseBlocks
{
    public abstract class ProducerBlock<TInput, TOutput> : BaseBlock, ICalculationOutput<TOutput>
    {
        private readonly BufferBlock<TOutput> _outputBuffer;

        protected ProducerBlock()
        { 
            _outputBuffer = new BufferBlock<TOutput>();
            InputBlock = new ActionBlock<TInput>(input => ProcessItem(input));

            InputBlock.Completion.ContinueWith(_ =>
            {
                if (InputBlock.Completion.IsFaulted)
                {
                    ((IDataflowBlock)_outputBuffer).Fault(InputBlock.Completion.Exception);
                }
                else
                {
                    _outputBuffer.Complete();
                }

            });
        }

        public override int InputCount() => InputBlock.InputCount;


        public override int OutputCount() => _outputBuffer.Count;



        public ActionBlock<TInput> InputBlock { get; protected set; }


        public void ProcessItem(TInput item)
        {
            // IEnumerable (yielding). Statistics collection impossible
            foreach (var yieldedOutput in DoWork(item))
            {
                _outputBuffer.SendAsync(yieldedOutput).Wait();
            }

        }

        public abstract IEnumerable<TOutput> DoWork(TInput item);

        public ISourceBlock<TOutput> GetOutput()
        {
            return _outputBuffer;
        }

        public ICalculation<TOutput> Then(ICalculation<TOutput> next)
        {
            ThenToTargetBlockWithoutDescription(next.GetTargetBlock());
            return next;
        }

        public ITargetBlock<TOutput> ThenToTargetBlockWithoutDescription(ITargetBlock<TOutput> next)
        {
            GetOutput().LinkTo(next, new DataflowLinkOptions { PropagateCompletion = true });
            return next;
        }

        public ICalculationTarget<TOutput> Then(ICalculationTarget<TOutput> next)
        {
            ThenToTargetBlockWithoutDescription(next.GetTargetBlock());
            return next;
        }

    }
}