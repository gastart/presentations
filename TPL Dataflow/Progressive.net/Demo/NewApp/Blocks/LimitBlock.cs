using System;
using System.Threading.Tasks.Dataflow;
using Common.Domain;
using NewApp.BaseBlocks;

namespace NewApp.Blocks
{
    public class LimitBlock : CalculationSameBaseBlock<Round>
    {
        public override string BlockName => "LimitBlock";

        public LimitBlock()
        {
            ProcessingBlock = new TransformBlock<Round, Round>((Func<Round, Round>)ProcessItem);
        }

        public override Round DoWork(Round item)
        {
            throw new System.NotImplementedException();
        }
    }
}