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

        private readonly JoinBlock<Trial, Trial> _joinBlock;
        private readonly BufferBlock<Trial> _outputBuffer;

        public JoinLossesBlock()
        {
            _joinBlock = new JoinBlock<Trial, Trial>();
            _outputBuffer = new BufferBlock<Trial>();

            var joinAction = new ActionBlock<Tuple<Trial, Trial>>(items =>
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

        private Trial DoWork(Tuple<Trial, Trial> r)
        {

            if(r.Item1.Id != r.Item2.Id)
                throw new ArgumentException();

            var round = new Trial
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


        public void Post1(Trial item)
        {
            _joinBlock.Target1.SendAsync(item);
        }

        public void Post2(Trial item)
        {
            _joinBlock.Target2.SendAsync(item);
        }

        public override int InputCount() => -1;
        public override int OutputCount() => _joinBlock.OutputCount;

        public ISourceBlock<Tuple<Trial, Trial>> GetInputBlock()
        {
            return _joinBlock;
        }

        public ICalculation<Trial> Then(ICalculation<Trial> next)
        {
            var targetBlock = next.GetTargetBlock();
            _outputBuffer.LinkTo(targetBlock, new DataflowLinkOptions { PropagateCompletion = true });

            AddNextBlock(next.GetBaseBlock());
            return next;
        }

        public ITargetBlock<Trial> Then(ITargetBlock<Trial> nullTarget)
        {
            _outputBuffer.LinkTo(nullTarget, new DataflowLinkOptions { PropagateCompletion = true });
            return nullTarget;
        }

        public ICalculationTarget<Trial> Then(ICalculationTarget<Trial> next)
        {
            Then(next.GetTargetBlock());
            AddNextBlock(next.GetBaseBlock());
            return next;
        }
    }
}