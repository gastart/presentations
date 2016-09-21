using System;
using System.Threading.Tasks.Dataflow;
using Common.Domain;
using Common.Helpers;

namespace Presentation
{
    class Program
    {
        static void Main(string[] args)
        {
            var joiner = new JoinBlock<Trial,Trial>();
            var transformer = new TransformBlock<Tuple<Trial,Trial>, Trial>(t =>
            {
                Trial combined = new Trial(t.Item1.Id);
                combined.Losses.AddRange(t.Item1.Losses);
                combined.Losses.AddRange(t.Item2.Losses);
                return combined;
            });

            var manipulator = new TransformBlock<Trial, Trial>(x =>
            {
                foreach (var loss in x.Losses)
                {
                    loss.Amount *= 0.8m;
                }

                return x;
            });

            var sb = new StatisticsBlock();

            joiner.LinkTo(transformer, new DataflowLinkOptions {PropagateCompletion = true});
            transformer.LinkTo(manipulator, new DataflowLinkOptions {PropagateCompletion = true});
            manipulator.LinkTo(sb.Block, new DataflowLinkOptions {PropagateCompletion = true});
            //sb.Block.LinkTo(DataflowBlock.NullTarget<Trial>());

            Generate(10, joiner.Target1);
            Generate(100, joiner.Target2);

            joiner.Complete();

            sb.Block.Completion.Wait();

            Console.WriteLine(sb.Average());
            Console.WriteLine(sb.Tvar10());
            Console.WriteLine(sb.Poh());

            Console.WriteLine("Done");

            Console.ReadKey();

        }

        private static void Generate(int i, ITargetBlock<Trial> target)
        {
            LossSimulator sim = new LossSimulator();

            foreach (var trial in sim.GenerateRandom(20000, i))
            {
                target.Post(trial);
            }
        }
    }
}
