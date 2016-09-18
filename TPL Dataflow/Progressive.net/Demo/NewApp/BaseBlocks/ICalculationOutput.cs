using System.Threading.Tasks.Dataflow;

namespace NewApp.BaseBlocks
{
    public interface ICalculationOutput<T>
    {
        ISourceBlock<T> GetOutput();
        ICalculation<T> Then(ICalculation<T> next);
        ITargetBlock<T> Then(ITargetBlock<T> nullTarget);

        ICalculationTarget<T> Then(ICalculationTarget<T> next);
    }
}