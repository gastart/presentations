using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using NewApp.BaseBlocks;
using NewApp.Domain;

namespace NewApp.Blocks
{
    public class JoinLossesBlock : BaseBlock
    {
        public override string BlockName => "JoinLossesBlock";

        private readonly JoinBlock<Round, Round> _joinBlock;
        private readonly BufferBlock<Round> _outputBuffer;

        public JoinLossesBlock()
        {
            _joinBlock = new JoinBlock<Round, Round>();
            _outputBuffer = new BufferBlock<Round>();

            var joinAction = new ActionBlock<Tuple<Round, Round>>(items =>
            {
                var joined = DoWork(items);
                _outputBuffer.SendAsync(joined).Wait();
            });

            joinAction.Completion.ContinueWith(_ =>
            {

                if (joinAction.Completion.IsFaulted)
                {
                    ((IDataflowBlock)_outputBuffer).Fault(joinAction.Completion.Exception);
                }
                else
                {
                    _outputBuffer.Complete();
                }

            });

            _joinBlock.LinkTo(joinAction,new DataflowLinkOptions {PropagateCompletion = true});
        }

        private Round DoWork(Tuple<Round, Round> r)
        {

            if(r.Item1.Id != r.Item2.Id)
                throw new ArgumentException();

            var round = new Round
            {
                Id = r.Item1.Id,
                Losses = new List<Loss>()
            };

            if(r.Item1.Losses != null)
                round.Losses.AddRange(r.Item1.Losses);

            if (r.Item2.Losses != null)
                round.Losses.AddRange(r.Item2.Losses);

            return round;
        }


        public void Post1(Round item)
        {
            _joinBlock.Target1.SendAsync(item);
        }

        public void Post2(Round item)
        {
            _joinBlock.Target2.SendAsync(item);
        }

        public override int InputCount() => -1;
        public override int OutputCount() => _joinBlock.OutputCount;

        public ISourceBlock<Tuple<Round, Round>> GetInputBlock()
        {
            return _joinBlock;
        }

        public ICalculation<Round> Then(ICalculation<Round> next)
        {
            var targetBlock = next.GetTargetBlock();
            _outputBuffer.LinkTo(targetBlock, new DataflowLinkOptions { PropagateCompletion = true });

            AddNextBlock(next.GetBaseBlock());
            return next;
        }

        public ITargetBlock<Round> Then(ITargetBlock<Round> nullTarget)
        {
            _outputBuffer.LinkTo(nullTarget, new DataflowLinkOptions { PropagateCompletion = true });
            return nullTarget;
        }

        public ICalculationTarget<Round> Then(ICalculationTarget<Round> next)
        {
            Then(next.GetTargetBlock());
            AddNextBlock(next.GetBaseBlock());
            return next;
        }
    }
}