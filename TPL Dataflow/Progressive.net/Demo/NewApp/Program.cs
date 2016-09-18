using System;
using System.Threading.Tasks.Dataflow;
using Common.Domain;
using NewApp.Blocks;

namespace NewApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string file1 = @"c:\temp\losses_large_all";

            var dataReader = new DataReaderBlock();
            var executionDataflowBlockOptionsSingle = new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 1};
            var executionDataflowBlockOptions = new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 10};

            var limiter = new LimitBlock(30000, executionDataflowBlockOptions);
            var scaler = new ScaleBlock(0.9m, executionDataflowBlockOptions);
            var risk = new RiskMeasuresBlock(executionDataflowBlockOptionsSingle);
            var nullTarget = DataflowBlock.NullTarget<Round>();

            dataReader
                .Then(limiter)
                .Then(scaler)
                .Then(risk)
                .ThenToTargetBlockWithoutDescription(nullTarget);

            dataReader.InputBlock.Post(file1);

            dataReader.InputBlock.Complete();
            risk.GetOutput().Completion.Wait();

            var res = risk.GetCalculationResult();
            Console.WriteLine(res.ToString());

            Console.ReadKey();

        }
    }
}
