using System.Threading.Tasks.Dataflow;

namespace BasicBlocks
{
    public class CustomBufferBlock<T> : ICustomSourceBlock<T>
    {
        private readonly BufferBlock<T> _buffer;
        

        public ISourceBlock<T> OutputBlock => _buffer;
        public ITargetBlock<T> TargetBlock => _buffer;


        public CustomBufferBlock()
        {
            _buffer = new BufferBlock<T>();
        }

        public ICustomBlock<T> Then(ICustomBlock<T> block)
        {
            _buffer.LinkTo(block.TargetBlock, new DataflowLinkOptions { PropagateCompletion = true });
            return  block;
        }

        public ITargetBlock<int> ThenTerminate(ITargetBlock<int> next)
        {
            throw new System.NotImplementedException();
        }

        public bool Post(T item)
        {
            return _buffer.Post(item);
        }

        public void Complete()
        {
            _buffer.Complete();
        }
    }
}