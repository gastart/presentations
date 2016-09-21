using System;
using System.Threading.Tasks.Dataflow;

namespace BasicBlocks
{
    public class CustomTransformBlock<T> : ICustomBlock<T>
    {
        private readonly TransformBlock<T, T> _transformBlock;
        public ISourceBlock<T> OutputBlock => _transformBlock;
        public ITargetBlock<T> TargetBlock => _transformBlock;


        public CustomTransformBlock()
        {
            _transformBlock = new TransformBlock<T, T>((Func<T, T>)DoWork);
        }

        private T DoWork(T item)
        {
            return item;
        }

        public bool Post(T item)
        {
            return _transformBlock.Post(item);
        }

        public ICustomBlock<T> Then(ICustomBlock<T> block)
        {
            OutputBlock.LinkTo(block.TargetBlock, new DataflowLinkOptions {PropagateCompletion = true});
            return block;
        }

        public ITargetBlock<int> ThenTerminate(ITargetBlock<int> next)
        {
            //OutputBlock.LinkTo(next);
            return next;
        }


        public void Complete()
        {
            _transformBlock.Complete();
        }
    }
}