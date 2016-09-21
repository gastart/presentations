using System.Threading.Tasks.Dataflow;

namespace BasicBlocks
{
    public interface ICustomBlock<T> : ICustomTargetBlock<T>, ICustomSourceBlock<T>
    {
       
    }

    public interface ICustomTargetBlock<T>
    {
        ITargetBlock<T> TargetBlock { get; }

        bool Post(T item);

        void Complete();
    }

    public interface ICustomSourceBlock<T>
    {
        ICustomBlock<T> Then(ICustomBlock<T> block);

        ITargetBlock<int> ThenTerminate(ITargetBlock<int> next);

        ISourceBlock<T> OutputBlock { get; }

    }
}