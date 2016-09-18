using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NewApp.Blocks;
using NewApp.DataAccess;
using NewApp.Domain;

namespace NewApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> sources = new List<string> {"", ""};
            string inFile = @"c:\temp\losses_large_streamed_in1";
            string outFile = @"c:\temp\losses_large_streamed_out";

            var executionDataflowBlockOptionsSingle = new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 1};
            var executionDataflowBlockOptions = new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 1};

            var roundCount = 1000000;
            var dataReader = new DataReaderBlock(DataFormat.Protobuf, roundCount);
            var dataWriter = new DataWriterBlock(outFile, DataFormat.Protobuf, roundCount, executionDataflowBlockOptionsSingle);

            var customJoin = new CustomJoinBlock(2, executionDataflowBlockOptionsSingle);

            var joiner = new JoinBlock<Round, Round>();
            

            var limiter = new LimitBlock(30000, executionDataflowBlockOptions);
            var scaler = new ScaleBlock(0.9m, executionDataflowBlockOptions);
            var risk = new RiskMeasuresBlock(executionDataflowBlockOptionsSingle);
            var nullTarget = DataflowBlock.NullTarget<Round>();

            dataReader
                .Then(limiter)
                .Then(scaler)
                .Then(risk)
                .Then(dataWriter)
                .Then(nullTarget);


            foreach (var s in sources)
            {
                customJoin.GetTargetBlock().Post()
                var dataReader2 = new DataReaderBlock(DataFormat.Protobuf, roundCount);
                processingContainer.ConnectLossSource(loadRoundsBlock, prop.LossHref);
                dataReader.InputBlock.Post(inFile);
                dataReader.InputBlock.Complete();
            }

            var calculationStartDate = DateTime.UtcNow;
            dataReader.InputBlock.Post(inFile);
            dataReader.InputBlock.Complete();

            //risk.GetOutput().Completion.Wait();

            while (!risk.GetOutput().Completion.IsCompleted)
            {
                var totalTimeElapsed = Math.Round((DateTime.UtcNow - calculationStartDate).TotalSeconds, 2);
                var calculatedTrials = risk.GetProcessedItemsCount();
                var percentComplete = (float)Math.Round((calculatedTrials / (float)roundCount) * 100, 2);

                Console.WriteLine($"Elapsed: {totalTimeElapsed}s, {calculatedTrials}/{roundCount} ({percentComplete}%)");

                var tasksToWaitFor = new[] { Task.Delay(1000), risk.GetOutput().Completion };
                Task.WaitAny(tasksToWaitFor);
            }
            Console.WriteLine("");
            var res = risk.GetCalculationResult();
            Console.WriteLine(res.ToString());

            Console.ReadKey();

        }
    }
}
