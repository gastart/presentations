using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Common.DataAccess;
using Common.Domain;
using Common.Helpers;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            LossSimulator lossSimulator = new LossSimulator();
            var fileName = @"c:\temp\losses_small_all";
            var fileName2 = @"c:\temp\losses_large_all";

            var losses = lossSimulator.GenerateStatic(20000, 100);
            var losses2 = lossSimulator.GenerateStatic(100000, 100);

            var betterLosses = lossSimulator.GenerateRandom(1000000, 1000);

            var protobufData = new ProtobufData<Trial>();
            var wireData = new WireData<Trial>();

            //wireData.WriteAll(betterLosses.ToList(), @"c:\temp\losses_small_better", Console.WriteLine);
            protobufData.WriteStreamedData(@"c:\temp\losses_large_streamed", betterLosses, Console.WriteLine);
            wireData.WriteStreamedData(@"c:\temp\losses_large_streamed", betterLosses, Console.WriteLine);

            //protobufData.WriteAll(losses.ToList(), fileName, Console.WriteLine);
            //protobufData.WriteAll(losses2.ToList(), fileName2, Console.WriteLine);
            //wireData.WriteAll(losses.ToList(), fileName, Console.WriteLine);
            //wireData.WriteAll(losses2.ToList(), fileName2, Console.WriteLine);

            List<Trial> r = protobufData.ReadAll(fileName);
            List<Trial> r2 = protobufData.ReadAll(fileName2);

            List<Trial> r3 = wireData.ReadAll(fileName);
            List<Trial> r4 = wireData.ReadAll(fileName2);


            //int roundCount = protobufData.WriteStreamedData(fileName + ".pb", losses, Console.WriteLine);
            //int roundCount2 = wireData.WriteStreamedData(fileName + ".wir", losses, Console.WriteLine);

            // IEnumerable<Trial> allData = wireData.GetStreamedData(fileName + ".wir", roundCount2);
            //IEnumerable<Trial> allData2 = protobufData.GetStreamedData(fileName + ".pb", roundCount);

            ActionBlock<Trial> consoleWriter = new ActionBlock<Trial>(x => Console.WriteLine(x));
            TransformBlock<Trial, Trial> aggregator = new TransformBlock<Trial, Trial>(x => x, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = 1
            });

            aggregator.LinkTo(consoleWriter);


            //foreach (var round in allData2)
            //{
            //    aggregator.Post(round);
            //}

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
