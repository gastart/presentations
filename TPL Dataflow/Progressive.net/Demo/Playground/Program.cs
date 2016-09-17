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

            var losses = lossSimulator.Generate(20000, 100);
            var losses2 = lossSimulator.Generate(100000, 100);

            var betterLosses = lossSimulator.GenerateBetter(20000);

            var protobufData = new ProtobufData<Round>();
            var wireData = new WireData<Round>();

            wireData.WriteAll(betterLosses.ToList(), @"c:\temp\losses_small_better", Console.WriteLine);

            //protobufData.WriteAll(losses.ToList(), fileName, Console.WriteLine);
            //protobufData.WriteAll(losses2.ToList(), fileName2, Console.WriteLine);
            //wireData.WriteAll(losses.ToList(), fileName, Console.WriteLine);
            //wireData.WriteAll(losses2.ToList(), fileName2, Console.WriteLine);

            List<Round> r = protobufData.ReadAll(fileName);
            List<Round> r2 = protobufData.ReadAll(fileName2);

            List<Round> r3 = wireData.ReadAll(fileName);
            List<Round> r4 = wireData.ReadAll(fileName2);


            //int roundCount = protobufData.WriteStreamedData(fileName + ".pb", losses, Console.WriteLine);
            //int roundCount2 = wireData.WriteStreamedData(fileName + ".wir", losses, Console.WriteLine);

            // IEnumerable<Round> allData = wireData.GetStreamedData(fileName + ".wir", roundCount2);
            //IEnumerable<Round> allData2 = protobufData.GetStreamedData(fileName + ".pb", roundCount);

            ActionBlock<Round> consoleWriter = new ActionBlock<Round>(x => Console.WriteLine(x));
            TransformBlock<Round, Round> aggregator = new TransformBlock<Round, Round>(x => x, new ExecutionDataflowBlockOptions()
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
