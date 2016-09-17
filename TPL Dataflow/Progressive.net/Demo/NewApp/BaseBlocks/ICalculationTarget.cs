using System.Threading.Tasks.Dataflow;

namespace NewApp.BaseBlocks
{
    public interface ICalculationTarget<T>
    {
        ITargetBlock<T> GetTargetBlock();
        BaseBlock GetBaseBlock();
    }
}