using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BasicBlocks
{
    public class StatefulBlock : ICustomBlock<int>
    {
        private readonly TransformBlock<int, int> _transformBlock;

        public ISourceBlock<int> OutputBlock => _transformBlock;
        public ITargetBlock<int> TargetBlock => _transformBlock;
        public Task Completion => _transformBlock.Completion;


        private readonly List<int> _values = new List<int>();

        public StatefulBlock()
        {
            _transformBlock = new TransformBlock<int, int>((Func<int, int>)DoWork, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });
        }

        private int DoWork(int item)
        {
            _values.Add(item);
            return item;
        }

        public double Average()
        {
            return _values.Average(x => x);
        }

        public bool Post(int item)
        {
            return TargetBlock.Post(item);
        }

        public void Complete()
        {
            TargetBlock.Complete();
        }

        public ICustomBlock<int> Then(ICustomBlock<int> block)
        {
            OutputBlock.LinkTo(block.TargetBlock, new DataflowLinkOptions { PropagateCompletion = true });
            return block;
        }

        public ITargetBlock<int> ThenTerminate(ITargetBlock<int> next)
        {
            OutputBlock.LinkTo(next, new DataflowLinkOptions { PropagateCompletion = true });
            return next;
        }
    }
}